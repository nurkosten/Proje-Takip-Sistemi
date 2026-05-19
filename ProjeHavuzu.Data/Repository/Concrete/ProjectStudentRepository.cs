using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;

namespace ProjeHavuzu.Data.Repository.Concrete
{
    public class ProjectStudentRepository : RepositoryBase<ProjectStudent>, IProjectStudentRepository
    {
        private readonly ApplicationContext _context;

        public ProjectStudentRepository(ApplicationContext context, DbSet<ProjectStudent> dbSet = null, ICurrentUserService currentUser = null) : base(context, dbSet)
        {
            _context = context;
        }

        public async Task<List<ProjectStudent>> GetProjectStudentsByProjectIdAsync(Guid projectId)
        {
            return await _context.ProjectStudents
                .AsNoTracking()
                .Where(ps => ps.ProjectId == projectId && !ps.IsDeleted)
                .Include(ps => ps.Student)
                .OrderByDescending(ps => ps.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<ProjectStudent>> GetProjectStudentsByStudentIdAsync(Guid studentId)
        {
            return await _context.ProjectStudents
                .AsNoTracking()
                .Where(ps => ps.StudentId == studentId && !ps.IsDeleted)
                .Include(ps => ps.Project)
                .OrderByDescending(ps => ps.CreatedDate)
                .ToListAsync();
        }

        public async Task<ProjectStudent?> GetProjectStudentAsync(Guid projectId, Guid studentId)
        {
            return await _context.ProjectStudents
                .FirstOrDefaultAsync(ps => ps.ProjectId == projectId && ps.StudentId == studentId && !ps.IsDeleted);
        }

        public async Task<bool> IsStudentAssignedToProjectAsync(Guid projectId, Guid studentId)
        {
            return await _context.ProjectStudents
                .AnyAsync(ps => ps.ProjectId == projectId && ps.StudentId == studentId && !ps.IsDeleted);
        }
    }
}
