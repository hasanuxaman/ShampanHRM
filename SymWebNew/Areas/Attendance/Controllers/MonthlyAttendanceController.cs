using SymOrdinary;
using SymRepository.Attendance;
using SymViewModel.Attendance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Attendance.Controllers
{
    public class MonthlyAttendanceController : Controller
    {
        //
        // GET: /Attendance/MonthlyAttendance/

        public ActionResult Index()
        {
            MonthlyAttendanceVM vm = new MonthlyAttendanceVM();
            return View(vm);
        }

        [HttpGet]
        public ActionResult SelectToInsert(string fid)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            MonthlyAttendanceRepo _repo = new MonthlyAttendanceRepo();
            MonthlyAttendanceVM vm = new MonthlyAttendanceVM();
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                vm.FiscalYearDetailId = Convert.ToInt32(fid);
                result = _repo.SelectToInsert(vm);
                Session["result"] = result[0] + "~" + result[1];
                return View("Index", vm);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("Index", vm);
            }
        }
    }
}
