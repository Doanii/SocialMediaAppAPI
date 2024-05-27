using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Enum;
using System;
using System.Threading.Tasks;

namespace SocialMediaAppAPI.Services
{
    public class ActivityService
    {
        private readonly APIDbContext _dbContext;

        public ActivityService(APIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> CreateActivity(User user, ActivityEnum type, string content)
        {
            Activity activity = new Activity
            {
                Id = Guid.NewGuid(),
                Content = content,
                Type = type,
                User = user,
                UserId = user.Id
            };

            _dbContext.Activities.Add(activity);
            await _dbContext.SaveChangesAsync();

            return "Activity Generated!";
        }
    }
}
