using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class IssueListingViewModel
    {
        public int? ProjectId { get; set; }

        public List<Project> Projects { get; set; }

        public List<Issue> CurrentIssues { get; set; }
    }
}