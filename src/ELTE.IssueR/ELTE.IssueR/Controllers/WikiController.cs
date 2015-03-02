using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ELTE.IssueR.Models.Wiki;

namespace ELTE.IssueR.Controllers
{
    public class WikiController : BaseController
    {
        public ActionResult Index(String section)
        {
            WikiViewModel model = new WikiViewModel();

            //TODO

            return View("Index", model);
        }

        public ActionResult ViewDocument(Int32 id)
        {
            DocumentsViewModel model = new DocumentsViewModel();

            //TODO get data about document from DB
            // build model

            return View("ViewDocument", model);
        }

    }
}