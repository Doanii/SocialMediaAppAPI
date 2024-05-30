namespace SocialMediaAppAPI.Types.Requests
{
    public class LikeDTO
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}
