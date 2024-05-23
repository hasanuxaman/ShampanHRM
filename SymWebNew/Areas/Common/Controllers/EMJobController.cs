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
    public class EMJobController : Controller
    {
        //
        // GET: /Common/EMJob/

        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var IdFilter                    = Convert.ToString(Request["sSearch_0"]);
            var DesignationFilter           = Convert.ToString(Request["sSearch_1"]);
            var DepartmentFilter            = Convert.ToString(Request["sSearch_2"]);
            var VacancyFilter               = Convert.ToString(Request["sSearch_3"]);
            var RequiruitmentDateFilter     = Convert.ToString(Request["sSearch_4"]);
            var JobDescriptionFilter        = Convert.ToString(Request["sSearch_5"]);
            var FullNameFilter              = Convert.ToString(Request["sSearch_6"]);
            var PresentDistrictIdFilter     = Convert.ToString(Request["sSearch_7"]);
            var JobCategoryIdFilter         = Convert.ToString(Request["sSearch_8"]);
            var WebsiteFilter               = Convert.ToString(Request["sSearch_9"]);
            

        //00  //Id
        //01  //Designation
        //02  //Department
        //03  //Vacancy
        //04  //RequiruitmentDate
        //05  //JobDescription
        //06  //FullName
        //07  //PresentDistrictId
        //08  //JobCategoryId
        //09  //Website


            DateTime RecFromDate = DateTime.MinValue;
            DateTime RecToDate = DateTime.MaxValue;
            if (RequiruitmentDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                RecFromDate = RequiruitmentDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(RequiruitmentDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(RequiruitmentDateFilter.Split('~')[0]) : DateTime.MinValue;
                RecToDate = RequiruitmentDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(RequiruitmentDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(RequiruitmentDateFilter.Split('~')[1]) : DateTime.MinValue;
            }

            #endregion Column Search

            EMJobRepo _repo = new EMJobRepo();
            var getAllData = _repo.SelectAll();
            IEnumerable<EMJobVM> filteredData;
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
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_9"]);

                filteredData = getAllData
                   .Where(c =>
                           isSearchable1 &&  c.Designation    .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable2 &&  c.Department   .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable3 &&  c.Vacancy            .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable5 &&  c.RequiruitmentDate .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable4 &&  c.JobDescription          .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable6 &&  c.FullName        .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable7 &&  c.PresentDistrictId.ToString() .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable8 &&  c.JobCategoryId.ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable9 &&  c.Website      .ToLower().Contains(param.sSearch.ToLower()) 
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            if (
                  DesignationFilter         != ""  
               || DepartmentFilter          != ""  
               || VacancyFilter             != ""  
               || RequiruitmentDateFilter         != ""  
               || JobDescriptionFilter      != ""  
               || FullNameFilter                     != ""  
               || PresentDistrictIdFilter         != ""  
               || JobCategoryIdFilter     != ""  
               || WebsiteFilter           != ""  
               || (RequiruitmentDateFilter != "" && RequiruitmentDateFilter != "~")
                )
            {
                filteredData = filteredData
                                .Where(c =>
                                   (DesignationFilter == "" ||     c.Designation     .ToLower().Contains(DesignationFilter.ToLower()))
                                && (DepartmentFilter == "" ||       c.Department              .ToString().ToLower().Contains(DepartmentFilter.ToLower()))
                                && (VacancyFilter == "" ||          c.Vacancy               .ToString().ToLower().Contains(VacancyFilter.ToLower()))
                                && (JobDescriptionFilter == "" ||   c.JobDescription          .ToString().ToLower().Contains(JobDescriptionFilter.ToLower()))
                                && (FullNameFilter == "" ||         c.FullName       .ToString().ToLower().Contains(FullNameFilter.ToLower()))
                                && (PresentDistrictIdFilter == "" ||c.PresentDistrictId.ToString() .ToLower().Contains(PresentDistrictIdFilter.ToLower()))
                                && (JobCategoryIdFilter  == "" ||   c.JobCategoryId.ToString()  .ToLower().Contains(JobCategoryIdFilter .ToLower()))
                                && (WebsiteFilter       == "" ||c.Website    .ToLower().Contains(WebsiteFilter      .ToLower()))
                                && (RecFromDate == DateTime.MinValue || RecFromDate <= Convert.ToDateTime(c.RequiruitmentDate))
                                && (RecToDate == DateTime.MaxValue ||   RecToDate >= Convert.ToDateTime(c.RequiruitmentDate))
                                
                                );
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_9"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EMJobVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ?   c.Designation                   :
                sortColumnIndex == 2 && isSortable_2 ?   c.Department                    :
                sortColumnIndex == 3 && isSortable_3 ?   c.Vacancy.ToString()                       :
                sortColumnIndex == 4 && isSortable_4 ?   c.RequiruitmentDate             :
                sortColumnIndex == 5 && isSortable_5 ?   c.JobDescription                :
                sortColumnIndex == 6 && isSortable_6 ?   c.FullName                      :
                sortColumnIndex == 7 && isSortable_7 ?   c.PresentDistrictId.ToString()  :
                sortColumnIndex == 8 && isSortable_8 ?   c.JobCategoryId.ToString()      :
                sortColumnIndex == 9 && isSortable_9 ?   c.Website                       :
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
                , c.Designation                 
                , c.Department                  
                , c.Vacancy.ToString()                     
                , c.RequiruitmentDate           
                , c.JobDescription              
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
