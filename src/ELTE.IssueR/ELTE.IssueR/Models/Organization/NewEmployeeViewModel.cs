using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class NewEmployeeViewModel
    {
        public Int32 OrganizationId { get; set; }

        [Required]
        public String NewEmployeeUserName { get; set; }

        public NewEmployeeViewModel()
        {
            OrganizationId = -1;
            NewEmployeeUserName = "";
        }

        public NewEmployeeViewModel(Int32 oid)
        {
            OrganizationId = oid;
            NewEmployeeUserName = "";
        }
    }
}