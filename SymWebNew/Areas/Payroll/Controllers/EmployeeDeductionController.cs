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
    public class EmployeeDeductionController : Controller
    {
        //
        // GET: /Payroll/EmployeeDeduction/

        EmployeeDeductionRepo _eaRepo;
        public ActionResult Index()
        {
            //return View(new EmployeeDeductionRepo().SelectAll());
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param, string code, string name)
        {
            _eaRepo = new EmployeeDeductionRepo();

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var deductionAmountFilter = Convert.ToString(Request["sSearch_5"]);
            var periodNameFilter = Convert.ToString(Request["sSearch_6"]);

            var amountFrom = 0;
            var amountTo = 0;
            if (deductionAmountFilter.Contains('~'))
            {
                amountFrom = deductionAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(deductionAmountFilter.Split('~')[0]) == true ?  Convert.ToInt32(deductionAmountFilter.Split('~')[0]) : 0;
                amountTo = deductionAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(deductionAmountFilter.Split('~')[1]) == true ?  Convert.ToInt32(deductionAmountFilter.Split('~')[1]) : 0;
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

            var getAllData = _eaRepo.SelectAll();
            IEnumerable<EmployeeDeductionVM> filteredData;
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

                filteredData = getAllData
                   .Where(c => 
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.PeriodName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.DeductionAmount.ToString().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || (deductionAmountFilter != "" && deductionAmountFilter != "~") || periodNameFilter != "") 
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
                                            (amountFrom == 0 || amountFrom <= Convert.ToInt32(c.DeductionAmount))
                                            &&
                                            (amountTo == 0 || amountTo >= Convert.ToInt32(c.DeductionAmount)) 
                                            &&
                                            (periodNameFilter == "" || c.PeriodName.ToString().ToLower().Contains(periodNameFilter.ToLower()))
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
            Func<EmployeeDeductionVM, string> orderingFunction = (c => 
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                sortColumnIndex == 5 && isSortable_5 ? c.PeriodName :
                sortColumnIndex == 6 && isSortable_6 ? c.DeductionAmount.ToString() :
                sortColumnIndex ==7 && isSortable_7 ? c.Remarks :
                "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { 
                Convert.ToString(c.Id) 
                , c.Code //+ "~" + Convert.ToString(c.Id) 
                , c.EmpName 
                , c.Department 
                , c.Designation 
                , c.DeductionAmount.ToString()               
                , c.PeriodName.ToString()                
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
        public ActionResult EmployeeInfoForDeduction()
        {
            return View("_employee");
        }
        [Authorize]
        public ActionResult _EmployeeInfoForDeduction(JQueryDataTableParamModel param, string code, string name)
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
            var result = from c in displayedCompanies select new[] {   
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
        public ActionResult Create( string empId)
        {
            EmployeeDeductionVM vm = new EmployeeDeductionVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
           var tt= repo.SelectById(empId);
           vm.EmployeeId = empId;
           vm.EmpName =tt.Salutation_E+" "+tt.MiddleName+ " "+tt.LastName +"("+tt.Code +")";
           vm.DeductionAmount = 0;
           vm.DeductionDate = DateTime.Now.ToString("dd/MMM/yyyy");

           return View(vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(EmployeeDeductionVM vm)
        {
            string[] result = new string[6];
           try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
             //vm.FiscalYearDetailId = new FiscalYearRepo().FiscalPeriodIdByDate(Ordinary.DateToString(vm.DeductionDate));
             vm.DeductionDate = new FiscalYearRepo().FiscalPeriodStartDate(vm.FiscalYearDetailId);
             if (vm.FiscalYearDetailId == 0)
                if (vm.FiscalYearDetailId == 0)
                {
                    TempData["Msg"] = "Fail~Fiscal Year Not Exist on this Period";
                    FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                    return RedirectToAction("Index");

                }
                result = new EmployeeDeductionRepo().Insert(vm);
                TempData["Msg"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("EmployeeInfoForDeduction");

            }
            catch (Exception)
            {
                TempData["Msg"] = "Fail~Data Not ucceessfully";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            EmployeeDeductionVM vm = new EmployeeDeductionVM();
            EmployeeInfoRepo Erepo = new EmployeeInfoRepo();
            EmployeeDeductionRepo repo = new EmployeeDeductionRepo();
            vm = repo.SelectById(id);
            var tt = Erepo.SelectById(vm.EmployeeId);
            //vm.EmployeeId = vm.EmployeeId;
            vm.EmpName = tt.Salutation_E + " " + tt.MiddleName + " " + tt.LastName + "(" + tt.Code + ")";
            //vm.DeductionAmount = 0;
            //vm.DeductionDate = DateTime.Now.ToString("dd/MMM/yyyy");
            return View(vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(EmployeeDeductionVM vm)
        {
            string[] result = new string[6];
           try
            { ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMdd");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            //vm.FiscalYearDetailId = new FiscalYearRepo().FiscalPeriodIdByDate(Ordinary.DateToString(vm.DeductionDate));
            vm.DeductionDate = new FiscalYearRepo().FiscalPeriodStartDate(vm.FiscalYearDetailId);

            if (vm.FiscalYearDetailId == 0)
            {
                TempData["Msg"] = "Fail~Fiscal Year Not Exist on this Period";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
                result = new EmployeeDeductionRepo().Update(vm);
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
        public JsonResult EmployeeDeductionDelete(string ids)
        {
            EmployeeDeductionVM vm = new EmployeeDeductionVM();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMdd");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = new EmployeeDeductionRepo().Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
         public JsonResult GetPeriodName(string ids)
        {
            _eaRepo = new EmployeeDeductionRepo();
                 var result= _eaRepo.SelectAll().Select(m=>m.PeriodName.ToString());
            return Json(result, JsonRequestBehavior.AllowGet);
        }
         public ActionResult EmployeeDeductionListReport(string view, int? fid = null, string ProjectId = null, string DepartmentId = null, string SectionId = null, string DesignationId = null, string CodeF = null, string CodeT = null, string EmpName = null, string dojFrom = null, string dojTo = null)
         {
             try
             {
                 ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                 string EmployeeDeduction = "N";
                 if (!(User.IsInRole("Master") || User.IsInRole("Admin") || User.IsInRole("Account")))
                 {
                     //mgfOrganizationId = "";
                     //localSupplierOrganizationId = "";
                     //supplierOrganizationId = "";
                 }
                 ViewBag.EmployeeDeduction = EmployeeDeduction;
                 if (view == "Y")
                 {
                     return View();
                 }
                 ReportDocument doc = new ReportDocument();
                 List<EmployeeDeductionVM> getAllData = new List<EmployeeDeductionVM>();
                 getAllData = new EmployeeDeductionRepo().SelectAll(null,fid);
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
                 ds.Tables[0].TableName = "dtEmpDeduction";

                 string rptLocation = "";
                 rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"AllReports\\Payroll\\PayrollEntry\\rptEmployeeDeduction.rpt";
                 doc.Load(rptLocation);

                 doc.SetDataSource(ds);


                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";

                 doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";




                 //doc = new rptEmployeeDeduction();
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
