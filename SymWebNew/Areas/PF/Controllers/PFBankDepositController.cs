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
using Newtonsoft.Json;
using SymReporting.PF;
using SymWebUI.Areas.PF.Models;
namespace SymWebUI.Areas.PF.Controllers
{
    public class PFBankDepositController : Controller
    {
        public PFBankDepositController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /PF/PFBankDeposit/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        PFBankDepositRepo _repo = new PFBankDepositRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View("~/Areas/PF/Views/PFBankDeposit/Index.cshtml");
        }

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id 
            //01     //DepositDate
            //02     //DepositAmount
            //03     //BankBranchName
            //04     //Post
            //05     //TransactionType
            //BankBranchName
            #region Search and Filter Data

            string[] conditionFields = { "pfbd.TransType" };
            string[] conditionValues = { AreaTypePFVM.TransType };

            var getAllData = _repo.SelectAll(0, conditionFields, conditionValues);

            IEnumerable<PFBankDepositVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData.Where(c =>
                        isSearchable1 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.DepositDate.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.DepositAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.BankBranchName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<PFBankDepositVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? Ordinary.DateToString(c.DepositDate) :
                sortColumnIndex == 3 && isSortable_3 ? c.DepositAmount.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.BankBranchName :
                sortColumnIndex == 5 && isSortable_5 ? c.Post.ToString() :
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
                , c.DepositDate
                , c.DepositAmount.ToString()
                , c.BankBranchName
                , c.Post?"Posted":"Not Posted"
     
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
        ////[HttpGet]
        public ActionResult Create(PFBankDepositVM vm)////string tType = "")
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();

           
                vm.TransactionType = "Other";
           

            vm.TransType = AreaTypePFVM.TransType;
            vm.Operation = "add";
            return View("~/Areas/PF/Views/PFBankDeposit/Create.cshtml",vm);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(PFBankDepositVM vm)
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
                    vm.TransType = AreaTypePFVM.TransType;
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("~/Areas/PF/Views/PFBankDeposit/Create.cshtml", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;
                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2] });
                }
                else
                {
                    return View("~/Areas/PF/Views/PFBankDeposit/Create.cshtml", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/PF/Views/PFBankDeposit/Create.cshtml", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            PFBankDepositVM vm = new PFBankDepositVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.TransType = AreaTypePFVM.TransType;

            vm.Operation = "update";
            return View("~/Areas/PF/Views/PFBankDeposit/Create.cshtml", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            PFBankDepositVM vm = new PFBankDepositVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.TransType = AreaTypePFVM.TransType;
            result = _repo.Delete(vm, a);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Post(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = _repo.Post(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult GetPFBankDeposit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();

            PFBankDepositVM vm = new PFBankDepositVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            return Json(vm, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "report").ToString();
            return View("~/Areas/PF/Views/PFBankDeposit/Report.cshtml");
        }

        //[HttpGet]
        //public ActionResult ReportView(string dtFrom = "", string dtTo = "", string bankBranchId = "")
        //{
        //    try
        //    {
        //        string ReportHead = "";
        //        string rptLocation = "";
        //        ReportDocument doc = new ReportDocument();
        //        DataTable table = new DataTable();
        //        DataSet ds = new DataSet();
        //        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        //        PFBankDepositVM vm = new PFBankDepositVM();
        //        PFBankDepositRepo _repo = new PFBankDepositRepo();

        //        string[] conditionFields = { "pfbd.DepositDate>", "pfbd.DepositDate<", "dd.AccountId" };
        //        string[] conditionValues = { Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo), bankBranchId };
        //        table = _repo.Report(vm, conditionFields, conditionValues);
        //        ReportHead = "There are no data to Preview for PF Bank Deposit";
        //        if (table.Rows.Count > 0)
        //        {
        //            ReportHead = "PF Bank Deposit List";
        //        }
        //        ds.Tables.Add(table);
        //        ds.Tables[0].TableName = "dtPFBankDeposit";
        //        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptPFBankDeposit.rpt";

        //        doc.Load(rptLocation);
        //        doc.SetDataSource(ds);
        //        string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
        //        doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
        //        doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
        //        //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
        //        var rpt = RenderReportAsPDF(doc);
        //        doc.Close();
        //        return rpt;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        [HttpGet]
        public ActionResult ReportView(int id)
        {

            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                //PFBankDepositVM vm = new PFBankDepositVM();
                string[] cFields = {  "pfbd.Id" };
                string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };
                var Result = _repo.SelectAll(0, cFields, cValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));


                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                dt.TableName = "dtPFBankDeposits";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptBankDeposit.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult reportVeiw(int id)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                PFBankDepositVM PFBankDepositvm = new PFBankDepositVM();
                PFReport report = new PFReport();
                PFBankDepositvm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
                vm.Id = id;
                vm.Code = PFBankDepositvm.Code;
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/PF/Views/PFBankDeposit/reportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult PFBankDepositReport(PFReportVM vm)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                
                //PFBankDepositVM vm = new PFBankDepositVM();
                string[] cFields = { "pfbd.Code", "pfbd.Id", "pfbd.DepositDate>", "pfbd.DepositDate<", "pfbd.TransType" };
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };
                var Result = _repo.SelectAll(0, cFields, cValues);

               dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));

                
                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                dt.TableName = "dtPFBankDeposits";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptBankDeposit.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }

    }
}
