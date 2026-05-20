using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.Entites.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.MVCUI.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IProjectStudentService _projectStudentService;
        private readonly IProjectRequestService _projectRequestService;
        private readonly UserManager<AppUser> _userManager;

        public StudentController(
            IProjectService projectService,
            IProjectStudentService projectStudentService,
            IProjectRequestService projectRequestService,
            UserManager<AppUser> userManager)
        {
            _projectService = projectService;
            _projectStudentService = projectStudentService;
            _projectRequestService = projectRequestService;
            _userManager = userManager;
        }

        // Projelerim & Durum - Öğrencinin kendi projeleri
        [HttpGet]
        public async Task<IActionResult> MyProjects()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var projects = await _projectStudentService.GetProjectsByStudentIdAsync(user.Id);
                ViewBag.StudentName = user.FullName;
                return View(projects);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Projeler yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // Proje Havuzu - Tüm projeleri görüntüleme (Server-side DataTables)
        [HttpGet]
        public async Task<IActionResult> ProjectPool()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var pendingRequests = await _projectRequestService.GetRequestsByStudentIdAsync(user.Id);
                ViewBag.PendingRequestProjectIds = pendingRequests
                    .Where(r => r.RequestStatus == Data.Entites.ProjectRequestStatus.Pending)
                    .Select(r => r.ProjectId)
                    .ToList();
            }
            return View();
        }

        /// <summary>
        /// DataTables server-side processing AJAX endpoint (Student).
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
                    description = p.Description != null && p.Description.Length > 60
                        ? p.Description.Substring(0, 60) + "..."
                        : p.Description,
                    categoryName = p.CategoryName,
                    difficultyLevel = p.DifficultyLevel.ToString(),
                    completionPercentage = p.CompletionPercentage,
                    consultantFullName = string.IsNullOrWhiteSpace(p.ConsultantFullName) ? "Danışman Atanmadı" : p.ConsultantFullName,
                    endDate = p.EndDate.ToString("dd.MM.yyyy")
                })
            });
        }

        // Proje detayı görüntüleme
        [HttpGet]
        public async Task<IActionResult> ProjectDetails(Guid id, string? returnUrl = null)
        {
            try
            {
                var project = await _projectService.GetProjectDetailAsync(id);
                if (project == null)
                {
                    TempData["ErrorMessage"] = "Proje bulunamadı.";
                    return RedirectToStudentReturn(returnUrl, nameof(ProjectPool));
                }

                ViewBag.ReturnUrl = GetSafeReturnUrl(returnUrl) ?? Url.Action(nameof(ProjectPool))!;
                return View(project);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Proje yüklenirken hata oluştu: {ex.Message}";
                return RedirectToStudentReturn(returnUrl, nameof(ProjectPool));
            }
        }

        // Proje için istek gönderme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendProjectRequest(Guid projectId, string? message)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = "Oturumunuz kapalı." });

                    return RedirectToAction("Login", "Account");
                }

                // Mevcut istek kontrolü
                var hasPending = await _projectRequestService.HasPendingRequestAsync(projectId, user.Id);
                if (hasPending)
                {
                    var errorMsg = "Bu proje için zaten bekleyen bir isteğiniz bulunmaktadır.";
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = errorMsg });

                    TempData["ErrorMessage"] = errorMsg;
                    return RedirectToAction("ProjectPool");
                }

                var result = await _projectRequestService.SendRequestAsync(projectId, user.Id, message);
                if (result)
                {
                    var successMsg = "Proje isteğiniz başarıyla gönderildi. Danışman onayı bekleniyor.";
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = true, message = successMsg });

                    TempData["SuccessMessage"] = successMsg;
                }
                else
                {
                    var errorMsg = "İstek gönderilemedi. Lütfen tekrar deneyin.";
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = errorMsg });

                    TempData["ErrorMessage"] = errorMsg;
                }

                return RedirectToAction("ProjectPool");
            }
            catch (Exception ex)
            {
                var errorMsg = $"İstek gönderilirken hata oluştu: {ex.Message}";
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = errorMsg });

                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction("ProjectPool");
            }
        }

        // Öğrencinin gönderdiği istekleri görüntüleme
        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var requests = await _projectRequestService.GetRequestsByStudentIdAsync(user.Id);
                return View(requests);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"İstekler yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        private string? GetSafeReturnUrl(string? returnUrl) =>
            !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : null;

        private IActionResult RedirectToStudentReturn(string? returnUrl, string fallbackAction, object? fallbackRouteValues = null)
        {
            var safeReturn = GetSafeReturnUrl(returnUrl);
            if (safeReturn != null)
                return LocalRedirect(safeReturn);

            return RedirectToAction(fallbackAction, fallbackRouteValues);
        }
    }
}
