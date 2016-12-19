using Facebook;
using HttpUtils;
using RestSharp;
using SoLoud.Models;
using SoLoud.Repositories;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SoLoud.ApiControllers
{
    [Authorize]
    [RoutePrefix("api/Posts")]
    public class Post2Controller : BaseApiController
    {
        [HttpPost]
        [Route("")]
        public void Post([FromBody]string postMessage)
        {
            var fb = new FacebookClient(FacebookAccessToken);

            dynamic par = new System.Dynamic.ExpandoObject();
            par.message = postMessage;
            par.access_token = FacebookAccessToken;
            //par.source = "http://img-9gag-fun.9cache.com/photo/aLMGEW6_460s.jpg";

            dynamic res = fb.Post("me/feed", par);

            var repo = new PostRepo(UserId);
            repo.Add(par.message);
        }

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

        [HttpPost]
        [Route("ByImage")]
        public async Task PostByImage([FromUri]postImageouliz Requesto)
        {
            var reqo = Request.Content.ReadAsStreamAsync().Result;
            HttpMultipartParser parser = new HttpMultipartParser(reqo);

            var client = new RestClient("https://graph.facebook.com/v2.8");
            var req = new RestRequest("me/photos", Method.POST);

            req.AddParameter("message", Requesto.Caption);
            req.AddParameter("access_token", FacebookAccessToken);
            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            //if (parser.Success)
            //{
            //    var i = 0;
            //    foreach (var file in parser.Files)
            //    {
            //        req.AddFile("source" + i, file.Content, file.FileName, file.ContentType);
            //        i++;
            //    }
            //}
                    req.AddFile("source", parser.Files[0].Content, parser.Files[0].FileName, parser.Files[0].ContentType);

            IRestResponse resp = client.Execute(req);

        }
        private SoLoud.Models.File filo { get; set; }
        private SoLoud.Models.File filo2 { get; set; }
        [HttpPost]
        [Route("ByImage2")]
        public async Task PostByImage2([FromUri]postImageouliz Requesto)
        {
        

            var reqo = Request.Content.ReadAsStreamAsync().Result;
            HttpMultipartParser parser = new HttpMultipartParser(reqo);

            var client = new RestClient("http://localhost:55741/api");
            var req = new RestRequest("Posts/ByImage?Caption=yololeilo", Method.POST);
            req.AddParameter("message", Requesto.Caption);
            req.AddParameter("access_token", FacebookAccessToken);
            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            //if (parser.Success)
            //{
            //    var i = 0;
            //    foreach (var file in parser.Files)
            //    {
            //        req.AddFile("source" + i, file.Content, file.FileName, file.ContentType);
            //        i++;
            //    }
            //}
            req.AddFile("source", parser.Files[0].Content, parser.Files[0].FileName, parser.Files[0].ContentType);

        }

        public class postImageouliz
        {
            public string Caption { get; set; }
        }

        private void Send2()
        {
            var client = new RestClient("https://graph.facebook.com/v2.8");

            var req = new RestRequest("me/feed", Method.POST);
            //req.AddFile("source", frontImage.CopyTo);
            req.AddParameter("message", "Test mutli photo story2");

            req.AddParameter("access_token", FacebookAccessToken);

            req.AddParameter("attached_media[0]", new { media_fbid = "10154731371079266" });
            req.AddParameter("attached_media[1]", new { media_fbid = "10154731391714266" });

            //Send request
            IRestResponse resp = client.Execute(req);
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