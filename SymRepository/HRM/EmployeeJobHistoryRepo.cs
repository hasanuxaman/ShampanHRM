using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
  public  class EmployeeJobHistoryRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeJobHistoryVM> SelectAll()
        {
            try
            {
                return new EmployeeJobHistoryDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectAll=================
        public List<EmployeeJobHistoryVM> SelectAllByEmployee(string employeeId)
        {
            try
            {
                return new EmployeeJobHistoryDAL().SelectAllByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeJobHistoryVM SelectById(int Id)
        {
            try
            {
                return new EmployeeJobHistoryDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeJobHistoryVM employeeJobHistoryVM)
        {
            try
            {
                return new EmployeeJobHistoryDAL().Insert(employeeJobHistoryVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeJobHistoryVM employeeJobHistoryVM)
        {
            try
            {
                return new EmployeeJobHistoryDAL().Update(employeeJobHistoryVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(EmployeeJobHistoryVM employeeJobHistoryVM, string[] ids)
        {
            try
            {
                return new EmployeeJobHistoryDAL().Delete(employeeJobHistoryVM, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAllForReport=================
        public List<EmployeeJobHistoryVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeJobHistoryDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
