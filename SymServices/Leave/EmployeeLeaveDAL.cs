using Microsoft.Office.Interop.Excel;
using SymOrdinary;
using SymServices.Attendance;
using SymServices.Common;
using SymServices.HRM;
using SymViewModel.Attendance;
using SymViewModel.HRM;
using SymViewModel.Leave;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymServices.Leave
{
    public class EmployeeLeaveDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        #region Methods
        //==================SelectAll=================
        public List<EmployeeLeaveVM> SelectAll(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
       {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeLeaveVM> VMs = new List<EmployeeLeaveVM>();
            EmployeeLeaveVM vm;
            #endregion
            try
            {
                

                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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
                #region sql statement
                sqlText = @"
SELECT 
l.Id
,l.EmployeeId
,ej.Supervisor
,l.EmployeeLeaveStructureId
,l.LeaveYear
,l.LeaveType_E
,CONVERT(NVARCHAR, CONVERT(DATE, l.FromDate, 112), 103) FromDate
,CONVERT(NVARCHAR, CONVERT(DATE, l.ToDate, 112), 103) ToDate
,l.TotalLeave
,l.ApprovedBy
,ISNULL(l.IsApprove,0)IsApprove
,ISNULL(l.IsReject,0) IsReject
,l.IsHalfDay
,ISNULL(l.IsLWP, 0) IsLWP
,CASE WHEN l.IsApprove=1 THEN 'Approved'  WHEN l.IsReject=1 THEN 'Rejected'  ELSE 'Pending' END AS Approval
,CASE WHEN l.IsHalfDay=1 THEN 'Half Day' ELSE 'Full Day' END AS DayType
,l.Remarks
,l.IsActive
,l.IsArchive
,l.CreatedBy
,l.CreatedAt
,l.CreatedFrom
,l.LastUpdateBy
,l.LastUpdateAt
,l.LastUpdateFrom
,isnull(eInfo.Code,'') EmpCode,isnull(empname,'')EmpName
    FROM EmployeeLeave l
left outer join ViewEmployeeInformation eInfo on eInfo.id=l.EmployeeId
left outer join EmployeeJob ej on l.EmployeeId = ej.EmployeeId
Where l.IsArchive=0
    ORDER BY l.FromDate DESC, l.LeaveType_E
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Transaction = transaction;
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeaveVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Supervisor = dr["Supervisor"].ToString();
                    vm.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsReject = Convert.ToBoolean(dr["IsReject"]);
                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    vm.IsLWP = Convert.ToBoolean(dr["IsLWP"]);
                    vm.Approval = dr["Approval"].ToString();
                    vm.DayType = dr["DayType"].ToString();
                    vm.EmpCode = dr["EmpCode"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
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
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }
        public List<EmployeeLeaveVM> SelectAllFromSchedule(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeLeaveVM> VMs = new List<EmployeeLeaveVM>();
            EmployeeLeaveVM vm;
            #endregion
            try
            {


                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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
                #region sql statement
                sqlText = @"
SELECT 
l.Id
,l.EmployeeId
,ej.Supervisor
,l.EmployeeLeaveStructureId
,l.LeaveYear
,l.LeaveType_E
,CONVERT(NVARCHAR, CONVERT(DATE, l.FromDate, 112), 103) FromDate
,CONVERT(NVARCHAR, CONVERT(DATE, l.ToDate, 112), 103) ToDate
,l.TotalLeave
,l.ApprovedBy
,ISNULL(l.IsApprove,0)IsApprove
,ISNULL(l.IsReject,0) IsReject
,l.IsHalfDay
,ISNULL(l.IsLWP, 0) IsLWP
,CASE WHEN l.IsApprove=1 THEN 'Approved'  WHEN l.IsReject=1 THEN 'Rejected'  ELSE 'Pending' END AS Approval
,CASE WHEN l.IsHalfDay=1 THEN 'Half Day' ELSE 'Full Day' END AS DayType
,l.Remarks
,l.IsActive
,l.IsArchive
,l.CreatedBy
,l.CreatedAt
,l.CreatedFrom
,l.LastUpdateBy
,l.LastUpdateAt
,l.LastUpdateFrom
,isnull(eInfo.Code,'') EmpCode,isnull(empname,'')EmpName
    FROM EmployeeLeaveSchedule l
left outer join ViewEmployeeInformation eInfo on eInfo.id=l.EmployeeId
left outer join EmployeeJob ej on l.EmployeeId = ej.EmployeeId
Where l.IsArchive=0
    ORDER BY l.FromDate DESC, l.LeaveType_E
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Transaction = transaction;
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeaveVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Supervisor = dr["Supervisor"].ToString();
                    vm.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsReject = Convert.ToBoolean(dr["IsReject"]);
                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    vm.IsLWP = Convert.ToBoolean(dr["IsLWP"]);
                    vm.Approval = dr["Approval"].ToString();
                    vm.DayType = dr["DayType"].ToString();
                    vm.EmpCode = dr["EmpCode"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
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
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }
        //==================SelectAll=================
        public List<EmployeeLeaveBalanceVM> EmployeeLeaveBalance(string EmployeeId, string leaveyear = "0")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveBalanceVM> employeeLeaves = new List<EmployeeLeaveBalanceVM>();
            EmployeeLeaveBalanceVM employeeLeave;
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
                sqlText = @"SELECT ID,ES.LEAVETYPE_E ,isnull(es.OpeningLeaveDays,0)OpeningLeaveDays,isnull( ES.LEAVEDAYS,0)LEAVEDAYS
,ISNULL(EL.LEAVE,0) USED,ISNULL(isnull( ES.LEAVEDAYS,0)+isnull(es.OpeningLeaveDays,0)-ISNULL(EL.LEAVE,0),0) HAVE
FROM EMPLOYEELEAVESTRUCTURE ES
LEFT OUTER JOIN (
SELECT EMPLOYEELEAVESTRUCTUREID,SUM(TOTALLEAVE) LEAVE FROM EMPLOYEELEAVE where IsApprove=1 and IsReject = 0  GROUP BY EMPLOYEELEAVESTRUCTUREID
) EL ON EL.EMPLOYEELEAVESTRUCTUREID=ES.ID
WHERE ES.EMPLOYEEID=@EmployeeId
and ES.LeaveYear=@LeaveYear
    ORDER BY ES.LEAVETYPE_E
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                objComm.Parameters.AddWithValue("@LeaveYear", leaveyear);
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeLeave = new EmployeeLeaveBalanceVM();
                    employeeLeave.LeaveType = dr["LEAVETYPE_E"].ToString();
                    employeeLeave.OpeningBalance = dr["OpeningLeaveDays"].ToString();
                    employeeLeave.Total = dr["LEAVEDAYS"].ToString();
                    employeeLeave.Used = dr["USED"].ToString();
                    employeeLeave.Have = dr["HAVE"].ToString();
                    employeeLeaves.Add(employeeLeave);
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
            return employeeLeaves;
        }
        //==================SelectAllForSupervisor=================
        public List<EmployeeLeaveVM> SelectAllForSupervisor(string SupervisorId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {        
                       
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeLeaveVM> VMs = new List<EmployeeLeaveVM>();
            EmployeeLeaveVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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

                EmployeeInfoDAL _empDal = new EmployeeInfoDAL();
                EmployeeInfoVM empVM = new EmployeeInfoVM();
                string[] conFields = { "Id" };
                string[] conValues = { SupervisorId };
                empVM = _empDal.SelectCommonFields(conFields, conValues, currConn, transaction).FirstOrDefault();
               

                #region sql statement
                sqlText = @"
SELECT
 l.Id
,l.EmployeeId
,ej.Supervisor 
,SUBSTRING(ej.Supervisor, 99, CHARINDEX('~', ej.Supervisor)) as SupervisorCode

,l.EmployeeLeaveStructureId
,l.LeaveYear
,l.LeaveType_E
,CONVERT(NVARCHAR, CONVERT(DATE, l.FromDate, 112), 103) FromDate
,CONVERT(NVARCHAR, CONVERT(DATE, l.ToDate, 112), 103) ToDate
,l.TotalLeave
,ISNULL(l.IsApprove,0)IsApprove
,ISNULL(l.IsReject ,0)IsReject
,l.IsHalfDay
,l.IsHalfDay
,ISNULL(l.IsLWP, 0) IsLWP
,CASE WHEN l.IsApprove=1 THEN 'Approved'  WHEN l.IsReject=1 THEN 'Rejected'  ELSE 'Pending' END AS Approval
,CASE WHEN l.IsHalfDay=1 THEN 'Half Day' ELSE 'Full Day' END AS DayType
,l.ApprovedBy
,ISNULL(eInfo.Code,'') EmpCode,isnull(empname,'')EmpName
    FROM EmployeeLeave l
LEFT OUTER JOIN ViewEmployeeInformation eInfo on eInfo.id=l.EmployeeId
LEFT OUTER JOIN EmployeeJob ej on l.EmployeeId = ej.EmployeeId
Where l.IsArchive=0 and eInfo.IsActive=1

";


                //if (!string.IsNullOrWhiteSpace(empVM.Code))
                //{
                //sqlText += " And ej.Supervisor like '%"+empVM.Code+"%'";
                //sqlText += " And ej.Supervisor like '%"+"@code"+"%'";
                sqlText += " AND SUBSTRING(ej.Supervisor, 0, CHARINDEX('~', ej.Supervisor))  = @Code";
                //}
                sqlText += @" ORDER BY CASE 
                WHEN l.IsApprove = 1 THEN 2
                WHEN l.IsReject = 1 THEN 1
                ELSE 3
                END DESC,
                    l.FromDate DESC";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);              
                objComm.Parameters.AddWithValue("@Code", empVM.Code);   
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeaveVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Supervisor = dr["Supervisor"].ToString();
                    vm.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsReject = Convert.ToBoolean(dr["IsReject"]);

                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    vm.IsLWP = Convert.ToBoolean(dr["IsLWP"]);
                    vm.Approval = dr["Approval"].ToString();
                    vm.DayType = dr["DayType"].ToString();
                    vm.EmpCode = dr["EmpCode"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }

        //==================SelectAll=================
        public List<EmployeeLeaveVM> SelectAllByDate(string code, string FromDate, string ToDate)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveVM> employeeLeaves = new List<EmployeeLeaveVM>();
            EmployeeLeaveVM employeeLeave;
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
 l.Id
,l.EmployeeId
,ej.Supervisor
,l.EmployeeLeaveStructureId
,l.LeaveYear
,l.LeaveType_E
,CONVERT(NVARCHAR, CONVERT(DATE, l.FromDate, 112), 103) FromDate
,CONVERT(NVARCHAR, CONVERT(DATE, l.ToDate, 112), 103) ToDate
,l.TotalLeave
,ISNULL(l.IsApprove,0)IsApprove
,ISNULL(l.IsReject ,0)IsReject
,l.IsHalfDay
,l.IsHalfDay
,ISNULL(l.IsLWP, 0) IsLWP
,CASE WHEN l.IsApprove=1 THEN 'Approved' WHEN l.IsReject=1 THEN 'Rejected' ELSE 'Pending' END AS Approval
,CASE WHEN l.IsHalfDay=1 THEN 'Half Day' ELSE 'Full Day' END AS DayType
,l.ApprovedBy
,ISNULL(eInfo.Code,'') EmpCode,isnull(empname,'')EmpName
    FROM EmployeeLeave l
LEFT OUTER JOIN ViewEmployeeInformation eInfo on eInfo.id=l.EmployeeId
LEFT OUTER JOIN EmployeeJob ej on l.EmployeeId = ej.EmployeeId
Where l.IsArchive=0 and eInfo.IsActive=1 and l.FromDate between @FromDate and @ToDate 
";
                if (!string.IsNullOrWhiteSpace(code))
                {
                    sqlText += " And eInfo.id=@code";
                }
                sqlText += " ORDER BY l.FromDate DESC, l.LeaveType_E";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(code))
                {
                    objComm.Parameters.AddWithValue("@code", code);
                }
                if (!string.IsNullOrWhiteSpace(FromDate))
                {
                    objComm.Parameters.AddWithValue("@FromDate", FromDate);
                }
                if (!string.IsNullOrWhiteSpace(ToDate))
                {
                    objComm.Parameters.AddWithValue("@ToDate", ToDate);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeLeave = new EmployeeLeaveVM();
                    employeeLeave.Id = Convert.ToInt32(dr["Id"]);
                    employeeLeave.EmployeeId = dr["EmployeeId"].ToString();
                    employeeLeave.Supervisor = dr["Supervisor"].ToString();
                    employeeLeave.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    employeeLeave.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    employeeLeave.LeaveType_E = dr["LeaveType_E"].ToString();
                    employeeLeave.FromDate = dr["FromDate"].ToString();
                    employeeLeave.ToDate = dr["ToDate"].ToString();                    
                    employeeLeave.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    employeeLeave.ApprovedBy = dr["ApprovedBy"].ToString();
                    employeeLeave.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    employeeLeave.IsReject = Convert.ToBoolean(dr["IsReject"]);
                    employeeLeave.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    employeeLeave.IsLWP = Convert.ToBoolean(dr["IsLWP"]);
                    employeeLeave.Approval = dr["Approval"].ToString();
                    employeeLeave.DayType = dr["DayType"].ToString();
                    employeeLeave.EmpCode = dr["EmpCode"].ToString();
                    employeeLeave.EmpName = dr["EmpName"].ToString();
                    employeeLeaves.Add(employeeLeave);
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
            return employeeLeaves;
        }
        public List<EmployeeLeaveVM> SelectAllByDateRange(string FromDate, string ToDate)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveVM> employeeLeaves = new List<EmployeeLeaveVM>();
            EmployeeLeaveVM employeeLeave;
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
 l.Id
,l.EmployeeId
,ej.Supervisor
,l.EmployeeLeaveStructureId
,l.LeaveYear
,l.LeaveType_E
,CONVERT(NVARCHAR, CONVERT(DATE, l.FromDate, 112), 103) FromDate
,CONVERT(NVARCHAR, CONVERT(DATE, l.ToDate, 112), 103) ToDate
,l.TotalLeave
,ISNULL(l.IsApprove,0)IsApprove
,ISNULL(l.IsReject ,0)IsReject
,l.IsHalfDay
,l.IsHalfDay
,ISNULL(l.IsLWP, 0) IsLWP
,CASE WHEN l.IsApprove=1 THEN 'Approved' WHEN l.IsReject=1 THEN 'Rejected' ELSE 'Pending' END AS Approval
,CASE WHEN l.IsHalfDay=1 THEN 'Half Day' ELSE 'Full Day' END AS DayType
,l.ApprovedBy
,ISNULL(eInfo.Code,'') EmpCode,isnull(empname,'')EmpName
    FROM EmployeeLeave l
LEFT OUTER JOIN ViewEmployeeInformation eInfo on eInfo.id=l.EmployeeId
LEFT OUTER JOIN EmployeeJob ej on l.EmployeeId = ej.EmployeeId
Where l.IsArchive=0 and eInfo.IsActive=1 and l.FromDate between @FromDate and @ToDate 
";
               
                sqlText += " ORDER BY l.FromDate DESC, l.LeaveType_E";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (!string.IsNullOrWhiteSpace(FromDate))
                {
                    objComm.Parameters.AddWithValue("@FromDate", FromDate);
                }
                if (!string.IsNullOrWhiteSpace(ToDate))
                {
                    objComm.Parameters.AddWithValue("@ToDate", ToDate);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeLeave = new EmployeeLeaveVM();
                    employeeLeave.Id = Convert.ToInt32(dr["Id"]);
                    employeeLeave.EmployeeId = dr["EmployeeId"].ToString();
                    employeeLeave.Supervisor = dr["Supervisor"].ToString();
                    employeeLeave.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    employeeLeave.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    employeeLeave.LeaveType_E = dr["LeaveType_E"].ToString();
                    employeeLeave.FromDate = dr["FromDate"].ToString();
                    employeeLeave.ToDate = dr["ToDate"].ToString();
                    employeeLeave.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    employeeLeave.ApprovedBy = dr["ApprovedBy"].ToString();
                    employeeLeave.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    employeeLeave.IsReject = Convert.ToBoolean(dr["IsReject"]);
                    employeeLeave.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    employeeLeave.IsLWP = Convert.ToBoolean(dr["IsLWP"]);
                    employeeLeave.Approval = dr["Approval"].ToString();
                    employeeLeave.DayType = dr["DayType"].ToString();
                    employeeLeave.EmpCode = dr["EmpCode"].ToString();
                    employeeLeave.EmpName = dr["EmpName"].ToString();
                    employeeLeaves.Add(employeeLeave);
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
            return employeeLeaves;
        }
        public List<EmployeeLeaveVM> SelectAll(string code, string status)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveVM> employeeLeaves = new List<EmployeeLeaveVM>();
            EmployeeLeaveVM employeeLeave;
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
 l.Id
,l.EmployeeId
,ej.Supervisor
,l.EmployeeLeaveStructureId
,l.LeaveYear
,l.LeaveType_E
,CONVERT(NVARCHAR, CONVERT(DATE, l.FromDate, 112), 103) FromDate
,CONVERT(NVARCHAR, CONVERT(DATE, l.ToDate, 112), 103) ToDate
,l.TotalLeave
,ISNULL(l.IsApprove,0)IsApprove
,ISNULL(l.IsReject ,0)IsReject
,l.IsHalfDay
,l.IsHalfDay
,ISNULL(l.IsLWP, 0) IsLWP
,CASE WHEN l.IsApprove=1 THEN 'Approved' WHEN l.IsReject=1 THEN 'Rejected' ELSE 'Pending' END AS Approval
,CASE WHEN l.IsHalfDay=1 THEN 'Half Day' ELSE 'Full Day' END AS DayType
,l.ApprovedBy
,ISNULL(eInfo.Code,'') EmpCode,isnull(empname,'')EmpName
    FROM EmployeeLeave l
LEFT OUTER JOIN ViewEmployeeInformation eInfo on eInfo.id=l.EmployeeId
LEFT OUTER JOIN EmployeeJob ej on l.EmployeeId = ej.EmployeeId
Where l.IsArchive=0 and eInfo.IsActive=1
";
                if (!string.IsNullOrWhiteSpace(code))
                {
                    sqlText += " And eInfo.id=@code";
                }
                sqlText += " ORDER BY l.FromDate DESC, l.LeaveType_E";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(code))
                {
                    objComm.Parameters.AddWithValue("@code", code);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeLeave = new EmployeeLeaveVM();
                    employeeLeave.Id = Convert.ToInt32(dr["Id"]);
                    employeeLeave.EmployeeId = dr["EmployeeId"].ToString();
                    employeeLeave.Supervisor = dr["Supervisor"].ToString();
                    employeeLeave.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    employeeLeave.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    employeeLeave.LeaveType_E = dr["LeaveType_E"].ToString();
                    employeeLeave.FromDate = dr["FromDate"].ToString();
                    employeeLeave.ToDate = dr["ToDate"].ToString();
                    employeeLeave.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    employeeLeave.ApprovedBy = dr["ApprovedBy"].ToString();
                    employeeLeave.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    employeeLeave.IsReject = Convert.ToBoolean(dr["IsReject"]);
                    employeeLeave.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    employeeLeave.IsLWP = Convert.ToBoolean(dr["IsLWP"]);
                    employeeLeave.Approval = dr["Approval"].ToString();
                    employeeLeave.DayType = dr["DayType"].ToString();
                    employeeLeave.EmpCode = dr["EmpCode"].ToString();
                    employeeLeave.EmpName = dr["EmpName"].ToString();
                    employeeLeaves.Add(employeeLeave);
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
            return employeeLeaves;
        }
        public List<EmployeeLeaveVM> SelectAllfromSchedule(string code, string status)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveVM> employeeLeaves = new List<EmployeeLeaveVM>();
            EmployeeLeaveVM employeeLeave;
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
 l.Id
,l.EmployeeId
,ej.Supervisor
,l.EmployeeLeaveStructureId
,l.LeaveYear
,l.LeaveType_E
,CONVERT(NVARCHAR, CONVERT(DATE, l.FromDate, 112), 103) FromDate
,CONVERT(NVARCHAR, CONVERT(DATE, l.ToDate, 112), 103) ToDate
,l.TotalLeave
,ISNULL(l.IsApprove,0)IsApprove
,ISNULL(l.IsReject ,0)IsReject
,l.IsHalfDay
,l.IsHalfDay
,ISNULL(l.IsLWP, 0) IsLWP
,CASE WHEN l.IsApprove=1 THEN 'Approved' WHEN l.IsReject=1 THEN 'Rejected' ELSE 'Pending' END AS Approval
,CASE WHEN l.IsHalfDay=1 THEN 'Half Day' ELSE 'Full Day' END AS DayType
,l.ApprovedBy
,ISNULL(eInfo.Code,'') EmpCode,isnull(empname,'')EmpName
    FROM EmployeeLeaveSchedule l
LEFT OUTER JOIN ViewEmployeeInformation eInfo on eInfo.id=l.EmployeeId
LEFT OUTER JOIN EmployeeJob ej on l.EmployeeId = ej.EmployeeId
Where l.IsArchive=0 and eInfo.IsActive=1
";
                if (!string.IsNullOrWhiteSpace(code))
                {
                    sqlText += " And eInfo.id=@code";
                }
                sqlText += " ORDER BY l.FromDate DESC, l.LeaveType_E";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(code))
                {
                    objComm.Parameters.AddWithValue("@code", code);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeLeave = new EmployeeLeaveVM();
                    employeeLeave.Id = Convert.ToInt32(dr["Id"]);
                    employeeLeave.EmployeeId = dr["EmployeeId"].ToString();
                    employeeLeave.Supervisor = dr["Supervisor"].ToString();
                    employeeLeave.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    employeeLeave.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    employeeLeave.LeaveType_E = dr["LeaveType_E"].ToString();
                    employeeLeave.FromDate = dr["FromDate"].ToString();
                    employeeLeave.ToDate = dr["ToDate"].ToString();
                    employeeLeave.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    employeeLeave.ApprovedBy = dr["ApprovedBy"].ToString();
                    employeeLeave.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    employeeLeave.IsReject = Convert.ToBoolean(dr["IsReject"]);
                    employeeLeave.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    employeeLeave.IsLWP = Convert.ToBoolean(dr["IsLWP"]);
                    employeeLeave.Approval = dr["Approval"].ToString();
                    employeeLeave.DayType = dr["DayType"].ToString();
                    employeeLeave.EmpCode = dr["EmpCode"].ToString();
                    employeeLeave.EmpName = dr["EmpName"].ToString();
                    employeeLeaves.Add(employeeLeave);
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
            return employeeLeaves;
        }
        public List<EmployeeLeaveStructureVM> SelectAllOpening(string empcode, string vyear)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveStructureVM> employeeLeaves = new List<EmployeeLeaveStructureVM>();
            EmployeeLeaveStructureVM employeeLeave;
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
                sqlText = @"select els.Id
,els.EmployeeId
,einfo.Code EmpCode
,einfo.Salutation_E Salutation
,einfo.MiddleName
,einfo.LastName
,els.LeaveStructureId
,els.LeaveYear
,els.LeaveType_E
,isnull(els.LeaveDays,0)LeaveDays
,isnull(els.OpeningLeaveDays,0)OpeningLeaveDays
,els.IsEarned
,els.IsCompensation
,els.MaxBalance
 from EmployeeLeaveStructure els
left outer join employeeinfo einfo on els.employeeid=einfo.id
and einfo.IsArchive=0
";
                if (!string.IsNullOrWhiteSpace(vyear))
                {
                    sqlText += " where els.LeaveYear='" + vyear + "'";
                }
                if (!string.IsNullOrWhiteSpace(empcode))
                {
                    sqlText += " and einfo.Code='" + empcode + "'";
                }
                sqlText += " order by els.LeaveYear,einfo.code,els.LeaveType_E";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeLeave = new EmployeeLeaveStructureVM();
                    employeeLeave.Id = Convert.ToInt32(dr["Id"]);
                    employeeLeave.EmployeeId = dr["EmployeeId"].ToString();
                    employeeLeave.EmpCode = dr["EmpCode"].ToString();
                    employeeLeave.EmpName = dr["Salutation"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                    employeeLeave.LeaveStructureId = Convert.ToInt32(dr["LeaveStructureId"]);
                    employeeLeave.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    employeeLeave.LeaveType_E = dr["LeaveType_E"].ToString();
                    employeeLeave.LeaveDays = Convert.ToDecimal(dr["LeaveDays"]);
                    employeeLeave.OpeningDays = Convert.ToDecimal(dr["OpeningLeaveDays"]);
                    employeeLeave.MaxBalance = Convert.ToDecimal(dr["MaxBalance"]);
                    employeeLeave.IsEarned = Convert.ToBoolean(dr["IsEarned"]);
                    employeeLeave.IsCompensation = Convert.ToBoolean(dr["IsCompensation"]);
                    employeeLeaves.Add(employeeLeave);
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
            return employeeLeaves;
        }
        public EmployeeLeaveVM SelectById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeLeaveVM vm = new EmployeeLeaveVM();
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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
                #region sql statement
                sqlText = @"SELECT
EmployeeLeave.Id
,EmployeeLeave.EmployeeId
,P.OtherId
,EmployeeLeaveStructureId
,LeaveYear
,LeaveType_E
,FromDate
,ToDate
,TotalLeave
,ApprovedBy
,ISNULL(IsApprove,0)IsApprove
,ISNULL(IsReject ,0)IsReject
,IsHalfDay
,ISNULL(IsLWP, 0) IsLWP
,EmployeeLeave.Remarks
,CASE WHEN IsApprove=1 THEN 'Approved' WHEN IsReject=1 THEN 'Rejected' ELSE 'Pending' END AS Approval
,CASE WHEN IsHalfDay=1 THEN 'Half Day' ELSE 'Full Day' END AS DayType
,ApprovedBy
,isnull(eInfo.Code,'') EmpCode,isnull(eInfo.Salutation_E,'') Salutation,isnull(eInfo.MiddleName,'')MiddleName,isnull(eInfo.LastName,'')LastName
FROM EmployeeLeave
left outer join employeeInfo eInfo on eInfo.id=EmployeeLeave.EmployeeId
left outer join EmployeePersonalDetail P on P.EmployeeId=EmployeeLeave.EmployeeId
where  EmployeeLeave.id=@Id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Transaction = transaction;

                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsReject = Convert.ToBoolean(dr["IsReject"]);

                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    vm.IsLWP = Convert.ToBoolean(dr["IsLWP"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Approval = dr["Approval"].ToString();
                    vm.DayType = dr["DayType"].ToString();
                    vm.EmpCode = dr["EmpCode"].ToString();
                    vm.OtherId = dr["OtherId"].ToString();
                    vm.EmpName = dr["Salutation"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                    //employeeLeaveVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    //employeeLeaveVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    //employeeLeaveVM.CreatedBy = dr["CreatedBy"].ToString();
                    //employeeLeaveVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    //employeeLeaveVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    //employeeLeaveVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    //employeeLeaveVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                }
                dr.Close();
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return vm;
        }
        public EmployeeLeaveVM SelectScheduleById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeLeaveVM vm = new EmployeeLeaveVM();
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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
                #region sql statement
                sqlText = @"SELECT
EmployeeLeave.Id
,EmployeeLeave.EmployeeId
,P.OtherId
,EmployeeLeaveStructureId
,LeaveYear
,LeaveType_E
,FromDate
,ToDate
,TotalLeave
,ApprovedBy
,ISNULL(IsApprove,0)IsApprove
,ISNULL(IsReject ,0)IsReject
,IsHalfDay
,ISNULL(IsLWP, 0) IsLWP
,EmployeeLeave.Remarks
,CASE WHEN IsApprove=1 THEN 'Approved' WHEN IsReject=1 THEN 'Rejected' ELSE 'Pending' END AS Approval
,CASE WHEN IsHalfDay=1 THEN 'Half Day' ELSE 'Full Day' END AS DayType
,ApprovedBy
,isnull(eInfo.Code,'') EmpCode,isnull(eInfo.Salutation_E,'') Salutation,isnull(eInfo.MiddleName,'')MiddleName,isnull(eInfo.LastName,'')LastName
FROM EmployeeLeaveSchedule
left outer join employeeInfo eInfo on eInfo.id=EmployeeLeave.EmployeeId
left outer join EmployeePersonalDetail P on P.EmployeeId=EmployeeLeave.EmployeeId
where  EmployeeLeave.id=@Id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Transaction = transaction;

                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsReject = Convert.ToBoolean(dr["IsReject"]);

                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    vm.IsLWP = Convert.ToBoolean(dr["IsLWP"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Approval = dr["Approval"].ToString();
                    vm.DayType = dr["DayType"].ToString();
                    vm.EmpCode = dr["EmpCode"].ToString();
                    vm.OtherId = dr["OtherId"].ToString();
                    vm.EmpName = dr["Salutation"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                    //employeeLeaveVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    //employeeLeaveVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    //employeeLeaveVM.CreatedBy = dr["CreatedBy"].ToString();
                    //employeeLeaveVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    //employeeLeaveVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    //employeeLeaveVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    //employeeLeaveVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                }
                dr.Close();
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return vm;
        }
        //==================SelectByID=================
        public List<EmployeeLeaveVM> SelectScheduleByEmployeeId(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeLeaveVM vm = new EmployeeLeaveVM();
            List<EmployeeLeaveVM> VMs = new List<EmployeeLeaveVM>();
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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
                #region sql statement
                sqlText = @"SELECT
EmployeeLeave.Id
,EmployeeId
,EmployeeLeaveStructureId
,LeaveYear
,LeaveType_E
,FromDate
,ToDate
,TotalLeave
,ApprovedBy
,ISNULL(IsApprove,0)IsApprove
,ISNULL(IsReject ,0)IsReject
,IsHalfDay
,EmployeeLeave.Remarks
,case when IsApprove=1 then 'Approved'  WHEN IsReject=1 THEN 'Rejected'  else 'Pending' end as Approval
,ApprovedBy
,case when IsHalfDay=1 then 'Half Day' else 'Full Day' end as DayType
,isnull(eInfo.Code,'') EmpCode,isnull(eInfo.Salutation_E,'') Salutation,isnull(eInfo.MiddleName,'')MiddleName,isnull(eInfo.LastName,'')LastName
FROM EmployeeLeaveSchedule
left outer join employeeInfo eInfo on eInfo.id=EmployeeLeave.EmployeeId
where  EmployeeId=@Id

 ORDER BY FromDate DESC, LeaveType_E
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Transaction = transaction;

                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeaveVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsReject = Convert.ToBoolean(dr["IsReject"]);

                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Approval = dr["Approval"].ToString();
                    vm.DayType = dr["DayType"].ToString();
                    vm.EmpCode = dr["EmpCode"].ToString();
                    vm.EmpName = dr["Salutation"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }
        public List<EmployeeLeaveVM> SelectByEmployeeId(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeLeaveVM vm = new EmployeeLeaveVM();
            List<EmployeeLeaveVM> VMs = new List<EmployeeLeaveVM>();
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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
                #region sql statement
                sqlText = @"SELECT
EmployeeLeave.Id
,EmployeeId
,EmployeeLeaveStructureId
,LeaveYear
,LeaveType_E
,FromDate
,ToDate
,TotalLeave
,ApprovedBy
,ISNULL(IsApprove,0)IsApprove
,ISNULL(IsReject ,0)IsReject
,IsHalfDay
,EmployeeLeave.Remarks
,case when IsApprove=1 then 'Approved'  WHEN IsReject=1 THEN 'Rejected'  else 'Pending' end as Approval
,ApprovedBy
,case when IsHalfDay=1 then 'Half Day' else 'Full Day' end as DayType
,isnull(eInfo.Code,'') EmpCode,isnull(eInfo.Salutation_E,'') Salutation,isnull(eInfo.MiddleName,'')MiddleName,isnull(eInfo.LastName,'')LastName
FROM EmployeeLeave
left outer join employeeInfo eInfo on eInfo.id=EmployeeLeave.EmployeeId
where  EmployeeId=@Id

 ORDER BY FromDate DESC, LeaveType_E
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Transaction = transaction;

                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeaveVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsReject = Convert.ToBoolean(dr["IsReject"]);

                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Approval = dr["Approval"].ToString();
                    vm.DayType = dr["DayType"].ToString();
                    vm.EmpCode = dr["EmpCode"].ToString();
                    vm.EmpName = dr["Salutation"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }
        //==================SelectByEMPID=================
        public EmployeeLeaveVM SelectByEMPId(string empId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeLeaveVM vm = new EmployeeLeaveVM();
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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
                #region sql statement
                sqlText = @"SELECT
EmployeeLeave.Id
,EmployeeId
,EmployeeLeaveStructureId
,LeaveYear
,LeaveType_E
,FromDate
,ToDate
,TotalLeave
,ApprovedBy
,ISNULL(IsApprove,0)IsApprove
,ISNULL(IsReject ,0)IsReject
,IsHalfDay
,EmployeeLeave.Remarks
,case when IsApprove=1 then 'Approved'  WHEN IsReject=1 THEN 'Rejected'  else 'Pending' end as Approval
,ApprovedBy
,case when IsHalfDay=1 then 'Half Day' else 'Full Day' end as DayType
,isnull(eInfo.Code,'') EmpCode,isnull(eInfo.Salutation_E,'') Salutation,isnull(eInfo.MiddleName,'')MiddleName,isnull(eInfo.LastName,'')LastName
FROM EmployeeLeave
left outer join employeeInfo eInfo on eInfo.id=EmployeeLeave.EmployeeId
where  eInfo.id=@Id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Transaction = transaction;

                objComm.Parameters.AddWithValue("@Id", empId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsReject = Convert.ToBoolean(dr["IsApprove"]);

                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Approval = dr["Approval"].ToString();
                    vm.DayType = dr["DayType"].ToString();
                    vm.EmpCode = dr["EmpCode"].ToString();
                    vm.EmpName = dr["Salutation"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                    //employeeLeaveVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    //employeeLeaveVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    //employeeLeaveVM.CreatedBy = dr["CreatedBy"].ToString();
                    //employeeLeaveVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    //employeeLeaveVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    //employeeLeaveVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    //employeeLeaveVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                }
                dr.Close();
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return vm;
        }
        //==================Insert =================
        public string[] Insert(EmployeeLeaveVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertEmployeeLeave"; //Method Name
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                CommonDAL _cDal = new CommonDAL();
                #region HolidayCheck
                SettingDAL _sDAL = new SettingDAL();
                string HolidayCheck = _sDAL.settingValue("Leave", "HolidayCheck", currConn, transaction);
                if (HolidayCheck == "Y")
                {
                    string[] conditionFields = { "HoliDay>", "HoliDay<" };
                    string[] conditionValues = { Ordinary.DateToString(vm.FromDate), Ordinary.DateToString(vm.ToDate) };

                    List<HoliDayVM> VMs = new List<HoliDayVM>();
                    VMs = new HoliDayDAL().SelectAll(conditionFields, conditionValues, currConn, transaction);
                    string holidays = "";
                    foreach (var item in VMs)
                    {
                        holidays += item.HoliDay + ":" + item.HoliDayType;
                    }

                    if (!string.IsNullOrWhiteSpace(holidays))
                    {
                        retResults[1] = "Holidays: " + holidays;
                        return retResults;
                    }
                }
                #endregion

                #region CheckPoint
                if (vm.Id > 0)
                {
                    //Status Check
                    EmployeeLeaveVM newVM = new EmployeeLeaveVM();
                    newVM = SelectById(vm.Id, currConn, transaction);

                    if (newVM.IsApprove)
                    {
                        retResults[1] = "This Leave Already Approved! Can't Update";
                        return retResults;
                    }
                    if (newVM.IsReject)
                    {
                        retResults[1] = "This Leave Already Rejected! Can't Update";
                        return retResults;
                    }

                    {
                        string[] conditionField = { "EmployeeLeaveId" };
                        string[] conditionValue = { vm.Id.ToString() };
                        retResults = _cDal.DeleteTable("EmployeeLeaveDetail", conditionField, conditionValue, currConn, transaction);
                        if (retResults[1] == "Fail")
                        {
                            retResults[1] = "Please Contact Administrator";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }

                    {
                        string[] conditionField = { "Id" };
                        string[] conditionValue = { vm.Id.ToString() };
                        retResults = _cDal.DeleteTable("EmployeeLeave", conditionField, conditionValue, currConn, transaction);
                        if (retResults[1] == "Fail")
                        {
                            retResults[1] = "Please Contact Administrator";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }

                }
                #endregion


                EmployeeInfoDAL _dal = new EmployeeInfoDAL();
                string EmployeeId = vm.EmployeeId;// _dal.SelectEmpForSearch(empLeaveVM.EmployeeId, "current", currConn, transaction).Id;
                sqlText = "select top 1 Id from EMPLOYEELEAVESTRUCTURE where employeeId=@employeeId and leaveyear=@leaveyear and LeaveType_E=@LeaveType_E";
                SqlCommand cmd0 = new SqlCommand(sqlText, currConn);
                cmd0.Parameters.AddWithValue("@employeeId", EmployeeId);
                cmd0.Parameters.AddWithValue("@leaveyear", vm.LeaveYear);
                cmd0.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E);
                cmd0.Transaction = transaction;
                int leaveStructureId = 0;
                SqlDataReader dr;
                dr = cmd0.ExecuteReader();
                while (dr.Read())
                {
                    leaveStructureId = Convert.ToInt32(dr["Id"]);
                }
                dr.Close();
                #region Save
                //int foundId = (int)objfoundId;
                sqlText = "  ";
                sqlText += @" INSERT INTO EmployeeLeave( EmployeeId
,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,IsApprove,IsHalfDay
,IsLWP	
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)
                    VALUES ( @EmployeeId
,@EmployeeLeaveStructureId,@LeaveYear,@LeaveType_E,@FromDate,@ToDate,@TotalLeave,@ApprovedBy,@IsApprove,@IsHalfDay
,@IsLWP	
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom)
                    SELECT SCOPE_IDENTITY()";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdInsert.Parameters.AddWithValue("@EmployeeLeaveStructureId", leaveStructureId);
                cmdInsert.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                cmdInsert.Parameters.AddWithValue("@IsHalfDay", vm.IsHalfDay);
                cmdInsert.Parameters.AddWithValue("@IsLWP", vm.IsLWP);
                cmdInsert.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E);
                cmdInsert.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate));
                if (vm.IsHalfDay)
                {
                    vm.ToDate = vm.FromDate;
                }
                cmdInsert.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate));
                cmdInsert.Parameters.AddWithValue("@TotalLeave", vm.TotalLeave);
                cmdInsert.Parameters.AddWithValue("@ApprovedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@IsApprove", vm.IsApprove);
                cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                var exeRes = cmdInsert.ExecuteScalar();
                Id = Convert.ToInt32(exeRes);
                vm.Id = Id;
                if (Id <= 0)
                {
                    retResults[1] = "Please Input Employee Leave Value";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Input Employee Leave Value", "");
                }
                if (vm.IsApprove)
                {
                    vm.LastUpdateAt = vm.CreatedAt;
                    vm.LastUpdateBy = vm.CreatedBy;
                    vm.LastUpdateFrom = vm.CreatedFrom;
                    retResults = EmployeeLeaveDetail(vm, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        //retResults[1] = "Employee Leave Not Approve";
                        //retResults[3] = sqlText;
                        throw new ArgumentNullException("Employee Leave Not Approve", "");
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
        public string[] InsertSchedule(EmployeeLeaveVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertEmployeeLeave"; //Method Name
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                CommonDAL _cDal = new CommonDAL();
                #region HolidayCheck
                SettingDAL _sDAL = new SettingDAL();
                string HolidayCheck = _sDAL.settingValue("Leave", "HolidayCheck", currConn, transaction);
                if (HolidayCheck == "Y")
                {
                    string[] conditionFields = { "HoliDay>", "HoliDay<" };
                    string[] conditionValues = { Ordinary.DateToString(vm.FromDate), Ordinary.DateToString(vm.ToDate) };

                    List<HoliDayVM> VMs = new List<HoliDayVM>();
                    VMs = new HoliDayDAL().SelectAll(conditionFields, conditionValues, currConn, transaction);
                    string holidays = "";
                    foreach (var item in VMs)
                    {
                        holidays += item.HoliDay + ":" + item.HoliDayType;
                    }

                    if (!string.IsNullOrWhiteSpace(holidays))
                    {
                        retResults[1] = "Holidays: " + holidays;
                        return retResults;
                    }
                }
                #endregion

                #region CheckPoint
                if (vm.Id > 0)
                {
                    //Status Check
                    EmployeeLeaveVM newVM = new EmployeeLeaveVM();
                    newVM = SelectById(vm.Id, currConn, transaction);

                    if (newVM.IsApprove)
                    {
                        retResults[1] = "This Leave Already Approved! Can't Update";
                        return retResults;
                    }
                    if (newVM.IsReject)
                    {
                        retResults[1] = "This Leave Already Rejected! Can't Update";
                        return retResults;
                    }

                    {
                        string[] conditionField = { "EmployeeLeaveId" };
                        string[] conditionValue = { vm.Id.ToString() };
                        retResults = _cDal.DeleteTable("EmployeeLeaveDetailSchedule", conditionField, conditionValue, currConn, transaction);
                        if (retResults[1] == "Fail")
                        {
                            retResults[1] = "Please Contact Administrator";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }

                    {
                        string[] conditionField = { "Id" };
                        string[] conditionValue = { vm.Id.ToString() };
                        retResults = _cDal.DeleteTable("EmployeeLeave", conditionField, conditionValue, currConn, transaction);
                        if (retResults[1] == "Fail")
                        {
                            retResults[1] = "Please Contact Administrator";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }

                }
                #endregion


                EmployeeInfoDAL _dal = new EmployeeInfoDAL();
                string EmployeeId = vm.EmployeeId;// _dal.SelectEmpForSearch(empLeaveVM.EmployeeId, "current", currConn, transaction).Id;
                sqlText = "select top 1 Id from EMPLOYEELEAVESTRUCTURE where employeeId=@employeeId and leaveyear=@leaveyear and LeaveType_E=@LeaveType_E";
                SqlCommand cmd0 = new SqlCommand(sqlText, currConn);
                cmd0.Parameters.AddWithValue("@employeeId", EmployeeId);
                cmd0.Parameters.AddWithValue("@leaveyear", vm.LeaveYear);
                cmd0.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E);
                cmd0.Transaction = transaction;
                int leaveStructureId = 0;
                SqlDataReader dr;
                dr = cmd0.ExecuteReader();
                while (dr.Read())
                {
                    leaveStructureId = Convert.ToInt32(dr["Id"]);
                }
                dr.Close();
                #region Save
                //int foundId = (int)objfoundId;
                sqlText = "  ";
                sqlText += @" INSERT INTO EmployeeLeaveSchedule( EmployeeId
,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,IsApprove,IsHalfDay
,IsLWP	
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)
                    VALUES ( @EmployeeId
,@EmployeeLeaveStructureId,@LeaveYear,@LeaveType_E,@FromDate,@ToDate,@TotalLeave,@ApprovedBy,@IsApprove,@IsHalfDay
,@IsLWP	
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom)
                    SELECT SCOPE_IDENTITY()";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdInsert.Parameters.AddWithValue("@EmployeeLeaveStructureId", leaveStructureId);
                cmdInsert.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                cmdInsert.Parameters.AddWithValue("@IsHalfDay", vm.IsHalfDay);
                cmdInsert.Parameters.AddWithValue("@IsLWP", vm.IsLWP);
                cmdInsert.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E);
                cmdInsert.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate));
                if (vm.IsHalfDay)
                {
                    vm.ToDate = vm.FromDate;
                }
                cmdInsert.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate));
                cmdInsert.Parameters.AddWithValue("@TotalLeave", vm.TotalLeave);
                cmdInsert.Parameters.AddWithValue("@ApprovedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@IsApprove", vm.IsApprove);
                cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                var exeRes = cmdInsert.ExecuteScalar();
                Id = Convert.ToInt32(exeRes);
                vm.Id = Id;
                if (Id <= 0)
                {
                    retResults[1] = "Please Input Employee Leave Value";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Input Employee Leave Value", "");
                }
                if (vm.IsApprove)
                {
                    vm.LastUpdateAt = vm.CreatedAt;
                    vm.LastUpdateBy = vm.CreatedBy;
                    vm.LastUpdateFrom = vm.CreatedFrom;
                    retResults = EmployeeLeaveDetail(vm, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        //retResults[1] = "Employee Leave Not Approve";
                        //retResults[3] = sqlText;
                        throw new ArgumentNullException("Employee Leave Not Approve", "");
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
        public string[] Approve(EmployeeLeaveVM empLeaveVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeLeave"; //Method Name
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                sqlText = "select top 1 * from EmployeeLeave where Id=@Id";
                SqlCommand cmd01 = new SqlCommand(sqlText, currConn);
                cmd01.Parameters.AddWithValue("@Id", empLeaveVM.Id);
                cmd01.Transaction = transaction;
                EmployeeLeaveVM elvm = new EmployeeLeaveVM();
                elvm = empLeaveVM;
                SqlDataReader dr1;
                dr1 = cmd01.ExecuteReader();
                while (dr1.Read())
                {
                    elvm.Id = Convert.ToInt32(dr1["Id"]);
                    elvm.EmployeeLeaveStructureId = Convert.ToInt32(dr1["EmployeeLeaveStructureId"]);
                    elvm.EmployeeId = dr1["EmployeeId"].ToString();
                    elvm.LeaveYear = Convert.ToInt32(dr1["LeaveYear"]);
                    elvm.LeaveType_E = dr1["LeaveType_E"].ToString();
                    elvm.IsHalfDay = Convert.ToBoolean(dr1["IsHalfDay"]);
                    elvm.IsLWP = Convert.ToBoolean(dr1["IsLWP"]);
                    elvm.FromDate = Ordinary.StringToDate(dr1["FromDate"].ToString());
                    elvm.ToDate = Ordinary.StringToDate(dr1["ToDate"].ToString());
                    if (elvm.IsHalfDay)
                    {
                        elvm.ToDate = elvm.FromDate;
                    }
                    elvm.TotalLeave = Convert.ToDecimal(dr1["TotalLeave"]);
                    elvm.Remarks = dr1["Remarks"].ToString();
                    elvm.CreatedAt = dr1["CreatedAt"].ToString();
                    elvm.CreatedBy = dr1["CreatedBy"].ToString();
                    elvm.CreatedFrom = dr1["CreatedFrom"].ToString();
                }
                dr1.Close();
                ////else
                ////Update Approve
                retResults = EmployeeLeaveDetail(elvm, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    //retResults[1] = "Employee Leave Not Approve";
                    //retResults[3] = sqlText;
                    throw new ArgumentNullException("Employee Leave Not Approve", "");
                }

                #region ApprovedLeaveRejectProcess
                if (empLeaveVM.IsReject == true)
                {
                    AttendanceDailyNewVM attnVM = new AttendanceDailyNewVM();
                    attnVM.EmployeeId = elvm.EmployeeId;
                    attnVM.DateFrom = elvm.FromDate;
                    attnVM.DateTo = elvm.ToDate;
                    retResults = new DailyAttendanceProcessDAL().ApprovedLeaveRejectProcess(attnVM, null, null, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], retResults[1]);
                    }
                }

                #endregion

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
                if (empLeaveVM.IsReject != true)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Leave Approved Successfully.";
                    retResults[2] = Id.ToString();
                }
                else
                {
                    retResults[0] = "Success";
                    retResults[1] = "Leave Rejected Successfully.";
                    retResults[2] = Id.ToString();
                }
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
        public string[] LeaveSchedule(EmployeeLeaveVM empLeaveVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertLeaveSchedule"; //Method Name
          
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

                sqlText = "select top 1 * from EmployeeLeave where Id=@Id";
                SqlCommand cmd01 = new SqlCommand(sqlText, currConn);
                cmd01.Parameters.AddWithValue("@Id", empLeaveVM.Id);
                cmd01.Transaction = transaction;
                EmployeeLeaveVM elvm = new EmployeeLeaveVM();
                elvm = empLeaveVM;
                SqlDataReader dr1;
                dr1 = cmd01.ExecuteReader();
                while (dr1.Read())
                {
                    elvm.EmployeeId = dr1["EmployeeId"].ToString();
                    elvm.FromDate = Ordinary.StringToDate(dr1["FromDate"].ToString());
                    elvm.ToDate = Ordinary.StringToDate(dr1["ToDate"].ToString());


                    DateTime elvmFromDate =Convert.ToDateTime(elvm.FromDate);
                    DateTime elvmToDate = Convert.ToDateTime(elvm.ToDate);

                    List<string> dateList = GenerateDateRange(elvmFromDate, elvmToDate);

                    foreach (string formattedDate in dateList)
                    {
                        #region sql LeaveShedule
                        sqlText = "";
                        sqlText = "Insert into LeaveSchedule(EmployeeId,LeaveDate) values(@EmployeeId,@LeaveDate)";
                        #endregion sql LeaveShedule
                        #region Parameters LeaveShedule
                        SqlCommand cmd = new SqlCommand(sqlText, currConn);
                        cmd.Parameters.AddWithValue("@EmployeeId", elvm.EmployeeId);
                        cmd.Parameters.AddWithValue("@LeaveDate", formattedDate);
                        cmd.Transaction = transaction;
                        var exeRes = cmd.ExecuteNonQuery();           
                    }

                }
                dr1.Close();

              
                #endregion Parameters LeaveShedule
                
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
        static List<string> GenerateDateRange(DateTime startDate, DateTime endDate)
        {
            List<string> dateList = new List<string>();

            for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                dateList.Add(currentDate.ToString("dd-MMM-yyyy"));
            }

            return dateList;
        }
        private string[] EmployeeLeaveDetail(EmployeeLeaveVM vm, SqlConnection currConn, SqlTransaction transaction)
        {
            string sqlText = "";
            string[] retResults = new string[6];
            retResults[0] = "Success";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = vm.Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ApproveLeave"; //Method Name
            int Update = 0;
            try
            {
                decimal PrevoiusTotalApprovedLeave = 0;
                decimal TotalleaveDays = 0;
                #region LeaveBalance

                SettingDAL _sDAL = new SettingDAL();
                string HolidayCheck = _sDAL.settingValue("Leave", "HolidayCheck", currConn, transaction);
                bool IsHolyDayLeaveSkip = _sDAL.settingValue("HRM", "IsHolyDayLeaveSkip") == "Y" ? true : false;

                if (vm.IsReject != true)
                {
                    sqlText = "select isnull(sum(totalleave),0) PrevoiusTotalApprovedLeave from EmployeeLeave where    LeaveYear='" + vm.LeaveYear + "' and LeaveType_E ='" + vm.LeaveType_E + "' and id !='" + vm.Id + "' and employeeid='" + vm.EmployeeId + "'  and IsApprove=1";
                    SqlCommand cmdPTAL = new SqlCommand(sqlText, currConn);
                    cmdPTAL.CommandText = sqlText;
                    cmdPTAL.Transaction = transaction;
                    var exeRes = cmdPTAL.ExecuteScalar();
                    PrevoiusTotalApprovedLeave = Convert.ToDecimal(exeRes);
                    sqlText = @"select ISNULL(isnull( LEAVEDAYS,0)+isnull(OpeningLeaveDays,0),0) TotalleaveDays from EmployeeLeaveStructure ";
                    sqlText = sqlText + @" where employeeid='" + vm.EmployeeId + "' and LeaveYear='" + vm.LeaveYear + "' and LeaveType_E='" + vm.LeaveType_E + "' ";
                    SqlCommand cmdLD = new SqlCommand(sqlText, currConn);
                    cmdLD.CommandText = sqlText;
                    cmdLD.Transaction = transaction;
                    SqlDataReader dr1;
                    dr1 = cmdLD.ExecuteReader();
                    while (dr1.Read())
                    {
                        TotalleaveDays = Convert.ToDecimal(dr1["TotalleaveDays"]);
                    }
                    dr1.Close();
                    if (vm.TotalLeave + PrevoiusTotalApprovedLeave > TotalleaveDays)
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "This employee has no enough Leave in banalce";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("This employee has no enough Leave in banalce", "");
                    }
                }
                #endregion LeaveBalance
                int ldays = Convert.ToInt32((Convert.ToDateTime(vm.ToDate) - Convert.ToDateTime(vm.FromDate)).TotalDays) + 1;
                string LType = "";

                #region LeaveType
                if (vm.IsReject != true)
                {
                    sqlText = "select top 1 LType from EnumLeaveType where Name='" + vm.LeaveType_E + "'";
                    SqlCommand cmdFindLT = new SqlCommand();
                    cmdFindLT.Connection = currConn;
                    cmdFindLT.CommandText = sqlText;
                    cmdFindLT.Transaction = transaction;
                    cmdFindLT.CommandType = CommandType.Text;
                    using (SqlDataReader dr2 = cmdFindLT.ExecuteReader())
                    {
                        while (dr2.Read())
                        {
                            LType = dr2["LType"].ToString();
                        }
                        dr2.Close();
                    }
                }
               


                #endregion  LeaveType
                if (!IsHolyDayLeaveSkip)
                {
                    #region Regular Leave Process
                    for (int i = 0; i < ldays; i++)
                    {
                        string LeaveDate = Convert.ToDateTime(vm.FromDate).AddDays(i).ToString("yyyyMMdd");



                        if (HolidayCheck == "Y")
                        {
                            #region Holiday Check
                            if (vm.IsReject != true)
                            {
                                sqlText = "select * from HoliDay where HoliDay='" + LeaveDate + "'";
                                SqlCommand cmdFindH = new SqlCommand();
                                cmdFindH.Connection = currConn;
                                cmdFindH.CommandText = sqlText;
                                cmdFindH.Transaction = transaction;
                                cmdFindH.CommandType = CommandType.Text;
                                using (SqlDataReader dr = cmdFindH.ExecuteReader())
                                {
                                    while (dr.Read())
                                    {
                                        retResults[1] = Convert.ToDateTime(vm.FromDate).AddDays(i).ToString("dd/MMM/yyyy") + " Already Assign as Holiday";
                                        throw new ArgumentNullException(Convert.ToDateTime(vm.FromDate).AddDays(i).ToString("dd/MMM/yyyy") + " Already Assign as Holiday");
                                    }
                                    dr.Close();
                                }
                            }
                            #endregion  Holiday Check
                        }

                        #region Leave Check
                        if (vm.IsReject != true)
                        {
                            sqlText = "SELECT ISNULL(SUM(TotalLeave),0) TotalLeave FROM EmployeeLeaveDetail WHERE 1=1 AND LeaveDate='" + LeaveDate + "'";
                            sqlText += " AND  EmployeeId='" + vm.EmployeeId + "'";
                            sqlText += " AND IsApprove=1 AND IsReject=0";
                            SqlCommand cmdFindL = new SqlCommand();
                            cmdFindL.Connection = currConn;
                            cmdFindL.CommandText = sqlText;
                            cmdFindL.Transaction = transaction;
                            cmdFindL.CommandType = CommandType.Text;
                            using (SqlDataReader dr3 = cmdFindL.ExecuteReader())
                            {
                                while (dr3.Read())
                                {
                                    decimal TotalLeave = Convert.ToDecimal(dr3["TotalLeave"]);
                                    if (TotalLeave >= 1)
                                    {
                                        retResults[0] = "Fail";
                                        retResults[1] = Convert.ToDateTime(vm.FromDate).AddDays(i).ToString("dd/MMM/yyyy") + " Already Assign as Leave";
                                        throw new ArgumentNullException(retResults[1], "");
                                    }
                                }
                                dr3.Close();
                            }
                        }
                        #endregion  Leave Check
                        //Insert or Update (Approve, ReApprove) / Update  (Reject) For From Date
                        //if (i == ldays - 1)
                        //{
                        //goto Result;
                        //Find the LeaveDetail Id First to Update this
                        #region Sql Finding ID
                        int Id = 0;
                        sqlText = "";
                        sqlText = "select Id from EmployeeLeaveDetail";
                        sqlText += " where 1=1";
                        sqlText += " and EmployeeId='" + vm.EmployeeId + "'";
                        sqlText += " and EmployeeLeaveId='" + vm.Id + "'";
                        sqlText += " and LeaveType_E='" + vm.LeaveType_E + "'";
                        sqlText += " and LeaveDate='" + Convert.ToDateTime(vm.FromDate).AddDays(i).ToString("yyyyMMdd") + "'";
                        SqlCommand cmdFind = new SqlCommand();
                        cmdFind.Connection = currConn;
                        cmdFind.CommandText = sqlText;
                        cmdFind.Transaction = transaction;
                        cmdFind.CommandType = CommandType.Text;
                        using (SqlDataReader dr5 = cmdFind.ExecuteReader())
                        {
                            while (dr5.Read())
                            {
                                Id = Convert.ToInt32(dr5["Id"]);
                            }
                            dr5.Close();
                        }
                        #endregion Sql Finding ID
                        if (vm.IsReject != true)
                        {
                            if (Id <= 0)//Insert Approve//If Id Not Exist Make Insert Else Update
                            {
                                #region sql Approve
                                sqlText = @"Insert into EmployeeLeaveDetail
(
EmployeeId
,EmployeeLeaveId
,LeaveYear
,LeaveType_E
,LType
,TotalLeave
,LeaveDate
,ApprovedBy
,IsApprove
,RejectedBy
,RejectDate
,IsReject
,IsHalfDay
,IsLWP
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
) Values (
 @EmployeeId
,@EmployeeLeaveId
,@LeaveYear
,@LeaveType_E
,@LType
,@TotalLeave
,@LeaveDate
,@ApprovedBy
,@IsApprove
,@RejectedBy
,@RejectDate
,@IsReject
,@IsHalfDay
,@IsLWP
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@LastUpdateBy
,@LastUpdateAt
,@LastUpdateFrom
)";
                                #endregion sql Approve
                                #region Parameters Approve
                                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                                cmd.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                                cmd.Parameters.AddWithValue("@EmployeeLeaveId", vm.Id);
                                cmd.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                                cmd.Parameters.AddWithValue("@LType", LType);
                                cmd.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E);
                                cmd.Parameters.AddWithValue("@LeaveDate", Convert.ToDateTime(vm.FromDate).AddDays(i).ToString("yyyyMMdd"));
                                cmd.Parameters.AddWithValue("@ApproveDate", vm.LastUpdateAt);
                                cmd.Parameters.AddWithValue("@ApprovedBy", vm.ApprovedBy);
                                cmd.Parameters.AddWithValue("@IsApprove", true);
                                cmd.Parameters.AddWithValue("@IsReject", false);
                                cmd.Parameters.AddWithValue("@RejectDate", "");
                                cmd.Parameters.AddWithValue("@RejectedBy", "");
                                cmd.Parameters.AddWithValue("@IsHalfDay", vm.IsHalfDay);
                                if (vm.IsHalfDay)
                                {
                                    cmd.Parameters.AddWithValue("@TotalLeave", ".5");
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@TotalLeave", "1");
                                }
                                cmd.Parameters.AddWithValue("@IsLWP", vm.IsLWP);

                                cmd.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                                cmd.Parameters.AddWithValue("@IsActive", true);
                                cmd.Parameters.AddWithValue("@IsArchive", false);
                                cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                                cmd.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                                cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                                cmd.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                                cmd.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                                cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                                var exeRes = cmd.ExecuteNonQuery();
                                Update = Convert.ToInt32(exeRes);
                                #endregion Parameters Approve
                           
                            }
                            else //Update Approve or ReApprove
                            {
                                #region sql ReApprove
                                sqlText = "";
                                sqlText = "update EmployeeLeaveDetail set";
                                sqlText += " IsApprove='1'";
                                sqlText += " ,IsReject='0'";
                                //sqlText += " ,TotalLeave='0'";
                                sqlText += " ,ApproveDate=@ApproveDate";
                                sqlText += " ,ApprovedBy=@ApprovedBy";
                                sqlText += " ,RejectDate=@RejectDate";
                                sqlText += " ,RejectedBy=@RejectedBy";
                                sqlText += " where Id=@Id";
                                #endregion sql ReApprove
                                #region Parameters ReApprove
                                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                                cmd.Parameters.AddWithValue("@Id", Id);
                                cmd.Parameters.AddWithValue("@ApproveDate", vm.LastUpdateAt);
                                cmd.Parameters.AddWithValue("@ApprovedBy", vm.ApprovedBy);
                                cmd.Parameters.AddWithValue("@RejectDate", "");
                                cmd.Parameters.AddWithValue("@RejectedBy", "");
                                cmd.Transaction = transaction;
                                var exeRes = cmd.ExecuteNonQuery();
                                Update = Convert.ToInt32(exeRes);
                                #endregion Parameters ReApprove                           
                            }
                        }
                        else
                        {
                            //Update Approve to Reject for the Same Id
                            #region sql Rejected
                            sqlText = "";
                            sqlText = "update EmployeeLeaveDetail set";
                            sqlText += " IsApprove='0'";
                            sqlText += " ,IsReject='1'";
                            //sqlText += " ,TotalLeave='0'";
                            sqlText += " ,ApproveDate=@ApproveDate";
                            sqlText += " ,ApprovedBy=@ApprovedBy";
                            sqlText += " ,RejectDate=@RejectDate";
                            sqlText += " ,RejectedBy=@RejectedBy";
                            sqlText += " where Id=@Id";
                            #endregion sql Rejected
                            #region Parameters Rejected
                            SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                            cmd.Parameters.AddWithValue("@Id", Id);
                            cmd.Parameters.AddWithValue("@ApproveDate", "");
                            cmd.Parameters.AddWithValue("@ApprovedBy", "");
                            cmd.Parameters.AddWithValue("@RejectDate", vm.LastUpdateAt);
                            cmd.Parameters.AddWithValue("@RejectedBy", vm.RejectedBy);
                            var exeRes = cmd.ExecuteNonQuery();
                            Update = Convert.ToInt32(exeRes);
                            #endregion Parameters Rejected
                        }
                        //}
                    }
                    #endregion 
                }
                else
                {
                    #region HolyDay Skip Leave Process

                    var employeeJob = new EmployeeJobDAL().SelectByEmployee(vm.EmployeeId.ToString(), currConn, transaction);
                    List<DateTime> businessDays = Ordinary.GetBusinessDaysDate(Convert.ToDateTime(vm.FromDate), Convert.ToDateTime(vm.ToDate), employeeJob.FirstHoliday ?? "Friday", employeeJob.SecondHoliday ?? "Friday");
                    List<DateTime> holidays = new HoliDayDAL().SelectAllHoliDate();

                    List<DateTime> filteredDates = businessDays.Where(d => !holidays.Contains(d)).ToList();
                    foreach (var Day in filteredDates)
                    {

                        string LeaveDate = Convert.ToDateTime(Day).ToString("yyyyMMdd");

                        #region Leave Check
                        if (vm.IsReject != true)
                        {
                            sqlText = "SELECT ISNULL(SUM(TotalLeave),0) TotalLeave FROM EmployeeLeaveDetail WHERE 1=1 AND LeaveDate='" + LeaveDate + "'";
                            sqlText += " AND  EmployeeId='" + vm.EmployeeId + "'";
                            sqlText += " AND IsApprove=1 AND IsReject=0";
                            SqlCommand cmdFindL = new SqlCommand();
                            cmdFindL.Connection = currConn;
                            cmdFindL.CommandText = sqlText;
                            cmdFindL.Transaction = transaction;
                            cmdFindL.CommandType = CommandType.Text;
                            using (SqlDataReader dr3 = cmdFindL.ExecuteReader())
                            {
                                while (dr3.Read())
                                {
                                    decimal TotalLeave = Convert.ToDecimal(dr3["TotalLeave"]);
                                    if (TotalLeave >= 1)
                                    {
                                        retResults[0] = "Fail";
                                        retResults[1] = Convert.ToDateTime(Day).ToString("dd/MMM/yyyy") + " Already Assign as Leave";
                                        throw new ArgumentNullException(retResults[1], "");
                                    }
                                }
                                dr3.Close();
                            }
                        }
                        #endregion  Leave Check
                        //Insert or Update (Approve, ReApprove) / Update  (Reject) For From Date
                        //if (i == ldays - 1)
                        //{
                        //goto Result;
                        //Find the LeaveDetail Id First to Update this
                        #region Sql Finding ID
                        int Id = 0;
                        sqlText = "";
                        sqlText = "select Id from EmployeeLeaveDetail";
                        sqlText += " where 1=1";
                        sqlText += " and EmployeeId='" + vm.EmployeeId + "'";
                        sqlText += " and EmployeeLeaveId='" + vm.Id + "'";
                        sqlText += " and LeaveType_E='" + vm.LeaveType_E + "'";
                        sqlText += " and LeaveDate='" + Convert.ToDateTime(Day).ToString("yyyyMMdd") + "'";
                        SqlCommand cmdFind = new SqlCommand();
                        cmdFind.Connection = currConn;
                        cmdFind.CommandText = sqlText;
                        cmdFind.Transaction = transaction;
                        cmdFind.CommandType = CommandType.Text;
                        using (SqlDataReader dr5 = cmdFind.ExecuteReader())
                        {
                            while (dr5.Read())
                            {
                                Id = Convert.ToInt32(dr5["Id"]);
                            }
                            dr5.Close();
                        }
                        #endregion Sql Finding ID
                        if (vm.IsReject != true)
                        {
                            if (Id <= 0)//Insert Approve//If Id Not Exist Make Insert Else Update
                            {
                                #region sql Approve
                                sqlText = @"Insert into EmployeeLeaveDetail
(
EmployeeId
,EmployeeLeaveId
,LeaveYear
,LeaveType_E
,LType
,TotalLeave
,LeaveDate
,ApprovedBy
,IsApprove
,RejectedBy
,RejectDate
,IsReject
,IsHalfDay
,IsLWP
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
) Values (
 @EmployeeId
,@EmployeeLeaveId
,@LeaveYear
,@LeaveType_E
,@LType
,@TotalLeave
,@LeaveDate
,@ApprovedBy
,@IsApprove
,@RejectedBy
,@RejectDate
,@IsReject
,@IsHalfDay
,@IsLWP
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@LastUpdateBy
,@LastUpdateAt
,@LastUpdateFrom
)";
                                #endregion sql Approve
                                #region Parameters Approve
                                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                                cmd.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                                cmd.Parameters.AddWithValue("@EmployeeLeaveId", vm.Id);
                                cmd.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                                cmd.Parameters.AddWithValue("@LType", LType);
                                cmd.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E);
                                cmd.Parameters.AddWithValue("@LeaveDate", Convert.ToDateTime(Day).ToString("yyyyMMdd"));
                                cmd.Parameters.AddWithValue("@ApproveDate", vm.LastUpdateAt);
                                cmd.Parameters.AddWithValue("@ApprovedBy", vm.ApprovedBy);
                                cmd.Parameters.AddWithValue("@IsApprove", true);
                                cmd.Parameters.AddWithValue("@IsReject", false);
                                cmd.Parameters.AddWithValue("@RejectDate", "");
                                cmd.Parameters.AddWithValue("@RejectedBy", "");
                                cmd.Parameters.AddWithValue("@IsHalfDay", vm.IsHalfDay);
                                if (vm.IsHalfDay)
                                {
                                    cmd.Parameters.AddWithValue("@TotalLeave", ".5");
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@TotalLeave", "1");
                                }
                                cmd.Parameters.AddWithValue("@IsLWP", vm.IsLWP);

                                cmd.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                                cmd.Parameters.AddWithValue("@IsActive", true);
                                cmd.Parameters.AddWithValue("@IsArchive", false);
                                cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                                cmd.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                                cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                                cmd.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                                cmd.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                                cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                                var exeRes = cmd.ExecuteNonQuery();
                                Update = Convert.ToInt32(exeRes);
                                #endregion Parameters Approve
                            }
                            else //Update Approve or ReApprove
                            {
                                #region sql ReApprove
                                sqlText = "";
                                sqlText = "update EmployeeLeaveDetail set";
                                sqlText += " IsApprove='1'";
                                sqlText += " ,IsReject='0'";
                                //sqlText += " ,TotalLeave='0'";
                                sqlText += " ,ApproveDate=@ApproveDate";
                                sqlText += " ,ApprovedBy=@ApprovedBy";
                                sqlText += " ,RejectDate=@RejectDate";
                                sqlText += " ,RejectedBy=@RejectedBy";
                                sqlText += " where Id=@Id";
                                #endregion sql ReApprove
                                #region Parameters ReApprove
                                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                                cmd.Parameters.AddWithValue("@Id", Id);
                                cmd.Parameters.AddWithValue("@ApproveDate", vm.LastUpdateAt);
                                cmd.Parameters.AddWithValue("@ApprovedBy", vm.ApprovedBy);
                                cmd.Parameters.AddWithValue("@RejectDate", "");
                                cmd.Parameters.AddWithValue("@RejectedBy", "");
                                cmd.Transaction = transaction;
                                var exeRes = cmd.ExecuteNonQuery();
                                Update = Convert.ToInt32(exeRes);
                                #endregion Parameters ReApprove
                            }
                        }
                        else
                        {
                            //Update Approve to Reject for the Same Id
                            #region sql Rejected
                            sqlText = "";
                            sqlText = "update EmployeeLeaveDetail set";
                            sqlText += " IsApprove='0'";
                            sqlText += " ,IsReject='1'";
                            //sqlText += " ,TotalLeave='0'";
                            sqlText += " ,ApproveDate=@ApproveDate";
                            sqlText += " ,ApprovedBy=@ApprovedBy";
                            sqlText += " ,RejectDate=@RejectDate";
                            sqlText += " ,RejectedBy=@RejectedBy";
                            sqlText += " where Id=@Id";
                            #endregion sql Rejected
                            #region Parameters Rejected
                            SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                            cmd.Parameters.AddWithValue("@Id", Id);
                            cmd.Parameters.AddWithValue("@ApproveDate", "");
                            cmd.Parameters.AddWithValue("@ApprovedBy", "");
                            cmd.Parameters.AddWithValue("@RejectDate", vm.LastUpdateAt);
                            cmd.Parameters.AddWithValue("@RejectedBy", vm.RejectedBy);
                            var exeRes = cmd.ExecuteNonQuery();
                            Update = Convert.ToInt32(exeRes);
                            #endregion Parameters Rejected
                        }
                        //}
                    }
                    #endregion 

                }




                //Result:
                if (vm.IsReject != true)
                {
                    if (Update <= 0)
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "This Leave is not Approved";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("This Leave is not Approved", "");
                    }
                }
                {
                    ////////if (Update <= 0)
                    ////////{
                    ////////    retResults[0] = "Fail";
                    ////////    retResults[1] = "This Leave is not Rejected";
                    ////////    retResults[3] = sqlText;
                    ////////    throw new ArgumentNullException("This Leave is not Rejected", "");
                    ////////}
                }
                #region Update Employee Leave (Master Table)
                sqlText = "";
                sqlText = "update EmployeeLeave set";
                if (vm.IsReject != true)
                {
                    sqlText += " IsApprove='1'";
                    sqlText += " ,IsReject='0'";
                    sqlText += " ,ApprovedBy='" + vm.ApprovedBy + "'";
                    sqlText += " ,ApproveDate='" + vm.LastUpdateAt + "'";
                    sqlText += " ,RejectedBy=''";
                    sqlText += " ,RejectDate=''";
                }
                else
                {
                    sqlText += " IsApprove='0'";
                    sqlText += " ,IsReject='1'";
                    sqlText += " ,ApprovedBy=''";
                    sqlText += " ,ApproveDate=''";
                    sqlText += " ,RejectedBy='" + vm.RejectedBy + "'";
                    sqlText += " ,RejectDate='" + vm.LastUpdateAt + "'";
                }
                sqlText += " ,LastUpdateBy='" + vm.LastUpdateBy + "'";
                sqlText += " ,LastUpdateAt='" + vm.LastUpdateAt + "'";
                sqlText += " ,LastUpdateFrom='" + vm.LastUpdateFrom + "'";
                sqlText += " where Id='" + vm.Id + "'";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                var exeRes1 = cmdUpdate.ExecuteNonQuery();
                int countId = Convert.ToInt32(exeRes1);
                #endregion Update Employee Leave (Master Table)
                if (countId < 0)
                {
                    if (vm.IsReject != true)
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "This Leave is not Approved! System Fail";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("This Leave is not Approved! System Fail", "");
                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "This Leave is not Rejected! System Fail";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("This Leave is not Rejected! System Fail", "");
                    }
                }
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                return retResults;
            }
            finally
            {
                //retResults[0] = "Success";
            }
            return retResults;
        }
        //==================Update =================
        public string[] Update(EmployeeLeaveVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "LeaveUpdate"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToLeave"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EmployeeLeave set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " EmployeeLeaveStructureId=@EmployeeLeaveStructureId,";
                    sqlText += " LeaveYear=@LeaveYear,";
                    sqlText += " LeaveType_E=@LeaveType_E,";
                    sqlText += " FromDate=@FromDate,";
                    sqlText += " ToDate=@ToDate,";
                    sqlText += " TotalLeave=@TotalLeave,";
                    if (vm.IsApprove == true)
                    {
                        sqlText += " ApprovedBy=@ApprovedBy,";
                        sqlText += " IsApprove=@IsApprove,";
                    }
                    sqlText += " IsHalfDay=@IsHalfDay,";
                    sqlText += " IsLWP=@IsLWP,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeLeaveStructureId", vm.EmployeeLeaveStructureId);
                    cmdUpdate.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                    cmdUpdate.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E);
                    cmdUpdate.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate));
                    cmdUpdate.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate));
                    cmdUpdate.Parameters.AddWithValue("@TotalLeave", vm.TotalLeave);
                    if (vm.IsApprove == true)
                    {
                        cmdUpdate.Parameters.AddWithValue("@ApprovedBy", vm.ApprovedBy);
                        cmdUpdate.Parameters.AddWithValue("@IsApprove", vm.IsApprove);
                    }
                    cmdUpdate.Parameters.AddWithValue("@IsHalfDay", vm.IsHalfDay);
                    cmdUpdate.Parameters.AddWithValue("@IsLWP", vm.IsLWP);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", false);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    if (vm.IsApprove)
                    {
                        vm.CreatedAt = vm.LastUpdateAt;
                        vm.CreatedBy = vm.LastUpdateBy;
                        vm.CreatedFrom = vm.LastUpdateFrom;
                        retResults = EmployeeLeaveDetail(vm, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            retResults[1] = "Employee Leave Not Approve";
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Employee Leave Not Approve", "");
                        }
                    }
                    //#region Commit
                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException("Leave Update", empLeaveVM.Degree_E + " could not updated.");
                    //}
                    //#endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Leave Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Leave.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";
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
        //==================Update Leave Balance =================
        public string[] UpdateLeaveBalance(EmployeeLeaveVM empLeaveVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "LeaveUpdate"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToLeave"); }
                #endregion open connection and transaction
                if (empLeaveVM != null)
                {
                    #region Update Settings
                    foreach (var item in empLeaveVM.EmployeeLeaveStructures)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeLeaveStructure set";
                        sqlText += " OpeningLeaveDays=@OpeningLeaveDays,";
                        sqlText += " LeaveDays=@LeaveDays,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        sqlText += " and LeaveYear=@LeaveYear";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", item.Id);
                        cmdUpdate.Parameters.AddWithValue("@LeaveDays", item.LeaveDays);
                        cmdUpdate.Parameters.AddWithValue("@OpeningLeaveDays", item.OpeningDays);
                        cmdUpdate.Parameters.AddWithValue("@LeaveYear", item.LeaveYear);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", empLeaveVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", empLeaveVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", empLeaveVM.LastUpdateFrom);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        retResults[2] = empLeaveVM.Id.ToString();// Return Id
                        retResults[3] = sqlText; //  SQL Query
                        iSTransSuccess = true;
                    }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Leave Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Leave.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";
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
        //==================Update Leave Balance =================
        public string[] UpdateLeaveStructureBalance(EmployeeLeaveStructureVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "LeaveStructureUpdate"; //Method Name
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
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EmployeeLeaveStructure set";
                    //sqlText += " EmployeeId=@EmployeeId";
                    sqlText += " LeaveStructureId=@LeaveStructureId";
                    sqlText += " ,LeaveYear=@LeaveYear";
                    sqlText += " ,LeaveType_E=@LeaveType_E";
                    sqlText += " ,LeaveDays=@LeaveDays";
                    sqlText += " ,OpeningLeaveDays=@OpeningLeaveDays";
                    sqlText += " ,IsEarned=@IsEarned";
                    sqlText += " ,IsCompensation=@IsCompensation";
                    sqlText += " ,IsCarryForward=@IsCarryForward";
                    sqlText += " ,MaxBalance=@MaxBalance";
                    sqlText += " ,Remarks=@Remarks";
                    sqlText += " ,IsActive=@IsActive";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    //cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@LeaveStructureId", vm.LeaveStructureId);
                    cmdUpdate.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                    cmdUpdate.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LeaveDays", vm.LeaveDays);
                    cmdUpdate.Parameters.AddWithValue("@OpeningLeaveDays", vm.OpeningLeaveDays);
                    cmdUpdate.Parameters.AddWithValue("@IsEarned", vm.IsEarned);
                    cmdUpdate.Parameters.AddWithValue("@IsCompensation", vm.IsCompensation);
                    cmdUpdate.Parameters.AddWithValue("@IsCarryForward", vm.IsCarryForward);
                    cmdUpdate.Parameters.AddWithValue("@MaxBalance", vm.MaxBalance);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", false);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Leave Structure Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Leave Structure.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";
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
        public EmployeeLeaveVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeLeaveVM employeeLeaveVM = new EmployeeLeaveVM();
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
,EmployeeLeaveStructureId
,LeaveYear
,LeaveType_E
,FromDate
,ToDate
,TotalLeave
,ApprovedBy
,IsApprove
,IsHalfDay
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
FROM EmployeeLeave    
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
                        employeeLeaveVM.Id = Convert.ToInt32(dr["Id"]);
                        employeeLeaveVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeeLeaveVM.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                        employeeLeaveVM.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                        employeeLeaveVM.LeaveType_E = dr["LeaveType_E"].ToString();
                        employeeLeaveVM.FromDate = dr["FromDate"].ToString();
                        employeeLeaveVM.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                        employeeLeaveVM.ApprovedBy = dr["ApprovedBy"].ToString();
                        employeeLeaveVM.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                        employeeLeaveVM.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                        employeeLeaveVM.Remarks = dr["Remarks"].ToString();
                        employeeLeaveVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeeLeaveVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                        employeeLeaveVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeeLeaveVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeeLeaveVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeeLeaveVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeeLeaveVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
            return employeeLeaveVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeeLeaveVM employeeLeaveVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "LeaveDelete"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToLeave"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (employeeLeaveVM.Id > 0)
                {
                    #region Delete Settings
                    sqlText = "";
                    sqlText = "delete EmployeeLeave  ";

                    sqlText += " where isnull(IsApprove,0)=0 and isnull(IsReject,0)=0 and  Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", employeeLeaveVM.Id);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = employeeLeaveVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        retResults[1] = "Leave Already Approved /Reject (could not Delete.)";

                        throw new ArgumentNullException(retResults[1], retResults[1]);
                    }
                    #endregion Commit
                    #endregion Delete Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Leave Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Leave.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";
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
        public string[] DeleteArchive(EmployeeLeaveVM employeeLeaveVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "LeaveDelete"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToLeave"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (employeeLeaveVM.Id > 0)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = " Update EmployeeLeave set";
                    sqlText += " IsApprove=0";
                    sqlText += " ,IsArchive=@IsArchive";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where  Id=@Id";


                    sqlText = " update EmployeeLeaveDetail set";
                    sqlText += " TotalLeave=0";
                    sqlText += " ,IsApprove=0";
                    sqlText += " ,IsArchive=@IsArchive";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";

                    sqlText += " where  EmployeeLeaveId=@Id";

                    //employeeLeaveVM.IsApprove

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", employeeLeaveVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeLeaveVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeLeaveVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeLeaveVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = employeeLeaveVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        //retResults[1] = "Leave Already Approved /Reject (could not Delete.)";

                        //throw new ArgumentNullException(retResults[1], retResults[1]);
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Leave Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Leave.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";
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





        public bool ImportExcelFile(string fileName, EmployeeLeaveVM vm)
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                 "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                                 "Excel 12.0;HDR=YES;" + "\"";
            OleDbConnection theConnection = new OleDbConnection(connectionString);
            try
            {
                theConnection.Open();
                OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [DataSheet$]", theConnection);
                DataSet dt = new DataSet();
                da.Fill(dt);
                var a = "";
                EmployeeLeaveStructureVM el = new EmployeeLeaveStructureVM();
                List<EmployeeLeaveStructureVM> els = new List<EmployeeLeaveStructureVM>();// = new EmployeeLeaveVM();
                foreach (DataRow item in dt.Tables[0].Rows)
                {
                    el = new EmployeeLeaveStructureVM();
                    el.Id = Convert.ToInt32(item["Id"].ToString());
                    el.LeaveYear = Convert.ToInt32(item["LeaveYear"].ToString());
                    el.LeaveDays = Convert.ToDecimal(item["LeaveDays"].ToString());
                    el.OpeningDays = Convert.ToDecimal(item["OpeningLeaveDays"].ToString());
                    els.Add(el);
                }
                vm.EmployeeLeaveStructures = els;
                string[] retResults = new string[6];
                retResults = UpdateLeaveBalance(vm, null, null);
                if (retResults[0] == "Fail")
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                theConnection.Close();
            }
            return true;
        }
        public bool ExportExcelFile(string Filepath, string FileName, string vyear)
        {
            try
            {
                Application app = new Application();
                _Workbook workbook = app.Workbooks.Add(System.Type.Missing);
                _Worksheet worksheet = new Worksheet();
                app.Visible = false;
                worksheet = workbook.Sheets["Sheet1"] as _Worksheet;
                worksheet = workbook.ActiveSheet as _Worksheet;
                worksheet.Name = "DataSheet";
                worksheet.Cells[1, 1] = "Id";
                worksheet.Cells[1, 2] = "EmpCode";
                worksheet.Cells[1, 3] = "EmpName";
                worksheet.Cells[1, 4] = "LeaveYear";
                worksheet.Cells[1, 5] = "LeaveType";
                worksheet.Cells[1, 6] = "LeaveDays";
                worksheet.Cells[1, 7] = "OpeningLeaveDays";
                worksheet.Cells[1, 8] = "Earned";
                worksheet.Cells[1, 9] = "Compensation";
                worksheet.Cells[1, 10] = "MaxBalance";
                #region DataRead From DB
                #region Variables
                SqlConnection currConn = null;
                string sqlText = "";
                int j = 2;
                #endregion
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"select els.Id
,els.EmployeeId,einfo.Code EmpCode
,einfo.Salutation_E  +' '+ einfo.MiddleName+' '+einfo.LastName EmpName,els.LeaveStructureId,els.LeaveYear
,els.LeaveType_E LeaveType,els.LeaveDays,els.OpeningLeaveDays,els.IsEarned Earned,els.IsCompensation Compensation,els.MaxBalance
 from EmployeeLeaveStructure els
left outer join employeeinfo einfo on els.employeeid=einfo.id
and einfo.IsArchive=0
";
                if (!string.IsNullOrWhiteSpace(vyear))
                {
                    sqlText += " where els.LeaveYear='" + vyear + "'";
                }
                sqlText += " order by els.LeaveYear,einfo.code,els.LeaveType_E";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);
                foreach (DataRow item in dt.Rows)
                {
                    worksheet.Cells[j, 1] = item["Id"].ToString();//"Id";
                    worksheet.Cells[j, 2] = item["EmpCode"].ToString();//"EmpCode";
                    worksheet.Cells[j, 3] = item["EmpName"].ToString();//"EmpName";
                    worksheet.Cells[j, 4] = item["LeaveYear"].ToString();//"LeaveYear";
                    worksheet.Cells[j, 5] = item["LeaveType"].ToString();//"LeaveType";
                    worksheet.Cells[j, 6] = item["LeaveDays"].ToString();//"LeaveDays";
                    worksheet.Cells[j, 7] = item["OpeningLeaveDays"].ToString();//"OpeningLeaveDays";
                    worksheet.Cells[j, 8] = item["Earned"].ToString();//"Earned";
                    worksheet.Cells[j, 9] = item["Compensation"].ToString(); //"Compensation";
                    worksheet.Cells[j, 10] = item["MaxBalance"].ToString(); //"Compensation";
                    j++;
                }
                //SqlDataReader dr;
                //dr = objComm.ExecuteReader();
                //while (dr.Read())
                //{
                //    worksheet.Cells[j, 1] = "Id";
                //    worksheet.Cells[j, 2] = "EmpCode";
                //    worksheet.Cells[j, 3] = "EmpName";
                //    worksheet.Cells[j, 4] = "LeaveYear";
                //    worksheet.Cells[j, 5] = "LeaveType";
                //    worksheet.Cells[j, 6] = "LeaveDays";
                //    worksheet.Cells[j, 7] = "OpeningLeaveDays";
                //    worksheet.Cells[j,8] = "Earned";
                //    worksheet.Cells[j, 9] = "Compensation";
                //    worksheet.Cells[j, 10] = "MaxBalance";
                //    //worksheet.Cells[j, 1] = Convert.ToInt32(dr["Id"]);
                //    //worksheet.Cells[j, 2] = dr["Name"].ToString();
                //    j++;
                //}
                //dr.Close();
                #endregion
                #endregion
                string xportFileName = string.Format(@"{0}" + FileName, Filepath);
                // save the application
                workbook.SaveAs(xportFileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing,
                                Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing);
                // Exit from the application
                app.Quit();
                releaseObject(worksheet);
                releaseObject(workbook);
                releaseObject(app);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        //public bool ExportExcelFileback(System.Data.DataTable dt, string Filepath, string FileName)
        //{
        //    _Application app = new Application();
        //    _Workbook workbook = app.Workbooks.Add(System.Type.Missing);
        //    _Worksheet worksheet = new Worksheet();
        //    app.Visible = false;
        //    worksheet = workbook.Sheets["Sheet1"] as _Worksheet;
        //    worksheet = workbook.ActiveSheet as _Worksheet;
        //    worksheet.Name = "DataSheet";
        //    #region DataRead From DB
        //    int startRow = 1;
        //    int colnum = 1;
        //    string data = null;
        //    int i = 0;
        //    int j = 0;
        //    foreach (DataColumn column in dt.Columns)
        //    {
        //        worksheet.Cells[startRow, colnum] = column.ColumnName;
        //        colnum++;
        //    }
        //    for (i = 0; i <= dt.Rows.Count - 1; i++)
        //    {
        //        for (j = 0; j <= dt.Columns.Count - 1; j++)
        //        {
        //            data = dt.Rows[i].ItemArray[j].ToString();
        //            worksheet.Cells[startRow + 1, j + 1] = data;
        //        }
        //        startRow++;
        //    }
        //    #endregion
        //    string xportFileName = string.Format(@"{0}" + FileName, Filepath);
        //    // save the application
        //    workbook.SaveAs(xportFileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //                    Type.Missing,
        //                    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing,
        //                    Type.Missing, Type.Missing, Type.Missing);
        //    // Exit from the application
        //    app.Quit();
        //    releaseObject(worksheet);
        //    releaseObject(workbook);
        //    releaseObject(app);
        //    return true;
        //}
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
            }
            finally
            {
                GC.Collect();
            }
        }



        public System.Data.DataTable Report(EmployeeLeaveVM vm)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            System.Data.DataTable dt = new System.Data.DataTable();
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
es.EmployeeId
,es.Id
,es.LeaveType_E  LeaveType
,ISNULL(es.OpeningLeaveDays,0)OpeningLeaveDays
,ISNULL( es.LeaveDays,0)LeaveDays
,ISNULL(el.Leave,0) Used
,ISNULL(ISNULL( es.LeaveDays,0)+ISNULL(es.OpeningLeaveDays,0)-ISNULL(el.Leave,0),0) Have
,ISNULL(elt.IsRegular,0)IsRegular
FROM EmployeeLeaveStructure es
LEFT OUTER JOIN 
(
SELECT EmployeeLeaveStructureId,SUM(TotalLeave) Leave 
FROM EmployeeLeave where IsApprove=1  
GROUP BY EmployeeLeaveStructureId
) el ON el.EmployeeLeaveStructureId=es.Id
LEFT OUTER JOIN EmployeeInfo ei on ei.Id=es.EmployeeId
LEFT OUTER JOIN EnumLeaveType elt on es.LeaveType_E=elt.Name
WHERE 1=1     
AND es.LeaveYear=@LeaveYear
AND ei.isActive=1 
 --AND es.EmployeeId='1_1'
";
                if (vm.IsRegular == true)
                {
                    sqlText += " AND elt.IsRegular=1";
                }
                else if (vm.IsRegular == false)
                {
                    sqlText += " AND isnull(elt.IsRegular,0)=0";
                }

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


                sqlText += " ORDER By ei.Code";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);


                da.Fill(dt);

                //string[] dateColumnChange = { "TransferDate" };
                //dt = Ordinary.DtMultiColumnStringToDate(dt, dateColumnChange);
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

