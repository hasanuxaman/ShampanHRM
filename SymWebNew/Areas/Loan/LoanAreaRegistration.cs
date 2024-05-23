using System.Web.Mvc;

namespace SymWebUI.Areas.Loan
{
    public class LoanAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Loan";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Loan_default",
                "Loan/{controller}/{action}/{id}",
                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "SymWebUI.Areas.Loan.Controllers" }
            );
        }
    }
}
