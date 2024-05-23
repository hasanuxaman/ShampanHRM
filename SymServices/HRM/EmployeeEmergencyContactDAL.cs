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
    public class EmployeeEmergencyContactDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeEmergencyContactVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeEmergencyContactVM> VMs = new List<EmployeeEmergencyContactVM>();
            EmployeeEmergencyContactVM vm;
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
,Name
,Relation
,Address
,District
,Division
,Country
,City
,PostalCode
,IsNull(PostOffice, '')PostOffice
,Phone
,Mobile
,Fax
,FileName
,Email
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeEmergencyContact
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
                    vm = new EmployeeEmergencyContactVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();

                    vm.Name = dr["Name"].ToString();
                    vm.Relation = dr["Relation"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.PostalCode = dr["PostalCode"].ToString();
                    vm.PostOffice = dr["PostOffice"].ToString();
                    vm.Phone = dr["Phone"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Fax = dr["Fax"].ToString();
                    vm.FileName = dr["FileName"].ToString();

                    
                    vm.Email = dr["Email"].ToString();
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
        //==================SelectByID=================
        public EmployeeEmergencyContactVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeEmergencyContactVM employeeEmergencyContactVM = new EmployeeEmergencyContactVM();

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
,IsNull(PostOffice, '')PostOffice
,Phone
,Mobile
,Fax
,FileName
,Email
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeEmergencyContact
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
                    employeeEmergencyContactVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeEmergencyContactVM.EmployeeId = dr["EmployeeId"].ToString();

                    employeeEmergencyContactVM.Name = dr["Name"].ToString();
                    employeeEmergencyContactVM.Relation = dr["Relation"].ToString();
                    employeeEmergencyContactVM.Address = dr["Address"].ToString();
                    employeeEmergencyContactVM.District = dr["District"].ToString();
                    employeeEmergencyContactVM.Division = dr["Division"].ToString();
                    employeeEmergencyContactVM.Country = dr["Country"].ToString();
                    employeeEmergencyContactVM.City = dr["City"].ToString();
                    employeeEmergencyContactVM.PostalCode = dr["PostalCode"].ToString();
                    employeeEmergencyContactVM.PostOffice = dr["PostOffice"].ToString();
                    employeeEmergencyContactVM.Phone = dr["Phone"].ToString();
                    employeeEmergencyContactVM.Mobile = dr["Mobile"].ToString();
                    employeeEmergencyContactVM.Fax = dr["Fax"].ToString();
                    employeeEmergencyContactVM.FileName = dr["FileName"].ToString();
                    
                    employeeEmergencyContactVM.Email = dr["Email"].ToString();
                    employeeEmergencyContactVM.Remarks = dr["Remarks"].ToString();
                    employeeEmergencyContactVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeEmergencyContactVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeEmergencyContactVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeEmergencyContactVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeEmergencyContactVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeEmergencyContactVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeEmergencyContactVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeeEmergencyContactVM;
        }
        //==================SelectBy EmployeeId=================
        public EmployeeEmergencyContactVM SelectByEmployeeId(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeEmergencyContactVM employeeEmergencyContactVM = new EmployeeEmergencyContactVM();

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
,IsNull(PostOffice, '')PostOffice
,Phone
,Mobile
,Fax
,FileName
,Email
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeEmergencyContact
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
                    employeeEmergencyContactVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeEmergencyContactVM.EmployeeId = dr["EmployeeId"].ToString();

                    employeeEmergencyContactVM.Name = dr["Name"].ToString();
                    employeeEmergencyContactVM.Relation = dr["Relation"].ToString();
                    employeeEmergencyContactVM.Address = dr["Address"].ToString();
                    employeeEmergencyContactVM.District = dr["District"].ToString();
                    employeeEmergencyContactVM.Division = dr["Division"].ToString();
                    employeeEmergencyContactVM.Country = dr["Country"].ToString();
                    employeeEmergencyContactVM.City = dr["City"].ToString();
                    employeeEmergencyContactVM.PostalCode = dr["PostalCode"].ToString();
                    employeeEmergencyContactVM.PostOffice = dr["PostOffice"].ToString();
                    employeeEmergencyContactVM.Phone = dr["Phone"].ToString();
                    employeeEmergencyContactVM.Mobile = dr["Mobile"].ToString();
                    employeeEmergencyContactVM.Fax = dr["Fax"].ToString();
                    employeeEmergencyContactVM.FileName = dr["FileName"].ToString();
                    
                    employeeEmergencyContactVM.Email = dr["Email"].ToString();
                    employeeEmergencyContactVM.Remarks = dr["Remarks"].ToString();
                    employeeEmergencyContactVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeEmergencyContactVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeEmergencyContactVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeEmergencyContactVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeEmergencyContactVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeEmergencyContactVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeEmergencyContactVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeeEmergencyContactVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeeEmergencyContactVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmergencyContact"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;


            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(employeeEmergencyContactVM))
                //{
                //    retResults[1] = "Please Input Employee Degree_E";
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
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeEmergencyContact ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Id<0";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Emergency Contact Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Emergency Contact Value", "");
                //}
                //#endregion Exist
                #region Save

                //int foundId = (int)objfoundId;
                //if (foundId <= 0)
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeEmergencyContact(	
EmployeeId,Name,Relation,Address,District,Division,Country,City,PostalCode,PostOffice,Phone,Mobile,Fax,FileName
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom

) VALUES (
@EmployeeId,@Name,@Relation,@Address,@District,@Division,@Country,@City,@PostalCode,@PostOffice,@Phone,@Mobile,@Fax,@FileName

,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                    cmdInsert.Parameters.AddWithValue("@Relation", vm.Relation );
                    cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@District", vm.District ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Division", vm.Division ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PostalCode", vm.PostalCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PostOffice", vm.PostOffice ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Phone", vm.Phone ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Mobile", vm.Mobile ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Fax", vm.Fax ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);

                    
                    //cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
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
                        retResults[1] = "Please Input Employee Emergency Contact Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Emergency Contact Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Emergency Contact already used";
                    throw new ArgumentNullException("Please Input Employee Emergency Contact Value", "");
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
        public string[] Update(EmployeeEmergencyContactVM employeeEmergencyContactVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Emergency ContactUpdate"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmergencyContactUpdate"); }

                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeEmergencyContact ";
                sqlText += " WHERE EmployeeId=@EmployeeId  AND Id<>@Id";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Id", employeeEmergencyContactVM.Id);
                cmdExist.Parameters.AddWithValue("@EmployeeId", employeeEmergencyContactVM.EmployeeId);
                
				var exeRes = cmdExist.ExecuteScalar();
				int exists = Convert.ToInt32(exeRes);
                if (exists > 0)
                {
                    retResults[1] = "This Emergency Contact already used";
                    throw new ArgumentNullException("Please Input Emergency Contact Value", "");
                }
                #endregion Exist

                if (employeeEmergencyContactVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeEmergencyContact set";
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
                    sqlText += " Fax=@Fax,";
                    if (employeeEmergencyContactVM.FileName !=null)
                    {
                        sqlText += " FileName=@FileName,";
                    }
                    
                    sqlText += " Email=@Email,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", employeeEmergencyContactVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", employeeEmergencyContactVM.EmployeeId);

                    cmdUpdate.Parameters.AddWithValue("@Name", employeeEmergencyContactVM.Name );
                    cmdUpdate.Parameters.AddWithValue("@Relation", employeeEmergencyContactVM.Relation );
                    cmdUpdate.Parameters.AddWithValue("@Address", employeeEmergencyContactVM.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@District", employeeEmergencyContactVM.District ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Division", employeeEmergencyContactVM.Division ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Country", employeeEmergencyContactVM.Country ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", employeeEmergencyContactVM.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostalCode", employeeEmergencyContactVM.PostalCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Phone", employeeEmergencyContactVM.Phone ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Mobile", employeeEmergencyContactVM.Mobile ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Fax", employeeEmergencyContactVM.Fax ?? Convert.DBNull);
                    if (employeeEmergencyContactVM.FileName != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", employeeEmergencyContactVM.FileName ?? Convert.DBNull);
                    }

                    
                    cmdUpdate.Parameters.AddWithValue("@Email", employeeEmergencyContactVM.Email ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", employeeEmergencyContactVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeEmergencyContactVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeEmergencyContactVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeEmergencyContactVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = employeeEmergencyContactVM.Id.ToString();// Return Id
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
                    throw new ArgumentNullException("Emergency Contact Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Emergency Contact.";
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
        public EmployeeEmergencyContactVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SqlConnection VcurrConn = null;
            SqlTransaction Vtransaction = null;
            SqlTransaction transaction = null;

            EmployeeEmergencyContactVM employeeEmergencyContactVM = new EmployeeEmergencyContactVM();

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
,Mobile
,Fax
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
FROM EmployeeEmergencyContact    
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
                        employeeEmergencyContactVM.Id = Convert.ToInt32(dr["Id"]);
                        employeeEmergencyContactVM.EmployeeId = dr["EmployeeId"].ToString();

                        employeeEmergencyContactVM.Name = dr["Name"].ToString();
                        employeeEmergencyContactVM.Relation = dr["Relation"].ToString();
                        employeeEmergencyContactVM.District = dr["District"].ToString();
                        employeeEmergencyContactVM.Division = dr["Division"].ToString();
                        employeeEmergencyContactVM.Country = dr["Country"].ToString();
                        employeeEmergencyContactVM.City = dr["City"].ToString();
                        employeeEmergencyContactVM.PostalCode = dr["PostalCode"].ToString();
                        employeeEmergencyContactVM.Phone = dr["Phone"].ToString();
                        employeeEmergencyContactVM.Mobile = dr["Mobile"].ToString();
                        employeeEmergencyContactVM.Fax = dr["Fax"].ToString();
                        employeeEmergencyContactVM.FileName = dr["FileName"].ToString();

                        employeeEmergencyContactVM.Remarks = dr["Remarks"].ToString();
                        employeeEmergencyContactVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeeEmergencyContactVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeeEmergencyContactVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeeEmergencyContactVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeeEmergencyContactVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeeEmergencyContactVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeeEmergencyContactVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return employeeEmergencyContactVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeeEmergencyContactVM employeeEmergencyContactVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EducationDelete"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEducation"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (employeeEmergencyContactVM.Id > 0)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeEmergencyContact set";
                    sqlText += " IsArchive=@IsArchive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", employeeEmergencyContactVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeEmergencyContactVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeEmergencyContactVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeEmergencyContactVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = employeeEmergencyContactVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Education Delete", employeeEmergencyContactVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Contact Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Contact Information.";
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
        public List<EmployeeEmergencyContactVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeEmergencyContactVM> VMs = new List<EmployeeEmergencyContactVM>();
            EmployeeEmergencyContactVM vm;
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
 isnull(EmCn.Id,0)						  Id
,ei.EmployeeId							EmployeeId
,isnull(EmCn.Name, 'NA')			 Name
,isnull(EmCn.Relation, 'NA')		 Relation
,isnull(EmCn.[Address], 'NA')		 [Address]
,isnull(EmCn.District, 'NA')		 District
,isnull(EmCn.Division, 'NA')		 Division
,isnull(EmCn.Country, 'NA')			Country
,isnull(EmCn.City, 'NA')			 City
,isnull(EmCn.PostalCode,'NA')		 PostalCode
,isnull(EmCn.Phone, 'NA')			 Phone
,isnull(EmCn.Mobile, 'NA')			Mobile
,isnull(EmCn.Fax, 'NA')				Fax
,isnull(EmCn.[FileName], 'NA')		 [FileName]

,isnull(EmCn.Email, 'NA')			Email
,isnull(EmCn.Remarks, 'NA')			Remarks
,isnull(EmCn.IsActive, 0)			IsActive
,isnull(EmCn.IsArchive, 0)			IsArchive
,isnull(EmCn.CreatedBy, 'NA')		 CreatedBy
,isnull(EmCn.CreatedAt, 'NA')		 CreatedAt
,isnull(EmCn.CreatedFrom, 'NA')		CreatedFrom
,isnull(EmCn.LastUpdateBy, 'NA')	 LastUpdateBy
,isnull(EmCn.LastUpdateAt,	'NA')	 LastUpdateAt
,isnull(EmCn.LastUpdateFrom,	'NA')	 LastUpdateFrom

    From ViewEmployeeInformation ei
		left outer join EmployeeEmergencyContact EmCn on ei.EmployeeId=EmCn.EmployeeId
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
                    vm = new EmployeeEmergencyContactVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();

                    vm.Name = dr["Name"].ToString();
                    vm.Relation = dr["Relation"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.PostalCode = dr["PostalCode"].ToString();
                    vm.Phone = dr["Phone"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Fax = dr["Fax"].ToString();
                    vm.FileName = dr["FileName"].ToString();


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
