using SymOrdinary;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;



namespace SymServices.Payroll
{
    public class EmployeeBonusDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods

        //==================Insert =================
        public string[] Insert(string bonusStructureId, EmployeeBonusDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {

            BonusStructureVM bonusStructureVM = new BonusStructureDAL().SelectAll(bonusStructureId).FirstOrDefault();

            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Bonus process"; //Method Name


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
                string EmployeeBonusId = "-";
                sqlText = "Select Top 1 Id from EmployeeBonus where BonusStructureId=@BonusStructureId and BranchId=@BranchId";
                SqlCommand cmdempBonusSelect = new SqlCommand(sqlText, currConn,transaction);
                cmdempBonusSelect.Parameters.AddWithValue("@BonusStructureId", bonusStructureVM.Id);
                cmdempBonusSelect.Parameters.AddWithValue("@BranchId", bonusStructureVM.BranchId);
                using (SqlDataReader dr = cmdempBonusSelect.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        EmployeeBonusId = dr["Id"].ToString();
                    }
                    dr.Close();
                }
                if (EmployeeBonusId=="-")
                {
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from EmployeeBonus where BranchId=@BranchId";
                    SqlCommand cmdempBonusCount = new SqlCommand(sqlText, currConn,transaction);
                    cmdempBonusCount.Parameters.AddWithValue("@BranchId", bonusStructureVM.BranchId);
					var exeRes = cmdempBonusCount.ExecuteScalar();
					int count2 = Convert.ToInt32(exeRes);

                     EmployeeBonusId = bonusStructureVM.BranchId.ToString() + "_" + (count2 + 1);
                     sqlText = @" Insert Into EmployeeBonus
(
Id
,Code
,Name
,BranchId
,BonusStructureId
,IsFixed
,PortionSalaryType
,BonusValue
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) VALUES (
 @Id
,@Code
,@Name
,@BranchId
,@BonusStructureId
,@IsFixed
,@PortionSalaryType
,@BonusValue
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)
";

                     SqlCommand employeeBnous = new SqlCommand(sqlText, currConn, transaction);
                     employeeBnous.Parameters.AddWithValue("@Id", EmployeeBonusId);
                     employeeBnous.Parameters.AddWithValue("@Code", bonusStructureVM.Code);
                     employeeBnous.Parameters.AddWithValue("@Name", bonusStructureVM.Name);
                     employeeBnous.Parameters.AddWithValue("@BranchId", bonusStructureVM.BranchId);
                     employeeBnous.Parameters.AddWithValue("@BonusStructureId", bonusStructureVM.Id);
                     employeeBnous.Parameters.AddWithValue("@IsFixed", bonusStructureVM.IsFixed);
                     employeeBnous.Parameters.AddWithValue("@PortionSalaryType", bonusStructureVM.PortionSalaryType);
                     employeeBnous.Parameters.AddWithValue("@BonusValue", bonusStructureVM.BonusValue);
                     employeeBnous.Parameters.AddWithValue("@Remarks", bonusStructureVM.Remarks);
                     employeeBnous.Parameters.AddWithValue("@IsActive", true);
                     employeeBnous.Parameters.AddWithValue("@IsArchive", false);
                     employeeBnous.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                     employeeBnous.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                     employeeBnous.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                     employeeBnous.ExecuteNonQuery();
                }

                sqlText = @"Delete EmployeeBonusDetail ";
                sqlText += " where EmployeeBonusId=@EmployeeBonusId";

                if (vm.ProjectId != "0_0")
                {
                    sqlText += " and ProjectId=@ProjectId";
                }
                if (vm.DepartmentId != "0_0")
                {
                    sqlText += " and DepartmentId=@DepartmentId";
                }
                if (vm.SectionId != "0_0")
                {
                    sqlText += " and SectionId=@SectionId";
                }

                SqlCommand cmdDeletePrevious = new SqlCommand(sqlText, currConn, transaction);
                cmdDeletePrevious.Parameters.AddWithValue("@EmployeeBonusId", EmployeeBonusId);
                if (vm.ProjectId != "0_0")
                {
                    cmdDeletePrevious.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                }
                if (vm.DepartmentId != "0_0")
                {
                    cmdDeletePrevious.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                }
                if (vm.SectionId != "0_0")
                {
                    cmdDeletePrevious.Parameters.AddWithValue("@SectionId", vm.SectionId);
                }
                cmdDeletePrevious.ExecuteNonQuery();


                List<EmployeeInfoVM> employeeVms = new List<EmployeeInfoVM>();
                EmployeeInfoVM employeeVm;
                sqlText = @"SELECT
 e.Id
,j.GrossSalary
,j.BasicSalary
,t.ProjectId
,t.DepartmentId
,t.SectionId
    From EmployeeInfo e
	left join EmployeeJob j on j.EmployeeId=e.Id
	left join EmployeeTransfer t on t.EmployeeId=e.Id and t.IsCurrent=1
Where e.IsArchive=0 And e.IsActive=1 and j.JoinDate>=@DOJFrom and j.JoinDate<=@DOJTo";
                if (vm.ProjectId != "0_0")
                {
                    sqlText += " and t.ProjectId=@ProjectId";
                }
                if (vm.DepartmentId != "0_0")
                {
                    sqlText += " and t.DepartmentId=@DepartmentId";
                }
                if (vm.SectionId != "0_0")
                {
                    sqlText += " and t.SectionId=@SectionId";
                }
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@DOJFrom", Ordinary.DateToString(bonusStructureVM.DOJFrom));
                cmd.Parameters.AddWithValue("@DOJTo", Ordinary.DateToString(bonusStructureVM.DOJTo));
                if (vm.ProjectId != "0_0")
                {
                    cmd.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                }
                if (vm.DepartmentId != "0_0")
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                }
                if (vm.SectionId != "0_0")
                {
                    cmd.Parameters.AddWithValue("@SectionId", vm.SectionId);
                }

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        employeeVm = new EmployeeInfoVM();
                        employeeVm.Id = dr["Id"].ToString();
                        employeeVm.ProjectId = dr["ProjectId"].ToString();
                        employeeVm.DepartmentId = dr["DepartmentId"].ToString();
                        employeeVm.SectionId = dr["SectionId"].ToString();
                        employeeVm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        employeeVm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                        employeeVms.Add(employeeVm);
                    }
                    dr.Close();
                }

                sqlText = @"Insert Into EmployeeBonusDetail

(
 EmployeeBonusId
,ProjectId
,DepartmentId
,SectionId
,EmployeeId
,GrossSalary
,BasicSalary
,BonusValue
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) Values (
 @EmployeeBonusId
,@ProjectId
,@DepartmentId
,@SectionId
,@EmployeeId
,@GrossSalary
,@BasicSalary
,@BonusValue
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)";
                SqlCommand cmdempBonusDet;
                if (employeeVms.Count > 0)
                {
                    decimal bonusValue = 0;
                    foreach (EmployeeInfoVM item in employeeVms)
                    {
                        bonusValue = bonusStructureVM.BonusValue;
                        if (!bonusStructureVM.IsFixed)
                        {
                            bonusValue = bonusValue / 100;
                            if (bonusStructureVM.PortionSalaryType.ToUpper().Trim() == "GROSS")
                            {
                                bonusValue = bonusValue * item.GrossSalary;
                            }
                            else
                            {
                                bonusValue = bonusValue * item.BasicSalary;
                            }
                        }


                        cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                        cmdempBonusDet.Parameters.AddWithValue("@EmployeeBonusId", EmployeeBonusId);
                        cmdempBonusDet.Parameters.AddWithValue("@ProjectId", item.ProjectId);
                        cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", item.DepartmentId);
                        cmdempBonusDet.Parameters.AddWithValue("@SectionId", item.SectionId);
                        cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", item.Id);
                        cmdempBonusDet.Parameters.AddWithValue("@GrossSalary", item.GrossSalary);
                        cmdempBonusDet.Parameters.AddWithValue("@BasicSalary", item.BasicSalary);
                        cmdempBonusDet.Parameters.AddWithValue("@BonusValue", bonusValue);
                        cmdempBonusDet.Parameters.AddWithValue("@Remarks", "--");
                        cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                        cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                        cmdempBonusDet.ExecuteNonQuery();
                    }

                }

                #region Save

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
       
        public List<EmployeeBonusVM> SelectAll(int BranchId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeBonusVM> VMs = new List<EmployeeBonusVM>();
            EmployeeBonusVM vm;
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

                sqlText = @"select 
 eb.Id
,eb.Code
,eb.Name
,eb.BranchId
,eb.BonusStructureId
,eb.IsFixed
,eb.PortionSalaryType
,eb.BonusValue
,eb.Remarks
,bs.Name BonusStructure
from EmployeeBonus eb
left outer join BonusStructure bs on eb.BonusStructureId=bs.Id
Where eb.IsArchive=0 And eb.IsActive=1 And eb.BranchId=@BranchId";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@BranchId", BranchId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeBonusVM();
                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.BonusStructureId = dr["BonusStructureId"].ToString();
                    vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                    vm.PortionSalaryType = dr["PortionSalaryType"].ToString();
                    vm.BonusValue = Convert.ToDecimal(dr["BonusValue"]);
                    vm.BonusStructureName = dr["BonusStructure"].ToString();
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
        
        public EmployeeBonusVM SelectById(string employeeBonusId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeBonusVM vm = new EmployeeBonusVM();

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

                sqlText = @"select 
 eb.Id
,eb.Code
,eb.Name
,eb.BranchId
,eb.BonusStructureId
,eb.IsFixed
,eb.PortionSalaryType
,eb.BonusValue
,eb.Remarks
,bs.Name BonusStructure
from EmployeeBonus eb
left outer join BonusStructure bs on eb.BonusStructureId=bs.Id
Where eb.IsArchive=0 And eb.IsActive=1 And eb.Id=@employeeBonusId";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@employeeBonusId", employeeBonusId);

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm.Id = dr["Id"].ToString();
                        vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                        vm.Code = dr["Code"].ToString();
                        vm.Name = dr["Name"].ToString();
                        vm.BonusStructureId = dr["BonusStructureId"].ToString();
                        vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                        vm.PortionSalaryType = dr["PortionSalaryType"].ToString();
                        vm.BonusValue = Convert.ToDecimal(dr["BonusValue"]);
                        vm.BonusStructureName = dr["BonusStructure"].ToString();
                        vm.Remarks = dr["Remarks"].ToString();

                    }
                    dr.Close();
                }
                vm.employeeBonusDetailVM = SelectAllEmpBonusDetails(currConn, true, vm.Id);


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
        
        public List<EmployeeBonusDetailVM> SelectAllEmpBonusDetails(SqlConnection currConn, bool callFromOutSide, string employeeBonusId = null)
        {

            #region Variables

            string sqlText = "";
            List<EmployeeBonusDetailVM> VMs = new List<EmployeeBonusDetailVM>();
            EmployeeBonusDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                }
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @" 
select 
 ebd.Id
,ebd.EmployeeBonusId
,ebd.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,ebd.BonusValue
,e.DesignationId, e.DepartmentId, e.SectionId, e.ProjectId
,ebd.Remarks
from EmployeeBonusDetail ebd
left outer join ViewEmployeeInformation e on ebd.EmployeeId=e.id

Where 1=1 and  ebd.IsArchive=0
";

                if (!string.IsNullOrWhiteSpace(employeeBonusId))
                {
                    sqlText += " and ebd.EmployeeBonusId=@employeeBonusId";
                }
                sqlText += @" 
ORDER BY e.Department,e.EmpName desc";


                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(employeeBonusId))
                {
                    objComm.Parameters.AddWithValue("@employeeBonusId", employeeBonusId);
                }

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeBonusDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.EmployeeBonusId = dr["EmployeeBonusId"].ToString();
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.BonusValue = Convert.ToDecimal(dr["BonusValue"]);
                        vm.Remarks = dr["Remarks"].ToString();
                        vm.EmpName = dr["EmpName"].ToString();
                        vm.Code = dr["Code"].ToString();
                        vm.Designation = dr["Designation"].ToString();
                        vm.Department = dr["Department"].ToString();
                        vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                        vm.Section = dr["Section"].ToString();
                        vm.Project = dr["Project"].ToString();
                        vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                        vm.DesignationId = dr["DesignationId"].ToString();
                        vm.DepartmentId = dr["DepartmentId"].ToString();
                        vm.SectionId = dr["SectionId"].ToString();
                        vm.ProjectId = dr["ProjectId"].ToString();
                        VMs.Add(vm);
                    }
                    dr.Close();
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
                if (currConn != null && !callFromOutSide)
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
        
        public EmployeeBonusDetailVM SelectByIdBonusDetail(int bonusDetailId)
        {
            #region Variables

            SqlConnection currConn = _dbsqlConnection.GetConnection();
            string sqlText = "";
            EmployeeBonusDetailVM vm = new EmployeeBonusDetailVM();
            #endregion
            try
            {
                #region open connection and transaction
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @" 
select 
 ebd.Id
,ebd.EmployeeBonusId
,ebd.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,ebd.BonusValue
,ebd.Remarks
from EmployeeBonusDetail ebd
left outer join ViewEmployeeInformation e on ebd.EmployeeId=e.id
Where ebd.Id=@bonusDetailId";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@bonusDetailId", bonusDetailId);

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.EmployeeBonusId = dr["EmployeeBonusId"].ToString();
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.BonusValue = Convert.ToDecimal(dr["BonusValue"]);
                        vm.Remarks = dr["Remarks"].ToString();
                        vm.EmpName = dr["EmpName"].ToString();
                        vm.Code = dr["Code"].ToString();
                        vm.Designation = dr["Designation"].ToString();
                        vm.Department = dr["Department"].ToString();
                        vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                        vm.Section = dr["Section"].ToString();
                        vm.Project = dr["Project"].ToString();
                        vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);

                    }
                    dr.Close();
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

            return vm;
        }
       
        public string[] SingleBonusUpdate(EmployeeBonusDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "Employee Bonus update"; //Method Name


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

                sqlText = @"Update EmployeeBonusDetail set

                         BonusValue     =@BonusValue
                        ,Remarks        =@Remarks
                        ,LastUpdateBy      =@LastUpdateBy
                        ,LastUpdateAt      =@LastUpdateAt
                        ,LastUpdateFrom    =@LastUpdateFrom
                        where ID=@Id
                        ";
                SqlCommand cmdempBonusDet;

                cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                cmdempBonusDet.Parameters.AddWithValue("@Id", vm.Id);
                cmdempBonusDet.Parameters.AddWithValue("@BonusValue", vm.BonusValue);
                cmdempBonusDet.Parameters.AddWithValue("@Remarks", vm.Remarks);
                cmdempBonusDet.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                cmdempBonusDet.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                cmdempBonusDet.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                cmdempBonusDet.ExecuteNonQuery();
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
                retResults[1] = "Data Update Successfully.";

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
       
        public string[] SingleBonusAdd(EmployeeBonusDetailVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            EmployeeBonusVM employeeBonusVM=SelectById(vm.EmployeeBonusId);
            BonusStructureVM bonusStructureVM = new BonusStructureDAL().SelectAll(employeeBonusVM.BonusStructureId).FirstOrDefault();

            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Single Employee Bonus Add"; //Method Name


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

                #region logic
                sqlText = "select COUNT(*) from EmployeeBonusDetail where EmployeeId=@EmployeeId and EmployeeBonusId=@EmployeeBonusId";
                SqlCommand cmd1 = new SqlCommand(sqlText, currConn,transaction);
                cmd1.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmd1.Parameters.AddWithValue("@EmployeeBonusId", vm.EmployeeBonusId);
				var exeRes = cmd1.ExecuteScalar();
				int count = Convert.ToInt32(exeRes);
                if (count>0)
                {
                    retResults[1] = "This employee already used";
                     throw new ArgumentNullException("This employee already used","This employee already used");
                }
                EmployeeInfoVM employeeVm =null;
                sqlText = @"SELECT TOP 1
 e.Id
,j.GrossSalary
,j.BasicSalary
,t.ProjectId
,t.DepartmentId
,t.SectionId
    From EmployeeInfo e
	left join EmployeeJob j on j.EmployeeId=e.Id
	left join EmployeeTransfer t on t.EmployeeId=e.Id and t.IsCurrent=1
Where e.IsArchive=0 And e.IsActive=1 and j.JoinDate>=@DOJFrom and j.JoinDate<=@DOJTo and e.Id=@EmployeeId";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@DOJFrom", Ordinary.DateToString(bonusStructureVM.DOJFrom));
                cmd.Parameters.AddWithValue("@DOJTo", Ordinary.DateToString(bonusStructureVM.DOJTo));
                cmd.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        employeeVm = new EmployeeInfoVM();
                        employeeVm.Id = dr["Id"].ToString();
                        employeeVm.ProjectId = dr["ProjectId"].ToString();
                        employeeVm.DepartmentId = dr["DepartmentId"].ToString();
                        employeeVm.SectionId = dr["SectionId"].ToString();
                        employeeVm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        employeeVm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    }
                    dr.Close();
                }

                

                sqlText = @"Insert Into EmployeeBonusDetail

(
 EmployeeBonusId
,ProjectId
,DepartmentId
,SectionId
,EmployeeId
,GrossSalary
,BasicSalary
,BonusValue
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) Values (
 @EmployeeBonusId
,@ProjectId
,@DepartmentId
,@SectionId
,@EmployeeId
,@GrossSalary
,@BasicSalary
,@BonusValue
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)";
                SqlCommand cmdempBonusDet;
                if (employeeVm != null)
                {
                    decimal bonusValue = 0;
                    bonusValue = bonusStructureVM.BonusValue;
                    if (!bonusStructureVM.IsFixed)
                    {
                        bonusValue = bonusValue / 100;
                        if (bonusStructureVM.PortionSalaryType.ToUpper().Trim() == "GROSS")
                        {
                            bonusValue = bonusValue * employeeVm.GrossSalary;
                        }
                        else
                        {
                            bonusValue = bonusValue * employeeVm.BasicSalary;
                        }
                    }


                    cmdempBonusDet = new SqlCommand(sqlText, currConn, transaction);
                    cmdempBonusDet.Parameters.AddWithValue("@EmployeeBonusId", employeeBonusVM.Id);
                    cmdempBonusDet.Parameters.AddWithValue("@ProjectId", employeeVm.ProjectId);
                    cmdempBonusDet.Parameters.AddWithValue("@DepartmentId", employeeVm.DepartmentId);
                    cmdempBonusDet.Parameters.AddWithValue("@SectionId", employeeVm.SectionId);
                    cmdempBonusDet.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdempBonusDet.Parameters.AddWithValue("@GrossSalary", employeeVm.GrossSalary);
                    cmdempBonusDet.Parameters.AddWithValue("@BasicSalary", employeeVm.BasicSalary);
                    cmdempBonusDet.Parameters.AddWithValue("@BonusValue", bonusValue);
                    cmdempBonusDet.Parameters.AddWithValue("@Remarks", "--");
                    cmdempBonusDet.Parameters.AddWithValue("@IsActive", true);
                    cmdempBonusDet.Parameters.AddWithValue("@IsArchive", false);
                    cmdempBonusDet.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdempBonusDet.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdempBonusDet.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdempBonusDet.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This employee out of date range";
                    throw new ArgumentNullException("This employee out of Joining Date range", "This employee out of Joining Date range");
                }
                #endregion

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
                retResults[1] = "Data Update Successfully.";

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
        //==================Delete =================
       
        public string[] EmployeeBonusDetailsDelete(EmployeeBonusDetailVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteBonusDetails"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToBonusD"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeBonusDetail set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }




                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Bonus Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Bonus Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Bonus Information.";
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
        
        public string[] EmployeeBonusDelete(EmployeeBonusVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteBonus"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToBonusD"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeBonus set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn,transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                        #region Details
                        sqlText = "";
                        sqlText = "update EmployeeBonusDetail set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where EmployeeBonusId=@Id";

                        SqlCommand cmdUpdate2 = new SqlCommand(sqlText, currConn,transaction);
                        cmdUpdate2.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate2.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate2.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate2.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate2.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

					    exeRes = cmdUpdate2.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                        #endregion
                    }




                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Bonus Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Bonus Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Bonus Information.";
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

        //==================SelectAllForReport =================
        public List<EmployeeBonusVM> SelectAllForReport(int BranchId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeBonusVM> VMs = new List<EmployeeBonusVM>();
            EmployeeBonusVM vm;
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

                sqlText = @"select 
 eb.Id
,eb.Code
,eb.Name
,eb.BranchId
,eb.BonusStructureId
,eb.IsFixed
,eb.PortionSalaryType
,eb.BonusValue
,eb.Remarks
,bs.Name BonusStructure
from EmployeeBonus eb
left outer join BonusStructure bs on eb.BonusStructureId=bs.Id
Where eb.IsArchive=0 And eb.IsActive=1 And eb.BranchId=@BranchId";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@BranchId", BranchId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeBonusVM();
                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.BonusStructureId = dr["BonusStructureId"].ToString();
                    vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                    vm.PortionSalaryType = dr["PortionSalaryType"].ToString();
                    vm.BonusValue = Convert.ToDecimal(dr["BonusValue"]);
                    vm.BonusStructureName = dr["BonusStructure"].ToString();
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
