using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymViewModel.Attendance;
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
    public class EmployeeDailyAbsenceDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();

        //==================SelectAll =================
        public List<EmployeeDailyAbsenceVM> SelectAll(string ProjectId, string DepartmentId, string SectionId, string date = "", string tType = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeDailyAbsenceVM> VMs = new List<EmployeeDailyAbsenceVM>();
            EmployeeDailyAbsenceVM vm;
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

--declare @tType as varchar(20)  
--set @tType = 'Absence'
--
--declare @date as varchar(20)  
--set @date = '20171105'

select @StartDate1=periodStart,@EndDate1=PeriodEnd from FiscalYearDetail
where @date between periodStart and PeriodEnd


create table #EJ(EmployeeId  varchar(50),Emptatus varchar(50))
insert into #EJ
    EXEC dbo.ProEmployeeJobs @StartDate = @StartDate1,@EndDate=@EndDate1

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
,eda.TransactionType
,eda.AbsentDate
,eda.Remarks
,'1' IsAbsent
,IsNull(eda.IsManual, 0) IsManual
FROM #EJ
LEFT OUTER JOIN ViewEmployeeInformation emab ON #EJ.EmployeeId = emab.EmployeeId
LEFT OUTER JOIN EmployeeDailyAbsence eda ON emab.EmployeeId =ISNULL( eda.EmployeeId,0)
WHERE emab.IsArchive=0 AND emab.IsActive=1
AND eda.AbsentDate=@date
AND eda.TransactionType=@tType
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
,'0'
,null
,0 
,0
FROM #EJ
LEFT OUTER JOIN ViewEmployeeInformation emab ON #EJ.EmployeeId = emab.EmployeeId
WHERE #EJ.EmployeeId NOT IN(
SELECT DISTINCT EmployeeId FROM EmployeeDailyAbsence
WHERE AbsentDate=@date and TransactionType=@tType)
) AS a
WHERE 1=1


";
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
                objComm.Parameters.AddWithValue("@tType", tType);


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeDailyAbsenceVM();
                    vm.Department = dr["Department"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.TransactionType = tType;
                    vm.IsAbsent = Convert.ToBoolean(dr["IsAbsent"]);
                    vm.IsAbsentPrevious = vm.IsAbsent;
                    vm.IsManual = Convert.ToBoolean(dr["IsManual"]);
                    vm.SearchDate = date;
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
        public string[] Insert(List<EmployeeDailyAbsenceVM> VMs, string AbsentDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Daily Absence"; //Method Name
            int transResult = 0;
            string sqlText = "";
            bool iSTransSuccess = false;

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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                HoliDayDAL _holiDayDAL = new HoliDayDAL();
                var holiDay = _holiDayDAL.SelectByDate(AbsentDate, currConn, transaction).HoliDayType;
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
                sqlText += @" INSERT INTO dbo.EmployeeDailyAbsence
(
EmployeeId
,AbsentDate
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,TransactionType
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
,@AbsentDate
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@TransactionType
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
                        string[] conditionFields = { "EmployeeId", "AbsentDate", "TransactionType" };
                        string[] conditionValues = { item.EmployeeId, Ordinary.DateToString(AbsentDate), item.TransactionType };
                        isExist = _cDal.ExistCheck("EmployeeDailyAbsence", conditionFields, conditionValues, currConn, transaction);
                        if (isExist)
                        {
                            retResults = _cDal.DeleteTable("EmployeeDailyAbsence", conditionFields, conditionValues, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                retResults[1] = "Update Fail!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        if (!item.IsAbsent)
                        {
                            continue;
                        }
                        #endregion CheckPoint
                        //item.Id = _cDal.NextId("EmployeeDailyAbsence", currConn, transaction);
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                        if (item.IsManual != true)
                        {
                            if (item.IsAbsent == item.IsAbsentPrevious)
                            {
                                item.IsManual = false;
                            }
                            else
                            {
                                item.IsManual = true;
                            }
                        }

                        //cmdInsert.Parameters.AddWithValue("@Id", item.Id);
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", item.EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@AbsentDate", Ordinary.DateToString(AbsentDate));
                        cmdInsert.Parameters.AddWithValue("@ProjectId", item.ProjectId);
                        cmdInsert.Parameters.AddWithValue("@DepartmentId", item.DepartmentId);
                        cmdInsert.Parameters.AddWithValue("@SectionId", item.SectionId);
                        cmdInsert.Parameters.AddWithValue("@DesignationId", item.DesignationId);
                        cmdInsert.Parameters.AddWithValue("@TransactionType", item.TransactionType);
                        cmdInsert.Parameters.AddWithValue("@DayStatus", DayStatus);
                        cmdInsert.Parameters.AddWithValue("@IsManual", item.IsManual);

                        cmdInsert.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, holiDayVM.Remarks);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", item.CreatedBy ?? identity.Name);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", item.CreatedAt ?? DateTime.Now.ToString("yyyyMMdd"));
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", item.CreatedFrom ?? identity.WorkStationIP);
                        cmdInsert.ExecuteNonQuery();
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #endregion Insert Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException(" Employee Daily Absence", "Could not found any item.");
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
                    retResults[1] = "Employee Daily Absence Saved Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to Employee Daily Absence Insert.";
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

        //==================Report =================
        public DataTable Report(EmployeeDailyAbsenceVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
,IsNull(eda.AbsentDate, 0) AbsentDate
,TransactionType
FROM  EmployeeDailyAbsence eda 
LEFT OUTER JOIN ViewEmployeeInformationAll ve ON ve.EmployeeId = eda.EmployeeId
WHERE 1=1 
AND ve.IsArchive=0 AND ve.IsActive=1
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
                string[] columnsToChange = { "JoinDate", "AbsentDate" };
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

    }
}
