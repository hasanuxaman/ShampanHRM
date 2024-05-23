using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;

namespace SymRepository.HRM
{
    public class EmployeeJobRepo
    {
        #region Methods
         //==================SelectAll=================
        public List<EmployeeJobVM> SelectAll()
        {
            try
            {
                return new EmployeeJobDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

        //==================SelectAllByEmployee=================
        public EmployeeJobVM SelectByEmployee(string employeeId)
        {
            try
            {
                return new EmployeeJobDAL().SelectByEmployee(employeeId,null,null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================Insert =================
        public string[] Insert(EmployeeJobVM employeeJobVM)
        {
            try
            {
                return new EmployeeJobDAL().Insert(employeeJobVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeJobVM employeeJobVM)
        {
            try
            {
                return new EmployeeJobDAL().Update(employeeJobVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

        //==================SelectAllForReport=================
        public List<EmployeeJobVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeJobDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
