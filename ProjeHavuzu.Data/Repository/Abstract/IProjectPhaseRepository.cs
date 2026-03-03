using ProjeHavuzu.Data.Entites;

namespace ProjeHavuzu.Data.Repository.Abstract
{
    public interface IProjectPhaseRepository : IRepository<ProjectPhase>
    {
        Task<List<ProjectPhase>> GetPhasesByProjectIdAsync(Guid projectId);
        Task<ProjectPhase?> GetPhaseByIdAsync(Guid phaseId);
    }
}
