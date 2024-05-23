using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using OfficeOpenXml;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymRepository.Common;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Common;
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
using SymRepository.Leave;
namespace SymWebUI.Areas.Common.Controllers
{
    public class EmployeeEarnLeaveEncashmentController : Controller
    {
        EmployeeEarnLeaveEncashmentRepo _eaRepo = new EmployeeEarnLeaveEncashmentRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //#region Action Methods

        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            return View();
        }

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            _eaRepo = new EmployeeEarnLeaveEncashmentRepo();
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var PeriodNameFilter = Convert.ToString(Request["sSearch_1"]);
            var RemarksFilter = Convert.ToString(Request["sSearch_2"]);
            #endregion Column Search
            #region Search and Filter Data
            var getAllData = _eaRepo.GetYear();
            IEnumerable<EmployeeLeaveEncashmentVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Year.ToLower().Contains(param.sSearch.ToLower())
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
                                .Where(c => (PeriodNameFilter == "" || c.Year.ToLower().Contains(PeriodNameFilter.ToLower()))
                                    &&
                                    (RemarksFilter == "" || c.Remarks.ToLower().Contains(RemarksFilter.ToLower()))
                                );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeLeaveEncashmentVM, string> orderingFunction = (
                c => sortColumnIndex == 1 && isSortable_1 ? c.Year :
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
                             Convert.ToString(c.FiscalYearDetailId)
                             , c.Year
                             , c.Remarks 
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
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeLeaveEncashmentVM EarningVM = new EmployeeLeaveEncashmentVM();
            EmployeeEarnLeaveEncashmentRepo arerepo = new EmployeeEarnLeaveEncashmentRepo();
            if (id != "0")
            {
                EarningVM = arerepo.SelectById(id);//find emp code
                vm = repo.SelectById(EarningVM.EmployeeId);
                vm.EmployeeLeaveEncashmentVM = EarningVM;
                vm.FiscalYearDetailId = EarningVM.FiscalYearDetailId;
            }
            Session["empid"] = id;


            return View(vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(string btn, EmployeeInfoVM empVM)
        {
            EmployeeLeaveEncashmentVM vm = new EmployeeLeaveEncashmentVM();
            string[] result = new string[6];
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm = empVM.EmployeeLeaveEncashmentVM;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
              
                if (btn.ToLower() != "save")
                {
                    vm.EarningAmount = 0;
                }
                result = new EmployeeEarnLeaveEncashmentRepo().Insert(vm);
                if (result[0].ToLower() == "success" && btn.ToLower() != "save")
                {
                    result[1] = "Data Deleted Successfully";
                }
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Post(EmployeeInfoVM empVM)
        {
            EmployeeLeaveEncashmentVM vm = new EmployeeLeaveEncashmentVM();
            string[] result = new string[6];
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm = empVM.EmployeeLeaveEncashmentVM;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                result = new EmployeeEarnLeaveEncashmentRepo().Post(vm);
                if (result[0].ToLower() == "success" )
                {
                    result[1] = "Data Approve Successfully";
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
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            EmployeeEarnLeaveEncashmentRepo arerepo = new EmployeeEarnLeaveEncashmentRepo();
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var tt = arerepo.SelectAll(null, FID).FirstOrDefault();
            if (tt != null)
            {
                ViewBag.periodName = tt.Year;
            }
            ViewBag.Id = FID;
            return View();
        }

        public ActionResult _EmployeeOtherEarningDetails(JQueryDataTableParamVM param, int FID)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var EmployeeNameFilter = Convert.ToString(Request["sSearch_2"]);
         
            var BasicSalaryFilter = Convert.ToString(Request["sSearch_3"]);
        
            var BasicamountFrom = 0;
            var BasicamountTo = 0;
            if (BasicSalaryFilter.Contains('~'))
            {
                BasicamountFrom = BasicSalaryFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(BasicSalaryFilter.Split('~')[0]) == true ? Convert.ToInt32(BasicSalaryFilter.Split('~')[0]) : 0;
                BasicamountTo = BasicSalaryFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(BasicSalaryFilter.Split('~')[1]) == true ? Convert.ToInt32(BasicSalaryFilter.Split('~')[1]) : 0;
            }
            
            #endregion
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeEarnLeaveEncashmentRepo arerepo = new EmployeeEarnLeaveEncashmentRepo();
            var getAllData = arerepo.SelectAll(null, FID);
            IEnumerable<EmployeeLeaveEncashmentVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
               
                filteredData = getAllData.Where(c =>
                    isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.EncashmentBalance.ToString().ToLower().Contains(param.sSearch.ToLower())
                    
                );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (codeFilter != "" || EmployeeNameFilter != "" || (BasicSalaryFilter != "" && BasicSalaryFilter != "~") )
            {
                filteredData = filteredData.Where(c =>
                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                    && (EmployeeNameFilter == "" || c.EmpName.ToLower().Contains(EmployeeNameFilter.ToLower()))
                    && (BasicamountFrom == 0 || BasicamountFrom <= Convert.ToInt32(c.EncashmentBalance))
                    && (BasicamountTo == 0 || BasicamountTo >= Convert.ToInt32(c.EncashmentBalance))
                    );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeLeaveEncashmentVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.EncashmentBalance.ToString() :
        
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
                             ,c.Code
                             , c.EmpName
                             , c.EncashmentBalance.ToString() 
                        
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
        public ActionResult SingleOtherEarningEdit(string OtherearngingId)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            EmployeeLeaveEncashmentVM empotherearningvm = new EmployeeLeaveEncashmentVM();
            EmployeeEarnLeaveEncashmentRepo arerepo = new EmployeeEarnLeaveEncashmentRepo();
            EmployeeLeaveRepo _levRepo = new EmployeeLeaveRepo();

            if (!string.IsNullOrWhiteSpace(OtherearngingId))
                empotherearningvm = arerepo.SelectById(OtherearngingId);
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            if (!string.IsNullOrWhiteSpace(OtherearngingId) && !string.IsNullOrWhiteSpace(empotherearningvm.Id))
            {
                vm = repo.SelectById(empotherearningvm.EmployeeId);
            }
            vm.EmployeeLeaveEncashmentVM = empotherearningvm;
            Session["empid"] = empotherearningvm.Id;
            vm.FiscalYearDetailId = Convert.ToInt32(empotherearningvm.FiscalYearDetailId);
            #region Balance Check
            if (!string.IsNullOrWhiteSpace(OtherearngingId) && !string.IsNullOrWhiteSpace(empotherearningvm.Id))
            {
                vm.employeeLeaveBalanceVMs = arerepo.EmployeeLeaveEncashmentBalance(empotherearningvm.EmployeeId.ToString(), vm.EmployeeLeaveEncashmentVM.Year.ToString());

                var model = vm.employeeLeaveBalanceVMs.FirstOrDefault();
                vm.EmployeeLeaveEncashmentVM.AnualBalance = model != null ? Convert.ToDecimal(model.Have) : 0;
            }
            #endregion

            return View(vm);
        }

        public ActionResult ImportOtherEarning()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            return View();
        }

        public ActionResult ImportOtherEarningExcel(HttpPostedFileBase file, int FYDId = 0)
        {
            string[] result = new string[6];
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Common/Home");
                }
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + file.FileName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(fullPath);
                }
                ShampanIdentityVM vm = new ShampanIdentityVM();
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                result = _eaRepo.ImportExcelFile(fullPath, file.FileName, vm, FYDId);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportOtherEarning");
                //return RedirectToAction("OpeningBalance");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportOtherEarning");
            }
        }

        public ActionResult DownloadOtherEarningExcel(string ProjectId, string DepartmentId, string SectionId
            , string DesignationId, string CodeF, string CodeT, int fid = 0, int ETId = 0, string Orderby = null)
        {
            DataTable dt = new DataTable();
            string[] result = new string[6];
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Common/Home");
                }
                string FileName = "Download.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                string contentType = MimeMapping.GetMimeMapping(fullPath);
                //string fullPath = @"C:\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }
                dt = new EmployeeEarnLeaveEncashmentRepo().ExportExcelFile(fullPath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, ETId, Orderby);
                //exp(dt);
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

                string filename = "EmpLeave Encashment" + fid;
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

        public ActionResult DetailCreate(string empcode = "", string btn = "current", string FiscalYearDetailId = "0", string edType = "0", string id = "0", string salaryMonth = "")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            string EmployeeId = "";
            decimal EncashmentRatio = 0;
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeEarnLeaveEncashmentRepo arerepo = new EmployeeEarnLeaveEncashmentRepo();
            EmployeeLeaveEncashmentVM EarningVM = new EmployeeLeaveEncashmentVM();
            EmployeeLeaveRepo _levRepo = new EmployeeLeaveRepo();
            SettingRepo _setDAL = new SettingRepo();

            EmployeeInfoVM vm = new EmployeeInfoVM();

            EncashmentRatio = Convert.ToDecimal(_setDAL.settingValue("Encashment", "EncashmentRatio"));

            if (!string.IsNullOrEmpty(Session["empid"] as string) && Session["empid"] as string != "0")
            {
                string empid = Session["empid"] as string;
                EarningVM = arerepo.SelectById(empid);//find emp code
                vm = repo.SelectById(EarningVM.EmployeeId);
                vm.EmployeeLeaveEncashmentVM = EarningVM;
                #region Balance Check
                vm.employeeLeaveBalanceVMs = arerepo.EmployeeLeaveEncashmentBalance(EarningVM.EmployeeId.ToString(), vm.EmployeeLeaveEncashmentVM.Year.ToString());

                var model = vm.employeeLeaveBalanceVMs.FirstOrDefault();
                vm.EmployeeLeaveEncashmentVM.AnualBalance = model != null ? Convert.ToDecimal(model.Have) : 0;
                vm.EmployeeLeaveEncashmentVM.EncashmentRatio = EncashmentRatio;
                #endregion

                Session["empid"] = "";
                // find exist earning date
            }
            else if (id != "0")
            {
                EarningVM = arerepo.SelectById(id);//find emp code
                vm = repo.SelectById(EarningVM.EmployeeId);
                vm.EmployeeLeaveEncashmentVM = EarningVM;

                #region Balance Check
                vm.employeeLeaveBalanceVMs = arerepo.EmployeeLeaveEncashmentBalance(EarningVM.EmployeeId.ToString(), vm.EmployeeLeaveEncashmentVM.Year.ToString());

                var model = vm.employeeLeaveBalanceVMs.FirstOrDefault();
                vm.EmployeeLeaveEncashmentVM.AnualBalance = model != null ? Convert.ToDecimal(model.Have) : 0;
                vm.EmployeeLeaveEncashmentVM.EncashmentRatio = EncashmentRatio;

                #endregion

                // find exist earning date
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
                    EarningVM = arerepo.SelectByIdandFiscalyearDetail(vm.Id, FiscalYearDetailId, edType, salaryMonth);
                    EarningVM.FiscalYearDetailId = Convert.ToInt32(FiscalYearDetailId);
                    EarningVM.EarningTypeId = Convert.ToInt32(edType);
                    #region Balance Check
                    vm.employeeLeaveBalanceVMs = arerepo.EmployeeLeaveEncashmentBalance(vm.Id.ToString(), FiscalYearDetailId.ToString());

                    var model = vm.employeeLeaveBalanceVMs.FirstOrDefault();
                    EarningVM.AnualBalance = model != null ? Convert.ToDecimal(model.Have) : 0;
                    EarningVM.EncashmentRatio = EncashmentRatio;

                    #endregion


                }
                if (string.IsNullOrWhiteSpace(FiscalYearDetailId))
                {
                    FiscalYearDetailId = "0";
                }
                //svms = arerepo.SingleEmployeeEntry(EmployeeId, FiscalYearDetailId);
                vm.EmployeeLeaveEncashmentVM = EarningVM;
                vm.EmployeeLeaveEncashmentVM.EmployeeId = EmployeeId;
                vm.EmployeeLeaveEncashmentVM.FiscalYearDetailId = Convert.ToInt32(FiscalYearDetailId);

            }
            return PartialView("_detailCreate", vm);
        }

        public ActionResult Delete(string ids)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            EmployeeLeaveEncashmentVM EarningVM = new EmployeeLeaveEncashmentVM();
            EmployeeEarnLeaveEncashmentRepo arerepo = new EmployeeEarnLeaveEncashmentRepo();
            EarningVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            EarningVM.LastUpdateBy = identity.Name;
            EarningVM.LastUpdateFrom = identity.WorkStationIP;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = arerepo.Delete(EarningVM, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public JsonResult OvertimeAmount(string OTHrs, string OTHrsSpecial, string Code, string CommonPeriodId, string EmployeeId)
        {

            string otAmount = "0";
            if (!string.IsNullOrWhiteSpace(OTHrs) && !string.IsNullOrWhiteSpace(OTHrsSpecial))
            {
                otAmount = _eaRepo.OvertimeAmount(OTHrs, OTHrsSpecial, Code, CommonPeriodId, EmployeeId);
            }
            return Json(otAmount, JsonRequestBehavior.AllowGet);
        }


        public ActionResult _rptIndexPartial(string ProjectId, string DepartmentId, string SectionId, string DesignationId
                    , string CodeF, string CodeT, int fid = 0, int fidTo = 0, int ETId = 0, string Orderby = null)
        {
            EmployeeLeaveEncashmentVM vm = new EmployeeLeaveEncashmentVM();
            vm.ProjectId = ProjectId;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.DesignationId = DesignationId;
            vm.CodeF = CodeF;
            vm.CodeT = CodeT;
            vm.FiscalYearDetailId = fid;
            vm.fidTo = fidTo;
            vm.EarningTypeId = ETId;
            vm.Orderby = Orderby;
            return PartialView("_rptIndex", vm);
        }

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }
        //#endregion Action Methods
    }
}
