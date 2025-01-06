namespace Backend_PM_Project.Models
{
    //userDTOs
    public record RegisterNewUser(string userName, string userPassword, string userEmail);
    public record ResetPasswordDTO(string userNewPassword, Guid userId);
    public record DeleteUserByIdDTO(Guid userId);

    //postDTOs
    public record NewPostDTO(string postTitle, string postDesc, byte[] postImg, Guid userId);

    //trainerDTOs
    public record RegisterNewTrainer(string userName, string userPassword, string userEmail);
}
