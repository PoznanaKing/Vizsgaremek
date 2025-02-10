namespace AuthApi.Models.Dtos
{
    public class postDTOs
    {
        public record UploadPostDTO(string post_title, IFormFile? post_image, string post_description, string user_id);
        public record GetPostWithIdDTO(int post_id);
        public record DeletePostDTO(int post_id);
        public record UpdatePostDTO(string post_title, IFormFile? post_image, string post_description, int post_id);
    }
}
