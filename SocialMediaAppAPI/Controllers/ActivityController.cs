using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Attributes;
using SocialMediaAppAPI.Types.Requests;

namespace SocialMediaAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateApiToken]
    public class ActivityController(APIDbContext _dbContext) : ControllerBase
    {
        private User GetAuthenticatedUser()
        {
            return HttpContext.Items["AuthenticatedUser"] as User;
        }

        [HttpGet]
        public async Task<List<ActivityDTO>> GetActivities()
        {
            User user = GetAuthenticatedUser();
            return await _dbContext.Activities.Select(a => new ActivityDTO
            {
                UserId = a.UserId,
                CreatedAt = a.CreatedAt,
                Content = a.Content,
                Type = a.Type,
            }).Where(a => a.UserId == user.Id).OrderByDescending(u => u.CreatedAt).ToListAsync();
        }
    }
}
