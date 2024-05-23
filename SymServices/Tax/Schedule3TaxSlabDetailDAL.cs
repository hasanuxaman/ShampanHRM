using SymOrdinary;
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
    public class Schedule3TaxSlabDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        #region Methods
        //==================SelectAll=================
        public List<Schedule3TaxSlabDetailVM> SelectAll(int Id = 0, int Schedule3TaxSlabId = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<Schedule3TaxSlabDetailVM> VMs = new List<Schedule3TaxSlabDetailVM>();
            Schedule3TaxSlabDetailVM vm;
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
tsd.Id
,tsd.Schedule3TaxSlabId
,tsd.SlabName




,ISNULL(tsd.EarningCeilingMin, 0)         EarningCeilingMin
,ISNULL(tsd.EarningCeilingMax, 0)         EarningCeilingMax
,ISNULL(tsd.CeilingMin	     , 0)         CeilingMin	
,ISNULL(tsd.CeilingMax	     , 0)         CeilingMax	

,tsd.Ratio

,tsd.Remarks
,tsd.IsActive
,tsd.IsArchive
,tsd.CreatedBy
,tsd.CreatedAt
,tsd.CreatedFrom
,tsd.LastUpdateBy
,tsd.LastUpdateAt
,tsd.LastUpdateFrom

   FROM Schedule3TaxSlabDetails tsd
WHERE  1=1  AND tsd.IsArchive = 0

";
                if (Id > 0)
                {
                    sqlText += @" and tsd.Id=@Id";
                }

                if (Schedule3TaxSlabId > 0)
                {
                    sqlText += @" and tsd.Schedule3TaxSlabId=@Schedule3TaxSlabId";
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

                if (Schedule3TaxSlabId > 0)
                {
                    objComm.Parameters.AddWithValue("@Schedule3TaxSlabId", Schedule3TaxSlabId);
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new Schedule3TaxSlabDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Schedule3TaxSlabId = Convert.ToInt32(dr["Schedule3TaxSlabId"]);
                    vm.SlabName = dr["SlabName"].ToString();
                    
                    vm.EarningCeilingMin = Convert.ToDecimal(dr["EarningCeilingMin"]);
                    vm.EarningCeilingMax = Convert.ToDecimal(dr["EarningCeilingMax"]);
                    vm.CeilingMin        = Convert.ToDecimal(dr["CeilingMin"]);
                    vm.CeilingMax        = Convert.ToDecimal(dr["CeilingMax"]);

                    vm.Ratio = Convert.ToDecimal(dr["Ratio"]);

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
        public string[] Insert(Schedule3TaxSlabDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = { "Fail", "Fail", Id.ToString(), sqlText, "ex", "InsertSchedule3TaxSlabDetail" };
            //0 - Success or Fail//1 - Success or Fail Message//2 - Return Id//3 - SQL Query//4 - catch ex//5 - Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionTax();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion open connection and transaction
                #region Save
                vm.Id = Ordinary.NextId("Schedule3TaxSlabDetails", currConn, transaction);
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO Schedule3TaxSlabDetails(Id
,Schedule3TaxSlabId
,SlabName
,EarningCeilingMin
,EarningCeilingMax
,CeilingMin
,CeilingMax

,Ratio
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
VALUES (@Id
,@Schedule3TaxSlabId
,@SlabName
,@EarningCeilingMin
,@EarningCeilingMax
,@CeilingMin
,@CeilingMax

,@Ratio
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Schedule3TaxSlabId", vm.Schedule3TaxSlabId);
                    cmdInsert.Parameters.AddWithValue("@SlabName", vm.SlabName);
                    cmdInsert.Parameters.AddWithValue("@EarningCeilingMin", vm.EarningCeilingMin);
                    cmdInsert.Parameters.AddWithValue("@EarningCeilingMax", vm.EarningCeilingMax);
                    cmdInsert.Parameters.AddWithValue("@CeilingMin", vm.CeilingMin);
                    cmdInsert.Parameters.AddWithValue("@CeilingMax", vm.CeilingMax);
                    
                    
                    cmdInsert.Parameters.AddWithValue("@Ratio", vm.Ratio);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This Schedule3TaxSlabDetail already used!";
                    throw new ArgumentNullException("Please Input Schedule3TaxSlabDetail Value", "");
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
        
        
        
        #endregion Methods
    }
}
