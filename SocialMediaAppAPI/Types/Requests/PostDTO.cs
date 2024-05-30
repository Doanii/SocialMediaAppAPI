using SocialMediaAppAPI.Models;

namespace SocialMediaAppAPI.Types.Requests
{
    public class PostDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public ICollection<Attachments> Attachments { get; set; }
    }
}
