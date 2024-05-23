using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.GF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SymServices.GF
{
    public class EmployeeSettlementDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        public List<EmployeeSettlementVM> DropDown(string tType = "", int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeSettlementVM> VMs = new List<EmployeeSettlementVM>();
            EmployeeSettlementVM vm;
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
                #region sql statement
                sqlText = @"
SELECT
Id

   FROM GFEmployeeSettlements
WHERE  1=1
AND IsArchive=0
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeSettlementVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
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

        //==================DropDown=================
        public List<EmployeeSettlementVM> LeftEmployeeDropDown(string tType = "", int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeSettlementVM> VMs = new List<EmployeeSettlementVM>();
            EmployeeSettlementVM vm;
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
                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();

                #region sql statement
                sqlText = @"
SELECT
es.Id
,es.EmployeeId
,ve.Code 
,ve.EmpName Name
   FROM GFEmployeeSettlements es

";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON es.EmployeeId=ve.EmployeeId";
                sqlText += " WHERE  1=1 AND es.IsArchive=0";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeSettlementVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();
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
        public List<EmployeeSettlementVM> SelectLeftEmployeeList(string EmployeeId = "", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeSettlementVM> VMs = new List<EmployeeSettlementVM>();
            EmployeeSettlementVM vm;
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
                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();

                #region sql statement
                #region SqlText
                sqlText = @"
Select
ve.Id
,el.EmployeeId
,ve.Code EmpCode
,ve.EmpName
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,ve.EmployeeId
,ve.DesignationId
,ve.DepartmentId
,ve.SectionId
,ve.ProjectId
,ve.GrossSalary
,ve.BasicSalary
,ve.JoinDate
,el.LeftDate
,IsNull(DATEDIFF(year, ve.JoinDate, el.LeftDate),0) TotalJobDurationYear
";
                sqlText += " From " + hrmDB + ".[dbo].EmployeeLeftInformation el";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON el.EmployeeId=ve.EmployeeId";
                sqlText += @" Where 1=1 AND el.IsArchive=0
AND el.EmployeeId Not In (Select EmployeeId From GFEmployeeSettlements)
";
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += @" AND el.EmployeeId=@EmployeeId";
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
                sqlText += @" ORDER BY TotalJobDurationYear desc";

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

                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeSettlementVM();
                    //vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmpCode = dr["EmpCode"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();

                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.LastGross = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.LastBasic = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.LeftDate = Ordinary.StringToDate(dr["LeftDate"].ToString());
                    vm.TotalJobDurationYear = Convert.ToInt32(dr["TotalJobDurationYear"]);
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
        public List<EmployeeSettlementVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeSettlementVM> VMs = new List<EmployeeSettlementVM>();
            EmployeeSettlementVM vm;
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
                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();

                #region sql statement
                #region SqlText

                sqlText = @"
SELECT
 es.Id
,es.GFPolicyId
,gfp.PolicyName GFPolicyName
,ISNULL(es.PolicyJobDurationYearFrom,0)PolicyJobDurationYearFrom
,ISNULL(es.PolicyJobDurationYearTo  ,0)PolicyJobDurationYearTo
,ISNULL(PolicyMultipicationFactor,0)PolicyMultipicationFactor
,ISNULL(PolicyIsFixed,0)            PolicyIsFixed
,es.PolicyLastBasicMultipication
,es.EmployeeId
,es.ProjectId
,es.DepartmentId
,es.SectionId
,es.DesignationId
,es.JoinDate
,es.LeftDate
,ISNULL(es.TotalJobDurationYear,0)TotalJobDurationYear
,es.LastGross
,es.LastBasic
,es.SettlementDate
,es.GFValue
,es.ServiceCharge

,ve.Code EmpCode
,ve.EmpName
,ve.Designation
,ve.Department
,ve.Section
,ve.Project

,es.Remarks
,es.IsActive
,es.IsArchive
,es.CreatedBy
,es.CreatedAt
,es.CreatedFrom
,es.LastUpdateBy
,es.LastUpdateAt
,es.LastUpdateFrom

FROM GFEmployeeSettlements es
LEFT OUTER JOIN GFPolicies gfp ON es.GFPolicyId = gfp.Id
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON es.EmployeeId=ve.EmployeeId";
                sqlText += " WHERE  1=1 AND es.IsArchive = 0";


                if (Id > 0)
                {
                    sqlText += @" and es.Id=@Id";
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
                sqlText += @" ORDER BY TotalJobDurationYear desc";

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
                    vm = new EmployeeSettlementVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.GFPolicyId = Convert.ToInt32(dr["GFPolicyId"]);
                    vm.GFPolicyName = dr["GFPolicyName"].ToString();
                   
                    vm.PolicyJobDurationYearFrom = Convert.ToInt32(dr["PolicyJobDurationYearFrom"]);
                    vm.PolicyJobDurationYearTo = Convert.ToInt32(dr["PolicyJobDurationYearTo"]);
                    vm.PolicyMultipicationFactor = Convert.ToDecimal(dr["PolicyMultipicationFactor"]);
                    vm.PolicyIsFixed = Convert.ToBoolean(dr["PolicyIsFixed"]);
                    vm.PolicyLastBasicMultipication = Convert.ToDecimal(dr["PolicyLastBasicMultipication"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.LeftDate = Ordinary.StringToDate(dr["LeftDate"].ToString());

                    vm.TotalJobDurationYear = Convert.ToInt32(dr["TotalJobDurationYear"]);
                    vm.LastGross = Convert.ToDecimal(dr["LastGross"]);
                    vm.LastBasic = Convert.ToDecimal(dr["LastBasic"]);
                    vm.SettlementDate = Ordinary.StringToDate(dr["SettlementDate"].ToString());
                    vm.GFValue = Convert.ToDecimal(dr["GFValue"]);
                    vm.ServiceCharge = Convert.ToDecimal(dr["ServiceCharge"]);

                    vm.EmpCode = dr["EmpCode"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();

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
        public string[] Insert(EmployeeSettlementVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertEmployeeSettlement"; //Method Name
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
                #region Save


                vm.Id = _cDal.NextId("GFEmployeeSettlements", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GFEmployeeSettlements(
Id
,GFPolicyId
,PolicyJobDurationYearFrom
,PolicyJobDurationYearTo
,PolicyLastBasicMultipication
,EmployeeId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,JoinDate
,LeftDate
,TotalJobDurationYear
,LastGross
,LastBasic
,SettlementDate
,GFValue
,ServiceCharge
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@GFPolicyId
,@PolicyJobDurationYearFrom
,@PolicyJobDurationYearTo
,@PolicyLastBasicMultipication
,@EmployeeId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@JoinDate
,@LeftDate
,@TotalJobDurationYear
,@LastGross
,@LastBasic
,@SettlementDate
,@GFValue
,@ServiceCharge
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@GFPolicyId", vm.GFPolicyId);
                    cmdInsert.Parameters.AddWithValue("@PolicyJobDurationYearFrom", vm.PolicyJobDurationYearFrom);
                    cmdInsert.Parameters.AddWithValue("@PolicyJobDurationYearTo", vm.PolicyJobDurationYearTo);
                    
                    cmdInsert.Parameters.AddWithValue("@PolicyMultipicationFactor", vm.PolicyMultipicationFactor);
                    cmdInsert.Parameters.AddWithValue("@PolicyIsFixed", vm.PolicyIsFixed);
                    cmdInsert.Parameters.AddWithValue("@PolicyLastBasicMultipication", vm.PolicyLastBasicMultipication);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@JoinDate", Ordinary.DateToString(vm.JoinDate));
                    cmdInsert.Parameters.AddWithValue("@LeftDate", Ordinary.DateToString(vm.LeftDate));
                    cmdInsert.Parameters.AddWithValue("@TotalJobDurationYear", vm.TotalJobDurationYear);
                    cmdInsert.Parameters.AddWithValue("@LastGross", vm.LastGross);
                    cmdInsert.Parameters.AddWithValue("@LastBasic", vm.LastBasic);
                    cmdInsert.Parameters.AddWithValue("@SettlementDate", Ordinary.DateToString(vm.SettlementDate));
                    cmdInsert.Parameters.AddWithValue("@GFValue", vm.GFValue);
                    cmdInsert.Parameters.AddWithValue("@ServiceCharge", vm.ServiceCharge);
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
                        throw new ArgumentNullException("Unexpected error to update GFEmployeeSettlements.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This EmployeeSettlement already used!";
                    throw new ArgumentNullException("Please Input EmployeeSettlement Value", "");
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
            #region Catch and Finally
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
        public string[] Update(EmployeeSettlementVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeSettlement Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeSettlement"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE GFEmployeeSettlements SET";

                    sqlText += "  GFPolicyId=@GFPolicyId";
                    
                    
        
                    sqlText += ", PolicyJobDurationYearFrom=@PolicyJobDurationYearFrom";
                    sqlText += ", PolicyJobDurationYearTo=@PolicyJobDurationYearTo";

                    sqlText += ", PolicyLastBasicMultipication=@PolicyLastBasicMultipication";
                    sqlText += ", PolicyMultipicationFactor=@PolicyMultipicationFactor";
                    sqlText += ", PolicyIsFixed=@PolicyIsFixed";
                    sqlText += ", EmployeeId=@EmployeeId";
                    sqlText += ", ProjectId=@ProjectId";
                    sqlText += ", DepartmentId=@DepartmentId";
                    sqlText += ", SectionId=@SectionId";
                    sqlText += ", DesignationId=@DesignationId";
                    sqlText += ", JoinDate=@JoinDate";
                    sqlText += ", LeftDate=@LeftDate";
                    sqlText += ", TotalJobDurationYear=@TotalJobDurationYear";
                    sqlText += ", LastGross=@LastGross";
                    sqlText += ", LastBasic=@LastBasic";
                    sqlText += ", SettlementDate=@SettlementDate";
                    sqlText += ", GFValue=@GFValue";
                    sqlText += ", ServiceCharge=@ServiceCharge";

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
                    cmdUpdate.Parameters.AddWithValue("@GFPolicyId", vm.GFPolicyId);
                    
                    cmdUpdate.Parameters.AddWithValue("@PolicyJobDurationYearFrom", vm.PolicyJobDurationYearFrom);
                    cmdUpdate.Parameters.AddWithValue("@PolicyJobDurationYearTo", vm.PolicyJobDurationYearTo);
                    
                    cmdUpdate.Parameters.AddWithValue("@PolicyMultipicationFactor", vm.PolicyMultipicationFactor);
                    cmdUpdate.Parameters.AddWithValue("@PolicyIsFixed", vm.PolicyIsFixed);
                    cmdUpdate.Parameters.AddWithValue("@PolicyLastBasicMultipication", vm.PolicyLastBasicMultipication);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdUpdate.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdUpdate.Parameters.AddWithValue("@JoinDate", Ordinary.DateToString(vm.JoinDate));
                    cmdUpdate.Parameters.AddWithValue("@LeftDate", Ordinary.DateToString(vm.LeftDate));
                    cmdUpdate.Parameters.AddWithValue("@TotalJobDurationYear", vm.TotalJobDurationYear);
                    cmdUpdate.Parameters.AddWithValue("@LastGross", vm.LastGross);
                    cmdUpdate.Parameters.AddWithValue("@LastBasic", vm.LastBasic);
                    cmdUpdate.Parameters.AddWithValue("@SettlementDate", Ordinary.DateToString(vm.SettlementDate));
                    cmdUpdate.Parameters.AddWithValue("@GFValue", vm.GFValue);
                    cmdUpdate.Parameters.AddWithValue("@ServiceCharge", vm.ServiceCharge);
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
                        throw new ArgumentNullException("Unexpected error to update GFEmployeeSettlements.", "");
                    }
                    #endregion SqlExecution

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("EmployeeSettlement Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("EmployeeSettlement Update", "Could not found any item.");
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
        public string[] Delete(EmployeeSettlementVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeSettlement"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string retVal = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeSettlement"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = " ";
                        sqlText = "";
                        sqlText = "UPDATE GFEmployeeSettlements set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);


                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to Delete  GFEmployeeSettlements.", "");
                        }
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(" GFEmployeeSettlements Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("EmployeeSettlement Information Delete", "Could not found any item.");
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
        public DataTable Report(EmployeeSettlementVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
                #region SqlText

                sqlText = @"
SELECT
 es.Id
,es.GFPolicyId
,gfp.PolicyName GFPolicyName
,es.PolicyJobDurationYearFrom
,es.PolicyJobDurationYearTo
,es.PolicyMultipicationFactor
,es.PolicyIsFixed
,es.PolicyLastBasicMultipication
,es.EmployeeId
,es.JoinDate
,es.LeftDate
,es.TotalJobDurationYear
,es.LastGross
,es.LastBasic
,es.SettlementDate
,es.GFValue
,es.ServiceCharge

,ve.Code EmpCode
,ve.EmpName
,ve.Designation
,ve.Department
,ve.Section
,ve.Project

,es.Remarks
FROM GFEmployeeSettlements es
LEFT OUTER JOIN GFPolicies gfp ON es.GFPolicyId = gfp.Id
";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON es.EmployeeId=ve.EmployeeId";
                sqlText += " WHERE  1=1 AND es.IsArchive = 0";
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
                string[] dateColumns = { "JoinDate", "LeftDate", "SettlementDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, dateColumns);
                #endregion SqlExecution

                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        #endregion
    }
}
