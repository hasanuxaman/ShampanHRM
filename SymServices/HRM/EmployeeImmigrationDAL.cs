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
    public class EmployeeImmigrationDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeImmigrationVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeImmigrationVM> VMs = new List<EmployeeImmigrationVM>();
            EmployeeImmigrationVM vm;
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
,ISNULL(EmployeeId, '0')EmployeeId
,ISNULL(ImmigrationType_E, '0')ImmigrationType_E
,ISNULL(ImmigrationNumber, '0')ImmigrationNumber
,ISNULL(IssueDate, '0')IssueDate
,ISNULL(ExpireDate, '0')ExpireDate
,ISNULL(IssuedBy_E, '0')IssuedBy_E
,ISNULL(EligibleReviewDate, '0')EligibleReviewDate
,ISNULL(Remarks, '0')Remarks
,ISNULL(IsActive, '0')IsActive
,ISNULL(IsArchive, '0')IsArchive
,ISNULL(CreatedBy, '0')CreatedBy
,ISNULL(CreatedAt, '0')CreatedAt
,ISNULL(CreatedFrom, '0')CreatedFrom
,ISNULL(LastUpdateBy, '0')LastUpdateBy
,ISNULL(LastUpdateAt, '0')LastUpdateAt
,ISNULL(LastUpdateFrom, '0')LastUpdateFrom
    FROM EmployeeImmigration
Where IsArchive=0
    ORDER BY ImmigrationType_E
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
                    vm = new EmployeeImmigrationVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ImmigrationType_E = dr["ImmigrationType_E"].ToString();
                    vm.ImmigrationNumber = dr["ImmigrationNumber"].ToString();
                    vm.IssueDate = Ordinary.StringToDate(dr["IssueDate"].ToString());
                    vm.ExpireDate =Ordinary.StringToDate( dr["ExpireDate"].ToString());
                    vm.IssuedBy_E = dr["IssuedBy_E"].ToString();
                    vm.EligibleReviewDate = dr["EligibleReviewDate"].ToString();
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
        public List<EmployeeImmigrationVM> SelectAllByEmployee(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeImmigrationVM> employeeImmigrations = new List<EmployeeImmigrationVM>();
            EmployeeImmigrationVM employeeImmigration;
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
,ISNULL(EmployeeId, '0')EmployeeId
,ISNULL(ImmigrationType_E, '0')ImmigrationType_E
,ISNULL(ImmigrationNumber, '0')ImmigrationNumber
,ISNULL(IssueDate, '0')IssueDate
,ISNULL(ExpireDate, '0')ExpireDate
,ISNULL(IssuedBy_E, '0')IssuedBy_E
,ISNULL(EligibleReviewDate, '0')EligibleReviewDate
,ISNULL(Remarks, '0')Remarks
,ISNULL(IsActive, '0')IsActive
,ISNULL(IsArchive, '0')IsArchive
,ISNULL(CreatedBy, '0')CreatedBy
,ISNULL(CreatedAt, '0')CreatedAt
,ISNULL(CreatedFrom, '0')CreatedFrom
,ISNULL(LastUpdateBy, '0')LastUpdateBy
,ISNULL(LastUpdateAt, '0')LastUpdateAt
,ISNULL(LastUpdateFrom, '0')LastUpdateFrom
    FROM EmployeeImmigration
Where IsArchive=0 and EmployeeId=@EmployeeId
    ORDER BY ImmigrationType_E
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
                    employeeImmigration = new EmployeeImmigrationVM();
                    employeeImmigration.Id = Convert.ToInt32(dr["Id"]);
                    employeeImmigration.EmployeeId = dr["EmployeeId"].ToString();
                    employeeImmigration.ImmigrationType_E = dr["ImmigrationType_E"].ToString();
                    employeeImmigration.ImmigrationNumber = dr["ImmigrationNumber"].ToString();
                    employeeImmigration.IssueDate = Ordinary.StringToDate(dr["IssueDate"].ToString());
                    employeeImmigration.ExpireDate = Ordinary.StringToDate(dr["ExpireDate"].ToString());
                    employeeImmigration.IssuedBy_E = dr["IssuedBy_E"].ToString();
                    employeeImmigration.EligibleReviewDate = dr["EligibleReviewDate"].ToString();
                    employeeImmigration.Remarks = dr["Remarks"].ToString();
                    employeeImmigration.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeImmigration.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeImmigration.CreatedBy = dr["CreatedBy"].ToString();
                    employeeImmigration.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeImmigration.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeImmigration.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeImmigration.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeImmigrations.Add(employeeImmigration);
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

            return employeeImmigrations;
        }
        //==================SelectByID=================
        public EmployeeImmigrationVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeImmigrationVM employeeImmigrationVM = new EmployeeImmigrationVM();

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
,ISNULL(EmployeeId, '0')EmployeeId
,ISNULL(ImmigrationType_E, '0')ImmigrationType_E
,ISNULL(ImmigrationNumber, '0')ImmigrationNumber
,ISNULL(IssueDate, '0')IssueDate
,ISNULL(ExpireDate, '0')ExpireDate
,ISNULL(IssuedBy_E, '0')IssuedBy_E
,ISNULL(EligibleReviewDate, '0')EligibleReviewDate
,ISNULL(FileName, '0')FileName
,ISNULL(Remarks, '0')Remarks
,ISNULL(IsActive, '0')IsActive
,ISNULL(IsArchive, '0')IsArchive
,ISNULL(CreatedBy, '0')CreatedBy
,ISNULL(CreatedAt, '0')CreatedAt
,ISNULL(CreatedFrom, '0')CreatedFrom
,ISNULL(LastUpdateBy, '0')LastUpdateBy
,ISNULL(LastUpdateAt, '0')LastUpdateAt
,ISNULL(LastUpdateFrom, '0')LastUpdateFrom
FROM EmployeeImmigration
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
                    employeeImmigrationVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeImmigrationVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeImmigrationVM.ImmigrationType_E = dr["ImmigrationType_E"].ToString();
                    employeeImmigrationVM.ImmigrationNumber = dr["ImmigrationNumber"].ToString();
                    employeeImmigrationVM.IssueDate = Ordinary.StringToDate(dr["IssueDate"].ToString());
                    employeeImmigrationVM.ExpireDate = Ordinary.StringToDate(dr["ExpireDate"].ToString());
                    employeeImmigrationVM.IssuedBy_E = dr["IssuedBy_E"].ToString();
                    employeeImmigrationVM.EligibleReviewDate = dr["EligibleReviewDate"].ToString();
                    employeeImmigrationVM.FileName = dr["FileName"].ToString();
                    employeeImmigrationVM.Remarks = dr["Remarks"].ToString();
                    employeeImmigrationVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeImmigrationVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeImmigrationVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeImmigrationVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeImmigrationVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeImmigrationVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeImmigrationVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeeImmigrationVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeeImmigrationVM empImmigrationVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeImmigration"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                //#region Validation
                //if (string.IsNullOrEmpty(empImmigrationVM.Degree_E))
                //{
                //    retResults[1] = "Please Input Employee Degree_E";
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

                #region Exist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeImmigration ";
                sqlText += " WHERE EmployeeId=@EmployeeId And ImmigrationNumber=@ImmigrationNumber";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", empImmigrationVM.EmployeeId);
                cmdExist.Parameters.AddWithValue("@ImmigrationNumber", empImmigrationVM.ImmigrationNumber);
                object objfoundId = cmdExist.ExecuteScalar();

                if (objfoundId == null)
                {
                    retResults[1] = "Please Input Employee Immigration Value";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Input Employee Immigration Value", "");
                }
                #endregion Exist
                #region Save

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeImmigration(	
                    EmployeeId,ImmigrationType_E,ImmigrationNumber,IssueDate,ExpireDate,IssuedBy_E,FileName,Remarks,EligibleReviewDate,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)
                    VALUES (
                    @EmployeeId,@ImmigrationType_E,@ImmigrationNumber,@IssueDate,@ExpireDate,@IssuedBy_E,@FileName,@Remarks,@EligibleReviewDate,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom)
                    SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", empImmigrationVM.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@ImmigrationType_E", empImmigrationVM.ImmigrationType_E ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ImmigrationNumber", empImmigrationVM.ImmigrationNumber );
                    cmdInsert.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(empImmigrationVM.IssueDate));
                    cmdInsert.Parameters.AddWithValue("@ExpireDate", Ordinary.DateToString(empImmigrationVM.ExpireDate));
                    cmdInsert.Parameters.AddWithValue("@IssuedBy_E", empImmigrationVM.IssuedBy_E ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FileName", empImmigrationVM.FileName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", empImmigrationVM.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@EligibleReviewDate", empImmigrationVM.EligibleReviewDate ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", empImmigrationVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", empImmigrationVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", empImmigrationVM.CreatedFrom);

                    cmdInsert.Transaction = transaction;
					var exeRes = cmdInsert.ExecuteScalar();
					Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Immigration Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Immigration Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Immigration already used";
                    throw new ArgumentNullException("This Employee Immigration already used", "");
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
        public string[] Update(EmployeeImmigrationVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ImmigrationUpdate"; //Method Name

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
                
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToImmigration"); }
               
                #endregion open connection and transaction

                #region Exist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeImmigration ";
                sqlText += " WHERE EmployeeId=@EmployeeId And ImmigrationNumber=@ImmigrationNumber AND Id<>@Id";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@ImmigrationNumber", vm.ImmigrationNumber);

                if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                {
                    retResults[1] = "This Immigration Number already used";
                    throw new ArgumentNullException("This Immigration Number already used", "");
                }
                #endregion Exist

                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeImmigration set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " ImmigrationType_E=@ImmigrationType_E,";
                    sqlText += " ImmigrationNumber=@ImmigrationNumber,";
                    sqlText += " IssueDate=@IssueDate,";
                    sqlText += " ExpireDate=@ExpireDate,";
                    sqlText += " IssuedBy_E=@IssuedBy_E,";
                    sqlText += " EligibleReviewDate=@EligibleReviewDate,";
                    if (vm.FileName != null)
                    {
                        sqlText += " FileName=@FileName,";
                    }
                    sqlText += " Remarks=@Remarks,";
             //       sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@ImmigrationType_E", vm.ImmigrationType_E ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ImmigrationNumber", vm.ImmigrationNumber );
                    cmdUpdate.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(vm.IssueDate));
                    cmdUpdate.Parameters.AddWithValue("@ExpireDate", Ordinary.DateToString(vm.ExpireDate));
                    cmdUpdate.Parameters.AddWithValue("@IssuedBy_E", vm.IssuedBy_E ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EligibleReviewDate", vm.EligibleReviewDate ?? Convert.DBNull);
                    if (vm.FileName != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    }
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    //cmdUpdate.Parameters.AddWithValue("@IsActive", empImmigrationVM.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", false);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    //#region Commit

                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException("Immigration Update", empImmigrationVM.Degree_E + " could not updated.");
                    //}

                    //#endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Immigration Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Immigration.";
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
        public EmployeeImmigrationVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeeImmigrationVM employeeImmigrationVM = new EmployeeImmigrationVM();

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
,ISNULL(EmployeeId, '0')EmployeeId
,ISNULL(ImmigrationType_E, '0')ImmigrationType_E
,ISNULL(ImmigrationNumber, '0')ImmigrationNumber
,ISNULL(IssueDate, '0')IssueDate
,ISNULL(ExpireDate, '0')ExpireDate
,ISNULL(IssuedBy_E, '0')IssuedBy_E
,ISNULL(EligibleReviewDate, '0')EligibleReviewDate
,ISNULL(Remarks, '0')Remarks
,ISNULL(IsActive, '0')IsActive
,ISNULL(IsArchive, '0')IsArchive
,ISNULL(CreatedBy, '0')CreatedBy
,ISNULL(CreatedAt, '0')CreatedAt
,ISNULL(CreatedFrom, '0')CreatedFrom
,ISNULL(LastUpdateBy, '0')LastUpdateBy
,ISNULL(LastUpdateAt, '0')LastUpdateAt
,ISNULL(LastUpdateFrom, '0')LastUpdateFrom
FROM EmployeeImmigration    
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
                        employeeImmigrationVM.Id = Convert.ToInt32(dr["Id"]);
                        employeeImmigrationVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeeImmigrationVM.ImmigrationType_E = dr["ImmigrationType_E"].ToString();
                        employeeImmigrationVM.ImmigrationNumber = dr["ImmigrationNumber"].ToString();
                        employeeImmigrationVM.IssueDate = dr["IssueDate"].ToString();
                        employeeImmigrationVM.ExpireDate = dr["ExpireDate"].ToString();
                        employeeImmigrationVM.IssuedBy_E = dr["IssuedBy_E"].ToString();
                        employeeImmigrationVM.EligibleReviewDate = dr["EligibleReviewDate"].ToString();
                        employeeImmigrationVM.Remarks = dr["Remarks"].ToString();
                        employeeImmigrationVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeeImmigrationVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeeImmigrationVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeeImmigrationVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeeImmigrationVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeeImmigrationVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeeImmigrationVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return employeeImmigrationVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeeImmigrationVM employeeImmigrationVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ImmigrationDelete"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToImmigration"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length>=1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeImmigration set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeImmigrationVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeImmigrationVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeImmigrationVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Immigration Delete", employeeImmigrationVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Immigration Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Immigration.";
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
        public List<EmployeeImmigrationVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeImmigrationVM> VMs = new List<EmployeeImmigrationVM>();
            EmployeeImmigrationVM vm;
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
 isnull(Immig.Id, '0')						  Id
,isnull(ei.EmployeeId, '0')							EmployeeId
,isnull(Immig.ImmigrationType_E   , 'NA' )ImmigrationType_E
,isnull(Immig.ImmigrationNumber  , 'NA'  )ImmigrationNumber
,isnull(Immig.IssueDate    , 'NA')IssueDate
,isnull(Immig.[ExpireDate]    , 'NA')[ExpireDate]
,isnull(Immig.IssuedBy_E   , 'NA' )IssuedBy_E
,isnull(Immig.EligibleReviewDate, 'Na'    )EligibleReviewDate
,isnull(Immig.Remarks		, 'NA')			Remarks
,isnull(Immig.IsActive, '0')			IsActive
,isnull(Immig.IsArchive, '0')			IsArchive
,isnull(Immig.CreatedBy, 'NA')		 CreatedBy
,isnull(Immig.CreatedAt, 'NA')		 CreatedAt
,isnull(Immig.CreatedFrom, 'NA')		CreatedFrom
,isnull(Immig.LastUpdateBy, 'NA')	 LastUpdateBy
,isnull(Immig.LastUpdateAt,	'NA')	 LastUpdateAt
,isnull(Immig.LastUpdateFrom,	'NA')	 LastUpdateFrom   

    From ViewEmployeeInformation ei
		left outer join EmployeeImmigration Immig on ei.EmployeeId=Immig.EmployeeId
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
                sqlText += " ,Immig.ImmigrationType_E";

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
                    vm = new EmployeeImmigrationVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ImmigrationType_E = dr["ImmigrationType_E"].ToString();
                    vm.ImmigrationNumber = dr["ImmigrationNumber"].ToString();
                    vm.IssueDate = Ordinary.StringToDate(dr["IssueDate"].ToString());
                    vm.ExpireDate = Ordinary.StringToDate(dr["ExpireDate"].ToString());
                    vm.IssuedBy_E = dr["IssuedBy_E"].ToString();
                    vm.EligibleReviewDate = dr["EligibleReviewDate"].ToString();
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
