using System;
using System.Collections.Generic;

namespace ProjeHavuzu.Data.DTOs.ProjectStudentDto
{
    public class AssignProjectsToStudentDto
    {
        public Guid StudentId { get; set; }
        public string? StudentFullName { get; set; }
        public string? StudentNumber { get; set; }
        public List<Guid> SelectedProjectIds { get; set; } = new List<Guid>();
        public List<ProjectSelectionDto> AvailableProjects { get; set; } = new List<ProjectSelectionDto>();
    }

    public class ProjectSelectionDto
    {
        public Guid ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public int EndTime { get; set; }
        public bool IsAssigned { get; set; }
    }
}
