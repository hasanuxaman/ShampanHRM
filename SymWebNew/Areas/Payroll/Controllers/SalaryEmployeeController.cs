using SymOrdinary;
using SymRepository.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Payroll.Controllers
{
    public class SalaryEmployeeController : Controller
    {
        //
        // GET: /Payroll/SalaryEmployee/
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        SalaryEmployeeRepo _repo = new SalaryEmployeeRepo();

        public ActionResult Index()
        {
            SalaryEmployeeVM vm = new SalaryEmployeeVM();
            return View(vm);
        }


        public ActionResult _index(SalaryEmployeeVM vm)
        {
            ModelState.Clear();
            #region Declare Variable
            #endregion
            List<SalaryEmployeeVM> VMs = new List<SalaryEmployeeVM>();
            string[] conFields = { "ve.Code>", "ve.Code<", "ve.DepartmentId", "ve.ProjectId", "ve.SectionId", "ve.DesignationId", "se.FiscalYearDetailId" };
            string[] conValues = { vm.CodeF, vm.CodeT, vm.DepartmentId, vm.ProjectId, vm.SectionId, vm.DesignationId, vm.FiscalYearDetailId.ToString() };

            VMs = _repo.SelectAll(0, conFields, conValues);
            return PartialView("_index", VMs);
        }


        [HttpPost]
        public ActionResult Create(List<SalaryEmployeeVM> VMs)
        {
            string[] result = new string[6];
            string mgs = "";
            SalaryEmployeeVM vm = new SalaryEmployeeVM();

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;

            VMs = VMs.Where(c => c.IsEmployeeChecked == true).ToList();

            //////if (VMs.Count == 0)
            //////{
            //////    return Json("Fail~Please Select Employee First!", JsonRequestBehavior.AllowGet);
            //////}


            result = _repo.Update(VMs, vm);
            mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }

    }
}
