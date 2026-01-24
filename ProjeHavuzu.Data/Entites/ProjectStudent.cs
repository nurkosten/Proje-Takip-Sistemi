using ProjeHavuzu.Data.Entites.Common;
using ProjeHavuzu.Data.Entites.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Entites
{
    public class ProjectStudent : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Guid StudentId { get; set; }

        // Navigation properties
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<AppUser> AspNetUsers { get; set; }
    }
}
