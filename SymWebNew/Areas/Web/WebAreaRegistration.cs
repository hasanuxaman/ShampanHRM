﻿using System.Web.Mvc;

namespace SymWebUI.Areas.Web
{
    public class WebAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Web";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
               "Web_default",
               "Web/{controller}/{action}/{id}",
                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
               namespaces: new string[] { "SymWebUI.Areas.Web.Controllers" }
           );
        }
    }
}
