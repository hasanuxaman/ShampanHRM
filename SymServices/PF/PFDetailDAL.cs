using SymOrdinary;
using SymServices.Common;
using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SymServices.PF
{
    public class PFDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        //==================SelectEmployeeList=================
        public List<PFDetailVM> SelectEmployeeList(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFDetailVM> VMs = new List<PFDetailVM>();
            PFDetailVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionPF();
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
                string hrmDB = _dbsqlConnection.HRMDB;

                #region sql statement
                #region SqlText
                sqlText = @"
SELECT  
pfd.Id
,pfd.PFHeaderId
,pfd.EmployeeId
,fyd.PeriodName
,ve.EmpName 
,ve.Code 
,pf.Code PFHeaderCode
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,pfd.BasicSalary
,pfd.EmployeePFValue
,pfd.EmployeerPFValue
FROM PFDetails pfd
LEFT OUTER JOIN PFHeader pf ON pf.Id=pfd.PFHeaderId 
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fyd ON pfd.FiscalYearDetailId=fyd.Id";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON pfd.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND pfd.IsArchive = 0 and fyd.PeriodName is not null
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
                    vm = new PFDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.PFHeaderCode = dr["PFHeaderCode"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.EmployeePFValue = Convert.ToDecimal(dr["EmployeePFValue"]);
                    vm.EmployeerPFValue = Convert.ToDecimal(dr["EmployeerPFValue"]);
                    //vm.TotalPF = vm.EmployeePFValue + vm.EmployeerPFValue;
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
        public List<PFDetailVM> SelectFiscalPeriod(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFDetailVM> VMs = new List<PFDetailVM>();
            PFDetailVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionPF();
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
                string hrmDB = _dbsqlConnection.GetConnection().Database;
                #region sql statement
                #region SqlText
                sqlText = @"
SELECT pfd.FiscalYearDetailId
,p.Name ProjectName
,p.Id ProjectId
,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd
,pfd.Post 
,sum(pfd.EmployeePFValue) EmployeePFValue
,sum(pfd.EmployeerPFValue) EmployerPFValue
,sum(pfd.EmployeePFValue)+sum(pfd.EmployeerPFValue) TotalPF
,ISNULL(pfd.IsBankDeposited,0) IsBankDeposited
FROM PFDetails pfd
";

                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].Project p ON pfd.ProjectId=p.Id";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fyd ON pfd.FiscalYearDetailId=fyd.Id";
                sqlText += "  LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON pfd.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND pfd.IsArchive = 0
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
                sqlText += "  GROUP BY p.Name,p.Id, pfd.FiscalYearDetailId, fyd.PeriodName, fyd.PeriodStart, fyd.PeriodEnd, pfd.Post, ISNULL(pfd.IsBankDeposited,0) ";
                sqlText += " ORDER BY fyd.PeriodStart DESC";

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
                    vm = new PFDetailVM();
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.ProjectName = dr["ProjectName"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.FiscalPeriod = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
                    vm.TotalEmployeeValue = Convert.ToDecimal(dr["EmployeePFValue"]);
                    vm.TotalEmployerValue = Convert.ToDecimal(dr["EmployerPFValue"]);
                    vm.TotalPF = Convert.ToDecimal(dr["TotalPF"]);
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsBankDeposited = Convert.ToBoolean(dr["IsBankDeposited"]);

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
        public List<PFHeaderVM> SelectFiscalPeriodHeader(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFHeaderVM> VMs = new List<PFHeaderVM>();
            PFHeaderVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionPF();
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
                string hrmDB = _dbsqlConnection.GetConnection().Database;
                #region sql statement
                #region SqlText
                sqlText = @"
SELECT 
pfd.Id
,pfd.Code
,pfd.FiscalYearDetailId
,p.Name ProjectName
,p.Id ProjectId
,fyd.PeriodName
,fyd.PeriodStart
,pfd.Post 
,pfd.EmployeePFValue EmployeePFValue
,pfd.EmployeerPFValue EmployerPFValue
,pfd.EmployeePFValue + pfd.EmployeerPFValue TotalPF

FROM PFHeader pfd
";

                sqlText += "  LEFT OUTER JOIN " + hrmDB + ".[dbo].Project p ON pfd.ProjectId=p.Id";
                sqlText += "  LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fyd ON pfd.FiscalYearDetailId=fyd.Id";
                sqlText += @" WHERE  1=1  AND pfd.IsArchive = 0
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
              //  sqlText += "  GROUP BY p.Name,p.Id, pfd.FiscalYearDetailId, fyd.PeriodName, fyd.PeriodStart, fyd.PeriodEnd, pfd.Post ";
                sqlText += " ORDER BY fyd.PeriodStart DESC";

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
                    vm = new PFHeaderVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.ProjectName = dr["ProjectName"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.FiscalPeriod = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.TotalEmployeeValue = Convert.ToDecimal(dr["EmployeePFValue"]);
                    vm.TotalEmployerValue = Convert.ToDecimal(dr["EmployerPFValue"]);
                    vm.TotalPF = Convert.ToDecimal(dr["TotalPF"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);

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
        public List<PFDetailVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFDetailVM> VMs = new List<PFDetailVM>();
            PFDetailVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionPF();
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
pfd.Id
,pfd.PFHeaderId
,pfd.FiscalYearDetailId
,pfd.PFStructureId
,pfd.ProjectId
,pfd.DepartmentId
,pfd.SectionId
,pfd.DesignationId
,pfd.EmployeeId
,pfd.EmployeePFValue
,pfd.EmployeerPFValue
,pfd.BasicSalary
,pfd.GrossSalary

,pfd.IsDistribute
,pfd.Post
,ISNULL(pfd.IsBankDeposited,0) IsBankDeposited

,pfd.Remarks
,pfd.IsActive
,pfd.IsArchive
,pfd.CreatedBy
,pfd.CreatedAt
,pfd.CreatedFrom
,pfd.LastUpdateBy
,pfd.LastUpdateAt
,pfd.LastUpdateFrom
,pfh.code


FROM PFDetails pfd
left outer join  PFHeader pfh on pfh.id=pfd.PFHeaderId
WHERE  1=1
";
                if (Id > 0)
                {
                    sqlText += @" and pfd.Id=@Id";
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
                    vm = new PFDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.PFStructureId = dr["PFStructureId"].ToString();
                    vm.PFHeaderId = dr["PFHeaderId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeePFValue = Convert.ToDecimal(dr["EmployeePFValue"]);
                    vm.EmployeerPFValue = Convert.ToDecimal(dr["EmployeerPFValue"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);

                    vm.IsDistribute = Convert.ToBoolean(dr["IsDistribute"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsBankDeposited = Convert.ToBoolean(dr["IsBankDeposited"]);
                    


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
        public List<PFDetailVM> SelectAllHeader(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFDetailVM> VMs = new List<PFDetailVM>();
            PFDetailVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionPF();
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
 
	   [Id]
      ,[Code]
      ,[FiscalYearDetailId]
      ,[ProjectId]
      ,[EmployeePFValue]
      ,[EmployeerPFValue]
      ,[Post]
      ,[Remarks]
      ,[IsActive]
      ,[IsArchive]
      ,[CreatedBy]
      ,[CreatedAt]
      ,[CreatedFrom]
      ,[LastUpdateBy]
      ,[LastUpdateAt]
      ,[LastUpdateFrom]

FROM PFHeader pfd 
WHERE  1=1
";
                if (Id > 0)
                {
                    sqlText += @" and pfd.Id=@Id";
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
                    vm = new PFDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PFStructureId = dr["PFStructureId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeePFValue = Convert.ToDecimal(dr["EmployeePFValue"]);
                    vm.EmployeerPFValue = Convert.ToDecimal(dr["EmployeerPFValue"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);

                    vm.IsDistribute = Convert.ToBoolean(dr["IsDistribute"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsBankDeposited = Convert.ToBoolean(dr["IsBankDeposited"]);
                    


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



        ////==================PFProcess =================
        public string[] PFProcess(string FiscalYearDetailId, string ProjectId, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PFProcess"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSchedule1SalaryMonthly"); }
                #endregion open connection and transaction

                string hrmDB = _dbsqlConnection.HRMDB;

                #region Checkpoint

                #region Post Check

                PFDetailVM varPFDetailVM = new PFDetailVM();
                string[] cFields = { "pfd.FiscalYearDetailId", "pfd.ProjectId" };
                string[] cValues = { FiscalYearDetailId, ProjectId };

                varPFDetailVM = SelectAll(0, cFields, cValues, currConn, transaction).FirstOrDefault();

                if (varPFDetailVM!=null &&  varPFDetailVM.Post)
                {
                    retResults[1] = "PF Already Posted for this Month! Can't Process!";
                    throw new ArgumentNullException("", retResults[1]);
                }

                CommonDAL _cDal = new CommonDAL();



                #endregion

                #endregion


                SettingDAL _settingDal = new SettingDAL();
                string PFFromPayroll = _settingDal.settingValue("PF", "FromPayroll").Trim();

                if (PFFromPayroll == "N")
                {
                    SalaryProcessDAL _salaryProcessDAL = new SalaryProcessDAL();
                    FiscalYearVM fiscalYearVM = new FiscalYearVM();
                    fiscalYearVM.BranchId = 1;
                    fiscalYearVM.CreatedAt = DateTime.Now.ToString("yyyyMMddhh:mm");
                    fiscalYearVM.CreatedBy = "Admin";
                    fiscalYearVM.CreatedFrom = "";

                   _salaryProcessDAL.SalaryPreProcessNew(Convert.ToInt32(FiscalYearDetailId), ProjectId, "0_0", "0_0", "0_0"
                , "0_0", "0_0", fiscalYearVM, "PF", "EGCB", null, null);

                }

                #region Checkpoint


                #region PF Check

                decimal PFAmount = 0;
                PFAmount = GetPFAmount(Convert.ToInt32(FiscalYearDetailId), null, null, null);

                if (PFAmount <= 0)
                {
                    retResults[1] = "No Data Found in Salary PF!";
                    return retResults;
                }

                #endregion

                #endregion

                sqlText = @"

insert into PFHeader (
Code       
,[Id]
,[FiscalYearDetailId]
,[ProjectId]
,[EmployeePFValue]
,[EmployeerPFValue]
,[Post]
,[Remarks]
,[IsActive]
,[IsArchive]
,[CreatedBy]
,[CreatedAt]
,[CreatedFrom]
,[LastUpdateBy]
,[LastUpdateAt]
,[LastUpdateFrom]
)
select 
@code
,@Id
,@FiscalYearDetailId
,SalaryPFDetail.ProjectId
,sum(SalaryPFDetail.PFValue)PFValue
,sum(SalaryPFDetailEmployeer.PFValue)EmployeerPF
,@Post
,'-' Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@LastUpdateBy
,@LastUpdateAt
,@LastUpdateFrom


";
                sqlText += " FROM " + hrmDB + ".dbo.SalaryPFDetail";
                sqlText += " left outer join " + hrmDB + ".dbo.SalaryPFDetailEmployeer on " + hrmDB +
                           ".dbo.SalaryPFDetail.EmployeeId=" + hrmDB + ".dbo.SalaryPFDetailEmployeer.EmployeeId";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetail.FiscalYearDetailId=" + hrmDB +
                           ".dbo.SalaryPFDetailEmployeer.FiscalYearDetailId";
                sqlText += " WHERE 1=1 ";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetail.FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetail.ProjectId=@ProjectId";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetailEmployeer.FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetail.PFValue>0";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetail.EmployeeStatus not in('left')";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetailEmployeer.PFValue>0";

                sqlText += @"  AND " + hrmDB + @".dbo.SalaryPFDetail.EmployeeId not in(select distinct  EmployeeId 
                from  " + hrmDB + @".dbo.SalaryEmployee
                where FiscalYearDetailId=@FiscalYearDetailId 
                and ProjectId=@ProjectId
                and IsHold=1)";

                sqlText += @"

group by SalaryPFDetail.ProjectId
--,SalaryPFDetail.Remarks
--,SalaryPFDetail.IsActive
--,SalaryPFDetail.IsArchive
--,SalaryPFDetail.CreatedBy
--,SalaryPFDetail.CreatedAt
--,SalaryPFDetail.CreatedFrom
--,SalaryPFDetail.LastUpdateBy
--,SalaryPFDetail.LastUpdateAt
--,SalaryPFDetail.LastUpdateFrom

";

                string deleteCurrent =
                    @" DELETE FROM PFHeader  WHERE FiscalYearDetailId=@FiscalYearDetailId and ProjectId=@ProjectId
                        ";


                SqlCommand cmd = new SqlCommand(deleteCurrent, currConn, transaction);
                cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
                cmd.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmd.ExecuteNonQuery();

                int nextId = _cDal.NextId("PFHeader", currConn, transaction);

                string NewCode = new CommonDAL().CodeGenerationPF("PFContribution", "PFContribution", DateTime.Now.ToString(), currConn, transaction);

                string code = NewCode;

                //////string code = "PFC-" + nextId.ToString().PadLeft(4, '0');

                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@code", code);
                cmd.Parameters.AddWithValue("@Id", nextId);
                cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
                cmd.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmd.Parameters.AddWithValue("@Post", false);

                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@IsArchive", false);
                cmd.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedAt);
                cmd.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmd.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);
                cmd.Parameters.AddWithValue("@LastUpdateBy", "");
                cmd.Parameters.AddWithValue("@LastUpdateAt", "");
                cmd.Parameters.AddWithValue("@LastUpdateFrom", "");


                cmd.ExecuteNonQuery();


                ResultVM resultvm = InsertPFDetails(FiscalYearDetailId, ProjectId, hrmDB, currConn, transaction,nextId.ToString());


                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "PFDetail Saved Successfully.";
                }
                #endregion Commit

            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
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

        private ResultVM InsertPFDetails(string FiscalYearDetailId, string ProjectId, string hrmDB, SqlConnection currConn,
            SqlTransaction transaction, string headerId)
        {
            try
            {
                string sqlText;
                int transResult;

                #region Save Data

                #region SqlText

                sqlText = "";
                sqlText = @"
--declare @FiscalYearDetailId as varchar(20)
declare @FiscalYear as varchar(20)
declare @FiscalYearId as varchar(20)
--set @FiscalYearDetailId=9
select @FiscalYear=[year],@FiscalYearId=FiscalYearId from " + hrmDB +
                          ".dbo.FiscalYearDetail where Id=@FiscalYearDetailId";
                sqlText += @" SELECT @FiscalYear,@FiscalYearId";

                sqlText += @" DELETE FROM PFDetails  WHERE FiscalYearDetailId=@FiscalYearDetailId and ProjectId=@ProjectId
------------------------------------------------
------------------------------------------------
declare @maxId as int
set @maxId = 0 
select @maxId=isnull(max(Id),0) from PFDetails


DBCC CHECKIDENT ('PFDetails', RESEED, @maxId)
------------------------------------------------
------------------------------------------------
INSERT INTO PFDetails 
(
FiscalYearDetailId
,PFStructureId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,EmployeeId
,EmployeePFValue,EmployeerPFValue
,IsDistribute
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom
,BasicSalary,GrossSalary
,Post
,PFHeaderId
) 

select 
@FiscalYearDetailId
,SalaryPFDetail.PFStructureId
,SalaryPFDetail.ProjectId
,SalaryPFDetail.DepartmentId
,SalaryPFDetail.SectionId
,SalaryPFDetail.DesignationId
,SalaryPFDetail.EmployeeId
,SalaryPFDetail.PFValue,SalaryPFDetailEmployeer.PFValue
,0
,SalaryPFDetail.Remarks,SalaryPFDetail.IsActive
,SalaryPFDetail.IsArchive
,SalaryPFDetail.CreatedBy,SalaryPFDetail.CreatedAt
,SalaryPFDetail.CreatedFrom,SalaryPFDetail.LastUpdateBy,SalaryPFDetail.LastUpdateAt
,SalaryPFDetail.LastUpdateFrom
,SalaryPFDetail.BasicSalary
,SalaryPFDetail.GrossSalary
,@Post
,@PFHeaderId
";
                sqlText += " FROM " + hrmDB + ".dbo.SalaryPFDetail";
                sqlText += " left outer join " + hrmDB + ".dbo.SalaryPFDetailEmployeer on " + hrmDB +
                           ".dbo.SalaryPFDetail.EmployeeId=" + hrmDB + ".dbo.SalaryPFDetailEmployeer.EmployeeId";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetail.FiscalYearDetailId=" + hrmDB +
                           ".dbo.SalaryPFDetailEmployeer.FiscalYearDetailId";
                sqlText += " WHERE 1=1 ";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetail.FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetail.ProjectId=@ProjectId";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetailEmployeer.FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetail.PFValue>0";
                sqlText += " AND " + hrmDB + ".dbo.SalaryPFDetailEmployeer.PFValue>0";
                sqlText += @"  AND " + hrmDB + @".dbo.SalaryPFDetail.EmployeeId not in(select distinct  EmployeeId 
                from  " + hrmDB + @".dbo.SalaryEmployee
                where FiscalYearDetailId=@FiscalYearDetailId 
                and ProjectId=@ProjectId
                and IsHold=1)";
                #endregion SqlText

                #region SqlExecution

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdUpdate.Parameters.AddWithValue("@ProjectId", ProjectId);
                cmdUpdate.Parameters.AddWithValue("@Post", false);
                cmdUpdate.Parameters.AddWithValue("@PFHeaderId", headerId);
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                if (transResult <= 0)
                {
                    
                    throw new ArgumentNullException("No Data Found for Process", "");
                }

                #endregion SqlExecution

                #endregion Save Data


                ResultVM resultVm = new ResultVM();
                resultVm.Status = "Success";
                resultVm.Message = "Success";

                return resultVm;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal GetPFAmount(int FiscalYearDetailId, string EmployeeId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

select ISNULL(sum(PFValue),0) Amount from SalaryPFDetail 
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


        ////==================Post =================
        //public string[] Post(string[] ids, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        //{
        //    #region Variables
        //    string[] retResults = new string[6];
        //    retResults[0] = "Fail";//Success or Fail
        //    retResults[1] = "Fail";// Success or Fail Message
        //    retResults[2] = "0";// Return Id
        //    retResults[3] = "sqlText"; //  SQL Query
        //    retResults[4] = "ex"; //catch ex
        //    retResults[5] = "PostPFDetail"; //Method Name
        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
        //    #endregion
        //    try
        //    {
        //        #region open connection and transaction
        //        #region New open connection and transaction
        //        if (VcurrConn != null)
        //        {
        //            currConn = VcurrConn;
        //        }
        //        if (Vtransaction != null)
        //        {
        //            transaction = Vtransaction;
        //        }
        //        #endregion New open connection and transaction
        //        if (currConn == null)
        //        {
        //            currConn = _dbsqlConnection.GetConnectionPF();
        //            if (currConn.State != ConnectionState.Open)
        //            {
        //                currConn.Open();
        //            }
        //        }
        //        #endregion open connection and transaction
        //        if (ids.Length >= 1)
        //        {
        //            PFBankDepositDAL _PFBankDepositDAL = new PFBankDepositDAL();
        //            #region Update Settings
        //            for (int i = 0; i < ids.Length - 1; i++)
        //            {

        //                #region Insert Into PFBankDeposits
        //                //retResults = _PFBankDepositDAL.InsertFromPFDetail(ids[i], auditvm, currConn, transaction);
        //                //if (retResults[0] == "Fail")
        //                //{
        //                //    throw new ArgumentNullException("PFBankDeposits Insert", ids[i] + " could not Insert.");
        //                //}
        //                #endregion Insert Into PFBankDeposits

        //                retResults = _cDal.FieldPost("PFDetails", "FiscalYearDetailId", ids[i], currConn, transaction);
        //                if (retResults[0] == "Fail")
        //                {
        //                    throw new ArgumentNullException("PFDetails Post", ids[i] + " could not Post.");
        //                }
        //            }
        //            #endregion Update Settings
        //        }
        //        else
        //        {
        //            throw new ArgumentNullException("PFDetail Information Post - Could not found any item.", "");
        //        }
        //    }
        //    #region catch
        //    catch (Exception ex)
        //    {
        //        retResults[0] = "Fail";//Success or Fail
        //        retResults[4] = ex.Message; //catch ex
        //        return retResults;
        //    }
        //    finally
        //    {
        //        if (VcurrConn == null)
        //        {
        //            if (currConn != null)
        //            {
        //                if (currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();
        //                }
        //            }
        //        }
        //    }
        //    #endregion
        //    return retResults;
        //}


        public string[] Post(PFDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = " PFDetails Post"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("Post PFDetails"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update PFDetails set";
                    sqlText += "  Post=@Post";

                    sqlText += " where FiscalYearDetailId=@FiscalYearDetailId and ProjectId=@ProjectId";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Post", true);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                   
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EEHeadVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Post PFDetails", "Could not found any item.");
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
                    retResults[1] = "Data  Successfully Post.";
                }
                else
                {
                    retResults[1] = "Unexpected error to Post PFDetails.";
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
        public string[] PostHeader(PFHeaderVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = " PFDetails Post"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("Post PFDetails"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update PFHeader set";
                    sqlText += "  Post=@Post";

                    sqlText += @" where Id=@Id

update PFDetails set Post=@Post where PFHeaderId=@Id

";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Post", true);
                    
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EEHeadVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Post PFDetails", "Could not found any item.");
                }

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }
                retResults[0] = "Success";
                retResults[1] = "Data  Successfully Post.";
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

        ////==================Report=================
        public DataTable Report(PFDetailVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                currConn = _dbsqlConnection.GetConnectionPF();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                string hrmDB = _dbsqlConnection.HRMDB;
                #region sql statement
                sqlText = @"
SELECT  
pfd.Id
,pfd.EmployeeId
,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,pfd.EmployeePFValue
,pfd.EmployeerPFValue
,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd
,pfd.FiscalYearDetailId
FROM PFDetails pfd
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fyd ON pfd.FiscalYearDetailId=fyd.Id";
                sqlText += "  LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON pfd.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND pfd.IsArchive = 0
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

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

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
            return dt;
        }

        //==================SelectTotalContribution=================
        public PFDetailVM SelectTotalContribution_TillMonth(int FiscalYearDetailIdTo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            PFDetailVM vm = new PFDetailVM();
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
                    currConn = _dbsqlConnection.GetConnectionPF();
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
--------declare @FiscalYearDetailIdTo as int
--------
--------set @FiscalYearDetailIdTo = 1042

SELECT
SUM(pfd.EmployeePFValue) TotalEmployeeContribution
,SUM(pfd.EmployeerPFValue) TotalEmployerContribution

FROM PFDetails pfd
WHERE  1=1
AND pfd.Post = 1
--------AND pfd.IsDistribute = 0
AND pfd.FiscalYearDetailId <= @FiscalYearDetailIdTo

HAVING 1=1
AND sum(pfd.EmployeePFValue) > 0
AND sum(pfd.EmployeerPFValue) > 0
";

                #endregion SqlText
                #region SqlExecution
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                objComm.Parameters.AddWithValue("@FiscalYearDetailIdTo", FiscalYearDetailIdTo);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new PFDetailVM();
                    vm.TotalEmployeeContribution = Convert.ToDecimal(dr["TotalEmployeeContribution"]);
                    vm.TotalEmployerContribution = Convert.ToDecimal(dr["TotalEmployerContribution"]);
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
            return vm;
        }

        //==================SelectDetailContribution=================
        public List<PFDetailVM> SelectDetailContribution_TillMonth(int FiscalYearDetailIdTo, string EmployeeId="", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFDetailVM> VMs = new List<PFDetailVM>();
            PFDetailVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionPF();
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
                string hrmDB = _dbsqlConnection.HRMDB;

                #region sql statement
                #region SqlText
                sqlText = "";



                sqlText = @"
--------declare @FiscalYearDetailIdTo as int
--------declare @EmployeeId as nvarchar(100)
--------
--------set @FiscalYearDetailIdTo = 1042
--------set @EmployeeId = '1_1'

---------------------PF Summary Contribution--------------------
----------------------------------------------------------------
;WITH PFSummaryContribution AS
(
SELECT
 pfd.ProjectId
,pfd.DepartmentId
,pfd.SectionId
,pfd.DesignationId
,pfd.EmployeeId
,SUM(pfd.EmployeePFValue) EmployeeTotalContribution
,SUM(pfd.EmployeerPFValue) EmployerTotalContribution

FROM PFDetails pfd
WHERE  1=1
AND pfd.Post = 1
--------AND pfd.IsDistribute = 0
AND pfd.FiscalYearDetailId <= @FiscalYearDetailIdTo
AND pfd.EmployeeId=@EmployeeId

GROUP BY pfd.EmployeeId
,pfd.ProjectId
,pfd.DepartmentId
,pfd.SectionId
,pfd.DesignationId

HAVING 1=1
AND SUM(pfd.EmployeePFValue) > 0
AND SUM(pfd.EmployeerPFValue) > 0
)


---------------------PFStatus-----------------------------------
----------------------------------------------------------------
, PFStatus AS
(
SELECT 
ROW_NUMBER() OVER (PARTITION BY pf.EmployeeId ORDER BY pf.FiscalYearDetailId ASC) AS RowNumber
, pf.* 
FROM PFDetails pf
WHERE 1=1
AND pf.EmployeePFValue > 0
)


, PFStart AS
(
SELECT * FROM PFStatus
WHERE RowNumber = 1
)


---------------------PFEndStatus--------------------------------
----------------------------------------------------------------
, PFEndStatus AS
(
SELECT 

ROW_NUMBER() OVER (PARTITION BY pf.EmployeeId ORDER BY pf.FiscalYearDetailId DESC) AS RowNumber
, pf.* 

from PFDetails pf

WHERE 1=1

AND pf.EmployeePFValue > 0
)

, PFEnd AS
(
SELECT * FROM PFEndStatus
WHERE RowNumber = 1
)


----------------------------------------------------------------
----------------------------------------------------------------
SELECT 
 ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,ve.JoinDate
,ve.DateOfPermanent
,ve.LeftDate
, fyd.PeriodStart PFStartDate
, fyd.PeriodName

, fydEnd.PeriodEnd PFEndDate
, fydEnd.PeriodName PFEndPeriodName

,pfsc.ProjectId
,pfsc.DepartmentId
,pfsc.SectionId
,pfsc.DesignationId
,pfsc.EmployeeId
,pfsc.EmployeeTotalContribution
,pfsc.EmployerTotalContribution 


FROM PFSummaryContribution pfsc
LEFT OUTER JOIN PFStart pfs ON pfs.EmployeeId = pfsc.EmployeeId
LEFT OUTER JOIN PFEnd pfe ON pfe.EmployeeId = pfsc.EmployeeId
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON pfsc.EmployeeId=ve.EmployeeId";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].FiscalYearDetail fyd ON pfs.FiscalYearDetailId = fyd.Id";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].FiscalYearDetail fydEnd ON pfe.FiscalYearDetailId = fydEnd.Id";
                sqlText += " ORDER BY ve.Code";

                if (string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText = sqlText.Replace("pfd.EmployeeId=@EmployeeId","1=1");
                }
                #endregion SqlText
                #region SqlExecution
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                objComm.Parameters.AddWithValue("@FiscalYearDetailIdTo", FiscalYearDetailIdTo);
                //objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new PFDetailVM();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.DateOfPermanent = Ordinary.StringToDate(dr["DateOfPermanent"].ToString());
                    vm.LeftDate = Ordinary.StringToDate(dr["LeftDate"].ToString());
                    vm.PFStartDate = Ordinary.StringToDate(dr["PFStartDate"].ToString());
                    vm.PeriodName = Convert.ToString(dr["PeriodName"]);
                    
                    vm.PFEndDate = Ordinary.StringToDate(dr["PFEndDate"].ToString());
                    vm.PFEndPeriodName = Convert.ToString(dr["PFEndPeriodName"]);



                    vm.ProjectId = Convert.ToString(dr["ProjectId"]);
                    vm.DepartmentId = Convert.ToString(dr["DepartmentId"]);
                    vm.SectionId = Convert.ToString(dr["SectionId"]);
                    vm.DesignationId = Convert.ToString(dr["DesignationId"]);
                    vm.EmployeeId = Convert.ToString(dr["EmployeeId"]);
                    vm.EmployeeTotalContribution = Convert.ToDecimal(dr["EmployeeTotalContribution"]);
                    vm.EmployerTotalContribution = Convert.ToDecimal(dr["EmployerTotalContribution"]);

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


        public DataTable PFReportSummaryDetail(string fydid, string rType,string ProjectId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                FiscalYearDetailVM dVM = new FiscalYearDetailVM();
                dVM = new FiscalYearDAL().SelectAll_FiscalYearDetail(Convert.ToInt32(fydid)).FirstOrDefault();
                string PeriodEnd = dVM.PeriodEnd;



                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionPF();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                string hrmDB = _dbsqlConnection.HRMDB;

                #region sql statement
                if (rType == "Summary")
                {
                    sqlText = @" 
----select @TransactionDate TransactionDate, 'Contribution' Particular,'Cont-001' AccountCode, sum(EmployeePFValue+EmployeerPFValue) DebitAmount, 
----0 CreditAmount, '' Remarks 
----from PFDetails pfd
----where pfd.FiscalYearDetailId = @FiscalYearDetailId
----And pfd.ProjectId = @ProjectId
----
----
----union all
----select @TransactionDate, 'PF Account', 'PF-001', 0, sum(EmployeePFValue+EmployeerPFValue), ''  from PFDetails
----where FiscalYearDetailId = @FiscalYearDetailId
----And ProjectId = @ProjectId

select distinct f.PeriodName, p.Name ProjectName, sum( d.EmployeePFValue) MemberContribution,sum(d.EmployeerPFValue) CompanyContribution ,sum( d.EmployeePFValue)+sum(d.EmployeerPFValue) Total from PFDetails d
left outer join " + hrmDB + @".dbo.Project p on d.ProjectId=p.Id
left outer join " + hrmDB + @".dbo.FiscalYearDetail f on d.FiscalYearDetailId=f.Id
where FiscalYearDetailId = @FiscalYearDetailId
And ProjectId = @ProjectId
group by  f.PeriodName, p.Name

";
                }
                else
                {
                    sqlText = @" 

select @TransactionDate TransactionDate, ve.EmpName Particular, ve.Code AccountCode, (EmployeePFValue+EmployeerPFValue) DebitAmount, 0 CreditAmount, '' Remarks 
from PFDetails pfd
left outer join HRMDB.dbo.ViewEmployeeInformation ve on pfd.EmployeeId = ve.EmployeeId

where pfd.FiscalYearDetailId = @FiscalYearDetailId
And pfd.ProjectId = @ProjectId

union all
select @TransactionDate, 'PF Account', 'PF-001', 0, sum(EmployeePFValue+EmployeerPFValue), ''  
from PFDetails
where FiscalYearDetailId = @FiscalYearDetailId
And ProjectId = @ProjectId

";

                }

                sqlText = sqlText.Replace("HRMDB", hrmDB);
                
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@FiscalYearDetailId", fydid);
                objComm.Parameters.AddWithValue("@TransactionDate", PeriodEnd);
                objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                


                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.SelectCommand.Transaction = transaction;

               
                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDate");


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
            return dt;


        }

        public DataTable PFEmployersProvisionsReport(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion

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
                    currConn = _dbsqlConnection.GetConnectionPF();
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
select 
ts.PFHeaderId
,PeriodName
,Project
,Section
,SectionOrderNo
,Designation
,DesignationOrderNo
,ts.BasicAmount
,ts.EmployeePFValue
,ts.EmployeerPFValue
,ts.TotalAmount
  from (
select distinct ts.PFHeaderId
,fs.PeriodName
,p.Name Project
,st.Name Section
,st.OrderNo SectionOrderNo
,dgg.Name Designation
,dgg.Serial DesignationOrderNo
,count(ts.employeeid)TotalEmployee
,sum(isnull(ts.BasicSalary,0))BasicAmount
,sum(isnull(ts.EmployeePFValue,0))EmployeePFValue
,sum(isnull(ts.EmployeerPFValue,0))EmployeerPFValue
,sum(isnull(ts.EmployeePFValue,0)+isnull(ts.EmployeerPFValue,0))TotalAmount
,0 EmployeeContribution
,0 EmployerContribution
from PFDetails ts 

";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[Section] st on ts.SectionId = st.Id";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[Project] p on ts.ProjectId = p.Id ";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fs on ts.FiscalYearDetailId = fs.Id";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[Designation] dg on ts.DesignationId = dg.Id";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[DesignationGroup] dgg on dg.DesignationGroupId = dgg.Id";
                sqlText += @" WHERE  1=1 
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

                sqlText += @" 
group by  ts.PFHeaderId
,fs.PeriodName  ,p.Name  ,st.Name  ,st.OrderNo  ,dgg.Name  ,dgg.Serial  
) as ts
order by SectionOrderNo,DesignationOrderNo ";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
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

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }

            #region catch

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

            return dt;
        }



        #endregion Methods

        
    }
}
