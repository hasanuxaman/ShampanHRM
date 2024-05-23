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
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymServices.Leave
{
    public class EmployeeCompensatoryLeaveDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        #region Methods
        //==================SelectAll=================
        public List<EmployeeCompensatoryLeaveVM> SelectAll(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeCompensatoryLeaveVM> VMs = new List<EmployeeCompensatoryLeaveVM>();
            EmployeeCompensatoryLeaveVM vm;
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
,l.LeaveYear
,l.LeaveType_E
,l.FromDate
,l.ToDate
,l.TotalLeave
,l.ApprovedBy
,ISNULL(l.IsApprove,0)IsApprove
,ISNULL(l.IsReject,0) IsReject
,l.IsHalfDay
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
    FROM EmployeeCompensatoryLeave l
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
                    vm = new EmployeeCompensatoryLeaveVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Supervisor = dr["Supervisor"].ToString();
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsReject = Convert.ToBoolean(dr["IsReject"]);
                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
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
        //        public List<EmployeeCompensatoryLeaveBalanceVM> EmployeeCompensatoryLeaveBalance(string EmployeeId, string leaveyear = "0")
        //        {
        //            #region Variables
        //            SqlConnection currConn = null;
        //            string sqlText = "";
        //            List<EmployeeCompensatoryLeaveBalanceVM> EmployeeCompensatoryLeaves = new List<EmployeeCompensatoryLeaveBalanceVM>();
        //            EmployeeCompensatoryLeaveBalanceVM EmployeeCompensatoryLeave;
        //            #endregion
        //            try
        //            {
        //                #region open connection and transaction
        //                currConn = _dbsqlConnection.GetConnection();
        //                if (currConn.State != ConnectionState.Open)
        //                {
        //                    currConn.Open();
        //                }
        //                #endregion open connection and transaction
        //                #region sql statement
        //                sqlText = @"SELECT ID,ES.LEAVETYPE_E ,isnull(es.OpeningLeaveDays,0)OpeningLeaveDays,isnull( ES.LEAVEDAYS,0)LEAVEDAYS
        //,ISNULL(EL.LEAVE,0) USED,ISNULL(isnull( ES.LEAVEDAYS,0)+isnull(es.OpeningLeaveDays,0)-ISNULL(EL.LEAVE,0),0) HAVE
        //FROM EmployeeCompensatoryLeaveSTRUCTURE ES
        //LEFT OUTER JOIN (
        //SELECT EmployeeCompensatoryLeaveSTRUCTUREID,SUM(TOTALLEAVE) LEAVE FROM EmployeeCompensatoryLeave where IsApprove=1 and IsReject = 0  GROUP BY EmployeeCompensatoryLeaveSTRUCTUREID
        //) EL ON EL.EmployeeCompensatoryLeaveSTRUCTUREID=ES.ID
        //WHERE ES.EMPLOYEEID=@EmployeeId
        //and ES.LeaveYear=@LeaveYear
        //    ORDER BY ES.LEAVETYPE_E
        //";
        //                SqlCommand objComm = new SqlCommand();
        //                objComm.Connection = currConn;
        //                objComm.CommandText = sqlText;
        //                objComm.CommandType = CommandType.Text;
        //                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
        //                objComm.Parameters.AddWithValue("@LeaveYear", leaveyear);
        //                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
        //                //dataAdapter.Fill(dataSet);
        //                SqlDataReader dr;
        //                dr = objComm.ExecuteReader();
        //                while (dr.Read())
        //                {
        //                    EmployeeCompensatoryLeave = new EmployeeCompensatoryLeaveBalanceVM();
        //                    EmployeeCompensatoryLeave.LeaveType = dr["LEAVETYPE_E"].ToString();
        //                    EmployeeCompensatoryLeave.OpeningBalance = dr["OpeningLeaveDays"].ToString();
        //                    EmployeeCompensatoryLeave.Total = dr["LEAVEDAYS"].ToString();
        //                    EmployeeCompensatoryLeave.Used = dr["USED"].ToString();
        //                    EmployeeCompensatoryLeave.Have = dr["HAVE"].ToString();
        //                    EmployeeCompensatoryLeaves.Add(EmployeeCompensatoryLeave);
        //                }
        //                #endregion
        //            }
        //            #region catch
        //            catch (SqlException sqlex)
        //            {
        //                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
        //            }
        //            #endregion
        //            #region finally
        //            finally
        //            {
        //                if (currConn != null)
        //                {
        //                    if (currConn.State == ConnectionState.Open)
        //                    {
        //                        currConn.Close();
        //                    }
        //                }
        //            }
        //            #endregion
        //            return EmployeeCompensatoryLeaves;
        //        }
        //==================SelectAllForSupervisor=================
        public List<EmployeeCompensatoryLeaveVM> SelectAllForSupervisor(string SupervisorId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeCompensatoryLeaveVM> VMs = new List<EmployeeCompensatoryLeaveVM>();
            EmployeeCompensatoryLeaveVM vm;
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

,l.LeaveYear
,l.LeaveType_E
,l.FromDate
,l.ToDate
,l.TotalLeave
,ISNULL(l.IsApprove,0)IsApprove
,ISNULL(l.IsReject ,0)IsReject
,l.IsHalfDay
,l.IsHalfDay
,CASE WHEN l.IsApprove=1 THEN 'Approved'  WHEN l.IsReject=1 THEN 'Rejected'  ELSE 'Pending' END AS Approval
,CASE WHEN l.IsHalfDay=1 THEN 'Half Day' ELSE 'Full Day' END AS DayType
,l.ApprovedBy
,ISNULL(eInfo.Code,'') EmpCode,isnull(empname,'')EmpName
    FROM EmployeeCompensatoryLeave l
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
                sqlText += " ORDER BY l.FromDate";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                //if (!string.IsNullOrWhiteSpace(empVM.Code))
                //{
                objComm.Parameters.AddWithValue("@Code", empVM.Code);
                //}
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeCompensatoryLeaveVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Supervisor = dr["Supervisor"].ToString();
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsReject = Convert.ToBoolean(dr["IsReject"]);

                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
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
        public List<EmployeeCompensatoryLeaveVM> SelectAll(string code, string status)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeCompensatoryLeaveVM> EmployeeCompensatoryLeaves = new List<EmployeeCompensatoryLeaveVM>();
            EmployeeCompensatoryLeaveVM EmployeeCompensatoryLeave;
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
,l.LeaveYear
,l.LeaveType_E
,l.FromDate
,l.ToDate
,l.TotalLeave
,ISNULL(l.IsApprove,0)IsApprove
,ISNULL(l.IsReject ,0)IsReject
,l.IsHalfDay
,l.IsHalfDay
,CASE WHEN l.IsApprove=1 THEN 'Approved' WHEN l.IsReject=1 THEN 'Rejected' ELSE 'Pending' END AS Approval
,CASE WHEN l.IsHalfDay=1 THEN 'Half Day' ELSE 'Full Day' END AS DayType
,l.ApprovedBy
,ISNULL(eInfo.Code,'') EmpCode,isnull(empname,'')EmpName
    FROM EmployeeCompensatoryLeave l
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
                    EmployeeCompensatoryLeave = new EmployeeCompensatoryLeaveVM();
                    EmployeeCompensatoryLeave.Id = Convert.ToInt32(dr["Id"]);
                    EmployeeCompensatoryLeave.EmployeeId = dr["EmployeeId"].ToString();
                    EmployeeCompensatoryLeave.Supervisor = dr["Supervisor"].ToString();
                    EmployeeCompensatoryLeave.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    EmployeeCompensatoryLeave.LeaveType_E = dr["LeaveType_E"].ToString();
                    EmployeeCompensatoryLeave.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    EmployeeCompensatoryLeave.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    EmployeeCompensatoryLeave.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    EmployeeCompensatoryLeave.ApprovedBy = dr["ApprovedBy"].ToString();
                    EmployeeCompensatoryLeave.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    EmployeeCompensatoryLeave.IsReject = Convert.ToBoolean(dr["IsReject"]);

                    EmployeeCompensatoryLeave.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    EmployeeCompensatoryLeave.Approval = dr["Approval"].ToString();
                    EmployeeCompensatoryLeave.DayType = dr["DayType"].ToString();
                    EmployeeCompensatoryLeave.EmpCode = dr["EmpCode"].ToString();
                    EmployeeCompensatoryLeave.EmpName = dr["EmpName"].ToString();
                    EmployeeCompensatoryLeaves.Add(EmployeeCompensatoryLeave);
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
            return EmployeeCompensatoryLeaves;
        }
        //        public List<EmployeeCompensatoryLeaveStructureVM> SelectAllOpening(string empcode, string vyear)
        //        {
        //            #region Variables
        //            SqlConnection currConn = null;
        //            string sqlText = "";
        //            List<EmployeeCompensatoryLeaveStructureVM> EmployeeCompensatoryLeaves = new List<EmployeeCompensatoryLeaveStructureVM>();
        //            EmployeeCompensatoryLeaveStructureVM EmployeeCompensatoryLeave;
        //            #endregion
        //            try
        //            {
        //                #region open connection and transaction
        //                currConn = _dbsqlConnection.GetConnection();
        //                if (currConn.State != ConnectionState.Open)
        //                {
        //                    currConn.Open();
        //                }
        //                #endregion open connection and transaction
        //                #region sql statement
        //                sqlText = @"select els.Id
        //,els.EmployeeId
        //,einfo.Code EmpCode
        //,einfo.Salutation_E Salutation
        //,einfo.MiddleName
        //,einfo.LastName
        //,els.LeaveStructureId
        //,els.LeaveYear
        //,els.LeaveType_E
        //,isnull(els.LeaveDays,0)LeaveDays
        //,isnull(els.OpeningLeaveDays,0)OpeningLeaveDays
        //,els.IsEarned
        //,els.IsCompensation
        //,els.MaxBalance
        // from EmployeeCompensatoryLeaveStructure els
        //left outer join employeeinfo einfo on els.employeeid=einfo.id
        //and einfo.IsArchive=0
        //";
        //                if (!string.IsNullOrWhiteSpace(vyear))
        //                {
        //                    sqlText += " where els.LeaveYear='" + vyear + "'";
        //                }
        //                if (!string.IsNullOrWhiteSpace(empcode))
        //                {
        //                    sqlText += " and einfo.Code='" + empcode + "'";
        //                }
        //                sqlText += " order by els.LeaveYear,einfo.code,els.LeaveType_E";
        //                SqlCommand objComm = new SqlCommand();
        //                objComm.Connection = currConn;
        //                objComm.CommandText = sqlText;
        //                objComm.CommandType = CommandType.Text;
        //                SqlDataReader dr;
        //                dr = objComm.ExecuteReader();
        //                while (dr.Read())
        //                {
        //                    EmployeeCompensatoryLeave = new EmployeeCompensatoryLeaveStructureVM();
        //                    EmployeeCompensatoryLeave.Id = Convert.ToInt32(dr["Id"]);
        //                    EmployeeCompensatoryLeave.EmployeeId = dr["EmployeeId"].ToString();
        //                    EmployeeCompensatoryLeave.EmpCode = dr["EmpCode"].ToString();
        //                    EmployeeCompensatoryLeave.EmpName = dr["Salutation"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
        //                    EmployeeCompensatoryLeave.LeaveStructureId = Convert.ToInt32(dr["LeaveStructureId"]);
        //                    EmployeeCompensatoryLeave.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
        //                    EmployeeCompensatoryLeave.LeaveType_E = dr["LeaveType_E"].ToString();
        //                    EmployeeCompensatoryLeave.LeaveDays = Convert.ToDecimal(dr["LeaveDays"]);
        //                    EmployeeCompensatoryLeave.OpeningDays = Convert.ToDecimal(dr["OpeningLeaveDays"]);
        //                    EmployeeCompensatoryLeave.MaxBalance = Convert.ToDecimal(dr["MaxBalance"]);
        //                    EmployeeCompensatoryLeave.IsEarned = Convert.ToBoolean(dr["IsEarned"]);
        //                    EmployeeCompensatoryLeave.IsCompensation = Convert.ToBoolean(dr["IsCompensation"]);
        //                    EmployeeCompensatoryLeaves.Add(EmployeeCompensatoryLeave);
        //                }
        //                #endregion
        //            }
        //            #region catch
        //            catch (SqlException sqlex)
        //            {
        //                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
        //            }
        //            #endregion
        //            #region finally
        //            finally
        //            {
        //                if (currConn != null)
        //                {
        //                    if (currConn.State == ConnectionState.Open)
        //                    {
        //                        currConn.Close();
        //                    }
        //                }
        //            }
        //            #endregion
        //            return EmployeeCompensatoryLeaves;
        //        }
        public EmployeeCompensatoryLeaveVM SelectById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeCompensatoryLeaveVM vm = new EmployeeCompensatoryLeaveVM();
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
EmployeeCompensatoryLeave.Id
,EmployeeId
,LeaveYear
,LeaveType_E
,FromDate
,ToDate
,TotalLeave
,ApprovedBy
,ISNULL(IsApprove,0)IsApprove
,ISNULL(IsReject ,0)IsReject
,IsHalfDay
,EmployeeCompensatoryLeave.Remarks
,CASE WHEN IsApprove=1 THEN 'Approved' WHEN IsReject=1 THEN 'Rejected' ELSE 'Pending' END AS Approval
,CASE WHEN IsHalfDay=1 THEN 'Half Day' ELSE 'Full Day' END AS DayType
,ApprovedBy
,isnull(eInfo.Code,'') EmpCode,isnull(eInfo.Salutation_E,'') Salutation,isnull(eInfo.MiddleName,'')MiddleName,isnull(eInfo.LastName,'')LastName
FROM EmployeeCompensatoryLeave
left outer join employeeInfo eInfo on eInfo.id=EmployeeCompensatoryLeave.EmployeeId
where  EmployeeCompensatoryLeave.id=@Id
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
                    //EmployeeCompensatoryLeaveVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    //EmployeeCompensatoryLeaveVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    //EmployeeCompensatoryLeaveVM.CreatedBy = dr["CreatedBy"].ToString();
                    //EmployeeCompensatoryLeaveVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    //EmployeeCompensatoryLeaveVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    //EmployeeCompensatoryLeaveVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    //EmployeeCompensatoryLeaveVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        public List<EmployeeCompensatoryLeaveVM> SelectByEmployeeId(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeCompensatoryLeaveVM vm = new EmployeeCompensatoryLeaveVM();
            List<EmployeeCompensatoryLeaveVM> VMs = new List<EmployeeCompensatoryLeaveVM>();
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
EmployeeCompensatoryLeave.Id
,EmployeeId
,LeaveYear
,LeaveType_E
,FromDate
,ToDate
,TotalLeave
,ApprovedBy
,ISNULL(IsApprove,0)IsApprove
,ISNULL(IsReject ,0)IsReject
,IsHalfDay
,EmployeeCompensatoryLeave.Remarks
,case when IsApprove=1 then 'Approved'  WHEN IsReject=1 THEN 'Rejected'  else 'Pending' end as Approval
,ApprovedBy
,case when IsHalfDay=1 then 'Half Day' else 'Full Day' end as DayType
,isnull(eInfo.Code,'') EmpCode,isnull(eInfo.Salutation_E,'') Salutation,isnull(eInfo.MiddleName,'')MiddleName,isnull(eInfo.LastName,'')LastName
FROM EmployeeCompensatoryLeave
left outer join employeeInfo eInfo on eInfo.id=EmployeeCompensatoryLeave.EmployeeId
where  EmployeeId=@Id
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
                    vm = new EmployeeCompensatoryLeaveVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
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
        public EmployeeCompensatoryLeaveVM SelectByEMPId(string empId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeCompensatoryLeaveVM vm = new EmployeeCompensatoryLeaveVM();
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
EmployeeCompensatoryLeave.Id
,EmployeeId
,EmployeeCompensatoryLeaveStructureId
,LeaveYear
,LeaveType_E
,FromDate
,ToDate
,TotalLeave
,ApprovedBy
,ISNULL(IsApprove,0)IsApprove
,ISNULL(IsReject ,0)IsReject
,IsHalfDay
,EmployeeCompensatoryLeave.Remarks
,case when IsApprove=1 then 'Approved'  WHEN IsReject=1 THEN 'Rejected'  else 'Pending' end as Approval
,ApprovedBy
,case when IsHalfDay=1 then 'Half Day' else 'Full Day' end as DayType
,isnull(eInfo.Code,'') EmpCode,isnull(eInfo.Salutation_E,'') Salutation,isnull(eInfo.MiddleName,'')MiddleName,isnull(eInfo.LastName,'')LastName
FROM EmployeeCompensatoryLeave
left outer join employeeInfo eInfo on eInfo.id=EmployeeCompensatoryLeave.EmployeeId
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
                    //EmployeeCompensatoryLeaveVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    //EmployeeCompensatoryLeaveVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    //EmployeeCompensatoryLeaveVM.CreatedBy = dr["CreatedBy"].ToString();
                    //EmployeeCompensatoryLeaveVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    //EmployeeCompensatoryLeaveVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    //EmployeeCompensatoryLeaveVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    //EmployeeCompensatoryLeaveVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        public string[] Insert(EmployeeCompensatoryLeaveVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertEmployeeCompensatoryLeave"; //Method Name
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


                #region CheckPoint
                if (vm.Id > 0)
                {
                    //Status Check
                    EmployeeCompensatoryLeaveVM newVM = new EmployeeCompensatoryLeaveVM();
                    newVM = SelectById(vm.Id, currConn, transaction);

                    if (newVM.IsApprove)
                    {
                        retResults[1] = "This Compensatory Leave Already Approved! Can't Update";
                        return retResults;
                    }
                    if (newVM.IsReject)
                    {
                        retResults[1] = "This Compensatory Leave Already Rejected! Can't Update";
                        return retResults;
                    }



                    {
                        string[] conditionField = { "Id" };
                        string[] conditionValue = { vm.Id.ToString() };
                        retResults = _cDal.DeleteTable("EmployeeCompensatoryLeave", conditionField, conditionValue, currConn, transaction);
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

                #region Save
                //int foundId = (int)objfoundId;
                sqlText = "  ";
                sqlText += @" INSERT INTO EmployeeCompensatoryLeave( EmployeeId
,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,IsApprove,IsHalfDay

,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)
                    VALUES ( @EmployeeId
,@LeaveYear,@LeaveType_E,@FromDate,@ToDate,@TotalLeave,@ApprovedBy,@IsApprove,@IsHalfDay

,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom)
                    SELECT SCOPE_IDENTITY()";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdInsert.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                cmdInsert.Parameters.AddWithValue("@IsHalfDay", vm.IsHalfDay);
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
                    retResults[1] = "Please Input Employee Compensatory Leave Value";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Input Employee Compensatory Leave Value", "");
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
        public string[] Approve(EmployeeCompensatoryLeaveVM empLeaveVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeCompensatoryLeave"; //Method Name
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
                sqlText = "select top 1 * from EmployeeCompensatoryLeave where Id=@Id";
                SqlCommand cmd01 = new SqlCommand(sqlText, currConn);
                cmd01.Parameters.AddWithValue("@Id", empLeaveVM.Id);
                cmd01.Transaction = transaction;
                EmployeeCompensatoryLeaveVM elvm = new EmployeeCompensatoryLeaveVM();
                elvm = empLeaveVM;
                SqlDataReader dr1;
                dr1 = cmd01.ExecuteReader();
                while (dr1.Read())
                {
                    elvm.Id = Convert.ToInt32(dr1["Id"]);
                    elvm.EmployeeId = dr1["EmployeeId"].ToString();
                    elvm.LeaveYear = Convert.ToInt32(dr1["LeaveYear"]);
                    elvm.LeaveType_E = dr1["LeaveType_E"].ToString();
                    elvm.IsHalfDay = Convert.ToBoolean(dr1["IsHalfDay"]);
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
                
                retResults = EmployeeCompensatoryLeave(elvm, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("Employee Leave Not Approve", "");
                }
                if (retResults[0] == "Success" || elvm.IsReject != true)
                {
                    retResults = InsertEmployeeLeaveStructureDetail(elvm, currConn, transaction);

                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException("Employee Leave Not Approve", "");
                    }
                }

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
        private string[] EmployeeCompensatoryLeave(EmployeeCompensatoryLeaveVM vm, SqlConnection currConn, SqlTransaction transaction)
        {
            string sqlText = "";
            string[] retResults = new string[6];
            retResults[0] = "Success";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = vm.Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ApproveLeave"; //Method Name
            try
            {

                #region Update Employee Leave (Master Table)
                sqlText = "";
                sqlText = "update EmployeeCompensatoryLeave set";
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

        public string[] InsertEmployeeLeaveStructureDetail(EmployeeCompensatoryLeaveVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertEmployeeCompensatoryLeave"; //Method Name
            string EmployeeLeaveStructureId = "16";
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


                #region CheckPoint

                #region EmployeeStructureGroup
                bool exist = _cDal.ExistCheck("EmployeeStructureGroup", "EmployeeId", vm.EmployeeId, currConn, transaction);
                if (!exist)
                {
                    retResults[1] = "Please Employee Structure Group";

                    throw new ArgumentNullException("Please Employee Structure Group", "");
                }
                #endregion EmployeeStructureGroup

                #region  Get Employee LeaveStructureId
                sqlText = "  ";
                sqlText = @"SELECT *
                                FROM EmployeeStructureGroup
                                WHERE  
                                EmployeeId=@EmployeeId 
                                ";
                SqlCommand cmdEmplLeaveSt = new SqlCommand(sqlText, currConn, transaction);
                cmdEmplLeaveSt.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                System.Data.DataTable dtEmpLeaveSt =new System.Data.DataTable ();
                SqlDataAdapter daEmpLeaveSt = new SqlDataAdapter(cmdEmplLeaveSt);
                daEmpLeaveSt.Fill(dtEmpLeaveSt);
                if (dtEmpLeaveSt.Rows.Count > 0)
                {
                    EmployeeLeaveStructureId = dtEmpLeaveSt.Rows[0]["LeaveStructureId"].ToString();
                }
                #endregion

                #region EmployeeLeaveStructure
                object exeRess;

                sqlText = "  ";
                sqlText += @"Select count(Id) from EmployeeLeaveStructure 
                                where EmployeeId=@EmployeeId and LeaveYear=@LeaveYear and LeaveType_E=@LeaveType_E
                            ";
                SqlCommand cmdLeaveStructureExist = new SqlCommand(sqlText, currConn, transaction);
                cmdLeaveStructureExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdLeaveStructureExist.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                cmdLeaveStructureExist.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E);
                exeRess = cmdLeaveStructureExist.ExecuteScalar();
                int alreadyExist = Convert.ToInt32(exeRess);
                if (alreadyExist < 1)
                {
                    var NextId = _cDal.NextId("EmployeeLeaveStructure", currConn, transaction);
                    #region Save
                    sqlText = "  ";
                    sqlText += @"   INSERT INTO EmployeeLeaveStructure(
                                            Id,EmployeeId,LeaveStructureId,LeaveYear,LeaveType_E,LeaveDays,OpeningLeaveDays,IsEarned,IsCompensation,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,IsCarryForward,MaxBalance         
                                            ) VALUES (
                                            @Id,@EmployeeId,@LeaveStructureId,@LeaveYear,@LeaveType_E,@LeaveDays,@OpeningLeaveDays,@IsEarned,@IsCompensation,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@IsCarryForward,@MaxBalance
                                            )  ";
                    SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                    cmdIns.Parameters.AddWithValue("@Id", NextId);
                    cmdIns.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdIns.Parameters.AddWithValue("@LeaveStructureId", EmployeeLeaveStructureId);
                    cmdIns.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                    cmdIns.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E);
                    cmdIns.Parameters.AddWithValue("@LeaveDays", vm.TotalLeave);
                    cmdIns.Parameters.AddWithValue("@OpeningLeaveDays", "0");
                    cmdIns.Parameters.AddWithValue("@IsEarned", false);
                    cmdIns.Parameters.AddWithValue("@IsCompensation", false);
                    cmdIns.Parameters.AddWithValue("@Remarks", "" ?? Convert.DBNull);
                    cmdIns.Parameters.AddWithValue("@IsActive", true);
                    cmdIns.Parameters.AddWithValue("@IsArchive", false);
                    cmdIns.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdIns.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdIns.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdIns.Parameters.AddWithValue("@IsCarryForward", false);
                    cmdIns.Parameters.AddWithValue("@MaxBalance", 0);
                    cmdIns.Transaction = transaction;
                    cmdIns.ExecuteNonQuery();
                    #endregion Save
                }
                else
                {
                    sqlText = "";
                    sqlText = "update EmployeeLeaveStructure set";

                    sqlText += " LeaveDays= LeaveDays + @TotalLeave,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where EmployeeId=@EmployeeId and LeaveType_E=@LeaveType_E and LeaveYear=@LeaveYear";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                    cmdUpdate.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E);
                    cmdUpdate.Parameters.AddWithValue("@TotalLeave", vm.TotalLeave);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.CreatedBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.CreatedAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.CreatedFrom);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                }
                #endregion

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
        public string[] Update(EmployeeCompensatoryLeaveVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                    sqlText = "update EmployeeCompensatoryLeave set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " EmployeeCompensatoryLeaveStructureId=@EmployeeCompensatoryLeaveStructureId,";
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
        //==================Update Leave Balance =================
        //public string[] UpdateLeaveStructureBalance(EmployeeCompensatoryLeaveStructureVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        //{
        //    #region Variables
        //    string[] retResults = new string[6];
        //    retResults[0] = "Fail";//Success or Fail
        //    retResults[1] = "Fail";// Success or Fail Message
        //    retResults[2] = "0";// Return Id
        //    retResults[3] = "sqlText"; //  SQL Query
        //    retResults[4] = "ex"; //catch ex
        //    retResults[5] = "LeaveStructureUpdate"; //Method Name
        //    int transResult = 0;
        //    string sqlText = "";
        //    bool iSTransSuccess = false;
        //    #endregion
        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
        //    try
        //    {
        //        #region open connection and transaction
        //        #region New open connection and transaction
        //        if (VcurrConn != null)
        //        {
        //            currConn = VcurrConn;
        //        }
        //        if (Vtransaction != null)
        //        {
        //            transaction = Vtransaction;
        //        }
        //        #endregion New open connection and transaction
        //        if (currConn == null)
        //        {
        //            currConn = _dbsqlConnection.GetConnection();
        //            if (currConn.State != ConnectionState.Open)
        //            {
        //                currConn.Open();
        //            }
        //        }
        //        if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToLeaveStructure"); }
        //        #endregion open connection and transaction
        //        if (vm != null)
        //        {
        //            #region Update Settings
        //            sqlText = "";
        //            sqlText = "update EmployeeCompensatoryLeaveStructure set";
        //            //sqlText += " EmployeeId=@EmployeeId";
        //            sqlText += " LeaveStructureId=@LeaveStructureId";
        //            sqlText += " ,LeaveYear=@LeaveYear";
        //            sqlText += " ,LeaveType_E=@LeaveType_E";
        //            sqlText += " ,LeaveDays=@LeaveDays";
        //            sqlText += " ,OpeningLeaveDays=@OpeningLeaveDays";
        //            sqlText += " ,IsEarned=@IsEarned";
        //            sqlText += " ,IsCompensation=@IsCompensation";
        //            sqlText += " ,IsCarryForward=@IsCarryForward";
        //            sqlText += " ,MaxBalance=@MaxBalance";
        //            sqlText += " ,Remarks=@Remarks";
        //            sqlText += " ,IsActive=@IsActive";
        //            sqlText += " ,LastUpdateBy=@LastUpdateBy";
        //            sqlText += " ,LastUpdateAt=@LastUpdateAt";
        //            sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
        //            sqlText += " where Id=@Id";
        //            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
        //            cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
        //            //cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
        //            cmdUpdate.Parameters.AddWithValue("@LeaveStructureId", vm.LeaveStructureId);
        //            cmdUpdate.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
        //            cmdUpdate.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E ?? Convert.DBNull);
        //            cmdUpdate.Parameters.AddWithValue("@LeaveDays", vm.LeaveDays);
        //            cmdUpdate.Parameters.AddWithValue("@OpeningLeaveDays", vm.OpeningLeaveDays);
        //            cmdUpdate.Parameters.AddWithValue("@IsEarned", vm.IsEarned);
        //            cmdUpdate.Parameters.AddWithValue("@IsCompensation", vm.IsCompensation);
        //            cmdUpdate.Parameters.AddWithValue("@IsCarryForward", vm.IsCarryForward);
        //            cmdUpdate.Parameters.AddWithValue("@MaxBalance", vm.MaxBalance);
        //            cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
        //            cmdUpdate.Parameters.AddWithValue("@IsActive", vm.IsActive);
        //            cmdUpdate.Parameters.AddWithValue("@IsArchive", false);
        //            cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
        //            cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
        //            cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
        //            cmdUpdate.Transaction = transaction;
        //            var exeRes = cmdUpdate.ExecuteNonQuery();
        //            transResult = Convert.ToInt32(exeRes);
        //            retResults[2] = vm.Id.ToString();// Return Id
        //            retResults[3] = sqlText; //  SQL Query
        //            #endregion Update Settings
        //            iSTransSuccess = true;
        //        }
        //        else
        //        {
        //            throw new ArgumentNullException("Leave Structure Update", "Could not found any item.");
        //        }
        //        if (iSTransSuccess == true)
        //        {
        //            if (Vtransaction == null)
        //            {
        //                if (transaction != null)
        //                {
        //                    transaction.Commit();
        //                }
        //            }
        //            retResults[0] = "Success";
        //            retResults[1] = "Data Update Successfully.";
        //        }
        //        else
        //        {
        //            retResults[1] = "Unexpected error to update Leave Structure.";
        //            throw new ArgumentNullException("", "");
        //        }
        //    }
        //    #region catch
        //    catch (Exception ex)
        //    {
        //        retResults[0] = "Fail";
        //        retResults[4] = ex.Message; //catch ex
        //        if (Vtransaction == null) { transaction.Rollback(); }
        //        return retResults;
        //    }
        //    finally
        //    {
        //        if (VcurrConn == null)
        //        {
        //            if (currConn != null)
        //            {
        //                if (currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();
        //                }
        //            }
        //        }
        //    }
        //    #endregion
        //    return retResults;
        //}
        //==================Select =================
        public EmployeeCompensatoryLeaveVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeCompensatoryLeaveVM EmployeeCompensatoryLeaveVM = new EmployeeCompensatoryLeaveVM();
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
,EmployeeCompensatoryLeaveStructureId
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
FROM EmployeeCompensatoryLeave    
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
                        EmployeeCompensatoryLeaveVM.Id = Convert.ToInt32(dr["Id"]);
                        EmployeeCompensatoryLeaveVM.EmployeeId = dr["EmployeeId"].ToString();
                        EmployeeCompensatoryLeaveVM.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                        EmployeeCompensatoryLeaveVM.LeaveType_E = dr["LeaveType_E"].ToString();
                        EmployeeCompensatoryLeaveVM.FromDate = dr["FromDate"].ToString();
                        EmployeeCompensatoryLeaveVM.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                        EmployeeCompensatoryLeaveVM.ApprovedBy = dr["ApprovedBy"].ToString();
                        EmployeeCompensatoryLeaveVM.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                        EmployeeCompensatoryLeaveVM.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                        EmployeeCompensatoryLeaveVM.Remarks = dr["Remarks"].ToString();
                        EmployeeCompensatoryLeaveVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        EmployeeCompensatoryLeaveVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                        EmployeeCompensatoryLeaveVM.CreatedBy = dr["CreatedBy"].ToString();
                        EmployeeCompensatoryLeaveVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        EmployeeCompensatoryLeaveVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        EmployeeCompensatoryLeaveVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        EmployeeCompensatoryLeaveVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
            return EmployeeCompensatoryLeaveVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeeCompensatoryLeaveVM EmployeeCompensatoryLeaveVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                if (EmployeeCompensatoryLeaveVM.Id > 0)
                {
                    #region Delete Settings
                    sqlText = "";
                    sqlText = "delete EmployeeCompensatoryLeave  ";

                    sqlText += " where isnull(IsApprove,0)=0 and isnull(IsReject,0)=0 and  Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", EmployeeCompensatoryLeaveVM.Id);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = EmployeeCompensatoryLeaveVM.Id.ToString();// Return Id
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
        public string[] DeleteArchive(EmployeeCompensatoryLeaveVM EmployeeCompensatoryLeaveVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                if (EmployeeCompensatoryLeaveVM.Id > 0)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = " Update EmployeeCompensatoryLeave set";
                    sqlText += " IsApprove=0";
                    sqlText += " ,IsArchive=@IsArchive";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where  Id=@Id";


                    sqlText = " update EmployeeCompensatoryLeaveDetail set";
                    sqlText += " TotalLeave=0";
                    sqlText += " ,IsApprove=0";
                    sqlText += " ,IsArchive=@IsArchive";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";

                    sqlText += " where  EmployeeCompensatoryLeaveId=@Id";

                    //EmployeeCompensatoryLeaveVM.IsApprove

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", EmployeeCompensatoryLeaveVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeeCompensatoryLeaveVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeeCompensatoryLeaveVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeeCompensatoryLeaveVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = EmployeeCompensatoryLeaveVM.Id.ToString();// Return Id
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



        public System.Data.DataTable Report(EmployeeCompensatoryLeaveVM vm)
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
FROM EmployeeCompensatoryLeaveStructure es
LEFT OUTER JOIN 
(
SELECT EmployeeCompensatoryLeaveStructureId,SUM(TotalLeave) Leave 
FROM EmployeeCompensatoryLeave where IsApprove=1  
GROUP BY EmployeeCompensatoryLeaveStructureId
) el ON el.EmployeeCompensatoryLeaveStructureId=es.Id
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

