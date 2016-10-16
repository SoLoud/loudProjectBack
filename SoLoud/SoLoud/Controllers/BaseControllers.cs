using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace SoLoud.Controllers
{
    public class BaseController : Controller
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ClaimsPrincipal User
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
                return User.Identity.GetUserId();
            }
        }

        internal string FacebookAccessToken
        {
            get
            {
                var claim = (User.Identity as ClaimsIdentity).FindFirst("FacebookAccessToken");

                string accessToken = null;
                if (claim != null)
                    accessToken = claim.Value;

                return accessToken;
            }
        }
    }

    public class BaseApiController : ApiController
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        private ClaimsPrincipal User
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
                return User.Identity.GetUserId();
            }
        }

        internal string FacebookAccessToken
        {
            get
            {
                var claim = (User.Identity as ClaimsIdentity).FindFirst("FacebookAccessToken");

                string accessToken = null;
                if (claim != null)
                    accessToken = claim.Value;

                return accessToken;
            }
        }
    }

}