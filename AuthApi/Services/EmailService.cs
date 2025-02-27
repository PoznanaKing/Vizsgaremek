using AuthApi.Models.Dtos;

using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using AuthApi.Services.IService;
using emailApi.Services.IServices;

namespace AuthApi.Services
{
    public class EmailService : IEmail
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendMail(EmailDTO emailDTO, int code)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailSettings:EmailUserName").Value));
            email.To.Add(MailboxAddress.Parse(emailDTO.To));
            email.Subject = "Regisztráció";

            email.Body = new TextPart(TextFormat.Html) { Text = $"<p style=\"font-family: Arial, sans-serif; font-size: 16px; color: #333; margin: 0; padding: 10px; background-color: #f9f9f9; border-radius: 5px; display: inline-block;\">Az aktiváló kódja: <span style=\"font-weight: bold; color: #007BFF;\">{code}</span></p>" };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailSettings:EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailSettings:EmailUserName").Value, _configuration.GetSection("EmailSettings:EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
        public void SendMessageEmail(string toEmail, string senderUsername, string content)
        {
            // Tól/től meghatározása
            var lastChar = senderUsername.ToLower().LastOrDefault();
            var suffix = "aeiouáéíóúöüőű".Contains(lastChar) ? "-től" : "-tól";

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:EmailUserName"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Új üzenet érkezett";

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f5f5f5;'>
            <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 8px; padding: 30px;'>
                <h2 style='color: #2c3e50; border-bottom: 2px solid #3498db; padding-bottom: 10px;'>
                    Üzenet érkezett {senderUsername}{suffix}
                </h2>
                <div style='margin: 20px 0; padding: 15px; background: #f8f9fa; border-radius: 5px;'>
                    {content}
                </div>
                <p style='color: #7f8c8d; font-size: 0.9em;'>
                    Ez egy automatikus üzenet, kérjük ne válaszoljon rá.
                </p>
            </div>
        </div>"
            };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration["EmailSettings:EmailHost"], 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration["EmailSettings:EmailUserName"], _configuration["EmailSettings:EmailPassword"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

    }
}
