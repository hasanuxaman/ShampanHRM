using SymOrdinary;
using SymRepository.HRM;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Loan;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        //
        // GET: /HRM/Lon/

        EmployeeStructureRepo _empRepo;
        public ActionResult Index(string id, string empcode, string btn)
        {
            string currentId = "";
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (empcode != null && btn != null)
            {
                vm = _infoRepo.SelectEmpForSearch(empcode, btn);
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    id = vm.Id;
                }
                else
                {
                    currentId = _infoRepo.DropDown(empcode).FirstOrDefault().Id;
                    id = currentId;
                }
            }
            if (Session["mgs"].ToString() != "")
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                id = identity.EmployeeId;
            }
              vm = _infoRepo.SelectById(id);
           
            vm.Id = id;
            return View(vm);
        }
        //#region Loan
        //public ActionResult _index(JQueryDataTableParamVM param, string Id)//EmployeeId
        //{

        //    var getAllData = _empRepo.SelectEmployeeLoanStructureAll(Id);
        //    IEnumerable<EmployeeLoanVM> filteredData;
        //    //Check whether the companies should be filtered by keyword
        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {

        //        //Optionally check whether the columns are searchable at all 
        //        var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
        //        var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
        //        var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
        //        var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

        //        filteredData = getAllData
        //           .Where(c => isSearchable1 && c.LoanType.ToLower().Contains(param.sSearch.ToLower())
        //                       ||
        //                       isSearchable2 && c.PrincipalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                       ||

        //                       isSearchable4 && c.InterestAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                        ||
        //                       isSearchable4 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower()));
        //    }
        //    else
        //    {
        //        filteredData = getAllData;
        //    }

        //    var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
        //    var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
        //    var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
        //    var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
        //    Func<EmployeeLoanVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.LoanType :
        //                                                   sortColumnIndex == 2 && isSortable_2 ? c.PrincipalAmount.ToString() :
        //                                                   sortColumnIndex == 3 && isSortable_3 ? c.InterestAmount.ToString() :
        //                                                   sortColumnIndex == 3 && isSortable_4 ? c.TotalAmount.ToString() :
        //                                                   "");
        //    var sortDirection = Request["sSortDir_0"]; // asc or desc
        //    if (sortDirection == "asc")
        //        filteredData = filteredData.OrderBy(orderingFunction);
        //    else
        //        filteredData = filteredData.OrderByDescending(orderingFunction);

        //    var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var result = from c in displayedCompanies
        //                 select new[] { c.LoanType
        //        , c.PrincipalAmount.ToString()
        //        , Convert.ToString(c.IsFixed == true ? "Y" : "N")
        //        , (c.InterestRate/100).ToString("P") 
        //        , c.InterestAmount.ToString()
        //        , c.TotalAmount.ToString()
        //        , c.NumberOfInstallment.ToString()
        //        , c.TotalAmount.ToString()
        //        , c.ApprovedDate.ToString()
        //        , c.StartDate.ToString()
        //        , c.EndDate.ToString()
        //        , Convert.ToString(c.IsHold == true ? "Y" : "N") };
        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = getAllData.Count(),
        //        iTotalDisplayRecords = filteredData.Count(),
        //        aaData = result
        //    }, 
        //    JsonRequestBehavior.AllowGet);
        //}
        //#endregion

    }
}
