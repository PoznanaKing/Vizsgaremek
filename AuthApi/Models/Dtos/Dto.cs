using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AuthApi.Models.Dtos
{
    public class LoginRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginRequestDto(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
    public class RegisterRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }

        public RegisterRequestDto(string userName, string password, string email, string fullName)
        {
            UserName = userName;
            Password = password;
            Email = email;
            FullName = fullName;
        }
    }
    public record AssignRoleRequestDto(string UserName, string RoleName);

    public record EmailDTO(string To);
    public class SendMessageRequestDto
    {
        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public string Content { get; set; }
    }
    public record UserByIdDTO(string username, string id, string email);
    public record UserDataUpdateDTO(string username, string email, string id);
    public class UserPasswordUpdateDTO
    {
        public string Id { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
