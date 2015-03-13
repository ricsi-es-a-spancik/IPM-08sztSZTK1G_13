namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Message
    {
        public int Id { get; set; }

        public string FromId { get; set; }

        public string ToId { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public bool IsRead { get; set; }

        public bool HideFromSender { get; set; }

        public bool HideFromTarget { get; set; }

        public virtual User User { get; set; }
    }
}
