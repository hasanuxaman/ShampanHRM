using SymOrdinary;
using SymViewModel.HRM;
using SymViewModel.Leave;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Leave
{
  public  class EmployeeLeaveStructureGroupDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        public List<EmployeeStructureGroupVM> SelectAllByEmployee(string employeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeStructureGroupVM> employeeSGs = new List<EmployeeStructureGroupVM>();
            EmployeeStructureGroupVM employeeSG;
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
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,EmployeeGroupId
,LeaveStructureId
,SalaryStructureId
,PFStructureId
,TaxStructureId
,BonusStructureId
,isnull(IsGross,0)IsGross
    FROM EmployeeStructureGroup
Where IsArchive=0 and EmployeeId=@EmployeeId
    ORDER BY Year 
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", employeeId);

                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeSG = new EmployeeStructureGroupVM();
                    employeeSG.Id = Convert.ToInt32(dr["Id"]);
                    employeeSG.EmployeeId = dr["EmployeeId"].ToString();
                    employeeSG.Remarks = dr["Remarks"].ToString();
                    employeeSG.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeSG.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    employeeSG.IsGross = Convert.ToBoolean(dr["IsGross"]);
                    employeeSG.CreatedBy = dr["CreatedBy"].ToString();
                    employeeSG.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeSG.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeSG.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeSG.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeSG.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

                    employeeSG.CreatedBy = dr["EmployeeGroupId"].ToString();
                    employeeSG.CreatedBy = dr["LeaveStructureId"].ToString();
                    employeeSG.CreatedBy = dr["SalaryStructureId"].ToString();
                    employeeSG.CreatedBy = dr["PFStructureId"].ToString();
                    employeeSG.CreatedBy = dr["TaxStructureId"].ToString();
                    employeeSG.CreatedBy = dr["BonusStructureId"].ToString();



                    employeeSGs.Add(employeeSG);
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

            return employeeSGs;
        }
        public bool SelectAllByEmployeeAndYear(string employeeId,int year)
        {
            // if exist then true else false
            #region Variables
            bool returnVal = false;
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeStructureGroupVM> employeeSGs = new List<EmployeeStructureGroupVM>();
            EmployeeStructureGroupVM employeeSG;
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
 count (*)
    FROM EmployeeStructureGroup
Where IsArchive=0 and EmployeeId=@EmployeeId and Year=@Year
    ORDER BY Year 
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", employeeId);
                objComm.Parameters.AddWithValue("@Year", year);
                var exeRes = objComm.ExecuteScalar();
                int a = Convert.ToInt32(exeRes);

                if (a>0)
                {
                    returnVal = true;
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

            return returnVal;
        }
//        public string[] Insert(EmployeeStructureGroupVM vm, SqlConnection currConn, SqlTransaction transaction)
//        {

//            #region Initializ
//            string sqlText = "";
//            int Id = 0;
//            string[] retResults = new string[6];
//            retResults[0] = "Fail";//Success or Fail
//            retResults[1] = "Fail";// Success or Fail Message
//            retResults[2] = Id.ToString();// Return Id
//            retResults[3] = sqlText; //  SQL Query
//            retResults[4] = "ex"; //catch ex
//            retResults[5] = "EmployeeDataInsert"; //Method Name


//            #endregion

//            #region Try

//            try
//            {
//                #region Exist

//                if (SelectAllByEmployeeAndYear(vm.EmployeeId, vm.Year))
//                {
//                    retResults[1] = "[" + vm.Year + "] Leave already assigned";// Success or Fail Message
//                    throw new ArgumentNullException("[" + vm.Year + "] Leave already assigned", "");
//                }
//                #endregion Exist

//                #region open connection and transaction
//                if (currConn == null)
//                {
//                    currConn = _dbsqlConnection.GetConnection();
//                    if (currConn.State != ConnectionState.Open)
//                    {
//                        currConn.Open();
//                    }
//                }
//                if (transaction == null)
//                {
//                    transaction = currConn.BeginTransaction("");
//                }

//                #endregion open connection and transaction


//                sqlText = @"Insert Into EmployeeStructureGroup
//(EmployeeId
//,StructureGroupId
//,Year
//,Remarks
//,IsActive
//,IsArchive
//,CreatedBy
//,CreatedAt
//,CreatedFrom
//) Values (
// @EmployeeId
//,@StructureGroupId
//,@Year
//,@Remarks
//,@IsActive
//,@IsArchive
//,@CreatedBy
//,@CreatedAt
//,@CreatedFrom
//";
//                SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
//                cmdExist1.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
//                cmdExist1.Parameters.AddWithValue("@StructureGroupId", vm.StructureGroupId);
//                cmdExist1.Parameters.AddWithValue("@Year", vm.Year);
//                cmdExist1.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
//                cmdExist1.Parameters.AddWithValue("@IsActive",true);
//                cmdExist1.Parameters.AddWithValue("@IsArchive", false);
//                cmdExist1.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
//                cmdExist1.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
//                cmdExist1.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

//                cmdExist1.Transaction = transaction;
//                cmdExist1.ExecuteNonQuery();
//                #region Detail .............................................................................
//                List<LeaveStructureDetailVM> leaveStructureDetailVMs = new List<LeaveStructureDetailVM>();
//                LeaveStructureDetailVM leaveStructureDetailVM;
//                int LeaveStructureId = 0;

//                sqlText = "select LeaveStructureId from StructureGroup where Id=@Id";
//                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
//                cmd1.Transaction = transaction;
//                cmd1.Parameters.AddWithValue("@Id", vm.StructureGroupId);
//                SqlDataReader dr;
//                dr = cmd1.ExecuteReader();
//                while (dr.Read())
//                {
//                    LeaveStructureId = Convert.ToInt32(dr["LeaveStructureId"]);
//                }
//                dr.Close();
//                if (LeaveStructureId <= 0)
//                {
//                    throw new ArgumentNullException("Leave structure not founded!", "");
//                }

//                sqlText = "select LeaveStructureId,LeaveType_E,LeaveDays,IsEarned,IsCompensation,Remarks from LeaveStructureDetail where IsArchive=0 and LeaveStructureId=@LeaveStructureId";
//                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
//                cmd2.Transaction = transaction;
//                cmd2.Parameters.AddWithValue("@LeaveStructureId", LeaveStructureId);
//                dr = cmd2.ExecuteReader();
//                while (dr.Read())
//                {
//                    leaveStructureDetailVM = new LeaveStructureDetailVM();
//                    leaveStructureDetailVM.LeaveStructureId = Convert.ToInt32(dr["LeaveStructureId"]);
//                    leaveStructureDetailVM.LeaveType_E = dr["LeaveType_E"].ToString();
//                    leaveStructureDetailVM.LeaveDays = dr["LeaveDays"].ToString();
//                    leaveStructureDetailVM.Remarks = dr["Remarks"].ToString();
//                    leaveStructureDetailVM.IsEarned = Convert.ToBoolean(dr["IsEarned"]);
//                    leaveStructureDetailVM.IsCompensation = Convert.ToBoolean(dr["IsCompensation"]);
//                    leaveStructureDetailVMs.Add(leaveStructureDetailVM);
//                }
//                dr.Close();

                
                   

//                    foreach (LeaveStructureDetailVM item in leaveStructureDetailVMs)
//                    {
//                        sqlText = "  ";
//                        sqlText += @" INSERT INTO EmployeeLeaveStructure(	
//EmployeeId,LeaveStructureId,LeaveYear,LeaveType_E,LeaveDays,IsEarned,IsCompensation
//,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)
//                    VALUES (
//@EmployeeId,@LeaveStructureId,@LeaveYear,@LeaveType_E,@LeaveDays,@IsEarned,@IsCompensation
//,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom)
//                    SELECT SCOPE_IDENTITY()";

//                        SqlCommand cmdExist11 = new SqlCommand(sqlText, currConn);
//                        cmdExist11.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

//                        cmdExist11.Parameters.AddWithValue("@LeaveStructureId", item.LeaveStructureId);
//                        cmdExist11.Parameters.AddWithValue("@LeaveYear", vm.Year);
//                        cmdExist11.Parameters.AddWithValue("@LeaveType_E", item.LeaveType_E);
//                        cmdExist11.Parameters.AddWithValue("@LeaveDays", item.LeaveDays);
//                        cmdExist11.Parameters.AddWithValue("@IsEarned", item.IsEarned);
//                        cmdExist11.Parameters.AddWithValue("@IsCompensation", item.IsCompensation);

//                        cmdExist11.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);
//                        cmdExist11.Parameters.AddWithValue("@IsActive", true);
//                        cmdExist11.Parameters.AddWithValue("@IsArchive", false);
//                        cmdExist11.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
//                        cmdExist11.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
//                        cmdExist11.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

//                        cmdExist11.Transaction = transaction;
//                        Id = Convert.ToInt32(cmdExist11.ExecuteScalar());
//                        if (Id <= 0)
//                        {
//                            retResults[1] = "Please Input Employee LeaveStructure Value";
//                            retResults[3] = sqlText;
//                            throw new ArgumentNullException("Please Input Employee LeaveStructure Value", "");
//                        }
//                    }



//                #endregion      .............................................................................



//                #region Commit
//                if (transaction != null)
//                {
//                    transaction.Commit();
//                }

//                #endregion Commit

//                #region SuccessResult

//                retResults[0] = "Success";
//                retResults[1] = "Data Save Successfully";
//                retResults[2] = Id.ToString();

//                #endregion SuccessResult

//            }

//            #endregion try

//            #region Catch and Finall



//            catch (Exception ex)
//            {
//                retResults[4] = ex.Message.ToString(); //catch ex

//                if (Vtransaction == null) { transaction.Rollback(); }
//                return retResults;
//            }

//            finally
//            {
//                if (currConn != null)
//                {
//                    if (currConn.State == ConnectionState.Open)
//                    {
//                        currConn.Close();

//                    }
//                }
//            }


//            #endregion

//            #region Results

//            return retResults;
//            #endregion

//        }

    }
}
