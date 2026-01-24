using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.MailService
{
    public interface IMailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
