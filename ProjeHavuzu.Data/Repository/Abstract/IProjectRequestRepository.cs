using ProjeHavuzu.Data.Entites;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjeHavuzu.Data.Repository.Abstract
{
    public interface IProjectRequestRepository : IRepository<ProjectRequest>
    {
        Task<List<ProjectRequest>> GetRequestsByProjectIdAsync(Guid projectId);
        Task<List<ProjectRequest>> GetRequestsByStudentIdAsync(Guid studentId);
        Task<List<ProjectRequest>> GetRequestsByConsultantIdAsync(Guid consultantId);
        Task<ProjectRequest?> GetExistingRequestAsync(Guid projectId, Guid studentId);
        Task<List<ProjectRequest>> GetPendingRequestsForConsultantAsync(Guid consultantId);
        Task<List<ProjectRequest>> GetAllPendingRequestsAsync();
    }
}
