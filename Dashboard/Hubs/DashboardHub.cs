using Dashboard.Data;
using Dashboard.Data.Requests;
using Dashboard.Types.Requests;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Models;
using System.Runtime.InteropServices;

namespace Dashboard.Hubs
{
    public class DashboardHub(DashboardDbContext dbContext) : Hub
    {

        public async Task NewPostReceived()
        {
            var posts = await dbContext.Posts
                .Join(dbContext.Users,
                      post => post.UserId,
                      user => user.Id,
                      (post, user) => new PostDTO
                      {
                          Id = post.Id,
                          Content = post.Content,
                          OPUsername = user.UserName,
                          LikeCount = post.LikeCount,
                          CommentCount = post.CommentCount,
                          CreatedAt = post.CreatedAt,
                          UserId = post.UserId,
                      })
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .ToListAsync();

            await Clients.All.SendAsync("NewPostReceived", posts);
        }


        public async Task<int> TotalPosts()
        {
            int count = await dbContext.Posts.Select(p => new CountDTO { Id = p.Id }).CountAsync();
            await Clients.All.SendAsync("TotalPosts", count);

            return count;
        }

        public async Task<int> NewPostsToday()
        {
            var today = DateTime.Today;
            int count = await dbContext.Posts
                                       .Where(p => p.CreatedAt >= today && p.CreatedAt < today.AddDays(1))
                                       .CountAsync();
            await Clients.All.SendAsync("NewPostsToday", count);
            return count;
        }

        public async Task<int> UserCount()
        {
            int usercount = await dbContext.Users.GroupBy(p => new CountDTO { Id = p.Id }).CountAsync();
            await Clients.All.SendAsync("UserCount", usercount);
            return usercount;
        }
    }
}
