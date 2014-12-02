using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ELTE.IssueR.Controllers
{
    public class BaseController : Controller
    {
        protected Models.IssueREntities _database;

        public BaseController()
        {
            _database = new Models.IssueREntities();
            
        }

        public void Log(ELTE.IssueR.Models.Logger.LogType t, String msg)
        {
            //Multithread
            
            System.Threading.Tasks.Task.Run(() => ELTE.IssueR.Models.Logger.Logger.Log(t, msg, Server.MapPath));
        }
    }
}