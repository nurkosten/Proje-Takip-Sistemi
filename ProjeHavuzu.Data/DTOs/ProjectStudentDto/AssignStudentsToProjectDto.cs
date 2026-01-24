using System;
using System.Collections.Generic;

namespace ProjeHavuzu.Data.DTOs.ProjectStudentDto
{
    public class AssignStudentsToProjectDto
    {
        public Guid ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public List<Guid> SelectedStudentIds { get; set; } = new List<Guid>();
        public List<StudentSelectionDto> AvailableStudents { get; set; } = new List<StudentSelectionDto>();
    }

    public class StudentSelectionDto
    {
        public Guid StudentId { get; set; }
        public string? FullName { get; set; }
        public string? StudentNumber { get; set; }
        public string? Email { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsAssigned { get; set; }
    }
}
