using ProjeHavuzu.Data.Entites.Enums;
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
        public DateTime? UpdatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DataStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string? Descripiton { get; set; }


    }
}
