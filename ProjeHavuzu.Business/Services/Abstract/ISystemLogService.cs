using ProjeHavuzu.Data.DTOs.SystemLogDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Abstract
{
    public interface ISystemLogService
    {
        Task<List<SystemLogListDto>> GetAllLogsAsync();
        Task AddLogAsync(string controller, string action, string detail, string logType = "Info", string? exception = null);
        Task AddLogBackgroundAsync(SystemLogCreateDto logDto); // Hangfire için
        Task<List<SystemLogListDto>> GetLogsByUserIdAsync(Guid userId);
        Task ClearLogsAsync(); // Opsiyonel
    }
}
