using Excel;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace SymServices.Payroll
{
    public class EmployeeOtherDeductionDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL cdal = new CommonDAL();
        #endregion

        //#region Methods


        //==================Get All Distinct FiscalPeriodName =================
        public List<EmployeeOtherDeductionVM> GetPeriodname()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeOtherDeductionVM> vms = new List<EmployeeOtherDeductionVM>();
            EmployeeOtherDeductionVM vm;
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
from ViewEmployeeOtherDeduction ve
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
                    vm = new EmployeeOtherDeductionVM();
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
        public List<EmployeeOtherDeductionVM> SelectAll(string empid = null, int? fid = null, int? DTId = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeOtherDeductionVM> vms = new List<EmployeeOtherDeductionVM>();
            EmployeeOtherDeductionVM vm;
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
,fs.PeriodName
,ea.DeductionDate
,ea.DeductionTypeId
,edt.Name DeductionType

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
From EmployeeOtherDeduction ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
left outer join  FiscalYearDetail fs on ea.FiscalYearDetailId=fs.Id
left outer join EarningDeductionType edt on ea.DeductionTypeId = edt.Id and edt.IsEarning = 0 
Where 1=1 and  ea.IsArchive=0  
";
                if (!string.IsNullOrEmpty(empid))
                {
                    sqlText += @" and ea.EmployeeId='" + empid + "'";
                }

                if (fid != null && fid != 0)
                {
                    sqlText += @" and ea.FiscalYearDetailId='" + fid + "'";
                }

                if (DTId != null && DTId != 0)
                {
                    sqlText += @" and ea.DeductionTypeId='" + DTId + "'";
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
                    vm = new EmployeeOtherDeductionVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.DeductionDate = Ordinary.StringToDate(dr["DeductionDate"].ToString());
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
        public EmployeeOtherDeductionVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeOtherDeductionVM vm = new EmployeeOtherDeductionVM();

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
,ea.DeductionDate
,ea.DeductionTypeId
,edt.Name DeductionType

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

From EmployeeOtherDeduction ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
left outer join EarningDeductionType edt on ea.DeductionTypeId = edt.Id and edt.IsEarning = 0 
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
                    vm = new EmployeeOtherDeductionVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.DeductionDate = Ordinary.StringToDate(dr["DeductionDate"].ToString());
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
        public EmployeeOtherDeductionVM SelectByIdandFiscalyearDetail(string empId, int FiscalYearDetailId = 0, string edType = "0")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeOtherDeductionVM vm = new EmployeeOtherDeductionVM();
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
,ea.DeductionTypeId
,ea.Remarks
,ISNULL(ea.GrossSalary, 0)  GrossSalary
,ISNULL(ea.BasicSalary, 0)  BasicSalary
,ISNULL(ea.Days, 0)         Days


From EmployeeOtherDeduction ea 
Where ea.IsArchive=0  and ea.EmployeeId=@Id 
and ea.FiscalYearDetailId=@FiscalYearDetailId
and ea.DeductionTypeId=@DeductionTypeId
";
                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@Id", empId);
                objCommVehicle.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                objCommVehicle.Parameters.AddWithValue("@DeductionTypeId", edType);
                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeOtherDeductionVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.DeductionDate = Ordinary.StringToDate(dr["DeductionDate"].ToString());
                    vm.DeductionTypeId = Convert.ToInt32(dr["DeductionTypeId"]);
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
        public string[] Insert(EmployeeOtherDeductionVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeOtherDeduction"; //Method Name
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
                sqlText += " SELECT   count(Id) FROM EmployeeOtherDeduction ";
                sqlText += @" WHERE EmployeeId=@EmployeeId and  FiscalYearDetailId=@FiscalYearDetailId
 and  DeductionTypeId=@DeductionTypeId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                cmdExist.Parameters.AddWithValue("@DeductionTypeId", vm.DeductionTypeId);
                var exeRes = cmdExist.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    sqlText = "  ";
                    sqlText += " delete FROM EmployeeOtherDeduction ";
                    sqlText += @" WHERE EmployeeId=@EmployeeId and  FiscalYearDetailId=@FiscalYearDetailId 
                                and  DeductionTypeId=@DeductionTypeId";
                    SqlCommand cmdExistD = new SqlCommand(sqlText, currConn);
                    cmdExistD.Transaction = transaction;
                    cmdExistD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExistD.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdExistD.Parameters.AddWithValue("@DeductionTypeId", vm.DeductionTypeId);

                    var exeResD = cmdExistD.ExecuteScalar();
                }

                #endregion Exist
                #region Save
                vm.Id = cdal.NextId("EmployeeOtherDeduction", currConn, transaction).ToString();
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeOtherDeduction
(Id
, EmployeeId
, DeductionAmount
, FiscalYearDetailId
, DeductionDate
, DeductionTypeId
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
, @DeductionDate
, @DeductionTypeId
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
                    cmdInsert.Parameters.AddWithValue("@DeductionDate", vm.DeductionDate);
                    cmdInsert.Parameters.AddWithValue("@DeductionTypeId", vm.DeductionTypeId);

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
                 int count=Convert.ToInt32(cmdInsert.ExecuteNonQuery());
                 if (count < 0)
                 { 
                  throw new ArgumentNullException("Please Input EmployeeOtherDeduction Value", "");
                 }
                }
                else
                {
                    retResults[1] = "Please Input EmployeeOtherDeduction Value";
                    throw new ArgumentNullException("Please Input EmployeeOtherDeduction Value", "");
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
        //==================Update =================
        public string[] Update(EmployeeOtherDeductionVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeOtherDeduction Update"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeOtherDeduction"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist
                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeeOtherDeduction ";
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
                    sqlText = "update EmployeeOtherDeduction set";
                    sqlText += " EmployeeId=@EmployeeId";
                    sqlText += " , DeductionAmount=@DeductionAmount";
                    sqlText += " , FiscalYearDetailId=@FiscalYearDetailId";
                    sqlText += " , DeductionDate=@DeductionDate";
                    sqlText += " , DeductionTypeId=@DeductionTypeId";

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
                    cmdUpdate.Parameters.AddWithValue("@DeductionDate", vm.DeductionDate);
                    cmdUpdate.Parameters.AddWithValue("@DeductionTypeId", vm.DeductionTypeId);
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

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EmployeeOtherDeductionVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeOtherDeduction Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update EmployeeOtherDeduction.";
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
        //==================Delete =================
        public string[] Delete(EmployeeOtherDeductionVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeOtherDeduction"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeOtherDeduction"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeOtherDeduction set";
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




                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("EmployeeOtherDeduction Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeOtherDeduction Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete EmployeeOtherDeduction Information.";
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
        //==================SelectAllForReport=================
        public List<EmployeeOtherDeductionVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int DTid = 0, string Orderby=null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeOtherDeductionVM> vms = new List<EmployeeOtherDeductionVM>();
            EmployeeOtherDeductionVM vm;
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
,ea.EmpName,ea.Code,ea.Designation,ea.Department, ea.JoinDate, ea.Section, ea.Project, ea.GrossSalary, ea.BasicSalary
,ea.DeductionAmount
,fyd.PeriodStart                 
,ea.FiscalYearDetailId
,ea.PeriodName
,ea.DeductionDate
,ea.DeductionTypeId
,ea.TypeName DeductionType
,ea.DesignationId, ea.DepartmentId, ea.SectionId, ea.ProjectId
,ea.Remarks
From ViewEmployeeOtherDeduction ea 
left outer join grade g on ea.gradeId = g.id
left outer join FiscalYearDetail fyd on ea.FiscalYearDetailId =fyd.Id
Where 1=1 
 --and ea.DeductionAmount > 0
";

                if (fid != 0)
                {
                    sqlText += @" and ea.FiscalYearDetailId>='" + fid + "'";
                }
                if (fidTo != 0)
                {
                    sqlText += @" and ea.FiscalYearDetailId<='" + fidTo + "'";
                }

                if (DTid != 0)
                {
                    sqlText += @" and ea.DeductionTypeId='" + DTid + "'";
                }

                if (ProjectId != "0_0")
                    sqlText += " and ea.ProjectId=@ProjectId";

                if (DepartmentId != "0_0")
                    sqlText += " and ea.DepartmentId=@DepartmentId ";

                if (SectionId != "0_0")
                    sqlText += " and ea.SectionId=@SectionId ";

                if (DesignationId != "0_0")
                    sqlText += " and ea.DesignationId=@DesignationId ";

                if (CodeF != "0_0")
                    sqlText += " and ea.Code>= @CodeF";

                if (CodeT != "0_0")
                    sqlText += " and ea.Code<= @CodeT";

                //sqlText += " ORDER BY ea.FiscalYearDetailId, ea.Department, ea.Section, ea.Code ";
                sqlText += " ORDER BY ea.FiscalYearDetailId";

                if (Orderby == "DCG")
                    sqlText += " , ea.department, ea.code, g.sl";
                else if (Orderby == "DDC")
                    sqlText += " ,ea.department, ea.JoinDate, ea.code";
                else if (Orderby == "DGC")
                    sqlText += " , ea.department, g.sl, ea.code";
                else if (Orderby == "DGDC")
                    sqlText += ", ea.department, g.sl, ea.JoinDate, ea.code";
                else if (Orderby == "CODE")
                    sqlText += ", ea.code";

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
                    vm = new EmployeeOtherDeductionVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.DeductionDate = Ordinary.StringToDate(dr["DeductionDate"].ToString());
                    vm.DeductionTypeId = Convert.ToInt32(dr["DeductionTypeId"]);
                    vm.DeductionType = dr["DeductionType"].ToString();

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
        #region ImportExport
        public string[] ImportExcelFile(string Fullpath, string fileName, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, int FYDId = 0)
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
            retResults[5] = "EmployeeOtherDeductionVM"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataSet ds = new DataSet();
            #endregion
            EmployeeInfoDAL _dalemp = new EmployeeInfoDAL();
            ViewEmployeeInfoVM empVM = new ViewEmployeeInfoVM();
            FiscalYearDAL fydal = new FiscalYearDAL();
            EarningDeductionTypeDAL eddal = new EarningDeductionTypeDAL();
            FiscalYearDetailVM FYDVM = new FiscalYearDetailVM();
            #region try
            try
            {
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
                    EmployeeOtherDeductionVM vm = new EmployeeOtherDeductionVM();
                    //empVM=_dalemp.se
                    empVM = _dalemp.ViewSelectAllEmployee(item["EmpCode"].ToString(), null, null, null, null, null, null, currConn, transaction).FirstOrDefault();
                    if (empVM == null || empVM.Id == null)
                    {
                        throw new ArgumentNullException("Employee Code " + item["EmpCode"].ToString() + " Not in System", "Employee Code " + item["EmpCode"].ToString() + " Not in System");
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
                            vm.DeductionTypeId = eddal.SelectById(Convert.ToInt32(item["DTId"].ToString()), currConn, transaction).Id;
                            if (vm.DeductionTypeId == 0)
                            {
                                throw new ArgumentNullException("Earning /Deduction not in System,", "Fiscal Period " + item["EDId"].ToString() + " Not in System");
                            }
                            else
                            {
                                if (!Ordinary.IsNumeric(item["Amount"].ToString()))
                                {
                                    throw new ArgumentNullException("Please input the Numeric Value in Amount", "Please input the Numeric Value in Amount");
                                }
                                else
                                {
                                    vm.DeductionDate = FYDVM.PeriodStart;
                                    vm.EmployeeId = empVM.Id;
                                    vm.FiscalYearDetailId = Convert.ToInt32(FYDVM.Id);
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
                                    
                                    
                                    
                                    retResults = Insert(vm, currConn, transaction);
                                    if (retResults[0] != "Success")
                                    {
                                        throw new ArgumentNullException("EmployeeOtherDeduction Update", "Could not found any item.");
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
                retResults[0] = "Fail";
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
        public System.Data.DataTable ExportExcelFile(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, int DTId = 0, string Orderby = null)
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
                var edName = eddal.SelectById(DTId).Name;

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
select * 
from (select Code EmpCode,EmpName,JoinDate, ISNULL(GradeSL,99) GradeSL,ISNULL(Grade+'-'+StepName,'NA') Grade
, ISNULL(NULLIF(Designation,'=NA='), 'NA')Designation
, ISNULL(NULLIF(Department,'=NA='), 'NA')Department
, ISNULL(NULLIF(Section,'=NA='), 'NA')Section
, ISNULL(NULLIF(Project,'=NA='), 'NA')Project
, DeductionAmount Amount,FiscalYeardetailId FYDId,DeductiontypeId DTId
, GrossSalary, BasicSalary, ISNULL(Days,0) Days
from ViewEmployeeOtherDeduction where 1=1
";
                if (DTId != 0)
                {
                    sqlText += " and DeductiontypeId='" + DTId + "'  ";
                }
                if (fid != 0)
                {
                    sqlText += @" and FiscalYearDetailId='" + fid + "'";
                }
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                    sqlText += @" and ViewEmployeeOtherDeduction.ProjectId='" + ProjectId + "'";
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                    sqlText += @" and ViewEmployeeOtherDeduction.DepartmentId='" + DepartmentId + "'";
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                    sqlText += @" and ViewEmployeeOtherDeduction.SectionId='" + SectionId + "'";
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                    sqlText += @" and ViewEmployeeOtherDeduction.DesignationId='" + DesignationId + "'";
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    sqlText += @" and ViewEmployeeOtherDeduction.Code >='" + CodeF + "'";
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    sqlText += @" and ViewEmployeeOtherDeduction.Code<='" + CodeT + "'";
                #region Regular
                sqlText += @"
union all
select vws.Code EmpCode, vws.EmpName,JoinDate,ISNULL(GradeSL,99) GradeSL,ISNULL(Grade+'-'+StepName,'NA') Grade
, ISNULL(NULLIF(Designation,'=NA='), 'NA')Designation
, ISNULL(NULLIF(Department,'=NA='), 'NA')Department
, ISNULL(NULLIF(Section,'=NA='), 'NA')Section
, ISNULL(NULLIF(Project,'=NA='), 'NA')Project
, 0 Amount,0 FYDId,0 DTId
, GrossSalary, BasicSalary, 0 Days
from ViewEmployeeInformation vws
where 1=1  AND vws.IsActive=1 AND vws.IsArchive=0 
AND vws.Joindate <= @PeriodEnd
";
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

                sqlText += @" and EmployeeId not in(select EmployeeId from ViewEmployeeOtherDeduction where 1=1 ";
                if (DTId != 0)
                {
                    sqlText += @" AND DeductiontypeId='" + DTId + "'  ";
                }
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
,  0 Amount,0 FYDId,0 DTId
, GrossSalary, BasicSalary, 0 Days
from ViewEmployeeInformation vws
where 1=1  AND vws.IsActive=0 AND vws.IsArchive=0 
AND  vws.LeftDate >= @PeriodStart 
";
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

                sqlText += @" and EmployeeId not in(select EmployeeId from ViewEmployeeOtherDeduction where 1=1 ";
                if (DTId != 0)
                {
                    sqlText += @" AND DeductiontypeId='" + DTId + "'  ";
                }
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
                da.SelectCommand.Parameters.AddWithValue("@PeriodEnd",PeriodEnd);
                da.SelectCommand.Parameters.AddWithValue("@PeriodStart",PeriodStart);

                da.Fill(dt);
                dt.Columns.Add("Fiscal Period");
                dt.Columns.Add("Type");
                dt.Columns.Remove("GradeSl");
                dt.Columns.Remove("JoinDate");
                foreach (DataRow row in dt.Rows)
                {
                    row["Fiscal Period"] = fname;
                    row["Type"] = edName;
                    row["FYDId"] = fid;
                    row["DTId"] = DTId;
                }

                #region Value Round

                string[] columnNames = { "GrossSalary", "BasicSalary", "Amount" };

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
        public bool ExportExcelFilebackup(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, int DTId = 0, string Orderby=null)
        {
            try
            {
                FiscalYearDAL fdal = new FiscalYearDAL();
                EarningDeductionTypeDAL eddal = new EarningDeductionTypeDAL();
                var fname = fdal.FYPeriodDetail(fid,null,null).FirstOrDefault().PeriodName;
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
from ViewEmployeeOtherDeduction where 1=1 ";
                if (DTId != 0)
                {
                    sqlText += " and DeductiontypeId='" + DTId + "'  ";
                }
                if (fid != 0)
                {
                    sqlText += @" and FiscalYearDetailId='" + fid + "'";
                }

                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                    sqlText += @" and ViewEmployeeOtherDeduction.ProjectId='" + ProjectId + "'";
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                    sqlText += @" and ViewEmployeeOtherDeduction.DepartmentId='" + DepartmentId + "'";
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                    sqlText += @" and ViewEmployeeOtherDeduction.SectionId='" + SectionId + "'";
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                    sqlText += @" and ViewEmployeeOtherDeduction.DesignationId='" + DesignationId + "'";
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    sqlText += @" and ViewEmployeeOtherDeduction.Code >='" + CodeF + "'";
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    sqlText += @" and ViewEmployeeOtherDeduction.Code<='" + CodeT + "'";
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

                sqlText += @" and EmployeeId not in(select EmployeeId from EmployeeOtherDeduction)
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
