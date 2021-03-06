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
    
    public partial class Organizations
    {
        public Organizations()
        {
            this.CoverImages = new HashSet<CoverImages>();
            this.Employees = new HashSet<Employees>();
            this.Projects = new HashSet<Projects>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> FoundationYear { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Activity { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<CoverImages> CoverImages { get; set; }
        public virtual ICollection<Employees> Employees { get; set; }
        public virtual ICollection<Projects> Projects { get; set; }
    }
}
