using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;

namespace SymRepository.HRM
{
  public  class EmployeeReferenceRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeReferenceVM> SelectAll()
        {
            try
            {
                return new EmployeeReferenceDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAllByEmployee=================
        public List<EmployeeReferenceVM> SelectAllByEmployee(string employeeId)
        {
            try
            {
                return new EmployeeReferenceDAL().SelectAllByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public EmployeeReferenceVM SelectById(int Id)
        {
            try
            {
                return new EmployeeReferenceDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeReferenceVM employeeReferenceVM)
        {
            try
            {
                return new EmployeeReferenceDAL().Insert(employeeReferenceVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeReferenceVM employeeReferenceVM)
        {
            try
            {
                return new EmployeeReferenceDAL().Update(employeeReferenceVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeeReferenceVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeeReferenceDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeReferenceVM employeeReferenceVM, string[] ids)
        {
            try
            {
                return new EmployeeReferenceDAL().Delete(employeeReferenceVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAllForReport=================
        public List<EmployeeReferenceVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeReferenceDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
