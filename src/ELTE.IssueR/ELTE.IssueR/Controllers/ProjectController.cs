using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ELTE.IssueR.Models;

namespace ELTE.IssueR.Controllers
{
    public class ProjectController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {/*
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/

            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProjectViewModel pvm)
        {/*
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/

            if (!ModelState.IsValid)
            {
                return View("Index", pvm);
            }

            User user = _database.Users.FirstOrDefault(u => u.UserName == "asd");//Session["userName"]);
            Employee currentUser = _database.Employees.FirstOrDefault(e => e.UserId == user.Id);

            _database.Projects.Add(new Project {
                OrganizationId = currentUser.OrganizationId,
                Name = pvm.Name,
                Description = pvm.Description,
                Deadline = pvm.Deadline
            });
            _database.SaveChanges();

            ViewBag.Information = "A projekt sikeresen létrejött.";

            return RedirectToAction("Index");
        }

        public ActionResult ProjectList(ProjectListViewModel plvm)
        {/*
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/

            plvm.ProjectList = _database.Projects.ToList();
            return View("ProjectList", plvm);   
        }

        public ActionResult ProjectData(int id)
        {/*
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/

            Project p = _database.Projects.FirstOrDefault(pr => pr.Id == id);
            ProjectViewModel pvm = new ProjectViewModel{
                Name = p.Name,
                Description = p.Description,
                Deadline = (DateTime)p.Deadline
            };
            List<Employee> projectMembers = _database.Employees.Where(e => e.ProjectId == p.Id).ToList();

            List<User> projectMembersUsers = new List<User>();
            foreach (Employee employee in projectMembers)
            {
                projectMembersUsers.Add(_database.Users.FirstOrDefault(u => u.Id == employee.UserId));
            }

            ProjectDataViewModel pdvm = new ProjectDataViewModel{
                Id = id,
                Project = pvm,
                ProjectMembers = projectMembersUsers
            };
        
            return View("ProjectData", pdvm);
        }

        [HttpGet]
        public ActionResult ProjectDataModify(int id)
        {/*
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/
            
            Project pr = _database.Projects.FirstOrDefault(p => p.Id == id);

            ProjectDataViewModel pdvm = new ProjectDataViewModel{
                Id = id,
                Project = new ProjectViewModel{
                    Name = pr.Name,
                    Description = pr.Description,
                    Deadline = (DateTime)pr.Deadline
                },
                ProjectMembers = new List<User>()
            };
            
            return View("ProjectDataModify", pdvm);
        }

        [HttpPost]
        public ActionResult ProjectDataModify(ProjectDataViewModel pdvm)
        {/*
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/

            Project p = _database.Projects.FirstOrDefault(pr => pr.Id == pdvm.Id);

            p.Name = pdvm.Project.Name;
            p.Description = pdvm.Project.Description;
            p.Deadline = pdvm.Project.Deadline;

            if (pdvm.ProjectMembers == null)
            {
                List<Employee> projectMembers = _database.Employees.Where(e => e.ProjectId == p.Id).ToList();

                List<User> projectMembersUsers = new List<User>();
                foreach (Employee employee in projectMembers)
                {
                    projectMembersUsers.Add(_database.Users.FirstOrDefault(u => u.Id == employee.UserId));
                }

                pdvm.ProjectMembers = projectMembersUsers;
            }

            _database.SaveChanges();

            return View("ProjectData", pdvm);
        }

        [HttpGet]
        public ActionResult ProjectMemberAdd(int Id)
        {
            List<Employee> notProjectMembers = _database.Employees.Where(e => e.ProjectId != Id).ToList();

            List<User> projectMembersUsers = new List<User>();
            foreach (Employee employee in notProjectMembers)
            {
                projectMembersUsers.Add(_database.Users.FirstOrDefault(u => u.Id == employee.UserId));
            }

            UserListViewModel ulvm = new UserListViewModel{
                Users = projectMembersUsers,
                ProjectId = Id
            };

            return View("ProjectMemberAdd", ulvm);
        }

        [HttpPost]
        public ActionResult ProjectMemberAdd(UserListViewModel ulvm)
        {
            string selectedItem = Request["selectedItem"];

            int projectId = ulvm.ProjectId;

            if (selectedItem == null)
            {
                return RedirectToAction("ProjectData", "Project", new { id = projectId });
            }

            int id;
            bool parsed = Int32.TryParse(selectedItem, out id);

            if (!parsed)
            {
                return RedirectToAction("ProjectData", "Project", new { id = projectId });
            }
            
            Employee e = _database.Employees.FirstOrDefault(em => em.UserId == id);
            e.ProjectId = projectId;

            return RedirectToAction("ProjectData", "Project", new { id = projectId });
        }
    }
}