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
    public class EmployeePresentAddressBanglaDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeePresentAddressBanglaVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePresentAddressBanglaVM> employeePresentAddressBanglaVMs = new List<EmployeePresentAddressBanglaVM>();
            EmployeePresentAddressBanglaVM employeePresentAddressBanglaVM;
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
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeePresentAddressBangla
Where IsArchive=0
    ORDER BY Name
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeePresentAddressBanglaVM = new EmployeePresentAddressBanglaVM();
                    employeePresentAddressBanglaVM.Id = Convert.ToInt32(dr["Id"]);
                    employeePresentAddressBanglaVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeePresentAddressBanglaVM.Address = dr["Address"].ToString();
                    employeePresentAddressBanglaVM.District = dr["District"].ToString();
                    employeePresentAddressBanglaVM.Division = dr["Division"].ToString();
                    employeePresentAddressBanglaVM.Country = dr["Country"].ToString();
                    employeePresentAddressBanglaVM.City = dr["City"].ToString();
                    employeePresentAddressBanglaVM.Remarks = dr["Remarks"].ToString();
                    employeePresentAddressBanglaVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeePresentAddressBanglaVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeePresentAddressBanglaVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeePresentAddressBanglaVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeePresentAddressBanglaVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeePresentAddressBanglaVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeePresentAddressBanglaVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeePresentAddressBanglaVMs.Add(employeePresentAddressBanglaVM);
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

            return employeePresentAddressBanglaVMs;
        }
        //==================SelectByID=================
        public EmployeePresentAddressBanglaVM SelectByEmployeeId(string EmployeeId )
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePresentAddressBanglaVM employeePresentAddressBanglaVM = new EmployeePresentAddressBanglaVM();

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
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeePresentAddressBangla
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
                    employeePresentAddressBanglaVM.Id = Convert.ToInt32(dr["Id"]);
                    employeePresentAddressBanglaVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeePresentAddressBanglaVM.Address = dr["Address"].ToString();
                    employeePresentAddressBanglaVM.District = dr["District"].ToString();
                    employeePresentAddressBanglaVM.Division = dr["Division"].ToString();
                    employeePresentAddressBanglaVM.Country = dr["Country"].ToString();
                    employeePresentAddressBanglaVM.City = dr["City"].ToString();
                    employeePresentAddressBanglaVM.Remarks = dr["Remarks"].ToString();
                    employeePresentAddressBanglaVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeePresentAddressBanglaVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeePresentAddressBanglaVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeePresentAddressBanglaVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeePresentAddressBanglaVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeePresentAddressBanglaVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeePresentAddressBanglaVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeePresentAddressBanglaVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeePresentAddressBanglaVM employeePresentAddressBanglaVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeePresentAddressBangla"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;


            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(employeePresentAddressBanglaVM.Name))
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
                //cmdExist.Parameters.AddWithValue("@EmployeeId", employeePresentAddressBanglaVM.EmployeeId);
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
                    sqlText += @" INSERT INTO EmployeePresentAddressBangla(	EmployeeId,Address,District,Division,Country
                                ,City,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@EmployeeId,@Address,@District,@Division,@Country,@City
                                        ,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@EmployeeId", employeePresentAddressBanglaVM.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Address", employeePresentAddressBanglaVM.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@District", employeePresentAddressBanglaVM.District ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Division", employeePresentAddressBanglaVM.Division ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Country", employeePresentAddressBanglaVM.Country ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@City", employeePresentAddressBanglaVM.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", employeePresentAddressBanglaVM.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", employeePresentAddressBanglaVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", employeePresentAddressBanglaVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", employeePresentAddressBanglaVM.CreatedFrom);

                    cmdInsert.Transaction = transaction;
					var exeRes = cmdInsert.ExecuteScalar();
					Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Present Address Bangla Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Present Address Bangla Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Present Address Bangla already used";
                    throw new ArgumentNullException("Please Input Employee Present Address Bangla Value", "");
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
        public string[] Update(EmployeePresentAddressBanglaVM employeePresentAddressBanglaVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Present Address Bangla Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToPresentAddressBangla"); }

                #endregion open connection and transaction
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeNominee ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", employeePresentAddressBanglaVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", employeePresentAddressBanglaVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", employeePresentAddressBanglaVM.Name);

                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This Nominee already used";
                //    throw new ArgumentNullException("Please Input Nominee Value", "");
                //}
                //#endregion Exist

                if (employeePresentAddressBanglaVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeePresentAddressBangla set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " Address=@Address,";
                    sqlText += " District=@District,";
                    sqlText += " Division=@Division,";
                    sqlText += " Country=@Country,";
                    sqlText += " City=@City,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", employeePresentAddressBanglaVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", employeePresentAddressBanglaVM.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@Address", employeePresentAddressBanglaVM.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@District", employeePresentAddressBanglaVM.District ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Division", employeePresentAddressBanglaVM.Division ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Country", employeePresentAddressBanglaVM.Country ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", employeePresentAddressBanglaVM.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", employeePresentAddressBanglaVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeePresentAddressBanglaVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeePresentAddressBanglaVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeePresentAddressBanglaVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = employeePresentAddressBanglaVM.Id.ToString();// Return Id
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
                    throw new ArgumentNullException("Present Address Bangla Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Present Address Bangla.";
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
        public EmployeePresentAddressBanglaVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeePresentAddressBanglaVM employeePresentAddressBanglaVM = new EmployeePresentAddressBanglaVM();

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
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeePresentAddressBangla 
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
                        employeePresentAddressBanglaVM.Id = Convert.ToInt32(dr["Id"]);
                        employeePresentAddressBanglaVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeePresentAddressBanglaVM.Address = dr["Address"].ToString();
                        employeePresentAddressBanglaVM.District = dr["District"].ToString();
                        employeePresentAddressBanglaVM.Division = dr["Division"].ToString();
                        employeePresentAddressBanglaVM.Country = dr["Country"].ToString();
                        employeePresentAddressBanglaVM.City = dr["City"].ToString();
                        employeePresentAddressBanglaVM.Remarks = dr["Remarks"].ToString();
                        employeePresentAddressBanglaVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeePresentAddressBanglaVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeePresentAddressBanglaVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeePresentAddressBanglaVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeePresentAddressBanglaVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeePresentAddressBanglaVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeePresentAddressBanglaVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return employeePresentAddressBanglaVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeePresentAddressBanglaVM EmployeePresentAddressBanglaVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToPresentAddressBangla"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (EmployeePresentAddressBanglaVM.Id > 0)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeePresentAddressBangla set";
                    sqlText += " IsArchive=@IsArchive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", EmployeePresentAddressBanglaVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeePresentAddressBanglaVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeePresentAddressBanglaVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeePresentAddressBanglaVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = EmployeePresentAddressBanglaVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Present Address Bangla Delete", EmployeePresentAddressBanglaVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Present Address Bangla Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Present Address Bangla.";
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
