using System.Net.Mail;
using System.Net;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Core.Config;

namespace Core.Services
{
    public class EmailServiceProvider : IEmailServiceProvider
    {
        private readonly EmailSettings _emailSettings;

        public EmailServiceProvider(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }



        public async Task<bool> SendNoReplyEmailAsync(string recipient, string subject, string content)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.To.Add(recipient);

                    message.Subject = subject;
                    message.Body = content;

                    using (var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
                           {
                               Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                               EnableSsl = true
                           })
                    {
                        await smtpClient.SendMailAsync(message);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
