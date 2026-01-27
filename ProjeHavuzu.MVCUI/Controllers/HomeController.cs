using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using ProjeHavuzu.Data.Repository.Concrete;
using ProjeHavuzu.MVCUI.Models;
using System.Diagnostics;
using System.Security.AccessControl;

namespace ProjeHavuzu.MVCUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public HomeController(IProjectRepository projectRepository, ICategoryRepository categoryRepository = null, IMapper mapper = null)
        {
            _projectRepository = projectRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }




        public async Task<IActionResult> Index()
        {  
            var projects = await _projectRepository.GetAllProjectsByCategoryAsync();
            var result = _mapper.Map<List<ProjectListDto>>(projects);

            return View("Index",result);
        }
        public async Task<IActionResult> Create()
        {
            ProjectCreateDto projectCreateDto = new ProjectCreateDto();
            projectCreateDto.Categories = await _categoryRepository.ListAsync();
            return View("Create", projectCreateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateDto projectCreateDto)
        {

            ProjectCreateDto projectDto = new ProjectCreateDto();

            projectCreateDto.Categories = await _categoryRepository.ListAsync();

            var project = _mapper.Map<Project>(projectCreateDto);
            await _projectRepository.AddAsync(project);


            return View("Index",_mapper.Map<List<ProjectListDto>>(await _projectRepository.GetAllProjectsByCategoryAsync()));
        }


        public IActionResult ProjectDetails(Guid id)
        {
            if (User.IsInRole("Teacher") || User.IsInRole("Admin"))
            {
                return RedirectToAction("ProjectDetails", "Teacher", new { id = id });
            }
            
            // Öğrenci ise veya rolü yoksa Student detayına gönder (Student controller Authorize ile login'e zorlayabilir)
            return RedirectToAction("ProjectDetails", "Student", new { id = id });
        }
    }
}
