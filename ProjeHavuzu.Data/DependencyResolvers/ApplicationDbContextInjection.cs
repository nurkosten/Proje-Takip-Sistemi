using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.Entites.Identity;

public static class ApplicationDbContextInjection
{
    public static void AddApplicationDbContextService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultServer")));

        services.
            AddIdentity<AppUser, AppRole>(options =>
        {
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationContext>()
        .AddDefaultTokenProviders();
        services.AddScoped<RoleManager<AppRole>>();


        services.ConfigureApplicationCookie(opt =>
        {
            opt.LoginPath = "/Account/Login";
            opt.AccessDeniedPath = "/Account/AccessDenied";
        });
    }
}
