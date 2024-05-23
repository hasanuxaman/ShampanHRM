using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.GF;
using SymRepository.Common;
using SymViewModel.GF;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymRepository.PF;
using SymViewModel.PF;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using SymReporting.PF;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System.Data;
using SymWebUI.Areas.PF.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;



namespace SymWebUI.Areas.GF.Controllers
{
    public class GFEmployeeProvisionController : Controller
    {
        public GFEmployeeProvisionController()
        {
            ViewBag.TransType = AreaTypeGFVM.TransType;
        }
        //
        // GET: /GF/GFEmployeeProvision/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        GFEmployeeProvisionRepo _repo = new GFEmployeeProvisionRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string gfHeaderId = "")
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/GF/Home");
            }

            ViewBag.gfHeaderId = gfHeaderId;
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param, string gfHeaderId = "")
        {
            //00     //Id 
            //01     //Code
            //02     //EmpName 
            //03     //Designation  
            //04     //Department
            //05     //ProvisionAmount



            #region Search and Filter Data

            var getAllData = _repo.SelectAll(0, new[] { "gfep.GFHeaderId" }, new[] { gfHeaderId });
            IEnumerable<GFEmployeeProvisionVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Department.ToString().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.ProvisionAmount.ToString().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.IncrementArrear.ToString().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.TotalProvisionAmount.ToString().Contains(param.sSearch.ToLower())
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
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<GFEmployeeProvisionVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.Department.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.ProvisionAmount.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.IncrementArrear.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.TotalProvisionAmount.ToString() :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                //Convert.ToString(c.Id),
                 c.Code
                , c.EmpName
                , c.Designation
                , c.Department
                , c.ProvisionAmount.ToString()
                , c.IncrementArrear.ToString()
                , c.TotalProvisionAmount.ToString()
     
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Process()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            GFEmployeeProvisionVM vm = new GFEmployeeProvisionVM();
            return View(vm);
        }


        [HttpPost]
        public ActionResult Process(string fydid = "", string ProjectId = "")
        {
            string[] result = new string[6];
            string mgs = "";
            GFEmployeeProvisionVM vm = new GFEmployeeProvisionVM();

            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;

            vm.FiscalYearDetailId = Convert.ToInt32(fydid);
            vm.ProjectId = ProjectId;

            result = _repo.Process(vm);

            mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }




        public ActionResult IndexFiscalPeriod(string EmployeeId = "", string fydid = "")
        {
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.fydid = fydid;

            return View();
        }
        public ActionResult _indexFiscalPeriod(JQueryDataTableParamModel param, string EmployeeId = "", string fydid = "")
        {

            GFEmployeeProvisionRepo _repo = new GFEmployeeProvisionRepo();
            List<GFHeaderVM> getAllData = new List<GFHeaderVM>();
            IEnumerable<GFHeaderVM> filteredData;
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
                       || isSearchable4 && c.ProvisionAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.IncrementArrear.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.TotalProvisionAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<GFHeaderVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.ProjectName :
                sortColumnIndex == 3 && isSortable_3 ? c.PeriodStart :
                sortColumnIndex == 4 && isSortable_4 ? c.ProvisionAmount.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.IncrementArrear.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.TotalProvisionAmount.ToString() :
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
                ,c.ProvisionAmount.ToString()
                ,c.IncrementArrear.ToString()
                ,c.TotalProvisionAmount.ToString()
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




        [Authorize(Roles = "Admin")]
        public JsonResult Post(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();

            GFHeaderVM headerVm = new GFHeaderVM();
            headerVm.Id = Convert.ToInt32(ids.Split('~')[0]);

            string[] result = new string[6];

            result = _repo.PostHeader(headerVm);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GFEmployeeProvisionsReport(int id)

        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";

                ReportDocument doc = new ReportDocument();
                System.Data.DataTable dt = new System.Data.DataTable();

                string[] conditionFields = { "h.Id" };
                string[] conditionValues = { id.ToString() };
                dt = _repo.GFEmployeeProvisionsReport(conditionFields, conditionValues);

             


                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                dt.TableName = "dtPFDetail";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\GF\\GFEmployeeProvisions.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                //doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";

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
        public ActionResult GFEmployersProvisionsReport(int id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";

                ReportDocument doc = new ReportDocument();
                System.Data.DataTable dt = new System.Data.DataTable();

                string[] conditionFields = { "ts.GFHeaderId" };
                string[] conditionValues = { id.ToString() };
                dt = _repo.GFEmployersProvisionsReport(conditionFields, conditionValues);




                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                dt.TableName = "EmployersProvisions";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\GF\\GFEmployersProvisionsSummary.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                //doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";

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
        public ActionResult GFEmployeeProvisionsDownload(int id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";

                ReportDocument doc = new ReportDocument();
                System.Data.DataTable dt = new System.Data.DataTable();

                string[] conditionFields = { "h.Id" };
                string[] conditionValues = { id.ToString() };
                dt = _repo.GFEmployeeProvisionsReport(conditionFields, conditionValues);
                var dataView = new DataView(dt);


                dt = dataView.ToTable(true,  "Code", "EmpName","Designation","Department",
                     "BasicSalary", "EmployeePFValue", "IncrementArrear", "JobMonth", "Total");
                #region Excel


                string StatementName = "GF Employers Provisions Details";

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

                string[] ReportHeaders = new string[] {  Line1, Line2, StatementName};

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
                return Redirect("GF/GFEmployeeProvision/index");
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

        private void ExcelSheetFormat(System.Data.DataTable dt, ExcelWorksheet workSheet, string[] ReportHeaders)
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
    }
}
