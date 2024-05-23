using SymOrdinary;
using SymRepository;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SymWebUI.Areas.MHRM.Controllers
{
    [OutputCache(NoStore = true, Duration = 0)]
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
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
            return View(vm);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserLogsVM vm, string returnUrl)
        {
            string workStationIP = "127.0.0.1";
            try
            {

                WebClient client = new WebClient();
                var realip = client.DownloadString("http://ipinfo.io");
                workStationIP = realip.Replace("\n", "").Replace(" ", "").Replace(",", ":").Replace("{", "").Replace("}", "").Replace("\"", "").ToString();
                workStationIP = workStationIP.Split(':')[1];
            }
            catch (Exception)
            {

                //throw;
            }
            try
            {
                Ordinary.CompanyLogoPath = new AppSettingsReader().GetValue("CompanyLogoPath", typeof(string)).ToString();
                string[] retResults = new string[2];
                UserInformationRepo userRepo = new UserInformationRepo();
                vm.Password = Ordinary.Encrypt(vm.Password, true);
                Tuple<bool, UserLogsVM> result = userRepo.UserLogIn(vm);
                CompanyRepo compRepo = new CompanyRepo();
                CompanyVM company = compRepo.SelectAll().FirstOrDefault();
                if (result.Item1)
                {

                    SettingRepo _setDAL = new SettingRepo();
                    bool IsESSEditPermission = _setDAL.settingValue("HRM", "IsESSEditPermission") == "Y" ? true : false;

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
                    List<UserRolesVM> roles = new UserRoleRepo().SelectAll(result.Item2.Id.ToString(), result.Item2.BranchId);
                    string[] rol = new string[roles.Count];
                    for (int i = 0; i < rol.Length; i++)
                    {
                        rol[i] = roles[i].RoleInfoId;
                    }
                    vm.SessionDate = DateTime.Now.ToString("dd-MMM-yyyy");
                    Session["SessionDate"] = vm.SessionDate.ToString();
                    Session["SessionYear"] = Convert.ToDateTime(vm.SessionDate).ToString("yyyy");
                    string roleTicket = ShampanIdentity.CreateRoleTicket(rol);


                    string basicTicket = ShampanIdentity.CreateBasicTicket(result.Item2.LogID
                                                                            ,result.Item2.FullName.Trim()
                                                                            ,company.Id.ToString().Trim()
                                                                            ,company.Name.ToString().Trim()
                                                                            ,result.Item2.BranchId.ToString()
                                                                            ,result.Item2.BranchId.ToString()
                                                                            ,vm.ComputerIPAddress
                                                                            ,"symphony.com"
                                                                            ,"BDT"
                                                                            ,Convert.ToDateTime(vm.SessionDate).ToString("yyyyMMdd")
                                                                            ,"HRM"
                                                                            ,result.Item2.EmployeeId.ToString().Trim()
                                                                            ,result.Item2.EmployeeCode.ToString().Trim()
                                                                            ,result.Item2.Id.ToString().Trim()
                                                                            ,result.Item2.IsAdmin
                                                                            ,result.Item2.IsESS
                                                                            ,result.Item2.IsHRM
                                                                            ,result.Item2.IsAttenance
                                                                            ,result.Item2.IsPayroll
                                                                            ,result.Item2.IsTAX
                                                                            ,result.Item2.IsPF
                                                                            ,result.Item2.IsGF
                                                                            ,workStationIP
                                                                            ,false
                                                                            ,false
                                                                            ,false
                                                                            ,false
                                                                            ,false
                                                                            ,false
                                                                            ,false
                                                                            ,false
                                                                            ,false
                                                                            ,false

                                                                            ,false
                                                                            ,false
                                                                            , false
                                                                            , false, false, IsESSEditPermission

                                                                            );
                    int timeOut = Convert.ToInt32(new AppSettingsReader().GetValue("COOKIE_TIMEOUT", typeof(string)));
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, FormsAuthentication.FormsCookieName, DateTime.Now, DateTime.Now.AddMinutes(30), true, basicTicket);
                    FormsAuthentication.SetAuthCookie(vm.LogID, true);
                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpContext.Response.Cookies.Add(new HttpCookie("SYMDemo", encTicket));
                    //HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                    HttpContext.Application["BasicTicket" + result.Item2.LogID] = basicTicket;
                    HttpContext.Application["RoleTicket" + result.Item2.LogID] = roleTicket;
                    DataTable sessiondt = new DataTable();

                    SymUserRoleRepo _repo = new SymUserRoleRepo();
                    sessiondt = _repo.RollByUserId(result.Item2.Id.ToString().Trim());
                    return RedirectToAction("Index", "EmployeeInfo", new { Area = "MHRM" });
                }
                else
                {
                    retResults[0] = "Fail"; retResults[1] = "User Name / Password is invalid!";
                    return RedirectToAction("Index");
                }

                Session["result"] = retResults[0] + "~" + retResults[1];
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
            return RedirectToAction("Index");
        }
    }
}
