using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;

namespace SymRepository.HRM
{
  public  class EmployeeDependentRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeDependentVM> SelectAll()
        {
            try
            {
                return new EmployeeDependentDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAll=================
        public List<EmployeeDependentVM> SelectAllByEmployee(string employeeId)
        {
            try
            {
                return new EmployeeDependentDAL().SelectAllByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public EmployeeDependentVM SelectById(int Id)
        {
            try
            {
                return new EmployeeDependentDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeDependentVM empEducationVM)
        {
            try
            {
                return new EmployeeDependentDAL().Insert(empEducationVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeDependentVM educationVM)
        {
            try
            {
                return new EmployeeDependentDAL().Update(educationVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeeDependentVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeeDependentDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeDependentVM EmployeeDependentVM, string[] ids)
        {
            try
            {
                return new EmployeeDependentDAL().Delete(EmployeeDependentVM, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeeDependentVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeDependentDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
