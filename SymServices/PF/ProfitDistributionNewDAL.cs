using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.PF;

namespace SymServices.PF
{
    public class ProfitDistributionNewDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion


        //==================SelectAll=================
        public List<ProfitDistributionNewVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProfitDistributionNewVM> VMs = new List<ProfitDistributionNewVM>();
            ProfitDistributionNewVM vm;
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
 pd.Id
,fydFrom.PeriodName PeriodNameFrom

,pd.Id
,ve.Code
,ve.EmpName
,pd.PreDistributionFundId
,pd.EmployeeId
,pd.DistributionDate
,pd.FiscalYearDetailId
,pd.EmployeeContribution
,pd.EmployerContribution
,pd.EmployeeProfit
,pd.EmployerProfit
,pd.MultiplicationFactor
,pd.EmployeeProfitDistribution
,pd.EmployeerProfitDistribution
,pd.TotalProfit
,ISNULL(pd.IsPaid,0) IsPaid
,pd.Post
,pd.Remarks
,pd.IsActive
,pd.IsArchive
,pd.CreatedBy
,pd.CreatedAt
,pd.CreatedFrom
,pd.LastUpdateBy
,pd.LastUpdateAt
,pd.LastUpdateFrom

FROM ProfitDistributionNew pd
";
                sqlText = sqlText + @" LEFT OUTER JOIN " + hrmDB + ".[dbo].FiscalYearDetail fydFrom ON pd.FiscalYearDetailId=fydFrom.Id";
                sqlText = sqlText + @" LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON ve.EmployeeId=pd.EmployeeId";

                sqlText = sqlText + @" WHERE  1=1 AND pd.IsArchive = 0";


                if (Id > 0)
                {
                    sqlText += @" and pd.Id=@Id";
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
                    vm = new ProfitDistributionNewVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.PeriodNameFrom = dr["PeriodNameFrom"].ToString();
                    vm.EmployeeCode = dr["Code"].ToString();
                    vm.EmployeeName = dr["EmpName"].ToString();

                    //vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    //vm.PFDetailFiscalYearDetailIds = dr["PFDetailFiscalYearDetailIds"].ToString();
                    //vm.PreDistributionFundIds = dr["PreDistributionFundIds"].ToString();
                    //vm.DistributionDate = Ordinary.StringToDate(dr["DistributionDate"].ToString());
                    //vm.TotalEmployeeContribution = Convert.ToDecimal(dr["TotalEmployeeContribution"]);
                    //vm.TotalEmployerContribution = Convert.ToDecimal(dr["TotalEmployerContribution"]);
                    //vm.TotalProfit = Convert.ToDecimal(dr["TotalProfit"]);

                    //vm.FiscalYearDetailIdTo = Convert.ToInt32(dr["FiscalYearDetailIdTo"]);
                    //vm.TotalExpense = Convert.ToDecimal(dr["TotalExpense"]);
                    //vm.AvailableDistributionAmount = Convert.ToDecimal(dr["AvailableDistributionAmount"]);

                    //vm.TotalWeightedContribution = Convert.ToDecimal(dr["TotalWeightedContribution"]);
                    //vm.MultiplicationFactor = Convert.ToDecimal(dr["MultiplicationFactor"]);
                    vm.TotalProfit = Convert.ToDecimal(dr["TotalProfit"]);

                    vm.  PreDistributionFundId = Convert.ToString(dr["PreDistributionFundId"]);
                    vm. EmployeeId= Convert.ToString(dr["PreDistributionFundId"]);
                    vm.DistributionDate = Ordinary.StringToDate(dr["DistributionDate"].ToString());
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    vm.EmployerContribution = Convert.ToDecimal(dr["EmployerContribution"]);
                    vm.EmployeeProfit = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfit = Convert.ToDecimal(dr["EmployerProfit"]);
                    vm.MultiplicationFactor = Convert.ToDecimal(dr["MultiplicationFactor"]);
                    vm.EmployeeProfitDistribution = Convert.ToDecimal(dr["EmployeeProfitDistribution"]);
                    vm.EmployeerProfitDistribution = Convert.ToDecimal(dr["EmployeerProfitDistribution"]);


                    vm.IsPaid = Convert.ToBoolean(dr["IsPaid"]);
                  //  vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);
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


        public ResultVM Process(ProfitDistributionNewVM vm,  SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProfitDistributionNewVM> VMs = new List<ProfitDistributionNewVM>();

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

                sqlText = @"
select EmployeeId
, sum(EmployeeContribution)EmployeeContribution
, sum(EmployerContribution)EmployerContribution
, sum(EmployeeProfit)EmployeeProfit
, sum(EmployerProfit)EmployerProfit 
, (sum(EmployeeContribution)+sum(EmployerContribution)+sum(EmployeeProfit)+sum(EmployerProfit)) TotalValue
from (

select EmployeeId, EmployeeContribution, EmployerContribution, EmployeeProfit,EmployerProfit
from EmployeePFOpeinig

union all 

select EmployeeId, EmployeeContribution, EmployerContribution, EmployeeProfit,EmployerProfit
from EmployeeBreakMonthPF

union all 

select  EmployeeId, EmployeePFValue EmployeeContribution, EmployeerPFValue,0 EmployeeProfit, 0 EmployerProfit
from PFDetails

union all 

select  EmployeeId, 0 EmployeeContribution, 0 EmployerContribution,EmployeeProfitDistribution EmployeeProfit,EmployeerProfitDistribution EmployerProfit
from ProfitDistributionNew
)
EmployeeProfits
group by EmployeeId

select  sum(EmployeeContribution)EmployeeContribution
, sum(EmployerContribution)EmployerContribution
, sum(EmployeeProfit)EmployeeProfit
, sum(EmployerProfit)EmployerProfit 
, (sum(EmployeeContribution)+sum(EmployerContribution)+sum(EmployeeProfit)+sum(EmployerProfit)) TotalValue
from (

select EmployeeId, EmployeeContribution, EmployerContribution, EmployeeProfit,EmployerProfit
from EmployeePFOpeinig

union all 

select EmployeeId, EmployeeContribution, EmployerContribution, EmployeeProfit,EmployerProfit
from EmployeeBreakMonthPF

union all 

select  EmployeeId, EmployeePFValue EmployeeContribution, EmployeerPFValue,0 EmployeeProfit, 0EmployerProfit
from PFDetails

union all 

select  EmployeeId, 0 EmployeeContribution, 0 EmployerContribution,EmployeeProfitDistribution EmployeeProfit,EmployeerProfitDistribution EmployerProfit
from ProfitDistributionNew
)
EmployeeProfits

";

                string checkExistData =
                    "Select Count(ID) from ProfitDistributionNew where PreDistributionFundId=@PreDistributionFundId";

                SqlCommand cmd = new SqlCommand(checkExistData, currConn, transaction);
                cmd.Parameters.AddWithValue("@PreDistributionFundId", vm.PreDistributionFundId);
                int count = (int)cmd.ExecuteScalar();

                if (count != 0)
                {
                    throw new Exception("Fund has been already distributed");

                }

                cmd.CommandText = sqlText;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                int nextId = _cDal.NextId("ProfitDistributionNew", currConn, transaction);

                DataSet dtEmployeeDetails = new DataSet();
                adapter.Fill(dtEmployeeDetails);

                PreDistributionFundDAL preDistributionFundDAL = new PreDistributionFundDAL();
                PreDistributionFundVM preDistributionFund = preDistributionFundDAL
                    .SelectAll(Convert.ToInt32(vm.PreDistributionFundId)).FirstOrDefault();

                DataTable dtFinalDistribution = new DataTable();

                dtFinalDistribution.Columns.Add("Id");
                dtFinalDistribution.Columns.Add("EmployeeId");

                dtFinalDistribution.Columns.Add("PreDistributionFundId");//
                dtFinalDistribution.Columns.Add("DistributionDate");//
                dtFinalDistribution.Columns.Add("FiscalYearDetailId");//
                dtFinalDistribution.Columns.Add("EmployeeContribution");//
                dtFinalDistribution.Columns.Add("EmployerContribution");//
                dtFinalDistribution.Columns.Add("EmployeeProfit");//
                dtFinalDistribution.Columns.Add("EmployerProfit");//
                dtFinalDistribution.Columns.Add("MultiplicationFactor");//
                dtFinalDistribution.Columns.Add("EmployeeProfitDistribution");//
                dtFinalDistribution.Columns.Add("EmployeerProfitDistribution");//
                dtFinalDistribution.Columns.Add("TotalProfit");//
                dtFinalDistribution.Columns.Add(new DataColumn() { ColumnName = "Post" ,DataType = typeof(bool)});//
                dtFinalDistribution.Columns.Add(new DataColumn() { ColumnName = "IsPaid", DataType = typeof(bool) });//
                dtFinalDistribution.Columns.Add("Remarks");//
                dtFinalDistribution.Columns.Add(new DataColumn() { ColumnName = "IsActive", DataType = typeof(bool) });//
                dtFinalDistribution.Columns.Add(new DataColumn() { ColumnName = "IsArchive", DataType = typeof(bool) });//

                //dtFinalDistribution.Columns.Add("IsActive");//
                //dtFinalDistribution.Columns.Add("IsArchive");//
                dtFinalDistribution.Columns.Add("CreatedBy");//
                dtFinalDistribution.Columns.Add("CreatedAt");//
                dtFinalDistribution.Columns.Add("CreatedFrom");//
                dtFinalDistribution.Columns.Add("LastUpdateBy");//
                dtFinalDistribution.Columns.Add("LastUpdateAt");//
                dtFinalDistribution.Columns.Add("LastUpdateFrom");//


                foreach (DataRow dataRow in dtEmployeeDetails.Tables[0].Rows)
                {
                    decimal multiplicationFactor = Convert.ToDecimal(preDistributionFund.TotalValue) /
                                        Convert.ToDecimal(dtEmployeeDetails.Tables[1].Rows[0]["TotalValue"]);

                    decimal totalProfit = Convert.ToDecimal(dataRow["TotalValue"]) * Convert.ToDecimal(preDistributionFund.TotalValue) /
                                                   Convert.ToDecimal(dtEmployeeDetails.Tables[1].Rows[0]["TotalValue"]);

                    dtFinalDistribution.Rows.Add(nextId,dataRow["EmployeeId"], vm.PreDistributionFundId, Ordinary.DateToString(vm.DistributionDate), "0",
                        dataRow["EmployeeContribution"], dataRow["EmployerContribution"], dataRow["EmployeeProfit"],
                        dataRow["EmployerProfit"],
                        multiplicationFactor, totalProfit / 2, totalProfit / 2, totalProfit, true, false, "-", true, false, "", "", "",
                        "", "","");

                    nextId++;
                }

                // update fiscal year detail Id



                string[] result = _cDal.BulkInsert("ProfitDistributionNew", dtFinalDistribution, currConn, transaction);

                string fiscalYearUpdate = @"
Update ProfitDistributionNew set FiscalYearDetailId = FiscalYearDetail.Id
from   " + _dbsqlConnection.HRMDB + @" .dbo.FiscalYearDetail
where Format(cast(DistributionDate as datetime),'MMM-yy') = FiscalYearDetail.PeriodName";

                cmd.CommandText = fiscalYearUpdate;
                cmd.ExecuteNonQuery();

                string PreDistributionFundText = @"update PreDistributionFunds set IsDistribute = 1
                where Id = @Id";
                cmd.CommandText = PreDistributionFundText;
                cmd.Parameters.Add("@Id", vm.PreDistributionFundId);
                cmd.ExecuteNonQuery();
   

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                ResultVM resultVm = new ResultVM
                {
                    Status = "Success",
                    Message = "Distribution Successful"
                };

                return resultVm;

            }
            #region catch
           
            catch (Exception ex)
            {
                throw ex;
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

        }

    }
}