using CrystalDecisions.CrystalReports.Engine;
using OfficeOpenXml;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.Payroll;
using SymRepository.Tax;
using SymViewModel.Common;
using SymViewModel.Payroll;
using SymViewModel.Tax;
using SymWebUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web.Mvc;

namespace SymWebUI.Areas.TAX.Controllers
{
    public class TaxReportController : Controller
    {
        //
        // GET: /TAX/Report/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;


        public ActionResult Index()
        {
            return View();
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult TaxCertificateReport()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "report").ToString();
            return View();
        }
        [HttpGet]
        public ActionResult TaxCertificateReportView(string empCodeF, string empCodeT, string fydIdFrom, string fydIdTo)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dtTaxIncome = new DataTable();
                DataTable dtTaxDeposit = new DataTable();

                DataSet ds = new DataSet();

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                Schedule1SalaryVM ssmVM = new Schedule1SalaryVM();
                Schedule1SalaryMonthlyRepo _ssmRepo = new Schedule1SalaryMonthlyRepo();

                TaxDepositVM tdVM = new TaxDepositVM();
                TaxDepositRepo _tdRepo = new TaxDepositRepo();


                if (empCodeF == "0_0" || empCodeF == "0" || empCodeF == "" || empCodeF == "null" || empCodeF == null)
                {
                    empCodeF = "";
                }
                if (empCodeT == "0_0" || empCodeT == "0" || empCodeT == "" || empCodeT == "null" || empCodeT == null)
                {
                    empCodeT = "";
                }

                if (fydIdFrom == "0_0" || fydIdFrom == "0" || fydIdFrom == "" || fydIdFrom == "null" || fydIdFrom == null)
                {
                    fydIdFrom = "";
                }

                if (fydIdTo == "0_0" || fydIdTo == "0" || fydIdTo == "" || fydIdTo == "null" || fydIdTo == null)
                {
                    fydIdTo = "";
                }






                string[] ssmConditionFields = { "ve.Code>", "ve.Code<", "ssm.FiscalyeardetailId>", "ssm.FiscalyeardetailId<" };
                string[] ssmConditionValues = { empCodeF, empCodeT, fydIdFrom, fydIdTo };

                dtTaxIncome = _ssmRepo.TaxIncomeReport(ssmVM, ssmConditionFields, ssmConditionValues);
                string[] tdConditionFields = { "ve.Code>", "ve.Code<", "td.FiscalyeardetailId>", "td.FiscalyeardetailId<" };
                string[] tdConditionValues = { empCodeF, empCodeT, fydIdFrom, fydIdTo };

                dtTaxDeposit = _tdRepo.Report(tdVM, tdConditionFields, tdConditionValues);

                dtTaxDeposit = FilterDepositDate(fydIdFrom, fydIdTo, dtTaxDeposit);

                ReportHead = "There are no data to Preview for Tax Certificate";
                if (dtTaxIncome.Rows.Count > 0)
                {
                    ReportHead = "Tax Certificate";
                }

                ds.Tables.Add(dtTaxIncome);
                ds.Tables[0].TableName = "dtTaxIncome";
                ds.Tables.Add(dtTaxDeposit);
                ds.Tables[1].TableName = "dtTaxDeposit";


                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Tax\\rptTaxCertificate.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";

                string FiscalPeriodFrom = "";
                string StartDate = "";
                string FiscalPeriodTo = "";
                string EndDate = "";
                FiscalYearRepo _fyRepo = new FiscalYearRepo();
                FiscalYearDetailVM fyDVM = new FiscalYearDetailVM();
                if (!string.IsNullOrWhiteSpace(fydIdFrom))
                {
                    fyDVM = _fyRepo.FYPeriodDetail(Convert.ToInt32(fydIdFrom)).FirstOrDefault();
                    FiscalPeriodFrom = fyDVM.Name;
                    StartDate = Ordinary.StringToDate(fyDVM.PeriodStart);
                }
                if (!string.IsNullOrWhiteSpace(fydIdTo))
                {
                    fyDVM = _fyRepo.FYPeriodDetail(Convert.ToInt32(fydIdTo)).FirstOrDefault();
                    FiscalPeriodTo = fyDVM.Name;
                    EndDate = Ordinary.StringToDate(fyDVM.PeriodEnd);
                }

                doc.DataDefinition.FormulaFields["FiscalPeriodFrom"].Text = "'" + FiscalPeriodFrom + "'";
                doc.DataDefinition.FormulaFields["FiscalPeriodTo"].Text = "'" + FiscalPeriodTo + "'";
                doc.DataDefinition.FormulaFields["StartDate"].Text = "'" + StartDate + "'";
                doc.DataDefinition.FormulaFields["EndDate"].Text = "'" + EndDate + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception e)
            {
                FileLogger.Log("TextReportController", "TaxCertificateReportView", e.ToString());
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult TaxReportNBRMonthly()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "report").ToString();
            return View();
        }
        [HttpGet]
        public ActionResult TaxReportNBRMonthlyView(string fydIdFrom, bool IsExcel = false)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dtTaxIncome = new DataTable();
                DataTable dtTaxDeposit = new DataTable();

                DataSet ds = new DataSet();

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                Schedule1SalaryVM ssmVM = new Schedule1SalaryVM();
                Schedule1SalaryMonthlyRepo _ssmRepo = new Schedule1SalaryMonthlyRepo();

                TaxDepositVM tdVM = new TaxDepositVM();
                TaxDepositRepo _tdRepo = new TaxDepositRepo();
                CompanyRepo _CompanyRepo = new CompanyRepo();

                CompanyVM vm= _CompanyRepo.SelectAll().FirstOrDefault();

                dtTaxIncome = _ssmRepo.TaxReportNBRMonthly(fydIdFrom);
                dtTaxIncome = Ordinary.DtColumnStringToDate(dtTaxIncome, "DepositDate");

                if (!IsExcel)
                {
                    ReportHead = "There are no data to Preview for Tax Certificate";
                    if (dtTaxIncome.Rows.Count > 0)
                    {
                        ReportHead = "Tax Certificate";
                    }

                    ds.Tables.Add(dtTaxIncome);
                    //ds.Tables[0].TableName = "dtTaxIncome";
                    ds.Tables[0].TableName = "dtSalaryTax_TIB";




                    //rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Tax\\rptTaxCertificate.rpt";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Tax\\rptSalaryTax_TIB.rpt";

                    doc.Load(rptLocation);
                    doc.SetDataSource(ds);
                    string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                    doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                    doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";

                    string FiscalPeriodFrom = "";
                    string StartDate = "";
                    string FiscalPeriodTo = "";
                    string EndDate = "";
                    FiscalYearRepo _fyRepo = new FiscalYearRepo();
                    FiscalYearDetailVM fyDVM = new FiscalYearDetailVM();
                    if (!string.IsNullOrWhiteSpace(fydIdFrom))
                    {
                        fyDVM = _fyRepo.FYPeriodDetail(Convert.ToInt32(fydIdFrom)).FirstOrDefault();
                        FiscalPeriodFrom = fyDVM.Name;
                        StartDate = Ordinary.StringToDate(fyDVM.PeriodStart);
                        FiscalPeriodFrom = Convert.ToDateTime(StartDate).ToString("MMM-yyyy");
                    }

                    doc.DataDefinition.FormulaFields["FiscalPeriodFrom"].Text = "'" + FiscalPeriodFrom + "'";
                    doc.DataDefinition.FormulaFields["FiscalPeriodTo"].Text = "'" + FiscalPeriodTo + "'";
                    doc.DataDefinition.FormulaFields["StartDate"].Text = "'" + StartDate + "'";
                    doc.DataDefinition.FormulaFields["EndDate"].Text = "'" + EndDate + "'";
                    doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + vm.Name + "'";
                    doc.DataDefinition.FormulaFields["CompanyAddress"].Text = "'" + vm.Address + "'";
                    doc.DataDefinition.FormulaFields["CompanyPhone"].Text = "'" + vm.Phone + "'";
                    doc.DataDefinition.FormulaFields["CompanyEmail"].Text = "'" + vm.Email + "'";
                    doc.DataDefinition.FormulaFields["TIN"].Text = "'" + vm.TaxId + "'";
                    var rpt = RenderReportAsPDF(doc);
                    doc.Close();
                    return rpt;
                }
                else
                {
                    dtTaxIncome.Columns.Remove("FiscalYearId");
                    dtTaxIncome.Columns.Remove("FiscalYearDetailId");
                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                    workSheet.Cells[1, 1].LoadFromDataTable(dtTaxIncome, true);

                    string filename = "TaxReportNBRMonthly" + "-" + DateTime.Now.ToString("yyyyMMdd");
                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                        excel.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                  
                    return RedirectToAction("TaxReportNBRMonthly");
                }
               
            }
            catch (Exception e)
            {
                FileLogger.Log("TextReportController", "TaxCertificateReportView", e.ToString());
                throw;
            }
        }



        private DataTable FilterDepositDate(string fydIdFrom, string fydIdTo, DataTable dtTaxDeposit)
        {
            var fiscalYearRepo = new FiscalYearRepo();


            if (string.IsNullOrEmpty(fydIdFrom) && string.IsNullOrEmpty(fydIdTo))
            {
                return dtTaxDeposit;
            }

            var temp = dtTaxDeposit.Copy();

            DateTime yearFrom;
            var rows = new List<DataRow>();
            try
            {
                if (!string.IsNullOrEmpty(fydIdFrom))
                {
                    yearFrom = Convert.ToDateTime(Ordinary
                        .StringToDate(fiscalYearRepo.FYPeriodDetail(Convert.ToInt32(fydIdFrom))
                            .FirstOrDefault().PeriodStart));

                    var dataRows = temp.AsEnumerable().Where(x =>
                        Convert.ToDateTime(x["DepositDate"]) >= yearFrom);

                    rows.AddRange(dataRows);
                }
           

                DateTime yearTo;

                if (!string.IsNullOrEmpty(fydIdTo))
                {
                    yearTo = Convert.ToDateTime(Ordinary
                        .StringToDate(fiscalYearRepo.FYPeriodDetail(Convert.ToInt32(fydIdTo))
                            .FirstOrDefault().PeriodEnd));

                    if (rows.Any())
                        temp = rows.CopyToDataTable();

                    var dataRows = temp.AsEnumerable().Where(x =>
                        Convert.ToDateTime(x["DepositDate"]) <= yearTo);

                    rows.RemoveAll(x => true);
                    rows.AddRange(dataRows);
                }
            }
            catch (Exception e)
            {
                return dtTaxDeposit;
            }




            return rows.Any() ? rows.CopyToDataTable() : new DataTable();
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult TaxSchedule1MonthlyReport()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "report").ToString();
            return View();
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult InvestmentTaxReport()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "report").ToString();
            return View();
        }



        [HttpGet]
        public ActionResult TaxSchedule1MonthlyReportView(string empCodeF, string empCodeT, string fydIdFrom, string fydIdTo)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dtTaxSchedule1 = new DataTable();
                DataTable dtEmployeeTaxSlabDetail = new DataTable();

                DataSet ds = new DataSet();

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                Schedule1SalaryVM ssmVM = new Schedule1SalaryVM();
                Schedule1SalaryMonthlyRepo _ssmRepo = new Schedule1SalaryMonthlyRepo();

                EmployeeTaxSlabDetailVM etsDVM = new EmployeeTaxSlabDetailVM();
                EmployeeTaxSlabDetailMonthlyRepo _etsDMRepo = new EmployeeTaxSlabDetailMonthlyRepo();


                if (empCodeF == "0_0" || empCodeF == "0" || empCodeF == "" || empCodeF == "null" || empCodeF == null)
                {
                    empCodeF = "";
                }
                if (empCodeT == "0_0" || empCodeT == "0" || empCodeT == "" || empCodeT == "null" || empCodeT == null)
                {
                    empCodeT = "";
                }

                if (fydIdFrom == "0_0" || fydIdFrom == "0" || fydIdFrom == "" || fydIdFrom == "null" || fydIdFrom == null)
                {
                    fydIdFrom = "";
                }

                if (fydIdTo == "0_0" || fydIdTo == "0" || fydIdTo == "" || fydIdTo == "null" || fydIdTo == null)
                {
                    fydIdTo = "";
                }






                string[] ssmConditionFields = { "ve.Code>", "ve.Code<", "ssm.FiscalyeardetailId>", "ssm.FiscalyeardetailId<" };
                string[] ssmConditionValues = { empCodeF, empCodeT, fydIdFrom, fydIdTo };

                dtTaxSchedule1 = _ssmRepo.Report(ssmVM, ssmConditionFields, ssmConditionValues);
                string[] tdConditionFields = { "ve.Code>", "ve.Code<", "ets.FiscalyeardetailId>", "ets.FiscalyeardetailId<" };
                string[] tdConditionValues = { empCodeF, empCodeT, fydIdFrom, fydIdTo };

                dtEmployeeTaxSlabDetail = _etsDMRepo.Report(etsDVM, tdConditionFields, tdConditionValues);


                ReportHead = "There are no data to Preview for Tax Schedule1 Monthly";
                if (dtTaxSchedule1.Rows.Count > 0)
                {
                    ReportHead = "Tax Schedule1 Monthly";
                }

                ds.Tables.Add(dtTaxSchedule1);
                ds.Tables[0].TableName = "dtTaxSchedule1";
                ds.Tables.Add(dtEmployeeTaxSlabDetail);
                ds.Tables[1].TableName = "dtEmployeeTaxSlabDetail";


                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Tax\\rptSchedule1SalaryMonthly.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                //doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                //doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";

                string FiscalPeriodFrom = "";
                string StartDate = "";
                string FiscalPeriodTo = "";
                string EndDate = "";
                FiscalYearRepo _fyRepo = new FiscalYearRepo();
                FiscalYearDetailVM fyDVM = new FiscalYearDetailVM();
                if (!string.IsNullOrWhiteSpace(fydIdFrom))
                {
                    fyDVM = _fyRepo.FYPeriodDetail(Convert.ToInt32(fydIdFrom)).FirstOrDefault();
                    FiscalPeriodFrom = fyDVM.Name;
                    StartDate = Ordinary.StringToDate(fyDVM.PeriodStart);
                }
                if (!string.IsNullOrWhiteSpace(fydIdTo))
                {
                    fyDVM = _fyRepo.FYPeriodDetail(Convert.ToInt32(fydIdTo)).FirstOrDefault();
                    FiscalPeriodTo = fyDVM.Name;
                    EndDate = Ordinary.StringToDate(fyDVM.PeriodEnd);
                }

                //doc.DataDefinition.FormulaFields["FiscalPeriodFrom"].Text = "'" + FiscalPeriodFrom + "'";
                //doc.DataDefinition.FormulaFields["FiscalPeriodTo"].Text = "'" + FiscalPeriodTo + "'";
                //doc.DataDefinition.FormulaFields["StartDate"].Text = "'" + StartDate + "'";
                //doc.DataDefinition.FormulaFields["EndDate"].Text = "'" + EndDate + "'";
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
        public ActionResult InvestmentTaxReportView(string empCodeF, string empCodeT, string year)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dtTaxSchedule1 = new DataTable();

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                Schedule1SalaryVM ssmVM = new Schedule1SalaryVM();
                Schedule1SalaryMonthlyRepo _ssmRepo = new Schedule1SalaryMonthlyRepo();

                EmployeeTaxSlabDetailVM etsDVM = new EmployeeTaxSlabDetailVM();
                EmployeeTaxSlabDetailMonthlyRepo _etsDMRepo = new EmployeeTaxSlabDetailMonthlyRepo();

                if (empCodeF == "0_0" || empCodeF == "0" || empCodeF == "" || empCodeF == "null" || empCodeF == null)
                {
                    empCodeF = "";
                }
                if (empCodeT == "0_0" || empCodeT == "0" || empCodeT == "" || empCodeT == "null" || empCodeT == null)
                {
                    empCodeT = "";
                }

                string[] ssmConditionFields = { "ve.Code>", "ve.Code<", "ssm.Year"};
                string[] ssmConditionValues = { empCodeF, empCodeT, year };

                dtTaxSchedule1 = _ssmRepo.InvestmentTaxReport(ssmVM, ssmConditionFields, ssmConditionValues);

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dtTaxSchedule1, true);

                string filename = " Investment & Tax Report " + "-" + DateTime.Now.ToString("yyyyMMdd");
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                return RedirectToAction("InvestmentTaxReport");

               
            }
            catch (Exception exception)
            {
                throw;
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult TaxSchedule1YearlyReport()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "report").ToString();
            return View();
        }
        [HttpGet]
        public ActionResult TaxSchedule1YearlyReportView(string empCodeF, string empCodeT, string fyear)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dtTaxSchedule1 = new DataTable();
                //DataTable dtTaxDeposit = new DataTable();

                DataSet ds = new DataSet();

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                Schedule1SalaryVM ssmVM = new Schedule1SalaryVM();
                Schedule1SalaryYearlyRepo _ssyRepo = new Schedule1SalaryYearlyRepo();

                //TaxDepositVM tdVM = new TaxDepositVM();
                //TaxDepositRepo _tdRepo = new TaxDepositRepo();


                if (empCodeF == "0_0" || empCodeF == "0" || empCodeF == "" || empCodeF == "null" || empCodeF == null)
                {
                    empCodeF = "";
                }
                if (empCodeT == "0_0" || empCodeT == "0" || empCodeT == "" || empCodeT == "null" || empCodeT == null)
                {
                    empCodeT = "";
                }

                if (fyear == "0_0" || fyear == "0" || fyear == "" || fyear == "null" || fyear == null)
                {
                    fyear = "";
                }






                string[] ssmConditionFields = { "ve.Code>", "ve.Code<", "ssy.Year" };
                string[] ssmConditionValues = { empCodeF, empCodeT, fyear };

                dtTaxSchedule1 = _ssyRepo.Report(ssmVM, ssmConditionFields, ssmConditionValues);
                //string[] tdConditionFields = { "ve.Code>", "ve.Code<", "td.FiscalyeardetailId>", "td.FiscalyeardetailId<" };
                //string[] tdConditionValues = { empCodeF, empCodeT, fydIdFrom, fydIdTo };

                //dtTaxDeposit = _tdRepo.Report(tdVM, tdConditionFields, tdConditionValues);


                ReportHead = "There are no data to Preview for Tax Schedule1 Yearly";
                if (dtTaxSchedule1.Rows.Count > 0)
                {
                    ReportHead = "Tax Schedule1 Yearly";
                }

                ds.Tables.Add(dtTaxSchedule1);
                ds.Tables[0].TableName = "dtTaxSchedule1";
                //ds.Tables.Add(dtTaxDeposit);
                //ds.Tables[1].TableName = "dtTaxDeposit";


                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Tax\\rptSchedule1SalaryYearly.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                //doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                //doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";



                //doc.DataDefinition.FormulaFields["FiscalPeriodFrom"].Text = "'" + FiscalPeriodFrom + "'";
                //doc.DataDefinition.FormulaFields["FiscalPeriodTo"].Text = "'" + FiscalPeriodTo + "'";
                //doc.DataDefinition.FormulaFields["StartDate"].Text = "'" + StartDate + "'";
                //doc.DataDefinition.FormulaFields["EndDate"].Text = "'" + EndDate + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Thread thread;

        [HttpGet]
        public ActionResult TaxCertificateEmail(string empCodeF, string empCodeT, string fydIdFrom, string fydIdTo)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dtTaxIncome = new DataTable();
                DataTable dtTaxDeposit = new DataTable();

                DataSet ds = new DataSet();

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                Schedule1SalaryVM ssmVM = new Schedule1SalaryVM();
                Schedule1SalaryMonthlyRepo _ssmRepo = new Schedule1SalaryMonthlyRepo();

                TaxDepositVM tdVM = new TaxDepositVM();
                TaxDepositRepo _tdRepo = new TaxDepositRepo();


                if (empCodeF == "0_0" || empCodeF == "0" || empCodeF == "" || empCodeF == "null" || empCodeF == null)
                {
                    empCodeF = "";
                }
                if (empCodeT == "0_0" || empCodeT == "0" || empCodeT == "" || empCodeT == "null" || empCodeT == null)
                {
                    empCodeT = "";
                }

                if (fydIdFrom == "0_0" || fydIdFrom == "0" || fydIdFrom == "" || fydIdFrom == "null" || fydIdFrom == null)
                {
                    fydIdFrom = "";
                }

                if (fydIdTo == "0_0" || fydIdTo == "0" || fydIdTo == "" || fydIdTo == "null" || fydIdTo == null)
                {
                    fydIdTo = "";
                }
                
                string[] ssmConditionFields = { "ve.Code>", "ve.Code<", "ssm.FiscalyeardetailId>", "ssm.FiscalyeardetailId<" };
                string[] ssmConditionValues = { empCodeF, empCodeT, fydIdFrom, fydIdTo };

                dtTaxIncome = _ssmRepo.TaxIncomeReport(ssmVM, ssmConditionFields, ssmConditionValues);
                string[] tdConditionFields = { "ve.Code>", "ve.Code<", "td.FiscalyeardetailId>", "td.FiscalyeardetailId<" };
                string[] tdConditionValues = { empCodeF, empCodeT, fydIdFrom, fydIdTo };

                dtTaxDeposit = _tdRepo.Report(tdVM, tdConditionFields, tdConditionValues);
                dtTaxDeposit = FilterDepositDate(fydIdFrom, fydIdTo, dtTaxDeposit);

                
                ReportHead = "There are no data to Preview for Tax Certificate";
                if (dtTaxIncome.Rows.Count > 0)
                {
                    ReportHead = "Tax Certificate";
                }

                ds.Tables.Add(dtTaxIncome);
                ds.Tables[0].TableName = "dtTaxIncome";
                ds.Tables.Add(dtTaxDeposit);
                ds.Tables[1].TableName = "dtTaxDeposit";


                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Tax\\rptTaxCertificate.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";

                string FiscalPeriodFrom = "";
                string StartDate = "";
                string FiscalPeriodTo = "";
                string EndDate = "";
                FiscalYearRepo _fyRepo = new FiscalYearRepo();
                FiscalYearDetailVM fyDVM = new FiscalYearDetailVM();
                if (!string.IsNullOrWhiteSpace(fydIdFrom))
                {
                    fyDVM = _fyRepo.FYPeriodDetail(Convert.ToInt32(fydIdFrom)).FirstOrDefault();
                    FiscalPeriodFrom = fyDVM.Name;
                    StartDate = Ordinary.StringToDate(fyDVM.PeriodStart);
                }
                if (!string.IsNullOrWhiteSpace(fydIdTo))
                {
                    fyDVM = _fyRepo.FYPeriodDetail(Convert.ToInt32(fydIdTo)).FirstOrDefault();
                    FiscalPeriodTo = fyDVM.Name;
                    EndDate = Ordinary.StringToDate(fyDVM.PeriodEnd);
                }

                doc.DataDefinition.FormulaFields["FiscalPeriodFrom"].Text = "'" + FiscalPeriodFrom + "'";
                doc.DataDefinition.FormulaFields["FiscalPeriodTo"].Text = "'" + FiscalPeriodTo + "'";
                doc.DataDefinition.FormulaFields["StartDate"].Text = "'" + StartDate + "'";
                doc.DataDefinition.FormulaFields["EndDate"].Text = "'" + EndDate + "'";

                string Preiod = StartDate + " to " + EndDate;

                thread = new Thread(unused => EmpEmailProcessTIB(ds, doc, Preiod));

                thread.Start();
                // EmpEmailProcess(ds, doc, FullPeriodName)
                string[] result = new string[6];
                result[0] = "Successfully";
                result[1] = "Pay Slip Email Sent";
                Session["result"] = result[0] + "~" + result[1];

                var rpt = RenderReportAsPDF(doc);
                //doc.Close();
                return View("TaxCertificateReport");
                
            }
            catch (Exception e)
            {
                FileLogger.Log("TextReportController", "TaxCertificateReportView", e.ToString());
                throw;
            }
        }

       
    

        public void EmpEmailProcessTIB(DataSet ds, ReportDocument rptDoc, string FiscalPeriod)
        {
            EmailSettingsTIB ems = new EmailSettingsTIB();
            SettingRepo _setDAL = new SettingRepo();
            ems.MailHeader = _setDAL.settingValue("Mail", "MailSubjectTC");
            ems.MailHeader = ems.MailHeader.Replace("vmonth", FiscalPeriod);
            string mailbody = _setDAL.settingValue("Mail", "MailBodyTC");
            DataSet DsTemp = new DataSet();
            string strSort = "EmployeeCode";
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                try
                {
                    DsTemp = new DataSet();
                    dt1 = new DataTable();
                    dt2 = new DataTable();

                    DataRow[] DataRow1 = ds.Tables[0].Select("EmployeeCode='" + item["EmployeeCode"].ToString() + "'");
                    DataView dtview1 = new DataView(DataRow1.CopyToDataTable());
                    dtview1.Sort = strSort;
                    dt1 = dtview1.ToTable();

                    DataRow[] DataRow2 = ds.Tables[1].Select("EmployeeCode='" + item["EmployeeCode"] + "'");
                    DataView dtview2 = new DataView(DataRow2.CopyToDataTable());
                    dtview2.Sort = strSort;
                    dt2 = dtview2.ToTable();

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                                                                      SecurityProtocolType.Tls11 |
                                                                      SecurityProtocolType.Tls;

                    ems.MailToAddress = dt1.Rows[0]["Email"].ToString();

                    if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
                    {

                      
                        ds.Tables[0].TableName = "dtTaxIncome";                    
                        ds.Tables[1].TableName = "dtTaxDeposit";

                    
                        ems.MailBody = mailbody.Replace("vmonth", FiscalPeriod)
                            .Replace("vname", item["EmployeeName"].ToString());
                     
                        ems.FileName = "Tax Certificate of " + item["EmployeeName"].ToString() + " (" + FiscalPeriod + ")";
                      
                        rptDoc.SetDataSource(ds);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        using (var smpt = new SmtpClient())
                        {
                            smpt.EnableSsl = ems.USsel;
                            smpt.Host = ems.ServerName;
                            smpt.Port = ems.Port;
                            smpt.UseDefaultCredentials = false;
                            smpt.EnableSsl = true;
                            smpt.Credentials = new NetworkCredential(ems.UserName, ems.Password);
                            MailMessage mailmessage = new MailMessage(
                                ems.MailFromAddress,
                                ems.MailToAddress,
                                ems.MailHeader,
                                ems.MailBody);
                            mailmessage.Attachments.Add(new Attachment(
                                rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat),
                                ems.FileName + ".pdf"));

                            smpt.Send(mailmessage);
                            mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                           FileLogger.Log("Tax Certificate Process", this.GetType().Name, "EmpEmail Send To:" + item["EmployeeName"].ToString() + Environment.NewLine + "EmpEmailAddress:" + dt1.Rows[0]["Email"].ToString());

                        }

                        //Thread.Sleep(500);
                        Thread.Sleep(3000);
                    }
                }
                catch (SmtpFailedRecipientException ex)
                {
                    FileLogger.Log("EmpEmailProcessTIB", this.GetType().Name, "EmpEmail Not Send To:" + item["EmpName"].ToString() + " " + ex.Message + Environment.NewLine + ex.StackTrace);

                    // throw ex;
                }
            }

            rptDoc.Close();
            thread.Abort();
        }

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }


    }
}
