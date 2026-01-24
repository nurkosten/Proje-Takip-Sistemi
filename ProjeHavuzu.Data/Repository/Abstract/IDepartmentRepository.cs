using ProjeHavuzu.Data.DTOs.DepartmentDto;
using ProjeHavuzu.Data.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Repository.Abstract
{
    public interface IDepartmentRepository:IRepository<Department> 
    {

        List<DepartmentFacultyDto> GetDepartmentsByFacultyName();
    }
}
