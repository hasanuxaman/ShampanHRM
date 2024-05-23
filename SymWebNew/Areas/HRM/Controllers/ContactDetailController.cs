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
    public class ContactDetailController : Controller
    {
        //
        // GET: /HRM/ContactDetail/
        EmployeeReferenceRepo _empRefRepo;
        EmployeeInfoRepo _infoRepo;
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index(string id, string empcode, string btn)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            string currentId = "";
            
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

            if (Session["mgs"] as string != "")
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                id = identity.EmployeeId;
            }
            Label_Back: 
            if (!string.IsNullOrWhiteSpace(id))
            {
                vm = _infoRepo.SelectById(id);
            }
            else
            {
                id = currentId;
                goto Label_Back;
            }

            List<EmployeeEmergencyContactVM> emergencyContacts = new List<EmployeeEmergencyContactVM>();
            List<EmployeeReferenceVM> references = new List<EmployeeReferenceVM>();

            EmployeePermanentAddressBanglaVM permanentAddressBangla = new EmployeePermanentAddressBanglaVM();
            EmployeePermanentAddressVM permanentAddress = new EmployeePermanentAddressVM();
            EmployeePresentAddressBanglaVM presentAddressBangla = new EmployeePresentAddressBanglaVM();
            EmployeePresentAddressVM presentAddress = new EmployeePresentAddressVM();

            //vm.emergencyContacts = emergencyContacts.ToList();
            //vm.references = references.ToList();

            if (id != null)
            {
                vm.presentAddress = new EmployeePresentAddressRepo().SelectByEmployeeId(vm.Id);
                vm.presentAddress.EmployeeId = id;
                vm.presentAddressBangla = new EmployeePresentAddressBanglaRepo().SelectByEmployeeId(vm.Id);
                vm.presentAddressBangla.EmployeeId = id;
                vm.permanentAddress = new EmployeePermanentAddressRepo().SelectByEmployeeId(vm.Id);
                vm.permanentAddress.EmployeeId = id;
                vm.permanentAddressBangla = new EmployeePermanentAddressBanglaRepo().SelectByEmployeeId(vm.Id);
                vm.permanentAddressBangla.EmployeeId = id;
            }
            return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public JsonResult PresentAddress(EmployeeInfoVM evm = null)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();
            EmployeePresentAddressRepo presentAddressRepo = new EmployeePresentAddressRepo();
            EmployeePermanentAddressRepo permanentRepo = new EmployeePermanentAddressRepo();
            EmployeePresentAddressVM vm = new EmployeePresentAddressVM();
            EmployeePermanentAddressVM vmP = new EmployeePermanentAddressVM();
            vm = evm.presentAddress;
            string[] result = new string[6];


            if (vm.AddressType == "permanent")
            {

                vmP.EmployeeId = vm.EmployeeId;
                vmP.Address = vm.Address;
                vmP.District = vm.District;
                vmP.Division = vm.Division;
                vmP.Country = vm.Country;
                vmP.City = vm.City;
                vmP.PostalCode = vm.PostalCode;
                vmP.Phone = vm.Phone;
                vmP.Fax = vm.Fax;
                vmP.Mobile = vm.Mobile;
                vmP.FileName = vm.FileName;
                vmP.Remarks = vm.Remarks;
                vmP.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vmP.CreatedBy = identity.Name;
                vmP.CreatedFrom = identity.WorkStationIP;
                vmP.EmployeeId = evm.Id;
                result = permanentRepo.Insert(vmP);

            }
            else
            {
                if (vm.Id <= 0)
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    vm.EmployeeId = evm.Id;
                    result = presentAddressRepo.Insert(vm);
                }
                else
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    result = presentAddressRepo.Update(vm);
                }
            }


            var mgs = result[0] + "~" + result[1];
            Session["mgs"] = "mgs";
            return Json(mgs, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index", new { Id = evm.Id, mgs = mgs });
        }
        [HttpPost]
        public ActionResult PresentAddressBangla(EmployeeInfoVM evm)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();

            EmployeePresentAddressBanglaRepo app = new EmployeePresentAddressBanglaRepo();
            EmployeePresentAddressBanglaVM vm = new EmployeePresentAddressBanglaVM();
            vm = evm.presentAddressBangla;
            string[] result = new string[6];
            if (vm.Id <= 0)
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.EmployeeId = evm.Id;
                result = app.Insert(vm);
            }
            else
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                result = app.Update(vm);
            }
            var mgs = result[0] + "~" + result[1];
            Session["mgs"] = "mgs";
            return RedirectToAction("Index", new { Id = evm.Id, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public JsonResult PermanentAddress(EmployeeInfoVM evm = null, HttpPostedFileBase permanentAddressF = null, string EmployeeId = null)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();
            string mgs = "";
            EmployeePermanentAddressRepo permanentRepo = new EmployeePermanentAddressRepo();
            EmployeePermanentAddressVM vm = new EmployeePermanentAddressVM();
            vm = evm.permanentAddress;

            if (permanentAddressF != null && permanentAddressF.ContentLength > 0)
            {
                vm.FileName = permanentAddressF.FileName;
            }
            string[] result = new string[6];
            if (vm.Id <= 0)
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.EmployeeId = evm.Id;
                result = permanentRepo.Insert(vm);
            }
            else
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                result = permanentRepo.Update(vm);
            }
            if (permanentAddressF != null && permanentAddressF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/PermanentAddress"), result[2] + permanentAddressF.FileName);
                permanentAddressF.SaveAs(path);
            }
            mgs = result[0] + "~" + result[1];
            Session["mgs"] = "mgs";
            return Json(mgs, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index", new { Id = evm.Id, mgs = mgs });
        }
        [HttpPost]
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult PermanentAddressBangla(EmployeeInfoVM evm)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();
            EmployeePermanentAddressBanglaRepo app = new EmployeePermanentAddressBanglaRepo();
            EmployeePermanentAddressBanglaVM vm = new EmployeePermanentAddressBanglaVM();
            vm = evm.permanentAddressBangla;
            string[] result = new string[6];
            if (vm.Id <= 0)
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.EmployeeId = evm.Id;
                result = app.Insert(vm);
            }
            else
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                result = app.Update(vm);
            }
            var mgs = result[0] + "~" + result[1];
            Session["mgs"] = "mgs";
            return RedirectToAction("Index", new { Id = evm.Id, mgs = mgs });
        }
        public ActionResult _index(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            EmployeeReferenceRepo _empRefApp = new EmployeeReferenceRepo();
            var getAllData = _empRefApp.SelectAllByEmployee(Id);


            IEnumerable<EmployeeReferenceVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Relation.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.Phone.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.Mobile.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }



            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeReferenceVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Relation :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Phone :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.Mobile :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                             Convert.ToString(c.Id)
                             , c.Name //+ "~" + Convert.ToString(c.Id)
                             , c.Relation
                             , c.Phone
                             , c.Mobile 
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Reference(string EmployeeId, int Id)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.referenceVM = new EmployeeReferenceRepo().SelectById(Id);

            }
            else
            {
                EmployeeReferenceVM refrvm = new EmployeeReferenceVM();
                refrvm.EmployeeId = EmployeeId;
                vm.referenceVM = refrvm;
            }
            return PartialView("_editReference", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public JsonResult Reference(EmployeeInfoVM vm, HttpPostedFileBase ReferenceF)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();
            string[] result = new string[6];
            _empRefRepo = new EmployeeReferenceRepo();
            EmployeeReferenceVM refrvm = new EmployeeReferenceVM();
            refrvm = vm.referenceVM;
            if (ReferenceF != null && ReferenceF.ContentLength > 0)
            {
                refrvm.FileName = ReferenceF.FileName;
            }
            if (refrvm.Id <= 0)
            {
                refrvm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                refrvm.CreatedBy = identity.Name;
                refrvm.CreatedFrom = identity.WorkStationIP;
                refrvm.IsActive = true;
                result = _empRefRepo.Insert(refrvm);
            }
            else
            {
                refrvm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                refrvm.LastUpdateBy = identity.Name;
                refrvm.LastUpdateFrom = identity.WorkStationIP;
                result = _empRefRepo.Update(refrvm);
            }
            if (ReferenceF != null && ReferenceF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Reference"), result[2] + ReferenceF.FileName);
                ReferenceF.SaveAs(path);
            }
            var mgs = result[0] + "~" + result[1];
            Session["mgs"] = "mgs";
            return Json(mgs, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index", new { Id = refrvm.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult ReferenceDelete(string ids)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "delete").ToString();

            _empRefRepo = new EmployeeReferenceRepo();
            EmployeeReferenceVM vm = new EmployeeReferenceVM();

            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _empRefRepo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
