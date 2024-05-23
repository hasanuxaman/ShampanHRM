using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SymWebUI.Areas.Common.Controllers
{
    public class RosterController : Controller
    {
        //
        // GET: /Common/Roster/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetRoster(int year, int month)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            AttendanceRosterVM mvm = new AttendanceRosterVM(); // get data from db

            //
                    List<AttendanceRosterDetailVM> test = new List<AttendanceRosterDetailVM>();
                    mvm.attendanceRosterDetails = test;
            //
            List<AttendanceRosterDetailVM> rosterDetails = mvm.attendanceRosterDetails; // get data from master
            AttendanceRosterDetailVM nvm;
            List<GroupVM> groups = new GroupRepo().DropDown(Convert.ToInt32(identity.BranchId)); // present number of group
            foreach (GroupVM item in groups)
            {
                if (rosterDetails==null)
                {
                     nvm = new AttendanceRosterDetailVM();
                     nvm.AttendanceGroupId = item.Id;
                     nvm.AttendanceGroupName = item.Name;
                    mvm.attendanceRosterDetails.Add(nvm);
                }
                else
                {
                    if (!rosterDetails.Any(x => x.AttendanceGroupId == item.Id))
                    {
                        nvm = new AttendanceRosterDetailVM();
                        nvm.AttendanceGroupId = item.Id;
                        nvm.AttendanceGroupName = item.Name;
                        mvm.attendanceRosterDetails.Add(nvm);
                    }
                }
            }
            mvm.StartDate =SymOrdinary.Ordinary.StringToDate( new DateTime(year, month, 01).ToString("yyyyMMdd"));
            mvm.EndDate = SymOrdinary.Ordinary.StringToDate(new DateTime(year, month+1, 01).AddDays(-1).ToString("yyyyMMdd"));

            ViewBag.MonthName = new DateTime(year, month, 01).ToString("MMMM");
            ViewBag.year = year;
            ViewBag.MonthNumber = month;
            ViewBag.TotalDays = DateTime.DaysInMonth(year, month);
            ViewBag.TotalGroup = new JavaScriptSerializer().Serialize(new GroupRepo().DropDown(Convert.ToInt32(identity.BranchId))) ;
            return View(mvm);
        }
        public ActionResult SetRoster(AttendanceRosterVM vm)
        {
            return View();
        }
        
    }
}
