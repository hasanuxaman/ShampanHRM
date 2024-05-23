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
    public class AppraisalMarkSetupsDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods

        //==================SelectAll=================
        public List<AppraisalMarkSetupsVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<AppraisalMarkSetupsVM> appraisalMarkSetupsVMs = new List<AppraisalMarkSetupsVM>();
            AppraisalMarkSetupsVM appraisalMarkSetupsVM;
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
 ams.Id
,d.Name AS DepartmentId
,ams.FeedBackYear
,ams.FeedBackMonthId
,fy.PeriodName
,ams.EatchQuestionMark

,ams.SupervisorMark
,ams.DepartmentHeadMark
,ams.ManagementMark
,ams.HRMark
,ams.UserMark

  FROM AppraisalMarkSetups AS ams
  JOIN Department  AS d ON ams.DepartmentId=d.Id
  JOIN FiscalYearDetail AS fy ON fy.Id=ams.FeedBackMonthId

";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    appraisalMarkSetupsVM = new AppraisalMarkSetupsVM();
                    appraisalMarkSetupsVM.Id = Convert.ToInt32(dr["Id"]);
                    appraisalMarkSetupsVM.DepartmentId = dr["DepartmentId"].ToString();
                    appraisalMarkSetupsVM.FeedBackYear = Convert.ToInt32(dr["FeedBackYear"]);
                    appraisalMarkSetupsVM.FeedBackMonthId = Convert.ToInt32(dr["FeedBackMonthId"]);
                    appraisalMarkSetupsVM.EatchQuestionMark = Convert.ToInt32(dr["EatchQuestionMark"]);

                    appraisalMarkSetupsVM.UserMark = Convert.ToInt32(dr["UserMark"]);
                    appraisalMarkSetupsVM.SupervisorMark = Convert.ToInt32(dr["SupervisorMark"]);
                    appraisalMarkSetupsVM.DepartmentHeadMark = Convert.ToInt32(dr["DepartmentHeadMark"]);
                    appraisalMarkSetupsVM.ManagementMark = Convert.ToInt32(dr["ManagementMark"]);
                    appraisalMarkSetupsVM.HRMark = Convert.ToInt32(dr["HRMark"]);
                    appraisalMarkSetupsVM.PeriodName = dr["PeriodName"].ToString();





                    appraisalMarkSetupsVMs.Add(appraisalMarkSetupsVM);
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

            return appraisalMarkSetupsVMs;
        }

        //==================SelectByID=================
        public AppraisalMarkSetupsVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            AppraisalMarkSetupsVM appraisalMarkSetupsVM = new AppraisalMarkSetupsVM();

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
ams.Id
,ams.DepartmentId
,ams.FeedBackYear
,ams.FeedBackMonthId
,ams.EatchQuestionMark
,ams.UserMark
,ams.SupervisorMark	
,ams.DepartmentHeadMark
,ams.ManagementMark
,ams.HRMark	
,FYD.PeriodName

FROM AppraisalMarkSetups AS ams
LEFT OUTER JOIN FiscalYearDetail AS FYD ON FYD.Id = ams.FeedBackMonthId
  
Where  ams.id=@Id    
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
                    appraisalMarkSetupsVM = new AppraisalMarkSetupsVM();
                    appraisalMarkSetupsVM.Id = Convert.ToInt32(dr["Id"]);
                    appraisalMarkSetupsVM.DepartmentId = dr["DepartmentId"].ToString();
                    appraisalMarkSetupsVM.FeedBackYear = Convert.ToInt32(dr["FeedBackYear"]);
                    appraisalMarkSetupsVM.FeedBackMonthId = Convert.ToInt32(dr["FeedBackMonthId"]);
                    appraisalMarkSetupsVM.PeriodName = dr["PeriodName"].ToString();
                    appraisalMarkSetupsVM.EatchQuestionMark = Convert.ToInt32(dr["EatchQuestionMark"]);

                    appraisalMarkSetupsVM.UserMark = Convert.ToInt32(dr["UserMark"]);
                    appraisalMarkSetupsVM.SupervisorMark = Convert.ToInt32(dr["SupervisorMark"]);
                    appraisalMarkSetupsVM.DepartmentHeadMark = Convert.ToInt32(dr["DepartmentHeadMark"]);
                    appraisalMarkSetupsVM.ManagementMark = Convert.ToInt32(dr["ManagementMark"]);
                    appraisalMarkSetupsVM.HRMark = Convert.ToInt32(dr["HRMark"]);

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

            return appraisalMarkSetupsVM;
        }

        //==================Insert =================
        public string[] Insert(AppraisalMarkSetupsVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertappraisalMarkSetupsVM"; //Method Name
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
                string tableName = "AppraisalMarkSetups";
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
                //
                //}

                #endregion Find Transaction Exist

                //int foundId = (int)objfoundId;
                if (1 == 1)
                {
                    sqlText = "";
                    sqlText += " INSERT INTO AppraisalMarkSetups";
                    sqlText += "(";
                    sqlText += "  DepartmentId";
                    sqlText += "  ,FeedBackYear";
                    sqlText += "  ,FeedBackMonthId";
                    sqlText += "  ,EatchQuestionMark";
                    sqlText += "  ,UserMark";
                    sqlText += "  ,SupervisorMark";
                    sqlText += "  ,DepartmentHeadMark";
                    sqlText += "  ,ManagementMark";
                    sqlText += "  ,HRMark";
                    sqlText += ")";

                    sqlText += " VALUES(";
                    sqlText += " @DepartmentId";
                    sqlText += " ,@FeedBackYear";
                    sqlText += " ,@FeedBackMonthId";
                    sqlText += " ,@EatchQuestionMark";
                    sqlText += " ,@UserMark";
                    sqlText += " ,@SupervisorMark";
                    sqlText += " ,@DepartmentHeadMark";
                    sqlText += " ,@ManagementMark";
                    sqlText += " ,@HRMark";


                    sqlText += ")   SELECT SCOPE_IDENTITY() ";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId.Trim());
                    cmdInsert.Parameters.AddWithValue("@FeedBackYear", vm.FeedBackYear);
                    cmdInsert.Parameters.AddWithValue("@FeedBackMonthId", vm.FeedBackMonthId);
                    cmdInsert.Parameters.AddWithValue("@EatchQuestionMark", vm.EatchQuestionMark);
                    cmdInsert.Parameters.AddWithValue("@UserMark", vm.UserMark);
                    cmdInsert.Parameters.AddWithValue("@SupervisorMark", vm.SupervisorMark);
                    cmdInsert.Parameters.AddWithValue("@DepartmentHeadMark", vm.DepartmentHeadMark);
                    cmdInsert.Parameters.AddWithValue("@ManagementMark", vm.ManagementMark);
                    cmdInsert.Parameters.AddWithValue("@HRMark", vm.HRMark);
                    

                    cmdInsert.Transaction = transaction;
                    var exeRes = cmdInsert.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Appraisal Mark Setups Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Appraisal Mark Setups Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Appraisal Mark Setups already used";
                    throw new ArgumentNullException("Please Input Appraisal Mark Setups Value", "");
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
        public string[] Update(AppraisalMarkSetupsVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "AppraisalMarkSetups Update"; //Method Name

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
                string tableName = "AppraisalMarkSetups";
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
                    sqlText = "update AppraisalMarkSetups set";
                    sqlText += " DepartmentId=@DepartmentId";
                    sqlText += " ,FeedBackYear=@FeedBackYear";
                    sqlText += " ,FeedBackMonthId=@FeedBackMonthId";
                    sqlText += " ,EatchQuestionMark=@EatchQuestionMark";
                    sqlText += " ,UserMark=@UserMark";
                    sqlText += " ,SupervisorMark=@SupervisorMark";
                    sqlText += " ,DepartmentHeadMark=@DepartmentHeadMark";
                    sqlText += " ,ManagementMark=@ManagementMark";
                    sqlText += " ,HRMark=@HRMark";
                    
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@FeedBackYear", vm.FeedBackYear);
                    cmdUpdate.Parameters.AddWithValue("@FeedBackMonthId", vm.FeedBackMonthId);
                    cmdUpdate.Parameters.AddWithValue("@EatchQuestionMark", vm.EatchQuestionMark);
                    cmdUpdate.Parameters.AddWithValue("@UserMark", vm.UserMark);
                    cmdUpdate.Parameters.AddWithValue("@SupervisorMark", vm.SupervisorMark);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentHeadMark", vm.DepartmentHeadMark);
                    cmdUpdate.Parameters.AddWithValue("@ManagementMark", vm.ManagementMark);
                    cmdUpdate.Parameters.AddWithValue("@HRMark", vm.HRMark);
                   
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
                    throw new ArgumentNullException("AppraisalMarkSetups Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update AppraisalMarkSetups.";
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
        public AppraisalMarkSetupsVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            AppraisalMarkSetupsVM appraisalMarkSetupsVM = new AppraisalMarkSetupsVM();

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
,FeedBackYear
,FeedBackMonthId
,EatchQuestionMark
,UserMark
,SupervisorMark
,DepartmentHeadMark
,ManagementMark
,HRMark
    From AppraisalMarkSetups
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
                        appraisalMarkSetupsVM = new AppraisalMarkSetupsVM();
                        appraisalMarkSetupsVM.Id = Convert.ToInt32(dr["Id"]);
                        appraisalMarkSetupsVM.DepartmentId = dr["DepartmentId"].ToString();
                        appraisalMarkSetupsVM.FeedBackYear = Convert.ToInt32(dr["FeedBackYear"]);
                        appraisalMarkSetupsVM.FeedBackMonthId = Convert.ToInt32(dr["FeedBackMonthId"]);
                        appraisalMarkSetupsVM.EatchQuestionMark = Convert.ToInt32(dr["EatchQuestionMark"]);
                        appraisalMarkSetupsVM.SupervisorMark = Convert.ToInt32(dr["SupervisorMark"]);

                        appraisalMarkSetupsVM.UserMark = Convert.ToInt32(dr["UserMark"]);
                        appraisalMarkSetupsVM.DepartmentHeadMark = Convert.ToInt32(dr["DepartmentHeadMark"]);
                        appraisalMarkSetupsVM.ManagementMark = Convert.ToInt32(dr["ManagementMark"]);
                        appraisalMarkSetupsVM.HRMark = Convert.ToInt32(dr["HRMark"]);
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

            return appraisalMarkSetupsVM;
        }

        //==================Delete =================
        public string[] Delete(AppraisalMarkSetupsVM appraisalMarkSetupsVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteAppraisalMarkSetupsVM"; //Method Name

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
                        sqlText = "update AppraisalMarkSetups set";
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
                        throw new ArgumentNullException("Appraisal Mark Setups  Delete", appraisalMarkSetupsVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Appraisal Mark Setups Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Appraisal Mark Setups Information.";
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

        public List<AppraisalMarkSetupsVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AppraisalMarkSetupsVM> VMs = new List<AppraisalMarkSetupsVM>();
            AppraisalMarkSetupsVM vm;
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
DepartmentId
   FROM AppraisalMarkSetups
-- WHERE IsArchive=0 and IsActive=1
   -- ORDER BY Name
";
                SqlCommand _objComm = new SqlCommand(sqlText, currConn);
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AppraisalMarkSetupsVM();
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

        #endregion

    }
}
