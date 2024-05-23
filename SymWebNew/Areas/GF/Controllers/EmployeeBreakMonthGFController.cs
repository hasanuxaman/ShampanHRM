using CrystalDecisions.CrystalReports.Engine;
using Newtonsoft.Json;
using OfficeOpenXml;
using SymOrdinary;
using SymReporting.PF;
using SymRepository.Common;
using SymRepository.GF;
using SymRepository.HRM;
using SymViewModel.Common;
using SymViewModel.GF;
using SymViewModel.HRM;
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

namespace SymWebUI.Areas.GF.Controllers
{
    public class EmployeeBreakMonthGFController : Controller
    {
        public EmployeeBreakMonthGFController()
        {
            ViewBag.TransType = AreaTypeGFVM.TransType;
        }
        EmployeeBreakMonthGFRepo _eaRepo = new EmployeeBreakMonthGFRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //
        // GET: /PF/EmployeeBreakMonthPF/

        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View();
        }

        public ActionResult _index(JQueryDataTableParamVM param)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var EmployeeNameFilter = Convert.ToString(Request["sSearch_2"]);
            var OpeningValueFilter = Convert.ToString(Request["sSearch_3"]);
            var OpeningValue1Filter = Convert.ToString(Request["sSearch_4"]);
            var OpeningDateFilter = Convert.ToString(Request["sSearch_5"]);
            var PostFilter = Convert.ToString(Request["sSearch_6"]);


            #endregion
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeBreakMonthGFRepo arerepo = new EmployeeBreakMonthGFRepo();
            var getAllData = arerepo.SelectAll();
            IEnumerable<EmployeeBreakMonthGFVM> filteredData;
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
                    || isSearchable3 && c.EmployerContribution.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.EmployerProfit.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.OpeningDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (codeFilter != "" || EmployeeNameFilter != "" || OpeningValueFilter != "" || OpeningValue1Filter != "" || OpeningDateFilter != "" || PostFilter != "")
            {
                filteredData = filteredData.Where(c =>
                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                    && (EmployeeNameFilter == "" || c.EmpName.ToLower().Contains(EmployeeNameFilter.ToLower()))
                    && (OpeningValueFilter == "" || c.EmployerContribution.ToString().Contains(OpeningValueFilter.ToLower()))
                    && (OpeningValue1Filter == "" || c.EmployerProfit.ToString().Contains(OpeningValue1Filter.ToLower()))
                    && (OpeningDateFilter == "" || c.OpeningDate.ToLower().Contains(OpeningDateFilter.ToLower()))
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
            Func<EmployeeBreakMonthGFVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.EmployerContribution.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.EmployerProfit.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.OpeningDate.ToString() :
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
                             , c.EmployerContribution.ToString() 
                             , c.EmployerProfit.ToString() 
                             , c.OpeningDate.ToString()
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

        public ActionResult SingleEmployeeBreakMonthGFEdit(string PFOpeinigId, string Operation = "")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            EmployeeBreakMonthGFVM empBreakMonthGFVM = new EmployeeBreakMonthGFVM();
            if (!string.IsNullOrWhiteSpace(PFOpeinigId))
                empBreakMonthGFVM = _eaRepo.SelectByIdAll(PFOpeinigId);
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            if (!string.IsNullOrWhiteSpace(PFOpeinigId) && !string.IsNullOrWhiteSpace(empBreakMonthGFVM.Id))
            {
                vm = repo.AllSelectById(empBreakMonthGFVM.EmployeeId);
            }
            vm.empBreakMonthGFVM = empBreakMonthGFVM;
            vm.Operation = Operation;

            Session["PFOpeinigId"] = empBreakMonthGFVM.Id;
            return View(vm);
        }

        public ActionResult DetailCreate(string empcode = "", string btn = "current", string id = "0", string Operation = "")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            string EmployeeId = "";
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            //EmployeeOtherEarningRepo arerepo = new EmployeeOtherEarningRepo();
            EmployeeBreakMonthGFVM PFOpeinigVM = new EmployeeBreakMonthGFVM();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrEmpty(Session["PFOpeinigId"] as string) && Session["PFOpeinigId"] as string != "0")
            {
                string PFOpeinigId = Session["PFOpeinigId"] as string;
                PFOpeinigVM = _eaRepo.SelectByIdAll(PFOpeinigId);//find emp code
                vm = repo.AllSelectById(PFOpeinigVM.EmployeeId);
                vm.empBreakMonthGFVM = PFOpeinigVM;
                Session["PFOpeinigId"] = "";
                // find exist earning date
            }
            else if (id != "0" && !string.IsNullOrWhiteSpace(id))
            {
                PFOpeinigVM = _eaRepo.SelectByIdAll(id);//find emp code
                vm = repo.AllSelectById(PFOpeinigVM.EmployeeId);
                vm.empBreakMonthGFVM = PFOpeinigVM;
                // find exist earning date
            }
            else
            {
                vm = repo.SelectEmpForSearchAll(empcode, btn);

                if (!string.IsNullOrWhiteSpace(vm.EmployeeId))
                {
                    PFOpeinigVM = _eaRepo.SelectByIdAll("", vm.EmployeeId);
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
                vm.empBreakMonthGFVM = PFOpeinigVM;
                vm.empBreakMonthGFVM.EmployeeId = EmployeeId;
                vm.Operation = Operation;
                if (vm.Operation.ToLower() == "add")
                {
                    vm.Id = null;
                    vm.empBreakMonthGFVM.Id = null;
                    vm.empBreakMonthGFVM.EmployerContribution = 0;
                    vm.empBreakMonthGFVM.EmployerProfit = 0;
                    vm.empBreakMonthGFVM.OpeningDate = null;
                    vm.empBreakMonthGFVM.Post = false;
                }
            }
            return PartialView("_detailCreate", vm);
        }

        [HttpPost]
        public ActionResult Create(EmployeeInfoVM empVM)
        {
            EmployeeBreakMonthGFVM vm = new EmployeeBreakMonthGFVM();
            string[] result = new string[6];
            try
            {
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm = empVM.empBreakMonthGFVM;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
               
                result = _eaRepo.Insert(vm);

                return Json(result[0] + "~" + result[1] + "~" + result[2], JsonRequestBehavior.AllowGet);

                ////Session["result"] = result[0] + "~" + result[1];

                ////return Redirect("/PF/EmployeeBreakMonthPF/SingleEmployeeBreakMonthPFEdit?PFOpeinigId=" + result[2].ToString());

                //return RedirectToAction("SingleEmployeeBreakMonthPFEdit", result[2]);

            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Update(EmployeeInfoVM empVM)
        {
            EmployeeBreakMonthGFVM vm = new EmployeeBreakMonthGFVM();
            string[] result = new string[6];
            try
            {
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm = empVM.empBreakMonthGFVM;
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                //vm.FiscalYearDetailId = empVM.FiscalYearDetailId;

                result = _eaRepo.Update(vm);
                ////if (result[0].ToLower() == "success" && btn.ToLower() != "save")
                ////{
                ////    result[1] = "Data Deleted Successfully";
                ////}
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
            EmployeeBreakMonthGFVM vm = new EmployeeBreakMonthGFVM();
            string[] result = new string[6];
            try
            {
                vm = empVM.empBreakMonthGFVM;
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

            EmployeeBreakMonthGFVM EarningVM = new EmployeeBreakMonthGFVM();

            EarningVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            EarningVM.LastUpdateBy = identity.Name;
            EarningVM.LastUpdateFrom = identity.WorkStationIP;
            string[] a = ids.Split('~');
            result = _eaRepo.MultiplePost(EarningVM, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportEmployeeBreakMonthPF()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
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
                    return Redirect("/PF/Home");
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
                return RedirectToAction("ImportEmployeeBreakMonthPF");
                //return RedirectToAction("OpeningBalance");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportEmployeeBreakMonthPF");
            }
        }

        public ActionResult DownloadExcel_Employee(string ProjectId, string DepartmentId, string SectionId , string DesignationId, string CodeF, string CodeT
            , string Orderby = null)
        {
            DataTable dt = new DataTable();
            string[] result = new string[6];
            try
            {
                EmployeeBreakMonthGFVM vm = new EmployeeBreakMonthGFVM();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/PF/Home");
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

                string filename = "EmployeeGFBreakMonth" + "-" + DateTime.Now.ToString("yyyyMMdd");
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
                return RedirectToAction("ImportEmployeeBreakMonthPF");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportEmployeeBreakMonthPF");
            }
        }


        public ActionResult DownloadExcel_Opening(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
          , string Orderby = null)
        {
            DataTable dt = new DataTable();
            string[] result = new string[6];
            try
            {
                EmployeeBreakMonthGFVM vm = new EmployeeBreakMonthGFVM();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/PF/Home");
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

                dt = _eaRepo.ExportExcelFileFormPFOpening(vm, fullPath, FileName);
                //exp(dt);
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

                string filename = "EmployeeGFBreakMonth" + "-" + DateTime.Now.ToString("yyyyMMdd");
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
                return RedirectToAction("ImportEmployeeBreakMonthPF");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportEmployeeBreakMonthPF");
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
                    ReportHead = "PF Employee BreakMonth Transactions";
                }
                dt.TableName = "dtEmployeeForfeiture";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\GF\\rptGFEmployeeBreakMonth.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";

                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
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
                EmployeeBreakMonthGFVM EmployeeBreakMonthGFVM = new EmployeeBreakMonthGFVM();
                PFReport report = new PFReport();
                EmployeeBreakMonthGFVM = _eaRepo.SelectById(id.ToString());
                vm.Id = id;
                vm.Code = EmployeeBreakMonthGFVM.Code;
                return PartialView("reportVeiw", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult EmployeeBreakMonthGFReport(PFReportVM vm)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                string[] cFields = { "e.Code", "pfo.Id", "e.JoinDate>", "e.JoinDate<" };
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo) };
                var Result = _eaRepo.SelectAllList(null, cFields, cValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));

                
                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                dt.TableName = "dtEmployeeForfeiture";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\GF\\rptGFEmployeeBreakMonth.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypeGFVM.TransType + "'";
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



        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }
        



    }
}
