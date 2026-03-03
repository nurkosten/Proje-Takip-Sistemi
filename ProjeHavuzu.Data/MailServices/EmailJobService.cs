using Hangfire;
using Microsoft.Extensions.Logging;
using ProjeHavuzu.Data.MailService;
using System.ComponentModel;

namespace ProjeHavuzu.Data.MailServices
{
    /// <summary>
    /// Hangfire tarafından çalıştırılan mail job servisi.
    /// Başarısız gönderimler için Hangfire'ın otomatik retry mekanizması kullanılır.
    /// </summary>
    public class EmailJobService : IEmailJobService
    {
        private readonly IMailSender _mailSender;
        private readonly ILogger<EmailJobService> _logger;

        public EmailJobService(IMailSender mailSender, ILogger<EmailJobService> logger)
        {
            _mailSender = mailSender;
            _logger = logger;
        }

        /// <summary>
        /// Email doğrulama mailini gönderir.
        /// AutomaticRetry: 5 deneme, exponential backoff ile.
        /// </summary>
        [AutomaticRetry(Attempts = 5, DelaysInSeconds = new int[] { 30, 60, 120, 300, 600 })]
        [DisplayName("📧 Email Doğrulama | Alıcı: {0} | Konu: {1}")]
        public async Task SendEmailConfirmationAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("📧 [Hangfire] Email Doğrulama job başlatıldı. Alıcı: {Email}", email);
            Console.WriteLine($"📧 [Hangfire] Email Doğrulama job başlatıldı. Alıcı: {email}");

            try
            {
                await _mailSender.SendEmailAsync(email, subject, htmlMessage);

                _logger.LogInformation("✅ [Hangfire] Email başarıyla gönderildi: {Email}", email);
                Console.WriteLine($"✅ [Hangfire] Email başarıyla gönderildi: {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [Hangfire] Email gönderimi BAŞARISIZ: {Email}, Hata: {Error}", email, ex.Message);
                Console.WriteLine($"❌ [Hangfire] Email gönderimi BAŞARISIZ: {email}, Hata: {ex.Message}");
                throw; // Hangfire retry mekanizması için exception'ı tekrar fırlat
            }
        }

        /// <summary>
        /// Şifre sıfırlama mailini gönderir.
        /// AutomaticRetry: 5 deneme, exponential backoff ile.
        /// </summary>
        [AutomaticRetry(Attempts = 5, DelaysInSeconds = new int[] { 30, 60, 120, 300, 600 })]
        [DisplayName("🔒 Şifre Sıfırlama | Alıcı: {0} | Konu: {1}")]
        public async Task SendPasswordResetAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("📧 [Hangfire] Şifre Sıfırlama job başlatıldı. Alıcı: {Email}", email);
            Console.WriteLine($"📧 [Hangfire] Şifre Sıfırlama job başlatıldı. Alıcı: {email}");

            try
            {
                await _mailSender.SendEmailAsync(email, subject, htmlMessage);

                _logger.LogInformation("✅ [Hangfire] Şifre sıfırlama maili başarıyla gönderildi: {Email}", email);
                Console.WriteLine($"✅ [Hangfire] Şifre sıfırlama maili başarıyla gönderildi: {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [Hangfire] Şifre sıfırlama maili BAŞARISIZ: {Email}, Hata: {Error}", email, ex.Message);
                Console.WriteLine($"❌ [Hangfire] Şifre sıfırlama maili BAŞARISIZ: {email}, Hata: {ex.Message}");
                throw; // Hangfire retry mekanizması için exception'ı tekrar fırlat
            }
        }

        /// <summary>
        /// Genel amaçlı email gönderir.
        /// AutomaticRetry: 5 deneme, exponential backoff ile.
        /// </summary>
        [AutomaticRetry(Attempts = 5, DelaysInSeconds = new int[] { 30, 60, 120, 300, 600 })]
        [DisplayName("✉️ Genel Email | Alıcı: {0} | Konu: {1}")]
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("📧 [Hangfire] Genel email job başlatıldı. Alıcı: {Email}, Konu: {Subject}", email, subject);
            Console.WriteLine($"📧 [Hangfire] Genel email job başlatıldı. Alıcı: {email}, Konu: {subject}");

            try
            {
                await _mailSender.SendEmailAsync(email, subject, htmlMessage);

                _logger.LogInformation("✅ [Hangfire] Email başarıyla gönderildi: {Email}", email);
                Console.WriteLine($"✅ [Hangfire] Email başarıyla gönderildi: {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [Hangfire] Email gönderimi BAŞARISIZ: {Email}, Hata: {Error}", email, ex.Message);
                Console.WriteLine($"❌ [Hangfire] Email gönderimi BAŞARISIZ: {email}, Hata: {ex.Message}");
                throw; // Hangfire retry mekanizması için exception'ı tekrar fırlat
            }
        }
    }
}
