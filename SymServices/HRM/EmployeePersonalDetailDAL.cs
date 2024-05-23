using SymOrdinary;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.HRM
{
    public class EmployeePersonalDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeePersonalDetailVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePersonalDetailVM> VMs = new List<EmployeePersonalDetailVM>();
            EmployeePersonalDetailVM vm;
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
,OtherId
,Gender_E
,FatherName	
,MotherName	
,SpouseName	
,PersonalContactNo	
,CorporateContactNo	
,CorporateContactLimit
,MaritalStatus_E
,Nationality_E
,DateOfBirth
,NickName
,Smoker
,PassportNumber
,ExpiryDate
,Religion
,TIN
,IsDisable
,KindsOfDisability
,DisabilityFile
,Signature
,BloodGroup_E
,FingerprintFile
,VaccineFile1
,VaccineFile2
,VaccineFile3
,VaccineFiles2
,PlaceOfBirth
,MarriageDate
,SpouseProfession
,SpouseDateOfBirth
,SpouseBloodGroup
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeePersonalDetail
Where IsArchive=0
    ORDER BY NickName
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeePersonalDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.OtherId = dr["OtherId"].ToString();
                    vm.Gender_E = dr["Gender_E"].ToString();
                    vm.FatherName = dr["FatherName"].ToString();
                    vm.MotherName = dr["MotherName"].ToString();
                    vm.SpouseName = dr["SpouseName"].ToString();
                    vm.PersonalContactNo = dr["PersonalContactNo"].ToString();
                    vm.CorporateContactNo = dr["CorporateContactNo"].ToString();
                    vm.CorporateContactLimit = Convert.ToDecimal(dr["CorporateContactLimit"].ToString());
                    vm.MaritalStatus_E = dr["MaritalStatus_E"].ToString();
                    vm.Nationality_E = dr["Nationality_E"].ToString();
                    vm.DateOfBirth = Ordinary.StringToDate(dr["DateOfBirth"].ToString());
                    vm.NickName = dr["NickName"].ToString();
                    vm.Smoker = Convert.ToBoolean(dr["Smoker"]);
                    vm.PassportNumber = dr["PassportNumber"].ToString();
                    vm.ExpiryDate = Ordinary.StringToDate(dr["ExpiryDate"].ToString());
                    vm.Religion = dr["Religion"].ToString();
                    vm.TIN = dr["TIN"].ToString();
                    vm.IsDisable = Convert.ToBoolean(dr["IsDisable"]);
                    vm.KindsOfDisability = dr["KindsOfDisability"].ToString();
                    vm.Signature = dr["Signature"].ToString();
                    vm.BloodGroup_E = dr["BloodGroup_E"].ToString();
                    vm.FingerprintFile = dr["FingerprintFile"].ToString();
                    vm.VaccineFile1 = dr["VaccineFile1"].ToString();
                    vm.VaccineFile2 = dr["VaccineFile2"].ToString();
                    vm.VaccineFile3 = dr["VaccineFile3"].ToString();
                    vm.VaccineFiles2 = dr["VaccineFiles2"].ToString();
                    vm.PlaceOfBirth = dr["PlaceOfBirth"].ToString();
                    vm.MarriageDate = Ordinary.StringToDate(dr["MarriageDate"].ToString());
                    vm.SpouseProfession = dr["SpouseProfession"].ToString();
                    vm.SpouseDateOfBirth = Ordinary.StringToDate(dr["SpouseDateOfBirth"].ToString());
                    vm.SpouseBloodGroup = dr["SpouseBloodGroup"].ToString();

                    //PlaceOfBirth
                    //MarriageDate
                    //SpouseProfession
                    //SpouseDateOfBirth
                    //SpouseBloodGroup


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
        public EmployeePersonalDetailVM SelectByEmployee(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePersonalDetailVM vm = new EmployeePersonalDetailVM();
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
Id
,EmployeeId
,OtherId
,Gender_E
,FatherName	
,MotherName	
,SpouseName	
,PersonalContactNo	
,CorporateContactNo	
,ISNULL(CorporateContactLimit,0) as  CorporateContactLimit
,MaritalStatus_E
,Nationality_E
,DateOfBirth
,NickName
,Smoker
,NID
,NIDFile
,PassportNumber
,PassportFile
,Signature
,ExpiryDate
,Religion
,TIN
,TINFile
,FingerprintFile
,Email
,ISNULL(IsDisable,0) as  IsDisable
,KindsOfDisability
,DisabilityFile
,BloodGroup_E
,PlaceOfBirth
,MarriageDate
,SpouseProfession
,SpouseDateOfBirth
,SpouseBloodGroup
,Remarks
,VaccineFile1
,VaccineFile2
,VaccineFile3
,VaccineFiles2
,NoChildren
,Heightft
,HeightIn
,Weight
,ChestIn
,TINFile
,ISNULL(HRMSCode,'') as  HRMSCode
,ISNULL(WDCode,'') as  WDCode
,ISNULL(TPNCode,'') as  TPNCode
,ISNULL(PersonalEmail,'') as  PersonalEmail
,ISNULL(IsVaccineDose1Complete,0) as  IsVaccineDose1Complete
,ISNULL(VaccineDose1Date,'') as  VaccineDose1Date
,ISNULL(VaccineDose1Name,'') as  VaccineDose1Name
,ISNULL(IsVaccineDose2Complete,0) as  IsVaccineDose2Complete
,ISNULL(VaccineDose2Date,'') as  VaccineDose2Date
,ISNULL(VaccineDose2Name,'') as  VaccineDose2Name
,ISNULL(IsVaccineDose3Complete,0) as  IsVaccineDose3Complete
,ISNULL(VaccineDose3Date,'') as  VaccineDose3Date
,ISNULL(VaccineDose3Name,'') as  VaccineDose3Name

,ISNULL(NoChildren,'') as  NoChildren
,ISNULL(Heightft,'') as  Heightft
,ISNULL(HeightIn,'') as  HeightIn
,ISNULL(Weight,'') as  Weight
,ISNULL(ChestIn,'') as  ChestIn


,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeePersonalDetail
Where IsArchive=0 AND EmployeeId=@EmployeeId
    ORDER BY NickName
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
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.OtherId = dr["OtherId"].ToString();
                    vm.Gender_E = dr["Gender_E"].ToString();
                    vm.FatherName = dr["FatherName"].ToString();
                    vm.MotherName = dr["MotherName"].ToString();
                    vm.SpouseName = dr["SpouseName"].ToString();
                    vm.PersonalContactNo = dr["PersonalContactNo"].ToString();
                    vm.CorporateContactNo = dr["CorporateContactNo"].ToString();
                    vm.CorporateContactLimit = Convert.ToDecimal(dr["CorporateContactLimit"].ToString());
                    vm.MaritalStatus_E = dr["MaritalStatus_E"].ToString();
                    vm.Nationality_E = dr["Nationality_E"].ToString();
                    vm.DateOfBirth = Ordinary.StringToDate(dr["DateOfBirth"].ToString());
                    vm.NickName = dr["NickName"].ToString();
                    vm.Smoker = Convert.ToBoolean(dr["Smoker"]);
                    vm.NID = dr["NID"].ToString();
                    vm.NIDFile = dr["NIDFile"].ToString();
                    vm.PassportNumber = dr["PassportNumber"].ToString();
                    vm.PassportFile = dr["PassportFile"].ToString();
                    vm.ExpiryDate = Ordinary.StringToDate(dr["ExpiryDate"].ToString());
                    vm.Religion = dr["Religion"].ToString();
                    vm.TIN = dr["TIN"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.IsDisable = Convert.ToBoolean(dr["IsDisable"]);
                    vm.KindsOfDisability = dr["KindsOfDisability"].ToString();
                    vm.DisabilityFile = dr["DisabilityFile"].ToString();
                    vm.Signature = dr["Signature"].ToString();
                    vm.BloodGroup_E = dr["BloodGroup_E"].ToString();

                    vm.PlaceOfBirth = dr["PlaceOfBirth"].ToString();
                    vm.MarriageDate = Ordinary.StringToDate(dr["MarriageDate"].ToString());
                    vm.SpouseProfession = dr["SpouseProfession"].ToString();
                    vm.SpouseDateOfBirth = Ordinary.StringToDate(dr["SpouseDateOfBirth"].ToString());
                    vm.SpouseBloodGroup = dr["SpouseBloodGroup"].ToString();

                    vm.HRMSCode = dr["HRMSCode"].ToString();
                    vm.WDCode = dr["WDCode"].ToString();
                    vm.TPNCode = dr["TPNCode"].ToString();
                    vm.PersonalEmail = dr["PersonalEmail"].ToString();
                    vm.IsVaccineDose1Complete = Convert.ToBoolean(dr["IsVaccineDose1Complete"].ToString());
                    vm.VaccineDose1Date = Ordinary.StringToDate(dr["VaccineDose1Date"].ToString());
                    vm.VaccineDose1Name = dr["VaccineDose1Name"].ToString();
                    vm.IsVaccineDose2Complete = Convert.ToBoolean(dr["IsVaccineDose2Complete"].ToString());
                    vm.VaccineDose2Date = Ordinary.StringToDate(dr["VaccineDose2Date"].ToString());
                    vm.VaccineDose2Name = dr["VaccineDose2Name"].ToString();
                    vm.IsVaccineDose3Complete = Convert.ToBoolean(dr["IsVaccineDose3Complete"].ToString());
                    vm.VaccineDose3Date = Ordinary.StringToDate(dr["VaccineDose3Date"].ToString());
                    vm.VaccineDose3Name = dr["VaccineDose3Name"].ToString();

                    vm.NoChildren = dr["NoChildren"].ToString();
                    vm.Heightft = dr["Heightft"].ToString();
                    vm.HeightIn = dr["HeightIn"].ToString();
                    vm.Weight = dr["Weight"].ToString();
                    vm.ChestIn = dr["ChestIn"].ToString();
                    vm.TINFile = dr["TINFile"].ToString();
                    vm.VaccineFile1 = dr["VaccineFile1"].ToString();
                    vm.VaccineFile2 = dr["VaccineFile1"].ToString();
                    vm.VaccineFile3 = dr["VaccineFile3"].ToString();
                    vm.VaccineFiles2 = dr["VaccineFiles2"].ToString();
                    vm.FingerprintFile = dr["FingerprintFile"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
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
        //==================SelectByID=================
        public EmployeePersonalDetailVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePersonalDetailVM vm = new EmployeePersonalDetailVM();

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
,OtherId
,Gender_E
,FatherName	
,MotherName	
,SpouseName	
,PersonalContactNo	
,CorporateContactNo	
,CorporateContactLimit
,MaritalStatus_E
,Nationality_E
,DateOfBirth
,NickName
,Smoker
,NID
,Email
,NIDFile
,PassportNumber
,ExpiryDate
,Religion
,TIN
,FingerprintFile
,IsDisable
,KindsOfDisability
,DisabilityFile
,Signature
,BloodGroup_E

,PlaceOfBirth
,MarriageDate
,SpouseProfession
,SpouseDateOfBirth
,SpouseBloodGroup
,VaccineFile1
,VaccineFile2
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
    From EmployeePersonalDetail
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
                    vm.OtherId = dr["OtherId"].ToString();
                    vm.Gender_E = dr["Gender_E"].ToString();
                    vm.Gender_E = dr["FatherName"].ToString();
                    vm.Gender_E = dr["MotherName"].ToString();
                    vm.Gender_E = dr["SpouseName"].ToString();
                    vm.Gender_E = dr["PersonalContactNo"].ToString();
                    vm.Gender_E = dr["CorporateContactNo"].ToString();
                    vm.Gender_E = dr["CorporateContactLimit"].ToString();
                    vm.MaritalStatus_E = dr["MaritalStatus_E"].ToString();
                    vm.Nationality_E = dr["Nationality_E"].ToString();
                    vm.DateOfBirth = Ordinary.StringToDate(dr["DateOfBirth"].ToString());
                    vm.NickName = dr["NickName"].ToString();
                    vm.Smoker = Convert.ToBoolean(dr["Smoker"]);
                    vm.NID = dr["NID"].ToString();
                    vm.NIDFile = dr["NIDFile"].ToString();
                    vm.FingerprintFile = dr["FingerprintFile"].ToString();
                    vm.PassportNumber = dr["PassportNumber"].ToString();
                    vm.ExpiryDate = Ordinary.StringToDate(dr["ExpiryDate"].ToString());
                    vm.Religion = dr["Religion"].ToString();
                    vm.TIN = dr["TIN"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.IsDisable = Convert.ToBoolean(dr["IsDisable"]);
                    vm.KindsOfDisability = dr["KindsOfDisability"].ToString();
                    vm.Signature = dr["Signature"].ToString();
                    //employeePersonalDetailVM.DisabilityFile = dr["DisabilityFile"].ToString();
                    vm.BloodGroup_E = dr["BloodGroup_E"].ToString();
                    vm.VaccineFile1 = dr["VaccineFile1"].ToString();
                    vm.VaccineFile2 = dr["VaccineFile2"].ToString();
                    vm.VaccineFiles2 = dr["VaccineFiles2"].ToString();
                    vm.PlaceOfBirth = dr["PlaceOfBirth"].ToString();
                    vm.MarriageDate = Ordinary.StringToDate(dr["MarriageDate"].ToString());
                    vm.SpouseProfession = dr["SpouseProfession"].ToString();
                    vm.SpouseDateOfBirth = Ordinary.StringToDate(dr["SpouseDateOfBirth"].ToString());
                    vm.SpouseBloodGroup = dr["SpouseBloodGroup"].ToString();


                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
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
        public string[] Insert(EmployeePersonalDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeePersonalDetail"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(employeePersonalDetailVM.Name))
                //{
                //    retResults[1] = "Please Input Employee PersonalDetail Name";
                //    return retResults;
                //}
                #endregion Validation
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();



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
                if (!string.IsNullOrWhiteSpace(vm.OtherId))
                {
                    if (CompanyName.ToUpper() != "G4S")
                    {
                        #region Exist
                        sqlText = "  ";
                        sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeePersonalDetail ";
                        sqlText += " WHERE OtherId=@OtherId";
                        SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                        cmdExist.Transaction = transaction;
                        cmdExist.Parameters.AddWithValue("@OtherId", vm.OtherId);
                        var exeRes = cmdExist.ExecuteScalar();
                        int objfoundId = Convert.ToInt32(exeRes);

                        if (objfoundId > 0)
                        {
                            retResults[1] = "Employee other Id can't be duplicate!";
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Employee other Id can't be duplicate!", "");
                        }
                        #endregion Exist
                    }
                }
                #region Country - Nationality Check
                var tt = vm.Nationality_E;
                sqlText = " ";
                sqlText += @"SELECT
Id
, EmployeeId
, Country 
From EmployeePermanentAddress
where IsArchive=0 and EmployeeId=@EmployeeId";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.Transaction = transaction;

                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                var ttCountry = "";

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    ttCountry = dr["Country"].ToString();
                }
                dr.Close();

                if (!tt.Contains(ttCountry))
                {
                    retResults[1] = "Employee nationality can't be other than his Country: " + ttCountry + "!";
                    throw new ArgumentNullException("Employee nationality can't be other than his Country: " + ttCountry + "!", "");
                }


                #endregion Country - Nationality Check

                #region Save

                //int foundId = (int)objfoundId;
                if (true)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeePersonalDetail(
EmployeeId,OtherId
,FatherName	
,MotherName	
,SpouseName	
,PersonalContactNo	
,CorporateContactNo	
,CorporateContactLimit
,Gender_E,MaritalStatus_E,Nationality_E,DateOfBirth,NickName,Smoker,NID,NIDFile,Remarks,IsActive
,PassportNumber
,ExpiryDate
,Religion
,TIN
,Email
,IsDisable
,KindsOfDisability
,DisabilityFile
,PassportFile
,Signature
,BloodGroup_E
,PlaceOfBirth
,MarriageDate
,SpouseProfession
,SpouseDateOfBirth
,SpouseBloodGroup
,HRMSCode
,WDCode
,TPNCode
,PersonalEmail
,IsVaccineDose1Complete
,VaccineDose1Date
,VaccineDose1Name
,IsVaccineDose2Complete
,VaccineDose2Date
,VaccineDose2Name
,IsVaccineDose3Complete
,VaccineDose3Date
,VaccineDose3Name

,NoChildren
,Heightft
,HeightIn
,Weight
,ChestIn
,TINFile
,FingerprintFile
,VaccineFile1
,VaccineFile2
,VaccineFile3
,VaccineFiles2
,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@EmployeeId,@OtherId
,@FatherName	
,@MotherName	
,@SpouseName	
,@PersonalContactNo	
,@CorporateContactNo	
,@CorporateContactLimit
,@Gender_E,@MaritalStatus_E,@Nationality_E,@DateOfBirth,@NickName,@Smoker,@NID,@NIDFile,@Remarks,@IsActive
,@PassportNumber
,@ExpiryDate
,@Religion
,@TIN
,@FingerprintFile
,@VaccineFile1
,@VaccineFile2
,@VaccineFile3
,@VaccineFiles2
,@Email
,@IsDisable
,@KindsOfDisability
,@DisabilityFile
,@PassportFile
,@Signature
,@BloodGroup_E
,@PlaceOfBirth
,@MarriageDate
,@SpouseProfession
,@SpouseDateOfBirth
,@SpouseBloodGroup
,@HRMSCode
,@WDCode
,@TPNCode
,@PersonalEmail
,@IsVaccineDose1Complete
,@VaccineDose1Date
,@VaccineDose1Name
,@IsVaccineDose2Complete
,@VaccineDose2Date
,@VaccineDose2Name
,@IsVaccineDose3Complete
,@VaccineDose3Date
,@VaccineDose3Name

,@NoChildren
,@Heightft
,@HeightIn
,@Weight
,@ChestIn
,@TINFile
,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@OtherId", vm.OtherId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FatherName", vm.FatherName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@MotherName", vm.MotherName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@SpouseName", vm.SpouseName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PersonalContactNo", vm.PersonalContactNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CorporateContactNo", vm.CorporateContactNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CorporateContactLimit", vm.CorporateContactLimit);
                    cmdInsert.Parameters.AddWithValue("@MaritalStatus_E", vm.MaritalStatus_E ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Gender_E", vm.Gender_E ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Nationality_E", vm.Nationality_E ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@DateOfBirth", Ordinary.DateToString(vm.DateOfBirth));
                    cmdInsert.Parameters.AddWithValue("@NickName", vm.NickName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Smoker", vm.Smoker);
                    cmdInsert.Parameters.AddWithValue("@NID", vm.NID ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@NIDFile", vm.NIDFile ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@PassportNumber", vm.PassportNumber ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PassportFile", vm.PassportFile ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ExpiryDate", Ordinary.DateToString(vm.ExpiryDate));
                    cmdInsert.Parameters.AddWithValue("@Religion", vm.Religion ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@BloodGroup_E", vm.BloodGroup_E ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TIN", vm.TIN ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsDisable", vm.IsDisable);
                    cmdInsert.Parameters.AddWithValue("@KindsOfDisability", vm.KindsOfDisability ?? Convert.DBNull);
                    if (!vm.IsDisable)
                    {
                        //vm.DisabilityFile = "";
                    }
                    cmdInsert.Parameters.AddWithValue("@DisabilityFile", vm.DisabilityFile ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Signature", vm.Signature ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@PlaceOfBirth", vm.PlaceOfBirth ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@MarriageDate", Ordinary.DateToString(vm.MarriageDate));
                    cmdInsert.Parameters.AddWithValue("@SpouseProfession", vm.SpouseProfession ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@SpouseDateOfBirth", Ordinary.DateToString(vm.SpouseDateOfBirth));
                    cmdInsert.Parameters.AddWithValue("@SpouseBloodGroup", vm.SpouseBloodGroup ?? Convert.DBNull);
                    //PlaceOfBirth
                    //MarriageDate
                    //SpouseProfession
                    //SpouseDateOfBirth
                    //SpouseBloodGroup

                    cmdInsert.Parameters.AddWithValue("@HRMSCode", vm.HRMSCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@WDCode", vm.WDCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TPNCode", vm.TPNCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PersonalEmail", vm.PersonalEmail ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsVaccineDose1Complete", vm.IsVaccineDose1Complete );
                    cmdInsert.Parameters.AddWithValue("@VaccineDose1Date", vm.VaccineDose1Date ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@VaccineDose1Name", vm.VaccineDose1Name ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsVaccineDose2Complete", vm.IsVaccineDose2Complete );
                    cmdInsert.Parameters.AddWithValue("@VaccineDose2Date", vm.VaccineDose2Date ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@VaccineDose2Name", vm.VaccineDose2Name ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsVaccineDose3Complete", vm.IsVaccineDose3Complete );
                    cmdInsert.Parameters.AddWithValue("@VaccineDose3Date", vm.VaccineDose3Date ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@VaccineDose3Name", vm.VaccineDose3Name ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@Heightft", vm.Heightft ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@HeightIn", vm.HeightIn ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Weight", vm.Weight ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@NoChildren", vm.NoChildren ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ChestIn", vm.ChestIn ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TINFile", vm.TINFile ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FingerprintFile", vm.FingerprintFile ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@VaccineFile1", vm.VaccineFile1 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@VaccineFile2", vm.VaccineFile2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@VaccineFile3", vm.VaccineFile3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@VaccineFiles2", vm.VaccineFiles2 ?? Convert.DBNull);

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
                        retResults[1] = "Please Input Employee PersonalDetail Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee PersonalDetail Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Personal Detail already used";
                    throw new ArgumentNullException("This Employee Personal Detail already used", "");
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
        public string[] Update(EmployeePersonalDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee PersonalDetail Update"; //Method Name

            int transResult = 0;

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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                if (!string.IsNullOrWhiteSpace(vm.OtherId))
                {
                    #region Exist
                    sqlText = "  ";
                    sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeePersonalDetail ";
                    sqlText += " WHERE OtherId=@OtherId and Id<>@Id and EmployeeId=@EmployeeId";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist.Parameters.AddWithValue("@OtherId", vm.OtherId);
                    cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    int objfoundId = (int)cmdExist.ExecuteScalar();

                    if (objfoundId > 0)
                    {
                        retResults[1] = "Employee other Id can't be duplicate!";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Employee other Id can't be duplicate!", "");
                    }
                    #endregion Exist
                }

                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeePersonalDetail set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " OtherId=@OtherId,";
                    sqlText += " Gender_E=@Gender_E,";
                    sqlText += " FatherName=@FatherName,";
                    sqlText += " MotherName=@MotherName,";
                    sqlText += " SpouseName=@SpouseName,";
                    sqlText += " PersonalContactNo=@PersonalContactNo,";
                    sqlText += " CorporateContactNo	=@CorporateContactNo,";
                    sqlText += " CorporateContactLimit=@CorporateContactLimit,";

                    sqlText += " MaritalStatus_E=@MaritalStatus_E,";
                    sqlText += " Nationality_E=@Nationality_E,";
                    sqlText += " DateOfBirth=@DateOfBirth,";
                    sqlText += " NickName=@NickName,";
                    sqlText += " Smoker=@Smoker,";
                    sqlText += " NID=@NID,";
                    if (vm.NIDFile != null)
                    {
                        sqlText += " NIDFile=@NIDFile,";
                    }
                    if (vm.TINFile != null)
                    {
                        sqlText += " TINFile=@TINFile,";
                    }
                    if (vm.PassportFile != null)
                    {
                        sqlText += " PassportFile=@PassportFile,";
                    }


                    if (vm.FingerprintFile != null)
                    {
                        sqlText += " FingerprintFile=@FingerprintFile,";
                    }
                    if (vm.VaccineFiles2 != null)
                    {
                        sqlText += " VaccineFiles2=@VaccineFiles2,";
                    }
                     if (vm.VaccineFile1 != null)
                    {
                        sqlText += " VaccineFile1=@VaccineFile1,";
                    }
                     if (vm.VaccineFile2 != null)
                     {
                         sqlText += " VaccineFile2=@VaccineFile2,";
                     }
                     if (vm.VaccineFile3 != null)
                     {
                         sqlText += " VaccineFile3=@VaccineFile3,";
                     }
                    if (vm.SignatureFile != null)
                    {
                        sqlText += " Signature=@Signature,";
                    }
                    
                    sqlText += " PassportNumber=@PassportNumber,";
                    sqlText += " ExpiryDate=@ExpiryDate,";
                    sqlText += " Religion=@Religion,";
                    sqlText += " TIN=@TIN,";
                    sqlText += " IsDisable=@IsDisable,";
                    sqlText += " KindsOfDisability=@KindsOfDisability,";
                    if (!vm.IsDisable)
                    {
                        vm.DisabilityFile = "~";
                    }
                    if (vm.DisabilityFile != null)
                    {
                        sqlText += " DisabilityFile=@DisabilityFile,";
                    }
                    sqlText += " BloodGroup_E=@BloodGroup_E,";

                    sqlText += " PlaceOfBirth=@PlaceOfBirth,";
                    sqlText += " MarriageDate=@MarriageDate,";
                    sqlText += " SpouseProfession=@SpouseProfession,";
                    sqlText += " SpouseDateOfBirth=@SpouseDateOfBirth,";
                    sqlText += " SpouseBloodGroup=@SpouseBloodGroup,";

                    sqlText += " HRMSCode                     =@HRMSCode,";
                    sqlText += " WDCode                       =@WDCode,";
                    sqlText += " TPNCode                      =@TPNCode,";
                    sqlText += " PersonalEmail                =@PersonalEmail,";
                    sqlText += " IsVaccineDose1Complete       =@IsVaccineDose1Complete,";
                    sqlText += " VaccineDose1Date             =@VaccineDose1Date,";
                    sqlText += " VaccineDose1Name             =@VaccineDose1Name,";
                    sqlText += " IsVaccineDose2Complete       =@IsVaccineDose2Complete,";
                    sqlText += " VaccineDose2Date             =@VaccineDose2Date,";
                    sqlText += " VaccineDose2Name             =@VaccineDose2Name,";
                    sqlText += " IsVaccineDose3Complete       =@IsVaccineDose3Complete,";
                    sqlText += " VaccineDose3Date             =@VaccineDose3Date,";
                    sqlText += " VaccineDose3Name             =@VaccineDose3Name,";

                    sqlText += " Heightft             =@Heightft,";
                    sqlText += " HeightIn             =@HeightIn,";
                    sqlText += " Weight             =@Weight,";
                    sqlText += " NoChildren             =@NoChildren,";
                    sqlText += " ChestIn             =@ChestIn,";




                    sqlText += " Remarks=@Remarks,";
                    sqlText += " Email=@Email,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@OtherId", vm.OtherId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Gender_E", vm.Gender_E ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@FatherName", vm.FatherName ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MotherName", vm.MotherName ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@SpouseName", vm.SpouseName ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PersonalContactNo", vm.PersonalContactNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@CorporateContactNo", vm.CorporateContactNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@CorporateContactLimit", vm.CorporateContactLimit);
                    cmdUpdate.Parameters.AddWithValue("@MaritalStatus_E", vm.MaritalStatus_E ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Nationality_E", vm.Nationality_E ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DateOfBirth", Ordinary.DateToString(vm.DateOfBirth));
                    cmdUpdate.Parameters.AddWithValue("@NickName", vm.NickName ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BloodGroup_E", vm.BloodGroup_E ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Smoker", vm.Smoker);
                    cmdUpdate.Parameters.AddWithValue("@NID", vm.NID ?? Convert.DBNull);
                    if (vm.NIDFile != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@NIDFile", vm.NIDFile ?? Convert.DBNull);
                    }
                    if (vm.PassportFile != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@PassportFile", vm.PassportFile ?? Convert.DBNull);
                    }
                    if (vm.Signature != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@Signature", vm.Signature ?? Convert.DBNull);
                    }
                    if (vm.TINFile != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@TINFile", vm.TINFile ?? Convert.DBNull);
                    }
                    if (vm.FingerprintFile != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FingerprintFile", vm.FingerprintFile ?? Convert.DBNull);
                    }
                    if (vm.VaccineFiles2 != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@VaccineFiles2", vm.VaccineFiles2 ?? Convert.DBNull);
                    }
                    if (vm.VaccineFile1 != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@VaccineFile1", vm.VaccineFile1 ?? Convert.DBNull);
                    }
                    if (vm.VaccineFile2 != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@VaccineFile2", vm.VaccineFile2 ?? Convert.DBNull);
                    }
                    if (vm.VaccineFile3 != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@VaccineFile3", vm.VaccineFile3 ?? Convert.DBNull);
                    }
                    cmdUpdate.Parameters.AddWithValue("@PassportNumber", vm.PassportNumber ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ExpiryDate", Ordinary.DateToString(vm.ExpiryDate));
                    cmdUpdate.Parameters.AddWithValue("@Religion", vm.Religion ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TIN", vm.TIN ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsDisable", vm.IsDisable);
                    cmdUpdate.Parameters.AddWithValue("@KindsOfDisability", vm.KindsOfDisability ?? Convert.DBNull);
                    if (!string.IsNullOrWhiteSpace(vm.DisabilityFile))
                    {
                        if (!vm.IsDisable)
                        {
                            vm.DisabilityFile = "";
                        }
                        cmdUpdate.Parameters.AddWithValue("@DisabilityFile", vm.DisabilityFile ?? Convert.DBNull);
                    }

                    
                    cmdUpdate.Parameters.AddWithValue("@PlaceOfBirth", vm.PlaceOfBirth ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MarriageDate", Ordinary.DateToString(vm.MarriageDate));
                    cmdUpdate.Parameters.AddWithValue("@SpouseProfession", vm.SpouseProfession ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@SpouseDateOfBirth", Ordinary.DateToString(vm.SpouseDateOfBirth));
                    cmdUpdate.Parameters.AddWithValue("@SpouseBloodGroup", vm.SpouseBloodGroup ?? Convert.DBNull);



                    cmdUpdate.Parameters.AddWithValue("@HRMSCode", vm.HRMSCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@WDCode", vm.WDCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TPNCode", vm.TPNCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PersonalEmail", vm.PersonalEmail ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsVaccineDose1Complete", vm.IsVaccineDose1Complete);
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose1Date", vm.VaccineDose1Date ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose1Name", vm.VaccineDose1Name ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsVaccineDose2Complete", vm.IsVaccineDose2Complete);
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose2Date", vm.VaccineDose2Date ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose2Name", vm.VaccineDose2Name ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsVaccineDose3Complete", vm.IsVaccineDose3Complete);
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose3Date", vm.VaccineDose3Date ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@VaccineDose3Name", vm.VaccineDose3Name ?? Convert.DBNull);


                    cmdUpdate.Parameters.AddWithValue("@Heightft", vm.Heightft ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@HeightIn", vm.HeightIn ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Weight", vm.Weight ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@NoChildren", vm.NoChildren ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ChestIn", vm.ChestIn ?? Convert.DBNull);


                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    transResult = (int)cmdUpdate.ExecuteNonQuery();

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
                    throw new ArgumentNullException("PersonalDetail Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update PersonalDetail.";
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return retResults;
        }
        //==================Select =================
        public EmployeePersonalDetailVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeePersonalDetailVM employeePersonalDetailVM = new EmployeePersonalDetailVM();

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
,OtherId
,Gender_E
,FatherName	
,MotherName	
,SpouseName	
,PersonalContactNo	
,CorporateContactNo	
,CorporateContactLimit
,MaritalStatus_E
,Nationality_E
,DateOfBirth
,NickName
,Smoker
,PassportNumber
,ExpiryDate
,Religion
,TIN
,IsDisable
,KindsOfDisability
,DisabilityFile
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeePersonalDetail   
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
                        employeePersonalDetailVM.Id = Convert.ToInt32(dr["Id"]);
                        employeePersonalDetailVM.EmployeeId = dr["EmployeeId"].ToString();
                        employeePersonalDetailVM.OtherId = dr["OtherId"].ToString();
                        employeePersonalDetailVM.Gender_E = dr["Gender_E"].ToString();
                        employeePersonalDetailVM.Gender_E = dr["FatherName"].ToString();
                        employeePersonalDetailVM.Gender_E = dr["MotherName"].ToString();
                        employeePersonalDetailVM.Gender_E = dr["SpouseName"].ToString();
                        employeePersonalDetailVM.Gender_E = dr["PersonalContactNo"].ToString();
                        employeePersonalDetailVM.Gender_E = dr["CorporateContactNo"].ToString();
                        employeePersonalDetailVM.Gender_E = dr["CorporateContactLimit"].ToString();
                        employeePersonalDetailVM.MaritalStatus_E = dr["MaritalStatus_E"].ToString();
                        employeePersonalDetailVM.Nationality_E = dr["Nationality_E"].ToString();
                        employeePersonalDetailVM.DateOfBirth = Ordinary.StringToDate(dr["DateOfBirth"].ToString());
                        employeePersonalDetailVM.NickName = dr["NickName"].ToString();
                        employeePersonalDetailVM.Smoker = Convert.ToBoolean(dr["Smoker"]);

                        employeePersonalDetailVM.PassportNumber = dr["PassportNumber"].ToString();
                        employeePersonalDetailVM.ExpiryDate = Ordinary.StringToDate(dr["ExpiryDate"].ToString());
                        employeePersonalDetailVM.Religion = dr["Religion"].ToString();
                        employeePersonalDetailVM.TIN = dr["TIN"].ToString();
                        employeePersonalDetailVM.IsDisable = Convert.ToBoolean(dr["IsDisable"]);
                        employeePersonalDetailVM.KindsOfDisability = dr["KindsOfDisability"].ToString();
                        //employeePersonalDetailVM.DisabilityFile = dr["DisabilityFile"].ToString();


                        employeePersonalDetailVM.Remarks = dr["Remarks"].ToString();
                        employeePersonalDetailVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        employeePersonalDetailVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        employeePersonalDetailVM.CreatedBy = dr["CreatedBy"].ToString();
                        employeePersonalDetailVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        employeePersonalDetailVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        employeePersonalDetailVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        employeePersonalDetailVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return employeePersonalDetailVM;
        }
        //==================Delete =================
        public string[] Delete(EmployeePersonalDetailVM EmployeePersonalDetailVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeletePersonalDetail"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToPersonalDetail"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used
                if (EmployeePersonalDetailVM.Id > 0)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeePersonalDetail set";
                    sqlText += " IsArchive=@IsArchive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", EmployeePersonalDetailVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeePersonalDetailVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeePersonalDetailVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeePersonalDetailVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = EmployeePersonalDetailVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("PersonalDetail Delete", EmployeePersonalDetailVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("PersonalDetail Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete PersonalDetail Information.";
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
