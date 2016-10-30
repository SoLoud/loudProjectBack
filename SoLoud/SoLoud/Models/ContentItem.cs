using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoLoud.Models
{
    public abstract class ContentItem
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        [JsonIgnore]
        public virtual ICollection<HashTag> HashTags { get; set; }
    }
}