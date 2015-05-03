using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services.Configuration;

namespace ELTE.IssueR.Models
{
    public class ChartManager
    {
        private readonly ApplicationDbContext _context;

        public ChartManager(ApplicationDbContext context)
        {
            this._context = context;
        }

        public PieChartData[] GetIssueStatusPieChart(int projectId)
        {
            var issuesByStatus = from i
                in _context.Issues
                where i.ProjectId == projectId
                group i by i.Status
                into g
                select new
                {
                    Label = g.Key.ToString(),
                    Value = g.Count()
                };

            return issuesByStatus.ToArray().Select(item => new PieChartData(item.Label, item.Value)).ToArray();
        }

        public PieChartData[] GetIssueTypePieChart(int projectId)
        {
            var issuesByType = from i
                in _context.Issues
                where i.ProjectId == projectId
                group i by i.Type
                into g
                select new
                {
                    Label = g.Key.ToString(),
                    Value = g.Count()
                };

            return issuesByType.ToArray().Select(item => new PieChartData(item.Label, item.Value)).ToArray(); ;
        }

        public PointChartData GetUserIssuesRadarChart(int projectId)
        {
            var query = (from issue
                in _context.Issues
                where issue.ProjectId == projectId
                group issue by issue.User.Name
                into g
                select new
                {
                    Name = g.Key,
                    IssueCount = g.Count(),
                    InProgressCount = g.Where(i => i.Status == IssueStatus.In_progress).Select(iss => iss).Count()
                }).ToArray();

            var labels = query.Select(d => d.Name).ToArray();

            var dataset1 = new PointChartDataSet
            {
                Label = "All Issues",
                Data = query.Select(d => d.IssueCount).ToArray()
            };

            var dataset2 = new PointChartDataSet
            {
                Label = "In Progress Issues",
                Data = query.Select(d => d.InProgressCount).ToArray()
            };

            return new PointChartData(labels, new[] { dataset1, dataset2 });
        }

        private Dictionary<IssueStatus, List<int>> GetInitStatusDictionary()
        {
            return Enum.GetNames(typeof (IssueStatus))
                .ToDictionary(
                    status => (IssueStatus) Enum.Parse(typeof (IssueStatus), status),
                    status => new List<int>() {0});
        }

        public PointChartData GetIssueStatusLineChart(IssueChangeScale scale, int projectId)
        {
            var aggregator = new IssueChangeAggregator<IssueStatus>(GetLogs(projectId), (l => l.OldStatus), (l => l.NewStatus));
            return GetLineChartData(aggregator, scale);
        }

        public PointChartData GetIssueTypeLineChart(IssueChangeScale scale, int projectId)
        {
            var aggregator = new IssueChangeAggregator<IssueType>(GetLogs(projectId), (l => l.OldType), (l => l.NewType));
            return GetLineChartData(aggregator, scale);
        }

        private IEnumerable<IssueChangeLog> GetLogs(int projectId)
        {
            return from l in _context.IssueChangeLogs
                where l.Issue.ProjectId == projectId
                orderby l.ModifiedAt
                select l;
        }

        private PointChartData GetLineChartData<TEnumType>(IssueChangeAggregator<TEnumType> aggregator,
            IssueChangeScale scale)
            where TEnumType : struct
        {
            List<string> labels;
            Dictionary<TEnumType, List<int>> counts;

            switch (scale)
            {
                case IssueChangeScale.RealTime:
                    aggregator.GetRealTimeChanges(out labels, out counts);
                    break;
                case IssueChangeScale.Daily:
                    aggregator.GetDailyChanges(out labels, out counts);
                    break;
                case IssueChangeScale.Weekly:
                    aggregator.GetWeeklyChanges(out labels, out counts);
                    break;
                default:
                    throw new ArgumentException(String.Format("Scale {0} is not supported.", scale));
            }

            return new PointChartData(labels.ToArray(),
                counts.Select(kv => new PointChartDataSet(kv.Key.ToString(), kv.Value.ToArray())).ToArray());
        }
    }
}