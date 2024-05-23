using JQueryDataTables.Models;
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
    public class AllEmployeeInfoController : Controller
    {
        //
        // GET: /HRM/AllEmployeeInfo/

        EmployeeAssetRepo _repo = new EmployeeAssetRepo();
        //EmployeeLeftInformationRepo _repo = new EmployeeLeftInformationRepo();
        EmployeeImmigrationRepo _immRepo;
        EmployeeTransferRepo _empTNRepo;
        EmployeeEmergencyContactRepo _emergencyApp;
        EmployeeReferenceRepo _empRefRepo;
        EmployeeInfoRepo _infoRepo;
        EmployeeJobHistoryRepo _empJHRepo;
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        [HttpGet]
        public ActionResult Index(string id, string empcode, string btn)
        {
            string currentId = "";

            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeEducationVM edu = new EmployeeEducationVM();
            EmployeeLanguageVM lng = new EmployeeLanguageVM();
            EmployeeProfessionalDegreeVM PD = new EmployeeProfessionalDegreeVM();
            EmployeePresentAddressVM presentAddress = new EmployeePresentAddressVM();
            EmployeePermanentAddressVM permanentAddress = new EmployeePermanentAddressVM();
            List<EmployeeReferenceVM> references = new List<EmployeeReferenceVM>();
            EmployeeNomineeVM nomVM = new EmployeeNomineeVM();
            EmployeeEmergencyContactVM evm = new EmployeeEmergencyContactVM();
            EmployeeDependentVM depdVM = new EmployeeDependentVM();
            EmployeeAssetVM asset = new EmployeeAssetVM();
            EmployeeInfoVM empVM = new EmployeeInfoVM();
            empVM.leftInformation = new EmployeeLeftInformationVM();
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();

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

            vm.personalDetail = new EmployeePersonalDetailRepo().SelectByEmployee(vm.Id); ;
            vm.personalDetail.EmployeeId = vm.Id;
            vm.presentAddress = new EmployeePresentAddressRepo().SelectByEmployeeId(vm.Id);
            vm.presentAddress.EmployeeId = id;
            vm.permanentAddress = new EmployeePermanentAddressRepo().SelectByEmployeeId(vm.Id);
            vm.permanentAddress.EmployeeId = id;
            vm.transferVM = new EmployeeTransferRepo().SelectByEmployeeCurrent(id);
            vm.transferVM.EmployeeId = id;
            vm.employeeJob = new EmployeeJobRepo().SelectByEmployee(id);
            vm.employeeJob.EmployeeId = id;

            vm.nomineeVM = nomVM;
            vm.dependentVM = depdVM;
            vm.Id = id;
            return View(vm);
        }


        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public JsonResult PresentAddress(EmployeeInfoVM evm = null)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();
            EmployeePresentAddressRepo presentAddressRepo = new EmployeePresentAddressRepo();
            EmployeePermanentAddressRepo permanentRepo = new EmployeePermanentAddressRepo();
            EmployeePresentAddressVM vm = new EmployeePresentAddressVM();
            EmployeePermanentAddressVM vmP = new EmployeePermanentAddressVM();
            EmployeePermanentAddressVM permanentAddress = new EmployeePermanentAddressVM();
            vm = evm.presentAddress;
            string[] result = new string[6];


            if (vm.AddressType == "permanent")
            {

                vmP.EmployeeId = vm.EmployeeId;
                vmP.Address = vm.Address;
                vmP.District = vm.District;
                vmP.Division = vm.Division;
                vmP.Country = vm.Country;
                vmP.City = vm.City;
                vmP.PostalCode = vm.PostalCode;
                vmP.Phone = vm.Phone;
                vmP.Fax = vm.Fax;
                vmP.Mobile = vm.Mobile;
                vmP.FileName = vm.FileName;
                vmP.Remarks = vm.Remarks;
                vmP.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vmP.CreatedBy = identity.Name;
                vmP.CreatedFrom = identity.WorkStationIP;
                vmP.EmployeeId = evm.Id;
                result = permanentRepo.Insert(vmP);

            }
            else
            {
                if (vm.Id <= 0)
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    vm.EmployeeId = evm.Id;
                    result = presentAddressRepo.Insert(vm);
                }
                else
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    result = presentAddressRepo.Update(vm);
                }
            }


            var mgs = result[0] + "~" + result[1];
            Session["mgs"] = "mgs";
            return Json(mgs, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index", new { Id = evm.Id, mgs = mgs });
        }

        [HttpPost]
        public JsonResult PermanentAddress(EmployeeInfoVM evm = null, HttpPostedFileBase permanentAddressF = null, string EmployeeId = null)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();
            string mgs = "";
            EmployeePermanentAddressRepo permanentRepo = new EmployeePermanentAddressRepo();
            EmployeePermanentAddressVM vm = new EmployeePermanentAddressVM();
            vm = evm.permanentAddress;

            if (permanentAddressF != null && permanentAddressF.ContentLength > 0)
            {
                vm.FileName = permanentAddressF.FileName;
            }
            string[] result = new string[6];
            if (vm.Id <= 0)
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.EmployeeId = evm.Id;
                result = permanentRepo.Insert(vm);
            }
            else
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                result = permanentRepo.Update(vm);
            }
            if (permanentAddressF != null && permanentAddressF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/PermanentAddress"), result[2] + permanentAddressF.FileName);
                permanentAddressF.SaveAs(path);
            }
            mgs = result[0] + "~" + result[1];
            Session["mgs"] = "mgs";
            return Json(mgs, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index", new { Id = evm.Id, mgs = mgs });
        }

        public ActionResult _index(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            EmployeeReferenceRepo _empRefApp = new EmployeeReferenceRepo();
            var getAllData = _empRefApp.SelectAllByEmployee(Id);


            IEnumerable<EmployeeReferenceVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Relation.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.Phone.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.Mobile.ToLower().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeReferenceVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Relation :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Phone :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.Mobile :
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
                             , c.Name //+ "~" + Convert.ToString(c.Id)
                             , c.Relation
                             , c.Phone
                             , c.Mobile 
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

        //============Reference===================
        [HttpGet]
        public ActionResult Reference(string EmployeeId, int Id)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.referenceVM = new EmployeeReferenceRepo().SelectById(Id);

            }
            else
            {
                EmployeeReferenceVM refrvm = new EmployeeReferenceVM();
                refrvm.EmployeeId = EmployeeId;
                vm.referenceVM = refrvm;
            }
            return PartialView("_editReference", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public JsonResult Reference(EmployeeInfoVM vm, HttpPostedFileBase ReferenceF)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();
            string[] result = new string[6];
            _empRefRepo = new EmployeeReferenceRepo();
            EmployeeReferenceVM refrvm = new EmployeeReferenceVM();
            refrvm = vm.referenceVM;
            if (ReferenceF != null && ReferenceF.ContentLength > 0)
            {
                refrvm.FileName = ReferenceF.FileName;
            }
            if (refrvm.Id <= 0)
            {
                refrvm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                refrvm.CreatedBy = identity.Name;
                refrvm.CreatedFrom = identity.WorkStationIP;
                refrvm.IsActive = true;
                result = _empRefRepo.Insert(refrvm);
            }
            else
            {
                refrvm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                refrvm.LastUpdateBy = identity.Name;
                refrvm.LastUpdateFrom = identity.WorkStationIP;
                result = _empRefRepo.Update(refrvm);
            }
            if (ReferenceF != null && ReferenceF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Reference"), result[2] + ReferenceF.FileName);
                ReferenceF.SaveAs(path);
            }
            var mgs = result[0] + "~" + result[1];
            Session["mgs"] = "mgs";
            return Json(mgs, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index", new { Id = refrvm.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult ReferenceDelete(string ids)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "delete").ToString();

            _empRefRepo = new EmployeeReferenceRepo();
            EmployeeReferenceVM vm = new EmployeeReferenceVM();

            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _empRefRepo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public ActionResult Edit(string id, string empcode, string btn)//employee Id
        {

            string currentId = "";
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrEmpty(Session["mgs"] as string))
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
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

            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            if (!(identity.IsAdmin || identity.IsHRM))
            {
                id = identity.EmployeeId;
            }
            if (id != null)
            {
                vm = _infoRepo.SelectById(id);
                vm.emergencyContactVM = new EmployeeEmergencyContactRepo().SelectByEmployeeId(id);
                vm.Id = id;
                vm.emergencyContactVM.EmployeeId = id;
            }
            return View(vm);
            // return View(_colorRepo.SelectColor(Id));
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(EmployeeInfoVM empvm, HttpPostedFileBase EmergencyContactF)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            String[] result = new string[6];
            EmployeeEmergencyContactVM vm = new EmployeeEmergencyContactVM();
            _emergencyApp = new EmployeeEmergencyContactRepo();
            vm = empvm.emergencyContactVM;
            vm.EmployeeId = empvm.Id;
            if (EmergencyContactF != null && EmergencyContactF.ContentLength > 0)
            {
                vm.FileName = EmergencyContactF.FileName;
            }
            Session["mgs"] = "mgs";
            try
            {
                if (vm.Id <= 0)
                {
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    result = _emergencyApp.Insert(vm);
                }
                else
                {
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    result = _emergencyApp.Update(vm);
                }
                if (EmergencyContactF != null && EmergencyContactF.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/EmergencyContact"), result[2] + EmergencyContactF.FileName);
                    EmergencyContactF.SaveAs(path);
                }
                var mgs = result[0] + "~" + result[1];

                //return Json(mgs, JsonRequestBehavior.AllowGet);
                return RedirectToAction("Edit", new { Id = empvm.Id, mgs = mgs });
            }
            catch (Exception)
            {
                var mgs = "Fail~Data has Not updated succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                //return Json(mgs, JsonRequestBehavior.AllowGet);
                return RedirectToAction("Edit", new { Id = empvm.Id, mgs = mgs });
            }
        }
        [HttpGet]
        public ActionResult Details(int Id)
        {
            _emergencyApp = new EmployeeEmergencyContactRepo();
            return PartialView(_emergencyApp.SelectById(Id));
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult Delete(EmployeeEmergencyContactVM vm)
        {
            string[] result = new string[6];
            try
            {
                _emergencyApp = new EmployeeEmergencyContactRepo();
                result = _emergencyApp.Delete(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                throw ex;
            }
        }

        public ActionResult _indexIM(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            EmployeeImmigrationRepo _empImRepo = new EmployeeImmigrationRepo();
            var getAllData = _empImRepo.SelectAllByEmployee(Id);
            IEnumerable<EmployeeImmigrationVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.ImmigrationNumber.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.ImmigrationType_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.IssuedBy_E.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.IssueDate.ToLower().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeImmigrationVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.ImmigrationNumber :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.ImmigrationType_E :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.IssuedBy_E :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.IssueDate :
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
                             ,c.ImmigrationNumber //+ "~" + Convert.ToString(c.Id)
                             , c.ImmigrationType_E
                             , c.IssuedBy_E
                             , c.IssueDate 
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
        public ActionResult Immigration(string EmployeeId, int Id)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.immigrationVM = new EmployeeImmigrationRepo().SelectById(Id);

            }
            else
            {
                EmployeeImmigrationVM imm = new EmployeeImmigrationVM();
                imm.EmployeeId = EmployeeId;
                vm.immigrationVM = imm;
            }
            return PartialView("_editImmigration", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Immigration(EmployeeInfoVM vm, HttpPostedFileBase ImmigrationF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();

            string[] retResults = new string[6];
            EmployeeImmigrationRepo empImmApp = new EmployeeImmigrationRepo();
            EmployeeImmigrationVM immVM = new EmployeeImmigrationVM();
            immVM = vm.immigrationVM;
            if (ImmigrationF != null && ImmigrationF.ContentLength > 0)
            {
                immVM.FileName = ImmigrationF.FileName;
            }
            if (immVM.Id <= 0)
            {
                immVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                immVM.CreatedBy = identity.Name;
                immVM.CreatedFrom = identity.WorkStationIP;
                retResults = empImmApp.Insert(immVM);
            }
            else
            {
                immVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                immVM.LastUpdateBy = identity.Name;
                immVM.LastUpdateFrom = identity.WorkStationIP;
                retResults = empImmApp.Update(immVM);
            }
            if (ImmigrationF != null && ImmigrationF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Immigration"), retResults[2] + ImmigrationF.FileName);
                ImmigrationF.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            Session["mgs"] = "mgs";
            //return Json(mgs, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index", new { Id = immVM.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult ImmigrationDelete(string ids)
        {
            EmployeeImmigrationRepo empImmApp = new EmployeeImmigrationRepo();
            EmployeeImmigrationVM vm = new EmployeeImmigrationVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "delete").ToString();

            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empImmApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        //==================Nominee===========
        public ActionResult _indexNominee(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            EmployeeNomineeRepo _empNoRepo = new EmployeeNomineeRepo();
            var getAllData = _empNoRepo.SelectAllByEmployee(Id);
            IEnumerable<EmployeeNomineeVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Relation.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.Mobile.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.Phone.ToLower().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeNomineeVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Relation :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Mobile :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.Phone :
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
                             , c.Name //+ "~" + Convert.ToString(c.Id)
                             , c.Relation
                             , c.Mobile
                             , c.Phone 
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
        public ActionResult Nominee(string EmployeeId, int Id)
        {
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.nomineeVM = new EmployeeNomineeRepo().SelectById(Id);

            }
            else
            {
                EmployeeNomineeVM nom = new EmployeeNomineeVM();
                nom.EmployeeId = EmployeeId;
                vm.nomineeVM = nom;
            }
            return PartialView("_editNominee", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Nominee(EmployeeInfoVM vm, HttpPostedFileBase NomineeF)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();

            string[] result = new string[6];
            EmployeeNomineeRepo empNonApp = new EmployeeNomineeRepo();
            EmployeeNomineeVM non = new EmployeeNomineeVM();
            non = vm.nomineeVM;
            if (NomineeF != null && NomineeF.ContentLength > 0)
            {
                non.FileName = NomineeF.FileName;
            }
            //edu.EmployeeId = vm.Id;
            if (non.Id <= 0)
            {
                non.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                non.CreatedBy = identity.Name;
                non.CreatedFrom = identity.WorkStationIP;
                result = empNonApp.Insert(non);
            }
            else
            {
                non.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                non.LastUpdateBy = identity.Name;
                non.LastUpdateFrom = identity.WorkStationIP;
                result = empNonApp.Update(non);
            }
            if (NomineeF != null && NomineeF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Nominee"), result[2] + NomineeF.FileName);
                NomineeF.SaveAs(path);
            }
            var mgs = result[0] + "~" + result[1];
            //return Json(mgs, JsonRequestBehavior.AllowGet);

            Session["mgs"] = "mgs";
            return RedirectToAction("Index", new { Id = non.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult NomineeDelete(string ids)
        {
            EmployeeNomineeRepo empNonApp = new EmployeeNomineeRepo();
            EmployeeNomineeVM vm = new EmployeeNomineeVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empNonApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        //==================Dependent===========
        public ActionResult _indexDependent(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            EmployeeDependentRepo _empDepRepo = new EmployeeDependentRepo();
            var getAllData = _empDepRepo.SelectAllByEmployee(Id);
            IEnumerable<EmployeeDependentVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Relation.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.Phone.ToLower().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeDependentVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Relation :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Phone :
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
                             , c.Name
                             , c.Relation, c.Mobile 
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
        public ActionResult Dependent(string EmployeeId, int Id)
        {
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.dependentVM = new EmployeeDependentRepo().SelectById(Id);

            }
            else
            {
                EmployeeDependentVM depen = new EmployeeDependentVM();
                depen.EmployeeId = EmployeeId;
                vm.dependentVM = depen;
            }
            return PartialView("_editDependent", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Dependent(EmployeeInfoVM vm, HttpPostedFileBase DependentF)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();

            string[] result = new string[6];
            EmployeeDependentRepo empDepApp = new EmployeeDependentRepo();
            EmployeeDependentVM dep = new EmployeeDependentVM();
            dep = vm.dependentVM;
            //edu.EmployeeId = vm.Id;
            if (DependentF != null && DependentF.ContentLength > 0)
            {
                dep.FileName = DependentF.FileName;
            }
            if (dep.Id <= 0)
            {
                dep.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                dep.CreatedBy = identity.Name;
                dep.CreatedFrom = identity.WorkStationIP;
                result = empDepApp.Insert(dep);
            }
            else
            {
                dep.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                dep.LastUpdateBy = identity.Name;
                dep.LastUpdateFrom = identity.WorkStationIP;
                result = empDepApp.Update(dep);
            }
            if (DependentF != null && DependentF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Dependent"), result[2] + DependentF.FileName);
                DependentF.SaveAs(path);
            }
            var mgs = result[0] + "~" + result[1];
            //return Json(mgs, JsonRequestBehavior.AllowGet);
            Session["mgs"] = "mgs";
            return RedirectToAction("Index", new { Id = dep.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult DependentDelete(string ids)
        {
            //EmployeeNomineeRepo empNonApp = new EmployeeNomineeRepo();
            EmployeeDependentRepo empDepApp = new EmployeeDependentRepo();
            EmployeeDependentVM vm = new EmployeeDependentVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empDepApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        //==================Asset===========
        [HttpGet]
        public ActionResult Asset(string EmployeeId, int Id = 0)
        {
            _repo = new EmployeeAssetRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);
            if (Id != 0)
            {
                vm.AssetVM = _repo.SelectAll(Id).FirstOrDefault();
            }
            else
            {
                EmployeeAssetVM asset = new EmployeeAssetVM();
                asset.EmployeeId = EmployeeId;
                vm.AssetVM = asset;
            }
            return PartialView("_empAsset", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(EmployeeInfoVM vme, HttpPostedFileBase AssetFile)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeAssetVM vm = new EmployeeAssetVM();
            _repo = new EmployeeAssetRepo();
            vm = vme.AssetVM;
            if (AssetFile != null && AssetFile.ContentLength > 0)
            {
                vm.FileName = AssetFile.FileName;
            }
            string[] retResults = new string[6];
            if (vm.Id <= 0)
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                retResults = _repo.Insert(vm);
            }
            else
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                retResults = _repo.Update(vm);
            }
            if (AssetFile != null && AssetFile.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Asset"), retResults[2] + AssetFile.FileName);
                AssetFile.SaveAs(path);
            }
            var mgs = retResults[0] + "~" + retResults[1];
            Session["mgs"] = "mgs";
            return RedirectToAction("Index", new { Id = vm.EmployeeId, mgs = mgs });
        }
        public JsonResult Delete(string ids)
        {
            EmployeeAssetVM vm = new EmployeeAssetVM();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        //==================Left===========
        public ActionResult _indexActiveEmployee(JQueryDataTableParamModel param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_5"]);
            //Code
            //EmpName 
            //Department 
            //Designation
            //JoinDate

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
            }
            #endregion Column Search


            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            List<EmployeeInfoVM> getAllData = new List<EmployeeInfoVM>();
            IEnumerable<EmployeeInfoVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (identity.IsAdmin || identity.IsHRM)
            {
                getAllData = _empRepo.SelectAllActiveEmp();
            }
            else
            {
                getAllData.Add(_empRepo.SelectById(Identit.EmployeeId));

            }

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
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.JoinDate.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #region Column Filtering
            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || (joinDateFilter != "" && joinDateFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    &&
                                    (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    &&
                                    (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    &&
                                    (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    &&
                                    (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
                                    &&
                                    (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.JoinDate))
                                );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.JoinDate) :
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
                , c.EmpName 
                , c.Department 
                , c.Designation
                , c.JoinDate
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
        public ActionResult Left()
        {
            EmployeeInfoVM empVM = new EmployeeInfoVM();
            empVM.leftInformation = new EmployeeLeftInformationVM();
            return View(empVM);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Left(EmployeeInfoVM evm, HttpPostedFileBase LeftInformationF)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_21", "process").ToString();
            EmployeeLeftInformationVM vm = new EmployeeLeftInformationVM();
            vm = evm.leftInformation;
            if (LeftInformationF != null && LeftInformationF.ContentLength > 0)
            {
                vm.FileName = LeftInformationF.FileName;
            }
            string[] retResults = new string[6];
            EmployeeLeftInformationRepo leftRepo = new EmployeeLeftInformationRepo();
            //vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //vm.CreatedBy = identity.Name;
            //vm.CreatedFrom = identity.WorkStationIP;

            if (vm.Id > 0)
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                retResults = leftRepo.Update(vm);
            }
            else
            {

                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                retResults = leftRepo.Insert(vm);
            }

            if (LeftInformationF != null && LeftInformationF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/LeftInformation"), retResults[2] + LeftInformationF.FileName);
                LeftInformationF.SaveAs(path);
            }
            //return Json(retResults[0] + "~" + retResults[1], JsonRequestBehavior.AllowGet);
            Session["result"] = retResults[0] + "~" + retResults[1];

            if (vm.Id > 0)
            {
                //return RedirectToAction("Left", "Left");
                return RedirectToAction("Index", "Left");
            }
            else
            {
                return RedirectToAction("Left", "Left");
                //return RedirectToAction("Index", "Left");
            }
        }

        public ActionResult leftdetailCreate(string id, string employeeId = "", string empcode = "", string btn = "current")
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeLeftInformationVM empleft = new EmployeeLeftInformationVM();
            EmployeeLeftInformationRepo empleftRepo = new EmployeeLeftInformationRepo();

            string[] conditionFields = { "el.EmployeeId", "ve.Code" };
            string[] conditionValues = { employeeId, empcode }; ;

            if (string.IsNullOrWhiteSpace(employeeId))
            {
                if (!string.IsNullOrWhiteSpace(empleft.EmployeeId))
                {
                    employeeId = empleft.EmployeeId;
                }
            }


            if (!string.IsNullOrEmpty(employeeId))
            {
                vm = repo.SelectByIdAll(employeeId);
            }
            if (!string.IsNullOrWhiteSpace(empcode) && !string.IsNullOrWhiteSpace(btn))
            {
                vm = repo.SelectEmpStructureAll(empcode, btn);
            }


            if (string.IsNullOrWhiteSpace(employeeId))
            {
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    employeeId = vm.Id;
                }
            }


            empleft.EmployeeId = vm.Id;
            vm.leftInformation = empleft;
            return PartialView("_leftDetail", vm);
        }
        public ActionResult leftdetailEdit(string id, string employeeId = "", string empcode = "", string btn = "current")
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeLeftInformationVM empleft = new EmployeeLeftInformationVM();
            EmployeeLeftInformationRepo empleftRepo = new EmployeeLeftInformationRepo();

            string[] conditionFields = { "el.EmployeeId", "ve.Code" };
            string[] conditionValues = { employeeId, empcode }; ;

            empleft = empleftRepo.SelectAll(Convert.ToInt32(id), conditionFields, conditionValues).FirstOrDefault();


            if (string.IsNullOrWhiteSpace(employeeId))
            {
                if (!string.IsNullOrWhiteSpace(empleft.EmployeeId))
                {
                    employeeId = empleft.EmployeeId;
                }
            }


            if (!string.IsNullOrEmpty(employeeId))
            {
                vm = repo.SelectByIdAll(employeeId);
            }
            if (!string.IsNullOrWhiteSpace(empcode) && !string.IsNullOrWhiteSpace(btn))
            {
                vm = repo.SelectEmpStructureAll(empcode, btn);
            }


            if (string.IsNullOrWhiteSpace(employeeId))
            {
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    employeeId = vm.Id;
                }
            }


            empleft.EmployeeId = vm.Id;
            vm.leftInformation = empleft;
            return PartialView("_leftDetail", vm);
        }

        //==============Personal Details=============
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
        public ActionResult PersonalDetail(EmployeeInfoVM vm, HttpPostedFileBase NIDF, HttpPostedFileBase DisabilityFile, HttpPostedFileBase PassportFile, HttpPostedFileBase SignatureFile)
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
            if (DisabilityFile != null && DisabilityFile.ContentLength > 0)
            {
                pvm.DisabilityFile = DisabilityFile.FileName;
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
            if (DisabilityFile != null && DisabilityFile.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/DisabilityFile"), retResults[2] + DisabilityFile.FileName);
                DisabilityFile.SaveAs(path);
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

    }
}
