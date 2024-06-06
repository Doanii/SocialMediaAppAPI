using Dashboard.Types.Enum;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaAppAPI.Models
{
    public class Activity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; }

        public ActivityEnum Type { get; set; }
        public string Content { get; set; }
    }
}