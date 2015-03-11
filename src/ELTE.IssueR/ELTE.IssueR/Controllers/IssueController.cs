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

        private void UpdateState(IssueListingViewModel model)
        {
            ViewBag.Projects = GetProjects();

            if (model.SelectedProjectId == null && ViewBag.Projects.Count != 0)
                model.SelectedProjectId = ViewBag.Projects[0].Id;

            if (model.SelectedProjectId != null)
                ViewBag.Issues = GetIssues(model.SelectedProjectId.Value);

            if (model.Issue == null)
                model.Issue = new IssueViewModel { ProjectId = model.SelectedProjectId };

            if (model.Issue.ProjectId != null)
            {
                model.Issue.Users = GetProjectMembers(model.Issue.ProjectId.Value);
                if (model.Issue.UserId == null && model.Issue.Users.Count != 0)
                    model.Issue.UserId = model.Issue.Users[0].Id;
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("ListIssues");
        }

        [HttpGet]
        public ActionResult ListIssues(int? selectedPrjId)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else 
            {
                IssueListingViewModel listing = new IssueListingViewModel();
                if (selectedPrjId.HasValue)
                {
                    listing.SelectedProjectId = selectedPrjId;
                }

                UpdateState(listing);
                return View("ListIssues", listing);
            }
        }

        [HttpPost]
        public ActionResult ListIssues(IssueListingViewModel listing)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                UpdateState(listing);
                return View("ListIssues", listing);
            }
        }

        [HttpPost]
        public ActionResult CreateIssue(IssueViewModel issue)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Az űrlap hibás adatokat tartlamaz.");
                IssueListingViewModel vm = new IssueListingViewModel{Issue = issue, SelectedProjectId = issue.ProjectId};
                UpdateState(vm);
                return View("ListIssues", vm);
            }

            if (_database.Issues.Count(i => issue.ProjectId == i.ProjectId && issue.Name.Equals(i.Name)) != 0)
            {
                ModelState.AddModelError("", "A megadott leírással már létezik feladat!");
                IssueListingViewModel vm = new IssueListingViewModel { Issue = issue, SelectedProjectId = issue.ProjectId };
                UpdateState(vm);
                return View("ListIssues", vm);
            }

            _database.Issues.Add(new Issue
            {
                Name = issue.Name,
                Type = Convert.ToInt16(issue.Type),
                Status = Convert.ToInt16(IssueViewModel.StatusEnum.ToDo),
                Deadline = issue.Deadline,
                UserId = issue.UserId.Value,
                ProjectId = issue.ProjectId.Value
            });

            _database.SaveChanges();

            return RedirectToAction("ListIssues", new { selectedPrjId = issue.ProjectId });
        }

        [HttpGet]
        public ActionResult RemoveIssue(int? projectId, int? issueId)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if(!projectId.HasValue || !issueId.HasValue)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Issue issueToRemove = _database.Issues.First(issue => issue.Id == issueId);
                _database.Issues.Remove(issueToRemove);
                _database.SaveChanges();

                return RedirectToAction("ListIssues", new { selectedPrjId = projectId });
            }
        }

        [HttpGet]
        public ActionResult Comments(int? issueId)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if(issueId.HasValue)
            {
                Issue issue = _database.Issues.First(i => i.Id == issueId);

                ViewBag.CurrentProjectId = issue.ProjectId;

                //TODO: add comments to database
                //ViewBag.Comments = issue.Comments.ToList();
                List<String> comments = new List<String> { 
                    "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                    "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                    "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                    "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                };

                ViewBag.Comments = comments;

                return View("Comments", new CommentsViewModel { /*IssueId = issueId.Value*/ });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}