using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.Enum;
using SymRepository.HRM;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace SymWebUI.Areas.Payroll.Controllers
{
    public class BonusProcessController : Controller
    {
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        BonusProcessRepo _repo = new BonusProcessRepo();

        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_54", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }

        public ActionResult BonusProcess()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_54", "process").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var BSTFilter = Convert.ToString(Request["sSearch_1"]);
            var BNFilter = Convert.ToString(Request["sSearch_2"]);
            var CodeFilter = Convert.ToString(Request["sSearch_3"]);
            var EmpNameFilter = Convert.ToString(Request["sSearch_4"]);
            var DepartmentFilter = Convert.ToString(Request["sSearch_5"]);
            var SectionFilter = Convert.ToString(Request["sSearch_6"]);
            var ProjectFilter = Convert.ToString(Request["sSearch_7"]);
            var AmountFilter = Convert.ToString(Request["sSearch_8"]);
            var AmountFrom = 0;
            var AmountTo = 0;
            if (AmountFilter.Contains('~'))
            {
                AmountFrom = AmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(AmountFilter.Split('~')[0]) == true ? Convert.ToInt32(AmountFilter.Split('~')[0]) : 0;
                AmountTo = AmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(AmountFilter.Split('~')[1]) == true ? Convert.ToInt32(AmountFilter.Split('~')[1]) : 0;
            }
            #endregion Column Search
            #region Search and Filter Data
            var getAllData = _repo.SelectBonusDetailAll();
            IEnumerable<BonusProcessVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.BonusStructureName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.BonusType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable8 && c.Amount.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            #region Column Filtering
            if (BNFilter != "" || BSTFilter != "" || CodeFilter != "" || EmpNameFilter != ""
                || DepartmentFilter != "" || SectionFilter != "" || ProjectFilter != ""
                || (AmountFilter != "~" && AmountFilter != ""))
            {
                filteredData = filteredData
                                .Where(c => (BNFilter == "" || c.BonusType.ToLower().Contains(BNFilter.ToLower()))
                                            && (BSTFilter == "" || c.BonusStructureName.ToLower().Contains(BSTFilter.ToLower()))
                                            && (CodeFilter == "" || c.Code.ToLower().Contains(CodeFilter.ToLower()))
                                            && (EmpNameFilter == "" || c.EmpName.ToLower().Contains(EmpNameFilter.ToLower()))
                                            && (DepartmentFilter == "" || c.Department.ToLower().Contains(DepartmentFilter.ToLower()))
                                            && (SectionFilter == "" || c.Section.ToLower().Contains(SectionFilter.ToLower()))
                                            && (ProjectFilter == "" || c.Project.ToLower().Contains(ProjectFilter.ToLower()))
                                            && (AmountFrom == 0 || AmountFrom <= Convert.ToInt32(c.Amount))
                                            && (AmountTo == 0 || AmountTo >= Convert.ToInt32(c.Amount))
                                        );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<BonusProcessVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.BonusStructureName :
                                                                  sortColumnIndex == 2 && isSortable_2 ? c.BonusType.ToString() :
                                                                  sortColumnIndex == 3 && isSortable_3 ? c.Code.ToString() :
                                                                  sortColumnIndex == 4 && isSortable_4 ? c.EmpName.ToString() :
                                                                  sortColumnIndex == 5 && isSortable_5 ? c.Department.ToString() :
                                                                  sortColumnIndex == 6 && isSortable_6 ? c.Section.ToString() :
                                                                  sortColumnIndex == 7 && isSortable_7 ? c.Project.ToString() :
                                                                  sortColumnIndex == 8 && isSortable_8 ? c.Amount.ToString() :
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
                , c.BonusStructureName //+ "~" + Convert.ToString(c.Id)
                , c.BonusType
                , c.Code.ToString()
                , c.EmpName.ToString()
                , c.Department.ToString()
                , c.Section.ToString()
                , c.Project.ToString()
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

        public ActionResult BonusProcessCreate(BonusProcessVM vm)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_54", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            BonusProcessRepo repo = new BonusProcessRepo();
            string[] result = new string[6];
            try
            {

                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                result = repo.BonusProcess(vm);
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("Fail~Bonus Not Processed Successfully!", JsonRequestBehavior.AllowGet);
            }
        }

        #region Backup

        public ActionResult BonusProcessCreateBackup(string bNameId, string bStructureId, string pDate, string PId, string DId
            , string SId, string DesId, string CF, string CT, string fYear, string fydId)
        {
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var permission = _reposur.SymRoleSession(identity.UserId, "1_54", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            BonusProcessVM vm = new BonusProcessVM();
            BonusProcessRepo repo = new BonusProcessRepo();
            EmployeeInfoRepo repoEmp = new EmployeeInfoRepo();
            if (PId != "0_0" && PId != "0" && PId != "" && PId != "null" && PId != null)
                vm.ProjectId = PId;
            if (DId != "0_0" && DId != "0" && DId != "" && DId != "null" && DId != null)
                vm.DepartmentId = DId;
            if (SId != "0_0" && SId != "0" && SId != "" && SId != "null" && SId != null)
                vm.SectionId = SId;
            if (DesId != "0_0" && DesId != "0" && DesId != "" && DesId != "null" && DesId != null)
                vm.DesignationId = DesId;
            if (CF != "0_0" && CF != "0" && CF != "" && CF != "null" && CF != null)
                vm.CodeF = CF;
            if (CT != "0_0" && CT != "0" && CT != "" && CT != "null" && CT != null)
                vm.CodeT = CT;

            vm.FiscalYearDetailId = Convert.ToInt32(fydId);
            vm.FiscalYear = Convert.ToInt32(fYear);

            vm.BonusStructureId = bStructureId;
            vm.BonusNameId = bNameId;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            //vm.BranchId = Convert.ToInt32(identity.BranchId);
            string[] result = repo.BonusProcess(vm);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult BonusReport(string BonusNameId, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
           , string Orderby, string view, string rptPG, string Statement = null, string SheetName="")
        {
            try
            {
                #region Variables
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                Session["permission"] = permission;
                if (permission == "False")
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
                string vBonusNameId = "0_0";
                string projectParam = "[All]";
                string deptParam = "[All]";
                string secParam = "[All]";
                string desigParam = "[All]";
                string codeFParam = "[All]";
                string codeTParam = "[All]";
                string bonusParam = "[All]";
                //if (bTypeId != "0_0" && bTypeId != "0" && bTypeId != "" && bTypeId != "null" && bTypeId != null)
                //{
                //    
                //    bonusParam = bonusRepo. //SelectAll(fid).FirstOrDefault().PeriodName;
                //}
                if (BonusNameId != "0_0" && BonusNameId != "0" && BonusNameId != "" && BonusNameId != "null" && BonusNameId != null)
                {
                    vBonusNameId = BonusNameId;
                    BonusNameRepo bonusRepo = new BonusNameRepo();
                    bonusParam = bonusRepo.SelectById(BonusNameId).Name;
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
                #endregion Variables
                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();
                string ReportFileName = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                BonusProcessRepo _repo = new BonusProcessRepo();
                //var BranchId = Convert.ToInt32(identity.BranchId);
                //var getAllData = _repo.SelectAll(BranchId);
                List<BonusProcessVM> getAllData = new List<BonusProcessVM>();
                if (SheetName == "BonusSheet2")
                {
                    getAllData = _repo.BonusReportSummary(vBonusNameId, vProjectId,  vSectionId);
                }
                else
                {
                    getAllData = _repo.Report(vBonusNameId, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, Orderby);

                }
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Bonus";
                if (getAllData.Count > 0)
                {
                    if (Statement == "y")
                        ReportHead = "Bonus Statement";
                    else
                        ReportHead = "Bonus List";
                }
                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                DataSet ds = new DataSet();
                ds.Tables.Add(table);

                string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                #region EnumReport
                if (CompanyName.ToUpper() == "TIB")//tib
                {
                    //string[] conFields = { "ReportType", "ReportId" };
                    //string[] conValues = { "BonusSheet", SheetName }; ////"Salary Certificate"
                    //enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);

                    //EnumReportVM varEnumReportVM = enumReportVMs.FirstOrDefault();
                    //ReportFileName = varEnumReportVM.ReportFileName;
                    //string ReportName = varEnumReportVM.Name;
                    if (SheetName == "BonusSheet2")
                    {
                        ReportFileName = "rptFestivalBonusSummary_TIB";
                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" + ReportFileName + ".rpt";
                    }
                    else
                    {
                        ReportFileName = "rptFestivalBonus_TIB";

                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" + ReportFileName + ".rpt";

                    }
                    //ds.Tables[0].TableName = "TIBSalary";
                }
                else
                {
                    if (project.ToLower() == "brac")
                    {
                        if (Statement == "y")
                        {
                            rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptBonusStatement.rpt";
                        }
                        else
                        {
                            rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptBonusBrac.rpt";
                        }
                    }
                    else
                    {
                        if (Statement == "y")
                        {
                            rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptBonusStatement.rpt";
                        }
                        else
                        {
                            rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptBonus.rpt";
                        }

                    }

                }
                #endregion
                ds.Tables[0].TableName = "dtBonus";
               


                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                //doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = doc.DataDefinition.FormulaFields;
                if (Statement != "y")
                {
                    //doc.DataDefinition.FormulaFields["rptParamGroup"].Text = "'" + rptPG + "'";
                    FormulaField(doc, crFormulaF, "rptParamGroup", rptPG);

                }
                //doc.DataDefinition.FormulaFields["bonusParam"].Text = "'" + bonusParam + "'";
                //doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                //doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                //doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                //doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                //doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                //doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                FormulaField(doc, crFormulaF, "bonusParam", bonusParam);
                FormulaField(doc, crFormulaF, "projectParam", projectParam);
                FormulaField(doc, crFormulaF, "deptParam", deptParam);
                FormulaField(doc, crFormulaF, "secParam", secParam);
                FormulaField(doc, crFormulaF, "desigParam", desigParam);
                FormulaField(doc, crFormulaF, "codeFParam", codeFParam);
                FormulaField(doc, crFormulaF, "codeTParam", codeTParam);
                FormulaField(doc, crFormulaF, "ReportHead", ReportHead);

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception ex)
            {
                FileLogger.Log("Bonus Report", this.GetType().Name, ex.Message);
                return RedirectToAction("BonusProcess");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ExcelReport(BonusProcessVM vm)
        {
            DataTable dt = new DataTable();
            ResultVM rVM = new ResultVM();
            BonusProcessRepo _repo = new BonusProcessRepo();

            List<BonusProcessVM> getAllData = new List<BonusProcessVM>();
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
             
            string[] result = new string[6];
            try
            {
                #region Parmeters
                if (!string.IsNullOrWhiteSpace(vm.MultipleOther3))
                {
                    vm.MultipleOther3 = vm.MultipleOther3.Trim(',');
                    vm.Other3List = vm.MultipleOther3.Split(',').ToList();
                }
                #endregion

                string[] conFields = { "sbd.BonusNameId"};
                string[] conValues = { vm.BonusNameId};

                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name; 
                string
                    Line2 = comInfo
                        .Address; 
                string Line3 = "Statement of Staff Earned Leave";

                string filename = "";
                if (vm.SheetName == "BonusSheet3" || vm.SheetName == "BonusSheet4" || vm.SheetName == "SalarySheet14")
                {
                    if (string.IsNullOrWhiteSpace(vm.BonusNameId))
                        vm.BonusNameId = "0_0";

                    if (string.IsNullOrWhiteSpace(vm.ProjectId))
                        vm.ProjectId = "0_0";

                    if (string.IsNullOrWhiteSpace(vm.DepartmentId ))
                      vm.DepartmentId = "0_0";

                    if (string.IsNullOrWhiteSpace(vm.SectionId ))
                        vm.SectionId = "0_0";

                    if (string.IsNullOrWhiteSpace(vm.DesignationId ))
                        vm.DesignationId = "0_0";

                    if (string.IsNullOrWhiteSpace(vm.CodeF ))
                        vm.CodeF = "0_0";

                    if (string.IsNullOrWhiteSpace(vm.CodeT))
                       vm.CodeT = "0_0";

                 
                    getAllData = _repo.Report(vm.BonusNameId, vm.ProjectId, vm.DepartmentId, vm.SectionId, vm.DesignationId, vm.CodeF, vm.CodeT, vm.Orderby, vm.SheetName);
                    DataTable table = new DataTable();
                    //DataTable dt = new DataTable();
                    table = Ordinary.ListToDataTable(getAllData.ToList());
                    table = Ordinary.DtColumnNameChange(table, "NetAmount", "BonusAmount");
                    var dataView = new DataView(table);
                    dt = table.Copy();

                  

                    if (vm.SheetName == "BonusSheet3")
                    {
                        string[] DecimalColumnNames = { "BonusAmount" };

                        // dt = Ordinary.DtSetColumnsOrder(dt, ShortColumnNames);
                        dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);
                        string DebitAccountNo = new SettingRepo().settingValue("Salary", "DebitA/CNo");

                        dt.Columns.Add(new DataColumn("Reason") { DefaultValue = "Bonus for " + vm.BounsName});
                        dt.Columns.Add(new DataColumn("PayeeAccType (Add1)") { DefaultValue = "Savings" });
                        dt.Columns.Add(new DataColumn("Debit A/C No.") { DefaultValue = DebitAccountNo });
                        dt.Columns.Add(new DataColumn("Payment Date") { DefaultValue = vm.PaymentDay });

                        dt = Ordinary.DtColumnNameChange(dt, "Code", "Customer Reference (16)");
                        dt = Ordinary.DtColumnNameChange(dt, "EmpName", "Payee Name");
                        dt = Ordinary.DtColumnNameChange(dt, "BankAccountNo", "Payee Bank Acc No");
                        dt = Ordinary.DtColumnNameChange(dt, "Routing_No", "PayeeBankRouting");
                        dt = Ordinary.DtColumnNameChange(dt, "ELAmount", "Amount");
                        dataView = new DataView(dt);
                        dt = dataView.ToTable(true, "Customer Reference (16)", "Payee Name", "Payee Bank Acc No",
                            "PayeeAccType (Add1)", "PayeeBankRouting"
                            , "Reason", "BonusAmount", "Payment Date", "Debit A/C No.", "Email");


                        Line3 = "Bonus BFTN: SB and Others ";
                        filename = "Bonus BFTN: SB and Others " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmm");
                    }

                    else if (vm.SheetName == "BonusSheet4")
                    {
                        string DebitAccountNo = new SettingRepo().settingValue("Salary", "DebitA/CNo");

                        string[] DecimalColumnNames = { "BonusAmount" };
                        dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);

                        dt.Columns.Add(new DataColumn("Reason") { DefaultValue = "Bonus for " + vm.BounsName });
                        dt.Columns.Add(new DataColumn("PayeeAccType (Add1)") { DefaultValue = "Savings" });
                        dt.Columns.Add(new DataColumn("Debit A/C No.") { DefaultValue = DebitAccountNo });
                        dt.Columns.Add(new DataColumn("Payment Date") { DefaultValue = vm.PaymentDay });
                        //dt = Ordinary.DtColumnNameChange(dt, "ELAmount", "Amount");

                        dt = Ordinary.DtColumnNameChange(dt, "Code", "Customer Reference (16)");
                        dt = Ordinary.DtColumnNameChange(dt, "EmpName", "Payee Name");
                        dt = Ordinary.DtColumnNameChange(dt, "BankAccountNo", "Payee Bank Acc No");
                        dataView = new DataView(dt);


                        dt = dataView.ToTable(true, "Customer Reference (16)", "Payee Name", "Payee Bank Acc No"
                            , "Reason", "BonusAmount", "Payment Date", "Debit A/C No.", "Email");

                        Line3 = "Bonus BFTN-SCB";
                        filename = "Bonus BFTN-SCB " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmm");
                    }

                    else if (vm.SheetName == "SalarySheet14")
                    {
                        dataView = new DataView(dt);
                        dt = dataView.ToTable(true, "EmpName", "Designation", "Code","Grade", "StepName", "BasicSalary", "HouseRent", "Medical"
                            , "TransportAllowance", "GrossSalary", "BonusAmount", "Stamp");
                        dt = Ordinary.DtColumnNameChange(dt, "Code", "EIN");
                        Line3 = "Festival Bonus ";
                        filename = "Festival Bonus " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmm");
                    }

                    string[] ReportHeaders = new string[] { Line1, Line2, Line3 };

                    ExcelSheetFormat(dt, workSheet, ReportHeaders);
                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                        excel.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
                else if (vm.SheetName == "BonusSheet2")
                {

                    if (string.IsNullOrWhiteSpace(vm.ProjectId))
                        vm.ProjectId = "0_0";

                    if (string.IsNullOrWhiteSpace(vm.SectionId))
                        vm.SectionId = "0_0";
                    getAllData = _repo.BonusReportSummary(vm.BonusNameId, vm.ProjectId, vm.SectionId);
                    DataTable table = new DataTable();
                    //DataTable dt = new DataTable();
                    table = Ordinary.ListToDataTable(getAllData.ToList());
                    table = Ordinary.DtColumnNameChange(table, "NetPayAmount", "BonusAmount");

                    var dataView = new DataView(table);
                    dt = table.Copy();
                    dataView = new DataView(dt);


                    dt = dataView.ToTable(true, "Designation", "Section", "Project"
                        , "BasicSalary", "GrossSalary", "HouseRent", "Medical", "TransportAllowance", "BonusAmount", "Stamp", "NetAmount");
                    Line3 = "Bonus Summary (Designation)";
                    filename = "Bonus Summary (Designation) " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmm");
                    string[] ReportHeaders = new string[] { Line1, Line2, Line3 };

                    ExcelSheetFormat(dt, workSheet, ReportHeaders);
                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                        excel.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
                else 
                {

                    rVM = _repo.DownloadExcel(vm, conFields, conValues);
                    if (rVM.Status == "Fail")
                    {
                        Session["result"] = rVM.Message;
                        return View("BonusReport");
                    }

                    filename = rVM.ReportName.Replace(",", " ");

                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                        rVM.excel.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }

                #region Unused

                //dt = _repo.ExcelReport(vm);

                //if (dt.Rows.Count == 0)
                //{
                //    result[0] = "Fail";
                //    result[1] = "No Data Found";
                //    Session["result"] = result[0] + "~" + result[1];

                //    return View("BonusReport");
                //}

                //string ReportName = dt.Rows[0]["BonusType"].ToString();

                //string[] removeColumnName = { "BonusType" };
                //dt = Ordinary.DtDeleteColumns(dt, removeColumnName);

                //string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();


                //ExcelPackage excel = new ExcelPackage();
                //var workSheet = excel.Workbook.Worksheets.Add("Sheet1");


                //if ((CompanyName.ToLower() == "kbl" || CompanyName.ToLower() == "anupam" || CompanyName.ToLower() == "kajol"))
                //{
                    #region Report Headers, Rows, Columns

                    //string Line1 = "KAZAL BROTHERS LIMITED";
                    //string Line2 = "CORPORATE OFFICE: Dr. Nawab Ali Tower, 6th floor, 24 Purana Paltan, Dhaka-1000";
                    //string Line3 = "Phone - 9515301, 9515302, Fax - 9515303 web:www.anupameducation.com";


                    //if (CompanyName.ToLower() == "anupam")
                    //{
                    //    Line1 = "ANUPAM PRINTERS";
                    //    Line2 = "Matuail Moghalnagar, Kadamtali Culvert Road, Matuail-1362, Demra, Dhaka";
                    //    Line3 = "Contact: 01718-298115; 01991-144534";
                    //}

                    //string[] conFields = { "ReportType", "ReportId" };
                    //string[] conValues = { "BonusSheet", vm.SheetName };
                    //List<EnumReportVM> enumReportVMs = new EnumReportRepo().SelectAll(0, conFields, conValues);

                    //string Title = enumReportVMs.FirstOrDefault().ReportName + " - " + ReportName;


                    //int LeftColumn = 8;

                    //if (vm.SheetName == "BonusSheet2")
                    //{
                    //    LeftColumn = 2;
                    //}


                    //string[] ReportHeaders = new string[] { "", Line1, Line2, Line3, Title };

                    //int TableHeadRow = 0;
                    //TableHeadRow = ReportHeaders.Length + 2;

                    //int RowCount = 0;
                    //RowCount = dt.Rows.Count;

                    //int ColumnCount = 0;
                    //ColumnCount = dt.Columns.Count;

                    //int GrandTotalRow = 0;
                    //GrandTotalRow = TableHeadRow + RowCount + 1;

                    //int InWordsRow = 0;
                    //InWordsRow = GrandTotalRow + 2;

                    //int SignatureSpaceRow = 0;
                    //SignatureSpaceRow = InWordsRow + 1;

                    //int SignatureRow = 0;
                    //SignatureRow = InWordsRow + 4;
                    #endregion

                    //workSheet.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                    ////////if (vm.SheetName == "BonusSheet1")
                    ////////{
                    #region Format
                    //workSheet.Cells["B" + (TableHeadRow + 1) + ":" + Ordinary.Alphabet[(ColumnCount + 1)] + (TableHeadRow + 1 + RowCount + 3)].Style.Numberformat.Format = "#,##0";
                    //workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount + 1)] + (TableHeadRow)].Style.Font.Bold = true;
                    //workSheet.Cells["A" + (RowCount + TableHeadRow + 1) + ":" + Ordinary.Alphabet[(ColumnCount + 1)] + (RowCount + TableHeadRow + 1)].Style.Font.Bold = true;
                    //workSheet.Cells[Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (RowCount + TableHeadRow + 1)].Style.Font.Bold = true;

                    //workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    //workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    //var format = new OfficeOpenXml.ExcelTextFormat();
                    //format.Delimiter = '~';
                    //format.TextQualifier = '"';
                    //format.DataTypes = new[] { eDataTypes.String };


                    //for (int i = 0; i < ReportHeaders.Length; i++)
                    //{
                    //    workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Merge = true;
                    //    workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    //    workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.Font.Bold = true;
                    //    workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.Font.Size = 14 - i;
                    //    workSheet.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

                    //}

                    //workSheet.Row(TableHeadRow).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //workSheet.Row(TableHeadRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //workSheet.Row(TableHeadRow).Style.WrapText = true;

                    //workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

                    #region Grand Total

                    //for (int i = LeftColumn + 1; i <= ColumnCount; i++)
                    //{
                    //    workSheet.Cells[GrandTotalRow, i].Formula = "=Sum(" + workSheet.Cells[TableHeadRow + 1, i].Address + ":" + workSheet.Cells[(TableHeadRow + RowCount), i].Address + ")";
                    //}

                    #region Total Not Required

                    //int ColumnIndex = 0;

                    //DataColumnCollection columns = dt.Columns;
                    //if (columns.Contains("BonusValue"))
                    //{
                    //    ColumnIndex = dt.Columns["BonusValue"].Ordinal + 1;

                    //    workSheet.Cells[GrandTotalRow, ColumnIndex].Value = "";

                    //}



                    #endregion


                    //object sumObject;
                    //sumObject = dt.Compute("Sum([NetPayAmount])", string.Empty);

                    //decimal NetPayable = Convert.ToDecimal(sumObject);

                    //string strNetPayable = NetPayable.ToString("0.##");

                    //string NetPayableInWords = Ordinary.ConvertToWords(strNetPayable, true);

                    //workSheet.Row(InWordsRow).Style.WrapText = true;
                    //workSheet.Row(InWordsRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    //workSheet.Cells[InWordsRow, 2, InWordsRow, ColumnCount].Merge = true;
                    //workSheet.Cells[InWordsRow, 2, InWordsRow, ColumnCount].Style.Font.Bold = true;
                    //workSheet.Cells[InWordsRow, 1].LoadFromText("Net Payable (In Words):");
                    //workSheet.Cells[InWordsRow, 2].LoadFromText(NetPayableInWords);


                    //workSheet.Cells[TableHeadRow, 1, GrandTotalRow - 1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    #endregion

                    //////workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //if (vm.SheetName == "BonusSheet1")
                    //{

                        #region Signature

                        //string signatory1Title = "Prepared By";
                        //string signatory2Title = "Audited By";
                        //string signatory3Title = "Checked By";
                        //string signatory4Title = "Authorized By";
                        //string signatory5Title = "Approved By";



                        //int signatory1Column = 1;
                        //int signatory2Column = 3;
                        //int signatory3Column = 5;
                        //int signatory4Column = 7;
                        //int signatory5Column = 9;


                        //workSheet.Cells[SignatureRow, signatory1Column].LoadFromText(signatory1Title);
                        //workSheet.Cells[SignatureRow, signatory2Column].LoadFromText(signatory2Title);
                        //workSheet.Cells[SignatureRow, signatory3Column].LoadFromText(signatory3Title);
                        //workSheet.Cells[SignatureRow, signatory4Column].LoadFromText(signatory4Title);
                        //workSheet.Cells[SignatureRow, signatory5Column].LoadFromText(signatory5Title);


                        //workSheet.Cells[SignatureRow, signatory1Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        //workSheet.Cells[SignatureRow, signatory2Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        //workSheet.Cells[SignatureRow, signatory3Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        //workSheet.Cells[SignatureRow, signatory4Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        //workSheet.Cells[SignatureRow, signatory5Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;


                        //workSheet.Row(SignatureRow).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        //workSheet.Row(SignatureRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        //workSheet.Row(SignatureRow).Style.WrapText = true;
                        //workSheet.Row(SignatureRow).Style.Font.Bold = true;

                        #endregion
                    //}
                    #endregion
                    //////}


                //}
                //else
                //{


                //    workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
                //}


                #endregion

                result[0] = "Successfull";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("BonusProcess");
            }
            catch (Exception ex)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("BonusProcess");
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

        [Authorize(Roles = "Admin")]
        public ActionResult DownloadExcel(BonusProcessVM vm)
        {
            DataTable dt = new DataTable();
            string[] result = new string[6];
            try
            {

                dt = _repo.ExcelData(vm);

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);


                string filename = "DownloadBonus";


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
                return RedirectToAction("BonusProcess");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("BonusProcess");
            }
        }

        public ActionResult UploadExcel(BonusProcessVM vm)
        {
            string[] result = new string[6];
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                result = _repo.ImportExcelFile(vm);
                Session["result"] = result[0] + "~" + result[1];
                return View("BonusProcess", vm);
                ////return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("BonusProcess", vm);

                ////return RedirectToAction("Index");
            }
        }

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }

        public ActionResult _rptIndexPartial(string BonusNameId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
        , string CodeF, string CodeT, string Orderby = null)
        {
            BonusProcessVM vm = new BonusProcessVM();
            vm.ProjectId = ProjectId;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.DesignationId = DesignationId;
            vm.CodeF = CodeF;
            vm.CodeT = CodeT;
            vm.BonusNameId = BonusNameId;
            vm.Orderby = Orderby;
            return PartialView("_rptIndex", vm);
        }

        public ActionResult _rptIndex(JQueryDataTableParamVM param, string BonusNameId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, string Orderby = null)
        {
            #region Declare Variable
            string vProjectId = "0_0";
            string vDepartmentId = "0_0";
            string vSectionId = "0_0";
            string vDesignationId = "0_0";
            string vCodeF = "0_0";
            string vCodeT = "0_0";
            string vBonusNameId = "0_0";
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (!(identity.IsAdmin || identity.IsPayroll))
            {
                //Id = identity.EmployeeId;
                vCodeF = identity.EmployeeCode;
                vCodeT = identity.EmployeeCode;
            }
            else
            {
                if (BonusNameId != "0_0" && BonusNameId != "0" && BonusNameId != "" && BonusNameId != "null" && BonusNameId != null)
                {
                    vBonusNameId = BonusNameId;
                }
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
            BonusProcessRepo _repo = new BonusProcessRepo();
            List<BonusProcessVM> getAllData = new List<BonusProcessVM>();// 
            getAllData = _repo.Report(vBonusNameId, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, Orderby);
            IEnumerable<BonusProcessVM> filteredData;
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
                    .Where(c => isSearchable1 && c.BonusType.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable8 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable9 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<BonusProcessVM, string> orderingFunction = (c => sortColumnIndex == 0 && isSortable_0 ? c.BonusType :
                                                           sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Department :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Section :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Project :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.BasicSalary.ToString() :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.GrossSalary.ToString() :
                                                           sortColumnIndex == 9 && isSortable_9 ? c.Amount.ToString() :
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
                             c.BonusType
                             , c.Code
                             , c.EmpName
                             , c.Designation
                             , c.Department
                             , c.Section
                             , c.Project
                             , c.BasicSalary.ToString()
                             , c.GrossSalary.ToString()
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

        public void FormulaField(ReportDocument objrpt, FormulaFieldDefinitions crFormulaF, string fieldName, string fieldValue)
        {
            try
            {
                FormulaFieldDefinition fs;
                fs = crFormulaF[fieldName];
                objrpt.DataDefinition.FormulaFields[fieldName].Text = "'" + fieldValue + "'";
            }
            catch (Exception ex)
            {


            }


        }
    }
}
