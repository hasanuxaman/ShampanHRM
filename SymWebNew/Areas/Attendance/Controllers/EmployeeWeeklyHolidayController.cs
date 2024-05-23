using JQueryDataTables.Models;
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

namespace SymWebUI.Areas.Attendance.Controllers
{
    public class EmployeeWeeklyHolidayController : Controller
    {
        //
        // GET: /Attendance/EmployeeWeeklyHoliday/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        EmployeeWeeklyHolidayRepo _repo = new EmployeeWeeklyHolidayRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Attendance/Home");
            }
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            #region Column Search
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var designationFilter = Convert.ToString(Request["sSearch_3"]);
            var departmentFilter = Convert.ToString(Request["sSearch_4"]);
            var projectFilter = Convert.ToString(Request["sSearch_5"]);
            var other1Filter = Convert.ToString(Request["sSearch_6"]);
            var other2Filter = Convert.ToString(Request["sSearch_7"]);
            var other3Filter = Convert.ToString(Request["sSearch_8"]);
            var dailyDateFilter = Convert.ToString(Request["sSearch_9"]);
            var dayOfWeekFilter = Convert.ToString(Request["sSearch_10"]);

            //01     //Code
            //02     //EmpName
            //03     //Designation
            //04     //Department
            //05     //Project
            //06     //Other1
            //07     //Other2
            //08     //Other3
            //09     //DailyDate
            //10     //DayOfWeek

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (dailyDateFilter.Contains('~'))
            {
                fromDate = dailyDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(dailyDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(dailyDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = dailyDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(dailyDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(dailyDateFilter.Split('~')[1]) : DateTime.MinValue;
            }



            #endregion Column Search

            #region Search and Filter Data
            string[] cFields = { "ewh.IsActive" };
            string[] cValues = { "1" };
            var getAllData = _repo.SelectAll(0, cFields, cValues);
            IEnumerable<EmployeeWeeklyHolidayVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_9"]);
                var isSearchable10 = Convert.ToBoolean(Request["bSearchable_10"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.Other1.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.Other2.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable8 && c.Other3.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable9 && c.DailyDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable10 && c.DayOfWeek.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data

            #region Column Search

            if (codeFilter != "" || empNameFilter != "" || designationFilter != "" || departmentFilter != "" || projectFilter != ""
               || other1Filter != "" || other2Filter != "" || other3Filter != ""
               || (dailyDateFilter != "" && dailyDateFilter != "~" || dayOfWeekFilter != "")
               )
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    && (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    && (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    && (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    && (projectFilter == "" || c.Project.ToLower().Contains(projectFilter.ToLower()))
                                    && (other1Filter == "" || c.Other1.ToLower().Contains(other1Filter.ToLower()))
                                    && (other2Filter == "" || c.Other2.ToLower().Contains(other2Filter.ToLower()))
                                    && (other3Filter == "" || c.Other3.ToLower().Contains(other3Filter.ToLower()))
                                    && (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.DailyDate))
                                    && (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.DailyDate))
                                    && (dayOfWeekFilter == "" || c.DayOfWeek.ToLower().Contains(dayOfWeekFilter.ToLower()))

                                );
            }
            #endregion Column Search


            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_9"]);
            var isSortable_10 = Convert.ToBoolean(Request["bSortable_10"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeWeeklyHolidayVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? c.Project :
                sortColumnIndex == 6 && isSortable_6 ? c.Other1 :
                sortColumnIndex == 7 && isSortable_7 ? c.Other2 :
                sortColumnIndex == 8 && isSortable_8 ? c.Other3 :
                sortColumnIndex == 9 && isSortable_9 ? Ordinary.DateToString(c.DailyDate) :
                sortColumnIndex == 10 && isSortable_10 ? c.DayOfWeek :
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
                , c.Code 
                , c.EmpName 
                , c.Designation 
                , c.Department 
                , c.Project 
                , c.Other1 
                , c.Other2 
                , c.Other3 
                , c.DailyDate 
                , c.DayOfWeek 
     
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            EmployeeWeeklyHolidayVM vm = new EmployeeWeeklyHolidayVM();
            vm.Operation = "add";
            return PartialView(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(EmployeeWeeklyHolidayVM vm)
        {
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            EmployeeWeeklyHolidayVM vm = new EmployeeWeeklyHolidayVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

    }
}
