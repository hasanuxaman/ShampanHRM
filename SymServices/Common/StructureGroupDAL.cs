using SymOrdinary;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.Common
{
    public class StructureGroupDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<StructureGroupVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<StructureGroupVM> structureGroupVMs = new List<StructureGroupVM>();
            StructureGroupVM structureGroupVM;
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
*
 
    From StructureGroup  
Where IsArchive=0
    ORDER BY id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    structureGroupVM = new StructureGroupVM();
                    structureGroupVM.Id = dr["Id"].ToString();
                    structureGroupVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    structureGroupVM.Name = dr["Name"].ToString();
                    structureGroupVM.LeaveStructureId = dr["LeaveStructureId"].ToString();
                    structureGroupVM.SalaryStructureId = dr["SalaryStructureId"].ToString();
                    structureGroupVM.EmployeeGroupId = dr["EmployeeGroupId"].ToString();
                    structureGroupVM.PFStructureId = dr["PFStructureId"].ToString();
                    structureGroupVM.TaxStructureId = dr["TaxStructureId"].ToString();
                    structureGroupVM.BonusStructureId = dr["BonusStructureId"].ToString();
                    structureGroupVM.Remarks = dr["Remarks"].ToString();
                    structureGroupVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    structureGroupVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    structureGroupVM.CreatedBy = dr["CreatedBy"].ToString();
                    structureGroupVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    structureGroupVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    structureGroupVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    structureGroupVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    structureGroupVMs.Add(structureGroupVM);
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

            return structureGroupVMs;
        }
        //==================SelectByID=================
        public StructureGroupVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            StructureGroupVM structureGroupVM = new StructureGroupVM();

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
*
    From StructureGroup
where  id=@Id
     
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
                    structureGroupVM.Id = dr["Id"].ToString();
                    structureGroupVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    structureGroupVM.LeaveStructureId = dr["LeaveStructureId"].ToString();
                    structureGroupVM.SalaryStructureId = dr["SalaryStructureId"].ToString();
                    structureGroupVM.EmployeeGroupId = dr["EmployeeGroupId"].ToString();
                    structureGroupVM.PFStructureId = dr["PFStructureId"].ToString();
                    structureGroupVM.TaxStructureId = dr["TaxStructureId"].ToString();
                    structureGroupVM.BonusStructureId = dr["BonusStructureId"].ToString();
                    structureGroupVM.Name = dr["Name"].ToString();
                    structureGroupVM.Remarks = dr["Remarks"].ToString();
                    structureGroupVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    structureGroupVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    structureGroupVM.CreatedBy = dr["CreatedBy"].ToString();
                    structureGroupVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    structureGroupVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    structureGroupVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    structureGroupVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return structureGroupVM;
        }
        //==================Insert =================
        public string[] Insert(StructureGroupVM structureGroupVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertStructureGroup"; //Method Name


            #endregion

            SqlConnection currConn = null;
            SqlTransaction transaction = null;


            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(StructureGroupVM.DepartmentId))
                //{
                //    retResults[1] = "Please Input Employee Travel Course";
                //    return retResults;
                //}
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

               
                #region Save
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from StructureGroup where BranchId=@BranchId";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@BranchId", structureGroupVM.BranchId);
                cmd2.Transaction = transaction;
				var exeRes = cmd2.ExecuteScalar();
				int count = Convert.ToInt32(exeRes);
                if (count>0)
                {
                    retResults[1] = "This StructureGroup already used";
                    throw new ArgumentNullException("This StructureGroup already used", ""); 
                }
                else
                {
                structureGroupVM.Id = structureGroupVM.BranchId.ToString() + "_" + (count + 1);

                    sqlText = "  ";
                    sqlText += @" INSERT INTO StructureGroup(Id,BranchId,Name,LeaveStructureId,SalaryStructureId,EmployeeGroupId
,PFStructureId
,TaxStructureId
,BonusStructureId
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Id,@BranchId,@Name,@LeaveStructureId,@SalaryStructureId,@AttendanceStructureId
,@PFStructureId
,@TaxStructureId
,@BonusStructureId
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        ";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@Id", structureGroupVM.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", structureGroupVM.BranchId);
                    cmdInsert.Parameters.AddWithValue("@Name", structureGroupVM.Name);
                    cmdInsert.Parameters.AddWithValue("@LeaveStructureId", structureGroupVM.LeaveStructureId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@SalaryStructureId", structureGroupVM.SalaryStructureId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@EmployeeGroupId", structureGroupVM.EmployeeGroupId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PFStructureId", structureGroupVM.PFStructureId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TaxStructureId", structureGroupVM.TaxStructureId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@BonusStructureId", structureGroupVM.BonusStructureId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", structureGroupVM.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", structureGroupVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", structureGroupVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", structureGroupVM.CreatedFrom);

                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();

                    //if (Id <= 0)
                    //{
                    //    retResults[1] = "Please Input StructureGroup Value";
                    //    retResults[3] = sqlText;
                    //    throw new ArgumentNullException("Please Input StructureGroup Value", "");
                    //}
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
                retResults[2] = structureGroupVM.Id;

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
        public string[] Update(StructureGroupVM structureGroupVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee StructureGroup Update"; //Method Name

            int transResult = 0;

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToStructureGroup"); }

                #endregion open connection and transaction
              

                if (structureGroupVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update StructureGroup set";
                    sqlText += " BranchId=@BranchId,";
                    sqlText += " Name=@Name,";
                    sqlText += " LeaveStructureId=@LeaveStructureId,";
                    sqlText += " SalaryStructureId=@SalaryStructureId,";
                    sqlText += " EmployeeGroupId=@EmployeeGroupId,";
                    sqlText += " PFStructureId=@PFStructureId,";
                    sqlText += " TaxStructureId=@TaxStructureId,";
                    sqlText += " BonusStructureId=@BonusStructureId,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", structureGroupVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", structureGroupVM.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@LeaveStructureId", structureGroupVM.LeaveStructureId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@SalaryStructureId", structureGroupVM.SalaryStructureId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeGroupId", structureGroupVM.EmployeeGroupId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PFStructureId", structureGroupVM.PFStructureId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TaxStructureId", structureGroupVM.TaxStructureId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BonusStructureId", structureGroupVM.BonusStructureId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Name", structureGroupVM.Name);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", structureGroupVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", structureGroupVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", structureGroupVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", structureGroupVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = structureGroupVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", StructureGroupVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("StructureGroup Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update StructureGroup.";
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
        //==================Select =================
        public StructureGroupVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            StructureGroupVM StructureGroupVM = new StructureGroupVM();

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
                sqlText = @"SELECT Top 1 
Id
,BranchId
,Name
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From StructureGroup 
";
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
                        StructureGroupVM.Id = dr["Id"].ToString();
                        StructureGroupVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                        StructureGroupVM.Name = dr["Name"].ToString();
                        StructureGroupVM.Remarks = dr["Remarks"].ToString();
                        StructureGroupVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        StructureGroupVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                        StructureGroupVM.CreatedBy = dr["CreatedBy"].ToString();
                        StructureGroupVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        StructureGroupVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        StructureGroupVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        StructureGroupVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return StructureGroupVM;
        }
        //==================Delete =================
        public string[] Delete(StructureGroupVM structureGroupVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteStructureGroup"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToStructureGroup"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length > 0)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update StructureGroup set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", structureGroupVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", structureGroupVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", structureGroupVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("StructureGroup Delete", structureGroupVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("StructureGroup Information Delete", "Could not found any item.");
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
                    retResults[1] = "Requested StructureGroup Information Successfully Deleted.";

                }
                else
                {
                    retResults[1] = "Unexpected error to delete StructureGroup Information.";
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

        public List<StructureGroupVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<StructureGroupVM> VMs = new List<StructureGroupVM>();
            StructureGroupVM vm;
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
   FROM StructureGroup
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
                    vm = new StructureGroupVM();
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
                sqlText = @"SELECT Id, Name    FROM StructureGroup ";
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

        #endregion
    }
}
