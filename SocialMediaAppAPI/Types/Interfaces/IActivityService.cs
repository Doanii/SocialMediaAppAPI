using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Enum;

namespace SocialMediaAppAPI.Types.Interfaces
{
    public interface IActivityService
    {
        Task<Activity> CreateActivity(User user, ActivityEnum type, string content);
    }
}
