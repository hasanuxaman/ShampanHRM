using SymOrdinary;
using SymServices.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Attendance
{
    public class AttendanceDailyDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion
        #region Methods
        //==================SelectAll=================
        public List<AttendanceDailyVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<AttendanceDailyVM> VMs = new List<AttendanceDailyVM>();
            AttendanceDailyVM vm;
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
,BranchId
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
,InTime
,InTimeBy
,OutTime
,OutTimeBy
,LateMin
,WorkingHrs
,WorkingHrsBy
,WorkingHrsRest
,TotalOT
,TotalOTBy
,ExtraOT
,OTRest
,BonusMinit
,LunchOutDeduct
,AttnStatus
,DayStatus
,EarlyDeduct
,LateDeduct
,TiffinAllow
,DinnerAllow
,IfterAllow
,GrossAmnt
,BasicAmnt
,TiffinAmnt
,IfterAmnt
,DinnerAmnt
,Remarks
    From AttendanceDaily

";
//Where IsArchive=0



                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AttendanceDailyVM();

                    vm.Id = dr["Id"].ToString();

                    vm.Remarks = dr["Remarks"].ToString();


                    vm.AttendanceStructureId = Convert.ToInt32(dr["AttendanceStructureId"].ToString());
                    vm.GroupId = dr["GroupId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ProxyID = dr["ProxyID"].ToString();
                    vm.DailyDate = Ordinary.StringToDate(dr["DailyDate"].ToString());
                    vm.PunchInTime = Ordinary.StringToTime(dr["PunchInTime"].ToString());
                    vm.PunchOutTime = Ordinary.StringToTime(dr["PunchOutTime"].ToString());
                    vm.PunchOutTimeNextday = Ordinary.StringToTime(dr["PunchOutTimeNextday"].ToString());
                    vm.PunchNextDay = Convert.ToBoolean(dr["PunchNextDay"].ToString());
                    vm.IsManual = Convert.ToBoolean(dr["IsManual"].ToString());
                    vm.InTime = Ordinary.StringToTime(dr["InTime"].ToString());
                    vm.InTimeBy = dr["InTimeBy"].ToString();
                    vm.OutTime = Ordinary.StringToTime(dr["OutTime"].ToString());
                    vm.OutTimeBy = dr["OutTimeBy"].ToString();
                    vm.LateMin = Convert.ToInt32(dr["LateMin"].ToString());
                    vm.WorkingHrs = dr["WorkingHrs"].ToString();
                    vm.WorkingHrsBy = dr["WorkingHrsBy"].ToString();
                    vm.WorkingHrsRest = dr["WorkingHrsRest"].ToString();
                    vm.TotalOT = Convert.ToDecimal(dr["TotalOT"].ToString());
                    vm.TotalOTBy = Convert.ToDecimal(dr["TotalOTBy"].ToString());
                    vm.ExtraOT = Convert.ToDecimal(dr["ExtraOT"].ToString());
                    vm.OTRest = Convert.ToDecimal(dr["OTRest"].ToString());
                    vm.BonusMinit = Convert.ToDecimal(dr["BonusMinit"].ToString());
                    vm.LunchOutDeduct = Convert.ToDecimal(dr["LunchOutDeduct"].ToString());
                    vm.AttnStatus = dr["AttnStatus"].ToString();
                    vm.DayStatus = dr["DayStatus"].ToString();
                    vm.EarlyDeduct = Convert.ToDecimal(dr["EarlyDeduct"].ToString());
                    vm.LateDeduct = Convert.ToDecimal(dr["LateDeduct"].ToString());
                    vm.TiffinAllow = Convert.ToBoolean(dr["TiffinAllow"].ToString());
                    vm.DinnerAllow = Convert.ToBoolean(dr["DinnerAllow"].ToString());
                    vm.IfterAllow = Convert.ToBoolean(dr["IfterAllow"].ToString());
                    vm.GrossAmnt = Convert.ToDecimal(dr["GrossAmnt"].ToString());
                    vm.BasicAmnt = Convert.ToDecimal(dr["BasicAmnt"].ToString());
                    vm.TiffinAmnt = Convert.ToDecimal(dr["TiffinAmnt"].ToString());
                    vm.IfterAmnt = Convert.ToDecimal(dr["IfterAmnt"].ToString());
                    vm.DinnerAmnt = Convert.ToDecimal(dr["DinnerAmnt"].ToString());
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
        //==================SelectByID=================
        public AttendanceDailyVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            AttendanceDailyVM vm = new AttendanceDailyVM();

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
,BranchId
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
,InTime
,InTimeBy
,OutTime
,OutTimeBy
,LateMin
,WorkingHrs
,WorkingHrsBy
,WorkingHrsRest
,TotalOT
,TotalOTBy
,ExtraOT
,OTRest
,BonusMinit
,LunchOutDeduct
,AttnStatus
,DayStatus
,EarlyDeduct
,LateDeduct
,TiffinAllow
,DinnerAllow
,IfterAllow
,GrossAmnt
,BasicAmnt
,TiffinAmnt
,IfterAmnt
,DinnerAmnt
,Remarks
    From AttendanceDaily
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
                    vm.Id = dr["BranchId"].ToString();

                    vm.Remarks = dr["Remarks"].ToString();
                   
                    vm.AttendanceStructureId = Convert.ToInt32(dr["AttendanceStructureId"].ToString());
                    vm.GroupId = dr["GroupId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ProxyID = dr["ProxyID"].ToString();
                    vm.DailyDate = Ordinary.StringToDate(dr["DailyDate"].ToString());
                    vm.PunchInTime = Ordinary.StringToTime(dr["PunchInTime"].ToString());
                    vm.PunchOutTime = Ordinary.StringToTime(dr["PunchOutTime"].ToString());
                    vm.PunchOutTimeNextday = Ordinary.StringToTime(dr["PunchOutTimeNextday"].ToString());
                    vm.PunchNextDay = Convert.ToBoolean(dr["PunchNextDay"].ToString());
                    vm.IsManual = Convert.ToBoolean(dr["IsManual"].ToString());
                    vm.InTime = Ordinary.StringToTime(dr["InTime"].ToString());
                    vm.InTimeBy = dr["InTimeBy"].ToString();
                    vm.OutTime = Ordinary.StringToTime(dr["OutTime"].ToString());
                    vm.OutTimeBy = dr["OutTimeBy"].ToString();
                    vm.LateMin = Convert.ToInt32(dr["LateMin"].ToString());
                    vm.WorkingHrs = dr["WorkingHrs"].ToString();
                    vm.WorkingHrsBy = dr["WorkingHrsBy"].ToString();
                    vm.WorkingHrsRest = dr["WorkingHrsRest"].ToString();
                    vm.TotalOT = Convert.ToDecimal(dr["TotalOT"].ToString());
                    vm.TotalOTBy = Convert.ToDecimal(dr["TotalOTBy"].ToString());
                    vm.ExtraOT = Convert.ToDecimal(dr["ExtraOT"].ToString());
                    vm.OTRest = Convert.ToDecimal(dr["OTRest"].ToString());
                    vm.BonusMinit = Convert.ToDecimal(dr["BonusMinit"].ToString());
                    vm.LunchOutDeduct = Convert.ToDecimal(dr["LunchOutDeduct"].ToString());
                    vm.AttnStatus = dr["AttnStatus"].ToString();
                    vm.DayStatus = dr["DayStatus"].ToString();
                    vm.EarlyDeduct = Convert.ToDecimal(dr["EarlyDeduct"].ToString());
                    vm.LateDeduct = Convert.ToDecimal(dr["LateDeduct"].ToString());
                    vm.TiffinAllow = Convert.ToBoolean(dr["TiffinAllow"].ToString());
                    vm.DinnerAllow = Convert.ToBoolean(dr["DinnerAllow"].ToString());
                    vm.IfterAllow = Convert.ToBoolean(dr["IfterAllow"].ToString());
                    vm.GrossAmnt = Convert.ToDecimal(dr["GrossAmnt"].ToString());
                    vm.BasicAmnt = Convert.ToDecimal(dr["BasicAmnt"].ToString());
                    vm.TiffinAmnt = Convert.ToDecimal(dr["TiffinAmnt"].ToString());
                    vm.IfterAmnt = Convert.ToDecimal(dr["IfterAmnt"].ToString());
                    vm.DinnerAmnt = Convert.ToDecimal(dr["DinnerAmnt"].ToString());
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



        //==================Insert =================
        public string[] Insert(AttendanceDailyVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                //if (string.IsNullOrEmpty(AttendanceDailyVM.DownloadDataId))
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
                #region Exist
                sqlText = @"Select isnull( isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) ,0)
                from AttendanceDaily where BranchId=@BranchId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                var exeRes = cmdExist.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);
                //if (count > 0)
                //{
                //    retResults[1] = "Already this Attendance Daily Id is Used!";
                //    throw new ArgumentNullException("Already this  Attendance Daily Id is Used!", "");
                //}
                #endregion Exist

                #region Save
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO AttendanceDaily(

Id
,BranchId
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
,InTime
,InTimeBy
,OutTime
,OutTimeBy
,LateMin
,WorkingHrs
,WorkingHrsBy
,WorkingHrsRest
,TotalOT
,TotalOTBy
,ExtraOT
,OTRest
,BonusMinit
,LunchOutDeduct
,AttnStatus
,DayStatus
,EarlyDeduct
,LateDeduct
,TiffinAllow
,DinnerAllow
,IfterAllow
,GrossAmnt
,BasicAmnt
,TiffinAmnt
,IfterAmnt
,DinnerAmnt

) 
                                VALUES (
@Id
,@BranchId
,@AttendanceStructureId
,@GroupId
,@EmployeeId
,@ProxyID
,@DailyDate
,@PunchInTime
,@PunchOutTime
,@PunchOutTimeNextday
,@PunchNextDay
,@IsManual
,@InTime
,@InTimeBy
,@OutTime
,@OutTimeBy
,@LateMin
,@WorkingHrs
,@WorkingHrsBy
,@WorkingHrsRest
,@TotalOT
,@TotalOTBy
,@ExtraOT
,@OTRest
,@BonusMinit
,@LunchOutDeduct
,@AttnStatus
,@DayStatus
,@EarlyDeduct
,@LateDeduct
,@TiffinAllow
,@DinnerAllow
,@IfterAllow
,@GrossAmnt
,@BasicAmnt
,@TiffinAmnt
,@IfterAmnt
,@DinnerAmnt
)                                        ";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                   
                    cmdInsert.Parameters.AddWithValue("@AttendanceStructureId", vm.AttendanceStructureId);
                    cmdInsert.Parameters.AddWithValue("@GroupId", vm.GroupId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ProxyID", vm.ProxyID ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@DailyDate", Ordinary.DateToString(vm.DailyDate));
                    cmdInsert.Parameters.AddWithValue("@PunchInTime", Ordinary.TimeToString(vm.PunchInTime));
                    cmdInsert.Parameters.AddWithValue("@PunchOutTime", Ordinary.TimeToString(vm.PunchOutTime));
                    cmdInsert.Parameters.AddWithValue("@PunchOutTimeNextday", Ordinary.TimeToString(vm.PunchOutTimeNextday));
                    cmdInsert.Parameters.AddWithValue("@PunchNextDay", vm.PunchNextDay);
                    cmdInsert.Parameters.AddWithValue("@IsManual", vm.IsManual);
                    cmdInsert.Parameters.AddWithValue("@InTime", Ordinary.TimeToString(vm.InTime));
                    cmdInsert.Parameters.AddWithValue("@InTimeBy", vm.InTimeBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@OutTime", Ordinary.TimeToString(vm.OutTime));
                    cmdInsert.Parameters.AddWithValue("@OutTimeBy", vm.OutTimeBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@LateMin", vm.LateMin);
                    cmdInsert.Parameters.AddWithValue("@WorkingHrs", vm.WorkingHrs);
                    cmdInsert.Parameters.AddWithValue("@WorkingHrsBy", vm.WorkingHrsBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@WorkingHrsRest", vm.WorkingHrsRest);
                    cmdInsert.Parameters.AddWithValue("@TotalOT", vm.TotalOT);
                    cmdInsert.Parameters.AddWithValue("@TotalOTBy", vm.TotalOTBy);
                    cmdInsert.Parameters.AddWithValue("@ExtraOT", vm.ExtraOT);
                    cmdInsert.Parameters.AddWithValue("@OTRest", vm.OTRest);
                    cmdInsert.Parameters.AddWithValue("@BonusMinit", vm.BonusMinit);
                    cmdInsert.Parameters.AddWithValue("@LunchOutDeduct", vm.LunchOutDeduct);
                    cmdInsert.Parameters.AddWithValue("@AttnStatus", vm.AttnStatus ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@DayStatus", vm.DayStatus ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@EarlyDeduct", vm.EarlyDeduct);
                    cmdInsert.Parameters.AddWithValue("@LateDeduct", vm.LateDeduct);
                    cmdInsert.Parameters.AddWithValue("@TiffinAllow", vm.TiffinAllow);
                    cmdInsert.Parameters.AddWithValue("@DinnerAllow", vm.DinnerAllow);
                    cmdInsert.Parameters.AddWithValue("@IfterAllow", vm.IfterAllow);
                    cmdInsert.Parameters.AddWithValue("@GrossAmnt", vm.GrossAmnt);
                    cmdInsert.Parameters.AddWithValue("@BasicAmnt", vm.BasicAmnt);
                    cmdInsert.Parameters.AddWithValue("@TiffinAmnt", vm.TiffinAmnt);
                    cmdInsert.Parameters.AddWithValue("@IfterAmnt", vm.IfterAmnt);
                    cmdInsert.Parameters.AddWithValue("@DinnerAmnt", vm.DinnerAmnt);

                    //cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, AttendanceDailyVM.Remarks);
                    //cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    //cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    //cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    //cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    //cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This Attendance Daily already used!";
                    throw new ArgumentNullException("Please Input Attendance Daily Value", "");
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

        public string[] Insert(List<AttendanceDailyVM> vms, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                //if (string.IsNullOrEmpty(AttendanceDailyVM.DownloadDataId))
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
                sqlText = "  ";
                sqlText += @" 
INSERT INTO AttendanceDaily(
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
,InTime
,InTimeBy
,OutTime
,OutTimeBy
,LateMin
,WorkingHrs
,WorkingHrsBy
,WorkingHrsRest
,TotalOT
,TotalOTBy
,ExtraOT
,OTRest
,BonusMinit
,LunchOutDeduct
,AttnStatus
,DayStatus
,EarlyDeduct
,LateDeduct
,TiffinAllow
,DinnerAllow
,IfterAllow
,GrossAmnt
,BasicAmnt
,TiffinAmnt
,IfterAmnt
,DinnerAmnt
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
,@InTime
,@InTimeBy
,@OutTime
,@OutTimeBy
,@LateMin
,@WorkingHrs
,@WorkingHrsBy
,@WorkingHrsRest
,@TotalOT
,@TotalOTBy
,@ExtraOT
,@OTRest
,@BonusMinit
,@LunchOutDeduct
,@AttnStatus
,@DayStatus
,@EarlyDeduct
,@LateDeduct
,@TiffinAllow
,@DinnerAllow
,@IfterAllow
,@GrossAmnt
,@BasicAmnt
,@TiffinAmnt
,@IfterAmnt
,@DinnerAmnt
) ";
                #endregion Save

                foreach (AttendanceDailyVM vm in vms)
                {
                    if (vm != null)
                    {
                        #region 
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        //cmdInsert.Parameters.AddWithValue("@Id", vm.Id);

                        cmdInsert.Parameters.AddWithValue("@AttendanceStructureId", vm.AttendanceStructureId);
                        cmdInsert.Parameters.AddWithValue("@GroupId", vm.GroupId ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@ProxyID", vm.ProxyID ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@DailyDate", vm.DailyDate);
                        cmdInsert.Parameters.AddWithValue("@PunchInTime",vm.PunchInTime);
                        cmdInsert.Parameters.AddWithValue("@PunchOutTime", vm.PunchOutTime);
                        cmdInsert.Parameters.AddWithValue("@PunchOutTimeNextday", vm.PunchOutTimeNextday);
                        cmdInsert.Parameters.AddWithValue("@PunchNextDay", vm.PunchNextDay);
                        cmdInsert.Parameters.AddWithValue("@IsManual", vm.IsManual);
                        cmdInsert.Parameters.AddWithValue("@InTime", vm.InTime);
                        cmdInsert.Parameters.AddWithValue("@InTimeBy", vm.InTimeBy ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@OutTime", (vm.OutTime));
                        cmdInsert.Parameters.AddWithValue("@OutTimeBy", vm.OutTimeBy ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@LateMin", vm.LateMin);
                        cmdInsert.Parameters.AddWithValue("@WorkingHrs", vm.WorkingHrs);
                        cmdInsert.Parameters.AddWithValue("@WorkingHrsBy", vm.WorkingHrsBy ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@WorkingHrsRest", vm.WorkingHrsRest);
                        cmdInsert.Parameters.AddWithValue("@TotalOT", vm.TotalOT);
                        cmdInsert.Parameters.AddWithValue("@TotalOTBy", vm.TotalOTBy);
                        cmdInsert.Parameters.AddWithValue("@ExtraOT", vm.ExtraOT);
                        cmdInsert.Parameters.AddWithValue("@OTRest", vm.OTRest);
                        cmdInsert.Parameters.AddWithValue("@BonusMinit", vm.BonusMinit);
                        cmdInsert.Parameters.AddWithValue("@LunchOutDeduct", vm.LunchOutDeduct);
                        cmdInsert.Parameters.AddWithValue("@AttnStatus", vm.AttnStatus ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@DayStatus", vm.DayStatus ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@EarlyDeduct", vm.EarlyDeduct);
                        cmdInsert.Parameters.AddWithValue("@LateDeduct", vm.LateDeduct);
                        cmdInsert.Parameters.AddWithValue("@TiffinAllow", vm.TiffinAllow);
                        cmdInsert.Parameters.AddWithValue("@DinnerAllow", vm.DinnerAllow);
                        cmdInsert.Parameters.AddWithValue("@IfterAllow", vm.IfterAllow);
                        cmdInsert.Parameters.AddWithValue("@GrossAmnt", vm.GrossAmnt);
                        cmdInsert.Parameters.AddWithValue("@BasicAmnt", vm.BasicAmnt);
                        cmdInsert.Parameters.AddWithValue("@TiffinAmnt", vm.TiffinAmnt);
                        cmdInsert.Parameters.AddWithValue("@IfterAmnt", vm.IfterAmnt);
                        cmdInsert.Parameters.AddWithValue("@DinnerAmnt", vm.DinnerAmnt);
                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);

                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                        #endregion
                    }
                    else
                    {
                        retResults[1] = "This Attendance Daily already used!";
                        throw new ArgumentNullException("Please Input Attendance Daily Value", "");
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

        //==================Update =================
        public string[] Update(AttendanceDailyVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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

                //#region Exist

                //CommonDAL cdal = new CommonDAL();
                //bool check = false;
                //string tableName = "AttendanceDaily";
                //string[] fieldName = { "Name" };
                //string[] fieldValue = { vm.Name.Trim() };

                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInUpdate(vm.Id.ToString(), tableName, fieldName[i], fieldValue[i], currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                //#endregion Exist


                #region Exist
                sqlText = @"Select isnull( isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) ,0)
                from AttendanceDaily where BranchId=@BranchId and Id<>@Id";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                cmdExist.Transaction = transaction;
                var exeRes = cmdExist.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);
                //if (count > 0)
                //{
                //    retResults[1] = "Already this Attendance Daily Id is Used!";
                //    throw new ArgumentNullException("Already this  Attendance Daily Id is Used!", "");
                //}
                #endregion Exist

                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update AttendanceDaily set";

                    sqlText += " BranchId=@BranchId";
                    sqlText += " ,AttendanceStructureId=@AttendanceStructureId";
                    sqlText += " ,GroupId=@GroupId";
                    sqlText += " ,EmployeeId=@EmployeeId";
                    sqlText += " ,ProxyID=@ProxyID";
                    sqlText += " ,DailyDate=@DailyDate";
                    sqlText += " ,PunchInTime=@PunchInTime";
                    sqlText += " ,PunchOutTime=@PunchOutTime";
                    sqlText += " ,PunchOutTimeNextday=@PunchOutTimeNextday";
                    sqlText += " ,PunchNextDay=@PunchNextDay";
                    sqlText += " ,IsManual=@IsManual";
                    sqlText += " ,InTime=@InTime";
                    sqlText += " ,InTimeBy=@InTimeBy";
                    sqlText += " ,OutTime=@OutTime";
                    sqlText += " ,OutTimeBy=@OutTimeBy";
                    sqlText += " ,LateMin=@LateMin";
                    sqlText += " ,WorkingHrs=@WorkingHrs";
                    sqlText += " ,WorkingHrsBy=@WorkingHrsBy";
                    sqlText += " ,WorkingHrsRest=@WorkingHrsRest";
                    sqlText += " ,TotalOT=@TotalOT";
                    sqlText += " ,TotalOTBy=@TotalOTBy";
                    sqlText += " ,ExtraOT=@ExtraOT";
                    sqlText += " ,OTRest=@OTRest";
                    sqlText += " ,BonusMinit=@BonusMinit";
                    sqlText += " ,LunchOutDeduct=@LunchOutDeduct";
                    sqlText += " ,AttnStatus=@AttnStatus";
                    sqlText += " ,DayStatus=@DayStatus";
                    sqlText += " ,EarlyDeduct=@EarlyDeduct";
                    sqlText += " ,LateDeduct=@LateDeduct";
                    sqlText += " ,TiffinAllow=@TiffinAllow";
                    sqlText += " ,DinnerAllow=@DinnerAllow";
                    sqlText += " ,IfterAllow=@IfterAllow";
                    sqlText += " ,GrossAmnt=@GrossAmnt";
                    sqlText += " ,BasicAmnt=@BasicAmnt";
                    sqlText += " ,TiffinAmnt=@TiffinAmnt";
                    sqlText += " ,IfterAmnt=@IfterAmnt";
                    sqlText += " ,DinnerAmnt=@DinnerAmnt";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, AttendanceDailyVM.Remarks);
                   


                    cmdUpdate.Parameters.AddWithValue("@AttendanceStructureId", vm.AttendanceStructureId);
                    cmdUpdate.Parameters.AddWithValue("@GroupId", vm.GroupId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ProxyID", vm.ProxyID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DailyDate",Ordinary.DateToString( vm.DailyDate));
                    cmdUpdate.Parameters.AddWithValue("@PunchInTime", Ordinary.TimeToString(vm.PunchInTime));
                    cmdUpdate.Parameters.AddWithValue("@PunchOutTime", Ordinary.TimeToString(vm.PunchOutTime));
                    cmdUpdate.Parameters.AddWithValue("@PunchOutTimeNextday", Ordinary.TimeToString(vm.PunchOutTimeNextday));
                    cmdUpdate.Parameters.AddWithValue("@PunchNextDay", vm.PunchNextDay);
                    cmdUpdate.Parameters.AddWithValue("@IsManual", vm.IsManual);
                    cmdUpdate.Parameters.AddWithValue("@InTime", Ordinary.TimeToString(vm.InTime));
                    cmdUpdate.Parameters.AddWithValue("@InTimeBy", vm.InTimeBy ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@OutTime", Ordinary.TimeToString(vm.OutTime));
                    cmdUpdate.Parameters.AddWithValue("@OutTimeBy", vm.OutTimeBy ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LateMin", vm.LateMin);
                    cmdUpdate.Parameters.AddWithValue("@WorkingHrs", vm.WorkingHrs);
                    cmdUpdate.Parameters.AddWithValue("@WorkingHrsBy", vm.WorkingHrsBy);
                    cmdUpdate.Parameters.AddWithValue("@WorkingHrsRest", vm.WorkingHrsRest);
                    cmdUpdate.Parameters.AddWithValue("@TotalOT", vm.TotalOT);
                    cmdUpdate.Parameters.AddWithValue("@TotalOTBy", vm.TotalOTBy);
                    cmdUpdate.Parameters.AddWithValue("@ExtraOT", vm.ExtraOT);
                    cmdUpdate.Parameters.AddWithValue("@OTRest", vm.OTRest);
                    cmdUpdate.Parameters.AddWithValue("@BonusMinit", vm.BonusMinit);
                    cmdUpdate.Parameters.AddWithValue("@LunchOutDeduct", vm.LunchOutDeduct);
                    cmdUpdate.Parameters.AddWithValue("@AttnStatus", vm.AttnStatus ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DayStatus", vm.DayStatus ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EarlyDeduct", vm.EarlyDeduct);
                    cmdUpdate.Parameters.AddWithValue("@LateDeduct", vm.LateDeduct);
                    cmdUpdate.Parameters.AddWithValue("@TiffinAllow", vm.TiffinAllow);
                    cmdUpdate.Parameters.AddWithValue("@DinnerAllow", vm.DinnerAllow);
                    cmdUpdate.Parameters.AddWithValue("@IfterAllow", vm.IfterAllow);
                    cmdUpdate.Parameters.AddWithValue("@GrossAmnt ", vm.GrossAmnt);
                    cmdUpdate.Parameters.AddWithValue("@BasicAmnt ", vm.BasicAmnt);
                    cmdUpdate.Parameters.AddWithValue("@TiffinAmnt", vm.TiffinAmnt);
                    cmdUpdate.Parameters.AddWithValue("@IfterAmnt ", vm.IfterAmnt);
                    cmdUpdate.Parameters.AddWithValue("@DinnerAmnt", vm.DinnerAmnt);
                    
                    cmdUpdate.Transaction = transaction;                   
                    transResult = (int)cmdUpdate.ExecuteNonQuery();

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", AttendanceDailyVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    retResults[1] = "This Attendance Daily already used!";
                    throw new ArgumentNullException("Please Input Attendance Daily Value", "");
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
                    retResults[1] = "Unexpected error to update Attendance Daily.";
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
        public string[] Delete(AttendanceDailyVM vm, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete"; //Method Name

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
                        sqlText = "update AttendanceDaily set";
                        //sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        //cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                        transResult = (int)cmdUpdate.ExecuteNonQuery();
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Attendance Daily Delete", vm.Id + " could not Delete.");
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
                    retResults[1] = "Unexpected error to delete Attendance Daily Information.";
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
        //==================DropDown =================
        public List<AttendanceDailyVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<AttendanceDailyVM> VMs = new List<AttendanceDailyVM>();
            AttendanceDailyVM vm;
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
   FROM AttendanceDaily
WHERE IsArchive=0 and IsActive=1
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AttendanceDailyVM();
                    vm.Id = dr["Id"].ToString();
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
            //==================Delete =================
        }

        //==================SelectAll=================
        public List<AttendanceDailyVM> SelectAllForReport(string MulitpleSelection=null,string Filter = null, string AttnStatus = null, string Summary=null, string AttendanceDate=null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AttendanceDailyVM> VMs = new List<AttendanceDailyVM>();
            AttendanceDailyVM vm;
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
,BranchId
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
,InTime
,InTimeBy
,OutTime
,OutTimeBy
,LateMin
,WorkingHrs
,WorkingHrsBy
,WorkingHrsRest
,TotalOT
,TotalOTBy
,ExtraOT
,OTRest
,BonusMinit
,LunchOutDeduct
,AttnStatus
,DayStatus
,EarlyDeduct
,LateDeduct
,TiffinAllow
,DinnerAllow
,IfterAllow
,GrossAmnt
,BasicAmnt
,TiffinAmnt
,IfterAmnt
,DinnerAmnt
,Remarks

,Code
,EmpName
,JoinDate
,EmploymentStatus
,EmploymentType
,Project
,Branch
,Department
,Section
,Designation
,Grade
,ProjectId
,SectionId
,DepartmentId
,DesignationId
,GradeId
,BranchId
,Gender
,Religion
,GroupName
,AttnStractureName
    From ViewAttendanceDaily
where 1=1 
";
                #region MultipleSelection
                List<string> SelectionList = MulitpleSelection.Split(',').ToList();
                if (SelectionList.Count > 0)
                {
                    string FilterId = "";
                    int i = 0;
                    if (Filter.ToLower() == "department")
                        FilterId = "DepartmentId";
                    else if (Filter.ToLower() == "section")
                        FilterId = "SectionId";
                    else if (Filter.ToLower() == "project")
                        FilterId = "ProjectId";
                    else if (Filter.ToLower() == "designation")
                        FilterId = "DesignationId";
                    else if (Filter.ToLower() == "attendancepolicy")
                        FilterId = "AttendanceStructureId";
                    else if (Filter.ToLower() == "group")
                        FilterId = "GroupId";

                    if (!string.IsNullOrWhiteSpace(FilterId))
                    {
                        sqlText += " AND (";
                        foreach (string item in SelectionList)
                        {
                            sqlText += " " + FilterId + "='" + item + "'";
                            if (i < SelectionList.Count - 1)
                            {
                                sqlText += " or ";
                            }
                            i++;
                        }
                        sqlText += ") ";
                    }
                }
                #endregion MultipleSelection

                if (!string.IsNullOrWhiteSpace(AttnStatus))
                    sqlText += " and AttnStatus=@AttnStatus";
                if (!string.IsNullOrWhiteSpace(AttendanceDate))
                    sqlText += " and DailyDate=@AttendanceDate";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (!string.IsNullOrWhiteSpace(AttnStatus))
                    objComm.Parameters.AddWithValue("@AttnStatus", AttnStatus);
                if (!string.IsNullOrWhiteSpace(AttendanceDate))
                    objComm.Parameters.AddWithValue("@AttendanceDate", AttendanceDate);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AttendanceDailyVM();

                    vm.Id = dr["Id"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.AttendanceStructureId = Convert.ToInt32(dr["AttendanceStructureId"].ToString());
                    vm.GroupId = dr["GroupId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ProxyID = dr["ProxyID"].ToString();
                    vm.DailyDate = Ordinary.StringToDate(dr["DailyDate"].ToString());
                    vm.PunchInTime = Ordinary.StringToTime(dr["PunchInTime"].ToString());
                    vm.PunchOutTime = Ordinary.StringToTime(dr["PunchOutTime"].ToString());
                    vm.PunchOutTimeNextday = Ordinary.StringToTime(dr["PunchOutTimeNextday"].ToString());
                    vm.PunchNextDay = Convert.ToBoolean(dr["PunchNextDay"].ToString());
                    vm.IsManual = Convert.ToBoolean(dr["IsManual"].ToString());
                    vm.InTime = Ordinary.StringToTime(dr["InTime"].ToString());
                    vm.InTimeBy = dr["InTimeBy"].ToString();
                    vm.OutTime = Ordinary.StringToTime(dr["OutTime"].ToString());
                    vm.OutTimeBy = dr["OutTimeBy"].ToString();
                    vm.LateMin = Convert.ToInt32(dr["LateMin"].ToString());
                    vm.WorkingHrs = dr["WorkingHrs"].ToString();
                    vm.WorkingHrsBy = dr["WorkingHrsBy"].ToString();
                    vm.WorkingHrsRest = dr["WorkingHrsRest"].ToString();
                    vm.TotalOT = Convert.ToDecimal(dr["TotalOT"].ToString());
                    vm.TotalOTBy = Convert.ToDecimal(dr["TotalOTBy"].ToString());
                    vm.ExtraOT = Convert.ToDecimal(dr["ExtraOT"].ToString());
                    vm.OTRest = Convert.ToDecimal(dr["OTRest"].ToString());
                    vm.BonusMinit = Convert.ToDecimal(dr["BonusMinit"].ToString());
                    vm.LunchOutDeduct = Convert.ToDecimal(dr["LunchOutDeduct"].ToString());
                    vm.AttnStatus = dr["AttnStatus"].ToString();
                    vm.DayStatus = dr["DayStatus"].ToString();
                    vm.EarlyDeduct = Convert.ToDecimal(dr["EarlyDeduct"].ToString());
                    vm.LateDeduct = Convert.ToDecimal(dr["LateDeduct"].ToString());
                    vm.TiffinAllow = Convert.ToBoolean(dr["TiffinAllow"].ToString());
                    vm.DinnerAllow = Convert.ToBoolean(dr["DinnerAllow"].ToString());
                    vm.IfterAllow = Convert.ToBoolean(dr["IfterAllow"].ToString());
                    vm.GrossAmnt = Convert.ToDecimal(dr["GrossAmnt"].ToString());
                    vm.BasicAmnt = Convert.ToDecimal(dr["BasicAmnt"].ToString());
                    vm.TiffinAmnt = Convert.ToDecimal(dr["TiffinAmnt"].ToString());
                    vm.IfterAmnt = Convert.ToDecimal(dr["IfterAmnt"].ToString());
                    vm.DinnerAmnt = Convert.ToDecimal(dr["DinnerAmnt"].ToString());
                    
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.JoinDate = dr["JoinDate"].ToString();
                    vm.EmploymentStatus = dr["EmploymentStatus"].ToString();
                    vm.EmploymentType = dr["EmploymentType"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Branch = dr["Branch"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.GradeId = dr["GradeId"].ToString();
                    //vm.BranchId = dr["BranchId"].ToString();
                    vm.Gender = dr["Gender"].ToString();
                    vm.Religion = dr["Religion"].ToString();
                    vm.GroupName = dr["GroupName"].ToString();
                    vm.AttnStractureName = dr["AttnStractureName"].ToString();

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


        #endregion Methods
    }
}
