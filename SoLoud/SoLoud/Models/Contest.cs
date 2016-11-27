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

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset EndingAt { get; set; }
        public Categoies Category { get; set; }
        public string ProductImageUrl { get; set; }
        public string ExampleImageUrl { get; set; }
    }
}