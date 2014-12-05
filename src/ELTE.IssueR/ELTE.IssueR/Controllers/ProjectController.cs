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
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("Project");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Project(ProjectViewModel pvm)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View("Project", pvm);
            }

            User user = _database.Users.FirstOrDefault(u => u.UserName == Session["userName"]);
            Employee currentUser = _database.Employees.FirstOrDefault(e => e.UserId == user.Id);

            _database.Projects.Add(new Project {
                OrganizationId = currentUser.OrganizationId,
                Name = pvm.Name,
                Description = pvm.Description,
                Deadline = pvm.Deadline
            });
            _database.SaveChanges();

            ViewBag.Information = "A projekt sikeresen létrejött.";

            return RedirectToAction("Project");
        }

        public ActionResult ProjectList(ProjectListViewModel plvm)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            plvm.ProjectList = _database.Projects.ToList();
            return View("ProjectList", plvm);   
        }

        public ActionResult ProjectData(int id)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
        
            return RedirectToAction("Project");
        }
	}
}