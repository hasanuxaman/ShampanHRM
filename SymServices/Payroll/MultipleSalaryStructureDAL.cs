using SymOrdinary;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Payroll
{
  public  class MultipleSalaryStructureDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        #region Methods
        //==================SelectAll=================
        //public List<QCVM> SelectAll(string mgfOrganizationId = null, string supplierOrganizationId = null, string localSupplierOrganizationId = null)

        public List<MultipleSalaryStructureVM> SelectAll(string empid = null, int? fid = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<MultipleSalaryStructureVM> vms = new List<MultipleSalaryStructureVM>();
            MultipleSalaryStructureVM vm;
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
ea.Id
,ea.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,ea.AreerAmount
,ea.FiscalYearDetailId
,fs.PeriodName
,ea.AreerDate
,e.DesignationId, e.DepartmentId, e.SectionId, e.ProjectId
,ea.Remarks
,ea.IsActive
,ea.IsArchive
,ea.CreatedBy
,ea.CreatedAt
,ea.CreatedFrom
,ea.LastUpdateBy
,ea.LastUpdateAt
,ea.LastUpdateFrom
From EmployeeAreer ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
left outer join  FiscalYearDetail fs on ea.FiscalYearDetailId=fs.Id
Where 1=1 and  ea.IsArchive=0
";
                if (!string.IsNullOrEmpty(empid))
                {
                    sqlText += @" and ea.EmployeeId='" + empid + "'";
                }

                if (fid != null && fid != 0)
                {
                    sqlText += @" and ea.FiscalYearDetailId='" + fid + "'";
                }


                sqlText += @" ORDER BY e.Department,e.EmpName desc";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(empid))
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", empid);
                }
                if (fid != null && fid != 0)
                {
                    objComm.Parameters.AddWithValue("@FiscalYearDetailId", fid);
                }


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new MultipleSalaryStructureVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.AreerAmount = Convert.ToDecimal(dr["AreerAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.AreerDate = Ordinary.StringToDate(dr["AreerDate"].ToString());

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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


                    vms.Add(vm);
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

            return vms;
        }
        //==================SelectByID=================
        public MultipleSalaryStructureVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            MultipleSalaryStructureVM vm = new MultipleSalaryStructureVM();

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
ea.Id
,ea.EmployeeId
,e.EmpName,e.Code,e.Designation,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,ea.AreerAmount
,ea.FiscalYearDetailId
,ea.AreerDate
,ea.Remarks
,ea.IsActive
,ea.IsArchive
,ea.CreatedBy
,ea.CreatedAt
,ea.CreatedFrom
,ea.LastUpdateBy
,ea.LastUpdateAt
,ea.LastUpdateFrom

From EmployeeAreer ea 
left outer join ViewEmployeeInformation e on ea.EmployeeId=e.id
Where ea.IsArchive=0  and ea.id=@Id
ORDER BY e.Department, e.EmpName desc
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
                    vm = new MultipleSalaryStructureVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.AreerAmount = Convert.ToDecimal(dr["AreerAmount"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.AreerDate = Ordinary.StringToDate(dr["AreerDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        public string[] Insert(EmployeeInfoVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeAreer"; //Method Name


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
               

                #endregion Exist
             
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
                retResults[2] = vm.Id.ToString();

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

        public List<MultipleSalaryStructureVM> SalaryTypeList()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<MultipleSalaryStructureVM> VMs = new List<MultipleSalaryStructureVM>();
            MultipleSalaryStructureVM vm;
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

                sqlText = @"select Id, Name from EnumSalaryType
where 1=1
--and TypeName in('other') 
and id in(
select distinct SalaryTypeId from EmployeeSalaryStructureDetail)
order by TypeName

";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new MultipleSalaryStructureVM();
                    vm.SalaryTypeId = dr["Id"].ToString();
                    vm.SalaryTypeName = dr["Name"].ToString();
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
