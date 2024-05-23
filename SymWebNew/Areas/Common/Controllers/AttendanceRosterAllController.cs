using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SymRepository.Common;
using SymViewModel.Common;
using SymOrdinary;
using System.Threading;
using SymViewModel.HRM;
using SymRepository.HRM;
using SymRepository.Attendance;
using SymViewModel.Attendance;
namespace SymWebUI.Areas.Common.Controllers
{

    [Authorize]
    public class AttendanceRosterAllController : Controller
    {
        AttendanceRosterAllRepo _repo = new AttendanceRosterAllRepo();
        
        public ActionResult Index()
        {
            return View("IndexDetail");
        }
        public ActionResult IndexDetail()
        {
            return View();
        }
        public ActionResult _index(JQueryDataTableParamVM param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var NameFilter = Convert.ToString(Request["sSearch_1"]);
            var CodeAmountFilter = Convert.ToString(Request["sSearch_2"]);
            var AttendanceStructureFilter = Convert.ToString(Request["sSearch_2"]);
            var AttendanceGroupFilter = Convert.ToString(Request["sSearch_2"]);
            #endregion Column Search
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var getAllData = _repo.SelectAll(Convert.ToInt32(identity.BranchId));
            IEnumerable<AttendanceRosterAllVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable4= Convert.ToBoolean(Request["bSearchable_2"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.AttendanceStructure.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.AttendanceGroup.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (NameFilter != "" || CodeAmountFilter != "" || AttendanceStructureFilter != "" || AttendanceGroupFilter != "")
            {
                filteredData = filteredData
                                .Where(c =>
                                            (NameFilter == "" || c.Name.ToLower().Contains(NameFilter.ToLower()))
                                            &&
                                             (CodeAmountFilter == "" || c.Code.ToLower().Contains(CodeAmountFilter.ToLower()))
                                              &&
                                             (AttendanceStructureFilter == "" || c.AttendanceStructure.ToLower().Contains(AttendanceStructureFilter.ToLower()))
                                              &&
                                             (AttendanceGroupFilter == "" || c.AttendanceGroup.ToLower().Contains(AttendanceGroupFilter.ToLower()))
                                        );
            }

            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<AttendanceRosterAllVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Name :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.AttendanceStructure :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.AttendanceGroup :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { Convert.ToString(c.Id), c.Code, c.Name, c.AttendanceStructure, c.AttendanceGroup };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult _indexDetail(JQueryDataTableParamVM param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var NameFilter = Convert.ToString(Request["sSearch_1"]);
            var AttendanceGroupFilter = Convert.ToString(Request["sSearch_2"]);
            var AttendanceStructureFilter = Convert.ToString(Request["sSearch_3"]);
            var FromDateFilter = Convert.ToString(Request["sSearch_4"]);
            var ToDateFilter = Convert.ToString(Request["sSearch_5"]);
            var RemarksFilter = Convert.ToString(Request["sSearch_6"]);
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;

            DateTime fromDateTo = DateTime.MinValue;
            DateTime toDateTo = DateTime.MaxValue;

            if (FromDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = FromDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(FromDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(FromDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = FromDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(FromDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(FromDateFilter.Split('~')[1]) : DateTime.MinValue;
            }
            if (ToDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDateTo = ToDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(ToDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(ToDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDateTo = ToDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(ToDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(ToDateFilter.Split('~')[1]) : DateTime.MinValue;
            }
            #endregion Column Search
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var getAllData = _repo.SelectDetailAll();
            IEnumerable<AttendanceRosterAllVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.AttendanceGroup.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.AttendanceStructure.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.FromDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.ToDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (NameFilter != "" || AttendanceGroupFilter != "" || AttendanceStructureFilter != "" || RemarksFilter != "" 
                || (FromDateFilter != "~" && FromDateFilter != "") || (ToDateFilter != "~" && ToDateFilter != ""))
            {
                filteredData = filteredData
                                .Where(c =>
                                            (NameFilter == "" || c.Name.ToLower().Contains(NameFilter.ToLower()))
                                            && (AttendanceGroupFilter == "" || c.AttendanceGroup.ToLower().Contains(AttendanceGroupFilter.ToLower()))
                                            && (AttendanceStructureFilter == "" || c.AttendanceStructure.ToLower().Contains(AttendanceStructureFilter.ToLower()))
                                            && (RemarksFilter == "" || c.Remarks.ToLower().Contains(RemarksFilter.ToLower()))
                                            && (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.FromDate))
                                            && (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.FromDate))
                                            && (fromDateTo == DateTime.MinValue || fromDateTo <= Convert.ToDateTime(c.ToDate))
                                            && (toDateTo == DateTime.MaxValue || toDateTo >= Convert.ToDateTime(c.ToDate))
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
            Func<AttendanceRosterAllVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.AttendanceGroup :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.AttendanceStructure :
                                                           sortColumnIndex == 4 && isSortable_4 ? Ordinary.DateToString(c.FromDate) :
                                                           sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.ToDate) :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Remarks :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { 
                c.Id
                , c.Name
                , c.AttendanceGroup
                , c.AttendanceStructure
                , c.FromDate
                , c.ToDate
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
            return PartialView();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult CreateDetail()
        {
            AttendanceRosterAllVM vm = new AttendanceRosterAllVM();
            return PartialView("CreateDetail",vm);
        }
        [HttpPost]
        public ActionResult CreateDetail(AttendanceRosterAllVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            try
            {
                result = _repo.InsertDetail(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(AttendanceRosterAllVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            try
            {
                
                result = _repo.Insert(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            return PartialView(_repo.SelectById(id));
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(AttendanceRosterAllVM vm, string btn)
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
                Session["result"] = "Fail~Data Not Updated";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Editdetail(string id)
        {
            string[] a = id.Split('~');
            string AttendanceStructureId = a[0];
            string AttendanceGroupId = a[1];
            return PartialView("EditDetail", _repo.SelectByDetailId(AttendanceStructureId, AttendanceGroupId));
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Editdetail(AttendanceRosterAllVM vm, string btn)
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
                Session["result"] = "Fail~Data Not Updated";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult Delete(string Ids)
        {
            string[] result = new string[6];
            var name = Ids.Split('~')[0];
            try
            {
                result = _repo.Delete(name);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("IndexDetail");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Deleted";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("IndexDetail");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult AttendanceRosterAllDelete(string ids)
        {
            AttendanceRosterAllVM vm = new AttendanceRosterAllVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            //result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        public JsonResult Areer(string value, string date, string employeeId)
        {

            return Json("successed", JsonRequestBehavior.AllowGet);
        }  
    }
}
