using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Models;

namespace SocialMediaAppAPI.Data
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Followers> Followers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Attachments> Attachments { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Followers>()
                .HasKey(f => new { f.UserId, f.FollowedUserId });

            modelBuilder.Entity<Followers>()
                .HasOne(f => f.User)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Followers>()
                .HasOne(f => f.FollowedUser)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Likes>()
                .HasKey(l => new { l.UserId, l.PostId });

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId);

            modelBuilder.Entity<Comments>()
                .HasKey(c => new { c.UserId, c.PostId });

            modelBuilder.Entity<Comments>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Comments>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);

            modelBuilder.Entity<Attachments>()
                .HasOne(a => a.Post)
                .WithMany(p => p.Attachments)
                .HasForeignKey(a => a.PostId);

            modelBuilder.Entity<Activity>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);
        }
    }
}
