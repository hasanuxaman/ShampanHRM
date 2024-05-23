using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.Enum;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class GLAccountController : Controller
    {
        //
        // GET: /Payroll/GLAccount/
				SymUserRoleRepo _reposur = new SymUserRoleRepo();
				ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        GLAccountRepo _repo = new GLAccountRepo();
        #region Actions
        public ActionResult Index()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_35", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param, string code, string name)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var GLAccountCodeFilter = Convert.ToString(Request["sSearch_1"]);
            var DescriptionFilter = Convert.ToString(Request["sSearch_2"]);
            var isActiveFilter = Convert.ToString(Request["sSearch_3"]);
            var projectFilter = Convert.ToString(Request["sSearch_4"]);
            var voucherTypeFilter = Convert.ToString(Request["sSearch_5"]);
            // GLAccountCode
            // Description 
            //IsActive
            //Remarks
            var isActiveFilter1 = isActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();
            var fromID = 0;
            var toID = 0;
            if (idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }
            #endregion Column Search
            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<GLAccountVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData
                   .Where(c => 
                       isSearchable1 && c.GLAccountCode.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.Description.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Project.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.VoucherType.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            #region Column Filtering
            if (GLAccountCodeFilter != "" || DescriptionFilter != "" || isActiveFilter != "" || projectFilter != "" || voucherTypeFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (GLAccountCodeFilter == "" || c.GLAccountCode.ToLower().Contains(GLAccountCodeFilter.ToLower()))
                                            &&
                                            (DescriptionFilter == "" || c.Description.ToLower().Contains(DescriptionFilter.ToLower()))
                                            &&
                                            (isActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(isActiveFilter1.ToLower()))
                                            &&
                                           (projectFilter == "" || c.Project.ToString().ToLower().Contains(projectFilter.ToLower()))
                                           &&
                                           (voucherTypeFilter == "" || c.VoucherType.ToString().ToLower().Contains(voucherTypeFilter.ToLower()))

                                           );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<GLAccountVM, string> orderingFunction = (c => 
                sortColumnIndex == 1 && isSortable_1 ? c.GLAccountCode :
                sortColumnIndex == 2 && isSortable_2 ? c.Description :
                sortColumnIndex == 3 && isSortable_3 ? c.IsActive.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.Project :
                sortColumnIndex == 5 && isSortable_5 ? c.VoucherType :
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
                , c.GLAccountCode 
                , c.Description 
                , Convert.ToString(c.IsActive == true ? "Active" : "Inactive") 
                , c.Project
                , c.VoucherType
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
               var permission= _reposur.SymRoleSession(identity.UserId, "1_35", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return PartialView();
        }
        [HttpPost]
        public ActionResult Create(GLAccountVM vm)
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
               var permission= _reposur.SymRoleSession(identity.UserId, "1_35", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return PartialView(_repo.SelectById(id));
        }
        [HttpPost]
        public ActionResult Edit(GLAccountVM vm, string btn)
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
        public ActionResult Delete(string ids)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_35", "delete").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            GLAccountVM vm = new GLAccountVM();
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
