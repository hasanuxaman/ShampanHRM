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
    public class ImmigrationController : Controller
    {
        EmployeeImmigrationRepo _immRepo;
        EmployeeInfoRepo _infoRepo;
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
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
            }
            return View(vm);
        }
        public ActionResult _index(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            EmployeeImmigrationRepo _empImRepo = new EmployeeImmigrationRepo();
            var getAllData = _empImRepo.SelectAllByEmployee(Id);
            IEnumerable<EmployeeImmigrationVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.ImmigrationNumber.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.ImmigrationType_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.IssuedBy_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.IssueDate.ToLower().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeImmigrationVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.ImmigrationNumber :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.ImmigrationType_E :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.IssuedBy_E :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.IssueDate :
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
                             ,c.ImmigrationNumber //+ "~" + Convert.ToString(c.Id)
                             , c.ImmigrationType_E
                             , c.IssuedBy_E
                             , c.IssueDate 
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
        public ActionResult Immigration(string EmployeeId, int Id)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.immigrationVM = new EmployeeImmigrationRepo().SelectById(Id);

            }
            else
            {
                EmployeeImmigrationVM imm = new EmployeeImmigrationVM();
                imm.EmployeeId = EmployeeId;
                vm.immigrationVM = imm;
            }
            return PartialView("_editImmigration", vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Immigration(EmployeeInfoVM vm,HttpPostedFileBase ImmigrationF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            string[] retResults = new string[6];
            EmployeeImmigrationRepo empImmApp = new EmployeeImmigrationRepo();
            EmployeeImmigrationVM immVM = new EmployeeImmigrationVM();
            immVM = vm.immigrationVM;
            if (ImmigrationF != null && ImmigrationF.ContentLength > 0)
            {
                immVM.FileName = ImmigrationF.FileName;
            }
            if (immVM.Id <= 0)
            {
                immVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                immVM.CreatedBy = identity.Name;
                immVM.CreatedFrom = identity.WorkStationIP;
                retResults=empImmApp.Insert(immVM);
            }
            else
            {
                immVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                immVM.LastUpdateBy = identity.Name;
                immVM.LastUpdateFrom = identity.WorkStationIP;
                retResults=empImmApp.Update(immVM);
            }
            if (ImmigrationF != null && ImmigrationF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Immigration"), retResults[2] + ImmigrationF.FileName);
                ImmigrationF.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            Session["mgs"] = "mgs";
            //return Json(mgs, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index", new { Id = immVM.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult ImmigrationDelete(string ids)
        {
            EmployeeImmigrationRepo empImmApp = new EmployeeImmigrationRepo();
            EmployeeImmigrationVM vm = new EmployeeImmigrationVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "delete").ToString();

            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empImmApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
