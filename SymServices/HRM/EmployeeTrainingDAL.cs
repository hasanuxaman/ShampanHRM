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
    public class EmployeeTrainingDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        #region Methods
        //==================SelectAll=================
        public List<EmployeeTrainingVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeTrainingVM> VMs = new List<EmployeeTrainingVM>();
            EmployeeTrainingVM vm;
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
,TrainingPlace_E
,TrainingStatus_E
,Topics
,InstituteName
,Location
,FundedBy
,DurationMonth
,DurationDay
,Achievement
,AllowancesTotalTk
,DateFrom
,DateTo
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeTraining
Where IsArchive=0
    ORDER BY Topics
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeTrainingVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.TrainingStatus_E = dr["TrainingStatus_E"].ToString();
                    vm.TrainingPlace_E = dr["TrainingPlace_E"].ToString();
                    vm.Topics = dr["Topics"].ToString();
                    vm.InstituteName = dr["InstituteName"].ToString();
                    vm.Location = dr["Location"].ToString();
                    vm.FundedBy = dr["FundedBy"].ToString();
                    vm.DurationMonth = Convert.ToInt32(dr["DurationMonth"]);
                    vm.DurationDay = Convert.ToInt32(dr["DurationDay"]);
                    vm.Achievement = dr["Achievement"].ToString();
                    vm.AllowancesTotalTk = Convert.ToDecimal(dr["AllowancesTotalTk"]);
                    vm.DateFrom = Ordinary.StringToDate(dr["DateFrom"].ToString());
                    vm.DateTo = Ordinary.StringToDate(dr["DateTo"].ToString());
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
        public List<EmployeeTrainingVM> SelectAllByEmployee(string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeTrainingVM> employeeTrainingVMs = new List<EmployeeTrainingVM>();
            EmployeeTrainingVM employeeTrainingVM;
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
,TrainingPlace_E
,TrainingStatus_E
,Topics
,InstituteName
,Location
,FundedBy
,DurationMonth
,DurationDay
,Achievement
,AllowancesTotalTk
,DateFrom
,DateTo
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeTraining
Where IsArchive=0 AND EmployeeId=@EmployeeId
    ORDER BY Topics
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
                    employeeTrainingVM = new EmployeeTrainingVM();
                    employeeTrainingVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeTrainingVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeTrainingVM.TrainingStatus_E = dr["TrainingStatus_E"].ToString();
                    employeeTrainingVM.TrainingPlace_E = dr["TrainingPlace_E"].ToString();
                    employeeTrainingVM.Topics = dr["Topics"].ToString();
                    employeeTrainingVM.InstituteName = dr["InstituteName"].ToString();
                    employeeTrainingVM.Location = dr["Location"].ToString();
                    employeeTrainingVM.FundedBy = dr["FundedBy"].ToString();
                    employeeTrainingVM.DurationMonth = Convert.ToInt32(dr["DurationMonth"]);
                    employeeTrainingVM.DurationDay = Convert.ToInt32(dr["DurationDay"]);
                    employeeTrainingVM.Achievement = dr["Achievement"].ToString();
                    employeeTrainingVM.AllowancesTotalTk = Convert.ToDecimal(dr["AllowancesTotalTk"]);
                    employeeTrainingVM.DateFrom = Ordinary.StringToDate(dr["DateFrom"].ToString());
                    employeeTrainingVM.DateTo = Ordinary.StringToDate(dr["DateTo"].ToString());
                    employeeTrainingVM.Remarks = dr["Remarks"].ToString();
                    employeeTrainingVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeTrainingVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeTrainingVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeTrainingVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeTrainingVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeTrainingVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeTrainingVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeTrainingVMs.Add(employeeTrainingVM);
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
            return employeeTrainingVMs;
        }
        //==================SelectByID=================
        public EmployeeTrainingVM SelectById(int Id)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeTrainingVM employeeTrainingVM = new EmployeeTrainingVM();
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
,TrainingPlace_E
,TrainingStatus_E
,Topics
,InstituteName
,Location
,FundedBy
,DurationMonth
,DurationDay
,Achievement
,AllowancesTotalTk
,DateFrom
,DateTo
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
    From EmployeeTraining
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
                    employeeTrainingVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeTrainingVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeTrainingVM.TrainingStatus_E = dr["TrainingStatus_E"].ToString();
                    employeeTrainingVM.TrainingPlace_E = dr["TrainingPlace_E"].ToString();
                    employeeTrainingVM.Topics = dr["Topics"].ToString();
                    employeeTrainingVM.InstituteName = dr["InstituteName"].ToString();
                    employeeTrainingVM.Location = dr["Location"].ToString();
                    employeeTrainingVM.FundedBy = dr["FundedBy"].ToString();
                    employeeTrainingVM.DurationMonth = Convert.ToInt32(dr["DurationMonth"]);
                    employeeTrainingVM.DurationDay = Convert.ToInt32(dr["DurationDay"]);
                    employeeTrainingVM.Achievement = dr["Achievement"].ToString();
                    employeeTrainingVM.AllowancesTotalTk = Convert.ToDecimal(dr["AllowancesTotalTk"]);
                    employeeTrainingVM.FileName = dr["FileName"].ToString();
                    employeeTrainingVM.DateFrom = Ordinary.StringToDate(dr["DateFrom"].ToString());
                    employeeTrainingVM.DateTo = Ordinary.StringToDate(dr["DateTo"].ToString());
                    employeeTrainingVM.Remarks = dr["Remarks"].ToString();
                    employeeTrainingVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeTrainingVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeTrainingVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeTrainingVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeTrainingVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeTrainingVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeTrainingVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
            return employeeTrainingVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeeTrainingVM employeeTrainingVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeTraining"; //Method Name
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
                #endregion open connection and transaction
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeTraining ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", employeeTrainingVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", employeeTrainingVM.Name);
                //object objfoundId = cmdExist.ExecuteScalar();
                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Training Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Training Value", "");
                //}
                //#endregion Exist
                #region Save
                //int foundId = (int)objfoundId;
                if (1 == 1)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeTraining(	EmployeeId,TrainingStatus_E
,TrainingPlace_E,Topics,InstituteName,Location,FundedBy,DurationMonth,DurationDay,Achievement,AllowancesTotalTk,FileName,DateFrom,DateTo
                                        ,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@EmployeeId,@TrainingStatus_E
,@TrainingPlace_E,@Topics,@InstituteName,@Location,@FundedBy,@DurationMonth,@DurationDay,@Achievement,@AllowancesTotalTk,@FileName,@DateFrom,@DateTo
                                        ,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) SELECT SCOPE_IDENTITY()";
                    SqlCommand cmd = new SqlCommand(sqlText, currConn);
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeTrainingVM.EmployeeId);
                    cmd.Parameters.AddWithValue("@TrainingStatus_E", employeeTrainingVM.TrainingStatus_E ?? Convert.DBNull);
                    cmd.Parameters.AddWithValue("@TrainingPlace_E", employeeTrainingVM.TrainingPlace_E ?? Convert.DBNull);
                    cmd.Parameters.AddWithValue("@Topics", employeeTrainingVM.Topics ?? Convert.DBNull);
                    cmd.Parameters.AddWithValue("@InstituteName", employeeTrainingVM.InstituteName ?? Convert.DBNull);
                    cmd.Parameters.AddWithValue("@Location", employeeTrainingVM.Location ?? Convert.DBNull);
                    cmd.Parameters.AddWithValue("@FundedBy", employeeTrainingVM.FundedBy ?? Convert.DBNull);
                    cmd.Parameters.AddWithValue("@DurationMonth", employeeTrainingVM.DurationMonth );
                    cmd.Parameters.AddWithValue("@DurationDay", employeeTrainingVM.DurationDay );
                    cmd.Parameters.AddWithValue("@Achievement", employeeTrainingVM.Achievement ?? Convert.DBNull);
                    cmd.Parameters.AddWithValue("@AllowancesTotalTk", employeeTrainingVM.AllowancesTotalTk);
                    cmd.Parameters.AddWithValue("@FileName", employeeTrainingVM.FileName ?? Convert.DBNull);
                    cmd.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(employeeTrainingVM.DateFrom));
                    cmd.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(employeeTrainingVM.DateTo));
                    cmd.Parameters.AddWithValue("@Remarks", employeeTrainingVM.Remarks ?? Convert.DBNull);
                    cmd.Parameters.AddWithValue("@IsActive", true);
                    cmd.Parameters.AddWithValue("@IsArchive", false);
                    cmd.Parameters.AddWithValue("@CreatedBy", employeeTrainingVM.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedAt", employeeTrainingVM.CreatedAt);
                    cmd.Parameters.AddWithValue("@CreatedFrom", employeeTrainingVM.CreatedFrom);
                    cmd.Transaction = transaction;
                    var exeRes = cmd.ExecuteScalar();
				    Id = Convert.ToInt32(exeRes);
                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Training Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Training Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Training already used";
                    throw new ArgumentNullException("Please Input Employee Training Value", "");
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
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
        public string[] Update(EmployeeTrainingVM employeeTrainingVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Training Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToTraining"); }
                #endregion open connection and transaction
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeTraining ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", employeeTrainingVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", employeeTrainingVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", employeeTrainingVM.Name);
                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This Training already used";
                //    throw new ArgumentNullException("Please Input Training Value", "");
                //}
                //#endregion Exist
                if (employeeTrainingVM != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EmployeeTraining set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " TrainingStatus_E=@TrainingStatus_E,";
                    sqlText += " TrainingPlace_E=@TrainingPlace_E,";
                    sqlText += " Topics=@Topics,";
                    sqlText += " InstituteName=@InstituteName,";
                    sqlText += " Location=@Location,";
                    sqlText += " FundedBy=@FundedBy,";
                    sqlText += " DurationMonth=@DurationMonth,";
                    sqlText += " DurationDay=@DurationDay,";
                    sqlText += " Achievement=@Achievement,";
                    sqlText += " AllowancesTotalTk=@AllowancesTotalTk,";
                    if (employeeTrainingVM.FileName !=null)
                    {
                        sqlText += " FileName=@FileName,";
                    }
                    sqlText += " DateFrom=@DateFrom,";
                    sqlText += " DateTo=@DateTo,";
                    sqlText += " Remarks=@Remarks,";
                    //sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", employeeTrainingVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", employeeTrainingVM.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@TrainingStatus_E", employeeTrainingVM.TrainingStatus_E ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TrainingPlace_E", employeeTrainingVM.TrainingPlace_E ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Topics", employeeTrainingVM.Topics ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@InstituteName", employeeTrainingVM.InstituteName ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Location", employeeTrainingVM.Location ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@FundedBy", employeeTrainingVM.FundedBy ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DurationMonth", employeeTrainingVM.DurationMonth);
                    cmdUpdate.Parameters.AddWithValue("@DurationDay", employeeTrainingVM.DurationDay);
                    cmdUpdate.Parameters.AddWithValue("@Achievement", employeeTrainingVM.Achievement ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@AllowancesTotalTk", employeeTrainingVM.AllowancesTotalTk);
                    if (employeeTrainingVM.FileName != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", employeeTrainingVM.FileName ?? Convert.DBNull);
                    }
                    cmdUpdate.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(employeeTrainingVM.DateFrom));
                    cmdUpdate.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(employeeTrainingVM.DateTo));
                    cmdUpdate.Parameters.AddWithValue("@Remarks", employeeTrainingVM.Remarks ?? Convert.DBNull);
                    //cmdUpdate.Parameters.AddWithValue("@IsActive", employeeTrainingVM.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeTrainingVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeTrainingVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeTrainingVM.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);
                    retResults[2] = employeeTrainingVM.Id.ToString();// Return Id
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
                    throw new ArgumentNullException("Training Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Training.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
        public EmployeeTrainingVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeTrainingVM employeeTrainingVM = new EmployeeTrainingVM();
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
,TrainingPlace_E
,TrainingStatus_E
,Topics
,InstituteName
,Location
,FundedBy
,DurationMonth
,DurationDay
,Achievement
,AllowancesTotalTk
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeTraining 
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
                        employeeTrainingVM.Id = Convert.ToInt32(dr["Id"]);
                        employeeTrainingVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeeTrainingVM.TrainingStatus_E = dr["TrainingStatus_E"].ToString();
                        employeeTrainingVM.TrainingPlace_E = dr["TrainingPlace_E"].ToString();
                        employeeTrainingVM.Topics = dr["Topics"].ToString();
                        employeeTrainingVM.InstituteName = dr["InstituteName"].ToString();
                        employeeTrainingVM.Location = dr["Location"].ToString();
                        employeeTrainingVM.FundedBy = dr["FundedBy"].ToString();
                        employeeTrainingVM.DurationMonth = Convert.ToInt32(dr["DurationMonth"]);
                        employeeTrainingVM.DurationDay = Convert.ToInt32(dr["DurationDay"]);
                        employeeTrainingVM.Achievement = dr["Achievement"].ToString();
                        employeeTrainingVM.AllowancesTotalTk = Convert.ToDecimal(dr["AllowancesTotalTk"]);
                        employeeTrainingVM.Remarks = dr["Remarks"].ToString();
                        employeeTrainingVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeeTrainingVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeeTrainingVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeeTrainingVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeeTrainingVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeeTrainingVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeeTrainingVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
            return employeeTrainingVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeeTrainingVM EmployeeTrainingVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteTraining"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToTraining"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (ids.Length>=1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeTraining set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeeTrainingVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeeTrainingVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeeTrainingVM.LastUpdateFrom);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult	= Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Training Delete", EmployeeTrainingVM.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Training Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Training Information.";
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
        public List<EmployeeTrainingVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeTrainingVM> VMs = new List<EmployeeTrainingVM>();
            EmployeeTrainingVM vm;
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
 isnull(Tr.Id,0)						  Id
,ei.EmployeeId							EmployeeId
,isnull(Tr.TrainingPlace_E	, 'NA')		TrainingPlace_E		
,isnull(Tr.TrainingStatus_E	, 'NA')		TrainingStatus_E		
,isnull(Tr.Topics	, 'NA')		Topics		
,isnull(Tr.InstituteName	, 'NA')		InstituteName		
,isnull(Tr.Location	, 'NA')		Location		
,isnull(Tr.FundedBy	, 'NA')		FundedBy		
,isnull(Tr.DurationMonth	, 0)		DurationMonth		
,isnull(Tr.DurationDay	, 0)		DurationDay		
,isnull(Tr.Achievement	, 'NA')		Achievement		
,isnull(Tr.AllowancesTotalTk	, 0)		AllowancesTotalTk		
,isnull(Tr.DateFrom	, 'NA')		DateFrom		
,isnull(Tr.DateTo	, 'NA')		DateTo		
,isnull(Tr.Remarks		, 'NA')			Remarks
,isnull(Tr.IsActive, 0)			IsActive
,isnull(Tr.IsArchive, 0)			IsArchive
,isnull(Tr.CreatedBy, 'NA')		 CreatedBy
,isnull(Tr.CreatedAt, 'NA')		 CreatedAt
,isnull(Tr.CreatedFrom, 'NA')		CreatedFrom
,isnull(Tr.LastUpdateBy, 'NA')	 LastUpdateBy
,isnull(Tr.LastUpdateAt,	'NA')	 LastUpdateAt
,isnull(Tr.LastUpdateFrom,	'NA')	 LastUpdateFrom   
    From ViewEmployeeInformation ei
		left outer join EmployeeTraining Tr on ei.EmployeeId=Tr.EmployeeId
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
                sqlText += " , Tr.Topics ";
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
                    vm = new EmployeeTrainingVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.TrainingStatus_E = dr["TrainingStatus_E"].ToString();
                    vm.TrainingPlace_E = dr["TrainingPlace_E"].ToString();
                    vm.Topics = dr["Topics"].ToString();
                    vm.InstituteName = dr["InstituteName"].ToString();
                    vm.Location = dr["Location"].ToString();
                    vm.FundedBy = dr["FundedBy"].ToString();
                    vm.DurationMonth = Convert.ToInt32(dr["DurationMonth"]);
                    vm.DurationDay = Convert.ToInt32(dr["DurationDay"]);
                    vm.Achievement = dr["Achievement"].ToString();
                    vm.AllowancesTotalTk = Convert.ToDecimal(dr["AllowancesTotalTk"]);
                    vm.DateFrom = Ordinary.StringToDate(dr["DateFrom"].ToString());
                    vm.DateTo = Ordinary.StringToDate(dr["DateTo"].ToString());
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
