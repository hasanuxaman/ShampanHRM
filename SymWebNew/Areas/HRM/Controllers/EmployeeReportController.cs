using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymOrdinary;
using SymRepository.Attendance;
using SymRepository.Common;
using SymRepository.Enum;
using SymRepository.HRM;
using SymRepository.Leave;
using SymRepository.Payroll;
using SymRepository.PF;
using SymRepository.Tax;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymViewModel.HRM;
using SymViewModel.Leave;
using SymViewModel.Payroll;
using SymWebUI.Areas.HRM.Report;
using SymWebUI.Areas.Payroll.Controllers;
using SymWebUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
namespace SymWebUI.Areas.HRM.Controllers
{
    [Authorize]
    public class EmployeeReportController : Controller
    {
        //
        // GET: /HRM/EmployeeReport/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        EmployeeInfoRepo _repo = new EmployeeInfoRepo();
        private static Thread thread;

       ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        public ActionResult Index()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
            string Employee = "N";
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Employee = "Y";
            }
            ViewBag.Employee = Employee;
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EmployeeList(string RT = null)
        {
            string Employee = "N";
            string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Employee = "Y";
            }
            ViewBag.Employee = Employee;
            ViewBag.RT = RT;
            ViewBag.CompanyName = CompanyName;
            return View();
        }


        [HttpPost]
        public ActionResult EmployeeList(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string Gender_E = null, string Religion = null
            , string GradeId = null, string RT = null
            , string other1 = "", string other2 = "", string other3 = "", string OrderByCode = "" ,string EmpJobType = null,string EmpCategory = null)
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    ProjectId = "";
                    DepartmentId = "";
                    DesignationId = "";
                    dtpFrom = "";
                    dtpTo = "";
                }
                ViewBag.Employee = Employee;
                ViewBag.RT = RT;
                #region Value assign to Parameters
                string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
               , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
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
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }
                #endregion Value assign to Parameters
                ReportDocument doc = new ReportDocument();
                string ReportHead = "";
                string rptLocation = "";
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();


                var getAllData = _empRepo.EmployeeList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, null, EmployeeId, Gender_E, Religion, GradeId, other1, other2, other3, OrderByCode, EmpJobType, EmpCategory);

                SettingRepo _sRepo = new SettingRepo();
                //_sRepo.settingValue("SalarySheet", "SalarySheet(1)");

                string TableName = "";
                if (RT == "Nominee")
                {
                    ReportHead = "There are no data to Preview for Nominee List";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Nominee List";
                        TableName = "dtEmployeeNomeniee";
                    }
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\NewReports\rptNomeniee.rpt"; 
                }
                if (RT == "Dependent")
                {
                    ReportHead = "There are no data to Preview for Dependent List";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Dependent List";
                        TableName = "dtEmployeeDependent";
                    }
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\NewReports\rptDependent.rpt";
                }

                if (RT == "EmpList")
                {
                    ReportHead = "There are no data to Preview for Employee List";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Employee List";
                        TableName = "SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy";
                    }
                    //rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeInfo.rpt";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\" + _sRepo.settingValue("Report", "rptEmployeeInfo") + ".rpt";


                }
                else if (RT == "EmpIDCard")
                {
                    ReportHead = "There are no data to Preview for Employee Id Card";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Employee Id Card";
                        TableName = "SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy";
                    }
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeIDCard.rpt";
                }


                if (OrderByCode == "true")
                {
                    ReportHead = "There are no data to Preview for Employee List";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Employee List";
                        TableName = "SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy";
                    }
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeInfoByCode.rpt";
                }

                string Logo = new AppSettingsReader().GetValue("Logo", typeof(string)).ToString();

                doc.Load(rptLocation);
                doc.Database.Tables[TableName].SetDataSource(getAllData);
                //doc.Database.Tables[0].SetDataSource(SymERPHtmlHelper.ConvertToDataTable<EmployeeInfoVM>(getAllData.ToList()));
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\" + Logo;
                string ImagePath = AppDomain.CurrentDomain.BaseDirectory + "Files\\EmployeeInfo";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'"; 
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
                doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
               // doc.DataDefinition.FormulaFields["ImagePath"].Text = "'" + ImagePath + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpPost]
        public ActionResult EmployeeListDownload(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string Gender_E = null, string Religion = null
            , string GradeId = null, string RT = null
            , string other1 = "", string other2 = "", string other3 = "", string OrderByCode = "", string EmpJobType = null, string EmpCategory = null)
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    ProjectId = "";
                    DepartmentId = "";
                    DesignationId = "";
                    dtpFrom = "";
                    dtpTo = "";
                }
                ViewBag.Employee = Employee;
                ViewBag.RT = RT;
                #region Value assign to Parameters
                string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
               , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
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
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }
                #endregion Value assign to Parameters
                ReportDocument doc = new ReportDocument();
                string ReportHead = "";
                string rptLocation = "";
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                var getAllData = _empRepo.EmployeeList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, null, EmployeeId, Gender_E, Religion, GradeId, other1, other2, other3, OrderByCode, EmpJobType, EmpCategory);

                SettingRepo _sRepo = new SettingRepo();
                //_sRepo.settingValue("SalarySheet", "SalarySheet(1)");
                if (getAllData.Count>0)
                {
                    //
                    DataTable dt = Ordinary.ToDataTable(getAllData);
                    var dataView = new DataView(dt);
                    dt = dataView.ToTable(true, "OtherId", "MiddleName", "Designation",
                        "Department", "Section", "Other3", "JoinDate", "DateOfBirth", "RetirementDate", "Gender_E", "EmploymentType_E", "ServiceLength", "Email", "Supervisor", "SupervisorEmail", "Mobile", "DotedLineManager", "DotedLineManagerEmail");
                    List<string> oldColumnNames = new List<string> { "OtherId", "MiddleName", "Gender_E", "EmploymentType_E", "Supervisor", "Other3" };
                    List<string> newColumnNames = new List<string> { "Employee Code", "Name", "Gender", "Employee Type", "Line Manager Name", "Cost Centers" };

                    dt = Ordinary.DtColumnNameChangeList(dt, oldColumnNames, newColumnNames);
                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("EmployeeInfo");
                    workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

                    string filename = "EmployeeInfo" + "-" + DateTime.Now.ToString("yyyyMMdd");
                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                        excel.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }

                return Redirect("EmployeeList");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region EmployeeInformationAll
        public ActionResult DownloadEmployeeInformationAll(HttpPostedFileBase file, string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string Gender, string Religion
            , string GradeId, string MulitpleColumn, string OrderByCode = "")
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
            string[] result = new string[6];
            DataTable dt = new DataTable();


            #region Value assign to Parameters
            string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
           , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
            string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
           , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
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
            if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
            {
                vdtpFrom = dtpFrom;
                dtpFromParam = vdtpFrom;
            }
            if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
            {
                vdtpTo = dtpTo;
                dtpToParam = vdtpTo;
            }
            #endregion Value assign to Parameters
            try
            {

                List<string> MulitpleColumnList = new List<string>();
                //MulitpleColumnList = MulitpleColumn.Split(',').ToList();
                string[] columnList = MulitpleColumn.Split(',');
                DataTable dtNew = new DataTable();

                if (MulitpleColumn != "0_0" && MulitpleColumn != "0" && MulitpleColumn != "" && MulitpleColumn != "null" && MulitpleColumn != null)
                {
                    if (columnList.Length >= 1)
                    {
                        for (int i = 0; i < columnList.Length; i++)
                        {
                            dtNew.Columns.Add(columnList[i], typeof(string));
                        }
                    }
                }



                string FileName = "Download.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                //string fullPath = @"C:\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                List<ViewEmployeeInfoAllVM> getAllData = new List<ViewEmployeeInfoAllVM>();
                dt = _empRepo.ExportExcelFile(fullPath, FileName, vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                , vdtpFrom, vdtpTo, Gender, Religion, GradeId, OrderByCode);

                if (MulitpleColumn != "0_0" && MulitpleColumn != "0" && MulitpleColumn != "" && MulitpleColumn != "null" && MulitpleColumn != null)
                {
                    foreach (DataRow itemdt in dt.Rows)
                    {
                        DataRow dr1 = dtNew.NewRow();
                        for (int i = 0; i < columnList.Length; i++)
                        {
                            dr1[columnList[i]] = itemdt[columnList[i]].ToString();
                        }

                        dtNew.Rows.Add(dr1.ItemArray);
                    }
                }
                else
                {
                    dtNew = dt;
                }

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + FileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    using (System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw))
                    {
                        GridView grid = new GridView();
                        grid.DataSource = dtNew;
                        grid.DataBind();
                        grid.RenderControl(htw);
                        Response.Write(sw.ToString());
                    }
                }
                Response.End();
                Session["result"] = result[0] + "~" + result[1];
                return Redirect("EmployeeInformationAll");
                //return Redirect("C:/" + FileName);
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return Redirect("EmployeeInformationAll");
            }
        }
        [Authorize]
        public ActionResult EmployeeInformationAll(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string ReportNo, string Gender, string Religion, string GradeId)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
            string Employee = "N";
            string EmployeeId = "";
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                EmployeeId = identity.EmployeeId;
                Employee = "Y";
                CodeF = identity.EmployeeCode;
                CodeT = identity.EmployeeCode;
                //Name = "";
                ProjectId = "";
                DepartmentId = "";
                dtpFrom = "";
                dtpTo = "";
            }
            ViewBag.Employee = Employee;
            if (string.IsNullOrWhiteSpace(ReportNo))
            {
                return View();
            }
            #region Value assign to Parameters
            string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
           , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
            string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
           , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
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
            if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
            {
                vdtpFrom = dtpFrom;
                dtpFromParam = vdtpFrom;
            }
            if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
            {
                vdtpTo = dtpTo;
                dtpToParam = vdtpTo;
            }
            #endregion Value assign to Parameters
            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            var getAllData = _empRepo.EmployeeInformationAll(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                , vdtpFrom, vdtpTo, Gender, Religion, GradeId);
            string ReportHead = "";
            ReportHead = "There are no data to Preview for Employee List";
            if (getAllData.Count > 0)
            {
                ReportHead = "Employee List";
            }
            return View();
        }
        public ActionResult _EmpInfoAllIndexPartial(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string Gender, string Religion
            , string GradeId, string MulitpleColumn, string OrderByCode = "")
        {
            ViewEmployeeInfoAllVM vm = new ViewEmployeeInfoAllVM();
            vm.CodeF = CodeF;
            vm.CodeT = CodeT;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.ProjectId = ProjectId;
            vm.DesignationId = DesignationId;
            vm.dtpFrom = dtpFrom;
            vm.dtpTo = dtpTo;
            vm.Gender = Gender;
            vm.Religion = Religion;
            vm.GradeId = GradeId;
            vm.MulitpleColumn = MulitpleColumn;

            return PartialView("_EmpInfoAllIndex", vm);
        }
        public ActionResult _EmpInfoAllIndex(JQueryDataTableParamVM param, string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string Gender, string Religion
            , string GradeId, string MulitpleColumn, string OrderByCode = "")
        {
            #region Comments
            /*
            Code 			
            EmpName           
            Designation		
            JoinDate
             * Grade
            BasicSalary
            GrossSalary
            Branch
            Department
            Section			
            Project 		
             * --EmployeeInfo
//ei.EmployeeId, ei.Code, ei.Salutation, ei.MiddleName, ei.LastName, ei.EmpName, ei.AttnUserId
//--EmployeeJob
//, ei.JoinDate
, ei.ProbationEnd
, ei.DateOfPermanent
, ei.LeftDate
//, ei.GrossSalary
//, ei.BasicSalary
, ei.IsPermanent
, ei.Supervisor
, ei.BankInfo
, ei.BankAccountNo
, ei.EmploymentStatus
, ei.EmploymentType
//, ei.Branch         , ei.BranchId
//, ei.Department     , ei.DepartmentId
//, ei.Section        , ei.SectionId
//, ei.Project        , ei.ProjectId
//, ei.Designation    , ei.DesignationId
//, ei.Grade          , ei.GradeId 
//--Transfer, Promotion
, ei.TransferDate
, ei.IsPromotion
, ei.PromotionDate
//--EmployeePersonalDetail
, ei.PersonalEmail
, ei.NickName	
, ei.DateOfBirth
, ei.Religion
, ei.BloodGroup
, ei.Gender			
, ei.MaritalStatus
, ei.Nationality
, ei.Smoker
, ei.NID
, ei.PassportNumber
, ei.ExpiryDate
, ei.TIN
, ei.IsDisable
, ei.KindsOfDisability
, ei.OtherId
//--EmployeePersonalDetail
, ei.PresentAddress
, ei.PresentMobile
, ei.PresentDistrict
, ei.PresentDivision
, ei.PresentCountry
, ei.PresentCity
, ei.PresentPostalCode
, ei.PresentPhone
, ei.PresentFax
//--EmployeePermanentAddress
, ei.PermanentAddress
, ei.PermanentMobile  
, ei.PermanentDistrict
, ei.PermanentDivision
, ei.PermanentCountry
, ei.PermanentCity
, ei.PermanentPostalCode
, ei.PermanentPhone
, ei.PermanentFax
//--EmployeeEmergencyContact
, ei.EmConName
, ei.EmConRelation
, ei.EmConAddress
, ei.EmConDistrict
, ei.EmConDivision
, ei.EmConCountry
, ei.EmConCity
, ei.EmConPostalCode
, ei.EmConPhone
, ei.EmConMobile
, ei.EmConFax
, ei.EducationDegree       
, ei.EducationInstitute    
, ei.EducationMajor        
, ei.EducationYearOfPassing
, ei.EducationTotalYear	                
             */
            #endregion Comments
            #region Column Search
            var CodeFilter = Convert.ToString(Request["sSearch_0"]);
            var EmpNameFilter = Convert.ToString(Request["sSearch_1"]);
            var DesignationFilter = Convert.ToString(Request["sSearch_2"]);
            var JoinDateFilter = Convert.ToString(Request["sSearch_3"]);
            var GradeFilter = Convert.ToString(Request["sSearch_4"]);
            var BasicSalary = Convert.ToString(Request["sSearch_5"]);
            var GrossSalary = Convert.ToString(Request["sSearch_6"]);
            var BranchFilter = Convert.ToString(Request["sSearch_7"]);
            var DepartmentFilter = Convert.ToString(Request["sSearch_8"]);
            var SectionFilter = Convert.ToString(Request["sSearch_9"]);//-->EmployeeJob
            var ProjectFilter = Convert.ToString(Request["sSearch_10"]);
            var ProbationEndFilter = Convert.ToString(Request["sSearch_11"]);
            var DateOfPermanentFilter = Convert.ToString(Request["sSearch_12"]);
            var IsPermanentFilter = Convert.ToString(Request["sSearch_13"]);
            var SupervisorFilter = Convert.ToString(Request["sSearch_14"]);
            var BankInfoFilter = Convert.ToString(Request["sSearch_15"]);
            var BankAccountNoFilter = Convert.ToString(Request["sSearch_16"]);
            var EmploymentStatusFilter = Convert.ToString(Request["sSearch_17"]);
            var EmploymentTypeFilter = Convert.ToString(Request["sSearch_18"]);//-->Transfer, Promotion                    
            var TransferDateFilter = Convert.ToString(Request["sSearch_19"]);
            var PromotionDateFilter = Convert.ToString(Request["sSearch_20"]);//-->EmployeePersonalDetail               
            var PersonalEmailFilter = Convert.ToString(Request["sSearch_21"]);
            var NickNameFilter = Convert.ToString(Request["sSearch_22"]);
            var DateOfBirthFilter = Convert.ToString(Request["sSearch_23"]);
            var ReligionFilter = Convert.ToString(Request["sSearch_24"]);
            var BloodGroupFilter = Convert.ToString(Request["sSearch_25"]);
            var GenderFilter = Convert.ToString(Request["sSearch_26"]);
            var MaritalStatusFilter = Convert.ToString(Request["sSearch_27"]);
            var NationalityFilter = Convert.ToString(Request["sSearch_28"]);
            var SmokerFilter = Convert.ToString(Request["sSearch_29"]);
            var NIDFilter = Convert.ToString(Request["sSearch_30"]);
            var PassportNumberFilter = Convert.ToString(Request["sSearch_31"]);
            var ExpiryDateFilter = Convert.ToString(Request["sSearch_32"]);
            var TINFilter = Convert.ToString(Request["sSearch_33"]);
            var IsDisableFilter = Convert.ToString(Request["sSearch_34"]);
            var KindsOfDisabilityFilter = Convert.ToString(Request["sSearch_35"]);
            var OtherIdFilter = Convert.ToString(Request["sSearch_36"]);//-->Employee PresentAddress              
            var PresentAddressFilter = Convert.ToString(Request["sSearch_37"]);
            var PresentMobileFilter = Convert.ToString(Request["sSearch_38"]);
            var PresentDistrictFilter = Convert.ToString(Request["sSearch_39"]);
            var PresentDivisionFilter = Convert.ToString(Request["sSearch_40"]);
            var PresentCountryFilter = Convert.ToString(Request["sSearch_41"]);
            var PresentCityFilter = Convert.ToString(Request["sSearch_42"]);
            var PresentPostalCodeFilter = Convert.ToString(Request["sSearch_43"]);
            var PresentPhoneFilter = Convert.ToString(Request["sSearch_44"]);
            var PresentFaxFilter = Convert.ToString(Request["sSearch_45"]);//-->Employee PermanentAddress            
            var PermanentAddressFilter = Convert.ToString(Request["sSearch_46"]);
            var PermanentMobileFilter = Convert.ToString(Request["sSearch_47"]);
            var PermanentDistrictFilter = Convert.ToString(Request["sSearch_48"]);
            var PermanentDivisionFilter = Convert.ToString(Request["sSearch_49"]);
            var PermanentCountryFilter = Convert.ToString(Request["sSearch_50"]);
            var PermanentCityFilter = Convert.ToString(Request["sSearch_51"]);
            var PermanentPostalCodeFilter = Convert.ToString(Request["sSearch_52"]);//-->Employee EmergencyContact    
            var EmConNameFilter = Convert.ToString(Request["sSearch_53"]);
            var EmConRelationFilter = Convert.ToString(Request["sSearch_54"]);
            var EmConAddressFilter = Convert.ToString(Request["sSearch_55"]);
            var EmConCountryFilter = Convert.ToString(Request["sSearch_56"]);
            var EmConMobileFilter = Convert.ToString(Request["sSearch_57"]);//-->Employee Education
            var EducationDegreeFilter = Convert.ToString(Request["sSearch_58"]);
            var EducationInstituteFilter = Convert.ToString(Request["sSearch_59"]);
            var EducationMajorFilter = Convert.ToString(Request["sSearch_60"]);
            var EducationYearOfPassingFilter = Convert.ToString(Request["sSearch_61"]);
            var EducationTotalYearFilter = Convert.ToString(Request["sSearch_62"]);
            #endregion Column Search
            #region Value assign to Parameters
            string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
           , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
            string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
           , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
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
            if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
            {
                vdtpFrom = dtpFrom;
                dtpFromParam = vdtpFrom;
            }
            if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
            {
                vdtpTo = dtpTo;
                dtpToParam = vdtpTo;
            }
            #endregion Value assign to Parameters
            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            List<ViewEmployeeInfoAllVM> getAllData = new List<ViewEmployeeInfoAllVM>();
            List<string> MulitpleColumnList = new List<string>();
            MulitpleColumnList = MulitpleColumn.Split(',').ToList();


            getAllData = _empRepo.EmployeeInformationAll(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                , vdtpFrom, vdtpTo, Gender, Religion, GradeId, OrderByCode);
            IEnumerable<ViewEmployeeInfoAllVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                #region Top Searchable Declare
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
                var isSearchable11 = Convert.ToBoolean(Request["bSearchable_11"]);
                var isSearchable12 = Convert.ToBoolean(Request["bSearchable_12"]);
                var isSearchable13 = Convert.ToBoolean(Request["bSearchable_13"]);
                var isSearchable14 = Convert.ToBoolean(Request["bSearchable_14"]);
                var isSearchable15 = Convert.ToBoolean(Request["bSearchable_15"]);
                var isSearchable16 = Convert.ToBoolean(Request["bSearchable_16"]);
                var isSearchable17 = Convert.ToBoolean(Request["bSearchable_17"]);
                var isSearchable18 = Convert.ToBoolean(Request["bSearchable_18"]);
                var isSearchable19 = Convert.ToBoolean(Request["bSearchable_19"]);
                var isSearchable20 = Convert.ToBoolean(Request["bSearchable_20"]);
                var isSearchable21 = Convert.ToBoolean(Request["bSearchable_21"]);
                var isSearchable22 = Convert.ToBoolean(Request["bSearchable_22"]);
                var isSearchable23 = Convert.ToBoolean(Request["bSearchable_23"]);
                var isSearchable24 = Convert.ToBoolean(Request["bSearchable_24"]);
                var isSearchable25 = Convert.ToBoolean(Request["bSearchable_25"]);
                var isSearchable26 = Convert.ToBoolean(Request["bSearchable_26"]);
                var isSearchable27 = Convert.ToBoolean(Request["bSearchable_27"]);
                var isSearchable28 = Convert.ToBoolean(Request["bSearchable_28"]);
                var isSearchable29 = Convert.ToBoolean(Request["bSearchable_29"]);
                var isSearchable30 = Convert.ToBoolean(Request["bSearchable_30"]);
                var isSearchable31 = Convert.ToBoolean(Request["bSearchable_31"]);
                var isSearchable32 = Convert.ToBoolean(Request["bSearchable_32"]);
                var isSearchable33 = Convert.ToBoolean(Request["bSearchable_33"]);
                var isSearchable34 = Convert.ToBoolean(Request["bSearchable_34"]);
                var isSearchable35 = Convert.ToBoolean(Request["bSearchable_35"]);
                var isSearchable36 = Convert.ToBoolean(Request["bSearchable_36"]);
                var isSearchable37 = Convert.ToBoolean(Request["bSearchable_37"]);
                var isSearchable38 = Convert.ToBoolean(Request["bSearchable_38"]);
                var isSearchable39 = Convert.ToBoolean(Request["bSearchable_39"]);
                var isSearchable40 = Convert.ToBoolean(Request["bSearchable_40"]);
                var isSearchable41 = Convert.ToBoolean(Request["bSearchable_41"]);
                var isSearchable42 = Convert.ToBoolean(Request["bSearchable_42"]);
                var isSearchable43 = Convert.ToBoolean(Request["bSearchable_43"]);
                var isSearchable44 = Convert.ToBoolean(Request["bSearchable_44"]);
                var isSearchable45 = Convert.ToBoolean(Request["bSearchable_45"]);
                var isSearchable46 = Convert.ToBoolean(Request["bSearchable_46"]);
                var isSearchable47 = Convert.ToBoolean(Request["bSearchable_47"]);
                var isSearchable48 = Convert.ToBoolean(Request["bSearchable_48"]);
                var isSearchable49 = Convert.ToBoolean(Request["bSearchable_49"]);
                var isSearchable50 = Convert.ToBoolean(Request["bSearchable_50"]);
                var isSearchable51 = Convert.ToBoolean(Request["bSearchable_51"]);
                var isSearchable52 = Convert.ToBoolean(Request["bSearchable_52"]);
                var isSearchable53 = Convert.ToBoolean(Request["bSearchable_53"]);
                var isSearchable54 = Convert.ToBoolean(Request["bSearchable_54"]);
                var isSearchable55 = Convert.ToBoolean(Request["bSearchable_55"]);
                var isSearchable56 = Convert.ToBoolean(Request["bSearchable_56"]);
                var isSearchable57 = Convert.ToBoolean(Request["bSearchable_57"]);
                var isSearchable58 = Convert.ToBoolean(Request["bSearchable_58"]);
                var isSearchable59 = Convert.ToBoolean(Request["bSearchable_59"]);
                var isSearchable60 = Convert.ToBoolean(Request["bSearchable_60"]);
                var isSearchable61 = Convert.ToBoolean(Request["bSearchable_61"]);
                var isSearchable62 = Convert.ToBoolean(Request["bSearchable_62"]);
                #endregion Top Searchable Declare
                #region Filtered Data
                filteredData = getAllData.Where(c =>
                       isSearchable0 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable1 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.JoinDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Grade.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.Branch.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable8 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable9 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower()) //--EmployeeJob
                    || isSearchable10 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable11 && c.ProbationEnd.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable12 && c.DateOfPermanent.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable13 && c.IsPermanent.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable14 && c.Supervisor.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable15 && c.BankInfo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable16 && c.BankAccountNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable17 && c.EmploymentStatus.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable18 && c.EmploymentType.ToString().ToLower().Contains(param.sSearch.ToLower())//--Transfer, Promotion
                    || isSearchable19 && c.TransferDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable20 && c.PromotionDate.ToString().ToLower().Contains(param.sSearch.ToLower())//--EmployeePersonalDetail
                    || isSearchable21 && c.PersonalEmail.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable22 && c.NickName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable23 && c.DateOfBirth.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable24 && c.Religion.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable25 && c.BloodGroup.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable26 && c.Gender.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable27 && c.MaritalStatus.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable28 && c.Nationality.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable29 && c.Smoker.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable30 && c.NID.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable31 && c.PassportNumber.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable32 && c.ExpiryDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable33 && c.TIN.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable34 && c.IsDisable.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable35 && c.KindsOfDisability.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable36 && c.OtherId.ToString().ToLower().Contains(param.sSearch.ToLower())//--EmployeePresentAddress
                    || isSearchable37 && c.PresentAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable38 && c.PresentMobile.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable39 && c.PresentDistrict.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable40 && c.PresentDivision.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable41 && c.PresentCountry.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable42 && c.PresentCity.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable43 && c.PresentPostalCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable44 && c.PresentPhone.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable45 && c.PresentFax.ToString().ToLower().Contains(param.sSearch.ToLower())//--EmployeePermanentAddress
                    || isSearchable46 && c.PermanentAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable47 && c.PermanentMobile.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable48 && c.PermanentDistrict.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable49 && c.PermanentDivision.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable50 && c.PermanentCountry.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable51 && c.PermanentCity.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable52 && c.PermanentPostalCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable53 && c.EmConName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable54 && c.EmConRelation.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable55 && c.EmConAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable56 && c.EmConCountry.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable57 && c.EmConMobile.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable58 && c.EducationDegree.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable59 && c.EducationInstitute.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable60 && c.EducationMajor.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable61 && c.EducationYearOfPassing.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable62 && c.EducationTotalYear.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
                #endregion Filtered Data
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (CodeFilter != ""                                             // 0 
                || EmpNameFilter != ""                    // 1 
                || DesignationFilter != ""                    // 2 
                || JoinDateFilter != ""                    // 3 
                || GradeFilter != ""                    // 4 
                || BasicSalary != ""                    // 5 
                || GrossSalary != ""                    // 6        
                || BranchFilter != ""                    // 7 
                || DepartmentFilter != ""                    // 8 
                || SectionFilter != ""                    // 9 
                || ProjectFilter != ""                    // 10
                || ProbationEndFilter != ""                    // 11
                || DateOfPermanentFilter != ""                    // 12
                || IsPermanentFilter != ""                    // 13
                || SupervisorFilter != ""                    // 14
                || BankInfoFilter != ""                    // 15
                || BankAccountNoFilter != ""                    // 16
                || EmploymentStatusFilter != ""                    // 17
                || EmploymentTypeFilter != ""                    // 18
                || TransferDateFilter != ""                    // 19
                || PromotionDateFilter != ""                    // 20
                || PersonalEmailFilter != ""                    // 21
                || NickNameFilter != ""                    // 22
                || DateOfBirthFilter != ""                    // 23
                || ReligionFilter != ""                    // 24
                || BloodGroupFilter != ""                    // 25
                || GenderFilter != ""                    // 26
                || MaritalStatusFilter != ""                    // 27
                || NationalityFilter != ""                    // 28
                || SmokerFilter != ""                    // 29  
                || NIDFilter != ""                    // 30
                || PassportNumberFilter != ""                    // 31
                || ExpiryDateFilter != ""                    // 32
                || TINFilter != ""                    // 33
                || IsDisableFilter != ""                    // 34
                || KindsOfDisabilityFilter != ""                    // 35
                || OtherIdFilter != ""                    // 36
                || PresentAddressFilter != ""                    // 37
                || PresentMobileFilter != ""                    // 38
                || PresentDistrictFilter != ""                    // 39
                || PresentDivisionFilter != ""                    // 40
                || PresentCountryFilter != ""                    // 41
                || PresentCityFilter != ""                    // 42
                || PresentPostalCodeFilter != ""                    // 43
                || PresentPhoneFilter != ""                    // 44
                || PresentFaxFilter != ""                    // 45
                || PermanentAddressFilter != ""                    // 46
                || PermanentMobileFilter != ""                    // 47
                || PermanentDistrictFilter != ""                    // 48
                || PermanentDivisionFilter != ""                    // 49
                || PermanentCountryFilter != ""                    // 50
                || PermanentCityFilter != ""                    // 51
                || PermanentPostalCodeFilter != ""                    // 52
                || EmConNameFilter != ""                    // 53
                || EmConRelationFilter != ""                    // 54
                || EmConAddressFilter != ""                    // 55
                || EmConCountryFilter != ""                    // 56
                || EmConMobileFilter != ""                    // 57
                || EducationDegreeFilter != ""                    // 58
                || EducationInstituteFilter != ""                    // 59
                || EducationMajorFilter != ""                    // 60
                || EducationYearOfPassingFilter != ""                    // 61
                || EducationTotalYearFilter != ""                    // 62
                )
            {
                filteredData = filteredData.Where(c =>
                    (CodeFilter == "" || c.Code.ToString().ToLower().Contains(CodeFilter.ToLower()))                                                    // 0 
                    && (EmpNameFilter == "" || c.EmpName.ToString().ToLower().Contains(EmpNameFilter.ToLower()))                       // 1 
                    && (DesignationFilter == "" || c.Designation.ToString().ToLower().Contains(DesignationFilter.ToLower()))                       // 2 
                    && (JoinDateFilter == "" || c.JoinDate.ToString().ToLower().Contains(JoinDateFilter.ToLower()))                       // 3 
                    && (GradeFilter == "" || c.Grade.ToString().ToLower().Contains(GradeFilter.ToLower()))                       // 4 
                    && (BasicSalary == "" || c.BasicSalary.ToString().ToLower().Contains(BasicSalary.ToLower()))                       // 5 
                    && (GrossSalary == "" || c.GrossSalary.ToString().ToLower().Contains(GrossSalary.ToLower()))                       // 6        
                    && (BranchFilter == "" || c.Branch.ToString().ToLower().Contains(BranchFilter.ToLower()))                       // 7 
                    && (DepartmentFilter == "" || c.Department.ToString().ToLower().Contains(DepartmentFilter.ToLower()))                       // 8 
                    && (SectionFilter == "" || c.Section.ToString().ToLower().Contains(SectionFilter.ToLower()))                       // 9 
                    && (ProjectFilter == "" || c.Project.ToString().ToLower().Contains(ProjectFilter.ToLower()))                       // 10
                    && (ProbationEndFilter == "" || c.ProbationEnd.ToString().ToLower().Contains(ProbationEndFilter.ToLower()))                       // 11
                    && (DateOfPermanentFilter == "" || c.DateOfPermanent.ToString().ToLower().Contains(DateOfPermanentFilter.ToLower()))                       // 12
                    && (IsPermanentFilter == "" || c.IsPermanent.ToString().ToLower().Contains(IsPermanentFilter.ToLower()))                       // 13
                    && (SupervisorFilter == "" || c.Supervisor.ToString().ToLower().Contains(SupervisorFilter.ToLower()))                       // 14
                    && (BankInfoFilter == "" || c.BankInfo.ToString().ToLower().Contains(BankInfoFilter.ToLower()))                       // 15
                    && (BankAccountNoFilter == "" || c.BankAccountNo.ToString().ToLower().Contains(BankAccountNoFilter.ToLower()))                       // 16
                    && (EmploymentStatusFilter == "" || c.EmploymentStatus.ToString().ToLower().Contains(EmploymentStatusFilter.ToLower()))                       // 17
                    && (EmploymentTypeFilter == "" || c.EmploymentType.ToString().ToLower().Contains(EmploymentTypeFilter.ToLower()))                       // 18
                    && (TransferDateFilter == "" || c.TransferDate.ToString().ToLower().Contains(TransferDateFilter.ToLower()))                       // 19
                    && (PromotionDateFilter == "" || c.PromotionDate.ToString().ToLower().Contains(PromotionDateFilter.ToLower()))                       // 20
                    && (PersonalEmailFilter == "" || c.PersonalEmail.ToString().ToLower().Contains(PersonalEmailFilter.ToLower()))                       // 21
                    && (NickNameFilter == "" || c.NickName.ToString().ToLower().Contains(NickNameFilter.ToLower()))                       // 22
                    && (DateOfBirthFilter == "" || c.DateOfBirth.ToString().ToLower().Contains(DateOfBirthFilter.ToLower()))                       // 23
                    && (ReligionFilter == "" || c.Religion.ToString().ToLower().Contains(ReligionFilter.ToLower()))                       // 24
                    && (BloodGroupFilter == "" || c.BloodGroup.ToString().ToLower().Contains(BloodGroupFilter.ToLower()))                       // 25
                    && (GenderFilter == "" || c.Gender.ToString().ToLower().Contains(GenderFilter.ToLower()))                       // 26
                    && (MaritalStatusFilter == "" || c.MaritalStatus.ToString().ToLower().Contains(MaritalStatusFilter.ToLower()))                       // 27
                    && (NationalityFilter == "" || c.Nationality.ToString().ToLower().Contains(NationalityFilter.ToLower()))                       // 28
                    && (SmokerFilter == "" || c.Smoker.ToString().ToLower().Contains(SmokerFilter.ToLower()))                       // 29  
                    && (NIDFilter == "" || c.NID.ToString().ToLower().Contains(NIDFilter.ToLower()))                       // 30
                    && (PassportNumberFilter == "" || c.PassportNumber.ToString().ToLower().Contains(PassportNumberFilter.ToLower()))                       // 31
                    && (ExpiryDateFilter == "" || c.ExpiryDate.ToString().ToLower().Contains(ExpiryDateFilter.ToLower()))                       // 32
                    && (TINFilter == "" || c.TIN.ToString().ToLower().Contains(TINFilter.ToLower()))                       // 33
                    && (IsDisableFilter == "" || c.IsDisable.ToString().ToLower().Contains(IsDisableFilter.ToLower()))                       // 34
                    && (KindsOfDisabilityFilter == "" || c.KindsOfDisability.ToString().ToLower().Contains(KindsOfDisabilityFilter.ToLower()))                       // 35
                    && (OtherIdFilter == "" || c.OtherId.ToString().ToLower().Contains(OtherIdFilter.ToLower()))                       // 36
                    && (PresentAddressFilter == "" || c.PresentAddress.ToString().ToLower().Contains(PresentAddressFilter.ToLower()))                       // 37
                    && (PresentMobileFilter == "" || c.PresentMobile.ToString().ToLower().Contains(PresentMobileFilter.ToLower()))                       // 38
                    && (PresentDistrictFilter == "" || c.PresentDistrict.ToString().ToLower().Contains(PresentDistrictFilter.ToLower()))                       // 39
                    && (PresentDivisionFilter == "" || c.PresentDivision.ToString().ToLower().Contains(PresentDivisionFilter.ToLower()))                       // 40
                    && (PresentCountryFilter == "" || c.PresentCountry.ToString().ToLower().Contains(PresentCountryFilter.ToLower()))                       // 41
                    && (PresentCityFilter == "" || c.PresentCity.ToString().ToLower().Contains(PresentCityFilter.ToLower()))                       // 42
                    && (PresentPostalCodeFilter == "" || c.PresentPostalCode.ToString().ToLower().Contains(PresentPostalCodeFilter.ToLower()))                       // 43
                    && (PresentPhoneFilter == "" || c.PresentPhone.ToString().ToLower().Contains(PresentPhoneFilter.ToLower()))                       // 44
                    && (PresentFaxFilter == "" || c.PresentFax.ToString().ToLower().Contains(PresentFaxFilter.ToLower()))                       // 45
                    && (PermanentAddressFilter == "" || c.PermanentAddress.ToString().ToLower().Contains(PermanentAddressFilter.ToLower()))                       // 46
                    && (PermanentMobileFilter == "" || c.PermanentMobile.ToString().ToLower().Contains(PermanentMobileFilter.ToLower()))                       // 47
                    && (PermanentDistrictFilter == "" || c.PermanentDistrict.ToString().ToLower().Contains(PermanentDistrictFilter.ToLower()))                       // 48
                    && (PermanentDivisionFilter == "" || c.PermanentDivision.ToString().ToLower().Contains(PermanentDivisionFilter.ToLower()))                       // 49
                    && (PermanentCountryFilter == "" || c.PermanentCountry.ToString().ToLower().Contains(PermanentCountryFilter.ToLower()))                       // 50
                    && (PermanentCityFilter == "" || c.PermanentCity.ToString().ToLower().Contains(PermanentCityFilter.ToLower()))                       // 51
                    && (PermanentPostalCodeFilter == "" || c.PermanentPostalCode.ToString().ToLower().Contains(PermanentPostalCodeFilter.ToLower()))                       // 52
                    && (EmConNameFilter == "" || c.EmConName.ToString().ToLower().Contains(EmConNameFilter.ToLower()))                       // 53
                    && (EmConRelationFilter == "" || c.EmConRelation.ToString().ToLower().Contains(EmConRelationFilter.ToLower()))                       // 54
                    && (EmConAddressFilter == "" || c.EmConAddress.ToString().ToLower().Contains(EmConAddressFilter.ToLower()))                       // 55
                    && (EmConCountryFilter == "" || c.EmConCountry.ToString().ToLower().Contains(EmConCountryFilter.ToLower()))                       // 56
                    && (EmConMobileFilter == "" || c.EmConMobile.ToString().ToLower().Contains(EmConMobileFilter.ToLower()))                       // 57
                    && (EducationDegreeFilter == "" || c.EducationDegree.ToString().ToLower().Contains(EducationDegreeFilter.ToLower()))                       // 58
                    && (EducationInstituteFilter == "" || c.EducationInstitute.ToString().ToLower().Contains(EducationInstituteFilter.ToLower()))                       // 59
                    && (EducationMajorFilter == "" || c.EducationMajor.ToString().ToLower().Contains(EducationMajorFilter.ToLower()))                       // 60
                    && (EducationYearOfPassingFilter == "" || c.EducationYearOfPassing.ToString().ToLower().Contains(EducationYearOfPassingFilter.ToLower()))                       // 61
                    && (EducationTotalYearFilter == "" || c.EducationTotalYear.ToString().ToLower().Contains(EducationTotalYearFilter.ToLower()))                       // 62
                    );
            }
            #endregion Column Filtering
            #region isSortable
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
            var isSortable_11 = Convert.ToBoolean(Request["bSortable_11"]);
            var isSortable_12 = Convert.ToBoolean(Request["bSortable_12"]);
            var isSortable_13 = Convert.ToBoolean(Request["bSortable_13"]);
            var isSortable_14 = Convert.ToBoolean(Request["bSortable_14"]);
            var isSortable_15 = Convert.ToBoolean(Request["bSortable_15"]);
            var isSortable_16 = Convert.ToBoolean(Request["bSortable_16"]);
            var isSortable_17 = Convert.ToBoolean(Request["bSortable_17"]);
            var isSortable_18 = Convert.ToBoolean(Request["bSortable_18"]);
            var isSortable_19 = Convert.ToBoolean(Request["bSortable_19"]);
            var isSortable_20 = Convert.ToBoolean(Request["bSortable_20"]);
            var isSortable_21 = Convert.ToBoolean(Request["bSortable_21"]);
            var isSortable_22 = Convert.ToBoolean(Request["bSortable_22"]);
            var isSortable_23 = Convert.ToBoolean(Request["bSortable_23"]);
            var isSortable_24 = Convert.ToBoolean(Request["bSortable_24"]);
            var isSortable_25 = Convert.ToBoolean(Request["bSortable_25"]);
            var isSortable_26 = Convert.ToBoolean(Request["bSortable_26"]);
            var isSortable_27 = Convert.ToBoolean(Request["bSortable_27"]);
            var isSortable_28 = Convert.ToBoolean(Request["bSortable_28"]);
            var isSortable_29 = Convert.ToBoolean(Request["bSortable_29"]);
            var isSortable_30 = Convert.ToBoolean(Request["bSortable_30"]);
            var isSortable_31 = Convert.ToBoolean(Request["bSortable_31"]);
            var isSortable_32 = Convert.ToBoolean(Request["bSortable_32"]);
            var isSortable_33 = Convert.ToBoolean(Request["bSortable_33"]);
            var isSortable_34 = Convert.ToBoolean(Request["bSortable_34"]);
            var isSortable_35 = Convert.ToBoolean(Request["bSortable_35"]);
            var isSortable_36 = Convert.ToBoolean(Request["bSortable_36"]);
            var isSortable_37 = Convert.ToBoolean(Request["bSortable_37"]);
            var isSortable_38 = Convert.ToBoolean(Request["bSortable_38"]);
            var isSortable_39 = Convert.ToBoolean(Request["bSortable_39"]);
            var isSortable_40 = Convert.ToBoolean(Request["bSortable_40"]);
            var isSortable_41 = Convert.ToBoolean(Request["bSortable_41"]);
            var isSortable_42 = Convert.ToBoolean(Request["bSortable_42"]);
            var isSortable_43 = Convert.ToBoolean(Request["bSortable_43"]);
            var isSortable_44 = Convert.ToBoolean(Request["bSortable_44"]);
            var isSortable_45 = Convert.ToBoolean(Request["bSortable_45"]);
            var isSortable_46 = Convert.ToBoolean(Request["bSortable_46"]);
            var isSortable_47 = Convert.ToBoolean(Request["bSortable_47"]);
            var isSortable_48 = Convert.ToBoolean(Request["bSortable_48"]);
            var isSortable_49 = Convert.ToBoolean(Request["bSortable_49"]);
            var isSortable_50 = Convert.ToBoolean(Request["bSortable_50"]);
            var isSortable_51 = Convert.ToBoolean(Request["bSortable_51"]);
            var isSortable_52 = Convert.ToBoolean(Request["bSortable_52"]);
            var isSortable_53 = Convert.ToBoolean(Request["bSortable_53"]);
            var isSortable_54 = Convert.ToBoolean(Request["bSortable_54"]);
            var isSortable_55 = Convert.ToBoolean(Request["bSortable_55"]);
            var isSortable_56 = Convert.ToBoolean(Request["bSortable_56"]);
            var isSortable_57 = Convert.ToBoolean(Request["bSortable_57"]);
            var isSortable_58 = Convert.ToBoolean(Request["bSortable_58"]);
            var isSortable_59 = Convert.ToBoolean(Request["bSortable_59"]);
            var isSortable_60 = Convert.ToBoolean(Request["bSortable_60"]);
            var isSortable_61 = Convert.ToBoolean(Request["bSortable_61"]);
            var isSortable_62 = Convert.ToBoolean(Request["bSortable_62"]);
            #endregion isSortable
            #region sortColumnIndex
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ViewEmployeeInfoAllVM, string> orderingFunction = (c =>
                sortColumnIndex == 0 && isSortable_0 ? c.Code :
                sortColumnIndex == 1 && isSortable_1 ? c.EmpName :
                sortColumnIndex == 2 && isSortable_2 ? c.Designation :
                sortColumnIndex == 3 && isSortable_3 ? c.JoinDate :
                sortColumnIndex == 4 && isSortable_4 ? c.Grade :
                sortColumnIndex == 5 && isSortable_5 ? c.BasicSalary.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.GrossSalary.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.Branch :
                sortColumnIndex == 8 && isSortable_8 ? c.Department :
                sortColumnIndex == 9 && isSortable_9 ? c.Section :
                sortColumnIndex == 10 && isSortable_10 ? c.Project :
                sortColumnIndex == 11 && isSortable_11 ? c.ProbationEnd :
                sortColumnIndex == 12 && isSortable_12 ? c.DateOfPermanent :
                sortColumnIndex == 13 && isSortable_13 ? c.IsPermanent.ToString() :
                sortColumnIndex == 14 && isSortable_14 ? c.Supervisor :
                sortColumnIndex == 15 && isSortable_15 ? c.BankInfo :
                sortColumnIndex == 16 && isSortable_16 ? c.BankAccountNo :
                sortColumnIndex == 17 && isSortable_17 ? c.EmploymentStatus :
                sortColumnIndex == 18 && isSortable_18 ? c.EmploymentType :
                sortColumnIndex == 19 && isSortable_19 ? c.TransferDate :
                sortColumnIndex == 20 && isSortable_20 ? c.PromotionDate :
                sortColumnIndex == 21 && isSortable_21 ? c.PersonalEmail :
                sortColumnIndex == 22 && isSortable_22 ? c.NickName :
                sortColumnIndex == 23 && isSortable_23 ? c.DateOfBirth :
                sortColumnIndex == 24 && isSortable_24 ? c.Religion :
                sortColumnIndex == 25 && isSortable_25 ? c.BloodGroup :
                sortColumnIndex == 26 && isSortable_26 ? c.Gender :
                sortColumnIndex == 27 && isSortable_27 ? c.MaritalStatus :
                sortColumnIndex == 28 && isSortable_28 ? c.Nationality :
                sortColumnIndex == 29 && isSortable_29 ? c.Smoker.ToString() :
                sortColumnIndex == 30 && isSortable_30 ? c.NID :
                sortColumnIndex == 31 && isSortable_31 ? c.PassportNumber :
                sortColumnIndex == 32 && isSortable_32 ? c.ExpiryDate :
                sortColumnIndex == 33 && isSortable_33 ? c.TIN :
                sortColumnIndex == 34 && isSortable_34 ? c.IsDisable.ToString() :
                sortColumnIndex == 35 && isSortable_35 ? c.KindsOfDisability :
                sortColumnIndex == 36 && isSortable_36 ? c.OtherId :
                sortColumnIndex == 37 && isSortable_37 ? c.PresentAddress :
                sortColumnIndex == 38 && isSortable_38 ? c.PresentMobile :
                sortColumnIndex == 39 && isSortable_39 ? c.PresentDistrict :
                sortColumnIndex == 40 && isSortable_40 ? c.PresentDivision :
                sortColumnIndex == 41 && isSortable_41 ? c.PresentCountry :
                sortColumnIndex == 42 && isSortable_42 ? c.PresentCity :
                sortColumnIndex == 43 && isSortable_43 ? c.PresentPostalCode :
                sortColumnIndex == 44 && isSortable_44 ? c.PresentPhone :
                sortColumnIndex == 45 && isSortable_45 ? c.PresentFax :
                sortColumnIndex == 46 && isSortable_46 ? c.PermanentAddress :
                sortColumnIndex == 47 && isSortable_47 ? c.PermanentMobile :
                sortColumnIndex == 48 && isSortable_48 ? c.PermanentDistrict :
                sortColumnIndex == 49 && isSortable_49 ? c.PermanentDivision :
                sortColumnIndex == 50 && isSortable_50 ? c.PermanentCountry :
                sortColumnIndex == 51 && isSortable_51 ? c.PermanentCity :
                sortColumnIndex == 52 && isSortable_52 ? c.PermanentPostalCode :
                sortColumnIndex == 53 && isSortable_53 ? c.EmConName :
                sortColumnIndex == 54 && isSortable_54 ? c.EmConRelation :
                sortColumnIndex == 55 && isSortable_55 ? c.EmConAddress :
                sortColumnIndex == 56 && isSortable_56 ? c.EmConCountry :
                sortColumnIndex == 57 && isSortable_57 ? c.EmConMobile :
                sortColumnIndex == 58 && isSortable_58 ? c.EducationDegree :
                sortColumnIndex == 59 && isSortable_59 ? c.EducationInstitute :
                sortColumnIndex == 60 && isSortable_60 ? c.EducationMajor :
                sortColumnIndex == 61 && isSortable_61 ? c.EducationYearOfPassing :
                sortColumnIndex == 62 && isSortable_62 ? c.EducationTotalYear.ToString() :
                "");
            #endregion sortColumnIndex
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            #region Display
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] 
                         { 
                             c.Code 			                                    // 0 
                             , c.EmpName                                            // 1 
                             , c.Designation	   	                                // 2 
                             , c.JoinDate          	                                // 3 
                             , c.Grade                                              // 4 
                             , c.BasicSalary            .ToString()                 // 5 
                             , c.GrossSalary            .ToString()         		// 6        
                             , c.Branch             			                    // 7 
                             , c.Department         			                    // 8 
                             , c.Section		      			                    // 9 
                             , c.Project 	          			                    // 10
                             , c.ProbationEnd       			                    // 11
                             , c.DateOfPermanent    			                    // 12
                             , c.IsPermanent        	?"Yes":"No"		            // 13
                             , c.Supervisor         			                    // 14
                             , c.BankInfo           			                    // 15
                             , c.BankAccountNo      			                    // 16
                             , c.EmploymentStatus   			                    // 17
                             , c.EmploymentType     			                    // 18
                             , c.TransferDate       			                    // 19
                             , c.PromotionDate      			                    // 20
                             , c.PersonalEmail      			                    // 21
                             , c.NickName	          			                    // 22
                             , c.DateOfBirth        			                    // 23
                             , c.Religion           			                    // 24
                             , c.BloodGroup         			                    // 25
                             , c.Gender			  			                        // 26
                             , c.MaritalStatus      			                    // 27
                             , c.Nationality        			                    // 28
                             , c.Smoker             	?"Yes":"No"			        // 29
                             , c.NID                			                    // 30
                             , c.PassportNumber     			                    // 31
                             , c.ExpiryDate         			                    // 32
                             , c.TIN                			                    // 33
                             , c.IsDisable          	?"Yes":"No"		            // 34
                             , c.KindsOfDisability  			                    // 35
                             , c.OtherId            			                    // 36
                             , c.PresentAddress     			                    // 37
                             , c.PresentMobile      			                    // 38
                             , c.PresentDistrict    			                    // 39
                             , c.PresentDivision    			                    // 40
                             , c.PresentCountry     			                    // 41
                             , c.PresentCity        			                    // 42
                             , c.PresentPostalCode  			                    // 43
                             , c.PresentPhone       			                    // 44
                             , c.PresentFax         			                    // 45
                             , c.PermanentAddress   			                    // 46
                             , c.PermanentMobile    			                    // 47
                             , c.PermanentDistrict  			                    // 48
                             , c.PermanentDivision  			                    // 49
                             , c.PermanentCountry   			                    // 50
                             , c.PermanentCity      			                    // 51
                             , c.PermanentPostalCode			                    // 52
                             , c.EmConName          			                    // 53
                             , c.EmConRelation      			                    // 54
                             , c.EmConAddress       			                    // 55
                             , c.EmConCountry       			                    // 56
                             , c.EmConMobile        			                    // 57
                             , c.EducationDegree            			            // 58        
                             , c.EducationInstitute         			            // 59        
                             , c.EducationMajor             			            // 60        
                             , c.EducationYearOfPassing     			            // 61        
                             , c.EducationTotalYear.ToString()     			        // 62        
                         };
            #endregion Display
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }
        #endregion EmployeeInformationAll

        #region EmployeeProximityInfo
        [Authorize]
        public ActionResult EmployeeProximityInfo(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string ReportNo)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
            string Employee = "N";
            string EmployeeId = "";
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                EmployeeId = identity.EmployeeId;
                Employee = "Y";
                CodeF = identity.EmployeeCode;
                CodeT = identity.EmployeeCode;
                //Name = "";
                ProjectId = "";
                DepartmentId = "";
                dtpFrom = "";
                dtpTo = "";
            }
            ViewBag.Employee = Employee;
            if (string.IsNullOrWhiteSpace(ReportNo))
            {
                return View();
            }
            #region Value assign to Parameters
            string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
           , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
            string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
           , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
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
            if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
            {
                vdtpFrom = dtpFrom;
                dtpFromParam = vdtpFrom;
            }
            if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
            {
                vdtpTo = dtpTo;
                dtpToParam = vdtpTo;
            }
            #endregion Value assign to Parameters

            CompanyRepo _CompanyRepo = new CompanyRepo();
            CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();


            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            var getAllData = _empRepo.EmployeeList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                , vdtpFrom, vdtpTo, null, null, null, null, null);
            string ReportHead = "";
            ReportHead = "There are no data to Preview for Employee Proximity ID";
            if (getAllData.Count > 0)
            {
                ReportHead = "Employee List with Proximity ID";
            }
            ReportDocument doc = new ReportDocument();
            string rptLocation = "";
            rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeProximityInfo.rpt";
            doc.Load(rptLocation);
            //doc.Database.Tables[0].SetDataSource(SymERPHtmlHelper.ConvertToDataTable<EmployeeInfoVM>(getAllData.ToList()));
            doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy"].SetDataSource(getAllData);
            string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
            doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
            doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
            doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
            doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
            doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
            doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
            doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
            doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
            doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
            doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
            doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
            doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
            var rpt = RenderReportAsPDF(doc);
            doc.Close();
            return rpt;
        }
        public ActionResult _EmpProximityInfoIndexPartial(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo)
        {
            EmployeeInfoVM vm = new EmployeeInfoVM();
            vm.CodeF = CodeF;
            vm.CodeT = CodeT;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.ProjectId = ProjectId;
            vm.DesignationId = DesignationId;
            vm.DOJFrom = dtpFrom;
            vm.DOJTo = dtpTo;
            return PartialView("_EmpProximityInfoIndex", vm);
        }
        public ActionResult _EmpProximityInfoIndex(JQueryDataTableParamVM param, string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo)
        {
            #region Comments
            /*
            Code 			   
            FullName        
            Designation	  
            Department      
            Section         
            Project         
            JoinDate        
            EmploymentType_E
            AttnUserId      
            Mobile          
            Address         
             */
            #endregion Comments
            #region Value assign to Parameters
            string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
           , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
            string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
           , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
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
            if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
            {
                vdtpFrom = dtpFrom;
                dtpFromParam = vdtpFrom;
            }
            if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
            {
                vdtpTo = dtpTo;
                dtpToParam = vdtpTo;
            }
            #endregion Value assign to Parameters
            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            List<EmployeeInfoVM> getAllData = new List<EmployeeInfoVM>();
            getAllData = _empRepo.EmployeeList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                , vdtpFrom, vdtpTo, null, null, null, null, null);
            IEnumerable<EmployeeInfoVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                #region Top Searchable Declare
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
                #endregion Top Searchable Declare
                #region Filtered Data
                filteredData = getAllData.Where(c =>
                       isSearchable0 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable1 && c.FullName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.JoinDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.EmploymentType_E.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable8 && c.AttnUserId.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable9 && c.Mobile.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable10 && c.Address.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
                #endregion Filtered Data
            }
            else
            {
                filteredData = getAllData;
            }
            #region isSortable
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
            #endregion isSortable
            #region sortColumnIndex
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 0 && isSortable_0 ? c.Code :
                sortColumnIndex == 1 && isSortable_1 ? c.FullName :
                sortColumnIndex == 2 && isSortable_2 ? c.Designation :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.Section :
                sortColumnIndex == 5 && isSortable_5 ? c.Project :
                sortColumnIndex == 6 && isSortable_6 ? c.JoinDate :
                sortColumnIndex == 7 && isSortable_7 ? c.EmploymentType_E :
                sortColumnIndex == 8 && isSortable_8 ? c.AttnUserId :
                sortColumnIndex == 9 && isSortable_9 ? c.Mobile :
                sortColumnIndex == 10 && isSortable_10 ? c.Address :
                "");
            #endregion sortColumnIndex
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            #region Display
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] 
                         { 
                               c.Code 			    
                             , c.FullName           
                             , c.Designation	    
                             , c.Department        
                             , c.Section           
                             , c.Project           
                             , c.JoinDate          
                             , c.EmploymentType_E  
                             , c.AttnUserId      
                             , c.Mobile          
		                     , c.Address         
                         };
            #endregion Display
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }
        #endregion EmployeeProximityInfo

        #region EmployeeImage

        //public ActionResult EmployeeImage(string CodeF, string CodeT, string DepartmentId, string SectionId
        //    , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string ReportNo)
        //{
        //    Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
        //    string Employee = "N";
        //    string EmployeeId = "";
        //    if (!(identity.IsAdmin || identity.IsHRM))
        //    {
        //        EmployeeId = identity.EmployeeId;
        //        Employee = "Y";
        //        CodeF = identity.EmployeeCode;
        //        CodeT = identity.EmployeeCode;
        //        //Name = "";
        //        ProjectId = "";
        //        DepartmentId = "";
        //        dtpFrom = "";
        //        dtpTo = "";
        //    }
        //    ViewBag.Employee = Employee;
        //    if (string.IsNullOrWhiteSpace(ReportNo))
        //    {
        //        return View();
        //    }
        //    #region Value assign to Parameters
        //    string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
        //   , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
        //    string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
        //   , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
        //    if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
        //    {
        //        vProjectId = ProjectId;
        //        ProjectRepo pRepo = new ProjectRepo();
        //        projectParam = pRepo.SelectById(ProjectId).Name;
        //    }
        //    if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
        //    {
        //        vDepartmentId = DepartmentId;
        //        DepartmentRepo dRepo = new DepartmentRepo();
        //        deptParam = dRepo.SelectById(DepartmentId).Name;
        //    }
        //    if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
        //    {
        //        vSectionId = SectionId;
        //        SectionRepo sRepo = new SectionRepo();
        //        secParam = sRepo.SelectById(SectionId).Name;
        //    }
        //    if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
        //    {
        //        vDesignationId = DesignationId;
        //        DesignationRepo desRepo = new DesignationRepo();
        //        desigParam = desRepo.SelectById(DesignationId).Name;
        //    }
        //    if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
        //    {
        //        vCodeF = CodeF;
        //        codeFParam = vCodeF;
        //    }
        //    if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
        //    {
        //        vCodeT = CodeT;
        //        codeTParam = vCodeT;
        //    }
        //    if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
        //    {
        //        vdtpFrom = dtpFrom;
        //        dtpFromParam = vdtpFrom;
        //    }
        //    if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
        //    {
        //        vdtpTo = dtpTo;
        //        dtpToParam = vdtpTo;
        //    }
        //    #endregion Value assign to Parameters
        //    EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
        //    var getAllData = _empRepo.EmployeeList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
        //        , vdtpFrom, vdtpTo, null, null, null, null, null);
        //    string ReportHead = "";
        //    ReportHead = "There are no data to Preview for Employee Proximity ID";
        //    if (getAllData.Count > 0)
        //    {
        //        ReportHead = "Employee List with Proximity ID";
        //    }
        //    ReportDocument doc = new ReportDocument();
        //    string rptLocation = "";
        //    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeProximityInfo.rpt";
        //    doc.Load(rptLocation);
        //    //doc.Database.Tables[0].SetDataSource(SymERPHtmlHelper.ConvertToDataTable<EmployeeInfoVM>(getAllData.ToList()));
        //    doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy"].SetDataSource(getAllData);
        //    string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
        //    doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
        //    doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
        //    doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
        //    doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
        //    doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
        //    doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
        //    doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
        //    doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
        //    doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
        //    doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
        //    var rpt = RenderReportAsPDF(doc);
        //    doc.Close();
        //    return rpt;
        //}


        //public ActionResult _EmployeeImageIndexPartial(string CodeF, string CodeT, string DepartmentId, string SectionId
        //    , string ProjectId, string DesignationId, string dtpFrom, string dtpTo)
        //{
        //    EmployeeInfoVM vm = new EmployeeInfoVM();
        //    vm.CodeF = CodeF;
        //    vm.CodeT = CodeT;
        //    vm.DepartmentId = DepartmentId;
        //    vm.SectionId = SectionId;
        //    vm.ProjectId = ProjectId;
        //    vm.DesignationId = DesignationId;
        //    vm.DOJFrom = dtpFrom;
        //    vm.DOJTo = dtpTo;
        //    return PartialView("_EmpProximityInfoIndex", vm);
        //}

        //public ActionResult _EmployeeImageIndex(JQueryDataTableParamVM param, string CodeF, string CodeT, string DepartmentId, string SectionId
        //    , string ProjectId, string DesignationId, string dtpFrom, string dtpTo)
        //{
        //    #region Comments
        //    /*
        //    Code 			   
        //    FullName        
        //    Designation	  
        //    Department      
        //    Section         
        //    Project         
        //    JoinDate        
        //    EmploymentType_E
        //    AttnUserId      
        //    Mobile          
        //    Address         
        //     */
        //    #endregion Comments
        //    #region Value assign to Parameters
        //    string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
        //   , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
        //    string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
        //   , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
        //    if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
        //    {
        //        vProjectId = ProjectId;
        //        ProjectRepo pRepo = new ProjectRepo();
        //        projectParam = pRepo.SelectById(ProjectId).Name;
        //    }
        //    if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
        //    {
        //        vDepartmentId = DepartmentId;
        //        DepartmentRepo dRepo = new DepartmentRepo();
        //        deptParam = dRepo.SelectById(DepartmentId).Name;
        //    }
        //    if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
        //    {
        //        vSectionId = SectionId;
        //        SectionRepo sRepo = new SectionRepo();
        //        secParam = sRepo.SelectById(SectionId).Name;
        //    }
        //    if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
        //    {
        //        vDesignationId = DesignationId;
        //        DesignationRepo desRepo = new DesignationRepo();
        //        desigParam = desRepo.SelectById(DesignationId).Name;
        //    }
        //    if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
        //    {
        //        vCodeF = CodeF;
        //        codeFParam = vCodeF;
        //    }
        //    if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
        //    {
        //        vCodeT = CodeT;
        //        codeTParam = vCodeT;
        //    }
        //    if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
        //    {
        //        vdtpFrom = dtpFrom;
        //        dtpFromParam = vdtpFrom;
        //    }
        //    if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
        //    {
        //        vdtpTo = dtpTo;
        //        dtpToParam = vdtpTo;
        //    }
        //    #endregion Value assign to Parameters
        //    EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
        //    List<EmployeeInfoVM> getAllData = new List<EmployeeInfoVM>();
        //    getAllData = _empRepo.EmployeeList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
        //        , vdtpFrom, vdtpTo, null, null, null, null, null);
        //    IEnumerable<EmployeeInfoVM> filteredData;
        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        //Optionally check whether the columns are searchable at all 
        //        #region Top Searchable Declare
        //        var isSearchable0 = Convert.ToBoolean(Request["bSearchable_0"]);
        //        var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
        //        var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
        //        var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
        //        var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
        //        var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
        //        var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
        //        var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
        //        var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
        //        var isSearchable9 = Convert.ToBoolean(Request["bSearchable_9"]);
        //        var isSearchable10 = Convert.ToBoolean(Request["bSearchable_10"]);
        //        #endregion Top Searchable Declare
        //        #region Filtered Data
        //        filteredData = getAllData.Where(c =>
        //               isSearchable0 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable1 && c.FullName.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable2 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable3 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable4 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable5 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable6 && c.JoinDate.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable7 && c.EmploymentType_E.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable8 && c.AttnUserId.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable9 && c.Mobile.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable10 && c.Address.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            );
        //        #endregion Filtered Data
        //    }
        //    else
        //    {
        //        filteredData = getAllData;
        //    }
        //    #region isSortable
        //    var isSortable_0 = Convert.ToBoolean(Request["bSortable_0"]);
        //    var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
        //    var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
        //    var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
        //    var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
        //    var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
        //    var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
        //    var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
        //    var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
        //    var isSortable_9 = Convert.ToBoolean(Request["bSortable_9"]);
        //    var isSortable_10 = Convert.ToBoolean(Request["bSortable_10"]);
        //    #endregion isSortable
        //    #region sortColumnIndex
        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
        //    Func<EmployeeInfoVM, string> orderingFunction = (c =>
        //        sortColumnIndex == 0 && isSortable_0 ? c.Code :
        //        sortColumnIndex == 1 && isSortable_1 ? c.FullName :
        //        sortColumnIndex == 2 && isSortable_2 ? c.Designation :
        //        sortColumnIndex == 3 && isSortable_3 ? c.Department :
        //        sortColumnIndex == 4 && isSortable_4 ? c.Section :
        //        sortColumnIndex == 5 && isSortable_5 ? c.Project :
        //        sortColumnIndex == 6 && isSortable_6 ? c.JoinDate :
        //        sortColumnIndex == 7 && isSortable_7 ? c.EmploymentType_E :
        //        sortColumnIndex == 8 && isSortable_8 ? c.AttnUserId :
        //        sortColumnIndex == 9 && isSortable_9 ? c.Mobile :
        //        sortColumnIndex == 10 && isSortable_10 ? c.Address :
        //        "");
        //    #endregion sortColumnIndex
        //    var sortDirection = Request["sSortDir_0"]; // asc or desc
        //    if (sortDirection == "asc")
        //        filteredData = filteredData.OrderBy(orderingFunction);
        //    else
        //        filteredData = filteredData.OrderByDescending(orderingFunction);
        //    #region Display
        //    var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var result = from c in displayedCompanies
        //                 select new[] 
        //                 { 
        //                       c.Code 			    
        //                     , c.FullName           
        //                     , c.Designation	    
        //                     , c.Department        
        //                     , c.Section           
        //                     , c.Project           
        //                     , c.JoinDate          
        //                     , c.EmploymentType_E  
        //                     , c.AttnUserId      
        //                     , c.Mobile          
        //                     , c.Address         
        //                 };
        //    #endregion Display
        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = getAllData.Count(),
        //        iTotalDisplayRecords = filteredData.Count(),
        //        aaData = result
        //    },
        //                JsonRequestBehavior.AllowGet);
        //}

        [Authorize]
        [HttpGet]
        public ActionResult EmployeeImage(string RT = null)
        {
            string Employee = "N";
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Employee = "Y";
            }
            ViewBag.Employee = Employee;
            ViewBag.RT = RT;
            return View();
        }
        [HttpPost]
        public ActionResult EmployeeImage(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string Gender_E = null, string Religion = null
            , string GradeId = null, string RT = null
            , string other1 = "", string other2 = "", string other3 = "", string OrderByCode = "")
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    ProjectId = "";
                    DepartmentId = "";
                    DesignationId = "";
                    dtpFrom = "";
                    dtpTo = "";
                }
                ViewBag.Employee = Employee;
                ViewBag.RT = RT;
                #region Value assign to Parameters
                string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
               , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
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
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }
                #endregion Value assign to Parameters
                ReportDocument doc = new ReportDocument();
                string ReportHead = "";
                string rptLocation = "";
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                var getAllData = _empRepo.EmployeeList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, null, EmployeeId, Gender_E, Religion, GradeId, other1, other2, other3, OrderByCode);

                SettingRepo _sRepo = new SettingRepo();
                //_sRepo.settingValue("SalarySheet", "SalarySheet(1)");


                if (RT == "EmpList")
                {
                    ReportHead = "There are no data to Preview for Employee List";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Employee List";
                    }
                    //rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeInfo.rpt";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\" + _sRepo.settingValue("Report", "rptEmployeeInfo") + ".rpt";


                }
                else if (RT == "EmpIDCard")
                {
                    ReportHead = "There are no data to Preview for Employee Id Card";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Employee Id Card";
                    }
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeIDCard.rpt";
                }


                if (OrderByCode == "true")
                {
                    ReportHead = "There are no data to Preview for Employee List";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Employee List";
                    }
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeInfoByCode.rpt";
                }
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeImageInfo.rpt";

                doc.Load(rptLocation);
             
                doc.Load(rptLocation);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy"].SetDataSource(getAllData);
                //doc.Database.Tables[0].SetDataSource(SymERPHtmlHelper.ConvertToDataTable<EmployeeInfoVM>(getAllData.ToList()));
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                string ImagePath = AppDomain.CurrentDomain.BaseDirectory + "Files\\EmployeeInfo";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
                doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
                doc.DataDefinition.FormulaFields["ImagePath"].Text = "'" + ImagePath + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Legacy Reports


        [Authorize]
        public ActionResult EA(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string dtAtnFrom
            , string dtAtnTo, string AttnStatus, string Punch, string ReportNo, string AttendanceStatus, string view)
        {
            //string AttendanceStatus = "";
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    //Name = "";
                    ProjectId = "";
                    DepartmentId = "";
                    dtpFrom = "";
                    dtpTo = "";
                }
                ViewBag.Employee = Employee;
                ViewBag.ReportNo = ReportNo;
                if (!string.IsNullOrWhiteSpace(view))
                {
                    return View();
                }
                string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
                , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0"
                , vdtAtnFrom = "0_0"
                , vdtAtnTo = "0_0"
                , vPunchMissIn = "0000", vPunchMissOut = "0000";
                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
                , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]"
                , dtAtnFromParam = "[All]", dtAtnToParam = "[All]", PunchMissInParam = "[All]", PunchMissOutParam = "[All]";
                #region Parameters
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
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }
                if (dtAtnFrom != "0" && dtAtnFrom != "" && dtAtnFrom != "null" && dtAtnFrom != null)
                {
                    vdtAtnFrom = dtAtnFrom;
                    dtAtnFromParam = vdtAtnFrom;
                }
                if (dtAtnTo != "0" && dtAtnTo != "" && dtAtnTo != "null" && dtAtnTo != null)
                {
                    vdtAtnTo = dtAtnTo;
                    dtAtnToParam = vdtAtnTo;
                }
                if (Punch == "PunchMissOut")
                {
                    vPunchMissOut = Punch;
                    PunchMissOutParam = vPunchMissOut;
                }
                if (Punch == "PunchMissIn")
                {
                    vPunchMissIn = Punch;
                    PunchMissInParam = vPunchMissIn;
                }
                #endregion Parameters

                ReportDocument doc = new ReportDocument();//default
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                List<AttLogsVM> attLogsVMs = new List<AttLogsVM>();
                attLogsVMs = _empRepo.EA(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                   , vdtpFrom, vdtpTo, vdtAtnFrom, vdtAtnTo, vPunchMissIn, vPunchMissOut, ReportNo, EmployeeId);
                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(attLogsVMs.ToList());

                DataView dv = new DataView(table);
                //dv.RowFilter = string.Format("TermName like '%{0}%' or TermDescription like '%{0}%'", AttendanceStatus.Text.Trim());
                dv.RowFilter = string.Format("AttnStatus = '{0}'", AttendanceStatus.Trim());
                //dgvTerms.DataSource = dv;

                //if (AttnStatus.ToLower() == "present")
                //{
                //    table = table.Select("[AttnStatus] ='Present'").CopyToDataTable();
                //}
                //else if (AttnStatus.ToLower() == "absent")
                //{
                //    table = table.Select("[AttnStatus] ='Absent'").CopyToDataTable();
                //}
                //else if (AttnStatus.ToLower() == "in miss")
                //{
                //    table = table.Select("[AttnStatus] ='Present (In Miss)'").CopyToDataTable();
                //}
                //else if (AttnStatus.ToLower() == "out miss")
                //{
                //    table = table.Select("[AttnStatus] ='Present (Out Miss)'").CopyToDataTable();
                //}
                //else if (AttnStatus.ToLower() == "all missing")
                //{
                //    table = table.Select("[AttnStatus] ='Present (In Miss)' OR [AttnStatus] ='Present (Out Miss)'").CopyToDataTable();
                //}
                //else if (AttnStatus.ToLower() == "late")
                //{
                //    table = table.Select("[AttnStatus] ='Present (Late)'").CopyToDataTable();
                //}

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                string ReportHead = "";
                ReportHead = "There are no data to Preview for Employee Attendance";
                if (attLogsVMs.Count > 0)
                {
                    ReportHead = "Employee Attendance";
                }
                string rptLocation = "";
                if (ReportNo == "1")
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\RptEA.rpt";
                }
                else if (ReportNo == "2")
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\RptEA2.rpt";
                }
                doc.Load(rptLocation);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_AttLogsVM_Proxy"].SetDataSource(dv);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["UpToDate"].Text = "'" + dtAtnTo + "'";
                doc.DataDefinition.FormulaFields["DateBetween"].Text = "'" + dtAtnFrom + " -> " + dtAtnTo + "'";
                doc.DataDefinition.FormulaFields["ReportNo"].Text = "'" + ReportNo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
                doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
                doc.DataDefinition.FormulaFields["dtAtnFromParam"].Text = "'" + dtAtnFromParam + "'";
                doc.DataDefinition.FormulaFields["dtAtnToParam"].Text = "'" + dtAtnToParam + "'";
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

        [Authorize]
        public ActionResult LBEW(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string ReportNo
            , string leaveyear, string LeaveType, string Gender_E, string Religion, string GradeId, string EmpCategory, string EmpJobType, string RT = "")
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                string Employee = "N";
                string EmployeeId = "";
                string GrossSalary = "";
                string Supervisor = "";
                DateTime PunchData =DateTime.Now;

                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    //Name = "";
                    ProjectId = "";
                    DepartmentId = "";
                    dtpFrom = "";
                    dtpTo = "";
                }
                ViewBag.Employee = Employee;
                ViewBag.RT = RT;
                if (string.IsNullOrWhiteSpace(ReportNo))
                {
                    return View();
                }
                #region Value assign to Parameters
             
                string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0", vCategoryId = "0_0"
               , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]", CategoryParam = "[All]";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null && ProjectId != "undefined")
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null && DepartmentId != "undefined")
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null && SectionId != "undefined")
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null && DesignationId != "undefined")
                {
                    vDesignationId = DesignationId;
                    DesignationRepo desRepo = new DesignationRepo();
                    desigParam = desRepo.SelectById(DesignationId).Name;
                }
                //if (EmpCategory != "0_0" && EmpCategory != "0" && EmpCategory != "" && EmpCategory != "null" && EmpCategory != null && EmpCategory != "undefined")
                //{
                //    vCategoryId = EmpCategory;
                //   Catego desRepo = new DesignationRepo();
                //    CategoryParam = desRepo.SelectById(DesignationId).Name;
                //}
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null && CodeF != "undefined")
                {
                    vCodeF = CodeF;
                    codeFParam = vCodeF;
                }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null && CodeT != "undefined")
                {
                    vCodeT = CodeT;
                    codeTParam = vCodeT;
                }
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null && dtpFrom != "undefined")
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null && dtpTo != "undefined")
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }
                #endregion Value assign to Parameters
                ReportDocument doc = new ReportDocument();
                string ReportHead = "";
                string rptLocation = "";
                #region ReportType
                if (RT == "LBEW")
                {
                    EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                    var getAllData = _empRepo.EmployeeLeaveList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                        , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId, Gender_E, Religion, GradeId);
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveList.rpt";
                    ReportHead = "There are no data to Preview for Leave Balance List (Employee Wise)";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Leave Balance List (Employee Wise)";
                    }
                    doc.Load(rptLocation);
                    doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeaveBalanceVM_Proxy"].SetDataSource(getAllData);
                }
                else if (RT == "LBTW")
                {
                    EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                    var getAllData = _empRepo.EmployeeLeaveList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                        , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId, Gender_E, Religion, GradeId);
                    ReportHead = "There are no data to Preview for Leave Balance List (Type Wise)";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Leave Balance List (Type Wise)";
                    }
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveList2.rpt";
                    doc.Load(rptLocation);
                    doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeaveBalanceVM_Proxy"].SetDataSource(getAllData);
                }
                else if (RT == "LSEW")
                {
                    EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                    var getAllData = _empRepo.EmployeeLeaveStatement(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                        , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId);
                    ReportHead = "There are no data to Preview for Leave Statement";
                    if (getAllData.Count > 0)
                    {
                        ReportHead = "Leave Statement";
                    }
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\RptEmployeeLeaveStatement.rpt";
                    doc.Load(rptLocation);
                    doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeaveStatementVM_Proxy"].SetDataSource(getAllData);
                }
                else if (RT == "LR")
                {
                    string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                    EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                    if (CompanyName.ToLower() != "g4s")
                    {

                  //      var getAllData = _empRepo.EmployeeLeaveRegisterShampan(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                  //, vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId);
                  //      ReportHead = "There are no data to Preview for Leave Register (Employee Wise)";
                  //      getAllData.Tables[0].TableName = "LeaveRegisterG4S";
                  //      getAllData.Tables[1].TableName = "LeaveSummaryG4S";
                  //      getAllData.Tables[2].TableName = "AnualLeaveSummaryG4S";

                  //      if (getAllData.Tables[0].Rows.Count > 0)
                  //      {
                  //          ReportHead = "Leave Register (Employee Wise)";
                  //      }
                  //      //rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveRegister.rpt";
                  //      rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveRegisterG4S.rpt";
                  //      doc.Load(rptLocation);
                  //      //doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeaveVM_Proxy"].SetDataSource(getAllData);
                  //      doc.SetDataSource(getAllData);




                        var getAllData = _empRepo.EmployeeLeaveRegister(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                     , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId);
                        ReportHead = "There are no data to Preview for Leave Register (Employee Wise)";
                        if (getAllData.Count > 0)
                        {
                            ReportHead = "Leave Register (Employee Wise)";
                        }
                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveRegister.rpt";
                       // rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveRegisterG4S.rpt";
                        doc.Load(rptLocation);
                        doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeaveVM_Proxy"].SetDataSource(getAllData);
                    }
                    else
                    {
                        var getAllData = _empRepo.EmployeeLeaveRegisterG4S(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                     , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId);
                        ReportHead = "There are no data to Preview for Leave Register (Employee Wise)";
                        getAllData.Tables[0].TableName = "LeaveRegisterG4S";
                        getAllData.Tables[1].TableName = "LeaveSummaryG4S";
                        getAllData.Tables[2].TableName = "AnualLeaveSummaryG4S";
                        
                        if (getAllData.Tables[0].Rows.Count > 0)
                        {
                            ReportHead = "Leave Register (Employee Wise)";
                        }
                        //rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveRegister.rpt";
                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveRegisterG4S.rpt";
                        doc.Load(rptLocation);
                        //doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeaveVM_Proxy"].SetDataSource(getAllData);
                        doc.SetDataSource(getAllData);

                    }
                  
                }
                else if (RT == "EnCash")
                {
                    EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();

                    var getAllData = _empRepo.EmployeeLeaveEncashmentG4S(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId);
                    ReportHead = "There are no data to Preview for Leave Register (Employee Wise)";
                    getAllData.Tables[0].TableName = "LeaveRegisterG4S";
                    getAllData.Tables[1].TableName = "LeaveSummaryG4S";
                    getAllData.Tables[2].TableName = "AnualLeaveSummaryG4S";

                    EmpCategory = getAllData.Tables[0].Rows[0]["EmpCategory"].ToString();
                    GrossSalary = getAllData.Tables[0].Rows[0]["GrossSalary"].ToString();
                    Supervisor = getAllData.Tables[0].Rows[0]["Supervisor"].ToString();

                    PunchData = DateTime.Parse(getAllData.Tables[1].Rows[0]["PunchData"].ToString());

                    if (getAllData.Tables[0].Rows.Count > 0)
                    {
                        ReportHead = "Leave Encashment (Employee Wise)";
                    }
                    //rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveRegister.rpt";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveEncashmentG4S.rpt";
                    doc.Load(rptLocation);
                    //doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeaveVM_Proxy"].SetDataSource(getAllData);
                    doc.SetDataSource(getAllData);
                }
                else if (RT == "SLSEW")
                {
                    EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                    var getAllData = _empRepo.EmployeeSummaryLeaveRegisterReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                       , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId);
                    ReportHead = "There are no data to Preview for Leave Register (Employee Wise)";
                    if (getAllData.Rows.Count > 0)
                    {
                        ReportHead = "Employee Summary Leave Register (Employee Wise)";
                    }
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeSummaryLeaveRegister.rpt";

                    getAllData.TableName = "dtSummaryEmployeeLeave";

                    doc.Load(rptLocation);
                    doc.SetDataSource(getAllData);
                }
                #endregion ReportType
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = doc.DataDefinition.FormulaFields;
                CommonWebMethod _CommonWebMethod = new CommonWebMethod();
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                if (RT == "LBEW" || RT == "LBTW" || RT == "LSEW")
                {
                    doc.DataDefinition.FormulaFields["UpToDate"].Text = "'" + DateTime.Now.ToString("dd/MMM/yyyy") + "'";
                }
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                if (RT != "SLSEW")
                {
                    doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                    doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                    doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                    doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                    doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                    doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                    doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
                    doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
                    doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                    doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";

                    if (RT == "EnCash")
                    {
                        doc.DataDefinition.FormulaFields["EmpCategory"].Text = "'" + EmpCategory + "'";
                        doc.DataDefinition.FormulaFields["GrossSalary"].Text = "'" + GrossSalary + "'";
                        doc.DataDefinition.FormulaFields["Supervisor"].Text = "'" + Supervisor + "'";
                        doc.DataDefinition.FormulaFields["PunchData"].Text = "'" + PunchData + "'";
                    }
                }
                _CommonWebMethod.FormulaField(doc, crFormulaF, "leaveyear", leaveyear.ToString());
               

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Commented Actions LBTW, LSEW
        //[Authorize]
        //public ActionResult LBTW(string CodeF, string CodeT, string DepartmentId, string SectionId
        //    , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string ReportNo
        //    , string leaveyear, string LeaveType, string Gender_E, string Religion, string GradeId)
        //{
        //    try
        //    {
        //        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //        string Employee = "N";
        //        string EmployeeId = "";
        //        if (!(identity.IsAdmin || identity.IsHRM))
        //        {
        //            EmployeeId = identity.EmployeeId;
        //            Employee = "Y";
        //            CodeF = identity.EmployeeCode;
        //            CodeT = identity.EmployeeCode;
        //            //Name = "";
        //            ProjectId = "";
        //            DepartmentId = "";
        //            dtpFrom = "";
        //            dtpTo = "";
        //        }
        //        ViewBag.Employee = Employee;
        //        if (string.IsNullOrWhiteSpace(ReportNo))
        //        {
        //            return View();
        //        }
        //        #region Value assign to Parameters
        //        string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
        //       , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
        //        string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
        //       , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
        //        if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
        //        {
        //            vProjectId = ProjectId;
        //            ProjectRepo pRepo = new ProjectRepo();
        //            projectParam = pRepo.SelectById(ProjectId).Name;
        //        }
        //        if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
        //        {
        //            vDepartmentId = DepartmentId;
        //            DepartmentRepo dRepo = new DepartmentRepo();
        //            deptParam = dRepo.SelectById(DepartmentId).Name;
        //        }
        //        if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
        //        {
        //            vSectionId = SectionId;
        //            SectionRepo sRepo = new SectionRepo();
        //            secParam = sRepo.SelectById(SectionId).Name;
        //        }
        //        if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
        //        {
        //            vDesignationId = DesignationId;
        //            DesignationRepo desRepo = new DesignationRepo();
        //            desigParam = desRepo.SelectById(DesignationId).Name;
        //        }
        //        if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
        //        {
        //            vCodeF = CodeF;
        //            codeFParam = vCodeF;
        //        }
        //        if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
        //        {
        //            vCodeT = CodeT;
        //            codeTParam = vCodeT;
        //        }
        //        if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
        //        {
        //            vdtpFrom = dtpFrom;
        //            dtpFromParam = vdtpFrom;
        //        }
        //        if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
        //        {
        //            vdtpTo = dtpTo;
        //            dtpToParam = vdtpTo;
        //        }
        //        #endregion Value assign to Parameters
        //        ReportDocument doc = new ReportDocument();
        //        string rptLocation = "";
        //        string ReportHead = "";
        //        //Report Set
        //        EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
        //        var getAllData = _empRepo.EmployeeLeaveList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
        //            , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId, Gender_E, Religion, GradeId);
        //        ReportHead = "There are no data to Preview for Leave Balance List (Type Wise)";
        //        if (getAllData.Count > 0)
        //        {
        //            ReportHead = "Leave Balance List (Type Wise)";
        //        }
        //        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveList2.rpt";
        //        doc.Load(rptLocation);
        //        doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeaveBalanceVM_Proxy"].SetDataSource(getAllData);
        //        string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
        //        doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
        //        doc.DataDefinition.FormulaFields["UpToDate"].Text = "'" + DateTime.Now.ToString("dd/MMM/yyyy") + "'";
        //        doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
        //        doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
        //        doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
        //        doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
        //        doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
        //        doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
        //        doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
        //        doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
        //        doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
        //        var rpt = RenderReportAsPDF(doc);
        //        doc.Close();
        //        return rpt;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //[Authorize]
        //public ActionResult LSEW(string CodeF, string CodeT, string DepartmentId, string SectionId
        //    , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string ReportNo, string leaveyear, string LeaveType)
        //{
        //    try
        //    {
        //        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //        string Employee = "N";
        //        string EmployeeId = "";
        //        if (!(identity.IsAdmin || identity.IsHRM))
        //        {
        //            EmployeeId = identity.EmployeeId;
        //            Employee = "Y";
        //            CodeF = identity.EmployeeCode;
        //            CodeT = identity.EmployeeCode;
        //            //Name = "";
        //            ProjectId = "";
        //            DepartmentId = "";
        //            dtpFrom = "";
        //            dtpTo = "";
        //        }
        //        ViewBag.Employee = Employee;
        //        if (string.IsNullOrWhiteSpace(ReportNo))
        //        {
        //            return View();
        //        }
        //        #region Value assign to Parameters
        //        string vProjectId = "0_0";
        //        string vDepartmentId = "0_0";
        //        string vSectionId = "0_0";
        //        string vDesignationId = "0_0";
        //        string vCodeF = "0_0";
        //        string vCodeT = "0_0";
        //        string vdtpFrom = "0_0";
        //        string vdtpTo = "0_0";
        //        string projectParam = "[All]";
        //        string deptParam = "[All]";
        //        string secParam = "[All]";
        //        string desigParam = "[All]";
        //        string codeFParam = "[All]";
        //        string codeTParam = "[All]";
        //        string dtpFromParam = "[All]";
        //        string dtpToParam = "[All]";
        //        if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
        //        {
        //            vProjectId = ProjectId;
        //            ProjectRepo pRepo = new ProjectRepo();
        //            projectParam = pRepo.SelectById(ProjectId).Name;
        //        }
        //        if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
        //        {
        //            vDepartmentId = DepartmentId;
        //            DepartmentRepo dRepo = new DepartmentRepo();
        //            deptParam = dRepo.SelectById(DepartmentId).Name;
        //        }
        //        if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
        //        {
        //            vSectionId = SectionId;
        //            SectionRepo sRepo = new SectionRepo();
        //            secParam = sRepo.SelectById(SectionId).Name;
        //        }
        //        if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
        //        {
        //            vDesignationId = DesignationId;
        //            DesignationRepo desRepo = new DesignationRepo();
        //            desigParam = desRepo.SelectById(DesignationId).Name;
        //        }
        //        if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
        //        {
        //            vCodeF = CodeF;
        //            codeFParam = vCodeF;
        //        }
        //        if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
        //        {
        //            vCodeT = CodeT;
        //            codeTParam = vCodeT;
        //        }
        //        if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
        //        {
        //            vdtpFrom = dtpFrom;
        //            dtpFromParam = vdtpFrom;
        //        }
        //        if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
        //        {
        //            vdtpTo = dtpTo;
        //            dtpToParam = vdtpTo;
        //        }
        //        #endregion Value assign to Parameters
        //        ReportDocument doc = new ReportDocument();
        //        string ReportHead = "";
        //        string rptLocation = "";
        //        EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
        //        var getAllData = _empRepo.EmployeeLeaveStatement(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
        //            , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId);
        //        ReportHead = "There are no data to Preview for Leave Statement";
        //        if (getAllData.Count > 0)
        //        {
        //            ReportHead = "Leave Statement";
        //        }
        //        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\RptEmployeeLeaveStatement.rpt";
        //        doc.Load(rptLocation);
        //        doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeaveStatementVM_Proxy"].SetDataSource(getAllData);
        //        string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
        //        doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
        //        doc.DataDefinition.FormulaFields["UpToDate"].Text = "'" + DateTime.Now.ToString("dd/MMM/yyyy") + "'";
        //        doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
        //        doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
        //        doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
        //        doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
        //        doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
        //        doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
        //        doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
        //        doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
        //        doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
        //        var rpt = RenderReportAsPDF(doc);
        //        doc.Close();
        //        return rpt;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        #endregion Commented Actions LBTW, LSEW

        [Authorize]
        public ActionResult LAF(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string ReportNo, string leaveyear, string LeaveType
            , string Statement = "", string LId = "0", string IsSupervisor ="N")
        {
            try
            {

                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                #region Variables

                string Employee = "N";
                string EmployeeId = "";
                if (IsSupervisor == "N")
                {
                    if (!(identity.IsAdmin || identity.IsHRM))
                    {
                        EmployeeId = identity.EmployeeId;
                        Employee = "Y";
                        CodeF = identity.EmployeeCode;
                        CodeT = identity.EmployeeCode;
                        //Name = "";
                        ProjectId = "";
                        DepartmentId = "";
                        dtpFrom = "";
                        dtpTo = "";
                    }
                }
                ViewBag.Employee = Employee;

                #endregion

                if (string.IsNullOrWhiteSpace(ReportNo))
                {
                    return View();
                }

                #region Parameters

                string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
               , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null && ProjectId != "undefined")
                { vProjectId = ProjectId; }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null && ProjectId != "undefined")
                { vDepartmentId = DepartmentId; }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null && SectionId != "undefined")
                { vSectionId = SectionId; }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null && DesignationId != "undefined")
                { vDesignationId = DesignationId; }
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null && CodeF != "undefined")
                { vCodeF = CodeF; }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null && CodeT != "undefined")
                { vCodeT = CodeT; }
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null && dtpFrom != "undefined")
                { vdtpFrom = dtpFrom; }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null && dtpTo != "undefined")
                { vdtpTo = dtpTo; }

                #endregion

                #region Data Call

                ReportDocument doc = new ReportDocument();
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                List<EmployeeLeaveBalanceVM> getAllData = new List<EmployeeLeaveBalanceVM>();
                if (string.IsNullOrWhiteSpace(Statement))
                {
                    getAllData = _empRepo.EmployeeLeaveList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                        , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId, null, null, null);
                }
                else
                {
                    getAllData = _empRepo.EmployeeLeaveStatementNew(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                        , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId, LId);
                }

                #endregion

                #region Report Call

                string ReportHead = "";
                string Suppervisor ="";
                ReportHead = "There are no data to Preview for Leave Application From";
                if (getAllData.Count > 0)
                {
                    foreach (var item in getAllData)
                    {
                        Suppervisor = item.Supervisor;
                    }

                    ReportHead = "Leave Application Form";
                }
                string rptLocation = "";
                if (string.IsNullOrWhiteSpace(Statement))
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveApplicationForm.rpt";
                }
                else
                {
                    if (CompanyName.ToUpper() == "BOLLORE")
                    {
                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveStatementBollore.rpt";
                    }
                   else if (CompanyName.ToUpper() == "G4S")
                    {
                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveStatementG4S.rpt";
                    }
                    else
                    {
                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveStatementNew.rpt";
                    }
                    
                }

                if (CompanyName.ToLower() == "kbl" || CompanyName.ToLower() == "anupam" || CompanyName.ToLower() == "kajol")
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveApplicationForm_Kajol.rpt";

                }
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM Companyvm = _CompanyRepo.SelectAll().FirstOrDefault();

                string Logo = new AppSettingsReader().GetValue("Logo", typeof(string)).ToString();

                doc.Load(rptLocation);

                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeaveBalanceVM_Proxy"].SetDataSource(getAllData);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\" + Logo;
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["UpToDate"].Text = "'" + DateTime.Now.ToString("dd/MMM/yyyy") + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Companyvm.Name.ToString() + "'";
                doc.DataDefinition.FormulaFields["CompanyAddress"].Text = "'" + Companyvm.Address.ToString() + "'";
                doc.DataDefinition.FormulaFields["Supervisor"].Text = "'" + Suppervisor + "'";
                

                ////CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = doc.DataDefinition.FormulaFields;
                FormulaField(doc, crFormulaF, "LeaveSignatureLocation", AppDomain.CurrentDomain.BaseDirectory + "File\\SignatureFile\\DefaultSignature.jpg");
                
                ////doc.DataDefinition.FormulaFields["LeaveSignatureLocation"].Text = "'" + AppDomain.CurrentDomain.BaseDirectory + "File\\SignatureFile\\DefaultSignature.jpg" +"'";


                #endregion

                var rpt = RenderReportAsPDF(doc);

                doc.Close();

                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void FormulaField(ReportDocument objrpt, FormulaFieldDefinitions crFormulaF, string fieldName, string fieldValue)
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

        [Authorize]
        [HttpGet]
        public ActionResult ExEmployeeList()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
            string Employee = "N";
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Employee = "Y";
            }
            ViewBag.Employee = Employee;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ExEmployeeList(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo)
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    //Name = "";
                    ProjectId = "";
                    DepartmentId = "";
                    dtpFrom = "";
                    dtpTo = "";
                }
                ViewBag.Employee = Employee;
                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                string vdtpFrom = "0_0";
                string vdtpTo = "0_0";
                string projectParam = "[All]";
                string deptParam = "[All]";
                string secParam = "[All]";
                string desigParam = "[All]";
                string codeFParam = "[All]";
                string codeTParam = "[All]";
                string dtpFromParam = "[All]";
                string dtpToParam = "[All]";
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
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                ReportDocument doc = new ReportDocument();
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                var getAllData = _empRepo.ExEmployeeList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Ex-Employee List";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Ex-Employee List";
                }
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptExEmployeeInfo.rpt";
                doc.Load(rptLocation);
                //doc.Database.Tables[0].SetDataSource(SymERPHtmlHelper.ConvertToDataTable<EmployeeInfoVM>(getAllData.ToList()));
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy"].SetDataSource(getAllData);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
                doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
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

        [Authorize]
        [HttpGet]
        public ActionResult UnConfirmedList()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
            string Employee = "N";
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Employee = "Y";
            }
            ViewBag.Employee = Employee;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult UnConfirmedList(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo)
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    //Name = "";
                    ProjectId = "";
                    DepartmentId = "";
                    dtpFrom = "";
                    dtpTo = "";
                }
                ViewBag.Employee = Employee;
                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                string vdtpFrom = "0_0";
                string vdtpTo = "0_0";
                string projectParam = "[All]";
                string deptParam = "[All]";
                string secParam = "[All]";
                string desigParam = "[All]";
                string codeFParam = "[All]";
                string codeTParam = "[All]";
                string dtpFromParam = "[All]";
                string dtpToParam = "[All]";
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
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }
                ReportDocument doc = new ReportDocument();
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                var getAllData = _empRepo.UnConfirmedList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Ex-Employee List";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Un Confirmed Employee List";
                }
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptUnConfirmEmployeeInfo.rpt";
                doc.Load(rptLocation);
                //doc.Database.Tables[0].SetDataSource(SymERPHtmlHelper.ConvertToDataTable<EmployeeInfoVM>(getAllData.ToList()));
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy"].SetDataSource(getAllData);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
                doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
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




        [Authorize]
        [HttpGet]
        public ActionResult EmpServiceLength()
        {
            string Employee = "N";
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Employee = "Y";
            }
            ViewBag.Employee = Employee;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult EmpServiceLength(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string Gender, string Religion
            , string GradeId)
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    //Name = "";
                    ProjectId = "";
                    DepartmentId = "";
                    dtpFrom = "";
                    dtpTo = "";
                }
                ViewBag.Employee = Employee;
                #region Value assign to Parameters
                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                string vdtpFrom = "0_0";
                string vdtpTo = "0_0";
                string projectParam = "[All]";
                string deptParam = "[All]";
                string secParam = "[All]";
                string desigParam = "[All]";
                string codeFParam = "[All]";
                string codeTParam = "[All]";
                string dtpFromParam = "[All]";
                string dtpToParam = "[All]";
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
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }
                #endregion Value assign to Parameters

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                ReportDocument doc = new ReportDocument();
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                var getAllData = _empRepo.EmpServiceLength(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId, Gender, Religion, GradeId);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Employee Service Length List";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Employee Service Length List";
                }
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmpServiceLength.rpt";
                doc.Load(rptLocation);
                //doc.Database.Tables[0].SetDataSource(SymERPHtmlHelper.ConvertToDataTable<EmployeeInfoVM>(getAllData.ToList()));
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy"].SetDataSource(getAllData);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
                doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
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

        [Authorize]
        [HttpGet]
        public ActionResult EmpTransfer()
        {
            string Employee = "N";
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Employee = "Y";
            }
            ViewBag.Employee = Employee;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult EmpTransfer(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtjFrom, string dtjTo, string dttFrom, string dttTo)
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    //Name = "";
                    ProjectId = "";
                    dtjFrom = "";
                    dtjTo = "";
                    dttFrom = "";
                    dttTo = "";
                }
                ViewBag.Employee = Employee;
                #region Value assign to Parameters
                string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
                , vCodeF = "0_0", vCodeT = "0_0", vdtjFrom = "0_0", vdtjTo = "0_0", vdttFrom = "0_0", vdttTo = "0_0";
                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtjFromParam = "[All]", dtjToParam = "[All]"
               , dttFromParam = "[All]", dttToParam = "[All]";
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
                if (dtjFrom != "0_0" && dtjFrom != "0" && dtjFrom != "" && dtjFrom != "null" && dtjFrom != null)
                {
                    vdtjFrom = dtjFrom;
                    dtjFromParam = vdtjFrom;
                }
                if (dtjTo != "0_0" && dtjTo != "0" && dtjTo != "" && dtjTo != "null" && dtjTo != null)
                {
                    vdtjTo = dtjTo;
                    dtjToParam = vdtjTo;
                }
                if (dttFrom != "0_0" && dttFrom != "0" && dttFrom != "" && dttFrom != "null" && dttFrom != null)
                {
                    vdttFrom = dttFrom;
                    dttFromParam = vdttFrom;
                }
                if (dttTo != "0_0" && dttTo != "0" && dttTo != "" && dttTo != "null" && dttTo != null)
                {
                    vdttTo = dttTo;
                    dttToParam = vdttTo;
                }
                #endregion Value assign to Parameters

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                ReportDocument doc = new ReportDocument();
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                var getAllData = _empRepo.EmpTransfer(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtjFrom, vdtjTo, EmployeeId, vdttFrom, vdttTo);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Employee Transfer";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Employee Transfer";
                }
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeTransfer.rpt";
                doc.Load(rptLocation);
                //doc.Database.Tables[0].SetDataSource(SymERPHtmlHelper.ConvertToDataTable<EmployeeInfoVM>(getAllData.ToList()));
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy"].SetDataSource(getAllData);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["dtjFromParam"].Text = "'" + dtjFromParam + "'";
                doc.DataDefinition.FormulaFields["dtjToParam"].Text = "'" + dtjToParam + "'";
                doc.DataDefinition.FormulaFields["dttFromParam"].Text = "'" + dttFromParam + "'";
                doc.DataDefinition.FormulaFields["dttToParam"].Text = "'" + dttToParam + "'";
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

        [Authorize]
        [HttpGet]
        public ActionResult EmpTraining()
        {
            string Employee = "N";
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Employee = "Y";
            }
            ViewBag.Employee = Employee;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult EmpTraining(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string topics)
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    ProjectId = "";
                    //Dept = ""; 
                    dtpFrom = "";
                    dtpTo = "";
                }
                ViewBag.Employee = Employee;
                string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
               , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
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
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                ReportDocument doc = new ReportDocument();
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                var getAllData = _empRepo.EmpTraining(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, topics, EmployeeId);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Employee Training List";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Employee Training List";
                }
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmpTraining.rpt";
                doc.Load(rptLocation);
                //doc.Database.Tables[0].SetDataSource(SymERPHtmlHelper.ConvertToDataTable<EmployeeInfoVM>(getAllData.ToList()));
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy"].SetDataSource(getAllData);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
                doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Name + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult EmpPromotion()
        {
            string Employee = "N";
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Employee = "Y";
            }
            ViewBag.Employee = Employee;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult EmpPromotion(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtjFrom, string dtjTo, string dtpFrom, string dtpTo)
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    ProjectId = "";
                    DepartmentId = "";
                    dtjFrom = "";
                    dtjTo = "";
                    dtpFrom = "";
                    dtpTo = "";
                }
                ViewBag.Employee = Employee;
                #region Value assign to Parameters
                string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
               , vCodeF = "0_0", vCodeT = "0_0", vdtjFrom = "0_0", vdtjTo = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtjFromParam = "[All]", dtjToParam = "[All]"
               , dtpFromParam = "[All]", dtpToParam = "[All]";
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
                if (dtjFrom != "0_0" && dtjFrom != "0" && dtjFrom != "" && dtjFrom != "null" && dtjFrom != null)
                {
                    vdtjFrom = dtjFrom;
                    dtjFromParam = vdtjFrom;
                }
                if (dtjTo != "0_0" && dtjTo != "0" && dtjTo != "" && dtjTo != "null" && dtjTo != null)
                {
                    vdtjTo = dtjTo;
                    dtjToParam = vdtjTo;
                }
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }
                #endregion Value assign to Parameters

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                ReportDocument doc = new ReportDocument();
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                var getAllData = _empRepo.EmpPromotion(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtjFrom, vdtjTo, EmployeeId, vdtpFrom, vdtpTo);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Employee Promotion List";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Employee Promotion List";
                }
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeePromotion.rpt";
                //rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeePromotion_G4S.rpt";
                doc.Load(rptLocation);
                //doc.Database.Tables[0].SetDataSource(SymERPHtmlHelper.ConvertToDataTable<EmployeeInfoVM>(getAllData.ToList()));
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy"].SetDataSource(getAllData);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["dtjFromParam"].Text = "'" + dtjFromParam + "'";
                doc.DataDefinition.FormulaFields["dtjToParam"].Text = "'" + dtjToParam + "'";
                doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
                doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
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

        [Authorize]
        //[HttpGet]
        //public ActionResult EmployeeProfiles()
        //{
        //    string Employee = "N";
        //    if (!(identity.IsAdmin || identity.IsHRM))
        //    {
        //        Employee = "Y";
        //    }
        //    ViewBag.Employee = Employee;
        //    return View();
        //}
        [Authorize]
        //[HttpPost]
        public ActionResult EmployeeProfiles(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string BloodGroup, string view = "")
        {

            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_27", "report").ToString();
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    //Name = "";
                    ProjectId = "";
                    DepartmentId = "";
                    dtpFrom = "";
                    dtpTo = "";
                }
                ViewBag.Employee = Employee;
                if (string.IsNullOrWhiteSpace(view))
                {
                    return View();
                }
                string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
               , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
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
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }
                ReportDocument doc = new ReportDocument();
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                EmployeePermanentAddressRepo _perAddRepo = new EmployeePermanentAddressRepo();
                EmployeePresentAddressRepo _prAddRepo = new EmployeePresentAddressRepo();
                EmployeeDependentRepo _depRepo = new EmployeeDependentRepo();
                EmployeeEducationRepo _eduRepo = new EmployeeEducationRepo();
                EmployeeEmergencyContactRepo _emConRepo = new EmployeeEmergencyContactRepo();
                EmployeeExtraCurriculumActivityRepo _exCurrRepo = new EmployeeExtraCurriculumActivityRepo();
                EmployeeImmigrationRepo _immigRepo = new EmployeeImmigrationRepo();
                EmployeeJobHistoryRepo _jHRepo = new EmployeeJobHistoryRepo();
                EmployeeJobRepo _jobRepo = new EmployeeJobRepo();
                EmployeeLanguageRepo _LnRepo = new EmployeeLanguageRepo();
                EmployeeLeftInformationRepo _LIRepo = new EmployeeLeftInformationRepo();
                EmployeeNomineeRepo _nmRepo = new EmployeeNomineeRepo();
                EmployeePersonalDetailRepo _PDRepo = new EmployeePersonalDetailRepo();
                EmployeePromotionRepo _promotionRepo = new EmployeePromotionRepo();
                EmployeeReferenceRepo _refRepo = new EmployeeReferenceRepo();
                EmployeeTrainingRepo _trainingRepo = new EmployeeTrainingRepo();
                EmployeeTransferRepo _transferRepo = new EmployeeTransferRepo();
                EmployeeTravelRepo _travelferRepo = new EmployeeTravelRepo();
                List<EmployeeInfoVM> getAllEmpInfo = _empRepo.EmployeeProfilesFull(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, BloodGroup, EmployeeId);
                List<EmployeePermanentAddressVM> getAllPerAdd = _perAddRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                                                    //Permanent Address
                List<EmployeePresentAddressVM> getAllPrAdd = _prAddRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                                                    //Present Address
                List<EmployeeDependentVM> getAllDepAdd = _depRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                            //Dependent
                List<EmployeeEducationVM> getAllEdu = _eduRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                             //Education
                List<EmployeeEmergencyContactVM> getAllEmCon = _emConRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                   //EmergencyContact
                List<EmployeeExtraCurriculumActivityVM> getAllExCurr = _exCurrRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);             //ExtraCurriculumActivity
                List<EmployeeImmigrationVM> getAllImmig = _immigRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                    //Immigration
                List<EmployeeJobHistoryVM> getAllJH = _jHRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                                   //JobHistory
                List<EmployeeJobVM> getAllJob = _jobRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                                //Job
                List<EmployeeLanguageVM> getAllLn = _LnRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                                  //Language
                List<EmployeeLeftInformationVM> getAllLI = _LIRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                       //LeftInformation
                List<EmployeeNomineeVM> getAllNm = _nmRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                                 //Nominee
                //List<EmployeePersonalDetailVM> getAllPD = _PDRepo.SelectAll();                        //PersonalDetail
                List<EmployeePromotionVM> getAllPromotion = _promotionRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                 //Promotion
                List<EmployeeReferenceVM> getAllRef = _refRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                                                    //Reference
                List<EmployeeTrainingVM> getAllTraining = _trainingRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                   //Training
                List<EmployeeTransferVM> getAllTransfer = _transferRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                    //Transfer
                List<EmployeeTravelVM> getAllTravel = _travelferRepo.SelectAllForReport(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, EmployeeId);                       //Travel
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Employee Profiles List";
                if (getAllEmpInfo.Count > 0)
                {
                    ReportHead = "Employee Profiles List";
                }
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                string rptLocation = "";
                //rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeProfiles.rpt";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeProfilesFull.rpt";
                doc.Load(rptLocation);
                //doc.Database.Tables[0].SetDataSource(SymERPHtmlHelper.ConvertToDataTable<EmployeeInfoVM>(getAllData.ToList()));
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy"].SetDataSource(getAllEmpInfo);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeePermanentAddressVM_Proxy"].SetDataSource(getAllPerAdd);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeePresentAddressVM_Proxy"].SetDataSource(getAllPrAdd);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeDependentVM_Proxy"].SetDataSource(getAllDepAdd);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeEducationVM_Proxy"].SetDataSource(getAllEdu);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeEmergencyContactVM_Proxy"].SetDataSource(getAllEmCon);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeExtraCurriculumActivityVM_Proxy"].SetDataSource(getAllExCurr);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeImmigrationVM_Proxy"].SetDataSource(getAllImmig);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeJobHistoryVM_Proxy"].SetDataSource(getAllJH);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeJobVM_Proxy"].SetDataSource(getAllJob);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLanguageVM_Proxy"].SetDataSource(getAllLn);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeftInformationVM_Proxy"].SetDataSource(getAllLI);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeNomineeVM_Proxy"].SetDataSource(getAllNm);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeePromotionVM_Proxy"].SetDataSource(getAllPromotion);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeReferenceVM_Proxy"].SetDataSource(getAllRef);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeTrainingVM_Proxy"].SetDataSource(getAllTraining);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeTransferVM_Proxy"].SetDataSource(getAllTransfer);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeTravelVM_Proxy"].SetDataSource(getAllTravel);
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\EmployeeInfo\\";
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                //doc.DataDefinition.FormulaFields["ImgPath"].Text = "'" + fullPath + "'";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
                doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
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

        [Authorize]
        [HttpGet]
        public ActionResult EmployeeBloodGroup()
        {
            string Employee = "N";
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Employee = "Y";
            }
            ViewBag.Employee = Employee;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult EmployeeBloodGroup(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string BloodGroup, string Gender_E, string Religion, string GradeId)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    ProjectId = "";
                    DepartmentId = "";
                    BloodGroup = "";
                }
                ViewBag.Employee = Employee;
                string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
               , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
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
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                ReportDocument doc = new ReportDocument();
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                var getAllData = _empRepo.EmployeeList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                    , vdtpFrom, vdtpTo, BloodGroup, EmployeeId, Gender_E, Religion, GradeId);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Employee Blood Group";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Employee Blood Group List";
                }
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeBloodGroup.rpt";
                doc.Load(rptLocation);
                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeInfoVM_Proxy"].SetDataSource(getAllData);
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\EmployeeInfo\\";
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["dtpFromParam"].Text = "'" + dtpFromParam + "'";
                doc.DataDefinition.FormulaFields["dtpToParam"].Text = "'" + dtpToParam + "'";
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

        #endregion


        [Authorize]
        public ActionResult Letters(EmployeeInfoVM vm)
        {
            try
            {
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();             
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_28", "report").ToString();
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    ////////vm = new SalarySheetVM();
                    vm.EmployeeId = identity.EmployeeId;
                    vm.CodeF = identity.EmployeeCode;
                    vm.CodeT = identity.EmployeeCode;
                }

                if (string.IsNullOrWhiteSpace(vm.View))
                {
                    return View(vm);
                }


                ReportDocument doc = new ReportDocument();
                DataSet ds = new DataSet();


                string rptLocation = "";
                string ReportHead = "";
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                #region EnumReport
                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

                string[] conFields = { "ReportType", "ReportId" };
                string[] conValues = { "HRMLetter", vm.LetterName };
                enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);
                EnumReportVM varEnumReportVM = new EnumReportVM();
                varEnumReportVM = enumReportVMs.FirstOrDefault();

                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                //"Appointment Letter", "Transfer Letter", "Promotion Letter", "Increment Letter"
                string ReportFileName = varEnumReportVM.ReportFileName;
                string ReportName = varEnumReportVM.Name;
                ReportHead = "There are no data to Preview for this " + ReportName;
                #endregion EnumReport

                if (!string.IsNullOrWhiteSpace(vm.MultipleCode))
                {
                    vm.CodeList = vm.MultipleCode.Split(',').ToList();
                }

                string[] conditionFields = { "e.Code>", "e.Code<", "e.DesignationId", "e.DepartmentId", "e.SectionId", "e.ProjectId", "e.JoinDate>", "e.JoinDate<" };
                string[] conditionValues = { vm.CodeF, vm.CodeT, vm.DesignationId, vm.DepartmentId, vm.SectionId, vm.ProjectId, Ordinary.DateToString(vm.JoinDateFrom), Ordinary.DateToString(vm.JoinDateTo) };


                DataTable getAllData = _empRepo.EmployeeListLetter(vm, conditionFields, conditionValues);

                ds.Tables.Add(getAllData);
                ds.Tables[0].TableName = "dtEmployeeListLetter";

                if (getAllData.Rows.Count > 0)
                {
                    ReportHead = ReportName;
                }
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\Letters\" + ReportFileName + ".rpt";


                doc.Load(rptLocation);
                doc.SetDataSource(ds);

                string LogoName = "COMPANYLOGO";  
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\" + LogoName + ".png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["IssueDate"].Text = "'" + vm.IssueDate + "'";
                doc.DataDefinition.FormulaFields["ReferenceNumber"].Text = "'" + vm.ReferenceNumber + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";

                if (ReportName == "Promotion Letter")
                {
                    DataTable dt = _empRepo.EmployeePromotionValue(vm);
                    if (dt.Rows.Count > 0)
                    {                        
                        DateTime ProDate = DateTime.ParseExact(dt.Rows[0]["PromotionDate"].ToString(), "yyyyMMdd", null);
                        doc.SetParameterValue("PreviousDesignation", dt.Rows[0]["PreviousDesignation"].ToString());
                        doc.SetParameterValue("NewDesignation", dt.Rows[0]["NewDesignation"].ToString());
                        doc.SetParameterValue("PromotionDate", ProDate);                       
                    }
                }
                if (ReportName == "Increment Letter")
                {
                    DataTable dt = _empRepo.EmployeeIncrementValue(vm);
                    if (dt.Rows.Count > 0)
                    {
                        doc.Subreports["SubReport"].SetDataSource(dt);
                    }
                }                

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        [Authorize]
        public ActionResult TravelLetter(EmployeeInfoVM vm)
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_28", "report").ToString();

                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    ////////vm = new SalarySheetVM();
                    vm.EmployeeId = identity.EmployeeId;
                    vm.CodeF = identity.EmployeeCode;
                    vm.CodeT = identity.EmployeeCode;
                }


                if (string.IsNullOrWhiteSpace(vm.View))
                {
                    return View(vm);
                }

                ReportDocument doc = new ReportDocument();
                DataSet ds = new DataSet();


                string rptLocation = "";
                string ReportHead = "";
                #region EnumReport
                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

                string[] conFields = { "ReportType", "ReportId" };
                string[] conValues = { "TravelLetter", vm.LetterName };
                enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);
                EnumReportVM varEnumReportVM = new EnumReportVM();
                varEnumReportVM = enumReportVMs.FirstOrDefault();

                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                //"Appointment Letter", "Transfer Letter", "Promotion Letter", "Increment Letter"
                string ReportFileName = varEnumReportVM.ReportFileName;
                string ReportName = varEnumReportVM.Name;
                ReportHead = "There are no data to Preview for this " + ReportName;
                #endregion EnumReport

                if (!string.IsNullOrWhiteSpace(vm.MultipleCode))
                {
                    vm.CodeList = vm.MultipleCode.Split(',').ToList();
                }


                //--and et.FromDate >= 20180101 and  et.ToDate <=20180114 
                string[] conditionFields = { "e.Code>", "e.Code<", "e.DesignationId", "e.DepartmentId", "e.SectionId"
                                               , "e.ProjectId", "e.JoinDate>", "e.JoinDate<", "et.FromDate>", "et.ToDate<" };
                string[] conditionValues = { vm.CodeF, vm.CodeT, vm.DesignationId, vm.DepartmentId, vm.SectionId, vm.ProjectId
                                               , Ordinary.DateToString(vm.JoinDateFrom), Ordinary.DateToString(vm.JoinDateTo)
                                               , Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo)  };


                DataTable getAllData = _empRepo.EmployeeListTravelLetter(vm, conditionFields, conditionValues);

                ds.Tables.Add(getAllData);
                ds.Tables[0].TableName = "dtEmployeeListTravelLetter";

                if (getAllData.Rows.Count > 0)
                {
                    ReportHead = ReportName;
                }
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\Letters\" + ReportFileName + ".rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);

                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["IssueDate"].Text = "'" + vm.IssueDate + "'";
                doc.DataDefinition.FormulaFields["ReferenceNumber"].Text = "'" + vm.ReferenceNumber + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Backup

        public ActionResult LettersBackup(string CodeF, string CodeT, string DepartmentId, string SectionId
          , string ProjectId, string DesignationId, string dtjFrom, string dtjTo, string dtFrom, string dtTo, string ReportNo, string LetterName, string empcodes
          , string issueDate, string referenceNumber)
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_28", "report").ToString();
                ViewBag.permission = "true";
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    ProjectId = "";
                    dtjFrom = "";
                    dtjTo = "";
                    dtFrom = "";
                    dtTo = "";
                }
                ViewBag.Employee = Employee;
                if (string.IsNullOrWhiteSpace(ReportNo))
                {
                    return View();
                }
                List<string> a = empcodes.Split(',').ToList();
                var b = a;
                #region Value assign to Parameters
                string vProjectId = "", vDepartmentId = "", vSectionId = "", vDesignationId = ""
                , vCodeF = "", vCodeT = "", vdtjFrom = "", vdtjTo = "", vdtFrom = "", vdtTo = "";


                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtjFromParam = "[All]", dtjToParam = "[All]"
               , dtFromParam = "[All]", dtToParam = "[All]";


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
                if (dtjFrom != "0_0" && dtjFrom != "0" && dtjFrom != "" && dtjFrom != "null" && dtjFrom != null)
                {
                    vdtjFrom = dtjFrom;
                    dtjFromParam = vdtjFrom;
                }
                if (dtjTo != "0_0" && dtjTo != "0" && dtjTo != "" && dtjTo != "null" && dtjTo != null)
                {
                    vdtjTo = dtjTo;
                    dtjToParam = vdtjTo;
                }
                if (dtFrom != "0_0" && dtFrom != "0" && dtFrom != "" && dtFrom != "null" && dtFrom != null)
                {
                    vdtFrom = dtFrom;
                    dtFromParam = vdtFrom;
                }
                if (dtTo != "0_0" && dtTo != "0" && dtTo != "" && dtTo != "null" && dtTo != null)
                {
                    vdtTo = dtTo;
                    dtToParam = vdtTo;
                }
                #endregion Value assign to Parameters
                ReportDocument doc = new ReportDocument();
                DataSet ds = new DataSet();


                string rptLocation = "";
                string ReportHead = "";
                #region EnumReport
                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

                string[] conFields = { "ReportType" };
                string[] conValues = { "HRMLetter" };
                enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);


                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                //"Appointment Letter", "Transfer Letter", "Promotion Letter", "Increment Letter"
                string ReportFileName = enumReportVMs.Where(c => c.ReportId == LetterName).FirstOrDefault().ReportFileName;
                string ReportName = enumReportVMs.Where(c => c.ReportId == LetterName).FirstOrDefault().Name;
                ReportHead = "There are no data to Preview for this " + ReportName;
                #endregion EnumReport

                string[] conditionFields = { "e.Code>", "e.Code<", "e.DesignationId", "e.DepartmentId", "e.SectionId", "e.ProjectId", "e.JoinDate>", "e.JoinDate<" };
                string[] conditionValues = { vCodeF, vCodeT, vDesignationId, vDepartmentId, vSectionId, vProjectId, Ordinary.DateToString(vdtjFrom), Ordinary.DateToString(vdtjTo) };


                DataTable getAllData = _empRepo.EmployeeListLetter(null, conditionFields, conditionValues);

                ds.Tables.Add(getAllData);
                ds.Tables[0].TableName = "dtEmployeeListLetter";

                if (getAllData.Rows.Count > 0)
                {
                    ReportHead = ReportName;
                }
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\Letters\" + ReportFileName + ".rpt";


                doc.Load(rptLocation);
                doc.SetDataSource(ds);

                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["IssueDate"].Text = "'" + issueDate + "'";
                doc.DataDefinition.FormulaFields["ReferenceNumber"].Text = "'" + referenceNumber + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ActionResult TravelLetterBackup(string CodeF, string CodeT, string DepartmentId, string SectionId
           , string ProjectId, string DesignationId, string dtjFrom, string dtjTo, string dtFrom, string dtTo, string ReportNo, string LetterName, string empcodes
           , string issueDate, string referenceNumber)
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_28", "report").ToString();
                ViewBag.permission = "true";
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    ProjectId = "";
                    dtjFrom = "";
                    dtjTo = "";
                    dtFrom = "";
                    dtTo = "";
                }
                ViewBag.Employee = Employee;
                if (string.IsNullOrWhiteSpace(ReportNo))
                {
                    return View();
                }
                List<string> a = empcodes.Split(',').ToList();
                var b = a;
                #region Value assign to Parameters
                string vProjectId = "", vDepartmentId = "", vSectionId = "", vDesignationId = ""
                , vCodeF = "", vCodeT = "", vdtjFrom = "", vdtjTo = "", vdtFrom = "", vdtTo = "";


                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtjFromParam = "[All]", dtjToParam = "[All]"
               , dtFromParam = "[All]", dtToParam = "[All]";


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
                if (dtjFrom != "0_0" && dtjFrom != "0" && dtjFrom != "" && dtjFrom != "null" && dtjFrom != null)
                {
                    vdtjFrom = dtjFrom;
                    dtjFromParam = vdtjFrom;
                }
                if (dtjTo != "0_0" && dtjTo != "0" && dtjTo != "" && dtjTo != "null" && dtjTo != null)
                {
                    vdtjTo = dtjTo;
                    dtjToParam = vdtjTo;
                }
                if (dtFrom != "0_0" && dtFrom != "0" && dtFrom != "" && dtFrom != "null" && dtFrom != null)
                {
                    vdtFrom = dtFrom;
                    dtFromParam = vdtFrom;
                }
                if (dtTo != "0_0" && dtTo != "0" && dtTo != "" && dtTo != "null" && dtTo != null)
                {
                    vdtTo = dtTo;
                    dtToParam = vdtTo;
                }
                #endregion Value assign to Parameters
                ReportDocument doc = new ReportDocument();
                DataSet ds = new DataSet();


                string rptLocation = "";
                string ReportHead = "";
                #region EnumReport
                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

                string[] conFields = { "ReportType" };
                string[] conValues = { "TravelLetter" };
                enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);


                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                //"Appointment Letter", "Transfer Letter", "Promotion Letter", "Increment Letter"
                string ReportFileName = enumReportVMs.Where(c => c.ReportId == LetterName).FirstOrDefault().ReportFileName;
                string ReportName = enumReportVMs.Where(c => c.ReportId == LetterName).FirstOrDefault().Name;
                ReportHead = "There are no data to Preview for this " + ReportName;
                #endregion EnumReport
                //--and et.FromDate >= 20180101 and  et.ToDate <=20180114 
                string[] conditionFields = { "e.Code>", "e.Code<", "e.DesignationId", "e.DepartmentId", "e.SectionId"
                                               , "e.ProjectId", "e.JoinDate>", "e.JoinDate<", "et.FromDate>", "et.ToDate<" };
                string[] conditionValues = { vCodeF, vCodeT, vDesignationId, vDepartmentId, vSectionId, vProjectId
                                               , Ordinary.DateToString(vdtjFrom), Ordinary.DateToString(vdtjTo)
                                               , Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo)  };


                DataTable getAllData = _empRepo.EmployeeListTravelLetter(null, conditionFields, conditionValues);

                ds.Tables.Add(getAllData);
                ds.Tables[0].TableName = "dtEmployeeListTravelLetter";

                if (getAllData.Rows.Count > 0)
                {
                    ReportHead = ReportName;
                }
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\Letters\" + ReportFileName + ".rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);

                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["IssueDate"].Text = "'" + issueDate + "'";
                doc.DataDefinition.FormulaFields["ReferenceNumber"].Text = "'" + referenceNumber + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult SalaryCertificateBackup(string fid, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
    , string other1, string other2, string other3, string bankId
    , string Orderby, string MulitpleProjectId, string view)
        {
            return null;

            #region Comments

            //////string[] result = new string[6];
            //////try
            //////{
            //////    var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
            //////    Session["permission"] = permission;
            //////    if (permission == "False")
            //////    {
            //////        return Redirect("/Payroll/Home");
            //////    }
            //////    if (string.IsNullOrWhiteSpace(view) || view == "Y")
            //////    {
            //////        return View();
            //////    }
            //////    string vProjectId = "0_0";
            //////    string vDepartmentId = "0_0";
            //////    string vSectionId = "0_0";
            //////    string vDesignationId = "0_0";
            //////    string vCodeF = "0_0";
            //////    string vCodeT = "0_0";
            //////    List<string> ProjectIdList = new List<string>();
            //////    if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
            //////    {
            //////        vProjectId = ProjectId;
            //////    }
            //////    if (MulitpleProjectId != "0_0" && MulitpleProjectId != "0" && MulitpleProjectId != "" && MulitpleProjectId != "null" && MulitpleProjectId != null)
            //////    {
            //////        ProjectIdList = MulitpleProjectId.Split(',').ToList();
            //////    }
            //////    if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
            //////        vDepartmentId = DepartmentId;
            //////    if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
            //////        vSectionId = SectionId;
            //////    if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
            //////        vDesignationId = DesignationId;
            //////    if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
            //////        vCodeF = CodeF;
            //////    if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
            //////        vCodeT = CodeT;
            //////    ReportDocument doc = new ReportDocument();

            //////    SalaryProcessRepo _empRepo = new SalaryProcessRepo();
            //////    DataSet ds = new DataSet();

            //////    ds = _empRepo.SalaryPreCalculationNew(fid, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT
            //////        , Orderby, other1, other2, other3, bankId, ProjectIdList);
            //////    if (ds.Tables[0].Rows.Count == 0)
            //////    {
            //////        result[0] = "Fail";
            //////        result[1] = "No Data Found";
            //////        Session["result"] = result[0] + "~" + result[1];

            //////        return View();
            //////    }


            //////    EnumReportRepo _reportRepo = new EnumReportRepo();
            //////    List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

            //////    string[] conFields = { "ReportType", "ReportName" };
            //////    string[] conValues = { "SalarySheet", "Salary Certificate" };
            //////    enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);

            //////    var salaryCertificate = enumReportVMs.FirstOrDefault();
            //////    string ReportFileName = salaryCertificate.ReportFileName;
            //////    string ReportName = salaryCertificate.Name;

            //////    SettingRepo _sRepo = new SettingRepo();

            //////    //_sRepo.settingValue("SalarySheet", "SalarySheet(1)");
            //////    ds.Tables[0].TableName = "dtSalarySheet";
            //////    var FullPeriodName = Convert.ToDateTime("01-" + ds.Tables[0].Rows[0]["PeriodName"].ToString()).ToString("MMMM-yyyy");
            //////    string rptLocation = "";

            //////    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" + ReportFileName + ".rpt";

            //////    doc.Load(rptLocation);
            //////    string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
            //////    doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
            //////    doc.DataDefinition.FormulaFields["FullPeriodName"].Text = "'" + FullPeriodName + "'";

            //////    doc.SetDataSource(ds);
            //////    var rpt = RenderReportAsPDF(doc);
            //////    doc.Close();
            //////    return rpt;
            //////}
            //////catch (Exception ex)
            //////{
            //////    result[0] = "Fail";
            //////    result[1] = "Process Fail";
            //////    Session["result"] = result[0] + "~" + result[1];

            //////    return View();
            //////    throw;
            //////}
            #endregion

        }

        public ActionResult ManualRosterAttendanceReportViewBackup(string codeFrom, string codeTo, string departmentId, string projectId, string sectionId, string dtFrom, string dtTo, string attnStatus, string fullOT)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                ////AttendanceDailyNewVM vm = new AttendanceDailyNewVM();
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
                DailyAttendanceProcessRepo _repo = new DailyAttendanceProcessRepo();
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

                string Logo = new AppSettingsReader().GetValue("Logo", typeof(string)).ToString();

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\" + Logo;
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


        #endregion

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult ManualRosterAttendanceReport(EmployeeInfoVM vm)
        {
             string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "70002", "report").ToString();
            vm.CompanyName = CompanyName;
            return View(vm);
        }
        [Authorize]
        public ActionResult ManualRosterAttendanceReportView(EmployeeInfoVM vm)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    ////////vm = new SalarySheetVM();
                    vm.EmployeeId = identity.EmployeeId;
                    vm.CodeF = identity.EmployeeCode;
                    vm.CodeT = identity.EmployeeCode;
                }

                string[] conFields = { "ve.Code>", "ve.Code<", "ve.DepartmentId", "ve.ProjectId", "ve.SectionId", "attn.DailyDate>", "attn.DailyDate<" };
                string[] conValues = { vm.CodeF, vm.CodeT, vm.DepartmentId, vm.ProjectId, vm.SectionId, Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo) };

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                DailyAttendanceProcessRepo _repo = new DailyAttendanceProcessRepo();
                table = _repo.Report(vm, conFields, conValues);
                ReportHead = "There are no data to Preview for Attendance Daily (" + vm.DateTo + ")";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "Attendance Daily List (From "+ vm.DateFrom + " to " + vm.DateTo + ")";
                }
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtDailyAttendanceProcess";
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                if (CompanyName.ToUpper() == "G4S")
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptDailyAttendanceProcess.rpt";

                    if (!string.IsNullOrWhiteSpace(vm.CodeF) && vm.CodeF == vm.CodeT)
                    {
                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptDailyAttendanceSingle_G4S.rpt";
                    }
                }
                else
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptDailyAttendanceProcess.rpt";

                    if (!string.IsNullOrWhiteSpace(vm.CodeF) && vm.CodeF == vm.CodeT)
                    {
                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptDailyAttendanceSingle.rpt";
                    }
                }

                string Logo = new AppSettingsReader().GetValue("Logo", typeof(string)).ToString();

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\" + Logo;
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["FullOT"].Text = "'" + vm.FullOT + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'"; 

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AttendanceSummeryReport(EmployeeInfoVM vm)
        {
            string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "70002", "report").ToString();
            vm.CompanyName = CompanyName;
            return View(vm);
        }

        [Authorize]
        public ActionResult AttendanceSummeryReportView(EmployeeInfoVM vm)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                if (!(identity.IsAdmin || identity.IsHRM))
                {                    
                    vm.EmployeeId = identity.EmployeeId;
                    vm.CodeF = identity.EmployeeCode;
                    vm.CodeT = identity.EmployeeCode;
                }

                string[] conFields = { "ve.Code>", "ve.Code<", "DailyDate>", "DailyDate<" };
                string[] conValues = { vm.CodeF, vm.CodeT, Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo) };

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                DailyAttendanceProcessRepo _repo = new DailyAttendanceProcessRepo();
                table = _repo.ReportAttendanceSummery(vm, conFields, conValues);
                ReportHead = "There are no data to Preview for Attendance Daily (" + vm.DateTo + ")";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "Attendance Summery (From " + vm.DateFrom + " to " + vm.DateTo + ")";
                }
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtDailyAttendanceProcess";
              
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptDailyAttendanceSummery.rpt";               

                string Logo = new AppSettingsReader().GetValue("Logo", typeof(string)).ToString();

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\" + Logo;
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["FullOT"].Text = "'" + vm.FullOT + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult SalaryCertificate(SalarySheetVM vm)
        
        {
            string[] result = new string[6];
            string ReportName = "";
            try
            {
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/HRM/Home");
                }

                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    vm.CodeFrom = identity.EmployeeCode;
                    vm.CodeTo = identity.EmployeeCode;
                }


                if (string.IsNullOrWhiteSpace(vm.View) || vm.View == "Y")
                {
                    vm.PaymentDate = DateTime.Now.ToString("dd-MMM-yyyy");
                    return View(vm);
                }
                if (vm.FiscalYearDetailIdTo == 0)
                {
                    vm.FiscalYearDetailIdTo = vm.FiscalYearDetailId;
                }


                if (vm.FiscalYearDetailId != vm.FiscalYearDetailIdTo)
                {
                    vm.IsMultipleSalary = true;
                }

                if (!string.IsNullOrWhiteSpace(vm.MultipleProjectId))
                {
                    vm.MultipleProjectId = vm.MultipleProjectId.Trim(',');
                    vm.ProjectIdList = vm.MultipleProjectId.Split(',').ToList();
                }

                ReportDocument doc = new ReportDocument();
                ReportDocument doc1 = new ReportDocument();

                SalaryProcessRepo _empRepo = new SalaryProcessRepo();
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();
                string ReportFileName = "";
                if (CompanyName.ToUpper() == "TIB")//tib
                {
                    #region EnumReport
                    string[] conFields = { "ReportType", "ReportId" };
                    string[] conValues = { "SalaryCertificate", vm.SheetName };
                    enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);

                    EnumReportVM varEnumReportVM = enumReportVMs.FirstOrDefault();
                    ReportFileName = varEnumReportVM.ReportFileName;
                    ReportName = varEnumReportVM.Name;
                    #endregion
                    DataTable dt1 = new DataTable();
                    vm.SheetName = varEnumReportVM.ReportId;
                    if (vm.FiscalYear.ToString() == "2024" && CompanyName.ToUpper() == "TIB" && vm.FiscalYearDetailId.ToString() == "1100" && vm.FiscalYearDetailIdTo.ToString() == "1111")
                    {
                        
                        SalaryProcessRepo _SalaryProcessRepo = new SalaryProcessRepo();
                        vm.CompanyName = CompanyName.ToUpper();
                        dt = _SalaryProcessRepo.TAX_108(vm);
                        dt1 = _SalaryProcessRepo.YearlyTAX(vm);


                        var dataView = new DataView(dt);
                        dt = dataView.ToTable(false, "EmployeeId", "EmpName", "Code", "TIN", "Section", "Designation", "Project", "Gender", "JoinDate", "LeftDate", "Email", "Basic", "HouseRent", "Medical", "TransportAllowance", "Gross", "PFEmployer", "ChildAllowance", "HARDSHIP", "LeaveEncashment"
                            , "TransportBill", "TAXDeduction", "Bonus", "Othere_OT", "RecognizedPF", "RecognizedGF", "Principal", "Profit", "TotalPaid", "RebateAmount", "TotalExemptedAmount");
                        dt.Columns["Basic"].ColumnName = "BasicSalary";
                        dt.Columns["Gross"].ColumnName = "GrossSalary";
                        dt.Columns["TransportAllowance"].ColumnName = "ConveyanceAllowance";
                        dt.Columns["TransportBill"].ColumnName = "TransferAllowance";
                        dt.Columns["TAXDeduction"].ColumnName = "TAXSalary";
                        dt.Columns["Othere_OT"].ColumnName = "Othere(OT)";
                        System.Data.DataColumn ChargeAllowance = new System.Data.DataColumn("ChargeAllowance", typeof(System.Decimal));
                        ChargeAllowance.DefaultValue = 0;
                        dt.Columns.Add(ChargeAllowance);
                        System.Data.DataColumn RestLeaveAllowance = new System.Data.DataColumn("RestLeaveAllowance", typeof(System.Decimal));
                        RestLeaveAllowance.DefaultValue = 0;
                        dt.Columns.Add(RestLeaveAllowance);
                        dt.TableName = "dtTIB_HRMSalary";
                        dt1.TableName = "dtYearlyTAX";

                        if (vm.SheetName.ToLower() == "salarysheet15")
                        {
                            dt = GetGreaterThanZero(dt);

                            dt.TableName = "dtTAX108";
                        }

                        ds.Tables.Add(dt);
                        ds.Tables.Add(dt1);
                    }
                    else
                    {
                        dt = _empRepo.TIBHRMSalary(vm);
                        

                        if (vm.SheetName.ToLower() == "salarysheet11")
                        {
                            dt.TableName = "dtTIB_HRMSalary";
                        }
                        else if (vm.SheetName.ToLower() == "salarysheet12")
                        {
                            dt.TableName = "dtTIB_HRMSalary";                        
                        }
                        else if (vm.SheetName.ToLower() == "salarysheet13")
                        {
                            dt.TableName = "dtTIB_HRMSalary";
                        }
                        else if (vm.SheetName.ToLower() == "salarysheet15")
                        {
                            dt.TableName = "dtTIB_HRMSalary";
                        }
                        else 
                        {
                            dt.TableName = "dtSalarySheet";
                        }
                        ds.Tables.Add(dt);
                    }


                }
                else
                {

                    #region EnumReport
                    string[] conFields = { "ReportType", "ReportId" };
                    string[] conValues = { "SalaryCertificate", vm.SheetName }; ////"Salary Certificate"
                    enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);
                    DataTable dt1 = new DataTable();
                    EnumReportVM varEnumReportVM = enumReportVMs.FirstOrDefault();
                    ReportFileName = varEnumReportVM.ReportFileName;
                    ReportName = varEnumReportVM.Name;
                    #endregion

                    vm.SheetName = varEnumReportVM.ReportId;

                    dt = _empRepo.SalarySheet(vm);
                    dt.TableName = "dtTIB_HRMSalary";

                    ////dt.TableName = "dtAnother_HRMSalary";

                    dt1.TableName = "dtYearlyTAX";
                    //dt.TableName = "dtAnother_HRMSalary";
                    DataTable copyDT = new DataTable();
                    copyDT = dt.Copy();
                    ds.Tables.Add(copyDT);

                    //DataTable copyDT1 = new DataTable();
                    //copyDT = dt1.Copy();
                    //ds.Tables.Add(copyDT1);
                }


                //////ds = _empRepo.SalaryPreCalculationNew(fid, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT
                //////    , Orderby, other1, other2, other3, bankId, ProjectIdList);

                if (dt.Rows.Count == 0)
                {
                    result[0] = "Fail";
                    result[1] = "No Data Found";
                    Session["result"] = result[0] + "~" + result[1];

                    return View(vm);
                }

                FiscalYearDetailVM fydVM = new FiscalYearRepo().FYPeriodDetail(Convert.ToInt32(vm.FiscalYearDetailId)).FirstOrDefault();

                string PeriodName = fydVM.PeriodName;

                var FullPeriodName = Convert.ToDateTime("01-" + PeriodName).ToString("MMMM-yyyy");
                var IncomeYearstart = Convert.ToDateTime("01-" + PeriodName).ToString("yyyy");
                int AssessmentYear = Convert.ToInt32(Convert.ToInt32(IncomeYearstart) + 1);
                string AssesMentYear = "";


                if (vm.IsMultipleSalary)
                {
                    fydVM = new FiscalYearRepo().FYPeriodDetail(Convert.ToInt32(vm.FiscalYearDetailIdTo)).FirstOrDefault();

                    string PeriodNameTo = fydVM.PeriodName;
                    string IncomeYearEnd = Convert.ToDateTime("01-" + PeriodNameTo).ToString("yyyy");
                    int AssessmentYearTo = Convert.ToInt32(Convert.ToInt32(IncomeYearEnd) + 1);

                    string FullPeriodNameTo = Convert.ToDateTime("01-" + PeriodNameTo).ToString("MMMM-yyyy");


                    FullPeriodName = FullPeriodName + " to " + FullPeriodNameTo;
                    IncomeYearstart = IncomeYearstart + "-" + IncomeYearEnd;
                    AssesMentYear = AssessmentYear + "-" + AssessmentYearTo;
                }
                string rptLocation = "";
                string rptFinalSattlementLocation = "";
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" + ReportFileName + ".rpt";

                if (vm.SheetName.ToLower() == "salarysheet14")
                {
                    rptFinalSattlementLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\RptYearlyBenifitTIB.rpt";
                    doc1.Load(rptFinalSattlementLocation);

                }
                if (vm.SheetName.ToLower() == "salarysheet15")
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\RptYearlyBenifitTIB.rpt";


                    ds.Tables[0].TableName = "dtTAX108";
                }
                doc.Load(rptLocation);
                companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = doc.DataDefinition.FormulaFields;            
                FormulaField(doc, crFormulaF, "IncomeYearstart", IncomeYearstart);
                FormulaField(doc, crFormulaF, "AssesMentYear", AssesMentYear);              
                FormulaField(doc, crFormulaF, "EmployeeCode", vm.Code);
                FormulaField(doc, crFormulaF, "JoinDate", vm.JoinDate.ToString());
                FormulaField(doc, crFormulaF, "EmployeeCode", vm.Gender);            

                if (vm.FiscalYear.ToString() == "2023" && CompanyName.ToUpper() == "TIB" && vm.FiscalYearDetailId.ToString() == "1088" && vm.FiscalYearDetailIdTo.ToString() == "1099")
                {
                    FormulaField(doc, crFormulaF, "IsCalculateBonus", "N");

                }
                if (vm.SheetName.ToLower() == "salarysheet14")
                {
                    if (CompanyName.ToUpper() == "TIB")//tib
                    {

                        thread = new Thread(unused => EmailSalaryCertificateTIB(ds, doc, doc1, IncomeYearstart, AssesMentYear));
                        thread.Start();
                        // EmpEmailProcess(ds, doc, FullPeriodName)
                        result[0] = "Success";
                        result[1] = "Salary Certificate Yearly Email Send";
                        Session["result"] = result[0] + "~" + result[1];
                        //return Redirect("/Acc/Home/");

                    }
                }
                else
                {
                    doc.SetDataSource(ds);

                    //doc.DataDefinition.
                    if (vm.ReportType == "PDF")
                    {
                        var rpt = RenderReportAsPDF(doc);
                        doc.Close();
                        return rpt;
                    }
                    else if (vm.ReportType == "EXCEL")
                    {
                        var rpt = RenderReportAsEXCEL(doc, ReportName);
                        doc.Close();
                        return rpt;
                    }
                    else if (vm.ReportType == "WORD")
                    {
                        var rpt = RenderReportAsWord(doc, ReportName);
                        doc.Close();
                        return rpt;
                    }

                }


                return View();
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = "Process Fail";
                Session["result"] = result[0] + "~" + result[1];

                return View();
                throw;
            }
        }

        private DataTable GetGreaterThanZero(DataTable dt)
        {
            var rows = dt.AsEnumerable().Where(x =>
                Convert.ToDecimal(x["RecognizedPF"]) != 0 && Convert.ToDecimal(x["RecognizedGF"]) != 0);

            if (rows.Any())
            {
                dt = rows.CopyToDataTable();
            }
            else
            {
                dt.Clear();
            }

            return dt;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EmployeeNewReport()
        {
            string Employee = "N";
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Employee = "Y";
            }
            ViewBag.Employee = Employee;
            EmployeeInfoVM vm = new EmployeeInfoVM();
            return View(vm);
        }

        [Authorize]
        public ActionResult EmployeeNewReportView(EmployeeInfoVM vm)
        {
            try
            {
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();               

                #region Objects & Variables
                ReportDocument doc = new ReportDocument();
                DataSet ds = new DataSet();
                DataTable masterDataDT = new DataTable();
                DataTable detailDataDT = new DataTable();
                EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                string ReportFileName = "";
                string ReportHeader = "";
                string rptLocation = "";
                string ReportHead = "";

                #endregion
                if (vm.ReportName == "HRM100")
                {
                    goto HRM100;
                }

                #region ESS User
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_28", "report").ToString();

                //string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    vm = new EmployeeInfoVM();
                    vm.EmployeeId = identity.EmployeeId;
                    vm.CodeF = identity.EmployeeCode;
                    vm.CodeT = identity.EmployeeCode;
                }

                #endregion
                #region Multiple Selection Parameters


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
                #region Value assign to Parameters

                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtjFromParam = "[All]", dtjToParam = "[All]"
               , dtFromParam = "[All]", dtToParam = "[All]"
               , TaxStructureParam = "[All]"
               , PFStructureParam = "[All]"
               , EDStructureParam = "[All]"
               , LeaveStructureParam = "[All]"
               , GroupParam = "[All]"
               ;


                if (vm.ProjectId == "0_0" || vm.ProjectId == "0" || vm.ProjectId == "" || vm.ProjectId == "null" || vm.ProjectId == null)
                {
                    vm.ProjectId = "";
                }
                else
                {
                    projectParam = new ProjectRepo().SelectById(vm.ProjectId).Name;
                }
                if (vm.DepartmentId == "0_0" || vm.DepartmentId == "0" || vm.DepartmentId == "" || vm.DepartmentId == "null" || vm.DepartmentId == null)
                {
                    vm.DepartmentId = "";
                }
                else
                {
                    deptParam = new DepartmentRepo().SelectById(vm.DepartmentId).Name;
                }
                if (vm.SectionId == "0_0" || vm.SectionId == "0" || vm.SectionId == "" || vm.SectionId == "null" || vm.SectionId == null)
                {
                    vm.SectionId = "";
                }
                else
                {
                    secParam = new SectionRepo().SelectById(vm.SectionId).Name;
                }
                if (vm.DesignationId == "0_0" || vm.DesignationId == "0" || vm.DesignationId == "" || vm.DesignationId == "null" || vm.DesignationId == null)
                {
                    vm.DesignationId = "";
                }
                else
                {
                    desigParam = new DesignationRepo().SelectById(vm.DesignationId).Name;
                }
                if (vm.CodeF == "0_0" || vm.CodeF == "0" || vm.CodeF == "" || vm.CodeF == "null" || vm.CodeF == null)
                {
                    vm.CodeF = "";
                }
                else
                {
                    codeFParam = vm.CodeF;
                }
                if (vm.CodeT == "0_0" || vm.CodeT == "0" || vm.CodeT == "" || vm.CodeT == "null" || vm.CodeT == null)
                {
                    vm.CodeT = "";
                }
                else
                {
                    codeTParam = vm.CodeT;
                }
                if (vm.JoinDateFrom == "0_0" || vm.JoinDateFrom == "0" || vm.JoinDateFrom == "" || vm.JoinDateFrom == "null" || vm.JoinDateFrom == null)
                {
                    vm.JoinDateFrom = "";
                }
                else
                {
                    dtjFromParam = vm.JoinDateFrom;
                }
                if (vm.JoinDateTo == "0_0" || vm.JoinDateTo == "0" || vm.JoinDateTo == "" || vm.JoinDateTo == "null" || vm.JoinDateTo == null)
                {
                    vm.JoinDateTo = "";
                }
                else
                {
                    dtjToParam = vm.JoinDateTo;
                }
                if (vm.DateFrom == "0_0" || vm.DateFrom == "0" || vm.DateFrom == "" || vm.DateFrom == "null" || vm.DateFrom == null)
                {
                    vm.DateFrom = "";
                }
                else
                {
                    dtFromParam = vm.DateFrom;
                }
                if (vm.DateTo == "0_0" || vm.DateTo == "0" || vm.DateTo == "" || vm.DateTo == "null" || vm.DateTo == null)
                {
                    vm.DateTo = "";
                }
                else
                {
                    dtToParam = vm.DateTo;
                }

                if (vm.TaxStructureId == "0_0" || vm.TaxStructureId == "0" || vm.TaxStructureId == "" || vm.TaxStructureId == "null" || vm.TaxStructureId == null)
                {
                    vm.TaxStructureId = "";
                }
                else
                {
                    TaxStructureParam = new TaxStructureRepo().SelectById(vm.TaxStructureId).Name;
                }

                if (vm.PFStructureId == "0_0" || vm.PFStructureId == "0" || vm.PFStructureId == "" || vm.PFStructureId == "null" || vm.PFStructureId == null)
                {
                    vm.PFStructureId = "";
                }
                else
                {
                    PFStructureParam = new PFStructureRepo().SelectById(vm.PFStructureId).Name;
                }
                if (vm.EDStructureId == "0_0" || vm.EDStructureId == "0" || vm.EDStructureId == "" || vm.EDStructureId == "null" || vm.EDStructureId == null)
                {
                    vm.EDStructureId = "";
                }
                else
                {
                    EDStructureParam = new EarningDeductionStructureRepo().SelectAll(Convert.ToInt32(vm.EDStructureId)).FirstOrDefault().Name;
                }
                if (vm.LeaveStructureId == "0_0" || vm.LeaveStructureId == "0" || vm.LeaveStructureId == "" || vm.LeaveStructureId == "null" || vm.LeaveStructureId == null)
                {
                    vm.LeaveStructureId = "";
                }
                else
                {
                    LeaveStructureParam = new LeaveStructureRepo().SelectById(Convert.ToInt32(vm.LeaveStructureId)).Name;
                }
                if (vm.GroupId == "0_0" || vm.GroupId == "0" || vm.GroupId == "" || vm.GroupId == "null" || vm.GroupId == null)
                {
                    vm.GroupId = "";
                }
                else
                {
                    GroupParam = new GroupRepo().SelectById(vm.GroupId).Name;
                }
                if (vm.GradeId == "0_0" || vm.GradeId == "0" || vm.GradeId == "" || vm.GradeId == "null" || vm.GradeId == null)
                {
                    vm.GradeId = "";
                    //GradeParam = new GradeRepo().SelectById(GradeId).Name;
                }
                else
                {

                }
                //TaxStructureId
                //PFStructureId
                //EDStructureId
                //LeaveStructureId
                //GroupId
                #endregion Value assign to Parameters

                #region Get Master Data
                //(cFields, cValues) c for condition
                if (vm.ReportName != "HRM13")
                {

                    if (vm.ReportName != "HRM14") //All Employee
                    {
                        string[] cFields = { "ei.Code>", "ei.Code<", "desig.Id", "d.Id", "s.Id", "p.Id"
                                               ,"ej.Other1","ej.Other2","ej.Other3"
                                               , "ej.JoinDate>", "ej.JoinDate<" 
                                               , "esg.TaxStructureId", "esg.PFStructureId", "esg.EDStructureId", "esg.LeaveStructureId"
                                               , "esg.GroupId", "g.Id", "empp.Gender_E", "empp.Religion" , "ei.IsActive"
                                           };
                        string[] cValues = { vm.CodeF, vm.CodeT, vm.DesignationId, vm.DepartmentId,vm.SectionId, vm.ProjectId
                                               ,vm.Other1, vm.Other2, vm.Other3
                                               , Ordinary.DateToString(vm.JoinDateFrom), Ordinary.DateToString(vm.JoinDateTo) 
                                               , vm.TaxStructureId, vm.PFStructureId, vm.EDStructureId, vm.LeaveStructureId
                                               , vm.GroupId, vm.GradeId, vm.Gender, vm.Religion, "1"
                                           };

                        masterDataDT = _empRepo.EmployeeNewReport(vm, cFields, cValues);
                    }
                    else if (vm.ReportName == "HRM14") //HardCode - Not Permanent/Probationary Employee
                    {
                        string[] cFields = { "ei.Code>", "ei.Code<", "desig.Id", "d.Id", "s.Id", "p.Id"
                                               ,"ej.Other1","ej.Other2","ej.Other3"
                                               , "ej.JoinDate>", "ej.JoinDate<" 
                                               , "esg.TaxStructureId", "esg.PFStructureId", "esg.EDStructureId", "esg.LeaveStructureId"
                                               , "esg.GroupId", "g.Id", "empp.Gender_E", "empp.Religion" , "ei.IsActive", "ej.IsPermanent"
                                           };
                        string[] cValues = { vm.CodeF, vm.CodeT, vm.DesignationId, vm.DepartmentId,vm.SectionId, vm.ProjectId
                                               ,vm.Other1, vm.Other2, vm.Other3
                                               , Ordinary.DateToString(vm.JoinDateFrom), Ordinary.DateToString(vm.JoinDateTo) 
                                               , vm.TaxStructureId, vm.PFStructureId, vm.EDStructureId, vm.LeaveStructureId
                                               , vm.GroupId, vm.GradeId, vm.Gender, vm.Religion, "1", "0"
                                           };

                        masterDataDT = _empRepo.EmployeeNewReport(vm, cFields, cValues);
                    }

                }
                else if (vm.ReportName == "HRM13") //HardCode  - Resign Employee
                {
                    string[] cFields = { "ei.Code>", "ei.Code<", "desig.Id", "d.Id", "s.Id", "p.Id"
                                               ,"ej.Other1","ej.Other2","ej.Other3"
                                               , "ej.JoinDate>", "ej.JoinDate<" 
                                               , "esg.TaxStructureId", "esg.PFStructureId", "esg.EDStructureId", "esg.LeaveStructureId"
                                               , "esg.GroupId", "g.Id", "empp.Gender_E", "empp.Religion" , "ei.IsActive"
                                           };
                    string[] cValues = { vm.CodeF, vm.CodeT, vm.DesignationId, vm.DepartmentId,vm.SectionId, vm.ProjectId
                                               ,vm.Other1, vm.Other2, vm.Other3
                                               , Ordinary.DateToString(vm.JoinDateFrom), Ordinary.DateToString(vm.JoinDateTo) 
                                               , vm.TaxStructureId, vm.PFStructureId, vm.EDStructureId, vm.LeaveStructureId
                                               , vm.GroupId, vm.GradeId, vm.Gender, vm.Religion, "0"
                                           };

                    masterDataDT = _empRepo.EmployeeNewReport(vm, cFields, cValues);

                }
                #endregion


                ds.Tables.Add(masterDataDT);
                ds.Tables[0].TableName = "dtEmployeeNewReport";
                List<string> EmployeeIdList = new List<string>();
                string EmployeeId = "";

                #region Get Detail Data

                if (vm.ReportName == "HRM15") //HardCode  -  EmployeeAsset
                {
                    #region Reading EmployeeId from MasterDT
                    foreach (DataRow dr in masterDataDT.Rows)
                    {
                        EmployeeId = dr["EmployeeId"].ToString();
                        EmployeeIdList.Add(EmployeeId);
                    }
                    #endregion

                    EmployeeAssetVM assetVM = new EmployeeAssetVM();
                    assetVM.EmployeeIdList = EmployeeIdList;
                    detailDataDT = new EmployeeAssetRepo().Report(assetVM);

                    #region Select MasterDT Again
                    DataTable newMasterDataDT = new DataTable();
                    {
                        if (detailDataDT != null && detailDataDT.Rows.Count > 0)
                        {

                            string MultipleEmployeeId = "";
                            foreach (DataRow dr in detailDataDT.Rows)
                            {
                                MultipleEmployeeId += "'" + dr["EmployeeId"].ToString() + "',";
                            }
                            MultipleEmployeeId = MultipleEmployeeId.Remove(MultipleEmployeeId.Length - 1);
                            newMasterDataDT = masterDataDT.Select("EmployeeId IN(" + MultipleEmployeeId + ")").CopyToDataTable();
                        }

                        ds.Tables.Remove(ds.Tables[0]);
                        ds.Tables.Add(newMasterDataDT);
                        ds.Tables[0].TableName = "dtEmployeeNewReport";
                    }
                    #endregion

                    ds.Tables.Add(detailDataDT);
                    ds.Tables[1].TableName = "dtEmployeeAsset";

                }
                else if (vm.ReportName == "HRM16") //HardCode - EmployeeTransfer
                {
                    #region Reading EmployeeId from MasterDT
                    foreach (DataRow dr in masterDataDT.Rows)
                    {
                        EmployeeId = dr["EmployeeId"].ToString();
                        EmployeeIdList.Add(EmployeeId);
                    }
                    #endregion
                    EmployeeTransferVM transferVM = new EmployeeTransferVM();
                    transferVM.EmployeeIdList = EmployeeIdList;
                    detailDataDT = new EmployeeTransferRepo().Report(transferVM);
                    #region Select MasterDT Again
                    DataTable newMasterDataDT = new DataTable();
                    {
                        string MultipleEmployeeId = "";
                        foreach (DataRow dr in detailDataDT.Rows)
                        {
                            MultipleEmployeeId += "'" + dr["EmployeeId"].ToString() + "',";
                        }
                        MultipleEmployeeId = MultipleEmployeeId.Remove(MultipleEmployeeId.Length - 1);
                        newMasterDataDT = masterDataDT.Select("EmployeeId IN(" + MultipleEmployeeId + ")").CopyToDataTable();

                        ds.Tables.Remove(ds.Tables[0]);
                        ds.Tables.Add(newMasterDataDT);
                        ds.Tables[0].TableName = "dtEmployeeNewReport";
                    }
                    #endregion

                    ds.Tables.Add(detailDataDT);
                    ds.Tables[1].TableName = "dtEmployeeTransfer";
                }
                else if (vm.ReportName == "HRM17") //HardCode - EmployeePromotion
                {
                    #region Reading EmployeeId from MasterDT
                    foreach (DataRow dr in masterDataDT.Rows)
                    {
                        EmployeeId = dr["EmployeeId"].ToString();
                        EmployeeIdList.Add(EmployeeId);
                    }
                    #endregion
                    EmployeePromotionVM promotionVM = new EmployeePromotionVM();
                    promotionVM.EmployeeIdList = EmployeeIdList;
                    detailDataDT = new EmployeePromotionRepo().Report(promotionVM);
                    #region Select MasterDT Again
                    DataTable newMasterDataDT = new DataTable();
                    {
                        string MultipleEmployeeId = "";
                        foreach (DataRow dr in detailDataDT.Rows)
                        {
                            MultipleEmployeeId += "'" + dr["EmployeeId"].ToString() + "',";
                        }
                        MultipleEmployeeId = MultipleEmployeeId.Remove(MultipleEmployeeId.Length - 1);
                        newMasterDataDT = masterDataDT.Select("EmployeeId IN(" + MultipleEmployeeId + ")").CopyToDataTable();

                        ds.Tables.Remove(ds.Tables[0]);
                        ds.Tables.Add(newMasterDataDT);
                        ds.Tables[0].TableName = "dtEmployeeNewReport";
                    }
                    #endregion
                    ds.Tables.Add(detailDataDT);
                    ds.Tables[1].TableName = "dtEmployeePromotion";
                }
                else if (vm.ReportName == "HRM18") //HardCode - EmployeeLeave
                {
                    EmployeeLeaveVM leaveVM = new EmployeeLeaveVM();
                    leaveVM.LeaveYear = vm.Year;
                    leaveVM.EmployeeIdList = EmployeeIdList;
                    {
                        leaveVM.IsRegular = true;
                        detailDataDT = new EmployeeLeaveRepo().Report(leaveVM);
                        ds.Tables.Add(detailDataDT);
                        ds.Tables[1].TableName = "dtEmployeeLeaveRegular";
                    }

                    {
                        leaveVM.IsRegular = false;
                        detailDataDT = new EmployeeLeaveRepo().Report(leaveVM);
                        ds.Tables.Add(detailDataDT);
                        ds.Tables[2].TableName = "dtEmployeeLeaveNotRegular";
                    }

                }

                else if (vm.ReportName == "Nomeniee") 
                {                   
                    detailDataDT = new EmployeeRepo().Nomeniee();
                    ds.Tables.Add(detailDataDT);
                    ds.Tables[1].TableName = "dtEmployeeNomeniee";   
                }
                else if (vm.ReportName == "Dependent") 
                {
                    detailDataDT = new EmployeeRepo().Dependent();
                    ds.Tables.Add(detailDataDT);
                    ds.Tables[1].TableName = "dtEmployeeDependent"; 
                }

                #endregion



            HRM100:

                #region EnumReport
                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

                string[] conFields = { "ReportType", "ReportId" };
                string[] conValues = { "HRM", vm.ReportName };
                enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);

                ReportFileName = enumReportVMs.FirstOrDefault().ReportFileName;
                ReportHeader = enumReportVMs.FirstOrDefault().Name;
            
                ReportHead = "There are no data to Preview for this " + ReportHeader;
                #endregion EnumReport
                if (masterDataDT.Rows.Count > 0)
                {
                    ReportHead = ReportHeader;
                }
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\NewReports\" + ReportFileName + ".rpt";


                if (vm.ReportName == "HRM100")
                {
                    masterDataDT = _empRepo.CombinedEmployeeSummaryReport(vm);
                    masterDataDT.TableName = "dtEmployeeSummaryReport";
                    ds.Tables.Add(masterDataDT);

                    //dt
                    if (masterDataDT.Rows.Count > 0)
                    {
                        ReportHead = ReportHeader;
                    }
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\NewReports\" + ReportFileName + ".rpt";

                }
                doc.Load(rptLocation);
                doc.SetDataSource(ds);

                string LogoName = "COMPANYLOGO";              
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\" + LogoName + ".png";
               // string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                if (vm.ReportName == "Nomeniee" || vm.ReportName == "Dependent") 
                {
                    doc.DataDefinition.FormulaFields["desigParam"].Text = vm.Designation;
                    doc.DataDefinition.FormulaFields["deptParam"].Text = vm.Department;
                    doc.DataDefinition.FormulaFields["secParam"].Text = vm.Section;
                    doc.DataDefinition.FormulaFields["codeTParam"].Text = vm.CodeT;
                    doc.DataDefinition.FormulaFields["codeFParam"].Text = vm.CodeF;
                    doc.DataDefinition.FormulaFields["projectParam"].Text = vm.Project;
                    doc.DataDefinition.FormulaFields["dtpFromParam"].Text = vm.DateFrom;
                    doc.DataDefinition.FormulaFields["dtpToParam"].Text = vm.DateTo;
                }

                if (vm.ReportName == "HRM100")
                {
                    FiscalYearDetailVM fydVM = new FiscalYearRepo().FYPeriodDetail(vm.FiscalYearDetailId).FirstOrDefault();

                    string PeriodName = fydVM.PeriodName;

                    var FullPeriodName = Convert.ToDateTime("01-" + PeriodName).ToString("MMMM-yyyy");
                    doc.DataDefinition.FormulaFields["FullPeriodName"].Text = "'" + FullPeriodName + "'";
                }
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        [Authorize]
        public ActionResult LeaveStatus(EmployeeInfoVM vm)
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_28", "report").ToString();


                if (string.IsNullOrWhiteSpace(vm.View))
                {
                    return View(vm);
                }

                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    vm = new EmployeeInfoVM();
                    vm.EmployeeId = identity.EmployeeId;
                    vm.CodeF = identity.EmployeeCode;
                    vm.CodeT = identity.EmployeeCode;
                }

                ReportDocument doc = new ReportDocument();
                DataSet ds = new DataSet();


                string rptLocation = "";
                string ReportHead = "";
                string ReportName = "Leave Status (Type Wise)";
                ReportHead = "There are no data to Preview for this Leave Status " + ReportName;

                if (!string.IsNullOrWhiteSpace(vm.MultipleCode))
                {
                    vm.CodeList = vm.MultipleCode.Split(',').ToList();
                }

                string[] conditionFields = { "e.Code>", "e.Code<", "e.DesignationId", "e.DepartmentId", "e.SectionId", "e.ProjectId", "e.JoinDate>", "e.JoinDate<" };
                string[] conditionValues = { vm.CodeF, vm.CodeT, vm.DesignationId, vm.DepartmentId, vm.SectionId, vm.ProjectId, Ordinary.DateToString(vm.JoinDateFrom), Ordinary.DateToString(vm.JoinDateTo) };


                DataTable dt = _repo.EmployeeListLetter(vm, conditionFields, conditionValues);

                ds.Tables.Add(dt);
                ds.Tables[0].TableName = "dtEmployeeListLetter";

                if (dt.Rows.Count > 0)
                {
                    ReportHead = ReportName;
                }
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\" + "rptEmployeeLeaveStatus_TypeWise.rpt";


                doc.Load(rptLocation);
                doc.SetDataSource(ds);

                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["IssueDate"].Text = "'" + vm.IssueDate + "'";
                doc.DataDefinition.FormulaFields["ReferenceNumber"].Text = "'" + vm.ReferenceNumber + "'";
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
        private FileStreamResult RenderReportAsEXCEL(ReportDocument rptDoc, String ReportName)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ReportName + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xls");
        }
        private FileStreamResult RenderReportAsWord(ReportDocument rptDoc, String ReportName)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);
            return File(stream, "application/octet-stream", ReportName + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".doc");
        }
        #endregion RenderReportAsPDF

        [Authorize]
        public ActionResult Export(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string ReportNo
            , string leaveyear, string LeaveType, string Gender_E, string Religion, string GradeId,string EmpCategory,string EmpJobType, string RT = "")
        {

            string[] result = new string[6];
            DataTable dt = new DataTable();
            try
            {

                
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                string Employee = "N";
                string EmployeeId = "";
                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    EmployeeId = identity.EmployeeId;
                    Employee = "Y";
                    CodeF = identity.EmployeeCode;
                    CodeT = identity.EmployeeCode;
                    //Name = "";
                    ProjectId = "";
                    DepartmentId = "";
                    dtpFrom = "";
                    dtpTo = "";
                }
                ViewBag.Employee = Employee;
                ViewBag.RT = RT;
                if (string.IsNullOrWhiteSpace(ReportNo))
                {
                    return View();
                }
                #region Value assign to Parameters
                string vProjectId = "0_0", vDepartmentId = "0_0", vSectionId = "0_0", vDesignationId = "0_0"
               , vCodeF = "0_0", vCodeT = "0_0", vdtpFrom = "0_0", vdtpTo = "0_0";
                string projectParam = "[All]", deptParam = "[All]", secParam = "[All]", desigParam = "[All]"
               , codeFParam = "[All]", codeTParam = "[All]", dtpFromParam = "[All]", dtpToParam = "[All]";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null && ProjectId != "undefined")
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null && DepartmentId != "undefined")
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null && SectionId != "undefined")
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null && DesignationId != "undefined")
                {
                    vDesignationId = DesignationId;
                    DesignationRepo desRepo = new DesignationRepo();
                    desigParam = desRepo.SelectById(DesignationId).Name;
                }
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null && CodeF != "undefined")
                {
                    vCodeF = CodeF;
                    codeFParam = vCodeF;
                }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null && CodeT != "undefined")
                {
                    vCodeT = CodeT;
                    codeTParam = vCodeT;
                }
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null && dtpFrom != "undefined")
                {
                    vdtpFrom = dtpFrom;
                    dtpFromParam = vdtpFrom;
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null && dtpTo != "undefined")
                {
                    vdtpTo = dtpTo;
                    dtpToParam = vdtpTo;
                }
                #endregion Value assign to Parameters
                #region ReportType
               
               if (RT == "LR")
                {
                    string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                    EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
                    if (CompanyName.ToLower() != "g4s")
                    {
                        var getAllData = _empRepo.EmployeeLeaveRegister(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                     , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId);
                         dt = Ordinary.ListToDataTable(getAllData);
                     
                    }
                    else
                    {
                         dt = _empRepo.EmployeeLeaveRegisterDownload(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId
                     , vdtpFrom, vdtpTo, leaveyear, LeaveType, EmployeeId, EmpCategory, EmpJobType);
                       
                    }

                }
   

               ExcelPackage excel = new ExcelPackage();
               var workSheet = excel.Workbook.Worksheets.Add("EmployeeLeaveRegister");
               workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

               string filename = "EmployeeLeaveRegister" + "-" + leaveyear  +DateTime.Now.ToString("yyyyMMdd");
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


                 #region Redirect

               result[0] = "Success";
               result[1] = "Successful~Data Download";

               Session["result"] = result[0] + "~" + result[1];
               return Redirect("Index");

               #endregion
               
            }
            catch (Exception e)
            {

                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(
                    result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine +
                    result[5].ToString(), this.GetType().Name,
                    result[4].ToString() + Environment.NewLine + result[3].ToString());
                return Redirect("Index");
            }
        }

        //public void EmailSalaryCertificateTIB(DataSet ds, ReportDocument rptDoc, ReportDocument rptDoc1, string IncomeYearstart, string AssesMentYear)
        //{
        //    EmailSettings ems = new EmailSettings();
        //    SettingRepo _setDAL = new SettingRepo();
        //    DataTable dt1 = new DataTable();
        //    string strSort = "Code";

        //    DataSet DsTemp = new DataSet();
        //    //string strSort = " Code";

        //    //dt.Tables[0].TableName = "dtTAX108";

        //    foreach (DataRow item in ds.Tables[0].Rows)
        //    {
        //        try
        //        {

        //            dt1 = new DataTable();

        //            DataRow[] DataRow1 = ds.Tables[0].Select("Code='" + item["Code"].ToString() + "'");
        //           // DataRow[] DataRow2 = ds.Tables[1].Select("employeeid='" + item["employeeid"].ToString() + "'");

        //            DataView dtview1 = new DataView(DataRow1.CopyToDataTable());
        //            dtview1.Sort = strSort;
        //            dt1 = dtview1.ToTable();

        //           // DataTable dtyearlyTax = DataRow2.CopyToDataTable();

        //            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
        //                                                              SecurityProtocolType.Tls11 |
        //                                                              SecurityProtocolType.Tls;

        //            dt1.TableName = "dtTIB_HRMSalary";
        //           // dtyearlyTax.TableName = "dtYearlyTAX";
        //            DsTemp = new DataSet();
        //            DsTemp.Tables.Add(dt1);
        //           // DsTemp.Tables.Add(dtyearlyTax);

        //            //"Alamgir.Hossain@symphonysoftt.com";

        //            //ems.MailToAddress = dt1.Rows[0]["Email"].ToString();

        //            if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
        //            {
        //                //dt1.TableName = "dtSalarySheet";

        //                ems.MailHeader = "Salary Certificate for the Income year " + IncomeYearstart + ".";
        //                ems.MailBody = "Dear " + item["EmpName"].ToString() + "," + System.Environment.NewLine + "This is Salary Certificate for the Income year " + IncomeYearstart + " and Assessment year " + AssesMentYear + ". \n\nKind regards, \nArifa Begum \nDeputy Coordinator - Finance & Accounts";
        //                ems.FileName = "Salary Certificate of " + item["EmpName"].ToString() + " " + IncomeYearstart;
        //                rptDoc.SetDataSource(DsTemp);

        //                DsTemp = DsTemp.Copy();

        //                var dtResult = GetGreaterThanZero(DsTemp.Tables[0].Copy());

        //                if (dtResult.Rows.Count > 0)
        //                {
        //                    DsTemp = new DataSet();

        //                    DsTemp.Tables.Add(dtResult);
        //                   // DsTemp.Tables.Add(dtyearlyTax.Copy());

        //                    DsTemp.Tables[0].TableName = "dtTAX108";
        //                    DsTemp.Tables[1].TableName = "dtYearlyTAX";
        //                    rptDoc1.SetDataSource(DsTemp);
        //                }



        //                using (var smpt = new SmtpClient())
        //                {
        //                    smpt.EnableSsl = ems.USsel;
        //                    smpt.Host = ems.ServerName;
        //                    smpt.Port = ems.Port;
        //                    smpt.UseDefaultCredentials = false;
        //                    smpt.EnableSsl = true;
        //                    smpt.Credentials = new NetworkCredential(ems.UserName, ems.Password);
        //                    MailMessage mailmessage = new MailMessage(
        //                        ems.MailFromAddress,
        //                        ems.MailToAddress,
        //                        ems.MailHeader,
        //                        ems.MailBody);
        //                    mailmessage.Attachments.Add(new Attachment(
        //                        rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat),
        //                        ems.FileName + ".pdf"));


        //                    if (dtResult.Rows.Count > 0)
        //                    {

        //                        mailmessage.Attachments.Add(new Attachment(
        //                            rptDoc1.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat),
        //                            "Yearly Benifit From TIB" + item["EmpName"].ToString() + ".pdf"));
        //                    }

        //                    smpt.Send(mailmessage);
        //                    mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

        //                    FileLogger.Log("EmailSalaryCertificateTIB", this.GetType().Name, "EmpEmail Send To:" + item["EmpName"].ToString() + Environment.NewLine + "EmpEmailAddress:" + dt1.Rows[0]["Email"].ToString());

        //                }

        //                Thread.Sleep(3000);
        //            }
        //        }
        //        catch (SmtpFailedRecipientException ex)
        //        {
        //            FileLogger.Log("EmailSalaryCertificateTIB", this.GetType().Name, "EmpEmail Not Send To:" + item["EmpName"].ToString() + " " + ex.Message + Environment.NewLine + ex.StackTrace);

        //        }
        //    }

        //    rptDoc.Close();
        //    thread.Abort();
        //}

        public void EmailSalaryCertificateTIB(DataSet dt, ReportDocument rptDoc, ReportDocument rptDoc1, string IncomeYearstart, string AssesMentYear)
        {
            EmailSettings ems = new EmailSettings();
            SettingRepo _setDAL = new SettingRepo();
            DataTable dt1 = new DataTable();
            string strSort = "Code";

            DataSet DsTemp = new DataSet();
            //string strSort = " Code";

            //dt.Tables[0].TableName = "dtTAX108";

            foreach (DataRow item in dt.Tables[0].Rows)
            {
                try
                {

                    dt1 = new DataTable();

                    DataRow[] DataRow1 = dt.Tables[0].Select("Code='" + item["Code"].ToString() + "'");
                    DataRow[] DataRow2 = dt.Tables[1].Select("employeeid='" + item["employeeid"].ToString() + "'");

                    DataView dtview1 = new DataView(DataRow1.CopyToDataTable());
                    dtview1.Sort = strSort;
                    dt1 = dtview1.ToTable();

                    DataTable dtyearlyTax = DataRow2.CopyToDataTable();

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                                                                      SecurityProtocolType.Tls11 |
                                                                      SecurityProtocolType.Tls;

                    dt1.TableName = "dtTIB_HRMSalary";
                    dtyearlyTax.TableName = "dtYearlyTAX";
                    DsTemp = new DataSet();
                    DsTemp.Tables.Add(dt1);
                    DsTemp.Tables.Add(dtyearlyTax);

                    //"Alamgir.Hossain@symphonysoftt.com";

                    ems.MailToAddress = dt1.Rows[0]["Email"].ToString();

                    if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
                    {
                        //dt1.TableName = "dtSalarySheet";

                        ems.MailHeader = "Salary Certificate for the Income year " + IncomeYearstart + ".";
                        ems.MailBody = "Dear " + item["EmpName"].ToString() + "," + System.Environment.NewLine + "This is Salary Certificate for the Income year " + IncomeYearstart + " and Assessment year " + AssesMentYear + ". \n\nKind regards, \nArifa Begum \nDeputy Coordinator - Finance & Accounts";
                        ems.FileName = "Salary Certificate of " + item["EmpName"].ToString() + " " + IncomeYearstart;
                        rptDoc.SetDataSource(DsTemp);

                        DsTemp = DsTemp.Copy();

                        var dtResult = GetGreaterThanZero(DsTemp.Tables[0].Copy());

                        if (dtResult.Rows.Count > 0)
                        {
                            DsTemp = new DataSet();

                            DsTemp.Tables.Add(dtResult);
                            DsTemp.Tables.Add(dtyearlyTax.Copy());

                            DsTemp.Tables[0].TableName = "dtTAX108";
                            DsTemp.Tables[1].TableName = "dtYearlyTAX";
                            rptDoc1.SetDataSource(DsTemp);
                        }



                        using (var smpt = new SmtpClient())
                        {
                            smpt.EnableSsl = ems.USsel;
                            smpt.Host = ems.ServerName;
                            smpt.Port = ems.Port;
                            smpt.UseDefaultCredentials = false;
                            smpt.EnableSsl = true;
                            smpt.Credentials = new NetworkCredential(ems.UserName, ems.Password);
                            MailMessage mailmessage = new MailMessage(
                                ems.MailFromAddress,
                                ems.MailToAddress,
                                ems.MailHeader,
                                ems.MailBody);
                            mailmessage.Attachments.Add(new Attachment(
                                rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat),
                                ems.FileName + ".pdf"));


                            if (dtResult.Rows.Count > 0)
                            {

                                mailmessage.Attachments.Add(new Attachment(
                                    rptDoc1.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat),
                                    "Yearly Benifit From TIB" + item["EmpName"].ToString() + ".pdf"));
                            }

                            smpt.Send(mailmessage);
                            mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                            FileLogger.Log("EmailSalaryCertificateTIB", this.GetType().Name, "EmpEmail Send To:" + item["EmpName"].ToString() + Environment.NewLine + "EmpEmailAddress:" + dt1.Rows[0]["Email"].ToString());

                        }

                        Thread.Sleep(3000);
                    }
                }
                catch (SmtpFailedRecipientException ex)
                {
                    FileLogger.Log("EmailSalaryCertificateTIB", this.GetType().Name, "EmpEmail Not Send To:" + item["EmpName"].ToString() + " " + ex.Message + Environment.NewLine + ex.StackTrace);

                }
            }

            rptDoc.Close();
            thread.Abort();
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

      
        [Authorize]
        public ActionResult MonthlyAttendanceDownload(EmployeeInfoVM vm)
        {

            string[] result = new string[6];

            try
            {
               
                DataTable table = new DataTable();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                if (!(identity.IsAdmin || identity.IsHRM))
                {
                    ////////vm = new SalarySheetVM();
                    vm.EmployeeId = identity.EmployeeId;
                    vm.CodeF = identity.EmployeeCode;
                    vm.CodeT = identity.EmployeeCode;
                }

                string[] conFields = { "ve.Code>", "ve.Code<", "ve.DepartmentId", "ve.ProjectId", "ve.SectionId", "attn.DailyDate>", "attn.DailyDate<","EmpCategory", "EmpJobType" };
                string[] conValues = { vm.CodeF, vm.CodeT, vm.DepartmentId, vm.ProjectId, vm.SectionId, Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo),vm.EmpCategory,vm.EmpJobType };
                DailyAttendanceProcessRepo _repo = new DailyAttendanceProcessRepo();

                table = _repo.Report(vm, conFields, conValues);
               
               
                var toRemove = new string[] { "OtherId","JoinDate", "PolicyName", "GroupName", "EmployeeId",  "ProxyID", "AttendanceMigrationId", "AttendanceStructureId", "GroupId", "DailyDateReportGroup","PunchNextDay","IsManual","IsDeductEarlyOut","EarlyOutMin","IsDeductLateIn"
                                ,"LateInMin" ,"LunchBreak" ,"WorkingHrsBy" ,"TotalOT" ,"TotalOTBy" ,"DayStatus"
                                , "EarlyDeduct", "LateDeduct", "TiffinAllow", "DinnerAllow", "IfterAllow" ,"TiffinAmnt","IfterAmnt","DinnerAmnt","DeductTiffTime","DeductIfterTime","DeductDinTime","GrossAmnt","BasicAmnt","Remarks","MovementEarlyOutMin","MovementLateInMin","DepartmentOrder"
                                ,"SectionOrder","ProjectOrder","PInTime","POutTime","PunchInTime","PunchOutTime","PunchOutTimeNextday","InTimeBy","OutTimeBy","Project"
                            };

                foreach (string col in toRemove)
                {
                    table.Columns.Remove(col);
                }

                List<string> oldColumnNames = new List<string> { "EmpName", "EmpJobType", "DailyDate", "WorkingHrs", "AttnStatus", "Section" };
                List<string> newColumnNames = new List<string> { "Employee Name", "Employee Job Type", "Month Date", "Total Hours", "Status", "Posting Branch" };

                table = Ordinary.DtColumnNameChangeList(table, oldColumnNames, newColumnNames);

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("EmployeeMonthlyAttendance");
                workSheet.Cells[1, 1].LoadFromDataTable(table, true);
                string filename = "EmployeeMonthlyAttendance" + "-" + DateTime.Now.ToString("yyyyMMdd");

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #region Redirect

                result[0] = "Success";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];
                return Redirect("ManualRosterAttendanceReport");

                #endregion
               

            }
            catch (Exception e)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(
                    result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine +
                    result[5].ToString(), this.GetType().Name,
                    result[4].ToString() + Environment.NewLine + result[3].ToString());
                return Redirect("ManualRosterAttendanceReport");
            }
        }

        [HttpGet]
        public ActionResult EmpNominee(string RT = null)
        {            
            return View();
        }
              
        public class EmailSettings
        {
            public string MailToAddress { get; set; }
            public string MailFromAddress = "payrolltib@ti-bangladesh.org";
            public bool USsel = true;
            public string Password = "L%xcgCOId37%k$&xN&EeS";
            public string UserName = "payrolltib@ti-bangladesh.org";

            public string ServerName = "smtp.office365.com";
            public string MailBody { get; set; }
            public string MailHeader { get; set; }
            public string Fiscalyear { get; set; }
            public int Port = 587;
            public HttpPostedFileBase fileUploader { get; set; }
            public string FileName { get; set; }
        }



    }
}
