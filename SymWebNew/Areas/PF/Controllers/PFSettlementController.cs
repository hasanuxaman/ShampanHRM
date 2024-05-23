using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using SymViewModel.Common;
using SymReporting.PF;
using SymWebUI.Areas.PF.Models;

namespace SymWebUI.Areas.PF.Controllers
{
    public class PFSettlementController : Controller
    {
        public PFSettlementController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /PF/PFSettlement/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        PFSettlementRepo _repo = new PFSettlementRepo();
        PFSettlementDetailRepo _repoDetail = new PFSettlementDetailRepo();


        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id
            //01     //Code
            //02     //EmpName
            //03     //Designation
            //04     //Department
            //05     //TotalPayableAmount
            //06     //AlreadyPaidAmount
            //07     //NetPayAmount
            //08     //Post

            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<PFSettlementVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.TotalPayableAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.AlreadyPaidAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable7 && c.NetPayAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable8 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PFSettlementVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? c.TotalPayableAmount.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.AlreadyPaidAmount.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.NetPayAmount.ToString() :
                sortColumnIndex == 8 && isSortable_8 ? c.Post.ToString() :
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
                , c.Code
                , c.EmpName
                , c.Designation
                , c.Department
                , c.TotalPayableAmount.ToString()
                , c.AlreadyPaidAmount.ToString()
                , c.NetPayAmount.ToString()
                , c.Post ? "Posted" : "Not Posted"
     
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


        [Authorize(Roles = "Admin")]
        public ActionResult IndexResignEmployee()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View();
        }
        public ActionResult _indexResignEmployee(JQueryDataTableParamModel param)
        {
            //00     //EmployeeId
            //01     //Code
            //02     //EmpName
            //03     //Designation
            //04     //Department
            //05     //LeftDate

            #region Search and Filter Data
            //////string[] cFields = { "pdf.Post", "pdf.IsDistribute" };
            //////string[] cValues = { "1", "0" };

            PFSettlementRepo _repoPFSettlement = new PFSettlementRepo();
            var getAllData = _repoPFSettlement.SelectAll_ResignEmployee();
            IEnumerable<PFSettlementVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.EmpResignDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PFSettlementVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.EmpResignDate) :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                Convert.ToString(c.EmployeeId)
                , c.Code
                , c.EmpName
                , c.Designation
                , c.Department
                , c.EmpResignDate
     
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

        [Authorize(Roles = "Admin")]
        public ActionResult Create(PFSettlementVM vm)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();

            vm = _repo.PreInsert(vm);

            vm.Operation = "add";
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(PFSettlementVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2] });
                }
                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            PFSettlementVM vm = new PFSettlementVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.detailVMs = _repoDetail.SelectByMasterId(Convert.ToInt32(id));

            vm.Operation = "update";
            return View("Create", vm);
        }




        //////[Authorize(Roles = "Admin")]
        //////public JsonResult Process(string ids, string settlementDate)
        //////{
        //////    Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
        //////    PFSettlementVM vm = new PFSettlementVM();
        //////    string[] IDs = ids.Split('~');
        //////    string[] result = new string[6];
        //////    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
        //////    vm.CreatedBy = identity.Name;
        //////    vm.CreatedFrom = identity.WorkStationIP;
        //////    vm.SettlementDate = settlementDate;

        //////    vm = _repo.PreInsert(vm, IDs);
        //////    return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        //////}


        [Authorize(Roles = "Admin")]
        public JsonResult Post(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = _repo.Post(a);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult GetPFSettlement(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();

            PFSettlementVM vm = new PFSettlementVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult ReportView(string Id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                PFSettlementVM vm = new PFSettlementVM();
                PFSettlementRepo _repo = new PFSettlementRepo();

                //////string[] conditionFields = { "ve.Code", "es.SettlementDate>", "es.SettlementDate<" };
                //////string[] conditionValues = { empCodeFrom, Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo) };
                vm.Id = Convert.ToInt32(Id);
                table = _repo.Report(vm);
                ReportHead = "There are no data to Preview for PF Settlement";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "PF Settlement Statement";
                }
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtPFSettlement";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptPFSettlement.rpt";


                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //[HttpGet]
        //public ActionResult PFSettlement_GLTransactionReport(string id)
        //{
        //    try
        //    {

        //        PFReportVM vm = new PFReportVM();
        //        PFReport report = new PFReport();

        //        vm = report.PFSettlement_GLTransactionReport(id);

        //        return File(vm.MemStream, "application/PDF");
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}




        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }




        #region Future Development // Comments

        //////[Authorize(Roles = "Admin")]
        //////[HttpGet]
        //////public ActionResult Create(PFSettlementVM vm)
        //////{
        //////    Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
        //////    vm.Operation = "add";
        //////    vm.TransactionType = "Other";
        //////    vm = _repo.PreSelect(vm);



        //////    if (string.IsNullOrWhiteSpace(vm.PreDistributionFundIds) || vm.TotalEmployeeContribution == 0 || vm.TotalEmployerContribution == 0 || vm.PFSettlementDetailVMs.Count() == 0)
        //////    {
        //////        Session["result"] = "Info" + "~" + "Found No Employee For Distribution!";
        //////        return RedirectToAction("Index");
        //////    }

        //////    return View(vm);
        //////}


        //////[Authorize(Roles = "Admin")]
        //////[HttpPost]
        //////public ActionResult CreateEdit(PFSettlementVM vm)
        //////{
        //////    string[] result = new string[6];
        //////    ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //////    try
        //////    {
        //////        if (vm.Operation.ToLower() == "add")
        //////        {
        //////            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
        //////            vm.CreatedBy = identity.Name;
        //////            vm.CreatedFrom = identity.WorkStationIP;
        //////            result = _repo.Insert(vm);
        //////            Session["result"] = result[0] + "~" + result[1];
        //////            if (result[0].ToLower() == "success")
        //////            {
        //////                return RedirectToAction("Edit", new { id = result[2] });
        //////            }
        //////            else
        //////            {
        //////                return View("Create", vm);
        //////            }
        //////        }
        //////        else if (vm.Operation.ToLower() == "update")
        //////        {
        //////            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
        //////            vm.LastUpdateBy = identity.Name;
        //////            vm.LastUpdateFrom = identity.WorkStationIP;
        //////            result = _repo.Update(vm);
        //////            Session["result"] = result[0] + "~" + result[1];
        //////            return RedirectToAction("Edit", new { id = result[2] });
        //////        }
        //////        else
        //////        {
        //////            return View("Create", vm);
        //////        }
        //////    }
        //////    catch (Exception)
        //////    {
        //////        Session["result"] = "Fail~Data Not Succeessfully!";
        //////        FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
        //////        return View("Create", vm);
        //////    }
        //////}

        //////[Authorize(Roles = "Admin")]
        //////[HttpGet]
        //////public ActionResult Edit(string id)
        //////{
        //////    Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
        //////    PFSettlementVM vm = new PFSettlementVM();
        //////    vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
        //////    vm.PFSettlementDetailVMs = _repoDetail.SelectByMasterId(Convert.ToInt32(id));
        //////    vm.Operation = "update";
        //////    return View("Create", vm);
        //////}

        //////[Authorize(Roles = "Admin")]
        //////public JsonResult Delete(string ids)
        //////{
        //////    Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
        //////    PFSettlementVM vm = new PFSettlementVM();
        //////    string[] a = ids.Split('~');
        //////    string[] result = new string[6];
        //////    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
        //////    vm.LastUpdateBy = identity.Name;
        //////    vm.LastUpdateFrom = identity.WorkStationIP;
        //////    result = _repo.Delete(vm, a);
        //////    return Json(result[0]+"~"+result[1], JsonRequestBehavior.AllowGet);
        //////}




        #endregion



    }
}
