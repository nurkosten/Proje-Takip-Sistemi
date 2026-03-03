using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.Entites.Identity;
using ProjeHavuzu.Data.Identity;

public static class ApplicationDbContextInjection
{
    public static void AddApplicationDbContextService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultServer")));

        services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddErrorDescriber<TurkishIdentityErrorDescriber>()
        .AddEntityFrameworkStores<ApplicationContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<RoleManager<AppRole>>();


        services.ConfigureApplicationCookie(opt =>
        {
            opt.LoginPath = "/Account/Login";
            opt.AccessDeniedPath = "/Account/AccessDenied";
            opt.Cookie.Name = "ProjeHavuzu.Auth";
            opt.Cookie.HttpOnly = true;
            opt.ExpireTimeSpan = TimeSpan.FromDays(30); // 30 gün boyunca hatırla
            opt.SlidingExpiration = true; // Kullanıcı siteyi kullandıkça süre uzasın
        });
    }
}
