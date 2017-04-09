using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SoLoud.Models
{
    public enum FileType
    {
        Avatar = 1, Photo
    }

    public class File
    {
        [IgnoreDataMember]
        public int FileId { get; set; }

        [StringLength(255)]
        [IgnoreDataMember]
        public string FileName { get; set; }

        [StringLength(100)]
        [IgnoreDataMember]
        public string ContentType { get; set; }

        [IgnoreDataMember]
        public byte[] Content { get; set; }

        [IgnoreDataMember]
        public FileType FileType { get; set; }

        public string url
        {
            get
            {
                var AuthorityUrl =  (System.Web.HttpContext.Current != null) ? System.Web.HttpContext.Current.Request.Url.GetLeftPart(System.UriPartial.Authority) : "https://soloud.azurewebsites.net";

                return AuthorityUrl + "/File?id=" + this.FileId;
            }
        }

        [IgnoreDataMember]
        public string PostId { get; set; }
        [IgnoreDataMember]
        [ForeignKey("PostId")]
        [InverseProperty("Photos")]
        public Post Post { get; set; }

        [IgnoreDataMember]
        public string ProductForContestId { get; set; }
        [IgnoreDataMember]
        [ForeignKey("ProductForContestId")]
        [InverseProperty("ProductPhotos")]
        public Contest ProductForContest { get; set; }

        [IgnoreDataMember]
        public string ExampleForContestId { get; set; }
        [IgnoreDataMember]
        [ForeignKey("ExampleForContestId")]
        [InverseProperty("ExamplePhotos")]
        public Contest ExampleForContest { get; set; }
    }
}