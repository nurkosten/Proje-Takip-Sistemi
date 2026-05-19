using ProjeHavuzu.Data.Entites;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Abstract
{
    public interface IProjectRequestService
    {
        Task<bool> SendRequestAsync(Guid projectId, Guid studentId, string? message);
        Task<bool> ApproveRequestAsync(Guid requestId, string? responseMessage);
        Task<bool> RejectRequestAsync(Guid requestId, string? responseMessage);
        Task<List<ProjectRequest>> GetRequestsByStudentIdAsync(Guid studentId);
        Task<List<ProjectRequest>> GetPendingRequestsForConsultantAsync(Guid consultantId);
        Task<List<ProjectRequest>> GetAllPendingRequestsAsync();
        Task<bool> HasPendingRequestAsync(Guid projectId, Guid studentId);
        Task<ProjectRequest?> GetRequestByIdAsync(Guid requestId);
    }
}
