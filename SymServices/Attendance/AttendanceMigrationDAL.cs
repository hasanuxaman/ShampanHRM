using Newtonsoft.Json.Linq;
using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace SymServices.Attendance
{
    public class AttendanceMigrationDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        ////==================SelectByMultiCondition=================
        public List<AttendanceMigrationVM> SelectByMultiCondition(string[] conFields = null, string[] conValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
            List<AttendanceMigrationVM> vms = new List<AttendanceMigrationVM>();
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
                #region sqlText
                sqlText = @"
SELECT
 Id
,AttendanceStructureId
,GroupId
,EmployeeId
,ProxyID
,DailyDate
,PunchInTime
,PunchOutTime
,PunchOutTimeNextday
,ISNULL(PunchNextDay, 0) PunchNextDay
,ISNULL(IsManual, 0) IsManual
,PInTime
,ISNULL(PInGrace, 0) PInGrace
,ISNULL(POutGrace, 0) POutGrace
,ISNULL(PInTimeStart		  , 0) PInTimeStart
,ISNULL(PInTimeEnd			  , 0) PInTimeEnd
,ISNULL(POutTime			  , 0) POutTime
,ISNULL(PLunchTime			  , 0) PLunchTime
,ISNULL(PLunchBreak			  , 0) PLunchBreak
,ISNULL(PWorkingHour		  , 0) PWorkingHour
,ISNULL(POTTime				  , 0) POTTime
,ISNULL(PIsTiff				  , 0) PIsTiff
,ISNULL(PTiffinSTime		  , 0) PTiffinSTime
,ISNULL(PIsTiffNextDay		  , 0) PIsTiffNextDay
,ISNULL(PDeductTiffTime	      , 0) PDeductTiffTime
,ISNULL(PIsIfter			  , 0) PIsIfter
,ISNULL(PIfterSTime		      , 0) PIfterSTime
,ISNULL(PIsIfterNextDay	      , 0) PIsIfterNextDay
,ISNULL(PDeductIfterTime	  , 0) PDeductIfterTime
,ISNULL(PDinnerSTime		  , 0) PDinnerSTime
,ISNULL(PIsDinNextDay		  , 0) PIsDinNextDay
,ISNULL(PDeductDinTime		  , 0) PDeductDinTime
,ISNULL(PIsDeductEarlyOut	  , 0) PIsDeductEarlyOut
,ISNULL(PEarlyOutMin		  , 0) PEarlyOutMin
,ISNULL(PIsDeductLateIn	      , 0) PIsDeductLateIn
,ISNULL(PLateInMin			  , 0) PLateInMin
,ISNULL(PIsOTRoundUp		  , 0) PIsOTRoundUp
,ISNULL(POTRoundUpMin		  , 0) POTRoundUpMin
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

From AttendanceMigration
where    1=1 
";
                #endregion sqlText

                int i = 0;
                if (conFields != null && conValues != null)
                {
                    foreach (string item in conFields)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[i]) || string.IsNullOrWhiteSpace(conValues[i]))
                        {
                            continue;
                        }
                        sqlText += " AND " + conFields[i] + "=@" + conFields[i];
                        i++;
                    }
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                i = 0;
                if (conFields != null && conValues != null)
                {
                    foreach (string item in conFields)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[i]) || string.IsNullOrWhiteSpace(conValues[i]))
                        {
                            continue;
                        }
                        objComm.Parameters.AddWithValue("@" + conFields[i], conValues[i]);
                        i++;
                    }
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AttendanceMigrationVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.AttendanceStructureId = Convert.ToInt32(dr["AttendanceStructureId"]);
                    vm.GroupId = dr["GroupId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ProxyID = dr["ProxyID"].ToString();
                    vm.DailyDate = Ordinary.StringToDate(dr["DailyDate"].ToString());
                    vm.PunchInTime = Ordinary.StringToTime(dr["PunchInTime"].ToString());
                    vm.PunchOutTime = Ordinary.StringToTime(dr["PunchOutTime"].ToString());
                    vm.PunchOutTimeNextday = Ordinary.StringToTime(dr["PunchOutTimeNextday"].ToString());
                    vm.PunchNextDay = Convert.ToBoolean(dr["PunchNextDay"]);
                    vm.IsManual = Convert.ToBoolean(dr["IsManual"]);
                    vm.PInTime = Ordinary.StringToTime(dr["PInTime"].ToString());
                    vm.PInGrace = Convert.ToInt32(dr["PInGrace"]);
                    vm.POutGrace = Convert.ToInt32(dr["POutGrace"]);
                    vm.PInTimeStart = Ordinary.StringToTime(dr["PInTimeStart"].ToString());
                    vm.PInTimeEnd = Ordinary.StringToTime(dr["PInTimeEnd"].ToString());
                    vm.POutTime = Ordinary.StringToTime(dr["POutTime"].ToString());
                    vm.PLunchTime = Ordinary.StringToTime(dr["PLunchTime"].ToString());
                    vm.PLunchBreak = Convert.ToInt32(dr["PLunchBreak"]);
                    vm.PWorkingHour = Convert.ToDecimal(dr["PWorkingHour"]);
                    vm.POTTime = Convert.ToInt32(dr["POTTime"]);
                    vm.PIsTiff = Convert.ToBoolean(dr["PIsTiff"]);
                    vm.PTiffinSTime = Ordinary.StringToTime(dr["PTiffinSTime"].ToString());
                    vm.PIsTiffNextDay = Convert.ToBoolean(dr["PIsTiffNextDay"]);
                    vm.PDeductTiffTime = Convert.ToInt32(dr["PDeductTiffTime"]);
                    vm.PIsIfter = Convert.ToBoolean(dr["PIsIfter"]);
                    vm.PIfterSTime = Ordinary.StringToTime(dr["PIfterSTime"].ToString());
                    vm.PIsIfterNextDay = Convert.ToBoolean(dr["PIsIfterNextDay"]);
                    vm.PDeductIfterTime = Convert.ToInt32(dr["PDeductIfterTime"]);
                    vm.PDinnerSTime = Ordinary.StringToTime(dr["PDinnerSTime"].ToString());
                    vm.PIsDinNextDay = Convert.ToBoolean(dr["PIsDinNextDay"]);
                    vm.PDeductDinTime = Convert.ToInt32(dr["PDeductDinTime"]);
                    vm.PIsDeductEarlyOut = Convert.ToBoolean(dr["PIsDeductEarlyOut"]);
                    vm.PEarlyOutMin = Convert.ToInt32(dr["PEarlyOutMin"]);
                    vm.PIsDeductLateIn = Convert.ToBoolean(dr["PIsDeductLateIn"]);
                    vm.PLateInMin = Convert.ToInt32(dr["PLateInMin"]);
                    vm.PIsOTRoundUp = Convert.ToBoolean(dr["PIsOTRoundUp"]);
                    vm.POTRoundUpMin = Convert.ToInt32(dr["POTRoundUpMin"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
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
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
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
            return vms;
        }

        //==================SelectByID=================
        public AttendanceMigrationVM SelectById(string Id = null, string EmployeeId = null, string AttendanceStructureId = null, string DailyDate = null, string PunchInTime = null, string punchOut = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
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
                #region sql Text
                sqlText = @"
SELECT
 Id
,AttendanceStructureId
,GroupId
,EmployeeId
,ProxyID
,DailyDate
,PunchInTime
,PunchOutTime
,PunchOutTimeNextday
,PunchNextDay
,IsManual
,PInTime
,PInGrace
,POutGrace
,PInTimeStart
,PInTimeEnd
,POutTime
,PLunchTime
,PLunchBreak
,PWorkingHour
,POTTime
,PIsTiff
,PTiffinSTime
,PIsTiffNextDay
,PDeductTiffTime
,PIsIfter
,PIfterSTime
,PIsIfterNextDay
,PDeductIfterTime
,PDinnerSTime
,PIsDinNextDay
,PDeductDinTime
,PIsDeductEarlyOut
,PEarlyOutMin
,PIsDeductLateIn
,PLateInMin
,PIsOTRoundUp
,POTRoundUpMin

,PWorkingMin
,PTiffinMin 
,PDinnerMin 
,PIfterMin  

,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

From AttendanceMigration
where  IsArchive=0 and 1=1 
";

                //PWorkingMin
                //PTiffinMin 
                //PDinnerMin 
                //PIfterMin  
                #endregion sql Text

                #region Parameters Check

                if (Id != null && Id != "null")
                {
                    sqlText += @" and Id=@Id ";
                }
                if (DailyDate != null && DailyDate != "null")
                {
                    sqlText += @" and DailyDate=@DailyDate ";
                }
                if (EmployeeId != null && EmployeeId != "null")
                {
                    sqlText += @" and EmployeeId=@EmployeeId ";
                }
                if (AttendanceStructureId != null && AttendanceStructureId != "null")
                {
                    sqlText += @" and AttendanceStructureId=@AttendanceStructureId ";
                }
                if (PunchInTime != null && PunchInTime != "null")
                {
                    sqlText += @" and PunchInTime=@PunchInTime ";
                }
                if (punchOut != null && punchOut != "null")
                {
                    sqlText += @" and punchOut=@punchOut ";
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (Id != null && Id != "null")
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                if (DailyDate != null && DailyDate != "null")
                {
                    objComm.Parameters.AddWithValue("@DailyDate", DailyDate);
                }
                if (EmployeeId != null && EmployeeId != "null")
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                if (AttendanceStructureId != null && AttendanceStructureId != "null")
                {
                    objComm.Parameters.AddWithValue("@AttendanceStructureId", AttendanceStructureId);
                }
                if (PunchInTime != null && PunchInTime != "null")
                {
                    objComm.Parameters.AddWithValue("@PunchInTime", PunchInTime);
                }
                if (punchOut != null && punchOut != "null")
                {
                    objComm.Parameters.AddWithValue("@punchOut", punchOut);
                }
                #endregion Parameters Check
                #region Data Reading

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AttendanceMigrationVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.AttendanceStructureId = Convert.ToInt32(dr["AttendanceStructureId"]);
                    vm.GroupId = dr["GroupId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ProxyID = dr["ProxyID"].ToString();
                    vm.DailyDate = Ordinary.StringToDate(dr["DailyDate"].ToString());
                    vm.PunchInTime = Ordinary.StringToTime(dr["PunchInTime"].ToString());
                    vm.PunchOutTime = Ordinary.StringToTime(dr["PunchOutTime"].ToString());
                    vm.PunchOutTimeNextday = Ordinary.StringToTime(dr["PunchOutTimeNextday"].ToString());
                    vm.PunchNextDay = Convert.ToBoolean(dr["PunchNextDay"]);
                    vm.IsManual = Convert.ToBoolean(dr["IsManual"]);
                    vm.PInTime = Ordinary.StringToTime(dr["PInTime"].ToString());
                    vm.PInGrace = Convert.ToInt32(dr["PInGrace"]);
                    vm.POutGrace = Convert.ToInt32(dr["POutGrace"]);
                    vm.PInTimeStart = Ordinary.StringToTime(dr["PInTimeStart"].ToString());
                    vm.PInTimeEnd = Ordinary.StringToTime(dr["PInTimeEnd"].ToString());
                    vm.POutTime = Ordinary.StringToTime(dr["POutTime"].ToString());
                    vm.PLunchTime = Ordinary.StringToTime(dr["PLunchTime"].ToString());
                    vm.PLunchBreak = Convert.ToInt32(dr["PLunchBreak"]);
                    vm.PWorkingHour = Convert.ToDecimal(dr["PWorkingHour"]);
                    vm.POTTime = Convert.ToInt32(dr["POTTime"]);
                    vm.PIsTiff = Convert.ToBoolean(dr["PIsTiff"]);
                    vm.PTiffinSTime = Ordinary.StringToTime(dr["PTiffinSTime"].ToString());
                    vm.PIsTiffNextDay = Convert.ToBoolean(dr["PIsTiffNextDay"]);
                    vm.PDeductTiffTime = Convert.ToInt32(dr["PDeductTiffTime"]);
                    vm.PIsIfter = Convert.ToBoolean(dr["PIsIfter"]);
                    vm.PIfterSTime = Ordinary.StringToTime(dr["PIfterSTime"].ToString());
                    vm.PIsIfterNextDay = Convert.ToBoolean(dr["PIsIfterNextDay"]);
                    vm.PDeductIfterTime = Convert.ToInt32(dr["PDeductIfterTime"]);
                    vm.PDinnerSTime = Ordinary.StringToTime(dr["PDinnerSTime"].ToString());
                    vm.PIsDinNextDay = Convert.ToBoolean(dr["PIsDinNextDay"]);
                    vm.PDeductDinTime = Convert.ToInt32(dr["PDeductDinTime"]);
                    vm.PIsDeductEarlyOut = Convert.ToBoolean(dr["PIsDeductEarlyOut"]);
                    vm.PEarlyOutMin = Convert.ToInt32(dr["PEarlyOutMin"]);
                    vm.PIsDeductLateIn = Convert.ToBoolean(dr["PIsDeductLateIn"]);
                    vm.PLateInMin = Convert.ToInt32(dr["PLateInMin"]);
                    vm.PIsOTRoundUp = Convert.ToBoolean(dr["PIsOTRoundUp"]);
                    vm.PWorkingMin = Convert.ToInt32(dr["PWorkingMin"]);
                    vm.PTiffinMin = Convert.ToInt32(dr["PTiffinMin"]);
                    vm.PDinnerMin = Convert.ToInt32(dr["PDinnerMin"]);
                    vm.PIfterMin = Convert.ToInt32(dr["PIfterMin"]);

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                }
                dr.Close();
                #endregion  Data Reading
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

        public List<AttendanceMigrationVM> SelectNotInAttendanceMigration(string DailyDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
            List<AttendanceMigrationVM> VMs = new List<AttendanceMigrationVM>();
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
                #region sql Text
                sqlText = @"
SELECT Code As EmployeeCode, Id As EmployeeId
FROM EmployeeInfo
WHERE 1=1
AND Id Not In(SELECT DISTINCT EmployeeId from AttendanceMigration WHERE DailyDate = @DailyDate)
AND Id Not IN('1_0')
AND IsActive = 1 AND IsArchive = 0
";
                #endregion sql Text

                #region Parameters Check
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@DailyDate", Ordinary.DateToString(DailyDate));
                #endregion Parameters Check
                #region Data Reading
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AttendanceMigrationVM();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeCode = dr["EmployeeCode"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null && transaction.Connection != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                #endregion  Data Reading
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
            return VMs;
        }

        //==================Insert =================
        public string[] Insert(AttendanceMigrationVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            int transResult = 0;
            #region Try
            try
            {
                #region Validation
                //if (string.IsNullOrEmpty(AttendanceMigrationVM.DownloadDataId))
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
                CommonDAL _cDal = new CommonDAL();
                vm.Id = _cDal.NextId("AttendanceMigration", currConn, transaction);

                if (vm != null)
                {
                    #region sqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO AttendanceMigration(
AttendanceStructureId
,GroupId
,EmployeeId
,ProxyID
,DailyDate
,PunchInTime
,PunchOutTime
,PunchOutTimeNextday
,PunchNextDay
,IsManual
,PInTime
,PInGrace
,POutGrace
,PInTimeStart
,PInTimeEnd
,POutTime
,PLunchTime
,PLunchBreak
,PWorkingHour
,POTTime
,PIsTiff
,PTiffinSTime
,PIsTiffNextDay
,PDeductTiffTime
,PIsIfter
,PIfterSTime
,PIsIfterNextDay
,PDeductIfterTime
,PDinnerSTime
,PIsDinNextDay
,PDeductDinTime
,PIsDeductEarlyOut
,PEarlyOutMin
,PIsDeductLateIn
,PLateInMin
,PIsOTRoundUp
,POTRoundUpMin

,PWorkingMin
,PTiffinMin 
,PDinnerMin 
,PIfterMin  

,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,ProcessTime  

) 
                                VALUES (
@AttendanceStructureId
,@GroupId
,@EmployeeId
,@ProxyID
,@DailyDate
,@PunchInTime
,@PunchOutTime
,@PunchOutTimeNextday
,@PunchNextDay
,@IsManual
,@PInTime
,@PInGrace
,@POutGrace
,@PInTimeStart
,@PInTimeEnd
,@POutTime
,@PLunchTime
,@PLunchBreak
,@PWorkingHour
,@POTTime
,@PIsTiff
,@PTiffinSTime
,@PIsTiffNextDay
,@PDeductTiffTime
,@PIsIfter
,@PIfterSTime
,@PIsIfterNextDay
,@PDeductIfterTime
,@PDinnerSTime
,@PIsDinNextDay
,@PDeductDinTime
,@PIsDeductEarlyOut
,@PEarlyOutMin
,@PIsDeductLateIn
,@PLateInMin
,@PIsOTRoundUp
,@POTRoundUpMin

,@PWorkingMin
,@PTiffinMin 
,@PDinnerMin 
,@PIfterMin  

,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@ProcessTime  

) 
";

                    //PWorkingMin
                    //PTiffinMin 
                    //PDinnerMin 
                    //PIfterMin  
                    #endregion sqlText
                    #region sqlExecution

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                    vm.PunchInTime = string.IsNullOrWhiteSpace(vm.PunchInTime) ? "0000" : Ordinary.TimeToString(vm.PunchInTime);
                    vm.PunchOutTime = string.IsNullOrWhiteSpace(vm.PunchOutTime) ? "0000" : Ordinary.TimeToString(vm.PunchOutTime);
                    vm.PunchOutTimeNextday = string.IsNullOrWhiteSpace(vm.PunchOutTimeNextday) ? "0000" : Ordinary.TimeToString(vm.PunchOutTimeNextday);

                    //cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@AttendanceStructureId", vm.AttendanceStructureId);
                    cmdInsert.Parameters.AddWithValue("@GroupId", vm.GroupId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ProxyID", vm.ProxyID ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@DailyDate", Ordinary.DateToString(vm.DailyDate));
                    cmdInsert.Parameters.AddWithValue("@PunchInTime", vm.PunchInTime);
                    cmdInsert.Parameters.AddWithValue("@PunchOutTime", vm.PunchOutTime);
                    cmdInsert.Parameters.AddWithValue("@PunchOutTimeNextday", vm.PunchOutTimeNextday);
                    cmdInsert.Parameters.AddWithValue("@PunchNextDay", vm.PunchNextDay);
                    cmdInsert.Parameters.AddWithValue("@IsManual", vm.IsManual);
                    cmdInsert.Parameters.AddWithValue("@PInTime", Ordinary.TimeToString(vm.PInTime));
                    cmdInsert.Parameters.AddWithValue("@PInGrace", vm.PInGrace);
                    cmdInsert.Parameters.AddWithValue("@POutGrace", vm.POutGrace);
                    cmdInsert.Parameters.AddWithValue("@PInTimeStart", Ordinary.TimeToString(vm.PInTimeStart));
                    cmdInsert.Parameters.AddWithValue("@PInTimeEnd", Ordinary.TimeToString(vm.PInTimeEnd));
                    cmdInsert.Parameters.AddWithValue("@POutTime", Ordinary.TimeToString(vm.POutTime));
                    cmdInsert.Parameters.AddWithValue("@PLunchTime", Ordinary.TimeToString(vm.PLunchTime));
                    cmdInsert.Parameters.AddWithValue("@PLunchBreak", vm.PLunchBreak);
                    cmdInsert.Parameters.AddWithValue("@PWorkingHour", vm.PWorkingHour);
                    cmdInsert.Parameters.AddWithValue("@POTTime", vm.POTTime);
                    cmdInsert.Parameters.AddWithValue("@PIsTiff", vm.PIsTiff);
                    cmdInsert.Parameters.AddWithValue("@PTiffinSTime", Ordinary.TimeToString(vm.PTiffinSTime));
                    cmdInsert.Parameters.AddWithValue("@PIsTiffNextDay", vm.PIsTiffNextDay);
                    cmdInsert.Parameters.AddWithValue("@PDeductTiffTime", vm.PDeductTiffTime);
                    cmdInsert.Parameters.AddWithValue("@PIsIfter", vm.PIsIfter);
                    cmdInsert.Parameters.AddWithValue("@PIfterSTime", Ordinary.TimeToString(vm.PIfterSTime));
                    cmdInsert.Parameters.AddWithValue("@PIsIfterNextDay", vm.PIsIfterNextDay);
                    cmdInsert.Parameters.AddWithValue("@PDeductIfterTime", vm.PDeductIfterTime);
                    cmdInsert.Parameters.AddWithValue("@PDinnerSTime", Ordinary.TimeToString(vm.PDinnerSTime));
                    cmdInsert.Parameters.AddWithValue("@PIsDinNextDay", vm.PIsDinNextDay);
                    cmdInsert.Parameters.AddWithValue("@PDeductDinTime", vm.PDeductDinTime);
                    cmdInsert.Parameters.AddWithValue("@PIsDeductEarlyOut", vm.PIsDeductEarlyOut);
                    cmdInsert.Parameters.AddWithValue("@PEarlyOutMin", vm.PEarlyOutMin);
                    cmdInsert.Parameters.AddWithValue("@PIsDeductLateIn", vm.PIsDeductLateIn);
                    cmdInsert.Parameters.AddWithValue("@PLateInMin", vm.PLateInMin);
                    cmdInsert.Parameters.AddWithValue("@PIsOTRoundUp", vm.PIsOTRoundUp);
                    cmdInsert.Parameters.AddWithValue("@POTRoundUpMin", vm.POTRoundUpMin);
                    cmdInsert.Parameters.AddWithValue("@PWorkingMin", vm.PWorkingMin);
                    cmdInsert.Parameters.AddWithValue("@PTiffinMin ", vm.PTiffinMin);
                    cmdInsert.Parameters.AddWithValue("@PDinnerMin ", vm.PDinnerMin);
                    cmdInsert.Parameters.AddWithValue("@PIfterMin  ", vm.PIfterMin);

                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, AttendanceMigrationVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@ProcessTime", vm.ProcessTime ?? DateTime.Now.ToString("yyyyMMddHHmm"));
                    var exe = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exe);
                    if (transResult <= 0)
                    {
                        retResults[1] = "Unexpected Error to Migration!";
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion sqlExecution

                }
                else
                {
                    retResults[1] = "This Attendance Migration already used!";
                    throw new ArgumentNullException(retResults[1], "");
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
                retResults[3] = sqlText;
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
        public string[] Update(AttendanceMigrationVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update AttendanceMigration set";
                    sqlText += " Remarks=@Remarks";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " ,PunchInTime=@PunchInTime";
                    sqlText += " ,PunchOutTime=@PunchOutTime";
                    sqlText += " ,PunchOutTimeNextday=@PunchOutTimeNextday";
                    sqlText += " ,PunchNextDay=@PunchNextDay";
                    sqlText += " ,IsManual=@IsManual";
                    sqlText += " ,ProcessTime=@ProcessTime";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    vm.PunchInTime = string.IsNullOrWhiteSpace(vm.PunchInTime) ? "0000" : Ordinary.TimeToString(vm.PunchInTime);
                    vm.PunchOutTime = string.IsNullOrWhiteSpace(vm.PunchOutTime) ? "0000" : Ordinary.TimeToString(vm.PunchOutTime);
                    vm.PunchOutTimeNextday = string.IsNullOrWhiteSpace(vm.PunchOutTimeNextday) ? "0000" : Ordinary.TimeToString(vm.PunchOutTimeNextday);

                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, AttendanceMigrationVM.Remarks);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@PunchInTime", vm.PunchInTime);
                    cmdUpdate.Parameters.AddWithValue("@PunchOutTime", vm.PunchOutTime);
                    cmdUpdate.Parameters.AddWithValue("@PunchOutTimeNextday", vm.PunchOutTimeNextday);
                    cmdUpdate.Parameters.AddWithValue("@PunchNextDay", vm.PunchNextDay);
                    cmdUpdate.Parameters.AddWithValue("@IsManual", vm.IsManual);
                    cmdUpdate.Parameters.AddWithValue("@ProcessTime", vm.ProcessTime ?? Convert.DBNull);
                    transResult = (int)cmdUpdate.ExecuteNonQuery();
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    if (transResult <= 0)
                    {
                    }
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    retResults[1] = "This Attendance Migration already used!";
                    throw new ArgumentNullException("Please Input Attendance Migration Value", "");
                }
                #region Commit
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
                    retResults[1] = "Unexpected error to update Attendance Migration.";
                    throw new ArgumentNullException("", "");
                }
                #endregion Commit
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[3] = sqlText;
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


        //==================Update =================
        public string[] UpdateByMultiCondition(AttendanceMigrationVM vm = null, string[] conFields = null, string[] conValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update AttendanceMigration set";
                    sqlText += " Remarks=@Remarks";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " ,PunchInTime=@PunchInTime";
                    sqlText += " ,PunchOutTime=@PunchOutTime";
                    sqlText += " ,PunchOutTimeNextday=@PunchOutTimeNextday";
                    sqlText += " ,PunchNextDay=@PunchNextDay";
                    sqlText += " ,IsManual=@IsManual";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    vm.PunchInTime = string.IsNullOrWhiteSpace(Ordinary.TimeToString(vm.PunchInTime)) ? "0000" : Ordinary.TimeToString(vm.PunchInTime);
                    vm.PunchOutTime = string.IsNullOrWhiteSpace(Ordinary.TimeToString(vm.PunchOutTime)) ? "0000" : Ordinary.TimeToString(vm.PunchOutTime);
                    vm.PunchOutTimeNextday = string.IsNullOrWhiteSpace(Ordinary.TimeToString(vm.PunchOutTimeNextday)) ? "0000" : Ordinary.TimeToString(vm.PunchOutTimeNextday);

                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, AttendanceMigrationVM.Remarks);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@PunchInTime", vm.PunchInTime);
                    cmdUpdate.Parameters.AddWithValue("@PunchOutTime", vm.PunchOutTime);
                    cmdUpdate.Parameters.AddWithValue("@PunchOutTimeNextday", vm.PunchOutTimeNextday);
                    cmdUpdate.Parameters.AddWithValue("@PunchNextDay", vm.PunchNextDay);
                    cmdUpdate.Parameters.AddWithValue("@IsManual", vm.IsManual);
                    transResult = (int)cmdUpdate.ExecuteNonQuery();
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    if (transResult <= 0)
                    {
                    }
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    retResults[1] = "This Attendance Migration already used!";
                    throw new ArgumentNullException("Please Input Attendance Migration Value", "");
                }
                #region Commit
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
                    retResults[1] = "Unexpected error to update Attendance Migration.";
                    throw new ArgumentNullException("", "");
                }
                #endregion Commit
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[3] = sqlText;
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

        //Kamrul
        public List<AttendanceMigrationVM> SelectMigration(string DailyDate, bool IsManual, string ProcessTime, string EmployeeId = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
            List<AttendanceMigrationVM> vms = new List<AttendanceMigrationVM>();
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
                #region sqlText
                sqlText = @"
SELECT
 Id
,AttendanceStructureId
,GroupId
,EmployeeId
,ProxyID
,DailyDate
,PunchInTime
,PunchOutTime
,PunchOutTimeNextday
,PunchNextDay
,IsManual
,PInTime
,PInGrace
,ISNULL(POutGrace, 0) POutGrace
,PInTimeStart
,PInTimeEnd
,POutTime
,PLunchTime
,PLunchBreak
,PWorkingHour
,POTTime
,PIsTiff
,PTiffinSTime
,PIsTiffNextDay
,PDeductTiffTime
,PIsIfter
,PIfterSTime
,PIsIfterNextDay
,PDeductIfterTime
,PDinnerSTime
,PIsDinNextDay
,PDeductDinTime
,PIsDeductEarlyOut
,PEarlyOutMin
,PIsDeductLateIn
,PLateInMin
,PIsOTRoundUp
,POTRoundUpMin
,PWorkingMin
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

From AttendanceMigration
where    1=1 
";
                #endregion sqlText
                if (IsManual)
                {
                    sqlText += " and IsManual=1";
                }
                else
                {
                    sqlText += " and ISNULL(IsManual,0)=0";
                }
                if (!string.IsNullOrWhiteSpace(DailyDate))
                {
                    sqlText += " and DailyDate=@DailyDate";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += " and EmployeeId=@EmployeeId";
                }
                if (!string.IsNullOrWhiteSpace(ProcessTime))
                {
                    //sqlText += " and ProcessTime=@ProcessTime";
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (!string.IsNullOrWhiteSpace(DailyDate))
                {
                    objComm.Parameters.AddWithValue("@DailyDate", Ordinary.DateToString(DailyDate));
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                if (!string.IsNullOrWhiteSpace(ProcessTime))
                {
                    objComm.Parameters.AddWithValue("@ProcessTime", ProcessTime);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AttendanceMigrationVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.AttendanceStructureId = Convert.ToInt32(dr["AttendanceStructureId"]);
                    vm.GroupId = dr["GroupId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ProxyID = dr["ProxyID"].ToString();
                    vm.DailyDate = Ordinary.StringToDate(dr["DailyDate"].ToString());
                    vm.PunchInTime = Ordinary.StringToTime(dr["PunchInTime"].ToString());
                    vm.PunchOutTime = Ordinary.StringToTime(dr["PunchOutTime"].ToString());
                    vm.PunchOutTimeNextday = Ordinary.StringToTime(dr["PunchOutTimeNextday"].ToString());
                    vm.PunchNextDay = Convert.ToBoolean(dr["PunchNextDay"]);
                    vm.IsManual = Convert.ToBoolean(dr["IsManual"]);
                    vm.PInTime = Ordinary.StringToTime(dr["PInTime"].ToString());
                    vm.PInGrace = Convert.ToInt32(dr["PInGrace"]);
                    vm.POutGrace = Convert.ToInt32(dr["POutGrace"]);
                    vm.PInTimeStart = Ordinary.StringToTime(dr["PInTimeStart"].ToString());
                    vm.PInTimeEnd = Ordinary.StringToTime(dr["PInTimeEnd"].ToString());
                    vm.POutTime = Ordinary.StringToTime(dr["POutTime"].ToString());
                    vm.PLunchTime = Ordinary.StringToTime(dr["PLunchTime"].ToString());
                    vm.PLunchBreak = Convert.ToInt32(dr["PLunchBreak"]);
                    vm.PWorkingHour = Convert.ToDecimal(dr["PWorkingHour"]);
                    vm.POTTime = Convert.ToInt32(dr["POTTime"]);
                    vm.PIsTiff = Convert.ToBoolean(dr["PIsTiff"]);
                    vm.PTiffinSTime = Ordinary.StringToTime(dr["PTiffinSTime"].ToString());
                    vm.PIsTiffNextDay = Convert.ToBoolean(dr["PIsTiffNextDay"]);
                    vm.PDeductTiffTime = Convert.ToInt32(dr["PDeductTiffTime"]);
                    vm.PIsIfter = Convert.ToBoolean(dr["PIsIfter"]);
                    vm.PIfterSTime = Ordinary.StringToTime(dr["PIfterSTime"].ToString());
                    vm.PIsIfterNextDay = Convert.ToBoolean(dr["PIsIfterNextDay"]);
                    vm.PDeductIfterTime = Convert.ToInt32(dr["PDeductIfterTime"]);
                    vm.PDinnerSTime = Ordinary.StringToTime(dr["PDinnerSTime"].ToString());
                    vm.PIsDinNextDay = Convert.ToBoolean(dr["PIsDinNextDay"]);
                    vm.PDeductDinTime = Convert.ToInt32(dr["PDeductDinTime"]);
                    vm.PIsDeductEarlyOut = Convert.ToBoolean(dr["PIsDeductEarlyOut"]);
                    vm.PEarlyOutMin = Convert.ToInt32(dr["PEarlyOutMin"]);
                    vm.PIsDeductLateIn = Convert.ToBoolean(dr["PIsDeductLateIn"]);
                    vm.PLateInMin = Convert.ToInt32(dr["PLateInMin"]);
                    vm.PIsOTRoundUp = Convert.ToBoolean(dr["PIsOTRoundUp"]);
                    vm.POTRoundUpMin = Convert.ToInt32(dr["POTRoundUpMin"]);
                    vm.PWorkingMin = Convert.ToInt32(dr["PWorkingMin"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
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
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
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
            return vms;
        }
        //Kamrul

        #endregion Methods

        #region New Methods
        ////==================SelectFromDownloadDataMultiDate=================
        public class SearchResult
        {
            public string Unit_name { get; set; }
            public string Registration_id { get; set; }
            public string Access_time { get; set; }
            public string Access_date { get; set; }
        }

        static DataTable AddJTokenListToDataTable(IList<JToken> jTokenList, DataTable dataTable)
        {
            try
            {
                if (!dataTable.Columns.Contains("CHECKTIME"))
                {
                    dataTable.Columns.Add("CHECKTIME", typeof(string));
                }
                if (!dataTable.Columns.Contains("USERID"))
                {
                    dataTable.Columns.Add("USERID", typeof(string));
                }               
                if (!dataTable.Columns.Contains("IsMigrate"))
                {
                    dataTable.Columns.Add("IsMigrate", typeof(string));
                }
                if (!dataTable.Columns.Contains("IsActive"))
                {
                    dataTable.Columns.Add("IsActive", typeof(string));
                }
                if (!dataTable.Columns.Contains("IsArchive"))
                {
                    dataTable.Columns.Add("IsArchive", typeof(string));
                }
                if (!dataTable.Columns.Contains("CreatedFrom"))
                {
                    dataTable.Columns.Add("CreatedFrom", typeof(string));
                }              
                if (!dataTable.Columns.Contains("CreatedBy"))
                {
                    dataTable.Columns.Add("CreatedBy", typeof(string));
                }
                foreach (JToken jToken in jTokenList)
                {
                    DataRow newRow = dataTable.NewRow();

                    // Map JToken properties to DataTable columns                    
                    newRow["CHECKTIME"] = jToken["access_date"].ToObject<string>() + " " + jToken["access_time"].ToObject<string>();
                    newRow["USERID"] = jToken["registration_id"].ToObject<string>();
                    newRow["IsMigrate"] = false;
                    newRow["IsActive"] = true;
                    newRow["IsArchive"] = false;
                    newRow["CreatedFrom"] = "API";                  
                    newRow["CreatedBy"] = "MC";

                    // Add the new row to the DataTable
                    dataTable.Rows.Add(newRow);
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public string[] SelectFromDownloadDataMultiDate(AttendanceMigrationVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            #endregion
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable McData = new DataTable();

            SqlConnection currConnMc = null;
            SqlTransaction transactionMc = null;
            //List<AttendanceMigrationVM> VMs = new List<AttendanceMigrationVM>();
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
            int transResult = 0;
            #endregion
            CommonDAL cDal = new CommonDAL();
            #region DataReadFromMc
            #region Try

            try
            {
                #region open connection and transaction Machine


                currConnMc = _dbsqlConnection.GetConnectionNoPoolMachine();

                currConnMc.Open();


                transactionMc = currConnMc.BeginTransaction("");

                #endregion  open connection and transaction Machine

                string sqlTextMc = "";

                if (CompanyName.ToUpper() == "G4S")
                {
                    var url = "https://rumytechnologies.com/rams/json_api";

                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpRequest.Method = "POST";

                    httpRequest.Accept = "application/json";
                    httpRequest.ContentType = "application/json";


                    paramVM.AttendanceDateFrom = Convert.ToDateTime(paramVM.AttendanceDateFrom).ToString("yyyy-MM-dd");
                    paramVM.AttendanceDateTo = Convert.ToDateTime(paramVM.AttendanceDateTo).ToString("yyyy-MM-dd");

                    var data = @"{
	                            ""operation"":""fetch_log"",
	                            ""auth_user"":""G4S"",
	                            ""auth_code"":""s2vbn06ssks0pfk152kyf2vpkdn25x1"",
	                            ""start_date"": """ + paramVM.AttendanceDateFrom + @""",
	                            ""end_date"": """ + paramVM.AttendanceDateTo + @""",
	                            ""start_time"":""1:00:01"",
	                            ""end_time"":""23:59:01""
	                            }";

                    using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                    {
                        streamWriter.Write(data);
                    }

                    string stresult;
                    var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        stresult = streamReader.ReadToEnd();
                    }

                    JObject search = JObject.Parse(stresult);

                    // get JSON result objects into a list
                    IList<JToken> results = search["log"].Children().ToList();
                    IList<SearchResult> searchResults = new List<SearchResult>();

                    foreach (JToken result in results)
                    {
                        // JToken.ToObject is a helper method that uses JsonSerializer internally
                        SearchResult searchResult = result.ToObject<SearchResult>();

                        ////Save these data in your database

                        DataTable dataTable = new DataTable();

                        // Add JToken list to DataTable
                        McData = AddJTokenListToDataTable(results, dataTable);
                    }

                }
                if (CompanyName.ToUpper() == "BOLLORE")
                {
                    sqlTextMc = @"
                               select emp_code USERID,punch_time CHECKTIME,''PunchData,''ProxyID,''ProxyID1,''PunchDate,''PunchTime ,0 IsMigrate
                                ,1 IsActive,0 IsArchive,'Mc'CreatedBy,'' CreatedAt, 'Local'CreatedFrom 
                                from iclock_transaction  where isnull(IsProcessed,0)=0
                                ";

                }
                if (CompanyName.ToUpper() == "SSL")
                {
                    sqlTextMc = @"
                                select USERID,CHECKTIME,''PunchData,''ProxyID,''ProxyID1,''PunchDate,''PunchTime ,0 IsMigrate
                                ,1 IsActive,0 IsArchive,'Mc'CreatedBy,'' CreatedAt, 'Local'CreatedFrom 
                                from CHECKINOUT where IsProcessed=0
                                ";
                }

                if (CompanyName.ToUpper() == "G4S")
                {
                    sqlTextMc = @"
                               select PunchData CHECKTIME,''PunchData,''ProxyID,''ProxyID1,''PunchDate,''PunchTime ,'false' IsMigrate
                                ,'true' IsActive,'false' IsArchive,'APP'CreatedBy,CreatedAt, 'APP'CreatedFrom 
                                from DownloadData  where isnull(IsMigrate,0)=0
                                ";
                }

                SqlCommand objComm = new SqlCommand(sqlTextMc, currConnMc);

                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.SelectCommand.Transaction = transactionMc;
                da.Fill(McData);

                #region sql statement

                //SqlCommand cmdUpdate = new SqlCommand(sqlTextMc, currConnMc);
                //cmdUpdate.Transaction = transactionMc;
                //transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion


                if (transactionMc != null && transactionMc.Connection != null)
                {
                    transactionMc.Commit();
                }

            }

            #endregion Try
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[3] = sqlText;
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transactionMc.Connection != null)
                {
                    transactionMc.Rollback();
                }
                return retResults;
            }
            #endregion
            #region Finally
            finally
            {

                if (currConnMc != null)
                {
                    if (currConnMc.State == ConnectionState.Open)
                    {
                        currConnMc.Close();
                    }
                }

            }
            #endregion
            #endregion DataReadFromMc

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
                    currConn = _dbsqlConnection.GetConnectionNoPool();
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


                //  RemovePunchTimeEmptyRows(McData);

                for (int i = 0; i < McData.Rows.Count; i++)
                {
                    if (McData.Rows[i]["CHECKTIME"].ToString().Length > 10)
                    {
                        McData.Rows[i]["PunchData"] = McData.Rows[i]["CHECKTIME"].ToString();
                        McData.Rows[i]["ProxyID"] = McData.Rows[i]["USERID"].ToString();
                        McData.Rows[i]["ProxyID1"] = McData.Rows[i]["USERID"].ToString();
                        McData.Rows[i]["CreatedAt"] = Convert.ToDateTime(McData.Rows[i]["CHECKTIME"].ToString()).ToString("yyyyMMdd");
                        McData.Rows[i]["PunchDate"] = Convert.ToDateTime(McData.Rows[i]["CHECKTIME"].ToString()).ToString("yyyyMMdd");
                        McData.Rows[i]["PunchTime"] = Convert.ToDateTime(McData.Rows[i]["CHECKTIME"].ToString()).ToString("HHmm");
                    }             

                }
               
                McData.Columns.Remove("CHECKTIME");
                McData.Columns.Remove("USERID");
               
                DataTable McDataCopy = McData.Copy();

                string ProcessTime = DateTime.Now.ToString("yyyyMMddhhmm");
                paramVM.ProcessTime = ProcessTime;

                retResults = cDal.BulkInsert("DownloadData", McDataCopy, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], "");
                }


                EmployeeDAL _eDal = new EmployeeDAL();

                AttendanceMigrationDAL _attnMDal = new AttendanceMigrationDAL();


                int length = 0;
                length = Convert.ToInt32((Convert.ToDateTime(paramVM.AttendanceDateTo) - Convert.ToDateTime(paramVM.AttendanceDateFrom)).TotalDays) + 1;
                string varDayOfWeek = "";
                for (int i = 0; i < length; i++)
                {
                    var attnDate = Convert.ToDateTime(paramVM.AttendanceDateFrom).AddDays(i).Date.ToString("dd-MMM-yyyy");
                    paramVM.AttendanceDate = attnDate;


                    #region EmployeeWeeklyHoliday
                    {
                        //Using date and DayOfWeek get EmployeeId from EmployeeJob (Distinct EmployeeId Union First and SecondHoliday) then Insert
                        varDayOfWeek = Convert.ToDateTime(attnDate).DayOfWeek.ToString();

                        sqlText = " ";
                        sqlText = @" 
    --declare @DayOfWeek as nvarchar(10)
    --declare @DailyDate as nvarchar(14)
    --set @DayOfWeek = 'Sunday'
    ----------------------------------
    DELETE  dbo.EmployeeWeeklyHoliday
    WHERE 1=1 
    AND DailyDate = @DailyDate 
    AND ISNULL(IsManual,0) = 0


    --------------------------------------
    Insert Into dbo.EmployeeWeeklyHoliday(
    EmployeeId,DayOfWeek,DailyDate
    ,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom
    )
    Select distinct 
    a.EmployeeId, @DayOfWeek, @DailyDate 
    ,'1' Remarks,'1' IsActive,'0' IsArchive,'1' CreatedBy,'1' CreatedAt,'1' CreatedFrom,'1' LastUpdateBy,'1' LastUpdateAt,'1' LastUpdateFrom

    from(
    Select EmployeeId, FirstHoliday Holiday from EmployeeJob
    Union All
    Select EmployeeId, SecondHoliday Holiday from EmployeeJob
    ) as a
    where 1=1 
    and a.Holiday =  @DayOfWeek
    and a.EmployeeId Not In 
    (
    SELECT EmployeeId From dbo.EmployeeWeeklyHoliday
    WHERE DailyDate = @DailyDate
    )

";

                        SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                        objComm.Parameters.AddWithValue("@DayOfWeek", varDayOfWeek);
                        objComm.Parameters.AddWithValue("@DailyDate", Ordinary.DateToString(attnDate));

                        var exec = objComm.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exec);

                    }

                    #endregion


                    retResults = _attnMDal.SelectFromDownloadData(paramVM, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    #region Update Attendance DayStatus as WH
                    {
                        sqlText = "";
                        sqlText = @"
--declare @DailyDate as nvarchar(14)

update AttendanceDailyNew set DayStatus='WH'
from EmployeeWeeklyHoliday
where EmployeeWeeklyHoliday.dailyDate=AttendanceDailyNew.DailyDate
and AttendanceDailyNew.EmployeeId=EmployeeWeeklyHoliday.EmployeeId
and EmployeeWeeklyHoliday.DailyDate = @DailyDate
and EmployeeWeeklyHoliday.IsActive = 1

update AttendanceDailyNew set DayStatus='Leave'
from EmployeeLeaveDetail ed
where ed.LeaveDate=AttendanceDailyNew.DailyDate
and AttendanceDailyNew.EmployeeId=ed.EmployeeId
and ed.LeaveDate = @DailyDate
and ed.IsActive = 1 


";
                        SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                        objComm.Parameters.AddWithValue("@DailyDate", Ordinary.DateToString(attnDate));

                        var exec = objComm.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exec);

                    }
                    #endregion

                }

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null && transaction.Connection != null)
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
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[3] = sqlText;
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction.Connection != null)
                {
                    if (Vtransaction == null) { transaction.Rollback(); }
                }
                return retResults;
            }
            #endregion
            #region Finally
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

        static void RemovePunchTimeEmptyRows(DataTable dataTable)
        {
            for (int i = dataTable.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dataTable.Rows[i];
                var data = row["CHECKTIME"].ToString();

                if (data.Length < 10)
                {
                    dataTable.Rows.RemoveAt(i);
                }
            }
        }

        //==================SelectFromDownloadData=================
        public string[] SelectFromDownloadData(AttendanceMigrationVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //List<AttendanceMigrationVM> VMs = new List<AttendanceMigrationVM>();
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
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
                    currConn = _dbsqlConnection.GetConnectionNoPool();
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

                #region VarVariables
                DownloadDataDAL _ddDal = new DownloadDataDAL();
                DownloadDataVM ddVM = new DownloadDataVM();
                List<DownloadDataVM> ddVMs = new List<DownloadDataVM>();
                List<AttendanceMigrationVM> VMs = new List<AttendanceMigrationVM>();
                AttendanceMigrationDAL _attnMDal = new AttendanceMigrationDAL();
                #endregion VarVariables

                ddVMs = _ddDal.SelectIsMigrated("0", currConn, transaction);

                #region Insert Into AttendanceMigration

                //string ProcessTime = DateTime.Now.ToString("yyyyMMddhhmmss");
                foreach (var item in ddVMs)
                {
                    vm = new AttendanceMigrationVM();
                    vm.ProxyID = item.ProxyID1;
                    //vm.ProxyID1 = item.ProxyID;
                    vm.AttendanceDate = item.PunchDate;
                    vm.AttendanceTime = item.PunchTime;

                    vm.CreatedAt = paramVM.CreatedAt;
                    vm.CreatedBy = paramVM.CreatedBy;
                    vm.CreatedFrom = paramVM.CreatedFrom;
                    vm.ProcessTime = paramVM.ProcessTime;
                    VMs.Add(vm);
                }


                if (VMs != null && VMs.Count > 0)
                {
                    retResults = _attnMDal.SelectToInsert(VMs, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                }
                #endregion

                #region Update DownloadData

                if (ddVMs != null && ddVMs.Count > 0)
                {
                    retResults = _ddDal.FieldUpdateList(ddVMs, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                }

                #endregion

                #region DayStatus

                //"Weekly", "Govt", "Festival", "Special"
                HoliDayDAL _holiDayDAL = new HoliDayDAL();
                HoliDayVM holiDayVM = new HoliDayVM();
                holiDayVM = _holiDayDAL.SelectByDate(paramVM.AttendanceDate, currConn, transaction);
                string holidayType = "";
                string DayStatus = "";
                holidayType = holiDayVM.HoliDayType;
                DayStatus = "R";

                if (holidayType == "Weekly")
                {
                    DayStatus = "WH";
                }
                else if (holidayType == "Govt")
                {
                    DayStatus = "GH";
                }
                else if (holidayType == "Festival")
                {
                    DayStatus = "FH";
                }
                else if (holidayType == "Special")
                {
                    DayStatus = "SH";
                }
                else
                {
                    DayStatus = "R";
                }
                #endregion DayStatus

                #region Insert Into DailyAttendanceProcess

                DailyAttendanceProcessDAL _dailyAttnProcessDal = new DailyAttendanceProcessDAL();
                List<AttendanceMigrationVM> attnMigrationVMs = new List<AttendanceMigrationVM>();
                attnMigrationVMs = SelectMigration(paramVM.AttendanceDate, false, paramVM.ProcessTime, "", currConn, transaction);

                if (attnMigrationVMs != null && attnMigrationVMs.Count > 0)
                {
                    retResults = _dailyAttnProcessDal.ProcessMultiple(paramVM.AttendanceDate, attnMigrationVMs, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                }

                #endregion Insert Into DailyAttendanceProcess

                #region Absent Entry
                //ManualRoster


                if (paramVM.AttendanceSystem != "ManualRoster")
                {
                    retResults = _attnMDal.AttendanceMigrationAbsentInsert(paramVM.AttendanceDate, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    retResults = _attnMDal.AttendanceDailyAbsentInsert(paramVM.AttendanceDate, DayStatus, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                }

                #endregion Absent Entry

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null && transaction.Connection != null)
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

            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[3] = sqlText;
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction.Connection != null)
                {
                    if (Vtransaction == null) { transaction.Rollback(); }
                }
                return retResults;
            }
            #endregion

            #region Finally
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

        ////==================SelectToInsert=================
        public string[] SelectToInsert(List<AttendanceMigrationVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //List<AttendanceMigrationVM> VMs = new List<AttendanceMigrationVM>();
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
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

                #region New Variables
                AttendanceMigrationDAL _attnMigrationDal = new AttendanceMigrationDAL();
                AttendanceMigrationVM attnMigrationVM = new AttendanceMigrationVM();

                EmployeeStructureGroupDAL _empStGroupDal = new EmployeeStructureGroupDAL();
                EmployeeStructureGroupVM empStGroupVM = new EmployeeStructureGroupVM();

                AttendanceRosterAllDAL _attnRosterAllDAL = new AttendanceRosterAllDAL();
                AttendanceRosterAllVM attnRosterAllVM = new AttendanceRosterAllVM();

                AttendanceStructureDAL _attnStructureDal = new AttendanceStructureDAL();
                AttendanceStructureVM attnStructureVM = new AttendanceStructureVM();

                EmployeeInfoDAL _empDal = new EmployeeInfoDAL();
                EmployeeInfoVM empVM = new EmployeeInfoVM();

                string employeeId = "";
                string employeeCode = "";
                string proxyId = "";

                string attendanceDate = "";
                string attendanceTime = "";
                bool isManual = false;

                string CreatedAt = "";
                string CreatedBy = "";
                string CreatedFrom = "";

                string attendanceGroupId = "";
                string attendanceStructureId = "";
                #endregion

                #region Variables

                int attnTime = 0;
                int inTimeStart = 0;
                int inTimeEnd = 0;
                int outTime = 0;

                #endregion

                foreach (var item in VMs)
                {
                    attnTime = 0;
                    inTimeStart = 0;
                    inTimeEnd = 0;
                    outTime = 0;

                    #region Data Fethcing Code, EmployeeId

                    if (!string.IsNullOrWhiteSpace(item.ProxyID)) //Get EmployeeId by ProxyID
                    {
                        string[] conFields = { "AttnUserId" };
                        string[] conValues = { item.ProxyID };
                        empVM = _empDal.SelectCommonFields(conFields, conValues, currConn, transaction).FirstOrDefault();

                        if (empVM != null)
                        {
                            item.IsManual = false;
                            item.EmployeeId = empVM.Id;
                            item.EmployeeCode = empVM.Code;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(item.EmployeeCode)) //Get EmployeeId by EmployeeCode
                    {
                        string[] conFields = { "Code" };
                        string[] conValues = { item.EmployeeCode };
                        if (empVM != null)
                        {
                            empVM = _empDal.SelectCommonFields(conFields, conValues, currConn, transaction).FirstOrDefault();
                            item.EmployeeId = empVM.Id;
                        }

                    }
                    #endregion

                    #region Debugging
                    if (item.EmployeeId == "1_8")
                    {
                        int a = 1;
                    }
                    #endregion

                    if (string.IsNullOrWhiteSpace(item.EmployeeId))
                    {
                        continue;
                    }

                    #region tempRegion
                    employeeId = item.EmployeeId;
                    employeeCode = item.EmployeeCode;
                    proxyId = item.ProxyID;

                    attendanceDate = item.AttendanceDate;
                    attendanceTime = item.AttendanceTime;
                    isManual = item.IsManual;

                    CreatedAt = item.CreatedAt;
                    CreatedBy = item.CreatedBy;
                    CreatedFrom = item.CreatedFrom;
                    #endregion tempRegion

                    #region Data Fethcing

                    {
                        string[] conFields = { "EmployeeId", "DailyDate" };
                        string[] conValues = { employeeId, Ordinary.DateToString(attendanceDate) };
                        attnMigrationVM = _attnMigrationDal.SelectByMultiCondition(conFields, conValues, currConn, transaction).FirstOrDefault();
                    }
                    #endregion Data Fetching

                    #region Preparing Data For Insert/Update

                    //////BackTrack_PunchNextDay:
                    if (attnMigrationVM != null && attnMigrationVM.Id > 0)
                    {
                        vm = new AttendanceMigrationVM();

                        #region Assign Data

                        vm = attnMigrationVM;

                        #region Condition Check
                        if (!string.IsNullOrWhiteSpace(attendanceTime))
                        {
                            attnTime = Convert.ToInt32(Ordinary.TimeToString(attendanceTime));
                            inTimeStart = Convert.ToInt32(Ordinary.TimeToString(attnMigrationVM.PInTimeStart));
                            inTimeEnd = Convert.ToInt32(Ordinary.TimeToString(attnMigrationVM.PInTimeEnd));
                            outTime = Convert.ToInt32(Ordinary.TimeToString(attnMigrationVM.POutTime));

                            #region Comments

                            ////if (attnTime >= inTimeStart && attnTime <= inTimeEnd)
                            ////{
                            ////    int storedPunchInTime = Convert.ToInt32(Ordinary.TimeToString(attnMigrationVM.PunchInTime));
                            ////    if (attnTime < storedPunchInTime || storedPunchInTime == 0)
                            ////    {
                            ////        vm.PunchInTime = attendanceTime;
                            ////    }
                            ////    else
                            ////    {
                            ////        vm.PunchInTime = attnMigrationVM.PunchInTime;
                            ////    }
                            ////}
                            ////else if (attnTime > inTimeEnd && attnTime <= 2359)
                            ////{
                            ////    int storedPunchOutTime = Convert.ToInt32(Ordinary.TimeToString(attnMigrationVM.PunchOutTime));
                            ////    if (attnTime > storedPunchOutTime || storedPunchOutTime == 0)
                            ////    {
                            ////        vm.PunchOutTime = attendanceTime;
                            ////    }
                            ////    else
                            ////    {
                            ////        vm.PunchOutTime = attnMigrationVM.PunchOutTime;
                            ////    }
                            ////    vm.PunchNextDay = false;
                            ////}
                            ////else
                            #endregion

                            if (attnTime < inTimeStart)
                            {
                                #region Comments

                                //////int storedPunchOutTimeNextday = Convert.ToInt32(Ordinary.TimeToString(attnMigrationVM.PunchOutTimeNextday));
                                //////if (attnTime > storedPunchOutTimeNextday)
                                //////{
                                //////    vm.PunchOutTimeNextday = attendanceTime;
                                //////}
                                //////else
                                //////{
                                //////    vm.PunchOutTimeNextday = attnMigrationVM.PunchOutTimeNextday;
                                //////}

                                //////vm.PunchNextDay = true;

                                #endregion

                                #region Update PunchNextDay for Previous Day

                                vm.DailyDate = Ordinary.DateToString(Convert.ToDateTime(attendanceDate).AddDays(-1).ToString());

                                string[] aConFields = { "EmployeeId", "DailyDate" };
                                string[] aConValues = { employeeId, vm.DailyDate };
                                attnMigrationVM = new AttendanceMigrationVM();
                                attnMigrationVM = _attnMigrationDal.SelectByMultiCondition(aConFields, aConValues, currConn, transaction).FirstOrDefault();

                                if (attnMigrationVM != null && attnMigrationVM.Id > 0)
                                {
                                    retResults = UpdateAttendanceMigration(attnMigrationVM, attendanceTime, item, isManual, currConn, transaction);
                                    if (retResults[0] == "Fail")
                                    {
                                        throw new ArgumentNullException(retResults[1], "");
                                    }

                                    continue;
                                }

                                #endregion
                            }
                            else
                            {
                                #region Update Current Day

                                retResults = UpdateAttendanceMigration(attnMigrationVM, attendanceTime, item, isManual, currConn, transaction);
                                if (retResults[0] == "Fail")
                                {
                                    throw new ArgumentNullException(retResults[1], "");
                                }

                                continue;

                                #endregion

                            }
                        }
                        #endregion

                        #endregion

                        #region Comments

                        ////vm.IsManual = isManual;
                        ////vm.Id = Convert.ToInt32(attnMigrationVM.Id);
                        ////vm.LastUpdateAt = item.CreatedAt;
                        ////vm.LastUpdateBy = item.CreatedBy;
                        ////vm.LastUpdateFrom = item.CreatedFrom;
                        ////vm.ProcessTime = item.ProcessTime;

                        ////#region Update
                        ////retResults = _attnMigrationDal.Update(vm, currConn, transaction);
                        ////if (retResults[0] == "Fail")
                        ////{
                        ////    throw new ArgumentNullException(retResults[1], "");
                        ////}
                        ////#endregion Update

                        #endregion

                    }
                    else
                    {
                        #region Data Fethcing
                        #region Employee Structure Group
                        if (employeeId != "1_0")
                        {


                            empStGroupVM = _empStGroupDal.SelectByEmployee(employeeId, currConn, transaction);
                            if (empStGroupVM == null || empStGroupVM.Id <= 0)
                            {
                                retResults[1] = "Employee Structure Group Not Assigned For this Employee! Code: " + employeeCode;
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        attendanceGroupId = empStGroupVM.EmployeeGroupId;
                        #endregion Employee Structure Group
                        #region Attendance Roster
                        string[] rConFields = { "ToDate", "AttendanceGroupId" };
                        string[] rConValues = { Ordinary.DateToString(attendanceDate), attendanceGroupId };

                        attnRosterAllVM = _attnRosterAllDAL.SelectAllByParameters(attendanceDate, attendanceGroupId, currConn, transaction).FirstOrDefault();
                        if (attnRosterAllVM == null || string.IsNullOrWhiteSpace(attnRosterAllVM.Id))
                        {
                            continue;
                            //retResults[1] = "Attendance Roster Not Assigned For this Employee! Code: " + employeeCode + " For the date: " + attendanceDate;
                            //throw new ArgumentNullException(retResults[1], "");
                        }
                        attendanceStructureId = attnRosterAllVM.AttendanceStructureId;
                        #endregion Attendance Roster
                        #region Attendance Structure

                        attnStructureVM = _attnStructureDal.SelectById(attendanceStructureId, currConn, transaction);
                        if (attnStructureVM == null || attnStructureVM.Id <= 0)
                        {
                            retResults[1] = "Attendance Structure Not Assigned For this Employee! Code: " + employeeCode;
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion Attendance Structure
                        #endregion Data Fethcing

                        vm = new AttendanceMigrationVM();

                        #region Condition Check


                        if (!string.IsNullOrWhiteSpace(attendanceTime))
                        {
                            attnTime = Convert.ToInt32(Ordinary.TimeToString(attendanceTime));
                            inTimeStart = Convert.ToInt32(Ordinary.TimeToString(attnStructureVM.InTimeStart));
                            inTimeEnd = Convert.ToInt32(Ordinary.TimeToString(attnStructureVM.InTimeEnd));
                            outTime = Convert.ToInt32(Ordinary.TimeToString(attnStructureVM.OutTime));

                            if (attnTime < inTimeStart)
                            {
                                vm.PunchOutTimeNextday = attendanceTime;
                                vm.PunchNextDay = true;

                                #region Update PunchNextDay for Previous Day
                                vm.DailyDate = Ordinary.DateToString(Convert.ToDateTime(attendanceDate).AddDays(-1).ToString());

                                string[] aConFields = { "EmployeeId", "DailyDate" };
                                string[] aConValues = { employeeId, vm.DailyDate };
                                attnMigrationVM = new AttendanceMigrationVM();
                                attnMigrationVM = _attnMigrationDal.SelectByMultiCondition(aConFields, aConValues, currConn, transaction).FirstOrDefault();

                                if (attnMigrationVM != null && attnMigrationVM.Id > 0)
                                {
                                    retResults = UpdateAttendanceMigration(attnMigrationVM, attendanceTime, item, isManual, currConn, transaction);
                                    if (retResults[0] == "Fail")
                                    {
                                        throw new ArgumentNullException(retResults[1], "");
                                    }

                                    continue;
                                    ////goto BackTrack_PunchNextDay;
                                }
                                #endregion
                            }
                            else
                            {
                                #region Insert Current Day

                                vm.PunchNextDay = false;

                                if (attnTime >= inTimeStart && attnTime <= inTimeEnd)
                                {
                                    vm.PunchInTime = attendanceTime;
                                }
                                else if (attnTime > inTimeEnd && attnTime <= 2359)
                                {
                                    vm.PunchOutTime = attendanceTime;
                                }

                                #region Data Assign
                                vm.CreatedAt = CreatedAt;
                                vm.CreatedBy = CreatedBy;
                                vm.CreatedFrom = CreatedFrom;
                                vm.AttendanceStructureId = Convert.ToInt32(attendanceStructureId);
                                vm.GroupId = attendanceGroupId;
                                vm.EmployeeId = employeeId;
                                vm.ProxyID = proxyId;
                                vm.ProcessTime = item.ProcessTime;

                                if (string.IsNullOrWhiteSpace(vm.DailyDate))
                                {
                                    vm.DailyDate = attendanceDate;
                                }

                                vm.PInTime = attnStructureVM.InTime;
                                vm.PInGrace = attnStructureVM.InGrace;
                                vm.POutGrace = attnStructureVM.OutGrace;
                                vm.PInTimeStart = attnStructureVM.InTimeStart;
                                vm.PInTimeEnd = attnStructureVM.InTimeEnd;
                                vm.POutTime = attnStructureVM.OutTime;
                                vm.PLunchTime = attnStructureVM.LunchTime;
                                vm.PLunchBreak = attnStructureVM.LunchBreak;
                                vm.PWorkingHour = attnStructureVM.WorkingHour;
                                vm.POTTime = attnStructureVM.OTTime;
                                vm.PIsTiff = attnStructureVM.IsTiff;
                                vm.PTiffinSTime = attnStructureVM.TiffinSTime;
                                vm.PIsTiffNextDay = attnStructureVM.IsTiffNextDay;
                                vm.PDeductTiffTime = attnStructureVM.DeductTiffTime;
                                vm.PIsIfter = attnStructureVM.IsIfter;
                                vm.PIfterSTime = attnStructureVM.IfterSTime;
                                vm.PIsIfterNextDay = attnStructureVM.IsIfterNextDay;
                                vm.PDeductIfterTime = attnStructureVM.DeductIfterTime;
                                vm.PDinnerSTime = attnStructureVM.DinnerSTime;
                                vm.PIsDinNextDay = attnStructureVM.IsDinNextDay;
                                vm.PDeductDinTime = attnStructureVM.DeductDinTime;
                                vm.PIsDeductEarlyOut = attnStructureVM.IsDeductEarlyOut;
                                vm.PEarlyOutMin = attnStructureVM.EarlyOutMin;
                                vm.PIsDeductLateIn = attnStructureVM.IsDeductLateIn;
                                vm.PLateInMin = attnStructureVM.LateInMin;
                                vm.PIsOTRoundUp = attnStructureVM.IsOTRoundUp;
                                vm.POTRoundUpMin = attnStructureVM.OTRoundUpMin;
                                vm.PWorkingMin = attnStructureVM.WorkingMin;
                                vm.PTiffinMin = attnStructureVM.TiffinMin;
                                vm.PDinnerMin = attnStructureVM.DinnerMin;
                                vm.PIfterMin = attnStructureVM.IfterMin;
                                #endregion Data Assign

                                vm.IsManual = isManual;

                                #region Insert
                                retResults = _attnMigrationDal.Insert(vm, currConn, transaction);
                                if (retResults[0] == "Fail")
                                {
                                    throw new ArgumentNullException(retResults[1], "");
                                }

                                #endregion Insert

                                #endregion
                            }

                        }
                        #endregion Condition Check


                    }
                    #endregion Preparing Data For Insert/Update
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
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = "0";
                #endregion SuccessResult
            }
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[3] = sqlText;
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            #endregion
            #region Finally
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

        #region Comments

        private string[] UpdateAttendanceMigration(AttendanceMigrationVM attnMigrationVM, string attendanceTime, AttendanceMigrationVM item, bool isManual, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                    currConn = _dbsqlConnection.GetConnectionNoPool();
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

                #region Assign Data
                AttendanceMigrationVM vm = new AttendanceMigrationVM();
                vm = attnMigrationVM;
                #region Condition Check
                if (!string.IsNullOrWhiteSpace(attendanceTime))
                {
                    int attnTime = Convert.ToInt32(Ordinary.TimeToString(attendanceTime));
                    int inTimeStart = Convert.ToInt32(Ordinary.TimeToString(attnMigrationVM.PInTimeStart));
                    int inTimeEnd = Convert.ToInt32(Ordinary.TimeToString(attnMigrationVM.PInTimeEnd));
                    int outTime = Convert.ToInt32(Ordinary.TimeToString(attnMigrationVM.POutTime));

                    if (attnTime >= inTimeStart && attnTime <= inTimeEnd)
                    {
                        int storedPunchInTime = Convert.ToInt32(Ordinary.TimeToString(attnMigrationVM.PunchInTime));
                        if (attnTime < storedPunchInTime || storedPunchInTime == 0)
                        {
                            vm.PunchInTime = attendanceTime;
                        }
                        else
                        {
                            vm.PunchInTime = attnMigrationVM.PunchInTime;
                        }
                    }
                    else if (attnTime > inTimeEnd && attnTime <= 2359)
                    {
                        int storedPunchOutTime = Convert.ToInt32(Ordinary.TimeToString(attnMigrationVM.PunchOutTime));
                        if (attnTime > storedPunchOutTime || storedPunchOutTime == 0)
                        {
                            vm.PunchOutTime = attendanceTime;
                        }
                        else
                        {
                            vm.PunchOutTime = attnMigrationVM.PunchOutTime;
                        }
                        vm.PunchNextDay = false;
                    }
                    else if (attnTime < inTimeStart)
                    {
                        int storedPunchOutTimeNextday = Convert.ToInt32(Ordinary.TimeToString(attnMigrationVM.PunchOutTimeNextday));
                        if (attnTime > storedPunchOutTimeNextday)
                        {
                            vm.PunchOutTimeNextday = attendanceTime;
                        }
                        else
                        {
                            vm.PunchOutTimeNextday = attnMigrationVM.PunchOutTimeNextday;
                        }

                        vm.PunchNextDay = true;
                    }
                }
                #endregion

                #endregion

                vm.IsManual = isManual;
                vm.Id = Convert.ToInt32(attnMigrationVM.Id);
                vm.LastUpdateAt = item.CreatedAt;
                vm.LastUpdateBy = item.CreatedBy;
                vm.LastUpdateFrom = item.CreatedFrom;
                vm.ProcessTime = item.ProcessTime;

                #region Update
                AttendanceMigrationDAL _attnMigrationDal = new AttendanceMigrationDAL();

                retResults = _attnMigrationDal.Update(vm, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], "");
                }
                #endregion Update

            }
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[3] = sqlText;
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction.Connection != null)
                {
                    if (Vtransaction == null) { transaction.Rollback(); }
                }
            }
            #endregion
            #region Finally
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

        #endregion


        ////==================DeleteProcess=================
        public string[] DeleteProcess(AttendanceMigrationVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            AttendanceMigrationVM vm = new AttendanceMigrationVM();
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
                    currConn = _dbsqlConnection.GetConnectionNoPool();
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


                sqlText = "";
                sqlText = @"
delete from AttendanceMigration
where 1=1
and IsManual = 0
and DailyDate >= @AttendanceDateFrom and DailyDate <= @AttendanceDateTo
";

                if (!string.IsNullOrWhiteSpace(paramVM.EmployeeCode))
                {
                    sqlText += @" and  EmployeeId in(select Id from EmployeeInfo where 1=1 and Code = @EmployeeCode) ";
                }


                sqlText += @"
delete from AttendanceDailyNew
where 1=1
--and IsManual = 0
and DailyDate >= @AttendanceDateFrom and DailyDate <= @AttendanceDateTo
";

                if (!string.IsNullOrWhiteSpace(paramVM.EmployeeCode))
                {
                    sqlText += @" and  EmployeeId in(select Id from EmployeeInfo where 1=1 and Code  = @EmployeeCode) ";
                }



                sqlText += @"
update DownloadData set IsMigrate = 0
where  PunchDate >= @AttendanceDateFrom and PunchDate <= @AttendanceDateTo
";

                if (!string.IsNullOrWhiteSpace(paramVM.EmployeeCode))
                {
                    sqlText += @" and  ProxyID1 in(select AttnUserId from EmployeeInfo where 1=1 and Code = @EmployeeCode) ";
                }

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@AttendanceDateFrom", Ordinary.DateToString(paramVM.AttendanceDateFrom));
                cmdUpdate.Parameters.AddWithValue("@AttendanceDateTo", Ordinary.DateToString(paramVM.AttendanceDateTo));

                if (!string.IsNullOrWhiteSpace(paramVM.EmployeeCode))
                {
                    cmdUpdate.Parameters.AddWithValue("@EmployeeCode", paramVM.EmployeeCode);
                }

                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null && transaction.Connection != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Delete Successfully.";
                retResults[2] = "0";
                #endregion SuccessResult
            }
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[3] = sqlText;
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction.Connection != null)
                {
                    if (Vtransaction == null) { transaction.Rollback(); }
                }
                return retResults;
            }
            #endregion
            #region Finally
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

        //==================AttendanceMigrationAbsentInsert =================
        public string[] AttendanceMigrationAbsentInsert(string AttendanceDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            int transResult = 0;
            #region Try
            try
            {
                #region Validation
                //if (string.IsNullOrEmpty(AttendanceMigrationVM.DownloadDataId))
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
                CommonDAL _cDal = new CommonDAL();
                //vm.Id = _cDal.NextId("AttendanceMigration", currConn, transaction);

                #region sqlText
                sqlText = "  ";
                sqlText = "";

                sqlText = @"
--declare @DailyDate as varchar(50)
--declare @DayStatus as varchar(50)
--set @DailyDate='20171028'
--set @DayStatus='R'


INSERT INTO AttendanceMigration( 
AttendanceStructureId,GroupId,EmployeeId,ProxyID,DailyDate,PunchInTime,PunchOutTime,PunchOutTimeNextday,PunchNextDay,IsManual,PInTime,PInGrace,POutGrace,PInTimeStart,PInTimeEnd
,POutTime,PLunchTime,PLunchBreak,PWorkingHour,POTTime,PIsTiff,PTiffinSTime,PIsTiffNextDay,PDeductTiffTime
,PIsIfter,PIfterSTime,PIsIfterNextDay,PDeductIfterTime,PDinnerSTime,PIsDinNextDay,PDeductDinTime,PIsDeductEarlyOut,PEarlyOutMin
,PIsDeductLateIn,PLateInMin,PIsOTRoundUp,POTRoundUpMin,PWorkingMin,PTiffinMin ,PDinnerMin ,PIfterMin  ,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
SELECT 
AttendanceRosterDetail.AttendanceStructureId
,EmployeeStructureGroup.EmployeeGroupId GroupId
,EmployeeInfo.id EmployeeId
,EmployeeInfo.AttnUserId ProxyID
,@DailyDate DailyDate
,'0000'PunchInTime
,'0000'PunchOutTime
,'0000'PunchOutTimeNextday
,'0' PunchNextDay
,0 IsManual
,IsNull(AttendanceStructure.InTime, 0) PInTime
,IsNull(AttendanceStructure.InGrace, 0) PInGrace
,IsNull(AttendanceStructure.OutGrace, 0) POutGrace
,IsNull(AttendanceStructure.InTimeStart, 0) PInTimeStart
,IsNull(AttendanceStructure.InTimeEnd, 0) PInTimeEnd
,IsNull(AttendanceStructure.OutTime, 0) POutTime
,IsNull(AttendanceStructure.LunchTime, 0) PLunchTime
,IsNull(AttendanceStructure.LunchBreak, 0) PLunchBreak
,IsNull(AttendanceStructure.WorkingHour, 0) PWorkingHour
,IsNull(AttendanceStructure.OTTime, 0) POTTime
,IsNull(AttendanceStructure.IsTiff, 0) PIsTiff
,IsNull(AttendanceStructure.TiffinSTime, 0) PTiffinSTime
,IsNull(AttendanceStructure.IsTiffNextDay, 0) PIsTiffNextDay
,IsNull(AttendanceStructure.DeductTiffTime, 0) PDeductTiffTime
,IsNull(AttendanceStructure.IsIfter, 0) PIsIfter
,IsNull(AttendanceStructure.IfterSTime, 0) PIfterSTime
,IsNull(AttendanceStructure.IsIfterNextDay, 0) PIsIfterNextDay
,IsNull(AttendanceStructure.DeductIfterTime, 0) PDeductIfterTime
,IsNull(AttendanceStructure.DinnerSTime, 0) PDinnerSTime
,IsNull(AttendanceStructure.IsDinNextDay, 0) PIsDinNextDay
,IsNull(AttendanceStructure.DeductDinTime, 0) PDeductDinTime
,IsNull(AttendanceStructure.IsDeductEarlyOut, 0) PIsDeductEarlyOut
,IsNull(AttendanceStructure.EarlyOutMin, 0) PEarlyOutMin
,IsNull(AttendanceStructure.IsDeductLateIn, 0) PIsDeductLateIn
,IsNull(AttendanceStructure.LateInMin, 0) PLateInMin
,IsNull(AttendanceStructure.Is_OTRoundUp, 0) PIsOTRoundUp
,IsNull(AttendanceStructure.OTRoundUpMin, 0) POTRoundUpMin
,IsNull(AttendanceStructure.WorkingMin, 0) PWorkingMin
,IsNull(AttendanceStructure.TiffinMin, 0) PTiffinMin
,IsNull(AttendanceStructure.DinnerMin, 0) PDinnerMin
,IsNull(AttendanceStructure.IfterMin, 0) PIfterMin
,'-'Remarks
,1 IsActive
,0 IsArchive
,'Admin'CreatedBy
,'19000101'CreatedAt
,'127.0.0.0' CreatedFrom

from EmployeeInfo
left outer join EmployeeStructureGroup on EmployeeStructureGroup.EmployeeId=EmployeeInfo.id
left outer join AttendanceRosterDetail on AttendanceRosterDetail.AttendanceGroupId=EmployeeStructureGroup.EmployeeGroupId and AttendanceRosterDetail.ToDate=@DailyDate
left outer join AttendanceStructure on AttendanceRosterDetail.AttendanceStructureId=AttendanceStructure.id

 where EmployeeInfo.isactive=1 and  EmployeeInfo.id not in(
select EmployeeId from AttendanceMigration where DailyDate=@DailyDate)

AND AttendanceRosterDetail.AttendanceStructureId is not null
AND EmployeeStructureGroup.EmployeeGroupId != '0'
";

                SqlCommand cmdInsertAttendanceMigration = new SqlCommand(sqlText, currConn, transaction);
                cmdInsertAttendanceMigration.Parameters.AddWithValue("@DailyDate", Ordinary.DateToString(AttendanceDate));
                //cmdInsertAttendanceMigration.Parameters.AddWithValue("@DayStatus", DayStatus);
                cmdInsertAttendanceMigration.ExecuteNonQuery();
                //transResult = Convert.ToInt32(exe);
                //if (transResult <= 0)
                //{
                //    retResults[1] = "Unexpected Error to Migration!";
                //    throw new ArgumentNullException(retResults[1], "");
                //}
                #endregion sqlExecution

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
                retResults[3] = sqlText;
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

        //==================AttendanceDailyAbsentInsert =================
        public string[] AttendanceDailyAbsentInsert(string AttendanceDate, string DayStatus = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            int transResult = 0;
            #region Try
            try
            {
                #region Validation
                //if (string.IsNullOrEmpty(AttendanceMigrationVM.DownloadDataId))
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
                CommonDAL _cDal = new CommonDAL();
                //vm.Id = _cDal.NextId("AttendanceMigration", currConn, transaction);

                #region sqlText
                sqlText = "  ";
                sqlText = "";

                sqlText = @"
INSERT INTO AttendanceDailyNew(
 AttendanceMigrationId,AttendanceStructureId,GroupId,EmployeeId,ProxyID,DailyDate,PunchInTime,PunchOutTime,PunchOutTimeNextday,PunchNextDay,IsManual,InTime,InTimeBy,OutTime,OutTimeBy
,IsDeductEarlyOut,EarlyOutMin,IsDeductLateIn,LateInMin,LunchBreak,WorkingHrs,WorkingHrsBy,TotalOT,TotalOTBy,AttnStatus,DayStatus,EarlyDeduct,LateDeduct
,TiffinAllow,DinnerAllow,IfterAllow,TiffinAmnt,IfterAmnt,DinnerAmnt,DeductTiffTime,DeductIfterTime,DeductDinTime,GrossAmnt,BasicAmnt,Remarks
) 
select 
Id AttendanceMigrationId
,IsNull(AttendanceStructureId, 0) AttendanceStructureId
,GroupId
,EmployeeId
,ProxyID
,DailyDate
,PunchInTime
,PunchOutTime
,PunchOutTimeNextday
,PunchNextDay
,IsManual
,'0000' InTime
,'0000' InTimeBy
,'0000' OutTime
,'0000' OutTimeBy
,0 IsDeductEarlyOut
,0 EarlyOutMin
,0 IsDeductLateIn
,0 LateInMin
,0 LunchBreak
,0 WorkingHrs
,0 WorkingHrsBy
,0 TotalOT
,0 TotalOTBy
,'Absent' AttnStatus
,@DayStatus
,0 EarlyDeduct
,0 LateDeduct
,0 TiffinAllow
,0 DinnerAllow
,0 IfterAllow
,0 TiffinAmnt
,0 IfterAmnt
,0 DinnerAmnt
,0 DeductTiffTime
,0 DeductIfterTime
,0 DeductDinTime
,0 GrossAmnt
,0 BasicAmnt
,'-'Remarks
 from AttendanceMigration
where EmployeeId not in(select EmployeeId from AttendanceDailyNew
where DailyDate=@DailyDate)
and DailyDate=@DailyDate

";

                SqlCommand cmdInsertAttendanceDaily = new SqlCommand(sqlText, currConn, transaction);
                cmdInsertAttendanceDaily.Parameters.AddWithValue("@DailyDate", Ordinary.DateToString(AttendanceDate));
                cmdInsertAttendanceDaily.Parameters.AddWithValue("@DayStatus", DayStatus);
                int i = cmdInsertAttendanceDaily.ExecuteNonQuery();

                //transResult = Convert.ToInt32(exe);
                //if (transResult <= 0)
                //{
                //    retResults[1] = "Unexpected Error to Migration!";
                //    throw new ArgumentNullException(retResults[1], "");
                //}
                #endregion sqlExecution

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
                retResults[3] = sqlText;
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
        #endregion New Methods
    }
}
