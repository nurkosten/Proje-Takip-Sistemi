using ProjeHavuzu.Data.DTOs.FacultyDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Business.Services.Abstract
{
    public interface IFacultyService
    {
        Task<List<FacultyListDto>> GetAllFacultiesAsync();
        Task<FacultyListDto> GetFacultyByIdAsync(Guid id);
        Task<bool> CreateFacultyAsync(FacultyCreateDto facultyCreateDto);
        Task<bool> UpdateFacultyAsync(Guid id, FacultyCreateDto facultyCreateDto);
        Task<bool> DeleteFacultyAsync(Guid id);
        Task<bool> SoftDeleteFacultyAsync(Guid id);
    }
}
