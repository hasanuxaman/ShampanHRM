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
    public class TaxScheduleYearlyController : Controller
    {
        //
        // GET: /TAX/TaxScheduleYearly/
        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;


        public ActionResult Index(string EmployeeId = "", string fYear = "")
        {
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.fYear = fYear;
            return View();
        }

        public ActionResult _index(JQueryDataTableParamModel param, string EmployeeId = "", string fYear = "")
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
            //Project
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
            IEnumerable<EmployeeInfoVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;


            Schedule1SalaryYearlyRepo _repo = new Schedule1SalaryYearlyRepo();
            string[] conditionFields = { "ssy.EmployeeId", "ssy.Year" };
            string[] conditionValues = { EmployeeId, fYear };
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
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
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

        public ActionResult IndexFiscalYear(string EmployeeId = "", string fYear = "")
        {
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.fYear = fYear;

            return View();
        }

        public ActionResult _indexFiscalYear(JQueryDataTableParamModel param, string EmployeeId = "", string fYear = "")
        {
            Schedule1SalaryYearlyRepo _repo = new Schedule1SalaryYearlyRepo();
            List<Schedule1SalaryVM> getAllData = new List<Schedule1SalaryVM>();
            IEnumerable<Schedule1SalaryVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] conditionFields = { "ssy.EmployeeId", "ssy.Year" };
            string[] conditionValues = { EmployeeId, fYear };
            getAllData = _repo.SelectFiscalPeriod(conditionFields, conditionValues);


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.FiscalPeriod.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Schedule1SalaryVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Year.ToString() :
                sortColumnIndex == 2 && isSortable_2 ? c.Remarks :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                 c.Year.ToString()
                , c.Year.ToString()
                , c.Remarks 
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

            Schedule1SalaryVM ssVM = new Schedule1SalaryVM();
            vm.schedule1SalaryVM = ssVM;

            vm.Operation = "add";


            return View(vm);
        }

        //Create
        [HttpPost]
        public ActionResult SASalaryYearlyCreateEdit(TaxScheduleVM taxVM, int fYear)
        {
            Schedule1SalaryVM vm = new Schedule1SalaryVM();
            Schedule1SalaryYearlyRepo _repo = new Schedule1SalaryYearlyRepo();
            vm = taxVM.schedule1SalaryVM;
            vm.Year = Convert.ToInt32(fYear);
            FiscalYearRepo fiscalyear = new FiscalYearRepo();
            var fyear = fiscalyear.SelectByYear(fYear);
            vm.FiscalYearId = fyear.Id;
            
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        //LoadSchedule1SalaryYearly
        public ActionResult Edit(string EmployeeId = "", string EmployeeCode = "", string fYear = "")
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

            #endregion Assing EmployeeInfo into TaxScheduleVM


            Schedule1SalaryVM schedule1SalaryYearlyVM = new Schedule1SalaryVM();
            Schedule1SalaryYearlyRepo _schedule1SalaryYearlyRepo = new Schedule1SalaryYearlyRepo();

            string[] conditionFields = { "ssy.EmployeeId", "ssy.Year" };
            string[] conditionValues = { EmployeeId, fYear };
            schedule1SalaryYearlyVM = _schedule1SalaryYearlyRepo.SelectAll(0, conditionFields, conditionValues).FirstOrDefault();
            //AND EmployeeId = '1_30' AND FiscalYearDetailId = '1025'

            vm.schedule1SalaryVM = schedule1SalaryYearlyVM;
            if (schedule1SalaryYearlyVM != null)
            {
                #region Fetch Data From Employee Tax Slab Details
                EmployeeTaxSlabDetailYearlyRepo _employeeTaxSlabDetailRepo = new EmployeeTaxSlabDetailYearlyRepo();
                List<EmployeeTaxSlabDetailVM> employeeTaxSlabDetailVMs = new List<EmployeeTaxSlabDetailVM>();
                employeeTaxSlabDetailVMs = _employeeTaxSlabDetailRepo.SelectAll(0, schedule1SalaryYearlyVM.Id);
                schedule1SalaryYearlyVM.employeeTaxSlabDetailVMs = employeeTaxSlabDetailVMs;
                vm.EmployeeTaxSlabCount = schedule1SalaryYearlyVM.employeeTaxSlabDetailVMs.Count;
                #endregion Fetch Data From Employee Tax Slab Details
                vm.FiscalYearDetailId = schedule1SalaryYearlyVM.FiscalYearDetailId;
                vm.Year = schedule1SalaryYearlyVM.Year;
            }

            vm.Operation = "update";
            return View("Create", vm);
            //return PartialView("_schedule1SalaryYearly", vm);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        //ProcessSchedule1SalaryYearly
        public ActionResult ProcessSASalaryYearly(string EmployeeId = "", string EmployeeCode = "", string fYear = ""
            , string LineA = ""
            )
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "add").ToString();

            TaxScheduleVM vm = new TaxScheduleVM();

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


            Schedule1SalaryVM schedule1SalaryYearlyVM = new Schedule1SalaryVM();
            Schedule1SalaryYearlyRepo _repo = new Schedule1SalaryYearlyRepo();
            bool isMonth = false;
            int taxSlabId = 1;
            schedule1SalaryYearlyVM = _repo.ProcessSASalary(LineA, taxSlabId, false);
            vm.schedule1SalaryVM = schedule1SalaryYearlyVM;
            vm.EmployeeTaxSlabCount = schedule1SalaryYearlyVM.employeeTaxSlabDetailVMs.Count;

            #region Assing EmployeeInfo into Schedule1SalaryYearlyVM

            vm.schedule1SalaryVM.EmployeeId = empInfoVM.EmployeeId;
            vm.schedule1SalaryVM.ProjectId = empInfoVM.ProjectId;
            vm.schedule1SalaryVM.DepartmentId = empInfoVM.DepartmentId;
            vm.schedule1SalaryVM.SectionId = empInfoVM.SectionId;
            vm.schedule1SalaryVM.DesignationId = empInfoVM.DesignationId;

            #endregion Assing EmployeeInfo into Schedule1SalaryYearlyVM

            #region ExistCheck
            Schedule1SalaryVM sasmVM = new Schedule1SalaryVM();
            string[] conditionFields = { "ssy.EmployeeId", "ssy.Year" };
            string[] conditionValues = { empInfoVM.EmployeeId, fYear };
            sasmVM = _repo.SelectAll(0, conditionFields, conditionValues).FirstOrDefault();

            if (sasmVM != null && sasmVM.Id > 0)
            {
                vm.Operation = "update";
                vm.schedule1SalaryVM.Id = sasmVM.Id;
                vm.schedule1SalaryVM.FiscalYearId = sasmVM.FiscalYearId;
            }
            else
            {
                vm.Operation = "add";
            }

            #endregion ExistCheck

            return PartialView("_schedule1SalaryYearly", vm);
        }





        public JsonResult SelectEmployeeDetails(string EmployeeId = "", string EmployeeCode = "")
        {
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
            string EmployeeName = empInfoVM.EmpName;
            string Designation = empInfoVM.Designation;
            string Department = empInfoVM.Department;
            string Section = empInfoVM.Section;
            string Project = empInfoVM.Project;


            string EmployeeDetails = EmployeeName + "~" + Designation + "~" + Department + "~" + Section + "~" + Project;
            #endregion Assing EmployeeInfo into TaxScheduleVM
            return Json(EmployeeDetails, JsonRequestBehavior.AllowGet);
        }

    }
}
