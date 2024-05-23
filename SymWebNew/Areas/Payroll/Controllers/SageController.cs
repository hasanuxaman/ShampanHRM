using CrystalDecisions.CrystalReports.Engine;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class SageController : Controller
    {
        //
        // GET: /Payroll/Sage/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_49", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        public ActionResult SageIntegrationIndex(string FID = null)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_49", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        public ActionResult _sageIntegrationIndex(JQueryDataTableParamVM param)
        {
            //Id
            //Code
            //PeriodName
            //Description
            //PostDate
            SageRepo _sageRepo = new SageRepo();
            List<JournalLedgerVM> getAllData = new List<JournalLedgerVM>();
            getAllData = _sageRepo.SelectAllJournalLedger();
            IEnumerable<JournalLedgerVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData.Where(c =>
                                  isSearchable1 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.PeriodName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.Description.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.PostDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
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
            Func<JournalLedgerVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.PeriodName :
                sortColumnIndex == 3 && isSortable_3 ? c.Description.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? Ordinary.DateToString(c.PostDate.ToString()) :
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
                             , c.Code   
                             , c.PeriodName   
                             , c.Description   
                             , c.PostDate
                             , c.IsReverse? "Reversed":"Posted"
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
        //public ActionResult SageIntegrationBackup(string FiscalYearDetailId, string PostingDate, string DepartmentId = null, string SectionId = null, string ProjectId = null, string empcodes = null, bool isReverse = false)
        //{
        //    var permission = _reposur.SymRollSession(identity.UserId, "1_49", "add").ToString();
        //    Session["permission"] = permission;
        //    if (permission == "False")
        //    {
        //        return Redirect("/Payroll/Home");
        //    }
        //    string[] result = new string[100];
        //    if (FiscalYearDetailId == null || PostingDate == null)
        //        return View();
        //    SageRepo _sageRepo = new SageRepo();
        //    List<JournalLedgerDetailVM> getAllData = new List<JournalLedgerDetailVM>();
        //    List<string> multiEmpIds = new List<string>();
        //    #region Value assign to Parameters
        //    string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vEmpCodes = "0_0";
        //    if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
        //    {
        //        vProjectId = ProjectId;
        //    }
        //    if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
        //    {
        //        vDepartmentId = DepartmentId;
        //    }
        //    if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
        //    {
        //        vSectionId = SectionId;
        //    }
        //    if (empcodes != "0_0" && empcodes != "0" && empcodes != "" && empcodes != "null" && empcodes != null)
        //    {
        //        vEmpCodes = empcodes;
        //        List<string> EmpCodeList = empcodes.Split(',').ToList();
        //        //var b = a;
        //        EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
        //        foreach (string sEmpId in EmpCodeList)
        //        {
        //            //multiEmployeeId[i] = _empRepo.ViewSelectAllEmployee(a[i]).FirstOrDefault().EmployeeId;
        //            multiEmpIds.Add(_empRepo.ViewSelectAllEmployee(sEmpId).FirstOrDefault().EmployeeId);
        //        }
        //    }
        //    else
        //    {
        //        multiEmpIds = null;
        //    }
        //    #endregion Value assign to Parameters
        //    try
        //    {
        //        result = _sageRepo.SageIntegration(FiscalYearDetailId, PostingDate, vDepartmentId, vSectionId, vProjectId, multiEmpIds, vEmpCodes, isReverse);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        //}
        public ActionResult SageIntegration(string FiscalYearDetailId, string PostingDate, string DepartmentId = null, string SectionId = null, string ProjectId = null, string empcodes = null, bool isReverse = false)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_49", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            string[] result = new string[100];
            if (FiscalYearDetailId == null || PostingDate == null)
                return View();

            SageRepo _sageRepo = new SageRepo();
            List<JournalLedgerDetailVM> getAllData = new List<JournalLedgerDetailVM>();
            List<string> multiEmpIds = new List<string>();

            #region Value assign to Parameters
            string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0";
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
            #endregion Value assign to Parameters


            SalaryProcessRepo _salRepo = new SalaryProcessRepo();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            //call emp view model
            List<string> ProjectIdList = new List<string>();
            ds = _salRepo.SalaryPreCalculationNew(FiscalYearDetailId, vProjectId, vDepartmentId, vSectionId, "0_0", "0_0", "0_0", "", ProjectIdList);
            decimal Dv_Cr = 0;
            decimal Dv_Dr = 0;
            decimal Jv_PF = 0;
            decimal Jv_TAX = 0;
            decimal Jv_Adv = 0;
            decimal Jv_LWP = 0;
            decimal Jv_Trans = 0;
            decimal Jv_Dr = 0;

            foreach (DataRow item in ds.Tables[0].Rows)
            {
                Dv_Cr += (decimal)item["NetPayForJournalCampe"];
                Jv_PF += (decimal)item["PFForJournalCampe"];
                Jv_TAX += (decimal)item["TAXForJournalCampe"];
                Jv_Adv += (decimal)item["AdvanceDeductionForJournalCampe"];
                Jv_LWP += (decimal)item["LeaveWOPayForJournalCampe"];
                Jv_Trans += (decimal)item["TransportBillForJournalCampe"];

            }
            Jv_Dr += Jv_PF + Jv_TAX + Jv_Adv + Jv_LWP + Jv_Trans;

            Dv_Dr = Dv_Cr;

            FiscalYearRepo fyRepo = new FiscalYearRepo();

            var PeriodName = fyRepo.FYPeriodDetail(Convert.ToInt32(FiscalYearDetailId)).FirstOrDefault().PeriodName;
            GLAccountRepo glaRepo = new GLAccountRepo();
            List<GLAccountVM> glavms = new List<GLAccountVM>();
            glavms = glaRepo.SelectAll(vProjectId);
            JournalLedgerDetailVM jldvm = new JournalLedgerDetailVM();

            foreach (GLAccountVM item in glavms.ToList())
            {
                jldvm = new JournalLedgerDetailVM();
                if (item.VoucherType == "GL-DV")
                {
                    jldvm.PeriodName = PeriodName;
                    jldvm.GLCode = item.GLAccountCode;
                    jldvm.AccDescription = item.Description;
                    jldvm.IsDebit = item.OutstandingLiabilities;
                    if (item.GLAccountType == "DV-OL")
                        jldvm.TransactionAmount = Dv_Dr;
                    if (item.GLAccountType == "DV-BK")
                        jldvm.TransactionAmount = Dv_Cr;

                    jldvm.TransactionDate = PostingDate;
                    jldvm.BatchDesc = "DV For the month of " + PeriodName;
                    jldvm.JournalType = "Debit Voucher";
                    jldvm.SrceType = item.VoucherType;
                    jldvm.Reference = "Shampan HRM & Payroll";
                    jldvm.IsReverse = isReverse;
                    jldvm.FiscalYearDetailId = FiscalYearDetailId;
                    jldvm.DepartmentId = vDepartmentId;
                    jldvm.SectionId = vSectionId;
                    jldvm.ProjectId = vProjectId;

                    getAllData.Add(jldvm);
                }
                else if (item.VoucherType == "GL-JV")
                {

                    jldvm.PeriodName = PeriodName;
                    jldvm.GLCode = item.GLAccountCode;
                    jldvm.AccDescription = item.Description;
                    jldvm.IsDebit = item.OutstandingLiabilities;
                    if (item.GLAccountType == "JV-OL")
                        jldvm.TransactionAmount = Jv_Dr;
                    else if (item.GLAccountType == "JV-PF")
                        jldvm.TransactionAmount = Jv_PF;
                    else if (item.GLAccountType == "JV-TAX")
                        jldvm.TransactionAmount = Jv_TAX;
                    else if (item.GLAccountType == "JV-ADV")
                        jldvm.TransactionAmount = Jv_Adv;
                    else if (item.GLAccountType == "JV-LWP")
                        jldvm.TransactionAmount = Jv_LWP;
                    else if (item.GLAccountType == "JV-TRNS")
                        jldvm.TransactionAmount = Jv_Trans;

                    jldvm.TransactionDate = PostingDate;
                    jldvm.BatchDesc = "JV For the month of " + PeriodName;
                    jldvm.JournalType = "Journal Voucher";
                    jldvm.SrceType = item.VoucherType;
                    jldvm.Reference = "Shampan HRM & Payroll";

                    jldvm.IsReverse = isReverse;
                    jldvm.FiscalYearDetailId = FiscalYearDetailId;
                    jldvm.DepartmentId = vDepartmentId;
                    jldvm.SectionId = vSectionId;
                    jldvm.ProjectId = vProjectId;
                    getAllData.Add(jldvm);

                }
            }
            try
            {
                result = _sageRepo.SageIntegration(getAllData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult _sageIndexPartial(string PostingDate, string FiscalYearDetailId = "", string DepartmentId = null, string SectionId = null, string ProjectId = null, string empcodes = null, string htmlId = "")
        {
            JournalLedgerDetailVM vm = new JournalLedgerDetailVM();
            vm.TransactionDate = PostingDate;
            vm.FiscalYearDetailId = FiscalYearDetailId;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.ProjectId = ProjectId;
            vm.empcodes = empcodes;
            vm.htmlId = htmlId;
            return PartialView("_sageIndex", vm);
        }
        public ActionResult _sageIndex(JQueryDataTableParamVM param, string TransactionDate, string FiscalYearDetailId = "", string DepartmentId = null, string SectionId = null, string ProjectId = null )
        {
             
            SageRepo _sageRepo = new SageRepo();
            List<JournalLedgerDetailVM> getAllData = new List<JournalLedgerDetailVM>();
            List<string> multiEmpIds = new List<string>();
            
            #region Value assign to Parameters
            string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0";
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
            #endregion Value assign to Parameters


            SalaryProcessRepo _salRepo = new SalaryProcessRepo();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            //call emp view model
            List<string> ProjectIdList = new List<string>();
            ds = _salRepo.SalaryPreCalculationNew(FiscalYearDetailId, vProjectId, vDepartmentId, vSectionId, "0_0", "0_0", "0_0", "", ProjectIdList);
            decimal Dv_Cr = 0;
            decimal Dv_Dr = 0;
            decimal Jv_PF = 0;
            decimal Jv_TAX = 0;
            decimal Jv_Adv = 0;
            decimal Jv_LWP = 0;
            decimal Jv_Trans = 0;
            decimal Jv_Dr = 0;

            foreach (DataRow item in ds.Tables[0].Rows)
            {
                Dv_Cr += (decimal)item["NetPayForJournalCampe"];
                Jv_PF += (decimal)item["PFForJournalCampe"];
                Jv_TAX += (decimal)item["TAXForJournalCampe"];
                Jv_Adv += (decimal)item["AdvanceDeductionForJournalCampe"];
                Jv_LWP += (decimal)item["LeaveWOPayForJournalCampe"];
                Jv_Trans += (decimal)item["TransportBillForJournalCampe"];

            }
            Jv_Dr += Jv_PF + Jv_TAX + Jv_Adv + Jv_LWP + Jv_Trans;

            Dv_Dr = Dv_Cr;

            FiscalYearRepo fyRepo = new FiscalYearRepo();

            var PeriodName=fyRepo.FYPeriodDetail(Convert.ToInt32( FiscalYearDetailId)).FirstOrDefault().PeriodName;
            GLAccountRepo glaRepo = new GLAccountRepo();
            List<GLAccountVM> glavms = new List<GLAccountVM>();
            glavms = glaRepo.SelectAll(vProjectId);
            JournalLedgerDetailVM jldvm = new JournalLedgerDetailVM();

            foreach (GLAccountVM item in glavms.ToList())
            {
                jldvm = new JournalLedgerDetailVM();
                if (item.VoucherType == "GL-DV")
                {
                        jldvm.PeriodName = PeriodName;
                        jldvm.GLCode = item.GLAccountCode;
                        jldvm.AccDescription = item.Description;
                        jldvm.IsDebit = item.OutstandingLiabilities;
                        if (item.GLAccountType == "DV-OL")
                            jldvm.TransactionAmount = Dv_Dr;
                        if (item.GLAccountType == "DV-BK")
                            jldvm.TransactionAmount = Dv_Cr;

                        jldvm.TransactionDate = TransactionDate;
                        jldvm.BatchDesc = "DV For the month of " + PeriodName;
                        jldvm.JournalType = "Debit Voucher";
                        jldvm.SrceType = item.VoucherType;
                        jldvm.Reference = "Shampan HRM & Payroll";
                        getAllData.Add(jldvm);
                }
                else if (item.VoucherType == "GL-JV")
                {
                    
                        jldvm.PeriodName = PeriodName;
                        jldvm.GLCode = item.GLAccountCode;
                        jldvm.AccDescription = item.Description;
                        jldvm.IsDebit = item.OutstandingLiabilities;
                        if (item.GLAccountType == "JV-OL")
                            jldvm.TransactionAmount = Jv_Dr;
                        else if (item.GLAccountType == "JV-PF")
                            jldvm.TransactionAmount = Jv_PF;
                        else if (item.GLAccountType == "JV-TAX")
                            jldvm.TransactionAmount = Jv_TAX;
                        else if (item.GLAccountType == "JV-ADV")
                            jldvm.TransactionAmount = Jv_Adv;
                        else if (item.GLAccountType == "JV-LWP")
                            jldvm.TransactionAmount = Jv_LWP;
                        else if (item.GLAccountType == "JV-TRNS")
                            jldvm.TransactionAmount = Jv_Trans;

                        jldvm.TransactionDate = TransactionDate;
                        jldvm.BatchDesc = "JV For the month of " + PeriodName;
                        jldvm.JournalType = "Journal Voucher";
                        jldvm.SrceType = item.VoucherType;
                        jldvm.Reference = "Shampan HRM & Payroll";

                        getAllData.Add(jldvm);
                    
                }
            }
          

                             


            //if (htmlId == "oldJournal")
            //{
            //    getAllData = _sageRepo.SelectAllJournalLedgerDetail(FiscalYearDetailId, TransactionDate);
            //}
            //else if (htmlId == "currentJournal")
            //{
            //    getAllData = _sageRepo.SageIntegrationVoucher(FiscalYearDetailId, vDepartmentId, vSectionId, vProjectId, multiEmpIds);
            //}
            IEnumerable<JournalLedgerDetailVM> filteredData;
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
                filteredData = getAllData
                    .Where(c => isSearchable0 && c.PeriodName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable1 && c.GLCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.AccDescription.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.TransactionAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.TransactionAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.TransactionDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.BatchDesc.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.JournalType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable8 && c.SrceType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable9 && c.Reference.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<JournalLedgerDetailVM, string> orderingFunction = (c => sortColumnIndex == 0 && isSortable_0 ? c.PeriodName :
                                                           sortColumnIndex == 1 && isSortable_1 ? c.GLCode :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.AccDescription :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.TransactionAmount.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.TransactionAmount.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.TransactionDate :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.BatchDesc :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.JournalType :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.SrceType :
                                                           sortColumnIndex == 9 && isSortable_9 ? c.Reference :
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
                             , c.GLCode           
                             , c.AccDescription   
                             , c.IsDebit ? c.TransactionAmount.ToString() : "0"
                             , c.IsDebit ? "0" : c.TransactionAmount.ToString() 
                             , c.TransactionDate  
                             , c.BatchDesc        
                             , c.JournalType      
                             , c.SrceType         
                             , c.Reference        
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
        public ActionResult PostedJournal(string Id = "")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_49", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            JournalLedgerDetailVM vm = new JournalLedgerDetailVM();
            SageRepo _sageRepo = new SageRepo();
            List<JournalLedgerDetailVM> getAllData = new List<JournalLedgerDetailVM>();
            getAllData = _sageRepo.SelectAllJournalLedgerDetail(null, null, Id);
            return PartialView("_postedJournal", getAllData);
        }

        public ActionResult ReportPostedJournal(string Id = "")
        {
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_49", "index").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                ReportDocument doc = new ReportDocument();
                SageRepo _sageRepo = new SageRepo();
                List<JournalLedgerDetailVM> getAllData = new List<JournalLedgerDetailVM>();
                getAllData = _sageRepo.SelectAllJournalLedgerDetail(null, null, Id);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for JournalLedger";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Journal Ledger";
                }
                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                DataSet ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtJournalLedger";
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptJournalLedger.rpt";
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
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

       
    }
}
