using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ELTE.IssueR.Models;

namespace ELTE.IssueR.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        [AllowAnonymous]
        public ActionResult Index()
        {
            // Select a few organizations to show
            List<Organization> orgs = _database.Organizations.Take(9).ToList();

            return View("Index", orgs);
        }
	}
}