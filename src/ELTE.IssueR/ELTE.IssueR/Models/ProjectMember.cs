namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;
	using ELTE.IssueR.Models.Permissions;

    public partial class ProjectMember
    {
        public string UserId { get; set; }

        public int ProjectId { get; set; }

        public BasePermission Status { get; set; }

        public virtual Project Project { get; set; }

        public virtual User User { get; set; }
    }
}
