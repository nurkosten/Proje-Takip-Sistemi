using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.DTOs.SystemLogDto; // DTO için
using System;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Concrete
{
    public class SystemHealthService : ISystemHealthService
    {
        private readonly ApplicationContext _context;
        private readonly ISystemLogService _systemLogService;

        public SystemHealthService(ApplicationContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
        }

        public async Task CheckSystemHealthAsync()
        {
            try
            {
                // Veritabanı bağlantı kontrolü
                bool canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    // Sağlıklı ise Info logu at
                   await _systemLogService.AddLogBackgroundAsync(new SystemLogCreateDto
                   {
                       Controller = "SystemHealth",
                       Action = "Check",
                       Detail = "Sistem sağlığı kontrol edildi: Veritabanı bağlantısı başarılı.",
                       LogType = "SystemCheck", // Özel bir log tipi
                       IpAddress = "Localhost",
                       Url = "Hangfire/RecurringJob",
                       HttpMethod = "JOB",
                       UserAgent = "Hangfire Server"
                   });
                }
                else
                {
                    // Bağlanamıyorsa Error logu at
                    await _systemLogService.AddLogBackgroundAsync(new SystemLogCreateDto
                    {
                        Controller = "SystemHealth",
                        Action = "Check",
                        Detail = "Sistem sağlığı kontrol edildi: Veritabanına erişilemiyor!",
                        LogType = "Error",
                        IpAddress = "Localhost",
                        Url = "Hangfire/RecurringJob",
                        HttpMethod = "JOB",
                        UserAgent = "Hangfire Server",
                        Exception = "CanConnectAsync returned false"
                    });
                }
            }
            catch (Exception ex)
            {
                // Kod hatası olursa
                await _systemLogService.AddLogBackgroundAsync(new SystemLogCreateDto
                {
                    Controller = "SystemHealth",
                    Action = "Check",
                    Detail = "Sistem sağlığı kontrol edilirken kritik hata oluştu.",
                    LogType = "Fatal",
                    IpAddress = "Localhost",
                    Url = "Hangfire/RecurringJob",
                    HttpMethod = "JOB",
                    UserAgent = "Hangfire Server",
                    Exception = ex.Message
                });
            }
        }
    }
}
