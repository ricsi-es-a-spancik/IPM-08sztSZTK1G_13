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
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProjectViewModel pvm)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View("Index", pvm);
            }

            string username = Session["userName"].ToString();
            User user = _database.Users.Where(u => u.UserName == username).FirstOrDefault();
            Employee currentUser = _database.Employees.FirstOrDefault(e => e.UserId == user.Id);

            if (currentUser != null)
            {
                Project pr = _database.Projects.Add(new Project {
                    OrganizationId = currentUser.OrganizationId,
                    Name = pvm.Name,
                    Description = pvm.Description,
                    Deadline = pvm.Deadline
                });
                _database.SaveChanges();

                _database.ProjectMembers.Add(new ProjectMember{
                    UserId = user.Id,
                    ProjectId = pr.Id
                });
                _database.SaveChanges();
            }

            return RedirectToAction("ProjectList", new ProjectListViewModel());
        }

        public ActionResult ProjectList(ProjectListViewModel plvm)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            plvm.ProjectList = _database.Projects.ToList();
            return View("ProjectList", plvm);   
        }

        public ActionResult ProjectData(int id)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Project p = _database.Projects.FirstOrDefault(pr => pr.Id == id);
            ProjectViewModel pvm = new ProjectViewModel{
                Name = p.Name,
                Description = p.Description,
                Deadline = (DateTime)p.Deadline
            };
            List<ProjectMember> projectMembers = _database.ProjectMembers.Where(pe => pe.ProjectId == p.Id).ToList();

            List<User> projectMembersUsers = new List<User>();
            foreach (ProjectMember pmember in projectMembers)
            {
                projectMembersUsers.Add(_database.Users.FirstOrDefault(u => u.Id == pmember.UserId));
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
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            
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
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Project p = _database.Projects.FirstOrDefault(pr => pr.Id == pdvm.Id);

            p.Name = pdvm.Project.Name;
            p.Description = pdvm.Project.Description;
            if (pdvm.Project.Deadline != null)
            {
                p.Deadline = pdvm.Project.Deadline;
            }

            if (pdvm.ProjectMembers == null)
            {
                List<ProjectMember> projectMembers = _database.ProjectMembers.Where(pm => pm.ProjectId == p.Id).ToList();

                List<User> projectMembersUsers = new List<User>();
                foreach (ProjectMember pmember in projectMembers)
                {
                    projectMembersUsers.Add(_database.Users.FirstOrDefault(u => u.Id == pmember.UserId));
                }

                pdvm.ProjectMembers = projectMembersUsers;
            }

            _database.SaveChanges();

            return View("ProjectData", pdvm);
        }

        [HttpGet]
        public ActionResult ProjectMemberAdd(int Id)
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string userName = Session["userName"].ToString();
            User currentUser = _database.Users.FirstOrDefault(u => u.UserName == userName);

            int orgId = _database.Employees.FirstOrDefault(e => e.UserId == currentUser.Id).OrganizationId;

            List<ProjectMember> notCurrentProjectMembers = _database.ProjectMembers.Where(pm => pm.ProjectId != Id).ToList();
            List<ProjectMember> projectMembers = _database.ProjectMembers.Where(pm => pm.ProjectId == Id).ToList();
            List<Employee> organizationMembers = _database.Employees.Where(e => e.OrganizationId == orgId).ToList();

            foreach (ProjectMember pm in projectMembers) //erase members that part of this project (but part of other projects)
            {
                ProjectMember member = notCurrentProjectMembers.FirstOrDefault(p => p.UserId == pm.UserId);
                notCurrentProjectMembers.Remove(member);
            }

            List<User> projectMembersUsers = new List<User>();
            foreach (ProjectMember pmember in notCurrentProjectMembers)
            {
                projectMembersUsers.Add(_database.Users.FirstOrDefault(u => u.Id == pmember.UserId));
            }

            //members without project
            foreach (Employee e in organizationMembers)
            {
                ProjectMember pm = _database.ProjectMembers.FirstOrDefault(p => p.UserId == e.UserId);
                if (pm == null) //not part of any projects
                {
                    projectMembersUsers.Add(_database.Users.FirstOrDefault(u => u.Id == e.UserId));
                }
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
            
            ProjectMember e = _database.ProjectMembers.FirstOrDefault(pm => pm.UserId == id);
            if (e == null) //employee not member of any projects
            {
                _database.ProjectMembers.Add(new ProjectMember{
                    UserId = id,
                    ProjectId = projectId
                });
            }
            else
            {
                e.ProjectId = projectId;
            }
            
            _database.SaveChanges();

            return RedirectToAction("ProjectData", "Project", new { id = projectId });
        }
    }
}