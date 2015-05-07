using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.DynamicData;

namespace ELTE.IssueR.Models
{

    public class IssueChangeAggregator<TEnumType> where TEnumType : struct, IConvertible
    {
        private readonly IEnumerable<IssueChangeLog<TEnumType>> _changes;

        public IssueChangeAggregator(IEnumerable<IssueChangeLog<TEnumType>> changes)
        {
            _changes = changes;
        }

        public void GetRealTimeChanges(out List<string> labels, out Dictionary<TEnumType, List<int>> counts)
        {
            labels = _changes.Select(l => l.Id.ToString()).ToList();
            counts = GetAggregatedChanges(_changes);
        }

        public void GetDailyChanges(out List<string> labels, out Dictionary<TEnumType, List<int>> counts)
        {
            labels = new List<string>();
            counts = GetInitialDictionary();

            var groupedByDays = from cl in _changes.AsEnumerable()
                group cl by cl.ModifiedAt.Date
                into g
                orderby g.Key
                select new
                {
                    Date = g.Key,
                    Logs = g.OrderBy(l => l.ModifiedAt)
                };

            foreach (var group in groupedByDays)
            {
                labels.Add(group.Date.ToString("yyyy MMMM dd"));
                var dailyChanges = GetAggregatedChanges(group.Logs);
                foreach (var item in dailyChanges)
                {
                    counts[item.Key].Add(counts[item.Key].Last() + item.Value.Last());
                }
            }
        }

        public void GetWeeklyChanges(out List<string> labels, out Dictionary<TEnumType, List<int>> counts)
        {
            labels = new List<string>();
            counts = GetInitialDictionary();

            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            var groupedByWeeks = from cl in _changes.AsEnumerable()
                group cl by
                    cl.ModifiedAt.Year*100 + cal.GetWeekOfYear(cl.ModifiedAt, dfi.CalendarWeekRule, dfi.FirstDayOfWeek)
                into g
                orderby g.Key
                select new
                {
                    Week = g.Key,
                    Logs = g.OrderBy(l => l.ModifiedAt)
                };

            foreach (var group in groupedByWeeks)
            {
                labels.Add(group.Week.ToString());
                var weeklyChanges = GetAggregatedChanges(group.Logs);
                foreach (var item in weeklyChanges)
                {
                    counts[item.Key].Add(counts[item.Key].Last() + item.Value.Last());
                }
            }
        }

        private Dictionary<TEnumType, List<int>> GetInitialDictionary()
        {
            return Enum.GetNames(typeof (TEnumType))
                .ToDictionary(
                    status => (TEnumType) Enum.Parse(typeof (TEnumType), status),
                    status => new List<int>() {0});
        }

        private Dictionary<TEnumType, List<int>> GetAggregatedChanges(IEnumerable<IssueChangeLog<TEnumType>> logs)
        {
            var counts = GetInitialDictionary();

            var i = 1;

            foreach (var change in logs)
            {
                foreach (var key in counts.Keys)
                {
                    counts[key].Add(counts[key].Last());
                }

                if (change.OldValue != null)
                {
                    counts[change.OldValue.Value][i] -= 1;
                }

                counts[change.NewValue][i] += 1;

                ++i;
            }

            foreach (var kv in counts)
            {
                kv.Value.RemoveAt(0);
            }

            return counts;
        }
    }
}