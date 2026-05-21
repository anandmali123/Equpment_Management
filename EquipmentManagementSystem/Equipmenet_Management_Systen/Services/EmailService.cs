using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Equipmenet_Management_Systen.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromPassword = _configuration["EmailSettings:FromPassword"];

                var smtp = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                var message = new MailMessage();
                message.From = new MailAddress(fromEmail);
                message.To.Add(to);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Log error appropriately
                throw new InvalidOperationException("Failed to send email", ex);
            }
        }
    }
}
