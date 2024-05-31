﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
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
        public async Task<ActionResult<FollowerDTO>> PostFollowers(Guid followedUserId)
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            // Create the new follower relationship
            Followers followers = new Followers
            {
                UserId = authenticatedUser.Id,
                FollowedUserId = followedUserId,
            };

            _context.Followers.Add(followers);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FollowerExists(followers.UserId, followers.FollowedUserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Retrieve the followed user
            var followedUser = await _context.Users.FindAsync(followedUserId);
            if (followedUser == null)
            {
                return NotFound();
            }

            var followedUserDto = new UserDTO
            {
                Id = followedUser.Id,
                Name = followedUser.Name,
                Email = followedUser.Email,
                UserName = followedUser.UserName,
                FollowCount = followedUser.FollowCount
            };

            // Return the followed user
            return CreatedAtAction("GetFollowers", new { id = followers.FollowedUserId }, followedUserDto);
        }

        // DELETE: api/Followers/5
        [HttpDelete("{followedUserId}")]
        public async Task<IActionResult> DeleteFollowers(Guid followedUserId)
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            var follower = await _context.Followers
                .FirstOrDefaultAsync(f => f.UserId == authenticatedUser.Id && f.FollowedUserId == followedUserId);

            if (follower == null)
            {
                return NotFound();
            }

            _context.Followers.Remove(follower);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private User? GetAuthenticatedUser()
        {
            return HttpContext.Items["AuthenticatedUser"] as User;
        }

        private bool FollowerExists(Guid userId, Guid followedUserId)
        {
            return _context.Followers.Any(e => e.UserId == userId && e.FollowedUserId == followedUserId);
        }
    }
}
