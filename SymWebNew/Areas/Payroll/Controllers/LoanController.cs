using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.Enum;
using SymRepository.HRM;
using SymRepository.Loan;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymViewModel.HRM;
using SymViewModel.Loan;
//using SymWebUI.Areas.Payroll.Reports.PayrollEntry;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
namespace SymWebUI.Areas.Payroll.Controllers
{
    public class LoanController : Controller
    {
        //
        // GET: /HRM/Loan/
				SymUserRoleRepo _reposur = new SymUserRoleRepo();
				ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index(string id)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_51", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (!(identity.IsAdmin || identity.IsPayroll))
            {
                id = identity.EmployeeId;
            }
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM empVm = _infoRepo.SelectById(id);
            empVm.Id = id;
            //empVm.fileName = id + ".jpg";
            //string directory = Server.MapPath(@"~/Files/EmployeeInfo\") + id + ".jpg";
            //if (!System.IO.File.Exists(directory))
            //{
            //    empVm.fileName = "0.jpg";
            //}
            return View(empVm);
        }
        public ActionResult AllLoan()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_51", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View();
        }
        
        public ActionResult _index(JQueryDataTableParamModel param, string code, string name, string employeeId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeLoanRepo _loanRepo = new EmployeeLoanRepo();
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var loanTypeFilter = Convert.ToString(Request["sSearch_5"]);
            var totalAmountFilter = Convert.ToString(Request["sSearch_6"]);
            var startDateFilter = Convert.ToString(Request["sSearch_7"]);            
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (startDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = startDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Convert.ToDateTime(startDateFilter.Split('~')[0]);
                toDate = startDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Convert.ToDateTime(startDateFilter.Split('~')[1]);
            }
            var amountFrom = 0;
            var amountTo = 0;
            if (totalAmountFilter.Contains('~'))
            {
                amountFrom = totalAmountFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(totalAmountFilter.Split('~')[0]);
                amountTo = totalAmountFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(totalAmountFilter.Split('~')[1]);
            }
            var fromID = 0;
            var toID = 0;
            if (idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }
            #endregion Column Search
            #region Search and Filter Data
            var getAllData = _loanRepo.SelectAll(Convert.ToInt32(identity.BranchId),employeeId);
                     

            IEnumerable<EmployeeLoanVM> filteredData;
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
                filteredData = getAllData
                   .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.LoanType.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.StartDate.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            #region Column Filtering
            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || loanTypeFilter != "" || (totalAmountFilter != "" && totalAmountFilter != "~") || (startDateFilter != "" && startDateFilter != "~"))
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
                                    (loanTypeFilter == "" || c.LoanType.ToLower().Contains(loanTypeFilter.ToLower()))
                                    &&
                                    (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.StartDate))
                                    &&
                                    (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.StartDate))
                                    &&
                                    (amountFrom == 0 || amountFrom <= Convert.ToInt32(c.TotalAmount))
                                    &&
                                    (amountTo == 0 || amountTo >= Convert.ToInt32(c.TotalAmount))
                                );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeLoanVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                sortColumnIndex == 5 && isSortable_5 ? c.LoanType :
                sortColumnIndex == 6 && isSortable_6 ? Ordinary.DateToString(c.StartDate) :
                sortColumnIndex == 7 && isSortable_7 ? c.TotalAmount.ToString() :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { 
               Convert.ToString(c.Id) 
                , c.Code //+ "~" + Convert.ToString(c.Id) 
                , c.EmpName 
                , c.Department 
                , c.Designation , c.LoanType// + "~" + Convert.ToString(c.Id)
                , c.TotalAmount.ToString()
                , c.StartDate
                //, c.Remarks 
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
        public ActionResult Create(string employeeId)
        {
            //List<EmployeeLoanVM> pas = new List<EmployeeLoanVM>();
            //pas.Add(new EmployeeLoanVM() { Code = "Gross" });
            //pas.Add(new EmployeeLoanVM() { Code = "Other Earning" });
            //pas.Add(new EmployeeLoanVM() { Code = "Gross" });
            //pas.Add(new EmployeeLoanVM() { Code = "Gross" });
            //pas.Add(new EmployeeLoanVM() { Code = "Gross" });
            //pas.Add(new EmployeeLoanVM() { Code = "Gross" });
            //pas.Add(new EmployeeLoanVM() { Code = "Gross" });
            //pas.Add(new EmployeeLoanVM() { Code = "Gross" });
            //return View(pas);
               var permission= _reposur.SymRoleSession(identity.UserId, "1_51", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
             


            EmployeeLoanVM vm = new EmployeeLoanVM();
            EmployeeVM emp = new EmployeeInfoRepo().EmployeeInfo(employeeId);
            vm.EmployeeId = employeeId;
            vm.Employee = emp.FullName;
            vm.Code = emp.Code;
            vm.Project = emp.Project;
            vm.Section = emp.Section;
            vm.Department = emp.Department;
            vm.Designation = emp.Designation;
            return View(vm);
        }
        [HttpPost]
        public ActionResult Create(EmployeeLoanVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                SettingRepo sRepo = new SettingRepo();
                bool FromSetting =Convert.ToBoolean( sRepo.settingValue("PFLoanRate", "FromSetting")=="Y"?true:false);
                int Upto12Month =Convert.ToInt32( sRepo.settingValue("PFLoanRate", "Upto12Month"));
                int GetterThen12Month = Convert.ToInt32(sRepo.settingValue("PFLoanRate", "GetterThen12Month"));

                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.BranchId = Convert.ToInt32(identity.BranchId);
                ////////////////
                //if (intPolicy == "Reduce")
                //{
                //    dIAmount = parseFloat(cPAmount) * parseFloat(parseFloat(InterestRate) / 100 / 12);
                //    totalInterest = parseFloat(totalInterest) + parseFloat(dIAmount);
                //    dTAmount = parseFloat(dPAmount) + parseFloat(dIAmount);
                //    cPAmount = parseFloat(cPAmount) - parseFloat(dPAmount);
                //    Subtotal = parseFloat(Subtotal) + parseFloat(dTAmount);
                //}
                //else if (intPolicy == "Fixed")
                //{
                //    dIAmount = parseFloat(InterestAmount) / parseFloat(installment);
                //    totalInterest = parseFloat(totalInterest) + parseFloat(dIAmount);
                //    dTAmount = parseFloat(dPAmount) + parseFloat(dIAmount);
                //    Subtotal = parseFloat(Subtotal) + parseFloat(dTAmount);
                //}
                //else if (intPolicy == "Rate")
                //{
                //    dIAmount = parseFloat(PrincipalAmount) * parseFloat(InterestRate) / 100 / 12;
                //    totalInterest = parseFloat(totalInterest) + parseFloat(dIAmount);
                //    dTAmount = parseFloat(dPAmount) + parseFloat(dIAmount);
                //    Subtotal = parseFloat(Subtotal) + parseFloat(dTAmount);
                //}
                //////////
                vm.IsHold = false;
                decimal cPAmount = vm.PrincipalAmount;
                decimal dPAmount = 0;
                 decimal dIAmount =0;
                 decimal dTAmount = 0;
                 int iLoop = 1;

                 var fractionValue = "0";

                List<MonthCalculation> monthCalculations = Ordinary.MonthCalculation(Ordinary.DateToString(vm.StartDate), vm.NumberOfInstallment);
                List<EmployeeLoanDetailVM> loanDetails = new List<EmployeeLoanDetailVM>();
                EmployeeLoanDetailVM loanDetail;

                var amount = vm.PrincipalAmount / vm.NumberOfInstallment;
                var actualValue = amount.ToString().Split('.')[0];

                if (amount == Math.Round(amount))
                {
                     fractionValue = "0";
                }
                else
                {
                     fractionValue = amount.ToString().Split('.')[1];
                }             
                var totalCount = vm.NumberOfInstallment - 1;


                #region  Reduce policy
                double annualInterestRate =Convert.ToDouble( vm.InterestRate/100); // Annual interest rate (10%)
                int loanTermMonths = vm.NumberOfInstallment; // Loan term in months
                // Calculate monthly interest rate

                double monthlyInterestRate = annualInterestRate / 12;

                double PrincipleAmount = 0;
                PrincipleAmount = Convert.ToDouble( cPAmount);

                // Calculate installment amount manually
                double installmentValue = (PrincipleAmount * monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, loanTermMonths)) / (Math.Pow(1 + monthlyInterestRate, loanTermMonths) - 1);
                #endregion

                for (int i = 0; i < monthCalculations.Count; i++)
                {
                    if (vm.InterestPolicy == "Reduce")
                    {
                        //dIAmount = (cPAmount * vm.InterestRate / 100 /12)* vm.NumberOfInstallment;
                        //dPAmount = vm.PrincipalAmount / vm.NumberOfInstallment;
                        //dTAmount = dPAmount + dIAmount;
                        //cPAmount = cPAmount - dPAmount;


                        double NewPrincipleAmount = Math.Round(PrincipleAmount);

                        double InterestAmount = Math.Round(NewPrincipleAmount *annualInterestRate / 12);
                        double PrinciipleWithInterestAmount = Math.Round(PrincipleAmount + InterestAmount);
                        double Deposit = Math.Round(installmentValue);
                        if (i == totalCount)
                        {
                            Deposit = Math.Round(PrinciipleWithInterestAmount);

                        }
                        double InterestPart = Math.Round(InterestAmount);
                        double PrinciplePart = Math.Round(Deposit - InterestPart);


                        PrincipleAmount = Math.Round(PrinciipleWithInterestAmount - installmentValue);

                        dPAmount = Convert.ToDecimal(PrinciplePart);
                        dIAmount =  Convert.ToDecimal(InterestPart);
                        dTAmount =Convert.ToDecimal( Deposit);
                       
                    }
                    else
                    {
                        dPAmount = vm.PrincipalAmount / vm.NumberOfInstallment;
                        dIAmount = vm.InterestAmount / vm.NumberOfInstallment;
                        dTAmount = dPAmount + dIAmount;
                    }

                    if (vm.InterestPolicy != "Reduce")
                    {
                        if (totalCount == i)
                        {
                            dPAmount = vm.PrincipalAmount - (Convert.ToInt32(actualValue) * (vm.NumberOfInstallment - 1));

                            dIAmount = Convert.ToDecimal(dIAmount);
                            var val = dIAmount.ToString().Split('.')[0];

                            dIAmount = Convert.ToInt32(vm.InterestAmount) - (Convert.ToInt32(val) * (vm.NumberOfInstallment - 1));
                        }
                        else
                        {
                            dPAmount = Math.Floor(dPAmount);
                            dIAmount = Math.Floor(dIAmount);
                        }
                    }
                    dTAmount = dPAmount + dIAmount;

                    loanDetail = new EmployeeLoanDetailVM();
                    loanDetail.PrincipalAmount = dPAmount;
                    loanDetail.InterestAmount = dIAmount;
                    loanDetail.InstallmentAmount = dTAmount;
                    loanDetail.InstallmentPaidAmount = 0;
                    loanDetail.PaymentScheduleDate = monthCalculations[i].StartDate;
                    loanDetail.PaymentDate = "";
                    loanDetail.IsHold = false;
                    loanDetail.IsPaid = false;
                    loanDetail.Remarks = "";
                    loanDetails.Add(loanDetail);
                    vm.EndDate = monthCalculations[i].StartDate;
                }



                //foreach (MonthCalculation item in monthCalculations)
                //{
                //   if (vm.InterestPolicy == "Reduce")
                //   {
                //       dIAmount = cPAmount * vm.InterestRate / 100 / 12;// vm.NumberOfInstallment;
                //       dPAmount = vm.PrincipalAmount / vm.NumberOfInstallment;
                //       dTAmount = dPAmount + dIAmount;
                //       cPAmount = cPAmount - dPAmount;
                //   }

                //   else if (vm.InterestPolicy == "Fixed")
                //   {
                //       dIAmount = vm.InterestAmount / vm.NumberOfInstallment;
                //       dPAmount = vm.PrincipalAmount / vm.NumberOfInstallment;

                //       dTAmount = dPAmount + dIAmount;

                //   }
                //   else
                //   {
                //       //if (FromSetting)
                //       //{
                //       //    if (iLoop <= 12)
                //       //    {
                //       dIAmount = vm.PrincipalAmount * vm.InterestRate / 100 / 12;
                //       //    }
                //       //    else
                //       //    {
                //       //        dIAmount = vm.PrincipalAmount * GetterThen12Month / 100 / 12;
                //       //    }

                //       //}
                //       //else
                //       //{

                //       //    dIAmount = vm.InterestAmount / vm.NumberOfInstallment;
                //       //}
                //       dPAmount = vm.PrincipalAmount / vm.NumberOfInstallment;
                //       //dIAmount = vm.InterestAmount / vm.NumberOfInstallment;
                //       dTAmount = dPAmount + dIAmount;
                      
                //   }
                //   loanDetail = new EmployeeLoanDetailVM();
                //   loanDetail.PrincipalAmount = dPAmount;
                //   loanDetail.InterestAmount = dIAmount;
                //   loanDetail.InstallmentAmount = dTAmount;
                //   loanDetail.InstallmentPaidAmount = 0;
                //   loanDetail.PaymentScheduleDate = item.StartDate;
                //   loanDetail.PaymentDate = "";
                //   loanDetail.IsHold = false;
                //   loanDetail.IsPaid = false;
                //   loanDetail.Remarks = "";
                //   loanDetails.Add(loanDetail);
                //   vm.EndDate = item.StartDate;
                //   iLoop++;
                //}

                vm.employeeLoanDetails = loanDetails;

                ////foreach (var item in vm.employeeLoanDetails)
                ////{
                ////    bool PeriodLock = false;

                ////    string PaymentScheduleDate = Convert.ToDateTime(item.PaymentScheduleDate).ToString("MMM-yy");

                ////    string[] conditionFields = { "PeriodName" };
                ////    string[] conditionValues = { PaymentScheduleDate };

                ////    var fiscalPeriod = new FiscalYearRepo().SelectAll_FiscalYearDetail(0, conditionFields, conditionValues).FirstOrDefault();

                ////    if (fiscalPeriod != null)
                ////    {
                ////        PeriodLock = fiscalPeriod.PeriodLock;
                ////    }

                ////    if (PeriodLock)
                ////    {
                ////        Session["result"] = "Fail" + "~" + PaymentScheduleDate + " Fiscal Period is Locked";

                ////        return View(vm);
                ////    }
                    
                ////}



                result = new EmployeeLoanRepo().EmployeeLoanInsert(vm);
                Session["result"] = result[0] + "~" + result[1];
                //return RedirectToAction("Index", new { id = vm .EmployeeId});
                return RedirectToAction("AllLoan");
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
            }
            return View(vm);
        }
        public JsonResult MonthCalculation(string date, int number)
        {
          

            return Json(Ordinary.MonthCalculation(Ordinary.DateToString(date),number), JsonRequestBehavior.AllowGet);


        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult EmployeeInfoForLoan()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_51", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View("_Employee");
        }
        [Authorize]
        public ActionResult _EmployeeInfoForLoan(JQueryDataTableParamVM param, string code, string name)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_5"]);
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
            }
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
            var getAllData = _empRepo.SelectAll();
            IEnumerable<EmployeeInfoVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
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
            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || (joinDateFilter != "" && joinDateFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
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
                sortColumnIndex == 6 && isSortable_6 ? c.Remarks :
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
                , c.Code //+ "~" + Convert.ToString(c.Id) 
                , c.EmpName 
                , c.Department 
                , c.Designation 
                , c.JoinDate.ToString()      
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
        public ActionResult Edit(string loanId)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            EmployeeLoanVM vm = new EmployeeLoanVM();
            vm = loanRepo.SelectLoan(loanId);
            return View(vm);
        }
        [HttpPost]
        public ActionResult Edit(EmployeeLoanVM vm, string Save )
        {
            Save = vm.Operation;
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (Save == "Save")
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.BranchId = Convert.ToInt32(identity.BranchId);
                vm.IsHold = false;
                decimal cPAmount = vm.PrincipalAmount;
                decimal dPAmount = 0;
                decimal dIAmount = 0;
                decimal dTAmount = 0;
                List<MonthCalculation> monthCalculations = Ordinary.MonthCalculation(Ordinary.DateToString(vm.StartDate), vm.NumberOfInstallment);
                List<EmployeeLoanDetailVM> loanDetails = new List<EmployeeLoanDetailVM>();
                EmployeeLoanDetailVM loanDetail;

                var amount = vm.PrincipalAmount / vm.NumberOfInstallment;
                var actualValue = amount.ToString().Split('.')[0];
                var fractionValue = amount.ToString().Split('.')[1];
                var totalCount = vm.NumberOfInstallment - 1;
                #region  Reduce policy
                double annualInterestRate = Convert.ToDouble(vm.InterestRate / 100); // Annual interest rate (10%)
                int loanTermMonths = 36; // Loan term in months
                // Calculate monthly interest rate

                double monthlyInterestRate = annualInterestRate / 12;

                double PrincipleAmount = 0;
                PrincipleAmount = Convert.ToDouble(cPAmount);

                // Calculate installment amount manually
                double installmentValue = (PrincipleAmount * monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, loanTermMonths)) / (Math.Pow(1 + monthlyInterestRate, loanTermMonths) - 1);
                #endregion

                for (int i = 0; i < monthCalculations.Count; i++)
                {
                    if (vm.InterestPolicy == "Reduce")
                    {
                        //dIAmount = (cPAmount * vm.InterestRate / 100 /12)* vm.NumberOfInstallment;
                        //dPAmount = vm.PrincipalAmount / vm.NumberOfInstallment;
                        //dTAmount = dPAmount + dIAmount;
                        //cPAmount = cPAmount - dPAmount;

                        double NewPrincipleAmount = Math.Round(PrincipleAmount);

                        double InterestAmount = Math.Round(NewPrincipleAmount * annualInterestRate / 12);
                        double PrinciipleWithInterestAmount = Math.Round(PrincipleAmount + InterestAmount);
                        double Deposit = Math.Round(installmentValue);
                        if (i == totalCount)
                        {
                            Deposit = Math.Round(PrinciipleWithInterestAmount);

                        }
                        double InterestPart = Math.Round(InterestAmount);
                        double PrinciplePart = Math.Round(Deposit - InterestPart);


                        PrincipleAmount = Math.Round(PrinciipleWithInterestAmount - installmentValue);

                        dPAmount = Convert.ToDecimal(PrinciplePart);
                        dIAmount = Convert.ToDecimal(InterestPart);
                        dTAmount = Convert.ToDecimal(Deposit);
                       
                    }
                    else
                    {
                        dPAmount = vm.PrincipalAmount / vm.NumberOfInstallment;
                        dIAmount = vm.InterestAmount / vm.NumberOfInstallment;
                        dTAmount = dPAmount + dIAmount;
                    }
                    if (vm.InterestPolicy != "Reduce")
                    {
                        if (totalCount == i)
                        {
                            dPAmount = vm.PrincipalAmount - (Convert.ToInt32(actualValue) * (vm.NumberOfInstallment - 1));

                            dIAmount = Convert.ToDecimal(dIAmount);
                            var val = dIAmount.ToString().Split('.')[0];

                            dIAmount = Convert.ToInt32(vm.InterestAmount) - (Convert.ToInt32(val) * (vm.NumberOfInstallment - 1));

                        }
                        else
                        {
                            dPAmount = Math.Floor(dPAmount);
                            dIAmount = Math.Floor(dIAmount);
                        }
                    }

                    loanDetail = new EmployeeLoanDetailVM();
                    loanDetail.PrincipalAmount = dPAmount;
                    loanDetail.InterestAmount = dIAmount;
                    loanDetail.InstallmentAmount = dTAmount;
                    loanDetail.InstallmentPaidAmount = 0;
                    loanDetail.PaymentScheduleDate = monthCalculations[i].StartDate;
                    loanDetail.PaymentDate = "";
                    loanDetail.IsHold = false;
                    loanDetail.IsPaid = false;
                    loanDetail.Remarks = "";
                    loanDetails.Add(loanDetail);
                    vm.EndDate = monthCalculations[i].StartDate;
                }

                vm.employeeLoanDetails = loanDetails;
                result = new EmployeeLoanRepo().EmployeeLoanInsert(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("AllLoan");
            }
            else
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.BranchId = Convert.ToInt32(identity.BranchId);
                EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
                result = loanRepo.EmployeeLoanUpdate(vm);
                Session["result"] = result[0] + "~" + result[1];
                //var mgs = result[0] + "~" + result[1];
                return RedirectToAction("AllLoan");
                //return Json(mgs, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult PaidEdit(string Id,string emploanId)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            EmployeeLoanDetailVM vm = new EmployeeLoanDetailVM();
            vm = loanRepo.SelectEmployeeLoan(Id, emploanId);
            return PartialView("PaidEdit",vm);
        }
        [HttpPost]
        public ActionResult PaidEdit(EmployeeLoanDetailVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.BranchId = identity.BranchId;
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
              result  = loanRepo.PaidEdit(vm);
                Session["result"] = result[0] + "~" + result[1];
                //return RedirectToAction("Index", new { id = vm .EmployeeId});
                return RedirectToAction("Edit", new { loanId = vm.EmployeeLoanId });
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Edit", new { loanId = vm.EmployeeLoanId });
            }
        }
        public ActionResult employeeLoanDetails(EmployeeLoanDetailVM vm)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
            return View(vm);
        }
        [HttpGet]
        public ActionResult Details(string loanId)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            EmployeeLoanVM vm = new EmployeeLoanVM();
            vm = loanRepo.SelectLoan(loanId);
            return View(vm);
        }
        public ActionResult LoanUpdate(string loanDetailsId, string employeeId, string loanId, bool ishold, bool HaveDuplicate, string remarks)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeLoanDetailVM loanDetail = new EmployeeLoanDetailVM();
            loanDetail.EmployeeLoanId = loanId;
            loanDetail.Id = Convert.ToInt32(loanDetailsId);
            loanDetail.EmployeeId = employeeId;
            loanDetail.IsHold = ishold;
            loanDetail.Remarks = remarks;
            loanDetail.HaveDuplicate = HaveDuplicate;
            loanDetail.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            loanDetail.LastUpdateBy = identity.Name;
            loanDetail.LastUpdateFrom = identity.WorkStationIP;
            //if (ishold==true)
            //     loanDetail.triger ="hold";
            if (HaveDuplicate == true)
                loanDetail.triger = "duplicate";
            else
                loanDetail.triger = "hold";
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            string[] result = new string[6];
            result = loanRepo.EmployeeLoanUpdate2(loanDetail);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoanUpdateDetail(string loanDetailsId, decimal principalAmount, decimal interestAmount, decimal installmentAmount, string remarks )
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeLoanDetailVM loanDetail = new EmployeeLoanDetailVM();
            loanDetail.Id = Convert.ToInt32(loanDetailsId);
            loanDetail.InterestAmount = interestAmount;
            loanDetail.PrincipalAmount = principalAmount;
            loanDetail.InstallmentAmount = installmentAmount;
            loanDetail.Remarks = remarks;
            loanDetail.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            loanDetail.LastUpdateBy = identity.Name;
            loanDetail.LastUpdateFrom = identity.WorkStationIP;
            loanDetail.triger = "update";
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            string[] result = new string[6];
            result = loanRepo.EmployeeLoanUpdate2(loanDetail);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ScheduleofLoanMember(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string view,string FromDate,string ToDate)
        {
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;

                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrWhiteSpace(view) || view == "Y")
                {
                    return View();
                }
                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                string vFromDate = "";
                string vToDate = "";
                string projectParam = "[All]";
                string deptParam = "[All]";
                string secParam = "[All]";
                string desigParam = "[All]";
                string codeFParam = "[All]";
                string codeTParam = "[All]";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                {
                    vDesignationId = DesignationId;
                    DesignationRepo desRepo = new DesignationRepo();
                    desigParam = desRepo.SelectById(DesignationId).Name;
                }
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                {
                    vCodeF = CodeF;
                    codeFParam = vCodeF;
                }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                {
                    vCodeT = CodeT;
                    codeTParam = vCodeT;
                }
                if (FromDate != "0_0" && FromDate != "0" && FromDate != "" && FromDate != "null" && FromDate != null)
                {
                    DateTime date = DateTime.ParseExact(FromDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    string formattedDate = date.ToString("yyyyMMdd");
                    vFromDate = formattedDate;
                  
                }
                if (ToDate != "0_0" && ToDate != "0" && ToDate != "" && ToDate != "null" && ToDate != null)
                {
                    DateTime date = DateTime.ParseExact(ToDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    string formattedDate = date.ToString("yyyyMMdd");
                    vToDate = formattedDate;                   
                }
                ReportDocument doc = new ReportDocument();
                EmployeeLoanRepo _repo = new EmployeeLoanRepo();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                List<EmployeeLoanDetailVM> getAllData = new List<EmployeeLoanDetailVM>();
                //var BranchId = Convert.ToInt32(identity.BranchId);
                //var getAllData = _repo.SelectAll(BranchId);
                string rptLocation = "";
                string ReportHead = "";

                getAllData = _repo.SelectAllForSLMPeport(vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT,vFromDate,vToDate);
                ReportHead = "There are no data to Preview for Schedule Loan Member";
                if (getAllData.Count > 0)
                {
                    if(FromDate!="")
                    {
                        ReportHead = "Schedule of Loan Member (Date of " + FromDate + " to " + ToDate + ")";
                    }
                    else
                    {
                        ReportHead = "Schedule of Loan Member";
                    }                    
                }
                table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtEmpLoan";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollEntry\rptScheduleofLoanMember.rpt";

                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                //doc = new rptLoanStatement();
                //doc.SetDataSource(ds);
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult EmployeeLoanReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string view)
        {
            try
            {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
            Session["permission"] = permission;

            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (string.IsNullOrWhiteSpace(view) || view == "Y")
                {
                    return View();
                }               
                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                string projectParam = "[All]";
                string deptParam = "[All]";
                string secParam = "[All]";
                string desigParam = "[All]";
                string codeFParam = "[All]";
                string codeTParam = "[All]";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                {
                    vDesignationId = DesignationId;
                    DesignationRepo desRepo = new DesignationRepo();
                    desigParam = desRepo.SelectById(DesignationId).Name;
                }
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                {
                    vCodeF = CodeF;
                    codeFParam = vCodeF;
                }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                {
                    vCodeT = CodeT;
                    codeTParam = vCodeT;
                }
                ReportDocument doc = new ReportDocument();
                EmployeeLoanRepo _repo = new EmployeeLoanRepo();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                List<EmployeeLoanDetailVM> getAllData = new List<EmployeeLoanDetailVM>();
                //var BranchId = Convert.ToInt32(identity.BranchId);
                //var getAllData = _repo.SelectAll(BranchId);
                string rptLocation = "";
                string ReportHead = "";
                getAllData = _repo.SelectAllForReport(vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT);
                ReportHead = "There are no data to Preview for Loan Statement";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Loan Statement";
                }
                table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtEmpLoan";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollEntry\rptLoanStatement.rpt";
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                //doc = new rptLoanStatement();
                //doc.SetDataSource(ds);
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult EmployeeLoanStatementReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string view)
        {
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;

                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrWhiteSpace(view) || view == "Y")
                {
                    return View();
                }
                string rptLocation = "";
                string ReportHead = "";

                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                string projectParam = "[All]";
                string deptParam = "[All]";
                string secParam = "[All]";
                string desigParam = "[All]";
                string codeFParam = "[All]";
                string codeTParam = "[All]";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                {
                    vDesignationId = DesignationId;
                    DesignationRepo desRepo = new DesignationRepo();
                    desigParam = desRepo.SelectById(DesignationId).Name;
                }
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                {
                    vCodeF = CodeF;
                    codeFParam = vCodeF;
                }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                {
                    vCodeT = CodeT;
                    codeTParam = vCodeT;

                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Loan\rptLoan.rpt";
                }
                ReportDocument doc = new ReportDocument();
                EmployeeLoanRepo _repo = new EmployeeLoanRepo();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                List<EmployeeLoanDetailVM> getAllData = new List<EmployeeLoanDetailVM>();
                //var BranchId = Convert.ToInt32(identity.BranchId);
                //var getAllData = _repo.SelectAll(BranchId);
              

                getAllData = _repo.SelectLoanStatementForReport(vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT);

                ReportHead = "There are no data to Preview for Loan Statement";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Loan Statement";
                }

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtEmpLoan";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollEntry\rptAllLoanStatement.rpt";
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                //doc = new rptLoanStatement();
                //doc.SetDataSource(ds);
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
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            EmployeeLoanVM vm = new EmployeeLoanVM();
            EmployeeLoanRepo _repo = new EmployeeLoanRepo();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
         public JsonResult PFBalance(string ApplicationDate, string emploanId)
         {
             //var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
             //Session["permission"] = permission;
             //if (permission == "False")
             //{
             //    return Redirect("/Payroll/Home");
             //}
             DataTable dt = new DataTable();
             string date = ApplicationDate;
             if (string.IsNullOrWhiteSpace(ApplicationDate))
             {
                 date = DateTime.Now.ToString("dd-MMM-yyyy");
             }

             EmployeeLoanVM vm = new EmployeeLoanVM();
             SettingRepo sRepo = new SettingRepo();
             EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
             dt = loanRepo.getBalance(date, emploanId);


             bool FromSetting = Convert.ToBoolean(sRepo.settingValue("PFLoanRate", "FromSetting")=="Y"?true:false);
             int Upto12Month = Convert.ToInt32(sRepo.settingValue("PFLoanRate", "Upto12Month"));
             int GetterThen12Month = Convert.ToInt32(sRepo.settingValue("PFLoanRate", "GetterThen12Month"));

             //ViewBag.FromSetting = FromSetting;
             //ViewBag.Upto12Month = Upto12Month;
             //ViewBag.GetterThen12Month = GetterThen12Month;


             if (dt !=null && dt.Rows.Count > 0)
             {
                 vm.PFBalance =Convert.ToDecimal(dt.Rows[0]["Balance"].ToString());
             }
             string AvailableRate=sRepo.settingValue("PFLoan", "AvailableRate");

             vm.AvailableRate = Convert.ToDecimal(AvailableRate);

             vm.FromSetting = FromSetting;
             vm.InterestRate = Upto12Month;
             vm.InterestRate1 = GetterThen12Month;

             vm.PFBalance = 999999999;


             return Json(vm, JsonRequestBehavior.AllowGet);
         }

         [HttpGet]
         public ActionResult ReportView(string id)
         {
             try
             {            
                 string ReportHead = "";
                 string rptLocation = "";


                 string[] cFields = { "I.Id" };
                 string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };

                 ReportDocument doc = new ReportDocument();
                 DataTable dt = new DataTable();
                 EmployeeLoanRepo _repo = new EmployeeLoanRepo();


                 dt = _repo.GetData( cFields, cValues);
                                

                 ReportHead = "There are no data to Preview for Transaction Loan";
                 if (dt.Rows.Count > 0)
                 {
                     ReportHead = "Loan Transactions";
                 }
                 dt.TableName = "dtLoan";

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
                if(CompanyName.ToLower() == "egcb")
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Loan\rptLoanEGCB.rpt";
                }
                 else
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Loan\rptLoan.rpt";
                }
                 

                 doc.Load(rptLocation);
                 doc.SetDataSource(dt);
                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                 FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;

                 doc.DataDefinition.FormulaFields["PrincipalPaid"].Text = dt.Rows[0]["PrincipalPaid"].ToString();
                 doc.DataDefinition.FormulaFields["InterestPaid"].Text = dt.Rows[0]["InterestPaid"].ToString();

                 doc.DataDefinition.FormulaFields["CompanyName"].Text = "''";


                 var rpt = RenderReportAsPDF(doc);
                 doc.Close();
                 return rpt;
             }
             catch (Exception)
             {
                 throw;
             }
         }

         public ActionResult AllLoanReport(EmployeeLoanVM vm)
         {
             string[] result = new string[6];
             try
             {
                 #region Try

                 #region Objects and Variables

                 string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                 var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                 Session["permission"] = permission;
                 if (permission == "False")
                 {
                     return Redirect("/Payroll/Home");
                 }

                 ReportDocument doc = new ReportDocument();
                 DataSet ds = new DataSet();
                 DataTable dt = new DataTable();

                 EmployeeLoanRepo _loanRepo = new EmployeeLoanRepo();
                 var getAllData = _loanRepo.SelectAll(Convert.ToInt32(identity.BranchId), "");

                 dt = Ordinary.ListToDataTable(getAllData.ToList());

                 #endregion

                 #region Report Call

                 EnumReportRepo _reportRepo = new EnumReportRepo();
                 List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

                 string[] conFields = { "ReportType", "ReportId" };
                 string[] conValues = { "LoanReport", "LoanReport2" };
                 enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);

                 string ReportFileName = enumReportVMs.FirstOrDefault().ReportFileName;
                 string ReportName = enumReportVMs.FirstOrDefault().Name;

                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                 string rptLocation = "";

                 rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Loan\" + ReportFileName + ".rpt";

                 doc.Load(rptLocation);

                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                 doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                 doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                 doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";


                 #endregion

                 dt.TableName = "dtLoanReport";

                 doc.SetDataSource(dt);

                 var rpt = RenderReportAsPDF(doc);
                 doc.Close();
                 return rpt;

                 #endregion Try
             }
             catch (Exception ex)
             {
                 result[0] = "Fail";
                 result[1] = "Process Fail";
                 Session["result"] = result[0] + "~" + result[1];
                 FileLogger.Log("LoanReport", this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);

                 return View();
             }
         }

         public ActionResult DownloadAllLoanReport(EmployeeLoanVM vm)
         {
             string[] result = new string[6];
             DataSet ds = new DataSet();
             DataTable dt = new DataTable();
             List<string> ProjectIdList = new List<string>();
             try
             {
                 #region Objects and Variables

                 ReportDocument doc = new ReportDocument();

                 string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                 var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "add").ToString();
                 Session["permission"] = permission;
                 if (permission == "False")
                 {
                     return Redirect("/Payroll/Home");
                 }

                 string FileName = "Download.xls";
                 string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                 //string fullPath = @"C:\";
                 if (System.IO.File.Exists(fullPath + FileName))
                 {
                     System.IO.File.Delete(fullPath + FileName);
                 }
                 #endregion

              
                 #region Pull Data

                 EmployeeLoanRepo _loanRepo = new EmployeeLoanRepo();
                 var getAllData = _loanRepo.SelectAll(Convert.ToInt32(identity.BranchId), "");

                 dt = Ordinary.ListToDataTable(getAllData.ToList());

                 var toRemove = new string[] {  "Operation","PFBalance","AvailableRate","Id","LoanType_E","EmployeeId","NumberOfInstallment"
                                                ,"PeriodName","ApplicationDate","ApprovedDate","IsApproved","EndDate","IsHold","Remarks"
                                                ,"IsActive","IsArchive","CreatedBy","CreatedAt","CreatedFrom","LastUpdateBy","LastUpdateAt","LastUpdateFrom","Project"
                                                ,"Section","employeeLoanDetails","InterestPolicy","IsFixed","InterestRate","InterestRate1","InterestAmount","PrincipalAmount"
                                                ,"BranchId","Employee","FiscalYearDetailId","RefundAmount","RefundDate","FromSetting"
                                            };

                 //List<string> oldColumnNames = new List<string> { "EmpName", "LoanTypeName", "PrincipalAmount", "InterestAmount", "InstallmentAmount" };
                 //List<string> newColumnNames = new List<string> { "Employee Name", "Type Name", "Principal Amount", "Interest Amount", "Installment Amount" };
                 //dt = Ordinary.DtColumnNameChangeList(dt, oldColumnNames, newColumnNames);

                 foreach (string col in toRemove)
                 {
                     dt.Columns.Remove(col);
                 }


                 #endregion

                 #region Validations

                 if (dt.Rows.Count == 0)
                 {
                     result[0] = "Fail";
                     result[1] = "No Data Found";
                     Session["result"] = result[0] + "~" + result[1];

                     return View();
                 }

                 #endregion

                 #region Report Call

                 string filename = "";


                 string[] conFields = { "ReportType", "ReportId" };
                 string[] conValues = { "LoanReport", "LoanReport1" };
                 List<EnumReportVM> enumReportVMs = new EnumReportRepo().SelectAll(0, conFields, conValues);

                 string ReportName = enumReportVMs.FirstOrDefault().Name;
                 filename = ReportName;

                 #endregion

                 #region Prepare Excel

                 ExcelPackage excel = new ExcelPackage();
                 var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

                 if (CompanyName.ToUpper() == "TIB" || CompanyName.ToUpper() == "G4S")
                 {
                     CompanyRepo cRepo = new CompanyRepo();
                     CompanyVM comInfo = cRepo.SelectById(1);
                     string Line1 = comInfo.Name; // "BRAC EPL STOCK BROKERAGE LIMITED";
                     string
                         Line2 = comInfo
                             .Address; // "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                     string Line3 = "";

                     string[] ReportHeaders = new string[] { Line1, Line2, Line3 };

                     ExcelSheetFormat(dt, workSheet, ReportHeaders);
                 }

                 #endregion

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

                 #region Redirect

                 result[0] = "Success";
                 result[1] = "Successful~Data Download";

                 Session["result"] = result[0] + "~" + result[1];
                 return Redirect("SalarySheet");

                 #endregion
             }
             catch (Exception e)
             {
                 Session["result"] = result[0] + "~" + result[1];
                 FileLogger.Log(
                     result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine +
                     result[5].ToString(), this.GetType().Name,
                     result[4].ToString() + Environment.NewLine + result[3].ToString());
                 return Redirect("SalarySheet");
             }
         }

         private void ExcelSheetFormat(DataTable dt, ExcelWorksheet workSheet, string[] ReportHeaders)
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
                 workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment =
                     ExcelHorizontalAlignment.Left;
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

                     workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" +
                                                                         workSheet.Cells[TableHeadRow + 1, colNumber]
                                                                             .Address + ":" +
                                                                         workSheet.Cells[(TableHeadRow + RowCount),
                                                                             colNumber].Address + ")";

                     #endregion
                 }
             }

             workSheet.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
             workSheet.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

             workSheet.Cells[
                     "A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)]
                 .Style
                 .Border.Top.Style = ExcelBorderStyle.Thin;
             workSheet.Cells[
                     "A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style
                 .Border.Left.Style = ExcelBorderStyle.Thin;
             workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

             #endregion
         }

    }
}
