using SymOrdinary;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.Enum
{
    public class EnumSalaryTypeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EnumSalaryTypeVM> SelectAllOthers(int BranchId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EnumSalaryTypeVM> salaryTypeVMs = new List<EnumSalaryTypeVM>();
            EnumSalaryTypeVM salaryTypeVM;
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
,Code
,Name
,BranchId
,TypeName
 ,isnull(SL,0)SL,isnull(IsGross,0)IsGross
,Remarks
,IsActive
,IsArchive
,IsEarning
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,isnull(GLAccountCode,'NA')GLAccountCode
,LastUpdateFrom
    From EnumSalaryType
Where IsArchive=0 And BranchId=@BranchId
   and  (TypeName IN ('other')) AND (IsGross = 0)
 ORDER BY IsEarning DESC, SL, Name
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@BranchId", BranchId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    salaryTypeVM = new EnumSalaryTypeVM();
                    salaryTypeVM.Id = dr["Id"].ToString();
                    salaryTypeVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    salaryTypeVM.Code = dr["Code"].ToString();
                    salaryTypeVM.Name = dr["Name"].ToString();
                    salaryTypeVM.TypeName = dr["TypeName"].ToString();
                    salaryTypeVM.Remarks = dr["Remarks"].ToString();
                    salaryTypeVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    salaryTypeVM.IsEarning = Convert.ToBoolean(dr["IsEarning"]);
                    salaryTypeVM.IsGross = Convert.ToBoolean(dr["IsGross"]);
                    salaryTypeVM.SL = Convert.ToInt32(dr["SL"]);
                    salaryTypeVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    salaryTypeVM.CreatedBy = dr["CreatedBy"].ToString();
                    salaryTypeVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    salaryTypeVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    salaryTypeVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    salaryTypeVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    salaryTypeVM.GLAccountCode = dr["GLAccountCode"].ToString();
                    salaryTypeVMs.Add(salaryTypeVM);
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

            return salaryTypeVMs;
        }
        public List<EnumSalaryTypeVM> SelectAll(int BranchId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EnumSalaryTypeVM> salaryTypeVMs = new List<EnumSalaryTypeVM>();
            EnumSalaryTypeVM salaryTypeVM;
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
,Code
,Name
,BranchId
,TypeName
 ,isnull(SL,0)SL,isnull(IsGross,0)IsGross
,Remarks
,IsActive
,IsArchive
,IsEarning
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,isnull(GLAccountCode,'NA')GLAccountCode
,LastUpdateFrom
    From EnumSalaryType
Where IsArchive=0 And BranchId=@BranchId
       ORDER BY Isearning desc,sl,Name   

";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@BranchId", BranchId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    salaryTypeVM = new EnumSalaryTypeVM();
                    salaryTypeVM.Id = dr["Id"].ToString();
                    salaryTypeVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    salaryTypeVM.Code = dr["Code"].ToString();
                    salaryTypeVM.Name = dr["Name"].ToString();
                    salaryTypeVM.TypeName = dr["TypeName"].ToString();
                    salaryTypeVM.Remarks = dr["Remarks"].ToString();
                    salaryTypeVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    salaryTypeVM.IsEarning = Convert.ToBoolean(dr["IsEarning"]);
                    salaryTypeVM.IsGross = Convert.ToBoolean(dr["IsGross"]);
                    salaryTypeVM.SL = Convert.ToInt32(dr["SL"]);
                    salaryTypeVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    salaryTypeVM.CreatedBy = dr["CreatedBy"].ToString();
                    salaryTypeVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    salaryTypeVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    salaryTypeVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    salaryTypeVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    salaryTypeVM.GLAccountCode = dr["GLAccountCode"].ToString();
                    salaryTypeVMs.Add(salaryTypeVM);
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

            return salaryTypeVMs;
        }

        public List<EnumSalaryTypeVM> SelectAllPrinciple(int BranchId = 0, string Id = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EnumSalaryTypeVM> salaryTypeVMs = new List<EnumSalaryTypeVM>();
            EnumSalaryTypeVM salaryTypeVM;
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

                sqlText += @"
SELECT
Id
,Code
,Name
,BranchId
,TypeName
 ,isnull(SL,0)SL,isnull(IsGross,0)IsGross
,Remarks
,IsActive
,IsArchive
,IsEarning
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,LastUpdateFrom
,isnull(GLAccountCode,'NA')GLAccountCode
    From EnumSalaryType
Where 1=1 And BranchId=@BranchId

and   (IsArchive = 0) AND ( (TypeName NOT IN ('other', 'gross')) OR
                         (IsArchive = 0) AND (IsGross = 1))

";
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    sqlText += " AND Id=@Id";
                }

                sqlText += " ORDER BY IsEarning DESC, SL, Name";


                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@BranchId", BranchId);
                
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    salaryTypeVM = new EnumSalaryTypeVM();
                    salaryTypeVM.Id = dr["Id"].ToString();
                    salaryTypeVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    salaryTypeVM.Code = dr["Code"].ToString();
                    salaryTypeVM.Name = dr["Name"].ToString();
                    salaryTypeVM.TypeName = dr["TypeName"].ToString();
                    salaryTypeVM.Remarks = dr["Remarks"].ToString();
                    salaryTypeVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    salaryTypeVM.IsEarning = Convert.ToBoolean(dr["IsEarning"]);
                    salaryTypeVM.IsEarning = Convert.ToBoolean(dr["IsEarning"]);
                    salaryTypeVM.IsGross = Convert.ToBoolean(dr["IsGross"]);
                    salaryTypeVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    salaryTypeVM.CreatedBy = dr["CreatedBy"].ToString();
                    salaryTypeVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    salaryTypeVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    salaryTypeVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    salaryTypeVM.GLAccountCode = dr["GLAccountCode"].ToString();
                    salaryTypeVMs.Add(salaryTypeVM);
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

            return salaryTypeVMs;
        }

        //==================SelectByID=================
        public EnumSalaryTypeVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EnumSalaryTypeVM SalaryTypeVM = new EnumSalaryTypeVM();

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
,Code
,Name
,TypeName
 ,isnull(SL,0)SL,isnull(IsGross,0)IsGross
,BranchId
,Remarks
,IsActive
,IsEarning
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,isnull(GLAccountCode,'NA')GLAccountCode
,LastUpdateFrom
    From EnumSalaryType
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
                    SalaryTypeVM.Id = dr["Id"].ToString();
                    SalaryTypeVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    SalaryTypeVM.Code = dr["Code"].ToString();
                    SalaryTypeVM.Name = dr["Name"].ToString();
                    SalaryTypeVM.TypeName = dr["TypeName"].ToString();
                    SalaryTypeVM.Remarks = dr["Remarks"].ToString();
                    SalaryTypeVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    SalaryTypeVM.IsEarning = Convert.ToBoolean(dr["IsEarning"]);
                    SalaryTypeVM.IsEarning = Convert.ToBoolean(dr["IsEarning"]);
                    SalaryTypeVM.IsGross = Convert.ToBoolean(dr["IsGross"]); 
                    SalaryTypeVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    SalaryTypeVM.CreatedBy = dr["CreatedBy"].ToString();
                    SalaryTypeVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    SalaryTypeVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    SalaryTypeVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    SalaryTypeVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    SalaryTypeVM.GLAccountCode = dr["GLAccountCode"].ToString();

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

            return SalaryTypeVM;
        }
        //==================Insert =================
        public string[] Insert(EnumSalaryTypeVM VM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertSalaryType"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(SalaryTypeVM.DepartmentId))
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
                sqlText = "  ";
                sqlText += " SELECT count( BranchId) FROM EnumSalaryType ";
                sqlText += " WHERE name=@Name   and BranchId=@BranchId and TypeName in('gross','Basic','tax','PFEmployeer','PFEmployee','Loan','LoanInterest')";
                SqlCommand cmdExist2 = new SqlCommand(sqlText, currConn);
                cmdExist2.Transaction = transaction;
                cmdExist2.Parameters.AddWithValue("@Name", VM.Name);
                cmdExist2.Parameters.AddWithValue("@BranchId", VM.BranchId);

                var exeRes1 = cmdExist2.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes1);
                if (objfoundId > 0)
                {
                    retResults[1] = "Salary type already used!";
                    throw new ArgumentNullException("This Salary Type Can't Update", "");
                }
                #endregion Exist

                #region Exist 
               

                #endregion Exist


                #region Save


                if (objfoundId<=0)
                {
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from EnumSalaryType where BranchId=@BranchId";
                    SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                    cmd2.Transaction = transaction;
                    cmd2.Parameters.AddWithValue("@BranchId", VM.BranchId);
					var exeRes = cmd2.ExecuteScalar();
					int count = Convert.ToInt32(exeRes);
                    VM.Id = VM.BranchId.ToString() + "_" + (count + 1);

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EnumSalaryType(Id,GLAccountCode,sl,IsGross,Code,Name,NameTrim,TypeName,BranchId,Remarks,IsActive,IsEarning,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                            VALUES (@Id,@GLAccountCode,@sl,@IsGross,@Code,@Name,@NameTrim,@TypeName,@BranchId,@Remarks,@IsActive,@IsEarning,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        ";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@Id", VM.Id);
                    cmdInsert.Parameters.AddWithValue("@Code", VM.Code.Trim());
                    cmdInsert.Parameters.AddWithValue("@Name", VM.Name.Trim());
                    cmdInsert.Parameters.AddWithValue("@NameTrim", VM.Name.Replace(" ","").Trim());
                    cmdInsert.Parameters.AddWithValue("@sl", "0");
                    cmdInsert.Parameters.AddWithValue("@IsGross", "0");
                    cmdInsert.Parameters.AddWithValue("@TypeName", "Other");
                    cmdInsert.Parameters.AddWithValue("@BranchId", VM.BranchId);
                    cmdInsert.Parameters.AddWithValue("@Remarks", VM.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@GLAccountCode", VM.GLAccountCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsEarning", VM.IsEarning);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", VM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", VM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", VM.CreatedFrom);

                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This Salary Type already used";
                    throw new ArgumentNullException("Please Input SalaryType Value", "");
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
                retResults[1] = "Data Save Successfully";
                retResults[2] = Id.ToString();

                #endregion SuccessResult

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }

            catch (Exception ex)
            {
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
        public string[] Update(EnumSalaryTypeVM VM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee SalaryType Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSalaryType"); }

                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) Id FROM EnumSalaryType ";
                sqlText += " WHERE Name=@Name And Id<>@Id and BranchId=@BranchId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Id", VM.Id);
                cmdExist.Parameters.AddWithValue("@Name", VM.Name.Trim());
                cmdExist.Parameters.AddWithValue("@BranchId", VM.BranchId);

                var exeRes = cmdExist.ExecuteScalar();
				int exists = Convert.ToInt32(exeRes);
                if (exists > 0)
                {
                    retResults[1] = "This Salary Type already used";
                    throw new ArgumentNullException("This Salary Type already used", "");
                }
                #endregion Exist

                #region Exist
                sqlText = "  ";
                sqlText += " SELECT count( BranchId) FROM EnumSalaryType ";
                sqlText += " WHERE name=@Name and Id<>@Id and BranchId=@BranchId and TypeName in('gross','Basic','tax','PFEmployeer','PFEmployee','Loan','LoanInterest')";
               

                SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                cmdExist1.Transaction = transaction;
                cmdExist1.Parameters.AddWithValue("@Id", VM.Id);
                cmdExist1.Parameters.AddWithValue("@Name", VM.Name);
                cmdExist1.Parameters.AddWithValue("@BranchId", VM.BranchId);

                var exeRes1 = cmdExist1.ExecuteScalar();
                int exists1 = Convert.ToInt32(exeRes1);
                if (exists1 > 0)
                {
                    retResults[1] = "Salary type already used!";
                    throw new ArgumentNullException("This Salary Type Can't Update", "");
                }
                #endregion Exist


                if (VM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EnumSalaryType set";
                    sqlText += " Code=@Code,";
                    sqlText += " Name=@Name,";
                    sqlText += " NameTrim=@NameTrim,";
                    sqlText += " BranchId=@BranchId,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsEarning=@IsEarning,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " GLAccountCode=@GLAccountCode,";

                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", VM.Id);
                    cmdUpdate.Parameters.AddWithValue("@Code", VM.Code.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Name", VM.Name.Trim());
                    cmdUpdate.Parameters.AddWithValue("@NameTrim", VM.Name.Replace(" ","").Trim());
                    cmdUpdate.Parameters.AddWithValue("@BranchId", VM.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", VM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@GLAccountCode", VM.GLAccountCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsEarning", VM.IsEarning);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", VM.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", VM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", VM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", VM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = VM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", SalaryTypeVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("SalaryType Update", "Could not found any item.");
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
                    retResults[1] = "Requested SalaryType Information Successfully Updated.";

                }
                else
                {
                    retResults[1] = "Unexpected error to update SalaryType.";
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
        public string[] EditPrinciple(string Id, string GLAccountCode, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee SalaryType Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSalaryType"); }

                #endregion open connection and transaction


              
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EnumSalaryType set";
                    sqlText += " GLAccountCode=@GLAccountCode";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", Id);
                    cmdUpdate.Parameters.AddWithValue("@GLAccountCode", GLAccountCode ?? Convert.DBNull);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", SalaryTypeVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                


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
                    retResults[1] = "Requested Salary Type Information Successfully Updated.";

                }
                else
                {
                    retResults[1] = "Unexpected error to update SalaryType.";
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
        
        public string[] Delete(EnumSalaryTypeVM vm, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "SalaryType Delete"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length > 0)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {

                        #region Exist
                        sqlText = "  ";
                        sqlText += " SELECT count( BranchId) FROM EnumSalaryType ";
                        sqlText += " WHERE Id=@Id   and TypeName in('gross','Basic','tax','PFEmployeer','PFEmployee','Loan','LoanInterest')";
                        SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                        cmdExist1.Transaction = transaction;
                        cmdExist1.Parameters.AddWithValue("@Id", ids[i]);

                        var exeRes1 = cmdExist1.ExecuteScalar();
                        int exists1 = Convert.ToInt32(exeRes1);
                        if (exists1 > 0)
                        {
                            retResults[1] = "This Salary Type Can't Update";
                            throw new ArgumentNullException("This Salary Type Can't Update", "");
                        }
                        #endregion Exist

                        #region Exist
                        sqlText = "  ";
                        sqlText += " SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) ID FROM EnumSalaryType ";
                        sqlText += " WHERE TypeName in('gross','basic') And Id=@Id  ";


                        SqlCommand cmdExistG = new SqlCommand(sqlText, currConn);
                        cmdExistG.Transaction = transaction;
                        cmdExistG.Parameters.AddWithValue("@Id", ids[i]);
						var exeRes = cmdExistG.ExecuteScalar();
						int exists = Convert.ToInt32(exeRes);

                        if (exists > 0)
                        {
                            retResults[1] = "This Gross / Basic  Salary Type Can't Delete";
                            throw new ArgumentNullException("This Gross / Basic  Salary Type Can't Delete", "");
                        }
                        #endregion Exist

                        sqlText = "";
                        sqlText = "update EnumSalaryType set";
                        sqlText += " IsActive=@IsActive";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " ,LastUpdateBy=@LastUpdateBy";
                        sqlText += " ,LastUpdateAt=@LastUpdateAt";
                        sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                        transResult = (int)cmdUpdate.ExecuteNonQuery();
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Salary Type Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Type Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Project Information.";
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
        public List<EnumSalaryTypeVM> DropDownSalaryPortion(int BranchId)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EnumSalaryTypeVM> VMs = new List<EnumSalaryTypeVM>();
            EnumSalaryTypeVM vm;
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
TypeName,
Name
   FROM EnumSalaryType
WHERE IsArchive=0 and IsActive=1 and TypeName in('Gross','Basic') and BranchId=@BranchId
    ORDER BY Name
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", BranchId);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EnumSalaryTypeVM();
                    vm.Id = dr["TypeName"].ToString();
                    vm.Name = dr["Name"].ToString();
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

        public List<EnumSalaryTypeVM> DropDown(int BranchId, string ET)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EnumSalaryTypeVM> VMs = new List<EnumSalaryTypeVM>();
            EnumSalaryTypeVM vm;
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
Name
   FROM EnumSalaryType
WHERE IsArchive=0 and IsActive=1 and   BranchId=@BranchId and TypeName in('Basic','other','HouseRent','Medical','Conveyance','CarAllowance','TransportAllowance')
";
                if (!string.IsNullOrWhiteSpace(ET))
                {
                    sqlText += @" and   IsEarning=@ET";
                }
                sqlText += @" ORDER BY Name";


                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", BranchId);
                if (!string.IsNullOrWhiteSpace(ET))
                {
                    if (ET == "E")
                    {
                        _objComm.Parameters.AddWithValue("@ET", 1);
                    }
                    if (ET == "D")
                    {
                        _objComm.Parameters.AddWithValue("@ET", 0);
                    }
                }
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EnumSalaryTypeVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Name = dr["Name"].ToString();
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
