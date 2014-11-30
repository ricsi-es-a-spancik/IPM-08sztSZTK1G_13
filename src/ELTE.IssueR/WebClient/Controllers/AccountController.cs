using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using WebClient.Models;
using ELTE.IssueR.WebClient.Models.Account;

namespace WebClient.Controllers
{
    public class AccountController : BaseController
    {

        #region General

        private byte[] Salt(byte[] name, byte[] pw)
        {
            byte[] result = pw;

            for (Int32 i = 0; i < pw.Length; i = i + 3)
            {
                result[i] = name[i % name.Length];

                if (i % 5 == 0)
                    --i;
                if (i % 7 == 0)
                    --i;
            }

            byte[] reverse = result.Reverse().ToArray();

            for (Int32 i = 0; i < result.Length; ++i)
            {
                result[i] += reverse[i];
            }

            return result;
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
        public ActionResult Login(UserViewModel user)
        {
            if (!ModelState.IsValid)
            {
                //Nem adta meg valamelyiket.
                ModelState.AddModelError("", "A bejelentkezés sikertelen!");
                return View("Login", user);
            }

            User registered = _database.Users.FirstOrDefault(x => x.UserName == user.UserName);

            if (registered == null)
            {
                //Nem regisztrált felhasználónév
                ModelState.AddModelError("", "Hibás felhasználónév, vagy jelszó.");
                return View("Login", user);
            }

            // ellenőrizzük a jelszót (ehhez a kapott jelszót hash-elem és sózom)
            Byte[] passwordBytes = null;
            using (SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider())
            {
                passwordBytes = Salt(Encoding.UTF8.GetBytes(user.UserName), provider.ComputeHash(Encoding.UTF8.GetBytes(user.UserPassword)));
            }

            if (!passwordBytes.SequenceEqual(registered.Password))
            {
                //Nem egyezik meg a jelszó
                ModelState.AddModelError("", "Hibás felhasználónév, vagy jelszó.");
                return View("Login", user);
            }

            //Utolsó belépés frissítése
            registered.LastLogin = DateTime.Now;

            _database.SaveChanges();

            //Session adatok letárolása
            Session["userName"] = registered.UserName;
            Session.Timeout = 20; // max. 20 percig él a munkamenet

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

            Log(ELTE.IssueR.WebClient.Models.Logger.LogType.Account, "User " + registered.UserName + " logged in.");
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
        public ActionResult Register(NewUserViewModel registering)
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
            if (_database.Users.Count(c => c.UserName == registering.UserName) != 0)
            {
                ModelState.AddModelError("UserName", "A megadott felhasználónév már létezik.");
                return View("Register", registering);
            }

            // Kódoljuk a jelszót
            Byte[] passwordBytes = null;
            using (SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider())
            {
                passwordBytes = Salt(Encoding.UTF8.GetBytes(registering.UserName), provider.ComputeHash(Encoding.UTF8.GetBytes(registering.UserPassword)));
            }

            // elmentjük a felhasználó adatait
            _database.Users.Add(new User
            {
                UserName = registering.UserName,
                Email = registering.Email,
                Password = passwordBytes,
                Name = registering.Name,
                RegisterDate = DateTime.Now,
                LastLogin = null
            });
            _database.SaveChanges();

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

            Log(ELTE.IssueR.WebClient.Models.Logger.LogType.Account,"Successful Register (" + registering.UserName + ")");
            return RedirectToAction("Login");            
        }

        /// <summary>
        /// Kijelentkezés.
        /// </summary>
        public ActionResult Logout()
        {
            if (Session["userName"] != null)
            {
                String usrname = (String)Session["userName"];
                Session.Remove("userName");

                if (HttpContext.Application["usersList"] != null)
                    ((List<String>)HttpContext.Application["usersList"]).Remove(usrname);

                Log(ELTE.IssueR.WebClient.Models.Logger.LogType.Account, "User " + usrname + " has logged out.");
            }
            else
            {
                Log(ELTE.IssueR.WebClient.Models.Logger.LogType.Bug, "User has logged out without being logged in.");
            }

            return RedirectToAction("Index", "Home"); // átirányítjuk a főoldalra
        }

        #endregion

        #region Profile

        public ActionResult ProfileIndex()
        {


            return View("ProfileIndex");
        }

        #endregion

        #region Messages



        #endregion


    }
}