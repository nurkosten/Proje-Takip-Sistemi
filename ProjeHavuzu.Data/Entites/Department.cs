using ProjeHavuzu.Data.Entites.Common;
using ProjeHavuzu.Data.Entites.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjeHavuzu.Data.Entites
{
    public class Department:BaseEntity
    {
        
        public Guid FacultyId { get; set; }
        public string DepartmentName { get; set; }

        //relations property
        public Faculty Faculty { get; set; }
        public ICollection<AppUser> AppUsers { get; set; }
    }
}
