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
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Excel;
using OfficeOpenXml;
using System.Configuration;
using SymViewModel.Enum;
using SymRepository.Enum;
using OfficeOpenXml.Style;

namespace SymWebUI.Areas.Payroll.Controllers
{
    public class EmployeeEarningLeaveController : Controller
    {
        //
        // GET: /Payroll/EmployeeOtherDeduction/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        SalaryEarningLeaveRepo _eaRepo;

        //#region Action Methods
        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_39", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            return View();
        }

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            SalaryEarningLeaveRepo _eaRepo = new SalaryEarningLeaveRepo();

            #region Column Search

            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var PeriodNameFilter = Convert.ToString(Request["sSearch_1"]);
            var RemarksFilter = Convert.ToString(Request["sSearch_2"]);

            #endregion Column Search

            #region Search and Filter Data

            var getAllData = _eaRepo.GetPeriodname();
            IEnumerable<EmployeeEarningLeaveVM> filteredData;
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
                    .Where(c => (PeriodNameFilter == "" || c.Code.ToLower().Contains(PeriodNameFilter.ToLower()))
                                &&
                                (RemarksFilter == "" || c.EmpName.ToLower().Contains(RemarksFilter.ToLower()))
                    );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeEarningLeaveVM, string> orderingFunction = (
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
                select new[]
                {
                    Convert.ToString(c.Id), c.PeriodName, c.Remarks
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
            var permission = _reposur.SymRoleSession(identity.UserId, "1_39", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            EmployeeInfoVM vm = new EmployeeInfoVM();
            SalaryEarningLeaveRepo arerepo = new SalaryEarningLeaveRepo();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeEarningLeaveVM DeductionVM = new EmployeeEarningLeaveVM();
            if (id != "0")
            {
                DeductionVM = arerepo.SelectById(id); //find emp code
                vm = repo.SelectById(DeductionVM.EmployeeId);
                vm.FiscalYearDetailId = DeductionVM.FiscalYearDetailId;
                vm.EmployeeEarningLeaveVM = DeductionVM;
            }

            Session["empid"] = id;

            return View(vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(string btn, EmployeeInfoVM empVM)
        {
            EmployeeEarningLeaveVM vm = new EmployeeEarningLeaveVM();
            string[] result = new string[6];
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm = empVM.EmployeeEarningLeaveVM;

                if (!string.IsNullOrEmpty(empVM.SalaryPeriodId))
                {
                    vm.SalaryPreiodId = Convert.ToInt32(empVM.SalaryPeriodId);
                }

                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.DeductionDate = new FiscalYearRepo().FiscalPeriodStartDate(vm.FiscalYearDetailId);
                if (vm.FiscalYearDetailId == 0)
                {
                    Session["result"] = "Fail~Fiscal Year Not Exist on this Period";
                    FileLogger.Log(
                        result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine +
                        result[5].ToString(), this.GetType().Name,
                        result[4].ToString() + Environment.NewLine + result[3].ToString());
                    return RedirectToAction("Index");
                }

                if (btn.ToLower() != "save")
                {
                    vm.DeductionAmount = 0;
                }

                result = new SalaryEarningLeaveRepo().Insert(vm);
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
            var permission = _reposur.SymRoleSession(identity.UserId, "1_39", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            SalaryEarningLeaveRepo arerepo = new SalaryEarningLeaveRepo();
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var tt = arerepo.SelectAll(null, FID).FirstOrDefault();
            if (tt != null)
            {
                ViewBag.periodName = tt.PeriodName;
            }

            ViewBag.Id = FID;
            return View();
        }

        public ActionResult _EmployeeOtherDeductionDetails(JQueryDataTableParamModel param, int FID)
        {
            #region Column Search

            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var EmployeeNameFilter = Convert.ToString(Request["sSearch_2"]);
            var DeductionAmountFilter = Convert.ToString(Request["sSearch_3"]);
            var BasicSalaryFilter = Convert.ToString(Request["sSearch_4"]);
            var GrossSalaryFilter = Convert.ToString(Request["sSearch_5"]);
            var BasicamountFrom = 0;
            var BasicamountTo = 0;
            if (BasicSalaryFilter.Contains('~'))
            {
                BasicamountFrom = BasicSalaryFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(BasicSalaryFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(BasicSalaryFilter.Split('~')[0])
                        : 0;
                BasicamountTo = BasicSalaryFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(BasicSalaryFilter.Split('~')[1]) == true
                        ?
                        Convert.ToInt32(BasicSalaryFilter.Split('~')[1])
                        : 0;
            }

            var GrossamountFrom = 0;
            var GrossamountTo = 0;
            if (GrossSalaryFilter.Contains('~'))
            {
                GrossamountFrom = GrossSalaryFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(GrossSalaryFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(GrossSalaryFilter.Split('~')[0])
                        : 0;
                GrossamountTo = GrossSalaryFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(GrossSalaryFilter.Split('~')[1]) == true
                        ?
                        Convert.ToInt32(GrossSalaryFilter.Split('~')[1])
                        : 0;
            }

            var amountFrom = 0;
            var amountTo = 0;
            if (DeductionAmountFilter.Contains('~'))
            {
                amountFrom = DeductionAmountFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(DeductionAmountFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(DeductionAmountFilter.Split('~')[0])
                        : 0;
                amountTo = DeductionAmountFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(DeductionAmountFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(DeductionAmountFilter.Split('~')[0])
                        : 0;
            }

            #endregion

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryEarningLeaveRepo arerepo = new SalaryEarningLeaveRepo();
            var getAllData = arerepo.SelectAll(null, FID);
            IEnumerable<EmployeeEarningLeaveVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData
                    .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable3 && c.DeductionAmount.ToString().ToLower()
                                    .Contains(param.sSearch.ToLower())
                                || isSearchable4 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                                || isSearchable5 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #region Column Filtering

            if (codeFilter != "" || EmployeeNameFilter != "" || (BasicSalaryFilter != "" && BasicSalaryFilter != "~") ||
                (GrossSalaryFilter != "" && GrossSalaryFilter != "~") ||
                (DeductionAmountFilter != "" && DeductionAmountFilter != "~"))
            {
                filteredData = filteredData.Where(c =>
                    (codeFilter == "" || c.EmpName.ToLower().Contains(codeFilter.ToLower()))
                    && (EmployeeNameFilter == "" || c.EmpName.ToLower().Contains(EmployeeNameFilter.ToLower()))
                    && (BasicamountFrom == 0 || BasicamountFrom <= Convert.ToInt32(c.BasicSalary))
                    && (BasicamountTo == 0 || BasicamountTo >= Convert.ToInt32(c.BasicSalary))
                    && (GrossamountFrom == 0 || GrossamountFrom <= Convert.ToInt32(c.GrossSalary))
                    && (GrossamountTo == 0 || GrossamountTo >= Convert.ToInt32(c.GrossSalary))
                    && (amountFrom == 0 || amountFrom <= Convert.ToInt32(c.DeductionAmount))
                    && (amountTo == 0 || amountTo >= Convert.ToInt32(c.DeductionAmount))
                );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeEarningLeaveVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.DeductionAmount.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.BasicSalary.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.GrossSalary.ToString() :
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
                    Convert.ToString(c.Id), c.Code, c.EmpName, c.DeductionAmount.ToString(), c.BasicSalary.ToString(),
                    c.GrossSalary.ToString()
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
        public ActionResult SingleEmployeeEarningLeaveEdit(string OtherDeductionId)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_39", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            EmployeeEarningLeaveVM empotherDeductionvm = new EmployeeEarningLeaveVM();
            SalaryEarningLeaveRepo arerepo = new SalaryEarningLeaveRepo();
            if (!string.IsNullOrWhiteSpace(OtherDeductionId))
                empotherDeductionvm = arerepo.SelectById(OtherDeductionId);
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            if (!string.IsNullOrWhiteSpace(OtherDeductionId) && !string.IsNullOrWhiteSpace(empotherDeductionvm.Id))
            {
                vm = repo.SelectById(empotherDeductionvm.EmployeeId);
            }

            vm.EmployeeEarningLeaveVM = empotherDeductionvm;
            vm.EmployeeEarningLeaveVM.PunishmentFromBasic =
                new SettingRepo().settingValue("Deduction", "PunishmentFromBasic");
            vm.GrossSalary = vm.EmployeeEarningLeaveVM.GrossSalary;
            vm.BasicSalary = vm.EmployeeEarningLeaveVM.BasicSalary;
            vm.EmployeeEarningLeaveVM.Id = OtherDeductionId;
            Session["empid"] = empotherDeductionvm.Id;
            vm.FiscalYearDetailId = Convert.ToInt32(empotherDeductionvm.FiscalYearDetailId);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ImportEarningLeave()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_39", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult ImportEmployeeEarningLeaveExcel(HttpPostedFileBase file, int FYDId = 0,
            int SFiscalPeriodDetailId = 0)
        {
            string[] result = new string[6];
            try
            {
                byte[] imageSize = new byte[file.ContentLength];
                //Stream stream = file.InputStream;
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
                ShampanIdentityVM vm = new ShampanIdentityVM();
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                result = new SalaryEarningLeaveRepo().ImportExcelFile(fullPath, file.FileName, vm, null, null, FYDId,
                    SFiscalPeriodDetailId);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportEarningLeave");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportEarningLeave");
            }
        }

        public ActionResult DownloadEmployeeEarningLeaveExcel(HttpPostedFileBase file, string ProjectId,
            string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0,
            int DTId = 0, string Orderby = null)
        {
            string[] result = new string[6];
            DataTable dt = new DataTable();
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_39", "index").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                //EarningDeductionTypeRepo edtRepo = new EarningDeductionTypeRepo();
                //string edtvm = edtRepo.SelectById(DTId).Name;

                string FileName = "EmployeeEarningLeave.xlsx";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }

                //ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Tinned Goods");
                dt = new SalaryEarningLeaveRepo().ExportExcelFile(fullPath, FileName, ProjectId, DepartmentId,
                    SectionId, DesignationId, CodeF, CodeT, fid, Orderby);
                //exp(dt);

                string filename = "EmployeeEarningLeave" + "-" + dt.Rows[0]["Fiscal Period"].ToString();
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment;  filename=" + FileName);
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                result[0] = "Success";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportEarningLeave");
            }
            catch (Exception e)
            {
                Session["result"] = result[0] + "~" + result[1];
                //  FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportEarningLeave");
            }
        }

        private void exp(DataTable dt1)
        {
            DataGridView dgv1 = new DataGridView();
            dgv1.DataSource = dt1;
            string data = null;
            DataTable dt = new DataTable();
            dt = dt1;
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            int j = 1;
            int rowcount = j;
            int startRow = 7;
            foreach (DataRow datarow in dt.Rows)
            {
                rowcount += 1;
                for (int i = 1; i <= dt.Columns.Count; i++)
                {
                    if (rowcount == j + 1)
                    {
                        xlWorkSheet.Cells[j, i] = dt.Columns[i - 1].ColumnName;
                    }

                    xlWorkSheet.Cells[rowcount, i] = datarow[i - 1].ToString();
                }

                startRow++;
            }

            Microsoft.Office.Interop.Excel.Range Company = xlWorkSheet.get_Range("A1", "S1");
            Microsoft.Office.Interop.Excel.Range range = xlWorkSheet.get_Range("A1", "S" + rowcount);
            range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            range.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(153, 153, 153));
            xlWorkSheet.get_Range("A7", "S7").Interior.Color =
                System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.AliceBlue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            MessageBox.Show("Excel file Save Successfully");
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        public ActionResult DetailCreate(string empcode = "", string btn = "current", int FiscalYearDetailId = 0,
            string edType = "0", string id = "0", int SalaryPreiodId = 0)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_39", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            string EmployeeId = "";
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            SalaryEarningLeaveRepo arerepo = new SalaryEarningLeaveRepo();
            EmployeeEarningLeaveVM DeductionVM = new EmployeeEarningLeaveVM();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrWhiteSpace(Session["empid"] as string) && Session["empid"] as string != "0")
            {
                string empid = Session["empid"] as string;
                DeductionVM = arerepo.SelectById(empid); //find emp code
                vm = repo.SelectById(DeductionVM.EmployeeId);
                vm.EmployeeEarningLeaveVM = DeductionVM;
                vm.GrossSalary = vm.EmployeeEarningLeaveVM.GrossSalary;
                vm.BasicSalary = vm.EmployeeEarningLeaveVM.BasicSalary;
                vm.EmployeeEarningLeaveVM.PunishmentFromBasic =
                    new SettingRepo().settingValue("Deduction", "PunishmentFromBasic");
                if (DeductionVM.FiscalYearDetailId > 0)
                {
                    vm.EmployeeEarningLeaveVM.DaysOfMonth =
                        new FiscalYearRepo().SelectDaysOfMonth(Convert.ToInt32(DeductionVM.FiscalYearDetailId));
                }

                Session["empid"] = "";
                // find exist Deduction date
            }
            else if (id != "0")
            {
                DeductionVM = arerepo.SelectById(id); //find emp code
                vm = repo.SelectById(DeductionVM.EmployeeId);
                vm.EmployeeEarningLeaveVM = DeductionVM;


                // find exist Deduction date
            }
            else
            {
                vm = repo.SelectEmpForSearch(empcode, btn);
                if (SalaryPreiodId != 0 && empcode.ToLower() != "select")
                {
                    var salary = arerepo.SelectEmployeeBasicSalary(vm.Id, SalaryPreiodId.ToString());
                    vm.BasicSalary = salary.BasicSalary;
                    vm.GrossSalary = salary.GrossSalary;
                }

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
                    DeductionVM = arerepo.SelectByIdandFiscalyearDetail(vm.Id, FiscalYearDetailId, edType,
                        SalaryPreiodId.ToString());
                    //DeductionVM = arerepo.SelectAll(vm.Id, Convert.ToInt32(FiscalYearDetailId), Convert.ToInt32(edType)).FirstOrDefault();
                    DeductionVM.FiscalYearDetailId = Convert.ToInt32(FiscalYearDetailId);
                    //DeductionVM.DeductionTypeId = Convert.ToInt32(edType);
                }

                if (FiscalYearDetailId == 0)
                {
                    FiscalYearDetailId = 0;
                }

                //svms = arerepo.SingleEmployeeEntry(EmployeeId, FiscalYearDetailId);
                vm.EmployeeEarningLeaveVM = DeductionVM;
                vm.EmployeeEarningLeaveVM.EmployeeId = EmployeeId;
                vm.EmployeeEarningLeaveVM.FiscalYearDetailId = FiscalYearDetailId;
                vm.EmployeeEarningLeaveVM.SalaryPreiodId = SalaryPreiodId;

                #region Get Setting Value

                //if (edType == "4")
                //{
                vm.EmployeeEarningLeaveVM.PunishmentFromBasic =
                    new SettingRepo().settingValue("Deduction", "PunishmentFromBasic");
                if (FiscalYearDetailId > 0)
                {
                    vm.EmployeeEarningLeaveVM.DaysOfMonth =
                        new FiscalYearRepo().SelectDaysOfMonth(Convert.ToInt32(FiscalYearDetailId));
                }
                //}

                #endregion
            }

            return PartialView("_detailCreate", vm);
        }

        public ActionResult Delete(string ids)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_39", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            EmployeeEarningLeaveVM DeductionVM = new EmployeeEarningLeaveVM();
            SalaryEarningLeaveRepo odRepo = new SalaryEarningLeaveRepo();
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            DeductionVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            DeductionVM.LastUpdateBy = identity.Name;
            DeductionVM.LastUpdateFrom = identity.WorkStationIP;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = odRepo.Delete(DeductionVM, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public ActionResult SalaryEarningLeaveReport(string ProjectId, string DepartmentId, string SectionId,
            string DesignationId, string CodeF
            , string Orderby, string CodeT, string view, string rptPG1, string rptPG2, int fid = 0, int fidTo = 0,
            int DTId = 0)
        {
            try
            {
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
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
                string projectParam = "[All]";
                string deptParam = "[All]";
                string secParam = "[All]";
                string desigParam = "[All]";
                string codeFParam = "[All]";
                string codeTParam = "[All]";
                string fyParam = "[All]";
                string fyToParam = "[All]";
                string dtParam = "[All]";
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

                if (DTId != 0)
                {
                    EarningDeductionTypeRepo edRepo = new EarningDeductionTypeRepo();
                    dtParam = edRepo.SelectById(DTId).Name;
                }

                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }

                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }

                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }

                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
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

                if (rptPG1 == "Fiscal Period")
                    rptPG1 = "FP";
                else if (rptPG1 == "Employee Name")
                    rptPG1 = "EN";
                else if (rptPG1 == "Deduction Type")
                    rptPG1 = "DT";
                if (rptPG2 == "Fiscal Period")
                    rptPG2 = "FP";
                else if (rptPG2 == "Employee Name")
                    rptPG2 = "EN";
                else if (rptPG2 == "Deduction Type")
                    rptPG2 = "DT";
                ReportDocument doc = new ReportDocument();
                List<EmployeeEarningLeaveVM> getAllData = new List<EmployeeEarningLeaveVM>();
                SalaryEarningLeaveRepo repo = new SalaryEarningLeaveRepo();
                getAllData = repo.SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId, vDesignationId,
                    vCodeF, vCodeT, DTId, Orderby);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Other Deduction";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Other Deduction List";
                }

                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                DataSet ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtEmployeeOtherEarningDeduction";
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory +
                              @"Files\ReportFiles\Payroll\PayrollEntry\rptEmployeeOtherDeduction.rpt";
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
                doc.DataDefinition.FormulaFields["dtParam"].Text = "'" + dtParam + "'";
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

        public FileStreamResult Export()
        {
            var searchResults = new SalaryEarningLeaveRepo().GetPeriodname();
            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=Export.csv");
            var sw = new StreamWriter(new MemoryStream());
            sw.WriteLine("\"PeriodName\",\"PeriodName\",\"PeriodName\"");
            foreach (var line in searchResults.ToList())
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\"",
                    line.PeriodName,
                    line.PeriodName,
                    line.PeriodName));
            }

            sw.BaseStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(sw.BaseStream, "text/csv");
        }

        public ActionResult _rptIndexPartial(string ProjectId, string DepartmentId, string SectionId,
            string DesignationId
            , string CodeF, string CodeT, int fid = 0, int fidTo = 0, int DTId = 0, string Orderby = null)
        {
            EmployeeEarningLeaveVM vm = new EmployeeEarningLeaveVM();
            vm.ProjectId = ProjectId;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.DesignationId = DesignationId;
            vm.CodeF = CodeF;
            vm.CodeT = CodeT;
            vm.FiscalYearDetailId = fid;
            vm.fidTo = fidTo;
            //vm.DeductionTypeId = DTId;
            vm.Orderby = Orderby;
            return PartialView("_rptIndex", vm);
        }

        public ActionResult _rptIndex(JQueryDataTableParamVM param, string ProjectId, string DepartmentId,
            string SectionId, string DesignationId
            , string CodeF, string CodeT, int fid = 0, int fidTo = 0, int DTId = 0, string Orderby = null)
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
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                {
                    vProjectId = ProjectId;
                }

                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                }

                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                {
                    vSectionId = SectionId;
                }

                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
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

            SalaryEarningLeaveRepo _repo = new SalaryEarningLeaveRepo();
            var getAllData = _repo.SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                vDesignationId, vCodeF, vCodeT, DTId, Orderby);
            IEnumerable<EmployeeEarningLeaveVM> filteredData;
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
                                || isSearchable9 && c.DeductionAmount.ToString().ToLower()
                                    .Contains(param.sSearch.ToLower())
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
            Func<EmployeeEarningLeaveVM, string> orderingFunction = (c =>
                sortColumnIndex == 0 && isSortable_0 ? c.PeriodStart :
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? c.Section :
                sortColumnIndex == 6 && isSortable_6 ? c.Project :
                sortColumnIndex == 7 && isSortable_7 ? c.BasicSalary.ToString() :
                sortColumnIndex == 8 && isSortable_8 ? c.GrossSalary.ToString() :
                sortColumnIndex == 9 && isSortable_9 ? c.DeductionAmount.ToString() :
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
                    c.PeriodName, c.Code, c.EmpName, c.Designation, c.Department, c.Section, c.Project,
                    c.BasicSalary.ToString(), c.GrossSalary.ToString(), c.DeductionAmount.ToString()
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

        public ActionResult DownloadOtherDeductionExcel1(HttpPostedFileBase file, string ProjectId, string DepartmentId,
            string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, int DTId = 0,
            string Orderby = null)
        {
            string[] result = new string[6];
            DataTable dt = new DataTable();
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_39", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                string FileName = "DownloadSymphoni.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }

                dt = new SalaryEarningLeaveRepo().ExportExcelFile(fullPath, FileName, ProjectId, DepartmentId,
                    SectionId, DesignationId, CodeF, CodeT, fid, Orderby);
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + FileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    using (System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw))
                    {
                        GridView grid = new GridView();
                        grid.DataSource = dt;
                        grid.DataBind();
                        grid.RenderControl(htw);
                        Response.Write(sw.ToString());
                    }
                }

                Response.End();
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportOtherDeduction");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(
                    result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine +
                    result[5].ToString(), this.GetType().Name,
                    result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportOtherDeduction");
            }
        }

        public ActionResult EarningLeaveProcess()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_54", "process").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            return View();
        }

        public ActionResult EarningLeaveReport(string ProjectId, string DepartmentId, string SectionId,
            string DesignationId, string CodeF, string CodeT
            , string Orderby, string view, int fid = 0, int fidTo = 0, string SheetName = "")
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

                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }

                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }

                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }

                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
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

                #endregion Variables

                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();
                string ReportFileName = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                SalaryEarningLeaveRepo _repo = new SalaryEarningLeaveRepo();
                //var BranchId = Convert.ToInt32(identity.BranchId);
                //var getAllData = _repo.SelectAll(BranchId);
                List<EmployeeEarningLeaveVM> getAllData = new List<EmployeeEarningLeaveVM>();
                string ReportHead = "";

                if (SheetName == "EarnLeaveSheet2")
                {
                    getAllData = _repo.SelectAllForReportSummary(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                        vDesignationId, vCodeF, vCodeT, 0, Orderby);

                    ReportHead =
                        "There are no data to Preview for Employee Earning leave Summary(Designation) Statement";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Employee Earned Leave Summary(Designation) Statement";
                    }
                }

                else
                {
                    getAllData = _repo.SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                        vDesignationId, vCodeF, vCodeT, 0, Orderby);
                    ReportHead = "There are no data to Preview for Employee Earned Leave";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Employee Earned Leave Statement";
                    }
                }


                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                DataSet ds = new DataSet();
                ds.Tables.Add(table);

                string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string))
                    .ToString();

                #region EnumReport

                if (CompanyName.ToUpper() == "TIB") //tib
                {
                    //string[] conFields = { "ReportType", "ReportId" };
                    //string[] conValues = { "BonusSheet", SheetName }; ////"Salary Certificate"
                    //enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);

                    //EnumReportVM varEnumReportVM = enumReportVMs.FirstOrDefault();
                    if (SheetName == "EarnLeaveSheet2")
                    {
                        ReportFileName = "rptEmployeeEarningLeave_TIB_Summary";
                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" +
                                      ReportFileName + ".rpt";
                    }
                    else
                    {
                        ReportFileName = "rptEmployeeEarningLeave_TIB";
                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" +
                                      ReportFileName + ".rpt";
                    }

                    //string ReportName = varEnumReportVM.Name;
                    //ds.Tables[0].TableName = "TIBSalary";
                }
                else
                {
                    ReportFileName = "rptEmployeeEarningLeave_TIB";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" +
                                  ReportFileName + ".rpt";
                }

                #endregion

                ds.Tables[0].TableName = "dtEarningLeave";

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                //doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = doc.DataDefinition.FormulaFields;

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
                FormulaField(doc, crFormulaF, "CompanyName", cvm.Name);              

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public ActionResult EarningLeaveReportSummary(string ProjectId, string DepartmentId, string SectionId,
            string DesignationId, string CodeF, string CodeT
            , string Orderby, string view, int fid = 0, int fidTo = 0, string SheetName = "")
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

                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }

                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }

                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }

                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
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

                #endregion Variables

                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();
                string ReportFileName = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                SalaryEarningLeaveRepo _repo = new SalaryEarningLeaveRepo();
                //var BranchId = Convert.ToInt32(identity.BranchId);
                //var getAllData = _repo.SelectAll(BranchId);
                List<EmployeeEarningLeaveVM> getAllData = new List<EmployeeEarningLeaveVM>();
                getAllData = _repo.SelectAllForReportSummary(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                    vDesignationId, vCodeF, vCodeT, 0, Orderby);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Employee Earned Leave";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Employee Earned Leave Statement";
                }

                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                DataSet ds = new DataSet();
                ds.Tables.Add(table);

                string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string))
                    .ToString();

                #region EnumReport

                if (CompanyName.ToUpper() == "TIB") //tib
                {
                    //string[] conFields = { "ReportType", "ReportId" };
                    //string[] conValues = { "BonusSheet", SheetName }; ////"Salary Certificate"
                    //enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);

                    //EnumReportVM varEnumReportVM = enumReportVMs.FirstOrDefault();
                    ReportFileName = "rptEmployeeEarningLeave_TIB_Summary";
                    //string ReportName = varEnumReportVM.Name;
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" +
                                  ReportFileName + ".rpt";
                    //ds.Tables[0].TableName = "TIBSalary";
                }
                else
                {
                }

                #endregion

                ds.Tables[0].TableName = "dtEarningLeave";


                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                //doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = doc.DataDefinition.FormulaFields;

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
            catch (Exception)
            {
                throw;
            }
        }

        public void FormulaField(ReportDocument objrpt, FormulaFieldDefinitions crFormulaF, string fieldName,
            string fieldValue)
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

        public ActionResult EarningLeaveExcel(string ProjectId, string DepartmentId, string SectionId,
            string DesignationId, string CodeF, string CodeT
            , string Orderby, string view, int fid = 0, int fidTo = 0, string SheetName = "", string paymentDay = "")
        {
            try
            {
                #region Variables

                string[] result = new string[6];
                bool IsMultiple = false;
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

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

                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }

                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }

                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }

                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
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

                #endregion Variables

                if (fid != fidTo)
                {
                    IsMultiple = true;
                }

                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name; // "BRAC EPL STOCK BROKERAGE LIMITED";
                string
                    Line2 = comInfo
                        .Address; // "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "Statement of Staff Earned Leave";

                string filename = "";

                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

                SalaryEarningLeaveRepo _repo = new SalaryEarningLeaveRepo();

                List<EmployeeEarningLeaveVM> getAllData = new List<EmployeeEarningLeaveVM>();
                if (SheetName == "EarnLeaveSheet2")
                {
                    getAllData = _repo.SelectAllForReportSummary(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                        vDesignationId, vCodeF, vCodeT, 0, Orderby);
                }
                else
                {
                    getAllData = _repo.SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                        vDesignationId, vCodeF, vCodeT, 0, Orderby, SheetName);
                }

                DataTable table = new DataTable();
                DataTable dt = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                table = Ordinary.DtColumnNameChange(table, "DeductionAmount", "ELAmount");
                var dataView = new DataView(table);
                dt = table.Copy();

                string strSort = "SectionOrder";
                dataView.Sort = strSort;
                if (SheetName == "EarnLeaveSheet2")
                {

                    dt = dataView.ToTable(true, "Designation", "Section", "Project", "BasicSalary", "GrossSalary",
                        "HouseRent", "Medical"
                        , "TransportAllowance", "Stamp", "Days", "ELAmount");
                    if (IsMultiple)
                    {
                        Line3 = "Employee Earned Leave Summary -" + fyParam + " To " + fyToParam;
                    }

                    filename = "Employee Earned Leave Summary " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmm");
                }

                if (SheetName == "EarnLeaveSheet1")
                {
                    dt = dataView.ToTable(true, "Code", "EmpName", "Designation", "Department", "Section", "Project",
                        "StepName", "Grade", "BasicSalary", "GrossSalary", "HouseRent", "Medical"
                        , "TransportAllowance", "Stamp", "ELAmount", "Days", "DeductionDate", "Remarks");
                    if (IsMultiple)
                    {
                        Line3 = "Statement of Staff Earn Leave -" + fyParam + " To " + fyToParam;
                    }

                    filename = "Employee Earned Leave " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmm");
                }

                if (SheetName == "EarnLeaveSheet3")
                {
                    dt.Columns.Remove("ELAmount");
                    dt = Ordinary.DtColumnNameChange(dt, "NetDeductionAmount", "ELAmount");

                    string[] DecimalColumnNames = { "ELAmount" };


                    // dt = Ordinary.DtSetColumnsOrder(dt, ShortColumnNames);
                    dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);
                    string DebitAccountNo = new SettingRepo().settingValue("Salary", "DebitA/CNo");

                    dt.Columns.Add(new DataColumn("Reason") { DefaultValue = "EL for " + fyParam + " To " + fyToParam });
                    dt.Columns.Add(new DataColumn("PayeeAccType (Add1)") { DefaultValue = "Savings" });
                    dt.Columns.Add(new DataColumn("Debit A/C No.") { DefaultValue = DebitAccountNo });
                    dt.Columns.Add(new DataColumn("Payment Date") { DefaultValue = paymentDay });

                    dt = Ordinary.DtColumnNameChange(dt, "Code", "Customer Reference (16)");
                    dt = Ordinary.DtColumnNameChange(dt, "EmpName", "Payee Name");
                    dt = Ordinary.DtColumnNameChange(dt, "BankAccountNo", "Payee Bank Acc No");
                    dt = Ordinary.DtColumnNameChange(dt, "Routing_No", "PayeeBankRouting");
                    dt = Ordinary.DtColumnNameChange(dt, "ELAmount", "Amount");
                    dataView = new DataView(dt);
                    dt = dataView.ToTable(true, "Customer Reference (16)", "Payee Name", "Payee Bank Acc No",
                        "PayeeAccType (Add1)", "PayeeBankRouting"
                        , "Reason", "Amount", "Payment Date", "Debit A/C No.", "Email");

                    
                    Line3 = "BFTN: SB and Others -" + fyParam + " To " + fyToParam;
                    filename = "Earned Leave BFTN: SB and Others " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmm");
                }

                if (SheetName == "EarnLeaveSheet4")
                {
                    dt.Columns.Remove("ELAmount");
                    dt = Ordinary.DtColumnNameChange(dt, "NetDeductionAmount", "ELAmount");

                    string DebitAccountNo = new SettingRepo().settingValue("Salary", "DebitA/CNo");


                    string[] DecimalColumnNames = { "ELAmount" };
                    dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);

                    dt.Columns.Add(new DataColumn("Reason") { DefaultValue = "EL for " +fyParam + " To " + fyToParam });
                    dt.Columns.Add(new DataColumn("PayeeAccType (Add1)") { DefaultValue = "Savings" });
                    dt.Columns.Add(new DataColumn("Debit A/C No.") { DefaultValue = DebitAccountNo });
                    dt.Columns.Add(new DataColumn("Payment Date") { DefaultValue = paymentDay });
                    dt = Ordinary.DtColumnNameChange(dt, "ELAmount", "Amount");

                    dt = Ordinary.DtColumnNameChange(dt, "Code", "Customer Reference (16)");
                    dt = Ordinary.DtColumnNameChange(dt, "EmpName", "Payee Name");
                    dt = Ordinary.DtColumnNameChange(dt, "BankAccountNo", "Payee Bank Acc No");
                     dataView = new DataView(dt);


                    dt = dataView.ToTable(true, "Customer Reference (16)", "Payee Name", "Payee Bank Acc No"
                        , "Reason", "Amount", "Payment Date", "Debit A/C No.", "Email");

                    Line3 = "BFTN-SCB- -" + fyParam + " To " + fyToParam;
                    filename = "Earned Leave BFTN-SCB " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmm");
                }


                int LeftColumn = 5;
                int CenterColumn = 5;

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

                result[0] = "Successfull";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("EarningLeaveProcess");
            }
            catch (Exception e)
            {
                return RedirectToAction("EarningLeaveProcess");
            }
        }


        public ActionResult EarningLeaveSummaryExcel(string ProjectId, string DepartmentId, string SectionId,
            string DesignationId, string CodeF, string CodeT
            , string Orderby, string view, int fid = 0, int fidTo = 0)
        {
            try
            {
                #region Variables

                string[] result = new string[6];
                bool IsMultiple = false;
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

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

                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }

                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }

                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }

                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
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

                #endregion Variables

                if (fid != fidTo)
                {
                    IsMultiple = true;
                }

                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();
                string filename = "";

                SalaryEarningLeaveRepo _repo = new SalaryEarningLeaveRepo();

                List<EmployeeEarningLeaveVM> getAllData = new List<EmployeeEarningLeaveVM>();
                getAllData = _repo.SelectAllForReportSummary(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                    vDesignationId, vCodeF, vCodeT, 0, Orderby);

                DataTable table = new DataTable();
                DataTable dt = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                table = Ordinary.DtColumnNameChange(table, "DeductionAmount", "ELAmount");
                var dataView = new DataView(table);

                string strSort = "SectionOrder";
                dataView.Sort = strSort;
                dt = dataView.ToTable(true, "Designation", "Section", "Project", "BasicSalary", "GrossSalary",
                    "HouseRent", "Medical"
                    , "TransportAllowance", "Stamp", "Days", "ELAmount");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name; // "BRAC EPL STOCK BROKERAGE LIMITED";
                string
                    Line2 = comInfo
                        .Address; // "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "Statement of Staff Earning Leave Summary";
                if (IsMultiple)
                {
                    Line3 = "Statement of Staff Earning Leave Summary -" + fyParam + " To " + fyToParam;
                }

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                filename = "EmployeeEarningLeave Summary" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmm");

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
                return RedirectToAction("EarningLeaveProcess");
            }
            catch (Exception e)
            {
                return RedirectToAction("EarningLeaveProcess");
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

        //#endregion Action Methods


        //public JsonResult PunishmentAmount(string fydId)
        //{
        //    #region Get Setting Value
        //    string PunishmentFromBasic = new SettingRepo().settingValue("Deduction", "PunishmentFromBasic");
        //    int DaysOfMonth = new FiscalYearRepo().SelectDaysOfMonth(Convert.ToInt32(fydId));
        //    #endregion
        //    string result = PunishmentFromBasic + "~" + DaysOfMonth;
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
    }
}