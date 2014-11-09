using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebClient.Controllers
{
    public class BaseController : Controller
    {
        protected Models.IssueREntities _database;

        public BaseController()
        {
            _database = new Models.IssueREntities();
        }

    }
}