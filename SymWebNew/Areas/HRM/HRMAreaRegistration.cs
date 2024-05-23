using System.Web.Mvc;

namespace SymWebUI.Areas.HRM
{
    public class HRMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HRM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HRM_default",
                "HRM/{controller}/{action}/{id}",
                  new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "SymWebUI.Areas.HRM.Controllers" }
            );
        }
    }
}
