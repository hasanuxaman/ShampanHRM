using SymOrdinary;
using SymServices.Common;
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
    public class EmployeeLeaveDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeLeaveDetailVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveDetailVM> VMs = new List<EmployeeLeaveDetailVM>();
            EmployeeLeaveDetailVM vm;
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

,EmployeeLeaveStructureId
,LeaveYear
,LeaveType_E
,TotalLeave
,LeaveDate
,ApprovedBy
,IsApprove
,IsHalfDay
,ISNULL(IsLWP, 0) IsLWP
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    FROM EmployeeLeaveDetail
Where IsArchive=0
    ORDER BY LeaveDetailType_E
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeaveDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.LeaveDate =  Ordinary.StringToDate(dr["LeaveDate"].ToString());
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    vm.IsLWP = Convert.ToBoolean(dr["IsLWP"]);
                    
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        //==================SelectAll=================
        public List<EmployeeLeaveDetailVM> SelectAllByEmployee(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveDetailVM> VMs = new List<EmployeeLeaveDetailVM>();
            EmployeeLeaveDetailVM vm;
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

,EmployeeLeaveStructureId
,LeaveYear
,LeaveType_E
,TotalLeave
,LeaveDate
,ApprovedBy
,IsApprove
,IsHalfDay
,ISNULL(IsLWP, 0) IsLWP
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    FROM EmployeeLeaveDetail
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
                    vm = new EmployeeLeaveDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();

                    vm.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.LeaveDate = Ordinary.StringToDate(dr["LeaveDate"].ToString());
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    vm.IsLWP = Convert.ToBoolean(dr["IsLWP"]);

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        //==================SelectByID=================
        public EmployeeLeaveDetailVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeLeaveDetailVM vm = new EmployeeLeaveDetailVM();

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

,EmployeeLeaveStructureId
,LeaveYear
,LeaveType_E
,TotalLeave
,LeaveDate
,ApprovedBy
,IsApprove
,IsHalfDay
,ISNULL(IsLWP,0) IsLWP
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
FROM EmployeeLeaveDetail
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
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();

                    vm.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                    vm.LeaveDate =  Ordinary.StringToDate(dr["LeaveDate"].ToString());
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);
                    vm.IsLWP = Convert.ToBoolean(dr["IsLWP"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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
        //==================Insert =================
        public string[] Insert(EmployeeLeaveDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeLeaveDetail"; //Method Name


            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
                        
            #region Try

            try
            {

                //#region Validation
                //if (string.IsNullOrEmpty(empLeaveDetailVM.Degree_E))
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

                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeLeaveDetail ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Degree_E=@Degree_E";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", empLeaveDetailVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Degree_E", empLeaveDetailVM.Degree_E);
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee LeaveDetail Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee LeaveDetail Value", "");
                //}
                //#endregion Exist
                #region Save

                //int foundId = (int)objfoundId;
                if (vm != null)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeLeaveDetail( EmployeeId
,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,TotalLeave,LeaveDate,ApprovedBy,IsApprove,IsHalfDay
,IsLWP
,Remarks,EligibleReviewDate,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)
                    VALUES ( @EmployeeId,
,@EmployeeLeaveStructureId,@LeaveYear,@LeaveType_E,@TotalLeave,@LeaveDate,@ApprovedBy,@IsApprove,@IsHalfDay
,@IsLWP
,@Remarks,@EligibleReviewDate,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom)
                    SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                    cmdInsert.Parameters.AddWithValue("@EmployeeLeaveStructureId", vm.EmployeeLeaveStructureId);
                    cmdInsert.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                    cmdInsert.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E);
                    cmdInsert.Parameters.AddWithValue("@TotalLeave", vm.TotalLeave);
                    cmdInsert.Parameters.AddWithValue("@LeaveDate", Ordinary.DateToString(vm.LeaveDate));
                    cmdInsert.Parameters.AddWithValue("@ApprovedBy", vm.ApprovedBy);
                    cmdInsert.Parameters.AddWithValue("@IsApprove", vm.IsApprove);
                    cmdInsert.Parameters.AddWithValue("@IsHalfDay", vm.IsHalfDay);
                    cmdInsert.Parameters.AddWithValue("@IsLWP", vm.IsLWP);


                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdInsert.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee LeaveDetail Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee LeaveDetail Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee LeaveDetail already used";
                    throw new ArgumentNullException("Please Input Employee LeaveDetail Value", "");
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
        public string[] Update(EmployeeLeaveDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "LeaveDetailUpdate"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToLeaveDetail"); }

                #endregion open connection and transaction
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeLeaveDetail ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Degree_E=@Degree_E AND Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", LeaveDetailVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", LeaveDetailVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Degree_E", LeaveDetailVM.Degree_E);

                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This Degree already used";
                //    throw new ArgumentNullException("Please Input LeaveDetail Value", "");
                //}
                //#endregion Exist

                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "UPDATE EmployeeLeaveDetail set";
                    sqlText += " EmployeeId=@EmployeeId,";

                    sqlText += " EmployeeLeaveStructureId=@EmployeeLeaveStructureId,";
                    sqlText += " LeaveYear=@LeaveYear,";
                    sqlText += " LeaveType_E=@LeaveType_E,";
                    sqlText += " TotalLeave=@TotalLeave,";
                    sqlText += " LeaveDate=@LeaveDate,";
                    sqlText += " ApprovedBy=@ApprovedBy,";
                    sqlText += " IsApprove=@IsApprove,";
                    sqlText += " IsHalfDay=@IsHalfDay,";
                    sqlText += " IsLWP=@IsLWP,";
                    
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                    cmdUpdate.Parameters.AddWithValue("@EmployeeLeaveStructureId", vm.EmployeeLeaveStructureId);
                    cmdUpdate.Parameters.AddWithValue("@LeaveYear", vm.LeaveYear);
                    cmdUpdate.Parameters.AddWithValue("@LeaveType_E", vm.LeaveType_E);
                    cmdUpdate.Parameters.AddWithValue("@TotalLeave", vm.TotalLeave);
                    cmdUpdate.Parameters.AddWithValue("@LeaveDate", Ordinary.DateToString(vm.LeaveDate));
                    cmdUpdate.Parameters.AddWithValue("@ApprovedBy", vm.ApprovedBy);
                    cmdUpdate.Parameters.AddWithValue("@IsApprove", vm.IsApprove);
                    cmdUpdate.Parameters.AddWithValue("@IsHalfDay", vm.IsHalfDay);
                    cmdUpdate.Parameters.AddWithValue("@IsLWP", vm.IsLWP);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", false);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    //#region Commit

                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException("LeaveDetail Update", empLeaveDetailVM.Degree_E + " could not updated.");
                    //}

                    //#endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("LeaveDetail Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update LeaveDetail.";
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
        public EmployeeLeaveDetailVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeeLeaveDetailVM employeeLeaveDetailVM = new EmployeeLeaveDetailVM();

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
,EmployeeLeaveDetailStructureId
,LeaveDetailYear
,LeaveDetailType_E
,FromDate
,ToDate
,TotalLeaveDetail
,ApprovedBy
,IsApprove
,IsHalfDay
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
FROM EmployeeLeaveDetail    
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
                        employeeLeaveDetailVM.Id = Convert.ToInt32(dr["Id"]);
                        employeeLeaveDetailVM.EmployeeId = dr["EmployeeId"].ToString();

                        employeeLeaveDetailVM.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"]);
                        employeeLeaveDetailVM.LeaveYear = Convert.ToInt32(dr["LeaveYear"]);
                        employeeLeaveDetailVM.LeaveType_E = dr["LeaveType_E"].ToString();
                        employeeLeaveDetailVM.TotalLeave = Convert.ToDecimal(dr["TotalLeave"]);
                        employeeLeaveDetailVM.LeaveDate = dr["LeaveDate"].ToString();
                        employeeLeaveDetailVM.ApprovedBy = dr["ApprovedBy"].ToString();
                        employeeLeaveDetailVM.IsApprove = Convert.ToBoolean(dr["IsApprove"]);
                        employeeLeaveDetailVM.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"]);

                        employeeLeaveDetailVM.Remarks = dr["Remarks"].ToString();
                        employeeLeaveDetailVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeeLeaveDetailVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeeLeaveDetailVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeeLeaveDetailVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeeLeaveDetailVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeeLeaveDetailVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeeLeaveDetailVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return employeeLeaveDetailVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeeLeaveDetailVM employeeLeaveDetailVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "LeaveDetailDelete"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToLeaveDetail"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (employeeLeaveDetailVM.Id > 0)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeLeaveDetail set";
                    sqlText += " IsArchive=@IsArchive";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", employeeLeaveDetailVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeLeaveDetailVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeLeaveDetailVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeLeaveDetailVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = employeeLeaveDetailVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("LeaveDetail Delete", employeeLeaveDetailVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("LeaveDetail Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete LeaveDetail.";
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
        #endregion
        #region New Methods
        public decimal SelectLeaveDays(string EmployeeId, string fid, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable dt = new DataTable();
            string sqlText = "";
            decimal totalLeave = 0;
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
                #region Data Fetching
                DataTable fPeriodDt = new DataTable();
                string periodStart = "";
                string periodEnd = "";
                fPeriodDt = _cDal.SelectByCondition("FiscalYearDetail", "Id", fid, currConn, transaction);
                if (fPeriodDt == null || fPeriodDt.Rows.Count == 0)
                {
                    fPeriodDt = null;
                    return 0;
                }
                periodStart = fPeriodDt.Rows[0]["PeriodStart"].ToString();
                periodEnd = fPeriodDt.Rows[0]["PeriodEnd"].ToString();
                #endregion Data Fetching

                #region sql statement

                sqlText = @"
SELECT
IsNull(sum(TotalLeave),0) TotalLeave
From EmployeeLeaveDetail
Where 1=1
AND IsLWP = 0
AND EmployeeId = @EmployeeId
AND LeaveDate Between @PeriodStart AND @PeriodEnd
";
                //"Leave without pay";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@EmployeeId",EmployeeId);
                da.SelectCommand.Parameters.AddWithValue("@PeriodStart",periodStart);
                da.SelectCommand.Parameters.AddWithValue("@PeriodEnd",periodEnd);
                da.Fill(dt);
                totalLeave = Convert.ToInt32(dt.Rows[0]["TotalLeave"]);
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

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

            return totalLeave;
        }
        
        public int SelectLeaveWPDays(string EmployeeId, string fid, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable dt = new DataTable();
            string sqlText = "";
            int leaveWPDays = 0;
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
                #region Data Fetching
                DataTable fPeriodDt = new DataTable();
                string periodStart = "";
                string periodEnd = "";
                fPeriodDt = _cDal.SelectByCondition("FiscalYearDetail", "Id", fid, currConn, transaction);
                if (fPeriodDt == null || fPeriodDt.Rows.Count == 0)
                {
                    fPeriodDt = null;
                    return 0;
                }
                periodStart = fPeriodDt.Rows[0]["PeriodStart"].ToString();
                periodEnd = fPeriodDt.Rows[0]["PeriodEnd"].ToString();
                #endregion Data Fetching

                #region sql statement

                sqlText = @"
SELECT
IsNull(sum(TotalLeave),0) LWPDays
From EmployeeLeaveDetail
Where 1=1
AND IsLWP = 1
AND EmployeeId = @EmployeeId
AND LeaveDate Between @PeriodStart AND @PeriodEnd
";
                //"Leave without pay";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@EmployeeId",EmployeeId);
                da.SelectCommand.Parameters.AddWithValue("@PeriodStart",periodStart);
                da.SelectCommand.Parameters.AddWithValue("@PeriodEnd",periodEnd);
                da.Fill(dt);
                leaveWPDays = Convert.ToInt32(dt.Rows[0]["LWPDays"]);
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

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

            return leaveWPDays;
        }
        
        
        #endregion New Methods
    }
}

