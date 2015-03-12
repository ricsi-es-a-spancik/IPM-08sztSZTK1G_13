namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;

    public partial class CoverImage
    {
        public int Id { get; set; }

        public int OrganizationId { get; set; }

        public byte[] Image { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
