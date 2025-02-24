using AuthApi.Models.Dtos;

using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using AuthApi.Services.IService;
using emailApi.Services.IServices;
using AuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Services
{
    public class EmailService : IEmail
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _appDbContext;

        public EmailService(IConfiguration configuration, AppDbContext appDbContext)
        {
            _configuration = configuration;
            _appDbContext = appDbContext;
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

        public void SendMailFromTo(EmailFromToDTO emailFromToDTO)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailSettings:EmailUserName").Value));
            email.To.Add(MailboxAddress.Parse(emailFromToDTO.To));
            email.Subject = "Üzenet egy személyiedzőnek";

            email.Body = new TextPart(TextFormat.Html) { Text = "<div style=\"background-color:#2F4F4F; text-align:center; padding:20px;\"><h1 style=\"color:#D3D3D3;\">Kedves " + (_appDbContext.Users.FirstOrDefault(x => x.Id == emailFromToDTO.trainerid)?.UserName ?? "Ismeretlen")+ "</h1><h2 style=\"color:#D3D3D3;\">" + (_appDbContext.Users.FirstOrDefault(x => x.Id == emailFromToDTO.userid)?.UserName ?? "Ismeretlen") + " Üzenetet küldött neked!</h2><p style=\"color:#D3D3D3;\">" + emailFromToDTO.content + "</p></div>" };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailSettings:EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailSettings:EmailUserName").Value, _configuration.GetSection("EmailSettings:EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
