using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class IssueListingViewModel
    {
        //az aktuálisan kiválasztott projekt
        public int? SelectedProjectId { get; set; }

        public string FilterText { get; set; }

        public IEnumerable<Filter> Filters { get; set; }
    }
}