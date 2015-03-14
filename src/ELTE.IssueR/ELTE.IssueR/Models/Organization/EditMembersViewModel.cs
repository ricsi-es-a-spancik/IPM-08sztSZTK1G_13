using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class EditableEmployees
    {
        public string Id { get; set; }
        public String Username { get; set; }
        public Permissions.Permission Perm { get; set; }
    }

    public class EditMembersViewModel
    {
        public Int32 OrganizationId { get; set; }

        public List<EditableEmployees> Users { get; set; }
    }
}