using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Controllers
{
    public class RecruitmentController : Controller
    {
        //
        // GET: /Recruitment/
        JobCircularRepo _repo = new JobCircularRepo();
        PublicApplicantRepo _PArepo = new PublicApplicantRepo();

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
           public ActionResult JobCircular(string returnUrl)
        {
            return View();
        }
         public ActionResult _JobCircularindex(JQueryDataTableParamVM param, string code, string name)
        {
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

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Id.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.JobTitle.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.DesignationName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Expriance.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Deadline.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Description.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
          
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<JobCircularVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Id :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.JobTitle :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.DesignationName :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Expriance :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Deadline :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Description :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies 
                         select new[] { 
                              c.JobTitle
                             , c.DesignationName
                             , c.Expriance
                             , c.Deadline
                             , c.Description,
                             Convert.ToString(c.Id)

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
        public ActionResult ApplyCreate(string Id)
        {
            PublicApplicantVM vm=new PublicApplicantVM();
            vm.JobCircularId=Id;
                return View(vm);
            
         }
          [HttpPost]
        public ActionResult ApplyCreate( PublicApplicantVM vm,HttpPostedFileBase file)
        {
            string[] result = new string[6];
              if(file.FileName != null && file.ContentLength > 0){
               string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\RecruitmentCV\\" + vm.file.FileName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                if (vm.file != null && vm.file.ContentLength > 0)
                {
                    vm.file.SaveAs(fullPath);
                }
            vm.CVName =vm.file.FileName;
              }
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = "Admin";
            vm.CreatedFrom = "100";
            try
            {
                result = _PArepo.Insert(vm);
                Session["result"] = result[0] + "~" + result[1];
                return View();
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View();
            }
        }
        [HttpGet]
        public ActionResult ApplyEdit(string id)
        {
            return PartialView(_repo.SelectById(id));
        }

        [HttpPost]
        public ActionResult ApplyEdit(PublicApplicantVM vm, string btn)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {
                
                result = _PArepo.Update(vm);
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

    }
}
