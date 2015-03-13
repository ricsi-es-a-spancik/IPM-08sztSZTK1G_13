using ELTE.IssueR.Models;
using ELTE.IssueR.Models.Account;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ELTE.IssueR.Controllers
{
    public partial class AccountController : BaseController
    {
        [HttpPost]
        [AllowAnonymous]
        // [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var signInManager = new SignInManager<Models.User, string>(userManager, AuthenticationManager);
            var result = await signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    var identity = await AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);

                    // Get public information external APIs
                    // NOW: assemble a url to the user's profile picture

                    string pictureUrl = string.Empty;
                    var user = User.Identity.GetUserId();

                    switch (loginInfo.Login.LoginProvider)
                    {
                        case "Facebook":
                            var fbAccesToken = identity.FindAll("FacebookAccessToken").First().Value;
                            var fb = new FacebookClient(fbAccesToken);
                            dynamic fbMeInfo = fb.Get("/me");
                            pictureUrl = String.Format("http://graph.facebook.com/{0}/picture?type=large", fbMeInfo.id);
                            break;
                        case "Google":
                            var googleAccessToken = identity.FindAll("GoogleAccessToken").First().Value;
                            Uri apiRequestUri = new Uri(String.Format("https://www.googleapis.com/oauth2/v2/userinfo?access_token={0}", googleAccessToken));
                            using (var webClient = new System.Net.WebClient())
                            {
                                var json = webClient.DownloadString(apiRequestUri);
                                dynamic googleMeInfo = JsonConvert.DeserializeObject(json);
                                pictureUrl = googleMeInfo.picture;
                            }
                            break;
                        default:
                            break;
                    }

                    // return a confirmation view model to validate external user informations
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel
                    {
                        Name = loginInfo.ExternalIdentity.Name,
                        Email = loginInfo.Email,
                        PictureUrl = pictureUrl
                    });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                var id = User.Identity.GetUserId();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }

                var user = new User { UserName = model.Email, Email = model.Email, Name = model.Name };
                
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        _database.UserImages.Add(new UserImage { UserId = user.Id, ImageUrl = model.PictureUrl });
                        _database.SaveChanges();

                        var signInManager = new SignInManager<User, string>(userManager, HttpContext.GetOwinContext().Authentication);
                        await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        #region Helpers

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #endregion
    }
}