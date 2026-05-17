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
        public Guid? ConsultantId { get; set; } // Danışman ID
        public ProjectApprovalStatus ApprovalStatus { get; set; } = ProjectApprovalStatus.Pending;
        public string? RejectionReason { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public int? Percentile { get; set; }

        // Navigation property
        public string? InitialCode { get; set; } // Başlangıç kodları / snippetler
        public string? ProjectArea { get; set; } // Alan/Domain (Örn: Finans, Sağlık)

        // Navigation property
        public virtual AppUser AppUser { get; set; }
        public virtual AppUser Consultant { get; set; } // Danışman Navigation
        public virtual ICollection<ProjectPhase> Phases { get; set; }
        public virtual ICollection<ProjectLanguage> Languages { get; set; }


    }
}
