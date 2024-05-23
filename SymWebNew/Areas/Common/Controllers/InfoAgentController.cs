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
    public class InfoAgentController : Controller
    {
        //
        // GET: /Common/InfoAgent/

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
            var MobileBankTypeFilter = Convert.ToString(Request["sSearch_3"]);
            var MobileBankNoFilter = Convert.ToString(Request["sSearch_4"]);
            var MobileNoFilter = Convert.ToString(Request["sSearch_5"]);
            var EmailFilter = Convert.ToString(Request["sSearch_6"]);
            var CompanyNameFilter = Convert.ToString(Request["sSearch_7"]);
            var GenderFilter = Convert.ToString(Request["sSearch_8"]);
            var DOBFilter = Convert.ToString(Request["sSearch_9"]);


            //Id
            //FullName
            //PhotoName
            //PresentDistrictId
            //MobileBankType
            //MobileBankNo
            //MobileNo
            //Email
            //CompanyName
            //Gender
            //DOB

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (DOBFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = DOBFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(DOBFilter.Split('~')[0]) == true ? Convert.ToDateTime(DOBFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = DOBFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(DOBFilter.Split('~')[1]) == true ? Convert.ToDateTime(DOBFilter.Split('~')[1]) : DateTime.MinValue;
            }

            #endregion Column Search

            InfoAgentRepo _repo = new InfoAgentRepo();
            var getAllData = _repo.SelectAll();
            IEnumerable<InfoAgentVM> filteredData;
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
                           isSearchable1 && c.FullName              .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable2 && c.PresentDistrictId     .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable3 && c.MobileBankType     .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable4 && c.MobileBankNo     .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable5 && c.MobileNo     .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable6 && c.Email     .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable7 && c.CompanyName     .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable8 && c.Gender     .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable9 && c.DOB     .ToLower().Contains(param.sSearch.ToLower()) 
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            if (FullNameFilter != "" || PresentDistrictIdFilter != "" || MobileBankTypeFilter != "" 
               || MobileBankNoFilter != "" || MobileNoFilter != "" || EmailFilter != ""
               || CompanyNameFilter     != ""
               || GenderFilter          != ""
               || (DOBFilter != "" && DOBFilter != "~")
                )
            {
                filteredData = filteredData
                                .Where(c =>
                                  (FullNameFilter == "" || c.FullName.ToLower().Contains(FullNameFilter.ToLower()))
                                 && (PresentDistrictIdFilter == "" || c.PresentDistrictId.ToString().ToLower().Contains(PresentDistrictIdFilter.ToLower()))
                                 && (MobileBankTypeFilter == "" || c.Gender.ToString().ToLower().Contains(MobileBankTypeFilter.ToLower()))
                                 && (MobileBankNoFilter == "" || c.MobileBankNo.ToString().ToLower().Contains(MobileBankNoFilter.ToLower()))
                                 && (MobileNoFilter == "" || c.MobileNo.ToLower().Contains(MobileNoFilter.ToLower()))
                                 && (EmailFilter == "" || c.Email.ToLower().Contains(EmailFilter.ToLower()))
                                 && (CompanyNameFilter  == "" || c.CompanyName.ToLower().Contains(CompanyNameFilter .ToLower()))
                                 && (GenderFilter       == "" || c.Gender.ToLower().Contains(GenderFilter      .ToLower()))
                                 && (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.DOB))
                                 && (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.DOB))
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
            Func<InfoAgentVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ?   c.FullName                      :
                sortColumnIndex == 2 && isSortable_2 ?   c.PresentDistrictId.ToString() :
                sortColumnIndex == 3 && isSortable_3 ?   c.MobileBankType               :
                sortColumnIndex == 4 && isSortable_4 ?   c.MobileBankNo                 :
                sortColumnIndex == 5 && isSortable_5 ?   c.MobileNo                     :
                sortColumnIndex == 6 && isSortable_6 ?   c.Email                        :
                sortColumnIndex == 7 && isSortable_7 ?   c.CompanyName                  :
                sortColumnIndex == 8 && isSortable_8 ?   c.Gender                       :
                sortColumnIndex == 9 && isSortable_9 ?   c.DOB                          :
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
                , c.MobileBankType              
                , c.MobileBankNo                
                , c.MobileNo                    
                , c.Email                       
                , c.CompanyName                 
                , c.Gender                      
                , c.DOB                         
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
