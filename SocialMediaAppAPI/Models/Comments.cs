using SocialMediaAppAPI.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Comments
{
    [Key]
    public Guid CommentId { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; }

    [ForeignKey("Post")]
    public Guid PostId { get; set; }
    public Post Post { get; set; }

    public string Content { get; set; }
    public DateTime CommentedAt { get; set; }
}
