namespace Backend_PM_Project.Models
{
    public record RegisterNewUser(string userName, string userPassword, string userEmail);
    public record ResetPasswordDTO(string userNewPassword, Guid userId);
}
