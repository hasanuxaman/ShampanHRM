using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
   public class EmployeeRepo
    {
       EmployeeDAL _employeeDAL = new EmployeeDAL();
        #region Method

        //==================SelectAllEmployees Employee=================
        public List<EmployeeVM> SelectAllEmployee()
        {
            List<EmployeeVM> dataSet = new List<EmployeeVM>();
            try
            {
                dataSet = _employeeDAL.SelectAllEmployee();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataSet;

        }

        //==================Insert Employee=================
        public string[] EmployeeDataInsert(EmployeeVM employees, SqlConnection currConn, SqlTransaction transaction)
        {
            string[] retResults = new string[6];
            try
            {
                retResults = _employeeDAL.EmployeeDataInsert(employees, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            #region Results

            return retResults;
            #endregion


        }

        //==================Update Employee=================
        public string[] EmployeeUpdate(EmployeeVM employeeVM)
        {
            string[] retResults = new string[6];
            try
            {
                retResults = _employeeDAL.EmployeeUpdate(employeeVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retResults;
        }
        //==================Select Employee=================
        public EmployeeVM SelectEmployee(int Id)
        {
            return _employeeDAL.SelectSingleEmployee( Id);
        }

        //==================Select Employee=================
        public EmployeeVM SelectSingleEmployeeName(string logid)
        {
            return _employeeDAL.SelectSingleEmployeeName(logid);
        }
        public System.Data.DataTable Nomeniee()
        {
            return _employeeDAL.EmployeeNomeniee();
        }
   
        #endregion              
    
        public System.Data.DataTable Dependent()
        {
            return _employeeDAL.EmployeeDependent();
        }
    }
}
