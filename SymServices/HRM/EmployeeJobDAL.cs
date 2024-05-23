using SymOrdinary;
using SymServices.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymServices.HRM
{
    public class EmployeeJobDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeJobVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeJobVM> VMs = new List<EmployeeJobVM>();
            EmployeeJobVM vm;// = new EmployeeJobVM();
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
 j.Id
,j.EmployeeId
,j.JoinDate
,ISNULL(j.FromDate, '') FromDate
,ISNULL(j.RetirementDate, '') RetirementDate
,ISNULL(j.fristExDate, '') fristExDate
,ISNULL(j.secondExDate, '') secondExDate
,ISNULL(j.ContrExDate, '') ContrExDate
,ISNULL(j.ToDate, '') ToDate
,ISNULL(j.ProbationMonth, 0) ProbationMonth
,ISNULL(j.ExtendedProbationMonth, 0) ExtendedProbationMonth
,j.ProbationEnd
,j.DateOfPermanent
,j.EmploymentStatus_E
,j.EmploymentType_E
,j.Supervisor
,j.BankInfo
,j.BankAccountNo
,j.IsPermanent
,ISNULL(j.IsBuild, 0) IsBuild 
,ISNULL(j.IsTAXApplicable, 0) IsTAXApplicable
,ISNULL(j.IsPFApplicable, 0) IsPFApplicable

,ISNULL(j.IsGFApplicable, 0) IsGFApplicable
,ISNULL(j.IsInactive, 0) IsInactive
,ISNULL(j.Rank, 0) Rank
,ISNULL(j.Duration, 0) Duration
,ISNULL(j.DotedLineReport, '') DotedLineReport
,j.Extentionyn
,j.Retirement
,j.Force
,j.GrossSalary
,j.BasicSalary
,j.Remarks
,j.IsActive
,j.IsArchive
,j.CreatedBy
,j.CreatedAt
,j.CreatedFrom
,j.LastUpdateBy
,j.LastUpdateAt
,j.LastUpdateFrom
,t.ProjectId
,t.DepartmentId
,t.SectionId
,p.DesignationId,p.GradeId
,isnull(g.IsGross,0)IsGross
,gd.Name GradeName
,j.Other1
,j.Other2
,j.Other3
,j.Other4
,j.Other5
,ISNULL(j.IsJobBefore, '') IsJobBefore
,j.AccountType
,b.Name BankName
,j.FirstHoliday
,j.SecondHoliday
,j.BankAccountName
,j.Routing_No
,j.EmpCategory
,j.EmpJobType
    From EmployeeJob j
	left outer join EmployeeTransfer t on j.EmployeeId=t.EmployeeId and t.IsCurrent=1
	left outer join EmployeePromotion p on j.EmployeeId=p.EmployeeId and p.IsCurrent=1
	left outer join dbo.EmployeeStructureGroup g on g.EmployeeId=j.EmployeeId and p.IsCurrent=1
	left outer join dbo.Grade gd on gd.Id=g.GradeId and p.IsCurrent=1
		left outer join dbo.Bank b on b.Id=j.BankInfo
Where j.IsArchive=0 
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                //objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                //SqlDataReader dr;
                //dr = objComm.ExecuteReader();

                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(objComm);
                ad.Fill(dt);                                                                                                                        
                foreach (DataRow dr in dt.Rows)
                {
                    vm = new EmployeeJobVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();

                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.ProbationMonth = Convert.ToInt32(dr["ProbationMonth"]);
                    vm.ExtendedProbationMonth = Convert.ToInt32(dr["ExtendedProbationMonth"]);
                    vm.ProbationEnd = Ordinary.StringToDate(dr["ProbationEnd"].ToString());
                    vm.DateOfPermanent = Ordinary.StringToDate(dr["DateOfPermanent"].ToString());
                    vm.EmploymentStatus_E = dr["EmploymentStatus_E"].ToString();
                    vm.EmploymentType_E = dr["EmploymentType_E"].ToString();
                    vm.Supervisor = dr["Supervisor"].ToString();
                    vm.BankInfo = dr["BankInfo"].ToString();
                    vm.BankAccountNo = dr["BankAccountNo"].ToString();
                    vm.IsPermanent = Convert.ToBoolean(dr["IsPermanent"]);
                    vm.IsBuild = Convert.ToBoolean(dr["IsBuild"]);
                    vm.IsPFApplicable = Convert.ToBoolean(dr["IsPFApplicable"]);
                    vm.IsTAXApplicable = Convert.ToBoolean(dr["IsTAXApplicable"]);
                    vm.IsGFApplicable = Convert.ToBoolean(dr["IsGFApplicable"]);
                    vm.IsInactive = Convert.ToBoolean(dr["IsInactive"]);

                    vm.Rank = dr["Rank"].ToString();
                    vm.Duration = dr["Duration"].ToString();
                    vm.RetirementDate = Ordinary.StringToDate(dr["RetirementDate"].ToString());
                    vm.fristExDate = Ordinary.StringToDate(dr["fristExDate"].ToString());
                    vm.secondExDate = Ordinary.StringToDate(dr["secondExDate"].ToString());
                    vm.ContrExDate = Ordinary.StringToDate(dr["ContrExDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.IsGross = Convert.ToBoolean(dr["IsGross"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();

                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.GradeId = dr["GradeId"].ToString();
                    vm.GradeName = dr["GradeName"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.Other1 = dr["Other1"].ToString();
                    vm.Other2 = dr["Other2"].ToString();
                    vm.Other3 = dr["Other3"].ToString();
                    vm.Other4 = dr["Other4"].ToString();
                    vm.Other5 = dr["Other5"].ToString();

                    vm.IsJobBefore = Convert.ToBoolean(dr["IsJobBefore"]);
                    vm.AccountType = dr["AccountType"].ToString();
                    vm.BankName = dr["BankName"].ToString();
                    vm.FirstHoliday = dr["FirstHoliday"].ToString();
                    vm.SecondHoliday = dr["SecondHoliday"].ToString();
                    vm.BankAccountName = dr["BankAccountName"].ToString();
                    vm.Routing_No = dr["Routing_No"].ToString();
                    vm.EmpCategory = dr["EmpCategory"].ToString();
                    vm.EmpJobType = dr["EmpJobType"].ToString();
                    vm.Retirement = dr["Retirement"].ToString();
                    vm.DotedLineReport = dr["DotedLineReport"].ToString();
                    vm.Force = dr["Force"].ToString();
                    vm.Extentionyn = dr["Extentionyn"].ToString();
                    vm.RetirementDate = Ordinary.StringToDate(dr["RetirementDate"].ToString());
                    //IsJobBefore
                    //AccountType

                    VMs.Add(vm);
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

            return VMs;
        }

        //==================SelectByID=================        
        public EmployeeJobVM SelectByEmployee(string EmployeeId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeJobVM vm = new EmployeeJobVM();
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
 j.Id
,j.EmployeeId

,j.JoinDate
,ISNULL(j.FromDate, '')FromDate
,ISNULL(j.RetirementDate, '') RetirementDate  
,ISNULL(j.fristExDate, '') fristExDate
,ISNULL(j.secondExDate, '') secondExDate
,ISNULL(j.ContrExDate, '') ContrExDate
,ISNULL(j.ToDate, '')ToDate
,ISNULL(j.ProbationMonth, 0) ProbationMonth
,ISNULL(j.ExtendedProbationMonth, 0) ExtendedProbationMonth
,j.ProbationEnd
,j.DateOfPermanent
,j.EmploymentStatus_E
,j.EmploymentType_E
,j.Supervisor
,j.BankInfo
,j.BankAccountNo
,j.IsPermanent
,ISNULL(j.IsBuild, 0) IsBuild 
,ISNULL(j.IsTAXApplicable, 0) IsTAXApplicable
,ISNULL(j.IsPFApplicable, 0) IsPFApplicable
,ISNULL(j.IsPFApplicable, 0) IsPFApplicable
,ISNULL(j.IsGFApplicable, 0) IsGFApplicable
,ISNULL(j.IsInactive, 0) IsInactive
,ISNULL(j.Rank, 0) Rank
,ISNULL(j.Duration, 0) Duration 
,ISNULL(j.DotedLineReport, '') DotedLineReport
,j.Extentionyn
,j.Retirement
,j.Force
,j.GrossSalary
,j.BasicSalary
,j.Remarks
,j.IsActive
,j.IsArchive
,j.CreatedBy
,j.CreatedAt
,j.CreatedFrom
,j.LastUpdateBy
,j.LastUpdateAt
,j.LastUpdateFrom
,t.ProjectId
,t.DepartmentId
,t.SectionId
,p.DesignationId,p.GradeId
,isnull(g.IsGross,0)IsGross
,gd.Name GradeName
,j.Other1
,j.Other2
,j.Other3
,j.Other4
,j.Other5
,ISNULL(j.IsJobBefore, 0) IsJobBefore
,j.AccountType
,b.Name BankName
,j.FirstHoliday
,j.SecondHoliday
,j.BankAccountName
,j.Routing_No
,j.EmpCategory
,j.EmpJobType
    ,isnull(j.GFStartFrom,j.JoinDate)GFStartFrom

    From EmployeeJob j

	left outer join EmployeeTransfer t on j.EmployeeId=t.EmployeeId and t.IsCurrent=1
	left outer join EmployeePromotion p on j.EmployeeId=p.EmployeeId and p.IsCurrent=1
	left outer join dbo.EmployeeStructureGroup g on g.EmployeeId=j.EmployeeId and p.IsCurrent=1
	left outer join dbo.Grade gd on gd.Id=g.GradeId and p.IsCurrent=1
			left outer join dbo.Bank b on b.Id=j.BankInfo
Where j.IsArchive=0 AND j.EmployeeId=@EmployeeId

";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.Transaction = transaction;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                //SqlDataReader dr;
                //dr = objComm.ExecuteReader();

                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(objComm);
                ad.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.ProbationMonth = Convert.ToInt32(dr["ProbationMonth"]);
                    vm.ExtendedProbationMonth = Convert.ToInt32(dr["ExtendedProbationMonth"]);
                    vm.ProbationEnd = Ordinary.StringToDate(dr["ProbationEnd"].ToString());
                    vm.DateOfPermanent = Ordinary.StringToDate(dr["DateOfPermanent"].ToString());
                    vm.GFStartFrom = Ordinary.StringToDate(dr["GFStartFrom"].ToString());
                    vm.EmploymentStatus_E = dr["EmploymentStatus_E"].ToString();
                    vm.EmploymentType_E = dr["EmploymentType_E"].ToString();
                    vm.Supervisor = dr["Supervisor"].ToString();
                    vm.BankInfo = dr["BankInfo"].ToString();
                    vm.BankAccountNo = dr["BankAccountNo"].ToString();
                    vm.IsPermanent = Convert.ToBoolean(dr["IsPermanent"]);
                    vm.IsBuild = Convert.ToBoolean(dr["IsBuild"]);
                    vm.IsTAXApplicable = Convert.ToBoolean(dr["IsTAXApplicable"]);
                    vm.IsPFApplicable = Convert.ToBoolean(dr["IsPFApplicable"]);
                    vm.IsGFApplicable = Convert.ToBoolean(dr["IsGFApplicable"]);
                    vm.IsInactive = Convert.ToBoolean(dr["IsInactive"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.IsGross = Convert.ToBoolean(dr["IsGross"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.Rank = dr["Rank"].ToString();
                    vm.Duration = dr["Duration"].ToString();
                    vm.DotedLineReport = dr["DotedLineReport"].ToString();
                    vm.Retirement = dr["Retirement"].ToString();
                    vm.Force = dr["Force"].ToString();
                    vm.Extentionyn = dr["Extentionyn"].ToString();
                    vm.RetirementDate = Ordinary.StringToDate(dr["RetirementDate"].ToString());
                    vm.fristExDate = Ordinary.StringToDate(dr["fristExDate"].ToString());
                    vm.secondExDate = Ordinary.StringToDate(dr["secondExDate"].ToString());
                    vm.ContrExDate = Ordinary.StringToDate(dr["ContrExDate"].ToString());
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.GradeId = dr["GradeId"].ToString();
                    vm.GradeName = dr["GradeName"].ToString();
                    vm.EmpJobType = dr["EmpJobType"].ToString();
                    vm.EmpCategory = dr["EmpCategory"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());

                    vm.Other1 = dr["Other1"].ToString();
                    vm.Other2 = dr["Other2"].ToString();
                    vm.Other3 = dr["Other3"].ToString();
                    vm.Other4 = dr["Other4"].ToString();
                    vm.Other5 = dr["Other5"].ToString();
                    vm.IsJobBefore = Convert.ToBoolean(dr["IsJobBefore"]);
                    vm.AccountType = dr["AccountType"].ToString();
                    vm.BankName = dr["BankName"].ToString();
                    vm.FirstHoliday = dr["FirstHoliday"].ToString();
                    vm.SecondHoliday = dr["SecondHoliday"].ToString();
                    vm.BankAccountName = dr["BankAccountName"].ToString();
                    vm.Routing_No = dr["Routing_No"].ToString();
                  


                    //IsJobBefore
                    //AccountType
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
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

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

            return vm;
        }

        //==================Insert=================
        public string[] Insert(EmployeeJobVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeJob"; //Method Name

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


                #region Designation

                sqlText = " update EmployeePromotion set IsCurrent=0 where EmployeeId =@EmployeeId";
                SqlCommand cmdPromotionUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdPromotionUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                try
                {
                    var promotionUpdateExec = cmdPromotionUpdate.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new ArgumentNullException("Promotion is not complete", "");
                }
                //int foundId = (int)objfoundId;

                sqlText = "  ";
                sqlText += @" INSERT INTO EmployeePromotion(	EmployeeId,DesignationId,IsPromotion,PromotionDate
                                ,IsCurrent,FileName,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@EmployeeId,@DesignationId,@IsPromotion,@PromotionDate
                                ,@IsCurrent,@FileName,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) SELECT SCOPE_IDENTITY()";

                SqlCommand cmdPromotionInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdPromotionInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdPromotionInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                //cmdP1.Parameters.AddWithValue("@GradeId", employeeJobVM.GradeId);
                cmdPromotionInsert.Parameters.AddWithValue("@IsPromotion", false);
                cmdPromotionInsert.Parameters.AddWithValue("@PromotionDate", Ordinary.DateToString(vm.JoinDate));
                cmdPromotionInsert.Parameters.AddWithValue("@IsCurrent", true);
                cmdPromotionInsert.Parameters.AddWithValue("@FileName", "");
                cmdPromotionInsert.Parameters.AddWithValue("@Remarks", "");
                cmdPromotionInsert.Parameters.AddWithValue("@IsActive", true);
                cmdPromotionInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdPromotionInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdPromotionInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                cmdPromotionInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                var promotionInsertExec = cmdPromotionInsert.ExecuteNonQuery();

                #endregion
                #region Department
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                //int foundId = (int)objfoundId;
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from EmployeeTransfer where BranchId=@BranchId";
                SqlCommand cmdTransferId = new SqlCommand(sqlText, currConn);
                cmdTransferId.Parameters.AddWithValue("@BranchId", identity.BranchId);
                cmdTransferId.Transaction = transaction;
                var ccount = cmdTransferId.ExecuteScalar();
                int count = Convert.ToInt32(ccount);
                string transfer_Id = identity.BranchId.ToString() + "_" + (count + 1);

                sqlText = " update EmployeeTransfer set IsCurrent=0 where EmployeeId =@EmployeeId";
                SqlCommand cmdTransferUpdate = new SqlCommand(sqlText, currConn);
                cmdTransferUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdTransferUpdate.Transaction = transaction;
                try
                {
                    var transferUpdateExec = cmdTransferUpdate.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new ArgumentNullException("Transfer is not complete", "");
                }

                sqlText = "  ";
                sqlText += @" INSERT INTO EmployeeTransfer(Id,EmployeeId,BranchId,ProjectId,DepartmentId,SectionId,Other1,Other2,Other3,Other4,Other5,TransferDate,IsCurrent
                                        ,FileName,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Id,@EmployeeId,@BranchId,@ProjectId,@DepartmentId,@SectionId,@Other1,@Other2,@Other3,@Other4,@Other5,@TransferDate,@IsCurrent
                                        ,@FileName,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) ";

                SqlCommand cmdTransferInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdTransferInsert.Parameters.AddWithValue("@Id", transfer_Id);
                cmdTransferInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdTransferInsert.Parameters.AddWithValue("@BranchId", identity.BranchId);
                cmdTransferInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                cmdTransferInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                cmdTransferInsert.Parameters.AddWithValue("@TransferDate", Ordinary.DateToString(vm.JoinDate) ?? Convert.DBNull);
                cmdTransferInsert.Parameters.AddWithValue("@IsCurrent", true);
                cmdTransferInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                cmdTransferInsert.Parameters.AddWithValue("@Other1", vm.Other1 ?? Convert.DBNull);
                cmdTransferInsert.Parameters.AddWithValue("@Other2", vm.Other2 ?? Convert.DBNull);
                cmdTransferInsert.Parameters.AddWithValue("@Other3", vm.Other3 ?? Convert.DBNull);
                cmdTransferInsert.Parameters.AddWithValue("@Other4", vm.Other4 ?? Convert.DBNull);
                cmdTransferInsert.Parameters.AddWithValue("@Other5", vm.Other5 ?? Convert.DBNull);
                cmdTransferInsert.Parameters.AddWithValue("@Remarks", "");
                cmdTransferInsert.Parameters.AddWithValue("@FileName", "");
                cmdTransferInsert.Parameters.AddWithValue("@IsActive", true);
                cmdTransferInsert.Parameters.AddWithValue("@IsArchive", false);
                cmdTransferInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdTransferInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                cmdTransferInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                var eransferInsertExec = cmdTransferInsert.ExecuteNonQuery();
                #endregion

                #region Save

                //int foundId = (int)objfoundId;
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO EmployeeJob(
EmployeeId
,JoinDate
,FromDate
,RetirementDate
,fristExDate
,secondExDate
,ContrExDate
,ToDate
,ProbationMonth
,ExtendedProbationMonth
,ProbationEnd
,DateOfPermanent
,EmploymentStatus_E
,EmploymentType_E
,Supervisor
,BankInfo
,BankAccountNo
,IsPermanent
,IsBuild
,IsTAXApplicable
,IsPFApplicable
,IsGFApplicable
,IsInactive
,GrossSalary
,BasicSalary
,Other1
,Other2
,Other3
,Other4
,Other5
,IsJobBefore
,AccountType
,FirstHoliday
,SecondHoliday
,Force
,Rank
,Duration
,DotedLineReport
,Retirement
,Extentionyn
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,EmpCategory
,EmpJobType
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,BankAccountName
,Routing_No
,GFStartFrom
) VALUES (

@EmployeeId
,@JoinDate
,@FromDate
,@RetirementDate
,@fristExDate
,@secondExDate
,@ContrExDate
,@ToDate
,@ProbationMonth
,@ExtendedProbationMonth
,@ProbationEnd
,@DateOfPermanent
,@EmploymentStatus_E
,@EmploymentType_E
,@Supervisor
,@BankInfo
,@BankAccountNo
,@IsPermanent
,@IsBuild
,@IsTAXApplicable
,@IsPFApplicable 
,@IsGFApplicable
,@IsInactive
,@GrossSalary
,@BasicSalary
,@Other1
,@Other2
,@Other3
,@Other4
,@Other5
,@IsJobBefore
,@AccountType
,@FirstHoliday
,@SecondHoliday
,@Force
,@Rank
,@Duration
,@DotedLineReport
,@Retirement
,@Extentionyn
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@EmpCategory
,@EmpJobType
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@BankAccountName
,@Routing_No
,@GFStartFrom

) SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@JoinDate", Ordinary.DateToString(vm.JoinDate));
                    cmdInsert.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate));
                    cmdInsert.Parameters.AddWithValue("@RetirementDate", Ordinary.DateToString(vm.RetirementDate));
                    cmdInsert.Parameters.AddWithValue("@fristExDate", Ordinary.DateToString(vm.fristExDate));
                    cmdInsert.Parameters.AddWithValue("@secondExDate", Ordinary.DateToString(vm.secondExDate));
                    cmdInsert.Parameters.AddWithValue("@ContrExDate", Ordinary.DateToString(vm.ContrExDate));
                    cmdInsert.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate));
                    cmdInsert.Parameters.AddWithValue("@GFStartFrom", Ordinary.DateToString(vm.GFStartFrom));
                    cmdInsert.Parameters.AddWithValue("@ProbationMonth", vm.ProbationMonth);
                    
                    cmdInsert.Parameters.AddWithValue("@ExtendedProbationMonth", vm.ExtendedProbationMonth);
                    cmdInsert.Parameters.AddWithValue("@ProbationEnd", Ordinary.DateToString(vm.ProbationEnd) ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@DateOfPermanent", Ordinary.DateToString(vm.DateOfPermanent) ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@EmploymentStatus_E", vm.EmploymentStatus_E);
                    cmdInsert.Parameters.AddWithValue("@EmploymentType_E", vm.EmploymentType_E);
                    cmdInsert.Parameters.AddWithValue("@BankInfo", vm.BankInfo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@BankAccountNo", vm.BankAccountNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Supervisor", vm.Supervisor ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsPermanent", vm.IsPermanent);
                    cmdInsert.Parameters.AddWithValue("@IsBuild", vm.IsBuild);
                    cmdInsert.Parameters.AddWithValue("@IsTAXApplicable", vm.IsTAXApplicable);
                    cmdInsert.Parameters.AddWithValue("@IsPFApplicable", vm.IsPFApplicable);
                    cmdInsert.Parameters.AddWithValue("@IsGFApplicable", vm.IsGFApplicable);
                    cmdInsert.Parameters.AddWithValue("@IsInactive", vm.IsInactive);
                    
                    cmdInsert.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);
                    cmdInsert.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                    cmdInsert.Parameters.AddWithValue("@Other1", vm.Other1 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Other2", vm.Other2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Other3", vm.Other3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Other4", vm.Other4 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Other5", vm.Other5 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsJobBefore", vm.IsJobBefore);
                    cmdInsert.Parameters.AddWithValue("@AccountType", vm.AccountType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FirstHoliday", vm.FirstHoliday ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@SecondHoliday", vm.SecondHoliday ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@Force", vm.Force ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Retirement", vm.Retirement ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Extentionyn", vm.Extentionyn ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Duration", vm.Duration ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Rank", vm.Rank ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@DotedLineReport", vm.DotedLineReport ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@EmpCategory", vm.EmpCategory ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@EmpJobType", vm.EmpJobType ?? Convert.DBNull);
                    //cmdInsert.Parameters.AddWithValue("@Retirement", vm.Retirement ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@BankAccountName", vm.BankAccountName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Routing_No", vm.Routing_No ?? Convert.DBNull);
                    cmdInsert.Transaction = transaction;
                    var exeRes = cmdInsert.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee EmployeeJob Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee EmployeeJob Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee EmployeeJob already used";
                    throw new ArgumentNullException("Please Input Employee EmployeeJob Value", "");
                }


                #endregion Save

                #region EmployeeStructureGroup

                sqlText = "Select count(Id) from EmployeeStructureGroup where EmployeeId=@EmployeeId";
                SqlCommand cmdExists = new SqlCommand(sqlText, currConn);
                cmdExists.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExists.Transaction = transaction;
                var exeRes1 = cmdExists.ExecuteScalar();
                int Exists = Convert.ToInt32(exeRes1);
                if (Exists <= 0)
                {
                    #region EmployeeStructureGroup Insert
                    sqlText = "Select count(Id) from EmployeeStructureGroup ";
                    SqlCommand cmd3 = new SqlCommand(sqlText, currConn);
                    cmd3.Transaction = transaction;
                    var exeRes = cmd3.ExecuteScalar();
                    int count1 = Convert.ToInt32(exeRes);
                    string EmployeeStructureGroup_Id = (count1 + 1).ToString();


                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeStructureGroup( EmployeeId,IsGross,EmployeeGroupId,LeaveStructureId,SalaryStructureId,PFStructureId
                            ,TaxStructureId,BonusStructureId,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                            ) 
                            VALUES ( @EmployeeId,0,0,0,0,0,0,0,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                            ) ";

                    SqlCommand cmdESG = new SqlCommand(sqlText, currConn);

                    cmdESG.Parameters.AddWithValue("@Id", EmployeeStructureGroup_Id);

                    cmdESG.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdESG.Parameters.AddWithValue("@Remarks", "");
                    cmdESG.Parameters.AddWithValue("@IsActive", true);
                    cmdESG.Parameters.AddWithValue("@IsArchive", false);
                    cmdESG.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdESG.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdESG.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    cmdESG.Transaction = transaction;
                    cmdESG.ExecuteNonQuery();
                    #endregion EmployeeStructureGroup Insert
                }
                #endregion EmployeeStructureGroup





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
        public string[] Update(EmployeeJobVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeJob Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeJob"); }

                #endregion open connection and transaction
                #region Designation/Promotion
                sqlText = "";
                sqlText = "update EmployeePromotion set";
                sqlText += " DesignationId=@DesignationId,";
                //sqlText += " GradeId=@GradeId,";
                sqlText += " LastUpdateBy=@LastUpdateBy,";
                sqlText += " LastUpdateAt=@LastUpdateAt,";
                sqlText += " LastUpdateFrom=@LastUpdateFrom";
                sqlText += " where EmployeeId=@EmployeeId and IsCurrent=@IsCurrent";

                SqlCommand cmdDesg = new SqlCommand(sqlText, currConn);
                cmdDesg.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdDesg.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                //cmdDesg.Parameters.AddWithValue("@GradeId", employeeJobVM.GradeId);
                cmdDesg.Parameters.AddWithValue("@IsCurrent", true);
                cmdDesg.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                cmdDesg.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                cmdDesg.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                cmdDesg.Transaction = transaction;
                cmdDesg.ExecuteNonQuery();

                #endregion
                #region Department

                sqlText = "";
                sqlText = "update EmployeeTransfer set";
                sqlText += " ProjectId=@ProjectId,";
                sqlText += " DepartmentId=@DepartmentId,";
                sqlText += " SectionId=@SectionId,";
                sqlText += " Other1=@Other1,";
                sqlText += " Other2=@Other2,";
                sqlText += " Other3=@Other3,";
                sqlText += " Other4=@Other4,";
                sqlText += " Other5=@Other5,";
                sqlText += " LastUpdateBy=@LastUpdateBy,";
                sqlText += " LastUpdateAt=@LastUpdateAt,";
                sqlText += " LastUpdateFrom=@LastUpdateFrom";
                sqlText += " where EmployeeId=@EmployeeId and IsCurrent=@IsCurrent";

                SqlCommand cmdDep = new SqlCommand(sqlText, currConn);
                cmdDep.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdDep.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                cmdDep.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                cmdDep.Parameters.AddWithValue("@Other1", vm.Other1 ?? Convert.DBNull);
                cmdDep.Parameters.AddWithValue("@Other2", vm.Other2 ?? Convert.DBNull);
                cmdDep.Parameters.AddWithValue("@Other3", vm.Other3 ?? Convert.DBNull);
                cmdDep.Parameters.AddWithValue("@Other4", vm.Other4 ?? Convert.DBNull);
                cmdDep.Parameters.AddWithValue("@Other5", vm.Other5 ?? Convert.DBNull);
                cmdDep.Parameters.AddWithValue("@IsCurrent", true);
                cmdDep.Parameters.AddWithValue("@SectionId", vm.SectionId);
                cmdDep.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                cmdDep.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                cmdDep.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                cmdDep.Transaction = transaction;
                cmdDep.ExecuteNonQuery();

                #endregion
                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeJob set";

                    sqlText += " JoinDate=@JoinDate";
                    sqlText += ",FromDate=@FromDate";
                    sqlText += ",RetirementDate=@RetirementDate";
                    sqlText += ",fristExDate=@fristExDate";
                    sqlText += ",secondExDate=@secondExDate";
                    sqlText += ",ContrExDate=@ContrExDate";
                    sqlText += ",ToDate=@ToDate";
                    sqlText += ", ExtendedProbationMonth=@ExtendedProbationMonth";
                    sqlText += ", ProbationMonth=@ProbationMonth";
                    sqlText += ", ProbationEnd=@ProbationEnd";
                    sqlText += ", DateOfPermanent=@DateOfPermanent";
                    sqlText += ", EmploymentStatus_E=@EmploymentStatus_E";
                    sqlText += ", EmploymentType_E=@EmploymentType_E";
                    sqlText += ", Supervisor=@Supervisor";
                    sqlText += ", BankInfo=@BankInfo";
                    sqlText += ", BankAccountNo=@BankAccountNo";
                    sqlText += ", IsPermanent=@IsPermanent";
                    sqlText += " , IsBuild=@IsBuild";
                    sqlText += " , IsTAXApplicable=@IsTAXApplicable";
                    sqlText += " , IsPFApplicable=@IsPFApplicable";
                    sqlText += " , IsGFApplicable=@IsGFApplicable";
                    sqlText += " ,IsInactive=@IsInactive";
                    sqlText += ", Rank=@Rank";
                    sqlText += ", Duration=@Duration";
                    sqlText += ", DotedLineReport=@DotedLineReport";
                    sqlText += ", GrossSalary=@GrossSalary";
                    sqlText += ", BasicSalary=@BasicSalary";
                    sqlText += ", GFStartFrom=@GFStartFrom";
                    sqlText += ", Other1=@Other1";
                    sqlText += ", Other2=@Other2";
                    sqlText += ", Other3=@Other3";
                    sqlText += ", Other4=@Other4";
                    sqlText += ", Other5=@Other5";
                    sqlText += ", IsJobBefore=@IsJobBefore";
                    
                    sqlText += ", AccountType=@AccountType";
                    sqlText += ", FirstHoliday=@FirstHoliday";
                    sqlText += ", SecondHoliday=@SecondHoliday";

                    sqlText += ", ProjectId=@ProjectId";
                    sqlText += ", DepartmentId=@DepartmentId";
                    sqlText += ", SectionId=@SectionId";
                    sqlText += ", DesignationId=@DesignationId";
                    sqlText += ", BankAccountName=@BankAccountName";
                    sqlText += ", Routing_No=@Routing_No";
                    sqlText += ", EmpCategory=@EmpCategory";
                    sqlText += ", EmpJobType=@EmpJobType";
                    sqlText += ", Retirement=@Retirement";
                    sqlText += ", Force=@Force";
                    sqlText += ", Extentionyn=@Extentionyn";

                    sqlText += ", IsRebate=@IsRebate";
                    sqlText += ", Remarks=@Remarks";
                    sqlText += ", IsActive=@IsActive";
                    sqlText += ", LastUpdateBy=@LastUpdateBy";
                    sqlText += ", LastUpdateAt=@LastUpdateAt";
                    sqlText += ", LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";







                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                    cmdUpdate.Parameters.AddWithValue("@JoinDate", Ordinary.DateToString(vm.JoinDate));
                    cmdUpdate.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate));
                    cmdUpdate.Parameters.AddWithValue("@RetirementDate", Ordinary.DateToString(vm.RetirementDate));
                    cmdUpdate.Parameters.AddWithValue("@fristExDate", Ordinary.DateToString(vm.fristExDate));
                    cmdUpdate.Parameters.AddWithValue("@secondExDate", Ordinary.DateToString(vm.secondExDate));
                    cmdUpdate.Parameters.AddWithValue("@ContrExDate", Ordinary.DateToString(vm.ContrExDate));
                    cmdUpdate.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate));
                    cmdUpdate.Parameters.AddWithValue("@GFStartFrom", Ordinary.DateToString(vm.GFStartFrom));
                    cmdUpdate.Parameters.AddWithValue("@ProbationMonth", vm.ProbationMonth);
                    cmdUpdate.Parameters.AddWithValue("@ExtendedProbationMonth", vm.ExtendedProbationMonth);
                    cmdUpdate.Parameters.AddWithValue("@ProbationEnd", Ordinary.DateToString(vm.ProbationEnd));
                    cmdUpdate.Parameters.AddWithValue("@DateOfPermanent", Ordinary.DateToString(vm.DateOfPermanent));
                    cmdUpdate.Parameters.AddWithValue("@EmploymentStatus_E", vm.EmploymentStatus_E);
                    cmdUpdate.Parameters.AddWithValue("@EmploymentType_E", vm.EmploymentType_E);
                    cmdUpdate.Parameters.AddWithValue("@Supervisor", vm.Supervisor ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BankInfo", vm.BankInfo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BankAccountNo", vm.BankAccountNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsPermanent", vm.IsPermanent);
                    cmdUpdate.Parameters.AddWithValue("@IsBuild", vm.IsBuild);
                    cmdUpdate.Parameters.AddWithValue("@IsTAXApplicable", vm.IsTAXApplicable);
                    cmdUpdate.Parameters.AddWithValue("@IsPFApplicable", vm.IsPFApplicable);
                    cmdUpdate.Parameters.AddWithValue("@IsGFApplicable", vm.IsGFApplicable);
                    cmdUpdate.Parameters.AddWithValue("@Rank", vm.Rank ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Duration", vm.Duration ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@DotedLineReport", vm.DotedLineReport ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsInactive", vm.IsInactive);
                    cmdUpdate.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);
                    cmdUpdate.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                    cmdUpdate.Parameters.AddWithValue("@Other1", vm.Other1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Other2", vm.Other2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Other3", vm.Other3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Other4", vm.Other4 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Other5", vm.Other5 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsJobBefore", vm.IsJobBefore);
                    cmdUpdate.Parameters.AddWithValue("@AccountType", vm.AccountType ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@FirstHoliday", vm.FirstHoliday ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@SecondHoliday", vm.SecondHoliday ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdUpdate.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdUpdate.Parameters.AddWithValue("@BankAccountName", vm.BankAccountName ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Routing_No", vm.Routing_No ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EmpCategory", vm.EmpCategory ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EmpJobType", vm.EmpJobType ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Retirement", vm.Retirement ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Force", vm.Force ?? Convert.DBNull); 
                    cmdUpdate.Parameters.AddWithValue("@Extentionyn", vm.Extentionyn ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@IsRebate", vm.IsRebate);

                    cmdUpdate.Transaction = transaction;
                    var exeRes1 = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes1);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                    }

                    #endregion Commit

                    #endregion Update Settings

                    #region EmployeeStructureGroup

                    sqlText = "Select count(Id) from EmployeeStructureGroup where EmployeeId=@EmployeeId";
                    SqlCommand cmdExists = new SqlCommand(sqlText, currConn);
                    cmdExists.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdExists.Transaction = transaction;
                    var exeRes2 = cmdExists.ExecuteScalar();
                    int Exists = Convert.ToInt32(exeRes2);
                    if (Exists <= 0)
                    {
                        #region EmployeeStructureGroup Insert

                        sqlText = "  ";
                        sqlText += @" INSERT INTO EmployeeStructureGroup(EmployeeId,EmployeeGroupId,LeaveStructureId,SalaryStructureId,PFStructureId
                            ,TaxStructureId,BonusStructureId,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                            ) 
                            VALUES (@EmployeeId,0,0,0,0,0,0,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                            ) ";

                        SqlCommand cmdESG = new SqlCommand(sqlText, currConn);

                        //cmdESG.Parameters.AddWithValue("@Id", EmployeeStructureGroup_Id);

                        cmdESG.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                        cmdESG.Parameters.AddWithValue("@Remarks", "");
                        cmdESG.Parameters.AddWithValue("@IsActive", true);
                        cmdESG.Parameters.AddWithValue("@IsArchive", false);
                        cmdESG.Parameters.AddWithValue("@CreatedBy", vm.LastUpdateBy);
                        cmdESG.Parameters.AddWithValue("@CreatedAt", vm.LastUpdateAt);
                        cmdESG.Parameters.AddWithValue("@CreatedFrom", vm.LastUpdateFrom);

                        cmdESG.Transaction = transaction;
                        cmdESG.ExecuteNonQuery();
                        #endregion EmployeeStructureGroup Insert
                    }
                    #endregion EmployeeStructureGroup
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("PersonalDetail Update", "Could not found any item.");
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


        public string[] Update_EmployeeJob(EmployeeJobVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeJob Update"; //Method Name

            int transResult = 0;

            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeJob"); }

                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "UPDATE EmployeeJob SET";

                    sqlText += "  ProjectId=@ProjectId";
                    sqlText += ", DepartmentId=@DepartmentId";
                    sqlText += ", SectionId=@SectionId";

                    sqlText += ", Other1=@Other1";
                    sqlText += ", Other2=@Other2";
                    sqlText += ", Other3=@Other3";
                    sqlText += ", Other4=@Other4";
                    sqlText += ", Other5=@Other5";
                    if (!string.IsNullOrWhiteSpace(vm.DesignationId))
                    {

                        sqlText += ", DesignationId=@DesignationId";
                    }
                    sqlText += ", LastUpdateBy=@LastUpdateBy";
                    sqlText += ", LastUpdateAt=@LastUpdateAt";
                    sqlText += ", LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where EmployeeId=@EmployeeId";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdUpdate.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdUpdate.Parameters.AddWithValue("@Other1", vm.Other1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Other2", vm.Other2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Other3", vm.Other3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Other4", vm.Other4 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Other5", vm.Other5 ?? Convert.DBNull);
                    if (!string.IsNullOrWhiteSpace(vm.DesignationId))
                    {
                        cmdUpdate.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    }

                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    var exeRes1 = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes1);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query


                    if (transResult <= 0)
                    {
                    }


                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("PersonalDetail Update", "Could not found any item.");
                }

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";
                #endregion Commit
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

        public string[] Update_EmployeeJob_Designation(EmployeeJobVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeJob Update"; //Method Name

            int transResult = 0;

            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeJob"); }

                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "UPDATE EmployeeJob SET";

                    sqlText += "  DesignationId=@DesignationId";

                    sqlText += ", LastUpdateBy=@LastUpdateBy";
                    sqlText += ", LastUpdateAt=@LastUpdateAt";
                    sqlText += ", LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where EmployeeId=@EmployeeId";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@DesignationId", vm.DesignationId);

                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes1 = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes1);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query


                    if (transResult <= 0)
                    {
                    }


                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("PersonalDetail Update", "Could not found any item.");
                }

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";
                #endregion Commit
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

        public string[] UpdateJobSalary(string EmployeeId, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeJob Update"; //Method Name

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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("UpdateToEmployeeJob");
                }
                #endregion open connection and transaction

                #region Update Settings

                sqlText = "";
                sqlText += "declare @EmployeeId as varchar(50)";
                sqlText += " set @EmployeeId='" + EmployeeId + "' ";

                sqlText += @"

update EmployeeJob set GrossSalary=sal.GrossSalary
from(
SELECT  distinct EmployeeId, sum(amount)   GrossSalary
FROM            EmployeeSalaryStructureDetail
WHERE        (EmployeeId = @EmployeeId)
and IsEarning=1 and SalaryType not in('gross')group by EmployeeId) sal where EmployeeJob.EmployeeId=sal.EmployeeId

update EmployeeJob set BasicSalary=sal.BasicSalary
from(
SELECT   distinct EmployeeId,  sum(amount)   BasicSalary
FROM            EmployeeSalaryStructureDetail
WHERE        (EmployeeId = @EmployeeId)
and IsEarning=1 and SalaryType  in('basic')
group by EmployeeId) sal where EmployeeJob.EmployeeId=sal.EmployeeId
 
";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);

                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);

                retResults[2] = EmployeeId;// Return Id
                retResults[3] = sqlText; //  SQL Query

                #region Commit

                if (transResult <= 0)
                {
                }

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
        public List<EmployeeJobVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeJobVM> VMs = new List<EmployeeJobVM>();
            EmployeeJobVM vm;
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
 isnull(j.Id,0)                                 Id
,ei.EmployeeId
,isnull(j.JoinDate						, 0) JoinDate
,isnull(j.ProbationMonth						, '0') ProbationMonth
,isnull(j.ProbationEnd					, 'NA') ProbationEnd
,isnull(j.DateOfPermanent				, 'NA') DateOfPermanent
,isnull(j.EmploymentStatus_E			, '0') EmploymentStatus_E
,isnull(j.EmploymentType_E				, '0') EmploymentType_E
,isnull(j.Supervisor					, 'NA') Supervisor
,isnull(j.BankInfo						, 'NA') BankInfo
,isnull(j.BankAccountNo					, 'NA') BankAccountNo
,isnull(j.IsPermanent					, 0) IsPermanent
,isnull(j.GrossSalary					, 0) GrossSalary
,isnull(j.BasicSalary					, 0) BasicSalary
,isnull(j.Remarks						, 'NA') Remarks
,isnull(ei.Project						, 'NA') Project
,isnull(ei.Department					, 'NA') Department
,isnull(ei.Section						, 'NA') Section
,isnull(ei.Designation					, 'NA') Designation
,isnull(g.IsGross,0)IsGross
,ei.Grade GradeName
,isnull(emType.Name					, '0') EmploymentType_EName
,isnull(emStatus.Name					, '0') EmploymentStatus_EName
,isnull(j.Other1, 'NA') Other1
,isnull(j.Other2, 'NA') Other2
,isnull(j.Other3, 'NA') Other3
,isnull(j.Other4, 'NA') Other4
,isnull(j.Other5, 'NA') Other5
,ISNULL(j.IsJobBefore, 0) IsJobBefore
,j.AccountType
,b.Name BankName
,j.FirstHoliday
,j.SecondHoliday

    From ViewEmployeeInformation ei
	left outer join EmployeeJob j on j.EmployeeId=ei.EmployeeId 
	left outer join dbo.EmployeeStructureGroup g on g.EmployeeId=j.EmployeeId
	left outer join dbo.EnumEmploymentType emType on emType.Id=j.EmploymentType_E
	left outer join dbo.EnumEmploymentStatus emStatus on emStatus.Id=j.EmploymentStatus_E

	left outer join dbo.Bank b on b.Id=j.BankInfo
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

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                //objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);

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

                //SqlDataReader dr;
                //dr = objComm.ExecuteReader();
                //order by ei.GradeSL

                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(objComm);
                ad.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    vm = new EmployeeJobVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.ProbationMonth = Convert.ToInt32(dr["ProbationMonth"]);
                    vm.ProbationEnd = Ordinary.StringToDate(dr["ProbationEnd"].ToString());
                    vm.DateOfPermanent = Ordinary.StringToDate(dr["DateOfPermanent"].ToString());
                    vm.EmploymentStatus_E = dr["EmploymentStatus_E"].ToString();
                    vm.EmploymentType_E = dr["EmploymentType_E"].ToString();
                    vm.Supervisor = dr["Supervisor"].ToString();
                    vm.BankInfo = dr["BankInfo"].ToString();
                    vm.BankAccountNo = dr["BankAccountNo"].ToString();
                    vm.IsPermanent = Convert.ToBoolean(dr["IsPermanent"]);
                    vm.Remarks = dr["Remarks"].ToString();

                    vm.IsGross = Convert.ToBoolean(dr["IsGross"]);

                    vm.Project = dr["Project"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.GradeName = dr["GradeName"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());

                    vm.EmploymentType_EName = dr["EmploymentType_EName"].ToString();
                    vm.EmploymentStatus_EName = dr["EmploymentStatus_EName"].ToString();

                    vm.Other1 = dr["Other1"].ToString();
                    vm.Other2 = dr["Other2"].ToString();
                    vm.Other3 = dr["Other3"].ToString();
                    vm.Other4 = dr["Other4"].ToString();
                    vm.Other5 = dr["Other5"].ToString();
                    vm.IsJobBefore = Convert.ToBoolean(dr["IsJobBefore"]);
                    vm.AccountType = dr["AccountType"].ToString();
                    vm.BankName = dr["BankName"].ToString();
                    vm.FirstHoliday = dr["FirstHoliday"].ToString();
                    vm.SecondHoliday = dr["SecondHoliday"].ToString();


                    VMs.Add(vm);
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

            return VMs;
        }

        #endregion
    }
}
