using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
  public  class EmployeeNomineeRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeNomineeVM> SelectAll()
        {
            try
            {
                return new EmployeeNomineeDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAll=================
        public List<EmployeeNomineeVM> SelectAllByEmployee(string employeeId)
        {
            try
            {
                return new EmployeeNomineeDAL().SelectAllByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SelectAllEmployeeForExcel(string employeeId)
        {
            try
            {
                return new EmployeeNomineeDAL().SelectAllEmployeeForExcel(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectByID=================
        public EmployeeNomineeVM SelectById(int Id)
        {
            try
            {
                return new EmployeeNomineeDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeNomineeVM employeeNomineeVM)
        {
            try
            {
                return new EmployeeNomineeDAL().Insert(employeeNomineeVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeNomineeVM employeeNomineeVM)
        {
            try
            {
                return new EmployeeNomineeDAL().Update(employeeNomineeVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeeNomineeVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeeNomineeDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeNomineeVM employeeNomineeVM, string[] ids)
        {
            try
            {
                return new EmployeeNomineeDAL().Delete(employeeNomineeVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeeNomineeVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeNomineeDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
