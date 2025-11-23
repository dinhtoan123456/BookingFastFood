using Azure.Core;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Ass1_C_5_OrderFastFood.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var fromEmail = "dinhtoann52@gmail.com";
                var password = "eoek btrr oshl dljx"; // App password Google

                var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage(fromEmail, email, subject, htmlMessage)
                {
                    IsBodyHtml = true
                };

                await smtp.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email");
                throw;
            }
        }
    }
}
