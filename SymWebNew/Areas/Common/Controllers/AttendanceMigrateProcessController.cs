using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class AttendanceMigrateProcessController : Controller
    {
        //
        // GET: /Common/AttendanceMigrateProcess/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Process(string dateFrom, string dateTo)
        {

            return View();
        }
    }
}
