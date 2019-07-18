using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blog.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private BlogContext db;
        private JsonSerializerSettings jsonSettings;
        private JsonSerializerSettings jsonSettingsIndented;

        public BlogController(BlogContext db)
        {
            this.db = db;
            this.jsonSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            this.jsonSettingsIndented = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects, Formatting = Formatting.Indented };
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<Blog>> Get(bool includePosts = true, bool formatted = false)
        {
            IEnumerable<Blog> result;
            if (includePosts)
                result = db.Blogs.Include(blog => blog.Posts).AsEnumerable();
            else
                result = db.Blogs.AsEnumerable();

            if (formatted)
                return Ok(JsonConvert.SerializeObject(result, this.jsonSettingsIndented));
            else
                return Ok(JsonConvert.SerializeObject(result, this.jsonSettings));
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        public void Post([FromBody] Blog value)
        {
            var newEntry = new Blog
            {
                Rating = value.Rating,
                Posts = value.Posts,
                Url = value.Url
            };
            db.Blogs.Add(newEntry);
            db.SaveChanges();
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        public void Put(int id, [FromBody] Blog value)
        {
            var existingBlogEntry = db.Blogs.FirstOrDefault(b => b.BlogId == value.BlogId);
            if (existingBlogEntry == null)
            {
                throw new InvalidOperationException($"Could not find BlogId {value.BlogId} to update.");
            }
            existingBlogEntry.Posts = value.Posts;
            existingBlogEntry.Rating = value.Rating;
            existingBlogEntry.Url = value.Url;
            db.Blogs.Update(existingBlogEntry);
            db.SaveChanges();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        public void Delete(int id)
        {
            var existingBlogEntry = db.Blogs.FirstOrDefault(b => b.BlogId == id);
            if (existingBlogEntry == null)
            {
                throw new InvalidOperationException($"Could not find BlogId {id} to delte.");
            }
            db.Blogs.Remove(existingBlogEntry);
            db.SaveChanges();
        }
        
    }
}