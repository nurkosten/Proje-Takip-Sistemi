using Microsoft.Extensions.DependencyInjection;
using ProjeHavuzu.Data.Repository.Abstract;
using ProjeHavuzu.Data.Repository.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DependencyResolvers
{
    public static class DataRepositoryInjection
    {
        public static void AddDataRepositoryServices(this IServiceCollection services)
        {
            
            services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<ICategoryRepository, CategoryRepository>();
        }
    }
}
