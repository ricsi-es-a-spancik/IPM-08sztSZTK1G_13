using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class ProjectViewModel
    {
        [Required(ErrorMessage="A név magadása kötelező.")]
        [StringLength(50, ErrorMessage="Maximum 50 karakter lehet.")]
        public String Name { get; set; }

        [Required(ErrorMessage = "A leírás magadása kötelező.")]
        [StringLength(50, ErrorMessage = "Maximum 50 karakter lehet.")]
        public String Description { get; set; }

        [Required(ErrorMessage = "A határidő magadása kötelező.")]
        [DataType(DataType.Date)]
        public Nullable<DateTime> Deadline { get; set;}
    }
}