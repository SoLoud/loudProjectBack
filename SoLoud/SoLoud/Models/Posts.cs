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
    public class Post
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string text { get; set; }
        public bool IsVerified { get; set; }
        public DateTimeOffset? VerifiedAt { get; set; }
        [Required]
        public SocialMedia SocialMedium { get; set; }
    }
}