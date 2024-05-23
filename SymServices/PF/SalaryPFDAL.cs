using Excel;
using Microsoft.Office.Interop.Excel;
using SymOrdinary;
using SymServices.Attendance;
using SymServices.Common;
using SymServices.HRM;
using SymServices.Payroll;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace SymServices.PF
{
    public class SalaryPFDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion
        //#region Methods
        //==================Insert =================






        public string[] InsertSalaryPFNew(int FiscalYearDetailId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string EmployeeIdF, string EmployeeIdT, string EmpType
            , FiscalYearVM vm, string CompanyName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            /*EmpType
           New, Regular,OutGoing
            */



            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Salary PF Process"; //Method Name
            List<EmployeeInfoVM> employeeVms = new List<EmployeeInfoVM>();
            EmployeeInfoVM employeeVm;
            EmployeeInfoDAL _EmployeeInfoDAL = new EmployeeInfoDAL();
            MonthlyAttendanceVM monthlyAttendanceVM = new MonthlyAttendanceVM();
            MonthlyAttendanceDAL _monthlyAttendanceDAL = new MonthlyAttendanceDAL();


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

                #region Fiscal Year
                string PeriodEnd = "";
                string PeriodStart = "";
                sqlText = "";
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
                if (!string.IsNullOrWhiteSpace(EmployeeIdF) && !string.IsNullOrWhiteSpace(EmployeeIdT)
                    && EmployeeIdF == EmployeeIdT
                    && EmployeeIdF != "0_0"
                    && EmployeeIdT != "0_0")
                {
                    varEmployeeInfoVM = new EmployeeInfoVM();

                    string[] cFields = { "EmployeeId" };
                    string[] cValues = { EmployeeIdF };

                    varEmployeeInfoVM = _EmployeeInfoDAL.SelectAll(null, cFields, cValues, currConn, transaction).FirstOrDefault();
                    EmployeeCodeFrom = varEmployeeInfoVM.Code;
                    EmployeeCodeTo = varEmployeeInfoVM.Code;
                }
                else
                {
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
                varEmployeeInfoVM.FiscalYearDetailId = FiscalYearDetailId;
                varEmployeeInfoVM.CompanyName = CompanyName;

                employeeVms = _EmployeeInfoDAL.SelectAllEmployee_SalaryProcess(varEmployeeInfoVM, currConn, transaction);
                if (varEmployeeInfoVM.EmploymentType.ToLower()=="left")
                {
                    
                }
                #endregion

                #endregion EmployeeList

                #region Comments

                //////                sqlText = @"
                //////SELECT  
                //////ve.Id
                //////,ve.ProjectId
                //////,ve.DepartmentId
                //////,ve.SectionId
                //////,ve.DesignationId
                //////,ve.GradeId
                //////,ve.IsActive
                //////,ve.IsPermanent
                //////,ve.Code
                //////,ISNULL(fs.Gross,0) GrossSalary
                //////,ISNULL(fs.Basic,0) BasicSalary
                //////
                //////FROM ViewEmployeeInformation ve
                //////LEFT OUTER JOIN
                //////(
                //////SELECT Salary.EmployeeId
                //////,SUM(Salary.Basic)Basic
                //////,SUM(Salary.HouseRent)HouseRent
                //////,SUM(Salary.Medical)Medical
                //////,SUM(Salary.Conveyance)Conveyance
                //////,SUM(Salary.Gross)Gross
                //////FROM(
                ////// SELECT 
                //////ssd.EmployeeId
                //////,ISNULL(CASE WHEN  ssd.SalaryType = 'Basic' THEN ssd.Amount  else 0 END, 0) AS Basic
                //////,ISNULL(CASE WHEN   ssd.SalaryType = 'HouseRent' THEN ssd.Amount else 0 END, 0) AS HouseRent
                //////,ISNULL(CASE WHEN   ssd.SalaryType = 'Medical' THEN ssd.Amount  else 0 END, 0) AS Medical
                //////,ISNULL(CASE WHEN   ssd.SalaryType = 'Conveyance' THEN ssd.Amount  else 0 END, 0) AS Conveyance 
                //////,ISNULL(CASE WHEN   ssd.SalaryType = 'Gross' THEN ssd.Amount  else 0 END, 0) AS Gross 
                //////FROM EmployeeSalaryStructureDetail ssd
                ////// WHERE 1=1
                //////and ssd.IncrementDate<=@PeriodEnd  
                //////) AS Salary
                //////GROUP BY Salary.EmployeeId
                //////) AS fs ON ve.EmployeeId = fs.EmployeeId
                //////WHERE 1=1
                //////
                ////// ";

                //////                {
                //////                    if (ProjectId != "0_0")
                //////                        sqlText += " and  ve.ProjectId=@ProjectId";
                //////                    if (DepartmentId != "0_0")
                //////                        sqlText += " and  ve.DepartmentId=@DepartmentId";
                //////                    if (SectionId != "0_0")
                //////                        sqlText += " and  ve.SectionId=@SectionId";
                //////                    if (DesignationId != "0_0")
                //////                        sqlText += " and  ve.DesignationId=@DesignationId";
                //////                }

                //////                if (!string.IsNullOrWhiteSpace(EmployeeCodeFrom))
                //////                {
                //////                    sqlText += " and  ve.Code>=@EmployeeCodeFrom";
                //////                }
                //////                if (!string.IsNullOrWhiteSpace(EmployeeCodeTo))
                //////                {
                //////                    sqlText += " and  ve.Code<=@EmployeeCodeTo";
                //////                }


                //////                if (EmpType.ToLower() == "new")
                //////                {
                //////                    sqlText += " and  ve.IsActive=1";
                //////                    sqlText += " and  ve.JoinDate>=@PeriodStart";
                //////                    sqlText += " and  ve.JoinDate<=@PeriodEnd";
                //////                }
                //////                else if (EmpType.ToLower() == "regular")
                //////                {
                //////                    sqlText += " and  ve.IsActive=1";
                //////                    sqlText += " and  ve.JoinDate<@PeriodStart";
                //////                }
                //////                else if (EmpType.ToLower() == "left")
                //////                {
                //////                    sqlText += " and  ve.IsActive=0";
                //////                    sqlText += " and  ve.LeftDate>=@PeriodStart";
                //////                    sqlText += " and  ve.LeftDate<=@PeriodEnd";
                //////                }


                //////                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                //////                cmd.Parameters.AddWithValue("@PeriodStart", PeriodStart);
                //////                cmd.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
                //////                cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);

                //////                {
                //////                    if (ProjectId != "0_0")
                //////                        cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
                //////                    if (DepartmentId != "0_0")
                //////                        cmd.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                //////                    if (SectionId != "0_0")
                //////                        cmd.Parameters.AddWithValue("@SectionId", SectionId);
                //////                    if (DesignationId != "0_0")
                //////                        cmd.Parameters.AddWithValue("@DesignationId", DesignationId);
                //////                }


                //////                cmd.Parameters.AddWithValue("@EmployeeCodeFrom", EmployeeCodeFrom);
                //////                cmd.Parameters.AddWithValue("@EmployeeCodeTo", EmployeeCodeTo);

                //////                using (SqlDataReader dr = cmd.ExecuteReader())
                //////                {
                //////                    while (dr.Read())
                //////                    {
                //////                        employeeVm = new EmployeeInfoVM();
                //////                        employeeVm.Id = dr["Id"].ToString();
                //////                        employeeVm.ProjectId = dr["ProjectId"].ToString();
                //////                        employeeVm.DepartmentId = dr["DepartmentId"].ToString();
                //////                        employeeVm.SectionId = dr["SectionId"].ToString();
                //////                        employeeVm.DesignationId = dr["DesignationId"].ToString();
                //////                        employeeVm.GradeId = dr["GradeId"].ToString();
                //////                        employeeVm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                //////                        employeeVm.IsPermanent = Convert.ToBoolean(dr["IsPermanent"]);
                //////                        employeeVm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                //////                        employeeVm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                //////                        employeeVms.Add(employeeVm);
                //////                    }
                //////                    dr.Close();
                //////                }

                #endregion

                #region SqlText
                var sqlTextSalaryPFDetail = "";
                sqlTextSalaryPFDetail = @"Insert Into SalaryPFDetail
                        (
                        FiscalYearDetailId,PFStructureId,ProjectId,DepartmentId,SectionId,DesignationId,EmployeeId,PFValue,GrossSalary
                        ,BasicSalary,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,EmployeeStatus,GradeId
                        ) Values (
                        @FiscalYearDetailId,@PFStructureId,@ProjectId,@DepartmentId,@SectionId,@DesignationId,@EmployeeId,@PFValue,@GrossSalary
                        ,@BasicSalary,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@EmployeeStatus,@GradeId
                        ) SELECT SCOPE_IDENTITY()";

                var sqlTextSalaryPFDetailEmployeer = "";
                sqlTextSalaryPFDetailEmployeer = @"Insert Into SalaryPFDetailEmployeer
                        (
                        FiscalYearDetailId,SalaryPFDetailId,PFStructureId,ProjectId,DesignationId,DepartmentId,SectionId
                        ,EmployeeId,PFValue,GrossSalary,BasicSalary,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,EmployeeStatus,GradeId
                        ) Values (
                        @FiscalYearDetailId,@SalaryPFDetailId,@PFStructureId,@ProjectId,@DesignationId,@DepartmentId,@SectionId
                        ,@EmployeeId,@PFValue,@GrossSalary,@BasicSalary,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@EmployeeStatus,@GradeId
                        )";
                #endregion

                SqlCommand cmdempBonusDet;
                if (employeeVms.Count > 0)
                {

                    foreach (EmployeeInfoVM eployee in employeeVms)
                    {
                         
                            string[] cFields = { "ma.EmployeeId", "ma.FiscalYearDetailId" };
                            string[] cValues = { eployee.Id, FiscalYearDetailId.ToString() };
                            monthlyAttendanceVM = _monthlyAttendanceDAL.SelectAll(0, cFields, cValues, currConn, transaction).FirstOrDefault();

                         

                        #region Variables

                        bool havePF = false;
                        decimal PFValue = 0;
                        bool IsFixed = false;
                        string PortionSalaryType = "BASIC";
                        string PFStructureId = "0";
                        int spfdId = 0;

                        #endregion

                        #region Delete ExistingSalaryPFDetails
                        sqlText = @"Delete SalaryPFDetail ";
                        sqlText += " where 1=1 and EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId";

                        SqlCommand cmdDeletePFDetail = new SqlCommand(sqlText, currConn, transaction);
                        cmdDeletePFDetail.Parameters.AddWithValue("@EmployeeId", eployee.Id);
                        cmdDeletePFDetail.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                        cmdDeletePFDetail.ExecuteNonQuery();

                        #endregion

                        #region Delete ExistingSalaryPFDetailEmployer
                        sqlText = @" Delete SalaryPFDetailEmployeer ";
                        sqlText += " where 1=1 and EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId";

                        SqlCommand cmdDeletePFDetailEmployer = new SqlCommand(sqlText, currConn, transaction);
                        cmdDeletePFDetailEmployer.Parameters.AddWithValue("@EmployeeId", eployee.Id);
                        cmdDeletePFDetailEmployer.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                        cmdDeletePFDetailEmployer.ExecuteNonQuery();

                        #endregion

                        #region EmployeePF
                        sqlText = @"select * from EmployeePF
                            where EmployeeId=@EmployeeId";
                        SqlCommand cmdepf = new SqlCommand(sqlText, currConn, transaction);
                        cmdepf.Parameters.AddWithValue("@EmployeeId", eployee.Id);
                        using (SqlDataReader drepf = cmdepf.ExecuteReader())
                        {
                            while (drepf.Read())
                            {
                                havePF = false;
                                PFValue = 0;
                                IsFixed = false;
                                PortionSalaryType = "BASIC";
                                PFStructureId = "0";

                                PFValue = Convert.ToDecimal(drepf["PFValue"]);
                                IsFixed = Convert.ToBoolean(drepf["IsFixed"]);
                                PortionSalaryType = drepf["PortionSalaryType"].ToString();
                                PFStructureId = drepf["PFStructureId"].ToString();
                                havePF = true;
                            }
                            drepf.Close();
                        }
                        #endregion EmployeePF

                        if (havePF && eployee.IsPermanent && eployee.IsPFApplicable)
                        {
                            #region Calculate PFValue

                            if (!IsFixed)
                            {
                                PFValue = PFValue / 100;
                                if (PortionSalaryType.ToUpper().Trim() == "GROSS")
                                {
                                    PFValue = PFValue * eployee.GrossSalary;
                                }
                                else
                                {
                                    PFValue = PFValue * eployee.BasicSalary;
                                }
                            }

                           
                                //if (monthlyAttendanceVM.NPDay > 0)
                                //{
                                //    PFValue = PFValue - (PFValue * monthlyAttendanceVM.NPDay / monthlyAttendanceVM.DOM);// SSD.Amount - (amnt / monthlyAttendanceVM.DOM * (monthlyAttendanceVM.NPDay));
                                //}
                                PFValue = Math.Round(PFValue, MidpointRounding.AwayFromZero);
                             
                            #endregion

                            #region Insert PF Employee

                            cmdempBonusDet = new SqlCommand(sqlTextSalaryPFDetail, currConn, transaction);
                            cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                            cmdempBonusDet.Parameters.AddWithValue("@PFStructureId", PFStructureId);
                            cmdempBonusDet.Parameters.AddWithValue("@ProjectId", eployee.ProjectId);
                            cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", eployee.DepartmentId);
                            cmdempBonusDet.Parameters.AddWithValue("@SectionId", eployee.SectionId);
                            cmdempBonusDet.Parameters.AddWithValue("@DesignationId", eployee.DesignationId);
                            cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", eployee.Id);
                            cmdempBonusDet.Parameters.AddWithValue("@PFValue", PFValue);
                            cmdempBonusDet.Parameters.AddWithValue("@GrossSalary", eployee.GrossSalary);
                            cmdempBonusDet.Parameters.AddWithValue("@BasicSalary", eployee.BasicSalary);
                            cmdempBonusDet.Parameters.AddWithValue("@Remarks", "-");
                            cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                            cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                            cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                            cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                            cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                            cmdempBonusDet.Parameters.AddWithValue("@EmployeeStatus", EmpType);
                            cmdempBonusDet.Parameters.AddWithValue("@GradeId", eployee.GradeId);

                            var exeRes = cmdempBonusDet.ExecuteScalar();
                            spfdId = Convert.ToInt32(exeRes);

                            #endregion

                            #region Insert PF Employer

                            cmdempBonusDet = new SqlCommand(sqlTextSalaryPFDetailEmployeer, currConn, transaction);
                            cmdempBonusDet.Parameters.AddWithValue("@SalaryPFDetailId", spfdId);
                            cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                            cmdempBonusDet.Parameters.AddWithValue("@PFStructureId", PFStructureId);
                            cmdempBonusDet.Parameters.AddWithValue("@ProjectId", eployee.ProjectId);
                            cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", eployee.DepartmentId);
                            cmdempBonusDet.Parameters.AddWithValue("@SectionId", eployee.SectionId);
                            cmdempBonusDet.Parameters.AddWithValue("@DesignationId", eployee.DesignationId);
                            cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", eployee.Id);
                            cmdempBonusDet.Parameters.AddWithValue("@PFValue", PFValue);
                            cmdempBonusDet.Parameters.AddWithValue("@GrossSalary", eployee.GrossSalary);
                            cmdempBonusDet.Parameters.AddWithValue("@BasicSalary", eployee.BasicSalary);
                            cmdempBonusDet.Parameters.AddWithValue("@Remarks", "-");
                            cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                            cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                            cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                            cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                            cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                            cmdempBonusDet.Parameters.AddWithValue("@EmployeeStatus", EmpType);
                            cmdempBonusDet.Parameters.AddWithValue("@GradeId", eployee.GradeId);
                            cmdempBonusDet.ExecuteNonQuery();

                            #endregion

                        }
                    }

                    retResults[1] = employeeVms.Count.ToString() + " Employee PF Psocess Successed.";
                }
                else
                {
                    retResults[1] = "There have no employee to process PF.";
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


        #region Comments

        ////        public string[] InsertSalaryPFBackup(int FiscalYearDetailsId, string ProjectId, string DepartmentId, string SectionId, FiscalYearVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        ////        {

        ////            #region Initializ
        ////            string sqlText = "";
        ////            int Id = 0;
        ////            string[] retResults = new string[6];
        ////            retResults[0] = "Fail";//Success or Fail
        ////            retResults[1] = "Fail";// Success or Fail Message
        ////            retResults[2] = Id.ToString();// Return Id
        ////            retResults[3] = sqlText; //  SQL Query
        ////            retResults[4] = "ex"; //catch ex
        ////            retResults[5] = "Employee Salary PF Process"; //Method Name


        ////            #endregion
        ////            SqlConnection currConn = null;
        ////            SqlTransaction transaction = null;
        ////            #region Try

        ////            try
        ////            {

        ////                #region open connection and transaction
        ////                #region New open connection and transaction
        ////                if (VcurrConn != null)
        ////                {
        ////                    currConn = VcurrConn;
        ////                }

        ////                if (Vtransaction != null)
        ////                {
        ////                    transaction = Vtransaction;
        ////                }

        ////                #endregion New open connection and transaction

        ////                if (currConn == null)
        ////                {
        ////                    currConn = _dbsqlConnection.GetConnection();
        ////                    if (currConn.State != ConnectionState.Open)
        ////                    {
        ////                        currConn.Open();
        ////                    }
        ////                }


        ////                if (transaction == null)
        ////                {
        ////                    transaction = currConn.BeginTransaction("");
        ////                }

        ////                #endregion open connection and transaction

        ////                string PeriodEnd = "";
        ////                string NoAssignCode = "";

        ////                #region Fiscal Year Last Date
        ////                sqlText = @"select PeriodEnd from FiscalYearDetail
        ////                            where id=@FiscalYearDetailsId";
        ////                SqlCommand cmdfy = new SqlCommand(sqlText, currConn, transaction);
        ////                cmdfy.Parameters.AddWithValue("@FiscalYearDetailsId", FiscalYearDetailsId);
        ////                using (SqlDataReader dr = cmdfy.ExecuteReader())
        ////                {
        ////                    while (dr.Read())
        ////                    {
        ////                        PeriodEnd = dr["PeriodEnd"].ToString();
        ////                    }
        ////                    dr.Close();
        ////                }
        ////                #endregion Fiscal Year Last Date
        ////                #region Employee No Job Assign Check
        ////                sqlText = @"select   isnull(Stuff((SELECT ', ' + Code 
        ////                    FROM ViewEmployeeInformation 
        ////                    where id not in (select EmployeeId from EmployeeJob)
        ////                    and  IsArchive=0 and isactive=1
        ////                    and BranchId=@BranchId
        ////                    and JoinDate<=@PeriodEnd
        ////                    FOR XML PATH('')),1,1,''),'NA')  Code";
        ////                SqlCommand cmdnja = new SqlCommand(sqlText, currConn, transaction);
        ////                cmdnja.Parameters.AddWithValue("@BranchId", vm.BranchId);
        ////                cmdnja.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
        ////                using (SqlDataReader dr = cmdnja.ExecuteReader())
        ////                {
        ////                    while (dr.Read())
        ////                    {
        ////                        NoAssignCode = dr["Code"].ToString();
        ////                    }
        ////                    dr.Close();
        ////                }
        ////                if (!string.IsNullOrWhiteSpace(NoAssignCode) && NoAssignCode != "NA")
        ////                {
        ////                    //retResults[1] = "This Employee have not assigh JOB yet, Code : " + NoAssignCode;
        ////                    //retResults[3] = sqlText;
        ////                    //throw new ArgumentNullException("This Employee have not assigh JOB yet. Code : " + NoAssignCode, "");
        ////                }
        ////                #endregion Employee No Job Assign Check
        ////                #region Employee No Salary Structure Assign Check
        ////                sqlText = @"select   isnull(Stuff((SELECT ', ' + Code 
        ////                FROM ViewEmployeeInformation 
        ////                where id not in (select EmployeeId from EmployeeSalaryStructure)
        ////                and  IsArchive=0 and isactive=1
        ////                and BranchId=@BranchId
        ////                and JoinDate<=@PeriodEnd
        ////                FOR XML PATH('')),1,1,''),'NA')  Code";
        ////                SqlCommand cmdnss = new SqlCommand(sqlText, currConn, transaction);
        ////                cmdnss.Parameters.AddWithValue("@BranchId", vm.BranchId);
        ////                cmdnss.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
        ////                NoAssignCode = "";
        ////                using (SqlDataReader dr = cmdnss.ExecuteReader())
        ////                {
        ////                    while (dr.Read())
        ////                    {
        ////                        NoAssignCode = dr["Code"].ToString();
        ////                    }
        ////                    dr.Close();
        ////                }
        ////                if (!string.IsNullOrWhiteSpace(NoAssignCode) && NoAssignCode != "NA")
        ////                {
        ////                    //retResults[1] = "This Employee have not assigh Salary Structure yet, Code : " + NoAssignCode;
        ////                    //retResults[3] = sqlText;
        ////                    //throw new ArgumentNullException("This Employee have not assigh Salary Structure yet, Code : " + NoAssignCode, "");
        ////                }
        ////                #endregion Employee No Salary Structure Assign Check

        ////                string SalaryPFID = "-";
        ////                #region SalaryPFID retrive
        ////                sqlText = "Select Top 1 Id from SalaryPF where FiscalYearDetailId=@FiscalYearDetailId and BranchId=@BranchId";
        ////                SqlCommand cmdspf = new SqlCommand(sqlText, currConn, transaction);
        ////                cmdspf.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
        ////                cmdspf.Parameters.AddWithValue("@BranchId", vm.BranchId);
        ////                using (SqlDataReader dr = cmdspf.ExecuteReader())
        ////                {
        ////                    while (dr.Read())
        ////                    {
        ////                        SalaryPFID = dr["Id"].ToString();
        ////                    }
        ////                    dr.Close();
        ////                }
        ////                if (SalaryPFID == "-")
        ////                {
        ////                    sqlText = "Select  isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0)id  from SalaryPF where BranchId=@BranchId";
        ////                    SqlCommand cmdempspfHCount = new SqlCommand(sqlText, currConn, transaction);
        ////                    cmdempspfHCount.Parameters.AddWithValue("@BranchId", vm.BranchId);
        ////                    var tt = cmdempspfHCount.ExecuteScalar();
        ////                    int count2 = Convert.ToInt32(tt);// (int)cmdempspfHCount.ExecuteScalar();

        ////                    SalaryPFID = vm.BranchId.ToString() + "_" + (count2 + 1);
        ////                    sqlText = @" Insert Into SalaryPF
        ////                                    (
        ////                                     Id,BranchId,FiscalYearDetailId,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
        ////                                    ) VALUES (
        ////                                     @Id,@BranchId,@FiscalYearDetailId,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
        ////                                    )
        ////                                    ";

        ////                    SqlCommand employeespfHIn = new SqlCommand(sqlText, currConn, transaction);
        ////                    employeespfHIn.Parameters.AddWithValue("@Id", SalaryPFID);
        ////                    employeespfHIn.Parameters.AddWithValue("@BranchId", vm.BranchId);
        ////                    employeespfHIn.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
        ////                    employeespfHIn.Parameters.AddWithValue("@Remarks", "-");
        ////                    employeespfHIn.Parameters.AddWithValue("@IsActive", true);
        ////                    employeespfHIn.Parameters.AddWithValue("@IsArchive", false);
        ////                    employeespfHIn.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
        ////                    employeespfHIn.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
        ////                    employeespfHIn.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

        ////                    employeespfHIn.ExecuteNonQuery();
        ////                }

        ////                #endregion

        ////                #region Delete ExistingSalaryPFDetails
        ////                sqlText = @"Delete SalaryPFDetail ";
        ////                sqlText += " where SalaryPFId=@SalaryPFId";

        ////                if (ProjectId != "0_0")
        ////                {
        ////                    sqlText += " and ProjectId=@ProjectId";
        ////                }
        ////                if (DepartmentId != "0_0")
        ////                {
        ////                    sqlText += " and DepartmentId=@DepartmentId";
        ////                }
        ////                if (SectionId != "0_0")
        ////                {
        ////                    sqlText += " and SectionId=@SectionId";
        ////                }

        ////                SqlCommand cmdDeletePrevious = new SqlCommand(sqlText, currConn, transaction);
        ////                cmdDeletePrevious.Parameters.AddWithValue("@SalaryPFId", SalaryPFID);
        ////                if (ProjectId != "0_0")
        ////                {
        ////                    cmdDeletePrevious.Parameters.AddWithValue("@ProjectId", ProjectId);
        ////                }
        ////                if (DepartmentId != "0_0")
        ////                {
        ////                    cmdDeletePrevious.Parameters.AddWithValue("@DepartmentId", DepartmentId);
        ////                }
        ////                if (SectionId != "0_0")
        ////                {
        ////                    cmdDeletePrevious.Parameters.AddWithValue("@SectionId", SectionId);
        ////                }
        ////                cmdDeletePrevious.ExecuteNonQuery();

        ////                #endregion
        ////                #region Delete ExistingSalaryPFEmployeerDetails
        ////                sqlText = @"Delete SalaryPFDetailEmployeer ";
        ////                sqlText += " where SalaryPFId=@SalaryPFId";

        ////                if (ProjectId != "0_0")
        ////                {
        ////                    sqlText += " and ProjectId=@ProjectId";
        ////                }
        ////                if (DepartmentId != "0_0")
        ////                {
        ////                    sqlText += " and DepartmentId=@DepartmentId";
        ////                }
        ////                if (SectionId != "0_0")
        ////                {
        ////                    sqlText += " and SectionId=@SectionId";
        ////                }

        ////                SqlCommand cmdDeleteSPFEmpPrevious = new SqlCommand(sqlText, currConn, transaction);
        ////                cmdDeleteSPFEmpPrevious.Parameters.AddWithValue("@SalaryPFId", SalaryPFID);
        ////                if (ProjectId != "0_0")
        ////                {
        ////                    cmdDeleteSPFEmpPrevious.Parameters.AddWithValue("@ProjectId", ProjectId);
        ////                }
        ////                if (DepartmentId != "0_0")
        ////                {
        ////                    cmdDeleteSPFEmpPrevious.Parameters.AddWithValue("@DepartmentId", DepartmentId);
        ////                }
        ////                if (SectionId != "0_0")
        ////                {
        ////                    cmdDeleteSPFEmpPrevious.Parameters.AddWithValue("@SectionId", SectionId);
        ////                }
        ////                cmdDeleteSPFEmpPrevious.ExecuteNonQuery();

        ////                #endregion

        ////                List<EmployeeInfoVM> employeeVms = new List<EmployeeInfoVM>();
        ////                EmployeeInfoVM employeeVm;
        ////                sqlText = @"
        ////    select
        ////    pf.EmployeeId,pf.PFStructureId,pf.PFValue,pf.IsFixed,isnull(nullif(pf.PortionSalaryType,''),'NA')PortionSalaryType
        ////    ,e.ProjectId,e.DepartmentId,e.SectionId,e.DesignationId,e.GrossSalary,e.BasicSalary
        ////      from EmployeePF pf
        ////    left outer join 
        ////    ViewEmployeeInformation e on pf.EmployeeId=e.Id
        ////    where pf.IsArchive=0 and pf.isactive=1
        ////    and  e.IsArchive=0 and e.isactive=1
        ////    and e.JoinDate <=@PeriodEnd
        ////    and e.BranchId = @BranchId
        ////
        //// ";
        ////                if (ProjectId != "0_0")
        ////                    sqlText += " and e.ProjectId=@ProjectId";
        ////                if (DepartmentId != "0_0")
        ////                    sqlText += " and e.DepartmentId=@DepartmentId";
        ////                if (SectionId != "0_0")
        ////                    sqlText += " and e.SectionId=@SectionId";
        ////                sqlText += " order by e.Id";
        ////                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
        ////                cmd.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
        ////                cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);

        ////                if (ProjectId != "0_0")
        ////                    cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
        ////                if (DepartmentId != "0_0")
        ////                    cmd.Parameters.AddWithValue("@DepartmentId", DepartmentId);
        ////                if (SectionId != "0_0")
        ////                    cmd.Parameters.AddWithValue("@SectionId", SectionId);

        ////                using (SqlDataReader dr = cmd.ExecuteReader())
        ////                {
        ////                    while (dr.Read())
        ////                    {
        ////                        employeeVm = new EmployeeInfoVM();
        ////                        employeeVm.Id = dr["EmployeeId"].ToString();
        ////                        employeeVm.ProjectId = dr["ProjectId"].ToString();
        ////                        employeeVm.DepartmentId = dr["DepartmentId"].ToString();
        ////                        employeeVm.SectionId = dr["SectionId"].ToString();
        ////                        employeeVm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
        ////                        employeeVm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);

        ////                        employeeVm.PFValue = Convert.ToDecimal(dr["PFValue"]);
        ////                        employeeVm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
        ////                        employeeVm.PortionSalaryType = dr["PortionSalaryType"].ToString();
        ////                        employeeVm.PFStructureId = dr["PFStructureId"].ToString();
        ////                        employeeVms.Add(employeeVm);
        ////                    }
        ////                    dr.Close();
        ////                }

        ////                sqlText = @"Insert Into SalaryPFDetail
        ////                        (
        ////                        SalaryPFId,FiscalYearDetailId,PFStructureId,ProjectId,DepartmentId,SectionId,EmployeeId,PFValue,GrossSalary
        ////                        ,BasicSalary,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
        ////                        ) Values (
        ////                        @SalaryPFId,@FiscalYearDetailId,@PFStructureId,@ProjectId,@DepartmentId,@SectionId,@EmployeeId,@PFValue,@GrossSalary
        ////                        ,@BasicSalary,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
        ////                        ) SELECT SCOPE_IDENTITY()";
        ////                var sqlText2 = "";
        ////                sqlText2 = @"Insert Into SalaryPFDetailEmployeer
        ////                        (
        ////                        SalaryPFId,FiscalYearDetailId,SalaryPFDetailId,PFStructureId,ProjectId,DepartmentId,SectionId
        ////                        ,EmployeeId,PFValue,GrossSalary,BasicSalary,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
        ////                        ) Values (
        ////                        @SalaryPFId,@FiscalYearDetailId,@SalaryPFDetailId,@PFStructureId,@ProjectId,@DepartmentId,@SectionId
        ////                        ,@EmployeeId,@PFValue,@GrossSalary,@BasicSalary,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
        ////                        )";
        ////                SqlCommand cmdempBonusDet;
        ////                if (employeeVms.Count > 0)
        ////                {
        ////                    decimal PFValue = 0;
        ////                    foreach (EmployeeInfoVM item in employeeVms)
        ////                    {
        ////                        PFValue = item.PFValue;
        ////                        if (!item.IsFixed)
        ////                        {
        ////                            PFValue = PFValue / 100;
        ////                            if (item.PortionSalaryType.ToUpper().Trim() == "GROSS")
        ////                            {
        ////                                PFValue = PFValue * item.GrossSalary;
        ////                            }
        ////                            else
        ////                            {
        ////                                PFValue = PFValue * item.BasicSalary;
        ////                            }
        ////                        }


        ////                        cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@SalaryPFId", SalaryPFID);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@PFStructureId", item.PFStructureId);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@ProjectId", ProjectId);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", DepartmentId);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@SectionId", SectionId);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", item.Id);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@PFValue", PFValue);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@GrossSalary", item.GrossSalary);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@BasicSalary", item.BasicSalary);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@Remarks", "-");
        ////                        cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
        ////                        var exeRes = cmdempBonusDet.ExecuteScalar();
        ////                        int spfdId = Convert.ToInt32(exeRes);

        ////                        cmdempBonusDet = new SqlCommand(sqlText2, currConn, transaction);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@SalaryPFId", SalaryPFID);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@SalaryPFDetailId", spfdId);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailsId);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@PFStructureId", item.PFStructureId);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@ProjectId", ProjectId);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", DepartmentId);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@SectionId", SectionId);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", item.Id);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@PFValue", PFValue);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@GrossSalary", item.GrossSalary);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@BasicSalary", item.BasicSalary);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@Remarks", "-");
        ////                        cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
        ////                        cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
        ////                        cmdempBonusDet.ExecuteNonQuery();
        ////                    }

        ////                    retResults[1] = employeeVms.Count.ToString() + " Employee PF Psocess Successed.";
        ////                }
        ////                else
        ////                {
        ////                    retResults[1] = "There have no employee to process PF.";
        ////                }

        ////                #region Save

        ////                #endregion Save


        ////                #region Commit
        ////                if (Vtransaction == null)
        ////                {
        ////                    if (transaction != null)
        ////                    {
        ////                        transaction.Commit();
        ////                    }
        ////                }
        ////                #endregion Commit

        ////                #region SuccessResult

        ////                retResults[0] = "Success";

        ////                #endregion SuccessResult

        ////            }

        ////            #endregion try

        ////            #region Catch and Finall



        ////            catch (Exception ex)
        ////            {
        ////                retResults[0] = "Fail";//Success or Fail
        ////                retResults[4] = ex.Message.ToString(); //catch ex
        ////                if (Vtransaction == null)
        ////                {
        ////                    if (transaction != null)
        ////                    {
        ////                        if (Vtransaction == null) { transaction.Rollback(); }
        ////                    }
        ////                }
        ////                return retResults;
        ////            }

        ////            finally
        ////            {
        ////                if (VcurrConn == null)
        ////                {
        ////                    if (currConn != null)
        ////                    {
        ////                        if (currConn.State == ConnectionState.Open)
        ////                        {
        ////                            currConn.Close();
        ////                        }
        ////                    }
        ////                }

        ////            }


        ////            #endregion

        ////            #region Results

        ////            return retResults;
        ////            #endregion


        ////        }

        #endregion




        public string[] SalaryPFSingleAddorUpdate(SalaryPFDetailVM vm, int branchId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Employee Salary PF Process Single"; //Method Name


            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            System.Data.DataTable dsSS = new System.Data.DataTable();
            string EmployeeStatus = "";
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


                sqlText = @"select * from SalaryPFDetail ";
                sqlText += " where FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " and EmployeeId=@EmployeeId";

                SqlCommand objss = new SqlCommand();
                objss.Connection = currConn;
                objss.CommandText = sqlText;
                objss.CommandType = CommandType.Text;
                //objss.Parameters.AddWithValue("@Id", SalaryStructureId);
                objss.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                objss.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                objss.Transaction = transaction;

                SqlDataAdapter daSS = new SqlDataAdapter(objss);
                daSS.Fill(dsSS);
                foreach (DataRow item in dsSS.Rows)
                {
                    EmployeeStatus = item["EmployeeStatus"].ToString();
                }

                sqlText = @"Delete SalaryPFDetail ";
                sqlText += " where FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " and EmployeeId=@EmployeeId";
                SqlCommand cmdDeletePrevious = new SqlCommand(sqlText, currConn, transaction);
                cmdDeletePrevious.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                cmdDeletePrevious.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdDeletePrevious.ExecuteNonQuery();
                sqlText = @"Delete SalaryPFDetailEmployeer ";
                sqlText += " where FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " and EmployeeId=@EmployeeId";

                SqlCommand cmdDeletePrevious2 = new SqlCommand(sqlText, currConn, transaction);
                cmdDeletePrevious2.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                cmdDeletePrevious2.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdDeletePrevious2.ExecuteNonQuery();
                EmployeeInfoVM employeeVm = null;
                sqlText = @"
SELECT Top 1
e.Id,pf.EmployeeId,pf.PFStructureId,pf.PFValue,pf.IsFixed,isnull(nullif(pf.PortionSalaryType,''),'NA')PortionSalaryType
,e.ProjectId,e.DepartmentId,e.SectionId,e.DesignationId,e.GradeId,e.GrossSalary,e.BasicSalary
  from EmployeePF pf
left outer join 
ViewEmployeeInformation e on pf.EmployeeId=e.Id
where 1=1 
------and pf.IsArchive=0 and pf.isactive=1
------and  e.IsArchive=0 and e.isactive=1
and e.id=@EmployeeID";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@EmployeeID", vm.EmployeeId);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        employeeVm = new EmployeeInfoVM();
                        employeeVm.Id = dr["Id"].ToString();
                        employeeVm.ProjectId = dr["ProjectId"].ToString();
                        employeeVm.DepartmentId = dr["DepartmentId"].ToString();
                        employeeVm.DesignationId = dr["DesignationId"].ToString();
                        employeeVm.GradeId = dr["GradeId"].ToString();
                        employeeVm.SectionId = dr["SectionId"].ToString();
                        employeeVm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        employeeVm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                        employeeVm.PFValue = Convert.ToDecimal(dr["PFValue"]);
                        employeeVm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                        employeeVm.PortionSalaryType = dr["PortionSalaryType"].ToString();
                        employeeVm.PFStructureId = dr["PFStructureId"].ToString();
                    }
                    dr.Close();
                }
                sqlText = @"Insert Into SalaryPFDetail
(
 FiscalYearDetailId
,PFStructureId
,ProjectId
,DepartmentId
,DesignationId
,SectionId
,EmployeeId
,GradeId
,PFValue
,GrossSalary
,BasicSalary
,Remarks
,EmployeeStatus
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) Values (
@FiscalYearDetailId
,@PFStructureId
,@ProjectId
,@DepartmentId
,@DesignationId
,@SectionId
,@EmployeeId
,@GradeId
,@PFValue
,@GrossSalary
,@BasicSalary
,@Remarks
,@EmployeeStatus
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) SELECT SCOPE_IDENTITY()";
                var sqlText2 = "";
                sqlText2 = @"Insert Into SalaryPFDetailEmployeer
(
FiscalYearDetailId
,SalaryPFDetailId
,PFStructureId
,ProjectId
,DepartmentId
,DesignationId
,SectionId
,EmployeeId
,GradeId
,PFValue
,GrossSalary
,BasicSalary
,Remarks
,EmployeeStatus
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) Values (
@FiscalYearDetailId
,@SalaryPFDetailId
,@PFStructureId
,@ProjectId
,@DepartmentId
,@DesignationId
,@SectionId
,@EmployeeId
,@GradeId
,@PFValue
,@GrossSalary
,@BasicSalary
,@Remarks
,@EmployeeStatus
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)";
                SqlCommand cmdempBonusDet;
                if (employeeVm != null)
                {
                    decimal PFValue = 0;
                    PFValue = vm.PFValue;
                    cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                    cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdempBonusDet.Parameters.AddWithValue("@PFStructureId", employeeVm.PFStructureId);
                    cmdempBonusDet.Parameters.AddWithValue("@ProjectId", employeeVm.ProjectId);
                    cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", employeeVm.DepartmentId);
                    cmdempBonusDet.Parameters.AddWithValue("@SectionId", employeeVm.SectionId);
                    cmdempBonusDet.Parameters.AddWithValue("@DesignationId", employeeVm.DesignationId);
                    cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", employeeVm.Id);
                    cmdempBonusDet.Parameters.AddWithValue("@GradeId", employeeVm.GradeId);
                    cmdempBonusDet.Parameters.AddWithValue("@EmployeeStatus", EmployeeStatus);
                    cmdempBonusDet.Parameters.AddWithValue("@PFValue", PFValue);
                    cmdempBonusDet.Parameters.AddWithValue("@GrossSalary", employeeVm.GrossSalary);
                    cmdempBonusDet.Parameters.AddWithValue("@BasicSalary", employeeVm.BasicSalary);
                    cmdempBonusDet.Parameters.AddWithValue("@Remarks", "-");
                    cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                    cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                    cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdempBonusDet.ExecuteScalar();
                    int spfDId = Convert.ToInt32(exeRes);
                    cmdempBonusDet = new SqlCommand(sqlText2, currConn, transaction);
                    cmdempBonusDet.Parameters.AddWithValue("@SalaryPFDetailId", spfDId);
                    cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdempBonusDet.Parameters.AddWithValue("@PFStructureId", employeeVm.PFStructureId);
                    cmdempBonusDet.Parameters.AddWithValue("@ProjectId", employeeVm.ProjectId);
                    cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", employeeVm.DepartmentId);
                    cmdempBonusDet.Parameters.AddWithValue("@DesignationId", employeeVm.DesignationId);
                    cmdempBonusDet.Parameters.AddWithValue("@SectionId", employeeVm.SectionId);
                    cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", employeeVm.Id);
                    cmdempBonusDet.Parameters.AddWithValue("@GradeId", employeeVm.GradeId);
                    cmdempBonusDet.Parameters.AddWithValue("@EmployeeStatus", EmployeeStatus);
                    cmdempBonusDet.Parameters.AddWithValue("@PFValue", PFValue);
                    cmdempBonusDet.Parameters.AddWithValue("@GrossSalary", employeeVm.GrossSalary);
                    cmdempBonusDet.Parameters.AddWithValue("@BasicSalary", employeeVm.BasicSalary);
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
                    retResults[1] = "Have no structure.";
                    throw new ArgumentException("Have no structure", "Have no structure");
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
        public List<SalaryPFVM> SelectAll(int BranchId, int? fid)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryPFVM> VMs = new List<SalaryPFVM>();
            SalaryPFVM vm;
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
fyd.PeriodName,
spfd.FiscalYearDetailId
,spfd.PFValue
,spfd.Remarks
,vw.Department
,vw.Project
,vw.Designation
,vw.Section
,vw.EmpName
,vw.BasicSalary
,vw.GrossSalary
,vw.JoinDate
,spfd.DepartmentId
,spfd.DesignationId
,spfd.ProjectId
,spfd.SectionId
,vw.Code
from SalaryPFDetail spfd 
left outer join FiscalYearDetail fyd on spfd.FiscalYearDetailId =fyd.Id
left outer join ViewEmployeeInformation vw on spfd.EmployeeId=vw.id
Where spfd.IsArchive=0 And spfd.IsActive=1  and spfd.PFValue >0";
                if (fid != null && fid != 0)
                {
                    sqlText += @" and spfd.FiscalYearDetailId='" + fid + "'";
                }
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryPFVM();
                    vm.PeriodName = dr["PeriodName"].ToString();

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.PFValue = Convert.ToDecimal(dr["PFValue"].ToString());
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
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

        public List<SalaryPFVM> GetPeriodNameDistrinct()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryPFVM> VMs = new List<SalaryPFVM>();
            SalaryPFVM vm;
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
DISTINCT fyd.PeriodName,
spfd.FiscalYearDetailId,
fyd.PeriodStart,
spfd.Remarks
from ViewSalaryPFDetail spfd 
left outer join FiscalYearDetail fyd on spfd.FiscalYearDetailId =fyd.Id
Where spfd.IsArchive=0 And spfd.IsActive=1 and spfd.PFValue >=0
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
                    vm = new SalaryPFVM();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
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
        public string[] SalaryPFDetailsDelete(string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryPFD"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "Delete SalaryPFDetail where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        sqlText = "";
                        sqlText = "Delete SalaryPFDetailEmployeer where SalaryPFDetailId=@Id";
                        SqlCommand cmdUpdate2 = new SqlCommand(sqlText, currConn);
                        cmdUpdate2.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate2.Transaction = transaction;
                        exeRes = cmdUpdate2.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }




                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException("Salary PF Delete", vm.Id + " could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary PF Details Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary PF Details.";
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
        public string[] SalaryPFDelete(string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSalaryPF"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryPF"); }

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
                        sqlText = "Delete SalaryPF where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        #endregion

                        #region Details
                        sqlText = "";
                        sqlText = "Delete SalaryPFDetail where SalaryPFId=@Id";
                        SqlCommand cmdUpdate2 = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate2.Parameters.AddWithValue("@Id", Ids[i]);
                        exeRes = cmdUpdate2.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        sqlText = "";
                        sqlText = "Delete SalaryPFDetailEmployeer where SalaryPFId=@Id";
                        SqlCommand cmdUpdate3 = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate3.Parameters.AddWithValue("@Id", Ids[i]);
                        exeRes = cmdUpdate3.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        #endregion
                    }




                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException("Salary PF Delete"," could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary PF Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary PF.";
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
        public List<SalaryPFDetailVM> SelectAllSalaryPFDetails(int FId, SqlConnection currConn, bool callFromOutSide)
        {

            #region Variables

            string sqlText = "";
            List<SalaryPFDetailVM> VMs = new List<SalaryPFDetailVM>();
            SalaryPFDetailVM vm;
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
 Id
,EmpName
,code
,FiscalYearDetailId
,EmployeeId
,GrossSalary
,BasicSalary
,PFValue
,Remarks
from ViewSalaryPFDetail
Where 1=1 and FiscalYearDetailId=@FiscalYearDetailId and IsArchive=0
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@FiscalYearDetailId", FId);

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new SalaryPFDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.EmpName = dr["EmpName"].ToString();
                        vm.Code = dr["Code"].ToString();
                        vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                        vm.PFValue = Convert.ToDecimal(dr["PFValue"]);
                        vm.Remarks = dr["Remarks"].ToString();
                        //vm.EmployeeName = dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
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
        public SalaryPFDetailVM GetByIdSalaryPFDetails(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SalaryPFDetailVM vm = new SalaryPFDetailVM();

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
 spfd.Id
,spfd.GrossSalary
,spfd.BasicSalary
,spfd.PFValue
,spfd.Remarks
,e.Salutation_E
,e.MiddleName
,e.LastName
,spfd.EmployeeId
,spfd.FiscalYearDetailId
 from SalaryPFDetail spfd
 left join EmployeeInfo e on e.Id=spfd.EmployeeId
 where spfd.Id=@Id
     
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
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.PFValue = Convert.ToDecimal(dr["PFValue"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();

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
        public SalaryPFDetailVM GetByIdSalaryPFbyempidandfidDetail(string empid, int fid)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SalaryPFDetailVM vm = new SalaryPFDetailVM();

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
 spfd.Id
,spfd.GrossSalary
,spfd.BasicSalary
,spfd.PFValue
,spfd.Remarks
,e.Salutation_E
,e.MiddleName
,e.LastName
,spfd.EmployeeId
,spfd.FiscalYearDetailId
 from SalaryPFDetail spfd
 left join EmployeeInfo e on e.Id=spfd.EmployeeId
 where spfd.EmployeeId=@EmployeeId and spfd.FiscalYearDetailId=@FiscalYearDetailId
     
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", empid);
                objComm.Parameters.AddWithValue("@FiscalYearDetailId", fid);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.PFValue = Convert.ToDecimal(dr["PFValue"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
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
        public string[] SalaryPFSingleEdit(SalaryPFDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Employee Salary PF Process Single Edit"; //Method Name

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

                sqlText = @"Update  SalaryPFDetail

set
 PFValue            =@PFValue
,Remarks            =@Remarks
,LastUpdateBy       =@LastUpdateBy
,LastUpdateAt       =@LastUpdateAt
,LastUpdateFrom     =@LastUpdateFrom
where Id=@Id
";
                var sqlText2 = "";
                sqlText2 = @"Update SalaryPFDetailEmployeer

set
 PFValue            =@PFValue
,Remarks            =@Remarks
,LastUpdateBy       =@LastUpdateBy
,LastUpdateAt       =@LastUpdateAt
,LastUpdateFrom     =@LastUpdateFrom
where SalaryPFDetailId=@Id
";
                SqlCommand cmdempBonusDet;
                cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                cmdempBonusDet.Parameters.AddWithValue("@Id", vm.Id);
                cmdempBonusDet.Parameters.AddWithValue("@PFValue", vm.PFValue);
                cmdempBonusDet.Parameters.AddWithValue("@Remarks", vm.Remarks);
                cmdempBonusDet.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                cmdempBonusDet.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                cmdempBonusDet.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                cmdempBonusDet.ExecuteNonQuery();

                cmdempBonusDet = new SqlCommand(sqlText2, currConn, transaction);
                cmdempBonusDet.Parameters.AddWithValue("@Id", vm.Id);
                cmdempBonusDet.Parameters.AddWithValue("@PFValue", vm.PFValue);
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
        public string GetPeriodName(string SalaryPFId)
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

                sqlText = @"select f.PeriodName from SalaryPF t 
join FiscalYearDetail f on f.id=t.FiscalYearDetailId
where t.Id=@SalaryPFId";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@SalaryPFId", SalaryPFId);
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
        public string[] ImportExcelFile(string fileName, ShampanIdentityVM auditvm, int branchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, int FYDId = 0)
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
            //PFStructureDAL pfdal = new PFStructureDAL();
            FiscalYearDetailVM FYDVM = new FiscalYearDetailVM();
            #region try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                //DataSet dt = Ordinary.UploadExcel(fileName);
                DataSet ds = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                FileStream stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read);
                // We return the interface, so that
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
                var a = "";
                SalaryPFDetailVM vm = new SalaryPFDetailVM();

                foreach (DataRow item in dt.Rows)
                {
                    vm = new SalaryPFDetailVM();
                    empVM = _dalemp.ViewSelectAllEmployee(item["EmpCode"].ToString(), null, null, null, null, null, null, currConn, transaction).FirstOrDefault();
                    if (empVM == null || empVM.Id == null)
                    {
                        throw new ArgumentNullException("Employee Code" + item["EmpCode"].ToString() + " Not in System", "Employee Code " + item["EmpCode"].ToString() + " Not in System");
                    }
                    else
                    {
                        ////FYDVM = fydal.FYPeriodDetail(Convert.ToInt32(item["FYDId"].ToString()), currConn, transaction).FirstOrDefault();
                        FYDVM = fydal.FYPeriodDetail(FYDId, currConn, transaction).FirstOrDefault();
                        if (FYDVM == null)
                        {
                            throw new ArgumentNullException("Fiscal Period" + item["FYDId"].ToString() + " Not in System", "Fiscal Period " + item["FYDId"].ToString() + " Not in System");
                        }
                        else
                        {
                            //vm.PFStructureId = pfdal.SelectById(item["PFId"].ToString()).Id;
                            //if (vm.PFStructureId == null)
                            //{
                            //    throw new ArgumentNullException("Salary Provident Fund not in System,", "Provident Fund  " + item["PFId"].ToString() + " Not in System");
                            //}
                            //else
                            //{
                                if (!Ordinary.IsNumeric(item["Amount"].ToString()))
                                {
                                    throw new ArgumentNullException("Please input the Numeric Value in Amount", "Please input the Numeric Value in Amount");
                                }
                                else
                                {
                                    vm.EmployeeId = empVM.Id;
                                    vm.FiscalYearDetailId = Convert.ToString(FYDVM.Id);
                                    vm.PFValue = Convert.ToDecimal(item["Amount"]);
                                    vm.ProjectId = empVM.ProjectId;
                                    vm.DepartmentId = empVM.DepartmentId;
                                    vm.SectionId = empVM.SectionId;
                                    vm.DesignationId = empVM.DesignationId;
                                    vm.LastUpdateAt = auditvm.LastUpdateAt;
                                    vm.LastUpdateBy = auditvm.LastUpdateBy;
                                    vm.LastUpdateFrom = auditvm.LastUpdateFrom;
                                    vm.CreatedAt = auditvm.CreatedAt;
                                    vm.CreatedBy = auditvm.CreatedBy;
                                    vm.CreatedFrom = auditvm.CreatedFrom;
                                    retResults = SalaryPFSingleAddorUpdate(vm, branchId, VcurrConn, Vtransaction);
                                    if (retResults[0] != "Success")
                                    {
                                        throw new ArgumentNullException("Salary Other Earning Update", "Could not found any item.");
                                    }
                                }
                            //}
                        }
                    }
                }

                retResults = new SalaryProcessDAL().UpdateCurrentEmployee(vm.FiscalYearDetailId, VcurrConn, Vtransaction);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException("Salary PF Update", "Current Employee Update Fail!");
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
        public System.Data.DataTable ExportExcelFile(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, string Orderby = null)
        {
            string[] retResults = new string[6];
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                #region Fiscal Period
                FiscalYearDAL fdal = new FiscalYearDAL();
                SalaryStructureDAL eddal = new SalaryStructureDAL();
                FiscalYearDetailVM fyDVM = new FiscalYearDetailVM();
                fyDVM = fdal.FYPeriodDetail(fid, null, null).FirstOrDefault();
                var fname = fyDVM.PeriodName;
                string PeriodEnd = fyDVM.PeriodEnd;
                string PeriodStart = fyDVM.PeriodStart;
                #endregion

                #region DataRead From DB
                #region Variables
                SqlConnection currConn = null;
                string sqlText = "";
                int j = 2;
                #endregion

                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @" SELECT * FROM(";
                #region Regular
                sqlText += @"
select vw.Code EmpCode,vw.EmpName,ISNULL(vw.Grade+'-'+vw.StepName,'NA') Grade,
(case when vw.Designation is null then 'NA' when vw.Designation='=NA=' then 'NA' else vw.Designation end) Designation,
(case when vw.Department is null then 'NA' when vw.Department='=NA=' then 'NA' else vw.Department end) Department ,
(case when vw.Section is null then 'NA' when vw.Section='=NA=' then 'NA' else vw.Section end) Section,
(case when vw.Project is null then 'NA' when vw.Project='=NA=' then 'NA' else vw.Project end) Project,
Round(ISNULL(vw.PFValue,0),0) Amount,FiscalYeardetailId FYDId,cast(vw.PFStructureId as varchar) PFId
from dbo.ViewSalaryPFDetail vw
left outer join  dbo.PFStructure st on st.Id=vw.PFStructureId
left outer join  dbo.ViewEmployeeInformation vws on vws.EmployeeId=vw.EmployeeId
left outer join  dbo.Grade g on g.Id=vws.GradeId
where 1=1 AND vws.IsActive=1 AND vws.IsArchive=0 
AND vws.Joindate <= @PeriodEnd
";
                if (fid != 0)
                {
                    sqlText += @" and vw.FiscalYearDetailId='" + fid + "'";
                }
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                    sqlText += @" and vws.ProjectId='" + ProjectId + "'";
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                    sqlText += @" and vws.DepartmentId='" + DepartmentId + "'";
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                    sqlText += @" and vws.SectionId='" + SectionId + "'";
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                    sqlText += @" and vws.DesignationId='" + DesignationId + "'";
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    sqlText += @" and vws.Code >='" + CodeF + "'";
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    sqlText += @" and vws.Code<='" + CodeT + "'";
                #endregion
                #region  Resign From This Month or Later
                sqlText += @"
UNION ALL
select vw.Code EmpCode,vw.EmpName,ISNULL(vw.Grade+'-'+vw.StepName,'NA') Grade,
(case when vw.Designation is null then 'NA' when vw.Designation='=NA=' then 'NA' else vw.Designation end) Designation,
(case when vw.Department is null then 'NA' when vw.Department='=NA=' then 'NA' else vw.Department end) Department ,
(case when vw.Section is null then 'NA' when vw.Section='=NA=' then 'NA' else vw.Section end) Section,
(case when vw.Project is null then 'NA' when vw.Project='=NA=' then 'NA' else vw.Project end) Project,
Round(ISNULL(vw.PFValue,0),0) Amount,FiscalYeardetailId FYDId,cast(vw.PFStructureId as varchar) PFId
from dbo.ViewSalaryPFDetail vw
left outer join  dbo.PFStructure st on st.Id=vw.PFStructureId
left outer join  dbo.ViewEmployeeInformation vws on vws.EmployeeId=vw.EmployeeId
left outer join  dbo.Grade g on g.Id=vws.GradeId
where 1=1 AND vws.IsActive=0 AND vws.IsArchive=0 
AND vws.LeftDate >= @PeriodStart 
";
                if (fid != 0)
                {
                    sqlText += @" and vw.FiscalYearDetailId='" + fid + "'";
                }
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                    sqlText += @" and vws.ProjectId='" + ProjectId + "'";
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                    sqlText += @" and vws.DepartmentId='" + DepartmentId + "'";
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                    sqlText += @" and vws.SectionId='" + SectionId + "'";
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                    sqlText += @" and vws.DesignationId='" + DesignationId + "'";
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    sqlText += @" and vws.Code >='" + CodeF + "'";
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    sqlText += @" and vws.Code<='" + CodeT + "'";
                #endregion
                sqlText += @" ) as a ";

                sqlText += " ORDER BY a.FYDId";

                if (Orderby == "DCG")
                    sqlText += " , a.department, a.EmpCode";
                else if (Orderby == "DDC")
                    sqlText += " , a.department, a.EmpCode";
                else if (Orderby == "DGC")
                    sqlText += " , a.department, a.EmpCode";
                else if (Orderby == "DGDC")
                    sqlText += ", a.department, a.EmpCode";
                else if (Orderby == "CODE")
                    sqlText += ", a.EmpCode";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.SelectCommand.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
                da.SelectCommand.Parameters.AddWithValue("@PeriodStart", PeriodStart);
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                    throw new ArgumentNullException("TaxStructure has not been assaign any employee ");
                dt.Columns.Add("Fiscal Period");
                dt.Columns.Add("Type");

                foreach (DataRow row in dt.Rows)
                {
                    row["Fiscal Period"] = fname;
                    row["FYDId"] = fid;
                    row["Type"] = "PF";
                }


                #endregion

                #region Value Round

                string[] columnNames = { "Amount" };

                dt = Ordinary.DtValueRound(dt, columnNames);

                #endregion

                #endregion
                //if (tt == false)
                //{
                //    retResults[0] = "Fail";
                //    retResults[1] = "Data Download UnSuccessfully.";
                //}
                //{
                //    retResults[0] = "Success";
                //    retResults[1] = "Data Download Successfully.";
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;
                }
            }
            finally
            {
                GC.Collect();
            }
        }
        public List<SalaryPFDetailVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string Orderby)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryPFDetailVM> VMs = new List<SalaryPFDetailVM>();
            SalaryPFDetailVM vm;
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
spfd.PeriodName
,fyd.PeriodStart                 
,spfd.FiscalYearDetailId
,spfd.PFValue
,spfd.EmpName
,spfd.Code
,spfd.Designation
,spfd.Department
,spfd.Section
,spfd.Project
,spfd.JoinDate
,spfd.BasicSalary
,spfd.GrossSalary
,spfd.DesignationId
,spfd.DepartmentId
,spfd.SectionId
,spfd.ProjectId
,spfd.Remarks
from ViewSalaryPFDetail spfd 
left outer join grade g on spfd.gradeId = g.id
left outer join FiscalYearDetail fyd on spfd.FiscalYearDetailId =fyd.Id
Where spfd.IsArchive=0 And spfd.IsActive=1  and spfd.PFValue >0
";

                if (fid != 0)
                {
                    sqlText += @" and spfd.FiscalYearDetailId>='" + fid + "'";
                }
                if (fidTo != 0)
                {
                    sqlText += @" and spfd.FiscalYearDetailId<='" + fidTo + "'";
                }

                if (ProjectId != "0_0")
                    sqlText += " and spfd.ProjectId=@ProjectId";

                if (DepartmentId != "0_0")
                    sqlText += " and spfd.DepartmentId=@DepartmentId ";

                if (SectionId != "0_0")
                    sqlText += " and spfd.SectionId=@SectionId ";

                if (DesignationId != "0_0")
                    sqlText += " and spfd.DesignationId=@DesignationId ";

                if (CodeF != "0_0")
                    sqlText += " and spfd.Code>= @CodeF";

                if (CodeT != "0_0")
                    sqlText += " and spfd.Code<= @CodeT";

                //sqlText += " order by spfd.FiscalYearDetailId, spfd.Department, spfd.Section, spfd.Code ";
                sqlText += " ORDER BY spfd.FiscalYearDetailId";

                if (Orderby == "DCG")
                    sqlText += " , spfd.department, spfd.code, g.sl";
                else if (Orderby == "DDC")
                    sqlText += " ,spfd.department, spfd.JoinDate, spfd.code";
                else if (Orderby == "DGC")
                    sqlText += " , spfd.department, g.sl, spfd.code";
                else if (Orderby == "DGDC")
                    sqlText += ", spfd.department, g.sl, spfd.JoinDate, spfd.code";
                else if (Orderby == "CODE")
                    sqlText += ", spfd.code";
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
                    vm = new SalaryPFDetailVM();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
                    vm.PFValue = Convert.ToDecimal(dr["PFValue"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
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
    }
}
