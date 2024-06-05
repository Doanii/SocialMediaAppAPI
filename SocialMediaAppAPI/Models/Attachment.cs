using System.ComponentModel.DataAnnotations;

namespace SocialMediaAppAPI.Models
{
    public class Attachment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PostId { get; set; }
        public Post? Post { get; set; }

        public string Base64String { get; set; }
        public string ContentType { get; set; }
        public string? FileExtension { get; set; } = string.Empty;
    }
}
