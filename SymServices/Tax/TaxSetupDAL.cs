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
  public class TaxSetupDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
         CommonDAL cdal = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectAll=================
        public List<TaxSetupVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<TaxSetupVM> TaxSetupVMs = new List<TaxSetupVM>();
            TaxSetupVM VM;
            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
Id
,BranchId
,Code
,Name
,isnull(MinimumTax,0)MinimumTax
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From TXSetup 
Where IsArchive=0
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    VM = new TaxSetupVM();
                    VM.Id = dr["Id"].ToString();
                    VM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    VM.Name = dr["Name"].ToString();
                    VM.Code = dr["Code"].ToString();
                    VM.Remarks = dr["Remarks"].ToString();
                    VM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    VM.MinimumTax = Convert.ToDecimal(dr["MinimumTax"]);
                    VM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    VM.CreatedBy = dr["CreatedBy"].ToString();
                    VM.CreatedFrom = dr["CreatedFrom"].ToString();
                    VM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    VM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    VM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    TaxSetupVMs.Add(VM);
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

            return TaxSetupVMs;
        }
        //==================SelectByID=================
        public TaxSetupVM SelectById(string Id)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            TaxSetupVM VM = new TaxSetupVM();
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"SELECT
Id
,BranchId
,Code
,Name
,Remarks
,isnull(MinimumTax,0)MinimumTax
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From TXSetup 
Where IsArchive=0
     
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    VM.Id = dr["Id"].ToString();
                    VM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    VM.Name = dr["Name"].ToString();
                    VM.Code = dr["Code"].ToString();
                    VM.Remarks = dr["Remarks"].ToString();
                    VM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    VM.MinimumTax = Convert.ToDecimal(dr["MinimumTax"]);
                    VM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    VM.CreatedBy = dr["CreatedBy"].ToString();
                    VM.CreatedFrom = dr["CreatedFrom"].ToString();
                    VM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    VM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    VM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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
            return VM;
        }
        //==================Insert =================
        public string[] Insert(TaxSetupVM VM,int BranchId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertTaxSetup"; //Method Name
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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) Code FROM TXSetup ";
                sqlText += " WHERE Code=@Code and BranchId=@BranchId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Code", VM.Code);
                cmdExist.Parameters.AddWithValue("@BranchId", BranchId);
                var exeRes = cmdExist.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes);
                if (objfoundId > 0)
                {
                    retResults[1] = "Code already used!";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Code already used!", "");
                }
                #endregion Exist
                #region Save
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from TXSetup where BranchId=@BranchId";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@BranchId", BranchId);
                cmd2.Transaction = transaction;
				exeRes = cmd2.ExecuteScalar();
				int count = Convert.ToInt32(exeRes);
                VM.Id = BranchId.ToString() + "_" + (count + 1);
                if (true)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO TXSetup(Id,BranchId,Code,Name,MinimumTax,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Id,@BranchId,@Code,@Name,@MinimumTax,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom)";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Parameters.AddWithValue("@Id", VM.Id);
                    cmdExist1.Parameters.AddWithValue("@BranchId", Convert.ToInt32(BranchId));
                    cmdExist1.Parameters.AddWithValue("@Name", VM.Name);
                    cmdExist1.Parameters.AddWithValue("@Code", VM.Code);
                    cmdExist1.Parameters.AddWithValue("@MinimumTax", VM.MinimumTax);
                    cmdExist1.Parameters.AddWithValue("@Remarks", VM.Remarks ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@IsActive", true);
                    cmdExist1.Parameters.AddWithValue("@IsArchive", false);
                    cmdExist1.Parameters.AddWithValue("@CreatedBy", VM.CreatedBy);
                    cmdExist1.Parameters.AddWithValue("@CreatedAt", VM.CreatedAt);
                    cmdExist1.Parameters.AddWithValue("@CreatedFrom", VM.CreatedFrom);
                    cmdExist1.Transaction = transaction;
                    cmdExist1.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This TaxSetup already used";
                    throw new ArgumentNullException("Please Input TaxSetup Value", "");
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
                retResults[2] = VM.Id;
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
        public string[] Update(TaxSetupVM VM,int BranchId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee TaxSetup Update"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;

            string sqlText = "";

            bool iSTransSuccess = false;

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
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                transaction = currConn.BeginTransaction("UpdateToTaxSetup");
                #endregion open connection and transaction
                if (VM != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update TXSetup set";
                    sqlText += " BranchId=@BranchId,";
                    sqlText += " Name=@Name,";
                    sqlText += " Code=@Code,";
                    sqlText += " MinimumTax=@MinimumTax,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", VM.Id);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", BranchId);
                    cmdUpdate.Parameters.AddWithValue("@Code", VM.Code);
                    cmdUpdate.Parameters.AddWithValue("@Name", VM.Name);
                    cmdUpdate.Parameters.AddWithValue("@MinimumTax", VM.MinimumTax);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", VM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", VM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", VM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", VM.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    transResult = Convert.ToInt32(cmdUpdate.ExecuteNonQuery());
                    retResults[2] = VM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("TaxSetup Update", VM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("TaxSetup Update", "Could not found any item.");
                }
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to update TaxSetup.";
                    throw new ArgumentNullException("", "");
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
        //==================Delete =================
        public string[] Delete(TaxSetupVM VM, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteTaxSetup"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            bool iSTransSuccess = false;
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
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                transaction = currConn.BeginTransaction("DeleteToTaxSetup");
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update TXSetup set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", VM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", VM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", VM.LastUpdateFrom);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("TaxSetup Delete", VM.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("TaxSetup Information Delete", "Could not found any item.");
                }


                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to delete TaxSetup Information.";
                    throw new ArgumentNullException("", "");
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
        public List<TaxSetupVM> DropDown(int branch)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<TaxSetupVM> VMs = new List<TaxSetupVM>();
            TaxSetupVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
Id,
Name
   FROM TXSetup
WHERE IsArchive=0 and IsActive=1 and BranchId=@branch
    ORDER BY Name
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@branch", branch);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new TaxSetupVM();
                    vm.Id = dr["Id"].ToString();
                  
                    VMs.Add(vm);
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
        public List<string> Autocomplete(string term)
        {
            #region Variables
            SqlConnection currConn = null;
            List<string> vms = new List<string>();
            string sqlText = "";
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = "";
                sqlText = @"SELECT Id, Name    FROM TXSetup ";
                sqlText += @" WHERE Name like '%" + term + "%' and IsArchive=0 and IsActive=1 ORDER BY Name";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["Name"].ToString());
                    i++;
                }
                vms.Sort();
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
            return vms;
        }
        #endregion
    }
}
