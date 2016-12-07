using System;
using System.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SoLoud.Helpers
{
    public static class TokenHandler
    {
        private static string SIGNING_TOKEN
        {
            get
            {
                return "";
            }
        }

        private static string SECRET_KEY
        {
            get
            {
                return ConfigurationManager.AppSettings["secretKey"].ToString();
            }
        }

        public static string Create(string email, string FbToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var symmetricKey = Encoding.ASCII.GetBytes(SECRET_KEY).ToArray();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Email, email),
                            new Claim("FacebookAccessToken", FbToken),
                        }),
                TokenIssuerName = "SoLoud",
                //AppliesToAddress = "http://www.example.com",
                Lifetime = new Lifetime(now, now.AddDays(59)),
                SigningCredentials = new SigningCredentials(
                        new InMemorySymmetricSecurityKey(symmetricKey),
                        "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                        "http://www.w3.org/2001/04/xmlenc#sha256"),


            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public static bool Validate(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var asdas = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var jti = asdas.Claims.First(claim => claim.Type == "email").Value;

            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningToken = new JwtSecurityToken(token),
                ValidateLifetime = true,
                //AllowedAudience = "http://www.example.com",
                //SigningToken = new BinarySecretSecurityToken(symmetricKey),
                ValidIssuer = "SoLoud"
            };

            SecurityToken aosdk;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out aosdk);
            principal.Identities.First().Claims
                .Any(c => c.Type == ClaimTypes.Name && c.Value == "Pedro");

            principal.Identities.First().Claims
                .Any(c => c.Type == ClaimTypes.Role && c.Value == "Author");

            return true;
        }
    }
}