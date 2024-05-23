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
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    [Authorize]
    public class HoliDayController : Controller
    {
        //
        // GET: /Common/TimePolicy/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        HoliDayRepo holiDayRepo = new HoliDayRepo();
        public ActionResult Index()
        {
            //            return View(holiDayRepo.SelectAll());
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_12", "index").ToString();

            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            #region Column Search
            var holiDayFilter = Convert.ToString(Request["sSearch_1"]);
            var holiDayTypeFilter = Convert.ToString(Request["sSearch_2"]);
            var remarksFilter = Convert.ToString(Request["sSearch_3"]);
            //Name
            //HoliDay
            //HoliDayType

            #endregion Column Search

            #region Search and Filter Data

            var getAllData = holiDayRepo.SelectAll();
            IEnumerable<HoliDayVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.HoliDay.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.HoliDayType.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable3 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (holiDayFilter != "" || holiDayTypeFilter != "" || remarksFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (holiDayFilter == "" || c.HoliDay.ToLower().Contains(holiDayFilter.ToLower()))
                                            &&
                                            (holiDayTypeFilter == "" || c.HoliDayType.ToLower().Contains(holiDayTypeFilter.ToLower()))
                                            &&
                                            (remarksFilter == "" || c.Remarks.ToLower().Contains(remarksFilter.ToLower()))

                                        );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<HoliDayVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? Ordinary.DateToString(c.HoliDay) :
                sortColumnIndex == 2 && isSortable_2 ? c.HoliDayType :
                sortColumnIndex == 3 && isSortable_3 ? c.Remarks :
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
                //, c.Name //+ "~" + Convert.ToString(c.Id)
                , c.HoliDay
                , c.HoliDayType 
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
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_12", "add").ToString();

            return View();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(HoliDayVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.Name = vm.HoliDay;
            vm.Id = 0;
            try
            {
                result = holiDayRepo.Insert(vm);
                Session["result"] = result[0] + "~" + result[1];
                //////return RedirectToAction("Index");
                if (result[0] == "Fail")
                {
                    return View(vm);
                }
                else
                {
                    return RedirectToAction("Edit", new { id = vm.Id });
                }

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Save Failed";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(vm);
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_12", "edit").ToString();
            HoliDayVM vm = new HoliDayVM();
            vm = holiDayRepo.SelectById(id);
            return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(HoliDayVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.Name = vm.HoliDay;
            try
            {
                result = holiDayRepo.Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                ////return RedirectToAction("Index");
                return View(vm);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(vm);
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult HoliDayDelete(string ids)
        {
            HoliDayVM vm = new HoliDayVM();
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_12", "delete").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = holiDayRepo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

    }

}
