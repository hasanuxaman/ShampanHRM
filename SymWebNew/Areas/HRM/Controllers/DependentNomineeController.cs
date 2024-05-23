using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace SymWebUI.Areas.HRM.Controllers
{
    [Authorize]
    public class DependentNomineeController : Controller
    {
        string currentId = "";
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index(string id, string empcode, string btn)
        {
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
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
            if (Session["mgs"].ToString() != "")
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "edit").ToString();
            if (!(identity.IsAdmin || identity.IsHRM))
            {
                id = identity.EmployeeId;
            }
            if (id != null)
            {
                vm = _infoRepo.SelectById(id);
            }
            EmployeeNomineeVM nomVM = new EmployeeNomineeVM();
            EmployeeDependentVM depdVM = new EmployeeDependentVM();

            vm.nomineeVM = nomVM;
            vm.dependentVM = depdVM;
            vm.Id = id;
            return View(vm);
        }
        public ActionResult _indexNominee(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            EmployeeNomineeRepo _empNoRepo = new EmployeeNomineeRepo();
            var getAllData = _empNoRepo.SelectAllByEmployee(Id);
            IEnumerable<EmployeeNomineeVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Relation.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.Mobile.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.Phone.ToLower().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeNomineeVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Relation :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Mobile :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.Phone :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies 
                         select new[] 
                         { 
                             Convert.ToString(c.Id)
                             , c.Name //+ "~" + Convert.ToString(c.Id)
                             , c.Relation
                             , c.Mobile
                             , c.Phone 
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
        public ActionResult _indexDependent(JQueryDataTableParamVM param, string Id)//EmployeeId
        {

            EmployeeDependentRepo _empDepRepo = new EmployeeDependentRepo();
            var getAllData = _empDepRepo.SelectAllByEmployee(Id);
            IEnumerable<EmployeeDependentVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Relation.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.Phone.ToLower().Contains(param.sSearch.ToLower()));
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
            Func<EmployeeDependentVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Relation :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Phone :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies 
                         select new[] 
                         { 
                             Convert.ToString(c.Id)
                             , c.Name
                             , c.Relation, c.Mobile 
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
        public ActionResult Nominee(string EmployeeId, int Id)
        {
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.nomineeVM = new EmployeeNomineeRepo().SelectById(Id);

            }
            else
            {
                EmployeeNomineeVM nom = new EmployeeNomineeVM();
                nom.EmployeeId = EmployeeId;
                vm.nomineeVM = nom;
            }
            return PartialView("_editNominee", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Nominee(EmployeeInfoVM vm, HttpPostedFileBase NomineeF, HttpPostedFileBase VaccineFile1, HttpPostedFileBase VaccineFiles2, HttpPostedFileBase VaccineFile3)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();

            string[] retResults = new string[6];
            string[] result = new string[6];
            EmployeeNomineeRepo empNonApp = new EmployeeNomineeRepo();
            EmployeeNomineeVM non = new EmployeeNomineeVM();
            non = vm.nomineeVM;
            if (NomineeF != null && NomineeF.ContentLength > 0)
            {
                non.FileName = NomineeF.FileName;
            }
            if (VaccineFiles2 != null && VaccineFiles2.ContentLength > 0)
            {
                non.FileName = VaccineFiles2.FileName;
            }
            //if (VaccineFiles2 != null && VaccineFiles2.ContentLength > 0)
            //{
            //    non.VaccineFiles2 = VaccineFiles2.FileName;
            //}
            if (VaccineFile1 != null && VaccineFile1.ContentLength > 0)
            {
                non.VaccineFile1 = VaccineFile1.FileName;
            }
            if (VaccineFile3 != null && VaccineFile3.ContentLength > 0)
            {
                non.VaccineFile3 = VaccineFile3.FileName;
            }
            //edu.EmployeeId = vm.Id;
            if (non.Id <= 0)
            {
                non.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                non.CreatedBy = identity.Name;
                non.CreatedFrom = identity.WorkStationIP;
                result=empNonApp.Insert(non);
            }
            else
            {
                non.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                non.LastUpdateBy = identity.Name;
                non.LastUpdateFrom = identity.WorkStationIP;
                result=empNonApp.Update(non);
            }
            if (NomineeF != null && NomineeF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Nominee"), result[2] + NomineeF.FileName);
                NomineeF.SaveAs(path);
            }
            if (VaccineFiles2 != null && VaccineFiles2.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/VaccineFiles2"), result[2] + VaccineFiles2.FileName);
                VaccineFiles2.SaveAs(path);
            }
            //if (VaccineFiles2 != null && VaccineFiles2.ContentLength > 0)
            //{
            //    var path = Path.Combine(Server.MapPath("~/Files/VaccineFiles2"), retResults[2] + VaccineFiles2.FileName);
            //    VaccineFiles2.SaveAs(path);
            //}
            if (VaccineFile1 != null && VaccineFile1.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/VaccineFile1"), retResults[2] + VaccineFile1.FileName);
                VaccineFile1.SaveAs(path);
            }

            //if (VaccineFile2 != null && VaccineFile2.ContentLength > 0)
            //{
            //    var path = Path.Combine(Server.MapPath("~/Files/VaccineFiles2"), retResults[2] + VaccineFile2.FileName);
            //    VaccineFile2.SaveAs(path);
            //}

            if (VaccineFile3 != null && VaccineFile3.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/VaccineFile3"), retResults[2] + VaccineFile3.FileName);
                VaccineFile3.SaveAs(path);
            }
            var mgs = result[0] + "~" + result[1];
            //return Json(mgs, JsonRequestBehavior.AllowGet);

            Session["mgs"] = "mgs";
            return RedirectToAction("Index", new { Id = non.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult NomineeDelete(string ids)
        {
            EmployeeNomineeRepo empNonApp = new EmployeeNomineeRepo();
            EmployeeNomineeVM vm = new EmployeeNomineeVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empNonApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
               

        [HttpGet]
        public ActionResult Export(string Id)
        {
            try
            {
                System.Data.DataTable dt;
                EmployeeNomineeRepo _empNoRepo = new EmployeeNomineeRepo();
                dt = _empNoRepo.SelectAllEmployeeForExcel(Id);               

                #region Excel

                string filename = "Nominee " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Nominee");
                string[] ReportHeaders = new string[] { "Nominee List" };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("Index");
                #endregion
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void ExcelSheetFormat(System.Data.DataTable dt, ExcelWorksheet workSheet, string[] ReportHeaders)
        {
            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;

            int InWordsRow = 0;
            InWordsRow = GrandTotalRow + 1;

            int SignatureSpaceRow = 0;
            SignatureSpaceRow = InWordsRow + 1;

            int SignatureRow = 0;
            SignatureRow = InWordsRow + 4;
            workSheet.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);
            #region Format

            var format = new OfficeOpenXml.ExcelTextFormat();
            format.Delimiter = '~';
            format.TextQualifier = '"';
            format.DataTypes = new[] { eDataTypes.String };



            for (int i = 0; i < ReportHeaders.Length; i++)
            {
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                workSheet.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

            }
            int colNumber = 0;

            foreach (DataColumn col in dt.Columns)
            {
                colNumber++;
                if (col.DataType == typeof(DateTime))
                {
                    workSheet.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                }
                else if (col.DataType == typeof(Decimal))
                {

                    workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                    #region Grand Total
                    workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + workSheet.Cells[TableHeadRow + 1, colNumber].Address + ":" + workSheet.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                    #endregion
                }

            }

            workSheet.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

            workSheet.Cells["A" + (TableHeadRow) + ":" + Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A" + (TableHeadRow) + ":" + Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");
            #endregion
        }

        private static string[] Alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ", "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ", "EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", "EN", "EO", "EP", "EQ", "ER", "ES", "ET", "EU", "EV", "EW", "EX", "EY", "EZ", "FA", "FB", "FC", "FD", "FE", "FF", "FG", "FH", "FI", "FJ", "FK", "FL", "FM", "FN", "FO", "FP", "FQ", "FR", "FS", "FT", "FU", "FV", "FW", "FX", "FY", "FZ" };


        [HttpGet]
        public ActionResult Dependent(string EmployeeId, int Id)
        {
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.dependentVM = new EmployeeDependentRepo().SelectById(Id);

            }
            else
            {
                EmployeeDependentVM depen = new EmployeeDependentVM();
                depen.EmployeeId = EmployeeId;
                vm.dependentVM = depen;
            }
            return PartialView("_editDependent", vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Dependent(EmployeeInfoVM vm, HttpPostedFileBase DependentF)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();

            string[] result = new string[6];
            EmployeeDependentRepo empDepApp = new EmployeeDependentRepo();
            EmployeeDependentVM dep = new EmployeeDependentVM();
            dep = vm.dependentVM;
            //edu.EmployeeId = vm.Id;
            if (DependentF != null && DependentF.ContentLength > 0)
            {
                dep.FileName = DependentF.FileName;
            }
            if (dep.Id <= 0)
            {
                dep.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                dep.CreatedBy = identity.Name;
                dep.CreatedFrom = identity.WorkStationIP;
                result=empDepApp.Insert(dep);
            }
            else
            {
                dep.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                dep.LastUpdateBy = identity.Name;
                dep.LastUpdateFrom = identity.WorkStationIP;
                result=empDepApp.Update(dep);
            }
            if (DependentF != null && DependentF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/Dependent"), result[2] + DependentF.FileName);
                DependentF.SaveAs(path);
            }
            var mgs = result[0] + "~" + result[1];
            //return Json(mgs, JsonRequestBehavior.AllowGet);
            Session["mgs"] = "mgs";
            return RedirectToAction("Index", new { Id = dep.EmployeeId, mgs = mgs });
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult DependentDelete(string ids)
        {
            //EmployeeNomineeRepo empNonApp = new EmployeeNomineeRepo();
            EmployeeDependentRepo empDepApp = new EmployeeDependentRepo();
            EmployeeDependentVM vm = new EmployeeDependentVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empDepApp.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
