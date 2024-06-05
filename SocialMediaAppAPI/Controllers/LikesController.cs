using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Attributes;

namespace SocialMediaAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateApiToken]
    public class LikesController : ControllerBase
    {
        private readonly APIDbContext _context;

        public LikesController(APIDbContext context)
        {
            _context = context;
        }

        private User GetAuthenticatedUser()
        {
            return HttpContext.Items["AuthenticatedUser"] as User;
        }

        // GET: api/Likes/00000-00000-00000-00000
        [HttpGet("{postId}")]
        public async Task<ActionResult<List<Likes>>> GetLikes(Guid postId)
        {
            return await _context.Likes.Where(l => l.PostId == postId).ToListAsync() ?? [];
        }

        // POST: api/Likes/00000-00000-00000-00000
        [HttpPost("{postId}")]
        public async Task<ActionResult<bool>> LikePost(Guid postId)
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            Likes like = await _context.Likes
                .Where(l => l.PostId == postId && l.UserId == authenticatedUser.Id)
                .FirstOrDefaultAsync();
            Post post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (like == null)
            {
                Likes newLike = new()
                {
                    PostId = postId,
                    UserId = authenticatedUser.Id,
                    LikedAt = DateTime.UtcNow,
                };

                post.LikeCount++;
                _context.Likes.Add(newLike);
                await _context.SaveChangesAsync();

                return true;
            }

            post.LikeCount--;
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return false;
        }
    }
}
