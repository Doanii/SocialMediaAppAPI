using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Requests;

namespace SocialMediaAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly APIDbContext _context;

        public PostsController(APIDbContext context)
        {
            _context = context;
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPost(Guid id)
        {
            var post = await _context.Posts
                                        .Where(c => c.Id == id)
                                        .FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        // GET: api/Feed/{userId}/{page}/{amount}
        [HttpGet("/api/Feed/Following/{userId}/{page}/{amount}")]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetFollowingFeed(Guid userId, int page, int amount)
        {
            if (page == 0)
                page = 1;

            if (amount == 0)
                amount = int.MaxValue;

            var skip = (page - 1) * amount;

            // Get the list of users the current user is following
            var followedUserIds = await _context.Followers
                       .Where(f => f.UserId == userId)
                       .Select(f => f.FollowedUserId)
                       .ToListAsync();




            // Get posts from followed users
            var followedPosts = _context.Posts
                                         .Where(p => followedUserIds.Contains(p.UserId))
                                         .OrderByDescending(p => p.CreatedAt);

            // Get posts from non-followed users
            //var randomPosts = _context.Posts
            //                          .Where(p => !followedUserIds.Contains(p.UserId))
            //                          .OrderByDescending(p => p.CreatedAt);

            // Combine followed and random posts
            //var combinedPosts = followedPosts.Concat(randomPosts);

            // Apply pagination
            var paginatedPosts = await followedPosts
                                         .Skip(skip)
                                         .Take(amount)
                                         .ToListAsync();

            // Map to PostDTO
            var postDTOs = paginatedPosts.Select(post => new PostDTO
            {
                Id = post.Id,
                Content = post.Content,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId,
                Attachments = post.Attachments
            }).ToList();

            return Ok(postDTOs);
        }

        [HttpGet("/api/Feed/NotFollowing/{userId}/{page}/{amount}")]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetNotFollowingFeed(Guid userId, int page, int amount)
        {
            if (page == 0)
                page = 1;

            if (amount == 0)
                amount = int.MaxValue;

            var skip = (page - 1) * amount;

            // Get the list of users the current user is following
            var followedUserIds = await _context.Followers
                       .Where(f => f.UserId == userId)
                       .Select(f => f.FollowedUserId)
                       .ToListAsync();

            // Get posts from non-followed users
            var notFollowedPosts = _context.Posts
                                            .Where(p => !followedUserIds.Contains(p.UserId) && p.UserId != userId)
                                            .OrderByDescending(p => p.CreatedAt);

            // Apply pagination
            var paginatedPosts = await notFollowedPosts
                                         .Skip(skip)
                                         .Take(amount)
                                         .ToListAsync();

            // Map to PostDTO
            var postDTOs = paginatedPosts.Select(post => new PostDTO
            {
                Id = post.Id,
                Content = post.Content,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId,
                Attachments = post.Attachments
            }).ToList();

            return Ok(postDTOs);
        }

        // GET: api/Posts/1/10
        [HttpGet("{page}/{amount}")]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetPosts(int page, int amount)
        {
            if (page == 0)
                page = 1;

            if (amount == 0)
                amount = int.MaxValue;

            var skip = (page - 1) * amount;

            return await _context.Posts.Select(Post => new PostDTO
            {
                Id = Post.Id,
                Content = Post.Content,
                LikeCount = Post.LikeCount,
                CommentCount = Post.CommentCount,
                CreatedAt = Post.CreatedAt,
                UserId = Post.UserId,
                Attachments = Post.Attachments
            })
            .Skip(skip)
            .Take(amount)
            .ToListAsync();
        }


        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(Guid id, UpdatePostDTO post)
        {
            var existingPost = await _context.Posts
                                        .Where(c => c.Id == id)
                                        .FirstOrDefaultAsync();

            if (existingPost == null)
            {
                return NotFound();
            }

            // Only allow updating the content of the comment, not the UserId
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
            var createdPost = new Post
            {
                Id = Guid.NewGuid(),
                UserId = post.UserId,
                Content = post.Content,
                LikeCount = 0,
                CommentCount = 0,
                CreatedAt = DateTime.Now
            };

            _context.Posts.Add(createdPost);
            await _context.SaveChangesAsync();

            // Assuming you have an action called "GetPost" that takes id as a parameter
            return CreatedAtAction("GetPost", new { id = createdPost.Id }, createdPost);
        }


        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var post = await _context.Posts
                                        .Where(c => c.Id == id)
                                        .FirstOrDefaultAsync();
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
