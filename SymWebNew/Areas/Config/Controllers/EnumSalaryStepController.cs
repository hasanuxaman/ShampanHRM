using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SymRepository.Enum;
using SymViewModel.Common;
using SymRepository.Enum;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymOrdinary;
using System.Threading;
using SymRepository.Common;

namespace SymWebUI.Areas.Config.Controllers
{
    [Authorize]
    public class EnumSalaryStepController : Controller
    {
        //
        // GET: /Enum/EnumSalaryStep/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        EnumSalaryStepRepo _repo = new EnumSalaryStepRepo();
        #region Actions
        public ActionResult Index()
        {
          Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_17", "index").ToString();

            return View();
        }
        public ActionResult _index(JQueryDataTableParamVM param, string code, string name)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var NameFilter = Convert.ToString(Request["sSearch_1"]);
            var IsActiveFilter = Convert.ToString(Request["sSearch_2"]);
            var remarksFilter = Convert.ToString(Request["sSearch_3"]);

            var IsActiveFilter1 = IsActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();
            #endregion Column Search

            var getAllData = _repo.SelectAll();
            IEnumerable<EnumSalaryStepVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Remarks.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (NameFilter != "" || IsActiveFilter != "" || remarksFilter != "")   

            {
                filteredData = filteredData
                                .Where(c =>
                                           (NameFilter == "" || c.Name.ToLower().Contains(NameFilter.ToLower()))
                                           &&
                                           (IsActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(IsActiveFilter1.ToLower()))
                                            &&
                                           (remarksFilter == "" || c.Remarks.ToString().ToLower().Contains(remarksFilter.ToLower()))
                                           );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EnumSalaryStepVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Remarks :
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
                             , c.Name
                             , Convert.ToString(c.IsActive == true ? "Active" : "Inactive") 
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

        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_17", "add").ToString();
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(EnumSalaryStepVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
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
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_17", "edit").ToString();

            return PartialView(_repo.SelectById(id));
        }

        [HttpPost]
        public ActionResult Edit(EnumSalaryStepVM vm, string btn)
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

        //public ActionResult Delete(int id)
        //{
        //    ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //    EnumSalaryStepVM vm = new EnumSalaryStepVM();
        //    vm.LastUpdateAt = Ordinary.DateToString(DateTime.Now);
        //    vm.LastUpdateBy = Ordinary.UserName;
        //    vm.LastUpdateFrom = Ordinary.WorkStationIP;
        //    vm.Id = id;
        //    try
        //    {
        //        string[] result = new string[6];
        //        result = _repo.Delete(vm);
        //        Session["result"] = result[0] + "~" + result[1];
        //        return RedirectToAction("Index");

        //    }
        //    catch (Exception)
        //    {
        //        Session["result"] = "Fail~Data Not Deleted";
        //        return RedirectToAction("Index");
        //    }

        //}
        public JsonResult Delete(string ids)
        {
            EnumSalaryStepVM vm = new EnumSalaryStepVM();
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_17", "delete").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        #endregion Actions

    }
}
