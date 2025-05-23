﻿using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.GF;
using SymRepository.Common;
using SymViewModel.GF;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymWebUI.Areas.PF.Models;


namespace SymWebUI.Areas.GF.Controllers
{
    public class GFPolicyController : Controller
    {
        public GFPolicyController()
        {
            ViewBag.TransType = AreaTypeGFVM.TransType;
        }
        //
        // GET: /GF/GFPolicy/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        GFPolicyRepo _repo = new GFPolicyRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/GF/Home");
            }
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id 
            //01     //PolicyName      
            //02     //JobDurationYearFrom       
            //03     //JobDurationYearTo       
            //04     //LastBasicMultipication
            //05     //LastBasicMultipication
            //06     //Remarks



            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<GFPolicyVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6= Convert.ToBoolean(Request["bSearchable_6"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.PolicyName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.JobDurationYearFrom.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.JobDurationYearTo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.MultipicationFactor.ToString().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.LastBasicMultipication.ToString().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.Remarks.ToString().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<GFPolicyVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.PolicyName       :
                sortColumnIndex == 2 && isSortable_2 ? c.JobDurationYearFrom.ToString()     :
                sortColumnIndex == 3 && isSortable_3 ? c.JobDurationYearTo.ToString()     :
                sortColumnIndex == 4 && isSortable_4 ? c.MultipicationFactor.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.LastBasicMultipication.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.Remarks :
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
                , c.PolicyName       
                , c.JobDurationYearFrom.ToString()
                , c.JobDurationYearTo.ToString()
                , c.MultipicationFactor.ToString()
                , c.LastBasicMultipication.ToString()
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            GFPolicyVM vm = new GFPolicyVM();
            vm.Operation = "add";
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(GFPolicyVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Index");
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            GFPolicyVM vm = new GFPolicyVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            GFPolicyVM vm = new GFPolicyVM();
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
