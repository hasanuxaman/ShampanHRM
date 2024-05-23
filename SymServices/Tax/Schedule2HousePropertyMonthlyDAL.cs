using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Tax
{
    public class Schedule2HousePropertyMonthlyDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        #region Methods
        //==================DropDown=================
        public List<Schedule2HousePropertyVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<Schedule2HousePropertyVM> VMs = new List<Schedule2HousePropertyVM>();
            Schedule2HousePropertyVM vm;
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
   FROM Schedule2HousePropertyMonthlies
WHERE  1=1
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new Schedule2HousePropertyVM();
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
        //==================SelectEmployeeList=================
        public List<Schedule2HousePropertyVM> SelectEmployeeList(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<Schedule2HousePropertyVM> VMs = new List<Schedule2HousePropertyVM>();
            Schedule2HousePropertyVM vm;
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
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();

                #region sql statement
                #region SqlText
                sqlText = @"
SELECT DISTINCT 
ve.Id
,shm.EmployeeId
,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,ve.JoinDate
FROM Schedule2HousePropertyMonthlies shm
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fyd ON shm.FiscalYearDetailId=fyd.Id";
                sqlText += "  LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON shm.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND shm.IsArchive = 0
";
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
                sqlText += @" ORDER BY ve.Department, ve.EmpName desc";

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

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new Schedule2HousePropertyVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
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



        //==================SelectFiscalPeriod=================
        public List<Schedule2HousePropertyVM> SelectFiscalPeriod(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<Schedule2HousePropertyVM> VMs = new List<Schedule2HousePropertyVM>();
            Schedule2HousePropertyVM vm;
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
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();

                #region sql statement
                #region SqlText
                sqlText = @"
SELECT DISTINCT fyd.Id
,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd
FROM Schedule2HousePropertyMonthlies shm
";

                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fyd ON shm.FiscalYearDetailId=fyd.Id";
                sqlText += "  LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON shm.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND shm.IsArchive = 0
";
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
                sqlText += @" ORDER BY fyd.PeriodStart";

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

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new Schedule2HousePropertyVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
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





        //==================SelectAll=================
        public List<Schedule2HousePropertyVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<Schedule2HousePropertyVM> VMs = new List<Schedule2HousePropertyVM>();
            Schedule2HousePropertyVM vm;
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
                #region sql statement
                #region SqlText
                sqlText = @"

SELECT
shm.Id
,shm.EmployeeId
,shm.ProjectId
,shm.DepartmentId
,shm.SectionId
,shm.DesignationId
,shm.FiscalYearId
,shm.Year
,shm.FiscalYearDetailId
,shm.Line1
,shm.Line2
,shm.Line3
,shm.Line4
,shm.Line5
,shm.Line6
,shm.Line7
,shm.Line8
,shm.Line9
,shm.Line10
,shm.Line11
,ISNULL(shm.OtherTotal,0)OtherTotal
,ISNULL(shm.NetIncome ,0)NetIncome
,ISNULL(shm.TotalTaxNotPayAmount ,0)TotalTaxNotPayAmount

,shm.Remarks
,shm.IsActive
,shm.IsArchive
,shm.CreatedBy
,shm.CreatedAt
,shm.CreatedFrom
,shm.LastUpdateBy
,shm.LastUpdateAt
,shm.LastUpdateFrom

   FROM Schedule2HousePropertyMonthlies shm
WHERE  1=1  AND shm.IsArchive = 0

";
                if (Id > 0)
                {
                    sqlText += @" and shm.Id=@Id";
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
                    vm = new Schedule2HousePropertyVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.FiscalYearId = dr["FiscalYearId"].ToString();
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);

                    vm.Line1 = Convert.ToDecimal(dr["Line1"]);
                    vm.Line2 = Convert.ToDecimal(dr["Line2"]);
                    vm.Line3 = Convert.ToDecimal(dr["Line3"]);
                    vm.Line4 = Convert.ToDecimal(dr["Line4"]);
                    vm.Line5 = Convert.ToDecimal(dr["Line5"]);
                    vm.Line6 = Convert.ToDecimal(dr["Line6"]);
                    vm.Line7 = Convert.ToDecimal(dr["Line7"]);
                    vm.Line8 = Convert.ToDecimal(dr["Line8"]);
                    vm.Line9 = Convert.ToDecimal(dr["Line9"]);
                    vm.Line10 = Convert.ToDecimal(dr["Line10"]);
                    vm.Line11 = Convert.ToDecimal(dr["Line11"]);
                    vm.OtherTotal = Convert.ToDecimal(dr["OtherTotal"]);
                    vm.NetIncome = Convert.ToDecimal(dr["NetIncome"]);
                    vm.TotalTaxNotPayAmount = Convert.ToDecimal(dr["TotalTaxNotPayAmount"]);

                    //OtherTotal
                    //NetIncome

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
        public string[] Insert(Schedule2HousePropertyVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertSchedule2HousePropertyMonthly"; //Method Name
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
                FiscalYearDAL fiscalYearDAL = new FiscalYearDAL();
                FiscalYearVM fiscalYearVM = new FiscalYearVM();
                fiscalYearVM = fiscalYearDAL.SelectByYear(vm.Year);
                vm.FiscalYearId = fiscalYearVM.Id;


                vm.Id = _cDal.NextId("Schedule2HousePropertyMonthlies", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO Schedule2HousePropertyMonthlies(
Id
,EmployeeId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,FiscalYearId
,Year
,FiscalYearDetailId
,Line1
,Line2
,Line3
,Line4
,Line5
,Line6
,Line7
,Line8
,Line9
,Line10
,Line11
,OtherTotal
,NetIncome
,TotalTaxNotPayAmount
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
,@Line1
,@Line2
,@Line3
,@Line4
,@Line5
,@Line6
,@Line7
,@Line8
,@Line9
,@Line10
,@Line11
,@OtherTotal
,@NetIncome
,@TotalTaxNotPayAmount
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    #endregion SqlText
                    #region SqlExecution

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
                    cmdInsert.Parameters.AddWithValue("@Line1", vm.Line1);
                    cmdInsert.Parameters.AddWithValue("@Line2", vm.Line2);
                    cmdInsert.Parameters.AddWithValue("@Line3", vm.Line3);
                    cmdInsert.Parameters.AddWithValue("@Line4", vm.Line4);
                    cmdInsert.Parameters.AddWithValue("@Line5", vm.Line5);
                    cmdInsert.Parameters.AddWithValue("@Line6", vm.Line6);
                    cmdInsert.Parameters.AddWithValue("@Line7", vm.Line7);
                    cmdInsert.Parameters.AddWithValue("@Line8", vm.Line8);
                    cmdInsert.Parameters.AddWithValue("@Line9", vm.Line9);
                    cmdInsert.Parameters.AddWithValue("@Line10", vm.Line10);
                    cmdInsert.Parameters.AddWithValue("@Line11", vm.Line11);
                    cmdInsert.Parameters.AddWithValue("@OtherTotal", vm.OtherTotal);
                    cmdInsert.Parameters.AddWithValue("@NetIncome", vm.NetIncome);
                    cmdInsert.Parameters.AddWithValue("@TotalTaxNotPayAmount", vm.TotalTaxNotPayAmount);



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
                        throw new ArgumentNullException("Unexpected error to update Schedule2HousePropertyMonthlies.", "");
                    }
                    #endregion SqlExecution

                    #region Insert Into EmployeeSchedule2TaxSlabDetailDAL
                    EmployeeSchedule2TaxSlabDetailMonthlyDAL _employeeSchedule2TaxSlabDetailDAL = new EmployeeSchedule2TaxSlabDetailMonthlyDAL();
                    #region Calculate EmployeeSchedule2TaxSlabDetailDAL
                    if (vm.Schedule2TaxSlabId <= 0)
                    {
                        vm.Schedule2TaxSlabId = 1;
                    }
                    bool isMonth = true;

                    vm.employeeSchedule2TaxSlabDetailVMs = _employeeSchedule2TaxSlabDetailDAL.SelectEmployeeSchedule2TaxSlabDetails(vm.NetIncome, vm.Schedule2TaxSlabId, isMonth, currConn, transaction);
                    #endregion Calculate EmployeeSchedule2TaxSlabDetailDAL

                    if (vm.employeeSchedule2TaxSlabDetailVMs != null && vm.employeeSchedule2TaxSlabDetailVMs.Count > 0)
                    {
                        foreach (EmployeeSchedule2TaxSlabDetailVM EmployeeSchedule2TaxSlabDetailVM in vm.employeeSchedule2TaxSlabDetailVMs)
                        {
                            EmployeeSchedule2TaxSlabDetailVM dVM = new EmployeeSchedule2TaxSlabDetailVM();
                            dVM = EmployeeSchedule2TaxSlabDetailVM;
                            dVM.EmployeeId = vm.EmployeeId;
                            dVM.FiscalYearDetailId = vm.FiscalYearDetailId;
                            dVM.Year = vm.Year;
                            dVM.Schedule2Id = vm.Id;
                            dVM.Schedule2TaxSlabId = vm.Schedule2TaxSlabId;

                            dVM.CreatedAt = vm.CreatedAt;
                            dVM.CreatedBy = vm.CreatedBy;
                            dVM.CreatedFrom = vm.CreatedFrom;
                            retResults = _employeeSchedule2TaxSlabDetailDAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Insert Into EmployeeSchedule2TaxSlabDetailDAL
                    #region Update TotalTaxNotPayAmount In Schedule2HousePropertyMonthlies
                    sqlText = " ";
                    sqlText += @"
UPDATE Schedule2HousePropertyMonthlies SET TotalTaxNotPayAmount=a.TAXAmount
FROM 
(
SELECT DISTINCT FiscalYearDetailId,Schedule2Id,sum(TAXAmount)TAXAmount 
FROM EmployeeSchedule2TaxSlabDetailsMonthlies 
WHERE FiscalYearDetailId=@FiscalYearDetailId
GROUP BY FiscalYearDetailId,Schedule2Id
) AS a 
WHERE a.FiscalYearDetailId=Schedule2HousePropertyMonthlies.FiscalYearDetailId AND a.Schedule2Id=Schedule2HousePropertyMonthlies.Id
AND Schedule2HousePropertyMonthlies.FiscalYearDetailId=@FiscalYearDetailId

";
                    SqlCommand cmdTotalTaxNotPayAmount = new SqlCommand(sqlText, currConn, transaction);
                    cmdTotalTaxNotPayAmount.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    exeRes = cmdTotalTaxNotPayAmount.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Schedule2 Salary Monthlies.", "");
                    }
                    #endregion Update TotalTaxNotPayAmount In Schedule2HousePropertyMonthlies

                }
                else
                {
                    retResults[1] = "This Schedule2HousePropertyMonthly already used!";
                    throw new ArgumentNullException("Please Input Schedule2HousePropertyMonthly Value", "");
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
        public string[] Update(Schedule2HousePropertyVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    #region SqlText

                    sqlText = "";
                    sqlText = "UPDATE Schedule2HousePropertyMonthlies SET";
                    sqlText += "   EmployeeId=@EmployeeId";
                    sqlText += " , ProjectId=@ProjectId";
                    sqlText += " , DepartmentId=@DepartmentId";
                    sqlText += " , SectionId=@SectionId";
                    sqlText += " , DesignationId=@DesignationId";
                    sqlText += " , FiscalYearId=@FiscalYearId";
                    sqlText += " , Year=@Year";
                    sqlText += " , FiscalYearDetailId=@FiscalYearDetailId";
                    sqlText += " , Line1=@Line1";
                    sqlText += " , Line2=@Line2";
                    sqlText += " , Line3=@Line3";
                    sqlText += " , Line4=@Line4";
                    sqlText += " , Line5=@Line5";
                    sqlText += " , Line6=@Line6";
                    sqlText += " , Line7=@Line7";
                    sqlText += " , Line8=@Line8";
                    sqlText += " , Line9=@Line9";
                    sqlText += " , Line10=@Line10";
                    sqlText += " , Line11=@Line11";
                    sqlText += " , OtherTotal=@OtherTotal";
                    sqlText += " , NetIncome=@NetIncome";
                    sqlText += " , TotalTaxNotPayAmount=@TotalTaxNotPayAmount";


                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";
                    #endregion SqlText
                    #region SqlExecution

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdUpdate.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearId", vm.FiscalYearId);
                    cmdUpdate.Parameters.AddWithValue("@Year", vm.Year);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@Line1", vm.Line1);
                    cmdUpdate.Parameters.AddWithValue("@Line2", vm.Line2);
                    cmdUpdate.Parameters.AddWithValue("@Line3", vm.Line3);
                    cmdUpdate.Parameters.AddWithValue("@Line4", vm.Line4);
                    cmdUpdate.Parameters.AddWithValue("@Line5", vm.Line5);
                    cmdUpdate.Parameters.AddWithValue("@Line6", vm.Line6);
                    cmdUpdate.Parameters.AddWithValue("@Line7", vm.Line7);
                    cmdUpdate.Parameters.AddWithValue("@Line8", vm.Line8);
                    cmdUpdate.Parameters.AddWithValue("@Line9", vm.Line9);
                    cmdUpdate.Parameters.AddWithValue("@Line10", vm.Line10);
                    cmdUpdate.Parameters.AddWithValue("@Line11", vm.Line11);
                    cmdUpdate.Parameters.AddWithValue("@OtherTotal", vm.OtherTotal);
                    cmdUpdate.Parameters.AddWithValue("@NetIncome", vm.NetIncome);
                    cmdUpdate.Parameters.AddWithValue("@TotalTaxNotPayAmount", vm.TotalTaxNotPayAmount);


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
                        throw new ArgumentNullException("Unexpected error to update Schedule2HousePropertyMonthlies.", "");
                    }
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #endregion SqlExecution



                    #region Insert Into EmployeeSchedule2TaxSlabDetailDAL
                    EmployeeSchedule2TaxSlabDetailMonthlyDAL _employeeSchedule2TaxSlabDetailDAL = new EmployeeSchedule2TaxSlabDetailMonthlyDAL();


                    #region Calculate EmployeeSchedule2TaxSlabDetailDAL
                    if (vm.Schedule2TaxSlabId <= 0)
                    {
                        vm.Schedule2TaxSlabId = 1;
                    }
                    bool isMonth = true;

                    vm.employeeSchedule2TaxSlabDetailVMs = _employeeSchedule2TaxSlabDetailDAL.SelectEmployeeSchedule2TaxSlabDetails(vm.NetIncome, vm.Schedule2TaxSlabId, isMonth, currConn, transaction);
                    #endregion Calculate EmployeeSchedule2TaxSlabDetailDAL

                    if (vm.employeeSchedule2TaxSlabDetailVMs != null && vm.employeeSchedule2TaxSlabDetailVMs.Count > 0)
                    {
                        #region Delete Detail
                        try
                        {
                            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "EmployeeSchedule2TaxSlabDetailsMonthlies", "Schedule2Id", currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentNullException("Employee Tax Slab Details Monthlies - Unexpected Error", "");
                        }
                        #endregion Delete Detail
                        #region Insert Detail Again
                        foreach (EmployeeSchedule2TaxSlabDetailVM EmployeeSchedule2TaxSlabDetailVM in vm.employeeSchedule2TaxSlabDetailVMs)
                        {
                            EmployeeSchedule2TaxSlabDetailVM dVM = new EmployeeSchedule2TaxSlabDetailVM();
                            dVM = EmployeeSchedule2TaxSlabDetailVM;
                            dVM.EmployeeId = vm.EmployeeId;
                            dVM.FiscalYearDetailId = vm.FiscalYearDetailId;
                            dVM.Year = vm.Year;
                            dVM.Schedule2Id = vm.Id;
                            dVM.Schedule2TaxSlabId = vm.Schedule2TaxSlabId;

                            dVM.CreatedAt = vm.LastUpdateAt;
                            dVM.CreatedBy = vm.LastUpdateBy;
                            dVM.CreatedFrom = vm.LastUpdateFrom;
                            retResults = _employeeSchedule2TaxSlabDetailDAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        #endregion Insert Detail Again

                    }

                    #endregion Insert Into EmployeeSchedule2TaxSlabDetailDAL
                    #region Update TotalTaxNotPayAmount In Schedule2HousePropertyMonthlies
                    sqlText = " ";
                    sqlText += @"
UPDATE Schedule2HousePropertyMonthlies SET TotalTaxNotPayAmount=a.TAXAmount
FROM 
(
SELECT DISTINCT FiscalYearDetailId,Schedule2Id,sum(TAXAmount)TAXAmount 
FROM EmployeeSchedule2TaxSlabDetailsMonthlies 
WHERE FiscalYearDetailId=@FiscalYearDetailId
GROUP BY FiscalYearDetailId,Schedule2Id
) AS a 
WHERE a.FiscalYearDetailId=Schedule2HousePropertyMonthlies.FiscalYearDetailId AND a.Schedule2Id=Schedule2HousePropertyMonthlies.Id
AND Schedule2HousePropertyMonthlies.FiscalYearDetailId=@FiscalYearDetailId

";
                    SqlCommand cmdTotalTaxNotPayAmount = new SqlCommand(sqlText, currConn, transaction);
                    cmdTotalTaxNotPayAmount.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    exeRes = cmdTotalTaxNotPayAmount.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Schedule2 Salary Monthlies.", "");
                    }
                    #endregion Update TotalTaxNotPayAmount In Schedule2HousePropertyMonthlies

                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Schedule2HousePropertyMonthly Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Schedule2HousePropertyMonthly Update", "Could not found any item.");
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
        public string[] Delete(Schedule2HousePropertyVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSchedule2HousePropertyMonthly"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "UPDATE Schedule2HousePropertyMonthlies SET";
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
                        throw new ArgumentNullException("Schedule2HousePropertyMonthly Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Schedule2HousePropertyMonthly Information Delete", "Could not found any item.");
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

        ////==================Report=================
        public DataTable Report(Schedule2HousePropertyVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
                #region sql statement
                #region SqlText
                sqlText = @"
SELECT
shm.Id
,shm.EmployeeId
,shm.ProjectId
,shm.DepartmentId
,shm.SectionId
,shm.DesignationId
,shm.FiscalYearId
,shm.Year
,shm.FiscalYearDetailId
,shm.Line1
,shm.Line2
,shm.Line3
,shm.Line4
,shm.Line5
,shm.Line6
,shm.Line7
,shm.Line8
,shm.Line9
,shm.Line10
,shm.Line11
,ISNULL(shm.OtherTotal,0)OtherTotal
,ISNULL(shm.NetIncome,0) NetIncome
,ISNULL(shm.TotalTaxNotPayAmount,0) TotalTaxNotPayAmount


,shm.Remarks

   FROM Schedule2HousePropertyMonthlies shm
WHERE  1=1  AND shm.IsArchive = 0
";

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


        #endregion Methods

    }
}
