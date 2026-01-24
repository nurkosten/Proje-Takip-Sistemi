using Microsoft.AspNetCore.Identity;
using ProjeHavuzu.Data.Entites.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjeHavuzu.Data.Helpers
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            // 🔹 Admin Rolü
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new AppRole
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin"
                });
            }

            // 🔹 Admin Kullanıcı
            var adminEmail = "admin@admin.com";
            var adminPassword = "Admin123+";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    Id = Guid.NewGuid(),
                    UserName = adminEmail,   // 🔥 ZORUNLU
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (!createResult.Succeeded)
                {
                    // Hata loglama için
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Admin kullanıcı oluşturulamadı: {errors}");
                }

                var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Admin rolü atanamadı: {errors}");
                }
            }
            else
            {
                // Kullanıcı varsa şifresini güncelle (isteğe bağlı - sadece şifre yanlışsa)
                // Bu kısmı yorum satırına alabilirsiniz
                // var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
                // await userManager.ResetPasswordAsync(adminUser, token, adminPassword);
            }
        }
    }

}
