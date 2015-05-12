using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ELTE.IssueR.Models;
using ELTE.IssueR.Models.Wiki;

namespace ELTE.IssueR.Controllers
{
    public class WikiController : BaseController
    {
        [Authorize]
        public ActionResult Index(String section, Int32 id)
        {
            WikiViewModel model = new WikiViewModel();
            model.Section = section;
            model.Id = id;

            if(section == "Organization")
            {
                model.Docs = _database.OrganizationDocuments.Where(doc => doc.OrganizationId == id)
                    .ToDictionary(doc => doc.Name, doc => doc.Id);
            }
            else if(section == "Project")
            {
                model.Docs = _database.ProjectDocuments.Where(doc => doc.ProjectId == id)
                    .ToDictionary(doc => doc.Name, doc => doc.Id);
            }

            return View("Index", model);
        }

        [Authorize]
        public ActionResult ViewProjDocument(Int32 id)
        {
            ProjectDocument model = _database.ProjectDocuments.FirstOrDefault(doc => doc.Id == id);

            return View("ViewProjDocument", model);
        }

        [Authorize]
        public ActionResult ViewOrgDocument(Int32 id)
        {
            OrganizationDocument model = _database.OrganizationDocuments.FirstOrDefault(doc => doc.Id == id);

            return View("ViewOrgDocument", model);
        }

        [Authorize]
        public ActionResult NewWikiPage(String section, Int32 id)
        {
            if (section == "Organization")
            {
                return RedirectToAction("NewOrgWikiPage", new { id = id });
            }
            else if (section == "Project")
            {
                return RedirectToAction("NewProjWikiPage", new { id = id });
            }

            return RedirectToAction("Index", section);
        }

        #region Organization wiki

        [Authorize]
        [HttpGet]
        public ActionResult NewOrgWikiPage(Int32 id)
        {
            OrganizationDocument model = new OrganizationDocument();
            model.OrganizationId = id;

            return View("NewOrgWikiPage", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult NewOrgWikiPage(OrganizationDocument model, Int32 id)
        {
            model.Content = removeScripts(model.Content);

            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Hibás adatok.");
                return View("NewOrgWikiPage", model);
            }

            model.Author = User.Identity.Name;
            model.Modified = DateTime.Now;

            _database.OrganizationDocuments.Add(model);
            _database.SaveChanges();

            return RedirectToAction("Index", new { section = "Organization", id = model.OrganizationId });
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditOrgWikiPage(Int32 id)
        {
            OrganizationDocument model = _database.OrganizationDocuments.FirstOrDefault(doc => doc.Id == id);

            return View("EditOrgWikiPage", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditOrgWikiPage(OrganizationDocument model)
        {
            model.Content = removeScripts(model.Content);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Hibás adatok.");
                return View("EditOrgWikiPage", model);
            }

            model.Modified = DateTime.Now;

            _database.OrganizationDocuments.Remove(
                _database.OrganizationDocuments.FirstOrDefault(doc => doc.Id == model.Id));
            _database.OrganizationDocuments.Add(model);
            _database.SaveChanges();

            return RedirectToAction("Index", new { section = "Organization", id = model.OrganizationId });
        }

        [Authorize]
        public ActionResult RemoveOrgWikiPage(Int32 id)
        {
            OrganizationDocument document = _database.OrganizationDocuments.FirstOrDefault(doc => doc.Id == id);
            _database.OrganizationDocuments.Remove(document);
            _database.SaveChanges();

            return RedirectToAction("Index", new { section = "Organization", id = document.OrganizationId });
        }

        #endregion

        #region Project Wiki

        [Authorize]
        [HttpGet]
        public ActionResult NewProjWikiPage(Int32 id)
        {
            ProjectDocument model = new ProjectDocument();
            model.ProjectId = id;

            return View("NewProjWikiPage", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult NewProjWikiPage(ProjectDocument model, Int32 id)
        {
            model.Content = removeScripts(model.Content);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Hibás adatok.");
                return View("NewProjWikiPage", model);
            }

            model.Author = User.Identity.Name;
            model.Modified = DateTime.Now;

            _database.ProjectDocuments.Add(model);
            _database.SaveChanges();

            return RedirectToAction("Index", new { section = "Project", id = model.ProjectId });
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditProjWikiPage(Int32 id)
        {
            ProjectDocument model = _database.ProjectDocuments.FirstOrDefault(doc => doc.Id == id);

            return View("EditProjWikiPage", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditProjWikiPage(ProjectDocument model)
        {
            model.Content = removeScripts(model.Content);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Hibás adatok.");
                return View("EditProjWikiPage", model);
            }

            model.Modified = DateTime.Now;

            _database.ProjectDocuments.Remove(
                _database.ProjectDocuments.FirstOrDefault(doc => doc.Id == model.Id));
            _database.ProjectDocuments.Add(model);
            _database.SaveChanges();

            return RedirectToAction("Index", new { section = "Project", id = model.ProjectId });
        }

        [Authorize]
        public ActionResult RemoveProjWikiPage(Int32 id)
        {
            ProjectDocument document = _database.ProjectDocuments.FirstOrDefault(doc => doc.Id == id);
            _database.ProjectDocuments.Remove(document);
            _database.SaveChanges();

            return RedirectToAction("Index", new { section = "Project", id = document.ProjectId });
        }

        #endregion

        private String removeScripts(String input)
        {
            return input.Replace("script", "IDE_CSÚNYA_DOLGOT_AKARTAM_ÍRNI!");
        }
    }
}