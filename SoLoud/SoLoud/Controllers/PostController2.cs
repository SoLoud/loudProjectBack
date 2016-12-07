using Facebook;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SoLoud.Models;
using SoLoud.Repositories;
using Newtonsoft.Json;

namespace SoLoud.Controllers
{
    [Authorize]
    public class PostController : BaseController
    {
        [Route("")]
        public void Post(string postMessage)
        {
            var fb = new FacebookClient(FacebookAccessToken);

            dynamic par = new System.Dynamic.ExpandoObject();
            par.message = postMessage;
            par.access_token = FacebookAccessToken;

            dynamic res = fb.Post("me/feed", par);

            var repo = new PostRepo(UserId);
            repo.Add(par.message);
        }

        [HttpGet]
        [Route("Likes")]
        public void GetLikes()
        {
            var fb = new FacebookClient(FacebookAccessToken);

            var ResultAsGenericObject = fb.Get("10154307492864266/reactions?summary=true");
            //PostReactions result = fb.Get<PostReactions>("10154307492864266/reactions?summary=true"); // We could have just used this, but the FUCKING library wont cat strings to my enum correctly. So i have to do it myself
            PostReactions result = JsonConvert.DeserializeObject<PostReactions>(ResultAsGenericObject.ToString());
        }
    }
}