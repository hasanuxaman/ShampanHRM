using SymOrdinary;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.HRM
{
    public class EmployeePromotionDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeePromotionVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePromotionVM> VMs = new List<EmployeePromotionVM>();
            EmployeePromotionVM vm;
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
 p.Id
,p.EmployeeId
,p.DesignationId
,p.IsPromotion
,p.PromotionDate
,p.IsCurrent
,p.Remarks
,p.FileName
,p.IsActive
,p.IsArchive
,p.CreatedBy
,p.CreatedAt
,p.CreatedFrom
,p.LastUpdateBy
,p.LastUpdateAt
,p.LastUpdateFrom
,d.Name DesignationName
,p.GradeId
,g.Name Grade
    From EmployeePromotion p
	left outer join Designation d on d.Id=p.DesignationId
	left outer join Grade g on g.Id=p.GradeId
Where p.IsArchive=0
    ORDER BY p.Id DESC
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeePromotionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.IsPromotion = Convert.ToBoolean(dr["IsPromotion"]);
                    vm.PromotionDate =  Ordinary.StringToDate(dr["PromotionDate"].ToString());
                    vm.IsCurrent = Convert.ToBoolean(dr["IsCurrent"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.FileName = dr["FileName"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.DesignationName = dr["DesignationName"].ToString();
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
        //==================SelectAllByEmployee=================
        public List<EmployeePromotionVM> SelectAllByEmployee(string employeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePromotionVM> employeePromotionVMs = new List<EmployeePromotionVM>();
            EmployeePromotionVM employeePromotionVM;
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
 p.Id
,p.EmployeeId
,p.DesignationId
,d.Name DesignationName
,p.IsPromotion
,P.GradeId
,gr.Name GradeName
,p.PromotionDate
,p.IsCurrent
,p.Remarks
,p.FileName
,p.IsActive
,p.IsArchive
,p.CreatedBy
,p.CreatedAt
,p.CreatedFrom
,p.LastUpdateBy
,p.LastUpdateAt
,p.LastUpdateFrom

    From EmployeePromotion p
	left outer join Designation d on d.Id=p.DesignationId
	left outer join Grade gr on gr.Id = p.GradeId
Where p.IsArchive=0 AND p.EmployeeId=@EmployeeId
    ORDER BY p.Id DESC
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId",employeeId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeePromotionVM = new EmployeePromotionVM();
                    employeePromotionVM.Id = Convert.ToInt32(dr["Id"]);
                    employeePromotionVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeePromotionVM.DesignationId = dr["DesignationId"].ToString();
                    employeePromotionVM.DesignationName = dr["DesignationName"].ToString();
                    employeePromotionVM.IsPromotion = Convert.ToBoolean(dr["IsPromotion"]);
                    employeePromotionVM.GradeId =dr["GradeId"].ToString();
                    employeePromotionVM.GradeName = dr["GradeName"].ToString();
                    employeePromotionVM.PromotionDate = Ordinary.StringToDate(dr["PromotionDate"].ToString());
                    employeePromotionVM.IsCurrent = Convert.ToBoolean(dr["IsCurrent"]);
                    employeePromotionVM.Remarks = dr["Remarks"].ToString();
                    employeePromotionVM.FileName = dr["FileName"].ToString();
                    employeePromotionVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeePromotionVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeePromotionVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeePromotionVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeePromotionVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeePromotionVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeePromotionVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeePromotionVMs.Add(employeePromotionVM);
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

            return employeePromotionVMs;
        }
        //==================SelectByID=================
        public EmployeePromotionVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePromotionVM employeePromotionVM = new EmployeePromotionVM();

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
 p.Id
,p.EmployeeId
,p.DesignationId
,d.Name DesignationName
,p.GradeId
,p.StepId
,g.Name GradeName
,p.IsPromotion
,p.PromotionDate

,p.IsCurrent
,p.Remarks
,p.FileName
,p.IsActive
,p.IsArchive
,p.CreatedBy
,p.CreatedAt
,p.CreatedFrom
,p.LastUpdateBy
,p.LastUpdateAt
,p.LastUpdateFrom

    From EmployeePromotion p
	left outer join Designation d on d.Id=p.DesignationId
	left outer join Grade g on g.Id=p.GradeId
where  p.id=@Id
     
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
                    employeePromotionVM.Id = Convert.ToInt32(dr["Id"]);
                    employeePromotionVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeePromotionVM.DesignationId = dr["DesignationId"].ToString();
                    employeePromotionVM.DesignationName = dr["DesignationName"].ToString();

                    employeePromotionVM.GradeId = dr["GradeId"].ToString();
                    employeePromotionVM.StepId = dr["StepId"].ToString();
                    employeePromotionVM.GradeName = dr["GradeName"].ToString();
                    employeePromotionVM.IsPromotion = Convert.ToBoolean(dr["IsPromotion"]);
                    employeePromotionVM.PromotionDate = Ordinary.StringToDate(dr["PromotionDate"].ToString());
                    employeePromotionVM.IsCurrent = Convert.ToBoolean(dr["IsCurrent"]);
                    employeePromotionVM.Remarks = dr["Remarks"].ToString();
                    employeePromotionVM.FileName = dr["FileName"].ToString();
                    employeePromotionVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeePromotionVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeePromotionVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeePromotionVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeePromotionVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeePromotionVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeePromotionVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeePromotionVM;
        }
        public bool CheckPromotionDate(string employeeId,string date)
        {

            #region Variables

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

                #region sql statement

                sqlText = @"
select j.joinDate ,p.promotiondate
from employeejob j
left outer join EmployeePromotion p on p.employeeId=j.employeeId and p.isCurrent=1 and p.IsArchive=0
where j.employeeId=@employeeId
     
";
                string joinDate = "";
                string lastpromotionDate = "";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@employeeId", employeeId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    joinDate = dr["joinDate"].ToString();
                    lastpromotionDate = dr["promotiondate"].ToString();
                }
                dr.Close();
                if (string.IsNullOrWhiteSpace(lastpromotionDate))
                {
                    if (!string.IsNullOrWhiteSpace(joinDate))
                    {
                        if (Convert.ToInt32(joinDate)<=Convert.ToInt32(Ordinary.DateToString(date)))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (Convert.ToInt32(lastpromotionDate) <= Convert.ToInt32(Ordinary.DateToString(date)))
                    {
                        return true;
                    }
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

            return false;
        }
        //==================SelectByID=================
        public EmployeePromotionVM SelectByEmployeeCurrent(string employeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePromotionVM employeePromotionVM = new EmployeePromotionVM();

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

                sqlText = @"SELECT TOP 1
 p.Id
,p.EmployeeId
,p.DesignationId
,p.GradeId
,p.StepId
,p.IsPromotion
,p.PromotionDate
,p.IsCurrent
,p.Remarks
,p.FileName
,p.IsActive
,p.IsArchive
,p.CreatedBy
,p.CreatedAt
,p.CreatedFrom
,p.LastUpdateBy
,p.LastUpdateAt
,p.LastUpdateFrom
,d.Name DesignationName
    From EmployeePromotion p
	left outer join Designation d on d.Id=p.DesignationId
where  p.EmployeeId=@EmployeeId AND p.IsCurrent=@IsCurrent
 ORDER BY p.Id DESC
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", employeeId);
                objComm.Parameters.AddWithValue("@IsCurrent", true);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeePromotionVM.Id = Convert.ToInt32(dr["Id"]);
                    employeePromotionVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeePromotionVM.DesignationId = dr["DesignationId"].ToString();
                    employeePromotionVM.GradeId = dr["GradeId"].ToString();
                    employeePromotionVM.StepId = dr["StepId"].ToString();
                    employeePromotionVM.IsPromotion = Convert.ToBoolean(dr["IsPromotion"]);
                    employeePromotionVM.PromotionDate =  Ordinary.StringToDate(dr["PromotionDate"].ToString());
                    employeePromotionVM.IsCurrent = Convert.ToBoolean(dr["IsCurrent"]);
                    employeePromotionVM.Remarks = dr["Remarks"].ToString();
                    employeePromotionVM.FileName = dr["FileName"].ToString();
                    employeePromotionVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeePromotionVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeePromotionVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeePromotionVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeePromotionVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeePromotionVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeePromotionVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeePromotionVM.DesignationName = dr["DesignationName"].ToString();

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

            return employeePromotionVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeePromotionVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeePromotion"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(employeePromotionVM.Name))
                //{
                //    retResults[1] = "Please Input Employee Nominee Name";
                //    return retResults;
                //}
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
                sqlText = " update EmployeePromotion set IsCurrent=0 where EmployeeId =@EmployeeId";
                SqlCommand cmdExist0 = new SqlCommand(sqlText, currConn);
                cmdExist0.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist0.Transaction = transaction;
                try
                {
                    cmdExist0.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new ArgumentNullException("Promotion is not complete", "");
                }
                //int foundId = (int)objfoundId;
                if (1 == 1)
                {

                    #region Inactive Promotion History
                    sqlText = "";
                    sqlText = @"
UPDATE EmployeePromotion SET 
IsActive = 0
WHERE EmployeeId=@EmployeeId
";
                    SqlCommand cmdUpdateHistory = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdateHistory.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdateHistory.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                    var exec = cmdUpdateHistory.ExecuteNonQuery();
                    int transResult = Convert.ToInt32(exec);
                    #endregion

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeePromotion
(	
EmployeeId
,DesignationId
,GradeId
,IsPromotion
,PromotionDate
,IsCurrent
,FileName
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) 
VALUES (
@EmployeeId
,@DesignationId
,@GradeId
,@IsPromotion
,@PromotionDate
,@IsCurrent
,@FileName
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@GradeId", vm.GradeId);
                    cmdInsert.Parameters.AddWithValue("@IsPromotion", vm.IsPromotion);
                    cmdInsert.Parameters.AddWithValue("@PromotionDate", Ordinary.DateToString(vm.PromotionDate));
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);
                    cmdInsert.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    cmdInsert.Transaction = transaction;
					var exeRes = cmdInsert.ExecuteScalar();
					Id = Convert.ToInt32(exeRes);

                    #region Update EmployeeJob
                    EmployeeJobVM ejVM = new EmployeeJobVM();
                    ejVM.EmployeeId = vm.EmployeeId;
                    ejVM.DesignationId = vm.DesignationId;

                    ejVM.LastUpdateBy = vm.CreatedBy;
                    ejVM.LastUpdateAt= vm.CreatedAt;
                    ejVM.LastUpdateFrom = vm.CreatedFrom;

                    retResults = new EmployeeJobDAL().Update_EmployeeJob_Designation(ejVM, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Present Address  Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Present Address  Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Present Address Bangla already used";
                    throw new ArgumentNullException("Please Input Employee Present Address  Value", "");
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
        public string[] Update(EmployeePromotionVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Present Address  Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToPromotion"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Inactive Promotion History 
                    sqlText = "";
                    sqlText = @"
UPDATE EmployeePromotion SET 
IsActive = 0
WHERE EmployeeId=@EmployeeId
AND Id<>@Id
";
                    SqlCommand cmdUpdateHistory = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdateHistory.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdateHistory.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                    var exec = cmdUpdateHistory.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exec);
                    #endregion

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeePromotion set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " DesignationId=@DesignationId,";
                    sqlText += " GradeId=@GradeId,";
                    sqlText += " IsPromotion=@IsPromotion,";
                    sqlText += " PromotionDate=@PromotionDate,";
                    sqlText += " IsCurrent=@IsCurrent,";
                    if (vm.FileName !=null)
                    {
                        sqlText += " FileName=@FileName,";
                    }
                    
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdUpdate.Parameters.AddWithValue("@GradeId", vm.GradeId);
                    cmdUpdate.Parameters.AddWithValue("@IsPromotion", vm.IsPromotion);
                    cmdUpdate.Parameters.AddWithValue("@PromotionDate", Ordinary.DateToString(vm.PromotionDate));
                    cmdUpdate.Parameters.AddWithValue("@IsCurrent", true);
                    if (vm.FileName != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    }
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);


                    #region Update EmployeeJob
                    EmployeeJobVM ejVM = new EmployeeJobVM();
                    ejVM.EmployeeId = vm.EmployeeId;
                    ejVM.DesignationId = vm.DesignationId;

                    ejVM.LastUpdateBy = vm.LastUpdateBy;
                    ejVM.LastUpdateAt = vm.LastUpdateAt;
                    ejVM.LastUpdateFrom = vm.LastUpdateFrom;

                    retResults = new EmployeeJobDAL().Update_EmployeeJob_Designation(ejVM, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion

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
                    throw new ArgumentNullException("Promotion  Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Promotion .";
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
        //==================Select =================
        public EmployeePromotionVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeePromotionVM employeePromotionVM = new EmployeePromotionVM();

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
                sqlText = @"SELECT Top 1 
p.Id
,p.EmployeeId
,p.DesignationId
,p.IsPromotion
,p.PromotionDate
,p.IsCurrent
,p.FileName
,p.Remarks
,p.IsActive
,p.IsArchive
,p.CreatedBy
,p.CreatedAt
,p.CreatedFrom
,p.LastUpdateBy
,p.LastUpdateAt
,p.LastUpdateFrom
,d.Name DesignationName
    From EmployeePromotion p
	left outer join Designation d on d.Id=p.DesignationId
    From EmployeePromotion 
";
                if (query == null)
                {
                    if (Id != 0)
                    {
                        sqlText += " AND Id=@Id";
                    }
                    else
                    {
                        sqlText += " ORDER BY Id ";
                    }
                }
                else
                {
                    if (query == "FIRST")
                    {
                        sqlText += " ORDER BY Id ";
                    }
                    else if (query == "LAST")
                    {
                        sqlText += " ORDER BY Id DESC";
                    }
                    else if (query == "NEXT")
                    {
                        sqlText += " and  Id > @Id   ORDER BY Id";
                    }
                    else if (query == "PREVIOUS")
                    {
                        sqlText += "  and  Id < @Id   ORDER BY Id DESC";
                    }
                }


                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (Id != null)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                try
                {
                    dr = objComm.ExecuteReader();
                    while (dr.Read())
                    {
                        employeePromotionVM.Id = Convert.ToInt32(dr["Id"]);
                        employeePromotionVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeePromotionVM.DesignationId = dr["DesignationId"].ToString();
                        employeePromotionVM.IsPromotion = Convert.ToBoolean(dr["IsPromotion"]);
                        employeePromotionVM.PromotionDate =  Ordinary.StringToDate(dr["PromotionDate"].ToString());
                        employeePromotionVM.IsCurrent = Convert.ToBoolean(dr["IsCurrent"]);
                        employeePromotionVM.FileName = dr["FileName"].ToString();
                        employeePromotionVM.Remarks = dr["Remarks"].ToString();
                        employeePromotionVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeePromotionVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeePromotionVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeePromotionVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeePromotionVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeePromotionVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeePromotionVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                        employeePromotionVM.DesignationName = dr["DesignationName"].ToString();
                    }
                    dr.Close();
                }
                catch (Exception ex)
                {
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

            return employeePromotionVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeePromotionVM EmployeePromotionVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeletePromotion"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToPromotion"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length>=1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeePromotion set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " IsActive=@IsActive,";
                        
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeePromotionVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeePromotionVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeePromotionVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }
                    

                    retResults[2] ="";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Promotion Delete", EmployeePromotionVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Promotion  Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Promotion .";
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

        //==================SelectAllForReport=================
        public List<EmployeePromotionVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePromotionVM> VMs = new List<EmployeePromotionVM>();
            EmployeePromotionVM vm;
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
 isnull(p.Id, 0)										Id
,ei.EmployeeId											EmployeeId
,isnull(p.DesignationId			, 'NA')					DesignationId
,isnull(p.IsPromotion			, 0)					IsPromotion
,isnull(p.PromotionDate			, 'NA')					PromotionDate
,isnull(p.IsCurrent				, 0)					IsCurrent
,isnull(p.Remarks				, 'NA')					Remarks
,isnull(p.[FileName]			, 'NA')					[FileName]
,isnull(p.IsActive				, 0)					IsActive
,isnull(p.IsArchive				, 0)					IsArchive
,isnull(p.CreatedBy				, 'NA')					CreatedBy
,isnull(p.CreatedAt				, 'NA')					CreatedAt
,isnull(p.CreatedFrom			, 'NA')					CreatedFrom
,isnull(p.LastUpdateBy			, 'NA')					LastUpdateBy
,isnull(p.LastUpdateAt			, 'NA')					LastUpdateAt
,isnull(p.LastUpdateFrom		, 'NA')					LastUpdateFrom
,isnull(d.Name 	, 'NA')									Designation
,isnull(p.GradeId	, 'NA')										GradeId
,isnull(g.Name 		, 'NA')										Grade
     
	From ViewEmployeeInformation ei
	left outer join EmployeePromotion p on ei.EmployeeId=p.EmployeeId	 
	left outer join Designation d on d.Id=p.DesignationId
	left outer join Grade g on g.Id=p.GradeId
Where ei.IsArchive=0 and ei.isActive=1 and p.IsArchive=0 and p.isActive=1
";

                if (CodeF != "0_0")
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and ei.DepartmentId=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and ei.DesignationId=@DesignationId";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and ei.ProjectId=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and ei.SectionId=@SectionId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ei.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ei.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and ei.EmployeeId=@EmployeeId";
                }

                sqlText += "  order by ei.Department, ei.GradeSL, ei.joindate, ei.Code ";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeePromotionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.DesignationName = dr["Designation"].ToString();
                    vm.GradeId = dr["GradeId"].ToString();
                    vm.GradeName = dr["Grade"].ToString();
                    vm.IsPromotion = Convert.ToBoolean(dr["IsPromotion"]);
                    vm.PromotionDate = Ordinary.StringToDate(dr["PromotionDate"].ToString());
                    vm.IsCurrent = Convert.ToBoolean(dr["IsCurrent"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.FileName = dr["FileName"].ToString();
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

        public DataTable Report(EmployeePromotionVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
                sqlText = @"
SELECT
 isnull(p.Id, 0)										Id
,p.EmployeeId											EmployeeId
,isnull(p.DesignationId			, 'NA')					DesignationId
,isnull(p.IsPromotion			, 0)					IsPromotion
,isnull(p.PromotionDate			, '19000101')			PromotionDate
,isnull(p.IsCurrent				, 0)					IsCurrent
,isnull(p.Remarks				, 'NA')					Remarks
,isnull(d.Name 	, 'NA')									Designation
,isnull(p.GradeId	, 'NA')										GradeId
,isnull(g.Name 		, 'NA')										Grade
,case when isnull(p.IsCurrent, 0) = 0 then 'Previous'	else	'Current' end as PromotionStatus
     
	From 
EmployeePromotion p
left outer join  EmployeeInfo ei on ei.Id=p.EmployeeId
left outer join EmployeeJob ej on ej.EmployeeId=ei.Id
left outer join Designation d on d.Id=p.DesignationId
left outer join Grade g on g.Id=p.GradeId
Where 1=1 and ei.isActive=1 
and ej.JoinDate < p.PromotionDate
";
                #region More Conditions
                if (vm.EmployeeIdList != null && vm.EmployeeIdList.Count > 0)
                {
                    string MultipleEmployeeId = "";
                    foreach (var item in vm.EmployeeIdList)
                    {
                        MultipleEmployeeId += "'" + item + "',";
                    }
                    MultipleEmployeeId = MultipleEmployeeId.Remove(MultipleEmployeeId.Length - 1);
                    sqlText += " AND ei.Id IN(" + MultipleEmployeeId + ")";
                }
                #endregion
                #region ConditionFields
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
                #endregion ConditionFields
                sqlText += " ORDER By ei.Code, PromotionStatus";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                #region ConditionValues
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
                #endregion ConditionValues
                da.Fill(dt);

                string[] dateColumnChange = { "PromotionDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, dateColumnChange);
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
    }
}
