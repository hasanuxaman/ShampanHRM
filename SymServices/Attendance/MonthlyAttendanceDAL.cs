using MSScriptControl;
using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymServices.Leave;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
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
    public class MonthlyAttendanceDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        #region Methods
        public List<MonthlyAttendanceVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
          , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<MonthlyAttendanceVM> VMs = new List<MonthlyAttendanceVM>();
            MonthlyAttendanceVM vm;
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
ma.Id

,ve.Code
,ve.EmpName
,ve.Project
,ve.Department
,ve.Section
,ve.Designation

,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd

,ma.EmployeeId
,ma.FiscalYearDetailId
,ma.ProjectId
,ma.DepartmentId
,ma.SectionId
,ma.DesignationId


,ISNULL(ma.GrossSalary                ,0)GrossSalary
,ISNULL(ma.BasicSalary                ,0)BasicSalary
,ISNULL(ma.DOM                        ,0)DOM
,ISNULL(ma.HoliDay                    ,0)HoliDay


,ma.HoliDayDetails


,ISNULL(ma.OffDay                     ,0)OffDay
,ISNULL(ma.WorkingDay                 ,0)WorkingDay
,ISNULL(ma.NPDay                      ,0)NPDay
,ISNULL(ma.NPAmount                   ,0)NPAmount
,ISNULL(ma.LWPDay                     ,0)LWPDay
,ISNULL(ma.LWPAmount                  ,0)LWPAmount
,ISNULL(ma.AbsentDay                  ,0)AbsentDay
,ISNULL(ma.AbsentAmount               ,0)AbsentAmount
,ISNULL(ma.TotalLeave                 ,0)TotalLeave


,ma.LeaveDetail



,ISNULL(ma.PresentDay                ,0)PresentDay
,ISNULL(ma.LateDay                   ,0)LateDay
,ISNULL(ma.LateAmount                ,0)LateAmount
,ISNULL(ma.AttnBonus                 ,0)AttnBonus
,ISNULL(ma.OTAllow                   ,0)OTAllow
,ISNULL(ma.OTAllowBY                 ,0)OTAllowBY
,ISNULL(ma.OTAllowExtra              ,0)OTAllowExtra
,ISNULL(ma.OTRate                    ,0)OTRate
,ISNULL(ma.TotalOTHrs                ,0)TotalOTHrs
,ISNULL(ma.TotalOTHrsBY              ,0)TotalOTHrsBY
,ISNULL(ma.TotalOTHrsExtra           ,0)TotalOTHrsExtra
,ISNULL(ma.OTAmount                  ,0)OTAmount
,ISNULL(ma.OTAmountBY                ,0)OTAmountBY
,ISNULL(ma.OTAmountExtra             ,0)OTAmountExtra


,ma.EmploymentType

,ISNULL(ma.LateAbsentDay               ,0)LateAbsentDay
,ISNULL(ma.LateAbsentHour              ,0)LateAbsentHour
,ISNULL(ma.EarlyOutDayCount            ,0)EarlyOutDayCount
,ISNULL(ma.EarlyOutHourCount           ,0)EarlyOutHourCount
,ISNULL(ma.EarlyOutDeductAmount        ,0)EarlyOutDeductAmount
,ISNULL(ma.LateInDayCount              ,0)LateInDayCount
,ISNULL(ma.LateInHourCount             ,0)LateInHourCount
,ISNULL(ma.LateInDeductAmount          ,0)LateInDeductAmount
,ISNULL(ma.OtherDeductionDay           ,0)OtherDeductionDay


,ma.Remarks
,ma.IsActive
,ma.IsArchive
,ma.CreatedBy
,ma.CreatedAt
,ma.CreatedFrom
,ma.LastUpdateBy
,ma.LastUpdateAt
,ma.LastUpdateFrom

FROM MonthlyAttendance ma
LEFT OUTER JOIN ViewEmployeeInformation ve ON ma.EmployeeId = ve.EmployeeId
LEFT OUTER JOIN FiscalYearDetail fyd ON ma.FiscalYearDetailId = fyd.Id
WHERE  1=1 AND ma.IsArchive = 0
and ve.EmployeeId <> '1_0'
";

                if (Id > 0)
                {
                    sqlText += @" and Id=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();

                while (dr.Read())
                {
                    vm = new MonthlyAttendanceVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);

                    vm.EmpCode = Convert.ToString(dr["Code"]);
                    vm.EmpName = Convert.ToString(dr["EmpName"]);
                    vm.Project = Convert.ToString(dr["Project"]);
                    vm.Department = Convert.ToString(dr["Department"]);
                    vm.Section = Convert.ToString(dr["Section"]);
                    vm.Designation = Convert.ToString(dr["Designation"]);
                    vm.PeriodName = Convert.ToString(dr["PeriodName"]);
                    vm.PeriodStart = Convert.ToString(dr["PeriodStart"]);
                    vm.PeriodEnd = Convert.ToString(dr["PeriodEnd"]);


                    vm.EmployeeId = Convert.ToString(dr["EmployeeId"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.ProjectId = Convert.ToString(dr["ProjectId"]);
                    vm.DepartmentId = Convert.ToString(dr["DepartmentId"]);
                    vm.SectionId = Convert.ToString(dr["SectionId"]);
                    vm.DesignationId = Convert.ToString(dr["DesignationId"]);


                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.DOM = Convert.ToInt32(dr["DOM"]);
                    vm.HoliDay = Convert.ToInt32(dr["HoliDay"]);



                    vm.HoliDayDetails = Convert.ToString(dr["HoliDayDetails"]);



                    vm.OffDay = Convert.ToInt32(dr["OffDay"]);
                    vm.WorkingDay = Convert.ToInt32(dr["WorkingDay"]);
                    vm.NPDay = Convert.ToInt32(dr["NPDay"]);
                    vm.NPAmount = Convert.ToDecimal(dr["NPAmount"]);
                    vm.LWPDay = Convert.ToDecimal(dr["LWPDay"]);
                    vm.LWPAmount = Convert.ToDecimal(dr["LWPAmount"]);
                    vm.AbsentDay = Convert.ToDecimal(dr["AbsentDay"]);
                    vm.AbsentAmount = Convert.ToDecimal(dr["AbsentAmount"]);
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);


                    vm.LeaveDetail = Convert.ToString(dr["LeaveDetail"]);


                    vm.PresentDay = Convert.ToInt32(dr["PresentDay"]);
                    vm.LateDay = Convert.ToInt32(dr["LateDay"]);
                    vm.LateAmount = Convert.ToDecimal(dr["LateAmount"]);
                    vm.AttnBonus = Convert.ToDecimal(dr["AttnBonus"]);
                    vm.OTAllow = Convert.ToBoolean(dr["OTAllow"]);
                    vm.OTAllowBY = Convert.ToBoolean(dr["OTAllowBY"]);
                    vm.OTAllowExtra = Convert.ToBoolean(dr["OTAllowExtra"]);
                    vm.OTRate = Convert.ToDecimal(dr["OTRate"]);
                    vm.TotalOTHrs = Convert.ToDecimal(dr["TotalOTHrs"]);
                    vm.TotalOTHrsBY = Convert.ToDecimal(dr["TotalOTHrsBY"]);
                    vm.TotalOTHrsExtra = Convert.ToDecimal(dr["TotalOTHrsExtra"]);
                    vm.OTAmount = Convert.ToDecimal(dr["OTAmount"]);
                    vm.OTAmountBY = Convert.ToDecimal(dr["OTAmountBY"]);
                    vm.OTAmountExtra = Convert.ToDecimal(dr["OTAmountExtra"]);

                    vm.EmploymentType = Convert.ToString(dr["EmploymentType"]);



                    vm.LateAbsentDay = Convert.ToDecimal(dr["LateAbsentDay"]);
                    vm.LateAbsentHour = Convert.ToDecimal(dr["LateAbsentHour"]);
                    vm.EarlyOutDayCount = Convert.ToDecimal(dr["EarlyOutDayCount"]);
                    vm.EarlyOutHourCount = Convert.ToDecimal(dr["EarlyOutHourCount"]);
                    vm.EarlyOutDeductAmount = Convert.ToDecimal(dr["EarlyOutDeductAmount"]);
                    vm.LateInDayCount = Convert.ToDecimal(dr["LateInDayCount"]);
                    vm.LateInHourCount = Convert.ToDecimal(dr["LateInHourCount"]);
                    vm.LateInDeductAmount = Convert.ToDecimal(dr["LateInDeductAmount"]);
                    vm.OtherDeductionDay = Convert.ToDecimal(dr["OtherDeductionDay"]);



                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }

        //==================Update =================
        public string[] Update(List<MonthlyAttendanceVM> VMs, MonthlyAttendanceVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee GLGLCustomer Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToGLCustomer"); }
                #endregion open connection and transaction
                if (VMs != null && VMs.Count() > 0)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE MonthlyAttendance SET";

                    sqlText += "   Remarks=@Remarks";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    foreach (MonthlyAttendanceVM vm in VMs)
                    {
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                        cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", paramVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", paramVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", paramVM.LastUpdateFrom);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }



                    retResults[2] = VMs.FirstOrDefault().Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", GLCustomerVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("GLGLCustomer Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update GLGLCustomer.";
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

        //==================Insert =================



        public string[] Insert(List<MonthlyAttendanceVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Monthly Attendance"; //Method Name
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
                int SearchFid = VMs.FirstOrDefault().FiscalYearDetailId;
                #region SqlText
                sqlText = "  ";
                sqlText += @" INSERT INTO dbo.MonthlyAttendance
(
Id
,EmployeeId
,FiscalYearDetailId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,GrossSalary
,BasicSalary
,DOM
,HoliDay
,HoliDayDetails
,OffDay
,WorkingDay
,NPDay
,NPAmount
,LWPDay
,LWPAmount
,AbsentDay
,AbsentAmount
,TotalLeave
,LeaveDetail
,PresentDay
,LateDay
,LateAmount
,AttnBonus
,OTAllow
,OTAllowBY
,OTAllowExtra
,OTRate
,TotalOTHrs
,TotalOTHrsBY
,TotalOTHrsExtra
,OTAmount
,OTAmountBY
,OTAmountExtra

,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
)
 VALUES 
(
@Id
,@EmployeeId
,@FiscalYearDetailId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@GrossSalary
,@BasicSalary
,@DOM
,@HoliDay
,@HoliDayDetails
,@OffDay
,@WorkingDay
,@NPDay
,@NPAmount
,@LWPDay
,@LWPAmount
,@AbsentDay
,@AbsentAmount
,@TotalLeave
,@LeaveDetail
,@PresentDay
,@LateDay
,@LateAmount
,@AttnBonus
,@OTAllow
,@OTAllowBY
,@OTAllowExtra
,@OTRate
,@TotalOTHrs
,@TotalOTHrsBY
,@TotalOTHrsExtra
,@OTAmount
,@OTAmountBY
,@OTAmountExtra

,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)";
                #endregion SqlText

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                if (VMs.Count >= 1)
                {
                    #region Insert Settings
                    foreach (var item in VMs)
                    {
                        #region CheckPoint
                        bool isExist = false;
                        //if this employee in this month alredy exist then delete first then insert
                        string[] conditionFields = { "EmployeeId", "FiscalYearDetailId" };
                        string[] conditionValues = { item.EmployeeId, SearchFid.ToString() };
                        isExist = _cDal.ExistCheck("MonthlyAttendance", conditionFields, conditionValues, currConn, transaction);
                        if (isExist)
                        {
                            retResults = _cDal.DeleteTable("MonthlyAttendance", conditionFields, conditionValues, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                retResults[1] = "Update Fail!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }

                        #endregion CheckPoint

                        item.Id = _cDal.NextId("MonthlyAttendance", currConn, transaction);

                        #region SqlExecution
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                        cmdInsert.Parameters.AddWithValue("@Id", item.Id);
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", item.EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", item.FiscalYearDetailId);
                        cmdInsert.Parameters.AddWithValue("@ProjectId", item.ProjectId ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@DepartmentId", item.DepartmentId ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@SectionId", item.SectionId ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@DesignationId", item.DesignationId ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@GrossSalary", item.GrossSalary);
                        cmdInsert.Parameters.AddWithValue("@BasicSalary", item.BasicSalary);
                        cmdInsert.Parameters.AddWithValue("@DOM", item.DOM);
                        cmdInsert.Parameters.AddWithValue("@HoliDay", item.HoliDay);
                        cmdInsert.Parameters.AddWithValue("@HoliDayDetails", item.HoliDayDetails ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@OffDay", item.OffDay);
                        cmdInsert.Parameters.AddWithValue("@WorkingDay", item.WorkingDay);
                        cmdInsert.Parameters.AddWithValue("@NPDay", item.NPDay);
                        cmdInsert.Parameters.AddWithValue("@NPAmount", item.NPAmount);
                        cmdInsert.Parameters.AddWithValue("@LWPDay", item.LWPDay);
                        cmdInsert.Parameters.AddWithValue("@LWPAmount", item.LWPAmount);
                        cmdInsert.Parameters.AddWithValue("@AbsentDay", item.AbsentDay);
                        cmdInsert.Parameters.AddWithValue("@AbsentAmount", item.AbsentAmount);
                        cmdInsert.Parameters.AddWithValue("@TotalLeave", item.TotalLeave);
                        cmdInsert.Parameters.AddWithValue("@LeaveDetail", item.LeaveDetail ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@PresentDay", item.PresentDay);
                        cmdInsert.Parameters.AddWithValue("@LateDay", item.LateDay);
                        cmdInsert.Parameters.AddWithValue("@LateAmount", item.LateAmount);
                        cmdInsert.Parameters.AddWithValue("@AttnBonus", item.AttnBonus);
                        cmdInsert.Parameters.AddWithValue("@OTAllow", item.OTAllow);
                        cmdInsert.Parameters.AddWithValue("@OTAllowBY", item.OTAllowBY);
                        cmdInsert.Parameters.AddWithValue("@OTAllowExtra", item.OTAllowExtra);
                        cmdInsert.Parameters.AddWithValue("@OTRate", item.OTRate);

                        cmdInsert.Parameters.AddWithValue("@TotalOTHrs", item.TotalOTHrs);
                        cmdInsert.Parameters.AddWithValue("@TotalOTHrsBY", item.TotalOTHrsBY);
                        cmdInsert.Parameters.AddWithValue("@TotalOTHrsExtra", item.TotalOTHrsExtra);
                        cmdInsert.Parameters.AddWithValue("@OTAmount", item.OTAmount);
                        cmdInsert.Parameters.AddWithValue("@OTAmountBY", item.OTAmountBY);
                        cmdInsert.Parameters.AddWithValue("@OTAmountExtra", item.OTAmountExtra);

                        cmdInsert.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, holiDayVM.Remarks);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", item.CreatedBy ?? identity.Name);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", item.CreatedAt ?? DateTime.Now.ToString("yyyyMMdd"));
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", item.CreatedFrom ?? identity.WorkStationIP);
                        var exec = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exec);
                        if (transResult <= 0)
                        {
                            retResults[1] = "Unexpected error to Monthly Attendance Insert!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion SqlExecution

                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #endregion Insert Settings
                }
                else
                {
                    retResults[1] = "Monthly Attendance - Could not found any item!";
                    throw new ArgumentNullException(retResults[1], "");
                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Monthly Attendance Saved Successfully.";
                #endregion Commit
            }
            #region Catch and Finally
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
            #endregion Catch and Finally
            return retResults;
        }

        public string[] SelectToInsert(MonthlyAttendanceVM vVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            List<MonthlyAttendanceVM> VMs = new List<MonthlyAttendanceVM>();
            MonthlyAttendanceVM vm = new MonthlyAttendanceVM();
            #endregion
            #region tempRegion
            int fid = vVM.FiscalYearDetailId;
            //string employeeCode = vVM.EmployeeCode;
            //string attendanceDate = vVM.AttendanceDate;
            //string attendanceTime = vVM.AttendanceTime;
            //string proxyId = vVM.ProxyId;
            #endregion tempRegion

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
                DataTable empDt = new DataTable();
                DataTable fPeriodDt = new DataTable();
                string periodStart = "";
                string periodEnd = "";
                int daysOfMonth = 0;
                int holiDays = 0;
                int offDays = 0;
                int workingDays = 0;
                HoliDayDAL _hDal = new HoliDayDAL();
                EmployeeLeaveDetailDAL _empLeaveDDal = new EmployeeLeaveDetailDAL();

                DataTable empMAbsenceDt = new DataTable();
                string designationId = "";
                DataTable empPromotionDt = new DataTable();
                DataTable designationDt = new DataTable();
                DataTable empMOvertimeDt = new DataTable();

                DataTable empStGroupDt = new DataTable();
                string earningDeductionStructureId = "";
                SettingDAL _setDAL = new SettingDAL();
                ScriptControl sc = new ScriptControl();
                sc.Language = "VBScript";

                EarningDeductionStructureDAL _edStDal = new EarningDeductionStructureDAL();
                EarningDeductionStructureVM edStVM = new EarningDeductionStructureVM();

                #endregion New Variables
                #region Fiscal Period
                fPeriodDt = _cDal.SelectByCondition("FiscalYearDetail", "Id", fid.ToString(), currConn, transaction);
                if (fPeriodDt == null || fPeriodDt.Rows.Count == 0)
                {
                    retResults[1] = "Fiscal Period Not Exists!";
                    throw new ArgumentNullException(retResults[1], "");
                }
                periodStart = fPeriodDt.Rows[0]["PeriodStart"].ToString();
                periodEnd = fPeriodDt.Rows[0]["PeriodEnd"].ToString();
                daysOfMonth = Convert.ToInt32(periodEnd) - Convert.ToInt32(periodStart) + 1;


                holiDays = _hDal.SelectHolidDays(fid.ToString(), currConn, transaction);
                offDays = _hDal.SelectOffDays(fid.ToString(), currConn, transaction);
                workingDays = daysOfMonth - offDays - holiDays;
                #endregion Fiscal Period



                empDt = _cDal.SelectByCondition("ViewEmployeeInformation", "", "", currConn, transaction);
                if (empDt == null || empDt.Rows.Count == 0)
                {
                    retResults[1] = "Employee Not Exists!";
                    throw new ArgumentNullException(retResults[1], "");
                }

                decimal weeklyOTRate = 0;

                #region Data Fethcing And Assign
                foreach (var item in empDt.AsEnumerable())
                {
                    vm = new MonthlyAttendanceVM();
                    #region Profile
                    vm.EmployeeId = item["EmployeeId"].ToString();
                    vm.FiscalYearDetailId = fid;
                    vm.ProjectId = item["ProjectId"].ToString();
                    vm.DepartmentId = item["DepartmentId"].ToString();
                    vm.SectionId = item["SectionId"].ToString();
                    vm.DesignationId = item["DesignationId"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(item["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(item["BasicSalary"]);
                    #endregion Profile

                    empStGroupDt = _cDal.SelectByCondition("EmployeeStructureGroup", "EmployeeId", vm.EmployeeId, currConn, transaction);
                    if (empStGroupDt != null && empStGroupDt.Rows.Count > 0)
                    {
                        earningDeductionStructureId = empStGroupDt.Rows[0]["EarningDeductionStructureId"].ToString();
                        string[] edStConFields = { "Id" };
                        string[] edStConValues = { earningDeductionStructureId };
                        edStVM = _edStDal.SelectByMultiCondition(edStConFields, edStConValues, currConn, transaction).FirstOrDefault();
                    }
                    #region Days
                    vm.DOM = daysOfMonth;
                    vm.HoliDay = holiDays;
                    vm.HoliDayDetails = "";
                    vm.OffDay = offDays;
                    vm.WorkingDay = workingDays;
                    vm.NPDay = 0;
                    vm.NPAmount = 0;
                    vm.LWPDay = _empLeaveDDal.SelectLeaveWPDays(vm.EmployeeId, fid.ToString(), currConn, transaction);

                    vm.LWPAmount = 0;

                    #endregion Days
                    #region Absence; Late
                    string[] empMAConFields = { "EmployeeId", "FiscalYearDetailId" };
                    string[] empMAConValues = { vm.EmployeeId, fid.ToString() };
                    empMAbsenceDt = _cDal.SelectByMultiCondition("EmployeeMonthlyAbsence", empMAConFields, empMAConValues, currConn, transaction);
                    //vSalary/(DOM)
                    var DayRateCalc = _setDAL.settingValue("DayRate", "DayRate", currConn, transaction);
                    DayRateCalc = DayRateCalc.Replace("DOM", vm.DOM.ToString());
                    var lwpRatecalc = DayRateCalc;
                    if (empMAbsenceDt != null && empMAbsenceDt.Rows.Count > 0)
                    {
                        vm.AbsentDay = Convert.ToInt32(empMAbsenceDt.Rows[0]["AbsentDays"]);
                    }
                    vm.AbsentAmount = 0;
                    decimal dayRate = 0;
                    dynamic DRate = 0;
                    if (edStVM != null && edStVM.Id > 0)
                    {
                        if (vm.AbsentDay > 0)
                        {
                            if (vm.AbsentDay <= edStVM.FirstSlotAbsentDays)
                            {
                                if (edStVM.FirstSlotAbsentFrom.ToLower() == "gross")
                                {
                                    DayRateCalc = DayRateCalc.Replace("vSalary", vm.GrossSalary.ToString());
                                }
                                else if (edStVM.FirstSlotAbsentFrom.ToLower() == "basic")
                                {
                                    DayRateCalc = DayRateCalc.Replace("vSalary", vm.BasicSalary.ToString());
                                }
                            }
                            else if (vm.AbsentDay > edStVM.FirstSlotAbsentDays)
                            {
                                if (edStVM.SecondSlotAbsentFrom.ToLower() == "gross")
                                {
                                    DayRateCalc = DayRateCalc.Replace("vSalary", vm.GrossSalary.ToString());
                                }
                                else if (edStVM.SecondSlotAbsentFrom.ToLower() == "basic")
                                {
                                    DayRateCalc = DayRateCalc.Replace("vSalary", vm.BasicSalary.ToString());
                                }
                            }
                            DRate = sc.Eval(DayRateCalc);
                            dayRate = Convert.ToDecimal(DRate);
                            vm.AbsentAmount = vm.AbsentDay * dayRate;
                        }
                        if (edStVM.LWPFrom.ToLower() == "gross")
                        {
                            lwpRatecalc = lwpRatecalc.Replace("vSalary", vm.GrossSalary.ToString());
                        }
                        else if (edStVM.LWPFrom.ToLower() == "basic")
                        {
                            lwpRatecalc = lwpRatecalc.Replace("vSalary", vm.BasicSalary.ToString());
                        }
                        DRate = sc.Eval(lwpRatecalc);
                        dayRate = Convert.ToDecimal(DRate);
                        vm.LWPAmount = vm.LWPDay * dayRate;
                    }


                    vm.TotalLeave = _empLeaveDDal.SelectLeaveDays(vm.EmployeeId, fid.ToString(), currConn, transaction);
                    vm.LeaveDetail = "";
                    vm.PresentDay = workingDays - vm.NPDay - vm.LWPDay - vm.AbsentDay - Convert.ToInt32(Math.Floor(vm.TotalLeave));
                    if (empMAbsenceDt != null && empMAbsenceDt.Rows.Count > 0)
                    {
                        vm.LateDay = Convert.ToInt32(empMAbsenceDt.Rows[0]["LateInDays"]) + Convert.ToInt32(empMAbsenceDt.Rows[0]["EarlyOutDays"]);
                    }
                    vm.LateAmount = 1;
                    #endregion Absence; Late
                    #region Designation
                    string[] desigConFields = { "EmployeeId", "IsCurrent" };
                    string[] desigConValues = { vm.EmployeeId, "1" };
                    empPromotionDt = _cDal.SelectByMultiCondition("EmployeePromotion", desigConFields, desigConValues, currConn, transaction);
                    if (empPromotionDt != null && empPromotionDt.Rows.Count > 0)
                    {
                        designationId = empPromotionDt.Rows[0]["DesignationId"].ToString();
                        designationDt = _cDal.SelectByCondition("Designation", "Id", designationId, currConn, transaction);
                        if (designationDt != null && designationDt.Rows.Count > 0)
                        {
                            vm.AttnBonus = Convert.ToDecimal(designationDt.Rows[0]["AttendenceBonus"]);
                            vm.OTAllow = Convert.ToBoolean(designationDt.Rows[0]["OTAlloawance"]);
                            vm.OTAllowBY = Convert.ToBoolean(designationDt.Rows[0]["OTBayer"]);
                            vm.OTAllowExtra = Convert.ToBoolean(designationDt.Rows[0]["ExtraOT"]);
                        }
                    }
                    #endregion Designation


                    #region Overtime

                    if (edStVM != null && edStVM.Id > 0)
                    {
                        weeklyOTRate = edStVM.WeeklyOTRate;
                        var HourRateCalc = _setDAL.settingValue("OTHourRate", "OTHourRate", currConn, transaction);
                        HourRateCalc = HourRateCalc.Replace("vGross", vm.GrossSalary.ToString());
                        HourRateCalc = HourRateCalc.Replace("vBasic", vm.BasicSalary.ToString());
                        HourRateCalc = HourRateCalc.Replace("DOM", vm.DOM.ToString());
                        dynamic hourR = sc.Eval(HourRateCalc);
                        decimal hourRate = Convert.ToDecimal(hourR);

                        vm.OTRate = hourRate * weeklyOTRate;
                    }




                    //vm.OTRate = 1;
                    string[] empMOTConFields = { "EmployeeId", "FiscalYearDetailId" };
                    string[] empMOTConValues = { vm.EmployeeId, fid.ToString() };
                    empMOvertimeDt = _cDal.SelectByMultiCondition("EmployeeMonthlyOvertime", empMOTConFields, empMOTConValues, currConn, transaction);
                    if (empMOvertimeDt != null && empMOvertimeDt.Rows.Count > 0)
                    {
                        vm.TotalOTHrs = Convert.ToDecimal(empMOvertimeDt.Rows[0]["TotalOvertime"]);
                        //vm.TotalOTHrsBY        = fid;
                        //vm.TotalOTHrsExtra     = fid;
                        vm.OTAmount = vm.TotalOTHrs * vm.OTRate;
                        //vm.OTAmountBY          = fid;
                        //vm.OTAmountExtra       = fid;

                    }
                    #endregion Overtime

                    vm.CreatedAt = vVM.CreatedAt;
                    vm.CreatedBy = vVM.CreatedBy;
                    vm.CreatedFrom = vVM.CreatedFrom;
                    VMs.Add(vm);
                }

                #endregion Data Fethcing And Assign

                #region Insert
                retResults = Insert(VMs, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    retResults[1] = "Unexpected Error to Insert Monthly Attendance!";
                    throw new ArgumentNullException(retResults[1], "");
                }
                #endregion Insert
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
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
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

        public string[] MonthlyAttendanceProcessRegular(MonthlyAttendanceVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Monthly Attendance"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionNoPool();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction
                //int SearchFid = VMs.FirstOrDefault().FiscalYearDetailId;
                #region SqlText
                sqlText = "  ";

                #region Beginning

                sqlText += @" 
declare @StartDate as varchar(20)  
declare @EndDate as varchar(20)  
declare @FiscalYearDetailId as varchar(20)  
declare @OffDay as varchar(20)   
declare @HoliDay as varchar(20)  
declare @CreatedBy as varchar(20)   
declare @CreatedAt as varchar(20)  
declare @CreatedFrom as varchar(20)  

declare @DOM as varchar(50)

";

                sqlText += " set @CreatedBy='" + vm.CreatedBy + "';  ";
                sqlText += " set @CreatedAt='" + vm.CreatedAt + "';  ";
                sqlText += " set @CreatedFrom='" + vm.CreatedFrom + "';  ";
                sqlText += " set @FiscalYearDetailId='" + vm.FiscalYearDetailId + "';--Jun-17  ";

                sqlText += @" 

   
select @DOM=settingValue from setting where SettingGroup='DOM' and SettingName='DOM'


select @StartDate=periodStart,@EndDate=PeriodEnd from FiscalYearDetail
where id=@FiscalYearDetailId

select @dom= case when @dom='dom' then CONVERT(int,@EndDate)-CONVERT(int,@StartDate)+1 else 30 end

select @OffDay=count(distinct holiday) from HoliDay
where HoliDayType in('Weekly') and holiday between @StartDate and @EndDate

select  @HoliDay=count(distinct holiday) from HoliDay
where HoliDayType not in('Weekly') and holiday between @StartDate and @EndDate
";
                #endregion

                #region Store and Delete Data

                sqlText += @" 

---------------------------------------------------------------------------------------------------
-------------------------------Store Existing Data into a Temp Table-------------------------------

create table #Temp
(
    EmployeeId nvarchar(50)
	, FiscalYearDetailId int
    , Remarks nvarchar(500)
)

	INSERT INTO #Temp
	SELECT EmployeeId, FiscalYearDetailId, Remarks
	FROM MonthlyAttendance where FiscalYearDetailId=@FiscalYearDetailId



---------------------------------------------------------------------------------------------------
-------------------------------DELETE Existing Data------------------------------------------------
delete from MonthlyAttendance where FiscalYearDetailId=@FiscalYearDetailId
";
                #endregion

                #region Regular Employee

                sqlText += @" 
---------------------------------------------------------------------------------------------------
-------------------------------Regular-------------------------------------------------------------
insert into MonthlyAttendance(EmploymentType,NPDay,EmployeeId,FiscalYearDetailId,Projectid,DepartmentId,sectionId,DesignationId,GrossSalary,Basicsalary,DOM--,TotalOTHrs,TotalOTHrsBY
,AbsentDay,LateDay,OTAllow,OTAllowBY,OTAllowExtra,AttnBonus,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)

select 'Regular' EmploymentType
,0
, Employeejob.EmployeeId,@FiscalYearDetailId FiscalYearDetailId,Employeejob.Projectid,Employeejob.DepartmentId,Employeejob.sectionId,Employeejob.DesignationId
,job.GrossSalary
,job.Basicsalary
,CONVERT(int, @EndDate)-CONVERT(int, @StartDate)+1 DOM
--,Isnull(emo.TotalOvertime,0)TotalOvertime,Isnull(emo.TotalOvertimeBy,0)TotalOvertimeBy
,Isnull(ema.AbsentDays,0) AbsentDays
,Isnull(ema.LateInDays,0)+Isnull(ema.EarlyOutDays,0)LateInDays
,isnull(Designation.OTOrginal,'0')OTOrginal
,isnull(Designation.OTBayer,'0')OTBayer
,isnull(Designation.ExtraOT,'0')ExtraOT
,isnull(Designation.AttendenceBonus,'0')AttendenceBonus
,1 IsActive
,0 IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom

FROM
(
SELECT 
ej.EmployeeId
,SUM(CASE WHEN essd.SalaryType = 'Gross' THEN essd.Amount ELSE 0 END) GrossSalary
,SUM(CASE WHEN essd.SalaryType = 'Basic' THEN essd.Amount ELSE 0 END) BasicSalary

FROM employeejob ej
LEFT OUTER JOIN EmployeeSalaryStructureDetail essd ON ej.EmployeeId=essd.EmployeeId AND essd.SalaryType IN ('Gross','Basic')
WHERE 1=1 
AND essd.IncrementDate <= @EndDate

GROUP BY ej.EmployeeId
) AS job
LEFT OUTER JOIN Employeejob ON Employeejob.EmployeeId = job.EmployeeId
--LEFT OUTER JOIN EmployeeMonthlyOvertime emo on emo.EmployeeId=Employeejob.EmployeeId and emo.FiscalYearDetailId=@FiscalYearDetailId
LEFT OUTER JOIN EmployeeMonthlyAbsence ema on ema.EmployeeId=Employeejob.EmployeeId and ema.FiscalYearDetailId=@FiscalYearDetailId
LEFT OUTER JOIN Designation on Employeejob.DesignationId=Designation.Id
where JoinDate <@StartDate
and ISNULL(NULLIF(NULLIF(LeftDate,''),'19000101'),'21000101') > @EndDate



------------and ISNULL(NULLIF(LeftDate,'19000101'),'21000101') > @EndDate
------------and ISNULL(LeftDate,'21000101') > @EndDate
------------and ISNULL(LeftDate,'19000101') not between @StartDate and @EndDate
------------and Employeejob.IsActive=1

";
                #endregion

                #region New Employee

                sqlText += @" 
---------------------------------------------------------------------------------------------------
-------------------------------New-----------------------------------------------------------------
insert into MonthlyAttendance(EmploymentType,NPDay,EmployeeId,FiscalYearDetailId,Projectid,DepartmentId,sectionId,DesignationId,GrossSalary,Basicsalary,DOM,TotalOTHrs,TotalOTHrsBY
,AbsentDay,LateDay,OTAllow,OTAllowBY,OTAllowExtra,AttnBonus,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)

select 'New' EmploymentType
--,JoinDate
--,CONVERT(int,@EndDate)-CONVERT(int,Employeejob.LeftDate) NPDay
,CONVERT(int,Employeejob.JoinDate)-CONVERT(int,@StartDate) NPDay
, Employeejob.EmployeeId,@FiscalYearDetailId FiscalYearDetailId,Employeejob.Projectid,Employeejob.DepartmentId,Employeejob.sectionId,Employeejob.DesignationId
-----------------,Employeejob.GrossSalary,Employeejob.Basicsalary
,job.GrossSalary
,job.Basicsalary

,CONVERT(int, @EndDate)-CONVERT(int, @StartDate)+1 DOM
,Isnull(EmployeeMonthlyOvertime.TotalOvertime,0)TotalOvertime,Isnull(EmployeeMonthlyOvertime.TotalOvertimeBy,0)TotalOvertimeBy
,Isnull(EmployeeMonthlyAbsence.AbsentDays,0) AbsentDays
,Isnull(EmployeeMonthlyAbsence.LateInDays,0)+Isnull(EmployeeMonthlyAbsence.EarlyOutDays,0)LateInDays
,isnull(Designation.OTOrginal,'0')OTOrginal
,isnull(Designation.OTBayer,'0')OTBayer
,isnull(Designation.ExtraOT,'0')ExtraOT
,isnull(Designation.AttendenceBonus,'0')AttendenceBonus
,1 IsActive
,0 IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom

FROM
 (
 SELECT 
 ej.EmployeeId
 ,SUM(CASE WHEN essd.SalaryType = 'Gross' THEN essd.Amount ELSE 0 END) GrossSalary
 ,SUM(CASE WHEN essd.SalaryType = 'Basic' THEN essd.Amount ELSE 0 END) BasicSalary

FROM employeejob ej
LEFT OUTER JOIN EmployeeSalaryStructureDetail essd ON ej.EmployeeId=essd.EmployeeId AND essd.SalaryType IN ('Gross','Basic')
WHERE 1=1 
AND essd.IncrementDate <= @EndDate

GROUP BY ej.EmployeeId
) AS job
LEFT OUTER JOIN Employeejob ON Employeejob.EmployeeId = job.EmployeeId



LEFT OUTER JOIN EmployeeMonthlyOvertime on EmployeeMonthlyOvertime.EmployeeId=Employeejob.EmployeeId 
and EmployeeMonthlyOvertime.FiscalYearDetailId=@FiscalYearDetailId
 LEFT OUTER JOIN EmployeeMonthlyAbsence on EmployeeMonthlyAbsence.EmployeeId=Employeejob.EmployeeId 
and EmployeeMonthlyAbsence.FiscalYearDetailId=@FiscalYearDetailId
LEFT OUTER JOIN Designation on Employeejob.DesignationId=Designation.Id
where 1=1
and JoinDate between @StartDate and @EndDate
------------and Employeejob.IsActive=1

";
                #endregion

                #region Left/Resigned Employee

                sqlText += @" 
---------------------------------------------------------------------------------------------------
-------------------------------Left----------------------------------------------------------------


insert into MonthlyAttendance(EmploymentType,NPDay,EmployeeId,FiscalYearDetailId,Projectid,DepartmentId,sectionId,DesignationId,GrossSalary,Basicsalary,DOM--,TotalOTHrs,TotalOTHrsBY
,AbsentDay,LateDay,OTAllow,OTAllowBY,OTAllowExtra,AttnBonus,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)

select 'Left' EmploymentType
--,JoinDate
,CONVERT(int,@EndDate)-CONVERT(int,Employeejob.LeftDate) NPDay
--,CONVERT(int,Employeejob.JoinDate)-CONVERT(int,@StartDate) NPDay
, Employeejob.EmployeeId,@FiscalYearDetailId FiscalYearDetailId,Employeejob.Projectid,Employeejob.DepartmentId,Employeejob.sectionId,Employeejob.DesignationId
--------------,Employeejob.GrossSalary,Employeejob.Basicsalary
,job.GrossSalary
,job.Basicsalary

,CONVERT(int, @EndDate)-CONVERT(int, @StartDate)+1 DOM
--,Isnull(EmployeeMonthlyOvertime.TotalOvertime,0)TotalOvertime,Isnull(EmployeeMonthlyOvertime.TotalOvertimeBy,0)TotalOvertimeBy
,Isnull(EmployeeMonthlyAbsence.AbsentDays,0) AbsentDays
,Isnull(EmployeeMonthlyAbsence.LateInDays,0)+Isnull(EmployeeMonthlyAbsence.EarlyOutDays,0)LateInDays
,isnull(Designation.OTOrginal,'0')OTOrginal
,isnull(Designation.OTBayer,'0')OTBayer
,isnull(Designation.ExtraOT,'0')ExtraOT
,isnull(Designation.AttendenceBonus,'0')AttendenceBonus
,1 IsActive
,0 IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom


FROM
 (
 SELECT 
 ej.EmployeeId
 ,SUM(CASE WHEN essd.SalaryType = 'Gross' THEN essd.Amount ELSE 0 END) GrossSalary
 ,SUM(CASE WHEN essd.SalaryType = 'Basic' THEN essd.Amount ELSE 0 END) BasicSalary

FROM employeejob ej
LEFT OUTER JOIN EmployeeSalaryStructureDetail essd ON ej.EmployeeId=essd.EmployeeId AND essd.SalaryType IN ('Gross','Basic')
WHERE 1=1 
AND essd.IncrementDate <= @EndDate

GROUP BY ej.EmployeeId
) AS job
LEFT OUTER JOIN Employeejob ON Employeejob.EmployeeId = job.EmployeeId



--LEFT OUTER JOIN EmployeeMonthlyOvertime on EmployeeMonthlyOvertime.EmployeeId=Employeejob.EmployeeId 
--and EmployeeMonthlyOvertime.FiscalYearDetailId=@FiscalYearDetailId
 LEFT OUTER JOIN EmployeeMonthlyAbsence on EmployeeMonthlyAbsence.EmployeeId=Employeejob.EmployeeId 
and EmployeeMonthlyAbsence.FiscalYearDetailId=@FiscalYearDetailId
left outer join Designation on Employeejob.DesignationId=Designation.Id
where 1=1
and Employeejob.JoinDate < @StartDate and ISNULL(LeftDate,'19000101') between @StartDate and @EndDate
------------and Employeejob.IsActive=0

";
                #endregion

                #region New and Left Employee

                sqlText += @" 
---------------------------------------------------------------------------------------------------
-------------------------------New and Left--------------------------------------------------------

insert into MonthlyAttendance(EmploymentType,NPDay,EmployeeId,FiscalYearDetailId,Projectid,DepartmentId,sectionId,DesignationId,GrossSalary,Basicsalary,DOM--,TotalOTHrs,TotalOTHrsBY
,AbsentDay,LateDay,OTAllow,OTAllowBY,OTAllowExtra,AttnBonus,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)

select 'NewLeft' EmploymentType
--,JoinDate
,(CONVERT(int,Employeejob.JoinDate)-CONVERT(int,@StartDate)) + (CONVERT(int,@EndDate)-CONVERT(int,Employeejob.LeftDate)) NPDay
, Employeejob.EmployeeId,@FiscalYearDetailId FiscalYearDetailId,Employeejob.Projectid,Employeejob.DepartmentId,Employeejob.sectionId,Employeejob.DesignationId
----------------------,Employeejob.GrossSalary,Employeejob.Basicsalary
,job.GrossSalary
,job.Basicsalary

,CONVERT(int, @EndDate)-CONVERT(int, @StartDate)+1 DOM
--,Isnull(EmployeeMonthlyOvertime.TotalOvertime,0)TotalOvertime,Isnull(EmployeeMonthlyOvertime.TotalOvertimeBy,0)TotalOvertimeBy
,Isnull(EmployeeMonthlyAbsence.AbsentDays,0) AbsentDays
,Isnull(EmployeeMonthlyAbsence.LateInDays,0)+Isnull(EmployeeMonthlyAbsence.EarlyOutDays,0)LateInDays
,isnull(Designation.OTOrginal,'0')OTOrginal
,isnull(Designation.OTBayer,'0')OTBayer
,isnull(Designation.ExtraOT,'0')ExtraOT
,isnull(Designation.AttendenceBonus,'0')AttendenceBonus
,1 IsActive
,0 IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom


FROM
 (
 SELECT 
 ej.EmployeeId
 ,SUM(CASE WHEN essd.SalaryType = 'Gross' THEN essd.Amount ELSE 0 END) GrossSalary
 ,SUM(CASE WHEN essd.SalaryType = 'Basic' THEN essd.Amount ELSE 0 END) BasicSalary

FROM employeejob ej
LEFT OUTER JOIN EmployeeSalaryStructureDetail essd ON ej.EmployeeId=essd.EmployeeId AND essd.SalaryType IN ('Gross','Basic')
WHERE 1=1 
AND essd.IncrementDate <= @EndDate

GROUP BY ej.EmployeeId
) AS job
LEFT OUTER JOIN Employeejob ON Employeejob.EmployeeId = job.EmployeeId


--LEFT OUTER JOIN EmployeeMonthlyOvertime on EmployeeMonthlyOvertime.EmployeeId=Employeejob.EmployeeId 
--and EmployeeMonthlyOvertime.FiscalYearDetailId=@FiscalYearDetailId
 LEFT OUTER JOIN EmployeeMonthlyAbsence on EmployeeMonthlyAbsence.EmployeeId=Employeejob.EmployeeId 
and EmployeeMonthlyAbsence.FiscalYearDetailId=@FiscalYearDetailId
LEFT OUTER JOIN Designation on Employeejob.DesignationId=Designation.Id
where 1=1
and Employeejob.JoinDate >= @StartDate and ISNULL(LeftDate,'19000101') between @StartDate and @EndDate
------------and Employeejob.IsActive=0


";
                #endregion

                #region Update Monthly Attendance

                sqlText += @" 
---------------------------------------------------------------------------------------------------
-------------------------------Updating------------------------------------------------------------

";
                #region Leave Update

                sqlText += @" 
-------------------------------Leave----------------------------------

update MonthlyAttendance set TotalLeave=isnull(leave.LeaveDate,0)
from (select distinct employeeId, isnull(sum(EmployeeLeaveDetail.TotalLeave),0)LeaveDate
 from EmployeeLeaveDetail  
where (leaveDate between @StartDate and @EndDate) AND (IsLWP=0)  AND (IsReject = 0)
group by employeeId) as leave
where MonthlyAttendance.employeeId=leave.employeeId
and MonthlyAttendance.FiscalYearDetailId=@FiscalYearDetailId

update MonthlyAttendance set TotalLeave=0
where isnull(TotalLeave,0)<=0
and MonthlyAttendance.FiscalYearDetailId=@FiscalYearDetailId

update MonthlyAttendance set LWPDay=isnull(leave.LeaveDate,0)
from (select distinct employeeId, isnull(sum(EmployeeLeaveDetail.TotalLeave),0)LeaveDate
 from EmployeeLeaveDetail  
where (leaveDate between @StartDate and @EndDate) and IsLWP=1  AND (IsReject = 0)
group by employeeId) as leave
where MonthlyAttendance.employeeId=leave.employeeId
and MonthlyAttendance.FiscalYearDetailId=@FiscalYearDetailId


";
                #endregion

                #region Overtime Update

                sqlText += @" 
-------------------------------OT/Overtime----------------------------------
update MonthlyAttendance set TotalOTHrs=isnull(ot.TotalOvertime,0)
from(
select distinct employeeId,TotalOvertime from EmployeeMonthlyOvertime
where FiscalYearDetailId=FiscalYearDetailId)OT
where MonthlyAttendance.employeeId=OT.employeeId
and MonthlyAttendance.FiscalYearDetailId=@FiscalYearDetailId



update MonthlyAttendance set AbsentDay=isnull(ot.AbsentDays,0)
,LateDay=isnull(ot.LateInDays,0)+isnull(ot.EarlyOutDays,0)
from(
select distinct employeeId,isnull(AbsentDays ,0)AbsentDays
,isnull(LateInDays ,0)LateInDays,isnull(EarlyOutDays ,0)EarlyOutDays
from EmployeeMonthlyAbsence
where FiscalYearDetailId=@FiscalYearDetailId)OT
where MonthlyAttendance.employeeId=OT.employeeId
and MonthlyAttendance.FiscalYearDetailId=@FiscalYearDetailId

";
                #endregion

                #region Days Update

                sqlText += @" 

-------------------------------Days----------------------------------
update MonthlyAttendance set OffDay=@OffDay,HoliDay=@HoliDay
where 1=1 
and @FiscalYearDetailId=@FiscalYearDetailId
and  employeeid in
(
select EmployeeId from employeeJob
where 1=1
and JoinDate <@StartDate
and LeftDate not between @StartDate and @EndDate
and Employeejob.IsActive=1)


update MonthlyAttendance set OffDay=OffDay.OffDay
from(
select EmployeeId,JoinDate,count(distinct holiday)OffDay
from Employeejob
  CROSS JOIN HoliDay
where 1=1
  and HoliDayType in('Weekly') and holiday between @StartDate and leftDate
and leftDate  between @StartDate and @EndDate
and Employeejob.IsActive=0
group by EmployeeId,JoinDate) as OffDay
where MonthlyAttendance.EmployeeId=OffDay.EmployeeId

update MonthlyAttendance set HoliDay=HoliDay.HoliDay
from(
select EmployeeId,JoinDate,count(distinct holiday)HoliDay
from Employeejob
  CROSS JOIN HoliDay
where 1=1
  and HoliDayType not in('Weekly') and holiday between @StartDate and leftDate
and leftDate  between @StartDate and @EndDate
and Employeejob.IsActive=0
group by EmployeeId,JoinDate) as HoliDay
where MonthlyAttendance.EmployeeId=HoliDay.EmployeeId


update MonthlyAttendance set OffDay=OffDay.OffDay
from(
select EmployeeId,JoinDate,count(distinct holiday)OffDay
from Employeejob
  CROSS JOIN HoliDay
where 1=1
  and HoliDayType in('Weekly') and holiday between JoinDate  and @EndDate
and JoinDate  between @StartDate and @EndDate
and Employeejob.IsActive=1
group by EmployeeId,JoinDate) as OffDay
where MonthlyAttendance.EmployeeId=OffDay.EmployeeId



update MonthlyAttendance set HoliDay=HoliDay.HoliDay
from(
select EmployeeId,JoinDate,count(distinct holiday)HoliDay
from Employeejob
  CROSS JOIN HoliDay
where 1=1
  and HoliDayType not in('Weekly') and holiday between JoinDate  and @EndDate
and JoinDate  between @StartDate and @EndDate
and Employeejob.IsActive=1
group by EmployeeId,JoinDate) as HoliDay
where MonthlyAttendance.EmployeeId=HoliDay.EmployeeId

update MonthlyAttendance set
 Holiday=case when isnull(Holiday,0)=0 then 0 else Holiday end
 ,OffDay=case when isnull(OffDay,0)=0 then 0 else OffDay end
 ,AbsentDay=case when isnull(AbsentDay,0)=0 then 0 else AbsentDay end
 ,AbsentAmount=case when isnull(AbsentAmount,0)=0 then 0 else AbsentAmount end
 ,LateAmount=case when isnull(LateAmount,0)=0 then 0 else LateAmount end
 ,LateDay=case when isnull(LateDay,0)=0 then 0 else LateDay end
 ,LWPAmount=case when isnull(LWPAmount,0)=0 then 0 else LWPAmount end
 ,LWPDay=case when isnull(LWPDay,0)=0 then 0 else LWPDay end
 ,TotalOTHrsExtra=case when isnull(TotalOTHrsExtra,0)=0 then 0 else TotalOTHrsExtra end
 ,TotalOTHrsBY=case when isnull(TotalOTHrsBY,0)=0 then 0 else TotalOTHrsBY end
 ,TotalOTHrs=case when isnull(TotalOTHrs,0)=0 then 0 else TotalOTHrs end

where MonthlyAttendance.FiscalYearDetailId=@FiscalYearDetailId

update MonthlyAttendance set WorkingDay=DOM-HoliDay-OffDay
, PresentDay=DOM-HoliDay-OffDay-LWPDay-AbsentDay-TotalLeave
where 1=1 
and @FiscalYearDetailId=@FiscalYearDetailId

update MonthlyAttendance set OTRate=case 
when ed.HourRateCountFrom='gross' then MonthlyAttendance.grossSalary/ed.HourRateDivisionFactor
when ed.HourRateCountFrom='basic' then MonthlyAttendance.BasicSalary/ed.HourRateDivisionFactor

 else 0 end 
 from (select EarningDeductionStructure.*,EmployeeStructureGroup.EmployeeId from EmployeeStructureGroup
 LEFT OUTER JOIN EarningDeductionStructure on EarningDeductionStructure.id=EmployeeStructureGroup.EarningDeductionStructureId)
ed 
 where ed.EmployeeId=MonthlyAttendance.EmployeeId and  FiscalYearDetailId=@FiscalYearDetailId

  

 update MonthlyAttendance set 
 OTAmount=case when OTAllow=1 then isnull(OTTimes.WeeklyOTRate,0)*isnull(TotalOTHrs,0)*isnull(OTRate,0) else 0 end
, OTAmountBY=case when OTAllowBY=1 then isnull(OTTimes.WeeklyOTRate,0)*isnull(TotalOTHrsBY,0)*isnull(OTRate,0) else 0 end
, OTAmountExtra=case when OTAllowExtra=1 then isnull(OTTimes.WeeklyOTRate,0)*isnull(TotalOTHrsExtra,0)*isnull(OTRate,0) else 0 end
, NPAmount=case when NPAbsentFrom='gross' then GrossSalary/convert(int,@Dom)* NPDay  when NPAbsentFrom='basic' then BAsicSalary/convert(int,@Dom)* NPDay    else 0 end
, LWPAmount=case when LWPFrom='gross' then GrossSalary/convert(int,@Dom)* LWPDay  when LWPFrom='basic' then BAsicSalary/convert(int,@Dom)* LWPDay    else 0 end

 
 from(
 select EarningDeductionStructure.*,EmployeeStructureGroup.employeeId
 from EmployeeStructureGroup 
LEFT OUTER JOIN EarningDeductionStructure on EarningDeductionStructure.id=EmployeeStructureGroup.EarningDeductionStructureId)OTTimes
 where FiscalYearDetailId=@FiscalYearDetailId
and MonthlyAttendance.employeeId=OTTimes.employeeId

 
";
                #endregion

                #region LateIn / Early Out

                sqlText += @" 
-------------------------------LateIn/EarlyOut----------------------------------
update MonthlyAttendance set 
LateInHourCount = convert(varchar(10), convert(int,isnull(vhrs.TotalLateInMins,0)/60)) +'.'+   RIGHT('00'+convert(varchar(10), convert(int,isnull(vhrs.TotalLateInMins,0) %60)),2)
, EarlyOutHourCount = convert(varchar(10), convert(int,isnull(vhrs.TotalEarlyOutMins,0)/60)) +'.'+   RIGHT('00'+convert(varchar(10), convert(int,isnull(vhrs.TotalEarlyOutMins,0) %60)),2)
  from(  SELECT DISTINCT FiscalYearDetailId,EmployeeId,sum( TotalLateInMins)TotalLateInMins
  ,sum(TotalEarlyOutMins)TotalEarlyOutMins
  FROM EmployeeMonthlyOvertime
  group by FiscalYearDetailId,EmployeeId) vhrs 
  where MonthlyAttendance.FiscalYearDetailId =vhrs.FiscalYearDetailId
  and MonthlyAttendance.EmployeeId =vhrs.EmployeeId
  and  MonthlyAttendance.FiscalYearDetailId=@FiscalYearDetailId



-------------------------------LateIn/EarlyOut Minutes-----------------------------
update MonthlyAttendance set 
TotalLateInMins = vhrs.TotalLateInMins
, TotalEarlyOutMins = vhrs.TotalEarlyOutMins
  from(  SELECT DISTINCT FiscalYearDetailId,EmployeeId,sum( TotalLateInMins)TotalLateInMins
  ,sum(TotalEarlyOutMins)TotalEarlyOutMins
  FROM EmployeeMonthlyOvertime
  group by FiscalYearDetailId,EmployeeId) vhrs 
  where MonthlyAttendance.FiscalYearDetailId =vhrs.FiscalYearDetailId
  and MonthlyAttendance.EmployeeId =vhrs.EmployeeId
  and  MonthlyAttendance.FiscalYearDetailId=@FiscalYearDetailId
------------------------------------------------------------------------------


update MonthlyAttendance set LateInDayCount=isnull(vday.LateInDays,0),EarlyOutDayCount=isnull(vday.EarlyOutDays,0)
  from(  SELECT DISTINCT FiscalYearDetailId,EmployeeId,sum( LateInDays)LateInDays,sum(EarlyOutDays)EarlyOutDays
  FROM EmployeeMonthlyAbsence
  group by FiscalYearDetailId,EmployeeId) vday 
  where MonthlyAttendance.FiscalYearDetailId =vday.FiscalYearDetailId
  and MonthlyAttendance.EmployeeId =vday.EmployeeId
  and  MonthlyAttendance.FiscalYearDetailId=@FiscalYearDetailId

----------------,
 update MonthlyAttendance set 
  LateInDeductAmount=isnull(deduct.LateInDeductAmount1,0)+isnull(deduct.LateInDeductAmount2,0)
 ,EarlyOutDeductAmount=isnull(deduct.EarlyOutDeductAmount1,0)+isnull(deduct.EarlyOutDeductAmount2,0)
 ,AbsentAmount=isnull(deduct.AbsentAmount,0)
 ,LWPAmount=isnull(deduct.LWPAmount,0)

 ,LateAbsentDay=isnull(deduct.LateAbsentDay,0)


FROM (
 SELECT 
  case 
when eds.IsMonthlyLateInDeduct=1 and eds.HourRateCountFrom='gross' then GrossSalary/(eds.HourRateDivisionFactor) *(TotalLateInMins/60) 
when eds.IsMonthlyLateInDeduct=1 and eds.HourRateCountFrom='basic' then BasicSalary/(eds.HourRateDivisionFactor) *(TotalLateInMins/60) 
else 0 end LateInDeductAmount1



,case when  (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when MonthlyEarlyOutCountDays=0 then 1 else MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(MonthlyLateInCountDays))   +LWPDay) <= FirstSlotAbsentDays and FirstSlotAbsentFrom='basic' then BasicSalary/@Dom *  (floor(LateInDayCount*LateInAbsentDays/(case when MonthlyLateInCountDays=0 then 1 end)))
when        (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when MonthlyEarlyOutCountDays=0 then 1 else MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(MonthlyLateInCountDays))   +LWPDay) <= FirstSlotAbsentDays and FirstSlotAbsentFrom='gross' then GrossSalary/@Dom *  (floor(LateInDayCount*LateInAbsentDays/(case when MonthlyLateInCountDays=0 then 1 end)))
when        (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when MonthlyEarlyOutCountDays=0 then 1 else MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(MonthlyLateInCountDays))   +LWPDay) > FirstSlotAbsentDays and SecondSlotAbsentFrom='basic' then BasicSalary/@Dom *  (floor(LateInDayCount*LateInAbsentDays/(case when MonthlyLateInCountDays=0 then 1 end)))
when        (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when MonthlyEarlyOutCountDays=0 then 1 else MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(MonthlyLateInCountDays))   +LWPDay) > FirstSlotAbsentDays and SecondSlotAbsentFrom='gross' then GrossSalary/@Dom *  (floor(LateInDayCount*LateInAbsentDays/(case when MonthlyLateInCountDays=0 then 1 end)))
end         LateInDeductAmount2


, floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when MonthlyEarlyOutCountDays=0 then 1 else MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when MonthlyLateInCountDays=0 then 1 else MonthlyLateInCountDays end)) as LateAbsentDay


	
, case 
when eds.IsMonthlyEarlyOutDeduct=1 and eds.HourRateCountFrom='gross'  then GrossSalary/(case when eds.HourRateDivisionFactor=0 then 1 else HourRateDivisionFactor end) *(TotalEarlyOutMins/60)   
when eds.IsMonthlyEarlyOutDeduct=1 and eds.HourRateCountFrom='basic'  then BasicSalary/(case when eds.HourRateDivisionFactor=0 then 1 else HourRateDivisionFactor end) *(TotalEarlyOutMins/60)   
else 0    end EarlyOutDeductAmount1



,case when  (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when eds.MonthlyEarlyOutCountDays=0 then 1 else eds.MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when eds.MonthlyLateInCountDays=0 then 1 else eds.MonthlyLateInCountDays end))   +LWPDay) <= FirstSlotAbsentDays and FirstSlotAbsentFrom='basic' then BasicSalary/@Dom *  (floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when MonthlyEarlyOutCountDays=0 then 1 else MonthlyEarlyOutCountDays end)))
when        (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when eds.MonthlyEarlyOutCountDays=0 then 1 else eds.MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when eds.MonthlyLateInCountDays=0 then 1 else eds.MonthlyLateInCountDays end))   +LWPDay) <= FirstSlotAbsentDays and FirstSlotAbsentFrom='gross' then GrossSalary/@Dom *  (floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when MonthlyEarlyOutCountDays=0 then 1 else MonthlyEarlyOutCountDays end)))
when        (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when eds.MonthlyEarlyOutCountDays=0 then 1 else eds.MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when eds.MonthlyLateInCountDays=0 then 1 else eds.MonthlyLateInCountDays end))   +LWPDay) > FirstSlotAbsentDays and SecondSlotAbsentFrom='basic' then BasicSalary/@Dom *  (floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when MonthlyEarlyOutCountDays=0 then 1 else MonthlyEarlyOutCountDays end)))
when        (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when eds.MonthlyEarlyOutCountDays=0 then 1 else eds.MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when eds.MonthlyLateInCountDays=0 then 1 else eds.MonthlyLateInCountDays end))   +LWPDay) > FirstSlotAbsentDays and SecondSlotAbsentFrom='gross' then GrossSalary/@Dom *  (floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when MonthlyEarlyOutCountDays=0 then 1 else MonthlyEarlyOutCountDays end)))
end         EarlyOutDeductAmount2

----------------------------AbsentAmount----------------------------
,case when  (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when eds.MonthlyEarlyOutCountDays=0 then 1 else eds.MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when eds.MonthlyLateInCountDays=0 then 1 else eds.MonthlyLateInCountDays end))   +LWPDay) <= FirstSlotAbsentDays and FirstSlotAbsentFrom='basic' then BasicSalary/@Dom *  (AbsentDay)
when        (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when eds.MonthlyEarlyOutCountDays=0 then 1 else eds.MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when eds.MonthlyLateInCountDays=0 then 1 else eds.MonthlyLateInCountDays end))   +LWPDay) <= FirstSlotAbsentDays and FirstSlotAbsentFrom='gross' then GrossSalary/@Dom *  (AbsentDay)
when        (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when eds.MonthlyEarlyOutCountDays=0 then 1 else eds.MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when eds.MonthlyLateInCountDays=0 then 1 else eds.MonthlyLateInCountDays end))   +LWPDay) > FirstSlotAbsentDays and SecondSlotAbsentFrom='basic' then BasicSalary/@Dom *  (AbsentDay)
when        (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when eds.MonthlyEarlyOutCountDays=0 then 1 else eds.MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when eds.MonthlyLateInCountDays=0 then 1 else eds.MonthlyLateInCountDays end))   +LWPDay) > FirstSlotAbsentDays and SecondSlotAbsentFrom='gross' then GrossSalary/@Dom *  (AbsentDay)
end         AbsentAmount
--,BasicSalary/@Dom *  (AbsentDay)       AbsentAmount

----------------------------LWPAmount----------------------------
,case when  (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when eds.MonthlyEarlyOutCountDays=0 then 1 else eds.MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when eds.MonthlyLateInCountDays=0 then 1 else eds.MonthlyLateInCountDays end))   +LWPDay) <= FirstSlotAbsentDays and FirstSlotAbsentFrom='basic' then BasicSalary/@Dom *  (LWPDay)
when        (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when eds.MonthlyEarlyOutCountDays=0 then 1 else eds.MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when eds.MonthlyLateInCountDays=0 then 1 else eds.MonthlyLateInCountDays end))   +LWPDay) <= FirstSlotAbsentDays and FirstSlotAbsentFrom='gross' then GrossSalary/@Dom *  (LWPDay)
when        (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when eds.MonthlyEarlyOutCountDays=0 then 1 else eds.MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when eds.MonthlyLateInCountDays=0 then 1 else eds.MonthlyLateInCountDays end))   +LWPDay) > FirstSlotAbsentDays and SecondSlotAbsentFrom='basic' then BasicSalary/@Dom *  (LWPDay)
when        (AbsentDay +floor(EarlyOutDayCount*EarlyOutAbsentDays/(case when eds.MonthlyEarlyOutCountDays=0 then 1 else eds.MonthlyEarlyOutCountDays end))+floor(LateInDayCount*LateInAbsentDays/(case when eds.MonthlyLateInCountDays=0 then 1 else eds.MonthlyLateInCountDays end))   +LWPDay) > FirstSlotAbsentDays and SecondSlotAbsentFrom='gross' then GrossSalary/@Dom *  (LWPDay)
end         LWPAmount


,FiscalYearDetailId,MonthlyAttendance.employeeid 
 FROM MonthlyAttendance
 LEFT OUTER JOIN (select EmployeeStructureGroup.EmployeeId 
,IsMonthlyLateInDeduct
,IsMonthlyLateInHourlyCount
,MonthlyLateInCountDays
,LateInAbsentDays
,IsMonthlyEarlyOutDeduct
,IsMonthlyEarlyOutHourlyCount
,MonthlyEarlyOutCountDays
,EarlyOutAbsentDays 
,FirstSlotAbsentDays
,FirstSlotAbsentFrom
,SecondSlotAbsentFrom
,DayRateCountFrom
,HourRateCountFrom
,DayRateDivisionFactor
,HourRateDivisionFactor

FROM EarningDeductionStructure
  LEFT OUTER JOIN EmployeeStructureGroup on EarningDeductionStructure.Id=EmployeeStructureGroup.EarningDeductionStructureId) 
  eds
  on MonthlyAttendance.EmployeeId=eds.EmployeeId
) as deduct 
 where deduct.FiscalYearDetailId=MonthlyAttendance.FiscalYearDetailId
 and deduct.employeeid=MonthlyAttendance.employeeid

--------------
--UPDATE MonthlyAttendance SET LateAmount =isnull(EarlyOutDeductAmount,0)+isnull(LateInDeductAmount,0)
--where  MonthlyAttendance.FiscalYearDetailId=@FiscalYearDetailId

 UPDATE MonthlyAttendance
 SET LateAbsentDay = Isnull(attn.LateDay,0)/3,LateDay = Isnull(attn.LateDay,0),LateAmount=floor(Isnull(attn.LateDay,0)/3)*(ma.BasicSalary/@DOM)
 FROM  MonthlyAttendance ma
 LEFT OUTER JOIN (Select EmployeeId, LateInDays as LateDay from EmployeeMonthlyAbsence where FiscalYearDetailId=@FiscalYearDetailId) 
 attn on attn.EmployeeId= ma.EmployeeId
 WHERE 1=1 
 AND ma.FiscalYearDetailId=@FiscalYearDetailId
";

                #endregion

                #region Other Deduction

                sqlText += @" 
----------------------------OtherDeductionDay----------------------------
 UPDATE MonthlyAttendance
 SET OtherDeductionDay = eot.Days
 FROM  MonthlyAttendance ma
 LEFT OUTER JOIN EmployeeOtherDeduction eot ON  ma.FiscalYearDetailId=eot.FiscalYearDetailId
 AND ma.EmployeeId=eot.EmployeeId
 WHERE 1=1 
 AND ma.FiscalYearDetailId=@FiscalYearDetailId
 AND eot.Days>0

";
                #endregion


                

                #region Update Remarks from Store Data

                sqlText += @" 
----------------------------Update Remarks From the Temp Table----------------------------
UPDATE MonthlyAttendance
SET Remarks = t.Remarks 
FROM MonthlyAttendance ma
LEFT OUTER JOIN #Temp t ON ma.EmployeeId = t.EmployeeId AND ma.FiscalYearDetailId = t.FiscalYearDetailId
WHERE 1=1 
AND ma.FiscalYearDetailId=@FiscalYearDetailId

DROP TABLE #Temp

";

                #endregion

                #endregion

                #endregion SqlText
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);
                if (transResult <= 0)
                {
                    retResults[1] = "Unexpected error to Monthly Attendance Insert!";
                    throw new ArgumentNullException(retResults[1], "");
                }

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Monthly Attendance Saved Successfully.";
                #endregion Commit
            }
            #region Catch and Finally
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
            #endregion Catch and Finally
            return retResults;
        }

        public DataTable Report(MonthlyAttendanceVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Report"); }

                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT 
ma.EmployeeId
,ma.FiscalYearDetailId
,fyd.PeriodName
,ve.Code
,ve.EmpName
,ve.Designation
,ve.Department
,ma.DOM
------------,ma.PresentDay
,ma.TotalLeave
,ma.LWPDay
,ma.AbsentDay
,ISNULL(CONVERT(int, fyd.PeriodEnd)- convert(int,fyd.PeriodStart)+1,0) -(isnull(ma.NPDay,0)+isnull(ma.LWPDay,0)+isnull(ma.AbsentDay,0)+isnull(ma.TotalLeave,0)) PresentDay
------------,convert(varchar(10), convert(int,Round(ISNULL(ma.TotalOTHrs,0)/60 *ISNULL(eds.WeeklyOTRate,0),0)))+'.00'    PayOTHrs
, convert(varchar(10), convert(int,isnull(ma.TotalOTHrs,0)/60)) +'.'+   RIGHT('00'+convert(varchar(10), convert(int,isnull(ma.TotalOTHrs,0) %60)),2)  PayOTHrs ------ ShowTotalOTHrs
,ISNULL(CONVERT(int, fyd.PeriodEnd)- convert(int,fyd.PeriodStart)+1,0) -(isnull(ma.NPDay,0)+isnull(ma.LWPDay,0)+isnull(ma.AbsentDay,0)) PayDays


,LateInHourCount
,EarlyOutHourCount
,LateInHourCount+EarlyOutHourCount as DeductionHour
,ISNULL(ma.OtherDeductionDay,0)OtherDeductionDay
,ISNULL(ma.LateAbsentDay,0)LateAbsentDay
,ma.Remarks

------------,ma.* 

FROM MonthlyAttendance ma
LEFT OUTER JOIN ViewEmployeeInformation ve ON ma.EmployeeId = ve.EmployeeId
LEFT OUTER JOIN FiscalYearDetail fyd ON ma.FiscalYearDetailId = fyd.Id
LEFT OUTER JOIN EmployeeStructureGroup esg ON ma.EmployeeId = esg.EmployeeId
LEFT OUTER JOIN EarningDeductionStructure eds ON esg.EarningDeductionStructureId = eds.Id
LEFT OUTER JOIN EmployeeJob ej ON ma.EmployeeId = ej.EmployeeId
LEFT OUTER JOIN Designation desig ON ve.DesignationId = desig.Id

WHERE 1=1
AND ma.FiscalYearDetailId = @FiscalYearDetailId
 ------------AND ma.FiscalYearDetailId = 1016
";

                #region More Conditions
                if (vm.CodeList != null && vm.CodeList.Count > 0)
                {
                    string MultipleCode = "";
                    foreach (var item in vm.CodeList)
                    {
                        MultipleCode += "'" + item + "',";
                    }
                    MultipleCode = MultipleCode.Trim(',');
                    sqlText += " AND ve.Code IN(" + MultipleCode + ")";
                }
                if (vm.DesignationList != null && vm.DesignationList.Count > 0)
                {
                    string MultipleDesignation = "";
                    foreach (var item in vm.DesignationList)
                    {
                        MultipleDesignation += "'" + item + "',";
                    }
                    MultipleDesignation = MultipleDesignation.Trim(',');
                    sqlText += " AND ve.DesignationId IN(" + MultipleDesignation + ")";
                }
                if (vm.DepartmentList != null && vm.DepartmentList.Count > 0)
                {
                    string MultipleDepartment = "";
                    foreach (var item in vm.DepartmentList)
                    {
                        MultipleDepartment += "'" + item + "',";
                    }
                    MultipleDepartment = MultipleDepartment.Trim(',');
                    sqlText += " AND ve.DepartmentId IN(" + MultipleDepartment + ")";
                }
                if (vm.SectionList != null && vm.SectionList.Count > 0)
                {
                    string MultipleSection = "";
                    foreach (var item in vm.SectionList)
                    {
                        MultipleSection += "'" + item + "',";
                    }
                    MultipleSection = MultipleSection.Trim(',');
                    sqlText += " AND ve.SectionId IN(" + MultipleSection + ")";
                }
                if (vm.ProjectList != null && vm.ProjectList.Count > 0)
                {
                    string MultipleProject = "";
                    foreach (var item in vm.ProjectList)
                    {
                        MultipleProject += "'" + item + "',";
                    }
                    MultipleProject = MultipleProject.Trim(',');
                    sqlText += " AND ve.ProjectId IN(" + MultipleProject + ")";
                }
                if (vm.Other1List != null && vm.Other1List.Count > 0)
                {
                    string MultipleOther1 = "";
                    foreach (var item in vm.Other1List)
                    {
                        MultipleOther1 += "'" + item + "',";
                    }
                    MultipleOther1 = MultipleOther1.Trim(',');
                    sqlText += " AND ej.Other1 IN(" + MultipleOther1 + ")";
                }
                if (vm.Other2List != null && vm.Other2List.Count > 0)
                {
                    string MultipleOther2 = "";
                    foreach (var item in vm.Other2List)
                    {
                        MultipleOther2 += "'" + item + "',";
                    }
                    MultipleOther2 = MultipleOther2.Trim(',');
                    sqlText += " AND ej.Other2 IN(" + MultipleOther2 + ")";
                }
                if (vm.Other3List != null && vm.Other3List.Count > 0)
                {
                    string MultipleOther3 = "";
                    foreach (var item in vm.Other3List)
                    {
                        MultipleOther3 += "'" + item + "',";
                    }
                    MultipleOther3 = MultipleOther3.Trim(',');
                    sqlText += " AND ej.Other3 IN(" + MultipleOther3 + ")";
                }

                #endregion

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }


                //////sqlText += " Order By ve.DepartmentId, ve.ProjectId, ve.SectionId, ve.EmployeeId";

                if (vm.OrderBy == "DCG")
                    sqlText += " ORDER BY ve.Department, ve.Code";
                else if (vm.OrderBy == "DDC")
                    sqlText += " ORDER BY ve.Department, ve.Code";
                else if (vm.OrderBy == "DGC")
                    sqlText += " ORDER BY ve.Department, ve.Code";
                else if (vm.OrderBy == "DGDC")
                    sqlText += " ORDER BY ve.Department, ve.Code";
                else if (vm.OrderBy == "CODE")
                    sqlText += " ORDER BY ve.Code";
                else if (vm.OrderBy.ToLower() == "designation")
                    sqlText += " ORDER BY ISNULL(desig.PriorityLevel,0), ve.Code";



                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                da.Fill(dt);

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        #endregion Methods
    }
}

