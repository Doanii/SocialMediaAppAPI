using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Attributes;

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
        public async Task<ActionResult<IEnumerable<Followers>>> GetFollowers()
        {
            return await _context.Followers.ToListAsync();
        }

        // GET: api/Followers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Followers>> GetFollowers(Guid id)
        {
            var followers = await _context.Followers.FindAsync(id);

            if (followers == null)
            {
                return NotFound();
            }

            return followers;
        }

        // PUT: api/Followers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFollowers(Guid id, Followers followers)
        {
            if (id != followers.UserId)
            {
                return BadRequest();
            }

            _context.Entry(followers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FollowersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Followers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Followers>> PostFollowers(Followers followers)
        {
            _context.Followers.Add(followers);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FollowersExists(followers.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFollowers", new { id = followers.UserId }, followers);
        }

        // DELETE: api/Followers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFollowers(Guid id)
        {
            var followers = await _context.Followers.FindAsync(id);
            if (followers == null)
            {
                return NotFound();
            }

            _context.Followers.Remove(followers);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FollowersExists(Guid id)
        {
            return _context.Followers.Any(e => e.UserId == id);
        }
    }
}
