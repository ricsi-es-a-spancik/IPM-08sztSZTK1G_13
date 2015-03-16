namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Project
    {
        public Project()
        {
            Issues = new HashSet<Issue>();
            ProjectMembers = new HashSet<ProjectMember>();
            Documents = new HashSet<ProjectDocument>();
        }

        public int Id { get; set; }

        public int OrganizationId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? Deadline { get; set; }

        public virtual ICollection<Issue> Issues { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual ICollection<ProjectMember> ProjectMembers { get; set; }

        public virtual ICollection<ProjectDocument> Documents { get; set; }
    }
}
