using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Tax;
using SymViewModel.Tax;
using SymRepository.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Excel;
using OfficeOpenXml;
using SymRepository.Common;
using Newtonsoft.Json;

namespace SymWebUI.Areas.TAX.Controllers
{
    public class TaxDepositController : Controller
    {
        //
        // GET: /TAX/TaxDeposit/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        TaxDepositRepo _repo = new TaxDepositRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "70002", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Acc/Home");
            }
            TaxDepositVM vm = new TaxDepositVM();
            return View(vm);
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id 
            //01     //EmployeeCode 
            //02     //EmployeeName 
            //03     //Designation 
            //04     //ChallanNo 
            //05     //DepositAmount 
            //05     //DepositDate 
            #region Search and Filter Data
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var getAllData = _repo.SelectAll(0);
            IEnumerable<TaxDepositVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.EmployeeCode.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.EmployeeName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.ChallanNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.DepositAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.DepositDate.ToString().ToLower().Contains(param.sSearch.ToLower())

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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<TaxDepositVM, string> orderingFunction;
            orderingFunction = (c =>
            sortColumnIndex == 1 && isSortable_1 ? c.EmployeeCode :
            sortColumnIndex == 2 && isSortable_2 ? c.EmployeeName :
            sortColumnIndex == 3 && isSortable_3 ? c.Designation :
            sortColumnIndex == 4 && isSortable_3 ? c.ChallanNo :
            sortColumnIndex == 5 && isSortable_3 ? c.DepositAmount.ToString() :
            sortColumnIndex == 6 && isSortable_3 ? c.DepositDate :
            "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            IEnumerable<string[]> result;
            result = from c in displayedCompanies
                     select new[] { 
                Convert.ToString(c.Id) 
                , c.EmployeeCode             
                , c.EmployeeName             
                , c.Designation             
                , c.ChallanNo             
                , c.DepositAmount.ToString()            
                , c.DepositDate             
                
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
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "add").ToString();
            TaxDepositVM vm = new TaxDepositVM();
            vm.Operation = "add";
            return View(vm);
        }
        [HttpPost]
        public ActionResult CreateEdit(TaxDepositVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                if (vm.Operation == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { Id = result[2] });
                    }
                    else
                    {
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { Id = vm.Id });
                    }
                    else
                    {
                        return View("Create", vm);
                    }
                }
                else
                {
                    return RedirectToAction("Index");
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
        public ActionResult Edit(string Id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "edit").ToString();
            TaxDepositVM vm = new TaxDepositVM();
            vm = _repo.SelectAll(Convert.ToInt32(Id)).FirstOrDefault();

            vm.Operation = "update";
            return View("Create", vm);
        }
        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "delete").ToString();
            TaxDepositVM vm = new TaxDepositVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }



        [HttpGet, Authorize(Roles = "Admin")]
        public ActionResult ExcelImport()
        {
            return View();
        }


        [Authorize(Roles = "Admin")]
        public ActionResult DownloadExcel(string fid, string orderBy, string particular)
        {
            try
            {
                var result = new string[6];
                var repo = new TaxDepositRepo();

                if (string.IsNullOrEmpty(fid) || string.IsNullOrEmpty(orderBy) || string.IsNullOrEmpty(particular))
                {
                    result[0] = "Fail";
                    result[1] = "Fail~Field Missing";
                    Session["result"] = result[0] + "~" + result[1];

                    return RedirectToAction("ExcelImport");
                }

                var table = repo.GetExcelExportData(fid, orderBy, particular);

                if (table == null || table.Rows.Count == 0)
                {
                    result[0] = "Info";
                    result[1] = "No Data Found";
                    Session["result"] = result[0] + "~" + result[1];

                    return RedirectToAction("ExcelImport");
                }



                string filename = table.Rows[0]["PeriodName"].ToString();
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(table, true);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + "-" + particular + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                result[0] = "Success";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];

                return RedirectToAction("ExcelImport");
            }
            catch (Exception e)
            {
                // Session["result"] = result[0] + "~" + result[1];
                //  FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ExcelImport");
            }
        }



        [Authorize(Roles = "Admin")]
        public ActionResult DownloadSelfExcel(string fid, string orderBy, string particular)
        {
            try
            {
                var result = new string[6];
                var repo = new TaxDepositRepo();

                if (string.IsNullOrEmpty(fid) || string.IsNullOrEmpty(orderBy) || string.IsNullOrEmpty(particular))
                {
                    result[0] = "Fail";
                    result[1] = "Fail~Field Missing";
                    Session["result"] = result[0] + "~" + result[1];

                    return RedirectToAction("ExcelImport");
                }

                var table = repo.GetExcelSelfExportData(fid, orderBy, particular);

                if (table == null || table.Rows.Count == 0)
                {
                    result[0] = "Info";
                    result[1] = "No Data Found";
                    Session["result"] = result[0] + "~" + result[1];

                    return RedirectToAction("ExcelImport");
                }



                string filename = table.Rows[0]["Year"].ToString();
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(table, true);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment;  filename=" + FileName);
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + "-" + particular + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                result[0] = "Success";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];

                return RedirectToAction("ExcelImport");
            }
            catch (Exception e)
            {
                // Session["result"] = result[0] + "~" + result[1];
                //  FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ExcelImport");
            }
        }


        [HttpPost, Authorize(Roles = "Admin")]
        public ActionResult ImportExcel(HttpPostedFileBase file)
        {
            var result = new string[6];

            try
            {
                if (file == null)
                    return RedirectToAction("ExcelImport");

                var filePath = Server.MapPath("~/Files/Export/") + file.FileName;

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                if (file.ContentLength > 0)
                    file.SaveAs(filePath);

                var table = ReadExcel(file, filePath);

                result = new TaxDepositRepo().ImportData(table);


                Session["result"] = result[0] + "~" + result[1];

                return RedirectToAction("ExcelImport");
            }
            catch (Exception e)
            {
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ExcelImport");
            }

        }

        private DataTable ReadExcel(HttpPostedFileBase file, string filePath)
        {
            IExcelDataReader reader = null;
            FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);

            if (file.FileName.EndsWith(".xls"))
            {
                reader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (file.FileName.EndsWith(".xlsx"))
            {
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }

            reader.IsFirstRowAsColumnNames = true;
            var dataSet = reader.AsDataSet();
            var table = dataSet.Tables[0];
            reader.Close();
            return table;
        }

        #region Multiple Tax Deposit

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CreateMultiple()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "add").ToString();
            TaxDepositVM vm = new TaxDepositVM();
            vm.Operation = "add";
            return View(vm);
        }

        public ActionResult _indexTaxDeposit(TaxDepositVM vm)
        {
            #region Declare Variable
            //EmloyeeTAXSlabVM vm = new EmloyeeTAXSlabVM();
            if (vm.EmployeeCode == "0_0" || vm.EmployeeCode == "0" || vm.EmployeeCode == "" || vm.EmployeeCode == "null" || vm.EmployeeCode == null)
            {
                vm.EmployeeCode = "";
            }
            if (vm.EmployeeCodeTo == "0_0" || vm.EmployeeCodeTo == "0" || vm.EmployeeCodeTo == "" || vm.EmployeeCodeTo == "null" || vm.EmployeeCodeTo == null)
            {
                vm.EmployeeCodeTo = "";
            }
            if (vm.FiscalYearDetailId == null)
            {
                vm.FiscalYearDetailId = 0;
            }
            if (vm.Particular == "null" || vm.Particular == null)
            {
                vm.Particular = "";
            }

            #endregion Declare Variable

            List<TaxDepositVM> VMs = new List<TaxDepositVM>();

            VMs = _repo.EmployeeSalaryTaxList(vm, "CODE");

            return PartialView("_index", VMs);

        }

        [HttpPost]
        public ActionResult CreateTaxDeposit(List<TaxDepositVM> VMs, TaxDepositVM vm)
        {
            string[] result = new string[6];
            string mgs = "";
            IEnumerable<TaxDepositVM> updatedVMs = VMs;
            VMs = updatedVMs.Where(c => c.IsEmployeeChecked == true).ToList();

            if (VMs.Count > 0)
            {
                foreach (var item in VMs)
                {
                    bool PeriodLock = false;

                    PeriodLock = new FiscalYearRepo().FiscalPeriodLockCheck(item.FiscalYearDetailId);

                    if (PeriodLock)
                    {
                        mgs = "Fail" + "~" + item.PeriodName + " Fiscal Period is Locked";

                        return Json(mgs, JsonRequestBehavior.AllowGet);

                    }
                }
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(VMs);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

            if (dt == null || dt.Rows.Count == 0)
            {
                mgs = "Fail" + "~" + "Please select employee fast!";

                return Json(mgs, JsonRequestBehavior.AllowGet);

            }

            result = _repo.InsertEmployeeMultipleTaxDeposit(dt, vm);

            mgs = result[0] + "~" + result[1];

            return Json(mgs, JsonRequestBehavior.AllowGet);

        }


        #endregion




    }
}
