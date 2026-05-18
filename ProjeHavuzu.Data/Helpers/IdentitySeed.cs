using Microsoft.AspNetCore.Identity;
using ProjeHavuzu.Data.Entites.Identity;

namespace ProjeHavuzu.Data.Helpers
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            await CreateRolesAsync(roleManager);

            await CreateAdminUserAsync(userManager);
            await CreateTeacherUserAsync(userManager);
            await CreateStudentUserAsync(userManager);
        }

        private static async Task CreateRolesAsync(RoleManager<AppRole> roleManager)
        {
            string[] roles = { "Admin", "Teacher", "Student" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new AppRole
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName
                    };

                    var result = await roleManager.CreateAsync(role);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"{roleName} rolü oluşturulamadı: {errors}");
                    }
                }
            }
        }

        private static async Task CreateAdminUserAsync(UserManager<AppUser> userManager)
        {
            var adminEmail = "admin@admin.com";
            var adminPassword = "Admin123+";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    Id = Guid.NewGuid(),
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Admin kullanıcı oluşturulamadı: {errors}");
                }
            }

            await AddRoleIfMissingAsync(userManager, adminUser, "Admin");
        }

        private static async Task CreateTeacherUserAsync(UserManager<AppUser> userManager)
        {
            var teacherEmail = "teacher@ozal.edu.tr";
            var teacherPassword = "Teacher123+";

            var teacherUser = await userManager.FindByEmailAsync(teacherEmail);

            if (teacherUser == null)
            {
                teacherUser = new AppUser
                {
                    Id = Guid.NewGuid(),
                    UserName = teacherEmail,
                    Email = teacherEmail,
                    FirstName = "Test",
                    LastName = "Teacher",
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(teacherUser, teacherPassword);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Teacher kullanıcı oluşturulamadı: {errors}");
                }
            }

            await AddRoleIfMissingAsync(userManager, teacherUser, "Teacher");
        }

        private static async Task CreateStudentUserAsync(UserManager<AppUser> userManager)
        {
            var studentEmail = "student@ozal.edu.tr";
            var studentPassword = "Student123+";

            var studentUser = await userManager.FindByEmailAsync(studentEmail);

            if (studentUser == null)
            {
                studentUser = new AppUser
                {
                    Id = Guid.NewGuid(),
                    UserName = studentEmail,
                    Email = studentEmail,
                    FirstName = "Test",
                    LastName = "Student",
                    StudentNumber = "20260001",
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(studentUser, studentPassword);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Student kullanıcı oluşturulamadı: {errors}");
                }
            }

            await AddRoleIfMissingAsync(userManager, studentUser, "Student");
        }

        private static async Task AddRoleIfMissingAsync(
            UserManager<AppUser> userManager,
            AppUser user,
            string roleName)
        {
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                var roleResult = await userManager.AddToRoleAsync(user, roleName);

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"{user.Email} kullanıcısına {roleName} rolü atanamadı: {errors}");
                }
            }
        }
    }
}