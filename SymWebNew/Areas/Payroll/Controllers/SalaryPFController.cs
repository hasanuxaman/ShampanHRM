using CrystalDecisions.CrystalReports.Engine;
using OfficeOpenXml;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.PF;
using SymWebUI.Files.ReportFiles.Payroll.PayrollProcess;
//using SymWebUI.Areas.Payroll.Reports.PayrollProcess;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class SalaryPFController : Controller
    {
        //
        // GET: /Payroll/EmployeePF/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        public ActionResult _SalaryPF(JQueryDataTableParamVM param)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryPFRepo repo = new SalaryPFRepo();
            var getAllData = repo.GetPeriodNameDistrinct();
            IEnumerable<SalaryPFVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.PeriodName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryPFVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.PeriodStart :
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
            var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "add").ToString();
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
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            SalaryPFRepo saPFrepo = new SalaryPFRepo();
            SalaryPFDetailVM saPFDvm = new SalaryPFDetailVM();
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            saPFDvm = vm.saPFDvm;
            saPFDvm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            saPFDvm.CreatedBy = Identity.Name;
            saPFDvm.CreatedFrom = Identity.WorkStationIP;
            result = saPFrepo.SalaryPFSingleAddorUpdate(saPFDvm, Convert.ToInt32(Identity.BranchId));
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }
        //[HttpGet]
        //public JsonResult SalaryPFProces(EmployeeInfoVM vm)
        //{
        //    ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //    FiscalYearVM fyvm = new FiscalYearVM();
        //    SalaryPFRepo repo = new SalaryPFRepo();
        //    fyvm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
        //    fyvm.CreatedBy = identity.Name;
        //    fyvm.CreatedFrom = identity.WorkStationIP;
        //    fyvm.BranchId = Convert.ToInt32(identity.BranchId);
        //    string[] result = repo.InsertSalaryPF(vm.FiscalYearDetailId, vm.ProjectId, vm.DepartmentId, vm.SectionId, fyvm);
        //    return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Edit(int FID)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            SalaryPFRepo repo = new SalaryPFRepo();
            var tt = repo.SelectAll(Convert.ToInt32(identity.BranchId), FID).FirstOrDefault();
            if (tt != null)
            {
                ViewBag.periodName = tt.PeriodName;
            }
            ViewBag.Id = FID;
            return View();
        }
        public ActionResult _salaryPFDetails(JQueryDataTableParamVM param, int FID)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var CodeFilter = Convert.ToString(Request["sSearch_1"]);
            var EmployeeNameFilter = Convert.ToString(Request["sSearch_2"]);
            var BasicSalaryFilter = Convert.ToString(Request["sSearch_3"]);
            var GrossSalaryFilter = Convert.ToString(Request["sSearch_4"]);
            var PFValueFilter = Convert.ToString(Request["sSearch_5"]);
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
            var PFamountFrom = 0;
            var PFamountTo = 0;
            if (PFValueFilter.Contains('~'))
            {
                PFamountFrom = PFValueFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(PFValueFilter.Split('~')[0]) == true ? Convert.ToInt32(PFValueFilter.Split('~')[0]) : 0;
                PFamountTo = PFValueFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(PFValueFilter.Split('~')[0]) == true ? Convert.ToInt32(PFValueFilter.Split('~')[0]) : 0;
            }
            #endregion
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryPFRepo repo = new SalaryPFRepo();
            var getAllData = repo.SelectAllSalaryPFDetails(FID);
            IEnumerable<SalaryPFDetailVM> filteredData;
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
                    isSearchable5 && c.PFValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (CodeFilter != "" || EmployeeNameFilter != "" || (BasicSalaryFilter != "" && BasicSalaryFilter != "~") || (GrossSalaryFilter != "" && GrossSalaryFilter != "~") || (PFValueFilter != "" && PFValueFilter != "~"))
            {
                filteredData = filteredData.Where(c =>
                    (CodeFilter == "" || c.Code.ToLower().Contains(CodeFilter.ToLower()))
                    && (EmployeeNameFilter == "" || c.EmpName.ToLower().Contains(EmployeeNameFilter.ToLower()))
                    && (BasicamountFrom == 0 || BasicamountFrom <= Convert.ToInt32(c.BasicSalary))
                    && (BasicamountTo == 0 || BasicamountTo >= Convert.ToInt32(c.BasicSalary))
                    && (GrossamountFrom == 0 || GrossamountFrom <= Convert.ToInt32(c.GrossSalary))
                    && (GrossamountTo == 0 || GrossamountTo >= Convert.ToInt32(c.GrossSalary))
                    && (PFamountFrom == 0 || PFamountFrom <= Convert.ToInt32(c.PFValue))
                    && (PFamountTo == 0 || PFamountTo >= Convert.ToInt32(c.PFValue))
                    );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryPFDetailVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.BasicSalary.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.GrossSalary.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.PFValue.ToString() :
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
                             , c.PFValue.ToString() 
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
        public ActionResult SalaryPFDetailsDelete(string ids)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            SalaryPFRepo repo = new SalaryPFRepo();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = repo.SalaryPFDetailsDelete(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        public ActionResult SalaryPFDelete(string ids)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            SalaryPFRepo repo = new SalaryPFRepo();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = repo.SalaryPFDelete(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult SalaryPFSingle()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        [HttpGet]
        public ActionResult SinglePFEdit(int pfDetailsId)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            SalaryPFDetailVM saPFDvm = new SalaryPFRepo().GetByIdSalaryPFDetails(pfDetailsId);
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            if (pfDetailsId != 0 && saPFDvm.Id > 0)
            {
                vm = repo.SelectById(saPFDvm.EmployeeId);
            }
            vm.saPFDvm = saPFDvm;
            vm.FiscalYearDetailId = Convert.ToInt32(saPFDvm.FiscalYearDetailId);
            return View(vm);
        }
        public ActionResult DetailCreate(string empcode = "", string btn = "current", string FiscalYearDetailId = "0", int pfDetailsId = 0)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            SalaryPFDetailVM saPFDvm = new SalaryPFDetailVM();
            if (pfDetailsId != 0)
            {
                saPFDvm = new SalaryPFRepo().GetByIdSalaryPFDetails(pfDetailsId);
                if (saPFDvm.Id > 0)
                {
                    vm = repo.SelectById(saPFDvm.EmployeeId);
                }
            }
            else
            {
                vm = repo.SelectEmpForSearch(empcode, btn);
                if (vm.Id != null && FiscalYearDetailId != null)
                {
                    saPFDvm = new SalaryPFRepo().GetByIdSalaryPFbyempidandfidDetail(vm.Id, Convert.ToInt32(FiscalYearDetailId));
                    vm.saPFDvm = saPFDvm;
                }
            }
            if (vm.Id == null)
            {
                vm.EmpName = "Employee Name";
            }
            vm.saPFDvm = saPFDvm;
            return PartialView("_detailCreate", vm);
        }
        [HttpPost]
        public ActionResult SinglePFEdit(SalaryPFDetailVM vm)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            SalaryPFRepo repo = new SalaryPFRepo();
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            string[] result = repo.SalaryPFSingleEdit(vm);
            Session["result"] = result[0];
            ViewBag.msg = result[1];
            return View();
        }
        [HttpPost]
        public ActionResult SalaryPFSingle(string FiscalPeriodDetailsId, string empID)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            SalaryPFDetailVM vm = new SalaryPFDetailVM();
            SalaryPFRepo repo = new SalaryPFRepo();
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = Identity.Name;
            vm.CreatedFrom = Identity.WorkStationIP;
            vm.FiscalYearDetailId = FiscalPeriodDetailsId;
            vm.EmployeeId = empID;
            string[] getAllData = repo.SalaryPFSingleAddorUpdate(vm, Convert.ToInt32(Identity.BranchId));
            ViewBag.mgs = getAllData[0];
            return Json(getAllData, JsonRequestBehavior.AllowGet);
            //return JavaScript(string.Format("ShowResult('{0}','{1}','{2}','{3}')", getAllData[0], getAllData[1], dataAction, "/Payroll/EmployeePF"));
        }
        public ActionResult ImportPF()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        public ActionResult ImportPFExcel(HttpPostedFileBase file, int FYDId = 0)
        {
            string[] result = new string[6];
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "add").ToString();
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
                result = new SalaryPFRepo().ImportExcelFile(fullPath, vm, Convert.ToInt32(identity.BranchId), FYDId);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportPF");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportPF");
            }
        }
        public ActionResult DownloadExcel(HttpPostedFileBase file, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, string Orderby = null)
        {
            string[] result = new string[6];
            DataTable dt = new DataTable();
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_45", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                string FileName = "SalaryPF" + ".xlsx";

                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                //string fullPath = @"C:\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }
                dt = new SalaryPFRepo().ExportExcelFile(fullPath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, Orderby);
                if (dt.Rows.Count <= 0)
                {
                    result[0] = "Fail";
                    result[1] = "No Data Foud! ";
                    Session["result"] = result[0] + "~" + result[1];
                    return View("ImportPF");
                }
                else
                {
                    FileName = "SalaryPF-" + dt.Rows[0]["Fiscal Period"] + ".xlsx";
                }

                //Ordinary.exp(dt);
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + FileName);
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                //Response.ClearContent();
                //Response.AddHeader("content-disposition", "attachment;filename=" + FileName);
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
                result[0] = "Success";
                result[1] = "Data Save ";
                Session["result"] = result[0] + "~" + result[1];
                return View("ImportPF");
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;
                Session["result"] = result[0] + "~" + result[1];
                return View("ImportPF");
            }
        }
        public ActionResult SalaryPFReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, string view, string rptPG, string Orderby, int fid = 0, int fidTo = 0, string Statement = null)
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

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

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
                SalaryPFRepo _repo = new SalaryPFRepo();
                //var BranchId = Convert.ToInt32(identity.BranchId);
                List<SalaryPFDetailVM> getAllData = new List<SalaryPFDetailVM>();
                getAllData = new SalaryPFRepo().SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, Orderby);
                string ReportHead = "";
                ReportHead = "There are no data to Preview Provident Fund";
                if (getAllData.Count > 0)
                {
                    if (Statement == "y")
                        ReportHead = "Employee Provident Fund Statement";
                    else
                        ReportHead = "Salary Provident Fund";
                }
                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                DataSet ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtPF";
                string rptLocation = "";
                if (Statement == "y")
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptEmployeePFStatement.rpt";
                }
                else
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollProcess\rptSalaryPFList.rpt";
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
        public ActionResult _rptIndexPartial(string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, int fid = 0, int fidTo = 0, string Orderby = null)
        {
            SalaryPFDetailVM vm = new SalaryPFDetailVM();
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
            SalaryPFRepo repo = new SalaryPFRepo();
            var getAllData = new SalaryPFRepo().SelectAllForReport(fid, fidTo, vProjectId, vDepartmentId, vSectionId,
                vDesignationId, vCodeF, vCodeT, Orderby);
            IEnumerable<SalaryPFDetailVM> filteredData;
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
                               || isSearchable10 && c.PFValue.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<SalaryPFDetailVM, string> orderingFunction = (c => sortColumnIndex == 0 && isSortable_0 ? c.PeriodStart :
                                                           sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Department :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Section :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Project :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.BasicSalary.ToString() :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.GrossSalary.ToString() :
                                                           sortColumnIndex == 9 && isSortable_9 ? c.PFValue.ToString() :
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
                             , c.PFValue.ToString() 
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
        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }
    }
}
