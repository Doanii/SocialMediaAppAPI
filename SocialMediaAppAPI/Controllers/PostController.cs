using Microsoft.AspNetCore.Mvc;
using SocialMediaAppAPI.Services;

namespace SocialMediaAppAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController(PostService postService) : Controller
    {
        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            return Ok();
        }
    }
}
