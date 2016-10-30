using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoLoud.Models
{
    public class HashTag
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string ItemId { get; set; }
        public virtual ContentItem Item { get; set; }
        public string Name { get; set; }
        public bool IsRequired { get; set; }
    }
}