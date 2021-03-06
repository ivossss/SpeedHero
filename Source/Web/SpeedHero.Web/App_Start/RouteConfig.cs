﻿namespace SpeedHero.Web.App_Start
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Create post",
                url: "CreatePost",
                defaults: new { controller = "Post", action = "CreatePost" },
                namespaces: new[] { "SpeedHero.Web.Controllers" });

            routes.MapRoute(
                name: "Show post",
                url: "ShowPost/{id}",
                defaults: new { controller = "Post", action = "ShowPost", id = UrlParameter.Optional },
                namespaces: new[] { "SpeedHero.Web.Controllers" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "SpeedHero.Web.Controllers" });
        }
    }
}
