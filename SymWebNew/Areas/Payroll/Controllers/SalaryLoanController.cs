using SymOrdinary;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.Loan;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using System.Linq;
using JQueryDataTables.Models;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Data;
//using SymWebUI.Areas.Payroll.Reports.PayrollProcess;
using SymRepository.HRM;
using SymViewModel.HRM;
using SymRepository.Common;
using System.Configuration;
using SymViewModel.Enum;
using SymRepository.Enum;
using SymViewModel.Payroll;
using OfficeOpenXml;
using OfficeOpenXml.Style;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class SalaryLoanController : Controller
    {
        //
        // GET: /Payroll/EmployeeLoan/
				SymUserRoleRepo _reposur = new SymUserRoleRepo();
				ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        SalaryLoanRepo repo = new SalaryLoanRepo();
        public ActionResult Index()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_47", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            //repo.GetChildAllowanceAmount("1_10",6);
            return View();
        }
        public ActionResult _SalaryLoan(JQueryDataTableParamVM param, string code, string name)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var getAllData = repo.GetPeriodname();
            IEnumerable<SalaryLoanDetailVM> filteredData;
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
            Func<SalaryLoanDetailVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.PeriodStart :
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
               var permission= _reposur.SymRoleSession(identity.UserId, "1_47", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View();
        }
        [HttpGet]
        public ActionResult SalaryLoanProces(int FiscalPeriodDetailsId, string ProjectId, string DepartmentId, string SectionId)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_47", "process").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            FiscalYearVM vm = new FiscalYearVM();
            SalaryLoanRepo repo = new SalaryLoanRepo();
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            string[] result = repo.AddOrUpdate(FiscalPeriodDetailsId, ProjectId, DepartmentId, SectionId, vm);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int Fid)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_47", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            SalaryLoanRepo _slRepo = new SalaryLoanRepo();
            var vm = _slRepo.SelectAll(Fid).FirstOrDefault();
            if (vm != null)
            {
                ViewBag.periodName = vm.PeriodName;
            }
            ViewBag.Id = Fid;
            return View();
        }
        public ActionResult _salaryLoanDetails(JQueryDataTableParamModel param, int Fid)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var CodeFilter = Convert.ToString(Request["sSearch_1"]);
            var employeeNameFilter = Convert.ToString(Request["sSearch_2"]);
            var loanTypeNameFilter = Convert.ToString(Request["sSearch_3"]);
            var principalAmountFilter = Convert.ToString(Request["sSearch_4"]);
            var interestAmountFilter = Convert.ToString(Request["sSearch_5"]);
            var installmentAmountFilter = Convert.ToString(Request["sSearch_6"]);
            var principalAmountFrom = 0;
            var principalAmountTo = 0;
            if (principalAmountFilter.Contains('~'))
            {
                principalAmountFrom = principalAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(principalAmountFilter.Split('~')[0]) == true ?  Convert.ToInt32(principalAmountFilter.Split('~')[0]) : 0;
                principalAmountTo = principalAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(principalAmountFilter.Split('~')[1]) == true ?  Convert.ToInt32(principalAmountFilter.Split('~')[1]) : 0;
            }
            var interestAmountFrom = 0;
            var interestAmountTo = 0;
            if (interestAmountFilter.Contains('~'))
            {
                interestAmountFrom = interestAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(interestAmountFilter.Split('~')[0]) == true ? Convert.ToInt32(interestAmountFilter.Split('~')[0]) : 0;
                interestAmountTo = interestAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(interestAmountFilter.Split('~')[1]) == true ? Convert.ToInt32(interestAmountFilter.Split('~')[1]) : 0;
            }
            var installmentAmountFrom = 0;
            var installmentAmountTo = 0;
            if (installmentAmountFilter.Contains('~'))
            {
                installmentAmountFrom = installmentAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(installmentAmountFilter.Split('~')[0]) == true ? Convert.ToInt32(installmentAmountFilter.Split('~')[0]) : 0;
                installmentAmountTo = installmentAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(installmentAmountFilter.Split('~')[1]) == true ? Convert.ToInt32(installmentAmountFilter.Split('~')[1]) : 0;
            }
            #endregion Column Search
            #region Search and Filter Data
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryLoanRepo repo = new SalaryLoanRepo();
            var getAllData = repo.SelectAll(Fid);
            IEnumerable<SalaryLoanDetailVM> filteredData;
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
                               || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.LoanTypeName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.PrincipalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.InterestAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.InstallmentAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            #region Column Filtering
            if (employeeNameFilter != "" || CodeFilter != "" || loanTypeNameFilter != "" || (principalAmountFilter != "" && principalAmountFilter != "~")
                || (interestAmountFilter != "" && interestAmountFilter != "~") || (installmentAmountFilter != "" && installmentAmountFilter != "~")
                )
            {
                filteredData = filteredData
                                .Where(c => (CodeFilter == "" || c.Code.ToLower().Contains(CodeFilter.ToLower()))
                                            && (employeeNameFilter == "" || c.EmployeeName.ToLower().Contains(employeeNameFilter.ToLower()))
                                            && (loanTypeNameFilter == "" || c.LoanTypeName.ToLower().Contains(loanTypeNameFilter.ToLower()))
                                            && (principalAmountFrom == 0 || principalAmountFrom <= Convert.ToInt32(c.PrincipalAmount))
                                            && (principalAmountTo == 0 || principalAmountTo >= Convert.ToInt32(c.PrincipalAmount))
                                            && (interestAmountFrom == 0 || interestAmountFrom <= Convert.ToInt32(c.InterestAmount))
                                            && (interestAmountTo == 0 || interestAmountTo >= Convert.ToInt32(c.InterestAmount))
                                            && (installmentAmountFrom == 0 || installmentAmountFrom <= Convert.ToInt32(c.InstallmentAmount))
                                            && (installmentAmountTo == 0 || installmentAmountTo >= Convert.ToInt32(c.InstallmentAmount))
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
            Func<SalaryLoanDetailVM, string> orderingFunction = (c =>  sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                            sortColumnIndex == 2 && isSortable_2 ? c.EmployeeName :
                                                            sortColumnIndex == 3 && isSortable_3 ? c.LoanTypeName :
                                                            sortColumnIndex == 4 && isSortable_4 ? c.PrincipalAmount.ToString() :
                                                            sortColumnIndex == 5 && isSortable_5 ? c.InterestAmount.ToString() :
                                                            sortColumnIndex == 6 && isSortable_6 ? c.InstallmentAmount.ToString() :
                                                            "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] {
                //"" + "~" + 
                Convert.ToString(c.EmployeeId)
                ,c.Code
                , c.EmpName 
                , c.LoanTypeName 
                , c.PrincipalAmount.ToString()
                , c.InterestAmount.ToString()
                , c.InstallmentAmount.ToString()
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
        public ActionResult SingleSalaryLoanEdit(string empId, int fid = 0)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_47", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            List<SalaryLoanDetailVM> sevms = new List<SalaryLoanDetailVM>();
            SalaryLoanRepo arerepo = new SalaryLoanRepo();
            if (!string.IsNullOrWhiteSpace(empId) && fid != 0)
            {
                sevms = arerepo.SelectByIdandFiscalyearDetail(empId, fid);
            }
            vm = repo.SelectById(empId);
            Session["empid"] = empId;
            Session["FiscalYearDetailId"] = fid;
            vm.SalaryLoanDetailVMs = sevms;
            vm.FiscalYearDetailId = fid;
            return View(vm);
        }
        public ActionResult DetailCreate(string empcode = "", string btn = "current", int FiscalYearDetailId = 0, int id = 0)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_47", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            string EmployeeId = "";
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            SalaryLoanRepo sODRepo = new SalaryLoanRepo();
            List<SalaryLoanDetailVM> LoanVMs = new List<SalaryLoanDetailVM>();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrWhiteSpace(Session["empid"] as string) && Session["empid"].ToString() != "0_0"
               && !string.IsNullOrWhiteSpace(Session["FiscalYearDetailId"] as string) && Session["FiscalYearDetailId"] as string != "0"
 )
            {
                EmployeeId = Session["empid"].ToString();
                FiscalYearDetailId = Convert.ToInt32(Session["FiscalYearDetailId"]);
                LoanVMs = sODRepo.SelectByIdandFiscalyearDetail(EmployeeId, FiscalYearDetailId);
                vm = repo.SelectById(EmployeeId);
                Session["empid"] = "";
                Session["FiscalYearDetailId"] = "";
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
                    LoanVMs = sODRepo.SelectByIdandFiscalyearDetail(vm.Id, FiscalYearDetailId);
                    vm.SalaryLoanDetailVMs = LoanVMs;
                }
            }
            vm.SalaryLoanDetailVMs = LoanVMs;
            return PartialView("_detailCreate", vm);
        }
        public ActionResult Delete(string ids)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_47", "delete").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            SalaryLoanVM LoanVM = new SalaryLoanVM();
            SalaryLoanRepo _slRepo = new SalaryLoanRepo();
            LoanVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            LoanVM.LastUpdateBy = identity.Name;
            LoanVM.LastUpdateFrom = identity.WorkStationIP;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = _slRepo.Delete(LoanVM, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        //public JsonResult SalaryLoanDetailsDelete(string ids)
        //{
        //    SalaryLoanRepo repo = new SalaryLoanRepo();
        //    ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //    string[] a = ids.Split('~');
        //    string[] result = new string[6];
        //    result = repo.SalaryLoanDetailsDelete(a);
        //    return Json(result[1], JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult SalaryLoanDelete(string ids)
        //{
        //    SalaryLoanRepo repo = new SalaryLoanRepo();
        //    string[] a = ids.Split('~');
        //    string[] result = new string[6];
        //    result = repo.SalaryLoanDelete(a);
        //    return Json(result[1], JsonRequestBehavior.AllowGet);
        //}
        [HttpGet]
        public ActionResult SalaryLoanSingle()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_47", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View();
        }
        //[HttpGet]
        //public ActionResult SingleLoanEdit(int LoanDetailsId)
        //{
        //    SalaryLoanDetailVM vm = new SalaryLoanRepo().GetByIdSalaryLoanDetails(LoanDetailsId);
        //    return View(vm);
        //}
        //[HttpPost]
        //public ActionResult SingleLoanEdit(SalaryLoanDetailVM vm)
        //{
        //    ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //    SalaryLoanRepo repo = new SalaryLoanRepo();
        //    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
        //    vm.LastUpdateBy = identity.Name;
        //    vm.LastUpdateFrom = identity.WorkStationIP;
        //    string[] result = repo.SalaryLoanSingleEdit(vm);
        //    Session["result"] = result[0];
        //    ViewBag.msg = result[1];
        //    return View();
        //}
        [HttpPost]
        public JsonResult SalaryLoanSingle(int FiscalPeriodDetailsId, string empID)
        {
            SalaryLoanDetailVM vm = new SalaryLoanDetailVM();
            SalaryLoanRepo repo = new SalaryLoanRepo();
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = Identity.Name;
            vm.CreatedFrom = Identity.WorkStationIP;
            vm.FiscalYearDetailId = FiscalPeriodDetailsId;
            vm.EmployeeId = empID;
            string[] getAllData = repo.SalaryLoanSingleAddorUpdate(vm, Convert.ToInt32(Identity.BranchId));
            ViewBag.mgs = getAllData[0];
            return Json(getAllData, JsonRequestBehavior.AllowGet);
            //return JavaScript(string.Format("ShowResult('{0}','{1}','{2}','{3}')", getAllData[0], getAllData[1], dataAction, "/Payroll/EmployeeBonus"));
        }
        public ActionResult SalaryLoanReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, string view, string rptPG, string Orderby, int fid = 0, int fidTo = 0)
        {
            try
            {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
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

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                ReportDocument doc = new ReportDocument();
                SalaryLoanRepo _repo = new SalaryLoanRepo();
                List<SalaryLoanDetailVM> getAllData = new List<SalaryLoanDetailVM>();
                getAllData = _repo.SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, Orderby);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Salary Loan";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Salary Loan";
                }
                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                DataSet ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtLoan";
                //doc = new rptSalaryLoan();
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptSalaryLoan.rpt";
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
        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }
        public ActionResult _rptIndexPartial(string ProjectId, string DepartmentId, string SectionId, string DesignationId
               , string CodeF, string CodeT, int fid = 0, int fidTo = 0, string Orderby = null)
        {
            SalaryLoanDetailVM vm = new SalaryLoanDetailVM();
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
            SalaryLoanRepo repo = new SalaryLoanRepo();
            var getAllData = new SalaryLoanRepo().SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                vDesignationId, vCodeF, vCodeT,Orderby);
            IEnumerable<SalaryLoanDetailVM> filteredData;
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
                               || isSearchable10 && c.PrincipalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryLoanDetailVM, string> orderingFunction = (c => sortColumnIndex == 0 && isSortable_0 ? c.PeriodStart :
                                                           sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Department :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Section :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Project :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.BasicSalary.ToString() :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.GrossSalary.ToString() :
                                                           sortColumnIndex == 9 && isSortable_9 ? c.PrincipalAmount.ToString() :
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
                             , c.PrincipalAmount.ToString() 
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


        public ActionResult SingleSalaryLoanReport(EmployeeLoanVM vm)
        {
            string[] result = new string[6];
            try
            {
                #region Try

                #region Objects and Variables                
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                ReportDocument doc = new ReportDocument();              
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                int Fid = vm.FiscalYearDetailId;
                SalaryLoanRepo _slRepo = new SalaryLoanRepo(); 
                var vme = _slRepo.SelectAll(Fid);

                dt = Ordinary.ListToDataTable(vme.ToList());

                #endregion
              
                #region Report Call

                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

                string[] conFields = { "ReportType", "ReportId" };
                string[] conValues = { "LoanReport", "LoanReport1" };
                enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);

                string ReportFileName = enumReportVMs.FirstOrDefault().ReportFileName;
                string ReportName = enumReportVMs.FirstOrDefault().Name;

                SettingRepo _sRepo = new SettingRepo();

                FiscalYearDetailVM fydVM = new FiscalYearRepo().FYPeriodDetail(Convert.ToInt32(Fid))
                    .FirstOrDefault();

                string PeriodName = fydVM.PeriodName;

                string FullPeriodName = Convert.ToDateTime("01-" + PeriodName).ToString("MMMM-yyyy");

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                string rptLocation = "";
            
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Loan\" + ReportFileName + ".rpt";

                doc.Load(rptLocation);

                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["FullPeriodName"].Text = "'" + FullPeriodName + "'";               
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";


                #endregion

                dt.TableName = "dtLoanReport";

                doc.SetDataSource(dt);

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;

                #endregion Try
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = "Process Fail";
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log("LoanReport", this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);

                return View();
            }
        }

        public ActionResult DownloadSalaryLoanReport(EmployeeLoanVM vm)
        {
            string[] result = new string[6];
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            List<string> ProjectIdList = new List<string>();
            try
            {
                #region Objects and Variables

                ReportDocument doc = new ReportDocument();

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
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
                #endregion

                #region PeriodName

                FiscalYearRepo _fiscalYearRepo = new FiscalYearRepo();
                FiscalYearDetailVM fiscalYearDetailVM = new FiscalYearDetailVM();
                string PeriodName = "";              
                fiscalYearDetailVM = _fiscalYearRepo.FYPeriodDetail(Convert.ToInt32(vm.FiscalYearDetailId))
                    .FirstOrDefault();
                PeriodName = fiscalYearDetailVM.PeriodName;
                string FullPeriodName = Convert.ToDateTime("01-" + PeriodName).ToString("MMMM-yyyy");
             

                #endregion

                #region Pull Data

                string[] condFields = { "FiscalYearDetailId" };
                string[] condValues = { vm.FiscalYearDetailId.ToString() };

                SalaryLoanRepo _slRepo = new SalaryLoanRepo();
                var vme = _slRepo.SelectAll(vm.FiscalYearDetailId);

                dt = Ordinary.ListToDataTable(vme.ToList());

                var toRemove = new string[] {  "LoanType_E","FiscalYearDetailId","PeriodName","Department","Designation","Section","EmployeeId"
                                                ,"BasicSalary","GrossSalary","JoinDate","DepartmentId","DesignationId","ProjectId","LoanAmount"
                                                ,"Id","SalaryLoanId","SectionId","Remarks","IsActive","IsArchive","CreatedBy","CreatedAt","CreatedFrom"
                                                ,"LastUpdateBy","LastUpdateAt","LastUpdateFrom","EmployeeName","CodeF","CodeT","fidTo","PeriodStart","Orderby"
                                            };

                List<string> oldColumnNames = new List<string> { "EmpName", "LoanTypeName", "PrincipalAmount", "InterestAmount", "InstallmentAmount" };
                List<string> newColumnNames = new List<string> { "Employee Name", "Type Name", "Principal Amount", "Interest Amount", "Installment Amount" };
                dt = Ordinary.DtColumnNameChangeList(dt, oldColumnNames, newColumnNames);

                foreach (string col in toRemove)
                {
                    dt.Columns.Remove(col);
                }


                #endregion

                #region Validations

                if (dt.Rows.Count == 0)
                {
                    result[0] = "Fail";
                    result[1] = "No Data Found";
                    Session["result"] = result[0] + "~" + result[1];

                    return View();
                }

                #endregion

                #region Report Call

                string filename = "";


                string[] conFields = { "ReportType", "ReportId" };
                string[] conValues = { "LoanReport", "LoanReport1" };
                List<EnumReportVM> enumReportVMs = new EnumReportRepo().SelectAll(0, conFields, conValues);

                string ReportName = enumReportVMs.FirstOrDefault().Name;
                filename = ReportName + "-" + PeriodName;

                #endregion

                #region Prepare Excel

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

              if (CompanyName.ToUpper() == "TIB" || CompanyName.ToUpper() == "G4S")
                {
                    CompanyRepo cRepo = new CompanyRepo();
                    CompanyVM comInfo = cRepo.SelectById(1);
                    string Line1 = comInfo.Name; // "BRAC EPL STOCK BROKERAGE LIMITED";
                    string
                        Line2 = comInfo
                            .Address; // "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                    string Line3 = "";
                                 
                    string[] ReportHeaders = new string[] { Line1, Line2, Line3 };

                    ExcelSheetFormat(dt, workSheet, ReportHeaders);
                }
          
                #endregion

                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                #endregion

                #region Redirect

                result[0] = "Success";
                result[1] = "Successful~Data Download";

                Session["result"] = result[0] + "~" + result[1];
                return Redirect("SalarySheet");

                #endregion
            }
            catch (Exception e)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(
                    result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine +
                    result[5].ToString(), this.GetType().Name,
                    result[4].ToString() + Environment.NewLine + result[3].ToString());
                return Redirect("SalarySheet");
            }
        }

        private void ExcelSheetFormat(DataTable dt, ExcelWorksheet workSheet, string[] ReportHeaders)
        {
            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;

            int InWordsRow = 0;
            InWordsRow = GrandTotalRow + 1;

            int SignatureSpaceRow = 0;
            SignatureSpaceRow = InWordsRow + 1;

            int SignatureRow = 0;
            SignatureRow = InWordsRow + 4;
            workSheet.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

            #region Format

            var format = new OfficeOpenXml.ExcelTextFormat();
            format.Delimiter = '~';
            format.TextQualifier = '"';
            format.DataTypes = new[] { eDataTypes.String };


            for (int i = 0; i < ReportHeaders.Length; i++)
            {
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment =
                    ExcelHorizontalAlignment.Left;
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                workSheet.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);
            }

            int colNumber = 0;

            foreach (DataColumn col in dt.Columns)
            {
                colNumber++;
                if (col.DataType == typeof(DateTime))
                {
                    workSheet.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                }
                else if (col.DataType == typeof(Decimal))
                {
                    workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                    #region Grand Total

                    workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" +
                                                                        workSheet.Cells[TableHeadRow + 1, colNumber]
                                                                            .Address + ":" +
                                                                        workSheet.Cells[(TableHeadRow + RowCount),
                                                                            colNumber].Address + ")";

                    #endregion
                }
            }

            workSheet.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

            workSheet.Cells[
                    "A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)]
                .Style
                .Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[
                    "A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style
                .Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

            #endregion
        }
    }
}
