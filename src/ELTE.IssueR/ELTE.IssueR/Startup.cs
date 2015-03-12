using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Facebook;

[assembly: OwinStartup(typeof(ELTE.IssueR.Startup))]

namespace ELTE.IssueR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure Facebook authentication
            var x = new FacebookAuthenticationOptions();

            x.AppId = "1590065547872851";
            x.AppSecret = "a12e10a7f8abe32a2d7337b960b3fb37";

            x.Provider = new FacebookAuthenticationProvider()
            {
                OnAuthenticated = async context =>
                {
                    //Get the access token from FB and store it in the database and
                    //use FacebookC# SDK to get more information about the user
                    context.Identity.AddClaim(
                    new System.Security.Claims.Claim("FacebookAccessToken",
                                                         context.AccessToken));
                }
            };

            app.UseFacebookAuthentication(x);

            // Configura Google authentication
            var googleOpts = new GoogleOAuth2AuthenticationOptions();

            googleOpts.ClientId = "531665303176-bmi7su3lq61d51l1fcsfbsopgd3vg6kj.apps.googleusercontent.com";
            googleOpts.ClientSecret = "8KtlDjASkX-VzNPm3gPmbgRD";
            googleOpts.Scope.Add("profile");
            googleOpts.Scope.Add("email");

            googleOpts.Provider = new GoogleOAuth2AuthenticationProvider()
            {
                OnAuthenticated = async context =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("GoogleAccessToken", context.AccessToken));
                    }
            };

            app.UseGoogleAuthentication(googleOpts);
        }
    }
}
