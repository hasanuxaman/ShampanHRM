using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
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
    public class EmployeeSchedule3TaxSlabDetailMonthlyDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion


        #region Methods
        //==================DropDown=================
        public List<EmployeeSchedule3TaxSlabDetailVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeSchedule3TaxSlabDetailVM> VMs = new List<EmployeeSchedule3TaxSlabDetailVM>();
            EmployeeSchedule3TaxSlabDetailVM vm;
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
   FROM EmployeeSchedule3TaxSlabDetailsMonthlies
WHERE  1=1
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeSchedule3TaxSlabDetailVM();
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
        public List<EmployeeSchedule3TaxSlabDetailVM> SelectAll(int Id = 0, int Schedule3Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeSchedule3TaxSlabDetailVM> VMs = new List<EmployeeSchedule3TaxSlabDetailVM>();
            EmployeeSchedule3TaxSlabDetailVM vm;
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
                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();

                #region sql statement
                #region SqlText
                sqlText = @"

SELECT

 ets.Id
,ets.Schedule3Id
,ets.EmployeeId
,ets.Year
,ISNULL(ets.FiscalYearDetailId   ,0)FiscalYearDetailId
,ISNULL(ets.FiscalYearDetailIdTo ,0)FiscalYearDetailIdTo
,ets.Schedule3TaxSlabId
,ets.SlabName
,ISNULL(ets.EarningCeilingMin, 0) EarningCeilingMin	
,ISNULL(ets.EarningCeilingMax, 0) EarningCeilingMax
,ets.Ceiling
,ets.Ratio
,ets.ActualAmount
,ets.RestAmount
,ets.TaxAmount
,ets.Remarks
,ets.IsActive
,ets.IsArchive
,ets.CreatedBy
,ets.CreatedAt
,ets.CreatedFrom
,ets.LastUpdateBy
,ets.LastUpdateAt
,ets.LastUpdateFrom
,ISNULL(ets.TransactionType,'Salary') TransactionType

,ve.EmpName EmployeeName
,ve.Code EmployeeCode
,ve.Department
,ve.Section
,ve.Project
,ve.Designation

FROM EmployeeSchedule3TaxSlabDetailsMonthlies ets
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fyd ON ets.FiscalYearDetailId=fyd.Id";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fydTo ON ets.FiscalYearDetailIdTo=fydTo.Id";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[ViewEmployeeInformation] ve ON ets.EmployeeId = ve.EmployeeId";
                sqlText += " WHERE  1=1  AND ets.IsArchive = 0";
                if (Id > 0)
                {
                    sqlText += " AND ets.Id=@Id";
                }
                if (Schedule3Id > 0)
                {
                    sqlText += " AND ets.Schedule3Id=@Schedule3Id";
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

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }

                if (Schedule3Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Schedule3Id", Schedule3Id);
                }
                int DivisionFactor = 12;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeSchedule3TaxSlabDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Schedule3Id = Convert.ToInt32(dr["Schedule3Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.FiscalYearDetailIdTo = Convert.ToInt32(dr["FiscalYearDetailIdTo"]);
                    vm.Schedule3TaxSlabId = Convert.ToInt32(dr["Schedule3TaxSlabId"]);


                    vm.SlabName = dr["SlabName"].ToString();


                    vm.EarningCeilingMin = Convert.ToDecimal(dr["EarningCeilingMin"]);
                    vm.EarningCeilingMax = Convert.ToDecimal(dr["EarningCeilingMax"]);
                    vm.Ceiling = Convert.ToDecimal(dr["Ceiling"]);
                    vm.Ratio = Convert.ToDecimal(dr["Ratio"]);
                    vm.ActualAmount = Convert.ToDecimal(dr["ActualAmount"]);
                    vm.RestAmount = Convert.ToDecimal(dr["RestAmount"]);
                    vm.TaxAmount = Convert.ToDecimal(dr["TaxAmount"]);

                    vm.TransactionType = dr["TransactionType"].ToString();

                    ////if (vm.TransactionType == "Bonus")
                    ////{
                    ////    DivisionFactor = 1;
                    ////}

                    vm.MonthlyCeiling = vm.Ceiling / DivisionFactor;
                    vm.MonthlyEarningCeilingMin = vm.EarningCeilingMin / DivisionFactor;
                    vm.MonthlyEarningCeilingMax = vm.EarningCeilingMax / DivisionFactor;

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

                    vm.EmployeeName = dr["EmployeeName"].ToString();
                    vm.EmployeeCode = dr["EmployeeCode"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
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
        public string[] Insert(EmployeeSchedule3TaxSlabDetailVM vm, string tType = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertEmployeeSchedule3TaxSlabDetail"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion
            #region Try
            try
            {
                #region Validation
                #endregion Validation
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
                vm.Id = _cDal.NextId("EmployeeSchedule3TaxSlabDetailsMonthlies", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO EmployeeSchedule3TaxSlabDetailsMonthlies(
Id
,Schedule3Id
,EmployeeId
,Year
,FiscalYearDetailId
,FiscalYearDetailIdTo
,Schedule3TaxSlabId
,SlabName
,EarningCeilingMin
,EarningCeilingMax
,Ceiling
,Ratio
,ActualAmount
,RestAmount
,TaxAmount
,TransactionType
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@Schedule3Id
,@EmployeeId
,@Year
,@FiscalYearDetailId
,@FiscalYearDetailIdTo
,@Schedule3TaxSlabId
,@SlabName
,@EarningCeilingMin
,@EarningCeilingMax
,@Ceiling
,@Ratio
,@ActualAmount
,@RestAmount
,@TaxAmount
,@TransactionType
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";


                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Schedule3Id", vm.Schedule3Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Year", vm.Year);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailIdTo", vm.FiscalYearDetailIdTo);
                    
                    cmdInsert.Parameters.AddWithValue("@Schedule3TaxSlabId", vm.Schedule3TaxSlabId);
                    cmdInsert.Parameters.AddWithValue("@SlabName", vm.SlabName);

                    cmdInsert.Parameters.AddWithValue("@EarningCeilingMin", vm.EarningCeilingMin);
                    cmdInsert.Parameters.AddWithValue("@EarningCeilingMax", vm.EarningCeilingMax);
                    cmdInsert.Parameters.AddWithValue("@Ceiling", vm.Ceiling);
                    cmdInsert.Parameters.AddWithValue("@Ratio", vm.Ratio);
                    cmdInsert.Parameters.AddWithValue("@ActualAmount", vm.ActualAmount);
                    cmdInsert.Parameters.AddWithValue("@RestAmount", vm.RestAmount);
                    cmdInsert.Parameters.AddWithValue("@TaxAmount", vm.TaxAmount);
                    cmdInsert.Parameters.AddWithValue("@TransactionType", tType);

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
                        throw new ArgumentNullException("Unexpected error to update EmployeeSchedule3TaxSlabDetailsMonthlies.", "");
                    }
                    #endregion SqlExecution

                }
                else
                {
                    retResults[1] = "This EmployeeSchedule3TaxSlabDetail already used!";
                    throw new ArgumentNullException("Please Input EmployeeSchedule3TaxSlabDetail Value", "");
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

        ////==================Delete =================
        public string[] Delete(EmployeeSchedule3TaxSlabDetailVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeSchedule3TaxSlabDetail"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeSchedule3TaxSlabDetail"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "UPDATE EmployeeSchedule3TaxSlabDetailsMonthlies SET";
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
                        throw new ArgumentNullException("EmployeeSchedule3TaxSlabDetail Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("EmployeeSchedule3TaxSlabDetail Information Delete", "Could not found any item.");
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

        ////==================Report=================
        public DataTable Report(EmployeeSchedule3TaxSlabDetailVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; // _settingDal.settingValue("Database", "HRMDB");
                #region sql statement
                #region SqlText
                sqlText = @"
SELECT

 ets.Id
,ets.Schedule3Id
,ets.EmployeeId
,ets.Year
,ets.FiscalYearDetailId
,ets.Schedule3TaxSlabId
,ets.SlabName
,ISNULL(ets.EarningCeilingMin,0) EarningCeilingMin
,ISNULL(ets.EarningCeilingMax,0) EarningCeilingMax
ets.Ceiling
,ets.Ratio
,ets.ActualAmount
,ets.RestAmount
,ets.TaxAmount
,ets.Remarks

,ve.EmpName EmployeeName
,ve.Code EmployeeCode
,ve.Department
,ve.Section
,ve.Project
,ve.Designation

FROM EmployeeSchedule3TaxSlabDetailsMonthlies ets
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fyd ON ets.FiscalYearDetailId=fyd.Id";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[ViewEmployeeInformation] ve ON ets.EmployeeId = ve.EmployeeId";
                sqlText += " WHERE  1=1  AND ets.IsArchive = 0";
                //AND td.Fiscalyeardetailid > = '10' AND td.Fiscalyeardetailid <= '12'
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

        //==================SelectEmployeeSchedule3TaxSlabDetails=================
        public List<EmployeeSchedule3TaxSlabDetailVM> SelectEmployeeSchedule3TaxSlabDetails(decimal taxableAmount, decimal eligibleAmount, int Schedule3TaxSlabId = 1, bool isMonth = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            List<Schedule3TaxSlabDetailVM> VMs = new List<Schedule3TaxSlabDetailVM>();
            EmployeeSchedule3TaxSlabDetailVM employeeSchedule3TaxSlabDetailVM = new EmployeeSchedule3TaxSlabDetailVM();
            List<EmployeeSchedule3TaxSlabDetailVM> employeeSchedule3TaxSlabDetailVMs = new List<EmployeeSchedule3TaxSlabDetailVM>();
            string sqlText = "";
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


                #region Fetch Schedule3TaxSlab And Schedule3TaxSlab Details

                Schedule3TaxSlabVM Schedule3TaxSlabVM = new Schedule3TaxSlabVM();
                Schedule3TaxSlabDAL _Schedule3TaxSlabDAL = new Schedule3TaxSlabDAL();
                Schedule3TaxSlabVM = _Schedule3TaxSlabDAL.SelectAll(Schedule3TaxSlabId, null, null, currConn, transaction).FirstOrDefault();


                int divisionFactor = 1;
                if (isMonth)
                {
                    divisionFactor = 12;
                }

                Schedule3TaxSlabDetailVM Schedule3TaxSlabDetailVM = new Schedule3TaxSlabDetailVM();
                List<Schedule3TaxSlabDetailVM> Schedule3TaxSlabDetailVMs = new List<Schedule3TaxSlabDetailVM>();
                Schedule3TaxSlabDetailDAL _Schedule3TaxSlabDetailDAL = new Schedule3TaxSlabDetailDAL();

                decimal totalTaxableAmount = taxableAmount * divisionFactor;

                string[] conFields = { "EarningCeilingMax>", "EarningCeilingMin<" };
                string[] conValues = { totalTaxableAmount.ToString(), totalTaxableAmount.ToString() };
                Schedule3TaxSlabDetailVMs = _Schedule3TaxSlabDetailDAL.SelectAll(0, Schedule3TaxSlabId, conFields, conValues, currConn, transaction);
                #endregion Fetch Schedule3TaxSlab And Schedule3TaxSlab Details



                foreach (Schedule3TaxSlabDetailVM vm in Schedule3TaxSlabDetailVMs)
                {


                    vm.MonthlyEarningCeilingMax = vm.EarningCeilingMax / divisionFactor;
                    vm.MonthlyEarningCeilingMin = vm.EarningCeilingMin / divisionFactor;
                    vm.MonthlyCeiling = vm.CeilingMax / divisionFactor;


                    //if (taxableAmount > vm.EarningMonthlyCeiling)
                    //{
                    //    continue;
                    //}


                    if (eligibleAmount > vm.MonthlyCeiling)
                    {
                        vm.ActualAmount = vm.MonthlyCeiling;
                        vm.RestAmount = eligibleAmount;
                        vm.TaxAmount = vm.ActualAmount * vm.Ratio / 100;
                        eligibleAmount = eligibleAmount - vm.MonthlyCeiling;
                        VMs.Add(vm);
                    }
                    else
                    {
                        vm.ActualAmount = eligibleAmount <= 0 ? 0 : eligibleAmount;
                        vm.RestAmount = eligibleAmount <= 0 ? 0 : eligibleAmount;
                        decimal tt = eligibleAmount * vm.Ratio / 100;
                        vm.TaxAmount = tt <= 0 ? 0 : tt;
                        eligibleAmount = tt <= 0 ? 0 : eligibleAmount - vm.MonthlyCeiling;
                        VMs.Add(vm);
                        //break;
                    }
                }

                #region Assign Schedule3TaxSlabDetailVM into EmployeeSchedule3TaxSlabDetailVM


                foreach (var item in VMs)
                {
                    employeeSchedule3TaxSlabDetailVM = new EmployeeSchedule3TaxSlabDetailVM();
                    employeeSchedule3TaxSlabDetailVM.Schedule3TaxSlabId = item.Schedule3TaxSlabId;
                    employeeSchedule3TaxSlabDetailVM.SlabName = item.SlabName;
                    employeeSchedule3TaxSlabDetailVM.EarningCeilingMin = item.EarningCeilingMin;
                    employeeSchedule3TaxSlabDetailVM.EarningCeilingMax = item.EarningCeilingMax;
                    employeeSchedule3TaxSlabDetailVM.Ceiling = item.CeilingMax;

                    employeeSchedule3TaxSlabDetailVM.MonthlyEarningCeilingMax = item.MonthlyEarningCeilingMax;
                    employeeSchedule3TaxSlabDetailVM.MonthlyEarningCeilingMax = item.MonthlyEarningCeilingMin;
                    employeeSchedule3TaxSlabDetailVM.MonthlyCeiling = item.MonthlyCeiling;
                    employeeSchedule3TaxSlabDetailVM.MonthlyCeiling = item.MonthlyCeiling;
                    employeeSchedule3TaxSlabDetailVM.Ratio = item.Ratio;
                    employeeSchedule3TaxSlabDetailVM.ActualAmount = item.ActualAmount;
                    employeeSchedule3TaxSlabDetailVM.RestAmount = item.RestAmount;
                    employeeSchedule3TaxSlabDetailVM.TaxAmount = item.TaxAmount;
                    employeeSchedule3TaxSlabDetailVMs.Add(employeeSchedule3TaxSlabDetailVM);
                }
                #endregion Assign Schedule3TaxSlabDetailVM into EmployeeSchedule3TaxSlabDetailVM



                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

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
            return employeeSchedule3TaxSlabDetailVMs;
        }


        #endregion Methods
    }
}
