using SymOrdinary;
using SymRepository.HRM;
using SymRepository.Leave;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Leave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    [Authorize]
    public class LeaveStructureController : Controller
    {
        //
        // GET: /HRM/LeaveStructure/
        LeaveStructureRepo _lsRepo = new LeaveStructureRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _index(JQueryDataTableParamVM param, string code, string name)
        {
            _lsRepo = new LeaveStructureRepo();
            var getAllData = _lsRepo.SelectAll();
            IEnumerable<LeaveStructureVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
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
            Func<LeaveStructureVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Name :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Remarks :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { c.Code, c.Name, c.Remarks, Convert.ToString(c.Id) };
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
        public ActionResult LeaveStructure(int Id)
        {
            LeaveStructureVM vm = new LeaveStructureVM();

            if (Id != 0)
            {
                vm = new LeaveStructureRepo().SelectById(Id);

            }
            else
            {
                LeaveStructureVM vmm = new LeaveStructureVM();
                vm = vmm;
            }
            return PartialView("_edit", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult LeaveStructure(LeaveStructureVM vm)
        {
            string[] retResults = new string[6];
            _lsRepo = new LeaveStructureRepo();
            vm.BranchId = Ordinary.BranchId;
            if (vm.Id <= 0)
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                retResults = _lsRepo.Insert(vm);
            }
            else
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                retResults = _lsRepo.Update(vm);
            }
            Session["result"] = retResults[0] + "~" + retResults[1];
            return RedirectToAction("Index");
        }
    }
}
