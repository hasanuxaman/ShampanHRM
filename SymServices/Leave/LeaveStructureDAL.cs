using SymOrdinary;
using SymViewModel.HRM;
using SymViewModel.Leave;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.HRM
{
    public class LeaveStructureDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<LeaveStructureVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<LeaveStructureVM> leaveStructureVMs = new List<LeaveStructureVM>();
            LeaveStructureVM leaveStructureVM;
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
,Code
,Name
,BranchId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From LeaveStructure
Where IsArchive=0
    ORDER BY Name
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    leaveStructureVM = new LeaveStructureVM();
                    leaveStructureVM.Id = Convert.ToInt32(dr["Id"]);
                    leaveStructureVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    leaveStructureVM.Code = dr["Code"].ToString();
                    leaveStructureVM.Name = dr["Name"].ToString();
                    leaveStructureVM.Remarks = dr["Remarks"].ToString();
                    leaveStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    leaveStructureVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    leaveStructureVM.CreatedBy = dr["CreatedBy"].ToString();
                    leaveStructureVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    leaveStructureVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    leaveStructureVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    leaveStructureVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    leaveStructureVMs.Add(leaveStructureVM);
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

            return leaveStructureVMs;
        }
        public List<LeaveStructureVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<LeaveStructureVM> VMs = new List<LeaveStructureVM>();
            LeaveStructureVM vm;
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
   FROM LeaveStructure
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
                    vm = new LeaveStructureVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
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
        //==================SelectByID=================
        public LeaveStructureVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            LeaveStructureVM leaveStructureVM = new LeaveStructureVM();
            List<LeaveStructureDetailVM> leaveStructureDVMs = new List<LeaveStructureDetailVM> ();
            LeaveStructureDetailVM leaveStructureDVM;

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
,Code
,Name
,BranchId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From LeaveStructure
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
                    leaveStructureVM.Id = Convert.ToInt32(dr["Id"]);
                    leaveStructureVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    leaveStructureVM.Code = dr["Code"].ToString();
                    leaveStructureVM.Name = dr["Name"].ToString();
                    leaveStructureVM.Remarks = dr["Remarks"].ToString();
                    leaveStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    leaveStructureVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    leaveStructureVM.CreatedBy = dr["CreatedBy"].ToString();
                    leaveStructureVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    leaveStructureVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    leaveStructureVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    leaveStructureVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                }
                dr.Close();
                sqlText = @" select
Id
,LeaveStructureId
,LeaveType_E
,LeaveDays
,IsEarned
,IsCompensation
,IsCarryForward
,MaxBalance
,Remarks
from LeaveStructureDetail where LeaveStructureId=@LeaveStructureId
";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = currConn;
                cmd.CommandText = sqlText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@LeaveStructureId", Id);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    leaveStructureDVM = new LeaveStructureDetailVM();
                    leaveStructureDVM.Id = Convert.ToInt32(dr["Id"]);
                    leaveStructureDVM.LeaveStructureId = Convert.ToInt32(dr["LeaveStructureId"]);
                    leaveStructureDVM.LeaveType_E = dr["LeaveType_E"].ToString();
                    leaveStructureDVM.LeaveDays = dr["LeaveDays"].ToString();
                    leaveStructureDVM.IsEarned = Convert.ToBoolean(dr["IsEarned"]);
                    leaveStructureDVM.IsCompensation = Convert.ToBoolean(dr["IsCompensation"]);
                    leaveStructureDVM.IsCarryForward = Convert.ToBoolean(dr["IsCarryForward"]);
                    leaveStructureDVM.MaxBalance = Convert.ToInt32(dr["MaxBalance"]);
                    leaveStructureDVM.Remarks = dr["Remarks"].ToString();
                    leaveStructureDVMs.Add(leaveStructureDVM);
                }
                dr.Close();
                #endregion
                leaveStructureVM.leaveStructureDetailVMs = leaveStructureDVMs;
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

            return leaveStructureVM;
        }
        //==================Insert =================
        public string[] Insert(LeaveStructureVM leaveStructureVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertLeaveStructure"; //Method Name


            #endregion

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(LeaveStructureVM.DepartmentId))
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

                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM LeaveStructure ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", LeaveStructureVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", LeaveStructureVM.Name);
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Travel Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Travel Value", "");
                //}
                //#endregion Exist
                #region Save

                //int foundId = (int)objfoundId;

                    sqlText = "  ";
                    sqlText += @" INSERT INTO LeaveStructure(Code,Name,BranchId,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Code,@Name,@BranchId,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Code", leaveStructureVM.Code.Trim());
                    cmdInsert.Parameters.AddWithValue("@Name", leaveStructureVM.Name.Trim());
                    cmdInsert.Parameters.AddWithValue("@BranchId", leaveStructureVM.BranchId);

                    cmdInsert.Parameters.AddWithValue("@Remarks", leaveStructureVM.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", leaveStructureVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", leaveStructureVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", leaveStructureVM.CreatedFrom);

                    cmdInsert.Transaction = transaction;
                    var exeRes = cmdInsert.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input LeaveStructure Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input LeaveStructure Value", "");
                    }
                    if (leaveStructureVM.leaveStructureDetailVMs != null)
                    {
                        foreach (LeaveStructureDetailVM item in leaveStructureVM.leaveStructureDetailVMs)
                        {
                            sqlText = @"  Insert Into LeaveStructureDetail
(
LeaveStructureId
,LeaveType_E
,LeaveDays
,IsEarned
,IsCompensation
,IsCarryForward
,MaxBalance
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) Values (
 @LeaveStructureId
,@LeaveType_E
,@LeaveDays
,@IsEarned
,@IsCompensation
,@IsCarryForward
,@MaxBalance
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)
";

                            SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                            cmd1.Parameters.AddWithValue("@LeaveStructureId",Id);
                            cmd1.Parameters.AddWithValue("@LeaveType_E",item.LeaveType_E);
                            cmd1.Parameters.AddWithValue("@LeaveDays",item.LeaveDays);
                            cmd1.Parameters.AddWithValue("@IsEarned",item.IsEarned);
                            cmd1.Parameters.AddWithValue("@IsCompensation", item.IsCompensation);
                            cmd1.Parameters.AddWithValue("@IsCarryForward", item.IsCarryForward);
                            cmd1.Parameters.AddWithValue("@MaxBalance", item.MaxBalance);
                            cmd1.Parameters.AddWithValue("@Remarks",item.Remarks?? Convert.DBNull);
                            cmd1.Parameters.AddWithValue("@IsActive",true);
                            cmd1.Parameters.AddWithValue("@IsArchive",false);
                            cmd1.Parameters.AddWithValue("@CreatedBy",leaveStructureVM.CreatedBy);
                            cmd1.Parameters.AddWithValue("@CreatedAt",leaveStructureVM.CreatedAt);
                            cmd1.Parameters.AddWithValue("@CreatedFrom",leaveStructureVM.CreatedFrom);
                            cmd1.Transaction = transaction;
                            cmd1.ExecuteNonQuery();
                        }
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
        //==================Update =================
        public string[] Update(LeaveStructureVM leaveStructureVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee LeaveStructure Update"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToLeaveStructure"); }

                #endregion open connection and transaction
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM LeaveStructure ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", LeaveStructureVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", LeaveStructureVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", LeaveStructureVM.Name);

                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This Travel already used!";
                //    throw new ArgumentNullException("Please Input Travel Value", "");
                //}
                //#endregion Exist

                if (leaveStructureVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update LeaveStructure set";
                    sqlText += " Code=@Code,";
                    sqlText += " Name=@Name,";
                    sqlText += " BranchId=@BranchId,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", leaveStructureVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@Code", leaveStructureVM.Code);
                    cmdUpdate.Parameters.AddWithValue("@Name", leaveStructureVM.Name);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", leaveStructureVM.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", leaveStructureVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", leaveStructureVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", leaveStructureVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", leaveStructureVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = leaveStructureVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", LeaveStructureVM.BranchId + " could not updated.");
                    }
                    List<LeaveStructureDetailVM> OldleaveStructureDVMs = new List<LeaveStructureDetailVM>();
                    LeaveStructureDetailVM leaveStructureDVM;
                    sqlText = @" select
Id
,LeaveStructureId
,LeaveType_E
,LeaveDays
,IsEarned
,IsCompensation
,IsCarryForward
,MaxBalance
,Remarks
from LeaveStructureDetail where LeaveStructureId=@LeaveStructureId
";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = currConn;
                    cmd.CommandText = sqlText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@LeaveStructureId", leaveStructureVM.Id);
                    cmd.Transaction = transaction;
                    SqlDataReader dr;
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        leaveStructureDVM = new LeaveStructureDetailVM();
                        leaveStructureDVM.Id = Convert.ToInt32(dr["Id"]);
                        leaveStructureDVM.LeaveStructureId = Convert.ToInt32(dr["LeaveStructureId"]);
                        leaveStructureDVM.LeaveType_E = dr["LeaveType_E"].ToString();
                        leaveStructureDVM.LeaveDays = dr["LeaveDays"].ToString();
                        leaveStructureDVM.IsEarned = Convert.ToBoolean(dr["IsEarned"]);
                        leaveStructureDVM.IsCompensation = Convert.ToBoolean(dr["IsCompensation"]);
                        leaveStructureDVM.IsCarryForward = Convert.ToBoolean(dr["IsCarryForward"]);
                        leaveStructureDVM.MaxBalance = Convert.ToInt32(dr["MaxBalance"]);
                        leaveStructureDVM.Remarks = dr["Remarks"].ToString();
                        OldleaveStructureDVMs.Add(leaveStructureDVM);
                    }
                    dr.Close();
                    foreach (LeaveStructureDetailVM item in OldleaveStructureDVMs)
                    {
                        if (!leaveStructureVM.leaveStructureDetailVMs.Any(x=>x.Id==item.Id))
                        {
                            sqlText = "delete LeaveStructureDetail where Id=@Id";
                            SqlCommand cmd11 = new SqlCommand(sqlText, currConn);
                            cmd11.Parameters.AddWithValue("@Id",item.Id);
                            cmd11.Transaction = transaction;
                            cmd11.ExecuteNonQuery();
                        }
                    }
                    foreach (LeaveStructureDetailVM item in leaveStructureVM.leaveStructureDetailVMs)
                    {
                        if (!OldleaveStructureDVMs.Any(x => x.Id == item.Id))
                        {
                            sqlText = @"  Insert Into LeaveStructureDetail
(
LeaveStructureId
,LeaveType_E
,LeaveDays
,IsEarned
,IsCompensation
,IsCarryForward
,MaxBalance
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) Values (
 @LeaveStructureId
,@LeaveType_E
,@LeaveDays
,@IsEarned
,@IsCompensation
,@IsCarryForward
,@MaxBalance
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)
";

                            SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                            cmd1.Parameters.AddWithValue("@LeaveStructureId", leaveStructureVM.Id);
                            cmd1.Parameters.AddWithValue("@LeaveType_E", item.LeaveType_E);
                            cmd1.Parameters.AddWithValue("@LeaveDays", item.LeaveDays);
                            cmd1.Parameters.AddWithValue("@IsEarned", item.IsEarned);
                            cmd1.Parameters.AddWithValue("@IsCompensation", item.IsCompensation);
                            cmd1.Parameters.AddWithValue("@IsCarryForward", item.IsCarryForward);
                            cmd1.Parameters.AddWithValue("@MaxBalance", item.MaxBalance);
                            cmd1.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);
                            cmd1.Parameters.AddWithValue("@IsActive", true);
                            cmd1.Parameters.AddWithValue("@IsArchive", false);
                            cmd1.Parameters.AddWithValue("@CreatedBy", leaveStructureVM.LastUpdateBy);
                            cmd1.Parameters.AddWithValue("@CreatedAt", leaveStructureVM.LastUpdateAt);
                            cmd1.Parameters.AddWithValue("@CreatedFrom", leaveStructureVM.LastUpdateFrom);
                            cmd1.Transaction = transaction;
                            cmd1.ExecuteNonQuery();
                        }
                    }
                    foreach (LeaveStructureDetailVM item in leaveStructureVM.leaveStructureDetailVMs)
                    {
                        if (OldleaveStructureDVMs.Any(x => x.Id == item.Id))
                        {
                            sqlText = @"  

Update LeaveStructureDetail set
LeaveStructureId= @LeaveStructureId
,LeaveType_E=@LeaveType_E
,LeaveDays=@LeaveDays
,IsEarned=@IsEarned
,IsCompensation=@IsCompensation
,IsCarryForward=@IsCarryForward
,MaxBalance=@MaxBalance
,Remarks=@Remarks
,LastUpdateBy=@LastUpdateBy
,LastUpdateAt=@LastUpdateAt
,LastUpdateFrom=@LastUpdateFrom 
where Id=@Id
";

                            SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                            cmd1.Parameters.AddWithValue("@Id", item.Id);
                            cmd1.Parameters.AddWithValue("@LeaveStructureId", leaveStructureVM.Id);
                            cmd1.Parameters.AddWithValue("@LeaveType_E", item.LeaveType_E);
                            cmd1.Parameters.AddWithValue("@LeaveDays", item.LeaveDays);
                            cmd1.Parameters.AddWithValue("@IsEarned", item.IsEarned);
                            cmd1.Parameters.AddWithValue("@IsCompensation", item.IsCompensation);
                            cmd1.Parameters.AddWithValue("@IsCarryForward", item.IsCarryForward);
                            cmd1.Parameters.AddWithValue("@MaxBalance", item.MaxBalance);
                            cmd1.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);
                            cmd1.Parameters.AddWithValue("@LastUpdateBy", leaveStructureVM.LastUpdateBy);
                            cmd1.Parameters.AddWithValue("@LastUpdateAt", leaveStructureVM.LastUpdateAt);
                            cmd1.Parameters.AddWithValue("@LastUpdateFrom", leaveStructureVM.LastUpdateFrom);
                            cmd1.Transaction = transaction;
                            cmd1.ExecuteNonQuery();
                        }
                    }
                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("LeaveStructure Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update LeaveStructure.";
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
        public LeaveStructureVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            LeaveStructureVM leaveStructureVM = new LeaveStructureVM();

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
,Code
,Name
,BranchId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From LeaveStructure
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
                        leaveStructureVM.Id = Convert.ToInt32(dr["Id"]);
                        leaveStructureVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                        leaveStructureVM.Code = dr["Code"].ToString();
                        leaveStructureVM.Name = dr["Name"].ToString();
                        leaveStructureVM.Remarks = dr["Remarks"].ToString();
                        leaveStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        leaveStructureVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        leaveStructureVM.CreatedBy = dr["CreatedBy"].ToString();
                        leaveStructureVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        leaveStructureVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        leaveStructureVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        leaveStructureVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return leaveStructureVM;
        }
        //==================Delete =================
        public string[] Delete(LeaveStructureVM leaveStructureVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteLeaveStructure"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToLeaveStructure"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length > 0)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update LeaveStructure set";
                        sqlText += " IsArchive=@IsArchive";
                        sqlText += " ,LastUpdateBy=@LastUpdateBy";
                        sqlText += " ,LastUpdateAt=@LastUpdateAt";
                        sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", leaveStructureVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", leaveStructureVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", leaveStructureVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        sqlText = "";
                        sqlText = "update LeaveStructureDetail set";
                        sqlText += " IsArchive=@IsArchive";
                        sqlText += " ,LastUpdateBy=@LastUpdateBy";
                        sqlText += " ,LastUpdateAt=@LastUpdateAt";
                        sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where LeaveStructureId=@Id";

                        SqlCommand cmd0 = new SqlCommand(sqlText, currConn);
                        cmd0.Parameters.AddWithValue("@Id", ids[i]);
                        cmd0.Parameters.AddWithValue("@IsArchive", true);
                        cmd0.Parameters.AddWithValue("@LastUpdateBy", leaveStructureVM.LastUpdateBy);
                        cmd0.Parameters.AddWithValue("@LastUpdateAt", leaveStructureVM.LastUpdateAt);
                        cmd0.Parameters.AddWithValue("@LastUpdateFrom", leaveStructureVM.LastUpdateFrom);
                        cmd0.Transaction = transaction;
                        exeRes = cmd0.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("LeaveStructure Delete", leaveStructureVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("LeaveStructure Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete LeaveStructure Information.";
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
        #endregion
    }
}
