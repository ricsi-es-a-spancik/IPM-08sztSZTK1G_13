namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;

    public partial class ProjectDocument
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public string Author { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public DateTime? Modified { get; set; }

        public virtual Project Project { get; set; }
    }
}
