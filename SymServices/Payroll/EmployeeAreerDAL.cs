﻿using SymOrdinary;
using SymServices.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.Payroll
{
    public class EmployeeAreerDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL cdal = new CommonDAL();
        #endregion

        #region Methods
        //==================SelectAll=================

        public List<EmployeeAreerVM> SelectAll(string empid=null, int? fid = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeAreerVM> vms = new List<EmployeeAreerVM>();
            EmployeeAreerVM vm;
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
,ea.AreerAmount
,ea.FiscalYearDetailId
,fs.PeriodName
,ea.AreerDate
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
From EmployeeAreer ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
left outer join  FiscalYearDetail fs on ea.FiscalYearDetailId=fs.Id
Where 1=1 and  ea.IsArchive=0
";
                if (!string.IsNullOrEmpty(empid))
                {
                    sqlText += @" and ea.EmployeeId='" + empid + "'";
                }

                if (fid!=null && fid != 0)
	            {
                sqlText += @" and ea.FiscalYearDetailId='"+fid+"'";
	            }


                sqlText += @" ORDER BY e.Department,e.EmpName desc";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(empid))
                {
                    objCommVehicle.Parameters.AddWithValue("@EmployeeId", empid);
                }
                if (fid != null && fid != 0)
                {
                    objCommVehicle.Parameters.AddWithValue("@FiscalYearDetailId", fid);
                }
               

                SqlDataReader dr;
                dr = objCommVehicle.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeAreerVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.AreerAmount = Convert.ToDecimal(dr["AreerAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.AreerDate = Ordinary.StringToDate(dr["AreerDate"].ToString());

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
        public EmployeeAreerVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeAreerVM vm = new EmployeeAreerVM();

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
,ea.AreerAmount
,ea.FiscalYearDetailId
,ea.AreerDate
,ea.Remarks
,ea.IsActive
,ea.IsArchive
,ea.CreatedBy
,ea.CreatedAt
,ea.CreatedFrom
,ea.LastUpdateBy
,ea.LastUpdateAt
,ea.LastUpdateFrom

From EmployeeAreer ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
Where ea.IsArchive=0  and ea.id=@Id
ORDER BY e.Department, e.EmpName desc
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
                    vm = new EmployeeAreerVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.AreerAmount = Convert.ToDecimal(dr["AreerAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.AreerDate = Ordinary.StringToDate(dr["AreerDate"].ToString());
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

        //==================SelectByID=================
        public EmployeeAreerVM SelectByIdandFiscalyearDetail(string empId, string FiscalYearDetailId = "0")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeAreerVM vm = new EmployeeAreerVM();
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
,ea.AreerAmount
,ea.FiscalYearDetailId
,ea.AreerDate
,ea.Remarks

From EmployeeAreer ea 
Where ea.IsArchive=0  and ea.EmployeeId=@Id and ea.FiscalYearDetailId=@FiscalYearDetailId
 
";
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
                    vm = new EmployeeAreerVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.AreerAmount = Convert.ToDecimal(dr["AreerAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.AreerDate = Ordinary.StringToDate(dr["AreerDate"].ToString());
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
        public string[] Insert(EmployeeAreerVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeAreer"; //Method Name


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
                sqlText += " SELECT   count(Id) FROM EmployeeAreer ";
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
                    sqlText += " delete FROM EmployeeAreer ";
                    sqlText += " WHERE EmployeeId=@EmployeeId AND   FiscalYearDetailId=@FiscalYearDetailId";
                    SqlCommand cmdExistD = new SqlCommand(sqlText, currConn);
                    cmdExistD.Transaction = transaction;
                    cmdExistD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExistD.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                    var exeResD = cmdExistD.ExecuteScalar();
                }

                #endregion Exist
                #region Save
                vm.Id = cdal.NextId("EmployeeAreer", currConn, transaction).ToString();
                if (true)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeAreer(Id
                                ,EmployeeId,AreerAmount,FiscalYearDetailId,AreerDate,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                ) 
                                VALUES (@Id
                                ,@EmployeeId,@AreerAmount,@FiscalYearDetailId,@AreerDate,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                ) ";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);

                    cmdExist1.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist1.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExist1.Parameters.AddWithValue("@AreerAmount", vm.AreerAmount);
                    cmdExist1.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdExist1.Parameters.AddWithValue("@AreerDate", vm.AreerDate);
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
                    retResults[1] = "This EmployeeAreer already exist";
                    throw new ArgumentNullException("Please Input EmployeeAreer Value", "");
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
        public string[] Update(EmployeeAreerVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeAreer Update"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeAreer"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist
                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeeAreer ";
                    sqlText += " WHERE EmployeeId=@EmployeeId AND  Id<>@Id and  AreerDate=@AreerDate";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExist.Parameters.AddWithValue("@AreerDate", Ordinary.DateToString(vm.AreerDate));
					var exeRes = cmdExist.ExecuteScalar();
					int objfoundId = Convert.ToInt32(exeRes);

                    if (objfoundId > 0)
                    {
                        retResults[1] = "Areer Already Exist for this Employee on this period!";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Areer Already Exist for this Employee on this period!", "");
                    }

                    #endregion Exist

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeAreer set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " AreerAmount=@AreerAmount,";
                    sqlText += " FiscalYearDetailId=@FiscalYearDetailId,";
                    sqlText += " AreerDate=@AreerDate,";
                    
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@AreerAmount", vm.AreerAmount);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@AreerDate", vm.AreerDate);
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
                        // throw new ArgumentNullException("Education Update", EmployeeAreerVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeAreer Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update EmployeeAreer.";
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
        public string[] Delete(EmployeeAreerVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeAreer"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeAreer"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeAreer set";
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
                        throw new ArgumentNullException("EmployeeAreer Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeAreer Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete EmployeeAreer Information.";
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
