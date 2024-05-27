using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Interfaces;
using System.Runtime.CompilerServices;

namespace SocialMediaAppAPI.Services
{
    public class PostService(APIDbContext _dbContext) : IPostService
    {
        // Add: Puts the post given in the database.
        public async Task<Post> Add(Post post)
        {
            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();
            return post;
        }

        // Delete: Removes/Destroys the post given in the database.
        public async Task<Post> Delete(Post post)
        {
            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();
            return post;
        }

        // Get: Gets the post based on the given ID.
        public Task<Post> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetFeed(User user)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetFeedWithAttatchments(User user)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetFeedWithMetaData(User user)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetWithAllData(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetWithAttatchments(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Post> Update(Post post)
        {
            throw new NotImplementedException();
        }
    }
}
