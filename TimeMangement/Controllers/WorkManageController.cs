using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Raven.Imports.Newtonsoft.Json;
using TimeMangement.Models;
using System.Linq;

namespace TimeMangement.Controllers
{
    public class WorkManageController : RavenController
    {
        //
        // GET: /WorkManage/

        public ActionResult Index(DateTime? date)
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

            return View("Index", month.Days[day]);
        }

        [HttpPost]
        public ActionResult Save(string saveday)
        {
            var dayInstance = JsonConvert.DeserializeObject<Day>(saveday);
            var username = User.Identity.Name.Split('@')[0];
            var day = dayInstance.Date;
            var docId = String.Format("work/{0}/{1:0000}-{2:00}", username, day.Year, day.Month);
            var month = RavenSession.Load<Month>(docId);
            month.Days[day] = dayInstance;
            RavenSession.SaveChanges();
            return Json(dayInstance);
        }

        [HttpPost]
        public ActionResult AddNewHours()
        {

            return View("Index");
        }

        [HttpPost]
        public ActionResult AddTime(string saveday)
        {
            var dayInstance = JsonConvert.DeserializeObject<Day>(saveday);
            dayInstance.TimeInfos.Add(new TimeInfo());
            return Json(dayInstance);
        }


    }
}
