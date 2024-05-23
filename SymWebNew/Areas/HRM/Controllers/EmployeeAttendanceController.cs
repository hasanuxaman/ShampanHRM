using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Attendance;
using SymRepository.Common;
using SymRepository.HRM;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    [Authorize]
    public class EmployeeAttendanceController : Controller
    {
        #region Attendance
        CommonRepo crepo = new CommonRepo();
        EmployeeInfoRepo _repo = new EmployeeInfoRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        public ActionResult EmployeeInfoForAttendance(string Id)
        {

            EmployeeVM vm = new EmployeeVM();
            vm = _repo.EmployeeInfo(Id);
            if (!string.IsNullOrWhiteSpace(vm.Id))
            {
                return PartialView("_employee", vm);
            }
            else
            {
                Session["result"] = "Fail~There have no employee";
                return View("AttendanceLogList");
            }
        }
        [HttpGet]
        public ActionResult AttendanceLog()
        {
            if (Session["mgs"].ToString() != "")
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            if (identity.IsESS)
            {
                //RedirectToAction("_attendance",new {"EmployeeId"="1"});
            }
            return View();
        }

        [HttpGet]
        public ActionResult _attendance(string EmployeeId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            EmployeeInfoVM empvm = new EmployeeInfoVM();
            AttLogsVM vm = new AttLogsVM();
            EmployeeInfoRepo erepo = new EmployeeInfoRepo();
            empvm = erepo.SelectById(EmployeeId);
            AttendanceStructureRepo _tpRepo = new AttendanceStructureRepo();

            DateTime result = crepo.ServerDateTime();
            vm.PunchDate = Convert.ToDateTime(result).ToString("dd-MMM-yyyy");
            vm.PunchTime = Convert.ToDateTime(result).ToString("hh:mm tt");
            empvm.AttLogsVM = vm;
            if (identity.IsAdmin || identity.IsHRM)
            {
                vm.Self = false;
            }
            else
            {
                vm.Self = true;
            }
            vm.EmployeeId = EmployeeId;
            //ManualRoster
            string AttendanceSystem = new AppSettingsReader().GetValue("AttendanceSystem", typeof(string)).ToString();
            empvm.AttendanceSystem = AttendanceSystem;
            return PartialView("_attendance", empvm);
        }
        [HttpPost]
        public ActionResult AttendanceLog(EmployeeInfoVM emvm)
        {
            AttLogsVM vm = new AttLogsVM();
            vm = emvm.AttLogsVM;
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
            return RedirectToAction("AttendanceLog", new { mgs = mgs });
        }
        [HttpGet]
        public ActionResult AttendanceLogEdit(string id)
        {
            AttLogsRepo _attLogRepo = new AttLogsRepo();
            AttendanceStructureRepo _tpRepo = new AttendanceStructureRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            AttLogsVM attl = new AttLogsVM();
            attl = _attLogRepo.SelectById(id);
            vm = _repo.SelectById(attl.EmployeeId);
            vm.AttLogsVM = attl;
            vm.attvm = _tpRepo.SelectByEmployee(attl.EmployeeId);
            if (identity.IsAdmin || identity.IsHRM)
            {
                vm.AttLogsVM.Self = false;
            }
            else
            {
                vm.AttLogsVM.Self = true;
            }
            return View(vm);
        }
        public ActionResult AttendanceLogEditDetail(string empcode = "", string btn = "current")
        {

            EmployeeInfoVM vm = new EmployeeInfoVM();
            AttLogsVM attl = new AttLogsVM();
            AttendanceStructureRepo _tpRepo = new AttendanceStructureRepo();
            AttLogsRepo _attLogRepo = new AttLogsRepo();
            vm = _repo.SelectEmpForSearch(empcode, btn);

            return PartialView("AttendenceListDetail", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult AttendanceLogEdit(EmployeeInfoVM evm)
        {
            AttLogsVM vm = new AttLogsVM();
            vm = evm.AttLogsVM;
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] retResults = new string[6];
            AttLogsRepo repo = new AttLogsRepo();
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            retResults = repo.Update(vm);
            var mgs = retResults[0] + "~" + retResults[1];
            Session["mgs"] = "mgs";
            return RedirectToAction("AttendanceLogList", new { mgs = mgs });
        }
        public ActionResult AttendanceLogList()
        {
            if (Session["mgs"].ToString() != "")
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            return View("AttendanceLogList");
        }
        public ActionResult _indexAttendanceLogList(JQueryDataTableParamModel param, string code, string name)
        {
            var PunchDateFilter = Convert.ToString(Request["sSearch_1"]);
            var PunchTimeFilter = Convert.ToString(Request["sSearch_2"]);
            var CodeFilter = Convert.ToString(Request["sSearch_3"]);
            var NameFilter = Convert.ToString(Request["sSearch_4"]);
            var DepartmentFilter = Convert.ToString(Request["sSearch_5"]);
            var DesignationFilter = Convert.ToString(Request["sSearch_6"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_7"]);
            
            int pageNo = param.iDisplayStart;
            int pageSize = param.iDisplayLength;

            DateTime PunchDatefromDate = DateTime.MinValue;
            DateTime PunchDatetoDate = DateTime.MaxValue;
            DateTime joinfromDate = DateTime.MinValue;
            DateTime jointoDate = DateTime.MaxValue;
            if (PunchDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                PunchDatefromDate = PunchDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Convert.ToDateTime(PunchDateFilter.Split('~')[0]);
                PunchDatetoDate = PunchDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Convert.ToDateTime(PunchDateFilter.Split('~')[1]);
            }
            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                joinfromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Convert.ToDateTime(joinDateFilter.Split('~')[0]);
                jointoDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Convert.ToDateTime(joinDateFilter.Split('~')[1]);
            }
            AttLogsRepo _attLogRepo = new AttLogsRepo();
            List<AttLogsVM> getAllData = new List<AttLogsVM>();
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            if (identity.IsAdmin || identity.IsHRM)
            {

                getAllData = _attLogRepo.SelectAllData(pageNo, pageSize);
            }
            else
            {
                getAllData = _attLogRepo.SelectAll(Identit.EmployeeId);
            }
            IEnumerable<AttLogsVM> filteredData;
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
                   .Where(c => isSearchable1 && c.PunchDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.PunchTime.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.JoinDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }


            if ((PunchDateFilter != "" && PunchDateFilter != "~") || NameFilter != "" || CodeFilter != "" || PunchTimeFilter != ""
       || DepartmentFilter != "" || DesignationFilter != "" || (joinDateFilter != "" && joinDateFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c => (CodeFilter == "" || c.Code.ToLower().Contains(CodeFilter.ToLower()))
                                    &&
                                    (NameFilter == "" || c.EmpName.ToLower().Contains(NameFilter.ToLower()))
                                    &&
                                    (PunchTimeFilter == "" || c.PunchTime.ToLower().Contains(PunchTimeFilter.ToLower()))
                                    &&
                                    (DepartmentFilter == "" || c.Department.ToLower().Contains(DepartmentFilter.ToLower()))
                                    &&
                                    (DesignationFilter == "" || c.Designation.ToLower().Contains(DesignationFilter.ToLower()))
                                    &&
                                    (PunchDatefromDate == DateTime.MinValue || PunchDatefromDate < Convert.ToDateTime(c.PunchDate))
                                    &&
                                    (PunchDatetoDate == DateTime.MaxValue || Convert.ToDateTime(c.PunchDate) < PunchDatetoDate)
                                    &&
                                    (joinfromDate == DateTime.MinValue || joinfromDate < Convert.ToDateTime(c.JoinDate))
                                    &&
                                    (jointoDate == DateTime.MaxValue || Convert.ToDateTime(c.JoinDate) < jointoDate)
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

            Func<AttLogsVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? Ordinary.DateToString(c.PunchDate) :
                                                           sortColumnIndex == 2 && isSortable_2 ? Ordinary.TimeToString(c.PunchTime) :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Code :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.EmpName :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Department :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Designation :
                                                           sortColumnIndex == 7 && isSortable_7 ? Ordinary.DateToString(c.JoinDate) :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] 
                         {   
                             Convert.ToString(c.SLNo),
                             c.PunchDate 
                             , c.PunchTime 
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
        public ActionResult AttendancePolicy(string EmployeeId, string PunchDate)
        {
            AttendanceStructureRepo _tpRepo = new AttendanceStructureRepo();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            vm = repo.SelectById(EmployeeId);
            vm.attvm = _tpRepo.SelectByEmployee(EmployeeId, "");
            vm.attvm.EmployeeId = EmployeeId;
            if (vm.attvm.Id > 0)
            {
                return PartialView("_timePolicy", vm);
            }
            else
            {
                var mgs = "Fail~This Employee have no Time policy";
                Session["mgs"] = "mgs";
                ViewBag.mgs = "Fail~This Employee have no Time policy";
                return View(mgs);
            }
        }

        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult AttendanceLogDelete(string ids)
        {
            AttLogsVM vm = new AttLogsVM();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = new AttLogsRepo().Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        public ActionResult EmployeeInfo(string Id)
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeVM vm = new EmployeeVM();
            vm = repo.EmployeeInfo(Id);
            if (vm.IsPermanent)
            {
                return PartialView("_employee", vm);
            }
            else
            {
                var mgs = "Fail~This Employee not Permanent";
                Session["mgs"] = "mgs";
                return RedirectToAction("Leave", new { mgs = mgs });
            }
        }
        public ActionResult _index(JQueryDataTableParamVM param)
        {

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_5"]);

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
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (identity.IsAdmin || identity.IsHRM)
            {
                //getAllData = _empRepo.SelectAll();
                getAllData = _empRepo.SelectAllActiveEmp();
            }
            else
            {
                getAllData.Add(_empRepo.SelectById(Identit.EmployeeId));

            }
            IEnumerable<EmployeeInfoVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
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
                         select new[] 
            { 
                c.Id
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


        [HttpPost]
        public ActionResult AttendanceLogNew(EmployeeInfoVM emvm)
        {
            DownloadDataVM vm = new DownloadDataVM();
            vm.EmployeeId = emvm.AttLogsVM.EmployeeId;
            vm.Remarks = emvm.AttLogsVM.Remarks;
            //AttLogsVM vm = new AttLogsVM();

            //vm = emvm.AttLogsVM;
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] retResults = new string[6];
            AttLogsRepo erepo = new AttLogsRepo();
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.AttendanceSystem = emvm.AttendanceSystem;
            if (emvm.AttLogsVM.Self)
            {
                DateTime result = crepo.ServerDateTime();
                vm.PunchDate = Convert.ToDateTime(result).ToString("dd/MMM/yyyy");
                vm.PunchTime = Convert.ToDateTime(result).ToString("hh:mm tt");
            }
            else
            {
                vm.PunchDate = emvm.AttLogsVM.PunchDate;
                vm.PunchTime = emvm.AttLogsVM.PunchTime;
            }
            DownloadDataRepo _repo = new DownloadDataRepo();
            retResults = _repo.InsertManual(vm);
            var mgs = retResults[0] + "~" + retResults[1];

            if (retResults[0] == "Fail")
            {
                return Json(mgs, JsonRequestBehavior.AllowGet);
            }


            //AttendanceMigrationRepo _aMigrationRepo = new AttendanceMigrationRepo();
            //AttendanceMigrationVM aMigrationVM = new AttendanceMigrationVM();

            //aMigrationVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //aMigrationVM.CreatedBy = identity.Name;
            //aMigrationVM.CreatedFrom = identity.WorkStationIP;

            //aMigrationVM.AttendanceDateFrom = vm.PunchDate;
            //aMigrationVM.AttendanceDateTo = vm.PunchDate;
            //aMigrationVM.AttendanceSystem = emvm.AttendanceSystem;
            //retResults = _aMigrationRepo.SelectFromDownloadData(aMigrationVM);



            mgs = retResults[0] + "~" + retResults[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
