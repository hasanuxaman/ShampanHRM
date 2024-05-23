using CrystalDecisions.CrystalReports.Engine;
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
    public class EmployeeBonusController : Controller
    {
        //
        // GET: /Payroll/EmployeeBonus/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            //var permission= _reposur.SymRollSession(identity.UserId, "Payroll", "Employee", "index").ToString()
            //Session["permission"] = permission;
            //if (permission=="False")
            //{
            //    return Redirect("/Payroll/Home");
            //}
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Bonus Create (new)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// 
        public JsonResult EmployeeBonusSet(string bonusStructureId, string ProjectId, string DepartmentId, string SectionId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeBonusDetailVM vm = new EmployeeBonusDetailVM();
            vm.ProjectId = ProjectId;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            EmployeeBonusRepo repo = new EmployeeBonusRepo();
            string[] result = repo.Insert(bonusStructureId, vm);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BonusStructure(string BonusStructureId)
        {
            BonusStructureRepo _repo = new BonusStructureRepo();
            return PartialView(_repo.SelectById(BonusStructureId));
        }
        public ActionResult EmployeeInfo(string DOJFrom, string DOJTo)
        {
            EmployeeInfoVM vm = new EmployeeInfoVM();
            vm.DOJFrom = DOJFrom;
            vm.DOJTo = DOJTo;
            return PartialView("employee", vm);
        }
        public ActionResult _index(JQueryDataTableParamVM param, string code, string name, string DOJFrom, string DOJTo)
        {
            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            var getAllData = _empRepo.SelectAll(Ordinary.DateToString(DOJFrom), Ordinary.DateToString(DOJTo));
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
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Salutation_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.MiddleName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.LastName.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Salutation_E :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.MiddleName :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.LastName :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { c.Code, c.Code + "~" + Convert.ToString(c.Id), c.Salutation_E, c.MiddleName, c.LastName, c.Department };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult _employeeBonus(JQueryDataTableParamVM param, string code, string name)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var IsFixedFilter = Convert.ToString(Request["sSearch_2"]);
            var RemarksFilter = Convert.ToString(Request["sSearch_3"]);
            #endregion
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeBonusRepo repo = new EmployeeBonusRepo();
            var getAllData = repo.SelectAll(Convert.ToInt32(identity.BranchId));
            IEnumerable<EmployeeBonusVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.IsFixed.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.Remarks.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (codeFilter != "" || IsFixedFilter != "" || RemarksFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                            &&
                                            (IsFixedFilter == "" || c.IsFixed.ToString().ToLower().Contains(IsFixedFilter.ToLower()))
                                        );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeBonusVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Name :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.IsFixed.ToString() :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.Remarks :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { Convert.ToString(c.Id), c.Code, c.Name, c.IsFixed.ToString(), c.Remarks };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult _employeeBonusDetails(JQueryDataTableParamVM param, string code, string name, string employeeBonusId)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var EmployeeNameFilter = Convert.ToString(Request["sSearch_1"]);
            var BasicSalaryFilter = Convert.ToString(Request["sSearch_2"]);
            var GrossSalaryFilter = Convert.ToString(Request["sSearch_3"]);
            var BonusValueFilter = Convert.ToString(Request["sSearch_4"]);
            var BasicamountFrom = 0;
            var BasicamountTo = 0;
            if (BasicSalaryFilter.Contains('~'))
            {
                BasicamountFrom = BasicSalaryFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(BasicSalaryFilter.Split('~')[0]) == true ? 0 : Convert.ToInt32(BasicSalaryFilter.Split('~')[0]);
                BasicamountTo = BasicSalaryFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(BasicSalaryFilter.Split('~')[1]) == true ? 0 : Convert.ToInt32(BasicSalaryFilter.Split('~')[1]);
            }
            var GrossamountFrom = 0;
            var GrossamountTo = 0;
            if (GrossSalaryFilter.Contains('~'))
            {
                GrossamountFrom = GrossSalaryFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(GrossSalaryFilter.Split('~')[0]) == true ? 0 : Convert.ToInt32(GrossSalaryFilter.Split('~')[0]);
                GrossamountTo = GrossSalaryFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(GrossSalaryFilter.Split('~')[1]) == true ? 0 : Convert.ToInt32(GrossSalaryFilter.Split('~')[1]);
            }
            var BonusamountFrom = 0;
            var BonusamountTo = 0;
            if (BonusValueFilter.Contains('~'))
            {
                BonusamountFrom = BonusValueFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(BonusValueFilter.Split('~')[0]) == true ? 0 : Convert.ToInt32(BonusValueFilter.Split('~')[0]);
                BonusamountTo = BonusValueFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(BonusValueFilter.Split('~')[0]) == true ? 0 : Convert.ToInt32(BonusValueFilter.Split('~')[0]);
            }
            #endregion
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeBonusRepo repo = new EmployeeBonusRepo();
            var getAllData = repo.SelectAllEmpBonusDetails(employeeBonusId);
            IEnumerable<EmployeeBonusDetailVM> filteredData;
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
                   .Where(c => isSearchable1 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable4 && c.BonusValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable5 && c.Remarks.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (EmployeeNameFilter != "" || (BasicSalaryFilter != "" && BasicSalaryFilter != "~") || (GrossSalaryFilter != "" && GrossSalaryFilter != "~") || (BonusValueFilter != "" && BonusValueFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c => (EmployeeNameFilter == "" || c.EmpName.ToLower().Contains(EmployeeNameFilter.ToLower()))
                                            &&
                                             (BasicamountFrom == 0 || BasicamountFrom <= Convert.ToInt32(c.BasicSalary))
                                            &&
                                            (BasicamountTo == 0 || BasicamountTo >= Convert.ToInt32(c.BasicSalary))
                                             &&
                                             (GrossamountFrom == 0 || GrossamountFrom <= Convert.ToInt32(c.GrossSalary))
                                            &&
                                            (GrossamountTo == 0 || GrossamountTo >= Convert.ToInt32(c.GrossSalary))
                                             &&
                                             (BonusamountFrom == 0 || BonusamountFrom <= Convert.ToInt32(c.BonusValue))
                                            &&
                                            (BonusamountTo == 0 || BonusamountTo >= Convert.ToInt32(c.BonusValue))
                                        );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeBonusDetailVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.EmpName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.BasicSalary.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.GrossSalary.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.BonusValue.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Remarks :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { Convert.ToString(c.Id), c.EmpName, c.BasicSalary.ToString(), c.GrossSalary.ToString(), c.BonusValue.ToString() };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(string employeeBonusId)
        {
            EmployeeBonusRepo repo = new EmployeeBonusRepo();
            var getAllData = repo.SelectById(employeeBonusId);
            return View(getAllData);
        }
        [HttpGet]
        public ActionResult BonusEdit(int bonusDetailId)
        {
            EmployeeBonusRepo repo = new EmployeeBonusRepo();
            var getAllData = repo.SelectByIdBonusDetail(bonusDetailId);
            return View(getAllData);
        }
        [HttpPost]
        public ActionResult BonusEdit(EmployeeBonusDetailVM vm)
        {
            EmployeeBonusRepo repo = new EmployeeBonusRepo();
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = Identity.Name;
            vm.LastUpdateFrom = Identity.WorkStationIP;
            string[] getAllData = repo.SingleBonusUpdate(vm);
            ViewBag.mgs = getAllData[0];
            return View(vm);
            //return JavaScript(string.Format("ShowResult('{0}','{1}','{2}','{3}')", getAllData[0], getAllData[1], dataAction, "/Payroll/EmployeeBonus"));
        }
        [HttpGet]
        public ActionResult BonusNew(string employeeBonusId)
        {
            EmployeeBonusRepo repo = new EmployeeBonusRepo();
            EmployeeInfoRepo repoEmp = new EmployeeInfoRepo();
            var getAllData = repo.SelectById(employeeBonusId);
            ViewBag.empData = repoEmp.DropDown();
            return View(getAllData);
            //EmployeeBonusDetailVM vm = new EmployeeBonusDetailVM();
            // vm.EmployeeBonusId = "1_1";
            // vm.EmployeeId = "1_10";
            //vm.EmployeeName = "1_10";
            // return View(vm);
        }
        [HttpPost]
        public JsonResult BonusNew(string empBonusID, string empID)
        {
            EmployeeBonusDetailVM vm = new EmployeeBonusDetailVM();
            EmployeeBonusRepo repo = new EmployeeBonusRepo();
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = Identity.Name;
            vm.CreatedFrom = Identity.WorkStationIP;
            vm.EmployeeBonusId = empBonusID;
            vm.EmployeeId = empID;
            string[] getAllData = repo.SingleBonusAdd(vm);
            ViewBag.mgs = getAllData[0];
            return Json(getAllData, JsonRequestBehavior.AllowGet);
            //return JavaScript(string.Format("ShowResult('{0}','{1}','{2}','{3}')", getAllData[0], getAllData[1], dataAction, "/Payroll/EmployeeBonus"));
        }
        //
        public JsonResult BonusDetailsDelete(string ids)
        {
            EmployeeBonusRepo repo = new EmployeeBonusRepo();
            EmployeeBonusDetailVM vm = new EmployeeBonusDetailVM();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = repo.EmployeeBonusDetailsDelete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        public JsonResult BonusDelete(string ids)
        {
            EmployeeBonusRepo repo = new EmployeeBonusRepo();
            EmployeeBonusVM vm = new EmployeeBonusVM();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = repo.EmployeeBonusDelete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeNameByCode(string empCode)
        {
            string[] emp = new EmployeeInfoRepo().EmployeeNameByCode(empCode);
            return Json(emp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EmployeeBonusListReport(string view, string employeeBonusId = null, string ProjectId = null, string DepartmentId = null, string SectionId = null, string DesignationId = null, string CodeF = null, string CodeT = null, string EmpName = null, string dojFrom = null, string dojTo = null)
        {
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "index").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                //SymUserRollRepo _reposur = new SymUserRollRepo();
                string EmployeeBonus = "N";
                if (!(identity.IsAdmin || identity.IsPayroll))
                {
                    //mgfOrganizationId = "";
                    //localSupplierOrganizationId = "";
                    //supplierOrganizationId = "";
                }
                ViewBag.EmployeeBonus = EmployeeBonus;
                if (view == "Y")
                {
                    return View();
                }
                ReportDocument doc = new ReportDocument();
                List<EmployeeBonusDetailVM> getAllData = new List<EmployeeBonusDetailVM>();
                getAllData = new EmployeeBonusRepo().SelectAllEmpBonusDetails(employeeBonusId);
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
                ds.Tables[0].TableName = "dtEmpBonus";
                string rptLocation = "";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollEntry\rptEmployeeBonus.rpt";
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                //doc = new rptEmployeeBonus();
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
