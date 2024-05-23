using SymOrdinary;
using SymServices.HRM;
using SymViewModel.Attendance;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Attendance
{
    public class AttLogsDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion
        #region Methods
        //==================SelectAll=================
        public List<AttLogsVM> SelectAll(string Id = "")
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<AttLogsVM> attLogsVMs = new List<AttLogsVM>();
            AttLogsVM attLogsVM;
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
 al.ID SLNo
 ,al.proxyid1 UserID
,0 AttendanceStructureId
,al.PunchDate
,al.PunchTime
,al.Remarks
,ei.EmpName,ei.Designation,ei.Project,ei.Department,ei.JoinDate
,ei.Id EmployeeId
,ei.Code

    From downloaddata al left outer join ViewEmployeeInformation ei on al.ProxyId1=ei.AttnUserId
Where al.IsArchive=0 
";
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    sqlText += @" and ei.Id=@Id";
                }

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    attLogsVM = new AttLogsVM();

                    attLogsVM.SLNo = Convert.ToInt32(dr["SLNo"]);
                    attLogsVM.UserID = dr["UserID"].ToString();
                    attLogsVM.AttendanceStructureId = Convert.ToInt32(dr["AttendanceStructureId"]);

                    attLogsVM.PunchDate = Ordinary.StringToDate(dr["PunchDate"].ToString());
                    attLogsVM.PunchTime = Ordinary.StringToTime(dr["PunchTime"].ToString());
                    attLogsVM.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    attLogsVM.EmployeeId = dr["EmployeeId"].ToString();
                    attLogsVM.Code = dr["Code"].ToString();
                    attLogsVM.EmpName = dr["EmpName"].ToString();
                    attLogsVM.Project = dr["Project"].ToString();
                    attLogsVM.Department = dr["Department"].ToString();
                    attLogsVM.Designation = dr["Designation"].ToString();


                    attLogsVM.Remarks = dr["Remarks"].ToString();

                    attLogsVMs.Add(attLogsVM);
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
            return attLogsVMs;
        }


        //==================SelectAllData with Fetch=================
        public List<AttLogsVM> SelectAllData(int pageNo, int pageSize)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<AttLogsVM> attLogsVMs = new List<AttLogsVM>();
            AttLogsVM attLogsVM;
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
 al.ID SLNo
 ,al.proxyid1 UserID
,0 AttendanceStructureId
,al.PunchDate
,al.PunchTime
,al.Remarks
,ei.EmpName,ei.Designation,ei.Project,ei.Department,ei.JoinDate
,ei.Id EmployeeId
,ei.Code

    From downloaddata al left outer join ViewEmployeeInformation ei on al.ProxyId1=ei.AttnUserId
Where al.IsArchive=0 
";

                sqlText += @" ORDER BY al.Id,al.proxyid1 DESC";
                sqlText += @" OFFSET " + pageNo + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;                
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    attLogsVM = new AttLogsVM();

                    attLogsVM.SLNo = Convert.ToInt32(dr["SLNo"]);
                    attLogsVM.UserID = dr["UserID"].ToString();
                    attLogsVM.AttendanceStructureId = Convert.ToInt32(dr["AttendanceStructureId"]);

                    attLogsVM.PunchDate = Ordinary.StringToDate(dr["PunchDate"].ToString());
                    attLogsVM.PunchTime = Ordinary.StringToTime(dr["PunchTime"].ToString());
                    attLogsVM.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    attLogsVM.EmployeeId = dr["EmployeeId"].ToString();
                    attLogsVM.Code = dr["Code"].ToString();
                    attLogsVM.EmpName = dr["EmpName"].ToString();
                    attLogsVM.Project = dr["Project"].ToString();
                    attLogsVM.Department = dr["Department"].ToString();
                    attLogsVM.Designation = dr["Designation"].ToString();


                    attLogsVM.Remarks = dr["Remarks"].ToString();

                    attLogsVMs.Add(attLogsVM);
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
            return attLogsVMs;
        }

        //==================SelectByID=================
        public AttLogsVM SelectById(string Id)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            AttLogsVM attLogsVM = new AttLogsVM();

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
select
 l.SLNo
,l.UserID
,l.AttendanceStructureId
,l.PunchDate
,l.PunchTime
,l.Remarks
,l.IsActive
,l.IsArchive
,l.CreatedBy
,l.CreatedAt
,l.CreatedFrom
,l.LastUpdateBy
,l.LastUpdateAt
,l.LastUpdateFrom
,e.Id EmployeeId
    From AttLogs l
	left outer join EmployeeInfo e on e.AttnUserId=l.UserID
where  l.SLNo=@SLNo
     
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@SLNo", Id);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {

                    attLogsVM.SLNo = Convert.ToInt32(dr["SLNo"]);
                    attLogsVM.UserID = dr["UserID"].ToString();
                    attLogsVM.AttendanceStructureId = Convert.ToInt32(dr["AttendanceStructureId"]);

                    attLogsVM.PunchDate = Ordinary.StringToDate(dr["PunchDate"].ToString());
                    attLogsVM.PunchTime = Ordinary.StringToTime(dr["PunchTime"].ToString());
                    attLogsVM.EmployeeId = dr["EmployeeId"].ToString();


                    attLogsVM.Remarks = dr["Remarks"].ToString();
                    attLogsVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    attLogsVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    attLogsVM.CreatedBy = dr["CreatedBy"].ToString();
                    attLogsVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    attLogsVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    attLogsVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    attLogsVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
            return attLogsVM;
        }
        //==================Insert =================
        public string[] Insert(AttLogsVM attLogsVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Insert"; //Method Name


            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #region Try

            try
            {

                #region Validation
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
                #region Holiday Check
                sqlText = "select * from HoliDay where HoliDay='" + Convert.ToDateTime(attLogsVM.PunchDate).ToString("yyyyMMdd") + "'";
                SqlCommand cmdFindH = new SqlCommand();
                cmdFindH.Connection = currConn;
                cmdFindH.CommandText = sqlText;
                cmdFindH.Transaction = transaction;
                cmdFindH.CommandType = CommandType.Text;
                using (SqlDataReader dr = cmdFindH.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        retResults[1] = attLogsVM.PunchDate + " Already Assign as Holiday";
                        throw new ArgumentNullException(attLogsVM.PunchDate + " Already Assign as Holiday");
                    }
                }

                #endregion  Holiday Check

                #region Leave Check
                sqlText = "select * from EmployeeLeaveDetail where LeaveDate='" + Convert.ToDateTime(attLogsVM.PunchDate).ToString("yyyyMMdd") + "'";
                sqlText += " AND  EmployeeId='" + attLogsVM.EmployeeId + "'";
                SqlCommand cmdFindL = new SqlCommand();
                cmdFindL.Connection = currConn;
                cmdFindL.CommandText = sqlText;
                cmdFindL.Transaction = transaction;
                cmdFindL.CommandType = CommandType.Text;
                using (SqlDataReader dr = cmdFindL.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        retResults[1] = attLogsVM.PunchDate + " Already Assign as Leave";
                        throw new ArgumentNullException(attLogsVM.PunchDate + " Already Assign as Leave");
                    }
                }

                #endregion  Leave Check

                #region Save
                sqlText = "select AttnUserId from employeeInfo where Id=@employeeId";
                SqlCommand cmdFind = new SqlCommand();
                cmdFind.Connection = currConn;
                cmdFind.CommandText = sqlText;
                cmdFind.Transaction = transaction;
                cmdFind.CommandType = CommandType.Text;
                cmdFind.Parameters.AddWithValue("@employeeId", attLogsVM.EmployeeId);
                using (SqlDataReader dr = cmdFind.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        attLogsVM.UserID = dr["AttnUserId"].ToString();
                    }
                }
                if (string.IsNullOrWhiteSpace(attLogsVM.UserID))
                {
                    retResults[1] = "This employee have no Attendance User Id";

                    throw new ArgumentNullException("This employee have no Attendance User Id", "");
                }
                sqlText = "";
                sqlText = @"

SELECT ara.AttendanceStructureId From EmployeeGroup AG
left outer join AttendanceRosterDetail  ARA on ag.GroupId=ara.AttendanceGroupId

where AG.employeeid=@employeeId
and ara.ToDaTe=@ToDaTe
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.Transaction = transaction;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@employeeId", attLogsVM.EmployeeId);
                objComm.Parameters.AddWithValue("@ToDaTe",Ordinary.DateToString( attLogsVM.PunchDate));

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {

                        attLogsVM.AttendanceStructureId = Convert.ToInt32(dr["AttendanceStructureId"]);
                    }
                }
                if (attLogsVM.AttendanceStructureId <= 0)
                {
                    retResults[1] = " Attendance Roster Not Created!";
                    throw new ArgumentNullException("Attendance Roster Not Created", "");
                }
                if (true)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO AttLogs(
UserID
,AttendanceStructureId
,PunchDate
,PunchTime
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) 
                                VALUES (

 @UserID
,@AttendanceStructureId
,@PunchDate
,@PunchTime
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) ";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@UserID", attLogsVM.UserID);
                    cmdInsert.Parameters.AddWithValue("@AttendanceStructureId", attLogsVM.AttendanceStructureId);
                    cmdInsert.Parameters.AddWithValue("@PunchDate", Ordinary.DateToString(attLogsVM.PunchDate));
                    cmdInsert.Parameters.AddWithValue("@PunchTime", Ordinary.TimeToString(attLogsVM.PunchTime));
                    cmdInsert.Parameters.AddWithValue("@Remarks", attLogsVM.Remarks ?? Convert.DBNull);//, attLogsVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", attLogsVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", attLogsVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", attLogsVM.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This Attendance Log already used!";
                    throw new ArgumentNullException("Please Input Attendance Log Value", "");
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
                retResults[2] = "0";

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
        public string[] InsertDetail(AttLogsVM attLogsVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Insert"; //Method Name
            DownloadDataVM dwonVm = new DownloadDataVM();
            DownloadDataDAL _dalDownloandata = new DownloadDataDAL();
            List<DownloadDataVM> dwonVms = new List<DownloadDataVM>();
            AttendanceStructureVM attstVm = new AttendanceStructureVM();
            List<AttendanceStructureVM> attstVms = new List<AttendanceStructureVM>();
            AttendanceMigrationVM attmiVM = new AttendanceMigrationVM();
            AttendanceMigrationDAL attmidal = new AttendanceMigrationDAL();
            AttendanceDailyVM attdailyVm = new AttendanceDailyVM();
            AttendanceDailyDAL _daldaily = new AttendanceDailyDAL();
            ViewEmployeeInfoVM empvm = new ViewEmployeeInfoVM();
            EmployeeInfoDAL _dalemp = new EmployeeInfoDAL();
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #region Try
            try
            {
                #region Validation
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
                #region   Download Data
                //dwonVm.PunchData = attLogsVM.PunchData;
                //dwonVm.PunchDate = attLogsVM.PunchDate;
                //dwonVm.PunchTime = attLogsVM.PunchTime;
                //dwonVm.Remarks = attLogsVM.Remarks;
                //dwonVm.CreatedAt = attLogsVM.CreatedAt;
                //dwonVm.CreatedBy = attLogsVM.CreatedBy;
                //dwonVm.CreatedFrom = attLogsVM.CreatedFrom;
                //retResults = _dalDownloandata.Insert(dwonVm,currConn,transaction);
                //_dalDownloandata.SelectById();
                #endregion Get Data From Download
                #region GETAttendenceStructure
                sqlText = @"SELECT distinct e.Id EmployeeId, atdst.Id AttendanceStructureId 
,atdst.Name AttendanceStructureIdName
,InTime 
,InGrace 
,InTimeStart 
,InTimeEnd 
,OutTime 
,LunchTime 
,LunchBreak 
,WorkingHour 
,OTTime 
,OTTimeMax 
,IsTiff 
,TiffinSTime 
,IsTiffNextDay 
,TGraceTime 
,DeductTiffTime 
,IsIfter 
,IfterSTime 
,IsIfterNextDay 
,IfterGraceTime 
,DeductIfterTime 
,DinnerSTime 
,IsDinNextDay 
,DGraceTime 
,DeductDinTime 
,IsDeductEarlyOut 
,EarlyOutMin 
,IsDeductLateIn 
,LateInMin 
,BonusOTTime 
,MaxOut 
,MaxOutNextD 
,Is_OTRoundUp 
,OTRoundUpMin 
,atdst.Remarks 
,atrd.AttendanceGroupId AttendanceGroupId
 FROM  dbo.AttendanceStructure atdst
left outer join AttendanceRosterDetail atrd on atrd.AttendanceStructureId=atdst.Id
left outer join [group] gp on gp.Id=atrd.AttendanceGroupId
left outer join employeestructuregroup empgp on empgp.EmployeeGroupId=gp.Id
left outer join EmployeeInfo e on e.Id=empgp.EmployeeId
where atdst.IsActive=1 and atdst.IsArchive=0 and empgp.EmployeeId=@EmployeeId ";
                SqlCommand cmdCheck = new SqlCommand();
                cmdCheck.Connection = currConn;
                cmdCheck.CommandText = sqlText;
                cmdCheck.Transaction = transaction;
                cmdCheck.CommandType = CommandType.Text;
                cmdCheck.Parameters.AddWithValue("@EmployeeId", attLogsVM.EmployeeId);
                using (SqlDataReader dr = cmdCheck.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        //attstVm.Id = Convert.ToInt32(dr["AttendanceStructureId"]);
                        //attstVm.AttendanceGroupId = dr["AttendanceGroupId"].ToString();
                        //attstVm.EmployeeId = dr["EmployeeId"].ToString();
                        //attstVm.Name = dr["AttendanceStructureIdName"].ToString();
                        //attstVm.InTime = Ordinary.StringToTime(dr["InTime"].ToString());
                        //attstVm.OutTime = Ordinary.StringToTime(dr["OutTime"].ToString());
                        //attstVm.InGrace = Convert.ToInt32(dr["InGrace"]);
                        //attstVm.InTimeEnd = Ordinary.StringToTime(dr["InTimeEnd"].ToString());
                        //attstVm.LunchTime = Ordinary.StringToTime(dr["LunchTime"].ToString());
                        //attstVm.LunchBreak = Convert.ToInt32(dr["LunchBreak"].ToString());
                        //attstVm.Remarks = dr["Remarks"].ToString();
                        //attstVm.InTimeStart = Ordinary.StringToTime(dr["InTimeStart"].ToString());
                        //attstVm.WorkingHour = Convert.ToDecimal(dr["WorkingHour"].ToString());
                        //attstVm.OTTime = Convert.ToInt32(dr["OTTime"].ToString());
                        //attstVm.OTTimeMax = Convert.ToInt32(dr["OTTimeMax"].ToString());
                        //attstVm.IsTiff = Convert.ToBoolean(dr["IsTiff"].ToString());
                        //attstVm.TiffinSTime = Ordinary.StringToTime(dr["TiffinSTime"].ToString());
                        //attstVm.IsTiffNextDay = Convert.ToBoolean(dr["IsTiffNextDay"].ToString());
                        //attstVm.TGraceTime = Convert.ToInt32(dr["TGraceTime"].ToString());
                        //attstVm.DeductTiffTime = Convert.ToInt32(dr["DeductTiffTime"].ToString());
                        //attstVm.IsIfter = Convert.ToBoolean(dr["IsIfter"].ToString());
                        //attstVm.IfterSTime = Ordinary.StringToTime(dr["IfterSTime"].ToString());
                        //attstVm.IsIfterNextDay = Convert.ToBoolean(dr["IsIfterNextDay"].ToString());
                        //attstVm.IfterGraceTime = Convert.ToInt32(dr["IfterGraceTime"].ToString());
                        //attstVm.DeductIfterTime = Convert.ToInt32(dr["DeductIfterTime"].ToString());
                        //attstVm.DinnerSTime = Ordinary.StringToTime(dr["DinnerSTime"].ToString());
                        //attstVm.IsDinNextDay = Convert.ToBoolean(dr["IsDinNextDay"].ToString());
                        //attstVm.DGraceTime = Convert.ToInt32(dr["DGraceTime"].ToString());
                        //attstVm.DeductDinTime = Convert.ToInt32(dr["DeductDinTime"].ToString());
                        //attstVm.IsDeductEarlyOut = Convert.ToBoolean(dr["IsDeductEarlyOut"].ToString());
                        //attstVm.EarlyOutMin = Convert.ToInt32(dr["EarlyOutMin"].ToString());
                        //attstVm.IsDeductLateIn = Convert.ToBoolean(dr["IsDeductLateIn"].ToString());
                        //attstVm.LateInMin = Convert.ToInt32(dr["LateInMin"].ToString());
                        //attstVm.BonusOTTime = Convert.ToInt32(dr["BonusOTTime"].ToString());
                        //attstVm.MaxOut = Ordinary.StringToTime(dr["MaxOut"].ToString());
                        //attstVm.MaxOutNextD = Convert.ToBoolean(dr["MaxOutNextD"].ToString());
                        //attstVm.IsOTRoundUp = Convert.ToBoolean(dr["Is_OTRoundUp"].ToString());
                        //attstVm.OTRoundUpMin = Convert.ToInt32(dr["OTRoundUpMin"].ToString());
                    }
                }
                #endregion GETAttendenceStructure
                #region CheckTime
                DateTime IntimeStart = Convert.ToDateTime(attstVm.InTime);
                DateTime IntimeEnd = Convert.ToDateTime(attstVm.InTimeEnd);
                DateTime InTime = Convert.ToDateTime(attstVm.InTime);
                DateTime Outtime = Convert.ToDateTime(attstVm.OutTime);
                //int InGrace =Convert.ToInt32(Convert.ToDateTime(attstVm.InGrace).Minute);
                DateTime PunchTime = Convert.ToDateTime(attLogsVM.PunchTime);
                decimal WorkingHrs = 0;
                decimal OT = 0;
                if (IntimeStart <= PunchTime && PunchTime <= IntimeEnd)
                {
                    if (InTime.AddMinutes(Convert.ToDateTime(attstVm.InGrace).Minute).TimeOfDay <= PunchTime.TimeOfDay)
                    {
                        attmiVM.PunchInTime = PunchTime.ToString();
                    }
                }
                if (Outtime >= PunchTime)
                {
                    attmiVM.PunchOutTime = PunchTime.TimeOfDay.ToString();
                    WorkingHrs = InTime.Hour - PunchTime.Hour;
                }
                var LateMin = IntimeEnd.Minute - PunchTime.Minute;
                OT = WorkingHrs - attstVm.WorkingHour;
                #endregion CheckTime
                #region GET EmployeeInfo
                empvm = _dalemp.ViewSelectAllEmployee(null, attstVm.EmployeeId, null, null, null, null, null, currConn, transaction).FirstOrDefault();
                #endregion GET EmployeeInfo
                #region Insert attendance migration
                #region GetMigration
                attmiVM = attmidal.SelectById(null, attstVm.EmployeeId, attstVm.Id.ToString(), attLogsVM.PunchDate.ToString(), attLogsVM.PunchTime);
                #endregion GetMigration
                if (attmiVM.Id == null)
                {
                    //attmiVM.AttendanceStructureId = attstVm.Id;
                    //attmiVM.GroupId = attstVm.AttendanceGroupId;
                    //attmiVM.EmployeeId = attstVm.EmployeeId;
                    //attmiVM.ProxyID = dwonVm.ProxyID;
                    //attmiVM.DailyDate = attLogsVM.PunchDate;
                    //attmiVM.PunchInTime = attmiVM.PunchInTime;
                    //attmiVM.PunchOutTime = attmiVM.PunchOutTime;
                    //attmiVM.PunchOutTimeNextday = "NA";
                    //attmiVM.PunchNextDay = true;
                    //attmiVM.IsManual = true;
                    //attmiVM.InTime = attstVm.InTime;
                    //attmiVM.InGrace = attstVm.InGrace;
                    //attmiVM.InTimeStart = attstVm.InTimeStart;
                    //attmiVM.InTimeEnd = attstVm.InTimeEnd;
                    //attmiVM.OutTime = attstVm.OutTime;
                    //attmiVM.LunchTime = attstVm.LunchTime;
                    //attmiVM.LunchBreak = attstVm.LunchBreak;
                    //attmiVM.WorkingHour = attstVm.WorkingHour;
                    //attmiVM.OTTime = attstVm.OTTime;
                    //attmiVM.OTTimeMax = attstVm.OTTimeMax;
                    //attmiVM.IsTiff = attstVm.IsTiff;
                    //attmiVM.TiffinSTime = attstVm.TiffinSTime;
                    //attmiVM.IsTiffNextDay = attstVm.IsTiffNextDay;
                    //attmiVM.TGraceTime = attstVm.TGraceTime;
                    //attmiVM.DeductTiffTime = attstVm.DeductTiffTime;
                    //attmiVM.IsIfter = attstVm.IsIfter;
                    //attmiVM.IfterSTime = attstVm.IfterSTime;
                    //attmiVM.IsIfterNextDay = attstVm.IsIfterNextDay;
                    //attmiVM.IfterGraceTime = attstVm.IfterGraceTime;
                    //attmiVM.DeductIfterTime = attstVm.DeductIfterTime;
                    //attmiVM.DinnerSTime = attstVm.DinnerSTime;
                    //attmiVM.IsDinNextDay = attstVm.IsDinNextDay;
                    //attmiVM.DGraceTime = attstVm.DGraceTime;
                    //attmiVM.DeductDinTime = attstVm.DeductDinTime;
                    //attmiVM.IsDeductEarlyOut = attstVm.IsDeductEarlyOut;
                    //attmiVM.EarlyOutMin = attstVm.EarlyOutMin;
                    //attmiVM.IsDeductLateIn = attstVm.IsDeductLateIn;
                    //attmiVM.LateInMin = attstVm.LateInMin;
                    //attmiVM.BonusOTTime = attstVm.BonusOTTime;
                    //attmiVM.MaxOut = attstVm.MaxOut;
                    //attmiVM.MaxOutNextD = attstVm.MaxOutNextD;
                    //attmiVM.IsOTRoundUp = true;
                    //attmiVM.OTRoundUpMin = attstVm.OTRoundUpMin;
                    //attmiVM.Remarks = attLogsVM.Remarks;
                    //attmiVM.IsActive = true;
                    //attmiVM.IsArchive = false;
                    //attmiVM.CreatedBy = attLogsVM.CreatedBy;
                    //attmiVM.CreatedAt = attstVm.CreatedAt;
                    //attmiVM.CreatedFrom = attstVm.CreatedFrom;
                    //retResults = attmidal.Insert(attmiVM, currConn, transaction);
                }
                else
                {
                    retResults = attmidal.Update(attmiVM, currConn, transaction);
                }
                #endregion Insert attendance migration

                #region Insert Daily Attendence
                attdailyVm.AttendanceStructureId = attmiVM.AttendanceStructureId;
                attdailyVm.GroupId = attmiVM.GroupId;
                attdailyVm.EmployeeId = attmiVM.EmployeeId;
                attdailyVm.ProxyID = attmiVM.ProxyID;
                attdailyVm.DailyDate = attmiVM.DailyDate;
                attdailyVm.PunchInTime = attmiVM.PunchInTime;
                attdailyVm.PunchOutTime = attmiVM.PunchOutTime;
                attdailyVm.PunchOutTimeNextday = attmiVM.PunchOutTimeNextday;
                attdailyVm.PunchNextDay = attmiVM.PunchNextDay;
                attdailyVm.IsManual = attmiVM.IsManual;
                attdailyVm.InTime = attmiVM.PInTime;
                attdailyVm.InTimeBy = "Na";
                attdailyVm.OutTime = attmiVM.POutTime;
                attdailyVm.OutTimeBy = "Na";
                attdailyVm.LateMin = LateMin;
                attdailyVm.WorkingHrs = WorkingHrs.ToString();
                attdailyVm.WorkingHrsBy = "Na"; ;
                attdailyVm.WorkingHrsRest = "Na"; ;
                attdailyVm.TotalOT = OT;
                attdailyVm.TotalOTBy = 0;
                attdailyVm.ExtraOT = 0;
                attdailyVm.OTRest = 0;
                attdailyVm.BonusMinit = 0;
                attdailyVm.LunchOutDeduct = 0;
                attdailyVm.AttnStatus = "Na";
                attdailyVm.DayStatus = "Na";
                attdailyVm.EarlyDeduct = 0;
                attdailyVm.LateDeduct = 0;
                attdailyVm.TiffinAllow = true;
                attdailyVm.DinnerAllow = true;
                attdailyVm.IfterAllow = true;
                attdailyVm.GrossAmnt = empvm.GrossSalary;
                attdailyVm.BasicAmnt = empvm.BasicSalary;

                retResults = _daldaily.Insert(attdailyVm, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], retResults[1]);
                }

                #endregion Insert Daily Attendence
                //#region Update Download data
                //dwonVm.IsMigrate = true;
                //  dwonVm.LastUpdateAt = attLogsVM.CreatedAt;
                //  dwonVm.LastUpdateBy = attLogsVM.CreatedBy;
                //  dwonVm.LastUpdateFrom = attLogsVM.CreatedFrom;
                //retResults = _dalDownloandata.Insert(dwonVm,currConn,transaction);
                //#endregion Update Download data
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
                retResults[2] = "0";
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                //retResults[1] = ex.Message.ToString();
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
        public string[] Update(AttLogsVM attLogsVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = " Update"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }

                #endregion open connection and transaction


                if (true)
                {
                    sqlText = "";
                    sqlText = @"

SELECT ara.AttendanceStructureId From EmployeeGroup AG
left outer join AttendanceRosterDetail  ARA on ag.GroupId=ara.AttendanceGroupId

where AG.employeeid=@employeeId
and ara.ToDaTe=@ToDaTe
";

                    SqlCommand objComm = new SqlCommand();
                    objComm.Connection = currConn;
                    objComm.CommandText = sqlText;
                    objComm.Transaction = transaction;
                    objComm.CommandType = CommandType.Text;
                    objComm.Parameters.AddWithValue("@employeeId", attLogsVM.EmployeeId);
                    objComm.Parameters.AddWithValue("@ToDaTe", Ordinary.DateToString(attLogsVM.PunchDate));

                    using (SqlDataReader dr = objComm.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            attLogsVM.AttendanceStructureId = Convert.ToInt32(dr["AttendanceStructureId"]);
                        }
                    }
                    if (attLogsVM.AttendanceStructureId <= 0)
                    {
                        retResults[1] = " Attendance Roster Not Created!";
                        throw new ArgumentNullException("Attendance Roster Not Created", "");
                    }

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update AttLogs set";

                    sqlText += "  UserID=@UserID";
                    sqlText += " ,AttendanceStructureId=@AttendanceStructureId";
                    sqlText += " ,PunchDate=@PunchDate";
                    sqlText += " ,PunchTime=@PunchTime";



                    sqlText += " ,Remarks=@Remarks";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where SLNo=@SLNo";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@SLNo", attLogsVM.SLNo);
                    cmdUpdate.Parameters.AddWithValue("@UserID", attLogsVM.UserID);
                    cmdUpdate.Parameters.AddWithValue("@AttendanceStructureId", attLogsVM.AttendanceStructureId);
                    cmdUpdate.Parameters.AddWithValue("@PunchDate", Ordinary.DateToString(attLogsVM.PunchDate));
                    cmdUpdate.Parameters.AddWithValue("@PunchTime", Ordinary.TimeToString(attLogsVM.PunchTime));


                    cmdUpdate.Parameters.AddWithValue("@Remarks", attLogsVM.Remarks ?? Convert.DBNull);//, attLogsVM.Remarks);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", attLogsVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", attLogsVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", attLogsVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    transResult = (int)cmdUpdate.ExecuteNonQuery();

                    retResults[2] = attLogsVM.SLNo.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", AttLogsVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    retResults[1] = "This Attendance Log already used!";
                    throw new ArgumentNullException("Please Input Attendance Log Value", "");
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
                    retResults[1] = "Unexpected error to update Attendance Log.";
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
        public string[] Delete(AttLogsVM attLogsVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteBank"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update AttLogs set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where SLNo=@SLNo";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@SLNo", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", attLogsVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", attLogsVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", attLogsVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                        transResult = (int)cmdUpdate.ExecuteNonQuery();
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Attendance Log Delete", attLogsVM.SLNo + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException(" Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Attendance Log Information.";
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
