using CrystalDecisions.CrystalReports.Engine;
using SymOrdinary;
using SymRepository.Attendance;
using SymRepository.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    public class DailyAttendanceProcessController : Controller
    {
        //
        // GET: /Common/DailyAttendanceProcess/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        DailyAttendanceProcessRepo _repo = new DailyAttendanceProcessRepo();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _index(string codeFrom, string codeTo, string departmentId, string projectId
            , string sectionId, string designationId
            , string attnGroupId, string attnStId, string dtFrom, string dtTo, string attnStatus
            )
        {
            #region Declare Variable
            AttendanceDailyNewVM vm = new AttendanceDailyNewVM();
            if (codeFrom == "0_0" || codeFrom == "0" || codeFrom == "" || codeFrom == "null" || codeFrom == null)
            {
                codeFrom = "";
            }
            if (codeTo == "0_0" || codeTo == "0" || codeTo == "" || codeTo == "null" || codeTo == null)
            {
                codeTo = "";
            }

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
            vm.AttnStatus = attnStatus;
            #endregion Declare Variable
            List<AttendanceDailyNewVM> VMs = new List<AttendanceDailyNewVM>();

            string[] conFields = { "ve.Code>", "ve.Code<", "ve.DepartmentId", "ve.ProjectId", "ve.SectionId", "ve.DesignationId", "attn.DailyDate>", "attn.DailyDate<" };
            string[] conValues = { codeFrom, codeTo, departmentId, projectId, sectionId, designationId, Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo) };

            VMs = _repo.SelectAll(0, vm, conFields, conValues);
            return PartialView("_index", VMs);
        }



        [HttpPost]
        public ActionResult Create(List<AttendanceDailyNewVM> VMs, string isInTimeUpdate, string updatedInTime
            , string isOutTimeUpdate, string updatedOutTime, string isNextDay)
        {
            string[] result = new string[6];
            string mgs = "";
            AttendanceDailyNewVM vm = new AttendanceDailyNewVM();
            vm.IsInTimeUpdate = Convert.ToBoolean(isInTimeUpdate);
            vm.IsOutTimeUpdate = Convert.ToBoolean(isOutTimeUpdate);

            vm.UpdatedInTime = updatedInTime;
            vm.UpdatedOutTime = updatedOutTime;
            vm.IsNextDay = Convert.ToBoolean(isNextDay);
            //result = _repo.Update(VMs);
            result = _repo.UpdateAttendanceMigration(VMs, vm);
            mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "report").ToString();
            AttendanceDailyNewVM vm = new AttendanceDailyNewVM();
            return View("Report", vm);
        }
        [HttpGet]
        public ActionResult ReportView(string codeFrom, string codeTo, string departmentId, string projectId
            , string sectionId, string dtFrom, string dtTo, string attnStatus, string fullOT)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                //////AttendanceDailyNewVM vm = new AttendanceDailyNewVM();
                EmployeeInfoVM vm = new EmployeeInfoVM();


                if (codeFrom == "0_0" || codeFrom == "0" || codeFrom == "" || codeFrom == "null" || codeFrom == null)
                {
                    codeFrom = "";
                }
                if (codeTo == "0_0" || codeTo == "0" || codeTo == "" || codeTo == "null" || codeTo == null)
                {
                    codeTo = "";
                }

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
               
                vm.AttnStatus = attnStatus;
                string[] conFields = { "ve.Code>", "ve.Code<", "ve.DepartmentId", "ve.ProjectId", "ve.SectionId", "attn.DailyDate>", "attn.DailyDate<" };
                string[] conValues = { codeFrom, codeTo, departmentId, projectId, sectionId, Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo) };

                table = _repo.Report(vm, conFields, conValues);
                ReportHead = "There are no data to Preview for Attendance Daily (" + attnStatus + ")";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "Attendance Daily List (" + attnStatus + ")";
                }
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtDailyAttendanceProcess";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptDailyAttendanceProcess.rpt";

                if (!string.IsNullOrWhiteSpace(codeFrom) && codeFrom == codeTo)
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptDailyAttendanceSingle.rpt";
                }


                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["FullOT"].Text = "'" + fullOT + "'";


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

        //AttendanceDailyToDailyOTnAbsenceProcess
        public ActionResult Process1(string fydid = "", string employeeId = "")
        {
            if (string.IsNullOrWhiteSpace(fydid))
            {
                return View();
            }
            string[] result = new string[6];
            string mgs = "";
            ShampanIdentityVM vm = new ShampanIdentityVM();

            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;

            result = _repo.Process1(fydid, vm, employeeId);

            mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }
        //AttendanceDailyToMonthlyOTnAbsenceProcess
        public ActionResult Process2(string fydid = "", string employeeId = "")
        {
            if (string.IsNullOrWhiteSpace(fydid))
            {
                return View();
            }
            string[] result = new string[6];
            string mgs = "";
            ShampanIdentityVM vm = new ShampanIdentityVM();

            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;

            result = _repo.Process2(fydid, vm, employeeId);

            mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }
        //DailyOTnAbsenceToMonthlyOTnAbsenceProcess
        public ActionResult Process3(string fydid = "", string employeeId = "")
        {
            if (string.IsNullOrWhiteSpace(fydid))
            {
                return View();
            }
            string[] result = new string[6];
            string mgs = "";
            ShampanIdentityVM vm = new ShampanIdentityVM();

            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;

            result = _repo.Process3(fydid, vm, employeeId);

            mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }


        //Calendar Process
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CalendarProcess()
        {
            AttendanceDailyNewVM vm = new AttendanceDailyNewVM();
            return View("CalendarProcess", vm);
        }
        [HttpPost]
        public ActionResult CalendarProcess(string fydid = "")
        {
            string[] result = new string[6];
            string mgs = "";
            try
            {
                result = _repo.CalendarProcess(fydid);
                mgs = result[0] + "~" + result[1];
                return Json(mgs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                mgs = "Fail" + "~" + "Fail";
                return Json(mgs, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
