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
            User user = _database.Users.First(u => u.UserName.Equals(username));
            if (user.Employees.Count == 0)
                return new List<Project>();
            else
            {
                int orgId = user.Employees.First().OrganizationId;
                return _database.Projects.Where(prj => prj.OrganizationId == orgId).ToList();
            }
        }

        private List<Epic> GetEpics(int? projectId)
        {
            if (projectId != null)
                return _database.Epics.Where(epic => epic.ProjectId == projectId).ToList();
            else
                return new List<Epic>();
        }

        private List<Issue> GetIssues(int? epicId)
        {
            if (epicId != null)
                return _database.Issues.Where(issue => issue.EpicId == epicId).ToList();
            else
                return new List<Issue>();
        }

        private List<Issue> GetIssues(List<Epic> epics)
        {
            List<Issue> ret = new List<Issue>();
            foreach (Epic epic in epics)
            {
                ret.AddRange(GetIssues(epic.Id));
            }

            return ret;
        }

        private List<Employee> GetEmployees(int? projectId)
        {
            if (projectId == null)
                return new List<Employee>();
            else
            {
                int organizationId = _database.Projects.FirstOrDefault(prj => prj.Id == projectId).OrganizationId;
                return _database.Employees.Where(emp => emp.OrganizationId == organizationId).ToList();
            }
        }

        private void UpdateListingState(IssueViewModel newModel)
        {
            newModel.Projects = GetProjects();

            if (newModel.CurrentProjectId == null && newModel.Projects.Count != 0)
                newModel.CurrentProjectId = newModel.Projects[0].Id;

            newModel.CurrentEpics = GetEpics(newModel.CurrentProjectId);
            newModel.CurrentIssues = GetIssues(newModel.CurrentEpics);
        }

        private void UpdateAddingState(IssueState issue)
        {
            issue.Epics = GetEpics(issue.ProjectId);
            if (issue.EpicId == null && issue.Epics.Count != 0)
                issue.EpicId = issue.Epics[0].Id;

            issue.Employees = GetEmployees(issue.ProjectId);
            if (issue.EmployeeId == null && issue.Employees.Count != 0)
                issue.EmployeeId = issue.Employees[0].Id;
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

            IssueViewModel newModel = new IssueViewModel();
            UpdateListingState(newModel);

            return View("ListIssues", newModel);
        }

        [HttpPost]
        public ActionResult ListIssues(IssueViewModel newModel)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            UpdateListingState(newModel);
            return View("ListIssues", newModel);
        }

        [HttpGet]
        public ActionResult AddIssue(int? selectedProjId)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (selectedProjId == null)
                return RedirectToAction("ListIssues");

            if (!GetProjects().Exists(prj => prj.Id == selectedProjId))
                return RedirectToAction("ListIssues");

            IssueState issue = new IssueState();
            issue.ProjectId = selectedProjId;
            UpdateAddingState(issue);

            return View("AddIssue", issue);
        }

        [HttpPost]
        public ActionResult AddIssue(IssueState issue)
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

            if (_database.Issues.Count(i => issue.Name.Equals(i.Name)) != 0)
            {
                ModelState.AddModelError("", "A megadott leírással már létezik feladat!");
                return View("AddIssue", issue);
            }

            _database.Issues.Add(new Issue
            {
                Name = issue.Name,
                Type = (int)issue.Type,
                Status = (int)IssueState.StatusEnum.ToDo,
                Deadline = issue.Deadline,
                EpicId = (int)issue.EpicId,
                EmployeeId = (int)issue.EmployeeId
            });

            _database.SaveChanges();

            return RedirectToAction("ListIssues", new IssueViewModel { CurrentProjectId = issue.ProjectId });
        }

        [HttpGet]
        public ActionResult EditIssue(int? issueId)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            IssueState issue = new IssueState(_database.Issues.First(i => i.Id == issueId));
            
            UpdateAddingState(issue);
            return View("AddIssue", issue);
        }

        public ActionResult EditIssue(IssueState issue)
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

            if (_database.Issues.Count(i => issue.Name.Equals(i.Name) && issue.Id != i.Id) != 0)
            {
                ModelState.AddModelError("", "A megadott leírással már létezik feladat!");
                return View("AddIssue", issue);
            }

            Issue oldData = _database.Issues.First(i => i.Id == issue.Id);
            oldData.Name = issue.Name;
            oldData.Type = (int)issue.Type;
            oldData.Status = (int)issue.Status;
            oldData.EpicId = issue.EpicId.Value;
            oldData.EmployeeId = issue.EmployeeId.Value;
            oldData.Deadline = issue.Deadline;

            _database.SaveChanges();

            return RedirectToAction("ListIssues", new IssueViewModel { CurrentProjectId = issue.ProjectId });
        }
    }
}