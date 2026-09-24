using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogApi
{
    [ApiController]
    [Route("api/[controller]")]
    public class Blogpostcontroller : ControllerBase
    {
        private static readonly List<Blogpost> _posts = new();
        private static int _nextId = 1;

        [HttpGet]
        public ActionResult<List<Blogpost>> Osszes()
        {
            return Ok(_posts);
        }

        [HttpGet("{id}")]
        public ActionResult<Blogpost> Lekerdez(int id)
        {
            var p = _posts.FirstOrDefault(x => x.Id == id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public ActionResult<Blogpost> Letrehoz([FromBody] Blogpost create)
        {
            create.Id = _nextId++;
            create.CreatedAt = DateTime.UtcNow;
            _posts.Add(create);
            return CreatedAtAction(nameof(Lekerdez), new { id = create.Id }, create);
        }

        [HttpPut("{id}")]
        public ActionResult Frissit(int id, [FromBody] Blogpost update)
        {
            var existing = _posts.FirstOrDefault(x => x.Id == id);
            if (existing == null) return NotFound();
            existing.Title = update.Title;
            existing.Content = update.Content;
            existing.Author = update.Author;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Torol(int id)
        {
            var existing = _posts.FirstOrDefault(x => x.Id == id);
            if (existing == null) return NotFound();
            _posts.Remove(existing);
            return NoContent();
        }
    }
}
