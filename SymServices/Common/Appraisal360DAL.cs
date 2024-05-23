using SymOrdinary;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Common
{
    public class Appraisal360DAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods

        //==================SelectAll Appraisal360=================
        public List<Appraisal360VM> SelectAllAppraisal360()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<Appraisal360VM> appraisal360VMs = new List<Appraisal360VM>();
            Appraisal360VM appraisal360VM;
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
 a.Id
,ISNull (a.DepartmentId,'')DepartmentId
,ISNull (d.Name,'')DepartmentName
,ISNull  (a.UserId,'')UserId
,ISNull  (u.FullName,'')UserName
,ISNull  (a.FeedBackUserId,'')FeedBackUserId
,ISNull  (fu.FullName,'')FeedBackUserName
,ISNull  (a.FeedbackDate,'')FeedbackDate
,ISNull  (a.FeedBackYear,0)FeedBackYear
,ISNull  (a.FeedBackMonthId,0)FeedBackMonthId
,ISNull  (fyd.PeriodName,0)PeriodName
,ISNull  (a.IsFeedbackCompeted,0)IsFeedbackCompeted
,ISNull  (a.IsUser,0)IsUser
,ISNull  (a.IsSupervisor,0)IsSupervisor
,ISNull  (a.IsDepartmentHead,0)IsDepartmentHead
,ISNull  (a.IsManagement,0)IsManagement
,ISNull  (a.IsHR,0)IsHR
    FROM Appraisal360 AS a
	JOIN Department AS d ON a.DepartmentId =d.Id
	JOIN [USER] AS u ON a.UserId =u.Id
	JOIN [USER] AS fu ON a.FeedBackUserId =fu.Id
	JOIN FiscalYearDetail AS fyd ON a.FeedBackMonthId=fyd.Id
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    appraisal360VM = new Appraisal360VM();
                    appraisal360VM.Id = Convert.ToInt32(dr["Id"]);
                    appraisal360VM.DepartmentId = dr["DepartmentId"].ToString();
                    appraisal360VM.DepartmentName = dr["DepartmentName"].ToString();
                    appraisal360VM.UserId = dr["UserId"].ToString();
                    appraisal360VM.UserName = dr["UserName"].ToString();
                    appraisal360VM.FeedBackUserId = dr["FeedBackUserId"].ToString();
                    appraisal360VM.FeedBackUserName = dr["FeedBackUserName"].ToString();
                    //appraisal360VM.FeedbackDate = Convert.ToDateTime(dr["FeedbackDate"]) == null ? Convert.ToDateTime("2024-01-01") : Convert.ToDateTime(dr["FeedbackDate"]);
                    //appraisal360VM.FeedbackDate = dr["FeedbackDate"] == DBNull.Value ? DateTime.Parse("2024-01-01").Date : ((DateTime)dr["FeedbackDate"]).Date;
                    appraisal360VM.FeedbackDate = dr["FeedbackDate"] == DBNull.Value? DateTime.Parse("2024-01-01").Date  : ((DateTime)dr["FeedbackDate"]).Date; 
                    appraisal360VM.FeedBackYear = Convert.ToInt32(dr["FeedBackYear"]);
                    appraisal360VM.FeedBackMonthId = Convert.ToInt32(dr["FeedBackMonthId"]);
                    appraisal360VM.PeriodName = dr["PeriodName"].ToString();
                    appraisal360VM.IsFeedbackCompeted = Convert.ToBoolean(dr["IsFeedbackCompeted"]);
                    appraisal360VM.IsUser = dr["IsUser"] != DBNull.Value ? Convert.ToBoolean(dr["IsUser"]) : false;
                    appraisal360VM.IsSupervisor = dr["IsSupervisor"] != DBNull.Value ? Convert.ToBoolean(dr["IsSupervisor"]) : false;
                    appraisal360VM.IsDepartmentHead = dr["IsDepartmentHead"] != DBNull.Value ? Convert.ToBoolean(dr["IsDepartmentHead"]) : false;
                    appraisal360VM.IsManagement = dr["IsManagement"] != DBNull.Value ? Convert.ToBoolean(dr["IsManagement"]) : false;
                    appraisal360VM.IsHR = dr["IsHR"] != DBNull.Value ? Convert.ToBoolean(dr["IsHR"]) : false;

                    appraisal360VMs.Add(appraisal360VM);
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

            return appraisal360VMs;
        }

        //==================SelectAll ViewAppraisal360=================
        public List<ViewAppraisal360VM> SelectAll()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<ViewAppraisal360VM> viewAppraisal360VMVMs = new List<ViewAppraisal360VM>();
            ViewAppraisal360VM viewAppraisal360VM;
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
FeedBackYear
, PeriodName
, FeedbackBy
, DepartmentName
, UserCode
, UserName
, FeedbackCode
, FeedbackName
, IsFeedbackCompeted
, UserId
, FeedBackUserId
, FeedBackMonth
, Appraisal360Id
 FROM ViewAppraisal360
";


                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    viewAppraisal360VM = new ViewAppraisal360VM();
                    viewAppraisal360VM.FeedBackYear = Convert.ToInt32(dr["FeedBackYear"]);
                    viewAppraisal360VM.PeriodName = dr["PeriodName"].ToString();
                    viewAppraisal360VM.FeedbackBy = dr["FeedbackBy"].ToString();

                    viewAppraisal360VM.DepartmentName = dr["DepartmentId"].ToString();
                    viewAppraisal360VM.UserCode = dr["UserCode"].ToString();
                    viewAppraisal360VM.UserName = dr["UserName"].ToString();

                    viewAppraisal360VM.FeedbackCode = dr["FeedbackCode"].ToString();
                    viewAppraisal360VM.FeedbackName = dr["FeedbackName"].ToString();


                    viewAppraisal360VM.IsFeedbackCompeted = Convert.ToBoolean(dr["IsFeedbackCompeted"]);
                    viewAppraisal360VM.UserId = dr["UserId"].ToString();
                    viewAppraisal360VM.FeedBackUserId = dr["FeedBackUserId"].ToString();
                    viewAppraisal360VM.FeedBackMonth = Convert.ToInt32(dr["FeedBackMonth"]);
                    viewAppraisal360VM.Appraisal360Id = Convert.ToInt32(dr["Appraisal360Id"]);

                    viewAppraisal360VMVMs.Add(viewAppraisal360VM);
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

            return viewAppraisal360VMVMs;
        }

        //==================SelectByID=================
        public Appraisal360VM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            Appraisal360VM appraisal360VM = new Appraisal360VM();

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
Appraisal360.Id
,ISNULL (DepartmentId,'') DepartmentId
,ISNULL (UserId,'')UserId
,ISNULL (FeedBackUserId,'')FeedBackUserId
,ISNULL (FeedbackDate,'')FeedbackDate
,ISNULL (FeedBackYear,'')FeedBackYear
,ISNULL (FeedBackMonthId,'')FeedBackMonthId
,ISNULL (IsFeedbackCompeted,'')IsFeedbackCompeted
,ISNULL (IsUser,0)IsUser
,ISNULL (IsSupervisor,0)IsSupervisor
,ISNULL (IsDepartmentHead,0)IsDepartmentHead
,ISNULL (IsManagement,0)IsManagement
,ISNULL (IsHR,0)IsHR
,ISNULL (FYD.PeriodName,0)PeriodName

FROM Appraisal360
LEFT OUTER JOIN FiscalYearDetail FYD ON FYD.Id = Appraisal360.FeedBackMonthId

Where  Appraisal360.Id=@Id    
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
                    appraisal360VM = new Appraisal360VM();
                    appraisal360VM.Id = Convert.ToInt32(dr["Id"]);
                    appraisal360VM.DepartmentId = dr["DepartmentId"].ToString();
                    appraisal360VM.UserId = dr["UserId"].ToString();
                    appraisal360VM.FeedBackUserId = dr["FeedBackUserId"].ToString();
                    //appraisal360VM.FeedbackDate = Convert.ToDateTime(dr["FeedbackDate"]);
                    appraisal360VM.FeedbackDate = Convert.ToDateTime(dr["FeedbackDate"]).Date;

                    appraisal360VM.FeedBackYear = Convert.ToInt32(dr["FeedBackYear"]);
                    appraisal360VM.FeedBackMonthId = Convert.ToInt32(dr["FeedBackMonthId"]);
                    appraisal360VM.PeriodName = dr["PeriodName"].ToString();

                    appraisal360VM.IsFeedbackCompeted = Convert.ToBoolean(dr["IsFeedbackCompeted"]);

                    appraisal360VM.IsUser = Convert.ToBoolean(dr["IsUser"]);
                    appraisal360VM.IsSupervisor = Convert.ToBoolean(dr["IsSupervisor"]);
                    appraisal360VM.IsDepartmentHead = Convert.ToBoolean(dr["IsDepartmentHead"]);
                    appraisal360VM.IsManagement = Convert.ToBoolean(dr["IsManagement"]);
                    appraisal360VM.IsHR = Convert.ToBoolean(dr["IsHR"]);

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

            return appraisal360VM;
        }

        //==================Insert =================
        public string[] Insert(Appraisal360VM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertAppraisalQuestionsVM"; //Method Name
            int IDExist = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {
                #region Validation
                //if (string.IsNullOrEmpty(BranchVM.DepartmentId))
                //{
                //    retResults[1] = "Please Input Employee Travel Course";
                //    return retResults;
                //}
                #endregion Validation

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
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Appraisal360";
                //string[] fieldName = { "Question" };
                //string[] fieldValue = { vm.Question.Trim() };

                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                #endregion Exist
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM Branch ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", BranchVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", BranchVM.Name.Trim());
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Travel Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Travel Value", "");
                //}
                //#endregion Exist
                #region Save

                #region Find Transaction Exist

                //sqlText = "";
                //sqlText = sqlText + " SELECT COUNT(Id) FROM Question WHERE Question=@Question AND DepartmentId=@DepartmentId";
                //SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn, transaction);
                //cmdExistTran.Parameters.AddWithValueAndNullHandle("@DepartmentId", vm.DepartmentId);
                //cmdExistTran.Parameters.AddWithValueAndNullHandle("@Question", vm.Question.Trim());

                //IDExist = (int)cmdExistTran.ExecuteScalar();

                //if (IDExist > 0)
                //{
                //    throw new ArgumentNullException(MessageVM.msgExist, MessageVM.msgExist);
                //}

                #endregion Find Transaction Exist

                //int foundId = (int)objfoundId;
                if (1 == 1)
                {
                    sqlText = "";
                    sqlText += " INSERT INTO Appraisal360";
                    sqlText += "(";
                    sqlText += " DepartmentId";
                    sqlText += " ,UserId";
                    sqlText += " ,FeedBackUserId";
                    sqlText += " ,FeedbackDate";
                    sqlText += " ,FeedBackYear";
                    sqlText += " ,FeedBackMonthId";
                    sqlText += " ,IsFeedbackCompeted";

                    sqlText += " ,IsUser";
                    sqlText += " ,IsSupervisor";
                    sqlText += " ,IsDepartmentHead";
                    sqlText += " ,IsManagement";
                    sqlText += " ,IsHR";

                    sqlText += ")";

                    sqlText += " VALUES(";
                    sqlText += " @DepartmentId";
                    sqlText += " ,@UserId";
                    sqlText += " ,@FeedBackUserId";
                    sqlText += " ,@FeedbackDate";
                    sqlText += " ,@FeedBackYear";
                    sqlText += " ,@FeedBackMonthId";
                    sqlText += " ,@IsFeedbackCompeted";

                    sqlText += " ,@IsUser";
                    sqlText += " ,@IsSupervisor";
                    sqlText += " ,@IsDepartmentHead";
                    sqlText += " ,@IsManagement";
                    sqlText += " ,@IsHR";

                    sqlText += ")   SELECT SCOPE_IDENTITY() ";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId.Trim());
                    cmdInsert.Parameters.AddWithValue("@UserId", vm.UserId.Trim());
                    cmdInsert.Parameters.AddWithValue("@FeedBackUserId", vm.FeedBackUserId.Trim());
                    cmdInsert.Parameters.AddWithValue("@FeedbackDate", vm.FeedbackDate);
                    cmdInsert.Parameters.AddWithValue("@FeedBackYear", vm.FeedBackYear);
                    cmdInsert.Parameters.AddWithValue("@FeedBackMonthId", vm.FeedBackMonthId);
                    cmdInsert.Parameters.AddWithValue("@IsFeedbackCompeted", vm.IsFeedbackCompeted);

                    cmdInsert.Parameters.AddWithValue("@IsUser", vm.IsUser);
                    cmdInsert.Parameters.AddWithValue("@IsSupervisor", vm.IsSupervisor);
                    cmdInsert.Parameters.AddWithValue("@IsDepartmentHead", vm.IsDepartmentHead);
                    cmdInsert.Parameters.AddWithValue("@IsManagement", vm.IsManagement);
                    cmdInsert.Parameters.AddWithValue("@IsHR", vm.IsHR);

                    cmdInsert.Transaction = transaction;
                    var exeRes = cmdInsert.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Appraisal360 Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input AppraisalQuestions Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Appraisal360 already used";
                    throw new ArgumentNullException("Please Input Appraisal360 Value", "");
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
        public string[] Update(Appraisal360VM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Appraisal360 Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToAppraisal360VM"); }

                #endregion open connection and transaction

                #region Exist

                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Appraisal360";


                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                #endregion Exist
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM Branch ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", BranchVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", BranchVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", BranchVM.Name.Trim());

                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This Travel already used";
                //    throw new ArgumentNullException("Please Input Travel Value", "");
                //}
                //#endregion Exist

                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update Appraisal360 set";
                    sqlText += " DepartmentId=@DepartmentId,";
                    sqlText += " UserId=@UserId,";
                    sqlText += " FeedBackUserId=@FeedBackUserId,";
                    sqlText += " FeedbackDate=@FeedbackDate,";
                    sqlText += " FeedBackYear=@FeedBackYear,";
                    sqlText += " FeedBackMonthId=@FeedBackMonthId,";
                    sqlText += " IsFeedbackCompeted=@IsFeedbackCompeted,";

                    sqlText += " IsUser=@IsUser,";
                    sqlText += " IsSupervisor=@IsSupervisor,";

                    sqlText += " IsDepartmentHead=@IsDepartmentHead,";
                    sqlText += " IsManagement=@IsManagement,";
                    sqlText += " IsHR=@IsHR";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@UserId", vm.UserId);
                    cmdUpdate.Parameters.AddWithValue("@FeedBackUserId", vm.FeedBackUserId);
                    cmdUpdate.Parameters.AddWithValue("@FeedbackDate", vm.FeedbackDate);
                    cmdUpdate.Parameters.AddWithValue("@FeedBackYear", vm.FeedBackYear);
                    cmdUpdate.Parameters.AddWithValue("@FeedBackMonthId", vm.FeedBackMonthId);
                    cmdUpdate.Parameters.AddWithValue("@IsFeedbackCompeted", vm.IsFeedbackCompeted);

                    cmdUpdate.Parameters.AddWithValue("@IsUser", vm.IsUser);
                    cmdUpdate.Parameters.AddWithValue("@IsSupervisor", vm.IsSupervisor);
                    cmdUpdate.Parameters.AddWithValue("@IsDepartmentHead", vm.IsDepartmentHead);
                    cmdUpdate.Parameters.AddWithValue("@IsManagement", vm.IsManagement);
                    cmdUpdate.Parameters.AddWithValue("@IsHR", vm.IsHR);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", BranchVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("AppraisalQuestions Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update AppraisalQuestionsVM.";
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
        public Appraisal360VM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            Appraisal360VM appraisal360VM = new Appraisal360VM();

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
,DepartmentId
,UserId
,FeedBackUserId
,FeedbackDate
,FeedBackYear
,FeedBackMonthId
,IsFeedbackCompeted
,IsUser
,IsSupervisor
,IsDepartmentHead
,IsManagement
,IsHR
    From Appraisal360
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
                        appraisal360VM.Id = Convert.ToInt32(dr["Id"]);
                        appraisal360VM.DepartmentId = dr["DepartmentId"].ToString();
                        appraisal360VM.UserId = dr["UserId"].ToString();
                        appraisal360VM.FeedBackUserId = dr["FeedBackUserId"].ToString();
                        appraisal360VM.FeedbackDate = Convert.ToDateTime(dr["FeedbackDate"]);
                        appraisal360VM.FeedBackYear = Convert.ToInt32(dr["FeedBackYear"]);
                        appraisal360VM.FeedBackMonthId = Convert.ToInt32(dr["FeedBackMonthId"]);
                        appraisal360VM.IsFeedbackCompeted = Convert.ToBoolean(dr["IsFeedbackCompeted"]);



                        appraisal360VM.IsUser = Convert.ToBoolean(dr["IsUser"]);
                        appraisal360VM.IsSupervisor = Convert.ToBoolean(dr["IsSupervisor"]);
                        appraisal360VM.IsDepartmentHead = Convert.ToBoolean(dr["IsDepartmentHead"]);
                        appraisal360VM.IsManagement = Convert.ToBoolean(dr["IsManagement"]);
                        appraisal360VM.IsHR = Convert.ToBoolean(dr["IsHR"]);
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

            return appraisal360VM;
        }

        //==================Delete =================
        public string[] Delete(Appraisal360VM appraisalQuestionsVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteAppraisalQuestionsVM"; //Method Name

            int transResult = 0;
            int countId = 0;
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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToAppraisal360VM"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        //sqlText = "";
                        //sqlText = "update AppraisalQuestions set";

                        //sqlText += " IsDepartmentHead=@IsDepartmentHead,";
                        //sqlText += " IsManagement=@IsManagement,";
                        //sqlText += " IsHR=@IsHR";
                        //sqlText += " where Id=@Id";

                        //SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        //cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);

                        //cmdUpdate.Parameters.AddWithValue("@IsDepartmentHead", appraisalQuestionsVM.IsDepartmentHead);
                        //cmdUpdate.Parameters.AddWithValue("@IsManagement", appraisalQuestionsVM.IsManagement);
                        //cmdUpdate.Parameters.AddWithValue("@IsHR", appraisalQuestionsVM.IsHR);

                        sqlText = "";
                        sqlText = "update Appraisal360 set";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " IsArchive=@IsArchive";

                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);

                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Appraisal360 Delete", appraisalQuestionsVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Appraisal360 Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete AppraisalQuestions Information.";
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

        public List<Appraisal360VM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<Appraisal360VM> VMs = new List<Appraisal360VM>();
            Appraisal360VM vm;
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
Id
   FROM AppraisalQuestions
-- WHERE IsArchive=0 and IsActive=1
   -- ORDER BY Name
";
                SqlCommand _objComm = new SqlCommand(sqlText, currConn);
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new Appraisal360VM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    //vm.Question = dr["Question"].ToString();
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
        //==================Appraisal360DataProcess=================

        public bool Appraisal360DataProcess(string FiscalPeriodDetailId, string FYId, string DId)
        {


            #region Variables


            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Appraisal360 Process Successfully"; //Method Name

            SqlConnection currConn = null;
            string sqlText = "";
            //List<ViewAppraisal360VM> viewAppraisal360VMVMs = new List<ViewAppraisal360VM>();
            //ViewAppraisal360VM viewAppraisal360VM;
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

declare @UserId as varchar(100)
declare @FeedBackUserId as varchar(100)
declare @NoOfEmp as int
declare @NoOfDeptHead as int
declare @NoOfHR as int
declare @NoOfManagement as int
DECLARE @Counter INT = 1;
DECLARE @Counter1 INT = 1;
declare @FirstId as INT 
declare @LastId as INT 
DECLARE @UserType as varchar(10)

--select distinct UserId from ViewUsers where isactive=1 and  Department=@Dept and IsDepartmentHead=1
delete from Appraisal360Details where Appraisal360Id in(select Id from Appraisal360 where DepartmentId=@DepartmentId and FeedBackYear=@FeedBackYear and FeedBackMonthId=@FeedBackMonthId)
delete from Appraisal360 where DepartmentId=@DepartmentId and FeedBackYear=@FeedBackYear and FeedBackMonthId=@FeedBackMonthId
delete from AppraisalQuestionSetups where DepartmentId=@DepartmentId and FeedBackYear=@FeedBackYear and FeedBackMonthId=@FeedBackMonthId
delete from AppraisalFeedbackPersonSetups where DepartmentId=@DepartmentId and FeedBackYear=@FeedBackYear and FeedBackMonthId=@FeedBackMonthId

--select * from Appraisal360

create  table #TempUsers (Id int identity(1,1), UserId varchar(100))
create  table #TempUserWithFeedBack (id int identity(1,1), UserId varchar(100), FeedBackUserId varchar(100)
,IsUser bit,IsSupervisor bit,IsDepartmentHead bit,IsManagement bit, IsHR bit)
create  table #TempDept (Id int identity(1,1), UserId varchar(100))
create  table #TempHR (Id int identity(1,1), UserId varchar(100))
create  table #TempManagement (Id int identity(1,1), UserId varchar(100))

insert into #TempUsers(UserId)
select distinct UserId from ViewUsers where isactive=1 and  DepartmentId=@DepartmentId

insert into #TempDept(UserId)
select distinct UserId from ViewUsers where isactive=1 and   DepartmentId=@DepartmentId  and IsDepartmentHead=1

insert into #TempHR(UserId)
select distinct UserId from ViewUsers where isactive=1 and   IsHR=1

insert into #TempManagement(UserId)
select distinct UserId from ViewUsers where isactive=1 and   IsManagement=1

select @NoOfEmp=count(id) from #TempUsers  
select @NoOfDeptHead=count(id) from #TempDept  
select @NoOfHR=count(id) from #TempHR  
select @NoOfManagement=count(id) from #TempManagement  

SET @Counter  = 1;
WHILE @Counter <= @NoOfEmp
BEGIN
 select @UserId=UserId from #TempUsers where id=@Counter
					SET @Counter1  = 1;
				   WHILE @Counter1 <= @NoOfEmp
				BEGIN
			select @FeedBackUserId=UserId from #TempUsers where id=@Counter1
		 insert into #TempUserWithFeedBack(UserId,FeedBackUserId,IsUser) values(@UserId,@FeedBackUserId,1)
					SET @Counter1 = @Counter1 + 1;
				END;
    SET @Counter = @Counter + 1;
END;
 
 set @Counter  = 1;
WHILE @Counter <= @NoOfDeptHead
BEGIN
 select @FeedBackUserId=UserId from #TempDept where id=@Counter

					set @Counter1  = 1;
				   WHILE @Counter1 <= @NoOfEmp
				BEGIN
			select @UserId=UserId from #TempUsers where id=@Counter1
		 insert into #TempUserWithFeedBack(UserId,FeedBackUserId,IsDepartmentHead) values(@UserId,@FeedBackUserId,1)

					SET @Counter1 = @Counter1 + 1;
				END;
    SET @Counter = @Counter + 1;
END;
 
 set @Counter  = 1;
WHILE @Counter <= @NoOfHR
BEGIN
 select @FeedBackUserId=UserId from #TempHR where id=@Counter

					set @Counter1  = 1;
				   WHILE @Counter1 <= @NoOfEmp
				BEGIN
			select @UserId=UserId from #TempUsers where id=@Counter1
		 insert into #TempUserWithFeedBack(UserId,FeedBackUserId,IsHR) values(@UserId,@FeedBackUserId,1)

					SET @Counter1 = @Counter1 + 1;
				END;
    SET @Counter = @Counter + 1;
END;


 set @Counter  = 1;
WHILE @Counter <= @NoOfManagement
BEGIN
 select @FeedBackUserId=UserId from #TempManagement where id=@Counter

					set @Counter1  = 1;
				   WHILE @Counter1 <= @NoOfEmp
				BEGIN
			select @UserId=UserId from #TempUsers where id=@Counter1
		 insert into #TempUserWithFeedBack(UserId,FeedBackUserId,IsManagement) values(@UserId,@FeedBackUserId,1)

					SET @Counter1 = @Counter1 + 1;
				END;
    SET @Counter = @Counter + 1;
END;

insert into #TempUserWithFeedBack(UserId,FeedBackUserId,IsSupervisor)  
select j.EmployeeId,u.UserId ,1  from 
(
select EmployeeId,SUBSTRING(Supervisor, 1,  CHARINDEX('~', Supervisor) - 1) SupervisorCode
from EmployeeJob 
 where Supervisor is not null) 
 as j
left outer join ViewUsers u on j.SupervisorCode=u.Code
where j.EmployeeId in(
select UserId from #TempUsers)


insert into Appraisal360(DepartmentId,UserId,FeedBackUserId,FeedBackYear,FeedBackMonthId,IsFeedbackCompeted,IsUser,IsDepartmentHead,IsHR,IsManagement)
select @DepartmentId,UserId, FeedBackUserId,@FeedBackYear,@FeedBackMonthId,0,IsUser,IsDepartmentHead,IsHR,IsManagement from #TempUserWithFeedBack

select @FirstId=min(id) from Appraisal360  where DepartmentId=@DepartmentId
select @LastId=max(id) from Appraisal360  where DepartmentId=@DepartmentId
select @NoOfEmp= @LastId-@FirstId


set @Counter  = @FirstId;
WHILE @Counter <= @LastId
BEGIN
 select @UserType=case 
 when Isnull(IsUser,0)=1 then 'U'
 when Isnull(IsSupervisor,0)=1 then 'S'
 when Isnull(IsDepartmentHead,0)=1 then 'D'
 when Isnull(IsManagement,0)=1 then 'M'
 when Isnull(IsHR,0)=1 then 'H'
 else '' end 
 from Appraisal360 where id=@Counter
 if	@UserType='U'
 BEGIN
	insert into  Appraisal360Details(Appraisal360Id,DepartmentId,QuestionId,FeedbackValue,FeedbackUserType,FeedBackYear,FeedBackMonthId)
	select @Counter,DepartmentId,Id,0,@UserType,@FeedBackYear,@FeedBackMonthId from AppraisalQuestions
	where DepartmentId=@DepartmentId and isUser=1 and FeedBackYear=@FeedBackYear and FeedBackMonthId=@FeedBackMonthId
 end
 else  if	@UserType='S'
 BEGIN
	insert into  Appraisal360Details(Appraisal360Id,DepartmentId,QuestionId,FeedbackValue,FeedbackUserType,FeedBackYear,FeedBackMonthId)
	select @Counter,DepartmentId,Id,0,@UserType,@FeedBackYear,@FeedBackMonthId from AppraisalQuestions
	where DepartmentId=@DepartmentId and IsSupervisor=1 and FeedBackYear=@FeedBackYear and FeedBackMonthId=@FeedBackMonthId
 end
  else  if	@UserType='D'
 BEGIN
	insert into  Appraisal360Details(Appraisal360Id,DepartmentId,QuestionId,FeedbackValue,FeedbackUserType,FeedBackYear,FeedBackMonthId)
	select @Counter,DepartmentId,Id,0,@UserType,@FeedBackYear,@FeedBackMonthId from AppraisalQuestions
	where DepartmentId=@DepartmentId and IsDepartmentHead=1 and FeedBackYear=@FeedBackYear and FeedBackMonthId=@FeedBackMonthId
 end
   else  if	@UserType='M'
 BEGIN
	insert into  Appraisal360Details(Appraisal360Id,DepartmentId,QuestionId,FeedbackValue,FeedbackUserType,FeedBackYear,FeedBackMonthId)
	select @Counter,DepartmentId,Id,0,@UserType,@FeedBackYear,@FeedBackMonthId from AppraisalQuestions 
	where DepartmentId=@DepartmentId and IsManagement=1 and FeedBackYear=@FeedBackYear and FeedBackMonthId=@FeedBackMonthId
 end
    else  if	@UserType='H'
 BEGIN
	insert into  Appraisal360Details(Appraisal360Id,DepartmentId,QuestionId,FeedbackValue,FeedbackUserType,FeedBackYear,FeedBackMonthId)
	select @Counter,DepartmentId,Id,0,@UserType,@FeedBackYear,@FeedBackMonthId from AppraisalQuestions 
	where DepartmentId=@DepartmentId and IsHR=1 and FeedBackYear=@FeedBackYear and FeedBackMonthId=@FeedBackMonthId
 end
 SET @Counter = @Counter + 1;
END;

insert into AppraisalQuestionSetups(DepartmentId,FeedBackYear,FeedBackMonthId,UserQuestion,SupervisorQuestion,DepartmentHeadQuestion,ManagementQuestion,HRQuestion)
select distinct DepartmentId,@FeedBackYear,@FeedBackMonthId
,sum(case when IsUser=1 then 1 else 0 end) UserQuestion
,sum(case when IsSupervisor=1 then 1 else 0 end) SupervisorQuestion
,sum(case when IsDepartmentHead=1 then 1 else 0 end) DepartmentHeadQuestion
,sum(case when IsManagement=1 then 1 else 0 end) ManagementQuestion
,sum(case when IsHR=1 then 1 else 0 end) HRQuestion
from AppraisalQuestions 
where DepartmentId=@DepartmentId
 and FeedBackYear=@FeedBackYear and FeedBackMonthId=@FeedBackMonthId
group by DepartmentId

insert into AppraisalFeedbackPersonSetups(DepartmentId,UserId,FeedBackYear,FeedBackMonthId,UserNumber,SupervisorNumber,DepartmentHeadNumber,ManagementNumber,HRNumber)

select distinct DepartmentId,UserId,FeedBackYear,FeedBackMonthId
,isnull(sum(CAST(IsUser AS INT) ),0)UserPerson 
,isnull(sum(CAST(IsSupervisor AS INT) ),0)SupervisorPerson
,isnull(sum(CAST(IsDepartmentHead AS INT) ),0)DepartmentHeadPerson
,isnull(sum(CAST(IsManagement AS INT) ),0)ManagementPerson
,isnull(sum(CAST(IsHR AS INT) ),0)HRPerson
from Appraisal360
where DepartmentId=@DepartmentId
 and FeedBackYear=@FeedBackYear and FeedBackMonthId=@FeedBackMonthId
group by  DepartmentId,FeedBackYear,FeedBackMonthId,UserId

drop table #TempUsers
drop table #TempDept
drop table #TempHR
drop table #TempManagement
drop table #TempUserWithFeedBack

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@DepartmentId", DId);
                cmd.Parameters.AddWithValue("@FeedBackYear", FYId);
                cmd.Parameters.AddWithValue("@FeedBackMonthId", FiscalPeriodDetailId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";

                #endregion
            }
          
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

            return true;


        }
    }
}
