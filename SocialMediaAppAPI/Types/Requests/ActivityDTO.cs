using SocialMediaAppAPI.Types.Enum;

namespace SocialMediaAppAPI.Types.Requests
{
    public class ActivityDTO
    {
        public string Content {  get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ActivityEnum Type { get; set; }
    }
}
