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
            //TODO: csak organizationnak megfelelő projektek listázása
            return _database.Projects.ToList();
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

        private void UpdateListingState(IssueViewModel newModel)
        {
            newModel.Projects = GetProjects();

            if (newModel.CurrentProjectId == null && newModel.Projects.Count != 0)
                newModel.CurrentProjectId = newModel.Projects[0].Id;

            newModel.CurrentEpics = GetEpics(newModel.CurrentProjectId);
            newModel.CurrentIssues = GetIssues(newModel.CurrentEpics);

            /*if(newModel.CurrentProjectId != null)
            {
                newModel.CurrentEpics = GetEpics(newModel.CurrentProjectId);
                newModel.CurrentIssues = //_database.Issues.Where(issue => newModel.CurrentEpics.Exists(epic => epic.Id == issue.EpicId)).ToList();
                    _database.Issues.ToList().FindAll(issue => newModel.CurrentEpics.Exists(epic => epic.Id == issue.EpicId));
            }*/
        }

        /*private void UpdateAddingState(IssueState curState)
        {
            curState.Projects = GetProjects();
            if (curState.ProjectId == null && curState.Projects.Count != 0)
                curState.ProjectId = curState.Projects[0].Id;

            curState.Epics = GetEpics(curState.ProjectId);
            if (curState.EpicId == null && curState.Epics.Count != 0)
                curState.EpicId = curState.Epics[0].Id;
        }*/

        public ActionResult Index()
        {
            /*if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/

            return RedirectToAction("ListIssues");
        }

        [HttpGet]
        public ActionResult ListIssues()
        {
            /*if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/

            IssueViewModel newModel = new IssueViewModel();
            UpdateListingState(newModel);

            return View("ListIssues", newModel);
        }

        [HttpPost]
        public ActionResult ListIssues(IssueViewModel newModel)
        {
            /*if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/

            UpdateListingState(newModel);
            return View("ListIssues", newModel);
        }

        [HttpGet]
        public ActionResult AddIssue(int? selectedProjId)
        {
            /*if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/

            System.Diagnostics.Debug.WriteLine(selectedProjId);

            IssueState issue = new IssueState();
            issue.ProjectId = selectedProjId;
            issue.Epics = GetEpics(issue.ProjectId);
            if (issue.EpicId == null && issue.Epics.Count != 0)
                issue.EpicId = issue.Epics[0].Id;

            return View("AddIssue", issue);
        }

        [HttpPost]
        public ActionResult AddIssue(IssueState curState)
        {
            /*if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/
            
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Az űrlap hibás adatokat tartlamaz.");
                return View("AddIssue", curState);
            }

            return View("AddIssue");
        }
    }
}