using ProjeHavuzu.Data.DTOs.DepartmentDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Business.Services.Abstract
{
    public interface IDepartmentService
    {
        Task<List<DepartmentListDto>> GetAllDepartmentsAsync();
        Task<List<DepartmentFacultyDto>> GetDepartmentsByFacultyAsync();
        Task<DepartmentListDto> GetDepartmentByIdAsync(Guid id);
        Task<bool> CreateDepartmentAsync(DepartmentListDto departmentDto);
        Task<bool> UpdateDepartmentAsync(Guid id, DepartmentListDto departmentDto);
        Task<bool> DeleteDepartmentAsync(Guid id);
        Task<bool> SoftDeleteDepartmentAsync(Guid id);
    }
}
