using System;
using System.Collections.Generic;
using System.Web.Http;
using TimeMangement.Models;

namespace TimeMangement.Controllers.Api
{
    public class WorkManageApiController : RavenApiController
    {
        // api/workmanagement/day
        public Day GetDay(DateTime? date = null)
        {
            var username = User.Identity.Name.Split('@')[0];
            DateTime day = date ?? DateTime.Today;

            var projects = new List<string> {"RavenDB", "RavenFS"}; //TODO: take from index in raven
            var docId = String.Format("work/{0}/{1:0000}-{2:00}", username, day.Year, day.Month);
            var month = RavenSession.Load<Month>(docId);
            if (month == null)
            {
                month = new Month
                    {
                        UserName = username,
                    };
                RavenSession.Store(month, docId);
            }

            if (month.Days.ContainsKey(day) == false)
                month.Days.Add(day, new Day(day));

            return month.Days[day];
        }
// api/day/29-7-2013

    }
}
