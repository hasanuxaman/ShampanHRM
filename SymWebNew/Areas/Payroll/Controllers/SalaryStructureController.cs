using SymOrdinary;
using SymRepository.Common;
using SymRepository.Enum;
using SymRepository.HRM;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
namespace SymWebUI.Areas.Payroll.Controllers
{
    [Authorize]
    public class SalaryStructureController : Controller
    {
        //
        // GET: /Payroll/SalaryStructure/
				SymUserRoleRepo _reposur = new SymUserRoleRepo();
				ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        SalaryStructureRepo salaryStrurepo = new SalaryStructureRepo();
        public ActionResult Index()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_29", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View();
        }
        public ActionResult _SalaryStructure(JQueryDataTableParamVM param, string code, string name)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var nameFilter = Convert.ToString(Request["sSearch_2"]);
            var isActiveFilter = Convert.ToString(Request["sSearch_3"]);
            var remarksFilter = Convert.ToString(Request["sSearch_4"]);
            //Name
            //IsActive
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
            var getAllData = salaryStrurepo.SelectAll();
            IEnumerable<SalaryStructureVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if ( codeFilter != "" || nameFilter != "" || isActiveFilter != "" || remarksFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                            &&
                                            (nameFilter == "" || c.Name.ToLower().Contains(nameFilter.ToLower()))
                                            &&
                                            (isActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(isActiveFilter1.ToLower()))
                                            &&
                                           (remarksFilter == "" || c.Remarks.ToString().ToLower().Contains(remarksFilter.ToLower()))
                                           );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalaryStructureVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code : 
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
                sortColumnIndex == 3 && isSortable_3 ? c.IsActive.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.Remarks :
                                                           "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { 
                Convert.ToString(c.Id)
                , c.Code 
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
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_29", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            SalaryStructureVM vm = new SalaryStructureVM();
            SalaryStructureDetailVM salaryStructureDetailVM = new SalaryStructureDetailVM();
            List<SalaryStructureDetailVM> salaryStructureDetailVMs = new List<SalaryStructureDetailVM>();
            List<SalaryStructureDetailVM> salaryStructureDeductionDetailVMs = new List<SalaryStructureDetailVM>();
            EnumSalaryTypeRepo estr = new EnumSalaryTypeRepo();
            List<EnumSalaryTypeVM> tts = estr.SelectAll(Convert.ToInt32(identity.BranchId)).Where(x => x.IsGross == true).ToList();
            foreach (EnumSalaryTypeVM item in tts)
            {
                salaryStructureDetailVM = new SalaryStructureDetailVM();
                salaryStructureDetailVM.SalaryTypeId = item.Id;
                salaryStructureDetailVM.IsGross = item.IsGross;
                salaryStructureDetailVMs.Add(salaryStructureDetailVM);
            }
            vm.salaryStructureDetailVMs = salaryStructureDetailVMs;
            vm.salaryStructureDeductionDetailVMs = salaryStructureDeductionDetailVMs;
            return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(SalaryStructureVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            try
            {
                result = salaryStrurepo.Insert(vm);
                if (result[0]=="Fail")
                {
                    //if (result[1]=="This Salary Structure already exist")
                    //{
                    //    ViewBag.Fail = "This Salary Structure already exist";
                    //}
                    //else
                    //{
                    //    ViewBag.Fail = "Data Not Save Succeessfully";
                    //}
                    ViewBag.Fail = result[1].ToString();
                    return RedirectToAction("Create");
                }
                else
                {
                    ViewBag.Success = "Data saved successfully.";
                }
            }
            catch (Exception)
            {
                ViewBag.Fail = "Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Create");
            }
            return RedirectToAction("Index");
            //return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string Id)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_29", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            SalaryStructureVM vm = salaryStrurepo.SelectById(Id);
            return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(SalaryStructureVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            try
            {
                result = salaryStrurepo.Update(vm);
                Session["result"] = result[0]+"~"+result[1];
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
            }
            //return RedirectToAction("Index");
            return RedirectToAction("Edit", new {Id = vm.Id });
            //return View(vm);
        }
        [Authorize]
        [OutputCache(Duration = 86400)]
        public ViewResult SalaryStructureDetails()
        {
            var SalaryStructureDetailvm = new SalaryStructureDetailVM();
            return View("_SalaryStructureDetail", SalaryStructureDetailvm);
        }
        [OutputCache(Duration = 86400)]
        public ViewResult SalaryStructureDeductionDetails()
        {
            var SalaryStructureDeductionDetailvm = new SalaryStructureDetailVM();
            return View("_SalaryStructureDeductionDetail", SalaryStructureDeductionDetailvm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult Delete(string ids)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_29", "delete").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            SalaryStructureVM vm = new SalaryStructureVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = salaryStrurepo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
