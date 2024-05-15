using Core.Config;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Core.Services
{
    public class EmailServiceProvider : IEmailServiceProvider
    {
        private readonly EmailSettings _emailSettings;
        private ILogger<EmailServiceProvider> _logger;

        public EmailServiceProvider(EmailSettings emailSettings, ILogger<EmailServiceProvider> logger)
        {
            _emailSettings = emailSettings;
            _logger = logger;
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
                _logger.LogError(ex, ex.Message);
                return false;
            }

        }
    }
}
