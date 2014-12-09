using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class UserListViewModel
    {
        public List<User> Users { get; set; }
        public int ProjectId { get; set; }
    }
}