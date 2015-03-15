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
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProjectViewModel pvm)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", pvm);
            }

            string username = User.Identity.Name;
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

        [Authorize]
        public ActionResult ProjectList(ProjectListViewModel plvm)
        {
            plvm.ProjectList = _database.Projects.ToList();
            return View("ProjectList", plvm);   
        }

        [Authorize]
        public ActionResult ProjectData(int id)
        {
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
        [Authorize]
        public ActionResult ProjectDataModify(int id)
        {
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
        [Authorize]
        public ActionResult ProjectDataModify(ProjectDataViewModel pdvm)
        {
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
        [Authorize]
        public ActionResult ProjectMemberAdd(int Id)
        {
            string userName = User.Identity.Name;
            User currentUser = _database.Users.FirstOrDefault(u => u.UserName == userName);

            int orgId = _database.Projects.FirstOrDefault(p => p.Id == Id).OrganizationId;

            List<User> currentProjectMembers =
                _database.ProjectMembers.Where(pm => pm.ProjectId == Id).Select(pm => pm.User).ToList();

            List<User> currentOrgEmp = 
                _database.Employees.Where(e => e.OrganizationId == orgId).Select(e => e.User).ToList();

            currentOrgEmp.RemoveAll(u => currentProjectMembers.Contains(u));
            List<User> inter = currentOrgEmp;

            inter.Remove(currentUser);

            //List<ProjectMember> projectMembers = _database.ProjectMembers.Where(pm => pm.ProjectId == Id).ToList();
            //List<Employee> organizationMembers = _database.Employees.Where(e => e.OrganizationId == orgId).ToList();

            //erase currentuser
            /*foreach (Employee e in organizationMembers)
            {
                if (e.UserId == currentUser.Id)
                {
                    organizationMembers.Remove(e);
                    break;
                }
            }

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
            }*/

            UserListViewModel ulvm = new UserListViewModel{
                Users = inter,
                ProjectId = Id
            };

            return View("ProjectMemberAdd", ulvm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ProjectMemberAdd(UserListViewModel ulvm)
        {
            string selectedItem = Request["selectedItem"];

            int projectId = ulvm.ProjectId;

            if (selectedItem == null)
            {
                return RedirectToAction("ProjectData", "Project", new { id = projectId });
            }

            /*int id;
            bool parsed = Int32.TryParse(selectedItem, out id);

            if (!parsed)
            {
                return RedirectToAction("ProjectData", "Project", new { id = projectId });
            }*/
            
            _database.ProjectMembers.Add(new ProjectMember{
                    UserId = selectedItem,
                    ProjectId = projectId
            });
            
            _database.SaveChanges();

            return RedirectToAction("ProjectData", "Project", new { id = projectId });
        }

        [Authorize]
        public ActionResult ProjectMemberRemove(string removeableUserId, int projectId)
        {
            ProjectMember pm = _database.ProjectMembers.First(x => x.ProjectId == projectId && 
                                                                   x.UserId == removeableUserId);
            _database.ProjectMembers.Remove(pm);
            _database.SaveChanges();

            return RedirectToAction("ProjectData", "Project", new { id = projectId});
        }
    }
}