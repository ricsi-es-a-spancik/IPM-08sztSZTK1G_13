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
        [HttpGet, Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("ListIssues");
        }

        [HttpGet, Authorize]
        public ActionResult ListIssues(int? selectedPrjId)
        {
            IssueListingViewModel listing = new IssueListingViewModel();
            if (selectedPrjId.HasValue)
            {
                listing.SelectedProjectId = selectedPrjId;
            }

            UpdateIssueListing(listing);
            return View("ListIssues", listing);
        }

        [HttpPost, Authorize]
        public ActionResult ListIssues(IssueListingViewModel listing)
        {
            UpdateIssueListing(listing);
            return View("ListIssues", listing);
        }

        [HttpGet, Authorize]
        public ActionResult CreateIssue(int? projId)
        {
            if(!_database.Projects.Any(p => p.Id == projId))
            {
                return RedirectToAction("Index");
            }
            else
            {
                IssueViewModel issue = new IssueViewModel();
                issue.ProjectId = projId;
                UpdateIssue(issue);

                SetViewBagStatusSelectionList();
                SetViewBagTypeSelectionList();

                return View("CreateIssue", issue);
            }
        }

        [HttpPost, Authorize]
        public ActionResult CreateIssue(IssueViewModel issue)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Az űrlap hibás adatokat tartlamaz.");
                UpdateIssue(issue);
                SetViewBagStatusSelectionList();
                SetViewBagTypeSelectionList();
                return View("CreateIssue", issue);
            }
            else if (_database.Issues.Count(i => issue.ProjectId == i.ProjectId && issue.Name.Equals(i.Name)) != 0)
            {
                ModelState.AddModelError("", "A megadott leírással már létezik feladat!");
                UpdateIssue(issue);
                SetViewBagStatusSelectionList();
                SetViewBagTypeSelectionList();
                return View("CreateIssue", issue);
            }
            else
            {
                try
                {
                    _database.Issues.Add(new Issue
                    {
                        Name = issue.Name,
                        Type = IssueType.Bug,//Convert.ToInt16(issue.Type),
                        Status = IssueStatus.Done,//Convert.ToInt16(IssueViewModel.StatusEnum.ToDo),
                        Deadline = issue.Deadline,
                        UserId = issue.UserId,
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

        [HttpGet, Authorize]
        public ActionResult EditIssue(int? issueId)
        {
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

        [HttpPost, Authorize]
        public ActionResult EditIssue(IssueViewModel issue)
        {
            if (!ModelState.IsValid)
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
                    Type = IssueType.Bug,//Convert.ToInt16(issue.Type),
                    Status = IssueStatus.Done,//Convert.ToInt16(IssueViewModel.StatusEnum.ToDo),
                    Deadline = issue.Deadline,
                    UserId = issue.UserId,
                    ProjectId = issue.ProjectId.Value
                });

                _database.SaveChanges();

                ModelState.AddModelError("", "Nem sikerült elmenteni a feladatot!");

                return RedirectToAction("ListIssues", new { selectedPrjId = issue.ProjectId });
            }
        }

        [HttpGet, Authorize]
        public ActionResult RemoveIssue(int? projectId, int? issueId)
        {
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

        [HttpGet, Authorize]
        public ActionResult Comments(int? issueId)
        {
            if (User.Identity.Name == null)
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

        private IEnumerable<SelectListItem> statusSelectionList = null;
        private IEnumerable<SelectListItem> typeSelectionList = null;

        /// <summary>
        /// </summary>
        /// <returns>The projects which are associated with the current user.</returns>
        private List<Project> GetProjects()
        {
            string userId = User.Identity.Name;

            List<Project> ret = _database.Users.First(u => u.UserName.Equals(userId)).ProjectMembers.Aggregate(
                new List<Project>(), (list, pmem) => { list.Add(pmem.Project); return list; });

            return ret;
        }

        /// <summary>
        /// </summary>
        /// <param name="projectId">The currently selected project's id.</param>
        /// <returns>The issues which are associated with the given project.</returns>
        private List<Issue> GetIssues(int projectId)
        {
            return _database.Issues.Where(issue => issue.ProjectId == projectId).ToList();
        }

        /// <summary>
        /// </summary>
        /// <param name="projectId">The currently selected project's id.</param>
        /// <returns>The users which are associated with the given project.</returns>
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
            if (issue.ProjectId.HasValue)
            {
                issue.Users = GetProjectMembers(issue.ProjectId.Value);
                if (issue.UserId == null && issue.Users.Count != 0)
                    issue.UserId = issue.Users[0].Id;
            }
        }

        private void SetViewBagStatusSelectionList()
        {
            if (statusSelectionList == null)
                statusSelectionList = SetViewBagSelectionList(IssueStatus.To_do);

            ViewBag.StatusSelectionList = statusSelectionList;
        }

        private void SetViewBagTypeSelectionList()
        {
            if(typeSelectionList == null)
                typeSelectionList = SetViewBagSelectionList(IssueType.Feature);

            ViewBag.TypeSelectionList = typeSelectionList;
        }

        private IEnumerable<SelectListItem> SetViewBagSelectionList<TEnum>(TEnum selectedValue) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            //return new SelectList(Enum.GetValues(typeof(TEnum)).Cast<TEnum>());
            
            IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            IEnumerable<SelectListItem> items =
                from value in values
                select new SelectListItem
                {
                    Text = value.ToString().Replace('_', ' '),
                    Value = value.ToString(),
                    Selected = value.Equals(selectedValue)
                };

            return items;
        }
    }
}