using SymOrdinary;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Payroll
{
    public class SalaryStructureDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<SalaryStructureVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryStructureVM> salaryStructureVMs = new List<SalaryStructureVM>();
            SalaryStructureVM salaryStructureVM;
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
,Code
,Name
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


    From SalaryStructure
Where IsArchive=0
    ORDER BY Name
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        salaryStructureVM = new SalaryStructureVM();
                        salaryStructureVM.Id = dr["Id"].ToString();
                        salaryStructureVM.Code = dr["Code"].ToString();
                        salaryStructureVM.Name = dr["Name"].ToString();
                        salaryStructureVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                        salaryStructureVM.Remarks = dr["Remarks"].ToString();
                        salaryStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        salaryStructureVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        salaryStructureVM.CreatedBy = dr["CreatedBy"].ToString();
                        salaryStructureVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        salaryStructureVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        salaryStructureVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        salaryStructureVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                        salaryStructureVMs.Add(salaryStructureVM);
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

            return salaryStructureVMs;
        }
        //==================SelectByID=================
        public SalaryStructureVM SelectById(string Id)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SalaryStructureVM salaryStructureVM = new SalaryStructureVM();

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
            SqlTransaction transaction = currConn.BeginTransaction("");

            #endregion open connection and transaction

            try
            {


                #region sql statement

                sqlText = @"SELECT
Id
,Code
,Name
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
    From SalaryStructure
where  id=@Id
     
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                objComm.Transaction = transaction;
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        salaryStructureVM.Id = dr["Id"].ToString();
                        salaryStructureVM.Code = dr["Code"].ToString();
                        salaryStructureVM.Name = dr["Name"].ToString();
                        salaryStructureVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                        salaryStructureVM.Remarks = dr["Remarks"].ToString();
                        salaryStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        salaryStructureVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        salaryStructureVM.CreatedBy = dr["CreatedBy"].ToString();
                        salaryStructureVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        salaryStructureVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        salaryStructureVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        salaryStructureVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    }
                    dr.Close();
                }
                salaryStructureVM.salaryStructureDetailVMs = SalaryStructureDetails("e",salaryStructureVM.Id, currConn, transaction);
                salaryStructureVM.salaryStructureDeductionDetailVMs = SalaryStructureDetails("d",salaryStructureVM.Id, currConn, transaction);
                transaction.Commit();
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return salaryStructureVM;
        }

        public List<SalaryStructureDetailVM> SalaryStructureDetails(string isearning, string salaryStructureId, SqlConnection con, SqlTransaction transaction)
        {
            #region Variables

            string sqlText = "";
            List<SalaryStructureDetailVM> salaryStructureDVMs = new List<SalaryStructureDetailVM>();
            SalaryStructureDetailVM salaryStructureDVM;
            #endregion
            try
            {

                #region sql statement

                sqlText = @"SELECT
Id
,SalaryTypeId
,IsFixed
,Portion
,PortionSalaryType
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

    From SalaryStructureDetail
Where IsArchive=0 And SalaryStructureId=@SalaryStructureId
";
                if (!string.IsNullOrWhiteSpace(isearning))
                {
                    sqlText += @" and IsEarning=@IsEarning ";
                }

               
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = con;
                objComm.Transaction = transaction;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(isearning))
                {
                    if (isearning.ToLower()=="e")
                    {
                        objComm.Parameters.AddWithValue("@IsEarning", 1);
                    }
                    else if (isearning.ToLower() == "d")
                    {
                        objComm.Parameters.AddWithValue("@IsEarning", 0);
                    }

                }
                objComm.Parameters.AddWithValue("@SalaryStructureId", salaryStructureId);
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        salaryStructureDVM = new SalaryStructureDetailVM();
                        salaryStructureDVM.Id = Convert.ToInt32(dr["Id"]);
                        salaryStructureDVM.SalaryTypeId = dr["SalaryTypeId"].ToString();
                        salaryStructureDVM.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                        salaryStructureDVM.Portion = Convert.ToDecimal(dr["Portion"]);
                        salaryStructureDVM.PortionSalaryType = dr["PortionSalaryType"].ToString();
                        salaryStructureDVM.Remarks = dr["Remarks"].ToString();
                        salaryStructureDVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        salaryStructureDVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        salaryStructureDVM.CreatedBy = dr["CreatedBy"].ToString();
                        salaryStructureDVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        salaryStructureDVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        salaryStructureDVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        salaryStructureDVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                        salaryStructureDVMs.Add(salaryStructureDVM);
                    }
                    dr.Close();
                }



                #endregion
            }
            #region catch
            catch (Exception ex)
            {
            }

            #endregion

            return salaryStructureDVMs;

        }

        //==================Insert =================
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

                if (salaryStructureVM.salaryStructureDetailVMs != null)
                {
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

                            //if (item.IsFixed == false && item.Portion < 0)
                            //{
                            //    retResults[1] = " Salary Portion must Not Less than Zero (0)";
                            //    throw new ArgumentNullException("retResults[1], "");
                            //}


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
                }
                #endregion Earning

                #region Deduction
                if (salaryStructureVM.salaryStructureDeductionDetailVMs != null)
                {

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
                            if (item.IsFixed == false && item.Portion < 0)
                            {
                                retResults[1] = "Salary Portion must Not Less than Zero (0)";
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
                }
                #endregion Deduction
                #region update SalaryType
                sqlText = "  ";
                sqlText += @" update SalaryStructureDetail set IsGross=est.IsGross, SalaryType=est.TypeName
                                        from EnumSalaryType est
                                        where est.Id=SalaryStructureDetail.SalaryTypeId
                                        and SalaryStructureId=@SalaryStructureId ";
                SqlCommand cmdEJ1 = new SqlCommand(sqlText, currConn);

                cmdEJ1.Parameters.AddWithValue("@SalaryStructureId", salaryStructureVM.Id);
                cmdEJ1.Transaction = transaction;
                cmdEJ1.ExecuteNonQuery();


                #endregion update SalaryType

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

        public string[] InsertBackup(SalaryStructureVM salaryStructureVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
                if (exists >0)
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

                    if (salaryStructureVM.salaryStructureDetailVMs.Count <= 0)
                    {
                        retResults[1] = "This Salary Structure Have no Detail List";
                        throw new ArgumentNullException("This Salary Structure Have no Detail List", "");
                    }
                    else
                    {
                        #region
                        string estBasicId = string.Empty;
                        string estGrossId = string.Empty;
                            #region Basic
                        sqlText = "Select id from EnumSalaryType where TypeName in('basic') and BranchId=@BranchId";
                            SqlCommand cmdBId = new SqlCommand(sqlText, currConn);
                            cmdBId.Parameters.AddWithValue("@BranchId", salaryStructureVM.BranchId);
                            cmdBId.Transaction = transaction;

                            object objBId = cmdBId.ExecuteScalar();
                            if (objBId == null)
                            {
                                retResults[1] = "Basic Type salary not in System";
                                throw new ArgumentNullException("Basic Type salary not in System", "");
                            }
                            else
                            { 
                                estBasicId = objBId.ToString();
                            }
                         
                            var HaveBasic=salaryStructureVM.salaryStructureDetailVMs.Where(x=>x.SalaryTypeId==estBasicId);

                            if (HaveBasic == null || !HaveBasic.Any())
	                    {
		                     retResults[1] = "Please select Basic Type salary Head In Detail";
                             throw new ArgumentNullException("Please select Basic Type salary Head In Detail", "");
                        }
                            var HaveBasicFixed = salaryStructureVM.salaryStructureDetailVMs.Where(x =>
                                   x.SalaryTypeId == estBasicId
                                  && x.IsFixed == true
                                   );

                            if (HaveBasicFixed.Any())
                            {
                                retResults[1] = "Basic Type salary Head In Detail Not Fixed";
                                throw new ArgumentNullException("Basic Type salary Head In Detail Not Fixed", "");
                            }



                            #endregion Basic
                            #region Gross
                            sqlText = "Select id from EnumSalaryType where TypeName in('gross') and BranchId=@BranchId";
                            SqlCommand cmdGId = new SqlCommand(sqlText, currConn);
                            cmdGId.Parameters.AddWithValue("@BranchId", salaryStructureVM.BranchId);
                            cmdGId.Transaction = transaction;

                            object objGId = cmdGId.ExecuteScalar();
                            if (objGId == null)
                            {
                                retResults[1] = "Gross Type salary not in System";
                                throw new ArgumentNullException("Gross Type salary not in System", "");
                            }
                            else
                            {
                                estGrossId = objGId.ToString();
                            }

                            var HaveGross = salaryStructureVM.salaryStructureDetailVMs.Where(x => x.SalaryTypeId == estGrossId);

                            if (HaveGross == null || !HaveGross.Any())
                            {
                                retResults[1] = "Please select Gross Type salary Head In Detail";
                                throw new ArgumentNullException("Please select Gross Type salary Head In Detail", "");
                            }
                            var HaveGrossFixed = salaryStructureVM.salaryStructureDetailVMs.Where(x =>
                                x.SalaryTypeId == estGrossId
                               && x.IsFixed == true
                                );

                            if ( !HaveGrossFixed.Any())
                            {
                                retResults[1] = "Gross Type salary Head In Detail must Fixed";
                                throw new ArgumentNullException(" Gross Type salary Head In Detail must Fixed", "");
                            }

                            #endregion Gross
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
                            if (item.IsFixed == false && string.IsNullOrWhiteSpace(item.PortionSalaryType))
                            {
                                retResults[1] = "Only Fixed Item allow without salary From";
                                throw new ArgumentNullException("Only Fixed Item allow without salary From", "");
                            }
                            string SalaryType = "";
                            sqlText = "Select TypeName,IsEarning from EnumSalaryType where   BranchId=@BranchId and id=@id";
                            SqlCommand cmdGId1 = new SqlCommand(sqlText, currConn);
                            cmdGId1.Parameters.AddWithValue("@BranchId", salaryStructureVM.BranchId);
                            cmdGId1.Parameters.AddWithValue("@id", item.SalaryTypeId);
                            cmdGId1.Transaction = transaction;

                            object objST = cmdGId1.ExecuteScalar();
                            if (objST == null)
                            {
                                retResults[1] = "salaryType not in System";
                                throw new ArgumentNullException("salaryType not in System", "");
                            }
                            else
                            {
                                SalaryType = objST.ToString();
                            }
                            if (SalaryType.ToLower() != "gross" && item.Portion < 0)
                            {
                                retResults[1] = "Without gross Salary Portion must Not Less than Zero (0)";
                                throw new ArgumentNullException(retResults[1], "");
                            }

                            if (SalaryType.ToLower() == "basic" && item.PortionSalaryType.ToLower() == "basic")
                            {
                                retResults[1] = "Basic salary Not from Basic Type salary Head In Detail";
                                throw new ArgumentNullException("Basic salary Not from Basic Type salary Head In Detail", "");
                            }

                            sqlText = "  ";
                            sqlText += @" INSERT INTO SalaryStructureDetail(
                                        IsFixed,Portion,PortionSalaryType,SalaryType,SalaryTypeId,SalaryStructureId,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                        )  VALUES (
                                        @IsFixed,@Portion,@PortionSalaryType,@SalaryType,@SalaryTypeId,@SalaryStructureId,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                        ) ";

                            SqlCommand cmdD = new SqlCommand(sqlText, currConn);

                            cmdD.Parameters.AddWithValue("@IsFixed", item.IsFixed);
                            cmdD.Parameters.AddWithValue("@Portion", item.Portion);
                            cmdD.Parameters.AddWithValue("@PortionSalaryType", item.PortionSalaryType ?? Convert.DBNull);
                            cmdD.Parameters.AddWithValue("@SalaryTypeId", item.SalaryTypeId ?? Convert.DBNull);
                            cmdD.Parameters.AddWithValue("@SalaryStructureId", salaryStructureVM.Id);
                            cmdD.Parameters.AddWithValue("@SalaryType", SalaryType);

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
        //==================Update =================
        public string[] Update(SalaryStructureVM salaryStructureVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee SalaryStructure Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSalaryStructure"); }

                #endregion open connection and transaction


                if (salaryStructureVM != null)
                {
                    #region Update Header
                    sqlText = "  ";
                    sqlText += " SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) Id FROM SalaryStructure ";
                    sqlText += " WHERE Name=@Name And Id<>@Id";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", salaryStructureVM.Id);
                    cmdExist.Parameters.AddWithValue("@Code", salaryStructureVM.Code);
                    cmdExist.Parameters.AddWithValue("@Name", salaryStructureVM.Name);
					var exeRes = cmdExist.ExecuteScalar();
					int exists = Convert.ToInt32(exeRes);
                    if (exists > 0)
                    {
                        retResults[1] = "This Salary Structure already used";
                        throw new ArgumentNullException("This Salary Structure already used", "");
                    }

                    sqlText = @"Update SalaryStructure set
                            Code=@Code
                            ,Name=@Name
                            ,Remarks=@Remarks
                            ,LastUpdateBy=@LastUpdateBy
                            ,LastUpdateAt=@LastUpdateAt
                            ,LastUpdateFrom=@LastUpdateFrom
                            where Id=@Id
                    ";

                    SqlCommand cmdHUpdate = new SqlCommand();
                    cmdHUpdate.Connection = currConn;
                    cmdHUpdate.CommandType = CommandType.Text;
                    cmdHUpdate.Transaction = transaction;
                    cmdHUpdate.CommandText = sqlText;

                    cmdHUpdate.Parameters.AddWithValue("@Id", salaryStructureVM.Id);

                    cmdHUpdate.Parameters.AddWithValue("@Code", salaryStructureVM.Code);
                    cmdHUpdate.Parameters.AddWithValue("@Name", salaryStructureVM.Name);

                    cmdHUpdate.Parameters.AddWithValue("@Remarks", salaryStructureVM.Remarks ?? Convert.DBNull);
                    cmdHUpdate.Parameters.AddWithValue("@LastUpdateBy", salaryStructureVM.LastUpdateBy);
                    cmdHUpdate.Parameters.AddWithValue("@LastUpdateAt", salaryStructureVM.LastUpdateAt);
                    cmdHUpdate.Parameters.AddWithValue("@LastUpdateFrom", salaryStructureVM.LastUpdateFrom);

                    cmdHUpdate.ExecuteNonQuery();
                    #endregion Update Header

                    #region Delete Details
                    sqlText = "delete SalaryStructureDetail where SalaryStructureId='" + salaryStructureVM.Id + "'";
                    SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                    cmdDelete.Transaction = transaction;
                    cmdDelete.ExecuteNonQuery();
                    #endregion Delete Details
 
                            #region Earning
                            if (salaryStructureVM.salaryStructureDetailVMs != null)

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
                                        if (item.IsFixed == false && item.Portion < 0)
                                        {
                                            retResults[1] = " Salary Portion must Not Less than Zero (0)";
                                            throw new ArgumentNullException(retResults[1], "");
                                        }

                                        sqlText = "  ";
                                        sqlText += @" INSERT INTO SalaryStructureDetail(
                                        IsFixed,Portion,PortionSalaryType,SalaryType,IsEarning,SalaryTypeId,SalaryStructureId,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                        )  VALUES (
                                        @IsFixed,@Portion,@PortionSalaryType,@SalaryType,@IsEarning,@SalaryTypeId,@SalaryStructureId,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                        ) ";

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
                                        //cmdD.Parameters.AddWithValue("@PortionSalaryType", item.PortionSalaryType ?? Convert.DBNull);

                                        cmdD.Parameters.AddWithValue("@SalaryTypeId", item.SalaryTypeId ?? Convert.DBNull);
                                        cmdD.Parameters.AddWithValue("@SalaryStructureId", salaryStructureVM.Id);
                                        cmdD.Parameters.AddWithValue("@SalaryType", "Other");
                                        cmdD.Parameters.AddWithValue("@IsEarning", true);

                                        cmdD.Parameters.AddWithValue("@Remarks", salaryStructureVM.Remarks ?? Convert.DBNull);
                                        cmdD.Parameters.AddWithValue("@IsActive", true);
                                        cmdD.Parameters.AddWithValue("@IsArchive", false);
                                        cmdD.Parameters.AddWithValue("@CreatedBy", salaryStructureVM.LastUpdateBy);
                                        cmdD.Parameters.AddWithValue("@CreatedAt", salaryStructureVM.LastUpdateAt);
                                        cmdD.Parameters.AddWithValue("@CreatedFrom", salaryStructureVM.LastUpdateFrom);

                                        cmdD.Transaction = transaction;
                                        cmdD.ExecuteNonQuery();
                                    }
                                    #endregion
                            }
                            #endregion Earning

                            #region Deduction
                            if (salaryStructureVM.salaryStructureDeductionDetailVMs!=null)
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

                                   if (item.IsFixed == false && item.Portion < 0)
                                    {
                                        retResults[1] = "Salary Portion must Not Less than Zero (0)";
                                        throw new ArgumentNullException(retResults[1], "");
                                    }

                                    sqlText = "  ";
                                    sqlText += @" INSERT INTO SalaryStructureDetail(
                                        IsFixed,Portion,PortionSalaryType,SalaryType,IsEarning,SalaryTypeId,SalaryStructureId,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                                        )  VALUES (
                                        @IsFixed,@Portion,@PortionSalaryType,@SalaryType,@IsEarning,@SalaryTypeId,@SalaryStructureId,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                        ) ";

                                    SqlCommand cmdD = new SqlCommand(sqlText, currConn);

                                    cmdD.Parameters.AddWithValue("@IsFixed", item.IsFixed);
                                    cmdD.Parameters.AddWithValue("@Portion", item.Portion);
                                    if (item.IsFixed == false)
                                    {
                                        cmdD.Parameters.AddWithValue("@PortionSalaryType","Basic");
                                    }
                                    else
                                    {
                                        cmdD.Parameters.AddWithValue("@PortionSalaryType", Convert.DBNull);
                                    }
                                    cmdD.Parameters.AddWithValue("@SalaryTypeId", item.SalaryTypeId ?? Convert.DBNull);
                                    cmdD.Parameters.AddWithValue("@SalaryStructureId", salaryStructureVM.Id);
                                    cmdD.Parameters.AddWithValue("@SalaryType", "Other");
                                    cmdD.Parameters.AddWithValue("@IsEarning", false);

                                    cmdD.Parameters.AddWithValue("@Remarks", salaryStructureVM.Remarks ?? Convert.DBNull);
                                    cmdD.Parameters.AddWithValue("@IsActive", true);
                                    cmdD.Parameters.AddWithValue("@IsArchive", false);
                                    cmdD.Parameters.AddWithValue("@CreatedBy", salaryStructureVM.LastUpdateBy);
                                    cmdD.Parameters.AddWithValue("@CreatedAt", salaryStructureVM.LastUpdateAt);
                                    cmdD.Parameters.AddWithValue("@CreatedFrom", salaryStructureVM.LastUpdateFrom);

                                    cmdD.Transaction = transaction;
                                    cmdD.ExecuteNonQuery();
                                }
                                #endregion
                            }
                            #endregion Deduction
                            #region update SalaryType
                            sqlText = "  ";
                            sqlText += @" update SalaryStructureDetail set IsGross=est.IsGross, SalaryType=est.TypeName
                                        from EnumSalaryType est
                                        where est.Id=SalaryStructureDetail.SalaryTypeId
                                        and SalaryStructureId=@SalaryStructureId ";
                             SqlCommand cmdEJ1 = new SqlCommand(sqlText, currConn);

                            cmdEJ1.Parameters.AddWithValue("@SalaryStructureId", salaryStructureVM.Id);
                            cmdEJ1.Transaction = transaction;
                            cmdEJ1.ExecuteNonQuery();


                            #endregion update SalaryType
              

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("SalaryStructure Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update SalaryStructure.";
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return retResults;
        }

        //==================Delete =================
        public string[] Delete(SalaryStructureVM vm, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Salary Structure Delete"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length > 0)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update SalaryStructure set";
                        sqlText += " IsArchive=@IsArchive";
                        sqlText += " ,LastUpdateBy=@LastUpdateBy";
                        sqlText += " ,LastUpdateAt=@LastUpdateAt";
                        sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);


                        sqlText = "";
                        sqlText = "update SalaryStructureDetail set";
                        sqlText += " IsArchive=@IsArchive";
                        sqlText += " ,LastUpdateBy=@LastUpdateBy";
                        sqlText += " ,LastUpdateAt=@LastUpdateAt";
                        sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where SalaryStructureId=@Id";

                        SqlCommand cmdUpdate2 = new SqlCommand(sqlText, currConn);
                        cmdUpdate2.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate2.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate2.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate2.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate2.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                        cmdUpdate.ExecuteNonQuery();
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Salary Type Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Salary Structure  Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Project Infromation.";
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


        public List<SalaryStructureVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SalaryStructureVM> VMs = new List<SalaryStructureVM>();
            SalaryStructureVM vm;
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
Name
   FROM SalaryStructure
WHERE IsArchive=0 and IsActive=1
    ORDER BY Name
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SalaryStructureVM();
                    vm.Id = dr["Id"].ToString();
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
        #endregion
    }
}
