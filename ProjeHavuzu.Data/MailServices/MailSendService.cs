using ProjeHavuzu.Data.MailService;

namespace ProjeHavuzu.Data.MailServices
{
    public class MailSendService : IMailSender
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public MailSendService(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var mailSettings = _configuration.GetSection("EmailSettings");
                var testMode = bool.TryParse(mailSettings["TestMode"], out var isTestMode) && isTestMode;
                var testEmail = mailSettings["TestEmail"];

                if (testMode && !string.IsNullOrWhiteSpace(testEmail))
                {
                    email = testEmail;
                }

                var senderEmail = mailSettings["Mail"];
                var senderPassword = mailSettings["Password"];
                var senderHost = mailSettings["Host"];

                if (senderEmail == "your-email@gmail.com" || senderPassword == "your-app-password")
                {
                    throw new Exception("SMTP Ayarları Yapılmadı: Lütfen appsettings.json dosyasındaki EmailSettings bölümüne geçerli bir Gmail adresi ve Uygulama Şifresi giriniz.");
                }

                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
                {
                    throw new Exception("Email gönderimi için SMTP ayarları (Mail/Password) eksik.");
                }

                if (!int.TryParse(mailSettings["Port"], out int senderPort))
                {
                    senderPort = 587;
                }

                var displayName = mailSettings["DisplayName"] ?? "Proje Takip Sistemi";

                using var smtpClient = new System.Net.Mail.SmtpClient(senderHost)
                {
                    Port = senderPort,
                    Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                };

                using var mailMessage = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress(senderEmail, displayName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Mail gönderme başarısız: {ex.Message}");
            }
        }
    }
}
