using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SoLoud.Filters;

namespace SoLoud.Controllers
{
    [JsonOnly]
    public class BaseController : Controller
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ClaimsPrincipal UserClaim
        {
            get
            {
                return AuthenticationManager.User;
            }
        }

        internal string UserId
        {
            get
            {
                return UserClaim.Identity.GetUserId();
            }
        }

        internal string FacebookAccessToken
        {
            get
            {
                var claim = (UserClaim.Identity as ClaimsIdentity).FindFirst("FacebookAccessToken");

                string accessToken = null;
                if (claim != null)
                    accessToken = claim.Value;

                return accessToken;
            }
        }
    }
}

namespace SoLoud.ApiControllers
{
    [JsonOnly]
    public class BaseApiController : ApiController
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        private ClaimsPrincipal UserClaim
        {
            get
            {
                return AuthenticationManager.User;
            }
        }

        internal string UserId
        {
            get
            {
                return UserClaim.Identity.GetUserId();
            }
        }

        internal string FacebookAccessToken
        {
            get
            {
                var claim = (UserClaim.Identity as ClaimsIdentity).FindFirst("FacebookAccessToken");

                string accessToken = null;
                if (claim != null)
                    accessToken = claim.Value;

                return accessToken;
            }
        }
    }

}