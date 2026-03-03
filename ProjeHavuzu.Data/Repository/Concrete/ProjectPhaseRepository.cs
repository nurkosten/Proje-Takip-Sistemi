using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;

namespace ProjeHavuzu.Data.Repository.Concrete
{
    public class ProjectPhaseRepository : RepositoryBase<ProjectPhase>, IProjectPhaseRepository
    {
        private readonly ApplicationContext _context;

        public ProjectPhaseRepository(ApplicationContext context, DbSet<ProjectPhase>? dbSet = null) : base(context, dbSet)
        {
            _context = context;
        }

        public async Task<List<ProjectPhase>> GetPhasesByProjectIdAsync(Guid projectId)
        {
            return await _context.Set<ProjectPhase>()
                .Where(p => p.ProjectId == projectId && !p.IsDeleted)
                .OrderBy(p => p.Order)
                .ToListAsync();
        }

        public async Task<ProjectPhase?> GetPhaseByIdAsync(Guid phaseId)
        {
            return await _context.Set<ProjectPhase>()
                .FirstOrDefaultAsync(p => p.Id == phaseId && !p.IsDeleted);
        }
    }
}
