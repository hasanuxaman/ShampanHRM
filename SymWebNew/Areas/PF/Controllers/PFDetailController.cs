using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using SymViewModel.Common;
using SymReporting.PF;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Newtonsoft.Json;
using SymWebUI.Areas.PF.Models;
using SymRepository.Payroll;
using System.Web;

namespace SymWebUI.Areas.PF.Controllers
{
    public class PFDetailController : Controller
    {
        public PFDetailController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /PF/PFDetail/
        PFDetailRepo _repo = new PFDetailRepo();
        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        public ActionResult Index(int id)
        {
            try
            {
                ViewBag.id = id;

                PFDetailRepo _repo = new PFDetailRepo();
                string[] conditionFields = { "pfd.Id" };
                string[] conditionValues = { id.ToString() };
                var pfh = _repo.SelectFiscalPeriodHeader(conditionFields, conditionValues).FirstOrDefault();
                ViewBag.PeriodName = pfh.FiscalPeriod + " (" + pfh.ProjectName + " )";

                //if (!string.IsNullOrWhiteSpace(fydid))
                //{
                //    FiscalYearRepo _repoFiscalYear = new FiscalYearRepo();
                //    vm = _repoFiscalYear.SelectAll_FiscalYearDetail(Convert.ToInt32(fydid)).FirstOrDefault();
                //    ViewBag.PeriodName= vm.PeriodName;
                //}

                return View();

            }
            catch (Exception)
            {
                return View();

            }
        }

        public ActionResult _index(JQueryDataTableParamModel param, int id)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);

            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var designationFilter = Convert.ToString(Request["sSearch_3"]);
            var departmentFilter = Convert.ToString(Request["sSearch_4"]);
            var EmployeePFValueFilter = Convert.ToString(Request["sSearch_5"]);
            var EmployeerPFValueFilter = Convert.ToString(Request["sSearch_6"]);
            var EmployeePFValueFrom = 0;
            var EmployeePFValueTo = 0;

            if (EmployeePFValueFilter.Contains('~'))
            {
                EmployeePFValueFrom = EmployeePFValueFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(EmployeePFValueFilter.Split('~')[0]) == true ? Convert.ToInt32(EmployeePFValueFilter.Split('~')[0]) : 0;
                EmployeePFValueTo = EmployeePFValueFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(EmployeePFValueFilter.Split('~')[1]) == true ? Convert.ToInt32(EmployeePFValueFilter.Split('~')[1]) : 0;
            }
            var EmployeerPFValueFrom = 0;
            var EmployeerPFValueTo = 0;
            if (EmployeerPFValueFilter.Contains('~'))
            {
                EmployeerPFValueFrom = EmployeerPFValueFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(EmployeerPFValueFilter.Split('~')[0]) == true ? Convert.ToInt32(EmployeerPFValueFilter.Split('~')[0]) : 0;
                EmployeerPFValueTo = EmployeerPFValueFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(EmployeerPFValueFilter.Split('~')[1]) == true ? Convert.ToInt32(EmployeerPFValueFilter.Split('~')[1]) : 0;
            }

            //Code
            //EmpName 
            //Designation
            //Department 
            //EmployeePFValue
            //EmployeerPFValue

            #endregion Column Search

            PFDetailRepo _repo = new PFDetailRepo();
            List<PFDetailVM> getAllData = new List<PFDetailVM>();
            IEnumerable<PFDetailVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            string[] conditionFields = { "pfd.PFHeaderId" };
            string[] conditionValues = { id.ToString()};
            getAllData = _repo.SelectEmployeeList(conditionFields, conditionValues);

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
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.EmployeePFValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable7 && c.EmployeerPFValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }


            if (codeFilter != "" || empNameFilter != "" || designationFilter != "" || departmentFilter != "" || (EmployeePFValueFilter != "" && EmployeePFValueFilter != "~") || (EmployeerPFValueFilter != "" && EmployeerPFValueFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    && (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    && (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    && (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    && (EmployeePFValueFrom == 0 || EmployeePFValueFrom <= Convert.ToInt32(c.EmployeePFValue))
                                    && (EmployeePFValueTo == 0 || EmployeePFValueTo >= Convert.ToInt32(c.EmployeePFValue))
                                    && (EmployeerPFValueFrom == 0 || EmployeerPFValueFrom <= Convert.ToInt32(c.EmployeerPFValue))
                                    && (EmployeerPFValueTo == 0 || EmployeerPFValueTo >= Convert.ToInt32(c.EmployeerPFValue))
                                );
            }



            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PFDetailVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? c.BasicSalary.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.EmployeePFValue.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.EmployeerPFValue.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = displayedCompanies.Select(c => new[]
            {
                //Convert.ToString(c.Id)
                "", c.Code, c.EmpName, c.Designation, c.Department, c.BasicSalary.ToString(), c.EmployeePFValue.ToString(),
                c.EmployeerPFValue.ToString(),
            });
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult IndexFiscalPeriod(string EmployeeId = "", string fydid = "")
        {
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.fydid = fydid;

            return View();
        }
        public ActionResult _indexFiscalPeriod(JQueryDataTableParamModel param, string EmployeeId = "", string fydid = "")
        {

            PFDetailRepo _repo = new PFDetailRepo();
            List<PFHeaderVM> getAllData = new List<PFHeaderVM>();
            IEnumerable<PFHeaderVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] conditionFields = { "pfd.EmployeeId", "pfd.FiscalYearDetailId" };
            string[] conditionValues = { EmployeeId, fydid };
            getAllData = _repo.SelectFiscalPeriodHeader(conditionFields, conditionValues);


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
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.ProjectName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.FiscalPeriod.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.TotalEmployeeValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.TotalEmployerValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.TotalPF.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable7 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PFHeaderVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.ProjectName :
                sortColumnIndex == 3 && isSortable_3 ? c.PeriodStart :
                sortColumnIndex == 4 && isSortable_4 ? c.TotalEmployeeValue.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.TotalEmployerValue.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.TotalPF.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.Post.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                 c.Id.ToString()
                , c.Code
                , c.ProjectName
                , c.FiscalPeriod
                ,c.TotalEmployeeValue.ToString()
                ,c.TotalEmployerValue.ToString()
                ,c.TotalPF.ToString()
                , c.Post?"Posted":"Not Posted"
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

        public ActionResult PFProcess(string fydid = "", string ProjectId = "")
        {
            if (string.IsNullOrWhiteSpace(fydid))
            {
                return View();
            }
            string[] result = new string[6];
            string mgs = "";
            ShampanIdentityVM vm = new ShampanIdentityVM();

            PFDetailRepo _repo = new PFDetailRepo();
            //vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //vm.LastUpdateBy = identity.Name;
            //vm.LastUpdateFrom = identity.WorkStationIP;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;

            result = _repo.PFProcess(fydid,ProjectId, vm);

            mgs = result[0] + "~" + result[1];
            Session["result"] = mgs;
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Post(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
      
            PFHeaderVM headerVm = new PFHeaderVM();
            headerVm.Id = Convert.ToInt32(ids.Split('~')[0]);
       
            string[] result = new string[6];

            result = _repo.PostHeader(headerVm);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        public JsonResult GetPFDetail(string fydid)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
           
            PFDetailVM vm = new PFDetailVM();

            string[] cFields = {"pfd.FiscalYearDetailId"};
            string[] cValues = {fydid};
            vm = _repo.SelectAll(0, cFields, cValues).FirstOrDefault();
            
            return Json(vm, JsonRequestBehavior.AllowGet);
        }





        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "report").ToString();
            return View();
        }
        [HttpGet]
        public ActionResult ReportView(string fydid = "", string empCodeFrom = "", string empCodeTo = "", string groupBy = "")
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                PFDetailVM vm = new PFDetailVM();
                PFDetailRepo _repo = new PFDetailRepo();

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                string[] conditionFields = { "ve.Code>", "ve.Code<", "pfd.FiscalYearDetailId" };
                string[] conditionValues = { empCodeFrom, empCodeTo, fydid };
                table = _repo.Report(vm);
                ReportHead = "There are no data to Preview for PF Detail";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "PF Detail List";
                }
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtPFDetail";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptPFDetail.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                //doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                //doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
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
        public ActionResult PFContributionDetailsReport(int id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                PFReport report = new PFReport();


                string[] cFields = { "w.Id" };
                string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                string[] conditionFields = { "pfd.PFHeaderId" };
                string[] conditionValues = { id.ToString() };
                var Result = _repo.SelectEmployeeList(conditionFields, conditionValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));


                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                dt.TableName = "dtPFDetail";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptPFContributionDetails.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
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
        public ActionResult PFContributionSummarysReport(int id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                PFReport report = new PFReport();


                string[] cFields = { "w.Id" };
                string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                string[] conditionFields = { "ts.PFHeaderId" };
                string[] conditionValues = { id.ToString() };
                dt = _repo.PFEmployersProvisionsReport(conditionFields, conditionValues);

                //dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));


                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                dt.TableName = "EmployersProvisions";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\PFEmployeeContributionSummary.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
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
        public ActionResult PFContributionDetailsDownload(int id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";

                ReportDocument doc = new ReportDocument();
                System.Data.DataTable dt = new System.Data.DataTable();

                string[] conditionFields = { "pfd.PFHeaderId" };
                string[] conditionValues = { id.ToString() };
                var Result = _repo.SelectEmployeeList(conditionFields, conditionValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));

                string[] DecimalColumnNames = { "BasicSalary", "EmployeePFValue", "EmployeerPFValue" };
                dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);

                var dataView = new DataView(dt);


                dt = dataView.ToTable(true, "Code", "EmpName", "Designation", "Department",
                     "BasicSalary", "EmployeePFValue", "EmployeerPFValue");
                #region Excel


                string StatementName = "PF Employee Contribution Details";

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

                string[] ReportHeaders = new string[] { Line1, Line2, StatementName };

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
                return Redirect("PF/PFDetail/index");
                #endregion


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



        [HttpGet]
        public ActionResult PFReportSummaryDetail(string fydid, string rType, string ProjectId)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                PFReport report = new PFReport();
                fydid = fydid.Split('~')[0];
                ProjectId = fydid.Split('~')[1];
                vm = report.PFReportSummaryDetail(fydid, rType, ProjectId);

                #region Excel
                string filename = "PF Contribution " + rType;
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("PF Contribution " + rType);
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";
                
                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { Line1, Line2, Line3 };

                ExcelSheetFormat(vm.DataTable, workSheet, ReportHeaders);
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


        [HttpGet]
        public ActionResult reportVeiw(string id)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                PFDetailVM PFDetailvm = new PFDetailVM();
                PFReport report = new PFReport();
                string[] cFields = { "pfd.PFHeaderId" };
                string[] cValues = { id};
                PFDetailvm = _repo.SelectAll(0, cFields, cValues).FirstOrDefault();
                vm.PFHeaderId = Convert.ToInt32(PFDetailvm.PFHeaderId);
                vm.Code = PFDetailvm.Code;
                return PartialView("reportVeiw", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        

       [HttpPost]
        public ActionResult PFContributionReport(PFReportVM vm)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                PFReport report = new PFReport();
                //vm.Id = 1;
                string[] cFields = { "PFHeaderId" };
                string[] cValues = { vm.PFHeaderId.ToString() == "0" ? "" : vm.PFHeaderId.ToString() };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                dt = new PFReportRepo().PFContributionReport(vm, cFields, cValues);
                ReportHead = "There are no data to Preview for GL Transaction for Contribution";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Contribution GL Transactions";
                }
                dt.TableName = "dtPFContribution";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptPFContribution.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
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

      
       public ActionResult ImportExportPF()
       {
           SymUserRoleRepo _reposur = new SymUserRoleRepo();
           var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
           Session["permission"] = permission;
           if (permission == "False")
           {
               return Redirect("/PF/Home");
           }
           return View();
       }

       public ActionResult DownloadOtherEarningExcel(string ProjectId, string DepartmentId, string SectionId
         , string DesignationId, string CodeF, string CodeT, int fid = 0, int ETId = 0, string Orderby = null)
       {
           SymUserRoleRepo _reposur = new SymUserRoleRepo();
           DataTable dt = new DataTable();
           string[] result = new string[6];
           try
           {
               var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
               Session["permission"] = permission;
               if (permission == "False")
               {
                   return Redirect("/PF/Home");
               }
               string FileName = "Download.xls";
               string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
               string contentType = MimeMapping.GetMimeMapping(fullPath);
               //string fullPath = @"C:\";
               if (System.IO.File.Exists(fullPath + FileName))
               {
                   System.IO.File.Delete(fullPath + FileName);
               }
               dt = new EmployeeOtherEarningRepo().ExportExcelFilePF(fullPath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, Orderby);
               //exp(dt);
               ExcelPackage excel = new ExcelPackage();
               var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
               workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

               string filename = "PF of"+ "-" + dt.Rows[0]["Fiscal Period"].ToString();
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
               return RedirectToAction("ImportOtherEarning");
           }
           catch (Exception)
           {
               Session["result"] = result[0] + "~" + result[1];
               FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
               return RedirectToAction("ImportOtherEarning");
           }
       }
        







    }
}
