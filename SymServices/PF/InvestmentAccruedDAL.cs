using SymOrdinary;
using SymServices.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.PF
{
   public class InvestmentAccruedDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        public List<InvestmentAccruedVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<InvestmentAccruedVM> VMs = new List<InvestmentAccruedVM>();
            InvestmentAccruedVM vm;
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
                sqlText = @"
SELECT
 I.Id
,I.InvestmentNameId
,I.FiscalYearDetailId
,FD.PeriodName
,I.TransactionDate
,I.ReferenceNo
,I.InvestmentValue
,I.AccruedMonth
,I.InterestRate
,I.AccruedInterest
,I.AitInterest
,I.NetInterest
,I.Post
,I.CreatedBy
,I.CreatedAt
,I.CreatedFrom

from InvestmentAccrued I
left outer join   " + _dbsqlConnection.HRMDB + @".dbo.FiscalYearDetail fd on i.FiscalYearDetailId=fd.Id

WHERE  1=1 

";

                if (Id > 0)
                {
                    sqlText += @" and I.Id=@Id";
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

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new InvestmentAccruedVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.InvestmentNameId =  dr["InvestmentNameId"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
                    vm.TransactionDate =Ordinary.StringToDate( dr["TransactionDate"].ToString());
                    vm.ReferenceNo = dr["ReferenceNo"].ToString();
                    vm.InvestmentValue = Convert.ToDecimal(dr["InvestmentValue"].ToString());
                    vm.AccruedMonth = Convert.ToDecimal(dr["AccruedMonth"].ToString());
                    vm.InterestRate = Convert.ToDecimal(dr["InterestRate"].ToString());
                    vm.AccruedInterest = Convert.ToDecimal(dr["AccruedInterest"].ToString());
                    vm.AitInterest = Convert.ToDecimal(dr["AitInterest"].ToString());
                    vm.NetInterest = Convert.ToDecimal(dr["NetInterest"].ToString());
                    vm.Post =Convert.ToBoolean( dr["Post"].ToString());

                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();

 
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

        public string[] Process(InvestmentNameVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            int countId = 0;
            decimal AitRate = 0;
            decimal BeforeInterest = 0;

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToInvestmentName"); }
                #endregion open connection and transaction

                #region Exist Check
                

                CommonDAL _cDal = new CommonDAL();

               

                #endregion
                InvestmentNameDAL invNameDAL = new InvestmentNameDAL();
                DataSet ds = new DataSet(); 
                if (vm != null)
                {


                    vm.InvestmentAccrued.Id = _cDal.NextId("InvestmentAccrued", currConn, transaction);

                    var investmentName = invNameDAL.SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                    var InvestmentAccruedFYD = new FiscalYearDAL().SelectAll_FiscalYearDetailByDate(Ordinary.DateToString(vm.InvestmentAccrued.TransactionDate), currConn, transaction);
                    var InvestmentNameFYD = new FiscalYearDAL().SelectAll_FiscalYearDetailByDate(Ordinary.DateToString(investmentName.InvestmentDate), currConn, transaction);

                    DateTime TransactionDate = Convert.ToDateTime(Ordinary.StringToDate(vm.InvestmentAccrued.TransactionDate));
                    DateTime InvestmentStart = Convert.ToDateTime(Ordinary.StringToDate(investmentName.InvestmentDate));
                    
                    DateTime AccruedFYD = Convert.ToDateTime(Ordinary.StringToDate(InvestmentAccruedFYD.PeriodStart.ToString()));
                    DateTime NameFYD = Convert.ToDateTime(Ordinary.StringToDate(InvestmentNameFYD.PeriodStart));
                    int MonDiff = ((AccruedFYD.Year - NameFYD.Year) * 12) + AccruedFYD.Month - NameFYD.Month;
                    vm.InvestmentAccrued.FiscalYearDetailId = new FiscalYearDAL().SelectAll_FiscalYearDetailByDate(Ordinary.DateToString(vm.InvestmentAccrued.TransactionDate), currConn, transaction).Id.ToString();
                   
                    
                    vm.InvestmentAccrued.AccruedMonth = MonDiff;

                    #region Posted
                    sqlText = @"select count(Id) from InvestmentAccrued  where InvestmentNameId = @InvestmentNameId and FiscalYearDetailId=@FiscalYearDetailId
                   and post=1 ";
                    SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                    cmdCodeExist.Transaction = transaction;
                    cmdCodeExist.Parameters.AddWithValue("@InvestmentNameId", vm.Id);
                    cmdCodeExist.Parameters.AddWithValue("@FiscalYearDetailId", vm.InvestmentAccrued.FiscalYearDetailId);

                    countId = (int)cmdCodeExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        retResults[1] = "This Accured Already Posted for this month ";
                        throw new ArgumentNullException("", retResults[1]);
                    }
                    #endregion Posted
                    #region deleted

                    string deleteDetails = "delete from InvestmentAccrued where InvestmentNameId = @InvestmentNameId and FiscalYearDetailId=@FiscalYearDetailId";
                    SqlCommand cmdUpdate = new SqlCommand(deleteDetails, currConn,transaction);

                    cmdUpdate.Parameters.AddWithValue("@InvestmentNameId", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.InvestmentAccrued.FiscalYearDetailId);
                    cmdUpdate.ExecuteNonQuery();
                    #endregion deleted

                    string SelectSQL = @"

select distinct 
InvestmentId, InvestmentNameId, sum(investmentValue) InvestmentValue
from (
select   id InvestmentId, InvestmentNameId, InvestmentValue  from Investments where InvestmentNameId=@InvestmentNameId
union all
select   InvestmentId, @InvestmentNameId InvestmentNameId,investmentValue  from InvestmentRenew where
InvestmentId in(
select id from Investments where InvestmentNameId=@InvestmentNameId
) )as inv
group by InvestmentId, InvestmentNameId

select * from InvestmentNameDetails
where  InvestmentNameId=@InvestmentNameId
and @MonDiff between FromMonth and ToMonth

Select ISNULL(SUM(AccruedInterest),0) AccruedInterest from InvestmentAccrued where InvestmentNameId=@InvestmentNameId

Select ISNULL(AitInterest,0) AitInterest from InvestmentNames where Id=@InvestmentNameId

";

                    SqlDataAdapter da = new SqlDataAdapter(SelectSQL, currConn);
                    da.SelectCommand.Transaction = transaction;
                    da.SelectCommand.Parameters.AddWithValue("@InvestmentNameId", vm.Id);
                    da.SelectCommand.Parameters.AddWithValue("@MonDiff", MonDiff);

                    da.Fill(ds);
                       
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        vm.InvestmentAccrued.InvestmentValue = Convert.ToDecimal(item["InvestmentValue"].ToString());
                    }
                    foreach (DataRow item1 in ds.Tables[1].Rows)
                    {
                        vm.InvestmentAccrued.InterestRate = Convert.ToDecimal(item1["InterestRate"].ToString());
                    }

                    foreach (DataRow item in ds.Tables[2].Rows)
                    {
                        BeforeInterest = Convert.ToDecimal(item["AccruedInterest"].ToString());
                    }
                    foreach (DataRow item in ds.Tables[3].Rows)
                    {
                       AitRate = Convert.ToDecimal(item["AitInterest"].ToString());
                    }
                    if (MonDiff == 0)
                    {
                        decimal dateDiff = (TransactionDate - InvestmentStart).Days + 1;
                        decimal transactionMonthDay = TransactionDate.Day;
                        decimal fraction = dateDiff / transactionMonthDay;
                        vm.InvestmentAccrued.AccruedInterest = ((vm.InvestmentAccrued.InvestmentValue * vm.InvestmentAccrued.InterestRate / 100) / 12) * (dateDiff/transactionMonthDay );

                        vm.InvestmentAccrued.AitInterest = (vm.InvestmentAccrued.AccruedInterest / 100) * 10;
                        vm.InvestmentAccrued.NetInterest = vm.InvestmentAccrued.AccruedInterest - vm.InvestmentAccrued.AitInterest;

                    }
                    else
                    {
                        vm.InvestmentAccrued.AccruedInterest = (((vm.InvestmentAccrued.InvestmentValue * vm.InvestmentAccrued.InterestRate / 100) / 12) * vm.InvestmentAccrued.AccruedMonth) - BeforeInterest;

                        vm.InvestmentAccrued.AitInterest = (vm.InvestmentAccrued.AccruedInterest / 100) * AitRate;
                        vm.InvestmentAccrued.NetInterest = vm.InvestmentAccrued.AccruedInterest - vm.InvestmentAccrued.AitInterest;
                    }

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO InvestmentAccrued(
InvestmentNameId
,FiscalYearDetailId
,TransactionDate
,ReferenceNo
,InvestmentValue
,AccruedMonth
,InterestRate
,AccruedInterest
,AitInterest
,NetInterest
,Post
,CreatedBy
,CreatedAt
,CreatedFrom
,TransType

) VALUES (
 @InvestmentNameId
,@FiscalYearDetailId
,@TransactionDate
,@ReferenceNo
,@InvestmentValue
,@AccruedMonth
,@InterestRate
,@AccruedInterest
,@AitInterest
,@NetInterest
,@Post
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@TransType
) 
";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);                    
                    cmdInsert.Parameters.AddWithValue("@InvestmentNameId",vm.Id);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.InvestmentAccrued.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@TransactionDate", Ordinary.DateToString(vm.InvestmentAccrued.TransactionDate));
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo", vm.InvestmentAccrued.ReferenceNo);
                    cmdInsert.Parameters.AddWithValue("@InvestmentValue", vm.InvestmentAccrued.InvestmentValue);
                    cmdInsert.Parameters.AddWithValue("@AccruedMonth", vm.InvestmentAccrued.AccruedMonth);
                    cmdInsert.Parameters.AddWithValue("@InterestRate", vm.InvestmentAccrued.InterestRate);
                    cmdInsert.Parameters.AddWithValue("@AccruedInterest", vm.InvestmentAccrued.AccruedInterest);
                    cmdInsert.Parameters.AddWithValue("@AitInterest", vm.InvestmentAccrued.AitInterest);
                    cmdInsert.Parameters.AddWithValue("@NetInterest", vm.InvestmentAccrued.NetInterest);
                    cmdInsert.Parameters.AddWithValue("@Post", false);

                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom",  Ordinary.DateToString(DateTime.Now.ToString()));//vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@TransType", vm.TransType ?? "PF");

                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update InvestmentNames.", "");
                    }                     
                }
                else
                {
                    throw new ArgumentNullException("InvestmentName Update", "Could not found any item.");
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

        public string[] Post(string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Loan"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Post"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings

                    #region Posted
                   string sqlText = @"select count(Id) from InvestmentAccrued where    
                            Id = @id and post=1 ";
                    SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                    cmdCodeExist.Transaction = transaction;
                    cmdCodeExist.Parameters.AddWithValue("@id", ids[0]);

                    int countId = (int)cmdCodeExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        retResults[1] = "This Accured Already Posted for this month ";
                        throw new ArgumentNullException("", retResults[1]);
                    }
                    #endregion Posted

                    retResults = _cDal.FieldPost("InvestmentAccrued", "Id", ids[0], currConn, transaction);
                    if (retResults[0].ToLower() == "fail")
                    {
                        throw new ArgumentNullException("Loan Post", ids[0] + " could not Post.");
                    }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Loan Post - Could not found any item.", "");
                }

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                retResults[0] = "Success";
                retResults[1] = "Data Posted Successfully.";
                #endregion
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
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

        public string[] Delete(string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Loan"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Post"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length ; i++)
                    {
                        #region Posted
                        sqlText = @"select count(Id) from InvestmentAccrued where    
                            Id = @id and post=1 ";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@id", ids[i]);

                       int  countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            retResults[1] = "This Accured Already Posted for this month ";
                            throw new ArgumentNullException("", retResults[1]);
                        }
                        #endregion Posted
                        #region deleted

                        string deleteDetails = @"delete from InvestmentAccrued 
                        where   Id = @id ";
                        SqlCommand cmdUpdate = new SqlCommand(deleteDetails, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@id", ids[i]);
                        cmdUpdate.ExecuteNonQuery();
                        #endregion deleted
 
                    }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Loan Post - Could not found any item.", "");
                }

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                retResults[0] = "Success";
                retResults[1] = "Data Deleted Successfully.";
                #endregion
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
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


    
    }
}
