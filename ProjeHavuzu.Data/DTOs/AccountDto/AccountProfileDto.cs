using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DTOs.AccountDto
{
    public class AccountProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FacultyName { get; set; }
        public string DepartmentName { get; set; }
        public string? StudentNumber { get; set; }
    }
}
