using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TimeMangement
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "ApiByName",
                routeTemplate: "api/{controller}/{action}/{name}",
                defaults: null
            );

            config.Routes.MapHttpRoute(
            name: "ApiByAction",
            routeTemplate: "api/{controller}/{action}",
            defaults: new { action = "Get" }
        );



            config.Routes.MapHttpRoute(
            name: "GetProjects",
            routeTemplate: "api/{controller}/{action}/{employee}/{workMonth}",
            defaults: null

             );

            config.Routes.MapHttpRoute(
            name: "GetProjectsPerMonth",
            routeTemplate: "api/{controller}/{action}/{year}/{employee}/{prj}",
            defaults: null

             );



        }
    }
}
