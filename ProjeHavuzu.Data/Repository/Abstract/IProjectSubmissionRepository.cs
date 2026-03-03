
using ProjeHavuzu.Data.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjeHavuzu.Data.Repository.Abstract
{
    public interface IProjectSubmissionRepository : IRepository<ProjectSubmission>
    {
        // Custom method for complex retrieval if needed (e.g. Include user)
        Task<List<ProjectSubmission>> GetSubmissionsWithStudentAsync();
    }
}
