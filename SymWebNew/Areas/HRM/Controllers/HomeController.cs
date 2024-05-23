using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    public class chart 
    {
        public string Section { get; set; }
        public string Gender { get; set; }
        public int Person { get; set; }
    }
    public class chartMulti
    {
        public string Gender { get; set; }
        public List<int> Persons = new List<int>();
    }
   
    [Authorize]
    public class HomeController : Controller
    {
        #region Chart
        HomeRepo _repo=new HomeRepo();
        public ActionResult TotalEmployeeGender()
        {
            List<chart> chs = new List<chart>();
            chart ch = new chart();
            var emp = _repo.SelectAllEmployee();
            foreach (var a in emp) {
                ch = new chart();
                ch.Gender = a.Gender;
                ch.Person = a.Person;
                chs.Add(ch);
            }
            return Json(chs, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SectionGenderPerson()
        {
            List<string> Sections = new List<string>();
            List<string> Genders = new List<string>();
            List<object> objectList = new List<object>();
            List<chartMulti> chs = new List<chartMulti>();
            chartMulti ch = new chartMulti();
            DataSet ds = new DataSet();
            DataTable returndt = new DataTable();
            ds = _repo.TotalEmployeeSectionGender();

            foreach (DataRow sec1 in ds.Tables[1].Rows)
            {
                Sections.Add(sec1["Section"].ToString());
            }

            foreach (DataRow gend in ds.Tables[0].Rows)
            {
                Genders.Add(gend["Gender"].ToString());

            }
            foreach (DataRow gen in ds.Tables[0].Rows) 
            {
                ch = new chartMulti();
                ch.Gender = gen["Gender"].ToString(); 
                foreach (DataRow sec in ds.Tables[1].Rows)
                {
                    int c = Convert.ToInt32(_repo.TotalEmployeeSectionGender(sec["Section"].ToString(), ch.Gender));
                    ch.Persons.Add(c);
                  } 
                    chs.Add(ch);
            }
            objectList.Add(Sections);
            objectList.Add(Genders);
            objectList.Add(chs);
            return Json(objectList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult JoinDate(string Fromdate,string Todate,string Year) {
            ViewBag.Fromdate = Fromdate;
            ViewBag.Todate = Todate;
            return View("ChartJoinDate");
        }
        public ActionResult JoinDateChart(string Fromdate, string Todate, string Year)
        {
            List<string> Sections = new List<string>();
            List<string> Genders = new List<string>();
            List<object> objectList = new List<object>();
            List<chartMulti> chs = new List<chartMulti>();
            chartMulti ch = new chartMulti();
            DataSet ds = new DataSet();
            DataTable returndt = new DataTable();
            ds = _repo.TotalEmployeeJoinDate(Fromdate, Todate);
            foreach (DataRow sec1 in ds.Tables[0].Rows)
            {
                Sections.Add(sec1["Section"].ToString());
            }
            return Json(objectList, JsonRequestBehavior.AllowGet);
        }
        #endregion Chart
        #region Index
        EmployeeInfoRepo _infoRepo;
        public ActionResult MenuIndex()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        #region Transfer
        [HttpGet]
        public ActionResult Transfer()
        {
            return View();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Transfer(EmployeeInfoVM vm, HttpPostedFileBase TransferF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] retResults = new string[6];
            EmployeeTransferRepo empTrnApp = new EmployeeTransferRepo();
            EmployeeTransferVM trnvm = new EmployeeTransferVM();
            trnvm = vm.transferVM;
            if (TransferF != null && TransferF.ContentLength > 0)
            {
                trnvm.FileName = TransferF.FileName;
            }
            trnvm.BranchId = Convert.ToInt32(identity.BranchId);
            trnvm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            trnvm.CreatedBy = identity.Name;
            trnvm.CreatedFrom = identity.WorkStationIP;
            retResults = empTrnApp.Insert(trnvm);
            if (TransferF != null && TransferF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Transfer"), retResults[2] + TransferF.FileName);
                TransferF.SaveAs(path);
            }
            Session["result"] = retResults[0] + "~" + retResults[1];
            return RedirectToAction("Transfer");
        }

        [HttpGet]
        public ActionResult TransferOLD(string EmployeeId)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);
            vm.transferVM = new EmployeeTransferRepo().SelectByEmployeeCurrent(EmployeeId);
            return PartialView("_transferOLD", vm);
        }

        [HttpGet]
        public ActionResult TransferNEW(string EmployeeId)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);
            vm.transferVM = new EmployeeTransferRepo().SelectByEmployeeCurrent(EmployeeId);
            return PartialView("_transferNEW", vm);
        }
        public ActionResult Transferdetail(string id, string empcode = "", string btn = "current")
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrEmpty(id)) {
              vm=  repo.SelectById(id);
            }
            if (!string.IsNullOrWhiteSpace(empcode) && !string.IsNullOrWhiteSpace(btn)) {
                vm = repo.SelectEmpStructure(empcode, btn);
            }
            return PartialView("_transferdetail", vm);
        }
        #endregion Transfer
        #region Promotion
        [HttpGet]
        public ActionResult Promotion()
        {
            EmployeeInfoVM empVM = new EmployeeInfoVM();
            empVM.promotionVM = new EmployeePromotionVM();
            return View(empVM);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Promotion(EmployeeInfoVM vm, HttpPostedFileBase PromotionF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] retResults = new string[6];
            EmployeePromotionRepo empPrApp = new EmployeePromotionRepo();
            EmployeePromotionVM prnvm = new EmployeePromotionVM();
            prnvm = vm.promotionVM;
            if (PromotionF != null && PromotionF.ContentLength > 0)
            {
                prnvm.FileName = PromotionF.FileName;
            }
            prnvm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            prnvm.CreatedBy = identity.Name;
            prnvm.CreatedFrom = identity.WorkStationIP;
            retResults = empPrApp.Insert(prnvm);
            if (PromotionF != null && PromotionF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Promotion"), retResults[2] + PromotionF.FileName);
                PromotionF.SaveAs(path);
            }
            Session["result"] = retResults[0] + "~" + retResults[1];
            return RedirectToAction("Promotion");
        }
        [HttpGet]
        public ActionResult PromotionOLD(string EmployeeId)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);
            vm.promotionVM = new EmployeePromotionRepo().SelectByEmployeeCurrent(EmployeeId);
            return PartialView("_promotionOLD", vm);
        }
        [HttpGet]
        public ActionResult PromotionNEW(string EmployeeId)
        {
            _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);
            vm.promotionVM = new EmployeePromotionRepo().SelectByEmployeeCurrent(EmployeeId);
            return PartialView("_promotionNEW", vm);
        }

        public ActionResult Promotiondetail(string id, string empcode = "", string btn = "current")
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrEmpty(id))
            {
                vm = repo.SelectById(id);
            }
            if (!string.IsNullOrWhiteSpace(empcode) && !string.IsNullOrWhiteSpace(btn))
            {
                vm = repo.SelectEmpStructure(empcode, btn);
            }
            return PartialView("_promotiondetail", vm);
        }

        #endregion Promotion
        #region Left
        [HttpGet]
        public ActionResult Left()
        {
            EmployeeInfoVM empVM = new EmployeeInfoVM();
            empVM.leftInformation = new EmployeeLeftInformationVM();
            return View(empVM);
        }
        [HttpGet]
        public ActionResult LeftInfo()
        {
            EmployeeInfoVM empVM = new EmployeeInfoVM();
            empVM.leftInformation = new EmployeeLeftInformationVM();
            return PartialView("_left", empVM);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Left(EmployeeInfoVM evm, HttpPostedFileBase LeftInformationF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeLeftInformationVM vm = new EmployeeLeftInformationVM();
            vm = evm.leftInformation;
            if (LeftInformationF != null && LeftInformationF.ContentLength > 0)
            {
                vm.FileName = LeftInformationF.FileName;
            }
            string[] retResults = new string[6];
            EmployeeLeftInformationRepo leftRepo = new EmployeeLeftInformationRepo();
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            retResults = leftRepo.Insert(vm);
            if (LeftInformationF != null && LeftInformationF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/LeftInformation"), retResults[2] + LeftInformationF.FileName);
                LeftInformationF.SaveAs(path);
            }
            Session["result"] = retResults[0] + "~" + retResults[1];
            return RedirectToAction("Left");
        }
        #endregion Left
        #region Payroll

        [HttpGet]
        public ActionResult Payroll()
        {
            EmployeeInfoVM empVM = new EmployeeInfoVM();
            return View(empVM);
        }

        [HttpGet]
        public ActionResult SalaryStructure(string EmployeeId)
        {
            EmployeeInfoVM empVM = new EmployeeInfoVM();
            empVM.leftInformation = new EmployeeLeftInformationVM();
            return PartialView("_left", empVM);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult EmployeeSalaryStructure(EmployeeInfoVM evm, HttpPostedFileBase LeftInformationF)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeLeftInformationVM vm = new EmployeeLeftInformationVM();
            vm = evm.leftInformation;
            if (LeftInformationF != null && LeftInformationF.ContentLength > 0)
            {
                vm.FileName = LeftInformationF.FileName;
            }
            string[] retResults = new string[6];
            EmployeeLeftInformationRepo leftRepo = new EmployeeLeftInformationRepo();
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            retResults = leftRepo.Insert(vm);
            if (LeftInformationF != null && LeftInformationF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/LeftInformation"), retResults[2] + LeftInformationF.FileName);
                LeftInformationF.SaveAs(path);
            }
            Session["result"] = retResults[0] + "~" + retResults[1];
            return RedirectToAction("Left");
        }
        #endregion Left
        public ActionResult _index(JQueryDataTableParamModel param, string code, string name)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_5"]);
            var isActiveFilter = Convert.ToString(Request["sSearch_6"]);
            //Code
            //EmpName 
            //Department 
            //Designation
            //JoinDate

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
            }

            var isActiveFilter1 = isActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();


            var fromID = 0;
            var toID = 0;
            if (idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }
            #endregion Column Search


            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            List<EmployeeInfoVM> getAllData = new List<EmployeeInfoVM>();
            IEnumerable<EmployeeInfoVM> filteredData;
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (identity.IsAdmin || identity.IsHRM)
            {
                getAllData = _empRepo.SelectAll();
            }
            else
            {
                getAllData.Add(_empRepo.SelectById(identity.EmployeeId));

            }
            
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.JoinDate.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #region Column Filtering
            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || (joinDateFilter != "" && joinDateFilter != "~") || isActiveFilter != "")
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    &&
                                    (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    &&
                                    (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    &&
                                    (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    &&
                                    (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
                                    &&
                                    (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.JoinDate))
                                    &&
                                    (isActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(isActiveFilter1.ToLower()))
                                );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.JoinDate) :
                sortColumnIndex == 6 && isSortable_6 ? c.IsActive.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { 
                 Convert.ToString(c.Id)
                , c.Code
                , c.EmpName 
                , c.Department 
                , c.Designation
                , c.JoinDate
                , Convert.ToString(c.IsActive == true ? "Active" : "Inactive") 
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

        public JsonResult CheckPromotionDate(string employeeId,string date)
        {
            return Json(new EmployeePromotionRepo().CheckPromotionDate(employeeId, date), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckTransferDate(string employeeId, string date)
        {
            return Json(new EmployeeTransferRepo().CheckTransferDate(employeeId, date), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmployeeInfo(string Id)
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeVM vm = new EmployeeVM();
            if (!string.IsNullOrWhiteSpace(Id))
            {
                vm = repo.EmployeeInfo(Id);
                if (vm.IsPermanent)
                {
                    return PartialView("_employee", vm);
                }
                else
                {
                    Session["result"] = "Fail~This Employee not Permanent";
                    return RedirectToAction("Leave");
                }
            }
            else {
                return PartialView("_employee", vm);
            }
        }
        #endregion index
    }
}
