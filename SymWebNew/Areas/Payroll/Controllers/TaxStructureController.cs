using OfficeOpenXml;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.Tax;
using SymViewModel.Common;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class TaxStructureController : Controller
    {
        //
        // GET: /Payroll/TaxStructure/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        TaxStructureRepo _repo = new TaxStructureRepo();
        #region Actions
        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        public ActionResult _index(JQueryDataTableParamVM param, string code, string name)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var CodeFilter = Convert.ToString(Request["sSearch_1"]);
            var NameFilter = Convert.ToString(Request["sSearch_2"]);
            var TaxValueFilter = Convert.ToString(Request["sSearch_3"]);
            var IsFixedFilter = Convert.ToString(Request["sSearch_4"]);
            var AmountFrom = 0;
            var AmountTo = 0;
            if (TaxValueFilter.Contains('~'))
            {
                AmountFrom = TaxValueFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(TaxValueFilter.Split('~')[0]) == true ? Convert.ToInt32(TaxValueFilter.Split('~')[0]) : 0;
                AmountTo = TaxValueFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(TaxValueFilter.Split('~')[1]) == true ? Convert.ToInt32(TaxValueFilter.Split('~')[1]) : 0;
            }
            var IsFixedFilter1 = IsFixedFilter.ToLower() == "fixed" ? true.ToString() : false.ToString();
            #endregion Column Search
            var getAllData = _repo.SelectAll();
            IEnumerable<TaxStructureVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.TaxValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.IsFixed.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (CodeFilter != "" || NameFilter != "" || (TaxValueFilter != "~" && TaxValueFilter != "") || IsFixedFilter != "")
            {
                filteredData = filteredData
                                .Where(c =>
                                            (CodeFilter == "" || c.Code.ToLower().Contains(CodeFilter.ToLower()))
                                            &&
                                           (NameFilter == "" || c.Name.ToLower().Contains(NameFilter.ToLower()))
                                           &&
                                           (IsFixedFilter == "" || c.IsFixed.ToString().ToLower().Contains(IsFixedFilter1.ToLower()))
                                            &&
                                            (AmountFrom == 0 || AmountFrom <= Convert.ToInt32(c.TaxValue))
                                            &&
                                            (AmountTo == 0 || AmountTo >= Convert.ToInt32(c.TaxValue))
                                        );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<TaxStructureVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Name :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.TaxValue.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.IsFixed.ToString() :
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
                             , c.Code
                             , c.Name
                             , c.TaxValue.ToString()
                             , c.IsFixed==true? "Fixed":"Rate"
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
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return PartialView();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(TaxStructureVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = Identity.Name;
            vm.CreatedFrom = Identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(Identity.BranchId);
            try
            {
                result = _repo.Insert(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            TaxStructureVM vm = new TaxStructureVM();
            vm = _repo.SelectById(id);
            return PartialView(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(TaxStructureVM vm, string btn)
        {
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = Identity.Name;
            vm.LastUpdateFrom = Identity.WorkStationIP;
            try
            {
                result = _repo.Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Updated";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult Delete(string id)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            TaxStructureVM vm = new TaxStructureVM();
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.Id = id;
            try
            {
                //string[] result = new string[6];
                //result = _repo.Delete(vm);
                //Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Deleted";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult TaxStructureDelete(string ids)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            TaxStructureVM vm = new TaxStructureVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        #endregion Actions

        #region EmployeeTax
        public ActionResult ImportEmployeeTax()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        public ActionResult ImportEmployeeTaxExcel(HttpPostedFileBase file)
        {
            string[] result = new string[6];
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + file.FileName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(fullPath);
                }
                ShampanIdentityVM vm = new ShampanIdentityVM();
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                EmployeeTaxRepo _eTaxRepo = new EmployeeTaxRepo();
                result = _eTaxRepo.ImportExcelFile(fullPath, file.FileName, vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportEmployeeTax");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportEmployeeTax");
            }
        }
        public ActionResult DownloadExcel(HttpPostedFileBase file, string projectId, string departmentId, string sectionId, string fid)
        {
            //if (projectId == "0_0" || projectId == "0" || projectId == "" || projectId == "null" || projectId == null)
            //{
            //    projectId = "";
            //}
            //if (departmentId == "0_0" || departmentId == "0" || departmentId == "" || departmentId == "null" || departmentId == null)
            //{
            //    departmentId = "";
            //}
            //if (sectionId == "0_0" || sectionId == "0" || sectionId == "" || sectionId == "null" || sectionId == null)
            //{
            //    sectionId = "";
            //}
            string[] result = new string[6];
            DataTable dt = new DataTable();
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                ExcelPackage xlPackage = new ExcelPackage();
                string FileName = "Download.xlsx";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }
                EmployeeTaxVM vm = new EmployeeTaxVM();
                EmployeeTaxRepo _repo = new EmployeeTaxRepo();
                //string[] conFields = { "a.DepartmentId", "a.ProjectId", "a.SectionId" };
                //string[] conValues = { departmentId, projectId, sectionId };
                //dt = _repo.ExportExcelFile(vm, conFields, conValues);
                dt = _repo.ExportExcelFile(vm);
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=EmployeeTax.xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                result[0] = "Success";
                result[1] = "Data Saved Successfully!";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportEmployeeTax");
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;
                Session["result"] = result[0] + "~" + result[1];
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportEmployeeTax");
            }
        }


        #endregion EmployeeTax
    }
}
