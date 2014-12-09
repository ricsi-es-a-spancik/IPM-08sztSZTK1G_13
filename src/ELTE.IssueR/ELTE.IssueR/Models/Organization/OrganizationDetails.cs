using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class OrganizationDetails
    {
        public Organization Org { get; set; }

        public IEnumerable<Project> Projects { get; set; }
    }
}