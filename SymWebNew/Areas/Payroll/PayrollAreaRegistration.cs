using System.Web.Mvc;

namespace SymWebUI.Areas.Payroll
{
    public class PayrollAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Payroll";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Payroll_default",
                "Payroll/{controller}/{action}/{id}",
                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "SymWebUI.Areas.Payroll.Controllers" }
            );
        }
    }
}
