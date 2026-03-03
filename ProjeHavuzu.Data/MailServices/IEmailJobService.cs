namespace ProjeHavuzu.Data.MailServices
{
    /// <summary>
    /// Hangfire tarafından çağrılan mail job'ları için interface.
    /// Bu interface'in metodları doğrudan Hangfire tarafından invoke edilir.
    /// </summary>
    public interface IEmailJobService
    {
        /// <summary>
        /// Email doğrulama mailini gönderir.
        /// Hangfire bu metodu arka planda çalıştırır.
        /// </summary>
        Task SendEmailConfirmationAsync(string email, string subject, string htmlMessage);

        /// <summary>
        /// Şifre sıfırlama mailini gönderir.
        /// Hangfire bu metodu arka planda çalıştırır.
        /// </summary>
        Task SendPasswordResetAsync(string email, string subject, string htmlMessage);

        /// <summary>
        /// Genel amaçlı email gönderir.
        /// Hangfire bu metodu arka planda çalıştırır.
        /// </summary>
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
