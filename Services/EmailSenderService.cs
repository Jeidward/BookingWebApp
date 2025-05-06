using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail; // email sending
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class EmailSenderService(IConfiguration configuration)
    {
        public Task SendEmail(string email, string subject, string message)
        {
            var mail = "jeidwardroopz@gmail.com";
            var password = configuration["Email:Smtp:Password"];

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(mail, password),
                EnableSsl = true
            };

           return client.SendMailAsync(new MailMessage(
                from: mail,
                to: email,
                subject,
                message));

        }
    }
}
