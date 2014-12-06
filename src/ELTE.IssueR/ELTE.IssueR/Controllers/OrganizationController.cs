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
            return View("Add");
        }

        [HttpPost]
        public ActionResult Add(OrganizationViewModel orgViewModel)
        {
            if (!ModelState.IsValid)
            {
                //Nem adta meg valamelyiket.
                ModelState.AddModelError("", "Az űrlap hibás adatokat tartlamaz.");
                return View("Add", orgViewModel);
            }

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
	}
}