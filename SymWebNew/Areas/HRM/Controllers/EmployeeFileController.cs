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
    public class EmployeeFileController : Controller
    {
        //
        // GET: /HRM/EmployeeFile/
        EmployeeFilesRepo _Filesapp;
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

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
                vm.employeeFiles = new EmployeeFilesRepo().SelectByEmployeeId(id);
                vm.Id = id;
                vm.employeeFiles.EmployeeId = id;
            }
            return View(vm);
            // return View(_colorRepo.SelectColor(Id));
        }



        [HttpPost]
        public ActionResult Edit(EmployeeInfoVM empvm,HttpPostedFileBase FileName,HttpPostedFileBase SignatureFiles, HttpPostedFileBase DisabilityFile, 
            HttpPostedFileBase VaccineFiles2, HttpPostedFileBase VaccineFile3, HttpPostedFileBase VaccineFile1, HttpPostedFileBase FingerprintFile, 
            HttpPostedFileBase EmployeePersonalDetail_Fingerprint, HttpPostedFileBase NIDFile, HttpPostedFileBase PassportFile,
            HttpPostedFileBase N_VaccineFile1, HttpPostedFileBase EmployeeNominee_VaccineFile2, HttpPostedFileBase N_VaccineFile3,
            HttpPostedFileBase D_VaccineFile1, HttpPostedFileBase D_VaccineFile2, HttpPostedFileBase D_VaccineFile3,
            HttpPostedFileBase edu_Certificate, HttpPostedFileBase Lng_Achivement, HttpPostedFileBase Experience_Certificate,
            HttpPostedFileBase Extra_FileName, HttpPostedFileBase PassportVisa, HttpPostedFileBase BillVoucher,HttpPostedFileBase AssetFileName,
            HttpPostedFileBase Certificate, HttpPostedFileBase EmergencyContactF)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            String[] result = new string[6];
            EmployeeFilesVM vm = new EmployeeFilesVM();
            _Filesapp = new EmployeeFilesRepo();
            vm = empvm.employeeFiles;
            vm.EmployeeId = empvm.Id;

            if (EmployeePersonalDetail_Fingerprint != null && EmployeePersonalDetail_Fingerprint.ContentLength > 0)
            {
                vm.EmployeePersonalDetail_Fingerprint = EmployeePersonalDetail_Fingerprint.FileName;
            }
            if (FingerprintFile != null && FingerprintFile.ContentLength > 0)
            {
                vm.EmployeePersonalDetail_Fingerprint = FingerprintFile.FileName;
            }
            if (PassportFile != null && PassportFile.ContentLength > 0)
            {
                vm.EmployeePersonalDetail_PassportFile = PassportFile.FileName;
            }
            if (NIDFile != null && NIDFile.ContentLength > 0)
            {
                vm.EmployeePersonalDetail_NIDFile = NIDFile.FileName;
            }
            if (VaccineFile1 != null && VaccineFile1.ContentLength > 0)
            {
                vm.EmployeePersonalDetail_VaccineFile1 = VaccineFile1.FileName;
            }
            if (VaccineFiles2 != null && VaccineFiles2.ContentLength > 0)
            {
                vm.EmployeePersonalDetail_VaccineFiles2 = VaccineFiles2.FileName;
            }
            if (VaccineFile3 != null && VaccineFile3.ContentLength > 0)
            {
                vm.EmployeePersonalDetail_VaccineFile3 = VaccineFile3.FileName;
            }
            if (DisabilityFile != null && DisabilityFile.ContentLength > 0)
            {
                vm.EmployeePersonalDetail_DisabilityFile = DisabilityFile.FileName;
            }
            if (SignatureFiles != null && SignatureFiles.ContentLength > 0)
            {
                vm.SignatureFiles = SignatureFiles.FileName;
            }
            if (FileName != null && FileName.ContentLength > 0)
            {
                vm.FileName = FileName.FileName;
            }
            if (N_VaccineFile1 != null && N_VaccineFile1.ContentLength >0)
            {
                vm.EmployeeNominee_VaccineFile1 = N_VaccineFile1.FileName;   
            }
            if (EmployeeNominee_VaccineFile2 != null && EmployeeNominee_VaccineFile2.ContentLength > 0)
            {
                vm.EmployeeNominee_VaccineFile2 = EmployeeNominee_VaccineFile2.FileName;
            }
            if (VaccineFile1 != null && VaccineFile1.ContentLength > 0)
            {
                vm.EmployeeNominee_VaccineFile3 = VaccineFile1.FileName;
            }
            if (N_VaccineFile3 != null && N_VaccineFile3.ContentLength > 0)
            {
                vm.EmployeeNominee_VaccineFile3 = N_VaccineFile3.FileName;
            }
            if (D_VaccineFile1 != null && D_VaccineFile1.ContentLength > 0)
            {
                vm.Employeedependent_VaccineFile1 = D_VaccineFile1.FileName;
            }
            if (D_VaccineFile2 != null && D_VaccineFile2.ContentLength > 0)
            {
                vm.Employeedependent_VaccineFile2 = D_VaccineFile2.FileName;
            }
            if (D_VaccineFile3 != null && D_VaccineFile3.ContentLength > 0)
            {
                vm.Employeedependent_VaccineFile3 = D_VaccineFile3.FileName;
            }
            if (edu_Certificate != null && edu_Certificate.ContentLength > 0)
            {
                vm.edu_Certificate = edu_Certificate.FileName;
            }
            if (PassportVisa != null && PassportVisa.ContentLength > 0)
            {
                vm.PassportVisa = PassportVisa.FileName;
            }
            if (BillVoucher != null && BillVoucher.ContentLength > 0)
            {
                vm.BillVoucher = BillVoucher.FileName;
            }
            if (AssetFileName != null && AssetFileName.ContentLength > 0)
            {
                vm.AssetFileName = AssetFileName.FileName;
            }
            if (Certificate != null && Certificate.ContentLength > 0)
            {
                vm.Certificate = Certificate.FileName;
            }
            if (Extra_FileName != null && Extra_FileName.ContentLength > 0)
            {
                vm.Extra_FileName = Extra_FileName.FileName;
            }
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
                    result = _Filesapp.Insert(vm);
                }
                else
                {
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    result = _Filesapp.Update(vm);
                }

                if (EmployeePersonalDetail_Fingerprint != null && EmployeePersonalDetail_Fingerprint.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/EmployeePersonalDetail_Fingerprint"), result[2] + EmployeePersonalDetail_Fingerprint.FileName);
                    EmployeePersonalDetail_Fingerprint.SaveAs(path);
                }
                if (FingerprintFile != null && FingerprintFile.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/FingerprintFile"), result[2] + FingerprintFile.FileName);
                    FingerprintFile.SaveAs(path);
                }
                if (PassportFile != null && PassportFile.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/EmployeePersonalDetail_PassportFile"), result[2] + PassportFile.FileName);
                    PassportFile.SaveAs(path);
                }
                if (NIDFile != null && NIDFile.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/NIDFile"), result[2] + NIDFile.FileName);
                    NIDFile.SaveAs(path);
                }
                if (VaccineFile1 != null && VaccineFile1.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/VaccineFile1"), result[2] + VaccineFile1.FileName);
                    VaccineFile1.SaveAs(path);
                }
                if (VaccineFiles2 != null && VaccineFiles2.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/VaccineFiles2"), result[2] + VaccineFiles2.FileName);
                    VaccineFiles2.SaveAs(path);
                }
                if (VaccineFile3 != null && VaccineFile3.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/VaccineFile3"), result[2] + VaccineFile3.FileName);
                    VaccineFile3.SaveAs(path);
                }
                if (DisabilityFile != null && DisabilityFile.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/DisabilityFile"), result[2] + DisabilityFile.FileName);
                    DisabilityFile.SaveAs(path);
                }
                if (SignatureFiles != null && SignatureFiles.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/SignatureFiles"), result[2] + SignatureFiles.FileName);
                    SignatureFiles.SaveAs(path);
                }
                if (FileName != null && FileName.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/FileName"), result[2] + FileName.FileName);
                    FileName.SaveAs(path);
                }
                if (N_VaccineFile1 != null && N_VaccineFile1.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/N_VaccineFile1"), result[2] + N_VaccineFile1.FileName);
                    N_VaccineFile1.SaveAs(path);
                }
                if (EmployeeNominee_VaccineFile2 != null && EmployeeNominee_VaccineFile2.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/EmployeeNominee_VaccineFile2"), result[2] + EmployeeNominee_VaccineFile2.FileName);
                    EmployeeNominee_VaccineFile2.SaveAs(path);
                }
                if (N_VaccineFile3 != null && N_VaccineFile3.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/N_VaccineFile3"), result[2] + N_VaccineFile3.FileName);
                    N_VaccineFile3.SaveAs(path);
                }
                if (D_VaccineFile1 != null && D_VaccineFile1.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/D_VaccineFile1"), result[2] + D_VaccineFile1.FileName);
                    D_VaccineFile1.SaveAs(path);
                }
                if (D_VaccineFile2 != null && D_VaccineFile2.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/D_VaccineFile2"), result[2] + D_VaccineFile2.FileName);
                    D_VaccineFile2.SaveAs(path);
                }
                if (D_VaccineFile3 != null && D_VaccineFile3.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/D_VaccineFile3"), result[2] + D_VaccineFile3.FileName);
                    D_VaccineFile3.SaveAs(path);
                }
                if (edu_Certificate != null && edu_Certificate.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/edu_Certificate"), result[2] + edu_Certificate.FileName);
                    edu_Certificate.SaveAs(path);
                }
                if (PassportVisa != null && PassportVisa.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/PassportVisa"), result[2] + PassportVisa.FileName);
                    PassportVisa.SaveAs(path);
                }
                if (BillVoucher != null && BillVoucher.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/BillVoucher"), result[2] + BillVoucher.FileName);
                    BillVoucher.SaveAs(path);
                }
                if (AssetFileName != null && AssetFileName.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/AssetFileName"), result[2] + AssetFileName.FileName);
                    AssetFileName.SaveAs(path);
                }
                if (Certificate != null && Certificate.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/Certificate"), result[2] + Certificate.FileName);
                    Certificate.SaveAs(path);
                }
                if (Extra_FileName != null && Extra_FileName.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/Extra_FileName"), result[2] + Extra_FileName.FileName);
                    Extra_FileName.SaveAs(path);
                }
                if (EmergencyContactF != null && EmergencyContactF.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files/EmergencyContactF"), result[2] + EmergencyContactF.FileName);
                    EmergencyContactF.SaveAs(path);
                }
                var mgs = result[0] + "~" + result[1];

                //return Json(mgs, JsonRequestBehavior.AllowGet);
                return RedirectToAction("Edit", new { Id = empvm.Id, mgs = mgs });
            }
            catch (Exception)
            {
                var mgs = "Fail~Data has Not updated succeessfully";
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                //return Json(mgs, JsonRequestBehavior.AllowGet);
                return RedirectToAction("Edit", new { Id = empvm.Id, mgs = mgs });
            }
        }

        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult Delete(EmployeeFilesVM vm)
        {
            string[] result = new string[6];
            try
            {
                _Filesapp = new EmployeeFilesRepo();
                result = _Filesapp.Delete(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                throw ex;
            }
        }
    }
}
