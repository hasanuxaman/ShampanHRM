using SymOrdinary;
using SymRepository.Common;
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
using JQueryDataTables.Models;
using OfficeOpenXml;
using SymRepository.Tax;
using SymViewModel.Tax;
using Newtonsoft.Json;
using OfficeOpenXml.Style;

namespace SymWebUI.Areas.Payroll.Controllers
{
    public class SalaryStructureMatrixController : Controller
    {
        #region Declare

        SalaryStructureMatrixRepo _repo = new SalaryStructureMatrixRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        #endregion Declare

        public ActionResult Index(string currentYear = "", string currentYearPart = "")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_36", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }


            SalaryStructureMatrixVM vm = new SalaryStructureMatrixVM();
            vm.CurrentYear = currentYear;
            vm.CurrentYearPart = currentYearPart;
            return View(vm);
        }


        public ActionResult Create()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_36", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            return View();
        }

        public ActionResult MatrixEffectInfo(string fYear = "")
        {
            return View();

        }
        public ActionResult MatrixEffectCreate(string fYear = "")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_36", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeSalaryStructureVM vm = new EmployeeSalaryStructureVM();
            vm.CurrentYear = fYear;
            return View("MatrixEffectCreate", vm);
        }
        [HttpPost]
        public ActionResult MatrixEffectSave(EmployeeSalaryStructureVM saSamvm)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_36", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            saSamvm.IsArchive = false;
            saSamvm.IsActive = true;
            saSamvm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            saSamvm.CreatedBy = Identity.Name;
            saSamvm.CreatedFrom = Identity.WorkStationIP;
            result = new EmployeeSalaryOtherIncreamentRepo().InsertEmployeeSalaryIncrementMatrix(saSamvm);

            Session["result"] = result[0] + "~" + result[1];

            return RedirectToAction("MatrixEffectCreate");
        }

        public ActionResult MatrixCreate(string fYear = "", string fYearpart = "")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_36", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            SalaryStructureMatrixVM saSamvm = new SalaryStructureMatrixVM();
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            saSamvm.CurrentYear = fYear;
            saSamvm.CurrentYearPart = fYearpart;

            saSamvm.IsArchive = false;
            saSamvm.IsActive = true;
            saSamvm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            saSamvm.LastUpdateBy = Identity.Name;
            saSamvm.LastUpdateFrom = Identity.WorkStationIP;
            result = _repo.MatrixCreate(saSamvm);

            Session["result"] = result[0] + "~" + result[1];

            return RedirectToAction("Create");
        }



        public ActionResult DownloadMatrix(string fYear, string fYearpart = "")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_36", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            string filename = "MatrixSheet" + " -" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

            SalaryStructureMatrixVM saSamvm = new SalaryStructureMatrixVM();
            string[] result = new string[6];
            saSamvm.CurrentYear = fYear;
            saSamvm.CurrentYearPart = fYearpart;

            DataTable dt = _repo.MatrixCreateCheck(saSamvm);

            ExcelPackage excel = new ExcelPackage();
            ////var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            ////workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

            dt.Columns.Remove("CurrentYear");

            #region Top Sheet

            DataRow[] rowsTopSheet = dt.Select("SalaryTypeName = 'Basic'");
            DataTable dtTopSheet = rowsTopSheet.CopyToDataTable();
            dtTopSheet.Columns.Remove("Area");

            var workSheet = excel.Workbook.Worksheets.Add("TopSheet");
            workSheet.Cells[1, 1].LoadFromDataTable(dtTopSheet, true);

            int ColumnCount = 0;
            ColumnCount = dtTopSheet.Columns.Count;

            workSheet.Cells[1, 1, 1, ColumnCount].Style.Font.Bold = true;

            #endregion

            #region Dhaka

            DataRow[] rowsArea = dt.Select("Area = 'Dhaka'");
            DataTable dtArea = rowsArea.CopyToDataTable();

            ////DataView dataViewArea = new DataView(dtArea);
            ////dataViewArea.Sort = "Code ASC";
            ////dtArea = new DataTable();
            ////dtArea = dataViewArea.ToTable();

            dtArea.Columns.Remove("Area");

            workSheet = excel.Workbook.Worksheets.Add("Dhaka");
            //foreach (DataRow item in dtArea.Rows)
            //{
            //workSheet.Cells[1, 1].LoadFromDataReader(item, true);
                
            //}
            workSheet.Cells[1, 1].LoadFromDataTable(dtArea, true);

            #region Insert Row

            workSheet.InsertRow(6, 1);
            workSheet.InsertRow(7, 1);

            workSheet.InsertRow(12, 1);
            workSheet.InsertRow(13, 1);

            workSheet.InsertRow(18, 1);
            workSheet.InsertRow(19, 1);

            workSheet.InsertRow(24, 1);
            workSheet.InsertRow(25, 1);

            workSheet.InsertRow(30, 1);
            workSheet.InsertRow(31, 1);

            workSheet.InsertRow(36, 1);
            //workSheet.InsertRow(41, 1);
            //workSheet.InsertRow(46, 1);
            ////workSheet.InsertRow(51, 1);
            ////workSheet.InsertRow(56, 1);
            ////workSheet.InsertRow(61, 1);

            #endregion

            int colNumber = 0;

            ColumnCount = 0;
            ColumnCount = dtArea.Columns.Count;

            #region Total Calculation

            foreach (DataColumn col in dtArea.Columns)
            {
                colNumber++;

                if (col.DataType == typeof(Int32))
                {
                    ////workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0)";

                    #region Grand Total
                    workSheet.Cells[6, colNumber].Formula = "=Sum(" + workSheet.Cells[2, colNumber].Address + ":" + workSheet.Cells[5, colNumber].Address + ")";
                    workSheet.Cells[12, colNumber].Formula = "=Sum(" + workSheet.Cells[8, colNumber].Address + ":" + workSheet.Cells[11, colNumber].Address + ")";
                    workSheet.Cells[18, colNumber].Formula = "=Sum(" + workSheet.Cells[14, colNumber].Address + ":" + workSheet.Cells[17, colNumber].Address + ")";

                    workSheet.Cells[24, colNumber].Formula = "=Sum(" + workSheet.Cells[21, colNumber].Address + ":" + workSheet.Cells[23, colNumber].Address + ")";
                    workSheet.Cells[30, colNumber].Formula = "=Sum(" + workSheet.Cells[26, colNumber].Address + ":" + workSheet.Cells[29, colNumber].Address + ")";
                    workSheet.Cells[36, colNumber].Formula = "=Sum(" + workSheet.Cells[32, colNumber].Address + ":" + workSheet.Cells[35, colNumber].Address + ")";
                    //workSheet.Cells[36, colNumber].Formula = "=Sum(" + workSheet.Cells[32, colNumber].Address + ":" + workSheet.Cells[35, colNumber].Address + ")";
                    //workSheet.Cells[41, colNumber].Formula = "=Sum(" + workSheet.Cells[37, colNumber].Address + ":" + workSheet.Cells[40, colNumber].Address + ")";
                    //workSheet.Cells[46, colNumber].Formula = "=Sum(" + workSheet.Cells[42, colNumber].Address + ":" + workSheet.Cells[45, colNumber].Address + ")";
                    //workSheet.Cells[51, colNumber].Formula = "=Sum(" + workSheet.Cells[47, colNumber].Address + ":" + workSheet.Cells[50, colNumber].Address + ")";
                    ////workSheet.Cells[56, colNumber].Formula = "=Sum(" + workSheet.Cells[52, colNumber].Address + ":" + workSheet.Cells[55, colNumber].Address + ")";
                    ////workSheet.Cells[61, colNumber].Formula = "=Sum(" + workSheet.Cells[57, colNumber].Address + ":" + workSheet.Cells[60, colNumber].Address + ")";
                    ////workSheet.Cells[66, colNumber].Formula = "=Sum(" + workSheet.Cells[62, colNumber].Address + ":" + workSheet.Cells[65, colNumber].Address + ")";

                    ////workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + ws.Cells[TableHeadRow + 1, colNumber].Address + ":" + ws.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                    #endregion

                }

            }

            #endregion

            #region Row bold

            workSheet.Cells[1, 1, 1, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[6, 1, 6, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[11, 1, 11, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[16, 1, 16, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[21, 1, 21, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[26, 1, 26, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[31, 1, 31, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[36, 1, 36, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[41, 1, 41, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[46, 1, 46, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[51, 1, 51, ColumnCount].Style.Font.Bold = true;
            ////workSheet.Cells[56, 1, 56, ColumnCount].Style.Font.Bold = true;
            ////workSheet.Cells[61, 1, 61, ColumnCount].Style.Font.Bold = true;
            ////workSheet.Cells[66, 1, 66, ColumnCount].Style.Font.Bold = true;

            #endregion

            #region Total text

            workSheet.Cells[6, 1].LoadFromText("Total");
            workSheet.Cells[11, 1].LoadFromText("Total");
            workSheet.Cells[16, 1].LoadFromText("Total");
            workSheet.Cells[21, 1].LoadFromText("Total");
            workSheet.Cells[26, 1].LoadFromText("Total");
            workSheet.Cells[31, 1].LoadFromText("Total");
            workSheet.Cells[36, 1].LoadFromText("Total");
            workSheet.Cells[41, 1].LoadFromText("Total");
            workSheet.Cells[46, 1].LoadFromText("Total");
            workSheet.Cells[51, 1].LoadFromText("Total");
            ////workSheet.Cells[56, 1].LoadFromText("Total");
            ////workSheet.Cells[61, 1].LoadFromText("Total");
            ////workSheet.Cells[66, 1].LoadFromText("Total");

            #endregion

            #endregion

            #region Field

            DataRow[] rowsField = dt.Select("Area <> 'Dhaka'");
            DataTable dtField = rowsField.CopyToDataTable();

            DataView dataView = new DataView(dtField);
            dataView.Sort = "Code ASC";
            dtField = new DataTable();
            dtField = dataView.ToTable();

            dtField.Columns.Remove("Area");

            workSheet = excel.Workbook.Worksheets.Add("Field");
            workSheet.Cells[1, 1].LoadFromDataTable(dtField, true);

            workSheet.InsertRow(6, 1);
            workSheet.InsertRow(11, 1);
            workSheet.InsertRow(16, 1);
            ////workSheet.InsertRow(21, 1);
            ////workSheet.InsertRow(26, 1);
            ////workSheet.InsertRow(31, 1);
            ////workSheet.InsertRow(36, 1);

            colNumber = 0;

            ColumnCount = 0;
            ColumnCount = dtField.Columns.Count;

            #region Total Calculation

            foreach (DataColumn col in dtField.Columns)
            {
                colNumber++;

                if (col.DataType == typeof(Int32))
                {
                    ////workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0)";

                    #region Grand Total
                    workSheet.Cells[6, colNumber].Formula = "=Sum(" + workSheet.Cells[2, colNumber].Address + ":" + workSheet.Cells[5, colNumber].Address + ")";
                    workSheet.Cells[11, colNumber].Formula = "=Sum(" + workSheet.Cells[7, colNumber].Address + ":" + workSheet.Cells[10, colNumber].Address + ")";
                    workSheet.Cells[16, colNumber].Formula = "=Sum(" + workSheet.Cells[12, colNumber].Address + ":" + workSheet.Cells[15, colNumber].Address + ")";
                    workSheet.Cells[21, colNumber].Formula = "=Sum(" + workSheet.Cells[17, colNumber].Address + ":" + workSheet.Cells[20, colNumber].Address + ")";
                    //////workSheet.Cells[26, colNumber].Formula = "=Sum(" + workSheet.Cells[22, colNumber].Address + ":" + workSheet.Cells[25, colNumber].Address + ")";
                    //////workSheet.Cells[31, colNumber].Formula = "=Sum(" + workSheet.Cells[27, colNumber].Address + ":" + workSheet.Cells[30, colNumber].Address + ")";
                    //////workSheet.Cells[36, colNumber].Formula = "=Sum(" + workSheet.Cells[32, colNumber].Address + ":" + workSheet.Cells[35, colNumber].Address + ")";
                    //////workSheet.Cells[41, colNumber].Formula = "=Sum(" + workSheet.Cells[37, colNumber].Address + ":" + workSheet.Cells[40, colNumber].Address + ")";

                    #endregion

                }

            }

            #endregion

            #region Row bold

            workSheet.Cells[1, 1, 1, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[6, 1, 6, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[11, 1, 11, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[16, 1, 16, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[21, 1, 21, ColumnCount].Style.Font.Bold = true;
            ////workSheet.Cells[26, 1, 26, ColumnCount].Style.Font.Bold = true;
            ////workSheet.Cells[31, 1, 31, ColumnCount].Style.Font.Bold = true;
            ////workSheet.Cells[36, 1, 36, ColumnCount].Style.Font.Bold = true;
            ////workSheet.Cells[41, 1, 41, ColumnCount].Style.Font.Bold = true;

            #endregion

            #region Total text

            workSheet.Cells[6, 1].LoadFromText("Total");
            workSheet.Cells[11, 1].LoadFromText("Total");
            workSheet.Cells[16, 1].LoadFromText("Total");
            workSheet.Cells[21, 1].LoadFromText("Total");
            ////workSheet.Cells[26, 1].LoadFromText("Total");
            ////workSheet.Cells[31, 1].LoadFromText("Total");
            ////workSheet.Cells[36, 1].LoadFromText("Total");
            ////workSheet.Cells[41, 1].LoadFromText("Total");

            #endregion


            #endregion

            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }

            return RedirectToAction("Create");

        }



        public ActionResult IndexFiscalYears()
        {
            return View("IndexFiscalPeriod");
        }

        public ActionResult _indexFiscalYears(JQueryDataTableParamModel param)
        {

            SalaryStructureMatrixRepo _repo = new SalaryStructureMatrixRepo();
            List<SalaryStructureMatrixVM> getAllData = new List<SalaryStructureMatrixVM>();
            IEnumerable<SalaryStructureMatrixVM> filteredData;

            getAllData = _repo.SelectFiscalYearMonthlies();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.CurrentYear.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryStructureMatrixVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.CurrentYear.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                 c.CurrentYear.ToString()
                 ,c.CurrentYear.ToString()
                 ,c.CurrentYearPart.ToString()
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

        public ActionResult SalaryStructure(JQueryDataTableParamVM param, string CurrentYear = "", string CurrentYearPart = "")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_36", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            string vCurrentYear = "";

            EmployeeInfoVM empvm = new EmployeeInfoVM();
            List<SalaryStructureMatrixVM> vm = new List<SalaryStructureMatrixVM>();
            if (CurrentYear != "0_0" && CurrentYear != "0" && CurrentYear != "" && CurrentYear != "null" && CurrentYear != null)
            {
                vCurrentYear = CurrentYear;
            }



            vm = _repo.SelectAll(null, null, null, vCurrentYear, CurrentYearPart);
            empvm.SalaryStructureMatrixVM = vm;
            return View(empvm);
            //return PartialView("_salaryStructure", vm);
        }

        //public ActionResult _salaryStructure(JQueryDataTableParamVM param)
        //{
        //    //JQueryDataTableParamVM param; 
        //    string SalaryTypeName = " "; string GradeId = " ";
        //    ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //    var getAllData = _repo.SelectAll(SalaryTypeName, GradeId);
        //    IEnumerable<SalaryStructureMatrixVM> filteredData;
        //    filteredData = getAllData;
        //    var displayedCompanies = filteredData;
        //    var result = from c in displayedCompanies select new[] { 
        //        c.SalaryTypeName, 
        //        c.GradeId, 
        //        c.Step1Amount.ToString(), 
        //        c.Step2Amount.ToString(), 
        //        c.Step3Amount.ToString(), 
        //        c.Step4Amount.ToString(), 
        //        c.Step5Amount.ToString(), 
        //        c.Step6Amount.ToString(), 
        //        c.Step7Amount.ToString(), 
        //        c.Step8Amount.ToString(), 
        //        c.Step9Amount.ToString(), 
        //        c.Step10Amount.ToString(), 
        //        c.Step11Amount.ToString(), 
        //        c.Step12Amount.ToString(), 
        //        c.Step13Amount.ToString(), 
        //        c.Step14Amount.ToString(), 
        //        c.Step15Amount.ToString(), 
        //        c.Step16Amount.ToString(), 
        //        c.Step17Amount.ToString(), 
        //        c.Step18Amount.ToString(), 
        //        c.Step19Amount.ToString(), 
        //        c.Step20Amount.ToString(), 
        //        c.Step21Amount.ToString(), 
        //        c.Step22Amount.ToString(), 
        //        c.Step23Amount.ToString(), 
        //        c.Step24Amount.ToString(), 
        //        c.Step25Amount.ToString(), 
        //        c.Step26Amount.ToString(), 
        //        c.Step27Amount.ToString(), 
        //        c.Step28Amount.ToString(), 
        //        c.Step30Amount.ToString(), 
        //        c.Step30Amount.ToString(), 
        //        Convert.ToString(c.Id)
        //                 };
        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = getAllData.Count(),
        //        iTotalDisplayRecords = filteredData.Count(),
        //        aaData = result
        //    },
        //                JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public ActionResult Edit(SalaryStructureMatrixVM vm)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_36", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            SalaryStructureMatrixVM saSamvm = new SalaryStructureMatrixVM();
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            saSamvm = vm;
            saSamvm.IsArchive = false;
            saSamvm.IsActive = true;
            saSamvm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            saSamvm.LastUpdateBy = Identity.Name;
            saSamvm.LastUpdateFrom = Identity.WorkStationIP;
            result = _repo.Update(vm);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }

        public JsonResult BasicAmount(string grade, string step, string year, string StepSL, string yearpart)
        {
            string amount = "0";
            if (!string.IsNullOrWhiteSpace(grade))
            {
                amount = _repo.BasicAmount(grade, step, year, StepSL, yearpart);
            }

            return Json(amount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SalaryStructureDownload(string currentYear = "", string CurrentYearPart = "")
        {
            try
            {
                string[] result = new string[6];
                bool IsMultiple = false;
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("TopSheet");
                var permission = _reposur.SymRoleSession(identity.UserId, "1_36", "edit").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                string vCurrentYear = "";
                DataTable dt = new DataTable();

                List<SalaryStructureMatrixVM> vm = new List<SalaryStructureMatrixVM>();



                dt = _repo.SelectSalaryStructureMatrixDownload(currentYear, CurrentYearPart);
                //dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(vm));


                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name; // "BRAC EPL STOCK BROKERAGE LIMITED";
                string
                    Line2 = comInfo
                        .Address; // "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "Employee Salary Structure Matrix";
                string filename = "Employee Salary Structure Matrix" + " -" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { Line1, Line2, Line3 };

                //ExcelSheetFormat(dt, workSheet, ReportHeaders);
                //workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

                dt.Columns.Remove("CurrentYear");
                dt.Columns.Remove("CurrentYearPart");

                #region Top Sheet

                DataRow[] rowsTopSheet = dt.Select("SalaryTypeName = 'Basic'");
                DataTable dtTopSheet = rowsTopSheet.CopyToDataTable();
                dtTopSheet.Columns.Remove("Area");

                //////workSheet = excel.Workbook.Worksheets.Add("TopSheet");

                workSheet.Cells[1, 1].LoadFromDataTable(dtTopSheet, true);

                int ColumnCount = 0;
                ColumnCount = dtTopSheet.Columns.Count;

                workSheet.Cells[1, 1, 1, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells["d2:AP15"].Style.Numberformat.Format = "#,##0.00"; // You can change the format as needed

                var borderStyle = ExcelBorderStyle.Thin;

                var startCell = workSheet.Cells["A1"];
                var endCell = workSheet.Cells["AP15"];
                var range = workSheet.Cells[startCell.Address + ":" + endCell.Address];

                // Set borders for the specified range
                range.Style.Border.Top.Style = borderStyle;
                range.Style.Border.Bottom.Style = borderStyle;
                range.Style.Border.Left.Style = borderStyle;
                range.Style.Border.Right.Style = borderStyle;


                //var borderStyle = ExcelBorderStyle.Thin;
                //workSheet.Cells[workSheet.Dimension.Address].Style.Border.Top.Style = borderStyle;
                //workSheet.Cells[workSheet.Dimension.Address].Style.Border.Bottom.Style = borderStyle;
                //workSheet.Cells[workSheet.Dimension.Address].Style.Border.Left.Style = borderStyle;
                //workSheet.Cells[workSheet.Dimension.Address].Style.Border.Right.Style = borderStyle;


                //workSheet.Cells["A1:AP15"].Style.Border.BorderAround(borderStyle);
                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
                workSheet.PrinterSettings.FitToPage = true;
                #endregion

                #region Dhaka

                DataRow[] rowsArea = dt.Select("Area = 'Dhaka'");
                DataTable dtArea = rowsArea.CopyToDataTable();

                ////DataView dataViewArea = new DataView(dtArea);
                ////dataViewArea.Sort = "Code ASC";
                ////dtArea = new DataTable();
                ////dtArea = dataViewArea.ToTable();

                dtArea.Columns.Remove("Area");

                workSheet = excel.Workbook.Worksheets.Add("Dhaka");
                workSheet.Cells[1, 1].LoadFromDataTable(dtArea, true);

                #region Insert Row

                workSheet.InsertRow(6, 1);
                workSheet.InsertRow(7, 1);

                workSheet.InsertRow(12, 1);
                workSheet.InsertRow(13, 1);

                workSheet.InsertRow(18, 1);
                workSheet.InsertRow(19, 1);

                workSheet.InsertRow(24, 1);
                workSheet.InsertRow(25, 1);

                workSheet.InsertRow(30, 1);
                workSheet.InsertRow(31, 1);

                workSheet.InsertRow(36, 1);
                workSheet.InsertRow(37, 1);

                #endregion

                int colNumber = 0;

                ColumnCount = 0;
                ColumnCount = dtArea.Columns.Count;

                #region Total Calculation

                foreach (DataColumn col in dtArea.Columns)
                {
                    colNumber++;

                    if (col.DataType == typeof(decimal))
                    {
                        ////workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0)";

                        #region Grand Total
                        workSheet.Cells[6, colNumber].Formula = "=Sum(" + workSheet.Cells[2, colNumber].Address + ":" + workSheet.Cells[5, colNumber].Address + ")";
                        workSheet.Cells[12, colNumber].Formula = "=Sum(" + workSheet.Cells[8, colNumber].Address + ":" + workSheet.Cells[11, colNumber].Address + ")";
                        workSheet.Cells[18, colNumber].Formula = "=Sum(" + workSheet.Cells[14, colNumber].Address + ":" + workSheet.Cells[17, colNumber].Address + ")";

                        workSheet.Cells[24, colNumber].Formula = "=Sum(" + workSheet.Cells[21, colNumber].Address + ":" + workSheet.Cells[23, colNumber].Address + ")";
                        workSheet.Cells[30, colNumber].Formula = "=Sum(" + workSheet.Cells[26, colNumber].Address + ":" + workSheet.Cells[29, colNumber].Address + ")";
                        workSheet.Cells[36, colNumber].Formula = "=Sum(" + workSheet.Cells[32, colNumber].Address + ":" + workSheet.Cells[35, colNumber].Address + ")";
                        workSheet.Cells[42, colNumber].Formula = "=Sum(" + workSheet.Cells[38, colNumber].Address + ":" + workSheet.Cells[41, colNumber].Address + ")";
                        ////workSheet.Cells[56, colNumber].Formula = "=Sum(" + workSheet.Cells[52, colNumber].Address + ":" + workSheet.Cells[55, colNumber].Address + ")";
                        ////workSheet.Cells[61, colNumber].Formula = "=Sum(" + workSheet.Cells[57, colNumber].Address + ":" + workSheet.Cells[60, colNumber].Address + ")";
                        ////workSheet.Cells[66, colNumber].Formula = "=Sum(" + workSheet.Cells[62, colNumber].Address + ":" + workSheet.Cells[65, colNumber].Address + ")";

                        ////workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + ws.Cells[TableHeadRow + 1, colNumber].Address + ":" + ws.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                        #endregion

                    }

                }

                #endregion

                #region Row bold

                workSheet.Cells[1, 1, 1, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[6, 1, 6, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[12, 1, 12, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[18, 1, 18, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[24, 1, 24, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[30, 1, 30, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[36, 1, 36, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[42, 1, 42, ColumnCount].Style.Font.Bold = true;
                //workSheet.Cells[41, 1, 41, ColumnCount].Style.Font.Bold = true;
                //workSheet.Cells[46, 1, 46, ColumnCount].Style.Font.Bold = true;
                //workSheet.Cells[51, 1, 51, ColumnCount].Style.Font.Bold = true;
                ////workSheet.Cells[56, 1, 56, ColumnCount].Style.Font.Bold = true;
                ////workSheet.Cells[61, 1, 61, ColumnCount].Style.Font.Bold = true;
                ////workSheet.Cells[66, 1, 66, ColumnCount].Style.Font.Bold = true;

                #endregion

                #region Total text

                workSheet.Cells[6, 1].LoadFromText("Total");
                workSheet.Cells[12, 1].LoadFromText("Total");
                workSheet.Cells[18, 1].LoadFromText("Total");
                workSheet.Cells[24, 1].LoadFromText("Total");
                workSheet.Cells[30, 1].LoadFromText("Total");
                workSheet.Cells[36, 1].LoadFromText("Total");
                workSheet.Cells[42, 1].LoadFromText("Total");
                ////workSheet.Cells[56, 1].LoadFromText("Total");
                ////workSheet.Cells[61, 1].LoadFromText("Total");
                ////workSheet.Cells[66, 1].LoadFromText("Total");

                #endregion
                workSheet.Cells["d2:AP42"].Style.Numberformat.Format = "#,##0.00"; // You can change the format as needed

                #endregion

                #region Field

                DataRow[] rowsField = dt.Select("Area <> 'Dhaka'");
                DataTable dtField = rowsField.CopyToDataTable();

                DataView dataView = new DataView(dtField);
                dataView.Sort = "Code ASC";
                dtField = new DataTable();
                dtField = dataView.ToTable();

                dtField.Columns.Remove("Area");

                workSheet = excel.Workbook.Worksheets.Add("Field");
                workSheet.Cells[1, 1].LoadFromDataTable(dtField, true);

                workSheet.InsertRow(6, 1);
                workSheet.InsertRow(7, 1);
                workSheet.InsertRow(12, 1);
                workSheet.InsertRow(13, 1);
                workSheet.InsertRow(18, 1);
                workSheet.InsertRow(19, 1);
                workSheet.InsertRow(24, 1);
                workSheet.InsertRow(25, 1);
                workSheet.InsertRow(30, 1);
                workSheet.InsertRow(31, 1);
                workSheet.InsertRow(36, 1);
                workSheet.InsertRow(37, 1);

                colNumber = 0;

                ColumnCount = 0;
                ColumnCount = dtField.Columns.Count;

                #region Total Calculation

                foreach (DataColumn col in dtField.Columns)
                {
                    colNumber++;

                    if (col.DataType == typeof(decimal))
                    {
                        ////workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0)";

                        #region Grand Total
                        workSheet.Cells[6, colNumber].Formula = "=Sum(" + workSheet.Cells[2, colNumber].Address + ":" + workSheet.Cells[5, colNumber].Address + ")";
                        workSheet.Cells[12, colNumber].Formula = "=Sum(" + workSheet.Cells[8, colNumber].Address + ":" + workSheet.Cells[11, colNumber].Address + ")";
                        workSheet.Cells[18, colNumber].Formula = "=Sum(" + workSheet.Cells[14, colNumber].Address + ":" + workSheet.Cells[17, colNumber].Address + ")";

                        workSheet.Cells[24, colNumber].Formula = "=Sum(" + workSheet.Cells[21, colNumber].Address + ":" + workSheet.Cells[23, colNumber].Address + ")";
                        workSheet.Cells[30, colNumber].Formula = "=Sum(" + workSheet.Cells[26, colNumber].Address + ":" + workSheet.Cells[29, colNumber].Address + ")";
                        workSheet.Cells[36, colNumber].Formula = "=Sum(" + workSheet.Cells[32, colNumber].Address + ":" + workSheet.Cells[35, colNumber].Address + ")";
                        workSheet.Cells[42, colNumber].Formula = "=Sum(" + workSheet.Cells[38, colNumber].Address + ":" + workSheet.Cells[41, colNumber].Address + ")";
                        //////workSheet.Cells[26, colNumber].Formula = "=Sum(" + workSheet.Cells[22, colNumber].Address + ":" + workSheet.Cells[25, colNumber].Address + ")";
                        //////workSheet.Cells[31, colNumber].Formula = "=Sum(" + workSheet.Cells[27, colNumber].Address + ":" + workSheet.Cells[30, colNumber].Address + ")";
                        //////workSheet.Cells[36, colNumber].Formula = "=Sum(" + workSheet.Cells[32, colNumber].Address + ":" + workSheet.Cells[35, colNumber].Address + ")";
                        //////workSheet.Cells[41, colNumber].Formula = "=Sum(" + workSheet.Cells[37, colNumber].Address + ":" + workSheet.Cells[40, colNumber].Address + ")";

                        #endregion

                    }

                }

                #endregion

                #region Row bold


                workSheet.Cells[1, 1, 1, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[6, 1, 6, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[12, 1, 12, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[18, 1, 18, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[24, 1, 24, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[30, 1, 30, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[36, 1, 36, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[42, 1, 42, ColumnCount].Style.Font.Bold = true;

                #endregion

                #region Total text

                workSheet.Cells[6, 1].LoadFromText("Total");
                workSheet.Cells[12, 1].LoadFromText("Total");
                workSheet.Cells[18, 1].LoadFromText("Total");
                workSheet.Cells[24, 1].LoadFromText("Total");
                workSheet.Cells[30, 1].LoadFromText("Total");
                workSheet.Cells[36, 1].LoadFromText("Total");
                workSheet.Cells[42, 1].LoadFromText("Total");

                #endregion
                workSheet.Cells["d2:AP42"].Style.Numberformat.Format = "#,##0.00"; // You can change the format as needed


                #endregion

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
                return RedirectToAction("IndexFiscalYears");
                //return PartialView("_salaryStructure", vm);
            }
            catch (Exception e)
            {
                return RedirectToAction("IndexFiscalYears");
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