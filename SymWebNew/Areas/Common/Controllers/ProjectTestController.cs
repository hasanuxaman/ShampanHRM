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
    public class ProjectTestController : Controller
    {
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ProjectRepo _repo = new ProjectRepo();
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var nameFilter = Convert.ToString(Request["sSearch_1"]);
            var contactPersonFilter = Convert.ToString(Request["sSearch_2"]);
            var addressFilter = Convert.ToString(Request["sSearch_3"]);
            var countryFilter = Convert.ToString(Request["sSearch_4"]);
            var cityFilter = Convert.ToString(Request["sSearch_5"]);
            var mobileFilter = Convert.ToString(Request["sSearch_6"]);

     
            #endregion Column Search

            #region Search and Filter Data

            var getAllData = _repo.SelectAll();
            IEnumerable<ProjectVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);

                filteredData = getAllData.Where(c =>
                    isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.ContactPerson.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Address.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Country.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.City.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.Mobile.ToLower().Contains(param.sSearch.ToLower())                
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data

            #region Column Filtering
            if (nameFilter != "" || contactPersonFilter != "" || addressFilter != "" || countryFilter  != "" ||  cityFilter  != "" ||  mobileFilter  != "" )
            {
                filteredData = filteredData.Where(c =>
                    (nameFilter == "" || c.Name.ToLower().Contains(nameFilter.ToLower()))
                    && (contactPersonFilter == "" || c.ContactPerson.ToLower().Contains(contactPersonFilter.ToLower()))
                    && (addressFilter == "" || c.Address.ToLower().Contains(addressFilter.ToLower()))
                    && (countryFilter == "" || c.Country.ToLower().Contains(countryFilter.ToLower()))
                    && (cityFilter == "" || c.City.ToLower().Contains(cityFilter.ToLower()))
                    && (mobileFilter == "" || c.Mobile.ToLower().Contains(mobileFilter.ToLower()))
                    );
            }

            #endregion Column Filtering


            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ProjectVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Name :
                sortColumnIndex == 2 && isSortable_2 ? c.ContactPerson :
                sortColumnIndex == 3 && isSortable_3 ? c.Address :
                sortColumnIndex == 4 && isSortable_4 ? c.Country :
                sortColumnIndex == 5 && isSortable_5 ? c.City :
                sortColumnIndex == 6 && isSortable_6 ? c.Mobile :         
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
                , c.Name//+"~"+c.Name+"~"+(c.IsActive  ? "Active" : "Inactive")+"~"+c.Remarks
                , c.ContactPerson
                , c.Address
                , c.Country
                , c.City
                , c.Mobile
               
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
