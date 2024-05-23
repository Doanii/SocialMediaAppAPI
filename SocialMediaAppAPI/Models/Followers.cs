namespace SocialMediaAppAPI.Models
{
    public class Followers
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid FollowedUserId { get; set; }
        public User FollowedUser { get; set; }

        public DateTime FollowedAt { get; set; }
    }
}
