using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public partial class IssueChangeLog
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public DateTime ModifiedAt { get; set; }
        public IssueStatus? OldStatus { get; set; }
        public IssueStatus NewStatus { get; set; }
        public IssueType? OldType { get; set; }
        public IssueType NewType { get; set; }

        public virtual Issue Issue { get; set; }
    }
}