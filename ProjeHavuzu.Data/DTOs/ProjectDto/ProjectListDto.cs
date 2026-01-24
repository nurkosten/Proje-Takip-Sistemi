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
        public string ProjectTitle { get; set; }
        public string Description { get; set; }
        public DifficultStatus DifficultyLevel { get; set; }
        public int EndTime { get; set; } // Tamamlanma Süresi (gün olarak)
        public string? ProjectLink { get; set; } //
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedByFullName { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DataStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string? Descripiton { get; set; }
    }
}
