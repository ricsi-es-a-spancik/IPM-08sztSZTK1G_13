namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Employee
    {
        public string UserId { get; set; }

        public int OrganizationId { get; set; }

        public byte Status { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual User User { get; set; }
    }
}
