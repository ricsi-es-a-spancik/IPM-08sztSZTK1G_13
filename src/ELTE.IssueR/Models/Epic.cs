//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Epic
    {
        public Epic()
        {
            this.Issues = new HashSet<Issue>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }
    
        public virtual Project Project { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
    }
}
