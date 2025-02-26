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

        public void SendMail(EmailDTO emailDTO,int code)
        {
            
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailSettings:EmailUserName").Value));
            email.To.Add(MailboxAddress.Parse(emailDTO.To));
            email.Subject = "Regisztráció";
            
            email.Body = new TextPart(TextFormat.Html) { Text =$"<p style=\"font-family: Arial, sans-serif; font-size: 16px; color: #333; margin: 0; padding: 10px; background-color: #f9f9f9; border-radius: 5px; display: inline-block;\">Az aktiváló kódja: <span style=\"font-weight: bold; color: #007BFF;\">{code}</span></p>" };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailSettings:EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailSettings:EmailUserName").Value, _configuration.GetSection("EmailSettings:EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        
    }
}
