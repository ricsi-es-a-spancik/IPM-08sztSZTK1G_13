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

        private void UpdateIssueListing(IssueListingViewModel model)
        {
            ViewBag.Projects = GetProjects();

            if (model.SelectedProjectId == null && ViewBag.Projects.Count != 0)
                model.SelectedProjectId = ViewBag.Projects[0].Id;

            if (model.SelectedProjectId != null)
                ViewBag.Issues = GetIssues(model.SelectedProjectId.Value);
        }

        private void UpdateIssue(IssueViewModel issue)
        {
            if (issue.ProjectId != null)
            {
                issue.Users = GetProjectMembers(issue.ProjectId.Value);
                if (issue.UserId == null && issue.Users.Count != 0)
                    issue.UserId = issue.Users[0].Id;
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

                UpdateIssueListing(listing);
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
                UpdateIssueListing(listing);
                return View("ListIssues", listing);
            }
        }

        [HttpGet]
        public ActionResult CreateIssue(int? projId)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            
            if(!_database.Projects.Any(p => p.Id == projId))
            {
                return RedirectToAction("Index");
            }
            else
            {
                IssueViewModel issue = new IssueViewModel();
                issue.ProjectId = projId;
                UpdateIssue(issue);
                return View("CreateIssue", issue);
            }
        }

        [HttpPost]
        public ActionResult CreateIssue(IssueViewModel issue)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Az űrlap hibás adatokat tartlamaz.");
                UpdateIssue(issue);
                return View("CreateIssue", issue);
            }
            else if (_database.Issues.Count(i => issue.ProjectId == i.ProjectId && issue.Name.Equals(i.Name)) != 0)
            {
                ModelState.AddModelError("", "A megadott leírással már létezik feladat!");
                UpdateIssue(issue);
                return View("CreateIssue", issue);
            }
            else
            {
                try
                {
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
                }
                catch
                {
                    ModelState.AddModelError("", "Nem sikerült elmenteni a feladatot!");
                }

                return RedirectToAction("ListIssues", new { selectedPrjId = issue.ProjectId });
            }
        }

        public ActionResult EditIssue(int? issueId)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Issue issue = _database.Issues.First(i => i.Id == issueId);

            if (issue == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                IssueViewModel issueVm = new IssueViewModel(issue);
                UpdateIssue(issueVm);
                return View("EditIssue", issueVm);
            }
        }

        [HttpPost]
        public ActionResult EditIssue(IssueViewModel issue)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Az űrlap hibás adatokat tartlamaz.");
                UpdateIssue(issue);
                return View("CreateIssue", issue);
            }
            else if (_database.Issues.Count(i => issue.ProjectId == i.ProjectId && issue.Name.Equals(i.Name)) != 0)
            {
                ModelState.AddModelError("", "A megadott leírással már létezik feladat!");
                UpdateIssue(issue);
                return View("CreateIssue", issue);
            }
            else
            {
                Issue issueInDb = _database.Issues.First(i => i.Id == issue.Id);

                if(issueInDb == null)
                {
                    return RedirectToAction("ListIssues", new { selectedPrjId = issue.ProjectId });
                }
                else
                {
                    
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

                ModelState.AddModelError("", "Nem sikerült elmenteni a feladatot!");

                return RedirectToAction("ListIssues", new { selectedPrjId = issue.ProjectId });
            }
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