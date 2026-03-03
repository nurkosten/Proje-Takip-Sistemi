using Hangfire.Dashboard;

namespace ProjeHavuzu.MVCUI.Filters
{
    /// <summary>
    /// Hangfire Dashboard için Admin yetkilendirme filtresi.
    /// Sadece Admin rolündeki kullanıcıların dashboard'a erişmesine izin verir.
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            
            // Kullanıcı giriş yapmış mı ve Admin rolünde mi kontrol et
            return httpContext.User.Identity?.IsAuthenticated == true 
                   && httpContext.User.IsInRole("Admin");
        }
    }
}
