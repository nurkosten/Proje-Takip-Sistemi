using ProjeHavuzu.Data.DTOs.ProjectDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Business.Services.Abstract
{
    public interface IProjectService
    {
        Task<List<ProjectListDto>> GetAllProjectsAsync();
        Task<ProjectListDto> GetProjectByIdAsync(Guid id);
        Task<ProjectCreateDto> GetProjectCreateDtoAsync();
        Task<bool> CreateProjectAsync(ProjectCreateDto projectCreateDto);
        Task<bool> UpdateProjectAsync(Guid id, ProjectCreateDto projectCreateDto);
        Task<bool> DeleteProjectAsync(Guid id);
        Task<bool> SoftDeleteProjectAsync(Guid id);
    }
}
