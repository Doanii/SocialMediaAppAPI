using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class CommentsController : ControllerBase
    {
        private readonly APIDbContext _context;

        public CommentsController(APIDbContext context)
        {
            _context = context;
        }

        private User GetAuthenticatedUser()
        {
            return HttpContext.Items["AuthenticatedUser"] as User;
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comments>> GetComments(Guid id)
        {
            var comment = await _context.Comments
                                        .Where(c => c.CommentId == id)
                                        .FirstOrDefaultAsync();

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        // GET: api/Comments/5
        [HttpGet("{page}/{amount}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetComments(int page, int amount)
        {
            if (page == 0)
                page = 1;

            if (amount == 0)
                amount = int.MaxValue;

            var skip = (page - 1) * amount;

            return await _context.Comments.Select(comment => new CommentDTO
            {
                UserId = comment.UserId,
                PostId = comment.PostId,
                Content = comment.Content,
                CommentedAt = comment.CommentedAt,
            })
            .Skip(skip)
            .Take(amount)
            .ToListAsync();
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComments(Guid id, EditCommentDTO updatedComment)
        {
            var existingComment = await _context.Comments
                                        .Where(c => c.CommentId == id)
                                        .FirstOrDefaultAsync();

            if (existingComment == null)
            {
                return NotFound();
            }

            // Only allow updating the content of the comment, not the UserId
            existingComment.Content = updatedComment.Content;

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


        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comments>> PostComments(CreateCommentDTO comment)
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            var createdComment = new Comments
            {
                CommentId = Guid.NewGuid(),
                UserId = authenticatedUser.Id,
                PostId = comment.PostId,
                Content = comment.Content,
                CommentedAt = DateTime.Now
            };

            _context.Comments.Add(createdComment);
            await _context.SaveChangesAsync();

            // Assuming you have an action called "GetComment" that takes commentId as a parameter
            return CreatedAtAction(nameof(GetComments), new { id = createdComment.CommentId }, createdComment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComments(Guid id)
        {
            var comments = await _context.Comments
                                        .Where(c => c.CommentId == id)
                                        .FirstOrDefaultAsync();
            if (comments == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comments);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentsExists(Guid id)
        {
            return _context.Comments.Any(e => e.UserId == id);
        }
    }
}
