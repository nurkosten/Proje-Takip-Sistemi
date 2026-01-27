using ProjeHavuzu.Data.MailService;
using System;
using System.Collections.Generic;
using System.Text;

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
                // TEST MODU: Tüm mailler nurkosten@gmail.com adresine yönlendiriliyor
                string originalReceiver = email;
                email = "nurkosten@gmail.com";

                // Mail içeriğine orijinal alıcı bilgisini ekle
                htmlMessage = $"<div style='background-color: #fff3cd; color: #856404; padding: 10px; margin-bottom: 15px; border: 1px solid #ffeeba; border-radius: 5px; font-family: sans-serif;'>" +
                              $"<strong>⚠️ TEST MODU AKTİF</strong><br>" +
                              $"Bu e-posta aslında şu adrese gönderilecekti: <strong>{originalReceiver}</strong>" +
                              $"</div>" + htmlMessage;

                var mailSettings = _configuration.GetSection("EmailSettings");
                
                var senderEmail = mailSettings["Mail"];
                var senderPassword = mailSettings["Password"];
                var senderHost = mailSettings["Host"];

                // Placeholder kontrolü
                if (senderEmail == "your-email@gmail.com" || senderPassword == "your-app-password")
                {
                     throw new Exception("SMTP Ayarları Yapılmadı: Lütfen appsettings.json dosyasındaki EmailSettings bölümüne geçerli bir Gmail adresi ve Uygulama Şifresi giriniz.");
                }

                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
                {
                   throw new Exception("Email gönderimi için SMTP ayarları (Mail/Password) eksik.");
                }

                var senderPort = int.Parse(mailSettings["Port"]);
                var displayName = mailSettings["DisplayName"];

                var smtpClient = new System.Net.Mail.SmtpClient(senderHost)
                {
                    Port = senderPort,
                    Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                };

                var mailMessage = new System.Net.Mail.MailMessage
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
                // Hatayı fırlat
                throw new Exception($"Mail gönderme başarısız: {ex.Message}");
            }
        }
    }
}
