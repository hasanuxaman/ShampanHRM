using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Attendance.Controllers
{
    public class AttendanceMigrateProcessController : Controller
    {
        //
        // GET: /Attendance/AttendanceMigrateProcess/

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
