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
        public string Text { get; set; }
        public bool IsVerified { get; set; }
        public DateTimeOffset? VerifiedAt { get; set; }
        [Required]
        public SocialMedia SocialMedium { get; set; }
    }
}