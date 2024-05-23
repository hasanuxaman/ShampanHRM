using Excel;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace SymServices.Payroll
{
    public class EmployeeEarningLeaveDAL
    {
        #region Global Variables

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL cdal = new CommonDAL();

        #endregion

        //#region Methods


        //==================Get All Distinct FiscalPeriodName =================
        public List<EmployeeEarningLeaveVM> GetPeriodname()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeEarningLeaveVM> vms = new List<EmployeeEarningLeaveVM>();
            EmployeeEarningLeaveVM vm;

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
select distinct fyd.Id
,fyd.PeriodName
,fyd.PeriodStart
,ve.Remarks 
from EmployeeEarningLeave ve
left outer join  FiscalYearDetail fyd on ve.FiscalYearDetailId=fyd.Id 
where 1=1 
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
                    vm = new EmployeeEarningLeaveVM();
                    vm.Id = (dr["Id"]).ToString();
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

        //==================SelectAll=================
        public List<EmployeeEarningLeaveVM> SelectAll(string empid = null, int? fid = null, int? DTId = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeEarningLeaveVM> vms = new List<EmployeeEarningLeaveVM>();
            EmployeeEarningLeaveVM vm;

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
ea.Id
,ea.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,ea.DeductionAmount
,ea.FiscalYearDetailId
,ea.SalaryPreiodId
,fs.PeriodName
,ea.DeductionDate
--,ea.DeductionTypeId
--,edt.Name DeductionType

,e.DesignationId, e.DepartmentId, e.SectionId, e.ProjectId

,ea.Remarks
,ea.IsActive
,ea.IsArchive
,ea.CreatedBy
,ea.CreatedAt
,ea.CreatedFrom
,ea.LastUpdateBy
,ea.LastUpdateAt
,ea.LastUpdateFrom
From EmployeeEarningLeave ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
left outer join  FiscalYearDetail fs on ea.FiscalYearDetailId=fs.Id
--left outer join EarningDeductionType edt on ea.DeductionTypeId = edt.Id and edt.IsEarning = 0 
Where 1=1 and  ea.IsArchive=0 and ea.DeductionAmount > 0
";
                if (!string.IsNullOrEmpty(empid))
                {
                    sqlText += @" and ea.EmployeeId='" + empid + "'";
                }

                if (fid != null && fid != 0)
                {
                    sqlText += @" and ea.FiscalYearDetailId='" + fid + "'";
                }

                //if (DTId != null && DTId != 0)
                //{
                //    sqlText += @" and ea.DeductionTypeId='" + DTId + "'";
                //}

                sqlText += @" ORDER BY e.Department,e.EmpName desc";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeEarningLeaveVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.SalaryPreiodId = Convert.ToInt32(dr["SalaryPreiodId"]);
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.DeductionDate = Ordinary.StringToDate(dr["DeductionDate"].ToString());
                    //vm.DeductionTypeId = Convert.ToInt32(dr["DeductionTypeId"]);
                    //vm.DeductionType = dr["DeductionType"].ToString();

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

                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();


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
        public EmployeeEarningLeaveVM SelectById(string Id)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeEarningLeaveVM vm = new EmployeeEarningLeaveVM();

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
ea.Id
,ea.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,ea.DeductionAmount
,ea.FiscalYearDetailId
,ea.SalaryPreiodId
,ea.DeductionDate
--,ea.DeductionTypeId
--,edt.Name DeductionType
,ea.GrossSalary
,ea.BasicSalary
,ea.Remarks
,ea.IsActive
,ea.IsArchive
,ea.CreatedBy
,ea.CreatedAt
,ea.CreatedFrom
,ea.LastUpdateBy
,ea.LastUpdateAt
,ea.LastUpdateFrom

,ISNULL(ea.Days,0) Days
----------,@DOM DaysOfMonth

From EmployeeEarningLeave ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
--left outer join EarningDeductionType edt on ea.DeductionTypeId = edt.Id and edt.IsEarning = 0 
Where 1=1 and ea.IsArchive=0  and ea.id=@Id and ea.DeductionAmount > 0
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
                    vm = new EmployeeEarningLeaveVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.DeductionDate = Ordinary.StringToDate(dr["DeductionDate"].ToString());
                    vm.SalaryPreiodId = Convert.ToInt32(dr["SalaryPreiodId"]);
                    //vm.DeductionType = dr["DeductionType"].ToString();

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

                    vm.Days = Convert.ToDecimal(dr["Days"]);
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

        public EmployeeEarningLeaveVM SelectEmployeeBasicSalary(string Id, string SalaryPreiodId)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeEarningLeaveVM vm = new EmployeeEarningLeaveVM();

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

select isnull (sum(case when salarytype ='Gross' then Amount else 0 end ),0)Gross
, isnull (sum(case when salarytype ='Basic' then Amount  else 0 end ),0) [Basic]
from SalaryEarningDetail where EmployeeId=@Id and FiscalYearDetailId=@SalaryPreiodId
";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@Id", Id);
                objCommVehicle.Parameters.AddWithValue("@SalaryPreiodId", SalaryPreiodId);

                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeEarningLeaveVM();

                    vm.GrossSalary = Convert.ToDecimal(dr["Gross"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["Basic"]);
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

        public EmployeeEarningLeaveVM SelectByIdandFiscalyearDetail(string empId, int FiscalYearDetailId = 0,
            string edType = "0", string SalaryPreiodId = "")
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeEarningLeaveVM vm = new EmployeeEarningLeaveVM();

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
ea.Id
,ea.EmployeeId
,ea.DeductionAmount
,ea.FiscalYearDetailId
,ea.DeductionDate
,ea.Remarks
,ISNULL(ea.GrossSalary, 0)  GrossSalary
,ISNULL(ea.BasicSalary, 0)  BasicSalary
,ISNULL(ea.Days, 0)         Days


From EmployeeEarningLeave ea 
Where ea.IsArchive=0  and ea.EmployeeId=@Id 
and ea.FiscalYearDetailId=@FiscalYearDetailId

";
                if (!string.IsNullOrEmpty(SalaryPreiodId))
                {
                    sqlText += " and SalaryPreiodId = @SalaryPreiodId";
                }


                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@Id", empId);
                objCommVehicle.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);

                if (!string.IsNullOrEmpty(SalaryPreiodId))
                {
                    objCommVehicle.Parameters.AddWithValue("@SalaryPreiodId", SalaryPreiodId);
                }


                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeEarningLeaveVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.DeductionDate = Ordinary.StringToDate(dr["DeductionDate"].ToString());
                    //vm.DeductionTypeId = Convert.ToInt32(dr["DeductionTypeId"]);
                    vm.Remarks = dr["Remarks"].ToString();

                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.Days = Convert.ToDecimal(dr["Days"]);
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

        //==================Insert =================
        public string[] Insert(EmployeeEarningLeaveVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ

            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = Id.ToString(); // Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertEmployeeEarningLeave"; //Method Name
            EmployeeInfoDAL _dal = new EmployeeInfoDAL();

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

                #region Exist

                sqlText = "  ";
                sqlText += " SELECT   count(Id) FROM EmployeeEarningLeave ";
                sqlText += @" WHERE EmployeeId=@EmployeeId and  FiscalYearDetailId=@FiscalYearDetailId";

                if (vm.SalaryPreiodId != 0)
                {
                    sqlText += " and SalaryPreiodId = '" + vm.SalaryPreiodId + "'";
                }

                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                var exeRes = cmdExist.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    sqlText = "  ";
                    sqlText += " delete FROM EmployeeEarningLeave ";
                    sqlText += @" WHERE EmployeeId=@EmployeeId and  FiscalYearDetailId=@FiscalYearDetailId";

                    if (vm.SalaryPreiodId != 0)
                    {
                        sqlText += " and SalaryPreiodId = '" + vm.SalaryPreiodId + "'";
                    }


                    SqlCommand cmdExistD = new SqlCommand(sqlText, currConn);
                    cmdExistD.Transaction = transaction;
                    cmdExistD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExistD.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                    var exeResD = cmdExistD.ExecuteScalar();
                }

                #endregion Exist

                #region Save

                vm.Id = cdal.NextId("EmployeeEarningLeave", currConn, transaction).ToString();
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeEarningLeave
(Id
, EmployeeId
, DeductionAmount
, FiscalYearDetailId
, SalaryPreiodId
, DeductionDate
, ProjectId
, SectionId
, DepartmentId
, DesignationId
, Remarks
, IsActive
, IsArchive
, CreatedBy
, CreatedAt
, CreatedFrom

, GrossSalary
, BasicSalary
, Days

)VALUES (
@Id
, @EmployeeId
, @DeductionAmount
, @FiscalYearDetailId
, @SalaryPreiodId
, @DeductionDate
, @ProjectId
, @SectionId
, @DepartmentId
, @DesignationId
, @Remarks
, @IsActive
, @IsArchive
, @CreatedBy
, @CreatedAt
, @CreatedFrom

, @GrossSalary
, @BasicSalary
, @Days
) ";


                    ///Fetching Data
                    var emp = _dal
                        .ViewSelectAllEmployee(null, null, null, null, null, null, null, null)
                        .FirstOrDefault(x => x.Id.Equals(vm.EmployeeId));

                    vm.ProjectId = emp.ProjectId;
                    vm.SectionId = emp.SectionId;
                    vm.DepartmentId = emp.DepartmentId;
                    vm.DesignationId = emp.DesignationId;

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@DeductionAmount", vm.DeductionAmount);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@SalaryPreiodId", vm.SalaryPreiodId);
                    cmdInsert.Parameters.AddWithValue("@DeductionDate", vm.DeductionDate);
                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    cmdInsert.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);
                    cmdInsert.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                    cmdInsert.Parameters.AddWithValue("@Days", vm.Days);

                    cmdInsert.Transaction = transaction;
                    int count = Convert.ToInt32(cmdInsert.ExecuteNonQuery());
                    if (count < 0)
                    {
                        throw new ArgumentNullException("Please Input EmployeeEarningLeave Value", "");
                    }
                }
                else
                {
                    retResults[1] = "Please Input EmployeeEarningLeave Value";
                    throw new ArgumentNullException("Please Input EmployeeEarningLeave Value", "");
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
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex

                if (Vtransaction == null)
                {
                    transaction.Rollback();
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

        //==================Update =================
        public string[] Update(EmployeeEarningLeaveVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeEarningLeave Update"; //Method Name

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

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("UpdateToEmployeeEarningLeave");
                }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist

                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeeEarningLeave ";
                    sqlText += " WHERE EmployeeId=@EmployeeId AND  Id<>@Id and  DeductionDate=@DeductionDate";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExist.Parameters.AddWithValue("@DeductionDate", Ordinary.DateToString(vm.DeductionDate));
                    var exeRes = cmdExist.ExecuteScalar();
                    int objfoundId = Convert.ToInt32(exeRes);

                    if (objfoundId > 0)
                    {
                        retResults[1] = "Deduction already used for this Employee on this period!";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Deduction already used for this Employee on this period!", "");
                    }

                    #endregion Exist

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeEarningLeave set";
                    sqlText += " EmployeeId=@EmployeeId";
                    sqlText += " , DeductionAmount=@DeductionAmount";
                    sqlText += " , FiscalYearDetailId=@FiscalYearDetailId";
                    sqlText += " , SalaryPreiodId=@SalaryPreiodId";
                    sqlText += " , DeductionDate=@DeductionDate";
                    //sqlText += " , DeductionTypeId=@DeductionTypeId";

                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";

                    sqlText += " , GrossSalary=@GrossSalary";
                    sqlText += " , BasicSalary=@BasicSalary";
                    sqlText += " , Days=@Days";


                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@DeductionAmount", vm.DeductionAmount);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@SalaryPreiodId", vm.SalaryPreiodId);
                    cmdUpdate.Parameters.AddWithValue("@DeductionDate", vm.DeductionDate);
                    //cmdUpdate.Parameters.AddWithValue("@DeductionTypeId", vm.DeductionTypeId);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);
                    cmdUpdate.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                    cmdUpdate.Parameters.AddWithValue("@Days", vm.Days);

                    cmdUpdate.Transaction = transaction;
                    exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString(); // Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EmployeeEarningLeaveVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeEarningLeave Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update EmployeeEarningLeave.";
                    throw new ArgumentNullException("", "");
                }
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
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

            return retResults;
        }

        //==================Delete =================
        public string[] Delete(EmployeeEarningLeaveVM vm, string[] Ids, SqlConnection VcurrConn,
            SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeEarningLeave"; //Method Name

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

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("DeleteToEmployeeEarningLeave");
                }

                #endregion open connection and transaction

                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings

                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeEarningLeave set";
                        sqlText += " IsArchive=@IsArchive";
                        sqlText += " , DeductionAmount=0";
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


                    retResults[2] = ""; // Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("EmployeeEarningLeave Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeEarningLeave Information Delete",
                        "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete EmployeeEarningLeave Information.";
                    throw new ArgumentNullException("", "");
                }
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
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

            return retResults;
        }

        //==================SelectAllForReport=================
        public List<EmployeeEarningLeaveVM> SelectAllForReport(int fid, int fidTo, string ProjectId,
            string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int DTid = 0,
            string Orderby = null, string reportName = "")
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeEarningLeaveVM> vms = new List<EmployeeEarningLeaveVM>();
            EmployeeEarningLeaveVM vm;

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
ea.Id
,ea.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project,e.StepName,e.Grade 
,sal.BasicSalary
, sal.GrossSalary
, sal.HouseRent
, sal.Medical
, sal.TransportAllowance
, sal.Stamp

, e.BankAccountNo 
, e.Email
, e.Routing_No

--,ea.DeductionAmount
,round(ea.DeductionAmount,0)DeductionAmount
,round((ea.DeductionAmount-sal.Stamp),0)NetDeductionAmount

,ea.Days
,fyd.PeriodStart                 
,ea.FiscalYearDetailId
,ea.DeductionDate
,ea.DesignationId
, ea.DepartmentId, ea.SectionId, ea.ProjectId
,ea.Remarks
,e.SectionOrder 
,fyyd.PeriodName SalaryPeriod
,fyyd.PeriodName SalaryPeriodName

From EmployeeEarningLeave ea 
LEFT OUTER JOIN ViewEmployeeInformation e on ea.EmployeeId=e.id
left outer join FiscalYearDetail fyd on ea.FiscalYearDetailId =fyd.Id
left outer join FiscalYearDetail fyyd on ea.SalaryPreiodId =fyyd.Id



left outer join ( SELECT distinct FiscalYearDetailId,EmployeeId
 , sum( case when salaryhead='Gross' then Amount else 0 end) GrossSalary
 , sum( case when salaryhead='basic' then Amount else 0 end) BasicSalary
 , sum( case when salaryhead='HouseRent' then Amount else 0 end) HouseRent
 , sum( case when salaryhead='Medical' then Amount else 0 end) Medical
 , sum( case when salaryhead='TransportAllowance' then Amount else 0 end) TransportAllowance
 , sum( case when salaryhead='Stamp' then Amount else 0 end) Stamp
 from [ViewSalaryPreCalculation]
  group by FiscalYearDetailId ,EmployeeId

    ) sal on ea.SalaryPreiodId=sal.FiscalYearDetailId and ea.EmployeeId=sal.EmployeeId

Where 1=1 and ea.DeductionAmount > 0
";

                if (fid != 0)
                {
                    sqlText += @" and ea.FiscalYearDetailId>='" + fid + "'";
                }

                if (fidTo != 0)
                {
                    sqlText += @" and ea.FiscalYearDetailId<='" + fidTo + "'";
                }


                if (reportName == "EarnLeaveSheet3")
                {
                    sqlText += "AND e.BankName not in ('Standard Chartered Bank')";
                }
                else if (reportName == "EarnLeaveSheet4")
                {
                    sqlText += "AND e.BankName  in ('Standard Chartered Bank')";

                }

                //if (DTid != 0)
                //{
                //    sqlText += @" and ea.DeductionTypeId='" + DTid + "'";
                //}

                if (ProjectId != "0_0")
                    sqlText += " and ea.ProjectId=@ProjectId ";

                if (DepartmentId != "0_0")
                    sqlText += " and ea.DepartmentId=@DepartmentId ";

                if (SectionId != "0_0")
                    sqlText += " and ea.SectionId=@SectionId ";

                if (DesignationId != "0_0")
                    sqlText += " and ea.DesignationId=@DesignationId ";

                if (CodeF != "0_0")
                    sqlText += " and e.Code>= @CodeF ";

                if (CodeT != "0_0")
                    sqlText += " and e.Code<= @CodeT ";

                //sqlText += " ORDER BY ea.FiscalYearDetailId, ea.Department, ea.Section, ea.Code ";
                //sqlText += " ORDER BY ea.FiscalYearDetailId";
                if (!string.IsNullOrWhiteSpace(Orderby))
                {
                    //if (Orderby == "DCG")
                    //    sqlText += " ORDER BY e.department, e.code";

                    sqlText += " ORDER BY e.SectionOrder,e.DesignationOrder,e.code";

                    //else if (Orderby == "DDC")
                    //    sqlText += " ORDER BY e.department, e.JoinDate, e.code";
                    //else if (Orderby == "DGC")
                    //    sqlText += " ORDER BY e.department,  e.code";
                    //else if (Orderby == "DGDC")
                    //    sqlText += " ORDER BY e.department,  e.JoinDate, e.code";
                    //else if (Orderby == "CODE")
                    //    sqlText += "ORDER BY e.code";
                }

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
                    vm = new EmployeeEarningLeaveVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.NetDeductionAmount = Convert.ToDecimal(dr["NetDeductionAmount"]);
                    vm.Days = Convert.ToDecimal(dr["Days"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    //vm.PeriodName = dr["PeriodName"].ToString();
                    vm.DeductionDate = Ordinary.StringToDate(dr["DeductionDate"].ToString());
                    //vm.DeductionTypeId = Convert.ToInt32(dr["DeductionTypeId"]);
                    //vm.DeductionType = dr["DeductionType"].ToString();

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());

                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.Medical = Convert.ToDecimal(dr["Medical"]);
                    vm.HouseRent = Convert.ToDecimal(dr["HouseRent"]);
                    vm.TransportAllowance = Convert.ToDecimal(dr["TransportAllowance"]);
                    vm.Stamp = Convert.ToDecimal(dr["Stamp"]);

                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionOrder = dr["SectionOrder"].ToString();
                    vm.StepName = dr["StepName"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    vm.BankAccountNo = dr["BankAccountNo"].ToString();
                    vm.Routing_No = dr["Routing_No"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.SalaryPeriod = dr["SalaryPeriod"].ToString();
                    vm.SalaryPeriodName = dr["SalaryPeriodName"].ToString();


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

        public List<EmployeeEarningLeaveVM> SelectAllForReportSummary(int fid, int fidTo, string ProjectId,
          string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int DTid = 0,
          string Orderby = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeEarningLeaveVM> vms = new List<EmployeeEarningLeaveVM>();
            EmployeeEarningLeaveVM vm;

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
 D.Name Designation 
 ,e.Section
 ,e.Project 
 ,D.Serial
, sum(sal.BasicSalary)BasicSalary
, sum(sal.GrossSalary)GrossSalary
, sum(sal.HouseRent)HouseRent
, sum(sal.Medical)Medical
, sum(sal.TransportAllowance)TransportAllowance
, sum(sal.Stamp)Stamp
, sum(round(ea.DeductionAmount,0))DeductionAmount
, sum(ea.Days)Days
, ea.SectionId
, ea.ProjectId
, e.SectionOrder
, ea.FiscalYearDetailId
,fyyd.PeriodName SalaryPeriodName
From EmployeeEarningLeave ea 
LEFT OUTER JOIN ViewEmployeeInformation e on ea.EmployeeId=e.id
LEFT OUTER JOIN DesignationGroup D on e.DesignationGroupId=D.Id
left outer join FiscalYearDetail fyd on ea.FiscalYearDetailId =fyd.Id
left outer join FiscalYearDetail fyyd on ea.SalaryPreiodId =fyyd.Id
left outer join ( SELECT distinct FiscalYearDetailId,EmployeeId
 , sum( case when salaryhead='Gross' then Amount else 0 end) GrossSalary
 , sum( case when salaryhead='basic' then Amount else 0 end) BasicSalary
 , sum( case when salaryhead='HouseRent' then Amount else 0 end) HouseRent
 , sum( case when salaryhead='Medical' then Amount else 0 end) Medical
 , sum( case when salaryhead='TransportAllowance' then Amount else 0 end) TransportAllowance
 , sum( case when salaryhead='Stamp' then Amount else 0 end) Stamp
 from [ViewSalaryPreCalculation]
  group by FiscalYearDetailId ,EmployeeId
    ) sal on ea.SalaryPreiodId=sal.FiscalYearDetailId and ea.EmployeeId=sal.EmployeeId
Where 1=1 and ea.DeductionAmount > 0
";

                if (fid != 0)
                {
                    sqlText += @" and ea.FiscalYearDetailId>='" + fid + "'";
                }

                if (fidTo != 0)
                {
                    sqlText += @" and ea.FiscalYearDetailId<='" + fidTo + "'";
                }

                //if (DTid != 0)
                //{
                //    sqlText += @" and ea.DeductionTypeId='" + DTid + "'";
                //}

                if (ProjectId != "0_0")
                    sqlText += " and ea.ProjectId=@ProjectId ";

                //if (DepartmentId != "0_0")
                //    sqlText += " and ea.DepartmentId=@DepartmentId ";

                if (SectionId != "0_0")
                    sqlText += " and ea.SectionId=@SectionId ";

                //if (DesignationId != "0_0")
                //    sqlText += " and ea.DesignationId=@DesignationId ";

                //if (CodeF != "0_0")
                //    sqlText += " and e.Code>= @CodeF ";

                //if (CodeT != "0_0")
                //    sqlText += " and e.Code<= @CodeT ";

                sqlText += " group by e.SectionOrder,D.Serial,D.Name ,e.Section, e.Project,ea.SectionId, ea.ProjectId,ea.FiscalYearDetailId,fyyd.PeriodName ";
                //sqlText += " ORDER BY ea.FiscalYearDetailId";
                //if (!string.IsNullOrWhiteSpace(Orderby))
                //{
                //    if (Orderby == "DCG")
                //        sqlText += " ORDER BY e.department, e.code";
                //    else if (Orderby == "DDC")
                //        sqlText += " ORDER BY e.department, e.JoinDate, e.code";
                //    else if (Orderby == "DGC")
                //        sqlText += " ORDER BY e.department,  e.code";
                //    else if (Orderby == "DGDC")
                //        sqlText += " ORDER BY e.department,  e.JoinDate, e.code";
                //    else if (Orderby == "CODE")
                //        sqlText += "ORDER BY e.code";
                //}

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (ProjectId != "0_0")
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);

                //if (DepartmentId != "0_0")
                //    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);

                if (SectionId != "0_0")
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);

                //if (DesignationId != "0_0")
                //    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);

                //if (CodeF != "0_0")
                //    objComm.Parameters.AddWithValue("@CodeF", CodeF);

                //if (CodeT != "0_0")
                //    objComm.Parameters.AddWithValue("@CodeT", CodeT);


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeEarningLeaveVM();
                    //vm.Id = (dr["Id"]).ToString();
                    //vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.Days = Convert.ToDecimal(dr["Days"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    //vm.PeriodStart = dr["PeriodStart"].ToString();
                    //vm.PeriodName = dr["PeriodName"].ToString();
                    //vm.DeductionDate = Ordinary.StringToDate(dr["DeductionDate"].ToString());
                    //vm.DeductionTypeId = Convert.ToInt32(dr["DeductionTypeId"]);
                    //vm.DeductionType = dr["DeductionType"].ToString();

                    //vm.Remarks = dr["Remarks"].ToString();
                    //vm.EmpName = dr["EmpName"].ToString();
                    //vm.Code = dr["Code"].ToString();
                    //vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());

                    vm.Designation = dr["Designation"].ToString();
                    //vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.Medical = Convert.ToDecimal(dr["Medical"]);
                    vm.HouseRent = Convert.ToDecimal(dr["HouseRent"]);
                    vm.TransportAllowance = Convert.ToDecimal(dr["TransportAllowance"]);
                    vm.Stamp = Convert.ToDecimal(dr["Stamp"]);

                    //vm.DesignationId = dr["DesignationId"].ToString();
                    //vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.SectionOrder = dr["SectionOrder"].ToString();
                    vm.SalaryPeriodName = dr["SalaryPeriodName"].ToString();
                    //vm.StepName = dr["StepName"].ToString();
                    //vm.Grade = dr["Grade"].ToString();


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


        private void ConditionMaker(BonusProcessVM vm, List<string> ListCFields, List<string> ListCValues)
        {
            string[] cFields =
            {
                "e.Code>", "e.Code<", "e.ProjectId", "e.DepartmentId", "e.SectionId", "e.DesignationId",
                "sbd.BonusNameId", "sbd.BonusStructureId", "ej.Other1", "ej.Other2"
            };


            string[] cValues =
            {
                vm.CodeF, vm.CodeT, vm.ProjectId, vm.DepartmentId, vm.SectionId, vm.DesignationId, vm.BonusNameId,
                vm.BonusStructureId, vm.Other1, vm.Other2
            };


            int CoditionCount = 0;

            CoditionCount = cValues.Where(c => !string.IsNullOrWhiteSpace(c)).ToList().Count();


            if (CoditionCount > 0)
            {
                if (cFields != null && cValues != null && cFields.Length == cValues.Length)
                {
                    for (int i = 0; i < cFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(cFields[i]) || string.IsNullOrWhiteSpace(cValues[i]) ||
                            cValues[i] == "0")
                        {
                            continue;
                        }

                        ListCFields.Add(cFields[i]);
                        ListCValues.Add(cValues[i]);
                    }
                }
            }

            //////ListCFields.AddRange(cFields.ToList());
            //////ListCValues.AddRange(cValues.ToList());
        }

        //#endregion

        #region ImportExport

        public string[] ImportExcelFile(string Fullpath, string fileName, ShampanIdentityVM auditvm,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, int FYDId = 0, int SFYId = 0)
        {
            #region Initializ

            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = Id.ToString(); // Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeeEarningLeaveVM"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataSet ds = new DataSet();

            #endregion

            EmployeeInfoDAL _dalemp = new EmployeeInfoDAL();
            ViewEmployeeInfoVM empVM = new ViewEmployeeInfoVM();
            FiscalYearDAL fydal = new FiscalYearDAL();
            EarningDeductionTypeDAL eddal = new EarningDeductionTypeDAL();
            FiscalYearDetailVM FYDVM = new FiscalYearDetailVM();
            FiscalYearDetailVM SFYVM = new FiscalYearDetailVM();

            #region try

            try
            {
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                System.Data.DataTable dt = new System.Data.DataTable();
                FileStream stream = System.IO.File.Open(Fullpath, FileMode.Open, FileAccess.Read);
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
                dt = ds.Tables[0].Select("empCode <>''").CopyToDataTable();

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

                foreach (DataRow item in dt.Rows)
                {
                    EmployeeEarningLeaveVM vm = new EmployeeEarningLeaveVM();
                    //empVM=_dalemp.se
                    empVM = _dalemp.ViewSelectAllEmployee(item["EmpCode"].ToString(), null, null, null, null, null,
                        null, currConn, transaction).FirstOrDefault();
                    if (empVM == null || empVM.Id == null)
                    {
                        throw new ArgumentNullException(
                            "Employee Code " + item["EmpCode"].ToString() + " Not in System",
                            "Employee Code " + item["EmpCode"].ToString() + " Not in System");
                    }
                    ////FYDVM = fydal.FYPeriodDetail(Convert.ToInt32(item["FYDId"].ToString()), currConn, transaction).FirstOrDefault();

                    FYDVM = fydal.FYPeriodDetail(FYDId, currConn, transaction).FirstOrDefault();

                    ////SFYVM = fydal.FYPeriodDetail(Convert.ToInt32(item["SFYId"].ToString()), currConn, transaction).FirstOrDefault();
                    SFYVM = fydal.FYPeriodDetail(SFYId, currConn, transaction).FirstOrDefault();

                    if (FYDVM == null)
                    {
                        throw new ArgumentNullException(
                            "Fiscal Period" + item["FYDId"].ToString() + " Not in System",
                            "Fiscal Period " + item["FYDId"].ToString() + " Not in System");
                    }

                    if (SFYVM == null)
                    {
                        throw new ArgumentNullException(
                            "Salary Fiscal Period" + item["SFYId"].ToString() + " Not in System",
                            "Fiscal Period " + item["SFYId"].ToString() + " Not in System");
                    }

                    if (!Ordinary.IsNumeric(item["Amount"].ToString()))
                    {
                        throw new ArgumentNullException("Please input the Numeric Value in Amount",
                            "Please input the Numeric Value in Amount");
                    }

                    vm.DeductionDate = FYDVM.PeriodStart;
                    vm.EmployeeId = empVM.Id;
                    vm.FiscalYearDetailId = Convert.ToInt32(FYDVM.Id);
                    vm.SalaryPreiodId = Convert.ToInt32(SFYVM.Id);
                    vm.DeductionAmount = Convert.ToDecimal(item["Amount"]);
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

                    vm.GrossSalary = Convert.ToDecimal(item["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(item["BasicSalary"]);
                    vm.Days = Convert.ToDecimal(item["Days"]);

                    if (CompanyName.ToLower() == "tib")
                    {
                        if (vm.DeductionAmount == 0)
                        {
                            vm.CalculateEL();
                        }
                    }

                    retResults = Insert(vm, currConn, transaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException("EmployeeEarningLeave Update",
                            "Could not found any item.");
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
                retResults[0] = "Fail";
                if (Vtransaction == null)
                {
                    transaction.Rollback();
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

        public System.Data.DataTable ExportExcelFile(string Filepath, string FileName, string ProjectId,
            string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0,
            string Orderby = null)
        {
            string[] results = new string[6];
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                #region Fiscal Period

                FiscalYearDAL fdal = new FiscalYearDAL();
                EarningDeductionTypeDAL eddal = new EarningDeductionTypeDAL();
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

                sqlText = @"

select * into #SalaryTemp from (
SELECT distinct FiscalYearDetailId,EmployeeId
 , sum( case when salaryhead='Gross' then Amount else 0 end) GrossSalary
 , sum( case when salaryhead='basic' then Amount else 0 end) BasicSalary
 , sum( case when salaryhead='HouseRent' then Amount else 0 end) HouseRent
 , sum( case when salaryhead='Medical' then Amount else 0 end) Medical
 , sum( case when salaryhead='TransportAllowance' then Amount else 0 end) TransportAllowance
 , sum( case when salaryhead='Stamp' then Amount else 0 end) Stamp
 from [ViewSalaryPreCalculation]
where FiscalYearDetailId = @FiscalYearDetailId
 group by FiscalYearDetailId,EmployeeId
 ) as salary


select * 
from (select Code EmpCode,EmpName,JoinDate, ISNULL(GradeSL,99) GradeSL,ISNULL(Grade+'-'+StepName,'NA') Grade
, ISNULL(NULLIF(Designation,'=NA='), 'NA')Designation
, ISNULL(NULLIF(Department,'=NA='), 'NA')Department
, ISNULL(NULLIF(Section,'=NA='), 'NA')Section
, ISNULL(NULLIF(Project,'=NA='), 'NA')Project
, DeductionAmount Amount,ViewEmployeeEarningLeave.FiscalYeardetailId FYDId,SalaryPreiodId SFYId
, st.GrossSalary, st.BasicSalary, ISNULL(Days,0) Days
from ViewEmployeeEarningLeave left outer join #SalaryTemp st on st.EmployeeId = ViewEmployeeEarningLeave.EmployeeId


where 1=1
";

                if (fid != 0)
                {
                    sqlText += @" and ViewEmployeeEarningLeave.FiscalYearDetailId='" + fid + "'";
                }

                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                    sqlText += @" and ViewEmployeeEarningLeave.ProjectId='" + ProjectId + "'";
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                    sqlText += @" and ViewEmployeeEarningLeave.DepartmentId='" + DepartmentId + "'";
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                    sqlText += @" and ViewEmployeeEarningLeave.SectionId='" + SectionId + "'";
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
                    sqlText += @" and ViewEmployeeEarningLeave.DesignationId='" + DesignationId + "'";
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    sqlText += @" and ViewEmployeeEarningLeave.Code >='" + CodeF + "'";
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    sqlText += @" and ViewEmployeeEarningLeave.Code<='" + CodeT + "'";

                #region Regular

                sqlText += @"
union all
select vws.Code EmpCode, vws.EmpName,JoinDate,ISNULL(GradeSL,99) GradeSL,ISNULL(Grade+'-'+StepName,'NA') Grade
, ISNULL(NULLIF(Designation,'=NA='), 'NA')Designation
, ISNULL(NULLIF(Department,'=NA='), 'NA')Department
, ISNULL(NULLIF(Section,'=NA='), 'NA')Section
, ISNULL(NULLIF(Project,'=NA='), 'NA')Project
, 0 Amount,0 FYDId,0 SFYId
, st.GrossSalary, st.BasicSalary, 0 Days
from ViewEmployeeInformation vws left outer join #SalaryTemp st on st.EmployeeId = vws.EmployeeId

where 1=1  AND vws.IsActive=1 AND vws.IsArchive=0 
AND vws.Joindate <= @PeriodEnd
";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                    sqlText += @" and vws.ProjectId='" + ProjectId + "'";
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                    sqlText += @" and vws.DepartmentId='" + DepartmentId + "'";
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                    sqlText += @" and vws.SectionId='" + SectionId + "'";
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
                    sqlText += @" and vws.DesignationId='" + DesignationId + "'";
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    sqlText += @" and vws.Code >='" + CodeF + "'";
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    sqlText += @" and vws.Code<='" + CodeT + "'";

                sqlText += @" and vws.EmployeeId not in(select EmployeeId from ViewEmployeeEarningLeave where 1=1 ";

                if (fid != 0)
                {
                    sqlText += @" AND FiscalYearDetailId='" + fid + "'";
                }

                sqlText += @" ) ";

                #endregion

                #region Resign From This Month or Later

                sqlText += @"
union all
select vws.Code EmpCode, vws.EmpName,JoinDate,ISNULL(GradeSL,99) GradeSL,ISNULL(Grade+'-'+StepName,'NA') Grade
, ISNULL(NULLIF(Designation,'=NA='), 'NA')Designation
, ISNULL(NULLIF(Department,'=NA='), 'NA')Department
, ISNULL(NULLIF(Section,'=NA='), 'NA')Section
, ISNULL(NULLIF(Project,'=NA='), 'NA')Project
,  0 Amount,0 FYDId,0 SFYId
, st.GrossSalary, st.BasicSalary, 0 Days
from ViewEmployeeInformation vws left outer join #SalaryTemp st on st.EmployeeId = vws.EmployeeId
where 1=1  AND vws.IsActive=0 AND vws.IsArchive=0 
AND  vws.LeftDate >= @PeriodStart 
";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                    sqlText += @" and vws.ProjectId='" + ProjectId + "'";
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                    sqlText += @" and vws.DepartmentId='" + DepartmentId + "'";
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                    sqlText += @" and vws.SectionId='" + SectionId + "'";
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
                    sqlText += @" and vws.DesignationId='" + DesignationId + "'";
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    sqlText += @" and vws.Code >='" + CodeF + "'";
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    sqlText += @" and vws.Code<='" + CodeT + "'";

                sqlText += @" and vws.EmployeeId not in(select EmployeeId from ViewEmployeeEarningLeave where 1=1 ";

                if (fid != 0)
                {
                    sqlText += @" AND FiscalYearDetailId='" + fid + "'";
                }

                sqlText += @" ) ";

                #endregion

                sqlText += @" ) as a";

                if (Orderby == "DCG")
                    sqlText += " order by a.department, a.EmpCode, a.GradeSl";
                else if (Orderby == "DDC")
                    sqlText += " order by a.department, a.JoinDate, a.code";
                else if (Orderby == "DGC")
                    sqlText += " order by a.department, a.GradeSl, a.code";
                else if (Orderby == "DGDC")
                    sqlText += " order by a.department, a.GradeSl, a.JoinDate, a.EmpCode";
                else if (Orderby == "CODE")
                    sqlText += " order by  a.EmpCode";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.SelectCommand.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
                da.SelectCommand.Parameters.AddWithValue("@PeriodStart", PeriodStart);
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", fid);

                da.Fill(dt);
                dt.Columns.Add("Fiscal Period");
                dt.Columns.Add("Salary Preiod");
                dt.Columns.Remove("GradeSl");
                dt.Columns.Remove("JoinDate");
                foreach (DataRow row in dt.Rows)
                {
                    var SalaryPreiod = new FiscalYearDAL().FYPeriodDetail(Convert.ToInt32(row["SFYId"]), null, null)
                        .FirstOrDefault();
                    //if (SalaryPreiod==null)
                    //{
                    //    SalaryPreiod.PeriodName = fname;
                    //}
                    row["Fiscal Period"] = fname;
                    row["Salary Preiod"] = SalaryPreiod == null ? fname : SalaryPreiod.PeriodName;
                    //row["Salary Preiod"] = SalaryPreiod.PeriodName;
                }

                #region Value Round

                string[] columnNames = { "Amount", "GrossSalary", "BasicSalary" };

                dt = Ordinary.DtValueRound(dt, columnNames);

                #endregion

                //bool tt =Ordinary.WriteDataTableToExcel(dt, "DataSheet", Filepath + FileName);
                //if (tt == false)
                //{
                //    results[0] = "Fail";
                //    results[1] = "Data Download UnSuccessfully.";
                //}
                //{
                //    results[0] = "Success";
                //    results[1] = "Data Download Successfully.";
                //}

                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                results[4] = ex.Message.ToString();
            }

            return dt;
        }

        public bool ExportExcelFilebackup(string Filepath, string FileName, string ProjectId, string DepartmentId,
            string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, int DTId = 0,
            string Orderby = null)
        {
            try
            {
                FiscalYearDAL fdal = new FiscalYearDAL();
                EarningDeductionTypeDAL eddal = new EarningDeductionTypeDAL();
                var fname = fdal.FYPeriodDetail(fid, null, null).FirstOrDefault().PeriodName;
                var edName = eddal.SelectById(DTId).Name;
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
                worksheet.Cells[1, 11] = "Type";
                worksheet.Cells[1, 12] = "FYDId";
                worksheet.Cells[1, 13] = "DTId";

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

                sqlText = @"select * from (select Code EmpCode,EmpName,GradeSL
(case when Designation is null then 'NA' when Designation='=NA=' then 'NA' else Designation end) Designation,
(case when Department is null then 'NA' when Department='=NA=' then 'NA' else Department end) Department ,
(case when Section is null then 'NA' when Section='=NA=' then 'NA' else Section end) Section,
(case when Project is null then 'NA' when Project='=NA=' then 'NA' else Project end) Project,
TypeName TransactionType,DeductionAmount Amount,FiscalYeardetailId FYDId,DeductiontypeId DTId
from EmployeeEarningLeave where 1=1 ";
                if (DTId != 0)
                {
                    sqlText += " and DeductiontypeId='" + DTId + "'  ";
                }

                if (fid != 0)
                {
                    sqlText += @" and FiscalYearDetailId='" + fid + "'";
                }

                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                    sqlText += @" and EmployeeEarningLeave.ProjectId='" + ProjectId + "'";
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                    sqlText += @" and EmployeeEarningLeave.DepartmentId='" + DepartmentId + "'";
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                    sqlText += @" and EmployeeEarningLeave.SectionId='" + SectionId + "'";
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
                    sqlText += @" and EmployeeEarningLeave.DesignationId='" + DesignationId + "'";
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    sqlText += @" and EmployeeEarningLeave.Code >='" + CodeF + "'";
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    sqlText += @" and EmployeeEarningLeave.Code<='" + CodeT + "'";
                sqlText += @"
union all
select Code EmpCode,EmpName,GradeSL
(case when Designation is null  then 'NA' when Designation='=NA=' then 'NA' else Designation end) Designation ,
(case when Department is null then 'NA' when Department='=NA=' then 'NA' else Department end) Department,
(case when Section is null then 'NA' when Section='=NA=' then 'NA' else Section end) Section,
(case when Project is null then 'NA' when Project='=NA=' then 'NA' else Project end) Project
,'LWP' TransactionType, 0 Amount,0 FYDId,0 DTId
from ViewEmployeeInformation vws
where 1=1 ";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" &&
                    ProjectId != null)
                    sqlText += @" and vws.ProjectId='" + ProjectId + "'";
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" &&
                    DepartmentId != null)
                    sqlText += @" and vws.DepartmentId='" + DepartmentId + "'";
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" &&
                    SectionId != null)
                    sqlText += @" and vws.SectionId='" + SectionId + "'";
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" &&
                    DesignationId != null)
                    sqlText += @" and vws.DesignationId='" + DesignationId + "'";
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    sqlText += @" and vws.Code >='" + CodeF + "'";
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    sqlText += @" and vws.Code<='" + CodeT + "'";

                sqlText += @" and EmployeeId not in(select EmployeeId from EmployeeEarningLeave)
) as a";
                if (Orderby == "DCG")
                    sqlText += " order by a.department, a.EmpCode, a.GradeSl";
                else if (Orderby == "DDC")
                    sqlText += " order by a.department, a.JoinDate, a.code";
                else if (Orderby == "DGC")
                    sqlText += " order by a.department, a.GradeSl, a.code";
                else if (Orderby == "DGDC")
                    sqlText += " order by a.department, a.GradeSl, a.JoinDate, a.EmpCode";
                else if (Orderby == "CODE")
                    sqlText += " ORDER BY  a.EmpCode";

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
                    worksheet.Cells[j, 11] = edName;
                    worksheet.Cells[j, 12] = fid;
                    worksheet.Cells[j, 13] = DTId;
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
    }
}