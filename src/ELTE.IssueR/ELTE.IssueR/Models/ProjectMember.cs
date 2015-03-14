namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;

    public partial class ProjectMember
    {
        public string UserId { get; set; }

        public int ProjectId { get; set; }

        public short Status { get; set; }

        public virtual Project Project { get; set; }

        public virtual User User { get; set; }
    }
}
