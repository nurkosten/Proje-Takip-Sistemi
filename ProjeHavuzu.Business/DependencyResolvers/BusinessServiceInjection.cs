using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Business.Services.Concrete;
using ProjeHavuzu.Data.DTOs.CategoryDto;
using ProjeHavuzu.Data.DTOs.DepartmentDto;
using ProjeHavuzu.Data.DTOs.FacultyDto;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.Validators.CategoryValidators;
using ProjeHavuzu.Data.Validators.DepartmentValidators;
using ProjeHavuzu.Data.Validators.FacultyValidators;
using ProjeHavuzu.Data.Validators.ProjectValidators;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Business.DependencyResolvers
{
    public static class BusinessServiceInjection
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            // Service Injections
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IFacultyService, FacultyService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IProjectStudentService, ProjectStudentService>();
            services.AddScoped<ISystemLogService, SystemLogService>();
            services.AddScoped<ISystemHealthService, SystemHealthService>(); // Health Check Servisi
            services.AddScoped<IProjectRequestService, ProjectRequestService>();
            services.AddScoped<IUnifiedLogService, UnifiedLogService>(); // Birleşik Log Servisi
            services.AddScoped<IProjectSubmissionService, ProjectSubmissionService>();
            services.AddScoped<IAdvisorCandidateService, AdvisorCandidateService>();

            // Validator Injections
            services.AddScoped<IValidator<ProjectCreateDto>, ProjectCreateValidator>();
            services.AddScoped<IValidator<CategoryAddDto>, CategoryAddValidator>();
            services.AddScoped<IValidator<CategoryEditDto>, CategoryEditValidator>();
            services.AddScoped<IValidator<FacultyCreateDto>, FacultyCreateValidator>();
            services.AddScoped<IValidator<DepartmentListDto>, DepartmentListValidator>();
        }
    }
}
