using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
   public class EmployeePresentAddressRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeePresentAddressVM> SelectAll()
        {
            try
            {
                return new EmployeePresentAddressDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeePresentAddressVM SelectByEmployeeId(string EmployeeId)
        {
            try
            {
                return new EmployeePresentAddressDAL().SelectByEmployeeId(EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeePresentAddressVM employeePresentAddressVM)
        {
            try
            {
                return new EmployeePresentAddressDAL().Insert(employeePresentAddressVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeePresentAddressVM employeePresentAddressVM)
        {
            try
            {
                return new EmployeePresentAddressDAL().Update(employeePresentAddressVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeePresentAddressVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeePresentAddressDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeePresentAddressVM employeePresentAddressVM)
        {
            try
            {
                return new EmployeePresentAddressDAL().Delete(employeePresentAddressVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeePresentAddressVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeePresentAddressDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
