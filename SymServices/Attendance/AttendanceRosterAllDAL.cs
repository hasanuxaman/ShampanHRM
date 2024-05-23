using SymOrdinary;
using SymServices.Common;
using SymViewModel.Attendance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Attendance
{
    public class AttendanceRosterAllDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods

        //==================SelectAll=================
        public List<AttendanceRosterAllVM> SelectAll(int BranchId)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<AttendanceRosterAllVM> vMs = new List<AttendanceRosterAllVM>();
            AttendanceRosterAllVM vM;
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
                arl.Id
                ,arl.BranchId
                ,arl.Code
                ,arl.Name
                ,arl.AttendanceStructureId
                ,ag.name AttendanceGroup
                ,ats.name AttendanceStructure
                ,arl.AttendanceGroupId
                ,arl.Remarks
                ,arl.IsActive
                ,arl.IsArchive
 
                From AttendanceRosterAll arl 
                left outer join AttendanceStructure ats on arl.AttendanceStructureId=ats.Id
                left outer join [Group] ag on arl.AttendanceGroupId=ag.Id
                Where arl.IsArchive=0 and arl.BranchId=@BranchId
                ORDER BY arl.Id
                ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.Parameters.AddWithValue("@BranchId", BranchId);
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vM = new AttendanceRosterAllVM();
                    vM.Id = dr["Id"].ToString();
                    vM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vM.Code = dr["Code"].ToString();
                    vM.Name = dr["Name"].ToString();
                    vM.AttendanceGroup = dr["AttendanceGroup"].ToString();
                    vM.AttendanceStructure = dr["AttendanceStructure"].ToString();
                    vM.Remarks = dr["Remarks"].ToString();
                    vM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vMs.Add(vM);
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

            return vMs;
        }

        //==================SelectAll=================
        public List<AttendanceRosterAllVM> SelectAllByParameters(string attendanceDate = "", string attendanceGroupId = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<AttendanceRosterAllVM> VMs = new List<AttendanceRosterAllVM>();
            AttendanceRosterAllVM vm;
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
                #region sql statement
                sqlText = @"
SELECT
arl.Id
,arl.Name
,ag.name AttendanceGroup
,ats.name AttendanceStructure
,arl.AttendanceGroupId
,arl.AttendanceStructureId
,arl.ToDate
,arl.Remarks
From AttendanceRosterDetail arl 
LEFT OUTER JOIN AttendanceStructure ats ON arl.AttendanceStructureId=ats.Id
LEFT OUTER JOIN [Group] ag ON arl.AttendanceGroupId=ag.Id
WHERE 1=1
";
                if (!string.IsNullOrWhiteSpace(Ordinary.DateToString(attendanceDate)))
                {
                    sqlText += " AND arl.ToDate = @attendanceDate";
                }
                if (!string.IsNullOrWhiteSpace(attendanceGroupId))
                {
                    sqlText += " AND arl.AttendanceGroupId = @attendanceGroupId";
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (!string.IsNullOrWhiteSpace(attendanceDate))
                {
                    objComm.Parameters.AddWithValue("@attendanceDate", Ordinary.DateToString(attendanceDate));
                }
                if (!string.IsNullOrWhiteSpace(attendanceGroupId))
                {
                    objComm.Parameters.AddWithValue("@attendanceGroupId", attendanceGroupId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AttendanceRosterAllVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.AttendanceGroup = dr["AttendanceGroup"].ToString();
                    vm.AttendanceGroupId = dr["AttendanceGroupId"].ToString();
                    vm.AttendanceStructure = dr["AttendanceStructure"].ToString();
                    vm.AttendanceStructureId = dr["AttendanceStructureId"].ToString();
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #region commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion commit
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
            return VMs;
        }

        //==================SelectAll=================
        public List<AttendanceRosterAllVM> SelectDetailAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AttendanceRosterAllVM> vMs = new List<AttendanceRosterAllVM>();
            AttendanceRosterAllVM vM;
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
arl.Name
,ag.name AttendanceGroup
,ats.name AttendanceStructure
,arl.AttendanceGroupId
,arl.AttendanceStructureId
,min( arl.ToDate) FromDate,max( arl.ToDate) ToDate,arl.Remarks
From AttendanceRosterDetail arl 
left outer join AttendanceStructure ats on arl.AttendanceStructureId=ats.Id
left outer join [Group] ag on arl.AttendanceGroupId=ag.Id
group by 
arl.Name
,ag.name 
,ats.name
,arl.AttendanceGroupId
,arl.AttendanceStructureId
,arl.Remarks

order by min(arl.ToDate) desc, arl.AttendanceGroupId, arl.AttendanceStructureId 
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vM = new AttendanceRosterAllVM();
                    vM.Name = dr["Name"].ToString();
                    //vM.Date = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vM.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vM.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vM.AttendanceGroup = dr["AttendanceGroup"].ToString();
                    vM.AttendanceGroupId = dr["AttendanceGroupId"].ToString();
                    vM.AttendanceStructure = dr["AttendanceStructure"].ToString();
                    vM.AttendanceStructureId = dr["AttendanceStructureId"].ToString();
                    vM.Remarks = dr["Remarks"].ToString();
                    vMs.Add(vM);
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

            return vMs;
        }

        //==================SelectByID=================
        public AttendanceRosterAllVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            AttendanceRosterAllVM vM = new AttendanceRosterAllVM();

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
                arl.Id
                ,arl.BranchId
                ,arl.Code
                ,arl.Name
                ,arl.AttendanceStructureId
                ,arl.AttendanceGroupId
                ,arl.Remarks
                ,arl.IsActive
                ,arl.IsArchive
 
                From AttendanceRosterAll arl 
                Where arl.IsArchive=0 and arl.Id=@Id
                ORDER BY arl.Id
                ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.Parameters.AddWithValue("@Id", Id);
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vM = new AttendanceRosterAllVM();
                    vM.Id = dr["Id"].ToString();
                    vM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vM.Code = dr["Code"].ToString();
                    vM.Name = dr["Name"].ToString();
                    vM.AttendanceStructureId = dr["AttendanceStructureId"].ToString();
                    vM.AttendanceGroupId = dr["AttendanceGroupId"].ToString();
                    vM.Remarks = dr["Remarks"].ToString();
                    vM.IsActive = Convert.ToBoolean(dr["IsActive"]);
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
            return vM;
        }

        //==================SelectByID=================
        public AttendanceRosterAllVM SelectByDetailId(string AttendanceStructureId, string AttendanceGroupId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            AttendanceRosterAllVM vM = new AttendanceRosterAllVM();
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
arl.Name
,ag.name AttendanceGroup
,ats.name AttendanceStructure
,arl.AttendanceGroupId
,arl.AttendanceStructureId
,min( arl.ToDate) FromDate,max( arl.ToDate) ToDate
From AttendanceRosterDetail arl 
left outer join AttendanceStructure ats on arl.AttendanceStructureId=ats.Id
left outer join [Group] ag on arl.AttendanceGroupId=ag.Id
group by 
arl.Name
,ag.name 
,ats.name
,arl.AttendanceGroupId
,arl.AttendanceStructureId
having arl.AttendanceStructureId=@AttendanceStructureId and  arl.AttendanceGroupId=@AttendanceGroupId";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.Parameters.AddWithValue("@AttendanceStructureId", AttendanceStructureId);
                objComm.Parameters.AddWithValue("@AttendanceGroupId", AttendanceGroupId);
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vM = new AttendanceRosterAllVM();
                    vM.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vM.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vM.AttendanceGroup = dr["AttendanceGroup"].ToString();
                    vM.AttendanceStructure = dr["AttendanceStructure"].ToString();
                    vM.AttendanceStructureId = dr["AttendanceStructureId"].ToString();
                    vM.AttendanceGroupId = dr["AttendanceGroupId"].ToString();
                    vM.Remarks = dr["Remarks"].ToString();
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
            return vM;
        }

        //==================Insert =================
        public string[] Insert(AttendanceRosterAllVM vM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertBank"; //Method Name


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
                #region Exist
                sqlText = @"Select IsNull(isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0),0) from AttendanceRosterAll where BranchId=@BranchId and AttendanceGroupId=@AttendanceGroupId
                    ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@BranchId", vM.BranchId);
                cmdExist.Parameters.AddWithValue("@AttendanceGroupId", vM.AttendanceGroupId);
                //cmdExist.Parameters.AddWithValue("@Id", vM.Id);
                cmdExist.Transaction = transaction;
                int countExist = (int)cmdExist.ExecuteScalar();
                if (countExist > 0)
                {
                    retResults[1] = "This Group Already is exist!";
                    throw new ArgumentNullException("This Group Already is exist!", "");
                }
                #endregion Exist
                #region Save
                sqlText = "Select IsNull(isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0),0) from AttendanceRosterAll where BranchId=@BranchId";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@BranchId", vM.BranchId);
                cmd2.Transaction = transaction;
                int count = (int)cmd2.ExecuteScalar();
                vM.Id = vM.BranchId.ToString() + "_" + (count + 1);
                //int foundId = (int)objfoundId;
                if (1 == 1)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO AttendanceRosterAll(
                        Id,BranchId,Code,Name,AttendanceStructureId,AttendanceGroupId,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                        ) 
                        VALUES (@Id,@BranchId,@Code,@Name,@AttendanceStructureId,@AttendanceGroupId,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                        ";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@Id", vM.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vM.BranchId);
                    cmdInsert.Parameters.AddWithValue("@Code", vM.Code);
                    cmdInsert.Parameters.AddWithValue("@Name", vM.Name);
                    cmdInsert.Parameters.AddWithValue("@AttendanceStructureId", vM.AttendanceStructureId);
                    cmdInsert.Parameters.AddWithValue("@AttendanceGroupId", vM.AttendanceGroupId);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vM.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vM.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vM.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vM.CreatedFrom);

                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();

                }
                else
                {
                    retResults[1] = "This Bank already used!";
                    throw new ArgumentNullException("Please Input Bank Value", "");
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
                retResults[2] = vM.Id;

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

        //==================Insert =================
        public string[] InsertDetail(AttendanceRosterAllVM vM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertBank"; //Method Name
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

                AttendanceStructureDAL _asDal = new AttendanceStructureDAL();
                GroupDAL _grpDal = new GroupDAL();
                vM.AttendanceStructure = _asDal.SelectById(vM.AttendanceStructureId).Name;
                vM.AttendanceGroup = _grpDal.SelectById(vM.AttendanceGroupId).Name;
                var name = Convert.ToDateTime(vM.FromDate).ToString("dd-MMM-yy") + " - " + Convert.ToDateTime(vM.ToDate).ToString("dd-MMM-yy") + "(" + vM.AttendanceStructure + " _ " + vM.AttendanceGroup + ")";


                #region Delete
                sqlText = @"
delete from AttendanceRosterDetail 
where AttendanceGroupId=@AttendanceGroupId 
------and AttendanceStructureId=@AttendanceStructureId
and ToDate between @DateFrom and @DateTo
";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);

                cmdExist.Parameters.AddWithValue("@AttendanceGroupId", vM.AttendanceGroupId);
                ////////cmdExist.Parameters.AddWithValue("@AttendanceStructureId", vM.AttendanceStructureId);
                cmdExist.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vM.FromDate));
                cmdExist.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vM.ToDate));
                //cmdExist.Parameters.AddWithValue("@Id", vM.Id);
                cmdExist.Transaction = transaction;
                int countExist = (int)cmdExist.ExecuteNonQuery();
                if (countExist > 0)
                {
                    retResults[1] = "This Attendance Roster Already is exist for this Atendance Group! "+vM.AttendanceGroup;
                    throw new ArgumentNullException(retResults[1], "");
                }
                #endregion Delete


                int datelength = Convert.ToInt32((Convert.ToDateTime(vM.ToDate) - Convert.ToDateTime(vM.FromDate)).TotalDays);
                sqlText = "  ";
                sqlText += @" INSERT INTO AttendanceRosterDetail(
                        Name,AttendanceStructureId,AttendanceGroupId,ToDate, Remarks) 
                        VALUES (@Name,@AttendanceStructureId,@AttendanceGroupId,@Date,@Remarks) ";

                for (int i = 0; i <= datelength; i++)
                {
                    #region Save

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    var date = Convert.ToDateTime(vM.FromDate).AddDays(i).ToString();
                    cmdInsert.Parameters.AddWithValue("@Date", Ordinary.DateToString(date));
                    cmdInsert.Parameters.AddWithValue("@AttendanceStructureId", vM.AttendanceStructureId);
                    cmdInsert.Parameters.AddWithValue("@Name", name);
                    cmdInsert.Parameters.AddWithValue("@AttendanceGroupId", vM.AttendanceGroupId);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vM.Remarks ?? Convert.DBNull);//, bankVM.Remarks);

                    var result = cmdInsert.ExecuteNonQuery();
                    if (Convert.ToInt32(result) <= 0)
                    {
                        retResults[1] = "Error!";
                        throw new ArgumentNullException("Error", "");
                    }
                    #endregion Save
                }
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
                retResults[2] = vM.Id;
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
        public string[] Update(AttendanceRosterAllVM vM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Bank Update"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToBank"); }

                #endregion open connection and transaction
                if (vM != null)
                {
                    #region Exist
                    sqlText = @"Select IsNull(isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0),0) from AttendanceRosterAll where BranchId=@BranchId and AttendanceGroupId=@AttendanceGroupId
                    and  Id <> @Id";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Parameters.AddWithValue("@BranchId", vM.BranchId);
                    cmdExist.Parameters.AddWithValue("@AttendanceGroupId", vM.AttendanceGroupId);
                    cmdExist.Parameters.AddWithValue("@Id", vM.Id);
                    cmdExist.Transaction = transaction;
                    var exeRes = cmdExist.ExecuteScalar();
                    int countExist = Convert.ToInt32(exeRes);
                    if (countExist > 0)
                    {
                        retResults[1] = "This Group Already is exist!";
                        throw new ArgumentNullException("This Group Already is exist!", "");
                    }
                    #endregion Exist

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update AttendanceRosterAll set";
                    sqlText += " BranchId=@BranchId";
                    sqlText += " ,Code=@Code";
                    sqlText += " ,Name=@Name";
                    sqlText += " ,AttendanceStructureId=@AttendanceStructureId";
                    sqlText += " ,AttendanceGroupId=@AttendanceGroupId";
                    sqlText += " ,Remarks=@Remarks";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vM.Id);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", vM.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@Code", vM.Code);
                    cmdUpdate.Parameters.AddWithValue("@Name", vM.Name);
                    cmdUpdate.Parameters.AddWithValue("@AttendanceStructureId", vM.AttendanceStructureId);
                    cmdUpdate.Parameters.AddWithValue("@AttendanceGroupId", vM.AttendanceGroupId);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vM.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    transResult = (int)cmdUpdate.ExecuteNonQuery();

                    retResults[2] = vM.Id.ToString();// Return Id
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
                    throw new ArgumentNullException("Bank Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Bank.";
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
        public AttendanceRosterAllVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            AttendanceRosterAllVM vM = new AttendanceRosterAllVM();

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
,BranchId
,Code
,Name
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From Bank 
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
                        vM.Id = dr["Id"].ToString();
                        vM.BranchId = Convert.ToInt32(dr["BranchId"]);
                        vM.Code = dr["Code"].ToString();
                        vM.Name = dr["Name"].ToString();
                        vM.Remarks = dr["Remarks"].ToString();
                        vM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        vM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                        vM.CreatedBy = dr["CreatedBy"].ToString();
                        vM.CreatedFrom = dr["CreatedFrom"].ToString();
                        vM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        vM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        vM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
            return vM;
        }

        //==================Delete =================
        public string[] Delete(string Name, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteBank"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToBank"); }

                #endregion open connection and transaction

                #region Update Settings

                #region Delete
                sqlText = @"delete from AttendanceRosterDetail where Name=@Name ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@Name", Name);
                //cmdExist.Parameters.AddWithValue("@Id", vM.Id);
                cmdExist.Transaction = transaction;
                int countExist = (int)cmdExist.ExecuteNonQuery();

                #endregion Delete


                retResults[2] = "";// Return Id
                retResults[3] = sqlText; //  SQL Query
                #region Commit

                #endregion Commit
                #endregion Update Settings
                iSTransSuccess = true;


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
