using SocialMediaAppAPI.Models;

namespace SocialMediaAppAPI.Types.Interfaces
{
    public interface IPostService
    {
        // Base CRUD Operations
        Task<Post> Add(Post post);
        Task<Post> Update(Post post);
        Task<Post> Delete(Post post);
        Task<Post> Get(Guid id);
        Task<Post> GetWithAttatchments(Guid id);
        Task<Post> GetWithAllData(Guid id);

        // Feed Get Operations
        Task<Post> GetFeed(User user);
        Task<Post> GetFeedWithAttatchments(User user);
        Task<Post> GetFeedWithMetaData(User user);
    }
}
