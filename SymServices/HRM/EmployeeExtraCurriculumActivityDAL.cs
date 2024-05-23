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
    public class EmployeeExtraCurriculumActivityDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeExtraCurriculumActivityVM> SelectAll()
        {           

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeExtraCurriculumActivityVM> VMs = new List<EmployeeExtraCurriculumActivityVM>();
            EmployeeExtraCurriculumActivityVM vm;
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
,EmployeeId
,Skill
,YearsOfExperience
,SkillQuality_E
,Institute  
,Address    
,Date       
,Achievement
,FileName
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeExtraCurriculumActivities
Where IsArchive=0
    ORDER BY Skill
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeExtraCurriculumActivityVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Skill = dr["Skill"].ToString();
                    vm.YearsOfExperience = Convert.ToDecimal(dr["YearsOfExperience"]);
                    vm.SkillQuality_E = dr["SkillQuality_E"].ToString();
                    vm.FileName = dr["FileName"].ToString();
                    vm.Institute = dr["Institute"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.Date = Ordinary.StringToDate(dr["Date"].ToString());
                    vm.Achievement = dr["Achievement"].ToString();
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
        public List<EmployeeExtraCurriculumActivityVM> SelectAllByEmployee(string employeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeExtraCurriculumActivityVM> employeeExtraCurriculumActivityVMs = new List<EmployeeExtraCurriculumActivityVM>();
            EmployeeExtraCurriculumActivityVM employeeExtraCurriculumActivityVM;
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
,EmployeeId
,Skill
,YearsOfExperience
,SkillQuality_E
,Institute  
,Address    
,Date       
,Achievement
,FileName
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeExtraCurriculumActivities
Where IsArchive=0 and EmployeeId=@EmployeeId
    ORDER BY Skill
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", employeeId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeExtraCurriculumActivityVM = new EmployeeExtraCurriculumActivityVM();
                    employeeExtraCurriculumActivityVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeExtraCurriculumActivityVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeExtraCurriculumActivityVM.Skill = dr["Skill"].ToString();
                    employeeExtraCurriculumActivityVM.YearsOfExperience = Convert.ToDecimal(dr["YearsOfExperience"]);
                    employeeExtraCurriculumActivityVM.SkillQuality_E = dr["SkillQuality_E"].ToString();
                    employeeExtraCurriculumActivityVM.Institute = dr["Institute"].ToString();
                    employeeExtraCurriculumActivityVM.Address = dr["Address"].ToString();
                    employeeExtraCurriculumActivityVM.Date = Ordinary.StringToDate(dr["Date"].ToString());
                    employeeExtraCurriculumActivityVM.Achievement = dr["Achievement"].ToString();
                    employeeExtraCurriculumActivityVM.FileName = dr["FileName"].ToString();
                    employeeExtraCurriculumActivityVM.Remarks = dr["Remarks"].ToString();
                    employeeExtraCurriculumActivityVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeExtraCurriculumActivityVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeExtraCurriculumActivityVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeExtraCurriculumActivityVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeExtraCurriculumActivityVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeExtraCurriculumActivityVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeExtraCurriculumActivityVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeExtraCurriculumActivityVMs.Add(employeeExtraCurriculumActivityVM);
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

            return employeeExtraCurriculumActivityVMs;
        }
        //==================SelectByID=================
        public EmployeeExtraCurriculumActivityVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeExtraCurriculumActivityVM employeeExtraCurriculumActivityVM = new EmployeeExtraCurriculumActivityVM();

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
,EmployeeId
,Skill
,YearsOfExperience
,SkillQuality_E
,Institute  
,Address    
,Date       
,Achievement
,FileName
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeExtraCurriculumActivities
where  id=@Id
     
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
                    employeeExtraCurriculumActivityVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeExtraCurriculumActivityVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeExtraCurriculumActivityVM.Skill = dr["Skill"].ToString();
                    employeeExtraCurriculumActivityVM.YearsOfExperience = Convert.ToDecimal(dr["YearsOfExperience"]);
                    employeeExtraCurriculumActivityVM.SkillQuality_E = dr["SkillQuality_E"].ToString();
                    employeeExtraCurriculumActivityVM.Institute = dr["Institute"].ToString();
                    employeeExtraCurriculumActivityVM.Address = dr["Address"].ToString();
                    employeeExtraCurriculumActivityVM.Date = Ordinary.StringToDate(dr["Date"].ToString());
                    employeeExtraCurriculumActivityVM.Achievement = dr["Achievement"].ToString();
                    employeeExtraCurriculumActivityVM.FileName = dr["FileName"].ToString();
                    employeeExtraCurriculumActivityVM.Remarks = dr["Remarks"].ToString();
                    employeeExtraCurriculumActivityVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeExtraCurriculumActivityVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeExtraCurriculumActivityVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeExtraCurriculumActivityVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeExtraCurriculumActivityVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeExtraCurriculumActivityVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeExtraCurriculumActivityVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeeExtraCurriculumActivityVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeeExtraCurriculumActivityVM employeeExtraCurriculumActivityVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmergencyContact"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(EmployeeExtraCurriculumActivityVM))
                //{
                //    retResults[1] = "Please Input Employee Degree_E";
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

                #region Exist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeExtraCurriculumActivities ";
                sqlText += " WHERE EmployeeId=@EmployeeId and Skill=@Skill";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", employeeExtraCurriculumActivityVM.EmployeeId);
                cmdExist.Parameters.AddWithValue("@Skill", employeeExtraCurriculumActivityVM.Skill);
                object objfoundId = cmdExist.ExecuteScalar();

                if (objfoundId == null)
                {
                    retResults[1] = "Please Input Employee ExtraCurriculam Value";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Input Employee ExtraCurriculam Value", "");
                }
                #endregion Exist
                #region Save

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeExtraCurriculumActivities(	
                    EmployeeId,Remarks,Skill,YearsOfExperience,SkillQuality_E,FileName,Institute,Address,Date,Achievement
                    ,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom)";
                    sqlText += @" VALUES (@EmployeeId,@Remarks,@Skill,@YearsOfExperience,@SkillQuality_E,@FileName,@Institute,@Address,@Date,@Achievement
                    ,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", employeeExtraCurriculumActivityVM.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Remarks", employeeExtraCurriculumActivityVM.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Skill", employeeExtraCurriculumActivityVM.Skill ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@YearsOfExperience", employeeExtraCurriculumActivityVM.YearsOfExperience );
                    cmdInsert.Parameters.AddWithValue("@SkillQuality_E", employeeExtraCurriculumActivityVM.SkillQuality_E ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FileName", employeeExtraCurriculumActivityVM.FileName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Institute", employeeExtraCurriculumActivityVM.Institute ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Address", employeeExtraCurriculumActivityVM.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Date", Ordinary.DateToString(employeeExtraCurriculumActivityVM.Date));
                    cmdInsert.Parameters.AddWithValue("@Achievement", employeeExtraCurriculumActivityVM.Achievement ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", employeeExtraCurriculumActivityVM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", employeeExtraCurriculumActivityVM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", employeeExtraCurriculumActivityVM.CreatedFrom);

                    cmdInsert.Transaction = transaction;
					var exeRes = cmdInsert.ExecuteScalar();
					Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee ExtraCurriculam Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee ExtraCurriculam Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee ExtraCurriculam already used";
                    throw new ArgumentNullException("This Employee ExtraCurriculam already used", "");
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
                retResults[1] = "Data Save Successfully";
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
        public string[] Update(EmployeeExtraCurriculumActivityVM employeeExtraCurriculumActivityVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee ExtraCurriculamUpdate"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmergencyContactUpdate"); }

                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeExtraCurriculumActivities ";
                sqlText += " WHERE EmployeeId=@EmployeeId  AND Id<>@Id AND Skill=@Skill";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Id", employeeExtraCurriculumActivityVM.Id);
                cmdExist.Parameters.AddWithValue("@EmployeeId", employeeExtraCurriculumActivityVM.EmployeeId);
                cmdExist.Parameters.AddWithValue("@Skill", employeeExtraCurriculumActivityVM.Skill);
                
				var exeRes = cmdExist.ExecuteScalar();
				int exists = Convert.ToInt32(exeRes);
                if (exists > 0)
                {
                    retResults[1] = "This ExtraCurriculam already used";
                    throw new ArgumentNullException("This Employee ExtraCurriculam already used", "");
                }
                #endregion Exist

                if (employeeExtraCurriculumActivityVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeExtraCurriculumActivities set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " Skill=@Skill,";
                    sqlText += " YearsOfExperience=@YearsOfExperience,";
                    sqlText += " SkillQuality_E=@SkillQuality_E,";
                    if (!string.IsNullOrWhiteSpace(employeeExtraCurriculumActivityVM.FileName))
                    {
                        sqlText += " FileName=@FileName,";
                    }
                    sqlText += " Institute=@Institute,";
                    sqlText += " Address=@Address,";
                    sqlText += " Date=@Date,";
                    sqlText += " Achievement=@Achievement,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", employeeExtraCurriculumActivityVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", employeeExtraCurriculumActivityVM.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@Skill", employeeExtraCurriculumActivityVM.Skill ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@YearsOfExperience", employeeExtraCurriculumActivityVM.YearsOfExperience);
                    cmdUpdate.Parameters.AddWithValue("@SkillQuality_E", employeeExtraCurriculumActivityVM.SkillQuality_E ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", employeeExtraCurriculumActivityVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Institute", employeeExtraCurriculumActivityVM.Institute ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Address", employeeExtraCurriculumActivityVM.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Date", Ordinary.DateToString(employeeExtraCurriculumActivityVM.Date));
                    cmdUpdate.Parameters.AddWithValue("@Achievement", employeeExtraCurriculumActivityVM.Achievement ?? Convert.DBNull);
                    if (!string.IsNullOrWhiteSpace(employeeExtraCurriculumActivityVM.FileName))
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", employeeExtraCurriculumActivityVM.FileName ?? Convert.DBNull);
                    }
                    cmdUpdate.Parameters.AddWithValue("@IsActive", employeeExtraCurriculumActivityVM.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeExtraCurriculumActivityVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeExtraCurriculumActivityVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeExtraCurriculumActivityVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = employeeExtraCurriculumActivityVM.Id.ToString();// Return Id
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
                    throw new ArgumentNullException("ExtraCurriculam Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update ExtraCurriculam.";
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
//        public EmployeeExtraCurriculumActivityVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
//        {

//            #region Variables

//            SqlConnection currConn = null;
//            string sqlText = "";

//            EmployeeExtraCurriculumActivityVM EmployeeExtraCurriculumActivityVM = new EmployeeExtraCurriculumActivityVM();

//            #endregion

//            try
//            {
//                #region open connection and transaction

//                currConn = _dbsqlConnection.GetConnection();
//                if (currConn.State != ConnectionState.Open)
//                {
//                    currConn.Open();
//                }

//                #endregion open connection and transaction

//                #region sql statement
//                sqlText = @"SELECT Top 1 
//Id
//,EmployeeId
//,Remarks
//,IsActive
//,CreatedAt 
//,CreatedBy 
//,CreatedFrom 
//,LastUpdateAt
//,LastUpdateBy
//,LastUpdateFrom
//FROM EmployeeExtraCurriculumActivities    
//";
//                if (query == null)
//                {
//                    if (Id != 0)
//                    {
//                        sqlText += " AND Id=@Id";
//                    }
//                    else
//                    {
//                        sqlText += " ORDER BY Id ";
//                    }
//                }
//                else
//                {
//                    if (query == "FIRST")
//                    {
//                        sqlText += " ORDER BY Id ";
//                    }
//                    else if (query == "LAST")
//                    {
//                        sqlText += " ORDER BY Id DESC";
//                    }
//                    else if (query == "NEXT")
//                    {
//                        sqlText += " and  Id > @Id   ORDER BY Id";
//                    }
//                    else if (query == "PREVIOUS")
//                    {
//                        sqlText += "  and  Id < @Id   ORDER BY Id DESC";
//                    }
//                }


//                SqlCommand objComm = new SqlCommand();
//                objComm.Connection = currConn;
//                objComm.CommandText = sqlText;
//                objComm.CommandType = CommandType.Text;
//                if (Id != null)
//                {
//                    objComm.Parameters.AddWithValue("@Id", Id);
//                }
//                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
//                //dataAdapter.Fill(dataSet);
//                SqlDataReader dr;
//                try
//                {
//                    dr = objComm.ExecuteReader();
//                    while (dr.Read())
//                    {
//                        EmployeeExtraCurriculumActivityVM.Id = Convert.ToInt32(dr["Id"]);
//                        EmployeeExtraCurriculumActivityVM.EmployeeId = dr["EmployeeId"].ToString();
//                        EmployeeExtraCurriculumActivityVM.Remarks = dr["Remarks"].ToString();
//                        EmployeeExtraCurriculumActivityVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
//                        EmployeeExtraCurriculumActivityVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
//                        EmployeeExtraCurriculumActivityVM.CreatedBy = dr["CreatedBy"].ToString();
//                        EmployeeExtraCurriculumActivityVM.CreatedFrom = dr["CreatedFrom"].ToString();
//                        EmployeeExtraCurriculumActivityVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
//                        EmployeeExtraCurriculumActivityVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
//                        EmployeeExtraCurriculumActivityVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
//                    }
//                    dr.Close();
//                }
//                catch (Exception ex)
//                {
//                }

//                #endregion
//            }
//            #region catch


//            catch (SqlException sqlex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
//            }
//            catch (Exception ex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
//            }

//            #endregion
//            #region finally

//            finally
//            {
//                if (currConn != null)
//                {
//                    if (currConn.State == ConnectionState.Open)
//                    {
//                        currConn.Close();
//                    }
//                }
//            }

//            #endregion

//            return EmployeeExtraCurriculumActivityVM;
//        }
        //==================Delete =================
        public string[] Delete(EmployeeExtraCurriculumActivityVM employeeExtraCurriculumActivityVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EducationDelete"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEducation"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length> 0)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeExtraCurriculumActivities set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeExtraCurriculumActivityVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeExtraCurriculumActivityVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeExtraCurriculumActivityVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }
 

                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Education Delete", employeeExtraCurriculumActivityVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Contact Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Contact Information.";
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
        public List<EmployeeExtraCurriculumActivityVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeExtraCurriculumActivityVM> VMs = new List<EmployeeExtraCurriculumActivityVM>();
            EmployeeExtraCurriculumActivityVM vm;
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
 isnull(ExCurr.Id,0)						  Id
,ei.EmployeeId							EmployeeId
,isnull(ExCurr.Skill	, 'NA')			Skill
,isnull(ExCurr.YearsOfExperience	, 0)		YearsOfExperience
,isnull(ExCurr.SkillQuality_E	, 'NA')		SkillQuality_E
,isnull(ExCurr.Institute  	, 'NA')		Institute  
,isnull(ExCurr.[Address]    	, 'NA')		[Address]   
,isnull(ExCurr.[Date]       	, 'NA')			[Date]       
,isnull(ExCurr.Achievement	, 'NA')			Achievement
,isnull(ExCurr.[FileName]		,'NA')		[FileName]
,isnull(ExCurr.Remarks		, 'NA')			Remarks
,isnull(ExCurr.IsActive, 0)			IsActive
,isnull(ExCurr.IsArchive, 0)			IsArchive
,isnull(ExCurr.CreatedBy, 'NA')		 CreatedBy
,isnull(ExCurr.CreatedAt, 'NA')		 CreatedAt
,isnull(ExCurr.CreatedFrom, 'NA')		CreatedFrom
,isnull(ExCurr.LastUpdateBy, 'NA')	 LastUpdateBy
,isnull(ExCurr.LastUpdateAt,	'NA')	 LastUpdateAt
,isnull(ExCurr.LastUpdateFrom,	'NA')	 LastUpdateFrom

    From ViewEmployeeInformation ei
		left outer join EmployeeExtraCurriculumActivities ExCurr on ei.EmployeeId=ExCurr.EmployeeId
Where ei.IsArchive=0 and ei.isActive=1 and ExCurr.IsArchive=0 and ExCurr.isActive=1
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

                sqlText += ", ExCurr.Skill";

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
                    vm = new EmployeeExtraCurriculumActivityVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Skill = dr["Skill"].ToString();
                    vm.YearsOfExperience = Convert.ToDecimal(dr["YearsOfExperience"]);
                    vm.SkillQuality_E = dr["SkillQuality_E"].ToString();
                    vm.FileName = dr["FileName"].ToString();
                    vm.Institute = dr["Institute"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.Date = Ordinary.StringToDate(dr["Date"].ToString());
                    vm.Achievement = dr["Achievement"].ToString();
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
