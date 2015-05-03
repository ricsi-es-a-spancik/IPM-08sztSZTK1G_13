using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace ELTE.IssueR.Controllers
{
    using ELTE.IssueR.Models;
    using Filter = ELTE.IssueR.Models.Filter;

    public class FilterController : BaseController
    {
        [HttpGet, Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Issue");
        }

        [HttpGet, Authorize]
        public ActionResult OrganizeFilters(int? projectId)
        {
            if (!projectId.HasValue || !_database.IsProjectMember(User.Identity.GetUserId(), projectId.Value))
            {
                return RedirectToAction("Index");
            }
            else
            {
                var filters = _database.Filters.Where(f => f.ProjectId == projectId.Value).ToList();
                filters.ForEach(f => {
                    f.UserNames = _database.GetUsers(f).Select(u => u.Name);
                    f.TypeTexts = f.DeserializedTypes.Select(t => Utility.GetEnumValueAsString(t));
                    f.StatusTexts = f.DeserializedStatuses.Select(s => Utility.GetEnumValueAsString(s));
                });

                ViewBag.ProjectId = projectId.Value;
                
                return View("OrganizeFilters", filters);
            }
        }

        [HttpGet, Authorize]
        public ActionResult AddFilter(int? projectId)
        {
            if(!projectId.HasValue || !_database.IsProjectMember(User.Identity.GetUserId(), projectId.Value)) 
            {
                return RedirectToAction("Index");
            } 
            else
            {
                try
                {
                    SetViewData(projectId.Value);
                }
                catch //no project found with the given id
                {
                    return RedirectToAction("Index");
                }

                return View("FilterForm", new Filter { ProjectId = projectId.Value });
            }
        }

        [HttpPost, Authorize]
        public ActionResult AddFilter(Filter filter)
        {
            if (!ModelState.IsValid)
            {
                SetViewData(filter.ProjectId);
                ModelState.AddModelError("", "Az űrlap hibás adatokat tartlamaz.");
                return View("FilterForm", filter);
            }
            else if(!filter.IsValid())
            {
                SetViewData(filter.ProjectId);
                ModelState.AddModelError("", "Nincs szűrési feltétel megadva!");
                return View("FilterForm", filter);
            }
            else
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine(filter.DeadlineInterval.HasValue);
                    _database.Filters.Add(filter);
                    _database.SaveChanges();
                }
                catch
                {
                    ModelState.AddModelError("", "A szűrő mentése sikertelen! Próbálja újra!");
                    return View("FilterForm", filter);
                }

                return RedirectToAction("OrganizeFilters", new { projectId = filter.ProjectId });
            }
        }

        [HttpGet, Authorize]
        public ActionResult ModifyFilter(int? filterId)
        {
            Filter filter = _database.Filters.First(f => f.Id == filterId);
            if (!filterId.HasValue || !_database.IsProjectMember(User.Identity.GetUserId(), filter.ProjectId))
            {
                return RedirectToAction("Index");
            }
            else
            {
                if(filter == null)
                {
                    return RedirectToAction("Index");
                }

                try
                {
                    SetViewData(filter.ProjectId);
                }
                catch //no project found with the given id
                {
                    return RedirectToAction("Index");
                }

                return View("FilterForm", filter);
            }
        }

        [HttpPost, Authorize]
        public ActionResult ModifyFilter(Filter filter)
        {
            if (!ModelState.IsValid)
            {
                SetViewData(filter.ProjectId);
                ModelState.AddModelError("", "Az űrlap hibás adatokat tartlamaz.");
                return View("FilterForm", filter);
            }
            else if (!filter.IsValid())
            {
                SetViewData(filter.ProjectId);
                ModelState.AddModelError("", "Nincs szűrési feltétel megadva!");
                return View("FilterForm", filter);
            }
            else
            {
                try
                {
                    Filter filterInDb = _database.Filters.First(f => f.Id == filter.Id);
                    _database.Entry(filterInDb).CurrentValues.SetValues(filter);
                    _database.SaveChanges();
                }
                catch
                {
                    ModelState.AddModelError("", "A szűrő mentése sikertelen! Próbálja újra!");
                    return View("FilterForm", filter);
                }

                return RedirectToAction("OrganizeFilters", new { projectId = filter.ProjectId });
            }
        }

        [HttpGet, Authorize]
        public ActionResult RemoveFilter(int? filterId)
        {
            if(!filterId.HasValue)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Filter filterToRemove = _database.Filters.First(f => f.Id == filterId);

                if(filterToRemove == null || !_database.IsProjectMember(User.Identity.GetUserId(), filterToRemove.ProjectId))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    _database.Filters.Remove(filterToRemove);
                    _database.SaveChanges();

                    return RedirectToAction("OrganizeFilters", new { projectId = filterToRemove.ProjectId });
                }
            }
        }

        private void SetViewData(int projectId)
        {
            var users = _database.GetUsersOnProject(projectId);
            ViewBag.Users = new MultiSelectList(users, "Id", "Name");
            ViewBag.Types = new MultiSelectList(Utility.GetEnumSelectionList<IssueType>(), "Value", "Text");
            ViewBag.Statuses = new MultiSelectList(Utility.GetEnumSelectionList<IssueStatus>(), "Value", "Text");
        }
    }
}