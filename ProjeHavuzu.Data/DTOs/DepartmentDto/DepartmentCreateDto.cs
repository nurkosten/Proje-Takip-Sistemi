using ProjeHavuzu.Data.DTOs.FacultyDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DTOs.DepartmentDto
{
    public class DepartmentCreateDto
    {
        public Guid? Id { get; set; }
        public string DepartmentName { get; set; }
        public Guid FacultyId { get; set; }
        public string? FacultyName { get; set; }

        // Dropdown için fakülte listesi
        public List<FacultyListDto>? Faculties { get; set; }
    }
}
