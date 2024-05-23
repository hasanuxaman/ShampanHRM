using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Payroll.Controllers
{
    public class SalaryDeductionController : Controller
    {
        //
        // GET: /Payroll/EmployeeDeduction/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _SalaryDeduction(JQueryDataTableParamVM param, string code, string name)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryDeductionRepo repo = new SalaryDeductionRepo();
            var getAllData = repo.SelectAll(Convert.ToInt32(identity.BranchId));
            IEnumerable<SalaryDeductionVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.PeriodName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Remarks.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryDeductionVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.PeriodName :
                                                           sortColumnIndex == 1 && isSortable_2 ? c.Remarks :
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
                , c.Remarks 
            };//{ "", c.PeriodName + "~" + Convert.ToString(c.Id), c.Remarks };
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
        public ActionResult Create()
        {
            return View();
        }
        [HttpGet]
        public JsonResult SalaryDeductionProces(int FiscalPeriodDetailsId, string ProjectId, string DepartmentId, string SectionId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            FiscalYearVM vm = new FiscalYearVM();
            SalaryDeductionRepo repo = new SalaryDeductionRepo();
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);

            string[] result = repo.AddOrUpdate(FiscalPeriodDetailsId, ProjectId, DepartmentId, SectionId, vm);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(string salaryDeductionID)
        {
            SalaryDeductionRepo repo = new SalaryDeductionRepo();
            ViewBag.periodName = repo.GetPeriodName(salaryDeductionID);
            ViewBag.Id = salaryDeductionID;
            return View();
        }
        public ActionResult _salaryDeductionDetails(JQueryDataTableParamModel param, string code, string name, string salaryDeductionID)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryDeductionRepo repo = new SalaryDeductionRepo();
            
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var employeeNameFilter = Convert.ToString(Request["sSearch_1"]);
            var deductionAmountFilter = Convert.ToString(Request["sSearch_2"]);

            //EmployeeName
            //DeductionAmount

            var deductionAmountFrom = 0;
            var deductionAmountTo = 0;
            if (deductionAmountFilter.Contains('~'))
            {
                deductionAmountFrom = deductionAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(deductionAmountFilter.Split('~')[0]) == true ? Convert.ToInt32(deductionAmountFilter.Split('~')[0]) : 0 ;
                deductionAmountTo = deductionAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(deductionAmountFilter.Split('~')[1]) == true ? Convert.ToInt32(deductionAmountFilter.Split('~')[1]) : 0 ;
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

            #region Search and Filter Data
            
            var getAllData = repo.SelectAllSalaryDeductionDetails(salaryDeductionID);
            IEnumerable<SalaryDeductionDetailVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.EmployeeName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.DeductionAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable3 && c.Remarks.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (employeeNameFilter != "" || (deductionAmountFilter != "" && deductionAmountFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c => 
                                            (employeeNameFilter == "" || c.EmployeeName.ToLower().Contains(employeeNameFilter.ToLower()))
                                            &&
                                            (deductionAmountFrom == 0 || deductionAmountFrom <= Convert.ToInt32(c.DeductionAmount))
                                            &&
                                            (deductionAmountTo == 0 || deductionAmountTo >= Convert.ToInt32(c.DeductionAmount))
                                            //&&
                                            //(fromDate == DateTime.MinValue || fromDate <=  Convert.ToDateTime(c.OverTimeDate))
                                            //&&
                                            //(toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.OverTimeDate))
                                        );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryDeductionDetailVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.EmployeeName :
                                                           sortColumnIndex == 4 && isSortable_2 ? c.DeductionAmount.ToString() :
                                                           sortColumnIndex == 5 && isSortable_3 ? c.Remarks :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { 
                //"" + "~" + 
                Convert.ToString(c.Id)
                , c.EmployeeName //+ "~" + Convert.ToString(c.Id)
                , c.DeductionAmount.ToString()
                
                //, c.Remarks 
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


        public JsonResult SalaryDeductionDetailsDelete(string ids)
        {
            SalaryDeductionRepo repo = new SalaryDeductionRepo();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = repo.SalaryDeductionDetailsDelete(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        public JsonResult SalaryDeductionDelete(string ids)
        {
            SalaryDeductionRepo repo = new SalaryDeductionRepo();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = repo.SalaryDeductionDelete(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult SalaryDeductionSingle()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SingleDeductionEdit(int DeductionDetailsId)
        {
            SalaryDeductionDetailVM vm = new SalaryDeductionRepo().GetByIdSalaryDeductionDetails(DeductionDetailsId);
            return View(vm);
        }
        [HttpPost]
        public ActionResult SingleDeductionEdit(SalaryDeductionDetailVM vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryDeductionRepo repo = new SalaryDeductionRepo();

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMdd");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            string[] result = repo.SalaryDeductionSingleEdit(vm);
            ViewBag.result = result[0];
            ViewBag.msg = result[1];
            return View();
        }

        [HttpPost]
        public JsonResult SalaryDeductionSingle(string FiscalPeriodDetailsId, string empID)
        {
            SalaryDeductionDetailVM vm = new SalaryDeductionDetailVM();
            SalaryDeductionRepo repo = new SalaryDeductionRepo();
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
            vm.CreatedBy = Identity.Name;
            vm.CreatedFrom = Identity.WorkStationIP;
            vm.FiscalYearDetailId = FiscalPeriodDetailsId;
            vm.EmployeeId = empID;


            string[] getAllData = repo.SalaryDeductionSingleAddorUpdate(vm, Convert.ToInt32(Identity.BranchId));
            ViewBag.mgs = getAllData[0];
            return Json(getAllData, JsonRequestBehavior.AllowGet);
            //return JavaScript(string.Format("ShowResult('{0}','{1}','{2}','{3}')", getAllData[0], getAllData[1], dataAction, "/Payroll/EmployeeBonus"));
        }
    }
}
