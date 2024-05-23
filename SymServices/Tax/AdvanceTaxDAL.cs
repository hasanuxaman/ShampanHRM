using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymServices.Tax
{
    public class AdvanceTaxDAL
    {

        #region Global Variables

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();

        #endregion

        #region Methods
        //==================DropDown=================
        public List<AdvanceTaxVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AdvanceTaxVM> VMs = new List<AdvanceTaxVM>();
            AdvanceTaxVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionTax();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT
Id
   FROM AdvanceTAX
WHERE  1=1
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AdvanceTaxVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    //vm.Code = dr["Code"].ToString();
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
        public List<AdvanceTaxVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<AdvanceTaxVM> VMs = new List<AdvanceTaxVM>();
            AdvanceTaxVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionTax();
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
                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim()_settingDal.settingValue("Database", "HRMDB").Trim();

                #region sql statement
                #region SqlText
                sqlText = @"

SELECT
td.Id
,ve.EmpName EmployeeName
,ve.Code EmployeeCode
,ve.Department
,ve.Section
,ve.Project
,ve.Designation

,td.EmployeeId
,td.EmployeeId
,td.ProjectId
,td.DepartmentId
,td.SectionId
,td.DesignationId
,td.FiscalYearId
,td.Year
,td.FiscalYearDetailId
--,td.ChallanNo
--,td.BankInformation
,td.DepositAmount
,td.DepositDate
,td.Remarks
,td.Particular
,td.IsActive
,td.IsArchive
,td.CreatedBy
,td.CreatedAt
,td.CreatedFrom
,td.LastUpdateBy
,td.LastUpdateAt
,td.LastUpdateFrom

   FROM AdvanceTAX td
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[ViewEmployeeInformation] ve ON td.EmployeeId = ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND td.IsArchive = 0
";
                if (Id > 0)
                {
                    sqlText += @" and td.Id=@Id";
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
                    vm = new AdvanceTaxVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeName = dr["EmployeeName"].ToString();
                    vm.EmployeeCode = dr["EmployeeCode"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Designation = dr["Designation"].ToString();

                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.FiscalYearId = dr["FiscalYearId"].ToString();

                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.FiscalYearId = dr["FiscalYearId"].ToString();
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);

                    ////vm.ChallanNo = dr["ChallanNo"].ToString();
                    ////vm.BankInformation = dr["BankInformation"].ToString();
                    vm.DepositAmount = Convert.ToDecimal(dr["DepositAmount"]);
                    vm.DepositDate = Ordinary.StringToDate(dr["DepositDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Particular = dr["Particular"].ToString();
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
        public string[] Insert(AdvanceTaxVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertTaxDeposit"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionTax();
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
                vm.Id = _cDal.NextId("AdvanceTAX", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO AdvanceTAX(
Id
,EmployeeId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,FiscalYearId
,Year
,FiscalYearDetailId
--,ChallanNo
,Particular
--,BankInformation
,DepositAmount
,DepositDate
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@EmployeeId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@FiscalYearId
,@Year
,@FiscalYearDetailId
--,@ChallanNo
,@Particular
--,@BankInformation
,@DepositAmount
,@DepositDate
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    //ChallanNo
                    //BankInformation
                    //DepositAmount
                    //DepositDate
                    #endregion SqlText
                    #region SqlExecution

                    EmployeeInfoDAL _empInfoDAL = new EmployeeInfoDAL();
                    ViewEmployeeInfoVM empInfoVM = new ViewEmployeeInfoVM();

                    empInfoVM = _empInfoDAL.ViewSelectAllEmployee(vm.EmployeeCode).FirstOrDefault();

                    vm.EmployeeId = empInfoVM.EmployeeId;
                    vm.DesignationId = empInfoVM.DesignationId;
                    vm.DepartmentId = empInfoVM.DepartmentId;
                    vm.SectionId = empInfoVM.SectionId;
                    vm.ProjectId = empInfoVM.ProjectId;


                    FiscalYearDAL fiscalYearDAL = new FiscalYearDAL();
                    FiscalYearVM fiscalYearVM = new FiscalYearVM();
                    fiscalYearVM = fiscalYearDAL.SelectByYear(vm.Year);
                    vm.FiscalYearId = fiscalYearVM.Id;


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearId", vm.FiscalYearId);
                    cmdInsert.Parameters.AddWithValue("@Year", vm.Year);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    ////cmdInsert.Parameters.AddWithValue("@ChallanNo", vm.ChallanNo);
                    ////cmdInsert.Parameters.AddWithValue("@BankInformation", vm.BankInformation);
                    cmdInsert.Parameters.AddWithValue("@DepositAmount", vm.DepositAmount);
                    cmdInsert.Parameters.AddWithValue("@Particular", vm.Particular);
                    cmdInsert.Parameters.AddWithValue("@DepositDate", Ordinary.DateToString(vm.DepositDate));

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
                        throw new ArgumentNullException("Unexpected error to update AdvanceTAX.", "");
                    }
                    #endregion SqlExecution

                }
                else
                {
                    retResults[1] = "This TaxDeposit already used!";
                    throw new ArgumentNullException("Please Input TaxDeposit Value", "");
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
        public string[] Update(AdvanceTaxVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee TaxDeposit Update"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionTax();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToTaxDeposit"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE AdvanceTAX SET";
                    sqlText += "   EmployeeId=@EmployeeId";
                    sqlText += " , ProjectId=@ProjectId";
                    sqlText += " , DepartmentId=@DepartmentId";
                    sqlText += " , SectionId=@SectionId";
                    sqlText += " , DesignationId=@DesignationId";
                    sqlText += " , FiscalYearId=@FiscalYearId";
                    sqlText += " , Year=@Year";
                    sqlText += " , FiscalYearDetailId=@FiscalYearDetailId";
                    ////sqlText += " , ChallanNo=@ChallanNo";
                    ////sqlText += " , BankInformation=@BankInformation";
                    sqlText += " , DepositAmount=@DepositAmount";
                    sqlText += " , DepositDate=@DepositDate";
                    sqlText += " , Particular=@Particular";

                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";


                    EmployeeInfoDAL _empInfoDAL = new EmployeeInfoDAL();
                    ViewEmployeeInfoVM empInfoVM = new ViewEmployeeInfoVM();

                    empInfoVM = _empInfoDAL.ViewSelectAllEmployee(vm.EmployeeCode).FirstOrDefault();

                    vm.EmployeeId = empInfoVM.EmployeeId;
                    vm.DesignationId = empInfoVM.DesignationId;
                    vm.DepartmentId = empInfoVM.DepartmentId;
                    vm.SectionId = empInfoVM.SectionId;
                    vm.ProjectId = empInfoVM.ProjectId;


                    FiscalYearDAL fiscalYearDAL = new FiscalYearDAL();
                    FiscalYearVM fiscalYearVM = new FiscalYearVM();
                    fiscalYearVM = fiscalYearDAL.SelectByYear(vm.Year);
                    vm.FiscalYearId = fiscalYearVM.Id;


                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdUpdate.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearId", vm.FiscalYearId);
                    cmdUpdate.Parameters.AddWithValue("@Year", vm.Year);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    ////cmdUpdate.Parameters.AddWithValue("@ChallanNo", vm.ChallanNo);
                    ////cmdUpdate.Parameters.AddWithValue("@BankInformation", vm.BankInformation);
                    cmdUpdate.Parameters.AddWithValue("@DepositAmount", vm.DepositAmount);
                    cmdUpdate.Parameters.AddWithValue("@DepositDate", Ordinary.DateToString(vm.DepositDate));

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@Particular", vm.Particular);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update AdvanceTAX.", "");
                    }
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("TaxDeposit Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("TaxDeposit Update", "Could not found any item.");
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
        public string[] Delete(AdvanceTaxVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteTaxDeposit"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionTax();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToTaxDeposit"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "UPDATE AdvanceTAX SET";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " WHERE Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("TaxDeposit Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("TaxDeposit Information Delete", "Could not found any item.");
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

        public string[] Delete(AdvanceTaxVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteTaxDeposit"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionTax();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToTaxDeposit"); }
                #endregion open connection and transaction


                #region Sql Statement

                sqlText = @"  delete from [AdvanceTAX] where EmployeeId = @eid and FiscalYearDetailId = @fid and Particular = @pr";

                #endregion


                var cmd = new SqlCommand(sqlText, currConn) { Transaction = transaction };

                cmd.Parameters.AddWithValue("@eid", vm.EmployeeId);
                cmd.Parameters.AddWithValue("@fid", vm.FiscalYearDetailId);
                cmd.Parameters.AddWithValue("@pr", vm.Particular);

                var rows = cmd.ExecuteNonQuery();

                if (VcurrConn == null)
                {
                    transaction.Commit();

                }

                retResults[0] = rows == 1 ? "Success" : "fail";
                retResults[1] = "Data Delete Successfully.";
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

        ////==================Report=================
        public DataTable Report(AdvanceTaxVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionTax();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                string hrmDB = _dbsqlConnection.HRMDB;
                #region sql statement
                #region SqlText
                sqlText = @"
SELECT
td.Id
,ve.EmpName EmployeeName
,ve.Code EmployeeCode
,ve.Department
,ve.Section
,ve.Project
,ve.Designation
,td.EmployeeId
,td.ProjectId
,td.DepartmentId
,td.SectionId
,td.DesignationId
,td.FiscalYearId
,td.Year
,td.FiscalYearDetailId
--,td.ChallanNo
--,td.BankInformation
,td.DepositAmount
,td.DepositDate
,td.Remarks

   FROM AdvanceTAX td
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[ViewEmployeeInformation] ve ON td.EmployeeId = ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND td.IsArchive = 0
";

                //AND td.Fiscalyeardetailid > = '10' AND td.Fiscalyeardetailid <= '12'
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
                dt = Ordinary.DtColumnStringToDate(dt, "DepositDate");

                #endregion SqlExecution

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


        public DataTable GetExcelExportData(string fid, string orderBy, string particular, string EmployeeCode = null, string EmployeeCodeTo = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            var sqlText = "";
            var table = new DataTable();
            #endregion

            try
            {
                string hrmDB = _dbsqlConnection.HRMDB;

                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionTax();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                    // transaction = currConn.BeginTransaction();
                }
                #endregion open connection and transaction

                var tableName = "";

                if (particular.ToLower() == "bonus")
                    tableName = "SalaryBonusDetail";
                else if (particular.ToLower() == "salary")
                    tableName = "SalaryTaxDetail";
                //////else if (particular.ToLower() == "self") tableName = "AdvanceTAX";

                #region sqlText

                if (tableName == "SalaryBonusDetail" || tableName == "SalaryTaxDetail")
                {
                    sqlText += @"
select std.EmployeeId
, ve.Code
, Ve.EmpName [Employee Name]

--, '' ChalanNo
--, '' BankInfo
, std.TaxValue DepositAmount
, '' DepositDate
, '' Remarks
, '" + particular.ToUpper() + @"' Particular  
, fyd.FiscalYearId
, fyd.Year
, std.FiscalYearDetailId
, fyd.PeriodName
from " + hrmDB + ".dbo." + tableName + @" std


LEFT OUTER JOIN " + hrmDB + @".dbo.FiscalYearDetail fyd ON std.FiscalYearDetailId = fyd.Id
LEFT OUTER JOIN " + hrmDB + @".dbo.ViewEmployeeInformation ve ON std.EmployeeId = ve.EmployeeId


WHERE 1=1 AND FiscalYearDetailId = @fid ";


                }

                #region Comments

                //////                else
                //////                {
                //////                    sqlText = @"SELECT 
                //////      td.[EmployeeId]
                //////	  ,ve.Code
                //////      ,Ve.EmpName [Employee Name]
                //////      ,td.[ChallanNo] ChalanNo
                //////      ,td.[BankInformation] BankInfo
                //////      ,td.[DepositAmount]
                //////      ,td.[DepositDate]
                //////      ,td.[Remarks]
                //////      ,td.[Particular]
                //////      ,td.[FiscalYearId]
                //////      ,td.[Year]
                //////      ,td.[FiscalYearDetailId]
                //////      ,fyd.PeriodName
                //////FROM [TAX_DB].[dbo].[AdvanceTAX] td 
                //////left outer join "+hrmDB+@".dbo.ViewEmployeeInformation ve on td.EmployeeId = ve.EmployeeId
                //////left outer join "+hrmDB+@".dbo.FiscalYearDetail fyd ON std.FiscalYearDetailId = fyd.Id
                //////    
                //////WHERE 1=1 AND FiscalYearDetailId = @fid
                //////
                //////";
                //////                }
                #endregion

                if (!string.IsNullOrWhiteSpace(EmployeeCode))
                {
                    sqlText += " and ve.Code>=@EmployeeCode";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeCodeTo))
                {
                    sqlText += " and ve.Code<=@EmployeeCodeTo";
                }

                if (orderBy == "DCG")
                    sqlText += " order by ve.department, ve.code, ve.GradeSl";
                else if (orderBy == "DDC")
                    sqlText += " order by ve.department, ve.JoinDate, ve.code";
                else if (orderBy == "DGC")
                    sqlText += " order by ve.department, ve.GradeSl, ve.code";
                else if (orderBy == "DGDC")
                    sqlText += " order by ve.department, ve.GradeSl, ve.JoinDate, ve.code";
                else if (orderBy == "CODE")
                    sqlText += " order by  ve.code";

                #endregion


                #region sql execution

                var cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@fid", fid);

                if (!string.IsNullOrWhiteSpace(EmployeeCode))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                }
                if (!string.IsNullOrWhiteSpace(EmployeeCodeTo))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCodeTo", EmployeeCodeTo);
                }


                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    row["DepositDate"] = Ordinary.StringToDate(row["DepositDate"].ToString());
                }


                #endregion
            }
            #region catch
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            return table;
        }


        public DataTable GetExcelSelfExportData(string fid, string orderBy, string particular)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            var sqlText = "";
            var table = new DataTable();
            #endregion

            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionTax();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                    // transaction = currConn.BeginTransaction();
                }
                #endregion open connection and transaction

                string hrmDB = _dbsqlConnection.HRMDB;


                #region sqlText



                sqlText = @"SELECT 
      td.[EmployeeId],
	  ve.Code
      ,Ve.EmpName [Employee Name]

      ,[FiscalYearId]
      ,[Year]
      ,[FiscalYearDetailId]
      --,[ChallanNo] ChalanNo
      --,[BankInformation] BankInfo
      ,[DepositAmount]
      ,[DepositDate]
      ,[Remarks]

      ,[Particular]
  FROM [AdvanceTAX] td left outer join " + hrmDB + @".dbo.ViewEmployeeInformation ve
  on td.EmployeeId = ve.EmployeeId
    
WHERE 1=1 AND FiscalYearDetailId = @fid and Particular = @Particular 

";
                if (orderBy == "DCG")
                    sqlText += " order by ve.department, ve.code, ve.GradeSl";
                else if (orderBy == "DDC")
                    sqlText += " order by ve.department, ve.JoinDate, ve.code";
                else if (orderBy == "DGC")
                    sqlText += " order by ve.department, ve.GradeSl, ve.code";
                else if (orderBy == "DGDC")
                    sqlText += " order by ve.department, ve.GradeSl, ve.JoinDate, ve.code";
                else if (orderBy == "CODE")
                    sqlText += " order by  ve.code";

                #endregion


                #region sql execution

                var cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@fid", fid);
                cmd.Parameters.AddWithValue("@Particular", particular);
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    row["DepositDate"] = Ordinary.StringToDate(row["DepositDate"].ToString());
                }


                #endregion
            }
            #region catch
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            return table;
        }


        public string[] ImportData(DataTable table, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertTaxDeposit"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion

            var identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
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
                    currConn = _dbsqlConnection.GetConnectionTax();
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

                foreach (DataRow row in table.Rows)
                {
                    var taxDeposit = new AdvanceTaxVM();

                    ////taxDeposit.BankInformation = row["BankInfo"].ToString();
                    ////taxDeposit.ChallanNo = row["ChalanNo"].ToString();
                    taxDeposit.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    taxDeposit.CreatedBy = identity.Name;
                    taxDeposit.CreatedFrom = identity.WorkStationIP;
                    taxDeposit.DepartmentId = "0";
                    taxDeposit.DepositAmount = Convert.ToDecimal(row["DepositAmount"]);
                    taxDeposit.DepositDate = row["DepositDate"].ToString();
                    taxDeposit.DesignationId = "0";
                    taxDeposit.EmployeeCode = row["Code"].ToString();
                    taxDeposit.EmployeeId = row["EmployeeId"].ToString();
                    taxDeposit.EmployeeName = row["Employee Name"].ToString();
                    taxDeposit.FiscalYearId = row["FiscalYearId"].ToString();
                    taxDeposit.ProjectId = "0";
                    taxDeposit.SectionId = "0";
                    taxDeposit.FiscalYearDetailId = Convert.ToInt32(row["FiscalYearDetailId"]);
                    taxDeposit.Year = Convert.ToInt32(row["Year"]);
                    taxDeposit.Remarks = row["Remarks"].ToString();
                    taxDeposit.Particular = row["Particular"].ToString();

                    retResults = Delete(taxDeposit, currConn, transaction);
                    retResults = Insert(taxDeposit, currConn, transaction);


                }


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
                retResults[2] = "";
                #endregion SuccessResult

                return retResults;
            }

            #region catch and finally

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
        }

        #endregion Methods

    }
}
