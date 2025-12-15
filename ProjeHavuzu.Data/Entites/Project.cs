using ProjeHavuzu.Data.Entites.Common;
using ProjeHavuzu.Data.Entites.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Entites
{
    public class Project:BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid ApplicationUserId { get; set; }
        
        
        // Navigation property

        public virtual ApplicationUser  ApplicationUser{ get; set; }


    }
}
