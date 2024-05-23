using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    [Authorize]
    public class JobController : Controller
    {
        //
        // GET: /HRM/Job/
        EmployeeTransferRepo _empTNRepo;
        EmployeePromotionRepo _empPRRepo;
        EmployeeJobHistoryRepo _empJHRepo;
        EmployeeInfoRepo _infoRepo;
        SymUserRoleRepo _reposur = new SymUserRoleRepo();

        public ActionResult Index(string id, string empcode, string btn)
        {
            string currentId = "";
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (empcode != null && btn != null)
            {
                vm = _infoRepo.SelectEmpForSearch(empcode, btn);
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    id = vm.Id;
                }
                else
                {
                    currentId = _infoRepo.DropDown(empcode).FirstOrDefault().Id;
                    id = currentId;
                }
            }

            if (!string.IsNullOrEmpty(Session["mgs"] as string))
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            if (!(identity.IsAdmin || identity.IsHRM))
            {
                id = identity.EmployeeId;
            }
            vm = _infoRepo.SelectById(id);
            vm.transferVM = new EmployeeTransferRepo().SelectByEmployeeCurrent(id);
            vm.transferVM.EmployeeId = id;
            vm.promotionVM = new EmployeePromotionRepo().SelectByEmployeeCurrent(id);
            vm.promotionVM.EmployeeId = id;
            vm.employeeJob = new EmployeeJobRepo().SelectByEmployee(id);
            vm.employeeJob.EmployeeId = id;

            #region Settings
            SettingRepo _settingRepo = new SettingRepo();
            string settingValue = _settingRepo.settingValue("EmployeeJob", "ProbationMonth");
            if (vm.employeeJob.ProbationMonth <= 0)
            {
                vm.employeeJob.ProbationMonth = Convert.ToInt32(settingValue); //settings;
            }

            settingValue = _settingRepo.settingValue("Holiday", "FirstHoliday");
            if (string.IsNullOrWhiteSpace(vm.employeeJob.FirstHoliday))
            {
                vm.employeeJob.FirstHoliday = settingValue;
            }

            settingValue = _settingRepo.settingValue("Holiday", "SecondHoliday");
            if (string.IsNullOrWhiteSpace(vm.employeeJob.SecondHoliday))
            {
                vm.employeeJob.SecondHoliday = settingValue;
            }
            //vm.employeeJob.LabelOther1 = _settingRepo.settingValue("Label", "Other1");
            //vm.employeeJob.LabelOther2 = _settingRepo.settingValue("Label", "Other2");
            //vm.employeeJob.LabelOther3 = _settingRepo.settingValue("Label", "Other3");


            #endregion Settings


            vm.employeeSG = new EmployeeStructureGroupRepo().SelectByEmployee(id);
            vm.employeeSG.EmployeeId = id;

            vm.Id = id;
            return View(vm);
        }
        #region Transfer
        public ActionResult _indexTN(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            _empTNRepo = new EmployeeTransferRepo();
            var getAllData = _empTNRepo.SelectAllByEmployee(Id);
            IEnumerable<EmployeeTransferVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.SectionName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.TransferDate.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.ProjectName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable5 && c.IsCurrent.ToString().ToLower().Contains(param.sSearch.ToLower()));
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeTransferVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.DepartmentName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.SectionName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.TransferDate :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.ProjectName :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.IsCurrent.ToString() :
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
                             , c.DepartmentName //+ "~" + Convert.ToString(c.Id)
                             , c.ProjectName
                             , c.SectionName
                             , c.TransferDate
                             , c.IsCurrent?"Current":"Previous" 
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
        //[Authorize(Roles = "Admin,Transfer")]
        [HttpGet]
        public ActionResult Transfer(string EmployeeId, string Id)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != "0")
            {
                vm.transferVM = new EmployeeTransferRepo().SelectById(Id);
            }
            else
            {
                vm.transferVM = new EmployeeTransferRepo().SelectByEmployeeCurrent(EmployeeId);
                vm.transferVM.EmployeeId = EmployeeId;
                vm.transferVM.Id = null;
                vm.transferVM.FileName = "";
            }
            return PartialView("_transfer", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Transfer(EmployeeInfoVM vm, HttpPostedFileBase TransferF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            string[] retResults = new string[6];
            EmployeeTransferRepo empTrnApp = new EmployeeTransferRepo();
            EmployeeTransferVM trnvm = new EmployeeTransferVM();
            trnvm = empTrnApp.SelectById(vm.transferVM.EmployeeId);

            if (vm.transferVM.ProjectId == trnvm.ProjectId && vm.transferVM.DepartmentId == trnvm.DepartmentId
                && vm.transferVM.SectionId == trnvm.SectionId)
            {

            }
            trnvm = vm.transferVM;

            if (TransferF != null && TransferF.ContentLength > 0)
            {
                trnvm.FileName = TransferF.FileName;
            }
            trnvm.BranchId = Ordinary.BranchId;
            if (trnvm.Id == null)
            {
                trnvm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                trnvm.CreatedBy = identity.Name;
                trnvm.CreatedFrom = identity.WorkStationIP;
                trnvm.BranchId = Convert.ToInt32(identity.BranchId);
                retResults = empTrnApp.Insert(trnvm);
            }
            else
            {
                trnvm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                trnvm.LastUpdateBy = identity.Name;
                trnvm.LastUpdateFrom = identity.WorkStationIP;
                retResults = empTrnApp.Update(trnvm);
            }
            if (TransferF != null && TransferF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Transfer"), retResults[2] + TransferF.FileName);
                TransferF.SaveAs(path);
            }
            Session["mgs"] = "mgs";
            var mgs = retResults[0] + "~" + retResults[1];
            //// return PartialView("_editEducation", vm);
            //return Json(mgs, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index", new { Id = trnvm.EmployeeId, mgs = mgs });
        }

        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult TransferDelete(string ids)
        {
            EmployeeTransferRepo empTrnApp = new EmployeeTransferRepo();
            EmployeeTransferVM vm = new EmployeeTransferVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empTrnApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Promotion
        public ActionResult _indexPR(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            _empPRRepo = new EmployeePromotionRepo();
            var getAllData = _empPRRepo.SelectAllByEmployee(Id);
            IEnumerable<EmployeePromotionVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.DesignationName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.GradeName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.PromotionDate.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable3 && c.IsCurrent.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeePromotionVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.DesignationName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.GradeName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.PromotionDate :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.IsCurrent.ToString() :
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
                             c.Id.ToString()
                             , c.DesignationName //+ "~" + Convert.ToString(c.Id)
                             , c.GradeName
                             , c.PromotionDate
                             , c.IsCurrent?"Current":"Previous" 
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
        //[Authorize(Roles = "Admin,Promotion")]
        [HttpGet]
        public ActionResult Promotion(string EmployeeId, int Id)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.promotionVM = new EmployeePromotionRepo().SelectById(Id);
            }
            else
            {
                vm.promotionVM = new EmployeePromotionRepo().SelectByEmployeeCurrent(EmployeeId);
                vm.promotionVM.EmployeeId = EmployeeId;
                vm.promotionVM.Id = 0;
                vm.promotionVM.FileName = "";
            }
            return PartialView("_promotion", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Promotion(EmployeeInfoVM vm, HttpPostedFileBase PromotionF)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            string[] retResults = new string[6];
            EmployeePromotionRepo empPrApp = new EmployeePromotionRepo();
            EmployeePromotionVM prnvm = new EmployeePromotionVM();
            prnvm = vm.promotionVM;
            if (PromotionF != null && PromotionF.ContentLength > 0)
            {
                prnvm.FileName = PromotionF.FileName;
            }
            if (prnvm.Id <= 0)
            {
                prnvm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                prnvm.CreatedBy = identity.Name;
                prnvm.CreatedFrom = identity.WorkStationIP;
                retResults = empPrApp.Insert(prnvm);
            }
            else
            {
                prnvm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                prnvm.LastUpdateBy = identity.Name;
                prnvm.LastUpdateFrom = identity.WorkStationIP;
                retResults = empPrApp.Update(prnvm);
            }
            if (PromotionF != null && PromotionF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Promotion"), retResults[2] + PromotionF.FileName);
                PromotionF.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            Session["mgs"] = "mgs";
            //return PartialView("_editEducation", vm);
            //return Json(mgs, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index", new { Id = prnvm.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult PromotionDelete(string ids)
        {
            EmployeePromotionRepo empTrnApp = new EmployeePromotionRepo();
            EmployeePromotionVM vm = new EmployeePromotionVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empTrnApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region JOB HISTORY
        public ActionResult _indexJH(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            _empJHRepo = new EmployeeJobHistoryRepo();
            var getAllData = _empJHRepo.SelectAllByEmployee(Id);


            IEnumerable<EmployeeJobHistoryVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Company.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.JobTitle.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.JobFrom.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                isSearchable4 && c.JobTo.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeJobHistoryVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Company :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.JobTitle :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.JobFrom.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.JobTo.ToString() :
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
                             c.Id.ToString()
                             , c.Company //+ "~" + Convert.ToString(c.Id)
                             , c.JobTitle
                             , c.JobFrom.ToString() 
                             , c.JobTo.ToString() 
                             ,c.ServiceLength
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
        public ActionResult JobHistory(string EmployeeId, int Id)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.jobHistoryVM = new EmployeeJobHistoryRepo().SelectById(Id);
            }
            else
            {
                EmployeeJobHistoryVM evm = new EmployeeJobHistoryVM();
                vm.jobHistoryVM = evm;
                vm.jobHistoryVM.EmployeeId = EmployeeId;
            }
            return PartialView("_jobHistory", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult JobHistory(EmployeeInfoVM vme, HttpPostedFileBase JobHistoryF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeJobHistoryVM vm = new EmployeeJobHistoryVM();
            EmployeeJobHistoryRepo empJHApp = new EmployeeJobHistoryRepo();
            vm = vme.jobHistoryVM;
            if (JobHistoryF != null && JobHistoryF.ContentLength > 0)
            {
                vm.FileName = JobHistoryF.FileName;
            }
            string[] retResults = new string[6];
            if (vm.Id <= 0)
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                retResults = empJHApp.Insert(vm);
            }
            else
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                retResults = empJHApp.Update(vm);
            }
            if (JobHistoryF != null && JobHistoryF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/JobHistory"), retResults[2] + JobHistoryF.FileName);
                JobHistoryF.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            Session["mgs"] = "mgs";
            // return PartialView("_editEducation", vm);
            //return Json(mgs, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index", new { Id = vm.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult JobHistoryDelete(string ids)
        {
            EmployeeJobHistoryVM vm = new EmployeeJobHistoryVM();
            EmployeeJobHistoryRepo empJHApp = new EmployeeJobHistoryRepo();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empJHApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        #endregion
        //[Authorize(Roles = "Admin,Left")]
        [HttpGet]
        public ActionResult Left(string id, string empcode, string btn)
        {
            string currentId = "";
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (empcode != null && btn != null)
            {
                vm = _infoRepo.SelectEmpForSearch(empcode, btn);
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    id = vm.Id;
                }
                else
                {
                    currentId = _infoRepo.DropDown(empcode).FirstOrDefault().Id;
                    id = currentId;
                }
            }
            vm = _infoRepo.SelectById(id);
            EmployeeLeftInformationVM vme = new EmployeeLeftInformationVM();
            vm.leftInformation = vme;
            vm.leftInformation.EmployeeId = vm.Id.ToString();
            //return PartialView("_left", vm);
            return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Left(EmployeeInfoVM evm, HttpPostedFileBase LeftInformationF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeLeftInformationVM vm = new EmployeeLeftInformationVM();
            vm = evm.leftInformation;
            if (LeftInformationF != null && LeftInformationF.ContentLength > 0)
            {
                vm.FileName = LeftInformationF.FileName;
            }
            string[] retResults = new string[6];
            EmployeeLeftInformationRepo leftRepo = new EmployeeLeftInformationRepo();
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            retResults = leftRepo.Insert(vm);
            if (LeftInformationF != null && LeftInformationF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/LeftInformation"), retResults[2] + LeftInformationF.FileName);
                LeftInformationF.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
            //Session["mgs"] = "mgs";

            //return Redirect("/HRM/EmployeeInfo?mgs =" + mgs);
        }
        [HttpPost]
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult EmployeeJob(EmployeeInfoVM vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            string[] retResults = new string[6];
            EmployeeJobRepo _empJobRepo = new EmployeeJobRepo();
            EmployeeJobVM empJob = new EmployeeJobVM();
            empJob = vm.employeeJob;
            if (empJob.Id <= 0)
            {
                empJob.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                empJob.CreatedBy = identity.Name;
                empJob.CreatedFrom = identity.WorkStationIP;
                retResults = _empJobRepo.Insert(empJob);

            }
            else
            {
                empJob.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                empJob.LastUpdateBy = identity.Name;
                empJob.LastUpdateFrom = identity.WorkStationIP;
                retResults = _empJobRepo.Update(empJob);
            }
            //Session["mgs"] = "mgs";
            var mgs = retResults[0] + "~" + retResults[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index", new { Id = empJob.EmployeeId, mgs = mgs });
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public JsonResult EmployeeSG(EmployeeInfoVM vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            string[] retResults = new string[6];
            EmployeeStructureGroupRepo _empJobRepo = new EmployeeStructureGroupRepo();
            EmployeeStructureGroupVM empJob = new EmployeeStructureGroupVM();
            empJob = vm.employeeSG;
            empJob.EmployeeId = vm.Id;
            if (empJob.Id <= 0)
            {
                empJob.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                empJob.CreatedBy = identity.Name;
                empJob.CreatedFrom = identity.WorkStationIP;
                retResults = _empJobRepo.Insert(empJob);

            }
            else
            {
                //empJob.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                //empJob.LastUpdateBy = identity.Name;
                //empJob.LastUpdateFrom = identity.WorkStationIP;
                //retResults = _empJobRepo.Update(empJob);
                retResults[0] = "Fail";
                retResults[1] = "Structure Already Saved";
            }
            var mgs = retResults[0] + "~" + retResults[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
            //Session["mgs"] = "mgs";
            //return RedirectToAction("Index", new { Id = empJob.EmployeeId, mgs = mgs });
        }
    }
}
