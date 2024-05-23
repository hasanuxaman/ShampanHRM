using Excel;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
namespace SymServices.HRM
{
    public class EmployeeStructureGroupDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectAll=================
        public EmployeeStructureGroupVM SelectByEmployee(string EmployeeId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeStructureGroupVM empStructureGpVm = new EmployeeStructureGroupVM();
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
                //,StructureGroupId
                //,Year
                sqlText = @"
SELECT TOP 1 esg.Id,esg.EmployeeId,esg.Remarks,esg.IsActive,esg.IsArchive,esg.CreatedBy,esg.CreatedAt,esg.CreatedFrom,esg.LastUpdateBy
,esg.LastUpdateAt,esg.LastUpdateFrom
,esg.EmployeeGroupId
,esg.LeaveStructureId
,IsNull(esg.EarningDeductionStructureId, 0) EarningDeductionStructureId


,esg.SalaryStructureId
,esg.GradeId
,esg.StepId
,IsNull(esg.IsGFApplicable,0)IsGFApplicable
,IsNull(esg.TaxPortion,0)TaxPortion
,IsNull(esg.EmpTaxValue,0)EmpTaxValue
,IsNull(esg.BonusTaxPortion,0)BonusTaxPortion
,IsNull(esg.EmpBonusTaxValue,0)EmpBonusTaxValue
,IsNull(esg.FixedOT,0)FixedOT
,isnull(esg.LeaveYear,'0')LeaveYear
,isnull(ej.BankPayAmount,'0')BankPayAmount
,isnull(esg.IsGross,'0')IsGross
,esg.PFStructureId,esg.TaxStructureId,esg.BonusStructureId,esg.ProjectAllocationId
,isnull(salary.Basic,0) [basic]
,isnull(salary.Gross,0) [gross]
,isnull(salary.HouseRent,0) HouseRent
,isnull(salary.Medical,0) Medical
,isnull(salary.Conveyance,0) Conveyance
From EmployeeStructureGroup esg left outer join EmployeeJob ej on esg.EmployeeId=ej.EmployeeId 
left outer join (select distinct EmployeeId, 
sum([Basic])[Basic],sum(HouseRent)HouseRent,sum(Gross)Gross,
sum(Medical)Medical,sum(Conveyance)Conveyance
from (select employeeId ,
case when salarytype ='Basic' then Amount end [Basic],
case when salarytype ='HouseRent' then Amount end HouseRent,
case when salarytype ='Gross' then Amount end Gross,
case when salarytype ='Medical' then Amount end Medical,
case when salarytype ='Conveyance' then Amount end Conveyance
 from EmployeeSalaryStructureDetail )as a

  group by EmployeeId) as salary on salary.EmployeeId = ej.EmployeeId
 left outer join dbo.EmployeeLeaveStructure el on esg.EmployeeId=el.EmployeeId
Where  esg.IsArchive=0 AND  esg.EmployeeId=@EmployeeId
                    ";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                //SqlDataReader dr;
                //dr = objComm.ExecuteReader();
                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(objComm);
                ad.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    empStructureGpVm.Id = Convert.ToInt32(dr["Id"].ToString());
                    empStructureGpVm.EmployeeId = dr["EmployeeId"].ToString();
                    empStructureGpVm.Remarks = dr["Remarks"].ToString();
                    empStructureGpVm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    empStructureGpVm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    empStructureGpVm.IsGFApplicable = Convert.ToBoolean(dr["IsGFApplicable"]);
                    empStructureGpVm.IsGross = Convert.ToBoolean(dr["IsGross"]);
                    empStructureGpVm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    empStructureGpVm.CreatedBy = dr["CreatedBy"].ToString();
                    empStructureGpVm.CreatedFrom = dr["CreatedFrom"].ToString();
                    empStructureGpVm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    empStructureGpVm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    empStructureGpVm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    empStructureGpVm.GradeId = dr["GradeId"].ToString();
                    empStructureGpVm.StepId = dr["StepId"].ToString();
                    empStructureGpVm.year = Convert.ToInt32(dr["LeaveYear"].ToString());
                    empStructureGpVm.EmployeeGroupId = dr["EmployeeGroupId"].ToString();
                    empStructureGpVm.EarningDeductionStructureId = Convert.ToInt32(dr["EarningDeductionStructureId"]);
                    empStructureGpVm.LeaveStructureId = dr["LeaveStructureId"].ToString();
                    empStructureGpVm.SalaryStructureId = dr["SalaryStructureId"].ToString();
                    empStructureGpVm.PFStructureId = dr["PFStructureId"].ToString();
                    empStructureGpVm.TaxStructureId = dr["TaxStructureId"].ToString();
                    empStructureGpVm.BonusStructureId = dr["BonusStructureId"].ToString();
                    empStructureGpVm.ProjectAllocationId = dr["ProjectAllocationId"].ToString();
                    empStructureGpVm.basic = Convert.ToDecimal(dr["basic"]);
                    empStructureGpVm.gross = Convert.ToDecimal(dr["gross"]);

                    empStructureGpVm.Housing = Convert.ToDecimal(dr["HouseRent"]);
                    empStructureGpVm.TA = Convert.ToDecimal(dr["Conveyance"]);
                    empStructureGpVm.Medical = Convert.ToDecimal(dr["Medical"]);
                    empStructureGpVm.BankPayAmount = Convert.ToDecimal(dr["BankPayAmount"]);
                    empStructureGpVm.TaxPortion = Convert.ToDecimal(dr["TaxPortion"]);
                    empStructureGpVm.EmpTaxValue = Convert.ToDecimal(dr["EmpTaxValue"]);
                    empStructureGpVm.BonusTaxPortion = Convert.ToDecimal(dr["BonusTaxPortion"]);
                    empStructureGpVm.EmpBonusTaxValue = Convert.ToDecimal(dr["EmpBonusTaxValue"]);
                    empStructureGpVm.FixedOT = Convert.ToDecimal(dr["FixedOT"]);
                }
                empStructureGpVm.EmployeeId = EmployeeId;
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
            return empStructureGpVm;
        }
        //==================SelectByID=================
        public string[] Insert(EmployeeStructureGroupVM vm, SqlConnection currConn, SqlTransaction transaction)
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
            retResults[5] = "InsertEmployeeStructureGroup"; //Method Name
            #endregion
            #region Try
            try
            {
                #region EmployeeStructureGroup
                sqlText = "  ";
                sqlText += @" INSERT INTO EmployeeStructureGroup(
EmployeeId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) VALUES (
 @EmployeeId
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                //cmdExist1.Parameters.AddWithValue("@EmployeeGroupId", employeeSGVM.EmployeeGroupId);
                //cmdExist1.Parameters.AddWithValue("@LeaveStructureId", employeeSGVM.LeaveStructureId);
                //cmdExist1.Parameters.AddWithValue("@SalaryStructureId", employeeSGVM.SalaryStructureId);
                //cmdExist1.Parameters.AddWithValue("@PFStructureId", employeeSGVM.PFStructureId);
                //cmdExist1.Parameters.AddWithValue("@TaxStructureId", employeeSGVM.TaxStructureId);
                //cmdExist1.Parameters.AddWithValue("@BonusStructureId", employeeSGVM.BonusStructureId);
                cmdInsert.Transaction = transaction;
                cmdInsert.ExecuteNonQuery();
                #endregion EmployeeStructureGroup
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                return retResults;
            }
            #endregion
            return retResults;
        }
        //==================Update =================
        public string[] Update(EmployeeJobVM employeeSGVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeJob Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeJob"); }
                #endregion open connection and transaction
                #region Designation/Promotion
                sqlText = "";
                sqlText = "update EmployeePromotion set";
                sqlText += " DesignationId=@DesignationId,";
                sqlText += " GradeId=@GradeId,";
                sqlText += " LastUpdateBy=@LastUpdateBy,";
                sqlText += " LastUpdateAt=@LastUpdateAt,";
                sqlText += " LastUpdateFrom=@LastUpdateFrom";
                sqlText += " where EmployeeId=@EmployeeId and IsCurrent=@IsCurrent";
                SqlCommand cmdDesg = new SqlCommand(sqlText, currConn);
                cmdDesg.Parameters.AddWithValue("@EmployeeId", employeeSGVM.EmployeeId);
                cmdDesg.Parameters.AddWithValue("@DesignationId", employeeSGVM.DesignationId);
                cmdDesg.Parameters.AddWithValue("@GradeId", employeeSGVM.GradeId);
                cmdDesg.Parameters.AddWithValue("@IsCurrent", true);
                cmdDesg.Parameters.AddWithValue("@LastUpdateBy", employeeSGVM.LastUpdateBy);
                cmdDesg.Parameters.AddWithValue("@LastUpdateAt", employeeSGVM.LastUpdateAt);
                cmdDesg.Parameters.AddWithValue("@LastUpdateFrom", employeeSGVM.LastUpdateFrom);
                cmdDesg.Transaction = transaction;
                cmdDesg.ExecuteNonQuery();
                #endregion
                #region Department
                sqlText = "";
                sqlText = "update EmployeeTransfer set";
                sqlText += " ProjectId=@ProjectId,";
                sqlText += " DepartmentId=@DepartmentId,";
                sqlText += " SectionId=@SectionId,";
                sqlText += " LastUpdateBy=@LastUpdateBy,";
                sqlText += " LastUpdateAt=@LastUpdateAt,";
                sqlText += " LastUpdateFrom=@LastUpdateFrom";
                sqlText += " where EmployeeId=@EmployeeId and IsCurrent=@IsCurrent";
                SqlCommand cmdDep = new SqlCommand(sqlText, currConn);
                cmdDep.Parameters.AddWithValue("@EmployeeId", employeeSGVM.EmployeeId);
                cmdDep.Parameters.AddWithValue("@ProjectId", employeeSGVM.ProjectId);
                cmdDep.Parameters.AddWithValue("@DepartmentId", employeeSGVM.DepartmentId);
                cmdDep.Parameters.AddWithValue("@IsCurrent", true);
                cmdDep.Parameters.AddWithValue("@SectionId", employeeSGVM.SectionId);
                cmdDep.Parameters.AddWithValue("@LastUpdateBy", employeeSGVM.LastUpdateBy);
                cmdDep.Parameters.AddWithValue("@LastUpdateAt", employeeSGVM.LastUpdateAt);
                cmdDep.Parameters.AddWithValue("@LastUpdateFrom", employeeSGVM.LastUpdateFrom);
                cmdDep.Transaction = transaction;
                cmdDep.ExecuteNonQuery();
                #endregion
                if (employeeSGVM != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EmployeeJob set";
                    sqlText += " JoinDate=@JoinDate,";
                    sqlText += " ProbationEnd=@ProbationEnd,";
                    sqlText += " DateOfPermanent=@DateOfPermanent,";
                    sqlText += " EmploymentStatus_E=@EmploymentStatus_E,";
                    sqlText += " EmploymentType_E=@EmploymentType_E,";
                    sqlText += " Supervisor=@Supervisor,";
                    sqlText += " IsPermanent=@IsPermanent,";
                    sqlText += " StructureGroupId=@StructureGroupId,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", employeeSGVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@JoinDate", Ordinary.DateToString(employeeSGVM.JoinDate));
                    cmdUpdate.Parameters.AddWithValue("@ProbationEnd", Ordinary.DateToString(employeeSGVM.ProbationEnd));
                    cmdUpdate.Parameters.AddWithValue("@DateOfPermanent", Ordinary.DateToString(employeeSGVM.DateOfPermanent));
                    cmdUpdate.Parameters.AddWithValue("@EmploymentStatus_E", employeeSGVM.EmploymentStatus_E);
                    cmdUpdate.Parameters.AddWithValue("@EmploymentType_E", employeeSGVM.EmploymentType_E);
                    cmdUpdate.Parameters.AddWithValue("@Supervisor", employeeSGVM.Supervisor ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsPermanent", employeeSGVM.IsPermanent);
                    cmdUpdate.Parameters.AddWithValue("@StructureGroupId", employeeSGVM.StructureGroupId);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", employeeSGVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", employeeSGVM.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeSGVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeSGVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeSGVM.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = employeeSGVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("PersonalDetail Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update PersonalDetail.";
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
        //==================Select =================
        public string[] EmployeeSalaryStructureFromGross(string EmployeeId, string SalaryStructureId, decimal Gross, ShampanIdentityVM siVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            Decimal GrossSalary = 0;
            decimal BasicSalary = 0;
            decimal SalaryAmount = 0;
            decimal Portion = 0;
            bool IsFixed = false;
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int EmployeeSalaryStructureNewId = 0;
            string EmployeeJoiningDate = "";
            string EmployeeJobId = "0";
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
                DataTable dtJob = new DataTable();
                DataSet dsSS = new DataSet();
                #region Job Check
                sqlText = "";
                sqlText += @" SELECT *  From EmployeeJob 
                                 where  EmployeeId=@EmployeeId  ";
                SqlCommand objCommssd = new SqlCommand();
                objCommssd.Connection = currConn;
                objCommssd.CommandText = sqlText;
                objCommssd.CommandType = CommandType.Text;
                objCommssd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                objCommssd.Transaction = transaction;
                SqlDataAdapter dajob = new SqlDataAdapter(objCommssd);
                dajob.Fill(dtJob);
                if (dtJob == null || dtJob.Rows.Count <= 0)
                {
                    retResults[1] = "This Employee Have not Assign In JOB, \nPlease Update the JOB First";
                    throw new ArgumentNullException("This Employee Have In Assign the JOB, \nPlease Update the JOB First", "");
                }
                else
                {
                    EmployeeJoiningDate = dtJob.Rows[0]["JoinDate"].ToString();
                    EmployeeJobId = dtJob.Rows[0]["Id"].ToString();
                }
                #endregion Job Check
                #region Check Exists
                sqlText = @"Select count(Id) from EmployeeSalaryStructure 
                                where 
                                EmployeeId=@EmployeeId 
                            ";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);
                if (count > 1)
                {
                    retResults[1] = "This Employee Already Assign the Salary Structure and can't Update\nYou can Give Increment";
                    throw new ArgumentNullException("This Employee Already Assign the Salary Structure and can't Update\nYou can Give Increment", "");
                }
                else if (count == 1)
                {
                    #region Update
                    #region SG Update
                    sqlText = "Select count(Id) from EmployeeStructureGroup where EmployeeId=@EmployeeId";
                    SqlCommand cmdExists = new SqlCommand(sqlText, currConn);
                    cmdExists.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdExists.Transaction = transaction;
                    exeRes = cmdExists.ExecuteScalar();
                    int exists = Convert.ToInt32(exeRes);
                    if (exists <= 0)
                    {
                        retResults[1] = "Please Save/Update Employee Job Information";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
                    }
                    sqlText = "  ";
                    sqlText += @" update  EmployeeStructureGroup set 
                            SalaryStructureId=@SalaryStructureId
                            ,LastUpdateBy=@LastUpdateBy
                            ,LastUpdateAt=@LastUpdateAt
                            ,LastUpdateFrom=@LastUpdateFrom
                            where EmployeeId=@EmployeeId   ";
                    SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                    cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdsg.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                    cmdsg.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                    cmdsg.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                    cmdsg.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                    cmdsg.Transaction = transaction;
                    cmdsg.ExecuteNonQuery();
                    #endregion SG Update
                    #region Delete EmployeeSalaryStructure
                    sqlText = "  ";
                    sqlText += @" delete from EmployeeSalaryStructure 
                                where EmployeeId=@EmployeeId  ";
                    SqlCommand cmdsgH = new SqlCommand(sqlText, currConn);
                    cmdsgH.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdsgH.Transaction = transaction;
                    cmdsgH.ExecuteNonQuery();
                    #endregion Delete EmployeeSalaryStructure
                    #region Delete EmployeeSalaryStructureDetail
                    sqlText = "  ";
                    sqlText += @" delete from EmployeeSalaryStructureDetail 
                                where EmployeeId=@EmployeeId  ";
                    SqlCommand cmdsgD = new SqlCommand(sqlText, currConn);
                    cmdsgD.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdsgD.Transaction = transaction;
                    cmdsgD.ExecuteNonQuery();
                    #endregion Delete EmployeeSalaryStructureDetail
                    #region Insert
                    #region EmployeeSalaryStructure
                    sqlText = "Select isnull(max(convert(int, Id)),0) from EmployeeSalaryStructure ";
                    SqlCommand cmdEmployeeSalaryStructureId = new SqlCommand(sqlText, currConn);
                    cmdEmployeeSalaryStructureId.Transaction = transaction;
                    exeRes = cmdEmployeeSalaryStructureId.ExecuteScalar();
                    EmployeeSalaryStructureNewId = Convert.ToInt32(exeRes);
                    EmployeeSalaryStructureNewId++;
                    sqlText = "";
                    sqlText += @" SELECT *  From SalaryStructure where  Id=@Id ";
                    SqlCommand objss = new SqlCommand();
                    objss.Connection = currConn;
                    objss.CommandText = sqlText;
                    objss.CommandType = CommandType.Text;
                    objss.Parameters.AddWithValue("@Id", SalaryStructureId);
                    objss.Transaction = transaction;
                    SqlDataAdapter daSS = new SqlDataAdapter(objss);
                    daSS.Fill(dsSS);
                    foreach (var item in dsSS.Tables[0].Rows)
                    {
                        sqlText = "  ";
                        sqlText += @" INSERT INTO EmployeeSalaryStructure(
                                    Id,EmployeeId,SalaryStructureId,TotalValue,IncrementDate,BranchId,Remarks,IsActive,IsArchive
                                    ,CreatedBy,CreatedAt,CreatedFrom
                                    )VALUES (
                                    @Id,@EmployeeId,@SalaryStructureId,@TotalValue,@IncrementDate,@BranchId,@Remarks,@IsActive
                                    ,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                    ) ";
                        SqlCommand cmdInsESSD = new SqlCommand(sqlText, currConn);
                        cmdInsESSD.Parameters.AddWithValue("@Id", EmployeeSalaryStructureNewId);
                        cmdInsESSD.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsESSD.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                        cmdInsESSD.Parameters.AddWithValue("@TotalValue", Gross);
                        cmdInsESSD.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                        cmdInsESSD.Parameters.AddWithValue("@BranchId", siVM.BranchId);
                        cmdInsESSD.Parameters.AddWithValue("@Remarks", Convert.DBNull);
                        cmdInsESSD.Parameters.AddWithValue("@IsActive", true);
                        cmdInsESSD.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsESSD.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsESSD.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsESSD.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsESSD.Transaction = transaction;
                        cmdInsESSD.ExecuteNonQuery();
                        EmployeeSalaryStructureNewId++;
                    }
                    #endregion EmployeeSalaryStructure
                    #region Save
                    #region sql statement
                    sqlText = "Select isnull(max(convert(int, Id)),0) from EmployeeSalaryStructureDetail ";
                    SqlCommand cmdId = new SqlCommand(sqlText, currConn);
                    cmdId.Transaction = transaction;
                    exeRes = cmdId.ExecuteScalar();
                    int NewId = Convert.ToInt32(exeRes);
                    DataSet ds = new DataSet();
                    sqlText = "";
                    sqlText += @" 
                                SELECT *  From SalaryStructureDetail where  SalaryStructureId=@SalaryStructureId and SalaryType in('Gross') 
                                SELECT *  From SalaryStructureDetail where  SalaryStructureId=@SalaryStructureId and SalaryType in('basic')  
                                SELECT *  From SalaryStructureDetail where  SalaryStructureId=@SalaryStructureId and SalaryType not in('basic') 
                                and PortionSalaryType in('Gross')  
                                SELECT *  From SalaryStructureDetail where  SalaryStructureId=@SalaryStructureId and SalaryType not in('basic') 
                                and PortionSalaryType in('basic')   
                                SELECT *  From SalaryStructureDetail where  SalaryStructureId=@SalaryStructureId and IsFixed =1 and  SalaryType 
                                not in('Gross','basic')
                                ";
                    SqlCommand objCommssd1 = new SqlCommand();
                    objCommssd1.Connection = currConn;
                    objCommssd1.CommandText = sqlText;
                    objCommssd1.CommandType = CommandType.Text;
                    objCommssd1.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                    objCommssd1.Transaction = transaction;
                    SqlDataAdapter da = new SqlDataAdapter(objCommssd1);
                    da.Fill(ds);
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeSalaryStructureDetail(
                                            Id,EmployeeSalaryStructureId,SalaryTypeId,SalaryType,EmployeeId,IsFixed,Portion,PortionSalaryType,Amount,Remarks,IsActive,IsArchive
                                            ,CreatedBy,CreatedAt,CreatedFrom 
                                            )VALUES (
                                            @Id,@EmployeeSalaryStructureId,@SalaryTypeId,@SalaryType,@EmployeeId,@IsFixed,@Portion,@PortionSalaryType,@Amount,@Remarks,@IsActive
                                            ,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom ) 
                                            ";
                    #endregion  sql statement
                    #region drGross
                    foreach (DataRow drGross in ds.Tables[0].Rows)
                    {
                        NewId++;
                        GrossSalary = Gross;
                        SalaryAmount = GrossSalary;
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drGross["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drGross["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", drGross["IsFixed"].ToString());
                        cmdInsert.Parameters.AddWithValue("@Portion", drGross["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drGross["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drGross["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion Save
                    #region drBasic
                    foreach (DataRow drBasic in ds.Tables[1].Rows)
                    {
                        #region Check
                        if (!Ordinary.IsNumeric(drBasic["Portion"].ToString()))
                        {
                            retResults[1] = "Salary Portion must Numeric";
                            throw new ArgumentNullException("Salary Portion must Numeric", "");
                        }
                        else
                        {
                            Portion = Convert.ToDecimal(drBasic["Portion"].ToString());
                        }
                        if (!Ordinary.IsBool(drBasic["IsFixed"].ToString()))
                        {
                            retResults[1] = "IsFixed must Bool";
                            throw new ArgumentNullException("IsFixed must Bool", "");
                        }
                        else
                        {
                            IsFixed = Convert.ToBoolean(drBasic["IsFixed"].ToString());
                        }
                        #endregion Check
                        if (!IsFixed)
                        {
                            BasicSalary = Math.Ceiling(GrossSalary * Portion / 100);
                            SalaryAmount = BasicSalary;
                        }
                        NewId++;
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drBasic["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drBasic["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", drBasic["IsFixed"].ToString());
                        cmdInsert.Parameters.AddWithValue("@Portion", Portion);
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drBasic["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drBasic["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion Save
                    #region drGrossPortion
                    foreach (DataRow drGrossPortion in ds.Tables[2].Rows)
                    {
                        #region Check
                        if (!Ordinary.IsNumeric(drGrossPortion["Portion"].ToString()))
                        {
                            retResults[1] = "Salary Portion must Numeric";
                            throw new ArgumentNullException("Salary Portion must Numeric", "");
                        }
                        else
                        {
                            Portion = Convert.ToDecimal(drGrossPortion["Portion"].ToString());
                        }
                        if (!Ordinary.IsBool(drGrossPortion["IsFixed"].ToString()))
                        {
                            retResults[1] = "IsFixed must Bool";
                            throw new ArgumentNullException("IsFixed must Bool", "");
                        }
                        else
                        {
                            IsFixed = Convert.ToBoolean(drGrossPortion["IsFixed"].ToString());
                        }
                        #endregion Check
                        if (!IsFixed)
                        {
                            SalaryAmount = Math.Ceiling(GrossSalary * Portion / 100);
                        }
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        NewId++;
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drGrossPortion["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drGrossPortion["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", drGrossPortion["IsFixed"].ToString());
                        cmdInsert.Parameters.AddWithValue("@Portion", Portion);
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drGrossPortion["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drGrossPortion["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion Save
                    #region drBasicPortion
                    foreach (DataRow drBasicPortion in ds.Tables[3].Rows)
                    {
                        #region Check
                        if (!Ordinary.IsNumeric(drBasicPortion["Portion"].ToString()))
                        {
                            retResults[1] = "Salary Portion must Numeric";
                            throw new ArgumentNullException("Salary Portion must Numeric", "");
                        }
                        else
                        {
                            Portion = Convert.ToDecimal(drBasicPortion["Portion"].ToString());
                        }
                        if (!Ordinary.IsBool(drBasicPortion["IsFixed"].ToString()))
                        {
                            retResults[1] = "IsFixed must Bool";
                            throw new ArgumentNullException("IsFixed must Bool", "");
                        }
                        else
                        {
                            IsFixed = Convert.ToBoolean(drBasicPortion["IsFixed"].ToString());
                        }
                        #endregion Check
                        if (!IsFixed)
                        {
                            SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                        }
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        NewId++;
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drBasicPortion["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drBasicPortion["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", drBasicPortion["IsFixed"].ToString());
                        cmdInsert.Parameters.AddWithValue("@Portion", Portion);
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drBasicPortion["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drBasicPortion["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion Save
                    #region drFixed
                    foreach (DataRow drFixed in ds.Tables[4].Rows)
                    {
                        #region Check
                        if (!Ordinary.IsNumeric(drFixed["Portion"].ToString()))
                        {
                            retResults[1] = "Salary Portion must Numeric";
                            throw new ArgumentNullException("Salary Portion must Numeric", "");
                        }
                        else
                        {
                            Portion = Convert.ToDecimal(drFixed["Portion"].ToString());
                        }
                        if (!Ordinary.IsBool(drFixed["IsFixed"].ToString()))
                        {
                            retResults[1] = "IsFixed must Bool";
                            throw new ArgumentNullException("IsFixed must Bool", "");
                        }
                        else
                        {
                            IsFixed = Convert.ToBoolean(drFixed["IsFixed"].ToString());
                        }
                        #endregion Check
                        if (IsFixed)
                        {
                            SalaryAmount = Math.Ceiling(Portion);
                        }
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        NewId++;
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drFixed["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drFixed["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", drFixed["IsFixed"].ToString());
                        cmdInsert.Parameters.AddWithValue("@Portion", Portion);
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drFixed["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drFixed["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion Save
                    #endregion Save
                    #endregion Insert
                    #endregion Update
                    #region EmployeeJob Update
                    //                    sqlText = "  ";
                    //                    sqlText += @" update  EmployeeJob set 
                    //                            GrossSalary=@GrossSalary
                    //                            ,BasicSalary=@BasicSalary
                    //                            ,LastUpdateBy=@LastUpdateBy
                    //                            ,LastUpdateAt=@LastUpdateAt
                    //                            ,LastUpdateFrom=@LastUpdateFrom
                    //                            where EmployeeId=@EmployeeId   ";
                    //                    SqlCommand cmdEJ = new SqlCommand(sqlText, currConn);
                    //                    cmdEJ.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    //                    cmdEJ.Parameters.AddWithValue("@GrossSalary", GrossSalary);
                    //                    cmdEJ.Parameters.AddWithValue("@BasicSalary", BasicSalary);
                    //                    cmdEJ.Parameters.AddWithValue("@LastUpdateBy", siVM.CreatedBy);
                    //                    cmdEJ.Parameters.AddWithValue("@LastUpdateAt", siVM.CreatedAt);
                    //                    cmdEJ.Parameters.AddWithValue("@LastUpdateFrom", siVM.CreatedFrom);
                    //                    cmdEJ.Transaction = transaction;
                    //                    cmdEJ.ExecuteNonQuery();
                    #endregion EmployeeJob Update
                }
                else if (count < 1)
                {
                    #region Insert
                    #region SG Update
                    sqlText = "  ";
                    sqlText += @" update  EmployeeStructureGroup set 
                            SalaryStructureId=@SalaryStructureId
                            ,LastUpdateBy=@LastUpdateBy
                            ,LastUpdateAt=@LastUpdateAt
                            ,LastUpdateFrom=@LastUpdateFrom
                            where EmployeeId=@EmployeeId   ";
                    SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                    cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdsg.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                    cmdsg.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                    cmdsg.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                    cmdsg.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                    cmdsg.Transaction = transaction;
                    cmdsg.ExecuteNonQuery();
                    #endregion SG Update
                    #region EmployeeSalaryStructure
                    sqlText = "Select isnull(max(convert(int, Id)),0) from EmployeeSalaryStructure ";
                    SqlCommand cmdEmployeeSalaryStructureId = new SqlCommand(sqlText, currConn);
                    cmdEmployeeSalaryStructureId.Transaction = transaction;
                    exeRes = cmdEmployeeSalaryStructureId.ExecuteScalar();
                    EmployeeSalaryStructureNewId = Convert.ToInt32(exeRes);
                    EmployeeSalaryStructureNewId++;
                    sqlText = "";
                    sqlText += @" SELECT *  From SalaryStructure where  Id=@Id ";
                    SqlCommand objss = new SqlCommand();
                    objss.Connection = currConn;
                    objss.CommandText = sqlText;
                    objss.CommandType = CommandType.Text;
                    objss.Parameters.AddWithValue("@Id", SalaryStructureId);
                    objss.Transaction = transaction;
                    SqlDataAdapter daSS = new SqlDataAdapter(objss);
                    dajob.Fill(dsSS);
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeSalaryStructure(
                                    Id,EmployeeId,SalaryStructureId,TotalValue,IncrementDate,BranchId,Remarks,IsActive,IsArchive
                                    ,CreatedBy,CreatedAt,CreatedFrom
                                    )VALUES (
                                    @Id,@EmployeeId,@SalaryStructureId,@TotalValue,@IncrementDate,@BranchId,@Remarks,@IsActive
                                    ,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                    ) ";
                    foreach (var item in dsSS.Tables[0].Rows)
                    {
                        SqlCommand cmdInsESSD = new SqlCommand(sqlText, currConn);
                        cmdInsESSD.Parameters.AddWithValue("@Id", EmployeeSalaryStructureNewId);
                        cmdInsESSD.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsESSD.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                        cmdInsESSD.Parameters.AddWithValue("@TotalValue", Gross);
                        cmdInsESSD.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                        cmdInsESSD.Parameters.AddWithValue("@BranchId", siVM.BranchId);
                        cmdInsESSD.Parameters.AddWithValue("@Remarks", Convert.DBNull);
                        cmdInsESSD.Parameters.AddWithValue("@IsActive", true);
                        cmdInsESSD.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsESSD.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsESSD.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsESSD.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsESSD.Transaction = transaction;
                        cmdInsESSD.ExecuteNonQuery();
                        EmployeeSalaryStructureNewId++;
                    }
                    #endregion EmployeeSalaryStructure
                    #region Save
                    #region sql statement
                    sqlText = "Select isnull(max(convert(int, Id)),0) from EmployeeSalaryStructureDetail ";
                    SqlCommand cmdId = new SqlCommand(sqlText, currConn);
                    cmdId.Transaction = transaction;
                    exeRes = cmdId.ExecuteScalar();
                    int NewId = Convert.ToInt32(exeRes);
                    DataSet ds = new DataSet();
                    sqlText = "";
                    sqlText += @" 
                                SELECT *  From SalaryStructureDetail where  SalaryStructureId=@SalaryStructureId and SalaryType in('Gross') 
                                SELECT *  From SalaryStructureDetail where  SalaryStructureId=@SalaryStructureId and SalaryType in('basic')  
                                SELECT *  From SalaryStructureDetail where  SalaryStructureId=@SalaryStructureId and SalaryType not in('basic') 
                                and PortionSalaryType in('Gross')  
                                SELECT *  From SalaryStructureDetail where  SalaryStructureId=@SalaryStructureId and SalaryType not in('basic') 
                                and PortionSalaryType in('basic')   
                                SELECT *  From SalaryStructureDetail where  SalaryStructureId=@SalaryStructureId and IsFixed =1 and  SalaryType 
                                not in('Gross','basic')
                                ";
                    SqlCommand objCommssd1 = new SqlCommand();
                    objCommssd1.Connection = currConn;
                    objCommssd1.CommandText = sqlText;
                    objCommssd1.CommandType = CommandType.Text;
                    objCommssd1.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                    objCommssd1.Transaction = transaction;
                    SqlDataAdapter da = new SqlDataAdapter(objCommssd1);
                    da.Fill(ds);
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeSalaryStructureDetail(
                                            Id,EmployeeSalaryStructureId,SalaryTypeId,SalaryType,EmployeeId,IsFixed,Portion,PortionSalaryType,Amount,Remarks,IsActive,IsArchive
                                            ,CreatedBy,CreatedAt,CreatedFrom 
                                            )VALUES (
                                            @Id,@EmployeeSalaryStructureId,@SalaryTypeId,@SalaryType,@EmployeeId,@IsFixed,@Portion,@PortionSalaryType,@Amount,@Remarks,@IsActive
                                            ,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom ) 
                                            ";
                    #endregion  sql statement
                    #region drGross
                    foreach (DataRow drGross in ds.Tables[0].Rows)
                    {
                        NewId++;
                        GrossSalary = Gross;
                        SalaryAmount = GrossSalary;
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drGross["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drGross["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", drGross["IsFixed"].ToString());
                        cmdInsert.Parameters.AddWithValue("@Portion", drGross["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drGross["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drGross["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion Save
                    #region drBasic
                    foreach (DataRow drBasic in ds.Tables[1].Rows)
                    {
                        #region Check
                        if (!Ordinary.IsNumeric(drBasic["Portion"].ToString()))
                        {
                            retResults[1] = "Salary Portion must Numeric";
                            throw new ArgumentNullException("Salary Portion must Numeric", "");
                        }
                        else
                        {
                            Portion = Convert.ToDecimal(drBasic["Portion"].ToString());
                        }
                        if (!Ordinary.IsBool(drBasic["IsFixed"].ToString()))
                        {
                            retResults[1] = "IsFixed must Bool";
                            throw new ArgumentNullException("IsFixed must Bool", "");
                        }
                        else
                        {
                            IsFixed = Convert.ToBoolean(drBasic["IsFixed"].ToString());
                        }
                        #endregion Check
                        if (!IsFixed)
                        {
                            BasicSalary = Math.Ceiling(GrossSalary * Portion / 100);
                            SalaryAmount = BasicSalary;
                        }
                        NewId++;
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drBasic["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drBasic["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", drBasic["IsFixed"].ToString());
                        cmdInsert.Parameters.AddWithValue("@Portion", Portion);
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drBasic["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drBasic["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion Save
                    #region drGrossPortion
                    foreach (DataRow drGrossPortion in ds.Tables[2].Rows)
                    {
                        #region Check
                        if (!Ordinary.IsNumeric(drGrossPortion["Portion"].ToString()))
                        {
                            retResults[1] = "Salary Portion must Numeric";
                            throw new ArgumentNullException("Salary Portion must Numeric", "");
                        }
                        else
                        {
                            Portion = Convert.ToDecimal(drGrossPortion["Portion"].ToString());
                        }
                        if (!Ordinary.IsBool(drGrossPortion["IsFixed"].ToString()))
                        {
                            retResults[1] = "IsFixed must Bool";
                            throw new ArgumentNullException("IsFixed must Bool", "");
                        }
                        else
                        {
                            IsFixed = Convert.ToBoolean(drGrossPortion["IsFixed"].ToString());
                        }
                        #endregion Check
                        if (!IsFixed)
                        {
                            SalaryAmount = Math.Ceiling(GrossSalary * Portion / 100);
                        }
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        NewId++;
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drGrossPortion["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drGrossPortion["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", drGrossPortion["IsFixed"].ToString());
                        cmdInsert.Parameters.AddWithValue("@Portion", Portion);
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drGrossPortion["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drGrossPortion["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion Save
                    #region drBasicPortion
                    foreach (DataRow drBasicPortion in ds.Tables[3].Rows)
                    {
                        #region Check
                        if (!Ordinary.IsNumeric(drBasicPortion["Portion"].ToString()))
                        {
                            retResults[1] = "Salary Portion must Numeric";
                            throw new ArgumentNullException("Salary Portion must Numeric", "");
                        }
                        else
                        {
                            Portion = Convert.ToDecimal(drBasicPortion["Portion"].ToString());
                        }
                        if (!Ordinary.IsBool(drBasicPortion["IsFixed"].ToString()))
                        {
                            retResults[1] = "IsFixed must Bool";
                            throw new ArgumentNullException("IsFixed must Bool", "");
                        }
                        else
                        {
                            IsFixed = Convert.ToBoolean(drBasicPortion["IsFixed"].ToString());
                        }
                        #endregion Check
                        if (!IsFixed)
                        {
                            SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                        }
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        NewId++;
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drBasicPortion["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drBasicPortion["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", drBasicPortion["IsFixed"].ToString());
                        cmdInsert.Parameters.AddWithValue("@Portion", Portion);
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drBasicPortion["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drBasicPortion["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion Save
                    #region drFixed
                    foreach (DataRow drFixed in ds.Tables[4].Rows)
                    {
                        #region Check
                        if (!Ordinary.IsNumeric(drFixed["Portion"].ToString()))
                        {
                            retResults[1] = "Salary Portion must Numeric";
                            throw new ArgumentNullException("Salary Portion must Numeric", "");
                        }
                        else
                        {
                            Portion = Convert.ToDecimal(drFixed["Portion"].ToString());
                        }
                        if (!Ordinary.IsBool(drFixed["IsFixed"].ToString()))
                        {
                            retResults[1] = "IsFixed must Bool";
                            throw new ArgumentNullException("IsFixed must Bool", "");
                        }
                        else
                        {
                            IsFixed = Convert.ToBoolean(drFixed["IsFixed"].ToString());
                        }
                        #endregion Check
                        if (!IsFixed)
                        {
                            SalaryAmount = Math.Ceiling(Portion);
                        }
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        NewId++;
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drFixed["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drFixed["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", drFixed["IsFixed"].ToString());
                        cmdInsert.Parameters.AddWithValue("@Portion", Portion);
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drFixed["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drFixed["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion Save
                    #endregion Save
                    #endregion Insert
                    #region EmployeeJob Update
                    //                    sqlText = "  ";
                    //                    sqlText += @" update  EmployeeJob set 
                    //                            GrossSalary=@GrossSalary
                    //                            ,BasicSalary=@BasicSalary
                    //                            ,LastUpdateBy=@LastUpdateBy
                    //                            ,LastUpdateAt=@LastUpdateAt
                    //                            ,LastUpdateFrom=@LastUpdateFrom
                    //                            where EmployeeId=@EmployeeId   ";
                    //                    SqlCommand cmdEJ = new SqlCommand(sqlText, currConn);
                    //                    cmdEJ.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    //                    cmdEJ.Parameters.AddWithValue("@GrossSalary", GrossSalary);
                    //                    cmdEJ.Parameters.AddWithValue("@BasicSalary", BasicSalary);
                    //                    cmdEJ.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                    //                    cmdEJ.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                    //                    cmdEJ.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                    //                    cmdEJ.Transaction = transaction;
                    //                    cmdEJ.ExecuteNonQuery();
                    #endregion EmployeeJob Update
                }
                #region EmployeeJob Update
                sqlText = "";
                sqlText += @" select EmployeeId,sum(Gross)Gross,sum(Basic)Basic  from(
select distinct EmployeeId, Amount Gross,0 Basic
  from EmployeeSalaryStructureDetail where PortionSalaryType='gross' and EmployeeId=@EmployeeId  
union all 
select distinct EmployeeId,0 Gross, Amount Basic
  from EmployeeSalaryStructureDetail where  PortionSalaryType='basic' and EmployeeId=@EmployeeId 
) as a group by EmployeeId ";
                SqlCommand cmdEJ = new SqlCommand();
                cmdEJ.Connection = currConn;
                cmdEJ.CommandText = sqlText;
                cmdEJ.CommandType = CommandType.Text;
                cmdEJ.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdEJ.Transaction = transaction;
                DataTable dtEJ = new DataTable();
                SqlDataAdapter daEJ = new SqlDataAdapter(cmdEJ);
                daEJ.Fill(dtEJ);
                foreach (DataRow item in dtEJ.Rows)
                {
                    GrossSalary = Convert.ToDecimal(item["Gross"]);
                    BasicSalary = Convert.ToDecimal(item["Basic"]);
                    sqlText = "  ";
                    sqlText += @" update  EmployeeJob set 
                            GrossSalary=@GrossSalary
                            ,BasicSalary=@BasicSalary
                            ,LastUpdateBy=@LastUpdateBy
                            ,LastUpdateAt=@LastUpdateAt
                            ,LastUpdateFrom=@LastUpdateFrom
                            where EmployeeId=@EmployeeId   ";
                    SqlCommand cmdEJ1 = new SqlCommand(sqlText, currConn);
                    cmdEJ1.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdEJ1.Parameters.AddWithValue("@GrossSalary", GrossSalary);
                    cmdEJ1.Parameters.AddWithValue("@BasicSalary", BasicSalary);
                    cmdEJ1.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                    cmdEJ1.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                    cmdEJ1.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                    cmdEJ1.Transaction = transaction;
                    cmdEJ1.ExecuteNonQuery();
                }
                #endregion EmployeeJob Update
                #endregion Check Exists
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
        public string[] EmployeeSalaryStructureFromBasic(string EmployeeId, string SalaryStructureId, decimal salaryInput, string gradeId, string stepId, bool isGross, decimal BankPayAmount, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            DataSet dsOld1 = new DataSet();
            DataSet dsOld2 = new DataSet();
            DataSet dsOld3 = new DataSet();
            DataSet dsOld4 = new DataSet();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            string stepName = "";
            decimal GrossSalary = 0;
            decimal SalaryAmount = 0;
            decimal MBasic = 0;
            decimal MMedical = 0;
            decimal MConveyance = 0;
            decimal Portion = 0;
            int EmployeeSalaryStructureNewId = 0;
            string EmployeeJoiningDate = "";
            string EmployeeJobId = "0";
            bool IsFixed = false;
            decimal GrossSalaryInput = 0;
            decimal BasicSalary = 0;
            decimal MedicalSalary = 0;
            decimal HouseRentSalary = 0;
            decimal ConvenceSalary = 0;

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
                SettingDAL _setDAL = new SettingDAL();
                var SalaryFromMatrix = Convert.ToBoolean(_setDAL.settingValue("Salary", "SalaryFromMatrix", currConn, transaction).ToLower() == "y" ? true : false);
                if (isGross)
                {
                    GrossSalaryInput = salaryInput;

                    try
                    {
                        #region VBScript
                        MSScriptControl.ScriptControl sc = new MSScriptControl.ScriptControl();
                        sc.Language = "VBScript";
                        var BasicCalc = _setDAL.settingValue("Salary", "BasicCalc", currConn, transaction);
                        BasicCalc = BasicCalc.Replace("vGross", GrossSalaryInput.ToString());
                        var basic = sc.Eval(BasicCalc);
                        BasicSalary = Convert.ToDecimal(basic);
                        if (!SalaryFromMatrix)
                        {
                            var HouseRentCalc = _setDAL.settingValue("Salary", "HouseRentCalc", currConn, transaction);
                            if (HouseRentCalc.Contains("vGross"))
                            {
                                HouseRentCalc = HouseRentCalc.Replace("vGross", GrossSalaryInput.ToString());
                            }
                            else if (HouseRentCalc.Contains("vBasic"))
                            {
                                HouseRentCalc = HouseRentCalc.Replace("vBasic", BasicSalary.ToString());
                            }
                            var hr = sc.Eval(HouseRentCalc);
                            HouseRentSalary = Convert.ToDecimal(hr);

                            var ConvenceCalc = _setDAL.settingValue("Salary", "ConvenceCalc", currConn, transaction);
                            if (ConvenceCalc.Contains("vGross"))
                            {
                                ConvenceCalc = ConvenceCalc.Replace("vGross", GrossSalaryInput.ToString());
                            }
                            else if (ConvenceCalc.Contains("vBasic"))
                            {
                                ConvenceCalc = ConvenceCalc.Replace("vBasic", BasicSalary.ToString());
                            }
                            var convence = sc.Eval(ConvenceCalc);
                            ConvenceSalary = Convert.ToDecimal(convence);

                            var MedicalCalc = _setDAL.settingValue("Salary", "MedicalCalc", currConn, transaction);
                            if (MedicalCalc.Contains("vGross"))
                            {
                                MedicalCalc = MedicalCalc.Replace("vGross", GrossSalaryInput.ToString());
                            }
                            else if (MedicalCalc.Contains("vBasic"))
                            {
                                MedicalCalc = MedicalCalc.Replace("vBasic", BasicSalary.ToString());
                            }
                            var medical = sc.Eval(MedicalCalc);
                            MedicalSalary = Convert.ToDecimal(medical);
                        }
                        #endregion VBScript
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Log("Fail" + Environment.NewLine + ex.Message + Environment.NewLine + "EmployeeSalaryStructureFromBasic", this.GetType().Name, ex.Message + Environment.NewLine + sqlText.ToString() + Environment.NewLine + ex.StackTrace);

                        throw ex;
                    }
                }
                else
                {
                    BasicSalary = salaryInput;
                }
                DataTable dteJob = new DataTable();
                DataTable dtePromossion = new DataTable();
                DataTable dtJobMatrix = new DataTable();
                DataSet dsSS = new DataSet();
                #region Job Check
                sqlText = "";
                sqlText += @" SELECT *  From EmployeeJob 
                                 where  EmployeeId=@EmployeeId  ";
                SqlCommand objCommejob = new SqlCommand();
                objCommejob.Connection = currConn;
                objCommejob.CommandText = sqlText;
                objCommejob.CommandType = CommandType.Text;
                objCommejob.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                objCommejob.Transaction = transaction;
                SqlDataAdapter daeJob = new SqlDataAdapter(objCommejob);
                daeJob.Fill(dteJob);
                if (dteJob == null || dteJob.Rows.Count <= 0)
                {
                    retResults[1] = "This Employee Have not Assign In JOB, \nPlease Update the JOB First";
                    throw new ArgumentNullException("This Employee Have In Assign the JOB, \nPlease Update the JOB First", "");
                }
                else
                {
                    EmployeeJoiningDate = dteJob.Rows[0]["JoinDate"].ToString();
                    EmployeeJobId = dteJob.Rows[0]["Id"].ToString();
                }
                #endregion Job Check
                #region Promossion
                sqlText = "";
                sqlText += @" SELECT *  From EmployeePromotion 
                                 where  EmployeeId=@EmployeeId  ";
                SqlCommand objCommePromossion = new SqlCommand();
                objCommePromossion.Connection = currConn;
                objCommePromossion.CommandText = sqlText;
                objCommePromossion.CommandType = CommandType.Text;
                objCommePromossion.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                objCommePromossion.Transaction = transaction;
                SqlDataAdapter daePromossion = new SqlDataAdapter(objCommePromossion);
                daePromossion.Fill(dtePromossion);
                if (dtePromossion == null || dtePromossion.Rows.Count > 1)
                {
                    //retResults[1] = "This Employee already Have Promotion,you can't Change the Structure, \nPlease Update the Promotion";
                    //throw new ArgumentNullException("This Employee already Have Promotion,you can't Change the Structure, \nPlease Update the Promotion", "");
                }
                //else
                //{
                //    EmployeeJoiningDate = dtePromossion.Rows[0]["JoinDate"].ToString();
                //    EmployeeJobId = dtePromossion.Rows[0]["Id"].ToString();
                //}
                #endregion Promossion
                #region Matrix Check
                if (stepId != null && SalaryFromMatrix == true)
                {
                    stepName = "step" + stepId + "Amount";
                    sqlText = "";
                    sqlText += @" select max(Basic)Basic,max(Medical)Medical,max(Conveyance)Conveyance from ( ";
                    sqlText += @" select  " + stepName + "  'Basic',0 'Medical',0 'Conveyance' from SalaryStructureMatrix where GradeId=@GradeId and salaryTypeName='basic'";
                    sqlText += @" union all select 0 'Basic',  " + stepName + "   'Medical',0 'Conveyance' from SalaryStructureMatrix where GradeId=@GradeId and salaryTypeName='medical'";
                    sqlText += @" union all select 0 'Basic',0 'Medical',  " + stepName + "   'Conveyance' from SalaryStructureMatrix where GradeId=@GradeId and salaryTypeName='conveyance') as a ";
                    SqlCommand objCommMatrix = new SqlCommand();
                    objCommMatrix.Connection = currConn;
                    objCommMatrix.CommandText = sqlText;
                    objCommMatrix.CommandType = CommandType.Text;
                    objCommMatrix.Parameters.AddWithValue("@GradeId", gradeId);
                    objCommMatrix.Transaction = transaction;
                    SqlDataAdapter dajobMatrix = new SqlDataAdapter(objCommMatrix);
                    dajobMatrix.Fill(dtJobMatrix);
                    if (dtJobMatrix == null || dtJobMatrix.Rows.Count <= 0)
                    {
                        retResults[1] = "This Employee Have not Assign In  Matrix, \nPlease Update the Matrix First";
                        throw new ArgumentNullException("This Employee Have In Assign the Matrix, \nPlease Update the Matrix First", "");
                    }
                    else
                    {
                        MBasic = Convert.ToDecimal(dtJobMatrix.Rows[0]["Basic"]);
                        MMedical = Convert.ToDecimal(dtJobMatrix.Rows[0]["Medical"]);
                        MConveyance = Convert.ToDecimal(dtJobMatrix.Rows[0]["Conveyance"]);
                    }
                }
                #endregion Matrix Check
                #region Check Exists
                sqlText = @"Select count(EmployeeId) from EmployeeSalaryStructure 
                                where EmployeeId=@EmployeeId";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);
                #region SG Update
                sqlText = "Select count(Id) from EmployeeStructureGroup where EmployeeId=@EmployeeId";
                SqlCommand cmdExists = new SqlCommand(sqlText, currConn);
                cmdExists.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdExists.Transaction = transaction;
                exeRes = cmdExists.ExecuteScalar();
                int exists = Convert.ToInt32(exeRes);
                if (exists <= 0)
                {
                    retResults[1] = "Please Save/Update Employee Job Information";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
                }
                sqlText = "  ";
                sqlText += @" update  EmployeeStructureGroup set 
                            SalaryStructureId=@SalaryStructureId
                            ,GradeId=@GradeId
                            ,StepId=@StepId
                            ,IsGross=@IsGross
                            ,LastUpdateBy=@LastUpdateBy
                            ,LastUpdateAt=@LastUpdateAt
                            ,LastUpdateFrom=@LastUpdateFrom
                            where EmployeeId=@EmployeeId   ";
                SqlCommand cmdesg = new SqlCommand(sqlText, currConn);
                cmdesg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdesg.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                cmdesg.Parameters.AddWithValue("@GradeId", gradeId);
                cmdesg.Parameters.AddWithValue("@StepId", stepId);
                cmdesg.Parameters.AddWithValue("@IsGross", isGross);
                cmdesg.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                cmdesg.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                cmdesg.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                cmdesg.Transaction = transaction;
                cmdesg.ExecuteNonQuery();
                #endregion SG Update
                if (count > 1)
                {
                    retResults[1] = "This Employee Already Assign the Salary Structure and can't Update\nYou can Give Increment";
                    throw new ArgumentNullException("This Employee Already Assign the Salary Structure and can't Update\nYou can Give Increment", "");
                }
                else if (count == 1)
                {
                    #region Update

                    #region OldData1
                    dsOld1 = new DataSet();
                    sqlText = "";
                    sqlText += @" 
                select * from EmployeeSalaryStructureDetail where employeeid=@EmployeeId and salarytypeid 
                not in ( select salarytypeid from SalaryStructureDetail where SalaryStructureId=@SalaryStructureId)
                and salaryType not in('Gross','Basic')  and IsFixed=0 and IsEarning=1   
                ";
                    SqlCommand objComOld1 = new SqlCommand();
                    objComOld1.Connection = currConn;
                    objComOld1.CommandText = sqlText;
                    objComOld1.CommandType = CommandType.Text;
                    objComOld1.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                    objComOld1.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    objComOld1.Transaction = transaction;
                    SqlDataAdapter daOld1 = new SqlDataAdapter(objComOld1);
                    daOld1.Fill(dsOld1);
                    #endregion OldData1
                    #region OldData2
                    dsOld2 = new DataSet();
                    sqlText = "";
                    sqlText += @" 
                    select * from EmployeeSalaryStructureDetail where employeeid=@EmployeeId and salarytypeid 
                    not in ( select salarytypeid from SalaryStructureDetail where SalaryStructureId=@SalaryStructureId) 
                    and salaryType not in('Gross','Basic')  and IsFixed=1 and IsEarning=1   
                ";
                    SqlCommand objComOld2 = new SqlCommand();
                    objComOld2.Connection = currConn;
                    objComOld2.CommandText = sqlText;
                    objComOld2.CommandType = CommandType.Text;
                    objComOld2.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                    objComOld2.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    objComOld2.Transaction = transaction;
                    SqlDataAdapter daOld2 = new SqlDataAdapter(objComOld2);
                    daOld2.Fill(dsOld2);
                    #endregion OldData2
                    #region OldData3
                    dsOld3 = new DataSet();
                    sqlText = "";
                    sqlText += @" 
                    select * from EmployeeSalaryStructureDetail where employeeid=@EmployeeId and salarytypeid 
                    not in ( select salarytypeid from SalaryStructureDetail where SalaryStructureId=@SalaryStructureId) 
                    and salaryType not in('Gross','Basic')  and IsFixed=0 and IsEarning=0   
                ";
                    SqlCommand objComOld3 = new SqlCommand();
                    objComOld3.Connection = currConn;
                    objComOld3.CommandText = sqlText;
                    objComOld3.CommandType = CommandType.Text;
                    objComOld3.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                    objComOld3.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    objComOld3.Transaction = transaction;
                    SqlDataAdapter daOld3 = new SqlDataAdapter(objComOld3);
                    daOld3.Fill(dsOld3);
                    #endregion OldData3
                    #region OldData4
                    dsOld4 = new DataSet();
                    sqlText = "";
                    sqlText += @" 
                    select * from EmployeeSalaryStructureDetail where employeeid=@EmployeeId and salarytypeid 
                    not in ( select salarytypeid from SalaryStructureDetail where SalaryStructureId=@SalaryStructureId)
                    and salaryType not in('Gross','Basic')  and IsFixed=1 and IsEarning=0   
                ";
                    SqlCommand objComOld4 = new SqlCommand();
                    objComOld4.Connection = currConn;
                    objComOld4.CommandText = sqlText;
                    objComOld4.CommandType = CommandType.Text;
                    objComOld4.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                    objComOld4.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    objComOld4.Transaction = transaction;
                    SqlDataAdapter daOld4 = new SqlDataAdapter(objComOld4);
                    daOld4.Fill(dsOld4);
                    #endregion OldData
                    #region Delete EmployeeSalaryStructureDetail
                    sqlText = "  ";
                    sqlText += @" delete from EmployeeSalaryStructureDetail 
                                where EmployeeId=@EmployeeId  ";
                    SqlCommand cmdsgD = new SqlCommand(sqlText, currConn);
                    cmdsgD.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdsgD.Transaction = transaction;
                    cmdsgD.ExecuteNonQuery();
                    #endregion Delete EmployeeSalaryStructureDetail
                    #region Delete EmployeeSalaryStructure
                    sqlText = "  ";
                    sqlText += @" delete from EmployeeSalaryStructure 
                                where EmployeeId=@EmployeeId  ";
                    SqlCommand cmdsgH = new SqlCommand(sqlText, currConn);
                    cmdsgH.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdsgH.Transaction = transaction;
                    cmdsgH.ExecuteNonQuery();
                    #endregion Delete EmployeeSalaryStructure
                    #endregion Update
                }
                #region  dsGB
                DataSet dsGB = new DataSet();
                sqlText = "";
                sqlText += @" 
                            SELECT *  From EnumSalaryType where  TypeName in('Gross') and  BranchId=@BranchId
                            SELECT *  From EnumSalaryType where  TypeName in('Basic') and  BranchId=@BranchId
";
                SqlCommand objCommGB = new SqlCommand();
                objCommGB.Connection = currConn;
                objCommGB.CommandText = sqlText;
                objCommGB.CommandType = CommandType.Text;
                objCommGB.Parameters.AddWithValue("@BranchId", siVM.BranchId);
                objCommGB.Transaction = transaction;
                SqlDataAdapter daGB = new SqlDataAdapter(objCommGB);
                daGB.Fill(dsGB);
                #endregion  dsGB
                #region Insert
                #region EmployeeSalaryStructure
                sqlText = "Select isnull(max(convert(int, Id)),0) from EmployeeSalaryStructure ";
                SqlCommand cmdEmployeeSalaryStructureId = new SqlCommand(sqlText, currConn);
                cmdEmployeeSalaryStructureId.Transaction = transaction;
                exeRes = cmdEmployeeSalaryStructureId.ExecuteScalar();
                EmployeeSalaryStructureNewId = Convert.ToInt32(exeRes);
                EmployeeSalaryStructureNewId++;
                sqlText = "";
                sqlText += @" SELECT *  From SalaryStructure where  Id=@Id ";
                SqlCommand objss = new SqlCommand();
                objss.Connection = currConn;
                objss.CommandText = sqlText;
                objss.CommandType = CommandType.Text;
                objss.Parameters.AddWithValue("@Id", SalaryStructureId);
                objss.Transaction = transaction;
                SqlDataAdapter daSS = new SqlDataAdapter(objss);
                daSS.Fill(dsSS);
                foreach (var item in dsSS.Tables[0].Rows)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeSalaryStructure(
                                    Id,EmployeeId,SalaryStructureId,TotalValue,IncrementDate,BranchId,IsCurrent,Remarks,IsActive,IsArchive
                                    ,CreatedBy,CreatedAt,CreatedFrom
                                    )VALUES (
                                    @Id,@EmployeeId,@SalaryStructureId,@TotalValue,@IncrementDate,@BranchId,@IsCurrent,@Remarks,@IsActive
                                    ,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                    ) ";
                    SqlCommand cmdInsESSD = new SqlCommand(sqlText, currConn);
                    cmdInsESSD.Parameters.AddWithValue("@Id", EmployeeSalaryStructureNewId);
                    cmdInsESSD.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdInsESSD.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                    cmdInsESSD.Parameters.AddWithValue("@TotalValue", BasicSalary);
                    cmdInsESSD.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                    cmdInsESSD.Parameters.AddWithValue("@BranchId", siVM.BranchId);
                    cmdInsESSD.Parameters.AddWithValue("@IsCurrent", true);
                    cmdInsESSD.Parameters.AddWithValue("@Remarks", Convert.DBNull);
                    cmdInsESSD.Parameters.AddWithValue("@IsActive", true);
                    cmdInsESSD.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsESSD.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdInsESSD.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdInsESSD.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdInsESSD.Transaction = transaction;
                    cmdInsESSD.ExecuteNonQuery();
                    //EmployeeSalaryStructureNewId++;
                }
                #endregion EmployeeSalaryStructure
                #region Update Promotion
                sqlText = "  ";
                sqlText += @" update  EmployeePromotion set 
                            GradeId=@GradeId
                            ,StepId=@StepId
                            ,LastUpdateBy=@LastUpdateBy
                            ,LastUpdateAt=@LastUpdateAt
                            ,LastUpdateFrom=@LastUpdateFrom
                            where IsCurrent=1 and EmployeeId=@EmployeeId   ";
                SqlCommand cmdep = new SqlCommand(sqlText, currConn);
                cmdep.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdep.Parameters.AddWithValue("@GradeId", gradeId);
                cmdep.Parameters.AddWithValue("@StepId", stepId);
                cmdep.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                cmdep.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                cmdep.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                cmdep.Transaction = transaction;
                cmdep.ExecuteNonQuery();
                #endregion Update Promotion
                #region Save
                #region sql statement
                sqlText = "Select isnull(max(convert(int, Id)),0) from EmployeeSalaryStructureDetail ";
                SqlCommand cmdId = new SqlCommand(sqlText, currConn);
                cmdId.Transaction = transaction;
                exeRes = cmdId.ExecuteScalar();
                int NewId = Convert.ToInt32(exeRes);
                DataSet ds = new DataSet();
                sqlText = "";
                sqlText += @" 
                SELECT *  From SalaryStructureDetail where  SalaryType in('Other') and IsFixed=0 and IsEarning=1 and  SalaryStructureId=@SalaryStructureId
                SELECT *  From SalaryStructureDetail where  SalaryType in('Other') and IsFixed=1 and IsEarning=1 and  SalaryStructureId=@SalaryStructureId
                SELECT *  From SalaryStructureDetail where  SalaryType in('Other') and IsFixed=0 and IsEarning=0 and  SalaryStructureId=@SalaryStructureId
                SELECT *  From SalaryStructureDetail where  SalaryType in('Other') and IsFixed=1 and IsEarning=0 and  SalaryStructureId=@SalaryStructureId
                SELECT *  From SalaryStructureDetail where  SalaryType in('Medical') and   SalaryStructureId=@SalaryStructureId
                SELECT *  From SalaryStructureDetail where  SalaryType in('Conveyance') and   SalaryStructureId=@SalaryStructureId
                SELECT *  From SalaryStructureDetail where  SalaryType in('HouseRent') and   SalaryStructureId=@SalaryStructureId
                ";
                SqlCommand objCommssd1 = new SqlCommand();
                objCommssd1.Connection = currConn;
                objCommssd1.CommandText = sqlText;
                objCommssd1.CommandType = CommandType.Text;
                objCommssd1.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                objCommssd1.Transaction = transaction;
                SqlDataAdapter da = new SqlDataAdapter(objCommssd1);
                da.Fill(ds);
                sqlText = "  ";
                sqlText += @" INSERT INTO EmployeeSalaryStructureDetail(
                                            Id,EmployeeSalaryStructureId,SalaryTypeId,SalaryType,EmployeeId,IsFixed,IsEarning,Portion,PortionSalaryType,Amount,IncrementDate,IsCurrent
                                            ,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom 
                                            )VALUES (
                                            @Id,@EmployeeSalaryStructureId,@SalaryTypeId,@SalaryType,@EmployeeId,@IsFixed,@IsEarning,@Portion,@PortionSalaryType,@Amount,@IncrementDate,@IsCurrent
                                            ,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom ) 
                                            ";
                #endregion  sql statement
                GrossSalary = BasicSalary;
                #region drNotFixedEarning
                foreach (DataRow drNotFixedEarning in ds.Tables[0].Rows)
                {
                    Portion = 0;
                    NewId++;
                    Portion = Convert.ToDecimal(drNotFixedEarning["Portion"].ToString());
                    SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                    GrossSalary = GrossSalary + SalaryAmount;
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                    cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                    cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedEarning["SalaryTypeId"].ToString());
                    cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedEarning["SalaryType"].ToString());
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedEarning["Portion"].ToString());
                    cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                    cmdInsert.Parameters.AddWithValue("@IsFixed", false);
                    cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                    cmdInsert.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);
                    cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                #endregion drNotFixedEarning
                #region drFixedEarning
                foreach (DataRow drFixedEarning in ds.Tables[1].Rows)
                {
                    Portion = 0;
                    NewId++;
                    Portion = Convert.ToDecimal(drFixedEarning["Portion"].ToString());
                    SalaryAmount = Math.Ceiling(Portion);
                    GrossSalary = GrossSalary + SalaryAmount;
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                    cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                    cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drFixedEarning["SalaryTypeId"].ToString());
                    cmdInsert.Parameters.AddWithValue("@SalaryType", drFixedEarning["SalaryType"].ToString());
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Portion", drFixedEarning["Portion"].ToString());
                    cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                    cmdInsert.Parameters.AddWithValue("@IsFixed", true);
                    cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                    cmdInsert.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);

                    cmdInsert.Parameters.AddWithValue("@Remarks", drFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                #endregion drFixedEarning
                #region drNotFixedDeduction
                foreach (DataRow drNotFixedDeduction in ds.Tables[2].Rows)
                {
                    Portion = 0;
                    NewId++;
                    Portion = Convert.ToDecimal(drNotFixedDeduction["Portion"].ToString());
                    SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                    cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                    cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedDeduction["SalaryTypeId"].ToString());
                    cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedDeduction["SalaryType"].ToString());
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedDeduction["Portion"].ToString());
                    cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedDeduction["PortionSalaryType"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                    cmdInsert.Parameters.AddWithValue("@IsFixed", false);
                    cmdInsert.Parameters.AddWithValue("@IsEarning", false);
                    cmdInsert.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);

                    cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedDeduction["Remarks"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                #endregion drNotFixedDeduction
                #region drFixedDeduction
                foreach (DataRow drFixedDeduction in ds.Tables[3].Rows)
                {
                    Portion = 0;
                    NewId++;
                    Portion = Convert.ToDecimal(drFixedDeduction["Portion"].ToString());
                    SalaryAmount = Math.Ceiling(Portion);
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                    cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                    cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drFixedDeduction["SalaryTypeId"].ToString());
                    cmdInsert.Parameters.AddWithValue("@SalaryType", drFixedDeduction["SalaryType"].ToString());
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Portion", drFixedDeduction["Portion"].ToString());
                    cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drFixedDeduction["PortionSalaryType"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                    cmdInsert.Parameters.AddWithValue("@IsFixed", true);
                    cmdInsert.Parameters.AddWithValue("@IsEarning", false);
                    cmdInsert.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);

                    cmdInsert.Parameters.AddWithValue("@Remarks", drFixedDeduction["Remarks"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                #endregion drFixedDeduction
                #region drMedical
                foreach (DataRow drNotFixedEarning in ds.Tables[4].Rows)//Medical
                {
                    Portion = 0;
                    NewId++;
                    IsFixed = Convert.ToBoolean(drNotFixedEarning["IsFixed"].ToString());
                    Portion = Convert.ToDecimal(drNotFixedEarning["Portion"].ToString());
                    SalaryAmount = Portion;
                    if (BasicSalary != MBasic)
                    {
                        if (!IsFixed)
                        {
                            SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                        }
                    }
                    else
                    {
                        if (MMedical > 0)
                        {
                            SalaryAmount = MMedical;
                        }
                        else
                        {
                            if (!IsFixed)
                            {
                                SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                            }
                        }
                    }
                    if (!SalaryFromMatrix)
                    {
                        if (!IsFixed)
                            SalaryAmount = MedicalSalary;
                    }
                    GrossSalary = GrossSalary + SalaryAmount;
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                    cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                    cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedEarning["SalaryTypeId"].ToString());
                    cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedEarning["SalaryType"].ToString());
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedEarning["Portion"].ToString());
                    cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                    cmdInsert.Parameters.AddWithValue("@IsFixed", IsFixed);
                    cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                    cmdInsert.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);
                    cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                #endregion drMedical
                #region drConveyance
                foreach (DataRow drNotFixedEarning in ds.Tables[5].Rows)//Conveyance
                {
                    Portion = 0;
                    NewId++;
                    IsFixed = Convert.ToBoolean(drNotFixedEarning["IsFixed"].ToString());
                    Portion = Convert.ToDecimal(drNotFixedEarning["Portion"].ToString());
                    SalaryAmount = Portion;
                    if (BasicSalary != MBasic)
                    {
                        if (!IsFixed)
                        {
                            SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                        }
                    }
                    else
                    {
                        if (MConveyance > 0)
                        {
                            SalaryAmount = MConveyance;
                        }
                        else
                        {
                            if (!IsFixed)
                            {
                                SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                            }
                        }
                    }
                    if (!SalaryFromMatrix)
                    {
                        if (!IsFixed)
                            SalaryAmount = ConvenceSalary;
                    }
                    GrossSalary = GrossSalary + SalaryAmount;
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                    cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                    cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedEarning["SalaryTypeId"].ToString());
                    cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedEarning["SalaryType"].ToString());
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedEarning["Portion"].ToString());
                    cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                    cmdInsert.Parameters.AddWithValue("@IsFixed", IsFixed);
                    cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                    cmdInsert.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);

                    cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                #endregion drConveyance
                #region drHR
                foreach (DataRow drNotFixedEarning in ds.Tables[6].Rows)//HR
                {
                    Portion = 0;
                    NewId++;
                    IsFixed = Convert.ToBoolean(drNotFixedEarning["IsFixed"].ToString());
                    Portion = Convert.ToDecimal(drNotFixedEarning["Portion"].ToString());
                    SalaryAmount = Portion;
                    if (!IsFixed)
                    {
                        SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                    }
                    if (!SalaryFromMatrix)
                    {
                        if (!IsFixed)
                            SalaryAmount = HouseRentSalary;
                    }

                    GrossSalary = GrossSalary + SalaryAmount;
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                    cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                    cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedEarning["SalaryTypeId"].ToString());
                    cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedEarning["SalaryType"].ToString());
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedEarning["Portion"].ToString());
                    cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                    cmdInsert.Parameters.AddWithValue("@IsFixed", IsFixed);
                    cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                    cmdInsert.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);

                    cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                #endregion drHR
                #region OldDataInstert
                if (count == 1)
                {
                    #region drNotFixedEarning
                    foreach (DataRow drNotFixedEarning in dsOld1.Tables[0].Rows)
                    {
                        Portion = 0;
                        NewId++;
                        Portion = Convert.ToDecimal(drNotFixedEarning["Portion"].ToString());
                        SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                        GrossSalary = GrossSalary + SalaryAmount;
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedEarning["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedEarning["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedEarning["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", false);
                        cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                        cmdInsert.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                        cmdInsert.Parameters.AddWithValue("@IsCurrent", true);

                        cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion drNotFixedEarning
                    #region drFixedEarning
                    foreach (DataRow drFixedEarning in dsOld2.Tables[0].Rows)
                    {
                        Portion = 0;
                        NewId++;
                        Portion = Convert.ToDecimal(drFixedEarning["Portion"].ToString());
                        SalaryAmount = Math.Ceiling(Portion);
                        GrossSalary = GrossSalary + SalaryAmount;
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drFixedEarning["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drFixedEarning["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@Portion", drFixedEarning["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", true);
                        cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                        cmdInsert.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                        cmdInsert.Parameters.AddWithValue("@IsCurrent", true);

                        cmdInsert.Parameters.AddWithValue("@Remarks", drFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion drFixedEarning
                    #region drNotFixedDeduction
                    foreach (DataRow drNotFixedDeduction in dsOld3.Tables[0].Rows)
                    {
                        Portion = 0;
                        NewId++;
                        Portion = Convert.ToDecimal(drNotFixedDeduction["Portion"].ToString());
                        SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedDeduction["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedDeduction["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedDeduction["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedDeduction["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", false);
                        cmdInsert.Parameters.AddWithValue("@IsEarning", false);
                        cmdInsert.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                        cmdInsert.Parameters.AddWithValue("@IsCurrent", true);

                        cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedDeduction["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion drNotFixedDeduction
                    #region drFixedDeduction
                    foreach (DataRow drFixedDeduction in dsOld4.Tables[0].Rows)
                    {
                        Portion = 0;
                        NewId++;
                        Portion = Convert.ToDecimal(drFixedDeduction["Portion"].ToString());
                        SalaryAmount = Math.Ceiling(Portion);
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drFixedDeduction["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drFixedDeduction["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@Portion", drFixedDeduction["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drFixedDeduction["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", true);
                        cmdInsert.Parameters.AddWithValue("@IsEarning", false);
                        cmdInsert.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                        cmdInsert.Parameters.AddWithValue("@IsCurrent", true);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drFixedDeduction["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion drFixedDeduction
                }
                #endregion OldDataInstert
                #region drGross
                foreach (DataRow drGross in dsGB.Tables[0].Rows)
                {
                    Portion = 0;
                    NewId++;
                    if (isGross)
                    {
                        GrossSalary = salaryInput;
                    }
                    Portion = GrossSalary;
                    SalaryAmount = Math.Ceiling(Portion);
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                    cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                    cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drGross["Id"].ToString());
                    cmdInsert.Parameters.AddWithValue("@SalaryType", "Gross");
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Portion", Portion);
                    cmdInsert.Parameters.AddWithValue("@PortionSalaryType", "Gross");
                    cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                    cmdInsert.Parameters.AddWithValue("@IsFixed", true);
                    cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                    cmdInsert.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);
                    cmdInsert.Parameters.AddWithValue("@Remarks", drGross["Remarks"].ToString() ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                #endregion drGross
                #region drBasic
                foreach (DataRow drBasic in dsGB.Tables[1].Rows)
                {
                    Portion = 0;
                    NewId++;
                    Portion = BasicSalary;
                    SalaryAmount = Math.Ceiling(Portion);
                    SqlCommand cmdBasic = new SqlCommand(sqlText, currConn);
                    cmdBasic.Parameters.AddWithValue("@Id", (NewId));
                    cmdBasic.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                    cmdBasic.Parameters.AddWithValue("@SalaryTypeId", drBasic["Id"].ToString());
                    cmdBasic.Parameters.AddWithValue("@SalaryType", "Basic");
                    cmdBasic.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdBasic.Parameters.AddWithValue("@Portion", Portion);
                    cmdBasic.Parameters.AddWithValue("@PortionSalaryType", "Basic");
                    cmdBasic.Parameters.AddWithValue("@Amount", SalaryAmount);
                    cmdBasic.Parameters.AddWithValue("@IsFixed", true);
                    cmdBasic.Parameters.AddWithValue("@IsEarning", true);
                    cmdBasic.Parameters.AddWithValue("@IncrementDate", EmployeeJoiningDate);
                    cmdBasic.Parameters.AddWithValue("@IsCurrent", true);
                    cmdBasic.Parameters.AddWithValue("@Remarks", drBasic["Remarks"].ToString() ?? Convert.DBNull);
                    cmdBasic.Parameters.AddWithValue("@IsActive", true);
                    cmdBasic.Parameters.AddWithValue("@IsArchive", false);
                    cmdBasic.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdBasic.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdBasic.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdBasic.Transaction = transaction;
                    cmdBasic.ExecuteNonQuery();
                }
                #endregion drBasic
                #endregion Save
                #endregion Insert

                #endregion Check Exists

                sqlText = "";
                if (!isGross)
                {
                    sqlText = "";
                    sqlText = @" 
update EmployeeSalaryStructureDetail set Amount=earn.amount
 from(
 select sum(amount)amount from EmployeeSalaryStructureDetail
 where 1=1 
 and IsEarning=1 and SalaryType not in('gross') and EmployeeId=@EmployeeId ) earn
 where EmployeeSalaryStructureDetail.EmployeeId=@EmployeeId
 and SalaryType in('gross') 
 update employeejob set GrossSalary=earn.amount
 from(
 select sum(amount)amount from EmployeeSalaryStructureDetail
 where 1=1 
 and IsEarning=1 and SalaryType not in('gross') and EmployeeId=@EmployeeId ) earn
 where employeejob.EmployeeId=@EmployeeId
 update employeejob set BasicSalary=earn.amount
 from(
 select sum(amount)amount from EmployeeSalaryStructureDetail
 where 1=1 
 and IsEarning=1 and SalaryType  in('basic') and EmployeeId=@EmployeeId ) earn
 where employeejob.EmployeeId=@EmployeeId
 ";
                }
                else
                {
                    sqlText = "";
                    sqlText = @" 
 update employeejob set GrossSalary=@GrossSalary,BasicSalary=@BasicSalary,BankPayAmount=@BankPayAmount
 where EmployeeId=@EmployeeId
";
                }
                SqlCommand cmdUpdateEarn = new SqlCommand(sqlText, currConn);
                cmdUpdateEarn.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                if (isGross)
                {
                    cmdUpdateEarn.Parameters.AddWithValue("@BasicSalary", BasicSalary);
                    cmdUpdateEarn.Parameters.AddWithValue("@BankPayAmount", BankPayAmount);
                    if (SalaryFromMatrix)
                    {
                        cmdUpdateEarn.Parameters.AddWithValue("@GrossSalary", GrossSalary);
                    }
                    else
                    {
                        cmdUpdateEarn.Parameters.AddWithValue("@GrossSalary", GrossSalaryInput);
                    }
                }
                cmdUpdateEarn.Transaction = transaction;
                exeRes = cmdUpdateEarn.ExecuteNonQuery();



                //transResult = Convert.ToInt32(exeRes);
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
                FileLogger.Log("Fail" + Environment.NewLine + ex.Message + Environment.NewLine + "EmployeeSalaryStructureFromBasic", this.GetType().Name, ex.Message + Environment.NewLine + sqlText.ToString() + Environment.NewLine + ex.StackTrace);

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

        public string[] EmployeeSalaryStructureTIB(string EmployeeId, string SalaryStructureId, decimal salaryInput1, string gradeId, string stepId, bool isGross, decimal BankPayAmount, ShampanIdentityVM siVM, EmployeeStructureGroupVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            DataSet dsOld1 = new DataSet();
            DataSet dsOld2 = new DataSet();
            DataSet dsOld3 = new DataSet();
            DataSet dsOld4 = new DataSet();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            string stepName = "";
            decimal GrossSalary = 0;
            decimal SalaryAmount = 0;
            decimal MBasic = 0;
            decimal MMedical = 0;
            decimal MConveyance = 0;
            decimal Portion = 0;
            int EmployeeSalaryStructureNewId = 0;
            string EmployeeJoiningDate = "";
            string EmployeeJobId = "0";
            bool IsFixed = false;
            decimal GrossSalaryInput = 0;
            decimal BasicSalary = 0;
            decimal MedicalSalary = 0;
            decimal HouseRentSalary = 0;
            decimal ConvenceSalary = 0;

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
                SettingDAL _setDAL = new SettingDAL();
                var SalaryFromMatrix = Convert.ToBoolean(_setDAL.settingValue("Salary", "SalaryFromMatrix", currConn, transaction).ToLower() == "y" ? true : false);

                vm.basic = salaryInput1;
                sqlText = "select count(Id) from EmployeeSalaryStructure where EmployeeId=@EmployeeId  ";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                int count = Convert.ToInt32(cmd.ExecuteScalar());


                if (count != 0)
                {
                    string deleteText = @"

delete  EmployeeSalaryStructureDetail
where EmployeeID= @EmployeeID

delete from EmployeeSalaryStructure
where EmployeeID= @EmployeeID

update EmployeeStructureGroup set gradeId=@gradeId, StepId=@StepId, SalaryStructureId=@SalaryStructureId where EmployeeId=@EmployeeID
 update EmployeePromotion set  gradeId=@gradeId, StepId=@StepId where EmployeeId=@EmployeeID and IsCurrent=1


";

                    cmd.CommandText = deleteText;

                    cmd.Parameters.AddWithValueAndParamCheck("@EmployeeID", EmployeeId);
                    cmd.Parameters.AddWithValueAndParamCheck("@gradeId", gradeId);
                    cmd.Parameters.AddWithValueAndParamCheck("@StepId", stepId);
                    cmd.Parameters.AddWithValueAndParamCheck("@SalaryStructureId", SalaryStructureId);
                    cmd.ExecuteNonQuery();
                }

                sqlText = "select isnull(max(cast(ID as int)),0)+1 from EmployeeSalaryStructure";
                cmd.CommandText = sqlText;
                string id = cmd.ExecuteScalar().ToString();


                sqlText = @"
                    insert into EmployeeSalaryStructure 
                    (
                        ID
                        ,EmployeeId
                        ,SalaryStructureId
                        ,TotalValue
                        ,IncrementDate
                        ,BranchId
                        ,Remarks
                        ,IsActive
                        ,IsArchive
                        ,CreatedBy
                        ,CreatedAt
                        ,CreatedFrom
                        ,LastUpdateBy
                        ,LastUpdateAt
                        ,LastUpdateFrom
                        ,IsCurrent
                    )
                    values (
                         @ID
                        ,@EmployeeId
                        ,@SalaryStructureId
                        ,@TotalValue
                        ,@IncrementDate
                        ,@BranchId
                        ,@Remarks
                        ,@IsActive
                        ,@IsArchive
                        ,@CreatedBy
                        ,@CreatedAt
                        ,@CreatedFrom
                        ,@LastUpdateBy
                        ,@LastUpdateAt
                        ,@LastUpdateFrom
                        ,@IsCurrent
                    )";

                cmd.CommandText = sqlText;

                cmd.Parameters.AddWithValueAndParamCheck("@ID", id);
                cmd.Parameters.AddWithValueAndParamCheck("@EmployeeId", EmployeeId);
                cmd.Parameters.AddWithValueAndParamCheck("@SalaryStructureId", SalaryStructureId);
                cmd.Parameters.AddWithValueAndParamCheck("@TotalValue", vm.basic + vm.Medical + vm.TA + vm.Housing); // need to update
                cmd.Parameters.AddWithValueAndParamCheck("@IncrementDate", 0);// need to update
                cmd.Parameters.AddWithValueAndParamCheck("@BranchId", siVM.BranchId);
                cmd.Parameters.AddWithValueAndParamCheck("@Remarks", "-");
                cmd.Parameters.AddWithValueAndParamCheck("@IsActive", 1);
                cmd.Parameters.AddWithValueAndParamCheck("@IsArchive", 0);
                cmd.Parameters.AddWithValueAndParamCheck("@CreatedBy", siVM.CreatedBy);
                cmd.Parameters.AddWithValueAndParamCheck("@CreatedAt", siVM.CreatedAt);
                cmd.Parameters.AddWithValueAndParamCheck("@CreatedFrom", siVM.CreatedFrom);
                cmd.Parameters.AddWithValueAndParamCheck("@LastUpdateBy", siVM.LastUpdateBy);
                cmd.Parameters.AddWithValueAndParamCheck("@LastUpdateAt", siVM.LastUpdateAt);
                cmd.Parameters.AddWithValueAndParamCheck("@LastUpdateFrom", siVM.LastUpdateFrom);
                cmd.Parameters.AddWithValueAndParamCheck("@IsCurrent", true);


                cmd.ExecuteNonQuery();

                sqlText = @"update EmployeeSalaryStructure set IncrementDate = EmployeeJob.JoinDate
                                from EmployeeJob 
                                where EmployeeJob.EmployeeId = EmployeeSalaryStructure.EmployeeId";

                cmd.CommandText = sqlText;
                cmd.ExecuteNonQuery();



                sqlText = @"
select * from(
select 1 SL,SalaryTypeId,SalaryType, Portion,isnull( PortionSalaryType,'Basic')PortionSalaryType, IsFixed,IsGross, IsEarning from SalaryStructureDetail 
where  SalaryStructureId='1_1'
and SalaryType not in ('other')
 union all
 select 3 SL,SalaryTypeId,SalaryType, Portion,isnull( PortionSalaryType,'Basic')PortionSalaryType, IsFixed,IsGross, IsEarning from SalaryStructureDetail 
where  SalaryStructureId='1_1'
and SalaryType in ('other')
and IsFixed=1

 union all
 select 4 SL,SalaryTypeId,SalaryType, Portion,isnull( PortionSalaryType,'Basic')PortionSalaryType, IsFixed,IsGross, IsEarning from SalaryStructureDetail 
where  SalaryStructureId='1_1'
and SalaryType in ('other')
and IsFixed=0 
union 
Select 2 SL,ID,Name,0,'Gross',0,0,1 from EnumSalaryType where TypeName = 'GROSS'
) St
order by sl
";

                SqlCommand cmdDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDetail.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmdDetail);
                DataTable dtSalaryStructures = new DataTable();

                dataAdapter.Fill(dtSalaryStructures);




                if (dtSalaryStructures.Rows.Count == 0)
                {
                    throw new Exception("No Data Found in Salary Structure");
                }
                #region insert into EmployeeSalaryStructureDetail

                string detailInsertText = @"
insert into EmployeeSalaryStructureDetail
(
ID
,SalaryTypeId
,SalaryType
,EmployeeId
,IsFixed
,Portion
,PortionSalaryType
,Amount
,EmployeeSalaryStructureId,
Remarks 
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,IsEarning
,IncrementDate
,IsCurrent
)

values (

 @ID
,@SalaryTypeId
,@SalaryType
,@EmployeeId
,@IsFixed
,@Portion
,@PortionSalaryType
,@Amount
,@EmployeeSalaryStructureId
,@Remarks 
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@LastUpdateBy
,@LastUpdateAt
,@LastUpdateFrom
,@IsEarning
,@IncrementDate
,@IsCurrent
)";
                #endregion insert into EmployeeSalaryStructureDetail

                cmdDetail.CommandText = detailInsertText;

                string detailId = "";
                foreach (DataRow dataRow in dtSalaryStructures.Rows)
                {
                    //cmdDetail.Parameters.AddWithValue()


                    sqlText = "select isnull(max(cast(ID as int)),0)+1 from EmployeeSalaryStructureDetail";
                    cmd.CommandText = sqlText;
                    detailId = cmd.ExecuteScalar().ToString();



                    if (dataRow["SL"].ToString() == "1")
                    {
                        if (dataRow["SalaryType"].ToString().ToLower() == "basic")
                        {
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.basic);
                        }
                        else if (dataRow["SalaryType"].ToString().ToLower() == "medical")
                        {
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.Medical);
                        }
                        else if (dataRow["SalaryType"].ToString().ToLower() == "conveyance")
                        {
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.TA);
                        }
                        else if (dataRow["SalaryType"].ToString().ToLower() == "houserent")
                        {
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.Housing);
                        }
                    }
                    if (dataRow["SL"].ToString() == "2")
                    {
                        if (dataRow["SalaryType"].ToString().ToLower() == "gross")
                        {
                            vm.gross = vm.Medical + vm.TA + vm.Housing + vm.basic;
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount",
                               vm.gross);
                        }
                    }
                    else if (dataRow["SL"].ToString() == "3")
                    {
                        decimal amount = Convert.ToDecimal(dataRow["Portion"]);
                        cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", amount);
                    }
                    else if (dataRow["SL"].ToString() == "4")
                    {
                        decimal portion = Convert.ToDecimal(dataRow["Portion"]);
                        decimal amount = 0;

                        if (Convert.ToBoolean(dataRow["IsGross"]))
                        {
                            amount = vm.gross * portion / 100;
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", amount);
                        }
                        else
                        {
                            amount = vm.basic * portion / 100;
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", amount);
                        }
                    }
                    string SType = dataRow["SalaryTypeId"].ToString();
                    string IsEarning = dataRow["IsEarning"].ToString();
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@ID", detailId);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@SalaryTypeId", dataRow["SalaryTypeId"].ToString());
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@SalaryType", dataRow["SalaryType"].ToString());
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@EmployeeId", EmployeeId);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsFixed", dataRow["IsFixed"].ToString()); // need update
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@Portion", dataRow["Portion"].ToString()); // need update
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@PortionSalaryType", dataRow["PortionSalaryType"].ToString());

                    cmdDetail.Parameters.AddWithValueAndParamCheck("@EmployeeSalaryStructureId", id);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@Remarks", "-");
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsActive", 1);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsArchive", 0);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@CreatedBy", siVM.CreatedBy);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@CreatedBy", siVM.CreatedBy);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@CreatedAt", siVM.CreatedAt);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@CreatedFrom", siVM.CreatedFrom);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@LastUpdateBy", siVM.LastUpdateBy);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@LastUpdateAt", siVM.LastUpdateAt);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@LastUpdateFrom", siVM.LastUpdateFrom); ;
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsEarning", dataRow["IsEarning"].ToString()); // need update
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IncrementDate", 0); // need update
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsCurrent", 1);


                    cmdDetail.ExecuteNonQuery();
                }


                sqlText = @"

update EmployeeSalaryStructureDetail set IncrementDate = EmployeeJob.JoinDate
from EmployeeJob 
where EmployeeJob.EmployeeId = EmployeeSalaryStructureDetail.EmployeeId

update EmployeeJob set BasicSalary = EmployeeSalaryStructureDetail.Amount
from EmployeeSalaryStructureDetail
where EmployeeSalaryStructureDetail.EmployeeId = EmployeeJob.EmployeeId
and EmployeeSalaryStructureDetail.SalaryType = 'Basic'

update EmployeeJob set GrossSalary = EmployeeSalaryStructureDetail.Amount
from EmployeeSalaryStructureDetail
where EmployeeSalaryStructureDetail.EmployeeId = EmployeeJob.EmployeeId
and EmployeeSalaryStructureDetail.SalaryType = 'Gross'

";

                cmd.CommandText = sqlText;

                cmd.ExecuteNonQuery();






                //transResult = Convert.ToInt32(exeRes);
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
                FileLogger.Log("Fail" + Environment.NewLine + ex.Message + Environment.NewLine + "EmployeeSalaryStructureFromBasic", this.GetType().Name, ex.Message + Environment.NewLine + sqlText.ToString() + Environment.NewLine + ex.StackTrace);

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
        public string[] EmployeeSalaryStructureG4S(string EmployeeId, string SalaryStructureId, decimal salaryInput1, string gradeId, string stepId, bool isGross, decimal BankPayAmount, ShampanIdentityVM siVM, EmployeeStructureGroupVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            DataSet dsOld1 = new DataSet();
            DataSet dsOld2 = new DataSet();
            DataSet dsOld3 = new DataSet();
            DataSet dsOld4 = new DataSet();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            string stepName = "";
            decimal GrossSalary = 0;
            decimal SalaryAmount = 0;
            decimal MBasic = 0;
            decimal MMedical = 0;
            decimal MConveyance = 0;
            decimal Portion = 0;
            int EmployeeSalaryStructureNewId = 0;
            string EmployeeJoiningDate = "";
            string EmployeeJobId = "0";
            bool IsFixed = false;
            decimal GrossSalaryInput = 0;
            decimal BasicSalary = 0;
            decimal MedicalSalary = 0;
            decimal HouseRentSalary = 0;
            decimal ConvenceSalary = 0;

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
                SettingDAL _setDAL = new SettingDAL();
                var SalaryFromMatrix = Convert.ToBoolean(_setDAL.settingValue("Salary", "SalaryFromMatrix", currConn, transaction).ToLower() == "y" ? true : false);

                vm.basic = salaryInput1;
                double inputSalary=(double)salaryInput1;
                sqlText = "select count(Id) from EmployeeSalaryStructure where EmployeeId=@EmployeeId  ";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                int count = Convert.ToInt32(cmd.ExecuteScalar());


                if (count != 0)
                {
                    string deleteText = @"

delete  EmployeeSalaryStructureDetail
where EmployeeID= @EmployeeID

delete from EmployeeSalaryStructure
where EmployeeID= @EmployeeID

update EmployeeStructureGroup set gradeId=@gradeId, StepId=@StepId, SalaryStructureId=@SalaryStructureId where EmployeeId=@EmployeeID
 update EmployeePromotion set  gradeId=@gradeId, StepId=@StepId where EmployeeId=@EmployeeID and IsCurrent=1


";

                    cmd.CommandText = deleteText;

                    cmd.Parameters.AddWithValueAndParamCheck("@EmployeeID", EmployeeId);
                    cmd.Parameters.AddWithValueAndParamCheck("@gradeId", gradeId);
                    cmd.Parameters.AddWithValueAndParamCheck("@StepId", stepId);
                    cmd.Parameters.AddWithValueAndParamCheck("@SalaryStructureId", SalaryStructureId);
                    cmd.ExecuteNonQuery();
                }

                sqlText = "select isnull(max(cast(ID as int)),0)+1 from EmployeeSalaryStructure";
                cmd.CommandText = sqlText;
                string id = cmd.ExecuteScalar().ToString();


                sqlText = @"
                    insert into EmployeeSalaryStructure 
                    (
                        ID
                        ,EmployeeId
                        ,SalaryStructureId
                        ,TotalValue
                        ,IncrementDate
                        ,BranchId
                        ,Remarks
                        ,IsActive
                        ,IsArchive
                        ,CreatedBy
                        ,CreatedAt
                        ,CreatedFrom
                        ,LastUpdateBy
                        ,LastUpdateAt
                        ,LastUpdateFrom
                        ,IsCurrent
                    )
                    values (
                         @ID
                        ,@EmployeeId
                        ,@SalaryStructureId
                        ,@TotalValue
                        ,@IncrementDate
                        ,@BranchId
                        ,@Remarks
                        ,@IsActive
                        ,@IsArchive
                        ,@CreatedBy
                        ,@CreatedAt
                        ,@CreatedFrom
                        ,@LastUpdateBy
                        ,@LastUpdateAt
                        ,@LastUpdateFrom
                        ,@IsCurrent
                    )";

                cmd.CommandText = sqlText;

                cmd.Parameters.AddWithValueAndParamCheck("@ID", id);
                cmd.Parameters.AddWithValueAndParamCheck("@EmployeeId", EmployeeId);
                cmd.Parameters.AddWithValueAndParamCheck("@SalaryStructureId", SalaryStructureId);
                cmd.Parameters.AddWithValueAndParamCheck("@TotalValue", vm.basic + vm.Medical + vm.TA + vm.Housing); // need to update
                cmd.Parameters.AddWithValueAndParamCheck("@IncrementDate", 0);// need to update
                cmd.Parameters.AddWithValueAndParamCheck("@BranchId", siVM.BranchId);
                cmd.Parameters.AddWithValueAndParamCheck("@Remarks", "-");
                cmd.Parameters.AddWithValueAndParamCheck("@IsActive", 1);
                cmd.Parameters.AddWithValueAndParamCheck("@IsArchive", 0);
                cmd.Parameters.AddWithValueAndParamCheck("@CreatedBy", siVM.CreatedBy);
                cmd.Parameters.AddWithValueAndParamCheck("@CreatedAt", siVM.CreatedAt);
                cmd.Parameters.AddWithValueAndParamCheck("@CreatedFrom", siVM.CreatedFrom);
                cmd.Parameters.AddWithValueAndParamCheck("@LastUpdateBy", siVM.LastUpdateBy);
                cmd.Parameters.AddWithValueAndParamCheck("@LastUpdateAt", siVM.LastUpdateAt);
                cmd.Parameters.AddWithValueAndParamCheck("@LastUpdateFrom", siVM.LastUpdateFrom);
                cmd.Parameters.AddWithValueAndParamCheck("@IsCurrent", true);


                cmd.ExecuteNonQuery();

                sqlText = @"update EmployeeSalaryStructure set IncrementDate = EmployeeJob.JoinDate
                                from EmployeeJob 
                                where EmployeeJob.EmployeeId = EmployeeSalaryStructure.EmployeeId";

                cmd.CommandText = sqlText;
                cmd.ExecuteNonQuery();



                sqlText = @"
select CASE
    WHEN SalaryType='Basic' THEN '3'
    WHEN SalaryType='Gross' THEN '2'

    ELSE SL
END SL,SalaryTypeId,SalaryType,PortionSalaryType,Portion,IsFixed,IsGross,IsEarning from(
select 1 SL,SalaryTypeId,SalaryType, Portion,isnull( PortionSalaryType,'Basic')PortionSalaryType, IsFixed,IsGross, IsEarning from SalaryStructureDetail 
where  SalaryStructureId='1_1'
and SalaryType not in ('other')



 union all
 select 4 SL,SalaryTypeId,SalaryType, Portion,isnull( PortionSalaryType,'Basic')PortionSalaryType, IsFixed,IsGross, IsEarning from SalaryStructureDetail 
where  SalaryStructureId='1_1'
and SalaryType in ('other')
and IsFixed=0 
union 
Select 2 SL,ID,Name,0,'Gross',1,1,1 from EnumSalaryType where TypeName = 'GROSS'
) St
order by sl
";

                SqlCommand cmdDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDetail.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmdDetail);
                DataTable dtSalaryStructures = new DataTable();

                dataAdapter.Fill(dtSalaryStructures);




                if (dtSalaryStructures.Rows.Count == 0)
                {
                    throw new Exception("No Data Found in Salary Structure");
                }
                #region insert into EmployeeSalaryStructureDetail

                string detailInsertText = @"
insert into EmployeeSalaryStructureDetail
(
ID
,SalaryTypeId
,SalaryType
,EmployeeId
,IsFixed
,Portion
,PortionSalaryType
,Amount
,EmployeeSalaryStructureId,
Remarks 
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,IsEarning
,IncrementDate
,IsCurrent
)

values (

 @ID
,@SalaryTypeId
,@SalaryType
,@EmployeeId
,@IsFixed
,@Portion
,@PortionSalaryType
,@Amount
,@EmployeeSalaryStructureId
,@Remarks 
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@LastUpdateBy
,@LastUpdateAt
,@LastUpdateFrom
,@IsEarning
,@IncrementDate
,@IsCurrent
)";
                #endregion insert into EmployeeSalaryStructureDetail

                cmdDetail.CommandText = detailInsertText;

                string detailId = "";
                foreach (DataRow dataRow in dtSalaryStructures.Rows)
                {
                    //cmdDetail.Parameters.AddWithValue()


                    sqlText = "select isnull(max(cast(ID as int)),0)+1 from EmployeeSalaryStructureDetail";
                    cmd.CommandText = sqlText;
                    detailId = cmd.ExecuteScalar().ToString();

                    if (dataRow["SL"].ToString() == "1")
                    {
                       
                        if (dataRow["SalaryType"].ToString().ToLower() == "medical")
                        {
                            double originalNumber = inputSalary;
                            double result = originalNumber *10 / 100;


                            int roundedResult = (int)Math.Round(result, 0, MidpointRounding.AwayFromZero);
                            vm.Medical = roundedResult;

                            if (Convert.ToDecimal(vm.Medical) <= 10000.00M)
                            {
                                cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.Medical);
                            }
                            else
                            {
                                vm.Medical = 10000;
                                cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.Medical);
                            }
                        }
                        else if (dataRow["SalaryType"].ToString().ToLower() == "conveyance")
                        {
                            double originalNumber = inputSalary;
                            double result = originalNumber * 5 / 100;

                            int roundedResult = (int)Math.Round(result, 0, MidpointRounding.AwayFromZero);

                            vm.TA = roundedResult;  

                            if (Convert.ToDecimal(vm.TA) <= 2500.00M)
                            {
                                cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.TA);
                            }
                            else
                            {
                                vm.TA = 2500;
                                cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.TA);
                            }
                        }
                        else if (dataRow["SalaryType"].ToString().ToLower() == "houserent")
                        {
                            double originalNumber = inputSalary;
                            double result = originalNumber * 25 / 100;

                            int roundedResult = (int)Math.Round(result, 0, MidpointRounding.AwayFromZero);


                            vm.Housing = roundedResult;

                            if (Convert.ToDecimal(vm.Housing) <= 25000.00M)
                            {
                                cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.Housing);
                            }
                            else
                            {
                                vm.Housing = 25000;
                                cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.Housing);
                            }
                        }                      
                    }

                    if (dataRow["SL"].ToString() == "2")
                    {
                        if (dataRow["SalaryType"].ToString().ToLower() == "gross")
                        {
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.basic);
                        }
                    }
                    else if (dataRow["SL"].ToString() == "3")
                    {
                        if (dataRow["SalaryType"].ToString().ToLower() == "basic")
                        {
                            vm.basic =Convert.ToInt32( inputSalary) -( vm.Housing +vm.TA +vm.Medical);
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.basic);
                        }
                    }
                   
                    string SType = dataRow["SalaryTypeId"].ToString();
                    string IsEarning = dataRow["IsEarning"].ToString();
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@ID", detailId);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@SalaryTypeId", dataRow["SalaryTypeId"].ToString());
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@SalaryType", dataRow["SalaryType"].ToString());
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@EmployeeId", EmployeeId);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsFixed", dataRow["IsFixed"].ToString()); // need update
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@Portion", dataRow["Portion"].ToString()); // need update
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@PortionSalaryType", dataRow["PortionSalaryType"].ToString());

                    cmdDetail.Parameters.AddWithValueAndParamCheck("@EmployeeSalaryStructureId", id);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@Remarks", "-");
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsActive", 1);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsArchive", 0);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@CreatedBy", siVM.CreatedBy);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@CreatedBy", siVM.CreatedBy);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@CreatedAt", siVM.CreatedAt);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@CreatedFrom", siVM.CreatedFrom);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@LastUpdateBy", siVM.LastUpdateBy);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@LastUpdateAt", siVM.LastUpdateAt);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@LastUpdateFrom", siVM.LastUpdateFrom); ;
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsEarning", dataRow["IsEarning"].ToString()); // need update
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IncrementDate", 0); // need update
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsCurrent", 1);


                    cmdDetail.ExecuteNonQuery();
                }


                sqlText = @"

update EmployeeSalaryStructureDetail set IncrementDate = EmployeeJob.JoinDate
from EmployeeJob 
where EmployeeJob.EmployeeId = EmployeeSalaryStructureDetail.EmployeeId

update EmployeeJob set BasicSalary = EmployeeSalaryStructureDetail.Amount
from EmployeeSalaryStructureDetail
where EmployeeSalaryStructureDetail.EmployeeId = EmployeeJob.EmployeeId
and EmployeeSalaryStructureDetail.SalaryType = 'Basic'

update EmployeeJob set GrossSalary = EmployeeSalaryStructureDetail.Amount
from EmployeeSalaryStructureDetail
where EmployeeSalaryStructureDetail.EmployeeId = EmployeeJob.EmployeeId
and EmployeeSalaryStructureDetail.SalaryType = 'Gross'

";

                cmd.CommandText = sqlText;

                cmd.ExecuteNonQuery();






                //transResult = Convert.ToInt32(exeRes);
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
                FileLogger.Log("Fail" + Environment.NewLine + ex.Message + Environment.NewLine + "EmployeeSalaryStructureFromBasic", this.GetType().Name, ex.Message + Environment.NewLine + sqlText.ToString() + Environment.NewLine + ex.StackTrace);

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

        public string[] EmployeeSalaryStructureBollore(string EmployeeId, string SalaryStructureId, decimal salaryInput1, string gradeId, string stepId, bool isGross, decimal BankPayAmount, ShampanIdentityVM siVM, EmployeeStructureGroupVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            DataSet dsOld1 = new DataSet();
            DataSet dsOld2 = new DataSet();
            DataSet dsOld3 = new DataSet();
            DataSet dsOld4 = new DataSet();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            //string stepName = "";
            //decimal GrossSalary = 0;
            //decimal SalaryAmount = 0;
            //decimal MBasic = 0;
            //decimal MMedical = 0;
            //decimal MConveyance = 0;
            //decimal Portion = 0;
            //int EmployeeSalaryStructureNewId = 0;
            //string EmployeeJoiningDate = "";
            //string EmployeeJobId = "0";
            //bool IsFixed = false;
            decimal GrossSalaryInput = 0;
            //decimal BasicSalary = 0;
            //decimal MedicalSalary = 0;
            //decimal HouseRentSalary = 0;
            //decimal ConvenceSalary = 0;

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
                SettingDAL _setDAL = new SettingDAL();
                //var SalaryFromMatrix = Convert.ToBoolean(_setDAL.settingValue("Salary", "SalaryFromMatrix", currConn, transaction).ToLower() == "y" ? true : false);

                vm.basic = salaryInput1;
                sqlText = "select count(Id) from EmployeeSalaryStructure where EmployeeId=@EmployeeId  ";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                int count = Convert.ToInt32(cmd.ExecuteScalar());


                if (count != 0)
                {
                    string deleteText = @"

delete  EmployeeSalaryStructureDetail
where EmployeeID= @EmployeeID

delete from EmployeeSalaryStructure
where EmployeeID= @EmployeeID

update EmployeeStructureGroup set gradeId=@gradeId, StepId=@StepId, SalaryStructureId=@SalaryStructureId,IsGross=@IsGross where EmployeeId=@EmployeeID
 update EmployeePromotion set  gradeId=@gradeId, StepId=@StepId where EmployeeId=@EmployeeID and IsCurrent=1


";

                    cmd.CommandText = deleteText;

                    cmd.Parameters.AddWithValueAndParamCheck("@EmployeeID", EmployeeId);
                    cmd.Parameters.AddWithValueAndParamCheck("@gradeId", gradeId);
                    cmd.Parameters.AddWithValueAndParamCheck("@StepId", stepId);
                    cmd.Parameters.AddWithValueAndParamCheck("@SalaryStructureId", SalaryStructureId);
                    cmd.Parameters.AddWithValueAndParamCheck("@IsGross", isGross);
                    cmd.ExecuteNonQuery();
                }

                sqlText = "select isnull(max(cast(ID as int)),0)+1 from EmployeeSalaryStructure";
                cmd.CommandText = sqlText;
                string id = cmd.ExecuteScalar().ToString();


                sqlText = @"
                    insert into EmployeeSalaryStructure 
                    (
                        ID
                        ,EmployeeId
                        ,SalaryStructureId
                        ,TotalValue
                        ,IncrementDate
                        ,BranchId
                        ,Remarks
                        ,IsActive
                        ,IsArchive
                        ,CreatedBy
                        ,CreatedAt
                        ,CreatedFrom
                        ,LastUpdateBy
                        ,LastUpdateAt
                        ,LastUpdateFrom
                        ,IsCurrent
                    )
                    values (
                         @ID
                        ,@EmployeeId
                        ,@SalaryStructureId
                        ,@TotalValue
                        ,@IncrementDate
                        ,@BranchId
                        ,@Remarks
                        ,@IsActive
                        ,@IsArchive
                        ,@CreatedBy
                        ,@CreatedAt
                        ,@CreatedFrom
                        ,@LastUpdateBy
                        ,@LastUpdateAt
                        ,@LastUpdateFrom
                        ,@IsCurrent
                    )";

                cmd.CommandText = sqlText;

                cmd.Parameters.AddWithValueAndParamCheck("@ID", id);
                cmd.Parameters.AddWithValueAndParamCheck("@EmployeeId", EmployeeId);
                cmd.Parameters.AddWithValueAndParamCheck("@SalaryStructureId", SalaryStructureId);
                cmd.Parameters.AddWithValueAndParamCheck("@TotalValue", vm.basic + vm.Medical + vm.TA + vm.Housing); // need to update
                cmd.Parameters.AddWithValueAndParamCheck("@IncrementDate", 0);// need to update
                cmd.Parameters.AddWithValueAndParamCheck("@BranchId", siVM.BranchId);
                cmd.Parameters.AddWithValueAndParamCheck("@Remarks", "-");
                cmd.Parameters.AddWithValueAndParamCheck("@IsActive", 1);
                cmd.Parameters.AddWithValueAndParamCheck("@IsArchive", 0);
                cmd.Parameters.AddWithValueAndParamCheck("@CreatedBy", siVM.CreatedBy);
                cmd.Parameters.AddWithValueAndParamCheck("@CreatedAt", siVM.CreatedAt);
                cmd.Parameters.AddWithValueAndParamCheck("@CreatedFrom", siVM.CreatedFrom);
                cmd.Parameters.AddWithValueAndParamCheck("@LastUpdateBy", siVM.LastUpdateBy);
                cmd.Parameters.AddWithValueAndParamCheck("@LastUpdateAt", siVM.LastUpdateAt);
                cmd.Parameters.AddWithValueAndParamCheck("@LastUpdateFrom", siVM.LastUpdateFrom);
                cmd.Parameters.AddWithValueAndParamCheck("@IsCurrent", true);


                cmd.ExecuteNonQuery();

                sqlText = @"update EmployeeSalaryStructure set IncrementDate = EmployeeJob.JoinDate
                                from EmployeeJob 
                                where EmployeeJob.EmployeeId = EmployeeSalaryStructure.EmployeeId";

                cmd.CommandText = sqlText;
                cmd.ExecuteNonQuery();



                sqlText = @"
select CASE
    WHEN SalaryType='Basic' THEN '2'
    WHEN SalaryType='Gross' THEN '1'

    ELSE '3'
END SL,SalaryTypeId,SalaryType,PortionSalaryType,Portion,IsFixed,IsGross,IsEarning from(
select 1 SL,SalaryTypeId,SalaryType, Portion,isnull( PortionSalaryType,'Basic')PortionSalaryType, IsFixed,IsGross, IsEarning from SalaryStructureDetail 
where  SalaryStructureId='1_1'
and SalaryType not in ('other')



 union all
 select 4 SL,SalaryTypeId,SalaryType, Portion,isnull( PortionSalaryType,'Basic')PortionSalaryType, IsFixed,IsGross, IsEarning from SalaryStructureDetail 
where  SalaryStructureId='1_1'
and SalaryType in ('other')
and IsFixed=0 
union 
Select 1 SL,ID,Name,0,'Gross',1,1,1 from EnumSalaryType where TypeName = 'GROSS'
) St
order by sl
";

                SqlCommand cmdDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDetail.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmdDetail);
                DataTable dtSalaryStructures = new DataTable();

                dataAdapter.Fill(dtSalaryStructures);




                if (dtSalaryStructures.Rows.Count == 0)
                {
                    throw new Exception("No Data Found in Salary Structure");
                }
                #region insert into EmployeeSalaryStructureDetail

                string detailInsertText = @"
insert into EmployeeSalaryStructureDetail
(
ID
,SalaryTypeId
,SalaryType
,EmployeeId
,IsFixed
,Portion
,PortionSalaryType
,Amount
,EmployeeSalaryStructureId,
Remarks 
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,IsEarning
,IncrementDate
,IsCurrent
)

values (

 @ID
,@SalaryTypeId
,@SalaryType
,@EmployeeId
,@IsFixed
,@Portion
,@PortionSalaryType
,@Amount
,@EmployeeSalaryStructureId
,@Remarks 
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@LastUpdateBy
,@LastUpdateAt
,@LastUpdateFrom
,@IsEarning
,@IncrementDate
,@IsCurrent
)";
                #endregion insert into EmployeeSalaryStructureDetail

                cmdDetail.CommandText = detailInsertText;

                string detailId = "";
                foreach (DataRow dataRow in dtSalaryStructures.Rows)
                {
                    //cmdDetail.Parameters.AddWithValue()


                    sqlText = "select isnull(max(cast(ID as int)),0)+1 from EmployeeSalaryStructureDetail";
                    cmd.CommandText = sqlText;
                    detailId = cmd.ExecuteScalar().ToString();

                    if (dataRow["SL"].ToString() == "1")
                    {
                        if (dataRow["SalaryType"].ToString().ToLower() == "gross")
                        {
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.basic);
                        }
                    }
                    if (dataRow["SL"].ToString() == "2")
                    {
                        decimal basic = Convert.ToDecimal(salaryInput1) * (vm.BasicPercentage / 100.00M);
                        vm.basic = basic;

                        if (dataRow["SalaryType"].ToString().ToLower() == "basic")
                        {
                           
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.basic);
                        }
                    }
                    else if (dataRow["SL"].ToString() == "3")
                    {
                      
                        if (dataRow["SalaryType"].ToString().ToLower() == "houserent")
                        {
                            decimal Housing = Convert.ToDecimal(vm.basic) * (50 / 100.00M);
                            vm.Housing = Housing;
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.Housing);
                        }
                        else if (dataRow["SalaryType"].ToString().ToLower() == "conveyance")
                        {

                            vm.TA = salaryInput1 - vm.basic - vm.Housing;

                              cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.TA);
                            
                        }
                        else if (dataRow["SalaryType"].ToString().ToLower() == "medical")
                        {
                            vm.Medical = 0;
                            cmdDetail.Parameters.AddWithValueAndParamCheck("@Amount", vm.Medical);
                        }
                    }



                    string SType = dataRow["SalaryTypeId"].ToString();
                    string IsEarning = dataRow["IsEarning"].ToString();
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@ID", detailId);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@SalaryTypeId", dataRow["SalaryTypeId"].ToString());
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@SalaryType", dataRow["SalaryType"].ToString());
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@EmployeeId", EmployeeId);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsFixed", dataRow["IsFixed"].ToString()); // need update
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@Portion", dataRow["Portion"].ToString()); // need update
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@PortionSalaryType", dataRow["PortionSalaryType"].ToString());

                    cmdDetail.Parameters.AddWithValueAndParamCheck("@EmployeeSalaryStructureId", id);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@Remarks", "-");
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsActive", 1);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsArchive", 0);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@CreatedBy", siVM.CreatedBy);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@CreatedBy", siVM.CreatedBy);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@CreatedAt", siVM.CreatedAt);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@CreatedFrom", siVM.CreatedFrom);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@LastUpdateBy", siVM.LastUpdateBy);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@LastUpdateAt", siVM.LastUpdateAt);
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@LastUpdateFrom", siVM.LastUpdateFrom); ;
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsEarning", dataRow["IsEarning"].ToString()); // need update
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IncrementDate", 0); // need update
                    cmdDetail.Parameters.AddWithValueAndParamCheck("@IsCurrent", 1);


                    cmdDetail.ExecuteNonQuery();
                }


                sqlText = @"

update EmployeeSalaryStructureDetail set IncrementDate = EmployeeJob.JoinDate
from EmployeeJob 
where EmployeeJob.EmployeeId = EmployeeSalaryStructureDetail.EmployeeId

update EmployeeJob set BasicSalary = EmployeeSalaryStructureDetail.Amount
from EmployeeSalaryStructureDetail
where EmployeeSalaryStructureDetail.EmployeeId = EmployeeJob.EmployeeId
and EmployeeSalaryStructureDetail.SalaryType = 'Basic'

update EmployeeJob set GrossSalary = EmployeeSalaryStructureDetail.Amount
from EmployeeSalaryStructureDetail
where EmployeeSalaryStructureDetail.EmployeeId = EmployeeJob.EmployeeId
and EmployeeSalaryStructureDetail.SalaryType = 'Gross'

";

                cmd.CommandText = sqlText;

                cmd.ExecuteNonQuery();






                //transResult = Convert.ToInt32(exeRes);
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
                FileLogger.Log("Fail" + Environment.NewLine + ex.Message + Environment.NewLine + "EmployeeSalaryStructureFromBasic", this.GetType().Name, ex.Message + Environment.NewLine + sqlText.ToString() + Environment.NewLine + ex.StackTrace);

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

        public string[] EmployeeLeaveStructure(string EmployeeId, string LeaveStructureId, string LeaveYear, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                //LeaveYear = Convert.ToDateTime(LeaveYear).ToString("yyyy");
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
                #region SG Update
                sqlText = "Select count(Id) from EmployeeStructureGroup where EmployeeId=@EmployeeId";
                SqlCommand cmdExists = new SqlCommand(sqlText, currConn, transaction);
                cmdExists.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                var exeRes = cmdExists.ExecuteScalar();
                int exists = Convert.ToInt32(exeRes);
                if (exists <= 0)
                {
                    retResults[1] = "Please Save/Update Employee Job Information";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
                }
                sqlText = "  ";
                sqlText += @" update  EmployeeStructureGroup set 
                            LeaveStructureId=@LeaveStructureId
                            ,CreatedBy=@CreatedBy
                            ,CreatedAt=@CreatedAt
                            ,CreatedFrom=@CreatedFrom
                            where EmployeeId=@EmployeeId   ";
                SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdsg.Parameters.AddWithValue("@LeaveStructureId", LeaveStructureId);
                cmdsg.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                cmdsg.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                cmdsg.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                cmdsg.Transaction = transaction;
                cmdsg.ExecuteNonQuery();
                #endregion SG Update
                #region Save
                sqlText = @"Select count(Id) from EmployeeLeaveStructure 
                                where EmployeeId=@EmployeeId and LeaveYear=@LeaveYear
                            ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn, transaction);
                cmdExist.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdExist.Parameters.AddWithValue("@LeaveYear", LeaveYear);
                exeRes = cmdExist.ExecuteScalar();
                int alreadyExist = Convert.ToInt32(exeRes);
                sqlText = "Select Isnull(max(Id),0)Id from EmployeeLeaveStructure ";
                SqlCommand cmdId = new SqlCommand(sqlText, currConn, transaction);
                exeRes = cmdId.ExecuteScalar();
                int ELStructureId = Convert.ToInt32(exeRes);
                if (alreadyExist > 0)
                {
                    #region Delete EmployeeLeaveStructure First
                    #region EmployeeLeave Check
//                    sqlText = @"Select count(Id) from EmployeeLeave 
//                                where EmployeeId=@EmployeeId and LeaveYear=@LeaveYear and IsArchive = 0
//                            ";
//                    SqlCommand cmdExistEmployeeLeave = new SqlCommand(sqlText, currConn, transaction);
//                    cmdExistEmployeeLeave.Parameters.AddWithValue("@EmployeeId", EmployeeId);
//                    cmdExistEmployeeLeave.Parameters.AddWithValue("@LeaveYear", LeaveYear);
//                    exeRes = cmdExistEmployeeLeave.ExecuteScalar();
//                    int alreadyExistEmployeeLeave = Convert.ToInt32(exeRes);

//                    if (alreadyExistEmployeeLeave > 0)
//                    {
//                        retResults[1] = "Please Delete Leave for this Employee!";
//                        retResults[3] = sqlText;
//                        throw new ArgumentNullException(retResults[1], "");
//                    }
                    #endregion EmployeeLeave Check
                    #region Delete
                    sqlText = "";
                    sqlText = "delete EmployeeLeaveStructure  ";
                    sqlText += "  where EmployeeId=@EmployeeId and LeaveYear=@LeaveYear";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@LeaveYear", LeaveYear);
                    exeRes = cmdUpdate.ExecuteNonQuery();

                    #endregion Delete

                    #endregion Delete EmployeeLeaveStructure First


                }
                #region sql statement
                sqlText = @"SELECT *
                                from LeaveStructureDetail
                                where  LeaveStructureId=@LeaveStructureId ";
                SqlCommand objCommLSD = new SqlCommand();
                objCommLSD.Connection = currConn;
                objCommLSD.CommandText = sqlText;
                objCommLSD.CommandType = CommandType.Text;
                objCommLSD.Parameters.AddWithValue("@LeaveStructureId", LeaveStructureId);
                objCommLSD.Transaction = transaction;
                SqlDataAdapter da1 = new SqlDataAdapter(objCommLSD);
                DataTable dtlsd = new DataTable();
                da1.Fill(dtlsd);
                //SqlDataReader dr;
                //dr = objCommLSD.ExecuteReader();
                foreach (DataRow dr in dtlsd.Rows)
                {
                    #region Save
                    sqlText = "  ";
                    sqlText += @"   INSERT INTO EmployeeLeaveStructure(
                                            Id,EmployeeId,LeaveStructureId,LeaveYear,LeaveType_E,LeaveDays,OpeningLeaveDays,IsEarned,IsCompensation,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,IsCarryForward,MaxBalance         
                                            ) VALUES (
                                            @Id,@EmployeeId,@LeaveStructureId,@LeaveYear,@LeaveType_E,@LeaveDays,@OpeningLeaveDays,@IsEarned,@IsCompensation,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@IsCarryForward,@MaxBalance
                                            )  ";
                    SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                    cmdIns.Parameters.AddWithValue("@Id", (ELStructureId + 1));
                    cmdIns.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdIns.Parameters.AddWithValue("@LeaveStructureId", LeaveStructureId);
                    cmdIns.Parameters.AddWithValue("@LeaveYear", LeaveYear);
                    cmdIns.Parameters.AddWithValue("@LeaveType_E", dr["LeaveType_E"].ToString());
                    cmdIns.Parameters.AddWithValue("@LeaveDays", dr["LeaveDays"].ToString());
                    cmdIns.Parameters.AddWithValue("@OpeningLeaveDays", "0");
                    cmdIns.Parameters.AddWithValue("@IsEarned", dr["IsEarned"].ToString());
                    cmdIns.Parameters.AddWithValue("@IsCompensation", dr["IsCompensation"].ToString());
                    cmdIns.Parameters.AddWithValue("@Remarks", dr["Remarks"].ToString() ?? Convert.DBNull);
                    cmdIns.Parameters.AddWithValue("@IsActive", true);
                    cmdIns.Parameters.AddWithValue("@IsArchive", false);
                    cmdIns.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdIns.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdIns.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdIns.Parameters.AddWithValue("@IsCarryForward", dr["IsCarryForward"].ToString());
                    cmdIns.Parameters.AddWithValue("@MaxBalance", dr["MaxBalance"].ToString());
                    cmdIns.Transaction = transaction;
                    cmdIns.ExecuteNonQuery();
                    ELStructureId++;
                    #endregion Save
                }
                #endregion
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
        public string[] EmployeePFStructure(string EmployeeId, string PFStructureId, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                #region SG Update
                sqlText = "Select count(Id) from EmployeeStructureGroup where EmployeeId=@EmployeeId";
                SqlCommand cmdExists = new SqlCommand(sqlText, currConn);
                cmdExists.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdExists.Transaction = transaction;
                var exeRes = cmdExists.ExecuteScalar();
                int Exists = Convert.ToInt32(exeRes);
                if (Exists <= 0)
                {
                    retResults[1] = "Please Save/Update Employee Job Information";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
                }
                sqlText = "  ";
                sqlText += @" update  EmployeeStructureGroup set 
                            PFStructureId=@PFStructureId
                            ,CreatedBy=@CreatedBy
                            ,CreatedAt=@CreatedAt
                            ,CreatedFrom=@CreatedFrom
                            where EmployeeId=@EmployeeId   ";
                SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdsg.Parameters.AddWithValue("@PFStructureId", PFStructureId);
                cmdsg.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                cmdsg.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                cmdsg.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                cmdsg.Transaction = transaction;
                cmdsg.ExecuteNonQuery();
                #endregion SG Update
                #region Save
                sqlText = @"Select count(Id) from EmployeePF 
                                where 
                                EmployeeId=@EmployeeId 
                            ";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmd2.Transaction = transaction;
                exeRes = cmd2.ExecuteScalar();
                int alreadyExist = Convert.ToInt32(exeRes);
                sqlText = @"SELECT *
                                from PFStructure
                                where  id=@PFStructureId and BranchId=@BranchId ";
                SqlCommand obj1 = new SqlCommand();
                obj1.Connection = currConn;
                obj1.CommandText = sqlText;
                obj1.CommandType = CommandType.Text;
                obj1.Parameters.AddWithValue("@PFStructureId", PFStructureId);
                obj1.Parameters.AddWithValue("@BranchId", siVM.BranchId);
                obj1.Transaction = transaction;
                SqlDataAdapter da1 = new SqlDataAdapter(obj1);
                da1.Fill(dt1);
                if (alreadyExist > 0)
                {
                    ////Update
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EmployeePF set";
                    sqlText += " PFStructureId=@PFStructureId,";
                    sqlText += " PFValue=@PFValue,";
                    sqlText += " IsFixed=@IsFixed,";
                    sqlText += " PortionSalaryType=@PortionSalaryType,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where EmployeeId=@EmployeeId";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@PFStructureId", PFStructureId);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@PortionSalaryType", dt1.Rows[0]["PortionSalaryType"].ToString() ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PFValue", dt1.Rows[0]["PFValue"].ToString());
                    cmdUpdate.Parameters.AddWithValue("@IsFixed", dt1.Rows[0]["IsFixed"].ToString());
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.ExecuteNonQuery();
                    #endregion Update Settings
                }
                else  //Insert
                {
                    sqlText = "Select ISNULL(max(cast(Id as int)),0) Id from EmployeePF";
                    SqlCommand cmdId = new SqlCommand(sqlText, currConn);
                    cmdId.Transaction = transaction;
                    exeRes = cmdId.ExecuteScalar();
                    int NewId = Convert.ToInt32(exeRes);
                    #region Save
                    sqlText = "  ";
                    sqlText += @"   INSERT INTO EmployeePF(
                                        Id,PFStructureId,EmployeeId,PFValue,IsFixed,Remarks,PortionSalaryType,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                        ) VALUES (
                                        @Id,@PFStructureId,@EmployeeId,@PFValue,@IsFixed,@Remarks,@PortionSalaryType,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                        )  ";
                    SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                    cmdIns.Parameters.AddWithValue("@Id", (NewId + 1));
                    cmdIns.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdIns.Parameters.AddWithValue("@PFStructureId", PFStructureId);
                    cmdIns.Parameters.AddWithValue("@PortionSalaryType", dt1.Rows[0]["PortionSalaryType"].ToString() ?? Convert.DBNull);
                    cmdIns.Parameters.AddWithValue("@Remarks", dt1.Rows[0]["Remarks"].ToString());
                    cmdIns.Parameters.AddWithValue("@PFValue", dt1.Rows[0]["PFValue"].ToString());
                    cmdIns.Parameters.AddWithValue("@IsFixed", dt1.Rows[0]["IsFixed"].ToString());
                    cmdIns.Parameters.AddWithValue("@IsActive", true);
                    cmdIns.Parameters.AddWithValue("@IsArchive", false);
                    cmdIns.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdIns.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdIns.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdIns.Transaction = transaction;
                    cmdIns.ExecuteNonQuery();
                    NewId++;
                    #endregion Save
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
        public string[] EmployeeOtherStructure(EmployeeStructureGroupVM vm, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                retResults = EmployeePFStructure(vm.EmployeeId, vm.PFStructureId, siVM, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    retResults[1] = "PF Structure Can't Updated, Please Contact with Administrator";
                    throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
                }
                retResults = EmployeeTaxStructure(vm.EmployeeId, vm.TaxStructureId, siVM, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    retResults[1] = "TAX Structure Can't Updated, Please Contact with Administrator";
                    throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
                }
                //retResults = BonusStructure(vm.EmployeeId, vm.BonusStructureId, siVM, currConn, transaction);
                //if (retResults[0] == "Fail")
                //{
                //    retResults[1] = "Bonus Structure Can't Updated, Please Contact with Administrator";
                //    throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
                //}
                retResults = ProjectAllocation(vm.EmployeeId, vm.ProjectAllocationId, siVM, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    retResults[1] = "Project Allocation Can't Updated, Please Contact with Administrator";
                    throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
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
        public string[] EmployeeTaxStructure(string EmployeeId, string TaxStructureId, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                #region SG Update
                sqlText = "Select count(Id) from EmployeeStructureGroup where EmployeeId=@EmployeeId";
                SqlCommand cmdExists = new SqlCommand(sqlText, currConn);
                cmdExists.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdExists.Transaction = transaction;
                var exeRes = cmdExists.ExecuteScalar();
                int Exists = Convert.ToInt32(exeRes);
                if (Exists <= 0)
                {
                    retResults[1] = "Please Save/Update Employee Job Information";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
                }
                sqlText = "  ";
                sqlText += @" update  EmployeeStructureGroup set 
                            TaxStructureId=@TaxStructureId
                            ,CreatedBy=@CreatedBy
                            ,CreatedAt=@CreatedAt
                            ,CreatedFrom=@CreatedFrom
                            where EmployeeId=@EmployeeId   ";
                SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdsg.Parameters.AddWithValue("@TaxStructureId", TaxStructureId);
                cmdsg.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                cmdsg.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                cmdsg.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                cmdsg.Transaction = transaction;
                cmdsg.ExecuteNonQuery();
                #endregion SG Update
                #region Save
                sqlText = @"Select count(Id) from EmployeeTax 
                                where 
                                EmployeeId=@EmployeeId 
                            ";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmd2.Transaction = transaction;
                exeRes = cmd2.ExecuteScalar();
                int alreadyExist = Convert.ToInt32(exeRes);
                sqlText = @"SELECT *  from TaxStructure
                                where  id=@TaxStructureId and BranchId=@BranchId ";
                SqlCommand obj1 = new SqlCommand();
                obj1.Connection = currConn;
                obj1.CommandText = sqlText;
                obj1.CommandType = CommandType.Text;
                obj1.Parameters.AddWithValue("@TaxStructureId", TaxStructureId);
                obj1.Parameters.AddWithValue("@BranchId", siVM.BranchId);
                obj1.Transaction = transaction;
                SqlDataAdapter da1 = new SqlDataAdapter(obj1);
                da1.Fill(dt1);
                if (dt1 == null || dt1.Rows.Count <= 0)
                {
                    retResults[1] = "Tax Structure Not Setup";
                    throw new ArgumentNullException("Tax Structure Not Setup", "");
                }
                if (alreadyExist > 0)
                {
                    ////Update
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EmployeeTax set";
                    sqlText += " TaxValue=@TaxValue,";
                    sqlText += " PortionSalaryType=@PortionSalaryType,";
                    sqlText += " TaxStructureId=@TaxStructureId,";
                    sqlText += " IsFixed=@IsFixed,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where EmployeeId=@EmployeeId";
                    //sqlText += " EmployeeId=@EmployeeId";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@TaxStructureId", TaxStructureId);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@PortionSalaryType", dt1.Rows[0]["PortionSalaryType"].ToString() ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TaxValue", dt1.Rows[0]["TaxValue"].ToString());
                    cmdUpdate.Parameters.AddWithValue("@IsFixed", dt1.Rows[0]["IsFixed"].ToString());
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.ExecuteNonQuery();
                    #endregion Update Settings
                }
                else  //Insert
                {
                    sqlText = "Select ISNULL(max(cast(Id as int)),0) Id from EmployeeTax ";
                    SqlCommand cmdId = new SqlCommand(sqlText, currConn);
                    cmdId.Transaction = transaction;
                    exeRes = cmdId.ExecuteScalar();
                    int NewId = Convert.ToInt32(exeRes);
                    #region Save
                    sqlText = "  ";
                    sqlText += @"   INSERT INTO EmployeeTax(
                                        Id,TaxStructureId,EmployeeId,TaxValue,IsFixed,Remarks,PortionSalaryType,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                        ) VALUES (
                                        @Id,@TaxStructureId,@EmployeeId,@TaxValue,@IsFixed,@Remarks,@PortionSalaryType,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                        )  ";
                    SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                    cmdIns.Parameters.AddWithValue("@Id", (NewId + 1));
                    cmdIns.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdIns.Parameters.AddWithValue("@TaxStructureId", TaxStructureId);
                    cmdIns.Parameters.AddWithValue("@Remarks", dt1.Rows[0]["Remarks"].ToString() ?? Convert.DBNull);
                    cmdIns.Parameters.AddWithValue("@PortionSalaryType", dt1.Rows[0]["PortionSalaryType"].ToString() ?? Convert.DBNull);
                    cmdIns.Parameters.AddWithValue("@TaxValue", dt1.Rows[0]["TaxValue"].ToString());
                    cmdIns.Parameters.AddWithValue("@IsFixed", dt1.Rows[0]["IsFixed"].ToString());
                    cmdIns.Parameters.AddWithValue("@IsActive", true);
                    cmdIns.Parameters.AddWithValue("@IsArchive", false);
                    cmdIns.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdIns.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdIns.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdIns.Transaction = transaction;
                    cmdIns.ExecuteNonQuery();
                    NewId++;
                    #endregion Save
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
        public string[] EmployeeGroup(string EmployeeId, string EmployeeGroupId, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                #region SG Update
                sqlText = "Select count(Id) from EmployeeStructureGroup where EmployeeId=@EmployeeId";
                SqlCommand cmdExists = new SqlCommand(sqlText, currConn);
                cmdExists.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdExists.Transaction = transaction;
                var exeRes = cmdExists.ExecuteScalar();
                int Exists = Convert.ToInt32(exeRes);
                if (Exists <= 0)
                {
                    retResults[1] = "Please Save/Update Employee Job Information";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
                }
                sqlText = "  ";
                sqlText += @" update  EmployeeStructureGroup set 
                            EmployeeGroupId=@EmployeeGroupId
                            ,CreatedBy=@CreatedBy
                            ,CreatedAt=@CreatedAt
                            ,CreatedFrom=@CreatedFrom
                            where EmployeeId=@EmployeeId   ";
                SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdsg.Parameters.AddWithValue("@EmployeeGroupId", EmployeeGroupId);
                cmdsg.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                cmdsg.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                cmdsg.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                cmdsg.Transaction = transaction;
                cmdsg.ExecuteNonQuery();
                #endregion SG Update
                #region Save
                sqlText = @"Select count(Id) from EmployeeGroup 
                                where 
                                EmployeeId=@EmployeeId 
                            ";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmd2.Transaction = transaction;
                exeRes = cmd2.ExecuteScalar();
                int alreadyExist = Convert.ToInt32(exeRes);
                sqlText = "Select count(Id) from EmployeeGroup ";
                SqlCommand cmdId = new SqlCommand(sqlText, currConn);
                cmdId.Transaction = transaction;
                exeRes = cmdId.ExecuteScalar();
                CommonDAL _dal = new CommonDAL();
                int NewId = _dal.NextId("EmployeeGroup", currConn, transaction);
                if (alreadyExist > 0)
                {
                    ////Update
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EmployeeGroup set";
                    sqlText += " GroupId=@GroupId,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where EmployeeId=@EmployeeId";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@GroupId", EmployeeGroupId);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.ExecuteNonQuery();
                    #endregion Update Settings
                }
                else  //Insert
                {
                    #region Save
                    sqlText = "  ";
                    sqlText += @"   INSERT INTO EmployeeGroup(
                                    Id,GroupId,EmployeeId,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                    ) VALUES (
                                    @Id,@GroupId,@EmployeeId,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom)  ";
                    SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                    cmdIns.Parameters.AddWithValue("@Id", (NewId + 1));
                    cmdIns.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmdIns.Parameters.AddWithValue("@GroupId", EmployeeGroupId);
                    cmdIns.Parameters.AddWithValue("@Remarks", Convert.DBNull);
                    cmdIns.Parameters.AddWithValue("@IsActive", true);
                    cmdIns.Parameters.AddWithValue("@IsArchive", false);
                    cmdIns.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdIns.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdIns.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdIns.Transaction = transaction;
                    cmdIns.ExecuteNonQuery();
                    NewId++;
                    #endregion Save
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
        public string[] AttendanceRoster(AttendanceRosterVM vm, ShampanIdentityVM siVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            DataTable dt1 = new DataTable();
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                sqlText = @"Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from AttendanceRoster 
                                where Id=@Id 
                            ";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@Id", vm.Id);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                int alreadyExist = Convert.ToInt32(exeRes);
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from AttendanceRoster ";
                SqlCommand cmdId = new SqlCommand(sqlText, currConn);
                cmdId.Transaction = transaction;
                exeRes = cmdId.ExecuteScalar();
                int RosterNewId = Convert.ToInt32(exeRes);
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from AttendanceRosterDetail ";
                SqlCommand cmdId1 = new SqlCommand(sqlText, currConn);
                cmdId1.Transaction = transaction;
                exeRes = cmdId1.ExecuteScalar();
                int RosterDetailNewId = Convert.ToInt32(exeRes);
                if (alreadyExist > 0)
                {
                    sqlText = @"delete from AttendanceRosterDetail
                   where AttendanceRosterId=@AttendanceRosterId ";
                    SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                    cmdDelete.Parameters.AddWithValue("@AttendanceRosterId", vm.Id);
                    cmdDelete.Transaction = transaction;
                    cmdDelete.ExecuteNonQuery();
                    ////Update
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update AttendanceRoster set";
                    sqlText += " Code=@Code,";
                    sqlText += " Name=@Name,";
                    sqlText += " StartDate=@StartDate,";
                    sqlText += " EndDate=@EndDate,";
                    sqlText += " BranchId=@BranchId,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code);
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name);
                    cmdUpdate.Parameters.AddWithValue("@StartDate", vm.StartDate);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                    cmdUpdate.Parameters.AddWithValue("@EndDate", vm.EndDate);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", siVM.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", siVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", siVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", siVM.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.ExecuteNonQuery();
                    #endregion Update Settings
                    #region Save Detail
                    foreach (var item in vm.attendanceRosterDetails)
                    {
                        string ToDate = Ordinary.DateToString(item.ToDate.ToString());
                        sqlText = "  ";
                        sqlText += @"   INSERT INTO AttendanceRosterDetail(
                                        Id,AttendanceRosterId,StartDate,EndDate,ToDate,AttendanceStructureId,AttendanceGroupId,Remarks
                                        ,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                        ) VALUES (                     
                                        @Id,@AttendanceRosterId,@StartDate,@EndDate,@ToDate,@AttendanceStructureId,@AttendanceGroupId,@Remarks
                                        ,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom   
                                        )  ";
                        SqlCommand cmdInsD = new SqlCommand(sqlText, currConn);
                        cmdInsD.Parameters.AddWithValue("@Id", (RosterDetailNewId + 1));
                        cmdInsD.Parameters.AddWithValue("@AttendanceRosterId", vm.Id);
                        cmdInsD.Parameters.AddWithValue("@StartDate", vm.StartDate);
                        cmdInsD.Parameters.AddWithValue("@EndDate", vm.EndDate);
                        cmdInsD.Parameters.AddWithValue("@ToDate", ToDate);
                        //cmdInsD.Parameters.AddWithValue("@AttendanceStructureId", item.AttendanceStructureId);
                        cmdInsD.Parameters.AddWithValue("@AttendanceGroupId", item.AttendanceGroupId);
                        cmdInsD.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                        cmdInsD.Parameters.AddWithValue("@IsActive", true);
                        cmdInsD.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsD.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsD.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsD.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsD.Transaction = transaction;
                        cmdInsD.ExecuteNonQuery();
                        RosterDetailNewId++;
                    }
                    #endregion Save Detail
                }
                else  //Insert
                {
                    #region Save
                    sqlText = "  ";
                    sqlText += @"   INSERT INTO AttendanceRoster(
                                   Id,BranchId,Code,Name,StartDate,EndDate,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                    ) VALUES (
                                    @Id,@BranchId,@Code,@Name,@StartDate,@EndDate,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                    )  ";
                    SqlCommand cmdIns = new SqlCommand(sqlText, currConn);
                    cmdIns.Parameters.AddWithValue("@Id", (RosterNewId + 1));
                    cmdIns.Parameters.AddWithValue("@Code", vm.Code);
                    cmdIns.Parameters.AddWithValue("@Name", vm.Name);
                    cmdIns.Parameters.AddWithValue("@StartDate", vm.StartDate);
                    cmdIns.Parameters.AddWithValue("@EndDate", vm.EndDate);
                    cmdIns.Parameters.AddWithValue("@BranchId", siVM.BranchId);
                    cmdIns.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                    cmdIns.Parameters.AddWithValue("@IsActive", true);
                    cmdIns.Parameters.AddWithValue("@IsArchive", false);
                    cmdIns.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                    cmdIns.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                    cmdIns.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                    cmdIns.Transaction = transaction;
                    cmdIns.ExecuteNonQuery();
                    RosterNewId++;
                    #region Save Detail
                    foreach (var item in vm.attendanceRosterDetails)
                    {
                        string ToDate = Ordinary.DateToString(item.ToDate.ToString());
                        sqlText = "  ";
                        sqlText += @"   INSERT INTO AttendanceRosterDetail(
                                        Id,AttendanceRosterId,StartDate,EndDate,ToDate,AttendanceStructureId,AttendanceGroupId,Remarks
                                        ,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                        ) VALUES (                     
                                        @Id,@AttendanceRosterId,@StartDate,@EndDate,@ToDate,@AttendanceStructureId,@AttendanceGroupId,@Remarks
                                        ,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom   
                                        )  ";
                        SqlCommand cmdInsD = new SqlCommand(sqlText, currConn);
                        cmdInsD.Parameters.AddWithValue("@Id", (RosterDetailNewId + 1));
                        cmdInsD.Parameters.AddWithValue("@AttendanceRosterId", RosterNewId);
                        cmdInsD.Parameters.AddWithValue("@StartDate", vm.StartDate);
                        cmdInsD.Parameters.AddWithValue("@EndDate", vm.EndDate);
                        cmdInsD.Parameters.AddWithValue("@ToDate", ToDate);
                        //cmdInsD.Parameters.AddWithValue("@AttendanceStructureId", item.AttendanceStructureId);
                        cmdInsD.Parameters.AddWithValue("@AttendanceGroupId", item.AttendanceGroupId);
                        cmdInsD.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                        cmdInsD.Parameters.AddWithValue("@IsActive", true);
                        cmdInsD.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsD.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                        cmdInsD.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                        cmdInsD.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                        cmdInsD.Transaction = transaction;
                        cmdInsD.ExecuteNonQuery();
                        RosterDetailNewId++;
                    }
                    #endregion Save Detail
                    #endregion Save
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
                return retResults;
            }
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
            #region Results
            return retResults;
            #endregion
        }
        public string[] BonusStructure(string EmployeeId, string BonusStructureId, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                #region SG Update
                sqlText = "Select count(Id) from EmployeeStructureGroup where EmployeeId=@EmployeeId";
                SqlCommand cmdExists = new SqlCommand(sqlText, currConn);
                cmdExists.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdExists.Transaction = transaction;
                var exeRes = cmdExists.ExecuteScalar();
                int Exists = Convert.ToInt32(exeRes);
                if (Exists <= 0)
                {
                    retResults[1] = "Please Save/Update Employee Job Information";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
                }
                sqlText = "  ";
                sqlText += @" update  EmployeeStructureGroup set 
                            BonusStructureId=@BonusStructureId
                            ,CreatedBy=@CreatedBy
                            ,CreatedAt=@CreatedAt
                            ,CreatedFrom=@CreatedFrom
                            where EmployeeId=@EmployeeId   ";
                SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdsg.Parameters.AddWithValue("@BonusStructureId", BonusStructureId);
                cmdsg.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                cmdsg.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                cmdsg.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                cmdsg.Transaction = transaction;
                cmdsg.ExecuteNonQuery();
                #endregion SG Update
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
        public string[] ProjectAllocation(string EmployeeId, string ProjectAllocationId, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                #region SG Update
                sqlText = "Select count(Id) from EmployeeStructureGroup where EmployeeId=@EmployeeId";
                SqlCommand cmdExists = new SqlCommand(sqlText, currConn);
                cmdExists.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdExists.Transaction = transaction;
                var exeRes = cmdExists.ExecuteScalar();
                int Exists = Convert.ToInt32(exeRes);
                if (Exists <= 0)
                {
                    retResults[1] = "Please Save/Update Employee Job Information";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
                }
                sqlText = "  ";
                sqlText += @" update  EmployeeStructureGroup set 
                            ProjectAllocationId=@ProjectAllocationId
                            ,CreatedBy=@CreatedBy
                            ,CreatedAt=@CreatedAt
                            ,CreatedFrom=@CreatedFrom
                            where EmployeeId=@EmployeeId   ";
                SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdsg.Parameters.AddWithValue("@ProjectAllocationId", ProjectAllocationId);
                cmdsg.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                cmdsg.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                cmdsg.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                cmdsg.Transaction = transaction;
                cmdsg.ExecuteNonQuery();
                #endregion SG Update
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

        public string[] EarningDeductionStructure(string EmployeeId, string EarningDeductionStructureId, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                #region SG Update
                bool isExist = false;
                isExist = _cDal.ExistCheck("EmployeeStructureGroup", "EmployeeId", EmployeeId, currConn, transaction);

                if (!isExist)
                {
                    retResults[1] = "Please Save/Update Employee Job Information";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException(retResults[1], "");
                }
                sqlText = "  ";
                sqlText += @" 
UPDATE  EmployeeStructureGroup SET 
EarningDeductionStructureId=@EarningDeductionStructureId
,CreatedBy=@CreatedBy
,CreatedAt=@CreatedAt
,CreatedFrom=@CreatedFrom
WHERE EmployeeId=@EmployeeId
";
                SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdsg.Parameters.AddWithValue("@EarningDeductionStructureId", EarningDeductionStructureId);
                cmdsg.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                cmdsg.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                cmdsg.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                cmdsg.Transaction = transaction;
                cmdsg.ExecuteNonQuery();
                #endregion SG Update
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

        public string[] EmployeeTaxPortion(string EmployeeId, decimal TaxPortion, decimal EmpTaxValue, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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

                #region SG Update

                bool isExist = false;
                isExist = _cDal.ExistCheck("EmployeeStructureGroup", "EmployeeId", EmployeeId, currConn, transaction);

                if (!isExist)
                {
                    retResults[1] = "Please Save/Update Employee Job Information";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException(retResults[1], "");
                }
                sqlText = "  ";
                sqlText += @" 
UPDATE  EmployeeStructureGroup SET 
TaxPortion=@TaxPortion
,EmpTaxValue = @EmpTaxValue
,CreatedBy=@CreatedBy
,CreatedAt=@CreatedAt
,CreatedFrom=@CreatedFrom
WHERE EmployeeId=@EmployeeId
";
                SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdsg.Parameters.AddWithValue("@TaxPortion", TaxPortion);
                cmdsg.Parameters.AddWithValue("@EmpTaxValue", EmpTaxValue);
                cmdsg.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                cmdsg.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                cmdsg.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                cmdsg.Transaction = transaction;
                cmdsg.ExecuteNonQuery();
                #endregion SG Update
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

        public string[] EmployeeBonusTaxPortion(string EmployeeId, decimal BonusTaxPortion, decimal EmpBonusTaxValue, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

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

                #region SG Update

                bool isExist = false;
                isExist = _cDal.ExistCheck("EmployeeStructureGroup", "EmployeeId", EmployeeId, currConn, transaction);

                if (!isExist)
                {
                    retResults[1] = "Please Save/Update Employee Job Information";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException(retResults[1], "");
                }
                sqlText = "  ";
                sqlText += @" 
UPDATE  EmployeeStructureGroup SET 
BonusTaxPortion=@BonusTaxPortion
,EmpBonusTaxValue = @EmpBonusTaxValue
,CreatedBy=@CreatedBy
,CreatedAt=@CreatedAt
,CreatedFrom=@CreatedFrom
WHERE EmployeeId=@EmployeeId
";
                SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdsg.Parameters.AddWithValue("@BonusTaxPortion", BonusTaxPortion);
                cmdsg.Parameters.AddWithValue("@EmpBonusTaxValue", EmpBonusTaxValue);
                cmdsg.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                cmdsg.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                cmdsg.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                cmdsg.Transaction = transaction;
                cmdsg.ExecuteNonQuery();
                #endregion SG Update
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

        public string[] EmployeeFixedOT(string EmployeeId, decimal FixedOT, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

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

                #region SG Update

                bool isExist = false;
                isExist = _cDal.ExistCheck("EmployeeStructureGroup", "EmployeeId", EmployeeId, currConn, transaction);

                if (!isExist)
                {
                    retResults[1] = "Please Save/Update Employee Job Information";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException(retResults[1], "");
                }
                sqlText = "  ";
                sqlText += @" 
UPDATE  EmployeeStructureGroup SET 
FixedOT=@FixedOT
,CreatedBy=@CreatedBy
,CreatedAt=@CreatedAt
,CreatedFrom=@CreatedFrom
WHERE EmployeeId=@EmployeeId
";
                SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdsg.Parameters.AddWithValue("@FixedOT", FixedOT);
                cmdsg.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                cmdsg.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                cmdsg.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                cmdsg.Transaction = transaction;
                cmdsg.ExecuteNonQuery();
                #endregion SG Update

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
        public string[] EmployeeIsGFApplicable(string EmployeeId, bool IsGFApplicable, ShampanIdentityVM siVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            DataTable dt1 = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

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

                #region SG Update

                bool isExist = false;
                isExist = _cDal.ExistCheck("EmployeeStructureGroup", "EmployeeId", EmployeeId, currConn, transaction);

                if (!isExist)
                {
                    retResults[1] = "Please Save/Update Employee Job Information";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException(retResults[1], "");
                }
                sqlText = "  ";
                sqlText += @" 
UPDATE  EmployeeStructureGroup SET 
IsGFApplicable=@IsGFApplicable
,CreatedBy=@CreatedBy
,CreatedAt=@CreatedAt
,CreatedFrom=@CreatedFrom
WHERE EmployeeId=@EmployeeId
";
                SqlCommand cmdsg = new SqlCommand(sqlText, currConn);
                cmdsg.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmdsg.Parameters.AddWithValue("@IsGFApplicable", IsGFApplicable);
                cmdsg.Parameters.AddWithValue("@CreatedBy", siVM.CreatedBy);
                cmdsg.Parameters.AddWithValue("@CreatedAt", siVM.CreatedAt);
                cmdsg.Parameters.AddWithValue("@CreatedFrom", siVM.CreatedFrom);
                cmdsg.Transaction = transaction;
                cmdsg.ExecuteNonQuery();
                #endregion SG Update

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


        #endregion
    }

    public static class SqlExtensions
    {
        public static void AddWithValueAndNullHandle(this SqlParameterCollection collection, string parameterName, object value)
        {
            if (value == null)
            {
                collection.AddWithValue(parameterName, DBNull.Value);
            }
            else
            {
                collection.AddWithValue(parameterName, value);
            }
        }
        public static int WordCount(this string str, string value)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                StringSplitOptions.RemoveEmptyEntries).Length;
        }
        public static bool IsGreaterThan(this int i)
        {
            return true;
        }


        public static void AddWithValueAndParamCheck(this SqlParameterCollection collection, string parameterName, object value)
        {
            if (collection.Contains(parameterName))
            {
                collection[parameterName].Value = value;
            }
            else
            {
                collection.AddWithValue(parameterName, value);
            }
        }
    }
}
