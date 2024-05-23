using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Attendance.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Attendance/Home/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

    }
}
