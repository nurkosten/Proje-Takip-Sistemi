using AutoMapper;
using Microsoft.AspNetCore.Http;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.SystemLogDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Concrete
{
    public class SystemLogService : ISystemLogService
    {
        private readonly ISystemLogRepository _systemLogRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SystemLogService(ISystemLogRepository systemLogRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _systemLogRepository = systemLogRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<SystemLogListDto>> GetAllLogsAsync()
        {
            var logs = await _systemLogRepository.ListAsync(x => !x.IsDeleted);
            var sortedLogs = logs.OrderByDescending(x => x.CreatedDate).ToList();
            return _mapper.Map<List<SystemLogListDto>>(sortedLogs);
        }

        public async Task AddLogAsync(string controller, string action, string detail, string logType = "Info", string exception = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var log = new SystemLog
            {
                Controller = controller,
                Action = action,
                Detail = detail,
                LogType = logType,
                Exception = exception,
                CreatedDate = DateTime.UtcNow
            };

            // Otomatik veri toplama
            if (httpContext != null)
            {
                log.IpAddress = httpContext.Connection?.RemoteIpAddress?.ToString();
                log.Url = httpContext.Request?.Path;
                log.HttpMethod = httpContext.Request?.Method;
                log.UserAgent = httpContext.Request?.Headers["User-Agent"].ToString();
            }
            else
            {
                // HttpContext yoksa (örn: Background Service) boş geç
                log.IpAddress = "N/A";
                log.Url = "N/A";
                log.HttpMethod = "N/A";
                log.UserAgent = "System";
            }

            await _systemLogRepository.AddAsync(log);
        }

        public async Task AddLogBackgroundAsync(SystemLogCreateDto logDto)
        {
            var log = new SystemLog
            {
                Controller = logDto.Controller,
                Action = logDto.Action,
                Detail = logDto.Detail,
                LogType = logDto.LogType,
                Exception = logDto.Exception,
                IpAddress = logDto.IpAddress,
                Url = logDto.Url,
                HttpMethod = logDto.HttpMethod,
                UserAgent = logDto.UserAgent,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = logDto.CreatedBy ?? Guid.Empty // Eğer kullanıcı ID varsa
            };

            await _systemLogRepository.AddAsync(log);
        }

        public async Task ClearLogsAsync()
        {
             var logs = await _systemLogRepository.ListAsync(x => !x.IsDeleted);
             foreach(var log in logs)
             {
                 log.IsDeleted = true;
                 _systemLogRepository.Update(log);
             }
        }
    }
}
