using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoLoud.Models
{
    public enum SocialMedia
    {
        Facebook, Twitter, GooglePlus, Instagram
    }
    public class Post : ContentItem
    {
        public Post()
        {
            CreatedAt = DateTimeOffset.Now;
        }

        public string Text { get; set; }
        public bool IsVerified { get; set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset? VerifiedAt { get; set; }
        [Required]
        public SocialMedia SocialMedium { get; set; }
        public string HashTags { get; set; }
        public virtual List<File> Photos { get; set; }

        public string ContestId { get; set; }
        public virtual Contest Contest { get; set; }

        public string FacebookId { get; set; }
    }
}