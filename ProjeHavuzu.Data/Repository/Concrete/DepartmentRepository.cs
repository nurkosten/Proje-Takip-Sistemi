using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.DTOs.DepartmentDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Repository.Concrete
{
    public class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
    {
        private readonly ApplicationContext _context;

        public DepartmentRepository(ApplicationContext context, DbSet<Department> dbSet = null) : base(context, dbSet)
        {
            _context = context;
        }
        public List<DepartmentFacultyDto> GetDepartmentsByFacultyName()
        {
            var result = _context.Departments
                .Join(_context.Faculties,
                      d => d.FacultyId,
                      f => f.Id,
                      (d, f) => new DepartmentFacultyDto
                      {
                          
                          FacultyName = f.FacultyName,
                          DepartmentName = d.DepartmentName
                      })
                .OrderBy(x=>x.FacultyName).OrderBy(x=>x.DepartmentName).ToList();

            return result;
        }

    }
}
