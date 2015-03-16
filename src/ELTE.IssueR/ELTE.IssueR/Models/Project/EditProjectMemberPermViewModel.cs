using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class EditProjectMemberPermViewModel
    {
        public ProjectMember Member { get; set; }

        public List<Models.Permissions.BasePermission> AvailablePermissions { get; set; }
    }
}