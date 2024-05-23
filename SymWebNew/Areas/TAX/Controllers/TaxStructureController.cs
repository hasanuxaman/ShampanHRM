using JQueryDataTables.Models;
using SymRepository.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.TAX.Controllers
{
    public class TaxStructureController : Controller
    {
        #region Declare
        TaxStructureRepo _repo = new TaxStructureRepo();
        TaxSetupRepo _repoSetup = new TaxSetupRepo();
        TaxSlabRepo _repoSlap = new TaxSlabRepo();
        #endregion Declare 
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var CodeNameFilter = Convert.ToString(Request["sSearch_1"]);
            var NameFilter = Convert.ToString(Request["sSearch_2"]);
            var TaxValueFilter = Convert.ToString(Request["sSearch_3"]);
            var IsFixedFilter = Convert.ToString(Request["sSearch_4"]);
            var PortionSalaryTypeFilter = Convert.ToString(Request["sSearch_5"]);
            #endregion Column Search
            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<TaxStructureVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.TaxValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.IsFixed.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.PortionSalaryType.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            #region Column Filtering
            if (TaxValueFilter != "" || IsFixedFilter != "" || PortionSalaryTypeFilter != "" || NameFilter != "" || CodeNameFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (CodeNameFilter == "" || c.Code.ToLower().Contains(CodeNameFilter.ToLower()))
                                            && (NameFilter == "" || c.Name.ToString().ToLower().Contains(NameFilter.ToLower()))
                                            && (TaxValueFilter == "" || c.TaxValue.ToString().ToLower().Contains(TaxValueFilter.ToLower()))
                                            && (IsFixedFilter == "" || c.IsFixed.ToString().ToString().ToLower().Contains(IsFixedFilter.ToLower()))
                                            && (PortionSalaryTypeFilter == "" || c.PortionSalaryType.ToString().ToString().ToLower().Contains(PortionSalaryTypeFilter.ToLower()))
                                            );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<TaxStructureVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Name.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.TaxValue.ToString() :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.IsFixed.ToString() :
                                                           sortColumnIndex == 4 && isSortable_5 ? c.PortionSalaryType :
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
                , c.Name
                , c.TaxValue.ToString()
                , c.IsFixed.ToString()
                , c.PortionSalaryType
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
        public ActionResult Details()
        {
            return View();
        }
       
    }
}
