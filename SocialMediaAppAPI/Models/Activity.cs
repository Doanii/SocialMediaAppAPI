using SocialMediaAppAPI.Types.Enum;

namespace SocialMediaAppAPI.Models
{
    public class Activity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public ActivityEnum Type { get; set; }
        public string Content { get; set; }
    }
}
