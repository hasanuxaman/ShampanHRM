using SymOrdinary;
using SymServices.Common;

using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace SymServices.PF
{
    public class InvestmentDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
      
        //==================SelectAll=================
        public List<InvestmentVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<InvestmentVM> VMs = new List<InvestmentVM>();
            InvestmentVM vm;
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
inv.Id
,inv.TransactionCode
,inv.TransactionType
,inv.ReferenceId

,inv.ReferenceNo
,ISNULL(inv.InvestmentNameId,0)InvestmentNameId
,ina.Name InvestmentName
,ISNULL(ina.AitInterest,0) AitInterest
,ISNULL(inv.InvestmentTypeId,0)InvestmentTypeId
,eit.Name InvestmentType
,inv.InvestmentAddress
,inv.InvestmentDate
,inv.FromDate
,inv.ToDate
,inv.MaturityDate
,inv.InvestmentRate
,inv.InvestmentValue
,inv.Remarks
,inv.Post
,inv.IsActive
,inv.IsArchive
,inv.CreatedBy
,inv.CreatedAt
,inv.CreatedFrom
,inv.LastUpdateBy
,inv.LastUpdateAt
,inv.LastUpdateFrom
,isnull(inv.IsEncashed,0)IsEncashed
,isnull(inv.IsEncashed,0)IsEncashed
, DATEDIFF(month, CONVERT(DATE, CONVERT(CHAR(8), inv.FromDate), 112), CONVERT(DATE, CONVERT(CHAR(8), inv.ToDate), 112))+1 AS InvestmenMonths
,FORMAT((inv.InvestmentValue * inv.InvestmentRate) / 100.00, 'N2') AS TotalInterest
,FORMAT((inv.InvestmentValue + ((inv.InvestmentValue * inv.InvestmentRate) / 100.00)), 'N2') AS TotalAmount
from Investments inv
LEFT OUTER JOIN EnumInvestmentTypes eit ON inv.InvestmentTypeId = eit.Id
LEFT OUTER JOIN InvestmentNames ina ON inv.InvestmentNameId = ina.Id
WHERE  1=1 AND inv.IsArchive = 0
";
                //InvestmentType
                //InvestmentTypeId
                //ReferenceNo

                if (Id > 0)
                {
                    sqlText += @" and inv.Id=@Id";
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
                    vm = new InvestmentVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    
                    vm.TransactionCode = dr["TransactionCode"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.ReferenceId = Convert.ToInt32(dr["ReferenceId"]);


                    vm.ReferenceNo = dr["ReferenceNo"].ToString();
                    vm.InvestmentNameId = Convert.ToInt32(dr["InvestmentNameId"]);
                    vm.InvestmentName = dr["InvestmentName"].ToString();
                    vm.AitInterest =Convert.ToDecimal(dr["AitInterest"].ToString());
                    vm.InvestmentTypeId = Convert.ToInt32(dr["InvestmentTypeId"]);
                    vm.InvestmentType = dr["InvestmentType"].ToString();

                    vm.InvestmentAddress = dr["InvestmentAddress"].ToString();
                    vm.InvestmentDate = Ordinary.StringToDate(dr["InvestmentDate"].ToString());
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.MaturityDate = Ordinary.StringToDate(dr["MaturityDate"].ToString());
                    vm.InvestmentRate = Convert.ToDecimal(dr["InvestmentRate"]);
                    vm.InvestmentValue = Convert.ToDecimal(dr["InvestmentValue"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);


                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsEncashed = Convert.ToBoolean(dr["IsEncashed"]);
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

                    vm.InvestmentMonths = dr["InvestmenMonths"].ToString();
                    vm.TotalInterest = ((Convert.ToDecimal(dr["TotalInterest"].ToString()) / 12)) * Convert.ToDecimal(vm.InvestmentMonths);
                    decimal roundedTotalInterest = Math.Round(vm.TotalInterest, 2);
                    vm.TotalInterest = roundedTotalInterest;
                    vm.AIT =Math.Round((vm.TotalInterest/100)* vm.AitInterest,2);
                    vm.NetInterest = vm.TotalInterest - vm.AIT; 
                    vm.TotalAmount =Math.Round( Convert.ToDecimal(vm.InvestmentValue) + Convert.ToDecimal(vm.NetInterest),2);


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

        //==================PreInsert=================

        //==================Insert =================
        public string[] Insert(InvestmentVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertInvestment"; //Method Name
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
                #region Exist Check
                ////bool isExist = false;
                string[] cFields = { "inv.ReferenceNo", "inv.InvestmentTypeId" };
                string[] cValues = { vm.ReferenceNo.Trim(), vm.InvestmentTypeId.ToString() };
                InvestmentVM varInvestmentVM = new InvestmentVM();
                varInvestmentVM = SelectAll(0, cFields, cValues, currConn, transaction).FirstOrDefault();

                ////isExist = _cDal.ExistCheck("Investments", conditionFields, conditionValues, currConn, transaction);

                if (varInvestmentVM != null)
                {
                    retResults[1] = "This Investment already used! " + varInvestmentVM.InvestmentName + ": " + varInvestmentVM.ReferenceNo;
                    throw new ArgumentNullException(retResults[1], "");
                }
                #endregion Exist Check

                vm.Id = _cDal.NextId("Investments", currConn, transaction);
                if (vm != null)
                {

                    string NewCode = new CommonDAL().CodeGenerationPF(vm.TransType, "Investment", vm.InvestmentDate, currConn, transaction);

                    vm.TransactionCode = NewCode;

                    ////vm.TransactionCode = "INV-" + (vm.Id.ToString()).PadLeft(4, '0');

                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO Investments(
Id
,TransactionCode
,TransactionType
,ReferenceId

,InvestmentNameId
,ReferenceNo
,InvestmentTypeId
,InvestmentAddress
,InvestmentDate
,FromDate
,ToDate
,MaturityDate
,InvestmentRate
,InvestmentValue
,Post
,TransType
 

,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@TransactionCode
,@TransactionType
,@ReferenceId

,@InvestmentNameId
,@ReferenceNo
,@InvestmentTypeId
,@InvestmentAddress
,@InvestmentDate
,@FromDate
,@ToDate
,@MaturityDate
,@InvestmentRate
,@InvestmentValue
,@Post
,@TransType
 
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    #endregion

                    #region SqlExecution

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);

                    cmdInsert.Parameters.AddWithValue("@TransactionCode", vm.TransactionCode);
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceId", vm.ReferenceId);

                    cmdInsert.Parameters.AddWithValue("@InvestmentNameId", vm.InvestmentNameId);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo", vm.ReferenceNo.Trim());
                    cmdInsert.Parameters.AddWithValue("@InvestmentTypeId", vm.InvestmentTypeId);
                    cmdInsert.Parameters.AddWithValue("@InvestmentAddress", vm.InvestmentAddress ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@InvestmentDate", Ordinary.DateToString(vm.InvestmentDate));
                    cmdInsert.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate) ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate) ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@MaturityDate", Ordinary.DateToString(vm.MaturityDate) ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@InvestmentRate", vm.InvestmentRate);
                    cmdInsert.Parameters.AddWithValue("@InvestmentValue", vm.InvestmentValue);
                    cmdInsert.Parameters.AddWithValue("@Post", vm.Post);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@TransType", vm.TransType ?? "PF");

                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Investments.", "");
                    }
                    #endregion
                    #region  Comments
//                    #region Detail
//                    {
//                        if (vm.detailVMs != null && vm.detailVMs.Count > 0)
//                        {
//                            InvestmentDetailDAL _detailDAL = new InvestmentDetailDAL();

//                            foreach (InvestmentDetailVM detailVM in vm.detailVMs)
//                            {
//                                detailVM.InvestmentId = vm.Id;
//                                detailVM.TransactionDate = vm.InvestmentDate;
//                                detailVM.TransactionType = vm.TransactionType;
//                            }

//                            retResults = _detailDAL.Insert(vm.detailVMs, currConn, transaction);

//                            if (retResults[0] == "Fail")
//                            {
//                                throw new ArgumentNullException(retResults[1], "");
//                            }

//                        }

//                    }
//                    #endregion

//                    #region GLTransactionDetail
//                    {
//                        if (vm.detailVMs != null && vm.detailVMs.Count > 0)
//                        {
//                            GLTransactionDetailDAL _detailDAL = new GLTransactionDetailDAL();
//                            List<GLTransactionDetailVM> detailVMs = new List<GLTransactionDetailVM>();
//                            GLTransactionDetailVM dVM = new GLTransactionDetailVM();

//                            foreach (InvestmentDetailVM detailVM in vm.detailVMs)
//                            {
//                                dVM = new GLTransactionDetailVM();
//                                dVM.TransactionMasterId = vm.Id;
//                                dVM.TransactionCode = vm.TransactionCode;
//                                dVM.TransactionDate = vm.InvestmentDate;
//                                dVM.TransactionType = vm.TransactionType;
//                                dVM.AccountId = detailVM.AccountId;
//                                dVM.DebitAmount = detailVM.DebitAmount;
//                                dVM.CreditAmount = detailVM.CreditAmount;
//                                dVM.TransactionAmount = 0;////vm.DepositAmount;
//                                dVM.Post = vm.Post;
//                                dVM.Remarks = detailVM.Remarks;
//                                dVM.CreatedBy = vm.CreatedBy;
//                                dVM.CreatedAt = vm.CreatedAt;
//                                dVM.CreatedFrom = vm.CreatedFrom;

//                                detailVMs.Add(dVM);
//                            }

//                            retResults = _detailDAL.Insert(detailVMs, currConn, transaction);

//                            if (retResults[0] == "Fail")
//                            {
//                                throw new ArgumentNullException(retResults[1], "");
//                            }

//                        }
//                    }
//                    #endregion

//                    #region Update Source

//                    #region Update Withdraws
//                    sqlText = "";
//                    sqlText = @"
//------declare @Id int = 1
//UPDATE Withdraws SET IsInvested = 1 WHERE 1=1 AND Id = @Id
//";
//                    if (!string.IsNullOrWhiteSpace(sqlText))
//                    {

//                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

//                        //cmdUpdate.Parameters.AddWithValue("@Id", vm.ReferenceId);

//                        //exeRes = cmdUpdate.ExecuteNonQuery();
//                        //transResult = Convert.ToInt32(exeRes);
//                        //if (transResult <= 0)
//                        //{
//                        //    retResults[3] = sqlText;
//                        //    throw new ArgumentNullException("Unexpected Error to update IsInvested Flag to Withdraws", "");
//                        //}

//                    }
//                    #endregion

//                    #endregion
                    #endregion  Comments

                }
                else
                {
                    retResults[1] = "This Investment already used!";
                    throw new ArgumentNullException("Please Input Investment Value", "");
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
        public string[] Update(InvestmentVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Investment Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToInvestment"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist Check
                    string[] cFields = { "inv.ReferenceNo", "inv.InvestmentTypeId", "inv.Id!" };
                    string[] cValues = { vm.ReferenceNo.Trim(), vm.InvestmentTypeId.ToString(), vm.Id.ToString() };

                    InvestmentVM varInvestmentVM = new InvestmentVM();
                    varInvestmentVM = SelectAll(0, cFields, cValues, currConn, transaction).FirstOrDefault();

                    if (varInvestmentVM != null)
                    {
                        retResults[1] = "This Investment already used! " + varInvestmentVM.InvestmentName + ": " + varInvestmentVM.ReferenceNo;
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion Exist Check

                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE Investments SET";
                    sqlText += " ReferenceNo =@ReferenceNo";
                    sqlText += " ,InvestmentNameId =@InvestmentNameId";
                    sqlText += " ,InvestmentTypeId =@InvestmentTypeId";
                    sqlText += " ,InvestmentAddress =@InvestmentAddress";
                    sqlText += " ,InvestmentDate =@InvestmentDate";
                    sqlText += " ,FromDate =@FromDate";
                    sqlText += " ,ToDate =@ToDate";
                    sqlText += " ,MaturityDate =@MaturityDate";
                    sqlText += " ,InvestmentRate =@InvestmentRate";
                    sqlText += " ,InvestmentValue =@InvestmentValue";
                    sqlText += " ,Post =@Post";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " , TransType=@TransType";
                    sqlText += " WHERE Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo", vm.ReferenceNo);
                    cmdUpdate.Parameters.AddWithValue("@InvestmentNameId", vm.InvestmentNameId);
                    cmdUpdate.Parameters.AddWithValue("@InvestmentTypeId", vm.InvestmentTypeId);
                    cmdUpdate.Parameters.AddWithValue("@InvestmentAddress", vm.InvestmentAddress ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@InvestmentDate", Ordinary.DateToString(vm.InvestmentDate));
                    cmdUpdate.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate) ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate) ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MaturityDate", Ordinary.DateToString(vm.MaturityDate) ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@InvestmentRate", vm.InvestmentRate);
                    cmdUpdate.Parameters.AddWithValue("@InvestmentValue", vm.InvestmentValue);
                    cmdUpdate.Parameters.AddWithValue("@Post", vm.Post);


                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@TransType", vm.TransType ?? "PF");

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Investments.", "");
                    }

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Investment Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings

                    

                    #region GLTransactionDetail
                    //{
                    //    if (vm.detailVMs != null && vm.detailVMs.Count > 0)
                    //    {
                    //        #region Delete First
                    //        sqlText = " ";
                    //        sqlText = "DELETE GLTransactionDetails WHERE TransactionCode=@TransactionCode";
                    //        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                    //        cmdDelete.Parameters.AddWithValue("@TransactionCode", vm.TransactionCode);
                    //        exeRes = cmdDelete.ExecuteNonQuery();
                    //        transResult = Convert.ToInt32(exeRes);

                    //        #endregion

                    //        #region Insert Again


                    //        GLTransactionDetailDAL _detailDAL = new GLTransactionDetailDAL();
                    //        List<GLTransactionDetailVM> detailVMs = new List<GLTransactionDetailVM>();
                    //        GLTransactionDetailVM dVM = new GLTransactionDetailVM();

                    //        foreach (InvestmentDetailVM detailVM in vm.detailVMs)
                    //        {
                    //            dVM = new GLTransactionDetailVM();
                    //            dVM.TransactionMasterId = vm.Id;
                    //            dVM.TransactionCode = vm.TransactionCode;

                    //            dVM.TransactionDate = vm.InvestmentDate;
                    //            dVM.TransactionType = vm.TransactionType;
                    //            dVM.AccountId = detailVM.AccountId;
                    //            dVM.DebitAmount = detailVM.DebitAmount;
                    //            dVM.CreditAmount = detailVM.CreditAmount;
                    //            dVM.TransactionAmount = 0;////vm.DepositAmount;
                    //            dVM.Post = vm.Post;
                    //            dVM.Remarks = detailVM.Remarks;
                    //            dVM.CreatedBy = vm.LastUpdateBy;
                    //            dVM.CreatedAt = vm.LastUpdateAt;
                    //            dVM.CreatedFrom = vm.LastUpdateFrom;

                    //            detailVMs.Add(dVM);
                    //        }

                    //        retResults = _detailDAL.Insert(detailVMs, currConn, transaction);

                    //        if (retResults[0] == "Fail")
                    //        {
                    //            throw new ArgumentNullException(retResults[1], "");
                    //        }
                    //        #endregion

                    //    }
                    //}
                    #endregion
                }
                else
                {
                    throw new ArgumentNullException("Investment Update", "Could not found any item.");
                }

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
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
        public string[] Delete(InvestmentVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteInvestment"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToInvestment"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Check Posted or Not Posted
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        retVal = _cDal.SelectFieldValue("Investments", "Post", "Id", ids[i].ToString(), currConn, transaction);
                        vm.Post = Convert.ToBoolean(retVal);
                        if (vm.Post == true)
                        {
                            retResults[0] = "Fail";
                            retResults[1] = "Data Alreday Posted! Cannot be Deleted.";
                            throw new ArgumentNullException("Data Alreday Posted! Cannot Deleted.", "");
                        }
                    }
                    #endregion Check Posted or Not Posted

                    #region Delete Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = " ";
                        sqlText = "DELETE Investments";
                        sqlText += " WHERE Id=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update Investments.", "");
                        }
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Investment Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                }
                else
                {
                    throw new ArgumentNullException("Investment Information Delete", "Could not found any item.");
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

        ////==================Post =================
        public string[] Post(string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PostInvestment"; //Method Name
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
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        retResults = _cDal.FieldPost("Investments", "Id", ids[i], currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            throw new ArgumentNullException("Investments Post", ids[i] + " could not Post.");
                        }
                    }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Investment Information Post - Could not found any item.", "");
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


        ////==================Report=================
        public DataTable Report(InvestmentVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                #region sql statement
                sqlText = @"
SELECT
inv.Id
,inv.InvestmentTypeId
,inv.ReferenceNo
,eit.Name InvestmentType
,inv.InvestmentAddress
,inv.InvestmentDate
,inv.FromDate
,inv.ToDate
,inv.MaturityDate
,inv.InvestmentRate
,inv.InvestmentValue
,inv.Remarks
,inv.Post
from Investments inv
LEFT OUTER JOIN EnumInvestmentTypes eit ON inv.InvestmentTypeId = eit.Id
WHERE  1=1 AND  inv.IsArchive = 0
";
                //ReferenceNo
                //InvestmentTypeId
                //InvestmentType
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
                string[] DtColumnChange = { "InvestmentDate", "FromDate", "ToDate", "MaturityDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, DtColumnChange);

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


        ////==================Report=================
        public DataTable InvestmentStatementReport(InvestmentVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                #region sql statement
                sqlText = @"
SELECT
inv.Id
,inv.InvestmentTypeId
,inv.ReferenceNo
,eit.Name InvestmentType
,inv.InvestmentAddress
,inv.InvestmentDate
,inv.FromDate
,inv.ToDate
,inv.MaturityDate
,inv.InvestmentRate
,inv.InvestmentValue
,inv.Remarks
,inv.Post

,ISNULL(roi.ROIDate				,'NA')ROIDate
,ISNULL(roi.ROITotalValue		,0)ROITotalValue
,ISNULL(roi.ROIRate				,0)ROIRate
,ISNULL(roi.TotalInterestValue	,0)TotalInterestValue
,ISNULL(roi.Remarks				,'NA')ROIRemarks

from Investments inv
LEFT OUTER JOIN EnumInvestmentTypes eit ON inv.InvestmentTypeId = eit.Id
LEFT OUTER JOIN ReturnOnInvestments roi ON inv.Id = roi.InvestmentId
WHERE  1=1 AND  inv.IsArchive = 0
";
                //ReferenceNo
                //InvestmentTypeId
                //InvestmentType
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
                string[] DtColumnChange = { "ROIDate", "InvestmentDate", "FromDate", "ToDate", "MaturityDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, DtColumnChange);

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


        #endregion
    }
}
