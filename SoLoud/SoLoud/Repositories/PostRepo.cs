using SoLoud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace SoLoud.Repositories
{
    public class PostRepo
    {
        private SoLoudContext Context { get; set; }
        private string UserId { get; set; }
        public PostRepo(string UserId)
        {
            this.UserId = UserId;
            this.Context = new SoLoudContext();
        }

        public Post Add(string text)
        {
            var newPost = new Post()
            {
                Id = Guid.NewGuid().ToString(),
                IsVerified = false,
                Text = text,
                UserId = UserId,
                SocialMedium = SocialMedia.Facebook
            };

            newPost = Context.Posts.Add(newPost);
            Context.SaveChanges();

            return newPost;
        }
        private void Add(Post Post)
        {
            Post.IsVerified = false;
            Post.Id = Guid.NewGuid().ToString();
            Post.UserId = UserId;

            Context.Posts.Add(Post);
            Context.SaveChanges();
        }
    }
}