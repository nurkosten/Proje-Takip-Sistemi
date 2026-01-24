using AutoMapper;
using FluentValidation;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.CategoryDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjeHavuzu.Business.Services.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CategoryAddDto> _addValidator;
        private readonly IValidator<CategoryEditDto> _editValidator;

        public CategoryService(
            ICategoryRepository categoryRepository, 
            IMapper mapper,
            IValidator<CategoryAddDto> addValidator,
            IValidator<CategoryEditDto> editValidator)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _addValidator = addValidator;
            _editValidator = editValidator;
        }

        public async Task<List<CategoryListDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.ListAsync(c => !c.IsDeleted);
            return _mapper.Map<List<CategoryListDto>>(categories);
        }

        public async Task<CategoryListDto> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null)
                return null;

            return _mapper.Map<CategoryListDto>(category);
        }

        public async Task<bool> CreateCategoryAsync(CategoryAddDto categoryAddDto)
        {
            // Validation
            var validationResult = await _addValidator.ValidateAsync(categoryAddDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            // Aynı isimde kategori var mı kontrol et
            var existingCategory = await _categoryRepository.GetAsync(
                c => c.CategoryName.Trim().ToLower() == categoryAddDto.CategoryName.Trim().ToLower() && !c.IsDeleted);
            
            if (existingCategory != null)
            {
                throw new InvalidOperationException("Bu isimde bir kategori zaten mevcut.");
            }

            var category = _mapper.Map<Category>(categoryAddDto);
            await _categoryRepository.AddAsync(category);
            return true;
        }

        public async Task<bool> UpdateCategoryAsync(CategoryEditDto categoryEditDto)
        {
            // Validation
            var validationResult = await _editValidator.ValidateAsync(categoryEditDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            var existingCategory = await _categoryRepository.GetAsync(c => c.Id == categoryEditDto.Id && !c.IsDeleted);
            if (existingCategory == null)
            {
                throw new ArgumentException("Kategori bulunamadı.");
            }

            // Aynı isimde başka bir kategori var mı kontrol et
            var duplicateCategory = await _categoryRepository.GetAsync(
                c => c.Id != categoryEditDto.Id && 
                     c.CategoryName.Trim().ToLower() == categoryEditDto.CategoryName.Trim().ToLower() && 
                     !c.IsDeleted);
            
            if (duplicateCategory != null)
            {
                throw new InvalidOperationException("Bu isimde bir kategori zaten mevcut.");
            }

            existingCategory.CategoryName = categoryEditDto.CategoryName;
            _categoryRepository.Update(existingCategory);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetAsync(c => c.Id == id);
            if (category == null)
            {
                throw new ArgumentException("Kategori bulunamadı.");
            }

            _categoryRepository.Remove(category);
            return true;
        }

        public async Task<bool> SoftDeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null)
            {
                throw new ArgumentException("Kategori bulunamadı.");
            }

            category.IsDeleted = true;
            category.IsActive = false;
            _categoryRepository.Update(category);
            return true;
        }
    }
}
