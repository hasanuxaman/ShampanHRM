using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;

namespace SymRepository.HRM
{
  public  class EmployeeImmigrationRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeImmigrationVM> SelectAll()
        {
            try
            {
                return new EmployeeImmigrationDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAllByEmployeeId
        public List<EmployeeImmigrationVM> SelectAllByEmployee(string EmployeeId)
        {
            try
            {
                return new EmployeeImmigrationDAL().SelectAllByEmployee(EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeImmigrationVM SelectById(int Id)
        {
            try
            {
                return new EmployeeImmigrationDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeImmigrationVM employeeImmigrationVM)
        {
            try
            {
                return new EmployeeImmigrationDAL().Insert(employeeImmigrationVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeImmigrationVM employeeImmigrationVM)
        {
            try
            {
                return new EmployeeImmigrationDAL().Update(employeeImmigrationVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeeImmigrationVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeeImmigrationDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeImmigrationVM employeeImmigrationVM, string[] ids)
        {
            try
            {
                return new EmployeeImmigrationDAL().Delete(employeeImmigrationVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeeImmigrationVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeImmigrationDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
