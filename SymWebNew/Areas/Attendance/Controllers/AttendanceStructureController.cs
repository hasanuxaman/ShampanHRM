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

namespace SymWebUI.Areas.Attendance.Controllers
{
     [Authorize]
    public class AttendanceStructureController : Controller
    {
        //
        // GET: /Attendance/TimePolicy/
        AttendanceStructureRepo _repo = new AttendanceStructureRepo();
        public ActionResult Index()
        {
            //            return View(timePRepo.SelectAll());
            return View();
        }
        public ActionResult _index(JQueryDataTableParamVM param)
        {

            var getAllData = _repo.SelectAll();
            IEnumerable<AttendanceStructureVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.InTime.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.OutTime.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && Convert.ToString(c.InGrace).ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.InTimeEnd.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.LunchTime.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.LunchBreak.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<AttendanceStructureVM, string> orderingFunction = (c =>
                                                           sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.InTime :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.OutTime :
                                                           sortColumnIndex == 4 && isSortable_4 ? Convert.ToString(c.InGrace) :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.InTimeEnd :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.LunchTime :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.LunchBreak.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { 
                c.Id.ToString()
                , c.Name //+ "~" + Convert.ToString(c.Id)
                , c.InTime, c.OutTime
                , c.InGrace.ToString()
                , c.InTimeEnd, c.LunchTime
                , c.LunchBreak.ToString()  
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
        public ActionResult Create(AttendanceStructureVM TimePolicyVM)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            TimePolicyVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            TimePolicyVM.CreatedBy = identity.Name;
            TimePolicyVM.CreatedFrom = identity.WorkStationIP;
            try
            {
                string[] result = new string[6];
                result = _repo.Insert(TimePolicyVM);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Save Failed";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            AttendanceStructureVM vm = _repo.SelectById(id);
            return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(AttendanceStructureVM vm)
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
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult TimePolicyDelete(string ids)
        {
            AttendanceStructureVM vm = new AttendanceStructureVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
