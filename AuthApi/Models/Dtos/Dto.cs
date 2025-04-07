using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AuthApi.Models.Dtos
{
    public record LoginRequestDto(string UserName, string Password);
    public record RegisterRequestDto(string UserName, string Password, string Email, string FullName);
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
