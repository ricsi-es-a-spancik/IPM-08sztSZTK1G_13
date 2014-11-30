using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using ELTE.IssueR.WebClient.Models;

namespace WebClient.Controllers
{
    public class IssueController : BaseController
    {
        private IssueViewModel issuevm = null;

        public ActionResult Index()
        {
            issuevm = new IssueViewModel(_database);
            ViewData.Add("projectlist", issuevm.Projects);
            return View("Index", issuevm);
        }
    }
}