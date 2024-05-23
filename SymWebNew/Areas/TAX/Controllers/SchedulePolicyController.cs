using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.TAX.Controllers
{
    public class SchedulePolicyController : Controller
    {
        #region Declare
        SchedulePolicyRepo _repo = new SchedulePolicyRepo();
        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        #endregion Declare
        public ActionResult Index()
        {
            List<SchedulePolicyVM> VMs = _repo.SelectAll();
            return View(VMs);
        }

        [HttpPost]
        public ActionResult Edit(List<SchedulePolicyVM> VMs)
        {
            string[] result = new string[6];
            try
            {
                Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "1_15", "edit").ToString();

                result = _repo.Update(VMs);
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }

        }


        #region Unused
        //public ActionResult _index(JQueryDataTableParamModel param)
        //{
        //    #region Column Search
        //    var idFilter = Convert.ToString(Request["sSearch_0"]);
        //    var lineNumberFilter = Convert.ToString(Request["sSearch_1"]);
        //    var scheduleNoFilter = Convert.ToString(Request["sSearch_2"]);
        //    var salaryHeadFilter = Convert.ToString(Request["sSearch_3"]);
        //    var fromBasicFilter = Convert.ToString(Request["sSearch_4"]);
        //    var isFixedFilter = Convert.ToString(Request["sSearch_5"]);
        //    var basicPortionFilter = Convert.ToString(Request["sSearch_6"]);
        //    var equalMaxMinAmountFilter = Convert.ToString(Request["sSearch_7"]);
        //    var remarksFilter = Convert.ToString(Request["sSearch_8"]);

        //    #endregion Column Search
        //    #region Search and Filter Data
        //    var getAllData = _repo.SelectAll();
        //    IEnumerable<SchedulePolicyVM> filteredData;
        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
        //        var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
        //        var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
        //        var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
        //        var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
        //        var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
        //        var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
        //        var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
        //        filteredData = getAllData
        //           .Where(c => isSearchable1 && c.LineNumber.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                     ||  isSearchable2  && c.ScheduleNo.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                     ||  isSearchable3  && c.SalaryHead.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                     ||  isSearchable4  && c.FromBasic.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                     ||  isSearchable5  && c.IsFixed.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                     ||  isSearchable6  && c.BasicPortion.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                     ||  isSearchable7  && c.EqualMaxMinAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
        //                     ||  isSearchable8  && c.Remarks.ToString().ToLower().Contains(param.sSearch.ToLower())

        //                       );
        //    }
        //    else
        //    {
        //        filteredData = getAllData;
        //    }
        //    #endregion Search and Filter Data
        //    #region Column Filtering
        //    if (lineNumberFilter != "" || scheduleNoFilter != "" || salaryHeadFilter != "" || fromBasicFilter != "" || isFixedFilter != "" || basicPortionFilter != "" || equalMaxMinAmountFilter != "" || remarksFilter != "" )
        //    {
        //        filteredData = filteredData
        //                        .Where(c => (lineNumberFilter == "" || c.LineNumber.ToString().ToLower().Contains(lineNumberFilter.ToLower()))
        //                             && (scheduleNoFilter == "" || c.ScheduleNo.ToString().ToLower().Contains(scheduleNoFilter.ToLower()))
        //                             && (salaryHeadFilter == "" || c.SalaryHead.ToString().ToLower().Contains(salaryHeadFilter.ToLower()))
        //                             && (fromBasicFilter == "" || c.FromBasic.ToString().ToLower().Contains(fromBasicFilter.ToString().ToLower()))
        //                             && (isFixedFilter == "" || c.IsFixed.ToString().ToLower().Contains(isFixedFilter.ToString().ToLower()))
        //                             && (basicPortionFilter == "" || c.BasicPortion.ToString().ToLower().Contains(basicPortionFilter.ToLower()))
        //                             && (equalMaxMinAmountFilter == "" || c.EqualMaxMinAmount.ToString().ToLower().Contains(equalMaxMinAmountFilter.ToLower()))
        //                             && (remarksFilter == "" || c.Remarks.ToString().ToLower().Contains(remarksFilter.ToLower()))

        //                                    );
        //    }
        //    #endregion Column Filtering
        //    var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
        //    var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
        //    var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
        //    var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
        //    var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
        //    var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
        //    var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
        //    var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);

        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
        //    Func<SchedulePolicyVM, string> orderingFunction = (c =>
        //        sortColumnIndex == 1 && isSortable_1 ? c.LineNumber.ToString() :
        //        sortColumnIndex == 2 && isSortable_2 ? c.ScheduleNo.ToString() :
        //        sortColumnIndex == 3 && isSortable_3 ? c.SalaryHead.ToString() :
        //        sortColumnIndex == 4 && isSortable_4 ? c.FromBasic.ToString() :
        //        sortColumnIndex == 5 && isSortable_5 ? c.IsFixed.ToString() :
        //        sortColumnIndex == 6 && isSortable_6 ? c.BasicPortion.ToString() :
        //        sortColumnIndex == 7 && isSortable_7 ? c.EqualMaxMinAmount.ToString() :
        //        sortColumnIndex == 8 && isSortable_8 ? c.Remarks.ToString() :

        //                                                   "");
        //    var sortDirection = Request["sSortDir_0"]; // asc or desc
        //    if (sortDirection == "asc")
        //        filteredData = filteredData.OrderBy(orderingFunction);
        //    else
        //        filteredData = filteredData.OrderByDescending(orderingFunction);
        //    var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var result = from c in displayedCompanies
        //                 select new[] { 
        //        Convert.ToString(c.Id)                
        //        , c.LineNumber.ToString()
        //        , c.ScheduleNo.ToString()
        //        , c.SalaryHead.ToString()
        //        , c.FromBasic==true  ? "Yes" : "No"
        //        , c.IsFixed==true  ? "Yes" : "No"
        //        , c.BasicPortion.ToString()
        //        , c.EqualMaxMinAmount.ToString()
        //        , c.Remarks.ToString()

        //    };
        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = getAllData.Count(),
        //        iTotalDisplayRecords = filteredData.Count(),
        //        aaData = result
        //    },
        //                JsonRequestBehavior.AllowGet);
        //}
        #endregion Unused


    }
}
