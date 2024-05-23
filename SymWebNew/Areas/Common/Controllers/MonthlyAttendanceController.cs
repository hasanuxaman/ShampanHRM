using CrystalDecisions.CrystalReports.Engine;
using SymOrdinary;
using SymRepository.Attendance;
using SymViewModel.Attendance;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class MonthlyAttendanceController : Controller
    {
        //
        // GET: /Common/MonthlyAttendance/
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        MonthlyAttendanceRepo _repo = new MonthlyAttendanceRepo();

        public ActionResult Index()
        {
            MonthlyAttendanceVM vm = new MonthlyAttendanceVM();
            return View(vm);
        }


        public ActionResult _index(MonthlyAttendanceVM vm)
        {
            #region Declare Variable
            ////////AttendanceDailyNewVM vm = new AttendanceDailyNewVM();
            ////////if (codeFrom == "0_0" || codeFrom == "0" || codeFrom == "" || codeFrom == "null" || codeFrom == null)
            ////////{
            ////////    codeFrom = "";
            ////////}
            ////////if (codeTo == "0_0" || codeTo == "0" || codeTo == "" || codeTo == "null" || codeTo == null)
            ////////{
            ////////    codeTo = "";
            ////////}

            ////////if (projectId == "0_0" || projectId == "0" || projectId == "" || projectId == "null" || projectId == null)
            ////////{
            ////////    projectId = "";
            ////////}
            ////////if (departmentId == "0_0" || departmentId == "0" || departmentId == "" || departmentId == "null" || departmentId == null)
            ////////{
            ////////    departmentId = "";
            ////////}
            ////////if (sectionId == "0_0" || sectionId == "0" || sectionId == "" || sectionId == "null" || sectionId == null)
            ////////{
            ////////    sectionId = "";
            ////////}
            ////////vm.AttnStatus = attnStatus;
            #endregion Declare Variable
            List<MonthlyAttendanceVM> VMs = new List<MonthlyAttendanceVM>();
            string[] conFields = { "ve.Code>", "ve.Code<", "ve.DepartmentId", "ve.ProjectId", "ve.SectionId", "ve.DesignationId", "ma.FiscalYearDetailId"};
            string[] conValues = { vm.CodeF, vm.CodeT, vm.DepartmentId, vm.ProjectId, vm.SectionId, vm.DesignationId, vm.FiscalYearDetailId.ToString()};

            VMs = _repo.SelectAll(0, conFields, conValues);
            return PartialView("_index", VMs);
        }


        [HttpPost]
        public ActionResult Create(List<MonthlyAttendanceVM> VMs)
        {
            string[] result = new string[6];
            string mgs = "";
            MonthlyAttendanceVM vm = new MonthlyAttendanceVM();

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;

            VMs = VMs.Where(c=> c.IsEmployeeChecked == true).ToList();

            if (VMs.Count == 0)
            {
                return Json("Fail~Please Select Employee First!", JsonRequestBehavior.AllowGet);
            }


            result = _repo.Update(VMs, vm);
            mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Process()
        {
            MonthlyAttendanceVM vm = new MonthlyAttendanceVM();
            return View(vm);
        }

        [HttpGet]
        public ActionResult SelectToInsert(string fid)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            MonthlyAttendanceRepo _repo = new MonthlyAttendanceRepo();
            MonthlyAttendanceVM vm = new MonthlyAttendanceVM();
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                vm.FiscalYearDetailId = Convert.ToInt32(fid);
                result = _repo.SelectToInsert(vm);
                Session["result"] = result[0] + "~" + result[1];
                return View("Process", vm);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("Process", vm);
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            MonthlyAttendanceVM vm = new MonthlyAttendanceVM();
            return View(vm);
        }

        [Authorize]
        public ActionResult ReportView(MonthlyAttendanceVM vm)
        {
            try
            {
                #region Objects & Variables
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                string rptLocation = "";
                string ReportHead = "";

                #endregion
                #region Fields
                //vm.CodeF                     = "";
                //vm.CodeT                     = "";
                //vm.vm.DepartmentId              = "";
                //vm.vm.SectionId                 = "";
                //vm.vm.ProjectId                 = "";
                //vm.vm.DesignationId             = "";
                //vm.MulitpleCode              = "";
                //vm.MulitpleDesignation       = "";
                //vm.MulitpleDepartment        = "";
                //vm.MulitpleSection           = "";
                //vm.MulitpleProject           = "";
                //vm.MulitpleOther1            = "";
                //vm.MulitpleOther2            = "";
                //vm.MulitpleOther3            = "";
                //vm.Other1                    = "";
                //vm.Other2                    = "";
                //vm.Other3                    = "";
                //vm.OrderByCode               = "";
                #endregion

                #region ESS User
                //string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    vm = new MonthlyAttendanceVM();
                    vm.EmployeeId = identity.EmployeeId;
                    vm.CodeF = identity.EmployeeCode;
                    vm.CodeT = identity.EmployeeCode;
                }

                #endregion
                #region Multiple Selection Parameters


                List<string> CodeList = new List<string>();
                List<string> DesignationList = new List<string>();
                List<string> DepartmentList = new List<string>();
                List<string> SectionList = new List<string>();
                List<string> ProjectList = new List<string>();
                List<string> Other1List = new List<string>();
                List<string> Other2List = new List<string>();
                List<string> Other3List = new List<string>();


                if (vm.MultipleCode != "0_0" && vm.MultipleCode != "0" && vm.MultipleCode != "" && vm.MultipleCode != "null" && vm.MultipleCode != null)
                {
                    vm.CodeList = vm.MultipleCode.Split(',').ToList();
                }

                if (vm.MultipleDesignation != "0_0" && vm.MultipleDesignation != "0" && vm.MultipleDesignation != "" && vm.MultipleDesignation != "null" && vm.MultipleDesignation != null)
                {
                    vm.DesignationList = vm.MultipleDesignation.Split(',').ToList();
                }

                if (vm.MultipleDepartment != "0_0" && vm.MultipleDepartment != "0" && vm.MultipleDepartment != "" && vm.MultipleDepartment != "null" && vm.MultipleDepartment != null)
                {
                    vm.DepartmentList = vm.MultipleDepartment.Split(',').ToList();
                }

                if (vm.MultipleSection != "0_0" && vm.MultipleSection != "0" && vm.MultipleSection != "" && vm.MultipleSection != "null" && vm.MultipleSection != null)
                {
                    vm.SectionList = vm.MultipleSection.Split(',').ToList();
                }

                if (vm.MultipleProject != "0_0" && vm.MultipleProject != "0" && vm.MultipleProject != "" && vm.MultipleProject != "null" && vm.MultipleProject != null)
                {
                    vm.ProjectList = vm.MultipleProject.Split(',').ToList();
                }

                if (vm.MultipleOther1 != "0_0" && vm.MultipleOther1 != "0" && vm.MultipleOther1 != "" && vm.MultipleOther1 != "null" && vm.MultipleOther1 != null)
                {
                    vm.Other1List = vm.MultipleOther1.Split(',').ToList();
                }

                if (vm.MultipleOther2 != "0_0" && vm.MultipleOther2 != "0" && vm.MultipleOther2 != "" && vm.MultipleOther2 != "null" && vm.MultipleOther2 != null)
                {
                    vm.Other2List = vm.MultipleOther2.Split(',').ToList();
                }

                if (vm.MultipleOther3 != "0_0" && vm.MultipleOther3 != "0" && vm.MultipleOther3 != "" && vm.MultipleOther3 != "null" && vm.MultipleOther3 != null)
                {
                    vm.Other3List = vm.MultipleOther3.Split(',').ToList();
                }

                //Other1
                //Other2
                //Other3
                #endregion


                string[] conditionFields = { "ve.Code>", "ve.Code<", "ve.DepartmentId", "ve.ProjectId", "ve.SectionId", "ej.Other1", "ej.Other2", "ej.Other3" };
                string[] conditionValues = { vm.CodeF, vm.CodeT, vm.DepartmentId, vm.ProjectId, vm.SectionId, vm.Other1, vm.Other2, vm.Other3 };
                dt = _repo.Report(vm, conditionFields, conditionValues);

                dt.TableName = "dtMonthlyAttendance";



                var FullPeriodName = Convert.ToDateTime("01-" + dt.Rows[0]["PeriodName"].ToString()).ToString("MMMM-yyyy");


                ReportHead = "There are no data to Preview for Monthly Attendance";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Monthly Attendance List";
                }


                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptMonthlyAttendance.rpt";


                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["FullPeriodName"].Text = "'" + FullPeriodName + "'";

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region RenderReportAsPDF

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }

        #endregion RenderReportAsPDF

    }
}
