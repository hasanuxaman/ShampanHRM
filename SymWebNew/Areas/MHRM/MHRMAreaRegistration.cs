using System.Web.Mvc;

namespace SymWebUI.Areas.MHRM
{
    public class MHRMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MHRM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MHRM_default",
                "MHRM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
