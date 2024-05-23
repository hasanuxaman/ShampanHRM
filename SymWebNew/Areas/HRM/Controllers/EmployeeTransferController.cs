using SymOrdinary;
using SymRepository.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    [Authorize]
    public class EmployeeTransferController : Controller
    {
        //
        // GET: /HRM/LeaveStructure/
        EmployeeTransferRepo _lsRepo = new EmployeeTransferRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _index(JQueryDataTableParamVM param, string code, string name)
        {
            _lsRepo = new EmployeeTransferRepo();
            var getAllData = _lsRepo.SelectAll();
            IEnumerable<EmployeeTransferVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TransferDate.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.IsCurrent.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable3 && c.Remarks.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeTransferVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TransferDate :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.IsCurrent.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Remarks :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { c.TransferDate, c.IsCurrent.ToString(), c.Remarks, Convert.ToString(c.Id) };
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
        public ActionResult EmployeeTransfer(string Id)
        {
            EmployeeTransferVM vm = new EmployeeTransferVM();

            if (Id !=null)
            {
                vm = new EmployeeTransferRepo().SelectById(Id);

            }
            else
            {
                EmployeeTransferVM vmm = new EmployeeTransferVM();
                vm = vmm;
            }
            return PartialView("_edit", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult EmployeeTransfer(EmployeeTransferVM vm)
        {
            string[] retResults = new string[6];
            _lsRepo = new EmployeeTransferRepo();
         //   vm.EmployeeId = 1;
            if (vm.Id != "" && vm.Id != "" && vm.Id!=null)
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.BranchId = Convert.ToInt32(identity.BranchId);
               retResults= _lsRepo.Insert(vm);
            }
            else
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                retResults=   _lsRepo.Update(vm);
            }
            Session["result"] = retResults[0] + "~" + retResults[1];
            return RedirectToAction("Index");
        }
    }
}
