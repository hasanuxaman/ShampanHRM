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
    public class Appraisal360Controller : Controller
    {
        //
        // GET: /Common/AppraisalQuestions/

        AppraisalQuestionsRepo _repo = new AppraisalQuestionsRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        SymUserRoleRepo _repoSymUser = new SymUserRoleRepo();
        Appraisal360Repo _app360rep = new Appraisal360Repo();
        Appraisal360DetailsRepo _app360Detailrepo = new Appraisal360DetailsRepo();
        SymUserAppraisal360repo _app360 = new SymUserAppraisal360repo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        AppraisalQuestionsRepo appraisalQuestionsRepo;
        public ActionResult Index()
        {
            var permission = _repoSymUser.SymRoleSession(identity.UserId, "1_42", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Appraisal360");
            }

            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            _app360rep = new Appraisal360Repo();

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var questionFilter = Convert.ToString(Request["sSearch_1"]);
            var PeriodNameFilter = Convert.ToString(Request["sSearch_2"]);


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

            var getAllData = _app360rep.SelectAll();
            IEnumerable<ViewAppraisal360VM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData.Where(c =>
                        isSearchable1 && c.PeriodName.ToLower().Contains(param.sSearch.ToLower())

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
                                .Where(c => (PeriodNameFilter == "" || c.PeriodName.ToLower().Contains(PeriodNameFilter.ToLower())

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
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_9"]);
            var isSortable_10 = Convert.ToBoolean(Request["bSortable_10"]);
            var isSortable_11 = Convert.ToBoolean(Request["bSortable_11"]);
            var isSortable_12 = Convert.ToBoolean(Request["bSortable_12"]);
            var isSortable_13 = Convert.ToBoolean(Request["bSortable_13"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ViewAppraisal360VM, string> orderingFunction = (c =>
                 sortColumnIndex == 1 && isSortable_1 ? Convert.ToString(c.FeedBackYear) :
                 sortColumnIndex == 2 && isSortable_2 ? Convert.ToString(c.PeriodName) :
                 sortColumnIndex == 3 && isSortable_3 ? Convert.ToString(c.FeedbackBy) :
                 sortColumnIndex == 4 && isSortable_4 ? Convert.ToString(c.DepartmentName) :
                 sortColumnIndex == 5 && isSortable_5 ? Convert.ToString(c.UserCode) :
                 sortColumnIndex == 6 && isSortable_6 ? Convert.ToString(c.UserName) :
                 sortColumnIndex == 7 && isSortable_7 ? Convert.ToString(c.FeedbackCode) :
                 sortColumnIndex == 8 && isSortable_8 ? Convert.ToString(c.FeedbackName) :
                 sortColumnIndex == 9 && isSortable_9 ? Convert.ToString(c.IsFeedbackCompeted) :
                 sortColumnIndex == 10 && isSortable_10 ? Convert.ToString(c.UserId) :
                 sortColumnIndex == 11 && isSortable_11 ? Convert.ToString(c.FeedBackUserId) :
                 sortColumnIndex == 12 && isSortable_12 ? Convert.ToString(c.FeedBackMonth) :
                 sortColumnIndex == 13 && isSortable_13 ? Convert.ToString(c.Appraisal360Id) :

                "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 

                 Convert.ToString(c.FeedBackYear)
                      ,c.PeriodName
                      ,c.FeedbackBy
                      ,c.DepartmentName
                      ,c.UserCode
                      ,c.UserName
                      ,c.FeedbackCode
                      ,c.FeedbackName

                ,Convert.ToString(c.IsFeedbackCompeted)
                ,c.UserId
                ,c.FeedBackUserId
                ,Convert.ToString( c.FeedBackMonth)
                ,Convert.ToString( c.Appraisal360Id)
                
                
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
        public ActionResult IndexAppraisal360()
        {
            var permission = _repoSymUser.SymRoleSession(identity.UserId, "1_42", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Appraisal360");
            }

            return View();
        }
        public ActionResult _IndexAppraisal360(JQueryDataTableParamModel param)
        {
            _app360rep = new Appraisal360Repo();

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var departmentIdFilter = Convert.ToString(Request["sSearch_1"]);
            var userNameFilter = Convert.ToString(Request["sSearch_2"]);

            var fromID = 0;
            var toID = 0;
            if (idFilter != null && idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }
            #endregion Column Search

            #region Search and Filter Data

            var getAllData = _app360rep.SelectAllAppraisal360();
            IEnumerable<Appraisal360VM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData.Where(c =>
                        isSearchable1 && c.DepartmentId.ToLower().Contains(param.sSearch.ToLower())
                        || isSearchable1 && c.UserName.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (departmentIdFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (departmentIdFilter == "" || c.DepartmentId.ToLower().Contains(departmentIdFilter.ToLower())
                                     &&
                                    (userNameFilter == "" || c.UserName.ToLower().Contains(userNameFilter.ToLower()))
                                   )
                                   );
            }

            #endregion Column Filtering

            //var isSortable_1  = Convert.ToBoolean(Request["bSortable_1 "]);
            var isSortable_1  = Convert.ToBoolean(Request["bSortable_1 "]);
            var isSortable_2  = Convert.ToBoolean(Request["bSortable_2 "]);
            var isSortable_3  = Convert.ToBoolean(Request["bSortable_3 "]);
            var isSortable_4  = Convert.ToBoolean(Request["bSortable_4 "]);
            var isSortable_5  = Convert.ToBoolean(Request["bSortable_5 "]);
            var isSortable_6  = Convert.ToBoolean(Request["bSortable_6 "]);
            var isSortable_7  = Convert.ToBoolean(Request["bSortable_7 "]);
            var isSortable_8  = Convert.ToBoolean(Request["bSortable_8 "]);
            var isSortable_9  = Convert.ToBoolean(Request["bSortable_9 "]);
            var isSortable_10 = Convert.ToBoolean(Request["bSortable_10"]);
            var isSortable_11 = Convert.ToBoolean(Request["bSortable_11"]);
            var isSortable_12 = Convert.ToBoolean(Request["bSortable_12"]);
            //var isSortable_13 = Convert.ToBoolean(Request["bSortable_13"]);
            //var isSortable_14 = Convert.ToBoolean(Request["bSortable_14"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Appraisal360VM, string> orderingFunction = (c =>
                //sortColumnIndex == 1 && isSortable_1 ? Convert.ToString(c.DepartmentId) :  appraisal360VM.FeedbackDate.ToString("MM/dd/yyyy");
                 sortColumnIndex == 1 && isSortable_1 ? Convert.ToString(c.DepartmentName) :
                 //sortColumnIndex == 2 && isSortable_2 ? Convert.ToString(c.UserId) :
                 sortColumnIndex == 2 && isSortable_2 ? Convert.ToString(c.UserName) :
                 //sortColumnIndex == 4 && isSortable_4 ? Convert.ToString(c.FeedBackUserId) :
                 sortColumnIndex == 3 && isSortable_3 ? Convert.ToString(c.FeedBackUserName) :
                 sortColumnIndex == 4 && isSortable_4 ? (c.FeedbackDate).ToString("MM/dd/yyyy") :
                 sortColumnIndex == 5 && isSortable_5 ? Convert.ToString(c.FeedBackYear) :
                 sortColumnIndex == 6 && isSortable_6 ? Convert.ToString(c.PeriodName) :
                 sortColumnIndex == 7 && isSortable_7 ? Convert.ToString(c.IsFeedbackCompeted) :
                 sortColumnIndex == 8 && isSortable_8 ? Convert.ToString(c.IsUser) :
                 sortColumnIndex == 9 && isSortable_9 ? Convert.ToString(c.IsSupervisor) :
                 sortColumnIndex == 10 && isSortable_10 ? Convert.ToString(c.IsDepartmentHead) :
                 sortColumnIndex == 11 && isSortable_11 ? Convert.ToString(c.IsManagement) :
                 sortColumnIndex == 12 && isSortable_12 ? Convert.ToString(c.IsHR) :

                "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
           
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                        c.Id.ToString()
                      //,c.DepartmentId
                      ,c.DepartmentName
                      //,c.UserId
                      ,c.UserName
                      //,c.FeedBackUserId
                      ,c.FeedBackUserName
                      ,(c.FeedbackDate).ToString("yyyy/MMM/dd")
                      ,Convert.ToString(c.FeedBackYear)
                      ,c.PeriodName
                      ,Convert.ToString(c.IsFeedbackCompeted)
                      ,Convert.ToString(c.IsUser)
                      ,Convert.ToString(c.IsSupervisor)
                      ,Convert.ToString(c.IsDepartmentHead)
                      ,Convert.ToString(c.IsManagement)
                      ,Convert.ToString(c.IsHR)

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
        public ActionResult IndexAppraisal360Details()
        {
            var permission = _repoSymUser.SymRoleSession(identity.UserId, "1_42", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Appraisal360");
            }

            return View();
        }
        public ActionResult _IndexAppraisal360Details(JQueryDataTableParamModel param)
        {
            _app360Detailrepo = new Appraisal360DetailsRepo();

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var departmentIdFilter = Convert.ToString(Request["sSearch_1"]);
            var PeriodNameFilter = Convert.ToString(Request["sSearch_2"]);

            var fromID = 0;
            var toID = 0;
            if (idFilter != null && idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }
            #endregion Column Search

            #region Search and Filter Data

            var getAllData = _app360Detailrepo.SelectAll();
            IEnumerable<Appraisal360DetailsVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData.Where(c =>
                        isSearchable1 && c.DepartmentId.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (departmentIdFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (departmentIdFilter == "" || c.DepartmentId.ToLower().Contains(departmentIdFilter.ToLower())

                                   )
                                     );
            }

            #endregion Column Filtering

            //var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1 "]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2 "]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3 "]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4 "]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5 "]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6 "]);
            //var isSortable_7 = Convert.ToBoolean(Request["bSortable_7 "]);


            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Appraisal360DetailsVM, string> orderingFunction = (c =>
                 //sortColumnIndex == 1 && isSortable_1 ? Convert.ToString(c.Appraisal360Id) :
                 sortColumnIndex == 1 && isSortable_1 ? Convert.ToString(c.DepartmentId) :
                 sortColumnIndex == 2 && isSortable_2 ? Convert.ToString(c.FeedBackYear) :
                 sortColumnIndex == 3 && isSortable_3 ? Convert.ToString(c.PeriodName) :
                 //sortColumnIndex == 4 && isSortable_4 ? Convert.ToString(c.QuestionId) :
                 sortColumnIndex == 4 && isSortable_4 ? Convert.ToString(c.Question) :
                 sortColumnIndex == 5 && isSortable_5 ? Convert.ToString(c.FeedbackValue) :
                 sortColumnIndex == 6 && isSortable_6 ? Convert.ToString(c.FeedbackUserType) :
                "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            //param.iDisplayLength = 100;
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                      
                             c.Id.ToString()
                            //,Convert.ToString( c.Appraisal360Id)
                             ,c.DepartmentId
                            , Convert.ToString( c.FeedBackYear)
                            ,  c.PeriodName
                            //, Convert.ToString( c.QuestionId)
                             ,c.Question
                            , Convert.ToString( c.FeedbackValue)
                            , Convert.ToString( c.FeedbackUserType)

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
       
        public ActionResult AppraisalDataProcess(string FiscalPeriodDetailId, string FYId, string DId)
        {
            try
            {
                //string[] result = new string[6];
                var permission = _app360rep.Appraisal360DataProcess(FiscalPeriodDetailId, FYId, DId).ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Common/Appraisal360");
                }
                string message = "Data Process Succesfully";
                var result = new { Success = "Success", Message = message };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result = new { Success = "Fail", Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }        
        }

        //==== Appraisal360 Create And Edit ===
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "add").ToString();
            return View();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(Appraisal360VM appraisal360)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            //BranchVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //BranchVM.CreatedBy = identity.Name;
            //BranchVM.CreatedFrom = identity.WorkStationIP;
            try
            {
                result = new Appraisal360Repo().Insert(appraisal360);
                Session["result"] = result[0] + "~" + result[1];
                //
                return RedirectToAction("Edit", new { @id = result[2] });
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(appraisal360);
            }
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "edit").ToString();
            Appraisal360VM vm = new Appraisal360VM();
            vm = _app360rep.SelectById(id);
            return View(vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(Appraisal360VM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            //vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //vm.LastUpdateBy = identity.Name;
            //vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {
                result = new Appraisal360Repo().Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                if (result[0] == "Fail")
                {
                    return View(vm);
                }
                //return RedirectToAction("Index");
                return RedirectToAction("Edit", new { @id = result[2] });
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(vm);
            }
        }

        //==== Appraisal360Details Create And Edit ===

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult CreateApp360Detail()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "add").ToString();
            return View();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult CreateApp360Detail(Appraisal360DetailsVM appraisal360Details)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            //BranchVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //BranchVM.CreatedBy = identity.Name;
            //BranchVM.CreatedFrom = identity.WorkStationIP;
            try
            {
                result = new Appraisal360DetailsRepo().Insert(appraisal360Details);
                Session["result"] = result[0] + "~" + result[1];
                //
                return RedirectToAction("Edit", new { @id = result[2] });
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(appraisal360Details);
            }
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult EditApp360Detail(int id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "edit").ToString();
            Appraisal360DetailsVM vm = new Appraisal360DetailsVM();
            vm = _app360Detailrepo.SelectById(id);
            return View(vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult EditApp360Detail(Appraisal360DetailsVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            //vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //vm.LastUpdateBy = identity.Name;
            //vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {
                result = new Appraisal360DetailsRepo().Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                if (result[0] == "Fail")
                {
                    return View(vm);
                }
                //return RedirectToAction("Index");
                return RedirectToAction("Edit", new { @id = result[2] });
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(vm);
            }
        }

        //======= Appraisal360Combination =======

        public ActionResult IndexAppraisal360Combination()
        {
            var permission = _repoSymUser.SymRoleSession(identity.UserId, "1_42", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Appraisal360");
            }

            return View();
        }
        public ActionResult _IndexAppraisal360Combination(JQueryDataTableParamModel param)
        {
            _app360Detailrepo = new Appraisal360DetailsRepo();

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var departmentIdFilter = Convert.ToString(Request["sSearch_1"]);
            var PeriodNameFilter = Convert.ToString(Request["sSearch_2"]);

            var fromID = 0;
            var toID = 0;
            if (idFilter != null && idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }
            #endregion Column Search

            #region Search and Filter Data

            var getAllData = _app360Detailrepo.SelectAll();
            IEnumerable<Appraisal360DetailsVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData.Where(c =>
                        isSearchable1 && c.DepartmentId.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (departmentIdFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (departmentIdFilter == "" || c.DepartmentId.ToLower().Contains(departmentIdFilter.ToLower())

                                   )
                                     );
            }

            #endregion Column Filtering

            //var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1 "]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2 "]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3 "]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4 "]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5 "]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6 "]);
            //var isSortable_7 = Convert.ToBoolean(Request["bSortable_7 "]);


            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Appraisal360DetailsVM, string> orderingFunction = (c =>
                //sortColumnIndex == 1 && isSortable_1 ? Convert.ToString(c.Appraisal360Id) :
                 sortColumnIndex == 1 && isSortable_1 ? Convert.ToString(c.DepartmentId) :
                 sortColumnIndex == 2 && isSortable_2 ? Convert.ToString(c.FeedBackYear) :
                 sortColumnIndex == 3 && isSortable_3 ? Convert.ToString(c.PeriodName) :
                     //sortColumnIndex == 4 && isSortable_4 ? Convert.ToString(c.QuestionId) :
                 sortColumnIndex == 4 && isSortable_4 ? Convert.ToString(c.Question) :
                 sortColumnIndex == 5 && isSortable_5 ? Convert.ToString(c.FeedbackValue) :
                 sortColumnIndex == 6 && isSortable_6 ? Convert.ToString(c.FeedbackUserType) :
                "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            //param.iDisplayLength = 100;
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                      
                             c.Id.ToString()
                            //,Convert.ToString( c.Appraisal360Id)
                             ,c.DepartmentId
                            , Convert.ToString( c.FeedBackYear)
                            ,  c.PeriodName
                            //, Convert.ToString( c.QuestionId)
                             ,c.Question
                            , Convert.ToString( c.FeedbackValue)
                            , Convert.ToString( c.FeedbackUserType)

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




    }
}
