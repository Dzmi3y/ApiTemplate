using System.Net.Mail;
using System.Net;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Core.Config;

namespace Core.Services
{
    public class EmailServiceProvider : IEmailServiceProvider
    {
        private readonly EmailSettings _email;

        public EmailServiceProvider(EmailSettings email)
        {
            _email = email;
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

                    using (var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                           {
                               Credentials = new NetworkCredential(_email.Email, _email.Password),
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
