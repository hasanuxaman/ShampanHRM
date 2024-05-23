using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;

using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    [Authorize]
    public class EmployeeInfoController : Controller
    {

        //
        // GET: /HRM/EmployeeInfo/
        //string[] roles = new string[] { "CC" };
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index1()
        {
            EmployeeInfoVM vm = new EmployeeInfoVM();
            vm.Code = "100";
            vm.FullName = "100";
            return View(vm);
        }
        public ActionResult Index(string returnUrl)
        {

            if (!string.IsNullOrEmpty(Session["mgs"] as string))
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_18", "index").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/HRM/Home");
                }
                if (identity.IsESS)
                {
                    return Redirect("/hrm/employeeinfo/Edit/" + identity.EmployeeId);
                }
                List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
                EmployeeInfoVM vm = new EmployeeInfoVM();
                vm.ClientEmployeeIndex = new AppSettingsReader().GetValue("ClientEmployeeIndex", typeof(string)).ToString();
                VMs.Add(vm);
                return View(VMs);
            }
            catch (Exception)
            {
                return Redirect(new AppSettingsReader().GetValue("PAGENOTFOUND", typeof(string)).ToString());
            }
        }
        [Authorize]
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var designationFilter = Convert.ToString(Request["sSearch_3"]);
            var departmentFilter = Convert.ToString(Request["sSearch_4"]);
            var sectionFilter = Convert.ToString(Request["sSearch_5"]);
            var projecttFilter = Convert.ToString(Request["sSearch_6"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_7"]);

            //Code
            //EmpName 
            //Designation
            //Department 
            //Section
            //Projectt
            //JoinDate

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
            }


            var fromID = 0;
            var toID = 0;
            if (idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }

            #endregion Column Search

            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            var getAllData = _empRepo.SelectAllActiveEmp();
            IEnumerable<EmployeeInfoVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);

                filteredData = getAllData
                   .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.Section.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.Project.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable7 && c.JoinDate.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            if (codeFilter != "" || empNameFilter != "" || designationFilter != "" || departmentFilter != "" || sectionFilter != "" || projecttFilter != "" || (joinDateFilter != "" && joinDateFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    && (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    && (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    && (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    && (sectionFilter == "" || c.Section.ToLower().Contains(sectionFilter.ToLower()))
                                    && (projecttFilter == "" || c.Project.ToLower().Contains(projecttFilter.ToLower()))
                                    && (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
                                    && (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.JoinDate))
                                );
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? c.Section :
                sortColumnIndex == 6 && isSortable_6 ? c.Project :
                sortColumnIndex == 7 && isSortable_7 ? Ordinary.DateToString(c.JoinDate) :
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
                  c.Id
                , c.Code
                , c.EmpName 
                , c.Designation
                , c.Department 
                , c.Section    
                , c.Project    
                , c.JoinDate
                //,c.Id
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
        public ActionResult _indexKajol(JQueryDataTableParamModel param)
        {

            #region Column Search Variables
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var designationFilter = Convert.ToString(Request["sSearch_3"]);
            var departmentFilter = Convert.ToString(Request["sSearch_4"]);
            var projectFilter = Convert.ToString(Request["sSearch_5"]);
            var other1Filter = Convert.ToString(Request["sSearch_6"]);
            var other2Filter = Convert.ToString(Request["sSearch_7"]);
            var other3Filter = Convert.ToString(Request["sSearch_8"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_9"]);

            //Code
            //EmpName 
            //Designation
            //Department 
            //Section
            //Projectt
            //JoinDate

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
            }


            var fromID = 0;
            var toID = 0;
            if (idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }

            #endregion Column Search

            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            var getAllData = _empRepo.SelectAllActiveEmp();
            IEnumerable<EmployeeInfoVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_9"]);
                filteredData = getAllData
                   .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.Project.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.Other1.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable7 && c.Other2.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable8 && c.Other3.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable9 && c.JoinDate.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #region Column Search

            if (codeFilter != "" || empNameFilter != "" || designationFilter != "" || departmentFilter != "" || projectFilter != ""
                || other1Filter != "" || other2Filter != "" || other3Filter != ""
                || (joinDateFilter != "" && joinDateFilter != "~")
                )
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    && (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    && (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    && (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    && (projectFilter == "" || c.Project.ToLower().Contains(projectFilter.ToLower()))
                                    && (other1Filter == "" || c.Other1.ToLower().Contains(other1Filter.ToLower()))
                                    && (other2Filter == "" || c.Other2.ToLower().Contains(other2Filter.ToLower()))
                                    && (other3Filter == "" || c.Other3.ToLower().Contains(other3Filter.ToLower()))
                                    && (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
                                    && (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.JoinDate))
                                );
            }
            #endregion

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_9"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? c.Project :
                sortColumnIndex == 6 && isSortable_6 ? c.Other1 :
                sortColumnIndex == 7 && isSortable_7 ? c.Other2 :
                sortColumnIndex == 8 && isSortable_8 ? c.Other3 :
                sortColumnIndex == 9 && isSortable_9 ? Ordinary.DateToString(c.JoinDate) :
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
                  c.Id
                , c.Code
                , c.EmpName 
                , c.Designation
                , c.Department 
                , c.Project  
                , c.Other1
                , c.Other2
                , c.Other3
                , c.JoinDate
                //,c.Id
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
        public ActionResult _indexSearch(JQueryDataTableParamVM param, string code, string name)
        {
            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            var getAllData = _empRepo.SelectAll();
            IEnumerable<EmployeeInfoVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Salutation_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.MiddleName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.LastName.ToLower().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeInfoVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Salutation_E :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.MiddleName :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.LastName :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { c.Code, c.Salutation_E, c.MiddleName, c.LastName, Convert.ToString(c.Id) };
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
        public ActionResult Edit(string Id, string empcode, string btn)
        {
            string currentId = "";
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            if (empcode != null && btn != null)
            {
                vm = _infoRepo.SelectEmpForSearch(empcode, btn);

                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    Id = vm.Id;
                }
                else
                {
                    currentId = _infoRepo.DropDown(empcode).FirstOrDefault().Id;
                    Id = currentId;
                }
            }
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Id = identity.EmployeeId;
            }
            EmployeePersonalDetailVM personalDetail = new EmployeePersonalDetailVM();
            personalDetail.BloodGroup_E = "Black";
            vm.personalDetail = personalDetail;
            if (!string.IsNullOrWhiteSpace(Id))
            {
                vm = _infoRepo.SelectById(Id);
            }
            EmployeePersonalDetailRepo _prRepo = new EmployeePersonalDetailRepo();

            vm.personalDetail = _prRepo.SelectByEmployee(Id); ;
            vm.personalDetail.EmployeeId = vm.Id;
            //vm.PhotoName = vm.PhotoName;
            string directory = Server.MapPath(@"~/Files/EmployeeInfo\") + vm.PhotoName; // + Id + ".jpg";
            if (!System.IO.File.Exists(directory))
            {
                if (string.IsNullOrWhiteSpace(vm.Gender_E) || vm.Gender_E.ToLower() == "male")
                {
                    vm.PhotoName = "0.jpg";

                }
                else
                {
                    vm.PhotoName = "0F.jpg";
                    if (!System.IO.File.Exists(Server.MapPath(@"~/Files/EmployeeInfo\0F.jpg")))
                    {
                        vm.PhotoName = "0.jpg";
                        
                    }
                }
               
            }
            return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(EmployeeInfoVM VM)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] result = new string[6];
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            VM.LastUpdateBy = identity.Name;
            VM.LastUpdateFrom = identity.WorkStationIP;
            VM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                result = _infoRepo.Update(VM);
                var mgs = result[0] + "~" + result[1];
                return Json(mgs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
                ////  Session["result"] = "Fail~Data has Not updated succeessfully";
                //var mgs = "Fail~Data has Not updated succeessfully";
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                //return RedirectToAction("Index", new { mgs = mgs });
            }
        }
        public ActionResult EditBackup(EmployeeInfoVM vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] result = new string[6];
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            Session["mgs"] = "mgs";
            try
            {
                result = _infoRepo.Update(vm);
                var mgs = result[0] + "~" + result[1];
                ViewBag.mgs = mgs;
                return Json(mgs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                //  Session["result"] = "Fail~Data has Not updated succeessfully";
                var mgs = "Fail~Data has Not updated succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index", new { mgs = mgs });
            }
        }
        [HttpGet]
        public ActionResult DesigEdit(int Id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            DesignationVM designation = new DesignationVM();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            vm.Id = Id.ToString();
            vm.designationVM = designation;
            return PartialView(vm);
            // return View(_colorRepo.SelectColor(Id));
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult DesigEdit(EmployeeInfoVM vm)
        {
            return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult PersonalDetail(EmployeeInfoVM vm, HttpPostedFileBase FingerprintFile, HttpPostedFileBase VaccineFiles2, HttpPostedFileBase VaccineFile1, HttpPostedFileBase VaccineFile2, HttpPostedFileBase VaccineFile3, HttpPostedFileBase TINFile, HttpPostedFileBase NIDF, HttpPostedFileBase TINF, HttpPostedFileBase DisabilityFile, HttpPostedFileBase PassportFile, HttpPostedFileBase SignatureFile)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            string[] retResults = new string[6];
            EmployeePersonalDetailRepo _prRepo = new EmployeePersonalDetailRepo();
            EmployeePersonalDetailVM pvm = new EmployeePersonalDetailVM();
            pvm = vm.personalDetail;
            if (NIDF != null && NIDF.ContentLength > 0)
            {
                pvm.NIDFile = NIDF.FileName;
            }
            if (TINFile != null && TINFile.ContentLength > 0)
            {
                pvm.TINFile = TINFile.FileName;
            }
            if (DisabilityFile != null && DisabilityFile.ContentLength > 0)
            {
                pvm.DisabilityFile = DisabilityFile.FileName;
            }
            if (FingerprintFile != null && FingerprintFile.ContentLength > 0)
            {
                pvm.FingerprintFile = FingerprintFile.FileName;
            }
            
            if (VaccineFiles2 != null && VaccineFiles2.ContentLength > 0)
            {
                pvm.VaccineFiles2 = VaccineFiles2.FileName;
            }
            if (VaccineFile1 != null && VaccineFile1.ContentLength > 0)
            {
                pvm.VaccineFile1 = VaccineFile1.FileName;
            }
            
            if (VaccineFile3 != null && VaccineFile3.ContentLength > 0)
            {
                pvm.VaccineFile3 = VaccineFile3.FileName;
            }
            if (PassportFile != null && PassportFile.ContentLength > 0)
            {
                pvm.PassportFile = PassportFile.FileName;
            }
            
            if (SignatureFile != null && SignatureFile.ContentLength > 0)
            {
                pvm.Signature = SignatureFile.FileName;
            }
            
            if (pvm.Id <= 0)
            {
                pvm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                pvm.CreatedBy = identity.Name;
                pvm.CreatedFrom = identity.WorkStationIP;
                retResults = _prRepo.Insert(pvm);

            }
            else
            {
                pvm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                pvm.LastUpdateBy = identity.Name;
                pvm.LastUpdateFrom = identity.WorkStationIP;
                retResults = _prRepo.Update(pvm);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            if (NIDF != null && NIDF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/NID"), retResults[2] + NIDF.FileName);
                NIDF.SaveAs(path);
            }
            //TINFile
            if (TINFile != null && TINFile.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/TINFile"), retResults[2] + TINFile.FileName);
                TINFile.SaveAs(path);
            }
            if (DisabilityFile != null && DisabilityFile.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/DisabilityFile"), retResults[2] + DisabilityFile.FileName);
                DisabilityFile.SaveAs(path);
            }
            if (FingerprintFile != null && FingerprintFile.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/FingerprintFile"), retResults[2] + FingerprintFile.FileName);
                FingerprintFile.SaveAs(path);
            }
            if (VaccineFiles2 != null && VaccineFiles2.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/VaccineFiles2"), retResults[2] + VaccineFiles2.FileName);
                VaccineFiles2.SaveAs(path);
            }
            if (VaccineFile1 != null && VaccineFile1.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/VaccineFile1"), retResults[2] + VaccineFile1.FileName);
                VaccineFile1.SaveAs(path);
            }

            if (VaccineFile2 != null && VaccineFile2.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/VaccineFiles2"), retResults[2] + VaccineFile2.FileName);
                VaccineFile2.SaveAs(path);
            }

            if (VaccineFile3 != null && VaccineFile3.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/VaccineFile3"), retResults[2] + VaccineFile3.FileName);
                VaccineFile3.SaveAs(path);
            }
            if (PassportFile != null && PassportFile.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/PassportFile"), retResults[2] + PassportFile.FileName);
                PassportFile.SaveAs(path);
            }
            if (SignatureFile != null && SignatureFile.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/SignatureFile"), retResults[2] + SignatureFile.FileName);
                SignatureFile.SaveAs(path);
            }
            
            Session["mgs"] = "mgs";
            Session["result"] = mgs;
            return RedirectToAction("Edit", new { Id = pvm.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult PersonalDetailb(dynamic vm)
        {

            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  

                    HttpPostedFileBase file = files[i];
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            EmployeeInfoVM vm = new EmployeeInfoVM();
            SettingRepo _setDAL = new SettingRepo();
            vm.AutoCode = _setDAL.settingValue("AutoCode", "Employee").ToString() == "Y" ? "Y" : "N";
            return PartialView("_Create", vm);
        }
        [HttpPost]
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult Create(EmployeeInfoVM vm)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            string[] result = new string[6];
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            vm.IsActive = true;
            EmployeeInfoRepo rmpRepo = new EmployeeInfoRepo();
            result = rmpRepo.Insert(vm);
            // Session["result"] = result[0] + "~" + result[1];
            var mgs = result[0] + "~" + result[1];
            Session["mgs"] = "mgs";
            if (result[0] != "Fail")
            {
                return RedirectToAction("Edit", new { Id = result[2], mgs = mgs });
            }
            else
            {
                return RedirectToAction("Index", new { mgs = mgs });
            }

        }

        public ActionResult EmployeeImage(HttpPostedFileBase file, string EmployeeId)
        {
            // return View();
            string[] result = new string[6];
            var mgs = "";
            if (file != null && file.ContentLength > 0)
            {
                EmployeeInfoRepo _repo = new EmployeeInfoRepo();
                var code = _repo.SelectById(EmployeeId).Code;

                string photoName = code + "." + file.FileName.Split('.')[1];

                result = _repo.UpdatePhoto(EmployeeId, photoName);

                if (result[0] == "Success")
                {
                    var path = Path.Combine(Server.MapPath("~/Files/EmployeeInfo"), photoName);
                    ////var path = Path.Combine(Server.MapPath("~/Files/EmployeeInfo"), code + "." + file.FileName.Split('.')[1]);
                    file.SaveAs(path);
                    mgs = "Success" + "~" + "Profile picture saved!";
                    Session["mgs"] = "mgs";
                }
                else
                {
                    mgs = "Fail" + "~" + "Profile picture Not saved";
                }

            }
            Session["mgs"] = "mgs";

            return Redirect("/HRM/EmployeeInfo/Edit/" + EmployeeId + "?mgs=" + mgs);
        }
        public JsonResult EmployeeImageDelete(string EmployeeId)
        {
            string[] result = new string[6];

            EmployeeInfoRepo _repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            vm = _repo.SelectById(EmployeeId);
            string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\EmployeeInfo\\" +  vm.PhotoName;
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                result = _repo.UpdatePhoto(EmployeeId, "0.jpg");

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ChangePassword(string Id, string empcode, string btn)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            string currentId = "";
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (empcode != null && btn != null)
            {
                vm = _infoRepo.SelectEmpForSearch(empcode, btn);
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    Id = vm.Id;
                }
                else
                {
                    currentId = _infoRepo.DropDown(empcode).FirstOrDefault().Id;
                    Id = currentId;
                }
            }
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Id = Session["EmployeeId"].ToString();
            }
            vm = _infoRepo.SelectById(Id);
            //vm.fileName = vm.Id + ".jpg";
            //string directory = Server.MapPath(@"~/Files/EmployeeInfo\") + vm.Id + ".jpg";
            //if (!System.IO.File.Exists(directory))
            //{
            //    vm.fileName = "0.jpg";
            //}
            return View(vm);
        }
        [HttpPost]
        public ActionResult ChangePassword(string OldPassword, string Password, string employeeId, string tt)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            UserInformationRepo userRepo = new UserInformationRepo();
            UserLogsVM vm = new UserLogsVM();
            //vm.OldPassword = Ordinary.Encrypt(OldPassword, true);
            //vm.Password = Ordinary.Encrypt(Password, true);

            vm.OldPassword = OldPassword;
            vm.Password = Password;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            //vm.EmployeeId = Session["EmployeeId"].ToString();
            vm.EmployeeId = employeeId;
            string[] retResults = new string[6];
            retResults = userRepo.ChangePassword(vm);
            var mgs = retResults[0] + "~" + retResults[1];
            Session["mgs"] = "mgs";
            return RedirectToAction("Edit", new { Id = employeeId, mgs = mgs });
        }
        //public ActionResult RptEmployeeInfo(string name = "", string code = "")
        //{

        //    try
        //    {

        //        EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
        //        var getAllData = _empRepo.SelectAll();

        //        //EmployeeInfoVM_Proxy : EmployeeInfoVM

        //        ReportClass doc = new ReportClass();//default
        //        doc = new rptEmployeeInfo();
        //        //doc.Database.Tables[0].SetDataSource(SymERPHtmlHelper.ConvertToDataTable<EmployeeInfoVM>(getAllData.ToList()));

        //        //doc.DataDefinition.FormulaFields["ReportName"].Text = "'" + ReportName + "'";
        //        //doc.DataDefinition.FormulaFields["SchoolName"].Text = "'" + SchoolName + "'";
        //        //doc.DataDefinition.FormulaFields["SchoolAddress"].Text = "'" + SchoolAddress + "'";
        //        //doc.DataDefinition.FormulaFields["SchoolContact"].Text = "'" + SchoolContact + "'";
        //        string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
        //        doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";

        //        var rpt = RenderReportAsPDF(doc);
        //        doc.Close();
        //        return rpt;

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}
        [Authorize]
        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult EmployeeDelete(string ids)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();

            string[] a = ids.Split('~');
            string[] result = new string[6];

            EmployeeInfoVM vm = new EmployeeInfoVM();
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _empRepo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeExist(string term)
        {
            string item = "";
            string code = "";
            string name = "";
            EmployeeInfoRepo prepo = new EmployeeInfoRepo();
            string Exit = "";
            string transfer = "0";
            if (!string.IsNullOrWhiteSpace(term))
            {
                item = term;
                code = term.Split('>')[0];//code
                if (term.Contains('>'))
                    name = term.Split('>')[1]; // name
                if (!string.IsNullOrWhiteSpace(code))
                    Exit = prepo.EmployeeExist(code);
                if (string.IsNullOrWhiteSpace(Exit) && !string.IsNullOrWhiteSpace(name))
                    Exit = prepo.EmployeeExist(name);
                if (string.IsNullOrWhiteSpace(Exit) && !string.IsNullOrWhiteSpace(item))
                    Exit = prepo.EmployeeExist(item);
            }
            if (!string.IsNullOrWhiteSpace(Exit))
            {
                transfer = Exit;
            }
            return Json(transfer, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        #region ImportData
        public ActionResult ImportData()
        {
            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            var getAllData = _empRepo.SelectAll();
            return View(getAllData);
        }
        public ActionResult _ImportData()
        {
            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            var getAllData = _empRepo.SelectAll();
            return View(getAllData);
        }
        #endregion ImportData
   
        [HttpPost]
        public ActionResult GetOtherId(string EmpCategory = "", string EmpJobType = "")
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (CompanyName.ToUpper() == "G4S")
            {
                EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
                int result = 0;
                try
                {
                    result = _infoRepo.NextOtherId(EmpCategory, EmpJobType);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
