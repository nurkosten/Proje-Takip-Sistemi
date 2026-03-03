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

        // GET: Category/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _categoryRepository.GetAsync(c => c.Id == id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var dto = new CategoryAddDto
            {
                CategoryName = category.CategoryName
            };
            ViewBag.CategoryId = id;
            return View(dto);
        }

        // POST: Category/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, CategoryAddDto categoryAddDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = id;
                return View(categoryAddDto);
            }

            var category = await _categoryRepository.GetAsync(c => c.Id == id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            category.CategoryName = categoryAddDto.CategoryName;
            _categoryRepository.Update(category);

            TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _categoryRepository.GetAsync(c => c.Id == id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // POST: Category/DeleteConfirmed/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var category = await _categoryRepository.GetAsync(c => c.Id == id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _categoryRepository.Remove(category);
                TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Kategori silinirken hata oluştu: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
