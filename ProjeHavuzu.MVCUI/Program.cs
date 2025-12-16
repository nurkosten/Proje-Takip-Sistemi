using ProjectHavuzu.Business.Services.Abstracts;
using ProjectHavuzu.Business.Services.Concretes;
using ProjeHavuzu.Data.DependencyResolvers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddApplicationDbContextService(builder.Configuration);
builder.Services.AddApplicationMappingService();
builder.Services.AddDataRepositoryServices();

builder.Services.AddScoped<ICategoryManagerService, CategoryManagerService>();
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
    await ApplicationRoleSeedInjection.SeedRolesAsync(scope.ServiceProvider);
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // ❗ MUTLAKA ÖNCE
app.UseAuthorization();  // ❗ SONRA

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
