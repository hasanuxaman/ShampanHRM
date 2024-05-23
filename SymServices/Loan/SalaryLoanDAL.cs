using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Loan;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace SymServices.Payroll
{
    public class SalaryLoanDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion
        //#region Methods
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
            retResults[5] = "Employee Salary Loan Process"; //Method Name

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
                    //    retResults[1] = "This Employee have not assigh JOB yet, Code : " + NoAssignCode;
                    //    retResults[3] = sqlText;
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
                    //retResults[1] = "This Employee have not assigh Salary Structure yet, Code : " + NoAssignCode;
                    //retResults[3] = sqlText;
                    //throw new ArgumentNullException("This Employee have not assigh Salary Structure yet, Code : " + NoAssignCode, "");
                }
                #endregion Employee No Salary Structure Assign Check
                string SalaryLoanID = "-";
                #region SalaryLoanID retrive
                sqlText = "Select Top 1 Id from SalaryLoan where FiscalYearDetailId=@FiscalYearDetailId and BranchId=@BranchId";
                SqlCommand cmdsLoan = new SqlCommand(sqlText, currConn, transaction);
                cmdsLoan.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                cmdsLoan.Parameters.AddWithValue("@BranchId", vm.BranchId);
                using (SqlDataReader dr = cmdsLoan.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        SalaryLoanID = dr["Id"].ToString();
                    }
                    dr.Close();
                }
                if (SalaryLoanID == "-")
                {
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from SalaryLoan where BranchId=@BranchId";
                    SqlCommand cmdempsLoanHCount = new SqlCommand(sqlText, currConn, transaction);
                    cmdempsLoanHCount.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    var tt = cmdempsLoanHCount.ExecuteScalar();
                    int count2 = Convert.ToInt32(tt);// (int)cmdempspfHCount.ExecuteScalar();

                    SalaryLoanID = vm.BranchId.ToString() + "_" + (count2 + 1);
                    sqlText = @" Insert Into SalaryLoan
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

                    SqlCommand employeesLoanHIn = new SqlCommand(sqlText, currConn, transaction);
                    employeesLoanHIn.Parameters.AddWithValue("@Id", SalaryLoanID);
                    employeesLoanHIn.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    employeesLoanHIn.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                    employeesLoanHIn.Parameters.AddWithValue("@Remarks", "-");
                    employeesLoanHIn.Parameters.AddWithValue("@IsActive", true);
                    employeesLoanHIn.Parameters.AddWithValue("@IsArchive", false);
                    employeesLoanHIn.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    employeesLoanHIn.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    employeesLoanHIn.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    employeesLoanHIn.ExecuteNonQuery();
                }

                #endregion

                #region Delete ExistingSalaryLoanDetails
                sqlText = @"Delete SalaryLoanDetail ";
                sqlText += " where SalaryLoanId=@SalaryLoanId";

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
                cmdDeletePrevious.Parameters.AddWithValue("@SalaryLoanId", SalaryLoanID);
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
                sqlText = @"select e.Id employeeId,eld.EmployeeLoanId,eld.id employeeLoanDetail,eld.InstallmentAmount
,e.SectionId,e.ProjectId,e.DepartmentId,e.DesignationId
,fp.id FiscalYearDetailId,eld.InterestAmount
from EmployeeLoanDetail eld
left outer join 
ViewEmployeeInformation e on eld.EmployeeId=e.Id
join (select Id, PeriodStart,PeriodEnd from FiscalYearDetail ) fp on eld.PaymentScheduleDate between fp.PeriodStart and fp.PeriodEnd

where eld.IsArchive=0 and eld.isactive=1
and  e.IsArchive=0 and e.isactive=1

 and fp.Id=@FiscalYearDetailId
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
                sqlText += "  order by e.Id";


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
                        employeeVm.DesignationId = dr["DesignationId"].ToString();

                        employeeVm.EmployeeLoanId = dr["EmployeeLoanId"].ToString();
                        employeeVm.EmployeeLoanDetailId = Convert.ToInt32(dr["employeeLoanDetail"]);
                        employeeVm.Amount = Convert.ToDecimal(dr["InstallmentAmount"]);
                        employeeVm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"]);
                        employeeVm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);

                        employeeVms.Add(employeeVm);
                    }
                }

                sqlText = @"Insert Into SalaryLoanDetail
(
 SalaryLoanId,EmployeeId,FiscalYearDetailId,ProjectId,DepartmentId,SectionId,DesignationId
,LoanAmount,InterestAmount,Remarks,IsActive
,IsArchive,CreatedBy,CreatedAt,CreatedFrom,EmployeeLoanDetailId

) Values (
 @SalaryLoanId,@EmployeeId,@FiscalYearDetailId,@ProjectId
,@DepartmentId,@SectionId,@DesignationId,@LoanAmount,@InterestAmount,@Remarks,@IsActive
,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@EmployeeLoanDetailId
) ";
                SqlCommand cmdempBonusDet;
                if (employeeVms.Count > 0)
                {
                    foreach (EmployeeInfoVM employee in employeeVms)
                    {

                        cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                        cmdempBonusDet.Parameters.AddWithValue("@SalaryLoanId", SalaryLoanID);
                        cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", employee.Id);
                        cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
                        cmdempBonusDet.Parameters.AddWithValue("@ProjectId", employee.ProjectId);
                        cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
                        cmdempBonusDet.Parameters.AddWithValue("@SectionId", employee.SectionId);
                        cmdempBonusDet.Parameters.AddWithValue("@DesignationId", employee.DesignationId);
                        cmdempBonusDet.Parameters.AddWithValue("@LoanAmount", employee.Amount);
                        cmdempBonusDet.Parameters.AddWithValue("@InterestAmount", employee.InterestAmount);
                        cmdempBonusDet.Parameters.AddWithValue("@Remarks", "-");
                        cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                        cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                        cmdempBonusDet.Parameters.AddWithValue("@EmployeeLoanDetailId", employee.EmployeeLoanDetailId);
                        cmdempBonusDet.ExecuteNonQuery();
                    }

                    retResults[1] = employeeVms.Count.ToString() + " Employee Loan Psocess Successed.";
                }
                else
                {
                    retResults[1] = "There have no employee to process Loan.";
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
                retResults[0] = "Fail";//Success or Fail
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

        public string[] InsertSalaryLoanNew(int FiscalYearDetailId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
    , string EmployeeIdF, string EmployeeIdT, string EmpType, FiscalYearVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Employee Salary Earning Process"; //Method Name
            string PeriodEnd = "";
            string PeriodStart = "";
            List<EmployeeInfoVM> employeeVms = new List<EmployeeInfoVM>();
            EmployeeInfoVM employeeVm;
            List<EmployeeLoanDetailVM> EmployeeLoanDetails = new List<EmployeeLoanDetailVM>();
            EmployeeLoanDetailVM EmployeeLoanDetail;

            #endregion

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            EmployeeInfoDAL _EmployeeInfoDAL = new EmployeeInfoDAL();

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

                #region Fiscal Year
                sqlText = @"select * from FiscalYearDetail
                            where id=@FiscalYearDetailsId";
                SqlCommand cmdfy = new SqlCommand(sqlText, currConn, transaction);
                cmdfy.Parameters.AddWithValue("@FiscalYearDetailsId", FiscalYearDetailId);
                using (SqlDataReader dr = cmdfy.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        PeriodEnd = dr["PeriodEnd"].ToString();
                        PeriodStart = dr["PeriodStart"].ToString();
                    }
                    dr.Close();
                }
                #endregion Fiscal Year

                #region Employee Codes

                string EmployeeCodeFrom = "";
                string EmployeeCodeTo = "";

                EmployeeInfoVM varEmployeeInfoVM = new EmployeeInfoVM();
                if (!string.IsNullOrWhiteSpace(EmployeeIdF) && EmployeeIdF != "0_0")
                {
                    varEmployeeInfoVM = new EmployeeInfoVM();

                    string[] cFields = { "EmployeeId" };
                    string[] cValues = { EmployeeIdF };

                    varEmployeeInfoVM = _EmployeeInfoDAL.SelectAll(null, cFields, cValues, currConn, transaction).FirstOrDefault();
                    EmployeeCodeFrom = varEmployeeInfoVM.Code;

                }

                if (!string.IsNullOrWhiteSpace(EmployeeIdT) && EmployeeIdT != "0_0")
                {
                    varEmployeeInfoVM = new EmployeeInfoVM();

                    string[] cFields = { "EmployeeId" };
                    string[] cValues = { EmployeeIdT };

                    varEmployeeInfoVM = _EmployeeInfoDAL.SelectAll(null, cFields, cValues, currConn, transaction).FirstOrDefault();
                    EmployeeCodeTo = varEmployeeInfoVM.Code;

                }

                #endregion

                #region EmployeeList

                #region Assign Data

                varEmployeeInfoVM = new EmployeeInfoVM();

                varEmployeeInfoVM.CodeF = EmployeeCodeFrom;
                varEmployeeInfoVM.CodeT = EmployeeCodeTo;
                varEmployeeInfoVM.PeriodStart = PeriodStart;
                varEmployeeInfoVM.PeriodEnd = PeriodEnd;
                varEmployeeInfoVM.ProjectId = ProjectId;
                varEmployeeInfoVM.DepartmentId = DepartmentId;
                varEmployeeInfoVM.SectionId = SectionId;
                varEmployeeInfoVM.DesignationId = DesignationId;
                ////varEmployeeInfoVM.EmployeeIdF = EmployeeIdF;
                ////varEmployeeInfoVM.EmployeeIdT = EmployeeIdT;
                varEmployeeInfoVM.EmploymentType = EmpType;

                employeeVms = _EmployeeInfoDAL.SelectAllEmployee_SalaryProcess(varEmployeeInfoVM, currConn, transaction);

                #endregion

                #endregion EmployeeList

                #region Comments

                ////                #region EmployeeList

                ////                sqlText = @"
                ////    select  *      from      ViewEmployeeInformation  
                ////    where 1=1    and  BranchId = @BranchId
                ////
                //// ";
                ////                if (ProjectId != "0_0")
                ////                    sqlText += " and  ProjectId=@ProjectId";
                ////                if (DepartmentId != "0_0")
                ////                    sqlText += " and  DepartmentId=@DepartmentId";
                ////                if (SectionId != "0_0")
                ////                    sqlText += " and  SectionId=@SectionId";
                ////                if (DesignationId != "0_0")
                ////                    sqlText += " and  DesignationId=@DesignationId";
                ////                if (EmployeeIdF != "0_0")
                ////                    sqlText += " and  EmployeeId>=@EmployeeIdF";
                ////                if (EmployeeIdT != "0_0")
                ////                    sqlText += " and  EmployeeId<=@EmployeeIdT";
                ////                if (EmpType.ToLower() == "new")
                ////                {
                ////                    sqlText += " and  IsActive=1";
                ////                    sqlText += " and  JoinDate>=@PeriodStart";
                ////                    sqlText += " and  JoinDate<=@PeriodEnd";
                ////                }
                ////                else if (EmpType.ToLower() == "regular")
                ////                {
                ////                    sqlText += " and  IsActive=1";
                ////                    sqlText += " and  JoinDate<@PeriodStart";
                ////                }
                ////                else if (EmpType.ToLower() == "left")
                ////                {
                ////                    sqlText += " and  IsActive=0";
                ////                    sqlText += " and  LeftDate>=@PeriodStart";
                ////                    sqlText += " and  LeftDate<=@PeriodEnd";
                ////                }
                ////                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                ////                cmd.Parameters.AddWithValue("@PeriodStart", PeriodStart);
                ////                cmd.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
                ////                cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);

                ////                if (ProjectId != "0_0")
                ////                    cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
                ////                if (DepartmentId != "0_0")
                ////                    cmd.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                ////                if (SectionId != "0_0")
                ////                    cmd.Parameters.AddWithValue("@SectionId", SectionId);
                ////                if (DesignationId != "0_0")
                ////                    cmd.Parameters.AddWithValue("@DesignationId", DesignationId);
                ////                if (EmployeeIdF != "0_0")
                ////                    cmd.Parameters.AddWithValue("@EmployeeIdF", EmployeeIdF);
                ////                if (EmployeeIdT != "0_0")
                ////                    cmd.Parameters.AddWithValue("@EmployeeIdT", EmployeeIdT);

                ////                using (SqlDataReader dr = cmd.ExecuteReader())
                ////                {
                ////                    while (dr.Read())
                ////                    {
                ////                        employeeVm = new EmployeeInfoVM();
                ////                        employeeVm.Id = dr["Id"].ToString();
                ////                        employeeVm.ProjectId = dr["ProjectId"].ToString();
                ////                        employeeVm.DepartmentId = dr["DepartmentId"].ToString();
                ////                        employeeVm.SectionId = dr["SectionId"].ToString();
                ////                        employeeVm.DesignationId = dr["DesignationId"].ToString();
                ////                        employeeVm.IsPermanent = Convert.ToBoolean(dr["IsPermanent"]);
                ////                        employeeVm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                ////                        employeeVm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                ////                        employeeVm.GradeId = dr["GradeId"].ToString();
                ////                        employeeVms.Add(employeeVm);
                ////                    }
                ////                    dr.Close();
                ////                }
                ////                #endregion EmployeeList

                #endregion

                #region SqlText
                string sqlTextSalaryLoanDetail = "";
                sqlTextSalaryLoanDetail = @"Insert Into SalaryLoanDetail
(
 EmployeeId,FiscalYearDetailId,ProjectId,DepartmentId,SectionId,DesignationId
,LoanAmount,InterestAmount,LoanType_E,Remarks,IsActive
,IsArchive,CreatedBy,CreatedAt,CreatedFrom,EmployeeLoanDetailId,EmployeeStatus,GradeId

) Values (
 @EmployeeId,@FiscalYearDetailId,@ProjectId
,@DepartmentId,@SectionId,@DesignationId,@LoanAmount,@InterestAmount,@LoanType_E,@Remarks,@IsActive
,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@EmployeeLoanDetailId,@EmployeeStatus,@GradeId
) ";
                #endregion

                if (employeeVms.Count > 0)
                {
                    foreach (EmployeeInfoVM employee in employeeVms)
                    {
                        #region Delete ExistingSalaryLoanDetail
                        sqlText = @"Delete SalaryLoanDetail ";
                        sqlText += " where 1=1 and EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId";

                        SqlCommand cmdDeletePFDetail = new SqlCommand(sqlText, currConn, transaction);
                        cmdDeletePFDetail.Parameters.AddWithValue("@EmployeeId", employee.Id);
                        cmdDeletePFDetail.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                        cmdDeletePFDetail.ExecuteNonQuery();

                        #endregion ExistingSalaryLoanDetail

                        #region SalaryLoan
                        sqlText = @"select el.LoanType_E, eld.* from EmployeeLoanDetail eld 
left outer join EmployeeLoan el on eld.EmployeeLoanId=el.Id";
                        sqlText += @" where 1=1 and eld.EmployeeId=@EmployeeId 
                       and eld.IsHold=0 and  eld.IsPaid=0  and  eld.HaveDuplicate=0 
                        and eld.PaymentScheduleDate>=@PeriodStart
                        and eld.PaymentScheduleDate<=@PeriodEnd
";


                        SqlCommand cmdepf = new SqlCommand(sqlText, currConn, transaction);
                        cmdepf.Parameters.AddWithValue("@EmployeeId", employee.Id);
                        cmdepf.Parameters.AddWithValue("@PeriodStart", PeriodStart);
                        cmdepf.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
                        EmployeeLoanDetails = new List<EmployeeLoanDetailVM>();
                        using (SqlDataReader drSSD = cmdepf.ExecuteReader())
                        {
                            while (drSSD.Read())
                            {
                                EmployeeLoanDetail = new EmployeeLoanDetailVM();

                                EmployeeLoanDetail.PrincipalAmount = Convert.ToDecimal(drSSD["PrincipalAmount"]);
                                EmployeeLoanDetail.InterestAmount = Convert.ToDecimal(drSSD["InterestAmount"]);
                                EmployeeLoanDetail.Id = Convert.ToInt32(drSSD["Id"]);
                                EmployeeLoanDetail.Remarks = drSSD["Remarks"].ToString();
                                EmployeeLoanDetail.LoanType_E = drSSD["LoanType_E"].ToString();

                                EmployeeLoanDetails.Add(EmployeeLoanDetail);
                            }
                            drSSD.Close();
                        }
                        #endregion SalaryLoan

                        if (EmployeeLoanDetails.Count > 0)
                        {
                            foreach (EmployeeLoanDetailVM ELD in EmployeeLoanDetails)
                            {
                                #region SqlExecution

                                SqlCommand cmdempBonusDet;

                                cmdempBonusDet = new SqlCommand(sqlTextSalaryLoanDetail, currConn, transaction);
                                cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", employee.Id);
                                cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                                cmdempBonusDet.Parameters.AddWithValue("@ProjectId", employee.ProjectId);
                                cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
                                cmdempBonusDet.Parameters.AddWithValue("@SectionId", employee.SectionId);
                                cmdempBonusDet.Parameters.AddWithValue("@DesignationId", employee.DesignationId);

                                cmdempBonusDet.Parameters.AddWithValue("@LoanType_E", ELD.LoanType_E);
                                cmdempBonusDet.Parameters.AddWithValue("@LoanAmount", ELD.PrincipalAmount);
                                cmdempBonusDet.Parameters.AddWithValue("@InterestAmount", ELD.InterestAmount);
                                cmdempBonusDet.Parameters.AddWithValue("@Remarks", ELD.Remarks);
                                cmdempBonusDet.Parameters.AddWithValue("@EmployeeLoanDetailId", ELD.Id);

                                cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                                cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                                cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                                cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                                cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                                cmdempBonusDet.Parameters.AddWithValue("@EmployeeStatus", EmpType);
                                cmdempBonusDet.Parameters.AddWithValue("@GradeId", employee.GradeId);
                                cmdempBonusDet.ExecuteNonQuery();
                                #endregion
                            }
                        }
                    }

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
        public string[] SalaryLoanSingleAddorUpdate(SalaryLoanDetailVM vm, int branchId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Employee Salary Loan Process Single"; //Method Name


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
                string SalaryLoanID = "-";
                sqlText = "Select Top 1 Id from SalaryLoan where FiscalYearDetailId=@FiscalYearDetailId and BranchId=@BranchId";
                SqlCommand cmdempBonusSelect = new SqlCommand(sqlText, currConn, transaction);
                cmdempBonusSelect.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                cmdempBonusSelect.Parameters.AddWithValue("@BranchId", branchId);
                using (SqlDataReader dr = cmdempBonusSelect.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        SalaryLoanID = dr["Id"].ToString();
                    }
                    dr.Close();
                }
                if (SalaryLoanID == "-")
                {
                    sqlText = "Select isnull( isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0),0) from SalaryLoan where BranchId=@BranchId";
                    SqlCommand cmdempBonusCount = new SqlCommand(sqlText, currConn, transaction);
                    cmdempBonusCount.Parameters.AddWithValue("@BranchId", branchId);
                    var exeRes = cmdempBonusCount.ExecuteScalar();
                    int count2 = Convert.ToInt32(exeRes);

                    SalaryLoanID = branchId.ToString() + "_" + (count2 + 1);
                    sqlText = @" Insert Into SalaryLoan
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
                    employeeBnous.Parameters.AddWithValue("@Id", SalaryLoanID);
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

                sqlText = @"Delete SalaryLoanDetail ";
                sqlText += " where SalaryLoanId=@SalaryLoanId";
                sqlText += " and EmployeeId=@EmployeeId";

                SqlCommand cmdDeletePrevious = new SqlCommand(sqlText, currConn, transaction);
                cmdDeletePrevious.Parameters.AddWithValue("@SalaryLoanId", SalaryLoanID);
                cmdDeletePrevious.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdDeletePrevious.ExecuteNonQuery();


                EmployeeInfoVM employeeVm = null;
                sqlText = @"select e.Id employeeId,eld.EmployeeLoanId,eld.id employeeLoanDetail,eld.InstallmentAmount
	,e.SectionId,e.ProjectId,e.DepartmentId, e.DesignationId 
,fp.id FiscalYearDetailId,eld.InterestAmount
from EmployeeLoanDetail eld
left join EmployeeLoan el on el.Id=eld.EmployeeLoanId
join (select Id, PeriodStart,PeriodEnd from FiscalYearDetail where Id=218) fp on eld.PaymentScheduleDate between fp.PeriodStart and fp.PeriodEnd
join ViewEmployeeInformation e on e.Id=eld.EmployeeId
join EmployeeTransfer t on t.EmployeeId=e.Id and t.IsCurrent=1
	 where el.IsHold=0 and eld.IsHold=0 and eld.IsPaid=0 and  e.IsArchive=0 And e.IsActive=1  and e.Id=@EmployeeID";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@EmployeeID", vm.EmployeeId);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        employeeVm = new EmployeeInfoVM();
                        employeeVm.Id = dr["employeeId"].ToString();
                        employeeVm.SectionId = dr["SectionId"].ToString();
                        employeeVm.ProjectId = dr["ProjectId"].ToString();
                        employeeVm.DepartmentId = dr["DepartmentId"].ToString();
                        employeeVm.DesignationId = dr["DesignationId"].ToString();
                        employeeVm.EmployeeLoanId = dr["EmployeeLoanId"].ToString();
                        employeeVm.EmployeeLoanDetailId = Convert.ToInt32(dr["employeeLoanDetail"]);
                        employeeVm.Amount = Convert.ToDecimal(dr["InstallmentAmount"]);
                        employeeVm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"]);
                        employeeVm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    }
                }

                sqlText = @"Insert Into SalaryLoanDetail

(
 SalaryLoanId
,EmployeeId
,FiscalYearDetailId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,LoanAmount
,InterestAmount
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,EmployeeLoanDetailId

) Values (
 @SalaryLoanId
,@EmployeeId
,@FiscalYearDetailId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@LoanAmount
,@InterestAmount
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@EmployeeLoanDetailId
) ";
                SqlCommand cmdempBonusDet;
                if (employeeVm != null)
                {
                    cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                    cmdempBonusDet.Parameters.AddWithValue("@SalaryLoanId", SalaryLoanID);
                    cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", employeeVm.Id);
                    cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", employeeVm.FiscalYearDetailId);
                    cmdempBonusDet.Parameters.AddWithValue("@ProjectId", employeeVm.ProjectId);
                    cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", employeeVm.DepartmentId);
                    cmdempBonusDet.Parameters.AddWithValue("@SectionId", employeeVm.SectionId);
                    cmdempBonusDet.Parameters.AddWithValue("@DesignationId", employeeVm.DesignationId);
                    cmdempBonusDet.Parameters.AddWithValue("@LoanAmount", employeeVm.Amount);
                    cmdempBonusDet.Parameters.AddWithValue("@InterestAmount", employeeVm.InterestAmount);

                    cmdempBonusDet.Parameters.AddWithValue("@Remarks", "-");
                    cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                    cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                    cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdempBonusDet.Parameters.AddWithValue("@EmployeeLoanDetailId", employeeVm.EmployeeLoanDetailId);
                    cmdempBonusDet.ExecuteNonQuery();

                    retResults[1] = " Employee Loan Psocess Successed.";
                }
                else
                {
                    retResults[1] = "Have no data to process Loan";
                    throw new ArgumentException("Have no data to process Loan", "Have no data to process Loan");
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
        public List<SalaryLoanDetailVM> SelectAll(int? fid = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryLoanDetailVM> VMs = new List<SalaryLoanDetailVM>();
            SalaryLoanDetailVM vm;
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
select
ve.LoanAmount PrincipalAmount
,ve.InterestAmount
,ve.LoanType_E
,elt.Name LoanTypeName
,ve.FiscalYearDetailId
,fyd.PeriodName
,ve.Department
,ve.Project
,ve.Designation
,ve.Section
,ve.EmpName
,ve.Code
,ve.EmployeeId
,ve.BasicSalary
,ve.GrossSalary
,ve.JoinDate
,ve.DepartmentId
,ve.DesignationId
,ve.ProjectId
,ve.SectionId
from viewSalaryLoanDetail ve
left outer join FiscalYearDetail fyd on ve.FiscalYearDetailId=fyd.Id
left outer join EnumLoanType elt on ve.LoanType_E=elt.Id
Where ve.IsArchive=0 And ve.IsActive=1 And ve.LoanAmount >0
";

                if (fid != null && fid != 0)
                {
                    sqlText += @" and ve.FiscalYearDetailId='" + fid + "'";
                }
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryLoanDetailVM();
                    vm.PrincipalAmount = Convert.ToDecimal(dr["PrincipalAmount"].ToString());
                    vm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"].ToString());
                    vm.InstallmentAmount = vm.PrincipalAmount + vm.InterestAmount;
                    vm.LoanTypeName = dr["LoanTypeName"].ToString();
                    vm.LoanType_E = dr["LoanType_E"].ToString();
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"].ToString());
                    vm.PeriodName = dr["PeriodName"].ToString();

                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());

                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    VMs.Add(vm);
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
      
        public List<SalaryLoanDetailVM> GetPeriodname()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryLoanDetailVM> vms = new List<SalaryLoanDetailVM>();
            SalaryLoanDetailVM vm;
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
select 
distinct fyd.PeriodName
,fyd.PeriodStart
,ve.FiscalYearDetailId,
ve.Remarks
from  ViewSalaryLoanDetail ve 
left outer join FiscalYearDetail fyd on ve.FiscalYearDetailId =fyd.Id
Where 1=1 
order by PeriodStart
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryLoanDetailVM();
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"].ToString());
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vms.Add(vm);
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
            return vms;
        }
        public string[] Delete(SalaryLoanVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryLoanD"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update SalaryLoanDetail set";
                        sqlText += " LoanAmount=0";
                        sqlText += " , IsArchive=@IsArchive";
                        sqlText += " , LastUpdateBy=@LastUpdateBy";
                        sqlText += " , LastUpdateAt=@LastUpdateAt";
                        sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }

                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException("Salary Loan Delete", vm.Id + " could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Loan Details Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary Loan Details.";
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
        public List<SalaryLoanDetailVM> SelectAllSalaryLoanDetails(string salaryLoanId, SqlConnection currConn, bool callFromOutSide)
        {

            #region Variables

            string sqlText = "";
            List<SalaryLoanDetailVM> VMs = new List<SalaryLoanDetailVM>();
            SalaryLoanDetailVM vm;
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
 sLoand.Id
,sLoand.SalaryLoanId
,sLoand.EmployeeId
,sLoand.LoanAmount
,sLoand.Remarks
,e.Salutation_E
,e.MiddleName
,e.LastName
 from SalaryLoanDetail sLoand
 left join EmployeeInfo e on e.Id=sLoand.EmployeeId
Where sLoand.SalaryLoanId=@SalaryLoanId and sLoand.IsArchive=0";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@SalaryLoanId", salaryLoanId);

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new SalaryLoanDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.SalaryLoanId = dr["SalaryLoanId"].ToString();
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.PrincipalAmount = Convert.ToDecimal(dr["LoanAmount"]);
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
        public SalaryLoanDetailVM GetByIdSalaryLoanDetails(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SalaryLoanDetailVM vm = new SalaryLoanDetailVM();

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

                sqlText = @"SELECT
sa.Id
,sa.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,sa.LoanAmount
,sa.FiscalYearDetailId
,sa.InterestAmount
,edt.Name LoadTypeName
,sa.Remarks
,sa.IsActive
,sa.IsArchive
,sa.CreatedBy
,sa.CreatedAt
,sa.CreatedFrom
,sa.LastUpdateBy
,sa.LastUpdateAt
,sa.LastUpdateFrom

From SalaryLoanDetail sa 
left outer join ViewEmployeeInformation e on sa.EmployeeId=e.id
left outer join EnumLoanType edt on sa.LoanType_E = edt.Id
Where 1=1 and  sa.IsArchive=0  and sa.id=@Id and sa.LoanAmount > 0
     
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.PrincipalAmount = Convert.ToDecimal(dr["LoanAmount"]);
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
                    vm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"].ToString());
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.LoanTypeName = dr["LoadTypeName"].ToString();

                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"].ToString());
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
        //==================SelectByID=================
        public List<SalaryLoanDetailVM> SelectByIdandFiscalyearDetail(string empId, int FiscalYearDetailId = 0, string edType = "0")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryLoanDetailVM> vms = new List<SalaryLoanDetailVM>();
            SalaryLoanDetailVM vm = new SalaryLoanDetailVM();
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
sa.Id
,sa.EmployeeId
,sa.InterestAmount
,sa.LoanAmount
,sa.FiscalYearDetailId
,sa.LoanType_E
,lot.Name LoadTypeName
,sa.Remarks
From SalaryLoanDetail sa 
left outer join EnumLoanType lot on lot.Id=sa.LoanType_E 
where sa.FiscalYearDetailId=@FiscalYearDetailId and sa.EmployeeId=@Id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", empId);

                objComm.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryLoanDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.PrincipalAmount = Convert.ToDecimal(dr["LoanAmount"]);
                    vm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.LoanType_E = dr["LoanType_E"].ToString();
                    vm.LoanTypeName = dr["LoadTypeName"].ToString();

                    vm.Remarks = dr["Remarks"].ToString();
                    vms.Add(vm);
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
            return vms;
        }
        public string[] SalaryLoanSingleEdit(SalaryLoanDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Employee Salary Loan Process Single Edit"; //Method Name

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

                sqlText = @"Update  SalaryLoanDetail

set
 LoanAmount            =@LoanAmount
,Remarks            =@Remarks
,LastUpdateBy       =@LastUpdateBy
,LastUpdateAt       =@LastUpdateAt
,LastUpdateFrom     =@LastUpdateFrom
where Id=@Id
";

                SqlCommand cmdempBonusDet;
                cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                cmdempBonusDet.Parameters.AddWithValue("@Id", vm.Id);
                cmdempBonusDet.Parameters.AddWithValue("@LoanAmount", vm.PrincipalAmount);
                cmdempBonusDet.Parameters.AddWithValue("@Remarks", vm.Remarks);
                cmdempBonusDet.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                cmdempBonusDet.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                cmdempBonusDet.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                cmdempBonusDet.ExecuteNonQuery();

                #region Save

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
        public string GetPeriodName(string SalaryLoanId)
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

                sqlText = @"select f.PeriodName from SalaryLoan t 
join FiscalYearDetail f on f.id=t.FiscalYearDetailId
where t.Id=@SalaryLoanId";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@SalaryLoanId", SalaryLoanId);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        result = dr["PeriodName"].ToString();
                    }
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
        public List<SalaryLoanDetailVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string Orderby)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryLoanDetailVM> VMs = new List<SalaryLoanDetailVM>();
            SalaryLoanDetailVM vm;
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
select
slnd.LoanAmount
,slnd.PeriodName
,slnd.Department
,slnd.Project
,slnd.Designation
,slnd.Section
,slnd.EmpName
,slnd.BasicSalary
,slnd.GrossSalary
,slnd.JoinDate
,slnd.DepartmentId
,slnd.DesignationId
,slnd.ProjectId
,slnd.SectionId
,fyd.PeriodStart                 
,slnd.FiscalYearDetailId
,slnd.Code
,slnd.EmployeeId
from ViewSalaryLoanDetail slnd
left outer join grade g on slnd.gradeId = g.id
left outer join FiscalYearDetail fyd on slnd.FiscalYearDetailId =fyd.Id
Where 1=1 And slnd.LoanAmount >0
";

                if (fid != 0)
                {
                    sqlText += @" and slnd.FiscalYearDetailId>='" + fid + "'";
                }
                if (fidTo != 0)
                {
                    sqlText += @" and slnd.FiscalYearDetailId<='" + fidTo + "'";
                }

                if (ProjectId != "0_0")
                    sqlText += " and slnd.ProjectId=@ProjectId";

                if (DepartmentId != "0_0")
                    sqlText += " and slnd.DepartmentId=@DepartmentId ";

                if (SectionId != "0_0")
                    sqlText += " and slnd.SectionId=@SectionId ";

                if (DesignationId != "0_0")
                    sqlText += " and slnd.DesignationId=@DesignationId ";

                if (CodeF != "0_0")
                    sqlText += " and slnd.Code>= @CodeF";

                if (CodeT != "0_0")
                    sqlText += " and slnd.Code<= @CodeT";

                //sqlText += " ORDER BY slnd.FiscalYearDetailId, slnd.Department, slnd.Section, slnd.Code ";

                sqlText += " ORDER BY slnd.FiscalYearDetailId";

                if (Orderby == "DCG")
                    sqlText += " , slnd.department, slnd.code, g.sl";
                else if (Orderby == "DDC")
                    sqlText += " ,slnd.department, slnd.JoinDate, slnd.code";
                else if (Orderby == "DGC")
                    sqlText += " , slnd.department, g.sl, slnd.code";
                else if (Orderby == "DGDC")
                    sqlText += ", slnd.department, g.sl, slnd.JoinDate, slnd.code";
                else if (Orderby == "CODE")
                    sqlText += ", slnd.code";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (ProjectId != "0_0")
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);

                if (DepartmentId != "0_0")
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);

                if (SectionId != "0_0")
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);

                if (DesignationId != "0_0")
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);

                if (CodeF != "0_0")
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);

                if (CodeT != "0_0")
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryLoanDetailVM();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();

                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.LoanAmount = Convert.ToDecimal(dr["LoanAmount"].ToString());
                    vm.PrincipalAmount = Convert.ToDecimal(dr["LoanAmount"].ToString());
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());

                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"].ToString());
                    VMs.Add(vm);
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
        //#endregion
        public decimal GetChildAllowanceAmount(string empid, int fid)
        {

            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            FiscalYearDAL fdal = new FiscalYearDAL();
            FiscalYearVM fidvm = new FiscalYearVM();
            SalaryLoanDetailVM vm = new SalaryLoanDetailVM();
            EmployeeDependentDAL empDependDAL = new EmployeeDependentDAL();
            List<EmployeeDependentVM> empDependVms = new List<EmployeeDependentVM>();
            decimal ChildAllowanceAmount;
            #endregion
            try
            {
                empDependVms = empDependDAL.SelectAllByEmployee(empid);
                SettingDAL setDal = new SettingDAL();
                int age = Convert.ToInt32(setDal.settingValue("Dependent", "Age", null, null));
                var AllowNumber = Convert.ToInt32(setDal.settingValue("Dependent", "Allow", null, null));
                var Allowance = Convert.ToDecimal(setDal.settingValue("Dependent", "Allowance", null, null));
                var FullPeriodName = Convert.ToDateTime(fdal.FYPeriodDetail(fid, null, null).Find(m => m.Id == fid).PeriodName);
                int count = 0;
                foreach (var item in empDependVms)
                {
                    int currentChildage = (Int32.Parse(FullPeriodName.ToString("yyyyMMdd")) - Int32.Parse(Convert.ToDateTime(item.DateofBirth).ToString("yyyyMMdd"))) / 10000;
                    if (age >= currentChildage)
                    {
                        count++;
                    }
                }
                if (count == 1)
                {
                    ChildAllowanceAmount = Allowance * 1;
                }
                else if (count == 0)
                {
                    ChildAllowanceAmount = Allowance * 0;
                }
                else if (count >= AllowNumber)
                {
                    if (count < AllowNumber)
                        ChildAllowanceAmount = Allowance * count;
                    ChildAllowanceAmount = Allowance * AllowNumber;
                }
                else
                {
                    ChildAllowanceAmount = Allowance * AllowNumber;
                }
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
            return ChildAllowanceAmount;
        }
    }
}
