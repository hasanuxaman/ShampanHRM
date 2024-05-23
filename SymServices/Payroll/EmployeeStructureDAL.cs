using Excel;
using SymOrdinary;
using SymServices.HRM;
using SymViewModel.HRM;
using SymViewModel.Loan;
using SymViewModel.Payroll;
using SymViewModel.PF;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Payroll
{
    public class EmployeeStructureDAL
    {
        #region Declare
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion Declare
        #region Methods
        //==================SelectAll=================
        public List<EmployeeSalaryStructureVM> SelectAll(string Id = "", string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeSalaryStructureVM> VMs = new List<EmployeeSalaryStructureVM>();
            EmployeeSalaryStructureVM vm;
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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT
Id
,EmployeeId
,SalaryStructureId
,1000 TotalValue
,ISNULL(NULLIF(IncrementDate,''),19000101)IncrementDate
,BranchId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

from EmployeeSalaryStructure
WHERE  1=1 AND IsArchive = 0
";

                if (!string.IsNullOrWhiteSpace(Id))
                {
                    sqlText += @" and Id=@Id";
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
                //EmployeeId
                //SalaryStructureId
                //TotalValue
                //IncrementDate
                //BranchId
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeSalaryStructureVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.SalaryStructureId = dr["SalaryStructureId"].ToString();
                    vm.TotalValue = Convert.ToDecimal(dr["TotalValue"]);
                    vm.IncrementDate = Ordinary.StringToDate(dr["IncrementDate"].ToString());
                    vm.BranchId = dr["BranchId"].ToString();
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

        public List<EmployeeSalaryStructureVM> SelectAllStructure(string Id = "", string[] conditionFields = null, string[] conditionValues = null
    , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeSalaryStructureVM> VMs = new List<EmployeeSalaryStructureVM>();
            EmployeeSalaryStructureVM vm;
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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
select distinct EmployeeSalaryStructureId Id
,d.EmployeeId, h.SalaryStructureId, ISNULL(NULLIF(d.IncrementDate,''),19000101)IncrementDate, Sum(Amount)TotalValue
,''Remarks
from EmployeeSalaryStructureDetail  d
left outer join EmployeeSalaryStructure h on d.EmployeeSalaryStructureId=h.id
WHERE  1=1  and PortionSalaryType!='Gross'

";

                if (!string.IsNullOrWhiteSpace(Id))
                {
                    sqlText += @" and Id=@Id";
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
                sqlText += @" group by  EmployeeSalaryStructureId  
,d.EmployeeId, h.SalaryStructureId, ISNULL(NULLIF(d.IncrementDate,''),19000101)  
 having Sum(Amount)>0
";

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
                //EmployeeId
                //SalaryStructureId
                //TotalValue
                //IncrementDate
                //BranchId
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeSalaryStructureVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.SalaryStructureId = dr["SalaryStructureId"].ToString();
                    vm.TotalValue = Convert.ToDecimal(dr["TotalValue"]);
                    vm.IncrementDate = Ordinary.StringToDate(dr["IncrementDate"].ToString());
                    VMs.Add(vm);
                }
                dr.Close();
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


        public List<EmployeeSalaryStructureVM> SelectEmployeeSalaryStructureDetailAll(string EmployeeId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables

            string sqlText = "";
            List<EmployeeSalaryStructureVM> VMs = new List<EmployeeSalaryStructureVM>();
            EmployeeSalaryStructureVM vm;
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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region sql statement

                sqlText = @"
SELECT  distinct esd.SalaryTypeId, esd.SalaryType, esd.EmployeeId, esd.Amount
,1 IsFixed ---- es.IsFixed

, es.IsEarning
-----,es.Portion
,ISNULL(es.PortionSalaryType,'') PortionSalaryType
,stype.Name SalaryTypeName 
,stype.sl
,ve.StepSL
,ve.GradeId
From

(
SELECT
SalaryTypeId
,SalaryType
,EmployeeId
,sum(Amount) Amount
from EmployeeSalaryStructureDetail
Where EmployeeId=@EmployeeId
and IsActive=1
group by SalaryTypeId, SalaryType, EmployeeId
) as esd	
									 
LEFT OUTER JOIN EmployeeSalaryStructureDetail es ON esd.EmployeeId = es.EmployeeId
and esd.SalaryTypeId = es.SalaryTypeId
left outer join [dbo].[EnumSalaryType] stype on es.SalaryTypeId=stype.Id
left outer join ViewEmployeeInformation ve on ve.EmployeeId = esd.EmployeeId
";
                sqlText += @" order by stype.sl";

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeSalaryStructureVM();
                        //vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.SalaryTypeId = dr["SalaryTypeId"].ToString();
                        vm.SalaryType = dr["SalaryType"].ToString();
                        vm.SalaryTypeName = dr["SalaryTypeName"].ToString();
                        vm.PortionSalaryType = dr["PortionSalaryType"].ToString();
                        //////vm.Portion = dr["Portion"].ToString();
                        vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                        vm.TotalValue = Convert.ToDecimal(dr["Amount"]);
                        vm.IsEarning = Convert.ToBoolean(dr["IsEarning"]);

                        vm.StepSL = Convert.ToString(dr["StepSL"]);
                        vm.GradeId = Convert.ToString(dr["GradeId"]);

                        VMs.Add(vm);
                    }
                    dr.Close();
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
                #endregion Commit
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

            return VMs;
        }



        public List<EmployeeSalaryStructureVM> SelectEmployeeSalaryStructureAll(string Id, string EmployeeSalaryStructureId = "")
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeSalaryStructureVM> VMs = new List<EmployeeSalaryStructureVM>();
            EmployeeSalaryStructureVM vm;
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
es.Id
,SalaryTypeId
,SalaryType
,EmployeeId
,IsFixed
,Portion
,PortionSalaryType
,Amount
,EmployeeSalaryStructureId
,es.Remarks
,es.IsEarning
,stype.Name SalaryTypeName 
 from EmployeeSalaryStructureDetail es
left outer join [dbo].[EnumSalaryType] stype on es.SalaryTypeId=stype.Id
 Where es.EmployeeId=@Id
and es.IsActive=1
and es.SalaryType!='Gross'

";
                if (!string.IsNullOrWhiteSpace(EmployeeSalaryStructureId))
                {
                    sqlText += " and EmployeeSalaryStructureId=@EmployeeSalaryStructureId";
                }
                sqlText += @" order by stype.sl";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                if (!string.IsNullOrWhiteSpace(EmployeeSalaryStructureId))
                {
                    objComm.Parameters.AddWithValue("@EmployeeSalaryStructureId", EmployeeSalaryStructureId);
                }

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeSalaryStructureVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.SalaryTypeId = dr["SalaryTypeId"].ToString();
                        vm.SalaryType = dr["SalaryType"].ToString();
                        vm.SalaryTypeName = dr["SalaryTypeName"].ToString();
                        vm.PortionSalaryType = dr["PortionSalaryType"].ToString();
                        vm.Portion = Convert.ToDecimal(dr["Portion"]);
                        vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                        vm.TotalValue = Convert.ToDecimal(dr["Amount"]);
                        vm.IsEarning = Convert.ToBoolean(dr["IsEarning"]);
                        vm.Remarks = dr["Remarks"].ToString();
                        //EmployeeSalaryStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);

                        VMs.Add(vm);
                    }
                    dr.Close();
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

        public EmployeeSalaryStructureDetailVM SelectEmployeeSalaryStructureDetail(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeSalaryStructureDetailVM vm = new EmployeeSalaryStructureDetailVM();
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
                        Id, SalaryTypeId, SalaryType, EmployeeId, IsFixed, Portion, PortionSalaryType,
                        Amount, EmployeeSalaryStructureId, Remarks, IsActive, IsArchive ,
                        IsEarning
                        FROM            EmployeeSalaryStructureDetail
                         Where Id=@Id
                        ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeSalaryStructureDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.SalaryTypeId = dr["SalaryTypeId"].ToString();
                        vm.SalaryType = dr["SalaryType"].ToString();
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.EmployeeSalaryStructureId = dr["EmployeeSalaryStructureId"].ToString();

                        vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                        vm.IsEarning = Convert.ToBoolean(dr["IsEarning"]);
                        vm.Portion = Convert.ToDecimal(dr["Portion"]);
                        vm.Remarks = dr["Remarks"].ToString();
                    }
                    dr.Close();
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

            return vm;
        }

        public List<EmployeeLoanVM> SelectEmployeeLoanStructureAll(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLoanVM> VMs = new List<EmployeeLoanVM>();
            EmployeeLoanVM vm;
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
 l.Id
,l.BranchId
,l.LoanType_E
,ltype.Name LoanType
,l.EmployeeId
,l.PrincipalAmount
,l.IsFixed
,l.InterestRate
,l.InterestAmount
,l.TotalAmount
,l.NumberOfInstallment
,l.ApprovedDate
,l.StartDate
,l.EndDate
,l.IsHold
,l.Remarks
,e.Salutation_E
,e.MiddleName
,e.LastName
 from EmployeeLoan l
left outer join EmployeeInfo e on l.EmployeeId=e.Id
left outer join  EnumLoanType ltype on ltype.Id=l.LoanType_E
WHERE l.IsArchive=0  and l.EmployeeId=@Id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeLoanVM();
                        vm.Id = dr["Id"].ToString();
                        vm.LoanType_E = dr["LoanType_E"].ToString();
                        vm.LoanType = dr["LoanType"].ToString();
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.PrincipalAmount = Convert.ToDecimal(dr["PrincipalAmount"].ToString());
                        vm.InterestRate = Convert.ToDecimal(dr["InterestRate"].ToString());
                        vm.InterestAmount = Convert.ToDecimal(dr["InterestAmount"].ToString());
                        vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                        vm.NumberOfInstallment = Convert.ToInt32(dr["NumberOfInstallment"].ToString());
                        vm.ApprovedDate = Ordinary.StringToDate(dr["ApprovedDate"].ToString());
                        vm.StartDate = Ordinary.StringToDate(dr["StartDate"].ToString());
                        vm.EndDate = Ordinary.StringToDate(dr["EndDate"].ToString());
                        vm.IsHold = Convert.ToBoolean(dr["IsHold"]);
                        vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);

                        vm.Remarks = dr["Remarks"].ToString();
                        //EmployeeSalaryStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);

                        VMs.Add(vm);
                    }
                    dr.Close();
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
        public List<EmployeePFVM> SelectEmployeePFtructureAll(string Id)
        {

            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePFVM> VMs = new List<EmployeePFVM>();
            EmployeePFVM vm;
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

                sqlText = @"select 
emppf.Id
,emppf.EmployeeId
,emppf.PFValue,
emppf.IsFixed,
emppf.PortionSalaryType
,pfs.Name PFName
from EmployeePF emppf
left outer join PFStructure pfs on emppf.PFStructureId=pfs.Id
 
 where emppf.IsArchive=0 and emppf.EmployeeId=@Id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeePFVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.PFValue = Convert.ToDecimal(dr["PFValue"].ToString());
                        vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);

                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.PortionSalaryType = dr["PortionSalaryType"].ToString();
                        vm.PFName = dr["PFName"].ToString();
                        //EmployeeSalaryStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        VMs.Add(vm);
                    }
                    dr.Close();
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
        public List<EmployeeTaxVM> SelectEmployeeTAXtructureAll(string Id)
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

                sqlText = @"select
empt.Id
,empt.EmployeeId
,txs.Name TaxName
,empt.TaxValue 
,empt.PortionSalaryType
,empt.IsFixed
from EmployeeTax empt
left outer join TaxStructure txs on empt.TaxStructureId=txs.Id
where empt.IsArchive='0' and empt.EmployeeId=@Id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeTaxVM();
                        vm.Id = dr["Id"].ToString();
                        vm.TaxName = dr["TaxName"].ToString();

                        vm.TaxValue = Convert.ToDecimal(dr["TaxValue"].ToString());
                        vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);

                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.PortionSalaryType = dr["PortionSalaryType"].ToString();
                        //EmployeeSalaryStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        VMs.Add(vm);
                    }
                    dr.Close();
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
        public List<EmployeeBonusVM> SelectEmployeeBonustructureAll(string Id)
        {

            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeBonusVM> VMs = new List<EmployeeBonusVM>();
            EmployeeBonusVM vm;
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

                sqlText = @"select
empb.id
,bostruct.Name BonusName
,bostruct.BonusValue
,bostruct.PortionSalaryType
,bostruct.IsFixed
,bostruct.DOJFrom
,bostruct.DOJTo
 from EmployeeBonusDetail empd 
left outer join  EmployeeBonus empb on empd.EmployeeBonusId=empb.Id
left outer join  BonusStructure bostruct on empb.BonusStructureId=bostruct.Id
where empb.IsArchive='0' and empd.EmployeeId=@Id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeBonusVM();
                        vm.Id = dr["Id"].ToString();
                        vm.BonusStructureName = dr["BonusName"].ToString();

                        vm.BonusValue = Convert.ToDecimal(dr["BonusValue"].ToString());
                        vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);


                        vm.PortionSalaryType = dr["PortionSalaryType"].ToString();
                        //EmployeeSalaryStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        VMs.Add(vm);
                    }
                    dr.Close();
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
        public string[] Insert(SalaryStructureVM salaryStructureVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertSalaryStructure"; //Method Name


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

                #region Save
                #region Check Name and Create Id

                sqlText = "  ";
                sqlText += " SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) Id FROM SalaryStructure ";
                sqlText += " WHERE Code=@Code And Name=@Name";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Code", salaryStructureVM.Code);
                cmdExist.Parameters.AddWithValue("@Name", salaryStructureVM.Name);
                var exeRes = cmdExist.ExecuteScalar();
                int exists = Convert.ToInt32(exeRes);
                if (exists > 0)
                {
                    retResults[1] = "This Salary Structure already used";
                    throw new ArgumentNullException("This Salary Structure already used", "");
                }

                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from SalaryStructure where BranchId=@BranchId";
                SqlCommand cmdB = new SqlCommand(sqlText, currConn);
                cmdB.Parameters.AddWithValue("@BranchId", salaryStructureVM.BranchId);
                cmdB.Transaction = transaction;
                exeRes = cmdB.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);
                #endregion Check Name and Create Id

                salaryStructureVM.Id = salaryStructureVM.BranchId.ToString() + "_" + (count + 1);

                #region Save Header

                sqlText = "  ";
                sqlText += @" INSERT INTO SalaryStructure(
                                    Id,Code,Name,BranchId,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                    )  VALUES (
                                     @Id,@Code,@Name,@BranchId,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                    ) ";

                SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);

                cmdExist1.Parameters.AddWithValue("@Id", salaryStructureVM.Id);

                cmdExist1.Parameters.AddWithValue("@Code", salaryStructureVM.Code);
                cmdExist1.Parameters.AddWithValue("@Name", salaryStructureVM.Name);

                cmdExist1.Parameters.AddWithValue("@BranchId", salaryStructureVM.BranchId);
                cmdExist1.Parameters.AddWithValue("@Remarks", salaryStructureVM.Remarks ?? Convert.DBNull);
                cmdExist1.Parameters.AddWithValue("@IsActive", true);
                cmdExist1.Parameters.AddWithValue("@IsArchive", false);
                cmdExist1.Parameters.AddWithValue("@CreatedBy", salaryStructureVM.CreatedBy);
                cmdExist1.Parameters.AddWithValue("@CreatedAt", salaryStructureVM.CreatedAt);
                cmdExist1.Parameters.AddWithValue("@CreatedFrom", salaryStructureVM.CreatedFrom);

                cmdExist1.Transaction = transaction;
                cmdExist1.ExecuteNonQuery();
                #endregion Save Header

                sqlText = "  ";
                sqlText += @" INSERT INTO SalaryStructureDetail(
                                IsFixed,Portion,PortionSalaryType,SalaryType,IsEarning,SalaryTypeId
                                ,SalaryStructureId,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                )  VALUES (
                                @IsFixed,@Portion,@PortionSalaryType,@SalaryType,@IsEarning,@SalaryTypeId
                                ,@SalaryStructureId,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                ) ";


                #region Earning

                if (salaryStructureVM.salaryStructureDetailVMs.FirstOrDefault().SalaryTypeId != null)
                {
                    #region
                    #region Duplicate
                    var duplicateKeys = salaryStructureVM.salaryStructureDetailVMs.GroupBy(x => x.SalaryTypeId)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key);

                    if (duplicateKeys.Any())
                    {
                        retResults[1] = "Duplicate salary head Already in Details ";
                        throw new ArgumentNullException("Duplicate salary head Already in Details ", "");
                    }
                    #endregion Duplicate

                    foreach (SalaryStructureDetailVM item in salaryStructureVM.salaryStructureDetailVMs)
                    {
                        if (string.IsNullOrWhiteSpace(item.SalaryTypeId))
                        {
                            retResults[1] = "Please Select Salary Deduction Head Properly";
                            throw new ArgumentNullException("Please Select Salary Deduction Head Properly", "");
                        }

                        if (item.IsFixed == false && item.Portion <= 0)
                        {
                            retResults[1] = " Salary Portion must Geter then Zero (0)";
                            throw new ArgumentNullException("Salary Salary Portion must Geter then Zero (0)", "");
                        }


                        SqlCommand cmdD = new SqlCommand(sqlText, currConn);

                        cmdD.Parameters.AddWithValue("@IsFixed", item.IsFixed);
                        cmdD.Parameters.AddWithValue("@Portion", item.Portion);
                        if (item.IsFixed == false)
                        {
                            cmdD.Parameters.AddWithValue("@PortionSalaryType", "Basic");
                        }
                        else
                        {
                            cmdD.Parameters.AddWithValue("@PortionSalaryType", Convert.DBNull);
                        }
                        cmdD.Parameters.AddWithValue("@SalaryTypeId", item.SalaryTypeId ?? Convert.DBNull);
                        cmdD.Parameters.AddWithValue("@SalaryStructureId", salaryStructureVM.Id);
                        cmdD.Parameters.AddWithValue("@SalaryType", "Other");
                        cmdD.Parameters.AddWithValue("@IsEarning", true);

                        cmdD.Parameters.AddWithValue("@Remarks", salaryStructureVM.Remarks ?? Convert.DBNull);
                        cmdD.Parameters.AddWithValue("@IsActive", true);
                        cmdD.Parameters.AddWithValue("@IsArchive", false);
                        cmdD.Parameters.AddWithValue("@CreatedBy", salaryStructureVM.CreatedBy);
                        cmdD.Parameters.AddWithValue("@CreatedAt", salaryStructureVM.CreatedAt);
                        cmdD.Parameters.AddWithValue("@CreatedFrom", salaryStructureVM.CreatedFrom);

                        cmdD.Transaction = transaction;
                        cmdD.ExecuteNonQuery();
                    }
                    #endregion
                }
                #endregion Earning

                #region Deduction
                if (salaryStructureVM.salaryStructureDeductionDetailVMs.FirstOrDefault().SalaryTypeId != null)
                {
                    #region
                    #region Duplicate
                    var duplicateKeys = salaryStructureVM.salaryStructureDeductionDetailVMs.GroupBy(x => x.SalaryTypeId)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key);

                    if (duplicateKeys.Any())
                    {
                        retResults[1] = "Duplicate salary head Already in Details ";
                        throw new ArgumentNullException("Duplicate salary head Already in Details ", "");
                    }
                    #endregion Duplicate

                    foreach (SalaryStructureDetailVM item in salaryStructureVM.salaryStructureDeductionDetailVMs)
                    {
                        if (string.IsNullOrWhiteSpace(item.SalaryTypeId))
                        {
                            retResults[1] = "Please Select Salary Deduction Head Properly";
                            throw new ArgumentNullException("Please Select Salary Deduction Head Properly", "");
                        }
                        if (item.IsFixed == false && item.Portion <= 0)
                        {
                            retResults[1] = "Salary Portion must Geter then Zero (0)";
                            throw new ArgumentNullException("Salary Salary Portion must Geter then Zero (0)", "");
                        }

                        SqlCommand cmdD = new SqlCommand(sqlText, currConn);

                        cmdD.Parameters.AddWithValue("@IsFixed", item.IsFixed);
                        cmdD.Parameters.AddWithValue("@Portion", item.Portion);
                        if (item.IsFixed == false)
                        {
                            cmdD.Parameters.AddWithValue("@PortionSalaryType", "Basic");
                        }
                        else
                        {
                            cmdD.Parameters.AddWithValue("@PortionSalaryType", Convert.DBNull);
                        }
                        cmdD.Parameters.AddWithValue("@SalaryTypeId", item.SalaryTypeId);

                        cmdD.Parameters.AddWithValue("@SalaryStructureId", salaryStructureVM.Id);
                        cmdD.Parameters.AddWithValue("@SalaryType", "Other");
                        cmdD.Parameters.AddWithValue("@IsEarning", false);

                        cmdD.Parameters.AddWithValue("@Remarks", salaryStructureVM.Remarks ?? Convert.DBNull);
                        cmdD.Parameters.AddWithValue("@IsActive", true);
                        cmdD.Parameters.AddWithValue("@IsArchive", false);
                        cmdD.Parameters.AddWithValue("@CreatedBy", salaryStructureVM.CreatedBy);
                        cmdD.Parameters.AddWithValue("@CreatedAt", salaryStructureVM.CreatedAt);
                        cmdD.Parameters.AddWithValue("@CreatedFrom", salaryStructureVM.CreatedFrom);

                        cmdD.Transaction = transaction;
                        cmdD.ExecuteNonQuery();
                    }
                    #endregion
                }
                #endregion Deduction
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
                retResults[2] = Id.ToString();
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


        public string[] InsertDetailNew(EmployeeSalaryStructureDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertDetailNew"; //Method Name

            int transResult = 0;

            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
            decimal basic = 0;
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
                if (transaction == null) transaction = currConn.BeginTransaction("");

                #endregion open connection and transaction

                sqlText = "Select Top 1 * from EmployeeSalaryStructureDetail";
                sqlText += " WHERE EmployeeId=@EmployeeId   ";
                sqlText += " and  SalaryType='basic' and IsCurrent = 1 ";
                SqlCommand cmdsEarning = new SqlCommand(sqlText, currConn, transaction);
                cmdsEarning.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                using (SqlDataReader dr = cmdsEarning.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        basic = Convert.ToDecimal(dr["Amount"]);
                        vm.EmployeeSalaryStructureId = dr["EmployeeSalaryStructureId"].ToString();
                    }
                    dr.Close();
                }

                #region Exist
                sqlText = "  ";
                sqlText += @" 
SELECT isnull(Id,0) Id
FROM EmployeeSalaryStructureDetail
WHERE SalaryTypeId=@SalaryTypeId 
and  EmployeeId=@EmployeeId and IsCurrent = 1
";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@SalaryTypeId", vm.SalaryTypeId);
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                var exeRes = cmdExist.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    retResults[1] = "SalaryType already used!";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Code already used!", "");
                }

                #endregion Exist

                if (vm != null)
                {
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from EmployeeSalaryStructureDetail ";
                    SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                    cmd2.Transaction = transaction;
                    exeRes = cmd2.ExecuteScalar();
                    int count = Convert.ToInt32(exeRes);

                    vm.Id = (count + 1);

                    #region Insert Settings
                    #region SQL
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeSalaryStructureDetail(
 Id
,SalaryTypeId
,SalaryType
,EmployeeId
,IsFixed
,Portion
,PortionSalaryType
,Amount
,EmployeeSalaryStructureId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,IsEarning                  
) 
VALUES ( 
@Id
,@SalaryTypeId
,@SalaryType
,@EmployeeId
,@IsFixed
,@Portion
,@PortionSalaryType
,@Amount
,@EmployeeSalaryStructureId
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@IsEarning) 
                                        ";
                    #endregion SQL


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@SalaryTypeId", vm.SalaryTypeId);
                    cmdInsert.Parameters.AddWithValue("@SalaryType", "Other");
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@IsFixed", vm.IsFixed);

                    cmdInsert.Parameters.AddWithValue("@Portion", vm.Portion);
                    if (vm.IsFixed)
                    {
                        vm.Amount = vm.Portion;
                        vm.PortionSalaryType = "";

                    }
                    else
                    {

                        vm.Amount = basic * vm.Portion / 100;
                        vm.PortionSalaryType = "Basic";

                    }
                    cmdInsert.Parameters.AddWithValue("@PortionSalaryType", vm.PortionSalaryType);
                    cmdInsert.Parameters.AddWithValue("@Amount", vm.Amount);
                    cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", vm.EmployeeSalaryStructureId);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@IsEarning", vm.IsEarning);

                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();


                    sqlText = "";
                    sqlText = @" 
update EmployeeSalaryStructureDetail set Amount=earn.amount
from(
select sum(amount)amount from EmployeeSalaryStructureDetail
where 1=1 
and IsEarning=1 and SalaryType not in('gross') and EmployeeId=@EmployeeId 
AND IsCurrent=1
) earn
where EmployeeSalaryStructureDetail.EmployeeId=@EmployeeId
AND SalaryType in('gross')
AND IsCurrent=1


----EmployeeJob GrossSalary
update employeejob set GrossSalary=earn.amount
from(
select sum(amount)amount from EmployeeSalaryStructureDetail
where 1=1 
and IsEarning=1 and SalaryType not in('gross') and EmployeeId=@EmployeeId ) earn
where employeejob.EmployeeId=@EmployeeId

----EmployeeJob BasicSalary
UPDATE EmployeeJob set BasicSalary=earn.amount
from(
select sum(amount)amount from EmployeeSalaryStructureDetail
where 1=1 
and IsEarning=1 and SalaryType in('basic') and EmployeeId=@EmployeeId ) earn
where employeejob.EmployeeId=@EmployeeId

 ";

                    SqlCommand cmdUpdateEarn = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdateEarn.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    exeRes = cmdUpdateEarn.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    #endregion Update Settings


                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Group Update", "Could not found any item.");
                }

                #region Commit
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
                    retResults[1] = "Unexpected error to update Group.";
                    throw new ArgumentNullException("", "");
                }
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


        public string[] UpdateDetailNew(EmployeeSalaryStructureDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee [Group] Update"; //Method Name
            decimal basic = 0;

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
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                transaction = currConn.BeginTransaction("");

                #endregion open connection and transaction
                ////If SalaryStructureId is FromGross Then Basic Check is not necessaary!
                #region GrossBasic Check
                bool IsGross = false;
//                sqlText = @"Select IsGross
//From
//EmployeeStructureGroup
//WHERE 1=1 
//";
//                sqlText += " AND EmployeeId=@EmployeeId   ";
//                SqlCommand cmdIsGross = new SqlCommand(sqlText, currConn, transaction);
//                cmdIsGross.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
//                using (SqlDataReader dr = cmdIsGross.ExecuteReader())
//                {
//                    while (dr.Read())
//                    {
//                        IsGross = Convert.ToBoolean(dr["IsGross"]);
//                    }
//                    dr.Close();
//                }

                //if (!IsGross)
                //{
                //    if (vm.SalaryType.ToLower() == "basic")
                //    {
                //        retResults[1] = "Basic not Changeable !";
                //        retResults[3] = sqlText;
                //        throw new ArgumentNullException("Basic not Changeable !", "");
                //    }
                //}
                #endregion GrossBasic Check

                #region LastIncrement
                sqlText = "  ";
                sqlText += @" 
select IsCurrent from EmployeeSalaryStructureDetail
where 1=1 
AND EmployeeId=@EmployeeId AND Id=@Id
order by id desc";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn, transaction);
                cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                var exeRes = cmdExist.ExecuteScalar();
                bool isCurrent = Convert.ToBoolean(exeRes);

                if (isCurrent == false)
                {
                    retResults[1] = "Previous Increment is not Editable!";
                    retResults[3] = sqlText;
                    //throw new ArgumentNullException(retResults[1], "");
                }

                #endregion LastIncrement

                #region Exist
                sqlText = "  ";
                sqlText += @" 
SELECT isnull(Id,0) Id
FROM EmployeeSalaryStructureDetail
WHERE SalaryTypeId=@SalaryTypeId 
and  EmployeeId=@EmployeeId and IsCurrent = 1 AND Id<>@Id
";
                cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@SalaryTypeId", vm.SalaryTypeId);
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@Id", vm.Id);

                exeRes = cmdExist.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    retResults[1] = "SalaryType already used!";
                    retResults[3] = sqlText;
                    //throw new ArgumentNullException("Code already used!", "");
                }

                #endregion Exist



                if (vm != null)
                {

                    //sqlText = "Select * from EmployeeSalaryStructureDetail";
                    //sqlText += " WHERE EmployeeId=@EmployeeId   ";
                    //sqlText += " and  SalaryType='basic' and IsCurrent = 1";
                    //SqlCommand cmdsEarning = new SqlCommand(sqlText, currConn, transaction);
                    //cmdsEarning.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    //using (SqlDataReader dr = cmdsEarning.ExecuteReader())
                    //{
                    //    while (dr.Read())
                    //    {
                    //        basic = Convert.ToDecimal(dr["Amount"]);
                    //        vm.EmployeeSalaryStructureId = dr["EmployeeSalaryStructureId"].ToString();
                    //    }
                    //    dr.Close();
                    //}

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeSalaryStructureDetail set";
                    sqlText += " SalaryTypeId=@SalaryTypeId,";
                    sqlText += " Portion=@Portion,";
                    sqlText += " IsFixed=@IsFixed,";
                    sqlText += " PortionSalaryType=@PortionSalaryType,";
                    sqlText += " Amount=@Amount,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@SalaryTypeId", vm.SalaryTypeId);
                    cmdUpdate.Parameters.AddWithValue("@IsFixed", vm.IsFixed);
                    cmdUpdate.Parameters.AddWithValue("@Portion", vm.Portion);

                    if (vm.IsFixed)
                    {
                        vm.Amount = vm.Portion;
                        vm.PortionSalaryType = "";
                    }
                    else
                    {
                        vm.Amount = basic * vm.Portion / 100;
                        vm.PortionSalaryType = "Basic";
                    }
                    cmdUpdate.Parameters.AddWithValue("@Amount", vm.Amount);
                    cmdUpdate.Parameters.AddWithValue("@PortionSalaryType", vm.PortionSalaryType);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    sqlText = "";
                    sqlText = @" 
update EmployeeSalaryStructureDetail set Amount=earn.amount
from(
select sum(amount)amount from EmployeeSalaryStructureDetail
where 1=1 
and IsEarning=1 and SalaryType not in('gross','other') and EmployeeId=@EmployeeId 
AND IsCurrent=1
) earn
where EmployeeSalaryStructureDetail.EmployeeId=@EmployeeId
AND SalaryType in('gross')
AND IsCurrent=1

--------------------------------------------------------
----------------EmployeeJob GrossSalary----------------
update employeejob set GrossSalary=earn.amount
from(
select sum(amount)amount from EmployeeSalaryStructureDetail
where 1=1 
and IsEarning=1 and SalaryType not in('gross','other') and EmployeeId=@EmployeeId ) earn
where employeejob.EmployeeId=@EmployeeId

-------------------------------------------------------
----------------EmployeeJob BasicSalary----------------
UPDATE EmployeeJob set BasicSalary=earn.amount
from(
select sum(amount)amount from EmployeeSalaryStructureDetail
where 1=1 
and IsEarning=1 and SalaryType in('basic') and EmployeeId=@EmployeeId ) earn
where employeejob.EmployeeId=@EmployeeId

 ";

                    SqlCommand cmdUpdateEarn = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdateEarn.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    exeRes = cmdUpdateEarn.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);


                    retResults[2] = vm.Id.ToString();// Return Id
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
                    throw new ArgumentNullException("Group Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Group.";
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


        public string[] DeleteDetailNew(EmployeeSalaryStructureDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete ESS"; //Method Name

            int transResult = 0;
            int countId = 0;
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
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                transaction = currConn.BeginTransaction("DeleteToBank");

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (vm.SalaryType != "Other")
                {
                    retResults[1] = "Basic not Changeable!";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Basic not Changeable", "");
                }

                #region Update Settings

                sqlText = "";
                sqlText = "delete EmployeeSalaryStructureDetail ";
                sqlText += " where Id=@Id AND IsCurrent = 1";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);



                retResults[2] = "";// Return Id
                retResults[3] = sqlText; //  SQL Query

                #region Commit

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("Employee Salary Structure Detail Delete", vm.Id + " could not Delete.");
                }

                #endregion Commit

                #endregion Update Settings
                iSTransSuccess = true;



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
                    retResults[1] = "Unexpected error to delete EmployeeSalaryStructureDetail Information.";
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

        #region Use Later

        public string[] InsertDetail(EmployeeSalaryStructureDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeeSalaryStructureDetail Update"; //Method Name

            int transResult = 0;

            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
            decimal basic = 0;
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
                if (transaction == null) transaction = currConn.BeginTransaction("UpdateToGroup");

                #endregion open connection and transaction

                sqlText = "Select Top 1 * from EmployeeSalaryStructureDetail";
                sqlText += " WHERE EmployeeId=@EmployeeId   ";
                sqlText += " and  SalaryType='basic' ";
                SqlCommand cmdsEarning = new SqlCommand(sqlText, currConn, transaction);
                cmdsEarning.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                using (SqlDataReader dr = cmdsEarning.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        basic = Convert.ToDecimal(dr["Amount"]);
                        vm.EmployeeSalaryStructureId = dr["EmployeeSalaryStructureId"].ToString();
                    }
                    dr.Close();
                }

                #region Exist
                sqlText = "  ";
                sqlText += @" SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) Code
                        FROM EmployeeSalaryStructureDetail ";
                sqlText += " WHERE SalaryTypeId=@SalaryTypeId   ";
                sqlText += " and  EmployeeId=@EmployeeId ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@SalaryTypeId", vm.SalaryTypeId);
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                var exeRes = cmdExist.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    retResults[1] = "SalaryType already used!";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Code already used!", "");
                }

                #endregion Exist

                if (vm != null)
                {
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from EmployeeSalaryStructureDetail ";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn, transaction);
                    exeRes = cmdNextId.ExecuteScalar();
                    int count = Convert.ToInt32(exeRes);

                    vm.Id = (count + 1);

                    #region Insert Settings
                    #region SQL
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeSalaryStructureDetail(
 Id
,SalaryTypeId
,SalaryType
,EmployeeId
,IsFixed
,Portion
,PortionSalaryType
,Amount
,EmployeeSalaryStructureId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,IsEarning   
,IsCurrent               
) 
VALUES ( 
@Id
,@SalaryTypeId
,@SalaryType
,@EmployeeId
,@IsFixed
,@Portion
,@PortionSalaryType
,@Amount
,@EmployeeSalaryStructureId
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@IsEarning
,@IsCurrent
) 
";
                    #endregion SQL


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@SalaryTypeId", vm.SalaryTypeId);
                    cmdInsert.Parameters.AddWithValue("@SalaryType", "Other");
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@IsFixed", vm.IsFixed);

                    cmdInsert.Parameters.AddWithValue("@Portion", vm.Portion);
                    if (vm.IsFixed)
                    {
                        vm.Amount = vm.Portion;
                        vm.PortionSalaryType = "";

                    }
                    else
                    {

                        vm.Amount = basic * vm.Portion / 100;
                        vm.PortionSalaryType = "Basic";

                    }
                    cmdInsert.Parameters.AddWithValue("@PortionSalaryType", vm.PortionSalaryType);
                    cmdInsert.Parameters.AddWithValue("@Amount", vm.Amount);
                    cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", vm.EmployeeSalaryStructureId);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@IsEarning", vm.IsEarning);
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);

                    cmdInsert.ExecuteNonQuery();
                    EmployeeJobDAL jDAL = new EmployeeJobDAL();

                    retResults = jDAL.UpdateJobSalary(vm.EmployeeId, currConn, transaction);
                    if (retResults[0] != "Success")
                    {
                        retResults[1] = "Job Salary not Updated.";
                        throw new ArgumentNullException("", "");
                    }
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
                    throw new ArgumentNullException("Group Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Group.";
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
        //==================Update =================
        public string[] UpdateDetail(EmployeeSalaryStructureDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee [Group] Update"; //Method Name
            decimal basic = 0;

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
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                transaction = currConn.BeginTransaction("UpdateToGroup");

                #endregion open connection and transaction
                ////If SalaryStructureId is FromGross Then Basic Check is not necessaary!
                bool IsGross = false;
                sqlText = @"Select IsGross
From
EmployeeStructureGroup
WHERE 1=1 
";
                sqlText += " AND EmployeeId=@EmployeeId   ";
                SqlCommand cmdIsGross = new SqlCommand(sqlText, currConn, transaction);
                cmdIsGross.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                using (SqlDataReader dr = cmdIsGross.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        IsGross = Convert.ToBoolean(dr["IsGross"]);
                    }
                    dr.Close();
                }

                if (!IsGross)
                {
                    if (vm.SalaryType.ToLower() == "basic")
                    {
                        retResults[1] = "Basic not Changeable !";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Basic not Changeable !", "");
                    }
                }

                #region Exist
                sqlText = "  ";
                sqlText += " SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) Code FROM EmployeeSalaryStructureDetail ";
                sqlText += " WHERE SalaryTypeId=@SalaryTypeId AND Id<>@Id  ";
                sqlText += " and  EmployeeId=@EmployeeId ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                cmdExist.Parameters.AddWithValue("@SalaryTypeId", vm.SalaryTypeId);
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                var exeRes = cmdExist.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    retResults[1] = "SalaryType already used!";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Code already used!", "");
                }

                #endregion Exist

                if (vm != null)
                {

                    sqlText = "Select Top 1 * from EmployeeSalaryStructureDetail";
                    sqlText += " WHERE EmployeeId=@EmployeeId   ";
                    sqlText += " and  SalaryType='basic' ";
                    SqlCommand cmdsEarning = new SqlCommand(sqlText, currConn, transaction);
                    cmdsEarning.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    using (SqlDataReader dr = cmdsEarning.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            basic = Convert.ToDecimal(dr["Amount"]);
                            vm.EmployeeSalaryStructureId = dr["EmployeeSalaryStructureId"].ToString();
                        }
                        dr.Close();
                    }

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeSalaryStructureDetail set";
                    sqlText += " SalaryTypeId=@SalaryTypeId,";
                    sqlText += " Portion=@Portion,";
                    sqlText += " IsFixed=@IsFixed,";
                    sqlText += " PortionSalaryType=@PortionSalaryType,";
                    sqlText += " Amount=@Amount,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@SalaryTypeId", vm.SalaryTypeId);
                    cmdUpdate.Parameters.AddWithValue("@IsFixed", vm.IsFixed);
                    cmdUpdate.Parameters.AddWithValue("@Portion", vm.Portion);

                    if (vm.IsFixed)
                    {
                        vm.Amount = vm.Portion;
                        vm.PortionSalaryType = "";

                    }
                    else
                    {
                        vm.Amount = basic * vm.Portion / 100;
                        vm.PortionSalaryType = "Basic";
                    }
                    cmdUpdate.Parameters.AddWithValue("@Amount", vm.Amount);
                    cmdUpdate.Parameters.AddWithValue("@PortionSalaryType", vm.PortionSalaryType);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    sqlText = "";
                    sqlText = @" update EmployeeSalaryStructureDetail set Amount=earn.amount
 from(
 select sum(amount)amount from EmployeeSalaryStructureDetail
 where 1=1 
 and IsEarning=1 and SalaryType not in('gross') and EmployeeId=@EmployeeId ) earn
 where EmployeeSalaryStructureDetail.EmployeeId=@EmployeeId
 and SalaryType in('gross') 

 update employeejob set GrossSalary=earn.amount
 from(
 select sum(amount)amount from EmployeeSalaryStructureDetail
 where 1=1 
 and IsEarning=1 and SalaryType not in('gross') and EmployeeId=@EmployeeId ) earn
 where employeejob.EmployeeId=@EmployeeId

 ";

                    SqlCommand cmdUpdateEarn = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdateEarn.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    exeRes = cmdUpdateEarn.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);


                    retResults[2] = vm.Id.ToString();// Return Id
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
                    throw new ArgumentNullException("Group Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Group.";
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
        public string[] DeleteDetail(EmployeeSalaryStructureDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete ESS"; //Method Name

            int transResult = 0;
            int countId = 0;
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
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                transaction = currConn.BeginTransaction("DeleteToBank");

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (vm.SalaryType != "Other")
                {
                    retResults[1] = "Basic not Changeable!";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Basic not Changeable", "");
                }

                #region Update Settings

                sqlText = "";
                sqlText = "delete EmployeeSalaryStructureDetail ";
                sqlText += " where Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);



                retResults[2] = "";// Return Id
                retResults[3] = sqlText; //  SQL Query

                #region Commit

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("Employee Salary Structure Detail Delete", vm.Id + " could not Delete.");
                }

                #endregion Commit

                #endregion Update Settings
                iSTransSuccess = true;



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
                    retResults[1] = "Unexpected error to delete EmployeeSalaryStructureDetail Information.";
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
        #endregion Use Later

        public DataTable ExcelData(EmployeeStructureGroupVM vm)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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

                #region SqlText

                sqlText = @"

select e.Code,E.EmpName,e.Designation,e.Department,e.Project,st.Name,Portion Amount,
e.EmployeeId,s.SalaryTypeId from EmployeeSalaryStructureDetail s
left outer join ViewEmployeeInformation e on e.EmployeeId=s.EmployeeId
left outer join EnumSalaryType st on st.Id=s.SalaryTypeId
where s.SalaryTypeId=@SalaryTypeId

";

                #endregion SqlText

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

                da.SelectCommand.Parameters.AddWithValue("@SalaryTypeId", vm.SalaryTypeId);

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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return dt;
        }

        public string[] ImportExcelFile(EmployeeStructureGroupVM paramVM)
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
            retResults[5] = "ImportExcelFile"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;

            #endregion

            #region try
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                #region Excel Reader

                string FileName = paramVM.File.FileName;
                string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                File.Delete(Fullpath);
                if (paramVM.File != null && paramVM.File.ContentLength > 0)
                {
                    paramVM.File.SaveAs(Fullpath);
                }


                FileStream stream = File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();


                dt = ds.Tables[0];
                reader.Close();

                File.Delete(Fullpath);
                #endregion

                #region open connection and transaction
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

                #region Update

                if (dt != null && dt.Rows.Count > 0)
                {

                    #region Assign Data
                    int i = 0;
                    foreach (DataRow dr in dt.Rows)
                    {

                        string EmployeeId = dr["EmployeeId"].ToString();
                        string SalaryTypeId = Convert.ToString(dr["SalaryTypeId"]);
                        decimal Amount = Convert.ToDecimal(dr["Amount"]);

                        #region Update Settings

                        sqlText = @"

--declare @EmployeeId as varchar(100)='1_1'
--declare @SalaryTypeId as varchar(100)='1_34'
--declare @Amount as Decimal(18,2)=1000


declare @gross as Decimal(18,2)
declare @basic as Decimal(18,2)
select @gross=Amount from EmployeeSalaryStructureDetail where SalaryType='gross' and EmployeeId=@EmployeeId  
select @basic=Amount from EmployeeSalaryStructureDetail where SalaryType='basic' and EmployeeId=@EmployeeId  

update EmployeeSalaryStructureDetail set Portion=@Amount,
Amount= case when IsFixed=1 then @Amount 
when IsFixed=0 and PortionSalaryType='basic' then @Amount*@basic/100
when IsFixed=0 and PortionSalaryType='gross' then @Amount*@gross/100
end
where EmployeeId=@EmployeeId and SalaryTypeId=@SalaryTypeId
                    
                    ";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@SalaryTypeId", SalaryTypeId);
                        cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmdUpdate.Parameters.AddWithValue("@Amount", Amount);

                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        retResults[2] = EmployeeId;// Return Id
                        retResults[3] = sqlText; //  SQL Query

                        #region Commit

                        if (transResult <= 0)
                        {
                        }

                        #endregion Commit

                        #endregion Update Settings


                    }
                    #endregion

                }
                else
                {
                    throw new ArgumentNullException("Excel Import", "Could not found any item.");
                }
                #endregion

                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Import Successfully.";
                //retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[1] = ex.Message.ToString(); //catch ex
                transaction.Rollback();
                return retResults;
            }
            finally
            {
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }


        #endregion Methods
    }
}
