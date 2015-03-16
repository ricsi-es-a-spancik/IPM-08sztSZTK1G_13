using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ELTE.IssueR.Models;
using ELTE.IssueR.Models.Permissions;
using System.Drawing;
using System.IO;

namespace ELTE.IssueR.Controllers
{
    public class OrganizationController : BaseController
    {
        [HttpGet]
        [Authorize]
        public ActionResult Add()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            string userName = User.Identity.Name;
            if (_database.Users.First(user => user.UserName.Equals(userName)).Employees.Count != 0)
                return RedirectToAction("Index", "Home");
            else
                return View("Add");
        }

        [HttpPost]
        [Authorize]
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
            string userName = User.Identity.Name;
            string userId = _database.Users.First(user => user.UserName.Equals(userName)).Id;
            _database.Employees.Add(new Employee
            {
                UserId = userId,
                OrganizationId = orgId,
                Status = Models.Permissions.BasicPermissions.Administrator.Code
            });

            _database.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Details(int orgId)
        {
            var org = _database.Organizations.Find(orgId);
            var projects = _database.Projects.Where(p => p.OrganizationId == orgId);

            return View("Details", new OrganizationDetails { Org = org, Projects = projects });
        }

        [AllowAnonymous]
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
        [Authorize]
        public ActionResult UploadCoverImage(int orgId)
        {
            return View("UploadCoverImage", orgId);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UploadCoverImage(HttpPostedFileBase file, int orgId)
        {
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

        [AllowAnonymous]
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

        [HttpGet]
        [Authorize]
        public ActionResult AddMember(Int32 orgId)
        {
            NewEmployeeViewModel model = new NewEmployeeViewModel(orgId);

            return View("AddMember", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddMember(Int32 orgId, NewEmployeeViewModel model)
        {
            model.OrganizationId = orgId;
            // az adatok hibás formátumban kerültek megadásra
            if (!ModelState.IsValid)
            {
                return View("AddMember", model);
            }

            // nem létezik a felhasználó
            if (!_database.Users.Any(u => u.UserName == model.NewEmployeeUserName))
            {
                ModelState.AddModelError("", "A megadott néven nem szerepel felhasználó!");
                return View("AddMember", model);
            }

            // már tagja az orgnak
            string uid = _database.Users.First(u => u.UserName == model.NewEmployeeUserName).Id;
            if (_database.Employees.Any(e => e.UserId == uid && e.OrganizationId == model.OrganizationId))
            {
                ModelState.AddModelError("", "A megadott felhasználó már a Szervezet tagja!");
                return View("AddMember", model);
            }

            // dolgozó hozzáadása az adatbázishoz
            _database.Employees.Add(new Employee
            {
                UserId = uid,
                OrganizationId = model.OrganizationId,
                Status = Models.Permissions.BasicPermissions.Worker.Code
            });
            _database.SaveChanges();

            Log(Models.Logger.LogType.Organization, "Member added to ORG_ID: " + model.OrganizationId + ", USER_ID: " + uid);
            TempData["Information"] = "Sikeres felvétel.";
            return RedirectToAction("Details", new { orgId = model.OrganizationId });
        }

        [Authorize]
        public ActionResult EditMember(Int32 orgId)
        {
            List<EditableEmployees> ee = new List<EditableEmployees>();
            foreach(Employee e in _database.Employees.Where(e => e.OrganizationId == orgId))
            {
                EditableEmployees temp = new EditableEmployees 
                { 
                    Id = e.UserId,
                    Username = e.User.UserName,
                    Perm = new Models.Permissions.Permission(e.Status)
                };
                ee.Add(temp);
            }
            EditMembersViewModel model = new EditMembersViewModel
            {
                OrganizationId = orgId,
                Users = ee
            };

            return View("EditMember", model);
        }

        [Authorize]
        public ActionResult EditMemberPermissions(Int32 orgId, string userId)
        {
            Employee e = _database.Employees.First(emp => emp.OrganizationId == orgId && emp.UserId == userId);
            List<Models.Permissions.BasePermission> ps = Enum.GetValues(typeof(Models.Permissions.BasePermission)).Cast<Models.Permissions.BasePermission>().Where(bp => bp != BasePermission.None).ToList();

            return View("EditMemberPermissions", new EditMemberPermViewModel { Employee = e, AvailablePermissions = ps});
        }

        [Authorize]
        public ActionResult AddPermission(Int32 orgId, string userId, Models.Permissions.BasePermission perm)
        {
            Employee e = _database.Employees.First(emp => emp.OrganizationId == orgId && emp.UserId == userId);
            Models.Permissions.Permission p = new Models.Permissions.Permission(e.Status);
            p.AddPermission(perm);
            e.Status = p.Code;
            _database.SaveChanges();

            return RedirectToAction("EditMemberPermissions", new { orgId = orgId, userId = userId });
        }

        [Authorize]
        public ActionResult RemovePermission(Int32 orgId, string userId, Models.Permissions.BasePermission perm)
        {
            Employee e = _database.Employees.First(emp => emp.OrganizationId == orgId && emp.UserId == userId);
            Models.Permissions.Permission p = new Models.Permissions.Permission(e.Status);
            p.RemovePermission(perm);
            e.Status = p.Code;
            _database.SaveChanges();

            return RedirectToAction("EditMemberPermissions", new { orgId = orgId, userId = userId });
        }

        [Authorize]
        public ActionResult RemoveMember(Int32 orgId, string userId)
        {
            //Ellenőrizni, hogy valóban jogosult jutott-e ide
            String thisUserName = User.Identity.Name;
            Models.Permissions.Permission p = new Permission(_database.Users.First(u => u.UserName == thisUserName).Employees.Where(e => e.OrganizationId == orgId).First().Status);
            if(!p.HasPermission(Models.Permissions.BasePermission.RemoveMember))
                return RedirectToAction("Index", "Home");

            //Kitörölni a dolgozót
            Employee emp = _database.Employees.First(e => e.OrganizationId == orgId && e.UserId == userId);
            _database.Employees.Remove(emp);
            _database.SaveChanges();

            return RedirectToAction("EditMember", new { orgId = orgId });
        }

        [Authorize]
        public ActionResult AddOrgMember(int orgId)
        {
            string username = User.Identity.Name;

            string userId = _database.Users.First(user => user.UserName == username).Id;
            var org = _database.Organizations.Find(orgId);

            if (org != null && _database.Users.First(user => user.UserName.Equals(username)).Employees.Count == 0)
            {
                _database.Employees.Add(new Employee
                {
                    UserId = userId,
                    OrganizationId = orgId,
                    Status = Models.Permissions.BasicPermissions.Worker.Code
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

        public HtmlString GetPermission(Int32 orgId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new HtmlString(Models.Permissions.BasicPermissions.New.ToString());
            }

            String username = User.Identity.Name;
            string userId = _database.Users.First(user => user.UserName == username).Id;
            Employee emp = _database.Employees.Find(userId, orgId);
            if(emp != null)
            {
                return new HtmlString(new Permission(emp.Status).ToString());
            }
            else
            {
                return new HtmlString(Models.Permissions.BasicPermissions.New.ToString());
            }
        }

        public HtmlString HasPermission(Int32 orgId, Models.Permissions.BasePermission perm)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new HtmlString(false.ToString());
            }

            String username = User.Identity.Name;
            string userId = _database.Users.First(user => user.UserName == username).Id;
            Employee emp = _database.Employees.Find(userId, orgId);
            if (emp != null)
            {
                return new HtmlString(new Models.Permissions.Permission(emp.Status).HasPermission(perm).ToString());
            }
            else
            {
                return new HtmlString(false.ToString());
            }
        }

        public HtmlString IsOrgMember(int orgId)
        {
            if (!User.Identity.IsAuthenticated)
                return new HtmlString("");

            string username = User.Identity.Name;
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
    }
}