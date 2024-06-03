namespace SocialMediaAppAPI.Types.Requests
{
    public class CreateCommentDTO
    {
        public Guid PostId { get; set; }
        public string Content { get; set; }
    }
}
