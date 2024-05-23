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
    public class EmployeeReferenceDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeReferenceVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeReferenceVM> employeeReferenceVMs = new List<EmployeeReferenceVM>();
            EmployeeReferenceVM employeeReferenceVM;
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
,Name
,Relation
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Mobile
,Designation
,WorkAddress
,IsNull(YearOfAcquaintance, 0)YearOfAcquaintance
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
    From EmployeeReference
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
                    employeeReferenceVM = new EmployeeReferenceVM();
                    employeeReferenceVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeReferenceVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeReferenceVM.Name = dr["Name"].ToString();
                    employeeReferenceVM.Relation = dr["Relation"].ToString();
                    employeeReferenceVM.Address = dr["Address"].ToString();
                    employeeReferenceVM.District = dr["District"].ToString();
                    employeeReferenceVM.Division = dr["Division"].ToString();
                    employeeReferenceVM.Country = dr["Country"].ToString();
                    employeeReferenceVM.City = dr["City"].ToString();
                    employeeReferenceVM.PostalCode = dr["PostalCode"].ToString();
                    employeeReferenceVM.Phone = dr["Phone"].ToString();
                    employeeReferenceVM.Mobile = dr["Mobile"].ToString();
                    
                    employeeReferenceVM.Designation = dr["Designation"].ToString();
                    employeeReferenceVM.WorkAddress = dr["WorkAddress"].ToString();
                    employeeReferenceVM.YearOfAcquaintance = Convert.ToInt32(dr["YearOfAcquaintance"]);
                    employeeReferenceVM.Remarks = dr["Remarks"].ToString();
                    employeeReferenceVM.FileName = dr["FileName"].ToString();
                    employeeReferenceVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeReferenceVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeReferenceVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeReferenceVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeReferenceVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeReferenceVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeReferenceVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeReferenceVMs.Add(employeeReferenceVM);
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

            return employeeReferenceVMs;
        }
        //==================SelectAll=================
        public List<EmployeeReferenceVM> SelectAllByEmployee(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeReferenceVM> employeeReferenceVMs = new List<EmployeeReferenceVM>();
            EmployeeReferenceVM employeeReferenceVM;
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
,Name
,Relation
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Mobile
,FileName
,Designation
,WorkAddress
,IsNull(YearOfAcquaintance, 0)YearOfAcquaintance
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeReference
Where IsArchive=0 and EmployeeId=@EmployeeId
    ORDER BY Name
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
                    employeeReferenceVM = new EmployeeReferenceVM();
                    employeeReferenceVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeReferenceVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeReferenceVM.Name = dr["Name"].ToString();
                    employeeReferenceVM.Relation = dr["Relation"].ToString();
                    employeeReferenceVM.Address = dr["Address"].ToString();
                    employeeReferenceVM.District = dr["District"].ToString();
                    employeeReferenceVM.Division = dr["Division"].ToString();
                    employeeReferenceVM.Country = dr["Country"].ToString();
                    employeeReferenceVM.City = dr["City"].ToString();
                    employeeReferenceVM.PostalCode = dr["PostalCode"].ToString();
                    employeeReferenceVM.Phone = dr["Phone"].ToString();
                    employeeReferenceVM.Mobile = dr["Mobile"].ToString();
                    employeeReferenceVM.FileName = dr["FileName"].ToString();
                    employeeReferenceVM.Designation = dr["Designation"].ToString();
                    employeeReferenceVM.WorkAddress = dr["WorkAddress"].ToString();
                    employeeReferenceVM.YearOfAcquaintance = Convert.ToInt32(dr["YearOfAcquaintance"]);
                    
                    
                    
                    employeeReferenceVM.Remarks = dr["Remarks"].ToString();
                    employeeReferenceVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeReferenceVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeReferenceVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeReferenceVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeReferenceVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeReferenceVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeReferenceVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeReferenceVMs.Add(employeeReferenceVM);
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

            return employeeReferenceVMs;
        }
        //==================SelectByID=================
        public EmployeeReferenceVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeReferenceVM employeeReferenceVM = new EmployeeReferenceVM();

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
,Name
,Relation
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Mobile
,Designation
,WorkAddress
,IsNull(YearOfAcquaintance, 0)YearOfAcquaintance
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
    From EmployeeReference
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
                    employeeReferenceVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeReferenceVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeReferenceVM.Name = dr["Name"].ToString();
                    employeeReferenceVM.Relation = dr["Relation"].ToString();
                    employeeReferenceVM.Address = dr["Address"].ToString();
                    employeeReferenceVM.District = dr["District"].ToString();
                    employeeReferenceVM.Division = dr["Division"].ToString();
                    employeeReferenceVM.Country = dr["Country"].ToString();
                    employeeReferenceVM.City = dr["City"].ToString();
                    employeeReferenceVM.PostalCode = dr["PostalCode"].ToString();
                    employeeReferenceVM.Phone = dr["Phone"].ToString();
                    employeeReferenceVM.Mobile = dr["Mobile"].ToString();
                    employeeReferenceVM.Designation = dr["Designation"].ToString();
                    employeeReferenceVM.WorkAddress = dr["WorkAddress"].ToString();
                    employeeReferenceVM.YearOfAcquaintance = Convert.ToInt32(dr["YearOfAcquaintance"]);
                    
                    
                    
                    employeeReferenceVM.Remarks = dr["Remarks"].ToString();
                    employeeReferenceVM.FileName = dr["FileName"].ToString();
                    employeeReferenceVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeReferenceVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeReferenceVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeReferenceVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeReferenceVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeReferenceVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeReferenceVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeeReferenceVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeeReferenceVM employeeReferenceVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeReference"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(employeeReferenceVM.Name))
                {
                    retResults[1] = "Please Input Employee Reference Name";
                    return retResults;
                }
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
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeReference ";
                sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", employeeReferenceVM.EmployeeId);
                cmdExist.Parameters.AddWithValue("@Name", employeeReferenceVM.Name);
                object objfoundId = cmdExist.ExecuteScalar();

                if (objfoundId == null)
                {
                    retResults[1] = "Please Input Employee Reference Value";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Input Employee Reference Value", "");
                }
                #endregion Exist
                #region Save

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {

                    sqlText = "  ";
sqlText += @" INSERT INTO EmployeeReference(EmployeeId,Name,Relation,Address,District,Division,Country
,City,PostalCode,Phone,Mobile
,Designation
,WorkAddress
,YearOfAcquaintance
,Remarks,FileName,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
VALUES (@EmployeeId,@Name,@Relation,@Address,@District,@Division,@Country,@City,@PostalCode,@Phone
,@Mobile
,@Designation
,@WorkAddress
,@YearOfAcquaintance 
,@Remarks,@FileName,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@EmployeeId", employeeReferenceVM.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Name", employeeReferenceVM.Name ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Relation", employeeReferenceVM.Relation ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Address", employeeReferenceVM.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@District", employeeReferenceVM.District ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Division", employeeReferenceVM.Division ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Country", employeeReferenceVM.Country ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@City", employeeReferenceVM.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PostalCode", employeeReferenceVM.PostalCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Phone", employeeReferenceVM.Phone ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Mobile", employeeReferenceVM.Mobile ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FileName", employeeReferenceVM.FileName ?? Convert.DBNull);

                    
                    
                    
                    cmdInsert.Parameters.AddWithValue("@Designation", employeeReferenceVM.Designation ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@WorkAddress", employeeReferenceVM.WorkAddress ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@YearOfAcquaintance", employeeReferenceVM.YearOfAcquaintance);
                    cmdInsert.Parameters.AddWithValue("@Remarks", employeeReferenceVM.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", employeeReferenceVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", employeeReferenceVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", employeeReferenceVM.CreatedFrom);

                    cmdInsert.Transaction = transaction;
					var exeRes = cmdInsert.ExecuteScalar();
					Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Reference Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Reference Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Reference already used!";
                    throw new ArgumentNullException("This Employee Reference already used!", "");
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
        public string[] Update(EmployeeReferenceVM employeeReferenceVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Reference Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToReference"); }

                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeReference ";
                sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name AND Id<>@Id";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Id", employeeReferenceVM.Id);
                cmdExist.Parameters.AddWithValue("@EmployeeId", employeeReferenceVM.EmployeeId);
                cmdExist.Parameters.AddWithValue("@Name", employeeReferenceVM.Name);

                
					var exeRes = cmdExist.ExecuteScalar();
					int exists = Convert.ToInt32(exeRes);

                    if (exists > 0)
                {
                    retResults[1] = "This Reference already used";
                    throw new ArgumentNullException("Please Input Reference Value", "");
                }
                #endregion Exist

                if (employeeReferenceVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeReference set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " Name=@Name,";
                    sqlText += " Relation=@Relation,";
                    sqlText += " Address=@Address,";
                    sqlText += " District=@District,";
                    sqlText += " Division=@Division,";
                    sqlText += " Country=@Country,";
                    sqlText += " City=@City,";
                    sqlText += " PostalCode=@PostalCode,";
                    sqlText += " Phone=@Phone,";
                    sqlText += " Mobile=@Mobile,";
                    if (employeeReferenceVM.FileName !=null)
                    {
                        sqlText += " FileName=@FileName,";
                    }
                    
                    
                    
                    sqlText += " Designation=@Designation,";
                    sqlText += " WorkAddress=@WorkAddress,";
                    sqlText += " YearOfAcquaintance=@YearOfAcquaintance,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", employeeReferenceVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", employeeReferenceVM.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@Name", employeeReferenceVM.Name );
                    cmdUpdate.Parameters.AddWithValue("@Relation", employeeReferenceVM.Relation );
                    cmdUpdate.Parameters.AddWithValue("@Address", employeeReferenceVM.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@District", employeeReferenceVM.District ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Division", employeeReferenceVM.Division ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Country", employeeReferenceVM.Country ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", employeeReferenceVM.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostalCode", employeeReferenceVM.PostalCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Phone", employeeReferenceVM.Phone ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Mobile", employeeReferenceVM.Mobile ?? Convert.DBNull);
                    if (employeeReferenceVM.FileName != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", employeeReferenceVM.FileName ?? Convert.DBNull);
                    }
                    
                    
                    
                    cmdUpdate.Parameters.AddWithValue("@Designation", employeeReferenceVM.Designation ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@WorkAddress", employeeReferenceVM.WorkAddress ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@YearOfAcquaintance", employeeReferenceVM.YearOfAcquaintance);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", employeeReferenceVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeReferenceVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeReferenceVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeReferenceVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = employeeReferenceVM.Id.ToString();// Return Id
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
                    throw new ArgumentNullException("Reference Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Reference.";
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
        public EmployeeReferenceVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeeReferenceVM employeeReferenceVM = new EmployeeReferenceVM();

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
,Name
,Relation
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
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
    From EmployeeReference   
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
                        employeeReferenceVM.Id = Convert.ToInt32(dr["Id"]);
                        employeeReferenceVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeeReferenceVM.Name = dr["Name"].ToString();
                        employeeReferenceVM.Relation = dr["Relation"].ToString();
                        employeeReferenceVM.Address = dr["Address"].ToString();
                        employeeReferenceVM.District = dr["District"].ToString();
                        employeeReferenceVM.Division = dr["Division"].ToString();
                        employeeReferenceVM.Country = dr["Country"].ToString();
                        employeeReferenceVM.City = dr["City"].ToString();
                        employeeReferenceVM.PostalCode = dr["PostalCode"].ToString();
                        employeeReferenceVM.Phone = dr["Phone"].ToString();
                        employeeReferenceVM.FileName = dr["FileName"].ToString();
                        employeeReferenceVM.Remarks = dr["Remarks"].ToString();
                        employeeReferenceVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeeReferenceVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeeReferenceVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeeReferenceVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeeReferenceVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeeReferenceVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeeReferenceVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return employeeReferenceVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeeReferenceVM EmployeeReferenceVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteReference"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToReference"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length>=1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeReference set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeeReferenceVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeeReferenceVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeeReferenceVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Reference Delete", EmployeeReferenceVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Reference Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Reference Information.";
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
        //==================SelectAll=================
        public List<EmployeeReferenceVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeReferenceVM> employeeReferenceVMs = new List<EmployeeReferenceVM>();
            EmployeeReferenceVM employeeReferenceVM;
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
isnull(Ref.Id, 0) Id
,ei.EmployeeId
,isnull(Ref.Name			, 'NA')		 Name
,isnull(Ref.Relation		, 'NA')		 Relation
,isnull(Ref.[Address]		, 'NA')		 [Address]
,isnull(Ref.District		, 'NA')		 District
,isnull(Ref.Division		, 'NA')		 Division
,isnull(Ref.Country			, 'NA')	 Country
,isnull(Ref.City			, 'NA')		 City
,isnull(Ref.PostalCode		, 'NA')		 PostalCode
,isnull(Ref.Phone			, 'NA')		 Phone
,isnull(Ref.Mobile			, 'NA')		 Mobile
,Ref.Designation
,Ref.WorkAddress
,IsNull(Ref.YearOfAcquaintance, 0)YearOfAcquaintance
,isnull(Ref.Remarks			, 'NA')	 Remarks
,isnull(Ref.[FileName]		, 'NA')		 [FileName]
,isnull(Ref.IsActive		, 0)		 IsActive
,isnull(Ref.IsArchive		, 0)		 IsArchive
,isnull(Ref.CreatedBy		, 'NA')		 CreatedBy
,isnull(Ref.CreatedAt		, 'NA')		 CreatedAt
,isnull(Ref.CreatedFrom		, 'NA')	 CreatedFrom
,isnull(Ref.LastUpdateBy	, 'NA')		 LastUpdateBy
,isnull(Ref.LastUpdateAt	, 'NA')		 LastUpdateAt
,isnull(Ref.LastUpdateFrom	, 'NA')		 LastUpdateFrom

    From ViewEmployeeInformation ei
		left outer join EmployeeReference Ref on ei.EmployeeId=Ref.EmployeeId
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
                    employeeReferenceVM = new EmployeeReferenceVM();
                    employeeReferenceVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeReferenceVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeReferenceVM.Name = dr["Name"].ToString();
                    employeeReferenceVM.Relation = dr["Relation"].ToString();
                    employeeReferenceVM.Address = dr["Address"].ToString();
                    employeeReferenceVM.District = dr["District"].ToString();
                    employeeReferenceVM.Division = dr["Division"].ToString();
                    employeeReferenceVM.Country = dr["Country"].ToString();
                    employeeReferenceVM.City = dr["City"].ToString();
                    employeeReferenceVM.PostalCode = dr["PostalCode"].ToString();
                    employeeReferenceVM.Phone = dr["Phone"].ToString();
                    employeeReferenceVM.Mobile = dr["Mobile"].ToString();
                    employeeReferenceVM.Designation = dr["Designation"].ToString();
                    employeeReferenceVM.WorkAddress = dr["WorkAddress"].ToString();
                    employeeReferenceVM.YearOfAcquaintance = Convert.ToInt32(dr["YearOfAcquaintance"]);
                    
                    
                    employeeReferenceVM.Remarks = dr["Remarks"].ToString();
                    employeeReferenceVM.FileName = dr["FileName"].ToString();
                    employeeReferenceVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeReferenceVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeReferenceVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeReferenceVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeReferenceVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeReferenceVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeReferenceVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeReferenceVMs.Add(employeeReferenceVM);
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

            return employeeReferenceVMs;
        }



        #endregion
    }
}
