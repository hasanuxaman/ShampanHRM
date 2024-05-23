using SymOrdinary;
using SymViewModel.Common;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Common
{
    public class Appraisal360UserFeedbackDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods

        //==================SelectAll=================
        public List<Appraisal360FeedBackVM> SelectAll(Appraisal360FeedBackVM vm)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<Appraisal360FeedBackVM> appraisalQuestionsVMs = new List<Appraisal360FeedBackVM>();
            Appraisal360FeedBackVM appraisalQuestionsVM;
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
SELECT AP.*,D.Name AS DesignationName  FROM ViewAppraisal360 AP
LEFT OUTER JOIN EmployeeJob EMJ ON AP.UserId=EMJ.EmployeeId
LEFT OUTER JOIN Designation D ON EMJ.DesignationId=D.Id 
WHERE 1=1  ";
                if (!string.IsNullOrEmpty(vm.UserId))
                {
                    sqlText += @" AND AP.UserId = '" + vm.UserId + "' ";
                }

                if (!string.IsNullOrEmpty(vm.FeedBackUserId))
                {
                    sqlText += @" AND AP.FeedBackUserId = '" + vm.FeedBackUserId + "' ";
                }
                if (!string.IsNullOrEmpty(vm.Appraisal360Id))
                {
                    sqlText += @" AND AP.Appraisal360Id = '" + vm.Appraisal360Id + "' ";
                }
                if (!string.IsNullOrEmpty(vm.DepartmentName))
                {
                    sqlText += @" AND AP.DepartmentName = '" + vm.DepartmentName.Trim() + "' ";
                }

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    appraisalQuestionsVM = new Appraisal360FeedBackVM();
                    appraisalQuestionsVM.FeedBackYear = dr["FeedBackYear"].ToString();
                    appraisalQuestionsVM.PeriodName = dr["PeriodName"].ToString();
                    appraisalQuestionsVM.DepartmentName = dr["DepartmentName"].ToString();
                    appraisalQuestionsVM.DesignationName = dr["DesignationName"].ToString();
                    appraisalQuestionsVM.UserCode = dr["UserCode"].ToString();
                    appraisalQuestionsVM.UserName = dr["UserName"].ToString();
                    appraisalQuestionsVM.FeedbackCode = dr["FeedbackCode"].ToString();
                    appraisalQuestionsVM.FeedbackName = dr["FeedbackName"].ToString();
                    appraisalQuestionsVM.IsFeedbackCompeted = dr["IsFeedbackCompeted"].ToString();
                    appraisalQuestionsVM.UserId = dr["UserId"].ToString();
                    appraisalQuestionsVM.FeedBackUserId = dr["FeedBackUserId"].ToString();
                    appraisalQuestionsVM.FeedBackMonth = dr["FeedBackMonth"].ToString();
                    appraisalQuestionsVM.Appraisal360Id = dr["Appraisal360Id"].ToString();
                    appraisalQuestionsVM.FeedbackBy = dr["FeedbackBy"].ToString();


                    appraisalQuestionsVMs.Add(appraisalQuestionsVM);
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

            return appraisalQuestionsVMs;
        }


        public List<Appraisal360DetailVM> SelectAllList(Appraisal360FeedBackVM vm)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<Appraisal360DetailVM> appraisalQuestionsVMs = new List<Appraisal360DetailVM>();
            Appraisal360DetailVM appraisalQuestionsVM;
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
SELECT * FROM ViewAppraisal360Details 
WHERE 1=1  ";

                if (!string.IsNullOrEmpty(vm.Appraisal360Id))
                {
                    sqlText += @" AND Appraisal360Id = '" + vm.Appraisal360Id + "' ";
                }

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    appraisalQuestionsVM = new Appraisal360DetailVM();
                    appraisalQuestionsVM.Id = Convert.ToInt32(dr["Appraisal360DetailId"].ToString());
                    appraisalQuestionsVM.FeedBackYear = dr["FeedBackYear"].ToString();
                    appraisalQuestionsVM.PeriodName = dr["PeriodName"].ToString();
                    appraisalQuestionsVM.DepartmentName = dr["DepartmentName"].ToString();
                    appraisalQuestionsVM.UserCode = dr["UserCode"].ToString();
                    appraisalQuestionsVM.UserName = dr["UserName"].ToString();
                    appraisalQuestionsVM.FeedbackCode = dr["FeedbackCode"].ToString();
                    appraisalQuestionsVM.FeedbackName = dr["FeedbackName"].ToString();
                    appraisalQuestionsVM.IsFeedbackCompeted = dr["IsFeedbackCompeted"].ToString();
                    appraisalQuestionsVM.UserId = dr["UserId"].ToString();
                    appraisalQuestionsVM.FeedBackUserId = dr["FeedBackUserId"].ToString();
                    appraisalQuestionsVM.Appraisal360Id = dr["Appraisal360Id"].ToString();
                    appraisalQuestionsVM.FeedbackBy = dr["FeedbackBy"].ToString();
                    appraisalQuestionsVM.Question = dr["Question"].ToString();
                    appraisalQuestionsVM.FeedbackValue = dr["FeedbackValue"].ToString();


                    appraisalQuestionsVMs.Add(appraisalQuestionsVM);
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

            return appraisalQuestionsVMs;
        }

        //==================SelectByID=================
        public AppraisalQuestionsVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            AppraisalQuestionsVM appraisalQuestionsVM = new AppraisalQuestionsVM();

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
,DepartmentId
,FeedBackYear
,FeedBackMonthId
,Question
,IsUser
,IsSupervisor
,IsDepartmentHead
,IsManagement
,IsHR
   FROM AppraisalQuestions
Where  id=@Id    
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
                    appraisalQuestionsVM = new AppraisalQuestionsVM();
                    appraisalQuestionsVM.Id = Convert.ToInt32(dr["Id"]);
                    appraisalQuestionsVM.DepartmentId = dr["DepartmentId"].ToString();
                    appraisalQuestionsVM.Question = dr["Question"].ToString();

                    appraisalQuestionsVM.IsUser = Convert.ToBoolean(dr["IsUser"]);
                    appraisalQuestionsVM.IsSupervisor = Convert.ToBoolean(dr["IsSupervisor"]);
                    appraisalQuestionsVM.IsDepartmentHead = Convert.ToBoolean(dr["IsDepartmentHead"]);
                    appraisalQuestionsVM.IsManagement = Convert.ToBoolean(dr["IsManagement"]);
                    appraisalQuestionsVM.IsHR = Convert.ToBoolean(dr["IsHR"]);

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

            return appraisalQuestionsVM;
        }

        //==================Insert =================
        public string[] Insert(AppraisalQuestionsVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                string tableName = "AppraisalQuestions";
                string[] fieldName = { "Question" };
                string[] fieldValue = { vm.Question.Trim() };

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
                //
                //}

                #endregion Find Transaction Exist

                //int foundId = (int)objfoundId;
                if (1 == 1)
                {
                    sqlText = "";
                    sqlText += " INSERT INTO AppraisalQuestions";
                    sqlText += "(";
                    sqlText += " DepartmentId";
                    sqlText += " ,FeedBackYear";
                    sqlText += " ,FeedBackMonthId";
                    sqlText += " ,Question";
                    sqlText += " ,IsUser";
                    sqlText += " ,IsSupervisor";
                    sqlText += " ,IsDepartmentHead";
                    sqlText += " ,IsManagement";
                    sqlText += " ,IsHR";
                    sqlText += " ,IsActive";
                    sqlText += " ,IsArchive";

                    sqlText += ")";

                    sqlText += " VALUES(";
                    sqlText += " @DepartmentId";
                    sqlText += " ,@FeedBackYear";
                    sqlText += " ,@FeedBackMonthId";
                    sqlText += " ,@Question";
                    sqlText += " ,@IsUser";
                    sqlText += " ,@IsSupervisor";
                    sqlText += " ,@IsDepartmentHead";
                    sqlText += " ,@IsManagement";
                    sqlText += " ,@IsHR";
                    sqlText += " ,@IsActive";
                    sqlText += " ,@IsArchive";

                    sqlText += ")   SELECT SCOPE_IDENTITY() ";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId.Trim());
                    cmdInsert.Parameters.AddWithValue("@FeedBackYear", vm.FeedBackYear);
                    cmdInsert.Parameters.AddWithValue("@FeedBackMonthId", vm.FeedBackMonthId);
                    cmdInsert.Parameters.AddWithValue("@Question", vm.Question.Trim());
                    cmdInsert.Parameters.AddWithValue("@IsUser", vm.IsUser);
                    cmdInsert.Parameters.AddWithValue("@IsSupervisor", vm.IsSupervisor);
                    cmdInsert.Parameters.AddWithValue("@IsDepartmentHead", vm.IsDepartmentHead);
                    cmdInsert.Parameters.AddWithValue("@IsManagement", vm.IsManagement);
                    cmdInsert.Parameters.AddWithValue("@IsHR", vm.IsHR);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);

                    cmdInsert.Transaction = transaction;
                    var exeRes = cmdInsert.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input AppraisalQuestions Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input AppraisalQuestions Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This AppraisalQuestions already used";
                    throw new ArgumentNullException("Please Input AppraisalQuestions Value", "");
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
        public string[] Update(Appraisal360FeedBackVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Appraisal360FeedBack Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("Appraisal360FeedBackVM"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    foreach (var item in vm.Details)
                    {
                        #region Update Details

                        sqlText = "";
                        sqlText = " UPDATE Appraisal360Details SET ";
                        sqlText += " FeedbackValue=@FeedbackValue ";

                        sqlText += " WHERE Id=@Id ";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", item.Id);
                        cmdUpdate.Parameters.AddWithValue("@FeedbackValue", item.FeedbackValue);

                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        retResults[2] = vm.Id.ToString();
                        retResults[3] = sqlText;

                        #region Commit

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("Appraisal360FeedBack Update", vm.UserName + " Appraisal360 FeedBack could not updated.");
                        }

                        #endregion Commit

                        #endregion Update Details
                    }

                    #region Update Header

                    sqlText = "";
                    sqlText = " UPDATE Appraisal360 SET ";
                    sqlText += " FeedbackDate=GETDATE() ";

                    sqlText += " WHERE Id=@Id ";

                    SqlCommand update = new SqlCommand(sqlText, currConn);
                    update.Parameters.AddWithValue("@Id", vm.Id);

                    update.Transaction = transaction;
                    var res = update.ExecuteNonQuery();
                    transResult = Convert.ToInt32(res);

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Appraisal360FeedBack Update", vm.UserName + " Appraisal360 FeedBack could not updated.");
                    }

                    #endregion Update Header

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Appraisal360FeedBack Update", "Appraisal360 FeedBack Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Appraisal360FeedBack.";
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


        //==================Update =================
        public string[] CompletedFeedBack(Appraisal360FeedBackVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Appraisal360FeedBack Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("CompletedFeedBack"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Header

                    sqlText = "";
                    sqlText = " UPDATE Appraisal360 SET ";
                    sqlText += " IsFeedbackCompeted=1 ";

                    sqlText += " WHERE Id=@Id ";

                    SqlCommand update = new SqlCommand(sqlText, currConn);
                    update.Parameters.AddWithValue("@Id", vm.Id);

                    update.Transaction = transaction;
                    var res = update.ExecuteNonQuery();
                    transResult = Convert.ToInt32(res);

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("CompletedFeedBack Update", vm.UserName + " Appraisal360 FeedBack could not updated.");
                    }

                    #endregion Update Header

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Completed FeedBack Update", "Appraisal360 FeedBack Could not found any item.");
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
                    retResults[1] = "Data Complete Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to update Appraisal360FeedBack.";
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
        public AppraisalQuestionsVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            AppraisalQuestionsVM appraisalQuestionsVM = new AppraisalQuestionsVM();

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
,Question
,IsUser
,IsSupervisor
,IsDepartmentHead
,IsManagement
,IsHR
    From AppraisalQuestions
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
                        appraisalQuestionsVM.Id = Convert.ToInt32(dr["Id"]);
                        appraisalQuestionsVM.DepartmentId = dr["DepartmentId"].ToString();
                        appraisalQuestionsVM.Question = dr["Question"].ToString();

                        appraisalQuestionsVM.IsUser = Convert.ToBoolean(dr["IsUser"]);
                        appraisalQuestionsVM.IsSupervisor = Convert.ToBoolean(dr["IsSupervisor"]);
                        appraisalQuestionsVM.IsDepartmentHead = Convert.ToBoolean(dr["IsDepartmentHead"]);
                        appraisalQuestionsVM.IsManagement = Convert.ToBoolean(dr["IsManagement"]);
                        appraisalQuestionsVM.IsHR = Convert.ToBoolean(dr["IsHR"]);
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

            return appraisalQuestionsVM;
        }

        //==================Delete =================
        public string[] Delete(AppraisalQuestionsVM appraisalQuestionsVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToAppraisalQuestionsVM"); }

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
                        sqlText = "update AppraisalQuestions set";
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
                        throw new ArgumentNullException("AppraisalQuestions Delete", appraisalQuestionsVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("AppraisalQuestions Information Delete", "Could not found any item.");
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

        public List<AppraisalQuestionsVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AppraisalQuestionsVM> VMs = new List<AppraisalQuestionsVM>();
            AppraisalQuestionsVM vm;
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
                sqlText = @" SELECT
Id,
Question
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
                    vm = new AppraisalQuestionsVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Question = dr["Question"].ToString();
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

        public List<EnumValue> GetFeedbackValue(string periodName, string feedBackYear)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EnumValue> VMs = new List<EnumValue>();
            List<EnumValue> vmList = new List<EnumValue>();
            EnumValue vm;
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
SELECT TOP 1 EatchQuestionMark AS Id, EatchQuestionMark AS MarkValue 
FROM AppraisalMarkSetups AMS
LEFT OUTER JOIN FiscalYearDetail FYD ON AMS.FeedBackMonthId = FYD.Id
WHERE 1=1  ";

                if (!string.IsNullOrEmpty(periodName))
                {
                    sqlText += @"  AND AMS.PeriodName = '" + periodName + "' ";
                }
                if (!string.IsNullOrEmpty(feedBackYear))
                {
                    sqlText += @" AND AMS.FeedBackYear = '" + feedBackYear + "' ";
                }

                SqlCommand _objComm = new SqlCommand(sqlText, currConn);
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EnumValue();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Value = dr["MarkValue"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion

                int number = 0;
                foreach (var item in VMs)
                {
                    for (int i = 0; i < item.Id; i++)
                    {
                        number++;
                        vm = new EnumValue();
                        vm.Id = number;
                        vm.Value = number.ToString();
                        vmList.Add(vm);
                    }
                }

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
            return vmList;
        }

        #endregion

    }
}
