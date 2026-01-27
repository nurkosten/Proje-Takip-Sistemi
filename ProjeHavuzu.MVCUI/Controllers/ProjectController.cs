using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using System;
using System.Threading.Tasks;

namespace ProjeHavuzu.MVCUI.Controllers
{
    //[Authorize(Roles ="Admin")]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public ProjectController(IProjectService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return View(projects);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var dto = await _projectService.GetProjectCreateDtoAsync();
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateDto projectCreateDto)
        {
            if (!ModelState.IsValid)
            {
                var dto = await _projectService.GetProjectCreateDtoAsync();
                projectCreateDto.Categories = dto.Categories;
                return View(projectCreateDto);
            }

            try
            {
                if(User.Identity.IsAuthenticated)
                {
                    // Kullanıcı ID'sini claim'den alıp projeye ekleyebiliriz
                    // projectCreateDto.CreatedBy = ...
                }
                
                await _projectService.CreateProjectAsync(projectCreateDto);
                TempData["SuccessMessage"] = "Proje başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var dto = await _projectService.GetProjectCreateDtoAsync();
                projectCreateDto.Categories = dto.Categories;
                return View(projectCreateDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                TempData["ErrorMessage"] = "Proje bulunamadı.";
                return RedirectToAction("Index");
            }

            var dto = await _projectService.GetProjectCreateDtoAsync();
            
            // Mevcut verileri DTO'ya map et veya manuel set et
            // AutoMapper burada kullanışlı olurdu ama manuel duralım şimdilik
            // dto.ProjectId = project.Id; // DTO'da ProjectId alanı yok
            dto.ProjectTitle = project.ProjectTitle;
            dto.Description = project.Description;
            dto.CategoryId = project.CategoryId;
            dto.DifficultyLevel = project.DifficultyLevel;
            dto.StartDate = project.StartDate;
            dto.EndDate = project.EndDate;
            dto.ProjectLink = project.ProjectLink;
            // DTO'da ProjectId yoksa View'da ID'yi Hidden olarak tutmalıyız.

            return View(dto);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, ProjectCreateDto projectCreateDto)
        {
            if (!ModelState.IsValid)
            {
                 var dto = await _projectService.GetProjectCreateDtoAsync();
                 projectCreateDto.Categories = dto.Categories;
                 return View(projectCreateDto);
            }

            try
            {
                await _projectService.UpdateProjectAsync(id, projectCreateDto);
                TempData["SuccessMessage"] = "Proje güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var dto = await _projectService.GetProjectCreateDtoAsync();
                projectCreateDto.Categories = dto.Categories;
                return View(projectCreateDto);
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                TempData["ErrorMessage"] = "Proje bulunamadı.";
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // SOFT DELETE (Çöp Kutusuna Gönder)
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
        public async Task<IActionResult> RecycleBin()
        {
            var deletedProjects = await _projectService.GetDeletedProjectsAsync();
            return View(deletedProjects);
        }

        // Geri Yükle
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                await _projectService.RestoreProjectAsync(id);
                TempData["SuccessMessage"] = "Proje geri yüklendi.";
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("RecycleBin");
        }

        // Kalıcı Olarak Sil
        public async Task<IActionResult> HardDelete(Guid id)
        {
            try
            {
                await _projectService.HardDeleteProjectAsync(id);
                TempData["SuccessMessage"] = "Proje kalıcı olarak silindi.";
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("RecycleBin");
        }

    }
}
