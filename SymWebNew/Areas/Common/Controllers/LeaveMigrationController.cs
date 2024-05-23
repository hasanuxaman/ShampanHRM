using SymOrdinary;
using SymRepository.Common;
using SymRepository.Enum;
using SymRepository.HRM;
using SymRepository.Leave;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Leave;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Windows.Forms;

namespace SymWebUI.Areas.Common.Controllers
{
    [Authorize]
    public class LeaveMigrationController : Controller
    {
        //
        // GET: /Common/LeaveMigration/
        EmployeeLeaveStructureRepo repo = new EmployeeLeaveStructureRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_2", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            return View();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult Migration(string year, string ids)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_2", "index").ToString();
            Session["permission"] = permission;
            string[] a = ids.Split('~');
            EmployeeLeaveStructureVM vm = new EmployeeLeaveStructureVM();
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            //vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //vm.CreatedBy = Identity.Name;
            //vm.CreatedFrom = Identity.WorkStationIP;
            //vm.LeaveStructureId = Convert.ToInt32(leaveStructureId);
            vm.employeeIds = a;
            vm.LeaveYear = Convert.ToInt32(year);


            ShampanIdentityVM siVM = new ShampanIdentityVM();
            siVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            siVM.LastUpdateBy = identity.Name;
            siVM.LastUpdateFrom = identity.WorkStationIP;

            siVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            siVM.CreatedBy = identity.Name;
            siVM.CreatedFrom = identity.WorkStationIP;
            siVM.BranchId = identity.BranchId;

            string[] result = new string[6];
            //result = repo.Insert(vm);
            result = repo.LeaveMigrationInsert(vm, siVM);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult OpeningBalance()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_1", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            return View();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult OpeningBalanceDownload(HttpPostedFileBase file, string vyear)
        {
            string[] result = new string[6];
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_1", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Common/Home");
                }
                string FileName = "Download.xlsx";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                //string fullPath = @"C:\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }
                new EmployeeLeaveRepo().ExportExcelFile(fullPath, FileName, vyear);
                Session["result"] = "Success~Data Download Successfully";
                return Redirect("/Files/Export/" + FileName);
                //return Redirect("C:/" + FileName);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Download";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("OpeningBalance");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult OpeningBalance(HttpPostedFileBase file)
        {
            string[] result = new string[6];
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                var permission = _reposur.SymRoleSession(identity.UserId, "1_1", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Common/Home");
                }
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + file.FileName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(fullPath);
                }

                EmployeeLeaveVM vm = new EmployeeLeaveVM();
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                new EmployeeLeaveRepo().ImportExcelFile(fullPath, vm);
                Session["result"] = "Success~Data Upload Successfully";
                return RedirectToAction("Index");
                //return RedirectToAction("OpeningBalance");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Upload";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("OpeningBalance");
            }
        }


        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult OpeningBalanceSingle(string empcode, string vyear, string test)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_3", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            EmployeeLeaveRepo elrepo = new EmployeeLeaveRepo();

            EmployeeLeaveVM vm = new EmployeeLeaveVM();
            List<EmployeeLeaveStructureVM> vms = new List<EmployeeLeaveStructureVM>();
            if (!string.IsNullOrWhiteSpace(empcode) && !string.IsNullOrWhiteSpace(vyear) && string.IsNullOrWhiteSpace(test))
            {
                vms = elrepo.SelectAllOpening(empcode, vyear);
                vm.EmployeeLeaveStructures = vms;
                return PartialView("_employeeLeaveStructures", vm);
            }
            else
            {
                return View(vm);
            }
            //return View();
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult OpeningBalanceSingle(EmployeeLeaveVM vm)
        {
            string[] result = new string[6];
            EmployeeLeaveRepo elrepo = new EmployeeLeaveRepo();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {

                result = elrepo.UpdateLeaveBalance(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
                //return RedirectToAction("OpeningBalanceSingle");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("OpeningBalanceSingle");
            }
        }
        [HttpPost]
        public ActionResult OpeningBalanceStructureSingle(EmployeeLeaveStructureVM vm)
        {
            string[] result = new string[6];
            EmployeeLeaveVM elvm = new EmployeeLeaveVM();

            EmployeeLeaveRepo elrepo = new EmployeeLeaveRepo();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {
                result = elrepo.UpdateLeaveStructureBalance(vm);
                Session["result"] = result[0] + "~" + result[1];
                //return RedirectToAction("Index");
                return RedirectToAction("OpeningBalanceSingle", new RouteValueDictionary(new { controller = "LeaveMigration", action = "OpeningBalanceSingle", empcode = vm.EmpCode, vyear = vm.LeaveYear, test = "test" }));

                //return RedirectToAction("OpeningBalanceSingle", new RouteValueDictionary(new { controller = "LeaveMigration", action = "OpeningBalanceSingle"}));

                //return RedirectToAction("OpeningBalanceSingle", elvm);
                //var mgs = result[0] + "~" + result[1];
                //Session["mgs"] = "mgs";
                //return Json(mgs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                //Session["result"] = "Fail~Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                var mgs = result[0] + "~" + result[1];
                Session["mgs"] = "mgs";
                return Json(mgs, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("OpeningBalanceSingle");
            }
        }
    }
}
