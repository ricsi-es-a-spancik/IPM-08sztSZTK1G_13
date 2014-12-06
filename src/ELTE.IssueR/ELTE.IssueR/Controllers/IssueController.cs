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
        private void UpdateModel(IssueViewModel newModel)
        {
            newModel.Projects = _database.Projects.ToList();

            if (newModel.CurrentProjectId == null && newModel.Projects.Count != 0)
                newModel.CurrentProjectId = newModel.Projects[0].Id;

            if(newModel.CurrentProjectId != null)
            {
                newModel.CurrentEpics = _database.Epics.Where(epic => epic.ProjectId == newModel.CurrentProjectId).ToList();
                newModel.CurrentIssues = _database.Issues.ToList().FindAll(issue => newModel.CurrentEpics.Exists(epic => epic.Id == issue.EpicId));
            }
        }

        public ActionResult Index()
        {
            return RedirectToAction("ListIssues");
        }

        [HttpGet]
        public ActionResult ListIssues()
        {
            IssueViewModel newModel = new IssueViewModel();
            UpdateModel(newModel);

            return View("ListIssues", newModel);
        }

        [HttpPost]
        public ActionResult ListIssues(IssueViewModel newModel)
        {
            UpdateModel(newModel);
            return View("ListIssues", newModel);
        }
    }
}