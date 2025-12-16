using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProjeHavuzu.Data.Entites.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DependencyResolvers
{
    public static class ApplicationRoleSeedInjection
    {
        public static async Task SeedRolesAsync(this IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();

            string[] roles = {"Admin","Teacher", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new AppRole
                    {
                        Name = role
                    });
                }
            }
        }
    }
}
