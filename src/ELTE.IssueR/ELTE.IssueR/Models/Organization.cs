namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Organization
    {
        public Organization()
        {
            CoverImages = new HashSet<CoverImage>();
            Employees = new HashSet<Employee>();
            Projects = new HashSet<Project>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int? FoundationYear { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Activity { get; set; }

        public string Description { get; set; }

        public virtual ICollection<CoverImage> CoverImages { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}
