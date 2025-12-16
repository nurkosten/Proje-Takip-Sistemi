using Microsoft.Extensions.DependencyInjection;
using ProjeHavuzu.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DependencyResolvers
{
    public static class ApplicationMappingInjection
    {
        public static  void AddApplicationMappingService(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
                
        }
    }
}
