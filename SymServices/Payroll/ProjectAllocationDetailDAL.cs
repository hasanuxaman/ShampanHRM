using SymOrdinary;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Payroll
{
   public class ProjectAllocationDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        public List<ProjectAllocationDetailVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<ProjectAllocationDetailVM> vms = new List<ProjectAllocationDetailVM>();
            ProjectAllocationDetailVM vm;
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
pad.Id
,pad.ProjectAllocationId
, pa.Code Code
, pa.Name Name
,pad.HeadName
,pad.GLCode1
,pad.Portion1
,pad.GLCode2
,pad.Portion2
,pad.GLCode3
,pad.Portion3
,pad.GLCode4
,pad.Portion4
,pad.IsActive
,pad.IsArchive
,pad.CreatedBy
,pad.CreatedAt
,pad.CreatedFrom
,pad.LastUpdateBy
,pad.LastUpdateAt
,pad.LastUpdateFrom
from ProjectAllocationDetail pad
left outer join ProjectAllocation pa on  pad.ProjectAllocationId=pa.Id
WHERE pad.IsArchive=0 
ORDER BY Name
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProjectAllocationDetailVM();
vm.Id=dr["Id"].ToString();
vm.HeadName=dr["HeadName"].ToString();
vm.GLCode1=dr["GLCode1"].ToString();
vm.Portion1=Convert.ToDecimal(dr["Portion1"].ToString());
vm.GLCode2=dr["GLCode2"].ToString();
vm.Portion2 = Convert.ToDecimal(dr["Portion2"].ToString());
vm.GLCode3=dr["GLCode3"].ToString();
vm.Portion3 = Convert.ToDecimal(dr["Portion3"].ToString());
vm.GLCode4=dr["GLCode4"].ToString();
vm.Portion4 = Convert.ToDecimal(dr["Portion4"].ToString());
vm.CreatedBy = dr["CreatedBy"].ToString();
vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
vm.CreatedFrom = dr["CreatedFrom"].ToString();
vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vms.Add(vm);
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

            return vms;
        }
        public ProjectAllocationDetailVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            ProjectAllocationDetailVM vm = new ProjectAllocationDetailVM(); ;
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
 pad.Id
,pad.ProjectAllocationId
,pad.HeadName
,pad.GLCode1
,pad.Portion1
,pad.GLCode2
,pad.Portion2
,pad.GLCode3
,pad.Portion3
,pad.GLCode4
,pad.Portion4
,pad.IsActive
,pad.IsArchive
,pad.CreatedBy
,pad.CreatedAt
,pad.CreatedFrom
,pad.LastUpdateBy
,pad.LastUpdateAt
,pad.LastUpdateFrom
from ProjectAllocationDetail pad
left outer join ProjectAllocation pa on  pad.ProjectAllocationId=pa.Id
WHERE pad.IsArchive=0 and pad.Id=@Id
ORDER BY Name
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
                    vm = new ProjectAllocationDetailVM();
                    vm.Id = dr["Id"].ToString();
                    vm.HeadName = dr["HeadName"].ToString();
                    vm.GLCode1 = dr["GLCode1"].ToString();
                    vm.Portion1 = Convert.ToDecimal(dr["Portion1"].ToString());
                    vm.GLCode2 = dr["GLCode2"].ToString();
                    vm.Portion2 = Convert.ToDecimal(dr["Portion2"].ToString());
                    vm.GLCode3 = dr["GLCode3"].ToString();
                    vm.Portion4 = Convert.ToDecimal(dr["Portion3"].ToString());
                    vm.GLCode4 = dr["GLCode4"].ToString();
                    vm.Portion4 = Convert.ToDecimal(dr["Portion4"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        public List<ProjectAllocationDetailVM> SelectByMasterId(string MasterId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            ProjectAllocationDetailVM vm = new ProjectAllocationDetailVM();
            List<ProjectAllocationDetailVM> vms = new List<ProjectAllocationDetailVM>();
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
pad.Id
,pad.ProjectAllocationId
,pad.HeadName
,pad.GLCode1
,pad.Portion1
,pad.GLCode2
,pad.Portion2
,pad.GLCode3
,pad.Portion3
,pad.GLCode4
,pad.Portion4
,pad.IsActive
,pad.IsArchive
,pad.CreatedBy
,pad.CreatedAt
,pad.CreatedFrom
,pad.LastUpdateBy
,pad.LastUpdateAt
,pad.LastUpdateFrom
from ProjectAllocationDetail pad
WHERE pad.IsArchive=0 and pad.ProjectAllocationId=@MasterId order by pad.Id";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@MasterId", MasterId);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProjectAllocationDetailVM();
                    vm.Id = dr["Id"].ToString();

                    vm.HeadName = dr["HeadName"].ToString();
                    vm.GLCode1 = dr["GLCode1"].ToString();
                    vm.Portion1 = Convert.ToDecimal(dr["Portion1"].ToString());
                    vm.GLCode2 = dr["GLCode2"].ToString();
                    vm.Portion2 = Convert.ToDecimal(dr["Portion2"].ToString());
                    vm.GLCode3 = dr["GLCode3"].ToString();
                    vm.Portion3 = Convert.ToDecimal(dr["Portion3"].ToString());
                    vm.GLCode4 = dr["GLCode4"].ToString();
                    vm.Portion4 = Convert.ToDecimal(dr["Portion4"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vms.Add(vm);
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

            return vms;
        }
        public string[] Insert(ProjectAllocationDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Insert ProjectAllocationDetail"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;

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


                #region Save
                if (vm != null)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO ProjectAllocationDetail(

  ProjectAllocationId
,HeadName
 ,GLCode1
 ,Portion1
 ,GLCode2
 ,Portion2
 ,GLCode3
 ,Portion3
 ,GLCode4
 ,Portion4
 ,IsActive
 ,IsArchive
 ,CreatedBy
 ,CreatedAt
 ,CreatedFrom
) VALUES (
@ProjectAllocationId
,@HeadName
,@GLCode1
,@Portion1
,@GLCode2
,@Portion2
,@GLCode3
,@Portion3
,@GLCode4
,@Portion4
 ,@IsActive
 ,@IsArchive
 ,@CreatedBy
 ,@CreatedAt
 ,@CreatedFrom
)";

                    SqlCommand _cmdInsert = new SqlCommand(sqlText, currConn);

                    _cmdInsert.Parameters.AddWithValue("@ProjectAllocationId", vm.ProjectAllocationId);
                    _cmdInsert.Parameters.AddWithValue("@HeadName", vm.HeadName);
                
                    _cmdInsert.Parameters.AddWithValue("@GLCode1", vm.GLCode1 );
                    _cmdInsert.Parameters.AddWithValue("@Portion1", vm.Portion1);
                    _cmdInsert.Parameters.AddWithValue("@GLCode2", vm.GLCode2);
                    _cmdInsert.Parameters.AddWithValue("@Portion2", vm.Portion2);
                    _cmdInsert.Parameters.AddWithValue("@GLCode3", vm.GLCode3);
                    _cmdInsert.Parameters.AddWithValue("@Portion3", vm.Portion3);
                    _cmdInsert.Parameters.AddWithValue("@GLCode4", vm.GLCode4);
                    _cmdInsert.Parameters.AddWithValue("@Portion4", vm.Portion4);

                    _cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    _cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    _cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    _cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    _cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);



                    _cmdInsert.Transaction = transaction;
                    var exeRes = _cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Indent Detail .", "");
                    }

                }
                else
                {
                    retResults[1] = "Please Input Indent Detail Values.";
                    throw new ArgumentNullException("Please Input Indent Detail Values.", "");
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
                
                #endregion SuccessResult
            }

            #endregion try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction != null)
                {
                    try
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                    catch (Exception)
                    {

                        retResults[1] = "Unexpected error to update ProjectAllocationDetail.";

                        return retResults;
                        // throw new ArgumentNullException("Unexpected error to update EmployeeInfo.", "EmployeeInfo");
                    }
                } return retResults;
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
        public string[] Update(ProjectAllocationDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Indent Detail"; //Method Name

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
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                transaction = currConn.BeginTransaction("Update");

                #endregion open connection and transaction


                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update ProjectAllocationDetail set";
  
                     sqlText += "ProjectAllocationId";
                     sqlText += "HeadName";
                     sqlText += "GLCode1";
                     sqlText += "Portion1";
                     sqlText += "GLCode2";
                     sqlText += "Portion2";
                     sqlText += "GLCode3";
                     sqlText += "Portion3";
                     sqlText += "GLCode4";
                     sqlText += "Portion4";
                     sqlText += "IsActive";
                     sqlText += "IsArchive";
                     sqlText += "LastUpdateBy";
                     sqlText += "LastUpdateAt";
                     sqlText += "LastUpdateFrom";

                    sqlText += " where Id=@Id";

                    SqlCommand _cmdUpdate = new SqlCommand(sqlText, currConn);

                    _cmdUpdate.Parameters.AddWithValue("@ProjectAllocationId",vm.ProjectAllocationId);
                   
                    _cmdUpdate.Parameters.AddWithValue("@HeadName",vm.HeadName);
                    _cmdUpdate.Parameters.AddWithValue("@GLCode1",vm.GLCode1);
                    _cmdUpdate.Parameters.AddWithValue("@Portion1",vm.Portion1);
                    _cmdUpdate.Parameters.AddWithValue("@GLCode2",vm.GLCode2);
                    _cmdUpdate.Parameters.AddWithValue("@Portion2",vm.Portion2);
                    _cmdUpdate.Parameters.AddWithValue("@GLCode3",vm.GLCode3);
                    _cmdUpdate.Parameters.AddWithValue("@Portion3",vm.Portion3);
                    _cmdUpdate.Parameters.AddWithValue("@GLCode4",vm.GLCode4);
                    _cmdUpdate.Parameters.AddWithValue("@Portion4",vm.Portion4);
                    _cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    _cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    _cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    _cmdUpdate.Transaction = transaction;
                    var exeRes = _cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update ProjectAllocation Detail.", "");
                    }
                }
                else
                {
                    throw new ArgumentNullException(" Detail Update", "Could not found any item.");
                }
                    #endregion Update Settings
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion Commit
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction != null)
                {
                    try
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                    catch (Exception)
                    {

                        retResults[1] = "Unexpected error to update ProjectAllocationDetail.";

                        return retResults;
                        // throw new ArgumentNullException("Unexpected error to update EmployeeInfo.", "EmployeeInfo");
                    }
                } return retResults;
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
        public string[] Delete(ProjectAllocationDetailVM vm, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Indent Detail Delete"; //Method Name

            int transResult = 0;
            int countId = 0;
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
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                transaction = currConn.BeginTransaction("Delete");

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length > 0)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update ProjectAllocationDetails set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand _cmdUpdate = new SqlCommand(sqlText, currConn);
                        _cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        _cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        _cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        _cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        _cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                        _cmdUpdate.Transaction = transaction;
                        var exeRes = _cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update Detail.", "");
                        }

                    }


                }
                else
                {
                    retResults[1] = "Could not found any item.";
                    throw new ArgumentNullException(" Detail Information Delete", "Could not found any item.");
                }
                    #endregion Update Settings

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Data Delete Successfully.";
                retResults[2] = vm.Id.ToString();

            }
                #endregion Commit
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction != null)
                {
                    try
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                    catch (Exception)
                    {

                        retResults[1] = "Unexpected error to update ProjectAllocationDetail.";

                        return retResults;
                        // throw new ArgumentNullException("Unexpected error to update EmployeeInfo.", "EmployeeInfo");
                    }
                } return retResults;
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
        #endregion
    }
}
