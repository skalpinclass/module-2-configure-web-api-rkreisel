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
        private readonly JsonSerializerSettings jsonSettings;

        public BlogController(BlogContext db)
        {
            this.db = db;
            this.jsonSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        //public ActionResult<IEnumerable<Blog>> Get(bool includePosts = true)
        public ActionResult<IEnumerable<Blog>> Get(bool includePosts = true)
        {
            IEnumerable<Blog> result;
            var query = db.Blogs;
            if (includePosts)
                result = query.Include(blog => blog.Posts).AsEnumerable();
            else
                result = query.AsEnumerable();
            return new JsonResult(result, this.jsonSettings);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        public ActionResult<Blog> Get(int id, bool includePosts = true)
        {
            Blog result;
            IEnumerable<Blog> query;
            if (includePosts)
                query = db.Blogs.Include(blog => blog.Posts);
            else
                query = db.Blogs;

            result = query.SingleOrDefault(b => b.BlogId == id);

            if (result == null)
                return NotFound($"Id {id} not found");

            return new JsonResult(result, this.jsonSettings);
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        public ActionResult Post([FromBody] Blog value)
        {
            var newEntry = new Blog
            {
                Rating = value.Rating,
                Posts = value.Posts,
                Url = value.Url
            };
            db.Blogs.Add(newEntry);
            db.SaveChanges();
            return Ok($"Blog post added. Id = {newEntry.BlogId}");
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        public ActionResult Put(int id, [FromBody] Blog value)
        {
            var existingBlogEntry = db.Blogs.Include(blog => blog.Posts).FirstOrDefault(b => b.BlogId == value.BlogId);
            if (existingBlogEntry == null)
            {
                return NotFound($"Could not find BlogId {value.BlogId} to update.");
            }
            foreach (var incomingPost in value.Posts)
            {
                var existingPost = existingBlogEntry.Posts.SingleOrDefault(p => p.BlogPostId == incomingPost.BlogPostId);
                if (existingPost != null)
                {
                    existingPost.Title = incomingPost.Title;
                    existingPost.Content = incomingPost.Content;
                }
                else
                {
                    existingBlogEntry.Posts.Add(new BlogPost { Content = incomingPost.Content, Title = incomingPost.Title, BlogId = existingBlogEntry.BlogId });
                }
            }
            existingBlogEntry.Rating = value.Rating;
            existingBlogEntry.Url = value.Url;
            db.Blogs.Update(existingBlogEntry);
            db.SaveChanges();
            return Ok($"Blog with id of {id} updated.");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        public ActionResult Delete(int id)
        {
            var existingBlogEntry = db.Blogs.FirstOrDefault(b => b.BlogId == id);
            if (existingBlogEntry == null)
            {
                return NotFound($"Could not find BlogId {id} to delete.");
            }
            db.Blogs.Remove(existingBlogEntry);
            db.SaveChanges();
            return Ok($"Blog with id of {id} was deleted.");
        }

    }
}