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
namespace SymServices.Attendance
{
    public class EmployeeMonthlyAbsenceDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();

        //==================SelectAll =================
        public List<EmployeeMonthlyAbsenceVM> SelectAll(string ProjectId, string DepartmentId, string SectionId, int fid = 0)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeMonthlyAbsenceVM> VMs = new List<EmployeeMonthlyAbsenceVM>();
            EmployeeMonthlyAbsenceVM vm;
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
--set @FiscalYearDetailId = '1006'

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
,eda.FiscalYearDetailId
,ISNULL(eda.Remarks, ' ') Remarks
,ISNULL( eda.AbsentDays,0)  AbsentDays
,ISNULL( eda.LateInDays,0)  LateInDays 
,ISNULL( eda.EarlyOutDays,0)  EarlyOutDays
,IsNull(eda.IsManual, 0) IsManual
FROM #EJ
LEFT OUTER JOIN ViewEmployeeInformation emab ON #EJ.EmployeeId = emab.EmployeeId
LEFT OUTER JOIN EmployeeMonthlyAbsence eda ON emab.EmployeeId =ISNULL(eda.EmployeeId,0)
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
,0
,''
,0 
,0
,0 
,0 
FROM #EJ
LEFT OUTER JOIN ViewEmployeeInformation emab ON #EJ.EmployeeId = emab.EmployeeId
WHERE #EJ.EmployeeId NOT IN(
SELECT DISTINCT EmployeeId FROM EmployeeMonthlyAbsence
WHERE 1=1 AND FiscalYearDetailId=@FiscalYearDetailId)
) as a
WHERE 1=1
";
                //LateInDays 
                //EarlyOutDays 
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
                    vm = new EmployeeMonthlyAbsenceVM();
                    vm.Department = dr["Department"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.AbsentDays = Convert.ToDecimal(dr["AbsentDays"]);
                    vm.LateInDays = Convert.ToDecimal(dr["LateInDays"]);
                    vm.EarlyOutDays = Convert.ToDecimal(dr["EarlyOutDays"]);
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.SearchFid = fid;
                    //AbsentDays
                    //LateInDays
                    //EarlyOutDays
                    //////////Previous

                    vm.AbsentDaysPrevious = vm.AbsentDays;
                    vm.LateInDaysPrevious = vm.LateInDays;
                    vm.EarlyOutDaysPrevious = vm.EarlyOutDays;
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

        //==================InsertFromDailyAbsence =================
        public string[] InsertFromDailyAbsence(string FiscalYearDetailId, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

--set @FiscalYearDetailId='1002'

SELECT @StartDate=periodStart,@EndDate=PeriodEnd from FiscalYearDetail
WHERE id=@FiscalYearDetailId
DELETE from EmployeeMonthlyAbsence
WHERE FiscalYearDetailId=@FiscalYearDetailId

 insert into EmployeeMonthlyAbsence(EmployeeId,FiscalYearDetailId
,ProjectId,DepartmentId,SectionId,DesignationId
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,AbsentDays)

 select distinct  EmployeeDailyAbsence.EmployeeId,@FiscalYearDetailId,
  EmployeeJob.ProjectId,EmployeeJob.DepartmentId,EmployeeJob.SectionId,EmployeeJob.DesignationId
,'-',@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
,Count(AbsentDate) AbsentDays 
  
 from EmployeeDailyAbsence left outer join EmployeeJob on EmployeeDailyAbsence.EmployeeId=EmployeeJob.EmployeeId
where AbsentDate between @StartDate and @EndDate
group by EmployeeDailyAbsence.EmployeeId,
  EmployeeJob.ProjectId,EmployeeJob.DepartmentId,EmployeeJob.SectionId,EmployeeJob.DesignationId

--select * from EmployeeMonthlyAbsence
--where FiscalYearDetailId=@FiscalYearDetailId
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
                    retResults[1] = "Unexpected error to Employee Monthly Absence Insert!";
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
                retResults[1] = "Employee Monthly Absence Saved Successfully!";


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
        public string[] Insert(List<EmployeeMonthlyAbsenceVM> VMs, string FiscalYearDetailId, ShampanIdentityVM auditvm = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                sqlText += @" INSERT INTO dbo.EmployeeMonthlyAbsence
(
EmployeeId
,FiscalYearDetailId
,AbsentDays
,LateInDays 
,EarlyOutDays
,ProjectId
,DepartmentId
,SectionId
,DesignationId
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
,@FiscalYearDetailId
,@AbsentDays
,@LateInDays 
,@EarlyOutDays
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@IsManual
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)";

                //LateInDays 
                //EarlyOutDays 

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                if (VMs.Count >= 1)
                {
                    #region Update Settings
                    foreach (var item in VMs)
                    {
                        #region CheckPoint
                        bool isExist = false;
                        //if this employee in this month absent days alredy exist then delete first then insert
                        string[] conditionFields = { "EmployeeId", "FiscalYearDetailId" };
                        string[] conditionValues = { item.EmployeeId, FiscalYearDetailId };
                        isExist = _cDal.ExistCheck("EmployeeMonthlyAbsence", conditionFields, conditionValues, currConn, transaction);
                        if (isExist)
                        {
                            retResults = _cDal.DeleteTable("EmployeeMonthlyAbsence", conditionFields, conditionValues, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                retResults[1] = "Update Fail!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        //if ((item.AbsentDays <= 0 || item.AbsentDays > 25) && (item.LateInDays <= 0 || item.LateInDays > 25) && (item.EarlyOutDays <= 0 || item.EarlyOutDays > 25))
                        //{
                        //    continue;
                        //}
                        #endregion CheckPoint

                        if (item.IsManual != true)
                        {
                            if (item.AbsentDays == item.AbsentDaysPrevious && item.LateInDays == item.LateInDaysPrevious && item.EarlyOutDays == item.EarlyOutDaysPrevious)
                            {
                                item.IsManual = false;
                            }
                            else
                            {
                                item.IsManual = true;
                            }
                        }


                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                        cmdInsert.Parameters.AddWithValue("@EmployeeId", item.EmployeeId.ToString());
                        cmdInsert.Parameters.AddWithValue("@AbsentDays", Convert.ToDecimal(item.AbsentDays));
                        cmdInsert.Parameters.AddWithValue("@LateInDays", Convert.ToDecimal(item.LateInDays));
                        cmdInsert.Parameters.AddWithValue("@EarlyOutDays", Convert.ToDecimal(item.EarlyOutDays));
                        cmdInsert.Parameters.AddWithValue("@ProjectId", item.ProjectId);
                        cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                        cmdInsert.Parameters.AddWithValue("@DepartmentId", item.DepartmentId);
                        cmdInsert.Parameters.AddWithValue("@SectionId", item.SectionId);
                        cmdInsert.Parameters.AddWithValue("@DesignationId", item.DesignationId);
                        cmdInsert.Parameters.AddWithValue("@IsManual", item.IsManual);
                        cmdInsert.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, holiDayVM.Remarks);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy ?? "Admin");
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt ?? "19900101");
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom ?? "Local");
                        var exec = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exec);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #endregion Insert Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Employee Monthly Absence", "Could not found any item.");
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
                    retResults[1] = "Employee Monthly Absence Saved Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to Employee Monthly Absence Insert.";
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
        public DataTable Report(EmployeeMonthlyAbsenceVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
,'1.00' AbsentDeduction
,ISNULL(fyd.PeriodName,0) PeriodName
,ISNULL(fyd.PeriodStart,0) PeriodStart
,ISNULL(ema.FiscalYearDetailId, 0) FiscalYearDetailId
,ISNULL(ema.AbsentDays,0)  AbsentDays
,ISNULL(ema.LateInDays,0)  LateInDays
,ISNULL(ema.EarlyOutDays,0)  EarlyOutDays



FROM ViewEmployeeInformationAll ve
LEFT OUTER JOIN EmployeeMonthlyAbsence ema ON ve.EmployeeId =ISNULL(ema.EmployeeId,0)
LEFT OUTER JOIN FiscalYearDetail fyd ON ema.FiscalYearDetailId = fyd.Id
WHERE 1=1
AND ve.IsArchive=0 and ve.IsActive=1
AND (ema.AbsentDays > 0 OR ema.LateInDays > 0 OR ema.EarlyOutDays > 0)
AND ema.FiscalYearDetailId = @FiscalYearDetailId
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
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "JoinDate");

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
                List<EmployeeMonthlyAbsenceVM> VMs = new List<EmployeeMonthlyAbsenceVM>();
                EmployeeMonthlyAbsenceVM vm = new EmployeeMonthlyAbsenceVM();

                foreach (DataRow item in dt.Rows)
                {
                    #region CheckPoint
                    if (!Ordinary.IsNumeric(item["AbsentDays"].ToString()))
                    {
                        retResults[1] = "Please input the Numeric Value in Absent Days!";
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    if (Convert.ToDecimal(item["AbsentDays"]) < 0)
                    {
                        //continue;
                    }
                    #endregion CheckPoint
                    #region Read Data
                    vm = new EmployeeMonthlyAbsenceVM();
                    vm.EmployeeId = item["EmployeeId"].ToString();
                    vm.Code = item["Code"].ToString();
                    vm.FiscalYearDetailId = Convert.ToInt32(item["FiscalYearDetailId"]);
                    vm.AbsentDays = Convert.ToDecimal(item["AbsentDays"]);
                    vm.LateInDays = Convert.ToDecimal(item["LateInDays"]);
                    vm.EarlyOutDays = Convert.ToDecimal(item["EarlyOutDays"]);
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
                string FiscalYearDetailId = VMs.FirstOrDefault().FiscalYearDetailId.ToString();
                retResults = Insert(VMs, FiscalYearDetailId, auditvm, VcurrConn, Vtransaction);
                if (retResults[0] == "Fail")
                {
                    retResults[1] = "Employee Monthly Absence Insert - Unexpected Error!";
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
        public DataTable ExportExcelFile(EmployeeMonthlyAbsenceVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--set @FiscalYearDetailId = '1006'

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
,eda.FiscalYearDetailId
,ISNULL(eda.Remarks, ' ') Remarks
,ISNULL( eda.AbsentDays,0)  AbsentDays
,ISNULL( eda.LateInDays,0)  LateInDays 
,ISNULL( eda.EarlyOutDays,0)  EarlyOutDays
,IsNull(eda.IsManual, 0) IsManual
FROM #EJ
LEFT OUTER JOIN ViewEmployeeInformation emab ON #EJ.EmployeeId = emab.EmployeeId
LEFT OUTER JOIN EmployeeMonthlyAbsence eda ON emab.EmployeeId =ISNULL(eda.EmployeeId,0)
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
,0
,''
,0 
,0
,0 
,0 
FROM #EJ
LEFT OUTER JOIN ViewEmployeeInformation emab ON #EJ.EmployeeId = emab.EmployeeId
WHERE #EJ.EmployeeId NOT IN(
SELECT DISTINCT EmployeeId FROM EmployeeMonthlyAbsence
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
                #endregion
                dt.Columns.Add("Fiscal Period");
                dt.Columns.Add("Type");
                if (dt.Rows.Count == 0)
                {
                    throw new ArgumentNullException("Monthly Absence has not given to any employee!");
                }
                foreach (DataRow row in dt.Rows)
                {
                    row["Fiscal Period"] = fname;
                    row["Type"] = "EmployeeMonthlyAbsence";
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
    }
}
