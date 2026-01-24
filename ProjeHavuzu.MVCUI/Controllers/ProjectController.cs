using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;

namespace ProjeHavuzu.MVCUI.Controllers
{
    //[Authorize(Roles ="Admin")]
    public class ProjectController : Controller
    {

        private readonly IProjectRepository _projectRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProjectController(IProjectRepository projectRepository, IMapper mapper = null, ICategoryRepository categoryRepository = null)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _projectRepository.GetAllProjectsByCategoryAsync();
            var result = _mapper.Map<List<ProjectListDto>>(projects);
            return View(result);
        }

        public async Task<IActionResult> Create()
        {
            ProjectCreateDto projectCreateDto = new ProjectCreateDto();
            projectCreateDto.Categories = await _categoryRepository.ListAsync();
            return View(projectCreateDto);

        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateDto projectCreateDto)
        {

            ProjectCreateDto projectDto = new ProjectCreateDto();

            projectCreateDto.Categories = await _categoryRepository.ListAsync();

            var project = _mapper.Map<Project>(projectCreateDto);
            await _projectRepository.AddAsync(project);


            return View("Index", _mapper.Map<List<ProjectListDto>>(await _projectRepository.GetAllProjectsByCategoryAsync()));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var projects = await _projectRepository.GetAllProjectsByCategoryAsync();
            var project = projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                TempData["ErrorMessage"] = "Proje bulunamadı.";
                return RedirectToAction("Index");
            }

            var result = _mapper.Map<ProjectListDto>(project);
            return View(result);
        }

    }
}
