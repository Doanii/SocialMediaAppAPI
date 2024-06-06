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
            var topUsers = await dbContext.Activities
                .GroupBy(a => a.UserId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(5) 
                .ToListAsync();

            if (topUsers.Any())
            {
                var topUserIds = topUsers.Select(u => u.UserId).ToList(); 

                var userInfos = await dbContext.Users
                    .Where(u => topUserIds.Contains(u.Id)) 
                    .Select(u => new { u.Id, u.UserName })  
                    .ToListAsync(); 

                var topUsersWithNames = topUsers
                    .Join(userInfos,
                          activity => activity.UserId, 
                          user => user.Id, 
                          (activity, user) => new  
                          {
                              UserId = activity.UserId,
                              UserName = user.UserName,
                              ActivityCount = activity.Count
                          })
                    .ToList(); 

                await Clients.All.SendAsync("MostPopularUsers", topUsersWithNames);
            }
            else
            {
                await Clients.All.SendAsync("MostPopularUsers", new List<object>());
            }
        }

        public async Task DisplayActivities()
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DashboardDbContext>();

            var activities = await dbContext.Activities
                                            .OrderByDescending(a => a.CreatedAt)
                                            .Take(10)
                                            .ToListAsync();
            await Clients.All.SendAsync("DisplayActivities", activities);
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


        public async Task<List<UserJoins>> UserJoinsPerDay()
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DashboardDbContext>();
            var list = await dbContext.Users.GroupBy((user) => user.CreatedAt.Date)
                .Select((g) => new UserJoins(g.Key, g.Count())).ToListAsync();
            await Clients.All.SendAsync("UserJoinsPerDay", list);
            return list;
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
