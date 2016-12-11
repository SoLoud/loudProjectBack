using Facebook;
using HttpUtils;
using RestSharp;
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
        [Route("asf")]
        public async Task PostByasfImage(/*postImageouliz Requesto*/)
        {
            var client = new RestClient("http://localhost:55741/Post/ByImage");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "multipart/form-data");
            request.AddHeader("authorization", "Bearer -cANN0FnxNga63VOJePNU-mJdyaPkz2b1qzlpZSAqssKrYISidnZzhIGCFZf66hZxbmKgQ18jW4IkrQSaO5pq35Q5tATF123GcNdbjH1uVxS18fzQ7PSSKLxws_TiPKIaRW7a7Z3G9qQL3mUvoVo189pX4pRm6A2dGSiYUbydcfZ1_sjnvUW7O-GWrHEAs1lhXqKnDsidVudMOzBBcSFICyg0T3sVgoRxDZWgoPI-DwyqEHxUUzvDjeSCQOPHm1RyTYvmML9UmLEgewHMzc90hrZdpAtWxayZHb8VQY1R_VcXCkvsYmv8w6QYqC1hctMzZdnNDsgLuOEaPzsXRtsXSXegyrkybPFs8wpaIindBrWEPPglakoOMF9_Y73BR9tAYyMWCIsdUZd7Coa9khOqvgJZQDdVOlnl1wKM-7Yje23bCY8C8nvqkYURo5VJgpKMgwcMLLuCPKqptYMZq9zxymWYnZ6H34o4WuEKrIG_iXC1XVThJXKXUN1RINr6rxdGzSkHC0zkdMN8NMT4pXmC4TPyoFUkAb4XxIDk5DWj1OS4eZZz7bWas2DWVf-dLKaN1yTl6Kv2ZgYd_OZPl7ekKluHOJVPLpmd1txuYoSKiY_nOKqIgcLSRa9-8InweiDT6dhgfkY1jOLyZUxt5uTc5f9R-SFg-J5TrCbjADP_nj-cwoywzofwVQwItXwJfYbTNTtR4wiAAqpK1b7fWBUjeXONr8DEbjdoei8k1r8_nlPywO72NXCfL-hbdZHtE47");
            request.AddFile("Image", @"C:\Users\Drakkar\Downloads\File.jpg\");
            request.AddParameter("Caption", "yoleleiol", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        private byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        [HttpPost]
        [Route("ByImage")]
        public async Task PostByImage([FromUri]postImageouliz Requesto)
        {
            //Stream req = Request.RequestContext.Content.ReadAsStreamAsync().Result;

            var reqo = Request.Content.ReadAsStreamAsync().Result;
            //Stream reqo2 = new MemoryStream();
            //reqo.CopyTo(reqo2);
            HttpMultipartParser parser = new HttpMultipartParser(reqo, "file0");
            //HttpMultipartParser parser2 = new HttpMultipartParser(reqo2, "file1");

            reqo.Position = 0;
            var bytear = ReadFully(reqo);

            var client = new RestClient("https://graph.facebook.com/v2.8");

            var req = new RestRequest("me/photos", Method.POST);
            //req.AddFile("source", frontImage.CopyTo);
            req.AddParameter("message", "Null object reference");

            req.AddParameter("access_token", FacebookAccessToken);

            var aadsasffsa = GetBytes(parser.yolo);
            req.AddFile("source", aadsasffsa, "asd.sp");

            //Send request
            IRestResponse resp = client.Execute(req);

            //var parser = new HttpMultipartParser(req, "file");

            //// Check if the request contains multipart/form-data.
            //if (!Request.Content.IsMimeMultipartContent())
            //{
            //    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            //}
        }
        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        public class postImageouliz
        {
            public string Caption { get; set; }
            //public System.Net.WebRequestMethods.File Image { get; set; }
        }

        public class postImageouliz2
        {
            public HttpPostedFileBase Image { get; set; }
        }

        private void UploadByAttach()
        {
            var client = new RestClient("https://graph.facebook.com/v2.8");

            var req = new RestRequest("me/photos", Method.POST);
            req.AddParameter("message", "Null object reference");

            req.AddParameter("access_token", FacebookAccessToken);

            //req.AddFile("source", frontImage.CopyTo);

            //Send request
            IRestResponse resp = client.Execute(req);
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