using SymOrdinary;
using SymServices.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.Payroll
{
    public class EmployeeReimbursableExpenseDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeReimbursableExpenseVM> SelectAll(string empID=null, int? fid = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeReimbursableExpenseVM> vms = new List<EmployeeReimbursableExpenseVM>();
            EmployeeReimbursableExpenseVM vm;
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
ea.Id
,ea.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,ea.ReimbursableExpenseAmount
,ea.FiscalYearDetailId
,fs.PeriodName
,ea.ReimbursableExpenseDate
,e.DesignationId, e.DepartmentId, e.SectionId, e.ProjectId
,ea.Remarks
,ea.IsActive
,ea.IsArchive
,ea.CreatedBy
,ea.CreatedAt
,ea.CreatedFrom
,ea.LastUpdateBy
,ea.LastUpdateAt
,ea.LastUpdateFrom

From EmployeeReimbursableExpense ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
left outer join  FiscalYearDetail fs on ea.FiscalYearDetailId=fs.Id

Where 1=1 and  ea.IsArchive=0
";
                if (!string.IsNullOrEmpty(empID))
                {
                    sqlText += @" and ea.EmployeeId='" + empID + "'";
                }
                if (fid != null && fid != 0)
                {
                    sqlText += @" and ea.FiscalYearDetailId='" + fid + "'";
                }
                sqlText += @" 
ORDER BY e.Department,e.EmpName desc";
                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(empID))
                {
                    objCommVehicle.Parameters.AddWithValue("@EmployeeId", empID);
                }
                if (fid != null && fid != 0)
                {
                    objCommVehicle.Parameters.AddWithValue("@FiscalYearDetailId", fid);
                }
                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeReimbursableExpenseVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ReimbursableExpenseAmount = Convert.ToDecimal(dr["ReimbursableExpenseAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodName = dr["PeriodName"].ToString();
                   
                    vm.ReimbursableExpenseDate =Ordinary.StringToDate( dr["ReimbursableExpenseDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = dr["CreatedAt"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = dr["LastUpdateAt"].ToString();
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vms.Add(vm);
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

            return vms;
        }
        //==================SelectByID=================
        public EmployeeReimbursableExpenseVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeReimbursableExpenseVM vm = new EmployeeReimbursableExpenseVM();

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
ea.Id
,ea.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,ea.ReimbursableExpenseAmount
,ea.FiscalYearDetailId
,ea.ReimbursableExpenseDate
,ea.Remarks
,ea.IsActive
,ea.IsArchive
,ea.CreatedBy
,ea.CreatedAt
,ea.CreatedFrom
,ea.LastUpdateBy
,ea.LastUpdateAt
,ea.LastUpdateFrom

From EmployeeReimbursableExpense ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
Where ea.IsArchive=0  and ea.id=@Id
ORDER BY e.Department,e.EmpName desc

";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@Id", Id);

                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeReimbursableExpenseVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ReimbursableExpenseAmount = Convert.ToDecimal(dr["ReimbursableExpenseAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.ReimbursableExpenseDate = Ordinary.StringToDate(dr["ReimbursableExpenseDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = dr["CreatedAt"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = dr["LastUpdateAt"].ToString();
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);

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
        public string[] Insert(EmployeeReimbursableExpenseVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeReimbursableExpense"; //Method Name


            #endregion

            SqlConnection currConn = null;
            SqlTransaction transaction = null;


            #region Try

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

                #region Exist
                sqlText = "  ";
                sqlText += " SELECT  count(Id)  FROM EmployeeReimbursableExpense ";
                sqlText += " WHERE EmployeeId=@EmployeeId AND   FiscalYearDetailId=@FiscalYearDetailId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
				var exeRes = cmdExist.ExecuteScalar();
				int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    sqlText = "  ";
                    sqlText += " delete  FROM EmployeeReimbursableExpense ";
                    sqlText += " WHERE EmployeeId=@EmployeeId AND   FiscalYearDetailId=@FiscalYearDetailId";
                    SqlCommand cmdExistD = new SqlCommand(sqlText, currConn);
                    cmdExistD.Transaction = transaction;
                    cmdExistD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExistD.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    var exeResD = cmdExistD.ExecuteScalar();
                }

                #endregion Exist
                #region Save
                CommonDAL cdal = new CommonDAL();
                vm.Id = cdal.NextId("EmployeeReimbursableExpense", currConn, transaction).ToString();
                //int foundId = (int)objfoundId;
                if (true)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeReimbursableExpense(Id
                                ,EmployeeId,ReimbursableExpenseAmount,FiscalYearDetailId,ReimbursableExpenseDate,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                ) 
                                VALUES (@Id
                                ,@EmployeeId,@ReimbursableExpenseAmount,@FiscalYearDetailId,@ReimbursableExpenseDate,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                ) 
                                ";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);

                    cmdExist1.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist1.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExist1.Parameters.AddWithValue("@ReimbursableExpenseAmount", vm.ReimbursableExpenseAmount);
                    cmdExist1.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdExist1.Parameters.AddWithValue("@ReimbursableExpenseDate",Ordinary.DateToString( vm.ReimbursableExpenseDate));
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
                    retResults[1] = "This Employee Reimbursable Expense already exist";
                    throw new ArgumentNullException("Please Input Employee Reimbursable Expense Value", "");
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
        public string[] Update(EmployeeReimbursableExpenseVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeReimbursableExpense Update"; //Method Name

            int transResult = 0;

            string sqlText = "";

            bool iSTransSuccess = false;

            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeRemExpense"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist
                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeeReimbursableExpense ";
                    sqlText += " WHERE EmployeeId=@EmployeeId AND  Id<>@Id and  ReimbursableExpenseDate=@ReimbursableExpenseDate";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExist.Parameters.AddWithValue("@ReimbursableExpenseDate", Ordinary.DateToString(vm.ReimbursableExpenseDate));
					var exeRes = cmdExist.ExecuteScalar();
					int objfoundId = Convert.ToInt32(exeRes);

                    if (objfoundId > 0)
                    {
                        retResults[1] = "Reimbursable Expense Already Exist for this Employee!";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Reimbursable Expense Already Exist for this Employee!", "");
                    }

                    #endregion Exist

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeReimbursableExpense set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " ReimbursableExpenseAmount=@ReimbursableExpenseAmount,";
                    sqlText += " FiscalYearDetailId=@FiscalYearDetailId,";
                    sqlText += " ReimbursableExpenseDate=@ReimbursableExpenseDate,";
                    
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@ReimbursableExpenseAmount", vm.ReimbursableExpenseAmount);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@ReimbursableExpenseDate",Ordinary.DateToString( vm.ReimbursableExpenseDate));
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EmployeeReimbursableExpenseVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeReimbursableExpense Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update EmployeeReimbursableExpense.";
                    throw new ArgumentNullException("", "");
                }

            }
            #region catch
            catch (Exception ex)
            {
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
        public string[] Delete(EmployeeReimbursableExpenseVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeReimbursableExpense"; //Method Name

            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            bool iSTransSuccess = false;

            #endregion

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeReimbursableExpense"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeReimbursableExpense set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
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
                        throw new ArgumentNullException("EmployeeReimbursableExpense Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeReimbursableExpense Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete EmployeeReimbursableExpense Information.";
                    throw new ArgumentNullException("", "");
                }

            }
            #region catch
            catch (Exception ex)
            {
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
