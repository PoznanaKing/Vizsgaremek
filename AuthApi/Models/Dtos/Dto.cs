namespace AuthApi.Models.Dtos
{
    public record LoginRequestDto(string UserName, string Password);
    public record RegisterRequestDto(string UserName, string Password, string Email, string FullName);
    public record AssignRoleRequestDto(string UserName, string RoleName);

    public record EmailDTO(string To);
    public record UploadPostDTO(string post_title, byte[]? post_image, string post_description, string user_id);
    public record GetPostWithIdDTO(int post_id);
}
