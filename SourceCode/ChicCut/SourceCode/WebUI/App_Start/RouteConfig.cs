using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            
            routes.MapRoute(
                name: "khoa-hoc",
                url: "khoa-hoc/{SEOCategoryName}/{id}.html",
                defaults: new { controller = "Form", action = "Register", isCourse = true}
            );

            routes.MapRoute(
                name: "khoa-hoc-cong-dong",
                url: "khoa-hoc-cong-dong/{SEOCategoryName}/{id}.html",
                defaults: new { controller = "Form", action = "Register", isCourse = false }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces : new[] { "WebUI.Controllers" }
            );
        }
    }
}