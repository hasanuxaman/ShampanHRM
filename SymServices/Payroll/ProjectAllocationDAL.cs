using SymOrdinary;
using SymServices.Common;
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
  public class ProjectAllocationDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        ProjectAllocationDetailDAL pdal = new ProjectAllocationDetailDAL();
        CommonDAL cdal = new CommonDAL();
        #endregion
        #region Methods
        public List<ProjectAllocationVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<ProjectAllocationVM> VMs = new List<ProjectAllocationVM>();
            ProjectAllocationVM vm;
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
   FROM ProjectAllocation
WHERE IsArchive=0 and IsActive=1
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
                    vm = new ProjectAllocationVM();
                    vm.Id = dr["Id"].ToString();
           
                    vm.Name = dr["Name"].ToString();
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
        //==================SelectAll ProjectAllocations=================
        public List<ProjectAllocationVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<ProjectAllocationVM> vms = new List<ProjectAllocationVM>();
            ProjectAllocationVM vm;
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
Name,
Code,BranchId
    FROM ProjectAllocation
WHERE IsArchive=0 
    ORDER BY Name";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                   vm = new ProjectAllocationVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.BranchId = dr["BranchId"].ToString();
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
        public ProjectAllocationVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            ProjectAllocationVM ProjectAllocation = new ProjectAllocationVM(); ;
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
Name,
Code,
BranchId
FROM ProjectAllocation
where id=@Id and IsArchive=0
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
                    ProjectAllocation = new ProjectAllocationVM();
                    ProjectAllocation.Id = dr["Id"].ToString();
                    ProjectAllocation.Name = dr["Name"].ToString();
                    ProjectAllocation.Code = dr["Code"].ToString();
                    ProjectAllocation.BranchId = dr["BranchId"].ToString();

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

            return ProjectAllocation;
        }
        //==================Insert ProjectAllocationes=================
        public string[] Insert(ProjectAllocationVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "ProjectAllocationDataInsert"; //Method Name
            int transResult = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(vm.Code))
                {
                    retResults[1] = "Please Input ProjectAllocation Code";
                    return retResults;
                }
                else if (string.IsNullOrEmpty(vm.Name))
                {
                    retResults[1] = "Please Input ProjectAllocation Name";
                    return retResults;
                }
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
                bool check = false;
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code,vm.Name };
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsert("ProjectAllocation", fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = fieldName[i] + " already used!";
                        throw new ArgumentNullException(fieldName[i] + " already used!", fieldName[i] + " already used!");
                    }
                }
                #endregion Exist
             
                #region Save

                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from ProjectAllocation where BranchId=@BranchId";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);
                vm.Id = vm.BranchId.ToString() + "_" + (count + 1);
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += " INSERT INTO ProjectAllocation(Id,Code,Name,BranchId,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)";
                    sqlText += " VALUES (@Id,@Code,@Name,@BranchId,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom )";
                    SqlCommand _cmdInsert = new SqlCommand(sqlText, currConn);
                    _cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    _cmdInsert.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    _cmdInsert.Parameters.AddWithValue("@Name", vm.Name.Trim());
                    _cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    _cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    _cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    _cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    _cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    _cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    _cmdInsert.Transaction = transaction;
                    var exeRes1 = _cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes1);
                    if (transResult <= 0)
                    {
                        retResults[1] = "Please Input ProjectAllocation Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input ProjectAllocation Value", "");
                    }
                    if (vm.ProjectAllocationDetailVM != null && vm.ProjectAllocationDetailVM.Count() > 0)
                    {
                        foreach (var Detail in vm.ProjectAllocationDetailVM)
                        {
                            if (Detail.GLCode1 != null && Detail.GLCode2 != null && Detail.GLCode3 != null && Detail.GLCode4 != null)
                            {
                                Detail.ProjectAllocationId = vm.Id;
                                Detail.CreatedAt = vm.CreatedAt;
                                Detail.CreatedBy = vm.CreatedBy;
                                Detail.CreatedFrom = vm.CreatedFrom;
                                retResults = pdal.Insert(Detail, currConn, transaction);
                                if (retResults[0] == "Fail")
                                {
                                    throw new ArgumentNullException("ProjectAllocation Details", "could not updated.");
                                }
                            }
                        }
                    }
                }
                else
                {
                    retResults[1] = "This ProjectAllocation already used";
                    throw new ArgumentNullException("Please Input ProjectAllocation Value", "");
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
                retResults[1] = "Data Save Successfully";
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
        //==================Update ProjectAllocation=================
        public string[] Update(ProjectAllocationVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ProjectAllocationUpdate"; //Method Name

            int transResult = 0;

            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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

                transaction = currConn.BeginTransaction("UpdateToProjectAllocation");

                #endregion open connection and transaction
                #region Exist
                bool check = false;
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code,vm.Name };
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInUpdate(vm.Id, "ProjectAllocation", fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = fieldName[i] + " already used!";
                        throw new ArgumentNullException(fieldName[i] + " already used!", "");
                    }
                }
                #endregion Exist

                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update ProjectAllocation set";
                    sqlText += " Code=@Code,";
                    sqlText += " Name=@Name,";
                    sqlText += " BranchId=@BranchId";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name.Trim());
                    cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);

                    cmdUpdate.Transaction = transaction;
                   

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        retResults[1] = vm.Name + " could not updated.";// Return Id

                        throw new ArgumentNullException("ProjectAllocationUpdate", vm.Name + " could not updated.");
                    }
                    #region insert Details into Master Table
                    if (vm.ProjectAllocationDetailVM != null && vm.ProjectAllocationDetailVM.Count() > 0)
                    {
                        #region Delete
                        try
                        {
                            CommonDAL cDal = new CommonDAL();
                            retResults = cDal.DeleteTableInformation(vm.Id, "ProjectAllocationDetail", "ProjectAllocationId", currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                //throw new ArgumentNullException("Inventory Receive Details", "could not updated.");
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentNullException("ProjectAllocation Details", "could not updated.");
                        }
                        #endregion Delete
                        foreach (var Detail in vm.ProjectAllocationDetailVM)
                        {
                            if (Detail.GLCode1 != null && Detail.GLCode2 != null && Detail.GLCode3 != null && Detail.GLCode4 != null)
                            {
                                Detail.ProjectAllocationId = vm.Id;
                                Detail.CreatedAt = vm.LastUpdateAt;
                                Detail.CreatedBy = vm.LastUpdateBy;
                                Detail.CreatedFrom = vm.LastUpdateFrom;
                                retResults = pdal.Insert(Detail, currConn, transaction);
                                if (retResults[0] == "Fail")
                                {
                                    throw new ArgumentNullException("ProjectAllocation Details", "could not updated.");
                                }
                            }
                        }
                    }
                  
                    retResults[2] = vm.Id.ToString();// Return Id

                    #endregion Update Settings
                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    retResults[1] = " Could not found any item.";
                    throw new ArgumentNullException("ProjectAllocationUpdate", "Could not found any item.");
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
                    retResults[1] = "Requested ProjectAllocation Information Successfully Updated.";

                }
                else
                {
                    retResults[1] = "Unexpected error to update ProjectAllocation.";
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
        //==================Select ProjectAllocation=================
        public ProjectAllocationVM SelectProjectAllocation(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            ProjectAllocationVM vm = new ProjectAllocationVM();

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
                sqlText = @"SELECT Top 1 Id,Code, Name, BranchId FROM ProjectAllocations  ";
                if (query == null)
                {
                    if (Id != 0)
                    {
                        sqlText += " AND Id=@Id";
                    }
                    else
                    {
                        sqlText += " ORDER BY Id ";
                    }
                }
                else
                {
                    if (query == "FIRST")
                    {
                        sqlText += " ORDER BY Id ";
                    }
                    else if (query == "LAST")
                    {
                        sqlText += " ORDER BY Id DESC";
                    }
                    else if (query == "NEXT")
                    {
                        sqlText += " and  Id > @Id   ORDER BY Id";
                    }
                    else if (query == "PREVIOUS")
                    {
                        sqlText += "  and  Id < @Id   ORDER BY Id DESC";
                    }
                }


                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (Id != null)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                try
                {
                    dr = objComm.ExecuteReader();
                    while (dr.Read())
                    {
                        vm.Id = dr["Id"].ToString();
                        vm.Name = dr["Name"].ToString();
                       vm.Code = dr["Code"].ToString();
                       vm.BranchId = dr["BranchId"].ToString();
                    }
                    dr.Close();
                }
                catch (Exception ex)
                {
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

            return vm;
        }
        //==================Delete ProjectAllocation=================
        public string[] Delete(ProjectAllocationVM vm, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Project Allocation Delete"; //Method Name

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

                transaction = currConn.BeginTransaction("Delete");

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length > 0)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update ProjectAllocation set";
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
                            throw new ArgumentNullException("Unexpected error to update ProjectAllocation.", "");
                        }

                    }
                }
                else
                {
                    retResults[1] = "Could not found any item.";
                    throw new ArgumentNullException("Indents Information Delete", "Could not found any item.");
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
                        retResults[1] = "Unexpected error to update Indent.";
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
