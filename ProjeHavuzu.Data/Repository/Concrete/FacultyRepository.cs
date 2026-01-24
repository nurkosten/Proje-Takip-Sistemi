using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Repository.Concrete
{
    public class FacultyRepository : RepositoryBase<Faculty>, IFacultyRepository
    {
        public FacultyRepository(ApplicationContext context, DbSet<Faculty> dbSet = null) : base(context, dbSet)
        {
        }
    }
}
