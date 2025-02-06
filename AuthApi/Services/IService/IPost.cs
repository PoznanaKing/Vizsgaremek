using AuthApi.Models.Dtos;

namespace AuthApi.Services.IService
{
    public interface IPost
    {
        Task<object> uploadPost(UploadPostDTO postDTO);
        Task<object> getPostWithId(GetPostWithIdDTO getPostWithIdDTO);
    }
}
