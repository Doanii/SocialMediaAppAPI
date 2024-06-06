namespace SocialMediaAppAPI.Types.Requests.Users
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public int FollowCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowing { get; set; }
    }
}
