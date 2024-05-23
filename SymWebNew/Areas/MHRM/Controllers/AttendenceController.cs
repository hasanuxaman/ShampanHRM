using SymOrdinary;
using SymRepository.Attendance;
using SymRepository.Common;
using SymViewModel.Attendance;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.MHRM.Controllers
{
    public class AttendenceController : Controller
    {
        //
        // GET: /MHRM/Attendence/
        CommonRepo crepo = new CommonRepo();
        public ActionResult Index()
        {
            AttLogsVM vm = new AttLogsVM();
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index(AttLogsVM vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            string[] retResults = new string[6];
            AttLogsRepo erepo = new AttLogsRepo();
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            if (vm.Self)
            {
                DateTime result = crepo.ServerDateTime();
                vm.PunchDate = Convert.ToDateTime(result).ToString("dd/MMM/yyyy");
                vm.PunchTime = Convert.ToDateTime(result).ToString("hh:mm tt");
            }
            retResults = erepo.Insert(vm);
            var mgs = retResults[0] + "~" + retResults[1];
            Session["mgs"] = "mgs";
            return View(vm);
        }
    }
}
