//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Issues
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }
        public int EmployeeId { get; set; }
        public int EpicId { get; set; }
        public Nullable<int> EstimatedTime { get; set; }
        public Nullable<int> TimeSpent { get; set; }
        public Nullable<int> Status { get; set; }
    
        public virtual Employees Employees { get; set; }
        public virtual Epics Epics { get; set; }
    }
}
