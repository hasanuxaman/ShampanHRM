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
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class EmployeeOtherDeductionController : Controller
    {
        //
        // GET: /Payroll/EmployeeOtherDeduction/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        EmployeeOtherDeductionRepo _eaRepo;
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
            EmployeeOtherDeductionRepo _eaRepo = new EmployeeOtherDeductionRepo();
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var PeriodNameFilter = Convert.ToString(Request["sSearch_1"]);
            var RemarksFilter = Convert.ToString(Request["sSearch_2"]);
            #endregion Column Search
            #region Search and Filter Data
            var getAllData = _eaRepo.GetPeriodname();
            IEnumerable<EmployeeOtherDeductionVM> filteredData;
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
            Func<EmployeeOtherDeductionVM, string> orderingFunction = (
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
                         select new[] { 
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
            EmployeeOtherDeductionRepo arerepo = new EmployeeOtherDeductionRepo();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeOtherDeductionVM DeductionVM = new EmployeeOtherDeductionVM();
            if (id != "0")
            {
                DeductionVM = arerepo.SelectById(id);//find emp code
                vm = repo.SelectById(DeductionVM.EmployeeId);
                vm.FiscalYearDetailId = DeductionVM.FiscalYearDetailId;
                vm.EmployeeOtherDeductionVM = DeductionVM;
            }
            Session["empid"] = id;

            return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(string btn, EmployeeInfoVM empVM)
        {
            EmployeeOtherDeductionVM vm = new EmployeeOtherDeductionVM();
            string[] result = new string[6];
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm = empVM.EmployeeOtherDeductionVM;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.DeductionDate = new FiscalYearRepo().FiscalPeriodStartDate(vm.FiscalYearDetailId);
                if (vm.FiscalYearDetailId == 0)
                {
                    Session["result"] = "Fail~Fiscal Year Not Exist on this Period";
                    FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                    return RedirectToAction("Index");
                }
                if (btn.ToLower() != "save")
                {
                    vm.DeductionAmount = 0;
                }
                result = new EmployeeOtherDeductionRepo().Insert(vm);
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
            EmployeeOtherDeductionRepo arerepo = new EmployeeOtherDeductionRepo();
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
            var DeductionTypeFilter = Convert.ToString(Request["sSearch_3"]);
            var DeductionAmountFilter = Convert.ToString(Request["sSearch_4"]);
            var BasicSalaryFilter = Convert.ToString(Request["sSearch_5"]);
            var GrossSalaryFilter = Convert.ToString(Request["sSearch_6"]);
            var BasicamountFrom = 0;
            var BasicamountTo = 0;
            if (BasicSalaryFilter.Contains('~'))
            {
                BasicamountFrom = BasicSalaryFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(BasicSalaryFilter.Split('~')[0]) == true ? Convert.ToInt32(BasicSalaryFilter.Split('~')[0]) : 0;
                BasicamountTo = BasicSalaryFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(BasicSalaryFilter.Split('~')[1]) == true ? Convert.ToInt32(BasicSalaryFilter.Split('~')[1]) : 0;
            }
            var GrossamountFrom = 0;
            var GrossamountTo = 0;
            if (GrossSalaryFilter.Contains('~'))
            {
                GrossamountFrom = GrossSalaryFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(GrossSalaryFilter.Split('~')[0]) == true ? Convert.ToInt32(GrossSalaryFilter.Split('~')[0]) : 0;
                GrossamountTo = GrossSalaryFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(GrossSalaryFilter.Split('~')[1]) == true ? Convert.ToInt32(GrossSalaryFilter.Split('~')[1]) : 0;
            }
            var amountFrom = 0;
            var amountTo = 0;
            if (DeductionAmountFilter.Contains('~'))
            {
                amountFrom = DeductionAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(DeductionAmountFilter.Split('~')[0]) == true ? Convert.ToInt32(DeductionAmountFilter.Split('~')[0]) : 0;
                amountTo = DeductionAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(DeductionAmountFilter.Split('~')[0]) == true ? Convert.ToInt32(DeductionAmountFilter.Split('~')[0]) : 0;
            }
            #endregion
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeOtherDeductionRepo arerepo = new EmployeeOtherDeductionRepo();
            var getAllData = arerepo.SelectAll(null, FID);
            IEnumerable<EmployeeOtherDeductionVM> filteredData;
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
                               || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.DeductionType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.DeductionAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (codeFilter != "" || EmployeeNameFilter != "" || (BasicSalaryFilter != "" && BasicSalaryFilter != "~") || (GrossSalaryFilter != "" && GrossSalaryFilter != "~") || (DeductionAmountFilter != "" && DeductionAmountFilter != "~") || DeductionTypeFilter != "")
            {
                filteredData = filteredData.Where(c =>
                    (codeFilter == "" || c.EmpName.ToLower().Contains(codeFilter.ToLower()))
                    && (EmployeeNameFilter == "" || c.EmpName.ToLower().Contains(EmployeeNameFilter.ToLower()))
                    && (DeductionTypeFilter == "" || c.DeductionType.ToLower().Contains(DeductionTypeFilter.ToLower()))
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
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeOtherDeductionVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.DeductionType :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.DeductionAmount.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.BasicSalary.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.GrossSalary.ToString() :
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
                             Convert.ToString(c.Id)
                             , c.Code
                             , c.EmpName
                             , c.DeductionType 
                             , c.DeductionAmount.ToString() 
                             , c.BasicSalary.ToString()
                             , c.GrossSalary.ToString()
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
        public ActionResult SingleOtherDeductionEdit(string OtherDeductionId)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_39", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeOtherDeductionVM empotherDeductionvm = new EmployeeOtherDeductionVM();
            EmployeeOtherDeductionRepo arerepo = new EmployeeOtherDeductionRepo();
            if (!string.IsNullOrWhiteSpace(OtherDeductionId))
                empotherDeductionvm = arerepo.SelectById(OtherDeductionId);
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            if (!string.IsNullOrWhiteSpace(OtherDeductionId) && !string.IsNullOrWhiteSpace(empotherDeductionvm.Id))
            {
                vm = repo.SelectById(empotherDeductionvm.EmployeeId);
            }
            vm.EmployeeOtherDeductionVM = empotherDeductionvm;
            Session["empid"] = empotherDeductionvm.Id;
            vm.FiscalYearDetailId = Convert.ToInt32(empotherDeductionvm.FiscalYearDetailId);
            return View(vm);
        }
        [HttpGet]
        public ActionResult ImportOtherDeduction()
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
        public ActionResult ImportOtherDeductionExcel(HttpPostedFileBase file, int FYDId = 0)
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
                result = new EmployeeOtherDeductionRepo().ImportExcelFile(fullPath, file.FileName, vm, null, null, FYDId);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportOtherDeduction");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportOtherDeduction");
            }
        }
        public ActionResult DownloadOtherDeductionExcel(HttpPostedFileBase file, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, int DTId = 0, string Orderby = null)
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
                EarningDeductionTypeRepo edtRepo = new EarningDeductionTypeRepo();
                string edtvm = edtRepo.SelectById(DTId).Name;

                string FileName = "OtherDeduction_" + edtvm.Trim() + ".xlsx";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }
                //ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Tinned Goods");
                dt = new EmployeeOtherDeductionRepo().ExportExcelFile(fullPath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, DTId, Orderby);
                //exp(dt);

                string filename = dt.Rows[0]["Type"].ToString() + "-" + dt.Rows[0]["Fiscal Period"].ToString();
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
                return RedirectToAction("ImportOtherDeduction");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportOtherDeduction");
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
            xlWorkSheet.get_Range("A7", "S7").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.AliceBlue);
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
        public ActionResult DetailCreate(string empcode = "", string btn = "current", int FiscalYearDetailId = 0, string edType = "0", string id = "0")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_39", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            string EmployeeId = "";
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeOtherDeductionRepo arerepo = new EmployeeOtherDeductionRepo();
            EmployeeOtherDeductionVM DeductionVM = new EmployeeOtherDeductionVM();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrWhiteSpace(Session["empid"] as string) && Session["empid"] as string != "0")
            {
                string empid = Session["empid"] as string;
                DeductionVM = arerepo.SelectById(empid);//find emp code
                vm = repo.SelectById(DeductionVM.EmployeeId);
                vm.EmployeeOtherDeductionVM = DeductionVM;
                Session["empid"] = "";
                // find exist Deduction date
            }
            else if (id != "0")
            {
                DeductionVM = arerepo.SelectById(id);//find emp code
                vm = repo.SelectById(DeductionVM.EmployeeId);
                vm.EmployeeOtherDeductionVM = DeductionVM;
                // find exist Deduction date
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
                    DeductionVM = arerepo.SelectByIdandFiscalyearDetail(vm.Id, FiscalYearDetailId, edType);
                    //DeductionVM = arerepo.SelectAll(vm.Id, Convert.ToInt32(FiscalYearDetailId), Convert.ToInt32(edType)).FirstOrDefault();
                    DeductionVM.FiscalYearDetailId = Convert.ToInt32(FiscalYearDetailId);
                    DeductionVM.DeductionTypeId = Convert.ToInt32(edType);
                }
                if (FiscalYearDetailId == 0)
                {
                    FiscalYearDetailId = 0;
                }
                //svms = arerepo.SingleEmployeeEntry(EmployeeId, FiscalYearDetailId);
                vm.EmployeeOtherDeductionVM = DeductionVM;
                vm.EmployeeOtherDeductionVM.EmployeeId = EmployeeId;
                vm.EmployeeOtherDeductionVM.FiscalYearDetailId = FiscalYearDetailId;

                #region Get Setting Value
                if (edType == "4")
                {
                    vm.EmployeeOtherDeductionVM.PunishmentFromBasic = new SettingRepo().settingValue("Deduction", "PunishmentFromBasic");
                    if (FiscalYearDetailId > 0)
                    {
                        vm.EmployeeOtherDeductionVM.DaysOfMonth = new FiscalYearRepo().SelectDaysOfMonth(Convert.ToInt32(FiscalYearDetailId));
                    }
                }

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
            EmployeeOtherDeductionVM DeductionVM = new EmployeeOtherDeductionVM();
            EmployeeOtherDeductionRepo odRepo = new EmployeeOtherDeductionRepo();
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            DeductionVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            DeductionVM.LastUpdateBy = identity.Name;
            DeductionVM.LastUpdateFrom = identity.WorkStationIP;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = odRepo.Delete(DeductionVM, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        public ActionResult EmployeeOtherDeductionReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF
            , string Orderby, string CodeT, string view, string rptPG1, string rptPG2, int fid = 0, int fidTo = 0, int DTId = 0)
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
                List<EmployeeOtherDeductionVM> getAllData = new List<EmployeeOtherDeductionVM>();
                EmployeeOtherDeductionRepo repo = new EmployeeOtherDeductionRepo();
                getAllData = repo.SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, DTId, Orderby);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Other Deduction";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Other Deduction List";
                }

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                DataSet ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtEmployeeOtherEarningDeduction";
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollEntry\rptEmployeeOtherDeduction.rpt";
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
        public FileStreamResult Export()
        {
            var searchResults = new EmployeeOtherDeductionRepo().GetPeriodname();
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
        public ActionResult _rptIndexPartial(string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, int fid = 0, int fidTo = 0, int DTId = 0, string Orderby = null)
        {
            EmployeeOtherDeductionVM vm = new EmployeeOtherDeductionVM();
            vm.ProjectId = ProjectId;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.DesignationId = DesignationId;
            vm.CodeF = CodeF;
            vm.CodeT = CodeT;
            vm.FiscalYearDetailId = fid;
            vm.fidTo = fidTo;
            vm.DeductionTypeId = DTId;
            vm.Orderby = Orderby;
            return PartialView("_rptIndex", vm);
        }
        public ActionResult _rptIndex(JQueryDataTableParamVM param, string ProjectId, string DepartmentId, string SectionId, string DesignationId
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
            EmployeeOtherDeductionRepo _repo = new EmployeeOtherDeductionRepo();
            var getAllData = _repo.SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                vDesignationId, vCodeF, vCodeT, DTId, Orderby);
            IEnumerable<EmployeeOtherDeductionVM> filteredData;
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
                var isSearchable10 = Convert.ToBoolean(Request["bSearchable_10"]);
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
                               || isSearchable9 && c.DeductionType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable10 && c.DeductionAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<EmployeeOtherDeductionVM, string> orderingFunction = (c => sortColumnIndex == 0 && isSortable_0 ? c.PeriodStart :
                                                           sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Department :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Section :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Project :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.BasicSalary.ToString() :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.GrossSalary.ToString() :
                                                           sortColumnIndex == 9 && isSortable_9 ? c.DeductionType :
                                                           sortColumnIndex == 10 && isSortable_10 ? c.DeductionAmount.ToString() :
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
                             , c.DeductionType
                             , c.DeductionAmount.ToString()
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
        public ActionResult DownloadOtherDeductionExcel1(HttpPostedFileBase file, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, int DTId = 0, string Orderby = null)
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
                dt = new EmployeeOtherDeductionRepo().ExportExcelFile(fullPath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, DTId, Orderby);
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
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportOtherDeduction");
            }
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
