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
    public class EmergencyContactController : Controller
    {
        //
        // GET: /PSS/Color/
        EmployeeEmergencyContactRepo _emergencyApp;
        //public ActionResult Index(int Id)
        //{
        //    EmployeeInfoVM empVM = new EmployeeInfoVM();
        //    empVM.Id=1;
        //   // return View(new EmployeeEmergencyContactRepo().SelectAll());
        //    return View(empVM);
        //}

        //[Authorize(Roles = "Admin,EmgcyContact")]
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
            EmployeeEmergencyContactVM vm = new EmployeeEmergencyContactVM ();
            _emergencyApp = new EmployeeEmergencyContactRepo();
            vm = empvm.emergencyContactVM;
            vm.EmployeeId=empvm.Id;
            if (EmergencyContactF != null && EmergencyContactF.ContentLength > 0)
            {
                vm.FileName = EmergencyContactF.FileName;
            }
            Session["mgs"] = "mgs";
            try
            {
                if (vm.Id<=0)
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
    }
}
