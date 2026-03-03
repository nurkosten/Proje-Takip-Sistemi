using System;

namespace ProjeHavuzu.Data.DTOs.ProjectDto
{
    public class ProjectPhaseDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}
