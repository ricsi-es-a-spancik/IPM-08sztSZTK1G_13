using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class EditMemberPermViewModel
    {
        public Employee Employee { get; set; }

        public List<Models.Permissions.BasePermission> AvailablePermissions { get; set; }
    }
}