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
                    // HttpContext verilerini burada topla (Background Job'da erişilemez)
                    var logDto = new ProjeHavuzu.Data.DTOs.SystemLogDto.SystemLogCreateDto
                    {
                        Controller = controllerName,
                        Action = actionName,
                        Detail = "Kullanıcı işlem gerçekleştirdi.",
                        LogType = "Info",
                        IpAddress = GetIpAddress(context.HttpContext),
                        Url = context.HttpContext.Request?.Path ?? "N/A",
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
