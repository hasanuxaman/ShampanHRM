using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.Enum;
using SymViewModel.Common;
using SymViewModel.Enum;
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
    public class PreEmployementInformationController : Controller
    {
        //
        // GET: /Common/PreEmployementInformation/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        PreEmployementInformationRepo _repo = new PreEmployementInformationRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id 
            //01     //ReferenceNumber
            //02     //IssueDate
            //03     //EmployeeName
            //04     //Designation  
            //05     //Department
            //06     //BasicSalary   
            //07     //GrossSalary 


            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<PreEmployementInformationVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.ReferenceNumber.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.IssueDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.EmployeeName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())

                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PreEmployementInformationVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.ReferenceNumber :
                sortColumnIndex == 2 && isSortable_2 ? c.IssueDate :
                sortColumnIndex == 3 && isSortable_3 ? c.EmployeeName :
                sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                sortColumnIndex == 5 && isSortable_5 ? c.Department :
                sortColumnIndex == 6 && isSortable_6 ? c.BasicSalary.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.GrossSalary.ToString() :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                Convert.ToString(c.Id)
               ,  c.ReferenceNumber      
               ,  c.IssueDate      
               ,  c.EmployeeName      
               ,  c.Designation        
               ,  c.Department      
               ,  c.BasicSalary.ToString()
               ,  c.GrossSalary.ToString()
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            PreEmployementInformationVM vm = new PreEmployementInformationVM();
            vm.Operation = "add";
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(PreEmployementInformationVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2] });
                }
                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("Create", vm);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            PreEmployementInformationVM vm = new PreEmployementInformationVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            PreEmployementInformationVM vm = new PreEmployementInformationVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "report").ToString();
            PreEmployementInformationVM vm = new PreEmployementInformationVM();
            return View("Report", vm);
        }
        [HttpGet]
        public ActionResult ReportViewBackup(string referenceNumber, string issueDate, string employeeName, string shortName
            , string designation, string department, string jobGrade, string jobGradeDesignation, string letterName)
        {
            return null;
            //try
            //{
            //    string ReportHead = "";
            //    string rptLocation = "";
            //    ReportDocument doc = new ReportDocument();
            //    DataTable table = new DataTable();
            //    DataSet ds = new DataSet();
            //    ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            //    PreEmployementInformationVM vm = new PreEmployementInformationVM();

            //    #region Parameters
            //    //if (codeFrom == "0_0" || codeFrom == "0" || codeFrom == "" || codeFrom == "null" || codeFrom == null)
            //    //{
            //    //    codeFrom = "";
            //    //}
            //    //if (codeTo == "0_0" || codeTo == "0" || codeTo == "" || codeTo == "null" || codeTo == null)
            //    //{
            //    //    codeTo = "";
            //    //}

            //    //if (projectId == "0_0" || projectId == "0" || projectId == "" || projectId == "null" || projectId == null)
            //    //{
            //    //    projectId = "";
            //    //}
            //    //if (departmentId == "0_0" || departmentId == "0" || departmentId == "" || departmentId == "null" || departmentId == null)
            //    //{
            //    //    departmentId = "";
            //    //}
            //    //if (sectionId == "0_0" || sectionId == "0" || sectionId == "" || sectionId == "null" || sectionId == null)
            //    //{
            //    //    sectionId = "";
            //    //}
            //    #endregion Parameters

            //    string[] conFields = { "pr.ReferenceNumber", "pr.IssueDate", "pr.EmployeeName", "pr.ShortName", "pr.Designation", "pr.Department", "pr.JobGrade", "pr.JobGradeDesignation" };
            //    string[] conValues = { referenceNumber, Ordinary.DateToString(issueDate), employeeName, shortName, designation, department, jobGrade, jobGradeDesignation };
            //    PreEmployementInformationRepo _repo = new PreEmployementInformationRepo();
            //    table = _repo.Report(vm, conFields, conValues);
            //    ReportHead = "There are no data to Preview for Pre Employement Information";
            //    if (table.Rows.Count > 0)
            //    {
            //        ReportHead = "Pre Employement Information";
            //    }
            //    ds.Tables.Add(table);
            //    ds.Tables[0].TableName = "dtPreEmployementInformation";

            //    #region EnumReport
            //    EnumReportRepo _reportRepo = new EnumReportRepo();
            //    List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

            //    string[] conFieldsEnum = { "ReportType" };
            //    string[] conValuesEnum = { "PreEmployLetter" };
            //    enumReportVMs = _reportRepo.SelectAll(0, conFieldsEnum, conValuesEnum);


            //    //"Appointment Letter", "Transfer Letter", "Promotion Letter", "Increment Letter"
            //    string ReportFileName = enumReportVMs.Where(c => c.ReportId == letterName).FirstOrDefault().ReportFileName;
            //    string ReportName = enumReportVMs.Where(c => c.ReportId == letterName).FirstOrDefault().Name;
            //    ReportHead = "There are no data to Preview for this " + ReportName;
            //    #endregion EnumReport
            //    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Common\Letters\" + ReportFileName + ".rpt";


            //    doc.Load(rptLocation);
            //    doc.SetDataSource(ds);
            //    string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
            //    doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
            //    doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";

            //    var rpt = RenderReportAsPDF(doc);
            //    doc.Close();
            //    return rpt;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public ActionResult ReportView(string ids, string letterName)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                PreEmployementInformationVM vm = new PreEmployementInformationVM();
                #region Parameters
                #endregion Parameters
                //vm.Id = Convert.ToInt32(Id);
                //string[] conFields = { "pr.ReferenceNumber", "pr.IssueDate", "pr.EmployeeName", "pr.ShortName", "pr.Designation", "pr.Department", "pr.JobGrade", "pr.JobGradeDesignation" };
                //string[] conValues = { referenceNumber, Ordinary.DateToString(issueDate), employeeName, shortName, designation, department, jobGrade, jobGradeDesignation };
                PreEmployementInformationRepo _repo = new PreEmployementInformationRepo();
                ids = ids.Substring(0, ids.Length - 1);
                string[] a = ids.Split('~');

                table = _repo.Report(vm, a);

                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtPreEmployementInformation";

                #region EnumReport
                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

                string[] conFieldsEnum = { "ReportType" };
                string[] conValuesEnum = { "PreEmployLetter" };
                enumReportVMs = _reportRepo.SelectAll(0, conFieldsEnum, conValuesEnum);


                //"Appointment Letter", "Transfer Letter", "Promotion Letter", "Increment Letter"
                string ReportFileName = enumReportVMs.Where(c => c.ReportId == letterName).FirstOrDefault().ReportFileName;
                string ReportName = enumReportVMs.Where(c => c.ReportId == letterName).FirstOrDefault().Name;
                ReportHead = "There are no data to Preview for this " + ReportName;
                if (table.Rows.Count > 0)
                {
                    ReportHead = ReportName;
                }
                #endregion EnumReport
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Common\Letters\" + ReportFileName + ".rpt";


                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";

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

        public JsonResult SelectEmployeeDetails(string refNo)
        {

            PreEmployementInformationVM vm = new PreEmployementInformationVM();
            string[] conditionFields = { "pr.ReferenceNumber" };
            string[] conditionValues = { refNo };
            vm = _repo.SelectAll(0, conditionFields, conditionValues).FirstOrDefault();
            return Json(vm, JsonRequestBehavior.AllowGet);
        }
    }
}
