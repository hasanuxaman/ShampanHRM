using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class DBUpdateController : Controller
    {
        //
        // GET: /Common/DBUpdate/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DBUpdate()
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_16", "process").ToString();
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                string[] result = new string[6];

                #region Settings
                SettingsVM vm = new SettingsVM();
                SettingRepo _repo = new SettingRepo();
                ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = Identity.Name;
                vm.CreatedFrom = Identity.WorkStationIP;
                vm.BranchId = Convert.ToInt32(Identity.BranchId);

                if (CompanyName == "brac")
                {
                    #region PF Module

                    result = _repo.settingsDataInsert(vm, "PF", "EntitleDate", "varchar(14)", "20180101");
                    result = _repo.settingsDataInsert(vm, "PFLoan", "AvailableRate", "decimal", "80");


                    #endregion
                }

                if (CompanyName == "brac")
                {
                    #region Tax Module
                    //////result = _repo.settingsDataInsert(vm, "Tax", "YearlyBonusNumber", "int", "2");

                    result = _repo.settingsDataInsert(vm, "Tax", "SalaryTaxPercent", "decimal", "80");
                    result = _repo.settingsDataInsert(vm, "Tax", "BonusTaxPercent", "decimal", "80");


                    #endregion
                }               
                result = _repo.settingsDataInsert(vm, "PFLoanRate", "FromSetting", "Boolean", "N");
                result = _repo.settingsDataInsert(vm, "PFLoanRate", "Upto12Month", "int", "5");
                result = _repo.settingsDataInsert(vm, "PFLoanRate", "GetterThen12Month", "int", "6");


                result = _repo.settingsDataInsert(vm, "GF", "GFStartFrom", "string", "20210701");
                result = _repo.settingsDataInsert(vm, "GF", "BreakMonthCalculate", "Boolean", "N");

                result = _repo.settingsDataInsert(vm, "Salary", "DebitA/CNo", "string", "N/A");
                result = _repo.settingsDataInsert(vm, "PFLoan", "BothContributionJobAge", "decimal", "2");

                result = _repo.settingsDataInsert(vm, "PF", "FromDOJ", "Boolean", "N");
                result = _repo.settingsDataInsert(vm, "Tax", "FromDOJ", "Boolean", "N");

                result = _repo.settingsDataInsert(vm, "Tax", "InvestmentDeductionFromTax", "Boolean", "N");
                result = _repo.settingsDataInsert(vm, "Salary", "ApproverEmail", "string", "abhishek.srivastava@bollore.com");


                #region HR-Payroll
                result = _repo.settingsDataInsert(vm, "HRM", "ELBalance", "decimal", "0");

                result = _repo.settingsDataInsert(vm, "HRM", "IsHolyDayLeaveSkip", "string", "N");

                result = _repo.settingsDataInsert(vm, "HRM", "IsESSEditPermission", "string", "N");


                result = _repo.settingsDataInsert(vm, "Encashment", "EncashmentRatio", "decimal", "50");

                result = _repo.settingsDataInsert(vm, "Tax", "TaxPercentByEmployee", "string", "N");


                result = _repo.settingsDataInsert(vm, "Deduction", "PunishmentFromBasic", "string", "Y");

                result = _repo.settingsDataInsert(vm, "Holiday", "FirstHoliday", "string", "Friday");
                result = _repo.settingsDataInsert(vm, "Holiday", "SecondHoliday", "string", "Friday");



                result = _repo.settingsDataInsert(vm, "Leave", "HolidayCheck", "string", "N");

                result = _repo.settingsDataInsert(vm, "Attendance", "AutoAttendanceMigration", "string", "Y");

                result = _repo.settingsDataInsert(vm, "Mail", "MailSubjectTC", "string", "Computer Generate Tax Certificate for the preiod of vmonth");
                result = _repo.settingsDataInsert(vm, "Mail", "MailBodyTC", "string", "Dear vname,Please find the attachment file of Tax Certificate for the preiod of vmonth. If you have any queries, please feel free to contact the Payroll and Reporting/ Payment Office. Kind Regards,Arifa Begum Deputy Coordinator - (Finance & Accounts)");


                result = _repo.settingsDataInsert(vm, "Attendance", "MovementEarlyOutAllowMin", "int", "60");
                result = _repo.settingsDataInsert(vm, "Attendance", "MovementLateInAllowMin", "int", "60");

                result = _repo.settingsDataInsert(vm, "Sage", "Currency", "string", "BDT");
                result = _repo.settingsDataInsert(vm, "Sage", "Username", "string", "-");
                result = _repo.settingsDataInsert(vm, "Sage", "Password", "string", "-");
                result = _repo.settingsDataInsert(vm, "Sage", "SourceType", "string", "JE");


                //result = _repo.settingsDataInsert(vm, "Attendance", "IsMonthlyLateInDeduct", "string", "Y");
                //result = _repo.settingsDataInsert(vm, "Attendance", "IsMonthlyLateInHourlyCount", "string", "N");
                //result = _repo.settingsDataInsert(vm, "Attendance", "MonthlyLateInCountDays", "string", "3");

                //result = _repo.settingsDataInsert(vm, "Attendance", "IsMonthlyEarlyOutDeduct", "string", "Y");
                //result = _repo.settingsDataInsert(vm, "Attendance", "IsMonthlyEarlyOutHourlyCount", "string", "Y");
                //result = _repo.settingsDataInsert(vm, "Attendance", "MonthlyEarlyOutCountDays", "string", "0");



                result = _repo.settingsDataInsert(vm, "OverTime", "DailyOTRoundUp", "string", "30");
                result = _repo.settingsDataInsert(vm, "OverTime", "MonthlyOTRoundUp", "string", "60");


                result = _repo.settingsDataInsert(vm, "Report", "rptEmployeeInfo", "string", "rptEmployeeInfo_Kajol");

                result = _repo.settingsDataInsert(vm, "Database", "HRMDB", "string", "KajolBrothersHRMDemo");
                result = _repo.settingsDataInsert(vm, "Database", "TAXDB", "string", "TAX_DB");
                result = _repo.settingsDataInsert(vm, "Database", "PFDB", "string", "PF_DB");
                result = _repo.settingsDataInsert(vm, "Database", "GFDB", "string", "GF_DB");


                result = _repo.settingsDataInsert(vm, "OverTime", "CountFrom", "string", "Gross");
                result = _repo.settingsDataInsert(vm, "OverTime", "CountFromDevided", "string", "270");


                result = _repo.settingsDataInsert(vm, "AutoUser", "Employee", "string", "Y");
                result = _repo.settingsDataInsert(vm, "AutoPassword", "Employee", "string", "123456");

                result = _repo.settingsDataInsert(vm, "EmployeeJob", "ProbationMonth", "int", "6");



                result = _repo.settingsDataInsert(vm, "EmployeeJob", "ProbationMonth", "int", "6");
                result = _repo.settingsDataInsert(vm, "Leave", "ParmanentCheck", "string", "Y");

                result = _repo.settingsDataInsert(vm, "Salary", "HouseRentCalc", "string", "vBasic*50/100");
                result = _repo.settingsDataInsert(vm, "Salary", "ConvenceCalc", "string", "vGross*8/100");
                result = _repo.settingsDataInsert(vm, "Salary", "MedicalCalc", "string", "vGross*2/100");
                result = _repo.settingsDataInsert(vm, "Salary", "SalaryFromMatrix", "Boolean", "N");
                result = _repo.settingsDataInsert(vm, "SalarySheet", "SalarySheet(1)", "string", "RptSalarySheetWithProject_Campe");
                result = _repo.settingsDataInsert(vm, "SalarySheet", "SalarySheet(2)", "string", "RptSalarySheet_Campe");
                result = _repo.settingsDataInsert(vm, "SalarySheet", "SalarySheet(3)", "string", "RptSalarySheetP1_Campe");
                result = _repo.settingsDataInsert(vm, "SalarySheet", "SalarySheet(4)", "string", "RptSalarySheetP2_Campe");
                result = _repo.settingsDataInsert(vm, "SalarySheet", "paySlip", "string", "RptPaySlipNew_Campe");
                result = _repo.settingsDataInsert(vm, "SalarySheet", "paySlip(email)", "string", "RptPaySlipNew_Campe");
                result = _repo.settingsDataInsert(vm, "AutoCode", "Employee", "Boolean", "Y");

                result = _repo.settingsDataInsert(vm, "Tax", "AmountofExemptedIncome", "Boolean", "Y");
                result = _repo.settingsDataInsert(vm, "Tax", "Divided", "Int", "3");
                result = _repo.settingsDataInsert(vm, "Tax", "Exempted", "Int", "450000");
                result = _repo.settingsDataInsert(vm, "Tax", "DividedBonus", "Int", "2");
                result = _repo.settingsDataInsert(vm, "Tax", "DividedMonth", "Int", "2");

                result = _repo.settingsDataInsert(vm, "PF", "FromPayroll", "Boolean", "Y");

                result = _repo.settingsDataInsert(vm, "GF", "DayWiseArear", "Boolean", "Y");
                result = _repo.settingsDataInsert(vm, "GF", "YearDay", "Int", "365");

           
                #endregion


                #endregion Settings

                #region DBUpdate

                DBUpdateRepo comprepo = new DBUpdateRepo();

                #region DBUpdate

                comprepo.HRPayroll_DBUpdate();


                //////comprepo.DatabaseTableChanges();


                #region HR-Payroll
                //////////PreEmployementInformation
                //////////EnumReport
                //////////OtherInfo

                //////////EnumColumnList
                //////////AttendanceDailyNew
                //////////EarningDeductionStructure
                //////////EmployeeDailyAbsence
                //////////EmployeeDailyOvertime
                //////////EmployeeMonthlyAbsence
                //////////EmployeeMonthlyOvertime
                //////////JobCircular
                //////////MonthlyAttendance
                //////////OTStructure
                //////////PublicApplicant
                #endregion

                #endregion Added Table

                #region ReomveField

                result = comprepo.DBTableFieldRemove("EmployeeAssets", "AssetName");
                result = comprepo.DBTableFieldRemove("AttendanceMigration", "OTTimeMax");
                result = comprepo.DBTableFieldRemove("AttendanceMigration", "TGraceTime");
                result = comprepo.DBTableFieldRemove("AttendanceMigration", "IfterGraceTime");
                result = comprepo.DBTableFieldRemove("AttendanceMigration", "DGraceTime");
                result = comprepo.DBTableFieldRemove("AttendanceMigration", "BonusOTTime");
                result = comprepo.DBTableFieldRemove("AttendanceMigration", "MaxOut");
                result = comprepo.DBTableFieldRemove("AttendanceMigration", "MaxOutNextD");

                result = comprepo.DBTableFieldRemove("AttendanceStructure", "OTTimeMax");
                result = comprepo.DBTableFieldRemove("AttendanceStructure", "TGraceTime");
                result = comprepo.DBTableFieldRemove("AttendanceStructure", "IfterGraceTime");
                result = comprepo.DBTableFieldRemove("AttendanceStructure", "DGraceTime");
                result = comprepo.DBTableFieldRemove("AttendanceStructure", "BonusOTTime");
                result = comprepo.DBTableFieldRemove("AttendanceStructure", "MaxOut");
                result = comprepo.DBTableFieldRemove("AttendanceStructure", "MaxOutNextD");
                #endregion ReomveField
                 
                #region AddField
                ////Type: bit - int - decimal(18, 2) - nvarchar(50) ////true = allow null
                ////////result = comprepo.DBTableFieldAdd("MonthlyAttendance", "LateInAbsentDays", "decimal(18, 2)", true);

                if (CompanyName == "brac")
                {
                    #region Tax Module

                    #region Field Add
                   
                    
                    comprepo.DBTAXTableFieldAdd("Schedule1SalaryMonthlies", "FinalTaxAmountMonthly", "decimal(18, 2)", true);
                    comprepo.DBTAXTableFieldAdd("Schedule1SalaryMonthlies", "InvestmentLimit", "decimal(18, 5)", true);
                    comprepo.DBTAXTableFieldAdd("Schedule1SalaryMonthlies", "TaxPaymentYearly", "decimal(18, 5)", true);
                    comprepo.DBTAXTableFieldAdd("Schedule1SalaryMonthlies", "RebateAmount", "decimal(18, 5)", true);
                    comprepo.DBTAXTableFieldAdd("Schedule1SalaryMonthlies", "RebateAmountMonthly", "decimal(18, 5)", true);


                    comprepo.DBTAXTableFieldAdd("EmployeeSchedule3TaxSlabDetailsMonthlies", "FiscalYearDetailIdTo", "int", true);
                    comprepo.DBTAXTableFieldAdd("EmployeeTaxSlabDetailsMonthlies", "FiscalYearDetailIdTo", "int", true);
                    comprepo.DBTAXTableFieldAdd("Schedule3InvestmentMonthlies", "FiscalYearDetailIdTo", "int", true);
                    comprepo.DBTAXTableFieldAdd("Schedule1SalaryMonthlies", "FiscalYearDetailIdTo", "int", true);

                    comprepo.DBTAXTableFieldAdd("Schedule1SalaryMonthlies", "TaxName", "nvarchar(200)", true);

                    comprepo.DBTAXTableFieldAdd("Schedule1SalaryMonthlies", "TransactionType", "nvarchar(50)", true);
                    comprepo.DBTAXTableFieldAdd("EmployeeTaxSlabDetailsMonthlies", "TransactionType", "nvarchar(50)", true);
                    comprepo.DBTAXTableFieldAdd("Schedule3InvestmentMonthlies", "TransactionType", "nvarchar(50)", true);
                    comprepo.DBTAXTableFieldAdd("EmployeeSchedule3TaxSlabDetailsMonthlies", "TransactionType", "nvarchar(50)", true);

                    comprepo.DBTAXTableFieldAdd("TaxDeposits", "Particular", "nvarchar(50)", true);


                    #endregion

                    #endregion
                }

              
                #region PF Module

                comprepo.PF_DBUpdate();

                #endregion

                #region HR-Payroll
                //////Moved to DAL
                result = comprepo.DBTableFieldAdd("EmployeeJob", "IsCarTAXApplicable", "bit", true);

                result = comprepo.DBTableFieldAdd("Designation", "DesignationGroupId", "nvarchar(20)", true);
                result = comprepo.DBTableFieldAdd("SalaryEmployee", "StepId", "nvarchar(20)", true);
                
                result = comprepo.DBTableFieldAdd("SalaryOtherEarning", "taxheadid", "int", true);

                result = comprepo.DBTableFieldAdd("SalaryEmployee", "StructureBasic", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("SalaryEmployee", "StructureHouseRent", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("SalaryEmployee", "StructureMedical", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("SalaryEmployee", "StructureTA", "decimal(18, 5)", true);

                result = comprepo.DBTableFieldAdd("EmployeeStructureGroup", "IsGFApplicable", "bit", true);
                result = comprepo.DBTableFieldAdd("EmployeeSalaryStructure", "IsArrear", "bit", true);
                result = comprepo.DBTableFieldAdd("EmployeeSalaryStructure", "ArrearFiscalYearDetailId", "int", true);

                result = comprepo.DBTableFieldAdd("Grade", "IsHouseRentFactorFromBasic", "bit", true);

                result = comprepo.DBTableFieldAdd("Grade", "IsFixedHouseRent", "bit", true);
                result = comprepo.DBTableFieldAdd("Grade", "HouseRentAllowance", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("Grade", "IsFixedSpecialAllowance", "bit", true);
                result = comprepo.DBTableFieldAdd("Grade", "SpecialAllowance", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("Grade", "LowerLimit", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("Grade", "MedianLimit", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("Grade", "UpperLimit", "decimal(18, 5)", true);


                result = comprepo.DBTableFieldAdd("Grade", "IsTAFactorFromBasic", "bit", true);
                result = comprepo.DBTableFieldAdd("Grade", "TAFactor", "bit", true);
                result = comprepo.DBTableFieldAdd("Grade", "IsMedicalFactorFromBasic", "bit", true);
                result = comprepo.DBTableFieldAdd("Grade", "Area", "nvarchar(20)", true);
                result = comprepo.DBTableFieldAdd("Grade", "GradeNo", "int", true);
                result = comprepo.DBTableFieldAdd("Grade", "CurrentBasic", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("Grade", "BasicNextYearFactor", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("Grade", "BasicNextStepFactor", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("Grade", "HouseRentFactor", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("Grade", "TAFactor", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("Grade", "MedicalFactor", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("Grade", "TAFactor", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("Grade", "TAFactor", "decimal(18, 5)", true);
                result = comprepo.DBTableFieldAdd("EmployeeOtherEarning", "SalaryMonthId", "nvarchar(50)", true);
                result = comprepo.DBTableFieldAdd("BonusName", "FestivalDate", "varchar(100)", true);
                result = comprepo.DBTableFieldAdd("BonusName", "IssueDate", "varchar(100)", true);

                result = comprepo.DBTableFieldAdd("EmployeeJob", "GFStartFrom", "nvarchar(20)", true);
                result = comprepo.DBTableFieldAdd("EmployeeJob", "IsRebate", "bit", true);

                result = comprepo.PF_DBTableFieldAdd("GFEmployeeProvisions", "IncrementArrear", "decimal(18, 5)", true);

                result = comprepo.PF_DBTableFieldAdd("GFEmployeeProvisions", "FiscalYearDetailStartDate", "nvarchar(20)", true);
                result = comprepo.PF_DBTableFieldAdd("GFEmployeeProvisions", "IsBreakMonth", "bit", true);
                result = comprepo.PF_DBTableFieldAdd("EmployeePFPayment", "FiscalYearDetailId", "int", true);
                result = comprepo.PF_DBTableFieldAdd("GFEmployeePayment", "FiscalYearDetailId", "int", true);

                result = comprepo.PF_DBTableFieldAdd("InvestmentRenew", "SourceTaxDeduct", "decimal(18, 2)", true);
                result = comprepo.PF_DBTableFieldAdd("InvestmentRenew", "OtherCharge", "decimal(18, 2)", true);
            

                #region Comments / Jan-12-2020
            
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "FirstHoliday", "nvarchar(10)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "SecondHoliday", "nvarchar(10)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeMonthlyAbsence", "IsManual", "bit", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeMonthlyOvertime", "IsManual", "bit", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeDailyAbsence", "IsManual", "bit", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeDailyOvertime", "IsManual", "bit", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "Other1Id", "int", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "Other2Id", "int", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "Other3Id", "int", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "Other4Id", "int", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "Other5Id", "int", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "ExtraOT", "bit", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "OTBayer", "bit", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "OTOrginal", "bit", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "OTAlloawance", "bit", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "AttendenceBonus", "decimal(18, 2)", true);
                //////result = comprepo.DBTableFieldAdd("Designation", "PriorityLevel", "int", true);


                //////result = comprepo.DBTableFieldAdd("UserGroup", "IsAdmin", "bit", true);
                //////result = comprepo.DBTableFieldAdd("UserGroup", "IsHRM", "bit", false);
                //////result = comprepo.DBTableFieldAdd("UserGroup", "IsAttendance", "bit", false);
                //////result = comprepo.DBTableFieldAdd("UserGroup", "IsPayroll", "bit", false);
                //////result = comprepo.DBTableFieldAdd("UserGroup", "IsTAX", "bit", false);
                //////result = comprepo.DBTableFieldAdd("UserGroup", "IsPF", "bit", false);
                //////result = comprepo.DBTableFieldAdd("UserGroup", "IsGF", "bit", false);



                //////result = comprepo.DBTableFieldAdd("EmployeeSalaryStructureDetail", "IncrementDate", "nvarchar(14)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeSalaryStructureDetail", "IsCurrent", "bit", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeSalaryStructure", "IsCurrent", "bit", false);

                //////result = comprepo.DBTableFieldAdd("SalaryBonusDetail", "FiscalYear", "int", true);
                //////result = comprepo.DBTableFieldAdd("SalaryBonusDetail", "FiscalYearDetailId", "int", true);


                //////result = comprepo.DBTableFieldAdd("EmployeeTravel", "EmbassyName", "nvarchar(100)", true);

                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "IsJobBefore", "bit", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "AccountType", "nvarchar(50)", true);

                //////result = comprepo.DBTableFieldAdd("EmployeePersonalDetail", "PlaceOfBirth", "nvarchar(100)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeePersonalDetail", "MarriageDate", "nvarchar(14)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeePersonalDetail", "SpouseProfession", "nvarchar(100)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeePersonalDetail", "SpouseDateOfBirth", "nvarchar(14)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeePersonalDetail", "SpouseBloodGroup", "nvarchar(20)", true);

                //////result = comprepo.DBTableFieldAdd("EmployeeDependent", "Gender", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeDependent", "EducationQualification", "nvarchar(500)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeDependent", "BloodGroup", "nvarchar(20)", true);

                //////result = comprepo.DBTableFieldAdd("EmployeeEmergencyContact", "Email", "nvarchar(50)", true);


                //////result = comprepo.DBTableFieldAdd("EmployeeReference", "Designation", "nvarchar(100)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeReference", "WorkAddress", "nvarchar(500)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeReference", "YearOfAcquaintance", "int", true);

                //////result = comprepo.DBTableFieldAdd("EmployeeJobHistory", "ReasonForLeaving", "nvarchar(100)", true);



                //////result = comprepo.DBTableFieldAdd("EmployeeTravel", "IssueDate", "nvarchar(14)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeTravel", "ExpiryDate", "nvarchar(14)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeTravel", "Country", "nvarchar(200)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeTravel", "PassportNumber", "nvarchar(50)", true);



                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "DayRateCountFrom", "nvarchar(50)", false);
                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "HourRateCountFrom", "nvarchar(50)", false);
                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "DayRateDivisionFactor", "int", false);
                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "HourRateDivisionFactor", "int", false);



                //////result = comprepo.DBTableFieldAdd("AttendanceDailyNew", "MovementEarlyOutMin", "decimal(18, 2)", true);
                //////result = comprepo.DBTableFieldAdd("AttendanceDailyNew", "MovementLateInMin", "decimal(18, 2)", true);


                //////result = comprepo.DBTableFieldAdd("EmployeeDailyAbsence", "DayStatus", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeDailyOvertime", "DayStatus", "nvarchar(20)", true);


                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "IsMonthlyLateInDeduct", "bit", false);
                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "IsMonthlyLateInHourlyCount", "bit", false);
                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "MonthlyLateInCountDays	", "int", false);

                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "LateInAbsentDays", "int", false);
                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "IsMonthlyEarlyOutDeduct", "bit", false);
                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "IsMonthlyEarlyOutHourlyCount", "bit", false);
                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "MonthlyEarlyOutCountDays", "int", false);
                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "EarlyOutAbsentDays", "int", false);







                //////result = comprepo.DBTableFieldAdd("EmployeeDailyOvertime", "LateInMins", "decimal(18, 2)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeDailyOvertime", "EarlyOutMins", "decimal(18, 2)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeMonthlyOvertime", "TotalLateInMins", "decimal(18, 2)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeMonthlyOvertime", "TotalEarlyOutMins", "decimal(18, 2)", false);



                //////result = comprepo.DBTableFieldAdd("MonthlyAttendance", "LateAbsentHour", "decimal(18, 2)", true);

                //////result = comprepo.DBTableFieldAdd("MonthlyAttendance", "EarlyOutDayCount", "decimal(18, 2)", true);
                //////result = comprepo.DBTableFieldAdd("MonthlyAttendance", "EarlyOutHourCount", "decimal(18, 2)", true);
                //////result = comprepo.DBTableFieldAdd("MonthlyAttendance", "EarlyOutDeductAmount", "decimal(18, 2)", true);

                //////result = comprepo.DBTableFieldAdd("MonthlyAttendance", "LateInDayCount", "decimal(18, 2)", true);
                //////result = comprepo.DBTableFieldAdd("MonthlyAttendance", "LateInHourCount", "decimal(18, 2)", true);
                //////result = comprepo.DBTableFieldAdd("MonthlyAttendance", "LateInDeductAmount", "decimal(18, 2)", true);

                //////result = comprepo.DBTableFieldAdd("EmployeeMonthlyOvertime", "TotalOvertimeActual", "decimal(18, 2)", true);
                //////result = comprepo.DBTableFieldAdd("AttendanceDailyNew", "TotalOTActual", "decimal(18, 2)", true);

                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "BankPayAmount", "decimal(18, 2)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "Other1", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "Other2", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "Other3", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "Other4", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "Other5", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeDailyOvertime", "LateInHrs", "decimal(18, 2)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeDailyOvertime", "EarlyOutHrs", "decimal(18, 2)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeMonthlyOvertime", "TotalLateInHrs", "decimal(18, 2)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeMonthlyOvertime", "TotalEarlyOutHrs", "decimal(18, 2)", false);
                //////result = comprepo.DBTableFieldAdd("MonthlyAttendance", "EmploymentType", "nvarchar(20)", false);
                //////result = comprepo.DBTableFieldAdd("MonthlyAttendance", "LateAbsentDay", "nvarchar(20)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "ProjectId", "nvarchar(20)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "DepartmentId", "nvarchar(20)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "SectionId", "nvarchar(20)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "DesignationId", "nvarchar(20)", false);
                ////////ProjectId, DepartmentId , SectionId, DesignationId,

                //////result = comprepo.DBTableFieldAdd("EnumCountry", "IsContact", "bit", true);

                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "ProbationMonth", "int", true);

                //////result = comprepo.DBTableFieldAdd("EmployeeAssets", "AssetId", "int", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeMonthlyOvertime", "OTRate", "decimal(18, 2)", true);

                //////result = comprepo.DBTableFieldAdd("EmployeeLeave", "IsLWP", "bit", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeLeaveDetail", "IsLWP", "bit", true);


                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "WeeklyOTRate", "decimal(18,2)", true);
                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "GovtOTRate", "decimal(18,2)", true);
                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "FestivalOTRate", "decimal(18,2)", true);
                //////result = comprepo.DBTableFieldAdd("EarningDeductionStructure", "SpecialOTRate", "decimal(18,2)", true);


                //////result = comprepo.DBTableFieldAdd("EmployeeStructureGroup", "EarningDeductionStructureId", "int", true);

                //////result = comprepo.DBTableFieldAdd("AttendanceMigration", "PWorkingMin", "int", true);
                //////result = comprepo.DBTableFieldAdd("AttendanceMigration", "PTiffinMin", "int", true);
                //////result = comprepo.DBTableFieldAdd("AttendanceMigration", "PDinnerMin", "int", true);
                //////result = comprepo.DBTableFieldAdd("AttendanceMigration", "PIfterMin", "int", true);

                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "WorkingMin", "int", true);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "TiffinMin", "int", true);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "DinnerMin", "int", true);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "IfterMin", "int", true);

                //////result = comprepo.DBTableFieldAdd("EmployeeMonthlyAbsence", "LateInDays", "decimal(18,2)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeMonthlyAbsence", "EarlyOutDays", "decimal(18,2)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeDailyAbsence", "TransactionType", "nvarchar(50)", true);
                //////result = comprepo.DBTableFieldAdd("DownloadData", "ProxyID1", "nvarchar(50)", true);


                //////result = comprepo.DBTableFieldAdd("EmployeeStructureGroup", "IsNew", "bit", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeStructureGroup", "LeaveYear", "int", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeStructureGroup", "SalaryInput", "decimal(18,2)", true);

                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "BankInfo", "nvarchar(20)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "BankAccountNo", "nvarchar(50)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "GrossSalary", "decimal(18,2)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "BasicSalary", "decimal(18,2)", false);
                //////result = comprepo.DBTableFieldAdd("Grade", "MinSalary", "decimal(18,2)", false);
                //////result = comprepo.DBTableFieldAdd("Grade", "MaxSalary", "decimal(18,2)", false);
                //////result = comprepo.DBTableFieldAdd("LeaveStructureDetail", "IsCarryForward", "bit", false);
                //////result = comprepo.DBTableFieldAdd("LeaveStructureDetail", "MaxBalance", "int", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeLeaveStructure", "IsCarryForward", "bit", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeLeaveStructure", "MaxBalance", "int", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "IsPermanent", "bit", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeJob", "StructureGroupId", "nvarchar(20)", false);
                //////result = comprepo.DBTableFieldRemove("EmployeeJob", "StructureGroupId");
                //////result = comprepo.DBTableFieldAdd("EmployeePersonalDetail", "BloodGroup_E", "nvarchar(20)", true);
                ////////result = comprepo.DBTableAdd("EmployeeStructureGroup", "BloodGroup_E", "nvarchar(20)");
                //////result = comprepo.DBTableFieldAdd("EmployeeLeave", "ApproveDate", "nvarchar(14)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeLeaveDetail", "ApproveDate", "nvarchar(14)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeLeaveDetail", "FiscalYearDetailId", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeStructureGroup", "GroupId", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeStructureGroup", "LeaveStructureId", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeStructureGroup", "SalaryStructureId", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeStructureGroup", "PFStructureId", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeStructureGroup", "TaxStructureId", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeStructureGroup", "BonusStructureId", "nvarchar(20)", true);
                //////result = comprepo.DBTableFieldAdd("EmployeeLoan", "RefundAmount", "decimal(18,2)", false);
                //////result = comprepo.DBTableFieldAdd("EmployeeLoan", "RefundDate", "nvarchar(14)", true);
                //////result = comprepo.DBTableFieldAdd("JournalLedger", "IsPushInSage", "bit", false);
                //////result = comprepo.DBTableFieldAdd("JournalLedgerDetail", "IsPushInSage", "bit", false);
                //////result = comprepo.DBTableFieldAdd("GLAccount", "ProjectId", "nvarchar(20)", false);
                //////result = comprepo.DBTableFieldAdd("GLAccount", "VoucherType", "nvarchar(20)", false);
                //////result = comprepo.DBTableFieldAdd("Company", "VATNo", "nvarchar(20)", false);
                //////result = comprepo.DBTableFieldAdd("Branchs", "VATNo", "nvarchar(20)", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "IsInOT", "bit", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "FirstSlotAbsentFromGross", "bit", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "FirstSlotAbsentDays", "int", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "SecondSlotAbsentFromGross", "bit", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "SecondSlotAbsentDays", "int", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "NPAbsentFromGross", "bit", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "LateInAllowDays", "int", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "SecondSlotAbsentDays", "int", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "LateInAbsentDays", "int", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "EarlyOutAllowDays", "int", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "EarlyOutAbsentDays", "int", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceStructure", "OutGrace", "int", false);
                //////result = comprepo.DBTableFieldAdd("AttendanceMigration", "POutGrace", "int", false);

                #endregion


                #endregion

                #endregion AddField

                #region FieldAlter
                result = comprepo.DBTableFieldAlter("MonthlyAttendance", "LWPDay", "decimal(18, 2)");

                result = comprepo.DBTableFieldAlter("MonthlyAttendance", "LateAbsentDay", "decimal(18, 2)");
                result = comprepo.DBTableFieldAlter("MonthlyAttendance", "AbsentDay", "decimal(18, 2)");
                #endregion FieldAlter

                #endregion DBUpdate

                Session["result"] = result[0] + "~" + result[1];
                return Redirect("/Common/Home/");
            }
            catch (Exception e)
            {
                return Redirect("/Common/Home/");
            }
        }
    }
}
