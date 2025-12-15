using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Entites.Identity
{
    public class ApplicationRole:IdentityRole<Guid>
    {
        public string RoleName { get; set; }
        public string Description { get; set; } 
    }
}
