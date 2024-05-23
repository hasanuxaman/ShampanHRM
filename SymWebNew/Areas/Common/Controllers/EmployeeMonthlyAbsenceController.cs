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
using System.Data;
using OfficeOpenXml;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;

namespace SymWebUI.Areas.Common.Controllers
{
    public class EmployeeMonthlyAbsenceController : Controller
    {
        //
        // GET: /Common/EmployeeMonthlyAbsence/

        EmployeeMonthlyAbsenceRepo _repo = new EmployeeMonthlyAbsenceRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _IndexPartial(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string fid)
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
            List<EmployeeMonthlyAbsenceVM> VMs = new List<EmployeeMonthlyAbsenceVM>();
            VMs = _repo.SelectAll(vProjectId, vDepartmentId, vSectionId, Convert.ToInt32(fid));
            FiscalYearRepo frepo = new FiscalYearRepo();
            var tt = frepo.FYPeriodDetail(Convert.ToInt32(fid)).FirstOrDefault().PeriodName;
            ViewBag.PeriodName = tt;
            return PartialView("_Index", VMs);
        }

        [HttpPost]
        public ActionResult Create(List<EmployeeMonthlyAbsenceVM> VMs, string fid)
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
        public ActionResult CreateFromDailyAbsence(string fid)
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
            result = _repo.InsertFromDailyAbsence(fid, vm);


            mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "70002", "report").ToString();
            EmployeeMonthlyAbsenceVM vm = new EmployeeMonthlyAbsenceVM();
            return View("Report", vm);
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
                EmployeeMonthlyAbsenceVM vm = new EmployeeMonthlyAbsenceVM();
                EmployeeMonthlyAbsenceRepo _repo = new EmployeeMonthlyAbsenceRepo();
                vm.FiscalYearDetailId = Convert.ToInt32(fid);
                string[] conditionFields = { "ve.DepartmentId", "ve.ProjectId", "ve.SectionId" };
                string[] conditionValues = { departmentId, projectId, sectionId };
                table = _repo.Report(vm, conditionFields, conditionValues);
                ReportHead = "There are no data to Preview for Employee Monthly Absence";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "Employee Monthly Absence List";
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
                ds.Tables[0].TableName = "dtEmployeeMonthlyAbsence";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptEmployeeMonthlyAbsence.rpt";
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


        public ActionResult ImportMonthlyAbsence()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            return View();
        }
        public ActionResult ImportMonthlyAbsenceExcel(HttpPostedFileBase file)
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
                return RedirectToAction("ImportMonthlyAbsence");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportMonthlyAbsence");
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
                EmployeeMonthlyAbsenceVM vm = new EmployeeMonthlyAbsenceVM();
                EmployeeMonthlyAbsenceRepo _repo = new EmployeeMonthlyAbsenceRepo();
                vm.FiscalYearDetailId = Convert.ToInt32(fid);
                string[] conditionFields = { "a.DepartmentId", "a.ProjectId", "a.SectionId" };
                string[] conditionValues = { departmentId, projectId, sectionId };
                dt = _repo.ExportExcelFile(vm, conditionFields, conditionValues);
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
                result[1] = "Data Saved Successfully!";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportMonthlyAbsence");
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;
                Session["result"] = result[0] + "~" + result[1];
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportMonthlyAbsence");
            }
        }
        

    }
}
