using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class ChartsViewModel
    {
        public List<PieChartData> TypePieChart { get; set; }

        public List<PieChartData> StatusPieChart { get; set; }
    }
}