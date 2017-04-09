using SoLoud.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoLoud.Models
{
    public enum Categoies
    {
        Charity,
        Cosmetics,
        [Display(Name = "Home Decoration")]
        HomeDecoration,
        Entertainment,
        Fashion,
        Fitness,
        Food,
        Pets,
        Travel
    }

    public class Contest : ContentItem
    {
        public Contest()
        {
            this.CreatedAt = DateTimeOffset.Now;
        }

        [MaxLength(255)]
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset EndingAt { get; set; }
        public Categoies Category { get; set; }
        public string RequiredHashTags { get; set; }
        public string OptionalHashTags { get; set; }
        public virtual List<File> ProductPhotos { get; set; }
        public virtual List<File> ExamplePhotos { get; set; }
    }
}