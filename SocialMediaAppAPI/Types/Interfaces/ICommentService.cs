using SocialMediaAppAPI.Models;

namespace SocialMediaAppAPI.Types.Interfaces
{
    public interface ICommentService
    {
        IQueryable<Comments>? GetItems(int page, int amount, Guid postId);

        Comments GetItemById(Guid id);

        void Add(Comments item);

        void Update(Comments item);

        void Delete(Comments item);
    }
}
