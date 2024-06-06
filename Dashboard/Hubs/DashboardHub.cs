using Dashboard.Data;
using Dashboard.Data.Requests;
using Dashboard.Types.Requests;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Models;
using System.Runtime.InteropServices;

namespace Dashboard.Hubs
{
    public class DashboardHub(IServiceScopeFactory scopeFactory) : Hub
    {
        public async Task MostPopularUsers()
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DashboardDbContext>();

            Console.WriteLine($"Fakka van andere kant?, {DateTime.Now}");
            // Query to find the top 3 users with the most activities
            var topUsers = await dbContext.Activities
                .GroupBy(a => a.UserId)  // Group by UserId
                .Select(g => new { UserId = g.Key, Count = g.Count() })  // Select UserId and count of activities
                .OrderByDescending(g => g.Count)  // Order by count descending
                .Take(5)  // Take the top 3 results
                .ToListAsync();  // Convert to a list

            if (topUsers.Any())
            {
                var topUserIds = topUsers.Select(u => u.UserId).ToList();  // Extract UserIds from top users

                // Query to get the usernames for the top 3 users
                var userInfos = await dbContext.Users
                    .Where(u => topUserIds.Contains(u.Id))  // Filter users by the UserIds found in the top users query
                    .Select(u => new { u.Id, u.UserName })  // Select the UserId and UserName
                    .ToListAsync();  // Convert to a list

                // Combine the top user information with their usernames
                var topUsersWithNames = topUsers
                    .Join(userInfos,  // Join the top users with their corresponding usernames
                          activity => activity.UserId,  // Join on UserId from top users
                          user => user.Id,  // Join on Id from users
                          (activity, user) => new  // Create a new anonymous object with required information
                          {
                              UserId = activity.UserId,
                              UserName = user.UserName,
                              ActivityCount = activity.Count
                          })
                    .ToList();  // Convert to a list

                // Send the top 3 users' information to all clients
                await Clients.All.SendAsync("MostPopularUsers", topUsersWithNames);
            }
            else
            {
                // Handle the case when no user is found
                await Clients.All.SendAsync("MostPopularUsers", new List<object>());
            }
        }

        public async Task NewPostReceived()
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DashboardDbContext>();

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

        public async Task MostPopulairPosts()
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DashboardDbContext>();

            var posts = await dbContext.Posts
                .Join(dbContext.Users,
                    post => post.UserId,
                    user => user.Id,
                    (post, user) => new
                    {
                        Post = post,
                        User = user
                    })
                .Select(joined => new PopulairPostDTO
                {
                    Id = joined.Post.Id,
                    Content = joined.Post.Content,
                    OPUsername = joined.User.UserName,
                    LikeCount = joined.Post.LikeCount,
                    CommentCount = joined.Post.CommentCount,
                    CreatedAt = joined.Post.CreatedAt,
                    UserId = joined.Post.UserId,
                    ActivityScore = joined.Post.LikeCount + joined.Post.CommentCount
                })
                .OrderByDescending(p => p.ActivityScore)
                .Take(3)
                .ToListAsync();


            await Clients.All.SendAsync("MostPopulairPosts", posts);
        }


        public async Task<int> TotalPosts()
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DashboardDbContext>();

            int count = await dbContext.Posts.Select(p => new CountDTO { Id = p.Id }).CountAsync();
            await Clients.All.SendAsync("TotalPosts", count);

            return count;
        }

        public async Task<int> NewPostsToday()
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DashboardDbContext>();

            var today = DateTime.Today;
            int count = await dbContext.Posts
                                       .Where(p => p.CreatedAt >= today && p.CreatedAt < today.AddDays(1))
                                       .CountAsync();
            await Clients.All.SendAsync("NewPostsToday", count);
            return count;
        }

        public async Task<int> UserCount()
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DashboardDbContext>();

            int usercount = await dbContext.Users.GroupBy(p => new CountDTO { Id = p.Id }).CountAsync();
            await Clients.All.SendAsync("UserCount", usercount);
            return usercount;
        }
    }
}
