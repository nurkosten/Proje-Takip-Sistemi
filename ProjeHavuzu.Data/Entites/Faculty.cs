using ProjeHavuzu.Data.Entites.Common;
using ProjeHavuzu.Data.Entites.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Entites
{
    public class Faculty:BaseEntity

    {
        public string FacultyName { get; set; }
        //relations property
        public ICollection<Department> Departments { get; set; }
        public ICollection<AppUser> AppUsers { get; set; }
    }

}
