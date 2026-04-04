using auth_api.Data;
using auth_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace auth_api.Controllers
{
    [Route("cms/images")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class ImagesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ImagesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            var images = await _db.Images.Include(i => i.ImageTags).ThenInclude(it => it.Tag).ToListAsync();
            return Ok(images);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile? file, [FromForm] string? url)
        {
            if (file == null && string.IsNullOrEmpty(url)) return BadRequest("File or URL required");

            var image = new Image();

            if (file != null)
            {
                var uploads = Path.Combine(_env.ContentRootPath, "Uploads");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                var filePath = Path.Combine(uploads, file.FileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                image.Url = $"/Uploads/{file.FileName}";
                image.FileName = file.FileName;
            }
            else if (!string.IsNullOrEmpty(url))
            {
                image.Url = url;
            }

            _db.Images.Add(image);
            await _db.SaveChangesAsync();
            return Ok(image);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _db.Images.FindAsync(id);
            if (image == null) return NotFound();
            _db.Images.Remove(image);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/tags")]
        public async Task<IActionResult> AddTags(int id, [FromBody] List<int> tagIds)
        {
            var image = await _db.Images.Include(i => i.ImageTags).FirstOrDefaultAsync(i => i.Id == id);
            if (image == null) return NotFound();

            foreach (var tagId in tagIds)
            {
                if (!image.ImageTags.Any(it => it.TagId == tagId))
                    image.ImageTags.Add(new ImageTag { ImageId = id, TagId = tagId });
            }

            await _db.SaveChangesAsync();
            return Ok(image);
        }

        [HttpDelete("{id}/tags/{tagId}")]
        public async Task<IActionResult> RemoveTag(int id, int tagId)
        {
            var imageTag = await _db.ImageTags.FirstOrDefaultAsync(it => it.ImageId == id && it.TagId == tagId);
            if (imageTag == null) return NotFound();
            _db.ImageTags.Remove(imageTag);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}