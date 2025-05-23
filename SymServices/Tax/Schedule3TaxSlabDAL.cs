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
    public class Schedule3TaxSlabDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        #region Methods
        //==================SelectAll=================
        public List<Schedule3TaxSlabVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<Schedule3TaxSlabVM> VMs = new List<Schedule3TaxSlabVM>();
            Schedule3TaxSlabVM vm;
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
ts.Id
,Name
,MaximumInvestment
,ts.Remarks
,ts.IsActive
,ts.IsArchive
,ts.CreatedBy
,ts.CreatedAt
,ts.CreatedFrom
,ts.LastUpdateBy
,ts.LastUpdateAt
,ts.LastUpdateFrom

   FROM Schedule3TaxSlabs ts
WHERE  1=1  AND ts.IsArchive = 0

";
                if (Id > 0)
                {
                    sqlText += @" and ts.Id=@Id";
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
                    vm = new Schedule3TaxSlabVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    vm.MaximumInvestment = Convert.ToDecimal(dr["MaximumInvestment"]);
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
        public string[] Insert(Schedule3TaxSlabVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertSchedule3TaxSlab"; //Method Name
            int transResult = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                //string tableName = "Schedule3TaxSlab";	
                //string[] fieldName = { "Code", "Name" };
                //string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };
                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInInsertWithBranch(tableName, fieldName[i], fieldValue[i], vm.BranchId, currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                #endregion Exist
                #endregion open connection and transaction
                #region Save
                #region FiscalYear Check
                //FiscalYearDAL fdal = new FiscalYearDAL();
                //var fp = fdal.PeriodLockByTransactionDate(vm.TransactionDateTime, currConn, transaction);
                //if (fp.PeriodLock)
                //{
                //    retResults[1] = "Fyscal Period is Lock";
                //    throw new ArgumentNullException("Fyscal Period is Lock", "Fyscal Period is Lock");
                //}
                #endregion FiscalYear Check
                vm.Id = Ordinary.NextId("Schedule3TaxSlabs", currConn, transaction);
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO Schedule3TaxSlabs(Id
,Name
,MaximumInvestment
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
VALUES (@Id
,@Name
,@MaximumInvestment
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        ";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                    cmdInsert.Parameters.AddWithValue("@MaximumInvestment", vm.MaximumInvestment);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, Schedule3TaxSlabVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Schedule3TaxSlabs.", "");
                    }
                    #region insert Details from Master into Detail Table
                    Schedule3TaxSlabDetailDAL _dDAL = new Schedule3TaxSlabDetailDAL();
                    if (vm.schedule3TaxSlabDetailVMs != null && vm.schedule3TaxSlabDetailVMs.Count > 0)
                    {
                        foreach (var schedule3TaxSlabDVM in vm.schedule3TaxSlabDetailVMs)
                        {
                            Schedule3TaxSlabDetailVM dVM = new Schedule3TaxSlabDetailVM();
                            dVM = schedule3TaxSlabDVM;
                            dVM.Schedule3TaxSlabId = vm.Id;
                            dVM.CreatedAt = vm.CreatedAt;
                            dVM.CreatedBy = vm.CreatedBy;
                            dVM.CreatedFrom = vm.CreatedFrom;
                            retResults = _dDAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException("Schedule3TaxSlab Details", "UnExpected Error.");
                            }
                        }
                    }
                    #endregion insert Details from Master into Detail Table
                }
                else
                {
                    retResults[1] = "This Schedule3TaxSlab already used!";
                    throw new ArgumentNullException("Please Input Schedule3TaxSlab Value", "");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        //==================Update =================
        public string[] Update(Schedule3TaxSlabVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Schedule3TaxSlab Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSchedule3TaxSlab"); }
                #endregion open connection and transaction
                #region FiscalYear Check
                //FiscalYearDAL fdal = new FiscalYearDAL();
                //var fp = fdal.PeriodLockByTransactionDate(vm.TransactionDateTime, currConn, transaction);
                //if (fp.PeriodLock)
                //{
                //    retResults[1] = "Fyscal Period is Lock";
                //    throw new ArgumentNullException("Fyscal Period is Lock", "Fyscal Period is Lock");
                //}
                #endregion FiscalYear Check
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update Schedule3TaxSlabs set";
                    sqlText += "   Name=@Name";
                    sqlText += " , MaximumInvestment=@MaximumInvestment";

                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name);
                    cmdUpdate.Parameters.AddWithValue("@MaximumInvestment", vm.MaximumInvestment);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, Schedule3TaxSlabVM.Remarks);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Schedule3TaxSlabs.", "");
                    }
                    #region insert Details from Master into Detail Table
                    Schedule3TaxSlabDetailDAL _dDal = new Schedule3TaxSlabDetailDAL();
                    if (vm.schedule3TaxSlabDetailVMs != null && vm.schedule3TaxSlabDetailVMs.Count > 0)
                    {
                        #region Delete Detail
                        try
                        {
                            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "Schedule3TaxSlabDetails", "Schedule3TaxSlabId", currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException("Schedule3TaxSlab Details", "could not updated.");
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentNullException("Schedule3TaxSlab Details", "could not updated.");
                        }
                        #endregion Delete Detail
                        #region Insert Detail Again
                        int i = 1;
                        foreach (var schedule3TaxSlabDVM in vm.schedule3TaxSlabDetailVMs)
                        {
                            Schedule3TaxSlabDetailVM dVM = new Schedule3TaxSlabDetailVM();
                            dVM = schedule3TaxSlabDVM;
                            dVM.Schedule3TaxSlabId = vm.Id;
                            dVM.CreatedAt = vm.LastUpdateAt;
                            dVM.CreatedBy = vm.LastUpdateBy;
                            dVM.CreatedFrom = vm.LastUpdateFrom;
                            retResults = _dDal.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException("Schedule3TaxSlab Details", "could not updated.");
                            }
                        }
                        #endregion Insert Detail Again
                    }
                    #endregion insert Details from Master into Detail Table
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", Schedule3TaxSlabVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Schedule3TaxSlab Update", "Could not found any item.");
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
        ////==================Delete =================
        public string[] Delete(Schedule3TaxSlabVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSchedule3TaxSlab"; //Method Name
            int transResult = 0;
            string sqlText = "";
            string retVal = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSchedule3TaxSlab"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (ids.Length >= 1)
                {
                    #region Delete Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText += " ";
                        sqlText += "DELETE Schedule3TaxSlabDetails";
                        sqlText += " WHERE Schedule3TaxSlabId=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update Schedule3TaxSlabDetails.", "");
                        }
                        sqlText = " ";
                        sqlText = "DELETE Schedule3TaxSlabs";
                        sqlText += " WHERE Id=@Id";
                        cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update Delete.", "");
                        }
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Schedule3TaxSlab Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                }
                else
                {
                    throw new ArgumentNullException("Schedule3TaxSlab Information Delete", "Could not found any item.");
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
        
        #endregion Methods
    }
}
