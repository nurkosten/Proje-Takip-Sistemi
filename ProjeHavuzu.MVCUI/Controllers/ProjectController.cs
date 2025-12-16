using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;

namespace ProjeHavuzu.MVCUI.Controllers
{
    public class ProjectController : Controller
    {

        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public ProjectController(IProjectRepository projectRepository, IMapper mapper = null)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()=>View();
        
        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateDto projectCreateDto) { 
            

            var project= _mapper.Map<Project>(projectCreateDto);
            await _projectRepository.AddAsync(project);


            return View("Index"); 
        }


    }
}
