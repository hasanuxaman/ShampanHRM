using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SymServices.Common
{
    public class PreEmployementInformationDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        public List<PreEmployementInformationVM> DropDownEmpName()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<PreEmployementInformationVM> VMs = new List<PreEmployementInformationVM>();
            PreEmployementInformationVM vm;
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
,EmployeeName Name
   FROM PreEmployementInformation
WHERE  1=1 AND IsArchive = 0
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new PreEmployementInformationVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
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

        public List<PreEmployementInformationVM> DropDownRef()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<PreEmployementInformationVM> VMs = new List<PreEmployementInformationVM>();
            PreEmployementInformationVM vm;
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
,ReferenceNumber 
,EmployeeName 
   FROM PreEmployementInformation
WHERE  1=1 AND IsArchive = 0
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new PreEmployementInformationVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.ReferenceNumber = dr["ReferenceNumber"].ToString();
                    vm.EmployeeName = dr["EmployeeName"].ToString();
                    vm.Name = vm.ReferenceNumber;
                    vm.Code = vm.ReferenceNumber + "~" + vm.Name;
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
        public List<PreEmployementInformationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PreEmployementInformationVM> VMs = new List<PreEmployementInformationVM>();
            PreEmployementInformationVM vm;
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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement
                #region SqlText

                sqlText = @"
SELECT
 pr.Id
,pr.ReferenceNumber
,pr.IssueDate
,pr.Salutation
,pr.EmployeeName
,pr.ShortName
,pr.Address
,pr.Designation
,pr.Department
,pr.JobGrade
,pr.JobGradeDesignation
,pr.BasicSalary   
,pr.HouseRentAllowance
,pr.MedicalAllowance
,pr.ConveyanceAllowance
,pr.GrossSalary 

,pr.Remarks
,pr.IsActive
,pr.IsArchive
,pr.CreatedBy
,pr.CreatedAt
,pr.CreatedFrom
,pr.LastUpdateBy
,pr.LastUpdateAt
,pr.LastUpdateFrom

FROM PreEmployementInformation pr
WHERE  1=1  AND pr.IsArchive = 0
";
                //ReferenceNumber
                //IssueDate
                //Salutation
                //EmployeeName
                //ShortName
                //Address
                //Designation
                //Department
                //JobGrade
                //JobGradeDesignation
                //BasicSalary
                //HouseRentAllowance
                //MedicalAllowance
                //ConveyanceAllowance
                //GrossSalary



                if (Id > 0)
                {
                    sqlText += @" and pr.Id=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new PreEmployementInformationVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.ReferenceNumber = dr["ReferenceNumber"].ToString();
                    vm.IssueDate = Ordinary.StringToDate(dr["IssueDate"].ToString());
                    vm.Salutation = dr["Salutation"].ToString();
                    vm.EmployeeName = dr["EmployeeName"].ToString();
                    vm.ShortName = dr["ShortName"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.JobGrade = dr["JobGrade"].ToString();
                    vm.JobGradeDesignation = dr["JobGradeDesignation"].ToString();
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.HouseRentAllowance = Convert.ToDecimal(dr["HouseRentAllowance"]);
                    vm.MedicalAllowance = Convert.ToDecimal(dr["MedicalAllowance"]);
                    vm.ConveyanceAllowance = Convert.ToDecimal(dr["ConveyanceAllowance"]);
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);



                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }
        //==================Insert =================
        public string[] Insert(PreEmployementInformationVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertPreEmployementInformation"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
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
                #region Save


                vm.Id = _cDal.NextId("PreEmployementInformation", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO PreEmployementInformation(
Id
,ReferenceNumber
,IssueDate
,Salutation
,EmployeeName
,ShortName
,Address
,Designation
,Department
,JobGrade
,JobGradeDesignation
,BasicSalary
,HouseRentAllowance
,MedicalAllowance
,ConveyanceAllowance
,GrossSalary



,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@ReferenceNumber
,@IssueDate
,@Salutation
,@EmployeeName
,@ShortName
,@Address
,@Designation
,@Department
,@JobGrade
,@JobGradeDesignation
,@BasicSalary
,@HouseRentAllowance
,@MedicalAllowance
,@ConveyanceAllowance
,@GrossSalary
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNumber", vm.ReferenceNumber);
                    cmdInsert.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(vm.IssueDate));
                    cmdInsert.Parameters.AddWithValue("@Salutation", vm.Salutation ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@EmployeeName", vm.EmployeeName);
                    cmdInsert.Parameters.AddWithValue("@ShortName", vm.ShortName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Designation", vm.Designation ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Department", vm.Department ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@JobGrade", vm.JobGrade ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@JobGradeDesignation", vm.JobGradeDesignation ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                    cmdInsert.Parameters.AddWithValue("@HouseRentAllowance", vm.HouseRentAllowance);
                    cmdInsert.Parameters.AddWithValue("@MedicalAllowance", vm.MedicalAllowance);
                    cmdInsert.Parameters.AddWithValue("@ConveyanceAllowance", vm.ConveyanceAllowance);
                    cmdInsert.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);


                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update PreEmployementInformation.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This PreEmployementInformation already used!";
                    throw new ArgumentNullException("Please Input PreEmployementInformation Value", "");
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
                retResults[2] = vm.Id.ToString();
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
        public string[] Update(PreEmployementInformationVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PreEmployementInformationUpdate"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE PreEmployementInformation SET";

                    sqlText += " ReferenceNumber=@ReferenceNumber";
                    sqlText += " , IssueDate=@IssueDate";
                    sqlText += " , Salutation=@Salutation";
                    sqlText += " , EmployeeName=@EmployeeName";
                    sqlText += " , ShortName=@ShortName";
                    sqlText += " , Address=@Address";
                    sqlText += " , Designation=@Designation";
                    sqlText += " , Department=@Department";
                    sqlText += " , JobGrade=@JobGrade";
                    sqlText += " , JobGradeDesignation=@JobGradeDesignation";
                    sqlText += " , BasicSalary=@BasicSalary";
                    sqlText += " , HouseRentAllowance=@HouseRentAllowance";
                    sqlText += " , MedicalAllowance=@MedicalAllowance";
                    sqlText += " , ConveyanceAllowance=@ConveyanceAllowance";
                    sqlText += " , GrossSalary=@GrossSalary";

                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";
                    //ReferenceNumber
                    //IssueDate
                    //Salutation
                    //EmployeeName
                    //ShortName
                    //Address
                    //Designation
                    //Department
                    //JobGrade
                    //JobGradeDesignation
                    //BasicSalary
                    //HouseRentAllowance
                    //MedicalAllowance
                    //ConveyanceAllowance
                    //GrossSalary

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNumber", vm.ReferenceNumber);
                    cmdUpdate.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(vm.IssueDate));
                    cmdUpdate.Parameters.AddWithValue("@Salutation", vm.Salutation ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeName", vm.EmployeeName);
                    cmdUpdate.Parameters.AddWithValue("@ShortName", vm.ShortName ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Designation", vm.Designation ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Department", vm.Department ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@JobGrade", vm.JobGrade ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@JobGradeDesignation", vm.JobGradeDesignation ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                    cmdUpdate.Parameters.AddWithValue("@HouseRentAllowance", vm.HouseRentAllowance);
                    cmdUpdate.Parameters.AddWithValue("@MedicalAllowance", vm.MedicalAllowance);
                    cmdUpdate.Parameters.AddWithValue("@ConveyanceAllowance", vm.ConveyanceAllowance);
                    cmdUpdate.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update PreEmployementInformation.", "");
                    }
                    #endregion SqlExecution

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("PreEmployementInformation Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("PreEmployementInformation Update", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }
        ////==================Delete =================
        public string[] Delete(PreEmployementInformationVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string retVal = "";
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
                if (ids.Length >= 1)
                {

                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update PreEmployementInformation set";
                        sqlText += " IsActive=@IsActive";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " ,LastUpdateBy=@LastUpdateBy";
                        sqlText += " ,LastUpdateAt=@LastUpdateAt";
                        sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("PreEmployementInformation Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("PreEmployementInformation Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }

        //==================Report=================
        public DataTable Report(PreEmployementInformationVM vm, string[] ids, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion
            try
            {
                #region open connection and transaction
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
                #region sql statement
                #region SqlText

                sqlText = @"
SELECT
 pr.Id
,pr.ReferenceNumber
,pr.IssueDate
,pr.Salutation
,pr.EmployeeName
,pr.ShortName
,pr.Address
,pr.Designation
,pr.Department
,pr.JobGrade
,pr.JobGradeDesignation
,pr.BasicSalary   
,pr.HouseRentAllowance
,pr.MedicalAllowance
,pr.ConveyanceAllowance
,pr.GrossSalary 

,pr.Remarks
FROM PreEmployementInformation pr
WHERE  1=1  AND pr.IsArchive = 0
";


                if (ids.Length > 0)
                {
                    string NewIds = "";
                    foreach (var item in ids)
                    {
                        NewIds +=  "'"+item + "',";
                    }
                    NewIds = NewIds.Substring(0, NewIds.Length-1);
                    sqlText += " AND pr.Id IN(" + NewIds + ")";
                }
                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "IssueDate");

                #endregion SqlExecution

                if (transaction != null)
                {
                    transaction.Commit();
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        #endregion


    }
}
