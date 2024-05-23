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
    public class TrainingController : Controller
    {
        EmployeeTrainingRepo _empTraApp;
        EmployeeInfoRepo _infoRepo;
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
            }
            vm.Id = id;
            return View(vm);
        }
        public ActionResult _index(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

             _empTraApp = new EmployeeTrainingRepo();
             var getAllData = _empTraApp.SelectAllByEmployee(Id);
            IEnumerable<EmployeeTrainingVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Topics.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Achievement.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.TrainingPlace_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.Location.ToLower().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeTrainingVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Topics :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Achievement :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.TrainingPlace_E :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.Location :
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
                             , c.Topics //+ "~" + Convert.ToString(c.Id)
                             , c.Achievement, c.TrainingPlace_E
                             , c.Location
                             ,c.DurationDay.ToString() 
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
        public ActionResult Training(string EmployeeId, int Id)
        {
             _infoRepo = new EmployeeInfoRepo();
             EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.trainingVM = new EmployeeTrainingRepo().SelectById(Id);

            }
            else
            {
                EmployeeTrainingVM tran = new EmployeeTrainingVM();
                tran.EmployeeId = EmployeeId;
                vm.trainingVM = tran;
            }
            return PartialView("_edittraining", vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Training(EmployeeInfoVM vm, HttpPostedFileBase TrainingF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            string[] retResults = new string[6];
             _empTraApp = new EmployeeTrainingRepo();
            EmployeeTrainingVM tran = new EmployeeTrainingVM();
            tran = vm.trainingVM;
            if (TrainingF != null && TrainingF.ContentLength > 0)
            {
                tran.FileName = TrainingF.FileName;
            }
            if (tran.Id <= 0)
            {
                tran.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                tran.CreatedBy = identity.Name;
                tran.CreatedFrom = identity.WorkStationIP;
                retResults=_empTraApp.Insert(tran);
            }
            else
            {
                tran.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                tran.LastUpdateBy = identity.Name;
                tran.LastUpdateFrom = identity.WorkStationIP;
               retResults= _empTraApp.Update(tran);
            }
            if (TrainingF != null && TrainingF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Training"), retResults[2] + TrainingF.FileName);
                TrainingF.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            //return Json(mgs, JsonRequestBehavior.AllowGet);
            Session["mgs"] = "mgs";
            return RedirectToAction("Index", new { Id = tran.EmployeeId, mgs = mgs });
        }

        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult Delete(string ids)
        {
            _empTraApp = new EmployeeTrainingRepo();
            EmployeeTrainingVM vm = new EmployeeTrainingVM();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _empTraApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
