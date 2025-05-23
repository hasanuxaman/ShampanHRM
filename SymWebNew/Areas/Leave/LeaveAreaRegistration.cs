﻿using System.Web.Mvc;

namespace SymWebUI.Areas.Leave
{
    public class LeaveAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Leave";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Leave_default",
                "Leave/{controller}/{action}/{id}",
                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "SymWebUI.Areas.Leave.Controllers" }
            );
        }
    }
}
