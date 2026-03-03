using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.DTOs.LogDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Abstract
{
    /// <summary>
    /// Birleşik Log Yönetimi Servisi Interface'i
    /// SOLID: ISP (Interface Segregation Principle) - Sadece log yönetimi ile ilgili metotlar
    /// </summary>
    public interface IUnifiedLogService
    {
        /// <summary>
        /// Log sayfasını ilk yüklerken gerekli filtre verilerini getirir.
        /// Log listesi BOŞ döner (AJAX ile çekilecek).
        /// </summary>
        Task<LogPageViewModel> GetLogsViewModelAsync(UnifiedLogFilterDto filter);

        /// <summary>
        /// DataTables için server-side log verisi getirir.
        /// </summary>
        Task<DataTableResponse<UnifiedLogListDto>> GetLogsDataAsync(DataTableRequest request, UnifiedLogFilterDto filter);

        /// <summary>
        /// Toplam log sayısını getirir
        /// </summary>
        Task<int> GetTotalLogCountAsync();

        /// <summary>
        /// Belirli bir log kaydını siler (soft delete)
        /// </summary>
        Task<bool> DeleteLogAsync(System.Guid logId);

        /// <summary>
        /// Seçili tarih aralığındaki logları temizler
        /// </summary>
        Task<int> ClearOldLogsAsync(int olderThanDays);
    }
}
