using ProjeHavuzu.Data.DTOs.CategoryDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Business.Services.Abstract
{
    public interface ICategoryService
    {
        Task<List<CategoryListDto>> GetAllCategoriesAsync();
        Task<CategoryListDto> GetCategoryByIdAsync(Guid id);
        Task<bool> CreateCategoryAsync(CategoryAddDto categoryAddDto);
        Task<bool> UpdateCategoryAsync(CategoryEditDto categoryEditDto);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task<bool> SoftDeleteCategoryAsync(Guid id);
    }
}
