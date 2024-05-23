using Newtonsoft.Json.Linq;
using SymOrdinary;
using SymRepository.Attendance;
using SymRepository.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class AttendanceMigrationController : Controller
    {
        //
        // GET: /Common/AttendanceMigration/
        private static Thread thread;
        string autoAttendanceMigration;

        AttendanceMigrationRepo repo = new AttendanceMigrationRepo();
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult SelectToInsertView()
        {
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
            //Session["permission"] = _reposur.SymRollSession(identity.UserId, "70002", "report").ToString();
            return View("SelectToInsert", vm);
        }

        [HttpGet]
        public ActionResult SelectToInsert(string empCode, string attnDate, string attnTime)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            AttendanceMigrationRepo _repo = new AttendanceMigrationRepo();
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                vm.EmployeeCode = empCode;
                vm.AttendanceDate = attnDate;
                vm.AttendanceTime = attnTime;

                result = _repo.SelectToInsert(vm);
                Session["result"] = result[0] + "~" + result[1];
                return View(vm);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(vm);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult SelectFromDownloadData()
        {
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
            //Session["permission"] = _reposur.SymRollSession(identity.UserId, "70002", "report").ToString();
            return View("SelectFromDownloadData", vm);
        }

        //codeF
        //codeT

        [HttpGet]
        public ActionResult SelectFromDownloadDataProcess(string attnDateFrom, string attnDateTo, string employeeCode)
        {
            string[] result = new string[6];
            //result[0] = "Fail";//Success or Fail
            //result[1] = "Fail Message";// Success or Fail Message
            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                AttendanceMigrationRepo _repo = new AttendanceMigrationRepo();
                AttendanceMigrationVM vm = new AttendanceMigrationVM();
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                vm.AttendanceDateFrom = attnDateFrom;
                vm.AttendanceDateTo = attnDateTo;

                if (employeeCode.Contains("~"))
                {
                    vm.EmployeeCode = employeeCode.Split('~')[0];
                }
                result = _repo.SelectFromDownloadData(vm);

                //thread = new Thread(unused => _repo.SelectFromDownloadData(vm));
                //thread.Start();

                //bool ThreadStop = false;
                //ThreadStop = true;
                //if (ThreadStop==true)
                //{
                //    thread.Abort();
                //}

                //Session["result"] = result[0] + "~" + result[1];
                //return Redirect("/Common/Home");
                return Json(result[1], JsonRequestBehavior.AllowGet);
                //return View("SelectFromDownloadData", vm);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                //return Redirect("/Common/Home");
                return View("SelectFromDownloadData");
            }
        }


        [HttpGet]
        public ActionResult StartAutoMigration(string attnDateFrom, string attnDateTo)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            AttendanceMigrationRepo _repo = new AttendanceMigrationRepo();
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
            try
            {


                #region Settings
                SettingsVM sVM = new SettingsVM();
                SettingRepo _sRepo = new SettingRepo();
                sVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                sVM.CreatedBy = identity.Name;
                sVM.CreatedFrom = identity.WorkStationIP;

                sVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                sVM.LastUpdateBy = identity.Name;
                sVM.LastUpdateFrom = identity.WorkStationIP;

                sVM.BranchId = Convert.ToInt32(identity.BranchId);
                string autoAttendanceMigration = "";

                sVM.SettingGroup = "Attendance";
                sVM.SettingName = "AutoAttendanceMigration";
                sVM.SettingValue = "Y";
                #endregion Settings

                _sRepo.settingsDataUpdate(sVM);

                thread = new Thread(unused => AutoMigration(attnDateFrom, attnDateTo));
                thread.Start();

                result[0] = "Succes";
                //result[1] = "Auto Attendance Migration Successfully!";

                return Json(result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                //return Redirect("/Common/Home");
                return View("SelectFromDownloadData");
            }
        }

        [HttpGet]
        public ActionResult StopAutoMigration()
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                #region Settings
                SettingsVM sVM = new SettingsVM();
                SettingRepo _sRepo = new SettingRepo();
                sVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                sVM.CreatedBy = identity.Name;
                sVM.CreatedFrom = identity.WorkStationIP;

                sVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                sVM.LastUpdateBy = identity.Name;
                sVM.LastUpdateFrom = identity.WorkStationIP;

                sVM.BranchId = Convert.ToInt32(identity.BranchId);

                sVM.SettingGroup = "Attendance";
                sVM.SettingName = "AutoAttendanceMigration";
                sVM.SettingValue = "N";
                #endregion Settings

                result = _sRepo.settingsDataUpdate(sVM);
                if (result[0] != "Fail")
                {
                    result[1] = "Auto Attendance Migration Stopped Successfully";

                }
                else
                {
                    result[1] = "Auto Attendance Migration Not Stopped Successfully";

                }

                return Json(result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("SelectFromDownloadData");
            }
        }


        public void AutoMigration(string attnDateFrom, string attnDateTo)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            AttendanceMigrationRepo _repo = new AttendanceMigrationRepo();
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
            SettingRepo _sRepo = new SettingRepo();

            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;

            vm.AttendanceDateFrom = attnDateFrom;
            vm.AttendanceDateTo = attnDateTo;

            try
            {
                autoAttendanceMigration = _sRepo.settingValue("Attendance", "AutoAttendanceMigration");
                while (autoAttendanceMigration.ToLower() == "y")
                {
                    autoAttendanceMigration = _sRepo.settingValue("Attendance", "AutoAttendanceMigration");
                    result = _repo.SelectFromDownloadData(vm);
                }
            }
            catch (Exception ex)
            {

                // throw ex;
            }

            autoAttendanceMigration = _sRepo.settingValue("Attendance", "AutoAttendanceMigration");
            if (autoAttendanceMigration.ToLower() == "n")
            {
                thread.Abort();
            }

        }


        [HttpGet]
        public ActionResult DeleteProcess(string attnDateFrom, string attnDateTo, string employeeCode)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            AttendanceMigrationRepo _repo = new AttendanceMigrationRepo();
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                vm.AttendanceDateFrom = attnDateFrom;
                vm.AttendanceDateTo = attnDateTo;

                if (employeeCode.Contains("~"))
                {
                    vm.EmployeeCode = employeeCode.Split('~')[0];
                }



                result = _repo.DeleteProcess(vm);
                return Json(result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("SelectFromDownloadData");
            }
        }



    }


}
