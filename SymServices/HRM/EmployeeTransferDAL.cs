using SymOrdinary;
using SymServices.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymServices.HRM
{
    public class EmployeeTransferDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDAL = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectAll=================
        public List<EmployeeTransferVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeTransferVM> VMs = new List<EmployeeTransferVM>();
            EmployeeTransferVM vm;
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
Id
,EmployeeId
,BranchId
,ProjectId
,TransferDate
,DepartmentId
,IsCurrent
,SectionId
,Remarks
,FileName
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeTransfer
Where IsArchive=0
    ORDER BY TransferDate
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeTransferVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.IsCurrent = Convert.ToBoolean(dr["IsCurrent"]);
                    vm.TransferDate = Ordinary.StringToDate(dr["TransferDate"].ToString());
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.FileName = dr["FileName"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        //==================SelectAllByEmployee=================
        public List<EmployeeTransferVM> SelectAllByEmployee(string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeTransferVM> employeeTransferVMs = new List<EmployeeTransferVM>();
            EmployeeTransferVM employeeTransferVM;
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
t.Id
,t.EmployeeId
,t.BranchId
,t.ProjectId
,t.TransferDate
,t.DepartmentId
,t.IsCurrent
,t.SectionId
,t.Other1
,t.Other2
,t.Other3
,t.Other4
,t.Other5
,t.Remarks
,t.FileName
,t.IsActive
,t.IsArchive
,t.CreatedBy
,t.CreatedAt
,t.CreatedFrom
,t.LastUpdateBy
,t.LastUpdateAt
,t.LastUpdateFrom
,s.Name SectionName
,d.Name DepartmentName
,p.Name ProjectName
    From EmployeeTransfer t
	left outer join Section s on s.Id=t.SectionId
	left outer join Department d on d.Id=t.DepartmentId
	left outer join Project p on p.Id=t.ProjectId
Where t.IsArchive=0  AND t.BranchId=@BranchId AND t.EmployeeId=@EmployeeId
    ORDER BY Id DESC
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@BranchId", Ordinary.BranchId);
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeTransferVM = new EmployeeTransferVM();
                    employeeTransferVM.Id = dr["Id"].ToString();
                    employeeTransferVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeTransferVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    employeeTransferVM.ProjectId = dr["ProjectId"].ToString();
                    employeeTransferVM.DepartmentId = dr["DepartmentId"].ToString();
                    employeeTransferVM.IsCurrent = Convert.ToBoolean(dr["IsCurrent"]);
                    employeeTransferVM.TransferDate = Ordinary.StringToDate(dr["TransferDate"].ToString());
                    employeeTransferVM.SectionId = dr["SectionId"].ToString();
                    employeeTransferVM.Other1 = dr["Other1"].ToString();
                    employeeTransferVM.Other2 = dr["Other2"].ToString();
                    employeeTransferVM.Other3 = dr["Other3"].ToString();
                    employeeTransferVM.Other4 = dr["Other4"].ToString();
                    employeeTransferVM.Other5 = dr["Other5"].ToString();
                    employeeTransferVM.Remarks = dr["Remarks"].ToString();
                    employeeTransferVM.FileName = dr["FileName"].ToString();
                    employeeTransferVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeTransferVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    employeeTransferVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeTransferVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeTransferVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeTransferVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeTransferVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeTransferVM.DepartmentName = dr["DepartmentName"].ToString();
                    employeeTransferVM.SectionName = dr["SectionName"].ToString();
                    employeeTransferVM.ProjectName = dr["ProjectName"].ToString();
                    employeeTransferVMs.Add(employeeTransferVM);
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
            return employeeTransferVMs;
        }
        //==================SelectByEmployee=================
        public EmployeeTransferVM SelectByEmployeeCurrent(string employeeId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeTransferVM employeeTransferVM = new EmployeeTransferVM();
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
                #region sql statement
                sqlText = @"SELECT top 1
t.Id
,t.EmployeeId
,t.BranchId
,t.ProjectId
,t.TransferDate
,t.DepartmentId
,t.IsCurrent
,t.SectionId
,t.Other1
,t.Other2
,t.Other3
,t.Other4
,t.Other5
,t.Remarks
,t.FileName
,t.IsActive
,t.IsArchive
,t.CreatedBy
,t.CreatedAt
,t.CreatedFrom
,t.LastUpdateBy
,t.LastUpdateAt
,t.LastUpdateFrom
,s.Name SectionName
,d.Name DepartmentName
,p.Name ProjectName
    From EmployeeTransfer t
	left outer join Section s on s.Id=t.SectionId
	left outer join Department d on d.Id=t.DepartmentId
	left outer join Project p on p.Id=t.ProjectId
Where t.IsArchive=0 AND t.IsCurrent=@IsCurrent AND t.BranchId=@BranchId AND t.EmployeeId=@EmployeeId
    ORDER BY TransferDate
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@BranchId", Ordinary.BranchId);
                objComm.Parameters.AddWithValue("@EmployeeId", employeeId);
                objComm.Parameters.AddWithValue("@IsCurrent", true);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeTransferVM.Id = dr["Id"].ToString();
                    employeeTransferVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeTransferVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    employeeTransferVM.ProjectId = dr["ProjectId"].ToString();
                    employeeTransferVM.DepartmentId = dr["DepartmentId"].ToString();
                    employeeTransferVM.IsCurrent = Convert.ToBoolean(dr["IsCurrent"]);
                    employeeTransferVM.TransferDate = Ordinary.StringToDate(dr["TransferDate"].ToString());
                    employeeTransferVM.SectionId = dr["SectionId"].ToString();
                    employeeTransferVM.Other1 = dr["Other1"].ToString();
                    employeeTransferVM.Other2 = dr["Other2"].ToString();
                    employeeTransferVM.Other3 = dr["Other3"].ToString();
                    employeeTransferVM.Other4 = dr["Other4"].ToString();
                    employeeTransferVM.Other5 = dr["Other5"].ToString();
                    employeeTransferVM.Remarks = dr["Remarks"].ToString();
                    employeeTransferVM.FileName = dr["FileName"].ToString();
                    employeeTransferVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeTransferVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    employeeTransferVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeTransferVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeTransferVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeTransferVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeTransferVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeTransferVM.DepartmentName = dr["DepartmentName"].ToString();
                    employeeTransferVM.SectionName = dr["SectionName"].ToString();
                    employeeTransferVM.ProjectName = dr["ProjectName"].ToString();
                }
                dr.Close();
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
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
            return employeeTransferVM;
        }
        //==================SelectByID=================
        public EmployeeTransferVM SelectById(string Id)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeTransferVM employeeTransferVM = new EmployeeTransferVM();
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
,EmployeeId
,BranchId
,ProjectId
,DepartmentId
,TransferDate
,SectionId
,Other1
,Other2
,Other3
,Other4
,Other5
,IsCurrent
,Remarks
,FileName
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeTransfer
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
                    employeeTransferVM.Id = dr["Id"].ToString();
                    employeeTransferVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeTransferVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    employeeTransferVM.ProjectId = dr["ProjectId"].ToString();
                    employeeTransferVM.IsCurrent = Convert.ToBoolean(dr["IsCurrent"]);
                    employeeTransferVM.DepartmentId = dr["DepartmentId"].ToString();
                    employeeTransferVM.TransferDate = Ordinary.StringToDate(dr["TransferDate"].ToString());
                    employeeTransferVM.SectionId = dr["SectionId"].ToString();
                    employeeTransferVM.Other1 = dr["Other1"].ToString();
                    employeeTransferVM.Other2 = dr["Other2"].ToString();
                    employeeTransferVM.Other3 = dr["Other3"].ToString();
                    employeeTransferVM.Other4 = dr["Other4"].ToString();
                    employeeTransferVM.Other5 = dr["Other5"].ToString();
                    employeeTransferVM.Remarks = dr["Remarks"].ToString();
                    employeeTransferVM.FileName = dr["FileName"].ToString();
                    employeeTransferVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeTransferVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    employeeTransferVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeTransferVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeTransferVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeTransferVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeTransferVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
            return employeeTransferVM;
        }
        public bool CheckTransferDate(string employeeId, string date)
        {
            #region Variables
            SqlConnection currConn = null;
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
                sqlText = @"
select j.joinDate ,t.transferDate
from employeejob j
left outer join EmployeeTransfer t on t.employeeId=j.employeeId and t.isCurrent=1 and t.IsArchive=0
where j.employeeId=@employeeId
";
                string joinDate = "";
                string lasttransferDate = "";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@employeeId", employeeId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    joinDate = dr["joinDate"].ToString();
                    lasttransferDate = dr["transferDate"].ToString();
                }
                dr.Close();
                if (string.IsNullOrWhiteSpace(lasttransferDate))
                {
                    if (!string.IsNullOrWhiteSpace(joinDate))
                    {
                        if (Convert.ToInt32(joinDate) <= Convert.ToInt32(Ordinary.DateToString(date)))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (Convert.ToInt32(lasttransferDate) <= Convert.ToInt32(Ordinary.DateToString(date)))
                    {
                        return true;
                    }
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
            return false;
        }
        //==================Insert =================
        public string[] Insert(EmployeeTransferVM employeeTransferVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeTransfer"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region Validation
                //if (string.IsNullOrEmpty(employeeTransferVM.DepartmentId))
                //{
                //    retResults[1] = "Please Input Employee Transfer Course";
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
                //int foundId = (int)objfoundId;
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from EmployeeTransfer where BranchId=@BranchId";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@BranchId", employeeTransferVM.BranchId);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);
                employeeTransferVM.Id = employeeTransferVM.BranchId.ToString() + "_" + (count + 1);
                sqlText = " update EmployeeTransfer set IsCurrent=0 where EmployeeId =@EmployeeId";
                SqlCommand cmdExist0 = new SqlCommand(sqlText, currConn);
                cmdExist0.Parameters.AddWithValue("@EmployeeId", employeeTransferVM.EmployeeId);
                cmdExist0.Transaction = transaction;
                try
                {
                    cmdExist0.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new ArgumentNullException("Transfer is not complete", "");
                }
                if (employeeTransferVM != null)
                {
                    #region Inactive Transfer History
                    sqlText = "";
                    sqlText = @"
UPDATE EmployeeTransfer SET 
IsActive = 0
WHERE EmployeeId=@EmployeeId
";
                    SqlCommand cmdUpdateHistory = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdateHistory.Parameters.AddWithValue("@Id", employeeTransferVM.Id);
                    cmdUpdateHistory.Parameters.AddWithValue("@EmployeeId", employeeTransferVM.EmployeeId);

                    var exec = cmdUpdateHistory.ExecuteNonQuery();
                    int transResult = Convert.ToInt32(exec);
                    #endregion



                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeTransfer(Id,EmployeeId,BranchId,ProjectId,DepartmentId,SectionId,Other1,Other2,Other3,Other4,Other5,TransferDate,IsCurrent
                                        ,FileName,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Id,@EmployeeId,@BranchId,@ProjectId,@DepartmentId,@SectionId,@Other1,@Other2,@Other3,@Other4,@Other5,@TransferDate,@IsCurrent
                                        ,@FileName,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) ";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Id", employeeTransferVM.Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", employeeTransferVM.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@BranchId", employeeTransferVM.BranchId);
                    cmdInsert.Parameters.AddWithValue("@ProjectId", employeeTransferVM.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", employeeTransferVM.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@TransferDate", Ordinary.DateToString(employeeTransferVM.TransferDate));
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);
                    cmdInsert.Parameters.AddWithValue("@SectionId", employeeTransferVM.SectionId);
                    cmdInsert.Parameters.AddWithValue("@Other1", employeeTransferVM.Other1 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Other2", employeeTransferVM.Other2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Other3", employeeTransferVM.Other3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Other4", employeeTransferVM.Other4 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Other5", employeeTransferVM.Other5 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", employeeTransferVM.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FileName", employeeTransferVM.FileName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", employeeTransferVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", employeeTransferVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", employeeTransferVM.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();

                    #region Update EmployeeJob
                    EmployeeJobVM ejVM = new EmployeeJobVM();
                    ejVM.EmployeeId = employeeTransferVM.EmployeeId;
                    ejVM.ProjectId = employeeTransferVM.ProjectId;
                    ejVM.DepartmentId = employeeTransferVM.DepartmentId;
                    ejVM.SectionId = employeeTransferVM.SectionId;

                    ejVM.Other1 = employeeTransferVM.Other1;
                    ejVM.Other2 = employeeTransferVM.Other2;
                    ejVM.Other3 = employeeTransferVM.Other3;
                    ejVM.Other4 = employeeTransferVM.Other4;
                    ejVM.Other5 = employeeTransferVM.Other5;


                    ejVM.LastUpdateBy = employeeTransferVM.CreatedBy;
                    ejVM.LastUpdateAt= employeeTransferVM.CreatedAt;
                    ejVM.LastUpdateFrom = employeeTransferVM.CreatedFrom;

                    retResults = new EmployeeJobDAL().Update_EmployeeJob(ejVM, currConn, transaction);

                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion


                    //if (Id <= 0)
                    //{
                    //    retResults[1] = "Please Input Employee Transfer Value";
                    //    retResults[3] = sqlText;
                    //    throw new ArgumentNullException("Please Input Employee Transfer Value", "");
                    //}
                }
                else
                {
                    retResults[1] = "This Employee Transfer already used";
                    throw new ArgumentNullException("Please Input Employee Transfer Value", "");
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
                retResults[2] = employeeTransferVM.Id;
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
        public string[] Update(EmployeeTransferVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Transfer Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToTransfer"); }
                #endregion open connection and transaction
                if (vm != null)
                {

                    #region Inactive Transfer History 
                    sqlText = "";
                    sqlText = @"
UPDATE EmployeeTransfer SET 
IsActive = 0
WHERE EmployeeId=@EmployeeId
AND Id<>@Id
";
                    SqlCommand cmdUpdateHistory = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdateHistory.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdateHistory.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                    var exec = cmdUpdateHistory.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exec);
                    #endregion


                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EmployeeTransfer set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " BranchId=@BranchId,";
                    sqlText += " ProjectId=@ProjectId,";
                    sqlText += " DepartmentId=@DepartmentId,";
                    sqlText += " TransferDate=@TransferDate,";
                    sqlText += " IsCurrent=@IsCurrent,";
                    sqlText += " SectionId=@SectionId,";
                    sqlText += " Other1=@Other1,";
                    sqlText += " Other2=@Other2,";
                    sqlText += " Other3=@Other3,";
                    sqlText += " Other4=@Other4,";
                    sqlText += " Other5=@Other5,";
                    sqlText += " Remarks=@Remarks,";
                    if (vm.FileName != null)
                    {
                        sqlText += " FileName=@FileName,";
                    }
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@TransferDate", Ordinary.DateToString(vm.TransferDate));
                    cmdUpdate.Parameters.AddWithValue("@IsCurrent", true);
                    cmdUpdate.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdUpdate.Parameters.AddWithValue("@Other1", vm.Other1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Other2", vm.Other2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Other3", vm.Other3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Other4", vm.Other4 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Other5", vm.Other5 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    if (vm.FileName != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    }
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);


                    #region Update EmployeeJob
                    EmployeeJobVM ejVM = new EmployeeJobVM();
                    ejVM.EmployeeId = vm.EmployeeId;
                    ejVM.ProjectId = vm.ProjectId;
                    ejVM.DepartmentId = vm.DepartmentId;
                    ejVM.SectionId = vm.SectionId;
                    ejVM.Other1 = vm.Other1;
                    ejVM.Other2 = vm.Other2;
                    ejVM.Other3 = vm.Other3;
                    ejVM.Other4 = vm.Other4;
                    ejVM.Other5 = vm.Other5;
                    ejVM.LastUpdateBy = vm.LastUpdateBy;
                    ejVM.LastUpdateAt = vm.LastUpdateAt;
                    ejVM.LastUpdateFrom = vm.LastUpdateFrom;

                    retResults = new EmployeeJobDAL().Update_EmployeeJob(ejVM, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EmployeeTransferVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings




                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Transfer Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Transfer.";
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
        public EmployeeTransferVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeTransferVM employeeTransferVM = new EmployeeTransferVM();
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
,EmployeeId
,BranchId
,ProjectId
,DepartmentId
,TransferDate
,IsCurrent
,SectionId
,Remarks
,FileName
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeTransfer 
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
                        employeeTransferVM.Id = dr["Id"].ToString();
                        employeeTransferVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeeTransferVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                        employeeTransferVM.ProjectId = dr["ProjectId"].ToString();
                        employeeTransferVM.DepartmentId = dr["DepartmentId"].ToString();
                        employeeTransferVM.TransferDate = Ordinary.StringToDate(dr["TransferDate"].ToString());
                        employeeTransferVM.IsCurrent = Convert.ToBoolean(dr["IsCurrent"]);
                        employeeTransferVM.SectionId = dr["SectionId"].ToString();
                        employeeTransferVM.Remarks = dr["Remarks"].ToString();
                        employeeTransferVM.FileName = dr["FileName"].ToString();
                        employeeTransferVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeeTransferVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                        employeeTransferVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeeTransferVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeeTransferVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeeTransferVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeeTransferVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
            return employeeTransferVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeeTransferVM EmployeeTransferVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteTransfer"; //Method Name
            int transResult = 0;
            int countId = 0;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToTransfer"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeTransfer set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeeTransferVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeeTransferVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeeTransferVM.LastUpdateFrom);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Transfer Delete", EmployeeTransferVM.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Transfer Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Transfer Information.";
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
        //==================SelectAllForReport=================
        public List<EmployeeTransferVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeTransferVM> VMs = new List<EmployeeTransferVM>();
            EmployeeTransferVM vm;
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
 isnull(Trans.Id,0)						  Id
,ei.EmployeeId							EmployeeId
,isnull(Trans.BranchId	, 0)		BranchId		
,isnull(Trans.ProjectId	, 'NA')		ProjectId		
,isnull(Trans.TransferDate	, 'NA')		TransferDate		
,isnull(Trans.DepartmentId	, 'NA')		DepartmentId		
,isnull(Trans.IsCurrent	, 0)		IsCurrent		
,isnull(Trans.SectionId	, 'NA')		SectionId
,isnull(dept.Name , 'NA') Department
,isnull(sec.Name , 'NA') Section
,isnull(pro.Name , 'NA') Project
,isnull(Trans.Remarks	, 'NA')		Remarks		
,isnull(Trans.[FileName]	, 'NA')		[FileName]		
,isnull(Trans.Remarks		, 'NA')			Remarks
,isnull(Trans.IsActive, 0)			IsActive
,isnull(Trans.IsArchive, 0)			IsArchive
,isnull(Trans.CreatedBy, 'NA')		 CreatedBy
,isnull(Trans.CreatedAt, 'NA')		 CreatedAt
,isnull(Trans.CreatedFrom, 'NA')		CreatedFrom
,isnull(Trans.LastUpdateBy, 'NA')	 LastUpdateBy
,isnull(Trans.LastUpdateAt,	'NA')	 LastUpdateAt
,isnull(Trans.LastUpdateFrom,	'NA')	 LastUpdateFrom   
    From ViewEmployeeInformation ei
		left outer join EmployeeTransfer Trans on ei.EmployeeId=Trans.EmployeeId
		left outer join Department dept on dept.Id=Trans.DepartmentId
		left outer join Section sec on sec.Id=Trans.SectionId
		left outer join Project pro on pro.Id=Trans.ProjectId
Where ei.IsArchive=0 and ei.isActive=1 and Trans.IsArchive=0 and Trans.isActive=1 
";
                if (CodeF != "0_0")
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and ei.DepartmentId=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and ei.DesignationId=@DesignationId";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and ei.ProjectId=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and ei.SectionId=@SectionId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ei.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ei.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and ei.EmployeeId=@EmployeeId";
                }
                sqlText += "  order by ei.Department, ei.GradeSL, ei.joindate, ei.Code ";
                sqlText += "   ,Trans.TransferDate";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeTransferVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentName = dr["Department"].ToString();
                    vm.SectionName = dr["Section"].ToString();
                    vm.ProjectName = dr["Project"].ToString();
                    vm.IsCurrent = Convert.ToBoolean(dr["IsCurrent"]);
                    vm.TransferDate = Ordinary.StringToDate(dr["TransferDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.FileName = dr["FileName"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

        public DataTable Report(EmployeeTransferVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
 isnull(Trans.Id,0)					Id
,Trans.EmployeeId					EmployeeId
,isnull(Trans.TransferDate	, '19000101')	TransferDate			
,isnull(Trans.IsCurrent	, 0)		IsCurrent		
,isnull(dept.Name , 'NA')			Department
,isnull(sec.Name , 'NA')			Section
,isnull(pro.Name , 'NA')			Project
,isnull(Trans.Remarks, 'NA')		Remarks		
,case when isnull(Trans.IsCurrent, 0) = 0 then 'Previous'	else	'Current' end as TransferStatus
From
EmployeeTransfer Trans 
        left outer join  EmployeeInfo ei on ei.Id=Trans.EmployeeId
		left outer join EmployeeJob ej on ej.EmployeeId=ei.Id
		left outer join Department dept on dept.Id=Trans.DepartmentId
		left outer join Section sec on sec.Id=Trans.SectionId
		left outer join Project pro on pro.Id=Trans.ProjectId
Where 1=1 and ei.isActive=1 
and ej.JoinDate < Trans.TransferDate
";
                #region More Conditions
                if (vm.EmployeeIdList != null && vm.EmployeeIdList.Count > 0)
                {
                    string MultipleEmployeeId = "";
                    foreach (var item in vm.EmployeeIdList)
                    {
                        MultipleEmployeeId += "'" + item + "',";
                    }
                    MultipleEmployeeId = MultipleEmployeeId.Remove(MultipleEmployeeId.Length - 1);
                    sqlText += " AND ei.Id IN(" + MultipleEmployeeId + ")";
                }
                #endregion

                #region ConditionFields
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


                #endregion ConditionFields
                sqlText += " ORDER By ei.Code, TransferStatus";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                #region ConditionValues
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
                #endregion ConditionValues
                da.Fill(dt);

                string[] dateColumnChange = { "TransferDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, dateColumnChange);
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

        #endregion
    }
}
