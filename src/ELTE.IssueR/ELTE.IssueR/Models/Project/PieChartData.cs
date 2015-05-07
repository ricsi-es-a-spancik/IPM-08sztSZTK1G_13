using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class PieChartData
    {
        public string Label { get; set; }
        public int Value { get; set; }

        public PieChartData(string label, int value)
        {
            this.Label = label;
            this.Value = value;
        }
    }
}