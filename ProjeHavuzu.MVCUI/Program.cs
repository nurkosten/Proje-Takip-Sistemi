using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpOverrides;
using ProjeHavuzu.Business.DependencyResolvers;
using ProjeHavuzu.Data.DependencyResolvers;
using ProjeHavuzu.Data.Entites.Identity;
using ProjeHavuzu.Data.Helpers;



using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Hangfire Servisleri
var connectionString = builder.Configuration.GetConnectionString("DefaultServer");
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new Hangfire.SqlServer.SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

// Hangfire Sunucusu
builder.Services.AddHangfireServer();

// Add services to the container.
// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Global Loglama Filtresi - Tüm controllerları loglar
    // options.Filters.Add(typeof(ProjeHavuzu.MVCUI.Filters.LogActionFilter)); 
    // ŞİMDİLİK YORUM SATIRI BIRAKIYORUM. Çünkü her refresh log yaratır. 
    // Sadece [LogAction] attribute'ü olanları veya manuel eklenenleri kullanmak daha temiz olabilir.
    // Ancak kullanıcı "otomatik" istediği için aktif ediyorum:
    options.Filters.Add(typeof(ProjeHavuzu.MVCUI.Filters.LogActionFilter));
}).AddRazorRuntimeCompilation();
builder.Services.AddApplicationDbContextService(builder.Configuration);
builder.Services.AddApplicationMappingService();
builder.Services.AddDataRepositoryServices();
builder.Services.AddBusinessServices();
builder.Services.AddMailService(builder.Configuration);
builder.Services.AddHttpContextAccessor(); // Loglama için gerekli


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


app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // ❗ MUTLAKA ÖNCE
app.UseAuthorization();  // ❗ SONRA

// Hangfire Dashboard (Panel) - İsteğe bağlı "/hangfire" adresinden erişilebilir
app.UseHangfireDashboard("/hangfire");

// Sistem Sağlık Kontrolü (Her Dakika)
RecurringJob.AddOrUpdate<ProjeHavuzu.Business.Services.Abstract.ISystemHealthService>(
    "system-health-check",
    service => service.CheckSystemHealthAsync(),
    Cron.Minutely);

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run(); 
