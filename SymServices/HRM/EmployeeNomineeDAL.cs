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
    public class EmployeeNomineeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeNomineeVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeNomineeVM> VMs = new List<EmployeeNomineeVM>();
            EmployeeNomineeVM vm;
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
,Address
,District
,NID
,Division
,Country
,City
,PostalCode
,PostOffice
,DateofBirth
,BirthCertificateNo
,Phone
,Mobile
,Fax
,ISNULL(IsVaccineDose1Complete, '') IsVaccineDose1Complete
,ISNULL(VaccineDose1Date, '') VaccineDose1Date
,ISNULL(VaccineDose1Name, '') VaccineDose1Name

,ISNULL(IsVaccineDose2Complete, '') IsVaccineDose2Complete
,ISNULL(VaccineDose2Date, '') VaccineDose2Date
,ISNULL(VaccineDose2Name, '') VaccineDose2Name

,ISNULL(IsVaccineDose3Complete, '') IsVaccineDose3Complete
,ISNULL(VaccineDose3Date, '') VaccineDose3Date
,ISNULL(VaccineDose3Name, '') VaccineDose3Name
,VaccineFile1

,VaccineFile3
,VaccineFiles2
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeNominee
Where IsArchive=0
    ORDER BY Name
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                //,ISNULL(j.IsVaccineDose1Complete, 0) IsVaccineDose1Complete
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeNomineeVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.Relation = dr["Relation"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.BirthReg = dr["BirthCertificateNo"].ToString();
                    vm.District = dr["District"].ToString();
                    vm.NID = dr["NID"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.PostalCode = dr["PostalCode"].ToString();
                    vm.PostOffice = dr["PostOffice"].ToString();
                    vm.DateofBirth =  Ordinary.StringToDate(dr["DateofBirth"].ToString());
                    vm.Phone = dr["Phone"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Fax = dr["Fax"].ToString();

                    vm.IsVaccineDose1Complete = Convert.ToBoolean(dr["IsVaccineDose1Complete"]);
                    vm.VaccineDose1Date = Ordinary.StringToDate(dr["VaccineDose1Date"].ToString());
                    vm.VaccineDose1Name = dr["VaccineDose1Name"].ToString();

                    vm.IsVaccineDose2Complete = Convert.ToBoolean(dr["IsVaccineDose2Complete"]);
                    vm.VaccineDose2Date = Ordinary.StringToDate(dr["VaccineDose2Date"].ToString());
                    vm.VaccineDose2Name = dr["VaccineDose2Name"].ToString();

                    vm.IsVaccineDose3Complete = Convert.ToBoolean(dr["IsVaccineDose3Complete"]);
                    vm.VaccineDose3Date = Ordinary.StringToDate(dr["VaccineDose3Date"].ToString());
                    vm.VaccineDose3Name = dr["VaccineDose3Name"].ToString();

                    vm.VaccineFile1 = dr["VaccineFile1"].ToString();
                    //vm.VaccineFile2 = dr["VaccineFile2"].ToString();
                    vm.VaccineFile3 = dr["VaccineFile3"].ToString();
                    vm.VaccineFiles2 = dr["VaccineFiles2"].ToString();

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
        public List<EmployeeNomineeVM> SelectAllByEmployee(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeNomineeVM> employeeNomineeVMs = new List<EmployeeNomineeVM>();
            EmployeeNomineeVM employeeNomineeVM;
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
,Address
,BirthCertificateNo
,District
,NID
,Division
,Country
,City
,PostalCode
,PostOffice
,DateofBirth
,Phone
,Mobile
,Fax
,ISNULL(IsVaccineDose1Complete, '') IsVaccineDose1Complete
,ISNULL(VaccineDose1Date, '') VaccineDose1Date
,ISNULL(VaccineDose1Name, '') VaccineDose1Name

,ISNULL(IsVaccineDose2Complete, '') IsVaccineDose2Complete
,ISNULL(VaccineDose2Date, '') VaccineDose2Date
,ISNULL(VaccineDose2Name, '') VaccineDose2Name

,ISNULL(IsVaccineDose3Complete, '') IsVaccineDose3Complete
,ISNULL(VaccineDose3Date, '') VaccineDose3Date
,ISNULL(VaccineDose3Name, '') VaccineDose3Name

,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeNominee
Where IsArchive=0 and EmployeeId=@EmployeeId
    ORDER BY Name
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                //,ISNULL(j.IsVaccineDose1Complete, 0) IsVaccineDose1Complete
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeNomineeVM = new EmployeeNomineeVM();
                    employeeNomineeVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeNomineeVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeNomineeVM.Name = dr["Name"].ToString();
                    employeeNomineeVM.Relation = dr["Relation"].ToString();
                    employeeNomineeVM.Address = dr["Address"].ToString();
                    employeeNomineeVM.BirthReg = dr["BirthCertificateNo"].ToString();
                    employeeNomineeVM.DateofBirth =  Ordinary.StringToDate(dr["DateofBirth"].ToString());
                    employeeNomineeVM.District = dr["District"].ToString();
                    employeeNomineeVM.NID = dr["NID"].ToString();
                    employeeNomineeVM.Division = dr["Division"].ToString();
                    employeeNomineeVM.Country = dr["Country"].ToString();
                    employeeNomineeVM.City = dr["City"].ToString();
                    employeeNomineeVM.PostalCode = dr["PostalCode"].ToString();
                    employeeNomineeVM.PostOffice = dr["PostOffice"].ToString();
                    employeeNomineeVM.Phone = dr["Phone"].ToString();
                    employeeNomineeVM.Mobile = dr["Mobile"].ToString();
                    employeeNomineeVM.Fax = dr["Fax"].ToString();
                    employeeNomineeVM.IsVaccineDose1Complete = Convert.ToBoolean(dr["IsVaccineDose1Complete"]);
                    //employeeNomineeVM.IsVaccineDose1Complete = Convert.ToBoolean(dr["IsVaccineDose1Complete"]);
                    employeeNomineeVM.VaccineDose1Date = Ordinary.StringToDate(dr["VaccineDose1Date"].ToString());
                    employeeNomineeVM.VaccineDose1Name = dr["VaccineDose1Name"].ToString();

                    employeeNomineeVM.IsVaccineDose2Complete = Convert.ToBoolean(dr["IsVaccineDose2Complete"]);
                    employeeNomineeVM.VaccineDose2Date = Ordinary.StringToDate(dr["VaccineDose2Date"].ToString());
                    employeeNomineeVM.VaccineDose2Name = dr["VaccineDose2Name"].ToString();

                    employeeNomineeVM.IsVaccineDose3Complete = Convert.ToBoolean(dr["IsVaccineDose3Complete"]);
                    employeeNomineeVM.VaccineDose3Date = Ordinary.StringToDate(dr["VaccineDose3Date"].ToString());
                    employeeNomineeVM.VaccineDose3Name = dr["VaccineDose3Name"].ToString();

                    //employeeNomineeVM.VaccineFile1 = dr["VaccineFile1"].ToString();
                    ////employeeNomineeVM.VaccineFile2 = dr["VaccineFile2"].ToString();
                    //employeeNomineeVM.VaccineFile3 = dr["VaccineFile3"].ToString();
                    //employeeNomineeVM.VaccineFiles2 = dr["VaccineFiles2"].ToString();

                    employeeNomineeVM.Remarks = dr["Remarks"].ToString();
                    employeeNomineeVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeNomineeVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeNomineeVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeNomineeVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeNomineeVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeNomineeVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeNomineeVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    employeeNomineeVMs.Add(employeeNomineeVM);
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

            return employeeNomineeVMs;
        }

        public DataTable SelectAllEmployeeForExcel(string EmployeeId)
        {
            #region Variables
            DataTable dt = new DataTable();
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeNomineeVM> employeeNomineeVMs = new List<EmployeeNomineeVM>();
            EmployeeNomineeVM employeeNomineeVM;
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
,Address
,BirthCertificateNo
,District
,NID
,Division
,Country
,City
,PostalCode
,PostOffice
,DateofBirth
,Phone
,Mobile
,Fax
,ISNULL(IsVaccineDose1Complete, '') IsVaccineDose1Complete
,ISNULL(VaccineDose1Date, '') VaccineDose1Date
,ISNULL(VaccineDose1Name, '') VaccineDose1Name

,ISNULL(IsVaccineDose2Complete, '') IsVaccineDose2Complete
,ISNULL(VaccineDose2Date, '') VaccineDose2Date
,ISNULL(VaccineDose2Name, '') VaccineDose2Name

,ISNULL(IsVaccineDose3Complete, '') IsVaccineDose3Complete
,ISNULL(VaccineDose3Date, '') VaccineDose3Date
,ISNULL(VaccineDose3Name, '') VaccineDose3Name

,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeNominee
Where IsArchive=0 and EmployeeId=@EmployeeId
    ORDER BY Name
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);               

                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.Fill(dt);

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

        //==================SelectByID=================
        public EmployeeNomineeVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeNomineeVM employeeNomineeVM = new EmployeeNomineeVM();

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
,Address
,District
,NID
,Division
,Country
,City
,PostalCode
,PostOffice
,DateofBirth
,BirthCertificateNo
,Phone
,Mobile
,Fax
,FileName
,ISNULL(IsVaccineDose1Complete, '') IsVaccineDose1Complete
,ISNULL(VaccineDose1Date, '') VaccineDose1Date
,ISNULL(VaccineDose1Name, '') VaccineDose1Name

,ISNULL(IsVaccineDose2Complete, '') IsVaccineDose2Complete
,ISNULL(VaccineDose2Date, '') VaccineDose2Date
,ISNULL(VaccineDose2Name, '') VaccineDose2Name

,ISNULL(IsVaccineDose3Complete, '') IsVaccineDose3Complete
,ISNULL(VaccineDose3Date, '') VaccineDose3Date
,ISNULL(VaccineDose3Name, '') VaccineDose3Name
,VaccineFile1
,VaccineFile3
,VaccineFiles2
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeNominee
where  id=@Id
     
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                //,ISNULL(j.IsVaccineDose1Complete, 0) IsVaccineDose1Complete
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeNomineeVM.Id = Convert.ToInt32(dr["Id"]);
                    employeeNomineeVM.EmployeeId = dr["EmployeeId"].ToString();
                    employeeNomineeVM.Name = dr["Name"].ToString();
                    employeeNomineeVM.Relation = dr["Relation"].ToString();
                    employeeNomineeVM.Address = dr["Address"].ToString();
                    employeeNomineeVM.BirthReg = dr["BirthCertificateNo"].ToString();
                    employeeNomineeVM.District = dr["District"].ToString();
                    employeeNomineeVM.NID = dr["NID"].ToString();
                    employeeNomineeVM.Division = dr["Division"].ToString();
                    employeeNomineeVM.Country = dr["Country"].ToString();
                    employeeNomineeVM.City = dr["City"].ToString();
                    employeeNomineeVM.DateofBirth =  Ordinary.StringToDate(dr["DateofBirth"].ToString());
                    employeeNomineeVM.PostalCode = dr["PostalCode"].ToString();
                    employeeNomineeVM.PostOffice = dr["PostOffice"].ToString();
                    employeeNomineeVM.Phone = dr["Phone"].ToString();
                    employeeNomineeVM.Mobile = dr["Mobile"].ToString();
                    employeeNomineeVM.Fax = dr["Fax"].ToString();
                    employeeNomineeVM.FileName = dr["FileName"].ToString();

                    employeeNomineeVM.IsVaccineDose1Complete = Convert.ToBoolean(dr["IsVaccineDose1Complete"]);
                    employeeNomineeVM.VaccineDose1Date = Ordinary.StringToDate(dr["VaccineDose1Date"].ToString());
                    employeeNomineeVM.VaccineDose1Name = dr["VaccineDose1Name"].ToString();

                    employeeNomineeVM.IsVaccineDose2Complete = Convert.ToBoolean(dr["IsVaccineDose2Complete"]);
                    employeeNomineeVM.VaccineDose2Date = Ordinary.StringToDate(dr["VaccineDose2Date"].ToString());
                    employeeNomineeVM.VaccineDose2Name = dr["VaccineDose2Name"].ToString();

                    employeeNomineeVM.IsVaccineDose3Complete = Convert.ToBoolean(dr["IsVaccineDose3Complete"]);
                    employeeNomineeVM.VaccineDose3Date = Ordinary.StringToDate(dr["VaccineDose3Date"].ToString());
                    employeeNomineeVM.VaccineDose3Name = dr["VaccineDose3Name"].ToString();

                    employeeNomineeVM.VaccineFile1 = dr["VaccineFile1"].ToString();
                    //employeeNomineeVM.VaccineFile2 = dr["VaccineFile2"].ToString();
                    employeeNomineeVM.VaccineFile3 = dr["VaccineFile3"].ToString();
                    employeeNomineeVM.VaccineFiles2 = dr["VaccineFiles2"].ToString();

                    employeeNomineeVM.Remarks = dr["Remarks"].ToString();
                    employeeNomineeVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    employeeNomineeVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    employeeNomineeVM.CreatedBy = dr["CreatedBy"].ToString();
                    employeeNomineeVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    employeeNomineeVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    employeeNomineeVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    employeeNomineeVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return employeeNomineeVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeeNomineeVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeNominee"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(vm.Name))
                {
                    retResults[1] = "Please Input Employee Nominee Name";
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

                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeNominee ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", vm.Name);
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Nominee Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Nominee Value", "");
                //}
                //#endregion Exist
                #region Save

                //int foundId = (int)objfoundId;
                //if (foundId <= 0)
                if (vm != null)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeNominee(	EmployeeId,Name,Relation,Address,District,NID,Division,Country
                                ,City,PostalCode,PostOffice,Mobile,Phone,Fax,DateofBirth,BirthCertificateNo,FileName,IsVaccineDose1Complete,VaccineDose1Date,VaccineDose1Name,IsVaccineDose2Complete,VaccineDose2Date,VaccineDose2Name,IsVaccineDose3Complete,VaccineDose3Date,VaccineDose3Name,VaccineFile1,VaccineFile3,VaccineFiles2,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@EmployeeId,@Name,@Relation,@Address,@District,@NID,@Division,@Country,@City,@PostalCode,@PostOffice,@Mobile,@Phone,@Fax
                                        ,@DateofBirth,@BirthCertificateNo,@FileName,@IsVaccineDose1Complete,@VaccineDose1Date,@VaccineDose1Name,@IsVaccineDose2Complete,@VaccineDose2Date,@VaccineDose2Name,@IsVaccineDose3Complete,@VaccineDose3Date,@VaccineDose3Name,@VaccineFile1,@VaccineFile3,@VaccineFiles2,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@EmployeeId",vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name );
                    cmdInsert.Parameters.AddWithValue("@Relation", vm.Relation );
                    cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@District", vm.District ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@NID", vm.NID ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Division", vm.Division ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PostalCode", vm.PostalCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PostOffice", vm.PostOffice ?? Convert.DBNull);
                    //cmdInsert.Parameters.AddWithValue("@PostOffice", vm.PostOffice);
                    cmdInsert.Parameters.AddWithValue("@Mobile", vm.Mobile ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@IsVaccineDose1Complete", vm.IsVaccineDose1Complete);
                    cmdInsert.Parameters.AddWithValue("@VaccineDose1Date", Ordinary.DateToString(vm.VaccineDose1Date));
                    cmdInsert.Parameters.AddWithValue("@VaccineDose1Name", vm.VaccineDose1Name ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@IsVaccineDose2Complete", vm.IsVaccineDose2Complete);
                    cmdInsert.Parameters.AddWithValue("@VaccineDose2Date", Ordinary.DateToString(vm.VaccineDose2Date));
                    cmdInsert.Parameters.AddWithValue("@VaccineDose2Name", vm.VaccineDose2Name ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@IsVaccineDose3Complete", vm.IsVaccineDose3Complete);
                    cmdInsert.Parameters.AddWithValue("@VaccineDose3Date", Ordinary.DateToString(vm.VaccineDose3Date));
                    cmdInsert.Parameters.AddWithValue("@VaccineDose3Name", vm.VaccineDose3Name ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@VaccineFile1", vm.VaccineFile1 ?? Convert.DBNull);
                    //cmdInsert.Parameters.AddWithValue("@VaccineFile2", vm.VaccineFile2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@VaccineFile3", vm.VaccineFile3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@VaccineFiles2", vm.VaccineFiles2 ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@Phone", vm.Phone ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Fax", vm.Fax ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@DateofBirth", Ordinary.DateToString(vm.DateofBirth));
                    cmdInsert.Parameters.AddWithValue("@BirthCertificateNo", vm.BirthReg ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FileName", vm.FileName?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    cmdInsert.Transaction = transaction;
					var exeRes = cmdInsert.ExecuteScalar();
					Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Nominee Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Nominee Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Nominee already used";
                    throw new ArgumentNullException("This Employee Nominee already used", "");
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
        public string[] Update(EmployeeNomineeVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Nominee Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToNominee"); }

                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EmployeeNominee set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " Name=@Name,";
                    sqlText += " Relation=@Relation,";
                    sqlText += " Address=@Address,";
                    sqlText += " BirthCertificateNo=@BirthCertificateNo,";
                    sqlText += " District=@District,";
                    sqlText += "NID=@NID, ";
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
                    sqlText += " IsVaccineDose1Complete=@IsVaccineDose1Complete,";
                    sqlText += "VaccineDose1Date=@VaccineDose1Date, ";
                    sqlText += " VaccineDose1Name=@VaccineDose1Name,";

                    sqlText += " IsVaccineDose2Complete=@IsVaccineDose2Complete,";
                    sqlText += "VaccineDose2Date=@VaccineDose2Date, ";
                    sqlText += " VaccineDose2Name=@VaccineDose2Name,";

                    sqlText += " IsVaccineDose3Complete=@IsVaccineDose3Complete,";
                    sqlText += "VaccineDose3Date=@VaccineDose3Date, ";
                    sqlText += " VaccineDose3Name=@VaccineDose3Name,";

                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name );
                    cmdUpdate.Parameters.AddWithValue("@Relation", vm.Relation );
                    cmdUpdate.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BirthCertificateNo", vm.BirthReg ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@District", vm.District ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("NID", vm.NID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Division", vm.Division ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostalCode", vm.PostalCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostOffice", vm.PostOffice ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Phone", vm.Phone ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Mobile", vm.Mobile ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Fax", vm.Fax ?? Convert.DBNull);
                    if (vm.FileName !=null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    }
                    if (vm.VaccineFiles2 != null)
                    {
                        //sqlText += " VaccineFiles2=@VaccineFiles2,";
                        cmdUpdate.Parameters.AddWithValue("@VaccineFiles2", vm.VaccineFiles2 ?? Convert.DBNull);
                    }
                    if (vm.VaccineFile1 != null)
                    {
                        sqlText += " VaccineFile1=@VaccineFile1,";
                    }
                    //if (vm.VaccineFile2 != null)
                    //{
                    //    sqlText += " VaccineFile2=@VaccineFile2,";
                    //}
                    if (vm.VaccineFile3 != null)
                    {
                        sqlText += " VaccineFile3=@VaccineFile3,";
                    }
                    cmdUpdate.Parameters.AddWithValue("@IsVaccineDose1Complete", vm.IsVaccineDose1Complete);
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose1Date", Ordinary.DateToString(vm.VaccineDose1Date));
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose1Name", vm.VaccineDose1Name ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@IsVaccineDose2Complete", vm.IsVaccineDose2Complete);
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose2Date", Ordinary.DateToString(vm.VaccineDose2Date));
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose2Name", vm.VaccineDose2Name ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@IsVaccineDose3Complete", vm.IsVaccineDose3Complete);
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose3Date", Ordinary.DateToString(vm.VaccineDose3Date));
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose3Name", vm.VaccineDose3Name ?? Convert.DBNull);
                    //if (vm.VaccineFiles2 != null)
                    //{
                    //    cmdUpdate.Parameters.AddWithValue("@VaccineFiles2", vm.VaccineFiles2 ?? Convert.DBNull);
                    //}
                    if (vm.VaccineFile1 != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@VaccineFile1", vm.VaccineFile1 ?? Convert.DBNull);
                    }
                    //if (vm.VaccineFile2 != null)
                    //{
                    //    cmdUpdate.Parameters.AddWithValue("@VaccineFile2", vm.VaccineFile2 ?? Convert.DBNull);
                    //}
                    if (vm.VaccineFile3 != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@VaccineFile3", vm.VaccineFile3 ?? Convert.DBNull);
                    }

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DateofBirth",  Ordinary.DateToString(vm.DateofBirth ));
                    //cmdUpdate.Parameters.AddWithValue("@IsActive", employeeNomineeVM.IsActive);
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
                    throw new ArgumentNullException("Nominee Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Nominee.";
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
        public EmployeeNomineeVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeeNomineeVM employeeNomineeVM = new EmployeeNomineeVM();

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
Id
,EmployeeId
,Name
,Relation
,Address
,District
,NID
,Division
,Country
,City
,PostalCode
,Phone
,Fax
,Remarks
,IsVaccineDose1Complete
,VaccineDose1Date
,VaccineDose1Name
,IsActive
,DateofBirth
,BirthCertificateNo
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeNominee   
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
                        employeeNomineeVM.Id = Convert.ToInt32(dr["Id"]);
                        employeeNomineeVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeeNomineeVM.Name = dr["Name"].ToString();
                        employeeNomineeVM.Relation = dr["Relation"].ToString();
                        employeeNomineeVM.Address = dr["Address"].ToString();
                        employeeNomineeVM.BirthReg = dr["BirthCertificateNo"].ToString();
                        employeeNomineeVM.District = dr["District"].ToString();
                        employeeNomineeVM.NID = dr["NID"].ToString();
                        employeeNomineeVM.Division = dr["Division"].ToString();
                        employeeNomineeVM.Country = dr["Country"].ToString();
                        employeeNomineeVM.City = dr["City"].ToString();
                        employeeNomineeVM.PostalCode = dr["PostalCode"].ToString();
                        employeeNomineeVM.Phone = dr["Phone"].ToString();
                        employeeNomineeVM.Fax = dr["Fax"].ToString();

                        employeeNomineeVM.IsVaccineDose1Complete = Convert.ToBoolean(dr["IsVaccineDose1Complete"]);
                        employeeNomineeVM.VaccineDose1Date = dr["VaccineDose1Date"].ToString();
                        employeeNomineeVM.VaccineDose1Name = dr["VaccineDose1Name"].ToString();

                        employeeNomineeVM.Remarks = dr["Remarks"].ToString();
                        employeeNomineeVM.DateofBirth =  Ordinary.StringToDate(dr["DateofBirth"].ToString());
                        employeeNomineeVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeeNomineeVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeeNomineeVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeeNomineeVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeeNomineeVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeeNomineeVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeeNomineeVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return employeeNomineeVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeeNomineeVM EmployeeNomineeVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteNominee"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToNominee"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length>=1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeNominee set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeeNomineeVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeeNomineeVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeeNomineeVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Nominee Delete", EmployeeNomineeVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Nominee Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Nominee Information.";
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
        public List<EmployeeNomineeVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeNomineeVM> VMs = new List<EmployeeNomineeVM>();
            EmployeeNomineeVM vm;
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
 isnull(Nm.Id,0)						  Id
,ei.EmployeeId							EmployeeId
,isnull(Nm.Name	, 'NA')		Name		
,isnull(Nm.Relation	, 'NA')		Relation		
,isnull(Nm.[Address]	, 'NA')		[Address]		
,isnull(Nm.District	, 'NA')		District		
,isnull(Nm.Division	, 'NA')		Division		
,isnull(Nm.Country	, 'NA')		Country		
,isnull(Nm.City	, 'NA')		City		
,isnull(Nm.PostalCode	, 'NA')		PostalCode		
,isnull(Nm.DateofBirth	, 'NA')		DateofBirth		
,isnull(Nm.BirthCertificateNo	, 'NA')		BirthCertificateNo		
,isnull(Nm.Phone	, 'NA')		Phone		
,isnull(Nm.Mobile	, 'NA')		Mobile		
,isnull(Nm.Fax	, 'NA')		Fax		
,isnull(Nm.Remarks		, 'NA')			Remarks
,isnull(Nm.IsActive, 0)			IsActive
,isnull(Nm.IsArchive, 0)			IsArchive
,isnull(Nm.CreatedBy, 'NA')		 CreatedBy
,isnull(Nm.CreatedAt, 'NA')		 CreatedAt
,isnull(Nm.CreatedFrom, 'NA')		CreatedFrom
,isnull(Nm.LastUpdateBy, 'NA')	 LastUpdateBy
,isnull(Nm.LastUpdateAt,	'NA')	 LastUpdateAt
,isnull(Nm.LastUpdateFrom,	'NA')	 LastUpdateFrom   
  
    From ViewEmployeeInformation ei
		left outer join EmployeeNominee Nm on ei.EmployeeId=Nm.EmployeeId
Where ei.IsArchive=0 and ei.isActive=1
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
                sqlText += "   ,Nm.Name ";

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
                    vm = new EmployeeNomineeVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.Relation = dr["Relation"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.BirthReg = dr["BirthCertificateNo"].ToString();
                    vm.District = dr["District"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.PostalCode = dr["PostalCode"].ToString();
                    vm.DateofBirth = Ordinary.StringToDate(dr["DateofBirth"].ToString());
                    vm.Phone = dr["Phone"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Fax = dr["Fax"].ToString();
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
