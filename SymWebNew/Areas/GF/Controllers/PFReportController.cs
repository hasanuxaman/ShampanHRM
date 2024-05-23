using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.PF;
using SymRepository.Common;
using SymViewModel.PF;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Data;
using SymReporting.PF;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymWebUI.Areas.PF.Models;
using Newtonsoft.Json;

namespace SymWebUI.Areas.GF.Controllers
{
    public class PFReportController : Controller
    {
        public PFReportController()
        {
            ViewBag.TransType = AreaTypeGFVM.TransType;
        }
        //
        // GET: /PF/PFReport/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        public ActionResult Index()
        {
            return View("~/Areas/PF/Views/PFReport/Index.cshtml");

        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult PFBankStatementReport()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "report").ToString();
            return View("~/Areas/PF/Views/PFReport/PFBankStatementReport.cshtml");

        }
        [HttpGet]
        public ActionResult PFBankStatementReportView(string dtFrom = "", string dtTo = "", string bankBranchId = "")
        {
            try
            {
                //PFBankDeposit
                //Withdraw
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dtPFBankDeposit = new DataTable();
                DataTable dtWithdraw = new DataTable();
                DataSet ds = new DataSet();

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                PFBankDepositVM pfbdVM = new PFBankDepositVM();
                PFBankDepositRepo _pfbdRepo = new PFBankDepositRepo();

                if (string.IsNullOrWhiteSpace(dtFrom))
                {
                    dtFrom = DateTime.MinValue.ToString();
                }

                if (string.IsNullOrWhiteSpace(dtTo))
                {
                    dtTo = DateTime.MaxValue.ToString();
                }

                pfbdVM.DateFrom = dtFrom;
                pfbdVM.DateTo = dtTo;
                pfbdVM.BankBranchId = Convert.ToInt32(bankBranchId);
                pfbdVM.TransType = AreaTypeGFVM.TransType;

                dtPFBankDeposit = _pfbdRepo.PFBankStatementReport(pfbdVM);

                ReportHead = "There are no data to Preview for PF Bank Statement";
                if (dtPFBankDeposit.Rows.Count > 0)
                {
                    ReportHead = AreaTypeGFVM.TransType+" Bank Statement";
                }

                ds.Tables.Add(dtPFBankDeposit);
                ds.Tables[0].TableName = "dtPFBankStatement";


                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptPFBankStatement.rpt";

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


        public ActionResult AccountStatement()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            return View("~/Areas/PF/Views/PFReport/AccountStatement.cshtml");

        }

        //public ActionResult AccountStatementReport(PFParameterVM paramVM)
        //{
        //    try
        //    {

        //        PFReportVM vm = new PFReportVM();
        //        PFReport rpt = new PFReport();
        //        vm.TransType = AreaTypeGFVM.TransType;
        //        vm = rpt.AccountStatement(paramVM);

        //        return File(vm.MemStream, "application/PDF");
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        //public ActionResult AccountLedgerReport(PFParameterVM paramVM)
        //{
        //    try
        //    {

        //        PFReportVM vm = new PFReportVM();
        //        PFReport rpt = new PFReport();
        //        vm.TransType = AreaTypeGFVM.TransType;
        //        vm = rpt.AccountLedgerReport(paramVM);

        //        return File(vm.MemStream, "application/PDF");
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        public ActionResult EmployeePFStatement()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            return View("~/Areas/PF/Views/PFReport/EmployeePFStatement.cshtml");

        }

        public ActionResult EmployeePFStatements(string rType, string EmployeeId,string ToDate,string FromDate)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                PFReport rpt = new PFReport();

                vm = rpt.EmployeePFStatements(rType,  EmployeeId, ToDate,FromDate);

                return File(vm.MemStream, "application/PDF");
            }
            catch (Exception)
            {
                throw;
            }
        }



        #region Excel Reports

        public ActionResult InvestmentStatement(PFParameterVM paramVM)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                PFExcel xcl = new PFExcel();
                vm.TransType = AreaTypeGFVM.TransType;
                vm = xcl.InvestmentStatement(paramVM);

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=" + vm.FileName + ".xlsx");
                vm.MemStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();

                return Redirect("InvestmentStatement");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Report(PFReportVM vm)
        {

            string[] result = new string[6];
            DataTable dt = new DataTable();
            try
            {

                return null;

            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return Redirect("Report");
            }
        }

        #endregion

        #region Report View

        [HttpGet]
        public ActionResult BankChargeReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/BankChargeReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeeBreakMonthPFReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/EmployeeBreakMonthPFReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeePFPaymentReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/EmployeePFPaymentReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeeForfeitureReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/EmployeeForfeitureReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeePFOpeinigReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/EmployeePFOpeinigReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeeReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/EmployeeReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult LoanReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/LoanReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult LoanMonthlyPaymentReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/LoanMonthlyPaymentReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult LoanRepaymentToBankReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/LoanRepaymentToBankReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult LoanSattlementReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/LoanSattlementReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult PFBankDepositsReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/PFBankDepositsReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult PFContributionReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/PFContributionReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult ProfitDistributionNewReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/ProfitDistributionNewReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult ReturnOnBankInterestsReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/ReturnOnBankInterestsReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult WithdrawsReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/WithdrawsReportVeiw.cshtml", vm);


            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Voucher_StatementReportVeiw(string JournalType="1")
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.JournalType = JournalType;
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/PF/Views/PFReport/Voucher_StatementReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeeLedgerReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/GF/Views/PFReport/EmployeeLedgerReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion

        #region PF Report (25 Dec 2021)

        //public PFReportVM VAT9_1_SubForm_Download(PFReportVM vm, DataTable dt, string[] ReportHeaders)
        //{
        //    ReportDocument objrpt = new ReportDocument();
        //    try
        //    {
        //        //////Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();


        //        //vm = new _9_1_VATReturnDAL().VAT9_1_SubForm_Download(vm);

        //        //DataRow dr = vm.dtComapnyProfile.Rows[0];

        //        //string ComapnyName = dr["CompanyLegalName"].ToString();
        //        //string VatRegistrationNo = dr["VatRegistrationNo"].ToString();
        //        //string Address1 = dr["Address1"].ToString();

        //        //string[] ReportHeaders = new string[] { ComapnyName, VatRegistrationNo, Address1, "Sub-form (For Note - " + vm.NoteNo + ")", "Type: " + vm.Type };

        //        //DataTable dt = vm.dtVATReturnSubForm;

        //        if (dt == null || dt.Rows.Count == 0)
        //        {
        //            return vm;
        //        }

        //        #region Column Name Change


        //        dt = Ordinary.DtSlColumnAdd(dt);

        //        string[] DtcolumnName = new string[dt.Columns.Count];
        //        int j = 0;
        //        foreach (DataColumn column in dt.Columns)
        //        {
        //            DtcolumnName[j] = column.ColumnName;
        //            j++;
        //        }

        //        for (int k = 0; k < DtcolumnName.Length; k++)
        //        {
        //            dt = Ordinary.DtColumnNameChange(dt, DtcolumnName[k], Ordinary.AddSpacesToSentence(DtcolumnName[k]));
        //        }

        //        string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
        //        string fileDirectory = pathRoot + "//Excel Files";
        //        Directory.CreateDirectory(fileDirectory);

        //        vm.FileName = vm.FileName + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

        //        fileDirectory += "\\" + vm.FileName + ".xlsx";
        //        FileStream objFileStrm = File.Create(fileDirectory);

        //        int TableHeadRow = 0;
        //        TableHeadRow = ReportHeaders.Length + 2;

        //        int RowCount = 0;
        //        RowCount = dt.Rows.Count;

        //        int ColumnCount = 0;
        //        ColumnCount = dt.Columns.Count;

        //        int GrandTotalRow = 0;
        //        GrandTotalRow = TableHeadRow + RowCount + 1;

        //        if (string.IsNullOrEmpty(vm.SheetName))
        //        {
        //            vm.SheetName = "Demo";
        //        }

        //        #endregion

        //        ExcelPackage package = new ExcelPackage(objFileStrm);

        //        ExcelWorksheet ws = package.Workbook.Worksheets.Add("Sub-form (For Note - " + vm.NoteNo + ")");

        //        ////ws.Cells["A1"].LoadFromDataTable(dt, true);
        //        ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

        //        #region Format

        //        var format = new OfficeOpenXml.ExcelTextFormat();
        //        format.Delimiter = '~';
        //        format.TextQualifier = '"';
        //        format.DataTypes = new[] { eDataTypes.String };



        //        for (int i = 0; i < ReportHeaders.Length; i++)
        //        {
        //            ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
        //            ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
        //            ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

        //        }
        //        int colNumber = 0;

        //        foreach (DataColumn col in dt.Columns)
        //        {
        //            colNumber++;
        //            if (col.DataType == typeof(DateTime))
        //            {
        //                ws.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
        //            }
        //            else if (col.DataType == typeof(Decimal))
        //            {

        //                ws.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

        //                #region Grand Total
        //                ws.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + ws.Cells[TableHeadRow + 1, colNumber].Address + ":" + ws.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
        //                #endregion
        //            }

        //        }

        //        ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
        //        ws.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

        //        ws.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //        ws.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

        //        ws.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");
        //        #endregion


        //        vm.varFileDirectory = fileDirectory;
        //        vm.varExcelPackage = package;
        //        vm.varFileStream = objFileStrm;
        //        ////package.Save();
        //        ////objFileStrm.Close();

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    finally { }
        //    return vm;
        //}

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
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
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
                    workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + workSheet.Cells[TableHeadRow + 1, colNumber].Address + ":" + workSheet.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                    #endregion
                }

            }

            workSheet.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

            workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");
            #endregion
        }

        [HttpPost]
        public ActionResult BankCharge_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "bd.TransactionDate>", "bd.TransactionDate<", "bd.TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypeGFVM.TransType };

                DataTable dt = _repo.BankCharge_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypeGFVM.TransType+" BankCharge_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + " Bank Charge Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + " Bank Charge Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("PF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult EmployeeBreakMonthPF_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();


                string[] conditionFields = { "e.EmployeeId" };
                string[] conditionValues = { vm.EmployeeId };

                DataTable dt = _repo.EmployeeBreakMonthGF_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = "EmployeeBreakMonth"+AreaTypeGFVM.TransType+"_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("EmployeeBreakMonth" + AreaTypeGFVM.TransType + "_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { "Employee Break Month "+ AreaTypeGFVM.TransType +" Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("GF/GFEmployeeProvision/IndexFiscalPeriod");

                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult EmployeePaymentPF_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();


                string[] conditionFields = { "e.EmployeeId" };
                string[] conditionValues = { vm.EmployeeId };

                DataTable dt = _repo.EmployeePaymentGF_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = "EmployeePayment" + AreaTypeGFVM.TransType + "_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("EmployeePayment" + AreaTypeGFVM.TransType + "_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { "Employee Payment " + AreaTypeGFVM.TransType + " Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("PF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult EmployeeForfeiture_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();


                string[] conditionFields = { "e.EmployeeId" };
                string[] conditionValues = { vm.EmployeeId };

                DataTable dt = _repo.GFEmployeeForfeiture_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = " EmployeeForfeiture_GF_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(" EmployeeForfeitureGF_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { " Employee Forfeiture GF Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("GF/GFEmployeeProvision/IndexFiscalPeriod");

                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult EmployeePFOpeinig_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "e.EmployeeId" };
                string[] conditionValues = { vm.EmployeeId };

                DataTable dt = _repo.EmployeeGFOpeinig_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = "EmployeeGFOpeinig_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("EmployeeGFOpeinig_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { "Employee GF Opeinig Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("GF/GFEmployeeProvision/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        //[HttpPost]
        //public ActionResult EmployeeStatement(PFReportVM vm)
        //{
        //    try
        //    {

        //        PFReportRepo _repo = new PFReportRepo();

        //        string[] conditionFields = { "e.EmployeeId" };
        //        string[] conditionValues = { vm.EmployeeId };

        //        DataTable dt = _repo.GFEmployeeStatement(conditionFields, conditionValues);

        //        #region Excel

        //        string filename = AreaTypeGFVM.TransType + " EmployeeStatement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
        //        ExcelPackage excel = new ExcelPackage();
        //        var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + " EmployeeStatement");
        //        CompanyRepo cRepo = new CompanyRepo();
        //        CompanyVM comInfo = cRepo.SelectById(1);
        //        string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
        //        string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
        //        string Line3 = "";

        //        int LeftColumn = 5;
        //        int CenterColumn = 5;

        //        string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + " Employee Statement", Line1, Line2, Line3 };

        //        ExcelSheetFormat(dt, workSheet, ReportHeaders);
        //        #region Excel Download

        //        using (var memoryStream = new MemoryStream())
        //        {
        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
        //            excel.SaveAs(memoryStream);
        //            memoryStream.WriteTo(Response.OutputStream);
        //            Response.Flush();
        //            Response.End();
        //        }
        //        #endregion
        //        return Redirect("GF/GFEmployeeProvision/IndexFiscalPeriod");
        //        #endregion

        //        //return File(vm.MemStream, "application/PDF");
        //    }

        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        [HttpPost]
        public ActionResult EmployeeStatement(PFReportVM vm)
        {
            try
            {
                string rptLocation = "";
                string ReportHead = "";

                PFReportRepo _repo = new PFReportRepo();
                CompanyRepo _CompanyRepo = new CompanyRepo();

                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();
                string[] conditionFields = { "e.EmployeeId","pf.TransactionDate>", "pf.TransactionDate<" };
                string[] conditionValues = { vm.EmployeeId,Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo) };

                DataTable dt = _repo.GFEmployeeStatement(conditionFields, conditionValues);
                if (vm.ReportType == "Excel")
                {
                    #region Excel

                    string filename = AreaTypeGFVM.TransType + "EmployeeStatement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + "EmployeeStatement");
                    CompanyRepo cRepo = new CompanyRepo();
                    CompanyVM comInfo = cRepo.SelectById(1);
                    string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                    string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                    string Line3 = "";

                    int LeftColumn = 5;
                    int CenterColumn = 5;

                    string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + "Employee Statement", Line1, Line2, Line3 };

                    ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                    return Redirect("GF/GFEmployeeProvision/IndexFiscalPeriod");

                    #endregion
                }
                else
                {
                    ReportDocument doc = new ReportDocument();
                    dt.TableName = "dtEmployeeStatement";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\GF\\rptEmployeeStatementDetails.rpt";

                    doc.Load(rptLocation);
                    doc.SetDataSource(dt);
                    string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                    FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                    doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                    doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                    doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypeGFVM.TransType + "'";
                    doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";

                    //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                    var rpt = RenderReportAsPDF(doc);
                    doc.Close();
                    return rpt;
                }
                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Loan_Statement(PFReportVM vm)
        {
            try
            {

                vm.TransType = AreaTypeGFVM.TransType;
                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "TransactionDate>", "TransactionDate<", "TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypeGFVM.TransType };

                DataTable dt = _repo.Loan_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypeGFVM.TransType + " Loan_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + " Loan_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + " Loan Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("PF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult LoanMonthlyPayment_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "TransactionDate>", "TransactionDate<", "TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo) , AreaTypeGFVM.TransType};

                DataTable dt = _repo.LoanMonthlyPayment_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypeGFVM.TransType + "LoanMonthlyPayment_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + " LoanMonthlyPayment_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + " Loan Monthly Payment Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("PF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult LoanRepaymentToBank_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();
                string[] conditionFields = { "TransactionDate>", "TransactionDate<", "TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypeGFVM.TransType };

                DataTable dt = _repo.LoanRepaymentToBank_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypeGFVM.TransType + " LoanRepaymentToBank_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + " LoanRepaymentToBank_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + " Loan Repayment To Bank Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("PF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult LoanSattlement_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "TransactionDate>", "TransactionDate<" ,"TransType"};
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypeGFVM.TransType };

                DataTable dt = _repo.LoanSattlement_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypeGFVM.TransType + " LoanSattlement_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + " LoanSattlement_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + " Loan Sattlement Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("PF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult PFBankDeposits_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();


                string[] conditionFields = { "bd.DepositDate>", "bd.DepositDate<", "bd.TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypeGFVM.TransType };

                DataTable dt = _repo.PFBankDeposits_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypeGFVM.TransType + " BankDeposits_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + " BankDeposits_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + " Bank Deposits Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("PF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult PFContribution_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "e.EmployeeId" };
                string[] conditionValues = { vm.EmployeeId};

                DataTable dt = _repo.GFContribution_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypeGFVM.TransType + " Contribution_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + " Contribution_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + " Contribution Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("GF/GFEmployeeProvision/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ProfitDistributionNew_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "pd.DistributionDate>", "pd.DistributionDate<", "pd.TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypeGFVM.TransType };

                DataTable dt = _repo.ProfitDistributionNew_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypeGFVM.TransType + " ProfitDistributionNew_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + " ProfitDistributionNew_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + " Profit Distribution Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("PF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ReturnOnBankInterests_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "bd.TransactionDate>", "bd.TransactionDate<", "bd.TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypeGFVM.TransType };

                DataTable dt = _repo.ReturnOnBankInterests_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypeGFVM.TransType + " ReturnOnBankInterests_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + " ReturnOnBankInterests_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + " Return On Bank Interests Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("PF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Withdraws_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "bd.WithdrawDate>", "bd.WithdrawDate<", "bd.TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypeGFVM.TransType };

                DataTable dt = _repo.Withdraws_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypeGFVM.TransType + " Withdraws_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + " Withdraws_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + " Withdraws Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("PF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Voucher_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = new string[] { "jd.JournalType", "jd.TransType"  ,"jd.COAId"};
                string[] conditionValues = new string[] { vm.JournalType, AreaTypeGFVM.TransType, vm.Id.ToString() == "0" ? null : vm.Id.ToString() };

                DataTable dt = _repo.Voucher_Statement(conditionFields, conditionValues);

                #region Excel

                string StatementName = "";
                if (vm.JournalType == "1")
                {
                    StatementName = AreaTypeGFVM.TransType + " JournalVoucher";
                }
                else if (vm.JournalType == "2")
                {
                    StatementName = AreaTypeGFVM.TransType + " PaymentVoucher";
                }
                else if (vm.JournalType == "3")
                {
                    StatementName = AreaTypeGFVM.TransType + " ReceiptVoucher";
                }

                string filename = StatementName + " -" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(StatementName);
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { StatementName, Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                return Redirect("PF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }


        #endregion

        #region FinancialReports
         [HttpGet]
        public ActionResult TrialBalanceVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypeGFVM.TransType;
                return PartialView("~/Areas/PF/Views/PFReport/TrialBalanceVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }
         [HttpPost]
         public ActionResult TrialBalanceReport(PFReportVM vm)
         {

             try
             {
                 PFReportRepo _repo = new PFReportRepo();
                 string ReportHead = "";
                 string rptLocation = "";
                 PFReport report = new PFReport();
                 vm.TransType = AreaTypeGFVM.TransType;

                 string[] cFields = { "TransType" };
                 string[] cValues = { AreaTypeGFVM.TransType };

                 ReportDocument doc = new ReportDocument();
                 DataSet ds = new DataSet();

                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                 ds = _repo.View_TrialBalance(vm, cFields, cValues);

                 //dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));


                 ReportHead = "There are no data to Preview for TrialBalance";
                 if (ds.Tables[0].Rows.Count > 0)
                 {
                     ReportHead = "TrialBalance";
                 }
                 ds.Tables[0].TableName = "dtFinancialReport";
                 rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptTrialBalance.rpt";
                 string TransactionAmount = ds.Tables[1].Rows.Count > 0 ? ds.Tables[1].Rows[0]["TransactionAmount"].ToString() : "";
                 doc.Load(rptLocation);
                 doc.SetDataSource(ds.Tables[0]);
                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                 FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                 doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                 doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                 doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypeGFVM.TransType + "'";
                 doc.DataDefinition.FormulaFields["DateFrom"].Text = "'" + vm.DateFrom + "'";
                 doc.DataDefinition.FormulaFields["DateTo"].Text = "'" + vm.DateTo + "'";
                 doc.DataDefinition.FormulaFields["TransactionAmount"].Text = "'" + TransactionAmount + "'";
                 doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                 //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                 var rpt = RenderReportAsPDF(doc);
                 doc.Close();
                 return rpt;
             }
             catch (Exception)
             {
                 throw;
             }
         }

         [HttpGet]
         public ActionResult IncomeStatementVeiw()
         {
             try
             {

                 PFReportVM vm = new PFReportVM();
                 vm.TransType = AreaTypeGFVM.TransType;
                 return PartialView("~/Areas/PF/Views/PFReport/IncomeStatementVeiw.cshtml", vm);

             }
             catch (Exception)
             {
                 throw;
             }
         }
         [HttpPost]
         public ActionResult IncomeStatementReport(PFReportVM vm)
         {

              try
             {
                 PFReportRepo _repo = new PFReportRepo();
                 string ReportHead = "";
                 string rptLocation = "";
                 PFReport report = new PFReport();
                 vm.TransType = AreaTypeGFVM.TransType;

                 string[] cFields = { "TransType" };
                 string[] cValues = { AreaTypeGFVM.TransType };

                 ReportDocument doc = new ReportDocument();
                 DataTable dt = new DataTable();


                 dt = _repo.View_IncomeStatement(vm, cFields, cValues);

                 //dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));
                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                 ReportHead = "There are no data to Preview for TrialBalance";
                 if (dt.Rows.Count > 0)
                 {
                     ReportHead = "IncomeStatement";
                 }
                 dt.TableName = "dtFinancialReport";
                 rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptIncomeStatement.rpt";

                 doc.Load(rptLocation);
                 doc.SetDataSource(dt);
                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                 FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                 doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                 doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                 doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypeGFVM.TransType + "'";
                 doc.DataDefinition.FormulaFields["DateFrom"].Text = "'" + vm.DateFrom + "'";
                 doc.DataDefinition.FormulaFields["DateTo"].Text = "'" + vm.DateTo + "'";
                 doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";

                 //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                 var rpt = RenderReportAsPDF(doc);
                 doc.Close();
                 return rpt;
             }
             catch (Exception)
             {
                 throw;
             }
         }

         [HttpGet]
         public ActionResult BalanceSheetVeiw()
         {
             try
             {

                 PFReportVM vm = new PFReportVM();
                 vm.TransType = AreaTypeGFVM.TransType;
                 return PartialView("~/Areas/PF/Views/PFReport/BalanceSheetVeiw.cshtml", vm);

             }
             catch (Exception)
             {
                 throw;
             }
         }
         [HttpPost]
         public ActionResult BalanceSheetReport(PFReportVM vm)
         {


             try
             {
                 PFReportRepo _repo = new PFReportRepo();
                 string ReportHead = "";
                 string rptLocation = "";
                 PFReport report = new PFReport();
                 vm.TransType = AreaTypeGFVM.TransType;

                 string[] cFields = { "TransType" };
                 string[] cValues = { AreaTypeGFVM.TransType };

                 ReportDocument doc = new ReportDocument();
                 DataTable dt = new DataTable();


                 dt = _repo.View_BalanceSheet(vm, cFields, cValues);

                 //dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));
                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                 ReportHead = "There are no data to Preview for TrialBalance";
                 if (dt.Rows.Count > 0)
                 {
                     ReportHead = "BalanceSheet";
                 }
                 dt.TableName = "dtFinancialReport";
                 rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptBalanceSheet.rpt";

                 doc.Load(rptLocation);
                 doc.SetDataSource(dt);
                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                 FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                 doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                 doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                 doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypeGFVM.TransType + "'";
                 doc.DataDefinition.FormulaFields["DateFrom"].Text = "'" + vm.DateFrom + "'";
                 doc.DataDefinition.FormulaFields["DateTo"].Text = "'" + vm.DateTo + "'";
                 doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";

                 //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                 var rpt = RenderReportAsPDF(doc);
                 doc.Close();
                 return rpt;
             }
             catch (Exception)
             {
                 throw;
             }
         }

         [HttpGet]
         public ActionResult LedgerVeiw()
         {
             try
             {

                 PFReportVM vm = new PFReportVM();
                 vm.TransType = AreaTypeGFVM.TransType;
                 return PartialView("~/Areas/PF/Views/PFReport/LedgerVeiw.cshtml", vm);
             }
             catch (Exception)
             {
                 throw;
             }
         }
         [HttpPost]
         public ActionResult LedgerReport(PFReportVM vm)
         {


             try
             {
                 PFReportRepo _repo = new PFReportRepo();
                 string ReportHead = "";
                 string rptLocation = "";
                 PFReport report = new PFReport();

                 vm.TransType = AreaTypeGFVM.TransType;
                 string[] cFields = { "TransType" };
                 string[] cValues = { AreaTypeGFVM.TransType };

                 ReportDocument doc = new ReportDocument();
                 DataTable dt = new DataTable();


                 dt = _repo.View_NetChange(vm,cFields, cValues);

                 //dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));
                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                 ReportHead = "There are no data to Preview for TrialBalance";
                 if (dt.Rows.Count > 0)
                 {
                     ReportHead = "LedgerReport";
                 }
                 dt.TableName = "dtLedger";
                 rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptLedger.rpt";

                 doc.Load(rptLocation);
                 doc.SetDataSource(dt);
                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                 FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                 doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                 doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                 doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypeGFVM.TransType + "'";
                 doc.DataDefinition.FormulaFields["DateFrom"].Text = "'" + vm.DateFrom + "'";
                 doc.DataDefinition.FormulaFields["DateTo"].Text = "'" + vm.DateTo + "'";
                 doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";

                 //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                 var rpt = RenderReportAsPDF(doc);
                 doc.Close();
                 return rpt;
             }
             catch (Exception)
             {
                 throw;
             }
         }

         [HttpPost]
         public ActionResult EmployeeLedger(PFReportVM vm)
         {
             try
             {
                 string rptLocation = "";
                 string ReportHead = "";

                 PFReportRepo _repo = new PFReportRepo();

                 string[] conditionFields = { };
                 string[] conditionValues = { };

                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                 DataTable dt = _repo.GFEmployeeLedger(vm, conditionFields, conditionValues);
                 if (vm.ReportType == "Excel")
                 {
                     dt.Columns.Remove("Loan");
                     #region Excel

                     string filename = AreaTypeGFVM.TransType + "EmployeeStatement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                     ExcelPackage excel = new ExcelPackage();
                     var workSheet = excel.Workbook.Worksheets.Add(AreaTypeGFVM.TransType + "EmployeeStatement");
                     CompanyRepo cRepo = new CompanyRepo();
                     CompanyVM comInfo = cRepo.SelectById(1);
                     string Line1 = comInfo.Name;
                     string Line2 = comInfo.Address;
                     string Line3 = "";


                     string[] ReportHeaders = new string[] { AreaTypeGFVM.TransType + "Employee Statement", Line1, Line2, Line3 };

                     ExcelSheetFormat(dt, workSheet, ReportHeaders);
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
                     return Redirect("GF/GFEmployeeProvision/IndexFiscalPeriod");

                     #endregion
                 }
                 else
                 {
                     ReportDocument doc = new ReportDocument();
                     dt.TableName = "dtEmployeeLedger";
                     if (vm.ReportType == "Individual")
                     {
                         rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\GF\\RptGFIndividualLedger.rpt";
                     }
                     else
                     {
                         rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\GF\\rptGFEmployeeLedger.rpt";
                     }

                     doc.Load(rptLocation);
                     doc.SetDataSource(dt);
                     string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                     FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                     doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                     doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypeGFVM.TransType + "'";
                     doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";

                     if (vm.DateFrom != null && vm.DateTo != null)
                     {
                         doc.DataDefinition.FormulaFields["DateFrom"].Text = "'" + vm.DateFrom + "'";
                         doc.DataDefinition.FormulaFields["DateTo"].Text = "'" + vm.DateTo + "'";

                     }
                     //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                     var rpt = RenderReportAsPDF(doc);
                     doc.Close();
                     return rpt;
                 }
                 //return File(vm.MemStream, "application/PDF");
             }

             catch (Exception)
             {
                 throw;
             }
         }

        #endregion

         public ActionResult YearClosingReportVeiw(string JournalType = "1")
         {
             try
             {
                 PFReportVM vm = new PFReportVM();
                 vm.JournalType = JournalType;
                 vm.TransType = AreaTypePFVM.TransType;
                 return View(vm);

             }
             catch (Exception)
             {
                 throw;
             }
         }
         public ActionResult Year_Closing(string fydid)
         {
             PFReportVM vm = new PFReportVM();
             string[] result = new string[6];
             try
             {
                 vm.YearTo = fydid;
                 vm.BaseEntity.CreatedBy = identity.Name;
                 vm.BaseEntity.CreatedFrom = identity.WorkStationIP;
                 vm.TransType = AreaTypePFVM.TransType;

                 PFReportRepo _repo = new PFReportRepo();
                 result = _repo.YearClosing(vm);
                 return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);

             }
             catch (Exception)
             {
                 throw;
             }
         }

         [HttpGet]
         public ActionResult IFRSReportView(string ReportType)
         {
             try
             {
                 PFReportVM vm = new PFReportVM();
                 vm.ReportType = ReportType;
                 vm.TransType = "PF";
                 vm.TransType = AreaTypePFVM.TransType;
                 return View(vm);
                 //return PartialView("~/Areas/PF/Views/PFReport/IFRSReporView.cshtml", vm);
             }
             catch (Exception)
             {
                 throw;
             }
         }
         [HttpPost]
         public ActionResult IFRSReport(PFReportVM vm)
         {

             try
             {
                 PFReportRepo _repo = new PFReportRepo();
                 string ReportHead = "";
                 string rptLocation = "";
                 PFReport report = new PFReport();
                 string fileName = "rptIFRSReportTB.rpt";
                 ReportDocument doc = new ReportDocument();
                 DataTable dt = new DataTable();
                 DataTable dt1 = new DataTable();
                 DataSet ds = new DataSet();
                 vm.TransType = AreaTypePFVM.TransType;
                 ds = _repo.IFRSReports(vm);
                 dt = ds.Tables[0];

                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();
                 FiscalYearRepo fyrepo = new FiscalYearRepo();

                 //vm.DateFrom = fyrepo.FYPeriodDetail(vm.MonthFrom).FirstOrDefault().PeriodName;
                 //vm.DateTo = fyrepo.FYPeriodDetail(vm.MonthTo).FirstOrDefault().PeriodName;

                 ReportHead = "";
                 if (dt.Rows.Count > 0)
                 {
                     if (vm.ReportType.ToUpper() == "BS")
                     {
                         //ReportHead = "Balance Sheet";
                         fileName = "rptIFRSReportBS.rpt";
                         dt1 = ds.Tables[1];
                         vm.DateFrom = Ordinary.StringToDate(dt1.Rows[0]["FirstEnd"].ToString());
                         vm.YearFrom = dt1.Rows[0]["FirstYear"].ToString();

                         vm.DateTo = Ordinary.StringToDate(dt1.Rows[0]["LastEnd"].ToString());
                         vm.YearTo = dt1.Rows[0]["LastYear"].ToString();

                     }
                     else if (vm.ReportType.ToUpper() == "IS")
                     {
                         //ReportHead = "Income Statement";
                         fileName = "rptIFRSReportIS.rpt";
                         dt1 = ds.Tables[1];
                         string json = JsonConvert.SerializeObject(dt1);
                         vm.PFReport1VMs = JsonConvert.DeserializeObject<List<PFReport1VM>>(json);
                         vm.PFReport1VM = vm.PFReport1VMs.FirstOrDefault();
                         vm.DateFrom = Ordinary.StringToDate(dt1.Rows[0]["FirstEnd"].ToString());
                         vm.YearFrom = dt1.Rows[0]["FirstYear"].ToString();

                         vm.DateTo = Ordinary.StringToDate(dt1.Rows[0]["LastEnd"].ToString());
                         vm.YearTo = dt1.Rows[0]["LastYear"].ToString();

                         vm.PFReport1VM.FirstNetProfit = Math.Abs(vm.PFReport1VM.FirstNetProfit);
                         vm.PFReport1VM.LastNetProfit = Math.Abs(vm.PFReport1VM.LastNetProfit);
                     }
                     else if (vm.ReportType.ToUpper() == "TB" || vm.ReportType.ToUpper() == "NC")
                     {
                         //ReportHead = "Trial Balance";
                         fileName = "rptIFRSReportTB.rpt";
                         if (vm.ReportType.ToUpper() == "NC")
                         {
                             fileName = "rptIFRSReportNC.rpt";

                         }

                         dt1 = ds.Tables[1];
                         string json = JsonConvert.SerializeObject(dt1);
                         vm.PFReport1VMs = JsonConvert.DeserializeObject<List<PFReport1VM>>(json);
                         vm.PFReport1VM = vm.PFReport1VMs.FirstOrDefault();
                         vm.DateFrom = Ordinary.StringToDate(dt1.Rows[0]["FirstEnd"].ToString());
                         vm.YearFrom = dt1.Rows[0]["FirstYear"].ToString();

                         vm.DateTo = Ordinary.StringToDate(dt1.Rows[0]["LastEnd"].ToString());
                         vm.YearTo = dt1.Rows[0]["LastYear"].ToString();

                         //vm.PFReport1VM.FirstNetProfit = Math.Abs(vm.PFReport1VM.FirstNetProfit);
                         //vm.PFReport1VM.LastNetProfit = Math.Abs(vm.PFReport1VM.LastNetProfit);
                     }

                 }
                 dt.TableName = "dtIFRSReport";
                 rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\" + fileName;

                 doc.Load(rptLocation);
                 doc.SetDataSource(dt);
                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                 FormulaFieldDefinitions crFormulaF = doc.DataDefinition.FormulaFields;

                 CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "FirstNetProfit", vm.PFReport1VM.FirstNetProfit.ToString("#,##0.00"));
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "LastNetProfit", vm.PFReport1VM.LastNetProfit.ToString("#,##0.00"));

                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "ReportHeaderA4", companyLogo);
                 //_vCommonFormMethod.FormulaField(doc, crFormulaF, "ReportHead", ReportHead);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "TransType", AreaTypePFVM.TransType);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "DateFrom", vm.DateFrom);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "DateTo", vm.DateTo);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "YearFrom", vm.YearFrom);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "YearTo", vm.YearTo);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "Address", cvm.Address);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "CompanyName", cvm.Name);



                 var rpt = RenderReportAsPDF(doc);
                 doc.Close();
                 return rpt;
             }
             catch (Exception)
             {
                 throw;
             }
         }
    }
}
