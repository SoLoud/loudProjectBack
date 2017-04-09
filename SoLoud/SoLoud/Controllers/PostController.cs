using SoLoud.Helpers;
using HttpUtils;
using RestSharp;
using SoLoud.Models;
using SoLoud.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Facebook;
using SoLoud.Filters;
using System.Data.Entity.Infrastructure;
using ContactHub.Helpers;
using System.Configuration;
using System.Data.Entity;

namespace SoLoud.ApiControllers
{
    [Authorize]
    [RoutePrefix("api/Posts")]
    public class Post2Controller : AuthorizedController
    {
        private void UploadByUrl()
        {
            var client = new RestClient("https://graph.facebook.com/v2.8");

            var req = new RestRequest("me/photos", Method.POST);
            //req.AddFile("source", frontImage.CopyTo);
            req.AddParameter("message", "Null object reference");

            req.AddParameter("access_token", FacebookAccessToken);

            req.AddParameter("url", "http://img-9gag-fun.9cache.com/photo/aLMGEW6_460s.jpg");

            //Send request
            IRestResponse resp = client.Execute(req);
        }

        private void sendEmailAboutPostVerification(Post post)
        {
            var email = new Email();
            email.FromAddress = "info@soloud.com";
            email.Body = String.Format("<div>somebody did a post and is waiting for verification. </div><br><div><p>{0}</p></div> ", post.Text);
            foreach (var photo in post.Photos)
                email.Body += String.Format("<img width='200' src='{0}'>", photo.url);
            email.FromName = "Soloud";
            email.Subject = "Post needs verification";

            var ElasticEmail = new ElasticEmailHelper();
            ElasticEmail.Send(email, new List<string>() { "soloudapp@gmail.com" });
        }

        [HttpPost]
        [Route("")]
        public void PostByImage()
        {
            var reqo = Request.Content.ReadAsStreamAsync().Result;
            HttpMultipartParser parser = new HttpMultipartParser(reqo);

            if (!parser.Success)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var post = new Post();
            post.ContestId = parser.Parameters["ContestId"];
            post.HashTags = parser.Parameters["Hashtags"];
            post.Id = Guid.NewGuid().ToString();
            post.IsVerified = false;
            post.Photos = parser.Files.Select(x => x.Value).ToList();
           
            post.SocialMedium = SocialMedia.Facebook;
            post.Text = parser.Parameters["caption"];
            post.UserId = UserId;

            ModelState.Clear();
            Validate(post);

            if (!ModelState.IsValid)
                throw new HttpResponseException(new HttpResponseMessage() { Content = new ObjectContent(ModelState.GetType(), ModelState, this.Configuration.Formatters.JsonFormatter) });

            db.Posts.Add(post);
            try
            {
                db.SaveChanges();

                sendEmailAboutPostVerification(post);
            }
            catch (DbUpdateException)
            {
                if (PostExists(post.Id))
                {
                    throw new HttpResponseException(HttpStatusCode.Conflict);
                }
                else
                {
                    throw;
                }
            }
        }

        private bool PostExists(string id)
        {
            return db.Posts.Any(x => x.Id == id);
        }
    }

    [MasterKeyFilter]
    [RoutePrefix("admin/Posts")]
    public class AdminPostController : ApiController
    {
        private string EncryptionKey = ConfigurationManager.AppSettings["EncryptionKey"].ToString();

        [HttpPost]
        [Route("")]
        public async Task VerifyPost(string PostId)
        {
            var db = new SoLoudContext();

            Post post = db.Posts.FirstOrDefault(x => x.Id == PostId);
            if (post == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("No Post with such id") });
            else if (post.IsVerified)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Post already verified") });

            var user = db.Users.Include("Claims").FirstOrDefault(x => x.Id == post.UserId);
            if (user == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.ExpectationFailed) { Content = new StringContent(String.Format("Post is connected to user with id {0} but no such user was found", post.UserId)) });

            var FbClaim = user.Claims.FirstOrDefault(x => x.ClaimType == SoloudClaimTypes.FacebookAccessToken.ToString() && x.UserId == user.Id);
            if (FbClaim == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.ExpectationFailed) { Content = new StringContent(String.Format("No saved fb token was found for user with id {0}", user.Id)) });

            var EncryptedFbToken = FbClaim.ClaimValue;
            var DecryptedFbToken = AESThenHMAC.SimpleDecryptWithPassword(EncryptedFbToken, EncryptionKey);

            var fbClient = new FbClient(DecryptedFbToken);

            var fbResponse = await fbClient.MultiphotoStory(post.Photos.ToList(), post.Text);

            post.VerifiedAt = DateTimeOffset.Now;
            post.IsVerified = true;
            post.FacebookId = fbResponse.id;
            db.SaveChanges();
        }
    }

    [Authorize]
    public class PostController : Controllers.BaseController
    {
        public class postImageouliz
        {
            public string Caption { get; set; }
            public HttpPostedFileBase Image { get; set; }
        }

        [HttpPost]
        [Route("ByImage")]
        public System.Web.Mvc.ActionResult ByImage(postImageouliz Requesto)
        {
            throw new Exception();
        }
    }

}