using ClassLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Services;
using SocialMediaAppAPI.Types.Attributes;
using SocialMediaAppAPI.Types.Requests.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
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

            return await _context.Users
                .Select(user => new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    CreatedAt = user.CreatedAt,
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
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                CreatedAt = user.CreatedAt,
                Email = user.Email,
                UserName = user.UserName,
                FollowCount = user.FollowCount,
                FollowingCount = user.FollowingCount,
                IsFollowing = _context.Followers.Any(f => f.UserId == authenticatedUser.Id && f.FollowedUserId == user.Id)
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
                    CreatedAt = user.CreatedAt,
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
            var errors = new List<string>();

            // Email validation regex pattern
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(createUserDto.Email, emailRegex))
            {
                // If the email is not in a valid format, add to errors
                errors.Add("Invalid email format");
            }

            // Username validation: convert to lowercase, remove spaces, and check for allowed characters
            var username = createUserDto.UserName.ToLower();
            var usernameRegex = @"^[a-z0-9._-]+$";

            if (username.Contains(" ") || !Regex.IsMatch(username, usernameRegex))
            {
                // If the username is invalid, add to errors
                errors.Add("Invalid username. Only lowercase letters, numbers, and the special characters '_', '.', '-' are allowed, and no spaces.");
            }

            // Check if the email already exists in the database
            var existingUserWithSameEmail = _context.Users.FirstOrDefault(u => u.Email == createUserDto.Email);

            if (existingUserWithSameEmail != null)
            {
                // If the email already exists, add to errors
                errors.Add("Email already exists");
            }

            // Check if the username already exists in the database
            var existingUserWithSameUsername = _context.Users.FirstOrDefault(u => u.UserName == username);

            if (existingUserWithSameUsername != null)
            {
                // If the username already exists, add to errors
                errors.Add("Username already exists");
            }

            // If there are any errors, return them
            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = PasswordHasher.HashPassword(createUserDto.Password),
                UserName = username,
                FollowCount = 0,
                FollowingCount = 0,
                CreatedAt = DateTime.UtcNow,
                ApiToken = GenerateString.Generate()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ActivityService activityService = new ActivityService(_context);
            Activity activity = await activityService.CreateActivity(user, Types.Enum.ActivityEnum.Liked, $"Nieuw account geregistreerd: @{user.UserName}.");

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
