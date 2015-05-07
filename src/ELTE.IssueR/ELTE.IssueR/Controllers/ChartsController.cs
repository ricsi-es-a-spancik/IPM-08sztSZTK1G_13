using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ELTE.IssueR.Models;

namespace ELTE.IssueR.Controllers
{
    public class ChartsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Pies(ProjectChartTypes type, int projectId)
        {
            var manager = new ChartManager(new ApplicationDbContext());

            switch (type)
            {
                case ProjectChartTypes.IssuesPerStatus:
                    return Ok(manager.GetIssueStatusPieChart(projectId));
                case ProjectChartTypes.IssuesPerType:
                    return Ok(manager.GetIssueTypePieChart(projectId));
                default:
                    return BadRequest("Chart type not supported.");
            }
        }

        [HttpGet]
        public IHttpActionResult Radars(int projectId)
        {
            var manager = new ChartManager(new ApplicationDbContext());
            return Ok(manager.GetUserIssuesRadarChart(projectId));
        }

        [HttpGet]
        public IHttpActionResult Lines(ProjectChartTypes type, IssueChangeScale scale, int projectId)
        {
            var manager = new ChartManager(new ApplicationDbContext());

            switch (type)
            {
                case ProjectChartTypes.IssuesPerStatus:
                    return Ok(manager.GetIssueStatusLineChart(scale, projectId));
                case ProjectChartTypes.IssuesPerType:
                    return Ok(manager.GetIssueTypeLineChart(scale, projectId));
                default:
                    return BadRequest(String.Format("Chart type {0} not supported.", type));
            }
        }
    }
}
