using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class PointChartData
    {
        public string[] Labels { get; set; }
        public PointChartDataSet[] Datasets { get; set; }

        public PointChartData(string[] labels, PointChartDataSet[] datasets)
        {
            this.Labels = labels;
            this.Datasets = datasets;
        }
    }

    public class PointChartDataSet
    {
        public string Label { get; set; }
        public int[] Data { get; set; }

        public PointChartDataSet()
        {
            
        }

        public PointChartDataSet(string label, int[] data)
        {
            this.Label = label;
            this.Data = data;
        }
    }
}