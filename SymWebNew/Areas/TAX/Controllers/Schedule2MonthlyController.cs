using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymRepository.Tax;
using SymViewModel.HRM;
using SymViewModel.Tax;
using SymRepository.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using SymViewModel.Common;

namespace SymWebUI.Areas.TAX.Controllers
{
    public class Schedule2MonthlyController : Controller
    {
        //
        // GET: /TAX/Schedule2Monthly/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;


        public ActionResult Index(string EmployeeId = "", string fydid = "")
        {
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.fydid = fydid;
            return View();
        }

        public ActionResult _index(JQueryDataTableParamModel param, string EmployeeId = "", string fydid = "")
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var designationFilter = Convert.ToString(Request["sSearch_3"]);
            var departmentFilter = Convert.ToString(Request["sSearch_4"]);
            var sectionFilter = Convert.ToString(Request["sSearch_5"]);
            var projecttFilter = Convert.ToString(Request["sSearch_6"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_7"]);

            //Code
            //EmpName 
            //Designation
            //Department 
            //Section
            //Projectt
            //JoinDate

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
            }


            #endregion Column Search

            List<Schedule2HousePropertyVM> getAllData = new List<Schedule2HousePropertyVM>();
            IEnumerable<Schedule2HousePropertyVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            Schedule2HousePropertyMonthlyRepo _repo = new Schedule2HousePropertyMonthlyRepo();
            string[] conditionFields = { "shm.EmployeeId", "shm.FiscalYearDetailId" };
            string[] conditionValues = { EmployeeId, fydid };
            getAllData = _repo.SelectEmployeeList(conditionFields, conditionValues);


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.Section.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.Project.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable7 && c.JoinDate.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            if (codeFilter != "" || empNameFilter != "" || designationFilter != "" || departmentFilter != "" || sectionFilter != "" || projecttFilter != "" || (joinDateFilter != "" && joinDateFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    && (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    && (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    && (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    && (sectionFilter == "" || c.Section.ToLower().Contains(sectionFilter.ToLower()))
                                    && (projecttFilter == "" || c.Project.ToLower().Contains(projecttFilter.ToLower()))
                                    && (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
                                    && (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.JoinDate))
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
            Func<Schedule2HousePropertyVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
               sortColumnIndex == 5 && isSortable_5 ? c.Section :
                sortColumnIndex == 6 && isSortable_6 ? c.Project :
                sortColumnIndex == 7 && isSortable_7 ? Ordinary.DateToString(c.JoinDate) :
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
                , c.Section    
                , c.Project    
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

        public ActionResult IndexFiscalPeriod(string EmployeeId = "", string fydid = "")
        {
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.fydid = fydid;

            return View();
        }
        public ActionResult _indexFiscalPeriod(JQueryDataTableParamModel param, string EmployeeId = "", string fydid = "")
        {

            Schedule2HousePropertyMonthlyRepo _repo = new Schedule2HousePropertyMonthlyRepo();
            List<Schedule2HousePropertyVM> getAllData = new List<Schedule2HousePropertyVM>();
            IEnumerable<Schedule2HousePropertyVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] conditionFields = { "shm.EmployeeId", "shm.FiscalYearDetailId" };
            string[] conditionValues = { EmployeeId, fydid };
            getAllData = _repo.SelectFiscalPeriod(conditionFields, conditionValues);


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.PeriodName.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Schedule2HousePropertyVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.PeriodName :
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
                , c.PeriodName
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
        public ActionResult Create(string EmployeeId = "", string EmployeeCode = "")
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "add").ToString();
            TaxScheduleVM vm = new TaxScheduleVM();

            #region Assing EmployeeInfo into TaxScheduleVM
            ViewEmployeeInfoVM empInfoVM = new ViewEmployeeInfoVM();
            EmployeeInfoRepo _empInfoRepo = new EmployeeInfoRepo();
            if (!string.IsNullOrWhiteSpace(EmployeeCode))
            {
                empInfoVM = _empInfoRepo.ViewSelectAllEmployee(EmployeeCode).FirstOrDefault();
            }
            else if (!string.IsNullOrWhiteSpace(EmployeeId))
            {
                empInfoVM = _empInfoRepo.ViewSelectAllEmployee("", EmployeeId).FirstOrDefault();
            }

            if (empInfoVM != null)
            {

                vm.EmployeeId = empInfoVM.EmployeeId;
                vm.ProjectId = empInfoVM.ProjectId;
                vm.DepartmentId = empInfoVM.DepartmentId;
                vm.SectionId = empInfoVM.SectionId;
                vm.DesignationId = empInfoVM.DesignationId;

                vm.EmployeeCode = empInfoVM.Code;
                vm.EmployeeName = empInfoVM.EmpName;
                vm.Project = empInfoVM.Project;
                vm.Department = empInfoVM.Department;
                vm.Section = empInfoVM.Section;
                vm.Designation = empInfoVM.Designation;
            }

            #endregion Assing EmployeeInfo into TaxScheduleVM

            Schedule2HousePropertyVM schedule2HousePropertyVM = new Schedule2HousePropertyVM();
            vm.schedule2HousePropertyVM = schedule2HousePropertyVM;

            vm.Operation = "add";


            return View(vm);
        }



        [HttpPost]
        public ActionResult CreateEdit(TaxScheduleVM taxVM)
        {
            Schedule2HousePropertyVM vm = new Schedule2HousePropertyVM();
            Schedule2HousePropertyMonthlyRepo _repo = new Schedule2HousePropertyMonthlyRepo();


            vm = taxVM.schedule2HousePropertyVM;
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                if (taxVM.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    result = _repo.Insert(vm);


                    var mgs = result[0] + "~" + result[1];
                    Session["mgs"] = "mgs";
                    return Json(mgs, JsonRequestBehavior.AllowGet);
                }
                else if (taxVM.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    var mgs = result[0] + "~" + result[1];
                    Session["mgs"] = "mgs";
                    return Json(mgs, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var mgs = result[0] + "~" + result[1];
                    Session["mgs"] = "mgs";
                    return Json(mgs, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                var mgs = result[0] + "~" + result[1];
                Session["mgs"] = "mgs";
                return Json(mgs, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
