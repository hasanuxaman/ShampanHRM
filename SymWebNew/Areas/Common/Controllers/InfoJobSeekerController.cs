using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class InfoJobSeekerController : Controller
    {
        //
        // GET: /Common/InfoJobSeeker/

        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var FullNameFilter = Convert.ToString(Request["sSearch_1"]);
            var PresentDistrictIdFilter = Convert.ToString(Request["sSearch_2"]);
            var GenderFilter = Convert.ToString(Request["sSearch_3"]);
            var DOBFilter = Convert.ToString(Request["sSearch_4"]);
            var JobCategoryIdFilter = Convert.ToString(Request["sSearch_5"]);
            var JobLevelFilter = Convert.ToString(Request["sSearch_6"]);
            var JobNatureFilter = Convert.ToString(Request["sSearch_7"]);
            var CareerObjectiveFilter = Convert.ToString(Request["sSearch_8"]);
            var LastSchoolFilter = Convert.ToString(Request["sSearch_9"]);
            var LastDegreeFilter = Convert.ToString(Request["sSearch_10"]);
            var EducationSubjectFilter = Convert.ToString(Request["sSearch_11"]);
            var LastCompanyFilter = Convert.ToString(Request["sSearch_12"]);
            var ExperianceYearFilter = Convert.ToString(Request["sSearch_13"]);
            var MobileFilter = Convert.ToString(Request["sSearch_14"]);
            var EmailFilter = Convert.ToString(Request["sSearch_15"]);
            var SpecialityFilter = Convert.ToString(Request["sSearch_16"]);
            var WebsiteFilter = Convert.ToString(Request["sSearch_17"]);

            //Id
            //AgentId
            //FullName
            //PhotoName
            //CVName
            //PresentDistrictId
            //Gender
            //DOB
            //JobCategoryId
            //JobLevel
            //JobNature
            //CareerObjective
            //LastSchool
            //LastDegree
            //EducationSubject
            //LastCompany
            //ExperianceYear
            //Mobile
            //Email
            //Speciality
            //Website

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (DOBFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = DOBFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(DOBFilter.Split('~')[0]) == true ? Convert.ToDateTime(DOBFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = DOBFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(DOBFilter.Split('~')[1]) == true ? Convert.ToDateTime(DOBFilter.Split('~')[1]) : DateTime.MinValue;
            }

            #endregion Column Search

            InfoJobSeekerRepo _repo = new InfoJobSeekerRepo();
            var getAllData = _repo.SelectAll();
            IEnumerable<InfoJobSeekerVM> filteredData;
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
                var isSearchable13 = Convert.ToBoolean(Request["bSearchable_13"]);
                var isSearchable14 = Convert.ToBoolean(Request["bSearchable_14"]);
                var isSearchable15 = Convert.ToBoolean(Request["bSearchable_15"]);
                var isSearchable16 = Convert.ToBoolean(Request["bSearchable_16"]);
                var isSearchable17 = Convert.ToBoolean(Request["bSearchable_17"]);

                filteredData = getAllData
                   .Where(c =>
                           isSearchable1 && c.FullName.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable2 && c.PresentDistrictId.ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable3 && c.Gender.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable4 && c.DOB.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable5 && c.JobCategoryId.ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable6 && c.JobLevel.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable7 && c.JobNature.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable8 && c.CareerObjective.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable9 && c.LastSchool.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable10 && c.LastDegree.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable11 && c.EducationSubject.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable12 && c.LastCompany.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable13 && c.ExperianceYear.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable14 && c.Mobile.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable15 && c.Email.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable16 && c.Speciality.ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable17 && c.Website.ToLower().Contains(param.sSearch.ToLower()) 
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            if (FullNameFilter != "" || PresentDistrictIdFilter != "" || GenderFilter != "" || (DOBFilter != "" && DOBFilter != "~")
               || JobCategoryIdFilter != "" || JobLevelFilter != "" || JobNatureFilter != ""
               || CareerObjectiveFilter     != ""
               || LastSchoolFilter          != ""
               || LastDegreeFilter          != ""
               || EducationSubjectFilter    != ""
               || LastCompanyFilter         != ""
               || ExperianceYearFilter      != ""
               || MobileFilter              != ""
               || EmailFilter               != ""
               || SpecialityFilter             != ""
               || WebsiteFilter             != ""
                )
            {
                filteredData = filteredData
                                .Where(c =>
                                  (FullNameFilter == "" || c.FullName.ToLower().Contains(FullNameFilter.ToLower()))
                                 && (PresentDistrictIdFilter == "" || c.PresentDistrictId.ToString().ToLower().Contains(PresentDistrictIdFilter.ToLower()))
                                 && (GenderFilter == "" || c.Gender.ToString().ToLower().Contains(GenderFilter.ToLower()))
                                 && (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.DOB))
                                 && (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.DOB))
                                 && (JobCategoryIdFilter == "" || c.JobCategoryId.ToString().ToLower().Contains(JobCategoryIdFilter.ToLower()))
                                 && (JobLevelFilter == "" || c.JobLevel.ToLower().Contains(JobLevelFilter.ToLower()))
                                 && (JobNatureFilter == "" || c.JobNature.ToLower().Contains(JobNatureFilter.ToLower()))
                                 && (CareerObjectiveFilter  == "" || c.CareerObjective.ToLower().Contains(CareerObjectiveFilter .ToLower()))
                                 && (LastSchoolFilter       == "" || c.LastSchool.ToLower().Contains(LastSchoolFilter      .ToLower()))
                                 && (LastDegreeFilter       == "" || c.LastDegree.ToLower().Contains(LastDegreeFilter      .ToLower()))
                                 && (EducationSubjectFilter == "" || c.EducationSubject.ToLower().Contains(EducationSubjectFilter.ToLower()))
                                 && (LastCompanyFilter      == "" || c.LastCompany.ToLower().Contains(LastCompanyFilter     .ToLower()))
                                 && (ExperianceYearFilter   == "" || c.ExperianceYear.ToLower().Contains(ExperianceYearFilter  .ToLower()))
                                 && (MobileFilter           == "" || c.Mobile.ToLower().Contains(MobileFilter          .ToLower()))
                                 && (EmailFilter            == "" || c.Email.ToLower().Contains(EmailFilter           .ToLower()))
                                 && (SpecialityFilter          == "" || c.Speciality    .ToLower().Contains(SpecialityFilter         .ToLower()))
                                 && (WebsiteFilter          == "" || c.Website    .ToLower().Contains(WebsiteFilter         .ToLower()))
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
            var isSortable_13 = Convert.ToBoolean(Request["bSortable_13"]);
            var isSortable_14 = Convert.ToBoolean(Request["bSortable_14"]);
            var isSortable_15 = Convert.ToBoolean(Request["bSortable_15"]);
            var isSortable_16 = Convert.ToBoolean(Request["bSortable_16"]);
            var isSortable_17 = Convert.ToBoolean(Request["bSortable_17"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<InfoJobSeekerVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ?  c.FullName                      :
                sortColumnIndex == 2 && isSortable_2 ?  c.PresentDistrictId.ToString() :
                sortColumnIndex == 3 && isSortable_3 ?  c.Gender                        :
                sortColumnIndex == 4 && isSortable_4 ?  c.DOB                         :
                sortColumnIndex == 5 && isSortable_5 ?  c.JobCategoryId.ToString()                         :
                sortColumnIndex == 6 && isSortable_6 ?  c.JobLevel                         :
                sortColumnIndex == 7 && isSortable_7 ?  c.JobNature                         :
                sortColumnIndex == 8 && isSortable_8 ?  c.CareerObjective                         :
                sortColumnIndex == 9 && isSortable_9 ?  c.LastSchool                         :
                sortColumnIndex == 10 && isSortable_10 ? c.LastDegree                         :
                sortColumnIndex == 11 && isSortable_11 ? c.EducationSubject                         :
                sortColumnIndex == 12 && isSortable_12 ? c.LastCompany                         :
                sortColumnIndex == 13 && isSortable_13 ? c.ExperianceYear                         :
                sortColumnIndex == 14 && isSortable_14 ? c.Mobile                         :
                sortColumnIndex == 15 && isSortable_15 ? c.Email                         :
                sortColumnIndex == 16 && isSortable_16 ? c.Speciality                             :
                sortColumnIndex == 17 && isSortable_17 ? c.Website                             :
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
                , c.FullName                    
                , c.PresentDistrictId.ToString()
                , c.Gender                      
                , c.DOB                         
                , c.JobCategoryId.ToString()    
                , c.JobLevel                    
                , c.JobNature                   
                , c.CareerObjective             
                , c.LastSchool                  
                , c.LastDegree                  
                , c.EducationSubject            
                , c.LastCompany                 
                , c.ExperianceYear              
                , c.Mobile                      
                , c.Email                       
                , c.Speciality                     
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
