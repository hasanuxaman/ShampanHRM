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
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class EmployeeSalaryIncrementController : Controller
    {
        //
        // GET: /Payroll/EmployeeSalaryIncrement/
        #region Declare
        EmployeeInfoRepo _emprepo = new EmployeeInfoRepo();
        EmployeeSalaryIncrementRepo _empincreament = new EmployeeSalaryIncrementRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        #endregion Declare
        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_40", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }

        public ActionResult _incrementIndex(EmployeeInfoVM vm)
        {
            //vm.ProjectId = ProjectId;
            //vm.DepartmentId = DepartmentId;
            //vm.SectionId = SectionId;
            //vm.DesignationId = DesignationId;
            //vm.CodeF = CodeF;
            //vm.CodeT = CodeT;
            //vm.Amount = Amount;
            //vm.GB = GB;
            //vm.FR = FR;


            return PartialView("_incrementIndex", vm);
        }


        
        ////////////public ActionResult _ceilingDetailIndex(string branchId, string glFiscalYearId, string Operation)
        ////////////{
        ////////////    #region Declare Variable
        ////////////    GLCeilingDetailVM vm = new GLCeilingDetailVM();
        ////////////    #endregion Declare Variable
        ////////////    List<GLCeilingDetailVM> VMs = new List<GLCeilingDetailVM>();

        ////////////    VMs = _detailRepo.SelectAllCeilingDetail(Convert.ToInt32(branchId), Convert.ToInt32(glFiscalYearId));
        ////////////    if (VMs != null)
        ////////////    {
        ////////////        VMs.FirstOrDefault().BranchId = Convert.ToInt32(branchId);
        ////////////        VMs.FirstOrDefault().GLFiscalYearId = Convert.ToInt32(glFiscalYearId);
        ////////////        VMs.FirstOrDefault().Operation = Operation;

        ////////////    }
        ////////////    return PartialView("_ceilingDetailIndex", VMs);
        ////////////}






        public ActionResult _rptIndexPartialBackup(string ProjectId, string DepartmentId, string SectionId, string DesignationId
        , string CodeF, string CodeT, bool GB, bool FR, decimal Amount)
        {
            EmployeeInfoVM vm = new EmployeeInfoVM();
            vm.ProjectId = ProjectId;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.DesignationId = DesignationId;
            vm.CodeF = CodeF;
            vm.CodeT = CodeT;
            vm.Amount = Amount;
            vm.GB = GB;
            vm.FR = FR;
            return PartialView("_rptIndex", vm);
        }




        [Authorize]
        public ActionResult _rptIndex(JQueryDataTableParamVM param, string ProjectId, string DepartmentId, string SectionId, string DesignationId
           , string CodeF, string CodeT, bool GB, bool FR, decimal Amount)
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
            var getAllData = _emprepo.EmployeeList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId);
            IEnumerable<EmployeeInfoVM> filteredData;
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
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                filteredData = getAllData
                   .Where(c =>
                                  isSearchable1 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable8 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                                                           sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Department :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Section :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Project :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.BasicSalary.ToString() :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.GrossSalary.ToString() :
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
                               c.Id
                             , c.Code
                             , c.FullName
                             , c.Designation
                             , c.Department
                             , c.Section
                             , c.Project
                             , c.BasicSalary.ToString()
                             , c.GrossSalary.ToString()
                             ,Convert.ToString(GB ? (FR ? c.GrossSalary*Amount/100 :c.GrossSalary+Amount  )  : (FR ? c.BasicSalary*Amount/100 :c.BasicSalary+Amount ))
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult insertIncreament(string Ids, string ProjectId, string DepartmentId, string SectionId, string DesignationId
           , string CodeF, string CodeT, string GB, string FR, decimal Amount, string IncreamentDate)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_40", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            List<string> empids = Ids.Split('~').ToList();
            string[] result = new string[6];
            var LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            var LastUpdateBy = identity.Name;
            var LastUpdateFrom = identity.WorkStationIP;
            result = _empincreament.InsertIncreament(empids, Convert.ToInt32(identity.BranchId), IncreamentDate, ProjectId, DepartmentId, SectionId, DesignationId
           , CodeF, CodeT, GB, FR, Amount, LastUpdateAt, LastUpdateBy, LastUpdateFrom);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmployeeIncrement(EmployeeInfoVM vm)

        {
            string[] result = new string[6];
            try
            {
                #region Try

                #region Objects and Variables

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                if (string.IsNullOrWhiteSpace(vm.View) || vm.View == "Y")
                {
                    return View(vm);
                }

                ReportDocument doc = new ReportDocument();
                SalaryProcessRepo _empRepo = new SalaryProcessRepo();
                DataTable dt = new DataTable();
                 dt = _empincreament.EmployeeIncrement(vm);

                #endregion

                #region Report Call

                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory +
                              @"Files\ReportFiles\Payroll\Salary\RptEmployeeIncrement.rpt";

                //rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" +ReportFileName + ".rpt";

                doc.Load(rptLocation);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = doc.DataDefinition.FormulaFields;
                CommonWebMethod _CommonWebMethod = new CommonWebMethod();

                #endregion

                doc.SetDataSource(dt);

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;

                #endregion Try
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = "Process Fail";
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log("EmployeeIncrement", this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);

                return View(vm);
            }
        }

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            try
            {
                Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(stream, "application/PDF");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}