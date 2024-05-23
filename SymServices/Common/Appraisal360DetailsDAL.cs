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
    public class Appraisal360DetailsDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods

        //==================SelectAll=================
        public List<Appraisal360DetailsVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<Appraisal360DetailsVM> appraisal360DetailsVMs = new List<Appraisal360DetailsVM>();
            Appraisal360DetailsVM appraisal360DetailsVM;
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
 ad.Id
,ad.Appraisal360Id
,d.Name AS DepartmentId
,ad.FeedBackYear
,ad.FeedBackMonthId
,fyd.PeriodName
,ad.QuestionId
,aq.Question
,ad.FeedbackValue
,ad.FeedbackUserType
    FROM Appraisal360Details AS ad
	JOIN Department AS d ON ad.DepartmentId=d.Id
	JOIN AppraisalQuestions AS aq ON ad.QuestionId=aq.Id
	JOIN FiscalYearDetail AS fyd ON ad.FeedBackMonthId=fyd.Id

";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    appraisal360DetailsVM = new Appraisal360DetailsVM();
                    appraisal360DetailsVM.Id = Convert.ToInt32(dr["Id"]);
                    appraisal360DetailsVM.Appraisal360Id = Convert.ToInt32(dr["Appraisal360Id"]);
                    appraisal360DetailsVM.DepartmentId = dr["DepartmentId"].ToString();
                    appraisal360DetailsVM.FeedBackYear = Convert.ToInt32(dr["FeedBackYear"]);
                    appraisal360DetailsVM.FeedBackMonthId = Convert.ToInt32(dr["FeedBackMonthId"]);
                    appraisal360DetailsVM.PeriodName = dr["PeriodName"].ToString();
                    appraisal360DetailsVM.QuestionId = Convert.ToInt32(dr["QuestionId"].ToString());
                    appraisal360DetailsVM.Question = dr["Question"].ToString();
                    appraisal360DetailsVM.FeedbackValue = Convert.ToInt32(dr["FeedbackValue"].ToString());
                    appraisal360DetailsVM.FeedbackUserType = dr["FeedbackUserType"].ToString();

                    appraisal360DetailsVMs.Add(appraisal360DetailsVM);
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

            return appraisal360DetailsVMs;
        }

        //==================SelectByID=================
        public Appraisal360DetailsVM SelectById(int Id)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            Appraisal360DetailsVM appraisal360DetailsVM = new Appraisal360DetailsVM();

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
 Appraisal360Details.Id
,IsNull(Appraisal360Id,0) Appraisal360Id
,IsNull(DepartmentId,'')DepartmentId
,IsNull(FeedBackYear,0)FeedBackYear
,IsNull(FeedBackMonthId,0)FeedBackMonthId
,IsNull(QuestionId,0)QuestionId
,IsNull(FeedbackValue,0)FeedbackValue
,IsNull(FeedbackUserType,'')FeedbackUserType
,ISNULL (FYD.PeriodName,0)PeriodName
FROM Appraisal360Details
LEFT OUTER JOIN FiscalYearDetail FYD ON FYD.Id = Appraisal360Details.FeedBackMonthId

Where  Appraisal360Details.Id=@Id    

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
                    appraisal360DetailsVM = new Appraisal360DetailsVM();
                    appraisal360DetailsVM = new Appraisal360DetailsVM();
                    appraisal360DetailsVM.Id = Convert.ToInt32(dr["Id"]);
                    appraisal360DetailsVM.Appraisal360Id = Convert.ToInt32(dr["Appraisal360Id"]);
                    appraisal360DetailsVM.DepartmentId = dr["DepartmentId"].ToString();
                    appraisal360DetailsVM.FeedBackYear = Convert.ToInt32(dr["FeedBackYear"]);
                    appraisal360DetailsVM.FeedBackMonthId = Convert.ToInt32(dr["FeedBackMonthId"]);
                    appraisal360DetailsVM.PeriodName = dr["PeriodName"].ToString();

                    appraisal360DetailsVM.QuestionId = Convert.ToInt32(dr["QuestionId"].ToString());
                    appraisal360DetailsVM.FeedbackValue = Convert.ToInt32(dr["FeedbackValue"].ToString());
                    appraisal360DetailsVM.FeedbackUserType = dr["FeedbackUserType"].ToString();

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

            return appraisal360DetailsVM;
        }

        //==================Insert =================
        public string[] Insert(Appraisal360DetailsVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                string tableName = "Appraisal360Details";
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
                    sqlText += " INSERT INTO Appraisal360Details";
                    sqlText += "(";
                    sqlText += " Appraisal360Id";
                    sqlText += " ,DepartmentId";
                    sqlText += " ,FeedBackYear";
                    sqlText += " ,FeedBackMonthId";
                    sqlText += " ,QuestionId";
                    sqlText += " ,FeedbackValue";
                    sqlText += " ,FeedbackUserType";
                    


                    sqlText += ")";

                    sqlText += " VALUES(";
                    sqlText += " @Appraisal360Id";
                    sqlText += " ,@DepartmentId";
                    sqlText += " ,@FeedBackYear";
                    sqlText += " ,@FeedBackMonthId";
                    sqlText += " ,@QuestionId";
                    sqlText += " ,@FeedbackValue";
                    sqlText += " ,@FeedbackUserType";
                   

                    sqlText += ")   SELECT SCOPE_IDENTITY() ";


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@Appraisal360Id", vm.Appraisal360Id);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId.Trim());
                    cmdInsert.Parameters.AddWithValue("@FeedBackYear", vm.FeedBackYear);
                    cmdInsert.Parameters.AddWithValue("@FeedBackMonthId", vm.FeedBackMonthId);
                    cmdInsert.Parameters.AddWithValue("@QuestionId", vm.QuestionId);
                    cmdInsert.Parameters.AddWithValue("@FeedbackValue", vm.FeedbackValue);
                    cmdInsert.Parameters.AddWithValue("@FeedbackUserType", vm.FeedbackUserType.Trim());
                   


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
        public string[] Update(Appraisal360DetailsVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "AppraisalQuestions Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToAppraisalQuestionsVM"); }

                #endregion open connection and transaction

                #region Exist

                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Appraisal360Details";
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
                    sqlText = "update Appraisal360Details set";
                    sqlText += " Appraisal360Id=@Appraisal360Id,";
                    sqlText += " DepartmentId=@DepartmentId,";
                    sqlText += " FeedBackYear=@FeedBackYear,";
                    sqlText += " FeedBackMonthId=@FeedBackMonthId,";
                    sqlText += " QuestionId=@QuestionId,";

                    sqlText += " FeedbackValue=@FeedbackValue,";
                    sqlText += " FeedbackUserType=@FeedbackUserType";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Appraisal360Id", vm.Appraisal360Id);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@FeedBackYear", vm.FeedBackYear);

                    cmdUpdate.Parameters.AddWithValue("@FeedBackMonthId", vm.FeedBackMonthId);
                    cmdUpdate.Parameters.AddWithValue("@QuestionId", vm.QuestionId);
                    cmdUpdate.Parameters.AddWithValue("@FeedbackValue", vm.FeedbackValue);
                    cmdUpdate.Parameters.AddWithValue("@FeedbackUserType", vm.FeedbackUserType);
                    

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
                    throw new ArgumentNullException("Appraisal360Details Update", "Could not found any item.");
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
        public Appraisal360DetailsVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            Appraisal360DetailsVM appraisal360DetailsVM = new Appraisal360DetailsVM();

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
,Appraisal360Id
,DepartmentId
,FeedBackYear
,FeedBackMonthId
,QuestionId
,FeedbackValue
,FeedbackUserType
    From Appraisal360Details
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
                        
                        appraisal360DetailsVM.Id = Convert.ToInt32(dr["Id"]);
                        appraisal360DetailsVM.Appraisal360Id = Convert.ToInt32(dr["Appraisal360Id"]);
                        appraisal360DetailsVM.DepartmentId = dr["DepartmentId"].ToString();
                        appraisal360DetailsVM.FeedBackYear = Convert.ToInt32(dr["FeedBackYear"]);
                        appraisal360DetailsVM.FeedBackMonthId = Convert.ToInt32(dr["FeedBackMonthId"]);
                        appraisal360DetailsVM.QuestionId = Convert.ToInt32(dr["QuestionId"].ToString());
                        appraisal360DetailsVM.FeedbackValue = Convert.ToInt32(dr["FeedbackValue"].ToString());
                        appraisal360DetailsVM.FeedbackUserType = dr["FeedbackUserType"].ToString();
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

            return appraisal360DetailsVM;
        }

        //==================Delete =================
        public string[] Delete(Appraisal360DetailsVM appraisal360DetailsVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                        throw new ArgumentNullException("AppraisalQuestions Delete", appraisal360DetailsVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Appraisal360 DetailsInformation Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Appraisal360 Details Information.";
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

        public List<Appraisal360DetailsVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<Appraisal360DetailsVM> VMs = new List<Appraisal360DetailsVM>();
            Appraisal360DetailsVM vm;
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
Id,
Appraisal360Id
   FROM Appraisal360Details
-- WHERE IsArchive=0 and IsActive=1
   -- ORDER BY Name
";
                SqlCommand _objComm = new SqlCommand(sqlText, currConn);
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new Appraisal360DetailsVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Appraisal360Id = Convert.ToInt32(dr["Appraisal360Id"]);
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

        #endregion

    }
}
