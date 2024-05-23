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
    public class SalaryOverTimeDAL
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
            retResults[5] = "Employee Salary OverTime Process"; //Method Name

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

                string SalaryOverTimeID = "-";
                #region SalaryOverTimeID retrive
                sqlText = "Select Top 1 Id from SalaryOverTime where FiscalYearDetailId=@FiscalYearDetailId and BranchId=@BranchId";
                SqlCommand cmdsOverTime = new SqlCommand(sqlText, currConn, transaction);
                cmdsOverTime.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                cmdsOverTime.Parameters.AddWithValue("@BranchId", vm.BranchId);
                using (SqlDataReader dr = cmdsOverTime.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        SalaryOverTimeID = dr["Id"].ToString();
                    }
                    dr.Close();
                }
                if (SalaryOverTimeID == "-")
                {
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from SalaryOverTime where BranchId=@BranchId";
                    SqlCommand cmdempsOverTimeHCount = new SqlCommand(sqlText, currConn, transaction);
                    cmdempsOverTimeHCount.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    var tt = cmdempsOverTimeHCount.ExecuteScalar();
                    int count2 = Convert.ToInt32(tt);// (int)cmdempspfHCount.ExecuteScalar();

                    SalaryOverTimeID = vm.BranchId.ToString() + "_" + (count2 + 1);
                    sqlText = @" Insert Into SalaryOverTime
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

                    SqlCommand employeesOverTimeHIn = new SqlCommand(sqlText, currConn, transaction);
                    employeesOverTimeHIn.Parameters.AddWithValue("@Id", SalaryOverTimeID);
                    employeesOverTimeHIn.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    employeesOverTimeHIn.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                    employeesOverTimeHIn.Parameters.AddWithValue("@Remarks", "-");
                    employeesOverTimeHIn.Parameters.AddWithValue("@IsActive", true);
                    employeesOverTimeHIn.Parameters.AddWithValue("@IsArchive", false);
                    employeesOverTimeHIn.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    employeesOverTimeHIn.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    employeesOverTimeHIn.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    employeesOverTimeHIn.ExecuteNonQuery();
                }

                #endregion

                #region Delete ExistingSalaryOverTimeDetails
                sqlText = @"Delete SalaryOverTimeDetail ";
                sqlText += " where SalaryOverTimeId=@SalaryOverTimeId";

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
                cmdDeletePrevious.Parameters.AddWithValue("@SalaryOverTimeId", SalaryOverTimeID);
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
                sqlText = @"	select e.Id employeeId,sum(ea.OverTimeAmount)Amount
	,e.SectionId,e.ProjectId,e.DepartmentId
from EmployeeOverTime ea
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

                sqlText = @"Insert Into SalaryOverTimeDetail

(
 SalaryOverTimeId
,EmployeeId
,FiscalYearDetailId
,ProjectId
,DepartmentId
,SectionId
,OverTimeAmount
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) Values (
 @SalaryOverTimeId
,@EmployeeId
,@FiscalYearDetailId
,@ProjectId
,@DepartmentId
,@SectionId
,@OverTimeAmount
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
                        cmdempBonusDet.Parameters.AddWithValue("@SalaryOverTimeId", SalaryOverTimeID);
                        cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                        cmdempBonusDet.Parameters.AddWithValue("@ProjectId", ProjectId);
                        cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                        cmdempBonusDet.Parameters.AddWithValue("@SectionId", SectionId);
                        cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", item.Id);

                        cmdempBonusDet.Parameters.AddWithValue("@OverTimeAmount", item.Amount);
                        cmdempBonusDet.Parameters.AddWithValue("@Remarks", "-");
                        cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                        cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                        cmdempBonusDet.ExecuteNonQuery();
                    }

                    retResults[1] = employeeVms.Count.ToString() + " Employee OverTime Psocess Successed.";
                }
                else
                {
                    retResults[1] = "There have no employee to process OverTime.";
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
        public string[] SalaryOverTimeSingleAddorUpdate(SalaryOverTimeDetailVM vm, int branchId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Employee Salary OverTime Process Single"; //Method Name


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
                string SalaryOverTimeID = "-";
                sqlText = "Select Top 1 Id from SalaryOverTime where FiscalYearDetailId=@FiscalYearDetailId and BranchId=@BranchId";
                SqlCommand cmdempBonusSelect = new SqlCommand(sqlText, currConn, transaction);
                cmdempBonusSelect.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                cmdempBonusSelect.Parameters.AddWithValue("@BranchId", branchId);
                using (SqlDataReader dr = cmdempBonusSelect.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        SalaryOverTimeID = dr["Id"].ToString();
                    }
                    dr.Close();
                }
                if (SalaryOverTimeID == "-")
                {
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from SalaryOverTime where BranchId=@BranchId";
                    SqlCommand cmdempBonusCount = new SqlCommand(sqlText, currConn, transaction);
                    cmdempBonusCount.Parameters.AddWithValue("@BranchId", branchId);
					var exeRes = cmdempBonusCount.ExecuteScalar();
					int count2 = Convert.ToInt32(exeRes);

                    SalaryOverTimeID = branchId.ToString() + "_" + (count2 + 1);
                    sqlText = @" Insert Into SalaryOverTime
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
                    employeeBnous.Parameters.AddWithValue("@Id", SalaryOverTimeID);
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

                sqlText = @"Delete SalaryOverTimeDetail ";
                sqlText += " where SalaryOverTimeId=@SalaryOverTimeId";
                sqlText += " and EmployeeId=@EmployeeId";

                SqlCommand cmdDeletePrevious = new SqlCommand(sqlText, currConn, transaction);
                cmdDeletePrevious.Parameters.AddWithValue("@SalaryOverTimeId", SalaryOverTimeID);
                cmdDeletePrevious.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdDeletePrevious.ExecuteNonQuery();


                EmployeeInfoVM employeeVm = null;
                sqlText = @"select e.Id employeeId,sum(ea.OverTimeAmount)Amount
	,t.SectionId,t.ProjectId,t.DepartmentId
from EmployeeOverTime ea
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

                sqlText = @"Insert Into SalaryOverTimeDetail

(
 SalaryOverTimeId
,EmployeeId
,FiscalYearDetailId
,ProjectId
,DepartmentId
,SectionId
,OverTimeAmount
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) Values (
 @SalaryOverTimeId
,@EmployeeId
,@FiscalYearDetailId
,@ProjectId
,@DepartmentId
,@SectionId
,@OverTimeAmount
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
                    cmdempBonusDet.Parameters.AddWithValue("@SalaryOverTimeId", SalaryOverTimeID);
                    cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdempBonusDet.Parameters.AddWithValue("@ProjectId", employeeVm.ProjectId);
                    cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", employeeVm.DepartmentId);
                    cmdempBonusDet.Parameters.AddWithValue("@SectionId", employeeVm.SectionId);
                    cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", employeeVm.Id);

                    cmdempBonusDet.Parameters.AddWithValue("@OverTimeAmount", employeeVm.Amount);
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
                    retResults[1] = "Have no data to process OverTime";
                    throw new ArgumentException("Have no data to process OverTime", "Have no data to process OverTime");
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
        public List<SalaryOverTimeVM> SelectAll(int BranchId,int? fid=null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryOverTimeVM> VMs = new List<SalaryOverTimeVM>();
            SalaryOverTimeVM vm;
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
sOverTime.Id
,sOverTime.BranchId
,sOverTime.Remarks
,sot.OverTimeAmount
,fyd.PeriodName
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
,sot.FiscalYearDetailId
,vw.Code
from SalaryOverTime sOverTime
left outer join SalaryOverTimeDetail sot on sot.SalaryOverTimeId=sOverTime.id
left outer join ViewEmployeeInformation vw on sot.EmployeeId=vw.id
left join FiscalYearDetail fyd on fyd.Id=sOverTime.FiscalYearDetailId
Where sOverTime.IsArchive=0 And sOverTime.IsActive=1 And sOverTime.BranchId=@BranchId and sot.OverTimeAmount >=0";
                if (fid != null && fid != 0)
                {
                    sqlText += @" and sot.FiscalYearDetailId='" + fid + "'";
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
                    vm = new SalaryOverTimeVM();
                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.OverTimeAmount = Convert.ToDecimal(dr["OverTimeAmount"].ToString());
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                   
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
        public string[] SalaryOverTimeDetailsDelete(string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryOverTimeD"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "Delete SalaryOverTimeDetail where Id=@Id";
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
                    //    throw new ArgumentNullException("Salary OverTime Delete", vm.Id + " could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary OverTime Details Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary OverTime Details.";
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
        public string[] SalaryOverTimeDelete(string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSalaryOverTime"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryOverTime"); }

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
                        sqlText = "Delete SalaryOverTime where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);

                        #endregion

                        #region Details
                        sqlText = "";
                        sqlText = "Delete SalaryOverTimeDetail where SalaryOverTimeId=@Id";
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
                    //    throw new ArgumentNullException("Salary OverTime Delete"," could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary OverTime Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary OverTime.";
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
        public List<SalaryOverTimeDetailVM> SelectAllSalaryOverTimeDetails(string salaryOverTimeId, SqlConnection currConn, bool callFromOutSide)
        {

            #region Variables

            string sqlText = "";
            List<SalaryOverTimeDetailVM> VMs = new List<SalaryOverTimeDetailVM>();
            SalaryOverTimeDetailVM vm;
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
 sOverTimed.Id
,sOverTimed.SalaryOverTimeId
,sOverTimed.EmployeeId
,sOverTimed.OverTimeAmount
,sOverTimed.Remarks
,e.Salutation_E
,e.MiddleName
,e.LastName
 from SalaryOverTimeDetail sOverTimed
 left join EmployeeInfo e on e.Id=sOverTimed.EmployeeId
Where sOverTimed.SalaryOverTimeId=@SalaryOverTimeId and sOverTimed.IsArchive=0";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@SalaryOverTimeId", salaryOverTimeId);

                using (SqlDataReader dr = objCommVehicle.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new SalaryOverTimeDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.SalaryOverTimeId = dr["SalaryOverTimeId"].ToString();
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.OverTimeAmount = Convert.ToDecimal(dr["OverTimeAmount"]);
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
        public SalaryOverTimeDetailVM GetByIdSalaryOverTimeDetails(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SalaryOverTimeDetailVM vm = new SalaryOverTimeDetailVM();

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
 sOverTimed.Id
,sOverTimed.OverTimeAmount
,sOverTimed.Remarks
,e.Salutation_E
,e.MiddleName
,e.LastName
,sOverTime.FiscalYearDetailId
 from SalaryOverTimeDetail sOverTimed
 left join EmployeeInfo e on e.Id=sOverTimed.EmployeeId
 left join SalaryOverTime sOverTime on sOverTime.Id=sOverTimed.SalaryOverTimeId
 where sOverTimed.Id=@Id
     
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
                    vm.OverTimeAmount = Convert.ToDecimal(dr["OverTimeAmount"]);
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
        public string[] SalaryOverTimeSingleEdit(SalaryOverTimeDetailVM vm)
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
            retResults[5] = "Employee Salary OverTime Process Single Edit"; //Method Name

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

                sqlText = @"Update  SalaryOverTimeDetail

set
 OverTimeAmount            =@OverTimeAmount
,Remarks            =@Remarks
,LastUpdateBy       =@LastUpdateBy
,LastUpdateAt       =@LastUpdateAt
,LastUpdateFrom     =@LastUpdateFrom
where Id=@Id
";

                SqlCommand cmdempBonusDet;
                cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                cmdempBonusDet.Parameters.AddWithValue("@Id", vm.Id);
                cmdempBonusDet.Parameters.AddWithValue("@OverTimeAmount", vm.OverTimeAmount);
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
        public string GetPeriodName(string SalaryOverTimeId)
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

                sqlText = @"select f.PeriodName from SalaryOverTime t 
join FiscalYearDetail f on f.id=t.FiscalYearDetailId
where t.Id=@SalaryOverTimeId";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@SalaryOverTimeId", SalaryOverTimeId);
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
