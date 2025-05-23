﻿using SymOrdinary;
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
    public class AttendanceStructureDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        #region Methods
        //==================SelectAll=================
        public List<AttendanceStructureVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AttendanceStructureVM> VMs = new List<AttendanceStructureVM>();
            AttendanceStructureVM vm;
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
                #region sqlText
                sqlText = @"
SELECT
Id
,Name
,InTime
,InGrace
,isnull(OutGrace,0)OutGrace
,InTimeStart
,InTimeEnd
,OutTime
,LunchTime
,LunchBreak
,WorkingHour
,OTTime
,IsTiff
,TiffinSTime
,IsTiffNextDay
,DeductTiffTime
,IsIfter
,IfterSTime
,IsIfterNextDay
,DeductIfterTime
,DinnerSTime
,IsDinNextDay
,DeductDinTime
,IsDeductEarlyOut
,EarlyOutMin
,IsDeductLateIn
,LateInMin
,Is_OTRoundUp
,OTRoundUpMin
,IsNull(WorkingMin, 0)WorkingMin
,IsNull(TiffinMin, 0) TiffinMin
,IsNull(DinnerMin, 0) DinnerMin
,IsNull(IfterMin, 0)  IfterMin
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,IsInOT
,LateInAllowDays
,LateInAbsentDays
,EarlyOutAllowDays
,EarlyOutAbsentDays
    From AttendanceStructure
Where IsArchive=0
";
                #endregion sqlText
                //WorkingMin
                //TiffinMin
                //DinnerMin
                //IfterMin
                #region Sql Execution
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AttendanceStructureVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    vm.InTime = Ordinary.StringToTime(dr["InTime"].ToString());
                    vm.OutTime = Ordinary.StringToTime(dr["OutTime"].ToString());
                    vm.InGrace = Convert.ToInt32(dr["InGrace"]);
                    vm.OutGrace = Convert.ToInt32(dr["OutGrace"]);
                    vm.InTimeEnd = Ordinary.StringToTime(dr["InTimeEnd"].ToString());
                    vm.LunchTime = Ordinary.StringToTime(dr["LunchTime"].ToString());
                    vm.LunchBreak = Convert.ToInt32(dr["LunchBreak"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.InTimeStart = Ordinary.StringToTime(dr["InTimeStart"].ToString());
                    vm.WorkingHour = Convert.ToDecimal(dr["WorkingHour"].ToString());
                    vm.OTTime = Convert.ToInt32(dr["OTTime"].ToString());
                    vm.IsTiff = Convert.ToBoolean(dr["IsTiff"].ToString());
                    vm.TiffinSTime = Ordinary.StringToTime(dr["TiffinSTime"].ToString());
                    vm.IsTiffNextDay = Convert.ToBoolean(dr["IsTiffNextDay"].ToString());
                    vm.DeductTiffTime = Convert.ToInt32(dr["DeductTiffTime"].ToString());
                    vm.IsIfter = Convert.ToBoolean(dr["IsIfter"].ToString());
                    vm.IfterSTime = Ordinary.StringToTime(dr["IfterSTime"].ToString());
                    vm.IsIfterNextDay = Convert.ToBoolean(dr["IsIfterNextDay"].ToString());
                    vm.DeductIfterTime = Convert.ToInt32(dr["DeductIfterTime"].ToString());
                    vm.DinnerSTime = Ordinary.StringToTime(dr["DinnerSTime"].ToString());
                    vm.IsDinNextDay = Convert.ToBoolean(dr["IsDinNextDay"].ToString());
                    vm.DeductDinTime = Convert.ToInt32(dr["DeductDinTime"].ToString());
                    vm.IsDeductEarlyOut = Convert.ToBoolean(dr["IsDeductEarlyOut"].ToString());
                    vm.EarlyOutMin = Convert.ToInt32(dr["EarlyOutMin"].ToString());
                    vm.IsDeductLateIn = Convert.ToBoolean(dr["IsDeductLateIn"].ToString());
                    vm.LateInMin = Convert.ToInt32(dr["LateInMin"].ToString());
                    vm.IsOTRoundUp = Convert.ToBoolean(dr["Is_OTRoundUp"].ToString());
                    vm.OTRoundUpMin = Convert.ToInt32(dr["OTRoundUpMin"].ToString());
                    vm.IsInOT = Convert.ToBoolean(dr["IsInOT"].ToString());
                    vm.LateInAllowDays = Convert.ToInt32(dr["LateInAllowDays"].ToString());
                    vm.LateInAbsentDays = Convert.ToInt32(dr["LateInAbsentDays"].ToString());
                    vm.EarlyOutAllowDays = Convert.ToInt32(dr["EarlyOutAllowDays"].ToString());
                    vm.EarlyOutAbsentDays = Convert.ToInt32(dr["EarlyOutAbsentDays"].ToString());
                    vm.WorkingMin = Convert.ToInt32(dr["WorkingMin"].ToString());
                    vm.TiffinMin = Convert.ToInt32(dr["TiffinMin"].ToString());
                    vm.DinnerMin = Convert.ToInt32(dr["DinnerMin"].ToString());
                    vm.IfterMin = Convert.ToInt32(dr["IfterMin"].ToString());
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion Sql Execution
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
        public AttendanceStructureVM SelectById(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            AttendanceStructureVM vm = new AttendanceStructureVM();
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
Id
,Name
,InTime
,InGrace
,isnull(OutGrace,0)OutGrace
,InTimeStart
,InTimeEnd
,OutTime
,LunchTime
,LunchBreak
,WorkingHour
,OTTime
,IsTiff
,TiffinSTime
,IsTiffNextDay
,DeductTiffTime
,IsIfter
,IfterSTime
,IsIfterNextDay
,DeductIfterTime
,DinnerSTime
,IsDinNextDay
,DeductDinTime
,IsDeductEarlyOut
,EarlyOutMin
,IsDeductLateIn
,LateInMin
,Is_OTRoundUp
,OTRoundUpMin
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,IsInOT
,IsNull(WorkingMin, 0)  WorkingMin
,IsNull(TiffinMin, 0)   TiffinMin
,IsNull(DinnerMin, 0)   DinnerMin
,IsNull(IfterMin, 0)   IfterMin
,LateInAllowDays
,LateInAbsentDays
,EarlyOutAllowDays
,EarlyOutAbsentDays
    From AttendanceStructure
where  id=@Id and IsArchive=0
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    vm.InTime = Ordinary.StringToTime(dr["InTime"].ToString());
                    vm.OutTime = Ordinary.StringToTime(dr["OutTime"].ToString());
                    vm.InGrace = Convert.ToInt32(dr["InGrace"]);
                    vm.OutGrace = Convert.ToInt32(dr["OutGrace"]);
                    vm.InTimeEnd = Ordinary.StringToTime(dr["InTimeEnd"].ToString());
                    vm.LunchTime = Ordinary.StringToTime(dr["LunchTime"].ToString());
                    vm.LunchBreak = Convert.ToInt32(dr["LunchBreak"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.InTimeStart = Ordinary.StringToTime(dr["InTimeStart"].ToString());
                    vm.WorkingHour = Convert.ToDecimal(dr["WorkingHour"].ToString());
                    vm.OTTime = Convert.ToInt32(dr["OTTime"].ToString());
                    vm.IsTiff = Convert.ToBoolean(dr["IsTiff"].ToString());
                    vm.TiffinSTime = Ordinary.StringToTime(dr["TiffinSTime"].ToString());
                    vm.IsTiffNextDay = Convert.ToBoolean(dr["IsTiffNextDay"].ToString());
                    vm.DeductTiffTime = Convert.ToInt32(dr["DeductTiffTime"].ToString());
                    vm.IsIfter = Convert.ToBoolean(dr["IsIfter"].ToString());
                    vm.IfterSTime = Ordinary.StringToTime(dr["IfterSTime"].ToString());
                    vm.IsIfterNextDay = Convert.ToBoolean(dr["IsIfterNextDay"].ToString());
                    vm.DeductIfterTime = Convert.ToInt32(dr["DeductIfterTime"].ToString());
                    vm.DinnerSTime = Ordinary.StringToTime(dr["DinnerSTime"].ToString());
                    vm.IsDinNextDay = Convert.ToBoolean(dr["IsDinNextDay"].ToString());
                    vm.DeductDinTime = Convert.ToInt32(dr["DeductDinTime"].ToString());
                    vm.IsDeductEarlyOut = Convert.ToBoolean(dr["IsDeductEarlyOut"].ToString());
                    vm.EarlyOutMin = Convert.ToInt32(dr["EarlyOutMin"].ToString());
                    vm.IsDeductLateIn = Convert.ToBoolean(dr["IsDeductLateIn"].ToString());
                    vm.LateInMin = Convert.ToInt32(dr["LateInMin"].ToString());
                    vm.IsOTRoundUp = Convert.ToBoolean(dr["Is_OTRoundUp"].ToString());
                    vm.OTRoundUpMin = Convert.ToInt32(dr["OTRoundUpMin"].ToString());
                    vm.IsInOT = Convert.ToBoolean(dr["IsInOT"].ToString());
                    vm.LateInAllowDays = Convert.ToInt32(dr["LateInAllowDays"].ToString());
                    vm.LateInAbsentDays = Convert.ToInt32(dr["LateInAbsentDays"].ToString());
                    vm.EarlyOutAllowDays = Convert.ToInt32(dr["EarlyOutAllowDays"].ToString());
                    vm.EarlyOutAbsentDays = Convert.ToInt32(dr["EarlyOutAbsentDays"].ToString());
                    vm.WorkingMin = Convert.ToInt32(dr["WorkingMin"].ToString());
                    vm.TiffinMin = Convert.ToInt32(dr["TiffinMin"].ToString());
                    vm.DinnerMin = Convert.ToInt32(dr["DinnerMin"].ToString());
                    vm.IfterMin = Convert.ToInt32(dr["IfterMin"].ToString());
                }
                dr.Close();
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
            return vm;
        }
        public List<AttendanceStructureVM> SelectAll(SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            AttendanceStructureVM vm = new AttendanceStructureVM();
            List<AttendanceStructureVM> VMs = new List<AttendanceStructureVM>();
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
Id
,Name
,InTime
,InGrace
,isnull(OutGrace,0)OutGrace
,InTimeStart
,InTimeEnd
,OutTime
,LunchTime
,LunchBreak
,WorkingHour
,OTTime
,IsTiff
,TiffinSTime
,IsTiffNextDay
,DeductTiffTime
,IsIfter
,IfterSTime
,IsIfterNextDay
,DeductIfterTime
,DinnerSTime
,IsDinNextDay
,DeductDinTime
,IsDeductEarlyOut
,EarlyOutMin
,IsDeductLateIn
,LateInMin
,Is_OTRoundUp
,OTRoundUpMin
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,IsInOT
,LateInAllowDays
,LateInAbsentDays
,EarlyOutAllowDays
,EarlyOutAbsentDays
,IsNull(WorkingMin, 0)  WorkingMin
,IsNull(TiffinMin , 0)  TiffinMin
,IsNull(DinnerMin , 0)  DinnerMin
,IsNull(IfterMin  , 0)  IfterMin
    From AttendanceStructure
where  1=1 and IsActive=1
";
                //WorkingMin
                //TiffinMin
                //DinnerMin
                //IfterMin
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.Transaction = transaction;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    VMs.Add(vm);
                }
                dr.Close();
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
            }
            #region catch
            catch (SqlException sqlex)
            {
                return VMs;
            }
            catch (Exception ex)
            {
                return VMs;
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
        public AttendanceStructureVM SelectByEmployee(string employeeId, string PunchDate = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            AttendanceStructureVM vm = new AttendanceStructureVM();
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
SELECT atts.*
From EmployeeInfo ei
left outer join EmployeeStructureGroup esg on esg.EmployeeId=ei.Id
left outer join AttendanceRosterDetail ard on ard.AttendanceGroupId=esg.EmployeeGroupId
left outer join AttendanceStructure atts on atts.Id=ard.AttendanceStructureId
where ei.id=@employeeid 
";
                if (!string.IsNullOrWhiteSpace(PunchDate) && PunchDate != null)
                {
                    sqlText += @" and ard.ToDate=@PunchDate";
                }
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@employeeid", employeeId);
                if (!string.IsNullOrWhiteSpace(PunchDate) && PunchDate != null)
                {
                    objComm.Parameters.AddWithValue("@PunchDate", Ordinary.DateToString(PunchDate));
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();

                string Id = "";

                while (dr.Read())
                {
                    Id = "";
                    Id = dr["Id"].ToString();

                    if (string.IsNullOrWhiteSpace(Id))
                    {
                        continue;
                    }

                    vm.Id = Convert.ToInt32(Id);
                    vm.Name = dr["Name"].ToString();
                    vm.InTime = Ordinary.StringToTime(dr["InTime"].ToString());
                    vm.OutTime = Ordinary.StringToTime(dr["OutTime"].ToString());
                    vm.InGrace = Convert.ToInt32(dr["InGrace"]);
                    vm.OutGrace = Convert.ToInt32(dr["OutGrace"]);
                    vm.InTimeEnd = Ordinary.StringToTime(dr["InTimeEnd"].ToString());
                    vm.LunchTime = Ordinary.StringToTime(dr["LunchTime"].ToString());
                    vm.LunchBreak = Convert.ToInt32(dr["LunchBreak"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.InTimeStart = Ordinary.StringToTime(dr["InTimeStart"].ToString());
                    vm.WorkingHour = Convert.ToDecimal(dr["WorkingHour"].ToString());
                    vm.OTTime = Convert.ToInt32(dr["OTTime"].ToString());
                    vm.IsTiff = Convert.ToBoolean(dr["IsTiff"].ToString());
                    vm.TiffinSTime = Ordinary.StringToTime(dr["TiffinSTime"].ToString());
                    vm.IsTiffNextDay = Convert.ToBoolean(dr["IsTiffNextDay"].ToString());
                    vm.DeductTiffTime = Convert.ToInt32(dr["DeductTiffTime"].ToString());
                    vm.IsIfter = Convert.ToBoolean(dr["IsIfter"].ToString());
                    vm.IfterSTime = Ordinary.StringToTime(dr["IfterSTime"].ToString());
                    vm.IsIfterNextDay = Convert.ToBoolean(dr["IsIfterNextDay"].ToString());
                    vm.DeductIfterTime = Convert.ToInt32(dr["DeductIfterTime"].ToString());
                    vm.DinnerSTime = Ordinary.StringToTime(dr["DinnerSTime"].ToString());
                    vm.IsDinNextDay = Convert.ToBoolean(dr["IsDinNextDay"].ToString());
                    vm.DeductDinTime = Convert.ToInt32(dr["DeductDinTime"].ToString());
                    vm.IsDeductEarlyOut = Convert.ToBoolean(dr["IsDeductEarlyOut"].ToString());
                    vm.EarlyOutMin = Convert.ToInt32(dr["EarlyOutMin"].ToString());
                    vm.IsDeductLateIn = Convert.ToBoolean(dr["IsDeductLateIn"].ToString());
                    vm.LateInMin = Convert.ToInt32(dr["LateInMin"].ToString());
                    vm.IsOTRoundUp = Convert.ToBoolean(dr["Is_OTRoundUp"].ToString());
                    vm.OTRoundUpMin = Convert.ToInt32(dr["OTRoundUpMin"].ToString());
                    vm.IsInOT = Convert.ToBoolean(dr["IsInOT"].ToString());
                    vm.LateInAllowDays = Convert.ToInt32(dr["LateInAllowDays"].ToString());
                    vm.LateInAbsentDays = Convert.ToInt32(dr["LateInAbsentDays"].ToString());
                    vm.EarlyOutAllowDays = Convert.ToInt32(dr["EarlyOutAllowDays"].ToString());
                    vm.EarlyOutAbsentDays = Convert.ToInt32(dr["EarlyOutAbsentDays"].ToString());
                    vm.WorkingMin = Convert.ToInt32(dr["WorkingMin"].ToString());
                    vm.TiffinMin = Convert.ToInt32(dr["TiffinMin"].ToString());
                    vm.DinnerMin = Convert.ToInt32(dr["DinnerMin"].ToString());
                    vm.IfterMin = Convert.ToInt32(dr["IfterMin"].ToString());
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
        //==================Insert =================
        public string[] Insert(AttendanceStructureVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "AttendanceStructure";
                string[] fieldName = { "Name" };
                string[] fieldValue = { vm.Name.Trim() };
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist
                #region Save
                sqlText = @"Select IsNull(isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0),0) from AttendanceStructure 
                                where 
                                Name=@Name And
                                InTime=@InTime And
                                OutTime=@OutTime And
                                InGrace=@InGrace And
                                OutGrace=@OutGrace And
                                InTimeEnd=@InTimeEnd
                            ";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@Name", vm.Name);
                cmd2.Parameters.AddWithValue("@InTime", Ordinary.TimeToString(vm.InTime));
                cmd2.Parameters.AddWithValue("@OutTime", Ordinary.TimeToString(vm.OutTime));
                cmd2.Parameters.AddWithValue("@InGrace", vm.InGrace);
                cmd2.Parameters.AddWithValue("@OutGrace", vm.OutGrace);
                cmd2.Parameters.AddWithValue("@InTimeEnd", Ordinary.TimeToString(vm.InTimeEnd));
                cmd2.Transaction = transaction;
                int count = (int)cmd2.ExecuteScalar();
                //int foundId = (int)objfoundId;
                if (count == 0)
                {
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO AttendanceStructure(
Name
,InTime
,InGrace
,OutGrace
,InTimeStart
,InTimeEnd
,OutTime
,LunchTime
,LunchBreak
,WorkingHour
,OTTime
,IsTiff
,TiffinSTime
,IsTiffNextDay
,DeductTiffTime
,IsIfter
,IfterSTime
,IsIfterNextDay
,DeductIfterTime
,DinnerSTime
,IsDinNextDay
,DeductDinTime
,IsDeductEarlyOut
,EarlyOutMin
,IsDeductLateIn
,LateInMin
,Is_OTRoundUp
,OTRoundUpMin
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,IsInOT
,LateInAllowDays
,LateInAbsentDays
,EarlyOutAllowDays
,EarlyOutAbsentDays
,WorkingMin
,TiffinMin
,DinnerMin
,IfterMin
) 
                                VALUES (
 @Name
,@InTime
,@InGrace
,@OutGrace
,@InTimeStart
,@InTimeEnd
,@OutTime
,@LunchTime
,@LunchBreak
,@WorkingHour
,@OTTime
,@IsTiff
,@TiffinSTime
,@IsTiffNextDay
,@DeductTiffTime
,@IsIfter
,@IfterSTime
,@IsIfterNextDay
,@DeductIfterTime
,@DinnerSTime
,@IsDinNextDay
,@DeductDinTime
,@IsDeductEarlyOut
,@EarlyOutMin
,@IsDeductLateIn
,@LateInMin
,@Is_OTRoundUp
,@OTRoundUpMin
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@IsInOT
,@LateInAllowDays
,@LateInAbsentDays
,@EarlyOutAllowDays
,@EarlyOutAbsentDays
,@WorkingMin
,@TiffinMin
,@DinnerMin
,@IfterMin
) 
";

                    //WorkingMin
                    //TiffinMin
                    //DinnerMin
                    //IfterMin

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                    cmdInsert.Parameters.AddWithValue("@InTime", Ordinary.TimeToString(vm.InTime));
                    cmdInsert.Parameters.AddWithValue("@OutTime", Ordinary.TimeToString(vm.OutTime));
                    cmdInsert.Parameters.AddWithValue("@InGrace", vm.InGrace);
                    cmdInsert.Parameters.AddWithValue("@OutGrace", vm.OutGrace);
                    cmdInsert.Parameters.AddWithValue("@InTimeEnd", Ordinary.TimeToString(vm.InTimeEnd));
                    cmdInsert.Parameters.AddWithValue("@LunchTime", Ordinary.TimeToString(vm.LunchTime));
                    cmdInsert.Parameters.AddWithValue("@LunchBreak", vm.LunchBreak);//, attendanceStructureVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, attendanceStructureVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@InTimeStart", Ordinary.TimeToString(vm.InTimeStart));
                    cmdInsert.Parameters.AddWithValue("@WorkingHour", vm.WorkingHour);
                    cmdInsert.Parameters.AddWithValue("@OTTime", vm.OTTime);
                    cmdInsert.Parameters.AddWithValue("@IsTiff", vm.IsTiff);
                    cmdInsert.Parameters.AddWithValue("@TiffinSTime", Ordinary.TimeToString(vm.TiffinSTime));
                    cmdInsert.Parameters.AddWithValue("@IsTiffNextDay", vm.IsTiffNextDay);
                    cmdInsert.Parameters.AddWithValue("@DeductTiffTime", vm.DeductTiffTime);
                    cmdInsert.Parameters.AddWithValue("@IsIfter", vm.IsIfter);
                    cmdInsert.Parameters.AddWithValue("@IfterSTime", Ordinary.TimeToString(vm.IfterSTime));
                    cmdInsert.Parameters.AddWithValue("@IsIfterNextDay", vm.IsIfterNextDay);
                    cmdInsert.Parameters.AddWithValue("@DeductIfterTime", vm.DeductIfterTime);
                    cmdInsert.Parameters.AddWithValue("@DinnerSTime", Ordinary.TimeToString(vm.DinnerSTime));
                    cmdInsert.Parameters.AddWithValue("@IsDinNextDay", vm.IsDinNextDay);
                    cmdInsert.Parameters.AddWithValue("@DeductDinTime", vm.DeductDinTime);
                    cmdInsert.Parameters.AddWithValue("@IsDeductEarlyOut", vm.IsDeductEarlyOut);
                    cmdInsert.Parameters.AddWithValue("@EarlyOutMin", 0);
                    cmdInsert.Parameters.AddWithValue("@IsDeductLateIn", vm.IsDeductLateIn);
                    cmdInsert.Parameters.AddWithValue("@LateInMin", 0);
                    cmdInsert.Parameters.AddWithValue("@Is_OTRoundUp", vm.IsOTRoundUp);
                    cmdInsert.Parameters.AddWithValue("@OTRoundUpMin", vm.OTRoundUpMin);
                    cmdInsert.Parameters.AddWithValue("@IsInOT", vm.IsInOT);
                    cmdInsert.Parameters.AddWithValue("@LateInAllowDays", 0);
                    cmdInsert.Parameters.AddWithValue("@LateInAbsentDays", 0);
                    cmdInsert.Parameters.AddWithValue("@EarlyOutAllowDays", 0);
                    cmdInsert.Parameters.AddWithValue("@EarlyOutAbsentDays", 0);

                    #region Minute Calculation
                    vm.WorkingMin = Convert.ToInt32(vm.WorkingHour * 60);

                    if (vm.IsTiffNextDay)
                    {
                        TimeSpan diff1 = Convert.ToDateTime(Ordinary.StringToTime("2359")) - Convert.ToDateTime(vm.InTime);
                        double m1 = diff1.TotalMinutes;
                        TimeSpan diff2 = Convert.ToDateTime(vm.TiffinSTime) - Convert.ToDateTime(Ordinary.StringToTime("0001"));
                        double m2 = diff2.TotalMinutes;
                        vm.TiffinMin = Convert.ToInt32(m1 + m2 + 2);
                    }
                    else
                    {
                        TimeSpan diff1 = Convert.ToDateTime(vm.TiffinSTime) - Convert.ToDateTime(vm.InTime);
                        double m1 = diff1.TotalMinutes;
                        vm.TiffinMin = Convert.ToInt32(m1);
                    }

                    if (vm.IsDinNextDay)
                    {
                        TimeSpan diff1 = Convert.ToDateTime(Ordinary.StringToTime("2359")) - Convert.ToDateTime(vm.InTime);
                        double m1 = diff1.TotalMinutes;
                        TimeSpan diff2 = Convert.ToDateTime(vm.DinnerSTime) - Convert.ToDateTime(Ordinary.StringToTime("0001"));
                        double m2 = diff2.TotalMinutes;
                        vm.DinnerMin = Convert.ToInt32(m1 + m2 + 2);
                    }
                    else
                    {
                        TimeSpan diff1 = Convert.ToDateTime(vm.DinnerSTime) - Convert.ToDateTime(vm.InTime);
                        double m1 = diff1.TotalMinutes;
                        vm.DinnerMin = Convert.ToInt32(m1);
                    }

                    if (vm.IsIfterNextDay)
                    {
                        TimeSpan diff1 = Convert.ToDateTime(Ordinary.StringToTime("2359")) - Convert.ToDateTime(vm.InTime);
                        double m1 = diff1.TotalMinutes;
                        TimeSpan diff2 = Convert.ToDateTime(vm.IfterSTime) - Convert.ToDateTime(Ordinary.StringToTime("0001"));
                        double m2 = diff2.TotalMinutes;
                        vm.DinnerMin = Convert.ToInt32(m1 + m2 + 2);
                    }
                    else
                    {
                        TimeSpan diff1 = Convert.ToDateTime(vm.IfterSTime) - Convert.ToDateTime(vm.InTime);
                        double m1 = diff1.TotalMinutes;
                        vm.DinnerMin = Convert.ToInt32(m1);
                    }


                    vm.WorkingMin = (vm.WorkingMin == 0 ? 0000 : vm.WorkingMin);
                    vm.TiffinMin = (vm.TiffinMin == 0 ? 0000 : vm.TiffinMin);
                    vm.DinnerMin = (vm.DinnerMin == 0 ? 0000 : vm.DinnerMin);
                    vm.IfterMin = (vm.IfterMin == 0 ? 0000 : vm.IfterMin);

                    #endregion Minute Calculation


                    cmdInsert.Parameters.AddWithValue("@WorkingMin", vm.WorkingMin);
                    cmdInsert.Parameters.AddWithValue("@TiffinMin", vm.TiffinMin);
                    cmdInsert.Parameters.AddWithValue("@DinnerMin", vm.DinnerMin);
                    cmdInsert.Parameters.AddWithValue("@IfterMin", vm.IfterMin);
                    cmdInsert.ExecuteNonQuery();
                    //if (Id <= 0)
                    //{
                    //    retResults[1] = "Please Input Bank Value";
                    //    retResults[3] = sqlText;
                    //    throw new ArgumentNullException("Please Input Bank Value", "");
                    //}
                }
                else
                {
                    retResults[1] = "This Time Policy already used!";
                    throw new ArgumentNullException("Please Input Time Policy Value", "");
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
        //==================Update =================
        public string[] Update(AttendanceStructureVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "AttendanceStructure";
                string[] fieldName = { "Name" };
                string[] fieldValue = { vm.Name.Trim() };
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInUpdate(vm.Id.ToString(), tableName, fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist
                sqlText = @"Select IsNull(isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0),0) from AttendanceStructure 
                                where 
                                Name=@Name And
                                InTime=@InTime And
                                OutTime=@OutTime And
                                InGrace=@InGrace And
                                OutGrace=@OutGrace And
                                InTimeEnd=@InTimeEnd And
                                 Id<>@Id
                            ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@Name", vm.Name);
                cmdExist.Parameters.AddWithValue("@InTime", Ordinary.TimeToString(vm.InTime));
                cmdExist.Parameters.AddWithValue("@OutTime", Ordinary.TimeToString(vm.OutTime));
                cmdExist.Parameters.AddWithValue("@InGrace", vm.InGrace);
                cmdExist.Parameters.AddWithValue("@OutGrace", vm.OutGrace);
                cmdExist.Parameters.AddWithValue("@InTimeEnd", Ordinary.TimeToString(vm.InTimeEnd));
                cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                cmdExist.Transaction = transaction;
                int count = (int)cmdExist.ExecuteScalar();
                if (count == 0)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update AttendanceStructure set";
                    sqlText += " Name=@Name";
                    sqlText += " ,InTime=@InTime";
                    sqlText += " ,InGrace=@InGrace";
                    sqlText += " ,OutGrace=@OutGrace";
                    sqlText += " ,InTimeStart=@InTimeStart";
                    sqlText += " ,InTimeEnd=@InTimeEnd";
                    sqlText += " ,OutTime=@OutTime";
                    sqlText += " ,LunchTime=@LunchTime";
                    sqlText += " ,LunchBreak=@LunchBreak";
                    sqlText += " ,WorkingHour=@WorkingHour";
                    sqlText += " ,OTTime=@OTTime";
                    sqlText += " ,IsTiff=@IsTiff";
                    sqlText += " ,TiffinSTime=@TiffinSTime";
                    sqlText += " ,IsTiffNextDay=@IsTiffNextDay";
                    sqlText += " ,DeductTiffTime=@DeductTiffTime";
                    sqlText += " ,IsIfter=@IsIfter";
                    sqlText += " ,IfterSTime=@IfterSTime";
                    sqlText += " ,IsIfterNextDay=@IsIfterNextDay";
                    sqlText += " ,DeductIfterTime=@DeductIfterTime";
                    sqlText += " ,DinnerSTime=@DinnerSTime";
                    sqlText += " ,IsDinNextDay=@IsDinNextDay";
                    sqlText += " ,DeductDinTime=@DeductDinTime";
                    sqlText += " ,IsDeductEarlyOut=@IsDeductEarlyOut";
                    sqlText += " ,EarlyOutMin=@EarlyOutMin";
                    sqlText += " ,IsDeductLateIn=@IsDeductLateIn";
                    sqlText += " ,LateInMin=@LateInMin";
                    sqlText += " ,Is_OTRoundUp=@Is_OTRoundUp";
                    sqlText += " ,OTRoundUpMin=@OTRoundUpMin";
                    sqlText += " ,Remarks=@Remarks";
                    sqlText += " ,IsActive=@IsActive";
                    sqlText += " ,IsArchive=@IsArchive";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " ,IsInOT=@IsInOT";
                    sqlText += " ,LateInAllowDays=@LateInAllowDays";
                    sqlText += " ,LateInAbsentDays=@LateInAbsentDays";
                    sqlText += " ,EarlyOutAllowDays=@EarlyOutAllowDays";
                    sqlText += " ,EarlyOutAbsentDays=@EarlyOutAbsentDays";
                    sqlText += " ,WorkingMin=@WorkingMin";
                    sqlText += " ,TiffinMin=@TiffinMin";
                    sqlText += " ,DinnerMin=@DinnerMin";
                    sqlText += " ,IfterMin=@IfterMin";
                    sqlText += " where Id=@Id";


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                    cmdInsert.Parameters.AddWithValue("@InTime", Ordinary.TimeToString(vm.InTime));
                    cmdInsert.Parameters.AddWithValue("@OutTime", Ordinary.TimeToString(vm.OutTime));
                    cmdInsert.Parameters.AddWithValue("@InGrace", vm.InGrace);
                    cmdInsert.Parameters.AddWithValue("@OutGrace", vm.OutGrace);
                    cmdInsert.Parameters.AddWithValue("@InTimeEnd", Ordinary.TimeToString(vm.InTimeEnd));
                    cmdInsert.Parameters.AddWithValue("@LunchTime", Ordinary.TimeToString(vm.LunchTime));
                    cmdInsert.Parameters.AddWithValue("@LunchBreak", vm.LunchBreak);//, attendanceStructureVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, attendanceStructureVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdInsert.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdInsert.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdInsert.Parameters.AddWithValue("@InTimeStart", Ordinary.TimeToString(vm.InTimeStart));
                    cmdInsert.Parameters.AddWithValue("@WorkingHour", vm.WorkingHour);
                    cmdInsert.Parameters.AddWithValue("@OTTime", vm.OTTime);
                    cmdInsert.Parameters.AddWithValue("@IsTiff", vm.IsTiff);
                    cmdInsert.Parameters.AddWithValue("@TiffinSTime", Ordinary.TimeToString(vm.TiffinSTime));
                    cmdInsert.Parameters.AddWithValue("@IsTiffNextDay", vm.IsTiffNextDay);
                    cmdInsert.Parameters.AddWithValue("@DeductTiffTime", vm.DeductTiffTime);
                    cmdInsert.Parameters.AddWithValue("@IsIfter", vm.IsIfter);
                    cmdInsert.Parameters.AddWithValue("@IfterSTime", Ordinary.TimeToString(vm.IfterSTime));
                    cmdInsert.Parameters.AddWithValue("@IsIfterNextDay", vm.IsIfterNextDay);
                    cmdInsert.Parameters.AddWithValue("@DeductIfterTime", vm.DeductIfterTime);
                    cmdInsert.Parameters.AddWithValue("@DinnerSTime", Ordinary.TimeToString(vm.DinnerSTime));
                    cmdInsert.Parameters.AddWithValue("@IsDinNextDay", vm.IsDinNextDay);
                    cmdInsert.Parameters.AddWithValue("@DeductDinTime", vm.DeductDinTime);
                    cmdInsert.Parameters.AddWithValue("@IsDeductEarlyOut", vm.IsDeductEarlyOut);
                    cmdInsert.Parameters.AddWithValue("@EarlyOutMin", 0);
                    cmdInsert.Parameters.AddWithValue("@IsDeductLateIn", vm.IsDeductLateIn);
                    cmdInsert.Parameters.AddWithValue("@LateInMin", 0);
                    cmdInsert.Parameters.AddWithValue("@Is_OTRoundUp", vm.IsOTRoundUp);
                    cmdInsert.Parameters.AddWithValue("@OTRoundUpMin", vm.OTRoundUpMin);
                    cmdInsert.Parameters.AddWithValue("@IsInOT", vm.IsInOT);
                    cmdInsert.Parameters.AddWithValue("@LateInAllowDays", 0);
                    cmdInsert.Parameters.AddWithValue("@LateInAbsentDays", 0);
                    cmdInsert.Parameters.AddWithValue("@EarlyOutAllowDays", 0);
                    cmdInsert.Parameters.AddWithValue("@EarlyOutAbsentDays", 0);

                    #region Minute Calculation
                    vm.WorkingMin = Convert.ToInt32(vm.WorkingHour * 60);

                    if (vm.IsTiffNextDay)
                    {
                        TimeSpan diff1 = Convert.ToDateTime(Ordinary.StringToTime("2359")) - Convert.ToDateTime(vm.InTime);
                        double m1 = diff1.TotalMinutes;
                        TimeSpan diff2 = Convert.ToDateTime(vm.TiffinSTime) - Convert.ToDateTime(Ordinary.StringToTime("0001"));
                        double m2 = diff2.TotalMinutes;
                        vm.TiffinMin = Convert.ToInt32(m1 + m2 + 2);
                    }
                    else
                    {
                        TimeSpan diff1 = Convert.ToDateTime(vm.TiffinSTime) - Convert.ToDateTime(vm.InTime);
                        double m1 = diff1.TotalMinutes;
                        vm.TiffinMin = Convert.ToInt32(m1);
                    }

                    if (vm.IsDinNextDay)
                    {
                        TimeSpan diff1 = Convert.ToDateTime(Ordinary.StringToTime("2359")) - Convert.ToDateTime(vm.InTime);
                        double m1 = diff1.TotalMinutes;
                        TimeSpan diff2 = Convert.ToDateTime(vm.DinnerSTime) - Convert.ToDateTime(Ordinary.StringToTime("0001"));
                        double m2 = diff2.TotalMinutes;
                        vm.DinnerMin = Convert.ToInt32(m1 + m2 + 2);
                    }
                    else
                    {
                        TimeSpan diff1 = Convert.ToDateTime(vm.DinnerSTime) - Convert.ToDateTime(vm.InTime);
                        double m1 = diff1.TotalMinutes;
                        vm.DinnerMin = Convert.ToInt32(m1);
                    }

                    if (vm.IsIfterNextDay)
                    {
                        TimeSpan diff1 = Convert.ToDateTime(Ordinary.StringToTime("2359")) - Convert.ToDateTime(vm.InTime);
                        double m1 = diff1.TotalMinutes;
                        TimeSpan diff2 = Convert.ToDateTime(vm.IfterSTime) - Convert.ToDateTime(Ordinary.StringToTime("0001"));
                        double m2 = diff2.TotalMinutes;
                        vm.DinnerMin = Convert.ToInt32(m1 + m2 + 2);
                    }
                    else
                    {
                        TimeSpan diff1 = Convert.ToDateTime(vm.IfterSTime) - Convert.ToDateTime(vm.InTime);
                        double m1 = diff1.TotalMinutes;
                        vm.DinnerMin = Convert.ToInt32(m1);
                    }

                    vm.WorkingMin = (vm.WorkingMin == 0 ? 0000 : vm.WorkingMin);
                    vm.TiffinMin = (vm.TiffinMin == 0 ? 0000 : vm.TiffinMin);
                    vm.DinnerMin = (vm.DinnerMin == 0 ? 0000 : vm.DinnerMin);
                    vm.IfterMin = (vm.IfterMin == 0 ? 0000 : vm.IfterMin);

                    #endregion Minute Calculation
                    cmdInsert.Parameters.AddWithValue("@WorkingMin", vm.WorkingMin);
                    cmdInsert.Parameters.AddWithValue("@TiffinMin", vm.TiffinMin);
                    cmdInsert.Parameters.AddWithValue("@DinnerMin", vm.DinnerMin);
                    cmdInsert.Parameters.AddWithValue("@IfterMin", vm.IfterMin);
                    transResult = (int)cmdInsert.ExecuteNonQuery();
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", AttendanceStructureVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    retResults[1] = "This Time Policy already used!";
                    throw new ArgumentNullException("Please Input Time Policy Value", "");
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
                    retResults[1] = "Unexpected error to update Time Policy.";
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
        public string[] Delete(AttendanceStructureVM vm, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete Attendance Structure"; //Method Name
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
                  CommonDAL _cDal = new CommonDAL();
                    bool existCheck = false;
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                           #region ExistCheck in AttendanceRosterDetail

                    existCheck = _cDal.ExistCheck("AttendanceRosterDetail","AttendanceStructureId",ids[i], currConn, transaction);
                    if (existCheck)
                    {
                        retResults[1] = "First Delete This Policy From Attendance Roster!";
                        throw new ArgumentNullException(retResults[1],"");
                    }

                    #endregion ExistCheck in AttendanceRosterDetail


                        sqlText = "";
                        sqlText = "update AttendanceStructure set";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
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
                        throw new ArgumentNullException("Time Policy Delete", vm.Id + " could not Delete.");
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
                    retResults[1] = "Unexpected error to delete Time Policy Information.";
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
        public List<AttendanceStructureVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AttendanceStructureVM> VMs = new List<AttendanceStructureVM>();
            AttendanceStructureVM vm;
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
   FROM AttendanceStructure
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
                    vm = new AttendanceStructureVM();
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
            //==================Delete =================
        }
        #endregion
    }
}
