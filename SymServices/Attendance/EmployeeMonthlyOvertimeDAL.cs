using SymOrdinary;
using SymViewModel.Attendance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOrdinary;
using SymServices.HRM;
using SymServices.Common;
using SymViewModel.HRM;
using System.Threading;
using SymViewModel.Common;
using System.IO;
using Excel;
using MSScriptControl;

namespace SymServices.Attendance
{
    public class EmployeeMonthlyOvertimeDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();
        public List<EmployeeMonthlyOvertimeVM> SelectAll(string ProjectId, string DepartmentId, string SectionId, int fid = 0)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeMonthlyOvertimeVM> VMs = new List<EmployeeMonthlyOvertimeVM>();
            EmployeeMonthlyOvertimeVM vm;
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
declare @StartDate1 as varchar(20)  
declare @EndDate1 as varchar(20)  

--declare @FiscalYearDetailId as varchar(20)  
--set @FiscalYearDetailId = '1005'

select @StartDate1=periodStart,@EndDate1=PeriodEnd from FiscalYearDetail
where Id=@FiscalYearDetailId

create table #EJ(EmployeeId  varchar(50),Emptatus varchar(50))
insert into #EJ
    EXEC dbo.ProEmployeeJobs @StartDate = @StartDate1,@EndDate=@EndDate1
	--select * from #EJ
	

SELECT * FROM (
SELECT 
emab.EmployeeId
,emab.Code
,emab.EmpName
,emab.Project
,emab.Department
,emab.Section
,emab.Designation
,emab.ProjectId
,emab.DepartmentId
,emab.SectionId
,emab.DesignationId
,eda.Remarks
,eda.FiscalYearDetailId
,ISNULL( eda.TotalOvertime,0)  TotalOvertime
,ISNULL( eda.TotalLateInHrs,0)  TotalLateInHrs
,ISNULL( eda.TotalEarlyOutHrs,0)  TotalEarlyOutHrs
,IsNull(eda.IsManual, 0) IsManual

FROM #EJ

LEFT OUTER JOIN ViewEmployeeInformation emab ON #EJ.EmployeeId = emab.EmployeeId
LEFT OUTER JOIN EmployeeMonthlyOvertime eda ON emab.EmployeeId =ISNULL(eda.EmployeeId,0)
WHERE emab.IsArchive=0 and emab.IsActive=1
AND eda.FiscalYearDetailId = @FiscalYearDetailId
UNION ALL
SELECT 
#EJ.EmployeeId
,Code
,EmpName
,Project
,Department
,Section
,Designation
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,'0'
,0
,0 
,0
,0
,0
FROM #EJ
LEFT OUTER JOIN ViewEmployeeInformation ON #EJ.EmployeeId = ViewEmployeeInformation.EmployeeId
WHERE ViewEmployeeInformation.EmployeeId NOT IN(
SELECT DISTINCT EmployeeId FROM EmployeeMonthlyOvertime
WHERE 1=1 AND FiscalYearDetailId=@FiscalYearDetailId
)
) as a
WHERE 1=1
";
                if (DepartmentId != "0_0")
                    sqlText += " AND a.DepartmentId=@DepartmentId ";
                if (ProjectId != "0_0")
                    sqlText += " AND a.ProjectId=@ProjectId";
                if (SectionId != "0_0")
                    sqlText += " AND a.SectionId=@SectionId ";
                sqlText += @" Order By a.DepartmentId, a.ProjectId, a.SectionId, a.EmployeeId
                
drop table #EJ
                ";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (DepartmentId != "0_0")
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                if (ProjectId != "0_0")
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                if (SectionId != "0_0")
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                objComm.Parameters.AddWithValue("@FiscalYearDetailId", fid);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeMonthlyOvertimeVM();
                    vm.Department = dr["Department"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();

                    //debugging
                    if (vm.EmployeeId == "1_81")
                    {
                        int a = 0;
                    }

                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    //vm.TotalOvertime = Convert.ToDecimal(dr["TotalOvertime"]);
                    decimal totalOTInMin = Convert.ToDecimal(dr["TotalOvertime"]);
                    var min = totalOTInMin % 60;
                    var hrs = (totalOTInMin - min) / 60;
                    var totalOTInHrs = Convert.ToInt32(hrs) + "." + Convert.ToInt32(min).ToString().PadLeft(2, '0');
                    vm.TotalOvertime = Convert.ToDecimal(totalOTInHrs);
                    vm.TotalLateInHrs = Convert.ToDecimal(dr["TotalLateInHrs"]);
                    vm.TotalEarlyOutHrs = Convert.ToDecimal(dr["TotalEarlyOutHrs"]);

                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.SearchFid = fid;

                    vm.TotalOvertimePrevious = vm.TotalOvertime;
                    vm.TotalLateInHrsPrevious = vm.TotalLateInHrs;
                    vm.TotalEarlyOutHrsPrevious = vm.TotalEarlyOutHrs;
                    vm.IsManual = Convert.ToBoolean(dr["IsManual"]);

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


        public string[] InsertFromDailyOvertime(string FiscalYearDetailId, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Monthly Overtime"; //Method Name
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
declare @StartDate as varchar(20)
declare @EndDate as varchar(20)
--declare @FiscalYearDetailId as varchar(20)

--set @FiscalYearDetailId='1003'

select @StartDate=periodStart,@EndDate=PeriodEnd from FiscalYearDetail
where id=@FiscalYearDetailId
 delete from EmployeeMonthlyOvertime
where FiscalYearDetailId=@FiscalYearDetailId

 insert into EmployeeMonthlyOvertime(EmployeeId,FiscalYearDetailId
,ProjectId,DepartmentId,SectionId,DesignationId,OTRate
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
,TotalOvertime,TotalOvertimeBy, TotalLateInHrs, TotalEarlyOutHrs)

 select distinct  EmployeeDailyOvertime.EmployeeId,@FiscalYearDetailId,
  EmployeeJob.ProjectId,EmployeeJob.DepartmentId,EmployeeJob.SectionId,EmployeeJob.DesignationId,0
,'-',@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
 ,Sum(Overtime)Overtime,Sum(case when Overtime>2 then 2 else Overtime end) OveTimeBy
, Sum(LateInHrs)LateInHrs, Sum(EarlyOutHrs)EarlyOutHrs 
  
 from EmployeeDailyOvertime left outer join EmployeeJob on EmployeeDailyOvertime.EmployeeId=EmployeeJob.EmployeeId
where OvertimeDate between @StartDate and @EndDate
group by EmployeeDailyOvertime.EmployeeId,
  EmployeeJob.ProjectId,EmployeeJob.DepartmentId,EmployeeJob.SectionId,EmployeeJob.DesignationId

select * from EmployeeMonthlyOvertime
where FiscalYearDetailId=@FiscalYearDetailId
";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);

                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                if (transResult <= 0)
                {
                    retResults[1] = "Unexpected error to Employee Monthly Overtime Insert!";
                    throw new ArgumentNullException(retResults[1], "");
                }

                #endregion Save

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Employee Monthly Overtime Saved Successfully!";


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
        public string[] Insert(List<EmployeeMonthlyOvertimeVM> VMs, string fid, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Monthly Overtime"; //Method Name
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
                sqlText = "  ";
                sqlText += @" INSERT INTO dbo.EmployeeMonthlyOvertime
(
EmployeeId
,FiscalYearDetailId
,TotalOvertime
,TotalLateInHrs
,TotalEarlyOutHrs
,TotalLateInMins
,TotalEarlyOutMins
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,OTRate
,IsManual
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom)
 VALUES 
(
@EmployeeId
,@FiscalYearDetailId
,@TotalOvertime
,@TotalLateInHrs
,@TotalEarlyOutHrs
,@TotalLateInMins
,@TotalEarlyOutMins
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@OTRate
,@IsManual
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom)";
                //LateInHrs
                //EarlyOutHrs

                #region New Variables
                DataTable empDt = new DataTable();
                DataTable fPeriodDt = new DataTable();
                string periodStart = "";
                string periodEnd = "";
                int daysOfMonth = 0;

                decimal GrossSalary = 0;
                decimal BasicSalary = 0;
                int DOM = 0;

                DataTable empStGroupDt = new DataTable();
                string earningDeductionStructureId = "";
                SettingDAL _setDAL = new SettingDAL();
                ScriptControl sc = new ScriptControl();
                sc.Language = "VBScript";

                EarningDeductionStructureDAL _edStDal = new EarningDeductionStructureDAL();
                EarningDeductionStructureVM edStVM = new EarningDeductionStructureVM();
                decimal weeklyOTRate = 0;
                EmployeeInfoDAL _eiDAL = new EmployeeInfoDAL();

                #endregion New Variables


                #region Fiscal Period
                fPeriodDt = _cDal.SelectByCondition("FiscalYearDetail", "Id", fid, currConn, transaction);
                if (fPeriodDt == null || fPeriodDt.Rows.Count == 0)
                {
                    retResults[1] = "Fiscal Period Not Exists!";
                    throw new ArgumentNullException(retResults[1], "");
                }
                periodStart = fPeriodDt.Rows[0]["PeriodStart"].ToString();
                periodEnd = fPeriodDt.Rows[0]["PeriodEnd"].ToString();
                daysOfMonth = Convert.ToInt32(periodEnd) - Convert.ToInt32(periodStart) + 1;
                #endregion Fiscal Period

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                if (VMs.Count >= 1)
                {
                    #region Update Settings
                    foreach (var item in VMs)
                    {
                        //debugging 
                        var debugEmployeeId = item.EmployeeId;
                        if (debugEmployeeId == "1_711")
                        {
                            int a = 0;
                        }

                        #region CheckPoint
                        //if (item.TotalOvertime <= 0 && item.TotalLateInHrs == 0 && item.TotalEarlyOutHrs == 0)
                        //{
                        //    continue;
                        //}

                        bool isExist = false;
                        //if this employee in this month absent days alredy exist then delete first then insert
                        string[] conditionFields = { "EmployeeId", "FiscalYearDetailId" };
                        string[] conditionValues = { item.EmployeeId, fid };
                        isExist = _cDal.ExistCheck("EmployeeMonthlyOvertime", conditionFields, conditionValues, currConn, transaction);

                        if (isExist)
                        {
                            retResults = _cDal.DeleteTable("EmployeeMonthlyOvertime", conditionFields, conditionValues, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                retResults[1] = "Update Fail!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }

                        #endregion CheckPoint

                        #region Profile
                        empDt = _eiDAL.SelectEmpForAttendance(item.EmployeeId, Ordinary.StringToDate(periodEnd), currConn, transaction);

                        //////empDt = _cDal.SelectByCondition("ViewEmployeeInformation", "EmployeeId", item.EmployeeId, currConn, transaction);

                        if (empDt == null || empDt.Rows.Count == 0)
                        {
                            retResults[1] = "Employee Not Exists! Employee: " + item.Code + "~" + item.EmpName;
                            //return retResults;
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        GrossSalary = Convert.ToDecimal(empDt.Rows[0]["GrossSalary"]);
                        BasicSalary = Convert.ToDecimal(empDt.Rows[0]["BasicSalary"]);
                        DOM = daysOfMonth;
                        #endregion Profile
                        #region OTRate
                        empStGroupDt = _cDal.SelectByCondition("EmployeeStructureGroup", "EmployeeId", item.EmployeeId, currConn, transaction);
                        if (empStGroupDt != null && empStGroupDt.Rows.Count > 0)
                        {
                            earningDeductionStructureId = empStGroupDt.Rows[0]["EarningDeductionStructureId"].ToString();
                            string[] edStConFields = { "Id" };
                            string[] edStConValues = { earningDeductionStructureId };
                            edStVM = _edStDal.SelectByMultiCondition(edStConFields, edStConValues, currConn, transaction).FirstOrDefault();
                        }

                        if (edStVM != null && edStVM.Id > 0)
                        {
                            weeklyOTRate = edStVM.WeeklyOTRate;
                            var HourRateCalc = _setDAL.settingValue("OTHourRate", "OTHourRate", currConn, transaction);
                            HourRateCalc = HourRateCalc.Replace("vGross", GrossSalary.ToString());
                            HourRateCalc = HourRateCalc.Replace("vBasic", BasicSalary.ToString());
                            HourRateCalc = HourRateCalc.Replace("DOM", DOM.ToString());
                            dynamic hourR = sc.Eval(HourRateCalc);
                            decimal hourRate = Convert.ToDecimal(hourR);
                            item.OTRate = hourRate * weeklyOTRate;
                        }
                        else
                        {
                            retResults[1] = "Earning Deduction Policy/Structure Not Assigned For Employee: " + item.Code + "~" + item.EmpName;
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion OTRate
                        #region Hour-Min Conversion

                        //totalOTHrs = decimal.Truncate(item.TotalOvertime);
                        //OTMins = item.TotalOvertime - totalOTHrs;
                        //item.TotalOvertime = totalOTHrs * 60 + OTMins;

                        decimal time = item.TotalOvertime;
                        int hrs = Convert.ToInt32(Math.Floor(time));
                        int min = Convert.ToInt32((Convert.ToDecimal(time) - Convert.ToDecimal(hrs)) * 100);
                        item.TotalOvertime = hrs * 60 + min;



                        time = item.TotalLateInHrs;
                        hrs = Convert.ToInt32(Math.Floor(time));
                        min = Convert.ToInt32((Convert.ToDecimal(time) - Convert.ToDecimal(hrs)) * 100);
                        item.TotalLateInMins = hrs * 60 + min;

                        //totalLateInHrs = decimal.Truncate(item.TotalLateInHrs);
                        //LateInMins = item.TotalLateInHrs - totalLateInHrs;
                        //item.TotalLateInMins = totalLateInHrs * 60 + LateInMins;


                        time = item.TotalEarlyOutHrs;
                        hrs = Convert.ToInt32(Math.Floor(time));
                        min = Convert.ToInt32((Convert.ToDecimal(time) - Convert.ToDecimal(hrs)) * 100);
                        item.TotalEarlyOutMins = hrs * 60 + min;



                        //totalEarlyOutHrs = decimal.Truncate(item.TotalEarlyOutHrs);
                        //EarlyOutMins = item.TotalEarlyOutHrs - totalEarlyOutHrs;
                        //item.TotalEarlyOutMins = totalEarlyOutHrs * 60 + EarlyOutMins;
                        #endregion
                        if (item.IsManual != true)
                        {
                            if (item.TotalOvertime == item.TotalOvertimePrevious && item.TotalLateInHrs == item.TotalLateInHrsPrevious && item.TotalEarlyOutHrs == item.TotalEarlyOutHrsPrevious)
                            {
                                item.IsManual = false;
                            }
                            else
                            {
                                item.IsManual = true;
                            }
                        }
                        #region Sql Execution

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                        cmdInsert.Parameters.AddWithValue("@EmployeeId", item.EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@TotalOvertime", item.TotalOvertime);
                        cmdInsert.Parameters.AddWithValue("@TotalLateInHrs", item.TotalLateInHrs);
                        cmdInsert.Parameters.AddWithValue("@TotalEarlyOutHrs", item.TotalEarlyOutHrs);
                        cmdInsert.Parameters.AddWithValue("@TotalLateInMins", item.TotalLateInMins);
                        cmdInsert.Parameters.AddWithValue("@TotalEarlyOutMins", item.TotalEarlyOutMins);

                        cmdInsert.Parameters.AddWithValue("@ProjectId", item.ProjectId);
                        cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", fid);
                        cmdInsert.Parameters.AddWithValue("@DepartmentId", item.DepartmentId);
                        cmdInsert.Parameters.AddWithValue("@SectionId", item.SectionId);
                        cmdInsert.Parameters.AddWithValue("@DesignationId", item.DesignationId);
                        cmdInsert.Parameters.AddWithValue("@OTRate", item.OTRate);
                        cmdInsert.Parameters.AddWithValue("@IsManual", item.IsManual);

                        cmdInsert.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);
                        var exe = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exe);
                        if (transResult <= 0)
                        {
                            retResults[1] = "Unexpected error to Employee Monthly Overtime Insert!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion

                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #endregion Insert Settings
                    #region Update MonthlyOvertime
                    SettingDAL _settingDal = new SettingDAL();
                    string MonthlyOTRoundUp = _settingDal.settingValue("OverTime", "MonthlyOTRoundUp", currConn, transaction).Trim();

                    retResults = UpdateTotalOvertime(fid, MonthlyOTRoundUp, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    #endregion Update MonthlyOvertime

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Employee Monthly Overtime", "Could not found any item.");
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
                    retResults[1] = "Employee Monthly Overtime Saved Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to Employee Monthly Overtime Insert.";
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

        //Might be Deprecated/Obsolete
        public string[] SingleInsertUpdate(EmployeeMonthlyOvertimeVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Monthly Overtime"; //Method Name
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
                sqlText = "  ";
                sqlText += @" INSERT INTO dbo.EmployeeMonthlyOvertime
(
EmployeeId
,FiscalYearDetailId
,TotalOvertime
,TotalLateInHrs
,TotalEarlyOutHrs
,TotalLateInMins
,TotalEarlyOutMins
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom)
 VALUES 
(
@EmployeeId
,@FiscalYearDetailId
,@TotalOvertime
,@TotalLateInHrs
,@TotalEarlyOutHrs
,@TotalLateInMins
,@TotalEarlyOutMins
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom)";
                if (vm != null)
                {
                    #region Update Settings
                    #region CheckPoint
                    bool isExist = false;
                    //if this employee in this month absent days alredy exist then delete first then insert
                    string[] conditionFields = { "EmployeeId", "FiscalYearDetailId" };
                    string[] conditionValues = { vm.EmployeeId, vm.FiscalYearDetailId.ToString() };
                    isExist = _cDal.ExistCheck("EmployeeMonthlyOvertime", conditionFields, conditionValues, currConn, transaction);

                    if (isExist)
                    {
                        retResults = _cDal.DeleteTable("EmployeeMonthlyOvertime", conditionFields, conditionValues, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            retResults[1] = "Update Fail!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion CheckPoint
                    #region Hour-Min Conversion
                    decimal time = vm.TotalOvertime;
                    int hrs = Convert.ToInt32(Math.Floor(time));
                    int min = Convert.ToInt32((Convert.ToDecimal(time) - Convert.ToDecimal(hrs)) * 100);
                    vm.TotalOvertime = hrs * 60 + min;



                    time = vm.TotalLateInHrs;
                    hrs = Convert.ToInt32(Math.Floor(time));
                    min = Convert.ToInt32((Convert.ToDecimal(time) - Convert.ToDecimal(hrs)) * 100);
                    vm.TotalLateInMins = hrs * 60 + min;


                    time = vm.TotalEarlyOutHrs;
                    hrs = Convert.ToInt32(Math.Floor(time));
                    min = Convert.ToInt32((Convert.ToDecimal(time) - Convert.ToDecimal(hrs)) * 100);
                    vm.TotalEarlyOutMins = hrs * 60 + min;
                    #endregion
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);


                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId.ToString());
                    cmdInsert.Parameters.AddWithValue("@TotalOvertime", vm.TotalOvertime);
                    cmdInsert.Parameters.AddWithValue("@TotalLateInHrs", vm.TotalLateInHrs);
                    cmdInsert.Parameters.AddWithValue("@TotalEarlyOutHrs", vm.TotalEarlyOutHrs);
                    cmdInsert.Parameters.AddWithValue("@TotalLateInMins", vm.TotalLateInMins);
                    cmdInsert.Parameters.AddWithValue("@TotalEarlyOutMins", vm.TotalEarlyOutMins);

                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@IsManual", vm.IsManual);


                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.ExecuteNonQuery();
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #endregion Insert Settings
                    #region Update EmployeeMonthlyOvertime
                    SettingDAL _settingDal = new SettingDAL();
                    string MonthlyOTRoundUp = _settingDal.settingValue("OverTime", "MonthlyOTRoundUp", currConn, transaction).Trim();

                    retResults = UpdateTotalOvertime(vm.FiscalYearDetailId.ToString(), MonthlyOTRoundUp, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    #endregion Update EmployeeMonthlyOvertime
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Employee Monthly Overtime", "Could not found any item.");
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
                    retResults[1] = "Employee Monthly Overtime Saved Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to Employee Monthly Overtime Insert.";
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

        public DataTable Report(EmployeeMonthlyOvertimeVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

                    if (!string.IsNullOrWhiteSpace(vm.DBName))
                    {
                        currConn.ChangeDatabase(vm.DBName);
                    }
                }
                ////////if (transaction == null) { transaction = currConn.BeginTransaction("Report"); }

                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT 
ve.EmployeeId
,ve.Code
,ve.EmpName
,ve.Project
,ve.Department
,ve.Section
,ve.Designation
,ve.ProjectId
,ve.DepartmentId
,ve.SectionId
,ve.DesignationId
,ve.JoinDate
,ISNULL(ve.BasicSalary,0) BasicSalary
,ISNULL(ve.GrossSalary,0) GrossSalary
,ISNULL(mot.OTRate,0) OTRate
,ISNULL(fyd.PeriodName,0) PeriodName
,ISNULL(fyd.PeriodStart,0) PeriodStart
,ISNULL(eda.FiscalYearDetailId, 0) FiscalYearDetailId
,ISNULL(eda.TotalOvertime,0)  TotalOvertime
,ISNULL(eda.TotalLateInHrs,0)  TotalLateInHrs
,ISNULL(eda.TotalEarlyOutHrs,0)  TotalEarlyOutHrs
,ISNULL(eds.WeeklyOTRate,0)  WeeklyOTRate
,ISNULL(eds.GovtOTRate,0)  GovtOTRate
,ISNULL(eds.FestivalOTRate,0)  FestivalOTRate
,ISNULL(eds.SpecialOTRate,0)  SpecialOTRate

, convert(varchar(10), convert(int,isnull(mot.TotalOTHrs,0)*ISNULL(eds.WeeklyOTRate,0)/60)) +'.'+   RIGHT('00'+convert(varchar(10),convert(int,isnull(mot.TotalOTHrs,0)*ISNULL(eds.WeeklyOTRate,0) %60)),2)  ShowTotalOTHrsPay
, convert(varchar(10), convert(int,isnull(mot.TotalOTHrsBY,0)*ISNULL(eds.WeeklyOTRate,0)/60)) +'.'+   RIGHT('00'+convert(varchar(10),convert(int,isnull(mot.TotalOTHrsBY,0)*ISNULL(eds.WeeklyOTRate,0) %60)),2)  ShowTotalOTHrsBYPay
, convert(varchar(10), convert(int,isnull(mot.TotalOTHrsExtra,0)*ISNULL(eds.WeeklyOTRate,0)/60)) +'.'+   RIGHT('00'+convert(varchar(10),convert(int,isnull(mot.TotalOTHrsExtra,0)*ISNULL(eds.WeeklyOTRate,0) %60)),2)  ShowTotalOvertimeExtraPay

,convert(varchar(10), convert(int,Round(ISNULL(mot.TotalOTHrs,0)/60 *ISNULL(eds.WeeklyOTRate,0),0)))+'.00'    ShowTotalOTHrsPayRound
, convert(varchar(10), convert(int,Round(isnull(mot.TotalOTHrsBY,0)/60  *ISNULL(eds.WeeklyOTRate,0),0))) +'.00'  ShowTotalOTHrsBYPayRound
, convert(varchar(10), convert(int,Round(isnull(mot.TotalOTHrsExtra,0)/60 *ISNULL(eds.WeeklyOTRate,0),0))) +'.00'    ShowTotalOvertimeExtraPayRound


, convert(varchar(10), convert(int,isnull(mot.TotalOTHrs,0)/60)) +'.'+   RIGHT('00'+convert(varchar(10), convert(int,isnull(mot.TotalOTHrs,0) %60)),2) ShowTotalOTHrs
, convert(varchar(10), convert(int,isnull(mot.TotalOTHrsBY,0)/60)) +'.'+   RIGHT('00'+convert(varchar(10), convert(int,isnull(mot.TotalOTHrsBY,0) %60)),2)  ShowTotalOTHrsBY
, convert(varchar(10), convert(int,isnull(mot.TotalOTHrsExtra,0)/60)) +'.'+   RIGHT('00'+convert(varchar(10), convert(int,isnull(mot.TotalOTHrsExtra,0) %60)),2)  ShowTotalOvertimeExtra

,ISNULL(mot.TotalOTHrs,0)/60 TotalOTHrs
,ISNULL(mot.TotalOTHrsBY,0)/60 TotalOTHrsBY
,ISNULL(mot.TotalOTHrsExtra,0)/60 TotalOTHrsExtra

,floor(ISNULL(mot.TotalOTHrs,0)/60*ISNULL(eds.WeeklyOTRate,0)) TotalOTHrsPay
,floor(ISNULL(mot.TotalOTHrsBY,0)/60*ISNULL(eds.WeeklyOTRate,0)) TotalOTHrsBYPay
,floor(ISNULL(mot.TotalOTHrsExtra,0)/60*ISNULL(eds.WeeklyOTRate,0)) TotalOTHrsExtraPay



 ,Round(Round(ISNULL(mot.TotalOTHrs,0)/60 *ISNULL(eds.WeeklyOTRate,0),0) * isnull(mot.otrate,0),0) OTAmount

,Round(ISNULL(mot.TotalOTHrsBY,0)/60 *ISNULL(eds.WeeklyOTRate,0),0)*isnull(mot.otrate,0) TotalOOTAmountByTHrsBY
,Round(ISNULL(mot.TotalOTHrsExtra,0)/60*ISNULL(eds.WeeklyOTRate,0),0)*isnull(mot.otrate,0)  OTAmountExtra

--------,ISNULL(mot.OTAmount,0)/60 OTAmount
--------,ISNULL(mot.OTAmountBY,0)/60  OTAmountBy
--------,ISNULL(mot.OTAmountExtra,60)  OTAmountExtra

,ISNULL(se.IsHold,0) IsHold
,case when ISNULL(se.IsHold,0) = 1 then 'Hold' else 'Not Hold' end HoldStatus

FROM ViewEmployeeInformationAll ve
LEFT OUTER JOIN EmployeeMonthlyOvertime eda ON ve.EmployeeId =ISNULL(eda.EmployeeId,0)
LEFT OUTER JOIN FiscalYearDetail fyd ON eda.FiscalYearDetailId = fyd.Id
LEFT OUTER JOIN MonthlyAttendance mot ON eda.FiscalYearDetailId = mot.FiscalYearDetailId and  eda.EmployeeId = mot.EmployeeId 
LEFT OUTER JOIN EarningDeductionStructure eds ON ve.EarningDeductionStructureId = eds.Id
LEFT OUTER JOIN dbo.Designation AS desig ON ve.DesignationId = desig.Id
LEFT OUTER JOIN EmployeeJob ej ON ve.EmployeeId = ej.EmployeeId
LEFT OUTER JOIN SalaryEmployee se ON eda.EmployeeId = se.EmployeeId and  eda.FiscalYearDetailId = se.FiscalYearDetailId
WHERE 1=1
------AND ve.IsArchive=0 and ve.IsActive=1
AND eda.TotalOvertime > 0
AND eda.FiscalYearDetailId = @FiscalYearDetailId

";

                if (vm.ProjectIdList != null && vm.ProjectIdList.Count > 0 && !string.IsNullOrWhiteSpace(vm.ProjectIdList.FirstOrDefault()))
                {
                    string sqlTextCondition = "";
                    sqlText += "  AND ve.ProjectId IN(";
                    foreach (string item in vm.ProjectIdList)
                    {
                        sqlTextCondition += "'" + item + "',";
                    }
                    sqlTextCondition = sqlTextCondition.Trim(',');
                    sqlText += sqlTextCondition + ")";
                }

                //if (vm.Other3List != null && vm.Other3List.Count > 0 && !string.IsNullOrWhiteSpace(vm.Other3List.FirstOrDefault()))
                //{
                //    string sqlTextCondition = "";
                //    //sqlText += "  AND ej.Other3 IN(";
                //    //foreach (string item in vm.Other3List)
                //    //{
                //    //    sqlTextCondition += "'" + item + "',";
                //    //}
                //    sqlTextCondition = sqlTextCondition.Trim(',');
                //    sqlText += sqlTextCondition + ")";
                //}




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

                if (!string.IsNullOrWhiteSpace(vm.HoldStatus))
                {
                    if (vm.HoldStatus.ToLower() == "hold")
                    {
                        sqlText += " AND se.IsHold=1 ";
                    }
                    else
                    {
                        sqlText += " AND se.IsHold=0 ";
                    }
                }

                //////sqlText += " Order By ve.DepartmentId, ve.ProjectId, ve.SectionId, ve.EmployeeId";
                if (string.IsNullOrWhiteSpace(vm.OrderBy))
                {
                    vm.OrderBy = "";
                }

                if (vm.OrderBy == "DCG")
                    sqlText += " ORDER BY ve.Department, ve.Code";
                else if (vm.OrderBy == "DDC")
                    sqlText += " ORDER BY ve.Department, ve.JoinDate, ve.Code";
                else if (vm.OrderBy == "DGC")
                    sqlText += " ORDER BY ve.Department, ve.Code";
                else if (vm.OrderBy == "DGDC")
                    sqlText += " ORDER BY ve.Department, ve.JoinDate, ve.Code";
                else if (vm.OrderBy == "CODE")
                    sqlText += " ORDER BY ve.Code";
                else if (vm.OrderBy.ToLower() == "designation")
                    sqlText += " ORDER BY ISNULL(desig.PriorityLevel,0), ve.Code";



                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                ////////da.SelectCommand.Transaction = transaction;
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
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "JoinDate");

                ////////if (Vtransaction == null && transaction != null)
                ////////{
                ////////    transaction.Commit();
                ////////}
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

        public string[] ImportExcelFile(string fullPath, string fileName, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Salary Provident Fund"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            EmployeeInfoDAL _dalemp = new EmployeeInfoDAL();
            ViewEmployeeInfoVM empVM = new ViewEmployeeInfoVM();
            FiscalYearDAL fydal = new FiscalYearDAL();
            FiscalYearDetailVM FYDVM = new FiscalYearDetailVM();
            List<EmployeeMonthlyOvertimeVM> VMs = new List<EmployeeMonthlyOvertimeVM>();


            #region try
            try
            {
                #region Reading Excel
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                FileStream stream = System.IO.File.Open(fullPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (fileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();
                dt = ds.Tables[0];
                reader.Close();
                #endregion Reading Excel
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
                foreach (DataRow item in dt.Rows)
                {
                    #region CheckPoint

                    if (!Ordinary.IsNumeric(item["TotalOvertime"].ToString()) || !Ordinary.IsNumeric(item["TotalLateInHrs"].ToString()) || !Ordinary.IsNumeric(item["TotalEarlyOutHrs"].ToString()))
                    {
                        retResults[1] = "Please input the Numeric Value in Amount!";
                        return retResults;
                    }
                    if (Convert.ToDecimal(item["TotalOvertime"]) < 0 || Convert.ToDecimal(item["TotalLateInHrs"]) < 0 || Convert.ToDecimal(item["TotalEarlyOutHrs"]) < 0)
                    {
                        retResults[1] = "Please input the Non-Negative Value in Amount!";
                        return retResults;
                    }
                    //if (!Ordinary.IsTimeFormat(item["TotalOvertime"].ToString()) || !Ordinary.IsTimeFormat(item["TotalLateInHrs"].ToString()) || !Ordinary.IsTimeFormat(item["TotalEarlyOutHrs"].ToString()))
                    //{
                    //    retResults[1] = "Please input the Numeric Value in Time Format!";
                    //    return retResults;
                    //}


                    EmployeeMonthlyOvertimeVM vm = new EmployeeMonthlyOvertimeVM();
                    empVM = _dalemp.ViewSelectAllEmployee(item["Code"].ToString(), "", "", "", "", "", "", currConn, transaction).FirstOrDefault();
                    if (empVM == null || empVM.Id == null)
                    {
                        retResults[1] = "Employee Code " + item["Code"].ToString() + " Not in System";
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    FYDVM = fydal.FYPeriodDetail(Convert.ToInt32(item["FiscalYearDetailId"].ToString()), currConn, transaction).FirstOrDefault();
                    if (FYDVM == null)
                    {
                        retResults[1] = "Fiscal Period" + item["FiscalYearDetailId"].ToString() + " Not in System";
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion CheckPoint
                    #region Assign Data
                    vm.EmployeeId = empVM.Id;
                    vm.FiscalYearDetailId = FYDVM.Id;
                    vm.TotalOvertime = Convert.ToDecimal(item["TotalOvertime"]);
                    vm.TotalLateInHrs = Convert.ToDecimal(item["TotalLateInHrs"]);
                    vm.TotalEarlyOutHrs = Convert.ToDecimal(item["TotalEarlyOutHrs"]);
                    vm.IsManual = true;


                    vm.ProjectId = empVM.ProjectId;
                    vm.DepartmentId = empVM.DepartmentId;
                    vm.SectionId = empVM.SectionId;
                    vm.DesignationId = empVM.DesignationId;
                    //vm.LastUpdateAt = auditvm.LastUpdateAt;
                    //vm.LastUpdateBy = auditvm.LastUpdateBy;
                    //vm.LastUpdateFrom = auditvm.LastUpdateFrom;
                    vm.CreatedAt = auditvm.CreatedAt;
                    vm.CreatedBy = auditvm.CreatedBy;
                    vm.CreatedFrom = auditvm.CreatedFrom;
                    VMs.Add(vm);
                    #endregion
                }
                #region Insert Data
                retResults = Insert(VMs, VMs.FirstOrDefault().FiscalYearDetailId.ToString(), auditvm, VcurrConn, Vtransaction);
                if (retResults[0] == "Fail")
                {
                    retResults[1] = "Employee Monthly Overtime - Could not found any item!";
                    throw new ArgumentNullException(retResults[1], "");
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
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                //retResults[2] = vm.Id.ToString();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }


        public DataTable ExportExcelFile(EmployeeMonthlyOvertimeVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            string[] retResults = new string[6];
            DataTable dt = new DataTable();
            SqlTransaction transaction = null;
            try
            {
                #region Variables
                SqlConnection currConn = null;
                string sqlText = "";
                #endregion
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("ExportExcelFile"); }
                #endregion open connection and transaction
                #region DataRead From DB
                #region sql statement
                sqlText = @"
declare @StartDate1 as varchar(20)  
declare @EndDate1 as varchar(20)  

--declare @FiscalYearDetailId as varchar(20)  
--set @FiscalYearDetailId = '1005'

select @StartDate1=periodStart,@EndDate1=PeriodEnd from FiscalYearDetail
where Id=@FiscalYearDetailId

create table #EJ(EmployeeId  varchar(50),Emptatus varchar(50))
insert into #EJ
    EXEC dbo.ProEmployeeJobs @StartDate = @StartDate1,@EndDate=@EndDate1
	--select * from #EJ
	

SELECT * FROM (
SELECT 
emab.EmployeeId
,emab.Code
,emab.EmpName
,emab.Project
,emab.Department
,emab.Section
,emab.Designation
,emab.ProjectId
,emab.DepartmentId
,emab.SectionId
,emab.DesignationId
,eda.Remarks
,eda.FiscalYearDetailId
,ISNULL( eda.TotalOvertime,0)  TotalOvertime
,ISNULL( eda.TotalLateInHrs,0)  TotalLateInHrs
,ISNULL( eda.TotalEarlyOutHrs,0)  TotalEarlyOutHrs
,IsNull(eda.IsManual, 0) IsManual

FROM #EJ

LEFT OUTER JOIN ViewEmployeeInformation emab ON #EJ.EmployeeId = emab.EmployeeId
LEFT OUTER JOIN EmployeeMonthlyOvertime eda ON emab.EmployeeId =ISNULL(eda.EmployeeId,0)
WHERE emab.IsArchive=0 and emab.IsActive=1
AND eda.FiscalYearDetailId = @FiscalYearDetailId
UNION ALL
SELECT 
#EJ.EmployeeId
,Code
,EmpName
,Project
,Department
,Section
,Designation
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,'0'
,0
,0 
,0
,0
,0
FROM #EJ
LEFT OUTER JOIN ViewEmployeeInformation ON #EJ.EmployeeId = ViewEmployeeInformation.EmployeeId
WHERE ViewEmployeeInformation.EmployeeId NOT IN(
SELECT DISTINCT EmployeeId FROM EmployeeMonthlyOvertime
WHERE 1=1 AND FiscalYearDetailId=@FiscalYearDetailId
)
) as a
WHERE 1=1
";
                FiscalYearDAL fdal = new FiscalYearDAL();
                var fname = fdal.FYPeriodDetail(vm.FiscalYearDetailId, currConn, transaction).FirstOrDefault().PeriodName;
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
                sqlText += @" Order By a.DepartmentId, a.ProjectId, a.SectionId, a.EmployeeId
                
drop table #EJ
";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
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
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                da.Fill(dt);

                decimal totalOTInMin = 0;
                decimal min = 0;
                decimal hrs = 0;
                string totalOTInHrs = "0.0";

                foreach (DataRow dr in dt.Rows)
                {
                    totalOTInMin = 0;
                    min = 0;
                    hrs = 0;
                    totalOTInHrs = "0.0";
                    totalOTInMin = Convert.ToDecimal(dr["TotalOvertime"]);
                    min = totalOTInMin % 60;
                    hrs = (totalOTInMin - min) / 60;
                    totalOTInHrs = Convert.ToInt32(hrs) + "." + Convert.ToInt32(min).ToString().PadLeft(2, '0');
                    dr["TotalOvertime"] = totalOTInHrs;
                }

                #endregion
                dt.Columns.Add("Fiscal Period");
                dt.Columns.Add("Type");
                if (dt.Rows.Count == 0)
                {
                    throw new ArgumentNullException("Monthly Overtime has not given to any employee!");
                }
                foreach (DataRow row in dt.Rows)
                {
                    row["Fiscal Period"] = fname;
                    row["Type"] = "EmployeeMonthlyOvertime";
                    row["FiscalYearDetailId"] = vm.FiscalYearDetailId;
                }
                #endregion
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                throw ex;
            }
            return dt;
        }


        public DataTable ExportExcelFileBackup(EmployeeMonthlyOvertimeVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            string[] retResults = new string[6];
            DataTable dt = new DataTable();
            SqlTransaction transaction = null;
            try
            {
                #region Variables
                SqlConnection currConn = null;
                string sqlText = "";
                #endregion
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("ExportExcelFile"); }
                #endregion open connection and transaction
                #region DataRead From DB
                #region sql statement
                sqlText = @"
SELECT * FROM (
SELECT 
emab.EmployeeId
,emab.Code
,emab.EmpName
,emab.Project
,emab.Department
,emab.Section
,emab.Designation
,emab.ProjectId
,emab.DepartmentId
,emab.SectionId
,emab.DesignationId

,eda.FiscalYearDetailId
,ISNULL( eda.TotalOvertime,0)/60  TotalOvertime
,ISNULL( eda.TotalLateInHrs,0)  TotalLateInHrs
,ISNULL( eda.TotalEarlyOutHrs,0)  TotalEarlyOutHrs


FROM EmployeeMonthlyOvertime eda
LEFT OUTER JOIN ViewEmployeeInformationAll emab ON emab.EmployeeId =ISNULL(eda.EmployeeId,0)
WHERE emab.IsArchive=0 and emab.IsActive=1
AND eda.FiscalYearDetailId = @FiscalYearDetailId
UNION ALL
SELECT 
EmployeeId
,Code
,EmpName
,Project
,Department
,Section
,Designation
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,0
,0 
,0
,0
FROM ViewEmployeeInformationAll
WHERE EmployeeId NOT IN(
SELECT DISTINCT EmployeeId FROM EmployeeMonthlyOvertime
WHERE 1=1 AND FiscalYearDetailId=@FiscalYearDetailId)
) as a
WHERE 1=1
";
                FiscalYearDAL fdal = new FiscalYearDAL();
                var fname = fdal.FYPeriodDetail(vm.FiscalYearDetailId, currConn, transaction).FirstOrDefault().PeriodName;
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
                sqlText += " Order By a.DepartmentId, a.ProjectId, a.SectionId, a.EmployeeId";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
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
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                da.Fill(dt);

                #endregion
                dt.Columns.Add("Fiscal Period");
                dt.Columns.Add("Type");
                if (dt.Rows.Count == 0)
                {
                    throw new ArgumentNullException("Monthly Overtime has not given to any employee!");
                }
                foreach (DataRow row in dt.Rows)
                {
                    row["Fiscal Period"] = fname;
                    row["Type"] = "EmployeeMonthlyOvertime";
                    row["FiscalYearDetailId"] = vm.FiscalYearDetailId;
                }
                #endregion
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                throw ex;
            }
            return dt;
        }


        //==================UpdateTotalOvertime =================
        public string[] UpdateTotalOvertime(string FiscalYearDetailId, string MonthlyOTRoundUp, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "UpdateTotalOvertime"; //Method Name
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
--declare @MonthlyOTRoundUp as int
--set @MonthlyOTRoundUp=60
--set @FiscalYearDetailId='1005'
update EmployeeMonthlyOvertime set TotalOvertimeActual=TotalOvertime
where FiscalYearDetailId=@FiscalYearDetailId

update EmployeeMonthlyOvertime set TotalOvertime= (TotalOvertime / @MonthlyOTRoundUp)*@MonthlyOTRoundUp
--update EmployeeMonthlyOvertime set TotalOvertime= TotalOvertime-( TotalOvertime % @MonthlyOTRoundUp)
where TotalOvertime>0
and FiscalYearDetailId=@FiscalYearDetailId

";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdInsert.Parameters.AddWithValue("@MonthlyOTRoundUp", Convert.ToDecimal(MonthlyOTRoundUp));
                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                //if (transResult <= 0)
                //{
                //    retResults[1] = "Unexpected error to Update Attendance Daily New OT!";
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
                retResults[1] = "Employee Monthly Overtime Updated Successfully!";


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

    }
}

