using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeMangement.Models;
using AttributeRouting.Web.Mvc;

namespace TimeMangement.Controllers
{
    public class HomeController : RavenController
    {

        public ActionResult Index()
        {
            //RavenSession.Store(david);
            return View();
        }

        [GET("account/Create")]
        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [POST("account/Create")]
        public ActionResult Create(User model)
        {
           // Dictionary<DateTime, List<WorkDayModel>>

            RavenSession.Store(new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                  //  Birtyday = model.Birtyday
                });
            
            return Content("Created User");
        }



        public ActionResult Time()
        {
            return Content(DateTime.Now.ToString());
        }

    }

 
}
