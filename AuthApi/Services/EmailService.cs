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

        public void SendCustomEmail(string to, string content, string senderUsername, bool isAdmin)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailSettings:EmailUserName").Value));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = "Üzenet érkezett";

            var senderName = isAdmin
                ? $"<span style=\"color: red;\">{senderUsername}</span>"
                : senderUsername;

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = $"<p style=\"font-family: Arial, sans-serif; font-size: 16px; color: #333; margin: 0; padding: 10px; background-color: #f9f9f9; border-radius: 5px; display: inline-block;\">Üzenet érkezett {senderName}-tól/től:</p>" +
                       $"<div style=\"font-family: Arial, sans-serif; font-size: 14px; color: #555; margin: 10px 0; padding: 10px; background-color: #f0f0f0; border-radius: 5px;\">{content}</div>"
            };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailSettings:EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailSettings:EmailUserName").Value, _configuration.GetSection("EmailSettings:EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
