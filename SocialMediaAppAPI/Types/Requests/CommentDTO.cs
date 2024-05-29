using SocialMediaAppAPI.Models;

namespace SocialMediaAppAPI.Types.Requests
{
    public class CommentDTO
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; }
        public DateTime CommentedAt { get; set; }
    }
}
