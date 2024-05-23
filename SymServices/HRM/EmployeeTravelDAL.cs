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
    public class EmployeeTravelDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeTravelVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeTravelVM> VMs = new List<EmployeeTravelVM>();
            EmployeeTravelVM vm;
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
,TravelType_E
,TravelFromAddress
,TravelToAddress
,FromDate
,ToDate
,FromTime
,ToTime
,Allowances
,IssueDate
,ExpiryDate
,Country
,PassportNumber
,EmbassyName
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeTravel
Where IsArchive=0
    ORDER BY TravelType_E
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeTravelVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.TravelType_E = dr["TravelType_E"].ToString();

                    vm.TravelFromAddress = dr["TravelFromAddress"].ToString();
                    vm.TravelToAddress = dr["TravelToAddress"].ToString();
                    vm.FromDate = Ordinary.StringToDate( dr["FromDate"].ToString());
                    vm.ToDate =  Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.FromTime = Ordinary.StringToTime( dr["FromTime"].ToString());
                    vm.ToTime = Ordinary.StringToTime( dr["ToTime"].ToString());
                    vm.TravleTime = Ordinary.DateDifferenceYMD(dr["FromDate"].ToString(),dr["ToDate"].ToString(),true);
                    vm.Allowances = Convert.ToDecimal(dr["Allowances"].ToString());
                    
//IssueDate
//ExpiryDate
//Country
//PassportNumber
                    vm.IssueDate = Ordinary.StringToDate(dr["IssueDate"].ToString());
                    vm.ExpiryDate = Ordinary.StringToDate(dr["ExpiryDate"].ToString());
                    vm.Country = dr["Country"].ToString();
                    vm.PassportNumber = dr["PassportNumber"].ToString();
                    vm.EmbassyName = dr["EmbassyName"].ToString();
                    
                    
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
        //==================SelectAll=================
        public List<EmployeeTravelVM> SelectAllByEmployee(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeTravelVM> VMs = new List<EmployeeTravelVM>();
            EmployeeTravelVM vm;
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
,TravelType_E
,TravelFromAddress
,TravelToAddress
,FromDate
,ToDate
,FromTime
,ToTime
,Allowances
,IssueDate
,ExpiryDate
,Country
,PassportNumber
,EmbassyName
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeTravel
Where IsArchive=0 and EmployeeId=@EmployeeId
    ORDER BY TravelType_E
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
                    vm = new EmployeeTravelVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.TravelType_E = dr["TravelType_E"].ToString();

                    vm.TravelFromAddress = dr["TravelFromAddress"].ToString();
                    vm.TravelToAddress = dr["TravelToAddress"].ToString();
                    vm.FromDate =  Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate =  Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.FromTime = Ordinary.StringToTime( dr["FromTime"].ToString());
                    vm.ToTime = Ordinary.StringToTime( dr["ToTime"].ToString());
                    vm.TravleTime = Ordinary.DateDifferenceYMD(dr["FromDate"].ToString(), dr["ToDate"].ToString(),true);
                    vm.Allowances = Convert.ToDecimal(dr["Allowances"].ToString());
//IssueDate
//ExpiryDate
//Country
//PassportNumber
                    vm.IssueDate = Ordinary.StringToDate(dr["IssueDate"].ToString());
                    vm.ExpiryDate = Ordinary.StringToDate(dr["ExpiryDate"].ToString());
                    vm.Country = dr["Country"].ToString();
                    vm.PassportNumber = dr["PassportNumber"].ToString();
                    vm.EmbassyName = dr["EmbassyName"].ToString();
                    
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
        public EmployeeTravelVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeTravelVM vm = new EmployeeTravelVM();

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
,TravelType_E
,TravelFromAddress
,TravelToAddress
,FromDate
,ToDate
,FromTime
,ToTime
,Allowances
,IssueDate
,ExpiryDate
,Country
,PassportNumber
,EmbassyName
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
    From EmployeeTravel
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
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.TravelType_E = dr["TravelType_E"].ToString();

                    vm.TravelFromAddress = dr["TravelFromAddress"].ToString();
                    vm.TravelToAddress = dr["TravelToAddress"].ToString();
                    vm.FromDate =  Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate =  Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.FromTime =Ordinary.StringToTime(dr["FromTime"].ToString());
                    vm.ToTime = Ordinary.StringToTime(dr["ToTime"].ToString());
                    vm.FileName = dr["FileName"].ToString();
                    vm.TravleTime = Ordinary.DateDifferenceYMD(dr["FromDate"].ToString(), dr["ToDate"].ToString(),true);
                    vm.Allowances = Convert.ToDecimal(dr["Allowances"].ToString());

                    vm.IssueDate = Ordinary.StringToDate(dr["IssueDate"].ToString());
                    vm.ExpiryDate = Ordinary.StringToDate(dr["ExpiryDate"].ToString());
                    vm.Country = dr["Country"].ToString();
                    vm.PassportNumber = dr["PassportNumber"].ToString();
                    vm.EmbassyName = dr["EmbassyName"].ToString();
                    
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return vm;
        }
        //==================Insert =================
        public string[] Insert(EmployeeTravelVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeTravel"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(employeeTravelVM.DepartmentId))
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

                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeTravel ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", employeeTravelVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", employeeTravelVM.Name);
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Travel Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Travel Value", "");
                //}
                //#endregion Exist
                #region Save

                //int foundId = (int)objfoundId;
                if (1 == 1)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeTravel(	
	EmployeeId,TravelType_E,TravelFromAddress,TravelToAddress,FromDate,ToDate,FromTime,ToTime,FileName,Allowances
,IssueDate,ExpiryDate,Country,PassportNumber,EmbassyName
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
)  VALUES (
@EmployeeId,@TravelType_E,@TravelFromAddress,@TravelToAddress,@FromDate,@ToDate,@FromTime,@ToTime,@FileName,@Allowances
,@IssueDate,@ExpiryDate,@Country,@PassportNumber,@EmbassyName
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@TravelType_E", vm.TravelType_E );

                    cmdInsert.Parameters.AddWithValue("@TravelFromAddress", vm.TravelFromAddress );
                    cmdInsert.Parameters.AddWithValue("@TravelToAddress", vm.TravelToAddress );
                    cmdInsert.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate));
                    cmdInsert.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate));
                    cmdInsert.Parameters.AddWithValue("@FromTime", Ordinary.TimeToString(vm.FromTime));//employeeTravelVM.FromTime ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ToTime", Ordinary.TimeToString(vm.ToTime));//employeeTravelVM.ToTime ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Allowances", vm.Allowances);
                    cmdInsert.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(vm.IssueDate));
                    cmdInsert.Parameters.AddWithValue("@ExpiryDate", Ordinary.DateToString(vm.ExpiryDate));
                    cmdInsert.Parameters.AddWithValue("@Country", vm.Country  ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PassportNumber", vm.PassportNumber ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@EmbassyName", vm.EmbassyName ?? Convert.DBNull);
                    
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    var exeRes = cmdInsert.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);
                    
                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Travel Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Travel Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Travel already used";
                    throw new ArgumentNullException("Please Input Employee Travel Value", "");
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
        public string[] Update(EmployeeTravelVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Travel Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToTravel"); }

                #endregion open connection and transaction
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeTravel ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", employeeTravelVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", employeeTravelVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", employeeTravelVM.Name);

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
                    sqlText = "update EmployeeTravel set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " TravelType_E=@TravelType_E,";

                    sqlText += " TravelFromAddress=@TravelFromAddress,";
                    sqlText += " TravelToAddress=@TravelToAddress,";
                    sqlText += " FromDate=@FromDate,";
                    sqlText += " ToDate=@ToDate,";
                    sqlText += " FromTime=@FromTime,";
                    sqlText += " ToTime=@ToTime,";
                    if (vm.FileName !=null)
                    {
                        sqlText += " FileName=@FileName,";
                    }

                    sqlText += " Allowances=@Allowances,";
                    sqlText += " IssueDate=@IssueDate,";
                    sqlText += " ExpiryDate=@ExpiryDate,";
                    sqlText += " Country=@Country,";
                    sqlText += " PassportNumber=@PassportNumber,";
                    sqlText += " EmbassyName=@EmbassyName,";
                    
                    sqlText += " Remarks=@Remarks,";
                    //sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@TravelType_E", vm.TravelType_E);

                    cmdUpdate.Parameters.AddWithValue("@Allowances", vm.Allowances);
                    cmdUpdate.Parameters.AddWithValue("@TravelFromAddress", vm.TravelFromAddress);
                    cmdUpdate.Parameters.AddWithValue("@TravelToAddress", vm.TravelToAddress );
                    cmdUpdate.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate));
                    cmdUpdate.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate));
                    cmdUpdate.Parameters.AddWithValue("@FromTime", Ordinary.TimeToString(vm.FromTime));
                    cmdUpdate.Parameters.AddWithValue("@ToTime", Ordinary.TimeToString(vm.ToTime));
                    if (vm.FileName != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    }
                    cmdUpdate.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(vm.IssueDate));
                    cmdUpdate.Parameters.AddWithValue("@ExpiryDate", Ordinary.DateToString(vm.ExpiryDate));
                    cmdUpdate.Parameters.AddWithValue("@Country", vm.Country  ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PassportNumber", vm.PassportNumber ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EmbassyName", vm.EmbassyName ?? Convert.DBNull);
                    
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                   // cmdUpdate.Parameters.AddWithValue("@IsActive", employeeTravelVM.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
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
                    throw new ArgumentNullException("Travel Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Travel.";
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
        public EmployeeTravelVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeeTravelVM employeeTravelVM = new EmployeeTravelVM();

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
,TravelType_E
,TravelFromAddress
,TravelToAddress
,FromDate
,ToDate
,FromTime
,ToTime
,CreatedAt1
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeTravel 
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
                        employeeTravelVM.Id = Convert.ToInt32(dr["Id"]);
                        employeeTravelVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeeTravelVM.TravelType_E = dr["TravelType_E"].ToString();

                        employeeTravelVM.TravelFromAddress = dr["TravelFromAddress"].ToString();
                        employeeTravelVM.TravelToAddress = dr["TravelToAddress"].ToString();
                        employeeTravelVM.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                        employeeTravelVM.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                        employeeTravelVM.FromTime = Ordinary.StringToTime(dr["FromTime"].ToString());
                        employeeTravelVM.ToTime = Ordinary.StringToTime(dr["ToTime"].ToString());

                        employeeTravelVM.Remarks = dr["Remarks"].ToString();
                        employeeTravelVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeeTravelVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeeTravelVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeeTravelVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeeTravelVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeeTravelVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeeTravelVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return employeeTravelVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeeTravelVM EmployeeTravelVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteTravel"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToTravel"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length>=1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeTravel set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeeTravelVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeeTravelVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeeTravelVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                         

                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                   

                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Travel Delete", EmployeeTravelVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Travel Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Travel Information.";
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
        public List<EmployeeTravelVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeTravelVM> VMs = new List<EmployeeTravelVM>();
            EmployeeTravelVM vm;
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
 isnull(Trav.Id,0)						  Id
,ei.EmployeeId							EmployeeId
,isnull(Trav.TravelType_E	, 'NA')		TravelType_E		
,isnull(Trav.TravelFromAddress	, 'NA')		TravelFromAddress		
,isnull(Trav.TravelToAddress	, 'NA')		TravelToAddress		
,isnull(Trav.FromDate	, 'NA')		FromDate		
,isnull(Trav.ToDate	, 'NA')		ToDate		
,isnull(Trav.FromTime	, 'NA')		FromTime		
,isnull(Trav.ToTime	, 'NA')		ToTime		
,isnull(Trav.Allowances	, 0)		Allowances		
,Trav.IssueDate
,Trav.ExpiryDate
,Trav.Country
,Trav.PassportNumber
,Trav.EmbassyName
,isnull(Trav.Remarks		, 'NA')			Remarks
,isnull(Trav.IsActive, 0)			IsActive
,isnull(Trav.IsArchive, 0)			IsArchive
,isnull(Trav.CreatedBy, 'NA')		 CreatedBy
,isnull(Trav.CreatedAt, 'NA')		 CreatedAt
,isnull(Trav.CreatedFrom, 'NA')		CreatedFrom
,isnull(Trav.LastUpdateBy, 'NA')	 LastUpdateBy
,isnull(Trav.LastUpdateAt,	'NA')	 LastUpdateAt
,isnull(Trav.LastUpdateFrom,	'NA')	 LastUpdateFrom   

  --  From ViewEmployeeInformation ei
		--left outer join EmployeeTravel Trav on ei.EmployeeId=Trav.EmployeeId

    From  EmployeeTravel Trav
		left outer join ViewEmployeeInformation ei on ei.EmployeeId=Trav.EmployeeId
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
                sqlText += "  ,Trav.TravelType_E ";

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
                    vm = new EmployeeTravelVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.TravelType_E = dr["TravelType_E"].ToString();

                    vm.TravelFromAddress = dr["TravelFromAddress"].ToString();
                    vm.TravelToAddress = dr["TravelToAddress"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.FromTime = Ordinary.StringToTime(dr["FromTime"].ToString());
                    vm.ToTime = Ordinary.StringToTime(dr["ToTime"].ToString());
                    vm.TravleTime = Ordinary.DateDifferenceYMD(dr["FromDate"].ToString(), dr["ToDate"].ToString(), true);
                    vm.Allowances = Convert.ToDecimal(dr["Allowances"].ToString());

                    vm.IssueDate = Ordinary.StringToDate(dr["IssueDate"].ToString());
                    vm.ExpiryDate = Ordinary.StringToDate(dr["ExpiryDate"].ToString());
                    vm.Country = dr["Country"].ToString();
                    vm.PassportNumber = dr["PassportNumber"].ToString();
                    vm.EmbassyName = dr["EmbassyName"].ToString();
                    

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
