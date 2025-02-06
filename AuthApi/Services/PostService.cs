using AuthApi.Models.Dtos;
using AuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using AuthApi.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace AuthApi.Services
{
    public class PostService:IPost
    {
        private readonly AppDbContext _appDbContext;

        public PostService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<object> getPostWithId(GetPostWithIdDTO getPostWithIdDTO)
        {
            var result = await _appDbContext.posts.FirstOrDefaultAsync(x=>x.PostId==getPostWithIdDTO.post_id);
            if (result!=null)
            {
                return result;
            }
            return null;
        }

        public async Task<object> uploadPost(UploadPostDTO postDTO)
        {
            byte[] postImageBytes = null;
            if (postDTO.post_image!=null&&postDTO.post_image.Length>0)
            {
                using(var memoryStream = new MemoryStream())
                {
                    postDTO.post_image.CopyTo(memoryStream.ToArray(),postDTO.post_image.Length);
                    postImageBytes = memoryStream.ToArray();
                }
            }
            var result = new PostTable
            {
                PostTitle = postDTO.post_title,
                PostDescription = postDTO.post_description,
                PostImage = postImageBytes,
                UserId = postDTO.user_id,
            };
            if (result != null)
            {
                
                return result;
            }
            return null;
        }
    }
}
