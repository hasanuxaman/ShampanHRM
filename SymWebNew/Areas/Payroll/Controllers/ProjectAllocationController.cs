using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class ProjectAllocationController : Controller
    {
        //
        // GET: /Payroll/ProjectAllocation/
        #region Declare
        ProjectAllocationRepo _repo = new ProjectAllocationRepo();
				SymUserRoleRepo _reposur = new SymUserRoleRepo();
				ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        #endregion Declare
        public ActionResult Index()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_32", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param, string code, string name, string Id)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var NameFilter = Convert.ToString(Request["sSearch_2"]);
            var BranchNameFilter = Convert.ToString(Request["sSearch_3"]);
            #endregion Column Search
            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<ProjectAllocationVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            #region Column Filtering
            if (codeFilter != "" || NameFilter != "" || BranchNameFilter != "")
            {
                filteredData = filteredData
                                .Where(c =>
                                   (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    && (NameFilter == "" || c.Name.ToLower().Contains(NameFilter.ToLower()))
                                );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ProjectAllocationVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
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
                , c.Code //+ "~" + Convert.ToString(c.Id) 
                , c.Name 
            };
            return Json(new
            {   sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Create()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_32", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            ProjectAllocationVM vm = new ProjectAllocationVM();
            ProjectAllocationDetailVM vmd = new ProjectAllocationDetailVM();
            List<ProjectAllocationDetailVM> ProjectAllocationDetail = new List<ProjectAllocationDetailVM>();
            ProjectAllocationDetail.Add(new ProjectAllocationDetailVM() { HeadName = "Gross",Portion1=0,Portion2=0,Portion3=0,Portion4=0,GLCode1="NA",GLCode2="NA",GLCode3="NA",GLCode4="NA" });
            ProjectAllocationDetail.Add(new ProjectAllocationDetailVM() { HeadName = "Conveyance", Portion1 = 0, Portion2 = 0, Portion3 = 0, Portion4 = 0, GLCode1 = "NA", GLCode2 = "NA", GLCode3 = "NA", GLCode4 = "NA" });
            ProjectAllocationDetail.Add(new ProjectAllocationDetailVM() { HeadName = "Overtime", Portion1 = 0, Portion2 = 0, Portion3 = 0, Portion4 = 0, GLCode1 = "NA", GLCode2 = "NA", GLCode3 = "NA", GLCode4 = "NA" });
            ProjectAllocationDetail.Add(new ProjectAllocationDetailVM() { HeadName = "Other Earning", Portion1 = 0, Portion2 = 0, Portion3 = 0, Portion4 = 0, GLCode1 = "NA", GLCode2 = "NA", GLCode3 = "NA", GLCode4 = "NA" });
            ProjectAllocationDetail.Add(new ProjectAllocationDetailVM() { HeadName = "Bonus", Portion1 = 0, Portion2 = 0, Portion3 = 0, Portion4 = 0, GLCode1 = "NA", GLCode2 = "NA", GLCode3 = "NA", GLCode4 = "NA" });
            ProjectAllocationDetail.Add(new ProjectAllocationDetailVM() { HeadName = "PF Employee", Portion1 = 0, Portion2 = 0, Portion3 = 0, Portion4 = 0, GLCode1 = "NA", GLCode2 = "NA", GLCode3 = "NA", GLCode4 = "NA" });
            ProjectAllocationDetail.Add(new ProjectAllocationDetailVM() { HeadName = "PF Employer", Portion1 = 0, Portion2 = 0, Portion3 = 0, Portion4 = 0, GLCode1 = "NA", GLCode2 = "NA", GLCode3 = "NA", GLCode4 = "NA" });
            ProjectAllocationDetail.Add(new ProjectAllocationDetailVM() { HeadName = "TAX", Portion1 = 0, Portion2 = 0, Portion3 = 0, Portion4 = 0, GLCode1 = "NA", GLCode2 = "NA", GLCode3 = "NA", GLCode4 = "NA" });
            ProjectAllocationDetail.Add(new ProjectAllocationDetailVM() { HeadName = "Loan Principle", Portion1 = 0, Portion2 = 0, Portion3 = 0, Portion4 = 0, GLCode1 = "NA", GLCode2 = "NA", GLCode3 = "NA", GLCode4 = "NA" });
            ProjectAllocationDetail.Add(new ProjectAllocationDetailVM() { HeadName = "Loan Interest", Portion1 = 0, Portion2 = 0, Portion3 = 0, Portion4 = 0, GLCode1 = "NA", GLCode2 = "NA", GLCode3 = "NA", GLCode4 = "NA" });
            ProjectAllocationDetail.Add(new ProjectAllocationDetailVM() { HeadName = "Other Deduction", Portion1 = 0, Portion2 = 0, Portion3 = 0, Portion4 = 0, GLCode1 = "NA", GLCode2 = "NA", GLCode3 = "NA", GLCode4 = "NA" });
            vm.ProjectAllocationDetailVM = ProjectAllocationDetail;
            return View(vm);
        }
        [HttpPost]
        public ActionResult Create(ProjectAllocationVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.BranchId = identity.BranchId;
              result=  _repo.Insert(vm);
              if (result[1] == "Fail") {
                  throw new ArgumentNullException("Please Input Expected Project Allocation Value", "");
              }
                Session["result"] = result[0] + "~" + result[1];
                //return RedirectToAction("Index", new { id = vm .EmployeeId});
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(vm);
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
            }
            return View(vm);
        }
        public ActionResult Edit(string Id)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_32", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            ProjectAllocationVM vm = new ProjectAllocationVM();
            ProjectAllocationRepo proallRepo = new ProjectAllocationRepo();
            List<ProjectAllocationDetailVM> vmds = new List<ProjectAllocationDetailVM>();
            ProjectAllocationDetailRepo prodetail = new ProjectAllocationDetailRepo();
            //ProjectAllocationDetailRepo prodetail = new ProjectAllocationDetailRepo();
            vm = proallRepo.SelectById(Id);
            vm.ProjectAllocationDetailVM = prodetail.SelectByMasterId(vm.Id);
            return View("Create", vm);
        }
        [HttpPost]
        public ActionResult Edit(ProjectAllocationVM vm )
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] retResults = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {
                retResults = _repo.Update(vm);
                if (retResults[0] == "Fail")
                {
                     throw new ArgumentNullException("ProjectAllocation", "Fail~Data not unsuccessfully.");
                }
                Session["result"] = retResults[0] + "~" + retResults[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Create", vm);
            }
        }
        public ActionResult Delete(string ids)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_32", "delete").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            ProjectAllocationVM vm = new ProjectAllocationVM();
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
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
