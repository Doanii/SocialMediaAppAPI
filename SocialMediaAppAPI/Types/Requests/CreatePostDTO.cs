using SocialMediaAppAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaAppAPI.Types.Requests
{
    public class CreatePostDTO
    {
        public string Content { get; set; }
        public ICollection<Attachments>? Attachments { get; set; }
    }
}
