using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
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
    public class QualificationController : Controller
    {
        //
        // GET: /HRM/Qualification/
        EmployeeInfoRepo _infoRepo;
        EmployeeJobHistoryRepo _empJHRepo;
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index(string id, string empcode, string btn)
        {
            string currentId = "";
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeEducationVM edu = new EmployeeEducationVM();
            EmployeeLanguageVM lng = new EmployeeLanguageVM();
            EmployeeProfessionalDegreeVM PD = new EmployeeProfessionalDegreeVM();

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
            if (Session["mgs"].ToString() != "")
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                id = identity.EmployeeId;
            }
            if (id != null)
            {
                vm = _infoRepo.SelectById(id);
            }
            vm.languageVM = lng;
            vm.educationVM = edu;
            return View(vm);
        }
        #region Education
        public ActionResult _indexEducation(JQueryDataTableParamVM param, string Id)//EmployeeId
        {
            EmployeeEducationRepo _empEdRepo = new EmployeeEducationRepo();
            var getAllData = _empEdRepo.SelectAllByEmployeeId(Id);
            IEnumerable<EmployeeEducationVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Degree_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Major.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.Institute.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.YearOfPassing.ToLower().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeEducationVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Degree_E :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Major :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Institute :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.YearOfPassing :
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
                             Convert.ToString(c.Id)
                             , c.Degree_E //+ "~" + Convert.ToString(c.Id)
                             , c.Major
                             , c.Institute
                             , c.YearOfPassing 
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
        public ActionResult Education(string EmployeeId, int Id)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.educationVM = new EmployeeEducationRepo().SelectById(Id);
            }
            else
            {
                EmployeeEducationVM edu = new EmployeeEducationVM();
                edu.EmployeeId = EmployeeId;
                vm.educationVM = edu;
            }
            return PartialView("_editEducation", vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Education(EmployeeInfoVM vm, HttpPostedFileBase EducationF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            string[] retResults = new string[6];
            EmployeeEducationRepo empEduApp = new EmployeeEducationRepo();
            EmployeeEducationVM edu = new EmployeeEducationVM();
            edu = vm.educationVM;
            if (EducationF != null && EducationF.ContentLength > 0)
            {
                edu.FileName = EducationF.FileName;
            }
            if (edu.Id <= 0)
            {
                edu.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                edu.CreatedBy = identity.Name;
                edu.CreatedFrom = identity.WorkStationIP;
                retResults = empEduApp.Insert(edu);
            }
            else
            {
                edu.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                edu.LastUpdateBy = identity.Name;
                edu.LastUpdateFrom = identity.WorkStationIP;
                retResults = empEduApp.Update(edu);
            }
            if (EducationF != null && EducationF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Education"), retResults[2] + EducationF.FileName);
                EducationF.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            //Session["mgs"] = "mgs";
            Session["result"] = mgs;

            return RedirectToAction("Index", new { Id = edu.EmployeeId, mgs = mgs });
            //return Json(mgs, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult EducationDelete(string ids)
        {
            EmployeeEducationRepo empEduApp = new EmployeeEducationRepo();
            EmployeeEducationVM vm = new EmployeeEducationVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empEduApp.Delete(vm, a);
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
            //Session["mgs"] = "mgs";
            Session["result"] = mgs;

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
        #region Language
        public ActionResult _indexLanguage(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            EmployeeLanguageRepo _empLgRepo = new EmployeeLanguageRepo();
            var getAllData = _empLgRepo.SelectAllByEmployee(Id);
            IEnumerable<EmployeeLanguageVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Language_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Fluency_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.Competency_E.ToLower().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeLanguageVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Language_E :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Fluency_E :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Competency_E :
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
                             Convert.ToString(c.Id)
                             , c.Language_E //+ "~" + Convert.ToString(c.Id)
                             , c.Competency_E
                             , c.Fluency_E 
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
        public ActionResult Language(string EmployeeId, int Id)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.languageVM = new EmployeeLanguageRepo().SelectById(Id);

            }
            else
            {
                EmployeeLanguageVM lang = new EmployeeLanguageVM();
                lang.EmployeeId = EmployeeId;
                vm.languageVM = lang;
            }
            return PartialView("_editLanguage", vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Language(EmployeeInfoVM vm, HttpPostedFileBase LanguageF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            string[] retResults = new string[6];
            EmployeeLanguageRepo empLangApp = new EmployeeLanguageRepo();
            EmployeeLanguageVM edu = new EmployeeLanguageVM();
            edu = vm.languageVM;
            if (LanguageF != null && LanguageF.ContentLength > 0)
            {
                edu.FileName = LanguageF.FileName;
            }
            if (edu.Id <= 0)
            {
                edu.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                edu.CreatedBy = identity.Name;
                edu.CreatedFrom = identity.WorkStationIP;
                retResults = empLangApp.Insert(edu);
            }
            else
            {
                edu.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                edu.LastUpdateBy = identity.Name;
                edu.LastUpdateFrom = identity.WorkStationIP;
                retResults = empLangApp.Update(edu);
            }
            if (LanguageF != null && LanguageF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Language"), retResults[2] + LanguageF.FileName);
                LanguageF.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            //Session["mgs"] = mgs;
            Session["result"] = mgs;
            //return Json(mgs, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index", new { Id = edu.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult LanguageDelete(string ids)
        {
            EmployeeLanguageRepo empLangApp = new EmployeeLanguageRepo();
            EmployeeLanguageVM vm = new EmployeeLanguageVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empLangApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region ExtraCurri
        public ActionResult _indexExtraCurri(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            EmployeeExtraCurriculumActivityRepo _empExRepo = new EmployeeExtraCurriculumActivityRepo();
            var getAllData = _empExRepo.SelectAllByEmployee(Id);
            IEnumerable<EmployeeExtraCurriculumActivityVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Skill.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.SkillQuality_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.YearsOfExperience.ToString().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeExtraCurriculumActivityVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Skill :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Institute :
                                                           sortColumnIndex == 2 && isSortable_3 ? c.Achievement :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.YearsOfExperience.ToString() :
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
                             Convert.ToString(c.Id)
                             , c.Skill //+ "~" + Convert.ToString(c.Id)
                             , c.Institute
                             , c.Achievement
                             , c.YearsOfExperience.ToString() 
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
        public ActionResult ExtraCurri(string EmployeeId, int Id)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.extraCurriculumVM = new EmployeeExtraCurriculumActivityRepo().SelectById(Id);

            }
            else
            {
                EmployeeExtraCurriculumActivityVM extraCuri = new EmployeeExtraCurriculumActivityVM();
                extraCuri.EmployeeId = EmployeeId;
                vm.extraCurriculumVM = extraCuri;
            }
            return PartialView("_editExtracurri", vm);
        }

        [HttpPost]
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult ExtraCurri(EmployeeInfoVM vm, HttpPostedFileBase extraCurriF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            string[] retResults = new string[6];
            EmployeeExtraCurriculumActivityRepo empExtraCriApp = new EmployeeExtraCurriculumActivityRepo();
            EmployeeExtraCurriculumActivityVM extraCri = new EmployeeExtraCurriculumActivityVM();
            extraCri = vm.extraCurriculumVM;
            if (extraCurriF != null && extraCurriF.ContentLength > 0)
            {
                extraCri.FileName = extraCurriF.FileName;
            }
            if (extraCri.Id <= 0)
            {
                extraCri.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                extraCri.CreatedBy = identity.Name;
                extraCri.CreatedFrom = identity.WorkStationIP;
                retResults = empExtraCriApp.Insert(extraCri);
            }
            else
            {
                extraCri.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                extraCri.LastUpdateBy = identity.Name;
                extraCri.LastUpdateFrom = identity.WorkStationIP;
                retResults = empExtraCriApp.Update(extraCri);
            }
            if (extraCurriF != null && extraCurriF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/ExtraCurriculam"), retResults[2] + extraCurriF.FileName);
                extraCurriF.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            //Session["mgs"] = "mgs";
            Session["result"] = mgs;

            //return Json(mgs, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index", new { Id = extraCri.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult ExtraCurriDelete(string ids)
        {
            EmployeeExtraCurriculumActivityRepo extraCuriApp = new EmployeeExtraCurriculumActivityRepo();
            EmployeeExtraCurriculumActivityVM vm = new EmployeeExtraCurriculumActivityVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = extraCuriApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region EmployeeProfessionalDegree
        public ActionResult _indexProfessionalDegree(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            EmployeeProfessionalDegreeRepo _empPDRepo = new EmployeeProfessionalDegreeRepo();
            var getAllData = _empPDRepo.SelectAllByEmployee(Id);
            IEnumerable<EmployeeProfessionalDegreeVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Degree_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Institute.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.YearOfPassing.ToLower().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeProfessionalDegreeVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Degree_E :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Institute :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Major :
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
                             Convert.ToString(c.Id)
                             , c.Degree_E //+ "~" + Convert.ToString(c.Id)
                             , c.Institute
                             , c.YearOfPassing 
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
        public ActionResult ProfessionalDegree(string EmployeeId, int Id)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.professionalDegreeVM = new EmployeeProfessionalDegreeRepo().SelectById(Id);

            }
            else
            {
                EmployeeProfessionalDegreeVM PD = new EmployeeProfessionalDegreeVM();
                PD.EmployeeId = EmployeeId;
                vm.professionalDegreeVM = PD;
            }
            return PartialView("_editProfessionalDegree", vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult ProfessionalDegree(EmployeeInfoVM vm, HttpPostedFileBase ProfessionalDegreeF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            string[] retResults = new string[6];
            EmployeeProfessionalDegreeRepo empPDApp = new EmployeeProfessionalDegreeRepo();
            EmployeeProfessionalDegreeVM PD = new EmployeeProfessionalDegreeVM();
            PD = vm.professionalDegreeVM;
            if (ProfessionalDegreeF != null && ProfessionalDegreeF.ContentLength > 0)
            {
                PD.FileName = ProfessionalDegreeF.FileName;
            }
            if (PD.Id <= 0)
            {
                PD.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                PD.CreatedBy = identity.Name;
                PD.CreatedFrom = identity.WorkStationIP;
                retResults = empPDApp.Insert(PD);
            }
            else
            {
                PD.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                PD.LastUpdateBy = identity.Name;
                PD.LastUpdateFrom = identity.WorkStationIP;
                retResults = empPDApp.Update(PD);
            }
            if (ProfessionalDegreeF != null && ProfessionalDegreeF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Language"), retResults[2] + ProfessionalDegreeF.FileName);
                ProfessionalDegreeF.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            //Session["mgs"] = mgs;
            Session["result"] = mgs;
            //return Json(mgs, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index", new { Id = PD.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult ProfessionalDegreeDelete(string ids)
        {
            EmployeeProfessionalDegreeRepo empPDApp = new EmployeeProfessionalDegreeRepo();
            EmployeeProfessionalDegreeVM vm = new EmployeeProfessionalDegreeVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empPDApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
