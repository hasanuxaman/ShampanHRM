using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.Payroll;
using SymWebUI.Areas.Payroll.Reports.PayrollProcess;
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
    public class SalaryConvenceController : Controller
    {
        //
        // GET: /Payroll/EmployeeConvence/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _SalaryConvence(JQueryDataTableParamVM param, string code, string name)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryConvenceRepo repo = new SalaryConvenceRepo();
            var getAllData = repo.SelectAll(Convert.ToInt32(identity.BranchId));
            IEnumerable<SalaryConvenceVM> filteredData;
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
            Func<SalaryConvenceVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.PeriodName :
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
                 Convert.ToString(c.Id)
                , c.PeriodName 
                , c.Remarks 
            };
                             
                             //{ "", c.PeriodName + "~" + Convert.ToString(c.Id), c.Remarks };
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
            return View();
        }
        [HttpGet]
        public JsonResult SalaryConvenceProces(int FiscalPeriodDetailsId, string ProjectId, string DepartmentId, string SectionId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            FiscalYearVM vm = new FiscalYearVM();
            SalaryConvenceRepo repo = new SalaryConvenceRepo();
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);

            string[] result = repo.AddOrUpdate(FiscalPeriodDetailsId, ProjectId, DepartmentId, SectionId, vm);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(string salaryConvenceID)
        {
            SalaryConvenceRepo repo = new SalaryConvenceRepo();
            ViewBag.periodName = repo.GetPeriodName(salaryConvenceID);
            ViewBag.Id = salaryConvenceID;
            return View();
        }
        public ActionResult _salaryConvenceDetails(JQueryDataTableParamModel param, string code, string name, string salaryConvenceID)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var employeeNameFilter = Convert.ToString(Request["sSearch_1"]);
            var convenceAmountFilter = Convert.ToString(Request["sSearch_2"]);

            var convenceAmountFrom = 0;
            var convenceAmountTo = 0;
            if (convenceAmountFilter.Contains('~'))
            {
                convenceAmountFrom = convenceAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(convenceAmountFilter.Split('~')[0]) == true ?  Convert.ToInt32(convenceAmountFilter.Split('~')[0]) : 0;
                convenceAmountTo = convenceAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(convenceAmountFilter.Split('~')[1]) == true ?  Convert.ToInt32(convenceAmountFilter.Split('~')[1]) : 0;
            }
               
            var fromID = 0;
            var toID = 0;
            if (idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }
            #endregion Column Search

            #region Search and Filter Data
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryConvenceRepo repo = new SalaryConvenceRepo();
            var getAllData = repo.SelectAllSalaryConvenceDetails(salaryConvenceID);
            IEnumerable<SalaryConvenceDetailVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.EmployeeName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.ConvenceAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable3 && c.Remarks.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data

            #region Column Filtering
            if (employeeNameFilter != "" || (convenceAmountFilter != "" && convenceAmountFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c => 
                                            (employeeNameFilter == "" || c.EmployeeName.ToLower().Contains(employeeNameFilter.ToLower()))
                                            &&
                                            (convenceAmountFrom == 0 || convenceAmountFrom <= Convert.ToInt32(c.ConvenceAmount))
                                            &&
                                            (convenceAmountTo == 0 || convenceAmountTo >= Convert.ToInt32(c.ConvenceAmount))
                                            //&&
                                            //(fromDate == DateTime.MinValue || fromDate <=  Convert.ToDateTime(c.OverTimeDate))
                                            //&&
                                            //(toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.OverTimeDate))
                                        );
            }

            #endregion Column Filtering


            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryConvenceDetailVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.EmployeeName :
                                                           sortColumnIndex == 4 && isSortable_2 ? c.ConvenceAmount.ToString() :
                                                           sortColumnIndex == 5 && isSortable_3 ? c.Remarks :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { 
                
                //"" + "~" + 
                Convert.ToString(c.Id)
                , c.EmployeeName //+ "~" + Convert.ToString(c.Id)
                , c.ConvenceAmount.ToString()
                //, c.Remarks 
            };
                //"" + "~" + Convert.ToString(c.Id), c.EmployeeName + "~" + Convert.ToString(c.Id), c.ConvenceAmount.ToString(), c.Remarks };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }


        public JsonResult SalaryConvenceDetailsDelete(string ids)
        {
            SalaryConvenceRepo repo = new SalaryConvenceRepo();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = repo.SalaryConvenceDetailsDelete(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        public JsonResult SalaryConvenceDelete(string ids)
        {
            SalaryConvenceRepo repo = new SalaryConvenceRepo();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = repo.SalaryConvenceDelete(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult SalaryConvenceSingle()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SingleConvenceEdit(int ConvenceDetailsId)
        {
            SalaryConvenceDetailVM vm = new SalaryConvenceRepo().GetByIdSalaryConvenceDetails(ConvenceDetailsId);
            return View(vm);
        }
        [HttpPost]
        public ActionResult SingleConvenceEdit(SalaryConvenceDetailVM vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SalaryConvenceRepo repo = new SalaryConvenceRepo();

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMdd");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            string[] result = repo.SalaryConvenceSingleEdit(vm);
            ViewBag.result = result[0];
            ViewBag.msg = result[1];
            return View();
        }

        [HttpPost]
        public JsonResult SalaryConvenceSingle(string FiscalPeriodDetailsId, string empID)
        {
            SalaryConvenceDetailVM vm = new SalaryConvenceDetailVM();
            SalaryConvenceRepo repo = new SalaryConvenceRepo();
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
            vm.CreatedBy = Identity.Name;
            vm.CreatedFrom = Identity.WorkStationIP;
            vm.FiscalYearDetailId = FiscalPeriodDetailsId;
            vm.EmployeeId = empID;


            string[] getAllData = repo.SalaryConvenceSingleAddorUpdate(vm, Convert.ToInt32(Identity.BranchId));
            ViewBag.mgs = getAllData[0];
            return Json(getAllData, JsonRequestBehavior.AllowGet);
            //return JavaScript(string.Format("ShowResult('{0}','{1}','{2}','{3}')", getAllData[0], getAllData[1], dataAction, "/Payroll/EmployeeBonus"));
        }
        public ActionResult SalaryConvenceReport(int? fid, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string Name, string dojFrom, string dojTo, string view)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                string QC = "N";
                if (!(User.IsInRole("Master") || User.IsInRole("Admin") || User.IsInRole("Account")))
                {
                }
                ViewBag.QC = QC;
                if (view == "Y")
                {
                    return View();
                }
                //rptProductListRepo _repo=new rptProductListRepo();
                ReportDocument doc = new ReportDocument();
                SalaryConvenceRepo _repo = new SalaryConvenceRepo();
                var BranchId = Convert.ToInt32(identity.BranchId);
                //var getAllData = _repo.SelectAll(BranchId);

                List<SalaryConvenceVM> getAllData = new List<SalaryConvenceVM>();
                getAllData = _repo.SelectAll(BranchId, fid);
                if (DepartmentId != "null" && DepartmentId != "" && DepartmentId != null && DepartmentId != "0" && DepartmentId != "0_0")
                    getAllData = getAllData.Where(m => m.DepartmentId == DepartmentId).ToList();
                if (SectionId != "null" && SectionId != "" && SectionId != null && SectionId != "0" && SectionId != "0_0")
                    getAllData = getAllData.Where(m => m.SectionId == SectionId).ToList();
                if (ProjectId != "null" && ProjectId != "" && ProjectId != null && ProjectId != "0" && ProjectId != "0_0")
                    getAllData = getAllData.Where(m => m.ProjectId == ProjectId).ToList();
                if (CodeF != "null" && CodeF != "" && CodeF != null && CodeF != "0" && CodeF != "0_0")
                    getAllData = getAllData.Where(x => Convert.ToInt32(x.Code) <= Convert.ToInt32(CodeF)).ToList();
                if (CodeT != "null" && CodeT != "" && CodeT != null && CodeT != "0" && CodeT != "0_0")
                    getAllData = getAllData.Where(x => Convert.ToInt32(x.Code) >= Convert.ToInt32(CodeT)).ToList();
                if (!string.IsNullOrEmpty(Name))
                    getAllData = getAllData.Where(x => x.EmpName.ToLower().Contains(Name)).ToList();
                //if (dojFrom != null && dojFrom != "false" && !string.IsNullOrEmpty(dojFrom))
                //    getAllData = getAllData.Where(x => Convert.ToDateTime(x.JoinDate) <= Convert.ToDateTime(dojFrom)).ToList();
                //if (dojTo != null && dojTo != "false" && !string.IsNullOrEmpty(dojFrom))
                //    getAllData = getAllData.Where(x => Convert.ToDateTime(x.JoinDate) >= Convert.ToDateTime(dojTo)).ToList();
                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());

                DataSet ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtConvence";

                doc = new rptSalaryConvence();
                doc.SetDataSource(ds);

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
