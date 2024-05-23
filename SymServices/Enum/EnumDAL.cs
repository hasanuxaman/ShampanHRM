using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Enum
{
    public class EnumDAL
    {
        #region PF - Module

        static string[] PFWithdrawTypes = new string[] {"Other" , "Investment", "PFSettlement", "PettyCashExpense", "ProfitDistribution", "ReserveFund"};


        public IEnumerable<object> GetPFWithdrawTypeList()
        {
            Array.Sort(PFWithdrawTypes, StringComparer.InvariantCulture);
            IEnumerable<object> enumList = from e in PFWithdrawTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        #endregion

        #region GL - GDIC - Module

        static string[] GLAuditStatus = { "Audited", "Not Audited" };

        public IEnumerable<object> GLAuditStatusList()
        {
            IEnumerable<object> enumList = from e in GLAuditStatus
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] GLDateTypes = { "Requisition Date", "Receive Date" };

        public IEnumerable<object> GLDateTypeList()
        {
            IEnumerable<object> enumList = from e in GLDateTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] GLYesNo = { "Yes", "No" };

        public IEnumerable<object> GLYesNoList()
        {
            IEnumerable<object> enumList = from e in GLYesNo
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }





        static string[] GLStatus = { "Created", "Posted", "Decline", "Rejected", "Approval Completed" };

        public IEnumerable<object> GLStatusList()
        {
            IEnumerable<object> enumList = from e in GLStatus
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] GLDocumentTypes = { "MTR", "MAR", "FIR", "MISC", "MAH", "ENG", "AVN" };

        public IEnumerable<object> GLDocumentTypeList()
        {
            IEnumerable<object> enumList = from e in GLDocumentTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] GLAccountNature = new string[] { "Dr", "Cr" };

        public IEnumerable<object> GLGetAccountNatureList()
        {
            IEnumerable<object> enumList = from e in GLAccountNature
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        #endregion

        static string[] DeductionState = new string[] { "Absent", "Late In", "Early Out" };
        public IEnumerable<object> GetDeductionStateList()
        {
            IEnumerable<object> enumList = from e in DeductionState
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        #region Acc
        //Acc  
        static string[] PostStatus = new string[] { "Posted", "Not Posted" };

        public IEnumerable<object> GetPostStatusList()
        {
            IEnumerable<object> enumList = from e in PostStatus
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] AccountNature = new string[] { "Dr", "Cr" };

        public IEnumerable<object> GetAccountNatureList()
        {
            IEnumerable<object> enumList = from e in AccountNature
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }
        static string[] ReportType = new string[] { "TrialBalance", "BalanceSheet", "IncomeStatement" };

        public IEnumerable<object> GetReportTypeList()
        {
            IEnumerable<object> enumList = from e in ReportType
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }
        static string[] AccountType = new string[] { "Bank", "Cash", "Other" };

        public IEnumerable<object> GetAccountTypeList()
        {
            IEnumerable<object> enumList = from e in AccountType
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] AreaNameTypes = new string[] { "Accounts", "Factory", "Production", "SalePoint" };

        public IEnumerable<object> GetAreaNameList()
        {
            IEnumerable<object> enumList = from e in AreaNameTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        //EnumTransactionType

        static string[] TransactionTypeList = new string[] { "Journal", "Payment", "Collection", "BankDeposit", "Withdraw", "FundTransfer" };

        public IEnumerable<object> GetTransactionTypeList()
        {
            IEnumerable<object> enumList = from e in TransactionTypeList
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] SaleCollectionGroupByTypes = new string[] { "Code", "Date", "Customer", "Pyament Type" };

        public IEnumerable<object> GetSaleCollectionGroupByList()
        {
            IEnumerable<object> enumList = from e in SaleCollectionGroupByTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] NonStockTypes = new string[] { "NonStock", "Stock" };

        public IEnumerable<object> GetNonStockTypeList()
        {
            IEnumerable<object> enumList = from e in NonStockTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] GroupTypes = new string[] { "Local", "Export" };
        public IEnumerable<object> GetGroupTypeList()
        {
            IEnumerable<object> enumList = from e in GroupTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] ProductTypes = new string[] { "Finish", "Non Stock", "Overhead", "Pack", "Raw", "Service", "Trading", "WIP" };
        public IEnumerable<object> GetProductTypeList()
        {
            IEnumerable<object> enumList = from e in ProductTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        #endregion Acc

        #region HRM - Payroll

        static string[] ReportTypes = new string[] { "PDF", "EXCEL" ,"WORD"};

        public IEnumerable<object> ReportTypesList()
        {
            IEnumerable<object> enumList = from e in ReportTypes
             select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }
        static string[] BonusStatus = new string[] { "Bonus", "Zero Bonus" };

        public IEnumerable<object> BonusStatusList()
        {
            IEnumerable<object> enumList = from e in BonusStatus
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] HoldStatus = new string[] { "Hold", "Not Hold" };

        public IEnumerable<object> HoldStatusList()
        {
            IEnumerable<object> enumList = from e in HoldStatus
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] DaysOfWeek = new string[] { "Friday", "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" };
        public IEnumerable<object> DaysOfWeekList()
        {

            IEnumerable<object> enumList = from e in DaysOfWeek
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }
        static string[] Force = new string[] { "Armed Forces", "Navy", "Air Force", "Border Guards Bangladesh", " Bangladesh Coast Guard", "Police", "RAB" };
        public IEnumerable<object> ForceList()
        {

            IEnumerable<object> enumList = from e in Force
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }
        static string[] Retirement = new string[] { "YES", "NO" };
        public IEnumerable<object> RetirementList()
        {

            IEnumerable<object> enumList = from e in Retirement
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }


        static string[] Import = new string[] { "EmployeeInfo", "EmployeePersonalDetail", "EmployeeEmergencyContact","EmployeeJob","Department","Designation","DesignationGroup","Bank","Branch","Asset","EmployeeEducation","EmployeeProfessionalDegree","EmployeeLanguage",
            "EmployeeExtraCurriculumActivities","EmployeeImmigration","EmployeeTraining","EmployeeTravel","EmployeeNominee","EmployeeDependent","EmployeeLeftInformation","EmployeeLeaveStructure","EmployeePF" };
        public IEnumerable<object> ImportList()
        {
            IEnumerable<object> enumList = from e in Import
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] EMPType = new string[] { "Direct", "Non-Management", "Management" };
        public IEnumerable<object> EMPTypeList()
        {

            IEnumerable<object> enumList = from e in EMPType
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }

        static string[] EmpCategory = new string[] { "Direct", "Indirect" };
        public IEnumerable<object> EmpCategoryList()
        {

            IEnumerable<object> enumList = from e in EmpCategory
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }
        static string[] NoChildren = new string[] { "1", "2", "3", "4", "5", "6","7","8","9","10"};
        public IEnumerable<object> NoChildrenList()
        {

            IEnumerable<object> enumList = from e in NoChildren
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }
        static string[] HeightInch = new string[] { "0","1", "2", "3", "4", "5", "6", "7", "8", "9", "10","11" };
        public IEnumerable<object> HeightInchList()
        {

            IEnumerable<object> enumList = from e in HeightInch
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }
        static string[] Feet = new string[] { "4", "5", "6", "7", "8", "9", "10", "11"};
        public IEnumerable<object> FeetList()
        {

            IEnumerable<object> enumList = from e in Feet
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }
        static string[] EmployeeStatus = new string[] { "Active", "InActive", "All" };

        public IEnumerable<object> GetEmployeeStatus()
        {
            IEnumerable<object> enumList = from e in EmployeeStatus
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] StructureTypes = new string[] { "Employee Group", "Leave Structure", "Salary Structure", "PF Structure"
            , "Tax Structure", "Bonus Structure","Leave Posting"};

        public IEnumerable<object> GetStructureTypeList()
        {
            IEnumerable<object> enumList = from e in StructureTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] absentDeductFrom = new string[] { "Gross", "Basic" };
        public IEnumerable<object> AbsentDeductFromList()
        {

            IEnumerable<object> enumList = from e in absentDeductFrom
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }
        static string[] daysCount = new string[] { "DOM", "30" };

        public IEnumerable<object> DaysCountList()
        {

            IEnumerable<object> enumList = from e in daysCount
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }



        //static string[] salaryHeadType = new string[] { "Gross", "Basic" };
        static string[] salaryHeadType = new string[] { "Basic" };
        public static IList<string> SalaryHeadType
        {
            get
            {
                return salaryHeadType.ToList<string>();
            }
        }

        static string[] salarySheetName = new string[] { "Salary Sheet(1)", "Salary Sheet(2)", "Salary Sheet(3)", "Salary Sheet(4)", "Pay Slip", "Pay Slip (Email)" };
        public static IList<string> SalarySheetName
        {
            get
            {
                return salarySheetName.ToList<string>();
            }
        }
        public IEnumerable<object> GetSalarySheetNameList()
        {
            IEnumerable<object> enumList = from e in SalarySheetName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] LetterName = new string[] { "Appointment Letter", "Transfer Letter", "Promotion Letter", "Increment Letter" };
        public IEnumerable<object> GetLetterNameList()
        {
            IEnumerable<object> enumList = from e in LetterName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }



        static string[] holiDayTypeName = new string[] { "Govt", "Festival", "Special" };
        public IEnumerable<object> GetHoliDayTypeNameList()
        {
            IEnumerable<object> enumList = from e in holiDayTypeName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] attnStatusName = new string[] { "All", "Present", "Absent", "Late", "Late and Absent", "All Missing", "In Miss", "Out Miss" };
        public IEnumerable<object> GetAttnStatusNameList()
        {
            IEnumerable<object> enumList = from e in attnStatusName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] loanInterestPolicy = new string[] { "Fixed", "Rate", "Reduce" };
        public static IList<string> LoanInterestPolicy
        {
            get
            {
                return loanInterestPolicy.ToList<string>();
            }
        }
        public IEnumerable<object> GetLoanInterestPolicyList()
        {
            IEnumerable<object> enumList = from e in LoanInterestPolicy
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] salaryProcessName = new string[] { "ALL", "SALARY EARNING", "PF", "TAX", "LOAN", "OTHER EARNING", "OTHER DEDUCTION"
            , "ATTENDANCE", "EMPLOYEE STATUS", "EMPLOYEE DEPENDENT", "FIXED OT" , "SALARY ARREAR", "Send Salary to Sage"};
        public static IList<string> SalaryProcessName
        {
            get
            {
                return salaryProcessName.ToList<string>();
            }
        }
        public IEnumerable<object> GetSalaryProcessNameList()
        {
            IEnumerable<object> enumList = from e in SalaryProcessName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        public IEnumerable<object> GetSalaryHeadTypeList()
        {
            IEnumerable<object> enumList = from e in SalaryHeadType
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] earningRptParamName = new string[] { "Fiscal Period", "Employee Name", "Earning Type" };

        public IEnumerable<object> GetEarningRptParamNameList()
        {
            IEnumerable<object> enumList = from e in earningRptParamName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] deductionRptParamName = new string[] { "Fiscal Period", "Employee Name", "Deduction Type" };

        public IEnumerable<object> GetDeductionRptParamNameList()
        {
            IEnumerable<object> enumList = from e in deductionRptParamName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] pfTaxRptParamName = new string[] { "Fiscal Period", "Employee Name" };

        public IEnumerable<object> GetPFTaxRptParamNameList()
        {
            IEnumerable<object> enumList = from e in pfTaxRptParamName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        #endregion


    }

}
