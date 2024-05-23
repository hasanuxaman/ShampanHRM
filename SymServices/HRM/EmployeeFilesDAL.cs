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
   public class EmployeeFilesDAL
   {
       #region Global Variables
       private const string FieldDelimeter = DBConstant.FieldDelimeter;
       private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

       #endregion
       #region Methods
       //==================SelectAll=================
       public List<EmployeeFilesVM> SelectAll()
       {

           #region Variables

           SqlConnection currConn = null;
           string sqlText = "";
           List<EmployeeFilesVM> VMs = new List<EmployeeFilesVM>();
           EmployeeFilesVM vm;
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
,EmployeePersonalDetail_Fingerprint
,EmployeePersonalDetail_PassportFile
,EmployeePersonalDetail_TINFiles
,EmployeePersonalDetail_NIDFile
,EmployeePersonalDetail_VaccineFile1
,EmployeePersonalDetail_VaccineFiles2
,EmployeePersonalDetail_VaccineFile3
,EmployeePersonalDetail_DisabilityFile
,EmployeeNominee_VaccineFile1
,EmployeeNominee_VaccineFile2
,EmployeeNominee_VaccineFile3
,SignatureFiles
,FileName
,Employeedependent_VaccineFile1
,Employeedependent_VaccineFile2
,Employeedependent_VaccineFile3
,edu_Certificate
,Lng_Achivement
,Experience_Certificate
,Extra_FileName
,BillVoucher
,AssetFileName
,Certificate
,IsActive
,IsArchive
    From EmployeeFile
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
                   vm = new EmployeeFilesVM();
                   vm.Id = Convert.ToInt32(dr["Id"]);
                   vm.EmployeeId = dr["EmployeeId"].ToString();
                   vm.EmployeePersonalDetail_Fingerprint = dr["EmployeePersonalDetail_Fingerprint"].ToString();
                   vm.EmployeePersonalDetail_PassportFile = dr["EmployeePersonalDetail_PassportFile"].ToString();
                   vm.EmployeePersonalDetail_NIDFile = dr["EmployeePersonalDetail_NIDFile"].ToString();
                   vm.EmployeePersonalDetail_TINFiles = dr["EmployeePersonalDetail_TINFiles"].ToString();
                   vm.SignatureFiles = dr["SignatureFiles"].ToString();
                   vm.FileName = dr["FileName"].ToString();
                   vm.EmployeePersonalDetail_VaccineFile1 = dr["EmployeePersonalDetail_VaccineFile1"].ToString();
                   vm.EmployeePersonalDetail_VaccineFiles2 = dr["EmployeePersonalDetail_VaccineFiles2"].ToString();
                   vm.EmployeePersonalDetail_VaccineFile3 = dr["EmployeePersonalDetail_VaccineFile3"].ToString();
                   vm.EmployeeNominee_VaccineFile1 = dr["EmployeeNominee_VaccineFile1"].ToString();
                   vm.EmployeeNominee_VaccineFile2 = dr["EmployeeNominee_VaccineFile2"].ToString();
                   vm.EmployeeNominee_VaccineFile3 = dr["EmployeeNominee_VaccineFile3"].ToString();
                   vm.Employeedependent_VaccineFile1 = dr["Employeedependent_VaccineFile1"].ToString();
                   vm.Employeedependent_VaccineFile2 = dr["Employeedependent_VaccineFile2"].ToString();
                   vm.Employeedependent_VaccineFile3 = dr["Employeedependent_VaccineFile3"].ToString();
                   vm.edu_Certificate = dr["edu_Certificate"].ToString();
                   vm.Lng_Achivement = dr["Lng_Achivement"].ToString();
                   vm.Experience_Certificate = dr["Experience_Certificate"].ToString();
                   vm.Extra_FileName = dr["Extra_FileName"].ToString();
                   vm.EmployeePersonalDetail_DisabilityFile = dr["EmployeePersonalDetail_DisabilityFile"].ToString();
                   vm.PassportVisa = dr["PassportVisa"].ToString();
                   vm.BillVoucher = dr["BillVoucher"].ToString();
                   vm.AssetFileName = dr["AssetFileName"].ToString();
                   vm.Certificate = dr["Certificate"].ToString();
                   vm.Remarks = dr["Remarks"].ToString();
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

       public EmployeeFilesVM SelectByEmployeeId(string EmployeeId)
       {

           #region Variables

           SqlConnection currConn = null;
           string sqlText = "";
           EmployeeFilesVM employeeFilesVM = new EmployeeFilesVM();

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
,EmployeePersonalDetail_Fingerprint
,EmployeePersonalDetail_PassportFile
,EmployeePersonalDetail_TINFiles
,EmployeePersonalDetail_NIDFile
,EmployeePersonalDetail_VaccineFile1
,EmployeePersonalDetail_VaccineFiles2
,EmployeePersonalDetail_VaccineFile3
,EmployeePersonalDetail_DisabilityFile
,SignatureFiles
,FileName
,EmployeeNominee_VaccineFile1
,EmployeeNominee_VaccineFile2
,EmployeeNominee_VaccineFile3
,Employeedependent_VaccineFile1
,Employeedependent_VaccineFile2
,Employeedependent_VaccineFile3
,edu_Certificate
,Lng_Achivement
,Experience_Certificate
,Extra_FileName
,PassportVisa
,BillVoucher
,AssetFileName
,Certificate
,IsActive
,IsArchive

    From EmployeeFile
where  EmployeeId=@EmployeeId
     
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
                   employeeFilesVM.Id = Convert.ToInt32(dr["Id"]);
                   employeeFilesVM.EmployeeId = dr["EmployeeId"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_Fingerprint = dr["EmployeePersonalDetail_Fingerprint"].ToString();
                   employeeFilesVM.SignatureFiles = dr["SignatureFiles"].ToString();
                   employeeFilesVM.FileName = dr["FileName"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_PassportFile = dr["EmployeePersonalDetail_PassportFile"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_TINFiles = dr["EmployeePersonalDetail_TINFiles"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_NIDFile = dr["EmployeePersonalDetail_NIDFile"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_VaccineFile1 = dr["EmployeePersonalDetail_VaccineFile1"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_VaccineFiles2 = dr["EmployeePersonalDetail_VaccineFiles2"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_VaccineFile3 = dr["EmployeePersonalDetail_VaccineFile3"].ToString();
                   employeeFilesVM.EmployeeNominee_VaccineFile1 = dr["EmployeeNominee_VaccineFile1"].ToString();
                   employeeFilesVM.EmployeeNominee_VaccineFile2 = dr["EmployeeNominee_VaccineFile2"].ToString();
                   employeeFilesVM.EmployeeNominee_VaccineFile3 = dr["EmployeeNominee_VaccineFile3"].ToString();
                   employeeFilesVM.Employeedependent_VaccineFile1 = dr["Employeedependent_VaccineFile1"].ToString();
                   employeeFilesVM.Employeedependent_VaccineFile2 = dr["Employeedependent_VaccineFile2"].ToString();
                   employeeFilesVM.Employeedependent_VaccineFile3 = dr["Employeedependent_VaccineFile3"].ToString();
                   employeeFilesVM.edu_Certificate = dr["edu_Certificate"].ToString();
                   employeeFilesVM.Lng_Achivement = dr["Lng_Achivement"].ToString();
                   employeeFilesVM.Experience_Certificate = dr["Experience_Certificate"].ToString();
                   employeeFilesVM.Extra_FileName = dr["Extra_FileName"].ToString();
                   employeeFilesVM.PassportVisa = dr["PassportVisa"].ToString();
                   employeeFilesVM.BillVoucher = dr["BillVoucher"].ToString();
                   employeeFilesVM.AssetFileName = dr["AssetFileName"].ToString();
                   employeeFilesVM.Certificate = dr["Certificate"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_DisabilityFile = dr["EmployeePersonalDetail_DisabilityFile"].ToString();

                   employeeFilesVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                   employeeFilesVM.IsArchive = Convert.ToBoolean(dr["IsActive"]);

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

           return employeeFilesVM;
       }

       //==================SelectByID=================
       public EmployeeFilesVM SelectById(int Id)
       {

           #region Variables

           SqlConnection currConn = null;
           string sqlText = "";
           EmployeeFilesVM employeeFilesVM = new EmployeeFilesVM();

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
,EmployeePersonalDetail_Fingerprint
,EmployeePersonalDetail_TINFiles
,EmployeePersonalDetail_NIDFile
,EmployeePersonalDetail_PassportFile
,EmployeePersonalDetail_TINFiles
,EmployeePersonalDetail_VaccineFile1
,EmployeePersonalDetail_VaccineFiles2
,EmployeePersonalDetail_VaccineFile3
,EmployeePersonalDetail_DisabilityFile
,SignatureFiles
,FileName
,EmployeeNominee_VaccineFile1
,EmployeeNominee_VaccineFile2
,EmployeeNominee_VaccineFile3
,Employeedependent_VaccineFile1
,Employeedependent_VaccineFile2
,Employeedependent_VaccineFile3
,edu_Certificate
,Lng_Achivement
,Experience_Certificate
,Extra_FileName
,PassportVisa
,BillVoucher
,AssetFileName
,Certificate
,IsActive
,IsArchive
    From EmployeeFile
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
                   employeeFilesVM.Id = Convert.ToInt32(dr["Id"]);
                   employeeFilesVM.EmployeeId = dr["EmployeeId"].ToString();

                   employeeFilesVM.EmployeePersonalDetail_Fingerprint = dr["EmployeePersonalDetail_Fingerprint"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_TINFiles = dr["EmployeePersonalDetail_TINFiles"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_NIDFile = dr["EmployeePersonalDetail_NIDFile"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_PassportFile = dr["EmployeePersonalDetail_PassportFile"].ToString();
                   employeeFilesVM.SignatureFiles = dr["SignatureFiles"].ToString();
                   employeeFilesVM.FileName = dr["FileName"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_VaccineFile1 = dr["EmployeePersonalDetail_VaccineFile1"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_VaccineFiles2 = dr["EmployeePersonalDetail_VaccineFiles2"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_VaccineFile3 = dr["EmployeePersonalDetail_VaccineFile3"].ToString();
                   employeeFilesVM.EmployeePersonalDetail_DisabilityFile = dr["EmployeePersonalDetail_DisabilityFile"].ToString();
                   employeeFilesVM.EmployeeNominee_VaccineFile1 = dr["EmployeeNominee_VaccineFile1"].ToString();
                   employeeFilesVM.EmployeeNominee_VaccineFile2 = dr["EmployeeNominee_VaccineFile2"].ToString();
                   employeeFilesVM.EmployeeNominee_VaccineFile3 = dr["EmployeeNominee_VaccineFile3"].ToString();
                   employeeFilesVM.Employeedependent_VaccineFile1 = dr["Employeedependent_VaccineFile1"].ToString();
                   employeeFilesVM.Employeedependent_VaccineFile2 = dr["Employeedependent_VaccineFile2"].ToString();
                   employeeFilesVM.Employeedependent_VaccineFile3 = dr["Employeedependent_VaccineFile3"].ToString();
                   employeeFilesVM.edu_Certificate = dr["edu_Certificate"].ToString();
                   employeeFilesVM.Lng_Achivement = dr["Lng_Achivement"].ToString();
                   employeeFilesVM.Experience_Certificate = dr["Experience_Certificate"].ToString();
                   employeeFilesVM.Extra_FileName = dr["Extra_FileName"].ToString();
                   employeeFilesVM.PassportVisa = dr["PassportVisa"].ToString();
                   employeeFilesVM.BillVoucher = dr["BillVoucher"].ToString();
                   employeeFilesVM.AssetFileName = dr["AssetFileName"].ToString();
                   employeeFilesVM.Certificate = dr["Certificate"].ToString();
                   employeeFilesVM.Remarks = dr["Remarks"].ToString();
                   employeeFilesVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                   employeeFilesVM.IsArchive = Convert.ToBoolean(dr["IsActive"]);

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

           return employeeFilesVM;
       }


     
       //==================Insert =================
//       public string[] Insert(EmployeeFilesVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
//       {

//           #region Initializ
//           string sqlText = "";
          
//           string[] retResults = new string[6];
//           retResults[0] = "Fail";//Success or Fail
//           retResults[1] = "Fail";// Success or Fail Message
//           //retResults[2] = Id.ToString();// Return Id
//           retResults[3] = sqlText; //  SQL Query
//           retResults[4] = "ex"; //catch ex
//           retResults[5] = "InsertEmployeeFile"; //Method Name
//           SqlConnection currConn = null;
//           SqlTransaction transaction = null;


//           #endregion

//           #region Try

//           try
//           {

             

//               #region open connection and transaction
//               #region New open connection and transaction
//               if (VcurrConn != null)
//               {
//                   currConn = VcurrConn;
//               }

//               if (Vtransaction != null)
//               {
//                   transaction = Vtransaction;
//               }

//               #endregion New open connection and transaction

//               if (currConn == null)
//               {
//                   currConn = _dbsqlConnection.GetConnection();
//                   if (currConn.State != ConnectionState.Open)
//                   {
//                       currConn.Open();
//                   }
//               }

//               if (transaction == null)
//               {
//                   transaction = currConn.BeginTransaction("");
//               }

//               #endregion open connection and transaction
              
//                   sqlText = "  ";
//                   sqlText += @" INSERT INTO EmployeeFile(	
//
//EmployeeId
//,Remarks
//,NIDFile
//,PassportFile
//,TINFile
//,FingerprintFile
//,VaccineFile1
//,VaccineFiles2
//,VaccineFile3
//,IsActive
//,IsArchive
//
//) VALUES (
//
//@EmployeeId
//,@Remarks
//,@NIDFile
//,@PassportFile
//,@TINFile
//,@FingerprintFile
//,@VaccineFile1
//,@VaccineFiles2
//,@VaccineFile3
//,@IsActive
//,@IsArchive
//) SELECT SCOPE_IDENTITY()";

//                   SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
//                   cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                  
                   
//                   cmdInsert.Parameters.AddWithValue("@NIDFile", vm.NIDFile ?? Convert.DBNull);
//                   cmdInsert.Parameters.AddWithValue("@PassportFile", vm.PassportFile ?? Convert.DBNull);
//                   cmdInsert.Parameters.AddWithValue("@TINFile", vm.TINFile ?? Convert.DBNull);
//                   cmdInsert.Parameters.AddWithValue("@VaccineFile1", vm.VaccineFile1 ?? Convert.DBNull);
//                   cmdInsert.Parameters.AddWithValue("@VaccineFile3", vm.VaccineFile3 ?? Convert.DBNull);
//                   cmdInsert.Parameters.AddWithValue("@VaccineFiles2", vm.VaccineFiles2 ?? Convert.DBNull);
//                   cmdInsert.Parameters.AddWithValue("@FingerprintFile", vm.FingerprintFile ?? Convert.DBNull);
                   
//                   //cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
//                   cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
//                   cmdInsert.Parameters.AddWithValue("@IsActive", true);
//                   cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                  

//                   cmdInsert.Transaction = transaction;
//                   var exeRes = cmdInsert.ExecuteScalar();
                   


//               #endregion Save
//               #region Commit
//               if (Vtransaction == null)
//               {
//                   if (transaction != null)
//                   {
//                       transaction.Commit();
//                   }
//               }

//               #endregion Commit

//               #region SuccessResult

//               retResults[0] = "Success";
//               retResults[1] = "Data Save Successfully";
//               //retResults[2] = Id.ToString();

//               #endregion SuccessResult

//           }

//           #endregion try

//           #region Catch and Finall



//           catch (Exception ex)
//           {
//               retResults[0] = "Fail";//Success or Fail
//               retResults[4] = ex.Message.ToString(); //catch ex

//               if (Vtransaction == null) { transaction.Rollback(); }
//               return retResults;
//           }

//           finally
//           {
//               if (VcurrConn == null)
//               {
//                   if (currConn != null)
//                   {
//                       if (currConn.State == ConnectionState.Open)
//                       {
//                           currConn.Close();
//                       }
//                   }
//               }
//           }


//           #endregion

//           #region Results

//           return retResults;
//           #endregion


//       }


       public string[] Insert(EmployeeFilesVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
           retResults[5] = "EmployeeFile"; //Method Name

           SqlConnection currConn = null;
           SqlTransaction transaction = null;
           #endregion

           #region Try

           try
           {

               
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
               
              

              

               //int foundId = (int)objfoundId;
               if (true)
               {

                    sqlText = "  ";
                   sqlText += @" INSERT INTO EmployeeFile(
EmployeeId
,EmployeePersonalDetail_Fingerprint
,EmployeePersonalDetail_PassportFile
,EmployeePersonalDetail_TINFiles
,EmployeePersonalDetail_NIDFile
,EmployeePersonalDetail_VaccineFile1
,EmployeePersonalDetail_VaccineFiles2
,EmployeePersonalDetail_VaccineFile3
,EmployeePersonalDetail_DisabilityFile
,SignatureFiles
,FileName
,EmployeeNominee_VaccineFile1
,EmployeeNominee_VaccineFile2
,EmployeeNominee_VaccineFile3

,Employeedependent_VaccineFile1
,Employeedependent_VaccineFile2
,Employeedependent_VaccineFile3
,edu_Certificate
,Lng_Achivement
,Experience_Certificate
,Extra_FileName
,PassportVisa
,BillVoucher
,AssetFileName
,Certificate
,IsArchive
,IsActive
) VALUES (
@EmployeeId
,@EmployeePersonalDetail_Fingerprint
,@EmployeePersonalDetail_PassportFile
,@EmployeePersonalDetail_TINFiles
,@EmployeePersonalDetail_NIDFile
,@EmployeePersonalDetail_VaccineFile1
,@EmployeePersonalDetail_VaccineFiles2
,@EmployeePersonalDetail_VaccineFile3
,@EmployeePersonalDetail_DisabilityFile
,@SignatureFiles
,@FileName
,@EmployeeNominee_VaccineFile1
,@EmployeeNominee_VaccineFile2
,@EmployeeNominee_VaccineFile3
,@Employeedependent_VaccineFile1
,@Employeedependent_VaccineFile2
,@Employeedependent_VaccineFile3
,@edu_Certificate
,@Lng_Achivement
,@Experience_Certificate
,@Extra_FileName
,@PassportVisa
,@BillVoucher
,@AssetFileName
,@Certificate
,@IsArchive
,@IsActive
) SELECT SCOPE_IDENTITY()";

                   SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                   cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                   cmdInsert.Parameters.AddWithValue("@EmployeePersonalDetail_Fingerprint", vm.EmployeePersonalDetail_Fingerprint ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@EmployeePersonalDetail_TINFiles", vm.EmployeePersonalDetail_TINFiles ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@EmployeePersonalDetail_PassportFile", vm.EmployeePersonalDetail_PassportFile ?? Convert.DBNull); 
                   cmdInsert.Parameters.AddWithValue("@EmployeePersonalDetail_NIDFile", vm.EmployeePersonalDetail_NIDFile ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@EmployeePersonalDetail_VaccineFile1", vm.EmployeePersonalDetail_VaccineFile1 ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@EmployeePersonalDetail_VaccineFiles2", vm.EmployeePersonalDetail_VaccineFiles2 ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@EmployeePersonalDetail_VaccineFile3", vm.EmployeePersonalDetail_VaccineFile3 ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@EmployeePersonalDetail_DisabilityFile", vm.EmployeePersonalDetail_DisabilityFile ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@SignatureFiles", vm.SignatureFiles ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@EmployeeNominee_VaccineFile1", vm.EmployeeNominee_VaccineFile1 ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@EmployeeNominee_VaccineFile2", vm.EmployeeNominee_VaccineFile2 ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@EmployeeNominee_VaccineFile3", vm.EmployeeNominee_VaccineFile3 ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@Employeedependent_VaccineFile1", vm.Employeedependent_VaccineFile1 ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@Employeedependent_VaccineFile2", vm.Employeedependent_VaccineFile2 ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@Employeedependent_VaccineFile3", vm.Employeedependent_VaccineFile3 ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@edu_Certificate", vm.edu_Certificate ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@Lng_Achivement", vm.Lng_Achivement ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@Experience_Certificate", vm.Experience_Certificate ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@Extra_FileName", vm.Extra_FileName ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@PassportVisa", vm.PassportVisa ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@BillVoucher", vm.BillVoucher ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@AssetFileName", vm.AssetFileName ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@Certificate", vm.Certificate ?? Convert.DBNull);
                   cmdInsert.Parameters.AddWithValue("@IsActive", true);
                   cmdInsert.Parameters.AddWithValue("@IsArchive", false);
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

      
       //public string[] Update(EmployeeFilesVM employeeFilesVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
       //{
       //    #region Variables

       //    string[] retResults = new string[6];
       //    retResults[0] = "Fail";//Success or Fail
       //    retResults[1] = "Fail";// Success or Fail Message
       //    retResults[2] = "0";// Return Id
       //    retResults[3] = "sqlText"; //  SQL Query
       //    retResults[4] = "ex"; //catch ex
       //    retResults[5] = "EmployeeFileUpdate"; //Method Name

       //    int transResult = 0;

       //    string sqlText = "";
       //    SqlConnection currConn = null;
       //    SqlTransaction transaction = null;
       //    bool iSTransSuccess = false;

       //    #endregion
       //    try
       //    {



       //        #region open connection and transaction
       //        #region New open connection and transaction
       //        if (VcurrConn != null)
       //        {
       //            currConn = VcurrConn;
       //        }

       //        if (Vtransaction != null)
       //        {
       //            transaction = Vtransaction;
       //        }

       //        #endregion New open connection and transaction

       //        if (currConn == null)
       //        {
       //            currConn = _dbsqlConnection.GetConnection();
       //            if (currConn.State != ConnectionState.Open)
       //            {
       //                currConn.Open();
       //            }
       //        }

       //        if (transaction == null) { transaction = currConn.BeginTransaction("UpdateEmployeeFileUpdate"); }

       //        #endregion open connection and transaction
       //        #region Exist
       //        sqlText = "  ";
       //        sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeFile ";
       //        sqlText += " WHERE EmployeeId=@EmployeeId  AND Id<>@Id";
       //        SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
       //        cmdExist.Transaction = transaction;
       //        cmdExist.Parameters.AddWithValue("@Id", employeeFilesVM.Id);
       //        cmdExist.Parameters.AddWithValue("@EmployeeId", employeeFilesVM.EmployeeId);

       //        var exeRes = cmdExist.ExecuteScalar();
       //        int exists = Convert.ToInt32(exeRes);
       //        if (exists > 0)
       //        {
       //            retResults[1] = "This employee Files already used";
       //            throw new ArgumentNullException("Please Input employee Files Value", "");
       //        }
       //        #endregion Exist

       //        if (employeeFilesVM != null)
       //        {
       //            #region Update Settings

       //            sqlText = "";

       //            if (employeeFilesVM.EmployeePersonalDetail_Fingerprint != null)
       //            {
       //                sqlText += " EmployeePersonalDetail_Fingerprint=@EmployeePersonalDetail_Fingerprint,";
       //            }
       //            sqlText += " Remarks=@Remarks,";
       //            sqlText += " IsActive=@IsActive,";
       //            sqlText += " LastUpdateBy=@LastUpdateBy,";
       //            sqlText += " LastUpdateAt=@LastUpdateAt,";
       //            sqlText += " LastUpdateFrom=@LastUpdateFrom";
       //            sqlText += " where Id=@Id";
       //            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
       //            cmdUpdate.Parameters.AddWithValue("@Id", employeeFilesVM.Id);
       //            cmdUpdate.Parameters.AddWithValue("@EmployeeId", employeeFilesVM.EmployeeId);

       //            if (employeeFilesVM.EmployeePersonalDetail_Fingerprint != null)
       //            {
       //                cmdUpdate.Parameters.AddWithValue("@EmployeePersonalDetail_Fingerprint", employeeFilesVM.EmployeePersonalDetail_Fingerprint ?? Convert.DBNull);
       //            }
       //            cmdUpdate.Transaction = transaction;
       //            exeRes = cmdUpdate.ExecuteNonQuery();
       //            transResult = Convert.ToInt32(exeRes);

       //            retResults[2] = employeeFilesVM.Id.ToString();// Return Id
       //            retResults[3] = sqlText; //  SQL Query

       //            #region Commit

       //            if (transResult <= 0)
       //            {
       //            }

       //            #endregion Commit

       //            #endregion Update Settings
       //            iSTransSuccess = true;
       //        }
       //        else
       //        {
       //            throw new ArgumentNullException("employee Files Update", "Could not found any item.");
       //        }


       //        if (iSTransSuccess == true)
       //        {
       //            if (Vtransaction == null)
       //            {
       //                if (transaction != null)
       //                {
       //                    transaction.Commit();
       //                }
       //            }
       //            retResults[0] = "Success";
       //            retResults[1] = "Data Update Successfully.";

       //        }
       //        else
       //        {
       //            retResults[1] = "Unexpected error to update employee Files.";
       //            throw new ArgumentNullException("", "");
       //        }

       //    }
       //    #region catch
       //    catch (Exception ex)
       //    {
       //        retResults[0] = "Fail";//Success or Fail
       //        retResults[4] = ex.Message; //catch ex
       //        if (Vtransaction == null) { transaction.Rollback(); }
       //        return retResults;
       //    }
       //    finally
       //    {
       //        if (VcurrConn == null)
       //        {
       //            if (currConn != null)
       //            {
       //                if (currConn.State == ConnectionState.Open)
       //                {
       //                    currConn.Close();
       //                }
       //            }
       //        }
       //    }

       //    #endregion

       //    return retResults;
       //}





       //==================Update =================

       public string[] Update(EmployeeFilesVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
       {
           #region Variables

           string[] retResults = new string[6];
           retResults[0] = "Fail";//Success or Fail
           retResults[1] = "Fail";// Success or Fail Message
           retResults[2] = "0";
           retResults[3] = "sqlText"; //  SQL Query
           retResults[4] = "ex"; //catch ex
           retResults[5] = "EmployeeFile Update"; //Method Name

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
               
                   #region Exist
                   sqlText = "  ";
                   sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeFile ";
                   sqlText += " WHERE EmployeeId=@EmployeeId  AND Id<>@Id";
                   SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                   cmdExist.Transaction = transaction;
                   cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                   cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                   
                   #endregion Exist
             

               if (vm != null)
               {
                   #region Update Settings

                   sqlText = "";
                   sqlText = "update EmployeeFile set";
                   sqlText += " EmployeeId=@EmployeeId,";

                   if (vm.EmployeePersonalDetail_Fingerprint != null)
                   {
                       sqlText += " EmployeePersonalDetail_Fingerprint=@EmployeePersonalDetail_Fingerprint,";
                   }
                   if (vm.EmployeePersonalDetail_PassportFile != null)
                   {
                       sqlText += " EmployeePersonalDetail_PassportFile=@EmployeePersonalDetail_PassportFile,";
                   }
                   if (vm.EmployeePersonalDetail_NIDFile != null)
                   {
                       sqlText += " EmployeePersonalDetail_NIDFile=@EmployeePersonalDetail_NIDFile,";
                   }
                   
                   if (vm.EmployeePersonalDetail_VaccineFile1 != null)
                   {
                       sqlText += " EmployeePersonalDetail_VaccineFile1=@EmployeePersonalDetail_VaccineFile1,";
                   }
                   if (vm.EmployeePersonalDetail_VaccineFiles2 != null)
                   {
                       sqlText += " EmployeePersonalDetail_VaccineFiles2=@EmployeePersonalDetail_VaccineFiles2,";
                   }
                   if (vm.EmployeePersonalDetail_VaccineFile3 != null)
                   {
                       sqlText += " EmployeePersonalDetail_VaccineFile3=@EmployeePersonalDetail_VaccineFile3,";
                   }
                   if (vm.EmployeePersonalDetail_DisabilityFile != null)
                   {
                       sqlText += " EmployeePersonalDetail_DisabilityFile=@EmployeePersonalDetail_DisabilityFile,";
                   }
                   if (vm.SignatureFiles != null)
                   {
                       sqlText += " SignatureFiles=@SignatureFiles,";
                   }
                   if (vm.FileName != null)
                   {
                       sqlText += " FileName=@FileName,";
                   }
                   if(vm.EmployeeNominee_VaccineFile1 != null)
                   {
                       sqlText += " EmployeeNominee_VaccineFile1=@EmployeeNominee_VaccineFile1,";
                   }
                   if (vm.EmployeeNominee_VaccineFile2 != null)
                   {
                       sqlText += " EmployeeNominee_VaccineFile2=@EmployeeNominee_VaccineFile2,";
                   }
                   if (vm.EmployeeNominee_VaccineFile3 != null)
                   {
                       sqlText += " EmployeeNominee_VaccineFile3=@EmployeeNominee_VaccineFile3,";
                   }
                   if (vm.Employeedependent_VaccineFile1 != null)
                   {
                       sqlText += " Employeedependent_VaccineFile1=@Employeedependent_VaccineFile1,";
                   }
                   if (vm.Employeedependent_VaccineFile2 != null)
                   {
                       sqlText += " Employeedependent_VaccineFile2=@Employeedependent_VaccineFile2,";
                   }
                   if (vm.Employeedependent_VaccineFile3 != null)
                   {
                       sqlText += " Employeedependent_VaccineFile3=@Employeedependent_VaccineFile3,";
                   }
                   if (vm.edu_Certificate != null)
                   {
                       sqlText += " edu_Certificate=@edu_Certificate,";
                   }
                   if (vm.Extra_FileName != null)
                   {
                       sqlText += " Extra_FileName=@Extra_FileName,";
                   }
                   if (vm.PassportVisa != null)
                   {
                       sqlText += " PassportVisa=@PassportVisa,";
                   }
                   if (vm.Certificate != null)
                   {
                       sqlText += " Certificate=@Certificate,";
                   }
                   if (vm.BillVoucher != null)
                   {
                       sqlText += " BillVoucher=@BillVoucher,";
                   }
                   if (vm.AssetFileName != null)
                   {
                       sqlText += " AssetFileName=@AssetFileName,";
                   }
                   sqlText += " Remarks=@Remarks,";
                   sqlText += " IsActive=@IsActive";
                   sqlText += " where Id=@Id";

                   SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                   cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                   cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                   
                   
                   if (vm.EmployeePersonalDetail_Fingerprint != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@EmployeePersonalDetail_Fingerprint", vm.EmployeePersonalDetail_Fingerprint ?? Convert.DBNull);
                   }
                   if (vm.SignatureFiles != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@SignatureFiles", vm.SignatureFiles ?? Convert.DBNull);
                   }
                   if (vm.EmployeePersonalDetail_PassportFile != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@EmployeePersonalDetail_PassportFile", vm.EmployeePersonalDetail_PassportFile ?? Convert.DBNull);
                   }
                   if (vm.EmployeePersonalDetail_NIDFile != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@EmployeePersonalDetail_NIDFile", vm.EmployeePersonalDetail_NIDFile ?? Convert.DBNull);
                   }
                   if (vm.EmployeePersonalDetail_VaccineFile1 != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@EmployeePersonalDetail_VaccineFile1", vm.EmployeePersonalDetail_VaccineFile1 ?? Convert.DBNull);
                   }
                   if (vm.EmployeePersonalDetail_VaccineFiles2 != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@EmployeePersonalDetail_VaccineFiles2", vm.EmployeePersonalDetail_VaccineFiles2 ?? Convert.DBNull);
                   }
                   if (vm.EmployeePersonalDetail_VaccineFile3 != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@EmployeePersonalDetail_VaccineFile3", vm.EmployeePersonalDetail_VaccineFile3 ?? Convert.DBNull);
                   }
                   if (vm.EmployeePersonalDetail_DisabilityFile != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@EmployeePersonalDetail_DisabilityFile", vm.EmployeePersonalDetail_DisabilityFile ?? Convert.DBNull);
                   }
                   if (vm.FileName != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                   }
                   if (vm.EmployeeNominee_VaccineFile1 != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@EmployeeNominee_VaccineFile1", vm.EmployeeNominee_VaccineFile1 ?? Convert.DBNull);
                   }
                   if (vm.EmployeeNominee_VaccineFile2 != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@EmployeeNominee_VaccineFile2", vm.EmployeeNominee_VaccineFile2 ?? Convert.DBNull);
                   }
                   if (vm.EmployeeNominee_VaccineFile3 != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@EmployeeNominee_VaccineFile3", vm.EmployeeNominee_VaccineFile3 ?? Convert.DBNull);
                   }
                   if (vm.Employeedependent_VaccineFile1 != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@Employeedependent_VaccineFile1", vm.Employeedependent_VaccineFile1 ?? Convert.DBNull);
                   }
                   if (vm.Employeedependent_VaccineFile2 != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@Employeedependent_VaccineFile2", vm.Employeedependent_VaccineFile2 ?? Convert.DBNull);
                   }
                   if (vm.Employeedependent_VaccineFile3 != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@Employeedependent_VaccineFile3", vm.Employeedependent_VaccineFile3 ?? Convert.DBNull);
                   }
                   if (vm.edu_Certificate != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@edu_Certificate", vm.edu_Certificate ?? Convert.DBNull);
                   }
                   if (vm.Extra_FileName != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@Extra_FileName", vm.Extra_FileName ?? Convert.DBNull);
                   }
                   if (vm.PassportVisa != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@PassportVisa", vm.PassportVisa ?? Convert.DBNull);
                   }
                   if (vm.Certificate != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@Certificate", vm.Certificate ?? Convert.DBNull);
                   }
                   if (vm.AssetFileName != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@AssetFileName", vm.AssetFileName ?? Convert.DBNull);
                   }
                   if (vm.BillVoucher != null)
                   {
                       cmdUpdate.Parameters.AddWithValue("@BillVoucher", vm.BillVoucher ?? Convert.DBNull);
                   }
                   cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                   cmdUpdate.Parameters.AddWithValue("@IsActive", true);
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
                   throw new ArgumentNullException("EmployeeFile Update", "Could not found any item.");
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
                   retResults[1] = "Unexpected error to update EmployeeFile.";
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

       //==================Delete =================
       public string[] Delete(EmployeeFilesVM EmployeeFilesVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
       {
           #region Variables

           string[] retResults = new string[6];
           retResults[0] = "Fail";//Success or Fail
           retResults[1] = "Fail";// Success or Fail Message
           retResults[2] = "0";// Return Id
           retResults[3] = "sqlText"; //  SQL Query
           retResults[4] = "ex"; //catch ex
           retResults[5] = "DeleteEmployeeFile"; //Method Name

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

               if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeFile"); }

               #endregion open connection and transaction
               #region Check is  it used

               #endregion Check is  it used
               if (EmployeeFilesVM.Id > 0)
               {
                   #region Update Settings

                   sqlText = "";
                   sqlText = "update EmployeeFile set";
                   sqlText += " IsArchive=@IsArchive,";
                   sqlText += " LastUpdateBy=@LastUpdateBy,";
                   sqlText += " LastUpdateAt=@LastUpdateAt,";
                   sqlText += " LastUpdateFrom=@LastUpdateFrom";
                   sqlText += " where Id=@Id";

                   SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                   cmdUpdate.Parameters.AddWithValue("@Id", EmployeeFilesVM.Id);
                   cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                   cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeeFilesVM.LastUpdateBy);
                   cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeeFilesVM.LastUpdateAt);
                   cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeeFilesVM.LastUpdateFrom);

                   cmdUpdate.Transaction = transaction;
                   var exeRes = cmdUpdate.ExecuteNonQuery();
                   transResult = Convert.ToInt32(exeRes);

                   retResults[2] = EmployeeFilesVM.Id.ToString();// Return Id
                   retResults[3] = sqlText; //  SQL Query

                   #region Commit

                   if (transResult <= 0)
                   {
                       throw new ArgumentNullException("EmployeeFile Delete", EmployeeFilesVM.Id + " could not Delete.");
                   }

                   #endregion Commit

                   #endregion Update Settings
                   iSTransSuccess = true;
               }
               else
               {
                   throw new ArgumentNullException("EmployeeFile Information Delete", "Could not found any item.");
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
                   retResults[1] = "Unexpected error to delete EmployeeFile Information.";
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
   }
}
