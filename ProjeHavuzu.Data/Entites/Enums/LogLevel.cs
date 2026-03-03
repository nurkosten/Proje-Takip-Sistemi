namespace ProjeHavuzu.Data.Entites.Enums
{
    /// <summary>
    /// Log seviyesi - Bilgi, Uyarı veya Hata
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Bilgi amaçlı loglar
        /// </summary>
        Info = 0,
        
        /// <summary>
        /// Uyarı logları
        /// </summary>
        Warning = 1,
        
        /// <summary>
        /// Hata logları
        /// </summary>
        Error = 2
    }
}
