using ClassLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Attributes;
using SocialMediaAppAPI.Types.Requests.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SocialMediaAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly APIDbContext _context;

        public UsersController(APIDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        [ValidateApiToken]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }
            //_context.DropAllTables();

            return await _context.Users
                .Select(user => new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    UserName = user.UserName,
                    FollowCount = user.FollowCount,
                    FollowingCount = user.FollowingCount,
                    IsFollowing = _context.Followers.Any(f => f.UserId == authenticatedUser.Id && f.FollowedUserId == user.Id)
                })
                .ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [ValidateApiToken]
        public async Task<ActionResult<UserDTO>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                FollowCount = user.FollowCount,
                FollowingCount = user.FollowingCount,
            };

            return userDto;
        }

        // GET: api/CurrentUser
        [HttpGet("/api/CurrentUser")]
        [ValidateApiToken]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetCurrentUser()
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            return await _context.Users
                .Select(user => new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    UserName = user.UserName,
                    FollowCount = user.FollowCount,
                    FollowingCount = user.FollowingCount,
                    IsFollowing = _context.Followers.Any(f => f.UserId == authenticatedUser.Id && f.FollowedUserId == user.Id)
                })
                .Where(user => user.Id == authenticatedUser.Id)
                .ToListAsync();
        }

        // POST: api/Register
        [HttpPost("/api/Register")]
        public async Task<ActionResult<UserDTO>> Register(CreateUserDTO createUserDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = PasswordHasher.HashPassword(createUserDto.Password),
                UserName = createUserDto.UserName,
                FollowCount = 0,
                FollowingCount = 0,
                ApiToken = GenerateString.Generate()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Register", new { id = user.Id }, user.ApiToken);
        }

        // POST: api/Login
        [HttpPost("/api/Login")]
        public async Task<ActionResult<string>> Login(LoginDTO loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.Password))
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(user.ApiToken);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, CreateUserDTO createUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            user.Name = createUserDto.Name;
            user.Email = createUserDto.Email;
            user.Password = PasswordHasher.HashPassword(createUserDto.Password);
            user.UserName = createUserDto.UserName;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private User? GetAuthenticatedUser()
        {
            return HttpContext.Items["AuthenticatedUser"] as User;
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
