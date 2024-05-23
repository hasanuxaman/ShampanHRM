using JQueryDataTables.Models;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class BlockEmployerController : Controller
    {
        //
        // GET: /Common/BlockEmployer/

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
            var JobCategoryIdFilter = Convert.ToString(Request["sSearch_3"]);
            var ContactPersonNameFilter = Convert.ToString(Request["sSearch_4"]);
            var ContactPersonDesignationFilter = Convert.ToString(Request["sSearch_5"]);
            var ContactPersonMobileFilter = Convert.ToString(Request["sSearch_6"]);
            var ContactPersonEmailFilter = Convert.ToString(Request["sSearch_7"]);
            var WebsiteFilter = Convert.ToString(Request["sSearch_8"]);
            var BlockStatusFilter = Convert.ToString(Request["sSearch_9"]);

            var BlockStatusFilter1 = BlockStatusFilter.ToLower() == "blocked" ? "Y" : "N";

                   //Id
                   //JobSeekerId
                   //EmployerId
                   //FullName
                   //LogoName
                   //PresentDistrictId
                   //JobCategoryId
                   //ContactPersonName
                   //ContactPersonDesignation
                   //ContactPersonMobile
                   //ContactPersonEmail
                   //Website
                   //BlockStatus 

            #endregion Column Search

            BlockEmployerRepo _repo = new BlockEmployerRepo();
            var getAllData = _repo.SelectAll();
            IEnumerable<BlockEmployerVM> filteredData;
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
                         ||isSearchable3 && c.JobCategoryId.ToString()  .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable4 && c.ContactPersonName .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable5 && c.ContactPersonDesignation .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable6 && c.ContactPersonMobile .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable7 && c.ContactPersonEmail .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable8 && c.Website  .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable9 && c.BlockStatus  .ToLower().Contains(param.sSearch.ToLower()) 
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            if (FullNameFilter != "" || PresentDistrictIdFilter != "" || JobCategoryIdFilter != "" 
               || ContactPersonNameFilter != "" || ContactPersonDesignationFilter != "" || ContactPersonMobileFilter != ""
               || ContactPersonEmailFilter     != ""
               || WebsiteFilter          != ""
               || BlockStatusFilter          != ""
                )
            {
                filteredData = filteredData
                                .Where(c =>
                                  (FullNameFilter == "" || c.FullName.ToLower().Contains(FullNameFilter.ToLower()))
                                 && (PresentDistrictIdFilter == "" ||   c.PresentDistrictId.ToString().ToLower().Contains(PresentDistrictIdFilter.ToLower()))
                                 && (JobCategoryIdFilter == "" ||       c.JobCategoryId.ToString().ToLower().Contains(JobCategoryIdFilter.ToLower()))
                                 && (ContactPersonNameFilter == "" ||   c.ContactPersonName.ToString().ToLower().Contains(ContactPersonNameFilter.ToLower()))
                                 && (ContactPersonDesignationFilter == "" || c.ContactPersonDesignation.ToLower().Contains(ContactPersonDesignationFilter.ToLower()))
                                 && (ContactPersonMobileFilter == "" || c.ContactPersonMobile.ToLower().Contains(ContactPersonMobileFilter.ToLower()))
                                 && (ContactPersonEmailFilter  == "" || c.ContactPersonEmail.ToLower().Contains(ContactPersonEmailFilter .ToLower()))
                                 && (WebsiteFilter       == "" ||       c.Website.ToLower().Contains(WebsiteFilter      .ToLower()))
                                 && (BlockStatusFilter       == "" ||       c.BlockStatus.ToLower().Contains(BlockStatusFilter1      .ToLower()))
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
            Func<BlockEmployerVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ?   c.FullName                     :
                sortColumnIndex == 2 && isSortable_2 ?   c.PresentDistrictId.ToString() :
                sortColumnIndex == 3 && isSortable_3 ?   c.JobCategoryId.ToString()     :
                sortColumnIndex == 4 && isSortable_4 ?   c.ContactPersonName            :
                sortColumnIndex == 5 && isSortable_5 ?   c.ContactPersonDesignation     :
                sortColumnIndex == 6 && isSortable_6 ?   c.ContactPersonMobile          :
                sortColumnIndex == 7 && isSortable_7 ?   c.ContactPersonEmail           :
                sortColumnIndex == 8 && isSortable_8 ?   c.Website                      :
                sortColumnIndex == 9 && isSortable_9 ?   c.BlockStatus                  :
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
                , c.JobCategoryId.ToString()    
                , c.ContactPersonName           
                , c.ContactPersonDesignation    
                , c.ContactPersonMobile         
                , c.ContactPersonEmail          
                , c.Website                     
                , c.BlockStatus.ToLower() == "y"? "Blocked":"Not Blocked"                     
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
