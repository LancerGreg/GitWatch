using GitWatch.Domain.Models;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using OwinSamle.Service;

namespace GitWatch.Data.Services
{
    public class OAuthAppProvider : OAuthAuthorizationServerProvider
    {
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
                {
                    var userName = context.UserName;
                    var password = context.Password;
                    var userService = new UserService();
                    GitHubUser user = userService.GetUSerByCredentials(userName, password);
                    if (user != null)
                    {
                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim("UserID", user.Id)
                        };
                        ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AutheticationType);
                        context.Validated(new AuthenticationTicket(oAuthIdentity, new AuthenticationProperrties() { }));
                    }
                    else
                    {
                        context.SetError("invalid_grnt", "Error");
                    }
                }


            )
        }
    }
}
