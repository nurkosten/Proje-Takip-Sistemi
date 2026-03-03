using System;
using System.Collections.Generic;

namespace ProjeHavuzu.Data.DTOs.LogDto
{
    /// <summary>
    /// Log sayfası için ViewModel - Filtreleri ve log listesini birlikte taşır
    /// </summary>
    public class LogPageViewModel
    {
        /// <summary>
        /// Filtreleme parametreleri
        /// </summary>
        public UnifiedLogFilterDto Filter { get; set; } = new UnifiedLogFilterDto();
        
        /// <summary>
        /// Filtrelenmiş log listesi
        /// </summary>
        public List<UnifiedLogListDto> Logs { get; set; } = new List<UnifiedLogListDto>();
        
        /// <summary>
        /// Toplam log sayısı (filtresiz)
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// Filtrelenmiş log sayısı
        /// </summary>
        public int FilteredCount { get; set; }
        
        /// <summary>
        /// Kullanıcı listesi (dropdown için)
        /// </summary>
        public List<UserSelectItem> Users { get; set; } = new List<UserSelectItem>();
    }
    
    /// <summary>
    /// Kullanıcı seçimi için basit item sınıfı
    /// </summary>
    public class UserSelectItem
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
