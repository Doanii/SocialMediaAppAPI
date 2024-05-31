using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Attributes;
using SocialMediaAppAPI.Types.Requests;
using Dashboard.Hubs;

namespace SocialMediaAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateApiToken]
    public class PostsController : ControllerBase
    {
        private readonly APIDbContext _context;

        public PostsController(APIDbContext context)
        {
            _context = context;
        }

        private User GetAuthenticatedUser()
        {
            return HttpContext.Items["AuthenticatedUser"] as User;
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDTO>> GetPost(Guid id)
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            var post = await _context.Posts
                .Where(c => c.Id == id)
                .Select(post => new PostDTO
                {
                    Id = post.Id,
                    Content = post.Content,
                    OPUsername = _context.Users.Where(u => u.Id == post.UserId).Select(u => u.UserName).FirstOrDefault(),
                    Following = _context.Followers.Any(f => f.UserId == authenticatedUser.Id && f.FollowedUserId == post.UserId),
                    LikeCount = post.LikeCount,
                    CommentCount = post.CommentCount,
                    CreatedAt = post.CreatedAt,
                    UserId = post.UserId,
                    Attachments = post.Attachments
                })
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        // GET: api/Feed/Following/{page}/{amount}
        [HttpGet("/api/Feed/Following/{page}/{amount}")]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetFollowingFeed(int page, int amount)
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            if (page == 0)
                page = 1;

            if (amount == 0)
                amount = int.MaxValue;

            var skip = (page - 1) * amount;

            var followedUserIds = await _context.Followers
                .Where(f => f.UserId == authenticatedUser.Id)
                .Select(f => f.FollowedUserId)
                .ToListAsync();

            var followedPosts = _context.Posts
                .Where(p => followedUserIds.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedAt);

            var paginatedPosts = await followedPosts
                .Skip(skip)
                .Take(amount)
                .Select(post => new PostDTO
                {
                    Id = post.Id,
                    Content = post.Content,
                    OPUsername = _context.Users.Where(u => u.Id == post.UserId).Select(u => u.UserName).FirstOrDefault(),
                    Following = followedUserIds.Contains(post.UserId),
                    LikeCount = post.LikeCount,
                    CommentCount = post.CommentCount,
                    CreatedAt = post.CreatedAt,
                    UserId = post.UserId,
                    Attachments = post.Attachments
                })
                .ToListAsync();

            return Ok(paginatedPosts);
        }

        [HttpGet("/api/Feed/NotFollowing/{page}/{amount}")]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetNotFollowingFeed(int page, int amount)
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            if (page == 0)
                page = 1;

            if (amount == 0)
                amount = int.MaxValue;

            var skip = (page - 1) * amount;

            var followedUserIds = await _context.Followers
                .Where(f => f.UserId == authenticatedUser.Id)
                .Select(f => f.FollowedUserId)
                .ToListAsync();

            var notFollowedPosts = _context.Posts
                .Where(p => !followedUserIds.Contains(p.UserId) && p.UserId != authenticatedUser.Id)
                .OrderByDescending(p => p.CreatedAt);

            var paginatedPosts = await notFollowedPosts
                .Skip(skip)
                .Take(amount)
                .Select(post => new PostDTO
                {
                    Id = post.Id,
                    Content = post.Content,
                    OPUsername = _context.Users.Where(u => u.Id == post.UserId).Select(u => u.UserName).FirstOrDefault(),
                    Following = followedUserIds.Contains(post.UserId),
                    LikeCount = post.LikeCount,
                    CommentCount = post.CommentCount,
                    CreatedAt = post.CreatedAt,
                    UserId = post.UserId,
                    Attachments = post.Attachments
                })
                .ToListAsync();

            return Ok(paginatedPosts);
        }

        // GET: api/Posts/1/10
        [HttpGet("{page}/{amount}")]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetPosts(int page, int amount)
        {
            var authenticatedUser = HttpContext.Items["AuthenticatedUser"] as User;
            if (authenticatedUser == null)
            {
                return BadRequest();
                return Unauthorized();
            }

            if (page == 0)
                page = 1;

            if (amount == 0)
                amount = int.MaxValue;

            var skip = (page - 1) * amount;

            var posts = await _context.Posts
                .Select(post => new PostDTO
                {
                    Id = post.Id,
                    Content = post.Content,
                    OPUsername = _context.Users.Where(u => u.Id == post.UserId).Select(u => u.UserName).FirstOrDefault(),
                    Following = _context.Followers.Any(f => f.UserId == authenticatedUser.Id && f.FollowedUserId == post.UserId),
                    LikeCount = post.LikeCount,
                    CommentCount = post.CommentCount,
                    CreatedAt = post.CreatedAt,
                    UserId = post.UserId,
                    Attachments = post.Attachments
                })
                .OrderByDescending(p => p.CreatedAt)
                .Skip(skip)
                .Take(amount)
                .ToListAsync();

            return Ok(posts);
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(Guid id, UpdatePostDTO post)
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            var existingPost = await _context.Posts
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (existingPost == null)
            {
                return NotFound();
            }

            existingPost.Content = post.Content;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(CreatePostDTO post)
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            var createdPost = new Post
            {
                Id = Guid.NewGuid(),
                UserId = authenticatedUser.Id,
                Content = post.Content,
                LikeCount = 0,
                CommentCount = 0,
                CreatedAt = DateTime.Now
            };

            _context.Posts.Add(createdPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = createdPost.Id }, createdPost);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            var post = await _context.Posts
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            if (post == null)
            {
                return NotFound();
            }

            if (post.UserId != authenticatedUser.Id)
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
