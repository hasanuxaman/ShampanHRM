using Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymOrdinary;
using SymServices.Common;
using SymServices.Enum;
using SymServices.HRM;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Payroll
{
    public class BonusProcessDAL
    {

        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDAL = new CommonDAL();
        EmployeeInfoDAL _dal = new EmployeeInfoDAL();
        #endregion

        #region Methods

        //==================SelectAll=================
        public List<BonusProcessVM> SelectAll(string Id = "", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<BonusProcessVM> vms = new List<BonusProcessVM>();
            BonusProcessVM vm;
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


                #region SqlText
                sqlText = @"
SELECT
sbd.Id
,sbd.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.Grosssalary, e.Basicsalary
,sbd.Amount 
,ISNULL(sbd.TaxValue,0)    TaxValue
,ISNULL(sbd.NetPayAmount,0)NetPayAmount

,sbd.DesignationId, sbd.DepartmentId, sbd.SectionId, sbd.ProjectId

, bs.name BonusStructureName
, bnm.name BonusType
,sbd.EffectDate
,sbd.FiscalYear
,sbd.FiscalYearDetailId
,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd

,ISNULL(sbd.IsManual, 0)IsManual

,sbd.Remarks
,sbd.IsActive
,sbd.IsArchive
,sbd.CreatedBy
,sbd.CreatedAt
,sbd.CreatedFrom
,sbd.LastUpdateBy
,sbd.LastUpdateAt
,sbd.LastUpdateFrom
From SalaryBonusDetail sbd 
left outer join ViewEmployeeInformation e on sbd.EmployeeId=e.id
left outer join BonusStructure bs on sbd.BonusStructureId = bs.Id 
left outer join BonusName bnm on sbd.BonusNameId = bnm.Id  
left OUTER JOIN FiscalYearDetail fyd ON sbd.FiscalYearDetailId = fyd.Id


Where 1=1 and  sbd.IsArchive=0 and sbd.Amount > 0
";
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    sqlText += @" and sbd.Id=@Id";
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

                sqlText += @" ORDER BY e.Department,e.EmpName desc";
                #endregion
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

                if (!string.IsNullOrWhiteSpace(Id))
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BonusProcessVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Amount = Convert.ToDecimal(dr["Amount"]);
                    vm.TaxValue = Convert.ToDecimal(dr["TaxValue"]);
                    vm.NetPayAmount = Convert.ToDecimal(dr["NetPayAmount"]);


                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();

                    vm.BonusStructureName = dr["BonusStructureName"].ToString();
                    vm.BonusType = dr["BonusType"].ToString();


                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);

                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();

                    vm.IsManual = Convert.ToBoolean(dr["IsManual"]);


                    vm.EffectDate = Ordinary.StringToDate(Convert.ToString(dr["EffectDate"]));
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.FiscalYear = Convert.ToInt32(dr["FiscalYear"]);
                    vm.FiscalPeriod = dr["FiscalPeriod"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();

                    vms.Add(vm);
                }
                dr.Close();
                #endregion

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

        public List<BonusProcessVM> SelectBonusDetailAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<BonusProcessVM> vms = new List<BonusProcessVM>();
            BonusProcessVM vm;
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
select distinct BonusNameId,bnm.Name BonusName, BonusStructureId,bs.Name BonusStructureName
,vem.Code ,vem.EmpName
,vem.DepartmentId,vem.Department
,vem.Section,vem.SectionId,vem.ProjectId,vem.Project,sbd.Id,
COUNT(vem.EmployeeId)TotalEmployee
,Sum(sbd.Amount) Amount 
,ISNULL(Sum(sbd.TaxValue),0)    TaxValue 
,ISNULL(Sum(sbd.NetPayAmount),0)NetPayAmount 

FROM SalaryBonusDetail sbd
left outer join BonusStructure bs on sbd.BonusStructureId = bs.Id 
left outer join BonusName bnm on sbd.BonusNameId = bnm.Id 
left outer join ViewEmployeeInformation vem on sbd.EmployeeId = vem.EmployeeId
group by BonusNameId,bnm.Name, BonusStructureId,bs.Name
, vem.Code ,vem.EmpName
,vem.DepartmentId,vem.Department
,vem.Section,vem.SectionId,vem.ProjectId,vem.Project,sbd.IsActive,sbd.IsArchive,sbd.Id
having sbd.IsActive=1 and sbd.IsArchive=0
order by BonusNameId,BonusStructureId,DepartmentId,SectionId,ProjectId
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BonusProcessVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Amount = Convert.ToDecimal(dr["Amount"]);
                    vm.TaxValue = Convert.ToDecimal(dr["TaxValue"]);
                    vm.NetPayAmount = Convert.ToDecimal(dr["NetPayAmount"]);
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.BonusStructureName = dr["BonusStructureName"].ToString();
                    vm.BonusType = dr["BonusName"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.TotalEmployee = dr["TotalEmployee"].ToString();
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
        public BonusProcessVM SelectById_Backup(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            BonusProcessVM vm = new BonusProcessVM();

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
sbd.Id
,sbd.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.Grosssalary, e.Basicsalary
,sbd.Amount Amount
,ISNULL(sbd.TaxValue,0) TaxValue
,ISNULL(sbd.NetPayAmount,0) NetPayAmount

,sbd.DesignationId, sbd.DepartmentId, sbd.SectionId, sbd.ProjectId

, bs.name BonusStructureName
, bnm.name BonusType

,sbd.FiscalYear
,sbd.FiscalYearDetailId
,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd
,sbd.Remarks
,sbd.Isbdctive
,sbd.Isbdrchive
,sbd.CreatedBy
,sbd.CreatedAt
,sbd.CreatedFrom
,sbd.LastUpdateBy
,sbd.LastUpdateAt
,sbd.LastUpdateFrom
From SalaryBonusDetail sbd 
left outer join ViewEmployeeInformation e on sbd.EmployeeId=e.id
left outer join BonusStructure bs on sbd.BonusStructureId = bs.Id 
left outer join BonusName bnm on sbd.BonusNameId = bnm.Id  
left OUTER JOIN FiscalYearDetail fyd ON sbd.FiscalYearDetailId = fyd.Id
Where 1=1 and  sbd.IsArchive=0 and sbd.id=@Id and sbd.Amount > 0
ORDER BY e.Department, e.EmpName desc
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BonusProcessVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Amount = Convert.ToDecimal(dr["Amount"]);
                    vm.TaxValue = Convert.ToDecimal(dr["TaxValue"]);
                    vm.NetPayAmount = Convert.ToDecimal(dr["NetPayAmount"]);


                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();

                    vm.BonusStructureName = dr["BonusStructureName"].ToString();
                    vm.BonusType = dr["BonusType"].ToString();


                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);

                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();

                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.FiscalYear = Convert.ToInt32(dr["FiscalYear"]);
                    vm.FiscalPeriod = dr["FiscalPeriod"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
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
        public string[] Insert(BonusProcessVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertBonusProcess"; //Method Name
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

                #endregion open connection and transaction

                #region Exist

                bool exist = false;
                string[] cFields = { "EmployeeId", "BonusNameId" };
                string[] cValues = { vm.EmployeeId, vm.BonusNameId.ToString() };
                exist = _cDAL.ExistCheck("SalaryBonusDetail", cFields, cValues, currConn, transaction);

                if (exist)
                {
                    retResults = _cDAL.DeleteTable("SalaryBonusDetail", cFields, cValues, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException("", retResults[1]);
                    }
                }

                #region Comments

                //////sqlText = "  ";
                //////sqlText += " SELECT   count(Id) FROM SalaryBonusDetail ";
                //////sqlText += " WHERE EmployeeId=@EmployeeId AND BonusNameId=@BonusNameId";
                //////SqlCommand cmdExist = new SqlCommand(sqlText, currConn, transaction);
                //////cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                //////cmdExist.Parameters.AddWithValue("@BonusNameId", vm.BonusNameId);
                //////var exeRes = cmdExist.ExecuteScalar();
                //////int objfoundId = Convert.ToInt32(exeRes);

                //////if (objfoundId > 0)
                //////{
                //////    sqlText = "  ";
                //////    sqlText += " delete FROM SalaryBonusDetail ";
                //////    sqlText += " WHERE EmployeeId=@EmployeeId AND BonusNameId=@BonusNameId";
                //////    SqlCommand cmdExistD = new SqlCommand(sqlText, currConn);
                //////    cmdExistD.Transaction = transaction;
                //////    cmdExistD.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                //////    cmdExistD.Parameters.AddWithValue("@BonusNameId", vm.BonusNameId);
                //////    var exeResD = cmdExistD.ExecuteScalar();
                //////}
                #endregion


                #endregion Exist

                #region Save
                //vm.Id = cdal.NextId("BonusProcess", currConn, transaction).ToString();
                if (vm != null)
                {
                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" INSERT INTO SalaryBonusDetail
(
EmployeeId
, Amount
, TaxValue
, NetPayAmount
, ProjectId
, SectionId
, DepartmentId
, DesignationId
, BonusStructureId
, BonusNameId
, BasicSalary
, GrossSalary
, GradeId
, IsManual
, EffectDate
, FiscalYear
, FiscalYearDetailId
, Remarks
, IsActive
, IsArchive
, CreatedBy
, CreatedAt
, CreatedFrom
) VALUES (
@EmployeeId
,@Amount
,@TaxValue
,@NetPayAmount
,@ProjectId
,@SectionId
,@DepartmentId
,@DesignationId
,@BonusStructureId
,@BonusNameId
,@BasicSalary
,@GrossSalary
,@GradeId
,@IsManual
,@EffectDate
,@FiscalYear
,@FiscalYearDetailId
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) ";
                    #endregion

                    #region SqlExecution

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    //cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Amount", vm.Amount);
                    cmdInsert.Parameters.AddWithValue("@TaxValue", vm.TaxValue);
                    cmdInsert.Parameters.AddWithValue("@NetPayAmount", vm.NetPayAmount);
                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@BonusStructureId", vm.BonusStructureId);
                    cmdInsert.Parameters.AddWithValue("@BonusNameId", vm.BonusNameId);
                    cmdInsert.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                    cmdInsert.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);
                    cmdInsert.Parameters.AddWithValue("@GradeId", vm.GradeId);

                    cmdInsert.Parameters.AddWithValue("@IsManual", vm.IsManual);

                    cmdInsert.Parameters.AddWithValue("@EffectDate", Ordinary.DateToString(vm.EffectDate));
                    cmdInsert.Parameters.AddWithValue("@FiscalYear", vm.FiscalYear);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                    //FiscalYear
                    //FiscalYearDetailId
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.ExecuteNonQuery();
                    #endregion
                }
                else
                {
                    retResults[1] = "Please Input Bonus Process Value";
                    throw new ArgumentNullException("Please Input Bonus Process Value", "");
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

        public string[] Insert(BonusProcessVM paramVM, List<BonusProcessVM> VMs, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertBonusProcess"; //Method Name
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

                #endregion open connection and transaction

                #region Save
                //vm.Id = cdal.NextId("BonusProcess", currConn, transaction).ToString();
                if (VMs != null && VMs.Count > 0)
                {
                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" INSERT INTO SalaryBonusDetail
(
EmployeeId
, Amount
, TaxValue
, NetPayAmount
, ProjectId
, SectionId
, DepartmentId
, DesignationId
, BonusStructureId
, BonusNameId
, BasicSalary
, GrossSalary
, GradeId
, IsManual
, EffectDate
, FiscalYear
, FiscalYearDetailId
, Remarks
, IsActive
, IsArchive
, CreatedBy
, CreatedAt
, CreatedFrom
) VALUES (
@EmployeeId
,@Amount
,@TaxValue
,@NetPayAmount
,@ProjectId
,@SectionId
,@DepartmentId
,@DesignationId
,@BonusStructureId
,@BonusNameId
,@BasicSalary
,@GrossSalary
,@GradeId
,@IsManual
,@EffectDate
,@FiscalYear
,@FiscalYearDetailId
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) ";
                    #endregion

                    #region SqlExecution

                    int index = 0;
                    for (index = 0; index < VMs.Count; index++)
                    {
                        var vm = VMs[index];

                        #region Exist

                        bool exist = false;
                        string[] cFields = { "EmployeeId", "BonusNameId" };
                        string[] cValues = { vm.EmployeeId, vm.BonusNameId };
                        exist = _cDAL.ExistCheck("SalaryBonusDetail", cFields, cValues, currConn, transaction);

                        if (exist)
                        {
                            retResults = _cDAL.DeleteTable("SalaryBonusDetail", cFields, cValues, currConn,
                                transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException("", retResults[1]);
                            }
                        }

                        #endregion Exist

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        //cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@Amount", vm.Amount);
                        cmdInsert.Parameters.AddWithValue("@TaxValue", vm.TaxValue);
                        cmdInsert.Parameters.AddWithValue("@NetPayAmount", vm.NetPayAmount);
                        cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                        cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                        cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                        cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                        cmdInsert.Parameters.AddWithValue("@BonusStructureId", vm.BonusStructureId);
                        cmdInsert.Parameters.AddWithValue("@BonusNameId", vm.BonusNameId);
                        cmdInsert.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                        cmdInsert.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);
                        cmdInsert.Parameters.AddWithValue("@GradeId", vm.GradeId);

                        cmdInsert.Parameters.AddWithValue("@IsManual", paramVM.IsManual);
                        cmdInsert.Parameters.AddWithValue("@EffectDate", Ordinary.DateToString(paramVM.EffectDate));
                        cmdInsert.Parameters.AddWithValue("@FiscalYear", paramVM.FiscalYear);
                        cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", paramVM.FiscalYearDetailId);

                        //FiscalYear
                        //FiscalYearDetailId
                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", paramVM.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", paramVM.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", paramVM.CreatedFrom);
                        cmdInsert.ExecuteNonQuery();
                    }

                    #endregion
                }
                else
                {
                    retResults[1] = "Please Input Bonus Process Value";
                    throw new ArgumentNullException("Please Input Bonus Process Value", "");
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

        #region Unused Methods

        //==================Update =================
        public string[] Update(BonusProcessVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Bonus Process Update"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToBonusProcess"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist
                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM SalaryBonusDetail ";
                    sqlText += " WHERE EmployeeId=@EmployeeId AND  Id<>@Id";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    var exeRes = cmdExist.ExecuteScalar();
                    int objfoundId = Convert.ToInt32(exeRes);

                    if (objfoundId > 0)
                    {
                        retResults[1] = "Bonus Process already useds!";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Bonus Process Already!", "");
                    }

                    #endregion Exist

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update SalaryBonusDetail set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " Amount=@Amount,";
                    sqlText += " TaxValue=@TaxValue,";
                    sqlText += " NetPayAmount=@NetPayAmount,";

                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@Amount", vm.Amount);
                    cmdUpdate.Parameters.AddWithValue("@TaxValue", vm.TaxValue);
                    cmdUpdate.Parameters.AddWithValue("@NetPayAmount", vm.NetPayAmount);
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
                        // throw new ArgumentNullException("Education Update", BonusProcessVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Bonus Process Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Bonus Process.";
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

        //==================Delete =================
        public string[] Delete(BonusProcessVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteBonusProcess"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToBonusProcess"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used
                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update SalaryBonusDetail set";
                        sqlText += " Amount=0";
                        sqlText += " , TaxValue=0";
                        sqlText += " , NetPayAmount=0";
                        sqlText += " , IsArchive=@IsArchive";
                        sqlText += " , LastUpdateBy=@LastUpdateBy";
                        sqlText += " , LastUpdateAt=@LastUpdateAt";
                        sqlText += " , LastUpdateFrom=@LastUpdateFrom";
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
                        throw new ArgumentNullException("Bonus Process Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Bonus Process Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Bonus Process Information.";
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

        #endregion

        public string[] BonusProcess(BonusProcessVM paramVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string[] retResults = new string[6];
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //BonusProcessVM vm = new BonusProcessVM();
            BonusStructureVM vBonusStructureVM = new BonusStructureVM();
            BonusStructureDAL _BonusStructureDAL = new BonusStructureDAL();
            List<EmployeeInfoVM> vEmployeeInfoVMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vEmployeeInfoVM = new EmployeeInfoVM();
            EmployeeInfoDAL _EmployeeInfoDAL = new EmployeeInfoDAL();
            var sqlText = "";
            #endregion

            #region Try
            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
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

                #region Salary_Bonus

                #region FiscalYearDetail
                FiscalYearDAL _FiscalYearDAL = new FiscalYearDAL();
                FiscalYearDetailVM vFiscalYearDetailVM = new FiscalYearDetailVM();
                {
                    string[] cFields = { "PeriodStart<", "PeriodEnd>" };
                    string[] cValues = { Ordinary.DateToString(paramVM.EffectDate), Ordinary.DateToString(paramVM.EffectDate) };
                    vFiscalYearDetailVM = _FiscalYearDAL.SelectAll_FiscalYearDetail(0, cFields, cValues, currConn, transaction).FirstOrDefault();

                    if (vFiscalYearDetailVM == null)
                    {
                        retResults[1] = "Fiscal Year Not Created for " + paramVM.EffectDate + "! Create First!";
                        throw new ArgumentNullException(retResults[1], "");

                    }

                }
                paramVM.FiscalYear = vFiscalYearDetailVM.Year;
                paramVM.FiscalYearDetailId = vFiscalYearDetailVM.Id;

                #endregion

                #region Bonus Structure

                vBonusStructureVM = _BonusStructureDAL.SelectAll(paramVM.BonusStructureId, currConn, transaction).FirstOrDefault();

                #endregion

                #region Employee List

                sqlText = @"
                SELECT 
                vem.Id
                ,isnull(sal.GrossSalary,0) GrossSalary   
                ,isnull(sal.BasicSalary,0) BasicSalary   
                ,vem.Gender
                ,vem.GradeId
                ,vem.Religion
                ,vem.projectid
                ,vem.DepartmentId
                ,vem.SectionId
                ,vem.DesignationId 
                FROM ViewEmployeeInformation vem left outer join
				( SELECT distinct FiscalYearDetailId,EmployeeId
 , sum( case when salaryhead='Gross' then Amount else 0 end) GrossSalary
 , sum( case when salaryhead='basic' then Amount else 0 end) BasicSalary
 , sum( case when salaryhead='HouseRent' then Amount else 0 end) HouseRent
 , sum( case when salaryhead='Medical' then Amount else 0 end) Medical
 , sum( case when salaryhead='TransportAllowance' then Amount else 0 end) TransportAllowance
 , sum( case when salaryhead='Stamp' then Amount else 0 end) Stamp
 from [ViewSalaryPreCalculation]
where FiscalYearDetailId = @FiscalYearDetailId
 group by FiscalYearDetailId,EmployeeId) sal 
 on sal.EmployeeId = vem.EmployeeId
                WHERE 1=1
                AND vem.IsActive = 1 AND vem.IsArchive = 0

                ";
                if (!string.IsNullOrWhiteSpace(paramVM.ProjectId))
                    sqlText += @" and vem.ProjectId='" + paramVM.ProjectId + "'";

                if (!string.IsNullOrWhiteSpace(paramVM.DepartmentId))
                    sqlText += @" and vem.DepartmentId='" + paramVM.DepartmentId + "'";

                if (!string.IsNullOrWhiteSpace(paramVM.SectionId))
                    sqlText += @" and vem.SectionId='" + paramVM.SectionId + "'";

                if (!string.IsNullOrWhiteSpace(paramVM.DesignationId))
                    sqlText += @" and vem.DesignationId='" + paramVM.DesignationId + "'";

                if (!string.IsNullOrWhiteSpace(paramVM.CodeF))
                    sqlText += @" and vem.Code >='" + paramVM.CodeF + "'";

                if (!string.IsNullOrWhiteSpace(paramVM.CodeT))
                    sqlText += @" and vem.Code<='" + paramVM.CodeT + "'";

                if (!string.IsNullOrWhiteSpace(vBonusStructureVM.Gender) && vBonusStructureVM.Gender.ToLower() != "all")
                    sqlText += @" and vem.Gender ='" + vBonusStructureVM.Gender + "'";

                if (!string.IsNullOrWhiteSpace(vBonusStructureVM.Religion) && vBonusStructureVM.Religion.ToLower() != "all")
                    sqlText += @" and vem.Religion ='" + vBonusStructureVM.Religion + "'";

                sqlText += @" and ( DATEDIFF(dd, CONVERT(datetime,vem.JoinDate),    @EffectDate   ) >='" + vBonusStructureVM.JobAge + "'";
                sqlText += @" and DATEDIFF(dd, CONVERT(datetime,vem.JoinDate),      @EffectDate   ) <='" + vBonusStructureVM.JobAgeTo + "')";

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@EffectDate", paramVM.EffectDate);
                objComm.Parameters.AddWithValue("@FiscalYearDetailId", paramVM.FiscalYearDetailId);


                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vEmployeeInfoVM = new EmployeeInfoVM();
                        vEmployeeInfoVM.Id = dr["Id"].ToString();
                        vEmployeeInfoVM.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        vEmployeeInfoVM.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                        vEmployeeInfoVM.ProjectId = dr["ProjectId"].ToString();
                        vEmployeeInfoVM.DesignationId = dr["DesignationId"].ToString();
                        vEmployeeInfoVM.SectionId = dr["SectionId"].ToString();
                        vEmployeeInfoVM.DepartmentId = dr["DepartmentId"].ToString();
                        vEmployeeInfoVM.Gender = dr["Gender"].ToString();
                        vEmployeeInfoVM.GradeId = dr["GradeId"].ToString();
                        vEmployeeInfoVM.Religion = dr["Religion"].ToString();
                        vEmployeeInfoVMs.Add(vEmployeeInfoVM);
                    }
                    dr.Close();
                }
                #endregion

                #region Save

                List<BonusProcessVM> VMs = new List<BonusProcessVM>();
                BonusProcessVM vm = new BonusProcessVM();

                #region Assign Data

                foreach (var item in vEmployeeInfoVMs)
                {
                    vm = new BonusProcessVM();
                    vm.EmployeeId = item.Id;

                    if (vBonusStructureVM.IsFixed == true)
                    {
                        vm.Amount = vBonusStructureVM.BonusValue;
                    }
                    else
                    {
                        if (vBonusStructureVM.PortionSalaryType == "Gross")
                        {
                            vm.Amount = item.GrossSalary * vBonusStructureVM.BonusValue / 100;
                        }
                        if (vBonusStructureVM.PortionSalaryType == "Basic")
                        {
                            vm.Amount = (item.BasicSalary * vBonusStructureVM.BonusValue) / 100;
                        }
                    }

                    vm.ProjectId = item.ProjectId;
                    vm.DesignationId = item.DesignationId;
                    vm.DepartmentId = item.DepartmentId;
                    vm.SectionId = item.SectionId;
                    vm.GradeId = item.GradeId;
                    vm.GrossSalary = item.GrossSalary;
                    vm.BasicSalary = item.BasicSalary;

                    vm.BonusNameId = paramVM.BonusNameId;
                    vm.BonusStructureId = paramVM.BonusStructureId;

                    vm.TaxValue = 0;

                    vm.NetPayAmount = vm.Amount - vm.TaxValue;

                    VMs.Add(vm);

                }
                #endregion


                #region Insert Data

                if (VMs != null && VMs.Count > 0)
                {
                    retResults = Insert(paramVM, VMs, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], retResults[1]);
                    }
                }
                #endregion



                #endregion Save
                #endregion Salary_Bonus

                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Bonus Process Successfully.";
                retResults[2] = "Id";
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

        //==================SelectAllForReport=================
        public List<BonusProcessVM> Report(string BonusNameId, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
           , string Orderby, string SheetName = "")
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<BonusProcessVM> vms = new List<BonusProcessVM>();
            BonusProcessVM vm;
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

SELECT 
sbd.Id
,sbd.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department,e.Project, e.JoinDate, e.Section,e.StepName,e.Grade,e.SectionOrder 
,e.BankAccountNo,e.Routing_No,e.Email
, e.Project, sbd.BasicSalary
, sbd.GrossSalary
, sal.HouseRent
, sal.Medical
, sal.TransportAllowance
, sal.Stamp
,sbd.Amount
,ISNULL(sbd.TaxValue,0) TaxValue
,ISNULL(sbd.NetPayAmount,0) NetPayAmount
,round((sbd.NetPayAmount-sal.Stamp),0)NetAmount

,sbd.DesignationId, sbd.DepartmentId, sbd.SectionId, sbd.ProjectId

, bs.name BonusStructureName
, bnm.name BonusType
, bnm.FestivalDate
,bnm.IssueDate
,sbd.FiscalYear
,sbd.FiscalYearDetailId
,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd
,sbd.EffectDate
,sbd.Remarks

From SalaryBonusDetail sbd 

LEFT OUTER JOIN ViewEmployeeInformation e on sbd.EmployeeId=e.id
LEFT OUTER JOIN BonusStructure bs on sbd.BonusStructureId = bs.Id 
LEFT OUTER JOIN BonusName bnm on sbd.BonusNameId = bnm.Id  
LEFT OUTER JOIN grade g on sbd.gradeId = g.id
LEFT OUTER JOIN FiscalYearDetail fyd ON sbd.FiscalYearDetailId = fyd.Id
LEFT OUTER JOIN Designation AS desig ON sbd.DesignationId = desig.Id
LEFT OUTER JOIN DesignationGroup AS desi ON desig.DesignationGroupId = desi.Id
left outer join ( 

SELECT Salary.EmployeeId
,SUM(Salary.Basic)Basic
,SUM(Salary.HouseRent)HouseRent
,SUM(Salary.Medical)Medical
,SUM(Salary.TransportAllowance)TransportAllowance
,SUM(Salary.GrossSalary)GrossSalary
,10 Stamp
FROM(
 SELECT 
ssd.EmployeeId
,ISNULL(CASE WHEN  ssd.SalaryType = 'Basic' THEN ssd.Amount  else 0 END, 0) AS Basic
,ISNULL(CASE WHEN   ssd.SalaryType = 'HouseRent' THEN ssd.Amount else 0 END, 0) AS HouseRent
,ISNULL(CASE WHEN   ssd.SalaryType = 'Medical' THEN ssd.Amount  else 0 END, 0) AS Medical
,ISNULL(CASE WHEN   ssd.SalaryType = 'Conveyance' THEN ssd.Amount  else 0 END, 0) AS TransportAllowance 
,ISNULL(CASE WHEN   ssd.SalaryType = 'Gross' THEN ssd.Amount  else 0 END, 0) AS GrossSalary  
FROM EmployeeSalaryStructureDetail ssd
 WHERE 1=1
and ssd.IncrementDate<=(select PeriodEnd from FiscalYearDetail where Id in (select distinct FiscalYearDetailId from SalaryBonusDetail where BonusNameId = @BonusNameId)) 
and ssd.IsActive=1
) AS Salary
GROUP BY Salary.EmployeeId

 ) sal 
on 
--sbd.FiscalYearDetailId=sal.FiscalYearDetailId 
--and 
sbd.EmployeeId=sal.EmployeeId
Where 1=1 
and  e.IsArchive=0 
--and e.isActive=1 
and sbd.Amount > 0

";
                #region ConditionFields
                if (SheetName == "BonusSheet3")
                {
                    sqlText += "AND e.BankName not in ('Standard Chartered Bank')";
                }
                else if (SheetName == "BonusSheet4")
                {
                    sqlText += "AND e.BankName  in ('Standard Chartered Bank')";

                }
                if (BonusNameId != "0_0")
                    sqlText += " and sbd.BonusNameId=@BonusNameId";

                if (ProjectId != "0_0")
                    sqlText += " and e.ProjectId=@ProjectId";

                if (DepartmentId != "0_0")
                    sqlText += " and e.DepartmentId=@DepartmentId ";

                if (SectionId != "0_0")
                    sqlText += " and e.SectionId=@SectionId ";

                if (DesignationId != "0_0")
                    sqlText += " and e.DesignationId=@DesignationId ";

                if (CodeF != "0_0")
                    sqlText += " and e.Code>= @CodeF";

                if (CodeT != "0_0")
                    sqlText += " and e.Code<= @CodeT";

                if (!string.IsNullOrWhiteSpace(Orderby))
                {

                    if (Orderby == "DCG")
                        sqlText += " ORDER BY e.department, e.code, g.sl";
                    else if (Orderby == "DDC")
                        sqlText += " ORDER BY e.department, e.JoinDate, e.code";
                    else if (Orderby == "DGC")
                        sqlText += " ORDER BY e.department, g.sl, e.code";
                    else if (Orderby == "DGDC")
                        sqlText += " ORDER BY e.department, g.sl, e.JoinDate, e.code";
                    else if (Orderby == "CODE")
                        sqlText += " ORDER BY  SectionOrder,desig.OrderNo,e.code";
                    else if (Orderby.ToLower() == "designation")
                        sqlText += " ORDER BY ISNULL(desig.PriorityLevel,0), e.code";
                }

                #endregion ConditionFields
                #region ConditionValues

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                if (BonusNameId != "0_0")
                    objComm.Parameters.AddWithValue("@BonusNameId", BonusNameId);

                if (ProjectId != "0_0")
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);

                if (DepartmentId != "0_0")
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);

                if (SectionId != "0_0")
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);

                if (DesignationId != "0_0")
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);

                if (CodeF != "0_0")
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);

                if (CodeT != "0_0")
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                #endregion ConditionValues
                #endregion SqlText

                #region SqlExecution

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BonusProcessVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Amount = Convert.ToDecimal(dr["Amount"]);
                    vm.TaxValue = Convert.ToDecimal(dr["TaxValue"]);
                    vm.NetPayAmount = Convert.ToDecimal(dr["NetPayAmount"]);
                    vm.NetAmount = Convert.ToDecimal(dr["NetAmount"]);

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    vm.StepName = dr["StepName"].ToString();
                    vm.SectionOrder = dr["SectionOrder"].ToString();

                    vm.BonusStructureName = dr["BonusStructureName"].ToString();
                    vm.BonusType = dr["BonusType"].ToString();
                    vm.FestivalDate = Ordinary.StringToDate( dr["FestivalDate"].ToString());
                    vm.IssueDate = Ordinary.StringToDate(dr["IssueDate"].ToString());


                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.HouseRent = Convert.ToDecimal(dr["HouseRent"]);
                    vm.Medical = Convert.ToDecimal(dr["Medical"]);
                    vm.TransportAllowance = Convert.ToDecimal(dr["TransportAllowance"]);
                    vm.Stamp = Convert.ToDecimal(dr["Stamp"]);
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();

                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.FiscalYear = Convert.ToInt32(dr["FiscalYear"]);
                    vm.FiscalPeriod = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
                    vm.EffectDate = Ordinary.StringToDate( dr["PeriodEnd"].ToString());
                    vm.BankAccountNo = dr["BankAccountNo"].ToString();
                    vm.Routing_No = dr["Routing_No"].ToString();
                    vm.Email = dr["Email"].ToString();
                    //FiscalYearDetailId
                    //FiscalYear
                    //FiscalPeriod
                    //PeriodStart
                    //PeriodEnd




                    vms.Add(vm);
                }
                dr.Close();
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

            return vms;
        }

        public List<BonusProcessVM> BonusReportSummary(string BonusNameId, string ProjectId, string SectionId)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<BonusProcessVM> vms = new List<BonusProcessVM>();
            BonusProcessVM vm;
            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region sql statement

                #region SqlText

                sqlText = @"
SELECT 
 D.Name Designation 
, D.Id DesignationId 
 ,e.Section
 ,e.Project 
 ,D.Serial

, sum(sbd.Amount)Amount

, sum(sal.BasicSalary)BasicSalary
, sum(sbd.GrossSalary)GrossSalary
, sum(sal.HouseRent)HouseRent
, sum(sal.Medical)Medical
, sum(sal.TransportAllowance)TransportAllowance
, sum(sal.Stamp)Stamp
,sum(ISNULL(sbd.TaxValue,0)) TaxValue
,sum(ISNULL(sbd.NetPayAmount,0)) NetPayAmount
,sum(round((sbd.NetPayAmount-sal.Stamp),0))NetAmount

, e.SectionId
, sbd.ProjectId
, e.SectionOrder

From SalaryBonusDetail sbd 

LEFT OUTER JOIN ViewEmployeeInformation e on sbd.EmployeeId=e.id
LEFT OUTER JOIN BonusStructure bs on sbd.BonusStructureId = bs.Id 
LEFT OUTER JOIN BonusName bnm on sbd.BonusNameId = bnm.Id  
LEFT OUTER JOIN grade g on sbd.gradeId = g.id
LEFT OUTER JOIN FiscalYearDetail fyd ON sbd.FiscalYearDetailId = fyd.Id
LEFT OUTER JOIN DesignationGroup D on e.DesignationGroupId=D.Id
LEFT OUTER JOIN Section s on s.Id = sbd.SectionId

left outer join ( 

SELECT Salary.EmployeeId
,SUM(Salary.Basic)BasicSalary
,SUM(Salary.HouseRent)HouseRent
,SUM(Salary.Medical)Medical
,SUM(Salary.TransportAllowance)TransportAllowance
,SUM(Salary.GrossSalary)GrossSalary
,10 Stamp
FROM(
 SELECT 
ssd.EmployeeId
,ISNULL(CASE WHEN  ssd.SalaryType = 'Basic' THEN ssd.Amount  else 0 END, 0) AS Basic
,ISNULL(CASE WHEN   ssd.SalaryType = 'HouseRent' THEN ssd.Amount else 0 END, 0) AS HouseRent
,ISNULL(CASE WHEN   ssd.SalaryType = 'Medical' THEN ssd.Amount  else 0 END, 0) AS Medical
,ISNULL(CASE WHEN   ssd.SalaryType = 'Conveyance' THEN ssd.Amount  else 0 END, 0) AS TransportAllowance 
,ISNULL(CASE WHEN   ssd.SalaryType = 'Gross' THEN ssd.Amount  else 0 END, 0) AS GrossSalary  
FROM EmployeeSalaryStructureDetail ssd
 WHERE 1=1
and ssd.IncrementDate<=(select PeriodEnd from FiscalYearDetail where Id in (select distinct FiscalYearDetailId from SalaryBonusDetail where BonusNameId = @BonusNameId)) 
and ssd.IsActive=1
) AS Salary
GROUP BY Salary.EmployeeId

) sal 

on 

sbd.EmployeeId=sal.EmployeeId
Where 1=1 
and  e.IsArchive=0 
--and e.isActive=1 
and sbd.Amount > 0

";
                #region ConditionFields
                if (BonusNameId != "0_0")
                    sqlText += " and sbd.BonusNameId=@BonusNameId";

                if (ProjectId != "0_0")
                    sqlText += " and e.ProjectId=@ProjectId";

                if (SectionId != "0_0")
                    sqlText += " and e.SectionId=@SectionId ";


                sqlText += "  group by e.SectionOrder,D.Serial,D.Name ,D.Id,e.Section, e.Project,e.SectionId, sbd.ProjectId  ";

               

                #endregion ConditionFields
                #region ConditionValues

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                if (BonusNameId != "0_0")
                    objComm.Parameters.AddWithValue("@BonusNameId", BonusNameId);

                if (ProjectId != "0_0")
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);

              

                if (SectionId != "0_0")
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);

              
                #endregion ConditionValues
                #endregion SqlText

                #region SqlExecution

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BonusProcessVM();
                   
                    vm.Amount = Convert.ToDecimal(dr["Amount"]);
                    vm.TaxValue = Convert.ToDecimal(dr["TaxValue"]);
                    vm.NetPayAmount = Convert.ToDecimal(dr["NetPayAmount"]);
                    vm.NetAmount = Convert.ToDecimal(dr["NetAmount"]);
                    vm.Designation = dr["Designation"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.SectionOrder = dr["SectionOrder"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.HouseRent = Convert.ToDecimal(dr["HouseRent"]);
                    vm.Medical = Convert.ToDecimal(dr["Medical"]);
                    vm.TransportAllowance = Convert.ToDecimal(dr["TransportAllowance"]);
                    vm.Stamp = Convert.ToDecimal(dr["Stamp"]);
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();

                  

                    vms.Add(vm);
                }
                dr.Close();
                #endregion SqlExecution


                #endregion

                string getRatioText = @"
select distinct dgg.Id DesignationId, SectionId,s.Name Section,s.OrderNo,Ratio
from SectionDesignationRatio sdr left outer join Designation desi 
on sdr.DesignationId = desi.Id left outer join DesignationGroup dgg
on desi.DesignationGroupId = dgg.Id left outer join Section s
on sdr.SectionId = s.Id
";
                DataTable dtRatio = new DataTable();
                DataTable dtFinalRatio = new DataTable();
                DataTable dtEmployees = vms.ToDataTable();
                DataTable dtEmployeesTemp = vms.ToDataTable();

                objComm.CommandText = getRatioText;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(objComm);
                sqlDataAdapter.Fill(dtRatio);

                int rowsCount = dtRatio.Rows.Count;


                for (int index = 0; index < rowsCount; index++)
                {
                    DataRow dataRow = dtRatio.Rows[index];

                    DataRow[] rows = dtEmployees.Select("DesignationId='" +
                                                        dataRow["DesignationId"] + "'");

                    if (rows.Any())
                    {

                        dtEmployeesTemp = dtEmployeesTemp.Select("DesignationId <> '" +
                                                                 dataRow["DesignationId"] + "'").CopyToDataTable();

                        DataTable dtTemp = rows.CopyToDataTable();

                        for (int i = 0; i < dtTemp.Rows.Count; i++)
                        {
                            DataRow dtTempRow = dtTemp.Rows[i];
                            dtTempRow["BasicSalary"] = Convert.ToDecimal(dtTempRow["BasicSalary"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["HouseRent"] = Convert.ToDecimal(dtTempRow["HouseRent"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["Medical"] = Convert.ToDecimal(dtTempRow["Medical"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["TransportAllowance"] = Convert.ToDecimal(dtTempRow["TransportAllowance"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["GrossSalary"] = Convert.ToDecimal(dtTempRow["GrossSalary"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["TaxValue"] = Convert.ToDecimal(dtTempRow["TaxValue"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["NetPayAmount"] = Convert.ToDecimal(dtTempRow["NetPayAmount"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["NetAmount"] = Convert.ToDecimal(dtTempRow["NetAmount"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["Stamp"] = Convert.ToDecimal(dtTempRow["Stamp"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["Amount"] = Convert.ToDecimal(dtTempRow["Amount"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["SectionId"] = dataRow["SectionId"];
                            dtTempRow["Section"] = dataRow["Section"];
                            dtTempRow["SectionOrder"] = dataRow["OrderNo"];

                        }

                        dtFinalRatio.Merge(dtTemp);


                    }


                }


                dtEmployeesTemp.Merge(dtFinalRatio);



                string createTempTable = @"

create table #tempEmployees
(
BasicSalary decimal(18,6),
HouseRent decimal(18,6),
Medical decimal(18,6),
TransportAllowance decimal(18,6),
GrossSalary  decimal(18,6),
TaxValue decimal(18,6),
NetPayAmount decimal(18,6),
NetAmount decimal(18,6),
Stamp decimal(18,6),
Amount  decimal(18,6),
SectionId  varchar(20),
Section varchar(200) ,
SectionOrder int,
ProjectId varchar(20),
--Serial int,
DesignationId varchar(20),
Project varchar(200) ,
Designation varchar(200)

)";
                objComm = new SqlCommand(createTempTable, currConn, transaction);
                int res = objComm.ExecuteNonQuery();

                CommonDAL commonDal = new CommonDAL();


                var view = new DataView(dtEmployeesTemp);

                dtEmployeesTemp = view.ToTable(false,
                    "HouseRent", "BasicSalary",

                    "Medical",
                    "TransportAllowance",
                    "GrossSalary",
                    "TaxValue",
                    "NetPayAmount",
                    "NetAmount",
                    "Stamp",
                    "Amount",
                    "SectionId",
                    "Section",
                    "SectionOrder",
                    "ProjectId",
                    "DesignationId",
                    "Project", "Designation");

                dtEmployeesTemp = Ordinary.DtValueRound(dtEmployeesTemp, new[]{"HouseRent","BasicSalary",
                   
                    "Medical",
                    "TransportAllowance",
                    "GrossSalary",
                    "TaxValue",
                    "NetPayAmount",
                    "NetAmount",
                    "Stamp"
                   });

                var result = commonDal.BulkInsert("#tempEmployees", dtEmployeesTemp, currConn, transaction);


                string getOrderedData =
                    @"
select te.*
--BasicSalary
--HouseRent,
--Medical,
--TransportAllowance,
--GrossSalary ,
--TaxValue,
--NetPayAmount,
--NetAmount,
--Stamp,
--Amount ,
--SectionId  ,
--Section  ,
--SectionOrder ,
--ProjectId ,
--DesignationId ,
--Project ,
--Designation 
,dgg.Serial
, bnm.FestivalDate
,bnm.IssueDate
from #tempEmployees te left outer join
DesignationGroup dgg on te.DesignationId = dgg.Id
LEFT OUTER JOIN BonusName bnm on @bonusNameId = bnm.Id 
order by SectionOrder, dgg.Serial

";


                objComm = new SqlCommand(getOrderedData, currConn, transaction);
                objComm.Parameters.Add("@bonusNameId",BonusNameId);
                sqlDataAdapter = new SqlDataAdapter(objComm);
                dtEmployeesTemp = new DataTable();
                sqlDataAdapter.Fill(dtEmployeesTemp);



                vms = dtEmployeesTemp.ToList<BonusProcessVM>();

                
                transaction.Commit();;
            }
            #region catch


       
            catch (Exception ex)
            {
                transaction.Rollback(); ;

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


        public DataTable Report(BonusProcessVM vm)
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

                #region Condition Maker
                List<string> ListCFields = new List<string>();
                List<string> ListCValues = new List<string>();

                ConditionMaker(vm, ListCFields, ListCValues);

                string[] conditionFields = ListCFields.ToArray();
                string[] conditionValues = ListCValues.ToArray();

                #endregion

                #region SqlText

                sqlText = @"
SELECT
e.Code
,e.EmpName
,e.Designation
,e.Department, e.JoinDate, e.Section, e.Project, ej.Other1, e.Grosssalary, e.Basicsalary
,ej.BankAccountNo
,sbd.Amount
,ISNULL(sbd.TaxValue,0) TaxValue
,(ISNULL(sbd.NetPayAmount,0)-10) NetPayAmount
,sbd.EmployeeId
, bs.name BonusStructureName
, bs.BonusValue
, bnm.name BonusType
,sbd.EffectDate
,sbd.FiscalYear
,fyd.PeriodName
,sbd.Remarks
, (CAST(bs.BonusValue as varchar(10)) +' %') BonusPercent

FROM SalaryBonusDetail sbd 
LEFT OUTER JOIN ViewEmployeeInformation e on sbd.EmployeeId=e.id
LEFT OUTER JOIN BonusStructure bs on sbd.BonusStructureId = bs.Id 
LEFT OUTER JOIN BonusName bnm on sbd.BonusNameId = bnm.Id  
LEFT OUTER JOIN grade g on sbd.gradeId = g.id
LEFT OUTER JOIN FiscalYearDetail fyd ON sbd.FiscalYearDetailId = fyd.Id
LEFT OUTER JOIN EmployeeJob ej on sbd.EmployeeId=ej.EmployeeId
LEFT OUTER JOIN Designation AS desig ON sbd.DesignationId = desig.Id

WHERE 1=1 and  e.IsArchive=0 and e.isActive=1

";
                if (!string.IsNullOrWhiteSpace(vm.BonusStatus))
                {
                    if (vm.BonusStatus.ToLower() == "bonus")
                    {
                        sqlText += "  AND sbd.Amount > 0 ";
                    }
                    else
                    {
                        sqlText += " AND sbd.Amount =0 ";
                    }
                }


                if (vm.Other3List != null && vm.Other3List.Count > 0 && !string.IsNullOrWhiteSpace(vm.Other3List.FirstOrDefault()))
                {
                    string sqlTextCondition = "";
                    sqlText += "  AND ej.other3 IN(";
                    foreach (string item in vm.Other3List)
                    {
                        sqlTextCondition += "'" + item + "',";
                    }
                    sqlTextCondition = sqlTextCondition.Trim(',');
                    sqlText += sqlTextCondition + ")";
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
                if (!string.IsNullOrWhiteSpace(vm.Orderby))
                {

                    if (vm.Orderby == "DCG")
                        sqlText += " ORDER BY e.department, e.code, g.sl";
                    else if (vm.Orderby == "DDC")
                        sqlText += " ORDER BY e.department, e.JoinDate, e.code";
                    else if (vm.Orderby == "DGC")
                        sqlText += " ORDER BY e.department, g.sl, e.code";
                    else if (vm.Orderby == "DGDC")
                        sqlText += " ORDER BY e.department, g.sl, e.JoinDate, e.code";
                    else if (vm.Orderby == "CODE")
                        sqlText += " ORDER BY  e.code";
                    else if (vm.Orderby.ToLower() == "designation")
                        sqlText += " ORDER BY ISNULL(desig.PriorityLevel,0), e.code";
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
                string[] ColumnChange = { "JoinDate", "EffectDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, ColumnChange);


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

        public DataTable ExcelReport(BonusProcessVM vm)
        {
            #region Variables

            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();


                if (CompanyName.ToLower() == "brac" && vm.SheetName == "BonusSheet2")
                {
                    string[] conFields = { "sbd.BonusNameId" };
                    string[] conValues = { vm.BonusNameId };
                    dt = BonusSummeryReport_Brac(vm, conFields, conValues);

                }
                else
                {
                    dt = Report(vm);
                }

                if (CompanyName.ToLower() == "kbl" || CompanyName.ToLower() == "anupam" || CompanyName.ToLower() == "kajol")
                {
                    #region Kazal
                    #region Select Columns

                    string[] shortColumnName = {     "Code"
                                                   , "EmpName"
                                                   , "Designation"
                                                   , "Department"
                                                   , "Other1"
                                                   , "JoinDate"
                                                   , "BankAccountNo"

                                                   , "Grosssalary"
                                                   , "BonusPercent"
                                                   , "Amount"                                                    
                                                   , "TaxValue"                                                  
                                                   , "NetPayAmount" 
                                             
                                                   , "Remarks"                                                   
                                                   , "BonusType"                                                 
                                                                                                                 
                                                 };

                    Type[] columnTypes = {     typeof(String)
                                             , typeof(String)
                                             , typeof(String)
                                             , typeof(String)
                                             , typeof(String)
                                             , typeof(String)
                                             , typeof(String)

                                             , typeof(decimal)                                                                          
                                             , typeof(String)                                                                          
                                             , typeof(decimal)                                                                          
                                             , typeof(decimal)                                                                          
                                             , typeof(decimal) 
 
                                             , typeof(String)                                                                          
                                             , typeof(String)                                                                          
                                         };


                    dt = Ordinary.DtSetColumnsOrder(dt, shortColumnName);
                    dt = Ordinary.DtSelectedColumn(dt, shortColumnName, columnTypes);

                    dt = Ordinary.DtColumnNameChange(dt, "Other1", "Zone/Job Location");

                    #endregion

                    #region Summary
                    if (vm.SheetName == "BonusSheet2")
                    {

                        var newSort = (from row in dt.AsEnumerable()
                                       group row by new
                                       {
                                           Department = row.Field<string>("Department")
                                           ,
                                           BonusType = row.Field<string>("BonusType")



                                       } into grp
                                       select new
                                       {
                                           Department = grp.Key.Department
                                           ,
                                           BonusType = grp.Key.BonusType
                                           ,
                                           NoOfEmployee = grp.Count()
                                           ,
                                           NetPayAmount = grp.Sum(r => r.Field<Decimal>("NetPayAmount"))
                                       }).ToList();
                        dt = Ordinary.ToDataTable(newSort);

                    }

                    #endregion

                    #endregion

                }
                else if (CompanyName.ToLower() == "brac")
                {

                }

                #region Add SL Column

                if (!dt.Columns.Contains("SL"))
                {
                    dt.Columns.Add("SL").SetOrdinal(0);
                    int i = 1;
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["SL"] = i;
                        i++;
                    }
                }


                #endregion

                #region Remove Column


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
            }

            #endregion

            return dt;
        }

        public DataTable ExcelData(BonusProcessVM vm)
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

                #region Condition Maker
                List<string> ListCFields = new List<string>();
                List<string> ListCValues = new List<string>();

                ConditionMaker(vm, ListCFields, ListCValues);

                string[] conditionFields = ListCFields.ToArray();
                string[] conditionValues = ListCValues.ToArray();

                #endregion

                #region SqlText

                sqlText = @"
SELECT
e.Code
,e.EmpName
,sbd.Amount
,ISNULL(sbd.TaxValue,0) TaxValue
,ISNULL(sbd.NetPayAmount,0) NetPayAmount
, bs.name BonusStructureName
, bnm.name BonusType


,e.Designation
,e.Department, e.JoinDate, e.Section, e.Project, ej.Other1, e.Grosssalary, e.Basicsalary
,ej.BankAccountNo
,sbd.Remarks

,sbd.EffectDate
,sbd.FiscalYear
,sbd.FiscalYearDetailId
,fyd.PeriodName
,sbd.BonusNameId
,sbd.BonusStructureId
,sbd.EmployeeId


FROM SalaryBonusDetail sbd 
LEFT OUTER JOIN ViewEmployeeInformation e on sbd.EmployeeId=e.id
LEFT OUTER JOIN BonusStructure bs on sbd.BonusStructureId = bs.Id 
LEFT OUTER JOIN BonusName bnm on sbd.BonusNameId = bnm.Id  
LEFT OUTER JOIN grade g on sbd.gradeId = g.id
LEFT OUTER JOIN FiscalYearDetail fyd ON sbd.FiscalYearDetailId = fyd.Id
LEFT OUTER JOIN EmployeeJob ej on sbd.EmployeeId=ej.EmployeeId
LEFT OUTER JOIN Designation AS desig ON sbd.DesignationId = desig.Id

WHERE 1=1 and  e.IsArchive=0 and e.isActive=1 and sbd.Amount > 0

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
                if (!string.IsNullOrWhiteSpace(vm.Orderby))
                {

                    if (vm.Orderby == "DCG")
                        sqlText += " ORDER BY e.department, e.code, g.sl";
                    else if (vm.Orderby == "DDC")
                        sqlText += " ORDER BY e.department, e.JoinDate, e.code";
                    else if (vm.Orderby == "DGC")
                        sqlText += " ORDER BY e.department, g.sl, e.code";
                    else if (vm.Orderby == "DGDC")
                        sqlText += " ORDER BY e.department, g.sl, e.JoinDate, e.code";
                    else if (vm.Orderby == "CODE")
                        sqlText += " ORDER BY  e.code";
                    else if (vm.Orderby.ToLower() == "designation")
                        sqlText += " ORDER BY ISNULL(desig.PriorityLevel,0), e.code";
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

                string[] ColumnChange = { "JoinDate", "EffectDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, ColumnChange);


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

        private void ConditionMaker(BonusProcessVM vm, List<string> ListCFields, List<string> ListCValues)
        {

            string[] cFields =  { 
 "e.Code>"                                   
,"e.Code<"                                   
,"e.ProjectId"                                   
,"e.DepartmentId"                                   
,"e.SectionId"                                   
,"e.DesignationId"                                   
,"sbd.BonusNameId"                                   
,"sbd.BonusStructureId" 
, "ej.Other1"                                  
, "ej.Other2"                                  
                                };


            string[] cValues =  { 
vm.CodeF
,vm.CodeT
,vm.ProjectId
,vm.DepartmentId
,vm.SectionId
,vm.DesignationId
,vm.BonusNameId
,vm.BonusStructureId                                   
,vm.Other1                                   
,vm.Other2                                   
                                };


            int CoditionCount = 0;

            CoditionCount = cValues.Where(c => !string.IsNullOrWhiteSpace(c)).ToList().Count();


            if (CoditionCount > 0)
            {
                if (cFields != null && cValues != null && cFields.Length == cValues.Length)
                {
                    for (int i = 0; i < cFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(cFields[i]) || string.IsNullOrWhiteSpace(cValues[i]) || cValues[i] == "0")
                        {
                            continue;
                        }

                        ListCFields.Add(cFields[i]);
                        ListCValues.Add(cValues[i]);

                    }
                }
            }

            //////ListCFields.AddRange(cFields.ToList());
            //////ListCValues.AddRange(cValues.ToList());

        }

        public string[] ImportExcelFile(BonusProcessVM paramVM)
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
                #region Save
                string Code = "";

                EmployeeInfoVM vEmployeeInfoVM = new EmployeeInfoVM();
                EmployeeInfoDAL _EmployeeInfoDAL = new EmployeeInfoDAL();
                BonusProcessVM vm = new BonusProcessVM();
                List<BonusProcessVM> VMs = new List<BonusProcessVM>();


                #region Assign Data
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    Code = "";
                    vEmployeeInfoVM = new EmployeeInfoVM();

                    vm = new BonusProcessVM();

                    Code = Convert.ToString(dr["Code"]);

                    string[] cFields = { "Code" };
                    string[] cValues = { Code };

                    vEmployeeInfoVM = _EmployeeInfoDAL.SelectAll(null, cFields, cValues, currConn, transaction).FirstOrDefault();

                    vm.EmployeeId = vEmployeeInfoVM.EmployeeId;
                    vm.ProjectId = vEmployeeInfoVM.ProjectId;
                    vm.DepartmentId = vEmployeeInfoVM.DepartmentId;
                    vm.SectionId = vEmployeeInfoVM.SectionId;
                    vm.DesignationId = vEmployeeInfoVM.DesignationId;
                    vm.GradeId = vEmployeeInfoVM.GradeId;
                    vm.BasicSalary = Convert.ToInt32(dr["Basicsalary"]);
                    vm.GrossSalary = Convert.ToInt32(dr["Grosssalary"]);

                    vm.BonusNameId = Convert.ToString(dr["BonusNameId"]);
                    vm.BonusStructureId = Convert.ToString(dr["BonusStructureId"]);
                    vm.Amount = Convert.ToInt32(dr["Amount"]);
                    vm.Remarks = Convert.ToString(dr["Remarks"]);


                    vm.TaxValue = Convert.ToInt32(dr["TaxValue"]);
                    vm.NetPayAmount = vm.Amount - vm.TaxValue;
                    vm.EffectDate = Convert.ToString(dr["EffectDate"]);
                    vm.IsManual = true;

                    VMs.Add(vm);

                    if (i == 0)
                    {
                        paramVM.FiscalYear = Convert.ToInt32(dr["FiscalYear"]);
                        paramVM.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                        i++;
                    }
                }
                #endregion

                #region Data Insert

                retResults = Insert(paramVM, VMs, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }
                #endregion
                #endregion
                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                //retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
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


        public ResultVM DownloadExcel(BonusProcessVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            ResultVM rVM = new ResultVM();
            string ReportName = "";

            System.Data.DataTable dt = new System.Data.DataTable();

            #endregion

            #region Parmeters
            if (!string.IsNullOrWhiteSpace(vm.MultipleOther3))
            {
                vm.MultipleOther3 = vm.MultipleOther3.Trim(',');
                vm.Other3List = vm.MultipleOther3.Split(',').ToList();
            }
            #endregion

            #region try
            try
            {
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                dt = ExcelReport(vm);

                if (dt.Columns.Contains("BonusType"))
                {
                    ReportName = dt.Rows[0]["BonusType"].ToString();
                    string[] removeColumnName = { "BonusType" };
                    dt = Ordinary.DtDeleteColumns(dt, removeColumnName);
                    rVM.ReportName = ReportName;
                }

                if (dt.Rows.Count == 0)
                {
                    rVM.Status = "Fail";
                    rVM.Message = "Fail~No Data Found";
                    return rVM;
                }


                #region Prepare Excel



                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");


                if ((CompanyName.ToLower() == "kbl" || CompanyName.ToLower() == "anupam" || CompanyName.ToLower() == "kajol") || CompanyName.ToLower()=="tib")
                {
                    #region Report Headers, Rows, Columns

                    string Line1 = "KAZAL BROTHERS LIMITED";
                    string Line2 = "CORPORATE OFFICE: Dr. Nawab Ali Tower, 6th floor, 24 Purana Paltan, Dhaka-1000";
                    string Line3 = "Phone - 9515301, 9515302, Fax - 9515303 web:www.anupameducation.com";


                    if (CompanyName.ToLower() == "anupam")
                    {
                        Line1 = "ANUPAM PRINTERS";
                        Line2 = "Matuail Moghalnagar, Kadamtali Culvert Road, Matuail-1362, Demra, Dhaka";
                        Line3 = "Contact: 01718-298115; 01991-144534";
                    }
                    if (CompanyName.ToLower() == "tib")
                    {
                        Line1 = "Transparency International Bangladesh";
                        Line2 = "";
                        Line3 = "";
                    }
                    string[] conFields = { "ReportType", "ReportId" };
                    string[] conValues = { "BonusSheet", vm.SheetName };
                    List<EnumReportVM> enumReportVMs = new EnumReportDAL().SelectAll(0, conFields, conValues);

                    string Title = enumReportVMs.FirstOrDefault().ReportName + " - " + ReportName;


                    int LeftColumn = 8;

                    if (vm.SheetName == "BonusSheet2")
                    {
                        LeftColumn = 2;
                    }


                    string[] ReportHeaders = new string[] { "", Line1, Line2, Line3, Title };

                    int TableHeadRow = 0;
                    TableHeadRow = ReportHeaders.Length + 2;

                    int RowCount = 0;
                    RowCount = dt.Rows.Count;

                    int ColumnCount = 0;
                    ColumnCount = dt.Columns.Count;

                    int GrandTotalRow = 0;
                    GrandTotalRow = TableHeadRow + RowCount + 1;

                    int InWordsRow = 0;
                    InWordsRow = GrandTotalRow + 2;

                    int SignatureSpaceRow = 0;
                    SignatureSpaceRow = InWordsRow + 1;

                    int SignatureRow = 0;
                    SignatureRow = InWordsRow + 4;
                    #endregion

                    workSheet.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                    ////if (vm.SheetName == "BonusSheet1")
                    ////{
                    #region Format
                    workSheet.Cells["B" + (TableHeadRow + 1) + ":" + Ordinary.Alphabet[(ColumnCount + 1)] + (TableHeadRow + 1 + RowCount + 3)].Style.Numberformat.Format = "#,##0";
                    workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount + 1)] + (TableHeadRow)].Style.Font.Bold = true;
                    workSheet.Cells["A" + (RowCount + TableHeadRow + 1) + ":" + Ordinary.Alphabet[(ColumnCount + 1)] + (RowCount + TableHeadRow + 1)].Style.Font.Bold = true;
                    workSheet.Cells[Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (RowCount + TableHeadRow + 1)].Style.Font.Bold = true;

                    workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    var format = new OfficeOpenXml.ExcelTextFormat();
                    format.Delimiter = '~';
                    format.TextQualifier = '"';
                    format.DataTypes = new[] { eDataTypes.String };


                    for (int i = 0; i < ReportHeaders.Length; i++)
                    {
                        workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Merge = true;
                        workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.Font.Bold = true;
                        workSheet.Cells[i + 1, 1, (i + 1), (ColumnCount)].Style.Font.Size = 14 - i;
                        workSheet.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

                    }

                    workSheet.Row(TableHeadRow).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(TableHeadRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Row(TableHeadRow).Style.WrapText = true;

                    workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

                    #region Grand Total

                    for (int i = LeftColumn + 1; i <= ColumnCount; i++)
                    {
                        workSheet.Cells[GrandTotalRow, i].Formula = "=Sum(" + workSheet.Cells[TableHeadRow + 1, i].Address + ":" + workSheet.Cells[(TableHeadRow + RowCount), i].Address + ")";
                    }

                    #region Total Not Required

                    int ColumnIndex = 0;

                    DataColumnCollection columns = dt.Columns;
                    if (columns.Contains("BonusValue"))
                    {
                        ColumnIndex = dt.Columns["BonusValue"].Ordinal + 1;

                        workSheet.Cells[GrandTotalRow, ColumnIndex].Value = "";

                    }



                    #endregion


                    object sumObject;
                    sumObject = dt.Compute("Sum([NetPayAmount])", string.Empty);

                    decimal NetPayable = Convert.ToDecimal(sumObject);

                    string strNetPayable = NetPayable.ToString("0.##");

                    string NetPayableInWords = Ordinary.ConvertToWords(strNetPayable, true);

                    workSheet.Row(InWordsRow).Style.WrapText = true;
                    workSheet.Row(InWordsRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    workSheet.Cells[InWordsRow, 2, InWordsRow, ColumnCount].Merge = true;
                    workSheet.Cells[InWordsRow, 2, InWordsRow, ColumnCount].Style.Font.Bold = true;
                    workSheet.Cells[InWordsRow, 1].LoadFromText("Net Payable (In Words):");
                    workSheet.Cells[InWordsRow, 2].LoadFromText(NetPayableInWords);


                    workSheet.Cells[TableHeadRow, 1, GrandTotalRow - 1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    #endregion

                    //////workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    if (vm.SheetName == "BonusSheet1")
                    {

                        #region Signature



                        string signatory1Title = "Prepared By";
                        string signatory2Title = "Audited By";
                        string signatory3Title = "Checked By";
                        string signatory4Title = "Authorized By";
                        string signatory5Title = "Approved By";



                        int signatory1Column = 1;
                        int signatory2Column = 3;
                        int signatory3Column = 5;
                        int signatory4Column = 7;
                        int signatory5Column = 9;


                        workSheet.Cells[SignatureRow, signatory1Column].LoadFromText(signatory1Title);
                        workSheet.Cells[SignatureRow, signatory2Column].LoadFromText(signatory2Title);
                        workSheet.Cells[SignatureRow, signatory3Column].LoadFromText(signatory3Title);
                        workSheet.Cells[SignatureRow, signatory4Column].LoadFromText(signatory4Title);
                        workSheet.Cells[SignatureRow, signatory5Column].LoadFromText(signatory5Title);


                        workSheet.Cells[SignatureRow, signatory1Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[SignatureRow, signatory2Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[SignatureRow, signatory3Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[SignatureRow, signatory4Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[SignatureRow, signatory5Column].Style.Border.Top.Style = ExcelBorderStyle.Thin;


                        workSheet.Row(SignatureRow).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Row(SignatureRow).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Row(SignatureRow).Style.WrapText = true;
                        workSheet.Row(SignatureRow).Style.Font.Bold = true;
                        #endregion
                    }
                    #endregion
                    ////}


                }
                else
                {


                    workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
                }



                #endregion

                rVM.excel = excel;

                rVM.Status = "Success";
                rVM.Message = "Successfully~Data Download";


            }
            #endregion

            catch (Exception ex)
            {
                rVM.Exception = ex.Message;
            }
            finally { }

            return rVM;


        }


        public DataTable BonusSummeryReport_Brac(BonusProcessVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
SELECT
e.Department
,e.Section                          AS [Cost Center]
,SUM(sbd.Amount)                    AS [Gross Pay]
,ISNULL(sum(sbd.TaxValue),0)        AS [Income Tax]
,ISNULL(sum(sbd.NetPayAmount),0)    AS [Net Pay]
, bnm.name  + ' (Summary)'          AS BonusType
FROM SalaryBonusDetail sbd 
LEFT OUTER JOIN ViewEmployeeInformation e on sbd.EmployeeId=e.id
LEFT OUTER JOIN BonusStructure bs on sbd.BonusStructureId = bs.Id 
LEFT OUTER JOIN BonusName bnm on sbd.BonusNameId = bnm.Id  
LEFT OUTER JOIN grade g on sbd.gradeId = g.id
LEFT OUTER JOIN FiscalYearDetail fyd ON sbd.FiscalYearDetailId = fyd.Id
LEFT OUTER JOIN EmployeeJob ej on sbd.EmployeeId=ej.EmployeeId
WHERE 1=1 and sbd.Amount > 0
        
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

                sqlText += @" group by e.Department,  e.Section, bnm.name";

                sqlText += @" order by e.Department,  e.Section, bnm.name";

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

                #region Add SL Column
                dt.Columns.Add("SL").SetOrdinal(0);
                int a = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    dr["SL"] = a;
                    a++;
                }
                #endregion

                //string[] ColumnChange = { "JoinDate", "EffectDate" };
                //dt = Ordinary.DtMultiColumnStringToDate(dt, ColumnChange);


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



        #endregion








        ////////////-----------BonusSummery
        ////        SELECT
        ////e.Department,  e.Section, 
        ////sum(sbd.Amount) BonusAmount
        ////,ISNULL(sum(sbd.TaxValue),0) TaxValue
        ////,ISNULL(sum(sbd.NetPayAmount),0) NetPayAmount

        ////FROM SalaryBonusDetail sbd 
        ////LEFT OUTER JOIN ViewEmployeeInformation e on sbd.EmployeeId=e.id
        ////LEFT OUTER JOIN BonusStructure bs on sbd.BonusStructureId = bs.Id 
        ////LEFT OUTER JOIN BonusName bnm on sbd.BonusNameId = bnm.Id  
        ////LEFT OUTER JOIN grade g on sbd.gradeId = g.id
        ////LEFT OUTER JOIN FiscalYearDetail fyd ON sbd.FiscalYearDetailId = fyd.Id
        ////LEFT OUTER JOIN EmployeeJob ej on sbd.EmployeeId=ej.EmployeeId
        ////WHERE 1=1 and  e.IsArchive=0 and e.isActive=1 and sbd.Amount > 0
        ////group by e.Department,  e.Section
        ////order by e.Department,  e.Section
    }
}
