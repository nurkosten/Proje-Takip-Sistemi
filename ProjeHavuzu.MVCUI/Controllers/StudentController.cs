using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.Entites.Identity;
using System;
using System.Threading.Tasks;

namespace ProjeHavuzu.MVCUI.Controllers
{
    [Authorize(Roles = "Student,Teacher,Admin")]
    public class StudentController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IProjectStudentService _projectStudentService;
        private readonly UserManager<AppUser> _userManager;

        public StudentController(
            IProjectService projectService,
            IProjectStudentService projectStudentService,
            UserManager<AppUser> userManager)
        {
            _projectService = projectService;
            _projectStudentService = projectStudentService;
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

        // Proje Havuzu - Tüm projeleri görüntüleme
        [HttpGet]
        public async Task<IActionResult> ProjectPool()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();
                return View(projects);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Projeler yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // Proje detayı görüntüleme
        [HttpGet]
        public async Task<IActionResult> ProjectDetails(Guid id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);
                if (project == null)
                {
                    TempData["ErrorMessage"] = "Proje bulunamadı.";
                    return RedirectToAction("ProjectPool");
                }
                return View(project);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Proje yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction("ProjectPool");
            }
        }
    }
}
