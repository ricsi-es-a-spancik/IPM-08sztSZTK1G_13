using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class IssueController : BaseController
    {
        IssueViewModel issuevm;

        public ActionResult Index()
        {
            issuevm = new IssueViewModel(_database);
            return View("Index", issuevm);
        }

        public ActionResult SelectProject(int prjid)
        {
            issuevm = new IssueViewModel(_database);
            issuevm.CurrentProjectId = prjid;
            issuevm.CurrentProject = issuevm.Projects.Find(prj => prj.Id == prjid);
            return View("Index", issuevm);
        }
    }
}