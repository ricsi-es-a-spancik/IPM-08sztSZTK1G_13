using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class EpicViewModel
    {
        [Required(ErrorMessage = "A bázis nevének megadása kötelező.")]
        [StringLength(50, ErrorMessage = "A bázis neve maximum 50 karakter lehet.")]
        public String Name { get; set; }

        public int ProjectId { get; set; }
    }
}