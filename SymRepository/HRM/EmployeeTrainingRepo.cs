using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;

namespace SymRepository.HRM
{
  public  class EmployeeTrainingRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeTrainingVM> SelectAll()
        {
            try
            {
                return new EmployeeTrainingDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectAllByEmployee=================
        public List<EmployeeTrainingVM> SelectAllByEmployee(string employeeId)
        {
            try
            {
                return new EmployeeTrainingDAL().SelectAllByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeTrainingVM SelectById(int Id)
        {
            try
            {
                return new EmployeeTrainingDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeTrainingVM employeeTrainingVM)
        {
            try
            {
                return new EmployeeTrainingDAL().Insert(employeeTrainingVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeTrainingVM employeeTrainingVM)
        {
            try
            {
                return new EmployeeTrainingDAL().Update(employeeTrainingVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeeTrainingVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeeTrainingDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeTrainingVM employeeTrainingVM, string[] ids)
        {
            try
            {
                return new EmployeeTrainingDAL().Delete(employeeTrainingVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeeTrainingVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeTrainingDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
