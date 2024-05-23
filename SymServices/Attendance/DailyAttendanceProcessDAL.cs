using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymServices.Attendance
{
    public class DailyAttendanceProcessDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        #region Methods
        ////==================SelectAll=================
        public List<AttendanceDailyNewVM> SelectAll(int Id = 0, AttendanceDailyNewVM vVM = null, string[] conFields = null, string[] conValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null; SqlTransaction transaction = null;
            string sqlText = "";
            AttendanceDailyNewVM vm = new AttendanceDailyNewVM();
            List<AttendanceDailyNewVM> VMs = new List<AttendanceDailyNewVM>();

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
                sqlText = @"
SELECT 
attn.Id
------From Other Table------
,ve.Code
,ve.EmpName
,ve.Department
,ve.Section
,ve.Project
,ve.Designation
,ve.JoinDate
,ast.Name PolicyName
,ast.InTime PInTime
,ast.OutTime POutTime
,gr.Name GroupName
------This Table------
,ISNULL(attn.EmployeeId                   ,'0')    EmployeeId           
,ISNULL(attn.ProxyID                      ,'0')    ProxyID              
,ISNULL(attn.AttendanceMigrationId        ,'0')    AttendanceMigrationId
,ISNULL(attn.AttendanceStructureId        ,'0')    AttendanceStructureId
,ISNULL(attn.GroupId                      ,'0')    GroupId              
,ISNULL(attn.DailyDate                    ,'0')    DailyDate            
,ISNULL(attn.PunchInTime                  ,'0')    PunchInTime          
,ISNULL(attn.PunchOutTime                 ,'0')    PunchOutTime         
,ISNULL(attn.PunchOutTimeNextday          ,'0')    PunchOutTimeNextday  
,ISNULL(attn.PunchNextDay                 ,'0')    PunchNextDay         
,ISNULL(attn.IsManual                     ,'0')    IsManual             
,ISNULL(attn.InTime                       ,'0')    InTime               
,ISNULL(attn.InTimeBy                     ,'0')    InTimeBy             
,ISNULL(attn.OutTime                      ,'0')    OutTime              
,ISNULL(attn.OutTimeBy                    ,'0')    OutTimeBy            
,ISNULL(attn.IsDeductEarlyOut             ,'0')    IsDeductEarlyOut     
,ISNULL(attn.EarlyOutMin                  ,'0')    EarlyOutMin          
,ISNULL(attn.IsDeductLateIn               ,'0')    IsDeductLateIn       
,ISNULL(attn.LateInMin                    ,'0')    LateInMin            
,ISNULL(attn.LunchBreak                   ,'0')    LunchBreak           
,ISNULL(attn.WorkingHrs                   ,'0')    WorkingHrs           
,ISNULL(attn.WorkingHrsBy                 ,'0')    WorkingHrsBy         
,ISNULL(attn.TotalOT                      ,'0')    TotalOT              
,ISNULL(attn.TotalOTBy                    ,'0')    TotalOTBy            
,ISNULL(attn.AttnStatus                   ,'0')    AttnStatus           
,ISNULL(attn.DayStatus                    ,'0')    DayStatus            
,ISNULL(attn.EarlyDeduct                  ,'0')    EarlyDeduct          
,ISNULL(attn.LateDeduct                   ,'0')    LateDeduct           
,ISNULL(attn.TiffinAllow                  ,'0')    TiffinAllow          
,ISNULL(attn.DinnerAllow                  ,'0')    DinnerAllow          
,ISNULL(attn.IfterAllow                   ,'0')    IfterAllow           
,ISNULL(attn.TiffinAmnt                   ,'0')    TiffinAmnt           
,ISNULL(attn.IfterAmnt                    ,'0')    IfterAmnt            
,ISNULL(attn.DinnerAmnt                   ,'0')    DinnerAmnt           
,ISNULL(attn.DeductTiffTime               ,'0')    DeductTiffTime       
,ISNULL(attn.DeductIfterTime              ,'0')    DeductIfterTime      
,ISNULL(attn.DeductDinTime                ,'0')    DeductDinTime        
,ISNULL(attn.GrossAmnt                    ,'0')    GrossAmnt            
,ISNULL(attn.BasicAmnt                    ,'0')    BasicAmnt            
,ISNULL(attn.Remarks                      ,'0')    Remarks              

From AttendanceDailyNew attn
LEFT OUTER JOIN ViewEmployeeInformation ve ON attn.EmployeeId = ve.EmployeeId 
LEFT OUTER JOIN AttendanceStructure ast ON attn.AttendanceStructureId = ast.Id
LEFT OUTER JOIN [Group] gr ON attn.GroupId = gr.Id
WHERE 1=1 
";

                #endregion sql statement
                //{ "All", "Present", "Absent", "Late", "All Missing", "In Miss", "Out Miss" };
                #region sql Execution
                #region ConditionCheck
                if (vVM.AttnStatus == "Present")
                {
                    sqlText += " AND (attn.InTime <> '0000' AND attn.OutTime <> '0000')";
                }
                else if (vVM.AttnStatus == "Absent")
                {
                    sqlText += " AND (attn.InTime = '0000' AND attn.OutTime = '0000')";
                }
                else if (vVM.AttnStatus == "Late")
                {
                    sqlText += " AND (attn.LateInMin > '0')";
                }
                else if (vVM.AttnStatus == "All Missing")
                {
                    sqlText += " AND ((attn.InTime = '0000' AND attn.OutTime <> '0000') OR (attn.InTime <> '0000' AND attn.OutTime = '0000'))";
                }
                else if (vVM.AttnStatus == "In Miss")
                {
                    sqlText += " AND (attn.InTime = '0000' AND attn.OutTime <> '0000')";
                }
                else if (vVM.AttnStatus == "Out Miss")
                {
                    sqlText += " AND (attn.InTime <> '0000' AND attn.OutTime = '0000')";
                }
                #endregion ConditionCheck
                string cField = "";
                if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                {
                    for (int i = 0; i < conFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[i]) || string.IsNullOrWhiteSpace(conValues[i]))
                        {
                            continue;
                        }
                        cField = conFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conFields[i] + "=@" + cField;
                    }
                }
                sqlText += " ORDER By attn.DailyDate, ve.Department, ve.Section, gr.Name, ve.Code";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                {
                    for (int j = 0; j < conFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[j]) || string.IsNullOrWhiteSpace(conValues[j]))
                        {
                            continue;
                        }
                        cField = conFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conValues[j]);
                    }
                }
                #endregion sql Execution
                #region DataReading
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AttendanceDailyNewVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.EmpCode = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());

                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ProxyID = dr["ProxyID"].ToString();
                    vm.AttendanceMigrationId = Convert.ToInt32(dr["AttendanceMigrationId"]);
                    vm.AttendanceStructureId = Convert.ToInt32(dr["AttendanceStructureId"]);
                    vm.GroupId = dr["GroupId"].ToString();
                    vm.DailyDate = Ordinary.StringToDate(dr["DailyDate"].ToString());
                    vm.PunchInTime = Ordinary.StringToTime(dr["PunchInTime"].ToString());
                    vm.PunchOutTime = Ordinary.StringToTime(dr["PunchOutTime"].ToString());
                    vm.PunchOutTimeNextday = Ordinary.StringToTime(dr["PunchOutTimeNextday"].ToString());
                    vm.PunchNextDay = Convert.ToBoolean(dr["PunchNextDay"]);
                    vm.IsManual = Convert.ToBoolean(dr["IsManual"]);
                    vm.InTime = Ordinary.StringToTime(dr["InTime"].ToString());
                    vm.InTimeBy = Ordinary.StringToTime(dr["InTimeBy"].ToString());
                    vm.OutTime = Ordinary.StringToTime(dr["OutTime"].ToString());
                    vm.OutTimeBy = Ordinary.StringToTime(dr["OutTimeBy"].ToString());
                    vm.IsDeductEarlyOut = Convert.ToBoolean(dr["IsDeductEarlyOut"]);
                    vm.EarlyOutMin = Convert.ToInt32(dr["EarlyOutMin"]);
                    vm.IsDeductLateIn = Convert.ToBoolean(dr["IsDeductLateIn"]);
                    vm.LateInMin = Convert.ToInt32(dr["LateInMin"]);
                    vm.LunchBreak = Convert.ToInt32(dr["LunchBreak"]);
                    vm.WorkingHrs = dr["WorkingHrs"].ToString();
                    vm.WorkingHrsBy = dr["WorkingHrsBy"].ToString();
                    vm.TotalOT = Convert.ToInt32(dr["TotalOT"]);
                    vm.TotalOTBy = Convert.ToInt32(dr["TotalOTBy"]);
                    vm.AttnStatus = dr["AttnStatus"].ToString();
                    vm.DayStatus = dr["DayStatus"].ToString();
                    vm.EarlyDeduct = Convert.ToDecimal(dr["EarlyDeduct"]);
                    vm.LateDeduct = Convert.ToDecimal(dr["LateDeduct"]);
                    vm.TiffinAllow = Convert.ToBoolean(dr["TiffinAllow"]);
                    vm.DinnerAllow = Convert.ToBoolean(dr["DinnerAllow"]);
                    vm.IfterAllow = Convert.ToBoolean(dr["IfterAllow"]);
                    vm.TiffinAmnt = Convert.ToDecimal(dr["TiffinAmnt"]);
                    vm.IfterAmnt = Convert.ToDecimal(dr["IfterAmnt"]);
                    vm.DinnerAmnt = Convert.ToDecimal(dr["DinnerAmnt"]);
                    vm.DeductTiffTime = Convert.ToInt32(dr["DeductTiffTime"]);
                    vm.DeductIfterTime = Convert.ToInt32(dr["DeductIfterTime"]);
                    vm.DeductDinTime = Convert.ToInt32(dr["DeductDinTime"]);
                    vm.GrossAmnt = Convert.ToDecimal(dr["GrossAmnt"]);
                    vm.BasicAmnt = Convert.ToDecimal(dr["BasicAmnt"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    VMs.Add(vm);

                }
                dr.Close();

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion DataReading
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
                if (VcurrConn != null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }

        //==================Update =================
        public string[] Update(List<AttendanceDailyNewVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "DailyAttendanceProcessDAL Update Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Monthly Absence"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region sqlText

                sqlText = "  ";
                sqlText += @" 
Update AttendanceDailyNew Set
InTime = @InTime
,OutTime = @OutTime
";
                #endregion sqlText

                if (VMs.Count >= 1)
                {
                    #region Update Settings
                    foreach (var item in VMs)
                    {
                        #region CheckPoint
                        if (!item.IsEmployeeChecked)
                        {
                            continue;
                        }

                        sqlText += @" WHERE 1=1 AND EmployeeId = @EmployeeId AND DailyDate = @DailyDate";

                        #endregion CheckPoint

                        item.Id = _cDal.NextId("AttendanceDailyNew", currConn, transaction);
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                        if (item.IsInTimeUpdate)
                        {
                            cmdUpdate.Parameters.AddWithValue("@InTime", Ordinary.TimeToString(item.UpdatedInTime));
                        }
                        else
                        {
                            cmdUpdate.Parameters.AddWithValue("@InTime", Ordinary.TimeToString(item.InTime));
                        }
                        if (item.IsOutTimeUpdate)
                        {
                            cmdUpdate.Parameters.AddWithValue("@OutTime", Ordinary.TimeToString(item.UpdatedOutTime));
                        }
                        else
                        {
                            cmdUpdate.Parameters.AddWithValue("@OutTime", Ordinary.TimeToString(item.OutTime));

                        }
                        cmdUpdate.Parameters.AddWithValue("@EmployeeId", item.EmployeeId);
                        cmdUpdate.Parameters.AddWithValue("@DailyDate", Ordinary.DateToString(item.DailyDate));

                        var exec = cmdUpdate.ExecuteNonQuery();

                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #endregion Insert Settings
                    iSTransSuccess = true;
                }
                else
                {
                    retResults[1] = "Daily Attendance Updated Successfully.";
                    throw new ArgumentNullException(retResults[1], "");
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
                    retResults[1] = "Daily Attendance Updated Successfully.";

                }
                else
                {
                    retResults[1] = "Daily Attendance Updated Successfully.";
                    throw new ArgumentNullException(retResults[1], "");
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

        ////==================UpdateAttendanceMigration =================
        public string[] UpdateAttendanceMigration(List<AttendanceDailyNewVM> VMs, AttendanceDailyNewVM paramVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Monthly Absence"; //Method Name
            int transResult = 0;
            string sqlText = "";
            bool iSTransSuccess = false;

            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            DailyAttendanceProcessDAL _dailyAttnProcessDal = new DailyAttendanceProcessDAL();
            List<AttendanceMigrationVM> attnMigrationVMs = new List<AttendanceMigrationVM>();
            AttendanceMigrationDAL _attnMigrationDal = new AttendanceMigrationDAL();

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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region sqlText


                #endregion sqlText

                if (VMs != null && VMs.Count >= 1)
                {
                    #region Update Settings
                    foreach (var item in VMs)
                    {
                        #region CheckPoint
                        if (!item.IsEmployeeChecked)
                        {
                            continue;
                        }
                        sqlText = "  ";
                        sqlText += @" 
                        Update AttendanceMigration Set
                        PunchInTime = @InTime
                        ,IsManual = 1
                        ";

                        if (paramVM.IsNextDay)
                        {
                            sqlText += @"   ,PunchOutTimeNextday = @OutTime
                                            ,PunchNextDay=@PunchNextDay";
                        }
                        else
                        {
                            sqlText += @"   ,PunchOutTime=@OutTime 
                                            ,PunchNextDay=@PunchNextDay";
                        }
                        sqlText += @" WHERE 1=1 AND EmployeeId = @EmployeeId AND DailyDate = @DailyDate";

                        #endregion CheckPoint
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                        if (paramVM.IsInTimeUpdate)
                        {
                            cmdUpdate.Parameters.AddWithValue("@InTime", Ordinary.TimeToString(paramVM.UpdatedInTime));
                        }
                        else
                        {
                            cmdUpdate.Parameters.AddWithValue("@InTime", Ordinary.TimeToString(item.InTime));
                        }
                        if (paramVM.IsOutTimeUpdate)
                        {
                            cmdUpdate.Parameters.AddWithValue("@OutTime", Ordinary.TimeToString(paramVM.UpdatedOutTime));
                        }
                        else
                        {
                            cmdUpdate.Parameters.AddWithValue("@OutTime", Ordinary.TimeToString(item.OutTime));
                        }

                        if (item.PunchNextDay || paramVM.IsNextDay)
                        {
                            cmdUpdate.Parameters.AddWithValue("@PunchNextDay", true);
                        }
                        else
                        {
                            cmdUpdate.Parameters.AddWithValue("@PunchNextDay", false);
                        }

                        cmdUpdate.Parameters.AddWithValue("@EmployeeId", item.EmployeeId);
                        cmdUpdate.Parameters.AddWithValue("@DailyDate", Ordinary.DateToString(item.DailyDate));
                        var exec = cmdUpdate.ExecuteNonQuery();

                        #region Insert Into DailyAttendanceProcess
                        attnMigrationVMs = _attnMigrationDal.SelectMigration(item.DailyDate, true, "", item.EmployeeId, currConn, transaction);
                        if (attnMigrationVMs != null && attnMigrationVMs.Count > 0)
                        {
                            #region Delete Existing Data
                            string[] conFields = { "DailyDate", "EmployeeId" };
                            string[] conValues = { Ordinary.DateToString(item.DailyDate), item.EmployeeId };
                            retResults = _cDal.DeleteTable("AttendanceDailyNew", conFields, conValues, currConn, transaction); //Delete(DailyDate, EmployeeId, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                            #endregion Delete Existing Data

                            retResults = _dailyAttnProcessDal.ProcessMultiple(item.DailyDate, attnMigrationVMs, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        #endregion Insert Into DailyAttendanceProcess

                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #endregion Insert Settings
                    iSTransSuccess = true;
                }
                else
                {
                    retResults[1] = "Daily Attendance Updated Successfully.";
                    throw new ArgumentNullException(retResults[1], "");
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
                    retResults[1] = "Daily Attendance Updated Successfully.";

                }
                else
                {
                    retResults[1] = "Daily Attendance Updated Successfully.";
                    throw new ArgumentNullException(retResults[1], "");
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

        ////==================ProcessMultiple =================
        public string[] ProcessMultiple(string DailyDate, List<AttendanceMigrationVM> attnMigrationVMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "DailyAttendanceProcessDAL ProcessMultiple Fail";//Success or Fail
            //retResults[0] = "Fail";//Success or Fail
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
                //if (string.IsNullOrEmpty(AttendanceStructureVM.DepartmentId))
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
                #region sqlText

                sqlText = "  ";
                sqlText += @" 
INSERT INTO AttendanceDailyNew(
AttendanceMigrationId
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
,IsDeductEarlyOut

,MovementEarlyOutMin
,EarlyOutMin
,IsDeductLateIn
,MovementLateInMin
,LateInMin
,LunchBreak
,WorkingHrs
,WorkingHrsBy
,TotalOT
,TotalOTBy
,AttnStatus
,DayStatus
,EarlyDeduct
,LateDeduct
,TiffinAllow
,DinnerAllow
,IfterAllow
,TiffinAmnt
,IfterAmnt
,DinnerAmnt
,DeductTiffTime
,DeductIfterTime
,DeductDinTime
,GrossAmnt
,BasicAmnt
,Remarks



) 
                                VALUES (
@AttendanceMigrationId
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
,@IsDeductEarlyOut
,@MovementEarlyOutMin
,@EarlyOutMin
,@IsDeductLateIn
,@MovementLateInMin
,@LateInMin
,@LunchBreak
,@WorkingHrs
,@WorkingHrsBy
,@TotalOT
,@TotalOTBy
,@AttnStatus
,@DayStatus
,@EarlyDeduct
,@LateDeduct
,@TiffinAllow
,@DinnerAllow
,@IfterAllow
,@TiffinAmnt
,@IfterAmnt
,@DinnerAmnt
,@DeductTiffTime
,@DeductIfterTime
,@DeductDinTime
,@GrossAmnt
,@BasicAmnt
,@Remarks

) 
                                        ";
                #endregion sqlText
                #region varVariables
                EmployeeInfoDAL _eDal = new EmployeeInfoDAL();
                DesignationDAL _dDal = new DesignationDAL();
                HoliDayDAL _hDal = new HoliDayDAL();
                AttendanceMigrationDAL amdal = new AttendanceMigrationDAL();
                List<AttendanceMigrationVM> amvms = new List<AttendanceMigrationVM>();
                AttendanceDailyNewVM vm = new AttendanceDailyNewVM();
                CommonDAL _cDal = new CommonDAL();
                HoliDayDAL _holiDayDAL = new HoliDayDAL();
                HoliDayVM holiDayVM = new HoliDayVM();
                string holidayType = "";
                string DayStatus = "";

                EmployeeWeeklyHolidayDAL _ewhDAL = new EmployeeWeeklyHolidayDAL();
                EmployeeWeeklyHolidayVM ewhVM = new EmployeeWeeklyHolidayVM();
                #endregion

                holiDayVM = _holiDayDAL.SelectByDate(DailyDate, currConn, transaction);

                #region DayStatus

                holidayType = holiDayVM.HoliDayType;
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







                #endregion

                foreach (AttendanceMigrationVM amvm in attnMigrationVMs)
                {
                    #region Debugging Purpose
                    if (amvm.EmployeeId == "1_8")
                    {
                        int a = 1;
                    }

                    //if (DailyDate == "18-Dec-2017")
                    //{
                    //    int a = 1;
                    //}
                    #endregion

                    #region Delete Existing Data
                    string[] conFields = { "DailyDate", "EmployeeId", "IsManual" };
                    string[] conValues = { Ordinary.DateToString(DailyDate), amvm.EmployeeId, "0" };
                    retResults = _cDal.DeleteTable("AttendanceDailyNew", conFields, conValues, currConn, transaction); //Delete(DailyDate, EmployeeId, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {                    
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion Delete Existing Data

                    Random rnd = new Random();

                    #region ChekPoint

                    #endregion ChekPoint
                    DataTable employeeDt = new DataTable();
                    employeeDt = _eDal.SelectEmpForAttendance(amvm.EmployeeId, amvm.DailyDate, currConn, transaction);

                    #region Data Assing
                    vm.AttendanceMigrationId = amvm.Id;
                    vm.AttendanceStructureId = amvm.AttendanceStructureId;
                    vm.GroupId = amvm.GroupId;
                    vm.EmployeeId = amvm.EmployeeId;
                    vm.ProxyID = amvm.ProxyID;
                    vm.DailyDate = amvm.DailyDate;
                    vm.PunchInTime = amvm.PunchInTime;
                    vm.PunchOutTime = amvm.PunchOutTime;
                    vm.PunchOutTimeNextday = amvm.PunchOutTimeNextday;
                    vm.PunchNextDay = amvm.PunchNextDay;
                    vm.IsManual = amvm.IsManual;
                    vm.LunchBreak = amvm.PLunchBreak;

                    if (employeeDt != null && employeeDt.Rows.Count > 0)
                    {
                        ////continue;

                        vm.GrossAmnt = Convert.ToDecimal(employeeDt.Rows[0]["GrossSalary"]);
                        vm.BasicAmnt = Convert.ToDecimal(employeeDt.Rows[0]["BasicSalary"]);

                    }

                    vm.DayStatus = "R";
                    #endregion Data Assing
                    #region Condition Check

                    vm.DayStatus = DayStatus;

                    {
                        ewhVM = new EmployeeWeeklyHolidayVM();
                        string[] cFields = { "ewh.EmployeeId", "ewh.DailyDate", "ewh.IsActive" };
                        string[] cValues = { amvm.EmployeeId, Ordinary.DateToString(DailyDate), "1" };
                        ewhVM = _ewhDAL.SelectAll(0, cFields, cValues, currConn, transaction).FirstOrDefault();
                        if (ewhVM != null)
                        {
                            vm.DayStatus = "WH";
                        }
                    }




                    if (Convert.ToInt32(Ordinary.TimeToString(amvm.PunchInTime)) > 0)
                    {
                        vm.InTime = amvm.PunchInTime;
                        if (Convert.ToInt32(Ordinary.TimeToString(vm.InTime)) >= (Convert.ToInt32(Ordinary.TimeToString(vm.InTime)) - 15))
                        {
                            vm.InTimeBy = vm.InTime;
                        }
                        else
                        {
                            vm.InTimeBy = Ordinary.StringToTime(Convert.ToDateTime(amvm.PInTime).AddMinutes(rnd.Next(-15, amvm.PInGrace)).ToString());
                        }
                    }
                    else
                    {
                        vm.InTime = amvm.PunchInTime;
                        vm.InTimeBy = amvm.PunchInTime;
                    }

                    if (Convert.ToInt32(Ordinary.TimeToString(amvm.PunchOutTime)) > 0 || Convert.ToInt32(Ordinary.TimeToString(amvm.PunchOutTimeNextday)) > 0)
                    {
                        if (vm.PunchNextDay)
                        {
                            vm.OutTime = amvm.PunchOutTimeNextday;
                        }
                        else
                        {
                            vm.OutTime = amvm.PunchOutTime;
                        }
                        if (Convert.ToInt32(Ordinary.TimeToString(vm.OutTime)) >= (Convert.ToInt32(Ordinary.TimeToString(amvm.POutTime))))
                        {
                            vm.OutTimeBy = Ordinary.StringToTime(Convert.ToDateTime(amvm.POutTime).AddMinutes(rnd.Next(-9, 9)).ToString());
                        }
                    }
                    else
                    {
                        vm.OutTime = amvm.PunchOutTime;
                        vm.OutTimeBy = amvm.PunchOutTime;
                    }
                    vm.LateInMin = 0;
                    vm.MovementLateInMin = 0;

                    vm.IsDeductLateIn = amvm.PIsDeductLateIn;
                    if (vm.IsDeductLateIn)
                    {
                        if (Convert.ToDateTime(vm.InTime) > Convert.ToDateTime(amvm.PInTime).AddMinutes(amvm.PInGrace))
                        {
                            TimeSpan diff1 = Convert.ToDateTime(vm.InTime) - Convert.ToDateTime(amvm.PInTime);
                            vm.LateInMin = Convert.ToInt32(diff1.TotalMinutes);
                        }
                    }
                    if (vm.LateInMin > 60)
                    {
                        vm.MovementLateInMin = vm.LateInMin;
                        vm.LateInMin = 0;

                    }
                    //debbuging
                    if (vm.EmployeeId == "1_99")
                    {
                        int a = 0;
                    }


                    vm.MovementEarlyOutMin = 0;
                    vm.EarlyOutMin = 0;
                    vm.IsDeductEarlyOut = amvm.PIsDeductEarlyOut;

                    //if (vm.PunchNextDay == false)
                    //{
                        if (vm.IsDeductEarlyOut)
                        {
                            if (Convert.ToInt32(Ordinary.TimeToString(vm.OutTime)) > 0 && Convert.ToDateTime(vm.OutTime) < Convert.ToDateTime(amvm.POutTime).AddMinutes(-amvm.POutGrace))
                            {
                                TimeSpan diff1 = Convert.ToDateTime(amvm.POutTime) - Convert.ToDateTime(vm.OutTime);
                                vm.EarlyOutMin = Convert.ToInt32(diff1.TotalMinutes);
                            }
                        }

                    //}

                    vm.MovementEarlyOutMin = vm.EarlyOutMin;

                    #endregion Condition Check
                    #region Working Hour, Dinner, Tiffin, Ifter

                    //TimeSpan diff = secondDate - firstDate;
                    //double hours = diff.TotalHours;
                    vm.WorkingHrs = "0.0";
                    double TotalWorkingMin = 0;
                    double TotalWorkingMinBy = 0;
                    double min = 0;
                    double hrs = 0;
                    if (vm.PunchNextDay)
                    {
                        TimeSpan diff1 = Convert.ToDateTime(Ordinary.StringToTime("2359")) - Convert.ToDateTime(amvm.PInTime);
                        double m1 = diff1.TotalMinutes;
                        TimeSpan diff2 = Convert.ToDateTime(vm.OutTime) - Convert.ToDateTime(Ordinary.StringToTime("0001"));
                        double m2 = diff2.TotalMinutes;
                        TotalWorkingMin = m1 + m2 + 2;
                    }
                    else
                    {
                        TimeSpan diff1 = Convert.ToDateTime(vm.OutTime) - Convert.ToDateTime(amvm.PInTime);
                        double m1 = diff1.TotalMinutes;
                        TotalWorkingMin = m1;
                    }
                    TotalWorkingMin = TotalWorkingMin - amvm.PLunchBreak;

                    if (Convert.ToDecimal(TotalWorkingMin) > 0)
                    {
                        min = TotalWorkingMin % 60;
                        hrs = (TotalWorkingMin - min) / 60;
                    }
                    vm.WorkingHrs = hrs + "." + min.ToString().PadLeft(2, '0');
                    vm.WorkingHrsBy = vm.WorkingHrs;
                    if (Convert.ToDecimal(TotalWorkingMin) > (amvm.PWorkingMin + amvm.POTTime))
                    {
                        TotalWorkingMinBy = Convert.ToDouble(amvm.PWorkingMin + amvm.POTTime);
                        min = TotalWorkingMinBy % 60;
                        hrs = (TotalWorkingMinBy - min) / 60;
                        vm.WorkingHrsBy = hrs + "." + min.ToString().PadLeft(2, '0');
                    }


                    vm.TiffinAllow = amvm.PIsTiff;
                    vm.TiffinAmnt = 0;
                    vm.DeductTiffTime = 0;
                    if (vm.TiffinAllow)
                    {
                        if (TotalWorkingMin > amvm.PTiffinMin)
                        {
                            if (employeeDt != null && employeeDt.Rows.Count > 0)
                            {
                                vm.TiffinAmnt = Convert.ToDecimal(employeeDt.Rows[0]["TiffinAmount"]);
                            }
                            vm.DeductTiffTime = amvm.PDeductTiffTime;
                        }
                    }

                    vm.DinnerAllow = amvm.PIsDinner;
                    vm.DinnerAmnt = 0;
                    vm.DeductDinTime = 0;
                    if (vm.TiffinAllow)
                    {
                        if (TotalWorkingMin > amvm.PDinnerMin)
                        {
                            if (employeeDt != null && employeeDt.Rows.Count > 0)
                            {
                                vm.DinnerAmnt = Convert.ToDecimal(employeeDt.Rows[0]["DinnerAmount"]);
                            }
                            vm.DeductDinTime = amvm.PDeductDinTime;
                        }
                    }

                    vm.IfterAllow = amvm.PIsIfter;
                    vm.IfterAmnt = 0;
                    vm.DeductIfterTime = 0;
                    if (vm.TiffinAllow)
                    {
                        if (TotalWorkingMin > amvm.PDinnerMin)
                        {
                            if (employeeDt != null && employeeDt.Rows.Count > 0)
                            {
                                vm.IfterAmnt = Convert.ToDecimal(employeeDt.Rows[0]["IfterAmount"]);
                            }
                            vm.DeductIfterTime = amvm.PDeductIfterTime;
                        }
                    }

                    if (vm.DayStatus == "WH" || vm.DayStatus == "GH" || vm.DayStatus == "FH" || vm.DayStatus == "SH")
                    {
                        vm.TotalOT = Convert.ToInt32(TotalWorkingMin);
                    }
                    else
                    {
                        vm.TotalOT = Convert.ToInt32(TotalWorkingMin) - vm.DeductIfterTime - vm.DeductDinTime - vm.DeductTiffTime - amvm.PWorkingMin;

                    }

                    if (amvm.PIsOTRoundUp)
                    {
                        if (vm.TotalOT < amvm.POTRoundUpMin)
                        {
                            vm.TotalOT = 0;
                        }
                    }

                    vm.TotalOTBy = vm.TotalOT;
                    if (Convert.ToDecimal(TotalWorkingMin) > (amvm.PWorkingMin + amvm.POTTime))
                    {
                        vm.TotalOTBy = amvm.POTTime;
                    }
                    #endregion Working Hour, Dinner, Tiffin, Ifter
                    #region AttnStatus
                    string stringInTime = "";
                    string stringOutTime = "";
                    stringInTime = Ordinary.TimeToString(vm.InTime);
                    stringOutTime = Ordinary.TimeToString(vm.OutTime);

                    //{ "All", "Present", "Absent", "Late", "All Missing", "In Miss", "Out Miss" };
                    if (stringInTime == "0000" && stringOutTime == "0000")
                    {
                        vm.AttnStatus = "Absent";
                    }
                    else if (vm.LateInMin > 0 || vm.MovementLateInMin > 0)
                    {
                        vm.AttnStatus = "Late";
                    }
                    else if (vm.EarlyOutMin > 0 || vm.MovementEarlyOutMin > 0)
                    {
                        vm.AttnStatus = "Early Out";
                    }
                    else if (stringInTime == "0000" && stringOutTime != "0000")
                    {
                        vm.AttnStatus = "In Miss";
                    }
                    else if (stringInTime != "0000" && stringOutTime == "0000")
                    {
                        vm.AttnStatus = "Out Miss";
                    }
                    else
                    {
                        vm.AttnStatus = "Present";
                    }
                    #endregion

                    vm.Remarks = amvm.Remarks;

                    #region Insert

                    if (retResults[0] == "Success")
                    {
                        #region SqlCommand
                        if (1 == 1)
                        {
                            #region SqlExecution
                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                            //cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                            cmdInsert.Parameters.AddWithValue("@AttendanceMigrationId", vm.AttendanceMigrationId);
                            cmdInsert.Parameters.AddWithValue("@AttendanceStructureId", vm.AttendanceStructureId);
                            cmdInsert.Parameters.AddWithValue("@GroupId", vm.GroupId);
                            cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                            cmdInsert.Parameters.AddWithValue("@ProxyID", vm.ProxyID);
                            cmdInsert.Parameters.AddWithValue("@DailyDate", Ordinary.DateToString(vm.DailyDate));
                            cmdInsert.Parameters.AddWithValue("@PunchInTime", Ordinary.TimeToString(vm.PunchInTime));
                            cmdInsert.Parameters.AddWithValue("@PunchOutTime", Ordinary.TimeToString(vm.PunchOutTime));
                            cmdInsert.Parameters.AddWithValue("@PunchOutTimeNextday", Ordinary.TimeToString(vm.PunchOutTimeNextday));
                            cmdInsert.Parameters.AddWithValue("@PunchNextDay", vm.PunchNextDay);
                            cmdInsert.Parameters.AddWithValue("@IsManual", vm.IsManual);
                            cmdInsert.Parameters.AddWithValue("@InTime", Ordinary.TimeToString(vm.InTime));
                            cmdInsert.Parameters.AddWithValue("@InTimeBy", string.IsNullOrWhiteSpace(vm.InTimeBy) ? Ordinary.TimeToString(vm.InTime) : Ordinary.TimeToString(vm.InTimeBy));
                            cmdInsert.Parameters.AddWithValue("@OutTime", Ordinary.TimeToString(vm.OutTime));
                            cmdInsert.Parameters.AddWithValue("@OutTimeBy", string.IsNullOrWhiteSpace(vm.OutTimeBy) ? Ordinary.TimeToString(vm.OutTime) : Ordinary.TimeToString(vm.OutTimeBy));
                            cmdInsert.Parameters.AddWithValue("@IsDeductEarlyOut", vm.IsDeductEarlyOut);
                            cmdInsert.Parameters.AddWithValue("@MovementEarlyOutMin", vm.MovementEarlyOutMin);
                            cmdInsert.Parameters.AddWithValue("@EarlyOutMin", vm.EarlyOutMin);
                            cmdInsert.Parameters.AddWithValue("@IsDeductLateIn", vm.IsDeductLateIn);
                            cmdInsert.Parameters.AddWithValue("@MovementLateInMin", vm.MovementLateInMin);
                            cmdInsert.Parameters.AddWithValue("@LateInMin", vm.LateInMin);
                            cmdInsert.Parameters.AddWithValue("@LunchBreak", vm.LunchBreak);
                            cmdInsert.Parameters.AddWithValue("@WorkingHrs", vm.WorkingHrs);
                            cmdInsert.Parameters.AddWithValue("@WorkingHrsBy", vm.WorkingHrsBy);
                            cmdInsert.Parameters.AddWithValue("@TotalOT", vm.TotalOT > 0 ? vm.TotalOT : 0);
                            cmdInsert.Parameters.AddWithValue("@TotalOTBy", vm.TotalOTBy > 0 ? vm.TotalOTBy : 0);
                            cmdInsert.Parameters.AddWithValue("@AttnStatus", vm.AttnStatus);
                            cmdInsert.Parameters.AddWithValue("@DayStatus", vm.DayStatus);
                            cmdInsert.Parameters.AddWithValue("@EarlyDeduct", vm.EarlyDeduct);
                            cmdInsert.Parameters.AddWithValue("@LateDeduct", vm.LateDeduct);
                            cmdInsert.Parameters.AddWithValue("@TiffinAllow", vm.TiffinAllow);
                            cmdInsert.Parameters.AddWithValue("@DinnerAllow", vm.DinnerAllow);
                            cmdInsert.Parameters.AddWithValue("@IfterAllow", vm.IfterAllow);
                            cmdInsert.Parameters.AddWithValue("@TiffinAmnt", vm.TiffinAmnt);
                            cmdInsert.Parameters.AddWithValue("@IfterAmnt", vm.IfterAmnt);
                            cmdInsert.Parameters.AddWithValue("@DinnerAmnt", vm.DinnerAmnt);
                            cmdInsert.Parameters.AddWithValue("@DeductTiffTime", vm.DeductTiffTime);
                            cmdInsert.Parameters.AddWithValue("@DeductIfterTime", vm.DeductIfterTime);
                            cmdInsert.Parameters.AddWithValue("@DeductDinTime", vm.DeductDinTime);
                            cmdInsert.Parameters.AddWithValue("@GrossAmnt", vm.GrossAmnt);
                            cmdInsert.Parameters.AddWithValue("@BasicAmnt", vm.BasicAmnt);
                            cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks);

                            //cmdInsert.Parameters.AddWithValue("@InTime", Ordinary.TimeToString(vm.InTime));

                            var exec = cmdInsert.ExecuteNonQuery();
                            Id = Convert.ToInt32(exec);
                            retResults[3] = sqlText;
                            if (Id <= 0)
                            {
                                retResults[1] = "Data Not Process";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                            #endregion
                        }
                        else
                        {
                            retResults[1] = "This Time Policy already used!";
                            throw new ArgumentNullException("Please Input Time Policy Value", "");
                        }
                        #endregion SqlCommand

                    }
                    #endregion

                    //vm.Id = vm.Id + 1;
                }
                #region Update AttendanceDailyNewOT
                SettingDAL _settingDal = new SettingDAL();
                string DailyOTRoundUp = _settingDal.settingValue("OverTime", "DailyOTRoundUp", currConn, transaction).Trim();

                retResults = UpdateTotalOT(DailyDate, DailyOTRoundUp, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], "");
                }

                #endregion Update AttendanceDailyNewOT
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

        ////==================Delete =================
        public string[] Delete(string DailyDate, string EmployeeId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteBank"; //Method Name

            string sqlText = "";

            bool iSTransSuccess = false;

            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;

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


                #region Update Settings

                sqlText = "";
                sqlText = "delete from AttendanceDailyNew  ";
                sqlText += " where 1=1 AND ISNULL(IsManual,0) = 0";

                if (!string.IsNullOrWhiteSpace(DailyDate))
                {
                    sqlText += " and DailyDate=@DailyDate";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += " and EmployeeId=@EmployeeId";
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
                var exec = objComm.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);
                //if (transResult <= 0)
                //{
                //    retResults[1] = "Update Fail!";
                //    throw new ArgumentNullException(retResults[1], "");
                //}

                retResults[2] = "";// Return Id
                retResults[3] = sqlText; //  SQL Query

                #region Commit



                #endregion Commit

                #endregion Update Settings
                iSTransSuccess = true;



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
                    retResults[1] = "Unexpected error to delete Holi Day Information.";
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


        ////==================Report=================
        public DataTable Report(EmployeeInfoVM vm, string[] conFields = null, string[] conValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                #region open connection and transaction
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
                #region sql statement
                sqlText = @"
Select 
--attn.Id
------From Other Table------
ve.Code
,ve.OtherId
,ve.EmpName
,ve.Department
,ve.Section
,ve.Project
,ve.Designation
,ve.Other1 as Division
,ve.EmpJobType
,ve.Supervisor
,CONVERT(NVARCHAR, CONVERT(DATE,  ve.JoinDate, 112), 103) JoinDate
,ast.Name PolicyName
,ast.InTime PInTime
,ast.OutTime POutTime
,gr.Name GroupName
------This Table------
,attn.EmployeeId
,attn.ProxyID
,attn.AttendanceMigrationId
,attn.AttendanceStructureId
,attn.GroupId
,CONVERT(NVARCHAR, CONVERT(DATE, attn.DailyDate, 112), 103) DailyDate
,attn.DailyDate DailyDateReportGroup
,attn.PunchInTime
,attn.PunchOutTime
,attn.PunchOutTimeNextday
,attn.PunchNextDay
,attn.IsManual
,attn.InTime
,attn.InTimeBy
,attn.OutTime
,attn.OutTimeBy
,attn.IsDeductEarlyOut
,attn.EarlyOutMin
,attn.IsDeductLateIn
,attn.LateInMin
,attn.LunchBreak
,case when attn.WorkingHrs = '0' then '0.00' else attn.WorkingHrs end as WorkingHrs
,case when attn.WorkingHrsBy = '0' then '0.00' else attn.WorkingHrsBy end as WorkingHrsBy
,attn.TotalOT
,attn.TotalOTBy
,case when attn.DayStatus='WH' then 'W Holiday' when attn.DayStatus='GH' then 'G Holiday' when attn.DayStatus='FH' then 'Festival' when attn.DayStatus='Leave' then 'Present' else attn.AttnStatus end as AttnStatus
,attn.DayStatus
,attn.EarlyDeduct
,attn.LateDeduct
,attn.TiffinAllow
,attn.DinnerAllow
,attn.IfterAllow
,attn.TiffinAmnt
,attn.IfterAmnt
,attn.DinnerAmnt
,attn.DeductTiffTime
,attn.DeductIfterTime
,attn.DeductDinTime
,attn.GrossAmnt
,attn.BasicAmnt
,attn.Remarks
,Isnull(attn.MovementEarlyOutMin,0) MovementEarlyOutMin
,Isnull(attn.MovementLateInMin,0) MovementLateInMin
,Isnull(d.OrderNo,0) DepartmentOrder
,Isnull(s.OrderNo,0) SectionOrder
,Isnull(p.OrderNo,0) ProjectOrder
From AttendanceDailyNew attn
Left Outer Join ViewEmployeeInformation ve ON attn.EmployeeId = ve.EmployeeId 
Left Outer Join AttendanceStructure ast ON attn.AttendanceStructureId = ast.Id
left outer join Department D on ve.DepartmentId=d.Id
left outer join Section S on ve.SectionId=S.Id
left outer join Project p on ve.ProjectId=p.Id
Left Outer Join [Group] gr ON attn.GroupId = gr.Id
WHERE 1=1 
";

                #endregion sql statement
                //{ "All", "Present", "Absent", "Late", "All Missing", "In Miss", "Out Miss" };
                #region sql Execution
                if (vm.AttnStatus == "Present")
                {
                    sqlText += " AND (attn.InTime <> '0000' AND attn.OutTime <> '0000')";
                }
                else if (vm.AttnStatus == "Absent")
                {
                    sqlText += " AND (attn.InTime = '0000' AND attn.OutTime = '0000')";
                }
                else if (vm.AttnStatus == "Late")
                {
                    sqlText += "  and AttnStatus in('late')";
                }
                else if (vm.AttnStatus == "Late and Absent")
                {
                    sqlText += " AND (  AttnStatus in('late') or (attn.InTime = '0000' AND attn.OutTime = '0000'))";
                }
                else if (vm.AttnStatus == "All Missing")
                {
                    sqlText += @" AND ((attn.InTime = '0000' AND attn.OutTime <> '0000') OR 
                                        (attn.InTime <> '0000' AND attn.OutTime = '0000'))";
                }
                else if (vm.AttnStatus == "In Miss")
                {
                    sqlText += " AND (attn.InTime = '0000' AND attn.OutTime <> '0000')";
                }
                else if (vm.AttnStatus == "Out Miss")
                {
                    sqlText += " AND (attn.InTime <> '0000' AND attn.OutTime = '0000')";
                }

                string cField = "";
                if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                {
                    for (int i = 0; i < conFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[i]) || string.IsNullOrWhiteSpace(conValues[i]) || conValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conFields[i] + "=@" + cField;
                    }
                }
                //sqlText += " ORDER By attn.DailyDate, ve.Department, ve.Section, ve.Code";
                sqlText += " ORDER By attn.DailyDate, d.OrderNo, s.OrderNo";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                {
                    for (int j = 0; j < conFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[j]) || string.IsNullOrWhiteSpace(conValues[j]) || conValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conValues[j]);
                    }
                }
                da.Fill(dt);
                #region ColumnDateChange
                string[] columnDate = { "DailyDate", "JoinDate" };
               // dt = Ordinary.DtMultiColumnStringToDate(dt, columnDate);
                #endregion ColumnDateChange
                #region ColumnTimeChange
                string[] columnTime = { "PInTime","POutTime","PunchInTime","PunchOutTime"
                                          ,"PunchOutTimeNextday","InTime","InTimeBy","OutTime","OutTimeBy"};
                dt = Ordinary.DtMultiColumnStringToTime(dt, columnTime);
                #endregion ColumnTimeChange


                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion sql Execution
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        public DataTable ReportAttendanceSummery(EmployeeInfoVM vm, string[] conFields = null, string[] conValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                #region open connection and transaction
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
                #region sql statement
                sqlText = @"
 with cat as(
 Select a.EmployeeId,ve.Code,ve.EmpName,ve.Department,ve.Designation,
  case when AttnStatus='Present' then Count(AttnStatus) else 0 end as Present ,
  case when AttnStatus='Absent' or DayStatus='GH' or DayStatus='WH' then Count(AttnStatus) else 0 end as Absent, 
  case when AttnStatus='Leave' then Count(AttnStatus) else 0 end as Leave, 
  case when AttnStatus='Late' then Count(AttnStatus) else 0 end as Late, 
  case when AttnStatus='In Miss' then Count(AttnStatus) else 0 end as InMiss, 
  case when AttnStatus='Out Miss' then Count(AttnStatus) else 0 end as OutMiss,
  case when DayStatus='WH' then Count(AttnStatus) else 0 end as WH,
  case when DayStatus='GH' then Count(AttnStatus) else 0 end as GH,
   Count(AttnStatus) TotalDay
  
  from AttendanceDailyNew a
   Left Outer Join ViewEmployeeInformation ve on ve.EmployeeId=a.EmployeeId  
  where  1=1 
";

              //  sqlText += "  DailyDate between '20240201' and '20240229'";


                #endregion sql statement
            
                #region sql Execution
               

                string cField = "";
                if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                {
                    for (int i = 0; i < conFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[i]) || string.IsNullOrWhiteSpace(conValues[i]) || conValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conFields[i] + "=@" + cField;
                    }
                }
                sqlText += @" group by a.EmployeeId,ve.Code,ve.EmpName,ve.Department,ve.Designation,AttnStatus,DayStatus
                              ) 
                              Select c.EmployeeId,Code,EmpName,Department,Designation,
                              Sum(Present)Present,Sum(Absent)-(Sum(GH)+Sum(WH))Absent,Sum(Leave)Leave,Sum(Late)Late,Sum(InMiss)InMiss,Sum(OutMiss)OutMiss,Sum(WH)WH,Sum(GH)GH,Sum(TotalDay)TotalDay  
                              from cat c  group by c.EmployeeId,EmpName,Code,Department,Designation";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                {
                    for (int j = 0; j < conFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[j]) || string.IsNullOrWhiteSpace(conValues[j]) || conValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conValues[j]);
                    }
                }
                da.Fill(dt);
               
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion sql Execution
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        ////==================ReportBySupervisor=================
        public DataTable ReportBySupervisor(EmployeeInfoVM vm, string[] conFields = null, string[] conValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                #region open connection and transaction
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
                #region sql statement
                sqlText = @"
Select 
--attn.Id
------From Other Table------
ve.Code
,ve.EmpName
,ve.Department
,ve.Section
,ve.Project
,ve.Designation
,CONVERT(NVARCHAR, CONVERT(DATE,  ve.JoinDate, 112), 103) JoinDate
,ast.Name PolicyName
,ast.InTime PInTime
,ast.OutTime POutTime
,gr.Name GroupName
------This Table------
,attn.EmployeeId
,attn.ProxyID
,attn.AttendanceMigrationId
,attn.AttendanceStructureId
,attn.GroupId
,CONVERT(NVARCHAR, CONVERT(DATE, attn.DailyDate, 112), 103) DailyDate
,attn.DailyDate DailyDateReportGroup
,attn.PunchInTime
,attn.PunchOutTime
,attn.PunchOutTimeNextday
,attn.PunchNextDay
,attn.IsManual
,attn.InTime
,attn.InTimeBy
,attn.OutTime
,attn.OutTimeBy
,attn.IsDeductEarlyOut
,attn.EarlyOutMin
,attn.IsDeductLateIn
,attn.LateInMin
,attn.LunchBreak
,case when attn.WorkingHrs = '0' then '0.00' else attn.WorkingHrs end as WorkingHrs
,case when attn.WorkingHrsBy = '0' then '0.00' else attn.WorkingHrsBy end as WorkingHrsBy
,attn.TotalOT
,attn.TotalOTBy
,case when attn.DayStatus='WH' then 'Friday' when attn.DayStatus='GH' then 'Holiday' when attn.DayStatus='FH' then 'Festival' else attn.AttnStatus end as AttnStatus
,attn.DayStatus
,attn.EarlyDeduct
,attn.LateDeduct
,attn.TiffinAllow
,attn.DinnerAllow
,attn.IfterAllow
,attn.TiffinAmnt
,attn.IfterAmnt
,attn.DinnerAmnt
,attn.DeductTiffTime
,attn.DeductIfterTime
,attn.DeductDinTime
,attn.GrossAmnt
,attn.BasicAmnt
,attn.Remarks
,Isnull(attn.MovementEarlyOutMin,0) MovementEarlyOutMin
,Isnull(attn.MovementLateInMin,0) MovementLateInMin
,Isnull(d.OrderNo,0) DepartmentOrder
,Isnull(s.OrderNo,0) SectionOrder
,Isnull(p.OrderNo,0) ProjectOrder
From AttendanceDailyNew attn
Left Outer Join ViewEmployeeInformation ve ON attn.EmployeeId = ve.EmployeeId 
Left Outer Join AttendanceStructure ast ON attn.AttendanceStructureId = ast.Id
left outer join Department D on ve.DepartmentId=d.Id
left outer join Section S on ve.SectionId=S.Id
left outer join Project p on ve.ProjectId=p.Id
Left Outer Join [Group] gr ON attn.GroupId = gr.Id
WHERE 1=1 
";

                #endregion sql statement
                //{ "All", "Present", "Absent", "Late", "All Missing", "In Miss", "Out Miss" };
                #region sql Execution
                if (vm.AttnStatus == "Present")
                {
                    sqlText += " AND (attn.InTime <> '0000' AND attn.OutTime <> '0000')";
                }
                else if (vm.AttnStatus == "Absent")
                {
                    sqlText += " AND (attn.InTime = '0000' AND attn.OutTime = '0000')";
                }
                else if (vm.AttnStatus == "Late")
                {
                    sqlText += "  and AttnStatus in('late')";
                }
                else if (vm.AttnStatus == "Late and Absent")
                {
                    sqlText += " AND (  AttnStatus in('late') or (attn.InTime = '0000' AND attn.OutTime = '0000'))";
                }
                else if (vm.AttnStatus == "All Missing")
                {
                    sqlText += @" AND ((attn.InTime = '0000' AND attn.OutTime <> '0000') OR 
                                        (attn.InTime <> '0000' AND attn.OutTime = '0000'))";
                }
                else if (vm.AttnStatus == "In Miss")
                {
                    sqlText += " AND (attn.InTime = '0000' AND attn.OutTime <> '0000')";
                }
                else if (vm.AttnStatus == "Out Miss")
                {
                    sqlText += " AND (attn.InTime <> '0000' AND attn.OutTime = '0000')";
                }

                string cField = "";
                if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                {
                    for (int i = 0; i < conFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[i]) || string.IsNullOrWhiteSpace(conValues[i]) || conValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conFields[i] + "=@" + cField;
                    }
                }
                sqlText += " And SUBSTRING(ve.Supervisor, 0, CHARINDEX('~', ve.Supervisor))= @Code";
                sqlText += " ORDER By attn.DailyDate, d.OrderNo, s.OrderNo";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                {
                    for (int j = 0; j < conFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[j]) || string.IsNullOrWhiteSpace(conValues[j]) || conValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conValues[j]);                       
                    }
                    da.SelectCommand.Parameters.AddWithValue("@Code", vm.CodeF);
                }
                da.Fill(dt);
                #region ColumnDateChange
                string[] columnDate = { "DailyDate", "JoinDate" };
                // dt = Ordinary.DtMultiColumnStringToDate(dt, columnDate);
                #endregion ColumnDateChange
                #region ColumnTimeChange
                string[] columnTime = { "PInTime","POutTime","PunchInTime","PunchOutTime"
                                          ,"PunchOutTimeNextday","InTime","InTimeBy","OutTime","OutTimeBy"};
                dt = Ordinary.DtMultiColumnStringToTime(dt, columnTime);
                #endregion ColumnTimeChange


                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion sql Execution
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        //==================UpdateTotalOT =================
        public string[] UpdateTotalOT(string DailyDate, string DailyOTRoundUp, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "UpdateAttendanceDailyNewOT"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region Save
                sqlText = "  ";
                sqlText += @" 
--declare @DailyOTRoundUp as int
--declare @DailyDate as varchar(20)
--set @DailyDate ='20171112'
--set @DailyOTRoundUp=30
update AttendanceDailyNew set TotalOTActual=TotalOT
where DailyDate=@DailyDate
update AttendanceDailyNew set TotalOT= TotalOT-( TotalOT % @DailyOTRoundUp)
where TotalOT>0
and TotalOT % 60< @DailyOTRoundUp 
and DailyDate=@DailyDate

";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@DailyDate", Ordinary.DateToString(DailyDate));
                cmdInsert.Parameters.AddWithValue("@DailyOTRoundUp", Convert.ToDecimal(DailyOTRoundUp));
                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                //////if (transResult <= 0)
                //////{
                //////    retResults[1] = "Unexpected error to Update Attendance Daily New OT!";
                //////    throw new ArgumentNullException(retResults[1], "");
                //////}

                #endregion Save

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Attendance Daily New OT Updated Successfully!";


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


        //AttendanceDailyToDailyOTnAbsenceProcess
        public string[] Process1(string FiscalYearDetailId, ShampanIdentityVM auditvm, string EmployeeId = "0_0", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Process1"; //Method Name
            //int transResult = 0;
            //string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region Save

                retResults = AttendanceDailyNewToEmployeeDailyOvertimeP1(FiscalYearDetailId, auditvm, EmployeeId, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], "");
                }
                retResults = AttendanceDailyNewToEmployeeDailyAbsenceP1(FiscalYearDetailId, auditvm, EmployeeId, currConn, transaction);
                if (retResults[0] == "Fail")
                {
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
                retResults[0] = "Success";
                retResults[1] = "Employee Daily Overtime and Daily Absence Updated Successfully!";
                #endregion Commit


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

        public string[] AttendanceDailyNewToEmployeeDailyOvertimeP1(string FiscalYearDetailId, ShampanIdentityVM auditvm, string EmployeeId = "0_0", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "AttendanceDailyNewToEmployeeDailyOvertime"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region Save
                sqlText = "  ";
                sqlText += @" 
--declare @FiscalYearDetailId as varchar(20)
declare @PeriodStart as varchar(20)
declare @PeriodEnd as varchar(20)
--set @FiscalYearDetailId='1005'
select @PeriodStart=PeriodStart, @PeriodEnd=PeriodEnd from FiscalYearDetail
where id=@FiscalYearDetailId

delete EmployeeDailyOvertime
where OvertimeDate between  @PeriodStart and @PeriodEnd
";
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" and EmployeeId=@EmployeeId";
                }
                else
                {
                    sqlText += @" and ISNULL(IsManual,0)=0";
                }

                sqlText += @" 

insert into EmployeeDailyOvertime(
EmployeeId
,OvertimeDate
,Overtime
,OvertimeBy
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,DayStatus
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,LateInHrs
,EarlyOutHrs
,LateInMins
,EarlyOutMins

)
select 
d.EmployeeId
,d.DailyDate OvertimeDate
--,d.TotalOT Overtime
,case when d.DayStatus='R' then d.TotalOT else convert(decimal(18,0), d.WorkingHrs ) *60 + (convert(decimal(18,2),d.WorkingHrs) % 1)*100 end  Overtime
,d.TotalOTBy OvertimeBy
,j.ProjectId
,j.DepartmentId
,j.SectionId
,j.DesignationId
,d.DayStatus 
,'' Remarks
,@IsActive        IsActive
,@IsArchive       IsArchive
,@CreatedBy       CreatedBy
,@CreatedAt       CreatedAt
,@CreatedFrom     CreatedFrom
,@LastUpdateBy    LastUpdateBy
,@LastUpdateAt    LastUpdateAt
,@LastUpdateFrom  LastUpdateFrom

, convert(varchar(10), convert(int,isnull(d.MovementLateInMin,0)/60) )+'.'+   RIGHT('00'+convert(varchar(10),isnull(d.MovementLateInMin,0) %60),2) LateInHrs
, convert(varchar(10), convert(int,isnull(d.MovementEarlyOutMin,0)/60)) +'.'+  RIGHT('00'+convert(varchar(10),isnull(d.MovementEarlyOutMin,0) %60),2) EarlyOutHrs

,d.MovementLateInMin LateInMins
,d.MovementEarlyOutMin EarlyOutMins

 from AttendanceDailyNew d
left outer join EmployeeJob j on d.EmployeeId=j.EmployeeId
where d.DailyDate between  @PeriodStart and @PeriodEnd
";
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" and d.EmployeeId=@EmployeeId";
                }
                else
                {
                    sqlText += @" and d.EmployeeId not in(
 select EmployeeId from EmployeeDailyOvertime
where OvertimeDate between  @PeriodStart and @PeriodEnd
 and IsManual=1 ) ";
                }

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);
                cmdInsert.Parameters.AddWithValue("@LastUpdateBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@LastUpdateAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@LastUpdateFrom", auditvm.CreatedFrom);

                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                if (transResult <= 0)
                {
                    retResults[1] = "Unexpected error to Update Employee Daily Overtime!";
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
                retResults[0] = "Success";
                retResults[1] = "Employee Daily Overtime Updated Successfully!";
                #endregion Commit
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
        public string[] AttendanceDailyNewToEmployeeDailyAbsenceP1(string FiscalYearDetailId, ShampanIdentityVM auditvm, string EmployeeId = "0_0", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "AttendanceDailyNewToEmployeeDailyAbsence"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region Save
                sqlText = "  ";
                sqlText += @" 
--declare @FiscalYearDetailId as varchar(20)
declare @PeriodStart as varchar(20)
declare @PeriodEnd as varchar(20)
--set @FiscalYearDetailId='1005'
select @PeriodStart=PeriodStart, @PeriodEnd=PeriodEnd from FiscalYearDetail
where id=@FiscalYearDetailId

delete EmployeeDailyAbsence
where AbsentDate between  @PeriodStart and @PeriodEnd
";
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" and EmployeeId=@EmployeeId";
                }
                else
                {
                    sqlText += @" and ISNULL(IsManual,0) = 0";
                }

                sqlText += @" 

insert into EmployeeDailyAbsence(
 EmployeeId
,AbsentDate
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,TransactionType
,DayStatus
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
)
select 
 d.EmployeeId
,d.DailyDate AbsentDate
,j.ProjectId
,j.DepartmentId
,j.SectionId
,j.DesignationId
,case 
WHEN isnull(d.AttnStatus,0) = 'Absent' THEN 'Absence'
WHEN isnull(d.LateInMin,0) > 0 THEN 'LateIn' 
WHEN isnull(d.EarlyOutMin,0) > 0 THEN 'EarlyOut' 
Else ''
End  TransactionType
,DayStatus
,'' Remarks
,@IsActive          IsActive
,@IsArchive         IsArchive
,@CreatedBy         CreatedBy
,@CreatedAt         CreatedAt
,@CreatedFrom       CreatedFrom
,@LastUpdateBy      LastUpdateBy
,@LastUpdateAt      LastUpdateAt
,@LastUpdateFrom    LastUpdateFrom

 from AttendanceDailyNew d
left outer join EmployeeJob j on d.EmployeeId=j.EmployeeId
where d.DailyDate between  @PeriodStart and @PeriodEnd
";
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" and d.EmployeeId=@EmployeeId";
                }
                else
                {
                    sqlText += @" and d.EmployeeId not in(
 select EmployeeId  FROM  EmployeeDailyAbsence
where AbsentDate between  @PeriodStart and @PeriodEnd
 and IsManual=1 ) ";
                }


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);
                cmdInsert.Parameters.AddWithValue("@LastUpdateBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@LastUpdateAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@LastUpdateFrom", auditvm.CreatedFrom);
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                }
                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                //if (transResult <= 0)
                //{
                //    retResults[1] = "Unexpected error to Update Employee Daily Absence!";
                //    throw new ArgumentNullException(retResults[1], "");
                //}
                #endregion Save
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Employee Daily Absence Updated Successfully!";


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

        //AttendanceDailyToMonthlyOTnAbsenceProcess
        public string[] Process2(string FiscalYearDetailId, ShampanIdentityVM auditvm, string EmployeeId = "0_0", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Process2"; //Method Name
            //int transResult = 0;
            //string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region Save

                retResults = AttendanceDailyNewToEmployeeMonthlyOvertimeP2(FiscalYearDetailId, auditvm, EmployeeId, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], "");
                }
                retResults = AttendanceDailyNewToEmployeeMonthlyAbsenceP2(FiscalYearDetailId, auditvm, EmployeeId, currConn, transaction);
                if (retResults[0] == "Fail")
                {
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
                retResults[0] = "Success";
                retResults[1] = "Employee Monthly Overtime and Monthly Absence Updated Successfully!";
                #endregion Commit


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

        public string[] AttendanceDailyNewToEmployeeMonthlyOvertimeP2(string FiscalYearDetailId, ShampanIdentityVM auditvm, string EmployeeId = "0_0", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "AttendanceDailyNewToEmployeeMonthlyOvertime"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region Save
                sqlText = "  ";
                sqlText += @" 
--declare @FiscalYearDetailId as varchar(20)
declare @PeriodStart as varchar(20)
declare @PeriodEnd as varchar(20)
--set @FiscalYearDetailId='1005'
select @PeriodStart=PeriodStart, @PeriodEnd=PeriodEnd from FiscalYearDetail
where id=@FiscalYearDetailId
delete EmployeeMonthlyOvertime
where FiscalYearDetailId=@FiscalYearDetailId
";
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" and EmployeeId=@EmployeeId";
                }
                else
                {
                    sqlText += @" and ISNULL(IsManual,0)=0";
                }

                sqlText += @" 

insert into EmployeeMonthlyOvertime(
EmployeeId
,FiscalYearDetailId
,TotalOvertimeActual
,TotalOvertime
,TotalOvertimeBy
,ProjectId
,DepartmentId
,SectionId
,DesignationId

,OTRate
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,TotalLateInHrs
,TotalEarlyOutHrs

,TotalLateInMins
,TotalEarlyOutMins
)

select 
 d.EmployeeId
,@FiscalYearDetailId FiscalYearDetailId
,0 TotalOvertimeActual
,d.Overtime TotalOvertime
,d.OvertimeBy TotalOvertimeBy

,j.ProjectId
,j.DepartmentId
,j.SectionId
,j.DesignationId
,0 OTRate
,'' Remarks
,@IsActive        IsActive
,@IsArchive       IsArchive
,@CreatedBy       CreatedBy
,@CreatedAt       CreatedAt
,@CreatedFrom     CreatedFrom
,@LastUpdateBy    LastUpdateBy
,@LastUpdateAt    LastUpdateAt
,@LastUpdateFrom  LastUpdateFrom

, convert(varchar(10), convert(int,isnull(d.MovementLateInMin,0)/60)) +'.'+   RIGHT('00'+convert(varchar(10),isnull(d.MovementLateInMin,0) %60),2)               TotalLateInHrs
, convert(varchar(10), convert(int,isnull(d.MovementEarlyOutMin,0)/60)) +'.'+  RIGHT('00'+convert(varchar(10),isnull(d.MovementEarlyOutMin,0) %60),2)TotalEarlyOutHrs
,d.MovementLateInMin TotalLateInMins
,d.MovementEarlyOutMin TotalEarlyOutMins
 from (
select 
d.EmployeeId
,sum(isnull(d.TotalOT,0)) Overtime
,sum(isnull(d.TotalOTBy,0)) OvertimeBy
,sum(isnull(d.LateInMin,0)) LateInMin
,sum(isnull(d.EarlyOutMin,0)) EarlyOutMin
,sum(isnull(d.MovementLateInMin,0)) MovementLateInMin
,sum(isnull(d.MovementEarlyOutMin,0)) MovementEarlyOutMin
from AttendanceDailyNew d

where d.DailyDate between @PeriodStart and @PeriodEnd
";
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" and d.EmployeeId=@EmployeeId";
                }
                else
                {
                    sqlText += @" and d.EmployeeId not in(
 select EmployeeId FROM  EmployeeMonthlyOvertime
where FiscalYearDetailId=@FiscalYearDetailId
 and IsManual=1 ) ";
                }
                sqlText += @" 

group by d.EmployeeId) as d
left outer join EmployeeJob j on d.EmployeeId=j.EmployeeId

";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);
                cmdInsert.Parameters.AddWithValue("@LastUpdateBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@LastUpdateAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@LastUpdateFrom", auditvm.CreatedFrom);
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                //if (transResult <= 0)
                //{
                //    retResults[1] = "Unexpected error to Update Employee Monthly Overtime!";
                //    throw new ArgumentNullException(retResults[1], "");
                //}


                #region Update MonthlyOvertime
                SettingDAL _settingDal = new SettingDAL();
                EmployeeMonthlyOvertimeDAL _EmployeeMonthlyOvertimeDAL = new EmployeeMonthlyOvertimeDAL();
                string MonthlyOTRoundUp = _settingDal.settingValue("OverTime", "MonthlyOTRoundUp", currConn, transaction).Trim();

                retResults = _EmployeeMonthlyOvertimeDAL.UpdateTotalOvertime(FiscalYearDetailId, MonthlyOTRoundUp, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], "");
                }

                #endregion Update MonthlyOvertime
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Employee Monthly Overtime Updated Successfully!";
                #endregion Commit
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

        public string[] AttendanceDailyNewToEmployeeMonthlyAbsenceP2(string FiscalYearDetailId, ShampanIdentityVM auditvm, string EmployeeId = "0_0", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "AttendanceDailyNewToEmployeeMonthlyAbsence"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region Save
                sqlText = "  ";
                sqlText += @" 
--declare @FiscalYearDetailId as varchar(20)
declare @PeriodStart as varchar(20)
declare @PeriodEnd as varchar(20)
--set @FiscalYearDetailId='1005'
select @PeriodStart=PeriodStart, @PeriodEnd=PeriodEnd from FiscalYearDetail
where id=@FiscalYearDetailId
delete EmployeeMonthlyAbsence
where FiscalYearDetailId=@FiscalYearDetailId
";
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" and EmployeeId=@EmployeeId";
                }
                else
                {
                    sqlText += @" and ISNULL(IsManual,0)=0";
                }

                sqlText += @" 

insert into EmployeeMonthlyAbsence(
EmployeeId
,FiscalYearDetailId
,AbsentDays
,LateInDays
,EarlyOutDays
,ProjectId
,DepartmentId
,SectionId
,DesignationId

,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

)

select 
 d.EmployeeId
,@FiscalYearDetailId FiscalYearDetailId
,AbsentDays  
,LateInDays
,EarlyOutDays

,j.ProjectId
,j.DepartmentId
,j.SectionId
,j.DesignationId

,'' Remarks
,@IsActive        IsActive
,@IsArchive       IsArchive
,@CreatedBy       CreatedBy
,@CreatedAt       CreatedAt
,@CreatedFrom     CreatedFrom
,@LastUpdateBy    LastUpdateBy
,@LastUpdateAt    LastUpdateAt
,@LastUpdateFrom  LastUpdateFrom
 from (
select 
d.EmployeeId
,sum(case WHEN isnull(d.AttnStatus,0) = 'Absent' and d.DayStatus='R' THEN 1 else 0 End ) AbsentDays  
,sum(case WHEN isnull(d.LateInMin,0) > 0 THEN 1 else 0 End) 		 LateInDays
,sum(case WHEN isnull(d.EarlyOutMin,0) > 0 THEN 1 else 0 End) 	     EarlyOutDays

from AttendanceDailyNew d

where d.DailyDate between @PeriodStart and @PeriodEnd
";
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" and d.EmployeeId=@EmployeeId";
                }
                else
                {
                    sqlText += @" and d.EmployeeId not in(
 select EmployeeId from EmployeeMonthlyAbsence
where FiscalYearDetailId=@FiscalYearDetailId
 and IsManual=1 ) ";
                }
                sqlText += @" 

group by d.EmployeeId) as d
left outer join EmployeeJob j on d.EmployeeId=j.EmployeeId
";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);
                cmdInsert.Parameters.AddWithValue("@LastUpdateBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@LastUpdateAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@LastUpdateFrom", auditvm.CreatedFrom);
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                //if (transResult <= 0)
                //{
                //    retResults[1] = "Unexpected error to Update Employee Monthly Absence!";
                //    throw new ArgumentNullException(retResults[1], "");
                //}
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Employee Monthly Absence Updated Successfully!";
                #endregion Commit
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

        //DailyOTnAbsenceToMonthlyOTnAbsenceProcess
        public string[] Process3(string FiscalYearDetailId, ShampanIdentityVM auditvm, string EmployeeId = "0_0", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Process3"; //Method Name
            //int transResult = 0;
            //string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region Save

                retResults = EmployeeDailyOvertimeToEmployeeMonthlyOvertimeP3(FiscalYearDetailId, auditvm, EmployeeId, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], "");
                }
                retResults = EmployeeDailyAbsenceToEmployeeMonthlyAbsenceP3(FiscalYearDetailId, auditvm, EmployeeId, currConn, transaction);
                if (retResults[0] == "Fail")
                {
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
                retResults[0] = "Success";
                retResults[1] = "Employee Monthly Overtime and Monthly Absence Updated Successfully!";
                #endregion Commit


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

        public string[] EmployeeDailyOvertimeToEmployeeMonthlyOvertimeP3(string FiscalYearDetailId, ShampanIdentityVM auditvm, string EmployeeId = "0_0", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeeDailyOvertimeToEmployeeMonthlyOvertime"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region Save
                sqlText = "  ";
                sqlText += @" 
--declare @FiscalYearDetailId as varchar(20)
declare @PeriodStart as varchar(20)
declare @PeriodEnd as varchar(20)
--set @FiscalYearDetailId='1005'
select @PeriodStart=PeriodStart, @PeriodEnd=PeriodEnd from FiscalYearDetail
where id=@FiscalYearDetailId

delete EmployeeMonthlyOvertime
where FiscalYearDetailId=@FiscalYearDetailId";
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" and EmployeeId=@EmployeeId";
                }
                else
                {
                    sqlText += @" and ISNULL(IsManual,0)=0";
                }

                sqlText += @" insert into EmployeeMonthlyOvertime(
EmployeeId
,FiscalYearDetailId
,TotalOvertimeActual
,TotalOvertime
,TotalOvertimeBy
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,OTRate
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,TotalLateInHrs
,TotalEarlyOutHrs
)
select 
distinct EmployeeId
,@FiscalYearDetailId
,0
,sum(Overtime) TotalOvertime
,sum(OvertimeBy)TotalOvertimeBy
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,0 OTRate
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,sum(LateInHrs)TotalLateInHrs
,sum(EarlyOutHrs)TotalEarlyOutHrs
 from(
select 
d.EmployeeId
,d.OvertimeDate OvertimeDate
,d.Overtime Overtime
,d.OvertimeBy OvertimeBy
,j.ProjectId
,j.DepartmentId
,j.SectionId
,j.DesignationId
,null Remarks
,@IsActive         IsActive
,@IsArchive        IsArchive
,@CreatedBy        CreatedBy
,@CreatedAt        CreatedAt
,@CreatedFrom      CreatedFrom
,@LastUpdateBy     LastUpdateBy
,@LastUpdateAt     LastUpdateAt
,@LastUpdateFrom   LastUpdateFrom
,d.LateInHrs LateInHrs
,d.EarlyOutHrs EarlyOutHrs
 
 from EmployeeDailyOvertime d
left outer join EmployeeJob j on d.EmployeeId=j.EmployeeId
where d.OvertimeDate between @PeriodStart and @PeriodEnd ";

                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" and d.EmployeeId=@EmployeeId";
                }
                else
                {
                    sqlText += @" and d.EmployeeId not in(
 select EmployeeId from  EmployeeMonthlyOvertime
where FiscalYearDetailId=@FiscalYearDetailId
 and IsManual=1 ) ";
                }
                sqlText += @" 

) as a
group by 
 EmployeeId
, OvertimeDate
, Overtime
, OvertimeBy
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,Remarks
, IsActive
, IsArchive
,CreatedBy
,CreatedAt
, CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
 

";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);
                cmdInsert.Parameters.AddWithValue("@LastUpdateBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@LastUpdateAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@LastUpdateFrom", auditvm.CreatedFrom);
                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                //if (transResult <= 0)
                //{
                //    retResults[1] = "Unexpected error to Update Employee Monthly Overtime!";
                //    throw new ArgumentNullException(retResults[1], "");
                //}
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Employee Monthly Overtime Updated Successfully!";
                #endregion Commit
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

        public string[] EmployeeDailyAbsenceToEmployeeMonthlyAbsenceP3(string FiscalYearDetailId, ShampanIdentityVM auditvm, string EmployeeId = "0_0", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeeDailyAbsenceToEmployeeMonthlyAbsence"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region Save
                sqlText = "  ";
                sqlText += @" 
--declare @FiscalYearDetailId as varchar(20)
declare @PeriodStart as varchar(20)
declare @PeriodEnd as varchar(20)
--set @FiscalYearDetailId='1005'
select @PeriodStart=PeriodStart, @PeriodEnd=PeriodEnd from FiscalYearDetail
where id=@FiscalYearDetailId
delete EmployeeMonthlyAbsence
where FiscalYearDetailId=@FiscalYearDetailId";
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" and EmployeeId=@EmployeeId";
                }
                else
                {
                    sqlText += @" and ISNULL(IsManual,0)=0";
                }

                sqlText += @" insert into EmployeeMonthlyAbsence(
EmployeeId
,FiscalYearDetailId
,AbsentDays
,LateInDays
,EarlyOutDays

,ProjectId
,DepartmentId
,SectionId
,DesignationId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
)
select 
distinct a.EmployeeId
,@FiscalYearDetailId
,a.AbsentDays  
,a.LateInDays
,a.EarlyOutDays

,j.ProjectId
,j.DepartmentId
,j.SectionId
,j.DesignationId
,'' Remarks
,@IsActive         IsActive
,@IsArchive        IsArchive
,@CreatedBy        CreatedBy
,@CreatedAt        CreatedAt
,@CreatedFrom      CreatedFrom
,@LastUpdateBy     LastUpdateBy
,@LastUpdateAt     LastUpdateAt
,@LastUpdateFrom   LastUpdateFrom
 from(
select 
d.EmployeeId
,sum(case WHEN isnull(d.TransactionType,0) = 'Absence' and DayStatus='R' THEN 1 else 0 End ) AbsentDays   
,sum(case WHEN isnull(d.TransactionType,0) = 'LateIn' THEN 1 else 0 End )  LateInDays 
,sum(case WHEN isnull(d.TransactionType,0) = 'EarlyOut' THEN 1 else 0 End )EarlyOutDays  
 from EmployeeDailyAbsence d
 where d.AbsentDate between @PeriodStart and @PeriodEnd
";
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" and d.EmployeeId=@EmployeeId";
                }
                else
                {
                    sqlText += @" and d.EmployeeId not in(
 select EmployeeId from EmployeeMonthlyAbsence
where FiscalYearDetailId=@FiscalYearDetailId
 and IsManual=1 ) ";
                }
                sqlText += @" 
 group by d.EmployeeId
 )as a
left outer join EmployeeJob j on a.EmployeeId=j.EmployeeId

";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);
                cmdInsert.Parameters.AddWithValue("@LastUpdateBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@LastUpdateAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@LastUpdateFrom", auditvm.CreatedFrom);
                if (EmployeeId != "0_0" && !string.IsNullOrWhiteSpace(EmployeeId))
                {
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                //if (transResult <= 0)
                //{
                //    retResults[1] = "Unexpected error to Update Employee Monthly Overtime!";
                //    throw new ArgumentNullException(retResults[1], "");
                //}
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Employee Monthly Overtime Updated Successfully!";
                #endregion Commit
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


        //Calendar Process
        public string[] CalendarProcess(string FiscalYearDetailId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "CalendarProcess"; //Method Name
            int transResult = 0;
            string sqlText = "";
            #endregion
            #region NewVariables
            AttendanceMigrationVM amVM = new AttendanceMigrationVM();
            List<AttendanceMigrationVM> amVMs = new List<AttendanceMigrationVM>();
            string newFromDate = "";
            string newToDate = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction
                #region Fiscal Year
                string PeriodEnd = "";
                string PeriodStart = "";
                sqlText = @"select * from FiscalYearDetail
                            where id=@FiscalYearDetailId";
                SqlCommand cmdfy = new SqlCommand(sqlText, currConn, transaction);
                cmdfy.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                using (SqlDataReader dr = cmdfy.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        PeriodEnd = dr["PeriodEnd"].ToString();
                        PeriodStart = dr["PeriodStart"].ToString();
                    }
                    dr.Close();
                }

                #endregion Fiscal Year
                #region Select New,Resigned and NewJoin&NewResigned Employee
                #region Select New Employee

                sqlText = " ";
                sqlText += @"
--NewJoin
select EmployeeId,@PeriodStart FromDate,JoinDate ToDate from EmployeeJob -- (JoinDate/ToDate-1)
where 1=1 
and IsActive=1 and  JoinDate between @PeriodStart and @PeriodEnd

";
                {
                    SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                    objComm.Parameters.AddWithValue("@PeriodStart", PeriodStart);
                    objComm.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
                    newFromDate = "";
                    newToDate = "";

                    SqlDataReader dr;
                    dr = objComm.ExecuteReader();
                    while (dr.Read())
                    {
                        amVM = new AttendanceMigrationVM();
                        amVM.EmployeeId = dr["EmployeeId"].ToString();
                        newFromDate = dr["FromDate"].ToString();
                        newToDate = dr["ToDate"].ToString();
                        if (newToDate == PeriodStart) //If the Employee Join in PeriodStart Nothing to Delete
                        {
                            continue;
                        }
                        amVM.AttendanceDateFrom = Ordinary.StringToDate(newFromDate);
                        amVM.AttendanceDateTo = Convert.ToDateTime(Ordinary.StringToDate(newToDate)).AddDays(-1).ToString();

                        amVMs.Add(amVM);
                    }
                    dr.Close();
                }
                #endregion
                #region Select Resigned Employee

                sqlText = " ";
                sqlText += @"
--Resign
select EmployeeId,LeftDate FromDate,@PeriodEnd ToDate  from EmployeeJob --(FromDate/LeftDate+1)
where 1=1 
and IsActive=0 and  LeftDate between @PeriodStart and @PeriodEnd

";
                {
                    SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                    objComm.Parameters.AddWithValue("@PeriodStart", PeriodStart);
                    objComm.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
                    newFromDate = "";
                    newToDate = "";
                    SqlDataReader dr;
                    dr = objComm.ExecuteReader();
                    while (dr.Read())
                    {
                        amVM = new AttendanceMigrationVM();
                        amVM.EmployeeId = dr["EmployeeId"].ToString();
                        newFromDate = dr["FromDate"].ToString();
                        newToDate = dr["ToDate"].ToString();
                        if (newFromDate == PeriodEnd) //If the Employee Left in PeriodEnd Nothing to Delete
                        {
                            continue;
                        }

                        amVM.AttendanceDateFrom = Convert.ToDateTime(Ordinary.StringToDate(newFromDate)).AddDays(1).ToString();
                        amVM.AttendanceDateTo = newToDate;

                        amVMs.Add(amVM);
                    }
                    dr.Close();
                }
                #endregion
                #region Select New Join and New Resigned Employee

                sqlText = " ";
                sqlText += @"

--NewJoin&NewResign
select EmployeeId,@PeriodStart FromDate,JoinDate ToDate from EmployeeJob -- (JoinDate/ToDate)-1
where 1=1 
and IsActive=0 and  JoinDate between @PeriodStart and @PeriodEnd and LeftDate between @PeriodStart and @PeriodEnd

";
                {
                    SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                    objComm.Parameters.AddWithValue("@PeriodStart", PeriodStart);
                    objComm.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
                    newFromDate = "";
                    newToDate = "";

                    SqlDataReader dr;
                    dr = objComm.ExecuteReader();
                    while (dr.Read())
                    {
                        amVM = new AttendanceMigrationVM();
                        amVM.EmployeeId = dr["EmployeeId"].ToString();
                        newFromDate = dr["FromDate"].ToString();
                        newToDate = dr["ToDate"].ToString();

                        if (newToDate == PeriodStart) //If the Employee Join in PeriodStart Nothing to Delete
                        {
                            continue;
                        }

                        amVM.AttendanceDateTo = Ordinary.StringToDate(newToDate);
                        amVM.AttendanceDateTo = Convert.ToDateTime(Ordinary.StringToDate(newToDate)).AddDays(-1).ToString();
                        amVMs.Add(amVM);
                    }
                    dr.Close();
                }
                #endregion
                #endregion
                #region Delete AttnMigration
                foreach (AttendanceMigrationVM item in amVMs)
                {
                    sqlText = " ";
                    sqlText = @"
delete from AttendanceMigration
where 1=1 
and ISNULL(IsManual,0) = 0
and DailyDate >= @AttendanceDateFrom and DailyDate <= @AttendanceDateTo
and EmployeeId = @EmployeeId";


                    sqlText = " ";
                    sqlText += @"
delete from AttendanceDailyNew
where 1=1
and ISNULL(IsManual,0) = 0
and DailyDate >= @AttendanceDateFrom and DailyDate <= @AttendanceDateTo
and EmployeeId = @EmployeeId
";

                    SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                    cmdDelete.Parameters.AddWithValue("@EmployeeId", item.EmployeeId);
                    cmdDelete.Parameters.AddWithValue("@AttendanceDateFrom", Ordinary.DateToString(item.AttendanceDateFrom));
                    cmdDelete.Parameters.AddWithValue("@AttendanceDateTo", Ordinary.DateToString(item.AttendanceDateTo));

                    var execResult = cmdDelete.ExecuteNonQuery();
                    transResult = Convert.ToInt32(execResult);

                }

                #endregion

                #region Update
                #region Update Attendance AttnStatus For Leave

                sqlText = "  ";
                sqlText += @" 
--declare @FiscalYearDetailId as varchar(20)
--declare @PeriodStart as varchar(20)
--declare @PeriodEnd as varchar(20)
--set @FiscalYearDetailId='1005'
--select @PeriodStart=PeriodStart, @PeriodEnd=PeriodEnd from FiscalYearDetail
--where id=@FiscalYearDetailId


update AttendanceDailyNew
set AttnStatus = 'Leave'
From
(
select eld.EmployeeId, eld.LeaveDate from EmployeeLeaveDetail eld
where 1=1
and eld.IsReject = 0 ";



                sqlText += @" 
) as Leave
where 1=1
AND AttendanceDailyNew.EmployeeId = Leave.EmployeeId
AND AttendanceDailyNew.DailyDate = Leave.LeaveDate
AND AttendanceDailyNew.DailyDate>=@PeriodStart
AND AttendanceDailyNew.DailyDate<=@PeriodEnd
";
                #endregion
                #region Update Attendance DayStatus For Hoilday
                sqlText += @" 
update AttendanceDailyNew
set DayStatus = Holiday.DayStatus
From
(
select Holiday HolidayDate, Left(HoliDayType,1)+'H' as DayStatus
from HoliDay
where 1=1
AND Holiday>=@PeriodStart
AND Holiday<=@PeriodEnd
";

                sqlText += @"
) as Holiday
where 1=1
AND AttendanceDailyNew.DailyDate = Holiday.HolidayDate";
                #endregion
                #region Update Attendance DayStatus For EmployeeWeeklyHoliday
                sqlText += @"
--declare @DailyDate as nvarchar(14)

update AttendanceDailyNew set DayStatus='WH'
from EmployeeWeeklyHoliday
where 1=1
and EmployeeWeeklyHoliday.dailyDate=AttendanceDailyNew.DailyDate
and AttendanceDailyNew.EmployeeId=EmployeeWeeklyHoliday.EmployeeId
AND EmployeeWeeklyHoliday.DailyDate>=@PeriodStart
AND EmployeeWeeklyHoliday.DailyDate<=@PeriodEnd
and EmployeeWeeklyHoliday.IsActive = 1
";
                #endregion

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@PeriodStart", PeriodStart);
                cmdUpdate.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);

                var exec = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);
                #endregion
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Calendar Process Done Successfully!";
                #endregion Commit
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

        //ApprovedLeaveRejectProcess
        public string[] ApprovedLeaveRejectProcess(AttendanceDailyNewVM vm, string[] conFields = null, string[] conValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "CalendarProcess"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                #region Save
                sqlText = "  ";
                sqlText += @" 
--declare @DateFrom as nvarchar(14)
--declare @DateTo as nvarchar(14)
--declare @EmployeeId  as nvarchar(10)
--
--set @DateFrom = 20180301
--set @DateTo = 20180305
--set @EmployeeId  = '1_99'

update AttendanceDailyNew
set AttnStatus = 'Absent'
From
(
select eld.EmployeeId, eld.LeaveDate from EmployeeLeaveDetail eld
where 1=1
AND eld.LeaveDate >= @DateFrom AND eld.LeaveDate <= @DateTo
AND eld.EmployeeId = @EmployeeId

) as Leave
where 1=1
AND AttendanceDailyNew.EmployeeId = Leave.EmployeeId
AND AttendanceDailyNew.DailyDate = Leave.LeaveDate";



                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                cmdUpdate.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                cmdUpdate.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));
                cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                var exec = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);


                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Attendance Daily Updated Successfully!";
                #endregion Commit
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


        public DataTable MonthlyAttendanceDownload(EmployeeInfoVM vm)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable Dt = new DataTable();
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
pd.OtherId [G4S ID],
  ei.EmpName, 
  ei.Designation, 
  ei.Department[Division], 
  ei.Project[Department],
  ei.EmploymentStatus,
  --//ei.CostCenter,
  ei.EmploymentType, 
  ei.JoinDate,ej.EmpCategory,
   DATEDIFF(MONTH, ei.JoinDate, GETDATE()) AS [Services Length in Months],
   DAY(EOMONTH(@dtpFrom))-sum(ISNULL(EL.LEAVE, 0))[Days Present],
  pd.DateOfBirth,
  SUM(CASE WHEN ES.LEAVETYPE_E = 'Annual Leave' THEN ISNULL(EL.LEAVE, 0)ELSE 0 END) AS [ANNUAL LEAVE TAKEN],

  SUM(CASE WHEN ES.LEAVETYPE_E = 'Casual Leave' THEN ISNULL(EL.LEAVE, 0)ELSE 0 END) AS [CASUAL LEAVE TAKEN],

  
  SUM(CASE WHEN ES.LEAVETYPE_E = 'Sick Leave' THEN ISNULL(EL.LEAVE, 0)ELSE 0 END) AS [SICK LEAVE TAKEN],

  SUM(CASE WHEN ES.LEAVETYPE_E = 'Maternity Leave' THEN ISNULL(EL.LEAVE, 0)ELSE 0 END) AS [MATERNITY LEAVE TAKEN],

  SUM(CASE WHEN ES.LEAVETYPE_E = 'Compensatory Leave' THEN ISNULL(EL.LEAVE, 0)ELSE 0 END) AS [COMPENSATORY LEAVE TAKEN],

  SUM(CASE WHEN ES.LEAVETYPE_E = 'Without Pay Leave' THEN ISNULL(EL.LEAVE, 0)ELSE 0 END) AS [WITHOUT PAY LEAVE TAKEN],
  sum(ISNULL(EL.LEAVE, 0)) as [TOTAL LEAVE],
  DAY(EOMONTH(@dtpFrom))[Total salary days]

  
FROM 
  ViewEmployeeInformation ei 
  LEFT OUTER JOIN EMPLOYEELEAVESTRUCTURE ES ON ES.EMPLOYEEID = ei.id
  LEFT OUTER JOIN (
    SELECT EMPLOYEELEAVESTRUCTUREID, SUM(TOTALLEAVE) LEAVE 
    FROM EMPLOYEELEAVE 
    WHERE IsApprove = 1  ";

                #region Parrameters Apply


                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    sqlText += "  and FromDate>=@dtpFrom";
                }
                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    sqlText += "  and ToDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(vm.CodeF))
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (!string.IsNullOrWhiteSpace(vm.CodeT))
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
              
                #endregion Parrameters Apply

                sqlText += @"  GROUP BY EMPLOYEELEAVESTRUCTUREID
  ) EL ON EL.EMPLOYEELEAVESTRUCTUREID = ES.ID
  LEFT OUTER JOIN EmployeePersonalDetail pd ON pd.employeeId = ei.Id
 LEFT OUTER JOIN EmployeeJob ej on ej.EmployeeId=ei.EmployeeId
WHERE ei.IsArchive = 0 AND ei.IsActive = 1";
                if (vm.EmpCategory!=null)
                {
                    sqlText += "  and ej.EmpCategory=@EmpCategory";
                }

                sqlText += @"
GROUP BY 
  ei.EmpName, 
  pd.OtherId, 
  ei.Designation, 
  ei.Department, 
  ei.Project,
  ei.EmploymentStatus,
--//  ei.CostCenter, 
  ei.EmploymentType, 
  ei.JoinDate,
    pd.DateOfBirth,ej.EmpCategory ";
                sqlText += @"
order by pd.OtherId ";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

                #region Parrameters set
                if (!string.IsNullOrWhiteSpace(vm.CodeF))
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeF", vm.CodeF);
                }
                if (!string.IsNullOrWhiteSpace(vm.CodeT))
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", vm.CodeT);
                }

                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    da.SelectCommand.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(vm.DateFrom));
                }
                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    da.SelectCommand.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(vm.DateTo));
                }
                if (!string.IsNullOrWhiteSpace(vm.EmpCategory))
                {
                    da.SelectCommand.Parameters.AddWithValue("@EmpCategory",(vm.EmpCategory));
                }
             
                #endregion Parrameters Apply

                da.Fill(Dt);

                Dt = Ordinary.DtColumnStringToDate(Dt, "JoinDate");






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
            return Dt;
        }

        public DataTable LateReason()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable Dt = new DataTable();
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

                sqlText = @"Select * from LateReson ";
               
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);   
                da.Fill(Dt);


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
            return Dt;
        }
        #endregion

    }

}
