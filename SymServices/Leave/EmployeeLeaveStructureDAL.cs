using SymOrdinary;
using SymServices.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Leave;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Leave
{
    public class EmployeeLeaveStructureDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeLeaveStructureVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveStructureVM> employeeLeaveStructures = new List<EmployeeLeaveStructureVM>();
            EmployeeLeaveStructureVM employeeLeaveStructure;
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
Id
,EmployeeId
,LeaveStructureId
,LeaveYear
,LeaveType_E
,LeaveDays
,IsEarned
,IsCompensation
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    FROM EmployeeLeaveStructure
Where IsArchive=0
    ORDER BY LeaveType_E
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeLeaveStructure = new EmployeeLeaveStructureVM();
                    employeeLeaveStructure.Id = Convert.ToInt32(dr["Id"]);
                    employeeLeaveStructure.EmployeeId = dr["EmployeeId"].ToString();

                    employeeLeaveStructure.LeaveStructureId = Convert.ToInt32(dr["LeaveStructureId"]);
                    employeeLeaveStructure.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    employeeLeaveStructure.LeaveType_E = dr["LeaveType_E"].ToString();
                    employeeLeaveStructure.LeaveDays = Convert.ToInt32(dr["LeaveDays"]);
                    employeeLeaveStructure.IsEarned = Convert.ToBoolean(dr["IsEarned"]);
                    employeeLeaveStructure.IsCompensation = Convert.ToBoolean(dr["IsCompensation"]);

                    employeeLeaveStructure.Remarks = dr["Remarks"].ToString();
                    employeeLeaveStructure.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeLeaveStructure.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeLeaveStructure.CreatedBy = dr["CreatedBy"].ToString();
                    employeeLeaveStructure.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeLeaveStructure.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeLeaveStructure.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeLeaveStructure.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeLeaveStructures.Add(employeeLeaveStructure);
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

            return employeeLeaveStructures;
        }
        public List<EmployeeLeaveStructureVM> DropDown(string EmployeeId, int year)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveStructureVM> VMs = new List<EmployeeLeaveStructureVM>();
            EmployeeLeaveStructureVM vm;
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
Id,
LEAVETYPE_E
   FROM EmployeeLeaveStructure
WHERE EmployeeId=@EmployeeId and LeaveYear=@LeaveYear
    ORDER BY LEAVETYPE_E
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                _objComm.Parameters.AddWithValue("@LeaveYear", year);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeaveStructureVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.LeaveType_E = dr["LEAVETYPE_E"].ToString();
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
        public List<EmployeeLeaveStructureVM> DropDown(int year)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveStructureVM> VMs = new List<EmployeeLeaveStructureVM>();
            EmployeeLeaveStructureVM vm;
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
Id,
LEAVETYPE_E
   FROM EmployeeLeaveStructure
WHERE LeaveYear=@LeaveYear
    ORDER BY LEAVETYPE_E
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@LeaveYear", year);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeaveStructureVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.LeaveType_E = dr["LEAVETYPE_E"].ToString();
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
        public string CheckEmployeeLeaveBalance(string employeeId, string leaveType, int year, decimal totalDay, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            decimal available = 0;
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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction


                EmployeeInfoDAL _dal = new EmployeeInfoDAL();
                ////string EmployeeId = _dal.SelectEmpForSearch(employeeId, "current", currConn, transaction).Id;
                #region sql statement

                sqlText = @"
SELECT ISNULL(ISNULL(ES.LEAVEDAYS,0)+ISNULL(ES.OpeningLeaveDays,0)-ISNULL(EL.LEAVE,0),0) HAVE
FROM EMPLOYEELEAVESTRUCTURE ES
LEFT OUTER JOIN (
SELECT EMPLOYEELEAVESTRUCTUREID,SUM(TOTALLEAVE) LEAVE FROM EMPLOYEELEAVE  where IsApprove=1 GROUP BY EMPLOYEELEAVESTRUCTUREID
) EL ON EL.EMPLOYEELEAVESTRUCTUREID=ES.ID
where es.employeeId=@employeeId and es.leaveyear=@leaveyear and es.LeaveType_E=@LeaveType_E
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Transaction = transaction;
                _objComm.Parameters.AddWithValue("@employeeId", employeeId);
                _objComm.Parameters.AddWithValue("@leaveyear", year);
                _objComm.Parameters.AddWithValue("@LeaveType_E", leaveType);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    available = Convert.ToDecimal(dr["HAVE"]);
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
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
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
            if (available < totalDay)
            {
                return "You have no sufficient balance";
            }
            else
            {
                return "T";
            }
        }
        //==================SelectAll=================
        public List<EmployeeLeaveStructureVM> SelectAllByEmployee(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveStructureVM> employeeLeaveStructures = new List<EmployeeLeaveStructureVM>();
            EmployeeLeaveStructureVM employeeLeaveStructure;
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
Id
,EmployeeId
,LeaveStructureId
,LeaveYear
,LeaveType_E
,LeaveDays
,IsEarned
,IsCompensation
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    FROM EmployeeLeaveStructure
Where IsArchive=0 and EmployeeId=@EmployeeId
    ORDER BY LeaveType_E
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeLeaveStructure = new EmployeeLeaveStructureVM();
                    employeeLeaveStructure.Id = Convert.ToInt32(dr["Id"]);
                    employeeLeaveStructure.EmployeeId = dr["EmployeeId"].ToString();

                    employeeLeaveStructure.LeaveStructureId = Convert.ToInt32(dr["LeaveStructureId"]);
                    employeeLeaveStructure.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    employeeLeaveStructure.LeaveType_E = dr["LeaveType_E"].ToString();
                    employeeLeaveStructure.LeaveDays = Convert.ToInt32(dr["LeaveDays"]);
                    employeeLeaveStructure.IsEarned = Convert.ToBoolean(dr["IsEarned"]);
                    employeeLeaveStructure.IsCompensation = Convert.ToBoolean(dr["IsCompensation"]);

                    employeeLeaveStructure.Remarks = dr["Remarks"].ToString();
                    employeeLeaveStructure.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeLeaveStructure.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeLeaveStructure.CreatedBy = dr["CreatedBy"].ToString();
                    employeeLeaveStructure.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeLeaveStructure.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeLeaveStructure.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeLeaveStructure.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeLeaveStructures.Add(employeeLeaveStructure);
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

            return employeeLeaveStructures;
        }
        //==================SelectByID=================
        public EmployeeLeaveStructureVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeLeaveStructureVM employeeLeaveStructureVM = new EmployeeLeaveStructureVM();

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
Id
,EmployeeId
,LeaveStructureId
,LeaveYear
,LeaveType_E
,LeaveDays
,IsEarned
,IsCompensation
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
FROM EmployeeLeaveStructure
where  id=@Id
     
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
                    employeeLeaveStructureVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeLeaveStructureVM.EmployeeId = dr["EmployeeId"].ToString();

                    employeeLeaveStructureVM.LeaveStructureId = Convert.ToInt32(dr["LeaveStructureId"]);
                    employeeLeaveStructureVM.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    employeeLeaveStructureVM.LeaveType_E = dr["LeaveType_E"].ToString();
                    employeeLeaveStructureVM.LeaveDays = Convert.ToInt32(dr["LeaveDays"]);
                    employeeLeaveStructureVM.IsEarned = Convert.ToBoolean(dr["IsEarned"]);
                    employeeLeaveStructureVM.IsCompensation = Convert.ToBoolean(dr["IsCompensation"]);

                    employeeLeaveStructureVM.Remarks = dr["Remarks"].ToString();
                    employeeLeaveStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeLeaveStructureVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeLeaveStructureVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeLeaveStructureVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeLeaveStructureVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeLeaveStructureVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeLeaveStructureVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeeLeaveStructureVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeeLeaveStructureVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeLeaveStructure"; //Method Name


            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;


            #region Try

            try
            {

                //#region Validation
                //if (string.IsNullOrEmpty(empLeaveStructureVM.Degree_E))
                //{
                //    retResults[1] = "Please Input Employee Degree_E";
                //    return retResults;
                //}
                //#endregion Validation

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


                #region Save

                List<LeaveStructureDetailVM> leaveStructureDetailVMs = new List<LeaveStructureDetailVM>();
                LeaveStructureDetailVM leaveStructureDetailVM;


                //sqlText = "select LeaveStructureId from EmployeeStructureGroup where Id=@Id";
                //SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                //cmd1.Transaction = transaction;
                //cmd1.Parameters.AddWithValue("@Id", empLeaveStructureVM.leaveStructureId);
                //SqlDataReader dr;
                //dr = cmd1.ExecuteReader();
                //while (dr.Read())
                //{
                //    empLeaveStructureVM.LeaveStructureId = Convert.ToInt32(dr["LeaveStructureId"]);
                //}
                //dr.Close();
                //if (empLeaveStructureVM.LeaveStructureId <= 0)
                //{
                //    throw new ArgumentNullException("Leave structure not founded!", "");
                //}

                sqlText = @"select LeaveStructureId,LeaveType_E,LeaveDays,IsEarned,IsCompensation
,ISNULL(IsCarryForward, 0) IsCarryForward
,ISNULL(MaxBalance, 0) MaxBalance
,Remarks 
from LeaveStructureDetail where IsArchive=0 and LeaveStructureId=@LeaveStructureId";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                cmd2.Parameters.AddWithValue("@LeaveStructureId", vm.LeaveStructureId);
                SqlDataReader dr;
                dr = cmd2.ExecuteReader();
                while (dr.Read())
                {
                    leaveStructureDetailVM = new LeaveStructureDetailVM();
                    leaveStructureDetailVM.LeaveStructureId = Convert.ToInt32(dr["LeaveStructureId"]);
                    leaveStructureDetailVM.LeaveType_E = dr["LeaveType_E"].ToString();
                    leaveStructureDetailVM.LeaveDays = dr["LeaveDays"].ToString();
                    leaveStructureDetailVM.Remarks = dr["Remarks"].ToString();
                    leaveStructureDetailVM.IsEarned = Convert.ToBoolean(dr["IsEarned"]);
                    leaveStructureDetailVM.IsCompensation = Convert.ToBoolean(dr["IsCompensation"]);
                    leaveStructureDetailVM.IsCarryForward = Convert.ToBoolean(dr["IsCarryForward"]);
                    leaveStructureDetailVM.MaxBalance = Convert.ToInt32(dr["MaxBalance"]);
                    leaveStructureDetailVMs.Add(leaveStructureDetailVM);
                }
                dr.Close();

                for (int i = 0; i < vm.employeeIds.Length - 1; i++)
                {
                    sqlText = "delete EmployeeLeaveStructure where EmployeeId=@EmployeeId and LeaveYear=@LeaveYear";
                    SqlCommand cmd0 = new SqlCommand(sqlText, currConn);
                    cmd0.Transaction = transaction;
                    cmd0.Parameters.AddWithValue("@EmployeeId", vm.employeeIds[i]);
                    cmd0.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                    cmd0.ExecuteNonQuery();

                    foreach (LeaveStructureDetailVM item in leaveStructureDetailVMs)
                    {
                        sqlText = "  ";
                        sqlText += @" INSERT INTO EmployeeLeaveStructure(	
EmployeeId,LeaveStructureId,LeaveYear,LeaveType_E,LeaveDays,IsEarned,IsCompensation
,IsCarryForward,MaxBalance,OpeningLeaveDays
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)
                    VALUES (
@EmployeeId,@LeaveStructureId,@LeaveYear,@LeaveType_E,@LeaveDays,@IsEarned,@IsCompensation
,@IsCarryForward,@MaxBalance,@OpeningLeaveDays
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom)
                    SELECT SCOPE_IDENTITY()";

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.employeeIds[i]);

                        cmdInsert.Parameters.AddWithValue("@LeaveStructureId", item.LeaveStructureId);
                        cmdInsert.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                        cmdInsert.Parameters.AddWithValue("@LeaveType_E", item.LeaveType_E);
                        cmdInsert.Parameters.AddWithValue("@LeaveDays", item.LeaveDays);
                        cmdInsert.Parameters.AddWithValue("@IsEarned", item.IsEarned);
                        cmdInsert.Parameters.AddWithValue("@IsCompensation", item.IsCompensation);
                        cmdInsert.Parameters.AddWithValue("@IsCarryForward", item.IsCarryForward);
                        cmdInsert.Parameters.AddWithValue("@MaxBalance", item.MaxBalance);
                        cmdInsert.Parameters.AddWithValue("@OpeningLeaveDays", 0);

                        cmdInsert.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                        cmdInsert.Transaction = transaction;
                        var exeRes = cmdInsert.ExecuteScalar();
                        Id = Convert.ToInt32(exeRes);
                        if (Id <= 0)
                        {
                            retResults[1] = "Please Input Employee LeaveStructure Value";
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Please Input Employee LeaveStructure Value", "");
                        }
                    }
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
                retResults[2] = Id.ToString();

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
        public string[] Update(EmployeeLeaveStructureVM empLeaveStructureVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "LeaveStructureUpdate"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToLeaveStructure"); }

                #endregion open connection and transaction
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeLeaveStructure ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Degree_E=@Degree_E AND Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", LeaveStructureVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", LeaveStructureVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Degree_E", LeaveStructureVM.Degree_E);

                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This Degree already used";
                //    throw new ArgumentNullException("Please Input LeaveStructure Value", "");
                //}
                //#endregion Exist

                if (empLeaveStructureVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeLeaveStructure set";
                    sqlText += " EmployeeId=@EmployeeId,";

                    sqlText += " LeaveStructureId=@LeaveStructureId,";
                    sqlText += " LeaveYear=@LeaveYear,";
                    sqlText += " LeaveType_E=@LeaveType_E,";
                    sqlText += " LeaveDays=@LeaveDays,";
                    sqlText += " IsEarned=@IsEarned,";
                    sqlText += " IsCompensation=@IsCompensation,";


                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", empLeaveStructureVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", empLeaveStructureVM.EmployeeId);

                    cmdUpdate.Parameters.AddWithValue("@LeaveStructureId", empLeaveStructureVM.LeaveStructureId);
                    cmdUpdate.Parameters.AddWithValue("@LeaveYear", empLeaveStructureVM.LeaveYear);
                    cmdUpdate.Parameters.AddWithValue("@LeaveType_E", empLeaveStructureVM.LeaveType_E);
                    cmdUpdate.Parameters.AddWithValue("@LeaveDays", empLeaveStructureVM.LeaveDays);
                    cmdUpdate.Parameters.AddWithValue("@IsEarned", empLeaveStructureVM.IsEarned);
                    cmdUpdate.Parameters.AddWithValue("@IsCompensation", empLeaveStructureVM.IsCompensation);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", empLeaveStructureVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", false);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", empLeaveStructureVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", empLeaveStructureVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", empLeaveStructureVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = empLeaveStructureVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    //#region Commit

                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException("LeaveStructure Update", empLeaveStructureVM.Degree_E + " could not updated.");
                    //}

                    //#endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("LeaveStructure Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update LeaveStructure.";
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
        //==================Select =================
        public EmployeeLeaveStructureVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeeLeaveStructureVM employeeLeaveStructureVM = new EmployeeLeaveStructureVM();

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
                sqlText = @"SELECT Top 1 
Id
,EmployeeId
,LeaveStructureId
,LeaveYear
,LeaveType_E
,LeaveDays
,IsEarned
,IsCompensation
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
FROM EmployeeLeaveStructure    
";
                if (query == null)
                {
                    if (Id != 0)
                    {
                        sqlText += " AND Id=@Id";
                    }
                    else
                    {
                        sqlText += " ORDER BY Id ";
                    }
                }
                else
                {
                    if (query == "FIRST")
                    {
                        sqlText += " ORDER BY Id ";
                    }
                    else if (query == "LAST")
                    {
                        sqlText += " ORDER BY Id DESC";
                    }
                    else if (query == "NEXT")
                    {
                        sqlText += " and  Id > @Id   ORDER BY Id";
                    }
                    else if (query == "PREVIOUS")
                    {
                        sqlText += "  and  Id < @Id   ORDER BY Id DESC";
                    }
                }


                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (Id != null)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                try
                {
                    dr = objComm.ExecuteReader();
                    while (dr.Read())
                    {
                        employeeLeaveStructureVM.Id = Convert.ToInt32(dr["Id"]);
                        employeeLeaveStructureVM.EmployeeId = dr["EmployeeId"].ToString();

                        employeeLeaveStructureVM.LeaveStructureId = Convert.ToInt32(dr["LeaveStructureId"]);
                        employeeLeaveStructureVM.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                        employeeLeaveStructureVM.LeaveType_E = dr["LeaveType_E"].ToString();
                        employeeLeaveStructureVM.LeaveDays = Convert.ToInt32(dr["LeaveDays"]);
                        employeeLeaveStructureVM.IsEarned = Convert.ToBoolean(dr["IsEarned"]);
                        employeeLeaveStructureVM.IsCompensation = Convert.ToBoolean(dr["IsCompensation"]);

                        employeeLeaveStructureVM.Remarks = dr["Remarks"].ToString();
                        employeeLeaveStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeeLeaveStructureVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeeLeaveStructureVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeeLeaveStructureVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeeLeaveStructureVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeeLeaveStructureVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeeLeaveStructureVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    }
                    dr.Close();
                }
                catch (Exception ex)
                {
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

            return employeeLeaveStructureVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeeLeaveStructureVM employeeLeaveStructureVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "LeaveStructureDelete"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToLeaveStructure"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (employeeLeaveStructureVM.Id > 0)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeLeaveStructure set";
                    sqlText += " IsArchive=@IsArchive";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", employeeLeaveStructureVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeLeaveStructureVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeLeaveStructureVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeLeaveStructureVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = employeeLeaveStructureVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("LeaveStructure Delete", employeeLeaveStructureVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("LeaveStructure Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete LeaveStructure.";
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


        //==================Insert =================
        public string[] LeaveMigrationInsert(EmployeeLeaveStructureVM vm, ShampanIdentityVM siVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "LeaveMigrationInsert"; //Method Name

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
                #region Save
                EmployeeStructureGroupDAL _EmployeeStructureGroupDAL = new EmployeeStructureGroupDAL();

                for (int i = 0; i < vm.employeeIds.Length - 1; i++)
                {
                    vm.LeaveStructureId = Convert.ToInt32(_EmployeeStructureGroupDAL.SelectByEmployee(vm.employeeIds[i],currConn, transaction).LeaveStructureId);

                    retResults = _EmployeeStructureGroupDAL.EmployeeLeaveStructure(vm.employeeIds[i]
                        , vm.LeaveStructureId.ToString(), vm.LeaveYear.ToString()
                        , siVM, currConn, transaction);

                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
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
                retResults[2] = Id.ToString();

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


        #endregion
    }
}
