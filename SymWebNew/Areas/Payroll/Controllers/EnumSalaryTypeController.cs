using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SymRepository.Enum;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymOrdinary;
using System.Threading;
using SymViewModel.Payroll;
using SymRepository.Payroll;
using SymRepository.Common;
namespace SymWebUI.Areas.Payroll.Controllers
{
    [Authorize]
    public class EnumSalaryTypeController : Controller
    {
        //
        // GET: /Enum/EnumSalaryType/
				SymUserRoleRepo _reposur = new SymUserRoleRepo();
				ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        EnumSalaryTypeRepo _repo = new EnumSalaryTypeRepo();
        #region Actions
        public ActionResult Index()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_34", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View();
        }
        public ActionResult _indexSalaryHead(JQueryDataTableParamVM param, string code, string name)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var CodeFilter = Convert.ToString(Request["sSearch_1"]);
            var NameFilter = Convert.ToString(Request["sSearch_2"]);
            var IsEarningFilter = Convert.ToString(Request["sSearch_3"]);
            var GLAccountCodeFilter = Convert.ToString(Request["sSearch_4"]);
            var IsActiveFilter = Convert.ToString(Request["sSearch_5"]);
           ///Code 
            ///Name
            ///IsEarning
            ///GLAccountCode
            ///IsActive
            var IsEarningFilter1 = IsEarningFilter.ToLower() == "earning" ? true.ToString() : false.ToString();
            var IsActiveFilter1 = IsActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();
            #endregion Column Search
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var getAllData = _repo.SelectAllOthers(Convert.ToInt32(identity.BranchId));
            IEnumerable<EnumSalaryTypeVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable3 && c.IsEarning.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.GLAccountCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable5 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (CodeFilter != "" || NameFilter != "" || IsEarningFilter != "" || GLAccountCodeFilter != "" || IsActiveFilter != "")
            {
                filteredData = filteredData
                                .Where(c =>
                                           (CodeFilter == "" || c.Code.ToLower().Contains(CodeFilter.ToLower()))
                                           &&
                                           (NameFilter == "" || c.Name.ToLower().Contains(NameFilter.ToLower()))
                                           &&
                                           (IsEarningFilter == "" || c.IsEarning.ToString().ToLower().Contains(IsEarningFilter1.ToLower()))
                                           &&
                                           (GLAccountCodeFilter == "" || c.GLAccountCode.ToString().ToLower().Contains(GLAccountCodeFilter.ToLower()))
                                           &&
                                           (IsActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(IsActiveFilter1.ToLower()))
                                            );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EnumSalaryTypeVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Name :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.IsEarning.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.GLAccountCode.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.IsActive.ToString() :
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
                             , Convert.ToString(c.IsEarning == true ? "Earning" : "Deduction") 
                             , c.GLAccountCode
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
        public ActionResult _indexSalaryHeadPrinciple(JQueryDataTableParamVM param)
        {
            #region Column Search
            var CodeFilter = Convert.ToString(Request["sSearch_0"]);
            var NameFilter = Convert.ToString(Request["sSearch_1"]);
            var IsEarningFilter = Convert.ToString(Request["sSearch_2"]);
            var GLAccountCodeFilter = Convert.ToString(Request["sSearch_3"]);
            var IsActiveFilter = Convert.ToString(Request["sSearch_4"]);
            var IsEarningFilter1 = IsEarningFilter.ToLower() == "earning" ? true.ToString() : false.ToString();
            var IsActiveFilter1 = IsActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();
            #endregion Column Search
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var getAllData = _repo.SelectAllPrinciple(Convert.ToInt32(identity.BranchId));
            IEnumerable<EnumSalaryTypeVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable0 = Convert.ToBoolean(Request["bSearchable_0"]);
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable0 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                       );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (CodeFilter != "" || NameFilter != "" || IsEarningFilter != "" || GLAccountCodeFilter != "" || IsActiveFilter != "")
            {
                filteredData = filteredData
                                .Where(c =>
                                           (CodeFilter == "" || c.Code.ToLower().Contains(CodeFilter.ToLower()))
                                           &&
                                           (NameFilter == "" || c.Name.ToLower().Contains(NameFilter.ToLower()))
                                          &&
                                           (IsEarningFilter == "" || c.IsEarning.ToString().ToLower().Contains(IsEarningFilter1.ToLower()))
                                           &&
                                           (GLAccountCodeFilter == "" || c.GLAccountCode.ToString().ToLower().Contains(GLAccountCodeFilter.ToLower()))
                                            &&
                                           (IsActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(IsActiveFilter1.ToLower()))
                                            );
            }
            #endregion Column Filtering
            var isSortable_0 = Convert.ToBoolean(Request["bSortable_0"]);
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EnumSalaryTypeVM, string> orderingFunction = (c => 
                sortColumnIndex == 0 && isSortable_0 ? c.Code :
                sortColumnIndex == 1 && isSortable_1 ? c.Name :
                sortColumnIndex == 2 && isSortable_2 ? c.IsEarning.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.GLAccountCode :
                sortColumnIndex == 4 && isSortable_4 ? c.IsActive.ToString() :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                             c.Code 
                             , c.Name
                             , Convert.ToString(c.IsEarning == true ? "Earning" : "Deduction") 
                             , c.GLAccountCode
                             , Convert.ToString(c.IsActive == true ? "Active" : "Inactive") 
                             , Convert.ToString(c.Id)
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
        public ActionResult _indexSalaryHeadOther(JQueryDataTableParamVM param)
        {
            #region Column Search
            var NameFilter = Convert.ToString(Request["sSearch_0"]);
            var IsEarningFilter = Convert.ToString(Request["sSearch_1"]);
            var GLAccountCodeFilter = Convert.ToString(Request["sSearch_2"]);
            var IsActiveFilter = Convert.ToString(Request["sSearch_3"]);
            var IsEarningFilter1 = IsEarningFilter.ToLower() == "earning" ? true.ToString() : false.ToString();
            var IsActiveFilter1 = IsActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();
            #endregion Column Search
            EarningDeductionTypeRepo _edRepo = new EarningDeductionTypeRepo();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var getAllData = _edRepo.SelectAll();
            IEnumerable<EarningDeductionTypeVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable0 = Convert.ToBoolean(Request["bSearchable_0"]);
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                filteredData = getAllData
                   .Where(c => isSearchable0 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable1 && c.IsEarning.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.GLAccountCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable3 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (NameFilter != "" || IsEarningFilter != "" || GLAccountCodeFilter != "" || IsActiveFilter != "")
            {
                filteredData = filteredData
                                .Where(c =>
                                           (NameFilter == "" || c.Name.ToLower().Contains(NameFilter.ToLower()))
                                          &&
                                           (IsEarningFilter == "" || c.IsEarning.ToString().ToLower().Contains(IsEarningFilter1.ToLower()))
                                           &&
                                           (GLAccountCodeFilter == "" || c.GLAccountCode.ToString().ToLower().Contains(GLAccountCodeFilter.ToLower()))
                                            &&
                                           (IsActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(IsActiveFilter1.ToLower()))
                                           );
            }
            #endregion Column Filtering
            var isSortable_0 = Convert.ToBoolean(Request["bSortable_0"]);
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EarningDeductionTypeVM, string> orderingFunction = (c => sortColumnIndex == 0 && isSortable_0 ? c.Name :
                                                           sortColumnIndex == 1 && isSortable_1 ? c.IsEarning.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.GLAccountCode.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.IsActive.ToString() :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                             c.Name
                             , Convert.ToString(c.IsEarning == true ? "Earning" : "Deduction") 
                             , c.GLAccountCode
                             , Convert.ToString(c.IsActive == true ? "Active" : "Inactive") 
                             , Convert.ToString(c.Id)
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
               var permission= _reposur.SymRoleSession(identity.UserId, "1_34", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return PartialView();
        }
        [HttpPost]
        public ActionResult Create(EnumSalaryTypeVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name.Trim();
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
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
               var permission= _reposur.SymRoleSession(identity.UserId, "1_34", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            EnumSalaryTypeVM vm = _repo.SelectById(id);
            return PartialView("Edit",vm);
        }
        [HttpGet]
        public ActionResult EditGLPrinciple(string Id)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_34", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            List<EnumSalaryTypeVM> VMs = _repo.SelectAllPrinciple(Convert.ToInt32(identity.BranchId), Id);
            EnumSalaryTypeVM vm = new EnumSalaryTypeVM();
            vm = VMs.FirstOrDefault();
            return PartialView("EditGLPrinciple", vm);
        }
        [HttpGet]
        public ActionResult EditGLOther(string Id)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_34", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            EarningDeductionTypeRepo _edRepo = new EarningDeductionTypeRepo();
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EarningDeductionTypeVM vm = _edRepo.SelectById(Convert.ToInt32(Id));
            return PartialView("EditGLOther", vm);
        }
        [HttpPost]
        public ActionResult Edit(EnumSalaryTypeVM vm, string btn)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
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
        public ActionResult EditPrinciple(string Id, string GLAccountCode)
        {
            string[] result = new string[6];
            try
            {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_34", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
                result = _repo.EditPrinciple( Id,  GLAccountCode);
                return Json(result, JsonRequestBehavior.AllowGet);
                //Session["result"] = result[0] + "~" + result[1];
                //return View("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Updated";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        public ActionResult EditOther(string Id, string GLAccountCode)
        {
            string[] result = new string[6];
            try
            {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_34", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
                EarningDeductionTypeRepo _edRepo = new EarningDeductionTypeRepo();
                result = _edRepo.EditOther(Id, GLAccountCode);
                return Json(result, JsonRequestBehavior.AllowGet);
                //Session["result"] = result[0] + "~" + result[1];
                //return View("Index");
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
               var permission= _reposur.SymRoleSession(identity.UserId, "1_34", "delete").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            EnumSalaryTypeVM vm = new EnumSalaryTypeVM();
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        #endregion Actions
    }
}
