using ProjeHavuzu.Data.DTOs.Common;
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
        Task<ProjectDetailDto> GetProjectDetailAsync(Guid id);
        Task<ProjectCreateDto> GetProjectCreateDtoAsync();
        Task<bool> CreateProjectAsync(ProjectCreateDto projectCreateDto);
        Task<bool> UpdateProjectAsync(Guid id, ProjectCreateDto projectCreateDto);
        Task<bool> DeleteProjectAsync(Guid id);
        Task<bool> SoftDeleteProjectAsync(Guid id);

        // Recycle Bin Methods
        Task<List<ProjectListDto>> GetDeletedProjectsAsync();
        Task<bool> RestoreProjectAsync(Guid id);
        Task<bool> HardDeleteProjectAsync(Guid id);

        // Phase Management
        Task<bool> TogglePhaseCompletionAsync(Guid phaseId);
        Task<List<ProjectPhaseDto>> GetProjectPhasesAsync(Guid projectId);

        // Server-side DataTables
        Task<DataTableResponse<ProjectListDto>> GetProjectsServerSideAsync(DataTableRequest request);

        Task<List<ProjectListDto>> GetProjectsByConsultantIdAsync(Guid consultantId);
        Task ApproveProjectAsync(Guid projectId, Guid advisorId, bool isAdmin = false);
        Task RejectProjectAsync(Guid projectId, Guid advisorId, string rejectionReason, bool isAdmin = false);
    }
}
