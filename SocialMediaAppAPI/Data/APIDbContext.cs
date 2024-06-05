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
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

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
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comments>()
                .HasKey(c => new { c.UserId, c.PostId, c.CommentId });

            modelBuilder.Entity<Comments>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comments>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Post)
                .WithMany(p => p.Attachments)
                .HasForeignKey(a => a.PostId);

            modelBuilder.Entity<Activity>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<User>().ToTable("Users", tb => tb.HasTrigger("iwjdij"));
            modelBuilder.Entity<Followers>().ToTable("Followers", tb => tb.HasTrigger("iwjdij"));
            modelBuilder.Entity<Post>().ToTable("Posts", tb => tb.HasTrigger("iwjdij"));
            modelBuilder.Entity<Likes>().ToTable("Likes", tb => tb.HasTrigger("iwjdij"));
            modelBuilder.Entity<Comments>().ToTable("Comments", tb => tb.HasTrigger("iwjdij"));
            modelBuilder.Entity<Attachment>().ToTable("Attachments", tb => tb.HasTrigger("iwjdij"));
            modelBuilder.Entity<Activity>().ToTable("Activities", tb => tb.HasTrigger("iwjdij"));
        }

        public void DropAllTables()
        {
                var dropForeignKeysSql = @"
            DECLARE @sql NVARCHAR(MAX) = N'';
            SELECT @sql += 'ALTER TABLE ' + QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) 
                        + ' DROP CONSTRAINT ' + QUOTENAME(CONSTRAINT_NAME) + ';'
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
            WHERE CONSTRAINT_TYPE = 'FOREIGN KEY';

            EXEC sp_executesql @sql;
        ";

                var dropTablesSql = @"
            DECLARE @sql NVARCHAR(MAX) = N'';
            SELECT @sql += 'DROP TABLE ' + QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) + ';'
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE';

            EXEC sp_executesql @sql;
        ";

                // Drop foreign keys first
                this.Database.ExecuteSqlRaw(dropForeignKeysSql);

                // Then drop tables
                this.Database.ExecuteSqlRaw(dropTablesSql);
        }
    }
}
