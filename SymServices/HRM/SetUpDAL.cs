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
    public class SetUpDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion


        #region Methods
        //==================SelectAll=================
        public List<Events> SelectAllEvent()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<Events> VMs = new List<Events>();
            Events vm;
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

                sqlText = @"select EventId
,Subject
,Description
,format(StartTime,'dd-MMM-yyyy') StartTime
,format(EndTime,'dd-MMM-yyyy') EndTime
,ThemeColor
,IsFullDay
from Events

  
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new Events();
                    vm.EventId = Convert.ToInt32(dr["EventId"]);
                    vm.Subject = dr["Subject"].ToString();
                    vm.Description = dr["Description"].ToString();
                    vm.Start = dr["StartTime"].ToString();
                    vm.End = dr["EndTime"].ToString();
                    vm.ThemeColor = dr["ThemeColor"].ToString();
                    vm.IsFullDay = Convert.ToBoolean(dr["IsFullDay"]);
                    //vm.Id = Convert.ToInt32(dr["Id"]);
                    //vm.EmployeeId = dr["EmployeeId"].ToString();
                    //vm.Name = dr["Name"].ToString();
                    //vm.Relation = dr["Relation"].ToString();
                    //vm.Address = dr["Address"].ToString();
                    //vm.BirthReg = dr["BirthCertificateNo"].ToString();
                    //vm.District = dr["District"].ToString();
                    //vm.Division = dr["Division"].ToString();
                    //vm.Country = dr["Country"].ToString();
                    //vm.City = dr["City"].ToString();
                    //vm.PostalCode = dr["PostalCode"].ToString();
                    //vm.DateofBirth = Ordinary.StringToDate(dr["DateofBirth"].ToString());
                    //vm.Phone = dr["Phone"].ToString();
                    //vm.Mobile = dr["Mobile"].ToString();
                    //vm.Fax = dr["Fax"].ToString();
                    //vm.Remarks = dr["Remarks"].ToString();
                    //vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    //vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    //vm.CreatedBy = dr["CreatedBy"].ToString();
                    //vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    //vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
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


        public List<EmployeesHierarchyVM> SelectAllEmployee()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeesHierarchyVM> VMs = new List<EmployeesHierarchyVM>();
            EmployeesHierarchyVM vm;
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

                sqlText = @"select EmployeeId
,Name
,Designation
,ISNULL(ReportingManager,'') ReportingManager
from EmployeesHierarchy

  
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeesHierarchyVM();
                    vm.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                    vm.Name = dr["Name"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.ReportingManager = Convert.ToInt32(dr["ReportingManager"]);
              
                    //vm.Id = Convert.ToInt32(dr["Id"]);
                    //vm.EmployeeId = dr["EmployeeId"].ToString();
                    //vm.Name = dr["Name"].ToString();
                    //vm.Relation = dr["Relation"].ToString();
                    //vm.Address = dr["Address"].ToString();
                    //vm.BirthReg = dr["BirthCertificateNo"].ToString();
                    //vm.District = dr["District"].ToString();
                    //vm.Division = dr["Division"].ToString();
                    //vm.Country = dr["Country"].ToString();
                    //vm.City = dr["City"].ToString();
                    //vm.PostalCode = dr["PostalCode"].ToString();
                    //vm.DateofBirth = Ordinary.StringToDate(dr["DateofBirth"].ToString());
                    //vm.Phone = dr["Phone"].ToString();
                    //vm.Mobile = dr["Mobile"].ToString();
                    //vm.Fax = dr["Fax"].ToString();
                    //vm.Remarks = dr["Remarks"].ToString();
                    //vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    //vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    //vm.CreatedBy = dr["CreatedBy"].ToString();
                    //vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    //vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
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
    }
}
        #endregion