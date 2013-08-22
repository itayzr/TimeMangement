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

        public ActionResult ProjectHours()
        {
            return View("ProjectHours");
        }

    }
}
