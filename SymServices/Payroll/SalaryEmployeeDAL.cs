using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Payroll;

namespace SymServices.Payroll
{
    public class SalaryEmployeeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        public List<SalaryEmployeeVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryEmployeeVM> VMs = new List<SalaryEmployeeVM>();
            SalaryEmployeeVM vm;
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

   FROM SalaryEmployee 
WHERE  1=1 AND Archive = 0
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryEmployeeVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    //////vm.Name = dr["Name"].ToString();
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

        //==================SelectAll=================
        public List<SalaryEmployeeVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SalaryEmployeeVM> VMs = new List<SalaryEmployeeVM>();
            SalaryEmployeeVM vm;
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
                #region SqlText

                sqlText = @"
SELECT
se.Id

,ve.Code
,ve.EmpName
,ve.Project
,ve.Department
,ve.Section
,ve.Designation

,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd

,se.EmployeeId
,se.ProjectId
,se.DepartmentId
,se.SectionId
,se.DesignationId
,se.FiscalYearDetailId
,se.EmployeeStatus
,se.GradeId
,se.IsHold
,se.HoldBy
,se.HoldAt
,se.UnHoldBy
,se.UnHoldAt
,se.Remarks
,se.IsActive
,se.IsArchive
,se.CreatedBy
,se.CreatedAt
,se.CreatedFrom
,se.LastUpdateBy
,se.LastUpdateAt
,se.LastUpdateFrom


FROM SalaryEmployee  se
LEFT OUTER JOIN ViewEmployeeInformation ve ON se.EmployeeId = ve.EmployeeId
LEFT OUTER JOIN FiscalYearDetail fyd ON se.FiscalYearDetailId = fyd.Id
WHERE  1=1 AND se.IsArchive = 0

";


                if (Id > 0)
                {
                    sqlText += @" and se.Id=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i]=="0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j]=="0")
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

                    vm = new SalaryEmployeeVM();
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
                    vm.ProjectId = Convert.ToString(dr["ProjectId"]);
                    vm.DepartmentId = Convert.ToString(dr["DepartmentId"]);
                    vm.SectionId = Convert.ToString(dr["SectionId"]);
                    vm.DesignationId = Convert.ToString(dr["DesignationId"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.EmployeeStatus = Convert.ToString(dr["EmployeeStatus"]);
                    vm.GradeId = Convert.ToString(dr["GradeId"]);
                    vm.IsHold = Convert.ToBoolean(dr["IsHold"]);
                    vm.HoldBy = Convert.ToString(dr["HoldBy"]);
                    vm.HoldAt = Convert.ToString(dr["HoldAt"]);
                    vm.UnHoldBy = Convert.ToString(dr["UnHoldBy"]);
                    vm.UnHoldAt = Convert.ToString(dr["UnHoldAt"]);
                    vm.Remarks = Convert.ToString(dr["Remarks"]);
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedBy = Convert.ToString(dr["CreatedBy"]);
                    vm.CreatedAt = Convert.ToString(dr["CreatedAt"]);
                    vm.CreatedFrom = Convert.ToString(dr["CreatedFrom"]);
                    vm.LastUpdateBy = Convert.ToString(dr["LastUpdateBy"]);
                    vm.LastUpdateAt = Convert.ToString(dr["LastUpdateAt"]);
                    vm.LastUpdateFrom = Convert.ToString(dr["LastUpdateFrom"]);

                    VMs.Add(vm);
                }
                dr.Close();

                #endregion SqlExecution

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
        //==================Insert =================
        public string[] InsertDetail(SalaryEmployeeVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertSalaryEmployee"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion
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
                #region Save

                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    #region sqlText/Comments
                    sqlText = @" 
----DECLARE @IsActive		AS bit
----DECLARE @IsArchive		AS bit
----DECLARE @CreatedBy		AS nvarchar(20)
----DECLARE @CreatedAt		AS nvarchar(14)
----DECLARE @CreatedFrom	AS varchar(50)	

----SET @IsActive	 = '1'
----SET @IsArchive	 = '0'
----SET @CreatedBy	 = 'Admin'
----SET @CreatedAt	 = '19000101'
----SET @CreatedFrom = 'Local'
";
                    #endregion

                    sqlText += @" 
----DECLARE @FiscalYearDetailId AS int
----SET @FiscalYearDetailId = 1018

---------------------------------------------------------------------------------------
----------------------------------------Delete Existing Data---------------------------
DELETE SalaryEmployee 
WHERE 1=1 
AND FiscalYearDetailId=@FiscalYearDetailId
AND EmployeeId=@EmployeeId
AND ISNULL(IsManual,0) = 0

---------------------------------------------------------------------------------------
----------------------------------------RESEED Identity--------------------------------
DECLARE @MaxId as int
SELECT @MaxId = ISNULL(MAX(Id),0)  FROM SalaryEmployee
DBCC CHECKIDENT ('[SalaryEmployee]', RESEED, @MaxId);


---------------------------------------------------------------------------------------
---------------------------------------Insert New Data---------------------------------
INSERT INTO SalaryEmployee
(
 [EmployeeId]
,[ProjectId]
,[DepartmentId]
,[SectionId]
,[DesignationId]
,[FiscalYearDetailId]
,[EmployeeStatus]
,[GradeId]
,[StepId]
,[IsHold]
,[HoldBy]
,[HoldAt]
,[UnHoldBy]
,[UnHoldAt]
,[IsManual]
,[Other1]
,[Other2]
,[Other3]
,[Remarks]
,[IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]

)

SELECT
DISTINCT
sed.EmployeeId
,sed.ProjectId
,sed.DepartmentId
,sed.SectionId
,sed.DesignationId
,sed.FiscalYearDetailId
,sed.EmployeeStatus
,esg.GradeId
,esg.StepId

,'0'IsHold,'0'HoldBy,'0'HoldAt,'0'UnHoldBy,'0'UnHoldAt, '0' IsManual
, ejb.Other1, ejb.Other2, ejb.Other3
,'NA' Remarks,@IsActive ,@IsArchive ,@CreatedBy ,@CreatedAt ,@CreatedFrom ,'' LastUpdateBy,'' LastUpdateAt,'' LastUpdateFrom
FROM SalaryEarningDetail sed
left outer join EmployeeJob ejb on sed.EmployeeId=ejb.EmployeeId
left outer join EmployeeStructureGroup esg on sed.EmployeeId=esg.EmployeeId

WHERE 1=1
AND sed.FiscalYearDetailId=@FiscalYearDetailId
AND sed.EmployeeId=@EmployeeId
AND sed.EmployeeId NOT IN 
(
SELECT EmployeeId FROM SalaryEmployee
WHERE 1=1
AND FiscalYearDetailId=@FiscalYearDetailId
AND ISNULL(IsManual,0) = 1
)

update SalaryEmployee set StructureBasic=SalaryEarningDetail.Amount
from (select EmployeeId,FiscalYearDetailId, Amount   FROM SalaryEarningDetail
where SalaryType='basic') SalaryEarningDetail
where SalaryEmployee.EmployeeId=SalaryEarningDetail.EmployeeId
and SalaryEmployee.FiscalYearDetailId=SalaryEarningDetail.FiscalYearDetailId
AND SalaryEmployee.FiscalYearDetailId=@FiscalYearDetailId


update SalaryEmployee set StructureHouseRent=SalaryEarningDetail.Amount
from (select EmployeeId,FiscalYearDetailId, Amount   FROM SalaryEarningDetail
where SalaryType='houserent') SalaryEarningDetail
where SalaryEmployee.EmployeeId=SalaryEarningDetail.EmployeeId
and SalaryEmployee.FiscalYearDetailId=SalaryEarningDetail.FiscalYearDetailId
AND SalaryEmployee.FiscalYearDetailId=@FiscalYearDetailId

update SalaryEmployee set StructureMedical=SalaryEarningDetail.Amount
from (select EmployeeId,FiscalYearDetailId, Amount   FROM SalaryEarningDetail
where SalaryType='medical') SalaryEarningDetail
where SalaryEmployee.EmployeeId=SalaryEarningDetail.EmployeeId
and SalaryEmployee.FiscalYearDetailId=SalaryEarningDetail.FiscalYearDetailId
AND SalaryEmployee.FiscalYearDetailId=@FiscalYearDetailId

update SalaryEmployee set StructureTA=SalaryEarningDetail.Amount
from (select EmployeeId,FiscalYearDetailId, Amount   FROM SalaryEarningDetail
where SalaryType='Conveyance') SalaryEarningDetail
where SalaryEmployee.EmployeeId=SalaryEarningDetail.EmployeeId
and SalaryEmployee.FiscalYearDetailId=SalaryEarningDetail.FiscalYearDetailId
AND SalaryEmployee.FiscalYearDetailId=@FiscalYearDetailId



";

                    #endregion SqlText
                    #region SqlExecution
                    if (string.IsNullOrWhiteSpace(vm.EmployeeId))
                    {
                        sqlText = sqlText.Replace("sed.EmployeeId=@EmployeeId", "1=1");
                        sqlText = sqlText.Replace("EmployeeId=@EmployeeId", "1=1");
                    }

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    if (!string.IsNullOrWhiteSpace(vm.EmployeeId))
                    {
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    }


                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    //////if (transResult <= 0)
                    //////{
                    //////    retResults[3] = sqlText;
                    //////    throw new ArgumentNullException("Unexpected error to update SalaryEmployee.", "");
                    //////}
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This SalaryEmployee already used!";
                    throw new ArgumentNullException("Please Input SalaryEmployee Value", "");
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
                retResults[2] = vm.Id.ToString();
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


        public string[] DeleteDetail(SalaryEmployeeVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertSalaryEmployee"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion
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
                #region Save

                if (vm != null)
                {

                    #region SqlText
                    sqlText = "  ";

                    sqlText += @" 
----DECLARE @FiscalYearDetailId AS int
----SET @FiscalYearDetailId = 1018

DELETE SalaryEmployee 
WHERE 1=1 
AND FiscalYearDetailId=@FiscalYearDetailId
AND EmployeeId=@EmployeeId

";

                    #endregion SqlText
                    #region SqlExecution
                    if (string.IsNullOrWhiteSpace(vm.EmployeeId))
                    {
                        sqlText = sqlText.Replace("EmployeeId=@EmployeeId", "1=1");
                    }

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    if (!string.IsNullOrWhiteSpace(vm.EmployeeId))
                    {
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    }


                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This SalaryEmployee already used!";
                    throw new ArgumentNullException("Please Input SalaryEmployee Value", "");
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
                retResults[1] = "Data Delete Successfully.";
                retResults[2] = vm.Id.ToString();
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
        public string[] Update(List<SalaryEmployeeVM> VMs, SalaryEmployeeVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                    sqlText = "UPDATE SalaryEmployee SET";

                    sqlText += "   Remarks=@Remarks";

                    sqlText += " , IsHold=@IsHold";
                    sqlText += " , HoldBy=@HoldBy";
                    sqlText += " , HoldAt=@HoldAt";
                    sqlText += " , UnHoldBy=@UnHoldBy";
                    sqlText += " , UnHoldAt=@UnHoldAt";
                    sqlText += " , IsManual=1";

                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    foreach (SalaryEmployeeVM vm in VMs)
                    {
                        #region Hold-Unhold Audit
                        if (vm.IsHold)
                        {
                            vm.HoldBy = paramVM.LastUpdateBy;
                            vm.HoldAt = paramVM.LastUpdateAt;
                            vm.UnHoldBy = "";
                            vm.UnHoldAt = "";
                        }
                        else
                        {
                            vm.HoldBy = "";
                            vm.HoldAt = "";
                            vm.UnHoldBy = paramVM.LastUpdateBy;
                            vm.UnHoldAt = paramVM.LastUpdateAt;
                        }
                        #endregion

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                        cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);

                        cmdUpdate.Parameters.AddWithValue("@IsHold", vm.IsHold);
                        cmdUpdate.Parameters.AddWithValue("@HoldBy", vm.HoldBy);
                        cmdUpdate.Parameters.AddWithValue("@HoldAt", vm.HoldAt);
                        cmdUpdate.Parameters.AddWithValue("@UnHoldBy", vm.UnHoldBy);
                        cmdUpdate.Parameters.AddWithValue("@UnHoldAt", vm.UnHoldAt);

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



        #region Future Development
        //        //==================Insert =================
        //        public string[] Insert(SalaryEmployeeVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        //        {
        //            return null;
        //            #region Future Development
        //            #region Initializ
        //            string sqlText = "";
        //            int Id = 0;
        //            string[] retResults = new string[6];
        //            retResults[0] = "Fail";//Success or Fail
        //            retResults[1] = "Fail";// Success or Fail Message
        //            retResults[2] = Id.ToString();// Return Id
        //            retResults[3] = sqlText; //  SQL Query
        //            retResults[4] = "ex"; //catch ex
        //            retResults[5] = "InsertSalaryEmployee"; //Method Name
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;
        //            int transResult = 0;
        //            #endregion
        //            #region Try
        //            try
        //            {
        //                #region Validation
        //                #endregion Validation
        //                #region open connection and transaction
        //                #region New open connection and transaction
        //                if (VcurrConn != null)
        //                {
        //                    currConn = VcurrConn;
        //                }
        //                if (Vtransaction != null)
        //                {
        //                    transaction = Vtransaction;
        //                }
        //                #endregion New open connection and transaction
        //                if (currConn == null)
        //                {
        //                    currConn = _dbsqlConnection.GetConnection();
        //                    if (currConn.State != ConnectionState.Open)
        //                    {
        //                        currConn.Open();
        //                    }
        //                }
        //                if (transaction == null)
        //                {
        //                    transaction = currConn.BeginTransaction("");
        //                }
        //                #endregion open connection and transaction
        //                #region Save

        //                vm.Id = _cDal.NextId("SalaryEmployee", currConn, transaction);
        //                if (vm != null)
        //                {
        //                    #region SqlText
        //                    sqlText = "  ";
        //                    sqlText += @" 
        //INSERT INTO SalaryEmployee(
        //Id
        //,Name
        //,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
        //) VALUES (
        //@Id
        //,@Name
        //,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
        //) 
        //";

        //                    #endregion SqlText
        //                    #region SqlExecution
        //                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
        //                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
        //                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
        //                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
        //                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
        //                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
        //                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
        //                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
        //                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
        //                    var exeRes = cmdInsert.ExecuteNonQuery();
        //                    transResult = Convert.ToInt32(exeRes);
        //                    if (transResult <= 0)
        //                    {
        //                        retResults[3] = sqlText;
        //                        throw new ArgumentNullException("Unexpected error to update SalaryEmployee.", "");
        //                    }
        //                    #endregion SqlExecution
        //                }
        //                else
        //                {
        //                    retResults[1] = "This SalaryEmployee already used!";
        //                    throw new ArgumentNullException("Please Input SalaryEmployee Value", "");
        //                }
        //                #endregion Save
        //                #region Commit
        //                if (Vtransaction == null)
        //                {
        //                    if (transaction != null)
        //                    {
        //                        transaction.Commit();
        //                    }
        //                }
        //                #endregion Commit
        //                #region SuccessResult
        //                retResults[0] = "Success";
        //                retResults[1] = "Data Save Successfully.";
        //                retResults[2] = vm.Id.ToString();
        //                #endregion SuccessResult
        //            }
        //            #endregion try
        //            #region Catch and Finall
        //            catch (Exception ex)
        //            {
        //                retResults[0] = "Fail";//Success or Fail
        //                retResults[4] = ex.Message.ToString(); //catch ex
        //                if (Vtransaction == null) { transaction.Rollback(); }
        //                return retResults;
        //            }
        //            finally
        //            {
        //                if (VcurrConn == null)
        //                {
        //                    if (currConn != null)
        //                    {
        //                        if (currConn.State == ConnectionState.Open)
        //                        {
        //                            currConn.Close();
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion
        //            #region Results
        //            return retResults;
        //            #endregion
        //            #endregion 
        //        }
        //        //==================Update =================
        //        public string[] Update(SalaryEmployeeVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        //        {
        //            return null;
        //            #region Future Development

        //            #region Variables
        //            string[] retResults = new string[6];
        //            retResults[0] = "Fail";//Success or Fail
        //            retResults[1] = "Fail";// Success or Fail Message
        //            retResults[2] = "0";
        //            retResults[3] = "sqlText"; //  SQL Query
        //            retResults[4] = "ex"; //catch ex
        //            retResults[5] = "SalaryEmployeeUpdate"; //Method Name
        //            int transResult = 0;
        //            string sqlText = "";
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;
        //            #endregion
        //            try
        //            {
        //                #region open connection and transaction
        //                #region New open connection and transaction
        //                if (VcurrConn != null)
        //                {
        //                    currConn = VcurrConn;
        //                }
        //                if (Vtransaction != null)
        //                {
        //                    transaction = Vtransaction;
        //                }
        //                #endregion New open connection and transaction
        //                if (currConn == null)
        //                {
        //                    currConn = _dbsqlConnection.GetConnection();
        //                    if (currConn.State != ConnectionState.Open)
        //                    {
        //                        currConn.Open();
        //                    }
        //                }
        //                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }
        //                #endregion open connection and transaction

        //                if (vm != null)
        //                {

        //                    #region Update Settings
        //                    #region SqlText
        //                    sqlText = "";
        //                    sqlText = "UPDATE SalaryEmployee SET";

        //                    sqlText += "   Name=@Name";
        //                    sqlText += " , Remarks=@Remarks";
        //                    sqlText += " , IsActive=@IsActive";
        //                    sqlText += " , LastUpdateBy=@LastUpdateBy";
        //                    sqlText += " , LastUpdateAt=@LastUpdateAt";
        //                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
        //                    sqlText += " WHERE Id=@Id";

        //                    #endregion SqlText
        //                    #region SqlExecution
        //                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
        //                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
        //                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name);
        //                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
        //                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
        //                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
        //                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
        //                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
        //                    var exeRes = cmdUpdate.ExecuteNonQuery();
        //                    transResult = Convert.ToInt32(exeRes);
        //                    if (transResult <= 0)
        //                    {
        //                        retResults[3] = sqlText;
        //                        throw new ArgumentNullException("Unexpected error to update SalaryEmployee.", "");
        //                    }
        //                    #endregion SqlExecution

        //                    retResults[2] = vm.Id.ToString();// Return Id
        //                    retResults[3] = sqlText; //  SQL Query
        //                    #region Commit
        //                    if (transResult <= 0)
        //                    {
        //                        // throw new ArgumentNullException("SalaryEmployee Update", vm.BranchId + " could not updated.");
        //                    }
        //                    #endregion Commit
        //                    #endregion Update Settings
        //                }
        //                else
        //                {
        //                    throw new ArgumentNullException("SalaryEmployee Update", "Could not found any item.");
        //                }
        //                #region Commit
        //                if (Vtransaction == null)
        //                {
        //                    if (transaction != null)
        //                    {
        //                        transaction.Commit();
        //                    }
        //                }
        //                #endregion Commit
        //                #region SuccessResult
        //                retResults[0] = "Success";
        //                retResults[1] = "Data Save Successfully.";
        //                retResults[2] = vm.Id.ToString();
        //                #endregion SuccessResult
        //            }
        //            #region catch
        //            catch (Exception ex)
        //            {
        //                retResults[0] = "Fail";//Success or Fail
        //                retResults[4] = ex.Message; //catch ex
        //                if (Vtransaction == null) { transaction.Rollback(); }
        //                return retResults;
        //            }
        //            finally
        //            {
        //                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();
        //                }
        //            }
        //            #endregion
        //            return retResults;
        //            #endregion

        //        }
        //        ////==================Delete =================
        //        public string[] Delete(SalaryEmployeeVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        //        {
        //            return null;
        //            #region Future Development
        //            #region Variables
        //            string[] retResults = new string[6];
        //            retResults[0] = "Fail";//Success or Fail
        //            retResults[1] = "Fail";// Success or Fail Message
        //            retResults[2] = "0";// Return Id
        //            retResults[3] = "sqlText"; //  SQL Query
        //            retResults[4] = "ex"; //catch ex
        //            retResults[5] = "DeleteSalaryEmployee"; //Method Name
        //            int transResult = 0;
        //            string sqlText = "";
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;
        //            #endregion
        //            try
        //            {
        //                #region open connection and transaction
        //                #region New open connection and transaction
        //                if (VcurrConn != null)
        //                {
        //                    currConn = VcurrConn;
        //                }
        //                if (Vtransaction != null)
        //                {
        //                    transaction = Vtransaction;
        //                }
        //                #endregion New open connection and transaction
        //                if (currConn == null)
        //                {
        //                    currConn = _dbsqlConnection.GetConnection();
        //                    if (currConn.State != ConnectionState.Open)
        //                    {
        //                        currConn.Open();
        //                    }
        //                }
        //                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
        //                #endregion open connection and transaction
        //                if (ids.Length >= 1)
        //                {

        //                    #region Update Settings
        //                    for (int i = 0; i < ids.Length - 1; i++)
        //                    {
        //                        sqlText = "";
        //                        sqlText = "update SalaryEmployee set";
        //                        sqlText += " IsActive=@IsActive";
        //                        sqlText += " ,IsArchive=@IsArchive";
        //                        sqlText += " ,LastUpdateBy=@LastUpdateBy";
        //                        sqlText += " ,LastUpdateAt=@LastUpdateAt";
        //                        sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
        //                        sqlText += " where Id=@Id";
        //                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
        //                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
        //                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
        //                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
        //                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
        //                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
        //                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
        //                        var exeRes = cmdUpdate.ExecuteNonQuery();
        //                        transResult = Convert.ToInt32(exeRes);
        //                    }
        //                    retResults[2] = "";// Return Id
        //                    retResults[3] = sqlText; //  SQL Query
        //                    #region Commit
        //                    if (transResult <= 0)
        //                    {
        //                        throw new ArgumentNullException("SalaryEmployee Delete", vm.Id + " could not Delete.");
        //                    }
        //                    #endregion Commit
        //                    #endregion Update Settings
        //                }
        //                else
        //                {
        //                    throw new ArgumentNullException("SalaryEmployee Information Delete", "Could not found any item.");
        //                }
        //                if (Vtransaction == null && transaction != null)
        //                {
        //                    transaction.Commit();
        //                    retResults[0] = "Success";
        //                    retResults[1] = "Data Delete Successfully.";
        //                }
        //            }
        //            #region catch
        //            catch (Exception ex)
        //            {
        //                retResults[0] = "Fail";//Success or Fail
        //                retResults[4] = ex.Message; //catch ex
        //                if (Vtransaction == null) { transaction.Rollback(); }
        //                return retResults;
        //            }
        //            finally
        //            {
        //                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();
        //                }
        //            }
        //            #endregion
        //            return retResults;
        //            #endregion
        //        }
        #endregion

        #endregion
    }
}
