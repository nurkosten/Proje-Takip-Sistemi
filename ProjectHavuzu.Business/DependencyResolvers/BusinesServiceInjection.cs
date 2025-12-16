using Microsoft.Extensions.DependencyInjection;
using ProjectHavuzu.Business.Services.Abstracts;
using ProjectHavuzu.Business.Services.Concretes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectHavuzu.Business.DependencyResolvers
{
    public static class BusinesServiceInjection
    {
        public static void AddBusinessServiceInjection(IServiceCollection services)
        {
            
            services.AddScoped<ICategoryManagerService, CategoryManagerService>();
            
        }
    }
}
