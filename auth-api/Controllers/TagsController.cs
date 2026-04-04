using auth_api.Data;
using auth_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace auth_api.Controllers
{
    [Route("cms/tags")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class TagsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TagsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _db.Tags.ToListAsync();
            return Ok(tags);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] Tag tag)
        {
            if (string.IsNullOrWhiteSpace(tag.Name)) return BadRequest("Tag name required");

            if (await _db.Tags.AnyAsync(t => t.Name.ToLower() == tag.Name.ToLower()))
                return Conflict("Tag already exists");

            _db.Tags.Add(tag);
            await _db.SaveChangesAsync();
            return Ok(tag);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _db.Tags.FindAsync(id);
            if (tag == null) return NotFound();
            _db.Tags.Remove(tag);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}