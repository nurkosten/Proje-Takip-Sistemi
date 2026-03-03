using ProjeHavuzu.Data.Entites;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjeHavuzu.Data.Repository.Abstract
{
    public interface IProjectStudentRepository : IRepository<ProjectStudent>
    {
        Task<List<ProjectStudent>> GetProjectStudentsByProjectIdAsync(Guid projectId);
        Task<List<ProjectStudent>> GetProjectStudentsByStudentIdAsync(Guid studentId);
        Task<ProjectStudent?> GetProjectStudentAsync(Guid projectId, Guid studentId);
        Task<bool> IsStudentAssignedToProjectAsync(Guid projectId, Guid studentId);
    }
}
