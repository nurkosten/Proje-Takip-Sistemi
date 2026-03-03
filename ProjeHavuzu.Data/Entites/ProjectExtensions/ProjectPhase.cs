using ProjeHavuzu.Data.Entites.Common;
using System;

namespace ProjeHavuzu.Data.Entites
{
    public class ProjectPhase : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public string PhaseName { get; set; } = string.Empty; // Örn: 1. Aşama: Hazırlık
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedDate { get; set; }
        
        public virtual Project Project { get; set; } = null!;
    }
}
