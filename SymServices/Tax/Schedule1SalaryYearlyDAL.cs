using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.HRM;
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
    public class Schedule1SalaryYearlyDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        #region Methods
        //==================DropDown=================
        public List<Schedule1SalaryVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<Schedule1SalaryVM> VMs = new List<Schedule1SalaryVM>();
            Schedule1SalaryVM vm;
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
   FROM Schedule1SalaryYearlies
WHERE  1=1
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new Schedule1SalaryVM();
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
        public List<EmployeeInfoVM> SelectEmployeeList(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
,ssy.EmployeeId
,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
FROM Schedule1SalaryYearlies ssy
";
                sqlText += "  LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON ssy.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND ssy.IsArchive = 0
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
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
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

        //==================SelectEmployeeList=================
        public List<EmployeeInfoVM> SelectEmployeeListMonthlies(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
,ssy.EmployeeId
,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
, Format(cast(ve.JoinDate as datetime),'dd-MMM-yyyy')JoinDate
FROM Schedule1SalaryMonthlies ssy
";
                sqlText += "  LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON ssy.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND ssy.IsArchive = 0 and ssy.TransactionType = 'Salary' 
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
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.JoinDate = dr["JoinDate"].ToString();
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

        //==================SelectFiscalYear=================
        public List<Schedule1SalaryVM> SelectFiscalYear(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<Schedule1SalaryVM> VMs = new List<Schedule1SalaryVM>();
            Schedule1SalaryVM vm;
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
SELECT DISTINCT fy.Id FiscalYearId
,fy.Year
,ssy.Remarks 
FROM Schedule1SalaryYearlies ssy
";

                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYear] fy ON ssy.FiscalYearId=fy.Id";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON ssy.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND ssy.IsArchive = 0
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
                sqlText += @" ORDER BY fy.Year";

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
                    vm = new Schedule1SalaryVM();
                    vm.FiscalYearId = dr["FiscalYearId"].ToString();
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.Remarks = dr["Remarks"].ToString();
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

        //==================Select FiscalYear Monthlies=================
        public List<Schedule1SalaryVM> SelectFiscalYearMonthlies(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<Schedule1SalaryVM> VMs = new List<Schedule1SalaryVM>();
            Schedule1SalaryVM vm;
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
SELECT DISTINCT fy.Id FiscalYearId
,fy.Year
,ssy.Remarks 
FROM Schedule1SalaryMonthlies ssy 
";

                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYear] fy ON ssy.FiscalYearId=fy.Id";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON ssy.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND ssy.IsArchive = 0 and fy.Year is not null
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
                sqlText += @" ORDER BY fy.Year";

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
                    vm = new Schedule1SalaryVM();
                    vm.FiscalYearId = dr["FiscalYearId"].ToString();
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.Remarks = dr["Remarks"].ToString();
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
        public List<Schedule1SalaryVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<Schedule1SalaryVM> VMs = new List<Schedule1SalaryVM>();
            Schedule1SalaryVM vm;
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
ssy.Id
,ISNULL(ssy.TaxSlabId, 0)TaxSlabId
,ssy.EmployeeId
,ssy.ProjectId
,ssy.DepartmentId
,ssy.SectionId
,ssy.DesignationId
,ssy.FiscalYearId
,ISNULL(ssy.Year,0) Year
,ISNULL(ssy.Line1A,0)Line1A,ISNULL(ssy.Line1B, 0)Line1B,ISNULL(ssy.Line1C, 0)Line1C,ssy.Line1Remarks
,ISNULL(ssy.Line2A,0)Line2A,ISNULL(ssy.Line2B, 0)Line2B,ISNULL(ssy.Line2C, 0)Line2C,ssy.Line2Remarks
,ISNULL(ssy.Line3A,0)Line3A,ISNULL(ssy.Line3B, 0)Line3B,ISNULL(ssy.Line3C, 0)Line3C,ssy.Line3Remarks
,ISNULL(ssy.Line4A,0)Line4A,ISNULL(ssy.Line4B, 0)Line4B,ISNULL(ssy.Line4C, 0)Line4C,ssy.Line4Remarks
,ISNULL(ssy.Line5A,0)Line5A,ISNULL(ssy.Line5B, 0)Line5B,ISNULL(ssy.Line5C, 0)Line5C,ssy.Line5Remarks
,ISNULL(ssy.Line6A,0)Line6A,ISNULL(ssy.Line6B, 0)Line6B,ISNULL(ssy.Line6C, 0)Line6C,ssy.Line6Remarks
,ISNULL(ssy.Line7A,0)Line7A,ISNULL(ssy.Line7B, 0)Line7B,ISNULL(ssy.Line7C, 0)Line7C,ssy.Line7Remarks
,ISNULL(ssy.Line8A,0)Line8A,ISNULL(ssy.Line8B, 0)Line8B,ISNULL(ssy.Line8C, 0)Line8C,ssy.Line8Remarks
,ISNULL(ssy.Line9A,0)Line9A,ISNULL(ssy.Line9B, 0)Line9B,ISNULL(ssy.Line9C, 0)Line9C,ssy.Line9Remarks
,ISNULL(ssy.Line10A,0)Line10A,ISNULL(ssy.Line10B,0)Line10B,ISNULL(ssy.Line10C,0)Line10C,ssy.Line10Remarks
,ISNULL(ssy.Line11A,0)Line11A,ISNULL(ssy.Line11B,0)Line11B,ISNULL(ssy.Line11C,0)Line11C,ssy.Line11Remarks
,ISNULL(ssy.Line12A,0)Line12A,ISNULL(ssy.Line12B,0)Line12B,ISNULL(ssy.Line12C,0)Line12C,ssy.Line12Remarks
,ISNULL(ssy.Line13A,0)Line13A,ISNULL(ssy.Line13B,0)Line13B,ISNULL(ssy.Line13C,0)Line13C,ssy.Line13Remarks
,ISNULL(ssy.Line14A,0)Line14A,ISNULL(ssy.Line14B,0)Line14B,ISNULL(ssy.Line14C,0)Line14C,ssy.Line14Remarks
,ISNULL(ssy.Line15A,0)Line15A,ISNULL(ssy.Line15B,0)Line15B,ISNULL(ssy.Line15C,0)Line15C,ssy.Line15Remarks
,ISNULL(ssy.Line16A,0)Line16A,ISNULL(ssy.Line16B,0)Line16B,ISNULL(ssy.Line16C,0)Line16C,ssy.Line16Remarks
,ISNULL(ssy.Line17A,0)Line17A,ISNULL(ssy.Line17B,0)Line17B,ISNULL(ssy.Line17C,0)Line17C,ssy.Line17Remarks
,ISNULL(ssy.Line18A,0)Line18A,ISNULL(ssy.Line18B,0)Line18B,ISNULL(ssy.Line18C,0)Line18C,ssy.Line18Remarks
,ISNULL(ssy.Line19A,0)Line19A,ISNULL(ssy.Line19B,0)Line19B,ISNULL(ssy.Line19C,0)Line19C,ssy.Line19Remarks
,ISNULL(ssy.Line20A,0)Line20A,ISNULL(ssy.Line20B,0)Line20B,ISNULL(ssy.Line20C,0)Line20C,ssy.Line20Remarks
,ISNULL(ssy.Line21A,0)Line21A,ISNULL(ssy.Line21B,0)Line21B,ISNULL(ssy.Line21C,0)Line21C,ssy.Line21Remarks
,ISNULL(ssy.Line22A,0)Line22A,ISNULL(ssy.Line22B,0)Line22B,ISNULL(ssy.Line22C,0)Line22C,ssy.Line22Remarks

,ISNULL(ssy.TotalIncomeAmount	,0) TotalIncomeAmount
,ISNULL(ssy.TotalExemptedAmount	,0) TotalExemptedAmount
,ISNULL(ssy.TotalTaxableAmount	,0) TotalTaxableAmount
,ISNULL(ssy.TotalTaxPayAmount	,0) TotalTaxPayAmount
,ssy.Remarks
,ssy.IsActive
,ssy.IsArchive
,ssy.CreatedBy
,ssy.CreatedAt
,ssy.CreatedFrom
,ssy.LastUpdateBy
,ssy.LastUpdateAt
,ssy.LastUpdateFrom

   FROM Schedule1SalaryYearlies ssy
WHERE  1=1  AND ssy.IsArchive = 0


";
                if (Id > 0)
                {
                    sqlText += @" and ssy.Id=@Id";
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
                    vm = new Schedule1SalaryVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.TaxSlabId = Convert.ToInt32(dr["TaxSlabId"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.FiscalYearId = dr["FiscalYearId"].ToString();
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.Line1A = Convert.ToDecimal(dr["Line1A"]);
                    vm.Line1B = Convert.ToDecimal(dr["Line1B"]);
                    vm.Line1C = Convert.ToDecimal(dr["Line1C"]);
                    vm.Line1Remarks = dr["Line1Remarks"].ToString();
                    vm.Line2A = Convert.ToDecimal(dr["Line2A"]);
                    vm.Line2B = Convert.ToDecimal(dr["Line2B"]);
                    vm.Line2C = Convert.ToDecimal(dr["Line2C"]);
                    vm.Line2Remarks = dr["Line2Remarks"].ToString();
                    vm.Line3A = Convert.ToDecimal(dr["Line3A"]);
                    vm.Line3B = Convert.ToDecimal(dr["Line3B"]);
                    vm.Line3C = Convert.ToDecimal(dr["Line3C"]);
                    vm.Line3Remarks = dr["Line3Remarks"].ToString();
                    vm.Line4A = Convert.ToDecimal(dr["Line4A"]);
                    vm.Line4B = Convert.ToDecimal(dr["Line4B"]);
                    vm.Line4C = Convert.ToDecimal(dr["Line4C"]);
                    vm.Line4Remarks = dr["Line4Remarks"].ToString();
                    vm.Line5A = Convert.ToDecimal(dr["Line5A"]);
                    vm.Line5B = Convert.ToDecimal(dr["Line5B"]);
                    vm.Line5C = Convert.ToDecimal(dr["Line5C"]);
                    vm.Line5Remarks = dr["Line5Remarks"].ToString();
                    vm.Line6A = Convert.ToDecimal(dr["Line6A"]);
                    vm.Line6B = Convert.ToDecimal(dr["Line6B"]);
                    vm.Line6C = Convert.ToDecimal(dr["Line6C"]);
                    vm.Line6Remarks = dr["Line6Remarks"].ToString();
                    vm.Line7A = Convert.ToDecimal(dr["Line7A"]);
                    vm.Line7B = Convert.ToDecimal(dr["Line7B"]);
                    vm.Line7C = Convert.ToDecimal(dr["Line7C"]);
                    vm.Line7Remarks = dr["Line7Remarks"].ToString();
                    vm.Line8A = Convert.ToDecimal(dr["Line8A"]);
                    vm.Line8B = Convert.ToDecimal(dr["Line8B"]);
                    vm.Line8C = Convert.ToDecimal(dr["Line8C"]);
                    vm.Line8Remarks = dr["Line8Remarks"].ToString();
                    vm.Line9A = Convert.ToDecimal(dr["Line9A"]);
                    vm.Line9B = Convert.ToDecimal(dr["Line9B"]);
                    vm.Line9C = Convert.ToDecimal(dr["Line9C"]);
                    vm.Line9Remarks = dr["Line9Remarks"].ToString();
                    vm.Line10A = Convert.ToDecimal(dr["Line10A"]);
                    vm.Line10B = Convert.ToDecimal(dr["Line10B"]);
                    vm.Line10C = Convert.ToDecimal(dr["Line10C"]);
                    vm.Line10Remarks = dr["Line10Remarks"].ToString();
                    vm.Line11A = Convert.ToDecimal(dr["Line11A"]);
                    vm.Line11B = Convert.ToDecimal(dr["Line11B"]);
                    vm.Line11C = Convert.ToDecimal(dr["Line11C"]);
                    vm.Line11Remarks = dr["Line11Remarks"].ToString();
                    vm.Line12A = Convert.ToDecimal(dr["Line12A"]);
                    vm.Line12B = Convert.ToDecimal(dr["Line12B"]);
                    vm.Line12C = Convert.ToDecimal(dr["Line12C"]);
                    vm.Line12Remarks = dr["Line12Remarks"].ToString();
                    vm.Line13A = Convert.ToDecimal(dr["Line13A"]);
                    vm.Line13B = Convert.ToDecimal(dr["Line13B"]);
                    vm.Line13C = Convert.ToDecimal(dr["Line13C"]);
                    vm.Line13Remarks = dr["Line13Remarks"].ToString();
                    vm.Line14A = Convert.ToDecimal(dr["Line14A"]);
                    vm.Line14B = Convert.ToDecimal(dr["Line14B"]);
                    vm.Line14C = Convert.ToDecimal(dr["Line14C"]);
                    vm.Line14Remarks = dr["Line14Remarks"].ToString();
                    vm.Line15A = Convert.ToDecimal(dr["Line15A"]);
                    vm.Line15B = Convert.ToDecimal(dr["Line15B"]);
                    vm.Line15C = Convert.ToDecimal(dr["Line15C"]);
                    vm.Line15Remarks = dr["Line15Remarks"].ToString();
                    vm.Line16A = Convert.ToDecimal(dr["Line16A"]);
                    vm.Line16B = Convert.ToDecimal(dr["Line16B"]);
                    vm.Line16C = Convert.ToDecimal(dr["Line16C"]);
                    vm.Line16Remarks = dr["Line16Remarks"].ToString();
                    vm.Line17A = Convert.ToDecimal(dr["Line17A"]);
                    vm.Line17B = Convert.ToDecimal(dr["Line17B"]);
                    vm.Line17C = Convert.ToDecimal(dr["Line17C"]);
                    vm.Line17Remarks = dr["Line17Remarks"].ToString();
                    vm.Line18A = Convert.ToDecimal(dr["Line18A"]);
                    vm.Line18B = Convert.ToDecimal(dr["Line18B"]);
                    vm.Line18C = Convert.ToDecimal(dr["Line18C"]);
                    vm.Line18Remarks = dr["Line18Remarks"].ToString();
                    vm.Line19A = Convert.ToDecimal(dr["Line19A"]);
                    vm.Line19B = Convert.ToDecimal(dr["Line19B"]);
                    vm.Line19C = Convert.ToDecimal(dr["Line19C"]);
                    vm.Line19Remarks = dr["Line19Remarks"].ToString();
                    vm.Line20A = Convert.ToDecimal(dr["Line20A"]);
                    vm.Line20B = Convert.ToDecimal(dr["Line20B"]);
                    vm.Line20C = Convert.ToDecimal(dr["Line20C"]);
                    vm.Line20Remarks = dr["Line20Remarks"].ToString();
                    vm.Line21A = Convert.ToDecimal(dr["Line21A"]);
                    vm.Line21B = Convert.ToDecimal(dr["Line21B"]);
                    vm.Line21C = Convert.ToDecimal(dr["Line21C"]);
                    vm.Line21Remarks = dr["Line21Remarks"].ToString();
                    vm.Line22A = Convert.ToDecimal(dr["Line22A"]);
                    vm.Line22B = Convert.ToDecimal(dr["Line22B"]);
                    vm.Line22C = Convert.ToDecimal(dr["Line22C"]);
                    vm.Line22Remarks = dr["Line22Remarks"].ToString();

                    vm.TotalIncomeAmount = Convert.ToDecimal(dr["TotalIncomeAmount"]);
                    vm.TotalExemptedAmount = Convert.ToDecimal(dr["TotalExemptedAmount"]);
                    vm.TotalTaxableAmount = Convert.ToDecimal(dr["TotalTaxableAmount"]);
                    vm.TotalTaxPayAmount = Convert.ToDecimal(dr["TotalTaxPayAmount"]);

                    SettingDAL _settingDal = new SettingDAL();
                    string Divided = _settingDal.settingValue("Tax", "Divided").Trim();
                    vm.OneThird = Convert.ToDecimal((Convert.ToDecimal(dr["TotalIncomeAmount"]) / Convert.ToDecimal(Divided)).ToString("N2"));

                    vm.RebateAmountYearly = vm.TotalTaxableAmount / 100 * 3;
                    vm.FinalTaxAmount = vm.TotalTaxPayAmount - vm.RebateAmountYearly;
                   


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
        public string[] Insert(Schedule1SalaryVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertSchedule1SalaryYearly"; //Method Name
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
                vm.Id = _cDal.NextId("Schedule1SalaryYearlies", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO Schedule1SalaryYearlies(
TaxSlabId
,EmployeeId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,FiscalYearId
,Year
,Line1A,Line1B,Line1C,Line1Remarks
,Line2A,Line2B,Line2C,Line2Remarks
,Line3A,Line3B,Line3C,Line3Remarks
,Line4A,Line4B,Line4C,Line4Remarks
,Line5A,Line5B,Line5C,Line5Remarks
,Line6A,Line6B,Line6C,Line6Remarks
,Line7A,Line7B,Line7C,Line7Remarks
,Line8A,Line8B,Line8C,Line8Remarks
,Line9A,Line9B,Line9C,Line9Remarks
,Line10A,Line10B,Line10C,Line10Remarks
,Line11A,Line11B,Line11C,Line11Remarks
,Line12A,Line12B,Line12C,Line12Remarks
,Line13A,Line13B,Line13C,Line13Remarks
,Line14A,Line14B,Line14C,Line14Remarks
,Line15A,Line15B,Line15C,Line15Remarks
,Line16A,Line16B,Line16C,Line16Remarks
,Line17A,Line17B,Line17C,Line17Remarks
,Line18A,Line18B,Line18C,Line18Remarks
,Line19A,Line19B,Line19C,Line19Remarks
,Line20A,Line20B,Line20C,Line20Remarks
,Line21A,Line21B,Line21C,Line21Remarks
,Line22A,Line22B,Line22C,Line22Remarks
,TotalIncomeAmount
,TotalExemptedAmount
,TotalTaxableAmount
,TotalTaxPayAmount

,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
 @TaxSlabId
,@EmployeeId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@FiscalYearId
,@Year
,@Line1A,@Line1B,@Line1C,@Line1Remarks
,@Line2A,@Line2B,@Line2C,@Line2Remarks
,@Line3A,@Line3B,@Line3C,@Line3Remarks
,@Line4A,@Line4B,@Line4C,@Line4Remarks
,@Line5A,@Line5B,@Line5C,@Line5Remarks
,@Line6A,@Line6B,@Line6C,@Line6Remarks
,@Line7A,@Line7B,@Line7C,@Line7Remarks
,@Line8A,@Line8B,@Line8C,@Line8Remarks
,@Line9A,@Line9B,@Line9C,@Line9Remarks
,@Line10A,@Line10B,@Line10C,@Line10Remarks
,@Line11A,@Line11B,@Line11C,@Line11Remarks
,@Line12A,@Line12B,@Line12C,@Line12Remarks
,@Line13A,@Line13B,@Line13C,@Line13Remarks
,@Line14A,@Line14B,@Line14C,@Line14Remarks
,@Line15A,@Line15B,@Line15C,@Line15Remarks
,@Line16A,@Line16B,@Line16C,@Line16Remarks
,@Line17A,@Line17B,@Line17C,@Line17Remarks
,@Line18A,@Line18B,@Line18C,@Line18Remarks
,@Line19A,@Line19B,@Line19C,@Line19Remarks
,@Line20A,@Line20B,@Line20C,@Line20Remarks
,@Line21A,@Line21B,@Line21C,@Line21Remarks
,@Line22A,@Line22B,@Line22C,@Line22Remarks
,@TotalIncomeAmount
,@TotalExemptedAmount
,@TotalTaxableAmount
,@TotalTaxPayAmount
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    #endregion SqlText
                    #region SqlExecution

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@TaxSlabId", vm.TaxSlabId);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearId", vm.FiscalYearId);
                    cmdInsert.Parameters.AddWithValue("@Year", vm.Year);
                    cmdInsert.Parameters.AddWithValue("@Line1A", vm.Line1A);
                    cmdInsert.Parameters.AddWithValue("@Line1B", vm.Line1B);
                    cmdInsert.Parameters.AddWithValue("@Line1C", vm.Line1C);
                    cmdInsert.Parameters.AddWithValue("@Line1Remarks", vm.Line1Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line2A", vm.Line2A);
                    cmdInsert.Parameters.AddWithValue("@Line2B", vm.Line2B);
                    cmdInsert.Parameters.AddWithValue("@Line2C", vm.Line2C);
                    cmdInsert.Parameters.AddWithValue("@Line2Remarks", vm.Line2Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line3A", vm.Line3A);
                    cmdInsert.Parameters.AddWithValue("@Line3B", vm.Line3B);
                    cmdInsert.Parameters.AddWithValue("@Line3C", vm.Line3C);
                    cmdInsert.Parameters.AddWithValue("@Line3Remarks", vm.Line3Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line4A", vm.Line4A);
                    cmdInsert.Parameters.AddWithValue("@Line4B", vm.Line4B);
                    cmdInsert.Parameters.AddWithValue("@Line4C", vm.Line4C);
                    cmdInsert.Parameters.AddWithValue("@Line4Remarks", vm.Line4Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line5A", vm.Line5A);
                    cmdInsert.Parameters.AddWithValue("@Line5B", vm.Line5B);
                    cmdInsert.Parameters.AddWithValue("@Line5C", vm.Line5C);
                    cmdInsert.Parameters.AddWithValue("@Line5Remarks", vm.Line5Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line6A", vm.Line6A);
                    cmdInsert.Parameters.AddWithValue("@Line6B", vm.Line6B);
                    cmdInsert.Parameters.AddWithValue("@Line6C", vm.Line6C);
                    cmdInsert.Parameters.AddWithValue("@Line6Remarks", vm.Line6Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line7A", vm.Line7A);
                    cmdInsert.Parameters.AddWithValue("@Line7B", vm.Line7B);
                    cmdInsert.Parameters.AddWithValue("@Line7C", vm.Line7C);
                    cmdInsert.Parameters.AddWithValue("@Line7Remarks", vm.Line7Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line8A", vm.Line8A);
                    cmdInsert.Parameters.AddWithValue("@Line8B", vm.Line8B);
                    cmdInsert.Parameters.AddWithValue("@Line8C", vm.Line8C);
                    cmdInsert.Parameters.AddWithValue("@Line8Remarks", vm.Line8Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line9A", vm.Line9A);
                    cmdInsert.Parameters.AddWithValue("@Line9B", vm.Line9B);
                    cmdInsert.Parameters.AddWithValue("@Line9C", vm.Line9C);
                    cmdInsert.Parameters.AddWithValue("@Line9Remarks", vm.Line9Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line10A", vm.Line10A);
                    cmdInsert.Parameters.AddWithValue("@Line10B", vm.Line10B);
                    cmdInsert.Parameters.AddWithValue("@Line10C", vm.Line10C);
                    cmdInsert.Parameters.AddWithValue("@Line10Remarks", vm.Line10Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line11A", vm.Line11A);
                    cmdInsert.Parameters.AddWithValue("@Line11B", vm.Line11B);
                    cmdInsert.Parameters.AddWithValue("@Line11C", vm.Line11C);
                    cmdInsert.Parameters.AddWithValue("@Line11Remarks", vm.Line11Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line12A", vm.Line12A);
                    cmdInsert.Parameters.AddWithValue("@Line12B", vm.Line12B);
                    cmdInsert.Parameters.AddWithValue("@Line12C", vm.Line12C);
                    cmdInsert.Parameters.AddWithValue("@Line12Remarks", vm.Line12Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line13A", vm.Line13A);
                    cmdInsert.Parameters.AddWithValue("@Line13B", vm.Line13B);
                    cmdInsert.Parameters.AddWithValue("@Line13C", vm.Line13C);
                    cmdInsert.Parameters.AddWithValue("@Line13Remarks", vm.Line13Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line14A", vm.Line14A);
                    cmdInsert.Parameters.AddWithValue("@Line14B", vm.Line14B);
                    cmdInsert.Parameters.AddWithValue("@Line14C", vm.Line14C);
                    cmdInsert.Parameters.AddWithValue("@Line14Remarks", vm.Line14Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line15A", vm.Line15A);
                    cmdInsert.Parameters.AddWithValue("@Line15B", vm.Line15B);
                    cmdInsert.Parameters.AddWithValue("@Line15C", vm.Line15C);
                    cmdInsert.Parameters.AddWithValue("@Line15Remarks", vm.Line15Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line16A", vm.Line16A);
                    cmdInsert.Parameters.AddWithValue("@Line16B", vm.Line16B);
                    cmdInsert.Parameters.AddWithValue("@Line16C", vm.Line16C);
                    cmdInsert.Parameters.AddWithValue("@Line16Remarks", vm.Line16Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line17A", vm.Line17A);
                    cmdInsert.Parameters.AddWithValue("@Line17B", vm.Line17B);
                    cmdInsert.Parameters.AddWithValue("@Line17C", vm.Line17C);
                    cmdInsert.Parameters.AddWithValue("@Line17Remarks", vm.Line17Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line18A", vm.Line18A);
                    cmdInsert.Parameters.AddWithValue("@Line18B", vm.Line18B);
                    cmdInsert.Parameters.AddWithValue("@Line18C", vm.Line18C);
                    cmdInsert.Parameters.AddWithValue("@Line18Remarks", vm.Line18Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line19A", vm.Line19A);
                    cmdInsert.Parameters.AddWithValue("@Line19B", vm.Line19B);
                    cmdInsert.Parameters.AddWithValue("@Line19C", vm.Line19C);
                    cmdInsert.Parameters.AddWithValue("@Line19Remarks", vm.Line19Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line20A", vm.Line20A);
                    cmdInsert.Parameters.AddWithValue("@Line20B", vm.Line20B);
                    cmdInsert.Parameters.AddWithValue("@Line20C", vm.Line20C);
                    cmdInsert.Parameters.AddWithValue("@Line20Remarks", vm.Line20Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line21A", vm.Line21A);
                    cmdInsert.Parameters.AddWithValue("@Line21B", vm.Line21B);
                    cmdInsert.Parameters.AddWithValue("@Line21C", vm.Line21C);
                    cmdInsert.Parameters.AddWithValue("@Line21Remarks", vm.Line21Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Line22A", vm.Line22A);
                    cmdInsert.Parameters.AddWithValue("@Line22B", vm.Line22B);
                    cmdInsert.Parameters.AddWithValue("@Line22C", vm.Line22C);
                    cmdInsert.Parameters.AddWithValue("@Line22Remarks", vm.Line22Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TotalIncomeAmount", vm.TotalIncomeAmount);
                    cmdInsert.Parameters.AddWithValue("@TotalExemptedAmount", vm.TotalExemptedAmount);
                    cmdInsert.Parameters.AddWithValue("@TotalTaxableAmount", vm.TotalTaxableAmount);
                    cmdInsert.Parameters.AddWithValue("@TotalTaxPayAmount", vm.TotalTaxPayAmount);


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
                        throw new ArgumentNullException("Unexpected error to update Schedule1SalaryYearlies.", "");
                    }
                    #endregion SqlExecution

                    #region Insert Into EmployeeTaxSlabDetailDAL
                    EmployeeTaxSlabDetailYearlyDAL _employeeTaxSlabDetailDAL = new EmployeeTaxSlabDetailYearlyDAL();
                    if (vm.employeeTaxSlabDetailVMs != null && vm.employeeTaxSlabDetailVMs.Count > 0)
                    {
                        foreach (EmployeeTaxSlabDetailVM EmployeeTaxSlabDetailVM in vm.employeeTaxSlabDetailVMs)
                        {
                            EmployeeTaxSlabDetailVM dVM = new EmployeeTaxSlabDetailVM();
                            dVM = EmployeeTaxSlabDetailVM;
                            dVM.EmployeeId = vm.EmployeeId;
                            dVM.FiscalYearId = vm.FiscalYearId;
                            dVM.Year = vm.Year;
                            dVM.Schedule1Id = vm.Id;
                            dVM.TaxSlabId = vm.TaxSlabId;

                            dVM.CreatedAt = vm.CreatedAt;
                            dVM.CreatedBy = vm.CreatedBy;
                            dVM.CreatedFrom = vm.CreatedFrom;
                            retResults = _employeeTaxSlabDetailDAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Insert Into EmployeeTaxSlabDetailDAL
                    #region Update TotalTaxPayAmount In Schedule1SalaryYearlies
                    sqlText = " ";
                    sqlText += @"
UPDATE Schedule1SalaryYearlies SET TotalTaxPayAmount=a.TAXAmount
FROM 
(
SELECT DISTINCT Year,Schedule1Id,sum(TAXAmount)TAXAmount 
FROM EmployeeTaxSlabDetailsYearlies 
WHERE Year=@FiscalYear
GROUP BY Year,Schedule1Id
) AS a 
WHERE a.Year=Schedule1SalaryYearlies.Year AND a.Schedule1Id=Schedule1SalaryYearlies.Id
AND Schedule1SalaryYearlies.Year=@FiscalYear

";
                    SqlCommand cmdTotalTaxPayAmount = new SqlCommand(sqlText, currConn, transaction);
                    cmdTotalTaxPayAmount.Parameters.AddWithValue("@FiscalYear", vm.Year);
                    exeRes = cmdTotalTaxPayAmount.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Schedule1 Salary Yearlies.", "");
                    }
                    #endregion Update TotalTaxPayAmount In Schedule1SalaryYearlies

                }
                else
                {
                    retResults[1] = "This Schedule1SalaryYearly already used!";
                    throw new ArgumentNullException("Please Input Schedule1SalaryYearly Value", "");
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
        public string[] Update(Schedule1SalaryVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Schedule1SalaryYearly Update"; //Method Name
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
                    sqlText = "UPDATE Schedule1SalaryYearlies SET";
                    sqlText += "   EmployeeId=@EmployeeId";
                    sqlText += " , TaxSlabId=@TaxSlabId";
                    sqlText += " , ProjectId=@ProjectId";
                    sqlText += " , DepartmentId=@DepartmentId";
                    sqlText += " , SectionId=@SectionId";
                    sqlText += " , DesignationId=@DesignationId";
                    sqlText += " , FiscalYearId=@FiscalYearId";
                    sqlText += " , Year=@Year";
                    sqlText += " , Line1A=@Line1A";
                    sqlText += " , Line1B=@Line1B";
                    sqlText += " , Line1C=@Line1C";
                    sqlText += " , Line1Remarks=@Line1Remarks";
                    sqlText += " , Line2A=@Line2A";
                    sqlText += " , Line2B=@Line2B";
                    sqlText += " , Line2C=@Line2C";
                    sqlText += " , Line2Remarks=@Line2Remarks";
                    sqlText += " , Line3A=@Line3A";
                    sqlText += " , Line3B=@Line3B";
                    sqlText += " , Line3C=@Line3C";
                    sqlText += " , Line3Remarks=@Line3Remarks";
                    sqlText += " , Line4A=@Line4A";
                    sqlText += " , Line4B=@Line4B";
                    sqlText += " , Line4C=@Line4C";
                    sqlText += " , Line4Remarks=@Line4Remarks";
                    sqlText += " , Line5A=@Line5A";
                    sqlText += " , Line5B=@Line5B";
                    sqlText += " , Line5C=@Line5C";
                    sqlText += " , Line5Remarks=@Line5Remarks";
                    sqlText += " , Line6A=@Line6A";
                    sqlText += " , Line6B=@Line6B";
                    sqlText += " , Line6C=@Line6C";
                    sqlText += " , Line6Remarks=@Line6Remarks";
                    sqlText += " , Line7A=@Line7A";
                    sqlText += " , Line7B=@Line7B";
                    sqlText += " , Line7C=@Line7C";
                    sqlText += " , Line7Remarks=@Line7Remarks";
                    sqlText += " , Line8A=@Line8A";
                    sqlText += " , Line8B=@Line8B";
                    sqlText += " , Line8C=@Line8C";
                    sqlText += " , Line8Remarks=@Line8Remarks";
                    sqlText += " , Line9A=@Line9A";
                    sqlText += " , Line9B=@Line9B";
                    sqlText += " , Line9C=@Line9C";
                    sqlText += " , Line9Remarks=@Line9Remarks";
                    sqlText += " , Line10A=@Line10A";
                    sqlText += " , Line10B=@Line10B";
                    sqlText += " , Line10C=@Line10C";
                    sqlText += " , Line10Remarks=@Line10Remarks";
                    sqlText += " , Line11A=@Line11A";
                    sqlText += " , Line11B=@Line11B";
                    sqlText += " , Line11C=@Line11C";
                    sqlText += " , Line11Remarks=@Line11Remarks";
                    sqlText += " , Line12A=@Line12A";
                    sqlText += " , Line12B=@Line12B";
                    sqlText += " , Line12C=@Line12C";
                    sqlText += " , Line12Remarks=@Line12Remarks";
                    sqlText += " , Line13A=@Line13A";
                    sqlText += " , Line13B=@Line13B";
                    sqlText += " , Line13C=@Line13C";
                    sqlText += " , Line13Remarks=@Line13Remarks";
                    sqlText += " , Line14A=@Line14A";
                    sqlText += " , Line14B=@Line14B";
                    sqlText += " , Line14C=@Line14C";
                    sqlText += " , Line14Remarks=@Line14Remarks";
                    sqlText += " , Line15A=@Line15A";
                    sqlText += " , Line15B=@Line15B";
                    sqlText += " , Line15C=@Line15C";
                    sqlText += " , Line15Remarks=@Line15Remarks";
                    sqlText += " , Line16A=@Line16A";
                    sqlText += " , Line16B=@Line16B";
                    sqlText += " , Line16C=@Line16C";
                    sqlText += " , Line16Remarks=@Line16Remarks";
                    sqlText += " , Line17A=@Line17A";
                    sqlText += " , Line17B=@Line17B";
                    sqlText += " , Line17C=@Line17C";
                    sqlText += " , Line17Remarks=@Line17Remarks";
                    sqlText += " , Line18A=@Line18A";
                    sqlText += " , Line18B=@Line18B";
                    sqlText += " , Line18C=@Line18C";
                    sqlText += " , Line18Remarks=@Line18Remarks";
                    sqlText += " , Line19A=@Line19A";
                    sqlText += " , Line19B=@Line19B";
                    sqlText += " , Line19C=@Line19C";
                    sqlText += " , Line19Remarks=@Line19Remarks";
                    sqlText += " , Line20A=@Line20A";
                    sqlText += " , Line20B=@Line20B";
                    sqlText += " , Line20C=@Line20C";
                    sqlText += " , Line20Remarks=@Line20Remarks";
                    sqlText += " , Line21A=@Line21A";
                    sqlText += " , Line21B=@Line21B";
                    sqlText += " , Line21C=@Line21C";
                    sqlText += " , Line21Remarks=@Line21Remarks";
                    sqlText += " , Line22A=@Line22A";
                    sqlText += " , Line22B=@Line22B";
                    sqlText += " , Line22C=@Line22C";
                    sqlText += " , Line22Remarks=@Line22Remarks";
                    sqlText += " , TotalIncomeAmount=@TotalIncomeAmount";
                    sqlText += " , TotalExemptedAmount=@TotalExemptedAmount";
                    sqlText += " , TotalTaxableAmount=@TotalTaxableAmount";
                    sqlText += " , TotalTaxPayAmount=@TotalTaxPayAmount";


                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";
                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@TaxSlabId", vm.TaxSlabId);
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdUpdate.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearId", vm.FiscalYearId);
                    cmdUpdate.Parameters.AddWithValue("@Year", vm.Year);
                    cmdUpdate.Parameters.AddWithValue("@Line1A", vm.Line1A);
                    cmdUpdate.Parameters.AddWithValue("@Line1B", vm.Line1B);
                    cmdUpdate.Parameters.AddWithValue("@Line1C", vm.Line1C);
                    cmdUpdate.Parameters.AddWithValue("@Line1Remarks", vm.Line1Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line2A", vm.Line2A);
                    cmdUpdate.Parameters.AddWithValue("@Line2B", vm.Line2B);
                    cmdUpdate.Parameters.AddWithValue("@Line2C", vm.Line2C);
                    cmdUpdate.Parameters.AddWithValue("@Line2Remarks", vm.Line2Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line3A", vm.Line3A);
                    cmdUpdate.Parameters.AddWithValue("@Line3B", vm.Line3B);
                    cmdUpdate.Parameters.AddWithValue("@Line3C", vm.Line3C);
                    cmdUpdate.Parameters.AddWithValue("@Line3Remarks", vm.Line3Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line4A", vm.Line4A);
                    cmdUpdate.Parameters.AddWithValue("@Line4B", vm.Line4B);
                    cmdUpdate.Parameters.AddWithValue("@Line4C", vm.Line4C);
                    cmdUpdate.Parameters.AddWithValue("@Line4Remarks", vm.Line4Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line5A", vm.Line5A);
                    cmdUpdate.Parameters.AddWithValue("@Line5B", vm.Line5B);
                    cmdUpdate.Parameters.AddWithValue("@Line5C", vm.Line5C);
                    cmdUpdate.Parameters.AddWithValue("@Line5Remarks", vm.Line5Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line6A", vm.Line6A);
                    cmdUpdate.Parameters.AddWithValue("@Line6B", vm.Line6B);
                    cmdUpdate.Parameters.AddWithValue("@Line6C", vm.Line6C);
                    cmdUpdate.Parameters.AddWithValue("@Line6Remarks", vm.Line6Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line7A", vm.Line7A);
                    cmdUpdate.Parameters.AddWithValue("@Line7B", vm.Line7B);
                    cmdUpdate.Parameters.AddWithValue("@Line7C", vm.Line7C);
                    cmdUpdate.Parameters.AddWithValue("@Line7Remarks", vm.Line7Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line8A", vm.Line8A);
                    cmdUpdate.Parameters.AddWithValue("@Line8B", vm.Line8B);
                    cmdUpdate.Parameters.AddWithValue("@Line8C", vm.Line8C);
                    cmdUpdate.Parameters.AddWithValue("@Line8Remarks", vm.Line8Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line9A", vm.Line9A);
                    cmdUpdate.Parameters.AddWithValue("@Line9B", vm.Line9B);
                    cmdUpdate.Parameters.AddWithValue("@Line9C", vm.Line9C);
                    cmdUpdate.Parameters.AddWithValue("@Line9Remarks", vm.Line9Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line10A", vm.Line10A);
                    cmdUpdate.Parameters.AddWithValue("@Line10B", vm.Line10B);
                    cmdUpdate.Parameters.AddWithValue("@Line10C", vm.Line10C);
                    cmdUpdate.Parameters.AddWithValue("@Line10Remarks", vm.Line10Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line11A", vm.Line11A);
                    cmdUpdate.Parameters.AddWithValue("@Line11B", vm.Line11B);
                    cmdUpdate.Parameters.AddWithValue("@Line11C", vm.Line11C);
                    cmdUpdate.Parameters.AddWithValue("@Line11Remarks", vm.Line11Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line12A", vm.Line12A);
                    cmdUpdate.Parameters.AddWithValue("@Line12B", vm.Line12B);
                    cmdUpdate.Parameters.AddWithValue("@Line12C", vm.Line12C);
                    cmdUpdate.Parameters.AddWithValue("@Line12Remarks", vm.Line12Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line13A", vm.Line13A);
                    cmdUpdate.Parameters.AddWithValue("@Line13B", vm.Line13B);
                    cmdUpdate.Parameters.AddWithValue("@Line13C", vm.Line13C);
                    cmdUpdate.Parameters.AddWithValue("@Line13Remarks", vm.Line13Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line14A", vm.Line14A);
                    cmdUpdate.Parameters.AddWithValue("@Line14B", vm.Line14B);
                    cmdUpdate.Parameters.AddWithValue("@Line14C", vm.Line14C);
                    cmdUpdate.Parameters.AddWithValue("@Line14Remarks", vm.Line14Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line15A", vm.Line15A);
                    cmdUpdate.Parameters.AddWithValue("@Line15B", vm.Line15B);
                    cmdUpdate.Parameters.AddWithValue("@Line15C", vm.Line15C);
                    cmdUpdate.Parameters.AddWithValue("@Line15Remarks", vm.Line15Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line16A", vm.Line16A);
                    cmdUpdate.Parameters.AddWithValue("@Line16B", vm.Line16B);
                    cmdUpdate.Parameters.AddWithValue("@Line16C", vm.Line16C);
                    cmdUpdate.Parameters.AddWithValue("@Line16Remarks", vm.Line16Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line17A", vm.Line17A);
                    cmdUpdate.Parameters.AddWithValue("@Line17B", vm.Line17B);
                    cmdUpdate.Parameters.AddWithValue("@Line17C", vm.Line17C);
                    cmdUpdate.Parameters.AddWithValue("@Line17Remarks", vm.Line17Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line18A", vm.Line18A);
                    cmdUpdate.Parameters.AddWithValue("@Line18B", vm.Line18B);
                    cmdUpdate.Parameters.AddWithValue("@Line18C", vm.Line18C);
                    cmdUpdate.Parameters.AddWithValue("@Line18Remarks", vm.Line18Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line19A", vm.Line19A);
                    cmdUpdate.Parameters.AddWithValue("@Line19B", vm.Line19B);
                    cmdUpdate.Parameters.AddWithValue("@Line19C", vm.Line19C);
                    cmdUpdate.Parameters.AddWithValue("@Line19Remarks", vm.Line19Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line20A", vm.Line20A);
                    cmdUpdate.Parameters.AddWithValue("@Line20B", vm.Line20B);
                    cmdUpdate.Parameters.AddWithValue("@Line20C", vm.Line20C);
                    cmdUpdate.Parameters.AddWithValue("@Line20Remarks", vm.Line20Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line21A", vm.Line21A);
                    cmdUpdate.Parameters.AddWithValue("@Line21B", vm.Line21B);
                    cmdUpdate.Parameters.AddWithValue("@Line21C", vm.Line21C);
                    cmdUpdate.Parameters.AddWithValue("@Line21Remarks", vm.Line21Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line22A", vm.Line22A);
                    cmdUpdate.Parameters.AddWithValue("@Line22B", vm.Line22B);
                    cmdUpdate.Parameters.AddWithValue("@Line22C", vm.Line22C);
                    cmdUpdate.Parameters.AddWithValue("@Line22Remarks", vm.Line22Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TotalIncomeAmount", vm.TotalIncomeAmount);
                    cmdUpdate.Parameters.AddWithValue("@TotalExemptedAmount", vm.TotalExemptedAmount);

                    cmdUpdate.Parameters.AddWithValue("@TotalTaxableAmount", vm.TotalTaxableAmount);

                    cmdUpdate.Parameters.AddWithValue("@TotalTaxPayAmount", vm.TotalTaxPayAmount);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Schedule1SalaryYearlies.", "");
                    }
                    #endregion SqlExecution

                    #region Insert Into EmployeeTaxSlabDetailDAL
                    EmployeeTaxSlabDetailYearlyDAL _employeeTaxSlabDetailDAL = new EmployeeTaxSlabDetailYearlyDAL();
                    if (vm.employeeTaxSlabDetailVMs != null && vm.employeeTaxSlabDetailVMs.Count > 0)
                    {
                        #region Delete Detail
                        try
                        {
                            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "EmployeeTaxSlabDetailsYearlies", "Schedule1Id", currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentNullException("Employee Tax Slab Details Yearlies - Unexpected Error", "");
                        }
                        #endregion Delete Detail
                        #region Insert Detail Again
                        foreach (EmployeeTaxSlabDetailVM EmployeeTaxSlabDetailVM in vm.employeeTaxSlabDetailVMs)
                        {
                            EmployeeTaxSlabDetailVM dVM = new EmployeeTaxSlabDetailVM();
                            dVM = EmployeeTaxSlabDetailVM;
                            dVM.EmployeeId = vm.EmployeeId;
                            dVM.FiscalYearId = vm.FiscalYearId;
                            dVM.Year = vm.Year;
                            dVM.Schedule1Id = vm.Id;
                            dVM.TaxSlabId = vm.TaxSlabId;

                            dVM.CreatedAt = vm.LastUpdateAt;
                            dVM.CreatedBy = vm.LastUpdateBy;
                            dVM.CreatedFrom = vm.LastUpdateFrom;
                            retResults = _employeeTaxSlabDetailDAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        #endregion Insert Detail Again

                    }

                    #endregion Insert Into EmployeeTaxSlabDetailDAL
                    #region Update TotalTaxPayAmount In Schedule1SalaryYearlies
                    sqlText = " ";
                    sqlText += @"
UPDATE Schedule1SalaryYearlies SET TotalTaxPayAmount=a.TAXAmount
FROM 
(
SELECT DISTINCT Year,Schedule1Id,sum(TAXAmount)TAXAmount 
FROM EmployeeTaxSlabDetailsYearlies 
WHERE Year=@FiscalYear
GROUP BY Year,Schedule1Id
) AS a 
WHERE a.Year=Schedule1SalaryYearlies.Year AND a.Schedule1Id=Schedule1SalaryYearlies.Id
AND Schedule1SalaryYearlies.Year=@FiscalYear

";
                    SqlCommand cmdTotalTaxPayAmount = new SqlCommand(sqlText, currConn, transaction);
                    cmdTotalTaxPayAmount.Parameters.AddWithValue("@FiscalYear", vm.Year);
                    exeRes = cmdTotalTaxPayAmount.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Schedule1 Salary Yearlies.", "");
                    }
                    #endregion Update TotalTaxPayAmount In Schedule1SalaryYearlies
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Schedule1SalaryYearly Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Schedule1SalaryYearly Update", "Could not found any item.");
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
        public string[] Delete(Schedule1SalaryVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSchedule1SalaryYearly"; //Method Name
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
                        sqlText = "UPDATE Schedule1SalaryYearlies SET";
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
                        throw new ArgumentNullException("Schedule1SalaryYearly Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Schedule1SalaryYearly Information Delete", "Could not found any item.");
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
        public DataTable Report(Schedule1SalaryVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                #region open connection and transaction
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();
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
ssy.Id
,ssy.EmployeeId
,ssy.TaxSlabId
,ssy.ProjectId
,ssy.DepartmentId
,ssy.SectionId
,ssy.DesignationId
,ssy.FiscalYearId
,ssy.Year
,ssy.Line1A,ssy.Line1B,ssy.Line1C,ssy.Line1Remarks
,ssy.Line2A,ssy.Line2B,ssy.Line2C,ssy.Line2Remarks
,ssy.Line3A,ssy.Line3B,ssy.Line3C,ssy.Line3Remarks
,ssy.Line4A,ssy.Line4B,ssy.Line4C,ssy.Line4Remarks
,ssy.Line5A,ssy.Line5B,ssy.Line5C,ssy.Line5Remarks
,ssy.Line6A,ssy.Line6B,ssy.Line6C,ssy.Line6Remarks
,ssy.Line7A,ssy.Line7B,ssy.Line7C,ssy.Line7Remarks
,ssy.Line8A,ssy.Line8B,ssy.Line8C,ssy.Line8Remarks
,ssy.Line9A,ssy.Line9B,ssy.Line9C,ssy.Line9Remarks
,ssy.Line10A,ssy.Line10B,ssy.Line10C,ssy.Line10Remarks
,ssy.Line11A,ssy.Line11B,ssy.Line11C,ssy.Line11Remarks
,ssy.Line12A,ssy.Line12B,ssy.Line12C,ssy.Line12Remarks
,ssy.Line13A,ssy.Line13B,ssy.Line13C,ssy.Line13Remarks
,ssy.Line14A,ssy.Line14B,ssy.Line14C,ssy.Line14Remarks
,ssy.Line15A,ssy.Line15B,ssy.Line15C,ssy.Line15Remarks
,ssy.Line16A,ssy.Line16B,ssy.Line16C,ssy.Line16Remarks
,ssy.Line17A,ssy.Line17B,ssy.Line17C,ssy.Line17Remarks
,ssy.Line18A,ssy.Line18B,ssy.Line18C,ssy.Line18Remarks
,ssy.Line19A,ssy.Line19B,ssy.Line19C,ssy.Line19Remarks
,ssy.Line20A,ssy.Line20B,ssy.Line20C,ssy.Line20Remarks
,ssy.Line21A,ssy.Line21B,ssy.Line21C,ssy.Line21Remarks
,ssy.Line22A,ssy.Line22B,ssy.Line22C,ssy.Line22Remarks
,ssy.TotalIncomeAmount
,ssy.TotalExemptedAmount
,ssy.TotalTaxableAmount
,ssy.TotalTaxPayAmount
,ssy.Remarks

   FROM Schedule1SalaryYearlies ssy
LEFT OUTER JOIN "+hrmDB+".[dbo].[ViewEmployeeInformation] ve on ve.EmployeeId=ssy.EmployeeId WHERE  1=1 AND ssy.IsArchive = 0 ";


                //TotalIncomeAmount
                //TotalExemptedAmount
                //TotalTaxableAmount
                //TotalTaxPayAmount

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

        #region Process
        //==================InsertProcessUpdate =================
        public string[] InsertProcessUpdate(string FiscalYear, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertProcessUpdate"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                    currConn = _dbsqlConnection.GetConnectionTax();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction
                bool isMonth = false;
                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();
                #region Insert
                retResults = InsertFromSchedule1SalaryMonthly(FiscalYear, auditvm, currConn, transaction);

                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], "");
                }
                #endregion Insert
                #region Process
                #region Select Inserted Data
                Schedule1SalaryVM ssVM = new Schedule1SalaryVM();
                List<Schedule1SalaryVM> ssVMs = new List<Schedule1SalaryVM>();
                List<Schedule1SalaryVM> updatedSSVMs = new List<Schedule1SalaryVM>();
                string[] conditionFields = { "ssy.Year" };
                string[] conditionValues = { FiscalYear };
                string LineA = "";
                ssVMs = SelectAll(0, conditionFields, conditionValues, currConn, transaction);
               
                #endregion Select Inserted Data
                #region Ready For Update After Process
                foreach (var item in ssVMs)
                {
                    Schedule1SalaryVM updatedSSVM = new Schedule1SalaryVM();
                    LineA = "";
                    LineA += "~" + item.Line1A;
                    LineA += "~" + item.Line2A;
                    LineA += "~" + item.Line3A;
                    LineA += "~" + item.Line4A;
                    LineA += "~" + item.Line5A;
                    LineA += "~" + item.Line6A;
                    LineA += "~" + item.Line7A;
                    LineA += "~" + item.Line8A;
                    LineA += "~" + item.Line9A;
                    LineA += "~" + item.Line10A;
                    LineA += "~" + item.Line11A;
                    LineA += "~" + item.Line12A;
                    LineA += "~" + item.Line13A;
                    LineA += "~" + item.Line14A;
                    LineA += "~" + item.Line15A;
                    LineA += "~" + item.Line16A;
                    LineA += "~" + item.Line17A;
                    LineA += "~" + item.Line18A;
                    LineA += "~" + item.Line19A;
                    LineA += "~" + item.Line20A;
                    LineA += "~" + item.Line21A;
                    LineA += "~" + item.Line22A;
                    #region ProcessSASalaryM
                    Schedule1SalaryMonthlyDAL _ssMonthlyDAL = new Schedule1SalaryMonthlyDAL();
                    updatedSSVM = _ssMonthlyDAL.ProcessSASalary(LineA, item.TaxSlabId, isMonth, currConn, transaction);

                    #region Assign Data For Updating
                    updatedSSVM.Id = item.Id;
                    updatedSSVM.EmployeeId = item.EmployeeId;
                    updatedSSVM.TaxSlabId = item.TaxSlabId;
                    updatedSSVM.ProjectId = item.ProjectId;
                    updatedSSVM.DepartmentId = item.DepartmentId;
                    updatedSSVM.SectionId = item.SectionId;
                    updatedSSVM.DesignationId = item.DesignationId;
                    updatedSSVM.FiscalYearId = item.FiscalYearId;
                    updatedSSVM.Year = item.Year;
                    //updatedSSVM.FiscalYearDetailId = item.FiscalYearDetailId;

                    updatedSSVM.LastUpdateBy = auditvm.CreatedBy;
                    updatedSSVM.LastUpdateAt = auditvm.CreatedAt;
                    updatedSSVM.LastUpdateFrom = auditvm.CreatedFrom;

                    #endregion Assign Data For Updating

                    #endregion ProcessSASalaryM

                    updatedSSVMs.Add(updatedSSVM);
                }
                #endregion Ready For Update After Process
                #endregion Process
                #region Update
                foreach (var item in updatedSSVMs)
                {
                    retResults = Update(item, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                }

                #endregion Update


                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Schedule1 Salary Yearly Saved Successfully!";
                #endregion Commit
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


        //==================InsertFromSalary =================
        public string[] InsertFromSchedule1SalaryMonthly(string FiscalYear, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertFromSchedule1SalaryMonthly"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                    currConn = _dbsqlConnection.GetConnectionTax();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();
                #region Save
                sqlText = "  ";
                sqlText += @" 
---------------Declaration---------------
--declare @EmployeeId as varchar (100)
--declare @FiscalYearDetailId as int
--declare @FiscalYear as varchar(20)
declare @FiscalYearId as varchar(20)
--
--set @EmployeeId = '1_99'
--set @FiscalYear = '2017'
";

                sqlText += " SELECT @FiscalYearId=Id";
                sqlText += " FROM " + hrmDB + ".[dbo].FiscalYear WHERE Year=@FiscalYear";
                sqlText += @"

---Delete First--
DELETE from Schedule1SalaryYearlies
WHERE Year=@FiscalYear

DELETE from EmployeeTaxSlabDetailsYearlies
WHERE Year=@FiscalYear

----Insert---
INSERT INTO Schedule1SalaryYearlies (
TaxSlabId
,EmployeeId,ProjectId,DepartmentId,SectionId,DesignationId
, FiscalYearId, Year 
, Line1A, Line2A, Line3A, Line4A, Line5A, Line6A, Line7A, Line8A, Line9A
, Line10A, Line11A, Line12A, Line13A, Line14A, Line15A, Line16A, Line17A, Line18A, Line19A
, Line20A, Line21A, Line22A

,Remarks, IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
)

SELECT 
TaxSlabId
, EmployeeId, ProjectId, DepartmentId, SectionId, DesignationId
, FiscalYearId, Year 
, Line1A, Line2A, Line3A, Line4A, Line5A, Line6A, Line7A, Line8A, Line9A
, Line10A, Line11A, Line12A, Line13A, Line14A, Line15A, Line16A, Line17A, Line18A, Line19A
, Line20A, Line21A, Line22A

,'-',@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
--,'-',1,0,'Admin','19000101','Local'

FROM
(
SELECT
ets.TaxSlabId
, ssm.EmployeeId, ssm.ProjectId, ssm.DepartmentId, ssm.SectionId, ssm.DesignationId
,ssm.FiscalYearId, ssm.Year
,SUM(ssm.Line1A) Line1A ,SUM(ssm.Line2A) Line2A ,SUM(ssm.Line3A) Line3A ,SUM(ssm.Line4A) Line4A 
,SUM(ssm.Line5A) Line5A ,SUM(ssm.Line6A) Line6A ,SUM(ssm.Line7A) Line7A ,SUM(ssm.Line8A) Line8A 
,SUM(ssm.Line9A) Line9A ,SUM(ssm.Line10A) Line10A ,SUM(ssm.Line11A) Line11A ,SUM(ssm.Line12A) Line12A 
,SUM(ssm.Line13A) Line13A ,SUM(ssm.Line14A) Line14A ,SUM(ssm.Line15A) Line15A ,SUM(ssm.Line16A) Line16A 
,SUM(ssm.Line17A) Line17A ,SUM(ssm.Line18A) Line18A ,SUM(ssm.Line19A) Line19A ,SUM(ssm.Line20A) Line20A 
,SUM(ssm.Line21A) Line21A ,SUM(ssm.Line22A) Line22A 

 FROM Schedule1SalaryMonthlies ssm
Left Outer Join EmloyeeTAXSlabs ets ON ssm.EmployeeId = ets.EmployeeId

 WHERE 1=1
AND ssm.Year=@FiscalYear 
GROUP BY ets.TaxSlabId, ssm.EmployeeId, ssm.ProjectId, ssm.DepartmentId, ssm.SectionId, ssm.DesignationId
,ssm.FiscalYearId, ssm.Year
 ) as a

";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@FiscalYear", FiscalYear);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);

                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                if (transResult <= 0)
                {
                    retResults[1] = "No Data Found in Schedule1 Salary Monthly!";
                    return retResults;
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
                retResults[0] = "Success";
                retResults[1] = "Schedule1 Salary Yearl Saved Successfully!";
                #endregion Commit
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




        #endregion Process

        #endregion Methods
    }
}
