namespace SocialMediaAppAPI.Models
{
    public class Likes
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid PostId { get; set; }
        public Post Post { get; set; }

        public DateTime LikedAt { get; set; }
    }
}
