using SymOrdinary;
using SymRepository.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    public class EmployeeDetailController : Controller
    {
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        EmployeeInfoVM vm = new EmployeeInfoVM();
        public ActionResult Index(string Id = "1_107")
        {
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                Id = identity.EmployeeId;
            }
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            List<EmployeeEmergencyContactVM> emergencyContacts = new List<EmployeeEmergencyContactVM>();
            List<EmployeeReferenceVM> references = new List<EmployeeReferenceVM>();

            EmployeePermanentAddressBanglaVM permanentAddressBangla = new EmployeePermanentAddressBanglaVM();
            EmployeePermanentAddressVM permanentAddress = new EmployeePermanentAddressVM();
            EmployeePresentAddressBanglaVM presentAddressBangla = new EmployeePresentAddressBanglaVM();
            EmployeePresentAddressVM presentAddress = new EmployeePresentAddressVM();
            EmployeePersonalDetailVM personalDetail = new EmployeePersonalDetailVM();
            EmployeePersonalDetailRepo _prRepo = new EmployeePersonalDetailRepo();
            personalDetail.BloodGroup_E = "Black";
            EmployeeInfoVM vm = new EmployeeInfoVM();
            vm.personalDetail = personalDetail;
            if (Id != null)
            {
                vm = _infoRepo.SelectById(Id);
                vm.personalDetail = _prRepo.SelectByEmployee(Id);
            }
            vm.personalDetail.EmployeeId = vm.Id;

            //vm.emergencyContacts = emergencyContacts.ToList();
            //vm.references = references.ToList();

            //vm.presentAddress = new EmployeePresentAddressRepo().SelectByEmployeeId(vm.Id);
            //vm.presentAddress.EmployeeId = id;
            //vm.presentAddressBangla = new EmployeePresentAddressBanglaRepo().SelectByEmployeeId(vm.Id);
            //vm.presentAddressBangla.EmployeeId = id;
            //vm.permanentAddress = new EmployeePermanentAddressRepo().SelectByEmployeeId(vm.Id);
            //vm.permanentAddress.EmployeeId = id;
            //vm.permanentAddressBangla = new EmployeePermanentAddressBanglaRepo().SelectByEmployeeId(vm.Id);
            //vm.permanentAddressBangla.EmployeeId = id;
            //vm.fileName = id + ".jpg";
            //string directory = Server.MapPath(@"~/Files/EmployeeInfo\") + id + ".jpg";
            //if (!System.IO.File.Exists(directory))
            //{
            //    vm.fileName = "0.jpg";
            //}
            return View(vm);
        }
        public ActionResult EmloyeeDetailFull(string empid)
        {
            vm.Id = empid;
            return PartialView("_employeeDetail", vm);
        }
    }
}
