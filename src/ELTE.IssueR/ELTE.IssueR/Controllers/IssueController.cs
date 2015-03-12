using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ELTE.IssueR.Models;

namespace ELTE.IssueR.Controllers
{
    public class IssueController : BaseController
    {
        private List<Project> GetProjects()
        {
            string username = (string)Session["userName"];

            List<Project> ret = new List<Project>();
            foreach(ProjectMember projMem in _database.Users.First(u => u.UserName.Equals(username)).ProjectMembers)
                ret.Add(projMem.Project);

            return ret;
        }

        private List<Issue> GetIssues(int projectId)
        {
            return _database.Issues.Where(issue => issue.ProjectId == projectId).ToList();
        }

        private List<User> GetProjectMembers(int projectId)
        {
            return _database.ProjectMembers.Where(conn => conn.ProjectId == projectId).Select(conn => conn.User).ToList();
        }

        private void UpdateListingState(IssueListingViewModel listing)
        {
            listing.Projects = GetProjects();

            if (listing.ProjectId == null && listing.Projects.Count != 0)
                listing.ProjectId = listing.Projects[0].Id;

            if(listing.ProjectId != null)
                listing.CurrentIssues = GetIssues(listing.ProjectId.Value);
        }

        private void UpdateAddingState(IssueViewModel issue)
        {
            if (issue.ProjectId != null)
            {
                issue.Users = GetProjectMembers(issue.ProjectId.Value);
                if (issue.UserId == null && issue.Users.Count != 0)
                    issue.UserId = issue.Users[0].Id;
            }
        }

        public ActionResult Index()
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("ListIssues");
        }

        [HttpGet]
        public ActionResult ListIssues()
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            IssueListingViewModel listing = new IssueListingViewModel();
            UpdateListingState(listing);

            return View("ListIssues", listing);
        }

        [HttpPost]
        public ActionResult ListIssues(IssueListingViewModel listing)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            UpdateListingState(listing);
            return View("ListIssues", listing);
        }

        [HttpGet]
        public ActionResult AddIssue(int? selectedProjId)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (selectedProjId == null || !GetProjects().Exists(prj => prj.Id == selectedProjId))
                return RedirectToAction("ListIssues");

            IssueViewModel issue = new IssueViewModel();
            issue.ProjectId = selectedProjId;
            UpdateAddingState(issue);

            return View("AddIssue", issue);
        }

        [HttpPost]
        public ActionResult AddIssue(IssueViewModel issue)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            UpdateAddingState(issue);
            
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Az űrlap hibás adatokat tartlamaz.");
                return View("AddIssue", issue);
            }

            if (_database.Issues.Count(i => issue.ProjectId == i.ProjectId && issue.Name.Equals(i.Name)) != 0)
            {
                ModelState.AddModelError("", "A megadott leírással már létezik feladat!");
                return View("AddIssue", issue);
            }

            _database.Issues.Add(new Issue
            {
                Name = issue.Name,
                Type = Convert.ToInt16(issue.Type),
                Status = Convert.ToInt16(IssueViewModel.StatusEnum.ToDo),
                Deadline = issue.Deadline,
                UserId = issue.UserId,
                ProjectId = issue.ProjectId.Value
            });

            _database.SaveChanges();

            return RedirectToAction("ListIssues", new IssueListingViewModel { ProjectId = issue.ProjectId });
        }

        [HttpGet]
        public ActionResult EditIssue(int? issueId)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            IssueViewModel issue = new IssueViewModel(_database.Issues.First(i => i.Id == issueId));
            
            UpdateAddingState(issue);
            return View("AddIssue", issue);
        }

        [HttpPost]
        public ActionResult EditIssue(IssueViewModel issue)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            UpdateAddingState(issue);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Az űrlap hibás adatokat tartlamaz.");
                return View("AddIssue", issue);
            }

            if (_database.Issues.Count(i => issue.ProjectId == i.ProjectId && issue.Name.Equals(i.Name) && issue.Id != i.Id) != 0)
            {
                ModelState.AddModelError("", "A megadott leírással már létezik feladat!");
                return View("AddIssue", issue);
            }

            Issue oldData = _database.Issues.First(i => i.Id == issue.Id);
            oldData.Name = issue.Name;
            oldData.Type = Convert.ToInt16((int)issue.Type);
            oldData.Status = Convert.ToInt16((int)issue.Status);
            oldData.UserId = issue.UserId;
            if(issue.Deadline != null)
                oldData.Deadline = issue.Deadline;

            _database.SaveChanges();

            return RedirectToAction("ListIssues", new IssueListingViewModel { ProjectId = issue.ProjectId });
        }
    }
}