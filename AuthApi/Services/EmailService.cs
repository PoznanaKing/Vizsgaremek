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

        public void SendMail(EmailDTO emailDTO)
        {
            
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailSettings:EmailUserName").Value));
            email.To.Add(MailboxAddress.Parse(emailDTO.To));
            email.Subject = "Regisztráció";
            
            email.Body = new TextPart(TextFormat.Html) { Text = "<h1 style=\"font-family: 'Brush Script MT', cursive; font-size: 40px; color: #ff1493; text-align: center; padding: 40px; background-color: #8a2be2; border-radius: 30px; box-shadow: 0 0 30px rgba(0, 0, 0, 0.5); text-shadow: 3px 3px 6px rgba(0, 0, 0, 0.4); transform: scale(1.2); transition: transform 0.3s ease; border: 5px solid #ffd700; border-radius: 50px; background-image: url('https://img.icons8.com/ios/50/ffffff/sparkles.png'); background-size: 30px 30px; background-repeat: no-repeat; background-position: center center;\">\r\n  ✨ Köszönjük a regisztrációt a PM project-nél! ✨\r\n</h1>\r\n" };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailSettings:EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailSettings:EmailUserName").Value, _configuration.GetSection("EmailSettings:EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        
    }
}
