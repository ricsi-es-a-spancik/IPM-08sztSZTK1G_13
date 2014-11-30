using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ELTE.IssueR.WebClient.Models
{
    public class IssueViewModel
    {
        private IssueREntities entity;

        //All projects in the database
        private List<Project> projects;

        private Int32 curProjectId;

        //Selected project and the connected epics and issues
        private Project curProject = null;
        private List<Epic> curEpics = new List<Epic>();
        private List<Issue> curIssues = new List<Issue>();

        public List<Project> Projects
        {
            get { return projects; }
            private set { projects = value; }
        }

        public Int32 CurrentProjectId
        {
            get { return curProjectId; }
            set { curProjectId = value; CurrentProject = projects.Find(prj => prj.Id == curProjectId); }
        }

        public Project CurrentProject 
        {
            get { return curProject; }
            set 
            { 
                curProject = value; 
                curEpics = entity.Epics.Where(epic => epic.ProjectId == curProject.Id).ToList();
                curIssues = entity.Issues.Where(issue => curEpics.Exists(epic => epic.Id == issue.EpicId)).ToList();
            }
        }

        public List<Epic> CurrentEpics
        {
            get { return curEpics; }
            private set { curEpics = value; }
        }

        public List<Issue> CurrentIssues
        {
            get { return curIssues; }
            private set { curIssues = value; }
        }

        public IssueViewModel(IssueREntities entity)
        {
            this.entity = entity;
            projects = entity.Projects.ToList();
        }
    }
}