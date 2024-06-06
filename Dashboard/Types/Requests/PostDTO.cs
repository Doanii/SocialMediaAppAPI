using System.Net.Mail;

namespace Dashboard.Types.Requests
{
    public class PostDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string OPUsername { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
    }
}
