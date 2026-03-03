using System;

namespace ProjeHavuzu.Data.DTOs.LogDto
{
    /// <summary>
    /// Birleşik log filtreleme DTO'su
    /// </summary>
    public class UnifiedLogFilterDto
    {
        /// <summary>
        /// Log Tipi Filtresi (Tümü = null, System, User)
        /// </summary>
        public string? LogTypeFilter { get; set; }
        
        /// <summary>
        /// Log Seviyesi Filtresi (Tümü = null, Info, Warning, Error)
        /// </summary>
        public string? LogLevelFilter { get; set; }
        
        /// <summary>
        /// Başlangıç Tarihi
        /// </summary>
        public DateTime? StartDate { get; set; }
        
        /// <summary>
        /// Bitiş Tarihi
        /// </summary>
        public DateTime? EndDate { get; set; }
        
        /// <summary>
        /// Kullanıcı ID'sine göre filtre
        /// </summary>
        public Guid? UserId { get; set; }
        
        /// <summary>
        /// Controller adına göre arama
        /// </summary>
        public string? ControllerSearch { get; set; }
        
        /// <summary>
        /// Arama metni (Detay içinde arama)
        /// </summary>
        public string? SearchText { get; set; }
    }
}
