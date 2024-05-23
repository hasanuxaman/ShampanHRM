using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class SalaryOtherEarningController : Controller
    {
        //
        // GET: /Payroll/SalaryOtherEarning/
        SalaryOtherEarningRepo _saRepo;
				SymUserRoleRepo _reposur = new SymUserRoleRepo();
				ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //#region Action Methods
        public ActionResult Index()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_44", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            SalaryOtherEarningRepo _eaRepo = new SalaryOtherEarningRepo();
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var PeriodNameFilter = Convert.ToString(Request["sSearch_1"]);
            var RemarksFilter = Convert.ToString(Request["sSearch_2"]);
            #endregion Column Search
            #region Search and Filter Data
            var getAllData = _eaRepo.GetPeriodname();
            IEnumerable<SalaryOtherEarningVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.PeriodName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            #region Column Filtering
            if (PeriodNameFilter != "" || RemarksFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (PeriodNameFilter == "" || c.PeriodName.ToLower().Contains(PeriodNameFilter.ToLower()))
                                    &&
                                    (RemarksFilter == "" || c.EmpName.ToLower().Contains(RemarksFilter.ToLower()))
                                );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryOtherEarningVM, string> orderingFunction = (
                c => sortColumnIndex == 1 && isSortable_1 ? c.PeriodStart :
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
                             Convert.ToString(c.FiscalYearDetailId)
                             , c.PeriodName
                             , c.Remarks 
            };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create(string id = "0")
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_44", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            EmployeeInfoVM vm = new EmployeeInfoVM();
            SalaryOtherEarningRepo arerepo = new SalaryOtherEarningRepo();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            SalaryOtherEarningVM EarningVM = new SalaryOtherEarningVM();
            if (id != "0")
            {
                EarningVM = arerepo.SelectById(id);//find emp code
                vm = repo.SelectById(EarningVM.EmployeeId);
                vm.FiscalYearDetailId = EarningVM.FiscalYearDetailId;
                vm.SalaryOtherEarningVM = EarningVM;
            }
            Session["empid"] = id;
            return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(string btn, EmployeeInfoVM empVM)
        {
            SalaryOtherEarningVM vm = new SalaryOtherEarningVM();
            string[] result = new string[6];
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm = empVM.SalaryOtherEarningVM;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                if (vm.FiscalYearDetailId == 0)
                {
                    Session["result"] = "Fail~Fiscal Year Not Exist on this Period";
                    FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                    return RedirectToAction("Index");
                }
                if (btn.ToLower() != "save")
                {
                    vm.EarningAmount = 0;
                }
                result = new SalaryOtherEarningRepo().Insert(vm);
                if (result[0].ToLower() == "success" && btn.ToLower() != "save")
                {
                    result[1] = "Information Deleted Successfully!";
                }
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Edit(int FID)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_44", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            SalaryOtherEarningRepo arerepo = new SalaryOtherEarningRepo();
            var tt = arerepo.SelectAll(null, FID).FirstOrDefault();
            if (tt != null)
            {
                ViewBag.periodName = tt.PeriodName;
            }
            ViewBag.Id = FID;
            return View();
        }
        ///Remain Unused
        public ActionResult _SalaryOtherEarningDetailsbyFiscal(JQueryDataTableParamModel param, int FID)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var EmployeeNameFilter = Convert.ToString(Request["sSearch_2"]);
            var BasicSalaryFilter = Convert.ToString(Request["sSearch_3"]);
            var GrossSalaryFilter = Convert.ToString(Request["sSearch_4"]);
            var BasicamountFrom = 0;
            var BasicamountTo = 0;
            if (BasicSalaryFilter.Contains('~'))
            {
                BasicamountFrom = BasicSalaryFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(BasicSalaryFilter.Split('~')[0]) == true ? Convert.ToInt32(BasicSalaryFilter.Split('~')[0]) : 0;
                BasicamountTo = BasicSalaryFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(BasicSalaryFilter.Split('~')[1]) == true ? Convert.ToInt32(BasicSalaryFilter.Split('~')[1]) : 0;
            }
            var GrossamountFrom = 0;
            var GrossamountTo = 0;
            if (GrossSalaryFilter.Contains('~'))
            {
                GrossamountFrom = GrossSalaryFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(GrossSalaryFilter.Split('~')[0]) == true ? Convert.ToInt32(GrossSalaryFilter.Split('~')[0]) : 0;
                GrossamountTo = GrossSalaryFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(GrossSalaryFilter.Split('~')[1]) == true ? Convert.ToInt32(GrossSalaryFilter.Split('~')[1]) : 0;
            }
            #endregion
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryOtherEarningRepo arerepo = new SalaryOtherEarningRepo();
            var getAllData = arerepo.SelectAllByFIDPeriod(FID);
            IEnumerable<SalaryOtherEarningVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (codeFilter != "" || EmployeeNameFilter != "" || (BasicSalaryFilter != "" && BasicSalaryFilter != "~") || (GrossSalaryFilter != "" && GrossSalaryFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.EmpName.ToLower().Contains(codeFilter.ToLower()))
                                   && EmployeeNameFilter == "" || c.EmpName.ToLower().Contains(EmployeeNameFilter.ToLower())
                                            && (BasicamountFrom == 0 || BasicamountFrom <= Convert.ToInt32(c.BasicSalary))
                                            && (BasicamountTo == 0 || BasicamountTo >= Convert.ToInt32(c.BasicSalary))
                                             && (GrossamountFrom == 0 || GrossamountFrom <= Convert.ToInt32(c.GrossSalary))
                                            && (GrossamountTo == 0 || GrossamountTo >= Convert.ToInt32(c.GrossSalary))
                                        );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryOtherEarningVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 5 && isSortable_3 ? c.BasicSalary.ToString() :
                                                           sortColumnIndex == 6 && isSortable_4 ? c.GrossSalary.ToString() :
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
                             Convert.ToString(c.EmployeeId)
                             , c.Code
                             , c.EmpName
                             , c.BasicSalary.ToString()
                             , c.GrossSalary.ToString()
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
        public ActionResult EditByEmployee(string empid, int FID)
         {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_44", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
             ViewBag.fId = FID;
             ViewBag.empid = empid;
             SalaryOtherEarningRepo arerepo = new SalaryOtherEarningRepo();
             ViewBag.empName = arerepo.SelectAll(empid, FID).FirstOrDefault().EmpName;
            return View();
        }
        public ActionResult _SalaryOtherEarningDetails(JQueryDataTableParamModel param, int FID)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var EmployeeNameFilter = Convert.ToString(Request["sSearch_2"]);
            var EarningTypeFilter = Convert.ToString(Request["sSearch_3"]);
            var EarningAmountFilter = Convert.ToString(Request["sSearch_4"]);
            var BasicSalaryFilter = Convert.ToString(Request["sSearch_5"]);
            var GrossSalaryFilter = Convert.ToString(Request["sSearch_6"]);
            var BasicamountFrom = 0;
            var BasicamountTo = 0;
            if (BasicSalaryFilter.Contains('~'))
            {
                BasicamountFrom = BasicSalaryFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(BasicSalaryFilter.Split('~')[0]) == true ? Convert.ToInt32(BasicSalaryFilter.Split('~')[0]) : 0;
                BasicamountTo = BasicSalaryFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(BasicSalaryFilter.Split('~')[1]) == true ? Convert.ToInt32(BasicSalaryFilter.Split('~')[1]) : 0;
            }
            var GrossamountFrom = 0;
            var GrossamountTo = 0;
            if (GrossSalaryFilter.Contains('~'))
            {
                GrossamountFrom = GrossSalaryFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(GrossSalaryFilter.Split('~')[0]) == true ? Convert.ToInt32(GrossSalaryFilter.Split('~')[0]) : 0;
                GrossamountTo = GrossSalaryFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(GrossSalaryFilter.Split('~')[1]) == true ? Convert.ToInt32(GrossSalaryFilter.Split('~')[1]) : 0;
            }
            var amountFrom = 0;
            var amountTo = 0;
            if (EarningAmountFilter.Contains('~'))
            {
                amountFrom = EarningAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(EarningAmountFilter.Split('~')[0]) == true ? Convert.ToInt32(EarningAmountFilter.Split('~')[0]) : 0;
                amountTo = EarningAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(EarningAmountFilter.Split('~')[0]) == true ? Convert.ToInt32(EarningAmountFilter.Split('~')[0]) : 0;
            }
            #endregion
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryOtherEarningRepo _saRepo = new SalaryOtherEarningRepo();
            var getAllData = _saRepo.SelectAll(null, FID);
            IEnumerable<SalaryOtherEarningVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.EarningType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.EarningAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (codeFilter != "" || EmployeeNameFilter != "" || EarningTypeFilter != "" || (BasicSalaryFilter != "" && BasicSalaryFilter != "~") || (GrossSalaryFilter != "" && GrossSalaryFilter != "~") || (EarningAmountFilter != "" && EarningAmountFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.EmpName.ToLower().Contains(codeFilter.ToLower()))
                                    && EmployeeNameFilter == "" || c.EmpName.ToLower().Contains(EmployeeNameFilter.ToLower())
                                    && EarningTypeFilter == "" || c.EarningType.ToLower().Contains(EmployeeNameFilter.ToLower())
                                    && (BasicamountFrom == 0 || BasicamountFrom <= Convert.ToInt32(c.BasicSalary))
                                    && (BasicamountTo == 0 || BasicamountTo >= Convert.ToInt32(c.BasicSalary))
                                    && (GrossamountFrom == 0 || GrossamountFrom <= Convert.ToInt32(c.GrossSalary))
                                    && (GrossamountTo == 0 || GrossamountTo >= Convert.ToInt32(c.GrossSalary))
                                    && (amountFrom == 0 || amountFrom <= Convert.ToInt32(c.EarningAmount))
                                    && (amountTo == 0 || amountTo >= Convert.ToInt32(c.EarningAmount))
                                    );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryOtherEarningVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.EarningType.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.EarningAmount.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.BasicSalary.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.GrossSalary.ToString() :
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
                             Convert.ToString(c.Id)
                             , c.Code
                             , c.EmpName
                             , c.EarningType.ToString()
                             , c.EarningAmount.ToString() 
                             , c.BasicSalary.ToString()
                             , c.GrossSalary.ToString()
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
        public ActionResult SingleSalaryOtherEarningEdit(string soeId)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_44", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            SalaryOtherEarningVM soevm = new SalaryOtherEarningVM();
            SalaryOtherEarningRepo arerepo = new SalaryOtherEarningRepo();
            soevm = arerepo.SelectById(soeId);
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            if (!string.IsNullOrWhiteSpace(soeId) && !string.IsNullOrWhiteSpace(soevm.Id))
            {
                vm = repo.SelectById(soevm.EmployeeId);
            }
            vm.SalaryOtherEarningVM = soevm;
            Session["empid"] = soevm.Id;
            vm.FiscalYearDetailId = Convert.ToInt32(soevm.FiscalYearDetailId);
            return View(vm);
        }
        public ActionResult DetailCreate(string empcode = "", string btn = "current", string FiscalYearDetailId = "0", string edType = "0", string id = "0")
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_44", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            string EmployeeId = "";
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            SalaryOtherEarningRepo sODRepo = new SalaryOtherEarningRepo();
            SalaryOtherEarningVM EarningVM = new SalaryOtherEarningVM();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrWhiteSpace(Session["empid"] as string) && Session["empid"] as string != "0")
            {
                string empid = Session["empid"].ToString();
                EarningVM = sODRepo.SelectById(empid);//find emp code
                vm = repo.SelectById(EarningVM.EmployeeId);
                vm.SalaryOtherEarningVM = EarningVM;
                Session["empid"] = "";
            }
            else if (id != "0")
            {
                EarningVM = sODRepo.SelectById(id);//find emp code
                vm = repo.SelectById(EarningVM.EmployeeId);
                vm.SalaryOtherEarningVM = EarningVM;
            }
            else
            {
                vm = repo.SelectEmpForSearch(empcode, btn);
                if (vm.EmpName == null)
                {
                    vm.EmpName = "Employee Name";
                }
                else
                {
                    EmployeeId = vm.Id;
                }
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    EarningVM = sODRepo.SelectByIdandFiscalyearDetail(vm.Id, FiscalYearDetailId, edType);
                    EarningVM.FiscalYearDetailId = Convert.ToInt32(FiscalYearDetailId);
                    EarningVM.EarningTypeId = Convert.ToInt32(edType);
                }
                if (string.IsNullOrWhiteSpace(FiscalYearDetailId))
                {
                    FiscalYearDetailId = "0";
                }
                vm.SalaryOtherEarningVM = EarningVM;
                vm.SalaryOtherEarningVM.EmployeeId = EmployeeId;
                vm.SalaryOtherEarningVM.FiscalYearDetailId = Convert.ToInt32(FiscalYearDetailId);
            }
            return PartialView("_detailCreate", vm);
        }
        public ActionResult Delete(string ids)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_44", "delete").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            SalaryOtherEarningVM earningVM = new SalaryOtherEarningVM();
            SalaryOtherEarningRepo oeRepo = new SalaryOtherEarningRepo();
            earningVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            earningVM.LastUpdateBy = identity.Name;
            earningVM.LastUpdateFrom = identity.WorkStationIP;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = oeRepo.Delete(earningVM, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        public ActionResult ImportOtherEarning()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_44", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View();
        }
        public ActionResult ImportOtherEarningExcel(HttpPostedFileBase file)
        {
            string[] result = new string[6];
            try
            {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_44", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + file.FileName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(fullPath);
                }
                ShampanIdentityVM vm = new ShampanIdentityVM();
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
               result= new SalaryOtherEarningRepo().ImportOtherExcelFile(fullPath, vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportOtherEarning");
                //return RedirectToAction("OpeningBalance");
            }
            catch (Exception)
            {
                 Session["result"] = result[0] + "~" + result[1];
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                 return RedirectToAction("ImportOtherEarning");
            }
        }
        public ActionResult DownloadOtherEarningExcel(HttpPostedFileBase file, string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, int fid = 0, int ETId = 0)
        {
            string[] result = new string[6];
            try
            {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_44", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
                string FileName = "Download.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }
              result=  new SalaryOtherEarningRepo().ExportOtherExcelFile(fullPath, FileName, ProjectId, DepartmentId, SectionId, DesignationId,CodeF, CodeT, fid,ETId);
                Session["result"] = "Success~Data Download Successfully";
                return Redirect("/Files/Export/" + FileName);
                //return Redirect("C:/" + FileName);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Download";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportEarning");
            }
        }
        public ActionResult SalaryOtherEarningReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string Codef, string CodeT, string view, string rptPG1, string rptPG2, string Orderby, int fid = 0, int fidTo = 0, int ETId = 0,string RT="E")
        {
            try
            {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
                ViewBag.RT = RT;
                if (string.IsNullOrWhiteSpace(view) || view == "Y")
                {
                    return View();
                }
                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                string projectParam = "[All]";
                string deptParam = "[All]";
                string secParam = "[All]";
                string desigParam = "[All]";
                string codeFParam = "[All]";
                string codeTParam = "[All]";
                string fyParam = "[All]";
                string fyToParam = "[All]";
                string etParam = "[All]";
                if (fid != 0)
                {
                    FiscalYearRepo fRepo = new FiscalYearRepo();
                    fyParam = fRepo.FYPeriodDetail(fid).FirstOrDefault().PeriodName;
                }
                if (fidTo != 0)
                {
                    FiscalYearRepo fRepo = new FiscalYearRepo();
                    fyToParam = fRepo.FYPeriodDetail(fidTo).FirstOrDefault().PeriodName;
                }
                if (ETId != 0)
                {
                    EarningDeductionTypeRepo edRepo = new EarningDeductionTypeRepo();
                    etParam = edRepo.SelectById(ETId).Name;
                }
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                {
                    vDesignationId = DesignationId;
                    DesignationRepo desRepo = new DesignationRepo();
                    desigParam = desRepo.SelectById(DesignationId).Name;
                }
                if (Codef != "0_0" && Codef != "0" && Codef != "" && Codef != "null" && Codef != null)
                {
                    vCodeF = Codef;
                    codeFParam = vCodeF;
                }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                {
                    vCodeT = CodeT;
                    codeTParam = vCodeT;
                }
                if (rptPG1 == "Fiscal Period")
                    rptPG1 = "FP";
                else if (rptPG1 == "Employee Name")
                    rptPG1 = "EN";
                else if (rptPG1 == "Earning Type")
                    rptPG1 = "ET";
                if (rptPG2 == "Fiscal Period")
                    rptPG2 = "FP";
                else if (rptPG2 == "Employee Name")
                    rptPG2 = "EN";
                else if (rptPG2 == "Earning Type")
                    rptPG2 = "ET";
                string rptLocation = "";
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Other Earning";

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ReportDocument doc = new ReportDocument();
                List<SalaryOtherEarningVM> getAllData = new List<SalaryOtherEarningVM>();
                SalaryOtherEarningRepo repo = new SalaryOtherEarningRepo();
                getAllData = repo.SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, ETId, Orderby);
                if (getAllData.Count > 0)
                    ReportHead = "Other Earning List";
                table = Ordinary.ListToDataTable(getAllData.ToList());
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtSalaryOtherEarningDeduction";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptSalaryOtherEarning.rpt";
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["rptParamGroupOne"].Text = "'" + rptPG1 + "'";
                doc.DataDefinition.FormulaFields["rptParamGroupTwo"].Text = "'" + rptPG2 + "'";
                doc.DataDefinition.FormulaFields["fyParam"].Text = "'" + fyParam + "'";
                doc.DataDefinition.FormulaFields["fyToParam"].Text = "'" + fyToParam + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["etParam"].Text = "'" + etParam + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }
        public ActionResult _rptIndexPartial(string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, int fid = 0, int fidTo = 0, int ETId = 0, string Orderby = null)
        {
            SalaryOtherEarningVM vm = new SalaryOtherEarningVM();
            vm.ProjectId = ProjectId;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.DesignationId = DesignationId;
            vm.CodeF = CodeF;
            vm.CodeT = CodeT;
            vm.FiscalYearDetailId = fid;
            vm.fidTo = fidTo;
            vm.EarningTypeId = ETId;
            vm.Orderby = Orderby;
            return PartialView("_rptIndex", vm);
        }
        public ActionResult _rptIndex(JQueryDataTableParamVM param, string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, int fid = 0, int fidTo = 0, int ETId = 0, string Orderby = null)
        {
            #region Declare Variable
            string vProjectId = "0_0";
            string vDepartmentId = "0_0";
            string vSectionId = "0_0";
            string vDesignationId = "0_0";
            string vCodeF = "0_0";
            string vCodeT = "0_0";
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (!(identity.IsAdmin || identity.IsPayroll))
            {
                //Id = identity.EmployeeId;
                vCodeF = identity.EmployeeCode;
                vCodeT = identity.EmployeeCode;
            }
            else
            {
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                {
                    vProjectId = ProjectId;
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                {
                    vSectionId = SectionId;
                }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                {
                    vDesignationId = DesignationId;
                }
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                {
                    vCodeF = CodeF;
                }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                {
                    vCodeT = CodeT;
                }
            }
            #endregion Declare Variable
            SalaryOtherEarningRepo _repo = new SalaryOtherEarningRepo();
            var getAllData = _repo.SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                vDesignationId, vCodeF, vCodeT, ETId, Orderby);
            IEnumerable<SalaryOtherEarningVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable0 = Convert.ToBoolean(Request["bSearchable_0"]);
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
                filteredData = getAllData
                   .Where(c => isSearchable0 && c.PeriodName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable1 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable8 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable9 && c.EarningType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable10 && c.EarningAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }
            var isSortable_0 = Convert.ToBoolean(Request["bSortable_0"]);
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
            Func<SalaryOtherEarningVM, string> orderingFunction = (c => sortColumnIndex == 0 && isSortable_0 ? c.PeriodStart :
                                                           sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Department :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Section :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Project :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.BasicSalary.ToString() :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.GrossSalary.ToString() :
                                                           sortColumnIndex == 9 && isSortable_9 ? c.EarningType :
                                                           sortColumnIndex == 10 && isSortable_10 ? c.EarningAmount.ToString() :
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
                             c.PeriodName
                             , c.Code
                             , c.EmpName
                             , c.Designation
                             , c.Department
                             , c.Section
                             , c.Project
                             , c.BasicSalary.ToString()
                             , c.GrossSalary.ToString()
                             , c.EarningType
                             , c.EarningAmount.ToString()
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
        //#endregion Action Methods
    }
}
