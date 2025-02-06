

using AuthApi.Models.Dtos;

namespace emailApi.Services.IServices
{
    public interface IEmail
    {
        void SendMail(EmailDTO emailDTO);
    }
}
