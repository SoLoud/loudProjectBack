using Facebook;
using SoLoud.Repositories;
using System.Linq;
using System.Web.Http;

namespace SoLoud.ApiControllers
{
    [Authorize]
    [RoutePrefix("api/Posts")]
    public class PostController : BaseApiController
    {
        [HttpPost]
        [Route("")]
        public void Post()
        {
            var fb = new FacebookClient(FacebookAccessToken);

            dynamic par = new System.Dynamic.ExpandoObject();
            par.message = "asfas";
            par.access_token = FacebookAccessToken;

            dynamic res = fb.Post("me/feed", par);

            var repo = new PostRepo(UserId);
            repo.Add(par.message);
        }
    }
}