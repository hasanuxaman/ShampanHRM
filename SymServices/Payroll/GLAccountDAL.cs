using SymOrdinary;
using SymServices.Common;
using SymViewModel.Enum;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SymServices.Payroll
{
    public class GLAccountDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        public List<GLAccountVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<GLAccountVM> VMs = new List<GLAccountVM>();
            GLAccountVM vm;
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

                sqlText = @"
SELECT
Id,
GLAccountCode 
   FROM GLAccount
WHERE IsArchive=0 and IsActive=1
    ORDER BY GLAccountCode
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLAccountVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["GLAccountCode"].ToString();
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
                sqlText = @"SELECT Id, GLAccountCode    FROM GLAccount ";
                sqlText += @" WHERE GLAccountCode like '%" + term + "%' and IsArchive=0 and IsActive=1 ORDER BY GLAccountCode";



                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["GLAccountCode"].ToString());
                    i++;
                }
                dr.Close();
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
        public List<GLAccountVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<GLAccountVM> VMs = new List<GLAccountVM>();
            GLAccountVM vm;
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
gl.Id
,gl.GLAccountCode
,isnull(gl.ProjectId,0)ProjectId
,isnull(p.Name,'NA') Project
,isnull(gl.VoucherType,0)VoucherType

,gl.Description
,gl.Remarks
,gl.IsActive
,isnull(gl.OutstandingLiabilities,0)OutstandingLiabilities
   FROM GLAccount gl
left outer join Project p on  gl.ProjectId=p.id
WHERE gl.IsArchive=0 
    ORDER BY gl.GLAccountCode
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLAccountVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.GLAccountCode = dr["GLAccountCode"].ToString();
                    vm.GLAccountCode = dr["GLAccountCode"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Description = dr["Description"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.VoucherType = dr["VoucherType"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.OutstandingLiabilities = Convert.ToBoolean(dr["OutstandingLiabilities"]);
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

        public List<GLAccountVM> SelectAll(string ProjectId = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLAccountVM> VMs = new List<GLAccountVM>();
            GLAccountVM vm;
            #endregion
            try
            {
                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT
gl.Id
,gl.GLAccountCode
,isnull(gl.ProjectId,0)ProjectId
,isnull(p.Name,'NA') Project
,isnull(gl.VoucherType,0)VoucherType

,gl.Description
,gl.Remarks
,isnull(gl.GLAccountType,'NA')GLAccountType
,gl.IsActive
,isnull(gl.OutstandingLiabilities,0)OutstandingLiabilities
   FROM GLAccount gl
left outer join Project p on  gl.ProjectId=p.id
WHERE gl.IsArchive=0 
     
";
                if (!string.IsNullOrWhiteSpace(ProjectId))
                    sqlText += @" and  gl.ProjectId=@ProjectId";

                sqlText += @"   ORDER BY projectId,VoucherType,OutstandingLiabilities, gl.GLAccountCode";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (!string.IsNullOrWhiteSpace(ProjectId))
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);


                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new GLAccountVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.GLAccountCode = dr["GLAccountCode"].ToString();
                        vm.ProjectId = dr["ProjectId"].ToString();
                        vm.Project = dr["Project"].ToString();
                        vm.VoucherType = dr["VoucherType"].ToString();
                        vm.Description = dr["Description"].ToString();
                        vm.Remarks = dr["Remarks"].ToString();
                        vm.GLAccountType = dr["GLAccountType"].ToString();
                        vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        vm.OutstandingLiabilities = Convert.ToBoolean(dr["OutstandingLiabilities"]);
                        VMs.Add(vm);
                    }
                    dr.Close();
                }

                #endregion
            }
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
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

            return VMs;
        }


        public GLAccountVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            GLAccountVM vm = new GLAccountVM(); ;
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

                sqlText = @"
SELECT
gl.Id
,gl.GLAccountCode
,isnull(gl.ProjectId,0)ProjectId
,isnull(p.Name,'NA') Project
,isnull(gl.VoucherType,0)VoucherType

,gl.Description
,gl.Remarks
,gl.IsActive
,isnull(gl.OutstandingLiabilities,0)OutstandingLiabilities
   FROM GLAccount gl
left outer join Project p on  gl.ProjectId=p.id
WHERE gl.Id=@Id 
    ORDER BY gl.GLAccountCode
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@Id", Id);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLAccountVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.GLAccountCode = dr["GLAccountCode"].ToString();
                    vm.Description = dr["Description"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.VoucherType = dr["VoucherType"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.OutstandingLiabilities = Convert.ToBoolean(dr["OutstandingLiabilities"]);

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

            return vm;
        }
        public string[] Insert(GLAccountVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertGLAccount"; //Method Name
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
                    transaction = currConn.BeginTransaction("Insert");
                }

                #endregion open connection and transaction

                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "GLAccount";
                string[] fieldName = { "GLAccountCode" };
                string[] fieldValue = { vm.GLAccountCode.Trim() };

                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                #endregion Exist

                #region Save
                if (vm != null)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO GLAccount(

 GLAccountCode
,Description
,ProjectId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,OutstandingLiabilities
,VoucherType
) VALUES (
@GLAccountCode
,@Description
,@ProjectId
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@OutstandingLiabilities
,@VoucherType
)SELECT SCOPE_IDENTITY()";

                    SqlCommand _cmdExist = new SqlCommand(sqlText, currConn);
                    _cmdExist.Parameters.AddWithValue("@ProjectId", vm.ProjectId.Trim());
                    _cmdExist.Parameters.AddWithValue("@VoucherType", vm.VoucherType.Trim());
                    _cmdExist.Parameters.AddWithValue("@GLAccountCode", vm.GLAccountCode.Trim());
                    _cmdExist.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);//, vm.Remarks?? Convert.DBNull);//, vm.Remarks?? Convert.DBNull);
                    _cmdExist.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, vm.Remarks?? Convert.DBNull);//, vm.Remarks?? Convert.DBNull);
                    _cmdExist.Parameters.AddWithValue("@IsActive", true);
                    _cmdExist.Parameters.AddWithValue("@IsArchive", false);
                    _cmdExist.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    _cmdExist.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    _cmdExist.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    _cmdExist.Parameters.AddWithValue("@OutstandingLiabilities", vm.OutstandingLiabilities);
                    _cmdExist.Transaction = transaction;
                    var exeRes = _cmdExist.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input GLAccount Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input GLAccount Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This GLAccount already used";
                    throw new ArgumentNullException("Please Input GLAccount Value", "");
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
                retResults[2] = Id.ToString();
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
        public string[] Update(GLAccountVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLAccount Update"; //Method Name
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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Exist

                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "GLAccount";
                string[] fieldName = { "GLAccountCode" };
                string[] fieldValue = { vm.GLAccountCode.Trim() };

                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInUpdate(vm.Id.ToString(), tableName, fieldName[i], fieldValue[i], currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                #endregion Exist

                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update GLAccount set";
                    sqlText += " GLAccountCode=@GLAccountCode";
                    sqlText += " ,ProjectId=@ProjectId";
                    sqlText += " ,Description=@Description";
                    sqlText += " ,Remarks=@Remarks";
                    sqlText += " ,IsActive=@IsActive";

                    sqlText += " ,VoucherType=@VoucherType";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " ,OutstandingLiabilities=@OutstandingLiabilities";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@GLAccountCode", vm.GLAccountCode.Trim());
                    cmdUpdate.Parameters.AddWithValue("@VoucherType", vm.VoucherType.Trim());
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);//, vm.Remarks?? Convert.DBNull);//, vm.Remarks?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, vm.Remarks?? Convert.DBNull);//, vm.Remarks?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@OutstandingLiabilities", vm.OutstandingLiabilities);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", ProjectVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("GLAccount Update", "Could not found any item.");
                }


                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to update GLAccount.";
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return retResults;
        }
        public string[] Delete(GLAccountVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLAccount Delete"; //Method Name

            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            bool iSTransSuccess = false;

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
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length > 0)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update GLAccount set";
                        sqlText += " IsActive=@IsActive";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " ,LastUpdateBy=@LastUpdateBy";
                        sqlText += " ,LastUpdateAt=@LastUpdateAt";
                        sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

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
                        throw new ArgumentNullException("GLAccount Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("GLAccount Information Delete", "Could not found any item.");
                }


                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to delete Project Information.";
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
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
