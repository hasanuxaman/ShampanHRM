using SymOrdinary;
using SymServices.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.Payroll
{
   public class SalaryStructureMatrixDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        //==================SelectAll=================
        public List<SalaryStructureMatrixVM> SelectAll(string SalaryTypeName = "", string GradeId = "", string desGroupId = "", string currentYear = "", string currentYearPart = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryStructureMatrixVM> VMs = new List<SalaryStructureMatrixVM>();
            SalaryStructureMatrixVM VM;
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
Id
,BranchId
,GradeId
,SalaryTypeName
,isnull(Step1Amount,0)Step1Amount 
,isnull(Step2Amount,0)Step2Amount 
,isnull(Step3Amount,0)Step3Amount 
,isnull(Step4Amount,0)Step4Amount 
,isnull(Step5Amount,0)Step5Amount 
,isnull(Step6Amount,0)Step6Amount 
,isnull(Step7Amount,0)Step7Amount 
,isnull(Step8Amount,0)Step8Amount 
,isnull(Step9Amount,0)Step9Amount 
,isnull(Step10Amount,0)Step10Amount
,isnull(Step11Amount,0)Step11Amount
,isnull(Step12Amount,0)Step12Amount
,isnull(Step13Amount,0)Step13Amount
,isnull(Step14Amount,0)Step14Amount
,isnull(Step15Amount,0)Step15Amount
,isnull(Step16Amount,0)Step16Amount
,isnull(Step17Amount,0)Step17Amount
,isnull(Step18Amount,0)Step18Amount
,isnull(Step19Amount,0)Step19Amount
,isnull(Step20Amount,0)Step20Amount
,isnull(Step21Amount,0)Step21Amount
,isnull(Step22Amount,0)Step22Amount
,isnull(Step23Amount,0)Step23Amount
,isnull(Step24Amount,0)Step24Amount
,isnull(Step25Amount,0)Step25Amount
,isnull(Step26Amount,0)Step26Amount
,isnull(Step27Amount,0)Step27Amount
,isnull(Step28Amount,0)Step28Amount
,isnull(Step29Amount,0)Step29Amount
,isnull(Step30Amount,0)Step30Amount

,isnull(Step1_5Amount,0)Step1_5Amount
,isnull(Step2_5Amount,0)Step2_5Amount
,isnull(Step3_5Amount,0)Step3_5Amount
,isnull(Step4_5Amount,0)Step4_5Amount
,isnull(Step5_5Amount,0)Step5_5Amount
,isnull(Step6_5Amount,0)Step6_5Amount
,isnull(Step7_5Amount,0)Step7_5Amount
,isnull(Step8_5Amount,0)Step8_5Amount
,isnull(Step9_5Amount,0)Step9_5Amount
,isnull(Step10_5Amount,0)Step10_5Amount
,isnull(Step11_5Amount,0)Step11_5Amount
,isnull(Step12_5Amount,0)Step12_5Amount
,isnull(Step13_5Amount,0)Step13_5Amount
,isnull(Step14_5Amount,0)Step14_5Amount
,isnull(Step15_5Amount,0)Step15_5Amount
,isnull(Step16_5Amount,0)Step16_5Amount
,isnull(Step17_5Amount,0)Step17_5Amount
,isnull(Step18_5Amount,0)Step18_5Amount
,isnull(Step19_5Amount,0)Step19_5Amount
,isnull(Step20_5Amount,0)Step20_5Amount
,isnull(Step21_5Amount,0)Step21_5Amount
,isnull(Step22_5Amount,0)Step22_5Amount
,isnull(Step23_5Amount,0)Step23_5Amount
,isnull(Step24_5Amount,0)Step24_5Amount
,isnull(Step25_5Amount,0)Step25_5Amount
,isnull(Step26_5Amount,0)Step26_5Amount
,isnull(Step27_5Amount,0)Step27_5Amount
,isnull(Step28_5Amount,0)Step28_5Amount
,isnull(Step29_5Amount,0)Step29_5Amount
,isnull(Step30_5Amount,0)Step30_5Amount
,isnull(Step31_5Amount,0)Step31_5Amount
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,DesignationGroupId
    From SalaryStructureMatrix
    Where 1=1  
";

                if (!string.IsNullOrWhiteSpace(SalaryTypeName))
                {
                    sqlText += @" and SalaryTypeName='" + SalaryTypeName + "'";
                }
                if (!string.IsNullOrWhiteSpace(GradeId))
                {
                    sqlText += @" and GradeId='" + GradeId + "'";
                }

                if (!string.IsNullOrWhiteSpace(desGroupId))
                {
                    sqlText += @" and DesignationGroupId='" + desGroupId + "'";
                }

                if (!string.IsNullOrWhiteSpace(currentYear))
                {
                    sqlText += @" and currentYear='" + currentYear + "'";
                }
                if (!string.IsNullOrWhiteSpace(currentYearPart))
                {
                    sqlText += @" and Isnull(CurrentYearPart,'A')='" + currentYearPart + "'";
                }
                sqlText += @"  order by SL";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
               
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    VM = new SalaryStructureMatrixVM();
                    VM.Id = dr["Id"].ToString();
                    VM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    VM.GradeId=dr["GradeId"].ToString();;
                    VM.SalaryTypeName=dr["SalaryTypeName"].ToString();;
                    VM.Step1Amount=Convert.ToDecimal(dr["Step1Amount"]);
                    VM.Step2Amount=Convert.ToDecimal(dr["Step2Amount"]);
                    VM.Step3Amount=Convert.ToDecimal(dr["Step3Amount"]);
                    VM.Step4Amount=Convert.ToDecimal(dr["Step4Amount"]);
                    VM.Step5Amount=Convert.ToDecimal(dr["Step5Amount"]);
                    VM.Step6Amount=Convert.ToDecimal(dr["Step6Amount"]);
                    VM.Step7Amount=Convert.ToDecimal(dr["Step7Amount"]);
                    VM.Step8Amount=Convert.ToDecimal(dr["Step8Amount"]);
                    VM.Step9Amount=Convert.ToDecimal(dr["Step9Amount"]);
                    VM.Step10Amount=Convert.ToDecimal(dr["Step10Amount"]);
                    VM.Step11Amount=Convert.ToDecimal(dr["Step11Amount"]);
                    VM.Step12Amount=Convert.ToDecimal(dr["Step12Amount"]);
                    VM.Step13Amount=Convert.ToDecimal(dr["Step13Amount"]);
                    VM.Step14Amount=Convert.ToDecimal(dr["Step14Amount"]);
                    VM.Step15Amount=Convert.ToDecimal(dr["Step15Amount"]);
                    VM.Step16Amount=Convert.ToDecimal(dr["Step16Amount"]);
                    VM.Step17Amount=Convert.ToDecimal(dr["Step17Amount"]);
                    VM.Step18Amount=Convert.ToDecimal(dr["Step18Amount"]);
                    VM.Step19Amount=Convert.ToDecimal(dr["Step19Amount"]);
                    VM.Step20Amount=Convert.ToDecimal(dr["Step20Amount"]);
                    VM.Step21Amount=Convert.ToDecimal(dr["Step21Amount"]);
                    VM.Step22Amount=Convert.ToDecimal(dr["Step22Amount"]);
                    VM.Step23Amount=Convert.ToDecimal(dr["Step23Amount"]);
                    VM.Step24Amount=Convert.ToDecimal(dr["Step24Amount"]);
                    VM.Step25Amount=Convert.ToDecimal(dr["Step25Amount"]);
                    VM.Step26Amount=Convert.ToDecimal(dr["Step26Amount"]);
                    VM.Step27Amount=Convert.ToDecimal(dr["Step27Amount"]);
                    VM.Step28Amount=Convert.ToDecimal(dr["Step28Amount"]);
                    VM.Step29Amount=Convert.ToDecimal(dr["Step30Amount"]);
                    VM.Step30Amount=Convert.ToDecimal(dr["Step30Amount"]);
                    VM.Step1_5Amount = Convert.ToDecimal(dr["Step1_5Amount"]);
                    VM.Step2_5Amount = Convert.ToDecimal(dr["Step2_5Amount"]);
                    VM.Step3_5Amount = Convert.ToDecimal(dr["Step3_5Amount"]);
                    VM.Step4_5Amount = Convert.ToDecimal(dr["Step4_5Amount"]);
                    VM.Step5_5Amount = Convert.ToDecimal(dr["Step5_5Amount"]);
                    VM.Step6_5Amount = Convert.ToDecimal(dr["Step6_5Amount"]);
                    VM.Step7_5Amount = Convert.ToDecimal(dr["Step7_5Amount"]);
                    VM.Step8_5Amount = Convert.ToDecimal(dr["Step8_5Amount"]);
                    VM.Step9_5Amount = Convert.ToDecimal(dr["Step9_5Amount"]);
                    VM.Step10_5Amount = Convert.ToDecimal(dr["Step10_5Amount"]);
                    VM.Step11_5Amount = Convert.ToDecimal(dr["Step11_5Amount"]);
                    VM.Step12_5Amount = Convert.ToDecimal(dr["Step12_5Amount"]);
                    VM.Step13_5Amount = Convert.ToDecimal(dr["Step13_5Amount"]);
                    VM.Step14_5Amount = Convert.ToDecimal(dr["Step14_5Amount"]);
                    VM.Step15_5Amount = Convert.ToDecimal(dr["Step15_5Amount"]);
                    VM.Step16_5Amount = Convert.ToDecimal(dr["Step16_5Amount"]);
                    VM.Step17_5Amount = Convert.ToDecimal(dr["Step17_5Amount"]);
                    VM.Step18_5Amount = Convert.ToDecimal(dr["Step18_5Amount"]);
                    VM.Step19_5Amount = Convert.ToDecimal(dr["Step19_5Amount"]);
                    VM.Step20_5Amount = Convert.ToDecimal(dr["Step20_5Amount"]);
                    VM.Step21_5Amount = Convert.ToDecimal(dr["Step21_5Amount"]);
                    VM.Step22_5Amount = Convert.ToDecimal(dr["Step22_5Amount"]);
                    VM.Step23_5Amount = Convert.ToDecimal(dr["Step23_5Amount"]);
                    VM.Step24_5Amount = Convert.ToDecimal(dr["Step24_5Amount"]);
                    VM.Step25_5Amount = Convert.ToDecimal(dr["Step25_5Amount"]);
                    VM.Step26_5Amount = Convert.ToDecimal(dr["Step26_5Amount"]);
                    VM.Step27_5Amount = Convert.ToDecimal(dr["Step27_5Amount"]);
                    VM.Step28_5Amount = Convert.ToDecimal(dr["Step28_5Amount"]);
                    VM.Step29_5Amount = Convert.ToDecimal(dr["Step29_5Amount"]);
                    VM.Step30_5Amount = Convert.ToDecimal(dr["Step30_5Amount"]);
                    VM.Step31_5Amount = Convert.ToDecimal(dr["Step31_5Amount"]);

                    VM.Remarks=dr["Remarks"].ToString();
                    VM.DesignationGroupId = dr["DesignationGroupId"].ToString();

                    VM.IsActive=Convert.ToBoolean(dr["IsActive"]);
                    VM.CreatedAt=Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    VM.CreatedBy=dr["CreatedBy"].ToString();
                    VM.CreatedFrom=dr["CreatedFrom"].ToString();
                    VM.LastUpdateAt=Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    VM.LastUpdateBy=dr["LastUpdateBy"].ToString();
                    VM.LastUpdateFrom=dr["LastUpdateFrom"].ToString();
                    VMs.Add(VM);
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
        //==================SelectByID=================
        public SalaryStructureMatrixVM SelectById(string Id)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            SalaryStructureMatrixVM VM = new SalaryStructureMatrixVM();
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
Id
,BranchId
,GradeId
,SalaryTypeName
,isnull(Step1Amount,0)Step1Amount 
,isnull(Step2Amount,0)Step2Amount 
,isnull(Step3Amount,0)Step3Amount 
,isnull(Step4Amount,0)Step4Amount 
,isnull(Step5Amount,0)Step5Amount 
,isnull(Step6Amount,0)Step6Amount 
,isnull(Step7Amount,0)Step7Amount 
,isnull(Step8Amount,0)Step8Amount 
,isnull(Step9Amount,0)Step9Amount 
,isnull(Step10Amount,0)Step10Amount
,isnull(Step11Amount,0)Step11Amount
,isnull(Step12Amount,0)Step12Amount
,isnull(Step13Amount,0)Step13Amount
,isnull(Step14Amount,0)Step14Amount
,isnull(Step15Amount,0)Step15Amount
,isnull(Step16Amount,0)Step16Amount
,isnull(Step17Amount,0)Step17Amount
,isnull(Step18Amount,0)Step18Amount
,isnull(Step19Amount,0)Step19Amount
,isnull(Step20Amount,0)Step20Amount
,isnull(Step21Amount,0)Step21Amount
,isnull(Step22Amount,0)Step22Amount
,isnull(Step23Amount,0)Step23Amount
,isnull(Step24Amount,0)Step24Amount
,isnull(Step25Amount,0)Step25Amount
,isnull(Step26Amount,0)Step26Amount
,isnull(Step27Amount,0)Step27Amount
,isnull(Step28Amount,0)Step28Amount
,isnull(Step29Amount,0)Step29Amount
,isnull(Step30Amount,0)Step30Amount
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From SalaryStructureMatrix
Where IsArchive=0 and IsActive=1 and Id=@Id";

                SqlCommand Vehicle = new SqlCommand();
                Vehicle.Connection = currConn;
                Vehicle.CommandText = sqlText;
                Vehicle.CommandType = CommandType.Text;
                Vehicle.Parameters.AddWithValue("@Id", Id);

                SqlDataReader dr;
                dr = Vehicle.ExecuteReader();
                while (dr.Read())
                {
                    VM.Id = dr["Id"].ToString();
                    VM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    VM.GradeId = dr["GradeId"].ToString(); ;
                    VM.SalaryTypeName = dr["SalaryTypeName"].ToString(); ;
                    VM.Step1Amount = Convert.ToDecimal(dr["Step1Amount"]);
                    VM.Step2Amount = Convert.ToDecimal(dr["Step2Amount"]);
                    VM.Step3Amount = Convert.ToDecimal(dr["Step3Amount"]);
                    VM.Step4Amount = Convert.ToDecimal(dr["Step4Amount"]);
                    VM.Step5Amount = Convert.ToDecimal(dr["Step5Amount"]);
                    VM.Step6Amount = Convert.ToDecimal(dr["Step6Amount"]);
                    VM.Step7Amount = Convert.ToDecimal(dr["Step7Amount"]);
                    VM.Step8Amount = Convert.ToDecimal(dr["Step8Amount"]);
                    VM.Step9Amount = Convert.ToDecimal(dr["Step9Amount"]);
                    VM.Step10Amount = Convert.ToDecimal(dr["Step10Amount"]);
                    VM.Step11Amount = Convert.ToDecimal(dr["Step11Amount"]);
                    VM.Step12Amount = Convert.ToDecimal(dr["Step12Amount"]);
                    VM.Step13Amount = Convert.ToDecimal(dr["Step13Amount"]);
                    VM.Step14Amount = Convert.ToDecimal(dr["Step14Amount"]);
                    VM.Step15Amount = Convert.ToDecimal(dr["Step15Amount"]);
                    VM.Step16Amount = Convert.ToDecimal(dr["Step16Amount"]);
                    VM.Step17Amount = Convert.ToDecimal(dr["Step17Amount"]);
                    VM.Step18Amount = Convert.ToDecimal(dr["Step18Amount"]);
                    VM.Step19Amount = Convert.ToDecimal(dr["Step19Amount"]);
                    VM.Step20Amount = Convert.ToDecimal(dr["Step20Amount"]);
                    VM.Step21Amount = Convert.ToDecimal(dr["Step21Amount"]);
                    VM.Step22Amount = Convert.ToDecimal(dr["Step22Amount"]);
                    VM.Step23Amount = Convert.ToDecimal(dr["Step23Amount"]);
                    VM.Step24Amount = Convert.ToDecimal(dr["Step24Amount"]);
                    VM.Step25Amount = Convert.ToDecimal(dr["Step25Amount"]);
                    VM.Step26Amount = Convert.ToDecimal(dr["Step26Amount"]);
                    VM.Step27Amount = Convert.ToDecimal(dr["Step27Amount"]);
                    VM.Step28Amount = Convert.ToDecimal(dr["Step28Amount"]);
                    VM.Step29Amount = Convert.ToDecimal(dr["Step30Amount"]);
                    VM.Step30Amount = Convert.ToDecimal(dr["Step30Amount"]);
                    VM.Remarks = dr["Remarks"].ToString();
                    VM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    VM.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    VM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    VM.CreatedBy = dr["CreatedBy"].ToString();
                    VM.CreatedFrom = dr["CreatedFrom"].ToString();
                    VM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    VM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    VM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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
            return VM;
        }
        //==================Insert =================


        public string[] Insert(SalaryStructureMatrixVM VM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertGrade"; //Method SalaryTypeName
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
                    transaction = currConn.BeginTransaction("Insert");
                }
                #endregion open connection and transaction
                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "SalaryStructureMatrix";
                string[] fieldName = { "SalaryTypeName" };
                string[] fieldValue = { VM.Id.Trim() };

                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist

                #region Save
                if (1 == 1)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO SalaryStructureMatrix(
Id
,BranchId
,GradeI
,SalaryTypeName
,Step1Amount
,Step2Amount
,Step3Amount
,Step4Amount
,Step5Amount
,Step6Amount
,Step7Amount
,Step8Amount
,Step9Amount
,Step10Amount
,Step11Amount
,Step12Amount
,Step13Amount
,Step14Amount
,Step15Amount
,Step16Amount
,Step17Amount
,Step18Amount
,Step19Amount
,Step20Amount
,Step21Amount
,Step22Amount
,Step23Amount
,Step24Amount
,Step25Amount
,Step26Amount
,Step27Amount
,Step28Amount
,Step29Amount
,Step30Amount
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) VALUES (
,@Id
,@BranchId
,@GradeId
,@SalaryTypeName
,@Step1Amount
,@Step2Amount
,@Step3Amount
,@Step4Amount
,@Step5Amount
,@Step6Amount
,@Step7Amount
,@Step8Amount
,@Step9Amount
,@Step10Amount
,@Step11Amount
,@Step12Amount
,@Step13Amount
,@Step14Amount
,@Step15Amount
,@Step16Amount
,@Step17Amount
,@Step18Amount
,@Step19Amount
,@Step20Amount
,@Step21Amount
,@Step22Amount
,@Step23Amount
,@Step24Amount
,@Step25Amount
,@Step26Amount
,@Step27Amount
,@Step28Amount
,@Step29Amount
,@Step30Amount
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)SELECT SCOPE_IDENTITY()";

                    SqlCommand _cmdInsert = new SqlCommand(sqlText, currConn);
                    _cmdInsert.Parameters.AddWithValue("@Id", VM.Id);
                    _cmdInsert.Parameters.AddWithValue("@BranchId", VM.BranchId);
                    _cmdInsert.Parameters.AddWithValue("@GradeId", VM.GradeId);
                    _cmdInsert.Parameters.AddWithValue("@SalaryTypeName", VM.SalaryTypeName);
                    _cmdInsert.Parameters.AddWithValue("@Step1Amount", VM.Step1Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step2Amount", VM.Step2Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step3Amount", VM.Step3Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step4Amount", VM.Step4Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step5Amount", VM.Step5Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step6Amount", VM.Step6Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step7Amount", VM.Step7Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step8Amount", VM.Step8Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step9Amount", VM.Step9Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step10Amount", VM.Step10Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step11Amount", VM.Step11Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step12Amount", VM.Step12Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step13Amount", VM.Step13Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step14Amount", VM.Step14Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step15Amount", VM.Step15Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step16Amount", VM.Step16Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step17Amount", VM.Step17Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step18Amount", VM.Step18Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step19Amount", VM.Step19Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step20Amount", VM.Step20Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step21Amount", VM.Step21Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step22Amount", VM.Step22Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step23Amount", VM.Step23Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step24Amount", VM.Step24Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step25Amount", VM.Step25Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step26Amount", VM.Step26Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step27Amount", VM.Step27Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step28Amount", VM.Step28Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step29Amount", VM.Step29Amount);
                    _cmdInsert.Parameters.AddWithValue("@Step30Amount", VM.Step30Amount);
                    _cmdInsert.Parameters.AddWithValue("@Remarks", VM.Remarks ?? Convert.DBNull);
                    _cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    _cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    _cmdInsert.Parameters.AddWithValue("@CreatedBy", VM.CreatedBy);
                    _cmdInsert.Parameters.AddWithValue("@CreatedAt", VM.CreatedAt);
                    _cmdInsert.Parameters.AddWithValue("@CreatedFrom", VM.CreatedFrom);
                    _cmdInsert.Transaction = transaction;
                    var exeRes = _cmdInsert.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input BloodGroup Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input BloodGroup Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This BloodGroup already used";
                    throw new ArgumentNullException("Please Input BloodGroup Value", "");
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


        public DataTable MatrixCreateCheck(SalaryStructureMatrixVM VM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGrade"; //Method SalaryTypeName
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
                    transaction = currConn.BeginTransaction("Insert");
                }
                #endregion open connection and transaction
                #region Exist
                //CommonDAL cdal = new CommonDAL();
                //bool check = false;
                //string tableName = "SalaryStructureMatrix";
                //string[] fieldName = { "SalaryTypeName" };
                //string[] fieldValue = { VM.Id.Trim() };

                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                #endregion Exist

                #region Save
                DataTable dt = ProcessSalaryStructureMatrix(VM.CurrentYear, VM.CurrentYearPart);

                string[] DeleteColumn = { "Id", "SL", "GradeId", "BasicNextYearFactor", "BasicNextStepFactor", "IsHouseRentFactorFromBasic"
                    , "HouseRentFactor", "IsTAFactorFromBasic", "TAFactor", "IsMedicalFactorFromBasic", "Id"
                    , "GradeNo", "MedicalFactor", "CurrentBasic", "Id", "Id", "Id"
 , "Step20_5Amount"
 , "Step21Amount"
 , "Step21_5Amount"
 , "Step22Amount"
 , "Step22_5Amount"
 , "Step23Amount"
 , "Step23_5Amount"
 , "Step24Amount"
 , "Step24_5Amount"
 , "Step25Amount"
 , "Step25_5Amount"
 , "Step26Amount"
 , "Step26_5Amount"
 , "Step27Amount"
 , "Step27_5Amount"
 , "Step28Amount"
 , "Step28_5Amount"
 , "Step29Amount"
 , "Step29_5Amount"
 , "Step30Amount"
 , "Step30_5Amount"
 , "Step31Amount"
 , "Step31_5Amount"
 , "IsActive"
 , "IsArchive"
};

                dt = Ordinary.DtDeleteColumns(dt, DeleteColumn);


                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return dt;

                #endregion Save

                #region Commit

              
            }

            #endregion try

            #region Catch and Finall


            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex

                throw ex;
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

            #endregion


        }


        public string[] MatrixEffectSave(SalaryStructureMatrixVM VM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertGrade"; //Method SalaryTypeName
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
                    transaction = currConn.BeginTransaction("Insert");
                }
                #endregion open connection and transaction
                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "SalaryStructureMatrix";
                string[] fieldName = { "CurrentYear" };
                string[] fieldValue = { VM.CurrentYear.ToString().Trim() };

                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist

                #region Save
               

                 DataTable dt=   ProcessSalaryStructureMatrix(VM.CurrentYear);

                 DataTable dt1 = dt.Copy();
                 dt1 = Ordinary.DtColumnAdd(dt1, "BranchId", "1", "string");
                 dt1 = Ordinary.DtColumnAdd(dt1, "DesignationGroupId", "1", "string");
                 //dt1 = Ordinary.DtColumnAdd(dt1, "IsActive", "1", "string");
                 //dt1 = Ordinary.DtColumnAdd(dt1, "IsArchive", "0", "string");
                 dt1 = Ordinary.DtColumnAdd(dt1, "CreatedBy", "Admin", "string");
                 dt1 = Ordinary.DtColumnAdd(dt1, "CreatedAt", "19000101", "string");
                 dt1 = Ordinary.DtColumnAdd(dt1, "CreatedFrom", "Local", "string");

                 string[] DeleteColumn = {   "BasicNextYearFactor", "BasicNextStepFactor", "IsHouseRentFactorFromBasic"
                                                , "HouseRentFactor", "IsTAFactorFromBasic", "TAFactor", "IsMedicalFactorFromBasic" 
                                            , "GradeNo", "MedicalFactor", "CurrentBasic","Code","Name","Area"};

                 dt1 = Ordinary.DtDeleteColumns(dt1, DeleteColumn);
                retResults= cdal.BulkInsert("SalaryStructureMatrix", dt1, currConn, transaction, 0, null, _dbsqlConnection.HRMDB);

     
                
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

        public string[] MatrixCreate(SalaryStructureMatrixVM VM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertGrade"; //Method SalaryTypeName
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
                    transaction = currConn.BeginTransaction("Insert");
                }
                #endregion open connection and transaction
                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "SalaryStructureMatrix";
                string[] fieldName = { "CurrentYear", "CurrentYearPart" };
                string[] fieldValue = { VM.CurrentYear.ToString().Trim(), VM.CurrentYearPart.ToString() };

                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}

                check = cdal.CheckDuplicateInInsertConditionFields(tableName, "CurrentYear", currConn, transaction, fieldName, fieldValue);

                 if (check == true)
                 {
                     retResults = cdal.DeleteTable(tableName, fieldName, fieldValue, currConn, transaction);
                    
                 }
                #endregion Exist

                #region Save


                 DataTable dt = ProcessSalaryStructureMatrix(VM.CurrentYear, VM.CurrentYearPart);

                DataTable dt1 = dt.Copy();
                dt1 = Ordinary.DtColumnAdd(dt1, "BranchId", "1", "string");
                dt1 = Ordinary.DtColumnAdd(dt1, "DesignationGroupId", "1", "string");
                //dt1 = Ordinary.DtColumnAdd(dt1, "IsActive", "1", "string");
                //dt1 = Ordinary.DtColumnAdd(dt1, "IsArchive", "0", "string");
                dt1 = Ordinary.DtColumnAdd(dt1, "CreatedBy", "Admin", "string");
                dt1 = Ordinary.DtColumnAdd(dt1, "CreatedAt", "19000101", "string");
                dt1 = Ordinary.DtColumnAdd(dt1, "CreatedFrom", "Local", "string");
                dt1 = Ordinary.DtColumnAdd(dt1, "CurrentYearPart", VM.CurrentYearPart.ToString(), "string");

                string[] DeleteColumn = {   "BasicNextYearFactor", "BasicNextStepFactor", "IsHouseRentFactorFromBasic"
                                                , "HouseRentFactor", "IsTAFactorFromBasic", "TAFactor", "IsMedicalFactorFromBasic" 
                                            , "GradeNo", "MedicalFactor", "CurrentBasic","Code","Name","Area"};

                dt1 = Ordinary.DtDeleteColumns(dt1, DeleteColumn);
                retResults = cdal.BulkInsert("SalaryStructureMatrix", dt1, currConn, transaction, 0, null, _dbsqlConnection.HRMDB);



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


        public DataTable ProcessSalaryStructureMatrix(string CurrentYear = "",string CurrentYearPart = "")
        {

            DataTable dt = SelectSalaryStructureMatrixForProcess((Convert.ToInt32(CurrentYear) - 1).ToString(), CurrentYearPart);

            dt.Columns.Add("IsActive", typeof(bool));
            dt.Columns.Add("IsArchive", typeof(bool));

            string FYear = (Convert.ToInt32(CurrentYear)).ToString();
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                decimal CurrentBasic = Convert.ToDecimal(dt.Rows[r]["CurrentBasic"].ToString()) * Convert.ToDecimal(dt.Rows[r]["BasicNextYearFactor"].ToString());

                dt.Rows[r]["CurrentBasic"] = Math.Round(CurrentBasic, MidpointRounding.AwayFromZero);
                if (!string.IsNullOrEmpty(CurrentYearPart))
                {
                    dt.Rows[r]["Id"] = FYear + "_" + CurrentYearPart  +"_" + r + 1;
                }
                else
                {
                    dt.Rows[r]["Id"] = FYear + "_" + r + 1;
                }
                dt.Rows[r]["IsActive"] = true;
                dt.Rows[r]["IsArchive"] = false;
                dt.Rows[r]["CurrentYear"] =FYear;
            }

            for (int r = 0; r < dt.Rows.Count; r++)
            {
                string Area = dt.Rows[r]["Area"].ToString();
                string BasicNextYearFactor = dt.Rows[r]["BasicNextYearFactor"].ToString();
                string BasicNextStepFactor = dt.Rows[r]["BasicNextStepFactor"].ToString();
                string IsHouseRentFactorFromBasic = dt.Rows[r]["IsHouseRentFactorFromBasic"].ToString();
                string HouseRentFactor = dt.Rows[r]["HouseRentFactor"].ToString();
                string IsTAFactorFromBasic = dt.Rows[r]["IsTAFactorFromBasic"].ToString();
                string TAFactor = dt.Rows[r]["TAFactor"].ToString();
                string IsMedicalFactorFromBasic = dt.Rows[r]["IsMedicalFactorFromBasic"].ToString();
                string MedicalFactor = dt.Rows[r]["MedicalFactor"].ToString();
                string SalaryTypeName = dt.Rows[r]["SalaryTypeName"].ToString();

                decimal returnValue = 0;
                if (SalaryTypeName.ToLower() == "basic")
                {
                    decimal value = Convert.ToDecimal(dt.Rows[r]["CurrentBasic"].ToString());

                    for (int c = 18; c < dt.Columns.Count; c++)
                    {
                        if (c == 18)
                        {
                            returnValue = value;
                            returnValue = Math.Round(returnValue, MidpointRounding.AwayFromZero);
                        }
                        else if (c > 18)
                        {
                            returnValue = value * Convert.ToDecimal(BasicNextStepFactor);
                            returnValue = Math.Round(returnValue, MidpointRounding.AwayFromZero);
                            value = returnValue;

                        }

                        dt.Rows[r][c] = returnValue;
                    }


                }
                else if (SalaryTypeName.ToLower() == "Houserent".ToLower())
                {
                    decimal value = Convert.ToDecimal(dt.Rows[r]["CurrentBasic"].ToString());
                    decimal HRent = 0;
                    for (int c = 18; c < dt.Columns.Count; c++)
                    {
                        if (c == 18)
                        {
                            HRent = value * Convert.ToDecimal(dt.Rows[r]["HouseRentFactor"].ToString()) / 100;
                            HRent = Math.Round(HRent, MidpointRounding.AwayFromZero);
                            dt.Rows[r][c] = HRent;
                        }
                        else if (c > 18)
                        {
                            returnValue = value * Convert.ToDecimal(BasicNextStepFactor);
                            returnValue = Math.Round(returnValue, MidpointRounding.AwayFromZero);
                            value = returnValue;
                            HRent = value * Convert.ToDecimal(dt.Rows[r]["HouseRentFactor"].ToString()) / 100;
                            HRent = Math.Round(HRent, MidpointRounding.AwayFromZero);
                            dt.Rows[r][c] = HRent;
                        }
                    }
                }
                else if (SalaryTypeName.ToLower() == "Medical".ToLower())
                {
                    decimal Medical = 0;
                    for (int c = 18; c < dt.Columns.Count; c++)
                    {
                        Medical = Convert.ToDecimal(dt.Rows[r]["MedicalFactor"].ToString());
                        Medical = Math.Round(Medical, MidpointRounding.AwayFromZero);
                        dt.Rows[r][c] = Medical;
                    }
                }
                else if (SalaryTypeName.ToLower() == "Conveyance".ToLower())
                {
                    decimal TA = 0;
                    for (int c = 18; c < dt.Columns.Count; c++)
                    {
                        TA = Convert.ToDecimal(dt.Rows[r]["TAFactor"].ToString());
                        TA = Math.Round(TA, MidpointRounding.AwayFromZero);
                        dt.Rows[r][c] = TA;
                    }
                }
            }

            return dt;
        }

        public DataTable SelectSalaryStructureMatrixForProcessX(string CurrentYear="",string CurrentYearPart = "")
        {
            #region Variables
            SqlConnection currConn = null; SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Insert");
                }
                #region sql statement
                sqlText = @"select 
m.Id
,m.CurrentYear
,m.SL
,g.id GradeId
,g.Area, g.Code, g.Name
,G.BasicNextYearFactor
,G.BasicNextStepFactor
,isnull(G.IsHouseRentFactorFromBasic,0)IsHouseRentFactorFromBasic
,G.HouseRentFactor
,isnull(G.IsTAFactorFromBasic,0)IsTAFactorFromBasic
,G.TAFactor
,isnull(G.IsMedicalFactorFromBasic,0)IsMedicalFactorFromBasic
,G.GradeNo
,G.MedicalFactor
,m.SalaryTypeName
,G.CurrentBasic
,0 Step1Amount 
,0 Step1_5Amount
,0 Step2Amount
,0 Step2_5Amount
,0 Step3Amount
,0 Step3_5Amount
,0 Step4Amount
,0 Step4_5Amount
,0 Step5Amount
,0 Step5_5Amount
,0 Step6Amount
,0 Step6_5Amount
,0 Step7Amount
,0 Step7_5Amount
,0 Step8Amount
,0 Step8_5Amount
,0 Step9Amount
,0 Step9_5Amount
,0 Step10Amount
,0 Step10_5Amount
,0 Step11Amount
,0 Step11_5Amount
,0 Step12Amount
,0 Step12_5Amount
,0 Step13Amount
,0 Step13_5Amount
,0 Step14Amount
,0 Step14_5Amount
,0 Step15Amount
,0 Step15_5Amount
,0 Step16Amount
,0 Step16_5Amount
,0 Step17Amount
,0 Step17_5Amount
,0 Step18Amount
,0 Step18_5Amount
,0 Step19Amount
,0 Step19_5Amount
,0 Step20Amount
,0 Step20_5Amount
,0 Step21Amount
,0 Step21_5Amount
,0 Step22Amount
,0 Step22_5Amount
,0 Step23Amount
,0 Step23_5Amount
,0 Step24Amount
,0 Step24_5Amount
,0 Step25Amount
,0 Step25_5Amount
,0 Step26Amount
,0 Step26_5Amount
,0 Step27Amount
,0 Step27_5Amount
,0 Step28Amount
,0 Step28_5Amount
,0 Step29Amount
,0 Step29_5Amount
,0 Step30Amount
,0 Step30_5Amount
,0 Step31Amount
,0 Step31_5Amount

 from SalaryStructureMatrix m
left outer join Grade g on m.GradeId=g.Id
where CurrentYear=@CurrentYear";
//                if(!string.IsNullOrEmpty(CurrentYearPart))
//                {
//                     sqlText += @"
//And isnull(CurrentYearPart,'A')=@CurrentYearPart";
//                }

                  sqlText += @"
 and g.IsActive=1
--order by g.GRADEnO, Area,g.SL,SalaryTypeName";

                  SqlDataAdapter adapter = new SqlDataAdapter();
                  adapter.SelectCommand = new SqlCommand(sqlText, currConn);
                  adapter.SelectCommand.Transaction = transaction;
                  adapter.SelectCommand.Parameters.AddWithValue("@CurrentYear", CurrentYear);


                //SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                //da.SelectCommand.Parameters.AddWithValue("@CurrentYear", CurrentYear);
                  adapter.Fill(ds);

                   dt = ds.Tables[0].Copy();


                #endregion

                #region Commit
                
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                

                #endregion Commit
            }
            #region catch
           catch (Exception ex)
            {
                transaction.Rollback();

                throw new ArgumentNullException("", ex.Message);

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
        public DataTable SelectSalaryStructureMatrixForProcess(string CurrentYear = "", string CurrentYearPart = "")
        {
            #region Variables
            SqlConnection currConn = null; SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Insert");
                }
                #region sql statement
                sqlText = @"
SELECT  
G.Id
, @CurrentYear CurrentYear
,G.SL
,g.id GradeId
,g.Area, g.Code, g.Name
,G.BasicNextYearFactor
,G.BasicNextStepFactor
,isnull(G.IsHouseRentFactorFromBasic,0)IsHouseRentFactorFromBasic
,G.HouseRentFactor
,isnull(G.IsTAFactorFromBasic,0)IsTAFactorFromBasic
,G.TAFactor
,isnull(G.IsMedicalFactorFromBasic,0)IsMedicalFactorFromBasic
,G.GradeNo
,G.MedicalFactor
,SalaryTypeName
,G.CurrentBasic
,0 Step1Amount 
,0 Step1_5Amount
,0 Step2Amount
,0 Step2_5Amount
,0 Step3Amount
,0 Step3_5Amount
,0 Step4Amount
,0 Step4_5Amount
,0 Step5Amount
,0 Step5_5Amount
,0 Step6Amount
,0 Step6_5Amount
,0 Step7Amount
,0 Step7_5Amount
,0 Step8Amount
,0 Step8_5Amount
,0 Step9Amount
,0 Step9_5Amount
,0 Step10Amount
,0 Step10_5Amount
,0 Step11Amount
,0 Step11_5Amount
,0 Step12Amount
,0 Step12_5Amount
,0 Step13Amount
,0 Step13_5Amount
,0 Step14Amount
,0 Step14_5Amount
,0 Step15Amount
,0 Step15_5Amount
,0 Step16Amount
,0 Step16_5Amount
,0 Step17Amount
,0 Step17_5Amount
,0 Step18Amount
,0 Step18_5Amount
,0 Step19Amount
,0 Step19_5Amount
,0 Step20Amount
,0 Step20_5Amount
,0 Step21Amount
,0 Step21_5Amount
,0 Step22Amount
,0 Step22_5Amount
,0 Step23Amount
,0 Step23_5Amount
,0 Step24Amount
,0 Step24_5Amount
,0 Step25Amount
,0 Step25_5Amount
,0 Step26Amount
,0 Step26_5Amount
,0 Step27Amount
,0 Step27_5Amount
,0 Step28Amount
,0 Step28_5Amount
,0 Step29Amount
,0 Step29_5Amount
,0 Step30Amount
,0 Step30_5Amount
,0 Step31Amount
,0 Step31_5Amount
 
FROM Grade G
CROSS JOIN (VALUES ('Conveyance'), ('HouseRent'), ('Medical'), ('Basic')) AS SalaryTypes(SalaryTypeName)
where g.IsActive=1

";
                //                if(!string.IsNullOrEmpty(CurrentYearPart))
                //                {
                //                     sqlText += @"
                //And isnull(CurrentYearPart,'A')=@CurrentYearPart";
                //                }

                sqlText += @"
order by g.GRADEnO, Area,g.SL,SalaryTypeName";

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(sqlText, currConn);
                adapter.SelectCommand.Transaction = transaction;
                adapter.SelectCommand.Parameters.AddWithValue("@CurrentYear", CurrentYear);


                //SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                //da.SelectCommand.Parameters.AddWithValue("@CurrentYear", CurrentYear);
                adapter.Fill(ds);

                dt = ds.Tables[0].Copy();


                #endregion

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }


                #endregion Commit
            }
            #region catch
            catch (Exception ex)
            {
                transaction.Rollback();

                throw new ArgumentNullException("", ex.Message);

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


        public DataTable SelectSalaryStructureMatrixDownload(string CurrentYear = "", string CurrentYearPart = "")
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
                sqlText = @"select 
m.CurrentYear
,Isnull(m.CurrentYearPart,'A')CurrentYearPart
,g.Area, g.Code, g.Name
,m.SalaryTypeName  
,Step1Amount       [Step 1]
,Step1_5Amount     [Step 1.5]
,Step2Amount       [Step 2]
,Step2_5Amount     [Step 2.5]
,Step3Amount       [Step 3]
,Step3_5Amount     [Step 3.5]
,Step4Amount       [Step 4]
,Step4_5Amount     [Step 4.5]
,Step5Amount       [Step 5]
,Step5_5Amount     [Step 5.5]
,Step6Amount       [Step 6]
,Step6_5Amount     [Step 6.5]
,Step7Amount       [Step 7]
,Step7_5Amount     [Step 7.5]
,Step8Amount       [Step 8]
,Step8_5Amount     [Step 8.5]
,Step9Amount       [Step 9]
,Step9_5Amount     [Step 9.5]
,Step10Amount      [Step 10]
,Step10_5Amount    [Step 10.5]
,Step11Amount      [Step 11]
,Step11_5Amount    [Step 11.5]
,Step12Amount      [Step 12]
,Step12_5Amount    [Step 12.5]
,Step13Amount      [Step 13]
,Step13_5Amount    [Step 13.5]
,Step14Amount      [Step 14]
,Step14_5Amount    [Step 14.5]
,Step15Amount      [Step 15]
,Step15_5Amount    [Step 15.5]
,Step16Amount      [Step 16]
,Step16_5Amount    [Step 16.5]
,Step17Amount      [Step 17]
,Step17_5Amount    [Step 17.5]
,Step18Amount      [Step 18]
,Step18_5Amount    [Step 18.5]
,Step19Amount      [Step 19]
,Step19_5Amount    [Step 19.5]
,Step20Amount      [Step 20]
 from SalaryStructureMatrix m
left outer join Grade g on m.GradeId=g.Id
where CurrentYear=@CurrentYear";
                if (!string.IsNullOrEmpty(CurrentYearPart))
                {
                    sqlText += @" 
 and Isnull(CurrentYearPart,'A')=@CurrentYearPart";
                }

                     sqlText += @" 
 and g.IsActive=1
 and g.CurrentBasic>0
order by g.GRADEnO, Area,g.SL 
, case 
when m.SalaryTypeName='Basic' then '1' 
when m.SalaryTypeName='HouseRent' then '2' 
when m.SalaryTypeName='Conveyance' then '3' 
when m.SalaryTypeName='Medical' then '4' 
else 99
end 

";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Parameters.AddWithValue("@CurrentYear", CurrentYear);
                if (!string.IsNullOrEmpty(CurrentYearPart))
                {
                    da.SelectCommand.Parameters.AddWithValue("@CurrentYearPart", CurrentYearPart);
                }
                da.Fill(dt);
                #endregion
            }
            #region catch
            catch (Exception ex)
            {
                throw new ArgumentNullException("", ex.Message);

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




        //==================Update =================
        public string[] Update(SalaryStructureMatrixVM VM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "SalaryStructureMatrixVM Update"; //Method SalaryTypeName
             SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;

            string sqlText = "";

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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                if (VM != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update SalaryStructureMatrix set";
                    sqlText += " SalaryTypeName=@SalaryTypeName";
                    sqlText += " ,GradeId=@GradeId";
                    sqlText +=" ,Step1Amount=@Step1Amount";
                    sqlText +=" ,Step2Amount=@Step2Amount";
                    sqlText +=" ,Step3Amount=@Step3Amount";
                    sqlText +=" ,Step4Amount=@Step4Amount";
                    sqlText +=" ,Step5Amount=@Step5Amount";
                    sqlText +=" ,Step6Amount=@Step6Amount";
                    sqlText +=" ,Step7Amount=@Step7Amount";
                    sqlText +=" ,Step8Amount=@Step8Amount";
                    sqlText +=" ,Step9Amount=@Step9Amount";
                    sqlText +=" ,Step10Amount=@Step10Amount";
                    sqlText +=" ,Step11Amount=@Step11Amount";
                    sqlText +=" ,Step12Amount=@Step12Amount";
                    sqlText +=" ,Step13Amount=@Step13Amount";
                    sqlText +=" ,Step14Amount=@Step14Amount";
                    sqlText +=" ,Step15Amount=@Step15Amount";
                    sqlText +=" ,Step16Amount=@Step16Amount";
                    sqlText +=" ,Step17Amount=@Step17Amount";
                    sqlText +=" ,Step18Amount=@Step18Amount";
                    sqlText +=" ,Step19Amount=@Step19Amount";
                    sqlText +=" ,Step20Amount=@Step20Amount";
                    sqlText +=" ,Step21Amount=@Step21Amount";
                    sqlText +=" ,Step22Amount=@Step22Amount";
                    sqlText +=" ,Step23Amount=@Step23Amount";
                    sqlText +=" ,Step24Amount=@Step24Amount";
                    sqlText +=" ,Step25Amount=@Step25Amount";
                    sqlText +=" ,Step26Amount=@Step26Amount";
                    sqlText +=" ,Step27Amount=@Step27Amount";
                    sqlText +=" ,Step28Amount=@Step28Amount";
                    sqlText +=" ,Step29Amount=@Step29Amount";
                    sqlText +=" ,Step30Amount=@Step30Amount";
                    sqlText += " ,Remarks=@Remarks";
                    sqlText += " ,IsActive=@IsActive";
                    sqlText += " ,IsArchive=@IsArchive";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", VM.Id);
                    cmdUpdate.Parameters.AddWithValue("@SalaryTypeName", VM.SalaryTypeName);
                    cmdUpdate.Parameters.AddWithValue("@GradeId", VM.GradeId);
                     cmdUpdate.Parameters.AddWithValue("@Step1Amount", VM.Step1Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step2Amount", VM.Step2Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step3Amount", VM.Step3Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step4Amount", VM.Step4Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step5Amount", VM.Step5Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step6Amount", VM.Step6Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step7Amount", VM.Step7Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step8Amount", VM.Step8Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step9Amount", VM.Step9Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step10Amount", VM.Step10Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step11Amount", VM.Step11Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step12Amount", VM.Step12Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step13Amount", VM.Step13Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step14Amount", VM.Step14Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step15Amount", VM.Step15Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step16Amount", VM.Step16Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step17Amount", VM.Step17Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step18Amount", VM.Step18Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step19Amount", VM.Step19Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step20Amount", VM.Step20Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step21Amount", VM.Step21Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step22Amount", VM.Step22Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step23Amount", VM.Step23Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step24Amount", VM.Step24Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step25Amount", VM.Step25Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step26Amount", VM.Step26Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step27Amount", VM.Step27Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step28Amount", VM.Step28Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step29Amount", VM.Step29Amount);
                     cmdUpdate.Parameters.AddWithValue("@Step30Amount", VM.Step30Amount);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", VM.Remarks ?? Convert.DBNull);//, vm.Remarks?? Convert.DBNull);//, vm.Remarks?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", VM.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", VM.IsArchive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", VM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", VM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", VM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = VM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", ProjectVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Structure Matrix Update", "Could not found any item.");
                }
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to update Salary Structure Matrix.";
                    throw new ArgumentNullException("", "");
                }

            }
            #region catch
            catch (Exception ex)
            {
                //throw new ArgumentNullException("", "");

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



        //==================Delete =================
        public string[] Delete(SalaryStructureMatrixVM VM, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete Salary Structure Matrix"; //Method SalaryTypeName

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryStructureMatrix"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update SalaryStructureMatrix set";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy",VM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt",VM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom",VM.LastUpdateFrom);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }

                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Salary Structure Matrix Delete", VM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("SalaryStructureMatrix Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete SalaryStructureMatrix Information.";
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
        public List<SalaryStructureMatrixVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryStructureMatrixVM> VMs = new List<SalaryStructureMatrixVM>();
            SalaryStructureMatrixVM vm;
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
Id,
SalaryTypeName
   FROM SalaryStructureMatrix
WHERE IsArchive=0 and IsActive=1
    ORDER BY SalaryTypeName
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryStructureMatrixVM();
                    vm.Id = dr["Id"].ToString();
                    vm.SalaryTypeName = dr["SalaryTypeName"].ToString();
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

        public List<SalaryStructureMatrixVM> CurrentYearPartDropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryStructureMatrixVM> VMs = new List<SalaryStructureMatrixVM>();
            SalaryStructureMatrixVM vm;
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

                sqlText = @"SELECT Distinct
CurrentYearPart
   FROM SalaryStructureMatrix
 WHERE CurrentYearPart is not null
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryStructureMatrixVM();
                    vm.CurrentYearPart = dr["CurrentYearPart"].ToString();
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


        public List<string> Autocomplete(string term)
        {

            #region Variables

            SqlConnection currConn = null;
            List<string> vms = new List<string>();

            string sqlText = "";

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
                sqlText = "";
                sqlText = @"SELECT Id, SalaryTypeName    FROM SalaryStructureMatrix ";
                sqlText += @" WHERE SalaryTypeName like '%" + term + "%' and IsArchive=0 and IsActive=1 ORDER BY SalaryTypeName";



                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["SalaryTypeName"].ToString());
                    i++;
                }
                dr.Close();
                vms.Sort();
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

        public string BasicAmount(string Grade, string Step, string year, string StepSL="",string yearpart="")
        {

            #region Variables
            string result = "";
            string StepName = "";
            SqlConnection currConn = null;

            string sqlText = "";

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

                string stepSL = StepSL;
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandType = CommandType.Text;
                if (string.IsNullOrWhiteSpace((stepSL)))
                {
                    sqlText = @"select SL from EnumSalaryStep
                where ID = @ID";
                    _objComm.Parameters.AddWithValue("@ID", Step);
                    _objComm.CommandText = sqlText;

                    stepSL = _objComm.ExecuteScalar().ToString();
                }
                  
               

               StepName = "Step" + stepSL.Replace(".", "_") + "Amount";
                #region sql statement
                //SalaryTypeName='basic' and
                sqlText = @"select  "+StepName+" from SalaryStructureMatrix ";
                sqlText += @"where   gradeid='" + Grade + "' and Currentyear ='"+year+"'";
                if (!string.IsNullOrEmpty(yearpart))
                {
                    sqlText += @"and Isnull(CurrentyearPart,'A') ='" + yearpart + "'";

                }
                sqlText += @" order by case when SalaryTypeName='Basic' then 'A'
 when SalaryTypeName='HouseRent' then 'B'
 when SalaryTypeName='Medical' then 'C'
 when SalaryTypeName='Conveyance' then 'D' end ";

                _objComm.CommandText = sqlText;


                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    result += dr[StepName] + "~";// +" ~ " + dr["Name"].ToString();// vms.Insert(i, dr["Code"].ToString() + " - " + dr["Name"].ToString());
                }
                dr.Close();
                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                return result;
               // throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                return result;
                //throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return result.TrimEnd('~');
        }

        public SalaryStructureMatrixVM StructureMatrix(string Grade, string stepSL, string CurrentYear, string SalaryTypeName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {

            #region Variables
            string result = "";
            string StepName = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            SalaryStructureMatrixVM vm = new SalaryStructureMatrixVM();
            string sqlText = "";
            DataTable dt = new DataTable();
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSalaryStructureMatrix"); }

                #endregion open connection and transaction
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandType = CommandType.Text;
                 

                StepName = "Step" + stepSL.Replace(".", "_") + "Amount";
                #region sql statement
                //SalaryTypeName='basic' and
                sqlText = @"select  CurrentYear, SalaryTypeName, " + StepName + " CurrentAmount from SalaryStructureMatrix ";
                sqlText += @"where   gradeid='" + Grade + "' and CurrentYear='" + CurrentYear + "' and SalaryTypeName='" + SalaryTypeName + "'";


                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.Transaction = transaction;
                objComm.CommandType = CommandType.Text;
               

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dt);
                foreach (DataRow item in dt.Rows)
                {
                    vm.CurrentAmount = Convert.ToDecimal(item["CurrentAmount"]);
                }
 
                #endregion
            }
            #region catch


            catch (SqlException sqlex)
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
                return vm;
                // throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
                return vm;
                //throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion
            #region Finally
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

            return vm;
        }

        public List<SalaryStructureMatrixVM> SelectFiscalYearMonthlies(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SalaryStructureMatrixVM> VMs = new List<SalaryStructureMatrixVM>();
            SalaryStructureMatrixVM vm;
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
                SettingDAL _settingDal = new SettingDAL();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim();

                #region sql statement
                #region SqlText
                sqlText = @"
select distinct CurrentYear ,Isnull(CurrentYearPart,'A')CurrentYearPart from SalaryStructureMatrix
WHERE  1=1 
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
                    vm = new SalaryStructureMatrixVM();
                    vm.CurrentYear = dr["CurrentYear"].ToString();
                    vm.CurrentYearPart = dr["CurrentYearPart"].ToString();
                   
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
            return VMs;
        }


        #endregion
    }
}
