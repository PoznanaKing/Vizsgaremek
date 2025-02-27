namespace AuthApi.Models.Dtos
{
    public record LoginRequestDto(string UserName, string Password);
    public record RegisterRequestDto(string UserName, string Password, string Email, string FullName);
    public record AssignRoleRequestDto(string UserName, string RoleName);

    public record EmailDTO(string To);
    public record CustomEmailRequestDto (string To, String Content, String UserId);
}
