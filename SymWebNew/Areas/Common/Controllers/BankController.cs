using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SymRepository.Common;
using SymViewModel.Common;
using SymOrdinary;
using System.Threading;
using SymViewModel.HRM;
using SymRepository.HRM;
using JQueryDataTables.Models;
namespace SymWebUI.Areas.Common.Controllers
{
    [Authorize]
    public class BankController : Controller
    {

        //
        // GET: /Common/Bank/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        BankRepo _bankRepo;
        public ActionResult Index()
        {
             Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "index").ToString();
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            _bankRepo = new BankRepo();

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var nameFilter = Convert.ToString(Request["sSearch_2"]);
            var isActiveFilter = Convert.ToString(Request["sSearch_3"]);
            var remarksFilter = Convert.ToString(Request["sSearch_4"]);

            var isActiveFilter1 = isActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();

            #endregion Column Search

            #region Search and Filter Data


            var getAllData = _bankRepo.SelectAll();
            IEnumerable<BankVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData.Where(c =>
                                   isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                                   || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                                   || isSearchable3 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   || isSearchable4 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                                   );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (codeFilter != "" || nameFilter != "" || isActiveFilter != "" || remarksFilter != "")
            {
                filteredData = filteredData.Where(c =>
                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                    && (nameFilter == "" || c.Name.ToLower().Contains(nameFilter.ToLower()))
                    && (isActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(isActiveFilter1.ToLower()))
                    && (remarksFilter == "" || c.Remarks.ToLower().Contains(remarksFilter.ToLower()))
                    );
            }

            #endregion Column Filtering


            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<BankVM, string> orderingFunction = (c =>
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
            var result = from c in displayedCompanies
                         select new[] { 
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
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "add").ToString();
            return PartialView();
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(BankVM vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            try
            {
                string[] result = new string[6];
                result = new BankRepo().Insert(vm);
                //Session["result"] = result[0] + "~" + result[1];
                ViewBag.Success = "Data Saved successfully.";
             //   return RedirectToAction("Index");
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fail = "Data Saved Fail.";
                //Session["result"] = "Fail~Data Not Succeessfully!";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "edit").ToString();

            return View(new BankRepo().SelectById(id));
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(BankVM vm)
        {
             string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {
               
                result = new BankRepo().Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult BankDelete(string ids)
        {
            BankVM vm = new BankVM();
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "delete").ToString();

            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = new BankRepo().Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult Test(string startDate, string endDate, string sidx, string sord, int page, int rows)
        {
            EmployeeInfoRepo _bankRepo = new EmployeeInfoRepo();
            //var logicLayer = _bankRepo.SelectAll();
            IEnumerable<EmployeeInfoVM> filteredData;

            //SalesLogic logicLayer = new SalesLogic();
            List<EmployeeInfoVM> context = new List<EmployeeInfoVM>();

            // If we aren't filtering by date, return this month's contributions

            if (string.IsNullOrWhiteSpace(startDate))
            {
                return View();
            }
            if (!string.IsNullOrWhiteSpace(startDate))
            {
                context = _bankRepo.SelectAll();
            }
            else // Filter by specified date range
            {
                //context = logicLayer.GetSalesByDateRange(startDate, endDate);
            }

            // Calculate page index, total pages, etc. for jqGrid to us for paging
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = context.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            // Order the results based on the order passed into the method
            string orderBy = string.Format("{0} {1}", sidx, sord);
            var sales = context.AsQueryable()
                               .OrderBy(x => x.Code)
                         .Skip(pageIndex * pageSize)
                         .Take(pageSize);

            // Format the data for the jqGrid
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                       from s in sales
                       select new
                       {
                           i = s.Id,
                           cell = new string[] {
                   s.Id.ToString(),
                   s.BranchId.ToString(),
                   s.Code,
                   s.PhotoName,
                   s.MiddleName, 
                  
                }
                       }).ToArray()
            };

            // Return the result in json
            return Json(jsonData);
        }
    }
}
