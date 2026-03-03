namespace ProjeHavuzu.Data.Entites.Enums
{
    /// <summary>
    /// Log kaynağı türü - Sistem veya Kullanıcı kaynaklı loglar
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Sistem tarafından otomatik oluşturulan loglar
        /// </summary>
        System = 0,
        
        /// <summary>
        /// Kullanıcı işlemleri sonucu oluşan loglar
        /// </summary>
        User = 1
    }
}
