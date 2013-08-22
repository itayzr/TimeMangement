using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Raven.Client.Indexes;
using TimeMangement.Models;

namespace TimeMangement.Indexes
{
    public class Projects_Tags : AbstractIndexCreationTask<Month, Projects_Tags.ReduceResult>
    {
        public class ReduceResult
        {
            public int Count;
            public string Project;
            public string Employee;
            public string WorkMonth;
            public string WorkYear;
            public float ProjectHours;
        }

        public Projects_Tags()
        {
            Map = months =>
                from month in months
                from day in month.Days
                from timeInfo in day.Value.TimeInfos
                where !timeInfo.Project.Equals("")
                select new
                {
                    Count = 1,
                    timeInfo.Project,
                    ProjectHours = ((TimeSpan.Parse(timeInfo.FinishTime).TotalHours - TimeSpan.Parse(timeInfo.StartTime).TotalHours)),
                    Employee=month.UserName,
                    WorkMonth=month.Id.Split('/')[2].Split('-')[1],
                    WorkYear = month.Id.Split('/')[2].Split('-')[0]
                };


            Reduce = results =>
                from reduceResult in results
                group reduceResult by new{reduceResult.Project,reduceResult.Employee,reduceResult.WorkMonth,reduceResult.WorkYear}
                    into g
                    select new
                    {
                        Count = g.Sum(x => x.Count),
                        ProjectHours = g.Sum(x => x.ProjectHours),
                        g.Key.WorkMonth,
                        g.Key.Project,
                        g.Key.Employee,
                        g.Key.WorkYear
                    };
                
        }
    }
}