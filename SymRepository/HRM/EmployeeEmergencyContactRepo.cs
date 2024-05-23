using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
   public class EmployeeEmergencyContactRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeEmergencyContactVM> SelectAll()
        {
            try
            {
                return new EmployeeEmergencyContactDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeEmergencyContactVM SelectById(int Id)
        {
            try
            {
                return new EmployeeEmergencyContactDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByEmployeeID=================
        public EmployeeEmergencyContactVM SelectByEmployeeId(string EmployeeId)
        {
            try
            {
                return new EmployeeEmergencyContactDAL().SelectByEmployeeId(EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeEmergencyContactVM employeeEmergencyContactVM)
        {
            try
            {
                return new EmployeeEmergencyContactDAL().Insert(employeeEmergencyContactVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeEmergencyContactVM employeeEmergencyContactVM)
        {
            try
            {
                return new EmployeeEmergencyContactDAL().Update(employeeEmergencyContactVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeeEmergencyContactVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeeEmergencyContactDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeEmergencyContactVM employeeEmergencyContactVM)
        {
            try
            {
                return new EmployeeEmergencyContactDAL().Delete(employeeEmergencyContactVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeeEmergencyContactVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeEmergencyContactDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion
    }
}
