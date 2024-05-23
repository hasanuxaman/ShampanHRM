using System.Web.Mvc;

namespace SymWebUI.Areas.GF
{
    public class GFAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "GF";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "GF_default",
                "GF/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                    namespaces: new string[] { "SymWebUI.Areas.GF.Controllers" }
            );
        }
    }
}
