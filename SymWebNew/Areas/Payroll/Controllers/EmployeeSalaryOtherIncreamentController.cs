using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Excel;
using OfficeOpenXml;
using SymRepository.Enum;
using SymRepository.Tax;

namespace SymWebUI.Areas.Payroll.Controllers
{
    public class EmployeeSalaryOtherIncreamentController : Controller
    {
        //
        // GET: /Payroll/EmployeeSalaryOtherIncreament/
        #region Declare
        EmployeeInfoRepo _emprepo = new EmployeeInfoRepo();
        EmployeeSalaryOtherIncreamentRepo _repo = new EmployeeSalaryOtherIncreamentRepo();

        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        #endregion Declare
        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_41", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View();
        }
        
        public ActionResult _IndexPartial(string ProjectId, string DepartmentId, string SectionId, string DesignationId
      , string CodeF, string CodeT)
        {
            EmployeeInfoVM vm = new EmployeeInfoVM();
            vm.ProjectId = ProjectId;
            vm.DepartmentId = DepartmentId;
            vm.SectionId = SectionId;
            vm.DesignationId = DesignationId;
            vm.CodeF = CodeF;
            vm.CodeT = CodeT;
            return PartialView("_Index", vm);
        }
       
        public ActionResult _Index(JQueryDataTableParamVM param, string ProjectId, string DepartmentId, string SectionId, string DesignationId
         , string CodeF, string CodeT)
        {
            #region Declare Variable
            string vProjectId = "0_0";
            string vDepartmentId = "0_0";
            string vSectionId = "0_0";
            string vDesignationId = "0_0";
            string vCodeF = "0_0";
            string vCodeT = "0_0";
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (!(identity.IsAdmin || identity.IsPayroll))
            {
                //Id = identity.EmployeeId;
                vCodeF = identity.EmployeeCode;
                vCodeT = identity.EmployeeCode;
            }
            else
            {
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                {
                    vProjectId = ProjectId;
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                {
                    vSectionId = SectionId;
                }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                {
                    vDesignationId = DesignationId;
                }
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                {
                    vCodeF = CodeF;
                }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                {
                    vCodeT = CodeT;
                }
            }
            #endregion Declare Variable
            var getAllData = _emprepo.EmployeeList(vCodeF, vCodeT, vDepartmentId, vSectionId, vProjectId, vDesignationId);
            IEnumerable<EmployeeInfoVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable0 = Convert.ToBoolean(Request["bSearchable_0"]);
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                //////var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                //////var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                filteredData = getAllData
                   .Where(c => isSearchable0 && c.Id.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable1 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
                    //////|| isSearchable7 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                    //////|| isSearchable8 && c.GrossSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }
            var isSortable_0 = Convert.ToBoolean(Request["bSortable_0"]);
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            //////var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            //////var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c => sortColumnIndex == 0 && isSortable_0 ? c.Id :
                                                           sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Department :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Section :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Project :
                //////sortColumnIndex == 7 && isSortable_7 ? c.BasicSalary.ToString() :
                //////sortColumnIndex == 8 && isSortable_8 ? c.GrossSalary.ToString() :
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
                               c.Id
                             , c.Code
                             , c.FullName
                             , c.Designation
                             , c.Department
                             , c.Section
                             , c.Project
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Selectemployee(string Id)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_41", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeStructureRepo _empstructRepo = new EmployeeStructureRepo();
            List<EmployeeSalaryStructureVM> employeeSalaryStructureVMs = new List<EmployeeSalaryStructureVM>();
            employeeSalaryStructureVMs = _empstructRepo.SelectEmployeeSalaryStructureDetailAll(Id);
            return PartialView("_IncreamentDetail", employeeSalaryStructureVMs);
        }

        public ActionResult LastIncrement(string EmployeeId)
        {
            ////////var permission = _reposur.SymRoleSession(identity.UserId, "1_41", "index").ToString();
            ////////Session["permission"] = permission;
            ////////if (permission == "False")
            ////////{
            ////////    return Redirect("/Payroll/Home");
            ////////}
            string[] cFields = { "EmployeeId", "IsCurrent" };
            string[] cValues = { EmployeeId, "1" };

            EmployeeStructureRepo _empstructRepo = new EmployeeStructureRepo();
            EmployeeSalaryStructureVM vm = new EmployeeSalaryStructureVM();
            vm = _empstructRepo.SelectAll("", cFields, cValues).FirstOrDefault();

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(List<EmployeeSalaryStructureVM> VMs, string incDate)
        {
            EmployeeSalaryOtherIncreamentRepo _esoIncrementRepo = new EmployeeSalaryOtherIncreamentRepo();
            EmployeeSalaryStructureVM vm = new EmployeeSalaryStructureVM();
            vm.EmployeeId = VMs.FirstOrDefault().EmployeeId;
            vm.IncrementDate = incDate;

            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.BranchId = identity.BranchId;

                result = _esoIncrementRepo.InsertEmployeeSalaryStructure(VMs, vm);
                var msg = result[0] + "~" + result[1];
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }

        
        public ActionResult MultipleCreateEdit(EmployeeSalaryStructureVM VM)
        {
             string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;   
            EmployeeSalaryOtherIncreamentRepo _esoIncrementRepo = new EmployeeSalaryOtherIncreamentRepo();
            EmployeeStructureRepo _empstructRepo = new EmployeeStructureRepo();
            SalaryStructureMatrixRepo _repo = new SalaryStructureMatrixRepo();

            try
            {
                foreach (var item in VM.IDs)
                {

                    EmployeeSalaryStructureVM vm = new EmployeeSalaryStructureVM();
                    EmployeeSalaryStructureVM SalaryStructurevm = new EmployeeSalaryStructureVM();
                    List<EmployeeSalaryStructureVM> employeeSalaryStructureVMs = new List<EmployeeSalaryStructureVM>();
                    employeeSalaryStructureVMs = _empstructRepo.SelectEmployeeSalaryStructureDetailAll(item);
                    SalaryStructurevm = employeeSalaryStructureVMs.FirstOrDefault();
                    string amount = "0";
                    decimal Basic = 0;
                    decimal HouseRent = 0;
                    decimal Medical = 0;
                    decimal Conveyance = 0;
                    
                    if (!string.IsNullOrWhiteSpace(SalaryStructurevm.GradeId))
                    {
                        amount = _repo.BasicAmount(SalaryStructurevm.GradeId, "", VM.CurrentYear, SalaryStructurevm.StepSL, "");
                    }
                    var arra = amount.Split('~');
                    
                    Basic = Convert.ToDecimal(arra[0]);

                    if (arra.Length > 1) {
                    HouseRent = Convert.ToDecimal(arra[1]);
                    Medical = Convert.ToDecimal(arra[2]);
                    Conveyance = Convert.ToDecimal(arra[3]);
                    }
                    foreach (EmployeeSalaryStructureVM vmD in employeeSalaryStructureVMs)
                    {
                        if (vmD.SalaryType.ToLower() == "basic")
                        {
                            vmD.AfterIncrementValue = Basic;
                            vmD.IncrementValue = vmD.AfterIncrementValue - vmD.TotalValue;
                        }
                        if (vmD.SalaryType.ToLower() == "houserent")
                        {
                            vmD.AfterIncrementValue = HouseRent;
                            vmD.IncrementValue = vmD.AfterIncrementValue - vmD.TotalValue;
                        }
                        if (vmD.SalaryType.ToLower() == "medical")
                        {
                            vmD.AfterIncrementValue = Medical;
                            vmD.IncrementValue = vmD.AfterIncrementValue - vmD.TotalValue;
                        }
                        if (vmD.SalaryType.ToLower() == "conveyance")
                        {
                            vmD.AfterIncrementValue = Conveyance;
                            vmD.IncrementValue = vmD.AfterIncrementValue - vmD.TotalValue;
                        }
                        if (vmD.SalaryType.ToLower() == "gross")
                        {
                            vmD.AfterIncrementValue = Basic + HouseRent + Medical + Conveyance;
                            vmD.IncrementValue = vmD.AfterIncrementValue - vmD.TotalValue;
                        }
                    }
                    vm.EmployeeId = item;
                    vm.IncrementDate = VM.IncrementDate;
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    vm.BranchId = identity.BranchId;
                    
                    result = _esoIncrementRepo.InsertEmployeeSalaryStructure(employeeSalaryStructureVMs, vm);
                }
                var msg = result[0] + "~" + result[1];
                Session["result"] = msg;

                return RedirectToAction("Index");
            }
                 
            catch (Exception e)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
            
              

            
        }


        [Authorize(Roles = "Admin"), HttpGet]
        public ActionResult ExcelImport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase file)
        {
            string[] result = new string[6];
            try
            {
                if (file == null)
                {
                    result[0] = "Fail";
                    result[1] = "Select Excel File!";
                    throw new ArgumentNullException();
                }


                #region Comments

                //////var filePath = Server.MapPath("~/Files/Export/") + file.FileName;

                //////if (System.IO.File.Exists(filePath))
                //////    System.IO.File.Delete(filePath);

                //////if (file.ContentLength > 0)
                //////    file.SaveAs(filePath);

                //////var table = ReadExcel(file, filePath);

                ////////var salaryTypes = new EnumSalaryTypeRepo().SelectAll(1);

                //////var viewModels = new List<EmployeeSalaryStructureVM>();
                //////var empstructRepo = new EmployeeStructureRepo();

                //////foreach (DataRow row in table.Rows)
                //////{
                //////    if (string.IsNullOrEmpty(row["EmployeeId"].ToString()) || string.IsNullOrWhiteSpace(row["EmployeeId"].ToString()))
                //////        throw new ArgumentNullException("EmployeeId");

                //////    var employeeSalaryStructureVMs = empstructRepo.SelectEmployeeSalaryStructureDetailAll(row["EmployeeId"].ToString());

                //////    foreach (DataColumn column in table.Columns)
                //////    {
                //////        if (column.ColumnName == "EmployeeId" || column.ColumnName == "Increment Date"||column.ColumnName == "Employee Name"
                //////            || column.ColumnName == "Code")
                //////        {
                //////            continue;
                //////        }

                //////        if(string.IsNullOrEmpty(row["Increment Date"].ToString()) || string.IsNullOrWhiteSpace(row["Increment Date"].ToString()))
                //////            throw new ArgumentNullException("Increment Date");

                //////        var vm = employeeSalaryStructureVMs.FirstOrDefault(x => x.SalaryType == column.ColumnName);


                //////        if (vm != null)
                //////        {
                //////            vm.IncrementValue = string.IsNullOrEmpty(row[column.ColumnName].ToString()) ? 0 : Convert.ToDecimal(row[column.ColumnName]);
                //////            vm.IncrementDate = string.IsNullOrEmpty(row["Increment Date"].ToString()) ? null : row["Increment Date"].ToString();
                //////            viewModels.Add(vm);

                //////        }


                //////    }

                #endregion

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                EmployeeSalaryStructureVM vm = new EmployeeSalaryStructureVM();
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.BranchId = identity.BranchId;
                vm.file = file;

                result = _repo.Insert_Increament(vm);

            }
            catch (Exception ex)
            {
            }
            finally
            {


            }

            Session["result"] = result[0] + "~" + result[1];
            return RedirectToAction("ExcelImport");


        }

        [Authorize(Roles = "Admin")]
        public ActionResult DownloadExcel(EmployeeInfoVM vm)
        {
            try
            {
                ////string orderBy = null
                var result = new string[6];

                DataTable dt = new DataTable();

                ////EmployeeInfoVM vm = new EmployeeInfoVM();
                ////vm.OrderBy = orderBy;
                
                dt = _repo.SelectAll_For_Increment(vm);

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment;  filename=" + FileName);
                    Response.AddHeader("content-disposition", "attachment;  filename=" + "Employee List - " + DateTime.Now.ToShortDateString() + ".xlsx");
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

        #region Comments

        //////private string[] SaveSalaryIncrement(List<EmployeeSalaryStructureVM> VMs, string incDate)
        //////{
        //////    vm.EmployeeId = VMs.FirstOrDefault().EmployeeId;
        //////    vm.IncrementDate = incDate;

        //////    string[] result = new string[6];


        //////    result = _esoIncrementRepo.InsertEmployeeSalaryStructure(VMs, vm);
        //////    var msg = result[0] + "~" + result[1];

        //////    return result;
        //////}

        #endregion

        #region Comments

        ////private DataTable ReadExcel(HttpPostedFileBase file, string filePath)
        ////{
        ////    IExcelDataReader reader = null;
        ////    FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);

        ////    if (file.FileName.EndsWith(".xls"))
        ////    {
        ////        reader = ExcelReaderFactory.CreateBinaryReader(stream);
        ////    }
        ////    else if (file.FileName.EndsWith(".xlsx"))
        ////    {
        ////        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        ////    }

        ////    reader.IsFirstRowAsColumnNames = true;
        ////    var dataSet = reader.AsDataSet();
        ////    var table = dataSet.Tables[0];
        ////    reader.Close();
        ////    return table;
        ////}

        #endregion


    }
}
