using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.MVCUI.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IProjectStudentService _projectStudentService;
        private readonly IProjectRequestService _projectRequestService;
        private readonly IProjectSubmissionService _submissionService;
        private readonly UserManager<ProjeHavuzu.Data.Entites.Identity.AppUser> _userManager;
        private readonly IAdvisorCandidateService _advisorCandidateService;

        public TeacherController(
            IProjectService projectService,
            IProjectStudentService projectStudentService,
            IProjectRequestService projectRequestService,
            IProjectSubmissionService submissionService,
            UserManager<ProjeHavuzu.Data.Entites.Identity.AppUser> userManager,
            IAdvisorCandidateService advisorCandidateService)
        {
            _projectService = projectService;
            _projectStudentService = projectStudentService;
            _projectRequestService = projectRequestService;
            _submissionService = submissionService;
            _userManager = userManager;
            _advisorCandidateService = advisorCandidateService;
        }

        // Dashboard for teachers
        public async Task<IActionResult> Index()
        {
            // Bekleyen istek sayısını göster
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var pendingRequests = await _projectRequestService.GetPendingRequestsForConsultantAsync(user.Id);
                ViewBag.PendingRequestCount = pendingRequests.Count;
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyProjects()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var projects = await _projectService.GetProjectsByConsultantIdAsync(user.Id);
            return View(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveProject(Guid id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var isAdmin = User.IsInRole("Admin");
                await _projectService.ApproveProjectAsync(id, user.Id, isAdmin);
                TempData["SuccessMessage"] = "Proje başarıyla onaylandı.";
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(MyProjects));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectProject(Guid id, string rejectionReason)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var isAdmin = User.IsInRole("Admin");
                await _projectService.RejectProjectAsync(id, user.Id, rejectionReason, isAdmin);
                TempData["SuccessMessage"] = "Proje reddedildi.";
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(MyProjects));
        }

        [HttpGet]
        public IActionResult ProjectPool()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProjectDetails(Guid id, string? returnUrl = null)
        {
            try
            {
                var project = await _projectService.GetProjectDetailAsync(id);
                if (project == null)
                {
                    TempData["ErrorMessage"] = "Proje bulunamadı.";
                    return RedirectToTeacherReturn(returnUrl, nameof(AllProjects));
                }

                ViewBag.ReturnUrl = GetSafeReturnUrl(returnUrl) ?? Url.Action(nameof(AllProjects))!;
                ViewBag.AssignedStudents = await _projectStudentService.GetStudentsByProjectIdAsync(id);
                return View(project);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Proje yüklenirken hata oluştu: {ex.Message}";
                return RedirectToTeacherReturn(returnUrl, nameof(AllProjects));
            }
        }

        // ====================
        // All Projects View and Edit
        // ====================

        [HttpGet]
        public IActionResult AllProjects()
        {
            return View();
        }

        /// <summary>
        /// DataTables server-side processing AJAX endpoint (Teacher).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GetProjectsData()
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

            var result = await _projectService.GetProjectsServerSideAsync(request);

            return Json(new
            {
                draw = result.Draw,
                recordsTotal = result.RecordsTotal,
                recordsFiltered = result.RecordsFiltered,
                data = result.Data.Select(p => new
                {
                    p.Id,
                    projectTitle = p.ProjectTitle,
                    description = p.Description != null && p.Description.Length > 50
                        ? p.Description.Substring(0, 50) + "..."
                        : p.Description,
                    categoryName = p.CategoryName,
                    difficultyLevel = p.DifficultyLevel.ToString(),
                    completionPercentage = p.CompletionPercentage,
                    endDate = p.EndDate.ToString("dd.MM.yyyy"),
                    createdByFullName = p.CreatedByFullName
                })
            });
        }

        [HttpGet]
        public async Task<IActionResult> EditProject(Guid id, string? returnUrl = null)
        {
            try
            {
                var projectDto = await _projectService.GetProjectCreateDtoAsync();
                var projectDetail = await _projectService.GetProjectDetailAsync(id);

                if (projectDetail == null)
                {
                    TempData["ErrorMessage"] = "Proje bulunamadı.";
                    return RedirectToTeacherReturn(returnUrl, nameof(AllProjects));
                }

                // Map to create DTO for editing
                var editDto = new ProjeHavuzu.Data.DTOs.ProjectDto.ProjectCreateDto
                {
                    ProjectTitle = projectDetail.ProjectTitle,
                    Description = projectDetail.Description,
                    CategoryId = projectDetail.CategoryId,
                    DifficultyLevel = projectDetail.DifficultyLevel,
                    StartDate = projectDetail.StartDate,
                    EndDate = projectDetail.EndDate,
                    ProjectLink = projectDetail.ProjectLink,
                    ProjectArea = projectDetail.ProjectArea,
                    InitialCode = projectDetail.InitialCode,
                    // Mevcut aşamaları yükle
                    ExistingPhases = projectDetail.Phases?.Select(p => new ProjeHavuzu.Data.DTOs.ProjectDto.ProjectPhaseDto
                    {
                        Id = p.Id,
                        PhaseName = p.PhaseName,
                        Description = p.Description,
                        Order = p.Order,
                        IsCompleted = p.IsCompleted,
                        CompletedDate = p.CompletedDate
                    }).ToList(),
                    // Mevcut dilleri yükle
                    SelectedLanguagesJson = projectDetail.Languages != null ? string.Join(",", projectDetail.Languages) : ""
                };

                ViewBag.ReturnUrl = GetSafeReturnUrl(returnUrl) ?? Url.Action(nameof(ProjectDetails), new { id })!;
                await PrepareEditProjectViewDataAsync(id, editDto);
                return View(editDto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Proje yüklenirken hata oluştu: {ex.Message}";
                return RedirectToTeacherReturn(returnUrl, nameof(AllProjects));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProject(Guid id, ProjeHavuzu.Data.DTOs.ProjectDto.ProjectCreateDto projectDto, string? returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ReturnUrl = GetSafeReturnUrl(returnUrl) ?? Url.Action(nameof(ProjectDetails), new { id })!;
                    await PrepareEditProjectViewDataAsync(id, projectDto);
                    return View(projectDto);
                }

                var result = await _projectService.UpdateProjectAsync(id, projectDto);

                if (result)
                {
                    TempData["SuccessMessage"] = "Proje başarıyla güncellendi.";
                    return RedirectToTeacherReturn(returnUrl, nameof(ProjectDetails), new { id });
                }

                TempData["ErrorMessage"] = "Proje güncellenirken bir hata oluştu.";
                ViewBag.ReturnUrl = GetSafeReturnUrl(returnUrl) ?? Url.Action(nameof(ProjectDetails), new { id })!;
                await PrepareEditProjectViewDataAsync(id, projectDto);
                return View(projectDto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                ViewBag.ReturnUrl = GetSafeReturnUrl(returnUrl) ?? Url.Action(nameof(ProjectDetails), new { id })!;
                await PrepareEditProjectViewDataAsync(id, projectDto);
                return View(projectDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            try
            {
                await _projectService.SoftDeleteProjectAsync(id);
                TempData["SuccessMessage"] = "Proje başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
            }
            return RedirectToAction("AllProjects");
        }

        // ====================
        // Assign Students to Project
        // ====================

        [HttpGet]
        public async Task<IActionResult> AssignStudentsToProject(Guid projectId, string? returnUrl = null)
        {
            try
            {
                var dto = await _projectStudentService.GetAssignStudentsToProjectDtoAsync(projectId);
                ViewBag.ReturnUrl = GetSafeReturnUrl(returnUrl) ?? Url.Action(nameof(ProjectDetails), new { id = projectId })!;
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToTeacherReturn(returnUrl, nameof(AllProjects));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStudentsToProject(Guid projectId, List<Guid> selectedStudentIds, string? returnUrl = null)
        {
            try
            {
                if (selectedStudentIds == null)
                    selectedStudentIds = new List<Guid>();

                await _projectStudentService.AssignStudentsToProjectAsync(projectId, selectedStudentIds);
                TempData["SuccessMessage"] = "Öğrenci atamaları başarıyla güncellendi.";
                return RedirectToTeacherReturn(returnUrl, nameof(ProjectDetails), new { id = projectId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(AssignStudentsToProject), new { projectId, returnUrl });
            }
        }

        // ====================
        // Students List (for Assigning Projects)
        // ====================

        [HttpGet]
        public async Task<IActionResult> Students()
        {
            try
            {
                var students = await _userManager.GetUsersInRoleAsync("Student");
                return View(students);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // ====================
        // Assign Projects to Student
        // ====================

        [HttpGet]
        public async Task<IActionResult> AssignProjectsToStudent(Guid studentId)
        {
            try
            {
                var dto = await _projectStudentService.GetAssignProjectsToStudentDtoAsync(studentId);
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignProjectsToStudent(Guid studentId, List<Guid> selectedProjectIds)
        {
            try
            {
                if (selectedProjectIds == null)
                    selectedProjectIds = new List<Guid>();

                await _projectStudentService.AssignProjectsToStudentAsync(studentId, selectedProjectIds);
                TempData["SuccessMessage"] = "Proje atamaları başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction("AssignProjectsToStudent", new { studentId });
            }
        }

        // ====================
        // View Assignments
        // ====================

        [HttpGet]
        public async Task<IActionResult> ViewProjectStudents(Guid projectId, string? returnUrl = null)
        {
            try
            {
                var students = await _projectStudentService.GetStudentsByProjectIdAsync(projectId);
                var project = await _projectService.GetProjectByIdAsync(projectId);

                ViewBag.ProjectTitle = project?.ProjectTitle ?? "Unknown";
                ViewBag.ProjectId = projectId;
                ViewBag.ReturnUrl = GetSafeReturnUrl(returnUrl) ?? Url.Action(nameof(ProjectDetails), new { id = projectId })!;

                return View(students);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToTeacherReturn(returnUrl, nameof(AllProjects));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ProjectSubmissions()
        {
            var submissions = await _submissionService.GetAllSubmissionsAsync();

            var studentIds = submissions.Select(s => s.StudentId).Distinct().ToList();
            var studentNames = new Dictionary<Guid, string>();
            foreach (var sid in studentIds)
            {
                var user = await _userManager.FindByIdAsync(sid.ToString());
                studentNames[sid] = user != null ? $"{user.FirstName} {user.LastName}" : sid.ToString();
            }

            ViewBag.StudentNames = studentNames;
            return View(submissions);
        }

        // ====================
        // Deactivate Student (Pasifleştir)
        // ====================

        [HttpPost]
        public async Task<IActionResult> DeactivateStudent(Guid studentId, string reason)
        {
            try
            {
                var student = await _userManager.FindByIdAsync(studentId.ToString());
                if (student == null)
                {
                    TempData["ErrorMessage"] = "Öğrenci bulunamadı.";
                    return RedirectToAction("Students");
                }

                if (string.IsNullOrWhiteSpace(reason))
                {
                    TempData["ErrorMessage"] = "Pasifleştirme nedeni boş olamaz.";
                    return RedirectToAction("Students");
                }

                student.IsActive = false;
                student.DeactivationReason = reason;
                var result = await _userManager.UpdateAsync(student);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"{student.FullName} başarıyla pasifleştirildi.";
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData["ErrorMessage"] = $"Öğrenci durumu güncellenirken bir hata oluştu: {errors}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
            }
            return RedirectToAction("Students");
        }

        // ====================
        // Activate Student (Aktifleştir)
        // ====================

        [HttpPost]
        public async Task<IActionResult> ActivateStudent(Guid studentId)
        {
            try
            {
                var student = await _userManager.FindByIdAsync(studentId.ToString());
                if (student == null)
                {
                    TempData["ErrorMessage"] = "Öğrenci bulunamadı.";
                    return RedirectToAction("Students");
                }

                student.IsActive = true;
                student.DeactivationReason = null;
                var result = await _userManager.UpdateAsync(student);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"{student.FullName} başarıyla aktifleştirildi.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Öğrenci durumu güncellenirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
            }
            return RedirectToAction("Students");
        }

        // ====================
        // Project Requests Management
        // ====================

        [HttpGet]
        public async Task<IActionResult> ProjectRequests()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var requests = await _projectRequestService.GetPendingRequestsForConsultantAsync(user.Id);
                return View(requests);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"İstekler yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequest(Guid requestId, string? responseMessage)
        {
            try
            {
                var result = await _projectRequestService.ApproveRequestAsync(requestId, responseMessage);
                if (result)
                {
                    TempData["SuccessMessage"] = "İstek onaylandı ve öğrenci projeye atandı.";
                }
                else
                {
                    TempData["ErrorMessage"] = "İstek onaylanırken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
            }
            return RedirectToAction("ProjectRequests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(Guid requestId, string? responseMessage)
        {
            try
            {
                var result = await _projectRequestService.RejectRequestAsync(requestId, responseMessage);
                if (result)
                {
                    TempData["SuccessMessage"] = "İstek reddedildi.";
                }
                else
                {
                    TempData["ErrorMessage"] = "İstek reddedilirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
            }
            return RedirectToAction("ProjectRequests");
        }
        private async Task PrepareEditProjectViewDataAsync(Guid projectId, ProjeHavuzu.Data.DTOs.ProjectDto.ProjectCreateDto dto)
        {
            var projectDto = await _projectService.GetProjectCreateDtoAsync();
            dto.Categories = projectDto.Categories;

            dto.Academicians = await _advisorCandidateService.GetAdvisorCandidatesAsync();

            ViewBag.AcademicianCount = dto.Academicians?.Count ?? 0;
            ViewBag.ProjectId = projectId;
        }

        private string? GetSafeReturnUrl(string? returnUrl) =>
            !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : null;

        private IActionResult RedirectToTeacherReturn(string? returnUrl, string fallbackAction, object? fallbackRouteValues = null)
        {
            var safeReturn = GetSafeReturnUrl(returnUrl);
            if (safeReturn != null)
                return LocalRedirect(safeReturn);

            return RedirectToAction(fallbackAction, fallbackRouteValues);
        }
    }
}
