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

            ViewBag.Projects = _database.Users.First(u => u.UserName.Equals(User.Identity.Name)).ProjectMembers.Select(pmem => pmem.Project).ToList();

            if (selectedPrjId.HasValue)
            {
                listing.SelectedProjectId = selectedPrjId;
            }
            else if(ViewBag.Projects.Count != 0)
            {
                System.Diagnostics.Debug.WriteLine("prj count != 0");
                listing.SelectedProjectId = ViewBag.Projects[0].Id;
            }

            ViewBag.Issues = _database.Issues.Where(issue => issue.ProjectId == listing.SelectedProjectId).ToList();

            return View("ListIssues", listing);
        }

        [HttpGet, Authorize]
        public ActionResult CreateIssue(int? projId)
        {
            if (!_database.Projects.Any(p => p.Id == projId)) //project id IS NOT valid
            {
                return RedirectToAction("Index");
            }
            else //project id IS valid
            {
                Issue issue = new Issue();
                issue.ProjectId = projId.Value;

                SetViewBagSelectionLists(projId.Value);

                return View("CreateIssue", issue);
            }
        }

        [HttpPost, Authorize]
        public ActionResult CreateIssue(Issue issue)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Az űrlap hibás adatokat tartlamaz.");
                SetViewBagSelectionLists(issue.ProjectId);
                return View("CreateIssue", issue);
            }
            else
            {
                try
                {
                    _database.Issues.Add(issue);
                    _database.SaveChanges();
                }
                catch
                {
                    ModelState.AddModelError("", "A feladat mentése sikertelen! Próbálja újra!");
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
                SetViewBagSelectionLists(issue.ProjectId);
                return View("CreateIssue", issue);
            }
        }

        [HttpPost, Authorize]
        public ActionResult EditIssue(Issue issue)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Az űrlap hibás adatokat tartlamaz.");
                SetViewBagSelectionLists(issue.ProjectId);
                return View("CreateIssue", issue);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(issue.Id);
                Issue issueInDb = _database.Issues.First(i => i.Id == issue.Id);
                System.Diagnostics.Debug.WriteLine(issueInDb.Id);
                if(issueInDb == null)
                {
                    ModelState.AddModelError("", "A módosítani kívánt feladat nem található az adatbázisban!");
                    return RedirectToAction("ListIssues", new { selectedPrjId = issue.ProjectId });
                }
                else
                {
                    try
                    {
                        _database.Entry(issueInDb).CurrentValues.SetValues(issue);
                        _database.SaveChanges();
                    }
                    catch
                    {
                        ModelState.AddModelError("", "A feladat mentése sikertelen! Próbálja újra!");
                        return View("CreateIssue", issue);
                    }
                    return RedirectToAction("ListIssues", new { selectedPrjId = issue.ProjectId });
                }
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

        private void SetViewBagSelectionLists(int projId)
        {
            SetViewBagStatusSelectionList();
            SetViewBagTypeSelectionList();
            SetViewBagAssigneeSelectionList(projId);
        }

        private void SetViewBagAssigneeSelectionList(int projId)
        {
            ViewBag.AssigneeSelectionList = _database.ProjectMembers.Where(pmem => pmem.ProjectId == projId).
                Select(pmem => new SelectListItem{ Value = pmem.UserId, Text = pmem.User.Name});
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