using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Enum;
using SocialMediaAppAPI.Types.Interfaces;
using System;
using System.Threading.Tasks;

namespace SocialMediaAppAPI.Services
{
    public class ActivityService : IActivityService
    {
        private readonly APIDbContext _dbContext;

        public ActivityService(APIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Activity> CreateActivity(User user, ActivityEnum type, string content)
        {
            Activity activity = new()
            {
                Id = Guid.NewGuid(),
                Content = content,
                Type = type,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Activities.Add(activity);
            await _dbContext.SaveChangesAsync();

            return activity;
        }
    }
}
