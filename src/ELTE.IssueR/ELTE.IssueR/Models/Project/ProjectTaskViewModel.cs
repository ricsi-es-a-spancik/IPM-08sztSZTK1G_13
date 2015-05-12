using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ELTE.IssueR.Models
{
    public class ProjectTaskViewModel
    {
        [Required(ErrorMessage = "A név magadása kötelező.")]
        [StringLength(50, ErrorMessage = "Maximum 50 karakter lehet.")]
        public String Name { get; set; }

        [Required(ErrorMessage = "A kezdő dátum magadása kötelező.")]
        [DataType(DataType.Date)]
        public Nullable<DateTime> StartDate { get; set; }

        [Required(ErrorMessage = "A befejező dátum magadása kötelező.")]
        [DataType(DataType.Date)]
        public Nullable<DateTime> EndDate { get; set; }

        public String Resource { get; set; }

        public int ProjectId { get; set; }

        public int SelectedTaskId { get; set; }

        //public List<Task> Tasks { get; set;}

        //public IEnumerable<SelectListItem> TaskItems { get {return new SelectList(Tasks, "Id", "Name");} }
    }
}