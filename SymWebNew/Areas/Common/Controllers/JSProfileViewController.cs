using SymViewModel.Common;
using SymRepository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JQueryDataTables.Models;
using SymOrdinary;

namespace SymWebUI.Areas.Common.Controllers
{
    public class JSProfileViewController : Controller
    {
        //
        // GET: /Common/JSProfileView/

        public ActionResult Index()
        {
            return View();
        }
         [Authorize]
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var IdFilter                = Convert.ToString(Request["sSearch_0"]);
            var ViewTypeFilter         = Convert.ToString(Request["sSearch_1"]);
            var ViewDateFilter          = Convert.ToString(Request["sSearch_2"]);
            var FullNameFilter      = Convert.ToString(Request["sSearch_3"]);
            var PresentDistrictIdFilter     = Convert.ToString(Request["sSearch_4"]);
            var JobCategoryIdFilter            = Convert.ToString(Request["sSearch_5"]);
            var WebsiteFilter            = Convert.ToString(Request["sSearch_6"]);

        //00  //Id
        //01  //ViewType
        //02  //ViewDate
        //03  //FullName
        //04  //PresentDistrictId
        //05  //JobCategoryId
        //06  //Website

              DateTime ViewFromDate = DateTime.MinValue;
            DateTime ViewToDate = DateTime.MaxValue;
            if (ViewDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                ViewFromDate = ViewDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(ViewDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(ViewDateFilter.Split('~')[0]) : DateTime.MinValue;
                ViewToDate = ViewDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(ViewDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(ViewDateFilter.Split('~')[1]) : DateTime.MinValue;
            }

            #endregion Column Search

            JSProfileViewRepo _repo = new JSProfileViewRepo();
            var getAllData = _repo.SelectAll();
            IEnumerable<JSProfileViewVM> filteredData;
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
                           isSearchable1 &&  c.ViewType      .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable2 &&  c.ViewDate          .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable3 &&  c.FullName        .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable4 &&  c.PresentDistrictId          .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable5 &&  c.JobCategoryId              .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable6 &&  c.Website          .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            if (
                  ViewTypeFilter        != "" 
               || ViewDateFilter      != "" 
               || FullNameFilter       != "" 
               || PresentDistrictIdFilter  != "" 
               || JobCategoryIdFilter      != "" 
               || WebsiteFilter           != ""  
               || (ViewDateFilter != "" && ViewDateFilter != "~")
                )
            {
                filteredData = filteredData
                                .Where(c =>
                                    (ViewTypeFilter == "" ||        c.ViewType               .ToLower().Contains(ViewTypeFilter.ToLower()))
                                && (ViewFromDate == DateTime.MinValue || ViewFromDate <= Convert.ToDateTime(c.ViewDate))
                                && (ViewToDate == DateTime.MaxValue || ViewToDate >= Convert.ToDateTime(c.ViewDate))
                                && (FullNameFilter == "" ||         c.FullName             .ToString().ToLower().Contains(FullNameFilter.ToLower()))
                                && (PresentDistrictIdFilter == "" ||c.PresentDistrictId      .ToString().ToLower().Contains(PresentDistrictIdFilter.ToLower()))
                                && (JobCategoryIdFilter == "" ||    c.JobCategoryId             .ToString().ToLower().Contains(JobCategoryIdFilter.ToLower()))
                                && (WebsiteFilter == "" ||          c.Website                   .ToLower().Contains(JobCategoryIdFilter.ToLower()))
                                );
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<JSProfileViewVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ?   c.ViewType                        :
                sortColumnIndex == 2 && isSortable_2 ?   c.ViewDate                        :
                sortColumnIndex == 3 && isSortable_3 ?   c.FullName                        :
                sortColumnIndex == 4 && isSortable_4 ?   c.PresentDistrictId.ToString()         :
                sortColumnIndex == 5 && isSortable_5 ?   c.JobCategoryId.ToString()             :
                sortColumnIndex == 6 && isSortable_6 ?   c.Website                              :
            "");                                     
                                                     
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] 
            { 
                  c.Id.ToString()
                , c.ViewType               
                , c.ViewDate               
                , c.FullName               
                , c.PresentDistrictId.ToString()
                , c.JobCategoryId.ToString()    
                , c.Website                     
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
