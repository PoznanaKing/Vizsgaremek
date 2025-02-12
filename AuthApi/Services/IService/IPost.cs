

using AuthApi.Models;
using static AuthApi.Models.Dtos.postDTOs;

namespace AuthApi.Services.IService
{
    public interface IPost
    {
        Task<object> uploadPost(UploadPostDTO postDTO);
        Task<object> getPostWithId(GetPostWithIdDTO getPostWithIdDTO);
        Task<PostTable> deletePost(DeletePostDTO deletePostDTO);
        Task<PostTable> updatePost(UpdatePostDTO updatePostDTO);
        
    }
}
