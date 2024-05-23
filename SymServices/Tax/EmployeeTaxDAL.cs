using Excel;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Tax
{
    public class EmployeeTaxDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion
        #region Methods
        //==================SelectAll=================
        public List<EmployeeTaxVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeTaxVM> EmployeeTaxVMs = new List<EmployeeTaxVM>();
            EmployeeTaxVM EmployeeTaxVM;
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
,TaxStructureId
,EmployeeId
,isnull(TaxValue  ,0)TaxValue
,IsFixed
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeTax
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
                    EmployeeTaxVM = new EmployeeTaxVM();
                    EmployeeTaxVM.Id = dr["Id"].ToString();
                    EmployeeTaxVM.TaxStructureId = dr["TaxStructureId"].ToString();
                    EmployeeTaxVM.EmployeeId = dr["EmployeeId"].ToString();
                    EmployeeTaxVM.TaxValue = Convert.ToDecimal(dr["TaxValue"]);
                    EmployeeTaxVM.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                    EmployeeTaxVM.Remarks = dr["Remarks"].ToString();
                    EmployeeTaxVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    EmployeeTaxVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    EmployeeTaxVM.CreatedBy = dr["CreatedBy"].ToString();
                    EmployeeTaxVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    EmployeeTaxVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    EmployeeTaxVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    EmployeeTaxVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    EmployeeTaxVMs.Add(EmployeeTaxVM);
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

            return EmployeeTaxVMs;
        }
        //==================SelectByID=================
        public EmployeeTaxVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeTaxVM EmployeeTaxVM = new EmployeeTaxVM();

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
,TaxStructureId
,EmployeeId
,isnull(TaxValue  ,0)TaxValue
,IsFixed
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeTax
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
                    EmployeeTaxVM.Id = dr["Id"].ToString();
                    EmployeeTaxVM.TaxStructureId = dr["TaxStructureId"].ToString();
                    EmployeeTaxVM.EmployeeId = dr["EmployeeId"].ToString();
                    EmployeeTaxVM.TaxValue = Convert.ToDecimal(dr["TaxValue"]);
                    EmployeeTaxVM.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                    EmployeeTaxVM.Remarks = dr["Remarks"].ToString();
                    EmployeeTaxVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    EmployeeTaxVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    EmployeeTaxVM.CreatedBy = dr["CreatedBy"].ToString();
                    EmployeeTaxVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    EmployeeTaxVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    EmployeeTaxVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    EmployeeTaxVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return EmployeeTaxVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeeTaxVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeTax"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(EmployeeTaxVM.EmployeeTaxId))
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
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeTax ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", EmployeeTaxVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", EmployeeTaxVM.Name);
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Travel Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Travel Value", "");
                //}
                //#endregion Exist
                #region Exist
                //sqlText = "  ";
                //sqlText += " SELECT  isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) Code FROM EmployeeTax ";
                //sqlText += " WHERE Code=@Code  ";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //var exeRes = cmdExist.ExecuteScalar();
                //int objfoundId = Convert.ToInt32(exeRes);

                //if (objfoundId > 0)
                //{
                //    retResults[1] = "Code already used!";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Code already used!", "");
                //}
                #endregion Exist

                #region Save
                sqlText = "Select  isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0)  from EmployeeTax ";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);

                vm.Id = (count + 1).ToString();
                //int foundId = (int)objfoundId;
                if (vm != null)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeTax(Id,EmployeeId,TaxStructureId,TaxValue ,IsFixed,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Id,@EmployeeId,@TaxStructureId,@TaxValue ,@IsFixed,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        ";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);

                    cmdExist1.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist1.Parameters.AddWithValue("@TaxStructureId", vm.TaxStructureId);
                    cmdExist1.Parameters.AddWithValue("@TaxValue", vm.TaxValue);
                    cmdExist1.Parameters.AddWithValue("@IsFixed", vm.IsFixed);
                    cmdExist1.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExist1.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@IsActive", true);
                    cmdExist1.Parameters.AddWithValue("@IsArchive", false);
                    cmdExist1.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdExist1.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdExist1.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    cmdExist1.Transaction = transaction;
                    cmdExist1.ExecuteNonQuery();


                }
                else
                {
                    retResults[1] = "This EmployeeTax already used";
                    throw new ArgumentNullException("Please Input EmployeeTax Value", "");
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
                retResults[2] = vm.Id;

                #endregion SuccessResult

            }

            #endregion try

            #region Catch and Finally



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
        public string[] Update(EmployeeTaxVM EmployeeTaxVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeTax Update"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;

            string sqlText = "";

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

                transaction = currConn.BeginTransaction("UpdateToEmployeeTax");

                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT  isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) Code FROM EmployeeTax ";
                sqlText += " WHERE Code=@Code AND Id<>@Id ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Id", EmployeeTaxVM.Id);
                var exeRes = cmdExist.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    retResults[1] = "Code already used!";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Code already used!", "");
                }

                #endregion Exist

                if (EmployeeTaxVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeTax set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " TaxStructureId=@TaxStructureId,";
                    sqlText += " TaxValue=@TaxValue,";
                    sqlText += " IsFixed=@IsFixed,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", EmployeeTaxVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeTaxVM.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@TaxStructureId", EmployeeTaxVM.TaxStructureId);
                    cmdUpdate.Parameters.AddWithValue("@TaxValue", EmployeeTaxVM.TaxValue);
                    cmdUpdate.Parameters.AddWithValue("@IsFixed", EmployeeTaxVM.IsFixed);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", EmployeeTaxVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeeTaxVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeeTaxVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeeTaxVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = EmployeeTaxVM.Id.ToString();// Return Id
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
                    retResults[1] = "Could not found any item.";
                    throw new ArgumentNullException("EmployeeTax Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update EmployeeTax.";
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
        //==================Delete =================
        public string[] Delete(EmployeeTaxVM EmployeeTaxVM, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeTax"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            int countId = 0;
            string sqlText = "";

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

                transaction = currConn.BeginTransaction("DeleteToEmployeeTax");

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeTax set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeeTaxVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeeTaxVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeeTaxVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }




                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("EmployeeTax Delete", EmployeeTaxVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeTax Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete EmployeeTax Information.";
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
        public List<EmployeeTaxVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeTaxVM> VMs = new List<EmployeeTaxVM>();
            EmployeeTaxVM vm;
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
Id,
Name
   FROM EmployeeTax
WHERE IsArchive=0 and IsActive=1
    ORDER BY Name
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeTaxVM();
                    vm.Id = dr["Id"].ToString();
                    VMs.Add(vm);
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

            return VMs;
        }
        public List<string> Autocomplete(string term)
        {

            #region Variables

            SqlConnection currConn = null;
            List<string> vms = new List<string>();

            string sqlText = "";

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
                sqlText = "";
                sqlText = @"SELECT Id, Name    FROM EmployeeTax ";
                sqlText += @" WHERE Name like '%" + term + "%' and IsArchive=0 and IsActive=1 ORDER BY Name";



                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["Name"].ToString());
                    i++;
                }
                vms.Sort();
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

            return vms;
        }

        ////==================ExportExcelFile =================
        public DataTable ExportExcelFile(EmployeeTaxVM vm, string[] conFields = null, string[] conValues = null)
        {
            string[] retResults = new string[6];
            DataTable dt = new DataTable();
            SqlTransaction transaction = null;
            try
            {
                #region Variables
                SqlConnection currConn = null;
                string sqlText = "";
                #endregion
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("ExportExcelFile"); }
                #endregion open connection and transaction
                #region DataRead From DB
                #region sql statement
                sqlText = @"
SELECT * FROM (
SELECT 

emab.Code
,emab.EmpName
,ISNULL( etax.TaxValue,0)  TaxValue

,emab.Project
,emab.Department
,emab.Section
,emab.Designation

,ISNULL(tx.Name, 'NA') TaxName
,ISNULL(etax.PortionSalaryType, 'NA') PortionSalaryType
,ISNULL(etax.IsFixed, '0') IsFixed


,emab.EmployeeId
,emab.ProjectId
,emab.DepartmentId
,emab.SectionId
,emab.DesignationId
,ISNULL(etax.TaxStructureId, 0) TaxStructureId

FROM EmployeeTax etax
LEFT OUTER JOIN ViewEmployeeInformation emab ON emab.EmployeeId =ISNULL(etax.EmployeeId,0)
LEFT OUTER JOIN TaxStructure tx ON etax.TaxStructureId = tx.Id
WHERE emab.IsArchive=0 and emab.IsActive=1

UNION ALL

SELECT 
Code
,EmpName
,'0' 


,Project
,Department
,Section
,Designation

,'NA'
,'NA'
,'0'
,EmployeeId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,'0'

FROM ViewEmployeeInformation
WHERE EmployeeId NOT IN(
SELECT DISTINCT EmployeeId FROM EmployeeTax
) 
)as a
WHERE 1=1  and a.EmployeeId <> '1_0'
";
                //string cField = "";
                //if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                //{
                //    for (int i = 0; i < conFields.Length; i++)
                //    {
                //        if (string.IsNullOrWhiteSpace(conFields[i]) || string.IsNullOrWhiteSpace(conValues[i]))
                //        {
                //            continue;
                //        }
                //        cField = conFields[i].ToString();
                //        cField = Ordinary.StringReplacing(cField);
                //        sqlText += " AND " + conFields[i] + "=@" + cField;
                //    }
                //}
                sqlText += " Order By a.Code, a.DepartmentId, a.ProjectId, a.SectionId, a.EmployeeId";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                //if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                //{
                //    for (int j = 0; j < conFields.Length; j++)
                //    {
                //        if (string.IsNullOrWhiteSpace(conFields[j]) || string.IsNullOrWhiteSpace(conValues[j]))
                //        {
                //            continue;
                //        }
                //        cField = conFields[j].ToString();
                //        cField = Ordinary.StringReplacing(cField);
                //        da.SelectCommand.Parameters.AddWithValue("@" + cField, conValues[j]);
                //    }
                //}
                da.Fill(dt);
                #endregion
                dt.Columns.Add("Type");
                if (dt.Rows.Count == 0)
                {
                    throw new ArgumentNullException("Employee Tax has not given to any employee!");
                }
                foreach (DataRow row in dt.Rows)
                {
                    row["Type"] = "Employee Tax";
                }
                #endregion
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                throw ex;
            }
            return dt;
        }

        ////==================ImportExcelFile =================
        public string[] ImportExcelFile(string fullPath, string fileName, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Employee Tax"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region try
            try
            {
                #region Reading Excel
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                FileStream stream = System.IO.File.Open(fullPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (fileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();
                dt = ds.Tables[0];
                reader.Close();
                #endregion Reading Excel
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
                List<EmployeeTaxVM> VMs = new List<EmployeeTaxVM>();
                EmployeeTaxVM vm = new EmployeeTaxVM();

                foreach (DataRow item in dt.Rows)
                {
                    #region CheckPoint
                    #endregion CheckPoint
                    #region Read Data
                    vm = new EmployeeTaxVM();
                    vm.EmployeeId = item["EmployeeId"].ToString();
                    vm.Code = item["Code"].ToString();

                    vm.TaxStructureId = item["TaxStructureId"].ToString();
                    vm.TaxValue = Convert.ToDecimal(item["TaxValue"]);
                    vm.IsFixed = Convert.ToBoolean(item["IsFixed"]);
                    vm.PortionSalaryType = item["PortionSalaryType"].ToString();

                    vm.ProjectId = item["ProjectId"].ToString();
                    vm.DepartmentId = item["DepartmentId"].ToString();
                    vm.SectionId = item["SectionId"].ToString();
                    vm.DesignationId = item["DesignationId"].ToString();


                    vm.CreatedAt = auditvm.CreatedAt;
                    vm.CreatedBy = auditvm.CreatedBy;
                    vm.CreatedFrom = auditvm.CreatedFrom;
                    VMs.Add(vm);
                    #endregion Read Data
                }
                #region Insert Data
                CommonDAL _cDal = new CommonDAL();


                foreach (var item in VMs)
                {
                    retResults = _cDal.DeleteTableByCondition("EmployeeTax", "EmployeeId", item.EmployeeId, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    retResults = Insert(item, currConn, transaction);

                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                }

                #endregion Insert Data
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                //retResults[2] = vm.Id.ToString();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }





        #endregion
    }
}
