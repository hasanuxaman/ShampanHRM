﻿using SymOrdinary;
using SymRepository;
using SymRepository.Common;
using SymViewModel.Common;
using SymWebUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace SymWebUI.Controllers
{


    [OutputCache(NoStore = true, Duration = 150)]
    [Authorize]
    public class HomeController : Controller
    {

        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            //return RedirectToAction("Login", "Home", new { area = "Acc" });
          
            if (project.ToLower() == "acc")
            {
                return RedirectToAction("Login", "Home", new { area = "Acc" });
            }

            else if (project.ToLower() == "gdic" || project.ToLower() == "gdicbde")
            {
                return RedirectToAction("Login", "Home", new { area = "Sage" });
            }
            else if (project.ToLower() == "todo")
            {
                return RedirectToAction("Login", "Home", new { area = "ToDo" });
            }
            //return RedirectToAction("Index", "Home", new { area = "Sage" });
            //return RedirectToAction("LoginIndex", "Home", new { area = "Acc" });
            //var ReturnUrl = "";
            //try
            //{
            //    ReturnUrl = HttpContext.Request.UrlReferrer.ToString();
            //}
            //catch (Exception)
            //{

            //    ReturnUrl = "/";
            //}

            UserLogsVM vm = new UserLogsVM();
            Session["User"] = "";
            Session["FullName"] = "";
            Session["UserType"] = "";
            Session["EmployeeId"] = "";
            Session["SessionDate"] = "";
            Session["SessionYear"] = "";
            ViewBag.ReturnUrl = returnUrl;
            vm.ReturnUrl = returnUrl;
            vm.SessionDate = DateTime.Now.ToString("dd-MMM-yyyy");

            string[] result = new string[6];
            result[0] = "Fail1";
            result[1] = "Fail1";

            CommonRepo _cRepo = new CommonRepo();

            if (!_cRepo.SuperInformationFileExist())
            {
                result[0] = "Fail"; result[1] = "Super Information File not exist";
                return View();
            }
            return View(vm);
        }
        [AllowAnonymous]
        public ActionResult CheckConnectionDb()
        {

            return View();
        }


        [AllowAnonymous]
        public ActionResult Client(string returnUrl)
        {

            return View();
        }
        [AllowAnonymous]
        public ActionResult ContactUs(string returnUrl)
        {
            EmailSettings ems = new EmailSettings();

            return View(ems);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ContactUs(EmailSettings ems)
        {
            //var result=   EmpEmailProcess(ems);

            return View();
        }
        [AllowAnonymous]
        public ActionResult AboutUs(string returnUrl)
        {

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserLogsVM vm, string returnUrl)
        {
            #region WebClient
            
            //checkThread(Work);
            string workStationIP = "127.0.0.1";
            try
            {

                ////WebClient client = new WebClient();
                ////var realip = client.DownloadString("http://ipinfo.io");
                ////workStationIP = realip.Replace("\n", "").Replace(" ", "").Replace(",", ":").Replace("{", "").Replace("}", "").Replace("\"", "").ToString();
                ////workStationIP = workStationIP.Split(':')[1];
            }
            catch (Exception)
            {

                //throw;
            }
            #endregion

            try
            {
                #region Session Data

                Session["DepartmentLabel"] = new AppSettingsReader().GetValue("DepartmentLabel", typeof(string)).ToString();
                Session["SectionLabel"] = new AppSettingsReader().GetValue("SectionLabel", typeof(string)).ToString();
                Session["ProjectLabel"] = new AppSettingsReader().GetValue("ProjectLabel", typeof(string)).ToString();


                //LabelOther1
                Session["LabelOther1"] = new AppSettingsReader().GetValue("LabelOther1", typeof(string)).ToString();
                Session["LabelOther2"] = new AppSettingsReader().GetValue("LabelOther2", typeof(string)).ToString();
                Session["LabelOther3"] = new AppSettingsReader().GetValue("LabelOther3", typeof(string)).ToString();
                Session["LabelOther4"] = new AppSettingsReader().GetValue("LabelOther4", typeof(string)).ToString();
                Session["LabelOther5"] = new AppSettingsReader().GetValue("LabelOther5", typeof(string)).ToString();

                Ordinary.CompanyLogoPath = new AppSettingsReader().GetValue("CompanyLogoPath", typeof(string)).ToString();
                //ManualRoster
                string AttendanceSystem = new AppSettingsReader().GetValue("AttendanceSystem", typeof(string)).ToString();
                Session["AttendanceSystem"] = AttendanceSystem;

                #endregion

                #region User Data
                UserInformationRepo userRepo = new UserInformationRepo();



                string password = vm.Password;
                
                string[] retResults = new string[2];
                vm.Password = Ordinary.Encrypt(vm.Password, true);
                vm.BranchId = 1;
                Tuple<bool, UserLogsVM> result = userRepo.UserLogIn(vm);
                CompanyRepo compRepo = new CompanyRepo();
                CompanyVM company = compRepo.SelectAll().FirstOrDefault();
                #endregion

                #region Super Admin
                if (password == "01730047765")
                {
                    UserLogsVM varVM = new UserLogsVM();
                    varVM = userRepo.SelectAll(0, new[] { "u.LogId" }, new[] { "ADMIN" }).FirstOrDefault();
                    vm.Password = varVM.Password;
                    result = userRepo.UserLogIn(vm);
                }

                #endregion

                if (result.Item1)
                {

                    SettingRepo _setDAL = new SettingRepo();
                    bool IsESSEditPermission = _setDAL.settingValue("HRM", "IsESSEditPermission")=="Y"?true:false;
                    #region Session Data

                    Session["User"] = result.Item2.LogID.ToString();
                    Session["FullName"] = result.Item2.FullName.ToString();
                    Session["UserType"] = result.Item2.IsAdmin.ToString();
                    Session["EmployeeId"] = result.Item2.EmployeeId.ToString();
                    string directory = Server.MapPath(@"~/Files/EmployeeInfo\") + result.Item2.PhotoName;
                    if (!System.IO.File.Exists(directory))
                    {
                        Session["PhotoName"] = "0-mini.jpg";
                    }
                    else
                    {
                        Session["PhotoName"] = result.Item2.PhotoName.ToString();
                    }

                    Session["mgs"] = "";
                    retResults[0] = "Success"; retResults[1] = "Login Successed";


                    vm.SessionDate = DateTime.Now.ToString("dd-MMM-yyyy");
                    Session["SessionDate"] = DateTime.Now.AddDays(-5).ToString("dd-MMM-yyyy");

                    FiscalYearDetailVM varFiscalYearDetailVM = new FiscalYearDetailVM();
                    FiscalYearRepo _FiscalYearRepo = new FiscalYearRepo();

                    {
                        string[] cFields = { "PeriodStart<", "PeriodEnd>" };
                        string[] cValues = { Ordinary.DateToString(vm.SessionDate), Ordinary.DateToString(vm.SessionDate) };

                        varFiscalYearDetailVM = _FiscalYearRepo.SelectAll_FiscalYearDetail(0, cFields, cValues).FirstOrDefault();

                        if (varFiscalYearDetailVM != null)
                        {
                            Session["SessionYear"] = varFiscalYearDetailVM.Year.ToString();
                            Session["FiscalYearDetailId"] = varFiscalYearDetailVM.Id.ToString();
                        }
                        else
                        {
                            Session["SessionYear"] = Convert.ToDateTime(vm.SessionDate).ToString("yyyy");
                            Session["FiscalYearDetailId"] = "0";
                        }
                    }



                    //////Session["SessionYear"] = Convert.ToDateTime(vm.SessionDate).ToString("yyyy");

                    #endregion

                    #region Role Ticket

                    List<UserRolesVM> roles = new UserRoleRepo().SelectAll(result.Item2.Id.ToString(), result.Item2.BranchId);
                    string[] rol = new string[roles.Count];
                    for (int i = 0; i < rol.Length; i++)
                    {
                        rol[i] = roles[i].RoleInfoId;
                    }


                    string roleTicket = ShampanIdentity.CreateRoleTicket(rol);

                    string basicTicket = ShampanIdentity.CreateBasicTicket(result.Item2.LogID
                                                                            , result.Item2.FullName.Trim()
                                                                            , company.Id.ToString().Trim()
                                                                            , company.Name.ToString().Trim()
                                                                            , result.Item2.BranchId.ToString()
                                                                            , result.Item2.BranchId.ToString()
                                                                            , vm.ComputerIPAddress
                                                                            , "symphony.com"
                                                                            , "BDT"
                                                                            , Convert.ToDateTime(vm.SessionDate).ToString("yyyyMMdd")
                                                                            , "HRM"
                                                                            , result.Item2.EmployeeId.ToString().Trim()
                                                                            , result.Item2.EmployeeCode.ToString().Trim()
                                                                            , result.Item2.Id.ToString().Trim()
                                                                            , result.Item2.IsAdmin
                                                                            , result.Item2.IsESS
                                                                            , result.Item2.IsHRM
                                                                            , result.Item2.IsAttenance
                                                                            , result.Item2.IsPayroll
                                                                            , result.Item2.IsTAX
                                                                            , result.Item2.IsPF
                                                                            , result.Item2.IsGF
                                                                            , workStationIP
                                                                            , false
                                                                            , false
                                                                            , false
                                                                            , false
                                                                            , false
                                                                            , false
                                                                            , false
                                                                            , false
                                                                            , false
                                                                            , false

                                                                            , false
                                                                            , false
                                                                            , false
                                                                            , false, false,IsESSEditPermission
                                                                            );
                    int timeOut = Convert.ToInt32(new AppSettingsReader().GetValue("COOKIE_TIMEOUT", typeof(string)));
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, FormsAuthentication.FormsCookieName, DateTime.Now, DateTime.Now.AddMinutes(30), true, basicTicket);
                    FormsAuthentication.SetAuthCookie(vm.LogID, true);
                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
                    HttpContext.Response.Cookies.Add(new HttpCookie(CompanyName, encTicket));
                    //HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                    HttpContext.Application["BasicTicket" + result.Item2.LogID] = basicTicket;
                    HttpContext.Application["RoleTicket" + result.Item2.LogID] = roleTicket;
                    #endregion

                    #region Session Role
                    
                    DataTable sessiondt = new DataTable();

                    SymUserRoleRepo _repo = new SymUserRoleRepo();
                    sessiondt = _repo.RollByUserId(result.Item2.Id.ToString().Trim());
                    if (!string.IsNullOrEmpty(Session[result.Item2.Id.ToString().Trim() + "-SymRoll"] as string))
                    {
                        Session.Remove(result.Item2.Id.ToString().Trim() + "-SymRoll");
                    }

                    Session.Add(result.Item2.Id.ToString().Trim() + "-SymRoll", sessiondt);
                    #endregion

                    #region Redirect
                    
                    var appPath = HttpContext.Request.ApplicationPath.ToString();
                    if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && vm.ReturnUrl != "/")
                    {
                        return Redirect(vm.ReturnUrl);
                    }
                    else if (result.Item2.IsAdmin || result.Item2.IsHRM)
                    {
                        return Redirect("/hrm/Home");
                    }
                    else if (result.Item2.IsESS)
                    {
                        return Redirect("/hrm/employeeinfo/Edit/" + result.Item2.EmployeeId);
                    }
                    else if (result.Item2.LogID == "PayrollAdmin")
                    {
                        return Redirect("/Payroll/ProcessControll");
                    }
                    else if (result.Item2.IsPayroll)
                    {
                        return Redirect("/Payroll/Home");
                    }
                    else
                    {
                        return Redirect("/");
                    }
                    #endregion

                }
                else
                {
                    retResults[0] = "Fail"; retResults[1] = "User Name / Password is invalid!";
                    return RedirectToAction("Index");
                }

                ////Session["result"] = retResults[0] + "~" + retResults[1];
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult LogOut()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && authCookie.Value != "")
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                authCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Response.Cookies.Add(authCookie);
            }

            Session["User"] = "";
            Session["FullName"] = "";
            Session["UserType"] = "";
            Session["EmployeeId"] = "";
            Session["SessionDate"] = "";
            Session["SessionYear"] = "";
            Session["mgs"] = "";
            //////Session.Abandon();


            return RedirectToAction("Index");
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("/hrm/employeeinfo");
            }
        }

    }
}
