using SymOrdinary;
using SymServices.Common;
using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SymServices.Tax
{
    public class Schedule1SalaryMonthlyDAL
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
   FROM Schedule1SalaryMonthlies
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
        public List<EmployeeInfoVM> SelectEmployeeList(string[] conditionFields = null, string[] conditionValues = null, string tType = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
,ssm.EmployeeId
,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,ISNULL(ssm.TransactionType,'Salary') TransactionType

FROM Schedule1SalaryMonthlies ssm
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fyd ON ssm.FiscalYearDetailId=fyd.Id";
                sqlText += "  LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON ssm.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND ssm.IsArchive = 0
";
                if (!string.IsNullOrWhiteSpace(tType))
                {
                    sqlText += " AND ISNULL(ssm.TransactionType,'Salary')=@TransactionType";
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

                if (!string.IsNullOrWhiteSpace(tType))
                {
                    objComm.Parameters.AddWithValue("@TransactionType", tType);
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
                    vm.TransactionType = dr["TransactionType"].ToString();
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
        public List<Schedule1SalaryVM> SelectFiscalPeriod(string[] conditionFields = null, string[] conditionValues = null, string tType = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
SELECT DISTINCT fyd.Id
,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd
,ssm.Remarks 
,ISNULL(ssm.TransactionType,'Salary') TransactionType
FROM Schedule1SalaryMonthlies ssm
";

                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fyd ON ssm.FiscalYearDetailId=fyd.Id";
                sqlText += "  LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON ssm.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND ssm.IsArchive = 0   and PeriodId is not null 
";
                if (!string.IsNullOrWhiteSpace(tType))
                {
                    sqlText += " AND ISNULL(ssm.TransactionType,'Salary')=@TransactionType";
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
                sqlText += @" ORDER BY fyd.PeriodStart desc";

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
                if (!string.IsNullOrWhiteSpace(tType))
                {
                    objComm.Parameters.AddWithValue("@TransactionType", tType);

                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new Schedule1SalaryVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalPeriod = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
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
        public List<Schedule1SalaryVM> SelectAll(int Id = 0, string[] conditionFields = null,
            string[] conditionValues = null, string tType = "", SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, List<EmloyeeTAXSlabVM> vms = null)
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
ssm.Id
,ssm.EmployeeId
,ISNULL(ssm.TaxSlabId, 0)TaxSlabId
,ssm.ProjectId
,ssm.DepartmentId
,ssm.SectionId
,ssm.DesignationId
,ssm.FiscalYearId
,ISNULL(ssm.Year, 0) Year
,ISNULL(ssm.FiscalYearDetailId, 0) FiscalYearDetailId
,ISNULL(ssm.FiscalYearDetailIdTo, 0) FiscalYearDetailIdTo
,ISNULL(ssm.Line1A,0)Line1A,ISNULL(ssm.Line1B,0)Line1B,ISNULL(ssm.Line1C,0)Line1C,ssm.Line1Remarks
,ISNULL(ssm.Line2A,0)Line2A,ISNULL(ssm.Line2B,0)Line2B,ISNULL(ssm.Line2C,0)Line2C,ssm.Line2Remarks
,ISNULL(ssm.Line3A,0)Line3A,ISNULL(ssm.Line3B,0)Line3B,ISNULL(ssm.Line3C,0)Line3C,ssm.Line3Remarks
,ISNULL(ssm.Line4A,0)Line4A,ISNULL(ssm.Line4B,0)Line4B,ISNULL(ssm.Line4C,0)Line4C,ssm.Line4Remarks
,ISNULL(ssm.Line5A,0)Line5A,ISNULL(ssm.Line5B,0)Line5B,ISNULL(ssm.Line5C,0)Line5C,ssm.Line5Remarks
,ISNULL(ssm.Line6A,0)Line6A,ISNULL(ssm.Line6B,0)Line6B,ISNULL(ssm.Line6C,0)Line6C,ssm.Line6Remarks
,ISNULL(ssm.Line7A,0)Line7A,ISNULL(ssm.Line7B,0)Line7B,ISNULL(ssm.Line7C,0)Line7C,ssm.Line7Remarks
,ISNULL(ssm.Line8A,0)Line8A,ISNULL(ssm.Line8B,0)Line8B,ISNULL(ssm.Line8C,0)Line8C,ssm.Line8Remarks
,ISNULL(ssm.Line9A,0)Line9A,ISNULL(ssm.Line9B,0)Line9B,ISNULL(ssm.Line9C,0)Line9C,ssm.Line9Remarks
,ISNULL(ssm.Line10A,0) Line10A,ISNULL(ssm.Line10B,0) Line10B,ISNULL(ssm.Line10C,0) Line10C,ssm.Line10Remarks
,ISNULL(ssm.Line11A,0) Line11A,ISNULL(ssm.Line11B,0) Line11B,ISNULL(ssm.Line11C,0) Line11C,ssm.Line11Remarks
,ISNULL(ssm.Line12A,0) Line12A,ISNULL(ssm.Line12B,0) Line12B,ISNULL(ssm.Line12C,0) Line12C,ssm.Line12Remarks
,ISNULL(ssm.Line13A,0) Line13A,ISNULL(ssm.Line13B,0) Line13B,ISNULL(ssm.Line13C,0) Line13C,ssm.Line13Remarks
,ISNULL(ssm.Line14A,0) Line14A,ISNULL(ssm.Line14B,0) Line14B,ISNULL(ssm.Line14C,0) Line14C,ssm.Line14Remarks
,ISNULL(ssm.Line15A,0) Line15A,ISNULL(ssm.Line15B,0) Line15B,ISNULL(ssm.Line15C,0) Line15C,ssm.Line15Remarks
,ISNULL(ssm.Line16A,0) Line16A,ISNULL(ssm.Line16B,0) Line16B,ISNULL(ssm.Line16C,0) Line16C,ssm.Line16Remarks
,ISNULL(ssm.Line17A,0) Line17A,ISNULL(ssm.Line17B,0) Line17B,ISNULL(ssm.Line17C,0) Line17C,ssm.Line17Remarks
,ISNULL(ssm.Line18A,0) Line18A,ISNULL(ssm.Line18B,0) Line18B,ISNULL(ssm.Line18C,0) Line18C,ssm.Line18Remarks
,ISNULL(ssm.Line19A,0) Line19A,ISNULL(ssm.Line19B,0) Line19B,ISNULL(ssm.Line19C,0) Line19C,ssm.Line19Remarks
,ISNULL(ssm.Line20A,0) Line20A,ISNULL(ssm.Line20B,0) Line20B,ISNULL(ssm.Line20C,0) Line20C,ssm.Line20Remarks
,ISNULL(ssm.Line21A,0) Line21A,ISNULL(ssm.Line21B,0) Line21B,ISNULL(ssm.Line21C,0) Line21C,ssm.Line21Remarks
,ISNULL(ssm.Line22A,0) Line22A,ISNULL(ssm.Line22B,0) Line22B,ISNULL(ssm.Line22C,0) Line22C,ssm.Line22Remarks
,ISNULL(ssm.TotalIncomeAmount	,0) TotalIncomeAmount
,ISNULL(ssm.TotalExemptedAmount	,0) TotalExemptedAmount
,ISNULL(ssm.TotalTaxableAmount	,0) TotalTaxableAmount
,ISNULL(ssm.TotalTaxPayAmount	,0) TotalTaxPayAmount
,ISNULL(ssm.FinalTaxAmount	    ,0) FinalTaxAmount
,ISNULL(ssm.FinalTaxAmountMonthly	    ,0) FinalTaxAmountMonthly
,ISNULL(ssm.FinalBonusTaxAmount	    ,0) FinalBonusTaxAmount
,ISNULL(ssm.RebateAmountMonthly	    ,0) RebateAmountMonthly

,ISNULL(ssm.TransactionType,'Salary') TransactionType
,ssm.Remarks
,ssm.IsActive
,ssm.IsArchive
,ssm.CreatedBy
,ssm.CreatedAt
,ssm.CreatedFrom
,ssm.LastUpdateBy
,ssm.LastUpdateAt
,ssm.LastUpdateFrom

FROM Schedule1SalaryMonthlies ssm
WHERE  1=1  AND ssm.IsArchive = 0
";
                if (!string.IsNullOrWhiteSpace(tType))
                {
                    sqlText += @" AND ISNULL(ssm.TransactionType,'Salary')=@TransactionType";

                }
                if (Id > 0)
                {
                    sqlText += @" and ssm.Id=@Id";
                }

                if (vms != null && vms.Count > 0)
                {
                    sqlText += " AND EmployeeId in ('" + string.Join("','", vms.Select(x => x.EmployeeId)) + "')";
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
                //sqlText += " Order By ssm.EmployeeId";

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

                if (!string.IsNullOrWhiteSpace(tType))
                {
                    objComm.Parameters.AddWithValue("@TransactionType", tType);

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
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.TaxSlabId = Convert.ToInt32(dr["TaxSlabId"]);
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.FiscalYearId = dr["FiscalYearId"].ToString();
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.FiscalYearDetailIdTo = Convert.ToInt32(dr["FiscalYearDetailIdTo"]);
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
                    vm.Line6C = Convert.ToDecimal(dr["Line6c"]);
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
                    vm.FinalTaxAmount = Convert.ToDecimal(dr["FinalTaxAmount"]);
                    vm.FinalTaxAmountMonthly = Convert.ToDecimal(dr["FinalTaxAmountMonthly"]);
                    vm.FinalBonusTaxAmount = Convert.ToDecimal(dr["FinalBonusTaxAmount"]);
                    vm.RebateAmountMonthly = Convert.ToDecimal(dr["RebateAmountMonthly"]);

                    SettingDAL _settingDal = new SettingDAL();
                    string Divided = _settingDal.settingValue("Tax", "Divided").Trim();
                    vm.OneThird =Convert.ToDecimal(((Convert.ToDecimal(dr["TotalIncomeAmount"])) / Convert.ToDecimal(Divided)).ToString("N2"));


                    vm.TransactionType = dr["TransactionType"].ToString();
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

        //==================SelectAll=================
        public List<YearlyTAXVM> YearlyTax(int Id = 0, string[] conditionFields = null,
            string[] conditionValues = null, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, List<EmloyeeTAXSlabVM> vms = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<YearlyTAXVM> VMs = new List<YearlyTAXVM>();
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

                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();


                sqlText = string.Format(@"
SELECT  ssm.[Id]
      ,ssm.[EmployeeId]
      ,ssm.[FiscalYearId]
      ,ssm.[FiscalDetailYearId]
      ,ssm.[Value]
      ,ssm.[AdvanceTax]
	  ,ve.EmpName EmployeeName
	  ,fy.Year FiscalYearName
	  ,fyd.PeriodName FiscalPeriodName
  FROM YearlyTaxBreakDown ssm
  left outer join {0}.dbo.FiscalYear fy on ssm.[FiscalYearId] = fy.Id
  left outer join {0}.dbo.FiscalYearDetail fyd on ssm.FiscalDetailYearId = fyd.Id
  left outer join {0}.dbo.ViewEmployeeInformation ve on ssm.EmployeeId = ve.EmployeeId
WHERE  1=1 
  
", hrmDB);
                if (Id > 0)
                {
                    sqlText += @" and ssm.Id=@Id";
                }

                if (vms != null && vms.Count > 0)
                {
                    sqlText += " AND EmployeeId in ('" + string.Join("','", vms.Select(x => x.EmployeeId)) + "')";
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
                sqlText += " Order By ssm.FiscalDetailYearId";

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

                DataTable dtResult = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(objComm);
                adapter.Fill(dtResult);

                VMs = JsonConvert.DeserializeObject<List<YearlyTAXVM>>(JsonConvert.SerializeObject(dtResult));

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch
            //catch (SqlException sqlex)
            //{
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //}
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
        public string[] Insert(Schedule1SalaryVM vm, string tType = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertSchedule1SalaryMonthly"; //Method Name
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
                //vm.Id = _cDal.NextId("Schedule1SalaryMonthlies", currConn, transaction);
                FiscalYearDAL fiscalYearDAL = new FiscalYearDAL();
                FiscalYearVM fiscalYearVM = new FiscalYearVM();
                fiscalYearVM = fiscalYearDAL.SelectByYear(vm.Year);
                vm.FiscalYearId = fiscalYearVM.Id;

                if (vm != null)
                {
                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO Schedule1SalaryMonthlies(
EmployeeId
,TaxSlabId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,FiscalYearId
,Year
,FiscalYearDetailId
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
,FinalTaxAmount

,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@EmployeeId
,@TaxSlabId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@FiscalYearId
,@Year
,@FiscalYearDetailId
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
,@FinalTaxAmount

,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    #endregion SqlText
                    #region SqlExecution

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    //cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@TaxSlabId", vm.TaxSlabId);

                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearId", vm.FiscalYearId);
                    cmdInsert.Parameters.AddWithValue("@Year", vm.Year);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
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
                    cmdInsert.Parameters.AddWithValue("@Line6c", vm.Line6C);
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
                    cmdInsert.Parameters.AddWithValue("@FinalTaxAmount", vm.FinalTaxAmount);



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
                        throw new ArgumentNullException("Unexpected error to update Schedule1SalaryMonthlies.", "");
                    }
                    #endregion SqlExecution

                    #region Insert Into EmployeeTaxSlabDetailDAL
                    EmployeeTaxSlabDetailMonthlyDAL _employeeTaxSlabDetailDAL = new EmployeeTaxSlabDetailMonthlyDAL();
                    if (vm.employeeTaxSlabDetailVMs != null && vm.employeeTaxSlabDetailVMs.Count > 0)
                    {
                        foreach (EmployeeTaxSlabDetailVM EmployeeTaxSlabDetailVM in vm.employeeTaxSlabDetailVMs)
                        {
                            EmployeeTaxSlabDetailVM dVM = new EmployeeTaxSlabDetailVM();
                            dVM = EmployeeTaxSlabDetailVM;
                            dVM.EmployeeId = vm.EmployeeId;
                            dVM.FiscalYearDetailId = vm.FiscalYearDetailId;
                            dVM.Year = vm.Year;
                            dVM.Schedule1Id = vm.Id;
                            dVM.TaxSlabId = vm.TaxSlabId;

                            dVM.CreatedAt = vm.CreatedAt;
                            dVM.CreatedBy = vm.CreatedBy;
                            dVM.CreatedFrom = vm.CreatedFrom;
                            retResults = _employeeTaxSlabDetailDAL.Insert(dVM, tType, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Insert Into EmployeeTaxSlabDetailDAL
                    #region Update TotalTaxPayAmount In Schedule1SalaryMonthlies
                    retResults = UpdateTotalTaxPayAmount(vm, tType, currConn, transaction);

                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion
                    #region Update FinalTaxAmount In Schedule1SalaryMonthlies
                    retResults = UpdateFinalTaxAmount(vm, tType, currConn, transaction);

                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion

                }
                else
                {
                    retResults[1] = "This Schedule1SalaryMonthly already used!";
                    throw new ArgumentNullException("Please Input Schedule1SalaryMonthly Value", "");
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
        public string[] Update(Schedule1SalaryVM vm, string tType = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Schedule1SalaryMonthly Update"; //Method Name
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

                string hrmDb = _dbsqlConnection.HRMDB;
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    if (vm.TaxSlabId <= 0)
                    {
                        vm.TaxSlabId = 1;
                    }
                    bool isMonth = true;

                    if (tType == "YearlyTax")
                    {
                        isMonth = false;
                    }


                    #region Ready For Update After Process
                    Schedule1SalaryVM tempSSMVM = new Schedule1SalaryVM();
                    tempSSMVM = vm;

                    SettingDAL _settingDal = new SettingDAL();
                    string AmountExYesorNo =   _settingDal.settingValue("Tax", "AmountofExemptedIncome").Trim();
                    string AmountEx = _settingDal.settingValue("Tax", "Exempted").Trim();
                    string Divided = _settingDal.settingValue("Tax", "Divided").Trim();
                    string BonusTaxPercent = _settingDal.settingValue("Tax", "BonusTaxPercent").Trim();


                    string LineA = "";

                    LineA = "";
                    LineA += "~" + tempSSMVM.Line1A;
                    LineA += "~" + tempSSMVM.Line2A;
                    LineA += "~" + tempSSMVM.Line3A;
                    LineA += "~" + tempSSMVM.Line4A;
                    LineA += "~" + tempSSMVM.Line5A;
                    LineA += "~" + tempSSMVM.Line6A;
                    LineA += "~" + tempSSMVM.Line7A;
                    LineA += "~" + tempSSMVM.Line8A;
                    LineA += "~" + tempSSMVM.Line9A;
                    LineA += "~" + tempSSMVM.Line10A;
                    LineA += "~" + tempSSMVM.Line11A;
                    LineA += "~" + tempSSMVM.Line12A;
                    LineA += "~" + tempSSMVM.Line13A;
                    LineA += "~" + tempSSMVM.Line14A;
                    LineA += "~" + tempSSMVM.Line15A;
                    LineA += "~" + tempSSMVM.Line16A;
                    LineA += "~" + tempSSMVM.Line17A;
                    LineA += "~" + tempSSMVM.Line18A;
                    LineA += "~" + tempSSMVM.Line19A;
                    LineA += "~" + tempSSMVM.Line20A;
                    LineA += "~" + tempSSMVM.Line21A;
                    LineA += "~" + tempSSMVM.Line22A;

                    #region ProcessSASalaryM
                    tempSSMVM = ProcessSASalary(LineA, tempSSMVM.TaxSlabId, isMonth, currConn, transaction);

                    #endregion ProcessSASalaryM


                    #region Assign Data For Updating
                    tempSSMVM.Id = vm.Id;
                    tempSSMVM.EmployeeId = vm.EmployeeId;
                    tempSSMVM.TaxSlabId = vm.TaxSlabId;
                    tempSSMVM.ProjectId = vm.ProjectId;
                    tempSSMVM.DepartmentId = vm.DepartmentId;
                    tempSSMVM.SectionId = vm.SectionId;
                    tempSSMVM.DesignationId = vm.DesignationId;
                    tempSSMVM.FiscalYearId = vm.FiscalYearId;
                    tempSSMVM.Year = vm.Year;
                    tempSSMVM.FiscalYearDetailId = vm.FiscalYearDetailId;
                    tempSSMVM.FiscalYearDetailIdTo = vm.FiscalYearDetailIdTo;

                    tempSSMVM.LastUpdateBy = vm.LastUpdateBy;
                    tempSSMVM.LastUpdateAt = vm.LastUpdateAt;
                    tempSSMVM.LastUpdateFrom = vm.LastUpdateFrom;

                    #endregion Assign Data For Updating

                    #endregion Ready For Update After Process

                    #region ReAssign Schedule1SalaryVM
                    vm = tempSSMVM;
                    #endregion ReAssign Schedule1SalaryVM


                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE Schedule1SalaryMonthlies SET";
                    sqlText += "   EmployeeId=@EmployeeId";
                    sqlText += " , TaxSlabId=@TaxSlabId";
                    sqlText += " , ProjectId=@ProjectId";
                    sqlText += " , DepartmentId=@DepartmentId";
                    sqlText += " , SectionId=@SectionId";
                    sqlText += " , DesignationId=@DesignationId";
                    sqlText += " , FiscalYearId=@FiscalYearId";
                    sqlText += " , Year=@Year";
                    sqlText += " , FiscalYearDetailId=@FiscalYearDetailId";
                    sqlText += " , FiscalYearDetailIdTo=@FiscalYearDetailIdTo";
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
                    sqlText += " , Line6c=@Line6c";
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
                    sqlText += " , FinalTaxAmount=@FinalTaxAmount";
                    
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += @" WHERE 1=1 
AND Id=@Id
                    
                    ";
                    #endregion SqlText
                    #region SqlExecution

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                    cmdUpdate.Parameters.AddWithValue("@TaxSlabId", vm.TaxSlabId);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdUpdate.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearId", vm.FiscalYearId);
                    cmdUpdate.Parameters.AddWithValue("@Year", vm.Year);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailIdTo", vm.FiscalYearDetailIdTo);
                    cmdUpdate.Parameters.AddWithValue("@Line1A", vm.Line1A);
                    cmdUpdate.Parameters.AddWithValue("@Line1B", vm.Line1B);
                    cmdUpdate.Parameters.AddWithValue("@Line1C", AmountExYesorNo.ToUpper() != "Y" ? vm.Line1C : vm.Line1A);
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
                    //cmdUpdate.Parameters.AddWithValue("@Line5C", vm.Line5C);
                    cmdUpdate.Parameters.AddWithValue("@Line5C", AmountExYesorNo.ToUpper() != "Y" ? vm.Line5C : vm.Line5A);

                    cmdUpdate.Parameters.AddWithValue("@Line5Remarks", vm.Line5Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Line6A", vm.Line6A);
                    cmdUpdate.Parameters.AddWithValue("@Line6B", vm.Line6B);
                    cmdUpdate.Parameters.AddWithValue("@Line6c", vm.Line6C);
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
                    if (Convert.ToInt32(BonusTaxPercent)>0)
                    {
                        vm.Line14A = vm.Line14A * Convert.ToInt32(BonusTaxPercent) / 100;
                        vm.Line14C = vm.Line14A * Convert.ToInt32(BonusTaxPercent) / 100;
                    }
                    else
                    {
                        vm.Line14A = 0;
                        vm.Line14C = 0;
                    }
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

                                   
                    decimal OneThird = vm.TotalIncomeAmount / decimal.Parse(Divided);

                    if (tType == "Salary")
                    {
                        if (OneThird < decimal.Parse(AmountEx) / 12)
                        {
                            vm.TotalTaxableAmount = Math.Round(vm.TotalIncomeAmount - OneThird);
                        }
                        else
                        {
                            vm.TotalTaxableAmount = Math.Round(vm.TotalIncomeAmount - decimal.Parse(AmountEx) / 12);
                        }
                    }
                    else
                    {
                        if (OneThird < decimal.Parse(AmountEx))
                        {
                            vm.TotalTaxableAmount = Math.Round(vm.TotalIncomeAmount - OneThird);
                        }
                        else
                        {
                            vm.TotalTaxableAmount = Math.Round(vm.TotalIncomeAmount - decimal.Parse(AmountEx));
                        }
                    }
                    vm.OneThird = OneThird;

                    cmdUpdate.Parameters.AddWithValue("@TotalIncomeAmount", vm.TotalIncomeAmount);
                    cmdUpdate.Parameters.AddWithValue("@TotalExemptedAmount", vm.TotalExemptedAmount);
                    cmdUpdate.Parameters.AddWithValue("@TotalTaxableAmount", vm.TotalTaxableAmount);
                    //cmdUpdate.Parameters.AddWithValue("@TotalTaxableAmount", AmountExYesorNo.ToUpper() != "Y" ? vm.TotalIncomeAmount : vm.TotalTaxableAmount);
                    cmdUpdate.Parameters.AddWithValue("@TotalTaxPayAmount", vm.TotalTaxPayAmount);


                    cmdUpdate.Parameters.AddWithValue("@FinalTaxAmount", vm.FinalTaxAmount);


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
                        throw new ArgumentNullException("Unexpected error to update Schedule1SalaryMonthlies.", "");
                    }
                    #endregion SqlExecution



                    #region Insert Into EmployeeTaxSlabDetailDAL
                    //#region Calculate EmployeeTaxSlabDetailDAL

                    EmployeeTaxSlabDetailMonthlyDAL _employeeTaxSlabDetailDAL = new EmployeeTaxSlabDetailMonthlyDAL();
                    //vm.employeeTaxSlabDetailVMs = _employeeTaxSlabDetailDAL.SelectEmployeeTaxSlabDetails(vm.TotalTaxableAmount, vm.TaxSlabId, isMonth, currConn, transaction);
                    //#endregion Calculate EmployeeTaxSlabDetailDAL

                    if (vm.employeeTaxSlabDetailVMs != null && vm.employeeTaxSlabDetailVMs.Count > 0)
                    {
                        #region Delete Detail
                        try
                        {
                            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "EmployeeTaxSlabDetailsMonthlies", "Schedule1Id", currConn, transaction);
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
                        foreach (EmployeeTaxSlabDetailVM EmployeeTaxSlabDetailVM in vm.employeeTaxSlabDetailVMs)
                        {
                            EmployeeTaxSlabDetailVM dVM = new EmployeeTaxSlabDetailVM();
                            dVM = EmployeeTaxSlabDetailVM;
                            dVM.EmployeeId = vm.EmployeeId;
                            dVM.FiscalYearDetailId = vm.FiscalYearDetailId;
                            dVM.FiscalYearDetailIdTo = vm.FiscalYearDetailIdTo;
                            dVM.Year = vm.Year;
                            dVM.Schedule1Id = vm.Id;
                            dVM.TaxSlabId = vm.TaxSlabId;

                            dVM.CreatedAt = vm.LastUpdateAt;
                            dVM.CreatedBy = vm.LastUpdateBy;
                            dVM.CreatedFrom = vm.LastUpdateFrom;
                            retResults = _employeeTaxSlabDetailDAL.Insert(dVM, tType, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        #endregion Insert Detail Again

                    }

                    #endregion Insert Into EmployeeTaxSlabDetailDAL

                    #region Update TotalTaxPayAmount In Schedule1SalaryMonthlies
                    retResults = UpdateTotalTaxPayAmount(vm, tType, currConn, transaction);

                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion

                    cmdUpdate.CommandText = RebateAmountGen(vm.Id.ToString(), "", "", "", null, "", "N", hrmDb);
                    cmdUpdate.ExecuteNonQuery();


                    #region Update FinalTaxAmount In Schedule1SalaryMonthlies
                    retResults = UpdateFinalTaxAmount(vm, tType, currConn, transaction);
                    if (tType == "YearlyTax")
                    {
                        retResults = UpdateHouseRentMedicalConveyance(vm, tType, currConn, transaction);
                    }
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion

                    #region Update FinalBonusTaxAmount In Schedule1SalaryMonthlies
                    if (tType == "Bonus")
                    {
                        retResults = UpdateFinalBonusTaxAmount(vm.FiscalYearDetailId, vm.FiscalYearDetailIdTo, vm.EmployeeId, currConn, transaction);

                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }

                    #endregion




                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Schedule1SalaryMonthly Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Schedule1SalaryMonthly Update", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = "Fail";//Success or Fail
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


        public string[] UpdateAdvanceTax(YearlyTAXVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Schedule1SalaryMonthly Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string hrmDb = _dbsqlConnection.HRMDB;
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
                 
                string fyid = @"SELECT [FiscalDetailYearId] FROM YearlyTaxBreakDown where Id=@ID";
                SqlCommand cmdfyid = new SqlCommand(fyid, currConn, transaction);
                cmdfyid.Parameters.AddWithValue("@ID", vm.Id);
                SqlDataAdapter adapterfyid = new SqlDataAdapter(cmdfyid);
                DataTable dtfyid = new DataTable();
                adapterfyid.Fill(dtfyid);
                decimal fyscalyearid = Convert.ToDecimal(dtfyid.Rows[0][0].ToString());

                string fyear = @"SELECT top 1 year FROM Schedule1SalaryMonthlies where FiscalYearDetailId=@ID";
                SqlCommand cmdfyear = new SqlCommand(fyear, currConn, transaction);
                cmdfyear.Parameters.AddWithValue("@ID", fyscalyearid);
                SqlDataAdapter adapterfyear = new SqlDataAdapter(cmdfyear);
                DataTable dtfyear = new DataTable();
                adapterfyear.Fill(dtfyear);
                decimal fyscalyear = Convert.ToDecimal(dtfyear.Rows[0][0].ToString());

                string fpriod = @"SELECT id FROM " + hrmDb + ".[dbo].[FiscalYearDetail] where Year=@ID";
                SqlCommand cmdfpriod = new SqlCommand(fpriod, currConn, transaction);
                cmdfpriod.Parameters.AddWithValue("@ID", fyscalyear);
                SqlDataAdapter adapterfpriod = new SqlDataAdapter(cmdfpriod);
                DataTable dtfpriod = new DataTable();
                adapterfpriod.Fill(dtfpriod);
                string fyearidFrom = dtfpriod.Rows[0][0].ToString();
                string fyearidTo= dtfpriod.Rows[7][0].ToString();


                string maxfid = @"SELECT MAX(FiscalYearDetailId) FROM  " + hrmDb + ".[dbo].[ViewSalaryPreCalculation]  where EmployeeId=@EmployeeId and FiscalYearDetailId between @fyearidFrom and @fyearidTo and SalaryHead='TAX' and Amount>0";
                SqlCommand cmdfid = new SqlCommand(maxfid, currConn, transaction);
                cmdfid.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdfid.Parameters.AddWithValue("@fyearidFrom", fyearidFrom);
                cmdfid.Parameters.AddWithValue("@fyearidTo", fyearidTo);
                SqlDataAdapter adapterfid = new SqlDataAdapter(cmdfid);
                DataTable dtfid = new DataTable();
                adapterfid.Fill(dtfid);
                decimal Maxfid = Convert.ToDecimal(dtfid.Rows[0][0].ToString());


                string RestMonth = @"SELECT Count([FiscalDetailYearId]) FROM YearlyTaxBreakDown
                where EmployeeId=@EmployeeId and FiscalDetailYearId>@Maxfid";
                SqlCommand cmdrm = new SqlCommand(RestMonth, currConn, transaction);
                cmdrm.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdrm.Parameters.AddWithValue("@Maxfid", Maxfid);
                SqlDataAdapter adapterrm = new SqlDataAdapter(cmdrm);
                DataTable dtfrm = new DataTable();
                adapterrm.Fill(dtfrm);
                decimal RestMonthDivided = Convert.ToDecimal(dtfrm.Rows[0][0].ToString());


                string taxpaid = @"SELECT SUM([Amount]) FROM  " + hrmDb + ".[dbo].[ViewSalaryPreCalculation]  where EmployeeId=@EmployeeId and FiscalYearDetailId between @fyearidFrom and @fyearidTo and SalaryHead='TAX' ";
                SqlCommand cmdpaid = new SqlCommand(taxpaid, currConn, transaction);
                cmdpaid.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdpaid.Parameters.AddWithValue("@fyearidFrom", fyearidFrom);
                cmdpaid.Parameters.AddWithValue("@fyearidTo", fyearidTo);
                SqlDataAdapter adapterpaid = new SqlDataAdapter(cmdpaid);
                DataTable dtpaid = new DataTable();
                adapterpaid.Fill(dtpaid);
                decimal TotalPaid = Convert.ToDecimal(dtpaid.Rows[0][0].ToString());


                string ftax = @"SELECT FinalTaxAmount FROM Schedule1SalaryMonthlies
                where EmployeeId=@EmployeeId and TransactionType='YearlyTax' and FiscalYearDetailId between @fyearidFrom and @fyearidTo ";
                SqlCommand command = new SqlCommand(ftax, currConn, transaction);
                command.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                command.Parameters.AddWithValue("@fyearidFrom", fyearidFrom);
                command.Parameters.AddWithValue("@fyearidTo", fyearidTo);
                SqlDataAdapter adapterftax = new SqlDataAdapter(command);
                DataTable dtfTax = new DataTable();
                adapterftax.Fill(dtfTax);
                decimal TotalYearlyTax = Convert.ToDecimal(dtfTax.Rows[0][0].ToString());

                SettingDAL _settingDal = new SettingDAL();
                string DividedMonth = _settingDal.settingValue("Tax", "DividedMonth").Trim();

                decimal taxUpdateValue = (TotalYearlyTax - vm.AdvanceTax - TotalPaid) / Convert.ToDecimal(DividedMonth);

//                string getSum = @"
//                select isnull(sum(Value),0), count(id)divideValue from YearlyTaxBreakDown
//                where EmployeeId = @EmployeeId and ID >= @ID ";
//                SqlCommand command = new SqlCommand(getSum, currConn, transaction);
//                command.Parameters.AddWithValue("@ID", vm.Id);
//                command.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
//                SqlDataAdapter adapter = new SqlDataAdapter(command);
//                DataTable dtYearlyTax = new DataTable();
//                adapter.Fill(dtYearlyTax);
                
//                decimal value = Convert.ToDecimal(dtYearlyTax.Rows[0][0]);
//                decimal divideValue = Convert.ToDecimal(dtYearlyTax.Rows[0][1]);
//                //value == 0 ? 0 :
//                decimal taxUpdateValue =  (value - vm.AdvanceTax) / divideValue;


                string updateTaxValue =
                    @"Update YearlyTaxBreakDown set Value = @Value where EmployeeId = @EmployeeId and ID >= @ID ";
                SqlCommand commandut = new SqlCommand(updateTaxValue, currConn, transaction);
                commandut.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                commandut.Parameters.AddWithValue("@Value", taxUpdateValue);
                commandut.Parameters.AddWithValue("@ID", vm.Id);
                commandut.ExecuteNonQuery();

                string updateAdvanceTax =
                    @"Update YearlyTaxBreakDown set AdvanceTax = @AdvanceTax where EmployeeId = @EmployeeId and ID = @ID ";
                SqlCommand commandat = new SqlCommand(updateAdvanceTax, currConn, transaction);
                commandat.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                commandat.Parameters.AddWithValue("@AdvanceTax", vm.AdvanceTax);
                commandat.Parameters.AddWithValue("@ID", vm.Id);
                commandat.ExecuteNonQuery();


                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";
                retResults[2] = vm.Id.ToString();// Return Id
                retResults[3] = sqlText; //  SQL Query

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = "Fail";//Success or Fail
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
            retResults[5] = "DeleteSchedule1SalaryMonthly"; //Method Name
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
                        sqlText = "UPDATE Schedule1SalaryMonthlies SET";
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
                        throw new ArgumentNullException("Schedule1SalaryMonthly Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Schedule1SalaryMonthly Information Delete", "Could not found any item.");
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
                currConn = _dbsqlConnection.GetConnectionTax();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();
                #region sql statement
                #region SqlText
                sqlText = @"
SELECT
ssm.Id
,ssm.EmployeeId

,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd

,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,ve.Code EmployeeCode
,ve.EmpName EmployeeName

,ssm.FiscalYearId
,ssm.Year
,ssm.FiscalYearDetailId
,ssm.Line1A,ssm.Line1B,ssm.Line1C,ssm.Line1Remarks
,ssm.Line2A,ssm.Line2B,ssm.Line2C,ssm.Line2Remarks
,ssm.Line3A,ssm.Line3B,ssm.Line3C,ssm.Line3Remarks
,ssm.Line4A,ssm.Line4B,ssm.Line4C,ssm.Line4Remarks
,ssm.Line5A,ssm.Line5B,ssm.Line5C,ssm.Line5Remarks
,ssm.Line6A,ssm.Line6B,ssm.Line6C,ssm.Line6Remarks
,ssm.Line7A,ssm.Line7B,ssm.Line7C,ssm.Line7Remarks
,ssm.Line8A,ssm.Line8B,ssm.Line8C,ssm.Line8Remarks
,ssm.Line9A,ssm.Line9B,ssm.Line9C,ssm.Line9Remarks
,ssm.Line10A,ssm.Line10B,ssm.Line10C,ssm.Line10Remarks
,ssm.Line11A,ssm.Line11B,ssm.Line11C,ssm.Line11Remarks
,ssm.Line12A,ssm.Line12B,ssm.Line12C,ssm.Line12Remarks
,ssm.Line13A,ssm.Line13B,ssm.Line13C,ssm.Line13Remarks
,ssm.Line14A,ssm.Line14B,ssm.Line14C,ssm.Line14Remarks
,ssm.Line15A,ssm.Line15B,ssm.Line15C,ssm.Line15Remarks
,ssm.Line16A,ssm.Line16B,ssm.Line16C,ssm.Line16Remarks
,ssm.Line17A,ssm.Line17B,ssm.Line17C,ssm.Line17Remarks
,ssm.Line18A,ssm.Line18B,ssm.Line18C,ssm.Line18Remarks
,ssm.Line19A,ssm.Line19B,ssm.Line19C,ssm.Line19Remarks
,ssm.Line20A,ssm.Line20B,ssm.Line20C,ssm.Line20Remarks
,ssm.Line21A,ssm.Line21B,ssm.Line21C,ssm.Line21Remarks
,ssm.Line22A,ssm.Line22B,ssm.Line22C,ssm.Line22Remarks
,ssm.Remarks
,ssm.TotalIncomeAmount
,ssm.TotalExemptedAmount
,ssm.TotalTaxableAmount
,ssm.TotalTaxPayAmount
FROM Schedule1SalaryMonthlies ssm
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fyd ON ssm.FiscalYearDetailId=fyd.Id";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[ViewEmployeeInformation] ve ON ssm.EmployeeId = ve.EmployeeId";
                sqlText += " WHERE  1=1  AND ssm.IsArchive = 0";

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


        ////==================Report=================
        public DataTable InvestmentTaxReport(Schedule1SalaryVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();
                #region sql statement
                #region SqlText
                sqlText = @"
select ve.Code EIN, ve.EmpName [Name], ve.Designation, ssm.Line1A [Basic], TotalTaxableAmount [Total Taxable Income]
, TotalTaxPayAmount [Total Tax], RebateAmount [Tax Rebate], FinalTaxAmount [Net Tax]
, (
select top 1 value from YearlyTaxBreakDown
where EmployeeId = ssm.EmployeeId
order by FiscalDetailYearId desc  
)  [Tax per month]
,ssm.InvestmentLimit - (ssm.Line15C*2) [Invest after PF]
from 
Schedule1SalaryMonthlies ssm
left outer join HRMDB.dbo.ViewEmployeeInformation ve
on ssm.EmployeeId = ve.EmployeeId
WHERE  1=1
and TransactionType = 'yearlyTAx'
and ve.IsTAXApplicable = 1
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

                sqlText += " order by ve.DesignationOrder, ve.Code";
                sqlText = sqlText.Replace("HRMDB", _dbsqlConnection.HRMDB);

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


        ////==================TaxIncomeReport=================
        public DataTable TaxIncomeReport(Schedule1SalaryVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();

                #region sql statement
                #region SqlText
                sqlText = @"
SELECT DISTINCT ssm.EmployeeId
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,ve.Code EmployeeCode
,ve.EmpName EmployeeName
,ve.Email

,sum(ssm.Line1A)Line1A,sum(ssm.Line1B)Line1B,sum(ssm.Line1C)Line1C
,sum(ssm.Line2A)Line2A,sum(ssm.Line2B)Line2B,sum(ssm.Line2C)Line2C
,sum(ssm.Line3A)Line3A,sum(ssm.Line3B)Line3B,sum(ssm.Line3C)Line3C
,sum(ssm.Line4A)Line4A,sum(ssm.Line4B)Line4B,sum(ssm.Line4C)Line4C
,sum(ssm.Line5A)Line5A,sum(ssm.Line5B)Line5B,sum(ssm.Line5C)Line5C
,sum(ssm.Line6A)Line6A,sum(ssm.Line6B)Line6B,sum(ssm.Line6C)Line6C
,sum(ssm.Line7A)Line7A,sum(ssm.Line7B)Line7B,sum(ssm.Line7C)Line7C
,sum(ssm.Line8A)Line8A,sum(ssm.Line8B)Line8B,sum(ssm.Line8C)Line8C
,sum(ssm.Line9A)Line9A,sum(ssm.Line9B)Line9B,sum(ssm.Line9C)Line9C

,sum(ssm.Line10A)Line10A,sum(ssm.Line10B)Line10B,sum(ssm.Line10C)Line10C

,sum(ssm.Line11A)Line11A,sum(ssm.Line11B)Line11B,sum(ssm.Line11C)Line11C
,sum(ssm.Line12A)Line12A,sum(ssm.Line12B)Line12B,sum(ssm.Line12C)Line12C
,sum(ssm.Line13A)Line13A,sum(ssm.Line13B)Line13B,sum(ssm.Line13C)Line13C
,sum(ssm.Line14A)Line14A,sum(ssm.Line14B)Line14B,sum(ssm.Line14C)Line14C
,sum(ssm.Line15A)Line15A,sum(ssm.Line15B)Line15B,sum(ssm.Line15C)Line15C
,sum(ssm.Line16A)Line16A,sum(ssm.Line16B)Line16B,sum(ssm.Line16C)Line16C
,sum(ssm.Line17A)Line17A,sum(ssm.Line17B)Line17B,sum(ssm.Line17C)Line17C
,sum(ssm.Line18A)Line18A,sum(ssm.Line18B)Line18B,sum(ssm.Line18C)Line18C
,sum(ssm.Line19A)Line19A,sum(ssm.Line19B)Line19B,sum(ssm.Line19C)Line19C

,sum(ssm.Line20A)Line20A,sum(ssm.Line20B)Line20B,sum(ssm.Line20C)Line20C

,sum(ssm.Line21A)Line21A,sum(ssm.Line21B)Line21B,sum(ssm.Line21C)Line21C
,sum(ssm.Line22A)Line22A,sum(ssm.Line22B)Line22B,sum(ssm.Line22C)Line22C

FROM Schedule1SalaryMonthlies ssm

";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[ViewEmployeeInformation] ve ON ssm.EmployeeId = ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND ssm.IsArchive = 0
";
                sqlText += @" AND ISNULL(ssm.TransactionType,'Salary') = 'Salary' ";
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
                sqlText += @" GROUP BY ssm.EmployeeId, ve.Designation, ve.Department, ve.Section, ve.Project , ve.Code, ve.EmpName,ve.Email
 ";

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

        public DataTable TaxReportNBRMonthly(string vFiscalYearDetailId)
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
                currConn = _dbsqlConnection.GetConnectionTax();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("Report");
                #endregion open connection and transaction
                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();
                string TaxDB = _dbsqlConnection.TAXDB; //_settingDal.settingValue("Database", "HRMDB").Trim();
                SalarySheetVM vm = new SalarySheetVM();
                vm.FiscalYearDetailId = Convert.ToInt32(vFiscalYearDetailId);
                //SalaryProcessDAL _SDale = new SalaryProcessDAL();
                string[] retResults = new SalaryProcessDAL().TempSalaryProcess(vm, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("T SalaryProcess Fail for Current month  ", "");

                }
                #region sql statement
                #region TempSalaryProcess
                /*
                

                }
                */
                #endregion TempSalaryProcess

                #region SqlText

                sqlText = @" 

	--declare @vFiscalYearDetailId as varchar(100)='1079'
	declare @LeftDataRange as varchar(100 )
	declare @LastDayOfMonth as varchar(100 )
	declare @FiscalYearId as varchar(100);
	declare @PeriodId as varchar(100);

	select @FiscalYearId=FiscalYearId,@LastDayOfMonth=PeriodEnd,@PeriodId=PeriodId from   " + hrmDB + @".dbo.FiscalYearDetail where id=@vFiscalYearDetailId 
	select @LeftDataRange=YearStart from   " + hrmDB + @".dbo.FiscalYear where id=@FiscalYearId  



	select  fd.id FiscalYearDetailId
,fd.FiscalYearId 
,ev.Code,ev.EmpName
	,ev.Project
,ev.Department
,ev.Section
,ev.Designation,
	isnull(e.Basic,PSale.Basic)Basic
	,isnull(e.HouseRent,PSale.HouseRent)HouseRent
	,isnull(e.Medical,PSale.Medical)Medical
	,isnull(e.TransportAllowance,PSale.TransportAllowance)TransportAllowance
	,isnull(0,PSale.Gross)Gross
	,isnull(CurrentDeposit.depositAmount,0) TAX
	,td.BankInformation,isnull(td.ChallanNo,'')ChallanNo
,td.DepositDate
,round(TotalDeposit.TotalDepositAmount,0)TotalDepositAmount

	,fd.PeriodName
,fd.Year,ev.TIN
	,case when ev.LeftDate='19000101' then '' else ev.LeftDate end LeftDate 
	from " + hrmDB + @".dbo.EmployeeInfo einfo
	left outer join ( 
select distinct EmployeeId,sum(depositAmount)depositAmount from " + TaxDB + @".dbo.TaxDeposits
	where FiscalYearDetailId=@vFiscalYearDetailId
	group by EmployeeId 
 
) as CurrentDeposit on einfo.Id=CurrentDeposit.EmployeeId
	left outer join ( select distinct EmployeeId,sum(depositAmount)TotalDepositAmount
    from " + TaxDB + @".dbo.TaxDeposits where FiscalYearId=@FiscalYearId
and FiscalYearDetailId in(select id from   " + hrmDB + @".dbo.FiscalYearDetail

where FiscalYearId=@FiscalYearId and periodId <=@PeriodId)
	group by EmployeeId  )TotalDeposit on einfo.id=TotalDeposit.EmployeeId
	left outer join  " + hrmDB + @".dbo.View_TIBSalary as e on einfo.Id=e.EmployeeId  and e.FiscalYearDetailId=@vFiscalYearDetailId
	left outer join  " + hrmDB + @".dbo.ViewEmployeeInformation as ev on ev.Id=einfo.id
	left outer join " + TaxDB + @".dbo. TaxDeposits as TD on einfo.Id=td.EmployeeId  and TD.FiscalYearDetailId=@vFiscalYearDetailId 
	left outer join " + hrmDB + @".dbo.FiscalYearDetail fd on fd.Id=@vFiscalYearDetailId

	left outer join (select s.EmployeeId,Basic,HouseRent,Medical,TransportAllowance
	,(Basic+HouseRent+Medical+TransportAllowance)Gross
 
 from 
 (select distinct s.EmployeeId,MAX(s.FiscalYearDetailId)FiscalYearDetailId 
from " + hrmDB + @".dbo.ViewSalaryPreCalculation s
group by s.EmployeeId
 ) as s
 left outer join (select distinct fiscalyeardetailid, EmployeeId
,sum(case when Salaryhead='basic' then   amount else 0 end )Basic
,sum(case when Salaryhead='HouseRent' then   amount else 0 end )HouseRent
,sum(case when Salaryhead='Medical' then   amount else 0 end )Medical
,sum(case when Salaryhead='TransportAllowance' then   amount else 0 end )TransportAllowance
 from " + hrmDB + @".dbo.ViewSalaryPreCalculation 
 group by fiscalyeardetailid, EmployeeId) sal on s.EmployeeId=sal.EmployeeId
 and s.FiscalYearDetailId=sal.FiscalYearDetailId)PSale on einfo.Id=PSale.EmployeeId
 

	where einfo.id not in(select EmployeeId from " + hrmDB + @".dbo.EmployeeJob
	where LeftDate<@LeftDataRange
	and LeftDate not in('19000101')
	and einfo.id not in(select EmployeeId from " + hrmDB + @".dbo.EmployeeJob
	where JoinDate>@LastDayOfMonth))
    --and ev.IsTAXApplicable = 1
    and TotalDeposit.TotalDepositAmount > 0
	order by ev.DesignationOrder,e.SectionOrder,e.Code

";

                //sqlText += @" left outer join  " + hrmDB + ".dbo.FiscalYearDetail as f on t.FiscalYearDetailId=f.Id ";
                //sqlText += @" where    t.FiscalYearDetailId=@vFiscalYearDetailId and (t.DepositAmount>0 or p.TotalDepositAmount>0)   ";
                //sqlText += @" Order by s.SectionOrder, s.Code ";


                #endregion SqlText
                #region SqlExecution
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@vFiscalYearDetailId", vFiscalYearDetailId);


                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "LeftDate");
                dt = Ordinary.DtValueRound(dt, new string[] { "TAX", "TotalDepositAmount" });
                #endregion SqlExecution

                #endregion
                if (transaction != null)
                {
                    transaction.Commit();

                }
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


        //==================UpdateTotalTaxPayAmount =================
        public string[] UpdateTotalTaxPayAmount(Schedule1SalaryVM vm, string tType = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Schedule1SalaryMonthly Update"; //Method Name
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

                    #region Update
                    sqlText = " ";
                    sqlText += @"
UPDATE Schedule1SalaryMonthlies SET TotalTaxPayAmount=a.TAXAmount
FROM 
(
SELECT DISTINCT FiscalYearDetailId, FiscalYearDetailIdTo, Schedule1Id,sum(TAXAmount)TAXAmount
FROM EmployeeTaxSlabDetailsMonthlies 
WHERE 1=1 
AND ISNULL(TransactionType,'Salary')=@TransactionType
AND (FiscalYearDetailId=@FiscalYearDetailId AND FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
AND EmployeeId=@EmployeeId
GROUP BY FiscalYearDetailId,FiscalYearDetailIdTo, Schedule1Id
) AS a 
WHERE 1=1
AND a.Schedule1Id=Schedule1SalaryMonthlies.Id
AND (a.FiscalYearDetailId=Schedule1SalaryMonthlies.FiscalYearDetailId  AND a.FiscalYearDetailIdTo=Schedule1SalaryMonthlies.FiscalYearDetailIdTo)                  
AND ISNULL(Schedule1SalaryMonthlies.TransactionType,'Salary')=@TransactionType
";


                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailIdTo", vm.FiscalYearDetailIdTo);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@TransactionType", tType);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    //if (transResult <= 0)
                    //{
                    //    retResults[3] = sqlText;
                    //    throw new ArgumentNullException("Unexpected error to update Schedule1 Salary Monthlies.", "");
                    //}

                    #endregion Update TotalTaxPayAmount In Schedule1SalaryMonthlies
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                }
                else
                {
                    throw new ArgumentNullException("Schedule1SalaryMonthly Update", "Could not found any item.");
                }
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = "Fail";//Success or Fail
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

        //==================UpdateFinalTaxAmount =================
        public string[] UpdateFinalTaxAmount(Schedule1SalaryVM vm, string tType = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Schedule1SalaryMonthly Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                string InvestmentDeductionFromTax = new SettingDAL().settingValue("Tax", "InvestmentDeductionFromTax");


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

                    #region Update
                    sqlText = " ";
                    sqlText += @"
------------
UPDATE Schedule1SalaryMonthlies SET ProcessedTaxAmount= ISNULL(FinalTax.TotalTaxPayAmount,0) - ISNULL(FinalTax.InvestmentTotalTaxNotPayAmount,0)
From(
select ssm.EmployeeId,  ISNULL(ssm.TotalTaxPayAmount, 0) TotalTaxPayAmount, ISNULL(sim.InvestmentTotalTaxNotPayAmount, 0) InvestmentTotalTaxNotPayAmount 
from  Schedule1SalaryMonthlies ssm
Left OUTER JOIN Schedule3InvestmentMonthlies sim ON ssm.EmployeeId = sim.EmployeeId
and (ssm.FiscalYearDetailId = sim.FiscalYearDetailId and ssm.FiscalYearDetailIdTo = sim.FiscalYearDetailIdTo)
and ISNULL(ssm.TransactionType,'Salary') = ISNULL(sim.TransactionType,'Salary')

where 1=1 
and (ssm.FiscalYearDetailId = @FiscalYearDetailId and ssm.FiscalYearDetailIdTo = @FiscalYearDetailIdTo)
and ssm.EmployeeId = @EmployeeId
and ISNULL(ssm.TransactionType,'Salary') = @TransactionType
)
 as FinalTax
where 1=1
and Schedule1SalaryMonthlies.EmployeeId = FinalTax.EmployeeId
and (Schedule1SalaryMonthlies.FiscalYearDetailId = @FiscalYearDetailId and Schedule1SalaryMonthlies.FiscalYearDetailIdTo = @FiscalYearDetailIdTo)
and Schedule1SalaryMonthlies.EmployeeId = @EmployeeId
AND ISNULL(Schedule1SalaryMonthlies.TransactionType,'Salary') = @TransactionType

------------
UPDATE Schedule1SalaryMonthlies SET FinalTaxAmount=
case 
when (TotalTaxPayAmount=0) or (ProcessedTaxAmount = 0) then 0
when (ProcessedTaxAmount < 0 and TotalTaxPayAmount > 0) or (ProcessedTaxAmount > 0 and ProcessedTaxAmount @rebate < (TaxSlabs.MinimumTAX/12) ) then (TaxSlabs.MinimumTAX/12) 

else ProcessedTaxAmount @rebate end


From TaxSlabs
where 1=1
and Schedule1SalaryMonthlies.TaxSlabId=TaxSlabs.id
and (Schedule1SalaryMonthlies.FiscalYearDetailId=@FiscalYearDetailId and Schedule1SalaryMonthlies.FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
and Schedule1SalaryMonthlies.EmployeeId=@EmployeeId
AND ISNULL(Schedule1SalaryMonthlies.TransactionType,'Salary') = @TransactionType
";

                    sqlText = sqlText.Replace("@rebate", InvestmentDeductionFromTax.ToLower() == "y" ? "-RebateAmountMonthly" : "");

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailIdTo", vm.FiscalYearDetailIdTo);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@TransactionType", tType);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    //if (transResult <= 0)
                    //{
                    //    retResults[3] = sqlText;
                    //    throw new ArgumentNullException("Unexpected error to update Schedule1 Salary Monthlies.", "");
                    //}

                    #endregion Update
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit

                    #endregion Commit
                }
                else
                {
                    throw new ArgumentNullException("Schedule1SalaryMonthly Update", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = "Fail";//Success or Fail
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

        public string[] UpdateFinalBonusTaxAmount(int FiscalYearDetailId, int FiscalYearDetailIdTo, string EmployeeId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Schedule1SalaryMonthly Update"; //Method Name
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

                #region Update
                sqlText = " ";
                sqlText += @"
--------------------------------
----declare @EmployeeId as varchar(100)
----declare @FiscalYearDetailId as int
----declare @FiscalYearDetailIdTo as int

----set @EmployeeId = '1_1'
----set @FiscalYearDetailId = 1054
----set @FiscalYearDetailIdTo = 1054
--------------------------------

UPDATE Schedule1SalaryMonthlies SET FinalBonusTaxAmount =fbt.BonusTax
FROM
(
select ISNULL(SUM(bt.Tax),0) BonusTax, bt.EmployeeId, bt.FiscalYearDetailId, bt.FiscalYearDetailIdTo
FROM
(
select ISNULL(FinalTaxAmount,0) Tax, EmployeeId, FiscalYearDetailId, FiscalYearDetailIdTo
From Schedule1SalaryMonthlies
where 1=1
and (FiscalYearDetailId=@FiscalYearDetailId and FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
and EmployeeId=@EmployeeId
and ISNULL(TransactionType,'Salary')='Bonus'

UNION ALL

select (-1)*ISNULL(FinalTaxAmount,0), EmployeeId, FiscalYearDetailId, FiscalYearDetailIdTo
From Schedule1SalaryMonthlies
where 1=1
and (FiscalYearDetailId=@FiscalYearDetailId and FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
and EmployeeId=@EmployeeId
and ISNULL(TransactionType,'Salary')='Salary'
) 
as bt
GROUP BY bt.EmployeeId, bt.FiscalYearDetailId, bt.FiscalYearDetailIdTo
) as fbt
where 1=1
and (Schedule1SalaryMonthlies.FiscalYearDetailId=fbt.FiscalYearDetailId and Schedule1SalaryMonthlies.FiscalYearDetailIdTo=fbt.FiscalYearDetailIdTo)
and Schedule1SalaryMonthlies.EmployeeId=fbt.EmployeeId
and ISNULL(Schedule1SalaryMonthlies.TransactionType,'Salary')='Bonus'
and (Schedule1SalaryMonthlies.FiscalYearDetailId=@FiscalYearDetailId and Schedule1SalaryMonthlies.FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
and Schedule1SalaryMonthlies.EmployeeId=@EmployeeId
";
                if (string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText = sqlText.Replace("Schedule1SalaryMonthlies.EmployeeId=@EmployeeId", "1=1");
                    sqlText = sqlText.Replace("EmployeeId=@EmployeeId", "1=1");
                }
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailIdTo", FiscalYearDetailIdTo);
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                //if (transResult <= 0)
                //{
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Unexpected error to update Schedule1 Salary Monthlies.", "");
                //}

                #endregion Update
                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";
                retResults[2] = EmployeeId.ToString();// Return Id
                retResults[3] = sqlText; //  SQL Query
                #region Commit

                #endregion Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = "Fail";//Success or Fail
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


        public string[] UpdateHouseRentMedicalConveyance(Schedule1SalaryVM vm, string tType = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Schedule1SalaryMonthly Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                string InvestmentDeductionFromTax = new SettingDAL().settingValue("Tax", "InvestmentDeductionFromTax");


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

                    #region Update

                    string RestMonth = @"   Select Line1A*12 Line1A,Line5A*12 Line5A,Line6A*12 Line6A,Line8A*12 Line8A from Schedule1SalaryMonthlies where EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId and TransactionType=@TransactionType";
                    SqlCommand cmdrm = new SqlCommand(RestMonth, currConn, transaction);
                    cmdrm.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdrm.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdrm.Parameters.AddWithValue("@TransactionType", "Salary");
                    SqlDataAdapter adapterrm = new SqlDataAdapter(cmdrm);
                    DataTable dt = new DataTable();
                    adapterrm.Fill(dt);                   
                  
                    if(dt.Rows.Count>0)
                    {
                        sqlText = " ";
                        sqlText += @"   Update Schedule1SalaryMonthlies set Line1A=@Line1A,Line1C=@Line1C, Line5A=@Line5A,Line5C=@Line5C,Line6A=@Line6A,Line6C=@Line6C,Line8A=@Line8A,Line8C=@Line8C  where EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId and TransactionType=@TransactionType ";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                        cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                        cmdUpdate.Parameters.AddWithValue("@TransactionType", tType);
                        cmdUpdate.Parameters.AddWithValue("@Line1A", dt.Rows[0]["Line1A"].ToString());
                        cmdUpdate.Parameters.AddWithValue("@Line1C", dt.Rows[0]["Line1A"].ToString());
                        cmdUpdate.Parameters.AddWithValue("@Line5A", dt.Rows[0]["Line5A"].ToString());
                        cmdUpdate.Parameters.AddWithValue("@Line5C", dt.Rows[0]["Line5A"].ToString());
                        cmdUpdate.Parameters.AddWithValue("@Line6A", dt.Rows[0]["Line6A"].ToString());
                        cmdUpdate.Parameters.AddWithValue("@Line6C", dt.Rows[0]["Line6A"].ToString());
                        cmdUpdate.Parameters.AddWithValue("@Line8A", dt.Rows[0]["Line8A"].ToString());
                        cmdUpdate.Parameters.AddWithValue("@Line8C", dt.Rows[0]["Line8A"].ToString());                   
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }

                    #endregion Update
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit

                    #endregion Commit
                }
                else
                {
                    throw new ArgumentNullException("Schedule1SalaryMonthly Update", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = "Fail";//Success or Fail
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


        public decimal GetBonusAmount(int FiscalYearDetailId, string EmployeeId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            decimal BonusAmount = 0;
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

                #region Update
                sqlText = " ";
                sqlText += @"
--------------------------------
----declare @EmployeeId as varchar(100)
----declare @FiscalYearDetailId as int

----set @EmployeeId = '1_1'
----set @FiscalYearDetailId = 1054
--------------------------------

select ISNULL(sum(Amount),0) Amount from SalaryBonusDetail 
WHERE 1=1
AND FiscalYearDetailId=@FiscalYearDetailId
AND EmployeeId=@EmployeeId
";
                if (string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText = sqlText.Replace("EmployeeId=@EmployeeId", "1=1");
                }
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                var exeRes = cmdUpdate.ExecuteScalar();

                BonusAmount = Convert.ToDecimal(exeRes);

                #endregion Update
                #region Commit

                #endregion Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
            }
            #region catch
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return BonusAmount;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return BonusAmount;
        }

        public decimal GetSchedule1Amount(int FiscalYearDetailId, string EmployeeId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            decimal Schedule1Amount = 0;
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

                #region Update
                sqlText = " ";
                sqlText += @"
--------------------------------
----declare @EmployeeId as varchar(100)
----declare @FiscalYearDetailId as int

----set @EmployeeId = '1_1'
----set @FiscalYearDetailId = 1054
--------------------------------

select ISNULL(sum(FinalTaxAmount),0) Amount from Schedule1SalaryMonthlies 
WHERE 1=1
AND FiscalYearDetailId=@FiscalYearDetailId
AND EmployeeId=@EmployeeId
AND ISNULL(TransactionType,'Salary') = 'Salary'
";
                if (string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText = sqlText.Replace("EmployeeId=@EmployeeId", "1=1");
                }
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                var exeRes = cmdUpdate.ExecuteScalar();

                Schedule1Amount = Convert.ToDecimal(exeRes);

                #endregion Update
                #region Commit

                #endregion Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
            }
            #region catch
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return Schedule1Amount;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return Schedule1Amount;
        }

        public decimal GetBonusTaxAmount(int FiscalYearDetailId, string EmployeeId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            decimal Schedule1Amount = 0;
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

                #region Update
                sqlText = " ";
                sqlText += @"
--------------------------------
----declare @EmployeeId as varchar(100)
----declare @FiscalYearDetailId as int

----set @EmployeeId = '1_1'
----set @FiscalYearDetailId = 1054
--------------------------------

select ISNULL(sum(FinalBonusTaxAmount),0) Amount from Schedule1SalaryMonthlies 
WHERE 1=1
AND FiscalYearDetailId=@FiscalYearDetailId
AND EmployeeId=@EmployeeId
AND ISNULL(TransactionType,'Salary') = 'Bonus'
";
                if (string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText = sqlText.Replace("EmployeeId=@EmployeeId", "1=1");
                }
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                var exeRes = cmdUpdate.ExecuteScalar();

                Schedule1Amount = Convert.ToDecimal(exeRes);

                #endregion Update
                #region Commit

                #endregion Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
            }
            #region catch
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return Schedule1Amount;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return Schedule1Amount;
        }


        //ProcessSASalaryM
        //==================SelectAll=================

        #region Processing Methods

        //==================InsertProcessUpdate =================
        public string[] InsertProcessUpdate(string FiscalYearDetailId, string FiscalYearDetailIdTo, string tType = "",
            ShampanIdentityVM auditvm = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            List<EmloyeeTAXSlabVM> vms = null, string advanceTax = "N", string fYear = "", string effectForm = "")
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
                string hrmDB = _dbsqlConnection.HRMDB;


                #region Checkpoint
                FiscalYearDAL _fyDAL = new FiscalYearDAL();
                FiscalYearDetailVM fydVM = new FiscalYearDetailVM();

                DataTable dtResult = _fyDAL.SelectFromToId(fYear);

                if (advanceTax == "Y")
                {
                    FiscalYearDetailId = dtResult.Rows[0][0].ToString();
                    FiscalYearDetailIdTo = dtResult.Rows[0][1].ToString();
                }

                if (FiscalYearDetailId == FiscalYearDetailIdTo)
                {

                    #region Previous Fiscal Period Status
                    fydVM = new FiscalYearDetailVM();

                    fydVM = _fyDAL.SelectAll_PreviousFiscalPeriod(Convert.ToInt32(FiscalYearDetailId), currConn, transaction).FirstOrDefault();

                    if (fydVM != null)
                    {
                        retResults[1] = "Previous Fiscal Period: " + fydVM.PeriodName + " must be Locked!";
                        return retResults;
                    }


                    #endregion

                    #region Current Fiscal Period Status
                    fydVM = new FiscalYearDetailVM();

                    fydVM = _fyDAL.SelectAll_FiscalYearDetail(Convert.ToInt32(FiscalYearDetailId), null, null, currConn, transaction).FirstOrDefault();

                    if (fydVM.PeriodLock)
                    {
                        retResults[1] = "This Fiscal Period: " + fydVM.PeriodName + " is Locked! Locked Data cannot be processed!";
                        return retResults;
                    }


                    #endregion

                }

                #endregion

                #region Bonus Check

                if (tType == "Bonus")
                {
                    decimal BonusAmount = 0;
                    BonusAmount = GetBonusAmount(Convert.ToInt32(FiscalYearDetailId), null, null, null);

                    if (BonusAmount <= 0)
                    {
                        retResults[1] = "No Data Found in Bonus!";
                        return retResults;
                    }
                }

                #endregion

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

                #region Schedule1 Check

                if (tType == "Bonus")
                {
                    decimal Schedule1Amount = 0;
                    Schedule1Amount = GetSchedule1Amount(Convert.ToInt32(FiscalYearDetailId), null, currConn, transaction);

                    if (Schedule1Amount <= 0)
                    {
                        retResults[1] = "No Data Found in Schedule1!";
                        return retResults;
                    }
                }

                #endregion

                #region Insert

                if (advanceTax == "N")
                {
                    retResults = InsertFromSalary(FiscalYearDetailId, FiscalYearDetailIdTo, tType, auditvm, currConn,
                        transaction, vms, advanceTax);
                }
                else if (advanceTax == "Y")
                {
                    retResults = InsertFromSalaryAdvance(FiscalYearDetailId, FiscalYearDetailIdTo, tType, auditvm,
                        currConn,
                        transaction, vms, fYear, effectForm);
                }


                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], "");
                }
                #endregion Insert

                #region Process
                #region Select Inserted Data
                Schedule1SalaryVM ssmVM = new Schedule1SalaryVM();
                List<Schedule1SalaryVM> ssmVMs = new List<Schedule1SalaryVM>();
                List<Schedule1SalaryVM> updatedSSMVMs = new List<Schedule1SalaryVM>();
                string[] conditionFields = { "ssm.FiscalYearDetailId", "ssm.FiscalYearDetailIdTo" };
                string[] conditionValues = { FiscalYearDetailId, FiscalYearDetailIdTo };
                ////string LineA = "";
                ssmVMs = SelectAll(0, conditionFields, conditionValues, tType, currConn, transaction, vms);//
                #endregion Select Inserted Data
                #region Ready For Update After Process

                

                #endregion Ready For Update After Process
                #endregion Process

                #region Update
                foreach (var item in ssmVMs)
                {
                    item.LastUpdateBy = auditvm.CreatedBy;
                    item.LastUpdateAt = auditvm.CreatedAt;
                    item.LastUpdateFrom = auditvm.CreatedFrom;

                    retResults = Update(item, tType, currConn, transaction);//
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                }

                #endregion Update

                #region InsertUpdate Schedule3InvestmentMonthly
                Schedule3InvestmentMonthlyDAL _simDAL = new Schedule3InvestmentMonthlyDAL();
                retResults = _simDAL.InsertUpdate(FiscalYearDetailId, FiscalYearDetailIdTo, tType, auditvm, currConn, transaction, vms);//

                if (tType.ToLower() == "bonus")
                {
                    string deleteExtraEmployee = string.Format(@"
delete Schedule1SalaryMonthlies where 
  FiscalYearDetailId=@fyid 
  and TransactionType = 'Bonus'
  and  EmployeeId  not in (
  select EmployeeId from {0}.dbo.SalaryBonusDetail
  where FiscalYearDetailId = @fyid)

delete from Schedule3InvestmentMonthlies where 1=1
  and  EmployeeId  not in (
  select EmployeeId from {0}.dbo.SalaryBonusDetail
  where FiscalYearDetailId = @fyid)
  and FiscalYearDetailId = @fyid


delete from EmployeeSchedule3TaxSlabDetailsMonthlies 
  where 1=1 and 
  EmployeeId not in (
  select EmployeeId from {0}.dbo.SalaryBonusDetail
  where FiscalYearDetailId = @fyid)
", hrmDB);

                    SqlCommand cmd = new SqlCommand(deleteExtraEmployee, currConn, transaction);
                    cmd.Parameters.AddWithValue("@fyid", FiscalYearDetailId);//
                    cmd.ExecuteNonQuery();
                }

                if (advanceTax.ToLower() == "y")
                {
                    ProcessMonthlyTax(fYear, effectForm, hrmDB, currConn, transaction, vms);
                }

                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], "");
                }
                #endregion InsertUpdate Schedule3InvestmentMonthly


                #region Bonus Tax Amount
                if (tType == "Bonus")
                {
                    retResults = UpdateFinalBonusTaxAmount(Convert.ToInt32(FiscalYearDetailId), Convert.ToInt32(FiscalYearDetailIdTo), "", currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException("", retResults[1]);
                    }
                }


                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Schedule1 " + tType + " Monthly Saved Successfully!";
                #endregion Commit
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
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

        //==================InsertProcessUpdate =================
        public string[] InsertProcessUpdateNew(string FiscalYearDetailId, string FiscalYearDetailIdTo, string tType = "",
            ShampanIdentityVM auditvm = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            List<EmloyeeTAXSlabVM> vms = null, string advanceTax = "N", string fYear = "", string effectForm = "")
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
                string hrmDB = _dbsqlConnection.HRMDB;

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

                FiscalYearDAL _fyDAL = new FiscalYearDAL();
                FiscalYearDetailVM fydVM = new FiscalYearDetailVM();

                DataTable dtResult = _fyDAL.SelectFromToId(fYear);
                FiscalYearDetailId = dtResult.Rows[0][0].ToString();
                FiscalYearDetailIdTo = dtResult.Rows[0][1].ToString();
                
             
                FiscalYearDAL fiscalYearDAL = new FiscalYearDAL();
                FiscalYearVM fiscalYearVM = new FiscalYearVM();
                fiscalYearVM = fiscalYearDAL.SelectByYear(Convert.ToInt32(fYear));
                string fyearId= fiscalYearVM.Id;


                foreach (EmloyeeTAXSlabVM EmpVM in vms)
                {                  

                    decimal FinalTaxAmount = 0;

                string SQLFinalTaxAmount = @"select FinalTaxAmount  from Schedule1SalaryMonthlies 
                                where employeeid=@employeeid and FiscalYearDetailId=@effectForm and TransactionType='Salary'";
                SqlCommand cmdFinalTaxAmount = new SqlCommand(SQLFinalTaxAmount, currConn, transaction);
                cmdFinalTaxAmount.Parameters.AddWithValue("@employeeid", EmpVM.EmployeeId);
                cmdFinalTaxAmount.Parameters.AddWithValue("@effectForm", effectForm);
                cmdFinalTaxAmount.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);

                SqlDataAdapter adapter = new SqlDataAdapter(cmdFinalTaxAmount);
                DataTable DtFinalTaxAmount = new DataTable();
                    adapter.Fill(DtFinalTaxAmount);
                    if(DtFinalTaxAmount.Rows.Count>0)
                    {
                       FinalTaxAmount =Convert.ToDecimal(DtFinalTaxAmount.Rows[0]["FinalTaxAmount"].ToString());
                    }

                    for (int i = Convert.ToInt32(effectForm); i <= Convert.ToInt32(FiscalYearDetailIdTo); i++)
                    {
                        string YearlyTaxBreakDown = @"select *   from YearlyTaxBreakDown
                        where employeeid=@employeeid and FiscalDetailYearId = @FiscalDetailYearId ";


                        SqlCommand comYTBD = new SqlCommand(YearlyTaxBreakDown, currConn, transaction);
                        comYTBD.Parameters.AddWithValue("@employeeid", EmpVM.EmployeeId);
                        comYTBD.Parameters.AddWithValue("@FiscalDetailYearId", i.ToString());
                        SqlDataAdapter adapterYTBD = new SqlDataAdapter(comYTBD);
                        DataTable DtYTBD = new DataTable();
                        adapterYTBD.Fill(DtYTBD);
                        if (DtYTBD != null && DtYTBD.Rows.Count > 0)
                        {
                            sqlText = @"update YearlyTaxBreakDown set [Value]=@FinalTaxAmount 
					             where employeeid=@EmployeeId and FiscalDetailYearId = @FiscalDetailYearId ";
                            SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                            cmd.Parameters.AddWithValue("@EmployeeId", EmpVM.EmployeeId);
                            cmd.Parameters.AddWithValue("@FiscalDetailYearId", i.ToString());
                            cmd.Parameters.AddWithValue("@FinalTaxAmount", FinalTaxAmount);

                          
                            cmd.ExecuteNonQuery();
                        }
                        else {

                            sqlText = @" insert into YearlyTaxBreakDown(EmployeeId,FiscalYearId,FiscalDetailYearId,[Value],AdvanceTax) 
                            values(@EmployeeId,@FiscalYearId,@FiscalDetailYearId,@FinalTaxAmount,0)
                                    ";
                            SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                            cmd.Parameters.AddWithValue("@EmployeeId", EmpVM.EmployeeId);
                            cmd.Parameters.AddWithValue("@FiscalYearId", fyearId);
                            cmd.Parameters.AddWithValue("@FiscalDetailYearId", i.ToString());
                            cmd.Parameters.AddWithValue("@FinalTaxAmount", FinalTaxAmount);
                            cmd.ExecuteNonQuery();

                        }
                    }

                    if (advanceTax == "Y")
                    {
                        retResults = InsertFromSalaryAdvance(FiscalYearDetailId, FiscalYearDetailIdTo, tType, auditvm,
                            currConn,
                            transaction, vms, fYear, effectForm);
                    }
                    string hrmDb = _dbsqlConnection.HRMDB;
                    #region Process
                    #region Select Inserted Data
                    Schedule1SalaryVM ssmVM = new Schedule1SalaryVM();
                    List<Schedule1SalaryVM> ssmVMs = new List<Schedule1SalaryVM>();
                    List<Schedule1SalaryVM> updatedSSMVMs = new List<Schedule1SalaryVM>();
                    string[] conditionFields = { "ssm.FiscalYearDetailId", "ssm.FiscalYearDetailIdTo" };
                    string[] conditionValues = { FiscalYearDetailId, FiscalYearDetailIdTo };
                    ////string LineA = "";
                    ssmVMs = SelectAll(0, conditionFields, conditionValues, tType, currConn, transaction, vms);//
                    #endregion Select Inserted Data
                    #region Ready For Update After Process



                    #endregion Ready For Update After Process
                    #endregion Process

                    #region Update
                    foreach (var item in ssmVMs)
                    {
                        item.LastUpdateBy = auditvm.CreatedBy;
                        item.LastUpdateAt = auditvm.CreatedAt;
                        item.LastUpdateFrom = auditvm.CreatedFrom;

                        retResults = Update(item, tType, currConn, transaction);//
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }

                    YearlyTAXVM vm = new YearlyTAXVM();
                    vm.EmployeeId = EmpVM.EmployeeId;

                    string fyid = @"SELECT MAX(Id) FROM  " + hrmDb + ".[dbo].[FiscalYearDetail] where PeriodName like 'Jan%'";
                    SqlCommand cmdfyid = new SqlCommand(fyid, currConn, transaction);
                    cmdfyid.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    SqlDataAdapter adapterfyid = new SqlDataAdapter(cmdfyid);
                    DataTable dtfyid = new DataTable();
                    adapterfyid.Fill(dtfyid);
                    decimal fyscalyearid = Convert.ToDecimal(dtfyid.Rows[0][0].ToString());

                    string id = @"SELECT [Id]  FROM YearlyTaxBreakDown where EmployeeId=@EmployeeId and FiscalDetailYearId=@FiscalDetailYearId";
                    SqlCommand cmdid = new SqlCommand(id, currConn, transaction);
                    cmdid.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdid.Parameters.AddWithValue("@FiscalDetailYearId", fyscalyearid);
                    SqlDataAdapter adapterid = new SqlDataAdapter(cmdid);
                    DataTable dtid = new DataTable();
                    adapterid.Fill(dtid);
                    string fid = dtid.Rows[0][0].ToString();

                    vm.Id = fid;
                    retResults = UpdateAdvanceTax(vm, currConn, transaction);

                    #endregion Update
 

                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Schedule1 " + tType + " Monthly Saved Successfully!";
                #endregion Commit
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
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


        private void ProcessMonthlyTax(string fYear, string effectForm, string hrmDB, SqlConnection currConn,
            SqlTransaction transaction,List<EmloyeeTAXSlabVM> vms = null)
        {
            try
            {
                // insert into the new TAX table
                DataTable dtyearlyTAX = new DataTable();
                DataTable dtfiscalYears = new DataTable();
                DataTable dtDepositAmounts = new DataTable();
                DataTable dtFinalResult = new DataTable();

                dtFinalResult.Columns.Add("EmployeeId");
                dtFinalResult.Columns.Add("FiscalYearId");
                dtFinalResult.Columns.Add("FiscalDetailYearId");
                dtFinalResult.Columns.Add("Value");
                dtFinalResult.Columns.Add(new DataColumn() { DefaultValue = 0, ColumnName = "AdvanceTax" });

                string getEmployees = @"
select * from Schedule1SalaryMonthlies ssm left outer join TaxSlabs ts on ssm.TaxSlabId = ts.Id 
where TransactionType='YearlyTax' and Year = @year";

                if (vms != null && vms.Count > 0)
                {
                    getEmployees += " and EmployeeId in ('" +
                                    string.Join("','", vms.Select(x => x.EmployeeId)) + "')";
                }

                string getFiscalYears = string.Format(@"select * from {0}.dbo.FiscalYearDetail
where id >= @effectForm", hrmDB);

                string getDepositAmount = string.Format(@"
select EmployeeId , Sum(DepositAmount)DepositAmount from TaxDeposits
where FiscalYearDetailId IN (SELECT ID from {0}.dbo.FiscalYearDetail where id > @effectForm and year = @year)
group by EmployeeId
", hrmDB);

                SqlCommand command = new SqlCommand(getEmployees, currConn, transaction);
                command.Parameters.AddWithValue("@year", fYear);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dtyearlyTAX);

                command.CommandText = getFiscalYears;
                command.Parameters.AddWithValue("@effectForm", effectForm);
                adapter.Fill(dtfiscalYears);

                command.CommandText = getDepositAmount;
                adapter.Fill(dtDepositAmounts);

                int monthCount = GetMonthCount(fYear, effectForm, currConn, transaction, hrmDB);

                string getAdjustment =
                    string.Format(@"

select EmployeeId, sum(AdjustmentValue)AdjustmentValue from (
select EmployeeId, sum(DepositAmount)AdjustmentValue from TaxDeposits ybd
                left outer join {0}.dbo.FiscalYearDetail fd on ybd.FiscalYearDetailId = fd.Id
                where FiscalYearDetailId < @effectForm and fd.year = @year
                group by EmployeeId

union 

select EmployeeId, sum(AdvanceTax)AdjustmentValue from YearlyTaxBreakDown ybd
                left outer join {0}.dbo.FiscalYearDetail fd on ybd.FiscalDetailYearId = fd.Id
                where FiscalDetailYearId < @effectForm and fd.year = @year
                group by EmployeeId

			)Adjustment
group by EmployeeId


", hrmDB);

                command.CommandText = getAdjustment;
                DataTable dtAdjustment = new DataTable();
                adapter.Fill(dtAdjustment);



                foreach (DataRow dataRow in dtfiscalYears.Rows)
                {
                    // prepare data table
                    foreach (DataRow row in dtyearlyTAX.Rows)
                    {
                        DataRow[] rows = dtDepositAmounts.Select("EmployeeId = '" + row["EmployeeId"] + "'");
                        DataRow[] adjustDataRows = dtAdjustment.Select("EmployeeId = '" + row["EmployeeId"] + "'");
                        decimal totalDeposit = 0;
                        decimal adjustmentValue = 0;

                        if (rows.Length > 0)
                        {
                            totalDeposit = Convert.ToDecimal(rows[0]["DepositAmount"]);
                        }

                        if (adjustDataRows.Length > 0)
                        {
                            adjustmentValue = Convert.ToDecimal(adjustDataRows[0]["AdjustmentValue"]);
                        }

                        decimal finalTaxAmount = Convert.ToDecimal(row["FinalTaxAmount"]) > 0 &&
                                                 Convert.ToDecimal(row["FinalTaxAmount"]) <
                                                 Convert.ToDecimal(row["MinimumTAX"])
                            ? Convert.ToDecimal(row["MinimumTAX"])
                            : Convert.ToDecimal(row["FinalTaxAmount"]);

                        decimal finalTaxMonthly = (finalTaxAmount - totalDeposit - adjustmentValue) / monthCount;
                        dtFinalResult.Rows.Add(row["EmployeeId"], dataRow["FiscalYearId"], dataRow["Id"],
                            finalTaxMonthly, 0);
                    }
                }

                // delete existing data
                CommonDAL commonDal = new CommonDAL();

                string createTempTable = @" 
CREATE TABLE #YearlyTaxBreakDown(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [varchar](50) NULL,
	[FiscalYearId] [varchar](50) NULL,
	[FiscalDetailYearId] [varchar](50) NULL,
	[Value] [decimal](18, 5) NULL,
	[AdvanceTax] [decimal](18, 5) NULL
	) ";

                command = new SqlCommand(createTempTable, currConn, transaction);
                command.ExecuteNonQuery();

                string[] result = commonDal.BulkInsert("#YearlyTaxBreakDown", dtFinalResult, currConn, transaction);

                //string fiscalYearId = dtfiscalYears.DefaultView.ToTable(true, "FiscalYearId").Rows[0][0].ToString();

                string deleteText = @"

delete YearlyTaxBreakDown from YearlyTaxBreakDown yd
join #YearlyTaxBreakDown ydt
on ydt.EmployeeId = yd.EmployeeId
and ydt.FiscalDetailYearId = yd.FiscalDetailYearId

drop table #YearlyTaxBreakDown ";

                command.CommandText = deleteText;
                // command.Parameters.AddWithValue("@FiscalYearId", fiscalYearId);
                command.ExecuteNonQuery();

                // insert new data

                result = commonDal.BulkInsert("YearlyTaxBreakDown", dtFinalResult, currConn, transaction);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private int GetMonthCount(string year, string effectFrom, SqlConnection connection, SqlTransaction transaction, string hrmDB)
        {
            string getFiscalCount = @"
select count(Id) from "+hrmDB+@".dbo.FiscalYearDetail
where Id >= @effectForm  and Year = @year";

            SqlCommand command = new SqlCommand(getFiscalCount, connection, transaction);
            command.Parameters.AddWithValue("@year", year);
            command.Parameters.AddWithValue("@effectForm", effectFrom);
            command.CommandText = getFiscalCount;
            int monthCount = Convert.ToInt32(command.ExecuteScalar());
            return monthCount;
        }

        //==================InsertFromSalary =================
        public string[] InsertFromSalary(string FiscalYearDetailId, string FiscalYearDetailIdTo, string tType = "",
            ShampanIdentityVM auditvm = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            List<EmloyeeTAXSlabVM> vms = null, string advanceTax = "N")
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertFromSalary"; //Method Name
            int transResult = 0;
            string sqlText = "";
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            try
            {

                #region Fiscal Period

                string TaxName = "";
                string PeriodFrom = "";
                string PeriodTo = "";

                FiscalYearDetailVM fydVM = new FiscalYearDetailVM();
                fydVM = new FiscalYearDAL().SelectAll_FiscalYearDetail(Convert.ToInt32(FiscalYearDetailId)).FirstOrDefault();
                PeriodFrom = fydVM.PeriodName;

                fydVM = new FiscalYearDetailVM();
                fydVM = new FiscalYearDAL().SelectAll_FiscalYearDetail(Convert.ToInt32(FiscalYearDetailIdTo)).FirstOrDefault();
                PeriodTo = fydVM.PeriodName;

                TaxName = PeriodFrom + " to " + PeriodTo;
                string InvestmentDeductionFromTax = new SettingDAL().settingValue("Tax", "InvestmentDeductionFromTax");

                #endregion

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

                string hrmDB = _dbsqlConnection.HRMDB;

                ////string BonusWithSalary = "";
                ////BonusWithSalary = _settingDal.settingValue("Tax", "BonusWithSalary").Trim();

                ////string BonusNumber = "";
                ////BonusNumber = _settingDal.settingValue("Tax", "YearlyBonusNumber");

                #region Save
                #region SqlText

                sqlText = "  ";
                sqlText += @" 
------------------------------Declaration---------------
------declare @EmployeeId as varchar (100)
------declare @FiscalYearDetailId as int
------declare @FiscalYearDetailIdTo as int
------declare @TaxName as varchar (100)
------declare @TransactionType as varchar (100)


declare @FiscalYear as varchar(20)
declare @FiscalYearId as varchar(20)

------set @EmployeeId = '1_99'
------set @FiscalYearDetailId = '10'
------set @FiscalYearDetailIdTo = '10'
------set @TaxName = 'Jul-18 to Jun-19'
------set @TransactionType = 'Salary'
";

                sqlText += " SELECT @FiscalYear=[year],@FiscalYearId=FiscalYearId";
                sqlText += " FROM " + hrmDB + ".[dbo].FiscalYearDetail WHERE Id=@FiscalYearDetailId ";

                sqlText += @" 

------------------------------Delete First-----------------
DELETE from Schedule1SalaryMonthlies
WHERE 1=1
AND (FiscalYearDetailId=@FiscalYearDetailId AND FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
AND ISNULL(TransactionType,'Salary') = @TransactionType

";
                if (vms != null && vms.Count > 0)
                {
                    sqlText += " AND EmployeeId in ('" + string.Join("','", vms.Select(x => x.EmployeeId)) + "')";
                }

                sqlText += @"
DELETE from EmployeeTaxSlabDetailsMonthlies
WHERE 1=1
AND (FiscalYearDetailId=@FiscalYearDetailId AND FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
AND ISNULL(TransactionType,'Salary') = @TransactionType
";
                if (vms != null && vms.Count > 0)
                {
                    sqlText += " AND EmployeeId in ('" + string.Join("','", vms.Select(x => x.EmployeeId)) + "')";
                }

                sqlText += @" 
------------------------------Insert------------------
INSERT INTO Schedule1SalaryMonthlies (
TaxSlabId
,EmployeeId,ProjectId,DepartmentId,SectionId,DesignationId
,FiscalYearId,Year,FiscalYearDetailId, FiscalYearDetailIdTo
,TaxName
,Line15A, Line3A, Line14A
,Line1A,Line5A,Line6A,Line8A
, Line13A
, Line20A
,TransactionType
,Remarks, IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
)
 
";


                sqlText += @"  
select TaxSlabId
,salary.EmployeeId
,ProjectId,DepartmentId,SectionId,DesignationId,@FiscalYearId FiscalYearId,@FiscalYear Year
,@FiscalYearDetailId FiscalYearDetailId
,@FiscalYearDetailIdTo FiscalYearDetailIdTo
,@TaxName TaxName

,ISNULL(pf.PFValue,0) @multiply PFValue 
,ISNULL(ar.Arrear,0) @multiply Arrear   

";
                if (tType == "Salary")
                {
                    sqlText = sqlText + @" , 0 Bonus ";
                }
                else if (tType == "Bonus" || tType == "YearlyTax" || tType == "YearlyTaxAdvanceTAX")
                {
                    sqlText = sqlText + @" ,ISNULL(b.Bonus,0) Bonus";
                }

                sqlText = sqlText + @"

,ISNULL(salary.Line1A,0) @multiply Line1A 
,ISNULL(salary.Line5A,0) @multiply Line5A 
,ISNULL(salary.Line6A,0) @multiply Line6A 
,ISNULL(salary.Line8A,0) @multiply Line8A 
,ISNULL(otAmnt.OTAmount,0) @multiply OTAmountLine13A 
,ISNULL(EL_Line_20.OTAmount,0) @multiply EL_Line_20 


,@TransactionType
,'NA', @IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom

from
(
select a.EmployeeId
,sum(Line1A)Line1A
,sum(Line5A)Line5A
,sum(Line6A)Line6A
,sum(Line8A)Line8A


from(
select 
sed.EmployeeId
,ISNULL(CASE WHEN  sed.SalaryType = 'Basic' THEN sed.Amount  else 0 END, 0) AS Line1A
,ISNULL(CASE WHEN   sed.SalaryType = 'HouseRent' THEN sed.Amount else 0 END, 0) AS Line5A
,ISNULL(CASE WHEN   sed.SalaryType = 'Medical' THEN sed.Amount  else 0 END, 0) AS Line6A
,ISNULL(CASE WHEN   sed.SalaryType = 'Conveyance' THEN sed.Amount  else 0 END, 0) AS Line8A 

";
                sqlText += @"  from " + hrmDB + @".dbo.SalaryEarningDetail sed
where 1=1 
AND (sed.FiscalYearDetailId>=@FiscalYearDetailId AND sed.FiscalYearDetailId<=@FiscalYearDetailIdTo)
) as a
group by a.EmployeeId
) as salary               
LEFT OUTER JOIN 
(
select EmployeeId, sum(spfe.PFValue) PFValue from " + hrmDB + @".dbo.SalaryPFDetailEmployeer as spfe
where 1=1 
AND (spfe.FiscalYearDetailId>=@FiscalYearDetailId AND spfe.FiscalYearDetailId<=@FiscalYearDetailIdTo)
GROUP BY EmployeeId
) as pf ON salary.EmployeeId=pf.EmployeeId

LEFT OUTER JOIN 
(
select EmployeeId, ISNULL(sum(sbd.Amount),0) Bonus from " + hrmDB + @".dbo.SalaryBonusDetail as sbd
where 1=1 
AND (sbd.FiscalYearDetailId>=@FiscalYearDetailId AND sbd.FiscalYearDetailId<=@FiscalYearDetailIdTo)
GROUP BY EmployeeId
) as b ON salary.EmployeeId=b.EmployeeId
LEFT OUTER JOIN 
(
select EmployeeId, ISNULL(sum(soe.EarningAmount),0) Arrear from " + hrmDB + @".dbo.SalaryOtherEarning as soe
where 1=1 
and soe.EarningTypeId in(select Id from " + hrmDB + @".dbo.EarningDeductionType where Name = 'Arrear')
AND (soe.FiscalYearDetailId>=@FiscalYearDetailId AND soe.FiscalYearDetailId<=@FiscalYearDetailIdTo)
GROUP BY EmployeeId
) as ar ON salary.EmployeeId=ar.EmployeeId

left outer join 
(
select distinct EmployeeId, sum(isnull(EarningAmount,0)) OTAmount from " + hrmDB + @".dbo.SalaryOtherEarning
where 1=1
and earningTypeid=(select id from " + hrmDB + @".dbo.EarningDeductionType 
where taxheadid in(
select Id from " + hrmDB + @".dbo.TAXHeadMapping where headName='OT_Line_13' ))
--and EmployeeId = '1_50' 
AND (FiscalYearDetailId>=@FiscalYearDetailId AND FiscalYearDetailId<=@FiscalYearDetailIdTo)
group by EmployeeId
) as otAmnt ON salary.EmployeeId=otAmnt.EmployeeId

left outer join 
(
select distinct EmployeeId, sum(isnull(EarningAmount,0)) OTAmount from " + hrmDB + @".dbo.SalaryOtherEarning
where 1=1
and earningTypeid=(select id from " + hrmDB + @".dbo.EarningDeductionType 
where taxheadid in(
select Id from " + hrmDB + @".dbo.TAXHeadMapping where headName='EL_Line_20' ))
--and EmployeeId = '1_50' 
AND (FiscalYearDetailId>=@FiscalYearDetailId AND FiscalYearDetailId<=@FiscalYearDetailIdTo)
group by EmployeeId
) as EL_Line_20 ON salary.EmployeeId=EL_Line_20.EmployeeId

LEFT OUTER JOIN " + hrmDB + @".dbo.ViewEmployeeInformation ve ON Salary.EmployeeId = ve.EmployeeId
LEFT OUTER JOIN EmloyeeTAXSlabs ets ON Salary.EmployeeId = ets.EmployeeId



";
                if (vms != null && vms.Count > 0)
                {
                    sqlText += " where 1=1 and ve.EmployeeId in ('" + string.Join("','", vms.Select(x => x.EmployeeId)) + "')";
                }

                if (advanceTax == "N")
                {
                    sqlText = sqlText.Replace("@multiply", "");
                }
                else if (advanceTax == "Y")
                {
                    sqlText = sqlText.Replace("@multiply", "*12");
                }


                #endregion

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValue("@TaxName", TaxName);
                cmdInsert.Parameters.AddWithValue("@TransactionType", tType);
                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdInsert.Parameters.AddWithValue("@FiscalYearDetailIdTo", FiscalYearDetailIdTo);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);

                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                string rebateAmountGen = RebateAmountGen("", FiscalYearDetailId, FiscalYearDetailIdTo, tType, vms,"","N",hrmDB);
                cmdInsert.CommandText = rebateAmountGen;
                cmdInsert.ExecuteNonQuery();

                //                string updateSalaryTax = @"
                //update Schedule1SalaryMonthlies 
                //set SalaryTAX = 0
                //where TotalTaxPayAmount = 0
                //
                //update Schedule1SalaryMonthlies set 
                //FinalTaxAmount = (case when (TotalTaxPayAmount";

                //                if (InvestmentDeductionFromTax.ToLower() == "y")
                //                {
                //                    updateSalaryTax += "-RebateAmountMonthly";
                //                }

                //                updateSalaryTax +=
                //                    @") < (minimumTax/12) then (minimumTax/12) else (TotalTaxPayAmount";

                //                if (InvestmentDeductionFromTax.ToLower() == "y")
                //                {
                //                    updateSalaryTax += "-RebateAmountMonthly";
                //                }

                //updateSalaryTax += @") end)
                //from 
                //Schedule1SalaryMonthlies sm
                //left outer join TaxSlabs ts on sm.TaxSlabId = ts.Id";



                //cmdInsert.CommandText = updateSalaryTax;
                //cmdInsert.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    retResults[1] = "No Data Found in Salary Earning!";
                    return retResults;
                    //throw new ArgumentNullException(retResults[1], "");
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
                retResults[1] = "Schedule1 Salary Monthly Saved Successfully!";
                #endregion Commit
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message;
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
        public string[] InsertFromSalaryAdvance(string FiscalYearDetailId, string FiscalYearDetailIdTo, string tType = "",
            ShampanIdentityVM auditvm = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            List<EmloyeeTAXSlabVM> vms = null, string fiscalYear = "", string effectForm = "")
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertFromSalary"; //Method Name
            int transResult = 0;
            string sqlText = "";
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            try
            {

                #region Fiscal Period

                string TaxName = "";
                string PeriodFrom = "";
                string PeriodTo = "";

                FiscalYearDetailVM fydVM = new FiscalYearDetailVM();
                fydVM = new FiscalYearDAL().SelectAll_FiscalYearDetail(Convert.ToInt32(FiscalYearDetailId)).FirstOrDefault();
                PeriodFrom = fydVM.PeriodName;

                fydVM = new FiscalYearDetailVM();
                fydVM = new FiscalYearDAL().SelectAll_FiscalYearDetail(Convert.ToInt32(FiscalYearDetailIdTo)).FirstOrDefault();
                PeriodTo = fydVM.PeriodName;

                TaxName = PeriodFrom + " to " + PeriodTo;

                #endregion

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

                string hrmDB = _dbsqlConnection.HRMDB;


                #region Save
                #region SqlText

                sqlText = "  ";
                sqlText += string.Format(@" 
------------------------------Declaration---------------
------declare @EmployeeId as varchar (100)
------declare @FiscalYearDetailId as int
------declare @FiscalYearDetailIdTo as int
------declare @TaxName as varchar (100)
------declare @TransactionType as varchar (100)
---declare @FiscalYear as varchar(20)
------set @EmployeeId = '1_99'
------set @FiscalYearDetailId = '10'
------set @FiscalYearDetailIdTo = '10'
------set @TaxName = 'Jul-18 to Jun-19'
------set @TransactionType = 'Salary'

declare @FiscalYearDetailId  as varchar (50)
declare @FiscalYearDetailIdTo  as varchar (50)

declare @FiscalYearId as varchar(20)


select @FiscalYearId=ID from {0}.dbo.FiscalYear
where Year = @year

select  @FiscalYearDetailId=min(id), @FiscalYearDetailIdTo=max(Id) from {0}.dbo.FiscalYearDetail
where Year = @year

", hrmDB);

                //sqlText += " SELECT @FiscalYear=[year],@FiscalYearId=FiscalYearId";
                //sqlText += " FROM " + hrmDB + ".[dbo].FiscalYearDetail WHERE Id=@FiscalYearDetailId ";

                sqlText += @" 

------------------------------Delete First-----------------
DELETE from Schedule1SalaryMonthlies
WHERE 1=1
AND (FiscalYearDetailId=@FiscalYearDetailId AND FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
AND ISNULL(TransactionType,'Salary') = @TransactionType

";
                if (vms != null && vms.Count > 0)
                {
                    sqlText += " AND EmployeeId in ('" + string.Join("','", vms.Select(x => x.EmployeeId)) + "')";
                }

                sqlText += @"
DELETE from EmployeeTaxSlabDetailsMonthlies
WHERE 1=1
AND (FiscalYearDetailId=@FiscalYearDetailId AND FiscalYearDetailIdTo=@FiscalYearDetailIdTo)
AND ISNULL(TransactionType,'Salary') = @TransactionType
";
                if (vms != null && vms.Count > 0)
                {
                    sqlText += " AND EmployeeId in ('" + string.Join("','", vms.Select(x => x.EmployeeId)) + "')";
                }



                sqlText += @" 
------------------------------Insert------------------
INSERT INTO Schedule1SalaryMonthlies (
TaxSlabId
,EmployeeId,ProjectId,DepartmentId,SectionId,DesignationId
,FiscalYearId,Year,FiscalYearDetailId, FiscalYearDetailIdTo
,TaxName
,Line15A, Line3A, Line14A
,Line1A,Line5A,Line6A,Line8A
, Line13A
,Line19A
, Line20A
,TransactionType
,Remarks, IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
)
 
";


                sqlText += string.Format(@"  


select ts.TaxSlabId,ev.EmployeeId,ev.ProjectId,ev.DepartmentId,ev.SectionId,ev.DesignationId
,@FiscalYearId,@year,@FiscalYearDetailId,@FiscalYearDetailIdTo
,@TaxName TaxName,
 Line15A -- PF
,0 Line3A --Arrear
,Line14A -- Bonus

, Line1A --Basic
, Line5A --HouseRent

, Line6A+isnull(EmployeeSalary.StructureMedical,0) --Medical
, Line8A+isnull(EmployeeSalary.StructureTA,0) --Conveyance

,0 *@monthCount Line13A --OT
,(case when ev.IsCarTAXApplicable=1 then Line1A * (5/100.00) else 0 end)  Line19A --EL_Line_20
,0 *@monthCount Line20A --EL_Line_20
,'YearlyTaX', '' Reamrks, 1,0,1,1,1
 from  {0}.dbo.ViewEmployeeInformation ev 
left outer join EmloyeeTAXSlabs ts on ev.EmployeeId=ts.EmployeeId
left outer join 
( 


select 
a.EmployeeId
,isnull(a.Basic *(ef.PFValue/100.00),0) Line15A -- pf
,isnull(a.Basic,0) Line1A -- basic
,isnull(
(select top 1 StructureBasic from {0}.dbo.SalaryEmployee 
where EmployeeId=a.EmployeeId
and StructureBasic is not null
order by FiscalYearDetailId desc) * (select isnull(BonusNumber,2) from {0}.dbo.EmployeeJob where EmployeeId = a.EmployeeId)
,0) Line14A -- bonus
,isnull(((Basic*HouseRentFactor)/100),0) Line5A -- hr
,isnull(g.TAFactor*@monthCount,0) Line8A -- con
,isnull(g.MedicalFactor*@monthCount,0) Line6A -- medical

from (
select distinct a.EmployeeId,e.GradeId,sum(Basic)Basic
from(
select a.EmployeeId,sum(Basic)Basic
from (
select distinct EmployeeId,StructureBasic,StructureBasic*COUNT(FiscalyearDetailId) Basic
from {0}.dbo.SalaryEmployee where FiscalYearDetailId  in( 
select Id from {0}.dbo.FiscalYearDetail
where Id < @effectForm  and Year = @year
)
--and GradeId is null
group by EmployeeId,StructureBasic
) as a
group by  a.EmployeeId 
union all

select distinct EmployeeId
,sum(case when salarytype='Basic' then Amount else 0 end )*@monthCount Basic
from {0}.dbo.EmployeeSalaryStructureDetail
group by EmployeeId

) as a
left outer join {0}.dbo.ViewEmployeeInformation e on a.EmployeeId=e.EmployeeId
group by a.EmployeeId,e.GradeId
) as a
left outer join {0}.dbo.Grade g on a.GradeId=g.Id
left outer join {0}.dbo.EmployeePF ef on a.EmployeeId = ef.EmployeeId

) e on e.EmployeeId=ev.EmployeeId
left outer join 
(
    select EmployeeId,sum(StructureTA)StructureTA, sum(StructureMedical)StructureMedical
from {0}.dbo.SalaryEmployee
            WHERE FiscalYearDetailId IN (SELECT Id
                                                     FROM {0}.dbo.FiscalYearDetail
                                                     WHERE Id < @effectForm
                                                       AND Year = @year)
----and EmployeeId = '1_9'

group by EmployeeId

)EmployeeSalary on EmployeeSalary.EmployeeId=ev.EmployeeId


where 1=1

", hrmDB);

                if (vms != null && vms.Count > 0)
                {
                    sqlText += " and ev.EmployeeId in ('" + string.Join("','", vms.Select(x => x.EmployeeId)) + "')";
                }


                #endregion


                int monthCount = GetMonthCount(fiscalYear, effectForm, currConn, transaction,hrmDB);

                sqlText = sqlText.Replace("@monthCount", monthCount.ToString());

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValue("@TaxName", TaxName);
                cmdInsert.Parameters.AddWithValue("@TransactionType", tType);
                //cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                //cmdInsert.Parameters.AddWithValue("@FiscalYearDetailIdTo", FiscalYearDetailIdTo);
                cmdInsert.Parameters.AddWithValue("@IsActive", true);
                cmdInsert.Parameters.AddWithValue("@year", fiscalYear);
                cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmdInsert.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);
                cmdInsert.Parameters.AddWithValue("@effectForm", effectForm);

                var exec = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exec);

                string rebateAmountGen = RebateAmountGen("", "", "", tType, vms, fiscalYear,  "Y", hrmDB);
                cmdInsert.CommandText = rebateAmountGen;
                cmdInsert.ExecuteNonQuery();


                if (transResult <= 0)
                {
                    retResults[1] = "No Data Found in Salary Earning!";
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
                retResults[1] = "Schedule1 Salary Monthly Saved Successfully!";
                #endregion Commit
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message;
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

        private string RebateAmountGen(string id = "", string fiscalYearFromId = "", string fiscalYearToId = "", string transactonType = "",
            List<EmloyeeTAXSlabVM> vms = null, string year = "", string advanceTAX = "N", string hrmDb = "")
        {
            string rebateAmountGen = @"
----------------- Calculating Rebate Amount-----------------------------------------------------

update Schedule1SalaryMonthlies set 
TaxPaymentYearly = (TotalTaxableAmount) * @multiply -- *12
,InvestmentLimit = ((TotalTaxableAmount) * @multiply) *(20/100.00) -- *12
where 1=1 @condition

update Schedule1SalaryMonthlies set 
InvestmentLimit = 15000000
where InvestmentLimit > 15000000 @condition

--update Schedule1SalaryMonthlies set 
--RebateAmount = (case when TaxPaymentYearly > 1500000 then InvestmentLimit * (10/100.00) else InvestmentLimit * (15/100.00) end)
--where 1=1 @condition

update Schedule1SalaryMonthlies set 
RebateAmount = InvestmentLimit * (15/100.00)
where 1=1 @condition

update Schedule1SalaryMonthlies set RebateAmountMonthly = RebateAmount/@multiply where 1=1 @condition

update Schedule1SalaryMonthlies set RebateAmountMonthly = 0 where TotalTaxPayAmount = 0  @condition

";
            string conditionText = "";

            if (!string.IsNullOrEmpty(id))
            {
                conditionText += " and Id=@Id";
            }

            if (!string.IsNullOrEmpty(fiscalYearFromId))
            {
                conditionText += " and FiscalYearDetailId>=@FiscalYearDetailId ";
            }
            if (!string.IsNullOrEmpty(fiscalYearToId))
            {
                conditionText += " AND FiscalYearDetailId<=@FiscalYearDetailIdTo";
            }

            if (!string.IsNullOrEmpty(transactonType))
            {
                conditionText += " AND TransactionType=@TransactionType";
            }

            if (vms != null && vms.Count > 0)
            {
                conditionText += " AND EmployeeId in ('" + string.Join("','", vms.Select(x => x.EmployeeId)) + "')";
            }

            if (!string.IsNullOrEmpty(year))
            {
                conditionText += @" ANd FiscalYearDetailId>=(select  min(id) from " + hrmDb + @".dbo.FiscalYearDetail where Year = @year) 
                                    AND FiscalYearDetailId<=(select  max(id) from " + hrmDb + @".dbo.FiscalYearDetail where Year = @year)";
            }

            rebateAmountGen = rebateAmountGen.Replace("@condition", conditionText);

            rebateAmountGen = rebateAmountGen.Replace("@multiply", advanceTAX.ToLower() == "n" ? "1" : "12");

            return rebateAmountGen;
        }

        //==================ProcessSASalaryM =================
        public Schedule1SalaryVM ProcessSASalary(string LineA, int taxSlabId, bool isMonth = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            Schedule1SalaryVM vm = new Schedule1SalaryVM();
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
                #region Process
                int divisionFactor = 1;
                if (isMonth)
                {
                    divisionFactor = 12;
                }

                SettingDAL _settingDal = new SettingDAL();
                string AmountExYesorNo = _settingDal.settingValue("Tax", "AmountofExemptedIncome").Trim();
                string AmountEx = _settingDal.settingValue("Tax", "Exempted").Trim();
                string Divided = _settingDal.settingValue("Tax", "Divided").Trim();
                string DividedBonus = _settingDal.settingValue("Tax", "DividedBonus").Trim();

                SchedulePolicyDAL _schedulePolicyDAL = new SchedulePolicyDAL();
                SchedulePolicyVM schedulePolicyVM = new SchedulePolicyVM();
                List<SchedulePolicyVM> schedulePolicyVMs = new List<SchedulePolicyVM>();
                string[] conditionFields = { "sp.ScheduleNo" };
                string[] conditionValues = { "S1" };
                schedulePolicyVMs = _schedulePolicyDAL.SelectAll(0, conditionFields, conditionValues, currConn, transaction);

                string[] LineAValues = LineA.Split('~');
                #region Line 1 - Basic
                schedulePolicyVM = new SchedulePolicyVM();
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line1").FirstOrDefault();

                vm.Line1A = Convert.ToDecimal(LineAValues[1]);
                var basicAmount = vm.Line1A;
                if (schedulePolicyVM.FromBasic)
                {

                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {

                    }
                    else
                    {
                        vm.Line1B = 0;
                    }
                }
                vm.Line1C = vm.Line1A - vm.Line1B;
                #endregion Line 1
                #region Line 2 - Special Pay
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line2").FirstOrDefault();
                vm.Line2A = Convert.ToDecimal(LineAValues[2]);
                if (schedulePolicyVM.FromBasic)
                {

                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {

                    }
                    else
                    {
                        vm.Line2B = 0;
                    }
                }
                vm.Line2C = vm.Line2A - vm.Line2B;
                #endregion Line 2
                #region Line 3 - Arrear
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line3").FirstOrDefault();

                vm.Line3A = Convert.ToDecimal(LineAValues[3]);
                if (schedulePolicyVM.FromBasic)
                {
                    var portionAmount = basicAmount * schedulePolicyVM.BasicPortion / 100;
                    decimal[] ArrayNumbers = { portionAmount, schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line3A };
                    vm.Line3B = ArrayNumbers.Min();
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {
                        decimal[] ArrayNumbers = { schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line3A };
                        vm.Line3B = ArrayNumbers.Min();
                    }
                    else
                    {
                        vm.Line3B = 0;
                    }
                }
                vm.Line3C = vm.Line3A - vm.Line3B;
                #endregion Line 3
                #region Line 4 - Dearness Allowance
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line4").FirstOrDefault();

                vm.Line4A = Convert.ToDecimal(LineAValues[4]);
                if (schedulePolicyVM.FromBasic)
                {

                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {

                    }
                    else
                    {
                        vm.Line4B = 0;
                    }
                }
                vm.Line4C = vm.Line4A - vm.Line4B;
                #endregion Line 4
                #region Line 5 - House Rent
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line5").FirstOrDefault();

                vm.Line5A = Convert.ToDecimal(LineAValues[5]);
                if (schedulePolicyVM.FromBasic)
                {
                    var portionAmount = basicAmount * schedulePolicyVM.BasicPortion / 100;
                    decimal[] ArrayNumbers = { portionAmount, schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line5A };
                    vm.Line5B = ArrayNumbers.Min();
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {
                        decimal[] ArrayNumbers = { schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line5A };
                        vm.Line5B = ArrayNumbers.Min();
                    }
                    else
                    {
                        vm.Line5B = 0;
                    }
                }


                vm.Line5C = vm.Line5A - vm.Line5B;
                #endregion Line 5
                #region Line 6 - Medical
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line6").FirstOrDefault();
                vm.Line6A = Convert.ToDecimal(LineAValues[6]);

                if (schedulePolicyVM.FromBasic)
                {
                    var portionAmount = basicAmount * schedulePolicyVM.BasicPortion / 100;
                    decimal[] ArrayNumbers = { portionAmount, schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line6A };
                    vm.Line6B = ArrayNumbers.Min();
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {
                        decimal[] ArrayNumbers = { schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line6A };
                        vm.Line6B = ArrayNumbers.Min();
                    }
                    else
                    {
                        vm.Line6B = 0;
                    }
                }
                vm.Line6C = vm.Line6A - vm.Line6B;
                #endregion Line 6
                #region Line 7 - Critical Diseases
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line7").FirstOrDefault();
                vm.Line7A = Convert.ToDecimal(LineAValues[7]);

                if (schedulePolicyVM.FromBasic)
                {
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {
                        if (schedulePolicyVM.BasicPortion == 100 && schedulePolicyVM.EqualMaxMinAmount == 0)
                        {
                            vm.Line7B = vm.Line7A;
                        }
                    }
                    else
                    {
                        vm.Line7B = 0;
                    }
                }
                vm.Line7C = vm.Line7A - vm.Line7B;

                #endregion Line 7
                #region Line 8 - Conveyance
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line8").FirstOrDefault();
                vm.Line8A = Convert.ToDecimal(LineAValues[8]);

                if (schedulePolicyVM.FromBasic)
                {
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {
                        decimal[] ArrayNumbers = { schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line8A };
                        vm.Line8B = ArrayNumbers.Min();
                    }
                    else
                    {
                        vm.Line8B = 0;
                    }
                }
                vm.Line8C = vm.Line8A - vm.Line8B;

                #endregion Line 8
                #region Line 9 - Festival Allowance
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line9").FirstOrDefault();
                vm.Line9A = Convert.ToDecimal(LineAValues[9]);
                if (schedulePolicyVM.FromBasic)
                {

                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {

                    }
                    else
                    {
                        vm.Line9B = 0;
                    }
                }


                vm.Line9C = vm.Line9A - vm.Line9B;

                #endregion Line 9
                #region Line 10 - Servant Allowance
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line10").FirstOrDefault();
                vm.Line10A = Convert.ToDecimal(LineAValues[10]);

                if (schedulePolicyVM.FromBasic)
                {

                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {

                    }
                    else
                    {
                        vm.Line10B = 0;
                    }
                }
                vm.Line10C = vm.Line10A - vm.Line10B;


                #endregion Line 10
                #region Line 11 - Leave Allowance
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line11").FirstOrDefault();
                vm.Line11A = Convert.ToDecimal(LineAValues[11]);

                if (schedulePolicyVM.FromBasic)
                {

                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {

                    }
                    else
                    {
                        vm.Line11B = 0;
                    }
                }
                vm.Line11C = vm.Line11A - vm.Line11B;
                #endregion Line 11
                #region Line 12 - Honorarium
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line12").FirstOrDefault();
                vm.Line12A = Convert.ToDecimal(LineAValues[12]);

                if (schedulePolicyVM.FromBasic)
                {

                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {

                    }
                    else
                    {
                        vm.Line12B = 0;
                    }
                }
                vm.Line12C = vm.Line12A - vm.Line12B;
                #endregion Line 12
                #region Line 13 - OverTime
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line13").FirstOrDefault();
                vm.Line13A = Convert.ToDecimal(LineAValues[13]);

                if (schedulePolicyVM.FromBasic)
                {

                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {

                    }
                    else
                    {
                        vm.Line13B = 0;
                    }
                }
                vm.Line13C = vm.Line13A - vm.Line13B;
                #endregion Line 13
                #region Line 14 - Bonus
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line14").FirstOrDefault();
                vm.Line14A = Convert.ToDecimal(LineAValues[14]); //(vm.Line1A * Convert.ToDecimal(DividedBonus)) / 12; 

                if (schedulePolicyVM.FromBasic)
                {
                    var portionAmount = basicAmount * schedulePolicyVM.BasicPortion / 100;
                    decimal[] ArrayNumbers = { portionAmount, schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line14A };
                    vm.Line14B = ArrayNumbers.Min();
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {
                        decimal[] ArrayNumbers = { schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line14A };
                        vm.Line14B = ArrayNumbers.Min();
                    }
                    else
                    {
                        vm.Line14B = 0;
                    }
                }
                vm.Line14C = vm.Line14A - vm.Line14B;
                #endregion Line 14
                #region Line 15 - EmployeePF
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line15").FirstOrDefault();
                vm.Line15A = Convert.ToDecimal(LineAValues[15]);

                if (schedulePolicyVM.FromBasic)
                {
                    var portionAmount = basicAmount * schedulePolicyVM.BasicPortion / 100;
                    decimal[] ArrayNumbers = { portionAmount, schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line15A };
                    vm.Line15B = ArrayNumbers.Min();
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {
                        decimal[] ArrayNumbers = { schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line15A };
                        vm.Line15B = ArrayNumbers.Min();
                    }
                    else
                    {
                        vm.Line15B = 0;
                    }
                }
                vm.Line15C = vm.Line15A - vm.Line15B;

                #endregion Line 15
                #region Line 16 - PFInterest
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line16").FirstOrDefault();
                vm.Line16A = Convert.ToDecimal(LineAValues[16]);

                if (schedulePolicyVM.FromBasic)
                {
                    var portionAmount = basicAmount * schedulePolicyVM.BasicPortion / 100;
                    decimal[] ArrayNumbers = { portionAmount, vm.Line16A };
                    vm.Line16B = ArrayNumbers.Min();
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {

                    }
                    else
                    {
                        vm.Line16B = 0;
                    }
                }
                vm.Line16C = vm.Line16A - vm.Line16B;
                #endregion Line 16
                #region Line 17 - TransportIcome
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line17").FirstOrDefault();
                vm.Line17A = Convert.ToDecimal(LineAValues[17]);

                if (schedulePolicyVM.FromBasic)
                {
                    var portionAmount = basicAmount * schedulePolicyVM.BasicPortion / 100;
                    decimal[] ArrayNumbers = { portionAmount, schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line17A };
                    vm.Line17B = ArrayNumbers.Min();
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {

                    }
                    else
                    {
                        vm.Line17B = 0;
                    }
                }
                vm.Line17C = vm.Line17A - vm.Line17B;
                #endregion Line 17
                #region Line 18 - AccommodationIncome
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line18").FirstOrDefault();
                vm.Line18A = Convert.ToDecimal(LineAValues[18]);

                if (schedulePolicyVM.FromBasic)
                {
                    var portionAmount = basicAmount * schedulePolicyVM.BasicPortion / 100;
                    decimal[] ArrayNumbers = { portionAmount, schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line18A };
                    vm.Line18B = ArrayNumbers.Min();
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {

                    }
                    else
                    {
                        vm.Line18B = 0;
                    }
                }
                vm.Line18C = vm.Line18A - vm.Line18B;
                #endregion Line 18
                #region Line 19 - Other
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line19").FirstOrDefault();
                vm.Line19A = Convert.ToDecimal(LineAValues[19]);

                if (schedulePolicyVM.FromBasic)
                {
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {
                    }
                    else
                    {
                        vm.Line19B = 0;
                    }
                }
                vm.Line19C = vm.Line19A - vm.Line19B;
                #endregion Line 19
                #region Line 20 - Leave EnCash
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line20").FirstOrDefault();
                vm.Line20A = Convert.ToDecimal(LineAValues[20]);

                if (schedulePolicyVM.FromBasic)
                {
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {

                    }
                    else
                    {
                        vm.Line20B = 0;
                    }
                }
                vm.Line20C = vm.Line20A - vm.Line20B;
                #endregion Line 20
                #region Line 21 - Gratuity
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line21").FirstOrDefault();
                vm.Line21A = Convert.ToDecimal(LineAValues[21]);

                if (schedulePolicyVM.FromBasic)
                {
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {
                        decimal[] ArrayNumbers = { schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line21A };
                        vm.Line21B = ArrayNumbers.Min();
                    }
                    else
                    {
                        vm.Line21B = 0;
                    }
                }
                vm.Line21C = vm.Line21A - vm.Line21B;
                #endregion Line 21
                #region Line 22 - WPF
                //For Line 22
                schedulePolicyVM = schedulePolicyVMs.Where(c => c.LineNumber == "Line22").FirstOrDefault();
                vm.Line22A = Convert.ToDecimal(LineAValues[22]);

                if (schedulePolicyVM.FromBasic)
                {
                    var portionAmount = basicAmount * schedulePolicyVM.BasicPortion / 100;
                    decimal[] ArrayNumbers = { portionAmount, schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line22A };
                    vm.Line22B = ArrayNumbers.Min();
                }
                else
                {
                    if (schedulePolicyVM.IsFixed)
                    {
                        decimal[] ArrayNumbers = { schedulePolicyVM.EqualMaxMinAmount / divisionFactor, vm.Line22A };
                        vm.Line21B = ArrayNumbers.Min();
                    }
                    else
                    {
                        vm.Line17B = 0;
                    }
                }
                vm.Line22C = vm.Line22A - vm.Line22B;
                #endregion Line 22

                #region TotalTaxPayAmount Calculation

                vm.TotalIncomeAmount = vm.Line1A + vm.Line2A + vm.Line3A + vm.Line4A + vm.Line5A + vm.Line6A + vm.Line7A
                    + vm.Line8A + vm.Line9A + vm.Line10A + vm.Line11A + vm.Line12A + vm.Line13A + vm.Line14A + vm.Line15A
                    + vm.Line16A + vm.Line17A + vm.Line18A + vm.Line19A + vm.Line20A + vm.Line21A + vm.Line22A;

                vm.TotalExemptedAmount = vm.Line1B + vm.Line2B + vm.Line3B + vm.Line4B + vm.Line5B + vm.Line6B + vm.Line7B
                    + vm.Line8B + vm.Line9B + vm.Line10B + vm.Line11B + vm.Line12B + vm.Line13B + vm.Line14B + vm.Line15B
                    + vm.Line16B + vm.Line17B + vm.Line18B + vm.Line19B + vm.Line20B + vm.Line21B + vm.Line22B;

                vm.TotalTaxableAmount = vm.Line1C + vm.Line2C + vm.Line3C + vm.Line4C + vm.Line5C + vm.Line6C + vm.Line7C
                    + vm.Line8C + vm.Line9C + vm.Line10C + vm.Line11C + vm.Line12C + vm.Line13C + vm.Line14C + vm.Line15C
                    + vm.Line16C + vm.Line17C + vm.Line18C + vm.Line19C + vm.Line20C + vm.Line21C + vm.Line22C;

                if(isMonth==true)
                {
                    decimal OneThird = vm.TotalIncomeAmount / decimal.Parse(Divided);
                    decimal monthlyEx = decimal.Parse(AmountEx) / 12;
                    if (OneThird < monthlyEx)
                    {
                        vm.TotalTaxableAmount = Math.Round(vm.TotalIncomeAmount - OneThird);
                    }
                    else
                    {
                        vm.TotalTaxableAmount = Math.Round(vm.TotalIncomeAmount - decimal.Parse(AmountEx) / 12);
                    }

                }
                else
                {
                    decimal OneThird = vm.TotalIncomeAmount / decimal.Parse(Divided);
                    decimal monthlyEx = decimal.Parse(AmountEx);
                    if (OneThird < monthlyEx)
                    {
                        vm.TotalTaxableAmount = Math.Round(vm.TotalIncomeAmount - OneThird);
                    }
                    else
                    {
                        vm.TotalTaxableAmount = Math.Round(vm.TotalIncomeAmount - decimal.Parse(AmountEx));
                    }

                }    

                List<EmployeeTaxSlabDetailVM> taxSlabDetailVMs = new List<EmployeeTaxSlabDetailVM>();
                vm.TaxSlabId = taxSlabId;
                EmployeeTaxSlabDetailMonthlyDAL _employeeTaxSlabDetailDAL = new EmployeeTaxSlabDetailMonthlyDAL();
                taxSlabDetailVMs = _employeeTaxSlabDetailDAL.SelectEmployeeTaxSlabDetails(vm.TotalTaxableAmount, vm.TaxSlabId, isMonth, currConn, transaction);

                vm.employeeTaxSlabDetailVMs = taxSlabDetailVMs;


                #endregion TotalTaxPayAmount Calculation

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Process
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
            return vm;
        }

        ////UpdateSalaryTaxDetail
        public string[] UpdateSalaryTaxDetail(string FiscalYearDetailId, string tType = "", ShampanIdentityVM auditvm = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string advanceTAX = "N")
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "UpdateSalaryTaxDetail"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                string SalaryTaxPercent = "";
                string BonusTaxPercent = "";
                string TaxPercentByEmployee = "";
                string TaxValueFromEmployee = "";

                SalaryTaxPercent = new SettingDAL().settingValue("Tax", "SalaryTaxPercent");
                BonusTaxPercent = new SettingDAL().settingValue("Tax", "BonusTaxPercent");
             
                if (TaxPercentByEmployee.ToLower() == "y")
                {
                    BonusTaxPercent = "100";
                    SalaryTaxPercent = "100";
                }

                #region Checkpoint
                FiscalYearDAL _fyDAL = new FiscalYearDAL();
                FiscalYearDetailVM fydVM = new FiscalYearDetailVM();


                #region Previous Fiscal Period Status
                fydVM = new FiscalYearDetailVM();

                fydVM = _fyDAL.SelectAll_PreviousFiscalPeriod(Convert.ToInt32(FiscalYearDetailId), currConn, transaction).FirstOrDefault();

                if (fydVM != null)
                {
                    retResults[1] = "Previous Fiscal Period: " + fydVM.PeriodName + " must be Locked!";
                    return retResults;
                }


                #endregion

                #region Current Fiscal Period Status
                fydVM = new FiscalYearDetailVM();

                fydVM = _fyDAL.SelectAll_FiscalYearDetail(Convert.ToInt32(FiscalYearDetailId), null, null, currConn, transaction).FirstOrDefault();

                if (fydVM.PeriodLock)
                {
                    retResults[1] = "This Fiscal Period: " + fydVM.PeriodName + " is Locked! Locked Data cannot be processed!";
                    return retResults;
                }


                #endregion


                #endregion


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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSchedule1SalaryMonthly"); }
                #endregion open connection and transaction

                string hrmDB = _dbsqlConnection.HRMDB;

                #region Update Settings
                #region SqlText
                if (tType == "Salary")
                {


                    if (advanceTAX.ToLower() == "y")
                    {
                        sqlText = @" UPDATE " + hrmDB + ".dbo.SalaryTaxDetail ";
                        sqlText += " SET TaxValue=((YearlyTaxBreakDown.Value";

                        sqlText += ")*@SalaryTaxPercent/100)";
                        sqlText += " FROM YearlyTaxBreakDown";
                        sqlText += " WHERE YearlyTaxBreakDown.FiscalDetailYearId=" + hrmDB + ".dbo.SalaryTaxDetail.FiscalYearDetailId";
                        sqlText += " AND YearlyTaxBreakDown.employeeid=" + hrmDB + ".dbo.SalaryTaxDetail.employeeid";
                        sqlText += " AND YearlyTaxBreakDown.FiscalDetailYearId=@FiscalYearDetailId";
                        sqlText += " UPDATE " + hrmDB + ".dbo.SalaryTaxDetail SET TaxValue = 0 where TaxValue < 0";

                    }
                    else
                    {
                        #region Schedule1 Check

                        decimal Schedule1Amount = 0;
                        Schedule1Amount = GetSchedule1Amount(Convert.ToInt32(FiscalYearDetailId), null, null, null);

                        if (Schedule1Amount <= 0)
                        {
                            retResults[1] = "No Data Found in Schedule1!";
                            return retResults;
                        }

                        #endregion

                        sqlText = "";
                        sqlText = @" UPDATE " + hrmDB + ".dbo.SalaryTaxDetail ";
                        sqlText += " SET TaxValue=((Schedule1SalaryMonthlies.FinalTaxAmount";

                        sqlText += ")*@SalaryTaxPercent/100)";
                        sqlText += " FROM Schedule1SalaryMonthlies";
                        sqlText += " WHERE Schedule1SalaryMonthlies.FiscalYearDetailId=" + hrmDB + ".dbo.SalaryTaxDetail.FiscalYearDetailId";
                        sqlText += " AND Schedule1SalaryMonthlies.employeeid=" + hrmDB + ".dbo.SalaryTaxDetail.employeeid";
                        sqlText += " AND Schedule1SalaryMonthlies.FiscalYearDetailId=@FiscalYearDetailId";
                        sqlText += " AND ISNULL(Schedule1SalaryMonthlies.TransactionType,'Salary')=@TransactionType";
                        sqlText += " UPDATE " + hrmDB + ".dbo.SalaryTaxDetail SET TaxValue = 0 where TaxValue < 0";

                       
                    }
              
                }
                else if (tType == "Bonus")
                {

                    #region Schedule1 Check

                    decimal BonusTaxAmount = 0;
                    BonusTaxAmount = GetBonusTaxAmount(Convert.ToInt32(FiscalYearDetailId), null, null, null);

                    if (BonusTaxAmount <= 0)
                    {
                        retResults[1] = "No Data Found in Bonus Tax!";
                        return retResults;
                    }

                    #endregion

                    sqlText = "";
                    sqlText = " UPDATE " + hrmDB + ".dbo.SalaryBonusDetail ";
                    sqlText += " SET TaxValue=(Schedule1SalaryMonthlies.FinalBonusTaxAmount*@BonusTaxPercent/100)";

                    sqlText += " FROM Schedule1SalaryMonthlies";
                    sqlText += " WHERE Schedule1SalaryMonthlies.FiscalYearDetailId=" + hrmDB + ".dbo.SalaryBonusDetail.FiscalYearDetailId";
                    sqlText += " AND Schedule1SalaryMonthlies.employeeid=" + hrmDB + ".dbo.SalaryBonusDetail.employeeid";
                    sqlText += " AND Schedule1SalaryMonthlies.FiscalYearDetailId=@FiscalYearDetailId";
                    sqlText += " AND ISNULL(Schedule1SalaryMonthlies.TransactionType,'Salary')=@TransactionType";

                    if (TaxPercentByEmployee.ToLower() == "y")
                    {
                        sqlText += @"   UPDATE " + hrmDB + @".dbo.SalaryBonusDetail  
                        SET TaxValue = (TaxValue* isnull(" + hrmDB + @".dbo.EmployeeStructureGroup.BonusTaxPortion,0)/100)
                        from " + hrmDB + @".dbo.EmployeeStructureGroup
                        where 
                        " + hrmDB + ".dbo.SalaryBonusDetail.EmployeeId = " + hrmDB + ".dbo.EmployeeStructureGroup.EmployeeId  ";
                    }

                    sqlText += "";
                    sqlText += " UPDATE " + hrmDB + ".dbo.SalaryBonusDetail SET NetPayAmount=(Amount-TaxValue)";
                    sqlText += " WHERE FiscalYearDetailId=" + hrmDB + ".dbo.SalaryBonusDetail.FiscalYearDetailId";
                    sqlText += " AND FiscalYearDetailId=@FiscalYearDetailId";


                }

                #endregion SqlText
                #region SqlExecution
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdUpdate.Parameters.AddWithValue("@TransactionType", tType);

                if (tType == "Salary")
                {
                    cmdUpdate.Parameters.AddWithValue("@SalaryTaxPercent", SalaryTaxPercent);

                }
                else if (tType == "Bonus")
                {
                    cmdUpdate.Parameters.AddWithValue("@BonusTaxPercent", BonusTaxPercent);

                }

                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                if (transResult <= 0)
                {
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Unexpected error to update Salary Tax.", "");
                }
                #endregion SqlExecution

                #region Commit
                if (transResult <= 0)
                {
                    // throw new ArgumentNullException("Schedule1SalaryMonthly Update", vm.BranchId + " could not updated.");
                }
                #endregion Commit
                #endregion Update Settings
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = tType + " Tax Update Successfully.";
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
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

        #endregion Processing Methods


        #endregion Methods

    }
}




