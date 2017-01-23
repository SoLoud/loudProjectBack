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

namespace SoLoud.ApiControllers
{
    [Authorize]
    [RoutePrefix("api/Posts")]
    public class Post2Controller : BaseApiController
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

        [HttpPost]
        [Route("")]
        public async Task PostByImage()
        {
            var reqo = Request.Content.ReadAsStreamAsync().Result;
            HttpMultipartParser parser = new HttpMultipartParser(reqo);

            var fbClient = new FbClient(FacebookAccessToken);

            if (parser.Success) 
                await fbClient.MultiphotoStory(parser.Files, parser.Parameters["caption"]);
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