using Hangfire;

namespace ProjeHavuzu.Data.MailServices
{
    /// <summary>
    /// Hangfire ile mail gönderim job'larını kuyruğa ekleyen servis.
    /// HTML içeriği burada üretilir ve Hangfire'a parametre olarak geçilir,
    /// böylece Hangfire Dashboard'da mail içerikleri görünür olur.
    /// </summary>
    public class BackgroundEmailService : IBackgroundEmailService
    {
        /// <summary>
        /// Email doğrulama mailini Hangfire kuyruğuna ekler.
        /// </summary>
        public void EnqueueEmailConfirmation(string email, string confirmationLink)
        {
            var subject = "Email Doğrulama";
            var htmlMessage = GenerateEmailConfirmationHtml(confirmationLink);

            BackgroundJob.Enqueue<IEmailJobService>(
                service => service.SendEmailConfirmationAsync(email, subject, htmlMessage));
        }

        /// <summary>
        /// Şifre sıfırlama mailini Hangfire kuyruğuna ekler.
        /// </summary>
        public void EnqueuePasswordReset(string email, string resetLink)
        {
            var subject = "Şifre Sıfırlama";
            var htmlMessage = GeneratePasswordResetHtml(resetLink);

            BackgroundJob.Enqueue<IEmailJobService>(
                service => service.SendPasswordResetAsync(email, subject, htmlMessage));
        }

        /// <summary>
        /// Genel amaçlı email'i Hangfire kuyruğuna ekler.
        /// </summary>
        public void EnqueueEmail(string email, string subject, string htmlMessage)
        {
            BackgroundJob.Enqueue<IEmailJobService>(
                service => service.SendEmailAsync(email, subject, htmlMessage));
        }

        #region HTML Template Generators

        private static string GenerateEmailConfirmationHtml(string confirmationLink)
        {
            return $@"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f4f4f4; border-radius: 10px;'>
                <div style='background-color: #ffffff; padding: 40px; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); text-align: center;'>
                    <h2 style='color: #28a745; margin-top: 0; margin-bottom: 20px; font-weight: bold;'>🎉 Aramıza Hoş Geldiniz!</h2>
                    <p style='color: #555; font-size: 16px; line-height: 1.6; margin-bottom: 30px;'>Merhaba,<br>Proje Takip Sistemi'ne kaydınız başarıyla oluşturuldu.<br>Hesabınızı aktif etmek ve giriş yapabilmek için lütfen aşağıdaki butonu kullanarak e-posta adresinizi doğrulayın.</p>
                    <a href='{confirmationLink}' style='background-color: #28a745; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 16px; display: inline-block; box-shadow: 0 2px 4px rgba(40, 167, 69, 0.3);'>Hesabımı Doğrula</a>
                    <p style='color: #999; font-size: 13px; margin-top: 30px; border-top: 1px solid #eee; padding-top: 20px;'>Bu bağlantı güvenliğiniz için oluşturulmuştur.</p>
                </div>
            </div>";
        }

        private static string GeneratePasswordResetHtml(string resetLink)
        {
            return $@"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f4f4f4; border-radius: 10px;'>
                <div style='background-color: #ffffff; padding: 40px; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); text-align: center;'>
                    <h2 style='color: #d9534f; margin-top: 0; margin-bottom: 20px; font-weight: bold;'>🔒 Şifre Sıfırlama</h2>
                    <p style='color: #555; font-size: 16px; line-height: 1.6; margin-bottom: 30px;'>Merhaba,<br>Hesabınız için bir şifre sıfırlama talebi aldık. Eğer bu isteği siz yaptıysanız, işlemi tamamlamak için aşağıdaki butona tıklayın.</p>
                    <a href='{resetLink}' style='background-color: #d9534f; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 16px; display: inline-block; box-shadow: 0 2px 4px rgba(217, 83, 79, 0.3);'>Şifremi Sıfırla</a>
                    <p style='color: #999; font-size: 13px; margin-top: 30px; border-top: 1px solid #eee; padding-top: 20px;'>Eğer bu işlemi siz yapmadıysanız, hesabınız güvendedir. Bu maili silebilirsiniz.</p>
                </div>
            </div>";
        }

        #endregion
    }
}
