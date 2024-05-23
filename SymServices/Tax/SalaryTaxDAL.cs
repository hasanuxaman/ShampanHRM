using Excel;
using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using OfficeOpenXml;


namespace SymServices.Tax
{
    public class SalaryTaxDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion
        #region Methods


        public DataTable Report(ParameterVM paramVM, string[] conditionFields = null, string[] conditionValues = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            System.Data.DataTable dt = new System.Data.DataTable();

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
                //std.Id
                //std.FiscalYearDetailId     
                //,std.ProjectId                                
                //,std.SectionId
                //,fyd.PeriodStart                                          
                //,std.Code 
                //,std.Section                             
                //,std.Project   
                //,std.JoinDate                                   
                //,std.BasicSalary                             
                //,std.GrossSalary                              
                //,std.DepartmentId                             
                //,std.DesignationId

                sqlText = @"
SELECT   
std.Code 
,std.EmpName   
,std.Designation 
,std.Department  
,std.TaxValue 
,std.PeriodName 
                           
from  ViewSalaryTaxDetail std
left outer join grade g on std.gradeId = g.id
left outer join FiscalYearDetail fyd on std.FiscalYearDetailId =fyd.Id
Where 1=1 and std.TaxValue >0
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
                sqlText += " ORDER BY std.FiscalYearDetailId";

                if (paramVM.Orderby == "DCG")
                    sqlText += " , std.department, std.code, g.sl";
                else if (paramVM.Orderby == "DDC")
                    sqlText += " ,std.department, std.JoinDate, std.code";
                else if (paramVM.Orderby == "DGC")
                    sqlText += " , std.department, g.sl, std.code";
                else if (paramVM.Orderby == "DGDC")
                    sqlText += ", std.department, g.sl, std.JoinDate, std.code";
                else if (paramVM.Orderby == "CODE")
                    sqlText += ", std.code";


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

            return dt;
        }

        public ResultVM DownloadExcel(ParameterVM paramVM)
        {
            #region Variables
            ResultVM rVM = new ResultVM();

            DataTable dt = new DataTable();

            #endregion
            try
            {

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                string[] cFields = { "std.FiscalYearDetailId>", "std.FiscalYearDetailId<", "std.Code>", "std.Code<", "std.DepartmentId", "std.DesignationId", "std.SectionId", "std.ProjectId" };
                string[] cValues = { paramVM.FiscalPeriodDetailId, paramVM.FiscalPeriodDetailIdTo, paramVM.CodeF, paramVM.CodeT, paramVM.DepartmentId, paramVM.DesignationId, paramVM.SectionId, paramVM.ProjectId };

                 //,std.ProjectId                                
                //,std.SectionId
                //,std.Code 
                //,std.DepartmentId                             
                //,std.DesignationId

                dt = Report(paramVM, cFields, cValues);

                #region Add SL Column
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (!dt.Columns.Contains("SL"))
                    {
                        dt.Columns.Add("SL").SetOrdinal(0);
                        int i = 1;
                        foreach (DataRow dr in dt.Rows)
                        {
                            dr["SL"] = i;
                            i++;
                        }
                    }

                    if (dt.Columns.Contains("PeriodName"))
                    {
                        string FullPeriodName = "";
                        FullPeriodName = Convert.ToDateTime("01-" + dt.Rows[0]["PeriodName"]).ToString("MMMM-yyyy");
                        rVM.ReportName = "SalaryTax - " + FullPeriodName;
                    }

                }

                #endregion

                #region Prepare Excel

                ExcelPackage excel = new ExcelPackage();

                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

                int TableHeadRow = 1;
                workSheet.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                rVM.excel = excel;

                rVM.Status = "Success";
                rVM.Message = "Successfully~Data Download";

                #endregion
            }
            catch (Exception ex)
            {
                rVM.Exception = ex.Message;
            }
            finally { }

            return rVM;


        }
        public string[] UpsateSalaryTaxNew(int FiscalYearDetailId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Employee Salary Tax Process"; //Method Name
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


                #region SqlText
                var sqlTextSalaryTaxDetail = "";
                sqlTextSalaryTaxDetail = @"
Update SalaryTaxDetail set TaxValue= 
EmpTaxValue from EmployeeStructureGroup where SalaryTaxDetail.EmployeeId=EmployeeStructureGroup.EmployeeId and SalaryTaxDetail.FiscalYearDetailId=@FiscalYearDetailId";
                SqlCommand cmdempSalaryTax;
                cmdempSalaryTax = new SqlCommand(sqlTextSalaryTaxDetail, currConn, transaction);
                cmdempSalaryTax.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdempSalaryTax.ExecuteNonQuery();
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
        //==================Insert =================
        public string[] InsertSalaryTaxNew(int FiscalYearDetailId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
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
            retResults[5] = "Employee Salary Tax Process"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            List<EmployeeInfoVM> employeeVms = new List<EmployeeInfoVM>();
            EmployeeInfoVM employeeVm;

            EmployeeInfoDAL _EmployeeInfoDAL = new EmployeeInfoDAL();

            string PeriodEnd = "";
            string NoAssignCode = "";
            string PeriodStart = "";

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
                varEmployeeInfoVM.EmploymentType = EmpType;
                varEmployeeInfoVM.CompanyName = CompanyName;
                varEmployeeInfoVM.FiscalYearDetailId = FiscalYearDetailId;

                employeeVms = _EmployeeInfoDAL.SelectAllEmployee_SalaryProcess(varEmployeeInfoVM, currConn, transaction);

                #endregion

                #endregion

                #region Comments



                ////                sqlText = @"
                ////    select  *
                ////      from  
                ////    ViewEmployeeInformation  
                ////    where 1=1
                ////    and  BranchId = @BranchId
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
                #endregion Comments

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
                    //retResults[1] = "This Employee have not assigh Salary Structure yet, Code : " + NoAssignCode;
                    //retResults[3] = sqlText;
                    //throw new ArgumentNullException("This Employee have not assigh Salary Structure yet, Code : " + NoAssignCode, "");
                }
                #endregion Employee No Salary Structure Assign Check

                #region SqlText
                var sqlTextSalaryTaxDetail = "";
                sqlTextSalaryTaxDetail = @"
Insert Into SalaryTaxDetail

(
 FiscalYearDetailId,TaxStructureId,ProjectId,DepartmentId,SectionId,DesignationId,EmployeeId,TaxValue
,GrossSalary,BasicSalary,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,EmployeeStatus,GradeId
) Values (
 @FiscalYearDetailId,@TaxStructureId,@ProjectId,@DepartmentId,@SectionId,@DesignationId,@EmployeeId
,@TaxValue,@GrossSalary,@BasicSalary,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@EmployeeStatus,@GradeId
)";
                #endregion

                if (employeeVms.Count > 0)
                {
                    foreach (EmployeeInfoVM employee in employeeVms)
                    {
                        #region Variables

                        bool haveTAX = false;
                        decimal TaxValue = 0;
                        bool IsFixed = false;
                        string PortionSalaryType = "BASIC";
                        string TaxStructureId = "0";

                        #endregion

                        #region Delete ExistingSalaryTaxDetail
                        sqlText = @"Delete SalaryTaxDetail ";
                        sqlText += " where 1=1 and EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId";

                        SqlCommand cmdDeletePFDetail = new SqlCommand(sqlText, currConn, transaction);
                        cmdDeletePFDetail.Parameters.AddWithValue("@EmployeeId", employee.Id);
                        cmdDeletePFDetail.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                        cmdDeletePFDetail.ExecuteNonQuery();

                        #endregion

                        #region EmployeeTax
                        sqlText = @"select * from EmployeeTax
                            where EmployeeId=@EmployeeId";
                        SqlCommand cmdepf = new SqlCommand(sqlText, currConn, transaction);
                        cmdepf.Parameters.AddWithValue("@EmployeeId", employee.Id);
                        using (SqlDataReader drepf = cmdepf.ExecuteReader())
                        {
                            while (drepf.Read())
                            {
                                TaxValue = Convert.ToDecimal(drepf["TaxValue"]);
                                IsFixed = Convert.ToBoolean(drepf["IsFixed"]);
                                PortionSalaryType = drepf["PortionSalaryType"].ToString();
                                TaxStructureId = drepf["TaxStructureId"].ToString();
                                haveTAX = true;
                            }
                            drepf.Close();
                        }
                        #endregion EmployeeTax

                        if (haveTAX)
                        {
                            #region Tax Calculation

                            if (!IsFixed)
                            {
                                TaxValue = TaxValue / 100;
                                if (PortionSalaryType.ToUpper().Trim() == "GROSS")
                                {
                                    TaxValue = TaxValue * employee.GrossSalary;
                                }
                                else
                                {
                                    TaxValue = TaxValue * employee.BasicSalary;
                                }
                            }
                            #endregion

                            #region Sql Execution

                            SqlCommand cmdempSalaryTax;

                            cmdempSalaryTax = new SqlCommand(sqlTextSalaryTaxDetail, currConn, transaction);
                            cmdempSalaryTax.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                            cmdempSalaryTax.Parameters.AddWithValue("@TaxStructureId", TaxStructureId);
                            cmdempSalaryTax.Parameters.AddWithValue("@ProjectId", employee.ProjectId);
                            cmdempSalaryTax.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
                            cmdempSalaryTax.Parameters.AddWithValue("@SectionId", employee.SectionId);
                            cmdempSalaryTax.Parameters.AddWithValue("@DesignationId", employee.DesignationId);
                            cmdempSalaryTax.Parameters.AddWithValue("@EmployeeId", employee.Id);
                            cmdempSalaryTax.Parameters.AddWithValue("@TaxValue", TaxValue);
                            cmdempSalaryTax.Parameters.AddWithValue("@GrossSalary", employee.GrossSalary);
                            cmdempSalaryTax.Parameters.AddWithValue("@BasicSalary", employee.BasicSalary);
                            cmdempSalaryTax.Parameters.AddWithValue("@Remarks", "-");
                            cmdempSalaryTax.Parameters.AddWithValue("@IsActive", true);
                            cmdempSalaryTax.Parameters.AddWithValue("@IsArchive", false);
                            cmdempSalaryTax.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                            cmdempSalaryTax.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                            cmdempSalaryTax.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                            cmdempSalaryTax.Parameters.AddWithValue("@EmployeeStatus", EmpType);
                            cmdempSalaryTax.Parameters.AddWithValue("@GradeId", employee.GradeId);
                            cmdempSalaryTax.ExecuteNonQuery();

                            #endregion
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
        public string[] SalaryTaxSingleAdd(EmployeeInfoVM vm, int branchId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Employee Salary Tax Process"; //Method Name
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
        public string[] SalaryTaxSingleAddorUpdate(SalaryTaxDetailVM vm, int branchId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Employee Salary Tax Process Single"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            System.Data.DataTable dsSS = new System.Data.DataTable();
            string EmployeeStatus = "";
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

                sqlText = @"select * from SalaryTaxDetail ";
                sqlText += " where FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " and EmployeeId=@EmployeeId";


                if (vm.EmployeeId == "1_1")
                {
                    int a = 0;
                }

                SqlCommand objss = new SqlCommand(sqlText, currConn, transaction);
                //objss.Parameters.AddWithValue("@Id", SalaryStructureId);
                objss.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                objss.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                SqlDataAdapter daSS = new SqlDataAdapter(objss);
                daSS.Fill(dsSS);
                foreach (DataRow item in dsSS.Rows)
                {
                    EmployeeStatus = item["EmployeeStatus"].ToString();
                }

                sqlText = @"Delete SalaryTaxDetail ";
                sqlText += " where FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " and EmployeeId=@EmployeeId";

                SqlCommand cmdDeletePrevious = new SqlCommand(sqlText, currConn, transaction);
                cmdDeletePrevious.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                cmdDeletePrevious.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdDeletePrevious.ExecuteNonQuery();


                EmployeeInfoVM employeeVm = null;
                sqlText = @"
SELECT Top 1
e.Id,pf.EmployeeId,pf.TaxStructureId,e.GradeId,pf.TaxValue,pf.IsFixed,isnull(nullif(pf.PortionSalaryType,''),'NA')PortionSalaryType
,e.ProjectId,e.DepartmentId,e.SectionId,e.DesignationId,e.GrossSalary,e.BasicSalary
  from EmployeeTax pf
left outer join 
ViewEmployeeInformation e on pf.EmployeeId=e.Id
where 1=1
--and pf.IsArchive=0 and pf.isactive=1
--and  e.IsArchive=0 and e.isactive=1

and e.Id=@EmployeeID";
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
                        employeeVm.SectionId = dr["SectionId"].ToString();
                        employeeVm.GradeId = dr["GradeId"].ToString();
                        employeeVm.DesignationId = dr["DesignationId"].ToString();
                        employeeVm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        employeeVm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);

                        employeeVm.TaxValue = Convert.ToDecimal(dr["TaxValue"]);
                        employeeVm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                        employeeVm.PortionSalaryType = dr["PortionSalaryType"].ToString();
                        employeeVm.TaxStructureId = dr["TaxStructureId"].ToString();
                    }
                }

                sqlText = @"Insert Into SalaryTaxDetail

(
FiscalYearDetailId
,TaxStructureId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,EmployeeId
,TaxValue
,GradeId
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
,@TaxStructureId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@EmployeeId
,@TaxValue
,@GradeId
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
                SqlCommand cmdSalaryTaxDet;
                if (employeeVm != null)
                {
                    decimal TaxValue = 0;

                    TaxValue = employeeVm.TaxValue;
                    if (!employeeVm.IsFixed)
                    {
                        //TaxValue = TaxValue; // / 100;
                        //if (employeeVm.PortionSalaryType.ToUpper().Trim() == "GROSS")
                        //{
                        //    TaxValue = TaxValue * employeeVm.GrossSalary;
                        //}
                        //else
                        //{
                        //    TaxValue = TaxValue * employeeVm.BasicSalary;
                        //}
                    }


                    cmdSalaryTaxDet = new SqlCommand(sqlText, currConn, transaction);

                    cmdSalaryTaxDet.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@TaxStructureId", employeeVm.TaxStructureId);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@ProjectId", employeeVm.ProjectId);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@DepartmentId", employeeVm.DepartmentId);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@SectionId", employeeVm.SectionId);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@DesignationId", employeeVm.DesignationId);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@EmployeeId", employeeVm.Id);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@TaxValue", vm.TaxValue);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@GradeId", employeeVm.GradeId);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@EmployeeStatus", EmployeeStatus);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@GrossSalary", employeeVm.GrossSalary);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@BasicSalary", employeeVm.BasicSalary);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@Remarks", "-");
                    cmdSalaryTaxDet.Parameters.AddWithValue("@IsActive", true);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@IsArchive", false);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdSalaryTaxDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdSalaryTaxDet.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "Have no structure";
                    throw new ArgumentException("have no data", "have no data");
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
        public List<SalaryTaxVM> GetPeriodNameDistrinct()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryTaxVM> VMs = new List<SalaryTaxVM>();
            SalaryTaxVM vm;
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
,std.FiscalYearDetailId,
std.Remarks
from  ViewSalaryTaxDetail std 
left outer join FiscalYearDetail fyd on std.FiscalYearDetailId =fyd.Id
Where std.IsArchive=0 And std.IsActive=1 and std.TaxValue >=0
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
                    vm = new SalaryTaxVM();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
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
        public List<SalaryTaxVM> SelectAll(int BranchId, int? fid = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryTaxVM> VMs = new List<SalaryTaxVM>();
            SalaryTaxVM vm;
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
std.Id                                                                 
,std.Remarks                               
,std.TaxValue                               
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
,std.FiscalYearDetailId                      
,vw.Code                                     
from  SalaryTaxDetail std
left outer join ViewEmployeeInformation vw on std.EmployeeId=vw.id
left outer join FiscalYearDetail fyd on std.FiscalYearDetailId =fyd.Id
Where vw.IsArchive=0 and vw.IsActive=1 and std.IsArchive=0 And std.IsActive=1 and std.TaxValue >=0
";
                if (fid != null && fid != 0)
                {
                    sqlText += @" and std.FiscalYearDetailId='" + fid + "'";
                }
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryTaxVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.TaxValue = Convert.ToDecimal(dr["TaxValue"].ToString());
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.Code = dr["Code"].ToString();
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
        public List<SalaryTaxDetailVM> SelectAllSalaryTaxDetails(int fid, SqlConnection currConn, bool callFromOutSide)
        {

            #region Variables

            string sqlText = "";
            List<SalaryTaxDetailVM> VMs = new List<SalaryTaxDetailVM>();
            SalaryTaxDetailVM vm;
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
,code
,EmpName
,FiscalYearDetailId
,EmployeeId
,GrossSalary
,BasicSalary
,TaxValue
,Remarks
from ViewSalaryTaxDetail
Where 1=1 and FiscalYearDetailId=@FiscalYearDetailId and IsArchive=0
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@FiscalYearDetailId", fid);

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new SalaryTaxDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                        vm.TaxValue = Convert.ToDecimal(dr["TaxValue"]);
                        vm.Remarks = dr["Remarks"].ToString();
                        vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
                        vm.EmpName = dr["EmpName"].ToString();
                        vm.Code = dr["Code"].ToString();
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
        public string[] SalaryTaxDetailsDelete(string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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


                transaction = currConn.BeginTransaction("DeleteToSalaryTaxD");

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "Delete SalaryTaxDetail where Id=@Id";

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
                    //    throw new ArgumentNullException("Salary Tax Delete", vm.Id + " could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Tax Details Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary Tax Details.";
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
        public string[] SalaryTaxDelete(string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSalaryTax"; //Method Name

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

                transaction = currConn.BeginTransaction("DeleteToSalaryTax");

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "Delete SalaryTax where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        #region Details
                        sqlText = "";
                        sqlText = "Delete SalaryTaxDetail where SalaryTaxId=@Id";

                        SqlCommand cmdUpdate2 = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate2.Parameters.AddWithValue("@Id", Ids[i]);
                        transResult = (int)cmdUpdate2.ExecuteNonQuery();
                        #endregion
                    }




                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException("Salary Tax Delete"," could not Delete.");
                    //}

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Tax Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary Tax.";
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
        public SalaryTaxDetailVM GetByIdSalaryTaxDetails(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SalaryTaxDetailVM vm = new SalaryTaxDetailVM();

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
 sTaxd.Id

,sTaxd.GrossSalary
,sTaxd.BasicSalary
,sTaxd.TaxValue
,sTaxd.Remarks
,e.Salutation_E
,e.MiddleName
,e.LastName
,sTaxd.EmployeeId
,sTaxd.FiscalYearDetailId
 from SalaryTaxDetail sTaxd
 left join EmployeeInfo e on e.Id=sTaxd.EmployeeId
 where sTaxd.Id=@Id
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
                    vm.EmployeeId = dr["EmployeeId"].ToString();

                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.TaxValue = Convert.ToDecimal(dr["TaxValue"]);
                    vm.Remarks = dr["Remarks"].ToString();

                    vm.EmployeeName = dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();

                    vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
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
        public SalaryTaxDetailVM GetByEmpIdandFdidSalaryTaxDetails(string empid, int fid)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SalaryTaxDetailVM vm = new SalaryTaxDetailVM();

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
 sTaxd.Id
,sTaxd.GrossSalary
,sTaxd.BasicSalary
,sTaxd.TaxValue
,sTaxd.Remarks
,e.Salutation_E
,e.MiddleName
,e.LastName
,sTaxd.EmployeeId
,sTaxd.FiscalYearDetailId
 from SalaryTaxDetail sTaxd
 left join EmployeeInfo e on e.Id=sTaxd.EmployeeId
 where sTaxd.EmployeeId=@EmployeeId and sTaxd.FiscalYearDetailId=@FiscalYearDetailId";
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
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.TaxValue = Convert.ToDecimal(dr["TaxValue"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.EmployeeName = dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                    vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
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
        public string[] SalaryTaxSingleEdit(SalaryTaxDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Employee Salary Tax Process Single Edit"; //Method Name

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

                sqlText = @"Update  SalaryTaxDetail

set
 TaxValue            =@TaxValue
,Remarks            =@Remarks
,LastUpdateBy       =@LastUpdateBy
,LastUpdateAt       =@LastUpdateAt
,LastUpdateFrom     =@LastUpdateFrom
where Id=@Id
";

                SqlCommand cmdempBonusDet;
                cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                cmdempBonusDet.Parameters.AddWithValue("@Id", vm.Id);
                cmdempBonusDet.Parameters.AddWithValue("@TaxValue", vm.TaxValue);
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
        public string GetPeriodName(string SalaryTaxId)
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

                sqlText = @"select f.PeriodName from SalaryTax t 
join FiscalYearDetail f on f.id=t.FiscalYearDetailId
where t.Id=@SalaryTaxId";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@SalaryTaxId", SalaryTaxId);
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
        public string[] ImportExcelFile(string fullPath, string fileName, ShampanIdentityVM auditvm, int branchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, int FYDId = 0)
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
            TaxStructureDAL taxdal = new TaxStructureDAL();
            FiscalYearDetailVM FYDVM = new FiscalYearDetailVM();
            #region try
            try
            {
                #region Reading Excel
                DataSet ds = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
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
                var a = "";
                SalaryTaxDetailVM vm = new SalaryTaxDetailVM();

                foreach (DataRow item in dt.Rows)
                {
                    vm = new SalaryTaxDetailVM();
                    empVM = _dalemp.ViewSelectAllEmployee(item["EmpCode"].ToString(), null, null, null, null, null, null, currConn, transaction).FirstOrDefault();

                    string empcode = item["EmpCode"].ToString();
                    if (empcode == "BES-00005")
                    {
                        int ab = 0;
                    }


                    if (empVM == null || empVM.Id == null)
                    {
                        throw new ArgumentNullException("Employee Code " + item["EmpCode"].ToString() + " Not in System", "Employee Code " + item["EmpCode"].ToString() + " Not in System");
                    }
                    else
                    {
                        FYDVM = fydal.FYPeriodDetail(FYDId, currConn, transaction).FirstOrDefault();
                        ////FYDVM = fydal.FYPeriodDetail(Convert.ToInt32(item["FYDId"].ToString()), currConn, transaction).FirstOrDefault();
                        if (FYDVM == null)
                        {
                            throw new ArgumentNullException("Fiscal Period" + item["FYDId"].ToString() + " Not in System", "Fiscal Period " + item["FYDId"].ToString() + " Not in System");
                        }
                        else
                        {
                            vm.TaxStructureId = taxdal.SelectById(item["TAXId"].ToString()).Id;
                            if (vm.TaxStructureId == null)
                            {
                                throw new ArgumentNullException("Salary TAX Fund not in System,", "Fiscal Period " + item["EDId"].ToString() + " Not in System");
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
                                    vm.FiscalYearDetailId = Convert.ToString(FYDVM.Id);
                                    vm.TaxValue = Convert.ToDecimal(item["Amount"]);
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
                                    retResults = SalaryTaxSingleAddorUpdate(vm, branchId, VcurrConn, Vtransaction);
                                    if (retResults[0] != "Success")
                                    {
                                        throw new ArgumentNullException("Salary Other Earning Update", "Could not found any item.");
                                    }
                                }
                            }
                        }
                    }
                }

                retResults = new SalaryProcessDAL().UpdateCurrentEmployee(vm.FiscalYearDetailId, VcurrConn, Vtransaction);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException("Salary Tax Update", "Current Employee Update Fail!");
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
vw.TaxValue Amount,FiscalYeardetailId FYDId,cast(vw.TaxStructureId as varchar) TaxId
, vw.EmployeeId
from ViewSalaryTaxDetail vw
left outer join  TaxStructure st on st.Id=TaxStructureId
left outer join  dbo.ViewEmployeeInformation vws on vws.EmployeeId=vw.EmployeeId
left outer join grade g on vws.gradeId = g.id
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
                #region Resign From This Month or Later
                sqlText += @"
UNION ALL
select vw.Code EmpCode,vw.EmpName,ISNULL(vw.Grade+'-'+vw.StepName,'NA') Grade,
(case when vw.Designation is null then 'NA' when vw.Designation='=NA=' then 'NA' else vw.Designation end) Designation,
(case when vw.Department is null then 'NA' when vw.Department='=NA=' then 'NA' else vw.Department end) Department ,
(case when vw.Section is null then 'NA' when vw.Section='=NA=' then 'NA' else vw.Section end) Section,
(case when vw.Project is null then 'NA' when vw.Project='=NA=' then 'NA' else vw.Project end) Project,
vw.TaxValue Amount,FiscalYeardetailId FYDId,cast(vw.TaxStructureId as varchar) TaxId
,vw.EmployeeId
from ViewSalaryTaxDetail vw
left outer join  TaxStructure st on st.Id=TaxStructureId
left outer join  dbo.ViewEmployeeInformation vws on vws.EmployeeId=vw.EmployeeId
left outer join grade g on vws.gradeId = g.id
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
                dt.Columns.Add("Fiscal Period");
                dt.Columns.Add("Type");

                if (dt.Rows.Count == 0)
                    throw new ArgumentNullException("TaxStructure has not been assign any employee ");
                foreach (DataRow row in dt.Rows)
                {
                    row["Fiscal Period"] = fname;
                    row["Type"] = "TAX";
                    row["FYDId"] = fid;
                }
                //var tt = Ordinary.WriteDataTableToExcel(dt, "DataSheet", Filepath + FileName);

                //if (tt == false)
                //{
                //    retResults[0] = "Fail";
                //    retResults[1] = "Data Download UnSuccessfully.";
                //}
                //{
                //    retResults[0] = "Success";
                //    retResults[1] = "Data Download Successfully.";
                //}
                #endregion


                #endregion

                #region Value Round

                string[] columnNames = { "Amount" };

                dt = Ordinary.DtValueRound(dt, columnNames);

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
        public List<SalaryTaxDetailVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string Orderby)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryTaxDetailVM> VMs = new List<SalaryTaxDetailVM>();
            SalaryTaxDetailVM vm;
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
std.Id     
,std.FiscalYearDetailId     
,fyd.PeriodStart                 
,std.TaxValue                               
,std.PeriodName  
,std.EmpName                                
,std.Code 
,std.Designation                              
,std.Department  
,std.Section                             
,std.Project   
,std.JoinDate                                   
,std.BasicSalary                             
,std.GrossSalary                              
,std.DepartmentId                             
,std.DesignationId                           
,std.ProjectId                                
,std.SectionId   
,std.Remarks                                
from  ViewSalaryTaxDetail std
left outer join grade g on std.gradeId = g.id
left outer join FiscalYearDetail fyd on std.FiscalYearDetailId =fyd.Id
Where std.IsArchive=0 And std.IsActive=1 and std.TaxValue >0
";
                //fidTo, 
                if (fid != 0)
                {
                    sqlText += @" and std.FiscalYearDetailId>='" + fid + "'";
                }
                if (fidTo != 0)
                {
                    sqlText += @" and std.FiscalYearDetailId<='" + fidTo + "'";
                }
                if (ProjectId != "0_0")
                    sqlText += " and std.ProjectId=@ProjectId";

                if (DepartmentId != "0_0")
                    sqlText += " and std.DepartmentId=@DepartmentId ";

                if (SectionId != "0_0")
                    sqlText += " and std.SectionId=@SectionId ";

                if (DesignationId != "0_0")
                    sqlText += " and std.DesignationId=@DesignationId ";

                if (CodeF != "0_0")
                    sqlText += " and std.Code>= @CodeF";

                if (CodeT != "0_0")
                    sqlText += " and std.Code<= @CodeT";
                //sqlText += " order by std.FiscalYearDetailId, std.Department, std.Section, std.Code ";
                sqlText += " ORDER BY std.FiscalYearDetailId";

                if (Orderby == "DCG")
                    sqlText += " , std.department, std.code, g.sl";
                else if (Orderby == "DDC")
                    sqlText += " ,std.department, std.JoinDate, std.code";
                else if (Orderby == "DGC")
                    sqlText += " , std.department, g.sl, std.code";
                else if (Orderby == "DGDC")
                    sqlText += ", std.department, g.sl, std.JoinDate, std.code";
                else if (Orderby == "CODE")
                    sqlText += ", std.code";
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
                    vm = new SalaryTaxDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.TaxValue = Convert.ToDecimal(dr["TaxValue"].ToString());
                    vm.PeriodName = dr["PeriodName"].ToString();
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

        #endregion
    }
}
