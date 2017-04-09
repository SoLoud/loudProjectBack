using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SoLoud.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MasterKeyFilter : Attribute, IAuthenticationFilter
    {
        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                       || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

        }
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            bool isAuthenticated = false;
            IEnumerable<string> headerValues = new List<string>();

            if (SkipAuthorization(context.ActionContext))
            {
                isAuthenticated = true;
            }

            //If there is exactly one X-ZUMO-MASTER header..
            else if (context.Request.Headers.TryGetValues("MasterKey", out headerValues) && headerValues.Count() == 1)
            {
                var headerValue = headerValues.First();
                var MasterKey = ConfigurationManager.AppSettings["MasterKey"].ToString();

                //... and its value equals the Master key, we are ok.
                if (headerValue == MasterKey) isAuthenticated = true;
            }

            //... in any other case this is an anauthorized request
            if (!isAuthenticated)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public bool AllowMultiple { get { return true; } }
    }
}
