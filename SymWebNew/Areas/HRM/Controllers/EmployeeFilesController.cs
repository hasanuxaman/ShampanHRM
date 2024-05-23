using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
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
    public class EmployeeFilesController : Controller
    {
        //
        // GET: /HRM/EmployeeFiles/

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
