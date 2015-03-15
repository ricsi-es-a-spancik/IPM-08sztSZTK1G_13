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

            List<Project> projects = _database.Users.First(u => u.UserName.Equals(User.Identity.Name)).ProjectMembers.Select(pmem => pmem.Project).ToList();

            if (selectedPrjId.HasValue && projects.Any(prj => prj.Id == selectedPrjId))
            {
                listing.SelectedProjectId = selectedPrjId;
            }
            else if(projects.Count != 0)
            {
                listing.SelectedProjectId = projects[0].Id;
            }

            ViewBag.Issues = _database.Issues.Where(issue => issue.ProjectId == listing.SelectedProjectId).ToList();
            ViewBag.Projects = projects;

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
            if(!issueId.HasValue)
            { 
                return RedirectToAction("Index"); 
            }
            else
            {
                Issue issue = _database.Issues.First(i => i.Id == issueId);

                SetViewBagForComments(issue);

                return View("Comments", new Comment { IssueId = issueId.Value });
            }
        }

        [HttpPost, Authorize]
        public ActionResult Comments(Comment comment)
        {
            Issue issue = _database.Issues.First(i => i.Id == comment.IssueId);

            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Hiba a hozzászólásban!");
                SetViewBagForComments(issue);
                return View("Comments", comment);
            }
            else
            {
                try
                {
                    comment.UserName = User.Identity.Name;
                    comment.SentAt = DateTime.Now;

                    _database.Comments.Add(comment);
                    _database.SaveChanges();
                }
                catch
                {
                    ModelState.AddModelError("", "Hiba történt a hozzászólás mentésekor!");
                    SetViewBagForComments(issue);
                    return View("Comments", comment);
                }

                //SetViewBagForComments(issue);
                return RedirectToAction("Comments", new { issueId = comment.IssueId });
                //return View("Comments", comment);
            }
        }

        private void SetViewBagForComments(Issue issue)
        {
            ViewBag.CurrentProject = issue.ProjectId;            
            ViewBag.Comments = issue.Comments.OrderBy(cm => cm.SentAt);
            ViewBag.IssueName = issue.Name;
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