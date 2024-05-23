using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymOrdinary;
using SymRepository.Attendance;
using SymRepository.Common;
using SymRepository.Enum;
using SymRepository.HRM;
using SymRepository.Payroll;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using SymViewModel.PF;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace SymWebUI.Areas.Payroll.Controllers
{
    public class SalaryProcessController : Controller
    {
        SymUserRoleRepo _reposur = new SymUserRoleRepo();

        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        //public delegate void EmpEmailProcess(DataSet ds, ReportDocument rptDoc, string FiscalPeriod);
        private static Thread thread;

        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            return View();
        }

        public ActionResult EmployeeSalary()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_50", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            return View();
        }

        public ActionResult SalaryInformation()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "process").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            return View();
        }

        public ActionResult _EmployeeInfo(JQueryDataTableParamVM param, string code, string name)
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
                fromDate = joinDateFilter.Split('~')[0] == ""
                    ? DateTime.MinValue
                    :
                    Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true
                        ?
                        Convert.ToDateTime(joinDateFilter.Split('~')[0])
                        : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == ""
                    ? DateTime.MaxValue
                    :
                    Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true
                        ?
                        Convert.ToDateTime(joinDateFilter.Split('~')[1])
                        : DateTime.MinValue;
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

            #region Column Filtering

            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" ||
                (joinDateFilter != "" && joinDateFilter != "~"))
            {
                filteredData = filteredData
                    .Where(c => (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                &&
                                (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                &&
                                (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                &&
                                (designationFilter == "" || c.Designation.ToString().ToLower()
                                    .Contains(designationFilter.ToLower()))
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
                sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.JoinDate) :
                sortColumnIndex == 6 && isSortable_6 ? c.Remarks :
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
                    Convert.ToString(c.Id), c.Code //+ "~" + Convert.ToString(c.Id) 
                    ,
                    c.EmpName, c.Department, c.Designation, c.JoinDate.ToString()
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

        public ActionResult _EmployeePF(JQueryDataTableParamVM param, string code, string empID)
        {
            #region Column Search

            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var EmployeeNameFilter = Convert.ToString(Request["sSearch_1"]);
            var BasicSalaryFilter = Convert.ToString(Request["sSearch_2"]);
            var GrossSalaryFilter = Convert.ToString(Request["sSearch_3"]);
            var PFValueFilter = Convert.ToString(Request["sSearch_4"]);
            var BasicamountFrom = 0;
            var BasicamountTo = 0;
            if (BasicSalaryFilter.Contains('~'))
            {
                BasicamountFrom = BasicSalaryFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(BasicSalaryFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(BasicSalaryFilter.Split('~')[0])
                        : 0;
                BasicamountTo = BasicSalaryFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(BasicSalaryFilter.Split('~')[1]) == true
                        ?
                        Convert.ToInt32(BasicSalaryFilter.Split('~')[1])
                        : 0;
            }

            var GrossamountFrom = 0;
            var GrossamountTo = 0;
            if (GrossSalaryFilter.Contains('~'))
            {
                GrossamountFrom = GrossSalaryFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(GrossSalaryFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(GrossSalaryFilter.Split('~')[0])
                        : 0;
                GrossamountTo = GrossSalaryFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(GrossSalaryFilter.Split('~')[1]) == true
                        ?
                        Convert.ToInt32(GrossSalaryFilter.Split('~')[1])
                        : 0;
            }

            var PFamountFrom = 0;
            var PFamountTo = 0;
            if (PFValueFilter.Contains('~'))
            {
                PFamountFrom = PFValueFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(PFValueFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(PFValueFilter.Split('~')[0])
                        : 0;
                PFamountTo = PFValueFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(PFValueFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(PFValueFilter.Split('~')[0])
                        : 0;
            }

            #endregion

            SalaryProcessRepo _salaryProcessRepo = new SalaryProcessRepo();
            var getAllData = _salaryProcessRepo.EmployeeSalaryPF(empID);
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
                filteredData = getAllData
                    .Where(c => isSearchable1 && c.FiscalPeriod.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable2 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable3 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable4 && c.PFValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable5 && c.Remarks.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            #region Column Filtering

            if (EmployeeNameFilter != "" || (BasicSalaryFilter != "" && BasicSalaryFilter != "~") ||
                (GrossSalaryFilter != "" && GrossSalaryFilter != "~") || (PFValueFilter != "" && PFValueFilter != "~"))
            {
                filteredData = filteredData
                    .Where(c => (EmployeeNameFilter == "" ||
                                 c.FiscalPeriod.ToLower().Contains(EmployeeNameFilter.ToLower()))
                                &&
                                (BasicamountFrom == 0 || BasicamountFrom <= Convert.ToInt32(c.BasicSalary))
                                &&
                                (BasicamountTo == 0 || BasicamountTo >= Convert.ToInt32(c.BasicSalary))
                                &&
                                (GrossamountFrom == 0 || GrossamountFrom <= Convert.ToInt32(c.GrossSalary))
                                &&
                                (GrossamountTo == 0 || GrossamountTo >= Convert.ToInt32(c.GrossSalary))
                                &&
                                (PFamountFrom == 0 || PFamountFrom <= Convert.ToInt32(c.PFValue))
                                &&
                                (PFamountTo == 0 || PFamountTo >= Convert.ToInt32(c.PFValue))
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
                sortColumnIndex == 1 && isSortable_1 ? c.FiscalPeriod :
                sortColumnIndex == 2 && isSortable_2 ? c.BasicSalary.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.GrossSalary.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.PFValue.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.Remarks :
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
                    Convert.ToString(c.Id), c.FiscalPeriod.ToString(), c.BasicSalary.ToString(),
                    c.GrossSalary.ToString(), c.PFValue.ToString()
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

        public ActionResult _EmployeeTax(JQueryDataTableParamVM param, string code, string empID)
        {
            #region Column Search

            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var FiscalPeriodFilter = Convert.ToString(Request["sSearch_1"]);
            var BasicSalaryFilter = Convert.ToString(Request["sSearch_2"]);
            var GrossSalaryFilter = Convert.ToString(Request["sSearch_3"]);
            var TaxValueFilter = Convert.ToString(Request["sSearch_4"]);
            var BasicamountFrom = 0;
            var BasicamountTo = 0;
            if (BasicSalaryFilter.Contains('~'))
            {
                BasicamountFrom = BasicSalaryFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(BasicSalaryFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(BasicSalaryFilter.Split('~')[0])
                        : 0;
                BasicamountTo = BasicSalaryFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(BasicSalaryFilter.Split('~')[1]) == true
                        ?
                        Convert.ToInt32(BasicSalaryFilter.Split('~')[1])
                        : 0;
            }

            var GrossamountFrom = 0;
            var GrossamountTo = 0;
            if (GrossSalaryFilter.Contains('~'))
            {
                GrossamountFrom = GrossSalaryFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(GrossSalaryFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(GrossSalaryFilter.Split('~')[0])
                        : 0;
                GrossamountTo = GrossSalaryFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(GrossSalaryFilter.Split('~')[1]) == true
                        ?
                        Convert.ToInt32(GrossSalaryFilter.Split('~')[1])
                        : 0;
            }

            var TaxamountFrom = 0;
            var TaxamountTo = 0;
            if (TaxValueFilter.Contains('~'))
            {
                TaxamountFrom = TaxValueFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(TaxValueFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(TaxValueFilter.Split('~')[0])
                        : 0;
                TaxamountTo = TaxValueFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(TaxValueFilter.Split('~')[1]) == true
                        ?
                        Convert.ToInt32(TaxValueFilter.Split('~')[1])
                        : 0;
            }

            #endregion

            SalaryProcessRepo _salaryProcessRepo = new SalaryProcessRepo();
            var getAllData = _salaryProcessRepo.EmployeeSalaryTax(empID);
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
                filteredData = getAllData
                    .Where(c => isSearchable1 && c.FiscalPeriod.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable2 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable3 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable4 && c.TaxValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable5 && c.Remarks.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            #region Column Filtering

            if (FiscalPeriodFilter != "" || (BasicSalaryFilter != "" && BasicSalaryFilter != "~") ||
                (GrossSalaryFilter != "" && GrossSalaryFilter != "~") ||
                (TaxValueFilter != "" && TaxValueFilter != "~"))
            {
                filteredData = filteredData
                    .Where(c => (FiscalPeriodFilter == "" ||
                                 c.FiscalPeriod.ToLower().Contains(FiscalPeriodFilter.ToLower()))
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
                sortColumnIndex == 1 && isSortable_1 ? c.FiscalPeriod :
                sortColumnIndex == 2 && isSortable_2 ? c.BasicSalary.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.GrossSalary.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.TaxValue.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.Remarks :
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
                    Convert.ToString(c.Id), c.FiscalPeriod.ToString(), c.BasicSalary.ToString(),
                    c.GrossSalary.ToString(), c.TaxValue.ToString()
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

        public ActionResult _EmployeeEarning(JQueryDataTableParamVM param, string code, string empID)
        {
            #region Column Search

            var FiscalPeriodFilter = Convert.ToString(Request["sSearch_0"]);
            var SalaryTypeFilter = Convert.ToString(Request["sSearch_1"]);
            var AmountFilter = Convert.ToString(Request["sSearch_2"]);
            var amountFrom = 0;
            var amountTo = 0;
            if (AmountFilter.Contains('~'))
            {
                amountFrom = AmountFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(AmountFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(AmountFilter.Split('~')[0])
                        : 0;
                amountTo = AmountFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(AmountFilter.Split('~')[1]) == true
                        ?
                        Convert.ToInt32(AmountFilter.Split('~')[1])
                        : 0;
            }

            #endregion

            SalaryProcessRepo _salaryProcessRepo = new SalaryProcessRepo();
            var getAllData = _salaryProcessRepo.EmployeeSalaryEarning(empID);
            IEnumerable<EmployeeInfoVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                    .Where(c => isSearchable1 && c.FiscalPeriod.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable2 && c.SalaryType.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable3 && c.Amount.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable4 && c.Remarks.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            #region Column Filtering

            if (FiscalPeriodFilter != "" || SalaryTypeFilter != "" || (AmountFilter != "" && AmountFilter != "~"))
            {
                filteredData = filteredData
                    .Where(c => (FiscalPeriodFilter == "" ||
                                 c.FiscalPeriod.ToLower().Contains(FiscalPeriodFilter.ToLower()))
                        &&
                        SalaryTypeFilter == "" || c.SalaryType.ToLower().Contains(SalaryTypeFilter.ToLower())
                        &&
                        (amountFrom == 0 || amountFrom <= Convert.ToInt32(c.Amount))
                        &&
                        (amountTo == 0 || amountTo >= Convert.ToInt32(c.Amount))
                    );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.FiscalPeriod :
                sortColumnIndex == 2 && isSortable_2 ? c.SalaryType.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.Amount.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.Remarks.ToString() :
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
                    c.FiscalPeriod.ToString(), c.SalaryType.ToString(), c.Amount.ToString(), c.Remarks
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

        public ActionResult _EmployeeLoan(JQueryDataTableParamVM param, string code, string empID)
        {
            #region Column Search

            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var FiscalPeriodFilter = Convert.ToString(Request["sSearch_1"]);
            var loanAmountFilter = Convert.ToString(Request["sSearch_2"]);
            var loanAmountFrom = 0;
            var loanAmountTo = 0;
            if (loanAmountFilter.Contains('~'))
            {
                loanAmountFrom = loanAmountFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(loanAmountFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(loanAmountFilter.Split('~')[0])
                        : 0;
                loanAmountTo = loanAmountFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(loanAmountFilter.Split('~')[1]) == true
                        ?
                        Convert.ToInt32(loanAmountFilter.Split('~')[1])
                        : 0;
            }

            #endregion Column Search

            SalaryProcessRepo _salaryProcessRepo = new SalaryProcessRepo();
            var getAllData = _salaryProcessRepo.EmployeeSalaryLoan(empID);
            IEnumerable<EmployeeInfoVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                filteredData = getAllData
                    .Where(c => isSearchable1 && c.FiscalPeriod.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable2 && c.Amount.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable3 && c.Remarks.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            #region Column Filtering

            if (FiscalPeriodFilter != "" || (loanAmountFilter != "" && loanAmountFilter != "~"))
            {
                filteredData = filteredData
                    .Where(c =>
                        (FiscalPeriodFilter == "" || c.FiscalPeriod.ToLower().Contains(FiscalPeriodFilter.ToLower()))
                        &&
                        (loanAmountFrom == 0 || loanAmountFrom <= Convert.ToInt32(c.Amount))
                        &&
                        (loanAmountTo == 0 || loanAmountTo >= Convert.ToInt32(c.Amount))
                    );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.FiscalPeriod :
                sortColumnIndex == 2 && isSortable_2 ? c.Amount.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.Remarks.ToString() :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                select new[] { c.FiscalPeriod.ToString(), c.Amount.ToString(), c.Remarks };
            return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = getAllData.Count(),
                    iTotalDisplayRecords = filteredData.Count(),
                    aaData = result
                },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmployeeSalaryTab(string employeeID)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_50", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(employeeID);
            ViewBag.empID = employeeID;
            ViewBag.emp = vm.Salutation_E.ToString() + " " + vm.MiddleName.ToString() + " " + vm.LastName.ToString();
            return View();
        }

        public ActionResult ProcessDelete(string fid, string ProjectId, string DepartmentId, string SectionId,
            string DesignationId, string CodeF, string CodeT)
        {
            string[] result = new string[6];
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "delete").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                    vProjectId = ProjectId;
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                    vDepartmentId = DepartmentId;
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                    vSectionId = SectionId;
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
                    vDesignationId = DesignationId;
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    vCodeF = CodeF;
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    vCodeT = CodeT;
                SalaryProcessRepo repo = new SalaryProcessRepo();
                result = repo.DeleteProcess(vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, fid);
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChildAllowanceDetail(string fid, string ProjectId, string DepartmentId, string SectionId,
            string DesignationId, string CodeF, string CodeT, string view)
        {
            string[] result = new string[6];
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

                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                    vProjectId = ProjectId;
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                    vDepartmentId = DepartmentId;
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                    vSectionId = SectionId;
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
                    vDesignationId = DesignationId;
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    vCodeF = CodeF;
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    vCodeT = CodeT;
                ReportDocument doc = new ReportDocument();
                SalaryProcessRepo _empRepo = new SalaryProcessRepo();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                //call emp view model
                ds = _empRepo.ChildAllowanceDetail(fid, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF,
                    vCodeT);
                dt = ds.Tables[0];
                ds.Tables[0].TableName = "dtChildAllowance";
                var FullPeriodName = Convert.ToDateTime(ds.Tables[0].Rows[0]["PeriodName"].ToString())
                    .ToString("MMMM-yyyy");
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory +
                              @"Files\ReportFiles\Payroll\PayrollProcess\rptChildAllowanceList.rpt";
                doc.Load(rptLocation);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["FullPeriodName"].Text = "'" + FullPeriodName + "'";
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

        public ActionResult SalaryPreProces(int FiscalPeriodDetailId, string PId, string DId, string SId, string DesId,
            string CF, string CT, string SP)
        {
            string[] result = new string[6];
            try
            {
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "process").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                FiscalYearVM vm = new FiscalYearVM();
                SalaryProcessRepo repo = new SalaryProcessRepo();
                EmployeeInfoRepo repoEmp = new EmployeeInfoRepo();
                if (SP == "ALL")
                {
                    SP = "";
                }

                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vEmployeeIdF = "0_0";
                string vEmployeeIdT = "0_0";
                if (PId != "0_0" && PId != "0" && PId != "" && PId != "null" && PId != null)
                    vProjectId = PId;
                if (DId != "0_0" && DId != "0" && DId != "" && DId != "null" && DId != null)
                    vDepartmentId = DId;
                if (SId != "0_0" && SId != "0" && SId != "" && SId != "null" && SId != null)
                    vSectionId = SId;
                if (DesId != "0_0" && DesId != "0" && DesId != "" && DesId != "null" && DesId != null)
                    vDesignationId = DesId;
                if (CF != "0_0" && CF != "0" && CF != "" && CF != "null" && CF != null)
                {
                    var emp = repoEmp.SelectEmpForSearch(CF, "current");
                    if (!string.IsNullOrWhiteSpace(emp.Id))
                    {
                        vEmployeeIdF = emp.Id;
                    }
                }

                if (CT != "0_0" && CT != "0" && CT != "" && CT != "null" && CT != null)
                {
                    var emp = repoEmp.SelectEmpForSearch(CT, "current");
                    if (!string.IsNullOrWhiteSpace(emp.Id))
                    {
                        vEmployeeIdT = emp.Id;
                    }
                }

                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.BranchId = Convert.ToInt32(identity.BranchId);
                result = repo.SalaryPreProcessNew(FiscalPeriodDetailId, vProjectId, vDepartmentId, vSectionId,
                    vDesignationId
                    , vEmployeeIdF, vEmployeeIdT, vm, SP, CompanyName);
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }

        //////public ActionResult SalarySheet(string fid, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
        //////    , string other1, string other2, string other3, string bankId
        //////    , string SheetName, string Orderby, string MulitpleProjectId, string MulitpleOther3, string view, string HoldStatus)

        public ActionResult SalarySheet(SalarySheetVM vm)
        {
            string[] result = new string[6];
            try
            {
                #region Try

                #region Objects and Variables
                 bool IsPayrollLock = false;
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                if (string.IsNullOrWhiteSpace(vm.View) || vm.View == "Y")
                {
                    vm.FiscalYearDetailId = Convert.ToInt32(Session["FiscalYearDetailId"]);
                    vm.HoldStatus = "Not Hold";
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

                //////List<string> ProjectIdList = new List<string>();
                //////List<string> Other3List = new List<string>();


                if (!string.IsNullOrWhiteSpace(vm.MultipleProjectId))
                {
                    vm.MultipleProjectId = vm.MultipleProjectId.Trim(',');
                    vm.ProjectIdList = vm.MultipleProjectId.Split(',').ToList();
                }

                if (!string.IsNullOrWhiteSpace(vm.MultipleOther3))
                {
                    vm.MultipleOther3 = vm.MultipleOther3.Trim(',');
                    vm.Other3List = vm.MultipleOther3.Split(',').ToList();
                }


                ReportDocument doc = new ReportDocument();
                SalaryProcessRepo _empRepo = new SalaryProcessRepo();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();

                #endregion

                if (CompanyName.ToUpper() == "TIB" || CompanyName.ToUpper() == "BOLLORE" || CompanyName.ToUpper() == "G4S") //tib
                {
                    if (vm.SheetName == "SalarySheet3")
                    {
                        string[] conditionFields = new string[]
                            { "ProjectId", "SectionId", "DesignationId", "FiscalYearDetailId" };
                        string[] conditionValues = new string[]
                            { vm.ProjectId, vm.SectionId, vm.DesignationId, vm.FiscalYearDetailId.ToString() };

                        ds = _empRepo.SalarySheet_TIB_SummeryOther(vm, conditionFields, conditionValues);
                        dt = ds.Tables[0];
                    }
                    else if (vm.SheetName == "SalarySheet2")
                    {
                        #region Excel DownLoad

                        //ds = _empRepo.SalarySheet_TIB_Excel(vm.FiscalYearDetailId.ToString(), vm);

                        //ExcelPackage excel = new ExcelPackage();
                        //var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                        //ExcelSheetFormat(ds.Tables[0], workSheet, new[] {"Salary Sheet(Summary)"});

                        //using (var memoryStream = new MemoryStream())
                        //{
                        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        //    Response.AddHeader("content-disposition", "attachment;  filename=" + "SalarySheet(Summary)" + ".xlsx");
                        //    excel.SaveAs(memoryStream);
                        //    memoryStream.WriteTo(Response.OutputStream);
                        //    Response.Flush();
                        //    Response.End();
                        //}


                        //result[0] = "Success";
                        //result[1] = "Successful~Data Download";

                        //Session["result"] = result[0] + "~" + result[1];
                        //return Redirect("SalarySheet");

                        #endregion

                        doc.PrintOptions.PaperSize = PaperSize.PaperLegal;

                        string[] condFields = { "FiscalYearDetailId" };
                        string[] condValues = { vm.FiscalYearDetailId.ToString() };
                        //ds = _empRepo.SalarySheet_TIB(condFields, condValues, vm.SheetName, vm.FiscalYearDetailId.ToString(), vm);
                        ds = _empRepo.SalarySheet_TIB_Excel(vm);
                        dt = ds.Tables[0];
                        if (dt.Rows.Count == 0)
                        {
                            result[0] = "Fail";
                            result[1] = "No Data Found";
                            Session["result"] = result[0] + "~" + result[1];

                            return View(vm);
                        }
                    }
                    else if (vm.SheetName == "Send Salary to Sage")
                    {
                        doc.PrintOptions.PaperSize = PaperSize.PaperLegal;

                        string[] condFields = { "FiscalYearDetailId" };
                        string[] condValues = { vm.FiscalYearDetailId.ToString() };
                        //ds = _empRepo.SalarySheet_TIB(condFields, condValues, vm.SheetName, vm.FiscalYearDetailId.ToString(), vm);
                        var resultVm = _empRepo.PostToSage(vm);


                        result[0] = resultVm.Status;
                        result[1] = resultVm.Message;
                        Session["result"] = result[0] + "~" + result[1];

                        return View(vm);

                    }
                    else
                    {
                        doc.PrintOptions.PaperSize = PaperSize.PaperLegal;                       
                        string[] condFields = { "FiscalYearDetailId" };
                        string[] condValues = { vm.FiscalYearDetailId.ToString() };
                        ds = _empRepo.SalarySheet_TIB(condFields, condValues, vm.SheetName,
                            vm.FiscalYearDetailId.ToString(), vm);
                        dt = ds.Tables[0];
                        dt1 = ds.Tables[1];

                       
                        if (dt.Rows.Count == 0)
                        {
                            result[0] = "Fail";
                            result[1] = "No Data Found";
                            Session["result"] = result[0] + "~" + result[1];

                            return View(vm);
                        }

                        if (CompanyName.ToUpper() == "BOLLORE")
                        {
                            IsPayrollLock = new FiscalYearRepo().SelectAll_FiscalYearDetail(Convert.ToInt32(vm.FiscalYearDetailId)).FirstOrDefault().PayrollLock;
                            if (vm.SheetName == "SalarySheet1")
                            {
                                dt.DefaultView.Sort = "Code";
                                dt = dt.DefaultView.ToTable();

                                dt.Columns["TransportAllowance"].ColumnName = "SPECIALALLOWANCE";
                        var toRemove = new string[] { "Section", "Name", "TIN", "BankAccountName", "Routing_No", "SectionOrder", "FiscalYearDetailId", "EmployeeId", "ProjectId", "DepartmentId", "SectionId", "DesignationId"
                    , "IsHold", "TransportBill", "Stamp", "PFEmployer", "PFLoan", "STAFFWELFARE", "DeductionTotal", "NetSalary", "Othere(OT)", "Vehicle(Adj)", "Other(Bonus)", "LeaveWOPay","GP","Travel","ChildAllowance","MOBILE(Allowance)"
                    ,"OtherAdjustment","NetPayment","SecOrderNo","Expr1","DesignationGroupName","OrderNo","StepName","TotalAdjustment","Medical"};
                        foreach (string col in toRemove)
                        {
                            dt.Columns.Remove(col);
                        }
                        dt.Columns.AddRange(new DataColumn[]
                     {
                     new DataColumn("PFEMPLOYER"),
                     new DataColumn("BONUSPROV"),
                     new DataColumn("LEAVEPROV"),
                     new DataColumn("GRATUITYPROV"),
                     new DataColumn("MEDICALPROV"),
                     new DataColumn("CTC"),
                     new DataColumn("EPFDED"),
                     new DataColumn("TDS"),
                     new DataColumn("TOTALDEDUCTION"),
                     new DataColumn("PAID"),
                     new DataColumn("FESTIVALBONUS2"),
                     new DataColumn("TOTAL"),
                     new DataColumn("EMPLOYER+EMPLOYEE"),
                     new DataColumn("BONUSPROV1"),
                     new DataColumn("LEAVEPROV1"),
                     new DataColumn("GRATUITYPROV1"),
                     new DataColumn("MEDICALPROV1"),
                     });

                        foreach (DataRow rows in dt.Rows)
                        {
                            decimal v = Convert.ToDecimal(rows["Basic"]) * (10 / 100.00M);

                            //decimal v=(Convert.ToDecimal(rows["Basic"]) * 10)/100;
                            rows["PFEMPLOYER"] = Convert.ToDecimal(rows["Basic"]) * (10 / 100.00M);
                            rows["BONUSPROV"] = Convert.ToDecimal(rows["Basic"]) * (25 / 100.00M);
                            rows["LEAVEPROV"] = (Convert.ToDecimal(rows["Gross"]) / 30 * 15) / 12;
                            rows["GRATUITYPROV"] = Convert.ToDecimal(rows["Basic"]) / 12;
                            if (rows["Grade"].ToString() == "C3" ||rows["Grade"].ToString() == "C2" || rows["Grade"].ToString() == "C1" || rows["Grade"].ToString() == "A1"
                                || rows["Grade"].ToString() == "A2" || rows["Grade"].ToString() == "A3" || rows["Grade"].ToString() == "B1-M3" || rows["Grade"].ToString() == "B1-M1"
                                )
                            {
                                var basic = Convert.ToDecimal(rows["Basic"]);
                                var Grade = rows["Grade"].ToString();
                                if (Convert.ToDecimal(rows["Basic"]) <= 50000.00M)
                                {
                                    rows["MEDICALPROV"] = (Convert.ToDecimal(rows["Basic"])/12); 
                                }
                                else
                                {
                                    rows["MEDICALPROV"] = (Convert.ToDecimal(50000) / 12); 
                                }

                            }
                            else
                            {
                                rows["MEDICALPROV"] = (Convert.ToDecimal(10000) / 12); 
                            }
                            rows["CTC"] = Convert.ToDecimal(rows["Gross"]) + Convert.ToDecimal(rows["PFEMPLOYER"]) + Convert.ToDecimal(rows["BONUSPROV"]) + Convert.ToDecimal(rows["GRATUITYPROV"]) + Convert.ToDecimal(rows["LEAVEPROV"]) + Convert.ToDecimal(rows["MEDICALPROV"]);
                            rows["EPFDED"] = Convert.ToDecimal(rows["PFEMPLOYER"]) * -1;
                            rows["TDS"] = Convert.ToDecimal(rows["TAX"]) * -1;
                            rows["TOTALDEDUCTION"] = (Convert.ToDecimal(rows["EPFDED"]) + Convert.ToDecimal(rows["TDS"])) * -1;
                            rows["PAID"] = (Convert.ToDecimal(rows["Gross"]) - Convert.ToDecimal(rows["TOTALDEDUCTION"])) * -1;
                            rows["FESTIVALBONUS2"] = 0;
                            rows["TOTAL"] = (Convert.ToDecimal(rows["PAID"]) + Convert.ToDecimal(rows["FESTIVALBONUS2"])) * -1;
                            rows["EMPLOYER+EMPLOYEE"] = (Convert.ToDecimal(rows["PFEMPLOYER"]) + Convert.ToDecimal(rows["PFEMPLOYER"])) * -1;

                            rows["BONUSPROV1"] = Convert.ToDecimal(rows["BONUSPROV"]) * -1;
                            rows["LEAVEPROV1"] = Convert.ToDecimal(rows["LEAVEPROV"]) * -1;
                            rows["GRATUITYPROV1"] = Convert.ToDecimal(rows["GRATUITYPROV"]) * -1;
                            rows["MEDICALPROV1"] = Convert.ToDecimal(rows["MEDICALPROV"]) * -1;
                            
                        }
                        dt = Ordinary.DtValueRound(dt, new[] { "MEDICALPROV1", "GRATUITYPROV1", "LEAVEPROV1", "EMPLOYER+EMPLOYEE", "TOTAL" ,"FESTIVALBONUS2","PAID" 
                        ,"TOTALDEDUCTION" , "TDS" ,"EPFDED","CTC" ,"GRATUITYPROV","LEAVEPROV" ,"BONUSPROV","PFEMPLOYER","MEDICALPROV"
                        });
                            }
                        }
                    }
                }

                else
                {  

                    dt = _empRepo.SalarySheet(vm);

                    if (dt.Rows.Count == 0)
                    {
                        result[0] = "Fail";
                        result[1] = "No Data Found";
                        Session["result"] = result[0] + "~" + result[1];

                        return View(vm);
                    }

                    if (CompanyName.ToLower() == "kbl" || CompanyName.ToLower() == "anupam" ||
                        CompanyName.ToLower() == "kajol" || CompanyName.ToLower() == "ssl")
                    {
                        #region kajol

                        #region Notes

                        ////////----------------Last Update - 08 November 2018--------------
                        ////////ReportId	    ReportName
                        ////////SalarySheet1	Full Salary --------------(Ready) 
                        ////////SalarySheet2	Bank Pay    --------------(Ready)	
                        ////////SalarySheet3	Cash Pay    --------------(Ready)	
                        ////////SalarySheet4	Bank Sheet  --------------(Ready)

                        #endregion

                        #region Main Top

                        if (vm.SheetName == "SalarySheet100") //--------Main Top SalarySheet
                        {
                            dt = new DataTable();
                            dt = _empRepo.SalarySheetMainTop(vm);
                        }

                        #endregion

                        if (!string.IsNullOrWhiteSpace(vm.HoldStatus) && vm.HoldStatus.ToLower() == "hold")
                        {
                            vm.HoldStatus = "Held Up ";
                        }
                        else
                        {
                            vm.HoldStatus = "";
                        }

                        #endregion
                    }
                }

                #region Report Call

                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();
                string[] conFields = { "ReportType", "ReportId" };
                string[] conValues = { "SalarySheet", vm.SheetName };
                enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);
            
                string ReportFileName = enumReportVMs.FirstOrDefault().ReportFileName;
                string ReportName = enumReportVMs.FirstOrDefault().Name;

                SettingRepo _sRepo = new SettingRepo();
                if (CompanyName.ToUpper() == "TIB" || CompanyName.ToUpper() == "BOLLORE" || CompanyName.ToUpper() == "G4S") //tib
                {
                    //dt.TableName = "dtSalary";
                    dt.TableName = "TIBSalary";
                    if (vm.SheetName == "SalarySheet5" || vm.SheetName == "SalarySheet6")
                    {
                        dt1.TableName = "DtTIBOtherHead";
                    }

                    if (vm.SheetName == "SalarySheet3")
                    {
                        dt.TableName = "dtSalarySummary";
                    }
                }
                else
                {
                    dt.TableName = "dtSalarySheet";                   
                }

                FiscalYearDetailVM fydVM = new FiscalYearRepo().FYPeriodDetail(Convert.ToInt32(vm.FiscalYearDetailId))
                    .FirstOrDefault();

                string PeriodName = fydVM.PeriodName;

                string FullPeriodName = Convert.ToDateTime("01-" + PeriodName).ToString("MMMM-yyyy");


                if (vm.IsMultipleSalary)
                {
                    fydVM = new FiscalYearRepo().FYPeriodDetail(Convert.ToInt32(vm.FiscalYearDetailIdTo))
                        .FirstOrDefault();

                    string PeriodNameTo = fydVM.PeriodName;

                    string FullPeriodNameTo = Convert.ToDateTime("01-" + PeriodNameTo).ToString("MMMM-yyyy");

                    FullPeriodName = FullPeriodName + " to " + FullPeriodNameTo;
                }
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                string rptLocation = "";
               // rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\RptSalarySheet.rpt";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" + ReportFileName + ".rpt";

                doc.Load(rptLocation);

                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["FullPeriodName"].Text = "'" + FullPeriodName + "'";
                doc.DataDefinition.FormulaFields["HoldStatus"].Text = "'" + vm.HoldStatus + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'"; 

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = doc.DataDefinition.FormulaFields;
                CommonWebMethod _CommonWebMethod = new CommonWebMethod();        
             
                if(!IsPayrollLock)
                {
                    _CommonWebMethod.FormulaField(doc, crFormulaF, "Preview", "Preview Only");
                }

                if (CompanyName.ToLower() == "kbl" || CompanyName.ToLower() == "anupam" ||
                    CompanyName.ToLower() == "kajol" || CompanyName.ToLower() == "ssl")
                {
                    if (ReportFileName == "RptSalarySheet_AnupamBank")
                    {
                        ////AccountNo Before September-2020 - 101-110-44945
                        ////AccountNo From September-2020 - 101-110-52517
                        string AccountNo_Anupam = "101-110-44945";

                        if (Convert.ToDateTime("01-" + PeriodName) >= Convert.ToDateTime("01-Sep-2020"))
                        {
                            AccountNo_Anupam = "101-110-52517";
                        }

                        _CommonWebMethod.FormulaField(doc, crFormulaF, "AccountNo", AccountNo_Anupam);
                    }
                }

                #endregion
               
                if (CompanyName.ToUpper() == "TIB" || CompanyName.ToUpper() == "BOLLORE" || CompanyName.ToUpper() == "G4S") //tib
                {
                    if (vm.SheetName.ToLower() == "salarysheet6")
                    {
                        if(CompanyName.ToUpper() == "BOLLORE")
                        {
                            thread = new Thread(unused => EmpEmailProcessBollore(ds, doc, FullPeriodName));                        
                        }
                        else
                        {
                          thread = new Thread(unused => EmpEmailProcessTIB(ds, doc, FullPeriodName));
                        }                   
                        thread.Start();
                        // EmpEmailProcess(ds, doc, FullPeriodName)
                        result[0] = "Successfully";
                        result[1] = "Pay Slip Email Sent";
                        Session["result"] = result[0] + "~" + result[1];
                        //return Redirect("/Acc/Home/");
                        return View(vm);
                    }
                }
                else
                {
                    if (vm.SheetName.ToLower() == "pay slip (email)")
                    {
                        EmailSettings emailsettings = new EmailSettings();
                        thread = new Thread(unused => EmpEmailProcess(dt, doc, FullPeriodName));
                        thread.Start();
                        // EmpEmailProcess(ds, doc, FullPeriodName)
                        result[0] = "Successfully";
                        result[1] = "Pay Slip Email Sent";
                        Session["result"] = result[0] + "~" + result[1];
                        //return Redirect("/Acc/Home/");
                        return View(vm);
                    }
                }


                if (CompanyName.ToUpper() == "TIB" || CompanyName.ToUpper() == "BOLLORE" || CompanyName.ToUpper() == "G4S") //tib
                {
                    if (vm.SheetName == "SalarySheet5")
                    {
                        if (CompanyName.ToUpper() == "G4S")
                        {
                            ds.Tables[0].TableName = "TIBSalary";
                            doc.SetDataSource(ds.Tables[0]);
                        }
                        else
                        {
                            doc.SetDataSource(ds);
                        }
                    }

                    
                    else
                    {
                        doc.SetDataSource(dt);
                    }
                }

                else
                {   //this is temporary
                    if (CompanyName.ToUpper() == "SSL")
                    {
                        if (vm.SheetName.ToLower() == "salarysheet2")
                        {
                            dt = dt.AsEnumerable()
                                .Where(row => row.Field<string>("BankAccountNo") != "NA")
                                .CopyToDataTable();
                        }
                        if (vm.SheetName.ToLower() == "salarysheet3")
                        {
                            dt = dt.AsEnumerable()
                                .Where(row => row.Field<string>("BankAccountNo") == "NA")
                                .CopyToDataTable();
                        }
                       
                    }
                   
                    doc.SetDataSource(dt);
                }

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
                FileLogger.Log("SaleSheet", this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);

                return View(vm);
            }
        }

        public ActionResult SalarySheetEmail(string fid, string ProjectId, string DepartmentId, string SectionId,
            string DesignationId, string CodeF, string CodeT)
        {
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                    vProjectId = ProjectId;
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                    vDepartmentId = DepartmentId;
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                    vSectionId = SectionId;
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
                    vDesignationId = DesignationId;
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    vCodeF = CodeF;
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    vCodeT = CodeT;
                ReportDocument doc = new ReportDocument();
                SalaryProcessRepo _empRepo = new SalaryProcessRepo();
                DataSet ds = new DataSet();
                DataSet dsEmail = new DataSet();
                string rptLocation = "";
                //call emp view model
                int cnt = 1;

                SalarySheetVM vm = new SalarySheetVM();
                ds = _empRepo.SalaryPreCalculationNew(vm);

                ////ds = _empRepo.SalaryPreCalculationNew(fid, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, "", "", "", "", "", null);


                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    ////dsEmail = _empRepo.SalaryPreCalculationNew(fid, vProjectId, vDepartmentId, vSectionId, vDesignationId, item["Code"].ToString(), item["Code"].ToString(), "", "", "", "", "", null);
                    dsEmail = _empRepo.SalaryPreCalculationNew(vm);
                    dsEmail.Tables[0].TableName = "dtSalarySheet";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory +
                                  @"Files\ReportFiles\Payroll\Salary\RptPaySlipNew.rpt";
                    //rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"AllReports\\Payroll\\Salary\\RptPaySlipNew.rpt";
                    doc.Load(rptLocation);
                    doc.SetDataSource(dsEmail);
                    string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                    doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                    var rpt = RenderReportAsPDF(doc);
                   // EmailSettings emailsettings = new EmailSettings();
                    EmailSettingsBollore emailsettings = new EmailSettingsBollore();
                    //EmpEmailProcess(dt, doc);
                    doc.Close();
                    cnt++;
                }

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult SalaryApproveRequest(SalarySheetVM vm)
        {
            try
            {
                string[] result = new string[6];
                SalaryProcessRepo _empRepo = new SalaryProcessRepo();

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                vm.SheetName = "SalarySheet1";
                vm.Orderby = "CODE";
                string[] condFields = { "FiscalYearDetailId" };
                string[] condValues = { vm.FiscalYearDetailId.ToString() };
                ds = _empRepo.SalarySheet_TIB(condFields, condValues, vm.SheetName,
                    vm.FiscalYearDetailId.ToString(), vm);
                dt = ds.Tables[0];
                dt1 = ds.Tables[1];


                if (dt.Rows.Count == 0)
                {
                    result[0] = "Fail";
                    result[1] = "No Data Found";
                    Session["result"] = result[0] + "~" + result[1];

                    return Redirect("/Payroll/SalaryProcess/SalarySheet/");

                }

               
                ReportDocument doc = new ReportDocument();
                string rptLocation = "";
                //call emp view model
                int cnt = 1;

               
                        dt.DefaultView.Sort = "Code";
                        dt = dt.DefaultView.ToTable();

                        dt.Columns["TransportAllowance"].ColumnName = "SPECIALALLOWANCE";
                        var toRemove = new string[] { "Section", "Name", "TIN", "BankAccountName", "Routing_No", "SectionOrder", "FiscalYearDetailId", "EmployeeId", "ProjectId", "DepartmentId", "SectionId", "DesignationId"
                    , "IsHold", "TransportBill", "Stamp", "PFEmployer", "PFLoan", "STAFFWELFARE", "DeductionTotal", "NetSalary", "Othere(OT)", "Vehicle(Adj)", "Other(Bonus)", "LeaveWOPay","GP","Travel","ChildAllowance","MOBILE(Allowance)"
                    ,"OtherAdjustment","NetPayment","SecOrderNo","Expr1","DesignationGroupName","OrderNo","StepName","TotalAdjustment","Medical"};
                        foreach (string col in toRemove)
                        {
                            dt.Columns.Remove(col);
                        }
                        dt.Columns.AddRange(new DataColumn[]
                     {
                     new DataColumn("PFEMPLOYER"),
                     new DataColumn("BONUSPROV"),
                     new DataColumn("LEAVEPROV"),
                     new DataColumn("GRATUITYPROV"),
                     new DataColumn("MEDICALPROV"),
                     new DataColumn("CTC"),
                     new DataColumn("EPFDED"),
                     new DataColumn("TDS"),
                     new DataColumn("TOTALDEDUCTION"),
                     new DataColumn("PAID"),
                     new DataColumn("FESTIVALBONUS2"),
                     new DataColumn("TOTAL"),
                     new DataColumn("EMPLOYER+EMPLOYEE"),
                     new DataColumn("BONUSPROV1"),
                     new DataColumn("LEAVEPROV1"),
                     new DataColumn("GRATUITYPROV1"),
                     new DataColumn("MEDICALPROV1"),
                     });

                        foreach (DataRow rows in dt.Rows)
                        {

                            rows["PFEMPLOYER"] = Convert.ToDecimal(rows["Basic"]) * (10 / 100.00M);
                            rows["BONUSPROV"] = Convert.ToDecimal(rows["Basic"]) * (25 / 100.00M);
                            rows["LEAVEPROV"] = (Convert.ToDecimal(rows["Gross"]) / 30 * 15) / 12;
                            rows["GRATUITYPROV"] = Convert.ToDecimal(rows["Basic"]) / 12;
                            if (rows["Grade"].ToString() == "C3" || rows["Grade"].ToString() == "C2" || rows["Grade"].ToString() == "C1" || rows["Grade"].ToString() == "A1"
                                || rows["Grade"].ToString() == "A2" || rows["Grade"].ToString() == "A3" || rows["Grade"].ToString() == "B1-M3" || rows["Grade"].ToString() == "B1-M1"
                                )
                            {
                                var basic = Convert.ToDecimal(rows["Basic"]);
                                var Grade = rows["Grade"].ToString();
                                if (Convert.ToDecimal(rows["Basic"]) <= 50000.00M)
                                {
                                    rows["MEDICALPROV"] = (Convert.ToDecimal(rows["Basic"]) / 12);
                                }
                                else
                                {
                                    rows["MEDICALPROV"] = (Convert.ToDecimal(50000) / 12);
                                }

                            }
                            else
                            {
                                rows["MEDICALPROV"] = (Convert.ToDecimal(10000) / 12);
                            }
                            rows["CTC"] = Convert.ToDecimal(rows["Gross"]) + Convert.ToDecimal(rows["PFEMPLOYER"]) + Convert.ToDecimal(rows["BONUSPROV"]) + Convert.ToDecimal(rows["GRATUITYPROV"]) + Convert.ToDecimal(rows["LEAVEPROV"]) + Convert.ToDecimal(rows["MEDICALPROV"]);
                            rows["EPFDED"] = Convert.ToDecimal(rows["PFEMPLOYER"]) * -1;
                            rows["TDS"] = Convert.ToDecimal(rows["TAX"]) * -1;

                            rows["TOTALDEDUCTION"] = (Convert.ToDecimal(rows["EPFDED"]) + Convert.ToDecimal(rows["TDS"])) * -1;
                            rows["PAID"] = (Convert.ToDecimal(rows["Gross"]) - Convert.ToDecimal(rows["TOTALDEDUCTION"])) * -1;
                            rows["FESTIVALBONUS2"] = 0;
                            rows["TOTAL"] = (Convert.ToDecimal(rows["PAID"]) + Convert.ToDecimal(rows["FESTIVALBONUS2"])) * -1;
                            rows["EMPLOYER+EMPLOYEE"] = (Convert.ToDecimal(rows["PFEMPLOYER"]) + Convert.ToDecimal(rows["PFEMPLOYER"])) * -1;

                            rows["BONUSPROV1"] = Convert.ToDecimal(rows["BONUSPROV"]) * -1;
                            rows["LEAVEPROV1"] = Convert.ToDecimal(rows["LEAVEPROV"]) * -1;
                            rows["GRATUITYPROV1"] = Convert.ToDecimal(rows["GRATUITYPROV"]) * -1;
                            rows["MEDICALPROV1"] = Convert.ToDecimal(rows["MEDICALPROV"]) * -1;




                        }
                        dt = Ordinary.DtValueRound(dt, new[] { "MEDICALPROV1", "GRATUITYPROV1", "LEAVEPROV1", "EMPLOYER+EMPLOYEE", "TOTAL" ,"FESTIVALBONUS2","PAID" 
                        ,"TOTALDEDUCTION" , "TDS" ,"EPFDED","CTC" ,"GRATUITYPROV","LEAVEPROV" ,"BONUSPROV","PFEMPLOYER","MEDICALPROV"
                        });
                        


                        #region Report Call

                        EnumReportRepo _reportRepo = new EnumReportRepo();
                        List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

                        string[] conFields = { "ReportType", "ReportId" };
                        string[] conValues = { "SalarySheet", "SalarySheet1" };
                        enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);

                        string ReportFileName = enumReportVMs.FirstOrDefault().ReportFileName;
                        string ReportName = enumReportVMs.FirstOrDefault().Name;

                        SettingRepo _sRepo = new SettingRepo();
                       
                           
                            dt.TableName = "TIBSalary";
                            
                        

                        FiscalYearDetailVM fydVM = new FiscalYearRepo().FYPeriodDetail(Convert.ToInt32(vm.FiscalYearDetailId))
                            .FirstOrDefault();

                        string PeriodName = fydVM.PeriodName;

                        string FullPeriodName = Convert.ToDateTime("01-" + PeriodName).ToString("MMMM-yyyy");


                        if (vm.IsMultipleSalary)
                        {
                            fydVM = new FiscalYearRepo().FYPeriodDetail(Convert.ToInt32(vm.FiscalYearDetailIdTo))
                                .FirstOrDefault();

                            string PeriodNameTo = fydVM.PeriodName;

                            string FullPeriodNameTo = Convert.ToDateTime("01-" + PeriodNameTo).ToString("MMMM-yyyy");

                            FullPeriodName = FullPeriodName + " to " + FullPeriodNameTo;
                        }


                        rptLocation = AppDomain.CurrentDomain.BaseDirectory +
                                      @"Files\ReportFiles\Payroll\Salary\RptSalarySheet.rpt";

                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" +
                                      ReportFileName + ".rpt";

                        doc.Load(rptLocation);
                        string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                        doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                        doc.DataDefinition.FormulaFields["FullPeriodName"].Text = "'" + FullPeriodName + "'";
                        doc.DataDefinition.FormulaFields["HoldStatus"].Text = "'" + vm.HoldStatus + "'";


                        FormulaFieldDefinitions crFormulaF;
                        crFormulaF = doc.DataDefinition.FormulaFields;
                        CommonWebMethod _CommonWebMethod = new CommonWebMethod();

                        thread = new Thread(unused => SalaryEmailRequestBollore(dt, doc, FullPeriodName));
                        thread.Start();
                        // EmpEmailProcess(ds, doc, FullPeriodName)
                        result[0] = "Successfully";
                        result[1] = "Request Email Sent";
                        Session["result"] = result[0] + "~" + result[1];
                        //return Redirect("/Acc/Home/");

                        #endregion
                        return Redirect("/Payroll/SalaryProcess/SalarySheet/");



            }
            catch (Exception)
            {
                throw;
            }
        }

        //////public ActionResult DownloadSalarySheet(HttpPostedFileBase file, string fid, string ProjectId, string DepartmentId, string SectionId
        //////    , string DesignationId, string CodeF, string CodeT, string MulitpleProjectId
        //////    , string other1, string other2, string other3, string Orderby, string bankId, string SheetName, string HoldStatus

        public ActionResult DownloadSalarySheet(SalarySheetVM vm)
        {
            string[] result = new string[6];
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            List<string> ProjectIdList = new List<string>();
            try
            {
                #region Objects and Variables

                ReportDocument doc = new ReportDocument();

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                string FileName = "Download.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                //string fullPath = @"C:\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }

                if (vm.FiscalYearDetailIdTo == 0)
                {
                    vm.FiscalYearDetailIdTo = vm.FiscalYearDetailId;
                }

                if (vm.FiscalYearDetailId != vm.FiscalYearDetailIdTo)
                {
                    vm.IsMultipleSalary = true;
                }

                #region Parmeters

                if (!string.IsNullOrWhiteSpace(vm.MultipleProjectId))
                {
                    vm.ProjectIdList = vm.MultipleProjectId.Split(',').ToList();
                }

                if (!string.IsNullOrWhiteSpace(vm.MultipleOther3))
                {
                    vm.Other3List = vm.MultipleOther3.Split(',').ToList();
                }

                #endregion

                #endregion

                #region PeriodName

                FiscalYearRepo _fiscalYearRepo = new FiscalYearRepo();
                FiscalYearDetailVM fiscalYearDetailVM = new FiscalYearDetailVM();
                string PeriodName = "";
                string PeriodNameTo = "";
                fiscalYearDetailVM = _fiscalYearRepo.FYPeriodDetail(Convert.ToInt32(vm.FiscalYearDetailId))
                    .FirstOrDefault();
                PeriodName = fiscalYearDetailVM.PeriodName;
                string FullPeriodName = Convert.ToDateTime("01-" + PeriodName).ToString("MMMM-yyyy");
                vm.FullPeriodName = FullPeriodName;

                #endregion

                #region Pull Data

                SalaryProcessRepo _empRepo = new SalaryProcessRepo();
                if (CompanyName.ToUpper() == "TIB" || CompanyName.ToUpper() == "BOLLORE"|| CompanyName.ToUpper() == "G4S") //tib
                {
                    dt = new DataTable();
                    string[] condFields = { "FiscalYearDetailId" };
                    string[] condValues = { vm.FiscalYearDetailId.ToString() };

                    if (vm.SheetName == "SalarySheet2")
                    {
                        ds = _empRepo.SalarySheet_TIB_Excel(vm);
                        dt = ds.Tables[0];
                        dt = TIBSalarySheet2(dt);
                    }
                    else if (vm.SheetName == "SalarySheet3")
                    {
                        string[] conditionFields = new string[]
                            { "ProjectId", "SectionId", "DesignationId", "FiscalYearDetailId" };
                        string[] conditionValues = new string[]
                            { vm.ProjectId, vm.SectionId, vm.DesignationId, vm.FiscalYearDetailId.ToString() };
                        DataTable DtResult = _empRepo.SalarySheet_TIB_SummeryOther(vm, conditionFields, conditionValues)
                            .Tables[0];

                        ds = _empRepo.SalarySheet_TIB_SummeryOtherDownload(vm, null, null, DtResult);
                        dt = ds.Tables[0];
                        dt = TIBSalarySheet3(dt);
                    }
                    else if (vm.SheetName == "SalarySheet16")
                    {
                        dt = _empRepo.DataExportForSunTemplate(vm);
                         dt = Ordinary.DtValueRound(dt, new[] { "Transaction Amount","Base Amount"});
                    }
                    else if (vm.SheetName == "SalarySheet25")
                    {
                        dt = _empRepo.DataExportForSunTemplatePayment(vm);
                        dt = Ordinary.DtValueRound(dt, new[] { "Transaction Amount", "Base Amount" });
                    }
                    else if (vm.SheetName == "SalarySheet26")
                    {
                        dt = _empRepo.DataExportForSunCarTransport(vm);
                        dt = Ordinary.DtValueRound(dt, new[] { "Transaction Amount", "Base Amount" });
                    }
                    else if (vm.SheetName == "SalarySheet27")
                    {
                        dt = _empRepo.DataExportForSunCarTransportPayment(vm);
                        dt = Ordinary.DtValueRound(dt, new[] { "Transaction Amount", "Base Amount" });
                    }
                    else
                    {
                        dt = _empRepo.SalarySheet_TIB(condFields, condValues, vm.SheetName,
                            vm.FiscalYearDetailId.ToString(), vm).Tables[0];
                        if (CompanyName.ToUpper() == "TIB" && vm.SheetName == "SalarySheet1")
                        {
                            dt.DefaultView.Sort = "Expr1";
                            dt = dt.DefaultView.ToTable();


                            var toRemove = new string[] { "Section", "JoinDate", "LeftDate", "Branch", "Name", "Email", "BankAccountName", "BankAccountNo", "BankName","LeaveWOPay"
                                , "AccountType", "Routing_No", "SectionOrder", "FiscalYearDetailId", "EmployeeId" ,"ProjectId","DepartmentId","SectionId","DesignationId","IsHold","SecOrderNo","Expr1","OrderNo","DesignationGroupName"
                                };
                            List<string> oldColumnNames = new List<string> { "TIN", "PFEmployer", "PFLoan", "TransportAllowance", "TransportBill" };
                            List<string> newColumnNames = new List<string> { "ETIN", "CPF (10%) on Basic", "PF Loan Adj.", "Salary Transport", "Transport" };
                           
                            dt = Ordinary.DtColumnNameChangeList(dt, oldColumnNames, newColumnNames);
                            foreach (string col in toRemove)
                            {
                                dt.Columns.Remove(col);
                            }
                        }

                        if (CompanyName.ToUpper() == "G4S" && vm.SheetName == "SalarySheet1")
                        {
                            dt.DefaultView.Sort = "Expr1";
                            dt = dt.DefaultView.ToTable();
                            
                            var toRemove = new string[] { "Section","Project", "JoinDate", "LeftDate", "Branch",  "Email", "TIN", "BankAccountName", "BankAccountNo", "BankName","LeaveWOPay","TransportBill","Stamp","PFLoan","STAFFWELFARE"
                                ,"Othere(OT)" ,"Vehicle(Adj)" ,"Other(Bonus)" ,"GP" ,"Travel" ,"ChildAllowance" ,"MOBILE(Allowance)"
                                , "AccountType", "Routing_No", "SectionOrder", "FiscalYearDetailId", "EmployeeId" ,"ProjectId","DepartmentId","SectionId","DesignationId","IsHold","SecOrderNo","Expr1","OrderNo","StepName","DesignationGroupName"
                            };
                           
                            foreach (string col in toRemove)
                            {
                                dt.Columns.Remove(col);
                            }

                            List<string> oldColumnNames = new List<string> { "OtherId", "EMPName", "CostCenter", "WorkinDays", "BusinessDivision", "HouseRent", "GrossSalary", "CompanyPF", "EmployeePF", "TotalDeduction", "NetSalary", "CarAllowance", "FactoredBasesalary", "TransportAllowance", "Monthlymotorcycleallowance", "OtherAllowanceMonthly", "Arrear", "PFEmployer", "CompanyPF", "EmployeePF", "SalaryAdj", "TAX", "Monthlymotorcyclededuction","SpecialAllowance" };
                            List<string> newColumnNames = new List<string> { "G4SID", "Employee Name", "Cost Center", "Working Days", "Business Division", "House Rent", "Gross Salary", "Company PF", "Employee PF", "Total Deduction", "Net Salary", "Car Allowance", "Base Salary", "Conveyance", "Motor Allowance", "Other Allowance", "Arrear Salary", "PF Company", "CompanyPF", "EmployeePF", "Excess salary adj", "TDS", "Deduction for M/C", "Special Allowance" };

                            dt = Ordinary.DtColumnNameChangeList(dt, oldColumnNames, newColumnNames);

                            var dataView = new DataView(dt);

                            dt = dataView.ToTable(true, "S.N.", "G4SID", "Employee Name", "Designation", "Business Division", "Cost Center", "Grade", "Working Days", "Basic", "House Rent", "Conveyance", "Medical", "Base Salary", "Motor Allowance", "Car Allowance", "Other Allowance", "Special Allowance"
                                , "Arrear Salary", "PF Company", "Gross Salary", "Excess salary adj", "TDS", "Advance", "Deduction for M/C", "Company PF", "Employee PF","AbsentDeduction", "Total Deduction", "Net Salary"
                      
                            );
                        }
                        if (vm.SheetName == "SalarySheet4")
                        {
                           // dt.DefaultView.Sort = "Code";
                            dt = dt.DefaultView.ToTable();
                            dt.Columns["NetSalary"].ColumnName = "Net Salary";

                            var toRemove = new string[] { "Code","JoinDate","LeftDate","Branch","CostCenter","Project","BusinessDivision","Section","Designation","Grade","Name","Email","TIN","BankAccountName","BankName","AccountType","Routing_No","SectionOrder","FiscalYearDetailId","EmployeeId","ProjectId"
                                ,"DepartmentId","SectionId","DesignationId","IsHold","WorkinDays","Basic","HouseRent","Medical","TransportAllowance","GrossSalary","TAX","TransportBill","Stamp","PFEmployer","PFLoan","STAFFWELFARE","TotalDeduction"  ,"Othere(OT)","Vehicle(Adj)","Other(Bonus)","LeaveWOPay"
                                ,"GP","Travel","ChildAllowance","MOBILE(Allowance)","CarAllowance","Arrear","OtherAdjustment","TotalAdjustment","SalaryAdj","FactoredBasesalary","PFEmployee","Advance","CarAdjustment","CompanyPF","EmployeePF","NetPayment","SecOrderNo","Expr1","DesignationGroupName","OrderNo","StepName","Monthlymotorcycleallowance","OtherAllowanceMonthly","Monthlymotorcyclededuction","OtherDeductionMonthly"
                            };

                            foreach (string col in toRemove)
                            {
                                dt.Columns.Remove(col);
                            }


                      string[] DecimalColumnNames =
                        {
                            "Net Salary"
                        };

                            dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);
                        }
                        if (vm.SheetName == "SalarySheet8")
                        {
                            dt = TIBSalarySheet8(dt);
                        }

                        if (vm.SheetName == "SalarySheet9")
                        {
                            dt = TIBSalarySheet9(dt, vm.FullPeriodName, vm.PaymentDate);
                        }

                        if (vm.SheetName == "SalarySheet10")
                        {
                            dt = TIBSalarySheet10(dt, vm.FullPeriodName, vm.PaymentDate);
                        }
                    }
                }
                else
                {
                    dt = _empRepo.SalarySheet(vm);
                }

                #endregion

                #region Validations

                if (dt.Rows.Count == 0)
                {
                    result[0] = "Fail";
                    result[1] = "No Data Found";
                    Session["result"] = result[0] + "~" + result[1];

                    return Redirect("SalarySheet");
                }

                #endregion

                #region Report Call

                string filename = "";


                if (vm.IsMultipleSalary)
                {
                    fiscalYearDetailVM = new FiscalYearRepo().FYPeriodDetail(Convert.ToInt32(vm.FiscalYearDetailIdTo))
                        .FirstOrDefault();

                    PeriodNameTo = fiscalYearDetailVM.PeriodName;

                    string FullPeriodNameTo = Convert.ToDateTime("01-" + PeriodNameTo).ToString("MMMM-yyyy");

                    FullPeriodName = FullPeriodName + " to" + FullPeriodNameTo;
                }

                string[] conFields = { "ReportType", "ReportId" };
                string[] conValues = { "SalarySheet", vm.SheetName };
                List<EnumReportVM> enumReportVMs = new EnumReportRepo().SelectAll(0, conFields, conValues);

                string ReportName = enumReportVMs.FirstOrDefault().Name;
                filename = ReportName + "-" + PeriodName;

                if (vm.IsMultipleSalary)
                {
                    filename = ReportName + "-" + PeriodName + " to " + PeriodNameTo;
                }

                #endregion

                #region Prepare Excel

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

                if (CompanyName == "brac")
                {
                    #region brac

                    #region Notes

                    ////////----------------Last Update - 25 February 2019--------------
                    ////////ReportId	    ReportName
                    ////////SalarySheet1	Full Salary Sheet
                    ////////SalarySheet2	Bank Pay
                    ////////SalarySheet3	Salary Summary
                    ////////SalarySheet5	Pay Slip
                    ////////SalarySheet6	pay Slip (Email)
                    ////////SalarySheet7	Salary Certificate
                    ////////SalarySheet8	Full Salary Sheet (Decimal)

                    string[] downloadableSheet = { "SalarySheet1", "SalarySheet2", "SalarySheet3", "SalarySheet8" };

                    if (!downloadableSheet.Contains(vm.SheetName))
                    {
                        Session["result"] = "Fail~This Report is not available in Excel!";
                        return Redirect("SalarySheet");
                    }

                    #endregion

                    BracExcelSalarySheet(dt, workSheet, vm);

                    #region Comments

                    //////#region Column Name Chagne
                    //////dt = Ordinary.DtColumnNameChange(dt, "EmpName", "Name");
                    //////dt = Ordinary.DtColumnNameChange(dt, "Other1", "Designation");
                    //////dt = Ordinary.DtColumnNameChange(dt, "PreEmploymentCheckUp", "Pre-Employment Check Up");
                    //////dt = Ordinary.DtColumnNameChange(dt, "MobileBill", "Excess Mobile Bill");

                    //////string[] DtcolumnName = new string[dt.Columns.Count];
                    //////int j = 0;
                    //////foreach (DataColumn column in dt.Columns)
                    //////{
                    //////    DtcolumnName[j] = column.ColumnName;
                    //////    j++;
                    //////}

                    //////for (int k = 0; k < DtcolumnName.Length; k++)
                    //////{
                    //////    dt = Ordinary.DtColumnNameChange(dt, DtcolumnName[k], Ordinary.AddSpacesToSentence(DtcolumnName[k]));
                    //////}

                    //////dt = Ordinary.DtColumnNameChange(dt, "Code", "BESL ID");
                    //////dt = Ordinary.DtColumnNameChange(dt, "Section", "Cost Center");
                    //////dt = Ordinary.DtColumnNameChange(dt, "PFEmployee", "PF By Employee");

                    //////#endregion

                    //////#region Report Headers, Rows, Columns

                    //////string Line1 = "BRAC EPL STOCK BROKERAGE LIMITED";
                    //////string Line2 = "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                    //////string Line3 = "Salary Sheet for the Month of " + FullPeriodName;


                    //////int LeftColumn = 5;
                    //////int CenterColumn = 5;

                    //////switch (vm.SheetName)
                    //////{
                    //////    case "SalarySheet2": ////----Bank Pay
                    //////        LeftColumn = 3;
                    //////        Line3 = "Salary for the Month of " + FullPeriodName;
                    //////        break;
                    //////    case "SalarySheet3":////----Salary Summary
                    //////        LeftColumn = 3;
                    //////        CenterColumn = 3;
                    //////        Line3 = "Salary Summary for the Month of " + FullPeriodName;
                    //////        break;
                    //////    default:
                    //////        break;
                    //////}


                    //////string[] ReportHeaders = new string[] { "", Line1, Line2, Line3 };


                    //////int TableHeadRow = 0;
                    //////TableHeadRow = ReportHeaders.Length + 2;

                    //////int RowCount = 0;
                    //////RowCount = dt.Rows.Count;

                    //////int ColumnCount = 0;
                    //////ColumnCount = dt.Columns.Count;

                    //////int GrandTotalRow = 0;
                    //////GrandTotalRow = TableHeadRow + RowCount + 1;

                    //////int InWordsRow = 0;
                    //////InWordsRow = GrandTotalRow + 1;

                    //////int SignatureSpaceRow = 0;
                    //////SignatureSpaceRow = InWordsRow + 1;

                    //////int SignatureRow = 0;
                    //////SignatureRow = InWordsRow + 4;
                    //////#endregion


                    //////workSheet.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                    //////#region Format
                    //////workSheet.Cells["B" + (TableHeadRow + 1) + ":" + Ordinary.Alphabet[(ColumnCount + 1)] + (TableHeadRow + 1 + RowCount + 3)].Style.Numberformat.Format = "#,##0";
                    //////workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount + 1)] + (TableHeadRow)].Style.Font.Bold = true;
                    //////workSheet.Cells["A" + (RowCount + TableHeadRow + 1) + ":" + Ordinary.Alphabet[(ColumnCount + 1)] + (RowCount + TableHeadRow + 1)].Style.Font.Bold = true;
                    //////workSheet.Cells[Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (RowCount + TableHeadRow + 1)].Style.Font.Bold = true;

                    //////workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    //////workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    //////var format = new OfficeOpenXml.ExcelTextFormat();
                    //////format.Delimiter = '~';
                    //////format.TextQualifier = '"';
                    //////format.DataTypes = new[] { eDataTypes.String };


                    //////for (int i = 0; i < ReportHeaders.Length; i++)
                    //////{
                    //////    workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Merge = true;
                    //////    workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    //////    workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.Font.Bold = true;
                    //////    workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.Font.Size = 14 - i;
                    //////    workSheet.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

                    //////}

                    //////workSheet.Row(TableHeadRow).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //////workSheet.Row(TableHeadRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //////workSheet.Row(TableHeadRow).Style.WrapText = true;

                    //////workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

                    //////#region Grand Total

                    //////for (int i = LeftColumn + 1; i <= ColumnCount; i++)
                    //////{
                    //////    workSheet.Cells[GrandTotalRow, i].Formula = "=Sum(" + workSheet.Cells[TableHeadRow + 1, i].Address + ":" + workSheet.Cells[(TableHeadRow + RowCount), i].Address + ")";
                    //////}
                    //////#endregion

                    //////object sumObject;
                    //////sumObject = dt.Compute("Sum([Net Salary])", string.Empty);

                    //////decimal NetSalary = Convert.ToDecimal(sumObject);

                    //////string strNetSalary = NetSalary.ToString("0.##");

                    //////string NetSalaryInWords = Ordinary.ConvertToWords(strNetSalary, true);

                    //////workSheet.Row(InWordsRow).Style.WrapText = true;
                    //////workSheet.Row(InWordsRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    //////workSheet.Cells[TableHeadRow + RowCount + 2, 2, TableHeadRow + RowCount + 2, ColumnCount].Merge = true;
                    //////workSheet.Cells[TableHeadRow + RowCount + 2, 2, TableHeadRow + RowCount + 2, ColumnCount].Style.Font.Bold = true;
                    //////workSheet.Cells[TableHeadRow + RowCount + 2, 1].LoadFromText("Net Salary (In Words):");
                    //////workSheet.Cells[TableHeadRow + RowCount + 2, 2].LoadFromText(NetSalaryInWords);

                    //////workSheet.Column(CenterColumn).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //////workSheet.Cells[TableHeadRow, 1, GrandTotalRow - 1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ////////////workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //////if (vm.SheetName == "SalarySheet1")////Full Salary Sheet
                    //////{
                    //////    #region Signature


                    //////    string signatory1Title = "Prepared By";
                    //////    string signatory2Title = "Checked By";
                    //////    string signatory3Title = "Checked By";
                    //////    string signatory4Title = "Approved By";


                    //////    int signatory1Column = (ColumnCount / 4) - 2;
                    //////    int signatory2Column = (ColumnCount / 4) * 2 - 2;
                    //////    int signatory3Column = (ColumnCount / 4) * 3 - 2;
                    //////    int signatory4Column = (ColumnCount / 4) * 4 - 2;


                    //////    workSheet.Cells[SignatureRow, signatory1Column].LoadFromText(signatory1Title);
                    //////    workSheet.Cells[SignatureRow, signatory2Column].LoadFromText(signatory2Title);
                    //////    workSheet.Cells[SignatureRow, signatory3Column].LoadFromText(signatory3Title);
                    //////    workSheet.Cells[SignatureRow, signatory4Column].LoadFromText(signatory4Title);


                    //////    workSheet.Cells[SignatureRow, signatory1Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    //////    workSheet.Cells[SignatureRow, signatory2Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    //////    workSheet.Cells[SignatureRow, signatory3Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    //////    workSheet.Cells[SignatureRow, signatory4Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;


                    //////    workSheet.Row(SignatureRow).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //////    workSheet.Row(SignatureRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //////    workSheet.Row(SignatureRow).Style.WrapText = true;
                    //////    workSheet.Row(SignatureRow).Style.Font.Bold = true;
                    //////    #endregion
                    //////}
                    ////#endregion

                    #endregion

                    #endregion
                }
               
                else if ((CompanyName.ToLower() == "kbl" || CompanyName.ToLower() == "anupam" ||
                          CompanyName.ToLower() == "kajol" || CompanyName.ToLower() == "ssl"))
                {
                    int TableHeadRow = 1;
                    workSheet.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);
                }
                else if (CompanyName.ToUpper() == "TIB"||CompanyName.ToUpper() == "G4S")
                {
                    CompanyRepo cRepo = new CompanyRepo();
                    CompanyVM comInfo = cRepo.SelectById(1);
                    string Line1 = comInfo.Name; // "BRAC EPL STOCK BROKERAGE LIMITED";
                    string
                        Line2 = comInfo
                            .Address; // "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                    string Line3 = "";
                    if (vm.SheetName == "SalarySheet1")
                    {
                        if (CompanyName.ToUpper() != "G4S")
                        {
                            Line2 = "Project: " + dt.Rows[0]["Project"].ToString();
                        }
                        Line3 = "Salary Sheet for the Month of " + vm.FullPeriodName;
                    }
                    if (vm.SheetName == "SalarySheet8")
                    {
                        Line3 = "Salary Sheet for the Month of " + vm.FullPeriodName;
                    }

                    if (vm.SheetName == "SalarySheet10")
                    {
                        Line3 = "BFTN-SCB-" + vm.FullPeriodName;
                    }

                    if (vm.SheetName == "SalarySheet9")
                    {
                        Line3 = "BFTN-SB-" + vm.FullPeriodName;
                    }

                    if (vm.SheetName == "SalarySheet2")
                    {
                        Line3 = "Salary Summary (Designation)-" + vm.FullPeriodName;
                    }

                    if (vm.SheetName == "SalarySheet3")
                    {
                        Line3 = "Salary Summary (Other)-" + vm.FullPeriodName;
                    }

                    int LeftColumn = 5;
                    int CenterColumn = 5;

                    string[] ReportHeaders = new string[] { Line1, Line2, Line3 };
                    
                    ExcelSheetFormat(dt, workSheet, ReportHeaders);
                }
                else if ( CompanyName.ToUpper() == "BOLLORE")
                {
                    CompanyRepo cRepo = new CompanyRepo();
                    CompanyVM comInfo = cRepo.SelectById(1);
                    string Line1 = comInfo.Name; // "BRAC EPL STOCK BROKERAGE LIMITED";
                    string Line2 = comInfo.Address; // "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                    string Line3 = "";
                    if (vm.SheetName == "SalarySheet8")
                    {
                        Line3 = "Salary Sheet for the Month of " + vm.FullPeriodName;
                    }

                    if (vm.SheetName == "SalarySheet10")
                    {
                        Line3 = "BFTN-SCB-" + vm.FullPeriodName;
                    }

                    if (vm.SheetName == "SalarySheet9")
                    {
                        Line3 = "BFTN-SB-" + vm.FullPeriodName;
                    }

                    if (vm.SheetName == "SalarySheet2")
                    {
                        Line3 = "Salary Summary (Designation)-" + vm.FullPeriodName;
                    }

                    if (vm.SheetName == "SalarySheet3")
                    {
                        Line3 = "Salary Summary (Other)-" + vm.FullPeriodName;
                    }
                    if (vm.SheetName == "SalarySheet21")
                    {
                        Line3 = "SALARY of-" + vm.FullPeriodName;
                    }
                    if (vm.SheetName == "SalarySheet22")
                    {
                        Line3 = "Transport Allowance of-" + vm.FullPeriodName;
                    }
                    if (vm.SheetName == "SalarySheet23")
                    {
                        Line3 = "Car Allowance of-" + vm.FullPeriodName;
                    }

                    int LeftColumn = 5;
                    int CenterColumn = 5;
                    if(vm.SheetName == "SalarySheet1")
                    {
                        dt.DefaultView.Sort = "Code";
                        dt = dt.DefaultView.ToTable();
                        dt.Columns["SpeacilAllowance"].ColumnName = "Special Allowance";
                        dt.Columns["GrossSalary"].ColumnName = "Gross Salary";

                        dt.Columns["Branch"].ColumnName = "Branch/Station Name";
                        var toRemove = new string[] { "Section", "Name", "TIN","Email", "BankAccountName","AccountType","EmploymentType","NID","Nationality_E", "Routing_No", "SectionOrder", "FiscalYearDetailId", "EmployeeId", "ProjectId", "DepartmentId", "SectionId", "DesignationId"
                    , "IsHold", "CarAllowance", "Stamp","PFLoan", "STAFFWELFARE", "DeductionTotal", "NetSalary", "Othere(OT)", "Vehicle(Adj)", "Other(Bonus)", "LeaveWOPay","GP","Travel","ChildAllowance","MOBILE(Allowance)"
                    ,"OtherAdjustment","NetPayment","SecOrderNo","Expr1","DesignationGroupName","OrderNo","StepName","TotalAdjustment","Medical","MEDICALALLOWANCE","Project","TransportAllowance"};
                        foreach (string col in toRemove)
                        {
                            dt.Columns.Remove(col);
                        }

                        List<string> oldColumnNames = new List<string> { "EmpName", "BankName", "WorkinDays", "BankAccountNo", "HouseRent", "DeductionTotal", "NetSalary", "BONUS PROV", "LEAVE PROV", "GRATUITY PROV", "MEDICAL PROV", "ELENCASHMENT", "SPECIAL ALLOWANCE", "SALESINCENTIVE", "TOTAL DEDUCTION", "NET PAY", "PF CONT. EMPLOYER","PFEmployer","Gross" };
                        List<string> newColumnNames = new List<string> { "Employee Name", "Bank Name", "Working Days", "Bank Account No", "House Rent", "Deduction Total", "Net Salary", "Bonus Prov", "Leave Prov", "Gratuity Prov", "Medical Prov", "EL Encashment", "Special Allowance", "Sales Incentive", "Total Deduction", "Net Pay", "PF Cont. Employer", "PF Employee Deduction", "Total Earnings" };

                        dt = Ordinary.DtColumnNameChangeList(dt, oldColumnNames, newColumnNames);


                        dt = Ordinary.DtValueRound(dt, new[] {"TOTALDEDUCTION" , "TDS" ,"EPFDED","CTC"});

                     dt.Columns.AddRange(new DataColumn[]
                     {                    
                     new DataColumn("TDS"),
                     new DataColumn("Total Deduction"),
                     new DataColumn("Net Pay"),
                     new DataColumn("PF Cont. Employer"),
                     new DataColumn("Bonus Prov"),
                     new DataColumn("Leave Prov"),
                     new DataColumn("Gratuity Prov"),
                     new DataColumn("Medical Prov"),
                     new DataColumn("CTC Before"),   
                     new DataColumn("CTC"), 
                     new DataColumn("VAR"),
                      new DataColumn("Remarks")                                
                     });

                        foreach (DataRow rows in dt.Rows)
                        {
                            decimal v = Convert.ToDecimal(rows["Basic"]) * (10 / 100.00M);

                            //decimal v=(Convert.ToDecimal(rows["Basic"]) * 10)/100;
                            rows["PF Cont. Employer"] = Math.Round(Convert.ToDecimal(rows["PF Employee Deduction"]), 2);
                            rows["Bonus Prov"] = Math.Round(Convert.ToDecimal(rows["Basic"]) * (25 / 100.00M), 2);
                            rows["Leave Prov"] = Math.Round((Convert.ToDecimal(rows["Gross Salary"]) / 30 * 15) / 12, 2);
                            rows["Gratuity Prov"] = Math.Round(Convert.ToDecimal(rows["Basic"]) / 12,2);

                            if (rows["Grade"].ToString() == "C3" || rows["Grade"].ToString() == "C2" || rows["Grade"].ToString() == "C1" || rows["Grade"].ToString() == "A1"
                               || rows["Grade"].ToString() == "A2" || rows["Grade"].ToString() == "A3" || rows["Grade"].ToString() == "B1-M3" || rows["Grade"].ToString() == "B1-M1"
                               )
                            {
                                rows["Medical Prov"] = Math.Round((Convert.ToDecimal(10000) / 12),2); 
                            }
                            else
                            {
                                var basic = Convert.ToDecimal(rows["Basic"]);
                                var Grade = rows["Grade"].ToString();
                                if (Convert.ToDecimal(rows["Basic"]) <= 50000.00M)
                                {
                                    rows["Medical Prov"] = Math.Round((Convert.ToDecimal(rows["Basic"]) / 12), 2);
                                }
                                else
                                {
                                    rows["Medical Prov"] = Math.Round((Convert.ToDecimal(50000) / 12), 2);
                                }                               
                            }
                            rows["CTC"] = Convert.ToDecimal(rows["Gross Salary"]) + Convert.ToDecimal(rows["PF Cont. Employer"]) + Convert.ToDecimal(rows["Bonus Prov"]) + Convert.ToDecimal(rows["Gratuity Prov"]) + Convert.ToDecimal(rows["Leave Prov"]) + Convert.ToDecimal(rows["Medical Prov"]);                         
                            rows["TDS"] = Convert.ToDecimal(rows["TAX"]);
                            rows["Total Deduction"] = (Convert.ToDecimal(rows["PF Cont. Employer"]) + Convert.ToDecimal(rows["TDS"]));
                            rows["Net Pay"] = (Convert.ToDecimal(rows["Gross Salary"]) - Convert.ToDecimal(rows["Total Deduction"]));
                            rows["CTC Before"] = "";
                            rows["VAR"] = "";
                            rows["Remarks"] = "";                                                                       

                        }
                        dt.Columns.Remove("TAX");
                        dt = Ordinary.DtValueRound(dt, new[] { "MEDICALPROV1", "GRATUITYPROV1", "LEAVEPROV1", "EMPLOYER+EMPLOYEE", "TOTAL" ,"FESTIVALBONUS2","PAID" 
                        ,"TOTALDEDUCTION" , "TDS" ,"EPFDED","CTC" ,"GRATUITYPROV","LEAVEPROV" ,"BONUSPROV","PFEMPLOYER","MEDICALPROV"
                        });
                        string[] DecimalColumnNames =
                        {
                            "Basic", "House Rent", "Gross Salary", "TDS", "PF Employee Deduction", "Deduction Total", "Net Salary", "Bonus Prov", "Leave Prov", "Gratuity Prov", "Medical Prov", "CTC", 
                            "EL Encashment","Sales Incentive","Special Allowance","Gross Salary","EL Encashment","Total Deduction" , "TDS" ,"PF Cont. Employer","Net Pay"
                        };

                        dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);
                    }

                    if (vm.SheetName == "SalarySheet21")
                    {
                        dt.DefaultView.Sort = "Code";
                        dt = dt.DefaultView.ToTable();
                        dt.Columns["NetSalary"].ColumnName = "Transaction Amount";

                        var toRemove = new string[] { "S.N.","Code","Branch","Designation","Grade","Gender","Department","BankName","JoinDate","DateofBirth","LeftDate","LopDays","WorkingDay","Project","Section","Name","Email","TIN","BankAccountName","AccountType"
,"Routing_No","SectionOrder","FiscalYearDetailId","EmployeeId","ProjectId","DepartmentId","SectionId","DesignationId","OtherId1","OtherId","EmploymentType","NID","Nationality_E","IsHold","Basic","HouseRent","Medical"
,"Othere(OT)","Vehicle(Adj)","Other(Bonus)","LeaveWOPay","GP","Travel","ChildAllowance","MOBILE(Allowance)","OtherAdjustment","TotalAdjustment","SecOrderNo","Expr1","DesignationGroupName","OrderNo","StepName","SpeacilAllowance","ELENCASHMENT",
"MEDICALALLOWANCE","SALESINCENTIVE","OtherEarning","Gross","TAX","Stamp","PFEmployer","PFLoan","STAFFWELFARE","OtherDeduction","DeductionTotal","NetPayment","CarAllowance","TransportAllowance"

                        };
                        foreach (string col in toRemove)
                        {
                            dt.Columns.Remove(col);
                        }


                        dt.Columns.AddRange(new DataColumn[]
                         {                    
                         new DataColumn("Dr./Cr."),
                         new DataColumn("Chqser"),
                         new DataColumn("Chqnum"),
                         new DataColumn("Chqdat"),
                         new DataColumn("Remarks"),   
                         });

                        foreach (DataRow rows in dt.Rows)
                        {
                            rows["Dr./Cr."] = "C";
                            rows["Chqser"] = "";
                            rows["Chqnum"] = "";
                            rows["Chqdat"] = "";
                            rows["Remarks"] = "SALARY " + vm.FullPeriodName;
                        }


                        string[] DecimalColumnNames =
                        {
                            "Transaction Amount"
                        };

                        dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);
                    }

                    if (vm.SheetName == "SalarySheet22")
                    {
                        dt.DefaultView.Sort = "Code";
                        dt = dt.DefaultView.ToTable();
                        dt.Columns["TransportAllowance"].ColumnName = "Transport Allowance";

                        var toRemove = new string[] { "S.N.","Code","Branch","Designation","Grade","Gender","Department","BankName","JoinDate","DateofBirth","LeftDate","LopDays","WorkingDay","Project","Section","Name","Email","TIN","BankAccountName","AccountType"
,"Routing_No","SectionOrder","FiscalYearDetailId","EmployeeId","ProjectId","DepartmentId","SectionId","DesignationId","OtherId1","OtherId","EmploymentType","NID","Nationality_E","IsHold","Basic","HouseRent","Medical"
,"Othere(OT)","Vehicle(Adj)","Other(Bonus)","LeaveWOPay","GP","Travel","ChildAllowance","MOBILE(Allowance)","OtherAdjustment","TotalAdjustment","SecOrderNo","Expr1","DesignationGroupName","OrderNo","StepName","SpeacilAllowance","ELENCASHMENT",
"MEDICALALLOWANCE","SALESINCENTIVE","OtherEarning","Gross","TAX","Stamp","PFEmployer","PFLoan","STAFFWELFARE","OtherDeduction","DeductionTotal","NetSalary","NetPayment","CarAllowance"

                        };
                        foreach (string col in toRemove)
                        {
                            dt.Columns.Remove(col);
                        }


                         dt.Columns.AddRange(new DataColumn[]
                         {                    
                         new DataColumn("Dr./Cr."),
                         new DataColumn("Chqser"),
                         new DataColumn("Chqnum"),
                         new DataColumn("Chqdat"),
                         new DataColumn("Remarks"),   
                         });

                        foreach (DataRow rows in dt.Rows)
                        {
                            rows["Dr./Cr."] = "C";
                            rows["Chqser"] ="";
                            rows["Chqnum"] ="";
                            rows["Chqdat"] = "";
                            rows["Remarks"] = "TRANSPORT ALLOWANCE " + vm.FullPeriodName;
                        }


                        string[] DecimalColumnNames =
                        {
                            "Transport Allowance"
                        };

                        dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);
                    }
                    if (vm.SheetName == "SalarySheet23")
                    {
                        dt.DefaultView.Sort = "Code";
                        dt = dt.DefaultView.ToTable();
                        dt.Columns["CarAllowance"].ColumnName = "Car Allowance";

                      
                        var toRemove = new string[] { "S.N.","Code","Branch","Designation","Grade","Gender","Department","BankName","JoinDate","DateofBirth","LeftDate","LopDays","WorkingDay","Project","Section","Name","Email","TIN","BankAccountName","AccountType"
,"Routing_No","SectionOrder","FiscalYearDetailId","EmployeeId","ProjectId","DepartmentId","SectionId","DesignationId","OtherId1","OtherId","EmploymentType","NID","Nationality_E","IsHold","Basic","HouseRent","Medical"
,"Othere(OT)","Vehicle(Adj)","Other(Bonus)","LeaveWOPay","GP","Travel","ChildAllowance","MOBILE(Allowance)","OtherAdjustment","TotalAdjustment","SecOrderNo","Expr1","DesignationGroupName","OrderNo","StepName","SpeacilAllowance","ELENCASHMENT",
"MEDICALALLOWANCE","SALESINCENTIVE","OtherEarning","Gross","TAX","Stamp","PFEmployer","PFLoan","STAFFWELFARE","OtherDeduction","DeductionTotal","NetSalary","NetPayment","TransportAllowance"
                    };
                        foreach (string col in toRemove)
                        {
                            dt.Columns.Remove(col);
                        }

                        dt.Columns.AddRange(new DataColumn[]
                         {                    
                         new DataColumn("Dr./Cr."),
                         new DataColumn("Chqser"),
                         new DataColumn("Chqnum"),
                         new DataColumn("Chqdat"),
                         new DataColumn("Remarks"),   
                         });

                        foreach (DataRow rows in dt.Rows)
                        {
                            rows["Dr./Cr."] = "C";
                            rows["Chqser"] = "";
                            rows["Chqnum"] = "";
                            rows["Chqdat"] = "";
                            rows["Remarks"] = "CAR ALLOWANCE " + vm.FullPeriodName;
                        }


                        string[] DecimalColumnNames =
                        {
                            "Car Allowance"
                        };

                        dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);
                    }
                    if (vm.SheetName == "SalarySheet24")
                    {
                        dt.DefaultView.Sort = "Code";
                        dt = dt.DefaultView.ToTable();
                        dt.Columns["CarAllowance"].ColumnName = "Medical Entitlement";

                        var toRemove = new string[] { "S.N.","Code","Branch","Designation","Gender","Department","BankName","JoinDate","DateofBirth","LeftDate","LopDays","WorkingDay","Project","Section","Name","Email","TIN","BankAccountName","AccountType"
,"Routing_No","SectionOrder","FiscalYearDetailId","EmployeeId","ProjectId","DepartmentId","SectionId","DesignationId","OtherId1","OtherId","EmploymentType","NID","Nationality_E","IsHold","GrossSalary","HouseRent","Medical"
,"Othere(OT)","Vehicle(Adj)","Other(Bonus)","LeaveWOPay","GP","Travel","ChildAllowance","MOBILE(Allowance)","OtherAdjustment","TotalAdjustment","SecOrderNo","Expr1","DesignationGroupName","OrderNo","StepName","SpeacilAllowance","ELENCASHMENT",
"MEDICALALLOWANCE","SALESINCENTIVE","OtherEarning","Gross","TAX","Stamp","PFEmployer","PFLoan","STAFFWELFARE","OtherDeduction","DeductionTotal","NetSalary","NetPayment","TransportAllowance"
                    };
                        foreach (string col in toRemove)
                        {
                            dt.Columns.Remove(col);
                        }                       

                        dt.Columns.AddRange(new DataColumn[]
                         {                    
                       
                         new DataColumn("Claim- Jan 23"),
                         new DataColumn("Claim- Feb 23"),
                         new DataColumn("Claim- Mar 23"),
                         new DataColumn("Claim- Apr 23"),
                         new DataColumn("Claim- May 23"),   
                         new DataColumn("Claim- Jun 23"),   
                         new DataColumn("Claim- Jul 23"),   
                         new DataColumn("Claim- Aug 23"),   
                         new DataColumn("Claim- Sep 23"),   
                         new DataColumn("Claim- Oct 23"),   
                         new DataColumn("Claim- Nov 23"),   
                         new DataColumn("Claim- Dec 23"),   
                         new DataColumn("Total Claims"),   
                         new DataColumn("Claims Paid"),   
                         new DataColumn("Balance Available"),                         
                         });

                        foreach (DataRow rows in dt.Rows)
                        {
                           
                            rows["Claim- Jan 23"] = 0;
                            rows["Claim- Feb 23"] = 0;
                            rows["Claim- Mar 23"] = 0;
                            rows["Claim- Apr 23"] = 0;
                            rows["Claim- May 23"] = 0;
                            rows["Claim- Jun 23"] = 0;
                            rows["Claim- Jul 23"] = 0;
                            rows["Claim- Aug 23"] = 0;
                            rows["Claim- Sep 23"] = 0;
                            rows["Claim- Oct 23"] = 0;
                            rows["Claim- Nov 23"] = 0;
                            rows["Claim- Dec 23"] = 0;
                            rows["Total Claims"] = 0;
                            rows["Claims Paid"] = 0;
                            rows["Balance Available"] = 0;                         
                        
                        }
                        foreach (DataRow rows in dt.Rows)
                        {
                            if (rows["Grade"].ToString() == "C3" || rows["Grade"].ToString() == "C2" || rows["Grade"].ToString() == "C1" || rows["Grade"].ToString() == "A1"
                                  || rows["Grade"].ToString() == "A2" || rows["Grade"].ToString() == "A3" || rows["Grade"].ToString() == "B1-M2" || rows["Grade"].ToString() == "B1-M3" || rows["Grade"].ToString() == "B1-M1"
                                  )
                            {
                                var basic = Convert.ToDecimal(rows["Basic"]);
                                var Grade = rows["Grade"].ToString();
                                if (Convert.ToDecimal(rows["Basic"]) <= 50000.00M)
                                {
                                    rows["Medical Entitlement"] = (Convert.ToDecimal(rows["Basic"]));
                                }
                                else
                                {
                                    rows["Medical Entitlement"] = (Convert.ToDecimal(50000));
                                }

                            }
                            else
                            {
                                rows["Medical Entitlement"] = (Convert.ToDecimal(10000));
                            }
                        }

                        string[] DecimalColumnNames =
                        {
                            "Medical Entitlement",
                            "Claim- Jan 23",
                            "Claim- Feb 23",
                            "Claim- Mar 23",
                            "Claim- Apr 23",
                            "Claim- May 23",
                            "Claim- Jun 23",
                            "Claim- Jul 23",
                            "Claim- Aug 23",
                            "Claim- Sep 23",
                            "Claim- Oct 23",
                            "Claim- Nov 23",
                            "Claim- Dec 23",
                            "Total Claims",
                            "Claims Paid",
                            "Balance Available",                        
                        };

                        dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);
                    }
                    string[] ReportHeaders = new string[] { Line1, Line2, Line3 };
                    ExcelSheetFormat(dt, workSheet, ReportHeaders);
                }
                #endregion

                #region Excel Download

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
                return Redirect("SalarySheet");

                #endregion
            }
            catch (Exception e)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(
                    result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine +
                    result[5].ToString(), this.GetType().Name,
                    result[4].ToString() + Environment.NewLine + result[3].ToString());
                return Redirect("SalarySheet");
            }
        }

        private DataTable TIBSalarySheet8(DataTable dt)
        {
            //dt.Columns.Add("Gross", typeof(decimal));//{@Basic}+{@HouseRent}+{@Medical}+{@TransportAllowance}
            //dt.Columns.Add("DeductionTotal", typeof(decimal));//{@TAX}+{@TransportBill}+{@Stamp}+{@StaffWelfare}+{@PFEmployer}+{@PFLoan}
            //dt.Columns.Add("NetSalary", typeof(decimal));//formula=tonumber({@SalaryGross}-{@DeductionTotal}) 
            //dt.Columns.Add("OtherAdjustment", typeof(decimal));//{dtSalary.Other(Earning)}+{dtSalary.HARDSHIP}
            //dt.Columns.Add("TotalAdjustment", typeof(decimal));//{@OverTime}+{@[Vehicle(Adj)}+{@Bonus}+{@TransportBill_LWP}+{@GP}+{@Travel}+{@MOBILE(Allowance)}+{@ChildAllowance}+{@OtherAdjustment}
            //dt.Columns.Add("NetPayment", typeof(decimal));//{@NetSalary}+{@TotalAdjustment}

            string[] DecimalColumnNames =
            {
                "Basic", "Basic", "HouseRent", "Medical", "TransportAllowance", "Gross", "TAX", "TransportBill",
                "Stamp", "PFEmployer", "PFLoan", "STAFFWELFARE", "DeductionTotal", "NetSalary", "Othere(OT)",
                "Vehicle(Adj)", "Other(Bonus)", "LeaveWOPay", "GP", "Travel", "ChildAllowance", "MOBILE(Allowance)",
                "OtherAdjustment", "TotalAdjustment", "NetPayment"
            };

            dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);

            dt = Ordinary.DtColumnNameChange(dt, "Code", "EIN");
            try
            {
                var dataView = new DataView(dt);

                string strSort = " Expr1, EIN";
                dataView.Sort = strSort;

                dt = dataView.ToTable(true, "EmpName", "Designation", "EIN", "Grade", "StepName", "Basic", "HouseRent",
                    "Medical", "TransportAllowance", "Gross", "TAX"
                    , "TransportBill", "Stamp", "PFEmployer", "PFLoan", "STAFFWELFARE", "DeductionTotal", "NetSalary",
                    "Othere(OT)", "Vehicle(Adj)"
                    , "Other(Bonus)", "LeaveWOPay", "GP", "Travel", "ChildAllowance", "MOBILE(Allowance)",
                    "OtherAdjustment", "TotalAdjustment", "NetPayment"
                );
            }
            catch (Exception e)
            {
                FileLogger.Log(
                    e.Message.ToString() + Environment.NewLine + e.Source.ToString() + Environment.NewLine +
                    e.StackTrace.ToString(), this.GetType().Name, e.Message.ToString());
            }

            return dt;
        }

        private DataTable TIBSalarySheet10(DataTable dt, string PreiodName = null, string PaymentDate = null)
        {
            string DebitAccountNo = new SettingRepo().settingValue("Salary", "DebitA/CNo");


            //dt.Columns.Add("Gross", typeof(decimal));//{@Basic}+{@HouseRent}+{@Medical}+{@TransportAllowance}
            //dt.Columns.Add("DeductionTotal", typeof(decimal));//{@TAX}+{@TransportBill}+{@Stamp}+{@StaffWelfare}+{@PFEmployer}+{@PFLoan}
            //dt.Columns.Add("NetSalary", typeof(decimal));//formula=tonumber({@SalaryGross}-{@DeductionTotal}) 
            //dt.Columns.Add("OtherAdjustment", typeof(decimal));//{dtSalary.Other(Earning)}+{dtSalary.HARDSHIP}
            //dt.Columns.Add("TotalAdjustment", typeof(decimal));//{@OverTime}+{@[Vehicle(Adj)}+{@Bonus}+{@TransportBill_LWP}+{@GP}+{@Travel}+{@MOBILE(Allowance)}+{@ChildAllowance}+{@OtherAdjustment}
            //dt.Columns.Add("NetPayment", typeof(decimal));//{@NetSalary}+{@TotalAdjustment}

            string[] DecimalColumnNames = { "NetPayment" };
            dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);

            dt.Columns.Add(new DataColumn("Reason") { DefaultValue = "Salary & Allowances for " + PreiodName });
            dt.Columns.Add(new DataColumn("PayeeAccType (Add1)") { DefaultValue = "Savings" });
            dt.Columns.Add(new DataColumn("Debit A/C No.") { DefaultValue = DebitAccountNo });
            dt.Columns.Add(new DataColumn("Payment Date") { DefaultValue = PaymentDate });

            dt = Ordinary.DtColumnNameChange(dt, "Code", "Customer Reference (16)");
            dt = Ordinary.DtColumnNameChange(dt, "EmpName", "Payee Name");
            dt = Ordinary.DtColumnNameChange(dt, "BankAccountNo", "Payee Bank Acc No");
            var dataView = new DataView(dt);


            dt = dataView.ToTable(true, "Customer Reference (16)", "Payee Name", "Payee Bank Acc No"
                , "Reason", "NetPayment", "Payment Date", "Debit A/C No.", "Email");
            return dt;
        }

        private DataTable TIBSalarySheet9(DataTable dt, string PreiodName = null, string PaymentDate = null)
        {
            string DebitAccountNo = new SettingRepo().settingValue("Salary", "DebitA/CNo");


            //            string[] DeleteColumnNames = {  
            //                    "AbsentDeduction",
            //                     "AccountType",
            //                     "AdvanceDeduction",
            //                     "Arrear",
            //                     "Branch",
            //                     "EmployeeId",
            //                     "FiscalYearDetailId",
            //                     "HARDSHIP",
            //                     "LeaveEncash",
            //                     "LeftDate",
            //                     "Option1",
            //                     "Other(Deduction)",
            //                     "Other(Earning)",
            //                     "Other(EL)",
            //                     "Other(Salary)",
            //                     "Other(StaffWF)",
            //                     "PFEmployee",
            //                     "PreEmploymentCheckUp",
            //                     "Punishment",
            //                        "ProjectId",
            //                        "DepartmentId",
            //                        "SectionId",
            //                        "DesignationId",
            //                        "IsHold",
            //                     "ReimbursableExpense"
            //                     };

            //            //'dt = Ordinary.DtDeleteColumns(dt, DeleteColumnNames);

            //            string[] ShortColumnNames = {  

            //"Project",
            //"Department",
            //"Section",
            //"Code",
            //"EmpName",
            //"JoinDate",
            //"Designation",
            //"Grade",
            //"StepName",
            //"Basic",
            //"HouseRent",
            //"Medical",
            //"TransportAllowance",
            //"Gross",
            //"TAX",
            //"TransportBill",
            //"Stamp",
            //"PFEmployer",
            //"Other(PF)",
            //"STAFFWELFARE",
            //"DeductionTotal",
            //"NetSalary",
            //"Othere(OT)",
            //"Vehicle(Adj)",
            //"Other(Bonus)",
            //"LeaveWOPay",
            //"MobileBill",
            //"Travel",
            //"ChildAllowance",
            //"MOBILE(Allowance)",
            //"OtherAdjustment",
            //"TotalAdjustment",
            //"NetPayment",
            //"BankName",
            //"BankAccountName",
            //"BankAccountNo",
            // "Routing_No",

            // };
            string[] DecimalColumnNames = { "NetPayment" };


            // dt = Ordinary.DtSetColumnsOrder(dt, ShortColumnNames);
            dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);

            dt.Columns.Add(new DataColumn("Reason") { DefaultValue = "Salary & Allowances for " + PreiodName });
            dt.Columns.Add(new DataColumn("PayeeAccType (Add1)") { DefaultValue = "Savings" });
            dt.Columns.Add(new DataColumn("Debit A/C No.") { DefaultValue = DebitAccountNo });
            dt.Columns.Add(new DataColumn("Payment Date") { DefaultValue = PaymentDate.ToString() });

            dt = Ordinary.DtColumnNameChange(dt, "Code", "Customer Reference (16)");
            dt = Ordinary.DtColumnNameChange(dt, "EmpName", "Payee Name");
            dt = Ordinary.DtColumnNameChange(dt, "BankAccountNo", "Payee Bank Acc No");
            dt = Ordinary.DtColumnNameChange(dt, "Routing_No", "PayeeBankRouting");
            dt = Ordinary.DtColumnNameChange(dt, "NetPayment", "Amount");
            var dataView = new DataView(dt);
            dt = dataView.ToTable(true, "Customer Reference (16)", "Payee Name", "Payee Bank Acc No",
                "PayeeAccType (Add1)", "PayeeBankRouting"
                , "Reason", "Amount", "Payment Date", "Debit A/C No.", "Email");
            return dt;
        }


        private DataTable TIBSalarySheet2(DataTable dt)
        {
            try
            {
                var dataView = new DataView(dt);

                string strSort = " Section";
                dataView.Sort = strSort;
                dt = dataView.ToTable(true, "Project", "Section", "Designation", "Basic", "HouseRent", "Medical",
                    "TransportAllowance", "Gross", "TAX"
                    , "TransportBill", "Stamp", "PFEmployer", "PFLoan", "STAFFWELFARE", "DeductionTotal", "NetSalary",
                    "Othere(OT)", "Vehicle(Adj)"
                    , "Other(Bonus)", "LeaveWOPay", "GP", "Travel", "ChildAllowance", "MOBILE(Allowance)",
                    "OtherAdjustment", "TotalAdjustment", "NetPayment"
                );
            }
            catch (Exception e)
            {
                FileLogger.Log(
                    e.Message.ToString() + Environment.NewLine + e.Source.ToString() + Environment.NewLine +
                    e.StackTrace.ToString(), this.GetType().Name, e.Message.ToString());
            }

            return dt;
        }

        private DataTable TIBSalarySheet3(DataTable dt)
        {
            try
            {
                string[] CheckColumn = new string[]
                {
                    "AbsentDeduction", "Adj.forSalaryProgramme", "Arrear", "AssetsLostAdj.", "BasicAdj", "CPFAdj",
                    "FestivalAllowanceAdj", "HARDSHIP", "HardshipAllowanceAdj", "HouserentAdj", "LeaveEncash",
                    "MedialAllow.Adj", "Other(Deduction)", "Other(Earning)", "Other(PF)", "Other(Salary)",
                    "Other(StaffWF)", "PayableAdj.InsurancePremium", "PreEmploymentCheckUp", "Punishment",
                    "ReimbursableExpense", "SalaryAdj", "TaxConsultantFee", "TaxDues",
                    "Welfare/EnjoyedExcessEL/ELAdjustment"
                };

                dt = Ordinary.DtRemoveEmptyColumn(dt, CheckColumn);
                var dataView = new DataView(dt);
                dt = dataView.ToTable(true, "Project", "Section", "Designation", "HARDSHIP", "Other(Salary)");
            }
            catch (Exception e)
            {
                FileLogger.Log(
                    e.Message.ToString() + Environment.NewLine + e.Source.ToString() + Environment.NewLine +
                    e.StackTrace.ToString(), this.GetType().Name, e.Message.ToString());
            }

            return dt;
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

        private void BracExcelSalarySheet(DataTable dt, ExcelWorksheet workSheet, SalarySheetVM vm)
        {
            #region brac

            #region Notes

            ////////----------------Last Update - 25 February 2019--------------
            ////////ReportId	    ReportName
            ////////SalarySheet1	Full Salary Sheet
            ////////SalarySheet2	Bank Pay
            ////////SalarySheet3	Salary Summary
            ////////SalarySheet5	Pay Slip
            ////////SalarySheet6	pay Slip (Email)
            ////////SalarySheet7	Salary Certificate
            ////////SalarySheet8	Full Salary Sheet (Decimal)

            //////string[] downloadableSheet = { "SalarySheet1", "SalarySheet2", "SalarySheet3", "SalarySheet8" };

            //////if (!downloadableSheet.Contains(vm.SheetName))
            //////{
            //////    Session["result"] = "Fail~This Report is not available in Excel!";
            //////    return Redirect("SalarySheet");
            //////}

            #endregion

            #region Column Name Chagne

            dt = Ordinary.DtColumnNameChange(dt, "EmpName", "Name");
            dt = Ordinary.DtColumnNameChange(dt, "Other1", "Designation");
            dt = Ordinary.DtColumnNameChange(dt, "PreEmploymentCheckUp", "Pre-Employment Check Up");
            dt = Ordinary.DtColumnNameChange(dt, "MobileBill", "Excess Mobile Bill");

            string[] DtcolumnName = new string[dt.Columns.Count];
            int j = 0;
            foreach (DataColumn column in dt.Columns)
            {
                DtcolumnName[j] = column.ColumnName;
                j++;
            }

            for (int k = 0; k < DtcolumnName.Length; k++)
            {
                dt = Ordinary.DtColumnNameChange(dt, DtcolumnName[k], Ordinary.AddSpacesToSentence(DtcolumnName[k]));
            }

            dt = Ordinary.DtColumnNameChange(dt, "Code", "BESL ID");
            dt = Ordinary.DtColumnNameChange(dt, "Section", "Cost Center");
            dt = Ordinary.DtColumnNameChange(dt, "PFEmployee", "PF By Employee");

            #endregion

            #region Report Headers, Rows, Columns

            string Line1 = "BRAC EPL STOCK BROKERAGE LIMITED";
            string Line2 = "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
            string Line3 = "Salary Sheet for the Month of " + vm.FullPeriodName;


            int LeftColumn = 5;
            int CenterColumn = 5;

            switch (vm.SheetName)
            {
                case "SalarySheet2": ////----Bank Pay
                    LeftColumn = 3;
                    Line3 = "Salary for the Month of " + vm.FullPeriodName;
                    break;
                case "SalarySheet3": ////----Salary Summary
                    LeftColumn = 3;
                    CenterColumn = 3;
                    Line3 = "Salary Summary for the Month of " + vm.FullPeriodName;
                    break;
                default:
                    break;
            }


            string[] ReportHeaders = new string[] { "", Line1, Line2, Line3 };


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

            #endregion


            workSheet.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

            #region Format

            workSheet.Cells[
                "B" + (TableHeadRow + 1) + ":" + Ordinary.Alphabet[(ColumnCount + 1)] +
                (TableHeadRow + 1 + RowCount + 3)].Style.Numberformat.Format = "#,##0";
            workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount + 1)] + (TableHeadRow)].Style
                .Font.Bold = true;
            workSheet.Cells[
                "A" + (RowCount + TableHeadRow + 1) + ":" + Ordinary.Alphabet[(ColumnCount + 1)] +
                (RowCount + TableHeadRow + 1)].Style.Font.Bold = true;
            workSheet.Cells[
                Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] +
                (RowCount + TableHeadRow + 1)].Style.Font.Bold = true;

            workSheet.Cells[
                    "A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)]
                .Style
                .Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[
                    "A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style
                .Border.Left.Style = ExcelBorderStyle.Thin;

            var format = new OfficeOpenXml.ExcelTextFormat();
            format.Delimiter = '~';
            format.TextQualifier = '"';
            format.DataTypes = new[] { eDataTypes.String };


            for (int i = 0; i < ReportHeaders.Length; i++)
            {
                workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Merge = true;
                workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.HorizontalAlignment =
                    ExcelHorizontalAlignment.Left;
                workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.Font.Bold = true;
                workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.Font.Size = 14 - i;
                workSheet.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);
            }

            workSheet.Row(TableHeadRow).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(TableHeadRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(TableHeadRow).Style.WrapText = true;

            workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

            #region Grand Total

            for (int i = LeftColumn + 1; i <= ColumnCount; i++)
            {
                workSheet.Cells[GrandTotalRow, i].Formula = "=Sum(" + workSheet.Cells[TableHeadRow + 1, i].Address +
                                                            ":" + workSheet.Cells[(TableHeadRow + RowCount), i]
                                                                .Address + ")";
            }

            #endregion

            object sumObject;
            sumObject = dt.Compute("Sum([Net Salary])", string.Empty);

            decimal NetSalary = Convert.ToDecimal(sumObject);

            string strNetSalary = NetSalary.ToString("0.##");

            string NetSalaryInWords = Ordinary.ConvertToWords(strNetSalary, true);

            workSheet.Row(InWordsRow).Style.WrapText = true;
            workSheet.Row(InWordsRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheet.Cells[TableHeadRow + RowCount + 2, 2, TableHeadRow + RowCount + 2, ColumnCount].Merge = true;
            workSheet.Cells[TableHeadRow + RowCount + 2, 2, TableHeadRow + RowCount + 2, ColumnCount].Style.Font.Bold =
                true;
            workSheet.Cells[TableHeadRow + RowCount + 2, 1].LoadFromText("Net Salary (In Words):");
            workSheet.Cells[TableHeadRow + RowCount + 2, 2].LoadFromText(NetSalaryInWords);

            workSheet.Column(CenterColumn).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells[TableHeadRow, 1, GrandTotalRow - 1, 1].Style.HorizontalAlignment =
                ExcelHorizontalAlignment.Center;

            //////workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            if (vm.SheetName == "SalarySheet1") ////Full Salary Sheet
            {
                #region Signature

                string signatory1Title = "Prepared By";
                string signatory2Title = "Checked By";
                string signatory3Title = "Checked By";
                string signatory4Title = "Approved By";


                int signatory1Column = (ColumnCount / 4) - 2;
                int signatory2Column = (ColumnCount / 4) * 2 - 2;
                int signatory3Column = (ColumnCount / 4) * 3 - 2;
                int signatory4Column = (ColumnCount / 4) * 4 - 2;


                workSheet.Cells[SignatureRow, signatory1Column].LoadFromText(signatory1Title);
                workSheet.Cells[SignatureRow, signatory2Column].LoadFromText(signatory2Title);
                workSheet.Cells[SignatureRow, signatory3Column].LoadFromText(signatory3Title);
                workSheet.Cells[SignatureRow, signatory4Column].LoadFromText(signatory4Title);


                workSheet.Cells[SignatureRow, signatory1Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[SignatureRow, signatory2Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[SignatureRow, signatory3Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[SignatureRow, signatory4Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;


                workSheet.Row(SignatureRow).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(SignatureRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(SignatureRow).Style.WrapText = true;
                workSheet.Row(SignatureRow).Style.Font.Bold = true;

                #endregion
            }

            #endregion

            #endregion
        }

        #region Backup

        public ActionResult DownloadSalarySheetBackup(HttpPostedFileBase file, string fid, string ProjectId,
            string DepartmentId, string SectionId
            , string DesignationId, string CodeF, string CodeT, string MulitpleProjectId
            , string other1, string other2, string other3, string Orderby, string bankId, string SheetName
        )
        {
            return null;

            #region Comments

            //////            string[] result = new string[6];
            //////            DataTable dt = new DataTable();
            //////            List<string> ProjectIdList = new List<string>();
            //////            try
            //////            {
            //////                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

            //////                var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "add").ToString();
            //////                Session["permission"] = permission;
            //////                if (permission == "False")
            //////                {
            //////                    return Redirect("/Payroll/Home");
            //////                }

            //////                string FileName = "Download.xls";
            //////                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
            //////                //string fullPath = @"C:\";
            //////                if (System.IO.File.Exists(fullPath + FileName))
            //////                {
            //////                    System.IO.File.Delete(fullPath + FileName);
            //////                }
            //////                //SalaryProcessRepo _empRepo = new SalaryProcessRepo();
            //////                //dt = _empRepo.ExportExcelFile(fullPath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, ProjectIdList
            //////                //    , other1, other2, other3, Orderby, bankId);
            //////                #region Parmeters
            //////                string vProjectId = "0_0";
            //////                string vDepartmentId = "0_0";
            //////                string vSectionId = "0_0";
            //////                string vDesignationId = "0_0";
            //////                string vCodeF = "0_0";
            //////                string vCodeT = "0_0";
            //////                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
            //////                {
            //////                    vProjectId = ProjectId;
            //////                }
            //////                if (MulitpleProjectId != "0_0" && MulitpleProjectId != "0" && MulitpleProjectId != "" && MulitpleProjectId != "null" && MulitpleProjectId != null)
            //////                {
            //////                    ProjectIdList = MulitpleProjectId.Split(',').ToList();
            //////                }
            //////                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
            //////                    vDepartmentId = DepartmentId;
            //////                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
            //////                    vSectionId = SectionId;
            //////                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
            //////                    vDesignationId = DesignationId;
            //////                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
            //////                    vCodeF = CodeF;
            //////                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
            //////                    vCodeT = CodeT;
            //////                #endregion

            //////                ReportDocument doc = new ReportDocument();

            //////                SalaryProcessRepo _empRepo = new SalaryProcessRepo();
            //////                DataSet ds = new DataSet();

            //////                string OrderByCode = "";
            //////                if (Orderby == "CODE")
            //////                {
            //////                    OrderByCode = Orderby;
            //////                }

            //////                ds = _empRepo.SalaryPreCalculationNew(fid, vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT
            //////                    , Orderby, other1, other2, other3, bankId, ProjectIdList);


            //////                if (ds.Tables[0].Rows.Count == 0)
            //////                {
            //////                    result[0] = "Fail";
            //////                    result[1] = "No Data Found";
            //////                    Session["result"] = result[0] + "~" + result[1];

            //////                    return View();
            //////                }

            //////                dt = ds.Tables[0];
            //////                string PeriodName = dt.Rows[0]["PeriodName"].ToString();
            //////                string filename = "SalarySheet" + "-" + PeriodName;

            //////                if (CompanyName == "brac")
            //////                {
            //////                    #region Brac
            //////                    #region Notes
            //////                    ////////----------------Last Update - 13 August 2018--------------
            //////                    ////////ReportId	    ReportName
            //////                    ////////SalarySheet1	Full Salary Sheet
            //////                    ////////SalarySheet2	Bank Pay
            //////                    ////////SalarySheet3	Salary Summary
            //////                    ////////SalarySheet5	Pay Slip
            //////                    ////////SalarySheet6	pay Slip (Email)
            //////                    ////////SalarySheet7	Salary Certificate
            //////                    #endregion
            //////                    #region Brac Excel Salary Sheet (Full Salary Sheet)
            //////                    #region Column Selection

            //////                    string[] shortColumnName = {           
            //////  "FiscalYearDetailId"                                   //01
            //////, "EmployeeId"                                           //02
            //////, "Code"                                                 //03
            //////, "EmpName"                                              //04
            //////, "Department"                                           //05
            //////, "Basic"                                                //06
            //////, "HouseRent"                                            //07
            //////, "Medical"                                              //08
            //////, "Conveyance"                                           //09
            //////, "Gross"                                                //10
            //////, "Arrear"                                               //11
            //////, "ReimbursableExpense"                                  //12
            //////, "PreEmploymentCheckUp"                                 //13
            //////, "OtherAllowanceMonthly"                                //14
            //////, "MobileBill"                                           //15
            //////, "TAX"                                                  //16
            //////, "AdvanceDeduction"                                     //17
            //////, "PFEmployee"                                           //18
            //////, "TotalLoan"                                            //19
            //////, "OtherDeductionMonthly"                                //20
            //////, "DOM"                                                  //21
            //////, "PayDays"                                              //22
            //////, "NPDays"                                               //23
            //////, "BankAccountNo"                                        //24
            //////, "Section"                                              //25
            //////, "SalaryDeduction"                                      //26
            //////, "TransportBill"                                        //27
            //////                                               };
            //////                    Type[] columnTypes = {                  
            //////  typeof(String)                                         //01
            //////, typeof(String)                                         //02
            //////, typeof(String)                                         //03
            //////, typeof(String)                                         //04
            //////, typeof(String)                                         //05
            //////, typeof(decimal)                                        //06
            //////, typeof(decimal)                                        //07
            //////, typeof(decimal)                                        //08
            //////, typeof(decimal)                                        //09
            //////, typeof(decimal)                                        //10
            //////, typeof(decimal)                                        //11
            //////, typeof(decimal)                                        //12
            //////, typeof(decimal)                                        //13
            //////, typeof(decimal)                                        //14
            //////, typeof(decimal)                                        //15
            //////, typeof(decimal)                                        //16
            //////, typeof(decimal)                                        //17
            //////, typeof(decimal)                                        //18
            //////, typeof(decimal)                                        //19
            //////, typeof(decimal)                                        //20
            //////, typeof(int)                                            //21
            //////, typeof(decimal)                                        //22
            //////, typeof(int)                                            //23
            //////, typeof(string)                                         //24
            //////, typeof(string)                                         //25
            //////, typeof(decimal)                                        //26
            //////, typeof(decimal)                                        //27
            //////                                         };


            //////                    dt = Ordinary.DtSetColumnsOrder(dt, shortColumnName);
            //////                    dt = Ordinary.DtSelectedColumn(dt, shortColumnName, columnTypes);

            //////                    if (Session["LabelOther1"].ToString() != "")
            //////                    {
            //////                        dt = Ordinary.DtColumnNameChange(dt, "Other1", Session["LabelOther1"].ToString());
            //////                    }

            //////                    dt = Ordinary.DtColumnNameChange(dt, "Code", "BESL ID");

            //////                    #endregion


            //////                    dt.Columns.Add("OtherDeduction", typeof(decimal));
            //////                    dt.Columns.Add("TotalEarning", typeof(decimal));
            //////                    dt.Columns.Add("TotalDeduction", typeof(decimal));
            //////                    dt.Columns.Add("NetSalary", typeof(decimal));
            //////                    #region Declarations
            //////                    decimal vBasic = 0;
            //////                    decimal vHouseRent = 0;
            //////                    decimal vMedical = 0;
            //////                    decimal vConveyance = 0;
            //////                    decimal vGross = 0;
            //////                    decimal vDOM = 0;
            //////                    decimal vPayDays = 0;
            //////                    decimal vArrear = 0;

            //////                    decimal vReimbursableExpense = 0;


            //////                    decimal vPreEmploymentCheckUp = 0;
            //////                    decimal vAdvanceDeduction = 0;
            //////                    decimal vTAX = 0;
            //////                    decimal vPFEmployee = 0;
            //////                    decimal vMobileBill = 0;
            //////                    decimal vTotalLoan = 0;
            //////                    decimal vTotalEarning = 0;
            //////                    decimal vTotalDeduction = 0;
            //////                    decimal vNPDays = 0;

            //////                    decimal vOtherAllowanceMonthly = 0;
            //////                    decimal vOtherDeductionMonthly = 0;
            //////                    decimal vSalaryDeduction = 0;
            //////                    decimal vTransportBill = 0;

            //////                    decimal vOtherDeduction = 0;


            //////                    #endregion

            //////                    #region Settings
            //////                    SettingRepo _settingsRepo = new SettingRepo();
            //////                    int decimalPlace = Convert.ToInt32(_settingsRepo.settingValue("SalarySheet", "DecimalPlace"));
            //////                    #endregion


            //////                    int i = 0;
            //////                    foreach (var item in dt.Rows)
            //////                    {
            //////                        #region Variables
            //////                        vBasic = 0;
            //////                        vHouseRent = 0;
            //////                        vMedical = 0;
            //////                        vConveyance = 0;
            //////                        vGross = 0;
            //////                        vDOM = 0;
            //////                        vPayDays = 0;
            //////                        vArrear = 0;
            //////                        vReimbursableExpense = 0;

            //////                        vPreEmploymentCheckUp = 0;
            //////                        vAdvanceDeduction = 0;
            //////                        vTAX = 0;
            //////                        vPFEmployee = 0;
            //////                        vMobileBill = 0;
            //////                        vTotalLoan = 0;
            //////                        vTotalEarning = 0;
            //////                        vTotalDeduction = 0;
            //////                        vNPDays = 0;

            //////                        vOtherAllowanceMonthly = 0;
            //////                        vOtherDeductionMonthly = 0;
            //////                        vSalaryDeduction = 0;
            //////                        vTransportBill = 0;
            //////                        vOtherDeduction = 0;


            //////                        #endregion
            //////                        #region Value Assign

            //////                        vBasic = Convert.ToDecimal(dt.Rows[i]["Basic"]);
            //////                        vHouseRent = Convert.ToDecimal(dt.Rows[i]["HouseRent"]);
            //////                        vMedical = Convert.ToDecimal(dt.Rows[i]["Medical"]);
            //////                        vConveyance = Convert.ToDecimal(dt.Rows[i]["Conveyance"]);
            //////                        vGross = Convert.ToDecimal(dt.Rows[i]["Gross"]);
            //////                        vDOM = Convert.ToDecimal(dt.Rows[i]["DOM"]);
            //////                        vPayDays = Convert.ToDecimal(dt.Rows[i]["PayDays"]);
            //////                        vArrear = Convert.ToDecimal(dt.Rows[i]["Arrear"]);
            //////                        vReimbursableExpense = Convert.ToDecimal(dt.Rows[i]["ReimbursableExpense"]);

            //////                        vPreEmploymentCheckUp = Convert.ToDecimal(dt.Rows[i]["PreEmploymentCheckUp"]);
            //////                        vAdvanceDeduction = Convert.ToDecimal(dt.Rows[i]["AdvanceDeduction"]);
            //////                        vTAX = Convert.ToDecimal(dt.Rows[i]["TAX"]);
            //////                        vPFEmployee = Convert.ToDecimal(dt.Rows[i]["PFEmployee"]);
            //////                        vMobileBill = Convert.ToDecimal(dt.Rows[i]["MobileBill"]);
            //////                        vTotalLoan = Convert.ToDecimal(dt.Rows[i]["TotalLoan"]);
            //////                        vNPDays = Convert.ToDecimal(dt.Rows[i]["NPDays"]);

            //////                        vOtherAllowanceMonthly = Convert.ToDecimal(dt.Rows[i]["OtherAllowanceMonthly"]);
            //////                        vOtherDeductionMonthly = Convert.ToDecimal(dt.Rows[i]["OtherDeductionMonthly"]);

            //////                        vSalaryDeduction = Convert.ToDecimal(dt.Rows[i]["SalaryDeduction"]);
            //////                        vTransportBill = Convert.ToDecimal(dt.Rows[i]["TransportBill"]);

            //////                        #endregion

            //////                        #region Calculation
            //////                        if (vNPDays > 0)
            //////                        {
            //////                            vGross = vGross / vDOM * vPayDays;
            //////                            dt.Rows[i]["Basic"] = vBasic / vDOM * vPayDays;
            //////                            dt.Rows[i]["HouseRent"] = vHouseRent / vDOM * vPayDays;
            //////                            dt.Rows[i]["Medical"] = vMedical / vDOM * vPayDays;
            //////                            dt.Rows[i]["Conveyance"] = vConveyance / vDOM * vPayDays;
            //////                            dt.Rows[i]["Gross"] = vGross;

            //////                        }

            //////                        vOtherDeduction = vSalaryDeduction + vTransportBill + vOtherDeductionMonthly;

            //////                        vTotalEarning = vGross + vArrear + vReimbursableExpense + vPreEmploymentCheckUp + vOtherAllowanceMonthly;
            //////                        vTotalDeduction = vAdvanceDeduction + vTAX + vPFEmployee + vMobileBill + vTotalLoan + vOtherDeduction;

            //////                        dt.Rows[i]["OtherDeduction"] = vOtherDeduction;
            //////                        dt.Rows[i]["TotalEarning"] = vTotalEarning;
            //////                        dt.Rows[i]["TotalDeduction"] = vTotalDeduction;
            //////                        dt.Rows[i]["NetSalary"] = vTotalEarning - vTotalDeduction;

            //////                        #endregion

            //////                        #region Value Rounding

            //////                        //////////dt.Rows[i]["Basic"] = Ordinary.NumericFormat(dt.Rows[i]["Basic"], decimalPlace);
            //////                        //////////dt.Rows[i]["HouseRent"] = Ordinary.NumericFormat(dt.Rows[i]["HouseRent"], decimalPlace);
            //////                        //////////dt.Rows[i]["Medical"] = Ordinary.NumericFormat(dt.Rows[i]["Medical"], decimalPlace);
            //////                        //////////dt.Rows[i]["Conveyance"] = Ordinary.NumericFormat(dt.Rows[i]["Conveyance"], decimalPlace);
            //////                        //////////dt.Rows[i]["Gross"] = Ordinary.NumericFormat(dt.Rows[i]["Gross"], decimalPlace);
            //////                        //////////dt.Rows[i]["Arrear"] = Ordinary.NumericFormat(dt.Rows[i]["Arrear"], decimalPlace);

            //////                        //////////dt.Rows[i]["ReimbursableExpense"] = Ordinary.NumericFormat(dt.Rows[i]["ReimbursableExpense"], decimalPlace);

            //////                        //////////dt.Rows[i]["PreEmploymentCheckUp"] = Ordinary.NumericFormat(dt.Rows[i]["PreEmploymentCheckUp"], decimalPlace);
            //////                        //////////dt.Rows[i]["MobileBill"] = Ordinary.NumericFormat(dt.Rows[i]["MobileBill"], decimalPlace);
            //////                        //////////dt.Rows[i]["TAX"] = Ordinary.NumericFormat(dt.Rows[i]["TAX"], 2);
            //////                        //////////dt.Rows[i]["AdvanceDeduction"] = Ordinary.NumericFormat(dt.Rows[i]["AdvanceDeduction"], decimalPlace);
            //////                        //////////dt.Rows[i]["PFEmployee"] = Ordinary.NumericFormat(dt.Rows[i]["PFEmployee"], decimalPlace);
            //////                        //////////dt.Rows[i]["TotalEarning"] = Ordinary.NumericFormat(dt.Rows[i]["TotalEarning"], decimalPlace);
            //////                        //////////dt.Rows[i]["TotalDeduction"] = Ordinary.NumericFormat(dt.Rows[i]["TotalDeduction"], decimalPlace);
            //////                        //////////dt.Rows[i]["NetSalary"] = Ordinary.NumericFormat(dt.Rows[i]["NetSalary"], decimalPlace);
            //////                        //////////////////dt.Rows[i]["NetSalary"] = Math.Round( Convert.ToDecimal(dt.Rows[i]["NetSalary"]), decimalPlace);

            //////                        //////////dt.Rows[i]["OtherAllowanceMonthly"] = Ordinary.NumericFormat(dt.Rows[i]["OtherAllowanceMonthly"], decimalPlace);
            //////                        //////////dt.Rows[i]["OtherDeductionMonthly"] = Ordinary.NumericFormat(dt.Rows[i]["OtherDeductionMonthly"], decimalPlace);
            //////                        //////////dt.Rows[i]["TotalLoan"] = Ordinary.NumericFormat(dt.Rows[i]["TotalLoan"], decimalPlace);

            //////                        #endregion
            //////                        i++;
            //////                    }

            //////                    #region Make Reday Excel Sheet Data

            //////                    if (SheetName == "SalarySheet1")
            //////                    {

            //////                        string[] removeColumnName = { "FiscalYearDetailId", "EmployeeId", "DOM", "PayDays", "NPDays", "BankAccountNo", "Section", "SalaryDeduction", "TransportBill", "OtherDeductionMonthly" };
            //////                        dt = Ordinary.DtDeleteColumns(dt, removeColumnName);
            //////                    }
            //////                    else if (SheetName == "SalarySheet2") //--------Bank Pay
            //////                    {
            //////                        {
            //////                            string[] shortColumnNameNew = { "EmpName", "BankAccountNo", "NetSalary" };
            //////                            Type[] columnTypesNew = { typeof(String), typeof(String), typeof(decimal) };

            //////                            dt = Ordinary.DtSetColumnsOrder(dt, shortColumnNameNew);
            //////                            dt = Ordinary.DtSelectedColumn(dt, shortColumnNameNew, columnTypesNew);

            //////                            string[] conFields = { "ReportType" };
            //////                            string[] conValues = { "SalarySheet" };
            //////                            List<EnumReportVM> enumReportVMs = new EnumReportRepo().SelectAll(0, conFields, conValues);

            //////                            string ReportName = enumReportVMs.Where(c => c.ReportId == SheetName).FirstOrDefault().Name;
            //////                            filename = ReportName + "-" + PeriodName;
            //////                        }
            //////                    }
            //////                    else if (SheetName == "SalarySheet3") //--------Salary Summary
            //////                    {
            //////                        {
            //////                            var newSort = (from row in dt.AsEnumerable()
            //////                                           group row by new
            //////                                           {
            //////                                               Department = row.Field<string>("Department")
            //////                                               ,
            //////                                               Section = row.Field<string>("Section")
            //////                                           } into grp
            //////                                           select new
            //////                                           {
            //////                                               Department = grp.Key.Department,
            //////                                               Section = grp.Key.Section
            //////                                               ,
            //////                                               Gross = grp.Sum(r => r.Field<Decimal>("Gross"))
            //////                                               ,
            //////                                               Arrear = grp.Sum(r => r.Field<Decimal>("Arrear"))
            //////                                               ,
            //////                                               ReimbursableExpense = grp.Sum(r => r.Field<Decimal>("ReimbursableExpense"))
            //////                                               ,
            //////                                               PreEmploymentCheckUp = grp.Sum(r => r.Field<Decimal>("PreEmploymentCheckUp"))
            //////                                               ,
            //////                                               OtherAllowanceMonthly = grp.Sum(r => r.Field<Decimal>("OtherAllowanceMonthly"))
            //////                                               ,
            //////                                               TotalEarning = grp.Sum(r => r.Field<Decimal>("TotalEarning"))
            //////                                               ,
            //////                                               MobileBill = grp.Sum(r => r.Field<Decimal>("MobileBill"))
            //////                                               ,
            //////                                               TAX = grp.Sum(r => r.Field<Decimal>("TAX"))
            //////                                               ,
            //////                                               AdvanceDeduction = grp.Sum(r => r.Field<Decimal>("AdvanceDeduction"))
            //////                                               ,
            //////                                               PFEmployee = grp.Sum(r => r.Field<Decimal>("PFEmployee"))
            //////                                               ,
            //////                                               TotalLoan = grp.Sum(r => r.Field<Decimal>("TotalLoan"))
            //////                                               ,
            //////                                               OtherDeduction = grp.Sum(r => r.Field<Decimal>("OtherDeduction"))
            //////                                               ,
            //////                                               TotalDeduction = grp.Sum(r => r.Field<Decimal>("TotalDeduction"))
            //////                                               ,
            //////                                               NetSalary = grp.Sum(r => r.Field<Decimal>("NetSalary"))


            //////                                           }).ToList();
            //////                            dt = Ordinary.ToDataTable(newSort);


            //////                            EnumReportRepo _reportRepo = new EnumReportRepo();
            //////                            List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

            //////                            string[] conFields = { "ReportType" };
            //////                            string[] conValues = { "SalarySheet" };
            //////                            enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);

            //////                            string ReportName = enumReportVMs.Where(c => c.ReportId == SheetName).FirstOrDefault().Name;
            //////                            filename = ReportName + "-" + PeriodName;
            //////                        }
            //////                    }
            //////                    else
            //////                    {
            //////                        Session["result"] = "Fail" + "~" + "This Sheet is Not Available in Excel";
            //////                        return Redirect("SalarySheet");
            //////                    }
            //////                    #endregion

            //////                    #endregion
            //////                    #endregion
            //////                }
            //////                else if (CompanyName.ToLower() == "kbl" || CompanyName.ToLower() == "anupam" || CompanyName.ToLower() == "kajol")
            //////                {
            //////                    #region Kazal
            //////                    #region Notes
            //////                    ////////----------------Last Update - 08 November 2018--------------
            //////                    ////////ReportId	    ReportName
            //////                    ////////SalarySheet1	Full Salary --------------(Not Ready) 
            //////                    ////////SalarySheet2	Bank Pay    --------------(Not Ready)	
            //////                    ////////SalarySheet3	Cash Pay    --------------(Not Ready)	
            //////                    ////////SalarySheet4	Bank Sheet  --------------(Ready)
            //////                    #endregion

            //////                    dt = dtSalarySheetKazolBackup(dt);

            //////                    #region Make Reday Excel Sheet Data

            //////                    if (SheetName == "SalarySheet4") //--------Bank Sheet
            //////                    {
            //////                        {

            //////                            DataTable dtTemp = new DataTable();
            //////                            dtTemp = dt;

            //////                            var dtBankSheet = dtTemp.AsEnumerable().Where(r => r.Field<decimal>("NetSalaryBank") > 0);

            //////                            if (dtBankSheet == null || dtBankSheet.Count() == 0)
            //////                            {
            //////                                Session["result"] = "Fail" + "~" + "No Employee For Bank Sheet";
            //////                                return Redirect("SalarySheet");
            //////                            }

            //////                            dt = dtBankSheet.CopyToDataTable();

            //////                            string[] shortColumnNameNew = { "Code", "EmpName", "BankAccountNo", "Designation", "NetSalaryBank" };
            //////                            Type[] columnTypesNew = { typeof(String), typeof(String), typeof(String), typeof(String), typeof(decimal) };

            //////                            dt = Ordinary.DtSetColumnsOrder(dt, shortColumnNameNew);
            //////                            dt = Ordinary.DtSelectedColumn(dt, shortColumnNameNew, columnTypesNew);

            //////                        }
            //////                    }
            //////                    else
            //////                    {
            //////                        Session["result"] = "Fail" + "~" + "This Sheet is Not Available in Excel";
            //////                        return Redirect("SalarySheet");
            //////                    }


            //////                    string[] conFields = { "ReportType" };
            //////                    string[] conValues = { "SalarySheet" };
            //////                    List<EnumReportVM> enumReportVMs = new EnumReportRepo().SelectAll(0, conFields, conValues);

            //////                    string ReportName = enumReportVMs.Where(c => c.ReportId == SheetName).FirstOrDefault().Name;
            //////                    filename = ReportName + "-" + PeriodName;


            //////                    #endregion
            //////                    #endregion
            //////                }

            //////                ExcelPackage excel = new ExcelPackage();
            //////                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            //////                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

            //////                ////////////workSheet.Cells[dt.Rows.Count + 4, 1].LoadFromText("Total");
            //////                ////////////for (int i = 0; i < dt.Columns.Count; i++)
            //////                ////////////{
            //////                ////////////    workSheet.Cells[dt.Rows.Count+4, 2 + i].Formula = "=Sum(" + workSheet.Cells[1, 2 + i].Address + ":" + workSheet.Cells[(dt.Rows.Count), (2 + i)].Address + ")";
            //////                ////////////}


            //////                workSheet.Cells["C2:" + Ordinary.Alphabet[(dt.Columns.Count + 1)] + (dt.Rows.Count + 3)].Style.Numberformat.Format = "#,##0";


            //////                using (var memoryStream = new MemoryStream())
            //////                {
            //////                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //////                    //Response.AddHeader("content-disposition", "attachment;  filename=" + FileName);
            //////                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
            //////                    excel.SaveAs(memoryStream);
            //////                    memoryStream.WriteTo(Response.OutputStream);
            //////                    Response.Flush();
            //////                    Response.End();
            //////                }
            //////                result[0] = "Success";
            //////                result[1] = "Successful~Data Download";


            //////                Session["result"] = result[0] + "~" + result[1];
            //////                return Redirect("SalarySheet");
            //////                //return Redirect("C:/" + FileName);
            //////            }
            //////            catch (Exception)
            //////            {
            //////                Session["result"] = result[0] + "~" + result[1];
            //////                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
            //////                return Redirect("SalarySheet");
            //////            }

            #endregion
        }

        #endregion

        public ActionResult _rptIndex(JQueryDataTableParamVM param, string fid, string ProjectId, string DepartmentId,
            string SectionId, string DesignationId, string CodeF, string CodeT, string PaySlip)
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
                fromDate = joinDateFilter.Split('~')[0] == ""
                    ? DateTime.MinValue
                    :
                    Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true
                        ?
                        Convert.ToDateTime(joinDateFilter.Split('~')[0])
                        : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == ""
                    ? DateTime.MaxValue
                    :
                    Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true
                        ?
                        Convert.ToDateTime(joinDateFilter.Split('~')[1])
                        : DateTime.MinValue;
            }

            #endregion Column Search

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

            #region Column Filtering

            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" ||
                (joinDateFilter != "" && joinDateFilter != "~"))
            {
                filteredData = filteredData
                    .Where(c => (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                && (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                && (departmentFilter == "" ||
                                    c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                && (designationFilter == "" || c.Designation.ToString().ToLower()
                                    .Contains(designationFilter.ToLower()))
                                && (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
                                && (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.JoinDate))
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
                sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.JoinDate) :
                sortColumnIndex == 6 && isSortable_6 ? c.Remarks :
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
                    Convert.ToString(c.Id), c.Code //+ "~" + Convert.ToString(c.Id) 
                    ,
                    c.EmpName, c.Department, c.Designation, c.JoinDate.ToString()
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

        public ActionResult _salaryInfomationLoad()
        {
            return PartialView("_salaryInformation");
        }

        public ActionResult _salaryInfomation1(JQueryDataTableParamVM param, string FiscalPeriodId = null)
        {
            #region Column Search

            var SLNoFilter = Convert.ToString(Request["sSearch_0"]);
            var CodeFilter = Convert.ToString(Request["sSearch_1"]);
            var EmpNameFilter = Convert.ToString(Request["sSearch_2"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_3"]);
            var DesignationFilter = Convert.ToString(Request["sSearch_4"]);
            var DepartmentFilter = Convert.ToString(Request["sSearch_5"]);
            var ProjectFilter = Convert.ToString(Request["sSearch_6"]);
            var SectionFilter = Convert.ToString(Request["sSearch_7"]);
            var PeriodNameFilter = Convert.ToString(Request["sSearch_8"]);
            var SalaryTypeFilter = Convert.ToString(Request["sSearch_9"]);
            var TypeFilter = Convert.ToString(Request["sSearch_10"]);
            var AmountFilter = Convert.ToString(Request["sSearch_11"]);
            var AmountFrom = 0;
            var AmountTo = 0;
            if (AmountFilter.Contains('~'))
            {
                AmountFrom = AmountFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(AmountFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(AmountFilter.Split('~')[0])
                        : 0;
                AmountTo = AmountFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(AmountFilter.Split('~')[1]) == true
                        ?
                        Convert.ToInt32(AmountFilter.Split('~')[1])
                        : 0;
            }

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = joinDateFilter.Split('~')[0] == ""
                    ? DateTime.MinValue
                    :
                    Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true
                        ?
                        Convert.ToDateTime(joinDateFilter.Split('~')[0])
                        : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == ""
                    ? DateTime.MaxValue
                    :
                    Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true
                        ?
                        Convert.ToDateTime(joinDateFilter.Split('~')[1])
                        : DateTime.MinValue;
            }

            //var fromID = 0;
            //var toID = 0;
            //if (idFilter.Contains('~'))
            //{
            //    //Split number range filters with ~
            //    fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
            //    toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            //}

            #endregion Column Search

            SalaryProcessRepo _salaryProcessRepo = new SalaryProcessRepo();
            var getAllData = _salaryProcessRepo.SalaryInfomation();
            IEnumerable<SalaryInformationVM> filteredData;
            //Check whether the companies should be filtered by keyword
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
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable10 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable11 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable12 = Convert.ToBoolean(Request["bSearchable_3"]);
                filteredData = getAllData
                    .Where(c => isSearchable1 && c.SLNo.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable2 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable3 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable4 && c.JoinDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable5 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable6 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable7 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable8 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable9 && c.PeriodName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable10 && c.Type.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable11 && c.SalaryType.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable12 && c.Amount.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable9 && c.FiscalYearDetailId.ToString().ToLower()
                                    .Contains(param.sSearch.ToLower())
                                ||
                                isSearchable10 && c.DepartmentId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable11 && c.SectionId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable12 && c.ProjectId.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #region Column Filtering

            if (SLNoFilter != "" || CodeFilter != "" || EmpNameFilter != ""
                || (AmountFilter != "" && AmountFilter != "~")
                || (joinDateFilter != "" && joinDateFilter != "~")
                || SalaryTypeFilter != ""
                || PeriodNameFilter != ""
                || SectionFilter != ""
                || ProjectFilter != ""
                || TypeFilter != ""
               )
            {
                filteredData = filteredData
                    .Where(c =>
                        (SLNoFilter == "" || c.SLNo.ToLower().Contains(SLNoFilter.ToLower()))
                        &&
                        (CodeFilter == "" || c.Code.ToLower().Contains(CodeFilter.ToLower()))
                        &&
                        (EmpNameFilter == "" || c.EmpName.ToLower().Contains(EmpNameFilter.ToLower()))
                        &&
                        (SalaryTypeFilter == "" || c.SalaryType.ToLower().Contains(SalaryTypeFilter.ToLower()))
                        &&
                        (SLNoFilter == "" || c.SLNo.ToLower().Contains(SLNoFilter.ToLower()))
                        &&
                        (SLNoFilter == "" || c.SLNo.ToLower().Contains(SLNoFilter.ToLower()))
                        &&
                        (SLNoFilter == "" || c.SLNo.ToLower().Contains(SLNoFilter.ToLower()))
                        &&
                        (SLNoFilter == "" || c.SLNo.ToLower().Contains(SLNoFilter.ToLower()))
                        &&
                        (SLNoFilter == "" || c.SLNo.ToLower().Contains(SLNoFilter.ToLower()))
                        &&
                        (SLNoFilter == "" || c.SLNo.ToLower().Contains(SLNoFilter.ToLower()))
                    );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryInformationVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.SLNo :
                sortColumnIndex == 2 && isSortable_2 ? c.Code.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.EmpName.ToString() :
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
                    Convert.ToString(c.SLNo), c.Code.ToString(), c.EmpName.ToString(), c.JoinDate.ToString(),
                    c.Designation.ToString(), c.Department.ToString(), c.Project.ToString(), c.Section.ToString(),
                    c.PeriodName.ToString(), c.SalaryType.ToString(), c.Type.ToString(), c.Amount.ToString()
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

        public ActionResult _salaryInfomation(JQueryDataTableParamVM param, string ProjectId = null,
            string SectionId = null, string DepartmentId = null, string FiscalPeriodId = null)
        {
            #region Column Search

            var CodeFilter = Convert.ToString(Request["sSearch_0"]);
            var EmpNameFilter = Convert.ToString(Request["sSearch_1"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_2"]);
            var DesignationFilter = Convert.ToString(Request["sSearch_3"]);
            var DepartmentFilter = Convert.ToString(Request["sSearch_4"]);
            var ProjectFilter = Convert.ToString(Request["sSearch_5"]);
            var SectionFilter = Convert.ToString(Request["sSearch_6"]);
            var PeriodNameFilter = Convert.ToString(Request["sSearch_7"]);
            var SalaryTypeFilter = Convert.ToString(Request["sSearch_8"]);
            var TypeFilter = Convert.ToString(Request["sSearch_9"]);
            var AmountFilter = Convert.ToString(Request["sSearch_10"]);
            var AmountFrom = 0;
            var AmountTo = 0;
            if (AmountFilter.Contains('~'))
            {
                AmountFrom = AmountFilter.Split('~')[0] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(AmountFilter.Split('~')[0]) == true
                        ?
                        Convert.ToInt32(AmountFilter.Split('~')[0])
                        : 0;
                AmountTo = AmountFilter.Split('~')[1] == ""
                    ? 0
                    :
                    Ordinary.IsInteger(AmountFilter.Split('~')[1]) == true
                        ?
                        Convert.ToInt32(AmountFilter.Split('~')[1])
                        : 0;
            }

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = joinDateFilter.Split('~')[0] == ""
                    ? DateTime.MinValue
                    :
                    Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true
                        ?
                        Convert.ToDateTime(joinDateFilter.Split('~')[0])
                        : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == ""
                    ? DateTime.MaxValue
                    :
                    Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true
                        ?
                        Convert.ToDateTime(joinDateFilter.Split('~')[1])
                        : DateTime.MinValue;
            }

            //var fromID = 0;
            //var toID = 0;
            //if (idFilter.Contains('~'))
            //{
            //    //Split number range filters with ~
            //    fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
            //    toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            //}

            #endregion Column Search

            SalaryProcessRepo _salaryProcessRepo = new SalaryProcessRepo();
            var getAllData = _salaryProcessRepo.SalaryInfomation();
            if (FiscalPeriodId != "null" && FiscalPeriodId != "" && FiscalPeriodId != null && FiscalPeriodId != "0" &&
                FiscalPeriodId != "0_0")
                getAllData = getAllData.Where(m => m.FiscalYearDetailId == FiscalPeriodId).ToList();
            if (DepartmentId != "null" && DepartmentId != "" && DepartmentId != null && DepartmentId != "0" &&
                DepartmentId != "0_0")
                getAllData = getAllData.Where(m => m.DepartmentId == DepartmentId).ToList();
            if (SectionId != "null" && SectionId != "" && SectionId != null && SectionId != "0" && SectionId != "0_0")
                getAllData = getAllData.Where(m => m.SectionId == SectionId).ToList();
            if (ProjectId != "null" && ProjectId != "" && ProjectId != null && ProjectId != "0" && ProjectId != "0_0")
                getAllData = getAllData.Where(m => m.ProjectId == ProjectId).ToList();
            IEnumerable<SalaryInformationVM> filteredData;
            //Check whether the companies should be filtered by keyword
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
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable10 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable11 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable12 = Convert.ToBoolean(Request["bSearchable_3"]);
                filteredData = getAllData
                    .Where(c => isSearchable2 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable3 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable4 && c.JoinDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable5 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable6 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable7 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable8 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable9 && c.PeriodName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable10 && c.Type.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable11 && c.SalaryType.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable12 && c.Amount.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable9 && c.FiscalYearDetailId.ToString().ToLower()
                                    .Contains(param.sSearch.ToLower())
                                ||
                                isSearchable10 && c.DepartmentId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable11 && c.SectionId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable12 && c.ProjectId.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #region Column Filtering

            if (CodeFilter != ""
                || EmpNameFilter != ""
                || (AmountFilter != "" && AmountFilter != "~")
                || (joinDateFilter != "~" && joinDateFilter != "")
                || SalaryTypeFilter != ""
                || PeriodNameFilter != ""
                || SectionFilter != ""
                || ProjectFilter != ""
                || TypeFilter != ""
                || DesignationFilter != ""
                || DepartmentFilter != ""
               )
            {
                filteredData = filteredData
                    .Where(c => (CodeFilter == "" || c.Code.ToLower().Contains(CodeFilter.ToLower()))
                                &&
                                (EmpNameFilter == "" || c.EmpName.ToLower().Contains(EmpNameFilter.ToLower()))
                                &&
                                (SalaryTypeFilter == "" || c.SalaryType.ToLower().Contains(SalaryTypeFilter.ToLower()))
                                &&
                                (TypeFilter == "" || c.Type.ToLower().Contains(TypeFilter.ToLower()))
                                &&
                                (ProjectFilter == "" || c.Project.ToLower().Contains(ProjectFilter.ToLower()))
                                &&
                                (DesignationFilter == "" ||
                                 c.Designation.ToLower().Contains(DesignationFilter.ToLower()))
                                &&
                                (DepartmentFilter == "" || c.Department.ToLower().Contains(DepartmentFilter.ToLower()))
                                &&
                                (SectionFilter == "" || c.Section.ToLower().Contains(SectionFilter.ToLower()))
                                &&
                                (PeriodNameFilter == "" || c.PeriodName.ToLower().Contains(PeriodNameFilter.ToLower()))
                                &&
                                (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
                                &&
                                (toDate == DateTime.MinValue || toDate >= Convert.ToDateTime(c.JoinDate))
                                &&
                                (AmountFrom == 0 || AmountFrom <= Convert.ToInt32(c.Amount))
                                &&
                                (AmountTo == 0 || AmountTo >= Convert.ToInt32(c.Amount))
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
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_9"]);
            var isSortable_10 = Convert.ToBoolean(Request["bSortable_10"]);
            var isSortable_11 = Convert.ToBoolean(Request["bSortable_11"]);
            var isSortable_12 = Convert.ToBoolean(Request["bSortable_12"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryInformationVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.SLNo :
                sortColumnIndex == 2 && isSortable_2 ? c.Code.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.EmpName.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.SalaryType.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.Amount.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? Ordinary.DateToString(c.JoinDate) :
                sortColumnIndex == 7 && isSortable_7 ? c.PeriodName.ToString() :
                sortColumnIndex == 8 && isSortable_8 ? c.Section.ToString() :
                sortColumnIndex == 9 && isSortable_9 ? c.Department.ToString() :
                sortColumnIndex == 10 && isSortable_10 ? c.Designation.ToString() :
                sortColumnIndex == 11 && isSortable_11 ? c.Project.ToString() :
                sortColumnIndex == 12 && isSortable_12 ? c.Type.ToString() :
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
                    c.Code.ToString(), c.EmpName.ToString(), c.JoinDate.ToString(), c.Designation.ToString(),
                    c.Department.ToString(), c.Project.ToString(), c.Section.ToString(), c.PeriodName.ToString(),
                    c.SalaryType.ToString(), c.Type.ToString(), c.Amount.ToString()
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

        public ActionResult BKash(string test, string test1)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }

            MyBkash vm = new MyBkash();
            vm.Amount = 5000;
            vm.TRXREF = "Ref Kamrul POS";
            vm.TRXID = "6025334221";
            return View(vm);
        }

        [HttpPost]
        public ActionResult BKash(MyBkash vm)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                                                       SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = new
                    RemoteCertificateValidationCallback
                    (
                        delegate { return true; }
                    );
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                    "https://bkash.qcashbd.com:8080/BkashWebProject/service_connector/ServiceConsume/trxcheck");
                string postData =
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?><REQUEST><USER>RTHD</USER><MSISDN>01777782628</MSISDN><TRXID>" +
                    vm.TRXID + "</TRXID><TRXREF>kamrul</TRXREF></REQUEST>";
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] byteArray = encoding.GetBytes(postData);
                request.ContentType = "application/xml";
                request.Accept = "application/xml";
                request.ContentLength = byteArray.Length;
                request.Method = "POST";
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Flush();
                dataStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                string responseValue = new StreamReader(resStream).ReadToEnd();
                //var responseValue = Request.Form["xmlmsg"];
                StringReader stream = null;
                XmlTextReader reader = null;
                DataSet xmlDS = new DataSet();
                stream = new StringReader(responseValue);
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                //MessageBox.Show(responseStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View();
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
 
        public void EmpEmailProcessTIB(DataSet ds, ReportDocument rptDoc, string FiscalPeriod)
        {
            EmailSettings ems = new EmailSettings();
            SettingRepo _setDAL = new SettingRepo();
            ems.MailHeader = _setDAL.settingValue("Mail", "MailSubject");
            ems.MailHeader = ems.MailHeader.Replace("vmonth", FiscalPeriod);
            string mailbody = _setDAL.settingValue("Mail", "MailBody");
            DataSet DsTemp = new DataSet();
            string strSort = " Code";
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                try
                {
                    DsTemp = new DataSet();
                    dt1 = new DataTable();
                    dt2 = new DataTable();

                    DataRow[] DataRow1 = ds.Tables[0].Select("Code='" + item["Code"].ToString() + "'");
                    DataView dtview1 = new DataView(DataRow1.CopyToDataTable());
                    dtview1.Sort = strSort;
                    dt1 = dtview1.ToTable();

                    DataRow[] DataRow2 = ds.Tables[1].Select("Code='" + item["Code"] + "'");
                    DataView dtview2 = new DataView(DataRow2.CopyToDataTable());
                    dtview2.Sort = strSort;
                    dt2 = dtview2.ToTable();

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                                                                      SecurityProtocolType.Tls11 |
                                                                      SecurityProtocolType.Tls;

                    ems.MailToAddress = dt1.Rows[0]["Email"].ToString();

                    if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
                    {
                        dt1.TableName = "TIBSalary";
                        dt2.TableName = "DtTIBOtherHead";

                        ems.MailBody = mailbody.Replace("vmonth", FiscalPeriod)
                            .Replace("vname", item["EmpName"].ToString());
                        //ems.MailBody = mailbody;
                        //dt1.ImportRow(item);
                        ems.FileName = "Payslip of " + item["EmpName"].ToString() + " (" + FiscalPeriod + ")";
                        DsTemp.Tables.Add(dt1);
                        DsTemp.Tables.Add(dt2);
                        rptDoc.SetDataSource(DsTemp);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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

                            smpt.Send(mailmessage);
                            mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                            FileLogger.Log("EmpEmailProcessTIB", this.GetType().Name, "EmpEmail Send To:" + item["EmpName"].ToString() + Environment.NewLine + "EmpEmailAddress:" + dt1.Rows[0]["Email"].ToString());

                        }

                        //Thread.Sleep(500);
                        Thread.Sleep(3000);
                    }
                }
                catch (SmtpFailedRecipientException ex)
                {
                    FileLogger.Log("EmpEmailProcessTIB", this.GetType().Name,"EmpEmail Not Send To:" + item["EmpName"].ToString()+" "+ex.Message + Environment.NewLine + ex.StackTrace);

                    // throw ex;
                }
            }

            rptDoc.Close();
            thread.Abort();
        }

        public void EmpEmailProcessBollore(DataSet ds, ReportDocument rptDoc, string FiscalPeriod)
        {
            EmailSettingsBollore ems = new EmailSettingsBollore();
            SettingRepo _setDAL = new SettingRepo();
            ems.MailHeader = _setDAL.settingValue("Mail", "MailSubject");
            ems.MailHeader = ems.MailHeader.Replace("vmonth", FiscalPeriod);
            string mailbody = _setDAL.settingValue("Mail", "MailBody");
            DataSet DsTemp = new DataSet();
            string strSort = " Code";
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                try
                {
                    DsTemp = new DataSet();
                    dt1 = new DataTable();
                    dt2 = new DataTable();


                    //DataRow[] DataRow1 = ds.Tables[0].Select("Code='" + item["Code"].ToString() + "'");
                    //DataView dtview1 = new DataView(DataRow1.CopyToDataTable());
                    //dtview1.Sort = strSort;
                    //dt1 = dtview1.ToTable();

                    //DataRow[] DataRow2 = ds.Tables[1].Select("Code='" + item["Code"] + "'");
                    //DataView dtview2 = new DataView(DataRow2.CopyToDataTable());
                    //dtview2.Sort = strSort;
                    //dt2 = dtview2.ToTable();


                    dt1 = ds.Tables[0].AsEnumerable()
                                        .Where(x => x.Field<string>("Email") != " ")
                                        .CopyToDataTable();


                    dt2 = ds.Tables[1].AsEnumerable()
                                        .Where(x => x.Field<string>("Code") != " ")
                                        .CopyToDataTable();


                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                                                                      SecurityProtocolType.Tls11 |
                                                                      SecurityProtocolType.Tls;

                    ems.MailToAddress = dt1.Rows[0]["Email"].ToString();

                    if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
                    {
                        dt1.TableName = "TIBSalary";
                        dt2.TableName = "DtTIBOtherHead";

                        ems.MailBody = mailbody.Replace("vmonth", FiscalPeriod)
                            .Replace("vname", item["EmpName"].ToString());
                        //ems.MailBody = mailbody;
                        //dt1.ImportRow(item);
                        ems.FileName = "Payslip of " + item["EmpName"].ToString() + " (" + FiscalPeriod + ")";
                        DsTemp.Tables.Add(dt1);
                        DsTemp.Tables.Add(dt2);
                        rptDoc.SetDataSource(DsTemp);

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

                            smpt.Send(mailmessage);
                            mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                            FileLogger.Log("EmpEmailProcessBollore", this.GetType().Name, "EmpEmail Send To:" + item["EmpName"].ToString() + Environment.NewLine + "EmpEmailAddress:" + dt1.Rows[0]["Email"].ToString());

                        }

                        //Thread.Sleep(500);
                        Thread.Sleep(3000);
                    }
                }
                catch (SmtpFailedRecipientException ex)
                {
                    FileLogger.Log("EmpEmailProcessBollore", this.GetType().Name, "EmpEmail Not Send To:" + item["EmpName"].ToString() + " " + ex.Message + Environment.NewLine + ex.StackTrace);

                    // throw ex;
                }
            }

            rptDoc.Close();
            thread.Abort();
        }

        public void EmpEmailProcess(DataTable dt, ReportDocument rptDoc, string FiscalPeriod)
        {
            EmailSettings ems = new EmailSettings();
            SettingRepo _setDAL = new SettingRepo();
            ems.MailHeader = _setDAL.settingValue("Mail", "MailSubject");
            ems.MailHeader = ems.MailHeader.Replace("vmonth", FiscalPeriod);
            string mailbody = _setDAL.settingValue("Mail", "MailBody");

            foreach (DataRow item in dt.Rows)
            {
                try
                {
                    ems.MailToAddress = item["EmpEmail"].ToString();
                    if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
                    {
                        DataTable dt1 = new DataTable();
                        dt1 = dt.Clone();
                        dt1.TableName = "dtSalarySheet";

                        ems.MailBody = mailbody.Replace("vmonth", FiscalPeriod);
                        ems.MailBody = mailbody.Replace("vname", item["EmpName"].ToString());
                        dt1.ImportRow(item);
                        ems.FileName = item["EmpName"].ToString() + " (" + FiscalPeriod + ")";
                        rptDoc.SetDataSource(dt1);
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

                            smpt.Send(mailmessage);
                            mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                        }

                        Thread.Sleep(500);
                    }
                }
                catch (SmtpFailedRecipientException ex)
                {
                    // throw ex;
                }
            }

            rptDoc.Close();
            thread.Abort();
        }

        public void SalaryEmailRequestBollore(DataTable dt, ReportDocument rptDoc, string FiscalPeriod)
        {
            EmailSettingsBollore ems = new EmailSettingsBollore();
            SettingRepo _setDAL = new SettingRepo();
            ems.MailHeader = "Computer Generate Salary Request for the month of vmonth";
            ems.MailHeader = ems.MailHeader.Replace("vmonth", FiscalPeriod);
            string mailbody = @"Dear Abhishek Sir,
         Please find the attachment file of Salary for the month of vmonth.";
     
                try
                {
                   

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                                                                      SecurityProtocolType.Tls11 |
                                                                      SecurityProtocolType.Tls;

                    ems.MailToAddress = "torekul.islam@symphonysoftt.com";
                    //abhishek.srivastava@bollore.com
                    if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
                    {

                        ems.MailBody = mailbody.Replace("vmonth", FiscalPeriod);
                       
                        //ems.MailBody = mailbody;
                        //dt1.ImportRow(item);
                        ems.FileName = "Salary month of " + " (" + FiscalPeriod + ")";
                     
                        rptDoc.SetDataSource(dt);

                        using (var smpt = new SmtpClient())
                        {
                            smpt.EnableSsl = ems.USsel;
                            smpt.Host = ems.ServerName;
                            smpt.Port = ems.Port;
                            smpt.UseDefaultCredentials = false;
                            smpt.EnableSsl = false;
                            smpt.Credentials = new NetworkCredential(ems.UserName, ems.Password);
                            MailMessage mailmessage = new MailMessage(
                                ems.MailFromAddress,
                                ems.MailToAddress,
                                ems.MailHeader,
                                ems.MailBody);
                          

                            MemoryStream outputStream = new MemoryStream();
                            using (ExcelPackage package = new ExcelPackage(outputStream))
                            {
                                ExcelWorksheet facilityWorksheet = package.Workbook.Worksheets.Add(ems.FileName);
                                facilityWorksheet.Cells.LoadFromDataTable(dt, true);

                                package.Save();
                            }

                            outputStream.Position = 0;
                            mailmessage.Attachments.Add(new Attachment(outputStream, ems.FileName, "application/vnd.ms-excel"));
                            mailmessage.Attachments.Add(new Attachment(
                              rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat),
                              ems.FileName + ".pdf"));

                            smpt.Send(mailmessage);
                            mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;


                        }
                    }
                        //Thread.Sleep(500);
                        Thread.Sleep(3000);
                    }
                catch (SmtpFailedRecipientException ex)
                {
                            FileLogger.Log("SalaryEmailRequestBollore", this.GetType().Name,ex.Message + Environment.NewLine + ex.StackTrace);

                    // throw ex;
                }
                rptDoc.Close();
                thread.Abort();
            }

     
        public class EmailSettings
        {
            public string MailToAddress { get; set; }
            public string MailFromAddress = "payrolltib@ti-bangladesh.info";
            public bool USsel = false;
            public string Password = "stcyjzvwympkjrwb";
            public string UserName = "payrolltib@ti-bangladesh.info";

            public string ServerName = "smtp.office365.com";
            public string MailBody { get; set; }
            public string MailHeader { get; set; }
            public string Fiscalyear { get; set; }
            public int Port = 587;
            public HttpPostedFileBase fileUploader { get; set; }
            public string FileName { get; set; }
        }

        public class EmailSettingsBollore
        {
            public string MailToAddress { get; set; }
            public string MailFromAddress = "BD.HRMS@BOLLORE.COM";
            public bool USsel = true;
            public string Password = "g8A9BpQw6o4G5KzPf72HiNt3";
            public string UserName = @"BTL\_svc_bbddac_0001";

            public string ServerName = @"csi-btl-smtp.bollore-logistics.com";
            public string MailBody { get; set; }
            public string MailHeader { get; set; }
            public string Fiscalyear { get; set; }
            public int Port = 587;
            public HttpPostedFileBase fileUploader { get; set; }
            public string FileName { get; set; }
        }


        /// <summary>
        /// Monthly OT Sheet
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult MonthlyOTSheetReport()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "70002", "report").ToString();
            return View("MonthlyOTSheetReport");
        }

        [HttpGet]
        public ActionResult MonthlyOTSheetReportView(string departmentId, string projectId, string sectionId,
            string fid, string reportType, string orderby
            , string MulitpleProjectId, string MulitpleOther3, string HoldStatus)
        {
            try
            {
                List<string> ProjectIdList = new List<string>();
                List<string> Other3List = new List<string>();

                if (projectId == "0_0" || projectId == "0" || projectId == "" || projectId == "null" ||
                    projectId == null)
                {
                    projectId = "";
                }

                if (departmentId == "0_0" || departmentId == "0" || departmentId == "" || departmentId == "null" ||
                    departmentId == null)
                {
                    departmentId = "";
                }

                if (sectionId == "0_0" || sectionId == "0" || sectionId == "" || sectionId == "null" ||
                    sectionId == null)
                {
                    sectionId = "";
                }


                if (MulitpleProjectId != "0_0" && MulitpleProjectId != "0" && MulitpleProjectId != "" &&
                    MulitpleProjectId != "null" && MulitpleProjectId != null)
                {
                    ProjectIdList = MulitpleProjectId.Split(',').ToList();
                }


                if (MulitpleOther3 != "0_0" && MulitpleOther3 != "0" && MulitpleOther3 != "" &&
                    MulitpleOther3 != "null" && MulitpleOther3 != null)
                {
                    Other3List = MulitpleOther3.Split(',').ToList();
                }

                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                EmployeeMonthlyOvertimeVM vm = new EmployeeMonthlyOvertimeVM();
                EmployeeMonthlyOvertimeRepo _repo = new EmployeeMonthlyOvertimeRepo();
                vm.FiscalYearDetailId = Convert.ToInt32(fid);
                vm.OrderBy = orderby;
                vm.HoldStatus = HoldStatus;

                vm.ProjectIdList = ProjectIdList;
                vm.Other3List = Other3List;

                string[] conditionFields = { "ve.DepartmentId", "ve.ProjectId", "ve.SectionId" };
                string[] conditionValues = { departmentId, projectId, sectionId };
                
                table = _repo.Report(vm, conditionFields, conditionValues);

                if (table.Rows.Count == 0)
                {
                    Session["result"] = "Fail" + "~" + "No Data Found";
                    return View("MonthlyOTSheetReport");
                }

                var FullPeriodName = Convert.ToDateTime("01-" + table.Rows[0]["PeriodName"].ToString())
                    .ToString("MMMM-yyyy");


                ReportHead = "There are no data to Preview for Employee Monthly Overtime";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "Employee Monthly Overtime List";
                }

                if (reportType == "D")
                {
                    ReportHead += " (Detail)";
                }
                else if (reportType == "S")
                {
                    ReportHead += " (Summary)";
                }

                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtEmployeeMonthlyOvertime";

                if (reportType == "D")
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory +
                                  @"Files\ReportFiles\Attendance\\rptEmployeeMOT.rpt";
                }
                else if (reportType == "S")
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory +
                                  @"Files\ReportFiles\Attendance\\rptEmployeeMOTSummary.rpt";
                }


                //if (System.IO.File.Exists(rptLocation))
                //{
                //    var tt = "";
                //    //System.IO.File.Delete(fullPath + FileName);
                //}
                if (!string.IsNullOrWhiteSpace(vm.HoldStatus) && vm.HoldStatus.ToLower() == "hold")
                {
                    vm.HoldStatus = "Held Up ";
                }
                else
                {
                    vm.HoldStatus = "";
                }

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["paramReportType"].Text = "'" + reportType + "'";
                doc.DataDefinition.FormulaFields["MOT"].Text = "'MOT'";
                doc.DataDefinition.FormulaFields["FullPeriodName"].Text = "'" + FullPeriodName + "'";
                doc.DataDefinition.FormulaFields["HoldStatus"].Text = "'" + vm.HoldStatus + "'";

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ActionResult Tax108Sheet(SalarySheetVM vm)
        {
            string[] result = new string[6];
            try
            {
                #region Try
                ExcelVM model = new ExcelVM();
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
                DataSet ReportResult = new DataSet();
                DataSet ds = new DataSet();
                DataTable dt1 = new DataTable();

                #endregion
                string[] conditionFields = new string[] { };
                string[] conditionValues = new string[] { };
                vm.CompanyName = CompanyName;
                if(vm.SheetName == "108Sheet")
                {
                    dt = _empRepo.TAX_108(vm);
                    dt1 = _empRepo.TAX_108_WithOutTIN(vm);
                    ReportResult.Tables.Add(dt);
                    ReportResult.Tables.Add(dt1);

                }
                else
                {
                    dt = _empRepo.TAX_108A(vm);
                    ReportResult.Tables.Add(dt);
                }
                if (vm.ReportType.ToLower() == "excel")
                {
                    if (vm.SheetName == "108Sheet")
                    {
                        DataTable table = new DataTable();
                        DataTable table1 = new DataTable();
                        DataTable table4 = new DataTable();
                        table = dt.Copy();
                        table1 = dt1.Copy();
                        table4 = dt.Copy();
                        table4.TableName = "table4";
                        var dataView = new DataView(table);
                        table = dataView.ToTable(false, "EmpName", "Code","TIN","Section", "Designation", "Project", "JoinDate", "Basic", "HouseRent", "Medical", "TransportAllowance", "Gross", "PFEmployer", "ChildAllowance", "HARDSHIP", "LeaveEncashment"
                            , "TransportBill", "TAXDeduction", "Bonus", "Othere_OT");
                        var dataView1 = new DataView(table4);
                        table4 = dataView1.ToTable(false, "EmpName", "Code","TIN", "Section", "Designation", "Project", "JoinDate", "RecognizedGF", "Principal", "Profit", "TotalExemptedAmount", "RebateAmount", "TAXDeduction");

                        ds.Tables.Add(table);
                        ds.Tables.Add(table1);
                        ds.Tables.Add(table4);

                        model = Ordinary.DownloadExcelMultiple(ds, "108Sheet", new[] { "PART1", "PART2", "PART3" });
                               using (var memoryStream = new MemoryStream())
                      {
                          Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                          Response.AddHeader("content-disposition", "attachment;  filename=" + model.FileName + ".xlsx");
                          model.varExcelPackage.SaveAs(memoryStream);
                          memoryStream.WriteTo(Response.OutputStream);
                          Response.Flush();
                          Response.End();
                      }
                    }
                    else
                    {

                        model = Ordinary.DownloadExcelMultiple(ReportResult, "108ASheet", new[] { "108A" });
                        using (var memoryStream = new MemoryStream())
                        {
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment;  filename=" + model.FileName + ".xlsx");
                            model.varExcelPackage.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }

                }
                else
                {
                    #region Report Call

                    EnumReportRepo _reportRepo = new EnumReportRepo();
                    List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

                    CompanyRepo _CompanyRepo = new CompanyRepo();

                    CompanyVM Companyvm = _CompanyRepo.SelectAll().FirstOrDefault();

                    string[] conFields = { "ReportType", "ReportId" };
                    string[] conValues = { "108Sheet", vm.SheetName };
                    enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);

                    string ReportFileName = enumReportVMs.FirstOrDefault().ReportFileName;
                    string ReportName = enumReportVMs.FirstOrDefault().Name;

                    SettingRepo _sRepo = new SettingRepo();

                    //dt.TableName = "dtSalary";

                    if (vm.SheetName == "108Sheet")
                    {
                        //dt.TableName = "dtTAX108";
                        ReportResult.Tables[0].TableName = "dtTAX108";
                        ReportResult.Tables[1].TableName = "dtTAX108TIN";
                    }
                    else
                    {
                        ReportResult.Tables[0].TableName = "dtTAX108A";
                    }


                    //FiscalYearDetailVM fydVM = new FiscalYearRepo().FYPeriodDetail(Convert.ToInt32(vm.FiscalYearDetailId))
                    //    .FirstOrDefault();

                    //string PeriodName = fydVM.PeriodName;

                    //string FullPeriodName = Convert.ToDateTime("01-" + PeriodName).ToString("MMMM-yyyy");


                    //if (vm.IsMultipleSalary)
                    //{
                    //    fydVM = new FiscalYearRepo().FYPeriodDetail(Convert.ToInt32(vm.FiscalYearDetailIdTo))
                    //        .FirstOrDefault();

                    //    string PeriodNameTo = fydVM.PeriodName;

                    //    string FullPeriodNameTo = Convert.ToDateTime("01-" + PeriodNameTo).ToString("MMMM-yyyy");

                    //    FullPeriodName = FullPeriodName + " to " + FullPeriodNameTo;
                    //}

                    int IncomeYearFrom = vm.FiscalYear - 1;
                    int AssessmentYearTo = vm.FiscalYear + 1;
                    string rptLocation = "";

                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" +
                                  ReportFileName + ".rpt";

                    doc.Load(rptLocation);
                    string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                    doc.DataDefinition.FormulaFields["companyLogo"].Text = "'" + companyLogo + "'";
                    //doc.DataDefinition.FormulaFields["FullPeriodName"].Text = "'" + FullPeriodName + "'";
                    //doc.DataDefinition.FormulaFields["HoldStatus"].Text = "'" + vm.HoldStatus + "'";


                    FormulaFieldDefinitions crFormulaF;
                    crFormulaF = doc.DataDefinition.FormulaFields;
                    CommonWebMethod _CommonWebMethod = new CommonWebMethod();
                    _CommonWebMethod.FormulaField(doc, crFormulaF, "IncomeYearFrom", IncomeYearFrom.ToString());
                    _CommonWebMethod.FormulaField(doc, crFormulaF, "AssessmentYearTo", AssessmentYearTo.ToString());
                    _CommonWebMethod.FormulaField(doc, crFormulaF, "Year", vm.FiscalYear.ToString());
                    _CommonWebMethod.FormulaField(doc, crFormulaF, "CompanyName", Companyvm.Name.ToString());
                    _CommonWebMethod.FormulaField(doc, crFormulaF, "CompanyAddress", Companyvm.Address.ToString());
                    _CommonWebMethod.FormulaField(doc, crFormulaF, "CompanyPhone", Companyvm.Phone.ToString());
                    _CommonWebMethod.FormulaField(doc, crFormulaF, "CompanyEmail", Companyvm.Email.ToString());
                    _CommonWebMethod.FormulaField(doc, crFormulaF, "TIN", Companyvm.TaxId.ToString());
                   
                    #endregion
                    if (vm.SheetName == "108Sheet")
                    {
                        doc.SetDataSource(ReportResult);
                    }
                    else
                    {
                        doc.SetDataSource(ReportResult);
                    }
                    var rpt = RenderReportAsPDF(doc);
                    doc.Close();
                    return rpt;
                }
                return Redirect("Tax108Sheet");
                #endregion Try
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = "Process Fail";
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log("Tax108", this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);

                return View(vm);
            }
        }

        public ActionResult ChildAllowance(EmployeeInfoVM vm)

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
                SettingRepo _setDAL = new SettingRepo();

                DataTable dt = new DataTable();

                #endregion
                #region Multiple Selection Parameters
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

                if (vm.MultipleOther2 != "0_0" && vm.MultipleOther2 != "0" && vm.MultipleOther2 != "" && vm.MultipleOther2 != "null" && vm.MultipleOther2 != null)
                {
                    vm.Other2List = vm.MultipleOther2.Split(',').ToList();
                }

                if (vm.MultipleOther3 != "0_0" && vm.MultipleOther3 != "0" && vm.MultipleOther3 != "" && vm.MultipleOther3 != "null" && vm.MultipleOther3 != null)
                {
                    vm.Other3List = vm.MultipleOther3.Split(',').ToList();
                }

                
                #endregion

                string[] conditionFields = new string[] { };
                string[] conditionValues = new string[] { };

                dt = _empRepo.ChildAllowance(vm);
                string AllowYears = _setDAL.settingValue("Dependent", "Age");

                string rptLocation = "";
                string ReportFileName = "rptChildAllowance";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\Salary\" + ReportFileName + ".rpt";

                doc.Load(rptLocation);
                doc.DataDefinition.FormulaFields["AllowYears"].Text = "'" + AllowYears + "'";
                doc.DataDefinition.FormulaFields["Date"].Text = "'" + vm.IssueDate + "'";


                FormulaFieldDefinitions crFormulaF;
                crFormulaF = doc.DataDefinition.FormulaFields;
                CommonWebMethod _CommonWebMethod = new CommonWebMethod();
            
                #endregion

                doc.SetDataSource(dt);

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;

            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = "Process Fail";
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log("ChildAllowance", this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);

                return View(vm);
            }
        }



        #region Comments

        ////public DataTable dtSalarySheetKazolBackup(DataTable dt)
        ////{

        ////    ////string[] shortColumnName = { "FiscalYearDetailId", "EmployeeId", "Code", "EmpName", "Department", "Basic", "HouseRent", "Medical", "Conveyance", "Gross", "Arrear", "ReimbursableExpense", "PreEmploymentCheckUp", "OtherAllowanceMonthly", "MobileBill", "TAX", "AdvanceDeduction", "PFEmployee", "TotalLoan", "OtherDeductionMonthly", "DOM", "PayDays", "NPDays", "BankAccountNo", "Section", "SalaryDeduction", "TransportBill" };
        ////    ////Type[] columnTypes = { typeof(String), typeof(String), typeof(String), typeof(String), typeof(String), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(int), typeof(decimal), typeof(int), typeof(string), typeof(string), typeof(decimal), typeof(decimal) };
        ////    #region Select Columns

        ////    string[] shortColumnName = { "FiscalYearDetailId", "PeriodName", "EmployeeId", "Code", "EmpName", "Department", "BankAccountNo"
        ////                                            , "Designation", "Project", "Other3"//01-19

        ////                                            , "Basic"                                                                       //20     typeof(decimal)
        ////                                            , "HouseRent"                                                                   //21     typeof(decimal)
        ////                                            , "Medical"                                                                     //22     typeof(decimal)
        ////                                            , "Conveyance"                                                                  //23     typeof(decimal)
        ////                                            , "Gross"                                                                       //24     typeof(decimal)
        ////                                            , "Arrear"                                                                      //25     typeof(decimal)
        ////                                            , "LeaveEncash"                                                                 //26     typeof(decimal)
        ////                                            , "ReimbursableExpense"                                                         //27     typeof(decimal)
        ////                                            , "AbsentDeduction"                                                             //28     typeof(decimal)
        ////                                            , "LeaveWOPay"                                                                  //29     typeof(decimal)
        ////                                            , "AdvanceDeduction"                                                            //30     typeof(decimal)
        ////                                            , "TotalLoan"                                                                   //31     typeof(decimal)
        ////                                            , "Tax"                                                                         //32     typeof(decimal)
        ////                                            , "SalaryDeduction"                                                             //33     typeof(decimal)
        ////                                            , "TransportBill"                                                               //34     typeof(decimal)
        ////                                            , "LateDeduction"                                                               //35     typeof(decimal)
        ////                                            , "Punishment"                                                                  //36     typeof(decimal)
        ////                                            , "BankPayAmount"                                                               //37     typeof(decimal)
        ////                                            , "PayDays"                                                                     //38     typeof(decimal)
        ////                                            , "NPAmount"                                                                    //39     typeof(decimal)
        ////                                         };

        ////    Type[] columnTypes = { typeof(int), typeof(String), typeof(String), typeof(String), typeof(String), typeof(String), typeof(String)
        ////                                     , typeof(String), typeof(String), typeof(String)//01-19

        ////                                     , typeof(decimal)                                                                           //20   
        ////                                     , typeof(decimal)                                                                           //21   
        ////                                     , typeof(decimal)                                                                           //22   
        ////                                     , typeof(decimal)                                                                           //23   
        ////                                     , typeof(decimal)                                                                           //24   
        ////                                     , typeof(decimal)                                                                           //25   
        ////                                     , typeof(decimal)                                                                           //26   
        ////                                     , typeof(decimal)                                                                           //27   
        ////                                     , typeof(decimal)                                                                           //28   
        ////                                     , typeof(decimal)                                                                           //29   
        ////                                     , typeof(decimal)                                                                           //30   
        ////                                     , typeof(decimal)                                                                           //31   
        ////                                     , typeof(decimal)                                                                           //32   
        ////                                     , typeof(decimal)                                                                           //33   
        ////                                     , typeof(decimal)                                                                           //34   
        ////                                     , typeof(decimal)                                                                           //35   
        ////                                     , typeof(decimal)                                                                           //36   
        ////                                     , typeof(decimal)                                                                           //37   
        ////                                     , typeof(decimal)                                                                           //38   
        ////                                     , typeof(decimal)                                                                           //39   


        ////                                 };


        ////    dt = Ordinary.DtSetColumnsOrder(dt, shortColumnName);
        ////    dt = Ordinary.DtSelectedColumn(dt, shortColumnName, columnTypes);

        ////    ////////dt.Columns.Add("OtherDeduction", typeof(decimal));
        ////    ////////dt.Columns.Add("TotalEarning", typeof(decimal));
        ////    ////////dt.Columns.Add("TotalDeduction", typeof(decimal));
        ////    dt.Columns.Add("NetSalaryBank", typeof(decimal));
        ////    dt.Columns.Add("NetSalaryCash", typeof(decimal));


        ////    dt.Columns.Add("BankSheetGross", typeof(decimal));
        ////    dt.Columns.Add("CashPayGross", typeof(decimal));

        ////    dt.Columns.Add("OtherEarning", typeof(decimal));
        ////    dt.Columns.Add("TotalDeduction", typeof(decimal));
        ////    dt.Columns.Add("OtherDeduction", typeof(decimal));
        ////    dt.Columns.Add("AllEarning", typeof(decimal));
        ////    dt.Columns.Add("AllDeduction", typeof(decimal));
        ////    dt.Columns.Add("BankSheetDeduction", typeof(decimal));

        ////    dt.Columns.Add("NetSalary", typeof(decimal));


        ////    #endregion
        ////    #region Declarations
        ////    decimal vGross = 0;
        ////    decimal vArrear = 0;
        ////    decimal vLeaveEncash = 0;
        ////    decimal vReimbursableExpense = 0;
        ////    decimal vAbsentDeduction = 0;
        ////    decimal vLeaveWOPay = 0;
        ////    decimal vAdvanceDeduction = 0;
        ////    decimal vTotalLoan = 0;
        ////    decimal vTax = 0;
        ////    decimal vSalaryDeduction = 0;
        ////    decimal vTransportBill = 0;
        ////    decimal vLateDeduction = 0;
        ////    decimal vPunishment = 0;
        ////    decimal vBankPayAmount = 0;
        ////    decimal vNPAmount = 0;

        ////    decimal vBankSheetGross = 0;
        ////    decimal vCashPayGross = 0;
        ////    decimal vOtherEarning = 0;
        ////    decimal vTotalDeduction = 0;
        ////    decimal vOtherDeduction = 0;
        ////    decimal vAllEarning = 0;
        ////    decimal vAllDeduction = 0;
        ////    decimal vBankSheetDeduction = 0;
        ////    decimal vNetSalaryBank = 0;
        ////    decimal vNetSalaryCash = 0;

        ////    decimal vNetSalary = 0;


        ////    #endregion

        ////    int i = 0;
        ////    foreach (var item in dt.Rows)
        ////    {
        ////        #region Variables
        ////        vGross = 0;
        ////        vArrear = 0;
        ////        vLeaveEncash = 0;
        ////        vReimbursableExpense = 0;
        ////        vAbsentDeduction = 0;
        ////        vLeaveWOPay = 0;
        ////        vAdvanceDeduction = 0;
        ////        vTotalLoan = 0;
        ////        vTax = 0;
        ////        vSalaryDeduction = 0;
        ////        vTransportBill = 0;
        ////        vLateDeduction = 0;
        ////        vPunishment = 0;
        ////        vBankPayAmount = 0;
        ////        vNPAmount = 0;


        ////        vBankSheetGross = 0;
        ////        vCashPayGross = 0;
        ////        vOtherEarning = 0;
        ////        vTotalDeduction = 0;
        ////        vOtherDeduction = 0;
        ////        vAllEarning = 0;
        ////        vAllDeduction = 0;
        ////        vBankSheetDeduction = 0;
        ////        vNetSalaryBank = 0;
        ////        vNetSalaryCash = 0;

        ////        vNetSalary = 0;


        ////        #endregion
        ////        #region Value Assign

        ////        vGross = Convert.ToDecimal(dt.Rows[i]["Gross"]);
        ////        vArrear = Convert.ToDecimal(dt.Rows[i]["Arrear"]);
        ////        vLeaveEncash = Convert.ToDecimal(dt.Rows[i]["LeaveEncash"]);
        ////        vReimbursableExpense = Convert.ToDecimal(dt.Rows[i]["ReimbursableExpense"]);
        ////        vAbsentDeduction = Convert.ToDecimal(dt.Rows[i]["AbsentDeduction"]);
        ////        vLeaveWOPay = Convert.ToDecimal(dt.Rows[i]["LeaveWOPay"]);
        ////        vAdvanceDeduction = Convert.ToDecimal(dt.Rows[i]["AdvanceDeduction"]);
        ////        vTotalLoan = Convert.ToDecimal(dt.Rows[i]["TotalLoan"]);
        ////        vTax = Convert.ToDecimal(dt.Rows[i]["Tax"]);
        ////        vSalaryDeduction = Convert.ToDecimal(dt.Rows[i]["SalaryDeduction"]);
        ////        vTransportBill = Convert.ToDecimal(dt.Rows[i]["TransportBill"]);
        ////        vLateDeduction = Convert.ToDecimal(dt.Rows[i]["LateDeduction"]);
        ////        vPunishment = Convert.ToDecimal(dt.Rows[i]["Punishment"]);
        ////        vBankPayAmount = Convert.ToDecimal(dt.Rows[i]["BankPayAmount"]);
        ////        vNPAmount = Convert.ToDecimal(dt.Rows[i]["NPAmount"]);

        ////        vAbsentDeduction = vAbsentDeduction + vNPAmount;
        ////        dt.Rows[i]["AbsentDeduction"] = vAbsentDeduction;


        ////        #endregion
        ////        #region Calculation

        ////        vBankSheetGross = vGross - vBankPayAmount; //Note vBankSheetGross = vCashPayGross
        ////        vCashPayGross = vBankSheetGross;

        ////        vOtherEarning = vArrear + vLeaveEncash + vReimbursableExpense;
        ////        vOtherDeduction = vSalaryDeduction + vTransportBill + vLateDeduction + vPunishment;

        ////        vTotalDeduction = vAbsentDeduction + vLeaveWOPay + vAdvanceDeduction + vTotalLoan + vTax;


        ////        vAllEarning = vBankSheetGross + vOtherEarning;
        ////        vAllDeduction = vTotalDeduction + vOtherDeduction;

        ////        if ((vAllEarning - vAllDeduction) < 0)
        ////        {
        ////            vBankSheetDeduction = vAllDeduction - vAllEarning;
        ////        }

        ////        vNetSalaryBank = vBankPayAmount - vBankSheetDeduction;
        ////        vNetSalaryBank = vNetSalaryBank > 0 ? vNetSalaryBank : 0;


        ////        vNetSalaryCash = vAllEarning - vAllDeduction;
        ////        vNetSalaryCash = vNetSalaryCash > 0 ? vNetSalaryCash : 0;


        ////        vNetSalary = vGross - vTotalDeduction + vOtherEarning - vOtherDeduction;
        ////        vNetSalary = vNetSalary > 0 ? vNetSalary : 0;


        ////        vNetSalary = Math.Round(vNetSalary);
        ////        vNetSalaryBank = Math.Round(vNetSalaryBank);
        ////        vNetSalaryCash = Math.Round(vNetSalaryCash);


        ////        dt.Rows[i]["NetSalary"] = vNetSalary;


        ////        dt.Rows[i]["NetSalaryBank"] = vNetSalaryBank;
        ////        dt.Rows[i]["NetSalaryCash"] = vNetSalaryCash;

        ////        dt.Rows[i]["BankSheetGross"] = vBankSheetGross;
        ////        dt.Rows[i]["CashPayGross"] = vCashPayGross;

        ////        dt.Rows[i]["OtherEarning"] = vOtherEarning;
        ////        dt.Rows[i]["TotalDeduction"] = vTotalDeduction;
        ////        dt.Rows[i]["OtherDeduction"] = vOtherDeduction;
        ////        dt.Rows[i]["AllEarning"] = vAllEarning;
        ////        dt.Rows[i]["AllDeduction"] = vAllDeduction;
        ////        dt.Rows[i]["BankSheetDeduction"] = vBankSheetDeduction;


        ////        #endregion

        ////        i++;
        ////    }

        ////    return dt;
        ////}

        #endregion
    }
}

