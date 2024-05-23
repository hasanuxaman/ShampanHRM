using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    [Authorize]
    public class EmployeeImmigrationController : Controller
    {
        //
        // GET: /HRM/EmployeeImmigration/
        EmployeeImmigrationRepo _immApp;
         SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {            
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "index").ToString();

            _immApp = new EmployeeImmigrationRepo();
            return View(_immApp.SelectAll());
        }


        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(EmployeeImmigrationVM vm)
        {
            string[] retResults = new string[6];
            _immApp = new EmployeeImmigrationRepo();
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                retResults = _immApp.Insert(vm);
                Session["result"] = retResults[0] + "~" + retResults[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";

                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult Edit()
        {
            return View();
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(EmployeeImmigrationVM vm)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            string[] Results = new string[6];
            _immApp = new EmployeeImmigrationRepo();
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                Results = _immApp.Insert(vm);
                Session["result"] = Results[0] + "~" + Results[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";

                return RedirectToAction("Index");
            }
        }
    }
}
