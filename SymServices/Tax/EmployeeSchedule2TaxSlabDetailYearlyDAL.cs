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
    public class EmployeeSchedule2TaxSlabDetailYearlyDAL
    {
        
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        public List<EmployeeSchedule2TaxSlabDetailVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeSchedule2TaxSlabDetailVM> VMs = new List<EmployeeSchedule2TaxSlabDetailVM>();
            EmployeeSchedule2TaxSlabDetailVM vm;
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
   FROM EmployeeSchedule2TaxSlabDetailsYearlies
WHERE  1=1
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeSchedule2TaxSlabDetailVM();
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
        public List<EmployeeSchedule2TaxSlabDetailVM> SelectAll(int Id = 0, int Schedule2Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeSchedule2TaxSlabDetailVM> VMs = new List<EmployeeSchedule2TaxSlabDetailVM>();
            EmployeeSchedule2TaxSlabDetailVM vm;
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
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim()

                #region sql statement
                #region SqlText
                sqlText = @"

SELECT

 ets.Id
,ets.Schedule2Id
,ets.EmployeeId
,ets.Year
,ets.FiscalYearId
,ets.Schedule2TaxSlabId
,ets.SlabName
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

,ve.EmpName EmployeeName
,ve.Code EmployeeCode
,ve.Department
,ve.Section
,ve.Project
,ve.Designation

FROM EmployeeSchedule2TaxSlabDetailsYearlies ets
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[ViewEmployeeInformation] ve ON ets.EmployeeId = ve.EmployeeId";
                sqlText += " WHERE  1=1  AND ets.IsArchive = 0";
                if (Id > 0)
                {
                    sqlText += " AND ets.Id=@Id";
                }
                if (Schedule2Id > 0)
                {
                    sqlText += " AND ets.Schedule2Id=@Schedule2Id";
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

                 if (Schedule2Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Schedule2Id", Schedule2Id);
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeSchedule2TaxSlabDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Schedule2Id = Convert.ToInt32(dr["Schedule2Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.FiscalYearId = dr["FiscalYearId"].ToString();
                    vm.Schedule2TaxSlabId = Convert.ToInt32(dr["Schedule2TaxSlabId"]);


                    vm.SlabName = dr["SlabName"].ToString();
                    vm.Ceiling = Convert.ToDecimal(dr["Ceiling"]);
                    vm.Ratio = Convert.ToDecimal(dr["Ratio"]);
                    vm.ActualAmount = Convert.ToDecimal(dr["ActualAmount"]);
                    vm.RestAmount = Convert.ToDecimal(dr["RestAmount"]);
                    vm.TaxAmount = Convert.ToDecimal(dr["TaxAmount"]);
                    vm.MonthlyCeiling = vm.Ceiling /12;

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
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
        public string[] Insert(EmployeeSchedule2TaxSlabDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertEmployeeSchedule2TaxSlabDetail"; //Method Name
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
                vm.Id = _cDal.NextId("EmployeeSchedule2TaxSlabDetailsYearlies", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO EmployeeSchedule2TaxSlabDetailsYearlies(
Id
,Schedule2Id
,EmployeeId
,Year
,FiscalYearId
,Schedule2TaxSlabId
,SlabName
,Ceiling
,Ratio
,ActualAmount
,RestAmount
,TaxAmount

,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@Schedule2Id
,@EmployeeId
,@Year
,@FiscalYearId
,@Schedule2TaxSlabId
,@SlabName
,@Ceiling
,@Ratio
,@ActualAmount
,@RestAmount
,@TaxAmount
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Schedule2Id", vm.Schedule2Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Year", vm.Year);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearId", vm.FiscalYearId);
                    cmdInsert.Parameters.AddWithValue("@Schedule2TaxSlabId", vm.Schedule2TaxSlabId);
                    cmdInsert.Parameters.AddWithValue("@SlabName", vm.SlabName);
                    cmdInsert.Parameters.AddWithValue("@Ceiling", vm.Ceiling);
                    cmdInsert.Parameters.AddWithValue("@Ratio", vm.Ratio);
                    cmdInsert.Parameters.AddWithValue("@ActualAmount", vm.ActualAmount);
                    cmdInsert.Parameters.AddWithValue("@RestAmount", vm.RestAmount);
                    cmdInsert.Parameters.AddWithValue("@TaxAmount", vm.TaxAmount);

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
                        throw new ArgumentNullException("Unexpected error to update EmployeeSchedule2TaxSlabDetailsYearlies.", "");
                    }
                    #endregion SqlExecution

                }
                else
                {
                    retResults[1] = "This EmployeeSchedule2TaxSlabDetail already used!";
                    throw new ArgumentNullException("Please Input EmployeeSchedule2TaxSlabDetail Value", "");
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
        public string[] Delete(EmployeeSchedule2TaxSlabDetailVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeSchedule2TaxSlabDetail"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeSchedule2TaxSlabDetail"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "UPDATE EmployeeSchedule2TaxSlabDetailsYearlies SET";
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
                        throw new ArgumentNullException("EmployeeSchedule2TaxSlabDetail Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("EmployeeSchedule2TaxSlabDetail Information Delete", "Could not found any item.");
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
        public DataTable Report(EmployeeSchedule2TaxSlabDetailVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
                string hrmDB =  _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB");
                #region sql statement
                #region SqlText
                sqlText = @"
SELECT

 ets.Id
,ets.Schedule2Id
,ets.EmployeeId
,ets.Year
,ets.FiscalYearId
,ets.Schedule2TaxSlabId
,ets.SlabName
,ets.Ceiling
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

FROM EmployeeSchedule2TaxSlabDetailsYearlies ets
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[ViewEmployeeInformation] ve ON ets.EmployeeId = ve.EmployeeId";
                sqlText += " WHERE  1=1  AND ets.IsArchive = 0";
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
        #endregion Methods

    }
}
