using System.ComponentModel.DataAnnotations;

namespace SocialMediaAppAPI.Models
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; }

        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
        public ICollection<Likes> Likes { get; set; }
        public ICollection<Comments> Comments { get; set; }
    }
}
