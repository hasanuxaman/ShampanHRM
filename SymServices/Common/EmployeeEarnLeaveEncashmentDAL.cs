////using Microsoft.Office.Interop.Excel;
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
using System.Drawing;
using System.IO;
using Excel;
using System.Configuration;
using SymViewModel.Leave;

namespace SymServices.Common
{
    public class EmployeeEarnLeaveEncashmentDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL cdal = new CommonDAL();
        #endregion

        //=================Get Distinct Period Name ================
        public List<EmployeeLeaveEncashmentVM> GetYear()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveEncashmentVM> vms = new List<EmployeeLeaveEncashmentVM>();
            EmployeeLeaveEncashmentVM vm;
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
select distinct [Year],[Remarks]
from EarnLeaveEncashmentStatement 
where 1=1 
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeaveEncashmentVM();


                    vm.Year = dr["Year"].ToString();
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

        //=================Get Employee Otherearning Information by fiscal year id ================
        public List<EmployeeLeaveEncashmentVM> GetEmpOtherEaringByFid(int fid)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveEncashmentVM> vms = new List<EmployeeLeaveEncashmentVM>();
            EmployeeLeaveEncashmentVM vm;
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
emoe.EmployeeId
,vw.EmpName
,vw.BasicSalary
,vw.GrossSalary
,emoe.EarningAmount
,emoe.FiscalYearDetailId
,emoe.Remarks
,fyd.PeriodName
from EmployeeOtherEarning emoe 
left outer join  ViewEmployeeInformation vw on vw.Id=emoe.EmployeeId
left outer join FiscalYearDetail fyd on fyd.Id=emoe.FiscalYearDetailId
where emoe.IsActive = 1 and emoe.IsArchive=0 
";
                if (fid != null && fid != 0)
                {
                    sqlText += @" and emoe.FiscalYearDetailId=@FiscalYearDetailId";
                }
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (fid != null && fid != 0)
                {
                    objComm.Parameters.AddWithValue("@FiscalYearDetailId", fid);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeaveEncashmentVM();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.EarningAmount = Convert.ToDecimal(dr["EarningAmount"].ToString());
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodName = dr["PeriodName"].ToString();
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
        public List<EmployeeLeaveEncashmentVM> SelectAll(string empid = null, int? fid = null, int? ETId = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveEncashmentVM> vms = new List<EmployeeLeaveEncashmentVM>();
            EmployeeLeaveEncashmentVM vm;
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
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project
,ea.Year
,ea.[EncashmentBalance]
,ea.[EncashmentRatio]

,e.DesignationId, e.DepartmentId, e.SectionId, e.ProjectId

,ea.Remarks

From EarnLeaveEncashmentStatement ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.Id

Where 1=1  
";
                if (!string.IsNullOrEmpty(empid))
                {
                    sqlText += @" and ea.EmployeeId=@EmployeeId ";
                }

                if (fid != null && fid != 0)
                {
                    sqlText += @" and ea.Year=@Year";
                }

                if (ETId != null && ETId != 0)
                {
                    sqlText += @" and ea.EarningTypeId=@EarningTypeId";
                }

                sqlText += @" ORDER BY e.Department,e.EmpName desc";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(empid))
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", empid);
                }
                if (fid != null && fid != 0)
                {
                    objComm.Parameters.AddWithValue("@Year", fid);
                }

                if (ETId != null && ETId != 0)
                {
                    objComm.Parameters.AddWithValue("@EarningTypeId", ETId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeaveEncashmentVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EncashmentBalance = Convert.ToDecimal(dr["EncashmentBalance"]);
                    vm.Year = dr["Year"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();



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

        public List<EmployeeLeaveBalanceVM> EmployeeLeaveEncashmentBalance(string EmployeeId, string leaveyear = "0")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveBalanceVM> employeeLeaves = new List<EmployeeLeaveBalanceVM>();
            EmployeeLeaveBalanceVM employeeLeave;
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
                sqlText = @"Select (AnnualLeaveEntitle-AnnualLeaveTaken) Have from EarnLeaveStatement
where EmployeeId=@EmployeeId
and [Year]=@LeaveYear
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                objComm.Parameters.AddWithValue("@LeaveYear", leaveyear);
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeLeave = new EmployeeLeaveBalanceVM();
                    employeeLeave.Have = dr["HAVE"].ToString();
                    employeeLeaves.Add(employeeLeave);
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
            return employeeLeaves;
        }

        //==================SelectByID=================
        public EmployeeLeaveEncashmentVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeLeaveEncashmentVM vm = new EmployeeLeaveEncashmentVM();

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
,isnull(ea.EncashmentBalance,0)EncashmentBalance,isnull(ea.EncashmentRatio,50)EncashmentRatio
,ea.Remarks
,ea.Year
,Isnull(ea.IsApproved,'N')IsApproved
From EarnLeaveEncashmentStatement ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
Where 1=1  and ea.id=@Id 

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
                    vm = new EmployeeLeaveEncashmentVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Year = dr["Year"].ToString();
                    vm.EncashmentBalance = Convert.ToDecimal(dr["EncashmentBalance"]);
                    vm.EncashmentRatio = Convert.ToDecimal(dr["EncashmentRatio"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.IsApproved = dr["IsApproved"].ToString();
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
        public EmployeeLeaveEncashmentVM SelectByIdandFiscalyearDetail(string empId, string FiscalYearDetailId = "0", string edType = "0", string salaryMonth = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeLeaveEncashmentVM vm = new EmployeeLeaveEncashmentVM();
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
,ISNULL(ea.EncashmentBalance, 0 ) EncashmentBalance
,ISNULL(ea.EncashmentRatio, 50 ) EncashmentRatio

,ea.Year
,ea.Remarks
From EarnLeaveEncashmentStatement ea 
Where  ea.EmployeeId=@Id 
and ea.Year=@FiscalYearDetailId
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
                    vm = new EmployeeLeaveEncashmentVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EncashmentBalance = Convert.ToDecimal(dr["EncashmentBalance"]);
                    vm.Year = dr["Year"].ToString(); ;
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
        public string[] Insert(EmployeeLeaveEncashmentVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction, string InputType = null)
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
            retResults[5] = "InsertEmployeeEarnLeaveEncashment"; //Method Name
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
                sqlText += " SELECT   count(Id) FROM EarnLeaveEncashmentStatement ";
                sqlText += @" WHERE EmployeeId=@EmployeeId and  Year=@FiscalYearDetailId
                                ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@FiscalYearDetailId", vm.Year);

                var exeRes = cmdExist.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    sqlText = "  ";
                    sqlText += " delete FROM EarnLeaveEncashmentStatement ";
                    sqlText += @" WHERE EmployeeId=@EmployeeId and  Year=@FiscalYearDetailId
                                 ";

                    SqlCommand cmdExistD = new SqlCommand(sqlText, currConn);
                    cmdExistD.Transaction = transaction;
                    cmdExistD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExistD.Parameters.AddWithValue("@FiscalYearDetailId", vm.Year);

                    var exeResD = cmdExistD.ExecuteScalar();
                }

                #endregion Exist
                #region Save

                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EarnLeaveEncashmentStatement
(

 EmployeeId
, EncashmentBalance
,EncashmentRatio
, Year
, Remarks

) VALUES (

 @EmployeeId
, @EncashmentBalance
,@EncashmentRatio
, @Year
, @Remarks

) SELECT SCOPE_IDENTITY();";
                    ///Fetching Data



                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@EncashmentBalance", vm.EncashmentBalance);
                    cmdInsert.Parameters.AddWithValue("@EncashmentRatio", vm.EncashmentRatio);
                    cmdInsert.Parameters.AddWithValue("@Year", vm.Year);
                    cmdInsert.Parameters.AddWithValue("@IsApproved", "N");
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Transaction = transaction;
                    var exec = cmdInsert.ExecuteScalar();
                    int transResult = Convert.ToInt32(exec);
                    vm.Id = transResult.ToString();

                }
                else
                {
                    retResults[1] = "Please Input EarnLeaveEncashment Value";
                    throw new ArgumentNullException("Please Input EarnLeaveEncashment Value", "");
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
        public string[] Update(EmployeeLeaveEncashmentVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeOtherEarning Update"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeOtherEarning"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist
                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeeOtherEarning ";
                    sqlText += " WHERE EmployeeId=@EmployeeId AND  Id<>@Id and  EarningDate=@EarningDate";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExist.Parameters.AddWithValue("@EarningDate", Ordinary.DateToString(vm.EarningDate));
                    var exeRes = cmdExist.ExecuteScalar();
                    int objfoundId = Convert.ToInt32(exeRes);

                    if (objfoundId > 0)
                    {
                        retResults[1] = "Earning already used for this Employee on this period!";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Earning already used for this Employee on this period!", "");
                    }

                    #endregion Exist

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeOtherEarning set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " EarningAmount=@EarningAmount,";
                    sqlText += " FiscalYearDetailId=@FiscalYearDetailId,";
                    sqlText += " EarningDate=@EarningDate,";
                    sqlText += " EarningTypeId=@EarningTypeId,";
                    sqlText += " IsApproved=@IsApproved,";

                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@EarningAmount", vm.EarningAmount);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@EarningDate", vm.EarningDate);
                    cmdUpdate.Parameters.AddWithValue("@EarningTypeId", vm.EarningTypeId);
                    cmdUpdate.Parameters.AddWithValue("@IsApproved", "N");
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EmployeeLeaveEncashmentVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeOtherEarning Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update EmployeeOtherEarning.";
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
        public string[] Delete(EmployeeLeaveEncashmentVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeOtherEarning"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeOtherEarning"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used
                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeOtherEarning set";
                        sqlText += " EarningAmount=0,";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
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
                        throw new ArgumentNullException("EmployeeOtherEarning Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeOtherEarning Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete EmployeeOtherEarning Information.";
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
        //==================Post =================
        public string[] Post(EmployeeLeaveEncashmentVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = " PFDetails Post"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
            #endregion
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Post"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EarnLeaveEncashmentStatement set";
                    sqlText += "  IsApproved=@IsApproved";
                    sqlText += @" where Id=@Id";

                    sqlText += @" 
                                 update EmployeeLeaveStructure set";
                    sqlText += " OpeningLeaveDays=OpeningLeaveDays-@Encashment";
                    sqlText += @" where LeaveType_E='Annual Leave' And EmployeeId=@EmployeeId";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Encashment", vm.EncashmentBalance);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@IsApproved", "Y");

                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EEHeadVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Post GFDetails", "Could not found any item.");
                }

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }
                retResults[0] = "Success";
                retResults[1] = "Data  Successfully Post.";
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
        //==================OvertimeAmount=================
        public string OvertimeAmount(string OTHrs, string OTHrsSpecial, string Code, string PayrollPeriodId = "0", string EmployeeId = "0", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            EmployeeInfoDAL _empDAL = new EmployeeInfoDAL();
            decimal basicSal = 0;

            //basicSal = _empDAL.ViewSelectAllEmployee().Where(c => c.Code.Equals(Code));
            ////basicSal = _empDAL.ViewSelectAllEmployee(Code, null, null, null, null, null, null, null, null).FirstOrDefault().BasicSalary;
            basicSal = _empDAL.SelectEmployeeBasicSalary(EmployeeId, PayrollPeriodId, VcurrConn, Vtransaction);

            SettingDAL _setDAL = new SettingDAL();
            string otDaysOfMonth = "";
            string otNormalRate = "";
            string otSpecialRate = "";
            decimal otHourlyrate = 0;
            decimal otNormalAmount = 0;
            decimal otSpecialAmount = 0;
            decimal otTotalAmount = 0;

            decimal decOTHrs = 0;
            decimal decOTHrsSpecial = 0;
            int OTHrs1 = 0;
            int OTHrs2 = 0;
            int OTHrsSpecial1 = 0;
            int OTHrsSpecial2 = 0;

            try
            {
                #region Overtime Amount

                //decOTHrs = OTHrs.Split('.')[0] * 60 + OTHrs.Split('.')[1];

                otDaysOfMonth = _setDAL.settingValue("OverTime", "DOM", null, null);
                otNormalRate = _setDAL.settingValue("OverTime", "NormalRate", null, null);
                otSpecialRate = _setDAL.settingValue("OverTime", "SpecialRate", null, null);
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                otHourlyrate = Convert.ToDecimal(basicSal / Convert.ToDecimal(otDaysOfMonth) / 8);

                if (CompanyName.ToLower() == "tib")
                {
                    otHourlyrate = (basicSal / 85);

                    string[] OTHrsparts = OTHrs.Split('.');
                    OTHrs1 = int.Parse(OTHrsparts[0]);


                    if (OTHrsparts.Length.ToString() == "2")
                    {
                        OTHrs2 = int.Parse(OTHrsparts[1].PadRight(2, '0'));

                    }

                    string[] OTHrsSpecialparts = OTHrsSpecial.Split('.');
                    OTHrsSpecial1 = int.Parse(OTHrsSpecialparts[0]);

                    if (OTHrsSpecialparts.Length.ToString() == "2")
                    {
                        OTHrsSpecial2 = int.Parse(OTHrsSpecialparts[1].PadRight(2, '0'));

                    }
                    decimal HrsMant = (Convert.ToDecimal(OTHrs1) * otHourlyrate);
                    decimal OTAmnt = (((Convert.ToDecimal(OTHrs2) * otHourlyrate)) / 60);
                    decimal TotalMant = Ordinary.NumericFormat((HrsMant + OTAmnt), 2);

                    otNormalAmount = TotalMant;
                    otSpecialAmount = ((Convert.ToDecimal(OTHrsSpecial1) * otHourlyrate) + ((Convert.ToDecimal(OTHrsSpecial2) * otHourlyrate)) / 60);
                }

                else
                {
                    otNormalAmount = Convert.ToDecimal(OTHrs) * otHourlyrate * Convert.ToDecimal(otNormalRate);
                    otSpecialAmount = Convert.ToDecimal(OTHrsSpecial) * otHourlyrate * Convert.ToDecimal(otSpecialRate);
                }


                otTotalAmount = otNormalAmount + otSpecialAmount;
                #endregion Overtime Amount
            }

            catch (Exception ex)
            {
                throw new ArgumentNullException("", "Exception:" + ex + FieldDelimeter + ex.Message.ToString());
            }
            //return result;
            return otTotalAmount.ToString();
        }

        //==================SelectAllForReport=================
        public List<EmployeeLeaveEncashmentVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int ETid = 0, string Orderby = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveEncashmentVM> vms = new List<EmployeeLeaveEncashmentVM>();
            EmployeeLeaveEncashmentVM vm;
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
,ea.EarningAmount
,fyd.PeriodStart                 
,ea.FiscalYearDetailId
,ea.PeriodName
,ea.EarningDate
,ea.EarningTypeId
,ea.TypeName EarningType
,ea.DesignationId, ea.DepartmentId, ea.SectionId, ea.ProjectId
,ea.Remarks
From ViewEmployeeOtherEarning ea 
left outer join grade g on ea.gradeId = g.id
left outer join FiscalYearDetail fyd on ea.FiscalYearDetailId =fyd.Id
Where 1=1
 --and ea.EarningAmount > 0
";

                if (fid != 0)
                {
                    sqlText += @" and ea.FiscalYearDetailId>='" + fid + "'";
                }
                if (fidTo != 0)
                {
                    sqlText += @" and ea.FiscalYearDetailId<='" + fidTo + "'";
                }
                if (ETid != 0)
                {
                    sqlText += @" and ea.EarningTypeId='" + ETid + "'";
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
                    vm = new EmployeeLeaveEncashmentVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EarningAmount = Convert.ToDecimal(dr["EarningAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.EarningDate = Ordinary.StringToDate(dr["EarningDate"].ToString());
                    vm.EarningTypeId = Convert.ToInt32(dr["EarningTypeId"]);
                    vm.EarningType = dr["EarningType"].ToString();

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

        //        #region ImportExport
        public string[] ImportExcelFile(string Fullpath, string fileName, ShampanIdentityVM auditvm
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, int FYDId = 0)
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
            retResults[5] = "EmployeeOtherEaringVM"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            EmployeeInfoDAL _dalemp = new EmployeeInfoDAL();
            ViewEmployeeInfoVM empVM = new ViewEmployeeInfoVM();

            #region try
            try
            {
                DataSet ds = new DataSet();
                //DataSet ds = Ordinary.UploadExcel(FullPath,fileName);
                System.Data.DataTable dt = new System.Data.DataTable();
                FileStream stream = System.IO.File.Open(Fullpath, FileMode.Open, FileAccess.Read);
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
                var a = "";

                #region SaveOtherEarning

                retResults = SaveOtherEarning(dt, auditvm, currConn, transaction, FYDId);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException("EarnLeaveEncashmentUpdate", "Could not found any item.");
                }

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
                //retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                retResults[0] = "Fail";//Success or Fail
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


        ////        public string[] SaveFixesOtSalaryProcess(int FYDId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null
        ////            , ShampanIdentityVM auditvm = null)
        ////        {
        ////            #region Initializ
        ////            string[] retResults = new string[6];
        ////            retResults[0] = "Fail";//Success or Fail
        ////            retResults[1] = "Fail";// Success or Fail Message
        ////            //retResults[2] = Id.ToString();// Return Id
        ////            //retResults[3] = sqlText; //  SQL Query
        ////            retResults[4] = "ex"; //catch ex
        ////            retResults[5] = "EmployeeOtherEaringVM"; //Method Name

        ////            SqlConnection currConn = null;
        ////            SqlTransaction transaction = null;

        ////            string sqlText = "";

        ////            #endregion

        ////            #region try
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

        ////                DataTable dt = new DataTable();

        ////                #region sql statement

        ////                #region Get SalaryMonthId

        ////                sqlText = @"
        ////select top 1 * from FiscalYearDetail where id<@FiscalYearDetailsId order by id desc
        ////
        ////";
        ////                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
        ////                objComm.Parameters.AddWithValue("@FiscalYearDetailsId", FYDId);
        ////                string salaryId = "";
        ////                using (SqlDataReader dr = objComm.ExecuteReader())
        ////                {
        ////                    while (dr.Read())
        ////                    {
        ////                        salaryId = dr["Id"].ToString();
        ////                    }
        ////                    dr.Close();
        ////                }

        ////                #endregion

        ////                #region Get Employee Info

        ////                sqlText = "";

        ////                sqlText = @"
        ////select 
        ////emp.Code EmpCode
        ////,esg.EmployeeId
        ////,'2' EDId
        ////,0 Amount
        ////,esg.FixedOT NormalOT
        ////,0 SpecialOT
        ////,@FYDId FYDId
        ////,@SalaryMonthId SalaryMonthId
        ////from EmployeeStructureGroup esg 
        ////left outer join ViewEmployeeInformation emp on esg.EmployeeId=emp.EmployeeId
        ////where FixedOT  is not null
        ////
        ////";

        ////                objComm = new SqlCommand(sqlText,currConn,transaction);
        ////                objComm.Parameters.AddWithValue("@FYDId", FYDId);
        ////                objComm.Parameters.AddWithValue("@SalaryMonthId", FYDId);

        ////                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
        ////                dataAdapter.Fill(dt);

        ////                #endregion



        ////                #region Delete Existing EmployeeOtherEarning

        ////                //////////foreach (DataRow item in dt.Rows)
        ////                //////////{
        ////                //////////    string empId = item["EmployeeId"].ToString();
        ////                //////////    string EarningTypeId = item["EDId"].ToString();


        ////                //////////    sqlText = @"Delete EmployeeOtherEarning ";
        ////                //////////    sqlText += " where 1=1 and EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId and EarningTypeId=@EarningTypeId";
        ////                //////////    SqlCommand cmdDeletePFDetail = new SqlCommand(sqlText, currConn, transaction);
        ////                //////////    cmdDeletePFDetail.Parameters.AddWithValue("@EmployeeId", empId);
        ////                //////////    cmdDeletePFDetail.Parameters.AddWithValue("@FiscalYearDetailId", FYDId);
        ////                //////////    cmdDeletePFDetail.Parameters.AddWithValue("@EarningTypeId", EarningTypeId);
        ////                //////////    cmdDeletePFDetail.ExecuteNonQuery();

        ////                //////////}


        ////                #endregion

        ////                #endregion

        ////                retResults = SaveOtherEarning(dt, auditvm, currConn, transaction, FYDId, salaryId);
        ////            }
        ////            #endregion try

        ////            #region Catch and Finall

        ////            catch (Exception ex)
        ////            {
        ////                retResults[4] = ex.Message.ToString(); //catch ex
        ////                retResults[0] = "Fail";//Success or Fail
        ////                if (Vtransaction == null) { transaction.Rollback(); }
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

        public string[] SaveOtherEarning(DataTable dt, ShampanIdentityVM auditvm
    , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, int FYDId = 0, string SalaryId = "")
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
            retResults[5] = "EmployeeOtherEaringVM"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            EmployeeInfoDAL _dalemp = new EmployeeInfoDAL();
            ViewEmployeeInfoVM empVM = new ViewEmployeeInfoVM();
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
                var a = "";

                foreach (DataRow item in dt.Rows)
                {
                    EmployeeLeaveEncashmentVM vm = new EmployeeLeaveEncashmentVM();
                    //empVM=_dalemp.se
                    empVM = _dalemp.ViewSelectAllEmployee(item["EmpCode"].ToString(), null, null, null, null, null, null, currConn, transaction).FirstOrDefault();
                    if (empVM == null || empVM.Id == null)
                    {
                        throw new ArgumentNullException("Employee Code " + item["EmpCode"].ToString() + " Not in System", "Employee Code " + item["EmpCode"].ToString() + " Not in System");
                    }
                    else
                    {
                        if (!Ordinary.IsNumeric(item["EncashmentBalance"].ToString()))
                        {
                            throw new ArgumentNullException("Please input the Numeric Value in EncashmentBalance", "Please input the Numeric Value in EncashmentBalance");
                        }
                        if (string.IsNullOrWhiteSpace((item["Year"].ToString())))
                        {
                            throw new ArgumentNullException("Please input the Numeric Value in Year", "Please input the Numeric Value in Year");
                        }


                        vm.EmployeeId = empVM.Id;
                        vm.Year = item["Year"].ToString();
                        vm.EncashmentBalance = Convert.ToDecimal(item["EncashmentBalance"].ToString());
                        retResults = Insert(vm, currConn, transaction, "Excel");
                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException("EmployeeOtherDeduction Update", "Could not found any item.");
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
                retResults[1] = ex.Message.ToString().Split('\r').FirstOrDefault();
                retResults[0] = "Fail";//Success or Fail
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
        //#region Comments

        //        public string[] ExportExcelFileBackup(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid, int ETId)
        //        {
        //            return null;

        //            //////            string[] results = new string[6];
        //            //////            try
        //            //////            {
        //            //////                FiscalYearDAL fdal = new FiscalYearDAL();
        //            //////                EarningDeductionTypeDAL eddal = new EarningDeductionTypeDAL();
        //            //////                var fname = fdal.FYPeriodDetail(fid).FirstOrDefault().PeriodName;
        //            //////                var edName = eddal.SelectById(ETId).Name;
        //            //////                Application app = new Application();
        //            //////                _Workbook workbook = app.Workbooks.Add(System.Type.Missing);
        //            //////                _Worksheet worksheet = new Worksheet();
        //            //////                Microsoft.Office.Interop.Excel.Range excelCellrange;
        //            //////                app.Visible = false;
        //            //////                worksheet = workbook.Sheets["Sheet1"] as _Worksheet;
        //            //////                worksheet = workbook.ActiveSheet as _Worksheet;
        //            //////                worksheet.Name = "DataSheet";
        //            //////                worksheet.Cells[1, 1] = "Sl#";
        //            //////                worksheet.Cells[1, 2] = "EmpCode";
        //            //////                worksheet.Cells[1, 3] = "EmpName";
        //            //////                worksheet.Cells[1, 4] = "Designation";
        //            //////                worksheet.Cells[1, 5] = "Department";
        //            //////                worksheet.Cells[1, 6] = "Section";
        //            //////                worksheet.Cells[1, 7] = "Project";
        //            //////                worksheet.Cells[1, 8] = "TransactionType";
        //            //////                worksheet.Cells[1, 9] = "Amount";
        //            //////                worksheet.Cells[1, 10] = "OTHrs";
        //            //////                worksheet.Cells[1, 11] = "OTHrsSpecial";
        //            //////                worksheet.Cells[1, 12] = "FiscalPeriod";
        //            //////                worksheet.Cells[1, 13] = "Type";
        //            //////                worksheet.Cells[1, 14] = "FYDId";
        //            //////                worksheet.Cells[1, 15] = "EDId";

        //            //////                excelCellrange = worksheet.Range[worksheet.Cells[1, 2], worksheet.Cells[5000, 2]];
        //            //////                excelCellrange.NumberFormat = "@";



        //            //////                #region DataRead From DB
        //            //////                #region Variables
        //            //////                SqlConnection currConn = null;
        //            //////                string sqlText = "";
        //            //////                int j = 2;
        //            //////                #endregion

        //            //////                #region open connection and transaction
        //            //////                currConn = _dbsqlConnection.GetConnection();
        //            //////                if (currConn.State != ConnectionState.Open)
        //            //////                {
        //            //////                    currConn.Open();
        //            //////                }
        //            //////                #endregion open connection and transaction
        //            //////                #region sql statement
        //            //////                sqlText = @"select * from (select Code EmpCode,EmpName,
        //            //////(case when Designation is null then 'NA' when Designation='=NA=' then 'NA' else Designation end) Designation,
        //            //////(case when Department is null then 'NA' when Department='=NA=' then 'NA' else Department end) Department ,
        //            //////(case when Section is null then 'NA' when Section='=NA=' then 'NA' else Section end) Section,
        //            //////(case when Project is null then 'NA' when Project='=NA=' then 'NA' else Project end) Project,
        //            //////TypeName TransactionType,EarningAmount Amount,OTHrs NormalRate,OTHrsSpecial SpecialRate,FiscalYeardetailId FYDId,EarningTypeId EDId
        //            //////from ViewEmployeeOtherEarning where 1=1 ";
        //            //////                if (ETId != 0)
        //            //////                {
        //            //////                    sqlText += " and EarningTypeId='" + ETId + "'  ";
        //            //////                }
        //            //////                if (fid != 0)
        //            //////                {
        //            //////                    sqlText += @" AND FiscalYearDetailId='" + fid + "'";
        //            //////                }
        //            //////                sqlText += @"
        //            //////union all
        //            //////select Code EmpCode,EmpName,
        //            //////(case when Designation is null  then 'NA' when Designation='=NA=' then 'NA' else Designation end) Designation ,
        //            //////(case when Department is null then 'NA' when Department='=NA=' then 'NA' else Department end) Department,
        //            //////(case when Section is null then 'NA' when Section='=NA=' then 'NA' else Section end) Section,
        //            //////(case when Project is null then 'NA' when Project='=NA=' then 'NA' else Project end) Project
        //            //////,'LWP' TransactionType, 0 Amount,0 NormalRate,0 SpecialRate,0 FYDId,0 EDId
        //            //////from ViewEmployeeInformation vws 
        //            //////where 1=1 AND vws.IsActive=1 AND vws.IsArchive=0";
        //            //////                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
        //            //////                    sqlText += @" and vws.ProjectId='" + ProjectId + "'";
        //            //////                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
        //            //////                    sqlText += @" and vws.DepartmentId='" + DepartmentId + "'";
        //            //////                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
        //            //////                    sqlText += @" and vws.SectionId='" + SectionId + "'";
        //            //////                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
        //            //////                    sqlText += @" and vws.DesignationId='" + DesignationId + "'";
        //            //////                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
        //            //////                    sqlText += @" and vws.Code >='" + CodeF + "'";
        //            //////                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
        //            //////                    sqlText += @" and vws.Code<='" + CodeT + "'";

        //            //////                sqlText += @" and EmployeeId not in(select EmployeeId from EmployeeOtherEarning)
        //            //////) as a order by Department,Section,Project, EmpCode ";

        //            //////                SqlCommand objComm = new SqlCommand();
        //            //////                objComm.Connection = currConn;
        //            //////                objComm.CommandText = sqlText;
        //            //////                objComm.CommandType = CommandType.Text;
        //            //////                SqlDataAdapter da = new SqlDataAdapter(objComm);
        //            //////                System.Data.DataTable dt = new System.Data.DataTable();
        //            //////                da.Fill(dt);


        //            //////                foreach (DataRow item in dt.Rows)
        //            //////                {
        //            //////                    worksheet.Cells[j, 1] = j - 1;
        //            //////                    worksheet.Cells[j, 2] = item["EmpCode"].ToString();
        //            //////                    worksheet.Cells[j, 3] = item["EmpName"].ToString();
        //            //////                    worksheet.Cells[j, 4] = item["Designation"].ToString();
        //            //////                    worksheet.Cells[j, 5] = item["Department"].ToString();
        //            //////                    worksheet.Cells[j, 6] = item["Section"].ToString();
        //            //////                    worksheet.Cells[j, 7] = item["Project"].ToString();
        //            //////                    worksheet.Cells[j, 8] = item["TransactionType"].ToString();
        //            //////                    worksheet.Cells[j, 9] = item["Amount"].ToString();
        //            //////                    worksheet.Cells[j, 10] = item["NormalRate"].ToString();
        //            //////                    worksheet.Cells[j, 11] = item["SpecialRate"].ToString();
        //            //////                    worksheet.Cells[j, 12] = fname;
        //            //////                    worksheet.Cells[j, 13] = edName;
        //            //////                    worksheet.Cells[j, 14] = fid;
        //            //////                    worksheet.Cells[j, 15] = ETId;
        //            //////                    j++;
        //            //////                }
        //            //////                #endregion
        //            //////                #endregion
        //            //////                string xportFileName = string.Format(@"{0}" + FileName, Filepath);
        //            //////                // save the application
        //            //////                workbook.SaveAs(xportFileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //            //////                                Type.Missing,
        //            //////                                Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing,
        //            //////                                Type.Missing, Type.Missing, Type.Missing);
        //            //////                // Exit from the application
        //            //////                app.Quit();
        //            //////                releaseObject(worksheet);
        //            //////                releaseObject(workbook);
        //            //////                releaseObject(app);

        //            //////                #region SuccessResult
        //            //////                results[0] = "Success";
        //            //////                results[1] = "Data Save Successfully.";
        //            //////                //retResults[2] = vm.Id.ToString();
        //            //////                #endregion SuccessResult
        //            //////            }
        //            //////            catch (Exception ex)
        //            //////            {
        //            //////                results[4] = ex.Message.ToString(); //catch ex
        //            //////                return results;
        //            //////                throw ex;
        //            //////            }
        //            //////            return results;
        //            //////        }
        //            //////        private void releaseObject(object obj)
        //            //////        {
        //            //////            try
        //            //////            {
        //            //////                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        //            //////                obj = null;
        //            //////            }
        //            //////            catch (Exception ex)
        //            //////            {
        //            //////                string exMessage = ex.Message;
        //            //////                if (ex.InnerException != null)
        //            //////                {
        //            //////                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;
        //            //////                }
        //            //////            }
        //            //////            finally
        //            //////            {
        //            //////                GC.Collect();
        //            //////            }
        //        }

        //        #endregion

        //        #endregion ImportExport

        public DataTable ExportExcelFile(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid, int ETId, string Orderby)
        {
            DataTable dt = new DataTable();
            string[] results = new string[6];
            try
            {
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
select * from (select vws.Code EmpCode, vws.EmpName ,Year , EncashmentBalance
from EarnLeaveEncashmentStatement
left outer join  ViewEmployeeInformation vws on EarnLeaveEncashmentStatement.EmployeeId=vws.EmployeeId
where 1=1 
";

                if (fid != 0)
                {
                    sqlText += @" AND Year='" + fid + "'";
                }
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                    sqlText += @" and ViewEmployeeInformation.ProjectId='" + ProjectId + "'";
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                    sqlText += @" and ViewEmployeeInformation.DepartmentId='" + DepartmentId + "'";
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                    sqlText += @" and ViewEmployeeInformation.SectionId='" + SectionId + "'";
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                    sqlText += @" and ViewEmployeeInformation.DesignationId='" + DesignationId + "'";
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    sqlText += @" and ViewEmployeeInformation.Code >='" + CodeF + "'";
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    sqlText += @" and ViewEmployeeInformation.Code<='" + CodeT + "'";
                #region Regular

                sqlText += @"
union all
select vws.Code EmpCode, vws.EmpName ,'" + fid + "'";
                sqlText += @"[Year]  ,0 EncashmentBalance
from ViewEmployeeInformation vws

where 1=1 AND vws.IsActive=1 AND vws.IsArchive=0 
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
                sqlText += @" and EmployeeId not in(select EmployeeId from EarnLeaveEncashmentStatement where 1=1 ";

                if (fid != 0)
                {
                    sqlText += @" AND Year='" + fid + "'";
                }
                sqlText += @" )";

                #endregion


                sqlText += @" ) as a ";


                //sqlText += " order by Department,EmpCode";
                //sqlText += " ORDER BY vw.FiscalYearDetailId";

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


                da.Fill(dt);



                #endregion
                #endregion

                //var tt = WriteDataTableToExcel(dt, "DataSheet", Filepath + FileName);

                #region Value Round

                //string[] columnNames = { "EncashmentBalance" };

                //dt = Ordinary.DtValueRound(dt, columnNames);

                #endregion

                #region SuccessResult
                //if (tt == false)
                //{
                //    results[0] = "Fail";
                //    results[1] = "Data Download UnSuccessfully.";
                //}
                //{
                //    results[0] = "Success";
                //    results[1] = "Data Download Successfully.";
                //}
                //retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            catch (Exception ex)
            {
                results[4] = ex.Message.ToString(); //catch ex
                //return results;
                throw ex;
            }
            return dt;
        }

        #region Unused Methods

        public bool cvsExport(System.Data.DataTable dt, string saveAsLocation)
        {
            bool result = false;
            try
            {
                StringBuilder sb = new StringBuilder();

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dt.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText(saveAsLocation + @"test.csv", sb.ToString());
                result = true;
            }
            catch (Exception ex)
            {

                throw;
            }
            return result;
        }
        public bool WriteDataTableToExcel(System.Data.DataTable dataTable, string worksheetName, string saveAsLocation)
        {
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook excelworkBook;
            Microsoft.Office.Interop.Excel.Worksheet excelSheet;
            Microsoft.Office.Interop.Excel.Range excelCellrange;

            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();

                excel.Visible = false;
                excel.DisplayAlerts = false;

                excelworkBook = excel.Workbooks.Add(Type.Missing);

                excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;
                excelSheet.Name = worksheetName;

                int j = 1;
                //excelSheet.Cells[1, 1] = Condition[0];
                //excelSheet.Cells[2, 1] = Condition[1];

                int rowcount = j;
                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[1000, 1]];
                excelCellrange.NumberFormat = "@";

                foreach (DataRow datarow in dataTable.Rows)
                {
                    rowcount += 1;
                    for (int i = 1; i <= dataTable.Columns.Count; i++)
                    {
                        if (rowcount == j + 1)
                        {
                            excelSheet.Cells[j, i] = dataTable.Columns[i - 1].ColumnName;
                        }
                        excelSheet.Cells[rowcount, i] = datarow[i - 1].ToString();
                    }

                }

                excelCellrange = excelSheet.Range[excelSheet.Cells[j, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                excelCellrange.EntireColumn.AutoFit();
                Microsoft.Office.Interop.Excel.Borders border = excelCellrange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;

                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[1, dataTable.Columns.Count]];
                excelCellrange.Font.Bold = true;






                //excelSheet.Cells[rowcount + 1, 8] = "Total";
                //excelSheet.Cells[rowcount + 1, 9] = InvAmount;
                //excelSheet.Cells[rowcount + 1, 11] = TaxAmount;
                //excelSheet.Cells[rowcount + 1, 12] = TaxPaid;
                //excelSheet.Cells[rowcount + 1, 13] = Balance;

                //excelCellrange = excelSheet.Range[excelSheet.Cells[j + 2, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                //excelCellrange.EntireColumn.AutoFit();
                //Microsoft.Office.Interop.Excel.Borders border = excelCellrange.Borders;
                //border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                //border.Weight = 2d;


                //excelCellrange = excel.get_Range(excelSheet.Cells[rowcount + 1, 8], excelSheet.Cells[rowcount + 1, 13]);
                //excelCellrange.Font.Bold = true;
                //excelCellrange.NumberFormat = "#,##0_);(#,##0)";

                //excelCellrange = excel.get_Range(excelSheet.Cells[6, 1], excelSheet.Cells[6, 13]);
                //excelCellrange.Font.Bold = true;
                //excelCellrange.NumberFormat = "#,##0_);(#,##0)";

                //excelCellrange = excel.get_Range(excelSheet.Cells[7, 7], excelSheet.Cells[rowcount + 1, 13]);
                //excelCellrange.NumberFormat = "#,##0_);(#,##0)";

                //excelCellrange = excel.get_Range(excelSheet.Cells[j + 3, 9], excelSheet.Cells[rowcount, dataTable.Columns.Count]);
                //excelCellrange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);






                excelworkBook.SaveAs(saveAsLocation);
                excelworkBook.Close();
                excel.Quit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                excelSheet = null;
                excelCellrange = null;
                excelworkBook = null;
            }

        }
        object missing = Type.Missing;
        public bool WriteDataTableToExcelUpdated(System.Data.DataTable dataTable, string saveAsLocation)
        {
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook excelworkBook;
            Microsoft.Office.Interop.Excel.Worksheet excelSheet;
            Microsoft.Office.Interop.Excel.Range excelCellrange;
            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();

                excel.Visible = false;
                excel.DisplayAlerts = false;

                excelworkBook = excel.Workbooks.Add(Type.Missing);

                excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;
                excelSheet.Name = "DataSheet";

                int j = 1;

                int rowcount = j + 2;
                int sl = 1;

                int row = 0;
                foreach (DataRow datarow in dataTable.Rows)
                {
                    row++;
                    datarow["SL"] = sl++;
                    rowcount += 1;
                    for (int i = 1; i <= dataTable.Columns.Count; i++)
                    {
                        if (rowcount == j + 3)
                        {
                            excelSheet.Cells[j + 2, i] = dataTable.Columns[i - 1].ColumnName;
                        }
                        excelSheet.Cells[rowcount, i] = datarow[i - 1].ToString();
                    }
                }

                excelCellrange = excelSheet.Range[excelSheet.Cells[j + 2, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                excelCellrange.EntireColumn.AutoFit();
                Microsoft.Office.Interop.Excel.Borders border = excelCellrange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;
                excelworkBook.SaveAs(saveAsLocation);
                excelworkBook.Close();
                excel.Quit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                excelSheet = null;
                excelCellrange = null;
                excelworkBook = null;
            }

        }
        #endregion
    }
}
