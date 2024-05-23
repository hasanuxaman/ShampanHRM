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
    public class EmployeeAssetController : Controller
    {
        //
        // GET: /HRM/EmployeeAsset/

        EmployeeAssetRepo _repo = new EmployeeAssetRepo();
        EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        public ActionResult Index(string id, string empcode, string btn)
        {
            string currentId = "";
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeAssetVM asset = new EmployeeAssetVM();
            if (empcode != null && btn != null)
            {
                currentId = _infoRepo.DropDown(empcode).FirstOrDefault().Id;
                vm = _infoRepo.SelectEmpForSearch(empcode, btn);
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    id = vm.Id;
                }
                else
                {
                    id = currentId;
                }
            }
            if (Session["mgs"].ToString() != "")
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                id = identity.EmployeeId;
            }
            if (id != null)
            {
                vm = _infoRepo.SelectById(id);
            }
            vm.AssetVM = asset;
            return View(vm);
        }

        public ActionResult _index(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            _repo = new EmployeeAssetRepo();
            var getAllData = _repo.SelectAll(0, Id);


            IEnumerable<EmployeeAssetVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.AssetName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.IssueDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.Remarks.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeAssetVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.AssetName :
                                                           sortColumnIndex == 2 && isSortable_2 ? Ordinary.DateToString(c.IssueDate) :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Remarks :
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
                             c.Id.ToString()
                             , c.AssetName //+ "~" + Convert.ToString(c.Id)
                             , c.IssueDate
                             , c.Remarks.ToString() 
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
        public ActionResult Asset(string EmployeeId, int Id = 0)
        {
            _repo = new EmployeeAssetRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);
            if (Id != 0)
            {
                vm.AssetVM = _repo.SelectAll(Id).FirstOrDefault();
            }
            else
            {
                EmployeeAssetVM asset = new EmployeeAssetVM();
                asset.EmployeeId = EmployeeId;
                vm.AssetVM = asset;
            }
            return PartialView("_empAsset", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(EmployeeInfoVM vme, HttpPostedFileBase AssetFile)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeAssetVM vm = new EmployeeAssetVM();
            _repo = new EmployeeAssetRepo();
            vm = vme.AssetVM;
            if (AssetFile != null && AssetFile.ContentLength > 0)
            {
                vm.FileName = AssetFile.FileName;
            }
            string[] retResults = new string[6];
            if (vm.Id <= 0)
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                retResults = _repo.Insert(vm);
            }
            else
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                retResults = _repo.Update(vm);
            }
            if (AssetFile != null && AssetFile.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Asset"), retResults[2] + AssetFile.FileName);
                AssetFile.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            Session["mgs"] = "mgs";
            return RedirectToAction("Index", new { Id = vm.EmployeeId, mgs = mgs });
        }

        public JsonResult Delete(string ids)
        {
            EmployeeAssetVM vm = new EmployeeAssetVM();
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
