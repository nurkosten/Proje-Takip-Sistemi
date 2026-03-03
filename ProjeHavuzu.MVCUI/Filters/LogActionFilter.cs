using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjeHavuzu.Business.Services.Abstract;
using System.Security.Claims;
using System.Threading.Tasks;
using Hangfire;

namespace ProjeHavuzu.MVCUI.Filters
{
    public class LogActionAttribute : TypeFilterAttribute
    {
        public LogActionAttribute() : base(typeof(LogActionFilter))
        {
        }
    }

    public class LogActionFilter : IAsyncActionFilter
    {
        private readonly ISystemLogService _systemLogService;

        public LogActionFilter(ISystemLogService systemLogService)
        {
            _systemLogService = systemLogService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Önce işlemi çalıştır
            var resultContext = await next();

            // Sadece başarılı işlemse logla (İsteğe bağlı hata da loglanabilir)
            if (resultContext.Exception == null)
            {
                var controllerName = context.RouteData.Values["controller"]?.ToString();
                var actionName = context.RouteData.Values["action"]?.ToString();
                
                // Ajax isteklerini veya gereksizleri filtrelemek istersek buraya ekleyebiliriz.
                if (controllerName != "SystemLog")
                {
                    // Kullanıcı bilgilerini detaylı al
                    var user = context.HttpContext.User;
                    var userEmail = user.Identity?.IsAuthenticated == true ? user.Identity.Name : "Misafir";
                    var fullNameClaim = user.FindFirst("FullName")?.Value;
                    var roles = string.Join(", ", user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value));
                    
                    var userDisplay = string.IsNullOrEmpty(fullNameClaim) ? userEmail : $"{fullNameClaim} ({userEmail})";
                    if (!string.IsNullOrEmpty(roles)) userDisplay += $" [{roles}]";

                    // URL ve QueryString
                    var path = context.HttpContext.Request?.Path ?? "N/A";
                    var query = context.HttpContext.Request?.QueryString.ToString() ?? "";
                    var fullUrl = path + query;

                    // HttpContext verilerini burada topla (Background Job'da erişilemez)
                    var logDto = new ProjeHavuzu.Data.DTOs.SystemLogDto.SystemLogCreateDto
                    {
                        Controller = controllerName,
                        Action = actionName,
                        Detail = $"{userDisplay} kullanıcısı {controllerName}/{actionName} işlemini gerçekleştirdi.",
                        LogType = "Info",
                        IpAddress = GetIpAddress(context.HttpContext),
                        Url = fullUrl,
                        HttpMethod = context.HttpContext.Request?.Method ?? "N/A",
                        UserAgent = context.HttpContext.Request?.Headers["User-Agent"].ToString() ?? "N/A"
                    };

                    // Kullanıcı ID'sini al (Varsa)
                    var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
                    {
                        logDto.CreatedBy = userId;
                    }

                    // Hangfire ile arka plana at (Fire-and-Forget)
                    BackgroundJob.Enqueue<ISystemLogService>(x => x.AddLogBackgroundAsync(logDto));
                }
            }
        }

        private string GetIpAddress(Microsoft.AspNetCore.Http.HttpContext context)
        {
            try
            {
                // X-Forwarded-For header'ını kontrol et (Proxy varsa)
                var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedHeader))
                {
                    // Birden fazla IP olabilir, ilki gerçek IP'dir
                    return forwardedHeader.Split(',')[0].Trim();
                }

                var ipAddress = context.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrEmpty(ipAddress))
                {
                    return "N/A";
                }

                // Localhost IPv6 düzeltmesi
                if (ipAddress == "::1")
                {
                    ipAddress = "127.0.0.1";
                }

                return ipAddress;
            }
            catch
            {
                return "N/A";
            }
        }
    }
}
