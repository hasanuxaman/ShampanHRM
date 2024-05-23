using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Attendance.Controllers
{
    public class AttendanceRosterController : Controller
    {




        //
        // GET: /Attendance/AttendanceRoster/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _index()
        {
            return View();
        }
        public ActionResult Create(string Group)
        {
            return View();
        }
        public ActionResult CreateLoad()
        {
            return View();
        }

    }
}
