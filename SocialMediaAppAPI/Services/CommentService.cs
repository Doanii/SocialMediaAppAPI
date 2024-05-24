using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Interfaces;

namespace SocialMediaAppAPI.Services
{
    public class CommentService(APIDbContext context) : ICommentService
    {
        public void Add(Comments item)
        {
            context.Comments.Add(item);
        }

        public void Delete(Comments item)
        {
            context.Comments.Remove(item);
        }

        public Comments GetItemById(Guid id)
        {
            return context.Comments.Find(id);    
        }

        public IQueryable<Comments>? GetItems(int page, int amount, Guid postId)
        {
            throw new NotImplementedException();
        }

        public void Update(Comments item)
        {
            throw new NotImplementedException();
        }
    }
}
