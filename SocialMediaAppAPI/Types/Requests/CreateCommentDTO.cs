namespace SocialMediaAppAPI.Types.Requests
{
    public class CreateCommentDTO
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; }
    }
}
