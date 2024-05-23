using SymOrdinary;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using SymViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SymServices.HRM;
using SymServices.Common;
using SymServices.Enum;
using System.Data.OleDb;
using Microsoft.Office.Interop.Excel;
using System.Linq;
using DataTable = Microsoft.Office.Interop.Excel.DataTable;
using SymServices.Attendance;
using SymViewModel.Attendance;

namespace SymServices.Payroll
{
    public class SalaryEarningDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        //#region Methods
        public string[] InsertSalaryEarningNew(int FiscalYearDetailId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
       , string EmployeeIdF, string EmployeeIdT, string EmpType, FiscalYearVM vm, string CompanyName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            EmployeeInfoDAL _EmployeeInfoDAL = new EmployeeInfoDAL();
            MonthlyAttendanceDAL _monthlyAttendanceDAL = new MonthlyAttendanceDAL();
            List<EmployeeSalaryStructureDetailVM> EmployeeSalaryStructureDetails = new List<EmployeeSalaryStructureDetailVM>();
            MonthlyAttendanceVM monthlyAttendanceVM = new MonthlyAttendanceVM();
            EmployeeSalaryStructureDetailVM EmployeeSalaryStructureDetail;

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
                varEmployeeInfoVM.CompanyName = CompanyName;


                employeeVms = _EmployeeInfoDAL.SelectAllEmployee_SalaryProcess(varEmployeeInfoVM, currConn, transaction);

                #endregion

                #endregion

                #region Comments

                //////                sqlText = @"
//////    select  *      from      ViewEmployeeInformation  
//////    where 1=1    and  BranchId = @BranchId
//////
////// ";
//////                if (ProjectId != "0_0")
//////                    sqlText += " and  ProjectId=@ProjectId";
//////                if (DepartmentId != "0_0")
//////                    sqlText += " and  DepartmentId=@DepartmentId";
//////                if (SectionId != "0_0")
//////                    sqlText += " and  SectionId=@SectionId";
//////                if (DesignationId != "0_0")
//////                    sqlText += " and  DesignationId=@DesignationId";
//////                if (EmployeeIdF != "0_0")
//////                    sqlText += " and  EmployeeId>=@EmployeeIdF";
//////                if (EmployeeIdT != "0_0")
//////                    sqlText += " and  EmployeeId<=@EmployeeIdT";
//////                if (EmpType.ToLower() == "new")
//////                {
//////                    sqlText += " and  IsActive=1";
//////                    sqlText += " and  JoinDate>=@PeriodStart";
//////                    sqlText += " and  JoinDate<=@PeriodEnd";
//////                }
//////                else if (EmpType.ToLower() == "regular")
//////                {
//////                    sqlText += " and  IsActive=1";
//////                    sqlText += " and  JoinDate<@PeriodStart";
//////                }
//////                else if (EmpType.ToLower() == "left")
//////                {
//////                    sqlText += " and  IsActive=0";
//////                    sqlText += " and  LeftDate>=@PeriodStart";
//////                    sqlText += " and  LeftDate<=@PeriodEnd";
//////                }
//////                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
//////                cmd.Parameters.AddWithValue("@PeriodStart", PeriodStart);
//////                cmd.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
//////                cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);

//////                if (ProjectId != "0_0")
//////                    cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
//////                if (DepartmentId != "0_0")
//////                    cmd.Parameters.AddWithValue("@DepartmentId", DepartmentId);
//////                if (SectionId != "0_0")
//////                    cmd.Parameters.AddWithValue("@SectionId", SectionId);
//////                if (DesignationId != "0_0")
//////                    cmd.Parameters.AddWithValue("@DesignationId", DesignationId);
//////                if (EmployeeIdF != "0_0")
//////                    cmd.Parameters.AddWithValue("@EmployeeIdF", EmployeeIdF);
//////                if (EmployeeIdT != "0_0")
//////                    cmd.Parameters.AddWithValue("@EmployeeIdT", EmployeeIdT);

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
//////                        employeeVm.IsPermanent = Convert.ToBoolean(dr["IsPermanent"]);
//////                        employeeVm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
//////                        employeeVm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
//////                        employeeVm.GradeId = dr["GradeId"].ToString();
//////                        employeeVms.Add(employeeVm);
//////                    }
//////                    dr.Close();
//////                }
                #endregion

                #region sqlText
                string sqlTextSalaryEarningDetail = "";
                sqlTextSalaryEarningDetail = @"Insert Into SalaryEarningDetail
(
FiscalYearDetailId,SalaryTypeId,SalaryType,EmployeeId,IsFixed,IsEarning,Portion
,PortionSalaryType,Amount,EmployeeSalaryStructureId,ProjectId,DepartmentId
,SectionId,DesignationId,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,EmployeeStatus,GradeId
) Values (
 @FiscalYearDetailId, @SalaryTypeId, @SalaryType
, @EmployeeId, @IsFixed, @IsEarning, @Portion, @PortionSalaryType, @Amount
, @EmployeeSalaryStructureId, @ProjectId, @DepartmentId, @SectionId, @DesignationId
, @Remarks, @IsActive, @IsArchive, @CreatedBy, @CreatedAt, @CreatedFrom,@EmployeeStatus,@GradeId
)";
                #endregion str

                if (employeeVms.Count > 0)
                {
                    foreach (EmployeeInfoVM employee in employeeVms)
                    {
                        if (!string.IsNullOrWhiteSpace(CompanyName) && CompanyName.ToLower() == "tib") 
                        {
                            string[] cFields = { "ma.EmployeeId" ,"ma.FiscalYearDetailId"};
                            string[] cValues = { employee.Id, FiscalYearDetailId.ToString() };
                            monthlyAttendanceVM = _monthlyAttendanceDAL.SelectAll(0, cFields, cValues, currConn, transaction).FirstOrDefault();

                        }
                        #region Delete Existing SalaryEarningDetail
                        sqlText = @"Delete SalaryEarningDetail ";
                        sqlText += " where 1=1 and EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId";

                        SqlCommand cmdDeleteEarningDetail = new SqlCommand(sqlText, currConn, transaction);
                        cmdDeleteEarningDetail.Parameters.AddWithValue("@EmployeeId", employee.Id);
                        cmdDeleteEarningDetail.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                        cmdDeleteEarningDetail.ExecuteNonQuery();

                        #endregion

                        #region Comments
                        //////                sqlText = @"select distinct SalaryTypeId,SalaryType,Portion,IsEarning,IsFixed,SalaryType,PortionSalaryType,0 EmployeeSalaryStructureId
                        //////                            ,sum(Amount)Amount from EmployeeSalaryStructureDetail
                        //////";
                        //////                sqlText += " where 1=1 and EmployeeId=@EmployeeId and IncrementDate<=@IncrementDate ";
                        //////                sqlText += " group by SalaryTypeId,SalaryType,Portion,IsEarning,IsFixed,SalaryType,PortionSalaryType ";
                        #endregion

                        #region EmployeeEarning

                        sqlText = @"
select distinct 
SalaryTypeId,SalaryType, 0 Portion,IsEarning,1 IsFixed,'NA' PortionSalaryType,0 EmployeeSalaryStructureId
,sum(Amount)Amount 
from EmployeeSalaryStructureDetail
";
                        sqlText += " where 1=1 and EmployeeId=@EmployeeId and IncrementDate<=@IncrementDate and IsActive=1";
                        sqlText += " group by SalaryTypeId,SalaryType,IsEarning";

                        SqlCommand cmdeEarning = new SqlCommand(sqlText, currConn, transaction);
                        cmdeEarning.Parameters.AddWithValue("@EmployeeId", employee.Id);
                        cmdeEarning.Parameters.AddWithValue("@IncrementDate", PeriodEnd);
                        //cmdeEarning.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                        EmployeeSalaryStructureDetails = new List<EmployeeSalaryStructureDetailVM>();
                        using (SqlDataReader drSSD = cmdeEarning.ExecuteReader())
                        {
                            while (drSSD.Read())
                            {
                                EmployeeSalaryStructureDetail = new EmployeeSalaryStructureDetailVM();
                                EmployeeSalaryStructureDetail.SalaryTypeId = drSSD["SalaryTypeId"].ToString();
                                EmployeeSalaryStructureDetail.SalaryType = drSSD["SalaryType"].ToString();
                                EmployeeSalaryStructureDetail.Portion = Convert.ToDecimal(drSSD["Portion"]);
                                EmployeeSalaryStructureDetail.IsEarning = Convert.ToBoolean(drSSD["IsEarning"]);
                                EmployeeSalaryStructureDetail.IsFixed = Convert.ToBoolean(drSSD["IsFixed"]);
                                EmployeeSalaryStructureDetail.PortionSalaryType = drSSD["PortionSalaryType"].ToString();
                                EmployeeSalaryStructureDetail.EmployeeSalaryStructureId = drSSD["EmployeeSalaryStructureId"].ToString();
                                EmployeeSalaryStructureDetail.Remarks = "NA";
                                EmployeeSalaryStructureDetail.Amount = Convert.ToDecimal(drSSD["Amount"]);


                                EmployeeSalaryStructureDetails.Add(EmployeeSalaryStructureDetail);
                            }
                            drSSD.Close();
                        }
                        #endregion EmployeeEarning

                        if (EmployeeSalaryStructureDetails.Count > 0)
                        {
                            foreach (EmployeeSalaryStructureDetailVM SSD in EmployeeSalaryStructureDetails)
                            {
                                #region Sql Execution
                                
                                SqlCommand cmdempBonusDet;

                                cmdempBonusDet = new SqlCommand(sqlTextSalaryEarningDetail, currConn, transaction);
                                //cmdempBonusDet.Parameters.AddWithValue("@SalaryEarningId", SalaryEarningID);
                                cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                                cmdempBonusDet.Parameters.AddWithValue("@SalaryTypeId", SSD.SalaryTypeId);
                                cmdempBonusDet.Parameters.AddWithValue("@SalaryType", SSD.SalaryType);
                                cmdempBonusDet.Parameters.AddWithValue("@IsFixed", SSD.IsFixed);
                                cmdempBonusDet.Parameters.AddWithValue("@IsEarning", SSD.IsEarning);
                                cmdempBonusDet.Parameters.AddWithValue("@Portion", SSD.Portion);
                                cmdempBonusDet.Parameters.AddWithValue("@PortionSalaryType", SSD.PortionSalaryType);
                                //cmdempBonusDet.Parameters.AddWithValue("@Amount", SSD.Amount);
                                cmdempBonusDet.Parameters.AddWithValue("@EmployeeSalaryStructureId", SSD.EmployeeSalaryStructureId);

                                cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", employee.Id);
                                cmdempBonusDet.Parameters.AddWithValue("@ProjectId", employee.ProjectId);
                                cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
                                cmdempBonusDet.Parameters.AddWithValue("@SectionId", employee.SectionId);
                                cmdempBonusDet.Parameters.AddWithValue("@DesignationId", employee.DesignationId);
                                cmdempBonusDet.Parameters.AddWithValue("@GradeId", employee.GradeId);

                                cmdempBonusDet.Parameters.AddWithValue("@Remarks", SSD.Remarks);
                                cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                                cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                                cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                                cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                                cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                                cmdempBonusDet.Parameters.AddWithValue("@EmployeeStatus", EmpType);
                                if (!string.IsNullOrWhiteSpace(CompanyName) && CompanyName.ToLower() == "tib") 
                                {
                                    decimal amnt = 0;
                                    amnt = SSD.Amount;
                                    if (SSD.SalaryType == "Gross"
                                        || SSD.SalaryType == "Basic"
                                        || SSD.SalaryType == "Medical"
                                        || SSD.SalaryType == "Conveyance"
                                        || SSD.SalaryType == "HouseRent"
                                        )
                                    {
                                        
                                        if (monthlyAttendanceVM.NPDay > 0)
                                        {
                                            amnt = SSD.Amount - (amnt / monthlyAttendanceVM.DOM * (monthlyAttendanceVM.NPDay));
                                        }
                                        amnt = Math.Round(amnt, MidpointRounding.AwayFromZero);
                                        cmdempBonusDet.Parameters.AddWithValue("@Amount", amnt);

                                    }
                                    else
                                    {
                                        cmdempBonusDet.Parameters.AddWithValue("@Amount", amnt);
                                    
                                    }
                                }
                                else
                                {
                                    cmdempBonusDet.Parameters.AddWithValue("@Amount", SSD.Amount);
                                
                                }
                                cmdempBonusDet.ExecuteScalar();
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


        public string[] InsertSalaryDependent(int FiscalYearDetailId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
, string EmployeeIdF, string EmployeeIdT, string EmpType, string CompanyName, FiscalYearVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            EmployeeInfoDAL _EmployeeInfoDAL = new EmployeeInfoDAL();
            List<EmployeeSalaryStructureDetailVM> EmployeeSalaryStructureDetails = new List<EmployeeSalaryStructureDetailVM>();
            EmployeeSalaryStructureDetailVM EmployeeSalaryStructureDetail;

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

                varEmployeeInfoVM.FiscalYearDetailId = FiscalYearDetailId;
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
                varEmployeeInfoVM.CompanyName = CompanyName;

                employeeVms = _EmployeeInfoDAL.SelectAllEmployee_SalaryProcess(varEmployeeInfoVM, currConn, transaction);

                #endregion

                #endregion

                // get salary type 

                SettingDAL settingDal = new SettingDAL();

               string depAge = settingDal.settingValue("Dependent", "Age", currConn, transaction);
               string depValue = settingDal.settingValue("Dependent", "Allowance", currConn, transaction);
               string depCount = settingDal.settingValue("Dependent", "Allow", currConn, transaction);

                sqlText = "select * from EnumSalaryType where IsChildAllow =1";

                System.Data.DataTable dtSalaryType = new System.Data.DataTable();

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dtSalaryType);

                if (dtSalaryType.Rows.Count == 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Data Save Successfully.";

                    return retResults;
                }




                string[] empIdara = employeeVms.Select(x => x.Id).ToArray();


                sqlText = @"

select EmployeeId, count(Id)Count from EmployeeDependent
where 1=1
and DATEDIFF(MONTH,Convert(datetime,DateofBirth),@EndDate) < =@year * 12
and EmployeeId in ('" + string.Join("','", empIdara) + @"')
and EmployeeDependent.IsActive=1 and EmployeeDependent.IsArchive=0
and EmployeeDependent.IsDependentAllowance=1
group by EmployeeId";


                System.Data.DataTable dtEmployees = new System.Data.DataTable();
                cmd.CommandText = sqlText;
                cmd.Parameters.AddWithValue("@year", depAge );
                cmd.Parameters.AddWithValue("@EndDate", Ordinary.StringToDate(PeriodEnd));

                adapter.SelectCommand = cmd;
                adapter.Fill(dtEmployees);



                string deletetText = @"
delete from SalaryEarningDetail
where FiscalYearDetailId =@FiscalYearDetailId and SalaryTypeId=@SalaryTypeId and EmployeeId in('" + string.Join("','", empIdara) + "')";

                cmd.CommandText = deletetText;

                cmd.Parameters.AddWithValueAndParamCheck("@FiscalYearDetailId",FiscalYearDetailId);
                cmd.Parameters.AddWithValueAndParamCheck("@SalaryTypeId", dtSalaryType.Rows[0]["Id"].ToString());
                cmd.ExecuteNonQuery();

                string salaryEarningText = @"

INSERT INTO SalaryEarningDetail
           (SalaryTypeId
           ,SalaryType
           ,EmployeeId
           ,IsFixed
           ,Portion
           ,PortionSalaryType
           ,Amount
           ,EmployeeSalaryStructureId
           ,Remarks
           ,IsActive
           ,IsArchive
           ,CreatedBy
           ,CreatedAt
           ,CreatedFrom
           ,LastUpdateBy
           ,LastUpdateAt
           ,LastUpdateFrom
           ,ProjectId
           ,DepartmentId
           ,SectionId
           ,DesignationId
           ,FiscalYearDetailId
           ,IsEarning
           ,EmployeeStatus
           ,GradeId)                        
values (
 @SalaryTypeId
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
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@FiscalYearDetailId
,@IsEarning
,@EmployeeStatus
,@GradeId

)
";
                cmd = new SqlCommand("", currConn, transaction);
                cmd.CommandText = salaryEarningText;


                foreach (DataRow dataRow in dtEmployees.Rows)
                {
                    decimal amount = 0;
                    decimal count = 0;

                    int empCount = Convert.ToInt32(dataRow["Count"]);
                    int _depCount = Convert.ToInt32(depCount);

                    count = empCount > _depCount ? _depCount : empCount;
                    amount = count * Convert.ToDecimal(depValue);
                    string empId = dataRow["EmployeeId"].ToString();

                    EmployeeInfoVM empVm = employeeVms.FirstOrDefault(x => x.Id == empId);


                    cmd.Parameters.AddWithValueAndParamCheck("@SalaryTypeId", dtSalaryType.Rows[0]["Id"].ToString());
                    cmd.Parameters.AddWithValueAndParamCheck("@SalaryType", dtSalaryType.Rows[0]["TypeName"].ToString());
                    cmd.Parameters.AddWithValueAndParamCheck("@EmployeeId", empId);
                    cmd.Parameters.AddWithValueAndParamCheck("@IsFixed",1);
                    cmd.Parameters.AddWithValueAndParamCheck("@Portion",0);
                    cmd.Parameters.AddWithValueAndParamCheck("@PortionSalaryType","NA");
                    cmd.Parameters.AddWithValueAndParamCheck("@Amount",amount);
                    cmd.Parameters.AddWithValueAndParamCheck("@EmployeeSalaryStructureId",0); // need to update
                    cmd.Parameters.AddWithValueAndParamCheck("@Remarks","-");
                    cmd.Parameters.AddWithValueAndParamCheck("@IsActive",1);
                    cmd.Parameters.AddWithValueAndParamCheck("@IsArchive",0);
                    cmd.Parameters.AddWithValueAndParamCheck("@CreatedBy",vm.CreatedBy);
                    cmd.Parameters.AddWithValueAndParamCheck("@CreatedAt",vm.CreatedAt);
                    cmd.Parameters.AddWithValueAndParamCheck("@CreatedFrom",vm.CreatedFrom);
                    cmd.Parameters.AddWithValueAndParamCheck("@LastUpdateBy","local");
                    cmd.Parameters.AddWithValueAndParamCheck("@LastUpdateAt","19000101");
                    cmd.Parameters.AddWithValueAndParamCheck("@LastUpdateFrom","local");
                    cmd.Parameters.AddWithValueAndParamCheck("@ProjectId", empVm.ProjectId);
                    cmd.Parameters.AddWithValueAndParamCheck("@DepartmentId", empVm.DepartmentId);
                    cmd.Parameters.AddWithValueAndParamCheck("@SectionId", empVm.SectionId);
                    cmd.Parameters.AddWithValueAndParamCheck("@DesignationId",empVm.DesignationId); // need to update
                    cmd.Parameters.AddWithValueAndParamCheck("@FiscalYearDetailId", FiscalYearDetailId);
                    cmd.Parameters.AddWithValueAndParamCheck("@IsEarning",1); 
                    cmd.Parameters.AddWithValueAndParamCheck("@EmployeeStatus", EmpType);
                    cmd.Parameters.AddWithValueAndParamCheck("@GradeId", empVm.GradeId);


                    cmd.ExecuteNonQuery();

                }


                sqlText = @"
update SalaryEarningDetail set EmployeeSalaryStructureId = EmployeeSalaryStructureDetail.EmployeeSalaryStructureId
from EmployeeSalaryStructureDetail

where EmployeeSalaryStructureDetail.EmployeeId = SalaryEarningDetail.EmployeeId
and SalaryEarningDetail.EmployeeSalaryStructureId = '0'";

                cmd.CommandText = sqlText;

                cmd.ExecuteNonQuery();


                #region Save
                #endregion Save
                #region Commit
                if (Vtransaction == null)
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



        public string[] SalaryEarningSingleAddorUpdate(List<SalaryEarningDetailVM> vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Employee Salary Earning Process Single"; //Method Name


            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            //            #region Try

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

            //                if (transaction == null)
            //                {
            //                    transaction = currConn.BeginTransaction("");
            //                }
            //                sqlText = @"Delete SalaryEarningDetail ";
            //                sqlText += " where SalaryEarningId=@SalaryEarningId";
            //                sqlText += " and EmployeeId=@EmployeeId";

            //                SqlCommand cmdDeletePrevious = new SqlCommand(sqlText, currConn, transaction);
            //                cmdDeletePrevious.Parameters.AddWithValue("@SalaryEarningId", SalaryEarningID);
            //                cmdDeletePrevious.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
            //                cmdDeletePrevious.ExecuteNonQuery();


            //                List<EmployeeInfoVM> employeeVms = new List<EmployeeInfoVM>();
            //                EmployeeInfoVM employeeVm;
            //                sqlText = @"	SELECT 
            //                                e.Id
            //                                ,e.GrossSalary
            //                                ,e.BasicSalary
            //                                ,e.ProjectId
            //                                ,e.DepartmentId
            //                                ,e.SectionId
            //,e.DesignationId
            //                                ,ess.SalaryTypeId,ess.SalaryType, ess.IsFixed, ess.Portion, ess.PortionSalaryType, ess.Amount, ess.EmployeeSalaryStructureId,ess.Remarks
            //                                from EmployeeSalaryStructureDetail ess
            //                                left outer join 
            //                                ViewEmployeeInformation e on ess.EmployeeId=e.Id
            //                                where ess.IsArchive=0 and ess.isactive=1
            //                                and  e.IsArchive=0 and e.isactive=1 and e.Id=@EmployeeID";
            //                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
            //                cmd.Parameters.AddWithValue("@EmployeeID", vm.EmployeeId);

            //                using (SqlDataReader dr = cmd.ExecuteReader())
            //                {
            //                    while (dr.Read())
            //                    {
            //                        employeeVm = new EmployeeInfoVM();
            //                        employeeVm.Id = dr["Id"].ToString();
            //                        employeeVm.ProjectId = dr["ProjectId"].ToString();
            //                        employeeVm.DepartmentId = dr["DepartmentId"].ToString();
            //                        employeeVm.SectionId = dr["SectionId"].ToString();
            //                        employeeVm.DesignationId = dr["DesignationId"].ToString();
            //                        employeeVm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
            //                        employeeVm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);

            //                        employeeVm.SalaryTypeId = dr["SalaryTypeId"].ToString();
            //                        employeeVm.SalaryType = dr["SalaryType"].ToString();
            //                        employeeVm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
            //                        employeeVm.Portion = Convert.ToDecimal(dr["Portion"]);
            //                        employeeVm.PortionSalaryType = dr["PortionSalaryType"].ToString();
            //                        employeeVm.Amount = Convert.ToDecimal(dr["Amount"]);
            //                        employeeVm.EmployeeSalaryStructureId = dr["EmployeeSalaryStructureId"].ToString();
            //                        employeeVm.Remarks = dr["Remarks"].ToString();
            //                        employeeVms.Add(employeeVm);
            //                    }
            //                    dr.Close();
            //                }

            //                sqlText = @"Insert Into SalaryEarningDetail
            //
            //(
            // SalaryEarningId
            //,FiscalYearDetailId
            //,SalaryTypeId
            //,SalaryType
            //,EmployeeId
            //,IsFixed
            //,Portion
            //,PortionSalaryType
            //,Amount
            //,EmployeeSalaryStructureId
            //,ProjectId
            //,DepartmentId
            //,SectionId
            //,DesignationId
            //,Remarks
            //,IsActive
            //,IsArchive
            //,CreatedBy
            //,CreatedAt
            //,CreatedFrom
            //) Values (
            // @SalaryEarningId
            //, @FiscalYearDetailId
            //, @SalaryTypeId
            //, @SalaryType
            //, @EmployeeId
            //, @IsFixed
            //, @Portion
            //, @PortionSalaryType
            //, @Amount
            //, @EmployeeSalaryStructureId
            //, @ProjectId
            //, @DepartmentId
            //, @SectionId
            //, @DesignationId
            //, @Remarks
            //, @IsActive
            //, @IsArchive
            //, @CreatedBy
            //, @CreatedAt
            //, @CreatedFrom
            //)";

            //                SqlCommand cmdempBonusDet;
            //                if (employeeVms.Count > 0)
            //                {
            //                    foreach (EmployeeInfoVM item in employeeVms)
            //                    {
            //                        cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
            //                        cmdempBonusDet.Parameters.AddWithValue("@SalaryEarningId", SalaryEarningID);
            //                        cmdempBonusDet.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
            //                        cmdempBonusDet.Parameters.AddWithValue("@SalaryTypeId", item.SalaryTypeId);
            //                        cmdempBonusDet.Parameters.AddWithValue("@SalaryType", item.SalaryType);
            //                        cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", item.Id);
            //                        cmdempBonusDet.Parameters.AddWithValue("@IsFixed", item.IsFixed);
            //                        cmdempBonusDet.Parameters.AddWithValue("@Portion", item.Portion);
            //                        cmdempBonusDet.Parameters.AddWithValue("@PortionSalaryType", item.PortionSalaryType);
            //                        cmdempBonusDet.Parameters.AddWithValue("@Amount", item.Amount);
            //                        cmdempBonusDet.Parameters.AddWithValue("@EmployeeSalaryStructureId", item.EmployeeSalaryStructureId);
            //                        cmdempBonusDet.Parameters.AddWithValue("@ProjectId", item.ProjectId);
            //                        cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", item.DepartmentId);
            //                        cmdempBonusDet.Parameters.AddWithValue("@SectionId", item.SectionId);
            //                        cmdempBonusDet.Parameters.AddWithValue("@DesignationId", item.DesignationId);
            //                        cmdempBonusDet.Parameters.AddWithValue("@Remarks", item.Remarks);
            //                        cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
            //                        cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
            //                        cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
            //                        cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
            //                        cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

            //                        cmdempBonusDet.ExecuteScalar();
            //                    }

            //                }

            //                #region Save

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

            //                #endregion SuccessResult

            //            }

            //            #endregion try
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

            #region Results

            return retResults;
            #endregion


        }
        //==================Distinct Employee Name =================
        public List<EmployeeInfoVM> GetEmployeebyfid(int? fid)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
Distinct
vw.Id
,vw.EmpName
,vw.Code
,vw.BasicSalary
,vw.GrossSalary
,vw.Department
,vw.Project
,vw.Designation
,vw.Section
,vw.BasicSalary
,vw.JoinDate
from  SalaryEarningDetail sed 
left outer join ViewEmployeeInformation vw on sed.EmployeeId=vw.id
left outer join FiscalYearDetail fyd on sed.FiscalYearDetailId =fyd.Id
Where sed.IsArchive=0 And sed.IsActive=1 and sed.Amount >=0";

                if (fid != null && fid != 0)
                {
                    sqlText += @" and fyd.Id='" + fid + "'";
                }
                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
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
        public List<SalaryEarningDetailVM> SelectAll(int? fid)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryEarningDetailVM> VMs = new List<SalaryEarningDetailVM>();
            SalaryEarningDetailVM vm;
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
sed.Id
,sed.Amount
,est.Name SalarytypeName
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
,sed.FiscalYearDetailId
,vw.Code
from  SalaryEarningDetail sed 
left outer join ViewEmployeeInformation vw on sed.EmployeeId=vw.id
left outer join FiscalYearDetail fyd on sed.FiscalYearDetailId =fyd.Id
left outer join EnumSalaryType est on sed.SalaryTypeId=est.Id
Where sed.IsArchive=0 And sed.IsActive=1 and sed.Amount >=0";

                if (fid != null && fid != 0)
                {
                    sqlText += @" and sed.FiscalYearDetailId='" + fid + "'";
                }
                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;


                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryEarningDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.Amount = Convert.ToDecimal(dr["Amount"].ToString());
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.SalaryTypeName = dr["SalarytypeName"].ToString();
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

        //==================SelectByID=================
        public SalaryEarningDetailVM SelectById(string Id)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            SalaryEarningDetailVM vm = new SalaryEarningDetailVM();
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
,e.EmpName
,sa.Amount
,sa.FiscalYearDetailId
,edt.Name SalaryTypeName
,sa.SalaryTypeId
,sa.Remarks
,sa.IsActive
,sa.IsArchive
,sa.CreatedBy
,sa.CreatedAt
,sa.CreatedFrom
,sa.LastUpdateBy
,sa.LastUpdateAt
,sa.LastUpdateFrom
From SalaryEarningDetail sa 
left outer join ViewEmployeeInformation e on sa.EmployeeId=e.id
left outer join EnumSalaryType edt on sa.SalaryTypeId = edt.Id 
Where 1=1 and  sa.IsArchive=0  and sa.id=@Id 
ORDER BY e.Department, e.EmpName desc
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
                    vm = new SalaryEarningDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Amount = Convert.ToDecimal(dr["Amount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.SalaryTypeId = dr["SalaryTypeId"].ToString();
                    vm.SalaryTypeName = dr["SalaryTypeName"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
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
        //==================SelectByID=================
        public List<SalaryEarningDetailVM> SelectAllById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SalaryEarningDetailVM vm = new SalaryEarningDetailVM();
            List<SalaryEarningDetailVM> vms = new List<SalaryEarningDetailVM>();
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
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,sa.Amount
,sa.FiscalYearDetailId
,edt.Name SalaryTypeName
,sa.SalaryTypeId
,sa.Remarks
,sa.IsActive
,sa.IsArchive
,sa.CreatedBy
,sa.CreatedAt
,sa.CreatedFrom
,sa.LastUpdateBy
,sa.LastUpdateAt
,sa.LastUpdateFrom
From SalaryEarningDetail sa 
left outer join ViewEmployeeInformation e on sa.EmployeeId=e.id
left outer join EnumSalaryType edt on sa.SalaryTypeId = edt.Id 
Where 1=1 and  sa.IsArchive=0  and sa.id=@Id 
ORDER BY e.Department, e.EmpName desc
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
                    vm = new SalaryEarningDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Amount = Convert.ToDecimal(dr["Amount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.SalaryTypeId = dr["SalaryTypeId"].ToString();
                    vm.SalaryTypeName = dr["SalaryTypeName"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
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
        //==================SelectByID=================
        public List<SalaryEarningDetailVM> SelectByIdandFiscalyearDetail(string empId, int FiscalYearDetailId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            SalaryEarningDetailVM vm = new SalaryEarningDetailVM();
            List<SalaryEarningDetailVM> vms = new List<SalaryEarningDetailVM>();
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
,sa.Amount
,sa.FiscalYearDetailId
,edt.Name
,sa.Remarks
,sa.SalaryTypeId
,sa.SalaryType
From SalaryEarningDetail sa 
left outer join EnumSalaryType edt on sa.SalaryTypeId = edt.Id 
Where sa.IsArchive=0  and sa.EmployeeId=@Id 
and sa.FiscalYearDetailId=@FiscalYearDetailId  and sa.IsEarning=1
ORDER BY edt.SL ASC
";
                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@Id", empId);

                objCommVehicle.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryEarningDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.SalaryTypeName = dr["Name"].ToString();
                    vm.Amount = Convert.ToDecimal(dr["Amount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.SalaryTypeId = dr["SalaryTypeId"].ToString();
                    vm.SalaryType = dr["SalaryType"].ToString();
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
        //==================Get All Distinct FiscalPeriodName =================
        public List<SalaryEarningDetailVM> GetPeriodname()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryEarningDetailVM> vms = new List<SalaryEarningDetailVM>();
            SalaryEarningDetailVM vm;
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
from  ViewSalaryEarningDetail ve 
left outer join FiscalYearDetail fyd on ve.FiscalYearDetailId =fyd.Id
Where 1=1 
order by PeriodStart
";
                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryEarningDetailVM();
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
        public string[] SalaryEarningDetailsDelete(string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryEarningD"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "Delete SalaryEarningDetail where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        sqlText = "";
                        sqlText = "Delete SalaryEarningDetailEmployeer where SalaryEarningDetailId=@Id";
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
                    //    throw new ArgumentNullException("Salary Earning Delete", vm.Id + " could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Earning Details Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary Earning Details.";
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
        public string[] SalaryEarningDelete(string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSalaryEarning"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryEarning"); }

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
                        sqlText = "Delete SalaryEarning where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        #endregion

                        #region Details
                        sqlText = "";
                        sqlText = "Delete SalaryEarningDetail where SalaryEarningId=@Id";
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
                    //    throw new ArgumentNullException("Salary Earning Delete"," could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Earning Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary Earning.";
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
        public List<EmployeeInfoVM> SelectAllSalaryEarningDetailsEmployeeAndPeriod(string salaryEarningId, SqlConnection currConn, bool callFromOutSide)
        {

            #region Variables

            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
e.id,e.Salutation_E,e.MiddleName,e.LastName
,SUM(sed.Amount) Amount,sed.Remarks,sed.FiscalYearDetailId
from EmployeeInfo e
right join SalaryEarningDetail sed on  sed.EmployeeId=e.Id
where  sed.SalaryEarningId=@SalaryEarningId and sed.IsArchive=0
group by e.id,e.Salutation_E,e.MiddleName,e.LastName
,sed.Remarks,sed.FiscalYearDetailId
 ";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@SalaryEarningId", salaryEarningId);

                using (SqlDataReader dr = objCommVehicle.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeInfoVM();
                        vm.Id = dr["Id"].ToString();
                        vm.FullName = dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                        vm.Amount = Convert.ToDecimal(dr["Amount"]);
                        vm.Remarks = dr["Remarks"].ToString();
                        vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
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
        public List<EmployeeInfoVM> SelectAllSalaryEarningDetails(string empoyeeId, int PeriodId, SqlConnection currConn, bool callFromOutSide)
        {

            #region Variables

            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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

                //                sqlText = @" 
                //
                //select 
                //e.Salutation_E,e.MiddleName,e.LastName
                //,sed.SalaryType,sed.Amount,sed.Remarks
                //from EmployeeInfo e
                //right join SalaryEarningDetail sed on  sed.EmployeeId=e.Id
                //where sed.FiscalYearDetailId=@PeriodId and e.Id=@empoyeeId ";

                sqlText = @" 
select
se.Id, e.Salutation_E,e.MiddleName,e.LastName
,sed.SalaryType ,sed.SalaryTypeId, sed.Amount,sed.Remarks
from EmployeeInfo e
left outer join SalaryEarningDetail sed on  sed.EmployeeId=e.Id

left outer join SalaryEarning se on  sed.SalaryEarningId=se.Id
where sed.FiscalYearDetailId=@PeriodId and e.Id=@empoyeeId ";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@PeriodId", PeriodId);
                objCommVehicle.Parameters.AddWithValue("@empoyeeId", empoyeeId);

                using (SqlDataReader dr = objCommVehicle.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeInfoVM();
                        vm.Id = dr["Id"].ToString();
                        vm.FullName = dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                        vm.SalaryType = dr["SalaryType"].ToString();
                        vm.SalaryTypeId = dr["SalaryTypeId"].ToString();
                        vm.Amount = Convert.ToDecimal(dr["Amount"]);
                        vm.Remarks = dr["Remarks"].ToString();
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
        public string[] SalaryEarningSingleEdit(SalaryEarningDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Employee Salary Earning Process Single Edit"; //Method Name

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

                sqlText = @"Update  SalaryEarningDetail

set
SalaryType			=@SalaryType
,Amount             =@Amount
,Remarks            =@Remarks
,LastUpdateBy       =@LastUpdateBy
,LastUpdateAt       =@LastUpdateAt
,LastUpdateFrom     =@LastUpdateFrom
where Id=@Id
";

                SqlCommand cmdempBonusDet;
                cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                cmdempBonusDet.Parameters.AddWithValue("@Id", vm.Id);
                cmdempBonusDet.Parameters.AddWithValue("@SalaryType", vm.SalaryType);
                cmdempBonusDet.Parameters.AddWithValue("@Amount", vm.Amount);
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
        public string GetPeriodName(string SalaryEarningId)
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

                sqlText = @"select f.PeriodName from SalaryEarning t 
join FiscalYearDetail f on f.id=t.FiscalYearDetailId
where t.Id=@SalaryEarningId";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@SalaryEarningId", SalaryEarningId);
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
        public SalaryEarningDetailVM SalaryEarningBySalaryTypeSingle(string Id, string employeeId, string SalaryTypeId)
        {
            string result = "";
            SqlConnection currConn = null;
            string sqlText = "";
            SalaryEarningDetailVM vm = new SalaryEarningDetailVM();
            try
            {


                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                sqlText = @"select 
e.id,e.Salutation_E,e.MiddleName,e.LastName,sed.Remarks,sed.FiscalYearDetailId
,sed.Amount,sed.SalaryType,sed.SalaryTypeId
from EmployeeInfo e
left outer join SalaryEarningDetail sed on  sed.EmployeeId=e.Id
where  sed.SalaryEarningId=@Id and sed.IsArchive=0 and e.Id=@employeeId and sed.SalaryTypeId=@SalaryTypeId";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@employeeId", employeeId);
                cmd.Parameters.AddWithValue("@SalaryTypeId", SalaryTypeId);
                SqlDataReader dr;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryEarningDetailVM();
                    vm.SalaryTypeId = dr["SalaryTypeId"].ToString();
                    vm.SalaryType = dr["SalaryType"].ToString();
                    vm.Amount = Convert.ToDecimal(dr["Amount"]);
                }
                dr.Close();

            }
            #region catch
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return vm;
        }
        #region ImportExport
        public string[] ImportExcelFile(string fileName, SalaryEarningDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Salary Earning"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region SQL Connection
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                 "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                                 "Excel 12.0;HDR=YES;" + "\"";
            OleDbConnection theConnection = new OleDbConnection(connectionString);
            #endregion SQL Connection
            EmployeeInfoDAL _dalemp = new EmployeeInfoDAL();
            ViewEmployeeInfoVM empVM = new ViewEmployeeInfoVM();
            FiscalYearDAL fydal = new FiscalYearDAL();
            EnumSalaryTypeDAL eddal = new EnumSalaryTypeDAL();
            FiscalYearDetailVM FYDVM = new FiscalYearDetailVM();
            #region try
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
                theConnection.Open();
                OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [DataSheet$]", theConnection);
                DataSet dt = new DataSet();
                da.Fill(dt);
                var a = "";
                foreach (DataRow item in dt.Tables[0].Rows)
                {
                    vm = new SalaryEarningDetailVM();
                    //empVM=_dalemp.se
                    empVM = _dalemp.ViewSelectAllEmployee(item["EmpCode"].ToString(), null, null, null, null, null, null, currConn, transaction).FirstOrDefault();
                    if (empVM == null || empVM.Id == null)
                    {
                        throw new ArgumentNullException("Employee Code " + item["EmpCode"].ToString() + " Not in System", "Employee Code " + item["EmpCode"].ToString() + " Not in System");
                    }
                    else
                    {
                        FYDVM = fydal.FYPeriodDetail(Convert.ToInt32(item["FYDId"].ToString()), currConn, transaction).FirstOrDefault();
                        if (vm.FiscalYearDetailId == null)
                        {
                            throw new ArgumentNullException("Fiscal Period" + item["FYDId"].ToString() + " Not in System", "Fiscal Period " + item["FYDId"].ToString() + " Not in System");
                        }
                        else
                        {
                            var SalaryTypeId = eddal.SelectById(item["SDId"].ToString());
                            if (SalaryTypeId == null)
                            {
                                throw new ArgumentNullException("Salary Ear not in System,", "Fiscal Period " + item["EDId"].ToString() + " Not in System");
                            }
                            else
                            {
                                if (!Ordinary.IsNumeric(item["Amount"].ToString()))
                                {
                                    throw new ArgumentNullException("Please input the Numeric Value in Amount", "Please input the Numeric Value in Amount");
                                }
                                else
                                {

                                    vm.EmployeeId = empVM.Id;
                                    vm.FiscalYearDetailId = Convert.ToInt32(FYDVM.Id);
                                    vm.Amount = Convert.ToDecimal(item["Amount"]);
                                    vm.ProjectId = empVM.ProjectId;
                                    vm.DepartmentId = empVM.DepartmentId;
                                    vm.SectionId = empVM.SectionId;
                                    vm.DesignationId = empVM.DesignationId;
                                    retResults = SalaryEarningSingleEdit(vm);
                                    if (retResults[0] != "Success")
                                    {
                                        throw new ArgumentNullException("Salary Earning Update", "Could not found any item.");
                                    }
                                }
                            }
                        }
                    }
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

        public bool ExportExcelFile(string Filepath, string FileName, int fid, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT)
        {
            try
            {
                FiscalYearDAL fdal = new FiscalYearDAL();

                var fname = fdal.FYPeriodDetail(fid, null, null).FirstOrDefault().PeriodName;

                Application app = new Application();
                _Workbook workbook = app.Workbooks.Add(System.Type.Missing);
                _Worksheet worksheet = new Worksheet();
                app.Visible = false;
                worksheet = workbook.Sheets["Sheet1"] as _Worksheet;
                worksheet = workbook.ActiveSheet as _Worksheet;
                worksheet.Name = "DataSheet";
                worksheet.Cells[1, 1] = "Sl#";
                worksheet.Cells[1, 2] = "EmpCode";
                worksheet.Cells[1, 3] = "EmpName";
                worksheet.Cells[1, 4] = "Designation";
                worksheet.Cells[1, 5] = "Department";
                worksheet.Cells[1, 6] = "Section";
                worksheet.Cells[1, 7] = "Project";
                worksheet.Cells[1, 8] = "TransactionType";
                worksheet.Cells[1, 9] = "Amount";
                worksheet.Cells[1, 10] = "Fiscal Period";
                worksheet.Cells[1, 11] = "SalaryType";
                worksheet.Cells[1, 12] = "FYDId";
                worksheet.Cells[1, 13] = "SDId";
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
                sqlText = @"select * from (select vw.Code EmpCode,vw.EmpName,
(case when Designation is null then 'NA' when Designation='=NA=' then 'NA' else Designation end) Designation,
(case when Department is null then 'NA' when Department='=NA=' then 'NA' else Department end) Department ,
(case when Section is null then 'NA' when Section='=NA=' then 'NA' else Section end) Section,
(case when Project is null then 'NA' when Project='=NA=' then 'NA' else Project end) Project,
SalaryType TransactionType,Amount,FiscalYeardetailId FYDId,CAST(SalaryTypeId AS varchar) SDId,st.TypeName SalaryType
from ViewSalaryEarningDetail vw
left outer join EnumSalaryType st on  vw.SalaryTypeId=st.Id where 1=1";
                if (fid != 0)
                {
                    sqlText += @" and FiscalYearDetailId='" + fid + "'";
                }
                sqlText += @"
union all
select Code EmpCode,EmpName,
(case when Designation is null  then 'NA' when Designation='=NA=' then 'NA' else Designation end) Designation ,
(case when Department is null then 'NA' when Department='=NA=' then 'NA' else Department end) Department,
(case when Section is null then 'NA' when Section='=NA=' then 'NA' else Section end) Section,
(case when Project is null then 'NA' when Project='=NA=' then 'NA' else Project end) Project
,'LWP' TransactionType, 0 Amount,0 FYDId,'0' SDId,'NA' SalaryType
from ViewEmployeeInformation vws
where 1=1 AND vws.IsActive=1 AND vws.IsArchive=0 ";
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
                sqlText += @" and EmployeeId not in (select EmployeeId from SalaryEarningDetail)
                            ) as a order by Department,Section,Project, EmpCode ";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);

                foreach (DataRow item in dt.Rows)
                {
                    worksheet.Cells[j, 1] = j - 1;
                    worksheet.Cells[j, 2] = item["EmpCode"].ToString();
                    worksheet.Cells[j, 3] = item["EmpName"].ToString();
                    worksheet.Cells[j, 4] = item["Designation"].ToString();
                    worksheet.Cells[j, 5] = item["Department"].ToString();
                    worksheet.Cells[j, 6] = item["Section"].ToString();
                    worksheet.Cells[j, 7] = item["Project"].ToString();
                    worksheet.Cells[j, 8] = item["TransactionType"].ToString();
                    worksheet.Cells[j, 9] = item["Amount"].ToString();
                    worksheet.Cells[j, 10] = fname;
                    worksheet.Cells[j, 11] = item["SalaryType"].ToString(); ;
                    worksheet.Cells[j, 12] = item["FYDId"].ToString();
                    worksheet.Cells[j, 13] = item["SDId"].ToString();
                    j++;
                }
                #endregion
                #endregion
                string xportFileName = string.Format(@"{0}" + FileName, Filepath);
                // save the application
                workbook.SaveAs(xportFileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing,
                                Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing);
                // Exit from the application
                app.Quit();
                releaseObject(worksheet);
                releaseObject(workbook);
                releaseObject(app);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return true;
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
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
            }
            finally
            {
                GC.Collect();
            }
        }
        #endregion ImportExport
        //==================SelectAllForReport=================
        public List<SalaryEarningDetailVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string Orderby)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryEarningDetailVM> VMs = new List<SalaryEarningDetailVM>();
            SalaryEarningDetailVM vm;
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
sed.Id
,sed.Amount
,sed.SalaryType SalarytypeName
,sed.SalaryTypeId
,est.Name SalaryName
,sed.PeriodName
,sed.EmpName
,sed.Code
,sed.Designation
,sed.Department
,sed.Section
,sed.Project
,sed.JoinDate
,sed.BasicSalary
,sed.GrossSalary
,sed.DepartmentId
,sed.DesignationId
,sed.ProjectId
,sed.SectionId
,sed.FiscalYearDetailId
,fyd.PeriodStart                 
from ViewSalaryEarningDetail sed
left outer join EnumSalaryType est on sed.SalaryTypeId=est.Id
left outer join grade g on sed.gradeId = g.id
left outer join FiscalYearDetail fyd on sed.FiscalYearDetailId =fyd.Id
Where 1=1 and sed.Amount >0 and sed.IsEarning=1 and est.name != 'Gross'
";

                if (fid != 0)
                {
                    sqlText += @" and sed.FiscalYearDetailId>='" + fid + "'";
                }
                if (fidTo != 0)
                {
                    sqlText += @" and sed.FiscalYearDetailId<='" + fidTo + "'";
                }
                if (ProjectId != "0_0")
                    sqlText += " and sed.ProjectId=@ProjectId";

                if (DepartmentId != "0_0")
                    sqlText += " and sed.DepartmentId=@DepartmentId ";

                if (SectionId != "0_0")
                    sqlText += " and sed.SectionId=@SectionId ";

                if (DesignationId != "0_0")
                    sqlText += " and sed.DesignationId=@DesignationId ";

                if (CodeF != "0_0")
                    sqlText += " and sed.Code>= @CodeF";

                if (CodeT != "0_0")
                    sqlText += " and sed.Code<= @CodeT";
                //sqlText += " ORDER BY  sed.FiscalYearDetailId, sed.Department, sed.Section, sed.Code, est.SL ";
                sqlText += " ORDER BY sed.FiscalYearDetailId";

                if (Orderby == "DCG")
                    sqlText += " , sed.department, sed.code, g.sl";
                else if (Orderby == "DDC")
                    sqlText += " ,sed.department, sed.JoinDate, sed.code";
                else if (Orderby == "DGC")
                    sqlText += " , sed.department, g.sl, sed.code";
                else if (Orderby == "DGDC")
                    sqlText += ", sed.department, g.sl, sed.JoinDate, sed.code";
                else if (Orderby == "CODE")
                    sqlText += ", sed.code";

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
                    vm = new SalaryEarningDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();

                    vm.Amount = Convert.ToDecimal(dr["Amount"].ToString());
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.SalaryTypeName = dr["SalarytypeName"].ToString();
                    vm.SalaryTypeId = dr["SalaryTypeId"].ToString();
                    vm.SalaryName = dr["SalaryName"].ToString();
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

        public DataSet SalarySheet(SalarySheetVM vm)
        {
            #region Variables



            DataSet ds = new DataSet();
          

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statement

                sqlText =
                    @"
                          create table #TempSalaey(
Id int identity(1,1) ,
[FiscalYearDetailId] varchar(100)  
,[EmployeeId]	varchar(100)   
,[Arrear]				 int   
,[LeaveEncash]			 int   
,[HouseRent]			 int   
,[AbsentDeduction]		 int   
,[TAX]					 int   
,[TransportBill]		 int   
,[PFEmployer]			 int   
,[ReimbursableExpense]	 int   
,[ChildAllowance]		 int   
,[Other(EL)]			 int   
,[HARDSHIP]			 int	   
,[Other(Earning)]		 int   
,[Other(StaffWF)]		 int   
,[Vehicle(Adj)]		 int	   
,[Stamp]				 int   
,[STAFFWELFARE]		 int	   
,[AdvanceDeduction]	 int	   
,[PFEmployee]			 int   
,[Other(Salary)]		 int   
,[Other(PF)]			 int   
,[MobileBill]			 int   
,[Basic]				 int   
,[MOBILE(Allowance)]	 int   
,[Punishment]			 int   
,[PreEmploymentCheckUp] int	   
,[Medical]				 int   
,[Other(Bonus)]		 int	   
,[Travel]				 int   
,[TransportAllowance]	 int   
,[Othere(OT)]			 int   
,[Other(Deduction)]	 int	   
,[LeaveWOPay]			 int   
,[Gross]				 int   
)
insert into #TempSalaey
exec ProSalarySheet
select 
e.Code
,e.EmpName
,e.JoinDate
,e.LeftDate
,e.Project
,e.Branch
,e.Department
,e.Section
,e.Designation
,e.Grade
,e.StepName
,e.Email
,e.TIN
,e.BankAccountName
,e.BankAccountNo
,e.BankName
,e.AccountType
,e.Routing_No
,[FiscalYearDetailId]	
,s.[EmployeeId]
,isnull([Arrear]					,0)[Arrear]
,isnull([LeaveEncash]				,0)[LeaveEncash]
,isnull([HouseRent]				,0)[HouseRent]
,isnull([AbsentDeduction]			,0)[AbsentDeduction]
,isnull([TAX]						,0)[TAX]
,isnull([TransportBill]			,0)[TransportBill]
,isnull([PFEmployer]				,0)[PFEmployer]
,isnull([ReimbursableExpense]		,0)[ReimbursableExpense]
,isnull([ChildAllowance]			,0)[ChildAllowance]
,isnull([Other(EL)]				,0)[Other(EL)]
,isnull([HARDSHIP]					,0)[HARDSHIP]
,isnull([Other(Earning)]			,0)[Other(Earning)]
,isnull([Other(StaffWF)]			,0)[Other(StaffWF)]
,isnull([Vehicle(Adj)]				,0)[Vehicle(Adj)]
,isnull([Stamp]					,0)[Stamp]
,isnull([STAFFWELFARE]				,0)[STAFFWELFARE]
,isnull([AdvanceDeduction]			,0)[AdvanceDeduction]
,isnull([PFEmployee]				,0)[PFEmployee]
,isnull([Other(Salary)]			,0)[Other(Salary)]
,isnull([Other(PF)]				,0)[Other(PF)]
,isnull([MobileBill]				,0)[MobileBill]
,isnull([Basic]					,0)[Basic]
,isnull([MOBILE(Allowance)]		,0)[MOBILE(Allowance)]
,isnull([Punishment]				,0)[Punishment]
,isnull([PreEmploymentCheckUp]		,0)[PreEmploymentCheckUp]
,isnull([Medical]					,0)[Medical]
,isnull([Other(Bonus)]				,0)[Other(Bonus)]
,isnull([Travel]					,0)[Travel]
,isnull([TransportAllowance]		,0)[TransportAllowance]
,isnull([Othere(OT)]				,0)[Othere(OT)]
,isnull([Other(Deduction)]			,0)[Other(Deduction)]
,isnull([LeaveWOPay]				,0)[LeaveWOPay]
,isnull([Gross]					,0)[Gross]

,s.* from #TempSalaey s
left outer join ViewEmployeeInformation e on s.EmployeeId=e.EmployeeId
where FiscalYearDetailId=@FiscalYearDetailId
drop table #TempSalaey";

               
                #endregion

                #region SQL Command

                SqlCommand objCommSaleReport = new SqlCommand();
                objCommSaleReport.Connection = currConn;

                objCommSaleReport.CommandText = sqlText;
                objCommSaleReport.CommandType = CommandType.Text;

                #endregion

                #region Parameters
                objCommSaleReport.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
               
                #endregion


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleReport);
                dataAdapter.Fill(ds);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("ReportDSDAL", "MonthlySales", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ReportDSDAL", "MonthlySales", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return ds;
        }

        //#endregion
    }
}


