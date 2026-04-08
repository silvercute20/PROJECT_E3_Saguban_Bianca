using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace auth_api.Controllers
{
    [ApiController]
    [Route("packs")]
    public class PacksController : ControllerBase
    {
        private static List<Pack> packs = new List<Pack>();

        [HttpGet]
        public IActionResult GetAll() => Ok(packs);

        [HttpPost]
        public IActionResult Create([FromBody] Pack pack)
        {
            pack.Id = packs.Count + 1;
            packs.Add(pack);
            return CreatedAtAction(nameof(GetAll), new { id = pack.Id }, pack);
        }
    }

    public class Pack
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}