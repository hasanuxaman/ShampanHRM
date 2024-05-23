using CrystalDecisions.CrystalReports.Engine;
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
    public class SalaryEarningController : Controller
    {
        //
        // GET: /Payroll/EmployeeEarning/
				SymUserRoleRepo _reposur = new SymUserRoleRepo();
				ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_43", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View();
        }
        public ActionResult _SalaryEarning(JQueryDataTableParamVM param, string code, string name)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryEarningRepo repo = new SalaryEarningRepo();
            var getAllData = repo.GetPeriodname();
            IEnumerable<SalaryEarningDetailVM> filteredData;
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
            Func<SalaryEarningDetailVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.PeriodStart :
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
                 Convert.ToString(c.FiscalYearDetailId)
                , c.PeriodName 
                , c.Remarks 
            };
            //{ "", c.PeriodName + "~" + Convert.ToString(c.Id), c.Remarks };
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
        public ActionResult Create(string id = "0")
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_43", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            EmployeeInfoVM vm = new EmployeeInfoVM();
            SalaryEarningRepo saEarningrepo = new SalaryEarningRepo();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            List<SalaryEarningDetailVM> saEarningDvms = new List<SalaryEarningDetailVM>();
            SalaryEarningDetailVM saEarningDvm = new SalaryEarningDetailVM();
            if (id != "0")
            {
                saEarningDvm = saEarningrepo.SelectById(id);//find emp code
                vm = repo.SelectById(saEarningDvm.EmployeeId);
                vm.FiscalYearDetailId = saEarningDvm.FiscalYearDetailId;
                vm.SalaryEarningVMs = saEarningDvms;
            }
            Session["empid"] = id;
           return RedirectToAction("SingleEarningEdit", vm);
        }
        [HttpPost]
        public ActionResult Create(List<SalaryEarningDetailVM> vm)
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            SalaryEarningRepo saEarningrepo = new SalaryEarningRepo();
            List<SalaryEarningDetailVM> saEarningDvm = new List<SalaryEarningDetailVM>();
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            //saEarningDvm = vm.SalaryEarningVMs.ToList();
            result = saEarningrepo.SalaryEarningSingleAddorUpdate(saEarningDvm);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int FID)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_43", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            SalaryEarningRepo repo = new SalaryEarningRepo();
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            ViewBag.periodName = repo.SelectAll(FID).FirstOrDefault().PeriodName;
            ViewBag.Id = FID;
            return View();
        }
        public ActionResult _EarningDetails(JQueryDataTableParamVM param, int FID)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var EmployeeNameFilter = Convert.ToString(Request["sSearch_2"]);
            #endregion
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryEarningRepo arerepo = new SalaryEarningRepo();
            var getAllData = arerepo.GetEmployeebyfid(FID);
            IEnumerable<EmployeeInfoVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (codeFilter != "" || EmployeeNameFilter != "" )
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.EmpName.ToLower().Contains(codeFilter.ToLower()))
                                   && EmployeeNameFilter == "" || c.EmpName.ToLower().Contains(EmployeeNameFilter.ToLower())
                                        );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { Convert.ToString(c.Id), c.Code, c.EmpName};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult SingleEarningEdit(string empId,int fid=0)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_43", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            List<SalaryEarningDetailVM> sevms = new List<SalaryEarningDetailVM>();
            SalaryEarningRepo arerepo = new SalaryEarningRepo();
            if (!string.IsNullOrWhiteSpace(empId) && fid != 0)
            {
                sevms = arerepo.SelectByIdandFiscalyearDetail(empId, fid);
                vm = repo.SelectById(empId);
                Session["empid"] = empId;
                Session["FiscalYearDetailId"] = fid;
            }
            vm.SalaryEarningVMs = sevms;
            vm.FiscalYearDetailId = fid;
            return View(vm);
        }
        public ActionResult DetailCreate(string empcode = "", string btn = "current", int FiscalYearDetailId=0, string edType = "0", string id = "0")
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_43", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            string EmployeeId = "";
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            SalaryEarningRepo sODRepo = new SalaryEarningRepo();
            List<SalaryEarningDetailVM> EarningVMs = new List<SalaryEarningDetailVM>();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrWhiteSpace(id) && id != "0")
            {
                EmployeeId = id;
                //EmployeeId = Session["empid"].ToString();
                //FiscalYearDetailId = Convert.ToInt32(Session["FiscalYearDetailId"]);
                EarningVMs = sODRepo.SelectByIdandFiscalyearDetail(EmployeeId, FiscalYearDetailId);
                vm = repo.SelectById(EmployeeId);
                vm.SalaryEarningVMs = EarningVMs;
                //Session["empid"] = "";
                //Session["FiscalYearDetailId"] = "";
            }
            else
            {
                vm = repo.SelectEmpForSearch(empcode, btn);
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    EmployeeId = vm.Id;
                    EarningVMs = sODRepo.SelectByIdandFiscalyearDetail(vm.Id, FiscalYearDetailId);
                    vm.SalaryEarningVMs = EarningVMs;
                }
                else {
                    vm.EmpName = "Employee Name";
                    vm.SalaryEarningVMs = EarningVMs;
                }
            }
            return PartialView("_detailCreate", vm);
        }
        public ActionResult _salaryEarningDetailsByEmployee(JQueryDataTableParamVM param, string employeeId, int PeriodId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryEarningRepo repo = new SalaryEarningRepo();
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var salaryTypeFilter = Convert.ToString(Request["sSearch_1"]);
            var amountFilter = Convert.ToString(Request["sSearch_2"]);
            var remarksFilter = Convert.ToString(Request["sSearch_3"]);
            //SalaryType
            //Amount
            //Remarks 
            var amountFrom = 0;
            var amountTo = 0;
            if (amountFilter.Contains('~'))
            {
                amountFrom = amountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(amountFilter.Split('~')[0]) == true ? Convert.ToInt32(amountFilter.Split('~')[0]) : 0;
                amountTo = amountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(amountFilter.Split('~')[1]) == true ? Convert.ToInt32(amountFilter.Split('~')[1]) : 0;
            }
            #endregion 
            var getAllData = repo.SelectAllSalaryEarningDetails(employeeId,PeriodId);
            IEnumerable<EmployeeInfoVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.SalaryType.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Amount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable3 && c.Remarks.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (salaryTypeFilter != "" || (amountFilter != "~" && amountFilter != "") || remarksFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (salaryTypeFilter == "" || c.SalaryType.ToLower().Contains(salaryTypeFilter.ToLower()))
                                            &&
                                             (amountFrom == 0 || amountFrom <= Convert.ToInt32(c.Amount))
                                            &&
                                            (amountTo == 0 || amountTo >= Convert.ToInt32(c.Amount))
                                             &&
                                             (remarksFilter == "" || c.Remarks.ToLower().Contains(remarksFilter.ToLower()))
                                        );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.FullName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Amount.ToString() :
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
                             Convert.ToString(c.Id) + "~" + c.SalaryTypeId
                             , c.SalaryType                            
                             , c.Amount.ToString()
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
        public ActionResult SalaryEarningDetailsDelete(string ids)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_43", "delete").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            SalaryEarningRepo repo = new SalaryEarningRepo();
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = repo.SalaryEarningDetailsDelete(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        public JsonResult SalaryEarningDelete(string ids)
        {
            SalaryEarningRepo repo = new SalaryEarningRepo();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = repo.SalaryEarningDelete(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult SalaryEarningSingle()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_43", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View();
        }
        [HttpGet]
        public ActionResult EmployeeSalaryEarn(string employeeId,int PeriodId)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_43", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            ViewBag.employeeId = employeeId;
            ViewBag.PeriodId=PeriodId;
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult SingleEarningEdit(string id, string employeeId, string SalaryTypeId)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_43", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            SalaryEarningDetailVM vm = new SalaryEarningDetailVM();
            SalaryEarningRepo repo = new SalaryEarningRepo();
            vm = repo.SalaryEarningBySalaryTypeSingle(id, employeeId, SalaryTypeId);
            //EmployeeAreerRepo repo = new EmployeeAreerRepo();
            //vm = repo.SelectById(vm);
            //var tt = Erepo.SelectById(vm.EmployeeId);
            //vm.EmployeeId = vm.EmployeeId;
            //vm.EmpName = tt.Salutation_E + " " + tt.MiddleName + " " + tt.LastName + "(" + tt.Code + ")";
            //vm.AreerAmount = 0;
            //vm.AreerDate = DateTime.Now.ToString("dd/MMM/yyyy");
            return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult SingleEarningEdit(SalaryEarningDetailVM vm)
        {
            string[] result = new string[6];
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                //vm.FiscalYearDetailId = new FiscalYearRepo().FiscalPeriodIdByDate(Ordinary.DateToString(vm.AreerDate));
                result = new SalaryEarningRepo().SalaryEarningSingleEdit(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        public ActionResult ImportEarning()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_43", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View();
        }
        public ActionResult ImportEarningExcel(HttpPostedFileBase file)
        {
            string[] result = new string[6];
            try
            {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_43", "add").ToString();
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
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                SalaryEarningDetailVM vm = new SalaryEarningDetailVM();
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                new SalaryEarningRepo().ImportExcelFile(file.FileName, vm);
                Session["result"] = "Success~Data Upload Successfully";
                return RedirectToAction("ImportEarning");
                //return RedirectToAction("OpeningBalance");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Upload";
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportEarning");
            }
        }
        public ActionResult DownloadEarningExcel(HttpPostedFileBase file,int fid, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT)
        {
            string[] result = new string[6];
            try
            {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_43", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
                string FileName = "Download.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                //string fullPath = @"C:\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }
                new SalaryEarningRepo().ExportExcelFile(fullPath, FileName, fid, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT);
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
        public ActionResult SalaryEarningReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
            , string Orderby, string view, string rptPG, int fid = 0, int fidTo = 0)
        {
            try
            {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
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
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                {
                    vCodeF = CodeF;
                    codeFParam = vCodeF;
                }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                {
                    vCodeT = CodeT;
                    codeTParam = vCodeT;
                }
                if (rptPG.ToLower() == "fiscal period")
                    rptPG = "FP";
                else if (rptPG.ToLower() == "employee name")
                    rptPG = "EN";
                ReportDocument doc = new ReportDocument();
                SalaryEarningRepo _repo = new SalaryEarningRepo();
                //var BranchId = Convert.ToInt32(identity.BranchId);
                //var getAllData = _repo.SelectAll(BranchId);
                List<SalaryEarningDetailVM> getAllData = new List<SalaryEarningDetailVM>();
                getAllData = _repo.SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, Orderby);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Salary Earning";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Salary Earning";
                }

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                DataSet ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtEarning";
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptSalaryEarning.rpt";
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["rptParamGroup"].Text = "'" + rptPG + "'";
                doc.DataDefinition.FormulaFields["fyParam"].Text = "'" + fyParam + "'";
                doc.DataDefinition.FormulaFields["fyToParam"].Text = "'" + fyToParam + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
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



        public ActionResult SalaryReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
          , string Orderby, string view, string rptPG, int fid = 0, int fidTo = 0)
        {
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrWhiteSpace(view) || view == "Y")
                {
                    return View();
                }
                //string vProjectId = "0_0";
                //string vDepartmentId = "0_0";
                //string vSectionId = "0_0";
                //string vDesignationId = "0_0";
                //string vCodeF = "0_0";
                //string vCodeT = "0_0";
                //string projectParam = "[All]";
                //string deptParam = "[All]";
                //string secParam = "[All]";
                //string desigParam = "[All]";
                //string codeFParam = "[All]";
                //string codeTParam = "[All]";
                //string fyParam = "[All]";
                //string fyToParam = "[All]";
                //if (fid != 0)
                //{
                //    FiscalYearRepo fRepo = new FiscalYearRepo();
                //    fyParam = fRepo.FYPeriodDetail(fid).FirstOrDefault().PeriodName;
                //}
                //if (fidTo != 0)
                //{
                //    FiscalYearRepo fRepo = new FiscalYearRepo();
                //    fyToParam = fRepo.FYPeriodDetail(fidTo).FirstOrDefault().PeriodName;
                //}
               
               
               
               
                
                ReportDocument doc = new ReportDocument();
                SalaryEarningRepo _repo = new SalaryEarningRepo();
                DataSet ds = new DataSet();
                SalarySheetVM vm = new SalarySheetVM();
                vm.FiscalYearDetailId = fid;
             
                ds = _repo.SalarySheet(vm);
               
                //string ReportHead = "";
                //ReportHead = "There are no data to Preview for Salary Earning";
                //if (dt.Count > 0)
                //{
                //    ReportHead = "Salary Earning";
                //}
                DataTable table = new DataTable();

                ds.Tables[0].TableName = "dtSalary";
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptSalary_TIB.rpt";
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                //string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                //doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                //doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                //doc.DataDefinition.FormulaFields["rptParamGroup"].Text = "'" + rptPG + "'";
                //doc.DataDefinition.FormulaFields["fyParam"].Text = "'" + fyParam + "'";
                //doc.DataDefinition.FormulaFields["fyToParam"].Text = "'" + fyToParam + "'";
                //doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                //doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                //doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                //doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                //doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                //doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
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
           , string CodeF, string CodeT, int fid = 0, int fidTo = 0, string Orderby = null)
        {
            SalaryEarningDetailVM vm = new SalaryEarningDetailVM();
            vm.ProjectId = ProjectId;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.DesignationId = DesignationId;
            vm.CodeF = CodeF;
            vm.CodeT = CodeT;
            vm.FiscalYearDetailId = fid;
            vm.fidTo = fidTo;
            vm.Orderby = Orderby;
            return PartialView("_rptIndex", vm);
        }
        public ActionResult _rptIndex(JQueryDataTableParamVM param, string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, int fid = 0, int fidTo = 0, string Orderby = null)
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
            SalaryEarningRepo repo = new SalaryEarningRepo();
            var getAllData = new SalaryEarningRepo().SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                vDesignationId, vCodeF, vCodeT,Orderby);
            IEnumerable<SalaryEarningDetailVM> filteredData;
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
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_9"]);
                var isSearchable10 = Convert.ToBoolean(Request["bSearchable_10"]);
                var isSearchable11 = Convert.ToBoolean(Request["bSearchable_11"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.PeriodName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable8 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable9 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable10 && c.SalaryName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable10 && c.Amount.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<SalaryEarningDetailVM, string> orderingFunction = (c => sortColumnIndex == 0 && isSortable_0 ? c.PeriodStart :
                                                           sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Department :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Section :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Project :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.BasicSalary.ToString() :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.GrossSalary.ToString() :
                                                           sortColumnIndex == 9 && isSortable_9 ? c.SalaryName :
                                                           sortColumnIndex == 10 && isSortable_10 ? c.Amount.ToString() :
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
                             , c.SalaryName.ToString()
                             , c.Amount.ToString() 
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
    }
}
