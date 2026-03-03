using System;
using System.Collections.Generic;

namespace ProjeHavuzu.Data.DTOs.ProjectStudentDto
{
    public class ProjectStudentAssignDto
    {
        public Guid ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public string? Description { get; set; }
        public int CompletionPercentage { get; set; }
        public Guid StudentId { get; set; }
        public string? StudentFullName { get; set; }
        public string? StudentNumber { get; set; }
        public DateTime ProjectCreatedDate { get; set; }
    }
}
