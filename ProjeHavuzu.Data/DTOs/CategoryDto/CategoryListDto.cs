using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace ProjeHavuzu.Data.DTOs.CategoryDto
{
    public class CategoryListDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }


    }
}
