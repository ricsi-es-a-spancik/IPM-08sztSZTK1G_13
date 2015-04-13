using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class PieChartData
    {
        public int value { get; set; }
        public string color { get; set; }
        public string highlight { get; set; }
        public string label { get; set; }

        public PieChartData(int value, string color, string highlight, string label)
        {
            this.value = value;
            this.color = color;
            this.highlight = highlight;
            this.label = label;
        }
    }
}