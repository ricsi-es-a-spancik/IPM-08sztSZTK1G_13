namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;

    public partial class OrganizationDocument
    {
        public int Id { get; set; }

        public int OrganizationId { get; set; }

        public string Author { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public DateTime? Modified { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
