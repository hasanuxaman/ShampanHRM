using System.Web.Mvc;

namespace SymWebUI.Areas.TAX
{
    public class TAXAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "TAX";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "TAX_default",
                "TAX/{controller}/{action}/{id}",
                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "SymWebUI.Areas.TAX.Controllers" }
            );
        }
    }
}
