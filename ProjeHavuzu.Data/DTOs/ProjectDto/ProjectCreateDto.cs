using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DTOs.ProjectDto
{
    public class ProjectCreateDto
    {

        public string? ProjectTitle { get; set; }
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CustomCategory { get; set; } // Öğrencinin elle yazacağı kategori

        public DifficultStatus? DifficultyLevel { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ProjectLink { get; set; } 
        public string? InitialCode { get; set; }
        public string? ProjectArea { get; set; } // Domain
        
        public Guid? AppUserId { get; set; }
        public Guid? ConsultantId { get; set; } // Seçilen Danışman
        
        public List<Category>? Categories { get; set; }
        public List<ProjeHavuzu.Data.DTOs.AccountDto.UserListDto>? Academicians { get; set; }
        
        // Yeni Alanlar
        public List<string> PhaseNames { get; set; } = new List<string>();
        public List<string> PhaseDescriptions { get; set; } = new List<string>();
        
        // Mevcut Aşamalar (Edit için)
        public List<ProjectPhaseDto>? ExistingPhases { get; set; }
        
        // Seçilen Diller (Checkbox veya Tagify için)
        public string? SelectedLanguagesJson { get; set; } // "C#,Java" gibi virgülle ayrılmış string gelebilir
    }
}
