

using AuthApi.Models.Dtos;

namespace emailApi.Services.IServices
{
    public interface IEmail
    {
        void SendMail(EmailDTO emailDTO, int code);
        void SendCustomEmail(string to, string content, string senderUsername, bool isAdmin);
    }
}
