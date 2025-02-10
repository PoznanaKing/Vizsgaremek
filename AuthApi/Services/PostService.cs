using AuthApi.Models.Dtos;
using AuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using AuthApi.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using static AuthApi.Models.Dtos.postDTOs;

namespace AuthApi.Services
{
    public class PostService:IPost
    {
        private readonly AppDbContext _appDbContext;

        public PostService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<PostTable> deletePost(DeletePostDTO deletePostDTO)
        {
            var deletingPost = await _appDbContext.posts.FirstOrDefaultAsync(x=>x.PostId==deletePostDTO.post_id);
            if (deletingPost != null)
            {
                return deletingPost;
            }
            return null;
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

        public async Task<PostTable> updatePost(UpdatePostDTO updatePostDTO)
        {
            byte[] postImageBytes = null;

            // Ha van kép, konvertáld byte[]-ra
            if (updatePostDTO.post_image != null && updatePostDTO.post_image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await updatePostDTO.post_image.CopyToAsync(memoryStream); // Aszinkron fájl másolás
                    postImageBytes = memoryStream.ToArray();
                }
            }

            // Aszinkron módon lekérjük a posztot az adatbázisból
            var updatePost = await _appDbContext.posts.FirstOrDefaultAsync(x => x.PostId == updatePostDTO.post_id);

            if (updatePost != null)
            {
                // Frissítjük az adatokat az eredeti objektumban
                updatePost.PostTitle = updatePostDTO.post_title;
                updatePost.PostDescription = updatePostDTO.post_description;
                updatePost.PostImage = postImageBytes;

                // Elmentjük a változtatásokat az adatbázisba
                await _appDbContext.SaveChangesAsync(); // Ez a változtatások mentéséért felelős

                return updatePost; // Visszatérünk a frissített objektummal
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
                    postDTO.post_image.CopyTo(memoryStream);
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
