using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.Data.Repository.Concrete
{
    public class ProjectRequestRepository : RepositoryBase<ProjectRequest>, IProjectRequestRepository
    {
        private readonly ApplicationContext _context;

        public ProjectRequestRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ProjectRequest>> GetRequestsByProjectIdAsync(Guid projectId)
        {
            return await _context.ProjectRequests
                .Include(r => r.Student)
                .Include(r => r.Project)
                .Where(r => r.ProjectId == projectId && !r.IsDeleted)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<List<ProjectRequest>> GetRequestsByStudentIdAsync(Guid studentId)
        {
            return await _context.ProjectRequests
                .Include(r => r.Project)
                .Where(r => r.StudentId == studentId && !r.IsDeleted)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<List<ProjectRequest>> GetRequestsByConsultantIdAsync(Guid consultantId)
        {
            return await _context.ProjectRequests
                .Include(r => r.Student)
                .Include(r => r.Project)
                .Where(r => r.Project != null && r.Project.ConsultantId == consultantId && !r.IsDeleted)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<ProjectRequest?> GetExistingRequestAsync(Guid projectId, Guid studentId)
        {
            return await _context.ProjectRequests
                .FirstOrDefaultAsync(r => r.ProjectId == projectId && r.StudentId == studentId && !r.IsDeleted);
        }

        public async Task<List<ProjectRequest>> GetPendingRequestsForConsultantAsync(Guid consultantId)
        {
            return await _context.ProjectRequests
                .Include(r => r.Student)
                .Include(r => r.Project)
                .Where(r => r.Project != null && r.Project.ConsultantId == consultantId 
                            && r.RequestStatus == ProjectRequestStatus.Pending 
                            && !r.IsDeleted)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<List<ProjectRequest>> GetAllPendingRequestsAsync()
        {
            return await _context.ProjectRequests
                .Include(r => r.Student)
                .Include(r => r.Project)
                .Where(r => r.RequestStatus == ProjectRequestStatus.Pending && !r.IsDeleted)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }
    }
}
