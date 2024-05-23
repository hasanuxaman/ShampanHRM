using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Payroll/Home/
        public ActionResult Index()
        {
            return View();
        }
    }
}
