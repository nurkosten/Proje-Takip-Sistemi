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
        public DifficultStatus DifficultyLevel { get; set; }
        public int EndTime { get; set; } // Tamamlanma Süresi (gün olarak)
        public string ProjetLink { get; set; } //
        public Guid? AppUserId { get; set; }
    }
}
