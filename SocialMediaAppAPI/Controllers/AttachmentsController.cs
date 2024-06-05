using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Models;
using SocialMediaAppAPI.Types.Requests.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        private readonly APIDbContext _context;

        public AttachmentsController(APIDbContext context)
        {
            _context = context;
        }

        // GET: api/Attachments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttachmentsDTO>>> GetAttachments()
        {
            return await _context.Attachments
                .Select(a => new AttachmentsDTO
                {
                    Id = a.Id,
                    PostId = a.PostId,
                    Base64String = a.Base64String,
                    ContentType = a.ContentType,
                    FileExtension = a.FileExtension
                })
                .ToListAsync();
        }

        // PUT: api/Attachments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttachments(Guid id, AttachmentsDTO attachmentsDTO)
        {
            if (id != attachmentsDTO.Id)
            {
                return BadRequest();
            }

            var attachments = await _context.Attachments.FindAsync(id);
            if (attachments == null)
            {
                return NotFound();
            }

            attachments.PostId = attachmentsDTO.PostId;
            attachments.Base64String = attachmentsDTO.Base64String;
            attachments.ContentType = attachmentsDTO.ContentType;
            attachments.FileExtension = attachmentsDTO.FileExtension;

            _context.Entry(attachments).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttachmentsExists(id))
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

        // POST: api/Attachments
        [HttpPost]
        public async Task<ActionResult<AttachmentsDTO>> PostAttachments(CreateAttachmentsDTO createAttachmentsDTO)
        {
            var attachments = new Attachment
            {
                Id = Guid.NewGuid(),
                PostId = createAttachmentsDTO.PostId,
                Base64String = createAttachmentsDTO.Base64String,
                ContentType = createAttachmentsDTO.ContentType,
                FileExtension = createAttachmentsDTO.FileExtension
            };

            _context.Attachments.Add(attachments);
            await _context.SaveChangesAsync();

            var attachmentsDTO = new AttachmentsDTO
            {
                Id = attachments.Id,
                PostId = attachments.PostId,
                Base64String = attachments.Base64String,
                ContentType = attachments.ContentType,
                FileExtension = attachments.FileExtension
            };

            return CreatedAtAction("GetAttachments", new { id = attachmentsDTO.Id }, attachmentsDTO);
        }

        // DELETE: api/Attachments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AttachmentsDTO>> DeleteAttachments(Guid id)
        {
            var attachments = await _context.Attachments.FindAsync(id);
            if (attachments == null)
            {
                return NotFound();
            }

            _context.Attachments.Remove(attachments);
            await _context.SaveChangesAsync();

            var attachmentsDTO = new AttachmentsDTO
            {
                Id = attachments.Id,
                PostId = attachments.PostId,
                Base64String = attachments.Base64String,
                ContentType = attachments.ContentType,
                FileExtension = attachments.FileExtension
            };

            return attachmentsDTO;
        }

        private bool AttachmentsExists(Guid id)
        {
            return _context.Attachments.Any(e => e.Id == id);
        }
    }
}
