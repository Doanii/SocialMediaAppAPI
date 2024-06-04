using Dashboard.Data;
using Dashboard.Data.Requests;
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
            List<Post> posts = await dbContext.Posts
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
