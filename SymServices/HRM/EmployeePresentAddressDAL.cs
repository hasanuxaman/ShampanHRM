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
    public class EmployeePresentAddressDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeePresentAddressVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePresentAddressVM> employeePresentAddressVMs = new List<EmployeePresentAddressVM>();
            EmployeePresentAddressVM employeePresentAddressVM;
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
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Fax
,Mobile
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
    From EmployeePresentAddress
Where IsArchive=0
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeePresentAddressVM = new EmployeePresentAddressVM();
                    employeePresentAddressVM.Id = Convert.ToInt32(dr["Id"]);
                    employeePresentAddressVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeePresentAddressVM.Address = dr["Address"].ToString();
                    employeePresentAddressVM.District = dr["District"].ToString();
                    employeePresentAddressVM.Division = dr["Division"].ToString();
                    employeePresentAddressVM.Country = dr["Country"].ToString();
                    employeePresentAddressVM.City = dr["City"].ToString();
                    employeePresentAddressVM.PostalCode = dr["PostalCode"].ToString();
                    employeePresentAddressVM.Phone = dr["Phone"].ToString();
                    employeePresentAddressVM.Fax = dr["Fax"].ToString();
                    employeePresentAddressVM.FileName = dr["FileName"].ToString();
                    employeePresentAddressVM.Mobile = dr["Mobile"].ToString();
                    employeePresentAddressVM.Remarks = dr["Remarks"].ToString();
                    employeePresentAddressVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeePresentAddressVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeePresentAddressVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeePresentAddressVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeePresentAddressVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeePresentAddressVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeePresentAddressVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeePresentAddressVMs.Add(employeePresentAddressVM);
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

            return employeePresentAddressVMs;
        }
        //==================SelectByID=================
        public EmployeePresentAddressVM SelectByEmployeeId(string EmployeeId )
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePresentAddressVM employeePresentAddressVM = new EmployeePresentAddressVM();

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
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Fax
,Mobile
,Isnull(PostOffice,'')PostOffice
,FileName
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeePresentAddress
where  EmployeeId=@EmployeeId  
     
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeePresentAddressVM.Id = Convert.ToInt32(dr["Id"]);
                    employeePresentAddressVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeePresentAddressVM.Address = dr["Address"].ToString();
                    employeePresentAddressVM.District = dr["District"].ToString();
                    employeePresentAddressVM.Division = dr["Division"].ToString();
                    employeePresentAddressVM.Country = dr["Country"].ToString();
                    employeePresentAddressVM.City = dr["City"].ToString();
                    employeePresentAddressVM.PostalCode = dr["PostalCode"].ToString();
                    employeePresentAddressVM.PostOffice = dr["PostOffice"].ToString();
                    employeePresentAddressVM.Phone = dr["Phone"].ToString();
                    employeePresentAddressVM.Fax = dr["Fax"].ToString();
                    employeePresentAddressVM.Mobile = dr["Mobile"].ToString();
                    employeePresentAddressVM.FileName = dr["FileName"].ToString();
                    employeePresentAddressVM.Remarks = dr["Remarks"].ToString();
                    employeePresentAddressVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeePresentAddressVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeePresentAddressVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeePresentAddressVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeePresentAddressVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeePresentAddressVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeePresentAddressVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeePresentAddressVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeePresentAddressVM employeePresentAddressVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeePresentAddress"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(employeePresentAddressVM.Name))
                //{
                //    retResults[1] = "Please Input Employee Nominee Name";
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

                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeNominee ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", employeePresentAddressVM.EmployeeId);
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Nominee Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Nominee Value", "");
                //}
                //#endregion Exist
                #region Save

                //int foundId = (int)objfoundId;
                if (1 == 1)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeePresentAddress(	EmployeeId,Address,District,Division,Country
                                ,City,PostOffice,PostalCode,Phone,Fax,Mobile,FileName,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@EmployeeId,@Address,@District,@Division,@Country,@City,@PostOffice,@PostalCode,@Phone,@Fax,@Mobile,@FileName
                                        ,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@EmployeeId", employeePresentAddressVM.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Address", employeePresentAddressVM.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@District", employeePresentAddressVM.District ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Division", employeePresentAddressVM.Division ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Country", employeePresentAddressVM.Country ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@City", employeePresentAddressVM.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PostalCode", employeePresentAddressVM.PostalCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PostOffice", employeePresentAddressVM.PostOffice ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Phone", employeePresentAddressVM.Phone ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Fax", employeePresentAddressVM.Fax ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Mobile", employeePresentAddressVM.Mobile ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FileName", employeePresentAddressVM.FileName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", employeePresentAddressVM.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", employeePresentAddressVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", employeePresentAddressVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", employeePresentAddressVM.CreatedFrom);

                    cmdInsert.Transaction = transaction;
					var exeRes = cmdInsert.ExecuteScalar();
					Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Present Address  Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Present Address  Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Present Address Bangla already used";
                    throw new ArgumentNullException("Please Input Employee Present Address  Value", "");
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
                FileLogger.Log("EmployeePresentAddressDAL", retResults[5].ToString(), ex.Message.ToString());
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
        public string[] Update(EmployeePresentAddressVM employeePresentAddressVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Present Address  Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToPresentAddress"); }

                #endregion open connection and transaction
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeNominee ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", employeePresentAddressVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", employeePresentAddressVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", employeePresentAddressVM.Name);

                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This Nominee already used";
                //    throw new ArgumentNullException("Please Input Nominee Value", "");
                //}
                //#endregion Exist

                if (employeePresentAddressVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeePresentAddress set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " Address=@Address,";
                    sqlText += " District=@District,";
                    sqlText += " Division=@Division,";
                    sqlText += " Country=@Country,";
                    sqlText += " City=@City,";
                    sqlText += " PostalCode=@PostalCode,";
                    sqlText += " PostOffice=@PostOffice,";
                    sqlText += " Phone=@Phone,";
                    sqlText += " Fax=@Fax,";
                    sqlText += " Mobile=@Mobile,";
                    if (employeePresentAddressVM.FileName != null)
                    {
                        sqlText += " FileName=@FileName,";
                    }
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", employeePresentAddressVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", employeePresentAddressVM.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@Address", employeePresentAddressVM.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@District", employeePresentAddressVM.District ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Division", employeePresentAddressVM.Division ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Country", employeePresentAddressVM.Country ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", employeePresentAddressVM.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostalCode", employeePresentAddressVM.PostalCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostOffice", employeePresentAddressVM.PostOffice ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Phone", employeePresentAddressVM.Phone ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Fax", employeePresentAddressVM.Fax ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Mobile", employeePresentAddressVM.Mobile ?? Convert.DBNull);
                    if (employeePresentAddressVM.FileName != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", employeePresentAddressVM.FileName ?? Convert.DBNull);
                    }
                    cmdUpdate.Parameters.AddWithValue("@Remarks", employeePresentAddressVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeePresentAddressVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeePresentAddressVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeePresentAddressVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = employeePresentAddressVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Present Address  Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Present Address .";
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
        public EmployeePresentAddressVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeePresentAddressVM employeePresentAddressVM = new EmployeePresentAddressVM();

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
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Fax
,Mobile
,FileName
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeePresentAddress 
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
                        employeePresentAddressVM.Id = Convert.ToInt32(dr["Id"]);
                        employeePresentAddressVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeePresentAddressVM.Address = dr["Address"].ToString();
                        employeePresentAddressVM.District = dr["District"].ToString();
                        employeePresentAddressVM.Division = dr["Division"].ToString();
                        employeePresentAddressVM.Country = dr["Country"].ToString();
                        employeePresentAddressVM.City = dr["City"].ToString();
                        employeePresentAddressVM.PostalCode = dr["PostalCode"].ToString();
                        employeePresentAddressVM.Phone = dr["Phone"].ToString();
                        employeePresentAddressVM.Fax = dr["Fax"].ToString();
                        employeePresentAddressVM.Mobile = dr["Mobile"].ToString();
                        employeePresentAddressVM.FileName = dr["FileName"].ToString();
                        employeePresentAddressVM.Remarks = dr["Remarks"].ToString();
                        employeePresentAddressVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeePresentAddressVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeePresentAddressVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeePresentAddressVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeePresentAddressVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeePresentAddressVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeePresentAddressVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return employeePresentAddressVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeePresentAddressVM EmployeePresentAddressVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeletePresent Address Bangla"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToPresentAddress"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (EmployeePresentAddressVM.Id > 0)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeePresentAddress set";
                    sqlText += " IsArchive=@IsArchive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", EmployeePresentAddressVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeePresentAddressVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeePresentAddressVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeePresentAddressVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = EmployeePresentAddressVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Present Address Delete", EmployeePresentAddressVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Present Address  Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Present Address .";
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
        public List<EmployeePresentAddressVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePresentAddressVM> VMs = new List<EmployeePresentAddressVM>();
            EmployeePresentAddressVM vm;
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
isnull(PrAd.Id, 0)                      Id
,ei.EmployeeId                          EmployeeId
,isnull(PrAd.[Address]	, 'NA')			 [Address]
,isnull(PrAd.District	, 'NA')			 District
,isnull(PrAd.Division	, 'NA')			 Division
,isnull(PrAd.Country	, 'NA')			 Country
,isnull(PrAd.City		, 'NA')			 City
,isnull(PrAd.PostalCode	, 'NA')		 PostalCode
,isnull(PrAd.Phone		, 'NA')			 Phone
,isnull(PrAd.Fax		, 'NA')			 Fax
,isnull(PrAd.Mobile		, 'NA')		 Mobile
,isnull(PrAd.Remarks	, 'NA')			 Remarks
,isnull(PrAd.[FileName]	, 'NA')		 [FileName]
,isnull(PrAd.IsActive		, 0)		 IsActive
,isnull(PrAd.IsArchive		, 0)		 IsArchive
,isnull(PrAd.CreatedBy		, 'NA')		 CreatedBy
,isnull(PrAd.CreatedAt		, 'NA')		 CreatedAt
,isnull(PrAd.CreatedFrom	, 'NA')		 CreatedFrom
,isnull(PrAd.LastUpdateBy	, 'NA')		 LastUpdateBy
,isnull(PrAd.LastUpdateAt	, 'NA')		 LastUpdateAt
,isnull(PrAd.LastUpdateFrom	, 'NA')	 LastUpdateFrom
    From ViewEmployeeInformation ei
		left outer join EmployeePresentAddress PrAd on ei.EmployeeId=PrAd.EmployeeId
Where ei.IsArchive=0 and ei.isActive=1
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


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeePresentAddressVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.District = dr["District"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.PostalCode = dr["PostalCode"].ToString();
                    vm.Phone = dr["Phone"].ToString();
                    vm.Fax = dr["Fax"].ToString();
                    vm.FileName = dr["FileName"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
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
