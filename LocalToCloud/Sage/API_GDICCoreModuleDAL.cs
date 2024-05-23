using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SymOrdinary;
using SymViewModel.Sage;
using System.Linq;
using System.Threading;

namespace APICloud.Sage
{
    public class API_GDICCoreModuleDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        public static Thread thread;
        #endregion
        public List<CommissionBillVM> API_DropDown_CommissionBill(string CommissionBillNoList = "", string BranchCode = "", string StartDate = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CommissionBillVM> VMs = new List<CommissionBillVM>();
            CommissionBillVM vm;
            #endregion
            try
            {

                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionGDICCommissionBill();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT
distinct
 CommissionBillNo
,CONVERT(VARCHAR(8), ComDate, 112) ComDate
,BranchCode
,DeptCode

   FROM CommissionBill
WHERE  1=1
AND Lock = 'LOCKED'

";
                if (!string.IsNullOrWhiteSpace(CommissionBillNoList))
                {
                    sqlText += " AND CommissionBillNo NOT IN (" + CommissionBillNoList + ")";
                }

                if (!string.IsNullOrWhiteSpace(BranchCode))
                {
                    sqlText += @" AND BranchCode=@BranchCode";
                }
                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    sqlText += @" AND CONVERT(VARCHAR(8), ComDate, 112) > @StartDate";
                }


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                if (!string.IsNullOrWhiteSpace(BranchCode))
                {
                    objComm.Parameters.AddWithValue("@BranchCode", BranchCode);
                }
                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    objComm.Parameters.AddWithValue("@StartDate", StartDate);
                }


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CommissionBillVM();
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
                    vm.ComDate = Ordinary.StringToDate(dr["ComDate"].ToString());
                    vm.BranchCode = dr["BranchCode"].ToString();
                    vm.DeptCode = dr["DeptCode"].ToString();
                    vm.Name = vm.CommissionBillNo + "~" + vm.ComDate + "~" + vm.BranchCode + "~" + vm.DeptCode;
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


        //==================SelectAllDetail=================
        public List<CommissionBillDetailVM> API_SelectAll_CommissionBillDetail(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CommissionBillDetailVM> VMs = new List<CommissionBillDetailVM>();
            CommissionBillDetailVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionGDICCommissionBill();
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
 cbd.CommissionBillNo 
,CONVERT(VARCHAR(8), cb.ComDate, 112) ComDate
,cbd.SLNo 
,cbd.DeptCode 
,cbd.Class 
,cbd.DocumentNo 
,cbd.MRNo 
,CONVERT(VARCHAR(8), m.MRDate, 112) MRDate
,cbd.BranchCode 
,cbd.CustomerID 
,cbd.InsuredName 
,cbd.NetPremium
,cbd.RateOfCommission 
,cbd.CommissionAmount 
,cbd.TaxRate 
,cbd.TaxAmount 
,cbd.NetCommission 

FROM  CommissionBill_Details  cbd
LEFT OUTER JOIN CommissionBill cb ON cb.CommissionBillNo = cbd.CommissionBillNo
LEFT OUTER JOIN MRStatus m ON cbd.MRNo = m.MRNo

WHERE  1=1
AND cbd.NetPremium > 0

-------AND cb.Lock = 'LOCKED'
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

                sqlText += "  ORDER BY CONVERT(bigint, cbd.MRNo)";

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

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CommissionBillDetailVM();
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
                    vm.SLNo = Convert.ToInt32(dr["SLNo"].ToString());
                    vm.DeptCode = dr["DeptCode"].ToString();
                    vm.Class = dr["Class"].ToString();
                    vm.DocumentNo = dr["DocumentNo"].ToString();
                    vm.MRNo = dr["MRNo"].ToString();
                    vm.BranchCode = dr["BranchCode"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.InsuredName = dr["InsuredName"].ToString();
                    vm.NetPremium = Convert.ToDecimal(dr["NetPremium"].ToString());
                    vm.RateOfCommission = Convert.ToDecimal(dr["RateOfCommission"].ToString());
                    vm.CommissionAmount = Convert.ToDecimal(dr["CommissionAmount"].ToString());
                    vm.TaxRate = Convert.ToDecimal(dr["TaxRate"].ToString());
                    vm.TaxAmount = Convert.ToDecimal(dr["TaxAmount"].ToString());
                    vm.NetCommission = Convert.ToDecimal(dr["NetCommission"].ToString());
                    vm.ComDate = Ordinary.StringToDate(dr["ComDate"].ToString());
                    vm.MRDate = Ordinary.StringToDate(dr["MRDate"].ToString());

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

    }
}
