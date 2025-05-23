﻿using SymOrdinary;
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

namespace SymWebUI.Areas.Attendance.Controllers
{
    public class AttendanceDailyController : Controller
    {
        //
        // GET: /Attendance/AttendanceDaily/

        AttendanceDailyRepo repo = new AttendanceDailyRepo();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _index(JQueryDataTableParamVM param)
        {
            var getAllData = repo.SelectAll();
            IEnumerable<AttendanceDailyVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && Convert.ToString(c.Id).Contains(param.sSearch.ToLower())
                               ||isSearchable2 && c.InTime.ToLower().Contains(param.sSearch.ToLower())
                               ||isSearchable3 && c.OutTime.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.PunchInTime.ToLower().Contains(param.sSearch.ToLower())
                               ||isSearchable5 && c.PunchOutTime.ToLower().Contains(param.sSearch.ToLower())
                               ||isSearchable6 && c.DailyDate.ToLower().Contains(param.sSearch.ToLower())
                               );
            }else {filteredData = getAllData;}

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<AttendanceDailyVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? Convert.ToString(c.Id) :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.InTime :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.OutTime :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.PunchInTime :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.PunchOutTime :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.DailyDate :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                Convert.ToString(c.Id)
                , c.InTime //+ "~" + Convert.ToString(c.Id)
                , c.OutTime
                , c.PunchInTime
                , c.PunchOutTime
                , c.DailyDate
            };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(AttendanceDailyVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            try
            {
                result = repo.Insert(vm);
                Session["result"] = result[0] + "~" + result[1];
                if (result[0].ToLower() == "success")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                    return View();
                }

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Save Failed";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View();
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            return View(repo.SelectById(id));
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(AttendanceDailyVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {

                result = repo.Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                if (result[0].ToLower() == "success")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                    return View(vm);
                }

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not ucceessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(vm);
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult Delete(string ids)
        {
            AttendanceDailyVM vm = new AttendanceDailyVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
