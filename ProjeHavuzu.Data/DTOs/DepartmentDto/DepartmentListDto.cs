using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DTOs.DepartmentDto
{
    public class DepartmentListDto
    {
        public Guid Id { get; set; }
        public string FacultyName { get; set; }
        public string DepartmentName { get; set; }

    }
}
