using SymServices.HRM;
using SymServices.Leave;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Leave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Leave
{
    public class EmployeeLeaveStructureRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeLeaveStructureVM> SelectAll()
        {
            try
            {
                return new EmployeeLeaveStructureDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<EmployeeLeaveStructureVM> DropDown(string employeeId, int year)
        {
            try
            {
                return new EmployeeLeaveStructureDAL().DropDown(employeeId, year);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeLeaveStructureVM> DropDown(int year)
        {
            try
            {
                return new EmployeeLeaveStructureDAL().DropDown(year);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string CheckEmployeeLeaveBalance(string employeeId, string leaveType, int year, decimal totalDay)
        {
            try
            {
                return new EmployeeLeaveStructureDAL().CheckEmployeeLeaveBalance(employeeId, leaveType, year, totalDay);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAll=================
        public List<EmployeeLeaveStructureVM> SelectAllByEmployeeId(string EmployeeId)
        {
            try
            {
                return new EmployeeLeaveStructureDAL().SelectAllByEmployee(EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeLeaveStructureVM SelectById(int Id)
        {
            try
            {
                return new EmployeeLeaveStructureDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeLeaveStructureVM vm)
        {
            try
            {
                return new EmployeeLeaveStructureDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeLeaveStructureVM LeaveStructureVM)
        {
            try
            {
                return new EmployeeLeaveStructureDAL().Update(LeaveStructureVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeeLeaveStructureVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeeLeaveStructureDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeLeaveStructureVM employeeLeaveStructureVM)
        {
            try
            {
                return new EmployeeLeaveStructureDAL().Delete(employeeLeaveStructureVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] LeaveMigrationInsert(EmployeeLeaveStructureVM vm, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeLeaveStructureDAL().LeaveMigrationInsert(vm, siVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
