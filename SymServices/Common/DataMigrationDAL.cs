using SymOrdinary;
using SymServices.HRM;
using SymServices.Leave;
using SymViewModel.Common;
using SymViewModel.Leave;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymServices.Common
{
    public class DataMigrationDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        //==================Insert =================
        public string[] Insert(string structureType = "", ShampanIdentityVM siVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
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
                #region Save
                EmployeeStructureGroupDAL _sgRepo = new EmployeeStructureGroupDAL();
                EmployeeStructureGroupVM vm = new EmployeeStructureGroupVM();

                sqlText = @"SELECT
Id
,EmployeeId
,EmployeeGroupId
,LeaveStructureId
,SalaryStructureId
,PFStructureId
,TaxStructureId
,BonusStructureId
,ProjectAllocationId
,GradeId
,StepId
,IsGross
,LeaveYear
,SalaryInput
From EmployeeStructureGroup

WHERE 1=1 AND IsNew = 1
and EmployeeId in(select id from EmployeeInfo where IsActive=1)
";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (structureType == "SalaryStructure")
                {
                    foreach (DataRow item in dt.AsEnumerable())
                    {
                        retResults = _sgRepo.EmployeeSalaryStructureFromBasic(item["EmployeeId"].ToString(), item["SalaryStructureId"].ToString(), Convert.ToDecimal(item["salaryInput"]), item["GradeId"].ToString(), item["StepId"].ToString(), Convert.ToBoolean(item["IsGross"]),0, siVM, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                              retResults[1] = "Salary Not Process";
                              throw new ArgumentNullException("Salary Not Process", "");
                        }
                    }
                }
                else if (structureType == "PFStructure")
                {
                    foreach (DataRow item in dt.AsEnumerable())
                    {
                        retResults = _sgRepo.EmployeePFStructure(item["EmployeeId"].ToString(), item["PFStructureId"].ToString(), siVM, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            retResults[1] = "PFStructure Not Process";
                            throw new ArgumentNullException("PFStructure Not Process", "");
                        }
                    }
                }
                else if (structureType == "TaxStructure")
                {
                    foreach (DataRow item in dt.AsEnumerable())
                    {
                        retResults = _sgRepo.EmployeeTaxStructure(item["EmployeeId"].ToString(), item["TaxStructureId"].ToString(), siVM, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            retResults[1] = "TaxStructure Not Process";
                            throw new ArgumentNullException("TaxStructure Not Process", "");
                        }
                    }
                }
                else if (structureType == "LeaveStructure")
                {
                    foreach (DataRow item in dt.AsEnumerable())
                    {
                        retResults = _sgRepo.EmployeeLeaveStructure(item["EmployeeId"].ToString(), item["LeaveStructureId"].ToString(), item["LeaveYear"].ToString(), siVM, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            retResults[1] = "LeaveStructure Not Process";
                            throw new ArgumentNullException("LeaveStructure Not Process", "");
                        }
                    }
                }
                else if (structureType == "EmployeeGroup")
                {
                    foreach (DataRow item in dt.AsEnumerable())
                    {
                        retResults = _sgRepo.EmployeeGroup(item["EmployeeId"].ToString(), item["EmployeeGroupId"].ToString(), siVM, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            retResults[1] = "EmployeeGroup Not Process";
                            throw new ArgumentNullException("EmployeeGroup Not Process", "");
                        }
                    }
                }
                //else if (structureType == "BonusStructure")
                //{
                //    foreach (DataRow item in dt.AsEnumerable())
                //    {
                //        retResults = _sgRepo.BonusStructure(item["EmployeeId"].ToString(), item["BonusStructureId"].ToString(), siVM, currConn, transaction);
                //    }
                //}

                #endregion Save
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }


        public string[] OpeningLeavePost( ShampanIdentityVM siVM , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
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
                #region Save
                EmployeeStructureGroupDAL _sgRepo = new EmployeeStructureGroupDAL();
                EmployeeStructureGroupVM vm = new EmployeeStructureGroupVM();

                sqlText = @"SELECT
Id
From EmployeeLeave
where isactive=1
order by id

";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                DataTable dt = new DataTable();
                da.Fill(dt);

                EmployeeLeaveDAL _ldal = new EmployeeLeaveDAL();
                EmployeeLeaveVM employeeLeaveVM = new EmployeeLeaveVM();
                employeeLeaveVM.IsApprove = true;
                employeeLeaveVM.IsReject = false;
                employeeLeaveVM.ApprovedBy ="Admin";
                employeeLeaveVM.RejectedBy = "";

                employeeLeaveVM.LastUpdateAt    = siVM.LastUpdateAt;
                employeeLeaveVM.LastUpdateBy = siVM.LastUpdateBy;
                employeeLeaveVM.LastUpdateFrom = siVM.LastUpdateFrom;
                employeeLeaveVM.CreatedAt = siVM.CreatedAt;
                employeeLeaveVM.CreatedBy = siVM.CreatedBy;
                employeeLeaveVM.CreatedFrom = siVM.CreatedFrom;


                    foreach (DataRow item in dt.AsEnumerable())
                    {
                        employeeLeaveVM.Id =Convert.ToInt32( item["Id"]);
                        retResults = _ldal.Approve(employeeLeaveVM, currConn, transaction);

                        if (retResults[0] == "Fail")
                        {
                            //continue;
                            throw new ArgumentNullException(retResults[1], retResults[1]);
                        }
                    }
                
                

                #endregion Save
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }


//        private void tt()
//        { 
// --delete from EmployeeLeave
//delete from EmployeeLeave where employeeid='1_1'

//         update EmployeeLeave set employeeid=employeeinfo.id
// from employeeinfo
// where employeeinfo.code='BES-'+EmployeeLeave.Remarks

// update EmployeeLeave set EmployeeLeaveStructureId=EmployeeLeaveStructure.id
// from EmployeeLeaveStructure
// where EmployeeLeave.employeeid=EmployeeLeaveStructure.employeeid

//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170115','20170115','0','admin','19000101',0,'Admin','19000101',0,0,'00082' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170115','20170115','0','admin','19000101',0,'Admin','19000101',0,0,'00165' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170122','20170122','0','admin','19000101',0,'Admin','19000101',0,0,'00171' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170129','20170202','0','admin','19000101',0,'Admin','19000101',0,0,'00076' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170122','20170124','0','admin','19000101',0,'Admin','19000101',0,0,'00029' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170101','20170101','0','admin','19000101',0,'Admin','19000101',0,0,'00141' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170102','20170102','0','admin','19000101',0,'Admin','19000101',0,0,'00058' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170129','20170131','0','admin','19000101',0,'Admin','19000101',0,0,'00184' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170202','20170202','0','admin','19000101',0,'Admin','19000101',0,0,'00082' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170206','20170206','0','admin','19000101',0,'Admin','19000101',0,0,'00111' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170205','20170205','0','admin','19000101',0,'Admin','19000101',0,0,'00024' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170207','20170207','0','admin','19000101',0,'Admin','19000101',0,0,'00109' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170103','20170103','0','admin','19000101',0,'Admin','19000101',0,0,'00096' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170212','20170213','0','admin','19000101',0,'Admin','19000101',0,0,'00189' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170205','20170207','0','admin','19000101',0,'Admin','19000101',0,0,'00201' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170205','20170205','0','admin','19000101',0,'Admin','19000101',0,0,'00184' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170209','20170209','0','admin','19000101',0,'Admin','19000101',0,0,'00184' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170219','20170219','0','admin','19000101',0,'Admin','19000101',0,0,'00184' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170213','20170213','0','admin','19000101',0,'Admin','19000101',0,0,'00168' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170214','20170215','0','admin','19000101',0,'Admin','19000101',0,0,'00069' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170215','20170215','0','admin','19000101',0,'Admin','19000101',0,0,'00155' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170219','20170219','0','admin','19000101',0,'Admin','19000101',0,0,'00109' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170222','20170223','0','admin','19000101',0,'Admin','19000101',0,0,'00080' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170222','20170222','0','admin','19000101',0,'Admin','19000101',0,0,'00066' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170214','20170216','0','admin','19000101',0,'Admin','19000101',0,0,'00193' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170119','20170119','0','admin','19000101',0,'Admin','19000101',0,0,'00193' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170219','20170220','0','admin','19000101',0,'Admin','19000101',0,0,'00193' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170301','20170302','0','admin','19000101',0,'Admin','19000101',0,0,'00096' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170226','20170226','0','admin','19000101',0,'Admin','19000101',0,0,'00181' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170305','20170305','0','admin','19000101',0,'Admin','19000101',0,0,'00069' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170312','20170315','0','admin','19000101',0,'Admin','19000101',0,0,'00062' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170309','20170309','0','admin','19000101',0,'Admin','19000101',0,0,'00197' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170309','20170309','0','admin','19000101',0,'Admin','19000101',0,0,'00073' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170312','20170312','0','admin','19000101',0,'Admin','19000101',0,0,'00155' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170314','20170316','0','admin','19000101',0,'Admin','19000101',0,0,'00089' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170314','20170316','0','admin','19000101',0,'Admin','19000101',0,0,'00162' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170319','20170323','0','admin','19000101',0,'Admin','19000101',0,0,'00101' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170312','20170312','0','admin','19000101',0,'Admin','19000101',0,0,'00141' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170316','20170316','0','admin','19000101',0,'Admin','19000101',0,0,'00209' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170315','20170316','0','admin','19000101',0,'Admin','19000101',0,0,'00032' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170316','20170316','0','admin','19000101',0,'Admin','19000101',0,0,'00109' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170313','20170313','0','admin','19000101',0,'Admin','19000101',0,0,'00058' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170321','20170321','0','admin','19000101',0,'Admin','19000101',0,0,'00152' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170116','20170117','0','admin','19000101',0,'Admin','19000101',0,0,'00152' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170322','20170322','0','admin','19000101',0,'Admin','19000101',0,0,'00214' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170322','20170323','0','admin','19000101',0,'Admin','19000101',0,0,'00029' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170327','20170327','0','admin','19000101',0,'Admin','19000101',0,0,'00029' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170323','20170323','0','admin','19000101',0,'Admin','19000101',0,0,'00165' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170403','20170403','0','admin','19000101',0,'Admin','19000101',0,0,'00059' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170406','20170406','0','admin','19000101',0,'Admin','19000101',0,0,'00024' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170406','20170406','0','admin','19000101',0,'Admin','19000101',0,0,'00187' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170313','20170313','0','admin','19000101',0,'Admin','19000101',0,0,'00096' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170411','20170411','0','admin','19000101',0,'Admin','19000101',0,0,'00069' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170319','20170319','0','admin','19000101',0,'Admin','19000101',0,0,'00219' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170412','20170412','0','admin','19000101',0,'Admin','19000101',0,0,'00168' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170416','20170416','0','admin','19000101',0,'Admin','19000101',0,0,'00229' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170418','20170418','0','admin','19000101',0,'Admin','19000101',0,0,'00147' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170413','20170413','0','admin','19000101',0,'Admin','19000101',0,0,'00141' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170413','20170413','0','admin','19000101',0,'Admin','19000101',0,0,'00201' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170419','20170419','0','admin','19000101',0,'Admin','19000101',0,0,'00040' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170423','20170423','0','admin','19000101',0,'Admin','19000101',0,0,'00119' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170424','20170425','0','admin','19000101',0,'Admin','19000101',0,0,'00058' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170416','20170416','0','admin','19000101',0,'Admin','19000101',0,0,'00184' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170425','20170425','0','admin','19000101',0,'Admin','19000101',0,0,'00184' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170430','20170430','0','admin','19000101',0,'Admin','19000101',0,0,'00237' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170430','20170430','0','admin','19000101',0,'Admin','19000101',0,0,'00101' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170430','20170430','0','admin','19000101',0,'Admin','19000101',0,0,'00216' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170424','20170424','0','admin','19000101',0,'Admin','19000101',0,0,'00059' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170427','20170427','0','admin','19000101',0,'Admin','19000101',0,0,'00223' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170430','20170430','0','admin','19000101',0,'Admin','19000101',0,0,'00062' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170502','20170502','0','admin','19000101',0,'Admin','19000101',0,0,'00235' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170511','20170511','0','admin','19000101',0,'Admin','19000101',0,0,'00005' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170430','20170430','0','admin','19000101',0,'Admin','19000101',0,0,'00171' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170507','20170507','0','admin','19000101',0,'Admin','19000101',0,0,'00201' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170507','20170508','0','admin','19000101',0,'Admin','19000101',0,0,'00062' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170509','20170509','0','admin','19000101',0,'Admin','19000101',0,0,'00205' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170514','20170515','0','admin','19000101',0,'Admin','19000101',0,0,'00047' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170507','20170507','0','admin','19000101',0,'Admin','19000101',0,0,'00066' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170511','20170511','0','admin','19000101',0,'Admin','19000101',0,0,'00069' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170511','20170511','0','admin','19000101',0,'Admin','19000101',0,0,'00040' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170504','20170504','0','admin','19000101',0,'Admin','19000101',0,0,'00098' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170511','20170511','0','admin','19000101',0,'Admin','19000101',0,0,'00098' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170514','20170514','0','admin','19000101',0,'Admin','19000101',0,0,'00098' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170514','20170514','0','admin','19000101',0,'Admin','19000101',0,0,'00229' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170511','20170511','0','admin','19000101',0,'Admin','19000101',0,0,'00171' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170511','20170511','0','admin','19000101',0,'Admin','19000101',0,0,'00098' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170514','20170514','0','admin','19000101',0,'Admin','19000101',0,0,'00098' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170516','20170516','0','admin','19000101',0,'Admin','19000101',0,0,'00155' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170518','20170518','0','admin','19000101',0,'Admin','19000101',0,0,'00230' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170517','20170518','0','admin','19000101',0,'Admin','19000101',0,0,'00077' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170518','20170518','0','admin','19000101',0,'Admin','19000101',0,0,'00165' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170521','20170521','0','admin','19000101',0,'Admin','19000101',0,0,'00229' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170528','20170601','0','admin','19000101',0,'Admin','19000101',0,0,'00189' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170524','20170525','0','admin','19000101',0,'Admin','19000101',0,0,'00189' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170504','20170504','0','admin','19000101',0,'Admin','19000101',0,0,'00189' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170511','20170511','0','admin','19000101',0,'Admin','19000101',0,0,'00189' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170517','20170517','0','admin','19000101',0,'Admin','19000101',0,0,'00189' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170521','20170521','0','admin','19000101',0,'Admin','19000101',0,0,'00189' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170522','20170523','0','admin','19000101',0,'Admin','19000101',0,0,'00237' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170523','20170524','0','admin','19000101',0,'Admin','19000101',0,0,'00168' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170521','20170524','0','admin','19000101',0,'Admin','19000101',0,0,'00155' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170521','20170521','0','admin','19000101',0,'Admin','19000101',0,0,'00171' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170523','20170525','0','admin','19000101',0,'Admin','19000101',0,0,'00229' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170525','20170525','0','admin','19000101',0,'Admin','19000101',0,0,'00058' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170518','20170518','0','admin','19000101',0,'Admin','19000101',0,0,'00140' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170521','20170521','0','admin','19000101',0,'Admin','19000101',0,0,'00140' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170528','20170528','0','admin','19000101',0,'Admin','19000101',0,0,'00198' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170528','20170529','0','admin','19000101',0,'Admin','19000101',0,0,'00223' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170530','20170530','0','admin','19000101',0,'Admin','19000101',0,0,'00135' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170529','20170529','0','admin','19000101',0,'Admin','19000101',0,0,'00066' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170604','20170608','0','admin','19000101',0,'Admin','19000101',0,0,'00147' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -2)','20170611','20170615','0','admin','19000101',0,'Admin','19000101',0,0,'00147' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170531','20170601','0','admin','19000101',0,'Admin','19000101',0,0,'00069' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170608','20170608','0','admin','19000101',0,'Admin','19000101',0,0,'00005' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170611','20170611','0','admin','19000101',0,'Admin','19000101',0,0,'00005' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170618','20170622','0','admin','19000101',0,'Admin','19000101',0,0,'00188' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170702','20170704','0','admin','19000101',0,'Admin','19000101',0,0,'00188' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170608','20170608','0','admin','19000101',0,'Admin','19000101',0,0,'00230' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170612','20170612','0','admin','19000101',0,'Admin','19000101',0,0,'00223' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170618','20170622','0','admin','19000101',0,'Admin','19000101',0,0,'00080' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170612','20170612','0','admin','19000101',0,'Admin','19000101',0,0,'00069' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -2)','20170611','20170615','0','admin','19000101',0,'Admin','19000101',0,0,'00119' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170615','20170615','0','admin','19000101',0,'Admin','19000101',0,0,'00096' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170618','20170618','0','admin','19000101',0,'Admin','19000101',0,0,'00096' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -2)','20170620','20170624','0','admin','19000101',0,'Admin','19000101',0,0,'00189' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170613','20170613','0','admin','19000101',0,'Admin','19000101',0,0,'00189' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170620','20170620','0','admin','19000101',0,'Admin','19000101',0,0,'00155' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Paternity Leave','20170628','20170702','0','admin','19000101',0,'Admin','19000101',0,0,'00205' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170702','20170706','0','admin','19000101',0,'Admin','19000101',0,0,'00135' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170620','20170621','0','admin','19000101',0,'Admin','19000101',0,0,'00162' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170702','20170706','0','admin','19000101',0,'Admin','19000101',0,0,'00162' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170628','20170629','0','admin','19000101',0,'Admin','19000101',0,0,'00162' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170628','20170629','0','admin','19000101',0,'Admin','19000101',0,0,'00221' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170621','20170621','0','admin','19000101',0,'Admin','19000101',0,0,'00111' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170521','20170525','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170521','20170525','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -2)','20170528','20170601','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170614','20170614','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170511','20170511','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170511','20170511','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170503','20170504','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170503','20170504','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170424','20170424','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170424','20170424','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170416','20170416','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170220','20170220','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170205','20170205','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170205','20170205','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170116','20170116','0','admin','19000101',0,'Admin','19000101',0,0,'00175' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170629','20170629','0','admin','19000101',0,'Admin','19000101',0,0,'00058' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170629','20170629','0','admin','19000101',0,'Admin','19000101',0,0,'00233' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170621','20170622','0','admin','19000101',0,'Admin','19000101',0,0,'00024' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170704','20170713','0','admin','19000101',0,'Admin','19000101',0,0,'00259' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170628','20170629','0','admin','19000101',0,'Admin','19000101',0,0,'00029' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170628','20170628','0','admin','19000101',0,'Admin','19000101',0,0,'00141' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170702','20170702','0','admin','19000101',0,'Admin','19000101',0,0,'00201' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170711','20170711','0','admin','19000101',0,'Admin','19000101',0,0,'00075' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170716','20170720','0','admin','19000101',0,'Admin','19000101',0,0,'00176' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170716','20170716','0','admin','19000101',0,'Admin','19000101',0,0,'00059' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170716','20170720','0','admin','19000101',0,'Admin','19000101',0,0,'00155' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Paternity Leave','20170716','20170720','0','admin','19000101',0,'Admin','19000101',0,0,'00040' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170712','20170713','0','admin','19000101',0,'Admin','19000101',0,0,'00058' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170718','20170719','0','admin','19000101',0,'Admin','19000101',0,0,'00171' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170723','20170723','0','admin','19000101',0,'Admin','19000101',0,0,'00237' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170718','20170718','0','admin','19000101',0,'Admin','19000101',0,0,'00225' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170715','20170719','0','admin','19000101',0,'Admin','19000101',0,0,'00198' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170720','20170720','0','admin','19000101',0,'Admin','19000101',0,0,'00223' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170723','20170723','0','admin','19000101',0,'Admin','19000101',0,0,'00220' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170711','20170720','0','admin','19000101',0,'Admin','19000101',0,0,'00048' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170723','20170727','0','admin','19000101',0,'Admin','19000101',0,0,'00140' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170730','20170730','0','admin','19000101',0,'Admin','19000101',0,0,'00171' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170709','20170713','0','admin','19000101',0,'Admin','19000101',0,0,'00162' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170716','20170720','0','admin','19000101',0,'Admin','19000101',0,0,'00162' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170723','20170730','0','admin','19000101',0,'Admin','19000101',0,0,'00162' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170730','20170730','0','admin','19000101',0,'Admin','19000101',0,0,'00219' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170802','20170802','0','admin','19000101',0,'Admin','19000101',0,0,'00171' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170803','20170803','0','admin','19000101',0,'Admin','19000101',0,0,'00229' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170730','20170731','0','admin','19000101',0,'Admin','19000101',0,0,'00266' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170803','20170803','0','admin','19000101',0,'Admin','19000101',0,0,'00223' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170806','20170806','0','admin','19000101',0,'Admin','19000101',0,0,'00258' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170807','20170807','0','admin','19000101',0,'Admin','19000101',0,0,'00263' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170807','20170808','0','admin','19000101',0,'Admin','19000101',0,0,'00265' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170813','20170813','0','admin','19000101',0,'Admin','19000101',0,0,'00237' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170809','20170809','0','admin','19000101',0,'Admin','19000101',0,0,'00155' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170809','20170809','0','admin','19000101',0,'Admin','19000101',0,0,'00263' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170813','20170813','0','admin','19000101',0,'Admin','19000101',0,0,'00024' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170910','20170914','0','admin','19000101',0,'Admin','19000101',0,0,'00089' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -2)','20170827','20170831','0','admin','19000101',0,'Admin','19000101',0,0,'00089' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170904','20170907','0','admin','19000101',0,'Admin','19000101',0,0,'00089' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170820','20170824','0','admin','19000101',0,'Admin','19000101',0,0,'00089' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170816','20170817','0','admin','19000101',0,'Admin','19000101',0,0,'00089' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170813','20170813','0','admin','19000101',0,'Admin','19000101',0,0,'00089' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170820','20170824','0','admin','19000101',0,'Admin','19000101',0,0,'00028' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170810','20170810','0','admin','19000101',0,'Admin','19000101',0,0,'00241' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170813','20170813','0','admin','19000101',0,'Admin','19000101',0,0,'00241' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170727','20170727','0','admin','19000101',0,'Admin','19000101',0,0,'00241' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170711','20170711','0','admin','19000101',0,'Admin','19000101',0,0,'00241' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170816','20170816','0','admin','19000101',0,'Admin','19000101',0,0,'00223' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170816','20170817','0','admin','19000101',0,'Admin','19000101',0,0,'00013' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170827','20170831','0','admin','19000101',0,'Admin','19000101',0,0,'00096' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170827','20170831','0','admin','19000101',0,'Admin','19000101',0,0,'00019' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170820','20170820','0','admin','19000101',0,'Admin','19000101',0,0,'00220' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170821','20170822','0','admin','19000101',0,'Admin','19000101',0,0,'00223' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170821','20170821','0','admin','19000101',0,'Admin','19000101',0,0,'00254' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170813','20170813','0','admin','19000101',0,'Admin','19000101',0,0,'00209' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170813','20170813','0','admin','19000101',0,'Admin','19000101',0,0,'00209' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170827','20170827','0','admin','19000101',0,'Admin','19000101',0,0,'00233' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170820','20170820','0','admin','19000101',0,'Admin','19000101',0,0,'00165' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170831','20170831','0','admin','19000101',0,'Admin','19000101',0,0,'00246' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170820','20170821','0','admin','19000101',0,'Admin','19000101',0,0,'00188' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170830','20170831','0','admin','19000101',0,'Admin','19000101',0,0,'00214' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170829','20170829','0','admin','19000101',0,'Admin','19000101',0,0,'00108' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170904','20170905','0','admin','19000101',0,'Admin','19000101',0,0,'00074' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170831','20170831','0','admin','19000101',0,'Admin','19000101',0,0,'00024' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170831','20170831','0','admin','19000101',0,'Admin','19000101',0,0,'00048' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170829','20170829','0','admin','19000101',0,'Admin','19000101',0,0,'00229' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170831','20170831','0','admin','19000101',0,'Admin','19000101',0,0,'00059' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170904','20170907','0','admin','19000101',0,'Admin','19000101',0,0,'00215' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170904','20170905','0','admin','19000101',0,'Admin','19000101',0,0,'00267' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170904','20170905','0','admin','19000101',0,'Admin','19000101',0,0,'00237' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170904','20170906','0','admin','19000101',0,'Admin','19000101',0,0,'00013' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170831','20170831','0','admin','19000101',0,'Admin','19000101',0,0,'00241' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170904','20170904','0','admin','19000101',0,'Admin','19000101',0,0,'00254' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170904','20170904','0','admin','19000101',0,'Admin','19000101',0,0,'00247' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170904','20170905','0','admin','19000101',0,'Admin','19000101',0,0,'00029' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170831','20170831','0','admin','19000101',0,'Admin','19000101',0,0,'00253' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170906','20170906','0','admin','19000101',0,'Admin','19000101',0,0,'00252' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170911','20170911','0','admin','19000101',0,'Admin','19000101',0,0,'00155' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170911','20170911','0','admin','19000101',0,'Admin','19000101',0,0,'00257' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170904','20170904','0','admin','19000101',0,'Admin','19000101',0,0,'00257' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave (BL-05 -1)','20170919','20170923','0','admin','19000101',0,'Admin','19000101',0,0,'00207' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170913','20170914','0','admin','19000101',0,'Admin','19000101',0,0,'00096' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170917','20170917','0','admin','19000101',0,'Admin','19000101',0,0,'00218' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170904','20170904','0','admin','19000101',0,'Admin','19000101',0,0,'00271' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170909','20170909','0','admin','19000101',0,'Admin','19000101',0,0,'00271' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170820','20170820','0','admin','19000101',0,'Admin','19000101',0,0,'00271' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170910','20170910','0','admin','19000101',0,'Admin','19000101',0,0,'00265' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170912','20170912','0','admin','19000101',0,'Admin','19000101',0,0,'00265' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170924','20170925','0','admin','19000101',0,'Admin','19000101',0,0,'00155' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170906','20170906','0','admin','19000101',0,'Admin','19000101',0,0,'00264' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170911','20170911','0','admin','19000101',0,'Admin','19000101',0,0,'00264' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170904','20170906','0','admin','19000101',0,'Admin','19000101',0,0,'00246' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170917','20170917','0','admin','19000101',0,'Admin','19000101',0,0,'00062' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170928','20170928','0','admin','19000101',0,'Admin','19000101',0,0,'00005' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20171002','20171002','0','admin','19000101',0,'Admin','19000101',0,0,'00005' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170927','20170928','0','admin','19000101',0,'Admin','19000101',0,0,'00073' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170928','20170928','0','admin','19000101',0,'Admin','19000101',0,0,'00237' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20170920','20170925','0','admin','19000101',0,'Admin','19000101',0,0,'00152' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20171002','20171002','0','admin','19000101',0,'Admin','19000101',0,0,'00237' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170928','20170928','0','admin','19000101',0,'Admin','19000101',0,0,'00209' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20171002','20171002','0','admin','19000101',0,'Admin','19000101',0,0,'00209' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170927','20170928','0','admin','19000101',0,'Admin','19000101',0,0,'00216' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170529','20170529','0','admin','19000101',0,'Admin','19000101',0,0,'00213' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170820','20170820','0','admin','19000101',0,'Admin','19000101',0,0,'00213' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170628','20170629','0','admin','19000101',0,'Admin','19000101',0,0,'00213' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170924','20170925','0','admin','19000101',0,'Admin','19000101',0,0,'00254' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170924','20170925','0','admin','19000101',0,'Admin','19000101',0,0,'00274' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170926','20170926','0','admin','19000101',0,'Admin','19000101',0,0,'00241' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170928','20170928','0','admin','19000101',0,'Admin','19000101',0,0,'00059' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20171002','20171003','0','admin','19000101',0,'Admin','19000101',0,0,'00101' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Annual Leave','20171004','20171005','0','admin','19000101',0,'Admin','19000101',0,0,'00066' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170928','20170928','0','admin','19000101',0,'Admin','19000101',0,0,'00245' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20171002','20171005','0','admin','19000101',0,'Admin','19000101',0,0,'00275' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20170927','20170928','0','admin','19000101',0,'Admin','19000101',0,0,'00062' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20171009','20171009','0','admin','19000101',0,'Admin','19000101',0,0,'00231' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)
//INSERT INTO EmployeeLeave(EmployeeId,EmployeeLeaveStructureId,LeaveYear,LeaveType_E,FromDate,ToDate,TotalLeave,ApprovedBy,ApproveDate,IsApprove,RejectedBy,RejectDate,IsReject,IsHalfDay,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,IsLWP)
//VALUES( '1_1','1','2017','Sick /Casual Leave','20171015','20171019','0','admin','19000101',0,'Admin','19000101',0,0,'00274' ,1,0,'Admin','19000101','local','Admin','19000101','local',0)

//       }
    }
}
