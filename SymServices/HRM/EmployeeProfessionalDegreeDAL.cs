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
    public class EmployeeProfessionalDegreeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        //==================SelectAll=================
        public List<EmployeeProfessionalDegreeVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeProfessionalDegreeVM> VMs = new List<EmployeeProfessionalDegreeVM>();
            EmployeeProfessionalDegreeVM vm;
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
,TotalYear
,YearOfPassing
,Level
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

,Marks
    FROM EmployeeProfessionalDegree
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
                    vm = new EmployeeProfessionalDegreeVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Degree_E = dr["Degree_E"].ToString();
                    vm.Institute = dr["Institute"].ToString();

                    vm.TotalYear = Convert.ToInt32(dr["TotalYear"]);
                    vm.YearOfPassing = dr["YearOfPassing"].ToString();
                    vm.Level = dr["Level"].ToString();
                    vm.CGPA = Convert.ToDecimal(dr["CGPA"]);
                    vm.Scale = Convert.ToDecimal(dr["Scale"]);
                    //vm.Result = dr["Result"].ToString();
                    //vm.Marks = Convert.ToDecimal(dr["Marks"]);
                    //vm.IsLast = Convert.ToBoolean(dr["IsLast"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.FileName = dr["FileName"].ToString();
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
        public List<EmployeeProfessionalDegreeVM> SelectAllByEmployeeId(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeProfessionalDegreeVM> employeeProfessionalDegreeVMs = new List<EmployeeProfessionalDegreeVM>();
            EmployeeProfessionalDegreeVM employeeProfessionalDegreeVM;
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

,TotalYear
,YearOfPassing
,Level
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


,Marks
    FROM EmployeeProfessionalDegree
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
                    employeeProfessionalDegreeVM = new EmployeeProfessionalDegreeVM();
                    employeeProfessionalDegreeVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeProfessionalDegreeVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeProfessionalDegreeVM.Degree_E = dr["Degree_E"].ToString();
                    employeeProfessionalDegreeVM.Institute = dr["Institute"].ToString();
                    //employeeEducation.Major = dr["Major"].ToString();
                    employeeProfessionalDegreeVM.TotalYear = Convert.ToInt32(dr["TotalYear"]);
                    employeeProfessionalDegreeVM.YearOfPassing = dr["YearOfPassing"].ToString();
                    employeeProfessionalDegreeVM.Level = dr["Level"].ToString();
                    //employeeProfessionalDegreeVM.CGPA = Convert.ToDecimal(dr["CGPA"]);
                    //employeeProfessionalDegreeVM.Scale = Convert.ToDecimal(dr["Scale"]);
                    employeeProfessionalDegreeVM.FileName = dr["FileName"].ToString();
                    //employeeEducation.Result = dr["Result"].ToString();
                    employeeProfessionalDegreeVM.Marks = Convert.ToDecimal(dr["Marks"]);
                    //employeeEducation.IsLast = Convert.ToBoolean(dr["IsLast"]);
                    employeeProfessionalDegreeVM.Remarks = dr["Remarks"].ToString();
                    employeeProfessionalDegreeVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeProfessionalDegreeVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    employeeProfessionalDegreeVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeProfessionalDegreeVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeProfessionalDegreeVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeProfessionalDegreeVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeProfessionalDegreeVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeProfessionalDegreeVMs.Add(employeeProfessionalDegreeVM);
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

            return employeeProfessionalDegreeVMs;
        }

        //==================SelectByID=================
        public EmployeeProfessionalDegreeVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeProfessionalDegreeVM employeeProfessionalDegreeVM = new EmployeeProfessionalDegreeVM();

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

,TotalYear
,YearOfPassing
,Level
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


,Marks

FROM EmployeeProfessionalDegree
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
                    employeeProfessionalDegreeVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeProfessionalDegreeVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeProfessionalDegreeVM.Degree_E = dr["Degree_E"].ToString();
                    employeeProfessionalDegreeVM.Institute = dr["Institute"].ToString();
                    employeeProfessionalDegreeVM.Level = dr["Level"].ToString();
                    employeeProfessionalDegreeVM.TotalYear = Convert.ToInt32(dr["TotalYear"]);
                    employeeProfessionalDegreeVM.YearOfPassing = dr["YearOfPassing"].ToString();
                    //employeeProfessionalDegreeVM.CGPA = Convert.ToDecimal(dr["CGPA"]);
                    //employeeProfessionalDegreeVM.Scale = Convert.ToDecimal(dr["Scale"]);
                    //employeeProfessionalDegreeVM.Result = dr["Result"].ToString();
                    employeeProfessionalDegreeVM.FileName = dr["FileName"].ToString();
                    employeeProfessionalDegreeVM.Marks = Convert.ToDecimal(dr["Marks"]);
                    //employeeProfessionalDegreeVM.IsLast = Convert.ToBoolean(dr["IsLast"]);
                    employeeProfessionalDegreeVM.Remarks = dr["Remarks"].ToString();
                    employeeProfessionalDegreeVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeProfessionalDegreeVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    employeeProfessionalDegreeVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeProfessionalDegreeVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeProfessionalDegreeVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeProfessionalDegreeVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeProfessionalDegreeVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeeProfessionalDegreeVM;
        }
        //        //==================SelectByID=================
        //        public EmployeeProfessionalDegreeVM SelectById(int Id)
        //        {

        //            #region Variables

        //            SqlConnection currConn = null;
        //            string sqlText = "";
        //            EmployeeProfessionalDegreeVM employeeProfessionalDegreeVM = new EmployeeProfessionalDegreeVM();

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

        ////                #region sql statement

        ////                sqlText = @"SELECT
        ////Id
        ////,EmployeeId
        ////,Language_E
        ////,Fluency_E
        ////,Competency_E
        ////,FileName
        ////,Remarks
        ////,IsActive
        ////,IsArchive
        ////,CreatedBy
        ////,CreatedAt
        ////,CreatedFrom
        ////,LastUpdateBy
        ////,LastUpdateAt
        ////,LastUpdateFrom
        ////    From EmployeeLanguage
        ////where  id=@Id
        ////     
        ////";

        ////                SqlCommand objComm = new SqlCommand();
        ////                objComm.Connection = currConn;
        ////                objComm.CommandText = sqlText;
        ////                objComm.CommandType = CommandType.Text;
        ////                objComm.Parameters.AddWithValue("@Id", Id);

        ////                SqlDataReader dr;
        ////                dr = objComm.ExecuteReader();
        ////                while (dr.Read())
        ////                {
        ////                    employeeLanguageVM.Id = Convert.ToInt32(dr["Id"]);
        ////                    employeeLanguageVM.EmployeeId = dr["EmployeeId"].ToString();
        ////                    employeeLanguageVM.Language_E = dr["Language_E"].ToString();
        ////                    employeeLanguageVM.Fluency_E = dr["Fluency_E"].ToString();
        ////                    employeeLanguageVM.Competency_E = dr["Competency_E"].ToString();
        ////                    employeeLanguageVM.FileName = dr["FileName"].ToString();
        ////                    employeeLanguageVM.Remarks = dr["Remarks"].ToString();
        ////                    employeeLanguageVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
        ////                    employeeLanguageVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
        ////                    employeeLanguageVM.CreatedBy = dr["CreatedBy"].ToString();
        ////                    employeeLanguageVM.CreatedFrom = dr["CreatedFrom"].ToString();
        ////                    employeeLanguageVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
        ////                    employeeLanguageVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
        ////                    employeeLanguageVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

        ////                }
        ////                dr.Close();


        ////                #endregion
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

        //            return employeeProfessionalDegreeVM;
        //        }

        //==================Insert =================
        public string[] Insert(EmployeeProfessionalDegreeVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeProfessionalDegree"; //Method Name
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
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeProfessionalDegree ";
                sqlText += " WHERE EmployeeId=@EmployeeId And Degree_E=@Degree_E";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@Degree_E", vm.Degree_E);
                object objfoundId = cmdExist.ExecuteScalar();

                if (objfoundId == null)
                {
                    retResults[1] = "Please Input Employee Professional Degree Value";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Input Employee Professional Degree Value", "");
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
INSERT INTO EmployeeProfessionalDegree(
EmployeeId
,Degree_E
,Institute

,TotalYear
,YearOfPassing
,Level
,FileName
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom


,Marks
) VALUES (
 @EmployeeId
,@Degree_E
,@Institute

,@TotalYear
,@YearOfPassing
,@Level
,@FileName
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom

,@Marks
) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);

                    cmdExist1.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExist1.Parameters.AddWithValue("@Degree_E", vm.Degree_E);
                    cmdExist1.Parameters.AddWithValue("@Institute", vm.Institute ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@Level", vm.Level ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@TotalYear", vm.TotalYear);
                    cmdExist1.Parameters.AddWithValue("@YearOfPassing", vm.YearOfPassing ?? Convert.DBNull);
                    //cmdExist1.Parameters.AddWithValue("@IsLast", vm.IsLast);
                    cmdExist1.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@IsActive", true);
                    cmdExist1.Parameters.AddWithValue("@IsArchive", false);
                    cmdExist1.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdExist1.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdExist1.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    //cmdExist1.Parameters.AddWithValue("@CGPA", vm.CGPA);
                    //cmdExist1.Parameters.AddWithValue("@Scale", vm.Scale);
                    //cmdExist1.Parameters.AddWithValue("@Result", vm.Result);
                    cmdExist1.Parameters.AddWithValue("@Marks", vm.Marks);

                    cmdExist1.Transaction = transaction;
                    var exeRes = cmdExist1.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Professional Degree";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Professional Degree Value", "");
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

        public string[] Update(EmployeeProfessionalDegreeVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Professional Degree Update"; //Method Name

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
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeProfessionalDegree ";
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


                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeProfessionalDegree set";
                    sqlText += " EmployeeId=@EmployeeId";
                    sqlText += " ,Degree_E=@Degree_E";
                    sqlText += ",Institute=@Institute";
                    sqlText += ",Level=@Level";
                    sqlText += " ,TotalYear=@TotalYear";
                    //sqlText += " ,CGPA=@CGPA";
                    sqlText += " ,YearOfPassing=@YearOfPassing";
                    //sqlText += " ,Scale=@Scale";
                    //sqlText += " ,Result=@Result";
                    sqlText += " ,Marks=@Marks";
                    //sqlText += " ,IsLast=@IsLast";
                    if (vm.FileName != null)
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
                    cmdUpdate.Parameters.AddWithValue("@Level", vm.Level ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TotalYear", vm.TotalYear);
                    //cmdUpdate.Parameters.AddWithValue("@CGPA", vm.CGPA);
                    cmdUpdate.Parameters.AddWithValue("@YearOfPassing", vm.YearOfPassing ?? Convert.DBNull);
                    //cmdUpdate.Parameters.AddWithValue("@Scale", vm.Scale);
                    //cmdUpdate.Parameters.AddWithValue("@Result", vm.Result ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Marks", vm.Marks);
                    //cmdUpdate.Parameters.AddWithValue("@IsLast", vm.IsLast);
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
                        throw new ArgumentNullException("Professional Degree Update", vm.Degree_E + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Professional Degree Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Professional Degree.";
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
        public string[] Delete(EmployeeProfessionalDegreeVM employeeProfessionalDegreeVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DegreeDelete"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToDegree "); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeProfessionalDegree set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", employeeProfessionalDegreeVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", employeeProfessionalDegreeVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", employeeProfessionalDegreeVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("EmployeeProfessionalDegreeDelete", employeeProfessionalDegreeVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeProfessionalDegreeDelete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete .";
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
    }
}
