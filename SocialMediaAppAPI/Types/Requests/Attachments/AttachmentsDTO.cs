using System.ComponentModel.DataAnnotations;

namespace SocialMediaAppAPI.Types.Requests.Attachments
{
    public class AttachmentsDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid PostId { get; set; }
        [Required]
        public string Base64String { get; set; }
        [Required]
        public string ContentType { get; set; }
        public string? FileExtension { get; set; } = string.Empty;
    }
}
