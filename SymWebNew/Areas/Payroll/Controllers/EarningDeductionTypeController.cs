using SymOrdinary;
using SymRepository.Common;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class EarningDeductionTypeController : Controller
    {
        //
        // GET: /Payroll/EarningDeductionType/
        EarningDeductionTypeRepo _repo = new EarningDeductionTypeRepo();
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
        public ActionResult _index(JQueryDataTableParamVM param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var NameFilter = Convert.ToString(Request["sSearch_1"]);
            var IsEarningFilter = Convert.ToString(Request["sSearch_2"]);
            var IsActiveFilter = Convert.ToString(Request["sSearch_3"]);
            var RemarksFilter = Convert.ToString(Request["sSearch_3"]);
            //Name
            //IsEarning
            //IsActive
            //Remarks
            var IsEarningFilter1 = IsEarningFilter.ToLower() == "earning" ? true.ToString() : false.ToString();
            var IsActiveFilter1 = IsActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();
            #endregion Column Search
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var getAllData = _repo.SelectAll();
            IEnumerable<EarningDeductionTypeVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.IsEarning.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable3 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (NameFilter != "" || IsEarningFilter != "" || IsActiveFilter != ""|| RemarksFilter != "")
            {
                filteredData = filteredData
                                .Where(c =>(NameFilter == "" || c.Name.ToLower().Contains(NameFilter.ToLower()))
                                           &&
                                           (IsEarningFilter == "" || c.IsEarning.ToString().ToLower().Contains(IsEarningFilter1.ToLower()))
                                           &&
                                           (IsActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(IsActiveFilter1.ToLower()))
                                            && 
                                            (RemarksFilter == "" || c.Remarks.ToLower().Contains(RemarksFilter.ToLower()))
                                           );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EarningDeductionTypeVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.IsEarning.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.IsActive.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Remarks :
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
                             , c.Name
                             , Convert.ToString(c.IsEarning == true ? "Earning" : "Deduction") 
                             , Convert.ToString(c.IsActive == true ? "Active" : "Inactive") 
                             , c.Remarks
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
    }
}
