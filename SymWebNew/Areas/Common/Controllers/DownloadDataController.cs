using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Attendance;
using SymRepository.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class DownloadDataController : Controller
    {
        //
        // GET: /Attendance/DownloadData/

        DownloadDataRepo _repo = new DownloadDataRepo();
        //#region Actions
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var PunchDataFilter = Convert.ToString(Request["sSearch_1"]);
            var ProxyIDFilter = Convert.ToString(Request["sSearch_2"]);
            var PunchDateFilter = Convert.ToString(Request["sSearch_3"]);
            var PunchTimeFilter = Convert.ToString(Request["sSearch_4"]);
            var NodeIDFilter = Convert.ToString(Request["sSearch_5"]);
            var IsMigrateFilter = Convert.ToString(Request["sSearch_6"]);


            var IsMigrateFilter1 = IsMigrateFilter.ToLower() == "migrated" ? true.ToString() : false.ToString();
            #endregion Column Search

            #region Search and Filter Data

            var getAllData = _repo.SelectAll();
            IEnumerable<DownloadDataVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);

                filteredData = getAllData.Where(c =>
                    isSearchable1 && c.PunchData.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.ProxyID.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.PunchDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.PunchTime.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.NodeID.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.IsMigrate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data

            #region Column Filtering
            if (PunchDataFilter != "" || ProxyIDFilter != "" || IsMigrateFilter != "" || PunchDateFilter != "")
            {
                filteredData = filteredData.Where(c =>
                    (PunchDataFilter == "" || c.PunchData.ToLower().Contains(PunchDataFilter.ToLower()))
                    && (ProxyIDFilter == "" || c.ProxyID.ToLower().Contains(ProxyIDFilter.ToLower()))
                    && (PunchDateFilter == "" || c.PunchDate.ToLower().Contains(PunchDateFilter.ToLower()))
                    && (PunchTimeFilter == "" || c.PunchTime.ToLower().Contains(PunchTimeFilter.ToLower()))
                    && (NodeIDFilter == "" || c.NodeID.ToLower().Contains(NodeIDFilter.ToLower()))
                    && (IsMigrateFilter == "" || c.IsMigrate.ToString().ToLower().Contains(IsMigrateFilter1.ToLower()))
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
            Func<DownloadDataVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.PunchData :
                sortColumnIndex == 2 && isSortable_2 ? c.ProxyID :
                sortColumnIndex == 3 && isSortable_3 ? c.PunchDate :
                sortColumnIndex == 4 && isSortable_4 ? c.PunchTime :
                sortColumnIndex == 5 && isSortable_5 ? c.NodeID :
                sortColumnIndex == 6 && isSortable_6 ? c.IsMigrate.ToString() :
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
                , c.PunchData
                , c.ProxyID
                , c.PunchDate
                , c.PunchTime
                , c.NodeID
                , Convert.ToString(c.IsMigrate == true ? "Migrated" : "Not Migrated") 
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

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(DownloadDataVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            //vm.BranchId = Convert.ToInt32(identity.BranchId);
            try
            {
                result = _repo.Insert(vm);
                Session["result"] = result[0] + "~" + result[1];
                if (result[0].ToLower() == "fail")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View();
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            return View(_repo.SelectById(id));
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(DownloadDataVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {

                result = _repo.Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                if (result[0].ToLower() == "fail")
                {
                    return View(vm);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Updated";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(vm);
            }
        }

        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult Delete(string ids)
        {
            DownloadDataVM vm = new DownloadDataVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        //#endregion Actions

    }
}
