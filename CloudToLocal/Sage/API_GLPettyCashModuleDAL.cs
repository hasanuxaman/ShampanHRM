using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SymOrdinary;
using SymViewModel.Sage;
using System.Linq;
using System.Threading;

namespace APILocal.Sage
{
    public class API_GLPettyCashModuleDAL //Previous Name - GLPettyCashRequisitionFormBDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        public static Thread thread;
        #endregion
        #region Methods
        //==================SelectAll=================
        public List<GLPettyCashRequisitionFormBVM> API_SelectAll_GLPettyCashRequisitionFormB(string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<GLPettyCashRequisitionFormBVM> VMs = new List<GLPettyCashRequisitionFormBVM>();
            GLPettyCashRequisitionFormBVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL_PC();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region sql statement
                #region SqlText

                sqlText = @"
SELECT
fb.Id
,fb.GLPettyCashRequisitionId
,fb.TransactionDateTime
,fb.CommissionBillNo
,fb.BranchId
,fb.MRNo
,fb.MRDate
,fb.DocumentNo
,fb.CustomerId
,fb.NetPremium
,ISNULL(CommissionRate,0)CommissionRate
,ISNULL(CommissionAmount,0)CommissionAmount
,ISNULL(AITRate,0)AITRate
,ISNULL(AITAmount,0)AITAmount

,ISNULL(fb.IsCompleted,0)IsCompleted



,fb.PCAmount
,fb.Post
,fb.Remarks
,fb.IsRejected
,fb.RejectedBy
,fb.RejectedDate
,fb.RejectedComments
,fb.DocumentType

,c.Name CustomerName
,c.Code CustomerCode
--,fb.PC
FROM  GLPettyCashRequisitionFormBs  fb
LEFT OUTER JOIN GLCustomers c ON fb.CustomerId = c.Id
LEFT OUTER JOIN GLPettyCashRequisitions m ON m.Id = fb.GLPettyCashRequisitionId
WHERE  1=1

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

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
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
                    vm = new GLPettyCashRequisitionFormBVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.GLPettyCashRequisitionId = Convert.ToInt32(dr["GLPettyCashRequisitionId"]);
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
                    vm.MRNo = dr["MRNo"].ToString();
                    vm.MRDate = Ordinary.StringToDate(dr["MRDate"].ToString());
                    vm.DocumentNo = dr["DocumentNo"].ToString();
                    vm.CustomerId = Convert.ToInt32(dr["CustomerId"]);

                    vm.NetPremium = Convert.ToDecimal(dr["NetPremium"]);
                    vm.CommissionRate = Convert.ToDecimal(dr["CommissionRate"]);
                    vm.CommissionAmount = Convert.ToDecimal(dr["CommissionAmount"]);
                    vm.AITRate = Convert.ToDecimal(dr["AITRate"]);
                    vm.AITAmount = Convert.ToDecimal(dr["AITAmount"]);

                    vm.IsCompleted = Convert.ToBoolean(dr["IsCompleted"]);



                    vm.PCAmount = Convert.ToDecimal(dr["PCAmount"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectedDate = dr["RejectedDate"].ToString();
                    vm.RejectedComments = dr["RejectedComments"].ToString();
                    vm.DocumentType = dr["DocumentType"].ToString();

                    vm.CustomerName = dr["CustomerName"].ToString();
                    vm.CustomerCode = dr["CustomerCode"].ToString();
                    //vm.PC = Convert.ToDecimal(dr["PC"]);

                    VMs.Add(vm);
                }
                dr.Close();

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
            return VMs;
        }

        public List<CommissionBillVM> API_DropDown_GLPettyCashRequisitionFormB(int branchId = 0)
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
                currConn = _dbsqlConnection.GetConnectionSageGL_PC();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT
distinct
 d.CommissionBillNo
 FROM GLPettyCashRequisitionFormBs d
 WHERE  1=1 
AND d.CommissionBillNo is not null
AND d.BranchId=@BranchId
AND d.Post = 1
AND d.IsRejected = 0
AND ISNULL(d.IsCompleted,0) = 0
";
                if (branchId == 0)
                {
                    sqlText = sqlText.Replace("d.BranchId=@BranchId", "1=1");
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                if (branchId > 0)
                {
                    objComm.Parameters.AddWithValue("@BranchId", branchId);
                }


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CommissionBillVM();
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
                    vm.Name = vm.CommissionBillNo;
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



        //==================Update =================
        public string[] API_Update_GLPettyCashRequisitionFormB(List<string> GLMRNos, string IsCompleted)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Update_GLPettyCashRequisitionFormB"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL_PC();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction

                if (GLMRNos != null && GLMRNos.Count() > 0)
                {
                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE GLPettyCashRequisitionFormBs SET";

                    sqlText += " IsCompleted=@IsCompleted";
                    sqlText += " WHERE MRNo=@MRNo";

                    #endregion SqlText
                    #region SqlExecution
                    foreach (var MRNo in GLMRNos)
                    {
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@MRNo", MRNo);
                        cmdUpdate.Parameters.AddWithValue("@IsCompleted", IsCompleted);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLPettyCashRequisitionFormBs.", "");
                        }
                    }

                    #endregion SqlExecution

                    retResults[2] = GLMRNos.FirstOrDefault().ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("GLPettyCashRequisitionFormB Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLPettyCashRequisitionFormB Update", "Could not found any item.");
                }
                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";
                retResults[2] = GLMRNos.FirstOrDefault().ToString();
                #endregion SuccessResult
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = "Data Not Update Successfully.";
                retResults[4] = ex.Message; //catch ex
                return retResults;
            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }


        public List<GLCustomerVM> API_SelectAll_GLCustomer(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLCustomerVM> VMs = new List<GLCustomerVM>();
            GLCustomerVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL_PC();
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
                sqlText = @"
SELECT
 c.Id
,c.Code
,c.Name
,c.MobileNo
,c.Address
,c.City
,c.TelephoneNo
,c.FaxNo
,c.Email
,c.ContactPerson
,c.ContactPersonDesignation
,c.ContactPersonTelephone
,c.ContactPersonEmail
,c.TIN
,c.BIN
,c.Remarks
,c.IsActive
,c.IsArchive
,c.CreatedBy
,c.CreatedAt
,c.CreatedFrom
,c.LastUpdateBy
,c.LastUpdateAt
,c.LastUpdateFrom
From GLCustomers c
Where  c.IsArchive=0
";
                if (Id > 0)
                {
                    sqlText += @" and c.Id=@Id";
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
                sqlText += @" ORDER BY c.Name";

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
                    vm = new GLCustomerVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.MobileNo = dr["MobileNo"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.TelephoneNo = dr["TelephoneNo"].ToString();
                    vm.FaxNo = dr["FaxNo"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.ContactPerson = dr["ContactPerson"].ToString();
                    vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    vm.ContactPersonTelephone = dr["ContactPersonTelephone"].ToString();
                    vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
                    vm.TIN = dr["TIN"].ToString();
                    vm.BIN = dr["BIN"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = dr["CreatedAt"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = dr["LastUpdateAt"].ToString();
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                if (transaction != null)
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


        public DataTable API_SelectAll_Data(string TableName, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL_PC();
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
                sqlText = @" SELECT * FROM " + TableName + " WHERE 1=1";

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

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

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

                if (transaction != null)
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return dt;
        }


        public string API_SelectAll_Data_XML(string TableName, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            string xmlData = "";
            #endregion
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL();
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
                sqlText = @" SELECT * FROM " + TableName + " WHERE 1=1";

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

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

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

                #region Column Change
                if (dt != null && dt.Rows.Count > 0)
                {
                    string columnName = "IsSynced";

                    DataColumnCollection columns = dt.Columns;
                    if (columns.Contains(columnName))
                    {
                        DataColumn col = dt.Columns[columnName];
                        foreach (DataRow dr in dt.Rows)
                        {
                            dr[col] = 1;
                        }
                    }
                }

                #endregion




                DataSet ds = new DataSet();

                ds.Tables.Add(dt);

                xmlData = ds.GetXml();

                if (transaction != null)
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return xmlData;
        }


        public string[] API_Update_Sync(string TableName)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "API_Update_Sync"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction

                #region Update Settings
                sqlText = "";
                sqlText = @" UPDATE " + TableName + " SET IsSynced=1";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);


                retResults[2] = "";// Return Id
                retResults[3] = sqlText; //  SQL Query
                #endregion Update Settings
                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";
                retResults[2] = "";
                #endregion SuccessResult
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = "Data Not Update Successfully.";
                retResults[4] = ex.Message; //catch ex
                return retResults;
            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }



        #endregion
    }
}
