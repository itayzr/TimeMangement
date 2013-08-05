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

        }
    }
}
