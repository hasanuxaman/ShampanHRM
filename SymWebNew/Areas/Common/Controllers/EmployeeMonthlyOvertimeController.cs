using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SymViewModel.Attendance;
using SymOrdinary;
using System.Threading;
using SymRepository.Attendance;
using SymRepository.Common;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Data;
using OfficeOpenXml;

namespace SymWebUI.Areas.Common.Controllers
{
    public class EmployeeMonthlyOvertimeController : Controller
    {
        //
        // GET: /Common/EmployeeMonthlyOvertime/

        EmployeeMonthlyOvertimeRepo _repo = new EmployeeMonthlyOvertimeRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _indexPartial(string ProjectId, string DepartmentId, string SectionId, string DesignationId, int fid = 0)
        {
            #region Declare Variable
            string vProjectId = "0_0";
            string vDepartmentId = "0_0";
            string vSectionId = "0_0";

            if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null && ProjectId != "undefined" )
            {
                vProjectId = ProjectId;
            }
            if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null && DepartmentId != "undefined" )
            {
                vDepartmentId = DepartmentId;
            }
            if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null && SectionId != "undefined" )
            {
                vSectionId = SectionId;
            }
            #endregion Declare Variable
            List<EmployeeMonthlyOvertimeVM> VMs = new List<EmployeeMonthlyOvertimeVM>();
            VMs = _repo.SelectAll(vProjectId, vDepartmentId, vSectionId, fid);
            return PartialView("_index", VMs);
        }

        [HttpPost]
        public ActionResult Create(List<EmployeeMonthlyOvertimeVM> VMs, string fid)
        {
            string[] result = new string[6];
            string mgs = "";
            ShampanIdentityVM vm = new ShampanIdentityVM();
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            result = _repo.Insert(VMs, fid, vm);
            mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateFromDailyOvertime(string fid)
        {
            string[] result = new string[6];
            string mgs = "";
            ShampanIdentityVM vm = new ShampanIdentityVM();
            //vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //vm.LastUpdateBy = identity.Name;
            //vm.LastUpdateFrom = identity.WorkStationIP;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            result = _repo.InsertFromDailyOvertime(fid, vm);
            mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "70002", "report").ToString();
            return View("Report");
        }
        [HttpGet]
        public ActionResult ReportView(string departmentId, string projectId, string sectionId, string fid, string reportType)
        {
            try
            {
                if (projectId == "0_0" || projectId == "0" || projectId == "" || projectId == "null" || projectId == null)
                {
                    projectId = "";
                }
                if (departmentId == "0_0" || departmentId == "0" || departmentId == "" || departmentId == "null" || departmentId == null)
                {
                    departmentId = "";
                }
                if (sectionId == "0_0" || sectionId == "0" || sectionId == "" || sectionId == "null" || sectionId == null)
                {
                    sectionId = "";
                }

                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                EmployeeMonthlyOvertimeVM vm = new EmployeeMonthlyOvertimeVM();
                EmployeeMonthlyOvertimeRepo _repo = new EmployeeMonthlyOvertimeRepo();
                vm.FiscalYearDetailId = Convert.ToInt32(fid);
                string[] conditionFields = { "ve.DepartmentId", "ve.ProjectId", "ve.SectionId" };
                string[] conditionValues = { departmentId, projectId, sectionId };
                table = _repo.Report(vm, conditionFields, conditionValues);
                if (table.Rows.Count == 0)
                {
                    Session["result"] = "Fail" + "~" + "No Data Found";
                    return View("Report");
                }


                ReportHead = "There are no data to Preview for Employee Monthly Overtime";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "Employee Monthly Overtime List";
                }
                if (reportType == "D")
                {
                    ReportHead += " (Detail)";
                }
                else if (reportType == "S")
                {
                    ReportHead += " (Summery)";
                }

                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtEmployeeMonthlyOvertime";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptEmployeeMonthlyOvertime.rpt";
                //if (System.IO.File.Exists(rptLocation))
                //{
                //    var tt = "";
                //    //System.IO.File.Delete(fullPath + FileName);
                //}
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["paramReportType"].Text = "'" + reportType + "'";
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



        public ActionResult ImportMonthlyOvertime()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            return View();
        }
        public ActionResult ImportMonthlyOvertimeExcel(HttpPostedFileBase file)
        {
            string[] result = new string[6];
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
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
                //vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                //vm.LastUpdateBy = identity.Name;
                //vm.LastUpdateFrom = identity.WorkStationIP;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                result = _repo.ImportExcelFile(fullPath, file.FileName, vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportMonthlyOvertime");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportMonthlyOvertime");
            }
        }
        public ActionResult DownloadExcel(HttpPostedFileBase file, string projectId, string departmentId, string sectionId, string fid)
        {
            if (projectId == "0_0" || projectId == "0" || projectId == "" || projectId == "null" || projectId == null)
            {
                projectId = "";
            }
            if (departmentId == "0_0" || departmentId == "0" || departmentId == "" || departmentId == "null" || departmentId == null)
            {
                departmentId = "";
            }
            if (sectionId == "0_0" || sectionId == "0" || sectionId == "" || sectionId == "null" || sectionId == null)
            {
                sectionId = "";
            }
            string[] result = new string[6];
            DataTable dt = new DataTable();
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Common/Home");
                }
                ExcelPackage xlPackage = new ExcelPackage();
                string FileName = "Download.xlsx";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }
                EmployeeMonthlyOvertimeVM vm = new EmployeeMonthlyOvertimeVM();
                EmployeeMonthlyOvertimeRepo _repo = new EmployeeMonthlyOvertimeRepo();
                vm.FiscalYearDetailId = Convert.ToInt32(fid);
                string[] conditionFields = { "a.DepartmentId", "a.ProjectId", "a.SectionId" };
                string[] conditionValues = { departmentId, projectId, sectionId };
                dt = _repo.ExportExcelFile(vm, conditionFields, conditionValues);
                //dt = _repo.ExportExcelFile(fullPath, FileName, ProjectId, DepartmentId, fid);
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=Download.xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                result[0] = "Success";
                result[1] = "Success~Data Save ";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportMonthlyOvertime");
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;
                Session["result"] = result[0] + "~" + result[1];
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportMonthlyOvertime");
            }
        }

    }
}
