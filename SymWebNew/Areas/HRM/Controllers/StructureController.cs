using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymViewModel.HRM;
using SymViewModel.Loan;
using SymViewModel.Payroll;
using SymViewModel.PF;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace SymWebUI.Areas.HRM.Controllers
{
    public class StructureController : Controller
    {
        EmployeeStructureRepo _empRepo = new EmployeeStructureRepo();
        EmployeeStructureGroupRepo _empgrouprepo = new EmployeeStructureGroupRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        public ActionResult Index(string id, string empcode, string btn)
        {
            string currentId = "";
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (empcode != null && btn != null)
            {
                vm = _infoRepo.SelectEmpForSearch(empcode, btn);
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    id = vm.Id;
                }
                else
                {
                    currentId = _infoRepo.DropDown(empcode).FirstOrDefault().Id;
                    id = currentId;
                }
            }
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                id = identity.EmployeeId;

            }
            if (id != null)
            {
                vm = _infoRepo.SelectById(id);
                vm.Id = id;
            }
            var getAllData = _empRepo.SelectEmployeeSalaryStructureAll(id);
            decimal EarningAmnt = 0;
            decimal DeductionAmnt = 0;
            foreach (var item in getAllData.Where(x => x.IsEarning == true))
            {
                EarningAmnt = EarningAmnt + item.TotalValue;
            }
            foreach (var item in getAllData.Where(x => x.IsEarning == false))
            {
                DeductionAmnt = DeductionAmnt + item.TotalValue;
            }
            ViewBag.EarningAmnt = EarningAmnt.ToString();
            ViewBag.DeductionAmnt = DeductionAmnt.ToString();
            return View(vm);
        }
        public ActionResult Structure(string Id)
        {
            EmployeeStructureGroupVM vm = new EmployeeStructureGroupVM();
            vm = _empgrouprepo.SelectByEmployee(Id);
            return PartialView("_structure", vm);
        }

        public JsonResult UpdateEmployeeStructure(string structureType, string structureId, string employeeId
            , string year, string basic, decimal TaxPortion, decimal EmpTaxValue, decimal BonusTaxPortion, decimal EmpBonusTaxValue, decimal FixedOT, bool IsGFApplicable)
        {

            EmployeeStructureGroupRepo _sgRepo = new EmployeeStructureGroupRepo();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "process").ToString();

            ShampanIdentityVM siVM = new ShampanIdentityVM();
            siVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            siVM.LastUpdateBy = identity.Name;
            siVM.LastUpdateFrom = identity.WorkStationIP;

            siVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            siVM.CreatedBy = identity.Name;
            siVM.CreatedFrom = identity.WorkStationIP;
            siVM.BranchId = identity.BranchId;
            string[] result = new string[6];
            if (structureType == "EmployeeGroup")
            {
                result = _sgRepo.EmployeeGroup(employeeId, structureId, siVM);
            }
            else if (structureType == "EarningDeductionStructure")
            {
                result = _sgRepo.EarningDeductionStructure(employeeId, structureId, siVM);
            }
            else if (structureType == "LeaveStructure")
            {
                if (string.IsNullOrWhiteSpace(year))
                {
                    return Json("Please Select Year", JsonRequestBehavior.AllowGet);
                }
                result = _sgRepo.EmployeeLeaveStructure(employeeId, structureId, year, siVM);
            }


            //else if (structureType == "SalaryStructure")
            //{
            //    if (string.IsNullOrWhiteSpace(basic))
            //    {
            //        return Json("Please Input Numeric Basic Salary", JsonRequestBehavior.AllowGet);
            //    }
            //    else if (!Ordinary.IsNumeric(basic))
            //        {
            //            return Json("Please Input Numeric Basic Salary", JsonRequestBehavior.AllowGet);
            //        }
            //    decimal gs = Convert.ToDecimal(basic);
            //    result = srt.EmployeeSalaryStructureFromBasic(employeeId, structureId, gs,"","", siVM);
            //}
            else if (structureType == "PFStructure")
            {
                result = _sgRepo.EmployeePFStructure(employeeId, structureId, siVM);
            }
            else if (structureType == "TaxStructure")
            {
                result = _sgRepo.EmployeeTaxStructure(employeeId, structureId, siVM);
            }
            else if (structureType == "BonusStructure")
            {
                result = _sgRepo.BonusStructure(employeeId, structureId, siVM);
            }
            else if (structureType == "ProjectAllocation")
            {
                result = _sgRepo.ProjectAllocation(employeeId, structureId, siVM);
            }
            else if (structureType == "TaxPortion")
            {
                result = _sgRepo.EmployeeTaxPortion(employeeId, TaxPortion,EmpTaxValue, siVM);
            }
            else if (structureType == "BonusTaxPortion")
            {
                result = _sgRepo.EmployeeBonusTaxPortion(employeeId, BonusTaxPortion, EmpBonusTaxValue, siVM);
            }
            else if (structureType == "FixedOT")
            {
                result = _sgRepo.EmployeeFixedOT(employeeId, FixedOT, siVM);
            }
            else if (structureType == "IsGFApplicable")
            {
                result = _sgRepo.EmployeeIsGFApplicable(employeeId, IsGFApplicable, siVM);
            }
            Session["result"] = result[0] + "~" + result[1];
            //return Json(new
            //{
            //    redirectUrl = Url.Action("Index"),
            //});

            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEmpSStructure(string st, string eid)
        {
            EmployeeSalaryStructureDetailVM vm = new EmployeeSalaryStructureDetailVM();
            vm.EmployeeId = eid;
            vm.Id = 0;
            vm.IsEarning = (st == "E" ? true : false);
            return PartialView("AddEmpSStructure", vm);
        }

        [HttpPost]
        public ActionResult AddEmpSStructure(EmployeeSalaryStructureDetailVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            try
            {

                result = _empRepo.InsertDetail(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Structure", action = "Structure", Id = vm.EmployeeId }));

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Structure", action = "Structure", Id = vm.EmployeeId }));

            }
        }
        [HttpGet]
        public ActionResult EditEmpSStructure(string id, string type)
        {
            ViewBag.Type = type;
            return PartialView("AddEmpSStructure", _empRepo.SelectEmployeeSalaryStructureDetail(id));
        }

        [HttpPost]
        public ActionResult EditEmpSStructure(EmployeeSalaryStructureDetailVM vm, string Update, string Delete)
        {
            string btn = (Update == "Update" ? Update : Delete);
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {
                if (btn == "Update")
                {
                    result = _empRepo.UpdateDetail(vm);
                    Session["result"] = result[0] + "~" + result[1];
                }
                else
                {
                    result = _empRepo.DeleteDetail(vm);

                }
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Structure", action = "Structure", Id = vm.EmployeeId }));

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Updated";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Structure", action = "Structure", Id = vm.EmployeeId }));

            }
        }
        public ActionResult DeleteEmpSStructure(string ids)
        {
            EmployeeSalaryStructureDetailVM vm = new EmployeeSalaryStructureDetailVM();
            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                string[] a = ids.Split('~');
                string[] result = new string[6];

                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.Id = Convert.ToInt32(a[0]);
                result = _empRepo.DeleteDetail(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Structure", action = "Index", Id = vm.EmployeeId }));

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Structure", action = "Index", Id = vm.EmployeeId }));

            }
        }

        public ActionResult EmployeeSalaryStructure(string Id)
        {
            return PartialView("_employeeSalaryStructure", new EmployeeStructureGroupRepo().SelectByEmployee(Id));
        }
        public ActionResult _employeeSalaryStructureEarningdetail(string Id, JQueryDataTableParamModel param)
        {
            var SalaryTypeName = Convert.ToString(Request["sSearch_0"]);
            var AmountFilter = Convert.ToString(Request["sSearch_1"]);
            var SalaryType = Convert.ToString(Request["sSearch_2"]);
            var IsEarningFilter = Convert.ToString(Request["sSearch_3"]);
            var isFixedFilter = Convert.ToString(Request["sSearch_4"]);


            var isFixedFilter1 = isFixedFilter.ToLower() == "fixed" ? true.ToString() : false.ToString();
            var IsEarningFilter1 = IsEarningFilter.ToLower() == "earning" ? true.ToString() : false.ToString();

            var getAllData = _empRepo.SelectEmployeeSalaryStructureAll(Id).Where(m => m.IsEarning == true);

            IEnumerable<EmployeeSalaryStructureVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.SalaryTypeName.ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable2 && c.TotalValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable3 && c.SalaryType.ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable4 && c.IsEarning.ToString().ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable5 && c.IsFixed.ToString().ToLower().Contains(param.sSearch.ToLower())
                                );
            }
            else
            {
                filteredData = getAllData;
            }
            if (IsEarningFilter != "" || SalaryTypeName != "" || isFixedFilter != "" || SalaryType != "" || (AmountFilter != "" && AmountFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c => (isFixedFilter == "" || c.IsFixed.ToString().ToLower().Contains(isFixedFilter1.ToLower()))
                                            && (SalaryTypeName == "" || c.SalaryType.ToLower().Contains(SalaryTypeName.ToLower()))
                                            && (SalaryType == "" || c.SalaryType.ToLower().Contains(SalaryType.ToLower()))
                                            && (IsEarningFilter == "" || c.IsEarning.ToString().ToLower().Contains(IsEarningFilter1.ToLower()))
                                            && (AmountFilter == "" || c.TotalValue.ToString().ToLower().Contains(AmountFilter.ToLower().ToString()))

                                        );
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeSalaryStructureVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.SalaryTypeName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.TotalValue.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.SalaryType :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.IsEarning.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.IsFixed.ToString() :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { c.Id.ToString(), c.SalaryTypeName, c.TotalValue.ToString(), c.SalaryType, Convert.ToString(c.IsFixed == true ? "Fixed" : "Rate") };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _employeeSalaryStructureDeductiondetail(string Id, JQueryDataTableParamModel param)
        {
            var SalaryTypeName = Convert.ToString(Request["sSearch_0"]);
            var AmountFilter = Convert.ToString(Request["sSearch_1"]);
            var SalaryType = Convert.ToString(Request["sSearch_2"]);
            var IsEarningFilter = Convert.ToString(Request["sSearch_3"]);
            var isFixedFilter = Convert.ToString(Request["sSearch_4"]);


            var isFixedFilter1 = isFixedFilter.ToLower() == "fixed" ? true.ToString() : false.ToString();
            var IsEarningFilter1 = IsEarningFilter.ToLower() == "earning" ? true.ToString() : false.ToString();

            var getAllData = _empRepo.SelectEmployeeSalaryStructureAll(Id).Where(m => m.IsEarning == false);

            IEnumerable<EmployeeSalaryStructureVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.SalaryTypeName.ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable2 && c.TotalValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable3 && c.SalaryType.ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable4 && c.IsEarning.ToString().ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable5 && c.IsFixed.ToString().ToLower().Contains(param.sSearch.ToLower())
                                );
            }
            else
            {
                filteredData = getAllData;
            }
            if (IsEarningFilter != "" || SalaryTypeName != "" || isFixedFilter != "" || SalaryType != "" || (AmountFilter != "" && AmountFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c => (isFixedFilter == "" || c.IsFixed.ToString().ToLower().Contains(isFixedFilter1.ToLower()))
                                            && (SalaryTypeName == "" || c.SalaryType.ToLower().Contains(SalaryTypeName.ToLower()))
                                            && (SalaryType == "" || c.SalaryType.ToLower().Contains(SalaryType.ToLower()))
                                            && (IsEarningFilter == "" || c.IsEarning.ToString().ToLower().Contains(IsEarningFilter1.ToLower()))
                                            && (AmountFilter == "" || c.TotalValue.ToString().ToLower().Contains(AmountFilter.ToLower().ToString()))

                                        );
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeSalaryStructureVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.SalaryTypeName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.TotalValue.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.SalaryType :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.IsEarning.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.IsFixed.ToString() :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { c.Id.ToString(), c.SalaryTypeName, c.TotalValue.ToString(), c.SalaryType, Convert.ToString(c.IsFixed == true ? "Fixed" : "Rate") };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _employeeLoanStructuredetail(string Id, JQueryDataTableParamModel param)
        {
            //var SalaryTypeFilter = Convert.ToString(Request["sSearch_0"]);
            //var isFixedFilter = Convert.ToString(Request["sSearch_1"]);
            //var PortionFilter = Convert.ToString(Request["sSearch_2"]);

            //var PortionSalaryTypeFilter = Convert.ToString(Request["sSearch_3"]);
            //var AmountFilter = Convert.ToString(Request["sSearch_4"]);
            //var IsEarningFilter = Convert.ToString(Request["sSearch_5"]);


            //var isFixedFilter1 = isFixedFilter.ToLower() == "y" ? true.ToString() : false.ToString();
            //var IsEarningFilter1 = IsEarningFilter.ToLower() == "y" ? true.ToString() : false.ToString();

            var getAllData = _empRepo.SelectEmployeeLoanStructureAll(Id);
            IEnumerable<EmployeeLoanVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.LoanType.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.PrincipalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||

                               isSearchable4 && c.InterestAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                               isSearchable4 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeLoanVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.LoanType :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.PrincipalAmount.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.InterestAmount.ToString() :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.TotalAmount.ToString() :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { c.LoanType
                , c.PrincipalAmount.ToString()
                , Convert.ToString(c.IsFixed == true ? "Fixed" : "Rate")
                , (c.InterestRate/100).ToString("P") 
                , c.InterestAmount.ToString()
                , c.TotalAmount.ToString()
                , c.NumberOfInstallment.ToString()
                , c.ApprovedDate.ToString()
                , c.StartDate.ToString()
                , c.EndDate.ToString()
                , Convert.ToString(c.IsHold == true ? "Hold" : "Not Hold") };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _employeePFStructuredetail(string Id, JQueryDataTableParamModel param)
        {


            var getAllData = _empRepo.SelectEmployeePFStructureAll(Id);
            IEnumerable<EmployeePFVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.PFName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.PFValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.PortionSalaryType.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                               isSearchable4 && c.IsFixed.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeePFVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.PFName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.PFValue.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.PortionSalaryType.ToString() :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.IsFixed.ToString() :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { c.PFName
                ,(c.PFValue/100).ToString("P") 
                 , c.PortionSalaryType.ToString()
                , Convert.ToString(c.IsFixed == true ? "Fixed" : "Rate")
               
        };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _employeeTAXStructuredetail(string Id, JQueryDataTableParamModel param)
        {


            var getAllData = _empRepo.SelectEmployeeTAXtructureAll(Id);
            IEnumerable<EmployeeTaxVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TaxName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.TaxValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.PortionSalaryType.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                               isSearchable4 && c.IsFixed.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeTaxVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TaxName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.TaxValue.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.PortionSalaryType.ToString() :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.IsFixed.ToString() : "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { c.TaxName
                , (c.TaxValue/100).ToString("P")
                 , c.PortionSalaryType.ToString()
                , Convert.ToString(c.IsFixed == true ? "Fixed" : "Rate")
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _employeeBonusStructuredetail(string Id, JQueryDataTableParamModel param)
        {
            var getAllData = _empRepo.SelectEmployeeBonustructureAll(Id);
            IEnumerable<EmployeeBonusVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.BonusStructureName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.BonusValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.PortionSalaryType.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                               isSearchable4 && c.IsFixed.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeBonusVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.BonusStructureName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.BonusValue.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.PortionSalaryType.ToString() :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.IsFixed.ToString() : "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                             c.BonusStructureName
                             , (c.BonusValue/100).ToString("P") 
                             , c.PortionSalaryType.ToString()
                             , Convert.ToString(c.IsFixed == true ? "Fixed" : "Rate")
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            SalaryStructureVM vm = new SalaryStructureVM();
            return View(vm);
        }
        [HttpPost]
        public ActionResult Create(SalaryStructureVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            try
            {
                result = _empRepo.Insert(vm);
                if (result[0] == "Fail")
                {
                    //if (result[1]=="This Salary Structure already exist")
                    //{
                    //    ViewBag.Fail = "This Salary Structure already exist";
                    //}
                    //else
                    //{
                    //    ViewBag.Fail = "Data Not Save Succeessfully";
                    //}
                    ViewBag.Fail = result[1].ToString();

                }
                else
                {
                    ViewBag.Success = "Data saved successfully.";
                }

            }
            catch (Exception)
            {
                ViewBag.Fail = "Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
            }
            return View(vm);
        }

    }
}
