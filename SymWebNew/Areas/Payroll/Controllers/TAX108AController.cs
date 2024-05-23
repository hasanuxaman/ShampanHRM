using CrystalDecisions.CrystalReports.Engine;
using Newtonsoft.Json;
using OfficeOpenXml;
using SymOrdinary;
using SymReporting.PF;
using SymRepository.Common;
using SymRepository.GF;
using SymRepository.HRM;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.GF;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using SymViewModel.PF;
using SymWebUI.Areas.PF.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Payroll.Controllers
{
    public class TAX108AController : Controller
    {
        public TAX108AController()
        {
            ViewBag.TransType = AreaTypeGFVM.TransType;
        }
        TAX108ARepo _eaRepo = new TAX108ARepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //
        // GET: /GF/EmployeeBreakMonthPF/

        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }

        public ActionResult _index(JQueryDataTableParamVM param)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var EmployeeNameFilter = Convert.ToString(Request["sSearch_2"]);
            var SubmitSerialNoFilter = Convert.ToString(Request["sSearch_3"]);
            var SubmitYearFilter = Convert.ToString(Request["sSearch_4"]);
            var SubmitDateFilter = Convert.ToString(Request["sSearch_5"]);
            var PostFilter = Convert.ToString(Request["sSearch_6"]);

            #endregion
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var getAllData = _eaRepo.SelectAll();
            IEnumerable<TAX108AVM> filteredData;
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
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.SubmitSerialNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.SubmitYear.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.SubmitDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (codeFilter != "" || EmployeeNameFilter != "" || SubmitSerialNoFilter != "" || SubmitYearFilter != "" || SubmitDateFilter != "" || PostFilter != "")
            {
                filteredData = filteredData.Where(c =>
                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                    && (EmployeeNameFilter == "" || c.EmpName.ToLower().Contains(EmployeeNameFilter.ToLower()))
                    && (SubmitSerialNoFilter == "" || c.SubmitSerialNo.ToString().Contains(SubmitSerialNoFilter.ToLower()))
                    && (SubmitYearFilter == "" || c.SubmitYear.ToString().Contains(SubmitYearFilter.ToLower()))
                    && (SubmitDateFilter == "" || c.SubmitDate.ToLower().Contains(SubmitDateFilter.ToLower()))
                    //&& (PostFilter == "" || c.Post.ToLower().Contains(PostFilter.ToLower()))
                    
                    );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<TAX108AVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.SubmitSerialNo.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.SubmitYear.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.SubmitDate.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.Post.ToString() :
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
                             ,c.Code
                             , c.EmpName
                             , c.SubmitSerialNo.ToString() 
                             , c.SubmitYear.ToString() 
                             , c.SubmitDate.ToString()
                             , c.Post==true ? "Posted" : "Not Posted"
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

        public ActionResult SingleEmployeeGFOpeinigEdit(string Id)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            TAX108AVM empTAX108AVM = new TAX108AVM();
            if (!string.IsNullOrWhiteSpace(Id))
                empTAX108AVM = _eaRepo.SelectById(Id);
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            if (!string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(empTAX108AVM.Id))
            {
                vm = repo.SelectById(empTAX108AVM.EmployeeId);
            }
            vm.empTAX108AVM = empTAX108AVM;
            Session["TAX108AId"] = empTAX108AVM.Id;
            return View(vm);
        }

        public ActionResult DetailCreate(string empcode = "", string btn = "current", string id = "0")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            string EmployeeId = "";
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            //EmployeeOtherEarningRepo arerepo = new EmployeeOtherEarningRepo();
            TAX108AVM _TAX108AVM = new TAX108AVM();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrEmpty(Session["TAX108AId"] as string) && Session["TAX108AId"] as string != "0")
            {
                string TAX108AId = Session["TAX108AId"] as string;
                _TAX108AVM = _eaRepo.SelectById(TAX108AId);//find emp code
                vm = repo.SelectById(_TAX108AVM.EmployeeId);
                vm.empTAX108AVM = _TAX108AVM;
                Session["TAX108AId"] = "";
                // find exist earning date
            }
            else if (id != "0" && !string.IsNullOrWhiteSpace(id))
            {
                _TAX108AVM = _eaRepo.SelectById(id);//find emp code
                vm = repo.SelectById(_TAX108AVM.EmployeeId);
                vm.empTAX108AVM = _TAX108AVM;
                // find exist earning date
            }
            else
            {
                vm = repo.SelectEmpForSearch(empcode, btn);

                if (!string.IsNullOrWhiteSpace(vm.EmployeeId))
                {
                    _TAX108AVM = _eaRepo.SelectById("", vm.EmployeeId);
                }

                if (vm.EmpName == null)
                {
                    vm.EmpName = "Employee Name";
                }
                else
                {
                    EmployeeId = vm.Id;
                }

                //svms = arerepo.SingleEmployeeEntry(EmployeeId, FiscalYearDetailId);
                vm.empTAX108AVM = _TAX108AVM;
                vm.empTAX108AVM.EmployeeId = EmployeeId;
            }
            return PartialView("_detailCreate", vm);
        }

        [HttpPost]
        public ActionResult Create(EmployeeInfoVM empVM)
        {
            TAX108AVM vm = new TAX108AVM();
            string[] result = new string[6];
            try
            {
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm = empVM.empTAX108AVM;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
               
                result = _eaRepo.Insert(vm);

                return Json(result[0] + "~" + result[1] + "~" + result[2], JsonRequestBehavior.AllowGet);

            
            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Update(EmployeeInfoVM empVM)
        {
            TAX108AVM vm = new TAX108AVM();
            string[] result = new string[6];
            try
            {
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm = empVM.empTAX108AVM;
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;

                result = _eaRepo.Update(vm);
               
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Post(EmployeeInfoVM empVM)
        {
            TAX108AVM vm = new TAX108AVM();
            string[] result = new string[6];
            try
            {
                vm = empVM.empTAX108AVM;
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;

                result = _eaRepo.Post(vm);
                
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult MultiplePost(string ids)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            string[] result = new string[6];

            TAX108AVM vm = new TAX108AVM();

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            string[] a = ids.Split('~');
            result = _eaRepo.MultiplePost(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportTAX108A()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/GF/Home");
            }
            return View();
        }

        public ActionResult ImportExcel(HttpPostedFileBase file)
        {
            string[] result = new string[6];
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
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
                result = _eaRepo.ImportExcelFile(fullPath, file.FileName, vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportTAX108A");
                //return RedirectToAction("OpeningBalance");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportTAX108A");
            }
        }

        public ActionResult DownloadExcel_Employee(string ProjectId, string DepartmentId, string SectionId , string DesignationId, string CodeF, string CodeT
            , string Orderby = null)
        {
            DataTable dt = new DataTable();
            string[] result = new string[6];
            try
            {
                TAX108AVM vm = new TAX108AVM();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                string FileName = "Download.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                string contentType = MimeMapping.GetMimeMapping(fullPath);
                //string fullPath = @"C:\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }

                vm.DesignationId = DesignationId;
                vm.DepartmentId = DepartmentId;
                vm.SectionId = SectionId;
                vm.ProjectId = ProjectId;
                vm.Orderby = Orderby;
                vm.Code = CodeF;
                vm.CodeT = CodeT;

                dt = _eaRepo.ExportExcelFileFormEmployee(vm, fullPath, FileName);
                //exp(dt);
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

                string filename = "TAX108A_Employee" + "-" + DateTime.Now.ToString("yyyyMMdd");
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                result[0] = "Successfull";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportTAX108A");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportTAX108A");
            }
        }


        public ActionResult DownloadExcel_Opening(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
          , string Orderby = null)
        {
            DataTable dt = new DataTable();
            string[] result = new string[6];
            try
            {
                TAX108AVM vm = new TAX108AVM();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/GF/Home");
                }
                string FileName = "Download.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                string contentType = MimeMapping.GetMimeMapping(fullPath);
                //string fullPath = @"C:\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }

                vm.DesignationId = DesignationId;
                vm.DepartmentId = DepartmentId;
                vm.SectionId = SectionId;
                vm.ProjectId = ProjectId;
                vm.Orderby = Orderby;
                vm.Code = CodeF;
                vm.CodeT = CodeT;

                dt = _eaRepo.ExportExcelFileFormGFOpening(vm, fullPath, FileName);
                //exp(dt);
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

                string filename = "TAX108A" + "-" + DateTime.Now.ToString("yyyyMMdd");
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                result[0] = "Successfull";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportTAX108A");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportTAX108A");
            }
        }

        [HttpGet]
        public ActionResult ReportView(int id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                string[] cFields = { "pfo.Id" };
                string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };
                var Result = _eaRepo.SelectAllList(null, cFields, cValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));


                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "GF Employee BreakMonth Transactions";
                }
                dt.TableName = "dtEmployeeForfeiture";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\GF\\rptPFEmployeeBreakMonth.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult reportVeiw(int id)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                TAX108AVM EmployeeGFOpeinigVM = new TAX108AVM();
                PFReport report = new PFReport();
                EmployeeGFOpeinigVM = _eaRepo.SelectById(id.ToString());
                vm.Id = id;
                vm.Code = EmployeeGFOpeinigVM.Code;
                return PartialView("reportVeiw", vm);

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
