using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymServices.Payroll
{
    public class SalaryOtherDeductionDAL
    {

        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL cdal = new CommonDAL();
        #endregion

        //==================Get All Distinct FiscalPeriodName =================
        public List<SalaryOtherDeductionVM> GetPeriodname()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryOtherDeductionVM> vms = new List<SalaryOtherDeductionVM>();
            SalaryOtherDeductionVM vm;
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
from  ViewSalaryOtherDeduction ve 
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
                    vm = new SalaryOtherDeductionVM();
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

        //==================SelectAll=================
        public List<SalaryOtherDeductionVM> SelectAll(string empid = null, int? fid = null, int? DTId = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryOtherDeductionVM> vms = new List<SalaryOtherDeductionVM>();
            SalaryOtherDeductionVM vm;
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
,sa.DeductionAmount
,sa.FiscalYearDetailId
,fs.PeriodName
,sa.DeductionTypeId
,edt.Name DeductionType

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
From SalaryOtherDeduction sa 
left outer join ViewEmployeeInformation e on sa.EmployeeId=e.id
left outer join  FiscalYearDetail fs on sa.FiscalYearDetailId=fs.Id
left outer join EarningDeductionType edt on sa.DeductionTypeId = edt.Id and edt.IsEarning = 0 
Where 1=1 and  e.IsArchive=0 and e.IsActive=1 and sa.DeductionAmount > 0
";
                if (!string.IsNullOrEmpty(empid))
                {
                    sqlText += @" and sa.EmployeeId='" + empid + "'";
                }

                if (fid != null && fid != 0)
                {
                    sqlText += @" and sa.FiscalYearDetailId='" + fid + "'";
                }
                if (DTId != null && DTId != 0)
                {
                    sqlText += @" and sa.DeductionTypeId='" + DTId + "'";
                }


                sqlText += @" ORDER BY e.Department,e.EmpName desc";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryOtherDeductionVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.DeductionTypeId = Convert.ToInt32(dr["DeductionTypeId"]);
                    vm.DeductionType = dr["DeductionType"].ToString();


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
        public SalaryOtherDeductionVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SalaryOtherDeductionVM vm = new SalaryOtherDeductionVM();

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
,sa.DeductionAmount
,sa.FiscalYearDetailId
,sa.DeductionTypeId
,edt.Name DeductionType
,sa.Remarks
,sa.IsActive
,sa.IsArchive
,sa.CreatedBy
,sa.CreatedAt
,sa.CreatedFrom
,sa.LastUpdateBy
,sa.LastUpdateAt
,sa.LastUpdateFrom

From SalaryOtherDeduction sa 
left outer join ViewEmployeeInformation e on sa.EmployeeId=e.id
left outer join EarningDeductionType edt on sa.DeductionTypeId = edt.Id and edt.IsEarning = 0 
Where 1=1 and  sa.IsArchive=0  and sa.id=@Id and sa.DeductionAmount > 0
ORDER BY e.Department, e.EmpName desc
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
                    vm = new SalaryOtherDeductionVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.DeductionTypeId = Convert.ToInt32(dr["DeductionTypeId"]);
                    vm.DeductionType = dr["DeductionType"].ToString();


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
        public SalaryOtherDeductionVM SelectByIdandFiscalyearDetail(string empId, string FiscalYearDetailId = "0", string edType = "0")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            SalaryOtherDeductionVM vm = new SalaryOtherDeductionVM();
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
,sa.DeductionAmount
,sa.FiscalYearDetailId
,sa.DeductionTypeId
,sa.Remarks
From SalaryOtherDeduction sa 
Where sa.IsArchive=0  and sa.EmployeeId=@Id 
and sa.FiscalYearDetailId=@FiscalYearDetailId
and sa.DeductionTypeId=@DeductionTypeId
 
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", empId);
                objComm.Parameters.AddWithValue("@DeductionTypeId", edType);
                objComm.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryOtherDeductionVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.DeductionTypeId = Convert.ToInt32(dr["DeductionTypeId"]);
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
        public string[] Insert(SalaryOtherDeductionVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertSalaryOtherDeduction"; //Method Name
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
                sqlText += " SELECT   count(Id) FROM SalaryOtherDeduction ";
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
                    sqlText += " delete FROM SalaryOtherDeduction ";
                    sqlText += " WHERE EmployeeId=@EmployeeId and  FiscalYearDetailId=@FiscalYearDetailId";
                    SqlCommand cmdExistD = new SqlCommand(sqlText, currConn);
                    cmdExistD.Transaction = transaction;
                    cmdExistD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExistD.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                    var exeResD = cmdExistD.ExecuteScalar();
                }
                #endregion Exist
                #region Save
                vm.Id = cdal.NextId("SalaryOtherDeduction", currConn, transaction).ToString();
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO SalaryOtherDeduction
(
Id
, EmployeeId
, DeductionAmount
, FiscalYearDetailId
, DeductionTypeId
, ProjectId
, SectionId
, DepartmentId
, DesignationId
, GradeId
, Remarks
, IsActive
, IsArchive
, CreatedBy
, CreatedAt
, CreatedFrom
) VALUES (
@Id
, @EmployeeId
, @DeductionAmount
, @FiscalYearDetailId
, @DeductionTypeId
, @ProjectId
, @SectionId
, @DepartmentId
, @DesignationId
, @GradeId
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


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@DeductionAmount", vm.DeductionAmount);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@DeductionTypeId", vm.DeductionTypeId);

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
                    retResults[1] = "Please Input Salary Other Deduction Value";
                    throw new ArgumentNullException("Please Input Salary Other Deduction Value", "");
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
//        public string[] Update(SalaryOtherDeductionVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
//        {
//            #region Variables

//            string[] retResults = new string[6];
//            retResults[0] = "Fail";//Success or Fail
//            retResults[1] = "Fail";// Success or Fail Message
//            retResults[2] = "0";
//            retResults[3] = "sqlText"; //  SQL Query
//            retResults[4] = "ex"; //catch ex
//            retResults[5] = "Employee Salary Other Deduction Update"; //Method Name

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
//                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSalaryOtherDeduction"); }

//                #endregion open connection and transaction

//                if (vm != null)
//                {
//                    #region Exist
//                    sqlText = "  ";
//                    sqlText += " SELECT COUNT( Id)Id FROM SalaryOtherDeduction ";
//                    sqlText += " WHERE EmployeeId=@EmployeeId AND  Id<>@Id";
//                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
//                    cmdExist.Transaction = transaction;
//                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);
//                    cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
//                    var exeRes = cmdExist.ExecuteScalar();
//                    int objfoundId = Convert.ToInt32(exeRes);

//                    if (objfoundId > 0)
//                    {
//                        retResults[1] = "Deduction already used for this Employee on this period!";
//                        retResults[3] = sqlText;
//                        throw new ArgumentNullException("Deduction already used for this Employee on this period!", "");
//                    }

//                    #endregion Exist

//                    #region Update Settings

//                    sqlText = "";
//                    sqlText = "update SalaryOtherDeduction set";
//                    sqlText += " EmployeeId=@EmployeeId,";
//                    sqlText += " DeductionAmount=@DeductionAmount,";
//                    sqlText += " FiscalYearDetailId=@FiscalYearDetailId,";
//                    sqlText += " DeductionTypeId=@DeductionTypeId,";

//                    sqlText += " Remarks=@Remarks,";
//                    sqlText += " LastUpdateBy=@LastUpdateBy,";
//                    sqlText += " LastUpdateAt=@LastUpdateAt,";
//                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
//                    sqlText += " where Id=@Id";

//                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
//                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
//                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
//                    cmdUpdate.Parameters.AddWithValue("@DeductionAmount", vm.DeductionAmount);
//                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
//                    cmdUpdate.Parameters.AddWithValue("@DeductionTypeId", vm.DeductionTypeId);
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
//                        // throw new ArgumentNullException("Education Update", SalaryOtherDeductionVM.BranchId + " could not updated.");
//                    }

//                    #endregion Commit

//                    #endregion Update Settings
//                    iSTransSuccess = true;
//                }
//                else
//                {
//                    throw new ArgumentNullException("Salary Other Deduction Update", "Could not found any item.");
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
//                    retResults[1] = "Unexpected error to update Salary Other Deduction.";
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
        public string[] Delete(SalaryOtherDeductionVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSalaryOtherDeduction"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryOtherDeduction"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update SalaryOtherDeduction set";
                        sqlText += " DeductionAmount=0";
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
                        throw new ArgumentNullException("Salary Other Deduction Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Other Deduction Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Salary Other Deduction Information.";
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

        public string[] InsertSalaryOtherDeductionNew(int FiscalYearDetailId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
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
            retResults[5] = "Employee SalaryOtherDeduction Process"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            List<EmployeeInfoVM> employeeVms = new List<EmployeeInfoVM>();
            EmployeeInfoVM employeeVm;

            List<EmployeeOtherDeductionVM> EmployeeOtherDeductions = new List<EmployeeOtherDeductionVM>();
            EmployeeOtherDeductionVM EmployeeOtherDeduction;

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
////                employeeVms = new List<EmployeeInfoVM>();

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
                var sqlTextSalaryTaxDetail = "";
                sqlTextSalaryTaxDetail += @" INSERT INTO SalaryOtherDeduction
(
  EmployeeId, DeductionAmount, FiscalYearDetailId, DeductionTypeId, ProjectId, SectionId, DepartmentId, DesignationId
, Remarks, IsActive, IsArchive, CreatedBy, CreatedAt, CreatedFrom,EmployeeStatus,GradeId
) VALUES (
  @EmployeeId, @DeductionAmount, @FiscalYearDetailId, @DeductionTypeId, @ProjectId, @SectionId, @DepartmentId, @DesignationId
, @Remarks, @IsActive, @IsArchive, @CreatedBy, @CreatedAt, @CreatedFrom,@EmployeeStatus,@GradeId
) ";
                #endregion str

                if (employeeVms.Count > 0)
                {
                    foreach (EmployeeInfoVM employee in employeeVms)
                    {
                        #region Delete ExistingSalaryTaxDetail
                        sqlText = @"Delete SalaryOtherDeduction ";
                        sqlText += " where 1=1 and EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId";

                        SqlCommand cmdDeletePFDetail = new SqlCommand(sqlText, currConn, transaction);
                        cmdDeletePFDetail.Parameters.AddWithValue("@EmployeeId", employee.Id);
                        cmdDeletePFDetail.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                        cmdDeletePFDetail.ExecuteNonQuery();

                        #endregion

                        #region EmployeePF
                        sqlText = @"select * from EmployeeOtherDeduction";
                        sqlText += " where 1=1 and EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId";

                        SqlCommand cmdepf = new SqlCommand(sqlText, currConn, transaction);
                        cmdepf.Parameters.AddWithValue("@EmployeeId", employee.Id);
                        cmdepf.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                        EmployeeOtherDeductions = new List<EmployeeOtherDeductionVM>();
                        using (SqlDataReader drEOE = cmdepf.ExecuteReader())
                        {
                            while (drEOE.Read())
                            {
                                EmployeeOtherDeduction = new EmployeeOtherDeductionVM();
                                EmployeeOtherDeduction.DeductionAmount = Convert.ToDecimal(drEOE["DeductionAmount"]);
                                EmployeeOtherDeduction.DeductionTypeId = Convert.ToInt32(drEOE["DeductionTypeId"]);
                                EmployeeOtherDeduction.Remarks = drEOE["Remarks"].ToString();

                                EmployeeOtherDeductions.Add(EmployeeOtherDeduction);
                            }
                            drEOE.Close();
                        }
                        #endregion EmployeePF
                        if (EmployeeOtherDeductions.Count > 0)
                        {
                            foreach (EmployeeOtherDeductionVM EOE in EmployeeOtherDeductions)
                            {
                                #region SqlExecution
                                
                                SqlCommand cmdInsert = new SqlCommand(sqlTextSalaryTaxDetail, currConn);
                                cmdInsert.Parameters.AddWithValue("@EmployeeId", employee.Id);
                                cmdInsert.Parameters.AddWithValue("@DeductionAmount", EOE.DeductionAmount);
                                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                                cmdInsert.Parameters.AddWithValue("@DeductionTypeId", EOE.DeductionTypeId);
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
        
        //==================SelectAllForReport=================
        public List<SalaryOtherDeductionVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int DTId = 0, string Orderby = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryOtherDeductionVM> vms = new List<SalaryOtherDeductionVM>();
            SalaryOtherDeductionVM vm;
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
,sa.EmpName,sa.Code,sa.Designation,sa.Department, sa.JoinDate, sa.Section, sa.Project, sa.GrossSalary, sa.BasicSalary
,sa.DeductionAmount
,sa.FiscalYearDetailId
,fyd.PeriodStart                 
,sa.PeriodName
,sa.DeductionTypeId
,sa.TypeName DeductionType
,sa.DesignationId, sa.DepartmentId, sa.SectionId, sa.ProjectId
,sa.Remarks
From ViewSalaryOtherDeduction sa 
left outer join grade g on sa.gradeId = g.id
left outer join FiscalYearDetail fyd on sa.FiscalYearDetailId =fyd.Id
Where 1=1 and sa.DeductionAmount > 0
";
                if (fid != 0)
                {
                    sqlText += @" and sa.FiscalYearDetailId>='" + fid + "'";
                }
                if (fidTo != 0)
                {
                    sqlText += @" and sa.FiscalYearDetailId<='" + fidTo + "'";
                }
                if (DTId != 0)
                {
                    sqlText += @" and sa.DeductionTypeId='" + DTId + "'";
                }

                if (ProjectId != "0_0")
                    sqlText += " and sa.ProjectId=@ProjectId";

                if (DepartmentId != "0_0")
                    sqlText += " and sa.DepartmentId=@DepartmentId ";

                if (SectionId != "0_0")
                    sqlText += " and sa.SectionId=@SectionId ";

                if (DesignationId != "0_0")
                    sqlText += " and sa.DesignationId=@DesignationId ";

                if (CodeF != "0_0")
                    sqlText += " and sa.Code>= @CodeF";

                if (CodeT != "0_0")
                    sqlText += " and sa.Code<= @CodeT";

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
                    vm = new SalaryOtherDeductionVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.DeductionTypeId = Convert.ToInt32(dr["DeductionTypeId"]);
                    vm.DeductionType = dr["DeductionType"].ToString();

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


    }

}
