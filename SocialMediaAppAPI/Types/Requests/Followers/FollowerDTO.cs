namespace SocialMediaAppAPI.Types.Requests.Followers
{
    public class FollowerDTO
    {
        public Guid UserId { get; set; }
        public Guid FollowedUserId { get; set; }
    }
}
