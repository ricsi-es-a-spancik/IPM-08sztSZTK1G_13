namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;

    public partial class UserImage
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public byte[] Image { get; set; }

        public string ImageUrl { get; set; }

        public virtual User User { get; set; }
    }
}
