using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Enums;
using System;
using System.Collections.Generic;

namespace ProjeHavuzu.Data.DTOs.ProjectDto
{
    public class ProjectDetailDto
    {
        public Guid Id { get; set; }
        public string ProjectTitle { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DifficultStatus DifficultyLevel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ProjectLink { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string CreatedByFullName { get; set; } = null!;
        public string? ConsultantFullName { get; set; }
        public string? ProjectArea { get; set; }
        public string? InitialCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // Aşamalar
        public List<ProjectPhaseDto> Phases { get; set; } = new();

        // Diller
        public List<string> Languages { get; set; } = new();

        // İlerleme Yüzdesi (Hesaplanmış)
        public int CompletionPercentage => Phases.Count == 0 ? 0 : (int)Math.Round((double)Phases.Count(p => p.IsCompleted) / Phases.Count * 100);

        // Tamamlanan Aşama Sayısı
        public int CompletedPhasesCount => Phases.Count(p => p.IsCompleted);

        // Toplam Aşama Sayısı
        public int TotalPhasesCount => Phases.Count;
    }
}
