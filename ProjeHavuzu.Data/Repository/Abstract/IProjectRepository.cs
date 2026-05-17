using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Repository.Abstract
{
  public interface IProjectRepository : IRepository<Project>
  {
    Task<List<ProjectListDto>> GetAllProjectsByCategoryAsync();
    Task<List<ProjectListDto>> GetDeletedProjectsAsync();
    Task<ProjectDetailDto> GetProjectDetailWithPhasesAsync(Guid projectId);

    /// <summary>
    /// Server-side DataTables processing: sayfalama, sıralama, arama
    /// </summary>
    Task<DataTableResponse<ProjectListDto>> GetProjectsServerSideAsync(DataTableRequest request);
    Task<List<ProjectListDto>> GetProjectsByConsultantIdAsync(Guid consultantId);
    Task<List<ProjectListDto>> GetProjectsForStudentAsync(Guid studentId);
  }
}
