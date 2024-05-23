using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    public class TransferController : Controller
    {
        #region Declare
        EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
        EmployeeInfoVM vm = new EmployeeInfoVM();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        #endregion Declare
        public ActionResult Index()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_19", "index").ToString();
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_5"]);
            var isActiveFilter = Convert.ToString(Request["sSearch_6"]);
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
            }


            #endregion Column Search


            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            List<EmployeeInfoVM> getAllData = new List<EmployeeInfoVM>();
            IEnumerable<EmployeeInfoVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (identity.IsAdmin || identity.IsHRM)
            {
                getAllData = _empRepo.SelectAllActiveEmp();
            }
            else
            {
                getAllData.Add(_empRepo.SelectById(Identit.EmployeeId));

            }
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
            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || (joinDateFilter != "" && joinDateFilter != "~") || isActiveFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    && (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    && (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    && (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.JoinDate) :
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
                , c.Code
                , c.EmpName 
                , c.Department 
                , c.Designation
                , c.JoinDate
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
        #region Transfer
        [HttpGet]
        public ActionResult Transfer()
        {
            return View();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Transfer(EmployeeInfoVM vm, HttpPostedFileBase TransferF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_19", "process").ToString();
            string[] retResults = new string[6];
            EmployeeTransferRepo empTrnApp = new EmployeeTransferRepo();
            EmployeeTransferVM trnvm = new EmployeeTransferVM();
            trnvm = vm.transferVM;
            if (TransferF != null && TransferF.ContentLength > 0)
            {
                trnvm.FileName = TransferF.FileName;
            }
            trnvm.BranchId = Convert.ToInt32(identity.BranchId);
            trnvm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            trnvm.CreatedBy = identity.Name;
            trnvm.CreatedFrom = identity.WorkStationIP;
            retResults = empTrnApp.Insert(trnvm);
            Session["result"] = retResults[0] + "~" + retResults[1];
            if (TransferF != null && TransferF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Transfer"), retResults[2] + TransferF.FileName);
                TransferF.SaveAs(path);
            }
            return Json(retResults[0] + "~" + retResults[1], JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Transfer");
        }
        [HttpGet]
        public ActionResult TransferOLD(string EmployeeId)
        {
            EmployeeTransferVM tranvm = new EmployeeTransferVM();
            if (!string.IsNullOrWhiteSpace(EmployeeId))
            {
                vm = _infoRepo.SelectById(EmployeeId);
                vm.transferVM = new EmployeeTransferRepo().SelectByEmployeeCurrent(EmployeeId);
            }
            else
            {
                vm.transferVM = tranvm;
            }
            return PartialView("_transferOLD", vm);
        }
        [HttpGet]
        public ActionResult TransferNEW(string EmployeeId)
        {
            EmployeeTransferVM tranvm = new EmployeeTransferVM();
            if (!string.IsNullOrWhiteSpace(EmployeeId))
            {
                _infoRepo = new EmployeeInfoRepo();
                vm = _infoRepo.SelectById(EmployeeId);
                vm.transferVM = new EmployeeTransferRepo().SelectByEmployeeCurrent(EmployeeId);
            }
            else
            {
                vm.transferVM = tranvm;
            }
            return PartialView("_transferNEW", vm);
        }
        public ActionResult Transferdetail(string id, string empcode = "", string btn = "current")
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrEmpty(id))
            {
                vm = repo.SelectById(id);
                vm.transferVM = new EmployeeTransferRepo().SelectByEmployeeCurrent(vm.Id);
            }
            if (!string.IsNullOrWhiteSpace(empcode) && !string.IsNullOrWhiteSpace(btn))
            {
                vm = repo.SelectEmpStructure(empcode, btn);
                if (!string.IsNullOrEmpty(id))
                {
                    vm.transferVM = new EmployeeTransferRepo().SelectByEmployeeCurrent(vm.Id);
                }
            }
            return PartialView("_transferdetail", vm);
        }
        public ActionResult EmployeeInfo(string Id)
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeVM vm = new EmployeeVM();
            if (!string.IsNullOrWhiteSpace(Id))
            {
                vm = repo.EmployeeInfo(Id);
                if (vm.IsPermanent)
                {
                    return PartialView("_employee", vm);
                }
                else
                {
                    Session["result"] = "Fail~This Employee not Permanent";
                    return RedirectToAction("Leave");
                }
            }
            else
            {
                return PartialView("_employee", vm);
            }
        }
        #endregion Transfer
    }
}