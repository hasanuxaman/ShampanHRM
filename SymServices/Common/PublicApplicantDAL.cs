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
    public class PublicApplicantDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<PublicApplicantVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<PublicApplicantVM> PublicApplicantVMs = new List<PublicApplicantVM>();
            PublicApplicantVM PublicApplicantVM;
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
 pa.Id
,pa.BranchId
,pa.JobCircularId
,jc.JobTitle  JobTitle
,desi.Name DesignationName
,pa.ApplicantName
,pa.ApplicantEmail
,pa.Expriance
,pa.CVIdentificationNo
,pa.CVName
,pa.WorkingExprianceDetail
,pa.PersonalDetail
,pa.Description
,pa.Remarks
,pa.IsActive
,pa.IsArchive
,pa.CreatedBy
,pa.CreatedAt
,pa.CreatedFrom
,pa.LastUpdateBy
,pa.LastUpdateAt
,pa.LastUpdateFrom
From PublicApplicant pa
left outer join JobCircular jc on pa.JobCircularId=jc.Id
left outer join designation desi on jc.DesignationId=desi.Id
Where pa.IsArchive=0
order by jc.JobTitle 
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    PublicApplicantVM = new PublicApplicantVM();
                    PublicApplicantVM.Id = dr["Id"].ToString();
                    PublicApplicantVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    PublicApplicantVM.JobCircularId = dr["JobCircularId"].ToString();
                    PublicApplicantVM.JobTitle = dr["JobTitle"].ToString();
                    PublicApplicantVM.DesignationName = dr["DesignationName"].ToString();
                    PublicApplicantVM.ApplicantName = dr["ApplicantName"].ToString();
                    PublicApplicantVM.ApplicantEmail = dr["ApplicantEmail"].ToString();
                    PublicApplicantVM.Expriance = dr["Expriance"].ToString();
                    PublicApplicantVM.CVIdentificationNo = dr["CVIdentificationNo"].ToString();
                    PublicApplicantVM.CVName = dr["CVName"].ToString();
                    PublicApplicantVM.WorkingExprianceDetail = dr["WorkingExprianceDetail"].ToString();
                    PublicApplicantVM.PersonalDetail = dr["PersonalDetail"].ToString();
                    PublicApplicantVM.Description = dr["Description"].ToString();
                    PublicApplicantVM.Remarks = dr["Remarks"].ToString();
                    PublicApplicantVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    PublicApplicantVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    PublicApplicantVM.CreatedBy = dr["CreatedBy"].ToString();
                    PublicApplicantVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    PublicApplicantVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    PublicApplicantVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    PublicApplicantVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    PublicApplicantVMs.Add(PublicApplicantVM);
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

            return PublicApplicantVMs;
        }
        //==================SelectByID=================
        public PublicApplicantVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            PublicApplicantVM PublicApplicantVM = new PublicApplicantVM();

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
 pa.Id
,pa.BranchId
,pa.JobCircularId
,jc.JobTitle  JobTitle
,desi.Name DesignationName
,pa.ApplicantName
,pa.ApplicantEmail
,pa.Expriance
,pa.CVIdentificationNo
,pa.CVName
,pa.WorkingExprianceDetail
,pa.PersonalDetail
,pa.Description
,pa.Remarks
,pa.IsActive
,pa.IsArchive
,pa.CreatedBy
,pa.CreatedAt
,pa.CreatedFrom
,pa.LastUpdateBy
,pa.LastUpdateAt
,pa.LastUpdateFrom
From PublicApplicant pa
left outer join JobCircular jc on pa.JobCircularId=jc.Id
left outer join designation desi on jc.DesignationId=desi.Id
Where  pa.id=@Id and pa.IsArchive=0
order by jc.JobTitle 
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
                    PublicApplicantVM.Id = dr["Id"].ToString();
                    PublicApplicantVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    PublicApplicantVM.JobCircularId = dr["JobCircularId"].ToString();
                    PublicApplicantVM.JobTitle = dr["JobTitle"].ToString();
                    PublicApplicantVM.DesignationName = dr["DesignationName"].ToString();
                    PublicApplicantVM.ApplicantName = dr["ApplicantName"].ToString();
                    PublicApplicantVM.ApplicantEmail = dr["ApplicantEmail"].ToString();
                    PublicApplicantVM.Expriance = dr["Expriance"].ToString();
                    PublicApplicantVM.CVIdentificationNo = dr["CVIdentificationNo"].ToString();
                    PublicApplicantVM.CVName = dr["CVName"].ToString();
                    PublicApplicantVM.WorkingExprianceDetail = dr["WorkingExprianceDetail"].ToString();
                    PublicApplicantVM.PersonalDetail = dr["PersonalDetail"].ToString();
                    PublicApplicantVM.Description = dr["Description"].ToString();
                    PublicApplicantVM.Remarks = dr["Remarks"].ToString();
                    PublicApplicantVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    PublicApplicantVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    PublicApplicantVM.CreatedBy = dr["CreatedBy"].ToString();
                    PublicApplicantVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    PublicApplicantVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    PublicApplicantVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    PublicApplicantVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return PublicApplicantVM;
        }
        //==================Insert =================
        public string[] Insert(PublicApplicantVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertPublicApplicant"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation

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
                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "PublicApplicant";
                string[] fieldName = { "ApplicantName", "ApplicantEmail" };
                string[] fieldValue = { vm.ApplicantName.Trim(), vm.ApplicantEmail.Trim()};

                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsertWithBranch(tableName, fieldName[i], fieldValue[i], vm.BranchId, currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist


                #endregion open connection and transaction

                #region Save
                sqlText = "Select IsNull(isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0),0) from PublicApplicant where BranchId=@BranchId";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);
                vm.Id = vm.BranchId.ToString() + "_" + (count + 1);
                vm.CVIdentificationNo=(1000+count + 1).ToString();
                //int foundId = (int)objfoundId;
                if (1 == 1)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO PublicApplicant(Id,BranchId,JobCircularId,ApplicantName,ApplicantEmail,Expriance,CVIdentificationNo,CVName,WorkingExprianceDetail,PersonalDetail,Description,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Id,@BranchId,@JobCircularId,@ApplicantName,@ApplicantEmail,@Expriance,@CVIdentificationNo,@CVName,@WorkingExprianceDetail,@PersonalDetail,@Description,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) ";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@JobCircularId", vm.JobCircularId.Trim());
                    cmdInsert.Parameters.AddWithValue("@ApplicantName", vm.ApplicantName.Trim());
                    cmdInsert.Parameters.AddWithValue("@ApplicantEmail", vm.ApplicantEmail.Trim());
                    cmdInsert.Parameters.AddWithValue("@Expriance", vm.Expriance.Trim());
                    cmdInsert.Parameters.AddWithValue("@CVIdentificationNo", vm.CVIdentificationNo.Trim());
                     cmdInsert.Parameters.AddWithValue("@CVName", vm.CVName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@WorkingExprianceDetail", vm.WorkingExprianceDetail ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PersonalDetail", vm.PersonalDetail?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, PublicApplicantVM.Remarks);
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
                    retResults[1] = "This PublicApplicant already used!";
                    throw new ArgumentNullException("Please Input PublicApplicant Value", "");
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
                retResults[2] = vm.Id;

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
        public string[] Update(PublicApplicantVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PublicApplicant Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToPublicApplicant"); }

                #endregion open connection and transaction
                #region Exist

                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "PublicApplicant";
               string[] fieldName = { "ApplicantName", "ApplicantEmail","CVIdentificationNo" };
                string[] fieldValue = { vm.ApplicantName.Trim(), vm.ApplicantEmail.Trim() , vm.CVIdentificationNo.Trim()};
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInUpdateWithBranch(vm.Id, tableName, fieldName[i], fieldValue[i], vm.BranchId, currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist
                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update PublicApplicant set";
                    sqlText += " BranchId=@BranchId";
                    sqlText += " , JobCircularId=@JobCircularId";
                    sqlText += " , ApplicantName=@ApplicantName";
                    sqlText += " , ApplicantEmail=@ApplicantEmail";
                    sqlText += " , Expriance=@Expriance";
                    sqlText += " , CVIdentificationNo=@CVIdentificationNo";
                    sqlText += " , WorkingExprianceDetail=@WorkingExprianceDetail";
                    sqlText += " , PersonalDetail=@PersonalDetail";
                    sqlText += " , Description=@Description";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@JobCircularId", vm.JobCircularId.Trim());
                    cmdUpdate.Parameters.AddWithValue("@ApplicantName", vm.ApplicantName.Trim());
                    cmdUpdate.Parameters.AddWithValue("@ApplicantEmail", vm.ApplicantEmail.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Expriance", vm.Expriance.Trim());
                    cmdUpdate.Parameters.AddWithValue("@CVIdentificationNo", vm.CVIdentificationNo.Trim());
                    cmdUpdate.Parameters.AddWithValue("@WorkingExprianceDetail", vm.WorkingExprianceDetail.Trim());
                    cmdUpdate.Parameters.AddWithValue("@PersonalDetail", vm.PersonalDetail.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Description", vm.Description.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, PublicApplicantVM.Remarks);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", PublicApplicantVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("PublicApplicant Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update PublicApplicant.";
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
      
        public string[] Delete(PublicApplicantVM PublicApplicantVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeletePublicApplicant"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToPublicApplicant"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update PublicApplicant set";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", PublicApplicantVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", PublicApplicantVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", PublicApplicantVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("PublicApplicant Delete", PublicApplicantVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("PublicApplicant Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete PublicApplicant Information.";
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
    }
}
