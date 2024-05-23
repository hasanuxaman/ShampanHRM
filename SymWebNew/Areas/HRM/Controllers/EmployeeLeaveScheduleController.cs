using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Attendance;
using SymRepository.Common;
using SymRepository.HRM;
using SymRepository.Leave;
using SymViewModel.Attendance;
using SymViewModel.HRM;
using SymViewModel.Leave;
using SymWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    public class EmployeeLeaveScheduleController : Controller
    {
        //
        // GET: /HRM/EmployeeLeave/
        EmployeeLeaveRepo elrepo = new EmployeeLeaveRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        private static Thread thread;

        #region Index Actions

        public ActionResult Index()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            ViewBag.permission = _reposur.SymRoleSession(identity.UserId, "1_23", "index").ToString();
            if (Session["mgs"].ToString() != "")
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            Session["empleaveId"] = "0";
            return View();
        }

        public ActionResult _index(JQueryDataTableParamModel param, string status, string empId)
        {
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var NameFilter = Convert.ToString(Request["sSearch_2"]);
            var LeaveTypeFilter = Convert.ToString(Request["sSearch_3"]);
            var FromdateFilter = Convert.ToString(Request["sSearch_4"]);
            var TodateFilter = Convert.ToString(Request["sSearch_5"]);
            var DateTypeFilter = Convert.ToString(Request["sSearch_6"]);
            var IsApproveFilter = Convert.ToString(Request["sSearch_7"]);
            DateTime FromfromDate = DateTime.MinValue;
            DateTime FromtoDate = DateTime.MaxValue;
            DateTime TofromDate = DateTime.MinValue;
            DateTime TotoDate = DateTime.MaxValue;
            if (FromdateFilter.Contains('~'))
            {
                //Split date range filters with ~
                FromfromDate = FromdateFilter.Split('~')[0] == "" ? DateTime.MinValue : Convert.ToDateTime(FromdateFilter.Split('~')[0]);
                FromtoDate = FromdateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Convert.ToDateTime(FromdateFilter.Split('~')[1]);
            }
            if (TodateFilter.Contains('~'))
            {
                //Split date range filters with ~
                TofromDate = TodateFilter.Split('~')[0] == "" ? DateTime.MinValue : Convert.ToDateTime(TodateFilter.Split('~')[0]);
                TotoDate = TodateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Convert.ToDateTime(TodateFilter.Split('~')[1]);
            }

            List<EmployeeLeaveVM> getAllData = new List<EmployeeLeaveVM>();
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (identity.IsAdmin || identity.IsHRM)
            {
                getAllData = elrepo.SelectAllFromSchedule();
            }
            else
            {
                getAllData = elrepo.SelectAllfromSchedule(Identit.EmployeeId, "");
            }
            IEnumerable<EmployeeLeaveVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.EmpCode.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.LeaveType_E.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.FromDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.ToDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.DayType.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.Approval.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            if (codeFilter != "" || NameFilter != "" || LeaveTypeFilter != "" || (FromdateFilter != "~" && FromdateFilter != "")
                || (TodateFilter != "" && TodateFilter != "~") || DateTypeFilter != "" || IsApproveFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.EmpCode.ToLower().Contains(codeFilter.ToLower()))
                                            && (NameFilter == "" || c.EmpName.ToLower().Contains(NameFilter.ToLower()))
                                             && (LeaveTypeFilter == "" || c.LeaveType_E.ToLower().Contains(LeaveTypeFilter.ToLower()))
                                            && (FromfromDate == DateTime.MinValue || FromfromDate < Convert.ToDateTime(c.FromDate))
                                                && (FromtoDate == DateTime.MaxValue || Convert.ToDateTime(c.FromDate) < FromtoDate)
                                                 && (TofromDate == DateTime.MinValue || TofromDate < Convert.ToDateTime(c.ToDate))
                                                && (TotoDate == DateTime.MaxValue || Convert.ToDateTime(c.ToDate) < TotoDate)
                                                  && (DateTypeFilter == "" || c.DayType.ToLower().Contains(DateTypeFilter.ToLower()))
                                              && (IsApproveFilter == "" || c.Approval.ToLower().Contains(IsApproveFilter.ToLower()))
                                        );
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeLeaveVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.EmpCode :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.LeaveType_E :
                                                           sortColumnIndex == 4 && isSortable_4 ? Ordinary.DateToString(c.FromDate) :
                                                           sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.ToDate) :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.DayType :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.Approval :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {  
                             Convert.ToString(c.IsApprove)+ "~" + Convert.ToString(c.IsReject)+ "~" +   Convert.ToString(c.Id)
                            //, Convert.ToString(c.IsReject)+ "~" +   Convert.ToString(c.Id)
                            , c.EmpCode 
                            , c.EmpName
                            , c.LeaveType_E
                            , c.FromDate
                            , c.ToDate
                            , c.DayType
                            , c.Approval
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

        public ActionResult _indexByEmployeeId(JQueryDataTableParamModel param, string empId)
        {
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var NameFilter = Convert.ToString(Request["sSearch_2"]);
            var LeaveTypeFilter = Convert.ToString(Request["sSearch_3"]);
            var FromdateFilter = Convert.ToString(Request["sSearch_4"]);
            var TodateFilter = Convert.ToString(Request["sSearch_5"]);
            var DateTypeFilter = Convert.ToString(Request["sSearch_6"]);
            var IsApproveFilter = Convert.ToString(Request["sSearch_7"]);
            var v = Convert.ToString(Request["sSearch_8"]);
            DateTime FromfromDate = DateTime.MinValue;
            DateTime FromtoDate = DateTime.MaxValue;
            DateTime TofromDate = DateTime.MinValue;
            DateTime TotoDate = DateTime.MaxValue;
            if (FromdateFilter.Contains('~'))
            {
                //Split date range filters with ~
                FromfromDate = FromdateFilter.Split('~')[0] == "" ? DateTime.MinValue : Convert.ToDateTime(FromdateFilter.Split('~')[0]);
                FromtoDate = FromdateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Convert.ToDateTime(FromdateFilter.Split('~')[1]);
            }
            if (TodateFilter.Contains('~'))
            {
                //Split date range filters with ~
                TofromDate = TodateFilter.Split('~')[0] == "" ? DateTime.MinValue : Convert.ToDateTime(TodateFilter.Split('~')[0]);
                TotoDate = TodateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Convert.ToDateTime(TodateFilter.Split('~')[1]);
            }
            List<EmployeeLeaveVM> getAllData = new List<EmployeeLeaveVM>();
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            getAllData = elrepo.SelectScheduleByEmployeeId(empId);
            IEnumerable<EmployeeLeaveVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.EmpCode.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.LeaveType_E.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.FromDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.ToDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.DayType.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.Approval.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            if (codeFilter != "" || NameFilter != "" || LeaveTypeFilter != "" || (FromdateFilter != "~" && FromdateFilter != "")
                || (TodateFilter != "" && TodateFilter != "~") || DateTypeFilter != "" || IsApproveFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.EmpCode.ToLower().Contains(codeFilter.ToLower()))
                                            && (NameFilter == "" || c.EmpName.ToLower().Contains(NameFilter.ToLower()))
                                             && (LeaveTypeFilter == "" || c.LeaveType_E.ToLower().Contains(LeaveTypeFilter.ToLower()))
                                            && (FromfromDate == DateTime.MinValue || FromfromDate < Convert.ToDateTime(c.FromDate))
                                                && (FromtoDate == DateTime.MaxValue || Convert.ToDateTime(c.FromDate) < FromtoDate)
                                                 && (TofromDate == DateTime.MinValue || TofromDate < Convert.ToDateTime(c.ToDate))
                                                && (TotoDate == DateTime.MaxValue || Convert.ToDateTime(c.ToDate) < TotoDate)
                                                  && (DateTypeFilter == "" || c.DayType.ToLower().Contains(DateTypeFilter.ToLower()))
                                              && (IsApproveFilter == "" || c.Approval.ToLower().Contains(IsApproveFilter.ToLower()))
                                        );
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeLeaveVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.EmpCode :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.LeaveType_E :
                                                           sortColumnIndex == 4 && isSortable_4 ? Ordinary.DateToString(c.FromDate) :
                                                           sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.ToDate) :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.DayType :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.Approval :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {  
                            Convert.ToString(c.IsApprove)+ "~" + Convert.ToString(c.IsReject)+ "~" +   Convert.ToString(c.Id)
                            , c.EmpCode 
                            , c.EmpName
                            , c.LeaveType_E
                            , c.FromDate
                            , c.ToDate
                            , c.DayType
                            , c.Approval
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

        [HttpGet]
        public ActionResult Leave(string empleaveId = null)
        {
            if (Session["mgs"] as string != "")
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_22", "process").ToString();
            EmployeeLeaveVM vm = new EmployeeLeaveVM();
            SettingRepo _setDAL = new SettingRepo();

            bool IsHolyDayLeaveSkip = _setDAL.settingValue("HRM", "IsHolyDayLeaveSkip") == "Y" ? true : false;

            if (!string.IsNullOrWhiteSpace(empleaveId))
            {
                vm = elrepo.SelectScheduleById(Convert.ToInt32(empleaveId));

            }
            vm.IsHolyDayLeaveSkip = IsHolyDayLeaveSkip;
            Session["empleaveId"] = empleaveId == null ? "0" : empleaveId;
            return View(vm);
        }

        public ActionResult _LeaveIndex(JQueryDataTableParamModel param, string status)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_5"]);
            //var isActiveFilter = Convert.ToString(Request["sSearch_6"]);
            //Code
            //EmpName 
            //Department 
            //Designation
            //JoinDate
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
            }
            //var isActiveFilter1 = isActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();
            var fromID = 0;
            var toID = 0;
            if (idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }
            #endregion Column Search
            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            List<EmployeeInfoVM> getAllData = new List<EmployeeInfoVM>();
            IEnumerable<EmployeeInfoVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (identity.IsAdmin || identity.IsHRM)
            {
                getAllData = _empRepo.SelectAllActiveEmp();
            }
            else
            {
                getAllData.Add(_empRepo.SelectById(Identit.EmployeeId));
                //RedirectToAction(
                //return RedirectToAction("LeavedetailForSearch", new { id = Identit.EmployeeId, empcode = "", btn = "current" });
                //LeavedetailForSearch(string id, string empcode = "", string btn = "current")

            }
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.JoinDate.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || (joinDateFilter != "" && joinDateFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    &&
                                    (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    &&
                                    (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    &&
                                    (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    &&
                                    (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
                                    &&
                                    (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.JoinDate))
                                );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                  sortColumnIndex == 1 && isSortable_1 ? c.Code :
                  sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                  sortColumnIndex == 3 && isSortable_3 ? c.Department :
                  sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                  sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.JoinDate) :
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
                , c.Department 
                , c.Designation
                , c.JoinDate
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

        public ActionResult LeaveSupervisor()
        {
            if (Session["mgs"].ToString() != "")
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_24", "index").ToString();
            //List<EmployeeLeaveVM> VMs = new List<EmployeeLeaveVM>();
            //VMs = elrepo.SelectAll();
            //return View(VMs);
            Session["Supervisor"] = "False";
            ViewBag.Supervisor = "Y";
            return View();
        }

        public ActionResult _LeaveSupervisorindex(JQueryDataTableParamModel param)
        {
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var NameFilter = Convert.ToString(Request["sSearch_2"]);
            var LeaveTypeFilter = Convert.ToString(Request["sSearch_3"]);
            var FromdateFilter = Convert.ToString(Request["sSearch_4"]);
            var TodateFilter = Convert.ToString(Request["sSearch_5"]);
            var DateTypeFilter = Convert.ToString(Request["sSearch_6"]);
            var SupervisorFilter = Convert.ToString(Request["sSearch_7"]);
            var IsApproveFilter = Convert.ToString(Request["sSearch_8"]);


            DateTime FromfromDate = DateTime.MinValue;
            DateTime FromtoDate = DateTime.MaxValue;
            DateTime TofromDate = DateTime.MinValue;
            DateTime TotoDate = DateTime.MaxValue;
            if (FromdateFilter.Contains('~'))
            {
                //Split date range filters with ~
                FromfromDate = FromdateFilter.Split('~')[0] == "" ? DateTime.MinValue : Convert.ToDateTime(FromdateFilter.Split('~')[0]);
                FromtoDate = FromdateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Convert.ToDateTime(FromdateFilter.Split('~')[1]);
            }
            if (TodateFilter.Contains('~'))
            {
                //Split date range filters with ~
                TofromDate = TodateFilter.Split('~')[0] == "" ? DateTime.MinValue : Convert.ToDateTime(TodateFilter.Split('~')[0]);
                TotoDate = TodateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Convert.ToDateTime(TodateFilter.Split('~')[1]);
            }
            List<EmployeeLeaveVM> getAllData = new List<EmployeeLeaveVM>();
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;


            getAllData = elrepo.SelectAllForSupervisor(Identit.EmployeeId);
            if (getAllData.Count > 0)
            {
                Session["Supervisor"] = "True";

            }
            //}

            IEnumerable<EmployeeLeaveVM> filteredData;
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

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.EmpCode.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.LeaveType_E.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.FromDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.ToDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.DayType.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.Supervisor.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable8 && c.Approval.ToLower().Contains(param.sSearch.ToLower())

                               );
            }
            else
            {
                filteredData = getAllData;
            }
            if (codeFilter != "" || NameFilter != "" || LeaveTypeFilter != "" || (FromdateFilter != "~" && FromdateFilter != "")
                || (TodateFilter != "" && TodateFilter != "~") || DateTypeFilter != "" || SupervisorFilter != "" || IsApproveFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.EmpCode.ToLower().Contains(codeFilter.ToLower()))
                                            && (NameFilter == "" || c.EmpName.ToLower().Contains(NameFilter.ToLower()))
                                             && (LeaveTypeFilter == "" || c.LeaveType_E.ToLower().Contains(LeaveTypeFilter.ToLower()))
                                            && (FromfromDate == DateTime.MinValue || FromfromDate < Convert.ToDateTime(c.FromDate))
                                                && (FromtoDate == DateTime.MaxValue || Convert.ToDateTime(c.FromDate) < FromtoDate)
                                                 && (TofromDate == DateTime.MinValue || TofromDate < Convert.ToDateTime(c.ToDate))
                                                && (TotoDate == DateTime.MaxValue || Convert.ToDateTime(c.ToDate) < TotoDate)
                                                  && (DateTypeFilter == "" || c.DayType.ToLower().Contains(DateTypeFilter.ToLower()))
                                              && (SupervisorFilter == "" || c.Supervisor.ToLower().Contains(SupervisorFilter.ToLower()))
                                              && (IsApproveFilter == "" || c.Approval.ToLower().Contains(IsApproveFilter.ToLower()))

                                        );
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeLeaveVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.EmpCode :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.LeaveType_E :
                                                           sortColumnIndex == 4 && isSortable_4 ? Ordinary.DateToString(c.FromDate) :
                                                           sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.ToDate) :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.DayType :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.Supervisor :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.Approval :

                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {  
                             Convert.ToString(c.IsApprove)+ "~" + Convert.ToString(c.IsReject)+ "~" +   Convert.ToString(c.Id)
                            //, Convert.ToString(c.IsReject)+ "~" +   Convert.ToString(c.Id)
                            , c.EmpCode 
                            , c.EmpName
                            , c.LeaveType_E
                            , c.FromDate
                            , c.ToDate
                            , c.DayType
                            , c.Supervisor
                            , c.Approval
                       
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

        #region Backup

        public ActionResult Index1(string code, string status)
        {
            return View(elrepo.SelectAll(code, status));
        }

        #endregion

        #endregion

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult LeaveApprove(string parm, string Code, string Id = "", string IsSupervisor = "")
        {
            #region Objects and Variables

            Session["mgs"] = "mgs";
            var mgs = "";
            string[] result = new string[6];
            EmployeeLeaveVM vm = new EmployeeLeaveVM();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string paramStatus = "";
            string paramReject = "";
            string paramApprove = "";
            int paramLength = 0;
            #endregion

            try
            {
                #region CheckPoint

                if (parm.Contains("~"))
                {
                    paramLength = parm.Split('~').Length;
                    paramStatus = parm.Split('~')[1].ToLower();
                    paramReject = parm.Split('~')[2].ToLower();
                    paramApprove = parm.Split('~')[3].ToLower();
                }

                if (paramStatus.ToLower() == "reject" && paramApprove == "false")
                {
                    if (paramReject == "true")
                    {
                        mgs = "Fail~This Leave already Rejected!";
                        goto Redirect;
                        ////return RedirectToAction("Index", new { mgs = mgs });
                    }
                }
                if (paramStatus.ToLower() == "approve" && paramApprove == "true")
                {
                    mgs = "Fail~This Leave already Approved!";
                    goto Redirect;

                    ////return RedirectToAction("Index", new { mgs = mgs });
                }
                if (paramStatus.ToLower() == "approve" && paramReject == "true")
                {
                    mgs = "Fail~This Leave already Rejected! Can't Approve!";
                    goto Redirect;

                    ////return RedirectToAction("Index", new { mgs = mgs });
                }
                #endregion

                #region Data Assign
                vm.Id = Convert.ToInt32(parm.Split('~')[0]);

                if (paramStatus.ToLower() == "reject")
                {
                    vm.IsApprove = false;
                    vm.IsReject = true;
                    vm.ApprovedBy = "";
                    vm.RejectedBy = Session["EmployeeId"].ToString();
                }
                else
                {
                    vm.IsApprove = true;
                    vm.IsReject = false;
                    vm.ApprovedBy = Session["EmployeeId"].ToString();
                    vm.RejectedBy = "";
                }
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                #endregion

                result = elrepo.Approve(vm);

                result = elrepo.LeaveSchedule(vm);

                mgs = result[0] + "~" + result[1];

                #region Email Send

                if (paramStatus == "approve" || paramStatus == "reject" && result[0] == "Success")
                {
                    EmailSettings emailsettings = new EmailSettings();
                    thread = new Thread(unused => EmpLeaveApproveEmailProcess(Code, Id));
                    thread.Start();
                }
                #endregion

            Redirect:

                if (!string.IsNullOrWhiteSpace(IsSupervisor))
                {
                    return RedirectToAction("LeaveSupervisor", new { mgs = mgs });
                }
                else
                {
                    return RedirectToAction("Index", new { mgs = mgs });
                }

            }
            catch (Exception)
            {
                mgs = "Fail" + "~" + "Action Not Successfull!";
                if (!string.IsNullOrWhiteSpace(IsSupervisor))
                {
                    return RedirectToAction("LeaveSupervisor", new { mgs = mgs });
                }
                else
                {
                    return RedirectToAction("Index", new { mgs = mgs });
                }
            }
        }

        [HttpGet]
        public ActionResult LeaveEdit(string parm, string IsSupervisor = "")
        {
            if (parm.Split('~')[1].ToLower() == "true")
            {
                var mgs = "Fail~This Leave already Approved, After Approved leave not editable";
                Session["mgs"] = "mgs";
                return RedirectToAction("Index", new { mgs = mgs });
            }
            EmployeeLeaveRepo repoel = new EmployeeLeaveRepo();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeLeaveVM employeeLeaveVM = repoel.SelectById(Convert.ToInt32(parm.Split('~')[0]));
            if (employeeLeaveVM.IsApprove)
            {
                var mgs = "Fail~This Leave already Approved, After Approved leave not editable";
                Session["mgs"] = "mgs";
                return RedirectToAction("Leave", new { mgs = mgs });
            }
            EmployeeInfoVM vm = repo.SelectById(employeeLeaveVM.EmployeeId);
            ////should Check Permanent of Not: From Setting
            SettingRepo _sRepo = new SettingRepo();
            string parmanentCheck = "";
            parmanentCheck = _sRepo.settingValue("Leave", "ParmanentCheck");
            if (parmanentCheck == "Y")
            {
                if (!vm.IsPermanent)
                {
                    var empleaveId = parm.Split('~')[0].ToLower();
                    var mgs = "Fail~This Employee not Permanent";
                    Session["mgs"] = "mgs";
                    return RedirectToAction("Index", new { mgs = mgs });
                }
            }

            EmployeeLeaveRepo _levRepo = new EmployeeLeaveRepo();
            List<EmployeeLeaveBalanceVM> employeeLeaveBalanceVMs = _levRepo.EmployeeLeaveBalance(employeeLeaveVM.EmployeeId, Session["SessionYear"].ToString());
            if (employeeLeaveBalanceVMs.Count <= 0)
            {
                var mgs = "Fail~This Employee have not Leave Assign";
                Session["mgs"] = "mgs";
                return RedirectToAction("Leave", new { mgs = mgs });
            }
            employeeLeaveVM.RedirectPage = "EmployeeLeave";
            vm.employeeLeaveVM = employeeLeaveVM;
            vm.employeeLeaveBalanceVMs = employeeLeaveBalanceVMs;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Leave(EmployeeLeaveVM vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            SymUserRoleRepo _reposur = new SymUserRoleRepo();
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_22", "process").ToString();
            string[] retResults = new string[6];
            EmployeeLeaveRepo repo = new EmployeeLeaveRepo();
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LeaveYear = Convert.ToInt32(Convert.ToDateTime(vm.FromDate).ToString("yyyy"));
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.Id = Convert.ToInt32(Session["empleaveId"]);
            //vm.IsApprove=User.IsInRole("Admin");
            if (vm.IsApprove == true)
            {
                vm.ApprovedBy = Session["EmployeeId"].ToString();
                vm.ApproveDate = DateTime.Now.ToString("dd/MMM/yyyy");
                vm.ToDate = vm.ToDate;
            }

            //List<DateTime> businessDays = Ordinary.GetBusinessDaysDate(new DateTime(2023, 2, 20), new DateTime(2023, 2, 28), "Friday", "Saturday");


            retResults = repo.InsertSchedule(vm);

            #region Email Sending
            if (retResults[0].ToLower() == "success")
            {
                if (vm.SaveType == "email")
                {
                    EmailSettings emailsettings = new EmailSettings();
                    thread = new Thread(unused => EmpLeaveApplyEmailProcess("", retResults[2], vm.EmployeeId));
                    thread.Start();
                }

            }
            #endregion Email Sending



            var mgs = retResults[0] + "~" + retResults[1];
            Session["result"] = mgs;

            if (!string.IsNullOrWhiteSpace(vm.RedirectPage))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Leave");

            }
        }

        ////public ActionResult EmployeeLeavedetailForSearch(string Id = "", string empleaveId = "0", string empcode = "", string btn = "current", string leaveyear = "0")
        public ActionResult EmployeeLeavedetailForSearch(EmployeeLeaveVM vm)
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeLeaveRepo _levRepo = new EmployeeLeaveRepo();
            EmployeeInfoVM varEmployeeInfoVM = new EmployeeInfoVM();
            ////EmployeeLeaveVM empleaveVM = new EmployeeLeaveVM();
            //EmployeeLeaveVM employeeLeaveVM = repoel.SelectById(Convert.ToInt32(parm.Split('~')[0]));
            if (vm.LeaveYear == 0)
            {
                vm.LeaveYear = Convert.ToInt32(Session["SessionYear"]);
            }
            if (!string.IsNullOrEmpty(vm.EmployeeId))
            {
                varEmployeeInfoVM = repo.SelectById(vm.EmployeeId);
            }
            if (!string.IsNullOrWhiteSpace(vm.EmpCode) && !string.IsNullOrWhiteSpace(vm.Button))
            {
                varEmployeeInfoVM = repo.SelectEmpStructure(vm.EmpCode, vm.Button);
            }
            varEmployeeInfoVM.employeeLeaveBalanceVMs = _levRepo.EmployeeLeaveBalance(varEmployeeInfoVM.Id, vm.LeaveYear.ToString());

            varEmployeeInfoVM.empleavevm = vm;
            if (vm.Id > 0)
            {
                varEmployeeInfoVM.empleavevm = _levRepo.SelectById(Convert.ToInt32(vm.Id));
            }
            varEmployeeInfoVM.empleavevm.RedirectPage = vm.RedirectPage;
            Session["empleaveId"] = vm.Id;
            ModelState.Clear();
            return PartialView("_leaveDetailForSearch", varEmployeeInfoVM);
        }

        [HttpPost]
        public ActionResult LeaveEdit(EmployeeLeaveVM vm)
        {
            #region Objects and Variables
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_23", "process").ToString();
            string[] retResults = new string[6];
            EmployeeLeaveRepo repo = new EmployeeLeaveRepo();
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.LeaveYear = Convert.ToInt32(Convert.ToDateTime(vm.FromDate).ToString("yyyy"));
            var mgs = "";
            #endregion

            if (vm.IsArchive == true)
            {
                retResults = repo.Delete(vm);
                mgs = retResults[0] + "~" + retResults[1];
            }
            if (vm.IsApprove == true)
            {
                vm.ApprovedBy = Session["EmployeeId"].ToString();
                vm.ApproveDate = DateTime.Now.ToString("dd/MMM/yyyy");
                vm.ToDate = vm.FromDate;
                retResults = repo.Update(vm);
                mgs = retResults[0] + "~" + retResults[1];
            }
            Session["mgs"] = "mgs";
            return RedirectToAction("Index", new { mgs = mgs });
        }

        [HttpGet]
        public ActionResult _leaveDetails(string parm)
        {
            int Id = Convert.ToInt32(parm.Split('~')[0]);
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                string Employee = "N";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    Employee = "Y";
                }
                ViewBag.Employee = Employee;
                EmployeeLeaveVM empleavm = new EmployeeLeaveVM();
                EmployeeInfoVM vm = new EmployeeInfoVM();
                empleavm = elrepo.SelectById(Id);
                EmployeeInfoRepo repo = new EmployeeInfoRepo();
                vm = repo.SelectById(empleavm.EmployeeId);
                vm.empleavevm = empleavm;
                //}
                return View(vm);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult EmployeeLeavedetaillistForSearch(int? Id, string empcode = "", string btn = "current")
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeLeaveVM empleave = new EmployeeLeaveVM();
            if (Id != null)
            {
                empleave = elrepo.SelectById(Convert.ToInt32(Id));
                vm = repo.SelectById(empleave.EmployeeId);
            }
            if (!string.IsNullOrWhiteSpace(empcode) && !string.IsNullOrWhiteSpace(btn))
            {
                vm = repo.SelectEmpForSearch(empcode, btn);
                empleave = elrepo.SelectByEMPId(vm.Id);
            }
            vm.empleavevm = empleave;
            return PartialView("_leaveDetailListForSearch", vm);
        }

        public ActionResult LeavedetailForSearch(string id, string btn = "current", string leaveyear = "0", string empcode = "")
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeLeaveRepo _levRepo = new EmployeeLeaveRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeLeaveVM empleave = new EmployeeLeaveVM();
            List<EmployeeLeaveBalanceVM> employeeLeaveBalanceVMs = new List<EmployeeLeaveBalanceVM>();
            if (leaveyear == "0")
            {
                leaveyear = Session["SessionYear"].ToString();
            }
            if (!string.IsNullOrEmpty(id))
            {
                vm = repo.SelectById(id);
            }
            if (!string.IsNullOrWhiteSpace(empcode) && !string.IsNullOrWhiteSpace(btn))
            {
                vm = repo.SelectEmpStructure(empcode, btn);
            }
            if (vm.Id != null)
            {
                vm.employeeLeaveBalanceVMs = _levRepo.EmployeeLeaveBalance(vm.Id, leaveyear);
            }
            else
            {
                vm.employeeLeaveBalanceVMs = employeeLeaveBalanceVMs;
            }
            vm.EmployeeId = vm.Id;
            SettingRepo _sRepo = new SettingRepo();
            string parmanentCheck = "";
            parmanentCheck = _sRepo.settingValue("Leave", "ParmanentCheck");
            if (parmanentCheck == "Y")
            {
                if (!vm.IsPermanent)
                {
                    //var empleaveId = parm.Split('~')[0].ToLower();
                    var mgs = "Fail#~#This Employee not Permanent";
                    //Session["mgs"] = "mgs";
                    //return RedirectToAction("Leave", empleaveId, new { mgs = mgs });
                    //return RedirectToAction("Leave", new { mgs = mgs });
                    return Json(mgs, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("_LeaveIndex");
                }
            }

            empleave.EmployeeId = vm.Id;
            vm.empleavevm = empleave;
            return PartialView("_leaveDetailForSearch", vm);
        }
        public ActionResult SingleLeavedetailForSearch(string id, string empcode = "", string btn = "current", string leaveyear = "0")
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeLeaveRepo _levRepo = new EmployeeLeaveRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeLeaveVM empleave = new EmployeeLeaveVM();
            List<EmployeeLeaveBalanceVM> employeeLeaveBalanceVMs = new List<EmployeeLeaveBalanceVM>();
            if (leaveyear == "0")
            {
                leaveyear = Session["SessionYear"].ToString();
            }
            if (!string.IsNullOrEmpty(id))
            {
                vm = repo.SelectLeaveScheduleById(id);
            }
            if (!string.IsNullOrWhiteSpace(empcode) && !string.IsNullOrWhiteSpace(btn))
            {
                vm = repo.SelectEmpStructure(empcode, btn);
            }
            if (vm.Id != null)
            {
                vm.employeeLeaveBalanceVMs = _levRepo.EmployeeLeaveBalance(vm.Id, leaveyear);
            }
            else
            {
                vm.employeeLeaveBalanceVMs = employeeLeaveBalanceVMs;
            }
            vm.EmployeeId = vm.Id;
            SettingRepo _sRepo = new SettingRepo();
            string parmanentCheck = "";
            //parmanentCheck = _sRepo.settingValue("Leave", "ParmanentCheck");
            //if (parmanentCheck == "Y")
            //{
            //    if (!vm.IsPermanent)
            //    {
            //        //var empleaveId = parm.Split('~')[0].ToLower();
            //        var mgs = "Fail#~#This Employee not Permanent";
            //        //Session["mgs"] = "mgs";
            //        //return RedirectToAction("Leave", empleaveId, new { mgs = mgs });
            //        //return RedirectToAction("Leave", new { mgs = mgs });
            //        return Json(mgs, JsonRequestBehavior.AllowGet);
            //        //return RedirectToAction("_LeaveIndex");
            //    }
            //}            

            empleave.EmployeeId = vm.Id;
            vm.empleavevm = empleave;
            return PartialView("_indexSingleLeave", vm);
        }
        #region Data Check and MISC

        public static string DataTableToJson(System.Data.DataTable dataTable)
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataTable, Newtonsoft.Json.Formatting.Indented);
                return json;
            }
            catch (Exception)
            {
                return "";
            }

        }

        public ActionResult LeaveBalance(string EmployeeId, string leaveyear = "0")
        {
            EmployeeLeaveRepo _levRepo = new EmployeeLeaveRepo();
            List<EmployeeLeaveBalanceVM> vms = new List<EmployeeLeaveBalanceVM>();
            vms = _levRepo.EmployeeLeaveBalance(EmployeeId, leaveyear);
            if (vms.Count > 0)
            {
                return PartialView("_leaveBalance", vms);
            }
            else
            {
                var mgs = "Fail~This Employee have not Leave Assign";
                Session["mgs"] = "mgs";
                return RedirectToAction("Leave", new { mgs = mgs });
            }
        }

        public JsonResult CheckEmployeeLeaveBalance(string employeeId, string leaveType, string year, decimal totalDay)
        {
            int Year = Convert.ToInt32(Convert.ToDateTime(year).ToString("yyyy"));
            return Json(new EmployeeLeaveStructureRepo().CheckEmployeeLeaveBalance(employeeId, leaveType, Year, totalDay), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckHoliday(string FromDate, string ToDate)
        {
            try
            {
                string[] conditionFields = { "HoliDay>", "HoliDay<" };
                string[] conditionValues = { Ordinary.DateToString(FromDate), Ordinary.DateToString(ToDate) };

                List<HoliDayVM> VMs = new List<HoliDayVM>();
                VMs = new HoliDayRepo().SelectAll(conditionFields, conditionValues);
                string holidays = "";
                foreach (var item in VMs)
                {
                    holidays += item.HoliDay + ":" + item.HoliDayType + "\r\n";
                }
                return Json(holidays, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public JsonResult DayCountSkipHoliday(string employeeId, string FromDate, string ToDate)
        {
            try
            {

                var employeeJob = new EmployeeJobRepo().SelectByEmployee(employeeId);
                List<DateTime> businessDays = Ordinary.GetBusinessDaysDate(Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate), employeeJob.FirstHoliday ?? "Friday", employeeJob.SecondHoliday ?? "Friday");

                List<DateTime> holidays = new HoliDayRepo().SelectAllHoliDate();

                List<DateTime> filteredDates = businessDays.Where(d => !holidays.Contains(d)).ToList();
                return Json(filteredDates.Count(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ActionResult EmployeeInfo(string Id)
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeVM vm = new EmployeeVM();
            if (!string.IsNullOrWhiteSpace(Id))
            {
                vm = repo.EmployeeInfo(Id);
                if (vm.IsPermanent)
                {
                    return PartialView("_employee", vm);
                }
                else
                {
                    Session["result"] = "Fail~This Employee not Permanent";
                    return RedirectToAction("Leave");
                }
            }
            else
            {
                return PartialView("_employee", vm);
            }
        }

        public ActionResult _indexSingle(string Id)
        {
            return PartialView("_indexSingle");
        }

        #endregion

        #region Email Actions

        [Authorize]
        public ActionResult LeaveEmail(string Code, string EmployeeId = "", string Id = "")
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                //Statement == LeaveEmail
                string[] result = new string[6];


                string FullPeriodName = "";
                EmailSettings emailsettings = new EmailSettings();
                thread = new Thread(unused => EmpLeaveApplyEmailProcess(Code, Id));
                thread.Start();
                // EmpEmailProcess(ds, doc, FullPeriodName)
                result[0] = "Successfully";
                result[1] = "Leave Email Sent";
                Session["result"] = result[0] + "~" + result[1];
                //return Redirect("/Acc/Home/");
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void EmpLeaveApplyEmailProcess(string Code = "", string Id = "", string EmployeeId = "")
        {
            //Fetch Employee Leave Info for this Id
            EmployeeLeaveVM empLeaveVM = new EmployeeLeaveVM();
            empLeaveVM = elrepo.SelectById(Convert.ToInt32(Id));

            //Fetch Admin User Info
            UserInformationRepo _repo = new UserInformationRepo();
            List<EmployeeInfoVM> getAllData = new List<EmployeeInfoVM>();
            //Open Later  //getAllData = _repo.SelectAllAdminUser();

            //Fetch SuperVisor Info
            EmployeeJobVM empJobVM = new EmployeeJobVM();
            EmployeeJobRepo _empJobRepo = new EmployeeJobRepo();
            //if (!string.IsNullOrWhiteSpace(EmployeeId))
            //{
            //    empJobVM = _empJobRepo.SelectByEmployee(EmployeeId);
            //}
            //else
            //{
            empJobVM = _empJobRepo.SelectByEmployee(empLeaveVM.EmployeeId);
            //}


            string superVisorCode = "";
            string superVisorName = "";
            double totalDays = 0;
            if (!string.IsNullOrWhiteSpace(empJobVM.Supervisor))
            {
                if (empJobVM.Supervisor.Contains('~'))
                {
                    superVisorCode = empJobVM.Supervisor.Split('~')[0];
                    superVisorName = empJobVM.Supervisor.Split('~')[1];
                    EmployeeInfoVM superVisorVM = new EmployeeInfoVM();
                    EmployeeInfoRepo _eInfoRepo = new EmployeeInfoRepo();
                    superVisorVM.Email = _eInfoRepo.ViewSelectAllEmployee(superVisorCode).FirstOrDefault().EmpEmail;
                    getAllData.Add(superVisorVM); //Adding SuperVisor Info to Data
                }
            }

            EmailSettings ems = new EmailSettings();
            SettingRepo _setDAL = new SettingRepo();

            ems.MailFromAddress = _setDAL.settingValue("Mail", "MailFromAddress");
            ems.Password = _setDAL.settingValue("Mail", "MailFromPSW");
            ems.UserName = ems.MailFromAddress;

            ems.MailHeader = _setDAL.settingValue("Mail", "MailSubjectLeaveApply");
            ems.MailHeader = ems.MailHeader.Replace("vFromDate", empLeaveVM.FromDate);
            ems.MailHeader = ems.MailHeader.Replace("vToDate", empLeaveVM.ToDate);
            string mailbody = _setDAL.settingValue("Mail", "MailBodyLeaveApply");
            bool IsHolyDayLeaveSkip = _setDAL.settingValue("HRM", "IsHolyDayLeaveSkip") == "Y" ? true : false;

            if (IsHolyDayLeaveSkip)
            {

                List<DateTime> businessDays = Ordinary.GetBusinessDaysDate(Convert.ToDateTime(empLeaveVM.FromDate), Convert.ToDateTime(empLeaveVM.ToDate), empJobVM.FirstHoliday ?? "Friday", empJobVM.SecondHoliday ?? "Friday");
                List<DateTime> holidays = new HoliDayRepo().SelectAllHoliDate();

                List<DateTime> filteredDates = businessDays.Where(d => !holidays.Contains(d)).ToList();
                totalDays = filteredDates.Count;
            }
            else
            {
                totalDays = (Convert.ToDateTime(empLeaveVM.ToDate) - Convert.ToDateTime(empLeaveVM.FromDate)).TotalDays + 1;
            }



            //            string stMailBody = @"Dear Sir vSupervisor, \n I am in need Leave  From: vFromDate To: vToDate 
            //            \n Leave Type: vLeaveType \n Purpose: vPurpose \n Would You Please to Approve the Leave!  \n Sincerely Yours \n vEmpName" + vm.LeaveType_E;

            mailbody = mailbody.Replace("\\n", Environment.NewLine);
            mailbody = mailbody.Replace("vSupervisor", superVisorName);
            mailbody = mailbody.Replace("vFromDate", empLeaveVM.FromDate);
            mailbody = mailbody.Replace("vToDate", empLeaveVM.ToDate);
            mailbody = mailbody.Replace("vTotalDays", totalDays.ToString());
            mailbody = mailbody.Replace("vLeaveType", empLeaveVM.LeaveType_E);
            mailbody = mailbody.Replace("vPurpose", empLeaveVM.Remarks);
            mailbody = mailbody.Replace("vEmpName", empLeaveVM.EmpName);
            mailbody = mailbody.Replace("vCode", empLeaveVM.OtherId == null || empLeaveVM.OtherId == "" ? empLeaveVM.EmpCode : empLeaveVM.OtherId);


            foreach (var item in getAllData)
            {
                try
                {
                    ems.MailToAddress = item.Email;
                    //ems.MailToAddress = "shariful.islam@symphonysoftt.com";

                    if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
                    {

                        ems.MailBody = mailbody;
                        ems.FileName = empLeaveVM.EmpName + " (" + empLeaveVM.FromDate + "-" + empLeaveVM.ToDate + ")";
                        using (var smpt = new SmtpClient())
                        {
                            smpt.EnableSsl = ems.USsel;
                            smpt.Host = ems.ServerName;
                            smpt.Port = ems.Port;
                            smpt.UseDefaultCredentials = true;
                            smpt.EnableSsl = true;
                            smpt.Credentials = new NetworkCredential(ems.UserName, ems.Password);
                            MailMessage mailmessage = new MailMessage(
                                ems.MailFromAddress,
                                ems.MailToAddress,
                                ems.MailHeader,
                                ems.MailBody);
                            //mailmessage.Attachments.Add(new Attachment(rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat), ems.FileName + ".pdf"));

                            smpt.Send(mailmessage);
                            mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                            //FileLogger.Log("EmpLeaveApplyEmailProcess", this.GetType().Name, "EmpEmail Send To:" + ems.MailToAddress);
                        }
                        Thread.Sleep(500);
                    }
                }
                catch (SmtpFailedRecipientException ex)
                {

                    //FileLogger.Log("EmpLeaveApplyEmailProcess", this.GetType().Name, "EmpEmail Send To:" + ems.MailToAddress + " " + ex.Message + Environment.NewLine + ex.StackTrace);
                    // throw ex;
                }
            }
            thread.Abort();

        }

        public void EmpLeaveApproveEmailProcess(string Code = "", string Id = "", string EmployeeId = "")
        {
            #region Objects and Variables

            //Fetch Employee Leave Info for this Id
            EmployeeLeaveVM vm = new EmployeeLeaveVM();

            //Fetch Admin User Info
            UserInformationRepo _repo = new UserInformationRepo();
            List<EmployeeInfoVM> getAllData = new List<EmployeeInfoVM>();

            //Fetch SuperVisor Info
            EmployeeInfoVM empInfoVM = new EmployeeInfoVM();
            EmployeeInfoRepo _eInfoRepo = new EmployeeInfoRepo();
            ViewEmployeeInfoVM viewEmpInfoVM = new ViewEmployeeInfoVM();
            EmailSettings ems = new EmailSettings();
            SettingRepo _SettingRepo = new SettingRepo();
            #endregion

            //mailbody = mailbody.Replace("vEmpName", vm.EmpName);

            try
            {
                #region Get Data
                double totalDays = 0;

                vm = elrepo.SelectById(Convert.ToInt32(Id));
                viewEmpInfoVM = _eInfoRepo.ViewSelectAllEmployee(Code).FirstOrDefault();
                empInfoVM.Email = viewEmpInfoVM.EmpEmail;
                empInfoVM.EmpName = viewEmpInfoVM.EmpName;

                getAllData.Add(empInfoVM); //Adding Employee Info to Data

                #endregion

                #region Mail Data Assign

                ems.MailFromAddress = _SettingRepo.settingValue("Mail", "MailFromAddress");
                ems.Password = _SettingRepo.settingValue("Mail", "MailFromPSW");
                ems.UserName = ems.MailFromAddress;

                ems.MailHeader = _SettingRepo.settingValue("Mail", "MailSubjectLeaveApprove");
                ems.MailHeader = ems.MailHeader.Replace("vFromDate", vm.FromDate);
                ems.MailHeader = ems.MailHeader.Replace("vToDate", vm.ToDate);
                string mailbody = _SettingRepo.settingValue("Mail", "MailBodyLeaveApprove");

                bool IsHolyDayLeaveSkip = _SettingRepo.settingValue("HRM", "IsHolyDayLeaveSkip") == "Y" ? true : false;

                if (IsHolyDayLeaveSkip)
                {
                    var employeeJob = new EmployeeJobRepo().SelectByEmployee(viewEmpInfoVM.Id);

                    List<DateTime> businessDays = Ordinary.GetBusinessDaysDate(Convert.ToDateTime(vm.FromDate), Convert.ToDateTime(vm.ToDate), employeeJob.FirstHoliday ?? "Friday", employeeJob.SecondHoliday ?? "Friday");
                    List<DateTime> holidays = new HoliDayRepo().SelectAllHoliDate();

                    List<DateTime> filteredDates = businessDays.Where(d => !holidays.Contains(d)).ToList();
                    totalDays = filteredDates.Count;
                }
                else
                {
                    totalDays = (Convert.ToDateTime(vm.ToDate) - Convert.ToDateTime(vm.FromDate)).TotalDays + 1;
                }

                //            string stMailBody = @"Dear Sir vSupervisor, \n I am in need Leave  From: vFromDate To: vToDate 
                //            \n Leave Type: vLeaveType \n Purpose: vPurpose \n Would You Please to Approve the Leave!  \n Sincerely Yours \n vEmpName" + vm.LeaveType_E;

                mailbody = mailbody.Replace("\\n", Environment.NewLine);
                mailbody = mailbody.Replace("vEmpName", empInfoVM.EmpName);
                mailbody = mailbody.Replace("vApproved", vm.IsReject == true ? "Reject" : "Approved");
                mailbody = mailbody.Replace("vFromDate", vm.FromDate);
                mailbody = mailbody.Replace("vToDate", vm.ToDate);
                mailbody = mailbody.Replace("vTotalDays", totalDays.ToString());
                mailbody = mailbody.Replace("vLeaveType", vm.LeaveType_E);
                mailbody = mailbody.Replace("vPurpose", vm.Remarks);
                mailbody = mailbody.Replace("vStatus", vm.Approval);

                #endregion

                ems.MailToAddress = empInfoVM.Email;
                //ems.MailToAddress = "shariful.islam@symphonysoftt.com";


                #region Email Sending

                if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
                {

                    ems.MailBody = mailbody;
                    ems.FileName = vm.EmpName + " (" + vm.FromDate + "-" + vm.ToDate + ")";
                    if (ems.MailFromAddress.Contains("@"))
                    {
                        using (var smpt = new SmtpClient())
                        {
                            smpt.EnableSsl = ems.USsel;
                            smpt.Host = ems.ServerName;
                            smpt.Port = ems.Port;
                            smpt.UseDefaultCredentials = false;
                            smpt.EnableSsl = true;
                            smpt.Credentials = new NetworkCredential(ems.UserName, ems.Password);

                            MailMessage mailmessage = new MailMessage(
    ems.MailFromAddress,
    ems.MailToAddress,
    ems.MailHeader,
    ems.MailBody);
                            //mailmessage.Attachments.Add(new Attachment(rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat), ems.FileName + ".pdf"));

                            smpt.Send(mailmessage);
                            mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                            //FileLogger.Log("EmpLeaveApproveEmailProcess", this.GetType().Name, "EmpEmail Send To:" + ems.MailToAddress);

                        }


                        Thread.Sleep(500);
                    }
                }
                #endregion
            }
            catch (SmtpFailedRecipientException ex)
            {
                //FileLogger.Log("EmpLeaveApproveEmailProcess", this.GetType().Name, "EmpEmail Not Send To:" + ems.MailToAddress + " " + ex.Message + Environment.NewLine + ex.StackTrace);


                // throw ex;
            }
            thread.Abort();

        }

        public class EmailSettings
        {
            public string MailToAddress { get; set; }
            public string MailFromAddress = "";
            public bool USsel = true;
            public string Password = "";
            public string UserName = "";
            public string ServerName = "smtp.gmail.com";
            //public string ServerName = "smtp-mail.outlook.com";
            //public string ServerName = "smtp.mail.yahoo.com";
            public string MailBody { get; set; }
            public string MailHeader { get; set; }
            public string Fiscalyear { get; set; }
            public int Port = 587;
            public HttpPostedFileBase fileUploader { get; set; }
            public string FileName { get; set; }
        }

        #endregion

        [Authorize]
        public ActionResult Delete(string id = "0")
        {
            EmployeeLeaveVM vm = new EmployeeLeaveVM();
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.Id = Convert.ToInt32(id);
            result = elrepo.Delete(vm);
            //Session["result"] = result[0] + "~" + result[1];

            string msg = result[0] + "~" + result[1];
            return Json(msg, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index");

        }

        [Authorize]
        public JsonResult DeleteDirect(string id = "0")
        {
            EmployeeLeaveVM vm = new EmployeeLeaveVM();
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.Id = Convert.ToInt32(id);
            result = elrepo.Delete(vm);
            string msg = result[0] + "~" + result[1];
            return Json(msg, JsonRequestBehavior.AllowGet);

        }
    }
}
