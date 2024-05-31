using Dashboard.Data;
using Dashboard.Data.Requests;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Models;

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

        public async Task TotalPosts()
        {
            int count = await dbContext.Posts.Select(p => new CountDTO { Id = p.Id }).CountAsync();
            await Clients.All.SendAsync("TotalPosts", count);
        }

        public async Task NewPostsToday()
        {
            var today = DateTime.Today;
            int count = await dbContext.Posts
                                       .Where(p => p.CreatedAt >= today && p.CreatedAt < today.AddDays(1))
                                       .CountAsync();
            await Clients.All.SendAsync("NewPostsToday", count);
        }
    }
}
