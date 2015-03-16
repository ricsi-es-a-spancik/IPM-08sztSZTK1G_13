using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using ELTE.IssueR.Models;
using ELTE.IssueR.Models.Account;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;

namespace ELTE.IssueR.Controllers
{
    public partial class AccountController : BaseController
    {

        #region General

        public HtmlString IdToName(string p_id)
        {
            if (Session["userName"] == null)
            {
                return new HtmlString("");
            }

            String result;

            User usr = _database.Users.FirstOrDefault(u => u.Id == p_id);
            if (usr == null)
                result = "";
            else
                result = usr.Name;

            return new HtmlString(result);
        }

        public HtmlString MyId()
        {
            if (Session["userName"] == null)
            {
                return new HtmlString("");
            }

            String result;

            String myName = (String)Session["userName"];
            User me = _database.Users.FirstOrDefault(x => x.UserName == myName);

            result = me.Id.ToString();

            return new HtmlString(result);
        }

        #endregion

        #region Login/Logout/Register

        /// <summary>
        /// Default bejelentkezés
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Bejelentkezés.
        /// </summary>
        [HttpGet]
        public ActionResult Login()
        {
            return View("Login");
        }

        /// <summary>
        /// Bejelentkezés.
        /// </summary>
        /// <param name="user">A felhasználó adatai.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UserViewModel user)
        {
            if (!ModelState.IsValid)
            {
                //Nem adta meg valamelyiket.
                ModelState.AddModelError("", "A bejelentkezés sikertelen!");
                return View("Login", user);
            }

            User registered = await userManager.FindAsync(user.UserName, user.UserPassword);

            if (registered == null)
            {
                //Nem regisztrált felhasználónév
                ModelState.AddModelError("", "Hibás felhasználónév, vagy jelszó.");
                return View("Login", user);
            }

            // beléptetés
            var userIdentity = await userManager.CreateIdentityAsync(registered, DefaultAuthenticationTypes.ApplicationCookie);
            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = user.RememberMe }, userIdentity);

            //Utolsó belépés frissítése
            registered.LastLogin = DateTime.Now;

            _database.SaveChanges();

            //Application adatok letárolása
            if (HttpContext.Application["usersList"] == null)
            {
                HttpContext.Application["usersList"] = new List<String>();
                ((List<String>)HttpContext.Application["usersList"]).Add(registered.UserName);
            }
            else
            {
                if (((List<String>)HttpContext.Application["usersList"]).Contains(registered.UserName))
                {
                    //SKIP
                }
                else
                {
                    ((List<String>)HttpContext.Application["usersList"]).Add(registered.UserName);
                }
            }

            Log(ELTE.IssueR.Models.Logger.LogType.Account, "User " + registered.UserName + " logged in.");
            return RedirectToAction("Index", "Home"); // átirányítjuk a főoldalra
        }

        /// <summary>
        /// Regisztráció.
        /// </summary>
        [HttpGet]
        public ActionResult Register()
        {
            return View("Register");
        }

        /// <summary>
        /// Regisztráció.
        /// </summary>
        /// <param name="customer">Regisztrációs adatok.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(NewUserViewModel registering)
        {
            //Formai követelmények ellenőrzése
            if (!ModelState.IsValid)
                return View("Register", registering);

            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

            if (String.IsNullOrEmpty(recaptchaHelper.Response))
            {
                ModelState.AddModelError("", "A Captcha szöveg nem lehet üres.");
                return View("Register", registering);
            }

            RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

            if (recaptchaResult != RecaptchaVerificationResult.Success)
            {
                ModelState.AddModelError("", "Helytelen Captcha szöveget írtál be.");
                return View("Register", registering);
            }

            // Létezik-e már a megadott felhasználó név
            var user = await userManager.FindAsync(registering.UserName, registering.UserPassword);

            if (user != null)
            {
                ModelState.AddModelError("UserName", "A megadott felhasználónév már létezik.");
                return View("Register", registering);
            }

            // elmentjük a felhasználó adatait
            user = new User
            {
                UserName = registering.UserName,
                Email = registering.Email,
                Name = registering.Name,
                RegisterDate = DateTime.Now,
                LastLogin = null
            };

            var result = await userManager.CreateAsync(user, registering.UserPassword);

            if(!result.Succeeded)
            {
                AddErrors(result);
            }

            ViewBag.Information = "A regisztráció sikeres volt. Kérjük, jelentkezzen be.";

            //Ha korábban be volt jelentkezve, akkor kijelentkeztetjük
            if (Session["userName"] != null)
            {
                String tempname = (String)Session["userName"];
                Session.Remove("userName");
                if (HttpContext.Application["usersList"] != null)
                {
                    ((List<String>)HttpContext.Application["usersList"]).Remove(tempname);
                }
            }

            Log(ELTE.IssueR.Models.Logger.LogType.Account,"Successful Register (" + registering.UserName + ")");
            return RedirectToAction("Login");            
        }

        /// <summary>
        /// Kijelentkezés.
        /// </summary>
        [Authorize]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            Log(ELTE.IssueR.Models.Logger.LogType.Account, "User " + User.Identity.Name + " has logged out.");
            return RedirectToAction("Index", "Home"); // átirányítjuk a főoldalra
        }

        #endregion

        #region Profile


        public ActionResult ProfileIndex()
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("ProfileIndex");
        }

        [Authorize]
        public ActionResult GetProfileImage()
        {
            // Get image of authenticated user
            string userId = User.Identity.GetUserId();
            UserImage img = _database.UserImages.FirstOrDefault(x => x.UserId == userId);

            if(img == null)
                return File("~/Content/NoImage.png", "image/png");

            // image url?
            if (img.ImageUrl != null)
                return Content(img.ImageUrl);

            // return bytearray as file content
            Byte[] imageContent = img.Image;

            if (imageContent == null) // amennyiben nem sikerült betölteni, egy alapértelmezett képet adunk vissza
                return File("~/Content/NoImage.png", "image/png");

            return File(imageContent, "image/png");
        }

        [HttpGet]
        public ActionResult AccountSettings()
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            String myName = (String)Session["userName"];
            User me = _database.Users.FirstOrDefault(x => x.UserName == myName);

            AccountSettingsViewModel model = new AccountSettingsViewModel(me);

            return View("AccountSettings", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccountSettings(AccountSettingsViewModel model)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Hibás adatok.");
                return View("AccountSettings", model);
            }

            String myName = (String)Session["userName"];
            User me = _database.Users.FirstOrDefault(x => x.UserName == myName);
            model.Id = me.Id;

            if (model.Email != me.Email)
            {
                me.Email = model.Email;
                ViewBag.Information = "Email megváltozott.";
            }

            if (model.UserPassword != null && model.UserPassword != "")
            {
                // kódoljuk a jelszót
                var hasher = new PasswordHasher();

                me.PasswordHash = hasher.HashPassword(model.UserPassword);
                ViewBag.Information += " Jelszó megváltozott.";
            }

            //Real name
            if (model.Name != me.Name)
            {
                me.Name = model.Name;
                ViewBag.Information += " Név megváltozott.";
            }

            _database.SaveChanges();

            ViewBag.Information += " A módosítás sikeres volt.";
            Log(Models.Logger.LogType.Account, "Profile successfully changed by: " + myName + ".");

            return View("AccountSettings", model);
        }

        [HttpGet]
        public ActionResult UploadProfileImage()
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("UploadProfileImage");
        }

        [HttpPost]
        public ActionResult UploadProfileImage(HttpPostedFileBase file)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            String myName = (String)Session["userName"];

            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                string ImageName = System.IO.Path.GetFileName(file.FileName);
                string physicalPath = Server.MapPath("~/App_Data/temp_upload/" + ImageName);

                // save image in folder
                file.SaveAs(physicalPath);
                System.Web.Helpers.WebImage img = new System.Web.Helpers.WebImage(physicalPath);

                if (img.Width > 2000)
                    img.Resize(2000, 2000, true, true);

                //save new record in database
                User me = _database.Users.FirstOrDefault(x => x.UserName == myName);
                UserImage usrimg = _database.UserImages.FirstOrDefault(x => x.UserId == me.Id);
                if (usrimg == null)
                {
                    usrimg = new UserImage();
                    usrimg.UserId = me.Id;
                    img.Resize(100, 100, true, true);
                    usrimg.Image = img.GetBytes();
                    _database.UserImages.Add(usrimg);
                }
                else
                {
                    img.Resize(100, 100, true, true);
                    usrimg.Image = img.GetBytes();
                }

                _database.SaveChanges();

                System.IO.File.Delete(physicalPath);

                ViewBag.Information += " A feltöltés sikeres volt.";
                Log(ELTE.IssueR.Models.Logger.LogType.Account, "User " + myName + " has successfully uploaded an image.");
            }
            else
            {
                ViewBag.Information += " A feltöltés sikertelen volt.";
                Log(ELTE.IssueR.Models.Logger.LogType.Account, "User " + myName + " has encountered an error during uploading an image.");
            }

            return RedirectToAction("ProfileIndex");
        }

        #endregion

        #region Messages

        [Authorize]
        public ActionResult Mails()
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("Mails");
        }

        public HtmlString NewMails()
        {
            if (Session["userName"] == null)
            {
                return new HtmlString("");
            }

            String result;

            String myName = (String)Session["userName"];
            User me = _database.Users.FirstOrDefault(u => u.UserName == myName);

            result = _database.Messages.Where(x => x.ToId == me.Id && !x.IsRead).ToList().Count.ToString();

            return new HtmlString(result);
        }

        public ActionResult MailsReceive()
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            String myName = (String)Session["userName"];
            User me = _database.Users.FirstOrDefault(u => u.UserName == myName);

            MessageViewModel vm = new MessageViewModel();

            vm.Messages.AddRange(_database.Messages.Where(x => x.ToId == me.Id && !x.HideFromTarget).ToList());

            return View("MailsReceive", vm);
        }

        public ActionResult MailsSent()
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            String myName = (String)Session["userName"];
            User me = _database.Users.FirstOrDefault(u => u.UserName == myName);

            MessageViewModel vm = new MessageViewModel();

            vm.Messages.AddRange(_database.Messages.Where(x => x.FromId == me.Id && !x.HideFromSender).ToList());

            return View("MailsSent", vm);
        }

        public ActionResult ReadMail(Int32 p_id)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            String myName = (String)Session["username"];
            User me = _database.Users.FirstOrDefault(x => x.UserName == myName);

            Message msg = _database.Messages.FirstOrDefault(x => x.Id == p_id);
            if (msg.ToId == me.Id)
                msg.IsRead = true;

            _database.SaveChanges();

            return View("ReadMail", msg);
        }

        public ActionResult DeleteMail(Int32 p_id)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Message msg = _database.Messages.FirstOrDefault(x => x.Id == p_id);

            return View("DeleteMail", msg);
        }

        public ActionResult DeleteMailConfirm(Int32 p_id)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Message msg = _database.Messages.FirstOrDefault(x => x.Id == p_id);
            String myName = (String)Session["userName"];
            User me = _database.Users.FirstOrDefault(u => u.UserName == myName);

            if (msg.FromId == me.Id)
            {
                msg.HideFromSender = true;
            }
            else
            {
                msg.HideFromTarget = true;
                msg.IsRead = true;
            }

            if (msg.HideFromSender && msg.HideFromTarget)
            {
                _database.Messages.Remove(msg);
            }
            _database.SaveChanges();


            Log(Models.Logger.LogType.Account, "Mail deleted by: " + myName + ".");
            return View("Mails", msg);
        }

        public ActionResult NewMail()
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            String myName = (String)Session["userName"];
            List<string> ids = _database.Users.Where(x => x.UserName != myName).Select(x => x.Id).ToList();

            return View("NewMailList", ids);
        }

        [HttpGet]
        public ActionResult NewMailTo(Int32 p_id)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            NewMessageViewModel vm = new NewMessageViewModel(p_id);

            return View("NewMail", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewMailTo(NewMessageViewModel model, string p_id)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Hibás adatok.");
                return View("NewMail", model);
            }

            String myName = (String)Session["userName"];
            User me = _database.Users.FirstOrDefault(u => u.UserName == myName);
            User to = _database.Users.FirstOrDefault(u => u.Id == p_id);

            _database.Messages.Add(new Message
            {
                Subject = model.Subject,
                Content = model.Content,
                FromId = me.Id,
                ToId = p_id,
                IsRead = false
            });

            _database.SaveChanges();
            Log(Models.Logger.LogType.Account, "New Mail sent from: " + myName + ", to: " + to.UserName + ".");

            return RedirectToAction("Mails");
        }

        #endregion
    }
}