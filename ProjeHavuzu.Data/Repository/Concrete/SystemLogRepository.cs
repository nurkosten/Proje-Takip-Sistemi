using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Repository.Concrete
{
    public class SystemLogRepository : RepositoryBase<SystemLog>, ISystemLogRepository
    {
        public SystemLogRepository(ApplicationContext context, DbSet<SystemLog> dbSet = null) : base(context, dbSet)
        {
        }
    }
}
