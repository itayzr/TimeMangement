using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using Raven.Imports.Newtonsoft.Json;
using TimeMangement.Indexes;
using TimeMangement.Models;


namespace TimeMangement.Controllers.Api
{

    public class WorkManageApiController : RavenApiController
    {
        
        public Day GetDay(DateTime? date = null)
        {
            var username = User.Identity.Name.Split('@')[0];
            DateTime day = date ?? DateTime.Today;
            var docId = string.Format("work/{0}/{1:0000}-{2:00}", username, day.Year, day.Month);
            var month = RavenSession.Load<Month>(docId);
            if (month == null)
            {
                return new Day(day);
            }

            if (month.Days.ContainsKey(day) == false)
                return new Day(day);
                
            return month.Days[day];
         }

        
        public Month GetMonth(DateTime? monthReturn = null)
        {
            var username = User.Identity.Name.Split('@')[0];
            DateTime day = monthReturn ?? DateTime.Today;
            var docId = string.Format("work/{0}/{1:0000}-{2:00}", username, day.Year, day.Month);
            var month = RavenSession.Load<Month>(docId);
            if (month == null)
            {
                month = new Month
                {
                    UserName = username,
                };
            }

           return month;
        }

        public List<string> GetProjects(string emp=null)
        {
            var employee =emp?? User.Identity.Name.Split('@')[0];
            List<string> projects = RavenSession.Query<Projects_Tags.ReduceResult, Projects_Tags>()
                .Where(x => x.Employee == employee)
                .OrderByDescending(x => x.Count)
                .ToList()
                .Select(x => x.Project)
                .Distinct()
                .ToList();
            return projects;
        }

        public Dictionary<string, float> GetProjectsHours(string employee=null,string workMonth=null)
        {
            var month = workMonth.Split('-')[1];
            var year = workMonth.Split('-')[0];
            Dictionary<string, float> projectHours = RavenSession.Query<Projects_Tags.ReduceResult, Projects_Tags>()
                .Where(x => x.WorkYear == year)
                .Where(x => x.Employee == employee)
                .Where(x => x.WorkMonth == month) 
                .ToDictionary(result => result.Project, result => result.ProjectHours);
            return projectHours;
        }


        public Dictionary<string, float> GetProjectHourPerMonth(string year, string employee, string prj)
        {
            Dictionary<string, float> projectHours = RavenSession.Query<Projects_Tags.ReduceResult, Projects_Tags>()
                .Where(x => x.WorkYear==year)
                .Where(x => x.Employee == employee)
                .Where(x => x.Project == prj)
                .ToDictionary(result => result.WorkMonth, result => result.ProjectHours);
            return projectHours;
        }


//        public Dictionary<string, float> GetTotalHourPerMonth(string year=null, string employee=null)
//        {
////            Dictionary<string, float> totalHours = RavenSession.Query<Projects_Tags.ReduceResult, Projects_Tags>()
////                .Where(x => x.WorkYear == year)
////                .Where(x => x.Employee == employee)
////                .ToDictionary(result => result.WorkMonth, result => result.ProjectHours);
////            return totalHours;
//        }




        public List<string> GetEmployeesList()
        {
            var employees =
                    (
                        from user in RavenSession.Query<User>()
                        select user.FirstName
                    )
                        .ToList();
            return employees;
        }
            
            
        [HttpPost]
        public object Save(Day dayInput)
        {
            var username = User.Identity.Name.Split('@')[0];
            var day = dayInput.Date;
            var docId = String.Format("work/{0}/{1:0000}-{2:00}", username, day.Year, day.Month);
            var month = RavenSession.Load<Month>(docId);
            if (month == null)
            {
                month = new Month
                    {
                        UserName = username,
                    };
                RavenSession.Store(month, docId);
                month.Days.Add(day,dayInput);
                return new { Success = true };
            }
            month.Days[day] = dayInput;
            RavenSession.SaveChanges();
            return new {Success = true};
        }

        [HttpPost]
        public object Delete(Day dayInput)
        {
            var username = User.Identity.Name.Split('@')[0];
            var day = dayInput.Date;
            var docId = String.Format("work/{0}/{1:0000}-{2:00}", username, day.Year, day.Month);
            var month = RavenSession.Load<Month>(docId);
            month.Days.Remove(day);
            RavenSession.SaveChanges();
            return new { Success = true };
        }





    }
}
