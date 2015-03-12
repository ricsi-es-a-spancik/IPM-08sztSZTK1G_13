namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Issue
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public short Type { get; set; }

        public short Status { get; set; }

        public DateTime? Deadline { get; set; }

        public string UserId { get; set; }

        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public virtual User User { get; set; }
    }
}
