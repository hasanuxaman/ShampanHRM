using SymOrdinary;
using SymRepository.Attendance;
using SymRepository.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class ELProcessController : Controller
    {
        //
        // GET: /Common/AttendanceMigration/
        private static Thread thread;
        string autoAttendanceMigration;

        AttendanceMigrationRepo repo = new AttendanceMigrationRepo();
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult ELProcess()
        {
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
            return View("ELProcess", vm);
        }

        [HttpGet]
        public ActionResult ELBalanceProcess(string attnDate)
        {
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
            vm.AttendanceDate = attnDate;
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            ELProcessRepo _repo = new ELProcessRepo();

            try
            {

                result = _repo.ELProcess(attnDate);
                Session["result"] = result[0] + "~" + result[1];
                return View("ELProcess", vm);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("ELProcess", vm);
            }
        }




    }


}
