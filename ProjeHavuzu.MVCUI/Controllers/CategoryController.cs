using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjeHavuzu.Data.DTOs.CategoryDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;

namespace ProjeHavuzu.MVCUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper = null)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.ListAsync();
            var result = _mapper.Map<List<CategoryListDto>>(categories);

            return View("Index", result);
        }


        public async Task<IActionResult> Create() => View("Create");

        [HttpPost]
        public async Task<IActionResult> Create(
    CategoryAddDto categoryAddDto,
    CancellationToken cancellationToken)
        {
            var category = _mapper.Map<Category>(categoryAddDto);

            await _categoryRepository.AddAsync(category);
            return RedirectToAction(nameof(Index));
        }






    }
}
