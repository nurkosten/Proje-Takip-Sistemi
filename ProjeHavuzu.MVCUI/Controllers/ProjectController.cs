using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using System;
using System.Threading.Tasks;

namespace ProjeHavuzu.MVCUI.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Student")]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ProjeHavuzu.Data.Entites.Identity.AppUser> _userManager;
        private readonly FluentValidation.IValidator<ProjectCreateDto> _validator;

        public ProjectController(IProjectService projectService, IMapper mapper, Microsoft.AspNetCore.Identity.UserManager<ProjeHavuzu.Data.Entites.Identity.AppUser> userManager, FluentValidation.IValidator<ProjectCreateDto> validator)
        {
            _projectService = projectService;
            _mapper = mapper;
            _userManager = userManager;
            _validator = validator;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// DataTables server-side processing AJAX endpoint.
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
                Columns = new System.Collections.Generic.List<DataTableColumn>(),
                Order = new System.Collections.Generic.List<DataTableOrder>()
            };

            // Kolonları oku
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

            // Sıralama bilgisini oku
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
        public async Task<IActionResult> Create()
        {
            var dto = new ProjectCreateDto();
            await PrepareProjectViewDataAsync(dto);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateDto projectCreateDto)
        {
            var validationResult = await _validator.ValidateAsync(projectCreateDto);
            if (!validationResult.IsValid)
            {
                validationResult.Errors.ForEach(error => ModelState.AddModelError(error.PropertyName, error.ErrorMessage));
            }

            if (!ModelState.IsValid)
            {
                await PrepareProjectViewDataAsync(projectCreateDto);
                return View(projectCreateDto);
            }

            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
                    {
                        projectCreateDto.AppUserId = userId;
                    }
                }

                await _projectService.CreateProjectAsync(projectCreateDto);
                TempData["SuccessMessage"] = "Proje başarıyla oluşturuldu ve danışman onayına gönderildi.";

                if (User.IsInRole("Student"))
                {
                    return RedirectToAction("MyProjects", "Student");
                }

                if (User.IsInRole("Teacher"))
                {
                    return RedirectToAction("MyProjects", "Teacher");
                }

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("MyProjects", "Admin");
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message + (ex.InnerException != null ? " İç Hata: " + ex.InnerException.Message : "");
                ModelState.AddModelError("", errorMessage);

                await PrepareProjectViewDataAsync(projectCreateDto);
                return View(projectCreateDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, string? returnUrl = null)
        {
            var project = await _projectService.GetProjectDetailAsync(id);
            if (project == null)
            {
                TempData["ErrorMessage"] = "Proje bulunamadı.";
                return RedirectAfterEdit(id, returnUrl);
            }

            var dto = new ProjectCreateDto();
            await PrepareProjectViewDataAsync(dto);
            ViewBag.ReturnUrl = GetDefaultEditReturnUrl(id, returnUrl);
            ViewBag.ProjectId = id;

            // Mevcut verileri DTO'ya map et
            dto.ProjectTitle = project.ProjectTitle;
            dto.Description = project.Description;
            dto.CategoryId = project.CategoryId;
            dto.DifficultyLevel = project.DifficultyLevel;
            dto.StartDate = project.StartDate;
            dto.EndDate = project.EndDate;
            dto.ProjectLink = project.ProjectLink;
            dto.ProjectArea = project.ProjectArea;
            dto.InitialCode = project.InitialCode;

            // Mevcut aşamaları yükle
            dto.ExistingPhases = project.Phases;

            // Mevcut dilleri yükle
            if (project.Languages != null && project.Languages.Any())
            {
                dto.SelectedLanguagesJson = string.Join(",", project.Languages);
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProjectCreateDto projectCreateDto, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = GetDefaultEditReturnUrl(id, returnUrl);
                ViewBag.ProjectId = id;
                await PrepareProjectViewDataAsync(projectCreateDto);
                return View(projectCreateDto);
            }

            try
            {
                await _projectService.UpdateProjectAsync(id, projectCreateDto);
                TempData["SuccessMessage"] = "Proje güncellendi.";
                return RedirectAfterEdit(id, returnUrl);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.ReturnUrl = GetDefaultEditReturnUrl(id, returnUrl);
                ViewBag.ProjectId = id;
                await PrepareProjectViewDataAsync(projectCreateDto);
                return View(projectCreateDto);
            }
        }

        private string? GetSafeReturnUrl(string? returnUrl) =>
            !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : null;

        private string GetDefaultEditReturnUrl(Guid id, string? returnUrl)
        {
            return GetSafeReturnUrl(returnUrl)
                ?? (User.IsInRole("Student") ? Url.Action("MyProjects", "Student")!
                    : User.IsInRole("Teacher") ? Url.Action("ProjectDetails", "Teacher", new { id })!
                    : User.IsInRole("Admin") ? Url.Action("MyProjects", "Admin")!
                    : Url.Action("Index")!);
        }

        private IActionResult RedirectAfterEdit(Guid id, string? returnUrl)
        {
            var safeReturn = GetSafeReturnUrl(returnUrl);
            if (safeReturn != null)
                return LocalRedirect(safeReturn);

            if (User.IsInRole("Student"))
                return RedirectToAction("MyProjects", "Student");
            if (User.IsInRole("Teacher"))
                return RedirectToAction("ProjectDetails", "Teacher", new { id });
            if (User.IsInRole("Admin"))
                return RedirectToAction("MyProjects", "Admin");
            return RedirectToAction("Index");
        }

        private async Task PrepareProjectViewDataAsync(ProjectCreateDto dto)
        {
            var createDto = await _projectService.GetProjectCreateDtoAsync();
            dto.Categories = createDto.Categories;
            dto.Academicians = await GetAdvisorCandidatesAsync();
        }

        private async Task<List<ProjeHavuzu.Data.DTOs.AccountDto.UserListDto>> GetAdvisorCandidatesAsync()
        {
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            var advisors = new List<ProjeHavuzu.Data.DTOs.AccountDto.UserListDto>();

            foreach (var user in teachers)
            {
                if (await _userManager.IsInRoleAsync(user, "Student"))
                    continue;
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    continue;

                advisors.Add(new ProjeHavuzu.Data.DTOs.AccountDto.UserListDto
                {
                    Id = user.Id,
                    FullName = user.FullName ?? $"{user.FirstName} {user.LastName}",
                    Email = user.Email ?? string.Empty
                });
            }

            return advisors.OrderBy(a => a.FullName).ToList();
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var project = await _projectService.GetProjectDetailAsync(id);
            if (project == null)
            {
                TempData["ErrorMessage"] = "Proje bulunamadı.";
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // Aşama Tamamlama Toggle (AJAX)
        [HttpPost]
        public async Task<IActionResult> TogglePhaseCompletion(Guid phaseId)
        {
            try
            {
                await _projectService.TogglePhaseCompletionAsync(phaseId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // SOFT DELETE (Çöp Kutusuna Gönder)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id);
                TempData["SuccessMessage"] = "Proje çöp kutusuna taşındı.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        // --- RECYCLE BIN ---

        // Çöp Kutusu Listesi
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RecycleBin()
        {
            var deletedProjects = await _projectService.GetDeletedProjectsAsync();
            return View(deletedProjects);
        }

        // Geri Yükle
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                await _projectService.RestoreProjectAsync(id);
                TempData["SuccessMessage"] = "Proje geri yüklendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("RecycleBin");
        }

        // Kalıcı Olarak Sil
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            try
            {
                await _projectService.HardDeleteProjectAsync(id);
                TempData["SuccessMessage"] = "Proje kalıcı olarak silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("RecycleBin");
        }

    }
}
