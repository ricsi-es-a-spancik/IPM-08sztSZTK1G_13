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
    
    public partial class Employees
    {
        public Employees()
        {
            this.Issues = new HashSet<Issues>();
        }
    
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public int ProjectId { get; set; }
        public short Status { get; set; }
    
        public virtual Organizations Organizations { get; set; }
        public virtual Projects Projects { get; set; }
        public virtual Users Users { get; set; }
        public virtual ICollection<Issues> Issues { get; set; }
    }
}
