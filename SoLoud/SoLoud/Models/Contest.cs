using SoLoud.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoLoud.Models
{
    public class Contest
    {
        public Contest()
        {
            this.CreatedAt = DateTimeOffset.Now;
        }

        [Key]
        public string Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public List<string> Hashtags { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset EndingAt { get; set; }
        public string Category { get; set; }
    }
}