using SymOrdinary;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using JQueryDataTables.Models;
using SymRepository.HRM;
using SymRepository.PF;
using SymRepository.Tax;
using SymViewModel.HRM;
using CrystalDecisions.CrystalReports.Engine;
using System.Data;
using System.IO;
using SymRepository.Common;
using SymWebUI.Areas.PF.Models;
using SymViewModel.Common;

namespace SymWebUI.Areas.PF.Controllers
{
    public class JournalController : Controller
    {
        public JournalController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /PF/Journal/
        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;


        private GLJournalRepo _glJournalRepo = new GLJournalRepo();
        public ActionResult Index(string JournalType = "1")
        {
            GLJournalVM vm = new GLJournalVM
            {
                JournalType = Convert.ToInt32(JournalType)

            };
            vm.TransType = AreaTypePFVM.TransType;

            return View("~/Areas/PF/Views/Journal/Index.cshtml",vm);

        }
     
        public ActionResult Create(string JournalType, string TransactionForm,string TransactionId)
        {
             int  TransactionType =0;
            EnumJournalTypeRepo _JournalTypeRepo = new EnumJournalTypeRepo();
                //1	=Journal Voucher	 
                //2	=Payment Voucher	 
                //3	=Receipt Voucher 
            var getAllData = _JournalTypeRepo.SelectAllJournalTransactionType(0, new[] { "NameTrim", "TransType" }, new[] { TransactionForm, AreaTypePFVM.TransType });
            if (getAllData != null)
            { 
               TransactionType = getAllData.FirstOrDefault().Id;
            }

          
            GLJournalVM vm = new GLJournalVM
            {
                Operation = "add",
                JournalType = Convert.ToInt32(JournalType),
                TransactionType = TransactionType,
                TransType=AreaTypePFVM.TransType

            };
            return View("~/Areas/PF/Views/Journal/Create.cshtml", vm);

        }

        public ActionResult _index(JQueryDataTableParamModel param, int JournalType =1)
        {
            #region Column Search
            //var idFilter = Convert.ToString(Request["sSearch_0"]);
            //var codeFilter = Convert.ToString(Request["sSearch_1"]);
            //var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            //var designationFilter = Convert.ToString(Request["sSearch_3"]);
            //var departmentFilter = Convert.ToString(Request["sSearch_4"]);
            //var sectionFilter = Convert.ToString(Request["sSearch_5"]);
            //var projecttFilter = Convert.ToString(Request["sSearch_6"]);
            //var joinDateFilter = Convert.ToString(Request["sSearch_7"]);

                //<th>Code</th>
                //<th>Transaction Date</th>
                //<th>Total Amount</th>
                //<th>Remarks</th>

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            //if (joinDateFilter.Contains('~'))
            //{
            //    //Split date range filters with ~
            //    fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
            //    toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
            //}


            var fromID = 0;
            var toID = 0;
            //if (idFilter.Contains('~'))
            //{
            //    //Split number range filters with ~
            //    fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
            //    toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            //}

            #endregion Column Search

            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            List<GLJournalVM> getAllData = new List<GLJournalVM>();
            IEnumerable<GLJournalVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            if (Convert.ToInt32( JournalType)>0)
            {
                getAllData = _glJournalRepo.SelectAll(0, new[] { "JournalType", "gl.TransType" }, new[] { JournalType.ToString(), AreaTypePFVM.TransType });
            }

            filteredData = getAllData;

            //if (string.IsNullOrWhiteSpace(EmployeeId) && string.IsNullOrWhiteSpace(fydid))
            //{
            //    if (Session["UserType"].ToString() == "True")
            //    {
            //        getAllData = _empRepo.SelectAllActiveEmp();
            //    }
            //    else
            //    {
            //        getAllData.Add(_empRepo.SelectById(Identit.EmployeeId));

            //    }
            //}
            //else
            //{
            //    Schedule1SalaryMonthlyRepo _repo = new Schedule1SalaryMonthlyRepo();
            //    string[] conditionFields = { "ssm.EmployeeId", "ssm.FiscalYearDetailId" };
            //    string[] conditionValues = { EmployeeId, fydid };
            //    getAllData = _repo.SelectEmployeeList(conditionFields, conditionValues, tType);
            //}



            //if (!string.IsNullOrEmpty(param.sSearch))
            //{
            //    //Optionally check whether the columns are searchable at all 
            //    var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
            //    var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
            //    var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
            //    var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
            //    var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
            //    var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
            //    var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
            //    filteredData = getAllData
            //        .Where(c =>
            //              isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
            //           || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
            //           || isSearchable3 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
            //           || isSearchable4 && c.Department.ToLower().Contains(param.sSearch.ToLower())
            //           || isSearchable5 && c.Section.ToLower().Contains(param.sSearch.ToLower())
            //           || isSearchable6 && c.Project.ToLower().Contains(param.sSearch.ToLower())
            //           || isSearchable7 && c.JoinDate.ToLower().Contains(param.sSearch.ToLower())
            //        );
            //}
            //else
            //{
            //    filteredData = getAllData;
            //}
            //if (codeFilter != "" || empNameFilter != "" || designationFilter != "" || departmentFilter != "" || sectionFilter != "" || projecttFilter != "" || (joinDateFilter != "" && joinDateFilter != "~"))
            //{
            //    filteredData = filteredData
            //                    .Where(c =>
            //                        (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
            //                        && (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
            //                        && (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
            //                        && (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
            //                        && (sectionFilter == "" || c.Section.ToLower().Contains(sectionFilter.ToLower()))
            //                        && (projecttFilter == "" || c.Project.ToLower().Contains(projecttFilter.ToLower()))
            //                        && (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
            //                        && (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.JoinDate))
            //                    );
            //}



            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
               sortColumnIndex == 5 && isSortable_5 ? c.Section :
                sortColumnIndex == 6 && isSortable_6 ? c.Project :
                sortColumnIndex == 7 && isSortable_7 ? Ordinary.DateToString(c.JoinDate) :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            //if (sortDirection == "asc")
            //    filteredData = filteredData.OrderBy(orderingFunction);
            //else
            //    filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                 Convert.ToString(c.Id)
                , c.Code
                , c.TransactionDate 
                , c.TransactionTypeName
                , c.TransactionValue.ToString()
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

        [HttpPost]
        public ActionResult CreateEdit(GLJournalVM vm)
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
                    result = _glJournalRepo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = vm.Id });
                    }

                    return PartialView("~/Areas/PF/Views/Journal/Create.cshtml", vm);

                }

                if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;
                    result = _glJournalRepo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = vm.Id });

                }

                return PartialView("~/Areas/PF/Views/Journal/Create.cshtml", vm);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return PartialView("~/Areas/PF/Views/Journal/Create.cshtml", vm);
            }
        }

        public ActionResult Edit(string id)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                GLJournalVM vm = _glJournalRepo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

                if (vm == null)
                    throw new Exception("null");

                vm.GLJournalDetails =
                    _glJournalRepo.SelectAllDetails(0, new[] { "gd.GLJournalId" }, new[] { vm.Id.ToString() });
                vm.Operation = "update";
                vm.TransType = AreaTypePFVM.TransType;
                return View("~/Areas/PF/Views/Journal/Create.cshtml", vm);

            }
            catch (Exception e)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());

                return View("~/Areas/PF/Views/Journal/Create.cshtml", new GLJournalVM());
                 
            }
        }  

        public ActionResult BlankItem(GLJournalDetailVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {

                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/PF/Views/Journal/_details.cshtml", vm);
                
               
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return PartialView("~/Areas/PF/Views/Journal/_details.cshtml", vm);
            }
        }

     
        public ActionResult ReportView(string id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                GLJournalVM vm = new GLJournalVM();

                string[] conditionFields = { "h.Id" };
                string[] conditionValues = { id };
                table = _glJournalRepo.Report(vm, conditionFields, conditionValues);
                ReportHead = "There are no data to Preview for Journal Voucher";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "Journal Voucher";
                }
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtGLTransaction";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptTIB_GLTransaction.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                //doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
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
        public JsonResult Post(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');

            GLJournalVM vm =  _glJournalRepo.SelectAll(Convert.ToInt32(a[0])).FirstOrDefault();
            if (vm.Post)
            {
                return Json("Already Posted", JsonRequestBehavior.AllowGet);

            }

            string[] result = new string[6];
            result = _glJournalRepo.Post(a);

            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
