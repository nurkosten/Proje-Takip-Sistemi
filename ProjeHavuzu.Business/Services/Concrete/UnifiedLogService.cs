using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.DTOs.LogDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Identity;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Concrete
{
    public class UnifiedLogService : IUnifiedLogService
    {
        private readonly ISystemLogRepository _systemLogRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UnifiedLogService(
            ISystemLogRepository systemLogRepository,
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _systemLogRepository = systemLogRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<LogPageViewModel> GetLogsViewModelAsync(UnifiedLogFilterDto filter)
        {
            var userSelectList = await _userManager.Users.AsNoTracking()
                .Where(u => u.IsActive)
                .OrderBy(u => u.FirstName)
                .Select(u => new UserSelectItem
                {
                    Id = u.Id,
                    DisplayName = $"{u.FirstName} {u.LastName}"
                }).ToListAsync();

            return new LogPageViewModel
            {
                Filter = filter ?? new UnifiedLogFilterDto(),
                Logs = new List<UnifiedLogListDto>(),
                TotalCount = 0,
                FilteredCount = 0,
                Users = userSelectList
            };
        }

        public async Task<DataTableResponse<UnifiedLogListDto>> GetLogsDataAsync(DataTableRequest request, UnifiedLogFilterDto filter)
        {
            return await _systemLogRepository.GetLogsServerSideAsync(request, filter);
        }

        public async Task<int> GetTotalLogCountAsync()
        {
            // Bu fonksiyon kullanılmıyor, DataTables kendi count'unu alıyor.
            // Bellek sorunu yaratmamak için boş dönüyoruz.
            return 0;
        }

        public async Task<bool> DeleteLogAsync(Guid logId)
        {
            var log = await _systemLogRepository.GetAsync(x => x.Id == logId && !x.IsDeleted);
            if (log == null) return false;

            log.IsDeleted = true;
            log.UpdatedDate = DateTime.UtcNow;
            _systemLogRepository.Update(log);
            return true;
        }

        public async Task<int> ClearOldLogsAsync(int olderThanDays)
        {
            // Dikkat: Çok fazla kayıt varsa bu da yavaş olabilir, ancak şimdilik böyle bırakıyoruz.
            var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
            var oldLogs = await _systemLogRepository.ListAsync(x => !x.IsDeleted && x.CreatedDate < cutoffDate);

            foreach (var log in oldLogs)
            {
                log.IsDeleted = true;
                log.UpdatedDate = DateTime.UtcNow;
                _systemLogRepository.Update(log);
            }

            return oldLogs.Count;
        }
    }
}
