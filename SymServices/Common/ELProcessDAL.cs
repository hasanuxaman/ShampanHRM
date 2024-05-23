////using Microsoft.Office.Interop.Excel;
using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Excel;
using System.Configuration;
using SymViewModel.Leave;

namespace SymServices.Common
{
    public class ELProcessDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL cdal = new CommonDAL();
        #endregion
        public string[] ELProcess(string attnDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, string InputType = null)
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
            retResults[5] = "InsertELProcess"; //Method Name
            EmployeeInfoDAL _dal = new EmployeeInfoDAL();


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
                decimal ELbalance = Convert.ToDecimal(new SettingDAL().settingValue("HRM", "ELBalance"));
                var fiscalyear = new FiscalYearDAL().FYPeriodDetailPeriodId(Convert.ToDateTime(attnDate).ToString("yyyyMM"));

                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT   count(Id) FROM ELBalanceProcess ";
                sqlText += @" WHERE FiscalYearDetailId=@FiscalYearDetailId and FiscalYear=@FiscalYear
                                ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@FiscalYear", fiscalyear.Year);
                cmdExist.Parameters.AddWithValue("@FiscalYearDetailId", fiscalyear.Id);

                var exeRes = cmdExist.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    sqlText = "  ";
                    sqlText += " delete FROM ELBalanceProcess ";
                    sqlText += @" WHERE FiscalYearDetailId=@FiscalYearDetailId and FiscalYear=@FiscalYear
                                 ";

                    SqlCommand cmdExistD = new SqlCommand(sqlText, currConn);
                    cmdExistD.Transaction = transaction;
                    cmdExistD.Parameters.AddWithValue("@FiscalYear", fiscalyear.Year);
                    cmdExistD.Parameters.AddWithValue("@FiscalYearDetailId", fiscalyear.Id);

                    var exeResD = cmdExistD.ExecuteScalar();
                }

                #endregion Exist

                var ActiveEmp = new EmployeeInfoDAL().SelectActiveEmp();
                DataTable dt = Ordinary.ListToDataTable(ActiveEmp);
                var dataView = new DataView(dt);
                dt = dataView.ToTable(true, "Id");

                dt.Columns["Id"].ColumnName = "EmployeeId";
                dt.Columns.Add(new DataColumn() { ColumnName = "FiscalYearDetailId", DefaultValue = fiscalyear.Id });
                dt.Columns.Add(new DataColumn() { ColumnName = "Balance", DefaultValue = ELbalance });
                dt.Columns.Add(new DataColumn() { ColumnName = "FiscalYear", DefaultValue = fiscalyear.Year });

                string[] result = new CommonDAL().BulkInsert("ELBalanceProcess", dt, currConn, transaction);

                #region Update EL Balance 
               
                    sqlText = "  ";
                    sqlText += @" UPDATE 
    t1
SET 
    t1.LeaveDays = isnull(t2.Balance,0)

FROM 
    EmployeeLeaveStructure t1
   left outer  JOIN (Select EmployeeId,FiscalYear,round(Sum (Balance),2)Balance from ELBalanceProcess 
  group by EmployeeId,FiscalYear)t2 ON t1.EmployeeId=t2.EmployeeId and t1.LeaveYear=t2.FiscalYear
  where t1.LeaveType_E='Annual Leave'
  and LeaveYear=@LeaveYear
                                 ";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.Parameters.AddWithValue("@LeaveYear", fiscalyear.Year);
                    cmdUpdate.ExecuteNonQuery();

                #endregion Update EL Balance 

                  	

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

       
    }
}
