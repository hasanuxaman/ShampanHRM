using SymOrdinary;
using SymRepository.Common;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
///k
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class BonusStructureController : Controller
    {
        //
        // GET: /Common/BonusStructure/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        BonusStructureRepo _repo = new BonusStructureRepo();
        #region Actions
        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_52", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        public ActionResult _index(JQueryDataTableParamVM param, string code, string name)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var CodeFilter = Convert.ToString(Request["sSearch_1"]);
            var NameFilter = Convert.ToString(Request["sSearch_2"]);
            var BonusValueFilter = Convert.ToString(Request["sSearch_3"]);
            var jobAgeFilter = Convert.ToString(Request["sSearch_4"]);
            var IsFixedFilter = Convert.ToString(Request["sSearch_5"]);
            var AmountFrom = 0;
            var AmountTo = 0;
            if (BonusValueFilter.Contains('~'))
            {
                AmountFrom = BonusValueFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(BonusValueFilter.Split('~')[0]) == true ? Convert.ToInt32(BonusValueFilter.Split('~')[0]) : 0;
                AmountTo = BonusValueFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(BonusValueFilter.Split('~')[1]) == true ? Convert.ToInt32(BonusValueFilter.Split('~')[1]) : 0;
            }
            var IsFixedFilter1 = IsFixedFilter.ToLower() == "fixed" ? true.ToString() : false.ToString();
            var getAllData = _repo.SelectAll();
            IEnumerable<BonusStructureVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.BonusValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.JobAge.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.IsFixed.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Column Search
            #region Column Filtering
            if (CodeFilter != "" || NameFilter != "" || (BonusValueFilter != "~" && BonusValueFilter != "") || jobAgeFilter != "" || IsFixedFilter != "")
            {
                filteredData = filteredData
                                .Where(c =>
                                            (CodeFilter == "" || c.Code.ToLower().Contains(CodeFilter.ToLower()))
                                            &&
                                           (NameFilter == "" || c.Name.ToLower().Contains(NameFilter.ToLower()))
                                            &&
                                           (jobAgeFilter == "" || c.JobAge.ToString().ToLower().Contains(jobAgeFilter.ToLower()))
                                           &&
                                           (IsFixedFilter == "" || c.IsFixed.ToString().ToLower().Contains(IsFixedFilter1.ToLower()))
                                            &&
                                            (AmountFrom == 0 || AmountFrom <= Convert.ToInt32(c.BonusValue))
                                            &&
                                            (AmountTo == 0 || AmountTo >= Convert.ToInt32(c.BonusValue))
                                        );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<BonusStructureVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Name :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.BonusValue.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.JobAge.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.IsFixed.ToString() :
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
                             , c.Code
                             , c.Name
                             , c.BonusValue.ToString()
                             , c.JobAge.ToString()
                             , c.IsFixed == true ? "Fixed" : "Rate"
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
             var permission = _reposur.SymRoleSession(identity.UserId, "1_52", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(BonusStructureVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = Identity.Name;
            vm.CreatedFrom = Identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(Identity.BranchId);
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
            var permission = _reposur.SymRoleSession(identity.UserId, "1_52", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View(_repo.SelectById(id));
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(BonusStructureVM vm, string btn)
        {
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = Identity.Name;
            vm.LastUpdateFrom = Identity.WorkStationIP;
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
        public ActionResult Delete(string id)
        {
             var permission = _reposur.SymRoleSession(identity.UserId, "1_52", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            BonusStructureVM vm = new BonusStructureVM();
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = Identity.Name;
            vm.LastUpdateFrom = Identity.WorkStationIP;
            vm.Id = id;
            try
            {
                //string[] result = new string[6];
                //result = _repo.Delete(vm);
                //Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Deleted";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult BonusStructureDelete(string ids)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_52", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            BonusStructureVM vm = new BonusStructureVM();
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
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
