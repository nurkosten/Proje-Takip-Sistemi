using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DTOs.ProjectDto
{
    public class ProjectCreateDto
    {

        public string ProjectTitle { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public DifficultStatus? DifficultyLevel { get; set; }
        public DateTime StartDate { get; set; } = new DateTime(2026, 1, 1);
        public DateTime EndDate { get; set; } = new DateTime(2026, 1, 1);
        public string? ProjectLink { get; set; } //
        public Guid? AppUserId { get; set; }
        public List<Category> Categories { get; set; }
    }
}
