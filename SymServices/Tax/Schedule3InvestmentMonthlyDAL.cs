using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SymServices.Tax
{
    public class Schedule3InvestmentMonthlyDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        #region Methods
        //==================DropDown=================
        public List<Schedule3InvestmentVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<Schedule3InvestmentVM> VMs = new List<Schedule3InvestmentVM>();
            Schedule3InvestmentVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionTax();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT
Id
   FROM Schedule3InvestmentMonthlies
WHERE  1=1
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new Schedule3InvestmentVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    //vm.Code = dr["Code"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }
        //==================SelectAll=================
        public List<Schedule3InvestmentVM> SelectAll(int Id = 0, string[] conditionFields = null,
            string[] conditionValues = null, string tType = "", SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, List<EmloyeeTAXSlabVM> vms = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<Schedule3InvestmentVM> VMs = new List<Schedule3InvestmentVM>();
            Schedule3InvestmentVM vm;
            #endregion
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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement
                #region SqlText
                sqlText = @"

SELECT
sim.Id
,sim.EmployeeId
,ISNULL(sim.Schedule3TaxSlabId,0) Schedule3TaxSlabId
,sim.ProjectId
,sim.DepartmentId
,sim.SectionId
,sim.DesignationId
,sim.FiscalYearId
,sim.Year
,ISNULL(sim.FiscalYearDetailId  ,0)FiscalYearDetailId
,ISNULL(sim.FiscalYearDetailIdTo,0)FiscalYearDetailIdTo
,sim.Line1
,sim.Line2
,sim.Line3
,sim.Line4
,sim.Line5
,sim.Line6
,sim.Line7
,sim.Line8
,sim.Line9
,sim.Line10
,sim.Line11
,ISNULL(sim.TotalInvestmentAmount,0)TotalInvestmentAmount
,ISNULL(sim.NetIncomeAmount25P,0)NetIncomeAmount25P
,ISNULL(sim.MaximumInvestmentAmountMonthly,0)MaximumInvestmentAmountMonthly

,ISNULL(sim.InvestmentTotalTaxNotPayAmount,0)InvestmentTotalTaxNotPayAmount
,ISNULL(sim.TotalTaxableAmount, 0) TotalTaxableAmount
,ISNULL(sim.TransactionType, 'Salary') TransactionType
,sim.Remarks
,sim.IsActive
,sim.IsArchive
,sim.CreatedBy
,sim.CreatedAt
,sim.CreatedFrom
,sim.LastUpdateBy
,sim.LastUpdateAt
,sim.LastUpdateFrom

   FROM Schedule3InvestmentMonthlies sim
WHERE  1=1  AND sim.IsArchive = 0

";
                if (!string.IsNullOrWhiteSpace(tType))
                {
                    sqlText += @" and sim.TransactionType=@TransactionType";
                    
                }
                if (Id > 0)
                {
                    sqlText += @" and sim.Id=@Id";
                }

                if (vms != null && vms.Count > 0)
                {
                    sqlText += " AND EmployeeId in ('" + string.Join("','", vms.Select(x => x.EmployeeId)) + "')";
                }


                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                if (!string.IsNullOrWhiteSpace(tType))
                {
                    objComm.Parameters.AddWithValue("@TransactionType", tType);
                }

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new Schedule3InvestmentVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Schedule3TaxSlabId = Convert.ToInt32(dr["Schedule3TaxSlabId"]);


                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.FiscalYearId = dr["FiscalYearId"].ToString();
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.FiscalYearDetailIdTo = Convert.ToInt32(dr["FiscalYearDetailIdTo"]);
                    

                    vm.Line1 = Convert.ToDecimal(dr["Line1"]);
                    vm.Line2 = Convert.ToDecimal(dr["Line2"]);
                    vm.Line3 = Convert.ToDecimal(dr["Line3"]);
                    vm.Line4 = Convert.ToDecimal(dr["Line4"]);
                    vm.Line5 = Convert.ToDecimal(dr["Line5"]);
                    vm.Line6 = Convert.ToDecimal(dr["Line6"]);
                    vm.Line7 = Convert.ToDecimal(dr["Line7"]);
                    vm.Line8 = Convert.ToDecimal(dr["Line8"]);
                    vm.Line9 = Convert.ToDecimal(dr["Line9"]);
                    vm.Line10 = Convert.ToDecimal(dr["Line10"]);
                    vm.Line11 = Convert.ToDecimal(dr["Line11"]);
                    vm.TotalInvestmentAmount = Convert.ToDecimal(dr["TotalInvestmentAmount"]);
                    vm.NetIncomeAmount25P = Convert.ToDecimal(dr["NetIncomeAmount25P"]);
                    vm.MaximumInvestmentAmountMonthly = Convert.ToDecimal(dr["MaximumInvestmentAmountMonthly"]);
                    vm.InvestmentTotalTaxNotPayAmount = Convert.ToDecimal(dr["InvestmentTotalTaxNotPayAmount"]);
                    vm.TotalTaxableAmount = Convert.ToDecimal(dr["TotalTaxableAmount"]);
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }

        //==================Insert =================
        public string[] Insert(Schedule3InvestmentVM vm, string tType="", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertSchedule3InvestmentMonthly"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion
            #region Try
            try
            {
                #region Validation
                #endregion Validation
                #region Before Connection
                SettingDAL _settingDal = new SettingDAL();
                string withInvestment = _settingDal.settingValue("Schedule3Investment", "WithInvestment").Trim();
                #endregion



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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Save
                FiscalYearDAL fiscalYearDAL = new FiscalYearDAL();
                FiscalYearVM fiscalYearVM = new FiscalYearVM();
                fiscalYearVM = fiscalYearDAL.SelectByYear(vm.Year);
                vm.FiscalYearId = fiscalYearVM.Id;
                if (vm != null)
                {

                    #region Finding AmountForTaxSlab
                    if (vm.Schedule3TaxSlabId <= 0)
                    {
                        vm.Schedule3TaxSlabId = 1;
                    }
                    bool isMonth = true;


                    decimal eligibleAmount = 0;

                    #region Fetch MaximumInvestmentAmount from Schedule3TaxSlabDAL
                    //decimal maximumInvestmentAmount = 0;

                    //string[] returnFields = new string[5];
                    //string[] selectFields = { "MaximumInvestment" };

                    //string[] conFields = { "Id" };
                    //string[] conValues = { vm.Schedule3TaxSlabId.ToString() };

                    //returnFields = _cDal.SelectFieldsByCondition("Schedule3TaxSlabs", selectFields, conFields, conValues, currConn, transaction);
                    //maximumInvestmentAmount = Convert.ToDecimal(returnFields[0]);
                    //vm.MaximumInvestmentAmountMonthly = maximumInvestmentAmount / 12;

                    #endregion Fetch MaximumInvestmentAmount from Schedule3TaxSlabDAL

                    #region Fetch Total Income From Schedule1SalaryMonthlyDAL;
                    //_cDal.SelectByCondition
                    decimal incomeAmount = 0;
                    decimal pfAmount = 0;
                    decimal netIncomeAmount = 0;
                    decimal totalTaxableAmount = 0;



                    string[] schedule1SelectFields = { "TotalIncomeAmount", "Line15A", "TotalTaxableAmount" };

                    string[] schedule1conFields = { "EmployeeId", "FiscalYearDetailId" };
                    string[] schedule1conValues = { vm.EmployeeId.ToString(), vm.FiscalYearDetailId.ToString() };
                    string[] returnFields = new string[5];
                    returnFields = _cDal.SelectFieldsByCondition("Schedule1SalaryMonthlies", schedule1SelectFields, schedule1conFields, schedule1conValues, currConn, transaction);

                    incomeAmount = Convert.ToDecimal(returnFields[0]);
                    pfAmount = Convert.ToDecimal(returnFields[1]);
                    totalTaxableAmount = Convert.ToDecimal(returnFields[2]);

                    netIncomeAmount = incomeAmount - pfAmount;
                    vm.NetIncomeAmount25P = netIncomeAmount * 20 / 100;

                    vm.TotalTaxableAmount = totalTaxableAmount;
                    vm.MaximumInvestmentAmountMonthly = totalTaxableAmount * 20 / 100;


                    #endregion Total Income From Schedule1SalaryMonthlyDAL;


                    if (withInvestment == "Y")
                    {
                        decimal[] comparing = { vm.TotalInvestmentAmount, vm.MaximumInvestmentAmountMonthly, vm.NetIncomeAmount25P };
                        eligibleAmount = comparing.Min();
                    }
                    else
                    {
                        decimal[] comparing = { vm.MaximumInvestmentAmountMonthly, vm.NetIncomeAmount25P };
                        eligibleAmount = comparing.Min();
                    }

                    #endregion Finding AmountForTaxSlab

                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO Schedule3InvestmentMonthlies(
EmployeeId
,Schedule3TaxSlabId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,FiscalYearId
,Year
,FiscalYearDetailId
,Line1
,Line2
,Line3
,Line4
,Line5
,Line6
,Line7
,Line8
,Line9
,Line10
,Line11
,Total
,NetIncomeAmount25P
,MaximumInvestmentAmountMonthly
,InvestmentTotalTaxNotPayAmount
,TotalTaxableAmount
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@EmployeeId
,@Schedule3TaxSlabId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@FiscalYearId
,@Year
,@FiscalYearDetailId
,@Line1
,@Line2
,@Line3
,@Line4
,@Line5
,@Line6
,@Line7
,@Line8
,@Line9
,@Line10
,@Line11
,@Total
,@NetIncomeAmount25P
,@MaximumInvestmentAmountMonthly
,@InvestmentTotalTaxNotPayAmount
,@TotalTaxableAmount
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    #endregion SqlText
                    #region SqlExecution

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearId", vm.FiscalYearId);
                    cmdInsert.Parameters.AddWithValue("@Year", vm.Year);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@Line1", vm.Line1);
                    cmdInsert.Parameters.AddWithValue("@Line2", vm.Line2);
                    cmdInsert.Parameters.AddWithValue("@Line3", vm.Line3);
                    cmdInsert.Parameters.AddWithValue("@Line4", vm.Line4);
                    cmdInsert.Parameters.AddWithValue("@Line5", vm.Line5);
                    cmdInsert.Parameters.AddWithValue("@Line6", vm.Line6);
                    cmdInsert.Parameters.AddWithValue("@Line7", vm.Line7);
                    cmdInsert.Parameters.AddWithValue("@Line8", vm.Line8);
                    cmdInsert.Parameters.AddWithValue("@Line9", vm.Line9);
                    cmdInsert.Parameters.AddWithValue("@Line10", vm.Line10);
                    cmdInsert.Parameters.AddWithValue("@Line11", vm.Line11);
                    cmdInsert.Parameters.AddWithValue("@TotalInvestmentAmount", vm.TotalInvestmentAmount);
                    cmdInsert.Parameters.AddWithValue("@NetIncomeAmount25P", vm.NetIncomeAmount25P);
                    cmdInsert.Parameters.AddWithValue("@MaximumInvestmentAmountMonthly", vm.MaximumInvestmentAmountMonthly);

                    cmdInsert.Parameters.AddWithValue("@InvestmentTotalTaxNotPayAmount", vm.InvestmentTotalTaxNotPayAmount);
                    cmdInsert.Parameters.AddWithValue("@TotalTaxableAmount", vm.TotalTaxableAmount);

                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Schedule3InvestmentMonthlies.", "");
                    }
                    #endregion SqlExecution

                    #region Insert Into EmployeeSchedule3TaxSlabDetailDAL
                    #region Calculate EmployeeSchedule3TaxSlabDetailDAL
                    EmployeeSchedule3TaxSlabDetailMonthlyDAL _employeeSchedule3TaxSlabDetailDAL = new EmployeeSchedule3TaxSlabDetailMonthlyDAL();

                    vm.employeeSchedule3TaxSlabDetailVMs = _employeeSchedule3TaxSlabDetailDAL.SelectEmployeeSchedule3TaxSlabDetails(vm.TotalTaxableAmount, eligibleAmount, vm.Schedule3TaxSlabId, isMonth, currConn, transaction);
                    #endregion Calculate EmployeeSchedule3TaxSlabDetailDAL

                    if (vm.employeeSchedule3TaxSlabDetailVMs != null && vm.employeeSchedule3TaxSlabDetailVMs.Count > 0)
                    {
                        foreach (EmployeeSchedule3TaxSlabDetailVM EmployeeSchedule3TaxSlabDetailVM in vm.employeeSchedule3TaxSlabDetailVMs)
                        {
                            EmployeeSchedule3TaxSlabDetailVM dVM = new EmployeeSchedule3TaxSlabDetailVM();
                            dVM = EmployeeSchedule3TaxSlabDetailVM;
                            dVM.EmployeeId = vm.EmployeeId;
                            dVM.FiscalYearDetailId = vm.FiscalYearDetailId;
                            dVM.Year = vm.Year;
                            dVM.Schedule3Id = vm.Id;
                            dVM.Schedule3TaxSlabId = vm.Schedule3TaxSlabId;

                            dVM.CreatedAt = vm.CreatedAt;
                            dVM.CreatedBy = vm.CreatedBy;
                            dVM.CreatedFrom = vm.CreatedFrom;
                            retResults = _employeeSchedule3TaxSlabDetailDAL.Insert(dVM, tType, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Insert Into EmployeeSchedule3TaxSlabDetailDAL
                    #region Update TotalTaxNotPayAmount In Schedule3InvestmentMonthlies
                    retResults = UpdateTotalTaxNotPayAmount(vm, tType, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion
                    #region Update FinalTaxAmount In Schedule1SalaryMonthlies
                    Schedule1SalaryVM s1sVM = new Schedule1SalaryVM();
                    s1sVM.FiscalYearDetailId = vm.FiscalYearDetailId;
                    s1sVM.EmployeeId = vm.EmployeeId;
                    retResults = new Schedule1SalaryMonthlyDAL().UpdateFinalTaxAmount(s1sVM, tType, currConn, transaction);

                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion
                }
                else
                {
                    retResults[1] = "This Schedule3InvestmentMonthly already used!";
                    throw new ArgumentNullException("Please Input Schedule3InvestmentMonthly Value", "");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
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
            #region Results
            return retResults;
            #endregion
        }

        //==================Update =================
        public string[] Update(Schedule3InvestmentVM vm, string tType="", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Schedule3InvestmentMonthly Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                #region Before Connection

                SettingDAL _settingDal = new SettingDAL();
                string withInvestment = _settingDal.settingValue("Schedule3Investment", "WithInvestment").Trim();
                #endregion
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings



                    #region Finding AmountForTaxSlab
                    if (vm.Schedule3TaxSlabId <= 0)
                    {
                        vm.Schedule3TaxSlabId = 1;
                    }
                    bool isMonth = true;
                    if (tType == "YearlyTax")
                    {
                        isMonth = false;
                    }

                    decimal eligibleAmount = 0;

                    #region Fetch MaximumInvestmentAmount from Schedule3TaxSlabDAL
                    //decimal maximumInvestmentAmount = 0;

                    //string[] returnFields = new string[5];
                    //string[] selectFields = { "MaximumInvestment" };

                    //string[] conFields = { "Id" };
                    //string[] conValues = { vm.Schedule3TaxSlabId.ToString() };

                    //returnFields = _cDal.SelectFieldsByCondition("Schedule3TaxSlabs", selectFields, conFields, conValues, currConn, transaction);
                    //maximumInvestmentAmount = Convert.ToDecimal(returnFields[0]);
                    //vm.MaximumInvestmentAmountMonthly = maximumInvestmentAmount / 12;

                    #endregion Fetch MaximumInvestmentAmount from Schedule3TaxSlabDAL

                    #region Fetch Total Income From Schedule1SalaryMonthlyDAL;
                    //_cDal.SelectByCondition
                    decimal incomeAmount = 0;
                    decimal pfAmount = 0;
                    decimal netIncomeAmount = 0;

                    string[] schedule1SelectFields = { "TotalIncomeAmount", "Line15A", "TotalTaxableAmount" };

                    //Debugging
                    if (vm.EmployeeId == "1_66")
                    {

                    }

                    string[] schedule1conFields = { "EmployeeId", "FiscalYearDetailId", "FiscalYearDetailIdTo"  };
                    string[] schedule1conValues = { vm.EmployeeId.ToString(), vm.FiscalYearDetailId.ToString() , vm.FiscalYearDetailIdTo.ToString() };
                    string[] returnFields = new string[5];
                    returnFields = _cDal.SelectFieldsByCondition("Schedule1SalaryMonthlies", schedule1SelectFields, schedule1conFields, schedule1conValues, currConn, transaction);

                    incomeAmount = Convert.ToDecimal(returnFields[0]);
                    pfAmount = Convert.ToDecimal(returnFields[1]);

                    netIncomeAmount = incomeAmount - pfAmount;
                    vm.NetIncomeAmount25P = netIncomeAmount * 20 / 100;

                    decimal totalTaxableAmount = Convert.ToDecimal(returnFields[2]);
                    vm.TotalTaxableAmount = totalTaxableAmount;
                    vm.MaximumInvestmentAmountMonthly = totalTaxableAmount * 20 / 100;
                    ////////////////vm.MaximumInvestmentAmountMonthly = 15000000/12;
                    #endregion Total Income From Schedule1SalaryMonthlyDAL;


                    if (withInvestment == "Y")
                    {
                        decimal[] comparing = { vm.TotalInvestmentAmount, vm.MaximumInvestmentAmountMonthly, vm.NetIncomeAmount25P };
                        eligibleAmount = comparing.Min();
                    }
                    else
                    {
                        decimal[] comparing = { vm.MaximumInvestmentAmountMonthly, vm.NetIncomeAmount25P };
                        eligibleAmount = comparing.Min();
                    }


                    #endregion Finding AmountForTaxSlab


                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE Schedule3InvestmentMonthlies SET";
                    sqlText += "   EmployeeId=@EmployeeId";

                    sqlText += " , Schedule3TaxSlabId=@Schedule3TaxSlabId";
                    sqlText += " , ProjectId=@ProjectId";
                    sqlText += " , DepartmentId=@DepartmentId";
                    sqlText += " , SectionId=@SectionId";
                    sqlText += " , DesignationId=@DesignationId";
                    sqlText += " , FiscalYearId=@FiscalYearId";
                    sqlText += " , Year=@Year";
                    sqlText += " , FiscalYearDetailId=@FiscalYearDetailId";
                    sqlText += " , FiscalYearDetailIdTo=@FiscalYearDetailIdTo";
                    sqlText += " , Line1=@Line1";
                    sqlText += " , Line2=@Line2";
                    sqlText += " , Line3=@Line3";
                    sqlText += " , Line4=@Line4";
                    sqlText += " , Line5=@Line5";
                    sqlText += " , Line6=@Line6";
                    sqlText += " , Line7=@Line7";
                    sqlText += " , Line8=@Line8";
                    sqlText += " , Line9=@Line9";
                    sqlText += " , Line10=@Line10";
                    sqlText += " , Line11=@Line11";
                    sqlText += " , TotalInvestmentAmount=@TotalInvestmentAmount";
                    sqlText += " , NetIncomeAmount25P=@NetIncomeAmount25P";
                    sqlText += " , MaximumInvestmentAmountMonthly=@MaximumInvestmentAmountMonthly";


                    sqlText += " , InvestmentTotalTaxNotPayAmount=@InvestmentTotalTaxNotPayAmount";
                    sqlText += " , TotalTaxableAmount=@TotalTaxableAmount";

                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";
                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                    cmdUpdate.Parameters.AddWithValue("@Schedule3TaxSlabId", vm.Schedule3TaxSlabId);
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdUpdate.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearId", vm.FiscalYearId);
                    cmdUpdate.Parameters.AddWithValue("@Year", vm.Year);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailIdTo", vm.FiscalYearDetailIdTo);
                    
                    cmdUpdate.Parameters.AddWithValue("@Line1", vm.Line1);
                    cmdUpdate.Parameters.AddWithValue("@Line2", vm.Line2);
                    cmdUpdate.Parameters.AddWithValue("@Line3", vm.Line3);
                    cmdUpdate.Parameters.AddWithValue("@Line4", vm.Line4);
                    cmdUpdate.Parameters.AddWithValue("@Line5", vm.Line5);
                    cmdUpdate.Parameters.AddWithValue("@Line6", vm.Line6);
                    cmdUpdate.Parameters.AddWithValue("@Line7", vm.Line7);
                    cmdUpdate.Parameters.AddWithValue("@Line8", vm.Line8);
                    cmdUpdate.Parameters.AddWithValue("@Line9", vm.Line9);
                    cmdUpdate.Parameters.AddWithValue("@Line10", vm.Line10);
                    cmdUpdate.Parameters.AddWithValue("@Line11", vm.Line11);
                    cmdUpdate.Parameters.AddWithValue("@TotalInvestmentAmount", vm.TotalInvestmentAmount);
                    cmdUpdate.Parameters.AddWithValue("@NetIncomeAmount25P", vm.NetIncomeAmount25P);
                    cmdUpdate.Parameters.AddWithValue("@MaximumInvestmentAmountMonthly", vm.MaximumInvestmentAmountMonthly);
                    cmdUpdate.Parameters.AddWithValue("@InvestmentTotalTaxNotPayAmount", vm.InvestmentTotalTaxNotPayAmount);

                    cmdUpdate.Parameters.AddWithValue("@TotalTaxableAmount", vm.TotalTaxableAmount);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Schedule3InvestmentMonthlies.", "");
                    }
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #endregion SqlExecution

                    #region Insert Into EmployeeSchedule3TaxSlabDetailDAL
                    #region Calculate EmployeeSchedule3TaxSlabDetailDAL
                    EmployeeSchedule3TaxSlabDetailMonthlyDAL _employeeSchedule3TaxSlabDetailDAL = new EmployeeSchedule3TaxSlabDetailMonthlyDAL();
                    vm.employeeSchedule3TaxSlabDetailVMs = _employeeSchedule3TaxSlabDetailDAL.SelectEmployeeSchedule3TaxSlabDetails(vm.TotalTaxableAmount, eligibleAmount, vm.Schedule3TaxSlabId, isMonth, currConn, transaction);
                    #endregion Calculate EmployeeSchedule3TaxSlabDetailDAL
                    if (vm.employeeSchedule3TaxSlabDetailVMs != null && vm.employeeSchedule3TaxSlabDetailVMs.Count > 0)
                    {
                        #region Delete Detail
                        try
                        {
                            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "EmployeeSchedule3TaxSlabDetailsMonthlies", "Schedule3Id", currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentNullException("Employee Tax Slab Details Monthlies - Unexpected Error", "");
                        }
                        #endregion Delete Detail
                        #region Insert Detail Again
                        foreach (EmployeeSchedule3TaxSlabDetailVM EmployeeSchedule3TaxSlabDetailVM in vm.employeeSchedule3TaxSlabDetailVMs)
                        {
                            EmployeeSchedule3TaxSlabDetailVM dVM = new EmployeeSchedule3TaxSlabDetailVM();
                            dVM = EmployeeSchedule3TaxSlabDetailVM;
                            dVM.EmployeeId = vm.EmployeeId;
                            dVM.FiscalYearDetailId = vm.FiscalYearDetailId;
                            dVM.FiscalYearDetailIdTo = vm.FiscalYearDetailIdTo;
                            
                            dVM.Year = vm.Year;
                            dVM.Schedule3Id = vm.Id;
                            dVM.Schedule3TaxSlabId = vm.Schedule3TaxSlabId;

                            dVM.CreatedAt = vm.LastUpdateAt;
                            dVM.CreatedBy = vm.LastUpdateBy;
                            dVM.CreatedFrom = vm.LastUpdateFrom;
                            retResults = _employeeSchedule3TaxSlabDetailDAL.Insert(dVM, tType, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        #endregion Insert Detail Again

                    }

                    #endregion Insert Into EmployeeSchedule3TaxSlabDetailDAL
                    #region Update TotalTaxNotPayAmount In Schedule3InvestmentMonthlies
                    retResults = UpdateTotalTaxNotPayAmount(vm, tType, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion
                    #region Update FinalTaxAmount In Schedule1SalaryMonthlies
                    Schedule1SalaryVM s1sVM = new Schedule1SalaryVM();
                    s1sVM.FiscalYearDetailId = vm.FiscalYearDetailId;
                    s1sVM.FiscalYearDetailIdTo = vm.FiscalYearDetailIdTo;
                    s1sVM.EmployeeId = vm.EmployeeId;
                    retResults = new Schedule1SalaryMonthlyDAL().UpdateFinalTaxAmount(s1sVM, tType, currConn, transaction);

                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion

                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Schedule3InvestmentMonthly Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Schedule3InvestmentMonthly Update", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }


        ////==================Delete =================
        public string[] Delete(Schedule3InvestmentVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSchedule3InvestmentMonthly"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "UPDATE Schedule3InvestmentMonthlies SET";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " WHERE Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Schedule3InvestmentMonthly Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Schedule3InvestmentMonthly Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }

        //==================UpdateTotalTaxPayAmount =================
        public string[] UpdateTotalTaxNotPayAmount(Schedule3InvestmentVM vm, string tType="", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Schedule3InvestmentMonthly Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update TotalTaxNotPayAmount In Schedule3InvestmentMonthlies
                    sqlText = " ";
                    sqlText += @"
UPDATE Schedule3InvestmentMonthlies SET InvestmentTotalTaxNotPayAmount=a.TAXAmount
FROM 
(
SELECT DISTINCT FiscalYearDetailId,FiscalYearDetailIdTo,Schedule3Id,sum(TAXAmount)TAXAmount 
FROM EmployeeSchedule3TaxSlabDetailsMonthlies 
WHERE 1=1
AND (FiscalYearDetailId=@FiscalYearDetailId AND FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
AND ISNULL(TransactionType,'Salary')=@TransactionType
GROUP BY FiscalYearDetailId,FiscalYearDetailIdTo,Schedule3Id
) AS a 
WHERE 1=1
AND (a.FiscalYearDetailId=Schedule3InvestmentMonthlies.FiscalYearDetailId AND a.FiscalYearDetailIdTo=Schedule3InvestmentMonthlies.FiscalYearDetailIdTo )
AND a.Schedule3Id=Schedule3InvestmentMonthlies.Id
AND (Schedule3InvestmentMonthlies.FiscalYearDetailId=@FiscalYearDetailId AND Schedule3InvestmentMonthlies.FiscalYearDetailIdTo=@FiscalYearDetailIdTo)

";
                    SqlCommand cmdTUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdTUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdTUpdate.Parameters.AddWithValue("@FiscalYearDetailIdTo", vm.FiscalYearDetailIdTo);
                    cmdTUpdate.Parameters.AddWithValue("@TransactionType", tType);
                    
                    var exeRes = cmdTUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    //if (transResult <= 0)
                    //{
                    //    retResults[3] = sqlText;
                    //    throw new ArgumentNullException("Unexpected error to update Schedule3 Salary Monthlies.", "");
                    //}
                    #endregion Update TotalTaxNotPayAmount In Schedule3InvestmentMonthlies
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit

                    #endregion Commit
                }
                else
                {
                    throw new ArgumentNullException("Schedule3InvestmentMonthly Update", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }

        ////==================Report=================
        public DataTable Report(Schedule3InvestmentVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionTax();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                #region SqlText
                sqlText = @"
SELECT
sim.Id
,sim.EmployeeId
,sim.ProjectId
,sim.DepartmentId
,sim.SectionId
,sim.DesignationId
,sim.FiscalYearId
,sim.Year
,sim.FiscalYearDetailId
,sim.Line1
,sim.Line2
,sim.Line3
,sim.Line4
,sim.Line5
,sim.Line6
,sim.Line7
,sim.Line8
,sim.Line9
,sim.Line10
,sim.Line11
,sim.Remarks

   FROM Schedule3InvestmentMonthlies sim
WHERE  1=1  AND sim.IsArchive = 0
";

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                da.Fill(dt);
                #endregion SqlExecution

                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }


        #region Process

        //==================InsertUpdate =================
        public string[] InsertUpdate(string FiscalYearDetailId, string FiscalYearDetailIdTo, string tType = "",
            ShampanIdentityVM auditvm = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            List<EmloyeeTAXSlabVM> vms = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertProcessUpdate"; //Method Name
            int transResult = 0;
            string sqlText = "";
            #endregion
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction
                #region Insert
                retResults = InsertFromSchedule1(FiscalYearDetailId, FiscalYearDetailIdTo, tType, auditvm, currConn, transaction,vms);

                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], "");
                }
                #endregion Insert
                #region Process
                #region Select Inserted Data
                Schedule3InvestmentVM simVM = new Schedule3InvestmentVM();
                List<Schedule3InvestmentVM> simVMs = new List<Schedule3InvestmentVM>();
                string[] conditionFields = { "sim.FiscalYearDetailId", "sim.FiscalYearDetailIdTo" };
                string[] conditionValues = { FiscalYearDetailId, FiscalYearDetailIdTo };
                simVMs = SelectAll(0, conditionFields, conditionValues, tType, currConn, transaction,vms);
                #endregion Select Inserted Data
                #endregion Process
                #region Update
                foreach (var item in simVMs)
                {
                    retResults = Update(item, tType, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                }

                #endregion Update


                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Schedule3 Investment Monthly Saved Successfully!";
                #endregion Commit
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
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



        //==================InsertFromSchedule1 =================
        public string[] InsertFromSchedule1(string FiscalYearDetailId, string FiscalYearDetailIdTo, string tType = "",
            ShampanIdentityVM auditvm = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, List<EmloyeeTAXSlabVM> vms = null)

        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertFromSchedule1"; //Method Name
            int transResult = 0;
            string sqlText = "";

            #endregion

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            try
            {

                string hrmDB = _dbsqlConnection.HRMDB;

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

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Insert");
                }

                #endregion open connection and transaction

                #region Save

                sqlText = "  ";
                sqlText += @" 
---------------Declaration---------------
--------declare @EmployeeId as varchar (100)
--------declare @TransactionType as varchar (100)
--------declare @FiscalYearDetailId as int
--------declare @FiscalYearDetailIdTo as int
--------declare @FiscalYear as varchar(20)
--------declare @FiscalYearId as varchar(20)

--------set @EmployeeId = '1_99'
--------set @FiscalYearDetailId = '1030'
--------set @FiscalYearDetailIdTo = '1030'
--------set @TransactionType = 'Salary'

DELETE from Schedule3InvestmentMonthlies
WHERE 1=1
AND (FiscalYearDetailId=@FiscalYearDetailId AND FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
AND ISNULL(TransactionType,'Salary')=@TransactionType

DELETE from EmployeeSchedule3TaxSlabDetailsMonthlies
WHERE
1=1 
AND (FiscalYearDetailId=@FiscalYearDetailId AND FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
AND ISNULL(TransactionType,'Salary')=@TransactionType

INSERT INTO Schedule3InvestmentMonthlies(
Schedule3TaxSlabId
,EmployeeId,ProjectId,DepartmentId,SectionId,DesignationId,FiscalYearId,Year
,FiscalYearDetailId
,FiscalYearDetailIdTo
,Line1,Line2,Line3,Line4,Line5,Line6,Line7,Line8,Line9,Line10,Line11
,TotalInvestmentAmount
,NetIncomeAmount25P
,MaximumInvestmentAmountMonthly
,InvestmentTotalTaxNotPayAmount
,TotalTaxableAmount
,TransactionType
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) 

SELECT 
1 Schedule3TaxSlabId
,EmployeeId,ProjectId,DepartmentId,SectionId,DesignationId,FiscalYearId,Year
,FiscalYearDetailId
,FiscalYearDetailIdTo
,0,0,0,0,0,0,0,0,0,0,0
,0 TotalInvestmentAmount
,(TotalIncomeAmount*20/100) NetIncomeAmount25P
,(TotalTaxableAmount*20/100) MaximumInvestmentAmountMonthly
,0 InvestmentTotalTaxNotPayAmount
,TotalTaxableAmount
--,'-',1,0,1,1,1
,@TransactionType
,'NA', @IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
from Schedule1SalaryMonthlies
WHERE 1=1
AND (FiscalYearDetailId=@FiscalYearDetailId AND FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
AND ISNULL(TransactionType,'Salary') = @TransactionType
";

                if (vms != null && vms.Count > 0)
                {
                    sqlText += " AND EmployeeId in ('" + string.Join("','", vms.Select(x => x.EmployeeId)) + "')";
                }


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailIdTo", FiscalYearDetailIdTo);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);

                cmdInsert.Parameters.AddWithValue("@TransactionType", tType);

                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                if (transResult <= 0)
                {
                    retResults[1] = "No Data Found in Schedule1 Salary!";
                    return retResults;
                    //throw new ArgumentNullException(retResults[1], "");
                }

                #endregion Save

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                retResults[0] = "Success";
                retResults[1] = "Schedule3 Investment Monthly Saved Successfully!";

                #endregion Commit
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
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

        #endregion Process

        #endregion Methods
    }
}
