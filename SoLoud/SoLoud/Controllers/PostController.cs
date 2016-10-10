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

namespace SoLoud.Controllers
{
    [Authorize]
    [RoutePrefix("Post")]
    public class PostController : BaseApiController
    {
        [HttpPost]
        [Route("")]
        async public void Post()
        {
            var fb = new FacebookClient(FacebookAccessToken);

            //          POST graph.facebook.com
            ///{user-id}/feed?
            //  message={message}&
            //  access_token={access-token}

            dynamic par = new System.Dynamic.ExpandoObject();
            par.message = "SoLoud test post NEW!";
            par.access_token = FacebookAccessToken;

            dynamic res = fb.Post("me/feed", par);

            dynamic result = fb.Get("me");
            var name = result.name;
        }
    }
}