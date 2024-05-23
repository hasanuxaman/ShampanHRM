using SymOrdinary;
using SymServices.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.Payroll
{
    public class EmployeeOverTimeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeOverTimeVM> SelectAll(string empId=null, int? fid = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeOverTimeVM> vms = new List<EmployeeOverTimeVM>();
            EmployeeOverTimeVM vm;
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
,ea.OverTimeAmount
,ea.FiscalYearDetailId
,fs.PeriodName
,ea.OverTimeDate
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

From EmployeeOverTime ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
left outer join  FiscalYearDetail fs on ea.FiscalYearDetailId=fs.Id
Where 1=1 and  ea.IsArchive=0
";
                if (!string.IsNullOrEmpty(empId))
                {
                    sqlText += @" and ea.EmployeeId='" + empId + "'";
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
                SqlDataReader dr;
                if (!string.IsNullOrEmpty(empId))
                {
                    objCommVehicle.Parameters.AddWithValue("@EmployeeId", empId);
                }
                if (fid != null && fid != 0)
                {
                    objCommVehicle.Parameters.AddWithValue("@FiscalYearDetailId", fid);
                }
               
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeOverTimeVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.OverTimeAmount = Convert.ToDecimal(dr["OverTimeAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.OverTimeDate =Ordinary.StringToDate( dr["OverTimeDate"].ToString());
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
        public EmployeeOverTimeVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeOverTimeVM vm = new EmployeeOverTimeVM();

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
,ea.OverTimeAmount
,ea.FiscalYearDetailId
,ea.OverTimeDate
,ea.Remarks
,ea.IsActive
,ea.IsArchive
,ea.CreatedBy
,ea.CreatedAt
,ea.CreatedFrom
,ea.LastUpdateBy
,ea.LastUpdateAt
,ea.LastUpdateFrom

    From EmployeeOverTime ea left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
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
                    vm = new EmployeeOverTimeVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.OverTimeAmount = Convert.ToDecimal(dr["OverTimeAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.OverTimeDate = Ordinary.StringToDate(dr["OverTimeDate"].ToString());
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
        public EmployeeOverTimeVM SelectByIdandFiscalyearDetail(string empId, string FiscalYearDetailId = "0")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeOverTimeVM vm = new EmployeeOverTimeVM();
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
,ea.OverTimeAmount
,ea.FiscalYearDetailId
,ea.OverTimeDate
,ea.Remarks
From EmployeeOverTime ea
Where ea.IsArchive=0  and ea.EmployeeId=@Id and ea.FiscalYearDetailId=@FiscalYearDetailId";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                objCommVehicle.Parameters.AddWithValue("@Id", empId);
                objCommVehicle.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeOverTimeVM();
                    vm.Id =Convert.ToInt32(dr["Id"].ToString());
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.OverTimeAmount = Convert.ToDecimal(dr["OverTimeAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.OverTimeDate = Ordinary.StringToDate(dr["OverTimeDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
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
        public string[] Insert(EmployeeOverTimeVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeOverTime"; //Method Name


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
                sqlText += " SELECT    count(Id) FROM EmployeeOverTime ";
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
                    sqlText += " delete FROM EmployeeOverTime ";
                    sqlText += " WHERE EmployeeId=@EmployeeId AND   FiscalYearDetailId=@FiscalYearDetailId";
                    SqlCommand cmdExistD = new SqlCommand(sqlText, currConn);
                    cmdExistD.Transaction = transaction;
                    cmdExistD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExistD.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    var exeResD = cmdExistD.ExecuteNonQuery();

                }

                #endregion Exist
                #region Save
                CommonDAL cdal = new CommonDAL();
                vm.Id = cdal.NextId("EmployeeOverTime", currConn, transaction);
                if (true)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeOverTime(Id
                                ,EmployeeId,OverTimeAmount,FiscalYearDetailId,OverTimeDate,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                ) 
                                VALUES (@Id
                                ,@EmployeeId,@OverTimeAmount,@FiscalYearDetailId,@OverTimeDate,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                ) 
                                ";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);

                    cmdExist1.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist1.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExist1.Parameters.AddWithValue("@OverTimeAmount", vm.OverTimeAmount);
                    cmdExist1.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdExist1.Parameters.AddWithValue("@OverTimeDate",Ordinary.DateToString( vm.OverTimeDate));
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
                    retResults[1] = "This Employee Overtime already exist";
                    throw new ArgumentNullException("Please Input Employee Overtime Value", "");
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
        public string[] Update(EmployeeOverTimeVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeOverTime Update"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeOverTime");}

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist
                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeeOverTime ";
                    sqlText += " WHERE EmployeeId=@EmployeeId AND  Id<>@Id and  OverTimeDate=@OverTimeDate";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExist.Parameters.AddWithValue("@OverTimeDate", Ordinary.DateToString(vm.OverTimeDate));
					var exeRes = cmdExist.ExecuteScalar();
					int objfoundId = Convert.ToInt32(exeRes);

                    if (objfoundId > 0)
                    {
                        retResults[1] = "Overtime Already Exist for this Employee!";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Overtime Already Exist for this Employee!", "");
                    }

                    #endregion Exist

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeOverTime set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " OverTimeAmount=@OverTimeAmount,";
                    sqlText += " FiscalYearDetailId=@FiscalYearDetailId,";
                    sqlText += " OverTimeDate=@OverTimeDate,";
                    
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@OverTimeAmount", vm.OverTimeAmount);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@OverTimeDate",Ordinary.DateToString( vm.OverTimeDate));
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
                        // throw new ArgumentNullException("Education Update", EmployeeOverTimeVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeOverTime Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update EmployeeOverTime.";
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
        public string[] Delete(EmployeeOverTimeVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeOverTime"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeOverTime"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeOverTime set";
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
                        throw new ArgumentNullException("EmployeeOverTime Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeOverTime Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete EmployeeOverTime Information.";
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
