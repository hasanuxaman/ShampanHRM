using SymServices.HRM;
using SymServices.Leave;
using SymViewModel.HRM;
using SymViewModel.Leave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Leave
{
   public class EmployeeLeaveDetailRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeLeaveDetailVM> SelectAll()
        {
            try
            {
                return new EmployeeLeaveDetailDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectAllByEmployee=================
        public List<EmployeeLeaveDetailVM> SelectAllByEmployee(string employeeId)
        {
            try
            {
                return new EmployeeLeaveDetailDAL().SelectAllByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeLeaveDetailVM SelectById(int Id)
        {
            try
            {
                return new EmployeeLeaveDetailDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeLeaveDetailVM employeeLeaveDetailVM)
        {
            try
            {
                return new EmployeeLeaveDetailDAL().Insert(employeeLeaveDetailVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeLeaveDetailVM employeeLeaveDetailVM)
        {
            try
            {
                return new EmployeeLeaveDetailDAL().Update(employeeLeaveDetailVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeLeaveDetailVM employeeLeaveDetailVM)
        {
            try
            {
                return new EmployeeLeaveDetailDAL().Delete(employeeLeaveDetailVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
