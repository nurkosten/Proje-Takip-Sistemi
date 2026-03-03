using System;

namespace ProjeHavuzu.Data.DTOs.LogDto
{
    /// <summary>
    /// Birleşik log listeleme DTO'su
    /// </summary>
    public class UnifiedLogListDto
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// Log Kaynağı: System veya User
        /// </summary>
        public string LogSource { get; set; } = "User";
        
        /// <summary>
        /// Log Seviyesi: Info, Warning, Error
        /// </summary>
        public string LogLevel { get; set; } = "Info";
        
        /// <summary>
        /// Controller adı
        /// </summary>
        public string Controller { get; set; } = string.Empty;
        
        /// <summary>
        /// Action adı
        /// </summary>
        public string Action { get; set; } = string.Empty;
        
        /// <summary>
        /// HTTP metodu (GET, POST, PUT, DELETE)
        /// </summary>
        public string HttpMethod { get; set; } = string.Empty;
        
        /// <summary>
        /// İstek URL'i
        /// </summary>
        public string Url { get; set; } = string.Empty;
        
        /// <summary>
        /// IP Adresi
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// Tarayıcı/User Agent bilgisi
        /// </summary>
        public string UserAgent { get; set; } = string.Empty;
        
        /// <summary>
        /// Log detayı/mesajı
        /// </summary>
        public string Detail { get; set; } = string.Empty;
        
        /// <summary>
        /// Hata detayı (varsa)
        /// </summary>
        public string? Exception { get; set; }
        
        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
        /// <summary>
        /// Kullanıcı ID'si (varsa)
        /// </summary>
        public Guid? UserId { get; set; }
        
        /// <summary>
        /// Kullanıcı adı soyadı ve email
        /// </summary>
        public string UserName { get; set; } = "Misafir / Sistem";
    }
}
