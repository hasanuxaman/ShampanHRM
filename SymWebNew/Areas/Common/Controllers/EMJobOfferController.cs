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
    public class EMJobOfferController : Controller
    {
        //
        // GET: /Common/EMJobOffer/

                public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var IdFilter                    = Convert.ToString(Request["sSearch_0"]);
            var OfferTypeFilter             = Convert.ToString(Request["sSearch_1"]);
            var FullNameFilter          = Convert.ToString(Request["sSearch_2"]);
            var PresentDistrictIdFilter = Convert.ToString(Request["sSearch_3"]);
            var GenderFilter            = Convert.ToString(Request["sSearch_4"]);
            var DOBFilter               = Convert.ToString(Request["sSearch_5"]);
            var JobCategoryIdFilter     = Convert.ToString(Request["sSearch_6"]);
            var MobileFilter            = Convert.ToString(Request["sSearch_7"]);
            var EmailFilter             = Convert.ToString(Request["sSearch_8"]);
            var DesignationFilter       = Convert.ToString(Request["sSearch_9"]);
            var DepartmentFilter        = Convert.ToString(Request["sSearch_10"]);
            var VacancyFilter           = Convert.ToString(Request["sSearch_11"]);
            var RequiruitmentDateFilter = Convert.ToString(Request["sSearch_12"]);

        //00  //Id
        //01  //OfferType
        //02  //FullName
        //03  //PresentDistrictId
        //04  //Gender
        //05  //DOB
        //06  //JobCategoryId
        //07  //Mobile
        //08  //Email
        //09  //Designation
        //10  //Department
        //11  //Vacancy
        //12  //RequiruitmentDate


            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            DateTime RecFromDate = DateTime.MinValue;
            DateTime RecToDate = DateTime.MaxValue;
            if (DOBFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = DOBFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(DOBFilter.Split('~')[0]) == true ? Convert.ToDateTime(DOBFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = DOBFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(DOBFilter.Split('~')[1]) == true ? Convert.ToDateTime(DOBFilter.Split('~')[1]) : DateTime.MinValue;
            }
            if (RequiruitmentDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                RecFromDate = RequiruitmentDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(RequiruitmentDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(RequiruitmentDateFilter.Split('~')[0]) : DateTime.MinValue;
                RecToDate = RequiruitmentDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(RequiruitmentDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(RequiruitmentDateFilter.Split('~')[1]) : DateTime.MinValue;
            }

            #endregion Column Search

            EMJobOfferRepo _repo = new EMJobOfferRepo();
            var getAllData = _repo.SelectAll();
            IEnumerable<EMJobOfferVM> filteredData;
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
                var isSearchable10 = Convert.ToBoolean(Request["bSearchable_10"]);
                var isSearchable11 = Convert.ToBoolean(Request["bSearchable_11"]);
                var isSearchable12 = Convert.ToBoolean(Request["bSearchable_12"]);

                filteredData = getAllData
                   .Where(c =>
                           isSearchable1 &&  c.OfferType.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable2 &&  c.FullName.ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable3 &&  c.PresentDistrictId.ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable5 &&  c.Gender.ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable4 &&  c.DOB.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable6 &&  c.JobCategoryId.ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable7 &&  c.Mobile.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable8 &&  c.Email.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable9 &&  c.Designation.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable10 && c.Department.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable11 && c.Vacancy.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable12 && c.RequiruitmentDate.ToLower().Contains(param.sSearch.ToLower()) 
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            if ((DOBFilter != "" && DOBFilter != "~")
               || OfferTypeFilter         != ""  
               || FullNameFilter          != ""  
               || PresentDistrictIdFilter != ""  
               || GenderFilter            != ""  
               || DOBFilter               != ""  
               || JobCategoryIdFilter     != ""  
               || MobileFilter            != ""  
               || EmailFilter             != ""  
               || DesignationFilter       != ""  
               || DepartmentFilter        != ""  
               || VacancyFilter           != ""  
               || (RequiruitmentDateFilter != "" && RequiruitmentDateFilter != "~")
                )
            {
                filteredData = filteredData
                                .Where(c =>
                                    (OfferTypeFilter == "" || c.FullName.ToLower().Contains(OfferTypeFilter.ToLower()))
                                && (FullNameFilter == "" || c.PresentDistrictId.ToString().ToLower().Contains(FullNameFilter.ToLower()))
                                && (PresentDistrictIdFilter == "" || c.Gender.ToString().ToLower().Contains(PresentDistrictIdFilter.ToLower()))
                                && (GenderFilter == "" || c.JobCategoryId.ToString().ToLower().Contains(GenderFilter.ToLower()))
                                && (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.DOB))
                                && (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.DOB))
                                && (JobCategoryIdFilter == "" || c.JobCategoryId.ToString().ToLower().Contains(JobCategoryIdFilter.ToLower()))
                                && (MobileFilter == "" || c.Mobile.ToLower().Contains(MobileFilter.ToLower()))
                                && (EmailFilter  == "" || c.Email.ToLower().Contains(EmailFilter .ToLower()))
                                && (DesignationFilter       == "" || c.Designation.ToLower().Contains(DesignationFilter      .ToLower()))
                                && (DepartmentFilter       == "" || c.Department.ToLower().Contains(DepartmentFilter      .ToLower()))
                                && (VacancyFilter == "" || c.Vacancy.ToLower().Contains(VacancyFilter.ToLower()))
                                && (RecFromDate == DateTime.MinValue || RecFromDate <= Convert.ToDateTime(c.RequiruitmentDate))
                                && (RecToDate == DateTime.MaxValue || RecToDate >= Convert.ToDateTime(c.RequiruitmentDate))
                                
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
            var isSortable_10 = Convert.ToBoolean(Request["bSortable_10"]);
            var isSortable_11 = Convert.ToBoolean(Request["bSortable_11"]);
            var isSortable_12 = Convert.ToBoolean(Request["bSortable_12"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EMJobOfferVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ?   c.OfferType                    :
                sortColumnIndex == 2 && isSortable_2 ?   c.FullName                     :
                sortColumnIndex == 3 && isSortable_3 ?   c.PresentDistrictId.ToString() :
                sortColumnIndex == 4 && isSortable_4 ?   c.Gender                       :
                sortColumnIndex == 5 && isSortable_5 ?   c.DOB                          :
                sortColumnIndex == 6 && isSortable_6 ?   c.JobCategoryId.ToString()     :
                sortColumnIndex == 7 && isSortable_7 ?   c.Mobile                       :
                sortColumnIndex == 8 && isSortable_8 ?   c.Email                        :
                sortColumnIndex == 9 && isSortable_9 ?   c.Designation                  :
                sortColumnIndex == 10 && isSortable_10 ? c.Department                   :
                sortColumnIndex == 11 && isSortable_11 ? c.Vacancy                      :
                sortColumnIndex == 12 && isSortable_12 ? c.RequiruitmentDate            :
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
                , c.OfferType                   
                , c.FullName                    
                , c.PresentDistrictId.ToString()
                , c.Gender                      
                , c.DOB                         
                , c.JobCategoryId.ToString()    
                , c.Mobile                      
                , c.Email                       
                , c.Designation                 
                , c.Department                  
                , c.Vacancy                     
                , c.RequiruitmentDate           
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
