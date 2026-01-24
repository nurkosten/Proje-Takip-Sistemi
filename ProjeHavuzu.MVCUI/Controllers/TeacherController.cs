using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Business.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.MVCUI.Controllers
{
    //[Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IProjectStudentService _projectStudentService;
        private readonly UserManager<ProjeHavuzu.Data.Entites.Identity.AppUser> _userManager;

        public TeacherController(
            IProjectService projectService,
            IProjectStudentService projectStudentService,
            UserManager<ProjeHavuzu.Data.Entites.Identity.AppUser> userManager)
        {
            _projectService = projectService;
            _projectStudentService = projectStudentService;
            _userManager = userManager;
        }

        // Dashboard for teachers
        public IActionResult Index()
        {
            return View();
        }

        // ====================
        // All Projects View and Edit
        // ====================

        [HttpGet]
        public async Task<IActionResult> AllProjects()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();
                return View(projects);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Projeler yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditProject(Guid id)
        {
            try
            {
                var projectDto = await _projectService.GetProjectCreateDtoAsync();
                var project = await _projectService.GetProjectByIdAsync(id);

                if (project == null)
                {
                    TempData["ErrorMessage"] = "Proje bulunamadı.";
                    return RedirectToAction("AllProjects");
                }

                // Map to create DTO for editing
                var editDto = new ProjeHavuzu.Data.DTOs.ProjectDto.ProjectCreateDto
                {
                    ProjectTitle = project.ProjectTitle,
                    Description = project.Description,
                    CategoryId = project.CategoryId,
                    DifficultyLevel = project.DifficultyLevel,
                    EndTime = project.EndTime,
                    ProjectLink = project.ProjectLink,
                    Categories = projectDto.Categories
                };

                ViewBag.ProjectId = id;
                return View(editDto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Proje yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction("AllProjects");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditProject(Guid id, ProjeHavuzu.Data.DTOs.ProjectDto.ProjectCreateDto projectDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var dto = await _projectService.GetProjectCreateDtoAsync();
                    projectDto.Categories = dto.Categories;
                    ViewBag.ProjectId = id;
                    return View(projectDto);
                }

                var result = await _projectService.UpdateProjectAsync(id, projectDto);

                if (result)
                {
                    TempData["SuccessMessage"] = "Proje başarıyla güncellendi.";
                    return RedirectToAction("AllProjects");
                }
                else
                {
                    TempData["ErrorMessage"] = "Proje güncellenirken bir hata oluştu.";
                    var dto = await _projectService.GetProjectCreateDtoAsync();
                    projectDto.Categories = dto.Categories;
                    ViewBag.ProjectId = id;
                    return View(projectDto);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                var dto = await _projectService.GetProjectCreateDtoAsync();
                projectDto.Categories = dto.Categories;
                ViewBag.ProjectId = id;
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
        public async Task<IActionResult> AssignStudentsToProject(Guid projectId)
        {
            try
            {
                var dto = await _projectStudentService.GetAssignStudentsToProjectDtoAsync(projectId);
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction("AllProjects");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignStudentsToProject(Guid projectId, List<Guid> selectedStudentIds)
        {
            try
            {
                if (selectedStudentIds == null)
                    selectedStudentIds = new List<Guid>();

                await _projectStudentService.AssignStudentsToProjectAsync(projectId, selectedStudentIds);
                TempData["SuccessMessage"] = "Öğrenci atamaları başarıyla güncellendi.";
                return RedirectToAction("AllProjects");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction("AssignStudentsToProject", new { projectId });
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
        public async Task<IActionResult> ViewProjectStudents(Guid projectId)
        {
            try
            {
                var students = await _projectStudentService.GetStudentsByProjectIdAsync(projectId);
                var project = await _projectService.GetProjectByIdAsync(projectId);

                ViewBag.ProjectTitle = project?.ProjectTitle ?? "Unknown";
                ViewBag.ProjectId = projectId;

                return View(students);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction("AllProjects");
            }
        }
    }
}
