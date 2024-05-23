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
    public class SchedulePolicyDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        //==================SelectAll=================
        public List<SchedulePolicyVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SchedulePolicyVM> VMs = new List<SchedulePolicyVM>();
            SchedulePolicyVM vm;
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
sp.Id
,sp.LineNumber
,sp.ScheduleNo
,sp.SalaryHead
,ISNULL(sp.FromBasic, 0) FromBasic
,ISNULL(sp.IsFixed, 0) IsFixed
,ISNULL(sp.BasicPortion, 0) BasicPortion
,ISNULL(sp.EqualMaxMinAmount, 0) EqualMaxMinAmount
,sp.Remarks
   FROM SchedulePolicies sp
WHERE  1=1 

";
                if (Id > 0)
                {
                    sqlText += @" and sp.Id=@Id";
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
                    vm = new SchedulePolicyVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.LineNumber = dr["LineNumber"].ToString();
                    vm.ScheduleNo = dr["ScheduleNo"].ToString();
                    vm.SalaryHead = dr["SalaryHead"].ToString();
                    vm.FromBasic = Convert.ToBoolean(dr["FromBasic"]);
                    vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                    vm.BasicPortion = Convert.ToDecimal(dr["BasicPortion"]);
                    vm.EqualMaxMinAmount = Convert.ToDecimal(dr["EqualMaxMinAmount"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    //Id
                    //LineNumber
                    //ScheduleNo
                    //SalaryHead
                    //FromBasic
                    //IsFixed
                    //BasicPortion
                    //EqualMaxMinAmount
                    //Remarks


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


        //==================Update =================
        public string[] Update(List<SchedulePolicyVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "SchedulePolicyUpdate"; //Method Name
            int transResult = 0;
            string sqlText = "";
            string Id = "";
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

                if (VMs != null && VMs.Count > 0)
                {
                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE SchedulePolicies SET";
                    sqlText += " LineNumber=@LineNumber";
                    sqlText += " , ScheduleNo=@ScheduleNo	";
                    sqlText += " , SalaryHead=@SalaryHead";
                    sqlText += " , FromBasic=@FromBasic";
                    sqlText += " , IsFixed=@IsFixed";
                    sqlText += " , BasicPortion=@BasicPortion";
                    sqlText += " , EqualMaxMinAmount=@EqualMaxMinAmount";
                    sqlText += " , Remarks=@Remarks";


                    sqlText += " WHERE Id=@Id";
                    #endregion SqlText
                    #region SqlExecution
                    foreach (var item in VMs)
                    {
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", item.Id);
                        cmdUpdate.Parameters.AddWithValue("@LineNumber", item.LineNumber);
                        cmdUpdate.Parameters.AddWithValue("@ScheduleNo", item.ScheduleNo);
                        cmdUpdate.Parameters.AddWithValue("@SalaryHead", item.SalaryHead);
                        cmdUpdate.Parameters.AddWithValue("@FromBasic", item.FromBasic);
                        cmdUpdate.Parameters.AddWithValue("@IsFixed", item.IsFixed);
                        cmdUpdate.Parameters.AddWithValue("@BasicPortion", item.BasicPortion);
                        cmdUpdate.Parameters.AddWithValue("@EqualMaxMinAmount", item.EqualMaxMinAmount);

                        cmdUpdate.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        Id = item.Id.ToString();
                    }

                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update SchedulePolicies.", "");
                    }
                    #endregion SqlExecution

                    retResults[2] = Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("SchedulePolicy Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("SchedulePolicy Update", "Could not found any item.");
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



    }
}
