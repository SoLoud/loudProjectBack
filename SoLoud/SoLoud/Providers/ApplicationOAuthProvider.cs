using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Collections.Generic;
using SoLoud.Helpers;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using SoLoud.Models;
using SoLoud.Controllers;

namespace SoLoud.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
                else if (context.ClientId == "web")
                {
                    var expectedUri = new Uri(context.Request.Uri, "/");
                    context.Validated(expectedUri.AbsoluteUri);
                }
            }

            return Task.FromResult<object>(null);
        }

        public async override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var ExternalLoginTokenType = getExternalLoginType(context);

            switch (ExternalLoginTokenType)
            {
                case ExternalLoginTokenType.Facebook:
                    await GrantResourceOwnerCredentialsFromFacebookToken(context);
                    return;
                case ExternalLoginTokenType.Twitter:
                    await GrantResourceOwnerCredentialsFromTwitterToken(context);
                    return;
                case ExternalLoginTokenType.Instagram:
                    await GrantResourceOwnerCredentialsFromInstagramToken(context);
                    return;
                default:
                    await base.GrantResourceOwnerCredentials(context);
                    return;
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
            //return base.ValidateClientAuthentication(context);
        }

        private ExternalLoginTokenType getExternalLoginType(OAuthGrantResourceOwnerCredentialsContext context)
        {
            string[] HeaderValues;
            if (context.Request.Headers.TryGetValue("ExternalLoginType", out HeaderValues))
            {
                var HeaderValue = HeaderValues[0];

                return EnumHandler.ParseEnum<ExternalLoginTokenType>(HeaderValue, ExternalLoginTokenType.None);
            }

            return ExternalLoginTokenType.None;
        }

        private string getExternalToken(OAuthGrantResourceOwnerCredentialsContext context)
        {
            string[] HeaderValues;
            if (context.Request.Headers.TryGetValue("ExternalToken", out HeaderValues))
            {
                var HeaderValue = HeaderValues[0];

                return HeaderValue;
            }

            return null;
        }

        private async Task GrantResourceOwnerCredentialsFromFacebookToken(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //Find External Token
            var facebookToken = getExternalToken(context);
            if (facebookToken == null)
                throw new Exception("ExternalToken is null");

            //Get User Using FacebookToken
            var fb = new FacebookClient(facebookToken);

            Facebook.Me me = fb.Get<Facebook.Me>("me", new { fields = "id, name, email, gender, birthday, picture.type(large)" });

            var User = userManager.FindByEmail(me.email);
            if (User == null)
            {
                var AccCtrl = new AccountController(HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(), HttpContext.Current.GetOwinContext().GetUserManager<ApplicationSignInManager>());
                User = await AccCtrl.CreateUser("User", me.email, me.email, null);
            }

            //Create Token and return
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("UserName", User.UserName));
            identity.AddClaim(new Claim("FacebookAccessToken", facebookToken));

            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                { "User", JsonConvert.SerializeObject(User) }
            });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);

            return;
        }

        private Task GrantResourceOwnerCredentialsFromTwitterToken(OAuthGrantResourceOwnerCredentialsContext context)
        {
            throw new NotImplementedException();
        }

        private Task GrantResourceOwnerCredentialsFromInstagramToken(OAuthGrantResourceOwnerCredentialsContext context)
        {
            throw new NotImplementedException();
        }

        private ApplicationUserManager userManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
    }

    public enum ExternalLoginTokenType
    {
        None,
        Facebook,
        Twitter,
        Instagram
    }
}