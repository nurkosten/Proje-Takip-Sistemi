using ProjeHavuzu.Data.Entites.Common;
using System;

namespace ProjeHavuzu.Data.Entites
{
    public class ProjectLanguage : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public string LanguageName { get; set; } // Örn: C#, Python
        
        public virtual Project Project { get; set; }
    }
}
