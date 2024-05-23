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
    public class TaxScheduleController : Controller
    {
        //
        // GET: /TAX/TaxSchedule/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;


        public ActionResult Index(string EmployeeId = "", string fydid = "", string tType = "")
        {
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.fydid = fydid;
            ViewBag.tType = tType;
            return View();
        }

        public ActionResult _1index(JQueryDataTableParamModel param, string EmployeeId = "", string fydid = "", string tType = "")
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

            if (string.IsNullOrWhiteSpace(EmployeeId) && string.IsNullOrWhiteSpace(fydid))
            {
                if (Session["UserType"].ToString() == "True")
                {
                    getAllData = _empRepo.SelectAllActiveEmp();
                }
                else
                {
                    getAllData.Add(_empRepo.SelectById(Identit.EmployeeId));

                }
            }
            else
            {
                Schedule1SalaryMonthlyRepo _repo = new Schedule1SalaryMonthlyRepo();
                string[] conditionFields = { "ssm.EmployeeId", "ssm.FiscalYearDetailId"};
                string[] conditionValues = { EmployeeId, fydid};
                getAllData = _repo.SelectEmployeeList(conditionFields, conditionValues, tType);
            }


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

        public ActionResult IndexFiscalPeriod(string EmployeeId = "", string fydid = "", string tType = "")
        {
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.fydid = fydid;
            ViewBag.tType = tType;

            //if (string.IsNullOrWhiteSpace(EmployeeId) && string.IsNullOrWhiteSpace(fydid))
            //{
            //    return View();
            //}


            return View();
        }
        public ActionResult _indexFiscalPeriod(JQueryDataTableParamModel param, string EmployeeId = "", string fydid = "", string tType = "")
        {

            Schedule1SalaryMonthlyRepo _repo = new Schedule1SalaryMonthlyRepo();
            List<Schedule1SalaryVM> getAllData = new List<Schedule1SalaryVM>();
            IEnumerable<Schedule1SalaryVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] conditionFields = { "ssm.EmployeeId", "ssm.FiscalYearDetailId"};
            string[] conditionValues = { EmployeeId, fydid };
            getAllData = _repo.SelectFiscalPeriod(conditionFields, conditionValues, tType);


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
                sortColumnIndex == 1 && isSortable_1 ? c.PeriodStart :
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
                 Convert.ToString(c.Id)
                , c.FiscalPeriod
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

        public ActionResult IndexFiscalYears(string EmployeeId = "", string fydid = "", string tType = "", string fYear = "")
        {
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.fydid = fydid;
            ViewBag.tType = tType;
            ViewBag.fYear = fYear;

            //if (string.IsNullOrWhiteSpace(EmployeeId) && string.IsNullOrWhiteSpace(fydid))
            //{
            //    return View();
            //}


            return View("IndexFiscalYears");
        }

        public ActionResult _indexFiscalYears(JQueryDataTableParamModel param, string EmployeeId = "", string fydid = "", string tType = "", string fYear = "")
        {

            Schedule1SalaryYearlyRepo _repo = new Schedule1SalaryYearlyRepo();
            List<Schedule1SalaryVM> getAllData = new List<Schedule1SalaryVM>();
            IEnumerable<Schedule1SalaryVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] conditionFields = { "ssy.EmployeeId", "ssy.Year" };
            string[] conditionValues = { EmployeeId, fYear };
            getAllData = _repo.SelectFiscalYearMonthlies(conditionFields, conditionValues);


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

            Schedule1SalaryVM schedule1SalaryMonthlyVM = new Schedule1SalaryVM();
            vm.schedule1SalaryVM = schedule1SalaryMonthlyVM;

            Schedule2HousePropertyVM schedule2HousePropertyMonthlyVM = new Schedule2HousePropertyVM();
            vm.schedule2HousePropertyVM = schedule2HousePropertyMonthlyVM;

            Schedule3InvestmentVM schedule3InvestmentMonthlyVM = new Schedule3InvestmentVM();
            vm.schedule3InvestmentVM = schedule3InvestmentMonthlyVM;

            vm.Operation = "add";


            return View(vm);
        }


        [HttpPost]
        public ActionResult CreateEdit(TaxScheduleVM taxVM, string fydid, string fyear)
        {
            Schedule1SalaryVM vm = new Schedule1SalaryVM();
            Schedule1SalaryMonthlyRepo _repo = new Schedule1SalaryMonthlyRepo();
            vm = taxVM.schedule1SalaryVM;
            //vm.EmployeeId = taxVM.EmployeeId;
            //vm.ProjectId = taxVM.ProjectId;
            //vm.DepartmentId = taxVM.DepartmentId;
            //vm.SectionId = taxVM.SectionId;
            //vm.DesignationId = taxVM.DesignationId;

            vm.FiscalYearDetailId = Convert.ToInt32(fydid);
            vm.FiscalYearDetailIdTo = Convert.ToInt32(fydid);

            vm.Year = Convert.ToInt32(fyear);


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
                    result = _repo.Update(vm, vm.TransactionType);
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


        [HttpPost]
        public ActionResult SBHousePropertyCreateEdit(TaxScheduleVM taxVM, string fydid, string fyear)
        {
            Schedule2HousePropertyVM vm = new Schedule2HousePropertyVM();
            Schedule2HousePropertyMonthlyRepo _repo = new Schedule2HousePropertyMonthlyRepo();
            vm = taxVM.schedule2HousePropertyVM;
            //vm.EmployeeId = taxVM.EmployeeId;
            //vm.ProjectId = taxVM.ProjectId;
            //vm.DepartmentId = taxVM.DepartmentId;
            //vm.SectionId = taxVM.SectionId;
            //vm.DesignationId = taxVM.DesignationId;

            vm.FiscalYearDetailId = Convert.ToInt32(fydid);
            vm.Year = Convert.ToInt32(fyear);

            if (taxVM.schedule2HousePropertyVM.Id > 0)
            {
                taxVM.Operation = "update";
            }
            else
            {
                taxVM.Operation = "add";
            }

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

        [HttpPost]
        public ActionResult SCInvestmentCreateEdit(TaxScheduleVM taxVM, string fydid, string fyear)
        {
            Schedule3InvestmentVM vm = new Schedule3InvestmentVM();
            Schedule3InvestmentMonthlyRepo _repo = new Schedule3InvestmentMonthlyRepo();
            vm = taxVM.schedule3InvestmentVM;
            //vm.EmployeeId = taxVM.EmployeeId;
            //vm.ProjectId = taxVM.ProjectId;
            //vm.DepartmentId = taxVM.DepartmentId;
            //vm.SectionId = taxVM.SectionId;
            //vm.DesignationId = taxVM.DesignationId;

            vm.FiscalYearDetailId = Convert.ToInt32(fydid);
            vm.Year = Convert.ToInt32(fyear);

            if (taxVM.schedule3InvestmentVM.Id > 0)
            {
                taxVM.Operation = "update";
            }
            else
            {
                taxVM.Operation = "add";
            }

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
        public ActionResult Edit(string EmployeeId = "", string EmployeeCode = "", string fydid = "", string tType = "")
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "add").ToString();
            TaxScheduleVM vm = new TaxScheduleVM();


            #region Assign EmployeeInfo into TaxScheduleVM
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

            vm.FinalTaxAmount = 0;
            #endregion Assign EmployeeInfo into TaxScheduleVM


            Schedule1SalaryVM schedule1SalaryVM = new Schedule1SalaryVM();
            Schedule1SalaryMonthlyRepo _schedule1SalaryMonthlyRepo = new Schedule1SalaryMonthlyRepo();

            string[] conditionFields = { "ssm.EmployeeId", "ssm.FiscalYearDetailId" };
            string[] conditionValues = { EmployeeId, fydid };
            schedule1SalaryVM = _schedule1SalaryMonthlyRepo.SelectAll(0, conditionFields, conditionValues, tType).FirstOrDefault();
            //AND EmployeeId = '1_30' AND FiscalYearDetailId = '1025'
            vm.FinalTaxAmount = schedule1SalaryVM.FinalTaxAmount;
            vm.FinalTaxAmountMonthly = schedule1SalaryVM.FinalTaxAmountMonthly;
            vm.FinalBonusTaxAmount = schedule1SalaryVM.FinalBonusTaxAmount;

            vm.schedule1SalaryVM = schedule1SalaryVM;
            if (schedule1SalaryVM != null)
            {
                #region Fetch Data From Employee Tax Slab Details
                EmployeeTaxSlabDetailMonthlyRepo _employeeTaxSlabDetailRepo = new EmployeeTaxSlabDetailMonthlyRepo();
                List<EmployeeTaxSlabDetailVM> employeeTaxSlabDetailVMs = new List<EmployeeTaxSlabDetailVM>();
                employeeTaxSlabDetailVMs = _employeeTaxSlabDetailRepo.SelectAll(0, schedule1SalaryVM.Id);
                schedule1SalaryVM.employeeTaxSlabDetailVMs = employeeTaxSlabDetailVMs;
                vm.EmployeeTaxSlabCount = schedule1SalaryVM.employeeTaxSlabDetailVMs.Count;
                #endregion Fetch Data From Employee Tax Slab Details
                vm.FiscalYearDetailId = schedule1SalaryVM.FiscalYearDetailId;
                vm.Year = schedule1SalaryVM.Year;

               // schedule1SalaryVM.CalculateTotalRebate();
            }




            //As Schedule1Salary Already Saved
            #region Fetching Schedule2HousePropertyVM
            string[] conditionFieldsHP = { "shm.EmployeeId", "shm.FiscalYearDetailId" };
            string[] conditionValuesHP = { EmployeeId, fydid };


            Schedule2HousePropertyVM schedule2HousePropertyVM = new Schedule2HousePropertyVM();
            vm.schedule2HousePropertyVM = schedule2HousePropertyVM;
            Schedule2HousePropertyMonthlyRepo _schedule2HousePropertyMonthlyRepo = new Schedule2HousePropertyMonthlyRepo();
            schedule2HousePropertyVM = _schedule2HousePropertyMonthlyRepo.SelectAll(0, conditionFieldsHP, conditionValuesHP).FirstOrDefault();
            if (schedule2HousePropertyVM != null)
            {
                vm.schedule2HousePropertyVM = schedule2HousePropertyVM;

                #region Fetch Data From Employee Schedule2 Tax Slab Details
                EmployeeSchedule2TaxSlabDetailMonthlyRepo _employeeSchedule2TaxSlabDetailRepo = new EmployeeSchedule2TaxSlabDetailMonthlyRepo();
                List<EmployeeSchedule2TaxSlabDetailVM> employeeSchedule2TaxSlabDetailVMs = new List<EmployeeSchedule2TaxSlabDetailVM>();
                employeeSchedule2TaxSlabDetailVMs = _employeeSchedule2TaxSlabDetailRepo.SelectAll(0, schedule2HousePropertyVM.Id);
                schedule2HousePropertyVM.employeeSchedule2TaxSlabDetailVMs = employeeSchedule2TaxSlabDetailVMs;
                vm.EmployeeSchedule2TaxSlabCount = schedule2HousePropertyVM.employeeSchedule2TaxSlabDetailVMs.Count;
                #endregion Fetch Data From Employee Schedule2 Tax Slab Details
                vm.FiscalYearDetailId = schedule2HousePropertyVM.FiscalYearDetailId;
                vm.Year = schedule2HousePropertyVM.Year;

            }

            #endregion Fetching Schedule2HousePropertyVM

            #region Assign EmployeeInfo into Schedule2HousePropertyVM
            if (string.IsNullOrWhiteSpace(vm.schedule2HousePropertyVM.EmployeeId))
            {
                vm.schedule2HousePropertyVM.EmployeeId = empInfoVM.EmployeeId;
                vm.schedule2HousePropertyVM.ProjectId = empInfoVM.ProjectId;
                vm.schedule2HousePropertyVM.DepartmentId = empInfoVM.DepartmentId;
                vm.schedule2HousePropertyVM.SectionId = empInfoVM.SectionId;
                vm.schedule2HousePropertyVM.DesignationId = empInfoVM.DesignationId;
            }
            #endregion Assign EmployeeInfo into Schedule2HousePropertyVM



            #region Fetching Schedule3InvestmentVM
            string[] conditionFieldsInv = { "sim.EmployeeId", "sim.FiscalYearDetailId" };
            string[] conditionValuesInv = { EmployeeId, fydid };


            Schedule3InvestmentVM schedule3InvestmentVM = new Schedule3InvestmentVM();
            vm.schedule3InvestmentVM = schedule3InvestmentVM;
            Schedule3InvestmentMonthlyRepo _schedule3InvestmentMonthlyRepo = new Schedule3InvestmentMonthlyRepo();
            schedule3InvestmentVM = _schedule3InvestmentMonthlyRepo.SelectAll(0, conditionFieldsInv, conditionValuesInv, tType).FirstOrDefault();
            if (schedule3InvestmentVM != null)
            {
                vm.schedule3InvestmentVM = schedule3InvestmentVM;

                #region Fetch Data From Employee Schedule3 Tax Slab Details
                EmployeeSchedule3TaxSlabDetailMonthlyRepo _employeeSchedule3TaxSlabDetailRepo = new EmployeeSchedule3TaxSlabDetailMonthlyRepo();
                List<EmployeeSchedule3TaxSlabDetailVM> employeeSchedule3TaxSlabDetailVMs = new List<EmployeeSchedule3TaxSlabDetailVM>();
                employeeSchedule3TaxSlabDetailVMs = _employeeSchedule3TaxSlabDetailRepo.SelectAll(0, schedule3InvestmentVM.Id);
                schedule3InvestmentVM.employeeSchedule3TaxSlabDetailVMs = employeeSchedule3TaxSlabDetailVMs;
                vm.EmployeeSchedule3TaxSlabCount = schedule3InvestmentVM.employeeSchedule3TaxSlabDetailVMs.Count;
                #endregion Fetch Data From Employee Schedule3 Tax Slab Details
                vm.FiscalYearDetailId = schedule3InvestmentVM.FiscalYearDetailId;
                vm.Year = schedule3InvestmentVM.Year;
            }

            #endregion Fetching Schedule3InvestmentVM

            #region Assign EmployeeInfo into Schedule3InvestmentVM
            if (string.IsNullOrWhiteSpace(vm.schedule3InvestmentVM.EmployeeId))
            {
                vm.schedule3InvestmentVM.EmployeeId = empInfoVM.EmployeeId;
                vm.schedule3InvestmentVM.ProjectId = empInfoVM.ProjectId;
                vm.schedule3InvestmentVM.DepartmentId = empInfoVM.DepartmentId;
                vm.schedule3InvestmentVM.SectionId = empInfoVM.SectionId;
                vm.schedule3InvestmentVM.DesignationId = empInfoVM.DesignationId;
            }
            #endregion Assign EmployeeInfo into Schedule3InvestmentVM

            vm.Operation = "update";
            vm.TransactionType = tType;
            return View("Create", vm);
            //return PartialView("_schedule1SalaryMonthly", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditYearly(string EmployeeId = "", string EmployeeCode = "", string fydid = "", string tType = "", string fYear = "")
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "add").ToString();
            TaxScheduleVM vm = new TaxScheduleVM();


            #region Assign EmployeeInfo into TaxScheduleVM
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

            vm.FinalTaxAmount = 0;
            #endregion Assign EmployeeInfo into TaxScheduleVM


            Schedule1SalaryVM schedule1SalaryVM = new Schedule1SalaryVM();
            Schedule1SalaryMonthlyRepo _schedule1SalaryMonthlyRepo = new Schedule1SalaryMonthlyRepo();

            string[] conditionFields = { "ssm.EmployeeId", "ssm.FiscalYearDetailId", "ssm.year" };
            string[] conditionValues = { EmployeeId, fydid, fYear };
            schedule1SalaryVM = _schedule1SalaryMonthlyRepo.SelectAll(0, conditionFields, conditionValues, tType).FirstOrDefault();
            //AND EmployeeId = '1_30' AND FiscalYearDetailId = '1025'
            vm.FinalTaxAmount = schedule1SalaryVM.FinalTaxAmount;
            vm.FinalTaxAmountMonthly = schedule1SalaryVM.FinalTaxAmountMonthly;
            vm.FinalBonusTaxAmount = schedule1SalaryVM.FinalBonusTaxAmount;

            vm.schedule1SalaryVM = schedule1SalaryVM;
            if (schedule1SalaryVM != null)
            {
                #region Fetch Data From Employee Tax Slab Details
                EmployeeTaxSlabDetailMonthlyRepo _employeeTaxSlabDetailRepo = new EmployeeTaxSlabDetailMonthlyRepo();
                List<EmployeeTaxSlabDetailVM> employeeTaxSlabDetailVMs = new List<EmployeeTaxSlabDetailVM>();
                employeeTaxSlabDetailVMs = _employeeTaxSlabDetailRepo.SelectAll(0, schedule1SalaryVM.Id);
                schedule1SalaryVM.employeeTaxSlabDetailVMs = employeeTaxSlabDetailVMs;
                vm.EmployeeTaxSlabCount = schedule1SalaryVM.employeeTaxSlabDetailVMs.Count;
                #endregion Fetch Data From Employee Tax Slab Details
                vm.FiscalYearDetailId = schedule1SalaryVM.FiscalYearDetailId;
                vm.Year = schedule1SalaryVM.Year;

               // schedule1SalaryVM.CalculateTotalRebate();
            }




            //As Schedule1Salary Already Saved
            #region Fetching Schedule2HousePropertyVM
            string[] conditionFieldsHP = { "shm.EmployeeId", "shm.FiscalYearDetailId", "shm.Year" };
            string[] conditionValuesHP = { EmployeeId, fydid,fYear };


            Schedule2HousePropertyVM schedule2HousePropertyVM = new Schedule2HousePropertyVM();
            vm.schedule2HousePropertyVM = schedule2HousePropertyVM;
            Schedule2HousePropertyMonthlyRepo _schedule2HousePropertyMonthlyRepo = new Schedule2HousePropertyMonthlyRepo();
            schedule2HousePropertyVM = _schedule2HousePropertyMonthlyRepo.SelectAll(0, conditionFieldsHP, conditionValuesHP).FirstOrDefault();
            if (schedule2HousePropertyVM != null)
            {
                vm.schedule2HousePropertyVM = schedule2HousePropertyVM;

                #region Fetch Data From Employee Schedule2 Tax Slab Details
                EmployeeSchedule2TaxSlabDetailMonthlyRepo _employeeSchedule2TaxSlabDetailRepo = new EmployeeSchedule2TaxSlabDetailMonthlyRepo();
                List<EmployeeSchedule2TaxSlabDetailVM> employeeSchedule2TaxSlabDetailVMs = new List<EmployeeSchedule2TaxSlabDetailVM>();
                employeeSchedule2TaxSlabDetailVMs = _employeeSchedule2TaxSlabDetailRepo.SelectAll(0, schedule2HousePropertyVM.Id);
                schedule2HousePropertyVM.employeeSchedule2TaxSlabDetailVMs = employeeSchedule2TaxSlabDetailVMs;
                vm.EmployeeSchedule2TaxSlabCount = schedule2HousePropertyVM.employeeSchedule2TaxSlabDetailVMs.Count;
                #endregion Fetch Data From Employee Schedule2 Tax Slab Details
                vm.FiscalYearDetailId = schedule2HousePropertyVM.FiscalYearDetailId;
                vm.Year = schedule2HousePropertyVM.Year;

            }

            #endregion Fetching Schedule2HousePropertyVM

            #region Assign EmployeeInfo into Schedule2HousePropertyVM
            if (string.IsNullOrWhiteSpace(vm.schedule2HousePropertyVM.EmployeeId))
            {
                vm.schedule2HousePropertyVM.EmployeeId = empInfoVM.EmployeeId;
                vm.schedule2HousePropertyVM.ProjectId = empInfoVM.ProjectId;
                vm.schedule2HousePropertyVM.DepartmentId = empInfoVM.DepartmentId;
                vm.schedule2HousePropertyVM.SectionId = empInfoVM.SectionId;
                vm.schedule2HousePropertyVM.DesignationId = empInfoVM.DesignationId;
            }
            #endregion Assign EmployeeInfo into Schedule2HousePropertyVM



            #region Fetching Schedule3InvestmentVM
            string[] conditionFieldsInv = { "sim.EmployeeId", "sim.FiscalYearDetailId","sim.year" };
            string[] conditionValuesInv = { EmployeeId, fydid,fYear };


            Schedule3InvestmentVM schedule3InvestmentVM = new Schedule3InvestmentVM();
            vm.schedule3InvestmentVM = schedule3InvestmentVM;
            Schedule3InvestmentMonthlyRepo _schedule3InvestmentMonthlyRepo = new Schedule3InvestmentMonthlyRepo();
            schedule3InvestmentVM = _schedule3InvestmentMonthlyRepo.SelectAll(0, conditionFieldsInv, conditionValuesInv, tType).FirstOrDefault();
            if (schedule3InvestmentVM != null)
            {
                vm.schedule3InvestmentVM = schedule3InvestmentVM;

                #region Fetch Data From Employee Schedule3 Tax Slab Details
                EmployeeSchedule3TaxSlabDetailMonthlyRepo _employeeSchedule3TaxSlabDetailRepo = new EmployeeSchedule3TaxSlabDetailMonthlyRepo();
                List<EmployeeSchedule3TaxSlabDetailVM> employeeSchedule3TaxSlabDetailVMs = new List<EmployeeSchedule3TaxSlabDetailVM>();
                employeeSchedule3TaxSlabDetailVMs = _employeeSchedule3TaxSlabDetailRepo.SelectAll(0, schedule3InvestmentVM.Id);
                schedule3InvestmentVM.employeeSchedule3TaxSlabDetailVMs = employeeSchedule3TaxSlabDetailVMs;
                vm.EmployeeSchedule3TaxSlabCount = schedule3InvestmentVM.employeeSchedule3TaxSlabDetailVMs.Count;
                #endregion Fetch Data From Employee Schedule3 Tax Slab Details
                vm.FiscalYearDetailId = schedule3InvestmentVM.FiscalYearDetailId;
                vm.Year = schedule3InvestmentVM.Year;
            }

            #endregion Fetching Schedule3InvestmentVM

            #region Assign EmployeeInfo into Schedule3InvestmentVM
            if (string.IsNullOrWhiteSpace(vm.schedule3InvestmentVM.EmployeeId))
            {
                vm.schedule3InvestmentVM.EmployeeId = empInfoVM.EmployeeId;
                vm.schedule3InvestmentVM.ProjectId = empInfoVM.ProjectId;
                vm.schedule3InvestmentVM.DepartmentId = empInfoVM.DepartmentId;
                vm.schedule3InvestmentVM.SectionId = empInfoVM.SectionId;
                vm.schedule3InvestmentVM.DesignationId = empInfoVM.DesignationId;
            }
            #endregion Assign EmployeeInfo into Schedule3InvestmentVM

            vm.Operation = "update";
            vm.TransactionType = tType;
            return View("Create", vm);
            //return PartialView("_schedule1SalaryMonthly", vm);
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


        public ActionResult SASalaryFullProcess(string fydid = "", string fydidTo = "", string fYear = "",
            string sType = "", string tType = "", List<EmloyeeTAXSlabVM> VMs = null, string advanceTax = "N", string effectForm = "")
        {
            Schedule1SalaryVM schedule1SalaryVM = new Schedule1SalaryVM();
            schedule1SalaryVM.ScheduleType = sType;
            schedule1SalaryVM.TransactionType = tType;
            string mgs = "";
            try
            {
                //Salary
                //Bonus
                //YearlyTax

                if (VMs != null)
                {
                    VMs = VMs.Where(c => c.IsEmployeeChecked).ToList();

                }

                if (tType == "Salary" || tType == "Bonus" || tType == "YearlyTax" || tType == "YearlyTaxAdvanceTAX")
                {
                    if (string.IsNullOrWhiteSpace(fydid))
                    {
                        return View(schedule1SalaryVM);
                    }
                }
                else
                {
                    throw new ArgumentNullException();
                }


                string[] result = new string[6];
                ShampanIdentityVM vm = new ShampanIdentityVM();

                Schedule1SalaryMonthlyRepo _ssMonthlyRepo = new Schedule1SalaryMonthlyRepo();
                //vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                //vm.LastUpdateBy = identity.Name;
                //vm.LastUpdateFrom = identity.WorkStationIP;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;


                if (tType == "Salary" || tType == "Bonus" || tType == "YearlyTax" || tType == "YearlyTaxAdvanceTAX")
                {
                    result = _ssMonthlyRepo.InsertProcessUpdate(fydid, fydidTo, tType, vm, VMs, advanceTax, fYear, effectForm);


                }
                else
                {
                    throw new ArgumentNullException();
                }

                mgs = result[0] + "~" + result[1];
                return Json(mgs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return View(schedule1SalaryVM);
            }
        }

        #region Obsolete
        
        ////public ActionResult BonusTaxProcess(string fydid = "")
        ////{
        ////    return Json("Fail~Under Construction", JsonRequestBehavior.AllowGet);

        ////    Schedule1SalaryVM schedule1SalaryVM = new Schedule1SalaryVM();

        ////    if (string.IsNullOrWhiteSpace(fydid))
        ////    {
        ////        return View();
        ////    }

        ////    string processType = "BonusTax";

        ////    string[] result = new string[6];
        ////    string mgs = "";
        ////    ShampanIdentityVM vm = new ShampanIdentityVM();

        ////    Schedule1SalaryMonthlyRepo _ssMonthlyRepo = new Schedule1SalaryMonthlyRepo();
        ////    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
        ////    vm.CreatedBy = identity.Name;
        ////    vm.CreatedFrom = identity.WorkStationIP;

        ////    result = _ssMonthlyRepo.InsertProcessUpdate(fydid, processType, vm);



        ////    mgs = result[0] + "~" + result[1];
        ////    return Json(mgs, JsonRequestBehavior.AllowGet);
        ////}

        #endregion

        public ActionResult UpdateSalaryTaxDetail(string fydid = "", string tType = "", string advanceTAX = "N")
        {
            if (string.IsNullOrEmpty(advanceTAX))
            {
                advanceTAX = "N";
            }

            if (string.IsNullOrWhiteSpace(fydid))
            {
                ViewBag.tType = tType;
                ViewBag.advanceTAX = advanceTAX;
                return View();
            }

         

            string[] result = new string[6];
            string mgs = "";
            ShampanIdentityVM vm = new ShampanIdentityVM();

            Schedule1SalaryMonthlyRepo _ssMonthlyRepo = new Schedule1SalaryMonthlyRepo();
            //vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //vm.LastUpdateBy = identity.Name;
            //vm.LastUpdateFrom = identity.WorkStationIP;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;

            result = _ssMonthlyRepo.UpdateSalaryTaxDetail(fydid, tType, vm, advanceTAX);

            mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }


        public ActionResult IndexYearly(string EmployeeId = "", string fYear = "")
        {
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.fYear = fYear;
            return View("IndexYearly");
        }

        public ActionResult _indexYearly(JQueryDataTableParamModel param, string EmployeeId = "", string fYear = "")
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
            getAllData = _repo.SelectEmployeeListMonthlies(conditionFields, conditionValues);


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


        [Authorize(Roles = "Admin")]
        public ActionResult EmployeeIndex()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10010", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/TAX/Home");
            }

            EmloyeeTAXSlabVM vm = new EmloyeeTAXSlabVM();
            return View("EmployeeIndex", vm);
        }

        [HttpGet]
        public ActionResult _index(string codeFrom, string codeTo, string departmentId, string projectId
            , string sectionId, string designationId, string gender, string taxSlabId, string dojFrom = "", string dojTo = ""
        )
        {
            SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
            EmloyeeTAXSlabRepo _repo = new EmloyeeTAXSlabRepo();

            #region Declare Variable
            EmloyeeTAXSlabVM vm = new EmloyeeTAXSlabVM();
            if (codeFrom == "0_0" || codeFrom == "0" || codeFrom == "" || codeFrom == "null" || codeFrom == null)
            {
                codeFrom = "";
            }
            if (codeTo == "0_0" || codeTo == "0" || codeTo == "" || codeTo == "null" || codeTo == null)
            {
                codeTo = "";
            }

            if (projectId == "0_0" || projectId == "0" || projectId == "" || projectId == "null" || projectId == null)
            {
                projectId = "";
            }
            if (departmentId == "0_0" || departmentId == "0" || departmentId == "" || departmentId == "null" || departmentId == null)
            {
                departmentId = "";
            }
            if (sectionId == "0_0" || sectionId == "0" || sectionId == "" || sectionId == "null" || sectionId == null)
            {
                sectionId = "";
            }
            #endregion Declare Variable
            List<EmloyeeTAXSlabVM> VMs = new List<EmloyeeTAXSlabVM>();

            string[] conFields = { "ve.Code>", "ve.Code<", "ve.DepartmentId", "ve.ProjectId", "ve.SectionId", "ve.DesignationId", "ve.Gender", "ets.TaxSlabId" };
            string[] conValues = { codeFrom, codeTo, departmentId, projectId, sectionId, designationId, gender, taxSlabId };

            VMs = _repo.SelectAll(conFields, conValues, dojFrom, dojTo);
            return PartialView("_EmployeeList", VMs);
        }


        public ActionResult AdvanceTax(string EmployeeId = "", string tType = "", string fYear = "")
        {
            List<YearlyTAXVM> vms = new List<YearlyTAXVM>();
            try
            {
                Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "1_15", "index").ToString();
                //vms = _repo.SettingsAll();
                Schedule1SalaryMonthlyRepo _ssMonthlyRepo = new Schedule1SalaryMonthlyRepo();

                List<YearlyTAXVM> employeeTAX = _ssMonthlyRepo.YearlyTax(0, new[] { "ssm.EmployeeId","fy.Year" }, new[] { EmployeeId,fYear });

                return View(employeeTAX);
            }
            catch (Exception exception)
            {
                return View(vms);
            }
        }

        public ActionResult AdjustPre()
        {
            List<YearlyTAXVM> vms = new List<YearlyTAXVM>();
            try
            {
                Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "1_15", "index").ToString();
                //vms = _repo.SettingsAll();

                Schedule1SalaryMonthlyRepo _ssMonthlyRepo = new Schedule1SalaryMonthlyRepo();
                
               // TaxDepositRepo rep

                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                return RedirectToAction("Index");

            }
        }


        #region Unused
        [Authorize(Roles = "Admin")]
        [HttpGet]
        //LoadSchedule1SalaryMonthly
        public ActionResult LoadSASalaryM(string EmployeeId = "", string EmployeeCode = "", string fydid = "", string fyear = "")
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




            #region Fetch Data from SalaryEarningDetail
            SalaryEarningDetailVM salaryEarningDetailVM = new SalaryEarningDetailVM();
            List<SalaryEarningDetailVM> salaryEarningDetailVMs = new List<SalaryEarningDetailVM>();
            SalaryEarningRepo _salaryEarningRepo = new SalaryEarningRepo();

            salaryEarningDetailVMs = _salaryEarningRepo.SelectByIdandFiscalyearDetail(empInfoVM.EmployeeId, Convert.ToInt32(fydid));


            #endregion Fetch Data from SalaryEarningDetail

            Schedule1SalaryVM schedule1SalaryMonthlyVM = new Schedule1SalaryVM();
            vm.schedule1SalaryVM = schedule1SalaryMonthlyVM;

            #region Assing EmployeeInfo into Schedule1SalaryMonthlyVM

            vm.schedule1SalaryVM.EmployeeId = empInfoVM.EmployeeId;
            vm.schedule1SalaryVM.ProjectId = empInfoVM.ProjectId;
            vm.schedule1SalaryVM.DepartmentId = empInfoVM.DepartmentId;
            vm.schedule1SalaryVM.SectionId = empInfoVM.SectionId;
            vm.schedule1SalaryVM.DesignationId = empInfoVM.DesignationId;

            #endregion Assing EmployeeInfo into Schedule1SalaryMonthlyVM

            //vm.schedule1SalaryYearlyVM = schedule1SalaryYearlyVM;

            if (salaryEarningDetailVMs != null && salaryEarningDetailVMs.Count > 0)
            {
                vm.schedule1SalaryVM.Line1A = salaryEarningDetailVMs.Where(c => c.SalaryType == "Basic").FirstOrDefault().Amount;
                vm.schedule1SalaryVM.Line5A = salaryEarningDetailVMs.Where(c => c.SalaryType == "HouseRent").FirstOrDefault().Amount;
                vm.schedule1SalaryVM.Line6A = salaryEarningDetailVMs.Where(c => c.SalaryType == "Medical").FirstOrDefault().Amount;
                vm.schedule1SalaryVM.Line8A = salaryEarningDetailVMs.Where(c => c.SalaryType == "Conveyance").FirstOrDefault().Amount;
            }


            #region ExistCheck
            Schedule1SalaryMonthlyRepo _repo = new Schedule1SalaryMonthlyRepo();
            Schedule1SalaryVM sasmVM = new Schedule1SalaryVM();
            string[] conditionFields = { "EmployeeId", "FiscalYearDetailId" };
            string[] conditionValues = { EmployeeId, fydid };
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


            return PartialView("_schedule1SalaryMonthly", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        //ProcessSchedule1SalaryMonthly
        public ActionResult ProcessSASalaryM(string EmployeeId = "", string EmployeeCode = "", string fydid = "", string fyear = ""
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


            Schedule1SalaryVM schedule1SalaryMonthlyVM = new Schedule1SalaryVM();
            Schedule1SalaryMonthlyRepo _repo = new Schedule1SalaryMonthlyRepo();
            bool isMonth = true;
            int taxSlabId = 1;
            schedule1SalaryMonthlyVM = _repo.ProcessSASalary(LineA, taxSlabId, isMonth);
            vm.schedule1SalaryVM = schedule1SalaryMonthlyVM;
            vm.EmployeeTaxSlabCount = schedule1SalaryMonthlyVM.employeeTaxSlabDetailVMs.Count;

            #region Assing EmployeeInfo into Schedule1SalaryMonthlyVM

            vm.schedule1SalaryVM.EmployeeId = empInfoVM.EmployeeId;
            vm.schedule1SalaryVM.ProjectId = empInfoVM.ProjectId;
            vm.schedule1SalaryVM.DepartmentId = empInfoVM.DepartmentId;
            vm.schedule1SalaryVM.SectionId = empInfoVM.SectionId;
            vm.schedule1SalaryVM.DesignationId = empInfoVM.DesignationId;

            #endregion Assing EmployeeInfo into Schedule1SalaryMonthlyVM

            #region ExistCheck
            Schedule1SalaryVM sasmVM = new Schedule1SalaryVM();
            string[] conditionFields = { "ssm.EmployeeId", "ssm.FiscalYearDetailId" };
            string[] conditionValues = { empInfoVM.EmployeeId, fydid };
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

            return PartialView("_schedule1SalaryMonthly", vm);
        }

        #endregion Unused


        public ActionResult EditAdvanceTax(YearlyTAXVM vm, string fYear = "")
        {
            string[] result = new string[6];
            try
            {
                Schedule1SalaryMonthlyRepo _ssMonthlyRepo = new Schedule1SalaryMonthlyRepo();


                Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "1_15", "edit").ToString();
                ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                //vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                //vm.LastUpdateBy = Identity.Name;
                //vm.LastUpdateFrom = Identity.WorkStationIP;

                result = _ssMonthlyRepo.UpdateAdvanceTax(vm);

                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("AdvanceTax", new { vm.EmployeeId});
                //return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("AdvanceTax", new { vm.EmployeeId });


                //return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
                //throw;
            }
        }
    }
}
