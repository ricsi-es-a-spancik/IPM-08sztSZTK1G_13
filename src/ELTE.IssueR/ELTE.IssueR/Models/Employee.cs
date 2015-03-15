namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;
    using ELTE.IssueR.Models.Permissions;

    public partial class Employee
    {
        public string UserId { get; set; }

        public int OrganizationId { get; set; }

        public BasePermission Status { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual User User { get; set; }
    }
}
