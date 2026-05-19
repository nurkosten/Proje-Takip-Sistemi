using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Entites.Identity
{
    public class AppUser:IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }    
        public string FullName => $"{FirstName} {LastName}";
        public Guid? FacultyId { get; set; } 
        public Guid? DepartmentId { get; set; }
        public string? StudentNumber { get; set; }
        public string? StaffNumber { get; set; }
        public string? AcademicTitle { get; set; }
        public bool IsActive { get; set; } = true;
        public string? DeactivationReason { get; set; }

        //relations property
        public Faculty Faculty { get; set; }
        public Department Department { get; set; }
        public ICollection<ProjectStudent> ProjectStudents { get; set; } = new List<ProjectStudent>();
    }
}
