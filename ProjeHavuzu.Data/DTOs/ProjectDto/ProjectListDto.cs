using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DTOs.ProjectDto
{
    public class ProjectListDto
    {
        public Guid Id { get; set; }
        public string ProjectTitle { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DifficultStatus DifficultyLevel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ProjectLink { get; set; } //
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate => IsDeleted ? UpdatedDate : null;
        public string CreatedByFullName { get; set; } = null!;
        public Guid? UpdatedBy { get; set; }
        public DataStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string? ConsultantFullName { get; set; }
        public Guid? ConsultantId { get; set; }

        // İlerleme Takibi
        public int TotalPhasesCount { get; set; }
        public int CompletedPhasesCount { get; set; }
        public int CompletionPercentage => TotalPhasesCount == 0 ? 0 : (int)Math.Round((double)CompletedPhasesCount / TotalPhasesCount * 100);
    }
}
