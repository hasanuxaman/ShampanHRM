using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
  public  class EmployeeLanguageRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeLanguageVM> SelectAll()
        {
            try
            {
                return new EmployeeLanguageDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAll=================
        public List<EmployeeLanguageVM> SelectAllByEmployee(string employeeId)
        {
            try
            {
                return new EmployeeLanguageDAL().SelectAllByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeLanguageVM SelectById(int Id)
        {
            try
            {
                return new EmployeeLanguageDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeLanguageVM employeeLanguageVM)
        {
            try
            {
                return new EmployeeLanguageDAL().Insert(employeeLanguageVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeLanguageVM employeeLanguageVM)
        {
            try
            {
                return new EmployeeLanguageDAL().Update(employeeLanguageVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeeLanguageVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeeLanguageDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeLanguageVM employeeLanguageVM, string[] ids)
        {
            try
            {
                return new EmployeeLanguageDAL().Delete(employeeLanguageVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAll=================
        public List<EmployeeLanguageVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeLanguageDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
