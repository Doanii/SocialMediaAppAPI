using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMediaAppAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;
        public int FollowCount { get; set; } = 0;
        public int FollowingCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; }

        [Required]
        public string ApiToken { get; set; }

        public ICollection<Followers>? Followers { get; set; }
        public ICollection<Followers>? Following { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Likes>? Likes { get; set; }
        public ICollection<Comments>? Comments { get; set; }
    }
}
