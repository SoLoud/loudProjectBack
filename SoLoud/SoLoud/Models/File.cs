using System.ComponentModel.DataAnnotations;

namespace SoLoud.Models
{
    public enum FileType
    {
        Avatar = 1, Photo
    }

    public class File
    {
        public int FileId { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public FileType FileType { get; set; }

        public string ItemId { get; set; }
        public virtual ContentItem Item { get; set; }
    }
}