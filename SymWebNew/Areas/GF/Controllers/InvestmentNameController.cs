using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.PF;
using SymViewModel.Common;
using SymViewModel.PF;
using SymWebUI.Areas.PF.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.GF.Controllers
{
    public class InvestmentNameController : Controller
    {
        public InvestmentNameController()
        {
            ViewBag.TransType = AreaTypeGFVM.TransType;
        }
        //
        // GET: /PF/InvestmentName/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        InvestmentNameRepo _repo = new InvestmentNameRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }

            return View("~/Areas/PF/Views/InvestmentName/Index.cshtml");
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id 
            //01     //Name      
            //02     //Address 
            //03     //Remarks  


            #region Search and Filter Data
            string[] conditionFields = { "TransType" };
            string[] conditionValues = { AreaTypeGFVM.TransType };
            var getAllData = _repo.SelectAll(0,conditionFields,conditionValues);
            IEnumerable<InvestmentNameVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Address.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Remarks.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<InvestmentNameVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
                sortColumnIndex == 3 && isSortable_3 ? c.Address :
                sortColumnIndex == 4 && isSortable_4 ? c.Remarks :
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
                , c.Name      
                , c.Address 
                , c.Remarks  
     
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
        public ActionResult BlankItem(InvestmentNameDetailsVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {

                return PartialView("~/Areas/PF/Views/InvestmentName/_details.cshtml", vm);

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return PartialView("~/Areas/PF/Views/InvestmentName/_details.cshtml", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            InvestmentNameVM vm = new InvestmentNameVM();
            vm.TransType = AreaTypeGFVM.TransType;
            vm.TransType = AreaTypeGFVM.TransType;
            vm.Operation = "add";
            return View("~/Areas/PF/Views/InvestmentName/Create.cshtml", vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(InvestmentNameVM vm)
        {
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypeGFVM.TransType;
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("~/Areas/PF/Views/InvestmentName/Create.cshtml", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypeGFVM.TransType;
                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2] });
                }
                else
                {
                    return View("~/Areas/PF/Views/InvestmentName/Create.cshtml",vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/PF/Views/InvestmentName/Create.cshtml", vm);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            InvestmentNameVM vm = new InvestmentNameVM();
            vm.TransType = AreaTypeGFVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.InvestmentNameDetails = _repo.SelectAllDetails(0, new[] { "InvestmentNameId" }, new[] { vm.Id.ToString() });
            vm.InvestmentAccrueds = new InvestmentAccruedRepo().SelectAll(0, new[] { "I.InvestmentNameId" }, new[] { vm.Id.ToString() });
            vm.Operation = "update";
            return View("~/Areas/PF/Views/InvestmentName/Create.cshtml", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AccruedProcessView(string InvestmentNameId, string AccruedId)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            InvestmentNameVM vm = new InvestmentNameVM();
            vm.TransType = AreaTypeGFVM.TransType;
            vm.InvestmentAccrueds = new InvestmentAccruedRepo().SelectAll(0, new[] { "I.InvestmentNameId", "i.id" }, new[] { InvestmentNameId.ToString(), AccruedId });
            vm.Id = Convert.ToInt32(InvestmentNameId);
            vm.Operation = "add";
            vm.InvestmentAccrued = vm.InvestmentAccrueds.FirstOrDefault();
            return PartialView("~/Areas/PF/Views/InvestmentName/AccruedProcessView.cshtml", vm);
        }
        [HttpGet]
        public ActionResult LoanSattlementReportVeiw()
        {
            try
            {

                InvestmentNameVM vm = new InvestmentNameVM();
                vm.TransType = AreaTypeGFVM.TransType;

                return PartialView("~/Areas/PF/Views/InvestmentName/EditProcess.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AccruedProcess(InvestmentNameVM vm)
        {
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypeGFVM.TransType;
                    result =new InvestmentAccruedRepo().Process(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = vm.Id });
                    }
                    else
                    {
                        return RedirectToAction("Edit", new { id = vm.Id });
                    }
                }
                
                else
                {
                    return RedirectToAction("Edit", new { id = vm.Id });
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/PF/Views/InvestmentName/Create.cshtml", vm);
            }
        }
        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            InvestmentNameVM vm = new InvestmentNameVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.TransType = AreaTypeGFVM.TransType;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public JsonResult InvestmentNameInfoLoad(string Id = "0")
        {
            InvestmentNameVM vm = new InvestmentNameVM();
            vm.TransType = AreaTypeGFVM.TransType;
            if (!string.IsNullOrWhiteSpace(Id))
            {
                vm = _repo.SelectAll(Convert.ToInt32(Id), null, null).FirstOrDefault();
            }
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
        public ActionResult ReportView(string id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                ds = new PFReportRepo().InvestmentNameReport(Convert.ToInt32(id));

             

                //string[] cFields = { "td.TransactionMasterId", "td.TransactionType" };
                //string[] cValues = { vm.Id.ToString(), vm.TransactionType };
                //dt = _repoGLTransactionDetail.Report(null, cFields, cValues);
                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                ds.Tables[0].TableName = "DtInvestmentName";
                ds.Tables[2].TableName = "DtInvestmentAccrued";
                ds.Tables[1].TableName = "DtInvestmentNameDetails";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptInvestmentRegister.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
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

        [Authorize(Roles = "Admin")]
        public JsonResult AccruedDelete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            InvestmentNameVM vm = new InvestmentNameVM();
            vm.TransType = AreaTypeGFVM.TransType;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = new InvestmentAccruedRepo().Delete(a);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "Admin")]
        public JsonResult AccruedPost(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            InvestmentNameVM vm = new InvestmentNameVM();
            vm.TransType = AreaTypeGFVM.TransType;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = new InvestmentAccruedRepo().Post(a);

            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }

    }
}
