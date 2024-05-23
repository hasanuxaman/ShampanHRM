using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class AttendanceRosterController : Controller
    {




        //
        // GET: /Common/AttendanceRoster/

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
