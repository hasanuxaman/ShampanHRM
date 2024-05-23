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
using System.IO;
using Excel;
using SymViewModel.Common;

namespace SymServices.Attendance
{
    public class EmployeeDailyOvertimeDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();

        //==================SelectAll =================
        public List<EmployeeDailyOvertimeVM> SelectAll(string ProjectId, string DepartmentId, string SectionId, string date = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeDailyOvertimeVM> VMs = new List<EmployeeDailyOvertimeVM>();
            EmployeeDailyOvertimeVM vm;
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

--declare @date as varchar(20)  
--set @date = '20171105'

select @StartDate1=periodStart,@EndDate1=PeriodEnd from FiscalYearDetail
where @date between periodStart and PeriodEnd


create table #EJ(EmployeeId  varchar(50),Emptatus varchar(50))
insert into #EJ
    EXEC dbo.ProEmployeeJobs @StartDate = @StartDate1,@EndDate=@EndDate1



SELECT * FROM (
SELECT 
#EJ.EmployeeId
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
,edo.Remarks
,ISNULL( edo.Overtime,0)  Overtime
,ISNULL( edo.LateInHrs,0)  LateInHrs
,ISNULL( edo.EarlyOutHrs,0)  EarlyOutHrs
,IsNull(edo.IsManual, 0) IsManual
FROM #EJ

LEFT OUTER JOIN ViewEmployeeInformation emab ON #EJ.EmployeeId = emab.EmployeeId
LEFT OUTER JOIN EmployeeDailyOvertime edo ON emab.EmployeeId =ISNULL( edo.EmployeeId,0)
WHERE emab.IsArchive=0 AND emab.IsActive=1
AND edo.OvertimeDate=@date
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
FROM #EJ
LEFT OUTER JOIN ViewEmployeeInformation emab ON #EJ.EmployeeId = emab.EmployeeId
WHERE #EJ.EmployeeId NOT IN(
SELECT DISTINCT EmployeeId FROM EmployeeDailyOvertime
WHERE OvertimeDate=@date
)
) AS a
WHERE 1=1
";
                //LateInHrs
                //EarlyOutHrs
                if (ProjectId != "0_0")
                    sqlText += " AND a.ProjectId=@ProjectId";
                if (DepartmentId != "0_0")
                    sqlText += " AND a.DepartmentId=@DepartmentId ";
                if (SectionId != "0_0")
                    sqlText += " AND a.SectionId=@SectionId ";

                sqlText += @" Order By a.DepartmentId, a.ProjectId, a.SectionId, a.EmployeeId
drop table #EJ
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (ProjectId != "0_0")
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);

                if (DepartmentId != "0_0")
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);

                if (SectionId != "0_0")
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                objComm.Parameters.AddWithValue("@date", Ordinary.DateToString(date));

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeDailyOvertimeVM();
                    vm.Department = dr["Department"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    decimal OTInMin = Convert.ToDecimal(dr["Overtime"]);
                    var min = OTInMin % 60;
                    var hrs = (OTInMin - min) / 60;
                    var OTInHrs = Convert.ToInt32(hrs) + "." + Convert.ToInt32(min).ToString().PadLeft(2, '0');
                    vm.Overtime = Convert.ToDecimal(OTInHrs);
                    vm.LateInHrs = Convert.ToDecimal(dr["LateInHrs"]);
                    vm.EarlyOutHrs = Convert.ToDecimal(dr["EarlyOutHrs"]);
                    vm.SearchDate = date;
                    vm.OvertimePrevious = vm.Overtime;
                    vm.LateInHrsPrevious = vm.LateInHrs;
                    vm.EarlyOutHrsPrevious = vm.EarlyOutHrs;

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
        //==================Insert =================
        public string[] Insert(List<EmployeeDailyOvertimeVM> VMs, string OTDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Daily Overtime"; //Method Name
            int transResult = 0;
            string sqlText = "";
            bool iSTransSuccess = false;

            decimal OTHrs = 0;
            decimal OTMins = 0;

            decimal LateInHrs = 0;
            decimal LateInMins = 0;

            decimal EarlyOutHrs = 0;
            decimal EarlyOutMins = 0;

            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool isExist = false;
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

                HoliDayDAL _holiDayDAL = new HoliDayDAL();
                var holiDay = _holiDayDAL.SelectByDate(OTDate, currConn, transaction).HoliDayType;
                string DayStatus = "";
                if (holiDay == "Weekly")
                {
                    DayStatus = "WH";
                }
                else if (holiDay == "Govt")
                {
                    DayStatus = "GH";
                }
                else if (holiDay == "Festival")
                {
                    DayStatus = "FH";
                }
                else if (holiDay == "Special")
                {
                    DayStatus = "SH";
                }
                else
                {
                    DayStatus = "R";
                }
                sqlText = "  ";
                sqlText += @" INSERT INTO dbo.EmployeeDailyOvertime
(
EmployeeId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,OvertimeDate
,Overtime
,LateInHrs
,EarlyOutHrs
,LateInMins
,EarlyOutMins
,DayStatus
,IsManual
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
)
VALUES
(
@EmployeeId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@OvertimeDate
,@Overtime
,@LateInHrs
,@EarlyOutHrs
,@LateInMins
,@EarlyOutMins
,@DayStatus
,@IsManual
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)";
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                if (VMs.Count >= 1)
                {
                    #region Update Settings
                    foreach (var item in VMs)
                    {
                        #region CheckPoint
                        if (!Ordinary.IsNumeric(item.Overtime.ToString()))
                        {
                            retResults[1] = "Please input the Numeric Value in Amount!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        //if (item.Overtime <= 0 && item.LateInHrs <= 0 && item.EarlyOutHrs <= 0)
                        //{
                        //    continue;
                        //}



                        string[] conditionFields = { "EmployeeId", "OvertimeDate" };
                        string[] conditionValues = { item.EmployeeId, Ordinary.DateToString(OTDate) };
                        isExist = _cDal.ExistCheck("EmployeeDailyOvertime", conditionFields, conditionValues, currConn, transaction);
                        if (isExist)
                        {
                            retResults = _cDal.DeleteTable("EmployeeDailyOvertime", conditionFields, conditionValues, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                retResults[1] = "Update Fail!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }

                        #endregion CheckPoint
                        #region Hour-Min Conversion
                        
                        //OTHrs = decimal.Truncate(item.Overtime);
                        decimal time = item.Overtime;
                        int hrs = Convert.ToInt32(Math.Floor(time));
                        int min = Convert.ToInt32((Convert.ToDecimal(time) - Convert.ToDecimal(hrs)) * 100);
                        item.Overtime = hrs * 60 + min;

                        time = item.LateInHrs;
                        hrs = Convert.ToInt32(Math.Floor(time));
                        min = Convert.ToInt32((Convert.ToDecimal(time) - Convert.ToDecimal(hrs)) * 100);
                        item.LateInMins = hrs * 60 + min;

                        //LateInHrs = decimal.Truncate(item.LateInHrs);
                        //LateInMins = item.LateInHrs - LateInHrs;
                        //item.LateInMins = LateInHrs * 60 + LateInMins;

                        time = item.EarlyOutHrs;
                        hrs = Convert.ToInt32(Math.Floor(time));
                        min = Convert.ToInt32((Convert.ToDecimal(time) - Convert.ToDecimal(hrs)) * 100);
                        item.EarlyOutMins = hrs * 60 + min;

                        //EarlyOutHrs = decimal.Truncate(item.EarlyOutHrs);
                        //EarlyOutMins = item.EarlyOutHrs - EarlyOutHrs;
                        //item.EarlyOutMins = EarlyOutHrs * 60 + EarlyOutMins;

                        
                        time = item.OvertimePrevious;
                        hrs = Convert.ToInt32(Math.Floor(time));
                        min = Convert.ToInt32((Convert.ToDecimal(time) - Convert.ToDecimal(hrs)) * 100);
                        item.OvertimePrevious = hrs * 60 + min;
                        #endregion


                        if (item.IsManual != true)
                        {
                            if (item.Overtime == item.OvertimePrevious && item.LateInHrs == item.LateInHrsPrevious && item.EarlyOutHrs == item.EarlyOutHrsPrevious)
                            {
                                item.IsManual = false;
                            }
                            else
                            {
                                item.IsManual = true;
                            }
                        }
                        //LateInHrs
                        //EarlyOutHrs
                        #region Sql Execution
                        
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", item.EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@ProjectId", item.ProjectId);
                        cmdInsert.Parameters.AddWithValue("@DepartmentId", item.DepartmentId);
                        cmdInsert.Parameters.AddWithValue("@SectionId", item.SectionId);
                        cmdInsert.Parameters.AddWithValue("@DesignationId", item.DesignationId);
                        cmdInsert.Parameters.AddWithValue("@OvertimeDate", Ordinary.DateToString(OTDate));
                        cmdInsert.Parameters.AddWithValue("@Overtime", item.Overtime);
                        cmdInsert.Parameters.AddWithValue("@LateInHrs", item.LateInHrs);
                        cmdInsert.Parameters.AddWithValue("@EarlyOutHrs", item.EarlyOutHrs);

                        cmdInsert.Parameters.AddWithValue("@LateInMins", item.LateInMins);
                        cmdInsert.Parameters.AddWithValue("@EarlyOutMins", item.EarlyOutMins);
                        cmdInsert.Parameters.AddWithValue("@DayStatus", DayStatus);
                        cmdInsert.Parameters.AddWithValue("@IsManual", item.IsManual);


                        cmdInsert.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, holiDayVM.Remarks);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", item.CreatedBy ?? identity.Name);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", item.CreatedAt ?? DateTime.Now.ToString("yyyyMMdd"));
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", item.CreatedFrom ?? identity.WorkStationIP);
                        var exe = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exe);
                        if (transResult <= 0)
                        {
                            retResults[1] = "Unexpected error to Employee Daily Overtime Insert!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion

                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #endregion Insert Settings
                    iSTransSuccess = true;
                }
                else
                {
                    retResults[1] = "Employee Daily Overtime - Could not found any item!";
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
                    retResults[1] = "Employee Daily Overtime Saved Successfully!";
                }
                else
                {
                    retResults[1] = "Unexpected error to Employee Daily Overtime Insert!";
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

        //==================Report =================
        public DataTable Report(EmployeeDailyOvertimeVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
,ISNULL( edo.Overtime,0)/60  Overtime
,ISNULL( edo.LateInHrs,0)  LateInHrs
,ISNULL( edo.EarlyOutHrs,0)  EarlyOutHrs
,IsNull(edo.OvertimeDate, 0) OvertimeDate
,'1.00' OTRate
FROM ViewEmployeeInformationAll ve
LEFT OUTER JOIN EmployeeDailyOvertime edo ON ve.EmployeeId =edo.EmployeeId
WHERE 1=1 
AND ve.IsArchive=0 AND ve.IsActive=1
AND edo.Overtime >= 0
";
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
                sqlText += " Order By ve.DepartmentId, ve.ProjectId, ve.SectionId, ve.EmployeeId";
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

                da.Fill(dt);
                string[] columnsToChange = { "JoinDate", "OvertimeDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, columnsToChange);

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

        //==================ImportExcelFile =================
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
                List<EmployeeDailyOvertimeVM> VMs = new List<EmployeeDailyOvertimeVM>();
                EmployeeDailyOvertimeVM vm = new EmployeeDailyOvertimeVM();



                foreach (DataRow item in dt.Rows)
                {
                    #region CheckPoint
                    if (!Ordinary.IsNumeric(item["Overtime"].ToString()) || !Ordinary.IsNumeric(item["LateInHrs"].ToString()) || !Ordinary.IsNumeric(item["EarlyOutHrs"].ToString()))
                    {
                        retResults[1] = "Please input the Numeric Value in Amount!";
                        return retResults;
                    }
                    if (Convert.ToDecimal(item["Overtime"]) < 0 || Convert.ToDecimal(item["LateInHrs"]) < 0 || Convert.ToDecimal(item["EarlyOutHrs"]) < 0)
                    {
                        retResults[1] = "Please input the Non-Negative Value in Amount!";
                        return retResults;
                    }
                    //if (!Ordinary.IsTimeFormat(item["Overtime"].ToString()) || !Ordinary.IsTimeFormat(item["LateInHrs"].ToString()) || !Ordinary.IsTimeFormat(item["EarlyOutHrs"].ToString()))
                    //{
                    //    retResults[1] = "Please input the Numeric Value in Time Format!";
                    //    return retResults;
                    //}


                    #endregion CheckPoint
                    #region Read Data
                    vm = new EmployeeDailyOvertimeVM();
                    vm.EmployeeId = item["EmployeeId"].ToString();
                    vm.Code = item["Code"].ToString();
                    vm.OvertimeDate = item["SearchDate"].ToString();
                    vm.Overtime = Convert.ToDecimal(item["Overtime"]);
                    vm.LateInHrs = Convert.ToDecimal(item["LateInHrs"]);
                    vm.EarlyOutHrs = Convert.ToDecimal(item["EarlyOutHrs"]);

                    vm.ProjectId = item["ProjectId"].ToString();
                    vm.DepartmentId = item["DepartmentId"].ToString();
                    vm.SectionId = item["SectionId"].ToString();
                    vm.DesignationId = item["DesignationId"].ToString();
                    vm.IsManual = true;
                    //vm.LastUpdateAt = auditvm.LastUpdateAt;
                    //vm.LastUpdateBy = auditvm.LastUpdateBy;
                    //vm.LastUpdateFrom = auditvm.LastUpdateFrom;
                    vm.CreatedAt = auditvm.CreatedAt;
                    vm.CreatedBy = auditvm.CreatedBy;
                    vm.CreatedFrom = auditvm.CreatedFrom;
                    VMs.Add(vm);
                    #endregion Read Data
                }
                #region Insert Data
                string OTDate = VMs.FirstOrDefault().OvertimeDate;
                retResults = Insert(VMs, OTDate, VcurrConn, Vtransaction);
                if (retResults[0] == "Fail")
                {
                    retResults[1] = "Employee Daily Overtime - Could not found any item!";
                    throw new ArgumentNullException(retResults[1], "");
                }
                #endregion Insert Data
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

        //==================ExportExcelFile =================
        public DataTable ExportExcelFile(EmployeeDailyOvertimeVM vm, string[] conditionFields = null, string[] conditionValues = null)
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

--declare @date as varchar(20)  
--set @date = '20171105'

select @StartDate1=periodStart,@EndDate1=PeriodEnd from FiscalYearDetail
where @date between periodStart and PeriodEnd


create table #EJ(EmployeeId  varchar(50),Emptatus varchar(50))
insert into #EJ
    EXEC dbo.ProEmployeeJobs @StartDate = @StartDate1,@EndDate=@EndDate1



SELECT * FROM (
SELECT 
#EJ.EmployeeId
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
,edo.Remarks
,ISNULL( edo.Overtime,0)  Overtime
,ISNULL( edo.LateInHrs,0)  LateInHrs
,ISNULL( edo.EarlyOutHrs,0)  EarlyOutHrs
,IsNull(edo.IsManual, 0) IsManual
FROM #EJ

LEFT OUTER JOIN ViewEmployeeInformation emab ON #EJ.EmployeeId = emab.EmployeeId
LEFT OUTER JOIN EmployeeDailyOvertime edo ON emab.EmployeeId =ISNULL( edo.EmployeeId,0)
WHERE emab.IsArchive=0 AND emab.IsActive=1
AND edo.OvertimeDate=@date
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
FROM #EJ
LEFT OUTER JOIN ViewEmployeeInformation emab ON #EJ.EmployeeId = emab.EmployeeId
WHERE #EJ.EmployeeId NOT IN(
SELECT DISTINCT EmployeeId FROM EmployeeDailyOvertime
WHERE OvertimeDate=@date
)
) AS a
WHERE 1=1
";
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
                da.SelectCommand.Parameters.AddWithValue("@date", Ordinary.DateToString(vm.OvertimeDate));
                da.Fill(dt);

                decimal OTInMin = 0;
                decimal min = 0;
                decimal hrs = 0;
                string OTInHrs = "0.0";

                foreach (DataRow dr in dt.Rows)
                {
                    OTInMin = 0;
                    min = 0;
                    hrs = 0;
                    OTInHrs = "0.0";
                    OTInMin = Convert.ToDecimal(dr["Overtime"]);
                    min = OTInMin % 60;
                    hrs = (OTInMin - min) / 60;
                    OTInHrs = Convert.ToInt32(hrs) + "." + Convert.ToInt32(min).ToString().PadLeft(2, '0');
                    dr["Overtime"] = OTInHrs;
                }

                #endregion
                dt.Columns.Add("SearchDate");
                dt.Columns.Add("Type");
                if (dt.Rows.Count == 0)
                {
                    throw new ArgumentNullException("Daily Overtime has not given to any employee!");
                }
                foreach (DataRow row in dt.Rows)
                {
                    row["Type"] = "EmployeeDailyOvertime";
                    row["SearchDate"] = vm.OvertimeDate;
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

    }
}
