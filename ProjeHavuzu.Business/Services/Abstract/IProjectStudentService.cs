using ProjeHavuzu.Data.DTOs.ProjectStudentDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Abstract
{
    public interface IProjectStudentService
    {
        // Assign students to project
        Task<AssignStudentsToProjectDto> GetAssignStudentsToProjectDtoAsync(Guid projectId);
        Task<bool> AssignStudentsToProjectAsync(Guid projectId, List<Guid> studentIds);
        Task<bool> RemoveStudentFromProjectAsync(Guid projectId, Guid studentId);

        // Assign projects to student
        Task<AssignProjectsToStudentDto> GetAssignProjectsToStudentDtoAsync(Guid studentId);
        Task<bool> AssignProjectsToStudentAsync(Guid studentId, List<Guid> projectIds);
        Task<bool> RemoveProjectFromStudentAsync(Guid studentId, Guid projectId);

        // Query methods
        Task<List<ProjectStudentAssignDto>> GetStudentsByProjectIdAsync(Guid projectId);
        Task<List<ProjectStudentAssignDto>> GetProjectsByStudentIdAsync(Guid studentId);
    }
}
