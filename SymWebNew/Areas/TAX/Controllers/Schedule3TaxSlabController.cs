using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using SymViewModel.Common;
using SymRepository.Common;

namespace SymWebUI.Areas.TAX.Controllers
{
    public class Schedule3TaxSlabController : Controller
    {
        //
        // GET: /TAX/Schedule3Schedule3TaxSlab/

        Schedule3TaxSlabRepo _repo = new Schedule3TaxSlabRepo();
        Schedule3TaxSlabDetailRepo _detailRepo = new Schedule3TaxSlabDetailRepo();

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10010", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/TAX/Home");
            }

            Schedule3TaxSlabVM vm = new Schedule3TaxSlabVM();
            return View(vm);
        }
        public ActionResult _index(JQueryDataTableParamVM param)
        {

            #region Search and Filter Data
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var getAllData = _repo.SelectAll();
            IEnumerable<Schedule3TaxSlabVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.MaximumInvestment.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Schedule3TaxSlabVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.MaximumInvestment.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Remarks :
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
                , c.MaximumInvestment.ToString()
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
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "add").ToString();
            Schedule3TaxSlabVM vm = new Schedule3TaxSlabVM();
            List<Schedule3TaxSlabDetailVM> schedule3TaxSlabDVMs = new List<Schedule3TaxSlabDetailVM>();
            Schedule3TaxSlabDetailVM dVM = new Schedule3TaxSlabDetailVM();
            for (int i = 0; i < 6; i++)
            {
                dVM = new Schedule3TaxSlabDetailVM();
                dVM.SlabName = "Slab " + (i + 1);
                dVM.EarningCeilingMin = 0;
                dVM.EarningCeilingMax = 0;
                dVM.CeilingMin = 0;
                dVM.CeilingMax = 0;
                dVM.Ratio = 0;
                schedule3TaxSlabDVMs.Add(dVM);
            }
            schedule3TaxSlabDVMs.Last().SlabName = "Above";


            vm.schedule3TaxSlabDetailVMs = schedule3TaxSlabDVMs;
            vm.Operation = "add";
            return View(vm);
        }

        [HttpPost]
        public ActionResult CreateEdit(Schedule3TaxSlabVM vm)
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
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        vm.Id = 0;
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    result = _repo.Update(vm);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return View("Create", vm);
                    }
                }
                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data not Successfully";
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            Schedule3TaxSlabVM vm = new Schedule3TaxSlabVM();
            vm = _repo.SelectAll(Convert.ToInt32(id), null, null).FirstOrDefault();

            List<Schedule3TaxSlabDetailVM> schedule3TaxSlabDVMs = new List<Schedule3TaxSlabDetailVM>();
            schedule3TaxSlabDVMs = _detailRepo.SelectAll(0, Convert.ToInt32(id));
            vm.schedule3TaxSlabDetailVMs = schedule3TaxSlabDVMs;
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "delete").ToString();
            Schedule3TaxSlabVM vm = new Schedule3TaxSlabVM();
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
