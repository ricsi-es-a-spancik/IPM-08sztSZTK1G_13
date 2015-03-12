using ELTE.IssueR.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ELTE.IssueR.Controllers
{
    public class BaseController : Controller
    {
        protected Models.ApplicationDbContext _database;
        protected IUserStore<User> userStore;
        protected UserManager<User> userManager;

        public BaseController()
        {
            _database = new Models.ApplicationDbContext();
            userStore = new UserStore<User>(_database);
            userManager = new UserManager<User>(userStore);
        }

        public void Log(ELTE.IssueR.Models.Logger.LogType t, String msg)
        {
            //Multithread
            
            System.Threading.Tasks.Task.Run(() => ELTE.IssueR.Models.Logger.Logger.Log(t, msg, Server.MapPath));
        }
    }
}