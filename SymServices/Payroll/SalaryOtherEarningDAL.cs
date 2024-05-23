using Microsoft.Office.Interop.Excel;
using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymServices.Payroll
{
    public class SalaryOtherEarningDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL cdal = new CommonDAL();
        #endregion
        //#region Methods
        //==================Get All Distinct FiscalPeriodName =================
        public List<SalaryOtherEarningVM> GetPeriodname()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryOtherEarningVM> vms = new List<SalaryOtherEarningVM>();
            SalaryOtherEarningVM vm;
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
from  ViewSalaryOtherEarning ve 
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
                    vm = new SalaryOtherEarningVM();
                    vm.FiscalYearDetailId =Convert.ToInt32(dr["FiscalYearDetailId"].ToString());
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
        public List<SalaryOtherEarningVM> SelectAll(string empid = null, int? fid = null, int? ETid = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryOtherEarningVM> vms = new List<SalaryOtherEarningVM>();
            SalaryOtherEarningVM vm;
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
,e.EmpName,e.Code,e.Designation,e.Department, e.Section, e.Project
,ISNULL(e.JoinDate, '19000101') JoinDate
,ISNULL(e.GrossSalary, 0) GrossSalary
,ISNULL(e.BasicSalary, 0) BasicSalary
,ISNULL(sa.EarningAmount, 0) EarningAmount
,sa.FiscalYearDetailId
,fs.PeriodName
,sa.EarningTypeId
,edt.Name EarningType
,e.DesignationId, e.DepartmentId, e.SectionId, e.ProjectId
,sa.Remarks
,sa.IsActive
,sa.IsArchive
,sa.CreatedBy
,sa.CreatedAt
,sa.CreatedFrom
,sa.LastUpdateBy
,sa.LastUpdateAt
,sa.LastUpdateFrom
From SalaryOtherEarning sa 
left outer join ViewEmployeeInformation e on sa.EmployeeId=e.id
left outer join  FiscalYearDetail fs on sa.FiscalYearDetailId=fs.Id
left outer join EarningDeductionType edt on sa.EarningTypeId = edt.Id and edt.IsEarning = 1 
Where 1=1 and  e.IsArchive=0 and e.IsActive=1 and sa.EarningAmount > 0
";
                if (!string.IsNullOrEmpty(empid))
                {
                    sqlText += @" and sa.EmployeeId='" + empid + "'";
                }
                if (fid != null && fid != 0)
                {
                    sqlText += @" and sa.FiscalYearDetailId='" + fid + "'";
                }
                if (ETid != null && ETid != 0)
                {
                    sqlText += @" and sa.EarningTypeId='" + ETid + "'";
                }
                sqlText += @" ORDER BY e.Department,e.EmpName desc";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryOtherEarningVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EarningAmount = Convert.ToDecimal(dr["EarningAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.EarningTypeId = Convert.ToInt32(dr["EarningTypeId"]);
                    vm.EarningType = dr["EarningType"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
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
        public List<SalaryOtherEarningVM> SelectAllByFIDPeriod(int? fid)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryOtherEarningVM> vms = new List<SalaryOtherEarningVM>();
            SalaryOtherEarningVM vm;
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
SELECT distinct
sa.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,sa.FiscalYearDetailId
,fs.PeriodName
,e.DesignationId, e.DepartmentId, e.SectionId, e.ProjectId
,sa.Remarks
,sa.IsActive
,sa.IsArchive
,sa.CreatedBy
,sa.CreatedAt
,sa.CreatedFrom
,sa.LastUpdateBy
,sa.LastUpdateAt
,sa.LastUpdateFrom
From SalaryOtherEarning sa 
left outer join ViewEmployeeInformation e on sa.EmployeeId=e.id
left outer join  FiscalYearDetail fs on sa.FiscalYearDetailId=fs.Id
left outer join EarningDeductionType edt on sa.EarningTypeId = edt.Id and edt.IsEarning = 1 
Where 1=1 and  sa.IsArchive=0 and sa.EarningAmount > 0
";
                if (fid != null && fid != 0)
                {
                    sqlText += @" and sa.FiscalYearDetailId='" + fid + "'";
                }
                sqlText += @" ORDER BY e.Department,e.EmpName desc";
                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryOtherEarningVM();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
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
        public SalaryOtherEarningVM SelectById(string Id)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            SalaryOtherEarningVM vm = new SalaryOtherEarningVM();
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
,sa.EarningAmount
,sa.FiscalYearDetailId
,sa.EarningTypeId
,edt.Name EarningType
,sa.Remarks
,sa.IsActive
,sa.IsArchive
,sa.CreatedBy
,sa.CreatedAt
,sa.CreatedFrom
,sa.LastUpdateBy
,sa.LastUpdateAt
,sa.LastUpdateFrom
From SalaryOtherEarning sa 
left outer join ViewEmployeeInformation e on sa.EmployeeId=e.id
left outer join EarningDeductionType edt on sa.EarningTypeId = edt.Id and edt.IsEarning = 1 
Where 1=1 and  sa.IsArchive=0  and sa.id=@Id and sa.EarningAmount > 0
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
                    vm = new SalaryOtherEarningVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EarningAmount = Convert.ToDecimal(dr["EarningAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.EarningTypeId = Convert.ToInt32(dr["EarningTypeId"]);
                    vm.EarningType = dr["EarningType"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
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
        public SalaryOtherEarningVM SelectByIdandFiscalyearDetail(string empId, string FiscalYearDetailId = "0", string edType = "0")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            SalaryOtherEarningVM vm = new SalaryOtherEarningVM();
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
,sa.EarningAmount
,sa.FiscalYearDetailId
,sa.EarningTypeId
,sa.Remarks
From SalaryOtherEarning sa 
Where sa.IsArchive=0  and sa.EmployeeId=@Id 
and sa.FiscalYearDetailId=@FiscalYearDetailId
and sa.EarningTypeId=@EarningTypeId
";
                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@Id", empId);
                objCommVehicle.Parameters.AddWithValue("@EarningTypeId", edType);
                objCommVehicle.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryOtherEarningVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EarningAmount = Convert.ToDecimal(dr["EarningAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.EarningTypeId = Convert.ToInt32(dr["EarningTypeId"]);
                    vm.Remarks = dr["Remarks"].ToString();
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
        public string[] Insert(SalaryOtherEarningVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertSalaryOtherEarning"; //Method Name
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
                sqlText += " SELECT   count(Id) FROM SalaryOtherEarning ";
                sqlText += " WHERE EmployeeId=@EmployeeId and  FiscalYearDetailId=@FiscalYearDetailId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                var exeRes = cmdExist.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes);
                if (objfoundId > 0)
                {
                    sqlText = "  ";
                    sqlText += " delete FROM SalaryOtherEarning ";
                    sqlText += " WHERE EmployeeId=@EmployeeId and  FiscalYearDetailId=@FiscalYearDetailId";
                    SqlCommand cmdExistD = new SqlCommand(sqlText, currConn);
                    cmdExistD.Transaction = transaction;
                    cmdExistD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExistD.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    var exeResD = cmdExistD.ExecuteScalar();
                }
                #endregion Exist
                #region Save
                //vm.Id = cdal.NextId("SalaryOtherEarning", currConn, transaction).ToString();
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO SalaryOtherEarning
(
 EmployeeId
, EarningAmount
, FiscalYearDetailId
, EarningTypeId
, ProjectId
, GradeId
, SectionId
, DepartmentId
, DesignationId
, Remarks
, IsActive
, IsArchive
, CreatedBy
, CreatedAt
, CreatedFrom
) VALUES (
@EmployeeId
, @EarningAmount
, @FiscalYearDetailId
, @EarningTypeId
, @ProjectId
, @GradeId
, @SectionId
, @DepartmentId
, @DesignationId
, @Remarks
, @IsActive
, @IsArchive
, @CreatedBy
, @CreatedAt
, @CreatedFrom
) ";
                    ///Fetching Data
                    var emp = _dal.ViewSelectAllEmployee(null, null, null, null, null, null, null, null).Where(x => x.Id.Equals(vm.EmployeeId)).FirstOrDefault();
                    vm.ProjectId = emp.ProjectId;
                    vm.SectionId = emp.SectionId;
                    vm.DepartmentId = emp.DepartmentId;
                    vm.DesignationId = emp.DesignationId;
                    vm.GradeId = emp.GradeId;
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    //cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@EarningAmount", vm.EarningAmount);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@EarningTypeId", vm.EarningTypeId);
                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@GradeId", vm.GradeId);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "Please Input Salary Other Earning Value";
                    throw new ArgumentNullException("Please Input Salary Other Earning Value", "");
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
//        //==================Update =================
//        public string[] Update(SalaryOtherEarningVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
//        {
//            #region Variables
//            string[] retResults = new string[6];
//            retResults[0] = "Fail";//Success or Fail
//            retResults[1] = "Fail";// Success or Fail Message
//            retResults[2] = "0";
//            retResults[3] = "sqlText"; //  SQL Query
//            retResults[4] = "ex"; //catch ex
//            retResults[5] = "Employee Salary Other Earning Update"; //Method Name
//            int transResult = 0;
//            string sqlText = "";
//            bool iSTransSuccess = false;
//            #endregion
//            SqlConnection currConn = null;
//            SqlTransaction transaction = null;
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
//                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSalaryOtherEarning"); }
//                #endregion open connection and transaction
//                if (vm != null)
//                {
//                    #region Exist
//                    sqlText = "  ";
//                    sqlText += " SELECT COUNT( Id)Id FROM SalaryOtherEarning ";
//                    sqlText += " WHERE EmployeeId=@EmployeeId AND  Id<>@Id";
//                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
//                    cmdExist.Transaction = transaction;
//                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);
//                    cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
//                    var exeRes = cmdExist.ExecuteScalar();
//                    int objfoundId = Convert.ToInt32(exeRes);
//                    if (objfoundId > 0)
//                    {
//                        retResults[1] = "Earning already used for this Employee on this period!";
//                        retResults[3] = sqlText;
//                        throw new ArgumentNullException("Earning already used for this Employee on this period!", "");
//                    }
//                    #endregion Exist
//                    #region Update Settings
//                    sqlText = "";
//                    sqlText = "update SalaryOtherEarning set";
//                    sqlText += " EmployeeId=@EmployeeId,";
//                    sqlText += " EarningAmount=@EarningAmount,";
//                    sqlText += " FiscalYearDetailId=@FiscalYearDetailId,";
//                    sqlText += " EarningTypeId=@EarningTypeId,";
//                    sqlText += " Remarks=@Remarks,";
//                    sqlText += " LastUpdateBy=@LastUpdateBy,";
//                    sqlText += " LastUpdateAt=@LastUpdateAt,";
//                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
//                    sqlText += " where Id=@Id";
//                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
//                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
//                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
//                    cmdUpdate.Parameters.AddWithValue("@EarningAmount", vm.EarningAmount);
//                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
//                    cmdUpdate.Parameters.AddWithValue("@EarningTypeId", vm.EarningTypeId);
//                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
//                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
//                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
//                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
//                    cmdUpdate.Transaction = transaction;
//                    exeRes = cmdUpdate.ExecuteNonQuery();
//                    transResult = Convert.ToInt32(exeRes);
//                    retResults[2] = vm.Id.ToString();// Return Id
//                    retResults[3] = sqlText; //  SQL Query
//                    #region Commit
//                    if (transResult <= 0)
//                    {
//                        // throw new ArgumentNullException("Education Update", SalaryOtherEarningVM.BranchId + " could not updated.");
//                    }
//                    #endregion Commit
//                    #endregion Update Settings
//                    iSTransSuccess = true;
//                }
//                else
//                {
//                    throw new ArgumentNullException("Salary Other Earning Update", "Could not found any item.");
//                }
//                if (iSTransSuccess == true)
//                {
//                    if (Vtransaction == null)
//                    {
//                        if (transaction != null)
//                        {
//                            transaction.Commit();
//                        }
//                    }
//                    retResults[0] = "Success";
//                    retResults[1] = "Data Update Successfully.";
//                }
//                else
//                {
//                    retResults[1] = "Unexpected error to update Salary Other Earning.";
//                    throw new ArgumentNullException("", "");
//                }
//            }
//            #region catch
//            catch (Exception ex)
//            {
//                retResults[4] = ex.Message; //catch ex
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
//            return retResults;
//        }
        //==================Delete =================
        public string[] Delete(SalaryOtherEarningVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSalaryOtherEarning"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryOtherEarning"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update SalaryOtherEarning set";
                        sqlText += " EarningAmount=0";
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
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Salary Other Earning Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Other Earning Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary Other Earning Information.";
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
        public string[] InsertSalaryOtherEarningNew(int FiscalYearDetailId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
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
            retResults[5] = "Employee SalaryOtherEarning Process"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            List<EmployeeInfoVM> employeeVms = new List<EmployeeInfoVM>();
            EmployeeInfoVM employeeVm;
            List<EmployeeOtherEarningVM> EmployeeOtherEarnings = new List<EmployeeOtherEarningVM>();
            EmployeeOtherEarningVM EmployeeOtherEarning;
            EmployeeInfoDAL _EmployeeInfoDAL = new EmployeeInfoDAL();
            string PeriodEnd = "";
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

                #region sqlText
                var sqlTextSalaryTaxDetail = "";
                sqlTextSalaryTaxDetail += @" INSERT INTO SalaryOtherEarning
(
 EmployeeId, EarningAmount, FiscalYearDetailId, EarningTypeId, ProjectId, SectionId, DepartmentId, DesignationId
, Remarks, IsActive, IsArchive, CreatedBy, CreatedAt, CreatedFrom,EmployeeStatus,GradeId
) VALUES (
  @EmployeeId, @EarningAmount, @FiscalYearDetailId, @EarningTypeId, @ProjectId, @SectionId, @DepartmentId, @DesignationId
, @Remarks, @IsActive, @IsArchive, @CreatedBy, @CreatedAt, @CreatedFrom,@EmployeeStatus,@GradeId
) ";
                #endregion str

                if (employeeVms.Count > 0)
                {
                    foreach (EmployeeInfoVM employee in employeeVms)
                    {
                        #region Delete Existing SalaryOtherEarning
                        sqlText = @"Delete SalaryOtherEarning ";
                        sqlText += " where 1=1 and EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId";
                        SqlCommand cmdDeletePFDetail = new SqlCommand(sqlText, currConn, transaction);
                        cmdDeletePFDetail.Parameters.AddWithValue("@EmployeeId", employee.Id);
                        cmdDeletePFDetail.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                        cmdDeletePFDetail.ExecuteNonQuery();
                        #endregion

                        #region EmployeeOtherEarning
                        sqlText = @"select * from EmployeeOtherEarning";
                        sqlText += " where 1=1 and EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId";
                        SqlCommand cmdepf = new SqlCommand(sqlText, currConn, transaction);
                        cmdepf.Parameters.AddWithValue("@EmployeeId", employee.Id);
                        cmdepf.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                        EmployeeOtherEarnings = new List<EmployeeOtherEarningVM>();
                        using (SqlDataReader drEOE = cmdepf.ExecuteReader())
                        {
                            while (drEOE.Read())
                            {
                                EmployeeOtherEarning = new EmployeeOtherEarningVM();
                                EmployeeOtherEarning.EarningAmount = Convert.ToDecimal(drEOE["EarningAmount"]);
                                EmployeeOtherEarning.EarningTypeId = Convert.ToInt32(drEOE["EarningTypeId"]);
                                EmployeeOtherEarning.Remarks =drEOE["Remarks"].ToString();
                                EmployeeOtherEarnings.Add(EmployeeOtherEarning);
                            }
                            drEOE.Close();
                        }
                        #endregion

                        if (EmployeeOtherEarnings.Count > 0)
                        {
                            foreach (EmployeeOtherEarningVM EOE in EmployeeOtherEarnings)
                            {
                                #region Sql Execution
                                
                                SqlCommand cmdInsert = new SqlCommand(sqlTextSalaryTaxDetail, currConn);
                                cmdInsert.Parameters.AddWithValue("@EmployeeId", employee.Id);
                                cmdInsert.Parameters.AddWithValue("@EarningAmount", EOE.EarningAmount);
                                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                                cmdInsert.Parameters.AddWithValue("@EarningTypeId", EOE.EarningTypeId);
                                cmdInsert.Parameters.AddWithValue("@ProjectId", employee.ProjectId);
                                cmdInsert.Parameters.AddWithValue("@SectionId", employee.SectionId);
                                cmdInsert.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
                                cmdInsert.Parameters.AddWithValue("@DesignationId", employee.DesignationId);
                                cmdInsert.Parameters.AddWithValue("@Remarks", EOE.Remarks ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                                cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                                cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                                cmdInsert.Parameters.AddWithValue("@EmployeeStatus", EmpType);
                                cmdInsert.Parameters.AddWithValue("@GradeId", employee.GradeId);
                                cmdInsert.Transaction = transaction;
                                cmdInsert.ExecuteNonQuery();
                                #endregion
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
        public string[] ImportOtherExcelFile(string fileName, ShampanIdentityVM autditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Salary Other Earning"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            EmployeeInfoDAL _dalemp = new EmployeeInfoDAL();
            ViewEmployeeInfoVM empVM = new ViewEmployeeInfoVM();
            FiscalYearDAL fydal = new FiscalYearDAL();
            EarningDeductionTypeDAL eddal = new EarningDeductionTypeDAL();
            FiscalYearDetailVM FYDVM = new FiscalYearDetailVM();
            #region try
            try
            {
                #region open connection and transaction
                DataSet ds = Ordinary.UploadExcel(fileName);
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = ds.Tables[0].Select("empCode <>''").CopyToDataTable();
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
                foreach (DataRow item in dt.Rows)
                {
                    SalaryOtherEarningVM vm = new SalaryOtherEarningVM();
                    //empVM=_dalemp.se
                    empVM = _dalemp.ViewSelectAllEmployee(item["EmpCode"].ToString(), null, null, null, null, null, null, currConn, transaction).FirstOrDefault();
                    if (empVM == null || empVM.Id == null)
                    {
                        throw new ArgumentNullException("Employee Code " + item["EmpCode"].ToString() + " Not in System", "Employee Code " + item["EmpCode"].ToString() + " Not in System");
                    }
                    else
                    {
                        FYDVM = fydal.FYPeriodDetail(Convert.ToInt32(item["FYDId"].ToString()), currConn, transaction).FirstOrDefault();
                        if (FYDVM == null)
                        {
                            throw new ArgumentNullException("Fiscal Period" + item["FYDId"].ToString() + " Not in System", "Fiscal Period " + item["FYDId"].ToString() + " Not in System");
                        }
                        else
                        {
                            vm.EarningTypeId = eddal.SelectById(Convert.ToInt32(item["EDId"].ToString()), currConn, transaction).Id;
                            if (vm.EarningTypeId == 0)
                            {
                                throw new ArgumentNullException("Salary Other Earning not in System,", "Fiscal Period " + item["EDId"].ToString() + " Not in System");
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
                                    vm.EarningAmount = Convert.ToDecimal(item["Amount"]);
                                    vm.ProjectId = empVM.ProjectId;
                                    vm.DepartmentId = empVM.DepartmentId;
                                    vm.SectionId = empVM.SectionId;
                                    vm.DesignationId = empVM.DesignationId;
                                    vm.LastUpdateAt = autditvm.LastUpdateAt;
                                    vm.LastUpdateBy = autditvm.LastUpdateBy;
                                    vm.LastUpdateFrom = autditvm.LastUpdateFrom;
                                    vm.CreatedAt = autditvm.CreatedAt;
                                    vm.CreatedBy = autditvm.CreatedBy;
                                    vm.CreatedFrom = autditvm.CreatedFrom;
                                    retResults = Insert(vm,VcurrConn,Vtransaction);
                                    if (retResults[0] != "Success")
                                    {
                                        throw new ArgumentNullException("Salary Other Earning Update", "Could not found any item.");
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
        public string[] ExportotherExcelFile(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, int fid = 0, int ETId = 0)
        {
            string[] results = new string[6];
            try
            {
                FiscalYearDAL fdal = new FiscalYearDAL();
                EarningDeductionTypeDAL eddal = new EarningDeductionTypeDAL();
                var fname = fdal.FYPeriodDetail(fid,null,null).FirstOrDefault().PeriodName;
                var edName = eddal.SelectById(ETId).Name;
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
                sqlText = @"select * from (select Code EmpCode,EmpName,
(case when Designation is null then 'NA' when Designation='=NA=' then 'NA' else Designation end) Designation,
(case when Department is null then 'NA' when Department='=NA=' then 'NA' else Department end) Department ,
(case when Section is null then 'NA' when Section='=NA=' then 'NA' else Section end) Section,
(case when Project is null then 'NA' when Project='=NA=' then 'NA' else Project end) Project,
TypeName TransactionType,EarningAmount Amount,FiscalYeardetailId FYDId,EarningTypeId EDId
from ViewSalaryOtherEarning where 1=1";
                if (ETId != 0)
                {
                    sqlText += " and EarningTypeId='" + ETId + "'  ";
                }
                if (fid != 0)
                {
                    sqlText += @" AND FiscalYearDetailId='" + fid + "'";
                }
                sqlText += @"
union all
select Code EmpCode,EmpName,
(case when Designation is null  then 'NA' when Designation='=NA=' then 'NA' else Designation end) Designation ,
(case when Department is null then 'NA' when Department='=NA=' then 'NA' else Department end) Department,
(case when Section is null then 'NA' when Section='=NA=' then 'NA' else Section end) Section,
(case when Project is null then 'NA' when Project='=NA=' then 'NA' else Project end) Project
,'LWP' TransactionType, 0 Amount,0 FYDId,0 EDId
from ViewEmployeeInformation vws 
where 1=1 AND vws.IsActive=1 AND vws.IsArchive=0";
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
                sqlText += @" and EmployeeId not in(select EmployeeId from SalaryOtherEarning)
) as a order by Department,Section,Project, EmpCode ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);
                dt.Columns.Add("Fiscal Period");
                dt.Columns.Add("Type");
                foreach (DataRow row in dt.Rows)
                {
                    row["Fiscal Period"] = fname;
                    row["Type"] = edName;
                    row["FYDId"] = fid;
                    row["EDId"] = ETId;
                }
                var tt =Ordinary.WriteDataTableToExcel(dt, "DataSheet", Filepath + FileName);
                if (tt == false)
                {
                    results[0] = "Fail";
                    results[1] = "Data Download UnSuccessfully.";
                }
                {
                    results[0] = "Success";
                    results[1] = "Data Download Successfully.";
                }
                #endregion
                #endregion
                // save the application
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
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
        //==================SelectAllForReport=================
        public List<SalaryOtherEarningVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int ETid = 0, string Orderby=null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryOtherEarningVM> vms = new List<SalaryOtherEarningVM>();
            SalaryOtherEarningVM vm;
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
,sa.EmpName,sa.Code,sa.Designation,sa.Department,sa.Section, sa.Project
,ISNULL(sa.JoinDate, '19000101') JoinDate
,ISNULL(sa.GrossSalary, 0) GrossSalary
,ISNULL(sa.BasicSalary, 0) BasicSalary
,ISNULL(sa.EarningAmount, 0) EarningAmount
,fyd.PeriodStart                 
,sa.FiscalYearDetailId
,sa.PeriodName
,sa.EarningTypeId
,sa.TypeName EarningType
,sa.DesignationId, sa.DepartmentId, sa.SectionId, sa.ProjectId
,sa.Remarks
From ViewSalaryOtherEarning sa 
left outer join grade g on sa.gradeId = g.id
left outer join FiscalYearDetail fyd on sa.FiscalYearDetailId =fyd.Id
Where 1=1 and sa.EarningAmount > 0
";
                if (fid != 0)
                {
                    sqlText += @" and sa.FiscalYearDetailId>='" + fid + "'";
                }
                if (fidTo != 0)
                {
                    sqlText += @" and sa.FiscalYearDetailId<='" + fidTo + "'";
                }
                if (ETid != 0)
                {
                    sqlText += @" and sa.EarningTypeId='" + ETid + "'";
                }
                if (ProjectId != "0_0"   )
                    sqlText += " and sa.ProjectId=@ProjectId";
                if (DepartmentId != "0_0" )
                    sqlText += " and sa.DepartmentId=@DepartmentId ";
                if (SectionId != "0_0"   )
                    sqlText += " and sa.SectionId=@SectionId ";
                if (DesignationId != "0_0"  )
                    sqlText += " and sa.DesignationId=@DesignationId ";
                if (CodeF != "0_0")
                    sqlText += " and sa.Code>= @CodeF";
                if (CodeT != "0_0")
                    sqlText += " and sa.Code<= @CodeT";
                //sqlText += " ORDER BY sa.FiscalYearDetailId, sa.Department, sa.Section, sa.Code ";
                sqlText += " ORDER BY sa.FiscalYearDetailId";
                if (Orderby == "DCG")
                    sqlText += " , sa.department, sa.code, g.sl";
                else if (Orderby == "DDC")
                    sqlText += " ,sa.department, sa.JoinDate, sa.code";
                else if (Orderby == "DGC")
                    sqlText += " , sa.department, g.sl, sa.code";
                else if (Orderby == "DGDC")
                    sqlText += ", sa.department, g.sl, sa.JoinDate, sa.code";
                else if (Orderby == "CODE")
                    sqlText += ", sa.code";
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
                    vm = new SalaryOtherEarningVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EarningAmount = Convert.ToDecimal(dr["EarningAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.EarningTypeId = Convert.ToInt32(dr["EarningTypeId"]);
                    vm.EarningType = dr["EarningType"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
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
        //#endregion
    }
}
