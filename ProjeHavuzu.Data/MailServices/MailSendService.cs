using ProjeHavuzu.Data.MailService;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.MailServices
{
    public class MailSendService : IMailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }
}
