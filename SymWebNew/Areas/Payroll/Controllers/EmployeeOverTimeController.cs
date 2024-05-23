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
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Payroll.Controllers
{
    public class EmployeeOverTimeController : Controller
    {
        //
        // GET: /Payroll/EmployeeOverTime/

        EmployeeOverTimeRepo _eaRepo;
        public ActionResult Index()
        {
            //return View(new EmployeeOverTimeRepo().SelectAll());
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param, string code, string name)
        {
            _eaRepo = new EmployeeOverTimeRepo();

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var overTimeAmountFilter = Convert.ToString(Request["sSearch_5"]);
            var periodNameFilter = Convert.ToString(Request["sSearch_6"]);
            //var overTimeDateFilter = Convert.ToString(Request["sSearch_6"]);
            
            var amountFrom = 0;
            var amountTo = 0;
            if (overTimeAmountFilter.Contains('~'))
            {
                amountFrom = overTimeAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(overTimeAmountFilter.Split('~')[0]) == true ? Convert.ToInt32(overTimeAmountFilter.Split('~')[0]) :0;
                amountTo = overTimeAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(overTimeAmountFilter.Split('~')[1]) == true ? Convert.ToInt32(overTimeAmountFilter.Split('~')[1]) :0;

            }
               
            //DateTime fromDate = DateTime.MinValue;
            //DateTime toDate = DateTime.MaxValue;
            //if (overTimeDateFilter.Contains('~'))
            //{
            //    fromDate = overTimeDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(overTimeDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(overTimeDateFilter.Split('~')[0]): DateTime.MinValue;
            //    toDate = overTimeDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(overTimeDateFilter.Split('~')[1]) == true ?  Convert.ToDateTime(overTimeDateFilter.Split('~')[1]): DateTime.MinValue;
            //}

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

            var getAllData = _eaRepo.SelectAll();
            IEnumerable<EmployeeOverTimeVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.PeriodName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.OverTimeAmount.ToString().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || (overTimeAmountFilter != "" && overTimeAmountFilter != "~") || periodNameFilter != "") 
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                            &&
                                            (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                            &&
                                            (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                            &&
                                            (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                            //&&
                                            //(OverTimeAmountFilter == "" || c.OverTimeAmount.ToString().ToLower().Contains(OverTimeAmountFilter.ToLower()))
                                            &&
                                            (amountFrom == 0 || amountFrom <= Convert.ToInt32(c.OverTimeAmount))
                                            &&
                                            (amountTo == 0 || amountTo >= Convert.ToInt32(c.OverTimeAmount))
                                            //&&
                                            //(fromDate == DateTime.MinValue || fromDate <=  Convert.ToDateTime(c.OverTimeDate))
                                            //&&
                                            //(toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.OverTimeDate))
                                            &&
                                            (periodNameFilter == "" || c.PeriodName.ToLower().Contains(periodNameFilter.ToLower())) 
                                        );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeOverTimeVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Department :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.PeriodName :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.OverTimeAmount.ToString() :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.Remarks :
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
                , c.Code //+ "~" + Convert.ToString(c.Id) 
                , c.EmpName 
                , c.Department 
                , c.Designation 
                , c.OverTimeAmount.ToString()               
                , c.PeriodName                
                //, c.Remarks 
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
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult EmployeeInfoForOverTime()
        {
            return View("_Employee");
        }
        [Authorize]
        public ActionResult _EmployeeInfoForOverTime(JQueryDataTableParamModel param, string code, string name)
        {
            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_5"]);

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
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
            var getAllData = _empRepo.SelectAll();
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

                filteredData = getAllData
                   .Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.JoinDate.ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || (joinDateFilter != "" && joinDateFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                            &&
                                            (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                            &&
                                            (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                            &&
                                            (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                            &&
                                            (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
                                            &&
                                            (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.JoinDate))
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
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                sortColumnIndex == 5 && isSortable_5 ? c.JoinDate :
                sortColumnIndex == 6 && isSortable_6 ? c.Remarks :
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
                , c.Code //+ "~" + Convert.ToString(c.Id) 
                , c.EmpName 
                , c.Department 
                , c.Designation 
                , c.JoinDate.ToString()      
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
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create(string empId)
        {


            EmployeeOverTimeVM vm = new EmployeeOverTimeVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            var tt = repo.SelectById(empId);
            vm.EmployeeId = empId;
            vm.EmpName = tt.Salutation_E + " " + tt.MiddleName + " " + tt.LastName + "(" + tt.Code + ")";
            vm.OverTimeAmount = 0;
            vm.OverTimeDate = DateTime.Now.ToString("dd/MMM/yyyy");

            return View(vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(EmployeeInfoVM empVM)
        {
            EmployeeOverTimeVM vm = new EmployeeOverTimeVM();
            string[] result = new string[6];
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm = empVM.empovertime;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.OverTimeDate = new FiscalYearRepo().FiscalPeriodStartDate(vm.FiscalYearDetailId);
                if (vm.FiscalYearDetailId == 0)
                {
                    TempData["Msg"] = "Fail~Fiscal Year Not Exist on this Period";
                    FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                    return RedirectToAction("Index");
                }
                result = new EmployeeOverTimeRepo().Insert(vm);
                var mgs = result[0] + "~" + result[1];
                return Json(mgs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DetailCreate(string empcode = "", string btn = "current", string FiscalYearDetailId = "0")
        {
            string EmployeeId = "";
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeOverTimeRepo overtimeRepo = new EmployeeOverTimeRepo();

            EmployeeOverTimeVM OvertimeVM = new EmployeeOverTimeVM();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            vm = repo.SelectEmpForSearch(empcode, btn);
            if (vm.EmpName == null)
            {
                vm.EmpName = "Employee Name";
            }
            else
            {
                EmployeeId = vm.Id;
            }
            if (!string.IsNullOrWhiteSpace(vm.Id))
            {
                OvertimeVM = overtimeRepo.SelectByIdandFiscalyearDetail(vm.Id, FiscalYearDetailId);
                OvertimeVM.FiscalYearDetailId = Convert.ToInt32(FiscalYearDetailId);

            }
            if (string.IsNullOrWhiteSpace(FiscalYearDetailId))
            {
                FiscalYearDetailId = "0";
            }

            //svms = arerepo.SingleEmployeeEntry(EmployeeId, FiscalYearDetailId);
            vm.ovetimeVM = OvertimeVM;
            vm.arrearVM.EmployeeId = EmployeeId;
            return PartialView("_detailCreate", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            EmployeeOverTimeVM vm = new EmployeeOverTimeVM();
            EmployeeInfoRepo Erepo = new EmployeeInfoRepo();
            EmployeeOverTimeRepo repo = new EmployeeOverTimeRepo();
            vm = repo.SelectById(id);

            var tt = Erepo.SelectById(vm.EmployeeId);
            //vm.EmployeeId = vm.EmployeeId;
            vm.EmpName = tt.Salutation_E + " " + tt.MiddleName + " " + tt.LastName + "(" + tt.Code + ")";
            //vm.OverTimeAmount = 0;
            //vm.OverTimeDate = DateTime.Now.ToString("dd/MMM/yyyy");

            return View(vm);

        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(EmployeeOverTimeVM vm)
        {
            string[] result = new string[6];
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMdd");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;

                //vm.FiscalYearDetailId = new FiscalYearRepo().FiscalPeriodIdByDate(Ordinary.DateToString(vm.OverTimeDate));
                vm.OverTimeDate = new FiscalYearRepo().FiscalPeriodStartDate(vm.FiscalYearDetailId);

                if (vm.FiscalYearDetailId == 0)
                {
                    TempData["Msg"] = "Fail~Fiscal Year Not Exist on this Period";
                    FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                    return RedirectToAction("Index");
                }
                result = new EmployeeOverTimeRepo().Update(vm);
                TempData["Msg"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                TempData["Msg"] = "Fail~Data Not ucceessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult EmployeeOverTimeDelete(string ids)
        {
            EmployeeOverTimeVM vm = new EmployeeOverTimeVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMdd");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = new EmployeeOverTimeRepo().Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }


        public ActionResult EmployeeOverTimeListReport(string view, int? fid = null, string ProjectId = null, string DepartmentId = null, string SectionId = null, string DesignationId = null, string CodeF = null, string CodeT = null, string EmpName = null, string dojFrom = null, string dojTo = null)


        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                string EmployeeOverTime = "N";
                if (!(User.IsInRole("Master") || User.IsInRole("Admin") || User.IsInRole("Account")))
                {
                    //mgfOrganizationId = "";
                    //localSupplierOrganizationId = "";
                    //supplierOrganizationId = "";

                }
                ViewBag.EmployeeOverTime = EmployeeOverTime;
                if (view == "Y")
                {
                    return View();
                }
                ReportDocument doc = new ReportDocument();

                List<EmployeeOverTimeVM> getAllData = new List<EmployeeOverTimeVM>();
                getAllData = new EmployeeOverTimeRepo().SelectAll(null, fid);

                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                {
                    getAllData = getAllData.Where(x => x.ProjectId.Equals(ProjectId)).ToList();
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                {
                    getAllData = getAllData.Where(x => x.DepartmentId.Equals(DepartmentId)).ToList();

                }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                {
                    getAllData = getAllData.Where(x => x.SectionId.Equals(SectionId)).ToList();

                }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                {
                    getAllData = getAllData.Where(x => x.DesignationId.Equals(DesignationId)).ToList();

                }


                DataTable table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());

                DataSet ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtEmpOverTime";


                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"AllReports\\Payroll\\PayrollEntry\\rptEmployeeOverTime.rpt";
                doc.Load(rptLocation);

                doc.SetDataSource(ds);


                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";


                //doc = new rptEmployeeOverTime();
                //doc.SetDataSource(ds);

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
