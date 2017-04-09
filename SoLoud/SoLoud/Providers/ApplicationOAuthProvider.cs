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
using System.Linq;
using System.Data.Entity;
using ContactHub.Helpers;
using System.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;

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

        public override Task MatchEndpoint(OAuthMatchEndpointContext context)
        {
            if (context.IsTokenEndpoint)
            {
                // Allows cors for the /token endpoint this is different from webapi endpoints. 
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "*" });
                if (context.Request.Method == "OPTIONS")
                    context.RequestCompleted();
                return Task.FromResult(0);
            }

            return base.MatchEndpoint(context);
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
                case ExternalLoginTokenType.None:
                    await GrantCredentials(context);
                    return;
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

        private async Task<string> getParameter(OAuthGrantResourceOwnerCredentialsContext context, string parameter)
        {
            var form = await context.Request.ReadFormAsync();

            if (!string.IsNullOrEmpty(form[parameter]))
                return form[parameter];
            else
                return null;
        }

        private async Task<string> getPassword(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return await getParameter(context, "password");
        }

        private async Task<string> getUsername(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return await getParameter(context, "username");
        }

        internal async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await userManager.GenerateEmailConfirmationTokenAsync(userID);

            var callbackUrl = String.Format("{0}://{1}/Account/ConfirmEmail?userId={2}&code={3}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, userID, code);
            await userManager.SendEmailAsync(userID, subject, "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;
        }

        private async Task GrantCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            string username = await getUsername(context);
            string password = await getPassword(context);

            // Require the user to have a confirmed email before they can log on.
            var user = userManager.Find(username, password);
            if (user == null)
            {
                context.SetError("10001", "Wrong Email/Password combination");
                return;
            }
            else if (!await userManager.IsEmailConfirmedAsync(user.Id))
            {
                var AccCtrl = new AccountController(HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(), HttpContext.Current.GetOwinContext().GetUserManager<ApplicationSignInManager>());

                await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account-Resend");

                context.SetError("10002", "Email is not confirmed. A confirmation email has been sent to " + username + ". Confirm your account and try again.");
                return;
            }

            //Create Token and return
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("UserName", username));
            identity.AddClaim(new Claim("UserId", user.Id));

            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                { "User", JsonConvert.SerializeObject(user) }
            });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);

            return;
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

            //Save fbtoken to db
            var db = new SoLoudContext();
            //We need to refetch from db in order to be able to edit/add claims. If we dont the context is not tracking the Claim entities and changes will not save
            User = db.Users.Include("Claims").FirstOrDefault(x => x.Id == User.Id);
            var fbTokenClaim = User.Claims.FirstOrDefault(x => x.ClaimType == SoloudClaimTypes.FacebookAccessToken.ToString());
            if(fbTokenClaim == null)
            {
                fbTokenClaim = new Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim() {
                    ClaimType = SoloudClaimTypes.FacebookAccessToken.ToString()
                };
                User.Claims.Add(fbTokenClaim);
            }
            fbTokenClaim.ClaimValue = AESThenHMAC.SimpleEncryptWithPassword(facebookToken, ConfigurationManager.AppSettings["EncryptionKey"].ToString());
            db.SaveChanges();

            //Create Token and return
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("UserName", User.UserName));
            identity.AddClaim(new Claim("UserId", User.Id));
            identity.AddClaim(new Claim(SoloudClaimTypes.FacebookAccessToken.ToString(), facebookToken));

            //find user roles
            var UserRoles = User.Roles.Join(db.Roles, x => x.RoleId, r => r.Id, (x, r) => r.Name);
            if (UserRoles != null && UserRoles.Count() > 0)
                identity.AddClaim(new Claim("Roles", UserRoles.Aggregate((acc, cur) => acc += "," + cur)));

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