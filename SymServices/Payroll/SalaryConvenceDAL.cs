using SymOrdinary;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.Payroll
{
    public class SalaryConvenceDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion
        #region Methods
        //==================Insert =================
        public string[] AddOrUpdate(int FiscalYearDetailsId, string ProjectId, string DepartmentId, string SectionId, FiscalYearVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Employee Salary Convence Process"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
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

                string PeriodEnd = "";
                string NoAssignCode = "";

                #region Fiscal Year Last Date
                sqlText = @"select PeriodEnd from FiscalYearDetail
                            where id=@FiscalYearDetailsId";
                SqlCommand cmdfy = new SqlCommand(sqlText, currConn, transaction);
                cmdfy.Parameters.AddWithValue("@FiscalYearDetailsId", FiscalYearDetailsId);
                using (SqlDataReader dr = cmdfy.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        PeriodEnd = dr["PeriodEnd"].ToString();
                    }
                    dr.Close();
                }
                #endregion Fiscal Year Last Date
                #region Employee No Job Assign Check
                sqlText = @"select   isnull(Stuff((SELECT ', ' + Code 
                    FROM ViewEmployeeInformation 
                    where id not in (select EmployeeId from EmployeeJob)
                    and  IsArchive=0 and isactive=1
                    and BranchId=@BranchId
                    and JoinDate<=@PeriodEnd
                    FOR XML PATH('')),1,1,''),'NA')  Code";
                SqlCommand cmdnja = new SqlCommand(sqlText, currConn, transaction);
                cmdnja.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdnja.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
                using (SqlDataReader dr = cmdnja.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        NoAssignCode = dr["Code"].ToString();
                    }
                    dr.Close();
                }
                if (!string.IsNullOrWhiteSpace(NoAssignCode) && NoAssignCode != "NA")
                {
                    retResults[1] = "This Employee have not assigh JOB yet, Code : " + NoAssignCode;
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("This Employee have not assigh JOB yet. Code : " + NoAssignCode, "");
                }
                #endregion Employee No Job Assign Check
                #region Employee No Salary Structure Assign Check
                sqlText = @"select   isnull(Stuff((SELECT ', ' + Code 
                FROM ViewEmployeeInformation 
                where id not in (select EmployeeId from EmployeeSalaryStructure)
                and  IsArchive=0 and isactive=1
                and BranchId=@BranchId
                and JoinDate<=@PeriodEnd
                FOR XML PATH('')),1,1,''),'NA')  Code";
                SqlCommand cmdnss = new SqlCommand(sqlText, currConn, transaction);
                cmdnss.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdnss.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
                NoAssignCode = "";
                using (SqlDataReader dr = cmdnss.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        NoAssignCode = dr["Code"].ToString();
                    }
                    dr.Close();
                }
                if (!string.IsNullOrWhiteSpace(NoAssignCode) && NoAssignCode != "NA")
                {
                    retResults[1] = "This Employee have not assigh Salary Structure yet, Code : " + NoAssignCode;
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("This Employee have not assigh Salary Structure yet, Code : " + NoAssignCode, "");
                }
                #endregion Employee No Salary Structure Assign Check

                string SalaryConvenceID = "-";
                #region SalaryConvenceID retrive
                sqlText = "Select Top 1 Id from SalaryConvence where FiscalYearDetailId=@FiscalYearDetailId and BranchId=@BranchId";
                SqlCommand cmdsConvence = new SqlCommand(sqlText, currConn, transaction);
                cmdsConvence.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                cmdsConvence.Parameters.AddWithValue("@BranchId", vm.BranchId);
                using (SqlDataReader dr = cmdsConvence.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        SalaryConvenceID = dr["Id"].ToString();
                    }
                    dr.Close();
                }
                if (SalaryConvenceID == "-")
                {
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from SalaryConvence where BranchId=@BranchId";
                    SqlCommand cmdempsConvenceHCount = new SqlCommand(sqlText, currConn, transaction);
                    cmdempsConvenceHCount.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    var tt = cmdempsConvenceHCount.ExecuteScalar();
                    int count2 = Convert.ToInt32(tt);// (int)cmdempspfHCount.ExecuteScalar();

                    SalaryConvenceID = vm.BranchId.ToString() + "_" + (count2 + 1);
                    sqlText = @" Insert Into SalaryConvence
(
 Id
,BranchId
,FiscalYearDetailId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) VALUES (
 @Id
,@BranchId
,@FiscalYearDetailId
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)
";

                    SqlCommand employeesConvenceHIn = new SqlCommand(sqlText, currConn, transaction);
                    employeesConvenceHIn.Parameters.AddWithValue("@Id", SalaryConvenceID);
                    employeesConvenceHIn.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    employeesConvenceHIn.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                    employeesConvenceHIn.Parameters.AddWithValue("@Remarks", "-");
                    employeesConvenceHIn.Parameters.AddWithValue("@IsActive", true);
                    employeesConvenceHIn.Parameters.AddWithValue("@IsArchive", false);
                    employeesConvenceHIn.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    employeesConvenceHIn.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    employeesConvenceHIn.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    employeesConvenceHIn.ExecuteNonQuery();
                }

                #endregion

                #region Delete ExistingSalaryConvenceDetails
                sqlText = @"Delete SalaryConvenceDetail ";
                sqlText += " where SalaryConvenceId=@SalaryConvenceId";

                if (ProjectId != "0_0")
                {
                    sqlText += " and ProjectId=@ProjectId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += " and DepartmentId=@DepartmentId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += " and SectionId=@SectionId";
                }

                SqlCommand cmdDeletePrevious = new SqlCommand(sqlText, currConn, transaction);
                cmdDeletePrevious.Parameters.AddWithValue("@SalaryConvenceId", SalaryConvenceID);
                if (ProjectId != "0_0")
                {
                    cmdDeletePrevious.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (DepartmentId != "0_0")
                {
                    cmdDeletePrevious.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (SectionId != "0_0")
                {
                    cmdDeletePrevious.Parameters.AddWithValue("@SectionId", SectionId);
                }
                cmdDeletePrevious.ExecuteNonQuery();
                #endregion

                List<EmployeeInfoVM> employeeVms = new List<EmployeeInfoVM>();
                EmployeeInfoVM employeeVm;
                sqlText = @"	select e.Id employeeId,sum(ea.ConvenceAmount)Amount
	,e.SectionId,e.ProjectId,e.DepartmentId
from EmployeeConvence ea
	 left outer join ViewEmployeeInformation e on e.Id=ea.EmployeeId
	 where  e.IsArchive=0 And e.IsActive=1
and ea.FiscalYearDetailId=@FiscalYearDetailId
    and e.JoinDate <=@PeriodEnd
    and e.BranchId = @BranchId
 ";
                if (ProjectId != "0_0")
                {
                    sqlText += " and e.ProjectId=@ProjectId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += " and e.DepartmentId=@DepartmentId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += " and e.SectionId=@SectionId";
                }
                sqlText += " group by e.Id	,e.SectionId,e.ProjectId,e.DepartmentId   order by e.Id";


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                cmd.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
                cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                if (ProjectId != "0_0")
                {
                    cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (DepartmentId != "0_0")
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (SectionId != "0_0")
                {
                    cmd.Parameters.AddWithValue("@SectionId", SectionId);
                }

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        employeeVm = new EmployeeInfoVM();
                        employeeVm.Id = dr["employeeId"].ToString();
                        employeeVm.SectionId = dr["SectionId"].ToString();
                        employeeVm.ProjectId = dr["ProjectId"].ToString();
                        employeeVm.DepartmentId = dr["DepartmentId"].ToString();
                        employeeVm.Amount = Convert.ToDecimal(dr["Amount"]);
                        employeeVms.Add(employeeVm);
                    }
                    dr.Close();
                }

                sqlText = @"Insert Into SalaryConvenceDetail

(
 SalaryConvenceId
,EmployeeId
,FiscalYearDetailId
,ProjectId
,DepartmentId
,SectionId
,ConvenceAmount
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) Values (
 @SalaryConvenceId
,@EmployeeId
,@FiscalYearDetailId
,@ProjectId
,@DepartmentId
,@SectionId
,@ConvenceAmount
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) ";
                SqlCommand cmdempBonusDet;
                if (employeeVms.Count > 0)
                {
                    foreach (EmployeeInfoVM item in employeeVms)
                    {

                        cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                        cmdempBonusDet.Parameters.AddWithValue("@SalaryConvenceId", SalaryConvenceID);
                        cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                        cmdempBonusDet.Parameters.AddWithValue("@ProjectId", ProjectId);
                        cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                        cmdempBonusDet.Parameters.AddWithValue("@SectionId", SectionId);
                        cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", item.Id);

                        cmdempBonusDet.Parameters.AddWithValue("@ConvenceAmount", item.Amount);
                        cmdempBonusDet.Parameters.AddWithValue("@Remarks", "-");
                        cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                        cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                        cmdempBonusDet.ExecuteNonQuery();
                    }

                    retResults[1] = employeeVms.Count.ToString() + " Employee Convence Psocess Successed.";
                }
                else
                {
                    retResults[1] = "There have no employee to process Convence.";
                }

                #region Save

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

                #endregion SuccessResult

            }

            #endregion try

            #region Catch and Finall



            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
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
        public string[] SalaryConvenceSingleAddorUpdate(SalaryConvenceDetailVM vm, int branchId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Employee Salary Convence Process Single"; //Method Name


            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;


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
                string SalaryConvenceID = "-";
                sqlText = "Select Top 1 Id from SalaryConvence where FiscalYearDetailId=@FiscalYearDetailId and BranchId=@BranchId";
                SqlCommand cmdempBonusSelect = new SqlCommand(sqlText, currConn, transaction);
                cmdempBonusSelect.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                cmdempBonusSelect.Parameters.AddWithValue("@BranchId", branchId);
                using (SqlDataReader dr = cmdempBonusSelect.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        SalaryConvenceID = dr["Id"].ToString();
                    }
                    dr.Close();
                }
                if (SalaryConvenceID == "-")
                {
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from SalaryConvence where BranchId=@BranchId";
                    SqlCommand cmdempBonusCount = new SqlCommand(sqlText, currConn, transaction);
                    cmdempBonusCount.Parameters.AddWithValue("@BranchId", branchId);
					var exeRes = cmdempBonusCount.ExecuteScalar();
					int count2 = Convert.ToInt32(exeRes);

                    SalaryConvenceID = branchId.ToString() + "_" + (count2 + 1);
                    sqlText = @" Insert Into SalaryConvence
(
 Id
,BranchId
,FiscalYearDetailId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) VALUES (
 @Id
,@BranchId
,@FiscalYearDetailId
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)
";

                    SqlCommand employeeBnous = new SqlCommand(sqlText, currConn, transaction);
                    employeeBnous.Parameters.AddWithValue("@Id", SalaryConvenceID);
                    employeeBnous.Parameters.AddWithValue("@BranchId", branchId);
                    employeeBnous.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    employeeBnous.Parameters.AddWithValue("@Remarks", "-");
                    employeeBnous.Parameters.AddWithValue("@IsActive", true);
                    employeeBnous.Parameters.AddWithValue("@IsArchive", false);
                    employeeBnous.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    employeeBnous.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    employeeBnous.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    employeeBnous.ExecuteNonQuery();
                }

                sqlText = @"Delete SalaryConvenceDetail ";
                sqlText += " where SalaryConvenceId=@SalaryConvenceId";
                sqlText += " and EmployeeId=@EmployeeId";

                SqlCommand cmdDeletePrevious = new SqlCommand(sqlText, currConn, transaction);
                cmdDeletePrevious.Parameters.AddWithValue("@SalaryConvenceId", SalaryConvenceID);
                cmdDeletePrevious.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdDeletePrevious.ExecuteNonQuery();


                EmployeeInfoVM employeeVm = null;
                sqlText = @"select e.Id employeeId,sum(ea.ConvenceAmount)Amount
	,t.SectionId,t.ProjectId,t.DepartmentId
from EmployeeConvence ea
	 join EmployeeInfo e on e.Id=ea.EmployeeId
	 join EmployeeTransfer t on t.EmployeeId=e.Id and t.IsCurrent=1
	 where  e.IsArchive=0 And e.IsActive=1  and e.Id=@EmployeeID
	  group by e.Id	,t.SectionId,t.ProjectId,t.DepartmentId";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@EmployeeID", vm.EmployeeId);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        employeeVm = new EmployeeInfoVM();
                        employeeVm.Id = dr["employeeId"].ToString();
                        employeeVm.ProjectId = dr["ProjectId"].ToString();
                        employeeVm.DepartmentId = dr["DepartmentId"].ToString();
                        employeeVm.SectionId = dr["SectionId"].ToString();
                        employeeVm.Amount = Convert.ToDecimal(dr["Amount"]);
                    }
                    dr.Close();
                }

                sqlText = @"Insert Into SalaryConvenceDetail

(
 SalaryConvenceId
,EmployeeId
,FiscalYearDetailId
,ProjectId
,DepartmentId
,SectionId
,ConvenceAmount
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) Values (
 @SalaryConvenceId
,@EmployeeId
,@FiscalYearDetailId
,@ProjectId
,@DepartmentId
,@SectionId
,@ConvenceAmount
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) ";
                SqlCommand cmdempBonusDet;
                if (employeeVm != null)
                {

                    cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                    cmdempBonusDet.Parameters.AddWithValue("@SalaryConvenceId", SalaryConvenceID);
                    cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdempBonusDet.Parameters.AddWithValue("@ProjectId", employeeVm.ProjectId);
                    cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", employeeVm.DepartmentId);
                    cmdempBonusDet.Parameters.AddWithValue("@SectionId", employeeVm.SectionId);
                    cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", employeeVm.Id);

                    cmdempBonusDet.Parameters.AddWithValue("@ConvenceAmount", employeeVm.Amount);
                    cmdempBonusDet.Parameters.AddWithValue("@Remarks", "-");
                    cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                    cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                    cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdempBonusDet.ExecuteNonQuery();

                }
                else
                {
                    retResults[1] = "Have no data to process Convence";
                    throw new ArgumentException("Have no data to process Convence", "Have no data to process Convence");
                }
                #region Save

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

                #endregion SuccessResult

            }

            #endregion try

            #region Catch and Finall



            catch (Exception ex)
            {
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
        public List<SalaryConvenceVM> SelectAll(int BranchId,int? fid=null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryConvenceVM> VMs = new List<SalaryConvenceVM>();
            SalaryConvenceVM vm;
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

                sqlText = @"select 
sConvence.Id
,sConvence.BranchId
,sConvence.Remarks
,scd.ConvenceAmount
,fd.PeriodName
,vw.Department
,vw.Project
,vw.Designation
,vw.Section
,vw.EmpName
,vw.BasicSalary
,vw.GrossSalary
,vw.JoinDate
,vw.DepartmentId
,vw.DesignationId
,vw.ProjectId
,vw.SectionId
,scd.FiscalYearDetailId
,vw.Code
from SalaryConvence sConvence
left outer join SalaryConvenceDetail scd on scd.SalaryConvenceId=sConvence.id
left outer join ViewEmployeeInformation vw on scd.EmployeeId=vw.id
left join FiscalYearDetail fd on fd.Id=sConvence.FiscalYearDetailId
Where sConvence.IsArchive=0 And sConvence.IsActive=1 And sConvence.BranchId=@BranchId and scd.ConvenceAmount >=0";
                if (fid != null && fid != 0)
                {
                    sqlText += @" and scd.FiscalYearDetailId='" + fid + "'";
                }
                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@BranchId", BranchId);

                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryConvenceVM();
                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.ConvenceAmount = Convert.ToDecimal(dr["ConvenceAmount"].ToString());
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
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
        public string[] SalaryConvenceDetailsDelete(string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteBonusDetails"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryConvenceD"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "Delete SalaryConvenceDetail where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }




                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException("Salary Convence Delete", vm.Id + " could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Convence Details Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary Convence Details.";
                    throw new ArgumentNullException("", "");
                }

            }
            #region catch
            catch (Exception ex)
            {
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
        public string[] SalaryConvenceDelete(string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSalaryConvence"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryConvence"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        #region Header
                        sqlText = "";
                        sqlText = "Delete SalaryConvence where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);

                        #endregion

                        #region Details
                        sqlText = "";
                        sqlText = "Delete SalaryConvenceDetail where SalaryConvenceId=@Id";
                        SqlCommand cmdUpdate2 = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate2.Parameters.AddWithValue("@Id", Ids[i]);
					    exeRes = cmdUpdate2.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);

                        #endregion
                    }




                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException("Salary Convence Delete"," could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Convence Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary Convence.";
                    throw new ArgumentNullException("", "");
                }

            }
            #region catch
            catch (Exception ex)
            {
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
        public List<SalaryConvenceDetailVM> SelectAllSalaryConvenceDetails(string salaryConvenceId, SqlConnection currConn, bool callFromOutSide)
        {

            #region Variables

            string sqlText = "";
            List<SalaryConvenceDetailVM> VMs = new List<SalaryConvenceDetailVM>();
            SalaryConvenceDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                }
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @" 

select 
 sConvenced.Id
,sConvenced.SalaryConvenceId
,sConvenced.EmployeeId
,sConvenced.ConvenceAmount
,sConvenced.Remarks
,e.Salutation_E
,e.MiddleName
,e.LastName
 from SalaryConvenceDetail sConvenced
 left join EmployeeInfo e on e.Id=sConvenced.EmployeeId
Where sConvenced.SalaryConvenceId=@SalaryConvenceId and sConvenced.IsArchive=0";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@SalaryConvenceId", salaryConvenceId);

                using (SqlDataReader dr = objCommVehicle.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new SalaryConvenceDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.SalaryConvenceId = dr["SalaryConvenceId"].ToString();
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.ConvenceAmount = Convert.ToDecimal(dr["ConvenceAmount"]);
                        vm.Remarks = dr["Remarks"].ToString();
                        vm.EmployeeName = dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                        VMs.Add(vm);
                    }
                    dr.Close();
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
                if (currConn != null && !callFromOutSide)
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
        public SalaryConvenceDetailVM GetByIdSalaryConvenceDetails(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SalaryConvenceDetailVM vm = new SalaryConvenceDetailVM();

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

                sqlText = @"select 
 sConvenced.Id
,sConvenced.ConvenceAmount
,sConvenced.Remarks
,e.Salutation_E
,e.MiddleName
,e.LastName
,sConvence.FiscalYearDetailId
 from SalaryConvenceDetail sConvenced
 left join EmployeeInfo e on e.Id=sConvenced.EmployeeId
 left join SalaryConvence sConvence on sConvence.Id=sConvenced.SalaryConvenceId
 where sConvenced.Id=@Id
     
";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@Id", Id);

                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.ConvenceAmount = Convert.ToDecimal(dr["ConvenceAmount"]);
                    vm.Remarks = dr["Remarks"].ToString();

                    vm.EmployeeName = dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();

                    vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
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
        public string[] SalaryConvenceSingleEdit(SalaryConvenceDetailVM vm)
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
            retResults[5] = "Employee Salary Convence Process Single Edit"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region open connection and transaction
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

                sqlText = @"Update  SalaryConvenceDetail

set
 ConvenceAmount            =@ConvenceAmount
,Remarks            =@Remarks
,LastUpdateBy       =@LastUpdateBy
,LastUpdateAt       =@LastUpdateAt
,LastUpdateFrom     =@LastUpdateFrom
where Id=@Id
";

                SqlCommand cmdempBonusDet;
                cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                cmdempBonusDet.Parameters.AddWithValue("@Id", vm.Id);
                cmdempBonusDet.Parameters.AddWithValue("@ConvenceAmount", vm.ConvenceAmount);
                cmdempBonusDet.Parameters.AddWithValue("@Remarks", vm.Remarks);
                cmdempBonusDet.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                cmdempBonusDet.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                cmdempBonusDet.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                cmdempBonusDet.ExecuteNonQuery();

                #region Save

                #endregion Save


                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";

                #endregion SuccessResult

            }

            #endregion try

            #region Catch and Finall



            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex

                if (Vtransaction == null) { transaction.Rollback(); }
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
        public string GetPeriodName(string SalaryConvenceId)
        {
            string result = "";
            SqlConnection currConn = null;
            string sqlText = "";
            try
            {


                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                sqlText = @"select f.PeriodName from SalaryConvence t 
join FiscalYearDetail f on f.id=t.FiscalYearDetailId
where t.Id=@SalaryConvenceId";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@SalaryConvenceId", SalaryConvenceId);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        result = dr["PeriodName"].ToString();
                    }
                    dr.Close();
                }
            }
            #region catch
            catch (Exception ex)
            {
                return "";
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

            return result;
        }
        #endregion

    }
}
