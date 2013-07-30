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

        public ActionResult Index()
        {
            return View("Index");
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


    }
}
