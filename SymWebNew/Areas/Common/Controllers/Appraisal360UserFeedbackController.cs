using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
namespace SymWebUI.Areas.Common.Controllers
{
    [Authorize]
    public class Appraisal360UserFeedbackController : Controller
    {


        Appraisal360UserFeedbackRepo _repo = new Appraisal360UserFeedbackRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        Appraisal360UserFeedbackRepo appraisal360UserFeedbackRepo;

        public ActionResult Index(Appraisal360FeedBackVM vm)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_8", "index").ToString();
            return View(vm);
        }

        public ActionResult _index(JQueryDataTableParamModel param, Appraisal360FeedBackVM vm)
        {
            appraisal360UserFeedbackRepo = new Appraisal360UserFeedbackRepo();

            #region Column Search

            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var questionFilter = Convert.ToString(Request["sSearch_1"]);
            var departmentFilter = Convert.ToString(Request["sSearch_2"]);


            var fromID = 0;
            var toID = 0;
            if (idFilter.Contains('~'))
            {
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }

            #endregion Column Search

            #region Search and Filter Data

            if (!identity.IsAdmin)
            {
                vm.FeedBackUserId = identity.UserId;
            }
            var getAllData = appraisal360UserFeedbackRepo.SelectAll(vm);
            IEnumerable<Appraisal360FeedBackVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData.Where(c =>
                    isSearchable1 && c.FeedBackYear.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.UserCode.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.UserName.ToLower().Contains(param.sSearch.ToLower())

                );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering

            if (questionFilter != "")
            {
                filteredData = filteredData
                    .Where(c => (departmentFilter == "" || c.DepartmentId.ToLower().Contains(departmentFilter.ToLower())
                            &&
                            (questionFilter == "" || c.DepartmentName.ToLower().Contains(questionFilter.ToLower()))
                        )
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
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_10"]);


            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Appraisal360FeedBackVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.DepartmentId :
                sortColumnIndex == 2 && isSortable_2 ? c.DepartmentName :
                sortColumnIndex == 3 && isSortable_3 ? Convert.ToString(c.UserId) :
                sortColumnIndex == 4 && isSortable_4 ? Convert.ToString(c.UserCode) :
                sortColumnIndex == 5 && isSortable_5 ? Convert.ToString(c.FeedbackBy) :
                sortColumnIndex == 6 && isSortable_6 ? Convert.ToString(c.FeedBackMonth) :
                sortColumnIndex == 7 && isSortable_7 ? Convert.ToString(c.FeedbackName) :

                sortColumnIndex == 8 && isSortable_8 ? Convert.ToString(c.FeedBackYear) :
                sortColumnIndex == 9 && isSortable_9 ? Convert.ToString(c.PeriodName) :

                "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[]
                {
                    Convert.ToString(c.Appraisal360Id), c.FeedBackYear, c.PeriodName, c.DepartmentName, c.UserCode,
                    c.UserName, c.FeedbackCode, c.FeedbackName, c.FeedbackBy

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
        public ActionResult Create(Appraisal360FeedBackVM vm)
        {
            try
            {
                appraisal360UserFeedbackRepo = new Appraisal360UserFeedbackRepo();

                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "add").ToString();

                vm.Appraisal360Id = vm.Id.ToString();
                var getAllData = appraisal360UserFeedbackRepo.SelectAll(vm);
                vm = getAllData.FirstOrDefault();

                List<Appraisal360DetailVM> detailVMs = new List<Appraisal360DetailVM>();
                detailVMs = appraisal360UserFeedbackRepo.SelectAllList(vm);
                vm.Details = detailVMs;

                return View(vm);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
        }


        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult CreateEdit(Appraisal360FeedBackVM vm)
        {
            appraisal360UserFeedbackRepo = new Appraisal360UserFeedbackRepo();
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                result = appraisal360UserFeedbackRepo.Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Create", new { Id = vm.Id });
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(vm);
            }
        }


        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult CompletedFeedBack(Appraisal360FeedBackVM vm)
        {
            appraisal360UserFeedbackRepo = new Appraisal360UserFeedbackRepo();
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                result = appraisal360UserFeedbackRepo.CompletedFeedBack(vm);
                Session["result"] = result[0] + "~" + result[1];
                return Json(result);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }


    }
}
