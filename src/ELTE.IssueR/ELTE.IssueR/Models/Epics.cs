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
    
    public partial class Epics
    {
        public Epics()
        {
            this.Issues = new HashSet<Issues>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }
    
        public virtual Projects Projects { get; set; }
        public virtual ICollection<Issues> Issues { get; set; }
    }
}
