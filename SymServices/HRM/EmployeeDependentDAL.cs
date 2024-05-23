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
    public class EmployeeDependentDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeDependentVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeDependentVM> VMs = new List<EmployeeDependentVM>();
            EmployeeDependentVM vm;
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
,Name
,Relation
,DateofBirth
,Address
,District
,Division
,Country
,City
,BirthCertificateNo
,PostalCode
,ISNULL(PostOffice, '')PostOffice
,Phone
,Mobile
,Fax
,Gender	            
,EducationQualification
,BloodGroup            
,Remarks
,ISNULL(IsVaccineDose1Complete, '') IsVaccineDose1Complete
,ISNULL(VaccineDose1Date, '') VaccineDose1Date
,ISNULL(VaccineDose1Name, '') VaccineDose1Name
,ISNULL(IsVaccineDose2Complete, '') IsVaccineDose2Complete
,ISNULL(VaccineDose2Date, '') VaccineDose2Date
,ISNULL(VaccineDose2Name, '') VaccineDose2Name
,ISNULL(IsVaccineDose3Complete, '') IsVaccineDose3Complete
,ISNULL(VaccineDose3Date, '') VaccineDose3Date
,ISNULL(VaccineDose3Name, '') VaccineDose3Name
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,IsDependentAllowance

    From EmployeeDependent
Where IsArchive=0
    ORDER BY Name
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeDependentVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.Relation = dr["Relation"].ToString();
                    vm.DateofBirth = Ordinary.StringToDate(dr["DateofBirth"].ToString());
                    //vm.NID = dr["NID"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.District = dr["District"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.BirthCertificateNo = dr["BirthCertificateNo"].ToString();
                    vm.PostalCode = dr["PostalCode"].ToString();
                    vm.PostOffice = dr["PostOffice"].ToString();
                    vm.Phone = dr["Phone"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Gender = dr["Gender"].ToString();
                    vm.EducationQualification = dr["EducationQualification"].ToString();
                    vm.BloodGroup = dr["BloodGroup"].ToString();

                    //Gender
                    //EducationQualification
                    //BloodGroup

                    vm.Remarks = dr["Remarks"].ToString();

                    vm.IsVaccineDose1Complete = Convert.ToBoolean(dr["IsVaccineDose1Complete"]);
                    vm.VaccineDose1Date = Ordinary.StringToDate(dr["VaccineDose1Date"].ToString());
                    vm.VaccineDose1Name = dr["VaccineDose1Name"].ToString();
                    vm.IsVaccineDose2Complete = Convert.ToBoolean(dr["IsVaccineDose2Complete"]);
                    vm.VaccineDose2Date = Ordinary.StringToDate(dr["VaccineDose2Date"].ToString());
                    vm.VaccineDose2Name = dr["VaccineDose2Name"].ToString();
                    vm.IsVaccineDose3Complete = Convert.ToBoolean(dr["IsVaccineDose3Complete"]);
                    vm.VaccineDose3Date = Ordinary.StringToDate(dr["VaccineDose3Date"].ToString());
                    vm.VaccineDose3Name = dr["VaccineDose3Name"].ToString();

                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsDependentAllowance = Convert.ToBoolean(dr["IsDependentAllowance"]);
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
        public List<EmployeeDependentVM> SelectAllByEmployee(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeDependentVM> VMs = new List<EmployeeDependentVM>();
            EmployeeDependentVM vm;
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
,Name
,Relation
,DateofBirth
,Address
,District
,Division
,Country
,City
,BirthCertificateNo
,NID
,PostalCode
,ISNULL(PostOffice, '')PostOffice
,Phone
,Mobile
,Fax
,Gender
,EducationQualification
,BloodGroup
,Remarks

,ISNULL(IsVaccineDose1Complete, '') IsVaccineDose1Complete
,ISNULL(VaccineDose1Date, '') VaccineDose1Date
,ISNULL(VaccineDose1Name, '') VaccineDose1Name

,ISNULL(IsVaccineDose2Complete, '') IsVaccineDose2Complete
,ISNULL(VaccineDose2Date, '') VaccineDose2Date
,ISNULL(VaccineDose2Name, '') VaccineDose2Name

,ISNULL(IsVaccineDose3Complete, '') IsVaccineDose3Complete
,ISNULL(VaccineDose3Date, '') VaccineDose3Date
,ISNULL(VaccineDose3Name, '') VaccineDose3Name

,IsActive
,IsArchive
,isnull(IsDependentAllowance,'0')IsDependentAllowance

    From EmployeeDependent
Where IsArchive=0 and EmployeeId=@EmployeeId
    ORDER BY Name
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeDependentVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.Relation = dr["Relation"].ToString();
                    vm.DateofBirth = Ordinary.StringToDate(dr["DateofBirth"].ToString());
                    //vm.NID = dr["NID"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.District = dr["District"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.BirthCertificateNo = dr["BirthCertificateNo"].ToString();
                    vm.NID = dr["NID"].ToString();
                    vm.PostalCode = dr["PostalCode"].ToString();
                    vm.PostOffice = dr["PostOffice"].ToString();
                    vm.Phone = dr["Phone"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Fax = dr["Fax"].ToString();
                    vm.Gender = dr["Gender"].ToString();
                    vm.EducationQualification = dr["EducationQualification"].ToString();
                    vm.BloodGroup = dr["BloodGroup"].ToString();

                    //Gender
                    //EducationQualification
                    //BloodGroup

                    vm.Remarks = dr["Remarks"].ToString();

                    vm.IsVaccineDose1Complete = Convert.ToBoolean(dr["IsVaccineDose1Complete"]);
                    vm.VaccineDose1Date = Ordinary.StringToDate(dr["VaccineDose1Date"].ToString());
                    vm.VaccineDose1Name = dr["VaccineDose1Name"].ToString();

                    vm.IsVaccineDose2Complete = Convert.ToBoolean(dr["IsVaccineDose2Complete"]);
                    vm.VaccineDose2Date = Ordinary.StringToDate(dr["VaccineDose2Date"].ToString());
                    vm.VaccineDose2Name = dr["VaccineDose2Name"].ToString();

                    vm.IsVaccineDose3Complete = Convert.ToBoolean(dr["IsVaccineDose3Complete"]);
                    vm.VaccineDose3Date = Ordinary.StringToDate(dr["VaccineDose3Date"].ToString());
                    vm.VaccineDose3Name = dr["VaccineDose3Name"].ToString();

                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsDependentAllowance = Convert.ToBoolean(dr["IsDependentAllowance"]);
                    //vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    //vm.CreatedBy = dr["CreatedBy"].ToString();
                    //vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    //vm.LastUpdateAt =Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    //vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    //vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

        //==================SelectByID=================
        public EmployeeDependentVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeDependentVM vm = new EmployeeDependentVM();

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
,Name
,Relation
,DateofBirth
,Address
,District
,Division
,Country
,City
,BirthCertificateNo
,NID
,PostalCode
,ISNULL (PostOffice, '')PostOffice
,Phone
,Mobile
,Fax
,FileName
,Gender
,EducationQualification
,BloodGroup
,Remarks

,ISNULL(IsVaccineDose1Complete, '') IsVaccineDose1Complete
,ISNULL(VaccineDose1Date, '') VaccineDose1Date
,ISNULL(VaccineDose1Name, '') VaccineDose1Name

,ISNULL(IsVaccineDose2Complete, '') IsVaccineDose2Complete
,ISNULL(VaccineDose2Date, '') VaccineDose2Date
,ISNULL(VaccineDose2Name, '') VaccineDose2Name

,ISNULL(IsVaccineDose3Complete, '') IsVaccineDose3Complete
,ISNULL(VaccineDose3Date, '') VaccineDose3Date
,ISNULL(VaccineDose3Name, '') VaccineDose3Name

,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,Isnull(IsDependentAllowance,'0')IsDependentAllowance

    From EmployeeDependent
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
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.Relation = dr["Relation"].ToString();
                    vm.DateofBirth = Ordinary.StringToDate(dr["DateofBirth"].ToString());
                    //vm.NID = dr["NID"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.District = dr["District"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.BirthCertificateNo = dr["BirthCertificateNo"].ToString();
                    vm.NID = dr["NID"].ToString();
                    vm.PostalCode = dr["PostalCode"].ToString();
                    vm.PostOffice = dr["PostOffice"].ToString();

                    vm.Phone = dr["Phone"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Fax = dr["Fax"].ToString();
                    vm.FileName = dr["FileName"].ToString();

                    vm.Gender = dr["Gender"].ToString();
                    vm.EducationQualification = dr["EducationQualification"].ToString();
                    vm.BloodGroup = dr["BloodGroup"].ToString();

                    vm.Remarks = dr["Remarks"].ToString();

                    vm.IsVaccineDose1Complete = Convert.ToBoolean(dr["IsVaccineDose1Complete"]);
                    vm.VaccineDose1Date = Ordinary.StringToDate(dr["VaccineDose1Date"].ToString());
                    vm.VaccineDose1Name = dr["VaccineDose1Name"].ToString();

                    vm.IsVaccineDose2Complete = Convert.ToBoolean(dr["IsVaccineDose2Complete"]);
                    vm.VaccineDose2Date = Ordinary.StringToDate(dr["VaccineDose2Date"].ToString());
                    vm.VaccineDose2Name = dr["VaccineDose2Name"].ToString();

                    vm.IsVaccineDose3Complete = Convert.ToBoolean(dr["IsVaccineDose3Complete"]);
                    vm.VaccineDose3Date = Ordinary.StringToDate(dr["VaccineDose3Date"].ToString());
                    vm.VaccineDose3Name = dr["VaccineDose3Name"].ToString();

                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsDependentAllowance = Convert.ToBoolean(dr["IsDependentAllowance"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return vm;
        }
        //==================Insert =================
        public string[] Insert(EmployeeDependentVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeDependent"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;


            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(EmployeeDependentVM.DepartmentId))
                //{
                //    retResults[1] = "Please Input Employee Travel Course";
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
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeDependent ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", vm.Name);
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Dependent Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Dependent Value", "");
                //}
                //#endregion Exist
                //#region Exist
                //CommonDAL cdal = new CommonDAL();
                //bool check = false;
                //string tableName = "EmployeeDependent";	
                //string[] fieldName = { "Name" };
                //string[] fieldValue = { vm.Name.Trim() };

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

                //int foundId = (int)objfoundId;
                //if (foundId <= 0)
                if (1 == 1)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeDependent(
EmployeeId,Name,Relation,DateofBirth,BirthCertificateNo,NID,Address,District,Division,Country,City,PostalCode,PostOffice
,Phone,Mobile,Fax,FileName,Gender,EducationQualification,BloodGroup
,Remarks,IsVaccineDose1Complete,VaccineDose1Date,VaccineDose1Name,IsVaccineDose2Complete,VaccineDose2Date,VaccineDose2Name,IsVaccineDose3Complete,VaccineDose3Date,VaccineDose3Name,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,IsDependentAllowance) 
VALUES (
@EmployeeId,@Name,@Relation,@DateofBirth,@BirthCertificateNo,@NID,@Address,@District,@Division,@Country,@City,@PostalCode,@PostOffice
,@Phone,@Mobile,@Fax,@FileName
,@Gender,@EducationQualification,@BloodGroup
,@Remarks,@IsVaccineDose1Complete,@VaccineDose1Date,@VaccineDose1Name,@IsVaccineDose2Complete,@VaccineDose2Date,@VaccineDose2Name,@IsVaccineDose3Complete,@VaccineDose3Date,@VaccineDose3Name,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@IsDependentAllowance) 
SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);

                    cmdInsert.Parameters.AddWithValue("@Relation", vm.Relation ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@DateofBirth", Ordinary.DateToString(vm.DateofBirth));
                    cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@BirthCertificateNo", vm.BirthCertificateNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@NID", vm.NID ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@District", vm.District ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Division", vm.Division ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PostalCode", vm.PostalCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PostOffice", vm.PostOffice ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Phone", vm.Phone ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Mobile", vm.Mobile ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Fax", vm.Fax ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Gender", vm.Gender ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@EducationQualification", vm.EducationQualification ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@BloodGroup", vm.BloodGroup ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@IsVaccineDose1Complete", vm.IsVaccineDose1Complete);
                    cmdInsert.Parameters.AddWithValue("@VaccineDose1Date", Ordinary.DateToString(vm.VaccineDose1Date));
                    cmdInsert.Parameters.AddWithValue("@VaccineDose1Name", vm.VaccineDose1Name ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsVaccineDose2Complete", vm.IsVaccineDose2Complete);
                    cmdInsert.Parameters.AddWithValue("@VaccineDose2Date", Ordinary.DateToString(vm.VaccineDose2Date));
                    cmdInsert.Parameters.AddWithValue("@VaccineDose2Name", vm.VaccineDose2Name ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@IsVaccineDose3Complete", vm.IsVaccineDose3Complete);
                    cmdInsert.Parameters.AddWithValue("@VaccineDose3Date", Ordinary.DateToString(vm.VaccineDose3Date));
                    cmdInsert.Parameters.AddWithValue("@VaccineDose3Name", vm.VaccineDose3Name ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@IsDependentAllowance", vm.IsDependentAllowance);

                    cmdInsert.Transaction = transaction;
                    var exeRes = cmdInsert.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input EmployeeDependent Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input EmployeeDependent Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Dependent already used";
                    throw new ArgumentNullException("This Dependent already used", "");
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
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
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
        //==================Update =================
        public string[] Update(EmployeeDependentVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeDependent Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeDependent"); }

                #endregion open connection and transaction
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeDependent ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", vm.Name);

                //var exeRes = cmdExist.ExecuteScalar();
                //int exists = Convert.ToInt32(exeRes);

                //if (exists > 0)
                //{
                //    retResults[1] = "This Dependent Name already used";
                //    throw new ArgumentNullException("This Dependent Name already used", "");
                //}
                //#endregion Exist
                //#region Exist

                //CommonDAL cdal = new CommonDAL();
                //bool check = false;
                //string tableName = "EmployeeDependent";				
                //string[] fieldName = { "Name" };
                //string[] fieldValue = { vm.Name.Trim() };

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
                    sqlText = "update EmployeeDependent set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " Name=@Name,";
                    sqlText += " Relation=@Relation,";
                    sqlText += " DateofBirth=@DateofBirth,";
                    sqlText += " BirthCertificateNo=@BirthCertificateNo,";
                    sqlText += " NID=@NID,";
                    sqlText += " Address=@Address,";
                    sqlText += " District=@District,";
                    sqlText += " Division=@Division,";
                    sqlText += " Country=@Country,";
                    sqlText += " City=@City,";
                    sqlText += " PostalCode=@PostalCode,";
                    sqlText += " PostOffice=@PostOffice,";
                    sqlText += " Phone=@Phone,";
                    sqlText += " Mobile=@Mobile,";
                    sqlText += " Fax=@Fax,";
                    if (vm.FileName != null)
                    {
                        sqlText += " FileName=@FileName,";
                    }




                    sqlText += " Gender=@Gender,";
                    sqlText += " EducationQualification=@EducationQualification,";
                    sqlText += " BloodGroup=@BloodGroup,";
                    sqlText += " Remarks=@Remarks,";
                    // sqlText += " IsActive=@IsActive,";

                    sqlText += " IsVaccineDose1Complete=@IsVaccineDose1Complete,";
                    sqlText += " VaccineDose1Date=@VaccineDose1Date,";
                    sqlText += " VaccineDose1Name=@VaccineDose1Name,";

                    sqlText += " IsVaccineDose2Complete=@IsVaccineDose2Complete,";
                    sqlText += " VaccineDose2Date=@VaccineDose2Date,";
                    sqlText += " VaccineDose2Name=@VaccineDose2Name,";

                    sqlText += " IsVaccineDose3Complete=@IsVaccineDose3Complete,";
                    sqlText += " VaccineDose3Date=@VaccineDose3Date,";
                    sqlText += " VaccineDose3Name=@VaccineDose3Name,";

                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom,";
                    sqlText += " IsDependentAllowance=@IsDependentAllowance";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@IsDependentAllowance", vm.IsDependentAllowance);
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name);
                    cmdUpdate.Parameters.AddWithValue("@Relation", vm.Relation ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BirthCertificateNo", vm.BirthCertificateNo ?? Convert.DBNull);
                    //cmdUpdate.Parameters.AddWithValue("@NID", vm.NID);
                    cmdUpdate.Parameters.AddWithValue("@NID", vm.NID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DateofBirth", Ordinary.DateToString(vm.DateofBirth));
                    cmdUpdate.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@District", vm.District ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Division", vm.Division ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostalCode", vm.PostalCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostOffice", vm.PostOffice ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Phone", vm.Phone ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Mobile", vm.Mobile ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Fax", vm.Fax ?? Convert.DBNull);
                    if (vm.FileName != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    }

                    cmdUpdate.Parameters.AddWithValue("@Gender", vm.Gender ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EducationQualification", vm.EducationQualification ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BloodGroup", vm.BloodGroup ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@IsVaccineDose1Complete", vm.IsVaccineDose1Complete);
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose1Date", Ordinary.DateToString(vm.VaccineDose1Date));
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose1Name", vm.VaccineDose1Name ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@IsVaccineDose2Complete", vm.IsVaccineDose2Complete);
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose2Date", Ordinary.DateToString(vm.VaccineDose2Date));
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose2Name", vm.VaccineDose2Name ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@IsVaccineDose3Complete", vm.IsVaccineDose3Complete);
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose3Date", Ordinary.DateToString(vm.VaccineDose3Date));
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose3Name", vm.VaccineDose3Name ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    //cmdUpdate.Parameters.AddWithValue("@IsActive", employeeDependentVM.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", false);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

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
                    throw new ArgumentNullException("EmployeeDependent Update", "Could not found any item.");
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
                    retResults[1] = "Data Update Successfully";

                }
                else
                {
                    retResults[1] = "Unexpected error to update Employee Dependent.";
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
        public EmployeeDependentVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SqlConnection VcurrConn = null;
            SqlTransaction Vtransaction = null;
            SqlTransaction transaction = null;

            EmployeeDependentVM employeeDependentVM = new EmployeeDependentVM();

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


                #endregion open connection and transaction

                #region sql statement
                sqlText = @"SELECT Top 1 
Id
,EmployeeId
,Name
,Relation
,DateofBirth
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Mobile
,Fax
,Remarks

,IsVaccineDose1Complete
,VaccineDose1Date
,VaccineDose1Name

,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,isnull(IsDependentAllowance,'0')IsDependentAllowance

    From EmployeeDependent
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
                        employeeDependentVM.Id = Convert.ToInt32(dr["Id"]);
                        employeeDependentVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeeDependentVM.Name = dr["Name"].ToString();
                        employeeDependentVM.Relation = dr["Relation"].ToString();
                        employeeDependentVM.DateofBirth = dr["DateofBirth"].ToString();
                        employeeDependentVM.Address = dr["Address"].ToString();
                        employeeDependentVM.District = dr["District"].ToString();
                        employeeDependentVM.Division = dr["Division"].ToString();
                        employeeDependentVM.Country = dr["Country"].ToString();
                        employeeDependentVM.City = dr["City"].ToString();
                        employeeDependentVM.PostalCode = dr["PostalCode"].ToString();
                        employeeDependentVM.Phone = dr["Phone"].ToString();
                        employeeDependentVM.Mobile = dr["Mobile"].ToString();
                        employeeDependentVM.Fax = dr["Fax"].ToString();
                        employeeDependentVM.Remarks = dr["Remarks"].ToString();

                        employeeDependentVM.IsVaccineDose1Complete = Convert.ToBoolean(dr["IsVaccineDose1Complete"]);
                        employeeDependentVM.VaccineDose1Date = dr["VaccineDose1Date"].ToString();
                        employeeDependentVM.VaccineDose1Name = dr["VaccineDose1Name"].ToString();

                        employeeDependentVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeeDependentVM.IsDependentAllowance = Convert.ToBoolean(dr["IsDependentAllowance"]);
                        employeeDependentVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeeDependentVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeeDependentVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeeDependentVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeeDependentVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeeDependentVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return employeeDependentVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeeDependentVM EmployeeDependentVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeDependent"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeDependent"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        //if (EmployeeDependentVM.Id > 0)


                        sqlText = "";
                        sqlText = "update EmployeeDependent set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeeDependentVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeeDependentVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeeDependentVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }

                    retResults[2] = EmployeeDependentVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("EmployeeDependent Delete", EmployeeDependentVM.Id + " could not Delete.");
                    }

                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeDependent Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete EmployeeDependent Information.";
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
        public List<EmployeeDependentVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeDependentVM> VMs = new List<EmployeeDependentVM>();
            EmployeeDependentVM vm;
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
 isnull(Dep.Id, 0)						   Id
,ei.EmployeeId							   EmployeeId
,isnull(Dep.Name		, 'NA')			   Name
,isnull(Dep.Relation	, 'NA')			   Relation
,isnull(Dep.DateofBirth	, 'NA')			   DateofBirth
,isnull(Dep.[Address]		, 'NA')		   [Address]
,isnull(Dep.District	, 'NA')			   District
,isnull(Dep.Division	, 'NA')			   Division
,isnull(Dep.Country		, 'NA')			   Country
,isnull(Dep.City		, 'NA')			   City
,isnull(Dep.PostalCode	, 'NA')			   PostalCode
,isnull(Dep.Phone		, 'NA')			   Phone
,isnull(Dep.Mobile		, 'NA')			   Mobile
,isnull(Dep.Fax			, 'NA')		       Fax
,isnull(Dep.IsDependentAllowance			, '0')		       IsDependentAllowance
,Dep.Gender	            
,Dep.EducationQualification
,Dep.BloodGroup  
,isnull(Dep.Remarks		, 'NA')		       Remarks


    From ViewEmployeeInformation ei
		left outer join EmployeeDependent Dep on ei.EmployeeId=Dep.EmployeeId
Where Dep.IsArchive=0 and Dep.isActive=1
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
                    vm = new EmployeeDependentVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.Relation = dr["Relation"].ToString();
                    vm.DateofBirth = Ordinary.StringToDate(dr["DateofBirth"].ToString());
                    vm.Address = dr["Address"].ToString();
                    vm.District = dr["District"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.PostalCode = dr["PostalCode"].ToString();
                    vm.Phone = dr["Phone"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Fax = dr["Fax"].ToString();
                    vm.Gender = dr["Gender"].ToString();
                    vm.EducationQualification = dr["EducationQualification"].ToString();
                    vm.BloodGroup = dr["BloodGroup"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
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
