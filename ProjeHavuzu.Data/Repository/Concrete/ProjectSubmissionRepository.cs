
using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.Data.Repository.Concrete
{
    public class ProjectSubmissionRepository : RepositoryBase<ProjectSubmission>, IProjectSubmissionRepository
    {
        public ProjectSubmissionRepository(Context.ApplicationContext context) : base(context)
        {
        }

        public async Task<List<ProjectSubmission>> GetSubmissionsWithStudentAsync()
        {
            // İhtiyaca göre kullanıcı tablosuna Include eklenebilir
            // Şimdilik sadece Submission'ları döndürecek, ileride öğrenci bilgisi eklenirse Include(x => x.Student)
            return await Task.FromResult(_context.ProjectSubmissions.ToList());
        }

        // Server-side implementasyon için mevcut Repository base metodlarını kullanabiliriz veya özelleştiririz.
    }
}
