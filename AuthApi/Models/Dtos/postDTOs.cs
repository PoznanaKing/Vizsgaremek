namespace AuthApi.Models.Dtos
{
    public class postDTOs
    {
        public record UploadPostDTO(string post_title, byte[]? post_image, string post_description, string user_id);
        public record GetPostWithIdDTO(int post_id);
        public record DeletePostDTO(int post_id);
    }
}
