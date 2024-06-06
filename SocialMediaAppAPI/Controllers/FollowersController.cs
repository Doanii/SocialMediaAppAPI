using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Services;
using SocialMediaAppAPI.Types.Attributes;
using SocialMediaAppAPI.Types.Requests.Followers;
using SocialMediaAppAPI.Types.Requests.Users;

namespace SocialMediaAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateApiToken]
    public class FollowersController : ControllerBase
    {
        private readonly APIDbContext _context;

        public FollowersController(APIDbContext context)
        {
            _context = context;
        }

        // GET: api/Followers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FollowerDTO>>> GetFollowers()
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            return await _context.Followers
                .Where(f => f.FollowedUserId == authenticatedUser.Id)
                .Select(f => new FollowerDTO
                {
                    UserId = f.UserId,
                    FollowedUserId = f.FollowedUserId,
                })
                .ToListAsync();
        }

        // GET: api/Following
        [HttpGet("/api/Following")]
        public async Task<ActionResult<IEnumerable<FollowerDTO>>> GetFollowing()
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            return await _context.Followers
                .Where(f => f.UserId == authenticatedUser.Id)
                .Select(f => new FollowerDTO
                {
                    UserId = f.UserId,
                    FollowedUserId = f.FollowedUserId,
                })
                .ToListAsync();
        }

        // POST: api/Followers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{followedUserId}")]
        public async Task<ActionResult<UserDTO>> PostFollowers(Guid followedUserId)
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            // Check if the follow relationship already exists

            var existingFollower = await _context.Followers
                .FirstOrDefaultAsync(f => f.UserId == authenticatedUser.Id && f.FollowedUserId == followedUserId);

            if (existingFollower != null)
            {
                // Relationship exists, so remove it (unfollow)
                _context.Followers.Remove(existingFollower);
            }
            else
            {
                // Relationship does not exist, so create it (follow)
                Followers followers = new Followers
                {
                    UserId = authenticatedUser.Id,
                    FollowedUserId = followedUserId,
                    FollowedAt = DateTime.UtcNow // Assuming you want to track the follow date
                };

                _context.Followers.Add(followers);
            }

            // Retrieve the followed user
            var followedUser = await _context.Users.FindAsync(followedUserId);
            var currentUser = await _context.Users.FirstAsync(u => u.Id == authenticatedUser.Id);
            if (followedUser == null)
            {
                return NotFound();
            }

            if (existingFollower != null)
            {
                followedUser.FollowCount--;
                currentUser.FollowingCount--;

                ActivityService activityService = new ActivityService(_context);
                Activity activity = await activityService.CreateActivity(authenticatedUser, Types.Enum.ActivityEnum.Followed, $"@{authenticatedUser.UserName} heeft @{followedUser.UserName} ontvolgd.");
            }
            else
            {
                followedUser.FollowCount++;
                currentUser.FollowingCount++;

                ActivityService activityService = new ActivityService(_context);
                Activity activity = await activityService.CreateActivity(authenticatedUser, Types.Enum.ActivityEnum.Followed, $"@{authenticatedUser.UserName} volgt nu @{followedUser.UserName}.");
            }

            await _context.SaveChangesAsync();


            var followedUserDto = new UserDTO
            {
                Id = followedUser.Id,
                Name = followedUser.Name,
                Email = followedUser.Email,
                UserName = followedUser.UserName,
                FollowCount = followedUser.FollowCount,
                IsFollowing = existingFollower == null
            };

            // Return the followed user
            return CreatedAtAction("GetFollowers", followedUserDto);
        }

        private User? GetAuthenticatedUser()
        {
            return HttpContext.Items["AuthenticatedUser"] as User;
        }
    }
}
