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
            services.AddScoped<ICurrentUserService, CurrentUserService>();


            // Generic Repository Injection
            services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IFacultyRepository, FacultyRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IProjectStudentRepository, ProjectStudentRepository>();
            services.AddScoped<ISystemLogRepository, SystemLogRepository>();
        }
    }
}
