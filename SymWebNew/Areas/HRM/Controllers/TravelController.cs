using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    [Authorize]
    public class TravelController : Controller
    {
        EmployeeTravelRepo _empTrvApp;
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index(string id, string empcode, string btn)
        {
            string currentId = "";
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (empcode != null && btn != null)
            {
                vm = _infoRepo.SelectEmpForSearch(empcode, btn);
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    id = vm.Id;
                }
                else
                {
                    currentId = _infoRepo.DropDown(empcode).FirstOrDefault().Id;
                    id = currentId;
                }
            }
            if (Session["mgs"].ToString() != "")
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            if (!(identity.IsAdmin || identity.IsHRM))
            {
                id = identity.EmployeeId;
            }
            if (id != null)
            {
                vm = _infoRepo.SelectById(id);
                vm.Id = id;
            }
            return View(vm);
        }
        public ActionResult _index(JQueryDataTableParamVM param, string Id)//EmployeeId
        {
            _empTrvApp = new EmployeeTravelRepo();
            var getAllData = _empTrvApp.SelectAllByEmployee(Id);
            IEnumerable<EmployeeTravelVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TravelType_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.FromDate.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.ToDate.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.TravelToAddress.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeTravelVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TravelType_E :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.FromDate :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.ToDate :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.TravelToAddress :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies 
                         select new[] 
                         { 
                             Convert.ToString(c.Id)
                             , c.TravelType_E //+ "~" + Convert.ToString(c.Id)
                             , c.TravelToAddress
                             , c.FromDate
                             , c.ToDate
                             ,c.TravleTime 
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
        [HttpGet]
        public ActionResult Travel(string EmployeeId, int Id)
        {
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.travelVM = new EmployeeTravelRepo().SelectById(Id);

            }
            else
            {
                EmployeeTravelVM trv = new EmployeeTravelVM();
                trv.EmployeeId = EmployeeId;
                vm.travelVM = trv;
            }
            return PartialView("_editTravel", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Travel(EmployeeInfoVM vm, HttpPostedFileBase TravelF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            string[] retResults = new string[6];
            _empTrvApp = new EmployeeTravelRepo();
            EmployeeTravelVM tran = new EmployeeTravelVM();
            tran = vm.travelVM;
            if (TravelF != null && TravelF.ContentLength > 0)
            {
                tran.FileName = TravelF.FileName;
            }
            if (tran.Id <= 0)
            {
                tran.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                tran.CreatedBy = identity.Name;
                tran.CreatedFrom = identity.WorkStationIP;
                retResults=_empTrvApp.Insert(tran);
            }
            else
            {
                tran.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                tran.LastUpdateBy = identity.Name;
                tran.LastUpdateFrom = identity.WorkStationIP;
                retResults= _empTrvApp.Update(tran);
            }
            if (TravelF != null && TravelF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Travel"), retResults[2] + TravelF.FileName);
                TravelF.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            Session["mgs"] = "mgs";
            return RedirectToAction("Index", new { Id = tran.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult TravelDelete(string ids)
        {
            _empTrvApp = new EmployeeTravelRepo();
            EmployeeTravelVM vm = new EmployeeTravelVM();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _empTrvApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
