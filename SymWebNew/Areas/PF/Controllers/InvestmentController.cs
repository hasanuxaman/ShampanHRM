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
using Newtonsoft.Json;
using SymWebUI.Areas.PF.Models;
using OfficeOpenXml;

namespace SymWebUI.Areas.PF.Controllers
{
    public class InvestmentController : Controller
    {
        public InvestmentController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /PF/Investment/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        InvestmentRepo _repo = new InvestmentRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View("~/Areas/PF/Views/Investment/Index.cshtml");
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id 
            //01     //ReferenceNo
            //02     //InvestmentType
            //03     //InvestmentName
            //04     //InvestmentDate
            //05     //MaturityDate  
            //06     //InvestmentValue


            #region Search and Filter Data
            var getAllData = _repo.SelectAll(0, new string[] { "inv.TransType" }, new string[] { AreaTypePFVM.TransType });
            IEnumerable<InvestmentVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_9"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.ReferenceNo.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.TransactionCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.InvestmentType.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.InvestmentName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.InvestmentDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.MaturityDate.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.InvestmentValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable8 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable9 && c.IsEncashed.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_9"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<InvestmentVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.ReferenceNo :
                sortColumnIndex == 2 && isSortable_2 ? c.TransactionCode :
                sortColumnIndex == 3 && isSortable_3 ? c.InvestmentType :
                sortColumnIndex == 4 && isSortable_4 ? c.InvestmentName :
                sortColumnIndex == 5 && isSortable_5 ? c.InvestmentDate :
                sortColumnIndex == 6 && isSortable_6 ? c.MaturityDate :
                sortColumnIndex == 7 && isSortable_7 ? c.InvestmentValue.ToString() :
                sortColumnIndex == 8 && isSortable_8 ? c.Post.ToString() :
                sortColumnIndex == 9 && isSortable_9 ? c.IsEncashed.ToString() :               
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
                , c.ReferenceNo            
                , c.TransactionCode            
                , c.InvestmentType         
                , c.InvestmentName         
                , c.InvestmentDate            
                , c.MaturityDate              
                , c.InvestmentValue.ToString()
                , c.Post ? "Posted" : "Not Posted"               
                , c.IsEncashed ? "Encashed" : "Not Encashed"               
     
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
        public ActionResult Create(InvestmentVM vm)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            //vm = _repo.PreInsert(vm);
            vm.TransType = AreaTypePFVM.TransType;
            vm.Operation = "add";
            return View("~/Areas/PF/Views/Investment/Create.cshtml", vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(InvestmentVM vm)
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
                        return View("~/Areas/PF/Views/Investment/Create.cshtml", vm);
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
                    return View("~/Areas/PF/Views/Investment/Create.cshtml", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/PF/Views/Investment/Create.cshtml", vm);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            InvestmentVM vm = new InvestmentVM();
            vm.TransType = AreaTypePFVM.TransType;

            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            vm.Operation = "update";
            return View("~/Areas/PF/Views/Investment/Create.cshtml", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            InvestmentVM vm = new InvestmentVM();
            vm.TransType = AreaTypePFVM.TransType;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
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

        public JsonResult GetInvestment(string id)
        {
            InvestmentVM vm = new InvestmentVM();
            vm.TransType = AreaTypePFVM.TransType;

            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(vm, JsonRequestBehavior.AllowGet);
            }
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            //////string investment = vm.InvestmentValue + "~" + vm.InvestmentRate;
            return Json(vm, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "report").ToString();
            return View("~/Areas/PF/Views/Investment/Report.cshtml");
        }
        //[HttpGet]
        //public ActionResult ReportView(string dtFrom = "", string dtTo = "", string invId = "", string invTypeId = "", string statement = "")
        //{
        //    try
        //    {
        //        string ReportHead = "";
        //        string rptLocation = "";
        //        ReportDocument doc = new ReportDocument();
        //        DataTable table = new DataTable();
        //        DataSet ds = new DataSet();
        //        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        //        InvestmentVM vm = new InvestmentVM();
        //        InvestmentRepo _repo = new InvestmentRepo();

        //        string[] conditionFields = { "inv.InvestmentDate>", "inv.InvestmentDate<", "inv.Id", "inv.InvestmentTypeId" };
        //        string[] conditionValues = { Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo), invId, invTypeId };

        //        if (statement == "report")
        //        {


        //            table = _repo.Report(vm, conditionFields, conditionValues);
        //            ReportHead = "There are no data to Preview for Investment";
        //            if (table.Rows.Count > 0)
        //            {
        //                ReportHead = "Investment List";
        //            }
        //            ds.Tables.Add(table);
        //            ds.Tables[0].TableName = "dtInvestment";
        //            rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\rptInvestment.rpt";
        //        }
        //        else if (statement == "statement")
        //        {
        //            table = _repo.InvestmentStatementReport(vm, conditionFields, conditionValues);
        //            ReportHead = "There are no data to Preview for Investment Statement";
        //            if (table.Rows.Count > 0)
        //            {
        //                ReportHead = "Investment Statement List";
        //            }
        //            ds.Tables.Add(table);
        //            ds.Tables[0].TableName = "dtInvestment";
        //            rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\rptInvestmentStatement.rpt";
        //        }




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
                PFReport report = new PFReport();

                string[] cFields = { "i.Id"};
                string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                    dt = new PFReportRepo().InvestmentHeaderReport(null, cFields, cValues);
               
                ReportHead = "There are no data to Preview for GL Transaction for Investment";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Investment GL Transactions";
                }

                    dt.TableName = "dtInvestmentHeader";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptInvestmentNew.rpt";
              

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
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

        public ActionResult DownloadExcel()
        {

            string[] result = new string[6];
            DataTable dt = new DataTable();
            try
            {
                SymUserRoleRepo _reposur = new SymUserRoleRepo();
                var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Common/Home");
                }
                ExcelPackage xlPackage = new ExcelPackage();
                string FileName = "Download.xlsx";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }

                dt = new PFReportRepo().InvestmentSummery();

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=Download.xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                result[0] = "Success";
                result[1] = "Data Saved Successfully!";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("InvestmentName");
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("InvestmentName");
            }
        }

        [HttpGet]
        public ActionResult reportVeiw(int id)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                InvestmentVM Investmentvm = new InvestmentVM();
                Investmentvm.TransType = AreaTypePFVM.TransType;
                PFReport report = new PFReport();
                Investmentvm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
                vm.Id = id;
                vm.Code = Investmentvm.TransactionCode;
                return PartialView("~/Areas/PF/Views/Investment/reportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Investment_GLTransactionReport(PFReportVM vm)
        {


            try
            {
                string ReportHead = "";
                string rptLocation = "";
                vm.TransType = AreaTypePFVM.TransType;
                PFReport report = new PFReport();


                string[] cFields = { "inv.TransactionCode", "inv.Id", "inv.InvestmentDate>", "inv.InvestmentDate<", "inv.TransactionType" };
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo),AreaTypePFVM.TransType };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                WithdrawVM Withdrawvm = new WithdrawVM();
                Withdrawvm.TransType = AreaTypePFVM.TransType;

                var Result = _repo.SelectAll(0, cFields, cValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));

              

                ReportHead = "There are no data to Preview for GL Transaction for Investment";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Investment GL Transactions";
                }
                dt.TableName = "dtInvestmentEncashment";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptInvestment.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        public ActionResult Investment_DetailsReport(PFReportVM vm)
        {


            try
            {
                string ReportHead = "";
                string rptLocation = "";
                vm.TransType = AreaTypePFVM.TransType;

                PFReport report = new PFReport();


                string[] cFields = { "inv.TransactionCode", "inv.Id", "inv.InvestmentDate>", "inv.InvestmentDate<" };
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo) };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                WithdrawVM Withdrawvm = new WithdrawVM();

                 dt = new PFReportRepo().InvestmentDetailsReport(vm, cFields, cValues);




                ReportHead = "There are no data to Preview for GL Transaction for Investment";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Investment GL Transactions";
                }
                dt.TableName = "dtInvestment";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptInvestmentDetails.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }


         [HttpPost]
        public ActionResult InvestmentReport(PFReportVM vm)
        {


            try
            {
                string ReportHead = "";
                string rptLocation = "";
                vm.TransType = AreaTypePFVM.TransType;
                PFReport report = new PFReport();

                string[] cFields = { "i.TransactionCode", "i.Id", "i.InvestmentDate>", "i.InvestmentDate<" };
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo) };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                WithdrawVM Withdrawvm = new WithdrawVM();
                if (vm.ReportType=="H")
                {
                    dt = new PFReportRepo().InvestmentHeaderReport(vm, cFields, cValues);
                }
                else
                {
                    vm.Id = vm.Id;
                    dt = new PFReportRepo().InvestmentDetailsReport(vm, null,null);
                }




                ReportHead = "There are no data to Preview for GL Transaction for Investment";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Investment GL Transactions";
                }

                if (vm.ReportType == "H")
                {
                    dt.TableName = "dtInvestmentHeader";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptInvestmentNew.rpt";
                }
                else
                {
                    dt.TableName = "dtInvestmentDetails";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptInvestmentDetails.rpt";
                }
               

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
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


        [HttpGet]
        public ActionResult PFContributionReport(string Id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                PFReport report = new PFReport();
                //vm.Id = 1;
                PFReportVM vm=new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;

                string[] cFields = { "PFHeaderId" };
                string[] cValues = { "1" };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                dt = new PFReportRepo().PFContributionReport(vm, cFields, cValues);
                ReportHead = "There are no data to Preview for GL Transaction for Contribution";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Contribution GL Transactions";
                }
                dt.TableName = "dtPFContribution";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptPFContribution.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";

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
