using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ELTE.IssueR.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Filter = ELTE.IssueR.Models.Filter;

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

            List<Project> projects = _database.Users.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name)).ProjectMembers.Select(pmem => pmem.Project).ToList();

            if (selectedPrjId.HasValue && projects.Any(prj => prj.Id == selectedPrjId))
            {
                if (!IsProjectMember(selectedPrjId.Value))
                {
                    return RedirectToAction("Index");
                }

                listing.SelectedProjectId = selectedPrjId;
            }
            else if(projects.Count != 0)
            {
                listing.SelectedProjectId = projects[0].Id;
            }

            listing.Filters = _database.GetFilters(listing.SelectedProjectId.Value);

            ViewBag.Issues = _database.Issues.Where(issue => issue.ProjectId == listing.SelectedProjectId).ToList();
            ViewBag.Projects = projects;

            return View("ListIssues", listing);
        }

        [HttpGet, Authorize]
        public ActionResult CreateIssue(int? projId)
        {
            if (!_database.Projects.Any(p => p.Id == projId) || !IsProjectMember(projId.Value)) //project id IS NOT valid
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
            Issue issue = _database.Issues.FirstOrDefault(i => i.Id == issueId);

            if (issue == null || !IsProjectMember(issue.ProjectId))
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
                Issue issueInDb = _database.Issues.FirstOrDefault(i => i.Id == issue.Id);
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
            if (!projectId.HasValue || !issueId.HasValue || !IsProjectMember(projectId.Value))
            {
                return RedirectToAction("Index");
            }
            else
            {
                Issue issueToRemove = _database.Issues.FirstOrDefault(issue => issue.Id == issueId);

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
                Issue issue = _database.Issues.FirstOrDefault(i => i.Id == issueId);

                if (!IsProjectMember(issue.ProjectId))
                {
                    return RedirectToAction("Index");
                }

                SetViewBagForComments(issue);

                return View("Comments", new Comment { IssueId = issueId.Value });
            }
        }

        [HttpPost, Authorize]
        public ActionResult Comments(Comment comment)
        {
            Issue issue = _database.Issues.FirstOrDefault(i => i.Id == comment.IssueId);

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

                return RedirectToAction("Comments", new { issueId = comment.IssueId });
            }
        }

        [HttpPost, Authorize]
        public ActionResult Filter(IssueListingViewModel vm)
        {
            if(String.IsNullOrEmpty(vm.FilterText))
            {
                return RedirectToAction("Index");
            }

            System.Diagnostics.Debug.WriteLine(vm.SelectedProjectId.HasValue + " " + vm.FilterText);

            List<Project> projects = _database.Users.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name)).ProjectMembers.Select(pmem => pmem.Project).ToList();
            List<Issue> issues = _database.Issues.Where(issue => issue.ProjectId == vm.SelectedProjectId).ToList();

            List<Issue> filteredIssues = issues.Where(i => LowerSubStr(i.Name, vm.FilterText) ||
                LowerSubStr(i.Description, vm.FilterText) || LowerEq(i.Status.ToString(), vm.FilterText) ||
                LowerEq(i.Type.ToString(), vm.FilterText)).ToList();

            string[] filterWords = vm.FilterText.Split(' ');
            foreach(string word in filterWords)
            {
                filteredIssues.AddRange(issues.Where(i => !filteredIssues.Contains(i) && 
                    (LowerSubStr(i.Name, vm.FilterText) || LowerSubStr(i.Description, vm.FilterText))));
            }
            
            ViewBag.Issues = filteredIssues;
            ViewBag.Projects = projects;
            return View("ListIssues", vm);
        }

        [HttpGet, Authorize]
        public ActionResult CustomFilter(int? filterId)
        {
            if(!filterId.HasValue)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Filter filter = _database.Filters.FirstOrDefault(f => f.Id == filterId.Value);

                if(filter == null || !_database.IsProjectMember(User.Identity.GetUserId(), filter.ProjectId))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    IssueListingViewModel listing = new IssueListingViewModel();

                    listing.SelectedProjectId = filter.ProjectId;

                    string userId = User.Identity.GetUserId();
                    List<Project> projects = _database.ProjectMembers.Where(pm => pm.UserId == userId)
                        .Select(pm => pm.Project).ToList();

                    listing.Filters = _database.GetFilters(listing.SelectedProjectId.Value);
                    Filter ff = listing.Filters.FirstOrDefault(f => f.Id == filterId.Value);
                    ff.IsActive = true;

                    List<Issue> issues = _database.Issues.Where(issue => issue.ProjectId == listing.SelectedProjectId).ToList();
                    issues.RemoveAll(i => 
                           (filter.DeserializedUserIds.Any() && !filter.DeserializedUserIds.Contains(i.UserId))
                        || (filter.DeserializedTypes.Any() && !filter.DeserializedTypes.Contains(i.Type))
                        || (filter.DeserializedStatuses.Any() && !filter.DeserializedStatuses.Contains(i.Status))
                        || (i.Deadline.HasValue && (i.Deadline.Value - DateTime.Now).Days <= filter.DeadlineInterval));

                    ViewBag.Projects = projects;
                    ViewBag.Issues = issues;

                    return View("ListIssues", listing);
                }
            }
        }

        private bool LowerSubStr(string a, string b)
        {
            return String.IsNullOrEmpty(a) ? false : a.ToLower().Contains(b.ToLower());
        }

        private bool LowerEq(string a, string b)
        {
            return String.IsNullOrEmpty(a) ? false : a.ToLower().Equals(b.ToLower());
        }

        private bool IsProjectMember(int projId)
        {
            return _database.ProjectMembers.Any(pmem => pmem.ProjectId == projId && pmem.User.UserName == User.Identity.Name);
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

        // Charts

        [Authorize]
        public ActionResult Charts(int PrjId)
        {
            var issuesByType = from i
                                in _database.Issues
                                where i.ProjectId == PrjId
                                group i by i.Type into g
                                select new { TypeId = g.Key, IssuesCount = g.Count() };

            var typeData = new List<PieChartData>();

            foreach (var issueGroup in issuesByType.ToList())
            {
                typeData.Add(new PieChartData(issueGroup.IssuesCount, "#F7464A", "#FF5A5E", issueGroup.TypeId.ToString()));
            }

            var issuesByStatus = from i
                                 in _database.Issues
                                 where i.ProjectId == PrjId
                                 group i by i.Status into g
                                 select new { StatusId = g.Key, IssuesCount = g.Count() };

            var statusData = new List<PieChartData>();

            foreach (var issueGroup in issuesByStatus.ToList())
            {
                statusData.Add(new PieChartData(issueGroup.IssuesCount, "#F7464A", "#FF5A5E", issueGroup.StatusId.ToString()));
            }

            return View(new ChartsViewModel { TypePieChart = typeData, StatusPieChart = statusData });
        }
    }
}