using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class ProjectDataViewModel
    {
        public int Id { get; set; }
        public ProjectViewModel Project { get; set; }

        public List<User> ProjectMembers { get; set; }
    }
}