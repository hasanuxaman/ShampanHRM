using SymOrdinary;
using SymRepository.Enum;
using SymViewModel.Common;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using SymRepository.Common;

namespace SymWebUI.Areas.Common.Controllers
{
    public class JobCircularController : Controller
    {
        //
        // GET: /Enum/JobCircular/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        JobCircularRepo _repo = new JobCircularRepo();
        #region Actions
        public ActionResult Index()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_17", "index").ToString();
            var getAllData = _repo.SelectAll();

            return View(getAllData);
        }
        public ActionResult _index(JQueryDataTableParamVM param)
        {
             #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var NameFilter = Convert.ToString(Request["sSearch_1"]);
            var IsActiveFilter = Convert.ToString(Request["sSearch_2"]);
            var remarksFilter = Convert.ToString(Request["sSearch_3"]);
            var IsActiveFilter1 = IsActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();
            #endregion Column Search

            var getAllData = _repo.SelectAll();
            IEnumerable<JobCircularVM> filteredData;

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
                   .Where(c => isSearchable1 && c.Id.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.JobTitle.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.DesignationName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Expriance.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Deadline.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Description.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            //#region Column Filtering
            //if (NameFilter != "" || IsActiveFilter != "" || remarksFilter != "")   

            //{
            //    filteredData = filteredData
            //                    .Where(c =>
            //                               (NameFilter == "" || c.Name.ToLower().Contains(NameFilter.ToLower()))
            //                               &&
            //                               (IsActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(IsActiveFilter1.ToLower()))
            //                                &&
            //                               (remarksFilter == "" || c.Remarks.ToString().ToLower().Contains(remarksFilter.ToLower()))
            //                               );
            //}

            //#endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<JobCircularVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Id :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.JobTitle :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.DesignationName :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Expriance :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Deadline :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Description :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.IsActive.ToString() :
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
                             , c.JobTitle
                             , c.DesignationName
                             , c.Expriance
                             , c.Deadline
                             , c.Description
                         
                             , Convert.ToString(c.IsActive == true ? "Active" : "Inactive") 
                       
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
        public ActionResult Create(JobCircularVM vm)
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
        public ActionResult Edit(string id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_17", "edit").ToString();
            return PartialView(_repo.SelectById(id));
        }

        [HttpPost]
        public ActionResult Edit(JobCircularVM vm, string btn)
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
        //    JobCircularVM vm = new JobCircularVM();
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
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_17", "delete").ToString();
            JobCircularVM vm = new JobCircularVM();
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
