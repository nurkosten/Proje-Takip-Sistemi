using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjeHavuzu.Data.MailService;
using ProjeHavuzu.Data.MailServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DependencyResolvers
{
    public static class MailServiceInjection
    {
        public static void AddMailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMailSender,MailSendService>();
        }
    }
}
