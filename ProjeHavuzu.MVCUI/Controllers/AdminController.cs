using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.DTOs.LogDto;
using ProjeHavuzu.Data.Entites.Enums;
using ProjeHavuzu.Data.Entites.Identity;
using ProjeHavuzu.MVCUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.MVCUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUnifiedLogService _unifiedLogService;
        private readonly IProjectService _projectService;
        private readonly IProjectRequestService _projectRequestService;
        private readonly IFacultyService _facultyService;
        private readonly IDepartmentService _departmentService;

        public AdminController(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IUnifiedLogService unifiedLogService,
            IProjectService projectService,
            IProjectRequestService projectRequestService,
            IFacultyService facultyService,
            IDepartmentService departmentService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unifiedLogService = unifiedLogService;
            _projectService = projectService;
            _projectRequestService = projectRequestService;
            _facultyService = facultyService;
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(MyProjects));
        }

        public async Task<IActionResult> MyProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return View(projects);
        }

        public async Task<IActionResult> AdvisorApprovals()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            var pending = projects.Where(p => p.ApprovalStatus == ProjectApprovalStatus.Pending).ToList();
            return View(pending);
        }

        public async Task<IActionResult> ProjectRequests()
        {
            var requests = await _projectRequestService.GetAllPendingRequestsAsync();
            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveProject(Guid id, string? returnUrl = null)
        {
            try
            {
                var admin = await _userManager.GetUserAsync(User);
                if (admin == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                await _projectService.ApproveProjectAsync(id, admin.Id, isAdmin: true);
                TempData["SuccessMessage"] = "Proje onaylandı.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAdminReturn(returnUrl, nameof(AdvisorApprovals));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectProject(Guid id, string rejectionReason, string? returnUrl = null)
        {
            try
            {
                var admin = await _userManager.GetUserAsync(User);
                if (admin == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                await _projectService.RejectProjectAsync(id, admin.Id, rejectionReason, isAdmin: true);
                TempData["SuccessMessage"] = "Proje reddedildi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAdminReturn(returnUrl, nameof(AdvisorApprovals));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequest(Guid requestId, string? responseMessage, string? returnUrl = null)
        {
            var result = await _projectRequestService.ApproveRequestAsync(requestId, responseMessage);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                ? "Proje isteği onaylandı."
                : "İstek onaylanırken bir hata oluştu.";
            return RedirectToAdminReturn(returnUrl, nameof(ProjectRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(Guid requestId, string? responseMessage, string? returnUrl = null)
        {
            var result = await _projectRequestService.RejectRequestAsync(requestId, responseMessage);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                ? "Proje isteği reddedildi."
                : "İstek reddedilirken bir hata oluştu.";
            return RedirectToAdminReturn(returnUrl, nameof(ProjectRequests));
        }

        public async Task<IActionResult> Academicians()
        {
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            var roleMap = new Dictionary<Guid, IList<string>>();
            foreach (var teacher in teachers)
            {
                roleMap[teacher.Id] = await _userManager.GetRolesAsync(teacher);
            }

            ViewBag.UserRoles = roleMap;
            return View(teachers);
        }

        [HttpGet]
        public async Task<IActionResult> AddAcademician()
        {
            var model = new AdminAddAcademicianViewModel
            {
                Faculties = await _facultyService.GetAllFacultiesAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAcademician(AdminAddAcademicianViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Faculties = await _facultyService.GetAllFacultiesAsync();
                if (model.FacultyId.HasValue)
                {
                    model.Departments = await _departmentService.GetDepartmentsByFacultyIdAsync(model.FacultyId.Value);
                }
                return View(model);
            }

            if (!model.Email.Trim().EndsWith("@ozal.edu.tr", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Email", "Akademisyen e-postası @ozal.edu.tr uzantılı olmalıdır.");
                model.Faculties = await _facultyService.GetAllFacultiesAsync();
                return View(model);
            }

            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
                model.Faculties = await _facultyService.GetAllFacultiesAsync();
                return View(model);
            }

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = model.Email.Trim(),
                Email = model.Email.Trim(),
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                StaffNumber = model.StaffNumber.Trim(),
                AcademicTitle = model.AcademicTitle,
                PhoneNumber = model.PhoneNumber,
                FacultyId = model.FacultyId,
                DepartmentId = model.DepartmentId,
                EmailConfirmed = true,
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                model.Faculties = await _facultyService.GetAllFacultiesAsync();
                if (model.FacultyId.HasValue)
                {
                    model.Departments = await _departmentService.GetDepartmentsByFacultyIdAsync(model.FacultyId.Value);
                }
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Teacher");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                TempData["ErrorMessage"] = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Academicians));
            }

            TempData["SuccessMessage"] = $"{user.FullName} akademisyen olarak eklendi. Giriş bilgileri paylaşılabilir.";
            return RedirectToAction(nameof(Academicians));
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartmentsByFaculty(Guid facultyId)
        {
            var departments = await _departmentService.GetDepartmentsByFacultyIdAsync(facultyId);
            return Json(departments.Select(d => new { id = d.Id, name = d.DepartmentName }));
        }

        public async Task<IActionResult> Students()
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateStudent(Guid studentId, string reason)
        {
            var student = await _userManager.FindByIdAsync(studentId.ToString());
            if (student == null || !await _userManager.IsInRoleAsync(student, "Student"))
            {
                TempData["ErrorMessage"] = "Öğrenci bulunamadı.";
                return RedirectToAction(nameof(Students));
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["ErrorMessage"] = "Pasifleştirme nedeni boş olamaz.";
                return RedirectToAction(nameof(Students));
            }

            student.IsActive = false;
            student.DeactivationReason = reason.Trim();
            var result = await _userManager.UpdateAsync(student);

            TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded
                ? $"{student.FullName} pasifleştirildi."
                : string.Join(", ", result.Errors.Select(e => e.Description));

            return RedirectToAction(nameof(Students));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateStudent(Guid studentId)
        {
            var student = await _userManager.FindByIdAsync(studentId.ToString());
            if (student == null || !await _userManager.IsInRoleAsync(student, "Student"))
            {
                TempData["ErrorMessage"] = "Öğrenci bulunamadı.";
                return RedirectToAction(nameof(Students));
            }

            student.IsActive = true;
            student.DeactivationReason = null;
            var result = await _userManager.UpdateAsync(student);

            TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded
                ? $"{student.FullName} aktifleştirildi."
                : "Öğrenci durumu güncellenirken bir hata oluştu.";

            return RedirectToAction(nameof(Students));
        }

        [HttpGet]
        public async Task<IActionResult> AssignRole(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Academicians));
            }

            var availableRoles = new[] { "Teacher", "Student", "Admin" };
            var model = new AdminAssignRoleViewModel
            {
                UserId = user.Id,
                UserFullName = user.FullName ?? $"{user.FirstName} {user.LastName}",
                Email = user.Email ?? string.Empty,
                CurrentRoles = (await _userManager.GetRolesAsync(user)).ToList(),
                AvailableRoles = availableRoles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(AdminAssignRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Academicians));
            }

            if (string.IsNullOrWhiteSpace(model.SelectedRole))
            {
                TempData["ErrorMessage"] = "Lütfen bir rol seçiniz.";
                return RedirectToAction(nameof(AssignRole), new { userId = model.UserId });
            }

            if (!await _roleManager.RoleExistsAsync(model.SelectedRole))
            {
                TempData["ErrorMessage"] = "Seçilen rol sistemde bulunamadı.";
                return RedirectToAction(nameof(AssignRole), new { userId = model.UserId });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    TempData["ErrorMessage"] = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(AssignRole), new { userId = model.UserId });
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
            if (addResult.Succeeded)
            {
                TempData["SuccessMessage"] = $"{user.FullName} kullanıcısına '{model.SelectedRole}' rolü atandı.";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join(", ", addResult.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Academicians));
        }

        /// <summary>
        /// Birleşik Log Yönetimi Sayfası
        /// GET: /Admin/Logs
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logs([FromQuery] UnifiedLogFilterDto? filter)
        {
            filter ??= new UnifiedLogFilterDto();
            var viewModel = await _unifiedLogService.GetLogsViewModelAsync(filter);
            return View(viewModel);
        }

        /// <summary>
        /// DataTables server-side processing AJAX endpoint (Logs).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GetLogsData([FromForm] UnifiedLogFilterDto filter)
        {
            try
            {
                var request = new DataTableRequest
                {
                    Draw = int.TryParse(Request.Form["draw"], out var draw) ? draw : 1,
                    Start = int.TryParse(Request.Form["start"], out var start) ? start : 0,
                    Length = int.TryParse(Request.Form["length"], out var length) ? length : 50,
                    Search = new DataTableSearch
                    {
                        Value = Request.Form["search[value]"].ToString()
                    },
                    Columns = new List<DataTableColumn>(),
                    Order = new List<DataTableOrder>()
                };

                var colIdx = 0;
                while (!string.IsNullOrEmpty(Request.Form[$"columns[{colIdx}][data]"]))
                {
                    request.Columns.Add(new DataTableColumn
                    {
                        Data = Request.Form[$"columns[{colIdx}][data]"].ToString(),
                        Name = Request.Form[$"columns[{colIdx}][name]"].ToString(),
                        Searchable = Request.Form[$"columns[{colIdx}][searchable]"] == "true",
                        Orderable = Request.Form[$"columns[{colIdx}][orderable]"] == "true"
                    });
                    colIdx++;
                }

                var orderIdx = 0;
                while (!string.IsNullOrEmpty(Request.Form[$"order[{orderIdx}][column]"]))
                {
                    request.Order.Add(new DataTableOrder
                    {
                        Column = int.TryParse(Request.Form[$"order[{orderIdx}][column]"], out var col) ? col : 0,
                        Dir = Request.Form[$"order[{orderIdx}][dir]"].ToString()
                    });
                    orderIdx++;
                }

                var result = await _unifiedLogService.GetLogsDataAsync(request, filter);

                return Json(new
                {
                    draw = result.Draw,
                    recordsTotal = result.RecordsTotal,
                    recordsFiltered = result.RecordsFiltered,
                    data = result.Data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearOldLogs(int olderThanDays = 30)
        {
            var deletedCount = await _unifiedLogService.ClearOldLogsAsync(olderThanDays);
            TempData["SuccessMessage"] = $"{deletedCount} adet {olderThanDays} günden eski log kaydı temizlendi.";
            return RedirectToAction("Logs");
        }

        private string? GetSafeReturnUrl(string? returnUrl) =>
            !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : null;

        private IActionResult RedirectToAdminReturn(string? returnUrl, string fallbackAction)
        {
            var safeReturn = GetSafeReturnUrl(returnUrl);
            if (safeReturn != null)
                return LocalRedirect(safeReturn);

            return RedirectToAction(fallbackAction);
        }
    }
}
