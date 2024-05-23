using SymOrdinary;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.HRM
{
    public class EmployeeJobHistoryDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll Education=================
        public List<EmployeeJobHistoryVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeJobHistoryVM> VMs = new List<EmployeeJobHistoryVM>();
            EmployeeJobHistoryVM vm;
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
,Company
,JobTitle
,JobFrom
,JobTo
,ReasonForLeaving
,Remarks
,FileName
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

FROM EmployeeJobHistory where  IsArchive=0
ORDER BY Id DESC
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
                    vm = new EmployeeJobHistoryVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Company = dr["Company"].ToString();
                    vm.JobTitle = dr["JobTitle"].ToString();
                    vm.JobFrom = Ordinary.StringToDate(dr["JobFrom"].ToString());
                    vm.JobTo = Ordinary.StringToDate(dr["JobTo"].ToString());
                    
                    vm.ReasonForLeaving = dr["ReasonForLeaving"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.ServiceLength = Ordinary.DateDifferenceYMD(dr["JobFrom"].ToString(), dr["JobTo"].ToString(), false);
                    vm.FileName = dr["FileName"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        //==================SelectAll Education=================
        public List<EmployeeJobHistoryVM> SelectAllByEmployee(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeJobHistoryVM> employeeJobHistorys = new List<EmployeeJobHistoryVM>();
            EmployeeJobHistoryVM employeeJobHistory;
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
,EmployeeId
,Company
,JobTitle
,JobFrom
,JobTo
,ReasonForLeaving
,Remarks
,FileName
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
FROM EmployeeJobHistory where EmployeeId=@EmployeeId And IsArchive=0
ORDER BY Id DESC
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
                    employeeJobHistory = new EmployeeJobHistoryVM();
                    employeeJobHistory.Id = Convert.ToInt32(dr["Id"]);
                    employeeJobHistory.EmployeeId = dr["EmployeeId"].ToString();
                    employeeJobHistory.Company = dr["Company"].ToString();
                    employeeJobHistory.JobTitle = dr["JobTitle"].ToString();
                    employeeJobHistory.JobFrom =Ordinary.StringToDate( dr["JobFrom"].ToString());
                    employeeJobHistory.JobTo = Ordinary.StringToDate(dr["JobTo"].ToString());
                    employeeJobHistory.ServiceLength = Ordinary.DateDifferenceYMD(dr["JobFrom"].ToString(), dr["JobTo"].ToString(), false);
                    
                    employeeJobHistory.ReasonForLeaving = dr["ReasonForLeaving"].ToString();
                    employeeJobHistory.Remarks = dr["Remarks"].ToString();
                    employeeJobHistory.FileName = dr["FileName"].ToString();
                    employeeJobHistory.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeJobHistory.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    employeeJobHistory.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeJobHistory.CreatedBy = dr["CreatedBy"].ToString();
                    employeeJobHistory.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeJobHistory.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeJobHistory.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeJobHistory.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeJobHistorys.Add(employeeJobHistory);
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
            return employeeJobHistorys;
        }
        public EmployeeJobHistoryVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeJobHistoryVM employeeJobHistory = new EmployeeJobHistoryVM();

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
,Company
,JobTitle
,JobFrom
,JobTo
,FileName
,ReasonForLeaving
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
FROM EmployeeJobHistory 
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
                    employeeJobHistory.Id = Convert.ToInt32(dr["Id"]);
                    employeeJobHistory.EmployeeId = dr["EmployeeId"].ToString();
                    employeeJobHistory.Company = dr["Company"].ToString();
                    employeeJobHistory.JobTitle = dr["JobTitle"].ToString();
                    employeeJobHistory.JobFrom =Ordinary.StringToDate( dr["JobFrom"].ToString());
                    employeeJobHistory.JobTo = Ordinary.StringToDate(dr["JobTo"].ToString());
                    employeeJobHistory.ServiceLength = Ordinary.DateDifferenceYMD(dr["JobFrom"].ToString(), dr["JobTo"].ToString(), false);
                    
                    employeeJobHistory.ReasonForLeaving = dr["ReasonForLeaving"].ToString();
                    employeeJobHistory.Remarks = dr["Remarks"].ToString();
                    employeeJobHistory.FileName = dr["FileName"].ToString();
                    employeeJobHistory.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeJobHistory.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    employeeJobHistory.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeJobHistory.CreatedBy = dr["CreatedBy"].ToString();
                    employeeJobHistory.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeJobHistory.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeJobHistory.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeJobHistory.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeeJobHistory;
        }
        //==================Insert Education=================
        public string[] Insert(EmployeeJobHistoryVM empJobHistoryVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeJobHistory"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                //#region Validation
                //if (string.IsNullOrEmpty(empEducationVM.Remarks))
                //{
                //    retResults[1] = "Please Input Employee J";
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
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeJobHistory ";
                //sqlText += " WHERE Id=@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", empEducationVM.Id);
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Education Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Education Value", "");
                //}
                //#endregion Exist
                #region Save

                //int foundId = (int)objfoundId;
                if (1==1)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeJobHistory(EmployeeId,Company,JobTitle,JobFrom,JobTo
,FileName,ReasonForLeaving
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@EmployeeId,@Company,@JobTitle,@JobFrom,@JobTo
,@FileName,@ReasonForLeaving
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

//cmdExist1.Parameters.AddWithValue("@Id\",empEducationVM.Id            );
                    cmdInsert.Parameters.AddWithValue("@EmployeeId ",empJobHistoryVM.EmployeeId    );

                    cmdInsert.Parameters.AddWithValue("@Company", empJobHistoryVM.Company);
                    cmdInsert.Parameters.AddWithValue("@JobTitle", empJobHistoryVM.JobTitle);
                    cmdInsert.Parameters.AddWithValue("@JobFrom", Ordinary.DateToString(empJobHistoryVM.JobFrom));
                    cmdInsert.Parameters.AddWithValue("@JobTo", Ordinary.DateToString(empJobHistoryVM.JobTo));

                    
                    cmdInsert.Parameters.AddWithValue("@ReasonForLeaving", empJobHistoryVM.ReasonForLeaving ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", empJobHistoryVM.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FileName", empJobHistoryVM.FileName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", empJobHistoryVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", empJobHistoryVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", empJobHistoryVM.CreatedFrom);

                 

                    cmdInsert.Transaction = transaction;
					var exeRes = cmdInsert.ExecuteScalar();
					Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Education Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Education Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Education already used";
                    throw new ArgumentNullException("Please Input Employee Education Value", "");
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
        //==================Update JobHistory=================
        public string[] Update(EmployeeJobHistoryVM empJobHistoryVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "JobHistoryUpdate"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToJobHistory"); }

                #endregion open connection and transaction
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Code)Code FROM JobHistorys ";
                //sqlText += " WHERE Code=@Code AND Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", JobHistoryVM.Id);
                //cmdExist.Parameters.AddWithValue("@Code", JobHistoryVM.Code);

                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This JobHistorys already used";
                //    throw new ArgumentNullException("Please Input JobHistory Value", "");
                //}
                //#endregion Exist

                if (empJobHistoryVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeJobHistory set";
                    sqlText += " EmployeeId=@EmployeeId,";

                    sqlText += " Company=@Company,";
                    sqlText += " JobTitle=@JobTitle,";
                    sqlText += " JobFrom=@JobFrom,";
                    sqlText += " JobTo=@JobTo,";
                    
                    sqlText += " ReasonForLeaving=@ReasonForLeaving,";
                    sqlText += " Remarks=@Remarks,";
                    if (empJobHistoryVM.FileName != null)
                    {
                        sqlText += " FileName=@FileName,";
                    }
                   // sqlText += " IsActive=@IsActive       ,";
                    sqlText += " IsArchive=@IsArchive       ,";
                    sqlText += " LastUpdateBy=@LastUpdateBy   ,";
                    sqlText += " LastUpdateAt=@LastUpdateAt   ,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom ";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", empJobHistoryVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId ", empJobHistoryVM.EmployeeId);

                    cmdUpdate.Parameters.AddWithValue("@Company", empJobHistoryVM.Company);
                    cmdUpdate.Parameters.AddWithValue("@JobTitle", empJobHistoryVM.JobTitle);
                    cmdUpdate.Parameters.AddWithValue("@JobFrom", Ordinary.DateToString(empJobHistoryVM.JobFrom));
                    cmdUpdate.Parameters.AddWithValue("@JobTo", Ordinary.DateToString(empJobHistoryVM.JobTo));

                    if (empJobHistoryVM.FileName != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", empJobHistoryVM.FileName ?? Convert.DBNull);
                    }

                    cmdUpdate.Parameters.AddWithValue("@ReasonForLeaving", empJobHistoryVM.ReasonForLeaving ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", empJobHistoryVM.Remarks ?? Convert.DBNull);
                   // cmdUpdate.Parameters.AddWithValue("@IsActive", empJobHistoryVM.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", empJobHistoryVM.IsArchive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", empJobHistoryVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", empJobHistoryVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", empJobHistoryVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = empJobHistoryVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("JobHistoryUpdate", empJobHistoryVM.Company + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("JobHistoryUpdate", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update JobHistory.";
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


        public string[] Delete(EmployeeJobHistoryVM empJobHistoryVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteTransfer"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToTransfer"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeJobHistory set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", empJobHistoryVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", empJobHistoryVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", empJobHistoryVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Job History Delete", empJobHistoryVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Job History Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Job History Information.";
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
        public List<EmployeeJobHistoryVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeJobHistoryVM> VMs = new List<EmployeeJobHistoryVM>();
            EmployeeJobHistoryVM vm;
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
 isnull(JH.Id,0)						  Id
,ei.EmployeeId							EmployeeId
,isnull(JH.Company		, 'NA')			 Company
,isnull(JH.JobTitle		, 'NA')			 JobTitle
,isnull(JH.JobFrom		, 'NA')			 JobFrom
,isnull(JH.JobTo			, 'NA')			 JobTo
,isnull(JH.[FileName]	, 'NA')				  [FileName]
,JH.ReasonForLeaving
,isnull(JH.Remarks		, 'NA')			Remarks
,isnull(JH.IsActive, 0)			IsActive
,isnull(JH.IsArchive, 0)			IsArchive
,isnull(JH.CreatedBy, 'NA')		 CreatedBy
,isnull(JH.CreatedAt, 'NA')		 CreatedAt
,isnull(JH.CreatedFrom, 'NA')		CreatedFrom
,isnull(JH.LastUpdateBy, 'NA')	 LastUpdateBy
,isnull(JH.LastUpdateAt,	'NA')	 LastUpdateAt
,isnull(JH.LastUpdateFrom,	'NA')	 LastUpdateFrom   

    From ViewEmployeeInformation ei
		left outer join EmployeeJobHistory JH on ei.EmployeeId=JH.EmployeeId
Where ei.IsArchive=0 and ei.isActive=1 and JH.IsArchive=0 and JH.isActive=1
";
                if (CodeF != "0_0")
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and ei.DepartmentId=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and ei.DesignationId=@DesignationId";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and ei.ProjectId=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and ei.SectionId=@SectionId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ei.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ei.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and ei.EmployeeId=@EmployeeId";
                }

                sqlText += "  order by ei.Department, ei.GradeSL, ei.joindate, ei.Code ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;


                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }

                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeJobHistoryVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Company = dr["Company"].ToString();
                    vm.JobTitle = dr["JobTitle"].ToString();
                    vm.JobFrom = Ordinary.StringToDate(dr["JobFrom"].ToString());
                    vm.JobTo = Ordinary.StringToDate(dr["JobTo"].ToString());
                    
                    vm.ReasonForLeaving = dr["ReasonForLeaving"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.ServiceLength = Ordinary.DateDifferenceYMD(dr["JobFrom"].ToString(), dr["JobTo"].ToString(), false);
                    vm.FileName = dr["FileName"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
