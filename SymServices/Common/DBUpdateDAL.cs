using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SymServices.Common
{
    public class DBUpdateDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        #region DB Migrate

        public string[] HRPayroll_DBUpdate(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction

                #region Table Add

                retResults = DatabaseTableChanges(currConn, transaction);

                #endregion

                #region 2023/17/7
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "PhoneNumber", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("SalaryStructureMatrix", "CurrentYearPart", "nvarchar(140)", true, currConn, transaction);
                #endregion

                #region Field Add FROM NOV 2022

                retResults = DBTableFieldAdd("EmployeeNominee", "VaccineFile3", "nvarchar(150)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeeNominee", "VaccineFiles2", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeNominee", "VaccineFile1", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeePersonalDetail", "VaccineFile3", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "VaccineFile2", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "VaccineFiles2", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "VaccineFile1", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeePersonalDetail", "Fingerprint", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "FingerprintFile", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "DotedLineReport", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "ContrExDate", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "Extentionyn", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "secondExDate", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "fristExDate", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "RetirementDate", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "TINFile", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "IsInactive", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "IsBuild", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "EmpJobType", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "EmpCategory", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeeTransfer", "Other1", "nvarchar(200)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeTransfer", "Other2", "nvarchar(200)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeTransfer", "Other3", "nvarchar(200)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeTransfer", "Other4", "nvarchar(200)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeTransfer", "Other5", "nvarchar(200)", true, currConn, transaction);


                retResults = DBTableFieldAdd("EmployeePersonalDetail", "HRMSCode", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "WDCode", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "TPNCode", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "PersonalEmail", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "IsVaccineDose1Complete", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "VaccineDose1Date", "nvarchar(14)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "VaccineDose1Name", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "IsVaccineDose2Complete", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "VaccineDose2Date", "nvarchar(14)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "VaccineDose2Name", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "IsVaccineDose3Complete", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "VaccineDose3Date", "nvarchar(14)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "VaccineDose3Name", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeePersonalDetail", "NoChildren", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "Heightft", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "HeightIn", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "Weight", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeePersonalDetail", "ChestIn", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeePermanentAddress", "PostOffice", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeeReference", "PostOffice", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeeEmergencyContact", "PostOffice", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeeJob", "ExtendedProbationMonth", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeeJob", "IsPFApplicable", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "IsGFApplicable", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "IsInactive", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "FromDate", "nvarchar(14)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "ToDate", "nvarchar(14)", true, currConn, transaction);


                retResults = DBTableFieldAdd("EmployeeNominee", "NID", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeNominee", "IsVaccineDose1Complete", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeNominee", "PostOffice", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeeNominee", "VaccineDose1Date", "nvarchar(14)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeNominee", "VaccineDose1Name", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeNominee", "IsVaccineDose2Complete", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeNominee", "VaccineDose2Date", "nvarchar(14)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeNominee", "VaccineDose2Name", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeNominee", "IsVaccineDose3Complete", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeNominee", "VaccineDose3Date", "nvarchar(14)", true, currConn, transaction);
                 retResults = DBTableFieldAdd("EmployeeNominee", "VaccineDose3Name", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeeDependent", "PostOffice", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeDependent", "IsVaccineDose1Complete", "bit", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeeDependent", "VaccineDose1Date", "nvarchar(14)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeDependent", "VaccineDose1Name", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeDependent", "IsVaccineDose2Complete", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeDependent", "VaccineDose2Date", "nvarchar(14)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeDependent", "VaccineDose2Name", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeDependent", "IsVaccineDose3Complete", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeDependent", "VaccineDose3Date", "nvarchar(14)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeDependent", "VaccineDose3Name", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("EmployeeDependent", "NID", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("Designation", "GradeId", "nvarchar(50)", true);

                retResults = DBTableFieldAdd("Designation", "HospitalPlanC1", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "HospitalPlanC2", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "HospitalPlanC3", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "HospitalPlanC4", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "HospitalPlanC5", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "DeathCoveragePlanC6", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "MaternityPlanC7", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "MaternityPlanC8", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "MaternityPlanC9", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("Designation", "EntitlementC1", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "EntitlementC2", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "EntitlementC3", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "EntitlementC4", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "EntitlementC5", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("Designation", "MobileExpenseC1", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "MobileExpenseC2", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "MobileExpenseC3", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "MobileExpenseC4", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("Designation", "InternationalTravelC1", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "InternationalTravelC2", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "InternationalTravelC3", "nvarchar(50)", true, currConn, transaction);

                retResults = DBTableFieldAdd("Designation", "DomesticlTravelC1", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "DomesticTravelC2", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "DomesticTravelC3", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "DomesticTravelC4", "nvarchar(50)", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "DomesticTravelC5", "nvarchar(50)", true, currConn, transaction);
                //retResults = DBTableFieldAdd("Designation", "OrderNo", "int", true, currConn, transaction);


                
               
                #endregion

                #region Field Add
                ////Type: bit - int - decimal(18, 2) - nvarchar(50) ////true = allow null
                retResults = DBTableFieldAdd("EmployeeJob", "IsSalalryProcess", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeLeftInformation", "IsSalalryProcess", "bit", true, currConn, transaction);


                retResults = DBTableFieldAdd("EarnLeaveEncashmentStatement", "EncashmentRatio","decimal(18, 2)", true, currConn,transaction);

                retResults = DBTableFieldAdd("FiscalYearDetail", "PeriodId", "int", true);

                retResults = DBTableFieldAdd("EmployeeJob", "BankAccountName", "nvarchar(50)", true);
                retResults = DBTableFieldAdd("EmployeeJob", "Routing_No", "nvarchar(50)", true);
                retResults = DBTableFieldAdd("EmployeeStructureGroup", "FixedOT", "decimal(18, 2)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "IsTAXApplicable", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeJob", "Force", "nvarchar(50)", true);
                retResults = DBTableFieldAdd("EmployeeJob", "Rank", "nvarchar(50)", true);
                retResults = DBTableFieldAdd("EmployeeJob", "Duration", "nvarchar(50)", true);
                retResults = DBTableFieldAdd("EmployeeJob", "Retirement", "nvarchar(50)", true);

                retResults = DBTableFieldAdd("EmployeeDependent", "IsDependentAllowance", "bit", true, currConn,transaction);
                retResults = DBTableFieldAdd("EmployeeStructureGroup", "TaxPortion", "decimal(18, 2)", true, currConn, transaction);
                retResults = DBTableFieldAdd("EmployeeStructureGroup", "BonusTaxPortion", "decimal(18, 2)", true, currConn, transaction);

                retResults = DBTableFieldAdd("Project", "OrderNo", "int", true, currConn, transaction);
                retResults = DBTableFieldAdd("Section", "OrderNo", "int", true, currConn, transaction);
                retResults = DBTableFieldAdd("Department", "OrderNo", "int", true, currConn, transaction);
                retResults = DBTableFieldAdd("Designation", "OrderNo", "int", true, currConn, transaction);


                retResults = DBTableFieldAdd("SalaryEmployee", "Other1", "nvarchar(100)", true, currConn, transaction);
                retResults = DBTableFieldAdd("SalaryEmployee", "Other2", "nvarchar(100)", true, currConn, transaction);
                retResults = DBTableFieldAdd("SalaryEmployee", "Other3", "nvarchar(100)", true, currConn, transaction);

                retResults = DBTableFieldAdd("SalaryBonusDetail", "IsManual", "bit", true, currConn, transaction);
                retResults = DBTableFieldAdd("SalaryBonusDetail", "EffectDate", "nvarchar(14)", true, currConn, transaction);
                retResults = DBTableFieldAdd("BonusStructure", "JobAgeTo", "int", true, currConn,transaction);
                retResults = DBTableFieldAdd("EnumReport", "ReportSL", "int", true, currConn,transaction);
                retResults = DBTableFieldAdd("MonthlyAttendance", "TotalLateInMins", "decimal(18, 2)", true, currConn,transaction);
                retResults = DBTableFieldAdd("MonthlyAttendance", "TotalEarlyOutMins", "decimal(18, 2)", true, currConn,transaction);
                retResults = DBTableFieldAdd("MonthlyAttendance", "OtherDeductionDay", "decimal(18, 2)", true, currConn,transaction);
                retResults = DBTableFieldAdd("SalaryBonusDetail", "TaxValue", "decimal(18, 2)", true, currConn,transaction);
                retResults = DBTableFieldAdd("SalaryBonusDetail", "NetPayAmount", "decimal(18, 2)", true, currConn,transaction);
                retResults = DBTableFieldAdd("EmployeeOtherDeduction", "GrossSalary", "decimal(18, 2)", true, currConn,transaction);
                retResults = DBTableFieldAdd("EmployeeOtherDeduction", "BasicSalary", "decimal(18, 2)", true, currConn,transaction);
                retResults = DBTableFieldAdd("EmployeeOtherDeduction", "Days", "decimal(8, 2)", true, currConn,transaction);
                retResults = DBTableFieldAdd("EnumLeaveType", "IsRegular", "bit", true, currConn,transaction);
                retResults = DBTableFieldAdd("Asset", "IsVehicle", "bit", true, currConn,transaction);
                retResults = DBTableFieldAdd("Asset", "RegNo", "nvarchar(100)", true, currConn,transaction);
                retResults = DBTableFieldAdd("Asset", "EngineNo", "nvarchar(100)", true, currConn,transaction);
                retResults = DBTableFieldAdd("Asset", "ChassisNo", "nvarchar(100)", true, currConn,transaction);
                retResults = DBTableFieldAdd("Asset", "Model", "nvarchar(100)", true, currConn,transaction);
                retResults = DBTableFieldAdd("EnumReport", "IsVisible", "bit", true, currConn,transaction);
                retResults = DBTableFieldAdd("EnumReport", "Remarks", "nvarchar(500)", true, currConn,transaction);
                retResults = DBTableFieldAdd("AttendanceMigration", "ProcessTime", "nvarchar(14)", true, currConn,transaction);

                #endregion

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }


                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            #region Catch and Finally

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion

            return retResults;
        }

        public string[] PF_DBUpdate(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction

                #region Table Add

                retResults = PF_DatabaseTableChanges(currConn, transaction);

                #endregion

                #region Field Add

                retResults = PF_DBTableFieldAdd("COAGroups", "GroupSL", "int", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("COAs", "COASL", "int", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("COAs", "IsRetainedEarning", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("COAs", "IsNetProfit", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("COAs", "IsDepreciation", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("COAs", "COAType", "nvarchar(100)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("COAs", "COAType", "nvarchar(100)", true, currConn, transaction);

                retResults = PF_DBTableFieldAdd("GLJournals", "IsYearClosing", "bit", true, currConn, transaction);



                ////Type: bit - int - decimal(18, 2) - nvarchar(50) ////true = allow null
                retResults = PF_DBTableFieldAdd("ReturnOnInvestments", "IsFixed", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("InvestmentNames", "AitInterest", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("InvestmentAccrued", "AitInterest", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("InvestmentAccrued", "NetInterest", "decimal(18, 2)", true, currConn, transaction);


                retResults = PF_DBTableFieldAdd("PFBankDeposits", "ReferenceNo", "nvarchar(100)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFBankDeposits", "TransactionMediaId", "nvarchar(200)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("Withdraws", "ReferenceNo", "nvarchar(100)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("Withdraws", "TransactionMediaId", "nvarchar(200)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("Investments", "InvestmentNameId", "int", true, currConn, transaction);



                retResults = PF_DBTableFieldAdd("ReturnOnInvestments", "ActualInterestAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ReturnOnInvestments", "ServiceChargeAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ReturnOnBankInterests", "ActualInterestAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ReturnOnBankInterests", "ServiceChargeAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFSettlements", "PFStartDate", "nvarchar(14)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFSettlements", "PFEndDate", "nvarchar(14)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFSettlements", "TotalPayableAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFSettlements", "AlreadyPaidAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFSettlements", "NetPayAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributions", "IsPaid", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "IsPaid", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("Withdraws", "TransactionType", "nvarchar(100)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("Withdraws", "Post", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ReturnOnInvestments", "IsBankDeposited", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ReturnOnBankInterests", "IsBankDeposited", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFBankDeposits", "ReferenceId", "int", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFDetails", "IsBankDeposited", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFBankDeposits", "TransactionType", "nvarchar(100)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFBankDeposits", "Post", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ForfeitureAccounts", "TotalForfeitValue", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributions", "FiscalYearDetailIdTo", "int", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributions", "TotalExpense", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributions", "AvailableDistributionAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributions", "MultiplicationFactor", "decimal(18, 9)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributions", "TotalWeightedContribution", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "FiscalYearDetailIdTo", "int", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "IndividualTotalContribution", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "ServiceLengthMonthWeight", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "IndividualWeightedContribution", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "MultiplicationFactor", "decimal(18, 9)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "IndividualProfitValue", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "ServiceLengthMonth", "decimal(18, 2)", true, currConn, transaction);


                #endregion

                #region Foreign Key

                retResults = PF_DBTableForeignKeyAdd("Investments", "InvestmentNames", "InvestmentNameId", "Id", currConn, transaction);

                retResults = PF_DBTableForeignKeyAdd("PFBankDeposits", "TransactionMedias", "TransactionMediaId", "Id", currConn, transaction);

                retResults = PF_DBTableForeignKeyAdd("Withdraws", "TransactionMedias", "TransactionMediaId", "Id", currConn, transaction);

                #endregion


                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            #region Catch and Finally

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion

            return retResults;
        }

        public string[] DatabaseTableChanges(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";


            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction

                #region EnumEMPCategory
                sqlText = " ";
                sqlText = @"
CREATE TABLE [dbo].[ELBalanceProcess](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NULL,
	[FiscalYearDetailId] [int] NULL,
	[Balance] [decimal](18, 2) NULL,
	[FiscalYear] [int] NULL,
 CONSTRAINT [PK_ELBalanceProcess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[ELBalanceProcess]  WITH CHECK ADD  CONSTRAINT [FK_ELBalanceProcess_EmployeeInfo] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[EmployeeInfo] ([Id])

ALTER TABLE [dbo].[ELBalanceProcess] CHECK CONSTRAINT [FK_ELBalanceProcess_EmployeeInfo]

ALTER TABLE [dbo].[ELBalanceProcess]  WITH CHECK ADD  CONSTRAINT [FK_ELBalanceProcess_FiscalYearDetail] FOREIGN KEY([FiscalYearDetailId])
REFERENCES [dbo].[FiscalYearDetail] ([Id])

ALTER TABLE [dbo].[ELBalanceProcess] CHECK CONSTRAINT [FK_ELBalanceProcess_FiscalYearDetail]


";

                retResults = NewTableAdd("ELBalanceProcess", sqlText, currConn, transaction);

                #endregion


                #region EnumEMPCategory
                sqlText = " ";
                sqlText = @"
CREATE TABLE [dbo].[EnumEMPCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
 CONSTRAINT [PK_EnumEMPCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

";

                retResults = NewTableAdd("EnumEMPCategory", sqlText, currConn, transaction);

                #endregion

                #region EnumEmpJobType
                sqlText = " ";
                sqlText = @"
CREATE TABLE [dbo].[EnumEmpJobType](
	[Id] [bigint] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Type] [nvarchar](200) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
 CONSTRAINT [PK_EnumEmpJobType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[EnumEmpJobType] ADD  CONSTRAINT [DF_EnumEmpJobType_IsActive]  DEFAULT ((1)) FOR [IsActive]

ALTER TABLE [dbo].[EnumEmpJobType] ADD  CONSTRAINT [DF_EnumEmpJobType_IsArchive]  DEFAULT ((0)) FOR [IsArchive]

";

                retResults = NewTableAdd("EnumEmpJobType", sqlText, currConn, transaction);

                #endregion

                #region EmployeeCompensatoryLeave
                sqlText = " ";
                sqlText = @"
CREATE TABLE [dbo].[EmployeeCompensatoryLeave](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[LeaveYear] [int] NOT NULL,
	[LeaveType_E] [nvarchar](200) NOT NULL,
	[FromDate] [nvarchar](14) NOT NULL,
	[ToDate] [nvarchar](14) NOT NULL,
	[TotalLeave] [decimal](18, 1) NOT NULL,
	[ApprovedBy] [nvarchar](20) NULL,
	[ApproveDate] [nvarchar](14) NULL,
	[IsApprove] [bit] NOT NULL,
	[RejectedBy] [nvarchar](20) NULL,
	[RejectDate] [nvarchar](14) NULL,
	[IsReject] [bit] NULL,
	[IsHalfDay] [bit] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
 CONSTRAINT [PK_EmployeeCompensatoryLeave_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[EmployeeCompensatoryLeave]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeCompensatoryLeave_EmployeeInfo] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[EmployeeInfo] ([Id])

ALTER TABLE [dbo].[EmployeeCompensatoryLeave] CHECK CONSTRAINT [FK_EmployeeCompensatoryLeave_EmployeeInfo]

";

                retResults = NewTableAdd("EmployeeCompensatoryLeave", sqlText, currConn, transaction);

                #endregion

                #region EarnLeaveEncashmentStatement
                sqlText = " ";
                sqlText = @"
CREATE TABLE [dbo].[EarnLeaveEncashmentStatement](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nchar](10) NULL,
	[Year] [int] NULL,
	[EncashmentBalance] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](500) NULL,
 CONSTRAINT [PK_EarnLeaveEncashmentStatement] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

";

                retResults = NewTableAdd("EarnLeaveEncashmentStatement", sqlText, currConn, transaction);

                #endregion

                #region EarnLeaveStatement
                sqlText = " ";
                sqlText = @"
CREATE TABLE [dbo].[EarnLeaveStatement](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nchar](10) NULL,
	[Year] [int] NULL,
	[CFBalance] [decimal](18, 2) NULL,
	[AnnualLeaveEntitle] [decimal](18, 2) NULL,
	[AnnualLeaveTaken] [decimal](18, 2) NULL,
	[AnnualBalance] [decimal](18, 2) NULL,
 CONSTRAINT [PK_EarnLeaveStatement] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

";

                retResults = NewTableAdd("EarnLeaveStatement", sqlText, currConn, transaction);

                #endregion

                #region Files
                sqlText = " ";
                sqlText = @"
    
CREATE TABLE [dbo].[EmployeeFile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[EmployeePersonalDetail_NIDFile] [nvarchar](150) NULL,
	[EmployeePersonalDetail_PassportFile] [nvarchar](150) NULL,
	[EmployeePersonalDetail_Fingerprint] [nvarchar](150) NULL,
	[EmployeePersonalDetail_VaccineFile1] [nvarchar](150) NULL,
	[EmployeePersonalDetail_VaccineFiles2] [nvarchar](150) NULL,
	[EmployeePersonalDetail_VaccineFile3] [nvarchar](150) NULL,
	[EmployeeInfo_PhotoName] [nvarchar](150) NULL,
	[EmployeePersonalDetail_DisabilityFile] [nvarchar](150) NULL,
	[EmployeePersonalDetail_Signature] [nvarchar](150) NULL,
	[EmployeeNominee_VaccineFile1] [nvarchar](150) NULL,
	[EmployeeNominee_VaccineFile2] [nvarchar](150) NULL,
	[EmployeeNominee_VaccineFile3] [nvarchar](150) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsArchive] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[EmployeePersonalDetail_TINFiles] [nvarchar](150) NULL,
	[SignatureFiles] [nvarchar](150) NULL,
	[FileName] [nvarchar](150) NULL,
	[Employeedependent_VaccineFile3] [nvarchar](150) NULL,
	[Employeedependent_VaccineFile2] [nvarchar](150) NULL,
	[Employeedependent_VaccineFile1] [nvarchar](150) NULL,
	[Extra_FileName] [nvarchar](150) NULL,
	[Experience_Certificate] [nvarchar](150) NULL,
	[Lng_Achivement] [nvarchar](150) NULL,
	[edu_Certificate] [nvarchar](150) NULL,
	[PassportVisa] [nvarchar](150) NULL,
	[BillVoucher] [nvarchar](150) NULL,
	[AssetFileName] [nvarchar](150) NULL,
	[Certificate] [nvarchar](150) NULL,
 CONSTRAINT [PK_EmployeeFile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF_EmployeeFile_FingerprintFile]  DEFAULT ((0)) FOR [EmployeePersonalDetail_Fingerprint]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF_EmployeeFile_VaccineFile1]  DEFAULT ((0)) FOR [EmployeePersonalDetail_VaccineFile1]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF_EmployeeFile_VaccineFiles2]  DEFAULT ((0)) FOR [EmployeePersonalDetail_VaccineFiles2]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF_EmployeeFile_VaccineFile3]  DEFAULT ((0)) FOR [EmployeePersonalDetail_VaccineFile3]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__Emplo__23150941]  DEFAULT ((0)) FOR [EmployeePersonalDetail_TINFiles]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__Signa__24092D7A]  DEFAULT ((0)) FOR [SignatureFiles]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__FileN__24FD51B3]  DEFAULT ((0)) FOR [FileName]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__Emplo__25F175EC]  DEFAULT ((0)) FOR [Employeedependent_VaccineFile3]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__Emplo__26E59A25]  DEFAULT ((0)) FOR [Employeedependent_VaccineFile2]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__Emplo__27D9BE5E]  DEFAULT ((0)) FOR [Employeedependent_VaccineFile1]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__Extra__28CDE297]  DEFAULT ((0)) FOR [Extra_FileName]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__Exper__29C206D0]  DEFAULT ((0)) FOR [Experience_Certificate]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__Lng_A__2AB62B09]  DEFAULT ((0)) FOR [Lng_Achivement]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__edu_C__2BAA4F42]  DEFAULT ((0)) FOR [edu_Certificate]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__Passp__2C9E737B]  DEFAULT ((0)) FOR [PassportVisa]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__BillV__2D9297B4]  DEFAULT ((0)) FOR [BillVoucher]


ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__Asset__2E86BBED]  DEFAULT ((0)) FOR [AssetFileName]

ALTER TABLE [dbo].[EmployeeFile] ADD  CONSTRAINT [DF__EmployeeF__Certi__2F7AE026]  DEFAULT ((0)) FOR [Certificate]




";
                #endregion

                retResults = NewTableAdd("EmployeeFile", sqlText, currConn, transaction);

                #region EmployeeProfessionalDegree
                sqlText = " ";
                sqlText = @"

CREATE TABLE [dbo].[EmployeeProfessionalDegree](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[Degree_E] [nvarchar](200) NOT NULL,
	[Institute] [nvarchar](500) NULL,
	[YearOfPassing] [nvarchar](4) NULL,
	[IsLast] [bit] NULL,
	[FileName] [nchar](50) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[Marks] [numeric](18, 2) NULL,
	[TotalYear] [int] NULL,
	[Level] [nvarchar](50) NULL,
 CONSTRAINT [PK_EmployeeProfessionalDegree] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[EmployeeProfessionalDegree] ADD  DEFAULT ((0)) FOR [Level]


ALTER TABLE [dbo].[EmployeeProfessionalDegree]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeProfessionalDegree_EmployeeInfo] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[EmployeeInfo] ([Id])


ALTER TABLE [dbo].[EmployeeProfessionalDegree] CHECK CONSTRAINT [FK_EmployeeProfessionalDegree_EmployeeInfo]





";

                retResults = NewTableAdd("EmployeeProfessionalDegree", sqlText, currConn, transaction);

                #endregion

                #region EnumProfessionalDegree
                sqlText = " ";
                sqlText = @"
CREATE TABLE [dbo].[EnumProfessionalDegree](
	[Id] [int] NOT NULL,
	[ProfessionalDegrees] [nvarchar](200) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
 CONSTRAINT [PK_EnumProfessionalDegree] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
";
                retResults = NewTableAdd("EnumProfessionalDegree", sqlText, currConn, transaction);

                #endregion

                #region SalaryHeadMappings
                sqlText = " ";
                sqlText = @"


CREATE TABLE [dbo].[SalaryHeadMappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SL] [int] NULL,
	[GroupType] [nvarchar](max) NULL,
	[FieldGroup] [nvarchar](max) NULL,
	[FieldName] [nvarchar](max) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_SalaryHeadMappings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

";

                retResults = NewTableAdd("SalaryHeadMappings", sqlText, currConn, transaction);

                #endregion

                #region TempSalary
                sqlText = " ";
                sqlText = @"


CREATE TABLE [dbo].[TempSalary](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FiscalYearDetailId] [varchar](100) NULL,
	[EmployeeId] [varchar](100) NULL,
	[Basic] [decimal](18, 4) NULL,
	[HouseRent] [decimal](18, 4) NULL,
	[Medical] [decimal](18, 4) NULL,
	[TransportAllowance] [decimal](18, 4) NULL,
	[Gross] [decimal](18, 4) NULL,
	[TAX] [decimal](18, 4) NULL,
	[TransportBill] [decimal](18, 4) NULL,
	[Stamp] [decimal](18, 4) NULL,
	[PFEmployer] [decimal](18, 4) NULL,
	[PFLoan] [decimal](18, 4) NULL,
	[STAFFWELFARE] [decimal](18, 4) NULL,
	[DeductionTotal] [decimal](18, 4) NULL,
	[NetSalary] [decimal](18, 4) NULL,
	[Othere(OT)] [decimal](18, 4) NULL,
	[Vehicle(Adj)] [decimal](18, 4) NULL,
	[Other(Bonus)] [decimal](18, 4) NULL,
	[LeaveWOPay] [decimal](18, 4) NULL,
	[GP] [decimal](18, 4) NULL,
	[Travel] [decimal](18, 4) NULL,
	[ChildAllowance] [decimal](18, 4) NULL,
	[MOBILE(Allowance)] [decimal](18, 4) NULL,
	[OtherAdjustment] [decimal](18, 4) NULL,
	[TotalAdjustment] [decimal](18, 4) NULL,
	[NetPayment] [decimal](18, 4) NULL,
	[ProjectId] [varchar](100) NULL,
	[DepartmentId] [varchar](100) NULL,
	[SectionId] [varchar](100) NULL,
	[DesignationId] [varchar](100) NULL,
 CONSTRAINT [PK_TempSalary] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

";

                retResults = NewTableAdd("TempSalary", sqlText, currConn, transaction);

                #endregion

                #region DesignationGroup
                sqlText = " ";
                sqlText = @"


CREATE TABLE [dbo].[DesignationGroup](
	[Id] [nvarchar](20) NOT NULL,
	[Serial] [int] NULL,
	[BranchId] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
 CONSTRAINT [PK_DesignationGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[DesignationGroup] ADD  CONSTRAINT [DF_Table_1_PriorityLevel]  DEFAULT ((0)) FOR [Serial]

";

                retResults = NewTableAdd("DesignationGroup", sqlText, currConn, transaction);

                #endregion

                #region SalaryEmployee
                sqlText = " ";
                sqlText = @"


CREATE TABLE [dbo].[SalaryEmployee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[DepartmentId] [nvarchar](20) NOT NULL,
	[SectionId] [nvarchar](20) NOT NULL,
	[DesignationId] [nvarchar](20) NOT NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[EmployeeStatus] [varchar](50) NULL,
	[GradeId] [nvarchar](20) NULL,
	[IsHold] [bit] NULL,
	[HoldBy] [nvarchar](20) NULL,
	[HoldAt] [nvarchar](14) NULL,
	[UnHoldBy] [nvarchar](20) NULL,
	[UnHoldAt] [nvarchar](14) NULL,
	[IsManual] [bit] NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [varchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [varchar](50) NULL
) ON [PRIMARY]



";

                retResults = NewTableAdd("SalaryEmployee", sqlText, currConn, transaction);

                #endregion

                #region TAXHeadMapping
                sqlText = " ";
                sqlText = @"
CREATE TABLE [dbo].[TAXHeadMapping](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[HeadName] [varchar](500) NULL,
 CONSTRAINT [PK_TAXHeadMapping] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
";

                retResults = NewTableAdd("TAXHeadMapping", sqlText, currConn, transaction);

                #endregion

                #region LeaveSchedule
                sqlText = " ";
                sqlText = @"


                CREATE TABLE [dbo].[LeaveSchedule](
	                [Id] [int] IDENTITY(1,1) NOT NULL,	
	                [EmployeeId] [varchar](100) NULL,
	                [LeaveDate] [varchar](100) NULL	

                ) ON [PRIMARY]

                ";

                retResults = NewTableAdd("LeaveSchedule", sqlText, currConn, transaction);

                #endregion

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion

                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            #region Catch and Finally

            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion

            return retResults;

        }

        public string[] PF_DatabaseTableChanges(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction

                string sqlText = "";

                #region InvestmentNames
                sqlText = " ";
                sqlText = @"


CREATE TABLE [dbo].[InvestmentNames](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Address] [nvarchar](200) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
 CONSTRAINT [PK_InvestmentNames] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

";

                retResults = NewTableAdd("InvestmentNames", sqlText, currConn, transaction);


                #endregion

                #region TransactionMedias
                sqlText = " ";
                sqlText = @"

CREATE TABLE [dbo].[TransactionMedias](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
 CONSTRAINT [PK_TransactionMedias] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


";

                #endregion

                retResults = NewTableAdd("TransactionMedias", sqlText, currConn, transaction);


                #region TransactionMedias
                sqlText = " ";
                sqlText = @"

CREATE TABLE [dbo].[EmployeeForFeiture_New](
	[Id] [int] NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[ForFeitureDate] [nvarchar](14) NULL,
	[EmployeeContribution] [decimal](18, 2) NULL,
	[EmployerContribution] [decimal](18, 2) NULL,
	[EmployeeProfit] [decimal](18, 2) NULL,
	[EmployerProfit] [decimal](18, 2) NULL,
	[Post] [bit] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL
) ON [PRIMARY]

";

                retResults = NewTableAdd("EmployeeForFeiture_New", sqlText, currConn, transaction);


                sqlText = " ";
                sqlText = @"

CREATE TABLE [dbo].[NetProfitYearEnds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransType] [nvarchar](50) NULL,
	[Year] [varchar](50) NULL,
	[YearStart] [varchar](50) NULL,
	[YearEnd] [varchar](50) NULL,
	[COAId] [int] NULL,
	[COAType] [varchar](50) NULL,
	[TransactionAmount] [decimal](18, 4) NULL,
	[NetProfit] [decimal](18, 4) NULL,
	[RetainedEarning] [decimal](18, 4) NULL,
 CONSTRAINT [pk_NetProfitYearEnds] PRIMARY KEY CLUSTERED (	[Id] ASC)
)
";
               retResults = NewTableAdd("NetProfitYearEnds", sqlText, currConn, transaction);

                sqlText = " ";
                sqlText = @"
CREATE TABLE [dbo].[NetProfitGFYearEnds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransType] [nvarchar](50) NULL,
	[Year] [varchar](50) NULL,
	[YearStart] [varchar](50) NULL,
	[YearEnd] [varchar](50) NULL,
	[COAId] [int] NULL,
	[COAType] [varchar](50) NULL,
	[TransactionAmount] [decimal](18, 4) NULL,
	[NetProfit] [decimal](18, 4) NULL,
	[RetainedEarning] [decimal](18, 4) NULL,
 CONSTRAINT [pk_NetProfitGFYearEnds] PRIMARY KEY CLUSTERED (	[Id] ASC)
)

";

                retResults = NewTableAdd("NetProfitGFYearEnds", sqlText, currConn, transaction);


                #endregion

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion

                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";

            }

            #region Catch and Finally

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion

            return retResults;
        }

        public string[] NewTableAdd(string TableName, string createQuery, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name

            string sqlText = "";
            int transResult = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrWhiteSpace(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");

                }
                else if (string.IsNullOrWhiteSpace(createQuery))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");

                }

                #endregion Validation

                #region Prefetch

                sqlText = "";

                sqlText += " IF  NOT EXISTS (SELECT * FROM sys.objects ";
                sqlText += " WHERE object_id = OBJECT_ID(N'" + TableName + "') AND type in (N'U'))";

                sqlText += " BEGIN";
                sqlText += " " + createQuery;
                sqlText += " END";

                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);

                cmdPrefetch.Transaction = transaction;
                transResult = cmdPrefetch.ExecuteNonQuery();

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }



                #endregion Prefetch

                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";

            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            #endregion

            return retResults;
        }

        public string[] DBTableForeignKeyAdd(string ForeignTable, string PrimaryTable, string ForeignField, string PrimaryFieldi, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                sqlText = "";
                sqlText += @" 
IF NOT EXISTS (SELECT * 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'dbo.FK_paramForeignTable_paramPrimaryTable')
   AND parent_object_id = OBJECT_ID(N'dbo.paramPrimaryTable')
)


ALTER TABLE [dbo].[paramForeignTable]  WITH CHECK ADD  CONSTRAINT [FK_paramForeignTable_paramPrimaryTable] FOREIGN KEY([paramForeignField])
REFERENCES [dbo].[paramPrimaryTable] ([paramPrimaryField])

ALTER TABLE [dbo].[paramForeignTable] CHECK CONSTRAINT [FK_paramForeignTable_paramPrimaryTable]


";
                sqlText = Regex.Replace(sqlText, "paramForeignTable", ForeignTable);
                sqlText = Regex.Replace(sqlText, "paramPrimaryTable", PrimaryTable);
                sqlText = Regex.Replace(sqlText, "paramForeignField", ForeignField);
                sqlText = Regex.Replace(sqlText, "paramPrimaryField", PrimaryField);

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            return retResults;
        }

        public string[] DBTableAdd(string TableName, string FieldName, string DataType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (objects)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (columns)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (data type)");
                }
                #endregion Validation
                sqlText += " IF  NOT EXISTS (SELECT * FROM sys.objects ";
                sqlText += " WHERE object_id = OBJECT_ID(N'" + TableName + "') AND type in (N'U'))";
                sqlText += " BEGIN";
                sqlText += " CREATE TABLE " + TableName + "( " + FieldName + " " + DataType + " null) ";
                sqlText += " END";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            return retResults;
        }
        
        public string[] DBTableFieldAdd(string TableName, string FieldName, string DataType, bool NullType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (objects)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (columns)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (data type)");
                }
                #endregion Validation
                sqlText = "";
                sqlText += " if not exists(select * from sys.columns ";
                sqlText += " where Name = N'" + FieldName + "' and Object_ID = Object_ID(N'" + TableName + "'))   ";
                sqlText += " begin";
                if (NullType == true)
                {
                    sqlText += " ALTER TABLE " + TableName + " ADD " + FieldName + " " + DataType + " NULL DEFAULT 0 ;";
                }
                else
                {
                    sqlText += " ALTER TABLE " + TableName + " ADD " + FieldName + " " + DataType + " NOT NULL DEFAULT 0 ;";
                }
                sqlText += " END";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            return retResults;
        }
        
        public string[] DBTableFieldAlter(string TableName, string FieldName, string DataType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (objects)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (columns)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (data type)");
                }
                #endregion Validation
                sqlText = "";
                sqlText += " ALTER TABLE " + TableName + " ALTER COLUMN " + FieldName + "   " + DataType + "";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            return retResults;
        }
        
        public string[] DBTableFieldRemove(string TableName, string FieldName, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (objects)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (columns)");
                }
                #endregion Validation
                sqlText = "";
                sqlText += " if exists(select * from sys.columns ";
                sqlText += " where Name = N'" + FieldName + "' and Object_ID = Object_ID(N'" + TableName + "'))   ";
                sqlText += " begin";
                sqlText += " ALTER TABLE " + TableName + " DROP COLUMN " + FieldName;
                sqlText += " END";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            return retResults;
        }

        public string[] TAX_DBTableFieldAdd(string TableName, string FieldName, string DataType, bool NullType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionTax();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction

                retResults = DBTableFieldAdd(TableName, FieldName, DataType, NullType, currConn, transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            return retResults;
        }

        public string[] PF_DBTableFieldAdd(string TableName, string FieldName, string DataType, bool NullType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction

                retResults = DBTableFieldAdd(TableName, FieldName, DataType, NullType, currConn, transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            return retResults;
        }

        public string[] PF_DBTableForeignKeyAdd(string ForeignTable, string PrimaryTable, string ForeignField, string PrimaryField, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction

                #region Execution

                retResults = DBTableForeignKeyAdd(ForeignTable, PrimaryTable, ForeignField, PrimaryField, currConn, transaction);
                #endregion
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            return retResults;
        }

        public int NewTableExistCheck(string TableName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int transResult = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");

                }

                //else if (string.IsNullOrEmpty(DataType))
                //{
                //    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");

                //}

                #endregion Validation
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Prefetch

                sqlText = "";

                sqlText += " IF  EXISTS (SELECT * FROM sys.objects ";
                sqlText += " WHERE object_id = OBJECT_ID(N'" + TableName + "') AND type in (N'U'))";

                sqlText += " BEGIN Select 1 END";
                sqlText += " else BEGIN Select 0 END";

                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);

                //cmdPrefetch.ExecuteScalar();
                cmdPrefetch.Transaction = transaction;
                transResult = (int)cmdPrefetch.ExecuteScalar();

                #endregion Prefetch
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                FileLogger.Log("CommonDAL", "NewTableExistCheck", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                FileLogger.Log("CommonDAL", "NewTableExistCheck", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion

            return transResult;
        }


        #endregion
    }
}
