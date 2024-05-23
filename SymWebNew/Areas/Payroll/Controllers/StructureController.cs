using JQueryDataTables.Models;
using OfficeOpenXml;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Loan;
using SymViewModel.Payroll;
using SymViewModel.PF;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class StructureController : Controller
    {
        //
        // GET: /Payroll/Structure/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        EmployeeStructureRepo _empStructureRepo = new EmployeeStructureRepo();
        public ActionResult Index(string Id = null)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_33", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            if (!(identity.IsAdmin || identity.IsPayroll))
            {
                //id = identity.EmployeeId;
            }
            EmployeeInfoVM empVm = new EmployeeInfoVM();
            if (Id != null)
            {
                empVm.Id = Id;
            }
            return View(empVm);
        }
        public ActionResult StructureEmployee(string Id = "", string empcode = "", string btn = "current")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_33", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrWhiteSpace(Id))
            {
                vm = repo.SelectById(Id);
                vm = repo.SelectEmpStructure(vm.Code, btn);
            }
            else
            {
                vm = repo.SelectEmpStructure(empcode, btn);
            }
            if (vm.EmpName == null)
            {
                vm.EmpName = "Employee Name";
            }
            if (vm.Id != null)
            {
                vm.employeeSG = new EmployeeStructureGroupRepo().SelectByEmployee(vm.Id);
                vm.employeeSG.salaryInput = vm.employeeSG.basic;
                if (vm.employeeSG.IsGross)
                {
                    vm.employeeSG.salaryInput = vm.employeeSG.gross;
                }
            }
            else
            {
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                EmployeeStructureGroupVM emgvm = new EmployeeStructureGroupVM();
                if (CompanyName.ToUpper() == "BOLLORE")
                {
                    SettingRepo _setDAL = new SettingRepo();
                    emgvm.BasicPercentage = 60;
                }
                else
                {
                    emgvm.BasicPercentage = 0;
                }
                vm.employeeSG = emgvm;
            }
            if (vm.Id != null)
            {
                var getAllData = _empStructureRepo.SelectEmployeeSalaryStructureAll(vm.Id);
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
            }
            return PartialView("_employeeStructure", vm);
        }
        //public JsonResult UpdateEmployeeStructure(EmployeeStructureGroupVM vm ,string structureType, string structureId, string employeeId, string year, string basic)
        public ActionResult UpdateEmployeeStructure(string structureType, EmployeeStructureGroupVM vm)
        {
            string[] result = new string[6];
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_33", "edit").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                EmployeeStructureGroupRepo srt = new EmployeeStructureGroupRepo();
                ShampanIdentityVM siVM = new ShampanIdentityVM();
                siVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                siVM.LastUpdateBy = identity.Name;
                siVM.LastUpdateFrom = identity.WorkStationIP;
                siVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                siVM.CreatedBy = identity.Name;
                siVM.CreatedFrom = identity.WorkStationIP;
                siVM.BranchId = identity.BranchId;
                //vm.StepId=null;
                //vm.GradeId=null;
                if (structureType == "SalaryStructure")
                {
                    if (vm.salaryInput <= 0)
                    {
                        return Json("Please Input Numeric Salary", JsonRequestBehavior.AllowGet);
                    }

                    if (WebConfigurationManager.AppSettings["CompanyName"].ToLower() == "tib")
                    {
                        result = srt.EmployeeSalaryStructureTIB(vm.EmployeeId, vm.SalaryStructureId, vm.salaryInput, vm.GradeId, vm.StepId, vm.IsGross, vm.BankPayAmount, siVM, vm);
                    }
                    else if(WebConfigurationManager.AppSettings["CompanyName"].ToLower() == "g4s")
                    {
                        result = srt.EmployeeSalaryStructureG4S(vm.EmployeeId, vm.SalaryStructureId, vm.salaryInput, vm.GradeId, vm.StepId, vm.IsGross, vm.BankPayAmount, siVM, vm);
                    }
                    else if (WebConfigurationManager.AppSettings["CompanyName"].ToUpper() == "BOLLORE")
                    {
                        result = srt.EmployeeSalaryStructureBollore(vm.EmployeeId, vm.SalaryStructureId, vm.salaryInput, vm.GradeId, vm.StepId, vm.IsGross, vm.BankPayAmount, siVM, vm);
                    }
                    else
                    {
                        result = srt.EmployeeSalaryStructureFromBasic(vm.EmployeeId, vm.SalaryStructureId, vm.salaryInput, vm.GradeId, vm.StepId, vm.IsGross, vm.BankPayAmount, siVM);
                    }
                }
                else
                {
                    result = srt.EmployeeOtherStructure(vm, siVM);
                }
                if (structureType == "PFStructure")
                {
                    result = srt.EmployeePFStructure(vm.EmployeeId, vm.PFStructureId, siVM);
                }
                else if (structureType == "TaxStructure")
                {
                    result = srt.EmployeeTaxStructure(vm.EmployeeId, vm.TaxStructureId, siVM);
                }
                else if (structureType == "BonusStructure")
                {
                    result = srt.BonusStructure(vm.EmployeeId, vm.BonusStructureId, siVM);
                }
                else if (structureType == "ProjectAllocation")
                {
                    result = srt.ProjectAllocation(vm.EmployeeId, vm.ProjectAllocationId, siVM);
                }
                Session["result"] = result[0] + "~" + result[1];
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
                //throw;
            }
            //return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddEmpSStructure(string st, string eid)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_33", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeSalaryStructureDetailVM vm = new EmployeeSalaryStructureDetailVM();
            vm.EmployeeId = eid;
            vm.Id = 0;
            vm.IsEarning = (st == "E" ? true : false);
            return PartialView("AddEmpSStructure", vm);
        }
        [HttpPost]
        public ActionResult AddEmpSStructure(EmployeeSalaryStructureDetailVM vm)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_33", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            string[] result = new string[6];
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            try
            {
                result = _empStructureRepo.InsertDetail(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("StructureEarningDeductionDetail", new { employeeId = vm.EmployeeId, employeeSalaryStructureId = vm.EmployeeSalaryStructureId });

                //return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Structure", action = "Index", Id = vm.EmployeeId }));
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                //return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Structure", action = "Index", Id = vm.EmployeeId }));
                return RedirectToAction("StructureEarningDeductionDetail", new { employeeId = vm.EmployeeId, employeeSalaryStructureId = vm.EmployeeSalaryStructureId });

            }
        }
        [HttpGet]
        public ActionResult EditEmpSStructure(string id, string type)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_33", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            ViewBag.Type = type;
            EmployeeSalaryStructureDetailVM vm = new EmployeeSalaryStructureDetailVM();
            vm = _empStructureRepo.SelectEmployeeSalaryStructureDetail(id);
            return PartialView("AddEmpSStructure", vm);
        }
        [HttpPost]
        public ActionResult EditEmpSStructure(EmployeeSalaryStructureDetailVM vm, string Update, string Delete)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_33", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            string btn = (Update == "Update" ? Update : Delete);
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {
                if (btn == "Update")
                {
                    result = _empStructureRepo.UpdateDetailNew(vm);
                    Session["result"] = result[0] + "~" + result[1];
                }
                else
                {
                    result = _empStructureRepo.DeleteDetailNew(vm);
                }
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("StructureEarningDeductionDetail", new { employeeId = vm.EmployeeId, employeeSalaryStructureId = vm.EmployeeSalaryStructureId });

                //return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Structure", action = "Index", Id = vm.EmployeeId }));
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Updated";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("StructureEarningDeductionDetail", new { employeeId = vm.EmployeeId, employeeSalaryStructureId = vm.EmployeeSalaryStructureId });
                //return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Structure", action = "Index", Id = vm.EmployeeId }));
            }
        }
        public ActionResult DeleteEmpSStructure(string ids)
        {
            EmployeeSalaryStructureDetailVM vm = new EmployeeSalaryStructureDetailVM();
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_33", "delete").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                string[] a = ids.Split('~');
                string[] result = new string[6];
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.Id = Convert.ToInt32(a[0]);
                result = _empStructureRepo.DeleteDetail(vm);
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
            var permission = _reposur.SymRoleSession(identity.UserId, "1_33", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return PartialView("_employeeSalaryStructure", new EmployeeStructureGroupRepo().SelectByEmployee(Id));
        }
        public ActionResult _employeeSalaryStructureEarningdetail(string Id, string EmployeeSalaryStructureId, JQueryDataTableParamModel param)
        {
            var SalaryTypeName = Convert.ToString(Request["sSearch_0"]);
            var AmountFilter = Convert.ToString(Request["sSearch_1"]);
            //var SalaryType = Convert.ToString(Request["sSearch_2"]);
            var IsEarningFilter = Convert.ToString(Request["sSearch_2"]);
            var isFixedFilter = Convert.ToString(Request["sSearch_3"]);
            var isFixedFilter1 = isFixedFilter.ToLower() == "fixed" ? true.ToString() : false.ToString();
            var IsEarningFilter1 = IsEarningFilter.ToLower() == "earning" ? true.ToString() : false.ToString();
            var getAllData = _empStructureRepo.SelectEmployeeSalaryStructureAll(Id, EmployeeSalaryStructureId).Where(m => m.IsEarning == true);
            IEnumerable<EmployeeSalaryStructureVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                //var isSearchable5 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.SalaryTypeName.ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable2 && c.TotalValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                       //|| isSearchable3 && c.SalaryType.ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable3 && c.IsEarning.ToString().ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable4 && c.IsFixed.ToString().ToLower().Contains(param.sSearch.ToLower())
                                );
            }
            else
            {
                filteredData = getAllData;
            }
            if (IsEarningFilter != ""
                || SalaryTypeName != ""
                || isFixedFilter != ""
                //|| SalaryType != "" 
                || (AmountFilter != "" && AmountFilter != "~")
                )
            {
                filteredData = getAllData
                                .Where(c => (isFixedFilter == "" || c.IsFixed.ToString().ToLower().Contains(isFixedFilter1.ToLower()))
                                            && (SalaryTypeName == "" || c.SalaryType.ToLower().Contains(SalaryTypeName.ToLower()))
                                    //&& (SalaryType == "" || c.SalaryType.ToLower().Contains(SalaryType.ToLower()))
                                            && (IsEarningFilter == "" || c.IsEarning.ToString().ToLower().Contains(IsEarningFilter1.ToLower()))
                                            && (AmountFilter == "" || c.TotalValue.ToString().ToLower().Contains(AmountFilter.ToLower().ToString()))
                                        );
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            //var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeSalaryStructureVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.SalaryTypeName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.TotalValue.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.SalaryType :
                //sortColumnIndex == 4 && isSortable_4 ? c.IsEarning.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.IsFixed.ToString() :
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
                             c.Id.ToString()
                             , c.SalaryTypeName
                             , c.TotalValue.ToString()
                             //, c.SalaryType
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
        public ActionResult _employeeSalaryStructureDeductiondetail(string Id, string EmployeeSalaryStructureId, JQueryDataTableParamModel param)
        {
            var SalaryTypeName = Convert.ToString(Request["sSearch_0"]);
            var AmountFilter = Convert.ToString(Request["sSearch_1"]);
            //var SalaryType = Convert.ToString(Request["sSearch_2"]);
            var IsEarningFilter = Convert.ToString(Request["sSearch_2"]);
            var isFixedFilter = Convert.ToString(Request["sSearch_3"]);
            var isFixedFilter1 = isFixedFilter.ToLower() == "fixed" ? true.ToString() : false.ToString();
            var IsEarningFilter1 = IsEarningFilter.ToLower() == "earning" ? true.ToString() : false.ToString();
            var getAllData = _empStructureRepo.SelectEmployeeSalaryStructureAll(Id, EmployeeSalaryStructureId).Where(m => m.IsEarning == false);
            IEnumerable<EmployeeSalaryStructureVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                //var isSearchable5 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.SalaryTypeName.ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable2 && c.TotalValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                       //|| isSearchable3 && c.SalaryType.ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable3 && c.IsEarning.ToString().ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable4 && c.IsFixed.ToString().ToLower().Contains(param.sSearch.ToLower())
                                );
            }
            else
            {
                filteredData = getAllData;
            }
            if (IsEarningFilter != ""
                || SalaryTypeName != ""
                || isFixedFilter != ""
                //|| SalaryType != ""
                || (AmountFilter != "" && AmountFilter != "~")
                )
            {
                filteredData = filteredData
                                .Where(c => (isFixedFilter == "" || c.IsFixed.ToString().ToLower().Contains(isFixedFilter1.ToLower()))
                                            && (SalaryTypeName == "" || c.SalaryType.ToLower().Contains(SalaryTypeName.ToLower()))
                                    //&& (SalaryType == "" || c.SalaryType.ToLower().Contains(SalaryType.ToLower()))
                                            && (IsEarningFilter == "" || c.IsEarning.ToString().ToLower().Contains(IsEarningFilter1.ToLower()))
                                            && (AmountFilter == "" || c.TotalValue.ToString().ToLower().Contains(AmountFilter.ToLower().ToString()))
                                        );
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            //var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeSalaryStructureVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.SalaryTypeName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.TotalValue.ToString() :
                //sortColumnIndex == 3 && isSortable_3 ? c.SalaryType :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.IsEarning.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.IsFixed.ToString() :
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
                             c.Id.ToString()
                             , c.SalaryTypeName
                             , c.TotalValue.ToString()
                             //, c.SalaryType
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
            var getAllData = _empStructureRepo.SelectEmployeeLoanStructureAll(Id);
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
            var getAllData = _empStructureRepo.SelectEmployeePFStructureAll(Id);
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
            var getAllData = _empStructureRepo.SelectEmployeeTAXtructureAll(Id);
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
            var getAllData = _empStructureRepo.SelectEmployeeBonustructureAll(Id);
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
                         select new[] { c.BonusStructureName
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
            var permission = _reposur.SymRoleSession(identity.UserId, "1_33", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
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
                result = _empStructureRepo.Insert(vm);
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

        public ActionResult _indexEmployeeSalaryStructure(JQueryDataTableParamModel param, string employeeId)
        {
            //00     //Id 
            //01     //TotalValue      
            //02     //IncrementDate       
            //03     //Remarks
            #region Search and Filter Data
            string[] conditionFields = { "d.EmployeeId" };
            string[] conditionValues = { employeeId };
            var getAllData = _empStructureRepo.SelectAllStructure("", conditionFields, conditionValues);

            if (string.IsNullOrWhiteSpace(employeeId))
            {
                getAllData = new List<EmployeeSalaryStructureVM>();
            }

            IEnumerable<EmployeeSalaryStructureVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.TotalValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.IncrementDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Remarks.ToString().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeSalaryStructureVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.TotalValue.ToString() :
                sortColumnIndex == 2 && isSortable_2 ? c.IncrementDate :
                sortColumnIndex == 3 && isSortable_3 ? c.Remarks :
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
                , c.TotalValue.ToString()
                , c.IncrementDate 
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

        public ActionResult StructureEarningDeductionDetail(string employeeId = "", string employeeSalaryStructureId = "")
        {
            EmployeeInfoVM empVm = new EmployeeInfoVM();
            empVm.Id = employeeId;
            empVm.EmployeeSalaryStructureId = employeeSalaryStructureId;
            return View(empVm);
        }

        public ActionResult MultiStructure(EmployeeStructureGroupVM vm)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_33", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            if (!(identity.IsAdmin || identity.IsPayroll))
            {
                //id = identity.EmployeeId;
            }
            
            return View(vm);

        }

        [Authorize(Roles = "Admin")]
        public ActionResult DownloadExcel(EmployeeStructureGroupVM vm)
        {
            DataTable dt = new DataTable();
            string[] result = new string[6];
            try
            {

                dt = _empStructureRepo.ExcelData(vm);

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("SalaryStructure");

                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);


                string filename = "SalaryStructure";


                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                result[0] = "Successfull";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("MultiStructure");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("MultiStructure");
            }
        }

        public ActionResult UploadExcel(EmployeeStructureGroupVM vm)
        {
            string[] result = new string[6];
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                result = _empStructureRepo.ImportExcelFile(vm);
                Session["result"] = result[0] + "~" + result[1];
                return View("MultiStructure");
                ////return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message;
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("MultiStructure", vm);

                ////return RedirectToAction("Index");
            }
        }



    }
}
