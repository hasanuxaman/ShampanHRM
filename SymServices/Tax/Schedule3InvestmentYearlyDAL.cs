﻿using SymOrdinary;
using SymServices.Common;
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
    public class Schedule3InvestmentYearlyDAL
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
   FROM Schedule3InvestmentYearlies
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
        public List<Schedule3InvestmentVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
siy.Id
,siy.EmployeeId
,siy.ProjectId
,siy.DepartmentId
,siy.SectionId
,siy.DesignationId
,siy.FiscalYearId
,siy.Year
,siy.Line1
,siy.Line2
,siy.Line3
,siy.Line4
,siy.Line5
,siy.Line6
,siy.Line7
,siy.Line8
,siy.Line9
,siy.Line10
,siy.Line11
,siy.Remarks
,siy.IsActive
,siy.IsArchive
,siy.CreatedBy
,siy.CreatedAt
,siy.CreatedFrom
,siy.LastUpdateBy
,siy.LastUpdateAt
,siy.LastUpdateFrom

   FROM Schedule3InvestmentYearlies siy
WHERE  1=1  AND siy.IsArchive = 0

";
                if (Id > 0)
                {
                    sqlText += @" and siy.Id=@Id";
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
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new Schedule3InvestmentVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.FiscalYearId = dr["FiscalYearId"].ToString();
                    vm.Year = Convert.ToInt32(dr["Year"]);

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

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
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
        public string[] Insert(Schedule3InvestmentVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertSchedule3InvestmentYearly"; //Method Name
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
                vm.Id = _cDal.NextId("Schedule3InvestmentYearlies", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO Schedule3InvestmentYearlies(
Id
,EmployeeId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,FiscalYearId
,Year
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
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@EmployeeId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@FiscalYearId
,@Year
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
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    #endregion SqlText
                    #region SqlExecution

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearId", vm.FiscalYearId);
                    cmdInsert.Parameters.AddWithValue("@Year", vm.Year);
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
                        throw new ArgumentNullException("Unexpected error to update Schedule3InvestmentYearlies.", "");
                    }
                    #endregion SqlExecution

                }
                else
                {
                    retResults[1] = "This Schedule3InvestmentYearly already used!";
                    throw new ArgumentNullException("Please Input Schedule3InvestmentYearly Value", "");
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
        public string[] Update(Schedule3InvestmentVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Schedule3InvestmentYearly Update"; //Method Name
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
                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE Schedule3InvestmentYearlies SET";
                    sqlText += "   EmployeeId=@EmployeeId";
                    sqlText += " , ProjectId=@ProjectId";
                    sqlText += " , DepartmentId=@DepartmentId";
                    sqlText += " , SectionId=@SectionId";
                    sqlText += " , DesignationId=@DesignationId";
                    sqlText += " , FiscalYearId=@FiscalYearId";
                    sqlText += " , Year=@Year";
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

                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdUpdate.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearId", vm.FiscalYearId);
                    cmdUpdate.Parameters.AddWithValue("@Year", vm.Year);
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

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Schedule3InvestmentYearlies.", "");
                    }
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Schedule3InvestmentYearly Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Schedule3InvestmentYearly Update", "Could not found any item.");
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
            retResults[5] = "DeleteSchedule3InvestmentYearly"; //Method Name
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
                        sqlText = "UPDATE Schedule3InvestmentYearlies SET";
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
                        throw new ArgumentNullException("Schedule3InvestmentYearly Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Schedule3InvestmentYearly Information Delete", "Could not found any item.");
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
siy.Id
,siy.EmployeeId
,siy.ProjectId
,siy.DepartmentId
,siy.SectionId
,siy.DesignationId
,siy.FiscalYearId
,siy.Year
,siy.Line1
,siy.Line2
,siy.Line3
,siy.Line4
,siy.Line5
,siy.Line6
,siy.Line7
,siy.Line8
,siy.Line9
,siy.Line10
,siy.Line11
,siy.Remarks

   FROM Schedule3InvestmentYearlies siy
WHERE  1=1  AND siy.IsArchive = 0
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


        #endregion Methods
    }
}
