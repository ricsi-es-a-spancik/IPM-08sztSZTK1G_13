using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ELTE.IssueR.Models;

namespace ELTE.IssueR.Controllers
{
    public class OrganizationController : BaseController
    {
        //
        // GET: /Organization/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Register", "Account");
            }
            else
            {
                return View("Add");
            }
        }

        [HttpPost]
        public ActionResult Add(OrganizationViewModel orgViewModel)
        {
            // az adatok hibás formátumban kerültek megadásra
            if (!ModelState.IsValid)
            {
                return View("Add", orgViewModel);
            }

            // egyedi a vállalat neve?
            if (_database.Organizations.Count(org => org.Name == orgViewModel.Name) != 0)
            {
                ModelState.AddModelError("", "A megadott néven már szerepel egy vállalat a rendszerünkben!");
                return View("Add", orgViewModel);
            }

            // alapítás éve helyes?
            var currentYear = System.DateTime.Today.Year;
            if(orgViewModel.FoundationYear < 0 || orgViewModel.FoundationYear > currentYear)
            {
                ModelState.AddModelError("", String.Format("Az alapítás éve nem lehet kisebb, mint 0, vagy nagyobb, mint a {0}!", currentYear));
                return View("Add", orgViewModel);
            }

            // vállalat hozzáadása az adatbázishoz
            _database.Organizations.Add(new Organization
            {
                Name = orgViewModel.Name,
                FoundationYear = orgViewModel.FoundationYear,
                Country = orgViewModel.Country,
                City = orgViewModel.City,
                Activity = orgViewModel.Activity,
                Description = orgViewModel.Description
            });

            _database.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(int orgId)
        {
            return View("Details", _database.Organizations.Find(orgId));
        }

        public ActionResult Search(string orgName)
        {
            if(orgName == string.Empty)
            {
                ViewBag.SearchWithEmptyString = true;
                return View("SearchResults");
            }
            else
            {
                List<Organization> searchResult = _database.Organizations.Where(org => org.Name.Contains(orgName)).ToList();
                return View("SearchResults", searchResult);
            }
        }
	}
}