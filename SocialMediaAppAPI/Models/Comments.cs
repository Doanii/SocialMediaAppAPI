namespace SocialMediaAppAPI.Models
{
    public class Comments
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid PostId { get; set; }
        public Post Post { get; set; }

        public string Content { get; set; }
        public DateTime CommentedAt { get; set; }
    }
}
