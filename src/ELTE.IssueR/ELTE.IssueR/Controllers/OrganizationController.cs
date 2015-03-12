using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ELTE.IssueR.Models;
using System.Drawing;
using System.IO;

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
                string userName = Session["userName"].ToString();
                if (_database.Users.First(user => user.UserName.Equals(userName)).Employees.Count != 0)
                    return RedirectToAction("Index", "Home");
                else
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
            if (orgViewModel.FoundationYear < 0 || orgViewModel.FoundationYear > currentYear)
            {
                ModelState.AddModelError("", String.Format("Az alapítás éve nem lehet kisebb, mint 0, vagy nagyobb, mint a {0}!", currentYear));
                return View("Add", orgViewModel);
            }

            // vállalat hozzáadása az adatbázishoz
            int orgId = _database.Organizations.Add(new Organization
            {
                Name = orgViewModel.Name,
                FoundationYear = orgViewModel.FoundationYear,
                Country = orgViewModel.Country,
                City = orgViewModel.City,
                Activity = orgViewModel.Activity,
                Description = orgViewModel.Description
            }).Id;

            // jelenlegi felhasználó hozzáadása a létrehozott vállalathoz
            string userName = Session["userName"].ToString();
            string userId = _database.Users.First(user => user.UserName.Equals(userName)).Id;
            _database.Employees.Add(new Employee
            {
                UserId = userId,
                OrganizationId = orgId,
                Status = 1
            });

            _database.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(int orgId)
        {
            var org = _database.Organizations.Find(orgId);
            var projects = _database.Projects.Where(p => p.OrganizationId == orgId);

            return View("Details", new OrganizationDetails { Org = org, Projects = projects });
        }

        public ActionResult Search(string orgName)
        {
            if (orgName == string.Empty)
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

        [HttpGet]
        public ActionResult UploadCoverImage(int orgId)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("UploadCoverImage", orgId);
            }
        }

        [HttpPost]
        public ActionResult UploadCoverImage(HttpPostedFileBase file, int orgId)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                string ImageName = System.IO.Path.GetFileName(file.FileName);
                string physicalPath = Server.MapPath("~/App_Data/temp_upload/" + ImageName);

                // save image in folder
                file.SaveAs(physicalPath);
                System.Web.Helpers.WebImage img = new System.Web.Helpers.WebImage(physicalPath);

                // check if image has the required dimensions
                if ((double)img.Width / (double)img.Height != 2.00)
                {
                    ViewBag.ImageError = "A kép szélessége nem a magasság kétszerese!";
                    return View("UploadCoverImage", orgId);
                }

                try
                {
                    // delete previous images
                    var prevCovers = _database.CoverImages.Where(c => c.OrganizationId == orgId);
                    foreach (var cover in prevCovers)
                    {
                        _database.CoverImages.Remove(cover);
                    }

                    // add new images
                    img.Resize(1000, 500, true, true);
                    _database.CoverImages.Add(new CoverImage { OrganizationId = orgId, Image = img.GetBytes() });

                    _database.SaveChanges();
                }
                catch
                {
                    ViewBag.ProcessError = "A kép feltöltése során hiba történt!";
                    return View("UploadCoverImage", orgId);
                }

                System.IO.File.Delete(physicalPath);

                TempData["Information"] = " A feltöltés sikeres volt.";
                return RedirectToAction("Details", new { orgId = orgId });
            }
            else
            {
                ViewBag.ProcessError = "A kép feltöltése során hiba történt!";
                return View("UploadCoverImage", orgId);
            }
        }

        public FileResult CoverFor(int orgId, bool thumb)
        {
            IEnumerable<CoverImage> images = _database.Organizations.Where(org => org.Id == orgId).Select(org => org.CoverImages).FirstOrDefault();

            if (images.Count() == 0)
            {
                return File("/Images/blank_cover.jpg", "image/jpg");
            }
            else
            {
                byte[] imageContent = images.First().Image;

                if (!thumb)
                {
                    return File(imageContent, "image/jpg");
                }
                else
                {
                    Image img = Image.FromStream(new MemoryStream(imageContent));
                    Bitmap bmp = new Bitmap(img, new Size(300, 150));
                    ImageConverter converter = new ImageConverter();
                    return File((byte[])converter.ConvertTo(bmp, typeof(byte[])), "image/jpg");
                }
            }
        }

        public ActionResult AddOrgMember(int orgId)
        {
            if (Session["userName"] != null)
            {
                string username = Session["userName"].ToString();

                string userId = _database.Users.First(user => user.UserName == username).Id;
                var org = _database.Organizations.Find(orgId);

                if (org != null && _database.Users.First(user => user.UserName.Equals(username)).Employees.Count == 0)
                {
                    _database.Employees.Add(new Employee
                    {
                        UserId = userId,
                        OrganizationId = orgId,
                        Status = 1
                    });
                    _database.SaveChanges();

                    TempData["Information"] = "Csatlakoztál a vállalathoz.";
                    return RedirectToAction("Details", new { orgId = orgId });
                }
                else
                {
                    TempData["Information"] = "Sikertelen csatlakozás.";
                    return RedirectToAction("Details", new { orgId = orgId });
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public HtmlString IsOrgMember(int orgId)
        {
            if (Session["userName"] != null)
            {
                string username = Session["userName"].ToString();
                var userId = _database.Users.First(user => user.UserName == username).Id;
                var emp = _database.Employees.Find(userId, orgId);

                if (emp != null)
                {
                    return new HtmlString("true");
                }
                else
                {
                    return new HtmlString("false");
                }
            }
            else
            {
                return new HtmlString("");
            }
        }
    }
}