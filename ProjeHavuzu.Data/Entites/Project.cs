using ProjeHavuzu.Data.Entites.Common;
using ProjeHavuzu.Data.Entites.Enums;
using ProjeHavuzu.Data.Entites.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Entites
{
    public class Project : BaseEntity
    {
        public bool IsQuickDraft;

        public string ProjectTitle { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public DifficultStatus DifficultyLevel { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        public string? ProjectLink { get; set; } //
        public Guid? AppUserId { get; set; }
        public int? Percentile { get; set; }

        // Navigation property
        public virtual AppUser AppUser { get; set; }


    }
}
