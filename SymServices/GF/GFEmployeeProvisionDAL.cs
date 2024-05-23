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
using SymViewModel.PF;

namespace SymServices.GF
{
    public class GFEmployeeProvisionDAL
    {
        #region Global Variables

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();

        #endregion

        #region Methods

        //==================DropDown=================
        public List<GFEmployeeProvisionVM> DropDown(string tType = "", int branchId = 0)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<GFEmployeeProvisionVM> VMs = new List<GFEmployeeProvisionVM>();
            GFEmployeeProvisionVM vm;

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
----,PolicyName Name
   FROM GFEmployeeProvisions
WHERE  1=1
AND IsArchive=0
---ORDER BY PolicyName
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GFEmployeeProvisionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    //////vm.Name = dr["Name"].ToString();
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

        //==================SelectAll=================
        public List<GFEmployeeProvisionVM> SelectAll(int Id = 0, string[] conditionFields = null,
            string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GFEmployeeProvisionVM> VMs = new List<GFEmployeeProvisionVM>();
            GFEmployeeProvisionVM vm;

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

                sqlText = @"
SELECT
gfep.Id
,gfep.FiscalYearDetailId
,gfep.EmployeeId
,gfep.ProjectId
,gfep.DepartmentId
,gfep.SectionId
,gfep.DesignationId
,gfep.JoinDate
,gfep.GrossAmount
,gfep.BasicAmount
,gfep.ProvisionAmount
,gfep.GFPolicyId
,gfep.MultipicationFactor
,gfep.JobMonth

,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,isnull(gfep.IncrementArrear,0)IncrementArrear

,gfep.Remarks,gfep.IsActive,gfep.IsArchive,gfep.CreatedBy,gfep.CreatedAt,gfep.CreatedFrom,gfep.LastUpdateBy,gfep.LastUpdateAt,gfep.LastUpdateFrom

FROM GFEmployeeProvisions gfep
";
                sqlText += " LEFT OUTER JOIN " + hrmDB +
                           ".[dbo].ViewEmployeeInformation ve ON gfep.EmployeeId=ve.EmployeeId";
                sqlText += " WHERE  1=1 ";
                if (Id > 0)
                {
                    sqlText += @" AND gfep.eId=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) ||
                            string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }

                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                sqlText += " ORDER BY ve.Code ";


                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) ||
                            string.IsNullOrWhiteSpace(conditionValues[j]))
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
                    vm = new GFEmployeeProvisionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.EmployeeId = Convert.ToString(dr["EmployeeId"]);
                    vm.ProjectId = Convert.ToString(dr["ProjectId"]);
                    vm.DepartmentId = Convert.ToString(dr["DepartmentId"]);
                    vm.SectionId = Convert.ToString(dr["SectionId"]);
                    vm.DesignationId = Convert.ToString(dr["DesignationId"]);
                    vm.JoinDate = Convert.ToString(dr["JoinDate"]);
                    vm.GrossAmount = Convert.ToDecimal(dr["GrossAmount"]);
                    vm.BasicAmount = Convert.ToDecimal(dr["BasicAmount"]);
                    vm.GFPolicyId = Convert.ToInt32(dr["GFPolicyId"]);
                    vm.MultipicationFactor = Convert.ToDecimal(dr["MultipicationFactor"]);
                    vm.JobMonth = Convert.ToDecimal(dr["JobMonth"]);

                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();


                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

                    vm.ProvisionAmount = Convert.ToDecimal(dr["ProvisionAmount"]);
                    vm.IncrementArrear = Convert.ToDecimal(dr["IncrementArrear"]);
                    vm.TotalProvisionAmount = vm.ProvisionAmount + vm.IncrementArrear;
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

        //==================Insert =================
        public string[] Insert(List<GFEmployeeProvisionVM> VMs, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null)
        {
            #region Initializ

            string sqlText = "";
            int Next_Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = Next_Id.ToString(); // Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertGFEmployeeProvision"; //Method Name
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
                string FiscalYearDetailStartDate = new FiscalYearDAL().FiscalPeriodStartDate(VMs.FirstOrDefault().FiscalYearDetailId,currConn,transaction);
                Next_Id = _cDal.NextId("GFEmployeeProvisions", currConn, transaction);
                if (VMs != null && VMs.Count() > 0)
                {
                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GFEmployeeProvisions(
Id
,FiscalYearDetailId
,EmployeeId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,JoinDate
,GrossAmount
,BasicAmount
,ProvisionAmount
,GFPolicyId
,MultipicationFactor
,JobMonth
,GFHeaderId
,FiscalYearDetailStartDate
,IsBreakMonth
,IncrementArrear
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@FiscalYearDetailId
,@EmployeeId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@JoinDate
,@GrossAmount
,@BasicAmount
,@ProvisionAmount
,@GFPolicyId
,@MultipicationFactor
,@JobMonth
,@GFHeaderId
,@FiscalYearDetailStartDate
,@IsBreakMonth
,@IncrementArrear


,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";

                    #endregion

                    #region SqlExecution

                    foreach (GFEmployeeProvisionVM vm in VMs)
                    {
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        int A = Convert.ToInt32(Math.Round((vm.ProvisionAmount), 0));
                        cmdInsert.Parameters.AddWithValue("@Id", Next_Id);
                        cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                        cmdInsert.Parameters.AddWithValue("@FiscalYearDetailStartDate", FiscalYearDetailStartDate);
                        cmdInsert.Parameters.AddWithValue("@IsBreakMonth", vm.IsBreakMonth);
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                        cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                        cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                        cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                        cmdInsert.Parameters.AddWithValue("@JoinDate", vm.JoinDate);
                        cmdInsert.Parameters.AddWithValue("@GrossAmount", vm.GrossAmount);
                        cmdInsert.Parameters.AddWithValue("@BasicAmount", vm.BasicAmount);
                        cmdInsert.Parameters.AddWithValue("@ProvisionAmount", Math.Round((vm.ProvisionAmount),0));
                        cmdInsert.Parameters.AddWithValue("@GFPolicyId", vm.GFPolicyId);
                        cmdInsert.Parameters.AddWithValue("@MultipicationFactor", vm.MultipicationFactor);
                        cmdInsert.Parameters.AddWithValue("@JobMonth", vm.JobMonth);
                        cmdInsert.Parameters.AddWithValue("@IncrementArrear", Math.Round((vm.IncrementArrear),0));

                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                        cmdInsert.Parameters.AddWithValue("@GFHeaderId", vm.GFHeaderId);
                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GF Policies.", "");
                        }

                        Next_Id++;
                    }

                    #endregion
                }
                else
                {
                    retResults[1] = "This  GF Policy already used!";
                    throw new ArgumentNullException("Please Input  GF Policy Value", "");
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
                retResults[2] = Next_Id.ToString();

                #endregion SuccessResult
            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

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

        //==================Insert =================
        public string[] Process(GFEmployeeProvisionVM paramVM, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null)
        {
            #region Initializ

            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = Id.ToString(); // Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertGFEmployeeProvision"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ////int transResult = 0;

            DataSet ds = new DataSet();

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

                string hrmDB = _dbsqlConnection.HRMDB;
                string[] conditionFields = { "pfd.ProjectId", "pfd.FiscalYearDetailId" };
                string[] conditionValues = { paramVM.ProjectId, paramVM.FiscalYearDetailId.ToString() };
                var AlreadyPost = SelectFiscalPeriodHeader(conditionFields, conditionValues, currConn, transaction);
                if (AlreadyPost.Any())
                {
                    if (AlreadyPost.FirstOrDefault().Post)
                    {
                        retResults[1] = "GF Employee Provisions Already Posted for this month.";

                        throw new ArgumentNullException("Post GFEmployeeProvisions ", retResults[1]);
                    }
                   
                    
                }
                //check for post


                // header insert
                #region Process

                sqlText += @"
Insert into GFHeader
(   Id
       ,Code
      ,FiscalYearDetailId
      ,ProjectId
      ,ProvisionAmount
      ,Post
      ,Remarks
      ,IsActive
      ,IsArchive
      ,CreatedBy
      ,CreatedAt
      ,CreatedFrom
      ,LastUpdateBy
      ,LastUpdateAt
      ,LastUpdateFrom
)

SELECT DISTINCT 
@Id
,@Code
,FiscalYearDetailId
,ProjectId
,0
,@Post
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@LastUpdateBy
,@LastUpdateAt
,@LastUpdateFrom

FROM (
SELECT DISTINCT FiscalYearDetailId,ProjectId,DepartmentId,SectionId,DesignationId,EmployeeId
, CASE WHEN SalaryType='Basic' THEN SUM(Amount) END AS  Basic 
, CASE WHEN SalaryType='Gross' THEN SUM(Amount) END AS  Gross ";
                
                
                sqlText += @" FROM " + hrmDB + ".dbo.SalaryEarningDetail ";
                sqlText += @" WHERE 1=1 

AND EmployeeStatus not in('left')
AND FiscalYearDetailId=@FiscalYearDetailId 
AND ProjectId=@ProjectId 
  AND " + hrmDB + @".dbo.SalaryEarningDetail.EmployeeId not in(select distinct  EmployeeId 
                from  " + hrmDB + @".dbo.SalaryEmployee
                where FiscalYearDetailId=@FiscalYearDetailId 
                and ProjectId=@ProjectId
                and IsHold=1)
  AND " + hrmDB + @".dbo.SalaryEarningDetail.EmployeeId in(select distinct  EmployeeId 
            from " + hrmDB + @".dbo.EmployeeStructureGroup
where isnull(IsGFApplicable,1)=1)

GROUP BY FiscalYearDetailId,ProjectId,DepartmentId,SectionId,DesignationId,EmployeeId,SalaryType
) AS A
GROUP BY FiscalYearDetailId,ProjectId


";


                string deleteCurrent =
                    @" DELETE FROM GFHeader  WHERE FiscalYearDetailId=@FiscalYearDetailId and ProjectId=@ProjectId";


                SqlCommand cmd = new SqlCommand(deleteCurrent, currConn, transaction);
                cmd.Parameters.AddWithValue("@ProjectId", paramVM.ProjectId);
                cmd.Parameters.AddWithValue("@FiscalYearDetailId", paramVM.FiscalYearDetailId);
                cmd.ExecuteNonQuery();

                int nextId = _cDal.NextId("GFHeader", currConn, transaction);

                string NewCode = new CommonDAL().CodeGenerationPF("GF", "GFContribution",
                    DateTime.Now.ToString(), currConn, transaction);

                string code = NewCode;


                cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.Parameters.AddWithValue("@code", code);
                cmd.Parameters.AddWithValue("@Id", nextId);
                cmd.Parameters.AddWithValue("@ProjectId", paramVM.ProjectId);
                cmd.Parameters.AddWithValue("@FiscalYearDetailId", paramVM.FiscalYearDetailId);
                cmd.Parameters.AddWithValue("@Post", false);

                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@IsArchive", false);
                cmd.Parameters.AddWithValue("@CreatedBy", paramVM.CreatedAt);
                cmd.Parameters.AddWithValue("@CreatedAt", paramVM.CreatedAt);
                cmd.Parameters.AddWithValue("@CreatedFrom", paramVM.CreatedFrom);
                cmd.Parameters.AddWithValue("@LastUpdateBy", "");
                cmd.Parameters.AddWithValue("@LastUpdateAt", "");
                cmd.Parameters.AddWithValue("@LastUpdateFrom", "");
                cmd.Parameters.AddWithValue("@Remarks", "-");

                cmd.ExecuteNonQuery();

                // header id
                string[] result = DetailsInsert(paramVM, hrmDB,nextId.ToString(), currConn, transaction);



                // amount update
                string updateAMount = @"
update GFHeader set ProvisionAmount = Details.ProvisionAmount
from (
select GFHeaderId,sum(ProvisionAmount)ProvisionAmount from GFEmployeeProvisions
where GFHeaderId = @Id
group by GFHeaderId
) as Details
where Details.GFHeaderId = GFHeader.Id

";

                cmd.CommandText = updateAMount;
                //cmd.Parameters.AddWithValue("@Id", nextId);

                cmd.ExecuteNonQuery();


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
                //retResults[2] = vm.Id.ToString();

                #endregion SuccessResult
            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

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

        private string[] DetailsInsert(GFEmployeeProvisionVM paramVM, string hrmDB, string headerId,SqlConnection currConn,
            SqlTransaction transaction)
        {
            string[] retResults = new string[6];
            try
            {


             SettingDAL _settingDal = new SettingDAL();
             //string GFStartFrom = _settingDal.settingValue("GF", "GFStartFrom", currConn, transaction).Trim();
             string BreakMonthCalculate = _settingDal.settingValue("GF", "BreakMonthCalculate", currConn, transaction).Trim();
             string DayWiseArear = _settingDal.settingValue("GF", "DayWiseArear", currConn, transaction).Trim();
             string YearDay = _settingDal.settingValue("GF", "YearDay", currConn, transaction).Trim();

             //FiscalYearDetailVM fiscalYearDetailVM = new FiscalYearDAL().FYPeriodDetail(paramVM.FiscalYearDetailId, currConn, transaction).FirstOrDefault();

             string FiscalYearDetailStartDate = new FiscalYearDAL().FYPeriodDetail(paramVM.FiscalYearDetailId, currConn, transaction).FirstOrDefault().PeriodStart;
             string PreviousFiscalYearDetail = Convert.ToDateTime(Ordinary.StringToDate(FiscalYearDetailStartDate)).AddMonths(-1).ToString("yyyyMM");
             string PreviousFiscalYearDetailId = new FiscalYearDAL().FYPeriodDetailPeriodId(PreviousFiscalYearDetail, currConn, transaction).Id.ToString();




            DataSet ds = new DataSet();
            string sqlText;
            sqlText =
                " delete From GFEmployeeProvisions where FiscalYearDetailId=@FiscalYearDetailId and ProjectId=@ProjectId ";

            sqlText += @"
----declare @FiscalYearDetailId as int
----declare @ProjectId as varchar(100)
----set @FiscalYearDetailId =1077
----set @ProjectId ='1_2'


SELECT DISTINCT FiscalYearDetailId,ProjectId,DepartmentId,SectionId,DesignationId,EmployeeId
,SUM(Basic)BasicAmount,sum(Gross)GrossAmount FROM (
SELECT DISTINCT FiscalYearDetailId,ProjectId,DepartmentId,SectionId,DesignationId,EmployeeId
, CASE WHEN SalaryType='Basic' THEN SUM(Amount) END AS  Basic 
, CASE WHEN SalaryType='Gross' THEN SUM(Amount) END AS  Gross ";
            sqlText += @" FROM " + hrmDB + ".dbo.SalaryEarningDetail ";
            sqlText += @" WHERE 1=1 
AND FiscalYearDetailId=@FiscalYearDetailId 
AND ProjectId=@ProjectId 
AND EmployeeStatus not in('left')

  AND " + hrmDB + @".dbo.SalaryEarningDetail.EmployeeId not in(select distinct  EmployeeId 
                from  " + hrmDB + @".dbo.SalaryEmployee
                where FiscalYearDetailId=@FiscalYearDetailId 
                and ProjectId=@ProjectId
                and IsHold=1)
  AND " + hrmDB + @".dbo.SalaryEarningDetail.EmployeeId in(select distinct  EmployeeId 
            from " + hrmDB + @".dbo.EmployeeStructureGroup
where isnull(IsGFApplicable,1)=1)



GROUP BY FiscalYearDetailId,ProjectId,DepartmentId,SectionId,DesignationId,EmployeeId,SalaryType
) AS A
GROUP BY FiscalYearDetailId,ProjectId,DepartmentId,SectionId,DesignationId,EmployeeId

SELECT EmployeeId,JoinDate,isnull(GFStartFrom,JoinDate)GFStartFrom FROM  " + hrmDB + ".dbo.EmployeeJob where ProjectId=@ProjectId";


            sqlText += @" SELECT * FROM  " + hrmDB + ".dbo.FiscalYearDetail WHERE Id=@FiscalYearDetailId ";


            sqlText += @" SELECT * from GFPolicies

SELECT EmployeeId, sum(ProvisionAmount) TotalProvisionAmount from GFEmployeeProvisions
where ProjectId=@ProjectId
and FiscalYearDetailStartDate<@FiscalYearDetailStartDate
and isnull(IsBreakMonth,1)=0
GROUP BY EmployeeId

SELECT * from GFEmployeeProvisions
where ProjectId=@ProjectId
and FiscalYearDetailId=@PreviousFiscalYearDetailId
--and isnull(IsBreakMonth,1)=0


";

            #endregion SqlText

            #region SqlExecution

            SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
            da.SelectCommand.Transaction = transaction;
            da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", paramVM.FiscalYearDetailId);
            da.SelectCommand.Parameters.AddWithValue("@ProjectId", paramVM.ProjectId);
            da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailStartDate", FiscalYearDetailStartDate);
            da.SelectCommand.Parameters.AddWithValue("@PreviousFiscalYearDetailId", PreviousFiscalYearDetailId);

            da.Fill(ds);

            #endregion SqlExecution

            #endregion

            #region Ready Data

            List<GFEmployeeProvisionVM> VMs = new List<GFEmployeeProvisionVM>();
            GFEmployeeProvisionVM vm = new GFEmployeeProvisionVM();

            DataTable dtSalaryEarningDetail = new DataTable();
            DataTable dtEmployeeJob = new DataTable();
            DataTable dtFiscalYearDetail = new DataTable();
            DataTable dtGFPolicy = new DataTable();
            DataTable dtGFEmployeeProvision = new DataTable();
            DataTable dtGFEmployeeProvisionPreviousMonth = new DataTable();

            if (ds.Tables.Count != 6)
            {
                retResults[1] = "System Error!";
                throw new ArgumentNullException(retResults[1], "");
            }

            dtSalaryEarningDetail = ds.Tables[0];
            dtEmployeeJob = ds.Tables[1];
            dtFiscalYearDetail = ds.Tables[2];
            dtGFPolicy = ds.Tables[3];
            dtGFEmployeeProvision = ds.Tables[4];
            dtGFEmployeeProvisionPreviousMonth = ds.Tables[5];


            #region FiscalYearDetail

            DataRow[] dataRowFiscalYearDetail;
            dataRowFiscalYearDetail = dtFiscalYearDetail.Select("Id='" + paramVM.FiscalYearDetailId + "'");
            if (dataRowFiscalYearDetail == null)
            {
                retResults[1] = "System Error!";
                throw new ArgumentNullException(retResults[1], "");
            }

            string PeriodStart = Convert.ToString(dataRowFiscalYearDetail[0]["PeriodStart"]);
            string PeriodEnd = Convert.ToString(dataRowFiscalYearDetail[0]["PeriodEnd"]);

            #endregion

            #region Orginal
            foreach (DataRow dr in dtSalaryEarningDetail.Rows)
            {
                vm = new GFEmployeeProvisionVM();
                ////////vm.Id = Convert.ToInt32(dr["Id"]);
                vm.FiscalYearDetailId = paramVM.FiscalYearDetailId;
                vm.EmployeeId = Convert.ToString(dr["EmployeeId"]);

                vm.ProjectId = Convert.ToString(dr["ProjectId"]);
                vm.DepartmentId = Convert.ToString(dr["DepartmentId"]);
                vm.SectionId = Convert.ToString(dr["SectionId"]);
                vm.DesignationId = Convert.ToString(dr["DesignationId"]);

                #region Join Date / EmployeeJob

                DataRow[] dataRowEmployeeJob;
                dataRowEmployeeJob = dtEmployeeJob.Select("EmployeeId='" + vm.EmployeeId + "'");
                if (dataRowEmployeeJob == null)
                {
                    retResults[1] = "System Error!";
                    throw new ArgumentNullException(retResults[1], "");
                }

                vm.JoinDate = Convert.ToString(dataRowEmployeeJob[0]["JoinDate"]);
                vm.GFStartFrom = Convert.ToString(dataRowEmployeeJob[0]["GFStartFrom"]);

                #endregion

                vm.GrossAmount = Convert.ToDecimal(dr["GrossAmount"]);
                vm.BasicAmount = Convert.ToDecimal(dr["BasicAmount"]);

                #region GFPolicy
                string GFStart = Convert.ToInt32(vm.GFStartFrom) > Convert.ToInt32(vm.JoinDate) ? vm.GFStartFrom : vm.JoinDate;
                DateTime JoinDateD = Convert.ToDateTime(Ordinary.StringToDate(GFStart));
                DateTime PeriodEndD = Convert.ToDateTime(Ordinary.StringToDate(PeriodEnd));
                int month = ((PeriodEndD.Year - JoinDateD.Year) * 12) + PeriodEndD.Month - JoinDateD.Month;


                vm.IsBreakMonth = false;
                if (BreakMonthCalculate.ToLower() == "y")
                {
                    if (Convert.ToInt32(vm.GFStartFrom) < Convert.ToInt32(vm.JoinDate))
                    {
                        int JoinDate = Convert.ToInt32(Convert.ToDateTime(Ordinary.StringToDate(vm.JoinDate)).ToString("dd"));
                        if (month == 0 && JoinDate != 1)
                        {
                            vm.IsBreakMonth = true;
                        }
                        else if (month != 0 && JoinDate != 1)
                        {
                            month = month - 1;
                        }
                    }
                }
                int year = month / 12;

                DataRow[] dataRowGFPolicy;
                dataRowGFPolicy =
                    dtGFPolicy.Select("JobDurationYearFrom<='" + year + "' AND JobDurationYearTo>='" + year + "'");
                if (dataRowGFPolicy == null || dataRowGFPolicy.Count() == 0)
                {
                    continue;
                }

                int GFPolicyId = Convert.ToInt32(dataRowGFPolicy[0]["Id"]);
                decimal MultipicationFactor = Convert.ToDecimal(dataRowGFPolicy[0]["MultipicationFactor"]);

                #endregion

                #region GFEmployeeProvision

                DataRow[] dataRowGFEmployeeProvision;
                dataRowGFEmployeeProvision = dtGFEmployeeProvision.Select("EmployeeId='" + vm.EmployeeId + "'");

                decimal TotalProvisionAmount = 0;
                if (dataRowGFEmployeeProvision.Any())
                {
                    //TotalProvisionAmount = Convert.ToDecimal(dataRowGFEmployeeProvision[0]["TotalProvisionAmount"]) + vm.BasicAmount / 12;
                    TotalProvisionAmount = Convert.ToDecimal(dataRowGFEmployeeProvision[0]["TotalProvisionAmount"]);
                }
                DataRow[] dataRowdtGFEmployeeProvisionPreviousMonth;
                dataRowdtGFEmployeeProvisionPreviousMonth = dtGFEmployeeProvisionPreviousMonth.Select("EmployeeId='" + vm.EmployeeId + "'");
                decimal PreviousMonthBasic = 0;
                if (dataRowdtGFEmployeeProvisionPreviousMonth.Any())
                {
                    //TotalProvisionAmount = Convert.ToDecimal(dataRowGFEmployeeProvision[0]["TotalProvisionAmount"]) + vm.BasicAmount / 12;
                    PreviousMonthBasic = Convert.ToDecimal(dataRowdtGFEmployeeProvisionPreviousMonth[0]["BasicAmount"]);
                }
                decimal BasicDiffrence = 0;
                if (vm.EmployeeId == "1_182")
                {
                    //GFStartFrom = "20210501";
                }
                decimal FractionValue = 0;
                vm.IncrementArrear = 0;
                int daysinThismonth = DateTime.DaysInMonth(Convert.ToInt32(JoinDateD.ToString("yyyy")), Convert.ToInt32(JoinDateD.ToString("MM")));
                TimeSpan days = (PeriodEndD - JoinDateD);
                int serviceDay = days.Days;

                int dayOfMonth = Convert.ToInt32(JoinDateD.ToString("dd"));
                int FractionDays = (daysinThismonth - dayOfMonth) + 1;

                if (month > 0)
                {

                        if (vm.BasicAmount != PreviousMonthBasic && month > 1)
                        {

                            BasicDiffrence = vm.BasicAmount - PreviousMonthBasic;
                            if (DayWiseArear.ToLower() == "n")
                            {
                                
                                if (dayOfMonth == 1)
                                {
                                    FractionDays = 0;
                                }
                                else if (PreviousMonthBasic == 0)
                                {
                                    FractionDays = 0;

                                }
                                FractionValue = BasicDiffrence / 12 / daysinThismonth * FractionDays;
                                vm.IncrementArrear = FractionValue + (BasicDiffrence / 12 * month);
                                vm.JobMonth = month;
                            }
                            else
                            {
                                decimal perdayBasicFactorB = BasicDiffrence / Convert.ToDecimal(YearDay);
                                vm.IncrementArrear = perdayBasicFactorB * serviceDay;
                                vm.JobMonth = serviceDay;
                            }
                        }
                   
                }
                //if (month == 0)
                if (false)
                {
                    vm.ProvisionAmount = vm.BasicAmount / 12 / daysinThismonth * FractionDays;
                }
                else
                {
                    vm.ProvisionAmount = vm.BasicAmount / 12;

                }
                decimal TotalCurrentAmount = (vm.BasicAmount / 12) * MultipicationFactor * month;

                //vm.ProvisionAmount = TotalCurrentAmount - TotalProvisionAmount + vm.BasicAmount / 12;

                #endregion


                vm.GFPolicyId = GFPolicyId;
                vm.MultipicationFactor = MultipicationFactor;               


                //////////vm.Remarks = dr["Remarks"].ToString();
                vm.CreatedAt = paramVM.CreatedAt;
                vm.CreatedBy = paramVM.CreatedBy;
                vm.CreatedFrom = paramVM.CreatedFrom;
                vm.GFHeaderId = headerId;
                VMs.Add(vm);
            }
            #endregion Orginal


            if (VMs.Any())
            {
                retResults = Insert(VMs, currConn, transaction);

                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException(retResults[1], "");
                }
            }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return retResults;
        }

        #endregion


        public List<GFHeaderVM> SelectFiscalPeriodHeader(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GFHeaderVM> VMs = new List<GFHeaderVM>();
            GFHeaderVM vm;
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
 distinct pfd.Id,pfd.Code,pfd.FiscalYearDetailId,pfd.Post 
,p.Name ProjectName
,p.Id ProjectId
,fyd.PeriodName
,fyd.PeriodStart
,sum(isnull( d.ProvisionAmount,0) )ProvisionAmount
,sum(isnull( d.IncrementArrear,0) )IncrementArrear
,sum(isnull( d.ProvisionAmount,0)+isnull( d.IncrementArrear,0)  )TotalProvisionAmount
FROM GFEmployeeProvisions d
left outer join GFHeader pfd on d.GFHeaderId=pfd.Id


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
                sqlText += @"
                        group by pfd.Id,pfd.Code,pfd.FiscalYearDetailId,pfd.Post ,p.Name  
                        ,p.Id  
                        ,fyd.PeriodName
                        ,fyd.PeriodStart
                        ORDER BY fyd.PeriodStart DESC";

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
                    vm = new GFHeaderVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYearDetailId = Convert.ToString(dr["FiscalYearDetailId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.ProjectName = dr["ProjectName"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.FiscalPeriod = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.ProvisionAmount = Convert.ToDecimal(dr["ProvisionAmount"].ToString());
                    vm.IncrementArrear = Convert.ToDecimal(dr["IncrementArrear"].ToString());
                    vm.TotalProvisionAmount = Convert.ToDecimal(dr["TotalProvisionAmount"].ToString());
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

        public string[] PostHeader(GFHeaderVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                    sqlText = "update GFHeader set";
                    sqlText += "  Post=@Post";

                    sqlText += @" where Id=@Id

update GFEmployeeProvisions set Post=@Post where GFHeaderId=@Id

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
                    throw new ArgumentNullException("Post GFDetails", "Could not found any item.");
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

        public DataTable GFEmployeeProvisionsReport(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
Select  
h.Id PFHeaderId
,h.Code PFHeaderCode
,fd.PeriodName
,d.GFHeaderId
,e.Project,e.Department,e.Section,e.Code,e.EmpName,e.Designation,e.JoinDate
,d.ProvisionAmount EmployeePFValue
,d.EmployeeId
,d.BasicAmount BasicSalary
,d.MultipicationFactor
,d.JobMonth
,h.Post
,isnull(d.IncrementArrear,0)IncrementArrear
,isnull((isnull(d.IncrementArrear,0)+isnull(d.ProvisionAmount,0)),0)Total
from GFEmployeeProvisions D
left outer join GFHeader H on h.Id=D.GFHeaderId


";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation e ON D.EmployeeId=e.EmployeeId";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].[FiscalYearDetail] fd ON h.FiscalYearDetailId=fd.Id";
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

                sqlText += @" order by e.Code ";

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


        public DataTable GFEmployersProvisionsReport(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
ts.GFHeaderId
,PeriodName
,Project
,Section
,SectionOrderNo
,Designation
,DesignationOrderNo
,ts.BasicAmount
,ts.ProvisionAmount
,ts.IncrementArrear
,ts.TotalAmount
  from (
select distinct ts.GFHeaderId
,fs.PeriodName
,p.Name Project
,st.Name Section
,st.OrderNo SectionOrderNo
,dgg.Name Designation
,dgg.Serial DesignationOrderNo
,count(ts.employeeid)TotalEmployee
,sum(isnull(ts.BasicAmount,0))BasicAmount
,sum(isnull(ts.ProvisionAmount,0))ProvisionAmount
,sum(isnull(ts.IncrementArrear,0))IncrementArrear
,sum(isnull(ts.ProvisionAmount,0)+isnull(ts.IncrementArrear,0))TotalAmount
,0 EmployeeContribution
,0 EmployerContribution
from GFEmployeeProvisions ts

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
group by  ts.GFHeaderId
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
    }
}