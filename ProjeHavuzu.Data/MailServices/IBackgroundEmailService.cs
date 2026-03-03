namespace ProjeHavuzu.Data.MailServices
{
    /// <summary>
    /// Background mail gönderim işlemleri için interface.
    /// Bu interface, mail gönderimlerini Hangfire job'larına kuyruklar.
    /// </summary>
    public interface IBackgroundEmailService
    {
        /// <summary>
        /// Email doğrulama mailini arka planda gönderir.
        /// </summary>
        /// <param name="email">Alıcı email adresi</param>
        /// <param name="confirmationLink">Doğrulama linki</param>
        void EnqueueEmailConfirmation(string email, string confirmationLink);

        /// <summary>
        /// Şifre sıfırlama mailini arka planda gönderir.
        /// </summary>
        /// <param name="email">Alıcı email adresi</param>
        /// <param name="resetLink">Şifre sıfırlama linki</param>
        void EnqueuePasswordReset(string email, string resetLink);

        /// <summary>
        /// Genel amaçlı email'i arka planda gönderir.
        /// </summary>
        /// <param name="email">Alıcı email adresi</param>
        /// <param name="subject">Email konusu</param>
        /// <param name="htmlMessage">HTML formatında email içeriği</param>
        void EnqueueEmail(string email, string subject, string htmlMessage);
    }
}
