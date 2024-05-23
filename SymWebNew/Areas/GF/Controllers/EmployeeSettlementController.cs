using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymRepository.GF;
using SymViewModel.HRM;
using SymViewModel.GF;
using SymRepository.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using SymViewModel.Common;
using SymWebUI.Areas.PF.Models;

namespace SymWebUI.Areas.GF.Controllers
{
    public class EmployeeSettlementController : Controller
    {
        public EmployeeSettlementController()
        {
            ViewBag.TransType = AreaTypeGFVM.TransType;
        }
        //
        // GET: /GF/EmployeeSettlement/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        EmployeeSettlementRepo _repo = new EmployeeSettlementRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/GF/Home");
            }
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var designationFilter = Convert.ToString(Request["sSearch_3"]);
            var departmentFilter = Convert.ToString(Request["sSearch_4"]);
            var GFValueValueFilter = Convert.ToString(Request["sSearch_5"]);
            var GFValueFrom = 0;
            var GFValueTo = 0;

            if (GFValueValueFilter.Contains('~'))
            {
                GFValueFrom = GFValueValueFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(GFValueValueFilter.Split('~')[0]) == true ? Convert.ToInt32(GFValueValueFilter.Split('~')[0]) : 0;
                GFValueTo = GFValueValueFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(GFValueValueFilter.Split('~')[1]) == true ? Convert.ToInt32(GFValueValueFilter.Split('~')[1]) : 0;
            }

            //Code
            //EmpName 
            //Designation
            //Department 
            //GFValue

            #endregion Column Search

            EmployeeSettlementRepo _repo = new EmployeeSettlementRepo();
            List<EmployeeSettlementVM> getAllData = new List<EmployeeSettlementVM>();
            IEnumerable<EmployeeSettlementVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            //string[] conditionFields = { "pfd.EmployeeId", "pfd.FiscalYearDetailId" };
            //string[] conditionValues = { EmployeeId, fydid };
            getAllData = _repo.SelectAll();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.EmpCode.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.GFValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            if (codeFilter != "" || empNameFilter != "" || designationFilter != "" || departmentFilter != "" || (GFValueValueFilter != "" && GFValueValueFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.EmpCode.ToLower().Contains(codeFilter.ToLower()))
                                    && (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    && (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    && (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    && (GFValueFrom == 0 || GFValueFrom <= Convert.ToInt32(c.GFValue))
                                    && (GFValueTo == 0 || GFValueTo >= Convert.ToInt32(c.GFValue))
                                );
            }



            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeSettlementVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.EmpCode :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? c.GFValue.ToString() :
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
                , c.EmpCode
                , c.EmpName 
                , c.Designation
                , c.Department 
                , c.GFValue.ToString()    
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


        public ActionResult IndexLeftEmployee()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/GF/Home");
            }
            return View();
        }
        public ActionResult _indexLeftEmployee(JQueryDataTableParamModel param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var designationFilter = Convert.ToString(Request["sSearch_3"]);
            var departmentFilter = Convert.ToString(Request["sSearch_4"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_5"]);
            var leftDateFilter = Convert.ToString(Request["sSearch_6"]);
            var totalJobDurationYearFilter = Convert.ToString(Request["sSearch_7"]);

            DateTime fromJoinDate = DateTime.MinValue;
            DateTime toJoinDate = DateTime.MaxValue;


            DateTime fromLeftDate = DateTime.MinValue;
            DateTime toLeftDate = DateTime.MaxValue;


            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromJoinDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toJoinDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
            }


            if (leftDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromLeftDate = leftDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(leftDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(leftDateFilter.Split('~')[0]) : DateTime.MinValue;
                toLeftDate = leftDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(leftDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(leftDateFilter.Split('~')[1]) : DateTime.MinValue;
            }



            #endregion Column Search

            EmployeeSettlementRepo _repo = new EmployeeSettlementRepo();
            List<EmployeeSettlementVM> getAllData = new List<EmployeeSettlementVM>();
            IEnumerable<EmployeeSettlementVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            getAllData = _repo.SelectLeftEmployeeList();

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
                          isSearchable1 && c.EmpCode.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.JoinDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.LeftDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable7 && c.TotalJobDurationYear.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            if (codeFilter != "" || empNameFilter != "" || designationFilter != "" || departmentFilter != "" || (joinDateFilter != "" && joinDateFilter != "~") || (joinDateFilter != "" && leftDateFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.EmpCode.ToLower().Contains(codeFilter.ToLower()))
                                    && (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    && (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    && (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))

                                    && (fromJoinDate == DateTime.MinValue || fromJoinDate <= Convert.ToDateTime(c.JoinDate))
                                    && (toJoinDate == DateTime.MaxValue || toJoinDate >= Convert.ToDateTime(c.JoinDate))

                                    && (fromLeftDate == DateTime.MinValue || fromLeftDate <= Convert.ToDateTime(c.LeftDate))
                                    && (toLeftDate == DateTime.MaxValue || toLeftDate >= Convert.ToDateTime(c.LeftDate))
                                    );
            }



            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_5"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeSettlementVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.EmpCode :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.JoinDate) :
                sortColumnIndex == 6 && isSortable_6 ? Ordinary.DateToString(c.LeftDate) :
                sortColumnIndex == 7 && isSortable_7 ? c.TotalJobDurationYear.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                 Convert.ToString(c.EmployeeId)
                , c.EmpCode
                , c.EmpName 
                , c.Designation
                , c.Department 
                , c.JoinDate.ToString()    
                , c.LeftDate.ToString()    
                , c.TotalJobDurationYear.ToString()    
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
        public ActionResult Create(string EmployeeId = "")
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            EmployeeSettlementVM vm = new EmployeeSettlementVM();
            vm = _repo.SelectLeftEmployeeList(EmployeeId).FirstOrDefault();
            vm.Operation = "add";

            List<GFPolicyVM> gfPolicyVMs = new List<GFPolicyVM>();
            GFPolicyRepo _gfPolicyRepo = new GFPolicyRepo();
            gfPolicyVMs = _gfPolicyRepo.SelectAll();

            vm.gfPolicyVMs = gfPolicyVMs;
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(EmployeeSettlementVM vm)
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
            EmployeeSettlementVM vm = new EmployeeSettlementVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";

            List<GFPolicyVM> gfPolicyVMs = new List<GFPolicyVM>();
            GFPolicyRepo _gfPolicyRepo = new GFPolicyRepo();
            gfPolicyVMs = _gfPolicyRepo.SelectAll();

            vm.gfPolicyVMs = gfPolicyVMs;

            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            EmployeeSettlementVM vm = new EmployeeSettlementVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }


        public JsonResult SelectGFPolicy(string jobYear = "")
        {
            #region Assing EmployeeInfo into TaxScheduleVM

            GFPolicyRepo _GFPolicyRepo = new GFPolicyRepo();
            GFPolicyVM GFPolicyVM = new GFPolicyVM();
            string[] conditionFields = { };
            string[] conditionValues = { };

            GFPolicyVM = _GFPolicyRepo.SelectAllByJobYear(Convert.ToInt32(jobYear)).FirstOrDefault();

            if (GFPolicyVM == null)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }

            string GFPolicy = GFPolicyVM.JobDurationYearFrom
                + "~" + GFPolicyVM.JobDurationYearTo
                + "~" + GFPolicyVM.MultipicationFactor
                + "~" + GFPolicyVM.IsFixed
                + "~" + GFPolicyVM.LastBasicMultipication
                + "~" + GFPolicyVM.Id
                + "~" + GFPolicyVM.PolicyName
                ;
            #endregion Assing EmployeeInfo into TaxScheduleVM
            return Json(GFPolicy, JsonRequestBehavior.AllowGet);
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "report").ToString();
            return View();
        }
        [HttpGet]
        public ActionResult ReportView(string empCodeFrom = "", string empCodeTo = "", string reportType = "", string dtFrom = "", string dtTo = "")
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                EmployeeSettlementVM vm = new EmployeeSettlementVM();
                EmployeeSettlementRepo _repo = new EmployeeSettlementRepo();

                //string[] conditionFields = { "ve.Code", "tr.TransactionDateTime>", "tr.TransactionDateTime<" };
                //string[] conditionValues = { empCodeFrom, Ordinary.DateToString(TDF), Ordinary.DateToString(TDT) };


                if (reportType == "Detail")
                {
                    string[] conditionFields = { "ve.Code", "es.SettlementDate>", "es.SettlementDate<" };
                    string[] conditionValues = { empCodeFrom, Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo) };

                    table = _repo.Report(vm, conditionFields, conditionValues);
                    ReportHead = "There are no data to Preview for Employee Settlement";
                    if (table.Rows.Count > 0)
                    {
                        ReportHead = "Employee Settlement List";
                    }
                    ds.Tables.Add(table);
                    ds.Tables[0].TableName = "dtEmployeeSettlement";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\GF\\rptEmployeeSettlement.rpt";
                }
                else if (reportType == "Summary")
                {
                    string[] conditionFields = { "es.SettlementDate>", "es.SettlementDate<" };
                    string[] conditionValues = { Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo) };

                    table = _repo.Report(vm, conditionFields, conditionValues);
                    ReportHead = "There are no data to Preview for Employee Settlement (" + reportType + ")";
                    if (table.Rows.Count > 0)
                    {
                        ReportHead = "Employee Settlement (" + reportType + ")  List";
                    }
                    ds.Tables.Add(table);
                    ds.Tables[0].TableName = "dtEmployeeSettlement";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\GF\\rptEmployeeSettlementSummary.rpt";
                }

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["frmFromDate"].Text = "'" + dtFrom + "'";
                doc.DataDefinition.FormulaFields["frmToDate"].Text = "'" + dtTo + "'";
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
