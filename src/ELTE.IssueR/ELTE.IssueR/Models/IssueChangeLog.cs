using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public partial class IssueChangeLog<TEnumType> where TEnumType : struct, IConvertible
    {
        public IssueChangeLog()
        {
            if (!typeof (TEnumType).IsEnum)
                throw new ArgumentException("Generic type parameter must be a type of Enum");
        }

        public int Id { get; set; }
        public int IssueId { get; set; }
        public DateTime ModifiedAt { get; set; }
        public TEnumType? OldValue { get; set; }
        public TEnumType NewValue { get; set; }
        public virtual Issue Issue { get; set; }
    }

    public class IssueStatusChangeLog : IssueChangeLog<IssueStatus> { }
    public class IssueTypeChangeLog : IssueChangeLog<IssueType> { }
}