using CrystalDecisions.CrystalReports.Engine;
using OfficeOpenXml;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymRepository.Payroll;
using SymRepository.Tax;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using SymViewModel.Tax;
//using SymWebUI.Areas.Payroll.Reports;
//using SymWebUI.Areas.Payroll.Reports.PayrollProcess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class SalaryTaxController : Controller
    {
        //
        // GET: /Payroll/EmployeeTax/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        public ActionResult _SalaryTax(JQueryDataTableParamVM param, string code, string name)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryTaxRepo repo = new SalaryTaxRepo();
            var getAllData = repo.GetPeriodNameDistrinct();
            IEnumerable<SalaryTaxVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.PeriodName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Remarks.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryTaxVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.PeriodStart :
                sortColumnIndex == 1 && isSortable_2 ? c.Remarks :
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
                , c.PeriodName 
                , c.Remarks 
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
        public ActionResult Create()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(EmployeeInfoVM vm)
        {
            SalaryTaxRepo strepo = new SalaryTaxRepo();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            SalaryTaxDetailVM salaryTaxDetail = new SalaryTaxDetailVM();
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            salaryTaxDetail = vm.salaryTaxDetail;
            salaryTaxDetail.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            salaryTaxDetail.CreatedBy = Identity.Name;
            salaryTaxDetail.CreatedFrom = Identity.WorkStationIP;
            result = strepo.SalaryTaxSingleAddorUpdate(salaryTaxDetail, Convert.ToInt32(Identity.BranchId));
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            //return View();
        }
        public ActionResult Edit(int fid)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            SalaryTaxRepo repo = new SalaryTaxRepo();
            var vm = repo.SelectAll(Convert.ToInt32(identity.BranchId), fid).FirstOrDefault();
            if (vm != null)
            {
                ViewBag.periodName = vm.PeriodName;
            }
            ViewBag.Id = fid;
            return View();
        }
        public ActionResult _salaryTaxDetails(JQueryDataTableParamVM param, int fid)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var CodeFilter = Convert.ToString(Request["sSearch_1"]);
            var EmployeeNameFilter = Convert.ToString(Request["sSearch_2"]);
            var BasicSalaryFilter = Convert.ToString(Request["sSearch_3"]);
            var GrossSalaryFilter = Convert.ToString(Request["sSearch_4"]);
            var TaxValueFilter = Convert.ToString(Request["sSearch_5"]);
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
            var TaxamountFrom = 0;
            var TaxamountTo = 0;
            if (TaxValueFilter.Contains('~'))
            {
                TaxamountFrom = TaxValueFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(TaxValueFilter.Split('~')[0]) == true ? Convert.ToInt32(TaxValueFilter.Split('~')[0]) : 0;
                TaxamountTo = TaxValueFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(TaxValueFilter.Split('~')[0]) == true ? Convert.ToInt32(TaxValueFilter.Split('~')[0]) : 0;
            }
            #endregion
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryTaxRepo repo = new SalaryTaxRepo();
            var getAllData = repo.SelectAllSalaryTaxDetails(fid);
            IEnumerable<SalaryTaxDetailVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData.Where(c =>
                    isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    ||
                    isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                    ||
                    isSearchable3 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                    ||
                    isSearchable4 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                    ||
                    isSearchable5 && c.TaxValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (CodeFilter != "" || EmployeeNameFilter != "" || (BasicSalaryFilter != "" && BasicSalaryFilter != "~") || (GrossSalaryFilter != "" && GrossSalaryFilter != "~") || (TaxValueFilter != "" && TaxValueFilter != "~"))
            {
                filteredData = filteredData.Where(c =>
                    (CodeFilter == "" || c.Code.ToLower().Contains(CodeFilter.ToLower()))
                    &&
                    (EmployeeNameFilter == "" || c.EmpName.ToLower().Contains(EmployeeNameFilter.ToLower()))
                    &&
                    (BasicamountFrom == 0 || BasicamountFrom <= Convert.ToInt32(c.BasicSalary))
                    &&
                    (BasicamountTo == 0 || BasicamountTo >= Convert.ToInt32(c.BasicSalary))
                    &&
                    (GrossamountFrom == 0 || GrossamountFrom <= Convert.ToInt32(c.GrossSalary))
                    &&
                    (GrossamountTo == 0 || GrossamountTo >= Convert.ToInt32(c.GrossSalary))
                    &&
                    (TaxamountFrom == 0 || TaxamountFrom <= Convert.ToInt32(c.TaxValue))
                    &&
                    (TaxamountTo == 0 || TaxamountTo >= Convert.ToInt32(c.TaxValue))
                    );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryTaxDetailVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.BasicSalary.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.GrossSalary.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.TaxValue.ToString() :
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
                             , c.BasicSalary.ToString()
                             , c.GrossSalary.ToString()
                             , c.TaxValue.ToString()
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
        public ActionResult SalaryTaxDetailsDelete(string ids)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            SalaryTaxRepo repo = new SalaryTaxRepo();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = repo.SalaryTaxDetailsDelete(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        public ActionResult SalaryTaxDelete(string ids)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            SalaryTaxRepo repo = new SalaryTaxRepo();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = repo.SalaryTaxDelete(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult SalaryTaxSingle()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        [HttpPost]
        public JsonResult SalaryTaxSingle(string FiscalPeriodDetailsId, string empID)
        {
            SalaryTaxDetailVM vm = new SalaryTaxDetailVM();
            SalaryTaxRepo repo = new SalaryTaxRepo();
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = Identity.Name;
            vm.CreatedFrom = Identity.WorkStationIP;
            vm.FiscalYearDetailId = FiscalPeriodDetailsId;
            vm.EmployeeId = empID;
            string[] getAllData = repo.SalaryTaxSingleAddorUpdate(vm, Convert.ToInt32(Identity.BranchId));
            ViewBag.mgs = getAllData[0];
            return Json(getAllData, JsonRequestBehavior.AllowGet);
            //return JavaScript(string.Format("ShowResult('{0}','{1}','{2}','{3}')", getAllData[0], getAllData[1], dataAction, "/Payroll/EmployeeBonus"));
        }
        [HttpGet]
        public ActionResult SingleTaxEdit(int taxDetailsId)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            SalaryTaxDetailVM sataxvm = new SalaryTaxRepo().GetByIdSalaryTaxDetails(taxDetailsId);
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            if (taxDetailsId != 0)
            {
                vm = repo.SelectById(sataxvm.EmployeeId);
                vm.salaryTaxDetail = sataxvm;
                vm.FiscalYearDetailId = Convert.ToInt32(sataxvm.FiscalYearDetailId);
            }
            return View(vm);
        }
        public ActionResult DetailCreate(string empcode = "", string btn = "current", string FiscalYearDetailId = "0", int taxDetailsId = 0)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "create").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            SalaryTaxDetailVM sataxvm = new SalaryTaxDetailVM();
            if (taxDetailsId != 0)
            {
                sataxvm = new SalaryTaxRepo().GetByIdSalaryTaxDetails(taxDetailsId);
                vm = repo.SelectById(sataxvm.EmployeeId);
                vm.salaryTaxDetail = sataxvm;
            }
            else
            {
                vm = repo.SelectEmpForSearch(empcode, btn);
                if (vm.Id != null && FiscalYearDetailId != "null")
                {
                    sataxvm = new SalaryTaxRepo().GetByEmpIdandFdidSalaryTaxDetails(vm.Id, Convert.ToInt32(FiscalYearDetailId));
                    vm.salaryTaxDetail = sataxvm;
                }
                else
                {
                    vm.Code = empcode;
                }
            }
            if (vm.EmpName == null)
            {
                vm.EmpName = "Employee Name";
            }
            return PartialView("_detailCreate", vm);
        }
        [HttpPost]
        public ActionResult SingleTaxEdit(SalaryTaxDetailVM vm)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            SalaryTaxRepo repo = new SalaryTaxRepo();
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            string[] result = repo.SalaryTaxSingleEdit(vm);
            Session["result"] = result[0];
            ViewBag.msg = result[1];
            return View();
        }
        public ActionResult CheckPFId(string empid, int fid)
        {
            bool exiting = true;
            SalaryTaxRepo _repo = new SalaryTaxRepo();
            var exit = _repo.GetByEmpIdandFdidSalaryTaxDetails(empid, Convert.ToInt32(fid));
            if ((!string.IsNullOrWhiteSpace(empid) && fid == 0))
            {
                exiting = false;
            }
            return Json(exiting, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ImportTAX()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        public ActionResult ImportTAXExcel(HttpPostedFileBase file, int FYDId = 0)
        {
            string[] result = new string[6];
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
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
                result = new SalaryTaxRepo().ImportExcelFile(fullPath, file.FileName, vm, Convert.ToInt32(identity.BranchId), FYDId);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportTAX");
                //return RedirectToAction("OpeningBalance");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportTAX");
            }
        }
        public ActionResult DownloadExcel(HttpPostedFileBase file, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, string Orderby = null)
        {
            string[] result = new string[6];
            DataTable dt = new DataTable();
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                ExcelPackage xlPackage = new ExcelPackage();
                string FileName = "Download.xlsx";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                //string fullPath = @"C:\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }
                dt = new SalaryTaxRepo().ExportExcelFile(fullPath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, Orderby);
                if (dt.Rows.Count <= 0)
                {
                    result[0] = "Fail";
                    result[1] = "No Data Foud! ";
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("ImportTAX");
                }
                else
                {
                    FileName = "SalaryTAX-" + dt.Rows[0]["Fiscal Period"] + ".xlsx";
                }
                //Ordinary.exp(dt);
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename="+FileName);
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                //Response.ContentType = "application/vnd.ms-excel";
                //Response.AppendHeader("content-disposition", "attachment; filename=myfile.xls");
                //using (System.IO.StringWriter sw = new System.IO.StringWriter())
                //{
                //    using (System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw))
                //    {
                //        GridView grid = new GridView();
                //        grid.DataSource = dt;
                //        grid.DataBind();
                //        grid.RenderControl(htw);
                //        Response.Write(sw.ToString());
                //    }
                //}
                //Response.End();
                result[0] = "Success";
                result[1] = "Success~Data Save ";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportTAX");
                //return Redirect("C:/" + FileName);
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;
                Session["result"] = result[0] + "~" + result[1];
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportTAX");
            }
        }
        public ActionResult SalaryTaxReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
           , string Orderby, string view, string rptPG, int fid = 0, int fidTo = 0, string Statement = null)
        {
            try
            {
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
                #region Variables, List and Report Params
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
                if (rptPG.ToLower() == "fiscal period")
                    rptPG = "FP";
                else if (rptPG.ToLower() == "employee name")
                    rptPG = "EN";
                #endregion Variables, List and Report Params
                ReportDocument doc = new ReportDocument();
                SalaryTaxRepo _repo = new SalaryTaxRepo();
                //var BranchId = Convert.ToInt32(identity.BranchId);
                //var getAllData = _repo.SelectAll(BranchId);
                List<SalaryTaxDetailVM> getAllData = new List<SalaryTaxDetailVM>();
                getAllData = _repo.SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, Orderby);
                string ReportHead = "";
                ReportHead = "There are no data to Preview for Tax";
                if (getAllData.Count > 0)
                {
                    if (Statement == "y")
                        ReportHead = "Employee Tax Statement";
                    else
                        ReportHead = "Salary Tax";
                }

                //
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                DataSet ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtTAX";
                string rptLocation = "";
                if (Statement == "y")
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptEmployeeTAXStatement.rpt";
                }
                else
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptSalaryTax.rpt";
                }
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                if (Statement != "y")
                {
                    doc.DataDefinition.FormulaFields["rptParamGroup"].Text = "'" + rptPG + "'";
                }
                doc.DataDefinition.FormulaFields["fyParam"].Text = "'" + fyParam + "'";
                doc.DataDefinition.FormulaFields["fyToParam"].Text = "'" + fyToParam + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
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
        public ActionResult _rptIndexPartial(string ProjectId, string DepartmentId, string SectionId, string DesignationId
           , string CodeF, string CodeT, int fid = 0, int fidTo = 0, string Orderby = null)
        {
            SalaryTaxDetailVM vm = new SalaryTaxDetailVM();
            vm.ProjectId = ProjectId;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.DesignationId = DesignationId;
            vm.CodeF = CodeF;
            vm.CodeT = CodeT;
            vm.FiscalYearDetailId = fid.ToString();
            vm.fidTo = fidTo;
            vm.Orderby = Orderby;
            return PartialView("_rptIndex", vm);
        }
        public ActionResult _rptIndex(JQueryDataTableParamVM param, string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, int fid = 0, int fidTo = 0, string Orderby = null)
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
            SalaryTaxRepo _repo = new SalaryTaxRepo();
            var getAllData = _repo.SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                vDesignationId, vCodeF, vCodeT, Orderby);
            IEnumerable<SalaryTaxDetailVM> filteredData;
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
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_9"]);
                var isSearchable10 = Convert.ToBoolean(Request["bSearchable_10"]);
                filteredData = getAllData
                    .Where(c => isSearchable1 && c.PeriodName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable8 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable9 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable10 && c.TaxValue.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryTaxDetailVM, string> orderingFunction = (c => sortColumnIndex == 0 && isSortable_0 ? c.PeriodStart :
                                                           sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Department :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Section :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Project :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.BasicSalary.ToString() :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.GrossSalary.ToString() :
                                                           sortColumnIndex == 9 && isSortable_9 ? c.TaxValue.ToString() :
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
                             , c.TaxValue.ToString() 
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
        public ActionResult DownloadExcel1(HttpPostedFileBase file, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, string Orderby = null)
        {
            string[] result = new string[6];
            DataTable dt = new DataTable();
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                string FileName = "Download.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }
                dt = new SalaryTaxRepo().ExportExcelFile(fullPath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, Orderby);
                Ordinary.exp(dt);
                //Response.ClearContent();
                //Response.AddHeader("content-disposition", "attachment;filename="+FileName);
                //Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                //using (System.IO.StringWriter sw = new System.IO.StringWriter())
                //{
                //    using (System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw))
                //    {
                //        GridView grid = new GridView();
                //        grid.DataSource = dt;
                //        grid.DataBind();
                //        grid.RenderControl(htw);
                //        Response.Write(sw.ToString());
                //    }
                //}
                //Response.End();
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportTAX");
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;
                Session["result"] = result[0] + "~" + result[1];
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportTAX");
            }
        }


        public ActionResult DownloadTaxExcel(ParameterVM paramVM)
        {
            string[] result = new string[6];
            ResultVM rVM = new ResultVM();

            DataTable dt = new DataTable();
            List<string> ProjectIdList = new List<string>();
            try
            {
                #region Objects and Variables

                string filename = "SalaryTax";

                SalaryTaxRepo _repo = new SalaryTaxRepo();

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                //string FileName = "Download.xls";
                //string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                ////string fullPath = @"C:\";
                //if (System.IO.File.Exists(fullPath + FileName))
                //{
                //    System.IO.File.Delete(fullPath + FileName);
                //}

                
                #endregion

               

                rVM = _repo.DownloadExcel(paramVM);

                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + rVM.ReportName + ".xlsx");
                    rVM.excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion

                #region Redirect

                //result[0] = "Success";
                //result[1] = "Successful~Data Download";

                Session["result"] = rVM.Status + "~" + rVM.Message;
                return Redirect("SalaryTaxReport");
                #endregion

            }
            catch (Exception)
            {
                Session["result"] = rVM.Status + "~" + rVM.Message;
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return Redirect("SalaryTaxReport");
            }
        }

    
    }
}
