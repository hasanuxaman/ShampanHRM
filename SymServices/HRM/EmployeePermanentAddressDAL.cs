using SymOrdinary;
using SymServices.Common;
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
    public class EmployeePermanentAddressDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        #region Methods
       
        public List<EmployeePermanentAddressVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePermanentAddressVM> employeePermanentAddressVMs = new List<EmployeePermanentAddressVM>();
            EmployeePermanentAddressVM employeePermanentAddressVM;
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
    From EmployeePermanentAddress
Where IsArchive=0
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeePermanentAddressVM = new EmployeePermanentAddressVM();
                    employeePermanentAddressVM.Id = Convert.ToInt32(dr["Id"]);
                    employeePermanentAddressVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeePermanentAddressVM.Address = dr["Address"].ToString();
                    employeePermanentAddressVM.District = dr["District"].ToString();
                    employeePermanentAddressVM.Division = dr["Division"].ToString();
                    employeePermanentAddressVM.Country = dr["Country"].ToString();
                    employeePermanentAddressVM.City = dr["City"].ToString();
                    employeePermanentAddressVM.PostalCode = dr["PostalCode"].ToString();
                    employeePermanentAddressVM.Phone = dr["Phone"].ToString();
                    employeePermanentAddressVM.Mobile = dr["Mobile"].ToString();
                    employeePermanentAddressVM.FileName = dr["FileName"].ToString();
                    
                    employeePermanentAddressVM.Fax = dr["Fax"].ToString();
                    employeePermanentAddressVM.Remarks = dr["Remarks"].ToString();
                    employeePermanentAddressVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeePermanentAddressVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeePermanentAddressVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeePermanentAddressVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeePermanentAddressVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeePermanentAddressVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeePermanentAddressVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeePermanentAddressVMs.Add(employeePermanentAddressVM);
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

            return employeePermanentAddressVMs;
        }

        //==================SelectByID=================
        public EmployeePermanentAddressVM SelectByEmployeeId(string EmployeeId )
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePermanentAddressVM employeePermanentAddressVM = new EmployeePermanentAddressVM();

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
    From EmployeePermanentAddress
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
                    employeePermanentAddressVM.Id = Convert.ToInt32(dr["Id"]);
                    employeePermanentAddressVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeePermanentAddressVM.Address = dr["Address"].ToString();
                    employeePermanentAddressVM.District = dr["District"].ToString();
                    employeePermanentAddressVM.Division = dr["Division"].ToString();
                    employeePermanentAddressVM.Country = dr["Country"].ToString();
                    employeePermanentAddressVM.City = dr["City"].ToString();
                    employeePermanentAddressVM.PostalCode = dr["PostalCode"].ToString();
                    employeePermanentAddressVM.PostOffice = dr["PostOffice"].ToString();
                    employeePermanentAddressVM.Phone = dr["Phone"].ToString();
                    employeePermanentAddressVM.Fax = dr["Fax"].ToString();
                    employeePermanentAddressVM.Mobile = dr["Mobile"].ToString();
                    employeePermanentAddressVM.FileName = dr["FileName"].ToString();
                    employeePermanentAddressVM.Remarks = dr["Remarks"].ToString();
                    employeePermanentAddressVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeePermanentAddressVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeePermanentAddressVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeePermanentAddressVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeePermanentAddressVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeePermanentAddressVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeePermanentAddressVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeePermanentAddressVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeePermanentAddressVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeePermanentAddress"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(employeePermanentAddressVM.Name))
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

                bool IsExist = false;
                IsExist = _cDal.ExistCheck("EmployeePermanentAddress","EmployeeId",vm.EmployeeId, currConn, transaction);
                if (IsExist)
                {
                    retResults[1] = "Data Already being Inserted!";
                    throw new ArgumentNullException(retResults[1], "");
                    
                }
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeNominee ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", employeePermanentAddressVM.EmployeeId);
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
                if (vm != null)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeePermanentAddress(	EmployeeId,Address,District,Division,Country
                                ,City,PostOffice,PostalCode,Phone,Fax,Mobile,FileName,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@EmployeeId,@Address,@District,@Division,@Country,@City,@PostOffice,@PostalCode,@Phone,@Fax,@Mobile,@FileName
                                        ,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@District", vm.District ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Division", vm.Division ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PostalCode", vm.PostalCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PostOffice", vm.PostOffice ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Phone", vm.Phone ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Fax", vm.Fax ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Mobile", vm.Mobile ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    cmdInsert.Transaction = transaction;
					var exeRes = cmdInsert.ExecuteScalar();
					Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Permanent Address  Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Permanent Address  Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Permanent Address Bangla already used";
                    throw new ArgumentNullException("Please Input Employee Permanent Address  Value", "");
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
        public string[] Update(EmployeePermanentAddressVM employeePermanentAddressVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Permanent Address  Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToPermanentAddress"); }

                #endregion open connection and transaction
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeNominee ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", employeePermanentAddressVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", employeePermanentAddressVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", employeePermanentAddressVM.Name);

                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This Nominee already used";
                //    throw new ArgumentNullException("Please Input Nominee Value", "");
                //}
                //#endregion Exist

                if (employeePermanentAddressVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeePermanentAddress set";
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
                    if (employeePermanentAddressVM.FileName != null)
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
                    cmdUpdate.Parameters.AddWithValue("@Id", employeePermanentAddressVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", employeePermanentAddressVM.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@Address", employeePermanentAddressVM.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@District", employeePermanentAddressVM.District ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Division", employeePermanentAddressVM.Division ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Country", employeePermanentAddressVM.Country ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", employeePermanentAddressVM.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostalCode", employeePermanentAddressVM.PostalCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostOffice", employeePermanentAddressVM.PostOffice ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Phone", employeePermanentAddressVM.Phone ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Fax", employeePermanentAddressVM.Fax ?? Convert.DBNull);
                    if (employeePermanentAddressVM.FileName !=null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", employeePermanentAddressVM.FileName ?? Convert.DBNull);
                    }
                    cmdUpdate.Parameters.AddWithValue("@Mobile", employeePermanentAddressVM.Mobile ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", employeePermanentAddressVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeePermanentAddressVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeePermanentAddressVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeePermanentAddressVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = employeePermanentAddressVM.Id.ToString();// Return Id
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
                    throw new ArgumentNullException("Permanent Address  Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Permanent Address .";
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
        public EmployeePermanentAddressVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeePermanentAddressVM employeePermanentAddressVM = new EmployeePermanentAddressVM();

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
    From EmployeePermanentAddress 
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
                        employeePermanentAddressVM.Id = Convert.ToInt32(dr["Id"]);
                        employeePermanentAddressVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeePermanentAddressVM.Address = dr["Address"].ToString();
                        employeePermanentAddressVM.District = dr["District"].ToString();
                        employeePermanentAddressVM.Division = dr["Division"].ToString();
                        employeePermanentAddressVM.Country = dr["Country"].ToString();
                        employeePermanentAddressVM.City = dr["City"].ToString();
                        employeePermanentAddressVM.PostalCode = dr["PostalCode"].ToString();
                        employeePermanentAddressVM.Phone = dr["Phone"].ToString();
                        employeePermanentAddressVM.Fax = dr["Fax"].ToString();
                        employeePermanentAddressVM.Mobile = dr["Mobile"].ToString();
                        employeePermanentAddressVM.FileName = dr["FileName"].ToString();
                        employeePermanentAddressVM.Remarks = dr["Remarks"].ToString();
                        employeePermanentAddressVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeePermanentAddressVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeePermanentAddressVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeePermanentAddressVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeePermanentAddressVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeePermanentAddressVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeePermanentAddressVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return employeePermanentAddressVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeePermanentAddressVM EmployeePermanentAddressVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeletePermanent Address Bangla"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToPermanentAddress"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (EmployeePermanentAddressVM.Id > 0)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeePermanentAddress set";
                    sqlText += " IsArchive=@IsArchive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", EmployeePermanentAddressVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeePermanentAddressVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeePermanentAddressVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeePermanentAddressVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = EmployeePermanentAddressVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Permanent Address Delete", EmployeePermanentAddressVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Permanent Address  Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Permanent Address .";
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
        public List<EmployeePermanentAddressVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePermanentAddressVM> VMs = new List<EmployeePermanentAddressVM>();
            EmployeePermanentAddressVM vm;
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
 isnull(PerAd.Id, 0) Id
,ei.EmployeeId
,isnull(PerAd.[Address]	, 'NA')	[Address]
,isnull(PerAd.District	, 'NA')	District
,isnull(PerAd.Division	, 'NA')	Division
,isnull(PerAd.Country	, 'NA')	Country
,isnull(PerAd.City		, 'NA')	City
,isnull(PerAd.PostalCode, 'NA')	PostalCode
,isnull(PerAd.Phone		, 'NA')	Phone
,isnull(PerAd.Fax		, 'NA')	Fax
,isnull(PerAd.Mobile	, 'NA')	Mobile
,isnull(PerAd.[FileName]      , 'NA')     [FileName]
,isnull(PerAd.Remarks         , 'NA')     Remarks
,isnull(PerAd.IsActive        , 0)     IsActive
,isnull(PerAd.IsArchive       , 0)     IsArchive
,isnull(PerAd.CreatedBy       , 'NA')     CreatedBy
,isnull(PerAd.CreatedAt       , 'NA')     CreatedAt
,isnull(PerAd.CreatedFrom     , 'NA')     CreatedFrom
,isnull(PerAd.LastUpdateBy    , 'NA')     LastUpdateBy
,isnull(PerAd.LastUpdateAt    , 'NA')     LastUpdateAt
,isnull(PerAd.LastUpdateFrom  , 'NA')     LastUpdateFrom

    From ViewEmployeeInformation ei 
	left outer join EmployeePermanentAddress PerAd  on ei.EmployeeId=PerAd.EmployeeId
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
                    vm = new EmployeePermanentAddressVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.District = dr["District"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.PostalCode = dr["PostalCode"].ToString();
                    vm.Phone = dr["Phone"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.FileName = dr["FileName"].ToString();

                    vm.Fax = dr["Fax"].ToString();
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

#region Unused Methods
////==================SelectPresentAddress=================
        //public EmployeePermanentAddressVM SelectPresentAddress(string EmployeeId = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        //{
        //    #region Variables
        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
        //    string sqlText = "";
        //    EmployeePermanentAddressVM vm = new EmployeePermanentAddressVM();
        //    #endregion
        //    try
        //    {
        //        EmployeePresentAddressVM employeePresentAddressVM = new EmployeePresentAddressVM();
        //        EmployeePresentAddressDAL _employeePresentAddressDAL = new EmployeePresentAddressDAL();
        //        employeePresentAddressVM = _employeePresentAddressDAL.SelectByEmployeeId(EmployeeId);

        //        vm.EmployeeId = employeePresentAddressVM.EmployeeId;
        //        vm.Address = employeePresentAddressVM.Address;
        //        vm.District = employeePresentAddressVM.District;
        //        vm.Division = employeePresentAddressVM.Division;
        //        vm.Country = employeePresentAddressVM.Country;
        //        vm.City = employeePresentAddressVM.City;
        //        vm.PostalCode = employeePresentAddressVM.PostalCode;
        //        vm.Phone = employeePresentAddressVM.Phone;
        //        vm.Fax = employeePresentAddressVM.Fax;
        //        vm.Mobile = employeePresentAddressVM.Mobile;
        //        vm.FileName = employeePresentAddressVM.FileName;
        //        vm.Remarks = employeePresentAddressVM.Remarks;
        //    }
        //    #region catch
        //    catch (Exception ex)
        //    {
        //        throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
        //    }
        //    #endregion
        //    #region finally
        //    finally
        //    {
        //    }
        //    #endregion
        //    return vm;
        //}

        //==================SelectAll=================
#endregion Unused Methods
