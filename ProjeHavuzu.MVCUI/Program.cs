using Microsoft.AspNetCore.Identity;
using ProjeHavuzu.Business.DependencyResolvers;
using ProjeHavuzu.Data.DependencyResolvers;
using ProjeHavuzu.Data.Entites.Identity;
using ProjeHavuzu.Data.Helpers;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddApplicationDbContextService(builder.Configuration);
builder.Services.AddApplicationMappingService();
builder.Services.AddDataRepositoryServices();
builder.Services.AddBusinessServices();
builder.Services.AddMailService(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

    await IdentitySeed.SeedAsync(userManager, roleManager);
}


using (var scope = app.Services.CreateScope())
{
    await ApplicationRoleSeedInjection.SeedRolesAsync(scope.ServiceProvider);
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // ❗ MUTLAKA ÖNCE
app.UseAuthorization();  // ❗ SONRA

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run(); 
