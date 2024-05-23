using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;

namespace SymRepository.HRM
{
  public  class EmployeeTravelRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeTravelVM> SelectAll()
        {
            try
            {
                return new EmployeeTravelDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAllByEmployee=================
        public List<EmployeeTravelVM> SelectAllByEmployee(string EmployeeId)
        {
            try
            {
                return new EmployeeTravelDAL().SelectAllByEmployee(EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public EmployeeTravelVM SelectById(int Id)
        {
            try
            {
                return new EmployeeTravelDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeTravelVM employeeTravelVM)
        {
            try
            {
                return new EmployeeTravelDAL().Insert(employeeTravelVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeTravelVM employeeTravelVM)
        {
            try
            {
                return new EmployeeTravelDAL().Update(employeeTravelVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeeTravelVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeeTravelDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeTravelVM employeeTravelVM, string[] ids)
        {
            try
            {
                return new EmployeeTravelDAL().Delete(employeeTravelVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeeTravelVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeTravelDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
