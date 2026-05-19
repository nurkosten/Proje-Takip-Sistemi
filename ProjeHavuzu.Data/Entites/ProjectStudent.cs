using ProjeHavuzu.Data.Entites.Common;
using ProjeHavuzu.Data.Entites.Identity;

namespace ProjeHavuzu.Data.Entites
{
    public class ProjectStudent : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public Guid StudentId { get; set; }
        public AppUser Student { get; set; } = null!;
    }
}
