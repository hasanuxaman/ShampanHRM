using SymOrdinary;
using SymServices.Common;
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
    public class EmployeeEducationDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeEducationVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeEducationVM> VMs = new List<EmployeeEducationVM>();
            EmployeeEducationVM vm;
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
Id
,EmployeeId
,Degree_E
,Institute
,Major
,TotalYear
,YearOfPassing
,IsLast
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
,CGPA
,Scale
,Result
,Marks
    FROM EmployeeEducation
Where IsArchive=0
    ORDER BY YearOfPassing
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeEducationVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Degree_E = dr["Degree_E"].ToString();
                    vm.Institute = dr["Institute"].ToString();
                    vm.Major = dr["Major"].ToString();
                    vm.TotalYear = Convert.ToInt32(dr["TotalYear"]);
                    vm.YearOfPassing = dr["YearOfPassing"].ToString();
                    vm.CGPA = Convert.ToDecimal(dr["CGPA"]);
                    vm.Scale = Convert.ToDecimal(dr["Scale"]);
                    vm.Result = dr["Result"].ToString();
                    vm.Marks = Convert.ToDecimal(dr["Marks"]);
                    vm.IsLast = Convert.ToBoolean(dr["IsLast"]);
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
        //==================SelectByEmployeeId=================
        public List<EmployeeEducationVM> SelectAllByEmployeeId(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeEducationVM> employeeEducations = new List<EmployeeEducationVM>();
            EmployeeEducationVM employeeEducation;
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
Id
,EmployeeId
,Degree_E
,Institute
,Major
,TotalYear
,YearOfPassing
,IsLast
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
,CGPA
,Scale
,Result
,Marks
    FROM EmployeeEducation
Where IsArchive=0 And EmployeeId=@EmployeeId
    ORDER BY YearOfPassing
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeEducation = new EmployeeEducationVM();
                    employeeEducation.Id = Convert.ToInt32(dr["Id"]);
                    employeeEducation.EmployeeId = dr["EmployeeId"].ToString();
                    employeeEducation.Degree_E = dr["Degree_E"].ToString();
                    employeeEducation.Institute = dr["Institute"].ToString();
                    employeeEducation.Major = dr["Major"].ToString();
                    employeeEducation.TotalYear = Convert.ToInt32(dr["TotalYear"]);
                    employeeEducation.YearOfPassing = dr["YearOfPassing"].ToString();
                    employeeEducation.CGPA = Convert.ToDecimal(dr["CGPA"]);
                    employeeEducation.Scale = Convert.ToDecimal(dr["Scale"]);
                    employeeEducation.FileName = dr["FileName"].ToString();
                    employeeEducation.Result = dr["Result"].ToString();
                    employeeEducation.Marks = Convert.ToDecimal(dr["Marks"]);
                    employeeEducation.IsLast = Convert.ToBoolean(dr["IsLast"]);
                    employeeEducation.Remarks = dr["Remarks"].ToString();
                    employeeEducation.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeEducation.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeEducation.CreatedBy = dr["CreatedBy"].ToString();
                    employeeEducation.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeEducation.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeEducation.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeEducation.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeEducations.Add(employeeEducation);
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

            return employeeEducations;
        }
        //==================SelectByID=================
        public EmployeeEducationVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeEducationVM employeeEducationVM= new EmployeeEducationVM();
           
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
,Degree_E
,Institute
,Major
,TotalYear
,YearOfPassing
,IsLast
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
,CGPA
,Scale
,Result
,Marks

FROM EmployeeEducation
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
                    employeeEducationVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeEducationVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeEducationVM.Degree_E = dr["Degree_E"].ToString();
                    employeeEducationVM.Institute = dr["Institute"].ToString();
                    employeeEducationVM.Major = dr["Major"].ToString();
                    employeeEducationVM.TotalYear = Convert.ToInt32(dr["TotalYear"]);
                    employeeEducationVM.YearOfPassing = dr["YearOfPassing"].ToString();
                    employeeEducationVM.CGPA = Convert.ToDecimal(dr["CGPA"]);
                    employeeEducationVM.Scale = Convert.ToDecimal(dr["Scale"]);
                    employeeEducationVM.Result = dr["Result"].ToString();
                    employeeEducationVM.FileName = dr["FileName"].ToString();
                    employeeEducationVM.Marks = Convert.ToDecimal(dr["Marks"]);
                    employeeEducationVM.IsLast = Convert.ToBoolean(dr["IsLast"]);
                    employeeEducationVM.Remarks = dr["Remarks"].ToString();
                    employeeEducationVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeEducationVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeEducationVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeEducationVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeEducationVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeEducationVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeEducationVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeeEducationVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeeEducationVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeEducation"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(vm.Degree_E))
                {
                    retResults[1] = "Please Input Employee Degree_E";
                    return retResults;
                }
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
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeEducation ";
                sqlText += " WHERE EmployeeId=@EmployeeId And Degree_E=@Degree_E";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@Degree_E", vm.Degree_E);
                object objfoundId = cmdExist.ExecuteScalar();

                if (objfoundId == null)
                {
                    retResults[1] = "Please Input Employee Education Value";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Input Employee Education Value", "");
                }
                #endregion Exist

                //#region Exist
                //CommonDAL cdal = new CommonDAL();
                //bool check = false;
                //string tableName = "EmployeeEducation";
                //string[] fieldName = { "Degree_E" };
                //string[] fieldValue = { vm.Degree_E.Trim() };
				
                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                //#endregion Exist
                #region Save

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                //if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO EmployeeEducation(
EmployeeId
,Degree_E
,Institute
,Major
,TotalYear
,YearOfPassing
,IsLast
,FileName
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,CGPA
,Scale
,Result
,Marks
) VALUES (
 @EmployeeId
,@Degree_E
,@Institute
,@Major
,@TotalYear
,@YearOfPassing
,@IsLast
,@FileName
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@CGPA
,@Scale
,@Result
,@Marks
) SELECT SCOPE_IDENTITY()";
                    
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);

                    cmdExist1.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExist1.Parameters.AddWithValue("@Degree_E", vm.Degree_E);
                    cmdExist1.Parameters.AddWithValue("@Institute", vm.Institute?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@Major", vm.Major ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@TotalYear", vm.TotalYear );
                    cmdExist1.Parameters.AddWithValue("@YearOfPassing", vm.YearOfPassing ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@IsLast", vm.IsLast);
                    cmdExist1.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@IsActive", true);
                    cmdExist1.Parameters.AddWithValue("@IsArchive", false);
                    cmdExist1.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdExist1.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdExist1.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdExist1.Parameters.AddWithValue("@CGPA", vm.CGPA );
                    cmdExist1.Parameters.AddWithValue("@Scale", vm.Scale );
                    cmdExist1.Parameters.AddWithValue("@Result", vm.Result );
                    cmdExist1.Parameters.AddWithValue("@Marks", vm.Marks );

                    cmdExist1.Transaction = transaction;
					var exeRes = cmdExist1.ExecuteScalar();
					Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Education Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Education Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Degree already used";
                    throw new ArgumentNullException("This Degree already used", "");
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
        public string[] Update(EmployeeEducationVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EducationUpdate"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEducation"); }

                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeEducation ";
                sqlText += " WHERE EmployeeId=@EmployeeId And Degree_E=@Degree_E AND Id<>@Id";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@Degree_E", vm.Degree_E);

                var exeRes = cmdExist.ExecuteScalar();
                int exists = Convert.ToInt32(exeRes);

                if (exists > 0)
                {
                    retResults[1] = "This Degree already used";
                    throw new ArgumentNullException("This Degree already used", "");
                }
                #endregion Exist

                //#region Exist

                //CommonDAL cdal = new CommonDAL();
                //bool check = false;
                //string tableName = "EmployeeEducation";
                //string[] fieldName = { "Degree_E" };
                //string[] fieldValue = { vm.Degree_E.Trim() };

                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInUpdate(vm.Id.ToString(), tableName, fieldName[i], fieldValue[i], currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                //#endregion Exist

                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeEducation set";
                    sqlText += " EmployeeId=@EmployeeId";
                    sqlText += " ,Degree_E=@Degree_E";
                    sqlText += " ,Institute=@Institute";
                    sqlText += " ,Major=@Major";
                    sqlText += " ,TotalYear=@TotalYear";
                    sqlText += " ,CGPA=@CGPA";
                    sqlText += " ,YearOfPassing=@YearOfPassing";
                    sqlText += " ,Scale=@Scale";
                    sqlText += " ,Result=@Result";
                    sqlText += " ,Marks=@Marks";
                    sqlText += " ,IsLast=@IsLast";
                    if (vm.FileName !=null)
                    {
                        sqlText += " ,FileName=@FileName";
                    }
                    sqlText += " ,Remarks=@Remarks";
                   // sqlText += " ,IsActive=@IsActive";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@Degree_E", vm.Degree_E);

                    cmdUpdate.Parameters.AddWithValue("@Institute", vm.Institute ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Major", vm.Major ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TotalYear", vm.TotalYear );
                    cmdUpdate.Parameters.AddWithValue("@CGPA", vm.CGPA );
                    cmdUpdate.Parameters.AddWithValue("@YearOfPassing", vm.YearOfPassing??Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Scale", vm.Scale );
                    cmdUpdate.Parameters.AddWithValue("@Result", vm.Result ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Marks", vm.Marks);
                    cmdUpdate.Parameters.AddWithValue("@IsLast", vm.IsLast);
                    if (vm.FileName != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    }


                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                  //  cmdUpdate.Parameters.AddWithValue("@IsActive", educationVM.IsActive);
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
                        throw new ArgumentNullException("Education Update", vm.Degree_E + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Education Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Education.";
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
//        public EmployeeEducationVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
//        {

//            #region Variables

//            SqlConnection currConn = null;
//            string sqlText = "";

//            EmployeeEducationVM employeeEducationVM = new EmployeeEducationVM();

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
//,Degree_E
//,Institute
//,Major
//,TotalYear
//,Score
//,StartDate
//,EndDate
//,Remarks
//,IsActive
//,IsArchive
//,CreatedBy
//,CreatedAt
//,CreatedFrom
//,LastUpdateBy
//,LastUpdateAt
//,LastUpdateFrom
//FROM EmployeeEducation    
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
//                        employeeEducationVM.Id = Convert.ToInt32(dr["Id"]);
//                        employeeEducationVM.EmployeeId = dr["EmployeeId"].ToString();
//                        employeeEducationVM.Degree_E = dr["Degree_E"].ToString();
//                        employeeEducationVM.Institute = dr["Institute"].ToString();
//                        employeeEducationVM.Major = dr["Major"].ToString();
//                        employeeEducationVM.TotalYear = Convert.ToInt32(dr["TotalYear"]);
//                        //employeeEducationVM.Score = Convert.ToDecimal(dr["Score"]);
//                        //employeeEducationVM.StartDate = dr["StartDate"].ToString();
//                        //employeeEducationVM.EndDate = dr["EndDate"].ToString();
//                        employeeEducationVM.Remarks = dr["Remarks"].ToString();
//                        employeeEducationVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
//                        employeeEducationVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
//                        employeeEducationVM.CreatedBy = dr["CreatedBy"].ToString();
//                        employeeEducationVM.CreatedFrom = dr["CreatedFrom"].ToString();
//                        employeeEducationVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
//                        employeeEducationVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
//                        employeeEducationVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

//            return employeeEducationVM;
//        }
        //==================Delete =================
        public string[] Delete(EmployeeEducationVM employeeEducationVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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

                if (ids.Length>=1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeEducation set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeEducationVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeEducationVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeEducationVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }
                   

                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Education Delete", employeeEducationVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Education Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Education.";
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
        public List<EmployeeEducationVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeEducationVM> VMs = new List<EmployeeEducationVM>();
            EmployeeEducationVM vm;
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
 isnull(Edu.Id,0)						  Id
,ei.EmployeeId							EmployeeId
,isnull(Edu.Degree_E		, 'NA')			  Degree_E
,isnull(Edu.Institute		, 'NA')			  Institute
,isnull(Edu.Major			, 'NA')			  Major
,isnull(Edu.TotalYear		, 0)			  TotalYear
,isnull(Edu.YearOfPassing	, 'NA')			  YearOfPassing
,isnull(Edu.[FileName]		, 'NA')			  [FileName]
,isnull(Edu.Remarks			, 'NA')		  Remarks
,isnull(Edu.IsActive		, 0)			  IsActive
,isnull(Edu.IsArchive		, 0)			  IsArchive
,isnull(Edu.CreatedBy		, 'NA')			  CreatedBy
,isnull(Edu.CreatedAt		, 'NA')			  CreatedAt
,isnull(Edu.CreatedFrom		, 'NA')		  CreatedFrom
,isnull(Edu.LastUpdateBy	, 'NA')			  LastUpdateBy
,isnull(Edu.LastUpdateAt	, 'NA')			  LastUpdateAt
,isnull(Edu.LastUpdateFrom	, 'NA')			  LastUpdateFrom
,isnull(Edu.CGPA			, 0)			  CGPA
,isnull(Edu.Scale			, 0)			  Scale
,isnull(Edu.Result			, 'NA')			  Result
,isnull(Edu.Marks			, 0)			  Marks
,isnull(Edu.IsLast			, 0)			  IsLast

    From ViewEmployeeInformation ei
		left outer join EmployeeEducation Edu on ei.EmployeeId=Edu.EmployeeId
Where ei.IsArchive=0 and ei.isActive=1 and Edu.IsArchive=0 and Edu.isActive=1
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

                sqlText += " , Edu.YearOfPassing";

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

                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeEducationVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Degree_E = dr["Degree_E"].ToString();
                    vm.Institute = dr["Institute"].ToString();
                    vm.Major = dr["Major"].ToString();
                    vm.TotalYear = Convert.ToInt32(dr["TotalYear"]);
                    vm.YearOfPassing = dr["YearOfPassing"].ToString();
                    vm.CGPA = Convert.ToDecimal(dr["CGPA"]);
                    vm.Scale = Convert.ToDecimal(dr["Scale"]);
                    vm.Result = dr["Result"].ToString();
                    vm.Marks = Convert.ToDecimal(dr["Marks"]);
                    vm.IsLast = Convert.ToBoolean(dr["IsLast"]);
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

        #endregion
    }
}
