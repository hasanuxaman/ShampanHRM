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
    public class SalaryAreerDAL
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
            retResults[5] = "Employee Salary Areer Process"; //Method Name

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
                    //retResults[1] = "This Employee have not assigh JOB yet, Code : " + NoAssignCode;
                    //retResults[3] = sqlText;
                    //throw new ArgumentNullException("This Employee have not assigh JOB yet. Code : " + NoAssignCode, "");
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
                //    retResults[1] = "This Employee have not assigh Salary Structure yet, Code : " + NoAssignCode;
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("This Employee have not assigh Salary Structure yet, Code : " + NoAssignCode, "");
                }
                #endregion Employee No Salary Structure Assign Check

                string SalaryAreerID = "-";
                #region SalaryAreerID retrive
                sqlText = "Select Top 1 Id from SalaryAreer where FiscalYearDetailId=@FiscalYearDetailId and BranchId=@BranchId";
                SqlCommand cmdsAreer = new SqlCommand(sqlText, currConn, transaction);
                cmdsAreer.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                cmdsAreer.Parameters.AddWithValue("@BranchId", vm.BranchId);
                using (SqlDataReader dr = cmdsAreer.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        SalaryAreerID = dr["Id"].ToString();
                    }
                    dr.Close();
                }
                if (SalaryAreerID == "-")
                {
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from SalaryAreer where BranchId=@BranchId";
                    SqlCommand cmdempsAreerHCount = new SqlCommand(sqlText, currConn, transaction);
                    cmdempsAreerHCount.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    var tt = cmdempsAreerHCount.ExecuteScalar();
                    int count2 = Convert.ToInt32(tt);// (int)cmdempspfHCount.ExecuteScalar();

                    SalaryAreerID = vm.BranchId.ToString() + "_" + (count2 + 1);
                    sqlText = @" Insert Into SalaryAreer
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

                    SqlCommand employeesAreerHIn = new SqlCommand(sqlText, currConn, transaction);
                    employeesAreerHIn.Parameters.AddWithValue("@Id", SalaryAreerID);
                    employeesAreerHIn.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    employeesAreerHIn.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                    employeesAreerHIn.Parameters.AddWithValue("@Remarks", "-");
                    employeesAreerHIn.Parameters.AddWithValue("@IsActive", true);
                    employeesAreerHIn.Parameters.AddWithValue("@IsArchive", false);
                    employeesAreerHIn.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    employeesAreerHIn.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    employeesAreerHIn.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    employeesAreerHIn.ExecuteNonQuery();
                }

                #endregion

                #region Delete ExistingSalaryAreerDetails
                sqlText = @"Delete SalaryAreerDetail ";
                sqlText += " where SalaryAreerId=@SalaryAreerId";

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
                cmdDeletePrevious.Parameters.AddWithValue("@SalaryAreerId", SalaryAreerID);
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
                sqlText = @"	select e.Id employeeId,sum(ea.AreerAmount)Amount
	,e.SectionId,e.ProjectId,e.DepartmentId
from EmployeeAreer ea
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

                sqlText = @"Insert Into SalaryAreerDetail

(
 SalaryAreerId
,EmployeeId
,FiscalYearDetailId
,ProjectId
,DepartmentId
,SectionId
,AreerAmount
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) Values (
 @SalaryAreerId
,@EmployeeId
,@FiscalYearDetailId
,@ProjectId
,@DepartmentId
,@SectionId
,@AreerAmount
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
                        cmdempBonusDet.Parameters.AddWithValue("@SalaryAreerId", SalaryAreerID);
                        cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                        cmdempBonusDet.Parameters.AddWithValue("@ProjectId", ProjectId);
                        cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                        cmdempBonusDet.Parameters.AddWithValue("@SectionId", SectionId);
                        cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", item.Id);

                        cmdempBonusDet.Parameters.AddWithValue("@AreerAmount", item.Amount);
                        cmdempBonusDet.Parameters.AddWithValue("@Remarks", "-");
                        cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                        cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                        cmdempBonusDet.ExecuteNonQuery();
                    }

                    retResults[1] = employeeVms.Count.ToString() + " Employee Areer Psocess Successed.";
                }
                else
                {
                    retResults[1] = "There have no employee to process Areer.";
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
            return retResults;

            #endregion
        }
        public string[] SalaryAreerSingleAddorUpdate(SalaryAreerDetailVM vm, int branchId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Employee Salary Areer Process Single"; //Method Name


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
                string SalaryAreerID = "-";
                sqlText = "Select Top 1 Id from SalaryAreer where FiscalYearDetailId=@FiscalYearDetailId and BranchId=@BranchId";
                SqlCommand cmdempBonusSelect = new SqlCommand(sqlText, currConn, transaction);
                cmdempBonusSelect.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                cmdempBonusSelect.Parameters.AddWithValue("@BranchId", branchId);
                using (SqlDataReader dr = cmdempBonusSelect.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        SalaryAreerID = dr["Id"].ToString();
                    }
                    dr.Close();
                }
                if (SalaryAreerID == "-")
                {
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from SalaryAreer where BranchId=@BranchId";
                    SqlCommand cmdempBonusCount = new SqlCommand(sqlText, currConn, transaction);
                    cmdempBonusCount.Parameters.AddWithValue("@BranchId", branchId);
					var exeRes = cmdempBonusCount.ExecuteScalar();
					int count2 = Convert.ToInt32(exeRes);

                    SalaryAreerID = branchId.ToString() + "_" + (count2 + 1);
                    sqlText = @" Insert Into SalaryAreer
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
                    employeeBnous.Parameters.AddWithValue("@Id", SalaryAreerID);
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

                sqlText = @"Delete SalaryAreerDetail ";
                sqlText += " where SalaryAreerId=@SalaryAreerId";
                sqlText += " and EmployeeId=@EmployeeId";

                SqlCommand cmdDeletePrevious = new SqlCommand(sqlText, currConn, transaction);
                cmdDeletePrevious.Parameters.AddWithValue("@SalaryAreerId", SalaryAreerID);
                cmdDeletePrevious.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdDeletePrevious.ExecuteNonQuery();


                EmployeeInfoVM employeeVm = null;
                sqlText = @"select e.Id employeeId,sum(ea.AreerAmount)Amount
	,t.SectionId,t.ProjectId,t.DepartmentId
from EmployeeAreer ea
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

                sqlText = @"Insert Into SalaryAreerDetail

(
 SalaryAreerId
,EmployeeId
,FiscalYearDetailId
,ProjectId
,DepartmentId
,SectionId
,AreerAmount
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) Values (
 @SalaryAreerId
,@EmployeeId
,@FiscalYearDetailId
,@ProjectId
,@DepartmentId
,@SectionId
,@AreerAmount
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
                    cmdempBonusDet.Parameters.AddWithValue("@SalaryAreerId", SalaryAreerID);
                    cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdempBonusDet.Parameters.AddWithValue("@ProjectId", employeeVm.ProjectId);
                    cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", employeeVm.DepartmentId);
                    cmdempBonusDet.Parameters.AddWithValue("@SectionId", employeeVm.SectionId);
                    cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", employeeVm.Id);

                    cmdempBonusDet.Parameters.AddWithValue("@AreerAmount", employeeVm.Amount);
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
                    retResults[1] = "Have no data to process areer";
                    throw new ArgumentException("Have no data to process areer", "Have no data to process areer");
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
        public List<SalaryAreerVM> SelectAll(int BranchId,int? fid=null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryAreerVM> VMs = new List<SalaryAreerVM>();
            SalaryAreerVM vm;
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
sAreer.Id
,sareerd.AreerAmount
,vw.Department
,vw.Project
,vw.Designation
,vw.Section
,vw.EmpName
,vw.BasicSalary
,vw.GrossSalary
,vw.JoinDate
,vw.Code
,fyd.PeriodName
,vw.DepartmentId
,vw.DesignationId
,vw.ProjectId
,vw.SectionId
,sareerd.FiscalYearDetailId
from SalaryAreer sAreer
left outer join SalaryAreerDetail sareerd on sAreer.Id=sareerd.SalaryAreerId
left outer join FiscalYearDetail fyd on sareerd.FiscalYearDetailId=fyd.Id
left outer join ViewEmployeeInformation vw on sareerd.EmployeeId=vw.id
Where sAreer.IsArchive=0 And sAreer.IsActive=1 And sAreer.BranchId=@BranchId and sareerd.AreerAmount >=0";

                if (  fid != null && fid != 0)
                {
                    sqlText += @" and sareerd.FiscalYearDetailId='" + fid + "'";
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
                    vm = new SalaryAreerVM();
                    vm.Id = dr["Id"].ToString();
                    vm.AreerAmount = Convert.ToDecimal(dr["AreerAmount"].ToString());
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.JoinDate = dr["JoinDate"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"].ToString());
                  
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
        public string[] SalaryAreerDetailsDelete(string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryAreerD"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "Delete SalaryAreerDetail where Id=@Id";
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
                    //    throw new ArgumentNullException("Salary Areer Delete", vm.Id + " could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Areer Details Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary Areer Details.";
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
        public string[] SalaryAreerDelete(string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSalaryAreer"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryAreer"); }

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
                        sqlText = "Delete SalaryAreer where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);

                        #endregion

                        #region Details
                        sqlText = "";
                        sqlText = "Delete SalaryAreerDetail where SalaryAreerId=@Id";
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
                    //    throw new ArgumentNullException("Salary Areer Delete"," could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Areer Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary Areer.";
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
        public List<SalaryAreerDetailVM> SelectAllSalaryAreerDetails(string salaryAreerId, SqlConnection currConn, bool callFromOutSide)
        {

            #region Variables

            string sqlText = "";
            List<SalaryAreerDetailVM> VMs = new List<SalaryAreerDetailVM>();
            SalaryAreerDetailVM vm;
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
 sAreerd.Id
,sAreerd.SalaryAreerId
,sAreerd.EmployeeId
,sAreerd.AreerAmount
,sAreerd.Remarks
,e.Salutation_E
,e.MiddleName
,e.LastName
 from SalaryAreerDetail sAreerd
 left join EmployeeInfo e on e.Id=sAreerd.EmployeeId
Where sAreerd.SalaryAreerId=@SalaryAreerId and sAreerd.IsArchive=0";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@SalaryAreerId", salaryAreerId);

                using (SqlDataReader dr = objCommVehicle.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new SalaryAreerDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.SalaryAreerId = dr["SalaryAreerId"].ToString();
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.AreerAmount = Convert.ToDecimal(dr["AreerAmount"]);
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
        public SalaryAreerDetailVM GetByIdSalaryAreerDetails(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SalaryAreerDetailVM vm = new SalaryAreerDetailVM();

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
 sAreerd.Id
,sAreerd.AreerAmount
,sAreerd.Remarks
,e.Salutation_E
,e.MiddleName
,e.LastName
,sAreer.FiscalYearDetailId
 from SalaryAreerDetail sAreerd
 left join EmployeeInfo e on e.Id=sAreerd.EmployeeId
 left join SalaryAreer sAreer on sAreer.Id=sAreerd.SalaryAreerId
 where sAreerd.Id=@Id
     
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
                    vm.AreerAmount = Convert.ToDecimal(dr["AreerAmount"]);
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
        public string[] SalaryAreerSingleEdit(SalaryAreerDetailVM vm)
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
            retResults[5] = "Employee Salary Areer Process Single Edit"; //Method Name

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

                sqlText = @"Update  SalaryAreerDetail

set
 AreerAmount            =@AreerAmount
,Remarks            =@Remarks
,LastUpdateBy       =@LastUpdateBy
,LastUpdateAt       =@LastUpdateAt
,LastUpdateFrom     =@LastUpdateFrom
where Id=@Id
";

                SqlCommand cmdempBonusDet;
                cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                cmdempBonusDet.Parameters.AddWithValue("@Id", vm.Id);
                cmdempBonusDet.Parameters.AddWithValue("@AreerAmount", vm.AreerAmount);
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
        public string GetPeriodName(string SalaryAreerId)
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

                sqlText = @"select f.PeriodName from SalaryAreer t 
join FiscalYearDetail f on f.id=t.FiscalYearDetailId
where t.Id=@SalaryAreerId";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@SalaryAreerId", SalaryAreerId);
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
