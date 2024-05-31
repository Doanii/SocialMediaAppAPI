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

namespace SocialMediaAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly APIDbContext _context;

        public LikesController(APIDbContext context)
        {
            _context = context;
        }

        // GET: api/Likes/00000-00000-00000-00000
        [HttpGet("{postId}")]
        public async Task<ActionResult<List<Likes>>> GetLikes(Guid postId)
        {
            return await _context.Likes.Where(l => l.PostId == postId).ToListAsync() ?? [];
        }

        // POST: api/Likes/00000-00000-00000-00000
        [HttpPost("{postId}")]
        public async Task<ActionResult<bool>> LikePost(Guid postId, Guid userId)
        {
            Likes like = await _context.Likes.Where(l => l.PostId == postId).FirstAsync();

            if (like == null)
            {
                Likes newLike = new()
                {
                    PostId = postId,
                    UserId = userId,
                    LikedAt = DateTime.Now,
                };

                _context.Likes.Add(newLike);
                await _context.SaveChangesAsync();

                return true;
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return false;
        }
    }
}
