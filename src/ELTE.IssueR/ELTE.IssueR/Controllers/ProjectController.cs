using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ELTE.IssueR.Models;

namespace ELTE.IssueR.Controllers
{
    public class ProjectController : BaseController
    {
        [HttpGet]
        public ActionResult Project()
        {
            return View("Project");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Project(ProjectViewModel pvm)
        {
            if (!ModelState.IsValid)
            {
                return View("Project", pvm);
            }

            User user = _database.Users.FirstOrDefault(u => u.UserName == Session["userName"]);
            Employee currentUser = _database.Employees.FirstOrDefault(e => e.UserId == user.Id);

            return View("Project");
        }
	}
}