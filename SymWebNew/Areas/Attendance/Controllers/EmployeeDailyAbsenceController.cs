using CrystalDecisions.CrystalReports.Engine;
using SymOrdinary;
using SymRepository.Attendance;
using SymRepository.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Attendance.Controllers
{
    public class EmployeeDailyAbsenceController : Controller
    {
        //
        // GET: /Attendance/EmployeeDailyAbsence/

        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        EmployeeDailyAbsenceRepo repo = new EmployeeDailyAbsenceRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index(string tType)
        {
            EmployeeDailyAbsenceVM vm = new EmployeeDailyAbsenceVM();
            vm.TransactionType = tType;
            return View(vm);
        }
        public ActionResult _IndexPartial(string ProjectId, string DepartmentId, string SectionId, string date, string tType)
        {
            #region Declare Variable
            string vProjectId = "0_0";
            string vDepartmentId = "0_0";
            string vSectionId = "0_0";

            if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null && ProjectId != "undefined")
            {
                vProjectId = ProjectId;
            }
            if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null && DepartmentId != "undefined")
            {
                vDepartmentId = DepartmentId;
            }
            if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null && SectionId != "undefined")
            {
                vSectionId = SectionId;
            }
            #endregion Declare Variable
            List<EmployeeDailyAbsenceVM> VMs = new List<EmployeeDailyAbsenceVM>();
            VMs = repo.SelectAll(vProjectId, vDepartmentId, vSectionId, date, tType);
            return PartialView("_Index", VMs);
        }

        [HttpPost]
        public ActionResult Create(List<EmployeeDailyAbsenceVM> VMs, string AbsentDate)
        {
            string[] result = new string[6];
            //VMs = VMs.Where(c => c.IsAbsent.ToString().ToLower() == "true").ToList();
            result = repo.Insert(VMs, AbsentDate);
            var mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "70002", "report").ToString();
            EmployeeDailyAbsenceVM vm = new EmployeeDailyAbsenceVM();
            return View("Report", vm);
        }
        [HttpGet]
        public ActionResult ReportView(string departmentId, string projectId, string sectionId, string reportType, string dtFrom, string dtTo)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                EmployeeDailyAbsenceVM vm = new EmployeeDailyAbsenceVM();
                EmployeeDailyAbsenceRepo _repo = new EmployeeDailyAbsenceRepo();
                if (string.IsNullOrWhiteSpace(dtTo))
                {
                    dtTo = dtFrom;
                }
                string[] conditionFields = { "ve.DepartmentId", "ve.ProjectId", "ve.SectionId", "eda.AbsentDate>", "eda.AbsentDate<" };
                string[] conditionValues = { departmentId, projectId, sectionId, Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo) };
                table = _repo.Report(vm, conditionFields, conditionValues);
                ReportHead = "There are no data to Preview for Employee Daily Absence";

                if (table.Rows.Count > 0)
                {
                    ReportHead = "Employee Daily Absence List";
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
                ds.Tables[0].TableName = "dtEmployeeDailyAbsence";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptEmployeeDailyAbsence.rpt";
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

    }
}
