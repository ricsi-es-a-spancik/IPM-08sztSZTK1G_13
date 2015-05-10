using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ELTE.IssueR.Models;
using ELTE.IssueR.Models.Permissions;

namespace ELTE.IssueR.Controllers
{
    public class ProjectController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(Int32 orgId)
        {
            return View("Index", new ProjectViewModel { OrganizationId = orgId });
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
            User user = userManager.Users.First(u => u.UserName == username);
            Employee currentUser = _database.Employees.
                FirstOrDefault(e => e.OrganizationId == pvm.OrganizationId && e.UserId == user.Id);

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
                    ProjectId = pr.Id,
                    Status = BasicPermissions.Administrator.Code
                });
                _database.SaveChanges();

                return ProjectData(pr.Id);
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
            string OrganizationName = _database.Organizations.FirstOrDefault(o => o.Id == p.OrganizationId).Name;
            ProjectViewModel pvm = new ProjectViewModel{
                Name = p.Name,
                Description = p.Description,
                Deadline = (DateTime)p.Deadline,
                OrganizationId = p.OrganizationId,
                OrganizationName = OrganizationName
            };

            List<User> projectMembersUsers = p.ProjectMembers.Select(mem => mem.User).ToList();

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
            if (!checkPermission(BasePermission.EditContent, id))
                return RedirectToAction("Index", "Home");

            Project pr = _database.Projects.FirstOrDefault(p => p.Id == id);

            ProjectDataViewModel pdvm = new ProjectDataViewModel{
                Id = id,
                Project = new ProjectViewModel{
                    Name = pr.Name,
                    Description = pr.Description,
                    Deadline = (DateTime)pr.Deadline
                },
                ProjectMembers = pr.ProjectMembers.Select(mem => mem.User).ToList()
            };
            
            return View("ProjectDataModify", pdvm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ProjectDataModify(ProjectDataViewModel pdvm)
        {
            if (!checkPermission(BasePermission.EditContent, pdvm.Id))
                return RedirectToAction("Index", "Home");

            Project p = _database.Projects.FirstOrDefault(pr => pr.Id == pdvm.Id);

            p.Name = pdvm.Project.Name;
            p.Description = pdvm.Project.Description;
            if (pdvm.Project.Deadline != null)
            {
                p.Deadline = pdvm.Project.Deadline;
            }

            _database.SaveChanges();

            return RedirectToAction("ProjectData", new { id = p.Id });
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProjectMemberAdd(int Id)
        {
            //if (!checkPermission(BasePermission.AddMember, Id))
            //    return RedirectToAction("Index", "Home");

            string userName = User.Identity.Name;
            User currentUser = userManager.Users.FirstOrDefault(u => u.UserName == userName);

            Project pr = _database.Projects.FirstOrDefault(p => p.Id == Id);
            int orgId = pr.OrganizationId;

            List<User> currentProjectMembers = pr.ProjectMembers.Select(mem => mem.User).ToList();

            List<User> currentOrgEmp = pr.Organization.Employees.Select(emp => emp.User).ToList();

            currentOrgEmp.RemoveAll(u => currentProjectMembers.Contains(u));
            List<User> inter = currentOrgEmp;

            inter.Remove(currentUser);

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
            //if (!checkPermission(BasePermission.AddMember, ulvm.ProjectId))
            //    return RedirectToAction("Index", "Home");

            string selectedItem = Request["selectedItem"];

            int projectId = ulvm.ProjectId;

            if (selectedItem == null)
            {
                return RedirectToAction("ProjectData", "Project", new { id = projectId });
            }
            
            _database.ProjectMembers.Add(new ProjectMember{
                    UserId = selectedItem,
                    ProjectId = projectId,
                    Status = BasicPermissions.Worker.Code
            });
            
            _database.SaveChanges();

            return RedirectToAction("ProjectData", "Project", new { id = projectId });
        }

        [Authorize]
        public ActionResult ProjectMemberRemove(string removeableUserId, int projectId)
        {
            if (!checkPermission(BasePermission.RemoveMember, projectId))
                return RedirectToAction("Index", "Home");

            ProjectMember pm = _database.ProjectMembers.First(x => x.ProjectId == projectId && 
                                                                   x.UserId == removeableUserId);
            _database.ProjectMembers.Remove(pm);
            _database.SaveChanges();

            return RedirectToAction("ProjectData", "Project", new { id = projectId});
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProjectPlan(int Id)
        {
            //if (!checkPermission(BasePermission.EditContent, projectId))
            //    return RedirectToAction("Index", "Home");

            ProjectTaskViewModel ptvm = new ProjectTaskViewModel{
                ProjectId = Id
            };

            return View("ProjectPlan", ptvm);

        }

        [HttpPost]
        [Authorize]
        public ActionResult ProjectPlan(ProjectTaskViewModel ptvm)
        {
            //if (!checkPermission(BasePermission.EditContent, ptvm.ProjectId))
            //    return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                return View("ProjectPlan", ptvm);
            }

            Project p = _database.Projects.FirstOrDefault(x => x.Id == ptvm.ProjectId);
            if (ptvm.StartDate > ptvm.EndDate || 
                ptvm.EndDate > p.Deadline)
            {
                return View("ProjectPlan", ptvm);
            }

            _database.Tasks.Add(new Task{
                ProjectId = ptvm.ProjectId,
                Name = ptvm.Name,
                StartDate = ptvm.StartDate,
                EndDate = ptvm.EndDate,
                Resource = ptvm.Resource
            });

            _database.SaveChanges();
            
            return RedirectToAction("ProjectData", "Project", new { id = ptvm.ProjectId });
        }

        [Authorize]
        public ActionResult EditMemberPermissions(Int32 projId, string userId)
        {
            if (!checkPermission(BasePermission.EditMember, projId))
                return RedirectToAction("Index", "Home");

            ProjectMember pm = _database.ProjectMembers.First(mem => mem.ProjectId == projId && mem.UserId == userId);
            List<BasePermission> ps = Enum.GetValues(typeof(BasePermission)).Cast<BasePermission>().Where(bp => bp != BasePermission.None).ToList();

            return View("EditMemberPermissions", new EditProjectMemberPermViewModel { Member = pm, AvailablePermissions = ps });
        }

        [Authorize]
        public ActionResult AddPermission(Int32 projId, string userId, BasePermission perm)
        {
            if (!checkPermission(BasePermission.EditMember, projId))
                return RedirectToAction("Index", "Home");

            ProjectMember pm = _database.ProjectMembers.First(mem => mem.ProjectId == projId && mem.UserId == userId);
            Permission pr = new Permission(pm.Status);
            pr.AddPermission(perm);
            pm.Status = pr.Code;
            _database.SaveChanges();

            return RedirectToAction("EditMemberPermissions", new { projId = projId, userId = userId });
        }

        [Authorize]
        public ActionResult RemovePermission(Int32 projId, string userId, BasePermission perm)
        {
            if (!checkPermission(BasePermission.EditMember, projId))
                return RedirectToAction("Index", "Home");

            ProjectMember pm = _database.ProjectMembers.First(mem => mem.ProjectId == projId && mem.UserId == userId);
            Permission pr = new Permission(pm.Status);
            pr.RemovePermission(perm);
            pm.Status = pr.Code;
            _database.SaveChanges();

            return RedirectToAction("EditMemberPermissions", new { projId = projId, userId = userId });
        }

        public HtmlString HasPermission(Int32 projId, BasePermission perm)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new HtmlString(false.ToString());
            }

            String username = User.Identity.Name;
            string userId = _database.Users.First(user => user.UserName == username).Id;
            ProjectMember mem = _database.ProjectMembers.Find(userId, projId);
            if (mem != null)
            {
                return new HtmlString(new Permission(mem.Status).HasPermission(perm).ToString());
            }
            else
            {
                return new HtmlString(false.ToString());
            }
        }

        private bool checkPermission(BasePermission bp, Int32 projId)
        {
            ProjectMember me = _database.ProjectMembers.Where(mem => mem.ProjectId == projId && mem.UserId == User.Identity.Name).FirstOrDefault();
            if (me == null)
                return false;

            return ((Permission)me.Status).HasPermission(bp);
        }
    
    }
}