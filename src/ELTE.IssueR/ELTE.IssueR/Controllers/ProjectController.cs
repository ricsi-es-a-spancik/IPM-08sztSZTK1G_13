﻿using System;
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
        public ActionResult Project()
        {/*
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/

            return View("Project");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Project(ProjectViewModel pvm)
        {/*
            if (Session["userName"] == null)
            {
                return RedirectToAction("Index", "Home");
            }*/

            if (!ModelState.IsValid)
            {
                return View("Project", pvm);
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

            return RedirectToAction("Project");
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

            if (selectedItem == null)
            {
                return RedirectToAction("ProjectData", "Project", 1);
            }

            int id;
            bool parsed = Int32.TryParse(selectedItem, out id);

            if (!parsed)
            {
                return RedirectToAction("ProjectData", "Project", 1);
            }
            
            Employee e = _database.Employees.FirstOrDefault(em => em.UserId == id);
            e.ProjectId = ulvm.ProjectId;

            return RedirectToAction("ProjectData", "Project", 1);
        }
    }
}